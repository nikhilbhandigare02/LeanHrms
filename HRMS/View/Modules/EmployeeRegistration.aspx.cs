using DataObject;
using ProcessModel;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Script.Services;
using System.Web.Script.Serialization;
using System.Web.Services;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;

namespace HRMS.View.Modules
{
    public partial class EmployeeRegistration : Page
    {
        protected string UserId = null;
        private readonly EmployeeOnboardingBL onboardingBL = new EmployeeOnboardingBL();
        private const string DefaultEmployeePassword = "Pass@123";

        protected void Page_Load(object sender, EventArgs e)
        {
            UserId = Convert.ToString(Session["userId"]);
            if (string.Equals(Request.QueryString["action"], "checkDuplicate", StringComparison.OrdinalIgnoreCase))
            {
                WriteDuplicateValidationResponse();
                return;
            }

            if (!IsPostBack)
            {
                BindLookupDropdowns();
            }
        }

        private void WriteDuplicateValidationResponse()
        {
            string fieldName = Convert.ToString(Request.QueryString["fieldName"] ?? string.Empty);
            string fieldValue = Convert.ToString(Request.QueryString["fieldValue"] ?? string.Empty);
            string message = onboardingBL.CheckEmployeeOnboardingDuplicate(fieldName, fieldValue, 0);
            bool isDuplicate = !string.IsNullOrWhiteSpace(message);

            Response.Clear();
            Response.ContentType = "application/json";
            Response.Write("{\"IsDuplicate\":" + (isDuplicate ? "true" : "false") + ",\"Message\":\"" + HttpUtility.JavaScriptStringEncode(message) + "\"}");
            Response.End();
        }

        private void BindLookupDropdowns()
        {
            BindLookup(ddlGender, "Gender");
            BindLookup(ddlMaritalStatus, "Marital Status");
            BindLookup(ddlBloodGroup, "Blood Group");
            BindLookup(ddlNationality, "Nationality");
            BindLookup(ddlEmploymentType, "Emptype");
            BindLookup(ddlEmployeeCategory, "Emp Category");
            BindLookupValue(ddlProbationPeriod, "probation_month");
            BindLookup(ddlEmployeeStatus, "EmpStatus");
            BindLookup(ddlSeparationReason, "Separation Reason");
            BindProcedureDropdown(ddlCompany, "Sp_BindDropdownCompany");
            BindLookup(ddlDepartment, "Department");
            BindProcedureDropdown(ddlDesignation, "sp_bindDesignations");
            BindProcedureDropdown(ddlReportingManager, "sp_bindassignby");
            BindProcedureDropdown(ddlFunctionalManager, "get_hod_fun_manager", "Functional Manager");
            BindProcedureDropdown(ddlHod, "get_hod_fun_manager", "HOD");
            BindLookup(ddlEmployeeLevel, "Employee Level");
            BindLookup(ddlAttendanceType, "Attendance Type");
            BindLookup(ddlWeeklyOff, "Weekly Off");
            BindLookup(ddlAttendancePolicy, "Attendance Policy");
            BindLookup(ddlWorkLocation, "Work Location");
            BindLookup(ddlAssetCondition, "Asset Condition");
            BindLookup(ddlAssetStatus, "Asset Status");
        }

        private void BindLookup(DropDownList ddl, string lookupType)
        {
            ddl.DataSource = onboardingBL.BindLookupData(lookupType);
            ddl.DataTextField = "Text";
            ddl.DataValueField = "Id";
            ddl.DataBind();
            AddDisabledSelectOption(ddl);
        }

        private void BindLookupValue(DropDownList ddl, string lookupType)
        {
            ddl.DataSource = onboardingBL.BindLookupData(lookupType);
            ddl.DataTextField = "Text";
            ddl.DataValueField = "Value";
            ddl.DataBind();
            AddDisabledSelectOption(ddl);
        }

        private void BindProcedureDropdown(DropDownList ddl, string storedProcedureName, string lookupType = null)
        {
            ddl.DataSource = onboardingBL.BindStoredProcedureDropdown(storedProcedureName, lookupType);
            ddl.DataTextField = "Text";
            ddl.DataValueField = "Id";
            ddl.DataBind();
            AddDisabledSelectOption(ddl);
        }

        private void AddDisabledSelectOption(DropDownList ddl)
        {
            ListItem selectItem = new ListItem("-- Select --", string.Empty);
            selectItem.Attributes["disabled"] = "disabled";
            selectItem.Attributes["hidden"] = "hidden";
            ddl.Items.Insert(0, selectItem);
            ddl.ClearSelection();
            ddl.Items[0].Selected = true;
        }

