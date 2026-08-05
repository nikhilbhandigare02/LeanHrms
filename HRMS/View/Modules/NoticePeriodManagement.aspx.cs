using DataObject;
using ProcessModel;
using System;
using System.Web.UI;

namespace HRMS.View.Modules
{
    public partial class NoticePeriodManagement : System.Web.UI.Page
    {
        protected string UserId = null;
        protected int ResignationId = 0;

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
                if (Request.QueryString["ResignationId"] != null)
                {
                    int.TryParse(Request.QueryString["ResignationId"], out ResignationId);
                }

                if (ResignationId <= 0)
                {
                    // Reached from the sidebar "Notice" menu item with no specific
                    // resignation context - fall back to the logged-in user's own
                    // most recent resignation, so they see their own notice period.
                    int sessionUserId;
                    if (int.TryParse(UserId, out sessionUserId))
                    {
                        ResignationId = new HandoverprocessBL().GetLatestResignationIdForUser(sessionUserId);
                    }
                }

                hfResignationId.Value = ResignationId.ToString();

                if (ResignationId > 0)
                {
                    BindNoticePeriod(ResignationId);
                }
                else
                {
                    ShowEmptyState();
                }
            }
        }

        private void ShowEmptyState()
        {
            lblNoticeStatus.Text = "No active notice period";
            lblNoticeStatus.CssClass = "badge bg-secondary";
        }

        private void BindNoticePeriod(int resignationId)
        {
            try
            {
                HandoverprocessBL bl = new HandoverprocessBL();
                NoticePeriodDO model = bl.GetNoticePeriodDetails(resignationId);

                bool hasEndDate = model != null && model.NoticeEndDate.HasValue;
                int remainingDays = (model != null && model.RemainingDays.HasValue) ? model.RemainingDays.Value : 0;
                bool isActive = hasEndDate && remainingDays >= 0;

                lblNoticeStatus.Text = isActive ? "Active" : "Completed";
                lblNoticeStatus.CssClass = isActive ? "badge bg-success" : "badge bg-secondary";

                lblNoticeStartDate.Text = (model != null && model.NoticeStartDate.HasValue)
                    ? model.NoticeStartDate.Value.ToString("dd MMM yyyy") : "-";
                lblNoticeEndDate.Text = hasEndDate ? model.NoticeEndDate.Value.ToString("dd MMM yyyy") : "-";
                lblRemainingDays.Text = (model != null && model.RemainingDays.HasValue)
                    ? Math.Max(model.RemainingDays.Value, 0).ToString() : "-";
                lblLastWorkingDate.Text = (model != null && model.LastWorkingDate.HasValue)
                    ? model.LastWorkingDate.Value.ToString("dd MMM yyyy") : "-";

                string attendanceStatus = model != null ? model.AttendanceStatus : null;
                lblAttendanceStatus.Text = string.IsNullOrWhiteSpace(attendanceStatus) ? "-" : attendanceStatus;
                lblAttendanceStatus.CssClass = string.Equals(attendanceStatus, "Present", StringComparison.OrdinalIgnoreCase)
                    ? "info-value text-success"
                    : "info-value text-muted";
            }
            catch (Exception ex)
            {
                CommonBL errorlog = new CommonBL();
                errorlog.fnStoreErrorLog("NoticePeriodManagement", "BindNoticePeriod",
                    "Exception Message: " + ex.Message + " StackTrace: " + ex.StackTrace, UserId);
            }
        }
    }
}
