using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using MySql.Data.MySqlClient;
using ProcessModel;
using DataObject;

namespace Lean.View.Modules
{
    public partial class ExitInterview : System.Web.UI.Page
    {
        // Control declarations
        protected global::System.Web.UI.WebControls.HiddenField hdnExitInterviewId;
        protected global::System.Web.UI.HtmlControls.HtmlButton btn_addExitInterview;
        protected global::System.Web.UI.WebControls.Button btnBack;
        protected global::System.Web.UI.WebControls.Panel pnlExitInterviewForm;
        protected global::System.Web.UI.HtmlControls.HtmlGenericControl formTitle;
        protected global::System.Web.UI.WebControls.DropDownList ddlEmployee;
        protected global::System.Web.UI.WebControls.DropDownList ddlInterviewer;
        protected global::System.Web.UI.WebControls.DropDownList ddlInterviewStatus;
        protected global::System.Web.UI.WebControls.DropDownList ddlInterviewMode;
        protected global::System.Web.UI.WebControls.TextBox txtInterviewDate;
        protected global::System.Web.UI.WebControls.TextBox txtInterviewTime;
        protected global::System.Web.UI.WebControls.TextBox txtLocation;
        protected global::System.Web.UI.WebControls.TextBox txtNotes;
        protected global::System.Web.UI.WebControls.Button btnSave;
        protected global::System.Web.UI.WebControls.Button btnCancel;
        protected global::System.Web.UI.HtmlControls.HtmlGenericControl divLocation;
        protected global::System.Web.UI.WebControls.Panel pnlGridSection;
        protected global::System.Web.UI.WebControls.HiddenField hfPageIndex;
        protected global::System.Web.UI.WebControls.GridView gvExitInterviews;
        protected global::System.Web.UI.WebControls.DropDownList ddlPageSelector;
        protected global::System.Web.UI.UpdatePanel UpdatePanelForm;
        protected global::System.Web.UI.UpdatePanel UpdatePanelGrid;

        private ProcessModel.ExitInterviewBL objExitInterviewBL = new ProcessModel.ExitInterviewBL();
        private CommonBL objCommonBL = new CommonBL();

        protected int GetSafePageIndex()
        {
            int pageIndex = 0;
            if (!string.IsNullOrEmpty(hfPageIndex.Value))
            {
                int.TryParse(hfPageIndex.Value, out pageIndex);
            }
            return pageIndex;
        }

        protected int GetSerialNumber(object dataItemIndex)
        {
            int pageIndex = GetSafePageIndex();
            int pageSize = gvExitInterviews.PageSize;
            int itemIndex = Convert.ToInt32(dataItemIndex);
            return (pageIndex * pageSize) + itemIndex + 1;
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                BindEmployees();
                BindInterviewers();
                BindInterviewStatus();
                BindInterviewMode();
                BindExitInterviewGrid();
                hfPageIndex.Value = "0";
            }
        }

        protected void cvInterviewDate_ServerValidate(object source, ServerValidateEventArgs args)
        {
            // Only enforce "must be today or later" when scheduling a NEW interview.
            // An existing record being updated may legitimately already be in the past
            // (e.g. HR is editing notes/status after the interview already happened).
            bool isExistingRecord = !string.IsNullOrEmpty(hdnExitInterviewId.Value) && hdnExitInterviewId.Value != "0";
            if (isExistingRecord)
            {
                args.IsValid = true;
                return;
            }

            DateTime selectedDate;
            if (DateTime.TryParse(args.Value, out selectedDate))
            {
                args.IsValid = selectedDate >= DateTime.Today;
            }
            else
            {
                args.IsValid = false;
            }
        }

        protected void cvLocation_ServerValidate(object source, ServerValidateEventArgs args)
        {
            string selectedModeText = ddlInterviewMode.SelectedItem != null ? ddlInterviewMode.SelectedItem.Text : string.Empty;
            if (!IsVirtualMode(selectedModeText))
            {
                args.IsValid = true;
                return;
            }

            args.IsValid = !string.IsNullOrWhiteSpace(args.Value);
        }