        protected void SubmitEmployee_Click(object sender, EventArgs e)
        {
            try
            {
                string validationMessage = ValidateMandatoryFields();
                if (!string.IsNullOrWhiteSpace(validationMessage))
                {
                    ClientScript.RegisterStartupScript(
                        GetType(),
                        "EmployeeOnboardingValidation",
                        "if(window.Swal){Swal.fire({icon:'warning',title:'Please fill mandatory fields',text:'" + HttpUtility.JavaScriptStringEncode(validationMessage) + "',confirmButtonColor:'#2563EB'});}else{alert('" + HttpUtility.JavaScriptStringEncode(validationMessage) + "');}",
                        true);
                    return;
                }

                EmployeeOnboardingDO employee = BuildEmployeeOnboarding();
                EmployeeOnboardingResponseDO response = onboardingBL.SaveEmployeeOnboarding(employee);

                if (string.Equals(response.Status, "Success", StringComparison.OrdinalIgnoreCase))
                {
                    List<EmployeeAssetDO> assets = GetSubmittedAssets();
                    foreach (EmployeeAssetDO asset in assets)
                    {
                        EmployeeOnboardingResponseDO assetResponse = onboardingBL.SaveEmployeeAsset(response.UserId, asset, employee.InsertedBy);
                        if (!string.Equals(assetResponse.Status, "Success", StringComparison.OrdinalIgnoreCase))
                        {
                            string assetMessage = HttpUtility.JavaScriptStringEncode(string.IsNullOrWhiteSpace(assetResponse.Message)
                                ? "Employee asset save failed."
                                : assetResponse.Message);

                            ClientScript.RegisterStartupScript(
                                GetType(),
                                "EmployeeAssetSaveFailed",
                                "if(window.Swal){Swal.fire({icon:'error',title:'Employee onboarding save failed',text:'" + assetMessage + "',confirmButtonColor:'#2563EB'});}else{alert('" + assetMessage + "');}",
                                true);
                            return;
                        }
                    }

                    string generatedPassword = DefaultEmployeePassword;

                    if (chkSendCredentials.Checked && !string.IsNullOrWhiteSpace(employee.Email))
                    {
                        try
                        {
                            UserDetailsBL userDetailsBL = new UserDetailsBL();
                            userDetailsBL.SendUserCredentialsMail(employee.Email, employee.Username, generatedPassword);
                        }
                        catch (Exception mailEx)
                        {
                            CommonBL errorlog = new CommonBL();
                            errorlog.fnStoreErrorLog("EmployeeRegistration", "SubmitEmployee_Click_SendMail", "Exception Message: " + mailEx.Message + " StackTrace: " + mailEx.StackTrace, UserId);
                        }
                    }

                    litEmployeeId.Text = response.UserId.ToString();
                    litUsername.Text = HttpUtility.HtmlEncode(employee.Username);
                    litPassword.Text = HttpUtility.HtmlEncode(generatedPassword);

                    string script = @"
                        if (document.getElementById('statusText')) {
                            document.getElementById('statusText').textContent = 'Employee created';
                        }

                        if (window.Swal) {
                            Swal.fire({
                                icon: 'success',
                                title: 'Employee Registered Successfully',
                                confirmButtonText: 'Go To Employee List',
                                showDenyButton: true,
                                denyButtonText: 'Create Another',
                                confirmButtonColor: '#2563EB',
                                denyButtonColor: '#64748B',
                                allowOutsideClick: true,
                                timer: 1500,
                                timerProgressBar: true
                            }).then(function (result) {
                                if (result.isConfirmed) {
                                    window.location.href = '/View/Modules/EmployeeList.aspx';
                                } else {
                                    window.location.href = '/View/Modules/EmployeeRegistration.aspx';
                                }
                            });
                        } else {
                            alert('Employee Registered Successfully');
                            window.location.href = '/View/Modules/EmployeeRegistration.aspx';
                        }";
                    ClientScript.RegisterStartupScript(GetType(), "EmployeeOnboardingSaved", script, true);
                    return;
                }

                string message = HttpUtility.JavaScriptStringEncode(string.IsNullOrWhiteSpace(response.Message)
                    ? "Employee onboarding save failed."
                    : response.Message);

                ClientScript.RegisterStartupScript(
                    GetType(),
                    "EmployeeOnboardingFailed",
                    "if(window.Swal){Swal.fire({icon:'error',title:'Employee onboarding save failed',text:'" + message + "',confirmButtonColor:'#2563EB'});}else{alert('" + message + "');}",
                    true);
            }
            catch (Exception ex)
            {
                CommonBL errorlog = new CommonBL();
                errorlog.fnStoreErrorLog("EmployeeRegistration", "SubmitEmployee_Click", "Exception Message: " + ex.Message + " StackTrace: " + ex.StackTrace, UserId);

                ClientScript.RegisterStartupScript(
                    GetType(),
                    "EmployeeOnboardingException",
                    "if(window.Swal){Swal.fire({icon:'error',title:'Employee onboarding save failed',text:'Please try again or contact support.',confirmButtonColor:'#2563EB'});}else{alert('Employee onboarding save failed. Please try again or contact support.');}",
                    true);
            }
        }

        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public static EmployeeDuplicateValidationResponse CheckEmployeeDuplicate(string fieldName, string fieldValue)
        {
            EmployeeDuplicateValidationResponse response = new EmployeeDuplicateValidationResponse
            {
                IsDuplicate = false,
                Message = string.Empty
            };

            try
            {
                EmployeeOnboardingBL onboardingBL = new EmployeeOnboardingBL();
                string message = onboardingBL.CheckEmployeeOnboardingDuplicate(fieldName, fieldValue, 0);
                response.IsDuplicate = !string.IsNullOrWhiteSpace(message);
                response.Message = message;
            }
            catch
            {
                response.IsDuplicate = false;
                response.Message = string.Empty;
            }

            return response;
        }

