using DataObject;
using ProcessModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;

namespace HRMS.View.Modules
{
    public partial class ExitClearance : Page
    {
        protected string UserId = null;
        private ExitClearanceBL clearanceBL = new ExitClearanceBL();

        // Control declarations
        protected DropDownList ddlEmployee;
        protected Panel employeeInfo;
        protected Panel pnlClearanceSections;
        protected Panel actionButtons;
        protected Label lblEmployeeName;
        protected Label lblEmployeeCode;
        protected Label lblEmployeeEmail;
        protected Label lblResignationDate;
        protected Label lblLastWorkingDate;
        protected Label lblResignationId;
        protected HiddenField hfEmployeeResignationId;
        protected HiddenField hfEmployeeCode;
        protected HiddenField hfUserId;
        protected HiddenField hfExitClearanceId;
        protected Button btnBack;
        protected Button btnSubmit;

        protected void Page_Load(object sender, EventArgs e)
        {
            UserId = Convert.ToString(Session["userId"]);

            // Always bind clearance sections first to ensure dynamic controls are created
            BindClearanceSections();

            if (!IsPostBack)
            {
                // Check if user_id and resignation_id are passed from HandoverProcess
                string queryUserId = Request.QueryString["user_id"];
                string queryResignationId = Request.QueryString["resignation_id"];

                if (!string.IsNullOrEmpty(queryUserId) && !string.IsNullOrEmpty(queryResignationId))
                {
                    // Hide dropdown and directly load employee details
                    ddlEmployee.Visible = false;
                    int resignationId = Convert.ToInt32(queryResignationId);
                    LoadEmployeeDetails(resignationId);
                    employeeInfo.Visible = true;
                    actionButtons.Visible = true;
                }
                else
                {
                    // Show dropdown for manual selection
                    BindEmployeeDropdown();
                }
            }
        }

        private void BindEmployeeDropdown()
        {
            List<ExitClearanceEmployeeDO> employees = clearanceBL.GetEmployeesWithResignationRequests();

            ddlEmployee.DataSource = employees;
            ddlEmployee.DataTextField = "EmployeeName";
            ddlEmployee.DataValueField = "EmployeeResignationId";
            ddlEmployee.DataBind();

            // Add default item
            ddlEmployee.Items.Insert(0, new ListItem("-- Select Employee --", "0"));
            ddlEmployee.SelectedIndex = 0;

            // Hide clearance sections until employee is selected
            pnlClearanceSections.Visible = false;
            actionButtons.Visible = false;
            employeeInfo.Visible = false;
        }

        protected void ddlEmployee_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (ddlEmployee.SelectedValue != "")
            {
                int resignationId = Convert.ToInt32(ddlEmployee.SelectedValue);
                LoadEmployeeDetails(resignationId);
                employeeInfo.Visible = true;
                actionButtons.Visible = true;
            }
            else
            {
                ClearEmployeeInfo();
                employeeInfo.Visible = false;
                actionButtons.Visible = false;
                btnSubmit.Text = "Submit Clearance";
            }
        }

        private void LoadEmployeeDetails(int resignationId)
        {
            var employeeDetails = clearanceBL.GetEmployeeDetailsByResignationId(resignationId);

            if (employeeDetails != null)
            {
                lblEmployeeName.Text = employeeDetails.EmployeeName;
                lblEmployeeCode.Text = employeeDetails.EmployeeCode;
                lblEmployeeEmail.Text = employeeDetails.EmployeeEmail;
                lblResignationDate.Text = employeeDetails.ResignationDate.ToString("yyyy-MM-dd");
                lblLastWorkingDate.Text = employeeDetails.LastWorkingDate.ToString("yyyy-MM-dd");
                //lblResignationId.Text = resignationId.ToString();
                hfEmployeeResignationId.Value = resignationId.ToString();
                hfEmployeeCode.Value = employeeDetails.EmployeeCode;
                hfUserId.Value = employeeDetails.UserId.ToString();

                // Check if clearance data already exists
                var existingClearance = clearanceBL.GetExitClearanceByResignationId(resignationId);
                if (existingClearance != null)
                {
                    hfExitClearanceId.Value = existingClearance.ExitClearanceId.ToString();
                    LoadExistingClearanceData(existingClearance);
                    btnSubmit.Text = "Update Clearance";
                }
                else
                {
                    hfExitClearanceId.Value = "0";
                    btnSubmit.Text = "Submit Clearance";
                }
            }
        }

