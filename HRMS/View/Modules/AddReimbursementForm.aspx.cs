using DataObject;
using ProcessModel;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace HRMS.View.Modules
{
    public partial class AddReimbursementForm : System.Web.UI.Page
    {
        protected string UserId = null;

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

                // Bind status dropdown from backend
                BindStatusDropdown();

                // Bind month dropdown from backend
                BindMonthDropdown();

                // Check mode
                string mode = Convert.ToString(Session["Reimbursement_Mode"]);
                bool isViewMode = string.Equals(mode, "View", StringComparison.OrdinalIgnoreCase);
                bool isEditMode = string.Equals(mode, "Edit", StringComparison.OrdinalIgnoreCase);

                // Check if editing or viewing existing reimbursement
                if (Session["Reimbursement_ID"] != null)
                {
                    string reimbursementNumber = Convert.ToString(Session["Reimbursement_ID"]);
                    hdnReimbursementId.Value = reimbursementNumber;

                    if (isViewMode)
                    {
                        hdnIsView.Value = "1";
                        lblPageTitle.Text = "View Reimbursement";
                    }
                    else if (isEditMode)
                    {
                        hdnIsEdit.Value = "1";
                        lblPageTitle.Text = "Edit Reimbursement";
                        btnSave.Text = "Update Reimbursement";
                    }
                    else
                    {
                        hdnIsEdit.Value = "1";
                        lblPageTitle.Text = "Edit Reimbursement";
                        btnSave.Text = "Update Reimbursement";
                    }

                    LoadReimbursementDetails(reimbursementNumber);
                    BindReimbursementDocuments();

                    if (isViewMode)
                    {
                        // Make fields read-only in view mode
                        MakeFieldsReadOnly();
                    }
                }
                else
                {
                    hdnReimbursementId.Value = "0";
                    hdnIsEdit.Value = "0";
                    hdnIsView.Value = "0";
                    lblPageTitle.Text = "Add New Reimbursement";
                    btnSave.Text = "Save Reimbursement";
                    
                    // Generate new reimbursement number
                    txtReimbursementNumber.Text = GenerateReimbursementNumber();
                }
            }
        }

        private void BindStatusDropdown()
        {
            try
            {
                ReimbursementBL bl = new ReimbursementBL();
                var statusOptions = bl.GetStatusOptions();

                ddlStatus.Items.Clear();
                ddlStatus.Items.Add(new ListItem("-- Please Select --", "0"));

                foreach (var option in statusOptions)
                {
                    ddlStatus.Items.Add(new ListItem(option.Text, option.Id.ToString()));
                }

                ddlStatus.SelectedValue = "0";
            }
            catch (Exception ex)
            {
                ClientScript.RegisterStartupScript(
                    GetType(),
                    "error",
                    $"Swal.fire('Error','{HttpUtility.JavaScriptStringEncode(ex.Message)}','error');",
                    true);
            }
        }

        private void BindMonthDropdown()
        {
            try
            {
                ReimbursementBL bl = new ReimbursementBL();
                var monthOptions = bl.GetMonthOptions();

                ddlMonth.Items.Clear();
                ddlMonth.Items.Add(new ListItem("-- Please Select --", "0"));

                foreach (var option in monthOptions)
                {
                    ddlMonth.Items.Add(new ListItem(option.Text, option.Value.ToString()));
                }

                // Set current month as default
                ddlMonth.SelectedValue = DateTime.Now.Month.ToString();
            }
            catch (Exception ex)
            {
                ClientScript.RegisterStartupScript(
                    GetType(),
                    "error",
                    $"Swal.fire('Error','{HttpUtility.JavaScriptStringEncode(ex.Message)}','error');",
                    true);
            }
        }

        private string GenerateReimbursementNumber()
        {
            return "REIM" + DateTime.Now.ToString("yyyyMMddHHmmss");
        }

        private void LoadReimbursementDetails(string reimbursementNumber)
        {
            try
            {
                ReimbursementBL bl = new ReimbursementBL();
                ReimbursementDetailsDO details = bl.GetReimbursementDetailsById(reimbursementNumber);

                if (details != null)
                {
                    txtReimbursementNumber.Text = details.reimbursementNumber ?? "";
                    txtClaimType.Text = details.claimType ?? "";
                    txtClaimDate.Text = details.claimDate.ToString("dd-MMM-yyyy");
                    txtPaymentMonth.Text = details.paymentMonth ?? "";
                    txtClaimAmount.Text = details.claimAmount.ToString("N2");
                    txtStatus.Text = details.status ?? "";
                    
                    // Set dropdown value by matching the status text
                    var statusOptions = bl.GetStatusOptions();
                    var matchedOption = statusOptions.FirstOrDefault(o => o.Value == details.status);
                    if (matchedOption != null)
                    {
                        ddlStatus.SelectedValue = matchedOption.Id.ToString();
                    }
                    
                    txtRemarks.Text = details.remarks ?? "";
                    txtDocument.Text = GetDocumentDisplayName(details.document ?? "");
                    hdnDocumentData.Value = details.document ?? "";

                    // Set month dropdown based on claim date
                    if (details.claimDate != DateTime.MinValue)
                    {
                        ddlMonth.SelectedValue = details.claimDate.Month.ToString();
                    }
                }
            }
            catch (Exception ex)
            {
                ClientScript.RegisterStartupScript(
                    GetType(),
                    "error",
                    $"Swal.fire('Error','{HttpUtility.JavaScriptStringEncode(ex.Message)}','error');",
                    true);
            }
        }

        // Lists documents uploaded against the reimbursement flow for the current user,
        // via ReimbursementBL.GetReimbursementDocuments -> sp_getEmpReimbursementDocuments.
        private void BindReimbursementDocuments()
        {
            try
            {
                int userId = Convert.ToInt32(Session["userId"]);
                string reimbureseNo = txtReimbursementNumber.Text.ToString();
                ReimbursementBL bl = new ReimbursementBL();
                List<ReimbursementDocumentDO> documents = bl.GetReimbursementDocuments(userId, reimbureseNo) ?? new List<ReimbursementDocumentDO>();

                rptReimbursementDocuments.DataSource = documents;
                rptReimbursementDocuments.DataBind();

                rptReimbursementDocuments.Visible = documents.Count > 0;
                lblNoReimbursementDocuments.Visible = documents.Count == 0;
            }
            catch (Exception ex)
            {
                ClientScript.RegisterStartupScript(
                    GetType(),
                    "error",
                    $"Swal.fire('Error','{HttpUtility.JavaScriptStringEncode(ex.Message)}','error');",
                    true);
            }
        }

        protected void rptReimbursementDocuments_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            if (!string.Equals(e.CommandName, "DownloadReimbursementDocument", StringComparison.OrdinalIgnoreCase))
            {
                return;
            }

            try
            {
                int userDocDetId;
                if (!int.TryParse(Convert.ToString(e.CommandArgument), out userDocDetId))
                {
                    return;
                }

                int userId = Convert.ToInt32(Session["userId"]);
                string reimbureseNo = txtReimbursementNumber.Text.ToString();
                ReimbursementBL bl = new ReimbursementBL();
                List<ReimbursementDocumentDO> documents = bl.GetReimbursementDocuments(userId, reimbureseNo) ?? new List<ReimbursementDocumentDO>();
                ReimbursementDocumentDO document = documents.FirstOrDefault(d => d.UserDocDetId == userDocDetId);

                if (document == null)
                {
                    ClientScript.RegisterStartupScript(GetType(), "warning", "Swal.fire('Warning','Document not found','warning');", true);
                    return;
                }

                string documentsRoot = ConfigurationManager.AppSettings["EmployeeDocumentServerPath"];
                if (string.IsNullOrWhiteSpace(documentsRoot))
                {
                    return;
                }

                if (documentsRoot.StartsWith("~"))
                {
                    documentsRoot = Server.MapPath(documentsRoot);
                }

                string physicalPath = Path.Combine(documentsRoot, document.filepath ?? string.Empty, (document.FileName ?? string.Empty) + document.FileExtension);

                if (!File.Exists(physicalPath))
                {
                    ClientScript.RegisterStartupScript(GetType(), "warning", "Swal.fire('Warning','File not found on server','warning');", true);
                    return;
                }

                FileInfo fileInfo = new FileInfo(physicalPath);
                Response.Clear();
                Response.ContentType = "application/octet-stream";
                Response.AddHeader("Content-Disposition", "attachment; filename=\"" + fileInfo.Name + "\"");
                Response.AddHeader("Content-Length", fileInfo.Length.ToString());
                Response.TransmitFile(physicalPath);
                Response.Flush();
                Response.End();
            }
            catch (Exception ex)
            {
                ClientScript.RegisterStartupScript(
                    GetType(),
                    "error",
                    $"Swal.fire('Error','{HttpUtility.JavaScriptStringEncode(ex.Message)}','error');",
                    true);
            }
        }

        private string GetDocumentDisplayName(string documentData)
        {
            if (string.IsNullOrEmpty(documentData))
                return "";

            // Handle JSON array format: ["data:image/jpeg;base64,..."]
            string dataToCheck = documentData;
            if (documentData.StartsWith("[") && documentData.EndsWith("]"))
            {
                dataToCheck = documentData.Trim('[', ']', '"');
            }

            // Extract file extension from data URI prefix
            if (dataToCheck.Contains(","))
            {
                string[] parts = dataToCheck.Split(',');
                if (parts.Length > 0)
                {
                    string prefix = parts[0];
                    if (prefix.Contains("application/pdf"))
                        return "document.pdf";
                    else if (prefix.Contains("image/jpeg") || prefix.Contains("image/jpg"))
                        return "document.jpg";
                    else if (prefix.Contains("image/png"))
                        return "document.png";
                    else if (prefix.Contains("image/gif"))
                        return "document.gif";
                    else if (prefix.Contains("image/bmp"))
                        return "document.bmp";
                    else if (prefix.Contains("image/tiff") || prefix.Contains("image/tif"))
                        return "document.tif";
                    else if (prefix.Contains("application/msword"))
                        return "document.doc";
                    else if (prefix.Contains("application/vnd.openxmlformats-officedocument.wordprocessingml.document"))
                        return "document.docx";
                    else if (prefix.Contains("application/vnd.ms-excel"))
                        return "document.xls";
                    else if (prefix.Contains("application/vnd.openxmlformats-officedocument.spreadsheetml.sheet"))
                        return "document.xlsx";
                }
            }

            // Try to detect from magic bytes
            try
            {
                string base64Data = dataToCheck.Contains(",") ? dataToCheck.Split(',')[1] : dataToCheck;
                byte[] fileBytes = Convert.FromBase64String(base64Data);

                if (fileBytes.Length > 4)
                {
                    // PDF: %PDF
                    if (fileBytes[0] == 0x25 && fileBytes[1] == 0x50 && fileBytes[2] == 0x44 && fileBytes[3] == 0x46)
                        return "document.pdf";
                    // JPEG: FF D8 FF
                    else if (fileBytes[0] == 0xFF && fileBytes[1] == 0xD8 && fileBytes[2] == 0xFF)
                        return "document.jpg";
                    // PNG: 89 50 4E 47
                    else if (fileBytes[0] == 0x89 && fileBytes[1] == 0x50 && fileBytes[2] == 0x4E && fileBytes[3] == 0x47)
                        return "document.png";
                    // GIF: GIF8
                    else if (fileBytes[0] == 0x47 && fileBytes[1] == 0x49 && fileBytes[2] == 0x46 && fileBytes[3] == 0x38)
                        return "document.gif";
                    // BMP: BM
                    else if (fileBytes[0] == 0x42 && fileBytes[1] == 0x4D)
                        return "document.bmp";
                }
            }
            catch
            {
                // If conversion fails, return default
            }

            return "document";
        }

        private void MakeFieldsReadOnly()
        {
            txtClaimType.ReadOnly = true;
            txtClaimDate.ReadOnly = true;
            txtPaymentMonth.ReadOnly = true;
            txtClaimAmount.ReadOnly = true;
            txtRemarks.ReadOnly = true;
            txtDocument.ReadOnly = true;
            btnSave.Visible = false;
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                // Debug: Check if event is firing
                System.Diagnostics.Debug.WriteLine("btnSave_Click fired");
                System.Diagnostics.Debug.WriteLine("hdnIsEdit.Value: " + hdnIsEdit.Value);

                ReimbursementBL bl = new ReimbursementBL();
                ResponseDO result;

                if (hdnIsEdit.Value == "1")
                {
                    System.Diagnostics.Debug.WriteLine("Entering Edit mode branch");

                    // Edit mode: Update only status, remarks, and payment month using the new SP
                    int statusId = int.Parse(ddlStatus.SelectedValue);
                    string paymentMonth = ddlMonth.SelectedValue != "0" ? ddlMonth.SelectedItem.Text : txtPaymentMonth.Text;

                    System.Diagnostics.Debug.WriteLine("statusId: " + statusId);
                    System.Diagnostics.Debug.WriteLine("paymentMonth: " + paymentMonth);
                    System.Diagnostics.Debug.WriteLine("reimbursementNumber: " + txtReimbursementNumber.Text);

                    result = bl.UpdateReimbursementStatus(
                        txtReimbursementNumber.Text,
                        statusId,
                        txtRemarks.Text,
                        paymentMonth
                    );

                    System.Diagnostics.Debug.WriteLine("UpdateReimbursementStatus called. Result Status: " + result.Status);
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine("Entering Add mode branch");

                    // Add mode: Save full reimbursement details
                    string status = "pen"; // Default to Pending lookup value

                    ReimbursementDetailsDO reimbursement = new ReimbursementDetailsDO
                    {
                        reimbursementNumber = txtReimbursementNumber.Text,
                        claimType = txtClaimType.Text,
                        claimDate = DateTime.Parse(txtClaimDate.Text),
                        paymentMonth = txtPaymentMonth.Text,
                        claimAmount = decimal.Parse(txtClaimAmount.Text),
                        remarks = txtRemarks.Text,
                        document = txtDocument.Text,
                        status = status
                    };

                    result = bl.SaveReimbursementDetails(reimbursement);
                }

                if (result.Status == 1)
                {
                    ClientScript.RegisterStartupScript(
                        GetType(),
                        "msg",
                        $"Swal.fire('Success','{HttpUtility.JavaScriptStringEncode(result.message)}','success').then(function(){{ window.location.href='ReimbursementDetails.aspx'; }});",
                        true);
                }
                else
                {
                    ClientScript.RegisterStartupScript(
                        GetType(),
                        "msg",
                        $"Swal.fire('Warning','{HttpUtility.JavaScriptStringEncode(result.message)}','warning');",
                        true);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Exception: " + ex.Message);
                System.Diagnostics.Debug.WriteLine("StackTrace: " + ex.StackTrace);
                ClientScript.RegisterStartupScript(
                    GetType(),
                    "error",
                    $"Swal.fire('Error','{HttpUtility.JavaScriptStringEncode(ex.Message)}','error');",
                    true);
            }
        }

        protected void btnCancel_Click(object sender, EventArgs e)
        {
            Response.Redirect("ReimbursementDetails.aspx");
        }

        protected void btnBack_Click(object sender, EventArgs e)
        {
            Response.Redirect("ReimbursementDetails.aspx");
        }

        protected void btnDownloadDocument_Click(object sender, EventArgs e)
        {
            try
            {
                string documentData = hdnDocumentData.Value.Trim();
                string contentType = "application/octet-stream";
                string fileName = "document";

                if (string.IsNullOrEmpty(documentData))
                {
                    ClientScript.RegisterStartupScript(
                        GetType(),
                        "warning",
                        "Swal.fire('Warning','No document specified','warning');",
                        true);
                    return;
                }

                // Check if it's a URL
                if (documentData.StartsWith("http://", StringComparison.OrdinalIgnoreCase) ||
                    documentData.StartsWith("https://", StringComparison.OrdinalIgnoreCase))
                {
                    Response.Redirect(documentData, false);
                    return;
                }

                // Check if it's a physical file path
                if (System.IO.File.Exists(documentData))
                {
                    fileName = System.IO.Path.GetFileName(documentData);

                    Response.Clear();
                    Response.ContentType = contentType;
                    Response.AddHeader("Content-Disposition", $"attachment; filename=\"{fileName}\"");
                    Response.TransmitFile(documentData);
                    Response.End();
                    return;
                }

                // Handle JSON array format: ["data:image/jpeg;base64,..."]
                if (documentData.StartsWith("[") && documentData.EndsWith("]"))
                {
                    // Remove square brackets and quotes
                    documentData = documentData.Trim('[', ']', '"');
                }

                // Handle base64 string
                string base64Data = documentData;

                // Remove data URI prefix if present (e.g., "data:application/pdf;base64,")
                if (documentData.Contains(","))
                {
                    string[] parts = documentData.Split(',');
                    if (parts.Length > 1)
                    {
                        base64Data = parts[1];
                        // Try to extract content type from the prefix
                        string prefix = parts[0];
                        if (prefix.Contains("application/pdf"))
                        {
                            contentType = "application/pdf";
                            fileName = "document.pdf";
                        }
                        else if (prefix.Contains("image/jpeg") || prefix.Contains("image/jpg"))
                        {
                            contentType = "image/jpeg";
                            fileName = "document.jpg";
                        }
                        else if (prefix.Contains("image/png"))
                        {
                            contentType = "image/png";
                            fileName = "document.png";
                        }
                        else if (prefix.Contains("image/gif"))
                        {
                            contentType = "image/gif";
                            fileName = "document.gif";
                        }
                        else if (prefix.Contains("image/bmp"))
                        {
                            contentType = "image/bmp";
                            fileName = "document.bmp";
                        }
                        else if (prefix.Contains("image/tiff") || prefix.Contains("image/tif"))
                        {
                            contentType = "image/tiff";
                            fileName = "document.tif";
                        }
                        else if (prefix.Contains("application/msword"))
                        {
                            contentType = "application/msword";
                            fileName = "document.doc";
                        }
                        else if (prefix.Contains("application/vnd.openxmlformats-officedocument.wordprocessingml.document"))
                        {
                            contentType = "application/vnd.openxmlformats-officedocument.wordprocessingml.document";
                            fileName = "document.docx";
                        }
                        else if (prefix.Contains("application/vnd.ms-excel"))
                        {
                            contentType = "application/vnd.ms-excel";
                            fileName = "document.xls";
                        }
                        else if (prefix.Contains("application/vnd.openxmlformats-officedocument.spreadsheetml.sheet"))
                        {
                            contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                            fileName = "document.xlsx";
                        }
                    }
                }

                // Validate base64 string before conversion
                if (string.IsNullOrEmpty(base64Data))
                {
                    ClientScript.RegisterStartupScript(
                        GetType(),
                        "error",
                        "Swal.fire('Error','Empty base64 data','error');",
                        true);
                    return;
                }

                // Check if base64 string is valid (length must be multiple of 4)
                if (base64Data.Length % 4 != 0)
                {
                    ClientScript.RegisterStartupScript(
                        GetType(),
                        "error",
                        "Swal.fire('Error','Invalid base64 length','error');",
                        true);
                    return;
                }

                // Convert base64 to bytes
                byte[] fileBytes = Convert.FromBase64String(base64Data);

                // Detect file type from magic bytes if content type is still generic
                if (contentType == "application/octet-stream" && fileBytes.Length > 4)
                {
                    // PDF: %PDF
                    if (fileBytes[0] == 0x25 && fileBytes[1] == 0x50 && fileBytes[2] == 0x44 && fileBytes[3] == 0x46)
                    {
                        contentType = "application/pdf";
                        fileName = "document.pdf";
                    }
                    // JPEG: FF D8 FF
                    else if (fileBytes[0] == 0xFF && fileBytes[1] == 0xD8 && fileBytes[2] == 0xFF)
                    {
                        contentType = "image/jpeg";
                        fileName = "document.jpg";
                    }
                    // PNG: 89 50 4E 47
                    else if (fileBytes[0] == 0x89 && fileBytes[1] == 0x50 && fileBytes[2] == 0x4E && fileBytes[3] == 0x47)
                    {
                        contentType = "image/png";
                        fileName = "document.png";
                    }
                    // GIF: GIF8
                    else if (fileBytes[0] == 0x47 && fileBytes[1] == 0x49 && fileBytes[2] == 0x46 && fileBytes[3] == 0x38)
                    {
                        contentType = "image/gif";
                        fileName = "document.gif";
                    }
                    // BMP: BM
                    else if (fileBytes[0] == 0x42 && fileBytes[1] == 0x4D)
                    {
                        contentType = "image/bmp";
                        fileName = "document.bmp";
                    }
                }

                Response.Clear();
                Response.ContentType = contentType;
                Response.AddHeader("Content-Disposition", $"attachment; filename=\"{fileName}\"");
                Response.BinaryWrite(fileBytes);
                Response.End();
            }
            catch (FormatException)
            {
                ClientScript.RegisterStartupScript(
                    GetType(),
                    "error",
                    "Swal.fire('Error','Invalid base64 format. Please check the document data.','error');",
                    true);
            }
            catch (Exception ex)
            {
                ClientScript.RegisterStartupScript(
                    GetType(),
                    "error",
                    $"Swal.fire('Error','{HttpUtility.JavaScriptStringEncode(ex.Message)}','error');",
                    true);
            }
        }
    }
}
