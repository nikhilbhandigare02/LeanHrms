using DataObject;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;

namespace ProcessModel
{
    public class AppraisalBL
    {
        // Sp_Get_Appraisal_Details / sp_get_appraisal_details_by_id return dates
        // pre-formatted as "dd-MM-yyyy" strings (via MySQL's DATE_FORMAT). Parsing
        // them with Convert.ToDateTime() uses the server's current culture (often
        // MM-dd-yyyy), which throws FormatException for any day > 12 - e.g. "31-07-2026".
        private static DateTime ParseDdMmYyyy(object val)
        {
            string text = Convert.ToString(val);
            DateTime parsed;
            if (DateTime.TryParseExact(text, "dd-MM-yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out parsed))
            {
                return parsed;
            }
            // Fall back for any caller that isn't going through DATE_FORMAT.
            DateTime.TryParse(text, CultureInfo.InvariantCulture, DateTimeStyles.None, out parsed);
            return parsed;
        }

        public ResponseDO SaveAppraisalDetails(AppraisalDetailsDO appraisal)
        {
            ResponseDO response = new ResponseDO();

            try
            {
                int loggedInUserId = Convert.ToInt32(HttpContext.Current.Session["UserID"] ?? HttpContext.Current.Session["userId"] ?? 0);
                
                List<MySqlParameter> parameters = new List<MySqlParameter>
                {

                    DataClass.GetParameter("p_appraisal_effective_date", appraisal.appraisal_effective_date),
                    DataClass.GetParameter("p_salary_revision_date", appraisal.salary_revision_date),
                    DataClass.GetParameter("p_appraisal_ctc", appraisal.appraisal_ctc),
                    DataClass.GetParameter("p_gross_salary", appraisal.gross_salary),
                    DataClass.GetParameter("p_net_salary", appraisal.net_salary),
                    DataClass.GetParameter("p_increament_amount", appraisal.increament_amount),
                    DataClass.GetParameter("p_increament_percentage", appraisal.increament_percentage),
                    DataClass.GetParameter("p_increament_count", appraisal.increament_amount),
                    DataClass.GetParameter("p_user_id", appraisal.user_id), // Use the selected employee's ID from dropdown
                    DataClass.GetParameter("p_created_by", loggedInUserId) // Use the logged-in user's ID as creator

                };

                MySqlDataReader dr =
                    DataClass.GetDataReaderFromSpWithParam(
                        parameters,
                        "",
                        "Sp_save_appraisal_details");

                if (dr != null && dr.Read())
                {
                    response.Status =
                        Convert.ToInt32(dr["Status"]);

                    response.message =
                        dr["message"].ToString();
                }

                if (dr != null)
                    dr.Close();
            }
            catch (System.Exception ex)
            {
                response.Status = -1;
                response.message = ex.Message;
            }

            return response;
        }

        public List<AppraisalDetailsDO> GetAppraisalDetailsList()
        {
            List<AppraisalDetailsDO> list = new List<AppraisalDetailsDO>();

            try
            {
                using (MySqlDataReader dr =
                    DataClass.GetDataReaderFromSp(
                        "",
                        "Sp_Get_Appraisal_Details"))
                {
                    if (dr != null)
                    {
                        while (dr.Read())
                        {
                            AppraisalDetailsDO item = new AppraisalDetailsDO();

                            // Map all properties manually
                            for (int i = 0; i < dr.FieldCount; i++)
                            {
                                string colName = dr.GetName(i).ToLower();
                                object val = dr[i];

                                if (val == DBNull.Value)
                                    continue;

                                switch (colName)
                                {
                                    case "appraisal_id":
                                        item.appraisal_id = Convert.ToInt32(val);
                                        break;
                                    case "user_id":
                                        item.user_id = Convert.ToInt32(val);
                                        break;
                                    case "employee_name":
                                        item.employee_name = Convert.ToString(val);
                                        break;
                                    case "appraisal_effective_date":
                                        item.appraisal_effective_date = ParseDdMmYyyy(val);
                                        break;
                                    case "salary_revision_date":
                                        item.salary_revision_date = ParseDdMmYyyy(val);
                                        break;
                                    case "appraisal_ctc":
                                        item.appraisal_ctc = Convert.ToDecimal(val);
                                        break;
                                    case "gross_salary":
                                        item.gross_salary = Convert.ToDecimal(val);
                                        break;
                                    case "net_salary":
                                        item.net_salary = Convert.ToDecimal(val);
                                        break;
                                    case "increament_amount":
                                        item.increament_amount = Convert.ToDecimal(val);
                                        break;
                                    case "increament_percentage":
                                        item.increament_percentage = Convert.ToDecimal(val);
                                        break;
                                    case "oldctc":
                                        item.oldCTC = Convert.ToDecimal(val);
                                        break;
                                }
                            }

                            list.Add(item);
                        }
                    }
                }
            }
            catch (Exception)
            {
                // Log the error if needed, but return empty list
            }

            return list;
        }

        // For the AppraisalHistory.aspx page - returns every appraisal_details
        // row (active and superseded/is_active=0) so admins can see the full
        // increment trail per employee, filterable by employee and date range.
        public List<AppraisalDetailsDO> GetAppraisalHistory(int userId, DateTime? fromDate, DateTime? toDate)
        {
            List<AppraisalDetailsDO> list = new List<AppraisalDetailsDO>();

            try
            {
                List<MySqlParameter> parameters = new List<MySqlParameter>
                {
                    DataClass.GetParameter("p_user_id", userId),
                    DataClass.GetParameter("p_from_date", (object)fromDate ?? DBNull.Value),
                    DataClass.GetParameter("p_to_date", (object)toDate ?? DBNull.Value)
                };

                using (MySqlDataReader dr =
                    DataClass.GetDataReaderFromSpWithParam(
                        parameters,
                        "",
                        "sp_get_appraisal_history"))
                {
                    if (dr != null)
                    {
                        while (dr.Read())
                        {
                            AppraisalDetailsDO item = new AppraisalDetailsDO();

                            for (int i = 0; i < dr.FieldCount; i++)
                            {
                                string colName = dr.GetName(i).ToLower();
                                object val = dr[i];

                                if (val == DBNull.Value)
                                    continue;

                                switch (colName)
                                {
                                    case "appraisal_id":
                                        item.appraisal_id = Convert.ToInt32(val);
                                        break;
                                    case "user_id":
                                        item.user_id = Convert.ToInt32(val);
                                        break;
                                    case "emp_code":
                                        item.emp_code = Convert.ToString(val);
                                        break;
                                    case "employee_name":
                                        item.employee_name = Convert.ToString(val);
                                        break;
                                    case "appraisal_effective_date":
                                        item.appraisal_effective_date = ParseDdMmYyyy(val);
                                        break;
                                    case "salary_revision_date":
                                        item.salary_revision_date = ParseDdMmYyyy(val);
                                        break;
                                    case "appraisal_ctc":
                                        item.appraisal_ctc = Convert.ToDecimal(val);
                                        break;
                                    case "gross_salary":
                                        item.gross_salary = Convert.ToDecimal(val);
                                        break;
                                    case "net_salary":
                                        item.net_salary = Convert.ToDecimal(val);
                                        break;
                                    case "increament_amount":
                                        item.increament_amount = Convert.ToDecimal(val);
                                        break;
                                    case "increament_percentage":
                                        item.increament_percentage = Convert.ToDecimal(val);
                                        break;
                                    case "is_active":
                                        item.is_active = Convert.ToInt32(val) == 1;
                                        break;
                                    case "inserted_date":
                                        item.inserted_date_display = Convert.ToString(val);
                                        break;
                                }
                            }

                            list.Add(item);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                CommonBL errorlog = new CommonBL();
                errorlog.fnStoreErrorLog("AppraisalBL", "GetAppraisalHistory", "Exception Message: " + ex.Message + " StackTrace: " + ex.StackTrace, null);
            }

            return list;
        }

        public AppraisalDetailsDO GetAppraisalDetailsById(int appraisalId)
        {
            List<MySqlParameter> param = new List<MySqlParameter>();
            AppraisalDetailsDO result = null;

            param.Add(DataClass.GetParameter(
                "p_appraisal_id",
                appraisalId));

            try
            {
                using (MySqlDataReader dr =
                    DataClass.GetDataReaderFromSpWithParam(
                        param,
                        "",
                        "sp_get_appraisal_details_by_id"))
                {
                    if (dr != null && dr.HasRows && dr.Read())
                    {
                        result = new AppraisalDetailsDO();

                        // Map all properties manually
                        for (int i = 0; i < dr.FieldCount; i++)
                        {
                            string colName = dr.GetName(i).ToLower();
                            object val = dr[i];

                            if (val == DBNull.Value)
                                continue;

                            switch (colName)
                            {
                                case "appraisal_id":
                                    result.appraisal_id = Convert.ToInt32(val);
                                    break;
                                case "user_id":
                                    result.user_id = Convert.ToInt32(val);
                                    break;
                                case "employee_name":
                                    result.employee_name = Convert.ToString(val);
                                    break;
                                case "appraisal_effective_date":
                                    result.appraisal_effective_date = ParseDdMmYyyy(val);
                                    break;
                                case "salary_revision_date":
                                    result.salary_revision_date = ParseDdMmYyyy(val);
                                    break;
                                case "appraisal_ctc":
                                    result.appraisal_ctc = Convert.ToDecimal(val);
                                    break;
                                case "gross_salary":
                                    result.gross_salary = Convert.ToDecimal(val);
                                    break;
                                case "net_salary":
                                    result.net_salary = Convert.ToDecimal(val);
                                    break;
                                case "increament_amount":
                                    result.increament_amount = Convert.ToDecimal(val);
                                    break;
                                case "increament_percentage":
                                    result.increament_percentage = Convert.ToDecimal(val);
                                    break;
                                case "oldctc":
                                    result.oldCTC = Convert.ToDecimal(val);
                                    break;
                            }
                        }
                    }
                }
            }
            catch (Exception)
            {
                // Log if needed
            }

            return result;
        }

        public ResponseDO DeleteAppraisalDetails(int appraisalID)
        {
            ResponseDO response = new ResponseDO();
            try
            {
                int userId = Convert.ToInt32(HttpContext.Current.Session["UserID"] ?? HttpContext.Current.Session["userId"] ?? 0);
                
                List<MySqlParameter> parameters = new List<MySqlParameter>
                {
                    DataClass.GetParameter("p_appraisal_id", appraisalID),
                    DataClass.GetParameter("p_user_id", userId)
                };

                MySqlDataReader dr =
                    DataClass.GetDataReaderFromSpWithParam(
                        parameters,
                        "",
                        "sp_delete_appraisal_details");

                if (dr != null && dr.Read())
                {
                    response.Status =
                        Convert.ToInt32(dr["Status"]);

                    response.message =
                        dr["message"].ToString();
                }

                if (dr != null)
                    dr.Close();
            }
            catch (System.Exception ex)
            {
                response.Status = -1;
                response.message = ex.Message;
            }

            return response;
        }

        public ResponseDO UpdateAppraisalDetails(AppraisalDetailsDO appraisal)
        {
            ResponseDO response = new ResponseDO();

            try
            {
                int loggedInUserId = Convert.ToInt32(HttpContext.Current.Session["UserID"] ?? HttpContext.Current.Session["userId"] ?? 0);
                
                List<MySqlParameter> parameters = new List<MySqlParameter>
                {
                    DataClass.GetParameter("p_appraisal_id", appraisal.appraisal_id),
                    //DataClass.GetParameter("p_employee_name", appraisal.employee_name),
                    DataClass.GetParameter("p_appraisal_effective_date", appraisal.appraisal_effective_date),
                    DataClass.GetParameter("p_salary_revision_date", appraisal.salary_revision_date),
                    DataClass.GetParameter("p_appraisal_ctc", appraisal.appraisal_ctc),
                    DataClass.GetParameter("p_gross_salary", appraisal.gross_salary),
                    DataClass.GetParameter("p_net_salary", appraisal.net_salary),
                    DataClass.GetParameter("p_increament_amount", appraisal.increament_amount),
                    DataClass.GetParameter("p_increament_percentage", appraisal.increament_percentage),
                    DataClass.GetParameter("p_user_id", appraisal.user_id), // Use the stored employee ID
                    DataClass.GetParameter("p_updated_by", loggedInUserId) // Use the stored employee ID
                };

                MySqlDataReader dr =
                    DataClass.GetDataReaderFromSpWithParam(
                        parameters,
                        "",
                        "sp_update_appraisal_details");

                if (dr != null && dr.Read())
                {
                    response.Status =
                        Convert.ToInt32(dr["Status"]);

                    response.message =
                        dr["message"].ToString();
                }

                if (dr != null)
                    dr.Close();
            }
            catch (System.Exception ex)
            {
                response.Status = -1;
                response.message = ex.Message;
            }

            return response;
        }

        public decimal GetOldCTCByUserId(int userId)
        {
            decimal oldCTC = 0;

            try
            {
                List<MySqlParameter> parameters = new List<MySqlParameter>
                {
                    DataClass.GetParameter("p_user_id", userId)
                };

                using (MySqlDataReader dr =
                    DataClass.GetDataReaderFromSpWithParam(
                        parameters,
                        "",
                        "SP_GetOldCTCByUserId"))
                {
                    if (dr != null && dr.HasRows && dr.Read())
                    {
                        if (!dr.IsDBNull(0))
                        {
                            oldCTC = Convert.ToDecimal(dr["oldCTC"]);
                        }
                    }
                }
            }
            catch (Exception)
            {
                // Return 0 on error
            }

            return oldCTC;
        }
    }
}