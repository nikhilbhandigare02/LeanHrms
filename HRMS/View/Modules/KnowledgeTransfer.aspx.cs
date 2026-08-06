using DataObject;
using ProcessModel;
using System;
using System.Collections.Generic;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace HRMS.View.Modules
{
    public partial class KnowledgeTransfer : System.Web.UI.Page
    {
        protected string UserId = null;
        protected int ResignationId = 0;

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

            if (Session["userId"] == null)
            {
                Response.Redirect("~/view/authentication/login.aspx", false);
                return;
            }

            if (!IsPostBack)
            {
                bool resignationIdProvided = Request.QueryString["ResignationId"] != null;
                if (resignationIdProvided)
                {
                    int.TryParse(Request.QueryString["ResignationId"], out ResignationId);
                }

                if (ResignationId <= 0)
                {
                    // The Resignation module's sidebar menu links straight to this page
                    // with no ResignationId (it's just the entry point) - land quietly on
                    // the list so HR can pick a resignation. Only alert when an id WAS
                    // supplied but was invalid (e.g. a tampered/stale URL).
                    if (resignationIdProvided)
                    {
                        ScriptManager.RegisterStartupScript(this, GetType(), "invalidResignation",
                            "alert('Invalid resignation request.');", true);
                    }
                    Response.Redirect("~/View/Modules/KnowledgeTransferList.aspx", false);
                    return;
                }

                hfResignationId.Value = ResignationId.ToString();
                BindKTForm(ResignationId);
            }
            else
            {
                int.TryParse(hfResignationId.Value, out ResignationId);
            }
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
                    ddlKTStatus.SelectedValue = "Pending";
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

                BindProjectGrid();
            }
            catch (Exception ex)
            {
                CommonBL errorlog = new CommonBL();
                errorlog.fnStoreErrorLog("KnowledgeTransfer", "BindKTForm",
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
            string projectName = txtNewProjectName.Text.Trim();

            if (string.IsNullOrEmpty(projectName))
            {
                ScriptManager.RegisterStartupScript(this, GetType(), "reqProjectName",
                    "alert('Please enter a Project Name before adding a row.');", true);
                return;
            }

            var rows = ProjectRows;
            rows.Add(new KTProjectHandoverRowDO
            {
                ProjectName = projectName,
                AssignedEmployee = txtNewAssignedEmployee.Text.Trim(),
                Status = ddlNewProjectStatus.SelectedValue
            });
            ProjectRows = rows;

            txtNewProjectName.Text = string.Empty;
            txtNewAssignedEmployee.Text = string.Empty;
            ddlNewProjectStatus.SelectedValue = "Pending";

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
                        rows.RemoveAt(index);
                        ProjectRows = rows;
                        BindProjectGrid();
                    }
                }
            }
        }

        protected void btnSaveKT_Click(object sender, EventArgs e)
        {
            string ktPlan = txtKTPlan.Text.Trim();

            if (ResignationId <= 0)
            {
                ScriptManager.RegisterStartupScript(this, GetType(), "invalidResignation",
                    "alert('Invalid resignation request.');", true);
                return;
            }

            if (string.IsNullOrEmpty(ktPlan))
            {
                ScriptManager.RegisterStartupScript(this, GetType(), "reqKtPlan",
                    "alert('KT Plan is mandatory.');", true);
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
                ResignationId = ResignationId,
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
                        $"showKTResult('Success', '{safeMsg}', 'KnowledgeTransferList.aspx');", true);
                }
                else
                {
                    string safeErr = System.Web.HttpUtility.JavaScriptStringEncode(
                        result?.ResponseMsg ?? "Unable to save KT & Handover details.");
                    ScriptManager.RegisterStartupScript(this, GetType(), "ktError",
                        $"showKTResult('Error', '{safeErr}', null);", true);
                }
            }
            catch (Exception ex)
            {
                CommonBL errorlog = new CommonBL();
                errorlog.fnStoreErrorLog("KnowledgeTransfer", "btnSaveKT_Click",
                    "Exception Message: " + ex.Message + " StackTrace: " + ex.StackTrace, UserId);
                ScriptManager.RegisterStartupScript(this, GetType(), "ktException",
                    "alert('Error occurred while saving KT & Handover details.');", true);
            }
        }

        protected void btnCancel_Click(object sender, EventArgs e)
        {
            Response.Redirect("~/View/Modules/KnowledgeTransferList.aspx", false);
        }
    }
}