        private void BindEmployees()
        {
            try
            {
                List<DropDownData> employees = new List<DropDownData>();
                string DBName = ConfigurationManager.AppSettings["DBName"];
                string MySqlconnection = ConfigurationManager.ConnectionStrings["MysqlConnection"].ConnectionString;

                using (MySqlConnection con = new MySqlConnection(MySqlconnection))
                {
                    con.Open();
                    using (MySqlCommand cmd = new MySqlCommand("sp_getresignedusers", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        using (MySqlDataReader dr = cmd.ExecuteReader())
                        {
                            while (dr.Read())
                            {
                                employees.Add(new DropDownData
                                {
                                    Id = Convert.ToInt32(dr["user_id"]),
                                    Text = dr["user_fullname"].ToString()
                                });
                            }
                        }
                    }
                }

                ddlEmployee.Items.Clear();
                ddlEmployee.Items.Add(new ListItem("Select Employee", ""));
                if (employees != null)
                {
                    foreach (DropDownData emp in employees)
                    {
                        ddlEmployee.Items.Add(new ListItem(emp.Text, emp.Id.ToString()));
                    }
                }
            }
            catch (Exception ex)
            {
                objCommonBL.fnStoreErrorLog("ExitInterview", "BindEmployees", ex.Message, Convert.ToString(Session["userId"]));
            }
        }

        private void BindInterviewers()
        {
            try
            {
                List<DropDownData> interviewers = objExitInterviewBL.GetInterviewers();
                ddlInterviewer.Items.Clear();
                ddlInterviewer.Items.Add(new ListItem("Select Interviewer", ""));
                if (interviewers != null)
                {
                    foreach (DropDownData interviewer in interviewers)
                    {
                        ddlInterviewer.Items.Add(new ListItem(interviewer.Text, interviewer.Id.ToString()));
                    }
                }
            }
            catch (Exception ex)
            {
                objCommonBL.fnStoreErrorLog("ExitInterview", "BindInterviewers", ex.Message, Convert.ToString(Session["userId"]));
            }
        }

        private void BindInterviewStatus()
        {
            try
            {
                List<DropDownData> statusList = objCommonBL.BindLookupData("Schedule Status");
                ddlInterviewStatus.Items.Clear();
                ddlInterviewStatus.Items.Add(new ListItem("Select Status", ""));
                if (statusList != null)
                {
                    foreach (DropDownData status in statusList)
                    {
                        ddlInterviewStatus.Items.Add(new ListItem(status.Text, status.Id.ToString()));
                    }
                }
            }
            catch (Exception ex)
            {
                objCommonBL.fnStoreErrorLog("ExitInterview", "BindInterviewStatus", ex.Message, Convert.ToString(Session["userId"]));
            }
        }

        private void BindInterviewMode()
        {
            try
            {
                List<DropDownData> modeList = objCommonBL.BindLookupData("Interview Mode");
                ddlInterviewMode.Items.Clear();
                ddlInterviewMode.Items.Add(new ListItem("Select Mode", ""));
                if (modeList != null)
                {
                    foreach (DropDownData mode in modeList)
                    {
                        ddlInterviewMode.Items.Add(new ListItem(mode.Text, mode.Id.ToString()));
                    }
                }
            }
            catch (Exception ex)
            {
                objCommonBL.fnStoreErrorLog("ExitInterview", "BindInterviewMode", ex.Message, Convert.ToString(Session["userId"]));
            }
        }

        private void BindExitInterviewGrid()
        {
            try
            {
                List<ExitInterviewDO> exitInterviews = objExitInterviewBL.GetExitInterviewList();
                gvExitInterviews.DataSource = exitInterviews;
                gvExitInterviews.DataBind();

                // Bind pagination
                BindPagination(exitInterviews != null ? exitInterviews.Count : 0);
            }
            catch (Exception ex)
            {
                objCommonBL.fnStoreErrorLog("ExitInterview", "BindExitInterviewGrid", ex.Message, Convert.ToString(Session["userId"]));
            }
        }

        private void BindPagination(int totalRecords)
        {
            ddlPageSelector.Items.Clear();
            int pageSize = gvExitInterviews.PageSize;
            int totalPages = (int)Math.Ceiling((double)totalRecords / pageSize);

            for (int i = 1; i <= totalPages; i++)
            {
                ddlPageSelector.Items.Add(new ListItem("Page " + i, (i - 1).ToString()));
            }

            if (totalPages > 0)
            {
                ddlPageSelector.SelectedIndex = GetSafePageIndex();
            }
        }

        protected void btn_addExitInterview_ServerClick(object sender, EventArgs e)
        {
            pnlExitInterviewForm.Visible = true;
            pnlGridSection.Visible = false;
            ClearForm();
            formTitle.InnerText = "Schedule Exit Interview";
            hdnExitInterviewId.Value = "";
            btnSave.Text = "Save";
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                if (Page.IsValid)
                {
                    ExitInterviewDO exitInterview = new ExitInterviewDO();
                    
                    exitInterview.ExitInterviewId = string.IsNullOrEmpty(hdnExitInterviewId.Value) ? 0 : Convert.ToInt32(hdnExitInterviewId.Value);
                    exitInterview.UserId = Convert.ToInt32(ddlEmployee.SelectedValue);
                    exitInterview.InterviewerId = Convert.ToInt32(ddlInterviewer.SelectedValue);
                    exitInterview.InterviewDate = Convert.ToDateTime(txtInterviewDate.Text);
                    exitInterview.InterviewTime = txtInterviewTime.Text;
                    exitInterview.InterviewStatus = Convert.ToInt32(ddlInterviewStatus.SelectedValue);
                    exitInterview.InterviewMode = Convert.ToInt32(ddlInterviewMode.SelectedValue);
                    exitInterview.Interview_Status_id = Convert.ToInt32(ddlInterviewStatus.SelectedValue);
                    exitInterview.Interview_Mode_id = Convert.ToInt32(ddlInterviewMode.SelectedValue);
                    string selectedModeText = ddlInterviewMode.SelectedItem != null ? ddlInterviewMode.SelectedItem.Text : string.Empty;
                    exitInterview.Location = IsVirtualMode(selectedModeText) ? txtLocation.Text : string.Empty;
                    exitInterview.Notes = txtNotes.Text;
                    exitInterview.InsertedBy = Convert.ToInt32(Session["userId"]);

                    string result;
                    if (exitInterview.ExitInterviewId == 0)
                    {
                        result = objExitInterviewBL.SaveExitInterview(exitInterview);
                    }
                    else
                    {
                        result = objExitInterviewBL.UpdateExitInterview(exitInterview);
                    }

                    if (result.Contains("Success"))
                    {
                        string safeMsg = HttpUtility.JavaScriptStringEncode(result);
                        ScriptManager.RegisterStartupScript(this, GetType(), "success", "showMessage('Success', '" + safeMsg + "');", true);
                        pnlExitInterviewForm.Visible = false;
                        pnlGridSection.Visible = true;
                        BindExitInterviewGrid();
                        UpdatePanelGrid.Update();
                    }
                    else
                    {
                        string safeErr = HttpUtility.JavaScriptStringEncode(result);
                        ScriptManager.RegisterStartupScript(this, GetType(), "error", "showMessage('Error', '" + safeErr + "');", true);
                    }
                }
                else
                {
                    ScriptManager.RegisterStartupScript(this, GetType(), "invalidForm",
                        "showMessage('Error', 'Please correct the highlighted fields before saving.');", true);
                }
            }
            catch (Exception ex)
            {
                objCommonBL.fnStoreErrorLog("ExitInterview", "btnSave_Click", ex.Message, Convert.ToString(Session["userId"]));
                ScriptManager.RegisterStartupScript(this, GetType(), "error", "showMessage('Error', 'An error occurred while saving exit interview.');", true);
            }
        }

