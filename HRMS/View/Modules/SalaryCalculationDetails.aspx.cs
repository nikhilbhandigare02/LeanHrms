using System;
using System.Web.UI;
using DataObject;
using ProcessModel;

namespace HRMS.View.Modules
{
    public partial class SalaryCalculationDetails : System.Web.UI.Page
    {
        protected string UserId = null;
        protected int? UserIdParam
        {
            get { return ViewState["UserIdParam"] as int?; }
            set { ViewState["UserIdParam"] = value; }
        }
        protected string Mode
        {
            get { return (ViewState["Mode"] as string) ?? "view"; }
            set { ViewState["Mode"] = value; }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            UserId = Convert.ToString(Session["userId"]);
            if (Session["userId"] == null)
            {
                Response.Redirect("~/view/authentication/login.aspx", false);
                return;
            }

            if (!IsPostBack)
            {
                if (!string.IsNullOrEmpty(Request.QueryString["id"]))
                {
                    UserIdParam = Convert.ToInt32(Request.QueryString["id"]);
                }
                if (!string.IsNullOrEmpty(Request.QueryString["mode"]))
                {
                    Mode = Request.QueryString["mode"].ToLower();
                }

                if (UserIdParam.HasValue)
                {
                    LoadSalaryCalculationData(UserIdParam.Value);
                }

                // Show/hide buttons based on mode
                if (Mode == "edit")
                {
                    btnVerifyUpdate.Visible = true;
                }
                else
                {
                    // In view mode, make all fields read-only
                    txtLeaveDeductionDays.ReadOnly = true;
                    txtOtherDeduction.ReadOnly = true;
                    txtDeductedAmount.ReadOnly = true;
                    txtNetSalary.ReadOnly = true;
                }
            }
        }

        private void LoadSalaryCalculationData(int userId)
        {
            SalaryCalculationBL bl = new SalaryCalculationBL();
            SalaryCalculationDO data = bl.GetSalaryCalculationByUserId(userId);
            if (data != null)
            {
                // Store original values in ViewState for calculations and saving
                ViewState["OriginalMonthlySalary"] = data.monthly_salary;
                ViewState["OriginalPerDaySalary"] = data.per_day_salary;
                ViewState["EmployeeCode"] = data.employee_code;
                ViewState["EmployeeName"] = data.user_fullname;
                ViewState["PresentDays"] = data.present_days;
                ViewState["AbsentDays"] = data.absent_days;
                ViewState["UserId"] = data.user_id;

                txtEmployeeCode.Text = data.employee_code;
                txtEmployeeName.Text = data.user_fullname;
                txtEmail.Text = data.user_mail_id;
                txtVerificationStatus.Text = data.verification_status;
                txtMonthlySalary.Text = "₹ " + data.monthly_salary.ToString("N2");
                txtPerDaySalary.Text = "₹ " + data.per_day_salary.ToString("N2");
                txtTotalWorkingDays.Text = data.total_working_days.ToString();
                txtPresentDays.Text = data.present_days.ToString();
                txtAbsentDays.Text = data.absent_days.ToString();
                txtLeaveDeductionDays.Text = data.leave_deduction_days.ToString();
                txtOtherDeduction.Text = data.other_deduction.ToString("0.##");
                txtDeductedAmount.Text = "₹ " + data.deducted_amount.ToString("N2");
                txtNetSalary.Text = "₹ " + data.deducted_monthly_salary.ToString("N2");
            }
        }

        protected void btnBack_Click(object sender, EventArgs e)
        {
            Response.Redirect("~/View/Modules/SalaryCalculation.aspx", false);
        }

