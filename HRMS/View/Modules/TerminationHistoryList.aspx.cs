using DataObject;
using ProcessModel;
using System;
using System.Text;
using System.Web;

namespace HRMS.View.Modules
{
    public partial class TerminationHistoryList : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["userId"] == null)
            {
                Response.Redirect("~/view/authentication/login.aspx", false);
                return;
            }

            if (!IsPostBack)
            {
                BindHistory();
            }
        }

        private void BindHistory()
        {
            try
            {
                HandoverprocessBL bl = new HandoverprocessBL();
                var history = bl.GetTerminationHistory();

                gridHistory.DataSource = history;
                gridHistory.DataBind();
            }
            catch (Exception ex)
            {
                CommonBL errorlog = new CommonBL();
                errorlog.fnStoreErrorLog(
                    "TerminationHistoryList",
                    "BindHistory",
                    "Exception Message: " + ex.Message + " StackTrace: " + ex.StackTrace,
                    Convert.ToString(Session["userId"])
                );
            }
        }

        protected void gridHistory_PageIndexChanging(object sender, System.Web.UI.WebControls.GridViewPageEventArgs e)
        {
            gridHistory.PageIndex = e.NewPageIndex;
            BindHistory();
        }

        // Same pattern used elsewhere in the app (AttendanceDetails, etc.) -
        // an HTML table served with an Excel content-type, no extra library
        // needed. Exports the full history, not just the current page.
        protected void btnExport_Click(object sender, EventArgs e)
        {
            try
            {
                HandoverprocessBL bl = new HandoverprocessBL();
                var history = bl.GetTerminationHistory() ?? new System.Collections.Generic.List<TerminationHistoryDO>();

                StringBuilder sb = new StringBuilder();
                sb.Append("<table border='1'>");
                sb.Append("<tr><th>SR No</th><th>Employee Code</th><th>Employee Name</th><th>Action Type</th><th>Status</th><th>Recorded On</th></tr>");

                int srNo = 1;
                foreach (var item in history)
                {
                    sb.Append("<tr>");
                    sb.Append("<td>" + srNo + "</td>");
                    sb.Append("<td>" + HttpUtility.HtmlEncode(item.EmployeeCode) + "</td>");
                    sb.Append("<td>" + HttpUtility.HtmlEncode(item.EmployeeName) + "</td>");
                    sb.Append("<td>" + HttpUtility.HtmlEncode(item.ActionType) + "</td>");
                    sb.Append("<td>" + HttpUtility.HtmlEncode(item.Status) + "</td>");
                    sb.Append("<td>" + (item.RecordedDate.HasValue ? item.RecordedDate.Value.ToString("dd-MMM-yyyy") : "-") + "</td>");
                    sb.Append("</tr>");
                    srNo++;
                }

                sb.Append("</table>");

                Response.Clear();
                Response.ContentType = "application/vnd.ms-excel";
                Response.AddHeader("content-disposition", $"attachment;filename=TerminationHistory_{DateTime.Now:yyyyMMddHHmmss}.xls");
                Response.Output.Write(sb.ToString());
                Response.Flush();
                Response.End();
            }
            catch (Exception ex)
            {
                CommonBL errorlog = new CommonBL();
                errorlog.fnStoreErrorLog(
                    "TerminationHistoryList",
                    "btnExport_Click",
                    "Exception Message: " + ex.Message + " StackTrace: " + ex.StackTrace,
                    Convert.ToString(Session["userId"])
                );
            }
        }

        protected string GetStatusBadgeClass(object status)
        {
            string s = status?.ToString();
            if (s == "Terminated") return "terminated";
            if (s == "CAP1" || s == "CAP2") return "cap";
            if (s == "Show Cause Issued" || s == "Responded" || s == "Response_pending") return "showcause";
            if (s == "Removed") return "removed";
            return "other";
        }

        // Only rows still mid-workflow get a Manage link - Terminated/Removed
        // rows have nothing left to do.
        protected bool IsManageable(object status)
        {
            string s = status?.ToString();
            return s == "CAP1" || s == "CAP2" || s == "Show Cause Issued" || s == "Responded" || s == "Response_pending";
        }
    }
}
