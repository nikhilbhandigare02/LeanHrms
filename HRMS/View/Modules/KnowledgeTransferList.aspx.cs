using DataObject;
using ProcessModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace HRMS.View.Modules
{
    public partial class KnowledgeTransferList : System.Web.UI.Page
    {
        protected string UserId = null;

        private List<KTProjectHandoverRowDO> ProjectRows
        {
            get
            {
                var rows = ViewState["KTProjectRows"] as List<KTProjectHandoverRowDO>;
                if (rows == null)
                {
                    rows = new List<KTProjectHandoverRowDO>();
                    ViewState["KTProjectRows"] = rows;
                }
                return rows;
            }
            set
            {
                ViewState["KTProjectRows"] = value;
            }
        }

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

                BindKTListGrid();
            }
        }

        private static bool IsHrAccepted(ResignationDO resignation)
        {
            return resignation != null
                && string.Equals((resignation.hr_status ?? string.Empty).Trim(), "Accepted", StringComparison.OrdinalIgnoreCase);
        }

        protected void BindKTListGrid()
        {
            try
            {
                int reportingManagerId = Convert.ToInt32(Session["userId"]);
                HandoverprocessBL resignationBL = new HandoverprocessBL();
                var resignations = resignationBL.GetEmployeeResignationDetails(reportingManagerId)
                    .Where(IsHrAccepted)
                    .OrderByDescending(x => x.EmployeeResignationId)
                    .ToList();

                Session["KTListData"] = resignations;

                ApplySorting(ref resignations);

                int totalRecords = resignations.Count;
                int pageIndex = Convert.ToInt32(Session["KTListCurrentPageIndex"] ?? 0);
                int pageSize = 10;
                int totalPages = totalRecords > 0 ? (int)Math.Ceiling((double)totalRecords / pageSize) : 1;
                if (pageIndex < 0) pageIndex = 0;
                if (pageIndex >= totalPages) pageIndex = totalPages - 1;
                Session["KTListCurrentPageIndex"] = pageIndex;
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
                errorlog.fnStoreErrorLog("KnowledgeTransferList", "BindKTListGrid",
                    "Exception Message: " + ex.Message + " StackTrace: " + ex.StackTrace, UserId);
            }
        }

        protected void ddlPageSelector_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                int selectedPageIndex = Convert.ToInt32(ddlPageSelector.SelectedValue);
                Session["KTListCurrentPageIndex"] = selectedPageIndex;
                BindKTListGrid();
            }
            catch (Exception ex)
            {
                CommonBL errorlog = new CommonBL();
                errorlog.fnStoreErrorLog("KnowledgeTransferList", "ddlPageSelector_SelectedIndexChanged", "Exception Message" + ex.Message + "Strace=" + ex.StackTrace, UserId);
            }
        }

        protected void OnPageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            gvResignations.PageIndex = e.NewPageIndex;
            BindKTListGrid();
        }

        protected void gridview_Sorting(object sender, GridViewSortEventArgs e)
        {
            try
            {
                List<ResignationDO> createdet = Session["KTListData"] as List<ResignationDO>;
                if (createdet == null)
                {
                    int reportingManagerId = Convert.ToInt32(Session["userId"]);
                    HandoverprocessBL resignationBL = new HandoverprocessBL();
                    createdet = resignationBL.GetEmployeeResignationDetails(reportingManagerId)
                        .Where(IsHrAccepted)
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

                    gvResignations.DataSource = createdet;
                    gvResignations.DataBind();
                }
            }
            catch (Exception ex)
            {
                CommonBL errorlog = new CommonBL();
                errorlog.fnStoreErrorLog("KnowledgeTransferList", "gridview_Sorting", "Exception Message" + ex.Message + "Strace=" + ex.StackTrace, UserId);
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
                errorlog.fnStoreErrorLog("KnowledgeTransferList", "ApplySorting", "Exception Message" + ex.Message + "Strace=" + ex.StackTrace, UserId);
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
                errorlog.fnStoreErrorLog("KnowledgeTransferList", "UpdatePageInfoLabel", "Exception Message" + ex.Message + "Strace=" + ex.StackTrace, UserId);
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

                if (e.CommandName == "OpenKT")
                {
                    ShowKTForm(resignationId);
                    return;
                }
            }
            catch (Exception ex)
            {
                CommonBL errorlog = new CommonBL();
                errorlog.fnStoreErrorLog(
                    "KnowledgeTransferList",
                    "gvUsers_RowCommand",
                    ex.Message,
                    UserId);
            }
        }

        // ==================== KT FORM (same page - toggled via panel visibility,
        // so only ONE page name needs authorization/menu permission mapping) ====================

        private void ShowKTForm(int resignationId)
        {
            hfResignationId.Value = resignationId.ToString();
            pnlKTList.Visible = false;
            pnlKTForm.Visible = true;
            BindKTForm(resignationId);
        }

        private void ShowKTList()
        {
            pnlKTForm.Visible = false;
            pnlKTList.Visible = true;
            BindKTListGrid();
        }

        private void BindKTForm(int resignationId)
        {
            try
            {
                HandoverprocessBL bl = new HandoverprocessBL();

                // Reuse the existing HR Review lookup for the read-only employee /
                // resignation header info - avoids a duplicate employee query.
                HRReviewDO employeeInfo = bl.GetHRReviewDetails(resignationId);
                lblEmployeeId.Text = employeeInfo.EmployeeId > 0 ? employeeInfo.EmployeeId.ToString() : "-";
                lblEmployeeName.Text = string.IsNullOrWhiteSpace(employeeInfo.EmployeeName) ? "-" : employeeInfo.EmployeeName;
                lblDepartment.Text = string.IsNullOrWhiteSpace(employeeInfo.Department) ? "-" : employeeInfo.Department;
                lblDesignation.Text = string.IsNullOrWhiteSpace(employeeInfo.Designation) ? "-" : employeeInfo.Designation;
                lblResignationDate.Text = employeeInfo.ResignationDate == DateTime.MinValue ? "-" : employeeInfo.ResignationDate.ToString("dd-MM-yyyy");
                lblProposedLastWorkingDate.Text = employeeInfo.ProposedLastWorkingDate == DateTime.MinValue ? "-" : employeeInfo.ProposedLastWorkingDate.ToString("dd-MM-yyyy");

                KTHandoverDO existingKT = bl.GetKTHandoverByResignationId(resignationId);

                if (existingKT == null)
                {
                    // No KT record yet - blank form ready for entry.
                    hfKTId.Value = "0";
                    txtKTPlan.Text = string.Empty;
                    txtReplacementEmployee.Text = string.Empty;
                    ddlKTStatus.SelectedValue = "";
                    txtKTStartDate.Text = string.Empty;
                    txtKTCompletionDate.Text = string.Empty;
                    ProjectRows = new List<KTProjectHandoverRowDO>();
                    btnSaveKT.Text = "Save";
                }
                else
                {
                    hfKTId.Value = existingKT.KTId.ToString();
                    txtKTPlan.Text = existingKT.KTPlan;
                    txtReplacementEmployee.Text = existingKT.ReplacementEmployee;
                    SetDropDownValueSafe(ddlKTStatus, existingKT.KTStatus);
                    txtKTStartDate.Text = existingKT.KTStartDate.HasValue ? existingKT.KTStartDate.Value.ToString("yyyy-MM-dd") : string.Empty;
                    txtKTCompletionDate.Text = existingKT.KTCompletionDate.HasValue ? existingKT.KTCompletionDate.Value.ToString("yyyy-MM-dd") : string.Empty;

                    ProjectRows = bl.GetKTProjectHandoverRows(existingKT.KTId);
                    btnSaveKT.Text = "Update";
                }

                txtNewProjectName.Text = string.Empty;
                txtNewAssignedEmployee.Text = string.Empty;
                ddlNewProjectStatus.SelectedValue = "";

                BindProjectGrid();
            }
            catch (Exception ex)
            {
                CommonBL errorlog = new CommonBL();
                errorlog.fnStoreErrorLog("KnowledgeTransferList", "BindKTForm",
                    "Exception Message: " + ex.Message + " StackTrace: " + ex.StackTrace, UserId);
            }
        }

        private static void SetDropDownValueSafe(DropDownList ddl, string value)
        {
            if (!string.IsNullOrWhiteSpace(value) && ddl.Items.FindByValue(value) != null)
            {
                ddl.SelectedValue = value;
            }
        }

        private void BindProjectGrid()
        {
            gvProjectHandover.DataSource = ProjectRows;
            gvProjectHandover.DataBind();
        }

        protected void btnAddProjectRow_Click(object sender, EventArgs e)
        {
            // rfvNewProjectName/rfvNewAssignedEmployee/rfvNewProjectStatus (ValidationGroup
            // "ProjectRowGroup") already show the inline below-field messages and block
            // this postback client-side; Page.IsValid is the server-side safety net.
            if (!Page.IsValid)
            {
                return;
            }

            string projectName = txtNewProjectName.Text.Trim();
            string assignedEmployee = txtNewAssignedEmployee.Text.Trim();
            string status = ddlNewProjectStatus.SelectedValue;

            var rows = ProjectRows;
            rows.Add(new KTProjectHandoverRowDO
            {
                ProjectName = projectName,
                AssignedEmployee = assignedEmployee,
                Status = status
            });
            ProjectRows = rows;

            txtNewProjectName.Text = string.Empty;
            txtNewAssignedEmployee.Text = string.Empty;
            ddlNewProjectStatus.SelectedValue = "";

            BindProjectGrid();
        }

        protected void gvProjectHandover_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "RemoveRow")
            {
                int index;
                if (int.TryParse(e.CommandArgument.ToString(), out index))
                {
                    var rows = ProjectRows;
                    if (index >= 0 && index < rows.Count)
                    {
                        KTProjectHandoverRowDO row = rows[index];

                        // Only rows already persisted (loaded from an existing KT record)
                        // have a real KTProjectHandoverId - a row just added in this same
                        // session and not yet saved has nothing in the DB to delete yet.
                        if (row.KTProjectHandoverId > 0)
                        {
                            HandoverprocessBL bl = new HandoverprocessBL();
                            KTHandoverResponseDO result = bl.DeleteKTProjectHandoverRow(row.KTProjectHandoverId);

                            if (result == null || !result.Success)
                            {
                                string safeErr = System.Web.HttpUtility.JavaScriptStringEncode(
                                    result?.ResponseMsg ?? "Unable to delete knowledge transfer record.");
                                ScriptManager.RegisterStartupScript(this, GetType(), "ktRowDeleteError",
                                    $"showKTResult('Error', '{safeErr}');", true);
                                return;
                            }

                            string safeMsg = System.Web.HttpUtility.JavaScriptStringEncode(
                                result.ResponseMsg ?? "Knowledge transfer deleted successfully.");
                            ScriptManager.RegisterStartupScript(this, GetType(), "ktRowDeleted",
                                $"showKTResult('Success', '{safeMsg}');", true);
                        }

                        rows.RemoveAt(index);
                        ProjectRows = rows;
                        BindProjectGrid();
                    }
                }
            }
        }

        protected void btnSaveKT_Click(object sender, EventArgs e)
        {
            // rfvKTPlan/rfvReplacementEmployee/rfvKTStatus (ValidationGroup "KTMainGroup")
            // already show the inline below-field messages and block this postback
            // client-side; Page.IsValid is the server-side safety net.
            if (!Page.IsValid)
            {
                return;
            }

            int resignationId;
            int.TryParse(hfResignationId.Value, out resignationId);

            string ktPlan = txtKTPlan.Text.Trim();
            string replacementEmployee = txtReplacementEmployee.Text.Trim();
            string ktStatus = ddlKTStatus.SelectedValue;

            if (resignationId <= 0)
            {
                ScriptManager.RegisterStartupScript(this, GetType(), "invalidResignation",
                    "alert('Invalid resignation request.');", true);
                return;
            }

            DateTime? ktStartDate = null;
            DateTime parsedStart;
            if (DateTime.TryParse(txtKTStartDate.Text.Trim(), out parsedStart))
            {
                ktStartDate = parsedStart;
            }

            DateTime? ktCompletionDate = null;
            DateTime parsedCompletion;
            if (DateTime.TryParse(txtKTCompletionDate.Text.Trim(), out parsedCompletion))
            {
                ktCompletionDate = parsedCompletion;
            }

            int ktId;
            int.TryParse(hfKTId.Value, out ktId);

            var model = new KTHandoverDO
            {
                KTId = ktId,
                ResignationId = resignationId,
                KTPlan = ktPlan,
                ReplacementEmployee = txtReplacementEmployee.Text.Trim(),
                KTStatus = ddlKTStatus.SelectedValue,
                KTStartDate = ktStartDate,
                KTCompletionDate = ktCompletionDate
            };

            int userId = Convert.ToInt32(Session["userId"]);

            try
            {
                HandoverprocessBL bl = new HandoverprocessBL();
                KTHandoverResponseDO result = ktId > 0
                    ? bl.UpdateKTHandover(model, userId)
                    : bl.SaveKTHandover(model, userId);

                if (result != null && result.Success)
                {
                    int savedKTId = result.KTId > 0 ? result.KTId : ktId;
                    bl.SaveKTProjectHandoverRows(savedKTId, ProjectRows);

                    string safeMsg = System.Web.HttpUtility.JavaScriptStringEncode(
                        result.ResponseMsg ?? "KT & Handover details saved successfully.");
                    ScriptManager.RegisterStartupScript(this, GetType(), "ktSaved",
                        $"showKTResult('Success', '{safeMsg}');", true);

                    ShowKTList();
                }
                else
                {
                    string safeErr = System.Web.HttpUtility.JavaScriptStringEncode(
                        result?.ResponseMsg ?? "Unable to save KT & Handover details.");
                    ScriptManager.RegisterStartupScript(this, GetType(), "ktError",
                        $"showKTResult('Error', '{safeErr}');", true);
                }
            }
            catch (Exception ex)
            {
                CommonBL errorlog = new CommonBL();
                errorlog.fnStoreErrorLog("KnowledgeTransferList", "btnSaveKT_Click",
                    "Exception Message: " + ex.Message + " StackTrace: " + ex.StackTrace, UserId);
                ScriptManager.RegisterStartupScript(this, GetType(), "ktException",
                    "alert('Error occurred while saving KT & Handover details.');", true);
            }
        }

        protected void btnCancelKT_Click(object sender, EventArgs e)
        {
            ShowKTList();
        }
    }
}
