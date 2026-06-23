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
                int userId = Convert.ToInt32(HttpContext.Current.Session["UserID"] ?? HttpContext.Current.Session["userId"] ?? 0);
                
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
                    DataClass.GetParameter("p_user_id", userId),
                    DataClass.GetParameter("p_created_by", userId)

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
            MySqlDataReader dr =
                DataClass.GetDataReaderFromSp(
                    "",
                    "Sp_Get_Appraisal_Details");

            getDrtolist mapper = new getDrtolist();

            return mapper.getdatafromreder<AppraisalDetailsDO>(dr);
        }

        public AppraisalDetailsDO GetAppraisalDetailsById(int appraisalId)
        {
            List<MySqlParameter> param = new List<MySqlParameter>();

            param.Add(DataClass.GetParameter(
                "p_appraisal_id",
                appraisalId));

            MySqlDataReader dr =
                DataClass.GetDataReaderFromSpWithParam(
                    param,
                    "",
                    "sp_get_appraisal_details_by_id");

            getDrtolist mapper = new getDrtolist();

            return mapper
                .getdatafromreder<AppraisalDetailsDO>(dr)
                .FirstOrDefault();
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
                int userId = Convert.ToInt32(HttpContext.Current.Session["UserID"] ?? HttpContext.Current.Session["userId"] ?? 0);
                
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
                    DataClass.GetParameter("p_user_id", userId)
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