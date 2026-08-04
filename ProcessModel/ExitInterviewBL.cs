using DataObject;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;

namespace ProcessModel
{
    public class ExitInterviewBL
    {
        protected string UserId = null;
        private string DBName = ConfigurationManager.AppSettings["DBName"];
        private static string MySqlconnection = ConfigurationManager.ConnectionStrings["MysqlConnection"].ConnectionString;

        public List<DropDownData> GetInterviewers()
        {
            List<DropDownData> interviewers = new List<DropDownData>();
            try
            {
                using (MySqlConnection con = new MySqlConnection(MySqlconnection))
                {
                    con.Open();
                    using (MySqlCommand cmd = new MySqlCommand("sp_getUsersByRole", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        using (MySqlDataReader dr = cmd.ExecuteReader())
                        {
                            while (dr.Read())
                            {
                                string userType = dr["UserType"].ToString();
                                string displayName = $"{dr["user_fullname"]} ({userType})";
                                interviewers.Add(new DropDownData
                                {
                                    Id = Convert.ToInt32(dr["user_id"]),
                                    Text = displayName
                                });
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                CommonBL errorlog = new CommonBL();
                errorlog.fnStoreErrorLog("ExitInterviewBL", "GetInterviewers", ex.Message, UserId);
            }
            return interviewers;
        }

        public List<ExitInterviewDO> GetExitInterviewList()
        {
            List<ExitInterviewDO> exitInterviews = new List<ExitInterviewDO>();
            getDrtolist getDrtolistParam = new getDrtolist();

            try
            {
                exitInterviews = getDrtolistParam.getdatafromreder<ExitInterviewDO>(
                    DataClass.GetDataReaderFromSpWithParam(
                        null,
                        DBName,
                        "sp_GetExitInterviewList"
                    ));
            }
            catch (Exception ex)
            {
                CommonBL errorlog = new CommonBL();
                errorlog.fnStoreErrorLog("ExitInterviewBL", "GetExitInterviewList", ex.Message, UserId);
            }

            return exitInterviews;
        }

        public ExitInterviewDO GetExitInterviewById(int exitInterviewId)
        {
            List<ExitInterviewDO> exitInterviews = new List<ExitInterviewDO>();
            getDrtolist getDrtolistParam = new getDrtolist();
            List<MySqlParameter> sqlParameters = new List<MySqlParameter>();

            try
            {
                sqlParameters.Add(DataClass.GetParameter("@p_interview_id", exitInterviewId));
                exitInterviews = getDrtolistParam.getdatafromreder<ExitInterviewDO>(
                    DataClass.GetDataReaderFromSpWithParam(
                        sqlParameters,
                        DBName,
                        "sp_GetExitInterviewById"
                    ));
            }
            catch (Exception ex)
            {
                CommonBL errorlog = new CommonBL();
                errorlog.fnStoreErrorLog("ExitInterviewBL", "GetExitInterviewById", ex.Message, UserId);
            }

            return exitInterviews.FirstOrDefault();
        }

        private static object ParseInterviewTime(string value)
        {
            TimeSpan ts;
            return TimeSpan.TryParse(value, out ts) ? (object)ts : (object)TimeSpan.Zero;
        }

        public string SaveExitInterview(ExitInterviewDO obj)
        {
            string result = "Failed";

            try
            {
                using (MySqlConnection con = new MySqlConnection(MySqlconnection))
                using (MySqlCommand cmd = new MySqlCommand("sp_InsertExitInterview", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@p_user_id", obj.UserId);
                    cmd.Parameters.AddWithValue("@p_interviewer_id", obj.InterviewerId);
                    cmd.Parameters.AddWithValue("@p_interview_date", obj.InterviewDate);
                    cmd.Parameters.AddWithValue("@p_interview_time", ParseInterviewTime(obj.InterviewTime));
                    cmd.Parameters.AddWithValue("@p_interview_status", obj.Interview_Status_id);
                    cmd.Parameters.AddWithValue("@p_interview_mode", obj.Interview_Mode_id);
                    cmd.Parameters.AddWithValue("@p_location", string.IsNullOrWhiteSpace(obj.Location) ? (object)DBNull.Value : obj.Location);
                    cmd.Parameters.AddWithValue("@p_notes", string.IsNullOrWhiteSpace(obj.Notes) ? (object)DBNull.Value : obj.Notes);
                    cmd.Parameters.AddWithValue("@p_inserted_by", obj.InsertedBy);

                    var outId = new MySqlParameter("@p_interview_id", MySqlDbType.Int32) { Direction = ParameterDirection.Output };
                    var outSuccess = new MySqlParameter("@p_success", MySqlDbType.Byte) { Direction = ParameterDirection.Output };
                    var outMsg = new MySqlParameter("@p_response_msg", MySqlDbType.VarChar, 200) { Direction = ParameterDirection.Output };
                    cmd.Parameters.Add(outId);
                    cmd.Parameters.Add(outSuccess);
                    cmd.Parameters.Add(outMsg);

                    con.Open();
                    cmd.ExecuteNonQuery();

                    bool success = outSuccess.Value != DBNull.Value && Convert.ToInt32(outSuccess.Value) == 1;
                    string msg = outMsg.Value != DBNull.Value
                        ? Convert.ToString(outMsg.Value)
                        : (success ? "Exit interview scheduled successfully." : "Failed to schedule exit interview.");
                    result = (success ? "Success: " : "Error: ") + msg;

                    // Send email if successful
                    if (success && outId.Value != DBNull.Value && Convert.ToInt32(outId.Value) > 0)
                    {
                        try
                        {
                            int interviewId = Convert.ToInt32(outId.Value);
                            SendExitInterviewEmail(interviewId, "Exit Interview Scheduled");
                        }
                        catch (Exception emailEx)
                        {
                            // Log email error but don't fail the main operation
                            CommonBL errorlog = new CommonBL();
                            errorlog.fnStoreErrorLog("ExitInterviewBL", "SaveExitInterview-Email", "Exception Message: " + emailEx.Message + " StackTrace: " + emailEx.StackTrace, UserId);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                CommonBL errorlog = new CommonBL();
                errorlog.fnStoreErrorLog("ExitInterviewBL", "SaveExitInterview", ex.Message, UserId);
                result = "Error: " + ex.Message;
            }

            return result;
        }

        public string UpdateExitInterview(ExitInterviewDO obj)
        {
            string result = "Failed";

            try
            {
                using (MySqlConnection con = new MySqlConnection(MySqlconnection))
                using (MySqlCommand cmd = new MySqlCommand("sp_UpdateExitInterview", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@p_interview_id", obj.ExitInterviewId);
                    cmd.Parameters.AddWithValue("@p_user_id", obj.UserId);
                    cmd.Parameters.AddWithValue("@p_interviewer_id", obj.InterviewerId);
                    cmd.Parameters.AddWithValue("@p_interview_date", obj.InterviewDate);
                    cmd.Parameters.AddWithValue("@p_interview_time", ParseInterviewTime(obj.InterviewTime));
                    cmd.Parameters.AddWithValue("@p_interview_status", obj.Interview_Status_id);
                    cmd.Parameters.AddWithValue("@p_interview_mode", obj.Interview_Mode_id);
                    cmd.Parameters.AddWithValue("@p_location", string.IsNullOrWhiteSpace(obj.Location) ? (object)DBNull.Value : obj.Location);
                    cmd.Parameters.AddWithValue("@p_notes", string.IsNullOrWhiteSpace(obj.Notes) ? (object)DBNull.Value : obj.Notes);
                    cmd.Parameters.AddWithValue("@p_updated_by", obj.UpdatedBy.HasValue ? (object)obj.UpdatedBy.Value : DBNull.Value);

                    var outSuccess = new MySqlParameter("@p_success", MySqlDbType.Byte) { Direction = ParameterDirection.Output };
                    var outMsg = new MySqlParameter("@p_response_msg", MySqlDbType.VarChar, 200) { Direction = ParameterDirection.Output };
                    cmd.Parameters.Add(outSuccess);
                    cmd.Parameters.Add(outMsg);

                    con.Open();
                    cmd.ExecuteNonQuery();

                    bool success = outSuccess.Value != DBNull.Value && Convert.ToInt32(outSuccess.Value) == 1;
                    string msg = outMsg.Value != DBNull.Value
                        ? Convert.ToString(outMsg.Value)
                        : (success ? "Exit interview updated successfully." : "Failed to update exit interview.");
                    result = (success ? "Success: " : "Error: ") + msg;

                    // Send email if successful
                    if (success && obj.ExitInterviewId > 0)

                    {
                        try
                        {
                            SendExitInterviewEmail(obj.ExitInterviewId, "Exit Interview Updated");
                        }
                        catch (Exception emailEx)
                        {
                            // Log email error but don't fail the main operation
                            CommonBL errorlog = new CommonBL();
                            errorlog.fnStoreErrorLog("ExitInterviewBL", "UpdateExitInterview-Email", "Exception Message: " + emailEx.Message + " StackTrace: " + emailEx.StackTrace, UserId);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                CommonBL errorlog = new CommonBL();
                errorlog.fnStoreErrorLog("ExitInterviewBL", "UpdateExitInterview", ex.Message, UserId);
                result = "Error: " + ex.Message;
            }

            return result;
        }

        private void SendExitInterviewEmail(int interviewId, string subject)
        {
            try
            {
                string emailId = "";
                string emailBody = "";

                using (MySqlConnection con = new MySqlConnection(MySqlconnection))
                using (MySqlCommand cmd = new MySqlCommand("sp_GetExitInterviewEmail", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@p_interview_id", interviewId);

                    con.Open();
                    using (MySqlDataReader dr = cmd.ExecuteReader())
                    {
                        if (dr.Read())
                        {
                            emailId = dr["EmailId"] != DBNull.Value ? Convert.ToString(dr["EmailId"]) : "";
                            emailBody = dr["EmailBody"] != DBNull.Value ? Convert.ToString(dr["EmailBody"]) : "";
                        }
                    }
                }

                // Send email if email ID is available
                if (!string.IsNullOrWhiteSpace(emailId) && !string.IsNullOrWhiteSpace(emailBody))
                {
                    CommonBL commonBL = new CommonBL();
                    commonBL.SendEmail(emailId, "", subject, emailBody);
                }
            }
            catch (Exception ex)
            {
                // Log email error
                CommonBL errorlog = new CommonBL();
                errorlog.fnStoreErrorLog("ExitInterviewBL", "SendExitInterviewEmail", "Exception Message: " + ex.Message + " StackTrace: " + ex.StackTrace, UserId);
                throw; // Re-throw to let the caller handle it
            }
        }

        public string DeleteExitInterview(int exitInterviewId)
        {
            string result = "Failed";

            try
            {
                using (MySqlConnection con = new MySqlConnection(MySqlconnection))
                using (MySqlCommand cmd = new MySqlCommand("sp_DeleteExitInterview", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@p_interview_id", exitInterviewId);

                    var outSuccess = new MySqlParameter("@p_success", MySqlDbType.Byte) { Direction = ParameterDirection.Output };
                    var outMsg = new MySqlParameter("@p_response_msg", MySqlDbType.VarChar, 200) { Direction = ParameterDirection.Output };
                    cmd.Parameters.Add(outSuccess);
                    cmd.Parameters.Add(outMsg);

                    con.Open();
                    cmd.ExecuteNonQuery();

                    bool success = outSuccess.Value != DBNull.Value && Convert.ToInt32(outSuccess.Value) == 1;
                    string msg = outMsg.Value != DBNull.Value
                        ? Convert.ToString(outMsg.Value)
                        : (success ? "Exit interview deleted successfully." : "Failed to delete exit interview.");
                    result = (success ? "Success: " : "Error: ") + msg;
                }
            }
            catch (Exception ex)
            {
                CommonBL errorlog = new CommonBL();
                errorlog.fnStoreErrorLog("ExitInterviewBL", "DeleteExitInterview", ex.Message, UserId);
                result = "Error: " + ex.Message;
            }

            return result;
        }
    }
}
