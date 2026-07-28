using DataObject;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;

namespace ProcessModel
{
    public class AttendanceBL
    {
        protected string UserId = null;
        private string DBName = ConfigurationManager.AppSettings["DBName"];
        private static string MySqlconnection = ConfigurationManager.ConnectionStrings["MysqlConnection"] != null
            ? ConfigurationManager.ConnectionStrings["MysqlConnection"].ConnectionString
            : string.Empty;

        // Employee list for the Attendance Details filter dropdown, via
        // sp_get_attendance_employee_dropdown (Database\sp_get_attendance_employee_dropdown.sql).
        public List<DropDownData> GetEmployeeDropdown()
        {
            List<DropDownData> dropDownData = new List<DropDownData>();
            try
            {
                getDrtolist getDrtolistParam = new getDrtolist();
                List<MySqlParameter> mysqlParameters = new List<MySqlParameter>();
                dropDownData = getDrtolistParam.getdatafromreder<DropDownData>(
                    DataClass.GetDataReaderFromSpWithParam(mysqlParameters, DBName, "sp_get_attendance_employee_dropdown"));
            }
            catch (Exception ex)
            {
                CommonBL errorlog = new CommonBL();
                errorlog.fnStoreErrorLog("AttendanceBL", "GetEmployeeDropdown", "Exception Message: " + ex.Message + " StackTrace: " + ex.StackTrace, UserId);
            }

            return dropDownData;
        }

        // Attendance list via sp_get_attendance_list (Database\sp_get_attendance_list.sql),
        // which only filters by employee. Mapped manually (not via the getdatafromreder<T>
        // reflection mapper) because LoginTime/LogoutTime are nullable TimeSpan, which
        // Convert.ChangeType can't target. employeeId = 0 means "all employees".
        //
        // fromDate/toDate (advance search) are applied here in C#, not in the SP, by design
        // - null on either side means no bound on that side of the login_date range.
        public List<EmpAttendanceDO> GetAttendanceList(int employeeId, DateTime? fromDate = null, DateTime? toDate = null)
        {
            List<EmpAttendanceDO> list = new List<EmpAttendanceDO>();

            try
            {
                using (MySqlConnection con = new MySqlConnection(MySqlconnection))
                using (MySqlCommand cmd = new MySqlCommand("sp_get_attendance_list", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@p_employee_id", employeeId);
                    cmd.Parameters.AddWithValue("@p_from_date", fromDate);
                    cmd.Parameters.AddWithValue("@p_to_date", toDate);
                    con.Open();

                    using (MySqlDataReader dr = cmd.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            list.Add(new EmpAttendanceDO
                            {
                                UserId = Convert.ToInt32(dr["UserId"]),
                                EmployeeCode = dr["EmployeeCode"] == DBNull.Value ? string.Empty : Convert.ToString(dr["EmployeeCode"]),
                                EmployeeName = dr["EmployeeName"] == DBNull.Value ? string.Empty : Convert.ToString(dr["EmployeeName"]),
                                LoginDate = Convert.ToDateTime(dr["LoginDate"]),
                                LoginTime = ParseTimeValue(dr["LoginTime"]),
                                LogoutTime = ParseTimeValue(dr["LogoutTime"]),
                                WorkedHoursDisplay = dr["WorkedHoursDisplay"] == DBNull.Value ? null : Convert.ToString(dr["WorkedHoursDisplay"]),
                                IsBelowMinimum = dr["IsBelowMinimum"] == DBNull.Value ? (bool?)null : Convert.ToInt32(dr["IsBelowMinimum"]) == 1
                            });
                        }
                    }
                }

                if (fromDate.HasValue)
                {
                    DateTime fromDateOnly = fromDate.Value.Date;
                    list = list.FindAll(a => a.LoginDate.Date >= fromDateOnly);
                }

                if (toDate.HasValue)
                {
                    DateTime toDateOnly = toDate.Value.Date;
                    list = list.FindAll(a => a.LoginDate.Date <= toDateOnly);
                }
            }
            catch (Exception ex)
            {
                CommonBL errorlog = new CommonBL();
                errorlog.fnStoreErrorLog("AttendanceBL", "GetAttendanceList", "Exception Message: " + ex.Message + " StackTrace: " + ex.StackTrace, UserId);
            }

            return list;
        }

        // login_time/logout_time may come back as TimeSpan (MySQL TIME column) or as a
        // plain string, depending on how the column is defined - handle both.
        private TimeSpan? ParseTimeValue(object value)
        {
            if (value == null || value == DBNull.Value)
            {
                return null;
            }

            if (value is TimeSpan)
            {
                return (TimeSpan)value;
            }

            if (value is DateTime)
            {
                return ((DateTime)value).TimeOfDay;
            }

            TimeSpan parsed;
            if (TimeSpan.TryParse(Convert.ToString(value), out parsed))
            {
                return parsed;
            }

            return null;
        }
    }
}
