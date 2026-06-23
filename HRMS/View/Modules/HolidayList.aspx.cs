using DataObject;
using ExcelDataReader;
using ProcessModel;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace HRMS.View.Modules
{
    public partial class HolidayList : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["userId"] == null)
            {
                Response.Redirect("~/view/authentication/login.aspx", false);
                return;
            }

            if (!IsPostBack)
            {
                BindHolidayList();
            }
        }

        protected void btnUpload_Click(object sender, EventArgs e)
        {
            ClearMessage();

            if (!fileUpload.HasFile)
            {
                ShowError("Please select an Excel file.");
                return;
            }

            string fileExt = System.IO.Path.GetExtension(fileUpload.FileName).ToLower();
            if (fileExt != ".xlsx" && fileExt != ".xls")
            {
                ShowError("Only Excel File Allowed (.xlsx, .xls)");
                return;
            }

            try
            {
                List<HolidayListDO> holidays = ReadHolidayExcel();
                if (holidays.Count == 0)
                {
                    ShowError("No holiday records found in uploaded Excel.");
                    return;
                }

                int companyId = GetCompanyId();
                int userId = GetUserId();
                if (companyId <= 0)
                {
                    ShowError("Company id not found for logged-in user.");
                    return;
                }

                HolidayListBL bl = new HolidayListBL();
                bl.DeactivateCompanyHolidays(companyId, userId);

                foreach (HolidayListDO holiday in holidays)
                {
                    holiday.company_id = companyId;
                    holiday.inserted_by = userId;
                    holiday.inserted_date = DateTime.Now;
                    holiday.updated_by = userId;
                    holiday.updated_date = DateTime.Now;
                    bl.InsertHoliday(holiday);
                }

                BindHolidayList();
                ShowSuccess("Holiday list uploaded successfully.");
                ScriptManager.RegisterStartupScript(this, GetType(), "HolidayUploadSuccess",
                    "Swal.fire({ icon: 'success', title: 'Success', text: 'Holiday list uploaded successfully.', timer: 2500, showConfirmButton: false });", true);
            }
            catch (Exception ex)
            {
                ShowError("Holiday upload failed. " + ex.Message);
            }
        }

        protected void gvHolidayList_RowEditing(object sender, GridViewEditEventArgs e)
        {
            gvHolidayList.EditIndex = e.NewEditIndex;
            BindHolidayList();
        }

        protected void gvHolidayList_RowCancelingEdit(object sender, GridViewCancelEditEventArgs e)
        {
            gvHolidayList.EditIndex = -1;
            BindHolidayList();
        }

        protected void gvHolidayList_RowUpdating(object sender, GridViewUpdateEventArgs e)
        {
            ClearMessage();

            try
            {
                GridViewRow row = gvHolidayList.Rows[e.RowIndex];
                int holidayId = Convert.ToInt32(gvHolidayList.DataKeys[e.RowIndex].Value);
                DateTime holidayDate = Convert.ToDateTime(((TextBox)row.FindControl("txtDate")).Text.Trim());
                string day = ((TextBox)row.FindControl("txtDay")).Text.Trim();
                string holidayName = ((TextBox)row.FindControl("txtHoliday")).Text.Trim();

                HolidayListDO holiday = new HolidayListDO
                {
                    holiday_id = holidayId,
                    holiday_date = holidayDate,
                    holiday_day = day,
                    holiday_name = holidayName,
                    company_id = GetCompanyId(),
                    updated_by = GetUserId(),
                    updated_date = DateTime.Now
                };

                HolidayListBL bl = new HolidayListBL();
                bl.UpdateHoliday(holiday);

                gvHolidayList.EditIndex = -1;
                BindHolidayList();
                ShowSuccess("Holiday updated successfully.");
            }
            catch (Exception ex)
            {
                ShowError("Holiday update failed. " + ex.Message);
            }
        }

        protected void gvHolidayList_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName != "SoftDelete")
            {
                return;
            }

            ClearMessage();

            try
            {
                int holidayId = Convert.ToInt32(e.CommandArgument);
                HolidayListBL bl = new HolidayListBL();
                bl.DeleteHoliday(holidayId, GetCompanyId(), GetUserId());
                BindHolidayList();
                ShowSuccess("Holiday deleted successfully.");
            }
            catch (Exception ex)
            {
                ShowError("Holiday delete failed. " + ex.Message);
            }
        }

        private void BindHolidayList()
        {
            HolidayListBL bl = new HolidayListBL();
            gvHolidayList.DataSource = bl.GetHolidayList(GetCompanyId());
            gvHolidayList.DataBind();
        }

        private List<HolidayListDO> ReadHolidayExcel()
        {
            List<HolidayListDO> holidays = new List<HolidayListDO>();

            using (var stream = fileUpload.FileContent)
            using (var reader = ExcelReaderFactory.CreateReader(stream))
            {
                DataSet dataSet = reader.AsDataSet();
                if (dataSet == null || dataSet.Tables.Count == 0)
                {
                    throw new Exception("Excel file does not contain any sheet.");
                }

                DataTable dt = dataSet.Tables[0];
                if (dt.Columns.Count < 4)
                {
                    throw new Exception("Excel must contain Sr No, Date, Day, and Holiday columns.");
                }

                foreach (DataRow row in dt.Rows)
                {
                    if (IsHeaderOrEmptyRow(row))
                    {
                        continue;
                    }

                    int srNo;
                    DateTime holidayDate;
                    string holidayDay = Convert.ToString(row[2]).Trim();
                    if (!int.TryParse(Convert.ToString(row[0]).Trim(), out srNo) || !TryParseExcelDate(row[1], out holidayDate))
                    {
                        continue;
                    }

                    holidayDate = CorrectDateByDayName(holidayDate, holidayDay);

                    holidays.Add(new HolidayListDO
                    {
                        sr_no = srNo,
                        holiday_date = holidayDate,
                        holiday_day = holidayDay,
                        holiday_name = Convert.ToString(row[3]).Trim()
                    });
                }
            }

            return holidays;
        }

        private bool IsHeaderOrEmptyRow(DataRow row)
        {
            string firstColumn = Convert.ToString(row[0]).Trim();
            return string.IsNullOrEmpty(firstColumn) || firstColumn.Equals("Sr No", StringComparison.OrdinalIgnoreCase);
        }

        private bool TryParseExcelDate(object value, out DateTime date)
        {
            date = DateTime.MinValue;
            string text = Convert.ToString(value).Trim();

            if (value is DateTime)
            {
                date = (DateTime)value;
                return true;
            }

            double serialDate;
            if (double.TryParse(text, out serialDate))
            {
                date = DateTime.FromOADate(serialDate);
                return true;
            }

            string[] formats =
            {
                "dd-MM-yyyy",
                "dd/MM/yyyy",
                "dd.MM.yyyy",
                "yyyy-MM-dd",
                "yyyy/MM/dd",
                "yyyy.MM.dd"
            };

            return DateTime.TryParseExact(text, formats, CultureInfo.InvariantCulture, DateTimeStyles.None, out date);
        }

        private DateTime CorrectDateByDayName(DateTime parsedDate, string uploadedDay)
        {
            if (string.IsNullOrWhiteSpace(uploadedDay))
            {
                return parsedDate;
            }

            if (IsSameDayName(parsedDate, uploadedDay))
            {
                return parsedDate;
            }

            if (parsedDate.Day <= 12 && parsedDate.Month <= 12)
            {
                DateTime swappedDate = new DateTime(parsedDate.Year, parsedDate.Day, parsedDate.Month);
                if (IsSameDayName(swappedDate, uploadedDay))
                {
                    return swappedDate;
                }
            }

            return parsedDate;
        }

        private bool IsSameDayName(DateTime date, string uploadedDay)
        {
            return date.ToString("dddd", CultureInfo.InvariantCulture).Equals(uploadedDay.Trim(), StringComparison.OrdinalIgnoreCase);
        }

        private int GetUserId()
        {
            int userId = 0;
            int.TryParse(Convert.ToString(Session["userId"]), out userId);
            return userId;
        }

        private int GetCompanyId()
        {
            string[] keys = { "company_id", "companyId", "CompanyId", "CompanyID" };
            foreach (string key in keys)
            {
                if (Session[key] != null)
                {
                    int companyId = 0;
                    int.TryParse(Convert.ToString(Session[key]), out companyId);
                    if (companyId > 0)
                    {
                        return companyId;
                    }
                }
            }

            return 0;
        }

        private void ClearMessage()
        {
            lblMessage.Text = "";
            lblError.Text = "";
        }

        private void ShowSuccess(string message)
        {
            lblMessage.Text = message;
            lblError.Text = "";
        }

        private void ShowError(string message)
        {
            lblError.Text = message;
            lblMessage.Text = "";
        }
    }
}