        private void ClearEmployeeInfo()
        {
            lblEmployeeName.Text = string.Empty;
            lblEmployeeCode.Text = string.Empty;
            lblEmployeeEmail.Text = string.Empty;
            lblResignationDate.Text = string.Empty;
            lblLastWorkingDate.Text = string.Empty;
            //lblResignationId.Text = string.Empty;
            hfEmployeeResignationId.Value = string.Empty;
            hfEmployeeCode.Value = string.Empty;
            hfUserId.Value = string.Empty;
            hfExitClearanceId.Value = string.Empty;
        }

        private void LoadExistingClearanceData(ExitClearanceDataDO clearanceData)
        {
            // IT Department checkboxes
            SetCheckboxValue("chk_1", clearanceData.LaptopReturned);
            SetCheckboxValue("chk_2", clearanceData.DesktopReturned);
            SetCheckboxValue("chk_3", clearanceData.MobilePhoneReturned);
            SetCheckboxValue("chk_4", clearanceData.EmailDisabled);
            SetCheckboxValue("chk_5", clearanceData.VpnDisabled);
            SetCheckboxValue("chk_6", clearanceData.HrmsAccessRemoved);

            // IT Department remarks
            SetRemarksValue("txtRemarks_0", clearanceData.ItRemarks);

            // Administration Department checkboxes
            SetCheckboxValue("chk_7", clearanceData.IdCardReturned);
            SetCheckboxValue("chk_8", clearanceData.AccessCardReturned);
            SetCheckboxValue("chk_9", clearanceData.OfficeKeysReturned);
            SetCheckboxValue("chk_10", clearanceData.ParkingPassReturned);

            // Administration Department remarks
            SetRemarksValue("txtRemarks_1", clearanceData.AdministrationRemarks);

            // Finance Department checkboxes
            SetCheckboxValue("chk_11", clearanceData.LoanRecoveryCompleted);
            SetCheckboxValue("chk_12", clearanceData.SalaryAdvanceRecovered);
            SetCheckboxValue("chk_13", clearanceData.ExpenseClaimsProcessed);

            // Finance Department remarks
            SetRemarksValue("txtRemarks_2", clearanceData.FinanceRemarks);

            // Security Department checkboxes
            SetCheckboxValue("chk_14", clearanceData.BiometricDisabled);
            SetCheckboxValue("chk_15", clearanceData.BuildingAccessRevoked);

            // Security Department remarks
            SetRemarksValue("txtRemarks_3", clearanceData.SecurityRemarks);
        }

        private void SetCheckboxValue(string checkboxId, int value)
        {
            CheckBox chk = pnlClearanceSections.FindControl(checkboxId) as CheckBox;
            if (chk != null)
            {
                chk.Checked = (value > 0);
            }
        }

        private void SetRemarksValue(string textBoxId, string value)
        {
            TextBox txt = pnlClearanceSections.FindControl(textBoxId) as TextBox;
            if (txt != null && value != null)
            {
                txt.Text = value;
            }
        }