        protected void btnVerifyUpdate_Click(object sender, EventArgs e)
        {
            try
            {
                // Get values from ViewState and controls
                string empCode = ViewState["EmployeeCode"]?.ToString();
                string username = ViewState["EmployeeName"]?.ToString();
                int daysPresent = Convert.ToInt32(ViewState["PresentDays"]);
                int daysAbsent = Convert.ToInt32(ViewState["AbsentDays"]);
                decimal basicSalary = Convert.ToDecimal(ViewState["OriginalMonthlySalary"]);
                int userId = Convert.ToInt32(ViewState["UserId"]);
                
                // Get logged in user id
                int insertedBy = 0;
                if (Session["userId"] != null)
                {
                    insertedBy = Convert.ToInt32(Session["userId"]);
                }

                // Get current leave deduction days
                int totalDeductionDays = int.TryParse(txtLeaveDeductionDays.Text, out int parsedDeductionDays) ? parsedDeductionDays : 0;

                // Get current other deduction amount
                decimal otherDeduction = decimal.TryParse(txtOtherDeduction.Text, out decimal parsedOtherDeduction) && parsedOtherDeduction >= 0 ? parsedOtherDeduction : 0;

                // Get current deducted amount (strip ₹ sign and parse) - combined total (leave + other)
                string deductedAmountText = txtDeductedAmount.Text.Replace("₹", "").Trim();
                decimal totalDeduction = decimal.TryParse(deductedAmountText, out decimal parsedDeductedAmount) ? parsedDeductedAmount : 0;

                // Get current net salary (strip ₹ sign and parse)
                string netSalaryText = txtNetSalary.Text.Replace("₹", "").Trim();
                decimal netSalary = decimal.TryParse(netSalaryText, out decimal parsedNetSalary) ? parsedNetSalary : 0;

                // Call the BL method to save salary slip
                SalaryCalculationBL bl = new SalaryCalculationBL();
                var result = bl.SaveSalarySlip(empCode, username, daysPresent, daysAbsent, basicSalary, totalDeduction, totalDeductionDays, netSalary, userId, insertedBy, otherDeduction);

                if (result.Status == 1)
                {
                    ShowSuccessAlertAndRedirect(result.Message);
                }
                else
                {
                    ShowAlert(result.Message);
                }
            }
            catch (Exception ex)
            {
                ShowAlert("Error: " + ex.Message);
            }
        }

        private void ShowSuccessAlertAndRedirect(string message)
        {
            string redirectUrl = ResolveUrl("~/View/Modules/SalaryCalculation.aspx");
            string script = $"Swal.fire({{icon: 'success', title: 'Success', text: '{message.Replace("'", "\\'")}'}}).then((result) => {{ window.location.href = '{redirectUrl}'; }});";
            ScriptManager.RegisterStartupScript(this, GetType(), "successRedirect", script, true);
        }

        protected void txtLeaveDeductionDays_TextChanged(object sender, EventArgs e)
        {
            RecalculateDeductionAndNetSalary();
        }

        protected void txtOtherDeduction_TextChanged(object sender, EventArgs e)
        {
            RecalculateDeductionAndNetSalary();
        }

        private void RecalculateDeductionAndNetSalary()
        {
            // Get original values from ViewState
            if (ViewState["OriginalMonthlySalary"] == null || ViewState["OriginalPerDaySalary"] == null)
            {
                return;
            }

            decimal originalMonthlySalary = Convert.ToDecimal(ViewState["OriginalMonthlySalary"]);
            decimal perDaySalary = Convert.ToDecimal(ViewState["OriginalPerDaySalary"]);

            int leaveDays = int.TryParse(txtLeaveDeductionDays.Text, out int parsedLeaveDays) && parsedLeaveDays >= 0 ? parsedLeaveDays : 0;
            decimal otherDeduction = decimal.TryParse(txtOtherDeduction.Text, out decimal parsedOtherDeduction) && parsedOtherDeduction >= 0 ? parsedOtherDeduction : 0;

            // Calculate new deducted amount (leave deduction + other deduction) and net salary
            decimal leaveDeductionAmount = Math.Round(leaveDays * perDaySalary, 2);
            decimal deductedAmount = Math.Round(leaveDeductionAmount + otherDeduction, 2);
            decimal netSalary = Math.Round(originalMonthlySalary - deductedAmount, 2);

            // Update the textboxes
            txtDeductedAmount.Text = "₹ " + deductedAmount.ToString("N2");
            txtNetSalary.Text = "₹ " + netSalary.ToString("N2");
        }

        private void ShowAlert(string message)
        {
            string script = $"Swal.fire({{icon: 'error', title: 'Error', text: '{message.Replace("'", "\\'")}'}});";
            ScriptManager.RegisterStartupScript(this, GetType(), "alert", script, true);
        }

        private void ShowSuccessAlert(string message)
        {
            string script = $"Swal.fire({{icon: 'success', title: 'Success', text: '{message.Replace("'", "\\'")}'}});";
            ScriptManager.RegisterStartupScript(this, GetType(), "success", script, true);
        }
    }
}
