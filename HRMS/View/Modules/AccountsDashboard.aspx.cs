using DataObject;
using iText.Kernel.Pdf.Canvas.Parser.Data;
using ProcessModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using static DataObject.AccountDashboardDO;

namespace HRMS.View.Modules
{
    public partial class AccountsDashboard : System.Web.UI.Page
    {
        protected string UserId = null;
        AccountDashboardBL objBL = new AccountDashboardBL();

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
                else
                {
                    Session["CurrentPageIndex"] = 0;
                    Session["SearchResults"] = null;

                }
                BindTotalSalaryDisbursed();
                BindTotalReimburesement();
                BindActiveEmployeeCount();
                BindEmployeeSalaryDetails();
                BindEmployeeReimbursement();
            }
        }

        public void BindTotalSalaryDisbursed()
        {
            try
            {
                List<TotalDisbursedcountDO> salaryList = objBL.GetTotalSalaryDisbursed(UserId);

                if (salaryList != null && salaryList.Count > 0)
                {
                    salaryAmount.InnerText = "*****";
                    salaryAmount.Attributes["data-value"] =
                        "₹" + salaryList[0].TotalSalaryDisbursed.ToString("N2");
                    salaryAmount.Attributes["data-hidden"] = "true";
                }
                else
                {
                    salaryAmount.InnerText = "*****";
                    salaryAmount.Attributes["data-value"] = "₹0.00";
                    salaryAmount.Attributes["data-hidden"] = "true";
                }
            }
            catch (Exception ex)
            {
                CommonBL errorlog = new CommonBL();

                errorlog.fnStoreErrorLog(
                    "AccountsDashboard",
                    "BindTotalSalaryDisbursed",
                    "Exception Message : " + ex.Message +
                    " StackTrace : " + ex.StackTrace,
                    UserId);

                salaryAmount.InnerText = "₹0.00";
            }
        }

        public void BindTotalReimburesement()
        {
            try
            {
                List<TotalReimburesementdcountDO> reimbList = objBL.GetTotalReimbursementAmount();

                if (reimbList != null && reimbList.Count > 0)
                {
                    reimbAmount.InnerText = "*****";
                    reimbAmount.Attributes["data-value"] =
                        "₹" + reimbList[0].TotalReimbursement.ToString("N2");
                    reimbAmount.Attributes["data-hidden"] = "true";
                }
                else
                {
                    reimbAmount.InnerText = "*****";
                    reimbAmount.Attributes["data-value"] = "₹0.00";
                    reimbAmount.Attributes["data-hidden"] = "true";
                }
            }
            catch (Exception ex)
            {
                CommonBL errorlog = new CommonBL();

                errorlog.fnStoreErrorLog(
                    "AccountsDashboard",
                    "BindTotalReimburesement",
                    "Exception Message : " + ex.Message +
                    " StackTrace : " + ex.StackTrace,
                    UserId);

                reimbAmount.InnerText = "₹0.00";
            }
        }

        public void BindActiveEmployeeCount()
        {
            try
            {
                List<TotalActiveEmployeecountDO> employeeList = objBL.GetActiveEmployeeCount();

                if (employeeList != null && employeeList.Count > 0)
                {
                    litActiveEmployeeCount.Text = employeeList[0].ActiveEmployeeCount.ToString();
                }
                else
                {
                    litActiveEmployeeCount.Text = "0";
                }
            }
            catch (Exception ex)
            {
                CommonBL errorlog = new CommonBL();

                errorlog.fnStoreErrorLog(
                    "AccountsDashboard",
                    "BindActiveEmployeeCount",
                    "Exception Message : " + ex.Message +
                    " StackTrace : " + ex.StackTrace,
                    UserId);

                litActiveEmployeeCount.Text = "0";
            }
        }

        public void BindEmployeeSalaryDetails()
        {
            try
            {
                List<EmployeeSalaryDetailsDO> list = objBL.GetEmployeeSalaryDetails();

                rptEmployeeSalary.DataSource = list;
                rptEmployeeSalary.DataBind();

                lblNoSalaryData.Visible = (list == null || list.Count == 0);

            }
            catch (Exception ex)
            {
                CommonBL errorlog = new CommonBL();

                errorlog.fnStoreErrorLog(
                    "AccountsDashboard",
                    "BindEmployeeSalaryDetails",
                    "Exception Message : " + ex.Message +
                    " StackTrace : " + ex.StackTrace,
                    UserId);
                lblNoSalaryData.Visible = true;

            }
        }

        public void BindEmployeeReimbursement()
        {
            try
            {
                List<EmployeeReimbursementDO> list = objBL.GetEmployeeReimbursementDetails();

                rptEmployeeReimbursement.DataSource = list;
                rptEmployeeReimbursement.DataBind();

                lblnoreimbdata.Visible = (list == null || list.Count == 0);

            }
            catch (Exception ex)
            {
                CommonBL errorlog = new CommonBL();
                errorlog.fnStoreErrorLog(
                    "AccountsDashboard",
                    "BindEmployeeReimbursement",
                    "Exception Message : " + ex.Message +
                    " StackTrace : " + ex.StackTrace,
                    UserId);

                lblnoreimbdata.Visible = true;

            }
        }
        protected string GetStatusClass(string status)
        {
            switch (status.Trim().ToLower())
            {
                case "approved":
                    return "bg-success";

                case "pending":
                    return "bg-warning";

                case "rejected":
                    return "bg-danger";

                default:
                    return "bg-secondary";
            }
        }
    }
}
