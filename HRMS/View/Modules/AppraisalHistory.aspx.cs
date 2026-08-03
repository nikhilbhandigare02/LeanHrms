using DataObject;
using ProcessModel;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI.WebControls;

namespace HRMS.View.Modules
{
    public partial class AppraisalHistory : System.Web.UI.Page
    {
        protected string UserId = null;
        private readonly AppraisalBL appraisalBL = new AppraisalBL();

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
                BindEmployees();
                BindHistory();
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
                    ddlEmployeeFilter.DataSource = employees.OrderBy(x => x.user_fullname);
                    ddlEmployeeFilter.DataTextField = "user_fullname";
                    ddlEmployeeFilter.DataValueField = "UserId";
                    ddlEmployeeFilter.DataBind();
                }

                ddlEmployeeFilter.Items.Insert(0, new ListItem("-- All Employees --", "0"));
            }
            catch (Exception ex)
            {
                CommonBL errorlog = new CommonBL();
                errorlog.fnStoreErrorLog("AppraisalHistory", "BindEmployees", "Exception Message=" + ex.Message + " StackTrace=" + ex.StackTrace, UserId);
            }
        }

        private void BindHistory()
        {
            try
            {
                List<AppraisalDetailsDO> history = GetFilteredHistory();

                litTotalRecords.Text = history.Count.ToString();

                gvAppraisalHistory.DataSource = history;
                gvAppraisalHistory.DataBind();
            }
            catch (Exception ex)
            {
                CommonBL errorlog = new CommonBL();
                errorlog.fnStoreErrorLog("AppraisalHistory", "BindHistory", "Exception Message=" + ex.Message + " StackTrace=" + ex.StackTrace, UserId);
            }
        }

        private List<AppraisalDetailsDO> GetFilteredHistory()
        {
            int userId = 0;
            int.TryParse(ddlEmployeeFilter.SelectedValue, out userId);

            DateTime? fromDate = ParseDate(txtFromDate.Text);
            DateTime? toDate = ParseDate(txtToDate.Text);

            return appraisalBL.GetAppraisalHistory(userId, fromDate, toDate) ?? new List<AppraisalDetailsDO>();
        }

        private DateTime? ParseDate(string dateString)
        {
            if (string.IsNullOrWhiteSpace(dateString))
            {
                return null;
            }

            DateTime parsed;
            if (DateTime.TryParseExact(dateString.Trim(), "dd-MM-yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out parsed))
            {
                return parsed;
            }

            return null;
        }

        protected void btnSearch_Click(object sender, EventArgs e)
        {
            gvAppraisalHistory.PageIndex = 0;
            BindHistory();
        }

        protected void btnClear_Click(object sender, EventArgs e)
        {
            ddlEmployeeFilter.SelectedValue = "0";
            txtFromDate.Text = string.Empty;
            txtToDate.Text = string.Empty;
            gvAppraisalHistory.PageIndex = 0;
            BindHistory();
        }

        protected void gvAppraisalHistory_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            gvAppraisalHistory.PageIndex = e.NewPageIndex;
            BindHistory();
        }

        protected void btnExport_Click(object sender, EventArgs e)
        {
            try
            {
                List<AppraisalDetailsDO> history = GetFilteredHistory();

                StringBuilder sb = new StringBuilder();
                sb.Append("<table border='1'>");
                sb.Append("<tr><th>Employee Code</th><th>Employee Name</th><th>Effective Date</th><th>CTC</th><th>Gross Salary</th><th>Net Salary</th><th>Revision Date</th><th>Percentage %</th><th>Status</th><th>Saved On</th></tr>");

                foreach (AppraisalDetailsDO item in history)
                {
                    sb.Append("<tr>");
                    sb.Append("<td>" + HttpUtility.HtmlEncode(item.emp_code) + "</td>");
                    sb.Append("<td>" + HttpUtility.HtmlEncode(item.employee_name) + "</td>");
                    sb.Append("<td>" + item.appraisal_effective_date.ToString("dd-MMM-yyyy") + "</td>");
                    sb.Append("<td>" + item.appraisal_ctc.ToString("N2") + "</td>");
                    sb.Append("<td>" + item.gross_salary.ToString("N2") + "</td>");
                    sb.Append("<td>" + item.net_salary.ToString("N2") + "</td>");
                    sb.Append("<td>" + item.salary_revision_date.ToString("dd-MMM-yyyy") + "</td>");
                    sb.Append("<td>" + item.increament_percentage.ToString("N2") + "</td>");
                    sb.Append("<td>" + (item.is_active ? "Active" : "Inactive") + "</td>");
                    sb.Append("<td>" + HttpUtility.HtmlEncode(item.inserted_date_display) + "</td>");
                    sb.Append("</tr>");
                }

                sb.Append("</table>");

                Response.Clear();
                Response.Buffer = true;
                Response.ContentType = "application/vnd.ms-excel";
                Response.AddHeader("content-disposition", $"attachment;filename=AppraisalHistory_{DateTime.Now:yyyyMMddHHmmss}.xls");
                Response.Output.Write(sb.ToString());
                Response.Flush();
                Response.End();
            }
            catch (Exception ex)
            {
                CommonBL errorlog = new CommonBL();
                errorlog.fnStoreErrorLog("AppraisalHistory", "btnExport_Click", "Exception Message=" + ex.Message + " StackTrace=" + ex.StackTrace, UserId);
            }
        }

        protected void btnBack_Click(object sender, EventArgs e)
        {
            Response.Redirect("AppraisalDetails.aspx");
        }
    }
}