        public class EmployeeDuplicateValidationResponse
        {
            public bool IsDuplicate { get; set; }
            public string Message { get; set; }
        }

        private string ValidateMandatoryFields()
        {
            if (string.IsNullOrWhiteSpace(ValueOf(txtEmployeeCode))) return "Employee Code is required.";
            if (string.IsNullOrWhiteSpace(ValueOf(txtUsername))) return "Username is required.";
            if (string.IsNullOrWhiteSpace(ValueOf(txtFirstName))) return "First Name is required.";
            if (!Regex.IsMatch(ValueOf(txtFirstName), @"^[A-Za-z ]+$")) return "Write name in correct format.";
            if (!string.IsNullOrWhiteSpace(ValueOf(txtMiddleName)) && !Regex.IsMatch(ValueOf(txtMiddleName), @"^[A-Za-z ]+$")) return "Write name in correct format.";
            if (string.IsNullOrWhiteSpace(ValueOf(txtLastName))) return "Last Name is required.";
            if (!Regex.IsMatch(ValueOf(txtLastName), @"^[A-Za-z ]+$")) return "Write name in correct format.";
            if (string.IsNullOrWhiteSpace(ValueOf(txtEmail))) return "Email ID is required.";
            if (!Regex.IsMatch(ValueOf(txtEmail), @"^[^\s@]+@[^\s@]+\.[^\s@]{2,}$")) return "Enter a valid Email ID.";
            if (string.IsNullOrWhiteSpace(ValueOf(txtMobileNumber))) return "Mobile Number is required.";
            if (!Regex.IsMatch(ValueOf(txtMobileNumber), @"^\d{10}$")) return "Mobile Number must be 10 digits.";
            if (string.IsNullOrWhiteSpace(SelectedValueOf(ddlGender))) return "Gender is required.";
            if (string.IsNullOrWhiteSpace(ValueOf(txtDateOfBirth))) return "Date of Birth is required.";
            if (string.IsNullOrWhiteSpace(SelectedValueOf(ddlNationality))) return "Nationality is required.";
            if (string.IsNullOrWhiteSpace(SelectedValueOf(ddlMaritalStatus))) return "Marital Status is required.";
            if (string.IsNullOrWhiteSpace(SelectedValueOf(ddlBloodGroup))) return "Blood Group is required.";
            if (string.IsNullOrWhiteSpace(SelectedValueOf(ddlEmploymentType))) return "Employment Type is required.";
            if (string.IsNullOrWhiteSpace(SelectedValueOf(ddlEmployeeCategory))) return "Employee Category is required.";
            if (string.IsNullOrWhiteSpace(ValueOf(txtJoiningDate))) return "Joining Date is required.";
            if (string.IsNullOrWhiteSpace(SelectedValueOf(ddlProbationPeriod))) return "Probation Period is required.";
            if (string.IsNullOrWhiteSpace(SelectedValueOf(ddlEmployeeStatus))) return "Employee Status is required.";
            if (string.IsNullOrWhiteSpace(SelectedValueOf(ddlCompany))) return "Company is required.";
            if (string.IsNullOrWhiteSpace(SelectedValueOf(ddlDepartment))) return "Department is required.";
            if (string.IsNullOrWhiteSpace(ValueOf(txtBranchOffice))) return "Branch Office is required.";
            if (string.IsNullOrWhiteSpace(ValueOf(txtLocation))) return "Location is required.";
            if (string.IsNullOrWhiteSpace(SelectedValueOf(ddlDesignation))) return "Designation is required.";
            if (string.IsNullOrWhiteSpace(SelectedValueOf(ddlReportingManager))) return "Reporting Manager is required.";
            if (string.IsNullOrWhiteSpace(SelectedValueOf(ddlFunctionalManager))) return "Functional Manager is required.";
            if (string.IsNullOrWhiteSpace(SelectedValueOf(ddlHod))) return "HOD is required.";
            if (string.IsNullOrWhiteSpace(SelectedValueOf(ddlAttendanceType))) return "Attendance Type is required.";
            if (string.IsNullOrWhiteSpace(SelectedValueOf(ddlWeeklyOff))) return "Weekly Off is required.";
            if (string.IsNullOrWhiteSpace(ValueOf(txtWorkingHours))) return "Working Hours is required.";
            if (!Regex.IsMatch(ValueOf(txtWorkingHours), @"^([0-9]|[01][0-9]|2[0-3]):[0-5][0-9]$")) return "Working Hours must be in HH:mm format.";
            if (string.IsNullOrWhiteSpace(SelectedValueOf(ddlAttendancePolicy))) return "Attendance Policy is required.";
            if (string.IsNullOrWhiteSpace(ValueOf(txtPunchingDeviceId))) return "Punching Device ID is required.";
            if (string.IsNullOrWhiteSpace(ValueOf(txtBiometricId))) return "Biometric ID is required.";
            if (chkOvertimeEligible.Checked && string.IsNullOrWhiteSpace(ValueOf(txtOvertimeRate))) return "Overtime Rate is required.";
            if (string.IsNullOrWhiteSpace(SelectedValueOf(ddlWorkLocation))) return "Work Location is required.";
            string assetValidationMessage = ValidateSubmittedAssets();
            if (!string.IsNullOrWhiteSpace(assetValidationMessage)) return assetValidationMessage;

            return string.Empty;
        }