        private void BindClearanceSections()
        {
            List<ExitClearanceMasterDO> clearanceItems = clearanceBL.GetExitClearanceMaster();

            // Clear existing controls to avoid duplicates
            pnlClearanceSections.Controls.Clear();

            if (clearanceItems == null || clearanceItems.Count == 0)
            {
                // Add a message if no clearance items exist
                Label noDataLabel = new Label();
                noDataLabel.Text = "No clearance items configured. Please contact administrator.";
                noDataLabel.CssClass = "alert alert-warning";
                pnlClearanceSections.Controls.Add(noDataLabel);
                return;
            }

            // Group by department
            Dictionary<string, List<ExitClearanceMasterDO>> groupedItems = new Dictionary<string, List<ExitClearanceMasterDO>>();
            foreach (var item in clearanceItems)
            {
                if (!groupedItems.ContainsKey(item.DepartmentName))
                {
                    groupedItems[item.DepartmentName] = new List<ExitClearanceMasterDO>();
                }
                groupedItems[item.DepartmentName].Add(item);
            }

            // Create sections dynamically
            int sectionIndex = 0;
            foreach (var group in groupedItems)
            {
                HtmlGenericControl section = new HtmlGenericControl("div");
                section.Attributes["class"] = "clearance-section";

                // Section header
                HtmlGenericControl header = new HtmlGenericControl("div");
                header.Attributes["class"] = "clearance-section-header";
                header.InnerText = group.Key;
                section.Controls.Add(header);

                // Add checkboxes for each item
                foreach (var item in group.Value)
                {
                    HtmlGenericControl itemDiv = new HtmlGenericControl("div");
                    itemDiv.Attributes["class"] = "clearance-item";

                    CheckBox checkBox = new CheckBox();
                    checkBox.ID = "chk_" + item.ClearanceMasterId;
                    checkBox.CssClass = "clearance-checkbox";

                    Label label = new Label();
                    label.Text = item.ClearanceItem;
                    label.AssociatedControlID = checkBox.ID;

                    itemDiv.Controls.Add(checkBox);
                    itemDiv.Controls.Add(label);
                    section.Controls.Add(itemDiv);
                }

                // Add remarks textarea for each department (except Security)
                if (group.Key.ToLower() != "security")
                {
                    HtmlGenericControl remarksDiv = new HtmlGenericControl("div");
                    remarksDiv.Attributes["class"] = "clearance-remarks";

                    Label remarksLabel = new Label();
                    remarksLabel.Text = group.Key + " Remarks:";
                    remarksLabel.Attributes["style"] = "font-weight: bold; display: block; margin-bottom: 5px;";

                    TextBox remarksTextBox = new TextBox();
                    remarksTextBox.ID = "txtRemarks_" + sectionIndex;
                    remarksTextBox.TextMode = TextBoxMode.MultiLine;
                    remarksTextBox.CssClass = "form-control";
                    remarksTextBox.Rows = 3;
                    remarksTextBox.Attributes["placeholder"] = "Enter remarks for " + group.Key;

                    remarksDiv.Controls.Add(remarksLabel);
                    remarksDiv.Controls.Add(remarksTextBox);
                    section.Controls.Add(remarksDiv);
                }

                pnlClearanceSections.Controls.Add(section);
                sectionIndex++;
            }
        }

        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            try
            {
                int employeeResignationId = Convert.ToInt32(hfEmployeeResignationId.Value);
                string employeeCode = hfEmployeeCode.Value;
                int exitClearanceId = Convert.ToInt32(hfExitClearanceId.Value);
                int userId = Convert.ToInt32(UserId);

                // Collect checkbox values and remarks
                Dictionary<string, object> clearanceData = new Dictionary<string, object>();

                // IT Department
                clearanceData["laptop_returned"] = GetCheckboxValue("chk_1");
                clearanceData["desktop_returned"] = GetCheckboxValue("chk_2");
                clearanceData["mobile_phone_returned"] = GetCheckboxValue("chk_3");
                clearanceData["email_disabled"] = GetCheckboxValue("chk_4");
                clearanceData["vpn_disabled"] = GetCheckboxValue("chk_5");
                clearanceData["hrms_access_removed"] = GetCheckboxValue("chk_6");
                clearanceData["it_remarks"] = GetRemarksValue("txtRemarks_0");

                // Administration Department
                clearanceData["id_card_returned"] = GetCheckboxValue("chk_7");
                clearanceData["access_card_returned"] = GetCheckboxValue("chk_8");
                clearanceData["office_keys_returned"] = GetCheckboxValue("chk_9");
                clearanceData["parking_pass_returned"] = GetCheckboxValue("chk_10");
                clearanceData["administration_remarks"] = GetRemarksValue("txtRemarks_1");

                // Finance Department
                clearanceData["loan_recovery_completed"] = GetCheckboxValue("chk_11");
                clearanceData["salary_advance_recovered"] = GetCheckboxValue("chk_12");
                clearanceData["expense_claims_processed"] = GetCheckboxValue("chk_13");
                clearanceData["finance_remarks"] = GetRemarksValue("txtRemarks_2");

                // Security Department
                clearanceData["biometric_disabled"] = GetCheckboxValue("chk_14");
                clearanceData["building_access_revoked"] = GetCheckboxValue("chk_15");
                clearanceData["security_remarks"] = GetRemarksValue("txtRemarks_3");

                ExitClearanceResponseDO response;

                // Check if it's an update or insert
                if (exitClearanceId > 0)
                {
                    // Update existing record
                    response = clearanceBL.UpdateExitClearance(
                        exitClearanceId,
                        employeeResignationId,
                        employeeCode,
                        clearanceData,
                        userId
                    );
                }
                else
                {
                    // Insert new record
                    response = clearanceBL.InsertExitClearance(
                        employeeResignationId,
                        employeeCode,
                        clearanceData,
                        userId
                    );
                }

                if (response.Status == "Success")
                {
                    ClientScript.RegisterStartupScript(
                        GetType(),
                        "ClearanceSaved",
                        "if(window.Swal){Swal.fire({icon:'success',title:'Exit Clearance Saved Successfully',confirmButtonColor:'#2563EB'}).then(function(){window.location.href='HandoverProcess.aspx';});}else{alert('Exit Clearance Saved Successfully');window.location.href='HandoverProcess.aspx';}",
                        true);
                }
                else
                {
                    ClientScript.RegisterStartupScript(
                        GetType(),
                        "ClearanceFailed",
                        "if(window.Swal){Swal.fire({icon:'error',title:'Error',text:'" + System.Web.HttpUtility.JavaScriptStringEncode(response.Message) + "',confirmButtonColor:'#2563EB'});}else{alert('" + System.Web.HttpUtility.JavaScriptStringEncode(response.Message) + "');}",
                        true);
                }
            }
            catch (Exception ex)
            {
                CommonBL errorlog = new CommonBL();
                errorlog.fnStoreErrorLog("ExitClearance", "btnSubmit_Click", "Exception Message: " + ex.Message + " StackTrace: " + ex.StackTrace, UserId);

                ClientScript.RegisterStartupScript(
                    GetType(),
                    "ClearanceException",
                    "if(window.Swal){Swal.fire({icon:'error',title:'Error',text:'An error occurred. Please try again.',confirmButtonColor:'#2563EB'});}else{alert('An error occurred. Please try again.');}",
                    true);
            }
        }

        private int GetCheckboxValue(string checkboxId)
        {
            CheckBox chk = pnlClearanceSections.FindControl(checkboxId) as CheckBox;
            if (chk != null && chk.Checked)
            {
                // Extract the clearance_master_id from the checkbox ID (e.g., "chk_1" -> 1)
                string id = checkboxId.Replace("chk_", "");
                return Convert.ToInt32(id);
            }
            return 0;
        }

        private string GetRemarksValue(string textBoxId)
        {
            TextBox txt = pnlClearanceSections.FindControl(textBoxId) as TextBox;
            return txt != null ? txt.Text.Trim() : string.Empty;
        }

        protected void btnBack_Click(object sender, EventArgs e)
        {
            Response.Redirect("HandoverProcess.aspx");
        }
    }
}
