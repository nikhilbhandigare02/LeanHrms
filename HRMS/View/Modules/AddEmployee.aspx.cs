using ProcessModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using DataObject;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Web.UI.HtmlControls;

namespace HRMS.View.Modules
{
    public partial class AddEmployee : System.Web.UI.Page
    {
        protected string UserId = null;
        private const string EmployeeListUrl = "EmployeeList.aspx";
        private const string AssetSessionKeyPrefix = "EmployeeAssetDraft_";
        private const string DefaultEmployeePassword = "Pass@123";
        private bool _attendanceSectionComplete;
        private bool _bankSectionComplete;
        private bool _contactSectionComplete;
        private bool _educationSectionComplete;
        private bool _workExperienceSectionComplete;
        private bool _certificationSectionComplete;

        private string OriginalOfficialEmail
        {
            get { return Convert.ToString(ViewState["OriginalOfficialEmail"] ?? string.Empty); }
            set { ViewState["OriginalOfficialEmail"] = value ?? string.Empty; }
        }

        private string OriginalOfficialMobile
        {
            get { return Convert.ToString(ViewState["OriginalOfficialMobile"] ?? string.Empty); }
            set { ViewState["OriginalOfficialMobile"] = value ?? string.Empty; }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (string.Equals(Request.QueryString["action"], "checkOfficialContactDuplicate", StringComparison.OrdinalIgnoreCase))
            {
                WriteOfficialContactDuplicateValidationResponse();
                return;
            }

            UserId = Convert.ToString(Session["userId"]);
            int userId = 0;
            if (!IsPostBack)
            {
                if (Session["userId"] == null)
                {
                    Response.Redirect("~/view/authentication/login.aspx", false);
                    return;
                }

                if (Request.QueryString["user_id"] != null)
                {
                    userId = Convert.ToInt32(Request.QueryString["user_id"]);
                }
                else
                {
                    userId = 0;
                }
                BindDesignation();
                BindReportinngManager();
                BindCompany();
                BindEmployeeExporIntern();
                BindUpdateLookupDropdowns();
                BindAssetLookups();
                BindUserData(userId);
                BindEmployeeAssets(userId);
                BindEmployeeEducation(userId);
                BindEmployeeWorkExperience(userId);
                UpdateSectionStatuses();
            }

            LockViewOnlySections();
        }

        private void BindEmployeeAssets(int userId)
        {
            try
            {
                List<EmployeeAssetDetailsDO> assets = GetOrInitializeAssetDraft(userId);
                gvEmployeeAssets.DataSource = assets;
                gvEmployeeAssets.DataBind();
            }
            catch (Exception ex)
            {
                CommonBL errorlog = new CommonBL();
                errorlog.fnStoreErrorLog("AddEmployee", "BindEmployeeAssets", "Exception Message=" + ex.Message + " StackTrace=" + ex.StackTrace, UserId);
            }
        }

        private void BindEmployeeEducation(int userId)
        {
            try
            {
                UserDetailsBL userBAL = new UserDetailsBL();
                List<EmployeeEducationDetailsDO> educationList = userBAL.GetEmployeeEducation(userId);
                _educationSectionComplete = HasEducationData(educationList);
                educationPreviewBody.InnerHtml = BuildEducationRowsHtml(educationList);
            }
            catch (Exception ex)
            {
                CommonBL errorlog = new CommonBL();
                errorlog.fnStoreErrorLog("AddEmployee", "BindEmployeeEducation", "Exception Message=" + ex.Message + " StackTrace=" + ex.StackTrace, UserId);
            }
        }

        private void BindEmployeeWorkExperience(int userId)
        {
            try
            {
                UserDetailsBL userBAL = new UserDetailsBL();
                List<EmployeeWorkExperienceDetailsDO> experienceList = userBAL.GetEmployeeWorkExperience(userId);
                _workExperienceSectionComplete = HasWorkExperienceData(experienceList);
                workExperiencePreviewBody.InnerHtml = BuildWorkExperienceRowsHtml(experienceList);
            }
            catch (Exception ex)
            {
                CommonBL errorlog = new CommonBL();
                errorlog.fnStoreErrorLog("AddEmployee", "BindEmployeeWorkExperience", "Exception Message=" + ex.Message + " StackTrace=" + ex.StackTrace, UserId);
            }
        }

        private void LockViewOnlySections()
        {
            SetReadOnly(txtAlternateMobile, true);
            SetReadOnly(txtPersonalEmail, true);
            SetReadOnly(txtPermanentHouseNumber, true);
            SetReadOnly(txtPermanentBuildingName, true);
            SetReadOnly(txtPermanentStreet, true);
            SetReadOnly(txtPermanentArea, true);
            SetReadOnly(txtPermanentLandmark, true);
            SetReadOnly(txtPermanentCity, true);
            SetReadOnly(txtPermanentDistrict, true);
            SetReadOnly(txtPermanentState, true);
            SetReadOnly(txtPermanentCountry, true);
            SetReadOnly(txtPermanentPinCode, true);
            if (chkSameAsPermanent != null)
            {
                chkSameAsPermanent.InputAttributes["onclick"] = "return false;";
            }
            SetReadOnly(txtCurrentHouseNumber, true);
            SetReadOnly(txtCurrentBuildingName, true);
            SetReadOnly(txtCurrentStreet, true);
            SetReadOnly(txtCurrentArea, true);
            SetReadOnly(txtCurrentLandmark, true);
            SetReadOnly(txtCurrentCity, true);
            SetReadOnly(txtCurrentDistrict, true);
            SetReadOnly(txtCurrentState, true);
            SetReadOnly(txtCurrentCountry, true);
            SetReadOnly(txtCurrentPinCode, true);
            SetReadOnly(txtEmergencyContactName, true);
            SetReadOnly(txtEmergencyContactNumber, true);
            SetReadOnly(txtEmergencyContactRelationship, true);

            SetReadOnly(txtBankName, true);
            SetReadOnly(txtBankBranchName, true);
            SetReadOnly(txtAccountHolderName, true);
            SetReadOnly(txtAccountNumber, true);
            SetReadOnly(txtConfirmAccountNumber, true);
            SetReadOnly(txtIfscCode, true);
            SetReadOnly(txtAccountType, true);
            SetReadOnly(txtSalaryAccountFlag, true);
        }

        private void SetReadOnly(TextBox textBox, bool isReadOnly)
        {
            if (textBox == null)
            {
                return;
            }

            textBox.ReadOnly = isReadOnly;
        }

        private static string NormalizeEmailValue(string value)
        {
            return Convert.ToString(value ?? string.Empty).Trim().ToUpperInvariant();
        }

        private static string NormalizeMobileValue(string value)
        {
            return Convert.ToString(value ?? string.Empty).Trim();
        }

        private bool HasOfficialEmailChanged(string currentEmail)
        {
            return !string.Equals(
                NormalizeEmailValue(OriginalOfficialEmail),
                NormalizeEmailValue(currentEmail),
                StringComparison.Ordinal);
        }

        private bool HasOfficialMobileChanged(string currentMobile)
        {
            return !string.Equals(
                NormalizeMobileValue(OriginalOfficialMobile),
                NormalizeMobileValue(currentMobile),
                StringComparison.Ordinal);
        }

        private void SendUpdatedCredentialsIfRequired(UserDetailsBL userDetailsBL, UserDetailsDO user)
        {
            if (userDetailsBL == null || user == null)
            {
                return;
            }

            if (!HasOfficialEmailChanged(user.user_mail_id))
            {
                return;
            }

            string activeOfficialEmail = userDetailsBL.GetActiveOfficialEmail(user.UserId, user.user_mail_id);
            if (string.IsNullOrWhiteSpace(activeOfficialEmail))
            {
                return;
            }

            userDetailsBL.SendUserCredentialsMail(activeOfficialEmail.Trim(), user.Username, DefaultEmployeePassword);
            OriginalOfficialEmail = activeOfficialEmail;
        }

        private void UpdateSectionStatuses()
        {
            SetSectionStatus("contactSectionStatus", _contactSectionComplete);
            SetSectionStatus("attendanceSectionStatus", _attendanceSectionComplete);
            SetSectionStatus("bankSectionStatus", _bankSectionComplete);
            SetSectionStatus("educationSectionStatus", _educationSectionComplete);
            SetSectionStatus("workExperienceSectionStatus", _workExperienceSectionComplete);
            SetSectionStatus("certificationSectionStatus", _certificationSectionComplete);
        }

        private bool HasContactSectionData(UserDetailsDO userDetails)
        {
            if (userDetails == null)
            {
                return false;
            }

            bool hasOfficialContact = HasAnyText(userDetails.user_mail_id, userDetails.contact_detail);
            bool hasPersonalContact = HasAnyText(
                userDetails.AlternateMobileNumber,
                userDetails.PersonalEmail,
                userDetails.EmergencyContactName,
                userDetails.EmergencyContactNumber,
                userDetails.EmergencyContactRelationship);
            bool hasPermanentAddress = HasAnyText(
                userDetails.PermanentHouseNumber,
                userDetails.PermanentBuildingName,
                userDetails.PermanentStreet,
                userDetails.PermanentArea,
                userDetails.PermanentLandmark,
                userDetails.PermanentCity,
                userDetails.PermanentDistrict,
                userDetails.PermanentState,
                userDetails.PermanentCountry,
                userDetails.PermanentPinCode);
            bool hasCurrentAddress = IsTruthy(userDetails.SameAsPermanent) || HasAnyText(
                userDetails.CurrentHouseNumber,
                userDetails.CurrentBuildingName,
                userDetails.CurrentStreet,
                userDetails.CurrentArea,
                userDetails.CurrentLandmark,
                userDetails.CurrentCity,
                userDetails.CurrentDistrict,
                userDetails.CurrentState,
                userDetails.CurrentCountry,
                userDetails.CurrentPinCode);

            return hasOfficialContact && hasPersonalContact && hasPermanentAddress && hasCurrentAddress;
        }

        private void SetSectionStatus(string controlId, bool isComplete)
        {
            HtmlGenericControl statusControl = FindControlRecursive(this, controlId) as HtmlGenericControl;
            if (statusControl == null)
            {
                return;
            }

            statusControl.Attributes["class"] = isComplete ? "section-status completed" : "section-status pending";
            statusControl.InnerHtml = isComplete
                ? "<i class='fas fa-check-circle'></i> Completed"
                : "<i class='far fa-clock'></i> Pending";
        }

        private bool HasCertificationSectionData(UserDetailsDO userDetails)
        {
            if (userDetails == null)
            {
                return false;
            }

            bool hasCertificationFields = HasAnyText(
                userDetails.CertificationName,
                userDetails.CertificationAuthority,
                userDetails.CertificateNumber,
                FormatDate(userDetails.IssueDate),
                FormatDate(userDetails.ExpiryDate),
                userDetails.CertificationFile);

            bool hasMeaningfulRenewalValue =
                !string.IsNullOrWhiteSpace(userDetails.RenewalRequired) &&
                !string.Equals(userDetails.RenewalRequired.Trim(), "No", StringComparison.OrdinalIgnoreCase) &&
                !string.Equals(userDetails.RenewalRequired.Trim(), "0", StringComparison.OrdinalIgnoreCase);

            return hasCertificationFields || hasMeaningfulRenewalValue;
        }

        private Control FindControlRecursive(Control parent, string controlId)
        {
            if (parent == null || string.IsNullOrWhiteSpace(controlId))
            {
                return null;
            }

            Control directControl = parent.FindControl(controlId);
            if (directControl != null)
            {
                return directControl;
            }

            foreach (Control child in parent.Controls)
            {
                Control nestedControl = FindControlRecursive(child, controlId);
                if (nestedControl != null)
                {
                    return nestedControl;
                }
            }

            return null;
        }

        private bool HasEducationData(List<EmployeeEducationDetailsDO> educationList)
        {
            return educationList != null && educationList.Any(education =>
                !string.IsNullOrWhiteSpace(education.QualificationLevel) ||
                !string.IsNullOrWhiteSpace(education.DegreeName) ||
                !string.IsNullOrWhiteSpace(education.Specialization) ||
                !string.IsNullOrWhiteSpace(education.University) ||
                !string.IsNullOrWhiteSpace(education.InstituteName) ||
                !string.IsNullOrWhiteSpace(education.YearOfPassing) ||
                !string.IsNullOrWhiteSpace(education.PercentageCgpa) ||
                !string.IsNullOrWhiteSpace(education.CertificateFile));
        }

