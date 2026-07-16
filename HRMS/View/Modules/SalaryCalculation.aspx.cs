
using System;
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;
using DataObject;
using ProcessModel;

namespace HRMS.View.Modules
{
    public partial class SalaryCalculation : Page
    {
        protected string UserId = null;
        protected global::System.Web.UI.WebControls.TextBox txtSearch;
        protected global::System.Web.UI.WebControls.DropDownList ddlStatus;
        protected global::System.Web.UI.WebControls.Button btnFilter;
        protected global::System.Web.UI.WebControls.Button btnExport;
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

                BindSalaryCalculations();
            }
        }

        private void BindSalaryCalculations()
        {
            try
            {
                SalaryCalculationBL bl = new SalaryCalculationBL();
                var data = bl.GetSalaryCalculations();

                // Apply filters
                if (!string.IsNullOrEmpty(txtSearch.Text.Trim()))
                {
                    string searchText = txtSearch.Text.Trim().ToLower();
                    data = data.FindAll(x => 
                        (x.user_fullname != null && x.user_fullname.ToLower().Contains(searchText)) ||
                        (x.employee_code != null && x.employee_code.ToLower().Contains(searchText))
                    );
                }

                if (!string.IsNullOrEmpty(ddlStatus.SelectedValue))
                {
                    data = data.FindAll(x => x.verification_status == ddlStatus.SelectedValue);
                }

                gvSalaryCalculations.DataSource = data;
                gvSalaryCalculations.DataBind();
            }
            catch (Exception ex)
            {
                new CommonBL().fnStoreErrorLog("SalaryCalculation", "BindSalaryCalculations", ex.Message + " | StackTrace=" + ex.StackTrace, UserId);
            }
        }

        protected void btnFilter_Click(object sender, EventArgs e)
        {
            BindSalaryCalculations();
        }

        protected void ddlStatus_SelectedIndexChanged(object sender, EventArgs e)
        {
            BindSalaryCalculations();
        }

        protected void btnExport_Click(object sender, EventArgs e)
        {
            try
            {
                SalaryCalculationBL bl = new SalaryCalculationBL();
                var data = bl.GetSalaryCalculations();

                // Apply same filters as grid
                if (!string.IsNullOrEmpty(txtSearch.Text.Trim()))
                {
                    string searchText = txtSearch.Text.Trim().ToLower();
                    data = data.FindAll(x => 
                        (x.user_fullname != null && x.user_fullname.ToLower().Contains(searchText)) ||
                        (x.employee_code != null && x.employee_code.ToLower().Contains(searchText))
                    );
                }

                if (!string.IsNullOrEmpty(ddlStatus.SelectedValue))
                {
                    data = data.FindAll(x => x.verification_status == ddlStatus.SelectedValue);
                }

                // Create CSV
                StringBuilder sb = new StringBuilder();

                // Header row
                sb.AppendLine("\"SR No\",\"Employee Code\",\"Name\",\"Monthly Salary\",\"Per Day Salary\",\"Leave Deduction Days\",\"Deducted Amount\",\"Net Salary\",\"Total Working Days\",\"Present Days\",\"Absent Days\",\"Verification Status\"");

                // Data rows
                for (int i = 0; i < data.Count; i++)
                {
                    var item = data[i];
                    sb.AppendLine(
                        $"\"{i + 1}\"," +
                        $"\"{item.employee_code}\"," +
                        $"\"{item.user_fullname}\"," +
                        $"\"{item.monthly_salary.ToString("F2")}\"," +
                        $"\"{item.per_day_salary.ToString("F2")}\"," +
                        $"\"{item.leave_deduction_days}\"," +
                        $"\"{item.deducted_amount.ToString("F2")}\"," +
                        $"\"{item.deducted_monthly_salary.ToString("F2")}\"," +
                        $"\"{item.total_working_days}\"," +
                        $"\"{item.present_days}\"," +
                        $"\"{item.absent_days}\"," +
                        $"\"{item.verification_status}\""
                    );
                }

                // Download CSV
                Response.Clear();
                Response.Buffer = true;
                Response.ContentType = "application/vnd.ms-excel";
                Response.AddHeader("content-disposition", $"attachment;filename=SalaryCalculation_{DateTime.Now:yyyyMMddHHmmss}.csv");
                Response.Output.Write(sb.ToString());
                Response.Flush();
                Response.End();
            }
            catch (Exception ex)
            {
                new CommonBL().fnStoreErrorLog("SalaryCalculation", "btnExport_Click", ex.Message + " | StackTrace=" + ex.StackTrace, UserId);
            }
        }

        protected string FormatMoney(object value)
        {
            decimal d;
            decimal.TryParse(Convert.ToString(value), out d);
            return "&#8377; " + d.ToString("N2");
        }

        protected void btnView_Click(object sender, EventArgs e)
        {
            LinkButton btn = (LinkButton)sender;
            int userId = Convert.ToInt32(btn.CommandArgument);
            Response.Redirect("~/View/Modules/SalaryCalculationDetails.aspx?id=" + userId + "&mode=view", false);
        }

        protected void btnEdit_Click(object sender, EventArgs e)
        {
            LinkButton btn = (LinkButton)sender;
            int userId = Convert.ToInt32(btn.CommandArgument);
            Response.Redirect("~/View/Modules/SalaryCalculationDetails.aspx?id=" + userId + "&mode=edit", false);
        }
    }
}
