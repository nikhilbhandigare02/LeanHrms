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
using iText.IO.Font.Constants;
using iText.IO.Image;
using iText.Kernel.Colors;
using iText.Kernel.Font;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas.Draw;
using iText.Layout;
using iText.Layout.Borders;
using iText.Layout.Element;
using iText.Layout.Properties;
using ITable = iText.Layout.Element.Table;
using IImage = iText.Layout.Element.Image;
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

        // All active employees across companies (per requested scope).
        private void BindEmployees()
        {
            try
            {
                UserDetailsBL userBL = new UserDetailsBL();
                var activeUsers = (userBL.ViewAllUsers() ?? new List<UserDetailsDO>())
                    .Where(u => u != null && u.Isactive && u.UserId > 0)
                    .OrderBy(u => (u.user_fullname ?? string.Empty).Trim())
                    .ToList();

                ddlEmployee.Items.Clear();
                ddlEmployee.Items.Add(new ListItem("Select Employee", ""));
                foreach (var u in activeUsers)
                {
                    string name = string.IsNullOrWhiteSpace(u.user_fullname) ? u.Username : u.user_fullname;
                    ddlEmployee.Items.Add(new ListItem(name, u.UserId.ToString()));
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
                int selectedUserId = Convert.ToInt32(employeeValue);

                if (fromInt > toInt)
                {
                    ShowResults(false);
                    ClientScript.RegisterStartupScript(GetType(), "RangeError",
                        "showUserSavedMessage('Error', 'From Month cannot be after To Month.');", true);
                    return;
                }

                List<SalarySlipDO> slips = new SalarySlipBL().GetSalarySlipList(selectedUserId, yearInt, fromInt, toInt);

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
                ViewState["ss_userId"] = selectedUserId;
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

                int month;
                if (!int.TryParse(Convert.ToString(e.CommandArgument), out month) || ViewState["ss_userId"] == null)
                {
                    return;
                }

                int userId = Convert.ToInt32(ViewState["ss_userId"]);
                int year = Convert.ToInt32(ViewState["ss_year"]);

                SalarySlipDO slip = new SalarySlipBL().GetSalarySlipList(userId, year, month, month).FirstOrDefault();
                if (slip == null)
                {
                    ClientScript.RegisterStartupScript(GetType(), "NoSlipRow",
                        "showUserSavedMessage('Error', 'Salary slip not found.');", true);
                    return;
                }

                byte[] pdfBytes = GenerateSlipPdf(slip, userId);
                string safeName = (slip.Username ?? "Employee").Replace(" ", "_");
                string fileName = "SalarySlip_" + safeName + "_" + MonthName(month) + "_" + year + ".pdf";

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

        private byte[] GenerateSlipPdf(SalarySlipDO slip, int userId)
        {
            // Department isn't stored on the salary row, so pull it from the employee record.
            string department = string.Empty;
            try
            {
                var emp = (new UserDetailsBL().ViewAllUsers() ?? new List<UserDetailsDO>())
                    .FirstOrDefault(u => u != null && u.UserId == userId);
                if (emp != null) department = emp.department;
            }
            catch { /* department is optional on the slip */ }

            using (MemoryStream ms = new MemoryStream())
            {
                PdfDocument pdf = new PdfDocument(new PdfWriter(ms, new WriterProperties()));
                Document document = new Document(pdf);
                document.SetMargins(42, 42, 42, 42);

                PdfFont boldFont = PdfFontFactory.CreateFont(StandardFonts.HELVETICA_BOLD);
                PdfFont normalFont = PdfFontFactory.CreateFont(StandardFonts.HELVETICA);
                DeviceRgb brand = new DeviceRgb(37, 99, 235);

                // Header: logo + company info
                ITable headerTable = new ITable(UnitValue.CreatePercentArray(new float[] { 45, 55 })).UseAllAvailableWidth();
                string logoPath = Server.MapPath("~/assets/images/alphonsol_logo.png");
                if (File.Exists(logoPath))
                {
                    IImage logo = new IImage(ImageDataFactory.Create(logoPath)).SetWidth(110);
                    headerTable.AddCell(new Cell().Add(logo).SetBorder(Border.NO_BORDER).SetVerticalAlignment(VerticalAlignment.MIDDLE));
                }
                else
                {
                    headerTable.AddCell(new Cell().SetBorder(Border.NO_BORDER));
                }

                Div contactInfo = new Div().SetTextAlignment(TextAlignment.RIGHT);
                contactInfo.Add(new Paragraph("High-street Corporate Center, FB-03, Kapurbawdi Junction, Thane(W)-400601")
                    .SetFont(boldFont).SetFontSize(9).SetMarginBottom(0));
                contactInfo.Add(new Paragraph("Email Address - support@alphonsol.com").SetFont(boldFont).SetFontSize(9).SetMarginBottom(0));
                contactInfo.Add(new Paragraph("Website - www.alphonsol.com").SetFont(boldFont).SetFontSize(9).SetMarginBottom(0));
                headerTable.AddCell(new Cell().Add(contactInfo).SetBorder(Border.NO_BORDER).SetVerticalAlignment(VerticalAlignment.MIDDLE));
                document.Add(headerTable);

                LineSeparator line = new LineSeparator(new SolidLine());
                line.SetStrokeColor(brand);
                document.Add(line);

                int monthNum;
                int.TryParse(slip.Month, out monthNum);
                document.Add(new Paragraph("Salary Slip").SetFont(boldFont).SetFontSize(15).SetMarginTop(10).SetMarginBottom(2));
                document.Add(new Paragraph("Pay Period: " + MonthName(monthNum) + " " + slip.Year)
                    .SetFont(normalFont).SetFontSize(10).SetMarginBottom(12));

                // Employee info
                ITable infoTable = new ITable(UnitValue.CreatePercentArray(new float[] { 25, 25, 25, 25 })).UseAllAvailableWidth();
                AddInfoCell(infoTable, boldFont, normalFont, "Employee Name", slip.Username);
                AddInfoCell(infoTable, boldFont, normalFont, "Employee Code", slip.employeecode > 0 ? slip.employeecode.ToString() : "-");
                AddInfoCell(infoTable, boldFont, normalFont, "Designation", slip.DesignationName);
                AddInfoCell(infoTable, boldFont, normalFont, "Department", department);
                document.Add(infoTable.SetMarginBottom(14));

                // Earnings / Deductions
                ITable breakup = new ITable(UnitValue.CreatePercentArray(new float[] { 50, 50 })).UseAllAvailableWidth();

                ITable earn = new ITable(UnitValue.CreatePercentArray(new float[] { 60, 40 })).UseAllAvailableWidth();
                AddSectionHeader(earn, boldFont, brand, "Earnings");
                AddMoneyRow(earn, normalFont, "Basic Salary", slip.BasicSalary);
                AddMoneyRow(earn, normalFont, "House Rent Allowance", slip.HouseRentAllowance);
                AddMoneyRow(earn, normalFont, "Special Allowance", slip.SpecialAllowance);
                AddMoneyRow(earn, normalFont, "Leave Travel Allowance", slip.LeaveTravelAllowance);
                AddMoneyRow(earn, boldFont, "Total Earnings", slip.TotalEarnings);
                breakup.AddCell(new Cell().Add(earn).SetBorder(Border.NO_BORDER).SetPaddingRight(10));

                ITable ded = new ITable(UnitValue.CreatePercentArray(new float[] { 60, 40 })).UseAllAvailableWidth();
                AddSectionHeader(ded, boldFont, brand, "Deductions");
                AddMoneyRow(ded, normalFont, "Professional Tax", slip.ProfessionalTax);
                AddMoneyRow(ded, boldFont, "Total Deductions", slip.TotalDeductions);
                breakup.AddCell(new Cell().Add(ded).SetBorder(Border.NO_BORDER).SetPaddingLeft(10));
                document.Add(breakup.SetMarginBottom(14));

                // Net pay
                ITable netTable = new ITable(UnitValue.CreatePercentArray(new float[] { 60, 40 })).UseAllAvailableWidth();
                netTable.AddCell(new Cell().Add(new Paragraph("Net Salary Payable").SetFont(boldFont).SetFontSize(12))
                    .SetBackgroundColor(brand).SetFontColor(ColorConstants.WHITE).SetPadding(8).SetBorder(Border.NO_BORDER));
                netTable.AddCell(new Cell().Add(new Paragraph("Rs. " + slip.NetPay.ToString("N2")).SetFont(boldFont).SetFontSize(12).SetTextAlignment(TextAlignment.RIGHT))
                    .SetBackgroundColor(brand).SetFontColor(ColorConstants.WHITE).SetPadding(8).SetBorder(Border.NO_BORDER));
                document.Add(netTable);

                document.Add(new Paragraph("\nThis is a system generated salary slip and does not require a signature.")
                    .SetFont(normalFont).SetFontSize(8).SetFontColor(ColorConstants.GRAY));

                document.Close();
                return ms.ToArray();
            }
        }

        private void AddSectionHeader(ITable table, PdfFont boldFont, DeviceRgb brand, string title)
        {
            table.AddCell(new Cell(1, 2).Add(new Paragraph(title).SetFont(boldFont).SetFontSize(12))
                .SetBorder(Border.NO_BORDER).SetBorderBottom(new SolidBorder(brand, 1.5f)).SetPaddingBottom(4));
        }

        private void AddMoneyRow(ITable table, PdfFont font, string label, decimal value)
        {
            table.AddCell(new Cell().Add(new Paragraph(label).SetFont(font).SetFontSize(9)).SetBorder(Border.NO_BORDER).SetPadding(3));
            table.AddCell(new Cell().Add(new Paragraph("Rs. " + value.ToString("N2")).SetFont(font).SetFontSize(9).SetTextAlignment(TextAlignment.RIGHT))
                .SetBorder(Border.NO_BORDER).SetPadding(3));
        }

        private void AddInfoCell(ITable table, PdfFont boldFont, PdfFont normalFont, string label, string value)
        {
            Cell cell = new Cell().SetBorder(Border.NO_BORDER);
            cell.Add(new Paragraph(label).SetFont(normalFont).SetFontSize(8).SetFontColor(ColorConstants.GRAY).SetMarginBottom(0));
            cell.Add(new Paragraph(string.IsNullOrWhiteSpace(value) ? "-" : value).SetFont(boldFont).SetFontSize(10).SetMarginTop(0));
            table.AddCell(cell);
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
    }
}
