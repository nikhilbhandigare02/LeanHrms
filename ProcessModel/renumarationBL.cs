using DataObject;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace ProcessModel
{
    public class renumarationBL
    {
        private string DBName = ConfigurationManager.AppSettings["DBName"];
        private static string MySqlconnection = ConfigurationManager.ConnectionStrings["MysqlConnection"].ConnectionString;

        public List<renumarationDO> SaveRenumerationDetails(renumarationDO renum)
        {
            List<renumarationDO> listdata = new List<renumarationDO>();
            getDrtolist getDrtolistParam = new getDrtolist();
            List<MySqlParameter> mysqlParameters = new List<MySqlParameter>();

            try
            {
                string userId = HttpContext.Current.Session["userId"] != null
                            ? HttpContext.Current.Session["userId"].ToString()
                            : "";

                mysqlParameters.Add(DataClass.GetParameter("p_type", "InsertRenumeration"));
                mysqlParameters.Add(DataClass.GetParameter("p_user_id", renum.UserId));
                mysqlParameters.Add(DataClass.GetParameter("p_employee_name", renum.EmployeeName));
                mysqlParameters.Add(DataClass.GetParameter("p_monthly_ctc", renum.MonthlyCTC));
                mysqlParameters.Add(DataClass.GetParameter("p_yearly_ctc", renum.YearlyCTC));
                mysqlParameters.Add(DataClass.GetParameter("p_effective_date", renum.EffectiveDate));
                mysqlParameters.Add(DataClass.GetParameter("p_basic_salary", renum.BasicSalary));
                mysqlParameters.Add(DataClass.GetParameter("p_gross_salary", renum.GrossSalary));
                mysqlParameters.Add(DataClass.GetParameter("p_hra", renum.HRA));
                mysqlParameters.Add(DataClass.GetParameter("p_conveyance_allowance", renum.ConveyanceAllowance));
                mysqlParameters.Add(DataClass.GetParameter("p_special_allowance", renum.SpecialAllowance));
                mysqlParameters.Add(DataClass.GetParameter("p_other_earnings", renum.OtherEarnings));
                mysqlParameters.Add(DataClass.GetParameter("p_pf", renum.PF));
                mysqlParameters.Add(DataClass.GetParameter("p_esi", renum.ESI));
                mysqlParameters.Add(DataClass.GetParameter("p_professional_tax", renum.ProfessionalTax));
                mysqlParameters.Add(DataClass.GetParameter("p_tds", renum.TDS));
                mysqlParameters.Add(DataClass.GetParameter("p_other_deductions", renum.OtherDeductions));
                mysqlParameters.Add(DataClass.GetParameter("p_inserted_by", renum.InsertedBy));

                listdata = getDrtolistParam.getdatafromreder<renumarationDO>(
                    DataClass.GetDataReaderFromSpWithParam(mysqlParameters, DBName, "SP_InsertRenumeration"));
            }
            catch (Exception ex)
            {
                CommonBL errorlog = new CommonBL();
                errorlog.fnStoreErrorLog(
                    "renumarationBL",
                    "SaveRenumerationDetails",
                    "Exception Message: " + ex.Message + " | StackTrace=" + ex.StackTrace,
                    Convert.ToString(HttpContext.Current.Session["userId"]));
            }

            return listdata;
        }

        public bool CheckEffectiveDateOverlap(int userId, DateTime effectiveDate)
        {
            bool isOverlap = false;
            try
            {
                using (MySqlConnection con = new MySqlConnection(MySqlconnection))
                {
                    con.Open();
                    string query = @"SELECT COUNT(*) FROM renumeration_master 
                                    WHERE user_id = @p_user_id 
                                    AND effective_date = @p_effective_date AND is_active = 1;";

                    using (MySqlCommand cmd = new MySqlCommand(query, con))
                    {
                        cmd.Parameters.AddWithValue("@p_user_id", userId);
                        cmd.Parameters.AddWithValue("@p_effective_date", effectiveDate);
                        object countObj = cmd.ExecuteScalar();
                        int count = Convert.ToInt32(countObj ?? 0);
                        if (count > 0)
                        {
                            isOverlap = true;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                CommonBL errorlog = new CommonBL();
                errorlog.fnStoreErrorLog("renumarationBL", "CheckEffectiveDateOverlap", ex.Message, Convert.ToString(HttpContext.Current.Session["userId"]));
            }

            return isOverlap;
        }

        public List<RemunerationComponent> GetRemunerationComponents()
        {
            List<RemunerationComponent> components = new List<RemunerationComponent>();
            try
            {
                using (MySqlConnection con = new MySqlConnection(MySqlconnection))
                {
                    con.Open();
                    using (MySqlCommand cmd = new MySqlCommand("Sp_BindComponent_Forremuneration", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        using (MySqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                RemunerationComponent component = new RemunerationComponent();
                                component.Id = Convert.ToInt32(reader["Id"]);
                                component.Text = reader["Text"].ToString();
                                component.ComponentType = reader["ComponentType"] != DBNull.Value ? reader["ComponentType"].ToString() : string.Empty;
                                components.Add(component);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                CommonBL errorlog = new CommonBL();
                errorlog.fnStoreErrorLog("renumarationBL", "GetRemunerationComponents", ex.Message, Convert.ToString(HttpContext.Current.Session["userId"]));
            }
            return components;
        }

        public (string Status, string Message, int RemunerationId) SaveRemunerationDetails(
            string salaryStructureId,
            int? employeeId,
            int? status,
            DateTime effectiveFromDate,
            DateTime? effectiveToDate,
            int? employeeCategory,
            decimal ctcAmount,
            decimal grossSalary,
            decimal monthlySalary,
            decimal annualSalary,
            decimal basicSalary,
            decimal hra,
            decimal conveyanceAllowance,
            decimal medicalAllowance,
            decimal specialAllowance,
            decimal educationAllowance,
            decimal travelAllowance,
            decimal uniformAllowance,
            decimal telephoneAllowance,
            decimal foodAllowance,
            decimal shiftAllowance,
            decimal incentive,
            decimal bonus,
            decimal otherAllowance,
            decimal pf,
            decimal esi,
            decimal professionalTax,
            decimal tds,
            decimal labourWelfareFund,
            decimal loanDeduction,
            decimal advanceRecovery,
            decimal otherDeductions,
            int insertedBy)
        {
            string resultStatus = "Failed";
            string resultMessage = "Remuneration details save failed.";
            int resultRemunerationId = 0;

            try
            {
                using (MySqlConnection con = new MySqlConnection(MySqlconnection))
                using (MySqlCommand cmd = new MySqlCommand("SP_SaveRemunerationDetails", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandTimeout = 0;

                    cmd.Parameters.AddWithValue("@p_salary_structure_id", salaryStructureId);
                    cmd.Parameters.AddWithValue("@p_employee_id", ToDbIntValue(employeeId));
                    cmd.Parameters.AddWithValue("@p_status", ToDbIntValue(status));
                    cmd.Parameters.AddWithValue("@p_effective_from_date", effectiveFromDate);
                    cmd.Parameters.AddWithValue("@p_effective_to_date", ToDbValue(effectiveToDate));
                    cmd.Parameters.AddWithValue("@p_employee_category", ToDbIntValue(employeeCategory));
                    cmd.Parameters.AddWithValue("@p_ctc_amount", ctcAmount);
                    cmd.Parameters.AddWithValue("@p_gross_salary", grossSalary);
                    cmd.Parameters.AddWithValue("@p_monthly_salary", monthlySalary);
                    cmd.Parameters.AddWithValue("@p_annual_salary", annualSalary);
                    cmd.Parameters.AddWithValue("@p_basic_salary", basicSalary);
                    cmd.Parameters.AddWithValue("@p_hra", hra);
                    cmd.Parameters.AddWithValue("@p_conveyance_allowance", conveyanceAllowance);
                    cmd.Parameters.AddWithValue("@p_medical_allowance", medicalAllowance);
                    cmd.Parameters.AddWithValue("@p_special_allowance", specialAllowance);
                    cmd.Parameters.AddWithValue("@p_education_allowance", educationAllowance);
                    cmd.Parameters.AddWithValue("@p_travel_allowance", travelAllowance);
                    cmd.Parameters.AddWithValue("@p_uniform_allowance", uniformAllowance);
                    cmd.Parameters.AddWithValue("@p_telephone_allowance", telephoneAllowance);
                    cmd.Parameters.AddWithValue("@p_food_allowance", foodAllowance);
                    cmd.Parameters.AddWithValue("@p_shift_allowance", shiftAllowance);
                    cmd.Parameters.AddWithValue("@p_incentive", incentive);
                    cmd.Parameters.AddWithValue("@p_bonus", bonus);
                    cmd.Parameters.AddWithValue("@p_other_allowance", otherAllowance);
                    cmd.Parameters.AddWithValue("@p_pf", pf);
                    cmd.Parameters.AddWithValue("@p_esi", esi);
                    cmd.Parameters.AddWithValue("@p_professional_tax", professionalTax);
                    cmd.Parameters.AddWithValue("@p_tds", tds);
                    cmd.Parameters.AddWithValue("@p_labour_welfare_fund", labourWelfareFund);
                    cmd.Parameters.AddWithValue("@p_loan_deduction", loanDeduction);
                    cmd.Parameters.AddWithValue("@p_advance_recovery", advanceRecovery);
                    cmd.Parameters.AddWithValue("@p_other_deductions", otherDeductions);
                    cmd.Parameters.AddWithValue("@p_inserted_by", insertedBy);

                    con.Open();
                    using (MySqlDataReader dr = cmd.ExecuteReader())
                    {
                        if (dr.Read())
                        {
                            resultStatus = ReadString(dr, "Status");
                            resultMessage = ReadString(dr, "Message");
                            resultRemunerationId = ReadInt(dr, "remuneration_id") ?? 0;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                CommonBL errorlog = new CommonBL();
                errorlog.fnStoreErrorLog("renumarationBL", "SaveRemunerationDetails", "Exception Message: " + ex.Message + " StackTrace: " + ex.StackTrace, Convert.ToString(HttpContext.Current.Session["userId"]));
                resultMessage = "Remuneration details save failed due to an exception: " + ex.Message;
            }

            return (resultStatus, resultMessage, resultRemunerationId);
        }

        public (string Status, string Message) UpdateRemunerationDetails(
            int remunerationId,
            string salaryStructureId,
            int? employeeId,
            int? status,
            DateTime effectiveFromDate,
            DateTime? effectiveToDate,
            int? employeeCategory,
            decimal ctcAmount,
            decimal grossSalary,
            decimal monthlySalary,
            decimal annualSalary,
            decimal basicSalary,
            decimal hra,
            decimal conveyanceAllowance,
            decimal medicalAllowance,
            decimal specialAllowance,
            decimal educationAllowance,
            decimal travelAllowance,
            decimal uniformAllowance,
            decimal telephoneAllowance,
            decimal foodAllowance,
            decimal shiftAllowance,
            decimal incentive,
            decimal bonus,
            decimal otherAllowance,
            decimal pf,
            decimal esi,
            decimal professionalTax,
            decimal tds,
            decimal labourWelfareFund,
            decimal loanDeduction,
            decimal advanceRecovery,
            decimal otherDeductions,
            int updatedBy)
        {
            string resultStatus = "Failed";
            string resultMessage = "Remuneration details update failed.";

            try
            {
                using (MySqlConnection con = new MySqlConnection(MySqlconnection))
                using (MySqlCommand cmd = new MySqlCommand("SP_UpdateRemunerationDetails", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandTimeout = 0;

                    cmd.Parameters.AddWithValue("@p_remuneration_id", remunerationId);
                    cmd.Parameters.AddWithValue("@p_salary_structure_id", salaryStructureId);
                    cmd.Parameters.AddWithValue("@p_employee_id", ToDbIntValue(employeeId));
                    cmd.Parameters.AddWithValue("@p_status", ToDbIntValue(status));
                    cmd.Parameters.AddWithValue("@p_effective_from_date", effectiveFromDate);
                    cmd.Parameters.AddWithValue("@p_effective_to_date", ToDbValue(effectiveToDate));
                    cmd.Parameters.AddWithValue("@p_employee_category", ToDbIntValue(employeeCategory));
                    cmd.Parameters.AddWithValue("@p_ctc_amount", ctcAmount);
                    cmd.Parameters.AddWithValue("@p_gross_salary", grossSalary);
                    cmd.Parameters.AddWithValue("@p_monthly_salary", monthlySalary);
                    cmd.Parameters.AddWithValue("@p_annual_salary", annualSalary);
                    cmd.Parameters.AddWithValue("@p_basic_salary", basicSalary);
                    cmd.Parameters.AddWithValue("@p_hra", hra);
                    cmd.Parameters.AddWithValue("@p_conveyance_allowance", conveyanceAllowance);
                    cmd.Parameters.AddWithValue("@p_medical_allowance", medicalAllowance);
                    cmd.Parameters.AddWithValue("@p_special_allowance", specialAllowance);
                    cmd.Parameters.AddWithValue("@p_education_allowance", educationAllowance);
                    cmd.Parameters.AddWithValue("@p_travel_allowance", travelAllowance);
                    cmd.Parameters.AddWithValue("@p_uniform_allowance", uniformAllowance);
                    cmd.Parameters.AddWithValue("@p_telephone_allowance", telephoneAllowance);
                    cmd.Parameters.AddWithValue("@p_food_allowance", foodAllowance);
                    cmd.Parameters.AddWithValue("@p_shift_allowance", shiftAllowance);
                    cmd.Parameters.AddWithValue("@p_incentive", incentive);
                    cmd.Parameters.AddWithValue("@p_bonus", bonus);
                    cmd.Parameters.AddWithValue("@p_other_allowance", otherAllowance);
                    cmd.Parameters.AddWithValue("@p_pf", pf);
                    cmd.Parameters.AddWithValue("@p_esi", esi);
                    cmd.Parameters.AddWithValue("@p_professional_tax", professionalTax);
                    cmd.Parameters.AddWithValue("@p_tds", tds);
                    cmd.Parameters.AddWithValue("@p_labour_welfare_fund", labourWelfareFund);
                    cmd.Parameters.AddWithValue("@p_loan_deduction", loanDeduction);
                    cmd.Parameters.AddWithValue("@p_advance_recovery", advanceRecovery);
                    cmd.Parameters.AddWithValue("@p_other_deductions", otherDeductions);
                    cmd.Parameters.AddWithValue("@p_updated_by", updatedBy);

                    con.Open();
                    using (MySqlDataReader dr = cmd.ExecuteReader())
                    {
                        if (dr.Read())
                        {
                            resultStatus = ReadString(dr, "Status");
                            resultMessage = ReadString(dr, "Message");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                CommonBL errorlog = new CommonBL();
                errorlog.fnStoreErrorLog("renumarationBL", "UpdateRemunerationDetails", "Exception Message: " + ex.Message + " StackTrace: " + ex.StackTrace, Convert.ToString(HttpContext.Current.Session["userId"]));
                resultMessage = "Remuneration details update failed due to an exception: " + ex.Message;
            }

            return (resultStatus, resultMessage);
        }

        private object ToDbValue(DateTime? value)
        {
            return value.HasValue ? (object)value.Value : DBNull.Value;
        }

        private object ToDbIntValue(int? value)
        {
            return value.HasValue && value > 0 ? (object)value.Value : DBNull.Value;
        }

        private string ReadString(IDataRecord record, string columnName)
        {
            for (int i = 0; i < record.FieldCount; i++)
            {
                if (string.Equals(record.GetName(i), columnName, StringComparison.OrdinalIgnoreCase))
                {
                    return record[i] == DBNull.Value ? string.Empty : Convert.ToString(record[i]);
                }
            }
            return string.Empty;
        }

        private int? ReadInt(IDataRecord record, string columnName)
        {
            string str = ReadString(record, columnName);
            if (int.TryParse(str, out int value))
            {
                return value;
            }
            return null;
        }

        private decimal? ReadDecimal(IDataRecord record, string columnName)
        {
            string str = ReadString(record, columnName);
            if (decimal.TryParse(str, out decimal value))
            {
                return value;
            }
            return null;
        }

        private DateTime? ReadDateTime(IDataRecord record, string columnName)
        {
            if (record[columnName] == DBNull.Value)
                return null;
            return Convert.ToDateTime(record[columnName]);
        }

        public renumarationDO GetRemunerationDetailsById(int remunerationId)
        {
            renumarationDO renum = null;
            try
            {
                using (MySqlConnection con = new MySqlConnection(MySqlconnection))
                using (MySqlCommand cmd = new MySqlCommand("SP_GetRemunerationDetailsById", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@p_remuneration_id", remunerationId);
                    con.Open();
                    using (MySqlDataReader dr = cmd.ExecuteReader())
                    {
                        if (dr.Read())
                        {
                            // First get all values to avoid calling Read multiple times
                            int? renumId = ReadInt(dr, "remuneration_id");
                            int? userId = ReadInt(dr, "user_id");
                            int? insertedBy = ReadInt(dr, "inserted_by");

                            decimal? basic = ReadDecimal(dr, "basic_salary");
                            decimal? hra = ReadDecimal(dr, "hra");
                            decimal? conveyance = ReadDecimal(dr, "conveyance_allowance");
                            decimal? medical = ReadDecimal(dr, "medical_allowance");
                            decimal? special = ReadDecimal(dr, "special_allowance");
                            decimal? education = ReadDecimal(dr, "education_allowance");
                            decimal? travel = ReadDecimal(dr, "travel_allowance");
                            decimal? uniform = ReadDecimal(dr, "uniform_allowance");
                            decimal? telephone = ReadDecimal(dr, "telephone_allowance");
                            decimal? food = ReadDecimal(dr, "food_allowance");
                            decimal? shift = ReadDecimal(dr, "shift_allowance");
                            decimal? incentive = ReadDecimal(dr, "incentive");
                            decimal? bonus = ReadDecimal(dr, "bonus");
                            decimal? otherAllow = ReadDecimal(dr, "other_allowance");

                            decimal? pf = ReadDecimal(dr, "pf");
                            decimal? esi = ReadDecimal(dr, "esi");
                            decimal? pt = ReadDecimal(dr, "professional_tax");
                            decimal? tds = ReadDecimal(dr, "tds");
                            decimal? lwf = ReadDecimal(dr, "labour_welfare_fund");
                            decimal? loan = ReadDecimal(dr, "loan_deduction");
                            decimal? advance = ReadDecimal(dr, "advance_recovery");
                            decimal? otherDed = ReadDecimal(dr, "other_deductions");

                            renum = new renumarationDO
                            {
                                RenumerationId = renumId ?? 0,
                                SalaryStructureID = ReadString(dr, "salary_structure_id"),
                                UserId = userId ?? 0,
                                Status = ReadString(dr, "status"),
                                EffectiveFromDate = ReadDateTime(dr, "effective_from_date"),
                                EffectiveToDate = ReadDateTime(dr, "effective_to_date"),
                                EmployeeCategory = ReadString(dr, "employee_category"),
                                CTCAmount = ReadDecimal(dr, "ctc_amount"),
                                GrossSalary = ReadDecimal(dr, "gross_salary"),
                                MonthlySalary = ReadDecimal(dr, "monthly_salary"),
                                AnnualSalary = ReadDecimal(dr, "annual_salary"),

                                // Earnings components
                                BasicSalary = basic,
                                IsBasicSalaryEnabled = basic > 0,
                                HRA = hra,
                                IsHRAEnabled = hra > 0,
                                ConveyanceAllowance = conveyance,
                                IsConveyanceAllowanceEnabled = conveyance > 0,
                                MedicalAllowance = medical,
                                IsMedicalAllowanceEnabled = medical > 0,
                                SpecialAllowance = special,
                                IsSpecialAllowanceEnabled = special > 0,
                                EducationAllowance = education,
                                IsEducationAllowanceEnabled = education > 0,
                                TravelAllowance = travel,
                                IsTravelAllowanceEnabled = travel > 0,
                                UniformAllowance = uniform,
                                IsUniformAllowanceEnabled = uniform > 0,
                                TelephoneAllowance = telephone,
                                IsTelephoneAllowanceEnabled = telephone > 0,
                                FoodAllowance = food,
                                IsFoodAllowanceEnabled = food > 0,
                                ShiftAllowance = shift,
                                IsShiftAllowanceEnabled = shift > 0,
                                Incentive = incentive,
                                IsIncentiveEnabled = incentive > 0,
                                Bonus = bonus,
                                IsBonusEnabled = bonus > 0,
                                OtherAllowance = otherAllow,
                                IsOtherAllowanceEnabled = otherAllow > 0,

                                // Deductions components
                                PF = pf,
                                IsPFEnabled = pf > 0,
                                ESI = esi,
                                IsESIEnabled = esi > 0,
                                ProfessionalTax = pt,
                                IsProfessionalTaxEnabled = pt > 0,
                                TDS = tds,
                                IsTDSEnabled = tds > 0,
                                LabourWelfareFund = lwf,
                                IsLabourWelfareFundEnabled = lwf > 0,
                                LoanDeduction = loan,
                                IsLoanDeductionEnabled = loan > 0,
                                AdvanceRecovery = advance,
                                IsAdvanceRecoveryEnabled = advance > 0,
                                OtherDeductions = otherDed,
                                IsOtherDeductionsEnabled = otherDed > 0,

                                InsertedBy = insertedBy,
                                InsertedDate = ReadDateTime(dr, "inserted_date")
                            };
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                CommonBL errorlog = new CommonBL();
                errorlog.fnStoreErrorLog("renumarationBL", "GetRemunerationDetailsById", "Exception Message: " + ex.Message + " StackTrace: " + ex.StackTrace, Convert.ToString(HttpContext.Current.Session["userId"]));
            }

            return renum;
        }

        public (string Status, string Message) DeactivateRemuneration(int remunerationId)
        {
            string resultStatus = "Failed";
            string resultMessage = "Remuneration deactivation failed.";

            try
            {
                using (MySqlConnection con = new MySqlConnection(MySqlconnection))
                using (MySqlCommand cmd = new MySqlCommand("sp_deactive_remuneration", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@p_remuneration_id", remunerationId);
                    
                    con.Open();
                    using (MySqlDataReader dr = cmd.ExecuteReader())
                    {
                        if (dr.Read())
                        {
                            resultStatus = ReadString(dr, "Status");
                            resultMessage = ReadString(dr, "Message");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                CommonBL errorlog = new CommonBL();
                errorlog.fnStoreErrorLog("renumarationBL", "DeactivateRemuneration", "Exception Message: " + ex.Message + " StackTrace: " + ex.StackTrace, Convert.ToString(HttpContext.Current.Session["userId"]));
                resultMessage = "Remuneration deactivation failed due to an exception: " + ex.Message;
            }

            return (resultStatus, resultMessage);
        }
    }
}
