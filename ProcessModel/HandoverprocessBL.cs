using DataObject;
using MySql.Data.MySqlClient;
using MySqlX.XDevAPI;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace ProcessModel
{
    public class HandoverprocessBL
    {
        protected string UserId = null;
        private string DBName = ConfigurationManager.AppSettings["DBName"];
        private static string MySqlconnection = ConfigurationManager.ConnectionStrings["MysqlConnection"].ConnectionString;
        private static string Sqlconnection = ConfigurationManager.ConnectionStrings["Sqlconnection"] != null
            ? ConfigurationManager.ConnectionStrings["Sqlconnection"].ConnectionString
            : string.Empty;

        public List<ResignationDO> GetEmployeeResignationDetails(int reportingManagerId)
        {
            List<ResignationDO> listdata = new List<ResignationDO>();
            if (string.IsNullOrWhiteSpace(Sqlconnection))
            {
                return listdata;
            }

            try
            {
                string normalized = NormalizeMySqlConnectionString(Sqlconnection);
                using (MySqlConnection con = new MySqlConnection(normalized))
                {
                    con.Open();
                    using (MySqlCommand cmd = new MySqlCommand("Sp_GetEmployeeResignationDetailsHR", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        using (MySqlDataReader dr = cmd.ExecuteReader())
                        {
                            listdata = MapResignationRows(dr);
                        }
                    }
                }
            }
            catch (MySqlException exParam)
            {
                // Fallback for SP variants that take reporting manager as input.
                if (exParam.Message != null &&
                    (exParam.Message.IndexOf("expects", StringComparison.OrdinalIgnoreCase) >= 0
                    || exParam.Message.IndexOf("arguments", StringComparison.OrdinalIgnoreCase) >= 0))
                {
                    try
                    {
                        string normalized = NormalizeMySqlConnectionString(Sqlconnection);
                        using (MySqlConnection con = new MySqlConnection(normalized))
                        using (MySqlCommand cmd = new MySqlCommand("Sp_GetEmployeeResignationDetailsHR", con))
                        {
                            cmd.CommandType = CommandType.StoredProcedure;
                            if (reportingManagerId > 0)
                            {
                                cmd.Parameters.AddWithValue("@p_reporting_manager_id", reportingManagerId);
                            }
                            con.Open();
                            using (MySqlDataReader dr = cmd.ExecuteReader())
                            {
                                listdata = MapResignationRows(dr);
                            }
                        }
                    }
                    catch (Exception exFallback)
                    {
                        CommonBL errorlog = new CommonBL();
                        errorlog.fnStoreErrorLog(
                            "HandoverprocessBL",
                            "GetEmployeeResignationDetails_Fallback",
                            "Exception Message=" + exFallback.Message + " Strace=" + exFallback.StackTrace,
                            UserId
                        );
                    }
                }
                else
                {
                    CommonBL errorlog = new CommonBL();
                    errorlog.fnStoreErrorLog(
                        "HandoverprocessBL",
                        "GetEmployeeResignationDetails",
                        "Exception Message=" + exParam.Message + " Strace=" + exParam.StackTrace,
                        UserId
                    );
                }
            }
            catch (Exception ex)
            {
                CommonBL errorlog = new CommonBL();
                errorlog.fnStoreErrorLog(
                    "HandoverprocessBL",
                    "GetEmployeeResignationDetails",
                    "Exception Message=" + ex.Message + " Strace=" + ex.StackTrace,
                    UserId
                );
            }

            return listdata;
        }

        private List<ResignationDO> MapResignationRows(MySqlDataReader dr)
        {
            List<ResignationDO> listdata = new List<ResignationDO>();
            while (dr.Read())
            {
                listdata.Add(new ResignationDO
                {
                    UserId = GetIntSafe(dr, "user_id"),
                    EmployeeResignationId = GetIntSafe(dr, "employee_resignation_id"),
                    resignation_date = GetDateSafe(dr, "resignation_date"),
                    notice_period_days = GetIntSafe(dr, "notice_period_days"),
                    last_working_date = GetDateSafe(dr, "last_working_date"),
                    reason = GetStringSafe(dr, "reason"),
                    hr_status = string.IsNullOrWhiteSpace(GetStringSafe(dr, "status")) ? "Pending" : GetStringSafe(dr, "status"),
                    remarks = GetStringSafe(dr, "remarks"),
                    action_date = GetNullableDateSafe(dr, "action_date"),
                    reporting_manager = GetIntSafe(dr, "reporting_manager"),
                    EmployeeName = GetStringSafe(dr, "emp_name"),
                    EmployeeEmail = GetStringSafe(dr, "email_id"),
                    reporting_manager_name = GetStringSafe(dr, "reporting_manager_name"),
                    project_status = GetStringSafe(dr, "project_status"),
                    pending_days = GetIntSafe(dr, "pending_days"),
                    pending_days_display = GetStringSafe(dr, "pending_days_display"),
                    pending_hours = GetIntSafe(dr, "pending_hours"),
                    approval_hours = GetIntSafe(dr, "approval_hours"),
                    approval_days = GetIntSafe(dr, "approval_days"),
                    status_updated_flag = GetIntSafe(dr, "status_updated_flag"),
                    authority_status = GetStringSafe(dr, "authority_status")
                });
            }

            return listdata;
        }

        private int GetOrdinalIgnoreCase(IDataRecord dr, string col)
        {
            for (int i = 0; i < dr.FieldCount; i++)
            {
                if (string.Equals(dr.GetName(i), col, StringComparison.OrdinalIgnoreCase))
                {
                    return i;
                }
            }
            return -1;
        }

        private string GetStringSafe(IDataRecord dr, string col)
        {
            int i = GetOrdinalIgnoreCase(dr, col);
            return (i < 0 || dr.IsDBNull(i)) ? string.Empty : Convert.ToString(dr.GetValue(i));
        }

        private int GetIntSafe(IDataRecord dr, string col)
        {
            int i = GetOrdinalIgnoreCase(dr, col);
            return (i < 0 || dr.IsDBNull(i)) ? 0 : Convert.ToInt32(dr.GetValue(i));
        }

        private DateTime GetDateSafe(IDataRecord dr, string col)
        {
            int i = GetOrdinalIgnoreCase(dr, col);
            return (i < 0 || dr.IsDBNull(i)) ? DateTime.MinValue : Convert.ToDateTime(dr.GetValue(i));
        }

        private DateTime? GetNullableDateSafe(IDataRecord dr, string col)
        {
            int i = GetOrdinalIgnoreCase(dr, col);
            return (i < 0 || dr.IsDBNull(i)) ? (DateTime?)null : Convert.ToDateTime(dr.GetValue(i));
        }

        private string NormalizeMySqlConnectionString(string raw)
        {
            if (string.IsNullOrWhiteSpace(raw))
            {
                return raw;
            }

            var pairs = raw.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
            var dict = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            foreach (var pair in pairs)
            {
                var idx = pair.IndexOf('=');
                if (idx <= 0) continue;
                var key = pair.Substring(0, idx).Trim();
                var value = pair.Substring(idx + 1).Trim();
                dict[key] = value;
            }

            string server = dict.ContainsKey("Server") ? dict["Server"] :
                            dict.ContainsKey("Data Source") ? dict["Data Source"] : string.Empty;
            string port = dict.ContainsKey("Port") ? dict["Port"] : "3306";
            string database = dict.ContainsKey("Database") ? dict["Database"] :
                              dict.ContainsKey("Initial Catalog") ? dict["Initial Catalog"] : string.Empty;
            string user = dict.ContainsKey("User Id") ? dict["User Id"] :
                          dict.ContainsKey("uid") ? dict["uid"] :
                          dict.ContainsKey("User") ? dict["User"] : string.Empty;
            string password = dict.ContainsKey("Password") ? dict["Password"] :
                              dict.ContainsKey("pwd") ? dict["pwd"] : string.Empty;

            return string.Format(
                "Server={0};Port={1};Database={2};User={3};Password={4};Persist Security Info=True;Convert Zero Datetime=True;",
                server, port, database, user, password
            );
        }
        public List<HandoverProcessDO> SaveHandoverProcess(HandoverProcessDO obj)
        {
            List<HandoverProcessDO> list = new List<HandoverProcessDO>();
            getDrtolist getDrtolistParam = new getDrtolist();
            List<MySqlParameter> param = new List<MySqlParameter>();

            try
            {
                //param.Add(new MySqlParameter("@type", "SaveHandover"));
                param.Add(new MySqlParameter("@p_employee_resignation_id", obj.EmployeeResignationId));
                param.Add(new MySqlParameter("@p_user_id", obj.UserId));
                param.Add(new MySqlParameter("@p_PendriveBackup", obj.PendriveBackup ? 1 : 0));
                param.Add(new MySqlParameter("@p_LaptopWithCharger", obj.LaptopWithCharger ? 1 : 0));
                param.Add(new MySqlParameter("@p_ContactDetailsShared", obj.ContactDetailsShared ? 1 : 0));
                param.Add(new MySqlParameter("@p_DiarySubmitted", obj.DiarySubmitted ? 1 : 0));
                param.Add(new MySqlParameter("@p_ID_Card", obj.IDCard ? 1 : 0));

                param.Add(new MySqlParameter("@p_HR_Remark", obj.HR_Remark));
                param.Add(new MySqlParameter("@p_inserted_by", obj.InsertedBy));

                list = getDrtolistParam.getdatafromreder<HandoverProcessDO>(
                  DataClass.GetDataReaderFromSpWithParam(param, DBName, "SP_Save_Handover_Process")
              );
            }
            catch (Exception ex)
            {
                CommonBL errorlog = new CommonBL();
                errorlog.fnStoreErrorLog(
                    "HandoverProcessBL",
                    "SaveHandoverProcess",
                    ex.Message,
                    UserId
                );
            }

            return list;
        }
        public HandoverProcessDO GetHandoverByResignationId(int resignationId)
        {

            getDrtolist dr = new getDrtolist();
            List<MySqlParameter> param = new List<MySqlParameter>();

            //param.Add(new MySqlParameter("@type", "GetByResignationId"));
            param.Add(new MySqlParameter("@p_employee_resignation_id", resignationId));

            var list = dr.getdatafromreder<HandoverProcessDO>(
                DataClass.GetDataReaderFromSpWithParam(
                    param,
                    DBName,
                    "SP_Get_Handover_Process_By_ResignationId"
                ));

            return list != null && list.Count > 0 ? list[0] : null;
        }

        public List<TerminationProcessDO> SaveEmployeeTermination(TerminationProcessDO obj)
        {
            List<TerminationProcessDO> list = new List<TerminationProcessDO>();
            getDrtolist getDrtolistParam = new getDrtolist();
            List<MySqlParameter> param = new List<MySqlParameter>();

            try
            {
                param.Add(new MySqlParameter("@p_company_id", obj.CompanyId));
                param.Add(new MySqlParameter("@p_user_id", obj.UserId));
                param.Add(new MySqlParameter("@p_employee_code", obj.EmployeeCode));
                param.Add(new MySqlParameter("@p_termination_date", obj.TerminationDate));
                param.Add(new MySqlParameter("@p_termination_reason", obj.termination_reason ?? ""));
                param.Add(new MySqlParameter("@p_PerformanceRating", obj.PerformanceRating.HasValue ? obj.PerformanceRating.Value : (object)DBNull.Value));
                param.Add(new MySqlParameter("@p_NoticePeriodDays", obj.NoticePeriodDays.HasValue ? obj.NoticePeriodDays.Value : (object)DBNull.Value));
                param.Add(new MySqlParameter("@p_TerminationLetter", string.IsNullOrEmpty(obj.TerminationLetter) ? (object)DBNull.Value : obj.TerminationLetter));
                param.Add(new MySqlParameter("@p_ResponseDeadline", obj.ResponseDeadline.HasValue ? obj.ResponseDeadline.Value : (object)DBNull.Value));
                param.Add(new MySqlParameter("@p_NoticeLetter", string.IsNullOrEmpty(obj.NoticeLetter) ? (object)DBNull.Value : obj.NoticeLetter));
                param.Add(new MySqlParameter("@p_inserted_by", obj.InsertedBy));


                list = getDrtolistParam.getdatafromreder<TerminationProcessDO>(
                    DataClass.GetDataReaderFromSpWithParam(
                        param,
                        DBName,
                        "SP_Save_Employee_Termination"
                    )
                );
            }
            catch (Exception ex)
            {
                CommonBL errorlog = new CommonBL();
                errorlog.fnStoreErrorLog(
                    "TerminationProcessBL",
                    "SaveEmployeeTermination",
                    ex.Message,
                    UserId
                );
            }

            return list;
        }

        public List<UserDetailsDO> GetTerminationList(int companyId)
        {
            List<UserDetailsDO> list = new List<UserDetailsDO>();

            try
            {
                List<MySqlParameter> param = new List<MySqlParameter>();

                param.Add(new MySqlParameter("@p_company_id", companyId));

                var reader = DataClass.GetDataReaderFromSpWithParam(
                    param,
                    DBName,
                    "SP_GetTerminationDetails"
                );

                while (reader.Read())
                {
                    UserDetailsDO obj = new UserDetailsDO();

                    obj.UserId = Convert.ToInt32(reader["user_id"]);
                    obj.EmployeeCode = reader["employee_code"].ToString();
                    obj.notice_status = reader["notice_status"].ToString();

                    //obj.ResponseDeadline = reader["ResponseDeadline"] == DBNull.Value
                    //    ? (DateTime?)null
                    //    : Convert.ToDateTime(reader["ResponseDeadline"]);
                    obj.TerminationDate = reader["TerminationDate"] == DBNull.Value
                        ? (DateTime?)null
                        : Convert.ToDateTime(reader["TerminationDate"]);

                    
                    list.Add(obj);
                }

                reader.Close(); // ✅ Important: close reader
            }
            catch (Exception ex)
            {
                CommonBL errorlog = new CommonBL();
                errorlog.fnStoreErrorLog(
                    "HandoverprocessBL",
                    "GetTerminationList",
                    ex.Message,
                    UserId
                );
            }

            return list;
        }

        public List<TerminationProcessDO> saveshowcausenotice(TerminationProcessDO obj)
        {
            List<TerminationProcessDO> list = new List<TerminationProcessDO>();
            getDrtolist getDrtolistParam = new getDrtolist();
            List<MySqlParameter> param = new List<MySqlParameter>();

            try
            {
                param.Add(new MySqlParameter("@p_CompanyId", obj.CompanyId));
                param.Add(new MySqlParameter("@p_UserId", obj.UserId));
                param.Add(new MySqlParameter("@p_EmployeeCode", obj.EmployeeCode));
                param.Add(new MySqlParameter("@p_ResponseDeadline", obj.ResponseDeadline.HasValue ? obj.ResponseDeadline.Value : (object)DBNull.Value));
                param.Add(new MySqlParameter("@p_NoticeLetter", string.IsNullOrEmpty(obj.NoticeLetter) ? (object)DBNull.Value : obj.NoticeLetter));
                param.Add(new MySqlParameter("@p_InsertedBy", obj.InsertedBy));


                list = getDrtolistParam.getdatafromreder<TerminationProcessDO>(
                    DataClass.GetDataReaderFromSpWithParam(
                        param,
                        DBName,
                        "SP_SaveShowCauseNotice"
                    )
                );
            }
            catch (Exception ex)
            {
                CommonBL errorlog = new CommonBL();
                errorlog.fnStoreErrorLog(
                    "TerminationProcessBL",
                    "SaveEmployeeTermination",
                    ex.Message,
                    UserId
                );
            }

            return list;
        }
        public string GetShowCauseStatus(string USERID)
        {
            string status = "";

            List<MySqlParameter> param = new List<MySqlParameter>();

            param.Add(new MySqlParameter("@p_user_id", USERID));

            var dr = DataClass.GetDataReaderFromSpWithParam(
                param,
                DBName,
                "SP_GetShowCauseStatus"
            );

            if (dr.Read())
            {
                status = dr["notice_status"].ToString();
            }

            return status;
        }
        public TerminationProcessDO GetTerminationByUserId(int userId)
        {
            TerminationProcessDO data = null;

            List<MySqlParameter> param = new List<MySqlParameter>();
            param.Add(new MySqlParameter("@p_user_id", userId));

            using (var dr = DataClass.GetDataReaderFromSpWithParam(param, DBName, "SP_GetTerminationByUserId"))
            {
                if (dr.Read())
                {
                    data = new TerminationProcessDO
                    {
                        UserId = userId,
                        ResponseDeadline = dr["ResponseDeadline"] != DBNull.Value
                                           ? Convert.ToDateTime(dr["ResponseDeadline"])
                                           : (DateTime?)null
                    };
                }
            }

            return data;
        }



        public void UpdateNoticeStatus(int userId, string status)
        {
            getDrtolist dr = new getDrtolist();

            List<MySqlParameter> param = new List<MySqlParameter>();

            param.Add(new MySqlParameter("@p_user_id", userId));
            param.Add(new MySqlParameter("@p_notice_status", status));

            // Call SP (ignore result)
            dr.getdatafromreder<object>(
                DataClass.GetDataReaderFromSpWithParam(
                    param,
                    DBName,
                    "SP_UpdateNoticeStatusByUserId"
                )
            );
        }

        public ResignationActionResponseDO UpdateResignationActionBySp(int resignationId, string hrAction, string hrRemarks, DateTime? lastWorkingDate, int? extendedNoticeDays, int updatedBy)
        {
            var response = new ResignationActionResponseDO
            {
                Success = false,
                ResponseMsg = "Unable to update resignation action."
            };

            if (resignationId <= 0 || string.IsNullOrWhiteSpace(Sqlconnection))
            {
                response.ResponseMsg = "Invalid resignation request.";
                return response;
            }

            // Try candidate SP names and parameter sets. This avoids hard-coding one DB variant.
            string[] spNames = new[]
            {
                "SP_UpdateResignationAction",
                "Sp_UpdateResignationAction",
                "SP_SaveResignationAction",
                "Sp_SaveResignationAction"
            };

            string lastError = string.Empty;
            foreach (var sp in spNames)
            {
                if (TryExecuteResignationActionSp(sp, resignationId, hrAction, hrRemarks, lastWorkingDate, extendedNoticeDays, updatedBy, out response, out lastError))
                {
                    return response;
                }
            }

            if (!string.IsNullOrWhiteSpace(lastError))
            {
                response.ResponseMsg = "Unable to update resignation action. " + lastError;
            }

            return response;
        }

        private bool TryExecuteResignationActionSp(string spName, int resignationId, string hrAction, string hrRemarks, DateTime? lastWorkingDate, int? extendedNoticeDays, int updatedBy, out ResignationActionResponseDO response, out string errorMessage)
        {
            response = new ResignationActionResponseDO
            {
                Success = false,
                ResponseMsg = "Unable to update resignation action."
            };
            errorMessage = string.Empty;

            try
            {
                string normalized = NormalizeMySqlConnectionString(Sqlconnection);
                using (MySqlConnection con = new MySqlConnection(normalized))
                using (MySqlCommand cmd = new MySqlCommand(spName, con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    AddResignationParams(cmd.Parameters, resignationId, hrAction, hrRemarks, lastWorkingDate, extendedNoticeDays, updatedBy);
                    con.Open();

                    using (var dr = cmd.ExecuteReader())
                    {
                        response = ReadResignationActionResponse(dr, true);
                    }
                }

                return true;
            }
            catch (Exception exMySql)
            {
                // Try SQL style as fallback for secondary connection variants.
                try
                {
                    using (SqlConnection con = new SqlConnection(Sqlconnection))
                    using (SqlCommand cmd = new SqlCommand(spName, con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        AddResignationParams(cmd.Parameters, resignationId, hrAction, hrRemarks, lastWorkingDate, extendedNoticeDays, updatedBy);
                        con.Open();
                        using (var dr = cmd.ExecuteReader())
                        {
                            response = ReadResignationActionResponse(dr, false);
                        }
                    }
                    return true;
                }
                catch (Exception exSql)
                {
                    errorMessage = string.Format(
                        "SP: {0}; MySqlError: {1}; SqlError: {2}",
                        spName,
                        exMySql.Message,
                        exSql.Message
                    );
                    return false;
                }
            }
        }

        private void AddResignationParams(MySqlParameterCollection p, int resignationId, string hrAction, string hrRemarks, DateTime? lastWorkingDate, int? extendedNoticeDays, int updatedBy)
        {
            p.AddWithValue("@p_employee_resignation_id", resignationId);
            p.AddWithValue("@p_hr_action", hrAction ?? string.Empty);
            p.AddWithValue("@p_hr_remarks", string.IsNullOrWhiteSpace(hrRemarks) ? (object)DBNull.Value : hrRemarks);
            p.AddWithValue("@p_last_working_date", lastWorkingDate.HasValue ? (object)lastWorkingDate.Value : DBNull.Value);
            p.AddWithValue("@p_extended_notice_days", extendedNoticeDays.HasValue ? (object)extendedNoticeDays.Value : DBNull.Value);
            p.AddWithValue("@p_updated_by", updatedBy);
        }

        private void AddResignationParams(SqlParameterCollection p, int resignationId, string hrAction, string hrRemarks, DateTime? lastWorkingDate, int? extendedNoticeDays, int updatedBy)
        {
            p.AddWithValue("@p_employee_resignation_id", resignationId);
            p.AddWithValue("@p_hr_action", hrAction ?? string.Empty);
            p.AddWithValue("@p_hr_remarks", string.IsNullOrWhiteSpace(hrRemarks) ? (object)DBNull.Value : hrRemarks);
            p.AddWithValue("@p_last_working_date", lastWorkingDate.HasValue ? (object)lastWorkingDate.Value : DBNull.Value);
            p.AddWithValue("@p_extended_notice_days", extendedNoticeDays.HasValue ? (object)extendedNoticeDays.Value : DBNull.Value);
            p.AddWithValue("@p_updated_by", updatedBy);
        }

        private ResignationActionResponseDO ReadResignationActionResponse(IDataReader dr, bool isSuccessFallback)
        {
            var response = new ResignationActionResponseDO
            {
                Success = isSuccessFallback,
                ResponseMsg = isSuccessFallback ? "Resignation action updated successfully." : "Update failed."
            };

            try
            {
                if (dr.Read())
                {
                    string status = string.Empty;
                    string message = string.Empty;

                    for (int i = 0; i < dr.FieldCount; i++)
                    {
                        string name = dr.GetName(i);
                        if (string.Equals(name, "Status", StringComparison.OrdinalIgnoreCase) ||
                            string.Equals(name, "Success", StringComparison.OrdinalIgnoreCase))
                        {
                            status = Convert.ToString(dr[i]);
                        }

                        if (string.Equals(name, "Remarks", StringComparison.OrdinalIgnoreCase) ||
                            string.Equals(name, "ResponseMsg", StringComparison.OrdinalIgnoreCase) ||
                            string.Equals(name, "Message", StringComparison.OrdinalIgnoreCase))
                        {
                            message = Convert.ToString(dr[i]);
                        }
                    }

                    if (!string.IsNullOrWhiteSpace(status))
                    {
                        response.Success = status.Equals("Success", StringComparison.OrdinalIgnoreCase) ||
                                           status.Equals("1", StringComparison.OrdinalIgnoreCase) ||
                                           status.Equals("true", StringComparison.OrdinalIgnoreCase);
                    }

                    if (!string.IsNullOrWhiteSpace(message))
                    {
                        response.ResponseMsg = message;
                    }
                }
            }
            catch
            {
                // keep fallback defaults
            }

            return response;
        }



    }
}