        protected void btnCancel_Click(object sender, EventArgs e)
        {
            pnlExitInterviewForm.Visible = false;
            pnlGridSection.Visible = true;
            ClearForm();
            UpdatePanelGrid.Update();
        }

        protected void btnBack_Click(object sender, EventArgs e)
        {
            Response.Redirect("~/View/Modules/Home.aspx");
        }

        protected void gvExitInterviews_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            try
            {
                int exitInterviewId = Convert.ToInt32(e.CommandArgument);

                switch (e.CommandName)
                {
                    case "ViewInterview":
                        ViewExitInterview(exitInterviewId);
                        break;
                    case "EditInterview":
                        EditExitInterview(exitInterviewId);
                        break;
                    case "DeleteInterview":
                        DeleteExitInterview(exitInterviewId);
                        break;
                }
            }
            catch (Exception ex)
            {
                objCommonBL.fnStoreErrorLog("ExitInterview", "gvExitInterviews_RowCommand", ex.Message, Convert.ToString(Session["userId"]));
            }
        }

        private static bool IsVirtualMode(string modeText)
        {
            string normalized = (modeText ?? string.Empty).Trim();
            // The live "Interview Mode" lookup data has a typo ("Vertual" instead of
            // "Virtual") - match both so this keeps working if the typo gets fixed later.
            return string.Equals(normalized, "Virtual", StringComparison.OrdinalIgnoreCase)
                || string.Equals(normalized, "Vertual", StringComparison.OrdinalIgnoreCase);
        }

        private void SetDropDownValueSafe(DropDownList ddl, string value, string missingLabel)
        {
            if (ddl.Items.FindByValue(value) != null)
            {
                ddl.SelectedValue = value;
                return;
            }

            if (!string.IsNullOrEmpty(value) && value != "0")
            {
                ddl.Items.Add(new ListItem(missingLabel + " (ID: " + value + ")", value));
                ddl.SelectedValue = value;
            }
        }

        private void ViewExitInterview(int exitInterviewId)
        {
            try
            {
                ExitInterviewDO exitInterview = objExitInterviewBL.GetExitInterviewById(exitInterviewId);
                if (exitInterview != null)
                {
                    pnlExitInterviewForm.Visible = true;
                    pnlGridSection.Visible = false;
                    formTitle.InnerText = "View Exit Interview";
                    hdnExitInterviewId.Value = exitInterview.ExitInterviewId.ToString();

                    SetDropDownValueSafe(ddlEmployee, exitInterview.UserId.ToString(), "Unknown Employee");
                    SetDropDownValueSafe(ddlInterviewer, exitInterview.InterviewerId.ToString(), "Unknown Interviewer");
                    txtInterviewDate.Text = exitInterview.InterviewDate.ToString("yyyy-MM-dd");
                    txtInterviewTime.Text = exitInterview.InterviewTime;
                    SetDropDownValueSafe(ddlInterviewStatus, exitInterview.Interview_Status_id.ToString(), "Unknown Status");
                    SetDropDownValueSafe(ddlInterviewMode, exitInterview.Interview_Mode_id.ToString(), "Unknown Mode");
                    txtLocation.Text = exitInterview.Location;
                    txtNotes.Text = exitInterview.Notes;

                    // Disable all fields for view mode
                    ddlEmployee.Enabled = false;
                    ddlInterviewer.Enabled = false;
                    txtInterviewDate.Enabled = false;
                    txtInterviewTime.Enabled = false;
                    ddlInterviewStatus.Enabled = false;
                    ddlInterviewMode.Enabled = false;
                    txtLocation.Enabled = false;
                    txtNotes.Enabled = false;

                    // Hide buttons in view mode
                    btnSave.Visible = false;
                    btnCancel.Visible = false;

                    // Always render divLocation - Visible=false would suppress it from the
                    // HTML entirely, and client-side toggling can't reveal an element that
                    // was never rendered. Actual show/hide is CSS/JS-driven (toggleLocationField).
                    divLocation.Visible = true;
                }
            }
            catch (Exception ex)
            {
                objCommonBL.fnStoreErrorLog("ExitInterview", "ViewExitInterview", ex.Message, Convert.ToString(Session["userId"]));
            }
        }

        private void EditExitInterview(int exitInterviewId)
        {
            try
            {
                ExitInterviewDO exitInterview = objExitInterviewBL.GetExitInterviewById(exitInterviewId);
                if (exitInterview != null)
                {
                    pnlExitInterviewForm.Visible = true;
                    pnlGridSection.Visible = false;
                    formTitle.InnerText = "Edit Exit Interview";
                    hdnExitInterviewId.Value = exitInterview.ExitInterviewId.ToString();
                    btnSave.Text = "Update";

                    SetDropDownValueSafe(ddlEmployee, exitInterview.UserId.ToString(), "Unknown Employee");
                    SetDropDownValueSafe(ddlInterviewer, exitInterview.InterviewerId.ToString(), "Unknown Interviewer");
                    txtInterviewDate.Text = exitInterview.InterviewDate.ToString("yyyy-MM-dd");
                    txtInterviewTime.Text = exitInterview.InterviewTime;
                    SetDropDownValueSafe(ddlInterviewStatus, exitInterview.Interview_Status_id.ToString(), "Unknown Status");
                    SetDropDownValueSafe(ddlInterviewMode, exitInterview.Interview_Mode_id.ToString(), "Unknown Mode");
                    txtLocation.Text = exitInterview.Location;
                    txtNotes.Text = exitInterview.Notes;

                    // Disable employee dropdown in edit mode (cannot change employee)
                    ddlEmployee.Enabled = false;

                    // Enable other fields for editing
                    ddlInterviewer.Enabled = true;
                    txtInterviewDate.Enabled = true;
                    txtInterviewTime.Enabled = true;
                    ddlInterviewStatus.Enabled = true;
                    ddlInterviewMode.Enabled = true;
                    txtLocation.Enabled = true;
                    txtNotes.Enabled = true;

                    // Show buttons in edit mode
                    btnSave.Visible = true;
                    btnCancel.Visible = true;

                    // Always render divLocation - Visible=false would suppress it from the
                    // HTML entirely, and client-side toggling can't reveal an element that
                    // was never rendered. Actual show/hide is CSS/JS-driven (toggleLocationField).
                    divLocation.Visible = true;
                }
            }
            catch (Exception ex)
            {
                objCommonBL.fnStoreErrorLog("ExitInterview", "EditExitInterview", ex.Message, Convert.ToString(Session["userId"]));
            }
        }

        private void DeleteExitInterview(int exitInterviewId)
        {
            try
            {
                string result = objExitInterviewBL.DeleteExitInterview(exitInterviewId);
                if (result.Contains("Success"))
                {
                    string safeMsg = HttpUtility.JavaScriptStringEncode(result);
                    ScriptManager.RegisterStartupScript(this, GetType(), "success", "showMessage('Success', '" + safeMsg + "');", true);
                    BindExitInterviewGrid();
                }
                else
                {
                    string safeErr = HttpUtility.JavaScriptStringEncode(result);
                    ScriptManager.RegisterStartupScript(this, GetType(), "error", "showMessage('Error', '" + safeErr + "');", true);
                }
            }
            catch (Exception ex)
            {
                objCommonBL.fnStoreErrorLog("ExitInterview", "DeleteExitInterview", ex.Message, Convert.ToString(Session["userId"]));
            }
        }

        private void ClearForm()
        {
            ddlEmployee.SelectedIndex = 0;
            ddlInterviewer.SelectedIndex = 0;
            txtInterviewDate.Text = "";
            txtInterviewTime.Text = "";
            ddlInterviewStatus.SelectedIndex = 0;
            ddlInterviewMode.SelectedIndex = 0;
            txtLocation.Text = "";
            txtNotes.Text = "";

            // Enable all fields for new entry
            ddlEmployee.Enabled = true;
            ddlInterviewer.Enabled = true;
            txtInterviewDate.Enabled = true;
            txtInterviewTime.Enabled = true;
            ddlInterviewStatus.Enabled = true;
            ddlInterviewMode.Enabled = true;
            txtLocation.Enabled = true;
            txtNotes.Enabled = true;
            btnSave.Visible = true;
            btnSave.Text = "Save";
            btnCancel.Visible = true;

            // Always render divLocation - actual show/hide is CSS/JS-driven
            // (toggleLocationField), since Visible=false would suppress the element
            // from the HTML entirely and block any later client-side toggling.
            divLocation.Visible = true;
        }

        protected void OnPageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            gvExitInterviews.PageIndex = e.NewPageIndex;
            hfPageIndex.Value = e.NewPageIndex.ToString();
            BindExitInterviewGrid();
        }

        protected void ddlPageSelector_SelectedIndexChanged(object sender, EventArgs e)
        {
            int pageIndex = Convert.ToInt32(ddlPageSelector.SelectedValue);
            gvExitInterviews.PageIndex = pageIndex;
            hfPageIndex.Value = pageIndex.ToString();
            BindExitInterviewGrid();
        }

        protected void gvExitInterviews_Sorting(object sender, GridViewSortEventArgs e)
        {
            // Sorting logic can be implemented here
            BindExitInterviewGrid();
        }
    }
}
