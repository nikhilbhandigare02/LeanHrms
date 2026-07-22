using DataObject;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using static DataObject.AccountDashboardDO;

namespace ProcessModel
{
    public class AccountDashboardBL
    {
        protected string UserId = null;

        private string DBName = ConfigurationManager.AppSettings["DBName"];
        private static string MySqlconnection = ConfigurationManager.ConnectionStrings["MysqlConnection"].ConnectionString;
        private static string Sqlconnection = ConfigurationManager.ConnectionStrings["Sqlconnection"] != null
        ? ConfigurationManager.ConnectionStrings["Sqlconnection"].ConnectionString
        : string.Empty;
        public List<TotalDisbursedcountDO> GetTotalSalaryDisbursed(string UserId)
        {
            List<TotalDisbursedcountDO> listData = new List<TotalDisbursedcountDO>();

            try
            {
                getDrtolist getDrtolistParam = new getDrtolist();

                List<MySqlParameter> mysqlParameters = new List<MySqlParameter>();

                listData = getDrtolistParam.getdatafromreder<TotalDisbursedcountDO>(
                    DataClass.GetDataReaderFromSpWithParam(
                        mysqlParameters,
                        DBName,
                        "sp_GetTotalSalaryDisbursed"));
            }
            catch (Exception ex)
            {
                CommonBL errorlog = new CommonBL();

                errorlog.fnStoreErrorLog(
                    "AccountDashboardBL",
                    "GetTotalSalaryDisbursed",
                    "Exception Message : " + ex.Message +
                    " StackTrace : " + ex.StackTrace,
                    UserId);
            }

            return listData;
        }

        public List<TotalReimburesementdcountDO> GetTotalReimbursementAmount()
        {
            List<TotalReimburesementdcountDO> items = new List<TotalReimburesementdcountDO>();

            if (string.IsNullOrWhiteSpace(Sqlconnection))
            {
                return items;
            }

            try
            {
                using (MySqlConnection con = new MySqlConnection(Sqlconnection))
                using (MySqlCommand cmd = new MySqlCommand("sp_get_total_reimbursement_amount", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    con.Open();

                    using (MySqlDataReader dr = cmd.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            items.Add(new TotalReimburesementdcountDO
                            {
                                TotalReimbursement = Convert.ToDecimal(dr["TotalReimbursement"])
                            });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                CommonBL errorlog = new CommonBL();
                errorlog.fnStoreErrorLog(
                    "AccountDashboardBL",
                    "GetTotalReimbursementAmount",
                    "Exception Message: " + ex.Message + " StackTrace: " + ex.StackTrace,
                    UserId);
            }

            return items;
        }

        public List<TotalActiveEmployeecountDO> GetActiveEmployeeCount()
        {
            List<TotalActiveEmployeecountDO> items = new List<TotalActiveEmployeecountDO>();

            if (string.IsNullOrWhiteSpace(Sqlconnection))
            {
                return items;
            }

            try
            {
                using (MySqlConnection con = new MySqlConnection(Sqlconnection))
                using (MySqlCommand cmd = new MySqlCommand("sp_get_activeemployeecount_forAccountDashboard", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    con.Open();

                    using (MySqlDataReader dr = cmd.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            items.Add(new TotalActiveEmployeecountDO
                            {
                                ActiveEmployeeCount = Convert.ToInt32(dr["ActiveEmployeeCount"])
                            });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                CommonBL errorlog = new CommonBL();
                errorlog.fnStoreErrorLog(
                    "AccountDashboardBL",
                    "GetActiveEmployeeCount",
                    "Exception Message: " + ex.Message + " StackTrace: " + ex.StackTrace,
                    UserId);
            }

            return items;
        }

        public List<EmployeeSalaryDetailsDO> GetEmployeeSalaryDetails()
        {
            List<EmployeeSalaryDetailsDO> listData = new List<EmployeeSalaryDetailsDO>();

            try
            {
                getDrtolist getDrtolistParam = new getDrtolist();

                List<MySqlParameter> mysqlParameters = new List<MySqlParameter>();

                listData = getDrtolistParam.getdatafromreder<EmployeeSalaryDetailsDO>(
                    DataClass.GetDataReaderFromSpWithParam(
                        mysqlParameters,
                        DBName,
                        "sp_get_employee_salary_details"));
            }
            catch (Exception ex)
            {
                CommonBL errorlog = new CommonBL();

                errorlog.fnStoreErrorLog(
                    "AccountDashboardBL",
                    "GetEmployeeSalaryDetails",
                    "Exception Message : " + ex.Message +
                    " StackTrace : " + ex.StackTrace,
                    UserId);
            }

            return listData;
        }

        public List<EmployeeReimbursementDO> GetEmployeeReimbursementDetails()
        {
            List<EmployeeReimbursementDO> items = new List<EmployeeReimbursementDO>();

            if (string.IsNullOrWhiteSpace(Sqlconnection))
            {
                return items;
            }

            try
            {
                using (MySqlConnection con = new MySqlConnection(Sqlconnection))
                using (MySqlCommand cmd = new MySqlCommand("sp_get_reimbursement_dashboard_details", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    con.Open();

                    using (MySqlDataReader dr = cmd.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            items.Add(new EmployeeReimbursementDO
                            {
                                employee_name = dr["employee_name"].ToString(),
                                employee_code = dr["employee_code"].ToString(),
                                claim_type = dr["claim_type"].ToString(),
                                claim_amount = Convert.ToDecimal(dr["claim_amount"]),
                                claim_date = Convert.ToDateTime(dr["claim_date"]),
                                status = dr["status"].ToString()
                            });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                CommonBL errorlog = new CommonBL();
                errorlog.fnStoreErrorLog(
                    "AccountDashboardBL",
                    "GetEmployeeReimbursementDetails",
                    "Exception Message: " + ex.Message +
                    " StackTrace: " + ex.StackTrace,
                    UserId);
            }

            return items;
        }
    }
}
