using DataObject;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ProcessModel
{
    public class AppraisalBL
    {
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

            using (MySqlDataReader dr =
                DataClass.GetDataReaderFromSp(
                    "",
                    "Sp_Get_Appraisal_Details"))
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
                                item.appraisal_effective_date = Convert.ToDateTime(val);
                                break;
                            case "salary_revision_date":
                                item.salary_revision_date = Convert.ToDateTime(val);
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

            return list;
        }

        public AppraisalDetailsDO GetAppraisalDetailsById(int appraisalId)
        {
            List<MySqlParameter> param = new List<MySqlParameter>();
            AppraisalDetailsDO result = null;

            param.Add(DataClass.GetParameter(
                "p_appraisal_id",
                appraisalId));

            using (MySqlDataReader dr =
                DataClass.GetDataReaderFromSpWithParam(
                    param,
                    "",
                    "sp_get_appraisal_details_by_id"))
            {
                if (dr.HasRows && dr.Read())
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
                                result.appraisal_effective_date = Convert.ToDateTime(val);
                                break;
                            case "salary_revision_date":
                                result.salary_revision_date = Convert.ToDateTime(val);
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
    }
}