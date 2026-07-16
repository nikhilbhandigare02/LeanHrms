
using System;
using System.Collections.Generic;
using System.Configuration;
using DataObject;
using MySql.Data.MySqlClient;

namespace ProcessModel
{
    public class SalaryCalculationBL
    {
        private string DBName = ConfigurationManager.AppSettings["DBName"];
        private string UserId = null;

        public List<SalaryCalculationDO> GetSalaryCalculations()
        {
            List<SalaryCalculationDO> listdata = new List<SalaryCalculationDO>();
            getDrtolist getDrtolistParam = new getDrtolist();

            try
            {
                List<MySqlParameter> mysqlParameters = new List<MySqlParameter>();
                //mysqlParameters.Add(DataClass.GetParameter("p_user_id", 0));
                listdata = getDrtolistParam.getdatafromreder<SalaryCalculationDO>(
                    DataClass.GetDataReaderFromSpWithParam(mysqlParameters, DBName, "sp_get_salary_calculation")
                );
            }
            catch (Exception ex)
            {
                CommonBL errorlog = new CommonBL();
                errorlog.fnStoreErrorLog("SalaryCalculationBL", "GetSalaryCalculations", ex.Message + " | StackTrace=" + ex.StackTrace, UserId);
            }

            return listdata;
        }

        public SalaryCalculationDO GetSalaryCalculationByUserId(int userId)
        {
            SalaryCalculationDO data = null;
            getDrtolist getDrtolistParam = new getDrtolist();
            List<MySqlParameter> mysqlParameters = new List<MySqlParameter>();

            try
            {
                mysqlParameters.Add(DataClass.GetParameter("p_user_id", userId));
                List<SalaryCalculationDO> list = getDrtolistParam.getdatafromreder<SalaryCalculationDO>(
                    DataClass.GetDataReaderFromSpWithParam(mysqlParameters, DBName, "sp_get_salary_calculationById")
                );
                if (list.Count > 0)
                {
                    data = list[0];
                }
            }
            catch (Exception ex)
            {
                CommonBL errorlog = new CommonBL();
                errorlog.fnStoreErrorLog("SalaryCalculationBL", "GetSalaryCalculationByUserId", ex.Message + " | StackTrace=" + ex.StackTrace, UserId);
            }

            return data;
        }

        public (int Status, string Message) SaveSalarySlip(string empCode, string username, int daysPresent, int daysAbsent, decimal basicSalary, decimal totalDeduction, int totalDeductionDays, decimal netSalary, int userId, int insertedBy)
        {
            int status = 0;
            string message = "";
            getDrtolist getDrtolistParam = new getDrtolist();
            List<MySqlParameter> mysqlParameters = new List<MySqlParameter>();

            try
            {
                mysqlParameters.Add(DataClass.GetParameter("p_emp_code", empCode));
                mysqlParameters.Add(DataClass.GetParameter("p_username", username));
                mysqlParameters.Add(DataClass.GetParameter("p_days_present", daysPresent));
                mysqlParameters.Add(DataClass.GetParameter("p_days_absent", daysAbsent));
                mysqlParameters.Add(DataClass.GetParameter("p_basic_salary", basicSalary));
                mysqlParameters.Add(DataClass.GetParameter("p_total_deduction", totalDeduction));
                mysqlParameters.Add(DataClass.GetParameter("p_total_deduction_days", totalDeductionDays));
                mysqlParameters.Add(DataClass.GetParameter("p_net_salary", netSalary));
                mysqlParameters.Add(DataClass.GetParameter("p_user_id", userId));
                mysqlParameters.Add(DataClass.GetParameter("p_inserted_by", insertedBy));

                // Execute the stored procedure and read the result
                using (var reader = DataClass.GetDataReaderFromSpWithParam(mysqlParameters, DBName, "sp_save_salary_slip"))
                {
                    if (reader.Read())
                    {
                        status = Convert.ToInt32(reader["status"]);
                        message = Convert.ToString(reader["message"]);
                    }
                }
            }
            catch (Exception ex)
            {
                CommonBL errorlog = new CommonBL();
                errorlog.fnStoreErrorLog("SalaryCalculationBL", "SaveSalarySlip", ex.Message + " | StackTrace=" + ex.StackTrace, UserId);
                status = 0;
                message = "Error saving salary slip: " + ex.Message;
            }

            return (status, message);
        }
    }
}