        private string ValidateSubmittedAssets()
        {
            List<EmployeeAssetDO> assets = GetSubmittedAssets();
            if (assets.Count == 0)
            {
                return "Asset Type is required.";
            }

            for (int i = 0; i < assets.Count; i++)
            {
                EmployeeAssetDO asset = assets[i];
                string suffix = assets.Count > 1 ? " for asset row " + (i + 1) : string.Empty;

                if (string.IsNullOrWhiteSpace(asset.AssetType)) return "Asset Type is required" + suffix + ".";
                if (string.IsNullOrWhiteSpace(asset.AssetNumber)) return "Asset Number is required" + suffix + ".";
                if (string.IsNullOrWhiteSpace(asset.AssetName)) return "Asset Name is required" + suffix + ".";
                if (!asset.AssignedDate.HasValue) return "Assigned Date is required" + suffix + ".";
                if (string.IsNullOrWhiteSpace(asset.AssetCondition)) return "Asset Condition is required" + suffix + ".";
                if (string.IsNullOrWhiteSpace(asset.AssetStatus)) return "Asset Status is required" + suffix + ".";
            }

            return string.Empty;
        }

        private List<EmployeeAssetDO> GetSubmittedAssets()
        {
            List<EmployeeAssetDO> assets = new List<EmployeeAssetDO>();
            string assetsJson = hdnAssetsJson == null ? string.Empty : Convert.ToString(hdnAssetsJson.Value ?? string.Empty);

            if (!string.IsNullOrWhiteSpace(assetsJson))
            {
                try
                {
                    JavaScriptSerializer serializer = new JavaScriptSerializer();
                    List<SubmittedAssetRow> submittedAssets = serializer.Deserialize<List<SubmittedAssetRow>>(assetsJson);
                    if (submittedAssets != null)
                    {
                        foreach (SubmittedAssetRow submittedAsset in submittedAssets)
                        {
                            assets.Add(new EmployeeAssetDO
                            {
                                AssetType = Convert.ToString(submittedAsset.asset_type ?? string.Empty).Trim(),
                                AssetNumber = Convert.ToString(submittedAsset.asset_number ?? string.Empty).Trim(),
                                AssetName = Convert.ToString(submittedAsset.asset_name ?? string.Empty).Trim(),
                                AssignedDate = ParseDate(submittedAsset.assigned_date),
                                ReturnDate = ParseDate(submittedAsset.return_date),
                                AssetCondition = Convert.ToString(submittedAsset.asset_condition ?? string.Empty).Trim(),
                                AssetStatus = Convert.ToString(submittedAsset.asset_status ?? string.Empty).Trim()
                            });
                        }
                    }
                }
                catch (Exception ex)
                {
                    CommonBL errorlog = new CommonBL();
                    errorlog.fnStoreErrorLog("EmployeeRegistration", "GetSubmittedAssets", "Exception Message: " + ex.Message + " StackTrace: " + ex.StackTrace, UserId);
                }
            }

            if (assets.Count == 0)
            {
                assets.Add(new EmployeeAssetDO
                {
                    AssetType = ValueOf(txtAssetType),
                    AssetNumber = ValueOf(txtAssetNumber),
                    AssetName = ValueOf(txtAssetName),
                    AssignedDate = ParseDate(ValueOf(txtAssignedDate)),
                    ReturnDate = ParseDate(ValueOf(txtReturnDate)),
                    AssetCondition = SelectedValueOf(ddlAssetCondition),
                    AssetStatus = SelectedValueOf(ddlAssetStatus)
                });
            }

            return assets;
        }

