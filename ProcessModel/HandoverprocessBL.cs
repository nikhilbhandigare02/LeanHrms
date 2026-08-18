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

        private int? GetNullableIntSafe(IDataRecord dr, string col)
        {
            int i = GetOrdinalIgnoreCase(dr, col);
            return (i < 0 || dr.IsDBNull(i)) ? (int?)null : Convert.ToInt32(dr.GetValue(i));
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
                        "SP_Save_Employee_Terminationlist"
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

                    obj.TerminationDate = reader["TerminationDate"] == DBNull.Value
                        ? (DateTime?)null
                        : Convert.ToDateTime(reader["TerminationDate"]);

                    // Read defensively - the SP may or may not already return
                    // these columns; if not present, they just stay null and
                    // the Termination List screen falls back gracefully.
                    object responseDeadline = SafeGetValue(reader, "ResponseDeadline");
                    obj.ResponseDeadline = responseDeadline != null ? Convert.ToDateTime(responseDeadline) : (DateTime?)null;

                    object performanceRating = SafeGetValue(reader, "PerformanceRating");
                    obj.PerformanceRating = performanceRating != null ? Convert.ToInt32(performanceRating) : (int?)null;

                    object noticePeriodDays = SafeGetValue(reader, "NoticePeriodDays");
                    obj.NoticePeriodDays = noticePeriodDays != null ? Convert.ToInt32(noticePeriodDays) : (int?)null;

                    object reason = SafeGetValue(reader, "termination_reason");
                    obj.termination_reason = reason != null ? Convert.ToString(reason) : null;

                    object letter = SafeGetValue(reader, "TerminationLetter");
                    obj.TerminationLetter = letter != null ? Convert.ToString(letter) : null;

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

        // Reads a column value only if the reader's current result set actually
        // contains it, returning null (rather than throwing) when it doesn't.
        private static object SafeGetValue(IDataReader reader, string columnName)
        {
            try
            {
                int ordinal = reader.GetOrdinal(columnName);
                object value = reader.GetValue(ordinal);
                return value == DBNull.Value ? null : value;
            }
            catch (IndexOutOfRangeException)
            {
                return null;
            }
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

        public HRReviewDO GetHRReviewDetails(int resignationId)
        {
            var model = new HRReviewDO { ResignationId = resignationId };

            if (resignationId <= 0 || string.IsNullOrWhiteSpace(Sqlconnection))
            {
                return model;
            }

            try
            {
                string normalized = NormalizeMySqlConnectionString(Sqlconnection);
                using (MySqlConnection con = new MySqlConnection(normalized))
                using (MySqlCommand cmd = new MySqlCommand("Sp_GetHRReview", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@p_employee_resignation_id", resignationId);
                    con.Open();

                    using (var dr = cmd.ExecuteReader())
                    {
                        // Result set 1: employee_resignation + userm (aliases as returned by sp_GetHRReview)
                        if (dr.Read())
                        {
                            model.ResignationId = GetIntSafe(dr, "ResignationId");
                            model.EmployeeId = GetIntSafe(dr, "UserId");
                            model.EmployeeCode = GetStringSafe(dr, "EmployeeCode");
                            model.EmployeeName = GetStringSafe(dr, "EmployeeName");
                            model.Department = GetStringSafe(dr, "Department");
                            model.Designation = GetStringSafe(dr, "DesignationId");
                            model.ReportingManager = GetStringSafe(dr, "ReportingManager");
                            model.DateOfJoining = GetNullableDateSafe(dr, "DateOfJoining");
                            model.ResignationDate = GetDateSafe(dr, "ResignationDate");
                            model.ProposedLastWorkingDate = GetDateSafe(dr, "ProposedLastWorkingDate");
                            model.Reason = GetStringSafe(dr, "Reason");
                            model.ResignationStatus = GetStringSafe(dr, "ResignationStatus");
                        }

                        // Result set 2: tbl_hr_review (PascalCase columns)
                        if (dr.NextResult() && dr.Read())
                        {
                            model.HRReviewId = GetIntSafe(dr, "HRReviewId");
                            model.NoticeStartDate = GetNullableDateSafe(dr, "NoticeStartDate");
                            model.NoticePeriodRequired = GetStringSafe(dr, "NoticePeriodRequired");
                            model.NoticeDays = GetIntSafe(dr, "NoticeDays");
                            model.BuyoutApplicable = GetStringSafe(dr, "BuyoutApplicable");
                            model.RevisedLastWorkingDate = GetNullableDateSafe(dr, "RevisedLastWorkingDate");
                            model.HRRemarks = GetStringSafe(dr, "HRRemarks");
                            model.Status = GetStringSafe(dr, "Status");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                CommonBL errorlog = new CommonBL();
                errorlog.fnStoreErrorLog(
                    "HandoverprocessBL",
                    "GetHRReviewDetails",
                    "Exception Message=" + ex.Message + " Strace=" + ex.StackTrace,
                    UserId
                );
            }

            return model;
        }

        public HRReviewDO GetHRReviewById(int resignationId)
        {
            HRReviewDO model = null;

            if (resignationId <= 0 || string.IsNullOrWhiteSpace(MySqlconnection))
            {
                return model;
            }

            try
            {
                string normalized = NormalizeMySqlConnectionString(MySqlconnection);
                using (MySqlConnection con = new MySqlConnection(normalized))
                using (MySqlCommand cmd = new MySqlCommand("get_hr_review_by_id", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@p_ResignationId", resignationId);
                    con.Open();

                    using (var dr = cmd.ExecuteReader())
                    {
                        if (dr.Read())
                        {
                            model = new HRReviewDO
                            {
                                HRReviewId = GetIntSafe(dr, "HRReviewId"),
                                ResignationId = GetIntSafe(dr, "ResignationId"),
                                NoticeStartDate = GetNullableDateSafe(dr, "NoticeStartDate"),
                                NoticePeriodRequired = GetStringSafe(dr, "NoticePeriodRequired"),
                                NoticeDays = GetIntSafe(dr, "NoticeDays"),
                                BuyoutApplicable = GetStringSafe(dr, "BuyoutApplicable"),
                                RevisedLastWorkingDate = GetNullableDateSafe(dr, "RevisedLastWorkingDate"),
                                HRRemarks = GetStringSafe(dr, "HRRemarks"),
                                Status = GetStringSafe(dr, "Status")
                            };
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                CommonBL errorlog = new CommonBL();
                errorlog.fnStoreErrorLog(
                    "HandoverprocessBL",
                    "GetHRReviewById",
                    "Exception Message=" + ex.Message + " Strace=" + ex.StackTrace,
                    UserId
                );
            }

            return model;
        }

        public int GetLatestResignationIdForUser(int userId)
        {
            int resignationId = 0;

            if (userId <= 0 || string.IsNullOrWhiteSpace(MySqlconnection))
            {
                return resignationId;
            }

            try
            {
                string normalized = NormalizeMySqlConnectionString(MySqlconnection);
                using (MySqlConnection con = new MySqlConnection(normalized))
                using (MySqlCommand cmd = new MySqlCommand("sp_GetLatestResignationIdByUser", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@p_UserId", userId);
                    con.Open();

                    using (var dr = cmd.ExecuteReader())
                    {
                        if (dr.Read())
                        {
                            resignationId = GetIntSafe(dr, "ResignationId");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                CommonBL errorlog = new CommonBL();
                errorlog.fnStoreErrorLog(
                    "HandoverprocessBL",
                    "GetLatestResignationIdForUser",
                    "Exception Message=" + ex.Message + " Strace=" + ex.StackTrace,
                    UserId
                );
            }

            return resignationId;
        }

        public NoticePeriodDO GetNoticePeriodDetails(int resignationId)
        {
            NoticePeriodDO model = null;

            if (resignationId <= 0 || string.IsNullOrWhiteSpace(MySqlconnection))
            {
                return model;
            }

            try
            {
                string normalized = NormalizeMySqlConnectionString(MySqlconnection);
                using (MySqlConnection con = new MySqlConnection(normalized))
                using (MySqlCommand cmd = new MySqlCommand("sp_GetNoticePeriodDetails", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@p_ResignationId", resignationId);
                    con.Open();

                    using (var dr = cmd.ExecuteReader())
                    {
                        if (dr.Read())
                        {
                            model = new NoticePeriodDO
                            {
                                NoticeStartDate = GetNullableDateSafe(dr, "NoticeStartDate"),
                                NoticeEndDate = GetNullableDateSafe(dr, "NoticeEndDate"),
                                RemainingDays = GetNullableIntSafe(dr, "RemainingDays"),
                                LastWorkingDate = GetNullableDateSafe(dr, "LastWorkingDate"),
                                AttendanceStatus = GetStringSafe(dr, "AttendanceStatus")
                            };
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                CommonBL errorlog = new CommonBL();
                errorlog.fnStoreErrorLog(
                    "HandoverprocessBL",
                    "GetNoticePeriodDetails",
                    "Exception Message=" + ex.Message + " Strace=" + ex.StackTrace,
                    UserId
                );
            }

            return model;
        }

        public HRReviewResponseDO UpdateHRReview(HRReviewDO model, int updatedBy)
        {
            var response = new HRReviewResponseDO { Success = false, ResponseMsg = "Unable to update HR review." };

            if (model == null || model.HRReviewId <= 0 || string.IsNullOrWhiteSpace(MySqlconnection))
            {
                response.ResponseMsg = "Invalid HR review request.";
                return response;
            }

            try
            {
                string normalized = NormalizeMySqlConnectionString(MySqlconnection);
                using (MySqlConnection con = new MySqlConnection(normalized))
                using (MySqlCommand cmd = new MySqlCommand("sp_UpdateHRReview", con))
                
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@p_HRReviewId", model.HRReviewId);
                    cmd.Parameters.AddWithValue("@p_NoticeStartDate", model.NoticeStartDate.HasValue ? (object)model.NoticeStartDate.Value : DBNull.Value);
                    cmd.Parameters.AddWithValue("@p_NoticePeriodRequired", model.NoticePeriodRequired ?? string.Empty);
                    cmd.Parameters.AddWithValue("@p_NoticeDays", model.NoticeDays.HasValue ? (object)model.NoticeDays.Value : DBNull.Value);
                    cmd.Parameters.AddWithValue("@p_BuyoutApplicable", model.BuyoutApplicable ?? string.Empty);
                    cmd.Parameters.AddWithValue("@p_RevisedLastWorkingDate", model.RevisedLastWorkingDate.HasValue ? (object)model.RevisedLastWorkingDate.Value : DBNull.Value);
                    cmd.Parameters.AddWithValue("@p_HRRemarks", model.HRRemarks ?? string.Empty);
                    cmd.Parameters.AddWithValue("@p_UpdatedBy", updatedBy);

                    var outSuccess = new MySqlParameter("@p_Success", MySqlDbType.Byte) { Direction = ParameterDirection.Output };
                    var outMsg = new MySqlParameter("@p_ResponseMsg", MySqlDbType.VarChar, 200) { Direction = ParameterDirection.Output };

                    cmd.Parameters.Add(outSuccess);
                    cmd.Parameters.Add(outMsg);

                    con.Open();
                    cmd.ExecuteNonQuery();

                    response.Success = outSuccess.Value != DBNull.Value && Convert.ToInt32(outSuccess.Value) == 1;
                    response.ResponseMsg = outMsg.Value != DBNull.Value
                        ? Convert.ToString(outMsg.Value)
                        : (response.Success ? "HR Review updated successfully." : "Failed to update HR review.");
                }
            }
            catch (Exception ex)
            {
                CommonBL errorlog = new CommonBL();
                errorlog.fnStoreErrorLog(
                    "HandoverprocessBL",
                    "UpdateHRReview",
                    "Exception Message=" + ex.Message + " Strace=" + ex.StackTrace,
                    UserId
                );
                response.Success = false;
                response.ResponseMsg = "Error occurred while updating HR Review.";
            }

            return response;
        }

        public HRReviewResponseDO SaveHRReviewAndAccept(HRReviewDO model, int updatedBy)
        {
            HRReviewResponseDO response = new HRReviewResponseDO
            {
                Success = false,
                ResponseMsg = "Unable to save HR Review."
            };

            try
            {
                string normalized = NormalizeMySqlConnectionString(MySqlconnection);

                using (MySqlConnection con = new MySqlConnection(normalized))
                {
                    con.Open();

                    using (MySqlCommand cmd = new MySqlCommand("sp_SaveHRReview", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;

                        cmd.Parameters.AddWithValue("p_ResignationId", model.ResignationId);
                        cmd.Parameters.AddWithValue("p_NoticeStartDate",
                            model.NoticeStartDate.HasValue ? (object)model.NoticeStartDate.Value : DBNull.Value);
                        cmd.Parameters.AddWithValue("p_NoticePeriodRequired", model.NoticePeriodRequired);
                        cmd.Parameters.AddWithValue("p_NoticeDays",
                            model.NoticeDays.HasValue ? (object)model.NoticeDays.Value : DBNull.Value);
                        cmd.Parameters.AddWithValue("p_BuyoutApplicable", model.BuyoutApplicable);
                        cmd.Parameters.AddWithValue("p_RevisedLastWorkingDate",
                            model.RevisedLastWorkingDate.HasValue
                                ? (object)model.RevisedLastWorkingDate.Value
                                : DBNull.Value);
                        cmd.Parameters.AddWithValue("p_HRRemarks", model.HRRemarks);
                        cmd.Parameters.AddWithValue("p_UpdatedBy", updatedBy);
                       

                        MySqlParameter pHRReviewId = new MySqlParameter("p_HRReviewId", MySqlDbType.Int32);
                        pHRReviewId.Direction = ParameterDirection.Output;
                        cmd.Parameters.Add(pHRReviewId);

                        MySqlParameter pSuccess = new MySqlParameter("p_Success", MySqlDbType.Byte);
                        pSuccess.Direction = ParameterDirection.Output;
                        cmd.Parameters.Add(pSuccess);

                        MySqlParameter pResponse = new MySqlParameter("p_ResponseMsg", MySqlDbType.VarChar, 200);
                        pResponse.Direction = ParameterDirection.Output;
                        cmd.Parameters.Add(pResponse);

                        cmd.ExecuteNonQuery();

                        response.HRReviewId = pHRReviewId.Value != DBNull.Value
                            ? Convert.ToInt32(pHRReviewId.Value)
                            : 0;

                        response.Success = pSuccess.Value != DBNull.Value &&
                                           Convert.ToInt32(pSuccess.Value) == 1;

                        response.ResponseMsg = pResponse.Value != DBNull.Value
                            ? pResponse.Value.ToString()
                            : "";

                        if (!response.Success)
                            return response;
                    }

                    
                }
            }
            catch (Exception ex)
            {
                CommonBL errorlog = new CommonBL();
                errorlog.fnStoreErrorLog(
                    "HandoverprocessBL",
                    "SaveHRReviewAndAccept",
                    "Exception Message=" + ex.Message +
                    " StackTrace=" + ex.StackTrace,
                    UserId);

                response.Success = false;
                response.ResponseMsg = "Error occurred while saving HR Review.";
            }

            return response;
        }

        // Recipients/CC/BCC/Subject/Body/LetterHtml are all produced by
        // sp_get_resignation_accepted_mail_details, not built in C#, so HR can change
        // wording/recipients without a code change.
        public ResignationMailDO GetResignationAcceptedMailDetails(int resignationId)
        {
            ResignationMailDO mail = null;

            if (resignationId <= 0 || string.IsNullOrWhiteSpace(MySqlconnection))
            {
                return mail;
            }

            try
            {
                string normalized = NormalizeMySqlConnectionString(MySqlconnection);
                using (MySqlConnection con = new MySqlConnection(normalized))
                using (MySqlCommand cmd = new MySqlCommand("sp_get_resignation_accepted_mail_details", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@p_resignation_id", resignationId);
                    con.Open();

                    using (var dr = cmd.ExecuteReader())
                    {
                        if (dr.Read())
                        {
                            mail = new ResignationMailDO
                            {
                                ToEmail = GetStringSafe(dr, "ToEmail"),
                                CcEmail = GetStringSafe(dr, "CcEmail"),
                                BccEmail = GetStringSafe(dr, "BccEmail"),
                                Subject = GetStringSafe(dr, "Subject"),
                                Body = GetStringSafe(dr, "Body"),
                                LetterHtml = GetStringSafe(dr, "LetterHtml")
                            };
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                CommonBL errorlog = new CommonBL();
                errorlog.fnStoreErrorLog(
                    "HandoverprocessBL",
                    "GetResignationAcceptedMailDetails",
                    "Exception Message=" + ex.Message + " Strace=" + ex.StackTrace,
                    UserId
                );
            }

            return mail;
        }

        // Fetches the mail details for the given resignation from the SP and sends the
        // email, optionally with the acceptance letter attached as a PDF. Never throws -
        // any failure here must not undo the HR review that was just saved, so callers
        // can fire-and-forget this right after a successful accept.
        public void SendResignationAcceptedEmail(int resignationId, byte[] attachmentBytes = null, string attachmentFileName = null)
        {
            SendResignationAcceptedEmail(GetResignationAcceptedMailDetails(resignationId), attachmentBytes, attachmentFileName);
        }

        // Same as above, but takes an already-fetched ResignationMailDO so callers that
        // also need mail.LetterHtml (e.g. to render the PDF attachment) don't have to hit
        // sp_get_resignation_accepted_mail_details a second time.
        public void SendResignationAcceptedEmail(ResignationMailDO mail, byte[] attachmentBytes = null, string attachmentFileName = null)
        {
            try
            {
                if (mail == null || string.IsNullOrWhiteSpace(mail.ToEmail))
                {
                    return;
                }

                CommonBL commonBL = new CommonBL();
                commonBL.SendEmail(mail.ToEmail, mail.CcEmail, mail.BccEmail, mail.Subject, mail.Body, attachmentBytes, attachmentFileName);
            }
            catch (Exception ex)
            {
                CommonBL errorlog = new CommonBL();
                errorlog.fnStoreErrorLog(
                    "HandoverprocessBL",
                    "SendResignationAcceptedEmail",
                    "Exception Message=" + ex.Message + " Strace=" + ex.StackTrace,
                    UserId
                );
            }
        }

        // Recipients/Subject/Body for the manager-level accept/reject email
        // (ResignationList.aspx) come from sp_get_resignation_action_mail_details, not
        // built in C#. Unlike the HR-final-acceptance flow, there's no LetterHtml here -
        // Rejected never gets a PDF, and Accepted's PDF is still built in C# from the
        // resignation record the caller already has (GenerateResignationPdf).
        public ResignationMailDO GetResignationActionMailDetails(int resignationId, string action)
        {
            ResignationMailDO mail = null;

            if (resignationId <= 0 || string.IsNullOrWhiteSpace(MySqlconnection))
            {
                return mail;
            }

            try
            {
                string normalized = NormalizeMySqlConnectionString(MySqlconnection);
                using (MySqlConnection con = new MySqlConnection(normalized))
                using (MySqlCommand cmd = new MySqlCommand("sp_get_resignation_action_mail_details", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@p_resignation_id", resignationId);
                    cmd.Parameters.AddWithValue("@p_action", action ?? string.Empty);
                    con.Open();

                    using (var dr = cmd.ExecuteReader())
                    {
                        if (dr.Read())
                        {
                            mail = new ResignationMailDO
                            {
                                ToEmail = GetStringSafe(dr, "ToEmail"),
                                CcEmail = GetStringSafe(dr, "CcEmail"),
                                BccEmail = GetStringSafe(dr, "BccEmail"),
                                Subject = GetStringSafe(dr, "Subject"),
                                Body = GetStringSafe(dr, "Body")
                            };
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                CommonBL errorlog = new CommonBL();
                errorlog.fnStoreErrorLog(
                    "HandoverprocessBL",
                    "GetResignationActionMailDetails",
                    "Exception Message=" + ex.Message + " Strace=" + ex.StackTrace,
                    UserId
                );
            }

            return mail;
        }

        public KTHandoverDO GetKTHandoverByResignationId(int resignationId)
        {
            KTHandoverDO model = null;

            if (resignationId <= 0 || string.IsNullOrWhiteSpace(MySqlconnection))
            {
                return model;
            }

            try
            {
                string normalized = NormalizeMySqlConnectionString(MySqlconnection);
                using (MySqlConnection con = new MySqlConnection(normalized))
                using (MySqlCommand cmd = new MySqlCommand("sp_GetKTHandoverByResignationId", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@p_ResignationId", resignationId);
                    con.Open();

                    using (var dr = cmd.ExecuteReader())
                    {
                        if (dr.Read())
                        {
                            model = new KTHandoverDO
                            {
                                KTId = GetIntSafe(dr, "KTId"),
                                ResignationId = GetIntSafe(dr, "ResignationId"),
                                KTPlan = GetStringSafe(dr, "KTPlan"),
                                ReplacementEmployee = GetStringSafe(dr, "ReplacementEmployee"),
                                KTStatus = GetStringSafe(dr, "KTStatus"),
                                KTStartDate = GetNullableDateSafe(dr, "KTStartDate"),
                                KTCompletionDate = GetNullableDateSafe(dr, "KTCompletionDate")
                            };
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                CommonBL errorlog = new CommonBL();
                errorlog.fnStoreErrorLog(
                    "HandoverprocessBL",
                    "GetKTHandoverByResignationId",
                    "Exception Message=" + ex.Message + " Strace=" + ex.StackTrace,
                    UserId
                );
            }

            return model;
        }

        public KTHandoverResponseDO SaveKTHandover(KTHandoverDO model, int createdBy)
        {
            var response = new KTHandoverResponseDO { Success = false, ResponseMsg = "Unable to save KT & Handover details." };

            if (model == null || model.ResignationId <= 0 || string.IsNullOrWhiteSpace(MySqlconnection))
            {
                response.ResponseMsg = "Invalid KT & Handover request.";
                return response;
            }

            try
            {
                string normalized = NormalizeMySqlConnectionString(MySqlconnection);
                using (MySqlConnection con = new MySqlConnection(normalized))
                using (MySqlCommand cmd = new MySqlCommand("sp_SaveKTHandover", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@p_ResignationId", model.ResignationId);
                    cmd.Parameters.AddWithValue("@p_KTPlan", model.KTPlan ?? string.Empty);
                    cmd.Parameters.AddWithValue("@p_ReplacementEmployee", (object)model.ReplacementEmployee ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@p_KTStatus", model.KTStatus ?? "Pending");
                    cmd.Parameters.AddWithValue("@p_KTStartDate", model.KTStartDate.HasValue ? (object)model.KTStartDate.Value : DBNull.Value);
                    cmd.Parameters.AddWithValue("@p_KTCompletionDate", model.KTCompletionDate.HasValue ? (object)model.KTCompletionDate.Value : DBNull.Value);
                    cmd.Parameters.AddWithValue("@p_CreatedBy", createdBy);

                    var outKTId = new MySqlParameter("@p_KTId", MySqlDbType.Int32) { Direction = ParameterDirection.Output };
                    var outSuccess = new MySqlParameter("@p_Success", MySqlDbType.Byte) { Direction = ParameterDirection.Output };
                    var outMsg = new MySqlParameter("@p_ResponseMsg", MySqlDbType.VarChar, 200) { Direction = ParameterDirection.Output };

                    cmd.Parameters.Add(outKTId);
                    cmd.Parameters.Add(outSuccess);
                    cmd.Parameters.Add(outMsg);

                    con.Open();
                    cmd.ExecuteNonQuery();

                    response.KTId = outKTId.Value != DBNull.Value ? Convert.ToInt32(outKTId.Value) : 0;
                    response.Success = outSuccess.Value != DBNull.Value && Convert.ToInt32(outSuccess.Value) == 1;
                    response.ResponseMsg = outMsg.Value != DBNull.Value
                        ? Convert.ToString(outMsg.Value)
                        : (response.Success ? "KT & Handover details saved successfully." : "Failed to save KT & Handover details.");
                }
            }
            catch (Exception ex)
            {
                CommonBL errorlog = new CommonBL();
                errorlog.fnStoreErrorLog(
                    "HandoverprocessBL",
                    "SaveKTHandover",
                    "Exception Message=" + ex.Message + " Strace=" + ex.StackTrace,
                    UserId
                );
                response.Success = false;
                response.ResponseMsg = "Error occurred while saving KT & Handover details.";
            }

            return response;
        }

        public KTHandoverResponseDO UpdateKTHandover(KTHandoverDO model, int updatedBy)
        {
            var response = new KTHandoverResponseDO { Success = false, ResponseMsg = "Unable to update KT & Handover details." };

            if (model == null || model.KTId <= 0 || string.IsNullOrWhiteSpace(MySqlconnection))
            {
                response.ResponseMsg = "Invalid KT & Handover request.";
                return response;
            }

            try
            {
                string normalized = NormalizeMySqlConnectionString(MySqlconnection);
                using (MySqlConnection con = new MySqlConnection(normalized))
                using (MySqlCommand cmd = new MySqlCommand("sp_UpdateKTHandover", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@p_KTId", model.KTId);
                    cmd.Parameters.AddWithValue("@p_KTPlan", model.KTPlan ?? string.Empty);
                    cmd.Parameters.AddWithValue("@p_ReplacementEmployee", (object)model.ReplacementEmployee ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@p_KTStatus", model.KTStatus ?? "Pending");
                    cmd.Parameters.AddWithValue("@p_KTStartDate", model.KTStartDate.HasValue ? (object)model.KTStartDate.Value : DBNull.Value);
                    cmd.Parameters.AddWithValue("@p_KTCompletionDate", model.KTCompletionDate.HasValue ? (object)model.KTCompletionDate.Value : DBNull.Value);
                    cmd.Parameters.AddWithValue("@p_UpdatedBy", updatedBy);

                    var outSuccess = new MySqlParameter("@p_Success", MySqlDbType.Byte) { Direction = ParameterDirection.Output };
                    var outMsg = new MySqlParameter("@p_ResponseMsg", MySqlDbType.VarChar, 200) { Direction = ParameterDirection.Output };

                    cmd.Parameters.Add(outSuccess);
                    cmd.Parameters.Add(outMsg);

                    con.Open();
                    cmd.ExecuteNonQuery();

                    response.KTId = model.KTId;
                    response.Success = outSuccess.Value != DBNull.Value && Convert.ToInt32(outSuccess.Value) == 1;
                    response.ResponseMsg = outMsg.Value != DBNull.Value
                        ? Convert.ToString(outMsg.Value)
                        : (response.Success ? "KT & Handover details updated successfully." : "Failed to update KT & Handover details.");
                }
            }
            catch (Exception ex)
            {
                CommonBL errorlog = new CommonBL();
                errorlog.fnStoreErrorLog(
                    "HandoverprocessBL",
                    "UpdateKTHandover",
                    "Exception Message=" + ex.Message + " Strace=" + ex.StackTrace,
                    UserId
                );
                response.Success = false;
                response.ResponseMsg = "Error occurred while updating KT & Handover details.";
            }

            return response;
        }

        public List<KTProjectHandoverRowDO> GetKTProjectHandoverRows(int ktId)
        {
            var rows = new List<KTProjectHandoverRowDO>();

            if (ktId <= 0 || string.IsNullOrWhiteSpace(MySqlconnection))
            {
                return rows;
            }

            try
            {
                string normalized = NormalizeMySqlConnectionString(MySqlconnection);
                using (MySqlConnection con = new MySqlConnection(normalized))
                using (MySqlCommand cmd = new MySqlCommand("sp_GetKTProjectHandoverList", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@p_KTId", ktId);
                    con.Open();

                    using (var dr = cmd.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            rows.Add(new KTProjectHandoverRowDO
                            {
                                KTProjectHandoverId = GetIntSafe(dr, "KTProjectHandoverId"),
                                KTId = GetIntSafe(dr, "KTId"),
                                ProjectName = GetStringSafe(dr, "ProjectName"),
                                AssignedEmployee = GetStringSafe(dr, "AssignedEmployee"),
                                Status = GetStringSafe(dr, "Status")
                            });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                CommonBL errorlog = new CommonBL();
                errorlog.fnStoreErrorLog(
                    "HandoverprocessBL",
                    "GetKTProjectHandoverRows",
                    "Exception Message=" + ex.Message + " Strace=" + ex.StackTrace,
                    UserId
                );
            }

            return rows;
        }

        public bool SaveKTProjectHandoverRows(int ktId, List<KTProjectHandoverRowDO> rows)
        {
            if (ktId <= 0 || string.IsNullOrWhiteSpace(MySqlconnection))
            {
                return false;
            }

            try
            {
                string normalized = NormalizeMySqlConnectionString(MySqlconnection);
                using (MySqlConnection con = new MySqlConnection(normalized))
                {
                    con.Open();

                    using (MySqlCommand delCmd = new MySqlCommand("sp_DeleteKTProjectHandoverRows", con))
                    {
                        delCmd.CommandType = CommandType.StoredProcedure;
                        delCmd.Parameters.AddWithValue("@p_KTId", ktId);
                        delCmd.ExecuteNonQuery();
                    }

                    if (rows != null)
                    {
                        foreach (var row in rows)
                        {
                            if (row == null || string.IsNullOrWhiteSpace(row.ProjectName))
                            {
                                continue;
                            }

                            using (MySqlCommand insCmd = new MySqlCommand("sp_InsertKTProjectHandoverRow", con))
                            {
                                insCmd.CommandType = CommandType.StoredProcedure;
                                insCmd.Parameters.AddWithValue("@p_KTId", ktId);
                                insCmd.Parameters.AddWithValue("@p_ProjectName", row.ProjectName);
                                insCmd.Parameters.AddWithValue("@p_AssignedEmployee", (object)row.AssignedEmployee ?? DBNull.Value);
                                insCmd.Parameters.AddWithValue("@p_Status", row.Status ?? "Pending");
                                insCmd.ExecuteNonQuery();
                            }
                        }
                    }
                }

                return true;
            }
            catch (Exception ex)
            {
                CommonBL errorlog = new CommonBL();
                errorlog.fnStoreErrorLog(
                    "HandoverprocessBL",
                    "SaveKTProjectHandoverRows",
                    "Exception Message=" + ex.Message + " Strace=" + ex.StackTrace,
                    UserId
                );
                return false;
            }
        }

        public KTHandoverResponseDO DeleteKTProjectHandoverRow(int ktProjectHandoverId)
        {
            var response = new KTHandoverResponseDO { Success = false, ResponseMsg = "Unable to delete knowledge transfer record." };

            if (ktProjectHandoverId <= 0 || string.IsNullOrWhiteSpace(MySqlconnection))
            {
                response.ResponseMsg = "Invalid knowledge transfer request.";
                return response;
            }

            try
            {
                string normalized = NormalizeMySqlConnectionString(MySqlconnection);
                using (MySqlConnection con = new MySqlConnection(normalized))
                using (MySqlCommand cmd = new MySqlCommand("sp_DeleteKnowledgeTransfer", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@p_KTProjectHandoverId", ktProjectHandoverId);

                    var outSuccess = new MySqlParameter("@p_Success", MySqlDbType.Byte) { Direction = ParameterDirection.Output };
                    var outMsg = new MySqlParameter("@p_ResponseMsg", MySqlDbType.VarChar, 200) { Direction = ParameterDirection.Output };

                    cmd.Parameters.Add(outSuccess);
                    cmd.Parameters.Add(outMsg);

                    con.Open();
                    cmd.ExecuteNonQuery();

                    response.Success = outSuccess.Value != DBNull.Value && Convert.ToInt32(outSuccess.Value) == 1;
                    response.ResponseMsg = outMsg.Value != DBNull.Value
                        ? Convert.ToString(outMsg.Value)
                        : (response.Success ? "Knowledge transfer deleted successfully." : "Failed to delete knowledge transfer record.");
                }
            }
            catch (Exception ex)
            {
                CommonBL errorlog = new CommonBL();
                errorlog.fnStoreErrorLog(
                    "HandoverprocessBL",
                    "DeleteKTProjectHandoverRow",
                    "Exception Message=" + ex.Message + " Strace=" + ex.StackTrace,
                    UserId
                );
                response.Success = false;
                response.ResponseMsg = "Error occurred while deleting knowledge transfer record.";
            }

            return response;
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





