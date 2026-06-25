﻿﻿﻿﻿using DataObject;
using ProcessModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace HRMS.View.Modules
{
    public partial class Remunerationform : System.Web.UI.Page
    {
        renumarationBL obj = new renumarationBL();
        List<RemunerationComponent> allComponents;
        protected int? RemunerationId
        {
            get { return ViewState["RemunerationId"] as int?; }
            set { ViewState["RemunerationId"] = value; }
        }
        protected string Mode
        {
            get { return (ViewState["Mode"] as string) ?? "add"; }
            set { ViewState["Mode"] = value; }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                // Check query string
                if (!string.IsNullOrEmpty(Request.QueryString["id"]))
                {
                    RemunerationId = Convert.ToInt32(Request.QueryString["id"]);
                }
                if (!string.IsNullOrEmpty(Request.QueryString["mode"]))
                {
                    Mode = Request.QueryString["mode"].ToLower();
                }

                BindComponents();
                BindStatusDropdown();
                BindEmployeeCategoryDropdown();
                BindEmployees();

                if (RemunerationId.HasValue && (Mode == "view" || Mode == "edit"))
                {
                    LoadRemunerationData(RemunerationId.Value);
                    ApplyModeSettings();
                }
                else
                {
                    GenerateSalaryStructureID();
                    btnSave.Text = "Save";
                    btnSave.Visible = true;
                }
            }
        }

        private void ApplyModeSettings()
        {
            // Employee name and structure ID are always non-editable in view/edit modes
            ddlEmployee.Enabled = false;
            txtSalaryStructureID.ReadOnly = true;

            if (Mode == "view")
            {
                // In view mode, everything is non-editable
                txtEffectiveFromDate.ReadOnly = true;
                txtEffectiveToDate.ReadOnly = true;
                ddlStatus.Enabled = false;
                ddlEmployeeCategory.Enabled = false;
                txtCTCAmount.ReadOnly = true;
                txtGrossSalary.ReadOnly = true;
                txtMonthlySalary.ReadOnly = true;
                txtAnnualSalary.ReadOnly = true;
                btnSave.Visible = false;

                // Disable all repeater controls
                DisableRepeaterControls(rptEarnings);
                DisableRepeaterControls(rptDeductions);
            }
            else if (Mode == "edit")
            {
                // In edit mode, button says "Update"
                btnSave.Text = "Update";
                btnSave.Visible = true;
            }
        }

        private void DisableRepeaterControls(Repeater repeater)
        {
            foreach (RepeaterItem item in repeater.Items)
            {
                CheckBox chk = item.FindControl("chkComponent") as CheckBox;
                TextBox txt = item.FindControl("txtComponentAmount") as TextBox;
                if (chk != null) chk.Enabled = false;
                if (txt != null) txt.ReadOnly = true;
            }
        }

        private void LoadRemunerationData(int remunerationId)
        {
            renumarationDO renum = obj.GetRemunerationDetailsById(remunerationId);
            if (renum != null)
            {
                txtSalaryStructureID.Text = renum.SalaryStructureID;

                // Select employee
                if (ddlEmployee.Items.FindByValue(renum.UserId.ToString()) != null)
                {
                    ddlEmployee.SelectedValue = renum.UserId.ToString();
                }

                // Select status
                if (!string.IsNullOrEmpty(renum.Status) && ddlStatus.Items.FindByValue(renum.Status) != null)
                {
                    ddlStatus.SelectedValue = renum.Status;
                }

                // Dates
                                if (renum.EffectiveFromDate.HasValue)
                                {
                                    txtEffectiveFromDate.Text = renum.EffectiveFromDate.Value.ToString("dd-MM-yyyy");
                                }
                                if (renum.EffectiveToDate.HasValue)
                                {
                                    txtEffectiveToDate.Text = renum.EffectiveToDate.Value.ToString("dd-MM-yyyy");
                                }

                // Employee category
                if (!string.IsNullOrEmpty(renum.EmployeeCategory) && ddlEmployeeCategory.Items.FindByValue(renum.EmployeeCategory) != null)
                {
                    ddlEmployeeCategory.SelectedValue = renum.EmployeeCategory;
                }

                // Amounts
                                if (renum.CTCAmount.HasValue)
                                {
                                    txtCTCAmount.Text = renum.CTCAmount.Value.ToString("F2");
                                }

                // Load repeater data (earnings)
                LoadRepeaterData(rptEarnings, renum, "earning");

                // Load repeater data (deductions)
                LoadRepeaterData(rptDeductions, renum, "deduction");

                // Calculate the salary values based on loaded components
                CalculateGrossSalary(null, null);
            }
        }

        private void LoadRepeaterData(Repeater repeater, renumarationDO renum, string type)
        {
            foreach (RepeaterItem item in repeater.Items)
            {
                HiddenField hfName = item.FindControl("hfComponentName") as HiddenField;
                CheckBox chk = item.FindControl("chkComponent") as CheckBox;
                TextBox txt = item.FindControl("txtComponentAmount") as TextBox;

                if (hfName != null && chk != null && txt != null)
                {
                    string compName = hfName.Value.ToLower().Trim();
                    decimal? amount = null;

                    // Earnings
                    if (type == "earning")
                    {
                        if (compName.Contains("basic salary"))
                            amount = renum.BasicSalary;
                        else if (compName.Contains("hra"))
                            amount = renum.HRA;
                        else if (compName.Contains("conveyance"))
                            amount = renum.ConveyanceAllowance;
                        else if (compName.Contains("medical"))
                            amount = renum.MedicalAllowance;
                        else if (compName.Contains("special"))
                            amount = renum.SpecialAllowance;
                        else if (compName.Contains("education"))
                            amount = renum.EducationAllowance;
                        else if (compName.Contains("travel"))
                            amount = renum.TravelAllowance;
                        else if (compName.Contains("uniform"))
                            amount = renum.UniformAllowance;
                        else if (compName.Contains("telephone"))
                            amount = renum.TelephoneAllowance;
                        else if (compName.Contains("food"))
                            amount = renum.FoodAllowance;
                        else if (compName.Contains("shift"))
                            amount = renum.ShiftAllowance;
                        else if (compName.Contains("incentive"))
                            amount = renum.Incentive;
                        else if (compName.Contains("bonus"))
                            amount = renum.Bonus;
                        else if (compName.Contains("other"))
                            amount = renum.OtherAllowance;
                    }
                    // Deductions
                    else
                    {
                        if (compName.Contains("pf"))
                            amount = renum.PF;
                        else if (compName.Contains("esi"))
                            amount = renum.ESI;
                        else if (compName.Contains("professional tax"))
                            amount = renum.ProfessionalTax;
                        else if (compName.Contains("tds"))
                            amount = renum.TDS;
                        else if (compName.Contains("labour welfare"))
                            amount = renum.LabourWelfareFund;
                        else if (compName.Contains("loan"))
                            amount = renum.LoanDeduction;
                        else if (compName.Contains("advance"))
                            amount = renum.AdvanceRecovery;
                        else if (compName.Contains("other"))
                            amount = renum.OtherDeductions;
                    }

                    if (amount.HasValue && amount.Value > 0)
                    {
                        chk.Checked = true;
                        txt.Enabled = true;
                        txt.Text = amount.Value.ToString("F2");
                    }
                    else
                    {
                        chk.Checked = false;
                        txt.Enabled = false;
                        txt.Text = "0.00";
                    }
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
                errorlog.fnStoreErrorLog("Remunerationform", "BindEmployees", "Exception Message=" + ex.Message + " StackTrace=" + ex.StackTrace, Convert.ToString(Session["userId"]));
            }
        }

        private void BindStatusDropdown()
        {
            EmployeeOnboardingBL onboardingBL = new EmployeeOnboardingBL();
            List<DropDownData> statusList = onboardingBL.BindLookupData("salary status");
            ddlStatus.DataSource = statusList;
            ddlStatus.DataTextField = "Text";
            ddlStatus.DataValueField = "Id";
            ddlStatus.DataBind();
            ddlStatus.Items.Insert(0, new ListItem("-- Select Status --", ""));
        }

        private void BindEmployeeCategoryDropdown()
        {
            EmployeeOnboardingBL onboardingBL = new EmployeeOnboardingBL();
            List<DropDownData> categoryList = onboardingBL.BindLookupData("Emptype");
            ddlEmployeeCategory.DataSource = categoryList;
            ddlEmployeeCategory.DataTextField = "Text";
            ddlEmployeeCategory.DataValueField = "Id";
            ddlEmployeeCategory.DataBind();
            ddlEmployeeCategory.Items.Insert(0, new ListItem("-- Select Employee Category --", ""));
        }

        private void GenerateSalaryStructureID()
        {
            string year = DateTime.Now.Year.ToString();
            string random = new Random().Next(1000, 9999).ToString();
            txtSalaryStructureID.Text = "SS-" + year + "-" + random;
        }

        private void BindComponents()
        {
            allComponents = obj.GetRemunerationComponents();

            // Bind Earnings components
            var earnings = allComponents.Where(c => !string.IsNullOrEmpty(c.ComponentType) && c.ComponentType.ToUpper() == "EARNING").ToList();
            rptEarnings.DataSource = earnings;
            rptEarnings.DataBind();

            // Bind Deductions components
            var deductions = allComponents.Where(c => !string.IsNullOrEmpty(c.ComponentType) && c.ComponentType.ToUpper() == "DEDUCTION").ToList();
            rptDeductions.DataSource = deductions;
            rptDeductions.DataBind();
        }

        protected void chkComponent_CheckedChanged(object sender, EventArgs e)
        {
            CheckBox chk = (CheckBox)sender;
            RepeaterItem item = (RepeaterItem)chk.NamingContainer;
            TextBox txtAmount = (TextBox)item.FindControl("txtComponentAmount");

            if (chk.Checked)
            {
                txtAmount.Enabled = true;
            }
            else
            {
                txtAmount.Enabled = false;
                txtAmount.Text = "0";
                CalculateGrossSalary(sender, e);
            }
            UpdatePanel1.Update();
        }

        protected void txtMonthlySalary_TextChanged(object sender, EventArgs e)
        {
            if (decimal.TryParse(txtMonthlySalary.Text, out decimal monthly))
            {
                txtAnnualSalary.Text = (monthly * 12).ToString("F2");
            }
            UpdatePanel1.Update();
        }

        protected void txtCTCAmount_TextChanged(object sender, EventArgs e)
        {
            // Can implement any CTC related logic here
            UpdatePanel1.Update();
        }

        protected void CalculateGrossSalary(object sender, EventArgs e)
        {
            decimal totalEarnings = 0;

            // Calculate from earnings repeater
            foreach (RepeaterItem item in rptEarnings.Items)
            {
                CheckBox chk = (CheckBox)item.FindControl("chkComponent");
                TextBox txt = (TextBox)item.FindControl("txtComponentAmount");

                if (chk.Checked && decimal.TryParse(txt.Text, out decimal amount))
                {
                    totalEarnings += amount;
                }
            }

            txtGrossSalary.Text = totalEarnings.ToString("F2");

            // Calculate total deductions
            decimal totalDeductions = 0;
            foreach (RepeaterItem item in rptDeductions.Items)
            {
                CheckBox chk = (CheckBox)item.FindControl("chkComponent");
                TextBox txt = (TextBox)item.FindControl("txtComponentAmount");

                if (chk.Checked && decimal.TryParse(txt.Text, out decimal amount))
                {
                    totalDeductions += amount;
                }
            }

            // Calculate net monthly salary and annual salary
            decimal netMonthly = totalEarnings - totalDeductions;
            txtMonthlySalary.Text = netMonthly.ToString("F2");
            txtAnnualSalary.Text = (totalEarnings * 12).ToString("F2");

            UpdatePanel1.Update();
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                // Validate Employee selection
                if (string.IsNullOrEmpty(ddlEmployee.SelectedValue))
                {
                    ShowAlert("Please select an employee!");
                    return;
                }

                // Validate Effective from date is not empty
                if (string.IsNullOrEmpty(txtEffectiveFromDate.Text))
                {
                    ShowAlert("Effective from date is required!");
                    return;
                }
                
                // Parse dates
                DateTime effectiveFromDate;
                if (!DateTime.TryParseExact(txtEffectiveFromDate.Text, "d-M-yyyy", System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out effectiveFromDate))
                {
                    ShowAlert("Invalid date format! Please use dd-mm-yyyy.");
                    return;
                }

                DateTime? effectiveToDate = null;
                if (!string.IsNullOrEmpty(txtEffectiveToDate.Text) && DateTime.TryParseExact(txtEffectiveToDate.Text, "d-M-yyyy", System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out DateTime parsedToDate))
                {
                    effectiveToDate = parsedToDate;
                }

                // Parse dropdown values
                int? status = null;
                if (int.TryParse(ddlStatus.SelectedValue, out int statusVal) && statusVal > 0)
                {
                    status = statusVal;
                }

                int? employeeCategory = null;
                if (int.TryParse(ddlEmployeeCategory.SelectedValue, out int categoryVal) && categoryVal > 0)
                {
                    employeeCategory = categoryVal;
                }

                // Get Basic Salary
                decimal basicSalary = 0;
                decimal hra = 0;
                decimal conveyanceAllowance = 0;
                decimal medicalAllowance = 0;
                decimal specialAllowance = 0;
                decimal educationAllowance = 0;
                decimal travelAllowance = 0;
                decimal uniformAllowance = 0;
                decimal telephoneAllowance = 0;
                decimal foodAllowance = 0;
                decimal shiftAllowance = 0;
                decimal incentive = 0;
                decimal bonus = 0;
                decimal otherAllowance = 0;

                foreach (RepeaterItem item in rptEarnings.Items)
                {
                    HiddenField hfName = (HiddenField)item.FindControl("hfComponentName");
                    CheckBox chk = (CheckBox)item.FindControl("chkComponent");
                    TextBox txt = (TextBox)item.FindControl("txtComponentAmount");

                    if (hfName != null && chk.Checked && decimal.TryParse(txt.Text, out decimal amount))
                    {
                        string compName = hfName.Value.ToLower().Trim();
                        if (compName.Contains("basic salary"))
                            basicSalary = amount;
                        else if (compName.Contains("hra"))
                            hra = amount;
                        else if (compName.Contains("conveyance"))
                            conveyanceAllowance = amount;
                        else if (compName.Contains("medical"))
                            medicalAllowance = amount;
                        else if (compName.Contains("special"))
                            specialAllowance = amount;
                        else if (compName.Contains("education"))
                            educationAllowance = amount;
                        else if (compName.Contains("travel"))
                            travelAllowance = amount;
                        else if (compName.Contains("uniform"))
                            uniformAllowance = amount;
                        else if (compName.Contains("telephone"))
                            telephoneAllowance = amount;
                        else if (compName.Contains("food"))
                            foodAllowance = amount;
                        else if (compName.Contains("shift"))
                            shiftAllowance = amount;
                        else if (compName.Contains("incentive"))
                            incentive = amount;
                        else if (compName.Contains("bonus"))
                            bonus = amount;
                        else if (compName.Contains("other"))
                            otherAllowance = amount;
                    }
                }

                decimal pf = 0;
                decimal esi = 0;
                decimal professionalTax = 0;
                decimal tds = 0;
                decimal labourWelfareFund = 0;
                decimal loanDeduction = 0;
                decimal advanceRecovery = 0;
                decimal otherDeductions = 0;

                foreach (RepeaterItem item in rptDeductions.Items)
                {
                    HiddenField hfName = (HiddenField)item.FindControl("hfComponentName");
                    CheckBox chk = (CheckBox)item.FindControl("chkComponent");
                    TextBox txt = (TextBox)item.FindControl("txtComponentAmount");

                    if (hfName != null && chk.Checked && decimal.TryParse(txt.Text, out decimal amount))
                    {
                        string compName = hfName.Value.ToLower().Trim();
                        if (compName.Contains("pf"))
                            pf = amount;
                        else if (compName.Contains("esi"))
                            esi = amount;
                        else if (compName.Contains("professional tax"))
                            professionalTax = amount;
                        else if (compName.Contains("tds"))
                            tds = amount;
                        else if (compName.Contains("labour welfare"))
                            labourWelfareFund = amount;
                        else if (compName.Contains("loan"))
                            loanDeduction = amount;
                        else if (compName.Contains("advance"))
                            advanceRecovery = amount;
                        else if (compName.Contains("other"))
                            otherDeductions = amount;
                    }
                }

                // Validate CTC > 0
                if (string.IsNullOrEmpty(txtCTCAmount.Text) || !decimal.TryParse(txtCTCAmount.Text, out decimal ctc) || ctc <= 0)
                {
                    ShowAlert("CTC amount must be greater than zero!");
                    return;
                }

                // Validate Gross Salary
                if (string.IsNullOrEmpty(txtGrossSalary.Text) || !decimal.TryParse(txtGrossSalary.Text, out decimal gross) || gross <= 0)
                {
                    ShowAlert("Gross salary must be greater than zero!");
                    return;
                }

                // Validate Monthly & Annual Salary
                if (string.IsNullOrEmpty(txtMonthlySalary.Text) || !decimal.TryParse(txtMonthlySalary.Text, out decimal monthlySalary) || monthlySalary <= 0)
                {
                    ShowAlert("Monthly salary must be greater than zero!");
                    return;
                }

                if (string.IsNullOrEmpty(txtAnnualSalary.Text) || !decimal.TryParse(txtAnnualSalary.Text, out decimal annualSalary) || annualSalary <= 0)
                {
                    ShowAlert("Annual salary must be greater than zero!");
                    return;
                }

                // Validate Basic Salary <= Gross Salary
                if (basicSalary > gross)
                {
                    ShowAlert("Basic salary cannot exceed gross salary!");
                    return;
                }
                
                // Validate Gross Salary = Total Earnings (already calculated, just recheck)
                decimal totalEarnings = 0;
                foreach (RepeaterItem item in rptEarnings.Items)
                {
                    CheckBox chk = (CheckBox)item.FindControl("chkComponent");
                    TextBox txt = (TextBox)item.FindControl("txtComponentAmount");
                    
                    if (chk.Checked && decimal.TryParse(txt.Text, out decimal amount))
                    {
                        totalEarnings += amount;
                    }
                }
                
                if (Math.Abs(totalEarnings - gross) > 0.01m)
                {
                    ShowAlert("Gross salary must equal total earnings!");
                    return;
                }

                // Get user id from session
                int userId = 0;
                if (Session["userId"] != null && int.TryParse(Session["userId"].ToString(), out int sessionUserId))
                {
                    userId = sessionUserId;
                }

                if (RemunerationId.HasValue && Mode == "edit")
                {
                    // Update mode
                    var result = obj.UpdateRemunerationDetails(
                        RemunerationId.Value,
                        txtSalaryStructureID.Text,
                        Convert.ToInt32(ddlEmployee.SelectedValue),
                        status,
                        effectiveFromDate,
                        effectiveToDate,
                        employeeCategory,
                        ctc,
                        gross,
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
                        userId);

                    if (result.Status.Equals("Success", StringComparison.OrdinalIgnoreCase))
                    {
                        ShowSuccessAlert(result.Message);
                    }
                    else
                    {
                        ShowAlert(result.Message);
                    }
                }
                else
                {
                    // Add mode
                    var result = obj.SaveRemunerationDetails(
                        txtSalaryStructureID.Text,
                        Convert.ToInt32(ddlEmployee.SelectedValue),
                        status,
                        effectiveFromDate,
                        effectiveToDate,
                        employeeCategory,
                        ctc,
                        gross,
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
                        userId);

                    if (result.Status.Equals("Success", StringComparison.OrdinalIgnoreCase))
                    {
                        ShowSuccessAlert(result.Message);
                        // Reload page to clear all fields and reset form
                        ScriptManager.RegisterStartupScript(this, GetType(), "reloadPage", "setTimeout(function() { window.location.href = window.location.href; }, 1500);", true);
                    }
                    else
                    {
                        ShowAlert(result.Message);
                    }
                }
            }
            catch (Exception ex)
            {
                ShowAlert("Error: " + ex.Message);
            }
        }

        private void ShowAlert(string message)
        {
            string script = $"Swal.fire({{icon: 'error', title: 'Error', text: '{message.Replace("'", "\\'")}'}});";
            ScriptManager.RegisterStartupScript(this, GetType(), "alert", script, true);
        }

        protected void btnBack_Click(object sender, EventArgs e)
        {
            Response.Redirect("~/View/Modules/Remunerationdetails.aspx");
        }

        private void ShowSuccessAlert(string message)
        {
            string script = $"Swal.fire({{icon: 'success', title: 'Success', text: '{message.Replace("'", "\\'")}'}});";
            ScriptManager.RegisterStartupScript(this, GetType(), "success", script, true);
        }
    }
}
