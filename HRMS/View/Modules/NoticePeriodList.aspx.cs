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

        protected void BindNoticePeriodGrid()
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
                errorlog.fnStoreErrorLog("NoticePeriodList", "BindNoticePeriodGrid",
                    "Exception Message: " + ex.Message + " StackTrace: " + ex.StackTrace, UserId);
            }
        }

        protected void ddlPageSelector_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                int selectedPageIndex = Convert.ToInt32(ddlPageSelector.SelectedValue);
                Session["NoticePeriodCurrentPageIndex"] = selectedPageIndex;
                BindNoticePeriodGrid();
            }
            catch (Exception ex)
            {
                CommonBL errorlog = new CommonBL();
                errorlog.fnStoreErrorLog("NoticePeriodList", "ddlPageSelector_SelectedIndexChanged", "Exception Message" + ex.Message + "Strace=" + ex.StackTrace, UserId);
            }
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
                int totalPages = (int)Math.Ceiling((double)pagecount / 10);
                ddlPageSelector.Items.Clear();
                for (int i = 1; i <= totalPages; i++)
                {
                    ddlPageSelector.Items.Add(new ListItem($"{i}/{totalPages}", (i - 1).ToString()));
                }
                if (ddlPageSelector.Items.Count > 0)
                {
                    ddlPageSelector.SelectedValue = pageIndex.ToString();
                }
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