        private class SubmittedAssetRow
        {
            public string asset_type { get; set; }
            public string asset_number { get; set; }
            public string asset_name { get; set; }
            public string assigned_date { get; set; }
            public string return_date { get; set; }
            public string asset_condition { get; set; }
            public string asset_status { get; set; }
        }

        private EmployeeOnboardingDO BuildEmployeeOnboarding()
        {
            string fullName = string.Join(" ", new[]
            {
                ValueOf(txtFirstName),
                ValueOf(txtMiddleName),
                ValueOf(txtLastName)
            }).Replace("  ", " ").Trim();

            return new EmployeeOnboardingDO
            {
                UserId = ParseInt(Request.QueryString["user_id"]),
                EmployeeCode = ValueOf(txtEmployeeCode),
                Username = ValueOf(txtUsername),
                FullName = fullName,
                Email = ValueOf(txtEmail),
                Contact = ValueOf(txtMobileNumber),
                Gender = SelectedValueOf(ddlGender),
                DateOfBirth = ParseDate(ValueOf(txtDateOfBirth)),
                MaritalStatus = SelectedValueOf(ddlMaritalStatus),
                BloodGroup = SelectedValueOf(ddlBloodGroup),
                Nationality = SelectedValueOf(ddlNationality),
                EmploymentType = SelectedValueOf(ddlEmploymentType),
                EmployeeCategory = SelectedValueOf(ddlEmployeeCategory),
                JoiningDate = ParseDate(ValueOf(txtJoiningDate)),
                ConfirmationDate = ParseDate(ValueOf(txtConfirmationDate)),
                ProbationEndDate = ParseDate(ValueOf(txtProbationEndDate)),
                RetirementDate = null,
                ProbationMonths = ParseInt(SelectedValueOf(ddlProbationPeriod)),
                EmployeeStatus = SelectedValueOf(ddlEmployeeStatus),
                NoticePeriod = ParseInt(ValueOf(txtNoticePeriod)),
                ExitDate = ParseDate(ValueOf(txtExitDate)),
                SeparationReason = SelectedValueOf(ddlSeparationReason),
                Company = SelectedValueOf(ddlCompany),
                Department = SelectedValueOf(ddlDepartment),
                BranchOffice = ValueOf(txtBranchOffice),
                Location = ValueOf(txtLocation),
                ReportingManagerId = ParseInt(ddlReportingManager.SelectedValue),
                FunctionalManagerId = ParseInt(ddlFunctionalManager.SelectedValue),
                HodId = ParseInt(ddlHod.SelectedValue),
                Designation = SelectedValueOf(ddlDesignation),
                EmployeeLevel = SelectedValueOf(ddlEmployeeLevel),
                AttendanceType = SelectedValueOf(ddlAttendanceType),
                WeeklyOff = SelectedValueOf(ddlWeeklyOff),
                WorkingHours = NormalizeWorkingHours(ValueOf(txtWorkingHours)),
                AttendancePolicy = SelectedValueOf(ddlAttendancePolicy),
                PunchingDeviceId = ValueOf(txtPunchingDeviceId),
                BiometricId = ValueOf(txtBiometricId),
                OvertimeEligible = chkOvertimeEligible.Checked ? "60" : "61",
                OvertimeRate = chkOvertimeEligible.Checked ? ParseDecimal(ValueOf(txtOvertimeRate)) : 0,
                WorkLocation = SelectedValueOf(ddlWorkLocation),
                RemoteWorkAllowed = "0",
                // Payroll is handled outside Employee Registration, so no payroll values are submitted here.
                // SalaryStructure = string.Empty,
                // BasicSalary = 0,
                // GrossSalary = 0,
                // Ctc = 0,
                // SalaryEffectiveDate = null,
                // PaymentMode = string.Empty,
                // UanNumber = string.Empty,
                // PfNumber = string.Empty,
                // EsicNumber = string.Empty,
                // ProfessionalTaxNumber = string.Empty,
                // LabourWelfareFundNumber = string.Empty,
                // TaxRegime = string.Empty,
                // TdsApplicable = string.Empty,
                AssetType = ValueOf(txtAssetType),
                AssetNumber = ValueOf(txtAssetNumber),
                AssetName = ValueOf(txtAssetName),
                AssignedDate = ParseDate(ValueOf(txtAssignedDate)),
                ReturnDate = ParseDate(ValueOf(txtReturnDate)),
                AssetCondition = SelectedValueOf(ddlAssetCondition),
                AssetStatus = SelectedValueOf(ddlAssetStatus),
                InsertedBy = ParseInt(Convert.ToString(Session["userId"]))
            };
        }

