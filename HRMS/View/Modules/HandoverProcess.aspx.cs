using DataObject;
using ProcessModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Web;
using System.Web.Script.Serialization;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace HRMS.View.Modules
{
    public partial class HandoverProcess : System.Web.UI.Page
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

                BindHandOverGrid();
            }
        }

        protected void BindHandOverGrid()
        {
            try
            {
                int reportingManagerId = Convert.ToInt32(Session["userId"]);
                HandoverprocessBL handoverBL = new HandoverprocessBL();
                var resignations = handoverBL.GetEmployeeResignationDetails(reportingManagerId)
                    .Where(x => x.EmployeeResignationId > 0)
                    .OrderByDescending(x => x.EmployeeResignationId)
                    .Select(x => new HandOverDO
                    {
                        EmployeeResignationId = x.EmployeeResignationId,
                        UserId = x.UserId,
                        EmployeeName = x.EmployeeName,
                        EmployeeEmail = x.EmployeeEmail,
                        resignation_date = x.resignation_date,
                        last_working_date = x.last_working_date,
                        hr_status = string.IsNullOrWhiteSpace(x.hr_status) ? "Pending" : x.hr_status,
                        last_working_date_display = x.last_working_date == DateTime.MinValue
                            ? "-"
                            : x.last_working_date.ToString("yyyy-MM-dd")
                    })
                    .ToList();

                Session["HandoverListData"] = resignations;

                ApplySorting(ref resignations);

                int totalRecords = resignations.Count;
                int pageIndex = Convert.ToInt32(Session["CurrentPageIndex"] ?? 0);
                int totalPages = totalRecords > 0 ? (int)Math.Ceiling((double)totalRecords / 10) : 1;
                if (pageIndex < 0) pageIndex = 0;
                if (pageIndex >= totalPages) pageIndex = totalPages - 1;
                Session["CurrentPageIndex"] = pageIndex;
                hfPageIndexViewUser.Value = pageIndex.ToString();

                int pageSize = 10;
                int startRowIndex = pageIndex * pageSize;
                int endRowIndex = Math.Min(startRowIndex + pageSize, totalRecords);

                if (totalRecords > 0)
                {
                    List<HandOverDO> displayedData = resignations.GetRange(startRowIndex, endRowIndex - startRowIndex);

                    gvHandover.DataSource = displayedData;
                    gvHandover.DataBind();
                    gvHandover.Visible = true;

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
                    gvHandover.DataSource = null;
                    gvHandover.DataBind();
                    gvHandover.Visible = false;
                    ddlPageSelector.Visible = false;
                    UpdatePageInfoLabel(0, 0);
                }
            }
            catch (Exception ex)
            {
                gvHandover.Visible = false;
                CommonBL errorlog = new CommonBL();
                errorlog.fnStoreErrorLog("HandoverProcess", "BindHandOverGrid",
                    "Exception Message: " + ex.Message + " StackTrace: " + ex.StackTrace, UserId);
            }
        }



        protected List<HandOverDO> GetHandOverProcessFromAPI()
        {
            List<HandOverDO> users = new List<HandOverDO>();
            try
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri("http://103.118.17.144:813/");
                    //client.BaseAddress = new Uri("https://localhost:44360/");
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(
                        new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json")
                    );

                    HttpResponseMessage response =
        client.PostAsync(
            "UserList/GetHandOverProcessDetails",
            new StringContent("{}", Encoding.UTF8, "application/json")
        ).Result;


                    if (response.IsSuccessStatusCode)
                    {
                        var jsonString = response.Content.ReadAsStringAsync().Result;
                        JavaScriptSerializer js = new JavaScriptSerializer();
                        var result = js.Deserialize<HandOverResponseDO>(jsonString);

                        if (result.Success && result.HandOverProcessList != null)
                        {
                            users = result.HandOverProcessList.Select(u => new HandOverDO
                            {
                                EmployeeResignationId = u.EmployeeResignationId,
                                UserId = u.UserId,
                                EmployeeName = u.EmployeeName,
                                EmployeeEmail = u.EmployeeEmail,
                                resignation_date = u.resignation_date,
                                last_working_date = u.last_working_date,
                                hr_status = u.hr_status,
                                last_working_date_display = u.last_working_date_display
                            }).ToList();

                            UserDetailsBL userBL = new UserDetailsBL();
                            var userMap = userBL.ViewAllUsers()
                                .GroupBy(x => x.UserId)
                                .ToDictionary(g => g.Key, g => g.First());

                            foreach (var row in users)
                            {
                                UserDetailsDO userInfo;
                                if (userMap.TryGetValue(row.UserId, out userInfo))
                                {
                                    row.EmployeeName = string.IsNullOrWhiteSpace(userInfo.user_fullname) ? row.EmployeeName : userInfo.user_fullname;
                                    row.EmployeeEmail = string.IsNullOrWhiteSpace(userInfo.user_mail_id) ? row.EmployeeEmail : userInfo.user_mail_id;
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // log error
            }
            return users;
        }

        public int TotalRecordCount()
        {
            var handovers = Session["HandoverListData"] as List<HandOverDO>;
            if (handovers == null)
            {
                int reportingManagerId = Convert.ToInt32(Session["userId"]);
                handovers = new HandoverprocessBL().GetEmployeeResignationDetails(reportingManagerId)
                    .Where(x => x.EmployeeResignationId > 0)
                    .Select(x => new HandOverDO
                    {
                        EmployeeResignationId = x.EmployeeResignationId,
                        UserId = x.UserId
                    })
                    .ToList();
            }
            return handovers.Count;
        }
        protected void ddlPageSelector_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                int selectedPageIndex = Convert.ToInt32(ddlPageSelector.SelectedValue);
                Session["CurrentPageIndex"] = selectedPageIndex;

                if (Session["AdvSearchResViewUser"] != null)
                {
                    List<HandOverDO> searchResults = (List<HandOverDO>)Session["AdvSearchResViewUser"];
                    //searchResults = searchResults.OrderByDescending(t => t.Inserteddate).ToList();
                    ApplySorting(ref searchResults);

                    int totalRecords = searchResults.Count;
                    int pageIndex = selectedPageIndex;
                    hfPageIndexViewUser.Value = pageIndex.ToString();

                    int pageSize = gvHandover.PageSize;
                    int startRowIndex = pageIndex * pageSize;
                    int endRowIndex = Math.Min(startRowIndex + pageSize, totalRecords);

                    List<HandOverDO> displayedUsers = searchResults.GetRange(startRowIndex, endRowIndex - startRowIndex);
                    gvHandover.DataSource = displayedUsers;
                    gvHandover.DataBind();

                    UpdatePageInfoLabel(pageIndex, totalRecords);
                }
                else
                {
                    int companyId = Convert.ToInt32(Session["SelectedCompanyId"]);
                    BindHandOverGrid();

                }
            }
            catch (Exception ex)
            {
                CommonBL errorlog = new CommonBL();
                errorlog.fnStoreErrorLog("HandoverProcess", "ddlPageSelector_SelectedIndexChanged", "Exception Message" + ex.Message + "Strace=" + ex.StackTrace, UserId);
            }
        }
        protected void OnPageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            gvHandover.PageIndex = e.NewPageIndex;
            BindHandOverGrid();
        }
        protected void gridview_Sorting(object sender, GridViewSortEventArgs e)
        {
            try
            {
                List<HandOverDO> createdet = Session["HandoverListData"] as List<HandOverDO>;
                if (createdet == null)
                {
                    int reportingManagerId = Convert.ToInt32(Session["userId"]);
                    createdet = new HandoverprocessBL().GetEmployeeResignationDetails(reportingManagerId)
                        .Where(x => x.EmployeeResignationId > 0)
                        .Select(x => new HandOverDO
                        {
                            EmployeeResignationId = x.EmployeeResignationId,
                            UserId = x.UserId,
                            EmployeeName = x.EmployeeName,
                            EmployeeEmail = x.EmployeeEmail,
                            resignation_date = x.resignation_date,
                            last_working_date = x.last_working_date,
                            hr_status = string.IsNullOrWhiteSpace(x.hr_status) ? "Pending" : x.hr_status,
                            last_working_date_display = x.last_working_date == DateTime.MinValue
                                ? "-"
                                : x.last_working_date.ToString("yyyy-MM-dd")
                        })
                        .ToList();
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

                    gvHandover.DataSource = createdet;
                    gvHandover.DataBind();
                }
            }

            catch (Exception ex)
            {
                CommonBL errorlog = new CommonBL();
                errorlog.fnStoreErrorLog("HandoverProcess", "gridview_Sorting", "Exception Message" + ex.Message + "Strace=" + ex.StackTrace, UserId);
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
        private void ApplySorting(ref List<HandOverDO> users)
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
                errorlog.fnStoreErrorLog("HandoverProcess", "ApplySorting", "Exception Message" + ex.Message + "Strace=" + ex.StackTrace, UserId);
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
                errorlog.fnStoreErrorLog("HandoverProcess", "UpdatePageInfoLabel", "Exception Message" + ex.Message + "Strace=" + ex.StackTrace, UserId);
            }
        }

        //protected void gvUsers_RowCommand(object sender, GridViewCommandEventArgs e)
        //{
        //    if (e.CommandName == "Open")
        //    {
        //        int resignationId = Convert.ToInt32(e.CommandArgument);


        //        Response.Redirect(
        //            $"ViewHandoverProcess.aspx?rid={resignationId}",
        //            false
        //        );
        //    }
        //}
        protected void gvUsers_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "Open")
            {
                // Split CommandArgument to get both IDs
                string[] args = e.CommandArgument.ToString().Split('|');
                int resignationId = Convert.ToInt32(args[0]);
                int userId = Convert.ToInt32(args[1]);

                if (resignationId <= 0)
                {
                    ScriptManager.RegisterStartupScript(
                        this, GetType(),
                        "noHandover",
                        "showUserSavedMessage('Error', 'No handover request found for this user.');", true);
                    return;
                }

                Response.Redirect(
                    $"ViewHandoverProcess.aspx?rid={resignationId}&uid={userId}",
                    false
                );
            }
        }

    }
}