        private bool HasWorkExperienceData(List<EmployeeWorkExperienceDetailsDO> experienceList)
        {
            return experienceList != null && experienceList.Any(experience =>
                !string.IsNullOrWhiteSpace(experience.OrganizationName) ||
                !string.IsNullOrWhiteSpace(experience.Industry) ||
                !string.IsNullOrWhiteSpace(experience.Designation) ||
                experience.StartDate.HasValue ||
                experience.EndDate.HasValue ||
                !string.IsNullOrWhiteSpace(experience.TotalExperience) ||
                !string.IsNullOrWhiteSpace(experience.LastDrawnSalary));
        }

        private bool HasAnyText(params string[] values)
        {
            return values.Any(value => !string.IsNullOrWhiteSpace(value));
        }

        private string BuildEducationRowsHtml(List<EmployeeEducationDetailsDO> educationList)
        {
            if (educationList == null || educationList.Count == 0)
            {
                return "<tr id='educationEmptyRow'><td colspan='8' class='entry-empty'>No education added yet.</td></tr>";
            }

            System.Text.StringBuilder html = new System.Text.StringBuilder();
            foreach (EmployeeEducationDetailsDO education in educationList)
            {
                string certificateUrl = BuildCertificationViewUrl(education.CertificateFile);
                bool isBase64Image = IsBase64ImageContent(education.CertificateFile);
                bool isBase64Pdf = IsBase64PdfContent(education.CertificateFile);
                string certificateCell = string.IsNullOrWhiteSpace(education.CertificateFile)
                    ? "-"
                    : "<a class='entry-chip-link' href='" + HttpUtility.HtmlAttributeEncode(certificateUrl) + "' target='" + (isBase64Image || isBase64Pdf ? string.Empty : "_blank") + "' " +
                      "data-file-url='" + HttpUtility.HtmlAttributeEncode(certificateUrl) + "' " +
                      "data-file-type='" + HttpUtility.HtmlAttributeEncode(isBase64Pdf ? "pdf" : "image") + "' " +
                      "data-file-title='Education Certificate' " +
                      ((isBase64Image || isBase64Pdf) ? "onclick='return openCertificationFileModal(this);'" : string.Empty) + ">" +
                      "<i class='far fa-file-pdf'></i>" +
                      "<span class='entry-chip-caption'><span>Certificate</span><small>View file</small></span>" +
                      "</a>";

                html.Append("<tr>");
                html.Append("<td>").Append(HttpUtility.HtmlEncode(FormatCellValue(education.QualificationLevel))).Append("</td>");
                html.Append("<td>").Append(HttpUtility.HtmlEncode(FormatCellValue(education.DegreeName))).Append("</td>");
                html.Append("<td>").Append(HttpUtility.HtmlEncode(FormatCellValue(education.Specialization))).Append("</td>");
                html.Append("<td>").Append(HttpUtility.HtmlEncode(FormatCellValue(education.University))).Append("</td>");
                html.Append("<td>").Append(HttpUtility.HtmlEncode(FormatCellValue(education.InstituteName))).Append("</td>");
                html.Append("<td>").Append(HttpUtility.HtmlEncode(FormatCellValue(education.YearOfPassing))).Append("</td>");
                html.Append("<td>").Append(HttpUtility.HtmlEncode(FormatCellValue(education.PercentageCgpa))).Append("</td>");
                html.Append("<td>").Append(certificateCell).Append("</td>");
                html.Append("</tr>");
            }

            return html.ToString();
        }

        private string BuildWorkExperienceRowsHtml(List<EmployeeWorkExperienceDetailsDO> experienceList)
        {
            if (experienceList == null || experienceList.Count == 0)
            {
                return "<tr id='workExperienceEmptyRow'><td colspan='8' class='entry-empty'>No work experience added yet.</td></tr>";
            }

            System.Text.StringBuilder html = new System.Text.StringBuilder();
            foreach (EmployeeWorkExperienceDetailsDO experience in experienceList)
            {
                html.Append("<tr>");
                html.Append("<td>").Append(HttpUtility.HtmlEncode(FormatCellValue(experience.OrganizationName))).Append("</td>");
                html.Append("<td>").Append(HttpUtility.HtmlEncode(FormatCellValue(experience.Industry))).Append("</td>");
                html.Append("<td>").Append(HttpUtility.HtmlEncode(FormatCellValue(experience.Designation))).Append("</td>");
                html.Append("<td>").Append(HttpUtility.HtmlEncode(FormatCellValue(experience.EmploymentPeriod))).Append("</td>");
                html.Append("<td>").Append(HttpUtility.HtmlEncode(FormatDate(experience.StartDate))).Append("</td>");
                html.Append("<td>").Append(HttpUtility.HtmlEncode(FormatDate(experience.EndDate))).Append("</td>");
                html.Append("<td>").Append(HttpUtility.HtmlEncode(FormatCellValue(experience.TotalExperience))).Append("</td>");
                html.Append("<td>").Append(HttpUtility.HtmlEncode(FormatCellValue(experience.LastDrawnSalary))).Append("</td>");
                html.Append("</tr>");
            }

            return html.ToString();
        }

        private string FormatCellValue(string value)
        {
            return string.IsNullOrWhiteSpace(value) ? "-" : value.Trim();
        }

        private void BindAssetLookups()
        {
            try
            {
                EmployeeOnboardingBL onboardingBL = new EmployeeOnboardingBL();
                ddlAssetCondition.DataSource = onboardingBL.BindLookupData("Asset Condition");
                ddlAssetCondition.DataTextField = "Text";
                ddlAssetCondition.DataValueField = "Id";
                ddlAssetCondition.DataBind();
                ddlAssetCondition.Items.Insert(0, new ListItem("-- Select --", ""));

                ddlAssetStatus.DataSource = onboardingBL.BindLookupData("Asset Status");
                ddlAssetStatus.DataTextField = "Text";
                ddlAssetStatus.DataValueField = "Id";
                ddlAssetStatus.DataBind();
                ddlAssetStatus.Items.Insert(0, new ListItem("-- Select --", ""));
            }
            catch (Exception ex)
            {
                CommonBL errorlog = new CommonBL();
                errorlog.fnStoreErrorLog("AddEmployee", "BindAssetLookups", "Exception Message=" + ex.Message + " StackTrace=" + ex.StackTrace, UserId);
            }
        }

        protected void btnSaveAsset_Click(object sender, EventArgs e)
        {
            int employeeUserId = GetEmployeeUserIdFromQuery();
            if (employeeUserId <= 0)
            {
                ShowAssetMessage("Failed", "Employee UserId is required for asset save.");
                return;
            }

            string validationMessage = ValidateAssetForm();
            if (!string.IsNullOrWhiteSpace(validationMessage))
            {
                ShowAssetEditor();
                ShowAssetMessage("Failed", validationMessage);
                return;
            }

            List<EmployeeAssetDetailsDO> assets = GetOrInitializeAssetDraft(employeeUserId);
            int assetId;
            int.TryParse(hdnAssetAssignmentId.Value, out assetId);

            if (IsDuplicateAssetNumberInDraft(assets, txtAssetNumber.Text, assetId))
            {
                ShowAssetEditor();
                ShowAssetMessage("Failed", "Asset number already exists.");
                return;
            }

            EmployeeAssetDetailsDO draftAsset = BuildAssetDraftFromForm(employeeUserId, assetId);
            EmployeeAssetDetailsDO existing = assets.FirstOrDefault(x => x.AssetAssignmentId == draftAsset.AssetAssignmentId);

            if (existing != null)
            {
                existing.AssetType = draftAsset.AssetType;
                existing.AssetNumber = draftAsset.AssetNumber;
                existing.AssetName = draftAsset.AssetName;
                existing.AssignedDate = draftAsset.AssignedDate;
                existing.ReturnDate = draftAsset.ReturnDate;
                existing.AssetConditionId = draftAsset.AssetConditionId;
                existing.AssetCondition = draftAsset.AssetCondition;
                existing.AssetStatusId = draftAsset.AssetStatusId;
                existing.AssetStatus = draftAsset.AssetStatus;
            }
            else
            {
                if (draftAsset.AssetAssignmentId <= 0)
                {
                    draftAsset.AssetAssignmentId = GetNextDraftAssetId(assets);
                }

                assets.Add(draftAsset);
            }

            SaveAssetDraft(employeeUserId, assets);
            ClearAssetForm();
            BindEmployeeAssets(employeeUserId);
            // Keep asset changes on the page only; final DB save happens on the main employee update.
        }

        protected void btnSaveEducation_Click(object sender, EventArgs e)
        {
            int employeeUserId = GetEmployeeUserIdFromQuery();
            if (employeeUserId <= 0)
            {
                ShowAssetMessage("Failed", "Employee UserId is required for education save.");
                return;
            }

            EmployeeEducationDetailsDO education = new EmployeeEducationDetailsDO
            {
                QualificationLevel = ValueOf(txtQualificationLevel.Text),
                DegreeName = ValueOf(txtDegreeName.Text),
                Specialization = ValueOf(txtSpecialization.Text),
                University = ValueOf(txtUniversity.Text),
                InstituteName = ValueOf(txtInstituteName.Text),
                YearOfPassing = ValueOf(txtYearOfPassing.Text),
                PercentageCgpa = ValueOf(txtPercentageCgpa.Text),
                CertificateFile = GetUploadedEducationCertificateValue()
            };

            int educationId;
            int.TryParse(hdnEducationId.Value, out educationId);

            EmployeeOnboardingBL onboardingBL = new EmployeeOnboardingBL();
            EmployeeOnboardingResponseDO response = onboardingBL.SaveEmployeeEducation(educationId, employeeUserId, education, GetClientIdFromSession());

            string status = response == null ? "Failed" : Convert.ToString(response.Status ?? "Failed");
            string message = response == null || string.IsNullOrWhiteSpace(response.Message)
                ? "Employee education save failed."
                : response.Message;

            if (string.Equals(status, "Success", StringComparison.OrdinalIgnoreCase))
            {
                ClearEducationForm();
                BindEmployeeEducation(employeeUserId);
                ScriptManager.RegisterStartupScript(this, GetType(), "HideEducationEditor", "hideSectionEditor('educationEditorCard');", true);
            }
            else
            {
                ScriptManager.RegisterStartupScript(this, GetType(), "ShowEducationEditor", "showSectionEditor('educationEditorCard');", true);
                ShowAssetMessage(status, message);
            }
        }

        protected void btnSaveWorkExperience_Click(object sender, EventArgs e)
        {
            int employeeUserId = GetEmployeeUserIdFromQuery();
            if (employeeUserId <= 0)
            {
                ShowAssetMessage("Failed", "Employee UserId is required for work experience save.");
                return;
            }

            EmployeeWorkExperienceDetailsDO experience = new EmployeeWorkExperienceDetailsDO
            {
                OrganizationName = ValueOf(Request.Form["txtWorkOrganizationName"]),
                Industry = ValueOf(Request.Form["txtWorkIndustry"]),
                Designation = ValueOf(Request.Form["txtWorkDesignation"]),
                EmploymentPeriod = ValueOf(Request.Form["txtWorkEmploymentPeriod"]),
                StartDate = ParseHtmlDate(Request.Form["txtWorkStartDate"]),
                EndDate = ParseHtmlDate(Request.Form["txtWorkEndDate"]),
                TotalExperience = ValueOf(Request.Form["txtWorkTotalExperience"]),
                LastDrawnSalary = ValueOf(Request.Form["txtWorkLastDrawnSalary"])
            };

            int experienceId;
            int.TryParse(hdnWorkExperienceId.Value, out experienceId);

            EmployeeOnboardingBL onboardingBL = new EmployeeOnboardingBL();
            EmployeeOnboardingResponseDO response = onboardingBL.SaveEmployeeWorkExperience(experienceId, employeeUserId, experience, GetClientIdFromSession());

            string status = response == null ? "Failed" : Convert.ToString(response.Status ?? "Failed");
            string message = response == null || string.IsNullOrWhiteSpace(response.Message)
                ? "Employee work experience save failed."
                : response.Message;

            if (string.Equals(status, "Success", StringComparison.OrdinalIgnoreCase))
            {
                ClearWorkExperienceForm();
                BindEmployeeWorkExperience(employeeUserId);
                ScriptManager.RegisterStartupScript(this, GetType(), "HideWorkExperienceEditor", "hideSectionEditor('workExperienceEditorCard');", true);
            }
            else
            {
                ScriptManager.RegisterStartupScript(this, GetType(), "ShowWorkExperienceEditor", "showSectionEditor('workExperienceEditorCard');", true);
                ShowAssetMessage(status, message);
            }
        }

