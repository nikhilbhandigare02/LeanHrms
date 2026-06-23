using DataObject;
using ProcessModel;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace HRMS.View.Modules
{
    public partial class AddAppraisalForm : System.Web.UI.Page
    {
        protected string UserId = null;

        protected void Page_Load(object sender, EventArgs e)
        {
            UserId = Convert.ToString(Session["userId"]);

            if (!IsPostBack)
            {
                if (Session["userId"] == null)
                {
                    Response.Redirect("~/view/authentication/login.aspx", false);
                    return;
                }

                BindEmployees();

                // Check mode
                string mode = Convert.ToString(Session["Appraisal_Mode"]);
                bool isViewMode = string.Equals(mode, "View", StringComparison.OrdinalIgnoreCase);
                bool isEditMode = string.Equals(mode, "Edit", StringComparison.OrdinalIgnoreCase);

                // Check if editing or viewing existing appraisal
                if (Session["Appraisal_ID"] != null)
                {
                    int appraisalId = Convert.ToInt32(Session["Appraisal_ID"]);
                    hdnAppraisalId.Value = appraisalId.ToString();
                    
                    if (isViewMode)
                    {
                        hdnIsView.Value = "1";
                        lblPageTitle.Text = "View Appraisal";
                    }
                    else if (isEditMode)
                    {
                        hdnIsEdit.Value = "1";
                        lblPageTitle.Text = "Edit Appraisal";
                        btnSave.Text = "Update Appraisal";
                    }
                    else
                    {
                        hdnIsEdit.Value = "1";
                        lblPageTitle.Text = "Edit Appraisal";
                        btnSave.Text = "Update Appraisal";
                    }
                    
                    LoadAppraisalDetails(appraisalId);
                    
                    if (isViewMode)
                    {
                        // Make fields read-only in view mode
                        MakeFieldsReadOnly();
                    }
                    else if (isEditMode)
                    {
                        // In edit mode: make employee name uneditable, show textbox instead of dropdown
                        MakeEmployeeNameUneditable();
                    }
                }
                else
                {
                    hdnAppraisalId.Value = "0";
                    hdnIsEdit.Value = "0";
                    hdnIsView.Value = "0";
                    lblPageTitle.Text = "Add New Appraisal";
                    btnSave.Text = "Save Appraisal";
                }
            }
        }

        private void BindEmployees()
        {
            try
            {
                UserDetailsBL userBL = new UserDetailsBL();
                List<UserDetailsDO> employees = userBL.ViewAllUsers();

                if (employees != null && employees.Count > 0)
                {
                    ddlEmployee.DataSource = employees.OrderBy(x => x.user_fullname);
                    ddlEmployee.DataTextField = "user_fullname";
                    ddlEmployee.DataValueField = "UserId";
                    ddlEmployee.DataBind();
                }

                ddlEmployee.Items.Insert(0, new ListItem("-- Select Employee --", ""));
            }
            catch (Exception ex)
            {
                CommonBL errorlog = new CommonBL();
                errorlog.fnStoreErrorLog("AddAppraisalForm", "BindEmployees", "Exception Message=" + ex.Message + " StackTrace=" + ex.StackTrace, UserId);
            }
        }

        private void LoadAppraisalDetails(int appraisalId)
        {
            try
            {
                AppraisalBL appraisalBL = new AppraisalBL();
                AppraisalDetailsDO appraisal = appraisalBL.GetAppraisalDetailsById(appraisalId);

                if (appraisal != null)
                {
                    // Check mode
                    bool isViewMode = hdnIsView.Value == "1";
                    bool isEditMode = hdnIsEdit.Value == "1";
                    
                    if (isViewMode || isEditMode)
                    {
                        // In view or edit mode: show textbox, hide dropdown, populate with employee name
                        ddlEmployee.Visible = false;
                        txtEmployeeName.Visible = true;
                        txtEmployeeName.Text = appraisal.employee_name;
                    }
                    else
                    {
                        // In add mode: show dropdown, hide textbox, select employee
                        ddlEmployee.Visible = true;
                        txtEmployeeName.Visible = false;
                        
                        // Try to select by user_id first
                        if (appraisal.user_id > 0)
                        {
                            ListItem userItem = ddlEmployee.Items.FindByValue(appraisal.user_id.ToString());
                            if (userItem != null)
                            {
                                ddlEmployee.SelectedValue = appraisal.user_id.ToString();
                            }
                        }
                        
                        // If user_id didn't work, try by employee name
                        if (ddlEmployee.SelectedIndex == 0 && !string.IsNullOrWhiteSpace(appraisal.employee_name))
                        {
                            ListItem nameItem = ddlEmployee.Items.FindByText(appraisal.employee_name);
                            if (nameItem != null)
                            {
                                nameItem.Selected = true;
                            }
                        }
                    }
                    
                    txtEffectiveDate.Text = appraisal.appraisal_effective_date.ToString("dd-MM-yyyy");
                    txtAppraisalCTC.Text = appraisal.appraisal_ctc.ToString();
                    txtGrossSalary.Text = appraisal.gross_salary.ToString();
                    txtNetSalary.Text = appraisal.net_salary.ToString();
                    txtSalaryRevisionDate.Text = appraisal.salary_revision_date.ToString("dd-MM-yyyy");
                    txtIncrementPercentage.Text = appraisal.increament_percentage.ToString();
                    txtIncrementAmount.Text = appraisal.increament_amount.ToString();
                    
                    // Store user_id in hidden field for save operation
                    hdnUserId.Value = appraisal.user_id.ToString();
                    
                    // Calculate and set Old CTC: New CTC - Increment Amount
                    decimal oldCtc = appraisal.appraisal_ctc - appraisal.increament_amount;
                    txtCTCOld.Text = oldCtc.ToString();
                }
            }
            catch (Exception ex)
            {
                CommonBL errorlog = new CommonBL();
                errorlog.fnStoreErrorLog("AddAppraisalForm", "LoadAppraisalDetails", "Exception Message=" + ex.Message + " StackTrace=" + ex.StackTrace, UserId);
            }
        }

        private void MakeFieldsReadOnly()
        {
            // In view mode, we already hide the dropdown and show textbox
            // Just make sure all text fields are read-only and buttons are hidden
            txtEffectiveDate.ReadOnly = true;
            txtCTCOld.ReadOnly = true;
            txtAppraisalCTC.ReadOnly = true;
            txtGrossSalary.ReadOnly = true;
            txtNetSalary.ReadOnly = true;
            txtSalaryRevisionDate.ReadOnly = true;
            txtIncrementAmount.ReadOnly = true;
            txtIncrementPercentage.ReadOnly = true;
            txtEmployeeName.ReadOnly = true;
            
            // Hide save and cancel buttons, show only back button
            btnSave.Visible = false;
            btnCancel.Visible = false;
        }

        private void MakeEmployeeNameUneditable()
        {
            // In edit mode: employee name is already shown as textbox (from LoadAppraisalDetails)
            // Just make sure it's read-only, and keep other fields editable
            txtEmployeeName.ReadOnly = true;
            
            // Keep save and cancel buttons visible
            btnSave.Visible = true;
            btnCancel.Visible = true;
        }

        protected void btnBack_Click(object sender, EventArgs e)
        {
            Session["Appraisal_ID"] = null;
            Session["Appraisal_Mode"] = null;
            Response.Redirect("AppraisalDetails.aspx");
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                // Validation
                string validationMessage = ValidateForm();
                if (!string.IsNullOrWhiteSpace(validationMessage))
                {
                    ScriptManager.RegisterStartupScript(this, GetType(), "validationError",
                        $"Swal.fire('Validation Error', '{HttpUtility.JavaScriptStringEncode(validationMessage)}', 'error');", true);
                    return;
                }

                int userIdFromSession = 0;
                int.TryParse(Convert.ToString(Session["UserID"] ?? Session["userId"]), out userIdFromSession);

                // Determine which user_id to use
                int userId;
                if (hdnIsEdit.Value == "1" && !string.IsNullOrWhiteSpace(hdnUserId.Value))
                {
                    // In edit mode, use the stored user_id
                    userId = Convert.ToInt32(hdnUserId.Value);
                }
                else
                {
                    // In add mode, use the selected employee from dropdown
                    userId = Convert.ToInt32(ddlEmployee.SelectedValue);
                }

                AppraisalDetailsDO appraisal = new AppraisalDetailsDO
                {
                    appraisal_id = Convert.ToInt32(hdnAppraisalId.Value),
                    user_id = userId,
                    appraisal_effective_date = ParseDate(txtEffectiveDate.Text.Trim()),
                    salary_revision_date = ParseDate(txtSalaryRevisionDate.Text.Trim()),
                    appraisal_ctc = ParseDecimalValue(txtAppraisalCTC.Text),
                    gross_salary = ParseDecimalValue(txtGrossSalary.Text),
                    net_salary = ParseDecimalValue(txtNetSalary.Text),
                    increament_amount = ParseDecimalValue(txtIncrementAmount.Text),
                    increament_percentage = ParseDecimalValue(txtIncrementPercentage.Text),
                    created_by = userIdFromSession
                };

                AppraisalBL appraisalBL = new AppraisalBL();
                ResponseDO response;

                if (hdnIsEdit.Value == "1")
                {
                    response = appraisalBL.UpdateAppraisalDetails(appraisal);
                }
                else
                {
                    response = appraisalBL.SaveAppraisalDetails(appraisal);
                }

                if (response != null && response.Status == 1)
                {
                    Session["Appraisal_ID"] = null;
                    Session["Appraisal_Mode"] = null;
                    ScriptManager.RegisterStartupScript(this, GetType(), "saveSuccess",
                        $"Swal.fire('Success', '{HttpUtility.JavaScriptStringEncode(response.message)}', 'success').then(function() {{ window.location.href='AppraisalDetails.aspx'; }});", true);
                }
                else
                {
                    ScriptManager.RegisterStartupScript(this, GetType(), "saveFailed",
                        $"Swal.fire('Warning', '{HttpUtility.JavaScriptStringEncode(response.message)}', 'warning');", true);
                }
            }
            catch (Exception ex)
            {
                CommonBL errorlog = new CommonBL();
                errorlog.fnStoreErrorLog("AddAppraisalForm", "btnSave_Click", "Exception Message=" + ex.Message + " StackTrace=" + ex.StackTrace, UserId);
                ScriptManager.RegisterStartupScript(this, GetType(), "saveException",
                    $"Swal.fire('Error', '{HttpUtility.JavaScriptStringEncode(ex.Message)}', 'error');", true);
            }
        }

        private DateTime ParseDate(string dateString)
        {
            if (string.IsNullOrWhiteSpace(dateString))
                return DateTime.MinValue;

            DateTime parsedDate;
            if (DateTime.TryParseExact(dateString, "dd-MM-yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out parsedDate))
            {
                return parsedDate;
            }
            return DateTime.MinValue;
        }

        private string ValidateForm()
        {
            // Skip employee validation in edit mode since employee name is uneditable
            if (hdnIsEdit.Value != "1" && string.IsNullOrWhiteSpace(ddlEmployee.SelectedValue))
                return "Please select an employee.";

            if (string.IsNullOrWhiteSpace(txtEffectiveDate.Text))
                return "Please enter effective date.";

            if (!IsValidDate(txtEffectiveDate.Text))
                return "Please enter valid effective date in dd-MM-yyyy format.";

            if (string.IsNullOrWhiteSpace(txtAppraisalCTC.Text))
                return "Please enter appraisal CTC.";

            if (string.IsNullOrWhiteSpace(txtGrossSalary.Text))
                return "Please enter gross salary.";

            if (string.IsNullOrWhiteSpace(txtNetSalary.Text))
                return "Please enter net salary.";

            if (string.IsNullOrWhiteSpace(txtSalaryRevisionDate.Text))
                return "Please enter salary revision date.";

            if (!IsValidDate(txtSalaryRevisionDate.Text))
                return "Please enter valid salary revision date in dd-MM-yyyy format.";

            if (string.IsNullOrWhiteSpace(txtIncrementPercentage.Text))
                return "Please enter increment percentage.";

            return string.Empty;
        }

        private bool IsValidDate(string dateString)
        {
            DateTime parsedDate;
            return DateTime.TryParseExact(dateString, "dd-MM-yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out parsedDate);
        }

        private decimal ParseDecimalValue(string value)
        {
            decimal result;
            decimal.TryParse(value, out result);
            return result;
        }
    }
}

