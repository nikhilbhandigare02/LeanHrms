using DataObject;
using ProcessModel;
using System;
using System.Collections.Generic;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace HRMS.View.Modules
{
    public partial class AttendanceDetails : System.Web.UI.Page
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
                string today = DateTime.Now.ToString("yyyy-MM-dd");

                txtFromDate.Text = today;
                txtToDate.Text = today;
                BindEmployeeFilter();
                BindAttendanceGrid(0, null, null);
            }
        }

        private void GetCurrentFilters(out int employeeId, out DateTime? fromDate, out DateTime? toDate)
        {
            int.TryParse(ddlEmployeeFilter.SelectedValue, out employeeId);

            DateTime parsedFrom;
            fromDate = DateTime.TryParse(txtFromDate.Text, out parsedFrom) ? parsedFrom : (DateTime?)null;

            DateTime parsedTo;
            toDate = DateTime.TryParse(txtToDate.Text, out parsedTo) ? parsedTo : (DateTime?)null;
        }

        private void BindEmployeeFilter()
        {
            try
            {
                AttendanceBL attendanceBL = new AttendanceBL();
                List<DropDownData> employees = attendanceBL.GetEmployeeDropdown() ?? new List<DropDownData>();

                ddlEmployeeFilter.DataSource = employees;
                ddlEmployeeFilter.DataTextField = "Text";
                ddlEmployeeFilter.DataValueField = "Id";
                ddlEmployeeFilter.DataBind();

                ddlEmployeeFilter.Items.Insert(0, new ListItem("-- All Employees --", "0"));
            }
            catch (Exception ex)
            {
                CommonBL errorlog = new CommonBL();
                errorlog.fnStoreErrorLog("AttendanceDetails", "BindEmployeeFilter", "Exception Message=" + ex.Message + " StackTrace=" + ex.StackTrace, UserId);
            }
        }

        private void BindAttendanceGrid(int employeeId, DateTime? fromDate, DateTime? toDate)
        {
            try
            {
                AttendanceBL attendanceBL = new AttendanceBL();
                List<EmpAttendanceDO> attendance = attendanceBL.GetAttendanceList(employeeId, fromDate, toDate) ?? new List<EmpAttendanceDO>();
                gvAttendance.DataSource = attendance;
                gvAttendance.DataBind();
            }
            catch (Exception ex)
            {
                CommonBL errorlog = new CommonBL();
                errorlog.fnStoreErrorLog("AttendanceDetails", "BindAttendanceGrid", "Exception Message=" + ex.Message + " StackTrace=" + ex.StackTrace, UserId);
            }
        }

        protected void btnSearch_Click(object sender, EventArgs e)
        {
            int employeeId;
            DateTime? fromDate;
            DateTime? toDate;
            GetCurrentFilters(out employeeId, out fromDate, out toDate);
            gvAttendance.PageIndex = 0;
            BindAttendanceGrid(employeeId, fromDate, toDate);
        }

        protected void btnClear_Click(object sender, EventArgs e)
        {
            ddlEmployeeFilter.SelectedIndex = 0;
            txtFromDate.Text = string.Empty;
            txtToDate.Text = string.Empty;
            gvAttendance.PageIndex = 0;
            BindAttendanceGrid(0, null, null);
        }

        protected void gvAttendance_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            gvAttendance.PageIndex = e.NewPageIndex;

            int employeeId;
            DateTime? fromDate;
            DateTime? toDate;
            GetCurrentFilters(out employeeId, out fromDate, out toDate);
            BindAttendanceGrid(employeeId, fromDate, toDate);
        }

        protected void gvAttendance_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType != DataControlRowType.DataRow)
            {
                return;
            }

            EmpAttendanceDO attendance = e.Row.DataItem as EmpAttendanceDO;
            if (attendance == null)
            {
                return;
            }

            Label lblCheckIn = e.Row.FindControl("lblCheckIn") as Label;
            Label lblCheckOut = e.Row.FindControl("lblCheckOut") as Label;
            Label lblHours = e.Row.FindControl("lblHours") as Label;

            if (lblCheckIn != null)
            {
                lblCheckIn.Text = FormatTime(attendance.LoginTime);
            }

            if (lblCheckOut != null)
            {
                lblCheckOut.Text = FormatTime(attendance.LogoutTime);
            }

            if (lblHours != null)
            {
                lblHours.Text = string.IsNullOrWhiteSpace(attendance.WorkedHoursDisplay) ? "--" : attendance.WorkedHoursDisplay;

                if (attendance.IsBelowMinimum.HasValue)
                {
                    bool isBelowMinimum = attendance.IsBelowMinimum.Value;
                    lblHours.CssClass = isBelowMinimum ? "attendance-hours-low" : "attendance-hours-ok";
                    lblHours.ForeColor = isBelowMinimum ? System.Drawing.Color.Red : System.Drawing.Color.Empty;
                }
            }
        }

        protected void btnExport_Click(object sender, EventArgs e)
        {
            try
            {
                int employeeId;
                DateTime? fromDate;
                DateTime? toDate;
                GetCurrentFilters(out employeeId, out fromDate, out toDate);

                AttendanceBL attendanceBL = new AttendanceBL();
                List<EmpAttendanceDO> attendance = attendanceBL.GetAttendanceList(employeeId, fromDate, toDate) ?? new List<EmpAttendanceDO>();

                StringBuilder sb = new StringBuilder();
                sb.Append("<table border='1'>");
                sb.Append("<tr><th>Employee Name</th><th>Employee Code</th><th>Date</th><th>Check In</th><th>Check Out</th><th>Hrs</th></tr>");

                foreach (EmpAttendanceDO item in attendance)
                {
                    sb.Append("<tr>");
                    sb.Append("<td>" + HttpUtility.HtmlEncode(item.EmployeeName) + "</td>");
                    sb.Append("<td>" + HttpUtility.HtmlEncode(item.EmployeeCode) + "</td>");
                    sb.Append("<td>" + item.LoginDate.ToString("dd-MMM-yyyy") + "</td>");
                    sb.Append("<td>" + FormatTime(item.LoginTime) + "</td>");
                    sb.Append("<td>" + FormatTime(item.LogoutTime) + "</td>");
                    sb.Append("<td>" + (string.IsNullOrWhiteSpace(item.WorkedHoursDisplay) ? "--" : item.WorkedHoursDisplay) + "</td>");
                    sb.Append("</tr>");
                }

                sb.Append("</table>");

                Response.Clear();
                Response.Buffer = true;
                Response.ContentType = "application/vnd.ms-excel";
                Response.AddHeader("content-disposition", $"attachment;filename=AttendanceDetails_{DateTime.Now:yyyyMMddHHmmss}.xls");
                Response.Output.Write(sb.ToString());
                Response.Flush();
                Response.End();
            }
            catch (Exception ex)
            {
                CommonBL errorlog = new CommonBL();
                errorlog.fnStoreErrorLog("AttendanceDetails", "btnExport_Click", "Exception Message=" + ex.Message + " StackTrace=" + ex.StackTrace, UserId);
            }
        }

        private string FormatTime(TimeSpan? time)
        {
            if (!time.HasValue)
            {
                return "--";
            }

            DateTime todayWithTime = DateTime.Today.Add(time.Value);
            return todayWithTime.ToString("hh:mm tt");
        }
    }
}