        protected void gvEmployeeAssets_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            int employeeUserId = GetEmployeeUserIdFromQuery();
            int assetId;
            if (!int.TryParse(Convert.ToString(e.CommandArgument), out assetId) || assetId == 0)
            {
                ShowAssetMessage("Failed", "AssetId is required.");
                return;
            }

            List<EmployeeAssetDetailsDO> assets = GetOrInitializeAssetDraft(employeeUserId);
            EmployeeAssetDetailsDO asset = assets.FirstOrDefault(x => x.AssetAssignmentId == assetId);

            if (string.Equals(e.CommandName, "editAsset", StringComparison.OrdinalIgnoreCase))
            {
                if (asset == null)
                {
                    ShowAssetMessage("Failed", "Asset details not found.");
                    return;
                }

                hdnAssetAssignmentId.Value = asset.AssetAssignmentId.ToString();
                txtAssetType.Text = asset.AssetType;
                txtAssetNumber.Text = asset.AssetNumber;
                txtAssetName.Text = asset.AssetName;
                txtAssignedDate.Text = FormatDateForInput(asset.AssignedDate);
                txtReturnDate.Text = FormatDateForInput(asset.ReturnDate);
                SelectDropdownValue(ddlAssetCondition, asset.AssetConditionId.ToString());
                SelectDropdownValue(ddlAssetStatus, asset.AssetStatusId.ToString());
                btnSaveAsset.Text = "Update Asset";
                ShowAssetEditor();
            }
            else if (string.Equals(e.CommandName, "deleteAsset", StringComparison.OrdinalIgnoreCase))
            {
                if (asset == null)
                {
                    ShowAssetMessage("Failed", "Asset details not found.");
                    return;
                }

                assets.Remove(asset);
                SaveAssetDraft(employeeUserId, assets);
                ClearAssetForm();
                BindEmployeeAssets(employeeUserId);
                // Keep delete as a staged page change; final DB delete happens on the main employee update.
            }
        }

        private void BindUpdateLookupDropdowns()
        {
            try
            {
                EmployeeOnboardingBL onboardingBL = new EmployeeOnboardingBL();
                BindLookupDropdown(txtGender, onboardingBL.BindLookupData("Gender"));
                BindLookupDropdown(txtMaritalStatus, onboardingBL.BindLookupData("Marital Status"));
                BindLookupDropdown(txtBloodGroup, onboardingBL.BindLookupData("Blood Group"));
                BindLookupDropdown(txtNationality, onboardingBL.BindLookupData("Nationality"));
                BindLookupDropdown(txtEmployeeStatus, onboardingBL.BindLookupData("EmpStatus"));
                BindLookupDropdown(txtSeparationReason, onboardingBL.BindLookupData("Separation Reason"));
                BindLookupDropdown(txtEmployeeLevel, onboardingBL.BindLookupData("Employee Level"));
                BindLookupDropdown(txtAttendanceType, onboardingBL.BindLookupData("Attendance Type"));
                BindLookupDropdown(txtWeeklyOff, onboardingBL.BindLookupData("Weekly Off"));
                BindLookupDropdown(txtAttendancePolicy, onboardingBL.BindLookupData("Attendance Policy"));
                BindLookupDropdown(txtWorkLocation, onboardingBL.BindLookupData("Work Location"));
            }
            catch (Exception ex)
            {
                CommonBL errorlog = new CommonBL();
                errorlog.fnStoreErrorLog("AddEmployee", "BindUpdateLookupDropdowns", "Exception Message=" + ex.Message + " StackTrace=" + ex.StackTrace, UserId);
            }
        }

        private void BindLookupDropdown(DropDownList ddl, List<DropDownData> items)
        {
            if (ddl == null)
            {
                return;
            }

            ddl.DataSource = items ?? new List<DropDownData>();
            ddl.DataTextField = "Text";
            ddl.DataValueField = "Id";
            ddl.DataBind();
            ddl.Items.Insert(0, new ListItem("-- Please Select --", ""));
        }

        private EmployeeAssetDetailsDO BuildAssetDraftFromForm(int userId, int assetId)
        {
            return new EmployeeAssetDetailsDO
            {
                AssetAssignmentId = assetId,
                UserId = userId,
                AssetType = txtAssetType.Text.Trim(),
                AssetNumber = txtAssetNumber.Text.Trim(),
                AssetName = txtAssetName.Text.Trim(),
                AssignedDate = ParseAssetDate(txtAssignedDate.Text),
                ReturnDate = ParseAssetDate(txtReturnDate.Text),
                AssetConditionId = ParseIntValue(ddlAssetCondition.SelectedValue),
                AssetCondition = ddlAssetCondition.SelectedItem != null ? ddlAssetCondition.SelectedItem.Text : string.Empty,
                AssetStatusId = ParseIntValue(ddlAssetStatus.SelectedValue),
                AssetStatus = ddlAssetStatus.SelectedItem != null ? ddlAssetStatus.SelectedItem.Text : string.Empty
            };
        }

        private string ValidateAssetForm()
        {
            if (string.IsNullOrWhiteSpace(txtAssetType.Text)) return "Asset Type is required.";
            if (string.IsNullOrWhiteSpace(txtAssetNumber.Text)) return "Asset Number is required.";
            if (string.IsNullOrWhiteSpace(txtAssetName.Text)) return "Asset Name is required.";
            if (!ParseAssetDate(txtAssignedDate.Text).HasValue) return "Assigned Date is required in dd-MM-yyyy format.";
            if (!string.IsNullOrWhiteSpace(txtReturnDate.Text) && !ParseAssetDate(txtReturnDate.Text).HasValue) return "Return Date must be in dd-MM-yyyy format.";
            if (string.IsNullOrWhiteSpace(ddlAssetCondition.SelectedValue)) return "Asset Condition is required.";
            if (string.IsNullOrWhiteSpace(ddlAssetStatus.SelectedValue)) return "Asset Status is required.";
            return string.Empty;
        }

        private DateTime? ParseAssetDate(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return null;
            }

