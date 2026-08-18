using DataObject;
using ProcessModel;
using System;
using System.IO;
using System.Text.RegularExpressions;
using System.Web.UI;
using iText.Html2pdf;
using iText.IO.Font.Constants;
using iText.IO.Image;
using iText.Kernel.Colors;
using iText.Kernel.Font;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas.Draw;
using iText.Layout;
using iText.Layout.Borders;
using iText.Layout.Element;
using iText.Layout.Properties;

namespace HRMS.View.Modules
{
    public partial class ResignationHRReview : System.Web.UI.Page
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
                    ScriptManager.RegisterStartupScript(this, GetType(), "invalidResignation",
                        "alert('Invalid resignation request.');", true);
                    Response.Redirect("~/View/Modules/ResignationList.aspx", false);
                    return;
                }

                hfResignationId.Value = ResignationId.ToString();

                // Check if this is save mode or update mode
                string mode = Request.QueryString["mode"] ?? "update";
                BindHRReview(ResignationId, mode);
            }
            else
            {
                int.TryParse(hfResignationId.Value, out ResignationId);
            }
        }

        private void BindHRReview(int resignationId, string mode = "update")
        {
            try
            {
                HandoverprocessBL bl = new HandoverprocessBL();
                HRReviewDO model = bl.GetHRReviewDetails(resignationId);
                HRReviewDO existingReview = bl.GetHRReviewById(resignationId);

                lblEmployeeId.Text = model.EmployeeId > 0 ? model.EmployeeId.ToString() : "-";
                lblEmployeeName.Text = string.IsNullOrWhiteSpace(model.EmployeeName) ? "-" : model.EmployeeName;
                lblDepartment.Text = string.IsNullOrWhiteSpace(model.Department) ? "-" : model.Department;
                lblDesignation.Text = string.IsNullOrWhiteSpace(model.Designation) ? "-" : model.Designation;
                lblReportingManager.Text = string.IsNullOrWhiteSpace(model.ReportingManager) ? "-" : model.ReportingManager;
                lblDateOfJoining.Text = model.DateOfJoining.HasValue ? model.DateOfJoining.Value.ToString("dd-MM-yyyy") : "-";
                lblResignationDate.Text = model.ResignationDate == DateTime.MinValue ? "-" : model.ResignationDate.ToString("dd-MM-yyyy");
                lblProposedLastWorkingDate.Text = model.ProposedLastWorkingDate == DateTime.MinValue ? "-" : model.ProposedLastWorkingDate.ToString("dd-MM-yyyy");
                lblReason.Text = string.IsNullOrWhiteSpace(model.Reason) ? "-" : model.Reason;

                bool hasManagerRemark = !string.IsNullOrWhiteSpace(model.ManagerRemark);
                divManagerRemark.Visible = hasManagerRemark;
                if (hasManagerRemark)
                {
                    lblManagerRemark.Text = model.ManagerRemark;
                }

                bool alreadyReviewed = existingReview != null &&
                    string.Equals(existingReview.Status, "Accepted", StringComparison.OrdinalIgnoreCase);
                lblReviewStatus.Text = alreadyReviewed ? "HR review completed" : "HR review pending";
                lblReviewStatus.CssClass = alreadyReviewed ? "badge bg-success" : "badge bg-warning text-dark";

                // On initial load, default Revised Last Working Date to the Proposed Last
                // Working Date. It only switches to the Notice Start Date (today) +
                // Notice Days formula once the HR user actually changes Notice Days
                // (see recalculateRevisedLastWorkingDate() in the markup, and the
                // authoritative recompute in btnAcceptResignation_Click on submit).
                string defaultRevisedLastWorkingDate = model.ProposedLastWorkingDate == DateTime.MinValue
                    ? string.Empty
                    : model.ProposedLastWorkingDate.ToString("dd-MM-yyyy");

                // In save mode, don't populate existing review data - treat as new review
                if (mode == "save" || existingReview == null)
                {
                    txtNoticeDays.Text = "0";
                    txtRevisedLastWorkingDate.Text = defaultRevisedLastWorkingDate;
                    // Notice Start Date defaults to today when the form is opened for a
                    // new review; HR can still edit it before submitting.
                    txtNoticeStartDate.Text = DateTime.Today.ToString("yyyy-MM-dd");

                    hfHRReviewId.Value = "0";
                    btnAcceptResignation.Text = "Accept";

                    // Enable all fields for save mode. Revised Last Working Date stays
                    // ReadOnly (markup) - it is always derived from Notice Start Date
                    // + Notice Days, never entered manually.
                    ddlNoticePeriodRequired.Enabled = true;
                    ddlBuyoutApplicable.Enabled = true;
                    txtNoticeDays.Enabled = true;
                    txtNoticeStartDate.Enabled = true;
                    txtHRRemarks.Enabled = true;
                }
                else
                {
                    // Update mode - populate existing review data
                    ddlNoticePeriodRequired.SelectedValue = string.IsNullOrWhiteSpace(existingReview.NoticePeriodRequired) ? "" : existingReview.NoticePeriodRequired;
                    txtNoticeDays.Text = existingReview.NoticeDays.HasValue ? existingReview.NoticeDays.Value.ToString() : string.Empty;
                    ddlBuyoutApplicable.SelectedValue = string.IsNullOrWhiteSpace(existingReview.BuyoutApplicable) ? "" : existingReview.BuyoutApplicable;
                    txtRevisedLastWorkingDate.Text = defaultRevisedLastWorkingDate;
                    txtHRRemarks.Text = existingReview.HRRemarks;
                    // Show the saved Notice Start Date when reopening an existing review;
                    // fall back to today only for reviews saved before this field existed.
                    txtNoticeStartDate.Text = existingReview.NoticeStartDate.HasValue
                        ? existingReview.NoticeStartDate.Value.ToString("yyyy-MM-dd")
                        : DateTime.Today.ToString("yyyy-MM-dd");

                    // Update mode: Notice Period Required was already decided when the
                    // review was first saved - keep it locked so it can't be changed later.
                    ddlNoticePeriodRequired.Enabled = false;

                    hfHRReviewId.Value = existingReview.HRReviewId.ToString();
                    btnAcceptResignation.Text = "Update";
                }
            }
            catch (Exception ex)
            {
                CommonBL errorlog = new CommonBL();
                errorlog.fnStoreErrorLog("ResignationHRReview", "BindHRReview",
                    "Exception Message: " + ex.Message + " StackTrace: " + ex.StackTrace, UserId);
            }
        }

        protected void btnAcceptResignation_Click(object sender, EventArgs e)
        {
            // rfvNoticePeriodRequired/rfvNoticeStartDate/rfvBuyoutApplicable/
            // rfvRevisedLastWorkingDate/rfvHRRemarks (ValidationGroup "HRReviewGroup")
            // already show the inline below-field messages and block this postback
            // client-side; Page.IsValid is the server-side safety net.
            if (!Page.IsValid)
            {
                return;
            }

            int resignationId;
            int.TryParse(hfResignationId.Value, out resignationId);

            string noticePeriodRequired = ddlNoticePeriodRequired.SelectedValue;
            string buyoutApplicable = ddlBuyoutApplicable.SelectedValue;
            string hrRemarks = txtHRRemarks.Text.Trim();

            if (resignationId <= 0)
            {
                ScriptManager.RegisterStartupScript(this, GetType(), "invalidResignation",
                    "alert('Invalid resignation request.');", true);
                return;
            }

            DateTime noticeStartDate;
            if (!DateTime.TryParse(txtNoticeStartDate.Text.Trim(), out noticeStartDate))
            {
                ScriptManager.RegisterStartupScript(this, GetType(), "reqNoticeStartDate",
                    "alert('Please select Notice Start Date.');", true);
                return;
            }

            int noticeDaysParsed;
            bool noticeDaysProvided = int.TryParse(txtNoticeDays.Text.Trim(), out noticeDaysParsed) && noticeDaysParsed >= 0;
            int noticeDays = noticeDaysProvided ? noticeDaysParsed : 0;

            // Revised Last Working Date is derived from Notice Start Date + Notice Days -
            // recompute it authoritatively here rather than trusting the posted (ReadOnly)
            // textbox value. If Notice Days is empty/not selected, fall back to the
            // Proposed Last Working Date instead, matching the client-side display
            // behavior in recalculateRevisedLastWorkingDate().
            DateTime revisedLastWorkingDate;
            if (noticeDaysProvided)
            {
                revisedLastWorkingDate = noticeStartDate.AddDays(noticeDays);
            }
            else
            {
                DateTime proposedLastWorkingDate;
                revisedLastWorkingDate = DateTime.TryParse(lblProposedLastWorkingDate.Text, out proposedLastWorkingDate)
                    ? proposedLastWorkingDate
                    : noticeStartDate;
            }
            txtRevisedLastWorkingDate.Text = revisedLastWorkingDate.ToString("dd-MM-yyyy");

            int hrReviewId;
            int.TryParse(hfHRReviewId.Value, out hrReviewId);

            var model = new HRReviewDO
            {
                HRReviewId = hrReviewId,
                ResignationId = resignationId,
              
                NoticePeriodRequired = noticePeriodRequired,
                NoticeDays = noticeDays,
                BuyoutApplicable = buyoutApplicable,
                RevisedLastWorkingDate = revisedLastWorkingDate,
                HRRemarks = hrRemarks,
                NoticeStartDate= noticeStartDate
            };

            int updatedBy = Convert.ToInt32(Session["userId"]);

            try
            {
                HandoverprocessBL bl = new HandoverprocessBL();
                HRReviewResponseDO result = hrReviewId > 0
                    ? bl.UpdateHRReview(model, updatedBy)
                    : bl.SaveHRReviewAndAccept(model, updatedBy);

                if (result != null && result.Success)
                {
                    try
                    {
                        ResignationMailDO mailDetails = bl.GetResignationAcceptedMailDetails(resignationId);
                        byte[] pdfBytes = GenerateAcceptanceLetterPdf(mailDetails?.LetterHtml);

                        string fileName = "Resignation_Acceptance_" + resignationId + "_" + lblEmployeeName.Text + ".pdf";
                        bl.SendResignationAcceptedEmail(mailDetails, pdfBytes, fileName);
                    }
                    catch (Exception emailEx)
                    {
                        // Log email error but don't fail the main operation - the HR
                        // review was already saved successfully at this point.
                        CommonBL errorlog = new CommonBL();
                        errorlog.fnStoreErrorLog("ResignationHRReview", "btnAcceptResignation_Click-Email",
                            "Exception Message: " + emailEx.Message + " StackTrace: " + emailEx.StackTrace, UserId);
                    }

                    string safeMsg = System.Web.HttpUtility.JavaScriptStringEncode(
                        result.ResponseMsg ?? "HR Review saved successfully.And mail sent to Employee");
                    ScriptManager.RegisterStartupScript(this, GetType(), "hrReviewSaved",
                        $"showHRReviewResult('Success', '{safeMsg}', 'ResignationList.aspx');", true);
                }
                else
                {
                    string safeErr = System.Web.HttpUtility.JavaScriptStringEncode(
                        result?.ResponseMsg ?? "Unable to save HR review.");
                    ScriptManager.RegisterStartupScript(this, GetType(), "hrReviewError",
                        $"showHRReviewResult('Error', '{safeErr}', null);", true);
                }
            }
            catch (Exception ex)
            {
                CommonBL errorlog = new CommonBL();
                errorlog.fnStoreErrorLog("ResignationHRReview", "btnAcceptResignation_Click",
                    "Exception Message: " + ex.Message + " StackTrace: " + ex.StackTrace, UserId);
                ScriptManager.RegisterStartupScript(this, GetType(), "hrReviewException",
                    $"alert('Error: {ex.Message}');", true);
            }
        }

        protected void btnCancel_Click(object sender, EventArgs e)
        {
            Response.Redirect("~/View/Modules/ResignationList.aspx", false);
        }

        protected void btnBack_Click(object sender, EventArgs e)
        {
            Response.Redirect("~/View/Modules/ResignationList.aspx", false);
        }

        // sp_get_resignation_accepted_mail_details.LetterHtml is the COMPLETE letter
        // document - letterhead (logo/contact-info header) and all, with dynamic data
        // already filled in and the logo embedded as a base64 data URI (pulled from
        // app_image_library, same as sp_get_employee_payslip.sql does). This method
        // builds no markup of its own - it's just a pass-through to the PDF renderer.
        private byte[] GenerateAcceptanceLetterPdf(string letterHtml)
        {
            try
            {
                using (var ms = new MemoryStream())
                {
                    HtmlConverter.ConvertToPdf(letterHtml ?? string.Empty, ms);
                    return ms.ToArray();
                }
            }
            catch
            {
                return GenerateAcceptanceLetterPdfFallback(letterHtml);
            }
        }

        private static readonly Regex LetterBlockPattern = new Regex(
            @"<p[^>]*>(.*?)</p>|<ul[^>]*>(.*?)</ul>", RegexOptions.Singleline | RegexOptions.IgnoreCase);
        private static readonly Regex LetterListItemPattern = new Regex(
            @"<li[^>]*>(.*?)</li>", RegexOptions.Singleline | RegexOptions.IgnoreCase);
        private static readonly Regex HtmlTagPattern = new Regex(@"<[^>]+>", RegexOptions.Singleline);

        private static string StripHtmlTags(string html)
        {
            if (string.IsNullOrWhiteSpace(html))
            {
                return string.Empty;
            }

            return System.Web.HttpUtility.HtmlDecode(HtmlTagPattern.Replace(html, string.Empty)).Trim();
        }

        private byte[] GenerateAcceptanceLetterPdfFallback(string letterHtml)
        {
            try
            {
                using (var ms = new MemoryStream())
                {
                    WriterProperties writerProperties = new WriterProperties();
                    PdfWriter writer = new PdfWriter(ms, writerProperties);
                    PdfDocument pdf = new PdfDocument(writer);
                    Document document = new Document(pdf);
                    document.SetMargins(42, 42, 42, 42);

                    PdfFont boldFont = PdfFontFactory.CreateFont(StandardFonts.HELVETICA_BOLD);
                    PdfFont normalFont = PdfFontFactory.CreateFont(StandardFonts.HELVETICA);

                    iText.Layout.Element.Table headerTable = new iText.Layout.Element.Table(UnitValue.CreatePercentArray(new float[] { 45, 55 })).UseAllAvailableWidth();

                    string logoPath = Server.MapPath("~/assets/images/alphonsol_logo.png");
                    if (File.Exists(logoPath))
                    {
                        iText.Layout.Element.Image logo = new iText.Layout.Element.Image(ImageDataFactory.Create(logoPath)).SetWidth(110);
                        headerTable.AddCell(new Cell().Add(logo).SetBorder(Border.NO_BORDER).SetVerticalAlignment(VerticalAlignment.MIDDLE));
                    }
                    else
                    {
                        headerTable.AddCell(new Cell().SetBorder(Border.NO_BORDER));
                    }

                    Div contactInfo = new Div().SetTextAlignment(TextAlignment.RIGHT);
                    contactInfo.Add(new Paragraph("High-street Corporate Center, FB-03, Kapurbawdi Junction, Thane(W)-400601")
                        .SetFont(boldFont).SetFontSize(9).SetMarginBottom(0));
                    contactInfo.Add(new Paragraph("Contact No - 9920393999")
                        .SetFont(boldFont).SetFontSize(9).SetMarginBottom(0));
                    contactInfo.Add(new Paragraph("Email Address - support@alphonsol.com")
                        .SetFont(boldFont).SetFontSize(9).SetMarginBottom(0));
                    contactInfo.Add(new Paragraph("Website - www.alphonsol.com")
                        .SetFont(boldFont).SetFontSize(9).SetMarginBottom(0));
                    headerTable.AddCell(new Cell().Add(contactInfo).SetBorder(Border.NO_BORDER).SetVerticalAlignment(VerticalAlignment.MIDDLE));

                    document.Add(headerTable);

                    LineSeparator line = new LineSeparator(new SolidLine());
                    line.SetStrokeColor(new DeviceRgb(255, 140, 0));
                    document.Add(line);
                    document.Add(new Paragraph("\n"));

                    document.Add(new Paragraph("Acceptance of Resignation").SetFont(boldFont).SetFontSize(14).SetMarginBottom(14));

                    if (!string.IsNullOrWhiteSpace(letterHtml))
                    {
                        foreach (Match block in LetterBlockPattern.Matches(letterHtml))
                        {
                            if (block.Groups[1].Success)
                            {
                                string text = StripHtmlTags(block.Groups[1].Value);
                                if (!string.IsNullOrWhiteSpace(text))
                                {
                                    document.Add(new Paragraph(text).SetFont(normalFont).SetFontSize(11).SetMarginBottom(10));
                                }
                            }
                            else if (block.Groups[2].Success)
                            {
                                iText.Layout.Element.List list = new iText.Layout.Element.List().SetListSymbol("• ").SetFont(normalFont).SetFontSize(11).SetMarginBottom(14);
                                foreach (Match li in LetterListItemPattern.Matches(block.Groups[2].Value))
                                {
                                    list.Add(new iText.Layout.Element.ListItem(StripHtmlTags(li.Groups[1].Value)));
                                }
                                document.Add(list);
                            }
                        }
                    }

                    document.Close();
                    return ms.ToArray();
                }
            }
            catch (Exception ex)
            {
                CommonBL errorlog = new CommonBL();
                errorlog.fnStoreErrorLog("ResignationHRReview", "GenerateAcceptanceLetterPdfFallback",
                    "Exception Message: " + ex.Message + " StackTrace: " + ex.StackTrace, UserId);
                return null;
            }
        }
    }
}
