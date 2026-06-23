using DataObject;
using Newtonsoft.Json;
using ProcessModel;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Mail;
using System.Net.Mime;
using System.IO;
using System.Text;
using System.Web;
using System.Web.Script.Serialization;
using System.Web.UI;
using System.Web.UI.WebControls;
using iText.Kernel.Pdf;
using iText.Layout;
using iText.Layout.Element;
using iText.Layout.Properties;
using iText.Kernel.Font;
using iText.IO.Font.Constants;
using iText.Kernel.Colors;
using iText.Kernel.Pdf.Canvas.Draw;
using iText.IO.Image;
using iText.Layout.Borders;
using iText.Html2pdf;
using System.Drawing;
using System.Drawing.Imaging;
using System.Drawing.Drawing2D;

namespace HRMS.View.Modules
{
    public partial class ResignationList : System.Web.UI.Page
    {
        protected string UserId = null;
        protected void Page_Load(object sender, EventArgs e)
        {
            UserId = Convert.ToString(Session["userId"]);
            int userId = 0;
            if (!IsPostBack)
            {

                if (Session["userId"] == null)
                {
                    Response.Redirect("~/view/authentication/login.aspx", false);
                    return;
                }

                if (Request.QueryString["user_id"] != null)
                {
                    userId = Convert.ToInt32(Request.QueryString["user_id"]);
                }
                else
                {
                    userId = 0;
                }

                BindResignationGrid();
            }
        }
        protected void BindResignationGrid()
        {
            try
            {
                int reportingManagerId = Convert.ToInt32(Session["userId"]);
                HandoverprocessBL resignationBL = new HandoverprocessBL();
                var resignations = resignationBL.GetEmployeeResignationDetails(reportingManagerId)
                    .OrderByDescending(x => x.EmployeeResignationId)
                    .ToList();

                foreach (var row in resignations)
                {
                    row.last_working_date_display = row.last_working_date == DateTime.MinValue
                        ? "-"
                        : row.last_working_date.ToString("yyyy-MM-dd");
                }

                Session["ResignationListData"] = resignations;

                ApplySorting(ref resignations); 

                int totalRecords = resignations.Count;
                int pageIndex = Convert.ToInt32(Session["CurrentPageIndex"] ?? 0);
                int pageSize = 10;
                int totalPages = totalRecords > 0 ? (int)Math.Ceiling((double)totalRecords / pageSize) : 1;
                if (pageIndex < 0) pageIndex = 0;
                if (pageIndex >= totalPages) pageIndex = totalPages - 1;
                Session["CurrentPageIndex"] = pageIndex;
                hfPageIndexViewUser.Value = pageIndex.ToString();
                int startRowIndex = pageIndex * pageSize;
                int endRowIndex = Math.Min(startRowIndex + pageSize, totalRecords);

                if (totalRecords > 0)
                {
                    List<ResignationDO> displayedData = resignations.GetRange(startRowIndex, endRowIndex - startRowIndex);

                    gvResignations.DataSource = displayedData; 
                    gvResignations.DataBind();
                    gvResignations.Visible = true;

                    if (totalRecords > pageSize)
                    {
                        paginationContainer.Visible = true;
                        ddlPageSelector.Visible = true;
                        UpdatePageInfoLabel(pageIndex, totalRecords);
                    }
                    else
                    {
                        paginationContainer.Visible = false;
                        ddlPageSelector.Visible = false;
                    }
                }
                else
                {
                    gvResignations.DataSource = null;
                    gvResignations.DataBind();
                    gvResignations.Visible = true;
                    ddlPageSelector.Visible = false;
                    UpdatePageInfoLabel(0, 0);
                }
            }
            catch (Exception ex)
            {
                gvResignations.Visible = true;
                gvResignations.DataSource = null;
                gvResignations.DataBind();
                CommonBL errorlog = new CommonBL();
                errorlog.fnStoreErrorLog("ResignationBL", "BindResignationGridFromAPI",
                    "Exception Message: " + ex.Message + " StackTrace: " + ex.StackTrace, UserId);
            }
        }
        protected List<ResignationDO> GetResignationsFromAPI()
        {
            List<ResignationDO> users = new List<ResignationDO>();
            try
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri("http://103.118.17.144:813/");
                    //client.BaseAddress = new Uri("https://localhost:44360/");
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(
                        new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json")
                    );

                    HttpResponseMessage response =
        client.PostAsync(
            "UserList/GetResignationDetails",
            new StringContent("{}", Encoding.UTF8, "application/json")
        ).Result;


                    if (response.IsSuccessStatusCode)
                    {
                        var jsonString = response.Content.ReadAsStringAsync().Result;
                        JavaScriptSerializer js = new JavaScriptSerializer();
                        var result = js.Deserialize<ResignationResponseDO>(jsonString);

                        if (result.Success && result.ResignationList != null)
                        {
                            users = result.ResignationList.Select(u => new ResignationDO
                            {
                                EmployeeResignationId = u.EmployeeResignationId,
                                UserId = u.UserId,
                                EmployeeName = u.EmployeeName,
                                EmployeeEmail = u.EmployeeEmail,
                                resignation_date = u.resignation_date,
                                notice_period_days = u.notice_period_days,
                                last_working_date = u.last_working_date,
                                reason = u.reason,
                                hr_status = u.hr_status,
                                last_working_date_display=u.last_working_date_display
                            }).ToList();

                            UserDetailsBL userBL = new UserDetailsBL();
                            var userMap = userBL.ViewAllUsers()
                                .GroupBy(x => x.UserId)
                                .ToDictionary(g => g.Key, g => g.First());

                            users = users.Where(r => userMap.ContainsKey(r.UserId)).ToList();

                            foreach (var row in users)
                            {
                                UserDetailsDO userInfo;
                                if (userMap.TryGetValue(row.UserId, out userInfo))
                                {
                                    row.EmployeeName = string.IsNullOrWhiteSpace(userInfo.user_fullname) ? row.EmployeeName : userInfo.user_fullname;
                                    row.EmployeeEmail = string.IsNullOrWhiteSpace(userInfo.user_mail_id) ? row.EmployeeEmail : userInfo.user_mail_id;
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // log error
            }
            return users;
        }

        protected void btnSubmitResignationAction_Click(object sender, EventArgs e)
        {
            int resignationId = Convert.ToInt32(hfResignationId.Value);
            string action = hfHrAction.Value;  // Use hidden field
            string remark = txtHrRemark.Text.Trim();
            int updatedBy = Convert.ToInt32(Session["userId"]);

            int? extendedDays = string.IsNullOrEmpty(txtExtendedDays.Text)
                ? null : (int?)Convert.ToInt32(txtExtendedDays.Text);

            DateTime? lastWorkingDate = string.IsNullOrEmpty(txtLastWorkingDate.Text)
                ? null : (DateTime?)Convert.ToDateTime(txtLastWorkingDate.Text);

            if (action == "Rejected" && string.IsNullOrEmpty(remark))
            {
                ScriptManager.RegisterStartupScript(this, GetType(), "alert", "alert('Please enter HR remark for rejection.');", true);
                return;
            }

            try
            {
                HandoverprocessBL bl = new HandoverprocessBL();
                ResignationActionResponseDO result = bl.UpdateResignationActionBySp(
                    resignationId,
                    action,
                    remark,
                    lastWorkingDate,
                    extendedDays,
                    updatedBy
                );

                if (result != null && result.Success)
                {
                    string safeMsg = HttpUtility.JavaScriptStringEncode(result.ResponseMsg ?? "Resignation updated successfully.");
                    ScriptManager.RegisterStartupScript(
                         this,
                         GetType(),
                         "ResignationSavedScript",
                         $"showUserSavedMessage('Success', '{safeMsg}');",
                         true
                     );
                    bool emailSent = SendResignationEmail(resignationId, action, remark, lastWorkingDate);
                    if (emailSent)
                    {
                        ScriptManager.RegisterStartupScript(
                            this,
                            GetType(),
                            "ResignationEmailSuccess",
                            "showUserSavedMessage('Success', 'Resignation updated and mail sent successfully.');",
                            true
                        );
                    }
                    else
                    {
                        ScriptManager.RegisterStartupScript(
                            this,
                            GetType(),
                            "ResignationEmailWarning",
                            "showUserSavedMessage('Warning', 'Resignation updated, but email was not sent. Please check mail settings/error log.');",
                            true
                        );
                    }
                }
                else
                {
                    string safeErr = HttpUtility.JavaScriptStringEncode(result?.ResponseMsg ?? "Update failed from stored procedure.");
                    ScriptManager.RegisterStartupScript(
                         this,
                         GetType(),
                         "ResignationSavedScript",
                         $"showUserSavedMessage('Error', '{safeErr}');",
                         true
                     );
                }
            }
            catch (Exception ex)
            {
                ScriptManager.RegisterStartupScript(this, GetType(), "error", $"alert('Error: {ex.Message}');", true);
            }

            BindResignationGrid();
            // Close modal after showing message
            ScriptManager.RegisterStartupScript(this, GetType(), "closeModal", "closeResignationModal(); resetResignationFields();", true);
        }


        private bool SendResignationEmail(int resignationId, string action, string remark, DateTime? lastWorkingDate)
        {
            try
            {
                var resignations = Session["ResignationListData"] as List<ResignationDO>;
                if (resignations == null)
                {
                    int reportingManagerId = Convert.ToInt32(Session["userId"]);
                    resignations = new HandoverprocessBL().GetEmployeeResignationDetails(reportingManagerId);
                }
                var resignation = resignations.FirstOrDefault(r => r.EmployeeResignationId == resignationId);

                if (resignation == null)
                {
                    CommonBL errorlog = new CommonBL();
                    errorlog.fnStoreErrorLog(
                        "ResignationList",
                        "SendResignationEmail",
                        "Resignation record not found for id: " + resignationId,
                        UserId
                    );
                    return false;
                }

                if (string.IsNullOrWhiteSpace(resignation.EmployeeEmail))
                {
                    CommonBL errorlog = new CommonBL();
                    errorlog.fnStoreErrorLog(
                        "ResignationList",
                        "SendResignationEmail",
                        "Employee email missing for resignation id: " + resignationId + ", user id: " + resignation.UserId,
                        UserId
                    );
                    return false;
                }

                string employeeEmail = resignation.EmployeeEmail;
                string employeeName = resignation.EmployeeName;

                string subject = $"Your Resignation has been {action}";

                string statusColor = action.Equals("Accepted", StringComparison.OrdinalIgnoreCase) ? "#28a745" : "#dc3545";

                string body = $@"
        <div style='font-family: Arial, sans-serif; line-height:1.6; color:#333;'>
            <h2 style='color:{statusColor};'>Your Resignation has been {action}</h2>
            <p>Dear <strong>{employeeName}</strong>,</p>";

                if (action.Equals("Accepted", StringComparison.OrdinalIgnoreCase))
                {
                    body += $@"
            <p>Your resignation has been <strong style='color:{statusColor};'>accepted</strong> by HR.</p>
            <p>Your last working date is: <strong>{lastWorkingDate?.ToString("dd-MMM-yyyy")}</strong></p>";
                }
                else if (action.Equals("Rejected", StringComparison.OrdinalIgnoreCase))
                {
                    body += $@"
            <p>Your resignation has been <strong style='color:{statusColor};'>rejected</strong> by HR.</p>
            <p>HR Remark: <strong>{remark}</strong></p>";
                }

                body += @"
            <hr style='border:none; border-top:1px solid #ccc;'/>
            <p>Regards,<br/>HR Team</p>
        </div>";

                string Email = ConfigurationManager.AppSettings["SenderEmail"];
                string Password = ConfigurationManager.AppSettings["SenderPassword"];
                int Port = Convert.ToInt32(ConfigurationManager.AppSettings["SenderPort"]);
                string Host = ConfigurationManager.AppSettings["SenderHost"];

                using (MailMessage mail = new MailMessage())
                {
                    mail.From = new MailAddress(Email, "HRMS System");
                    mail.To.Add(employeeEmail);
                    mail.Subject = subject;
                    mail.Body = body;
                    mail.IsBodyHtml = true;

                    byte[] pdfBytes = null;
                    try
                    {
                        pdfBytes = GenerateResignationPdf(
                            resignation,
                            action,
                            remark,
                            employeeName
                        );
                    }
                    catch (Exception pdfEx)
                    {
                        CommonBL errorlog = new CommonBL();
                        errorlog.fnStoreErrorLog(
                            "ResignationList",
                            "SendResignationEmail",
                            "PDF generation failed: " + pdfEx.Message + " | StackTrace=" + pdfEx.StackTrace,
                            UserId
                        );
                    }

                    if (pdfBytes != null && pdfBytes.Length > 0)
                    {
                        string fileName = "Resignation_" + resignation.EmployeeResignationId + ".pdf";
                        var stream = new MemoryStream(pdfBytes);
                        var attachment = new Attachment(stream, fileName, MediaTypeNames.Application.Pdf);
                        mail.Attachments.Add(attachment);
                    }

                    using (SmtpClient smtp = new SmtpClient(Host, Port))
                    {
                        smtp.UseDefaultCredentials = false;
                        smtp.Credentials = new NetworkCredential(Email, Password);
                        smtp.EnableSsl = true;
                        smtp.Send(mail);
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                CommonBL errorlog = new CommonBL();
                string innerError = ex.InnerException != null
                    ? (" | InnerException: " + ex.InnerException.Message + " | InnerStack=" + ex.InnerException.StackTrace)
                    : string.Empty;
                errorlog.fnStoreErrorLog(
                    "ResignationList",
                    "SendResignationEmail",
                    "Email Error: " + ex.Message + " | StackTrace=" + ex.StackTrace + innerError,
                    UserId
                );
                ScriptManager.RegisterStartupScript(this, GetType(), "emailError", $"console.error('Email Error: {ex.Message}');", true);
                return false;
            }
        }

        private byte[] GenerateResignationPdf(ResignationDO res, string status, string hrRemarks, string employeeName)
        {
            // Make HTML template (with logo/header) the primary generator for consistent styling.
            byte[] htmlPdf = GenerateHtmlTemplatePdf(res, status, hrRemarks, employeeName);
            if (htmlPdf != null && htmlPdf.Length > 0)
            {
                return htmlPdf;
            }

            try
            {
                using (var ms = new MemoryStream())
                {
                    // Explicitly initialize iText crypto factory before writer creation.
                    // In some runtime environments this prevents Unknown PdfException at PdfWriter ctor.
                    try
                    {
                        var bcFactoryType = Type.GetType("iText.Bouncycastleconnector.BouncyCastleFactoryCreator, itext.bouncy-castle-connector", throwOnError: false);
                        if (bcFactoryType != null)
                        {
                            var getFactoryMethod = bcFactoryType.GetMethod("GetFactory");
                            if (getFactoryMethod != null)
                            {
                                getFactoryMethod.Invoke(null, null);
                            }
                        }
                    }
                    catch
                    {
                        // Ignore here; normal flow below will still try iText and fallback if needed.
                    }

                    // Force smart mode OFF to avoid SmartModePdfObjectsSerializer runtime issues
                    // seen with certain iText + crypto adapter combinations.
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

                    string resignationDateStr = res.resignation_date == DateTime.MinValue ? "-" : res.resignation_date.ToString("dd-MM-yyyy");
                    string lwdStr = res.last_working_date == DateTime.MinValue ? "-" : res.last_working_date.ToString("dd-MM-yyyy");

                    document.Add(new Paragraph("Resignation " + status + " Letter").SetFont(boldFont).SetFontSize(14).SetMarginBottom(14));
                    document.Add(new Paragraph("Dear " + employeeName + ",").SetFont(normalFont).SetFontSize(11).SetMarginBottom(12));
                    string actionVerb = status.Equals("Accepted", StringComparison.OrdinalIgnoreCase) ? "accept" :
                        (status.Equals("Rejected", StringComparison.OrdinalIgnoreCase) ? "reject" : status.ToLower());

                    document.Add(new Paragraph("This is to acknowledge and " + actionVerb + " your resignation dated " + resignationDateStr + ".")
                        .SetFont(normalFont).SetFontSize(11).SetMarginBottom(8));
                    document.Add(new Paragraph("As per Alphonsol Pvt. Ltd. company policy, you are required to serve a notice period of " + res.notice_period_days + " days. Accordingly, your last working day will be " + lwdStr + ".")
                        .SetFont(normalFont).SetFontSize(11).SetMarginBottom(14));
                    document.Add(new Paragraph("For a smooth handover and exit process, you are requested to complete the following on or before your last working day:")
                        .SetFont(normalFont).SetFontSize(11).SetMarginBottom(8));

                    iText.Layout.Element.List list = new iText.Layout.Element.List().SetListSymbol("• ").SetFont(normalFont).SetFontSize(11).SetMarginBottom(14);
                    list.Add(new iText.Layout.Element.ListItem("Return company laptop and charger"));
                    list.Add(new iText.Layout.Element.ListItem("Submit your company ID card"));
                    list.Add(new iText.Layout.Element.ListItem("Provide a backup of all official data in a pen drive"));
                    document.Add(list);

                    if (!string.IsNullOrWhiteSpace(hrRemarks))
                    {
                        document.Add(new Paragraph("HR Remarks").SetFont(boldFont).SetFontSize(11).SetMarginBottom(4));
                        document.Add(new Paragraph(hrRemarks).SetFont(normalFont).SetFontSize(11).SetMarginBottom(12));
                    }

                    document.Add(new Paragraph("Please ensure proper handover of your responsibilities during the notice period.")
                        .SetFont(normalFont).SetFontSize(11).SetMarginBottom(6));
                    document.Add(new Paragraph("If you have any queries, please feel free to reach out to me.")
                        .SetFont(normalFont).SetFontSize(11).SetMarginBottom(10));
                    document.Add(new Paragraph("We wish you all the best for your future endeavors.")
                        .SetFont(normalFont).SetFontSize(11).SetMarginBottom(20));

                    document.Add(new Paragraph("Regards,").SetFont(normalFont).SetFontSize(11).SetMarginBottom(0));
                    document.Add(new Paragraph("HR Team").SetFont(boldFont).SetFontSize(11).SetMarginBottom(0));
                    document.Add(new Paragraph("Alphonsol Pvt. Ltd.").SetFont(normalFont).SetFontSize(10).SetMarginBottom(0));

                    document.Close();
                    return ms.ToArray();
                }
            }
            catch
            {
                return GeneratePlainPdfFallback(res, status, hrRemarks, employeeName);
            }
        }

        private byte[] GenerateHtmlTemplatePdf(ResignationDO res, string status, string hrRemarks, string employeeName)
        {
            try
            {
                string resignationDateStr = res.resignation_date == DateTime.MinValue ? "-" : res.resignation_date.ToString("dd-MM-yyyy");
                string lwdStr = res.last_working_date == DateTime.MinValue ? "-" : res.last_working_date.ToString("dd-MM-yyyy");
                string actionVerb = status.Equals("Accepted", StringComparison.OrdinalIgnoreCase) ? "accept" :
                    (status.Equals("Rejected", StringComparison.OrdinalIgnoreCase) ? "reject" : status.ToLower());

                string logoHtml = string.Empty;
                string logoPath = Server.MapPath("~/assets/images/alphonsol_logo.png");
                if (File.Exists(logoPath))
                {
                    string base64 = Convert.ToBase64String(File.ReadAllBytes(logoPath));
                    logoHtml = "<img src='data:image/png;base64," + base64 + "' style='width:240px;display:block;' />";
                }

                string safeName = HttpUtility.HtmlEncode(employeeName ?? string.Empty);
                string safeRemark = HttpUtility.HtmlEncode(hrRemarks ?? string.Empty);

                string remarksBlock = string.IsNullOrWhiteSpace(hrRemarks)
                    ? string.Empty
                    : "<p style='margin:0 0 10px 0;'><b>HR Remarks:</b> " + safeRemark + "</p>";

                string html = @"
<!DOCTYPE html>
<html>
<head>
<meta charset='utf-8' />
<style>
body{font-family:Calibri,Arial,sans-serif;font-size:13px;color:#111;line-height:1.45;margin:0;padding:0;}
.page{padding:24px 28px;}
.header{width:100%;border-collapse:collapse;}
.left{width:42%;vertical-align:bottom;}
.right{width:58%;text-align:right;vertical-align:top;font-size:13px;font-weight:600;color:#4a4a4a;}
.cin{margin-top:6px;font-size:12px;color:#444;}
.sep{height:2px;background:#f28c28;border:0;margin:12px 0 20px 0;}
p{margin:0 0 10px 0;}
ul{margin:4px 0 14px 18px;padding:0;}
</style>
</head>
<body>
<div class='page'>
  <table class='header'>
    <tr>
      <td class='left'>" + logoHtml + @"<div class='cin'>CIN No - U72200MH2022PTC381560</div></td>
      <td class='right'>
        High-street Corporate Center, FB-03, Kapurbawdi Junction, Thane(W)-400601<br/>
        Contact No - 9920393999<br/>
        Email Address - support@alphonsol.com<br/>
        Website - www.alphonsol.com
      </td>
    </tr>
  </table>
  <hr class='sep' />
  <p>Dear " + safeName + @",</p>
  <p>This is to acknowledge and " + actionVerb + @" your resignation dated <b>" + resignationDateStr + @"</b>.</p>
  <p>As per Alphonsol Pvt. Ltd. company policy, you are required to serve a notice period of <b>" + res.notice_period_days + @" days</b>. Accordingly, your last working day will be <b>" + lwdStr + @"</b>.</p>
  <p>For a smooth handover and exit process, you are requested to complete the following on or before your last working day:</p>
  <ul>
    <li>Return the company laptop and charger</li>
    <li>Submit your company ID card</li>
    <li>Provide a backup of all official data in a pen drive</li>
  </ul>
  <p>Please ensure proper handover of your responsibilities during the notice period.</p>
  <p>If you have any queries, please feel free to reach out to me.</p>
  " + remarksBlock + @"
  <p>We wish you all the best for your future endeavors.</p>
</div>
</body>
</html>";

                using (var ms = new MemoryStream())
                {
                    HtmlConverter.ConvertToPdf(html, ms);
                    return ms.ToArray();
                }
            }
            catch
            {
                return GeneratePlainPdfFallback(res, status, hrRemarks, employeeName);
            }
        }

        private byte[] GeneratePlainPdfFallback(ResignationDO res, string status, string hrRemarks, string employeeName)
        {
            string resignationDateStr = res.resignation_date == DateTime.MinValue ? "-" : res.resignation_date.ToString("dd-MM-yyyy");
            string lwdStr = res.last_working_date == DateTime.MinValue ? "-" : res.last_working_date.ToString("dd-MM-yyyy");
            string actionVerb = status.Equals("Accepted", StringComparison.OrdinalIgnoreCase) ? "accept" :
                (status.Equals("Rejected", StringComparison.OrdinalIgnoreCase) ? "reject" : status.ToLower());

            string logoPath = Server.MapPath("~/assets/images/alphonsol_logo.png");
            byte[] logoJpegBytes = null;
            int logoW = 0, logoH = 0;
            if (File.Exists(logoPath))
            {
                using (var source = new Bitmap(logoPath))
                using (var bmp = new Bitmap(source.Width, source.Height))
                using (var ms = new MemoryStream())
                {
                    using (var g = Graphics.FromImage(bmp))
                    {
                        g.CompositingQuality = CompositingQuality.HighQuality;
                        g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                        g.SmoothingMode = SmoothingMode.HighQuality;
                        g.DrawImage(source, 0, 0, source.Width, source.Height);
                    }

                    var jpgEncoder = ImageCodecInfo.GetImageDecoders().FirstOrDefault(c => c.FormatID == ImageFormat.Jpeg.Guid);
                    if (jpgEncoder != null)
                    {
                        var encoderParams = new EncoderParameters(1);
                        encoderParams.Param[0] = new EncoderParameter(System.Drawing.Imaging.Encoder.Quality, 100L);
                        bmp.Save(ms, jpgEncoder, encoderParams);
                    }
                    else
                    {
                        bmp.Save(ms, ImageFormat.Jpeg);
                    }
                    logoJpegBytes = ms.ToArray();
                    logoW = bmp.Width;
                    logoH = bmp.Height;
                }
            }

            var textLines = new List<string>
            {
                "High-street Corporate Center, FB-03,",
                "Kapurbawdi Junction, Thane(W)-400601",
                "Contact No - 9920393999",
                "Email Address - support@alphonsol.com",
                "Website - www.alphonsol.com",
                "",
                "CIN No - U72200MH2022PTC381560",
                "",
                "Dear " + employeeName + ",",
                "",
                "This is to acknowledge and " + actionVerb + " your resignation dated " + resignationDateStr + ".",
                "As per Alphonsol Pvt. Ltd. company policy, you are required to serve a notice period of " + res.notice_period_days + " days.",
                "Accordingly, your last working day will be " + lwdStr + ".",
                "",
                "For a smooth handover and exit process, you are requested to complete the following",
                "on or before your last working day:",
                "  - Return the company laptop and charger",
                "  - Submit your company ID card",
                "  - Provide a backup of all official data in a pen drive",
                "",
                "Please ensure proper handover of your responsibilities during the notice period.",
                "If you have any queries, please feel free to reach out to me."
            };

            if (!string.IsNullOrWhiteSpace(hrRemarks))
            {
                textLines.Add("");
                textLines.Add("HR Remarks: " + hrRemarks);
            }
            textLines.Add("");
            textLines.Add("We wish you all the best for your future endeavors.");

            var contentBuilder = new StringBuilder();
            contentBuilder.Append("q 1 1 1 rg 0 0 595 842 re f Q\n");
            contentBuilder.Append("0.95 0.55 0.16 RG 2 w 0 706 m 595 706 l S\n");

            if (logoJpegBytes != null && logoJpegBytes.Length > 0 && logoW > 0 && logoH > 0)
            {
                float drawW = 190f;
                float drawH = (logoH * drawW) / logoW;
                contentBuilder.AppendFormat(System.Globalization.CultureInfo.InvariantCulture,
                    "q {0} 0 0 {1} 28 720 cm /Im1 Do Q\n", drawW, drawH);
            }
            else
            {
                contentBuilder.Append("BT /F2 24 Tf 28 748 Td (Alphonsol Pvt. Ltd.) Tj ET\n");
            }

            int y = 800;
            for (int i = 0; i < 4; i++)
            {
                contentBuilder.AppendFormat("BT /F1 9 Tf 255 {0} Td ({1}) Tj ET\n", y, EscapePdfText(textLines[i]));
                y -= 14;
            }

            y = 684;
            for (int i = 5; i < textLines.Count; i++)
            {
                string line = textLines[i];
                if (string.IsNullOrEmpty(line))
                {
                    y -= 10;
                    continue;
                }
                contentBuilder.AppendFormat("BT /F1 12 Tf 48 {0} Td ({1}) Tj ET\n", y, EscapePdfText(line));
                y -= 18;
            }

            byte[] contentBytes = Encoding.ASCII.GetBytes(contentBuilder.ToString());

            string obj1 = "1 0 obj << /Type /Catalog /Pages 2 0 R >> endobj\n";
            string obj2 = "2 0 obj << /Type /Pages /Kids [3 0 R] /Count 1 >> endobj\n";
            string obj3 = logoJpegBytes != null && logoJpegBytes.Length > 0
                ? "3 0 obj << /Type /Page /Parent 2 0 R /MediaBox [0 0 595 842] /Resources << /Font << /F1 4 0 R /F2 7 0 R >> /XObject << /Im1 5 0 R >> >> /Contents 6 0 R >> endobj\n"
                : "3 0 obj << /Type /Page /Parent 2 0 R /MediaBox [0 0 595 842] /Resources << /Font << /F1 4 0 R /F2 7 0 R >> >> /Contents 6 0 R >> endobj\n";
            string obj4 = "4 0 obj << /Type /Font /Subtype /Type1 /BaseFont /Helvetica >> endobj\n";
            string obj7 = "7 0 obj << /Type /Font /Subtype /Type1 /BaseFont /Helvetica-Bold >> endobj\n";

            byte[] obj1b = Encoding.ASCII.GetBytes(obj1);
            byte[] obj2b = Encoding.ASCII.GetBytes(obj2);
            byte[] obj3b = Encoding.ASCII.GetBytes(obj3);
            byte[] obj4b = Encoding.ASCII.GetBytes(obj4);
            byte[] obj7b = Encoding.ASCII.GetBytes(obj7);

            byte[] obj5b = null;
            if (logoJpegBytes != null && logoJpegBytes.Length > 0)
            {
                string imgHeader = "5 0 obj << /Type /XObject /Subtype /Image /Width " + logoW +
                    " /Height " + logoH +
                    " /ColorSpace /DeviceRGB /BitsPerComponent 8 /Filter /DCTDecode /Length " + logoJpegBytes.Length + " >> stream\n";
                string imgFooter = "\nendstream endobj\n";
                using (var ms = new MemoryStream())
                {
                    ms.Write(Encoding.ASCII.GetBytes(imgHeader), 0, Encoding.ASCII.GetByteCount(imgHeader));
                    ms.Write(logoJpegBytes, 0, logoJpegBytes.Length);
                    ms.Write(Encoding.ASCII.GetBytes(imgFooter), 0, Encoding.ASCII.GetByteCount(imgFooter));
                    obj5b = ms.ToArray();
                }
            }
            else
            {
                obj5b = Encoding.ASCII.GetBytes("5 0 obj <<>> endobj\n");
            }

            string contentHeader = "6 0 obj << /Length " + contentBytes.Length + " >> stream\n";
            string contentFooter = "\nendstream endobj\n";
            byte[] obj6b;
            using (var ms = new MemoryStream())
            {
                ms.Write(Encoding.ASCII.GetBytes(contentHeader), 0, Encoding.ASCII.GetByteCount(contentHeader));
                ms.Write(contentBytes, 0, contentBytes.Length);
                ms.Write(Encoding.ASCII.GetBytes(contentFooter), 0, Encoding.ASCII.GetByteCount(contentFooter));
                obj6b = ms.ToArray();
            }

            using (var pdfMs = new MemoryStream())
            {
                void WriteAscii(string s)
                {
                    byte[] b = Encoding.ASCII.GetBytes(s);
                    pdfMs.Write(b, 0, b.Length);
                }

                WriteAscii("%PDF-1.4\n");
                var offsets = new List<long> { 0 };

                offsets.Add(pdfMs.Position); pdfMs.Write(obj1b, 0, obj1b.Length);
                offsets.Add(pdfMs.Position); pdfMs.Write(obj2b, 0, obj2b.Length);
                offsets.Add(pdfMs.Position); pdfMs.Write(obj3b, 0, obj3b.Length);
                offsets.Add(pdfMs.Position); pdfMs.Write(obj4b, 0, obj4b.Length);
                offsets.Add(pdfMs.Position); pdfMs.Write(obj5b, 0, obj5b.Length);
                offsets.Add(pdfMs.Position); pdfMs.Write(obj6b, 0, obj6b.Length);
                offsets.Add(pdfMs.Position); pdfMs.Write(obj7b, 0, obj7b.Length);

                long xrefPos = pdfMs.Position;
                WriteAscii("xref\n0 8\n");
                WriteAscii("0000000000 65535 f \n");
                for (int i = 1; i <= 7; i++)
                {
                    WriteAscii(offsets[i].ToString("D10") + " 00000 n \n");
                }
                WriteAscii("trailer << /Size 8 /Root 1 0 R >>\n");
                WriteAscii("startxref\n");
                WriteAscii(xrefPos.ToString());
                WriteAscii("\n%%EOF");

                return pdfMs.ToArray();
            }
        }

        private string EscapePdfText(string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                return string.Empty;
            }
            return value.Replace("\\", "\\\\").Replace("(", "\\(").Replace(")", "\\)");
        }

        public int TotalRecordCount()
        {
            var resignations = Session["ResignationListData"] as List<ResignationDO>;
            if (resignations == null)
            {
                int reportingManagerId = Convert.ToInt32(Session["userId"]);
                resignations = new HandoverprocessBL().GetEmployeeResignationDetails(reportingManagerId);
            }
            return resignations.Count;
        }
        protected void ddlPageSelector_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                int selectedPageIndex = Convert.ToInt32(ddlPageSelector.SelectedValue);
                Session["CurrentPageIndex"] = selectedPageIndex;

                if (Session["AdvSearchResViewUser"] != null)
                {
                    List<ResignationDO> searchResults = (List<ResignationDO>)Session["AdvSearchResViewUser"];
                    //searchResults = searchResults.OrderByDescending(t => t.Inserteddate).ToList();
                    ApplySorting(ref searchResults);

                    int totalRecords = searchResults.Count;
                    int pageIndex = selectedPageIndex;
                    hfPageIndexViewUser.Value = pageIndex.ToString();

                    int pageSize = gvResignations.PageSize;
                    int startRowIndex = pageIndex * pageSize;
                    int endRowIndex = Math.Min(startRowIndex + pageSize, totalRecords);

                    List<ResignationDO> displayedUsers = searchResults.GetRange(startRowIndex, endRowIndex - startRowIndex);
                    gvResignations.DataSource = displayedUsers;
                    gvResignations.DataBind();

                    UpdatePageInfoLabel(pageIndex, totalRecords);
                }
                else
                {
                    int companyId = Convert.ToInt32(Session["SelectedCompanyId"]);
                    BindResignationGrid();

                }
            }
            catch (Exception ex)
            {
                CommonBL errorlog = new CommonBL();
                errorlog.fnStoreErrorLog("ResignationList", "ddlPageSelector_SelectedIndexChanged", "Exception Message" + ex.Message + "Strace=" + ex.StackTrace, UserId);
            }
        }
        protected void OnPageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            gvResignations.PageIndex = e.NewPageIndex;
            BindResignationGrid();
        }
        protected void gridview_Sorting(object sender, GridViewSortEventArgs e)
        {
            try
            {
                List<ResignationDO> createdet = Session["ResignationListData"] as List<ResignationDO>;
                if (createdet == null)
                {
                    int reportingManagerId = Convert.ToInt32(Session["userId"]);
                    createdet = new HandoverprocessBL().GetEmployeeResignationDetails(reportingManagerId);
                }

                if (createdet != null)
                {
                    string sortExpression = e.SortExpression;
                    string sortDirection = GetSortDirection(sortExpression);

                    if (sortDirection == "ASC")
                    {
                        createdet = createdet.OrderBy(p => p.GetType().GetProperty(sortExpression).GetValue(p, null)).ToList();
                    }
                    else
                    {
                        createdet = createdet.OrderByDescending(p => p.GetType().GetProperty(sortExpression).GetValue(p, null)).ToList();
                    }

                    gvResignations.DataSource = createdet;
                    gvResignations.DataBind();
                }
            }

            catch (Exception ex)
            {
                CommonBL errorlog = new CommonBL();
                errorlog.fnStoreErrorLog("ResignationList", "gridview_Sorting", "Exception Message" + ex.Message + "Strace=" + ex.StackTrace, UserId);
            }
        }
        private string GetSortDirection(string column)
        {


            string sortDirection = "ASC";
            if (ViewState["SortDirection"] != null)
            {
                if (ViewState["SortExpression"].ToString() == column)
                {
                    sortDirection = ViewState["SortDirection"].ToString() == "ASC" ? "DESC" : "ASC";
                }
            }
            ViewState["SortExpression"] = column;
            ViewState["SortDirection"] = sortDirection;
            return sortDirection;

        }
        private void ApplySorting(ref List<ResignationDO> users)
        {
            try
            {
                string sortExpression = ViewState["SortExpression"] as string;
                string sortDirection = ViewState["SortDirection"] as string;

                if (!string.IsNullOrEmpty(sortExpression) && !string.IsNullOrEmpty(sortDirection))
                {
                    if (sortDirection == "ASC")
                    {
                        users = users.OrderBy(p => p.GetType().GetProperty(sortExpression).GetValue(p, null)).ToList();
                    }
                    else
                    {
                        users = users.OrderByDescending(p => p.GetType().GetProperty(sortExpression).GetValue(p, null)).ToList();
                    }
                }
            }

            catch (Exception ex)
            {
                CommonBL errorlog = new CommonBL();
                errorlog.fnStoreErrorLog("ResignationList", "ApplySorting", "Exception Message" + ex.Message + "Strace=" + ex.StackTrace, UserId);
            }
        }
        protected void UpdatePageInfoLabel(int pageIndex, int pagecount)
        {
            try
            {
                int currentPage = pageIndex + 1;
                int totalPages = (int)Math.Ceiling((double)pagecount / 10);
                ddlPageSelector.Items.Clear();
                for (int i = 1; i <= totalPages; i++)
                {
                    ddlPageSelector.Items.Add(new System.Web.UI.WebControls.ListItem($"{i}/{totalPages}", (i - 1).ToString()));
                }
                if (ddlPageSelector.Items.Count > 0)
                {
                    ddlPageSelector.SelectedValue = pageIndex.ToString();
                }
            }
            catch (Exception ex)
            {
                CommonBL errorlog = new CommonBL();
                errorlog.fnStoreErrorLog("ResignationList", "UpdatePageInfoLabel", "Exception Message" + ex.Message + "Strace=" + ex.StackTrace, UserId);
            }
        }

        protected void gvUsers_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            try
            {
                int resignationId = Convert.ToInt32(e.CommandArgument);
                if (resignationId <= 0)
                {
                    ScriptManager.RegisterStartupScript(
                        this, GetType(),
                        "noResignation",
                        "showUserSavedMessage('Error', 'No resignation request found for this user.');", true);
                    return;
                }
                hfResignationId.Value = resignationId.ToString();

                var resignations = Session["ResignationListData"] as List<ResignationDO>;
                if (resignations != null)
                {
                    var selected = resignations.FirstOrDefault(x => x.EmployeeResignationId == resignationId);
                    bool isManagerAuthority = selected != null &&
                        string.Equals(Convert.ToString(selected.authority_status), "Manager Authority", StringComparison.OrdinalIgnoreCase);
                    bool alreadyActionTaken = selected != null &&
                        (string.Equals(Convert.ToString(selected.hr_status), "Accepted", StringComparison.OrdinalIgnoreCase)
                        || string.Equals(Convert.ToString(selected.hr_status), "Rejected", StringComparison.OrdinalIgnoreCase)
                        || string.Equals(Convert.ToString(selected.hr_status), "Accept", StringComparison.OrdinalIgnoreCase)
                        || string.Equals(Convert.ToString(selected.hr_status), "Reject", StringComparison.OrdinalIgnoreCase));

                    if (alreadyActionTaken)
                    {
                        ScriptManager.RegisterStartupScript(
                            this, GetType(),
                            "alreadyProcessed",
                            "showUserSavedMessage('Error', 'Action already completed for this resignation.');", true);
                        return;
                    }

                    if (selected != null && selected.status_updated_flag == 0)
                    {
                        ScriptManager.RegisterStartupScript(
                            this, GetType(),
                            "managerAuthorityOnly",
                            "showUserSavedMessage('Error', 'Manager Authority pending. HR action is disabled.');", true);
                        return;
                    }
                }

                if (e.CommandName == "Accept")
                {
                    ScriptManager.RegisterStartupScript(
                        this, GetType(),
                        "openModal", "openResignationModal('Accepted');", true);
                }

                if (e.CommandName == "Reject")
                {
                    ScriptManager.RegisterStartupScript(
                        this, GetType(),
                        "openModal", "openResignationModal('Rejected');", true);
                }
            }
            catch (Exception ex)
            {
                CommonBL errorlog = new CommonBL();
                errorlog.fnStoreErrorLog(
                    "ResignationList",
                    "gvUsers_RowCommand",
                    ex.Message,
                    UserId);
            }
        }

        protected bool CanHrTakeAction(object statusFlagObj)
        {
            int flag = 0;
            if (statusFlagObj != null)
            {
                int.TryParse(Convert.ToString(statusFlagObj), out flag);
            }
            return flag == 1;
        }

        protected string GetAuthorityStatusText(object statusFlagObj)
        {
            int flag = 0;
            if (statusFlagObj != null)
            {
                int.TryParse(Convert.ToString(statusFlagObj), out flag);
            }
            return flag == 1 ? "HR Authority" : "Manager Authority";
        }

        protected string GetAuthorityBadgeClass(object statusFlagObj)
        {
            int flag = 0;
            if (statusFlagObj != null)
            {
                int.TryParse(Convert.ToString(statusFlagObj), out flag);
            }
            return flag == 1 ? "bg-primary" : "bg-warning text-dark";
        }



    }
}
