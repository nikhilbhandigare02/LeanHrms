using DataObject;
using ProcessModel;
using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace HRMS.View.Modules
{
    public partial class EmployeeLeaveList : System.Web.UI.Page
    {
        protected string UserId = null;
        protected string UserRole = null;

        protected void Page_Load(object sender, EventArgs e)
        {
            UserId = Convert.ToString(Session["userId"]);
            UserRole = Convert.ToString(Session["userrole"]);
            if (!IsPostBack)
            {
                if (Session["userId"] == null)
                {
                    Response.Redirect("~/view/authentication/login.aspx", false);
                    return;
                }

                BindGridView();
            }
        }

        protected void BindGridView()
        {
            try
            {
                EmployeeLeaveBL leaveBL = new EmployeeLeaveBL();
                List<EmployeeLeaveListDO> leaves = leaveBL.GetAllLeaveRequestsForHr();
                gridview.DataSource = leaves;
                gridview.DataBind();
            }
            catch (Exception ex)
            {
                CommonBL errorlog = new CommonBL();
                errorlog.fnStoreErrorLog("EmployeeLeaveList", "BindGridView", "Exception Message" + ex.Message + "Strace=" + ex.StackTrace, UserId);
            }
        }

        protected bool ShouldShowEditButton(object approvalStatusObj)
        {
            if (approvalStatusObj == null || approvalStatusObj == DBNull.Value)
            {
                return false;
            }
            string approvalStatus = approvalStatusObj.ToString();
            
            // For HR: show edit button only when status is "Sent To HR Authority"
            if (UserRole.Equals("HR", StringComparison.OrdinalIgnoreCase))
            {
                return approvalStatus.Equals("Sent To HR Authority", StringComparison.OrdinalIgnoreCase);
            }
            // For Admin: show edit button only when status is "Send to Director Authority"
            else if (UserRole.Equals("Admin", StringComparison.OrdinalIgnoreCase))
            {
                return approvalStatus.Equals("Send to Director Authority", StringComparison.OrdinalIgnoreCase);
            }
            // For other roles: don't show edit button
            return false;
        }

        protected void gridview_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            try
            {
                if (e.CommandName == "viewLeave")
                {
                    string[] args = Convert.ToString(e.CommandArgument).Split('|');
                    string leaveId = args.Length > 0 ? args[0] : "0";
                    Response.Redirect("EmployeeLeaveView.aspx?leaveId=" + HttpUtility.UrlEncode(leaveId) + "&mode=view", false);
                }
                else if (e.CommandName == "editLeave")
                {
                    string leaveId = Convert.ToString(e.CommandArgument);
                    Response.Redirect("EmployeeLeaveStatusUpdate.aspx?leaveId=" + HttpUtility.UrlEncode(leaveId), false);
                }
            }
            catch (Exception ex)
            {
                CommonBL errorlog = new CommonBL();
                errorlog.fnStoreErrorLog("EmployeeLeaveList", "gridview_RowCommand", "Exception Message" + ex.Message + "Strace=" + ex.StackTrace, UserId);
            }
        }
    }
}
