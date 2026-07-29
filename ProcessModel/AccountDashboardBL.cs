using DataObject;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Net;
using System.Net.Mail;
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

        //public List<EmployeeSalaryDetailsDO> GetEmployeeSalaryDetails()
        //{
        //    List<EmployeeSalaryDetailsDO> listData = new List<EmployeeSalaryDetailsDO>();

        //    try
        //    {
        //        getDrtolist getDrtolistParam = new getDrtolist();

        //        List<MySqlParameter> mysqlParameters = new List<MySqlParameter>();

        //        listData = getDrtolistParam.getdatafromreder<EmployeeSalaryDetailsDO>(
        //            DataClass.GetDataReaderFromSpWithParam(
        //                mysqlParameters,
        //                DBName,
        //                "sp_get_employee_salary_details"));
        //    }
        //    catch (Exception ex)
        //    {
        //        CommonBL errorlog = new CommonBL();

        //        errorlog.fnStoreErrorLog(
        //            "AccountDashboardBL",
        //            "GetEmployeeSalaryDetails",
        //            "Exception Message : " + ex.Message +
        //            " StackTrace : " + ex.StackTrace,
        //            UserId);
        //    }

        //    return listData;
        //}
        public List<EmployeeSalaryDetailsDO> GetEmployeeSalaryDetails(string empCode, string empName, string status)
        {
            List<EmployeeSalaryDetailsDO> listData = new List<EmployeeSalaryDetailsDO>();

            try
            {
                getDrtolist getDrtolistParam = new getDrtolist();

                List<MySqlParameter> mysqlParameters = new List<MySqlParameter>();

                mysqlParameters.Add(new MySqlParameter("@p_emp_code", empCode));
                mysqlParameters.Add(new MySqlParameter("@p_emp_name", empName));
                mysqlParameters.Add(new MySqlParameter("@p_status", status));

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
                    ex.Message + ex.StackTrace,
                    UserId);
            }

            return listData;
        }
        //public List<EmployeeReimbursementDO> GetEmployeeReimbursementDetails()
        //{
        //    List<EmployeeReimbursementDO> items = new List<EmployeeReimbursementDO>();

        //    if (string.IsNullOrWhiteSpace(Sqlconnection))
        //    {
        //        return items;
        //    }

        //    try
        //    {
        //        using (MySqlConnection con = new MySqlConnection(Sqlconnection))
        //        using (MySqlCommand cmd = new MySqlCommand("sp_get_reimbursement_dashboard_details", con))
        //        {
        //            cmd.CommandType = CommandType.StoredProcedure;

        //            con.Open();

        //            using (MySqlDataReader dr = cmd.ExecuteReader())
        //            {
        //                while (dr.Read())
        //                {
        //                    items.Add(new EmployeeReimbursementDO
        //                    {
        //                        reimbursement_id = Convert.ToInt32(dr["reimbursement_id"]),   // <-- Add this
        //                        employee_name = dr["employee_name"].ToString(),
        //                        employee_code = dr["employee_code"].ToString(),
        //                        claim_type = dr["claim_type"].ToString(),
        //                        claim_amount = Convert.ToDecimal(dr["claim_amount"]),
        //                        claim_date = Convert.ToDateTime(dr["claim_date"]),
        //                        status = dr["status"].ToString()
        //                    });
        //                }
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        CommonBL errorlog = new CommonBL();
        //        errorlog.fnStoreErrorLog(
        //            "AccountDashboardBL",
        //            "GetEmployeeReimbursementDetails",
        //            "Exception Message: " + ex.Message +
        //            " StackTrace: " + ex.StackTrace,
        //            UserId);
        //    }

        //    return items;
        //}
        public List<EmployeeReimbursementDO> GetEmployeeReimbursementDetails(string empCode, string empName, string status)
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

                    // Pass Parameters
                    cmd.Parameters.Add(new MySqlParameter("@p_emp_code", empCode ?? ""));
                    cmd.Parameters.Add(new MySqlParameter("@p_emp_name", empName ?? ""));
                    cmd.Parameters.Add(new MySqlParameter("@p_status", status ?? ""));

                    con.Open();

                    using (MySqlDataReader dr = cmd.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            items.Add(new EmployeeReimbursementDO
                            {
                                reimbursement_id = Convert.ToInt32(dr["reimbursement_id"]),
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
        public List<UpdateSalaryStatusDO> UpdateSalaryStatus(int salarySlipDetailsId, string status, string updatedBy)
        {
            List<UpdateSalaryStatusDO> listData = new List<UpdateSalaryStatusDO>();

            try
            {
                getDrtolist getDrtolistParam = new getDrtolist();

                List<MySqlParameter> mysqlParameters = new List<MySqlParameter>();

                mysqlParameters.Add(DataClass.GetParameter("@p_salary_slip_details_id", salarySlipDetailsId));
                mysqlParameters.Add(DataClass.GetParameter("@p_status", status));
                mysqlParameters.Add(DataClass.GetParameter("@p_updated_by", updatedBy));

                listData = getDrtolistParam.getdatafromreder<UpdateSalaryStatusDO>(
                    DataClass.GetDataReaderFromSpWithParam(
                        mysqlParameters,
                        DBName,
                        "sp_update_salary_status"));
            }
            catch (Exception ex)
            {
                CommonBL errorlog = new CommonBL();

                errorlog.fnStoreErrorLog(
                    "AccountDashboardBL",
                    "UpdateSalaryStatus",
                    "Exception Message : " + ex.Message +
                    " StackTrace : " + ex.StackTrace,
                    UserId);
            }

            return listData;
        }

        public List<SalaryPaidMailDO> GetSalaryPaidMailDetails(int salarySlipId)
        {
            List<SalaryPaidMailDO> listData = new List<SalaryPaidMailDO>();

            try
            {
                getDrtolist getDrtolistParam = new getDrtolist();

                List<MySqlParameter> mysqlParameters = new List<MySqlParameter>();

                mysqlParameters.Add(new MySqlParameter("@p_salary_slip_id", salarySlipId));

                listData = getDrtolistParam.getdatafromreder<SalaryPaidMailDO>(
                    DataClass.GetDataReaderFromSpWithParam(
                        mysqlParameters,
                        DBName,
                        "sp_SendSalaryPaidMailDetails"));
            }
            catch (Exception ex)
            {
                CommonBL errorlog = new CommonBL();
                errorlog.fnStoreErrorLog(
                    "HomeBL",
                    "GetSalaryPaidMailDetails",
                    ex.Message + ex.StackTrace,
                    UserId);
            }

            return listData;
        }
        public void SendSalaryPaidMail(string toMail, string ccMail, string subject, string body)
        {
            try
            {
                string Email = ConfigurationManager.AppSettings["SenderEmail"];
                string Password = ConfigurationManager.AppSettings["SenderPassword"];
                int Port = Convert.ToInt32(ConfigurationManager.AppSettings["SenderPort"]);
                string Host = ConfigurationManager.AppSettings["SenderHost"];

                using (MailMessage mail = new MailMessage())
                {
                    mail.From = new MailAddress(Email, "HRMS");

                    foreach (string email in toMail.Split(';'))
                    {
                        if (!string.IsNullOrWhiteSpace(email))
                            mail.To.Add(email.Trim());
                    }

                    if (!string.IsNullOrWhiteSpace(ccMail))
                    {
                        foreach (string email in ccMail.Split(';'))
                        {
                            if (!string.IsNullOrWhiteSpace(email))
                                mail.CC.Add(email.Trim());
                        }
                    }

                    mail.Subject = subject;
                    mail.Body = body;
                    mail.IsBodyHtml = true;

                    using (SmtpClient smtp = new SmtpClient(Host, Port))
                    {
                        smtp.UseDefaultCredentials = false;
                        smtp.Credentials = new NetworkCredential(Email, Password);
                        smtp.EnableSsl = true;

                        smtp.Send(mail);
                    }
                }
            }
            catch (Exception ex)
            {
                CommonBL errorlog = new CommonBL();
                errorlog.fnStoreErrorLog(
                    "HomeBL",
                    "SendSalaryPaidMail",
                    ex.Message + ex.StackTrace,
                    UserId);
            }
        }

        public List<UpdateReimbursementStatusDO> UpdatereimbSalaryStatus(int reimbursementId, string status, string updatedBy)
        {
            List<UpdateReimbursementStatusDO> listData = new List<UpdateReimbursementStatusDO>();

            try
            {
                using (MySqlConnection con = new MySqlConnection(Sqlconnection))
                {
                    using (MySqlCommand cmd = new MySqlCommand("sp_update_reimbursement_status_byID", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;

                        cmd.Parameters.AddWithValue("@p_reimbursement_id", reimbursementId);
                        cmd.Parameters.AddWithValue("@p_status", status);
                        cmd.Parameters.AddWithValue("@p_updated_by", updatedBy);

                        con.Open();

                        using (MySqlDataReader dr = cmd.ExecuteReader())
                        {
                            while (dr.Read())
                            {
                                listData.Add(new UpdateReimbursementStatusDO
                                {
                                    reimbursement_id = Convert.ToInt32(dr["reimbursement_id"]),
                                    status = dr["status"].ToString(),
                                    updated_by = Convert.ToInt32(dr["updated_by"]),
                                    Success = dr["Success"].ToString(),
                                    Result = dr["Result"].ToString()
                                });
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                CommonBL errorlog = new CommonBL();
                errorlog.fnStoreErrorLog(
                    "AccountDashboardBL",
                    "UpdatereimbSalaryStatus",
                    ex.Message + ex.StackTrace,
                    UserId);
            }

            return listData;
        }
        public List<SalaryPaidMailDO> GetReimbPaidMailDetails(int reimbursementId)
        {
            List<SalaryPaidMailDO> list = new List<SalaryPaidMailDO>();

            try
            {
                getDrtolist obj = new getDrtolist();

                List<MySqlParameter> parameters = new List<MySqlParameter>();

                parameters.Add(DataClass.GetParameter("@p_reimbursement_id", reimbursementId));

                list = obj.getdatafromreder<SalaryPaidMailDO>(
                    DataClass.GetDataReaderFromSpWithParam(
                        parameters,
                        DBName,
                        "sp_SendReimbursementPaidMailDetails"));
            }
            catch (Exception ex)
            {
                CommonBL errorlog = new CommonBL();
                errorlog.fnStoreErrorLog(
                    "AccountDashboardBL",
                    "GetReimbPaidMailDetails",
                    ex.Message + ex.StackTrace,
                    UserId);
            }

            return list;
        }
    }
}
