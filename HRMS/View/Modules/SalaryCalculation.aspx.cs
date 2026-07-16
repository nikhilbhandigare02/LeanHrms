
using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using DataObject;
using ProcessModel;

namespace HRMS.View.Modules
{
    public partial class SalaryCalculation : Page
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

                BindSalaryCalculations();
            }
        }

        private void BindSalaryCalculations()
        {
            try
            {
                SalaryCalculationBL bl = new SalaryCalculationBL();
                var data = bl.GetSalaryCalculations();
                gvSalaryCalculations.DataSource = data;
                gvSalaryCalculations.DataBind();
            }
            catch (Exception ex)
            {
                new CommonBL().fnStoreErrorLog("SalaryCalculation", "BindSalaryCalculations", ex.Message + " | StackTrace=" + ex.StackTrace, UserId);
            }
        }

        protected string FormatMoney(object value)
        {
            decimal d;
            decimal.TryParse(Convert.ToString(value), out d);
            return "&#8377; " + d.ToString("N2");
        }

        protected void btnView_Click(object sender, EventArgs e)
        {
            LinkButton btn = (LinkButton)sender;
            int userId = Convert.ToInt32(btn.CommandArgument);
            Response.Redirect("~/View/Modules/SalaryCalculationDetails.aspx?id=" + userId + "&mode=view", false);
        }

        protected void btnEdit_Click(object sender, EventArgs e)
        {
            LinkButton btn = (LinkButton)sender;
            int userId = Convert.ToInt32(btn.CommandArgument);
            Response.Redirect("~/View/Modules/SalaryCalculationDetails.aspx?id=" + userId + "&mode=edit", false);
        }
    }
}