        private int GetProbationMonths()
        {
            DateTime? joiningDate = ParseDate(ValueOf(txtJoiningDate));
            DateTime? probationEndDate = ParseDate(ValueOf(txtProbationEndDate));
            if (!joiningDate.HasValue || !probationEndDate.HasValue || probationEndDate.Value <= joiningDate.Value)
            {
                return 0;
            }

            return ((probationEndDate.Value.Year - joiningDate.Value.Year) * 12) + probationEndDate.Value.Month - joiningDate.Value.Month;
        }

        private string TextOf(DropDownList ddl)
        {
            return ddl.SelectedIndex > 0 ? ddl.SelectedItem.Text.Trim() : string.Empty;
        }

        private string NormalizeWorkingHours(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return string.Empty;
            }

            TimeSpan parsed;
            if (TimeSpan.TryParse(value.Trim(), out parsed))
            {
                return parsed.ToString(@"hh\:mm\:ss");
            }

            return value.Trim();
        }

        private string SelectedValueOf(DropDownList ddl)
        {
            if (ddl == null)
            {
                return string.Empty;
            }

            if (ddl.SelectedIndex > 0)
            {
                return Convert.ToString(ddl.SelectedValue ?? string.Empty).Trim();
            }

            string postedValue = Request.Form[ddl.UniqueID];
            return Convert.ToString(postedValue ?? string.Empty).Trim();
        }

        private string ValueOf(HtmlInputControl input)
        {
            return input == null ? string.Empty : Convert.ToString(input.Value ?? string.Empty).Trim();
        }

        private bool IsYesValue(string value, string text)
        {
            string normalizedValue = Convert.ToString(value ?? string.Empty).Trim();
            string normalizedText = Convert.ToString(text ?? string.Empty).Trim();

            return normalizedValue == "1"
                || normalizedValue.Equals("yes", StringComparison.OrdinalIgnoreCase)
                || normalizedValue.Equals("true", StringComparison.OrdinalIgnoreCase)
                || normalizedText.Equals("yes", StringComparison.OrdinalIgnoreCase)
                || normalizedText.Equals("true", StringComparison.OrdinalIgnoreCase);
        }

        private DateTime? ParseDate(string value)
        {
            DateTime parsed;
            if (DateTime.TryParse(value, out parsed))
            {
                return parsed;
            }
            return null;
        }

        private int ParseInt(string value)
        {
            int parsed;
            return int.TryParse(Convert.ToString(value), out parsed) ? parsed : 0;
        }

        private decimal ParseDecimal(string value)
        {
            decimal parsed;
            return decimal.TryParse(Convert.ToString(value), out parsed) ? parsed : 0;
        }
    }
}
