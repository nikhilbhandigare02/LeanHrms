using DataObject;
using iText.Kernel.Pdf.Canvas.Parser.Data;
using ProcessModel;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
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
        ReimbursementBL reimbBL = new ReimbursementBL();

        protected void Page_Load(object sender, EventArgs e)
        {
           // Page.MaintainScrollPositionOnPostBack = true;
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
                BindEmployeeCode();
                BindEmployeeName();
                BindReimbEmployeeCode();
                BindReimbEmployeeName();
            }
        }

        public void BindTotalSalaryDisbursed()
        {
            try
            {
                List<TotalDisbursedcountDO> salaryList = objBL.GetTotalSalaryDisbursed(UserId);

                if (salaryList != null && salaryList.Count > 0)
                {
                    salaryAmount.InnerText = "₹" + salaryList[0].TotalSalaryDisbursed.ToString("N2");
                }
                else
                {
                    salaryAmount.InnerText = "₹0.00";
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
                    reimbAmount.InnerText = "₹" + reimbList[0].TotalReimbursement.ToString("N2");
                }
                else
                {
                    reimbAmount.InnerText = "₹0.00";
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
                string empCode = string.IsNullOrEmpty(ddlEmployeeCodeSearch.SelectedValue)
                 ? ""
                 : ddlEmployeeCodeSearch.SelectedValue;

                string empName = string.IsNullOrEmpty(ddlEmployeeNameSearch.SelectedValue)
                     ? ""
                     : ddlEmployeeNameSearch.SelectedValue;

                string status = string.IsNullOrEmpty(ddlStatusSearch.SelectedValue)
                                ? ""
                                : ddlStatusSearch.SelectedValue;


                List<EmployeeSalaryDetailsDO> list =
                    objBL.GetEmployeeSalaryDetails(empCode, empName, status);

                rptEmployeeSalary.DataSource = list;
                rptEmployeeSalary.DataBind();

                lblNoSalaryData.Visible = list.Count == 0;

                litSalaryGridCount.Text = list.Count + "/" + litActiveEmployeeCount.Text;
            }
            catch (Exception ex)
            {
                CommonBL errorlog = new CommonBL();
                errorlog.fnStoreErrorLog(
                    "AccountsDashboard",
                    "BindEmployeeSalaryDetails",
                    ex.Message + ex.StackTrace,
                    UserId);
            }
        }
        public void BindEmployeeReimbursement()
        {
            try
            {
                string empCode = string.IsNullOrEmpty(ddlReimbEmployeeCode.SelectedValue)
                ? ""
                : ddlReimbEmployeeCode.SelectedValue;

                string empName = string.IsNullOrEmpty(ddlReimbEmployeeName.SelectedValue)
                     ? ""
                     : ddlReimbEmployeeName.SelectedValue;

                string status = string.IsNullOrEmpty(ddlReimbStatusSearch.SelectedValue)
                                ? ""
                                : ddlReimbStatusSearch.SelectedValue;

                List<EmployeeReimbursementDO> list = objBL.GetEmployeeReimbursementDetails(empCode, empName, status);

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
        protected void ddlStatus_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                DropDownList ddl = (DropDownList)sender;

                int salarySlipId = Convert.ToInt32(ddl.ToolTip);
                string status = ddl.SelectedValue;

                List<UpdateSalaryStatusDO> result = objBL.UpdateSalaryStatus(
                    salarySlipId,
                    status,
                    UserId);

                if (result != null && result.Count > 0)
                {
                    string Status = result[0].Success;
                    string remarks = result[0].Result;

                    if (Status.Equals("Success", StringComparison.OrdinalIgnoreCase))
                    {
                        // Send Mail only when status is Paid
                        if (status.Equals("Paid", StringComparison.OrdinalIgnoreCase))
                        {
                            List<SalaryPaidMailDO> mailDetails = objBL.GetSalaryPaidMailDetails(salarySlipId);

                            if (mailDetails != null && mailDetails.Count > 0)
                            {
                                objBL.SendSalaryPaidMail(
                                    mailDetails[0].ToEmail,
                                    mailDetails[0].CcEmail,
                                    mailDetails[0].Subject,
                                    mailDetails[0].Body);
                            }
                        }

                        BindEmployeeSalaryDetails();
                        BindTotalSalaryDisbursed();
                        ClientScript.RegisterStartupScript(
                            this.GetType(),
                            "SalaryStatus",
                            "showAccountsSavedMessage('" + Status + "','" + remarks + "');",
                            true);
                    }
                    else
                    {
                        ClientScript.RegisterStartupScript(
                            this.GetType(),
                            "SalaryStatus",
                            "showAccountsSavedMessage('" + Status + "','" + remarks + "');",
                            true);
                    }
                }
            }
            catch (Exception ex)
            {
                CommonBL errorlog = new CommonBL();
                errorlog.fnStoreErrorLog(
                    "SalarySlip",
                    "ddlStatus_SelectedIndexChanged",
                    ex.Message + ex.StackTrace,
                    UserId);

                ClientScript.RegisterStartupScript(
                    this.GetType(),
                    "SalaryStatus",
                    "showAccountsSavedMessage('Failed','Unable to update salary status.');",
                    true);
            }
        }

        protected void rptEmployeeSalary_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item ||
                e.Item.ItemType == ListItemType.AlternatingItem)
            {
                DropDownList ddl = (DropDownList)e.Item.FindControl("ddlStatus");

                if (ddl != null)
                {
                    string status = DataBinder.Eval(e.Item.DataItem, "status").ToString();

                    if (ddl.Items.FindByValue(status) != null)
                    {
                        ddl.SelectedValue = status;
                    }
                }
            }
        }

        protected void btnClear_Click(object sender, EventArgs e)
        {
            ddlSearchBy.SelectedIndex = 0;


            if (ddlEmployeeCodeSearch.Items.Count > 0)
                ddlEmployeeCodeSearch.SelectedIndex = 0;

            if (ddlEmployeeNameSearch.Items.Count > 0)
                ddlEmployeeNameSearch.SelectedIndex = 0;

            if (ddlStatusSearch.Items.Count > 0)
                ddlStatusSearch.SelectedIndex = 0;


            divEmployeeCode.Visible = false;
            divEmployeeName.Visible = false;
            divStatus.Visible = false;

            BindEmployeeSalaryDetails();
        }
        protected void ddlSearchBy_SelectedIndexChanged(object sender, EventArgs e)
        {
            ddlEmployeeCodeSearch.SelectedIndex = 0;
            ddlEmployeeNameSearch.SelectedIndex = 0;
            ddlStatusSearch.SelectedIndex = 0;

            divStatus.Visible = false;
            divEmployeeCode.Visible = false;
            divEmployeeName.Visible = false;


            if (ddlSearchBy.SelectedValue == "Status")
            {
                divStatus.Visible = true;
            }
            else if (ddlSearchBy.SelectedValue == "EmployeeCode")
            {
                divEmployeeCode.Visible = true;
                BindEmployeeCode();
            }
            else if (ddlSearchBy.SelectedValue == "EmployeeName")
            {
                divEmployeeName.Visible = true;
                BindEmployeeName();
            }
        }

        public void BindEmployeeCode()
        {
            List<DropDownData_account> list1 = new List<DropDownData_account>();
            CommonBL commonbl = new CommonBL();
            try
            {
                list1 = commonbl.dropdownempcode_accountdashboard();
                if (list1 != null)
                {
                    ddlEmployeeCodeSearch.DataSource = list1;
                    ddlEmployeeCodeSearch.DataTextField = "Text";
                    ddlEmployeeCodeSearch.DataValueField = "Id";
                }
                else
                {
                    ddlEmployeeCodeSearch.DataSource = null;
                }
                ddlEmployeeCodeSearch.DataBind();
                ddlEmployeeCodeSearch.Items.Insert(0, new ListItem("-- Please Select and Search --", ""));
                ddlEmployeeCodeSearch.SelectedIndex = 0;


            }
            catch (Exception ex)
            {
                CommonBL errorlog = new CommonBL();
                errorlog.fnStoreErrorLog("AccountsDashboard", "BindEmployeeCode", "Exception Message" + ex.Message + "StackTrace=" + ex.StackTrace, UserId);
            }
        }
       
        public void BindEmployeeName()
        {
            List<DropDownData_account> list1 = new List<DropDownData_account>();
            CommonBL commonbl = new CommonBL();
            try
            {
                list1 = commonbl.dropdownusername_accountdashboard();
                if (list1 != null)
                {
                    ddlEmployeeNameSearch.DataSource = list1;
                    ddlEmployeeNameSearch.DataTextField = "Text";
                    ddlEmployeeNameSearch.DataValueField = "Id";
                }
                else
                {
                    ddlEmployeeNameSearch.DataSource = null;
                }
                ddlEmployeeNameSearch.DataBind();
                ddlEmployeeNameSearch.Items.Insert(0, new ListItem("-- Please Select and Search --", ""));
                ddlEmployeeNameSearch.SelectedIndex = 0;


            }
            catch (Exception ex)
            {
                CommonBL errorlog = new CommonBL();
                errorlog.fnStoreErrorLog("AccountsDashboard", "BindEmployeeName", "Exception Message" + ex.Message + "StackTrace=" + ex.StackTrace, UserId);
            }
        }
        protected void btnSearch_Click(object sender, EventArgs e)
        {
            if (ddlSearchBy.SelectedValue == "")
            {
                ClientScript.RegisterStartupScript(this.GetType(), "msg",
                    "Swal.fire('Validation','Please select Search By.','warning');", true);
                return;
            }

            if (ddlSearchBy.SelectedValue == "EmployeeName" &&
                string.IsNullOrEmpty(ddlEmployeeNameSearch.SelectedValue))
            {
                ClientScript.RegisterStartupScript(this.GetType(), "msg",
                    "Swal.fire('Validation','Please select Employee Name.','warning');", true);
                return;
            }

            if (ddlSearchBy.SelectedValue == "EmployeeCode" &&
                string.IsNullOrEmpty(ddlEmployeeCodeSearch.SelectedValue))
            {
                ClientScript.RegisterStartupScript(this.GetType(), "msg",
                    "Swal.fire('Validation','Please select Employee Code.','warning');", true);
                return;
            }

            if (ddlSearchBy.SelectedValue == "Status" &&
                string.IsNullOrEmpty(ddlStatusSearch.SelectedValue))
            {
                ClientScript.RegisterStartupScript(this.GetType(), "msg",
                    "Swal.fire('Validation','Please select Status.','warning');", true);
                return;
            }

            BindEmployeeSalaryDetails();
        }
        protected void ddlreimbStatus_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                DropDownList ddl = (DropDownList)sender;

                int reimbursementId = Convert.ToInt32(ddl.ToolTip);
                string status = ddl.SelectedValue;

                List<UpdateReimbursementStatusDO> result =
                    objBL.UpdatereimbSalaryStatus(
                        reimbursementId,
                        status,
                        UserId);

                if (result != null && result.Count > 0)
                {
                    string Status = result[0].Success;
                    string remarks = result[0].Result;

                    if (Status.Equals("Success", StringComparison.OrdinalIgnoreCase))
                    {
                        if (status.Equals("Paid", StringComparison.OrdinalIgnoreCase))
                        {
                            List<SalaryPaidMailDO> mailDetails = objBL.GetReimbPaidMailDetails(reimbursementId);

                            if (mailDetails != null && mailDetails.Count > 0)
                            {
                                objBL.SendSalaryPaidMail(
                                    mailDetails[0].ToEmail,
                                    mailDetails[0].CcEmail,
                                    mailDetails[0].Subject,
                                    mailDetails[0].Body);
                            }
                        }
                        BindEmployeeReimbursement();   
                        BindTotalReimburesement();
                        ClientScript.RegisterStartupScript(
                            this.GetType(),
                            "ReimbStatus",
                            "showAccountsSavedMessage('" + Status + "','" + remarks + "');",
                            true);
                    }
                    else
                    {
                        ClientScript.RegisterStartupScript(
                            this.GetType(),
                            "ReimbStatus",
                            "showAccountsSavedMessage('" + Status + "','" + remarks + "');",
                            true);
                    }
                }
            }
            catch (Exception ex)
            {
                CommonBL errorlog = new CommonBL();

                errorlog.fnStoreErrorLog(
                    "AccountsDashboard",
                    "ddlreimbStatus_SelectedIndexChanged",
                    ex.Message + ex.StackTrace,
                    UserId);

                ClientScript.RegisterStartupScript(
                    this.GetType(),
                    "ReimbStatus",
                    "showAccountsSavedMessage('Failed','Unable to update reimbursement status.');",
                    true);
            }
        }

        protected void rptEmployeeReimbursement_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            if (!string.Equals(e.CommandName, "ViewDocs", StringComparison.OrdinalIgnoreCase))
            {
                return;
            }

            try
            {
                int reimbursementId;
                if (!int.TryParse(Convert.ToString(e.CommandArgument), out reimbursementId))
                {
                    return;
                }

                ReimbursementOwnerDO owner = reimbBL.GetReimbursementOwnerById(reimbursementId);

                if (owner == null)
                {
                    lblNoReimbDocs.Visible = true;
                    rptReimbDocs.Visible = false;
                    ClientScript.RegisterStartupScript(this.GetType(), "openReimbDocsModal", "openReimbDocsModal();", true);
                    return;
                }

                hdnReimbDocsUserId.Value = owner.UserId.ToString();
                hdnReimbDocsNumber.Value = owner.ReimbursementNumber;

                List<ReimbursementDocumentDO> documents =
                    reimbBL.GetReimbursementDocuments(owner.UserId, owner.ReimbursementNumber) ?? new List<ReimbursementDocumentDO>();

                rptReimbDocs.DataSource = documents;
                rptReimbDocs.DataBind();

                rptReimbDocs.Visible = documents.Count > 0;
                lblNoReimbDocs.Visible = documents.Count == 0;

                ClientScript.RegisterStartupScript(this.GetType(), "openReimbDocsModal", "openReimbDocsModal();", true);
            }
            catch (Exception ex)
            {
                CommonBL errorlog = new CommonBL();
                errorlog.fnStoreErrorLog(
                    "AccountsDashboard",
                    "rptEmployeeReimbursement_ItemCommand",
                    "Exception Message : " + ex.Message + " StackTrace : " + ex.StackTrace,
                    UserId);
            }
        }

        protected void rptReimbDocs_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            bool isView = string.Equals(e.CommandName, "ViewReimbDoc", StringComparison.OrdinalIgnoreCase);
            bool isDownload = string.Equals(e.CommandName, "DownloadReimbDoc", StringComparison.OrdinalIgnoreCase);

            if (!isView && !isDownload)
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

                int userId;
                int.TryParse(hdnReimbDocsUserId.Value, out userId);
                string reimbursementNumber = hdnReimbDocsNumber.Value;

                List<ReimbursementDocumentDO> documents =
                    reimbBL.GetReimbursementDocuments(userId, reimbursementNumber) ?? new List<ReimbursementDocumentDO>();
                ReimbursementDocumentDO document = documents.FirstOrDefault(d => d.UserDocDetId == userDocDetId);

                if (document == null)
                {
                    ClientScript.RegisterStartupScript(this.GetType(), "warning", "Swal.fire('Warning','Document not found','warning');", true);
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
                    ClientScript.RegisterStartupScript(this.GetType(), "warning", "Swal.fire('Warning','File not found on server','warning');", true);
                    return;
                }

                FileInfo fileInfo = new FileInfo(physicalPath);
                Response.Clear();

                if (isView)
                {
                    string ext = fileInfo.Extension.ToLower();
                    string contentType;

                    switch (ext)
                    {
                        case ".pdf":
                            contentType = "application/pdf";
                            break;
                        case ".jpg":
                        case ".jpeg":
                            contentType = "image/jpeg";
                            break;
                        case ".png":
                            contentType = "image/png";
                            break;
                        case ".gif":
                            contentType = "image/gif";
                            break;
                        default:
                            contentType = "application/octet-stream";
                            break;
                    }

                    Response.ContentType = contentType;
                    Response.AddHeader("Content-Disposition", "inline; filename=\"" + fileInfo.Name + "\"");
                    Response.WriteFile(physicalPath);
                }
                else
                {
                    Response.ContentType = "application/octet-stream";
                    Response.AddHeader("Content-Disposition", "attachment; filename=\"" + fileInfo.Name + "\"");
                    Response.AddHeader("Content-Length", fileInfo.Length.ToString());
                    Response.TransmitFile(physicalPath);
                }

                Response.Flush();
                Response.End();
            }
            catch (Exception ex)
            {
                CommonBL errorlog = new CommonBL();
                errorlog.fnStoreErrorLog(
                    "AccountsDashboard",
                    "rptReimbDocs_ItemCommand",
                    "Exception Message : " + ex.Message + " StackTrace : " + ex.StackTrace,
                    UserId);
            }
        }

        protected void rptEmployeeReimbursement_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item ||
                e.Item.ItemType == ListItemType.AlternatingItem)
            {
                DropDownList ddl = (DropDownList)e.Item.FindControl("ddlreimbStatus");

                if (ddl != null)
                {
                    string status = DataBinder.Eval(e.Item.DataItem, "status").ToString();

                    if (ddl.Items.FindByValue(status) != null)
                    {
                        ddl.SelectedValue = status;
                    }
                }
            }
        }

        protected void btnReimbClear_Click(object sender, EventArgs e)
        {
            ddlSearchByReimb.SelectedIndex = 0;

            if (ddlReimbEmployeeCode.Items.Count > 0)
                ddlReimbEmployeeCode.SelectedIndex = 0;

            if (ddlReimbEmployeeName.Items.Count > 0)
                ddlReimbEmployeeName.SelectedIndex = 0;

            if (ddlReimbStatusSearch.Items.Count > 0)
                ddlReimbStatusSearch.SelectedIndex = 0;

            divReimbStatus.Visible = false;
            divReimbEmpCode.Visible = false;
            divReimbEmpName.Visible = false;

            BindEmployeeReimbursement();
        }

        protected void ddlSearchByReimb_SelectedIndexChanged(object sender, EventArgs e)
        {
            ddlReimbEmployeeCode.SelectedIndex = 0;
            ddlReimbEmployeeName.SelectedIndex = 0;
            ddlReimbStatusSearch.SelectedIndex = 0;

            divReimbStatus.Visible = false;
            divReimbEmpCode.Visible = false;
            divReimbEmpName.Visible = false;


            if (ddlSearchByReimb.SelectedValue == "Status")
            {
                divReimbStatus.Visible = true;
            }
            else if (ddlSearchByReimb.SelectedValue == "EmployeeCode")
            {
                divReimbEmpCode.Visible = true;
                BindReimbEmployeeCode();
            }
            else if (ddlSearchByReimb.SelectedValue == "EmployeeName")
            {
                divReimbEmpName.Visible = true;
                BindReimbEmployeeName();
            }

        }

        protected void btnReimbSearch_Click(object sender, EventArgs e)
        {
            if (ddlSearchByReimb.SelectedValue == "")
            {
                ClientScript.RegisterStartupScript(this.GetType(), "msg",
                    "Swal.fire('Validation','Please select Search By.','warning');", true);
                return;
            }

            if (ddlSearchByReimb.SelectedValue == "EmployeeName" &&
                string.IsNullOrEmpty(ddlReimbEmployeeName.SelectedValue))
            {
                ClientScript.RegisterStartupScript(this.GetType(), "msg",
                    "Swal.fire('Validation','Please select Employee Name.','warning');", true);
                return;
            }

            if (ddlSearchByReimb.SelectedValue == "EmployeeCode" &&
                string.IsNullOrEmpty(ddlReimbEmployeeCode.SelectedValue))
            {
                ClientScript.RegisterStartupScript(this.GetType(), "msg",
                    "Swal.fire('Validation','Please select Employee Code.','warning');", true);
                return;
            }

            if (ddlSearchByReimb.SelectedValue == "Status" &&
                string.IsNullOrEmpty(ddlReimbStatusSearch.SelectedValue))
            {
                ClientScript.RegisterStartupScript(this.GetType(), "msg",
                    "Swal.fire('Validation','Please select Status.','warning');", true);
                return;
            }

            BindEmployeeReimbursement();
        }

        public void BindReimbEmployeeCode()
        {
            List<DropDownData_account> list1 = new List<DropDownData_account>();
            CommonBL commonbl = new CommonBL();
            try
            {
                list1 = commonbl.dropdownempcode_accountdashboard();
                if (list1 != null)
                {
                    ddlReimbEmployeeCode.DataSource = list1;
                    ddlReimbEmployeeCode.DataTextField = "Text";
                    ddlReimbEmployeeCode.DataValueField = "Id";
                }
                else
                {
                    ddlReimbEmployeeCode.DataSource = null;
                }
                ddlReimbEmployeeCode.DataBind();
                ddlReimbEmployeeCode.Items.Insert(0, new ListItem("-- Please Select and Search --", ""));
                ddlReimbEmployeeCode.SelectedIndex = 0;


            }
            catch (Exception ex)
            {
                CommonBL errorlog = new CommonBL();
                errorlog.fnStoreErrorLog("AccountsDashboard", "BindReimbEmployeeCode", "Exception Message" + ex.Message + "StackTrace=" + ex.StackTrace, UserId);
            }
        }

        public void BindReimbEmployeeName()
        {
            List<DropDownData_account> list1 = new List<DropDownData_account>();
            CommonBL commonbl = new CommonBL();
            try
            {
                list1 = commonbl.dropdownusername_accountdashboard();
                if (list1 != null)
                {
                    ddlReimbEmployeeName.DataSource = list1;
                    ddlReimbEmployeeName.DataTextField = "Text";
                    ddlReimbEmployeeName.DataValueField = "Id";
                }
                else
                {
                    ddlReimbEmployeeName.DataSource = null;
                }
                ddlReimbEmployeeName.DataBind();
                ddlReimbEmployeeName.Items.Insert(0, new ListItem("-- Please Select and Search --", ""));
                ddlReimbEmployeeName.SelectedIndex = 0;


            }
            catch (Exception ex)
            {
                CommonBL errorlog = new CommonBL();
                errorlog.fnStoreErrorLog("AccountsDashboard", "BindReimbEmployeeName", "Exception Message" + ex.Message + "StackTrace=" + ex.StackTrace, UserId);
            }
        }
    }
}
