using ProcessModel;
using System;
using System.IO;
using System.Web.UI;
using iText.Html2pdf;

namespace HRMS.View.Modules
{
    public partial class SendDocuments : System.Web.UI.Page
    {
        protected string UserId = null;

        private static readonly string[] AllowedUploadExtensions = { ".pdf", ".doc", ".docx" };
        private const int MaxAttachmentSizeBytes = 10 * 1024 * 1024; // 10 MB
        private const string UploadCategory = "Others";

        protected void Page_Load(object sender, EventArgs e)
        {
            UserId = Convert.ToString(Session["userId"]);
            if (Session["userId"] == null)
            {
                Response.Redirect("~/view/authentication/login.aspx", false);
                return;
            }

        }

        protected void btnSendMail_Click(object sender, EventArgs e)
        {
            try
            {
                string category = ddlDocumentCategory.SelectedValue;
                if (string.IsNullOrWhiteSpace(category))
                {
                    ShowResult("Error", "Please select a document category.");
                    return;
                }

                if (string.IsNullOrWhiteSpace(txtTo.Text))
                {
                    ShowResult("Error", "Please enter at least one recipient email.");
                    return;
                }

                if (string.IsNullOrWhiteSpace(txtBody.Text))
                {
                    ShowResult("Error", "Please enter the email body.");
                    return;
                }

                byte[] attachmentBytes;
                string fileName;

                if (category.Equals(UploadCategory, StringComparison.OrdinalIgnoreCase))
                {
                    if (!TryReadUploadedDocument(out attachmentBytes, out fileName))
                    {
                        return;
                    }
                }
                else
                {
                    if (!TryGenerateDocument(category, out attachmentBytes, out fileName))
                    {
                        return;
                    }
                }

                string bodyHtml = "<div style=\"font-family:Arial, sans-serif; font-size:14px; color:#333; line-height:1.6;\">" +
                    System.Web.HttpUtility.HtmlEncode(txtBody.Text.Trim()).Replace("\r\n", "<br/>").Replace("\n", "<br/>") +
                    "</div>";

                CommonBL commonBL = new CommonBL();
                commonBL.SendEmail(
                    txtTo.Text.Trim(),
                    txtCc.Text.Trim(),
                    txtBcc.Text.Trim(),
                    txtSubject.Text.Trim(),
                    bodyHtml,
                    attachmentBytes,
                    fileName
                );

                ShowResult("Success", "Document sent successfully.");
                ClearForm();
            }
            catch (Exception ex)
            {
                CommonBL errorlog = new CommonBL();
                errorlog.fnStoreErrorLog("SendDocuments", "btnSendMail_Click",
                    "Exception Message: " + ex.Message + " StackTrace: " + ex.StackTrace, UserId);
                ShowResult("Error", "Failed to send document. Please try again.");
            }
        }

        // "Others": whatever HR attaches is sent as-is, no generation involved.
        private bool TryReadUploadedDocument(out byte[] attachmentBytes, out string fileName)
        {
            attachmentBytes = null;
            fileName = null;

            if (!fuDocument.HasFile)
            {
                ShowResult("Error", "Please attach a document to send.");
                return false;
            }

            string extension = Path.GetExtension(fuDocument.FileName).ToLowerInvariant();
            if (Array.IndexOf(AllowedUploadExtensions, extension) < 0)
            {
                ShowResult("Error", "Only PDF and Word documents are allowed.");
                return false;
            }

            if (fuDocument.PostedFile.ContentLength > MaxAttachmentSizeBytes)
            {
                ShowResult("Error", "Attachment is too large. Maximum allowed size is 10 MB.");
                return false;
            }

            using (var ms = new MemoryStream())
            {
                fuDocument.PostedFile.InputStream.CopyTo(ms);
                attachmentBytes = ms.ToArray();
            }
            fileName = fuDocument.FileName;
            return true;
        }

        // Offer/Appointment/Confirmation letters: rendered from sp_get_onboarding_document_html.
        private bool TryGenerateDocument(string category, out byte[] pdfBytes, out string fileName)
        {
            pdfBytes = null;
            fileName = null;

            string candidateName = txtCandidateName.Text.Trim();
            if (string.IsNullOrWhiteSpace(candidateName))
            {
                ShowResult("Error", "Please type the employee/candidate name.");
                return false;
            }

            DateTime effectiveDate;
            if (!DateTime.TryParse(txtEffectiveDate.Text, out effectiveDate))
            {
                ShowResult("Error", "Please select the effective/confirmation date.");
                return false;
            }

            CommonBL commonBL = new CommonBL();
            string letterHtml = commonBL.GetOnboardingDocumentHtml(
                0,
                category,
                txtDesignation.Text.Trim(),
                effectiveDate,
                txtAdditionalDetails.Text.Trim(),
                candidateName
            );

            if (string.IsNullOrWhiteSpace(letterHtml))
            {
                ShowResult("Error", "Unable to generate the document. Please try again.");
                return false;
            }

            using (var ms = new MemoryStream())
            {
                HtmlConverter.ConvertToPdf(letterHtml, ms);
                pdfBytes = ms.ToArray();
            }

            if (pdfBytes == null || pdfBytes.Length == 0)
            {
                ShowResult("Error", "Failed to render the document to PDF. Please try again.");
                return false;
            }

            fileName = category.Replace(" ", "_").Replace("(", "").Replace(")", "") + "_" + candidateName.Replace(" ", "_") + ".pdf";
            return true;
        }

        private void ShowResult(string status, string message)
        {
            string safeMsg = System.Web.HttpUtility.JavaScriptStringEncode(message);
            ScriptManager.RegisterStartupScript(this, GetType(), "sendDocResult",
                $"showSendDocumentResult('{status}', '{safeMsg}');", true);
        }

        private void ClearForm()
        {
            ddlDocumentCategory.SelectedIndex = 0;
            txtDesignation.Text = string.Empty;
            txtCandidateName.Text = string.Empty;
            txtEffectiveDate.Text = string.Empty;
            txtAdditionalDetails.Text = string.Empty;
            txtTo.Text = string.Empty;
            txtCc.Text = string.Empty;
            txtBcc.Text = string.Empty;
            txtSubject.Text = string.Empty;
            txtBody.Text = string.Empty;
        }
    }
}
