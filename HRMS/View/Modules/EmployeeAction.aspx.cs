using DataObject;
using ProcessModel;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Configuration;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Mail;
using System.Text;
using System.Web;
using System.Web.Script.Serialization;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace HRMS.View.Modules
{
    public partial class EmployeeAction : System.Web.UI.Page
    {
        protected string UserId = null;

        // Label for the grid's action link - reflects the NEXT step for this
        // employee's current status, not a generic "Terminate" once a CAP
        // round is already in progress.
        protected string GetTerminateActionLabel(object status)
        {
            string s = status?.ToString();
            if (s == "CAP1") return "Save CAP 2";
            // While CAP2's target date hasn't passed, this stays labeled Edit -
            // same underlying action as before, just relabeled per CAP2 window.
            if (s == "CAP2") return "Edit";
            // Notice already sent - still opens the same modal (to remove/
            // escalate), just reflects the current state instead of "Terminate".
            if (s == "ShowCauseIssued") return "Sent Show Cause Notice";
            return "Terminate";
        }

        // Once CAP2's target date has passed (SP_GetEmployeeTerminationStatus
        // reports CAP2_EXPIRED), neither Edit nor Terminate show anymore -
        // what happens next from there is handled by a separate scheduler.
        protected bool ShouldShowActionButton(object status)
        {
            string s = status?.ToString();
            return s != "Terminated" && s != "CAP2_EXPIRED";
        }

        protected string GetCapBadgeText(object status)
        {
            string s = status?.ToString();
            if (s == "CAP1") return "CAP 1 Issued";
            if (s == "CAP2") return "CAP 2 Issued";
            if (s == "CAP2_EXPIRED") return "CAP 2 Expired";
            return "";
        }

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

                // List view is unfiltered (no Select Company / search UI) - load
                // everyone straight away. Session["SelectedCompanyId"] is kept at
                // 0 purely so the existing paging/sorting handlers (which read it
                // to re-bind the grid) keep working unchanged.
                Session["SelectedCompanyId"] = 0;
                Session["CurrentPageIndex"] = 0;
                BindGridViewFromAPI(0);

                // Employees with an in-progress CAP/Show Cause case no longer
                // appear in the main grid at all, so History's "Manage" link
                // comes back here with this instead of a grid row click.
                int manageUserId = 0;
                int.TryParse(Request.QueryString["manage_user_id"], out manageUserId);
                if (manageUserId > 0)
                {
                    OpenManageModalForUser(manageUserId);
                }

                //BindDropdownTerminationReason();
            }
        }
        //public void BindDropdownTerminationReason()
        //{
        //    List<DropDownData> list1 = new List<DropDownData>();
        //    CommonBL commonbl = new CommonBL();
        //    try
        //    {
        //        list1 = commonbl.dropdowterminationReason();
        //        if (list1 != null)
        //        {
        //            ddlTerminationReason.DataSource = list1;
        //            ddlTerminationReason.DataTextField = "Text";
        //            ddlTerminationReason.DataValueField = "Id";
        //        }
        //        else
        //        {
        //            ddlTerminationReason.DataSource = null;
        //        }
        //        ddlTerminationReason.DataBind();
        //        ddlTerminationReason.Items.Insert(0, new ListItem("-- Please Select --", ""));


        //    }
        //    catch (Exception ex)
        //    {
        //        CommonBL errorlog = new CommonBL();
        //        errorlog.fnStoreErrorLog("EmployeeAction", "BindDropdownTerminationReason", "Exception Message" + ex.Message + "StackTrace=" + ex.StackTrace, UserId);
        //    }
        //}
        protected void BindGridViewFromAPI(int companyId)
        {
            try
            {
                var users = GetUsersFromAPI(companyId); // ✅ pass companyId

                // Resolve status for everyone up front - one batched round trip
                // regardless of list size - so fully Terminated employees can be
                // filtered out of the main grid before pagination. They stay
                // fully visible/searchable in Termination History instead of
                // being deleted; this only affects what this grid displays.
                HandoverprocessBL statusBl = new HandoverprocessBL();
                var statusByUserId = statusBl.GetEmployeeTerminationStatusBulk(
                    users.Select(u => u.UserId).ToList());

                foreach (var u in users)
                {
                    u.notice_status = statusByUserId.TryGetValue(u.UserId, out var s) ? s : "None";
                }

                // Main grid shows only untouched employees - anyone with ANY
                // termination action in progress (CAP1/CAP2/Show Cause) or
                // completed (Terminated) is managed from Termination History
                // instead, per explicit confirmation this replaces the earlier
                // "hide only Terminated" behavior.
                users = users.Where(u => u.notice_status == "None").ToList();

                ApplySorting(ref users); // existing sorting logic

                int totalRecords = users.Count;
                int pageSize = 10;
                int pageIndex = Convert.ToInt32(Session["CurrentPageIndex"] ?? 0);

                int totalPages = (int)Math.Ceiling((double)totalRecords / pageSize);
                if (pageIndex >= totalPages) pageIndex = Math.Max(totalPages - 1, 0);
                if (pageIndex < 0) pageIndex = 0;
                Session["CurrentPageIndex"] = pageIndex;
                hfPageIndexViewUser.Value = pageIndex.ToString();

                int startRowIndex = pageIndex * pageSize;
                int endRowIndex = Math.Min(startRowIndex + pageSize, totalRecords);

                if (totalRecords > 0)
                {
                    List<UserDetailsDO> displayedData = users.GetRange(startRowIndex, endRowIndex - startRowIndex);

                    gridview.DataSource = displayedData;
                    gridview.DataBind();
                    gridview.Visible = true;

                    paginationContainer.Visible = totalRecords > pageSize;
                    UpdatePageInfoLabel(pageIndex, totalRecords);
                }
                else
                {
                    gridview.DataSource = null;
                    gridview.DataBind();
                    gridview.Visible = false;
                    paginationContainer.Visible = false;
                    UpdatePageInfoLabel(0, 0);
                }
            }
            catch (Exception ex)
            {
                gridview.Visible = false;
                CommonBL errorlog = new CommonBL();
                errorlog.fnStoreErrorLog("EmployeeAction", "BindGridViewFromAPI",
                    "Exception Message: " + ex.Message + " StackTrace: " + ex.StackTrace, UserId);
            }
        }


        protected List<UserDetailsDO> GetUsersFromAPI(int companyId)
        {
            List<UserDetailsDO> users = new List<UserDetailsDO>();
            try
            {
                UserDetailsBL userDetailsBL = new UserDetailsBL();
                users = userDetailsBL.ViewAllUsers();

                if (companyId > 0)
                {
                    users = users.Where(u =>
                        u.company_id == companyId ||
                        u.CompanyId == companyId
                    ).ToList();
                }
            }
            catch (Exception ex)
            {
                CommonBL errorlog = new CommonBL();
                errorlog.fnStoreErrorLog("EmployeeAction", "GetUsersFromAPI",
                    "Exception Message: " + ex.Message + " StackTrace: " + ex.StackTrace, UserId);
            }
            return users;
        }


        public int TotalRecordCount()
        {

            UserDetailsDO userDO = new UserDetailsDO();
            UserDetailsBL userbl = new UserDetailsBL();
            List<UserDetailsDO> users = userbl.ViewAllUsers();

            return users.Count;
        }
        protected void UpdatePageInfoLabel(int pageIndex, int pagecount)
        {
            try
            {
                int pageSize = 10;
                int totalPages = Math.Max((int)Math.Ceiling((double)pagecount / pageSize), 1);

                List<PagerItem> pages = new List<PagerItem>();
                for (int i = 0; i < totalPages; i++)
                {
                    pages.Add(new PagerItem { PageIndex = i, PageNumber = (i + 1).ToString(), IsActive = i == pageIndex });
                }

                rptPageNumbers.DataSource = pages;
                rptPageNumbers.DataBind();

                lnkPrevPage.Enabled = pageIndex > 0;
                lnkPrevPage.CssClass = lnkPrevPage.Enabled ? "page-btn" : "page-btn disabled";

                lnkNextPage.Enabled = pageIndex < totalPages - 1;
                lnkNextPage.CssClass = lnkNextPage.Enabled ? "page-btn" : "page-btn disabled";
            }
            catch (Exception ex)
            {
                CommonBL errorlog = new CommonBL();
                errorlog.fnStoreErrorLog("EmployeeAction", "UpdatePageInfoLabel", "Exception Message" + ex.Message + "Strace=" + ex.StackTrace, UserId);
            }
        }
        protected void rptPageNumbers_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            try
            {
                if (e.CommandName == "GoToPage")
                {
                    Session["CurrentPageIndex"] = Convert.ToInt32(e.CommandArgument);
                    int companyId = Convert.ToInt32(Session["SelectedCompanyId"]);
                    BindGridViewFromAPI(companyId);
                }
            }
            catch (Exception ex)
            {
                CommonBL errorlog = new CommonBL();
                errorlog.fnStoreErrorLog("EmployeeAction", "rptPageNumbers_ItemCommand", "Exception Message" + ex.Message + "Strace=" + ex.StackTrace, UserId);
            }
        }
        protected void lnkPrevPage_Click(object sender, EventArgs e)
        {
            try
            {
                int pageIndex = Convert.ToInt32(Session["CurrentPageIndex"] ?? 0);
                if (pageIndex > 0)
                {
                    Session["CurrentPageIndex"] = pageIndex - 1;
                    int companyId = Convert.ToInt32(Session["SelectedCompanyId"]);
                    BindGridViewFromAPI(companyId);
                }
            }
            catch (Exception ex)
            {
                CommonBL errorlog = new CommonBL();
                errorlog.fnStoreErrorLog("EmployeeAction", "lnkPrevPage_Click", "Exception Message" + ex.Message + "Strace=" + ex.StackTrace, UserId);
            }
        }
        protected void lnkNextPage_Click(object sender, EventArgs e)
        {
            try
            {
                int pageIndex = Convert.ToInt32(Session["CurrentPageIndex"] ?? 0);
                Session["CurrentPageIndex"] = pageIndex + 1;
                int companyId = Convert.ToInt32(Session["SelectedCompanyId"]);
                BindGridViewFromAPI(companyId);
            }
            catch (Exception ex)
            {
                CommonBL errorlog = new CommonBL();
                errorlog.fnStoreErrorLog("EmployeeAction", "lnkNextPage_Click", "Exception Message" + ex.Message + "Strace=" + ex.StackTrace, UserId);
            }
        }
        private class PagerItem
        {
            public int PageIndex { get; set; }
            public string PageNumber { get; set; }
            public bool IsActive { get; set; }
        }
        protected void OnPageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            gridview.PageIndex = e.NewPageIndex;
            //BindGridView();
            int companyId = Convert.ToInt32(Session["SelectedCompanyId"]);
            BindGridViewFromAPI(companyId);
        }
        protected void gridview_Sorting(object sender, GridViewSortEventArgs e)
        {
            UserDetailsBL userDetailsBL = new UserDetailsBL();
            try
            {
                List<UserDetailsDO> createdet = userDetailsBL.ViewAllUsers();

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

                    gridview.DataSource = createdet;
                    gridview.DataBind();
                }
              ;
            }

            catch (Exception ex)
            {
                CommonBL errorlog = new CommonBL();
                errorlog.fnStoreErrorLog("EmployeeAction", "gridview_Sorting", "Exception Message" + ex.Message + "Strace=" + ex.StackTrace, UserId);
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
        private void ApplySorting(ref List<UserDetailsDO> users)
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
                errorlog.fnStoreErrorLog("EmployeeAction", "ApplySorting", "Exception Message" + ex.Message + "Strace=" + ex.StackTrace, UserId);
            }
        }
        //protected void btnConfirmTermination_Click(object sender, EventArgs e)
        //{

        //    TerminationProcessDO obj = new TerminationProcessDO
        //    {
        //        CompanyId = Convert.ToInt32(Session["SelectedCompanyId"]),
        //        UserId = Convert.ToInt32(hfUserId.Value),
        //        EmployeeCode = hfEmployeeCode.Value,
        //        TerminationDate = Convert.ToDateTime(txtTerminationDate.Text),
        //        //TerminationReasonId = Convert.ToInt32(ddlTerminationReason.SelectedValue),
        //       // Remark = txtTerminationRemark.Text.Trim(),
        //        InsertedBy = Convert.ToInt32(Session["UserId"])
        //    };

        //    HandoverprocessBL bl = new HandoverprocessBL();
        //    var result = bl.SaveEmployeeTermination(obj);

        //    if (result.Count > 0 && result[0].Status == 1)
        //    {
        //        // ✅ Clear controls
        //        ClearTerminationForm();

        //        // ✅ Success message
        //        ScriptManager.RegisterStartupScript(
        //            this,
        //            GetType(),
        //            "TerminationSuccess",
        //            $"showUserSavedMessage('Success', '{result[0].Message}');",
        //            true
        //        );

        //        // ✅ Close modal (optional but recommended)
        //        ScriptManager.RegisterStartupScript(
        //            this,
        //            GetType(),
        //            "CloseModal",
        //            "var m=document.getElementById('terminationModal'); if(m){bootstrap.Modal.getInstance(m)?.hide();}",
        //            true
        //        );

        //        // ✅ Refresh grid
        //        BindGridViewFromAPI(obj.CompanyId);
        //    }
        //}
        protected void btnConfirmTermination_Click(object sender, EventArgs e)
        {
            //int userId = 0;
            //if (!string.IsNullOrWhiteSpace(hfUserId.Value))
            //    int.TryParse(hfUserId.Value, out userId); // safe conversion

            //string employeeCode = hfEmployeeCode?.Value ?? "";
            //string terminationType = hfTerminationType?.Value ?? "";
            //string employeeEmail = hfEmployeeEmail?.Value ?? "";
            //string employeeName = hfEmployeeName?.Value ?? "";

            if (!ValidateTerminationForm(out string errorMessage))
            {
                ScriptManager.RegisterStartupScript(
                    this, GetType(), "validation",
                    $"showUserSavedMessage('Validation Error','{errorMessage}');",
                    true);

                ScriptManager.RegisterStartupScript(
                    this, GetType(), "keepopen",
                    "var m = new bootstrap.Modal(document.getElementById('terminationModal')); m.show();",
                    true);

                return; // stop execution
            }


            TerminationProcessDO obj = new TerminationProcessDO
            {
                CompanyId = Convert.ToInt32(hfCompanyId.Value),
                UserId = Convert.ToInt32(hfUserId.Value),
                EmployeeCode = hfEmployeeCode.Value,
                TerminationDate = Convert.ToDateTime(txtTerminationDate.Text),
                termination_reason = hfTerminationType.Value,
                TerminationType = hfTerminationType.Value,
                InsertedBy = Convert.ToInt32(Session["UserId"]),
                EmployeeEmail = hfEmployeeEmail.Value,
                EmployeeName = hfEmployeeName.Value
                //CompanyId = Convert.ToInt32(Session["SelectedCompanyId"]),
                //UserId = userId,                
                //EmployeeCode = employeeCode,
                //TerminationDate = Convert.ToDateTime(txtTerminationDate.Text),
                //termination_reason = terminationType,
                //InsertedBy = Convert.ToInt32(Session["UserId"]),
                //EmployeeEmail = employeeEmail,
                //EmployeeName = employeeName

            };

            HandoverprocessBL bl = new HandoverprocessBL();

            // Captured so the success message can reflect exactly which
            // action just completed (CAP1 / CAP2 / final) instead of a
            // generic "termination saved" message.
            int? capRoundForMessage = null;

            // PERFORMANCE BASED - 2 CAP rounds before the actual termination.
            // Each click is a new row: 1st -> CAP1, 2nd -> CAP2, 3rd -> final
            // Terminated (which is when is_terminated gets set in userm).
            if (hfTerminationType.Value == "Performance")
            {
                string existingStatus = bl.GetEmployeeTerminationStatus(obj.UserId);

                int? nextCapRound;
                string reasonText;
                if (existingStatus == "CAP1")
                {
                    nextCapRound = 2;
                    reasonText = "Performance Based Letter - CAP 2";
                }
                else if (existingStatus == "CAP2" || existingStatus == "CAP2_EXPIRED")
                {
                    nextCapRound = null;
                    reasonText = "Performance Based Letter";
                }
                else
                {
                    nextCapRound = 1;
                    reasonText = "Performance Based Letter - CAP 1";
                }

                obj.CapRound = nextCapRound;
                capRoundForMessage = nextCapRound;
                obj.termination_reason = reasonText;

                if (!string.IsNullOrEmpty(hfPerformanceRating.Value))
                    obj.PerformanceRating = Convert.ToInt32(hfPerformanceRating.Value);
                else
                    obj.PerformanceRating = null;

                obj.NoticePeriodDays = Convert.ToInt32(txtNoticePeriod.Text.Trim());
                obj.TerminationLetter = txtLetterPreview.Text.Trim();
            }

            // SHOW CAUSE
            if (hfTerminationType.Value == "ShowCause")
            {
                obj.termination_reason = "Show Cause Notice";

                if (!string.IsNullOrWhiteSpace(txtResponseDeadline.Text))
                    obj.ResponseDeadline = Convert.ToDateTime(txtResponseDeadline.Text);
                else
                    obj.ResponseDeadline = null;

                obj.NoticeLetter = txtNoticeLetter.Text.Trim();
            }

            // DIRECT TERMINATE
            if (hfTerminationType.Value == "DirectTerminate")
            {
                obj.termination_reason = txtDirectTerminationReason.Text.Trim();
                obj.TerminationLetter = txtDirectTerminationRemarks.Text.Trim();
            }

            var result = bl.SaveEmployeeTermination(obj);

            if (result.Count > 0 && result[0].Status == 1)
            {
                // SP resolves and returns the To/CC email addresses (employee's own
                // mail id + reporting manager's mail id) and the letter preview that
                // was actually saved - use those rather than re-deriving them here.
                var saved = result[0];
                obj.ToEmail = saved.ToEmail;
                obj.CCEmail = saved.CCEmail;
                obj.TerminationLetterPreview = saved.TerminationLetterPreview;
                if (!string.IsNullOrWhiteSpace(saved.EmployeeEmail))
                    obj.EmployeeEmail = saved.EmployeeEmail;
                if (!string.IsNullOrWhiteSpace(saved.EmployeeName))
                    obj.EmployeeName = saved.EmployeeName;

                // Separate SP call that reads the just-saved row back (by UserId,
                // not the row's id) and returns ToEmail, CCEmail, EmailSubject,
                // EmailBody together - use these as the authoritative values.
                var emailContent = bl.GetTerminationEmailContent(obj.UserId);
                if (emailContent != null)
                {
                    if (!string.IsNullOrWhiteSpace(emailContent.ToEmail))
                        obj.ToEmail = emailContent.ToEmail;
                    if (!string.IsNullOrWhiteSpace(emailContent.CCEmail))
                        obj.CCEmail = emailContent.CCEmail;
                    obj.EmailSubject = emailContent.EmailSubject;
                    obj.EmailBody = emailContent.EmailBody;
                }

                SendTerminationEmail(obj);

                // Escalating from the Show Cause tab reuses this same Submit
                // handler (per the Direct Terminate flow) - also mark the
                // original show-cause row Terminated so its own history
                // doesn't stay stuck at "Show Cause Issued".
                if (sender == btnEscalateShowCause)
                {
                    bl.MarkShowCauseAsTerminated(obj.UserId, obj.InsertedBy);
                }

                ClearTerminationForm();

                string successMessage = GetTerminationSuccessMessage(hfTerminationType.Value, capRoundForMessage);
                ScriptManager.RegisterStartupScript(
                    this, GetType(), "ok",
                    $"showUserSavedMessage('Success','{successMessage}');", true);

                ScriptManager.RegisterStartupScript(
                    this, GetType(), "close",
                    "bootstrap.Modal.getInstance(document.getElementById('terminationModal'))?.hide();",
                    true);

                BindGridViewFromAPI(obj.CompanyId);
            }
        }

        // One specific success message per action, instead of always showing
        // the save SP's generic "Termination record saved successfully."
        private string GetTerminationSuccessMessage(string terminationType, int? capRound)
        {
            if (terminationType == "Performance")
            {
                if (capRound == 1) return "CAP 1 issued successfully.";
                if (capRound == 2) return "CAP 2 issued successfully.";
                return "Employee terminated successfully.";
            }

            if (terminationType == "DirectTerminate")
                return "Direct Termination sent successfully.";

            return "Termination record saved successfully.";
        }

        private bool ValidateTerminationForm(out string errorMessage)
        {
            errorMessage = "";

            // Performance Based validation
            if (hfTerminationType.Value == "Performance")
            {
                if (string.IsNullOrWhiteSpace(hfPerformanceRating.Value))
                {
                    errorMessage = "Performance rating is required.";
                    return false;
                }

                if (string.IsNullOrWhiteSpace(txtNoticePeriod.Text) ||
                    !int.TryParse(txtNoticePeriod.Text.Trim(), out int noticeDaysCheck) ||
                    noticeDaysCheck < 0)
                {
                    errorMessage = "Please enter a valid notice period (0 or more days).";
                    return false;
                }

                if (string.IsNullOrWhiteSpace(txtLetterPreview.Text))
                {
                    errorMessage = "Termination letter cannot be empty.";
                    return false;
                }
                if (string.IsNullOrWhiteSpace(txtTerminationDate.Text))
                {
                    errorMessage = "Please select termination date.";
                    return false;
                }

                if (!DateTime.TryParse(txtTerminationDate.Text, out _))
                {
                    errorMessage = "Invalid termination date.";
                    return false;
                }
            }

            if (hfTerminationType.Value == "ShowCause")
            {
                if (string.IsNullOrWhiteSpace(txtShowCauseNoticeDays.Text) ||
                    !int.TryParse(txtShowCauseNoticeDays.Text.Trim(), out int showCauseNoticeDaysCheck) ||
                    showCauseNoticeDaysCheck < 0)
                {
                    errorMessage = "Please enter a valid notice days value (0 or more days).";
                    return false;
                }

                if (string.IsNullOrWhiteSpace(txtResponseDeadline.Text))
                {
                    errorMessage = "Response deadline is required.";
                    return false;
                }

                if (!DateTime.TryParse(txtResponseDeadline.Text, out _))
                {
                    errorMessage = "Invalid response deadline.";
                    return false;
                }

                if (string.IsNullOrWhiteSpace(txtNoticeLetter.Text))
                {
                    errorMessage = "Notice letter cannot be empty.";
                    return false;
                }

            }

            if (hfTerminationType.Value == "DirectTerminate")
            {
                if (string.IsNullOrWhiteSpace(txtDirectTerminationReason.Text))
                {
                    errorMessage = "Termination reason is required.";
                    return false;
                }

                if (string.IsNullOrWhiteSpace(txtTerminationDate.Text))
                {
                    errorMessage = "Please select termination date.";
                    return false;
                }

                if (!DateTime.TryParse(txtTerminationDate.Text, out _))
                {
                    errorMessage = "Invalid termination date.";
                    return false;
                }
            }

            return true;
        }

        private void SendTerminationEmail(TerminationProcessDO obj)
        {
            try
            {
                if (obj == null)
                    return;

                // "To" is always the employee's own mail id from the user master
                // (returned by the save SP as ToEmail); never hard-coded here.
                string employeeEmail = !string.IsNullOrWhiteSpace(obj.ToEmail) ? obj.ToEmail : obj.EmployeeEmail;
                string employeeName = obj.EmployeeName;

                if (string.IsNullOrEmpty(employeeEmail))
                    return; // Employee email unavailable - skip sending; save has already succeeded.

                // Subject/body are built by the save SP (EmailSubject/EmailBody) from
                // the same data it just persisted, so the HTML lives in one place in
                // the DB instead of being duplicated here. Fall back to a minimal
                // subject/body only if the SP hasn't been updated to return them yet.
                string subject = !string.IsNullOrWhiteSpace(obj.EmailSubject)
                    ? obj.EmailSubject
                    : "Employee Termination Letter";

                string body = !string.IsNullOrWhiteSpace(obj.EmailBody)
                    ? obj.EmailBody
                    : $"<p>Dear {employeeName},</p><p>This is to formally inform you that your employment has been terminated.</p>";

                string Email = ConfigurationManager.AppSettings["SenderEmail"];
                string Password = ConfigurationManager.AppSettings["SenderPassword"];
                int Port = Convert.ToInt32(ConfigurationManager.AppSettings["SenderPort"]);
                string Host = ConfigurationManager.AppSettings["SenderHost"];

                using (MailMessage mail = new MailMessage())
                {
                    mail.From = new MailAddress(Email, "HRMS System");
                    mail.To.Add(employeeEmail);

                    // CC = HR / reporting manager mail id resolved by the SP. If it
                    // isn't available, just send without CC rather than failing.
                    if (!string.IsNullOrWhiteSpace(obj.CCEmail))
                    {
                        foreach (string cc in obj.CCEmail.Split(new[] { ';', ',' }, StringSplitOptions.RemoveEmptyEntries))
                        {
                            string trimmedCc = cc.Trim();
                            if (!string.IsNullOrWhiteSpace(trimmedCc))
                                mail.CC.Add(trimmedCc);
                        }
                    }

                    mail.Subject = subject;
                    mail.Body = body;
                    mail.IsBodyHtml = true;

                    using (SmtpClient smtp = new SmtpClient(Host, Port))
                    {
                        smtp.UseDefaultCredentials = false;
                        smtp.Credentials = new NetworkCredential(Email, Password);
                        smtp.EnableSsl = true;
                        smtp.Send(mail);
                    }
                }
            }
            catch (Exception ex)
            {
                // Email failure must never fail the termination save - it has
                // already been committed by the time this method is called.
                // Still log it the same way every other module does, otherwise
                // a silent SMTP/config failure is invisible to HR.
                CommonBL errorlog = new CommonBL();
                errorlog.fnStoreErrorLog(
                    "EmployeeAction",
                    "SendTerminationEmail",
                    "Exception Message: " + ex.Message + " StackTrace: " + ex.StackTrace,
                    UserId
                );

                ScriptManager.RegisterStartupScript(
                    this, GetType(), "emailError",
                    $"console.error('Email Error: {ex.Message}');", true);
            }
        }


        private void ClearTerminationForm()
        {
            txtTerminationDate.Text = string.Empty;

            hfUserId.Value = string.Empty;
            hfEmployeeCode.Value = string.Empty;
            hfTerminationType.Value = string.Empty;
            hfPerformanceRating.Value = string.Empty;

            txtLetterPreview.Text = string.Empty;

            txtNoticePeriod.Text = "0";

            txtResponseDeadline.Text = string.Empty;
            txtNoticeLetter.Text = string.Empty;

            txtDirectTerminationReason.Text = string.Empty;
            txtDirectTerminationRemarks.Text = string.Empty;

        }

        private void ShowMessage(string msg, string type)
        {
            ScriptManager.RegisterStartupScript(
                this,
                GetType(),
                "alert",
                $"showUserSavedMessage('{type}','{msg}');",
                true
            );
        }

        protected void btnSendShowCause_Click(object sender, EventArgs e)
        {
            try
            {
                string empEmail = hfEmployeeEmail.Value;

                if (string.IsNullOrWhiteSpace(txtShowCauseNoticeDays.Text) ||
                    !int.TryParse(txtShowCauseNoticeDays.Text.Trim(), out int noticeDaysCheck) ||
                    noticeDaysCheck < 0)
                {
                    ShowMessage("Please enter a valid Notice Days value (0 or more days).", "error");

                    ScriptManager.RegisterStartupScript(this, GetType(), "openModal",
                        "var myModal = new bootstrap.Modal(document.getElementById('terminationModal')); myModal.show();", true);
                    return;
                }

                DateTime deadline = Convert.ToDateTime(txtResponseDeadline.Text);

                if (!DateTime.TryParse(txtResponseDeadline.Text, out deadline))
                {
                    ShowMessage("Response Deadline is not a valid date.", "error");

                    ScriptManager.RegisterStartupScript(this, GetType(), "openModal",
                "var myModal = new bootstrap.Modal(document.getElementById('terminationModal')); myModal.show();", true);
                    return;


                }

                string issue = txtNoticeLetter?.Text?.Trim() ?? string.Empty;
                if (string.IsNullOrEmpty(issue))
                {
                    ShowMessage("Please enter the Notice Letter.", "error");

                    // Keep modal open
                    ScriptManager.RegisterStartupScript(this, GetType(), "openModal",
                        "var myModal = new bootstrap.Modal(document.getElementById('terminationModal')); myModal.show();", true);
                    return;
                }

                if (string.IsNullOrEmpty(empEmail))
                {
                    ShowMessage("Employee email not found.", "error");

                    ScriptManager.RegisterStartupScript(this, GetType(), "openModal",
                "var myModal = new bootstrap.Modal(document.getElementById('terminationModal')); myModal.show();", true);
                    return;
                }

                TerminationProcessDO obj = new TerminationProcessDO();

                obj.CompanyId = Convert.ToInt32(hfCompanyId.Value);
                obj.UserId = Convert.ToInt32(hfUserId.Value);
                obj.EmployeeCode = hfEmployeeCode.Value;

                obj.NoticeLetter = issue;
                obj.ResponseDeadline = deadline;
                obj.NoticePeriodDays = int.TryParse(txtShowCauseNoticeDays.Text.Trim(), out int noticeDays) && noticeDays >= 0
                    ? noticeDays
                    : (int?)null;
                obj.InsertedBy = Convert.ToInt32(Session["UserId"]);

                HandoverprocessBL bl = new HandoverprocessBL();
                var result = bl.saveshowcausenotice(obj);

                if (result == null || result.Count == 0)
                {
                    ShowMessage("Unable to save the show cause notice.", "error");
                    return;
                }

                // Separate SP call, same pattern as the termination save flow -
                // looked up by UserId rather than the new row's id.
                var emailContent = bl.GetShowCauseEmailContent(obj.UserId);

                string toEmail = (emailContent != null && !string.IsNullOrWhiteSpace(emailContent.ToEmail))
                    ? emailContent.ToEmail
                    : empEmail;
                string subject = (emailContent != null && !string.IsNullOrWhiteSpace(emailContent.EmailSubject))
                    ? emailContent.EmailSubject
                    : "Show Cause Notice – Explanation Required";
                string body = (emailContent != null && !string.IsNullOrWhiteSpace(emailContent.EmailBody))
                    ? emailContent.EmailBody
                    : $"<p>Dear Employee,</p><p>A Show Cause Notice has been issued. Response deadline: {deadline:dd-MMM-yyyy}.</p>";

                string Email = ConfigurationManager.AppSettings["SenderEmail"];
                string Password = ConfigurationManager.AppSettings["SenderPassword"];
                int Port = Convert.ToInt32(ConfigurationManager.AppSettings["SenderPort"]);
                string Host = ConfigurationManager.AppSettings["SenderHost"];

                using (MailMessage mail = new MailMessage())
                {
                    mail.From = new MailAddress(Email, "HRMS System");
                    mail.To.Add(toEmail);
                    mail.Subject = subject;
                    mail.Body = body;
                    mail.IsBodyHtml = true;

                    using (SmtpClient smtp = new SmtpClient(Host, Port))
                    {
                        smtp.UseDefaultCredentials = false;
                        smtp.Credentials = new NetworkCredential(Email, Password);
                        smtp.EnableSsl = true;

                        smtp.Send(mail);
                    }
                }

                ShowMessage("Show Cause Notice is issued successfully.", "Success");

                // Refresh which of Send Show Cause Notice / Remove Termination
                // should be visible now, and keep the modal open on this tab so
                // HR sees the toggle flip immediately.
                LoadShowCauseButtonStatus();
                // Run after DOMContentLoaded (not immediately) - the page's own
                // DOMContentLoaded handler always resets the modal to the
                // Performance tab defaults on load, which would otherwise run
                // AFTER this script and flip Send Termination Notice / Escalate
                // to Termination back on top of Remove Termination.
                ScriptManager.RegisterStartupScript(this, GetType(), "reopenShowCause",
                    "window.addEventListener('DOMContentLoaded', function () { " +
                    "var m = new bootstrap.Modal(document.getElementById('terminationModal')); m.show(); showShowCause(); " +
                    "});", true);

            }
            catch (Exception ex)
            {
                CommonBL errorlog = new CommonBL();
                errorlog.fnStoreErrorLog(
                    "EmployeeAction",
                    "btnSendShowCause_Click",
                    "Exception Message: " + ex.Message + " StackTrace: " + ex.StackTrace,
                    UserId
                );

                ShowMessage(ex.Message, "error");
            }
        }

        protected void btnRemoveTermination_Click(object sender, EventArgs e)
        {
            try
            {
                int userId = Convert.ToInt32(hfUserId.Value);
                int updatedBy = Convert.ToInt32(Session["UserId"]);

                HandoverprocessBL bl = new HandoverprocessBL();
                bool removed = bl.RemoveShowCauseNotice(userId, updatedBy);

                if (removed)
                {
                    ShowMessage("Termination removed successfully.", "Success");
                }
                else
                {
                    ShowMessage("Unable to remove the termination.", "error");
                }

                // Reflects the actual saved state (is_active=0 now) - this is
                // the same lookup used everywhere else, not a separate flag.
                LoadShowCauseButtonStatus();
                // Run after DOMContentLoaded (not immediately) - the page's own
                // DOMContentLoaded handler always resets the modal to the
                // Performance tab defaults on load, which would otherwise run
                // AFTER this script and flip Send Termination Notice / Escalate
                // to Termination back on top of Remove Termination.
                ScriptManager.RegisterStartupScript(this, GetType(), "reopenShowCause",
                    "window.addEventListener('DOMContentLoaded', function () { " +
                    "var m = new bootstrap.Modal(document.getElementById('terminationModal')); m.show(); showShowCause(); " +
                    "});", true);
            }
            catch (Exception ex)
            {
                CommonBL errorlog = new CommonBL();
                errorlog.fnStoreErrorLog(
                    "EmployeeAction",
                    "btnRemoveTermination_Click",
                    "Exception Message: " + ex.Message + " StackTrace: " + ex.StackTrace,
                    UserId
                );

                ShowMessage(ex.Message, "error");
            }
        }

        public void LoadShowCauseButtonStatus()
        {
            string USERID = hfUserId.Value;

            if (string.IsNullOrEmpty(USERID))
                return;

            HandoverprocessBL bl = new HandoverprocessBL();

            string status = bl.GetShowCauseStatus(USERID);

            // Toggle, not two permanently visible buttons - which one shows is
            // driven entirely by the saved notice_status (SP_GetShowCauseStatus
            // only considers is_active=1 rows), so it's correct on every page
            // load/reopen, not just right after a click.
            bool showCauseIssued = (status == "Show Cause Issued" || status == "Responded" || status == "Response_pending");
            bool terminated = (status == "Terminated");

            btnSendShowCause.Visible = !showCauseIssued && !terminated;
            btnRemoveTermination.Visible = showCauseIssued;
            // Once actually terminated there's nothing left to escalate.
            btnEscalateShowCause.Visible = !terminated;
            // Only makes sense once a Show Cause Notice has actually been
            // sent - disabled beforehand, enabled once it's issued.
            btnEscalateShowCause.Enabled = showCauseIssued;
        }

        // Drives the Performance tab's Submit button through the 2 CAP rounds:
        // 1st save -> CAP1, 2nd -> CAP2, 3rd -> the actual termination. Same
        // saved-state-driven approach as LoadShowCauseButtonStatus, so the
        // right label/stage shows on every reopen, not just after a click.
        public void LoadPerformanceCapButtonStatus()
        {
            if (string.IsNullOrEmpty(hfUserId.Value))
                return;

            HandoverprocessBL bl = new HandoverprocessBL();
            string status = bl.GetEmployeeTerminationStatus(Convert.ToInt32(hfUserId.Value));

            // Escalate to Termination only makes sense once CAP2 has actually
            // been processed - disabled for None/CAP1, enabled from CAP2
            // onward (including after its target date expires).
            btnEscalateToTerminationPerf.Enabled =
                (status == "CAP2" || status == "CAP2_EXPIRED" || status == "Terminated");

            switch (status)
            {
                case "CAP1":
                    btnSendTerminationNotice.Text = "Save CAP 2";
                    btnSendTerminationNotice.Visible = true;
                    // HR can cancel right after CAP1 too, not just from CAP2 -
                    // SP_RemovePerformanceCap already clears both CAP1/CAP2.
                    btnRemovePerformanceCap.Visible = true;
                    hfCapStage.Value = "2";
                    break;
                case "CAP2":
                    // While CAP2's target date hasn't passed, HR can only
                    // cancel it here - the actual termination is decided by
                    // the scheduler once the window expires, not a click here.
                    btnSendTerminationNotice.Visible = false;
                    btnRemovePerformanceCap.Visible = true;
                    hfCapStage.Value = "Final";
                    break;
                case "CAP2_EXPIRED":
                    // CAP2's target date has passed - what happens next is
                    // handled by a separate scheduler, not from here.
                    btnSendTerminationNotice.Visible = false;
                    btnRemovePerformanceCap.Visible = false;
                    hfCapStage.Value = "Final";
                    break;
                case "Terminated":
                    // Already fully terminated - nothing left to record here.
                    btnSendTerminationNotice.Visible = false;
                    btnRemovePerformanceCap.Visible = false;
                    hfCapStage.Value = "Final";
                    break;
                default:
                    btnSendTerminationNotice.Text = "Save CAP 1";
                    btnSendTerminationNotice.Visible = true;
                    btnRemovePerformanceCap.Visible = false;
                    hfCapStage.Value = "1";
                    break;
            }
        }

        protected void btnRemovePerformanceCap_Click(object sender, EventArgs e)
        {
            try
            {
                int userId = Convert.ToInt32(hfUserId.Value);
                int updatedBy = Convert.ToInt32(Session["UserId"]);

                HandoverprocessBL bl = new HandoverprocessBL();
                bool removed = bl.RemovePerformanceCap(userId, updatedBy);

                if (removed)
                    ShowMessage("CAP termination removed successfully.", "Success");
                else
                    ShowMessage("Unable to remove the termination.", "error");

                // Reflects the actual saved state - falls back to "None" once
                // the CAP row is marked Removed, same lookup used everywhere.
                LoadPerformanceCapButtonStatus();
                ScriptManager.RegisterStartupScript(this, GetType(), "reopenPerformance",
                    "window.addEventListener('DOMContentLoaded', function () { " +
                    "var m = new bootstrap.Modal(document.getElementById('terminationModal')); m.show(); showPerformanceBased(); " +
                    "});", true);

                BindGridViewFromAPI(Convert.ToInt32(hfCompanyId.Value));
            }
            catch (Exception ex)
            {
                CommonBL errorlog = new CommonBL();
                errorlog.fnStoreErrorLog(
                    "EmployeeAction",
                    "btnRemovePerformanceCap_Click",
                    "Exception Message: " + ex.Message + " StackTrace: " + ex.StackTrace,
                    UserId
                );

                ShowMessage(ex.Message, "error");
            }
        }

        protected void gvEmployees_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "SelectEmployee")
            {
                int index = Convert.ToInt32(e.CommandArgument);

                var keys = gridview.DataKeys[index];

                hfUserId.Value = keys["UserId"].ToString();
                hfEmployeeCode.Value = keys["EmployeeCode"].ToString();
                hfEmployeeEmail.Value = keys["user_mail_id"].ToString();
                hfEmployeeName.Value = keys["user_fullname"].ToString();
                hfCompanyId.Value = keys["company_id"].ToString();

                LoadShowCauseButtonStatus();
                LoadPerformanceCapButtonStatus();

                ScriptManager.RegisterStartupScript(
                    this,
                    GetType(),
                    "openModal",
                    @"
            var myModal = new bootstrap.Modal(document.getElementById('terminationModal'));
            myModal.show();
            ",
                    true
                );
            }
        }

        // Same setup as gvEmployees_RowCommand's SelectEmployee branch, but
        // for an employee who no longer has a row in this grid (CAP1/CAP2/
        // Show Cause employees are only listed in Termination History now) -
        // looked up directly instead of from a clicked grid row.
        private void OpenManageModalForUser(int userId)
        {
            UserDetailsBL userDetailsBL = new UserDetailsBL();
            var user = userDetailsBL.ViewAllUsers().FirstOrDefault(u => u.UserId == userId);

            if (user == null)
                return;

            hfUserId.Value = user.UserId.ToString();
            hfEmployeeCode.Value = user.EmployeeCode;
            hfEmployeeEmail.Value = user.user_mail_id;
            hfEmployeeName.Value = user.user_fullname;
            hfCompanyId.Value = (user.company_id != 0 ? user.company_id : user.CompanyId).ToString();

            LoadShowCauseButtonStatus();
            LoadPerformanceCapButtonStatus();

            HandoverprocessBL bl = new HandoverprocessBL();
            string status = bl.GetEmployeeTerminationStatus(userId);
            string openTabScript = (status == "ShowCauseIssued") ? "showShowCause();" : "showPerformanceBased();";

            ScriptManager.RegisterStartupScript(
                this,
                GetType(),
                "openManageModal",
                "window.addEventListener('DOMContentLoaded', function () { " +
                "var myModal = new bootstrap.Modal(document.getElementById('terminationModal')); myModal.show(); " +
                openTabScript +
                " });",
                true
            );
        }



    }
}
