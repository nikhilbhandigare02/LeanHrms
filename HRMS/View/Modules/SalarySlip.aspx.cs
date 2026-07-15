using DataObject;
using ProcessModel;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using iText.Html2pdf;
using ListItem = System.Web.UI.WebControls.ListItem;

namespace HRMS.View.Modules
{
    public partial class SalarySlip : System.Web.UI.Page
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
                BindMonths(ddlFromMonth);
                BindMonths(ddlToMonth);
                BindEmployees();
            }
        }

        private void BindYears()
        {
            ddlYear.Items.Clear();
            ddlYear.Items.Add(new ListItem("Select Year", ""));
            int currentYear = DateTime.Now.Year;
            for (int y = currentYear; y >= currentYear - 6; y--)
            {
                ddlYear.Items.Add(new ListItem(y.ToString(), y.ToString()));
            }
        }

        private void BindMonths(DropDownList ddl)
        {
            ddl.Items.Clear();
            ddl.Items.Add(new ListItem("Select Month", ""));
            for (int m = 1; m <= 12; m++)
            {
                ddl.Items.Add(new ListItem(MonthName(m), m.ToString()));
            }
        }

        // Employee list for the dropdown, sourced from stored procedure sp_get_employee_dropdown.
        private void BindEmployees()
        {
            try
            {
                List<DropDownData> employees = new CommonBL().dropdownEmployee();

                ddlEmployee.Items.Clear();
                ddlEmployee.Items.Add(new ListItem("Select Employee", ""));
                if (employees != null)
                {
                    foreach (DropDownData emp in employees)
                    {
                        ddlEmployee.Items.Add(new ListItem(emp.Text, emp.Value.ToString()));
                    }
                }
            }
            catch (Exception ex)
            {
                new CommonBL().fnStoreErrorLog("SalarySlip", "BindEmployees", ex.Message + " Strace=" + ex.StackTrace, UserId);
            }
        }

        protected void btnSearch_Click(object sender, EventArgs e)
        {
            try
            {
                string year = Convert.ToString(ddlYear.SelectedValue);
                string fromMonth = Convert.ToString(ddlFromMonth.SelectedValue);
                string toMonth = Convert.ToString(ddlToMonth.SelectedValue);
                string employeeValue = Convert.ToString(ddlEmployee.SelectedValue);

                if (string.IsNullOrWhiteSpace(year) || string.IsNullOrWhiteSpace(fromMonth) ||
                    string.IsNullOrWhiteSpace(toMonth) || string.IsNullOrWhiteSpace(employeeValue))
                {
                    ShowResults(false);
                    ClientScript.RegisterStartupScript(GetType(), "Validation",
                        "showUserSavedMessage('Error', 'Please select Year, From Month, To Month, and Employee.');", true);
                    return;
                }

                int yearInt = Convert.ToInt32(year);
                int fromInt = Convert.ToInt32(fromMonth);
                int toInt = Convert.ToInt32(toMonth);
              //  int selectedUserId = Convert.ToInt32(employeeValue);

                if (fromInt > toInt)
                {
                    ShowResults(false);
                    ClientScript.RegisterStartupScript(GetType(), "RangeError",
                        "showUserSavedMessage('Error', 'From Month cannot be after To Month.');", true);
                    return;
                }

                // salary_slip_details is keyed by employeecode, so resolve it from the selected user.
                //int employeeCode = ResolveEmployeeCode(selectedUserId);
                string employeeCode = employeeValue;
                if (employeeCode == "0")
                {
                    ShowResults(false);
                    ClientScript.RegisterStartupScript(GetType(), "NoCode",
                        "showUserSavedMessage('Error', 'No employee code found for the selected employee.');", true);
                    return;
                }

                List<SalarySlipDO> slips = new SalarySlipBL().GetSalarySlipList(employeeCode, yearInt, fromInt, toInt);

                if (slips == null || slips.Count == 0)
                {
                    ShowResults(false);
                    ClientScript.RegisterStartupScript(GetType(), "NoSlip",
                        "showUserSavedMessage('Error', 'No salary slip found for the selected period.');", true);
                    return;
                }

                gvSlips.DataSource = slips;
                gvSlips.DataBind();

                // Keep selection for the per-row PDF download.
                ViewState["ss_empcode"] = employeeCode;
                ViewState["ss_year"] = yearInt;

                ShowResults(true);
            }
            catch (Exception ex)
            {
                ShowResults(false);
                new CommonBL().fnStoreErrorLog("SalarySlip", "btnSearch_Click", ex.Message + " Strace=" + ex.StackTrace, UserId);
                ClientScript.RegisterStartupScript(GetType(), "Error",
                    "showUserSavedMessage('Error', 'Unable to load salary slips. Please try again.');", true);
            }
        }

        private void ShowResults(bool show)
        {
            pnlResults.Visible = show;
            pnlNoData.Visible = !show;
        }

        protected void gvSlips_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            try
            {
                if (e.CommandName != "DownloadSlip")
                {
                    return;
                }

                // Row carries "employeecode|year|month" (employee code is a string).
                string[] parts = Convert.ToString(e.CommandArgument).Split('|');
                int year, month;
                if (parts.Length != 3 ||
                    string.IsNullOrWhiteSpace(parts[0]) ||
                    !int.TryParse(parts[1], out year) ||
                    !int.TryParse(parts[2], out month))
                {
                    return;
                }

                string employeeCode = parts[0].Trim();

                // SP returns the salary-slip HTML; we render it to PDF.
                string html = new SalarySlipBL().GetSalarySlipHtml(employeeCode, year, month);
                if (string.IsNullOrWhiteSpace(html) || html.Contains("No payslip found"))
                {
                    ClientScript.RegisterStartupScript(GetType(), "NoSlipRow",
                        "showUserSavedMessage('Error', 'Salary slip not found.');", true);
                    return;
                }

                byte[] pdfBytes = GeneratePdfFromHtml(html);
                string fileName = "SalarySlip_" + employeeCode + "_" + MonthName(month) + "_" + year + ".pdf";

                Response.Clear();
                Response.ContentType = "application/pdf";
                Response.AddHeader("Content-Disposition", $"attachment; filename=\"{fileName}\"");
                Response.BinaryWrite(pdfBytes);
                Response.End();
            }
            catch (System.Threading.ThreadAbortException)
            {
                // Raised by Response.End(); expected, safe to ignore.
            }
            catch (Exception ex)
            {
                new CommonBL().fnStoreErrorLog("SalarySlip", "gvSlips_RowCommand", ex.Message + " Strace=" + ex.StackTrace, UserId);
                ClientScript.RegisterStartupScript(GetType(), "Error",
                    "showUserSavedMessage('Error', 'Unable to download salary slip.');", true);
            }
        }

        // Renders the salary-slip HTML returned by the SP into a PDF.
        private byte[] GeneratePdfFromHtml(string html)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                HtmlConverter.ConvertToPdf(html, ms);
                return ms.ToArray();
            }
        }

        // --- Markup helpers (protected so the .aspx databinding expressions can call them) ---

        protected string GetMonthLabel(object monthValue)
        {
            int m;
            if (int.TryParse(Convert.ToString(monthValue), out m))
            {
                return MonthName(m);
            }
            return Convert.ToString(monthValue);
        }

        protected string FormatMoney(object value)
        {
            decimal d;
            decimal.TryParse(Convert.ToString(value), out d);
            return "&#8377; " + d.ToString("N2");
        }

        private string MonthName(int month)
        {
            if (month < 1 || month > 12) return string.Empty;
            return CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(month);
        }

        // Maps the selected employee (dropdown value = user id) to their employee code.
        private int ResolveEmployeeCode(int userId)
        {
            try
            {
                var emp = (new UserDetailsBL().ViewAllUsers() ?? new List<UserDetailsDO>())
                    .FirstOrDefault(u => u != null && u.UserId == userId);
                int code;
                if (emp != null && int.TryParse(Convert.ToString(emp.EmployeeCode), out code))
                {
                    return code;
                }
            }
            catch (Exception ex)
            {
                new CommonBL().fnStoreErrorLog("SalarySlip", "ResolveEmployeeCode", ex.Message + " Strace=" + ex.StackTrace, UserId);
            }
            return 0;
        }
    }
}
