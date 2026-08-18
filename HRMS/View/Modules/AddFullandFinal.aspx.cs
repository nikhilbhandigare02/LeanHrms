using DataObject;
using ProcessModel;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Net.Mime;
using System.Text.RegularExpressions;
using System.Web.UI;
using System.Web.UI.WebControls;
using iText.Html2pdf;

namespace HRMS.View.Modules
{
    public partial class AddFullandFinal : System.Web.UI.Page
    {
        protected string UserId = null;
        private const int SalarySlipMonthsBack = 6;

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
                BindEmployeeDropdown();
            }
        }

        // Only HR-accepted resignations are eligible for Full and Final settlement.
        private void BindEmployeeDropdown()
        {
            ddlEmployee.Items.Clear();
            ddlEmployee.Items.Add(new ListItem("-- Select Employee --", ""));

            try
            {
                List<ResignationDO> resignations = new HandoverprocessBL().GetEmployeeResignationDetails(0);
                if (resignations != null)
                {
                    var accepted = resignations
                        .Where(r => string.Equals(r.hr_status, "Accepted", StringComparison.OrdinalIgnoreCase))
                        .OrderBy(r => r.EmployeeName);

                    foreach (ResignationDO r in accepted)
                    {
                        ddlEmployee.Items.Add(new ListItem(r.EmployeeName, r.EmployeeResignationId.ToString()));
                    }
                }
            }
            catch (Exception ex)
            {
                new CommonBL().fnStoreErrorLog("AddFullandFinal", "BindEmployeeDropdown",
                    "Exception Message: " + ex.Message + " StackTrace: " + ex.StackTrace, UserId);
            }
        }

        protected void ddlEmployee_SelectedIndexChanged(object sender, EventArgs e)
        {
            int resignationId;
            if (!int.TryParse(ddlEmployee.SelectedValue, out resignationId) || resignationId <= 0)
            {
                pnlMail.Visible = false;
                return;
            }

            try
            {
                HandoverprocessBL bl = new HandoverprocessBL();
                HRReviewDO review = bl.GetHRReviewDetails(resignationId);
                ResignationMailDO mail = bl.GetFullAndFinalMailDetails(resignationId);

                if (review == null || string.IsNullOrWhiteSpace(review.EmployeeCode) ||
                    mail == null || string.IsNullOrWhiteSpace(mail.ToEmail))
                {
                    pnlMail.Visible = false;
                    ShowResult("Error", "Unable to load this employee's details.");
                    return;
                }

                txtTo.Text = mail.ToEmail;
                txtCc.Text = mail.CcEmail;
                txtSubject.Text = mail.Subject;
                txtBody.Text = HtmlToPlainText(mail.Body);

                List<string> monthLabels = GetAvailableSalarySlipMonths(review.EmployeeCode)
                    .Select(m => m.Label)
                    .ToList();
                lblAttachments.Text = BuildAttachmentsSummary(monthLabels);

                pnlMail.Visible = true;
            }
            catch (Exception ex)
            {
                pnlMail.Visible = false;
                new CommonBL().fnStoreErrorLog("AddFullandFinal", "ddlEmployee_SelectedIndexChanged",
                    "Exception Message: " + ex.Message + " StackTrace: " + ex.StackTrace, UserId);
            }
        }

        protected void btnSendMail_Click(object sender, EventArgs e)
        {
            int resignationId;
            if (!int.TryParse(ddlEmployee.SelectedValue, out resignationId) || resignationId <= 0)
            {
                ShowResult("Error", "Please select an employee.");
                return;
            }

            try
            {
                HandoverprocessBL bl = new HandoverprocessBL();
                HRReviewDO review = bl.GetHRReviewDetails(resignationId);

                if (review == null || string.IsNullOrWhiteSpace(review.EmployeeCode))
                {
                    ShowResult("Error", "Unable to load this employee's details.");
                    return;
                }

                if (string.IsNullOrWhiteSpace(txtTo.Text))
                {
                    ShowResult("Error", "This employee has no email on file.");
                    return;
                }

                DateTime effectiveDate = review.ProposedLastWorkingDate == DateTime.MinValue
                    ? DateTime.Today
                    : review.ProposedLastWorkingDate;

                CommonBL commonBL = new CommonBL();
                var attachments = new List<KeyValuePair<string, byte[]>>();

                byte[] relievingPdf = GenerateOnboardingLetterPdf(commonBL, review, "Relieving Letter", effectiveDate);
                if (relievingPdf != null)
                {
                    attachments.Add(new KeyValuePair<string, byte[]>("Relieving_Letter_" + review.EmployeeCode + ".pdf", relievingPdf));
                }

                byte[] experiencePdf = GenerateOnboardingLetterPdf(commonBL, review, "Experience Letter", effectiveDate);
                if (experiencePdf != null)
                {
                    attachments.Add(new KeyValuePair<string, byte[]>("Experience_Letter_" + review.EmployeeCode + ".pdf", experiencePdf));
                }

                SalarySlipBL salaryBl = new SalarySlipBL();
                foreach (var monthInfo in GetAvailableSalarySlipMonths(review.EmployeeCode))
                {
                    byte[] slipPdf = GenerateSalarySlipPdf(salaryBl, review.EmployeeCode, monthInfo.Year, monthInfo.Month);
                    if (slipPdf != null)
                    {
                        attachments.Add(new KeyValuePair<string, byte[]>(
                            "SalarySlip_" + review.EmployeeCode + "_" + monthInfo.Label.Replace(" ", "_") + ".pdf",
                            slipPdf));
                    }
                }

                if (attachments.Count == 0)
                {
                    ShowResult("Error", "No documents could be generated for this employee. Please check the error log.");
                    return;
                }

                string bodyHtml = "<div style=\"font-family:Arial, sans-serif; font-size:14px; color:#333; line-height:1.6;\">" +
                    System.Web.HttpUtility.HtmlEncode(txtBody.Text.Trim()).Replace("\r\n", "<br/>").Replace("\n", "<br/>") +
                    "</div>";

                SendMailWithAttachments(txtTo.Text.Trim(), txtCc.Text.Trim(), txtSubject.Text.Trim(), bodyHtml, attachments);

                bl.MarkRelievingMailSent(resignationId);

                ShowResult("Success", "Full and Final settlement mail sent successfully with " + attachments.Count + " attachment(s).");
            }
            catch (Exception ex)
            {
                new CommonBL().fnStoreErrorLog("AddFullandFinal", "btnSendMail_Click",
                    "Exception Message: " + ex.Message + " StackTrace: " + ex.StackTrace, UserId);
                ShowResult("Error", "Failed to send the settlement mail. Please try again.");
            }
        }

        // sp_get_onboarding_document_html ("Relieving Letter" / "Experience Letter" cases)
        // -> iText HtmlConverter, same pattern SendDocuments.aspx uses for onboarding letters.
        private byte[] GenerateOnboardingLetterPdf(CommonBL commonBL, HRReviewDO review, string category, DateTime effectiveDate)
        {
            try
            {
                string letterHtml = commonBL.GetOnboardingDocumentHtml(
                    review.EmployeeId,
                    category,
                    review.Designation,
                    effectiveDate,
                    string.Empty
                );

                if (string.IsNullOrWhiteSpace(letterHtml))
                {
                    return null;
                }

                using (var ms = new MemoryStream())
                {
                    HtmlConverter.ConvertToPdf(letterHtml, ms);
                    byte[] bytes = ms.ToArray();
                    return bytes.Length > 0 ? bytes : null;
                }
            }
            catch (Exception ex)
            {
                new CommonBL().fnStoreErrorLog("AddFullandFinal", "GenerateOnboardingLetterPdf",
                    category + ": " + ex.Message + " StackTrace: " + ex.StackTrace, UserId);
                return null;
            }
        }

        private byte[] GenerateSalarySlipPdf(SalarySlipBL salaryBl, string employeeCode, int year, int month)
        {
            try
            {
                string html = salaryBl.GetSalarySlipHtml(employeeCode, year, month);
                if (string.IsNullOrWhiteSpace(html))
                {
                    return null;
                }

                using (var ms = new MemoryStream())
                {
                    HtmlConverter.ConvertToPdf(html, ms);
                    byte[] bytes = ms.ToArray();
                    return bytes.Length > 0 ? bytes : null;
                }
            }
            catch (Exception ex)
            {
                new CommonBL().fnStoreErrorLog("AddFullandFinal", "GenerateSalarySlipPdf",
                    year + "-" + month + ": " + ex.Message + " StackTrace: " + ex.StackTrace, UserId);
                return null;
            }
        }

        // The 6 calendar months ending with the current month, restricted to whichever
        // of those actually have a saved salary slip - missing months are skipped rather
        // than blocking the mail (per the "attach whichever exist" requirement).
        private List<(int Year, int Month, string Label)> GetAvailableSalarySlipMonths(string employeeCode)
        {
            var found = new List<(int Year, int Month, string Label)>();
            SalarySlipBL salaryBl = new SalarySlipBL();
            DateTime cursor = DateTime.Today;

            for (int i = 0; i < SalarySlipMonthsBack; i++)
            {
                DateTime monthDate = cursor.AddMonths(-i);
                string html = salaryBl.GetSalarySlipHtml(employeeCode, monthDate.Year, monthDate.Month);
                if (!string.IsNullOrWhiteSpace(html))
                {
                    found.Add((monthDate.Year, monthDate.Month, monthDate.ToString("MMM yyyy")));
                }
            }

            found.Reverse(); // oldest first
            return found;
        }

        private string BuildAttachmentsSummary(List<string> monthLabels)
        {
            string slipsText = monthLabels.Count > 0
                ? string.Join(", ", monthLabels)
                : "none found for the last 6 months";
            return " Relieving Letter, Experience Letter, and Salary Slip(s): " + slipsText + ".";
        }

        private static string HtmlToPlainText(string html)
        {
            if (string.IsNullOrWhiteSpace(html))
            {
                return string.Empty;
            }

            string text = Regex.Replace(html, "<br\\s*/?>", "\n", RegexOptions.IgnoreCase);
            text = Regex.Replace(text, "</p>", "\n\n", RegexOptions.IgnoreCase);
            text = Regex.Replace(text, "<li>", "- ", RegexOptions.IgnoreCase);
            text = Regex.Replace(text, "</li>", "\n", RegexOptions.IgnoreCase);
            text = Regex.Replace(text, "<[^>]+>", string.Empty);
            return System.Web.HttpUtility.HtmlDecode(text).Trim();
        }

        private void SendMailWithAttachments(string toMail, string ccMail, string subject, string bodyHtml, List<KeyValuePair<string, byte[]>> attachments)
        {
            string emailFrom = ConfigurationManager.AppSettings["SenderEmail"];
            string password = ConfigurationManager.AppSettings["SenderPassword"];
            int port = Convert.ToInt32(ConfigurationManager.AppSettings["SenderPort"]);
            string host = ConfigurationManager.AppSettings["SenderHost"];

            using (MailMessage mail = new MailMessage())
            {
                mail.From = new MailAddress(emailFrom, "HRMS");

                foreach (string addr in toMail.Split(new[] { ';', ',' }, StringSplitOptions.RemoveEmptyEntries))
                {
                    if (!string.IsNullOrWhiteSpace(addr))
                        mail.To.Add(addr.Trim());
                }

                if (!string.IsNullOrWhiteSpace(ccMail))
                {
                    foreach (string addr in ccMail.Split(new[] { ';', ',' }, StringSplitOptions.RemoveEmptyEntries))
                    {
                        if (!string.IsNullOrWhiteSpace(addr))
                            mail.CC.Add(addr.Trim());
                    }
                }

                mail.Subject = subject;
                mail.Body = bodyHtml;
                mail.IsBodyHtml = true;

                foreach (var attachment in attachments)
                {
                    var stream = new MemoryStream(attachment.Value);
                    mail.Attachments.Add(new Attachment(stream, attachment.Key, MediaTypeNames.Application.Pdf));
                }

                using (SmtpClient smtp = new SmtpClient(host, port))
                {
                    smtp.UseDefaultCredentials = false;
                    smtp.Credentials = new NetworkCredential(emailFrom, password);
                    smtp.EnableSsl = true;
                    smtp.Send(mail);
                }
            }
        }

        private void ShowResult(string status, string message)
        {
            string safeMsg = System.Web.HttpUtility.JavaScriptStringEncode(message);
            ScriptManager.RegisterStartupScript(this, GetType(), "fullAndFinalResult",
                $"showFullAndFinalResult('{status}', '{safeMsg}');", true);
        }
    }
}