            DateTime parsed;
            string[] acceptedFormats = { "dd-MM-yyyy", "yyyy-MM-dd" };
            return DateTime.TryParseExact(value.Trim(), acceptedFormats, CultureInfo.InvariantCulture, DateTimeStyles.None, out parsed)
                ? parsed
                : (DateTime?)null;
        }

        private void ClearAssetForm()
        {
            hdnAssetAssignmentId.Value = "0";
            txtAssetType.Text = string.Empty;
            txtAssetNumber.Text = string.Empty;
            txtAssetName.Text = string.Empty;
            txtAssignedDate.Text = string.Empty;
            txtReturnDate.Text = string.Empty;
            ddlAssetCondition.SelectedIndex = 0;
            ddlAssetStatus.SelectedIndex = 0;
            btnSaveAsset.Text = "Add Asset";
            HideAssetEditor();
        }

        private List<EmployeeAssetDetailsDO> GetOrInitializeAssetDraft(int userId)
        {
            string key = GetAssetDraftSessionKey(userId);
            List<EmployeeAssetDetailsDO> assets = Session[key] as List<EmployeeAssetDetailsDO>;
            if (assets != null)
            {
                return assets;
            }

            UserDetailsBL userBAL = new UserDetailsBL();
            assets = userBAL.GetEmployeeAssets(userId) ?? new List<EmployeeAssetDetailsDO>();
            Session[key] = assets;
            return assets;
        }

        private void SaveAssetDraft(int userId, List<EmployeeAssetDetailsDO> assets)
        {
            Session[GetAssetDraftSessionKey(userId)] = assets ?? new List<EmployeeAssetDetailsDO>();
        }

        private string GetAssetDraftSessionKey(int userId)
        {
            return AssetSessionKeyPrefix + userId;
        }

        private int GetNextDraftAssetId(List<EmployeeAssetDetailsDO> assets)
        {
            int minimumId = assets == null || assets.Count == 0 ? 0 : assets.Min(x => x.AssetAssignmentId);
            return minimumId <= 0 ? minimumId - 1 : -1;
        }

        private bool IsDuplicateAssetNumberInDraft(List<EmployeeAssetDetailsDO> assets, string assetNumber, int currentAssetId)
        {
            string normalized = Convert.ToString(assetNumber ?? string.Empty).Trim();
            if (string.IsNullOrWhiteSpace(normalized) || assets == null)
            {
                return false;
            }

            return assets.Any(x =>
                x.AssetAssignmentId != currentAssetId &&
                string.Equals(Convert.ToString(x.AssetNumber ?? string.Empty).Trim(), normalized, StringComparison.OrdinalIgnoreCase));
        }

        private int ParseIntValue(string value)
        {
            int parsed;
            return int.TryParse(value, out parsed) ? parsed : 0;
        }

        private EmployeeOnboardingResponseDO PersistAssetDraft(int userId)
        {
            EmployeeOnboardingResponseDO response = new EmployeeOnboardingResponseDO
            {
                Status = "Success",
                Message = "Assets saved successfully.",
                UserId = userId
            };

            UserDetailsBL userBAL = new UserDetailsBL();
            EmployeeOnboardingBL onboardingBL = new EmployeeOnboardingBL();
            List<EmployeeAssetDetailsDO> currentDbAssets = userBAL.GetEmployeeAssets(userId) ?? new List<EmployeeAssetDetailsDO>();
            List<EmployeeAssetDetailsDO> draftAssets = GetOrInitializeAssetDraft(userId);

            foreach (EmployeeAssetDetailsDO asset in draftAssets)
            {
                EmployeeAssetDO request = new EmployeeAssetDO
                {
                    AssetType = asset.AssetType,
                    AssetNumber = asset.AssetNumber,
                    AssetName = asset.AssetName,
                    AssignedDate = asset.AssignedDate,
                    ReturnDate = asset.ReturnDate,
                    AssetCondition = asset.AssetConditionId.ToString(),
                    AssetStatus = asset.AssetStatusId.ToString()
                };

                EmployeeOnboardingResponseDO saveResponse = asset.AssetAssignmentId > 0
                    ? onboardingBL.UpdateEmployeeAsset(asset.AssetAssignmentId, userId, request, GetClientIdFromSession())
                    : onboardingBL.SaveEmployeeAsset(userId, request, GetClientIdFromSession());

                if (saveResponse == null || !string.Equals(saveResponse.Status, "Success", StringComparison.OrdinalIgnoreCase))
                {
                    return saveResponse ?? new EmployeeOnboardingResponseDO
                    {
                        Status = "Failed",
                        Message = "Employee asset save failed.",
                        UserId = userId
                    };
                }
            }

            List<int> retainedAssetIds = draftAssets.Where(x => x.AssetAssignmentId > 0).Select(x => x.AssetAssignmentId).ToList();
            foreach (EmployeeAssetDetailsDO existingAsset in currentDbAssets.Where(x => !retainedAssetIds.Contains(x.AssetAssignmentId)))
            {
                EmployeeOnboardingResponseDO deleteResponse = onboardingBL.DeleteEmployeeAsset(existingAsset.AssetAssignmentId, userId, GetClientIdFromSession());
                if (deleteResponse == null || !string.Equals(deleteResponse.Status, "Success", StringComparison.OrdinalIgnoreCase))
                {
                    return deleteResponse ?? new EmployeeOnboardingResponseDO
                    {
                        Status = "Failed",
                        Message = "Employee asset delete failed.",
                        UserId = userId
                    };
                }
            }

            SaveAssetDraft(userId, userBAL.GetEmployeeAssets(userId) ?? new List<EmployeeAssetDetailsDO>());
            return response;
        }

        private void ClearEducationForm()
        {
            hdnEducationId.Value = "0";
            txtQualificationLevel.Text = string.Empty;
            txtDegreeName.Text = string.Empty;
            txtSpecialization.Text = string.Empty;
            txtUniversity.Text = string.Empty;
            txtInstituteName.Text = string.Empty;
            txtYearOfPassing.Text = string.Empty;
            txtPercentageCgpa.Text = string.Empty;
        }

        private void ClearWorkExperienceForm()
        {
            hdnWorkExperienceId.Value = "0";
        }

        private string GetUploadedEducationCertificateValue()
        {
            if (fileEducationCertificate != null &&
                fileEducationCertificate.PostedFile != null &&
                !string.IsNullOrWhiteSpace(fileEducationCertificate.PostedFile.FileName))
            {
                return System.IO.Path.GetFileName(fileEducationCertificate.PostedFile.FileName);
            }

            int educationId;
            if (!int.TryParse(Convert.ToString(hdnEducationId.Value), out educationId))
            {
                educationId = 0;
            }

            if (educationId <= 0)
            {
                return string.Empty;
            }

            if (lnkEducationCertificate != null && lnkEducationCertificate.Visible)
            {
                string existingPath = Convert.ToString(lnkEducationCertificate.NavigateUrl ?? string.Empty).Trim();
                if (string.IsNullOrWhiteSpace(existingPath))
                {
                    return string.Empty;
                }

                string normalizedPath = existingPath.Replace("\\", "/");
                int slashIndex = normalizedPath.LastIndexOf('/');
                return slashIndex >= 0 ? normalizedPath.Substring(slashIndex + 1) : normalizedPath;
            }

            return string.Empty;
        }

        private string ValueOf(string value)
        {
            return string.IsNullOrWhiteSpace(value) ? string.Empty : value.Trim();
        }

        private string ValueOf(TextBox control)
        {
            return control == null ? string.Empty : ValueOf(control.Text);
        }

        private string SelectedValueOf(ListControl control)
        {
            return control == null ? string.Empty : ValueOf(control.SelectedValue);
        }

        private bool IsValidUiDate(string value)
        {
            return ParseUiDate(value).HasValue;
        }

        private DateTime? ParseUiDate(string value)
        {
            string input = ValueOf(value);
            if (string.IsNullOrWhiteSpace(input))
            {
                return null;
            }

            DateTime parsedDate;
            string[] acceptedFormats = { "dd-MM-yyyy", "yyyy-MM-dd" };
            return DateTime.TryParseExact(input, acceptedFormats, CultureInfo.InvariantCulture, DateTimeStyles.None, out parsedDate)
                ? parsedDate
                : (DateTime?)null;
        }

        private int ParseUiInt(string value)
        {
            int parsedValue;
            return int.TryParse(ValueOf(value), out parsedValue) ? parsedValue : 0;
        }

        private string GetEmployeePhotoValue()
        {
            if (fileEmployeePhoto != null &&
                fileEmployeePhoto.PostedFile != null &&
                !string.IsNullOrWhiteSpace(fileEmployeePhoto.PostedFile.FileName))
            {
                return System.IO.Path.GetFileName(fileEmployeePhoto.PostedFile.FileName);
            }

            if (imgEmployeePhotoPreview != null)
            {
                string previewValue = Convert.ToString(imgEmployeePhotoPreview.ImageUrl ?? string.Empty).Trim();
                if (IsBase64ImageContent(previewValue))
                {
                    return previewValue;
                }
            }

            return lblEmployeePhotoFileName == null
                ? string.Empty
                : Convert.ToString(lblEmployeePhotoFileName.Text ?? string.Empty).Trim();
        }

        private string ValidateAddEmployeeForm()
        {
            if (string.IsNullOrWhiteSpace(ValueOf(txtEmployeeCode))) return "Employee Code is required.";
            if (string.IsNullOrWhiteSpace(ValueOf(txt_name))) return "Username is required.";
            if (string.IsNullOrWhiteSpace(ValueOf(txt_fullname))) return "Employee Name is required.";
            if (!Regex.IsMatch(ValueOf(txt_fullname), @"^[A-Za-z ]+$")) return "Write name in correct format.";
            if (string.IsNullOrWhiteSpace(ValueOf(txtFirstName))) return "First Name is required.";
            if (!Regex.IsMatch(ValueOf(txtFirstName), @"^[A-Za-z ]+$")) return "Write name in correct format.";
            if (!string.IsNullOrWhiteSpace(ValueOf(txtMiddleName)) && !Regex.IsMatch(ValueOf(txtMiddleName), @"^[A-Za-z ]+$")) return "Write name in correct format.";
            if (string.IsNullOrWhiteSpace(ValueOf(txtLastName))) return "Last Name is required.";
            if (!Regex.IsMatch(ValueOf(txtLastName), @"^[A-Za-z ]+$")) return "Write name in correct format.";
            if (string.IsNullOrWhiteSpace(ValueOf(txtDisplayName))) return "Display Name is required.";
            if (!Regex.IsMatch(ValueOf(txtDisplayName), @"^[A-Za-z ]+$")) return "Write name in correct format.";
            if (string.IsNullOrWhiteSpace(SelectedValueOf(txtGender))) return "Gender is required.";
            if (string.IsNullOrWhiteSpace(ValueOf(txtDOB))) return "Date of Birth is required.";
            if (!IsValidUiDate(ValueOf(txtDOB))) return "Date of Birth must be in dd-MM-yyyy format.";
            if (string.IsNullOrWhiteSpace(SelectedValueOf(txtMaritalStatus))) return "Marital Status is required.";
            if (string.IsNullOrWhiteSpace(SelectedValueOf(txtBloodGroup))) return "Blood Group is required.";
            if (string.IsNullOrWhiteSpace(SelectedValueOf(txtNationality))) return "Nationality is required.";
            if (string.IsNullOrWhiteSpace(ValueOf(txtAadhaarNumber))) return "Aadhaar Number is required.";
            if (!Regex.IsMatch(ValueOf(txtAadhaarNumber), @"^\d{12}$")) return "Aadhaar Number must be 12 digits.";
            if (string.IsNullOrWhiteSpace(ValueOf(txtPanNumber))) return "PAN Number is required.";
            if (!Regex.IsMatch(ValueOf(txtPanNumber), @"^[A-Za-z]{5}\d{4}[A-Za-z]{1}$")) return "Enter a valid PAN Number.";
            if (!string.IsNullOrWhiteSpace(ValueOf(txtPassportExpiryDate)) && !IsValidUiDate(ValueOf(txtPassportExpiryDate))) return "Passport Expiry Date must be in dd-MM-yyyy format.";

            if (string.IsNullOrWhiteSpace(ValueOf(txt_contact))) return "Mobile Number is required.";
            if (!Regex.IsMatch(ValueOf(txt_contact), @"^\d{10}$")) return "Mobile Number must be 10 digits.";
            if (string.IsNullOrWhiteSpace(ValueOf(txt_email))) return "Official Email ID is required.";
            if (!Regex.IsMatch(ValueOf(txt_email), @"^[^\s@]+@[^\s@]+\.[^\s@]{2,}$")) return "Enter a valid Official Email ID.";
            if (!string.IsNullOrWhiteSpace(ValueOf(txtPersonalEmail)) && !Regex.IsMatch(ValueOf(txtPersonalEmail), @"^[^\s@]+@[^\s@]+\.[^\s@]{2,}$")) return "Enter a valid Personal Email ID.";

            if (string.IsNullOrWhiteSpace(SelectedValueOf(ddlexporintern))) return "Employment Type is required.";
            if (string.IsNullOrWhiteSpace(ValueOf(txtDateOfJoining))) return "Date Of Joining is required.";
            if (!IsValidUiDate(ValueOf(txtDateOfJoining))) return "Date Of Joining must be in dd-MM-yyyy format.";
            if (string.IsNullOrWhiteSpace(SelectedValueOf(ddlprobationperiod))) return "Probation Period is required.";
            if (!string.IsNullOrWhiteSpace(ValueOf(txtConfirmationDate)) && !IsValidUiDate(ValueOf(txtConfirmationDate))) return "Confirmation Date must be in dd-MM-yyyy format.";
            if (!string.IsNullOrWhiteSpace(ValueOf(txtProbationEndDate)) && !IsValidUiDate(ValueOf(txtProbationEndDate))) return "Probation End Date must be in dd-MM-yyyy format.";
            if (string.IsNullOrWhiteSpace(SelectedValueOf(txtEmployeeStatus))) return "Employee Status is required.";
            if (!string.IsNullOrWhiteSpace(ValueOf(txtExitDate)) && !IsValidUiDate(ValueOf(txtExitDate))) return "Exit Date must be in dd-MM-yyyy format.";

            if (string.IsNullOrWhiteSpace(SelectedValueOf(ddlcompany))) return "Company is required.";
            if (string.IsNullOrWhiteSpace(ValueOf(txtdept))) return "Department is required.";
            if (string.IsNullOrWhiteSpace(ValueOf(txtbranch))) return "Branch Office is required.";
            if (string.IsNullOrWhiteSpace(ValueOf(txtLocation))) return "Location is required.";
            if (string.IsNullOrWhiteSpace(SelectedValueOf(ddldesign))) return "Designation is required.";
            if (string.IsNullOrWhiteSpace(SelectedValueOf(ddl_reportingmanager))) return "Reporting Manager is required.";
            if (string.IsNullOrWhiteSpace(ValueOf(txtFunctionalManager))) return "Functional Manager is required.";
            if (string.IsNullOrWhiteSpace(ValueOf(txtHod))) return "HOD is required.";

            if (string.IsNullOrWhiteSpace(SelectedValueOf(txtAttendanceType))) return "Attendance Type is required.";
            if (string.IsNullOrWhiteSpace(SelectedValueOf(txtWeeklyOff))) return "Weekly Off is required.";
            if (string.IsNullOrWhiteSpace(ValueOf(txtWorkingHours))) return "Working Hours is required.";
            if (!Regex.IsMatch(ValueOf(txtWorkingHours), @"^([0-9]|[01][0-9]|2[0-3]):[0-5][0-9]$")) return "Working Hours must be in HH:mm format.";
            if (string.IsNullOrWhiteSpace(SelectedValueOf(txtAttendancePolicy))) return "Attendance Policy is required.";
            if (string.IsNullOrWhiteSpace(ValueOf(txtPunchingDeviceId))) return "Punching Device ID is required.";
            if (string.IsNullOrWhiteSpace(ValueOf(txtBiometricId))) return "Biometric ID is required.";
            if (IsTruthy(hdnOvertimeEligible.Value) && string.IsNullOrWhiteSpace(ValueOf(txtOvertimeRate))) return "Overtime Rate is required.";
            if (string.IsNullOrWhiteSpace(SelectedValueOf(txtWorkLocation))) return "Work Location is required.";

            return string.Empty;
        }

        private string ValidateOfficialContactForm()
        {
            if (string.IsNullOrWhiteSpace(ValueOf(txt_contact))) return "Mobile Number is required.";
            if (!Regex.IsMatch(ValueOf(txt_contact), @"^\d{10}$")) return "Mobile Number must be 10 digits.";
            if (string.IsNullOrWhiteSpace(ValueOf(txt_email))) return "Official Email ID is required.";
            if (!Regex.IsMatch(ValueOf(txt_email), @"^[^\s@]+@[^\s@]+\.[^\s@]{2,}$")) return "Enter a valid Official Email ID.";
            return string.Empty;
        }

        private DateTime? ParseHtmlDate(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return null;
            }

            DateTime parsed;
            return DateTime.TryParse(value, out parsed) ? parsed : (DateTime?)null;
        }

        private void ShowAssetEditor()
        {
            assetEditorCard.Attributes["class"] = "asset-editor-card";
        }

        private void HideAssetEditor()
        {
            assetEditorCard.Attributes["class"] = "asset-editor-card collapsed";
        }

        private int GetEmployeeUserIdFromQuery()
        {
            int userId;
            return int.TryParse(Convert.ToString(Request.QueryString["user_id"]), out userId) ? userId : 0;
        }

        private void WriteOfficialContactDuplicateValidationResponse()
        {
            string fieldName = Convert.ToString(Request.QueryString["fieldName"] ?? string.Empty);
            string fieldValue = Convert.ToString(Request.QueryString["fieldValue"] ?? string.Empty);
            int employeeUserId;
            int.TryParse(Convert.ToString(Request.QueryString["userId"]), out employeeUserId);

            EmployeeOnboardingBL onboardingBL = new EmployeeOnboardingBL();
            string message = onboardingBL.CheckEmployeeOnboardingDuplicate(fieldName, fieldValue, employeeUserId);
            bool isDuplicate = !string.IsNullOrWhiteSpace(message);

            Response.Clear();
            Response.ContentType = "application/json";
            Response.Write("{\"IsDuplicate\":" + (isDuplicate ? "true" : "false") + ",\"Message\":\"" + HttpUtility.JavaScriptStringEncode(message) + "\"}");
            Response.End();
        }

        private void SelectDropdownValue(DropDownList dropdown, string value)
        {
            if (dropdown == null)
            {
                return;
            }

            ListItem item = dropdown.Items.FindByValue(value ?? string.Empty);
            dropdown.ClearSelection();
            if (item != null)
            {
                item.Selected = true;
            }
        }

        private void ShowAssetMessage(string status, string message)
        {
            if (string.Equals(Convert.ToString(status ?? string.Empty), "Success", StringComparison.OrdinalIgnoreCase))
            {
                return;
            }

            string safeStatus = HttpUtility.JavaScriptStringEncode(status ?? "Failed");
            string safeMessage = HttpUtility.JavaScriptStringEncode(message ?? string.Empty);
            ScriptManager.RegisterStartupScript(this, GetType(), Guid.NewGuid().ToString("N"), "showUserSavedMessage('" + safeStatus + "', '" + safeMessage + "');", true);
        }

        public void BindDesignation()
        {
            List<DropDownData> list1 = new List<DropDownData>();
            CommonBL commonbl = new CommonBL();
            try
            {
                list1 = commonbl.dropdownDesignationSecondary();
                if (list1 != null && list1.Count > 0)
                {
                    ddldesign.DataSource = list1;
                    ddldesign.DataTextField = "Text";
                    ddldesign.DataValueField = "Id";
                }
                else
                {
                    ddldesign.DataSource = null;
                }

                ddldesign.DataBind();
                ddldesign.Items.Insert(0, new ListItem("-- Please Select --", ""));

                // Optional: Set default selected value (if required)
                //string defaultValue = "17"; // e.g., Technician
                //ListItem item = ddldesign.Items.FindByValue(defaultValue);
                //if (item != null)
                //{
                //    ddldesign.SelectedValue = defaultValue;
                //}
            }
            catch (Exception ex)
            {
                CommonBL errorlog = new CommonBL();
                errorlog.fnStoreErrorLog("Adduser", "BindDesignation", "Exception Message=" + ex.Message + " StackTrace=" + ex.StackTrace, UserId);
            }
        }

        public void BindReportinngManager()
        {
            List<DropDownData> list1 = new List<DropDownData>();
            CommonBL commonbl = new CommonBL();
            try
            {
                list1 = commonbl.dropdownassignbySecondary();
                if (list1 != null)
                {
                    ddl_reportingmanager.DataSource = list1;
                    ddl_reportingmanager.DataTextField = "Text";
                    ddl_reportingmanager.DataValueField = "Id";
                }
                else
                {
                    ddl_reportingmanager.DataSource = null;
                }
                ddl_reportingmanager.DataBind();
                ddl_reportingmanager.Items.Insert(0, new ListItem("-- Please Select --", ""));
            }
            catch (Exception ex)
            {
                CommonBL errorlog = new CommonBL();
                errorlog.fnStoreErrorLog("Adduser", "BindReportinngManager", "Exception Message" + ex.Message + "StackTrace=" + ex.StackTrace, UserId);
            }
        }

        public void BindCompany()
        {
            List<DropDownData> list1 = new List<DropDownData>();
            CommonBL commonbl = new CommonBL();
            try
            {
                list1 = commonbl.dropdowCompanySecondary();
                if (list1 != null)
                {
                    ddlcompany.DataSource = list1;
                    ddlcompany.DataTextField = "Text";
                    ddlcompany.DataValueField = "Id";
                }
                else
                {
                    ddlcompany.DataSource = null;
                }
                ddlcompany.DataBind();
                ddlcompany.Items.Insert(0, new ListItem("-- Please Select --", ""));
            }
            catch (Exception ex)
            {
                CommonBL errorlog = new CommonBL();
                errorlog.fnStoreErrorLog("Adduser", "BindCompany", "Exception Message" + ex.Message + "StackTrace=" + ex.StackTrace, UserId);
            }
        }

        public void BindEmployeeExporIntern()
        {
            List<DropDownData> list1 = new List<DropDownData>();
            try
            {
                EmployeeOnboardingBL onboardingBL = new EmployeeOnboardingBL();
                list1 = onboardingBL.BindLookupData("Emptype");
                if (list1 != null)
                {
                    ddlexporintern.DataSource = list1;
                    ddlexporintern.DataTextField = "Text";
                    ddlexporintern.DataValueField = "Id";
                }
                else
                {
                    ddlexporintern.DataSource = null;
                }
                ddlexporintern.DataBind();
                ddlexporintern.Items.Insert(0, new ListItem("-- Please Select --", ""));
            }
            catch (Exception ex)
            {
                CommonBL errorlog = new CommonBL();
                errorlog.fnStoreErrorLog("AddTask", "BindEmployeeExporIntern", "Exception Message" + ex.Message + "StackTrace=" + ex.StackTrace, UserId);
            }
        }
        private void BindUserData(int userId)
        {
            try
            {
                UserDetailsBL userBAL = new UserDetailsBL();
                string requestedEmpCode = Convert.ToString(Request.QueryString["emp_code"]);
                List<UserDetailsDO> userDetailsList = new List<UserDetailsDO>();
                userDetailsList = userBAL.GetUserDetailsFromSecondary(userId, requestedEmpCode);
                if ((userDetailsList == null || userDetailsList.Count == 0) && (!string.IsNullOrWhiteSpace(requestedEmpCode) || userId > 0))
                {
                    List<UserDetailsDO> secondaryUsers = userBAL.ViewAllUsers();
                    UserDetailsDO fallbackUser = null;

                    if (!string.IsNullOrWhiteSpace(requestedEmpCode))
                    {
                        fallbackUser = secondaryUsers.FirstOrDefault(x =>
                            string.Equals(
                                Convert.ToString(x.EmployeeCode ?? string.Empty).Trim(),
                                requestedEmpCode.Trim(),
                                StringComparison.OrdinalIgnoreCase
                            ));
                    }

                    if (fallbackUser == null && userId > 0)
                    {
                        fallbackUser = secondaryUsers.FirstOrDefault(x => x.UserId == userId);
                    }

                    if (fallbackUser != null)
                    {
                        userDetailsList = new List<UserDetailsDO> { fallbackUser };
                    }
                }

                if (userId != 0)
                {
                    if (userDetailsList.Count > 0)
                    {
                        UserDetailsDO userDetails = userDetailsList[0];
                        SetText(txtEmployeeId, userDetails.UserId > 0 ? userDetails.UserId.ToString() : string.Empty);
                        txtEmployeeCode.Text = userDetails.EmployeeCode;
                        txt_name.Text = userDetails.Username;
                        txt_fullname.Text = userDetails.user_fullname;
                        txt_email.Text = userDetails.user_mail_id;
                        txt_contact.Text = userDetails.contact_detail;
                        OriginalOfficialEmail = txt_email.Text;
                        OriginalOfficialMobile = txt_contact.Text;
                        SetDropDownValue(ddldesign, userDetails.designation_name);

                        SetText(txtFirstName, userDetails.FirstName);
                        SetText(txtMiddleName, userDetails.MiddleName);
                        SetText(txtLastName, userDetails.LastName);
                        SetText(txtDisplayName, userDetails.DisplayName);
                        SetText(txtGender, userDetails.Gender);
                        SetText(txtDOB, FormatDateForInput(userDetails.DateOfBirth));
                        SetText(txtAge, userDetails.Age > 0 ? userDetails.Age.ToString() : string.Empty);
                        SetText(txtMaritalStatus, userDetails.MaritalStatus);
                        SetText(txtBloodGroup, userDetails.BloodGroup);
                        SetText(txtNationality, userDetails.Nationality);
                        SetText(txtAadhaarNumber, userDetails.AadhaarNumber);
                        SetText(txtPanNumber, userDetails.PanNumber);
                        SetText(txtPassportNumber, userDetails.PassportNumber);
                        SetText(txtPassportExpiryDate, FormatDateForInput(userDetails.PassportExpiryDate));
                        BindEmployeePhoto(userDetails.EmployeePhoto);
                        SetText(txtAlternateMobile, userDetails.AlternateMobileNumber);
                        SetText(txtPersonalEmail, userDetails.PersonalEmail);
                        SetText(txtPermanentHouseNumber, userDetails.PermanentHouseNumber);
                        SetText(txtPermanentBuildingName, userDetails.PermanentBuildingName);
                        SetText(txtPermanentStreet, userDetails.PermanentStreet);
                        SetText(txtPermanentArea, userDetails.PermanentArea);
                        SetText(txtPermanentLandmark, userDetails.PermanentLandmark);
                        SetText(txtPermanentCity, userDetails.PermanentCity);
                        SetText(txtPermanentDistrict, userDetails.PermanentDistrict);
                        SetText(txtPermanentState, userDetails.PermanentState);
                        SetText(txtPermanentCountry, userDetails.PermanentCountry);
                        SetText(txtPermanentPinCode, userDetails.PermanentPinCode);
                        chkSameAsPermanent.Checked = IsTruthy(userDetails.SameAsPermanent);
                        SetText(txtCurrentHouseNumber, userDetails.CurrentHouseNumber);
                        SetText(txtCurrentBuildingName, userDetails.CurrentBuildingName);
                        SetText(txtCurrentStreet, userDetails.CurrentStreet);
                        SetText(txtCurrentArea, userDetails.CurrentArea);
                        SetText(txtCurrentLandmark, userDetails.CurrentLandmark);
                        SetText(txtCurrentCity, userDetails.CurrentCity);
                        SetText(txtCurrentDistrict, userDetails.CurrentDistrict);
                        SetText(txtCurrentState, userDetails.CurrentState);
                        SetText(txtCurrentCountry, userDetails.CurrentCountry);
                        SetText(txtCurrentPinCode, userDetails.CurrentPinCode);
                        SetText(txtEmergencyContactName, userDetails.EmergencyContactName);
                        SetText(txtEmergencyContactNumber, userDetails.EmergencyContactNumber);
                        SetText(txtEmergencyContactRelationship, userDetails.EmergencyContactRelationship);
                        _contactSectionComplete = HasContactSectionData(userDetails);
                        txtESICNo.Text = userDetails.ESIC_no.ToString();
                        txtPFNo.Text = userDetails.PF_no.ToString();
                        txtbranch.Text = userDetails.branch;
                        txtdept.Text = userDetails.department;

                        if (userDetails.probation_period_months != null)
                            SetDropDownValue(ddlprobationperiod, userDetails.probation_period_months.ToString());

                        if (userDetails.date_of_joining != null)
                        {
                            DateTime doj = Convert.ToDateTime(userDetails.date_of_joining);
                            txtDateOfJoining.Text = doj.ToString("yyyy-MM-dd");
                        }
                        SetText(txtConfirmationDate, FormatDateForInput(userDetails.ConfirmationDate));
                        SetText(txtProbationEndDate, FormatDateForInput(userDetails.ProbationEndDate));
                        SetText(txtEmployeeStatus, userDetails.EmployeeStatus);
                        SetText(txtNoticePeriod, userDetails.NoticePeriod > 0 ? userDetails.NoticePeriod.ToString() : string.Empty);
                        SetText(txtExitDate, FormatDateForInput(userDetails.ExitDate));
                        SetText(txtSeparationReason, userDetails.SeparationReason);
                        SetText(txtProbationStatus, userDetails.ProbationStatus);
                        SetText(txtProbationRemarks, userDetails.ProbationRemarks);
                        SetText(txtLocation, userDetails.Location);
                        SetText(txtFunctionalManager, userDetails.FunctionalManagerName);
                        SetText(txtHod, userDetails.HodName);
                        SetText(txtEmployeeLevel, userDetails.EmployeeLevel);
                        SetText(txtAttendanceType, userDetails.AttendanceType);
                        SetText(txtWeeklyOff, userDetails.WeeklyOff);
                        SetText(txtWorkingHours, FormatWorkingHours(userDetails.WorkingHours));
                        SetText(txtPunchingDeviceId, userDetails.PunchingDeviceId);
                        SetText(txtBiometricId, userDetails.BiometricId);
                        SetText(txtAttendancePolicy, userDetails.AttendancePolicy);
                        hdnOvertimeEligible.Value = Convert.ToString(userDetails.OvertimeEligible);
                        SetText(txtOvertimeRate, userDetails.OvertimeRate);
                        SetText(txtWorkLocation, userDetails.WorkLocation);
                        //SetText(txtRemoteWorkAllowed, userDetails.RemoteWorkAllowed);
                        _attendanceSectionComplete = HasAnyText(
                            userDetails.AttendanceType,
                            userDetails.WeeklyOff,
                            FormatWorkingHours(userDetails.WorkingHours),
                            userDetails.PunchingDeviceId,
                            userDetails.BiometricId,
                            userDetails.AttendancePolicy,
                            userDetails.OvertimeRate,
                            userDetails.WorkLocation);
                        SetText(txtBankName, userDetails.BankName);
                        SetText(txtBankBranchName, userDetails.BankBranchName);
                        SetText(txtAccountHolderName, userDetails.AccountHolderName);
                        SetText(txtAccountNumber, userDetails.AccountNumber);
                        SetText(txtConfirmAccountNumber, userDetails.ConfirmAccountNumber);
                        SetText(txtIfscCode, userDetails.IfscCode);
                        SetText(txtAccountType, userDetails.AccountType);
                        SetText(txtSalaryAccountFlag, userDetails.SalaryAccountFlagText);
                        _bankSectionComplete = HasAnyText(
                            userDetails.BankName,
                            userDetails.BankBranchName,
                            userDetails.AccountHolderName,
                            userDetails.AccountNumber,
                            userDetails.ConfirmAccountNumber,
                            userDetails.IfscCode,
                            userDetails.AccountType,
                            userDetails.SalaryAccountFlagText);
                        SetText(txtQualificationLevel, userDetails.QualificationLevel);
                        SetText(txtDegreeName, userDetails.DegreeName);
                        SetText(txtSpecialization, userDetails.Specialization);
                        SetText(txtUniversity, userDetails.University);
                        SetText(txtInstituteName, userDetails.InstituteName);
                        SetText(txtYearOfPassing, userDetails.YearOfPassing);
                        SetText(txtPercentageCgpa, userDetails.PercentageCgpa);
                        BindDocumentLink(lnkEducationCertificate, fileEducationCertificate, userDetails.EducationCertificateFile);
                        SetText(txtCertificationName, userDetails.CertificationName);
                        SetText(txtCertificationAuthority, userDetails.CertificationAuthority);
                        SetText(txtCertificateNumber, userDetails.CertificateNumber);
                        SetText(txtIssueDate, FormatDateForInput(userDetails.IssueDate));
                        SetText(txtExpiryDate, FormatDateForInput(userDetails.ExpiryDate));
                        SetText(txtRenewalRequired, userDetails.RenewalRequired);
                        SetLabelText(lblCertificationName, userDetails.CertificationName);
                        SetLabelText(lblCertificationAuthority, userDetails.CertificationAuthority);
                        SetLabelText(lblCertificateNumber, userDetails.CertificateNumber);
                        SetLabelText(lblCertificationIssueDate, FormatDate(userDetails.IssueDate));
                        SetLabelText(lblCertificationExpiryDate, FormatDate(userDetails.ExpiryDate));
                        SetLabelText(lblRenewalRequired, string.IsNullOrWhiteSpace(userDetails.RenewalRequired) ? "No" : userDetails.RenewalRequired);
                        BindCertificationFile(userDetails.CertificationFile);
                        _certificationSectionComplete = HasCertificationSectionData(userDetails);
                        ClearAssetForm();

                        if (userDetails.reporting_manager != null)
                        {
                            string rep = userDetails.reporting_manager.ToString();

                            SetDropDownValue(ddl_reportingmanager, rep);
                        }

                        if (userDetails.employee_type != null)
                        {
                            SetDropDownValue(ddlexporintern, userDetails.EmploymentTypeId);
                            if (string.IsNullOrWhiteSpace(ddlexporintern.SelectedValue))
                            {
                                SetDropDownValue(ddlexporintern, userDetails.employee_type);
                            }
                        }
                        SetDropDownValue(ddlcompany, userDetails.company_name);
                        if (userDetails.company_id != null)
                        {
                            string compId = userDetails.company_id.ToString();

                            SetDropDownValue(ddlcompany, compId);
                        }   


                        if (userDetails.designation_id != null)
                        {
                            string desigId = userDetails.designation_id.ToString();

                            SetDropDownValue(ddldesign, desigId);
                        }

                        txt_password.Visible = false;
                        lblpass.Visible = false;

                        if (Request.QueryString["mode"] == "edit")
                        {
                            txt_name.Enabled = true;
                            txt_fullname.Enabled = true;
                            txt_email.Enabled = true;
                            txt_contact.Enabled = true;

                            txtESICNo.Enabled = true;
                            txtPFNo.Enabled = true;
                            txtbranch.Enabled = true;
                            txtdept.Enabled = true;

                           
                            lbluser.Text = "Employee Update";
                            btn_submit.Text = "Update Employee";
                            btn_submit.Visible = true;
                            btnUpdateOfficialContact.Visible = true;
                            btn_rest.Text = "Clear";
                            btn_rest.Visible = true;

                            ddldesign.Enabled = true;
                            ddlcompany.Enabled = true;
                            ddl_reportingmanager.Enabled = true;
                            ddlprobationperiod.Enabled = true;
                            ddlexporintern.Enabled = true;

                        }
                    }
                }
                else
                {
                    //txtEmployeeCode.Text = userDetailsList[0].EmployeeCode;
                   // txtEmployeeCode.Enabled = false;
                }

            }
            catch (Exception exs)
            {
                CommonBL errorlog = new CommonBL();
                errorlog.fnStoreErrorLog("AddUsers", "BindUserData", "Exception Message: " + exs.Message + " StackTrace: " + exs.StackTrace, UserId);
            }

        }

        private UserDetailsDO MergeUserDetails(UserDetailsDO primary, UserDetailsDO secondary, UserDetailsDO mergedBase)
        {
            UserDetailsDO result = secondary ?? primary ?? mergedBase;
            if (result == null)
            {
                return null;
            }

            Func<string, string, string, string> pickString = (a, b, c) =>
                !string.IsNullOrWhiteSpace(a) ? a :
                !string.IsNullOrWhiteSpace(b) ? b :
                !string.IsNullOrWhiteSpace(c) ? c : string.Empty;

            Func<int, int, int, int> pickInt = (a, b, c) =>
                a > 0 ? a : (b > 0 ? b : (c > 0 ? c : 0));

            result.EmployeeCode = pickString(result.EmployeeCode, primary != null ? primary.EmployeeCode : null, mergedBase != null ? mergedBase.EmployeeCode : null);
            result.Username = pickString(result.Username, primary != null ? primary.Username : null, mergedBase != null ? mergedBase.Username : null);
            result.user_fullname = pickString(result.user_fullname, primary != null ? primary.user_fullname : null, mergedBase != null ? mergedBase.user_fullname : null);
            result.user_mail_id = pickString(result.user_mail_id, primary != null ? primary.user_mail_id : null, mergedBase != null ? mergedBase.user_mail_id : null);
            result.contact_detail = pickString(result.contact_detail, primary != null ? primary.contact_detail : null, mergedBase != null ? mergedBase.contact_detail : null);
            result.designation_name = pickString(result.designation_name, primary != null ? primary.designation_name : null, mergedBase != null ? mergedBase.designation_name : null);

            result.designation_id = pickInt(result.designation_id, primary != null ? primary.designation_id : 0, mergedBase != null ? mergedBase.designation_id : 0);
            result.company_id = pickInt(result.company_id, primary != null ? primary.company_id : 0, mergedBase != null ? mergedBase.company_id : 0);
            result.CompanyId = pickInt(result.CompanyId, primary != null ? primary.CompanyId : 0, mergedBase != null ? mergedBase.CompanyId : 0);
            result.company_name = pickString(result.company_name, primary != null ? primary.company_name : null, mergedBase != null ? mergedBase.company_name : null);

            result.ESIC_no = pickInt(result.ESIC_no, primary != null ? primary.ESIC_no : 0, mergedBase != null ? mergedBase.ESIC_no : 0);
            result.PF_no = pickInt(result.PF_no, primary != null ? primary.PF_no : 0, mergedBase != null ? mergedBase.PF_no : 0);
            result.department = pickString(result.department, primary != null ? primary.department : null, mergedBase != null ? mergedBase.department : null);
            result.branch = pickString(result.branch, primary != null ? primary.branch : null, mergedBase != null ? mergedBase.branch : null);
            result.division = pickString(result.division, primary != null ? primary.division : null, mergedBase != null ? mergedBase.division : null);
            result.reporting_manager = pickString(result.reporting_manager, primary != null ? primary.reporting_manager : null, mergedBase != null ? mergedBase.reporting_manager : null);
            result.employee_type = pickString(result.employee_type, primary != null ? primary.employee_type : null, mergedBase != null ? mergedBase.employee_type : null);
            result.EmploymentTypeId = pickString(result.EmploymentTypeId, primary != null ? primary.EmploymentTypeId : null, mergedBase != null ? mergedBase.EmploymentTypeId : null);
            result.probation_period_months = pickInt(result.probation_period_months, primary != null ? primary.probation_period_months : 0, mergedBase != null ? mergedBase.probation_period_months : 0);

            if (result.date_of_joining == DateTime.MinValue)
            {
                if (primary != null && primary.date_of_joining > DateTime.MinValue)
                {
                    result.date_of_joining = primary.date_of_joining;
                }
                else if (mergedBase != null && mergedBase.date_of_joining > DateTime.MinValue)
                {
                    result.date_of_joining = mergedBase.date_of_joining;
                }
            }

            return result;
        }

        private bool IsOnlyBasicUserData(UserDetailsDO user)
        {
            if (user == null) return true;

            bool hasAnyEmploymentData =
                user.ESIC_no > 0 ||
                user.PF_no > 0 ||
                !string.IsNullOrWhiteSpace(user.department) ||
                !string.IsNullOrWhiteSpace(user.branch) ||
                !string.IsNullOrWhiteSpace(user.division) ||
                user.probation_period_months > 0 ||
                !string.IsNullOrWhiteSpace(user.reporting_manager) ||
                !string.IsNullOrWhiteSpace(user.employee_type) ||
                user.date_of_joining > DateTime.MinValue;

            return !hasAnyEmploymentData;
        }

        private void SetDropDownValue(DropDownList ddl, string valueOrText)
        {
            if (ddl == null || string.IsNullOrWhiteSpace(valueOrText))
            {
                return;
            }

            ListItem item = ddl.Items.FindByValue(valueOrText);
            if (item == null)
            {
                item = ddl.Items.FindByText(valueOrText);
            }

            if (item != null)
            {
                ddl.ClearSelection();
                item.Selected = true;
            }
        }

        private void SetText(TextBox textBox, string value)
        {
            if (textBox != null)
            {
                textBox.Text = value ?? string.Empty;
            }
        }

        private void SetText(DropDownList dropdown, string value)
        {
            SetDropDownValue(dropdown, value);
        }

        private void SetLabelText(Label label, string value)
        {
            if (label != null)
            {
                label.Text = string.IsNullOrWhiteSpace(value) ? "-" : value;
            }
        }

        private void BindDocumentLink(HyperLink link, System.Web.UI.HtmlControls.HtmlInputFile fileInput, string filePath)
        {
            if (link == null)
            {
                return;
            }

            string url = BuildDocumentUrl(filePath);
            bool hasFile = !string.IsNullOrWhiteSpace(url);
            link.Visible = hasFile;
            link.NavigateUrl = url;

            if (fileInput != null)
            {
                fileInput.Style["display"] = hasFile ? "none" : "block";
            }
        }

        private void BindEmployeePhoto(string filePath)
        {
            string rawValue = Convert.ToString(filePath ?? string.Empty).Trim();
            bool isBase64Photo = IsBase64ImageContent(rawValue);
            string url = isBase64Photo ? BuildBase64ImageUrl(rawValue) : BuildDocumentUrl(rawValue);
            bool hasPhoto = !string.IsNullOrWhiteSpace(url);

            //if (lnkEmployeePhoto != null)
            //{
            //    lnkEmployeePhoto.NavigateUrl = url;
            //    lnkEmployeePhoto.Visible = hasPhoto;
            //}

            if (imgEmployeePhotoPreview != null)
            {
                imgEmployeePhotoPreview.ImageUrl = url;
                imgEmployeePhotoPreview.Visible = hasPhoto;
            }

            if (lblEmployeePhotoFileName != null)
            {
                string fileName = rawValue;
                if (isBase64Photo)
                {
                    fileName = "EmployeePhoto.png";
                }
                else if (!string.IsNullOrWhiteSpace(fileName))
                {
                    fileName = fileName.Replace("\\", "/");
                    int slashIndex = fileName.LastIndexOf('/');
                    if (slashIndex >= 0)
                    {
                        fileName = fileName.Substring(slashIndex + 1);
                    }
                }

                lblEmployeePhotoFileName.Text = fileName;
            }

            if (employeePhotoPreviewState != null)
            {
                employeePhotoPreviewState.Attributes["class"] = hasPhoto ? "employee-photo-existing" : "employee-photo-existing is-hidden";
            }

            if (employeePhotoEmptyState != null)
            {
                employeePhotoEmptyState.Attributes["class"] = hasPhoto ? "employee-photo-empty is-hidden" : "employee-photo-empty";
            }
        }

        private void BindCertificationFile(string filePath)
        {
            string url = BuildCertificationViewUrl(filePath);
            bool hasFile = !string.IsNullOrWhiteSpace(url);
            bool isBase64Image = IsBase64ImageContent(filePath);
            bool isBase64Pdf = IsBase64PdfContent(filePath);
            string normalizedFilePath = Convert.ToString(filePath ?? string.Empty).Trim();

            if (lnkCertificationFile != null)
            {
                lnkCertificationFile.NavigateUrl = url;
                lnkCertificationFile.Visible = hasFile;
                lnkCertificationFile.Attributes["data-file-url"] = url;
                lnkCertificationFile.Attributes["data-file-type"] = isBase64Pdf ? "pdf" : "image";

                if (isBase64Image || isBase64Pdf)
                {
                    lnkCertificationFile.Target = string.Empty;
                    lnkCertificationFile.Attributes["onclick"] = "return openCertificationFileModal(this);";
                }
                else
                {
                    lnkCertificationFile.Target = "_blank";
                    lnkCertificationFile.Attributes.Remove("onclick");
                }
            }

            if (lblCertificationFileName != null)
            {
                lblCertificationFileName.Text = hasFile ? GetFriendlyDocumentFileName(normalizedFilePath, isBase64Pdf ? "Certificate.pdf" : "Certificate.png") : "-";
            }

            if (lblCertificationFileMeta != null)
            {
                lblCertificationFileMeta.Text = hasFile
                    ? (isBase64Pdf ? "PDF  .  File available in employee record" : "Image  .  File available in employee record")
                    : "-";
            }

            if (lblCertificationNoFile != null)
            {
                lblCertificationNoFile.Visible = !hasFile;
            }
        }

        private string BuildCertificationViewUrl(string filePath)
        {
            string value = Convert.ToString(filePath ?? string.Empty).Trim();
            if (string.IsNullOrWhiteSpace(value))
            {
                return string.Empty;
            }

            if (IsBase64PdfContent(value))
            {
                return BuildBase64PdfUrl(value);
            }

            if (IsBase64ImageContent(value))
            {
                return BuildBase64ImageUrl(value);
            }

            return BuildDocumentUrl(value);
        }

        private bool IsBase64ImageContent(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return false;
            }

            string normalized = value.Trim();
            if (normalized.StartsWith("data:image/", StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }

            return normalized.StartsWith("/9j/", StringComparison.OrdinalIgnoreCase) ||
                   normalized.StartsWith("iVBOR", StringComparison.OrdinalIgnoreCase) ||
                   normalized.StartsWith("R0lGOD", StringComparison.OrdinalIgnoreCase) ||
                   normalized.StartsWith("Qk", StringComparison.OrdinalIgnoreCase);
        }

        private string BuildBase64ImageUrl(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return string.Empty;
            }

            string normalized = value.Trim();
            if (normalized.StartsWith("data:image/", StringComparison.OrdinalIgnoreCase))
            {
                return normalized;
            }

            string mimeType = normalized.StartsWith("/9j/", StringComparison.OrdinalIgnoreCase)
                ? "image/jpeg"
                : normalized.StartsWith("R0lGOD", StringComparison.OrdinalIgnoreCase)
                    ? "image/gif"
                    : normalized.StartsWith("Qk", StringComparison.OrdinalIgnoreCase)
                        ? "image/bmp"
                        : "image/png";

            return "data:" + mimeType + ";base64," + normalized;
        }

        private bool IsBase64PdfContent(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return false;
            }

            string normalized = value.Trim();
            if (normalized.StartsWith("data:application/pdf", StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }

            return normalized.StartsWith("JVBER", StringComparison.OrdinalIgnoreCase);
        }

        private string BuildBase64PdfUrl(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return string.Empty;
            }

            string normalized = value.Trim();
            if (normalized.StartsWith("data:application/pdf", StringComparison.OrdinalIgnoreCase))
            {
                return normalized;
            }

            return "data:application/pdf;base64," + normalized;
        }

        private string BuildDocumentUrl(string filePath)
        {
            string value = Convert.ToString(filePath ?? string.Empty).Trim();
            if (string.IsNullOrWhiteSpace(value))
            {
                return string.Empty;
            }

            if (value.StartsWith("http://", StringComparison.OrdinalIgnoreCase) ||
                value.StartsWith("https://", StringComparison.OrdinalIgnoreCase) ||
                value.StartsWith("/", StringComparison.OrdinalIgnoreCase) ||
                value.StartsWith("~/", StringComparison.OrdinalIgnoreCase))
            {
                return ResolveUrl(value);
            }

            return ResolveUrl("~/documents/" + value.TrimStart('\\', '/'));
        }

        private string GetFriendlyDocumentFileName(string filePath, string fallbackName)
        {
            string value = Convert.ToString(filePath ?? string.Empty).Trim();
            if (string.IsNullOrWhiteSpace(value))
            {
                return fallbackName;
            }

            if (IsBase64PdfContent(value) || IsBase64ImageContent(value))
            {
                return fallbackName;
            }

            value = value.Replace("\\", "/");
            int slashIndex = value.LastIndexOf('/');
            if (slashIndex >= 0 && slashIndex < value.Length - 1)
            {
                value = value.Substring(slashIndex + 1);
            }

            return string.IsNullOrWhiteSpace(value) ? fallbackName : value;
        }

        private string FormatDate(DateTime? value)
        {
            return value.HasValue && value.Value != DateTime.MinValue ? value.Value.ToString("dd-MM-yyyy") : string.Empty;
        }

        private string FormatDateForInput(DateTime? value)
        {
            return value.HasValue && value.Value != DateTime.MinValue ? value.Value.ToString("yyyy-MM-dd") : string.Empty;
        }

        private string FormatWorkingHours(string value)
        {
            if (TimeSpan.TryParse(value, out TimeSpan time))
            {
                return time.ToString(@"hh\:mm");
            }

            return value ?? string.Empty;
        }

        private bool IsTruthy(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return false;
            }

            string normalized = value.Trim().ToLowerInvariant();
            return normalized == "1" || normalized == "60" || normalized == "true" || normalized == "yes" || normalized == "y";
        }
        protected void SubmitButtonClick(object sender, EventArgs e)
        {
            UserDetailsDO user = new UserDetailsDO();
            ResponseDO response = new ResponseDO();
            try
            {
                string validationMessage = ValidateAddEmployeeForm();
                if (!string.IsNullOrWhiteSpace(validationMessage))
                {
                    ClientScript.RegisterStartupScript(this.GetType(), "UserSavedScript",
                              "showUserSavedMessage('Failed', '" + HttpUtility.JavaScriptStringEncode(validationMessage) + "');", true);
                    return;
                }

                if (btn_submit.Text == "Submit")
                {
                    int userId = GetClientIdFromSession();

                    user.user_type = "2"; //11 means emp id
                    user.Insertedby = userId;
                    user.CompanyId = 3; // Default
                    UserDetailsBL userdetails = new UserDetailsBL();
                    user.Username = txt_name.Text;
                    user.user_fullname = txt_fullname.Text;
                    user.user_mail_id = txt_email.Text;
                    user.contact_detail = txt_contact.Text;
                    user.password = DefaultEmployeePassword;
                    user.EmployeeCode = txtEmployeeCode.Text;
                    user.designation_id = Convert.ToInt32(ddldesign.SelectedValue);
                    user.designation_name = ddldesign.SelectedItem != null ? ddldesign.SelectedItem.Text : string.Empty;
                    string emailId = user.user_mail_id;

                    user.company_id = Convert.ToInt32(ddlcompany.SelectedValue);

                    user.ESIC_no = string.IsNullOrWhiteSpace(txtESICNo.Text) ? 0 : Convert.ToInt32(txtESICNo.Text);
                    user.PF_no = string.IsNullOrWhiteSpace(txtPFNo.Text) ? 0 : Convert.ToInt32(txtPFNo.Text);
                    user.branch = txtbranch.Text;
                    user.department = txtdept.Text;
                    user.division = string.Empty;
                    DateTime? joiningDate = ParseUiDate(txtDateOfJoining.Text);
                    if (!joiningDate.HasValue)
                    {
                        ClientScript.RegisterStartupScript(this.GetType(), "UserSavedScript",
                                  "showUserSavedMessage('Failed', 'Please enter Date Of Joining in dd-MM-yyyy format.');", true);
                        return;
                    }
                    user.date_of_joining = joiningDate.Value;
                    user.probation_period_months = Convert.ToInt32(ddlprobationperiod.SelectedValue);
                    user.reporting_manager = string.IsNullOrWhiteSpace(ddl_reportingmanager.SelectedValue) ? "0" : ddl_reportingmanager.SelectedValue;
                    user.employee_type = ddlexporintern.SelectedValue;

                    string username = user.Username;
                    string password = user.password;
                    List<UserDetailsDO> userdata = userdetails.SaveUserDetails(user);
                    if (string.Equals(userdata[0].Status, "Success", StringComparison.OrdinalIgnoreCase))
                    {
                        try
                        {
                            string status = userdata[0].Status;
                            string remark = userdata[0].Remarks;

                            ClientScript.RegisterStartupScript(this.GetType(), "UserSavedScript",
                                  "showUserSavedMessage('" + status + "', '" + remark + "');" +
                                  "setTimeout(function(){ window.location.href = '" + EmployeeListUrl + "'; }, 5000);", true);
                            userdetails.SendUserCredentialsMail(emailId, username, password);

                            ClearButton_Click(sender, e);
                        }
                        catch (Exception ex)
                        {
                            CommonBL errorlog = new CommonBL();
                            errorlog.fnStoreErrorLog("addUser", "SubmitButtonClick", "Exception Message" + ex.Message + "Strace=" + ex.StackTrace, UserId);
                        }
                    }
                    else
                    {
                        string status = userdata[0].Status;
                        string remark = userdata[0].Remarks;
                        ClientScript.RegisterStartupScript(this.GetType(), "UserSavedScript",
                                  "showUserSavedMessage('" + status + "', '" + remark + "');" +
                                  "setTimeout(function(){ window.location.href = '" + EmployeeListUrl + "'; }, 5000);", true);
                        return;
                    }

                }
                else if (btn_submit.Text == "Update" || btn_submit.Text == "Update Employee")
                {
                    user.UserId = Convert.ToInt32(Request.QueryString["user_id"]);
                    UserDetailsBL userdetails = new UserDetailsBL();
                    user.EmployeeCode = txtEmployeeCode.Text;
                    user.Username = txt_name.Text;
                    user.user_fullname = txt_fullname.Text;
                    user.FirstName = ValueOf(txtFirstName);
                    user.MiddleName = ValueOf(txtMiddleName);
                    user.LastName = ValueOf(txtLastName);
                    user.DisplayName = ValueOf(txtDisplayName);
                    user.Gender = SelectedValueOf(txtGender);
                    user.DateOfBirth = ParseUiDate(ValueOf(txtDOB));
                    user.MaritalStatus = SelectedValueOf(txtMaritalStatus);
                    user.BloodGroup = SelectedValueOf(txtBloodGroup);
                    user.Nationality = SelectedValueOf(txtNationality);
                    user.AadhaarNumber = ValueOf(txtAadhaarNumber);
                    user.PanNumber = ValueOf(txtPanNumber);
                    user.PassportNumber = ValueOf(txtPassportNumber);
                    user.PassportExpiryDate = ParseUiDate(ValueOf(txtPassportExpiryDate));
                    user.EmployeePhoto = GetEmployeePhotoValue();
                    user.designation_id = Convert.ToInt32(ddldesign.SelectedValue);
                    user.user_mail_id = txt_email.Text;
                    user.contact_detail = txt_contact.Text;
                    user.AlternateMobileNumber = ValueOf(txtAlternateMobile);
                    user.PersonalEmail = ValueOf(txtPersonalEmail);
                    user.PermanentHouseNumber = ValueOf(txtPermanentHouseNumber);
                    user.PermanentBuildingName = ValueOf(txtPermanentBuildingName);
                    user.PermanentStreet = ValueOf(txtPermanentStreet);
                    user.PermanentArea = ValueOf(txtPermanentArea);
                    user.PermanentLandmark = ValueOf(txtPermanentLandmark);
                    user.PermanentCity = ValueOf(txtPermanentCity);
                    user.PermanentDistrict = ValueOf(txtPermanentDistrict);
                    user.PermanentState = ValueOf(txtPermanentState);
                    user.PermanentCountry = ValueOf(txtPermanentCountry);
                    user.PermanentPinCode = ValueOf(txtPermanentPinCode);
                    user.SameAsPermanent = chkSameAsPermanent.Checked ? "1" : "0";
                    user.CurrentHouseNumber = ValueOf(txtCurrentHouseNumber);
                    user.CurrentBuildingName = ValueOf(txtCurrentBuildingName);
                    user.CurrentStreet = ValueOf(txtCurrentStreet);
                    user.CurrentArea = ValueOf(txtCurrentArea);
                    user.CurrentLandmark = ValueOf(txtCurrentLandmark);
                    user.CurrentCity = ValueOf(txtCurrentCity);
                    user.CurrentDistrict = ValueOf(txtCurrentDistrict);
                    user.CurrentState = ValueOf(txtCurrentState);
                    user.CurrentCountry = ValueOf(txtCurrentCountry);
                    user.CurrentPinCode = ValueOf(txtCurrentPinCode);
                    user.EmergencyContactName = ValueOf(txtEmergencyContactName);
                    user.EmergencyContactNumber = ValueOf(txtEmergencyContactNumber);
                    user.EmergencyContactRelationship = ValueOf(txtEmergencyContactRelationship);

                    user.company_id = Convert.ToInt32(ddlcompany.SelectedValue);
                    user.ESIC_no = ParseUiInt(ValueOf(txtESICNo));
                    user.PF_no = ParseUiInt(ValueOf(txtPFNo));
                    user.branch = txtbranch.Text;
                    user.department = txtdept.Text;
                    user.division = string.Empty;
                    DateTime? updateJoiningDate = ParseUiDate(txtDateOfJoining.Text);
                    if (!updateJoiningDate.HasValue)
                    {
                        ClientScript.RegisterStartupScript(this.GetType(), "UserSavedScript",
                                  "showUserSavedMessage('Failed', 'Please enter Date Of Joining in dd-MM-yyyy format.');", true);
                        return;
                    }
                    user.date_of_joining = updateJoiningDate.Value;
                    user.probation_period_months = Convert.ToInt32(ddlprobationperiod.SelectedValue);
                    user.reporting_manager = ddl_reportingmanager.SelectedValue;
                    user.employee_type = ddlexporintern.SelectedValue;
                    user.ConfirmationDate = ParseUiDate(ValueOf(txtConfirmationDate));
                    user.ProbationEndDate = ParseUiDate(ValueOf(txtProbationEndDate));
                    user.EmployeeStatus = SelectedValueOf(txtEmployeeStatus);
                    user.NoticePeriod = ParseUiInt(ValueOf(txtNoticePeriod));
                    user.ExitDate = ParseUiDate(ValueOf(txtExitDate));
                    user.SeparationReason = SelectedValueOf(txtSeparationReason);
                    user.Location = ValueOf(txtLocation);
                    user.FunctionalManagerName = ValueOf(txtFunctionalManager);
                    user.HodName = ValueOf(txtHod);
                    user.EmployeeLevel = SelectedValueOf(txtEmployeeLevel);
                    user.AttendanceType = SelectedValueOf(txtAttendanceType);
                    user.WeeklyOff = SelectedValueOf(txtWeeklyOff);
                    user.WorkingHours = ValueOf(txtWorkingHours);
                    user.PunchingDeviceId = ValueOf(txtPunchingDeviceId);
                    user.BiometricId = ValueOf(txtBiometricId);
                    user.AttendancePolicy = SelectedValueOf(txtAttendancePolicy);
                    user.OvertimeEligible = IsTruthy(hdnOvertimeEligible.Value) ? "60" : "61";
                    user.OvertimeRate = ValueOf(txtOvertimeRate);
                    user.WorkLocation = SelectedValueOf(txtWorkLocation);
                    user.BankName = ValueOf(txtBankName);
                    user.BankBranchName = ValueOf(txtBankBranchName);
                    user.AccountHolderName = ValueOf(txtAccountHolderName);
                    user.AccountNumber = ValueOf(txtAccountNumber);
                    user.ConfirmAccountNumber = ValueOf(txtConfirmAccountNumber);
                    user.IfscCode = ValueOf(txtIfscCode);
                    user.AccountType = ValueOf(txtAccountType);
                    user.SalaryAccountFlagText = ValueOf(txtSalaryAccountFlag);
                    user.Updatedby = GetClientIdFromSession();
                    //user.password = txt_password.Text;
                    List<UserDetailsDO> userdata = userdetails.UpdateEmployeeProfileCore(user);
                    if (userdata == null || userdata.Count == 0)
                    {
                        ClientScript.RegisterStartupScript(this.GetType(), "UserSavedScript",
                                  "showUserSavedMessage('Failed', 'Update response not received from database.');", true);
                        return;
                    }

                    string status = Convert.ToString(userdata[0].Status ?? string.Empty).Trim();
                    string remark = Convert.ToString(userdata[0].Remarks ?? string.Empty).Trim();
                    if (string.IsNullOrWhiteSpace(remark))
                    {
                        remark = string.Equals(status, "Success", StringComparison.OrdinalIgnoreCase)
                            ? "User updated successfully."
                            : "User update failed.";
                    }

                    if (string.Equals(status, "Success", StringComparison.OrdinalIgnoreCase))
                    {
                        EmployeeOnboardingResponseDO assetResponse = PersistAssetDraft(user.UserId);
                        if (assetResponse == null || !string.Equals(assetResponse.Status, "Success", StringComparison.OrdinalIgnoreCase))
                        {
                            string assetMessage = assetResponse == null || string.IsNullOrWhiteSpace(assetResponse.Message)
                                ? "Asset save failed during final update."
                                : assetResponse.Message;
                            ClientScript.RegisterStartupScript(this.GetType(), "UserSavedScript",
                                      "showUserSavedMessage('Failed', '" + assetMessage.Replace("'", "\\'") + "');", true);
                            return;
                        }

                        try
                        {
                            SendUpdatedCredentialsIfRequired(userdetails, user);
                            OriginalOfficialMobile = user.contact_detail;
                        }
                        catch (Exception mailEx)
                        {
                            CommonBL errorlog = new CommonBL();
                            errorlog.fnStoreErrorLog("AddEmployee", "SendUpdatedCredentialsIfRequired", "Exception Message=" + mailEx.Message + " StackTrace=" + mailEx.StackTrace, UserId);
                        }

                        remark = "Employee updated successfully.";
                        ClientScript.RegisterStartupScript(this.GetType(), "UserSavedScript",
                                  "showUserSavedMessage('" + status + "', '" + remark + "');" +
                                  "setTimeout(function(){ window.location.href = '" + EmployeeListUrl + "'; }, 4000);", true);
                        return;
                    }
                    else
                    {
                        ClientScript.RegisterStartupScript(this.GetType(), "UserSavedScript", "showUserSavedMessage('" + status + "', '" + remark + "');", true);
                    }
                }
            }
            catch (Exception ex)
            {
                CommonBL errorlog = new CommonBL();
                errorlog.fnStoreErrorLog("addUser", "SubmitButtonClick", "Exception Message" + ex.Message + "Strace=" + ex.StackTrace, UserId);
            }
        }
        protected void ClearButton_Click(object sender, EventArgs e)
        {
            try
            {
                txtEmployeeCode.Text = string.Empty;
                txt_name.Text = string.Empty;
                txt_fullname.Text = string.Empty;
                ddldesign.SelectedIndex = 0;
                txt_email.Text = string.Empty;
                txt_contact.Text = string.Empty;
                txt_password.Text = string.Empty;
                OriginalOfficialEmail = string.Empty;
                OriginalOfficialMobile = string.Empty;

                ddlcompany.SelectedIndex = 0;
                ddl_reportingmanager.SelectedIndex = 0;
                ddlprobationperiod.SelectedIndex = 0;
                txtdept.Text = string.Empty;
                txtESICNo.Text = string.Empty;
                txtPFNo.Text = string.Empty;
                txtbranch.Text = string.Empty;
                txtDateOfJoining.Text = string.Empty;
                ddlexporintern.SelectedIndex = 0;

            }
            catch (Exception ex)
            {
                CommonBL errorlog = new CommonBL();
                errorlog.fnStoreErrorLog("AddUser", "ClearButton_Click", "Exception Message" + ex.Message + "StackTrace=" + ex.StackTrace, UserId);
            }
        }
        protected void btnBack_Click(object sender, EventArgs e)
        {
            Response.Redirect("~/view/modules/" + EmployeeListUrl, false);
        }

        protected void btnUpdateOfficialContact_Click(object sender, EventArgs e)
        {
            try
            {
                int employeeUserId = GetEmployeeUserIdFromQuery();
                if (employeeUserId <= 0)
                {
                    ScriptManager.RegisterStartupScript(this, GetType(), Guid.NewGuid().ToString("N"), "showUserSavedMessage('Failed', 'Employee not found for official contact update.');", true);
                    return;
                }

                string validationMessage = ValidateOfficialContactForm();
                if (!string.IsNullOrWhiteSpace(validationMessage))
                {
                    ScriptManager.RegisterStartupScript(this, GetType(), Guid.NewGuid().ToString("N"), "showUserSavedMessage('Failed', '" + HttpUtility.JavaScriptStringEncode(validationMessage) + "');", true);
                    return;
                }

                UserDetailsBL userDetailsBL = new UserDetailsBL();
                int updatedBy = GetClientIdFromSession();
                string newOfficialEmail = ValueOf(txt_email);
                string newOfficialMobile = ValueOf(txt_contact);
                bool shouldSendCredentials = HasOfficialEmailChanged(newOfficialEmail);

                List<UserDetailsDO> response = userDetailsBL.UpdateEmployeeOfficialContact(employeeUserId, newOfficialEmail, newOfficialMobile, updatedBy);
                if (response == null || response.Count == 0)
                {
                    ScriptManager.RegisterStartupScript(this, GetType(), Guid.NewGuid().ToString("N"), "showUserSavedMessage('Failed', 'Update response not received from database.');", true);
                    return;
                }

                string status = Convert.ToString(response[0].Status ?? string.Empty).Trim();
                string remark = Convert.ToString(response[0].Remarks ?? string.Empty).Trim();
                if (string.IsNullOrWhiteSpace(remark))
                {
                    remark = string.Equals(status, "Success", StringComparison.OrdinalIgnoreCase)
                        ? "Official contact updated successfully."
                        : "Official contact update failed.";
                }

                if (string.Equals(status, "Success", StringComparison.OrdinalIgnoreCase))
                {
                    OriginalOfficialEmail = newOfficialEmail;
                    OriginalOfficialMobile = newOfficialMobile;

                    if (response[0].SendCredentialsMail == 1 || shouldSendCredentials)
                    {
                        string activeOfficialEmail = userDetailsBL.GetActiveOfficialEmail(employeeUserId, newOfficialEmail);
                        if (!string.IsNullOrWhiteSpace(activeOfficialEmail))
                        {
                            userDetailsBL.SendUserCredentialsMail(activeOfficialEmail.Trim(), ValueOf(txt_name), DefaultEmployeePassword);
                        }
                    }
                }
                else if (remark.IndexOf("Email ID already exists", StringComparison.OrdinalIgnoreCase) >= 0)
                {
                    ScriptManager.RegisterStartupScript(this, GetType(), Guid.NewGuid().ToString("N"), "setOfficialContactInlineError('email', '" + HttpUtility.JavaScriptStringEncode(remark) + "');", true);
                    return;
                }
                else if (remark.IndexOf("Mobile Number already exists", StringComparison.OrdinalIgnoreCase) >= 0)
                {
                    ScriptManager.RegisterStartupScript(this, GetType(), Guid.NewGuid().ToString("N"), "setOfficialContactInlineError('mobile', '" + HttpUtility.JavaScriptStringEncode(remark) + "');", true);
                    return;
                }

                ScriptManager.RegisterStartupScript(this, GetType(), Guid.NewGuid().ToString("N"), "showUserSavedMessage('" + HttpUtility.JavaScriptStringEncode(status) + "', '" + HttpUtility.JavaScriptStringEncode(remark) + "');", true);
            }
            catch (Exception ex)
            {
                CommonBL errorlog = new CommonBL();
                errorlog.fnStoreErrorLog("AddEmployee", "btnUpdateOfficialContact_Click", "Exception Message=" + ex.Message + " StackTrace=" + ex.StackTrace, UserId);
                ScriptManager.RegisterStartupScript(this, GetType(), Guid.NewGuid().ToString("N"), "showUserSavedMessage('Failed', 'Official contact update failed due to an exception.');", true);
            }
        }

        private int GetClientIdFromSession()
        {
            int userId = 0;
            if (Session["UserId"] != null)
            {
                userId = Convert.ToInt32(Session["UserId"]);
            }
            return userId;
        }
    }
}
