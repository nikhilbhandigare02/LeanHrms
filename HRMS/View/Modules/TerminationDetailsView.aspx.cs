using DataObject;
using ProcessModel;
using System;
using System.Linq;
using System.Text;
using System.Web;

namespace HRMS.View.Modules
{
    public partial class TerminationDetailsView : System.Web.UI.Page
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
                int userId;
                int.TryParse(Request.QueryString["user_id"], out userId);

                TerminationProcessDO record = null;
                if (userId > 0)
                {
                    HandoverprocessBL bl = new HandoverprocessBL();
                    record = bl.GetTerminationRecordForView(userId);
                }

                if (record == null)
                {
                    pnlDetails.Visible = false;
                    pnlNotFound.Visible = true;
                    return;
                }

                BindRecord(record);
            }
        }
        //code by tanmaya
        private void BindRecord(TerminationProcessDO record)
        {
            litEmployeeName.Text = HttpUtility.HtmlEncode(record.EmployeeName ?? "-");
            litEmployeeCode.Text = HttpUtility.HtmlEncode(record.EmployeeCode ?? "-");
            litInitials.Text = GetInitials(record.EmployeeName);

            litStatus.Text = "<span class='status-badge " + GetStatusBadgeClass(record.notice_status) + "'>"
                + HttpUtility.HtmlEncode(record.notice_status ?? "-") + "</span>";

            litTerminationType.Text = HttpUtility.HtmlEncode(FormatType(record.TerminationType));
            litReason.Text = HttpUtility.HtmlEncode(record.termination_reason ?? "-");
            litTerminationDate.Text = (record.TerminationDate != default(DateTime))
                ? record.TerminationDate.ToString("dd-MMM-yyyy")
                : "-";

            string letterContent = null;

            if (record.TerminationType == "Performance")
            {
                pnlPerformance.Visible = true;
                litPerformanceRating.Text = record.PerformanceRating.HasValue
                    ? record.PerformanceRating.Value + " / 5"
                    : "-";
                litNoticePeriod.Text = record.NoticePeriodDays.HasValue
                    ? record.NoticePeriodDays.Value + " days"
                    : "-";
                letterContent = record.TerminationLetter;
            }
            else if (record.TerminationType == "ShowCause")
            {
                pnlShowCause.Visible = true;
                litResponseDeadline.Text = record.ResponseDeadline.HasValue
                    ? record.ResponseDeadline.Value.ToString("dd-MMM-yyyy")
                    : "-";
                letterContent = !string.IsNullOrWhiteSpace(record.TerminationLetter)
                    ? record.TerminationLetter
                    : record.NoticeLetter;
            }
            else
            {
                letterContent = record.TerminationLetter;
            }

            litLetterContent.Text = FormatLetterHtml(letterContent);
        }

        // Renders the stored letter as proper paragraphs instead of raw
        // pre-wrapped text - trims each line so any stray leading/trailing
        // whitespace in the saved content doesn't throw the left alignment
        // off (the ragged-indent look), and gives blank-line-separated
        // paragraphs real spacing instead of relying on literal blank lines.
        private string FormatLetterHtml(string text)
        {
            if (string.IsNullOrWhiteSpace(text))
                return "-";

            string normalized = text.Replace("\r\n", "\n").Replace("\r", "\n");
            string[] paragraphs = normalized.Split(new[] { "\n\n" }, StringSplitOptions.None);

            StringBuilder sb = new StringBuilder();
            foreach (string paragraph in paragraphs)
            {
                string trimmedParagraph = paragraph.Trim();
                if (trimmedParagraph.Length == 0)
                    continue;

                string[] lines = trimmedParagraph
                    .Split('\n')
                    .Select(l => HttpUtility.HtmlEncode(l.Trim()))
                    .ToArray();

                sb.Append("<p class='letter-paragraph'>");
                sb.Append(string.Join("<br/>", lines));
                sb.Append("</p>");
            }

            return sb.Length > 0 ? sb.ToString() : "-";
        }

        private string FormatType(string terminationType)
        {
            switch (terminationType)
            {
                case "Performance": return "Performance Based Termination";
                case "ShowCause": return "Show Cause Notice";
                case "DirectTerminate": return "Direct Termination";
                default: return terminationType ?? "-";
            }
        }

        private string GetStatusBadgeClass(string status)
        {
            if (status == "Terminated") return "terminated";
            return "showcause";
        }

        private string GetInitials(string fullName)
        {
            if (string.IsNullOrWhiteSpace(fullName)) return "?";
            var parts = fullName.Trim().Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length == 1) return parts[0].Substring(0, 1).ToUpper();
            return (parts[0].Substring(0, 1) + parts[parts.Length - 1].Substring(0, 1)).ToUpper();
        }
    }
}
