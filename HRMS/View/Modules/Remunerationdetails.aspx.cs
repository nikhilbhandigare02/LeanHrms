using DataObject;
using ProcessModel;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace HRMS.View.Modules
{
    public partial class Remunerationdetails : System.Web.UI.Page
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
                BindYears();
            }
        }

        public void BindYears()
        {
            ddlYear.Items.Clear();
            ddlYear.Items.Add(new ListItem("-- Select Year --", ""));

            int currentYear = DateTime.Now.Year;

            for (int year = currentYear - 5; year <= currentYear + 5; year++)
            {
                ddlYear.Items.Add(new ListItem(year.ToString(), year.ToString()));
            }
        }

        protected void BindGrid(int? year)
        {
            try
            {
                SalarySlipBL bl = new SalarySlipBL();
                List<RemunerationDO> list = bl.GetRemunerationDetails(year);

                // Apply sorting if needed
                ApplySorting(ref list);

                int totalRecords = list.Count;
                int pageSize = 10;

                // Get current page index from session, reset if out of range
                int pageIndex = Convert.ToInt32(Session["CurrentPageIndex"] ?? 0);
                if (pageIndex * pageSize >= totalRecords)
                {
                    pageIndex = 0;
                    Session["CurrentPageIndex"] = 0;
                }

                hfPageIndexViewUser.Value = pageIndex.ToString();

                int startRowIndex = pageIndex * pageSize;
                int endRowIndex = Math.Min(startRowIndex + pageSize, totalRecords);

                // Prepare data for current page
                List<RemunerationDO> displayedData = new List<RemunerationDO>();
                if (totalRecords > 0 && startRowIndex < totalRecords)
                {
                    displayedData = list.GetRange(startRowIndex, endRowIndex - startRowIndex);
                }

                // Bind data to GridView
                gridview.DataSource = displayedData;
                gridview.DataBind();

                // Show/hide controls based on data
                bool hasData = displayedData.Count > 0;
                gridview.Visible = hasData;
                paginationContainer.Visible = hasData;
                ddlPageSelector.Visible = hasData && totalRecords > pageSize;

                UpdatePageInfoLabel(pageIndex, totalRecords);
            }
            catch (Exception ex)
            {
                // Hide grid and pagination on error
                gridview.Visible = false;
                paginationContainer.Visible = false;
                ddlPageSelector.Visible = false;

                // Log error
                CommonBL errorlog = new CommonBL();
                errorlog.fnStoreErrorLog(
                    "Remuneration",
                    "BindGrid",
                    "Exception Message: " + ex.Message + " StackTrace: " + ex.StackTrace,
                    UserId
                );
            }
        }

        protected void btnsearch_click(object sender, EventArgs e)
        {
            int? year = null;
            if (!string.IsNullOrEmpty(ddlYear.SelectedValue))
            {
                year = Convert.ToInt32(ddlYear.SelectedValue);
            }
            else
            {
                ClientScript.RegisterStartupScript(
                    GetType(),
                    "msg",
                    "Swal.fire('Warning','Please select year.','warning')",
                    true);
                return;
            }

            gridview.PageIndex = 0;
            BindGrid(year);
        }

        protected void btnclear_click(object sender, EventArgs e)
        {
            try
            {
                ddlYear.SelectedIndex = 0;
                gridview.DataSource = null;
                gridview.DataBind();
                gridview.Visible = false;
                ddlPageSelector.Visible = false;
            }
            catch (Exception ex)
            {
                ClientScript.RegisterStartupScript(this.GetType(), "Error",
                    $"Swal.fire('Error','{HttpUtility.JavaScriptStringEncode(ex.Message)}','error')", true);
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
                errorlog.fnStoreErrorLog("useruploaddocuments", "UpdatePageInfoLabel", "Exception Message" + ex.Message + "Strace=" + ex.StackTrace, UserId);
            }
        }

        protected void ddlPageSelector_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                int selectedPageIndex = Convert.ToInt32(ddlPageSelector.SelectedValue);
                Session["CurrentPageIndex"] = selectedPageIndex;
                int? year = string.IsNullOrEmpty(ddlYear.SelectedValue) ? (int?)null : Convert.ToInt32(ddlYear.SelectedValue);
                BindGrid(year);
            }
            catch (Exception ex)
            {
                CommonBL errorlog = new CommonBL();
                errorlog.fnStoreErrorLog("useruploaddocuments", "ddlPageSelector_SelectedIndexChanged", "Exception Message" + ex.Message + "Strace=" + ex.StackTrace, UserId);
            }
        }

        protected void OnPageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            gridview.PageIndex = e.NewPageIndex;
            Session["CurrentPageIndex"] = e.NewPageIndex;
            int? year = string.IsNullOrEmpty(ddlYear.SelectedValue) ? (int?)null : Convert.ToInt32(ddlYear.SelectedValue);
            BindGrid(year);
        }

        protected void gridview_Sorting(object sender, GridViewSortEventArgs e)
        {
            try
            {
                int? year = string.IsNullOrEmpty(ddlYear.SelectedValue) ? (int?)null : Convert.ToInt32(ddlYear.SelectedValue);
                SalarySlipBL userDetailsBL = new SalarySlipBL();
                List<RemunerationDO> createdet = userDetailsBL.GetRemunerationDetails(year);

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
            }
            catch (Exception ex)
            {
                CommonBL errorlog = new CommonBL();
                errorlog.fnStoreErrorLog("useruploaddocuments", "gridview_Sorting", "Exception Message" + ex.Message + "Strace=" + ex.StackTrace, UserId);
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

        private void ApplySorting(ref List<RemunerationDO> users)
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
                errorlog.fnStoreErrorLog("useruploaddocuments", "ApplySorting", "Exception Message" + ex.Message + "Strace=" + ex.StackTrace, UserId);
            }
        }

        protected void btnView_Click(object sender, EventArgs e)
        {
            LinkButton btn = (LinkButton)sender;
            int remunerationId = Convert.ToInt32(btn.CommandArgument);
            Response.Redirect("~/View/Modules/Remunerationform.aspx?id=" + remunerationId + "&mode=view");
        }

        protected void btnEdit_Click(object sender, EventArgs e)
        {
            LinkButton btn = (LinkButton)sender;
            int remunerationId = Convert.ToInt32(btn.CommandArgument);
            Response.Redirect("~/View/Modules/Remunerationform.aspx?id=" + remunerationId + "&mode=edit");
        }

        protected void btnDelete_Click(object sender, EventArgs e)
        {
            try
            {
                LinkButton btn = (LinkButton)sender;
                int remunerationId = Convert.ToInt32(btn.CommandArgument);
                
                renumarationBL bl = new renumarationBL();
                var result = bl.DeactivateRemuneration(remunerationId);

                if (result.Status.Equals("Success", StringComparison.OrdinalIgnoreCase))
                {
                    ClientScript.RegisterStartupScript(
                        GetType(),
                        "msg",
                        $"Swal.fire('Success','{HttpUtility.JavaScriptStringEncode(result.Message)}','success')",
                        true);
                }
                else
                {
                    ClientScript.RegisterStartupScript(
                        GetType(),
                        "error",
                        $"Swal.fire('Error','{HttpUtility.JavaScriptStringEncode(result.Message)}','error')",
                        true);
                }

                // Refresh grid
                int? year = string.IsNullOrEmpty(ddlYear.SelectedValue) ? (int?)null : Convert.ToInt32(ddlYear.SelectedValue);
                BindGrid(year);
            }
            catch (Exception ex)
            {
                CommonBL errorlog = new CommonBL();
                errorlog.fnStoreErrorLog("Remuneration", "Delete", "Exception Message: " + ex.Message + " StackTrace: " + ex.StackTrace, UserId);
                ClientScript.RegisterStartupScript(
                    GetType(),
                    "error",
                    $"Swal.fire('Error','{HttpUtility.JavaScriptStringEncode(ex.Message)}','error')",
                    true);
            }
        }

        protected void gridview_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            // Keep this for backward compatibility
        }
    }
}