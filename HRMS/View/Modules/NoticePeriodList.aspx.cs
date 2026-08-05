using DataObject;
using ProcessModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace HRMS.View.Modules
{
    public partial class NoticePeriodList : System.Web.UI.Page
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

                BindNoticePeriodGrid();
            }
        }

        private static bool IsHrAccepted(ResignationDO resignation)
        {
            return resignation != null
                && string.Equals((resignation.hr_status ?? string.Empty).Trim(), "Accepted", StringComparison.OrdinalIgnoreCase);
        }

        // The Last Working Date column must always show the Notice End Date
        // (tbl_hr_review.CreatedDate + NoticeDays, same calculation as
        // NoticePeriodManagement's sp_GetNoticePeriodDetails) rather than the
        // resignation's originally proposed last_working_date.
        private static void ApplyNoticeEndDateDisplay(HandoverprocessBL resignationBL, List<ResignationDO> resignations)
        {
            foreach (var row in resignations)
            {
                NoticePeriodDO noticePeriod = resignationBL.GetNoticePeriodDetails(row.EmployeeResignationId);
                row.last_working_date_display = (noticePeriod != null && noticePeriod.NoticeEndDate.HasValue)
                    ? noticePeriod.NoticeEndDate.Value.ToString("yyyy-MM-dd")
                    : "-";
            }
        }

        protected void BindNoticePeriodGrid()
        {
            try
            {
                int reportingManagerId = Convert.ToInt32(Session["userId"]);
                HandoverprocessBL resignationBL = new HandoverprocessBL();
                var resignations = resignationBL.GetEmployeeResignationDetails(reportingManagerId)
                    .Where(IsHrAccepted)
                    .OrderByDescending(x => x.EmployeeResignationId)
                    .ToList();

                ApplyNoticeEndDateDisplay(resignationBL, resignations);

                Session["NoticePeriodListData"] = resignations;

                ApplySorting(ref resignations);

                int totalRecords = resignations.Count;
                int pageIndex = Convert.ToInt32(Session["NoticePeriodCurrentPageIndex"] ?? 0);
                int pageSize = 10;
                int totalPages = totalRecords > 0 ? (int)Math.Ceiling((double)totalRecords / pageSize) : 1;
                if (pageIndex < 0) pageIndex = 0;
                if (pageIndex >= totalPages) pageIndex = totalPages - 1;
                Session["NoticePeriodCurrentPageIndex"] = pageIndex;
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
                        UpdatePageInfoLabel(pageIndex, totalRecords);
                    }
                    else
                    {
                        paginationContainer.Visible = false;
                    }
                }
                else
                {
                    gvResignations.DataSource = null;
                    gvResignations.DataBind();
                    gvResignations.Visible = true;
                    paginationContainer.Visible = false;
                    UpdatePageInfoLabel(0, 0);
                }
            }
            catch (Exception ex)
            {
                gvResignations.Visible = true;
                gvResignations.DataSource = null;
                gvResignations.DataBind();
                CommonBL errorlog = new CommonBL();
                errorlog.fnStoreErrorLog("NoticePeriodList", "BindNoticePeriodGrid",
                    "Exception Message: " + ex.Message + " StackTrace: " + ex.StackTrace, UserId);
            }
        }

        protected void rptPageNumbers_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            try
            {
                if (e.CommandName == "GoToPage")
                {
                    Session["NoticePeriodCurrentPageIndex"] = Convert.ToInt32(e.CommandArgument);
                    BindNoticePeriodGrid();
                }
            }
            catch (Exception ex)
            {
                CommonBL errorlog = new CommonBL();
                errorlog.fnStoreErrorLog("NoticePeriodList", "rptPageNumbers_ItemCommand", "Exception Message" + ex.Message + "Strace=" + ex.StackTrace, UserId);
            }
        }

        protected void lnkPrevPage_Click(object sender, EventArgs e)
        {
            try
            {
                int pageIndex = Convert.ToInt32(Session["NoticePeriodCurrentPageIndex"] ?? 0);
                if (pageIndex > 0)
                {
                    Session["NoticePeriodCurrentPageIndex"] = pageIndex - 1;
                    BindNoticePeriodGrid();
                }
            }
            catch (Exception ex)
            {
                CommonBL errorlog = new CommonBL();
                errorlog.fnStoreErrorLog("NoticePeriodList", "lnkPrevPage_Click", "Exception Message" + ex.Message + "Strace=" + ex.StackTrace, UserId);
            }
        }

        protected void lnkNextPage_Click(object sender, EventArgs e)
        {
            try
            {
                int pageIndex = Convert.ToInt32(Session["NoticePeriodCurrentPageIndex"] ?? 0);
                Session["NoticePeriodCurrentPageIndex"] = pageIndex + 1;
                BindNoticePeriodGrid();
            }
            catch (Exception ex)
            {
                CommonBL errorlog = new CommonBL();
                errorlog.fnStoreErrorLog("NoticePeriodList", "lnkNextPage_Click", "Exception Message" + ex.Message + "Strace=" + ex.StackTrace, UserId);
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
            gvResignations.PageIndex = e.NewPageIndex;
            BindNoticePeriodGrid();
        }

        protected void gridview_Sorting(object sender, GridViewSortEventArgs e)
        {
            try
            {
                List<ResignationDO> createdet = Session["NoticePeriodListData"] as List<ResignationDO>;
                if (createdet == null)
                {
                    int reportingManagerId = Convert.ToInt32(Session["userId"]);
                    HandoverprocessBL resignationBL = new HandoverprocessBL();
                    createdet = resignationBL.GetEmployeeResignationDetails(reportingManagerId)
                        .Where(IsHrAccepted)
                        .ToList();
                    ApplyNoticeEndDateDisplay(resignationBL, createdet);
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
                errorlog.fnStoreErrorLog("NoticePeriodList", "gridview_Sorting", "Exception Message" + ex.Message + "Strace=" + ex.StackTrace, UserId);
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
                errorlog.fnStoreErrorLog("NoticePeriodList", "ApplySorting", "Exception Message" + ex.Message + "Strace=" + ex.StackTrace, UserId);
            }
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
                errorlog.fnStoreErrorLog("NoticePeriodList", "UpdatePageInfoLabel", "Exception Message" + ex.Message + "Strace=" + ex.StackTrace, UserId);
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
                        "showUserSavedMessage('Error', 'No record found for this user.');", true);
                    return;
                }

                if (e.CommandName == "ViewNotice")
                {
                    Response.Redirect("~/View/Modules/NoticePeriodManagement.aspx?ResignationId=" + resignationId, false);
                    return;
                }
            }
            catch (Exception ex)
            {
                CommonBL errorlog = new CommonBL();
                errorlog.fnStoreErrorLog(
                    "NoticePeriodList",
                    "gvUsers_RowCommand",
                    ex.Message,
                    UserId);
            }
        }
    }
}
