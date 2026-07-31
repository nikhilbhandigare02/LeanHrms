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
        private readonly renumarationBL renumBL = new renumarationBL();

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
                BindAppraisalComponents(null);

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
                    // Explicitly ensure dropdown is visible and textbox is hidden in add mode
                    ddlEmployee.Visible = true;
                    txtEmployeeName.Visible = false;
                    txtSalaryRevisionDate.Text = DateTime.Now.ToString("dd-MM-yyyy");
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

        protected void ddlEmployee_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                // Increment fields should only ever reflect a change the user
                // actually made for the currently selected employee - never a
                // leftover value from whichever employee was selected before.
                txtIncrementAmount.Text = "";
                txtIncrementPercentage.Text = "";
                txtIncrementAmountMonthly.Text = "";

                if (!string.IsNullOrWhiteSpace(ddlEmployee.SelectedValue))
                {
                    int userId = Convert.ToInt32(ddlEmployee.SelectedValue);

                    AppraisalBL appraisalBL = new AppraisalBL();
                    decimal oldCTC = appraisalBL.GetOldCTCByUserId(userId);

                    txtCTCOld.Text = oldCTC > 0 ? oldCTC.ToString() : "";

                    renumarationDO activeRemuneration = renumBL.GetActiveRemunerationByUserId(userId);
                    BindAppraisalComponents(activeRemuneration);
                }
                else
                {
                    txtCTCOld.Text = "";
                    BindAppraisalComponents(null);
                }
            }
            catch (Exception ex)
            {
                CommonBL errorlog = new CommonBL();
                errorlog.fnStoreErrorLog("AddAppraisalForm", "ddlEmployee_SelectedIndexChanged", "Exception Message=" + ex.Message + " StackTrace=" + ex.StackTrace, UserId);
            }
        }

        private renumarationDO activeRemunerationForBinding;

        private void BindAppraisalComponents(renumarationDO activeRemuneration)
        {
            activeRemunerationForBinding = activeRemuneration;

            hdnActiveRemunerationId.Value = activeRemuneration != null ? activeRemuneration.RenumerationId.ToString() : "0";
            hdnEmployeeCategory.Value = activeRemuneration != null ? activeRemuneration.EmployeeCategory : "";
            hdnRemunerationStatus.Value = activeRemuneration != null ? activeRemuneration.Status : "";

            List<RemunerationComponent> allComponents = renumBL.GetRemunerationComponents() ?? new List<RemunerationComponent>();

            rptAppraisalEarnings.DataSource = allComponents
                .Where(c => string.Equals(c.ComponentType, "EARNING", StringComparison.OrdinalIgnoreCase))
                .ToList();
            rptAppraisalEarnings.DataBind();

            rptAppraisalDeductions.DataSource = allComponents
                .Where(c => string.Equals(c.ComponentType, "DEDUCTION", StringComparison.OrdinalIgnoreCase))
                .ToList();
            rptAppraisalDeductions.DataBind();
        }

        protected void rptAppraisalEarnings_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            ApplyComponentAmount(e, isEarning: true);
        }

        protected void rptAppraisalDeductions_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            ApplyComponentAmount(e, isEarning: false);
        }

        private void ApplyComponentAmount(RepeaterItemEventArgs e, bool isEarning)
        {
            if (e.Item.ItemType != ListItemType.Item && e.Item.ItemType != ListItemType.AlternatingItem)
            {
                return;
            }

            HiddenField hfName = (HiddenField)e.Item.FindControl("hfComponentName");
            CheckBox chk = (CheckBox)e.Item.FindControl("chkComponent");
            TextBox txt = (TextBox)e.Item.FindControl("txtComponentAmount");

            if (hfName == null || chk == null || txt == null)
            {
                return;
            }

            decimal? amount = activeRemunerationForBinding != null
                ? GetComponentAmount(activeRemunerationForBinding, hfName.Value, isEarning)
                : null;

            // Never gate this on txt.Enabled: ASP.NET silently discards posted
            // values for any control rendered server-side with Enabled=false,
            // even if client JS removes the disabled attribute before submit.
            // The checked/disabled visual state is handled entirely client-side
            // (see initializeAppraisalComponentStates in the markup).
            if (amount.HasValue && amount.Value > 0)
            {
                chk.Checked = true;
                txt.Text = amount.Value.ToString("F2");
                txt.Attributes["data-original-amount"] = amount.Value.ToString(CultureInfo.InvariantCulture);
            }
            else
            {
                chk.Checked = false;
                txt.Text = string.Empty;
                txt.Attributes["data-original-amount"] = "0";
            }
        }

        // Mirrors the component-name matching used by Remunerationform.aspx.cs's
        // save flow, so the same component list maps to the same renumarationDO fields.
        private decimal? GetComponentAmount(renumarationDO renum, string componentName, bool isEarning)
        {
            string name = (componentName ?? string.Empty).ToLowerInvariant().Trim();

            if (isEarning)
            {
                if (name.Contains("basic salary")) return renum.BasicSalary;
                if (name.Contains("hra")) return renum.HRA;
                if (name.Contains("conveyance")) return renum.ConveyanceAllowance;
                if (name.Contains("medical")) return renum.MedicalAllowance;
                if (name.Contains("special")) return renum.SpecialAllowance;
                if (name.Contains("education")) return renum.EducationAllowance;
                if (name.Contains("travel")) return renum.TravelAllowance;
                if (name.Contains("uniform")) return renum.UniformAllowance;
                if (name.Contains("telephone")) return renum.TelephoneAllowance;
                if (name.Contains("food")) return renum.FoodAllowance;
                if (name.Contains("shift")) return renum.ShiftAllowance;
                if (name.Contains("incentive")) return renum.Incentive;
                if (name.Contains("bonus")) return renum.Bonus;
                if (name.Contains("other")) return renum.OtherAllowance;
            }
            else
            {
                if (name.Contains("esi")) return renum.ESI;
                if (name.Contains("provident fund") || name.Contains("pf")) return renum.PF;
                if (name.Contains("professional tax")) return renum.ProfessionalTax;
                if (name.Contains("tds")) return renum.TDS;
                if (name.Contains("labour welfare")) return renum.LabourWelfareFund;
                if (name.Contains("loan")) return renum.LoanDeduction;
                if (name.Contains("advance")) return renum.AdvanceRecovery;
                if (name.Contains("other")) return renum.OtherDeductions;
            }

            return null;
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

                    // Set Old CTC from stored procedure result
                txtCTCOld.Text = appraisal.oldCTC.ToString();

                    renumarationDO activeRemuneration = renumBL.GetActiveRemunerationByUserId(appraisal.user_id);
                    BindAppraisalComponents(activeRemuneration);
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

            DisableComponentRepeater(rptAppraisalEarnings);
            DisableComponentRepeater(rptAppraisalDeductions);

            // Hide save and cancel buttons, show only back button
            btnSave.Visible = false;
            btnCancel.Visible = false;
        }

        private void DisableComponentRepeater(Repeater repeater)
        {
            foreach (RepeaterItem item in repeater.Items)
            {
                CheckBox chk = item.FindControl("chkComponent") as CheckBox;
                TextBox txt = item.FindControl("txtComponentAmount") as TextBox;
                if (chk != null) chk.Enabled = false;
                if (txt != null) txt.ReadOnly = true;
            }
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
                // Gross/Net/CTC/Increment are always recomputed from the checked
                // salary components server-side (the readonly textboxes are just a
                // client-side mirror for display, and shouldn't be trusted as the
                // source of truth for what gets saved).
                AppraisalTotals totals = ComputeAppraisalTotals();
                txtGrossSalary.Text = totals.GrossSalary.ToString("F2");
                txtNetSalary.Text = totals.NetSalary.ToString("F2");
                txtAppraisalCTC.Text = totals.NewCtc.ToString("F2");
                txtIncrementAmount.Text = totals.IncrementAmount.ToString("F2");
                txtIncrementPercentage.Text = totals.IncrementPercentage.ToString("F2");
                txtIncrementAmountMonthly.Text = totals.IncrementAmountMonthly.ToString("F2");

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
                    appraisal_ctc = totals.NewCtc,
                    gross_salary = totals.GrossSalary,
                    net_salary = totals.NetSalary,
                    increament_amount = totals.IncrementAmount,
                    increament_percentage = totals.IncrementPercentage,
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
                    TrySupersedeRemuneration(userId, appraisal, userIdFromSession, totals);

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

        // Deactivates the employee's current active remuneration row and inserts a
        // fresh one carrying the recalculated (CTC-ratio-scaled) component amounts,
        // instead of Sp_save_appraisal_details' own in-place gross_salary/ctc_amount
        // patch (which leaves the individual components stale).
        private void TrySupersedeRemuneration(int userId, AppraisalDetailsDO appraisal, int insertedBy, AppraisalTotals totals)
        {
            try
            {
                int oldRemunerationId;
                int.TryParse(hdnActiveRemunerationId.Value, out oldRemunerationId);

                ReadEarningsComponents(
                    out decimal basicSalary, out decimal hra, out decimal conveyanceAllowance,
                    out decimal medicalAllowance, out decimal specialAllowance, out decimal educationAllowance,
                    out decimal travelAllowance, out decimal uniformAllowance, out decimal telephoneAllowance,
                    out decimal foodAllowance, out decimal shiftAllowance, out decimal incentive,
                    out decimal bonus, out decimal otherAllowance);

                ReadDeductionComponents(
                    out decimal pf, out decimal esi, out decimal professionalTax, out decimal tds,
                    out decimal labourWelfareFund, out decimal loanDeduction, out decimal advanceRecovery,
                    out decimal otherDeductions);

                decimal grossSalary = totals.GrossSalary;
                decimal monthlySalary = totals.NetSalary;
                decimal annualSalary = totals.NewCtc;

                int? employeeCategory = null;
                if (int.TryParse(hdnEmployeeCategory.Value, out int categoryVal) && categoryVal > 0)
                {
                    employeeCategory = categoryVal;
                }

                int? status = null;
                if (int.TryParse(hdnRemunerationStatus.Value, out int statusVal) && statusVal > 0)
                {
                    status = statusVal;
                }

                // Deactivate first: SP_SaveRemunerationDetails refuses to insert
                // while an is_active=1 row already exists for this user.
                if (oldRemunerationId > 0)
                {
                    renumBL.DeactivateRemuneration(oldRemunerationId);
                }

                string salaryStructureId = "SS-" + DateTime.Now.Year + "-" + new Random().Next(1000, 9999);

                var result = renumBL.SaveRemunerationDetails(
                    salaryStructureId,
                    userId,
                    status,
                    appraisal.appraisal_effective_date,
                    null,
                    employeeCategory,
                    grossSalary,
                    monthlySalary,
                    annualSalary,
                    basicSalary,
                    hra,
                    conveyanceAllowance,
                    medicalAllowance,
                    specialAllowance,
                    educationAllowance,
                    travelAllowance,
                    uniformAllowance,
                    telephoneAllowance,
                    foodAllowance,
                    shiftAllowance,
                    incentive,
                    bonus,
                    otherAllowance,
                    pf,
                    esi,
                    professionalTax,
                    tds,
                    labourWelfareFund,
                    loanDeduction,
                    advanceRecovery,
                    otherDeductions,
                    insertedBy);

                if (!string.Equals(result.Status, "Success", StringComparison.OrdinalIgnoreCase))
                {
                    CommonBL errorlog = new CommonBL();
                    errorlog.fnStoreErrorLog("AddAppraisalForm", "TrySupersedeRemuneration",
                        "Remuneration insert did not succeed after appraisal save. Message: " + result.Message, UserId);
                }
            }
            catch (Exception ex)
            {
                CommonBL errorlog = new CommonBL();
                errorlog.fnStoreErrorLog("AddAppraisalForm", "TrySupersedeRemuneration", "Exception Message=" + ex.Message + " StackTrace=" + ex.StackTrace, UserId);
            }
        }

        private void ReadEarningsComponents(
            out decimal basicSalary, out decimal hra, out decimal conveyanceAllowance,
            out decimal medicalAllowance, out decimal specialAllowance, out decimal educationAllowance,
            out decimal travelAllowance, out decimal uniformAllowance, out decimal telephoneAllowance,
            out decimal foodAllowance, out decimal shiftAllowance, out decimal incentive,
            out decimal bonus, out decimal otherAllowance)
        {
            basicSalary = hra = conveyanceAllowance = medicalAllowance = specialAllowance = educationAllowance =
                travelAllowance = uniformAllowance = telephoneAllowance = foodAllowance = shiftAllowance =
                incentive = bonus = otherAllowance = 0;

            foreach (RepeaterItem item in rptAppraisalEarnings.Items)
            {
                HiddenField hfName = item.FindControl("hfComponentName") as HiddenField;
                CheckBox chk = item.FindControl("chkComponent") as CheckBox;
                TextBox txt = item.FindControl("txtComponentAmount") as TextBox;

                if (hfName == null || chk == null || txt == null || !chk.Checked || !decimal.TryParse(txt.Text, out decimal amount))
                {
                    continue;
                }

                string name = hfName.Value.ToLowerInvariant().Trim();
                if (name.Contains("basic salary")) basicSalary += amount;
                else if (name.Contains("hra")) hra += amount;
                else if (name.Contains("conveyance")) conveyanceAllowance += amount;
                else if (name.Contains("medical")) medicalAllowance += amount;
                else if (name.Contains("special")) specialAllowance += amount;
                else if (name.Contains("education")) educationAllowance += amount;
                else if (name.Contains("travel")) travelAllowance += amount;
                else if (name.Contains("uniform")) uniformAllowance += amount;
                else if (name.Contains("telephone")) telephoneAllowance += amount;
                else if (name.Contains("food")) foodAllowance += amount;
                else if (name.Contains("shift")) shiftAllowance += amount;
                else if (name.Contains("incentive")) incentive += amount;
                else if (name.Contains("bonus")) bonus += amount;
                // "Network Allowance" / "Over time" have no dedicated column in
                // remuneration_details, so they fall into the general-purpose bucket.
                else if (name.Contains("other") || name.Contains("network") || name.Contains("over time") || name.Contains("overtime"))
                    otherAllowance += amount;
            }
        }

        private void ReadDeductionComponents(
            out decimal pf, out decimal esi, out decimal professionalTax, out decimal tds,
            out decimal labourWelfareFund, out decimal loanDeduction, out decimal advanceRecovery,
            out decimal otherDeductions)
        {
            pf = esi = professionalTax = tds = labourWelfareFund = loanDeduction = advanceRecovery = otherDeductions = 0;

            foreach (RepeaterItem item in rptAppraisalDeductions.Items)
            {
                HiddenField hfName = item.FindControl("hfComponentName") as HiddenField;
                CheckBox chk = item.FindControl("chkComponent") as CheckBox;
                TextBox txt = item.FindControl("txtComponentAmount") as TextBox;

                if (hfName == null || chk == null || txt == null || !chk.Checked || !decimal.TryParse(txt.Text, out decimal amount))
                {
                    continue;
                }

                string name = hfName.Value.ToLowerInvariant().Trim();
                if (name.Contains("esi")) esi += amount;
                else if (name.Contains("provident fund") || name.Contains("pf")) pf += amount;
                else if (name.Contains("professional tax")) professionalTax += amount;
                else if (name.Contains("tds")) tds += amount;
                else if (name.Contains("labour welfare")) labourWelfareFund += amount;
                else if (name.Contains("loan")) loanDeduction += amount;
                else if (name.Contains("advance")) advanceRecovery += amount;
                else if (name.Contains("other")) otherDeductions += amount;
            }
        }

        private struct AppraisalTotals
        {
            public decimal GrossSalary;
            public decimal NetSalary;
            public decimal NewCtc;
            public decimal IncrementAmount;
            public decimal IncrementPercentage;
            public decimal IncrementAmountMonthly;
        }

        // Single source of truth for Gross/Net/CTC/Increment — always derived
        // from the checked salary components, never from the readonly display
        // textboxes (which only mirror client-side JS and can't be trusted).
        private AppraisalTotals ComputeAppraisalTotals()
        {
            ReadEarningsComponents(
                out decimal basicSalary, out decimal hra, out decimal conveyanceAllowance,
                out decimal medicalAllowance, out decimal specialAllowance, out decimal educationAllowance,
                out decimal travelAllowance, out decimal uniformAllowance, out decimal telephoneAllowance,
                out decimal foodAllowance, out decimal shiftAllowance, out decimal incentive,
                out decimal bonus, out decimal otherAllowance);

            decimal totalEarnings = basicSalary + hra + conveyanceAllowance + medicalAllowance + specialAllowance +
                educationAllowance + travelAllowance + uniformAllowance + telephoneAllowance + foodAllowance +
                shiftAllowance + incentive + bonus + otherAllowance;

            ReadDeductionComponents(
                out decimal pf, out decimal esi, out decimal professionalTax, out decimal tds,
                out decimal labourWelfareFund, out decimal loanDeduction, out decimal advanceRecovery,
                out decimal otherDeductions);

            decimal totalDeductions = pf + esi + professionalTax + tds + labourWelfareFund + loanDeduction + advanceRecovery + otherDeductions;

            decimal grossSalary = totalEarnings;
            decimal netSalary = totalEarnings - totalDeductions;
            decimal newCtc = totalEarnings * 12;

            decimal oldCtc = ParseDecimalValue(txtCTCOld.Text);
            decimal incrementAmount = newCtc - oldCtc;
            decimal incrementPercentage = oldCtc > 0 ? (incrementAmount / oldCtc) * 100 : 0;

            return new AppraisalTotals
            {
                GrossSalary = grossSalary,
                NetSalary = netSalary,
                NewCtc = newCtc,
                IncrementAmount = incrementAmount,
                IncrementPercentage = incrementPercentage,
                IncrementAmountMonthly = incrementAmount / 12
            };
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

            if (string.IsNullOrWhiteSpace(txtSalaryRevisionDate.Text))
                return "Please enter salary revision date.";

            if (!IsValidDate(txtSalaryRevisionDate.Text))
                return "Please enter valid salary revision date in dd-MM-yyyy format.";

            // Gross/Net/CTC/Increment are all computed server-side from the
            // checked salary components (see ComputeAppraisalTotals), so at
            // least one earning component must be checked with an amount.
            if (!HasAnyCheckedEarningComponent())
                return "Please select at least one earning component with an amount.";

            return string.Empty;
        }

        private bool HasAnyCheckedEarningComponent()
        {
            foreach (RepeaterItem item in rptAppraisalEarnings.Items)
            {
                CheckBox chk = item.FindControl("chkComponent") as CheckBox;
                TextBox txt = item.FindControl("txtComponentAmount") as TextBox;

                if (chk != null && chk.Checked && txt != null && decimal.TryParse(txt.Text, out decimal amount) && amount > 0)
                {
                    return true;
                }
            }

            return false;
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

