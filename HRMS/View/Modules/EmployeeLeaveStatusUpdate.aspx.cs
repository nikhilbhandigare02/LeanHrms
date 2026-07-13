using DataObject;
using ProcessModel;
using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace HRMS.View.Modules
{
    public partial class EmployeeLeaveStatusUpdate : System.Web.UI.Page
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

                BindApprovalStatusDropdown();

                string leaveIdQuery = Request.QueryString["leaveId"];
                if (!string.IsNullOrEmpty(leaveIdQuery))
                {
                    hfLeaveId.Value = leaveIdQuery;
                    LoadLeaveDetails(leaveIdQuery);
                }
                else
                {
                    Response.Redirect("EmployeeLeaveList.aspx", false);
                }
            }
        }

        protected void BindApprovalStatusDropdown()
        {
            try
            {
                CommonBL commonBL = new CommonBL();
                List<DropDownData> approvalStatusList = commonBL.BindLookupData("LeaveStatus");

                ddlApprovalStatus.DataSource = approvalStatusList;
                ddlApprovalStatus.DataTextField = "Text";
                ddlApprovalStatus.DataValueField = "Value";
                ddlApprovalStatus.DataBind();
            }
            catch (Exception ex)
            {
                CommonBL errorlog = new CommonBL();
                errorlog.fnStoreErrorLog("EmployeeLeaveStatusUpdate", "BindApprovalStatusDropdown", "Exception Message" + ex.Message + "Strace=" + ex.StackTrace, UserId);
            }
        }

        protected void LoadLeaveDetails(string leaveId)
        {
            try
            {
                EmployeeLeaveBL leaveBL = new EmployeeLeaveBL();
                EmployeeLeaveDetailDO leave = leaveBL.GetLeaveDetailsByLeaveId(Convert.ToInt32(leaveId));

                if (leave != null)
                {
                    txtLeaveType.Text = leave.leaves_types;
                    txtStartDate.Text = leave.start_date;
                    txtEndDate.Text = leave.end_date;
                    txtDescription.Text = leave.leave_description;
                    // Find the item with matching Text to set selected, since we don't know the Value
                    ListItem item = ddlApprovalStatus.Items.FindByText(leave.approval_status_text);
                    if (item != null)
                    {
                        ddlApprovalStatus.SelectedValue = item.Value;
                    }
                    txtRejectionRemark.Text = leave.rejection_remark;
                }
            }
            catch (Exception ex)
            {
                CommonBL errorlog = new CommonBL();
                errorlog.fnStoreErrorLog("EmployeeLeaveStatusUpdate", "LoadLeaveDetails", "Exception Message" + ex.Message + "Strace=" + ex.StackTrace, UserId);
            }
        }

        protected void btnUpdate_Click(object sender, EventArgs e)
        {
            try
            {
                int leaveId = Convert.ToInt32(hfLeaveId.Value);
                int userId = Convert.ToInt32(UserId);
                string startDate = txtStartDate.Text;
                string endDate = txtEndDate.Text;
                string rejectionRemark = txtRejectionRemark.Text;
                string approvalStatus = ddlApprovalStatus.SelectedValue;
                
                // Calculate leave count
                int leaveCount = 0;
                if (!string.IsNullOrEmpty(startDate) && !string.IsNullOrEmpty(endDate))
                {
                    DateTime startDt, endDt;
                    if (DateTime.TryParse(startDate, out startDt) && DateTime.TryParse(endDate, out endDt))
                    {
                        leaveCount = (endDt - startDt).Days + 1;
                    }
                }

                EmployeeLeaveBL leaveBL = new EmployeeLeaveBL();
                var result = leaveBL.UpdateLeaveStatus(leaveId, userId, startDate, endDate, rejectionRemark, approvalStatus, leaveCount);

                string statusParam = result.Success ? "Success" : "Fail";
                string remarkParam = result.Remarks;
                Response.Redirect($"EmployeeLeaveList.aspx?status={Uri.EscapeDataString(statusParam)}&remark={Uri.EscapeDataString(remarkParam)}", false);
            }
            catch (Exception ex)
            {
                CommonBL errorlog = new CommonBL();
                errorlog.fnStoreErrorLog("EmployeeLeaveStatusUpdate", "btnUpdate_Click", "Exception Message" + ex.Message + "Strace=" + ex.StackTrace, UserId);
                Response.Redirect($"EmployeeLeaveList.aspx?status=Fail&remark={Uri.EscapeDataString(ex.Message)}", false);
            }
        }

        protected void btnBack_Click(object sender, EventArgs e)
        {
            Response.Redirect("EmployeeLeaveList.aspx", false);
        }
    }
}
