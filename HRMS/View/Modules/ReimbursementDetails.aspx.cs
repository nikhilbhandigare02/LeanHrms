using DataObject;
using ProcessModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace HRMS.View.Modules
{
    public partial class ReimbursementDetails : System.Web.UI.Page
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
                BindReimbursementGrid();
            }
        }

        private void BindReimbursementGrid()
        {
            ReimbursementBL bl = new ReimbursementBL();
            List<ReimbursementDetailsDO> lst = bl.GetReimbursementDetailsList();
            gvReimbursement.DataSource = lst;
            gvReimbursement.DataBind();
        }

        protected void btnAddNew_Click(object sender, EventArgs e)
        {
            // Clear session variables to ensure add mode
            Session["Reimbursement_ID"] = null;
            Session["Reimbursement_Mode"] = null;
            Response.Redirect("AddReimbursementForm.aspx");
        }

        protected void btnView_Click(object sender, EventArgs e)
        {
            LinkButton btn = (LinkButton)sender;

            string reimbursementNumber = Convert.ToString(btn.CommandArgument);

            Session["Reimbursement_ID"] = reimbursementNumber;
            Session["Reimbursement_Mode"] = "View";

            Response.Redirect("AddReimbursementForm.aspx");
        }

        protected void btnEdit_Click(object sender, EventArgs e)
        {
            LinkButton btn = (LinkButton)sender;

            string reimbursementNumber = Convert.ToString(btn.CommandArgument);

            Session["Reimbursement_ID"] = reimbursementNumber;
            Session["Reimbursement_Mode"] = "Edit";

            Response.Redirect("AddReimbursementForm.aspx");
        }

        protected void btnDelete_Click(object sender, EventArgs e)
        {
            try
            {
                ReimbursementBL bl = new ReimbursementBL();

                LinkButton btn = (LinkButton)sender;
                string reimbursementNumber = Convert.ToString(btn.CommandArgument);
                ResponseDO result = bl.DeleteReimbursementDetails(reimbursementNumber);

                if (result.Status == 1)
                {
                    ClientScript.RegisterStartupScript(
                        GetType(),
                        "msg",
                        $"Swal.fire('Success','{HttpUtility.JavaScriptStringEncode(result.message)}','success')",
                        true);
                    BindReimbursementGrid();
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
                ClientScript.RegisterStartupScript(
                    GetType(),
                    "error",
                    $"Swal.fire('Error','{HttpUtility.JavaScriptStringEncode(ex.Message)}','error');",
                    true);
            }
        }
    }
}
