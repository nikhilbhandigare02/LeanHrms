using DataObject;
using ProcessModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace HRMS.View.Modules
{
    public partial class ImageLibrary : System.Web.UI.Page
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

                BindImages();
            }
        }

        private void BindImages()
        {
            try
            {
                List<AppImageDO> images = new CommonBL().GetAppImages();
                gvImages.DataSource = images;
                gvImages.DataBind();
            }
            catch (Exception ex)
            {
                new CommonBL().fnStoreErrorLog("ImageLibrary", "BindImages", ex.Message + " Strace=" + ex.StackTrace, UserId);
            }
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                string name = (txtImageName.Text ?? string.Empty).Trim();
                if (string.IsNullOrWhiteSpace(name))
                {
                    Toast("Error", "Please enter an image name.");
                    return;
                }

                if (!fuImage.HasFile)
                {
                    Toast("Error", "Please choose an image file.");
                    return;
                }

                string ext = Path.GetExtension(fuImage.FileName).ToLowerInvariant();
                string[] allowed = { ".png", ".jpg", ".jpeg", ".gif", ".svg", ".webp", ".bmp" };
                if (Array.IndexOf(allowed, ext) < 0)
                {
                    Toast("Error", "Unsupported file type. Use PNG, JPG, GIF, SVG, WEBP or BMP.");
                    return;
                }

                byte[] bytes = fuImage.FileBytes;
                if (bytes == null || bytes.Length == 0)
                {
                    Toast("Error", "The selected file is empty.");
                    return;
                }
                if (bytes.Length > 1 * 1024 * 1024)
                {
                    Toast("Error", "File is too large. Maximum allowed size is 300 kb.");
                    return;
                }

                string contentType = fuImage.PostedFile != null ? fuImage.PostedFile.ContentType : null;
                if (string.IsNullOrWhiteSpace(contentType) || contentType == "application/octet-stream")
                {
                    contentType = GuessContentType(ext);
                }

                AppImageDO img = new AppImageDO
                {
                    ImageType = ddlImageType.SelectedValue,
                    ImageName = name,
                    ImageBase64 = Convert.ToBase64String(bytes),
                    ContentType = contentType,
                    FileExtension = ext
                };

                int insertedBy = 0;
                int.TryParse(Convert.ToString(Session["userId"]), out insertedBy);

                AppImageDO result = new CommonBL().SaveAppImage(img, insertedBy);
                if (result != null && string.Equals(result.Status, "Success", StringComparison.OrdinalIgnoreCase))
                {
                    txtImageName.Text = string.Empty;
                    ddlImageType.SelectedIndex = 0;
                    BindImages();
                    Toast("Success", "Image saved successfully.");
                }
                else
                {
                    Toast("Error", result != null && !string.IsNullOrWhiteSpace(result.Remarks) ? result.Remarks : "Unable to save image.");
                }
            }
            catch (Exception ex)
            {
                new CommonBL().fnStoreErrorLog("ImageLibrary", "btnSave_Click", ex.Message + " Strace=" + ex.StackTrace, UserId);
                Toast("Error", "Unable to save image. Please try again.");
            }
        }

        protected void gvImages_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            try
            {
                if (e.CommandName != "DeleteImage")
                {
                    return;
                }

                int imageId;
                if (!int.TryParse(Convert.ToString(e.CommandArgument), out imageId))
                {
                    return;
                }

                int updatedBy = 0;
                int.TryParse(Convert.ToString(Session["userId"]), out updatedBy);

                AppImageDO result = new CommonBL().DeleteAppImage(imageId, updatedBy);
                BindImages();
                if (result != null && string.Equals(result.Status, "Success", StringComparison.OrdinalIgnoreCase))
                {
                    Toast("Success", "Image deleted successfully.");
                }
                else
                {
                    Toast("Error", "Unable to delete image.");
                }
            }
            catch (Exception ex)
            {
                new CommonBL().fnStoreErrorLog("ImageLibrary", "gvImages_RowCommand", ex.Message + " Strace=" + ex.StackTrace, UserId);
                Toast("Error", "Unable to delete image.");
            }
        }

        // Builds a data URI for inline preview in the grid.
        protected string GetImageSrc(object contentType, object base64)
        {
            string ct = Convert.ToString(contentType);
            if (string.IsNullOrWhiteSpace(ct))
            {
                ct = "image/png";
            }
            return "data:" + ct + ";base64," + Convert.ToString(base64);
        }

        private string GuessContentType(string ext)
        {
            switch (ext)
            {
                case ".png": return "image/png";
                case ".jpg":
                case ".jpeg": return "image/jpeg";
                case ".gif": return "image/gif";
                case ".svg": return "image/svg+xml";
                case ".webp": return "image/webp";
                case ".bmp": return "image/bmp";
                default: return "image/png";
            }
        }

        private void Toast(string status, string message)
        {
            string safe = (message ?? string.Empty).Replace("'", "\\'");
            ClientScript.RegisterStartupScript(GetType(), "ImageLibToast",
                $"showUserSavedMessage('{status}', '{safe}');", true);
        }
    }
}
