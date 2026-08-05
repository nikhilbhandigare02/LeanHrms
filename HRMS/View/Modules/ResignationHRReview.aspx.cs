using DataObject;
using ProcessModel;
using System;
using System.Web.UI;

namespace HRMS.View.Modules
{
    public partial class ResignationHRReview : System.Web.UI.Page
    {
        protected string UserId = null;
        protected int ResignationId = 0;

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
                if (Request.QueryString["ResignationId"] != null)
                {
                    int.TryParse(Request.QueryString["ResignationId"], out ResignationId);
                }

                if (ResignationId <= 0)
                {
                    ScriptManager.RegisterStartupScript(this, GetType(), "invalidResignation",
                        "alert('Invalid resignation request.');", true);
                    Response.Redirect("~/View/Modules/ResignationList.aspx", false);
                    return;
                }

                hfResignationId.Value = ResignationId.ToString();

                // Check if this is save mode or update mode
                string mode = Request.QueryString["mode"] ?? "update";
                BindHRReview(ResignationId, mode);
            }
            else
            {
                int.TryParse(hfResignationId.Value, out ResignationId);
            }
        }

        private void BindHRReview(int resignationId, string mode = "update")
        {
            try
            {
                HandoverprocessBL bl = new HandoverprocessBL();
                HRReviewDO model = bl.GetHRReviewDetails(resignationId);
                HRReviewDO existingReview = bl.GetHRReviewById(resignationId);

                lblEmployeeId.Text = model.EmployeeId > 0 ? model.EmployeeId.ToString() : "-";
                lblEmployeeName.Text = string.IsNullOrWhiteSpace(model.EmployeeName) ? "-" : model.EmployeeName;
                lblDepartment.Text = string.IsNullOrWhiteSpace(model.Department) ? "-" : model.Department;
                lblDesignation.Text = string.IsNullOrWhiteSpace(model.Designation) ? "-" : model.Designation;
                lblReportingManager.Text = string.IsNullOrWhiteSpace(model.ReportingManager) ? "-" : model.ReportingManager;
                lblDateOfJoining.Text = model.DateOfJoining.HasValue ? model.DateOfJoining.Value.ToString("dd-MM-yyyy") : "-";
                lblResignationDate.Text = model.ResignationDate == DateTime.MinValue ? "-" : model.ResignationDate.ToString("dd-MM-yyyy");
                lblProposedLastWorkingDate.Text = model.ProposedLastWorkingDate == DateTime.MinValue ? "-" : model.ProposedLastWorkingDate.ToString("dd-MM-yyyy");
                lblReason.Text = string.IsNullOrWhiteSpace(model.Reason) ? "-" : model.Reason;

                bool alreadyReviewed = existingReview != null &&
                    string.Equals(existingReview.Status, "Accepted", StringComparison.OrdinalIgnoreCase);
                lblReviewStatus.Text = alreadyReviewed ? "HR review completed" : "HR review pending";
                lblReviewStatus.CssClass = alreadyReviewed ? "badge bg-success" : "badge bg-warning text-dark";

                // On initial load, default Revised Last Working Date to the Proposed Last
                // Working Date. It only switches to the Notice Start Date (today) +
                // Notice Days formula once the HR user actually changes Notice Days
                // (see recalculateRevisedLastWorkingDate() in the markup, and the
                // authoritative recompute in btnAcceptResignation_Click on submit).
                string defaultRevisedLastWorkingDate = model.ProposedLastWorkingDate == DateTime.MinValue
                    ? string.Empty
                    : model.ProposedLastWorkingDate.ToString("dd-MM-yyyy");

                // In save mode, don't populate existing review data - treat as new review
                if (mode == "save" || existingReview == null)
                {
                    txtNoticeDays.Text = "0";
                    txtRevisedLastWorkingDate.Text = defaultRevisedLastWorkingDate;

                    hfHRReviewId.Value = "0";
                    btnAcceptResignation.Text = "Accept";

                    // Enable all fields for save mode. Revised Last Working Date stays
                    // ReadOnly (markup) - it is always derived from Notice Start Date
                    // (today) + Notice Days, never entered manually.
                    ddlNoticePeriodRequired.Enabled = true;
                    ddlBuyoutApplicable.Enabled = true;
                    txtNoticeDays.Enabled = true;
                    txtHRRemarks.Enabled = true;
                }
                else
                {
                    // Update mode - populate existing review data
                    ddlNoticePeriodRequired.SelectedValue = string.IsNullOrWhiteSpace(existingReview.NoticePeriodRequired) ? "" : existingReview.NoticePeriodRequired;
                    txtNoticeDays.Text = existingReview.NoticeDays.HasValue ? existingReview.NoticeDays.Value.ToString() : string.Empty;
                    ddlBuyoutApplicable.SelectedValue = string.IsNullOrWhiteSpace(existingReview.BuyoutApplicable) ? "" : existingReview.BuyoutApplicable;
                    txtRevisedLastWorkingDate.Text = defaultRevisedLastWorkingDate;
                    txtHRRemarks.Text = existingReview.HRRemarks;

                    // Update mode: Notice Period Required was already decided when the
                    // review was first saved - keep it locked so it can't be changed later.
                    ddlNoticePeriodRequired.Enabled = false;

                    hfHRReviewId.Value = existingReview.HRReviewId.ToString();
                    btnAcceptResignation.Text = "Update";
                }
            }
            catch (Exception ex)
            {
                CommonBL errorlog = new CommonBL();
                errorlog.fnStoreErrorLog("ResignationHRReview", "BindHRReview",
                    "Exception Message: " + ex.Message + " StackTrace: " + ex.StackTrace, UserId);
            }
        }

        protected void btnAcceptResignation_Click(object sender, EventArgs e)
        {
            int resignationId;
            int.TryParse(hfResignationId.Value, out resignationId);

            string noticePeriodRequired = ddlNoticePeriodRequired.SelectedValue;
            string buyoutApplicable = ddlBuyoutApplicable.SelectedValue;
            string hrRemarks = txtHRRemarks.Text.Trim();

            if (resignationId <= 0)
            {
                ScriptManager.RegisterStartupScript(this, GetType(), "invalidResignation",
                    "alert('Invalid resignation request.');", true);
                return;
            }

            if (string.IsNullOrEmpty(noticePeriodRequired))
            {
                ScriptManager.RegisterStartupScript(this, GetType(), "reqNotice",
                    "alert('Please select Notice Period Required.');", true);
                return;
            }

            if (string.IsNullOrEmpty(buyoutApplicable))
            {
                ScriptManager.RegisterStartupScript(this, GetType(), "reqBuyout",
                    "alert('Please select Buyout Applicable.');", true);
                return;
            }

            if (string.IsNullOrEmpty(txtRevisedLastWorkingDate.Text.Trim()))
            {
                ScriptManager.RegisterStartupScript(this, GetType(), "reqLwd",
                    "alert('Please select Revised Last Working Date.');", true);
                return;
            }

            if (string.IsNullOrEmpty(hrRemarks))
            {
                ScriptManager.RegisterStartupScript(this, GetType(), "reqRemarks",
                    "alert('HR Remarks are mandatory.');", true);
                return;
            }

            int noticeDays;
            if (!int.TryParse(txtNoticeDays.Text.Trim(), out noticeDays) || noticeDays < 0)
            {
                noticeDays = 0;
            }

            // Revised Last Working Date is always derived from Notice Start Date (today)
            // + Notice Days - recompute it authoritatively here rather than trusting the
            // posted (ReadOnly) textbox value.
            DateTime revisedLastWorkingDate = DateTime.Today.AddDays(noticeDays);
            txtRevisedLastWorkingDate.Text = revisedLastWorkingDate.ToString("dd-MM-yyyy");

            int hrReviewId;
            int.TryParse(hfHRReviewId.Value, out hrReviewId);

            var model = new HRReviewDO
            {
                HRReviewId = hrReviewId,
                ResignationId = resignationId,
                NoticePeriodRequired = noticePeriodRequired,
                NoticeDays = noticeDays,
                BuyoutApplicable = buyoutApplicable,
                RevisedLastWorkingDate = revisedLastWorkingDate,
                HRRemarks = hrRemarks
            };

            int updatedBy = Convert.ToInt32(Session["userId"]);

            try
            {
                HandoverprocessBL bl = new HandoverprocessBL();
                HRReviewResponseDO result = hrReviewId > 0
                    ? bl.UpdateHRReview(model, updatedBy)
                    : bl.SaveHRReviewAndAccept(model, updatedBy);

                if (result != null && result.Success)
                {
                    string safeMsg = System.Web.HttpUtility.JavaScriptStringEncode(
                        result.ResponseMsg ?? "HR Review saved successfully.");
                    ScriptManager.RegisterStartupScript(this, GetType(), "hrReviewSaved",
                        $"showHRReviewResult('Success', '{safeMsg}', 'ResignationList.aspx');", true);
                }
                else
                {
                    string safeErr = System.Web.HttpUtility.JavaScriptStringEncode(
                        result?.ResponseMsg ?? "Unable to save HR review.");
                    ScriptManager.RegisterStartupScript(this, GetType(), "hrReviewError",
                        $"showHRReviewResult('Error', '{safeErr}', null);", true);
                }
            }
            catch (Exception ex)
            {
                CommonBL errorlog = new CommonBL();
                errorlog.fnStoreErrorLog("ResignationHRReview", "btnAcceptResignation_Click",
                    "Exception Message: " + ex.Message + " StackTrace: " + ex.StackTrace, UserId);
                ScriptManager.RegisterStartupScript(this, GetType(), "hrReviewException",
                    $"alert('Error: {ex.Message}');", true);
            }
        }

        protected void btnCancel_Click(object sender, EventArgs e)
        {
            Response.Redirect("~/View/Modules/ResignationList.aspx", false);
        }
    }
}
