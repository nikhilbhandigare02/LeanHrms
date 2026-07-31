using DataObject;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Configuration;

namespace ProcessModel
{
    public class ExitClearanceBL
    {
        private readonly string DBName = ConfigurationManager.ConnectionStrings["MysqlConnection"].ConnectionString;
        private readonly string SqlConnectionDBName = ConfigurationManager.ConnectionStrings["SqlConnection"].ConnectionString;
        private readonly string UserId = "0";

        public List<ExitClearanceMasterDO> GetExitClearanceMaster()
        {
            List<ExitClearanceMasterDO> clearanceItems = new List<ExitClearanceMasterDO>();

            try
            {
                using (var dr = DataClass.GetDataReaderFromSp(DBName, "sp_getexitclearance"))
                {
                    while (dr.Read())
                    {
                        ExitClearanceMasterDO item = new ExitClearanceMasterDO
                        {
                            ClearanceMasterId = Convert.ToInt32(dr["clearance_master_id"]),
                            DepartmentName = Convert.ToString(dr["department_name"]),
                            ClearanceItem = Convert.ToString(dr["clearance_item"]),
                            DisplayOrder = Convert.ToInt32(dr["display_order"])
                        };
                        clearanceItems.Add(item);
                    }
                }
            }
            catch (Exception ex)
            {
                CommonBL errorlog = new CommonBL();
                errorlog.fnStoreErrorLog("ExitClearanceBL", "GetExitClearanceMaster", "Exception Message: " + ex.Message + " StackTrace: " + ex.StackTrace, UserId);
            }

            return clearanceItems;
        }

        public ExitClearanceEmployeeDetailsDO GetEmployeeDetailsByResignationId(int resignationId)
        {
            ExitClearanceEmployeeDetailsDO employeeDetails = null;

            try
            {
                List<MySqlParameter> parameters = new List<MySqlParameter>
                {
                    DataClass.GetParameter("@p_employee_resignation_id", resignationId)
                };

                using (var dr = DataClass.GetDataReaderFromSpWithParam(parameters, SqlConnectionDBName, "sp_getemployeedetailsbyresignationid"))
                {
                    if (dr.Read())
                    {
                        employeeDetails = new ExitClearanceEmployeeDetailsDO
                        {
                            EmployeeResignationId = Convert.ToInt32(dr["employee_resignation_id"]),
                            UserId = Convert.ToInt32(dr["user_id"]),
                            EmployeeName = Convert.ToString(dr["user_fullname"]),
                            EmployeeCode = Convert.ToString(dr["emp_code"]),
                            EmployeeEmail = Convert.ToString(dr["user_mail_id"]),
                            ResignationDate = Convert.ToDateTime(dr["resignation_date"]),
                            LastWorkingDate = Convert.ToDateTime(dr["last_working_date"])
                        };
                    }
                }
            }
            catch (Exception ex)
            {
                CommonBL errorlog = new CommonBL();
                errorlog.fnStoreErrorLog("ExitClearanceBL", "GetEmployeeDetailsByResignationId", "Exception Message: " + ex.Message + " StackTrace: " + ex.StackTrace, UserId);
            }

            return employeeDetails;
        }

        public List<ExitClearanceEmployeeDO> GetEmployeesWithResignationRequests()
        {
            List<ExitClearanceEmployeeDO> employees = new List<ExitClearanceEmployeeDO>();

            try
            {
                using (var dr = DataClass.GetDataReaderFromSp(DBName, "sp_getemployeeswithresignationrequests"))
                {
                    while (dr.Read())
                    {
                        ExitClearanceEmployeeDO employee = new ExitClearanceEmployeeDO
                        {
                            EmployeeResignationId = Convert.ToInt32(dr["employee_resignation_id"]),
                            UserId = Convert.ToInt32(dr["user_id"]),
                            EmployeeName = Convert.ToString(dr["user_fullname"]),
                            EmployeeCode = Convert.ToString(dr["emp_code"]),
                            Department = Convert.ToString(dr["department"])
                        };
                        employees.Add(employee);
                    }
                }
            }
            catch (Exception ex)
            {
                CommonBL errorlog = new CommonBL();
                errorlog.fnStoreErrorLog("ExitClearanceBL", "GetEmployeesWithResignationRequests", "Exception Message: " + ex.Message + " StackTrace: " + ex.StackTrace, UserId);
            }

            return employees;
        }

        public ExitClearanceDataDO GetExitClearanceByResignationId(int employeeResignationId)
        {
            ExitClearanceDataDO clearanceData = null;

            try
            {
                List<MySqlParameter> parameters = new List<MySqlParameter>
                {
                    DataClass.GetParameter("@p_employee_resignation_id", employeeResignationId)
                };

                using (var dr = DataClass.GetDataReaderFromSpWithParam(parameters, DBName, "sp_getexitclearancebyid"))
                {
                    if (dr.Read())
                    {
                        clearanceData = new ExitClearanceDataDO
                        {
                            ExitClearanceId = Convert.ToInt32(dr["exit_clearance_id"]),
                            EmployeeResignationId = Convert.ToInt32(dr["employee_resignation_id"]),
                            EmpCode = Convert.ToString(dr["emp_code"]),

                            // IT Department
                            LaptopReturned = Convert.ToInt32(dr["laptop_returned"]),
                            DesktopReturned = Convert.ToInt32(dr["desktop_returned"]),
                            MobilePhoneReturned = Convert.ToInt32(dr["mobile_phone_returned"]),
                            EmailDisabled = Convert.ToInt32(dr["email_disabled"]),
                            VpnDisabled = Convert.ToInt32(dr["vpn_disabled"]),
                            HrmsAccessRemoved = Convert.ToInt32(dr["hrms_access_removed"]),
                            ItRemarks = Convert.ToString(dr["it_remarks"]),

                            // Administration Department
                            IdCardReturned = Convert.ToInt32(dr["id_card_returned"]),
                            AccessCardReturned = Convert.ToInt32(dr["access_card_returned"]),
                            OfficeKeysReturned = Convert.ToInt32(dr["office_keys_returned"]),
                            ParkingPassReturned = Convert.ToInt32(dr["parking_pass_returned"]),
                            AdministrationRemarks = Convert.ToString(dr["administration_remarks"]),

                            // Finance Department
                            LoanRecoveryCompleted = Convert.ToInt32(dr["loan_recovery_completed"]),
                            SalaryAdvanceRecovered = Convert.ToInt32(dr["salary_advance_recovered"]),
                            ExpenseClaimsProcessed = Convert.ToInt32(dr["expense_claims_processed"]),
                            FinanceRemarks = Convert.ToString(dr["finance_remarks"]),

                            // Security Department
                            BiometricDisabled = Convert.ToInt32(dr["biometric_disabled"]),
                            BuildingAccessRevoked = Convert.ToInt32(dr["building_access_revoked"]),
                            SecurityRemarks = Convert.ToString(dr["security_remarks"]),

                            InsertedBy = Convert.ToInt32(dr["inserted_by"]),
                            InsertedDate = Convert.ToDateTime(dr["inserted_date"]),
                            ModifiedBy = dr["modified_by"] != DBNull.Value ? (int?)Convert.ToInt32(dr["modified_by"]) : null,
                            ModifiedDate = dr["modified_date"] != DBNull.Value ? (DateTime?)Convert.ToDateTime(dr["modified_date"]) : null,
                            IsActive = Convert.ToInt32(dr["is_active"])
                        };
                    }
                }
            }
            catch (Exception ex)
            {
                CommonBL errorlog = new CommonBL();
                errorlog.fnStoreErrorLog("ExitClearanceBL", "GetExitClearanceByResignationId", "Exception Message: " + ex.Message + " StackTrace: " + ex.StackTrace, UserId);
            }

            return clearanceData;
        }

        public ExitClearanceResponseDO UpdateExitClearance(int exitClearanceId, int employeeResignationId, string employeeCode, Dictionary<string, object> clearanceData, int modifiedBy)
        {
            ExitClearanceResponseDO response = new ExitClearanceResponseDO
            {
                Status = "Failed",
                Message = "Exit Clearance update failed.",
                ExitClearanceId = exitClearanceId
            };

            try
            {
                List<MySqlParameter> parameters = new List<MySqlParameter>
                {
                    DataClass.GetParameter("@p_exit_clearance_id", exitClearanceId),
                    DataClass.GetParameter("@p_employee_resignation_id", employeeResignationId),
                    DataClass.GetParameter("@p_emp_code", employeeCode),

                    // IT Department
                    DataClass.GetParameter("@p_laptop_returned", GetIntValue(clearanceData, "laptop_returned")),
                    DataClass.GetParameter("@p_desktop_returned", GetIntValue(clearanceData, "desktop_returned")),
                    DataClass.GetParameter("@p_mobile_phone_returned", GetIntValue(clearanceData, "mobile_phone_returned")),
                    DataClass.GetParameter("@p_email_disabled", GetIntValue(clearanceData, "email_disabled")),
                    DataClass.GetParameter("@p_vpn_disabled", GetIntValue(clearanceData, "vpn_disabled")),
                    DataClass.GetParameter("@p_hrms_access_removed", GetIntValue(clearanceData, "hrms_access_removed")),
                    DataClass.GetParameter("@p_it_remarks", GetStringValue(clearanceData, "it_remarks")),

                    // Administration Department
                    DataClass.GetParameter("@p_id_card_returned", GetIntValue(clearanceData, "id_card_returned")),
                    DataClass.GetParameter("@p_access_card_returned", GetIntValue(clearanceData, "access_card_returned")),
                    DataClass.GetParameter("@p_office_keys_returned", GetIntValue(clearanceData, "office_keys_returned")),
                    DataClass.GetParameter("@p_parking_pass_returned", GetIntValue(clearanceData, "parking_pass_returned")),
                    DataClass.GetParameter("@p_administration_remarks", GetStringValue(clearanceData, "administration_remarks")),

                    // Finance Department
                    DataClass.GetParameter("@p_loan_recovery_completed", GetIntValue(clearanceData, "loan_recovery_completed")),
                    DataClass.GetParameter("@p_salary_advance_recovered", GetIntValue(clearanceData, "salary_advance_recovered")),
                    DataClass.GetParameter("@p_expense_claims_processed", GetIntValue(clearanceData, "expense_claims_processed")),
                    DataClass.GetParameter("@p_finance_remarks", GetStringValue(clearanceData, "finance_remarks")),

                    // Security Department
                    DataClass.GetParameter("@p_biometric_disabled", GetIntValue(clearanceData, "biometric_disabled")),
                    DataClass.GetParameter("@p_building_access_revoked", GetIntValue(clearanceData, "building_access_revoked")),
                    DataClass.GetParameter("@p_security_remarks", GetStringValue(clearanceData, "security_remarks")),

                    DataClass.GetParameter("@p_modified_by", modifiedBy)
                };

                using (var dr = DataClass.GetDataReaderFromSpWithParam(parameters, DBName, "SP_UpdateExitClearance"))
                {
                    if (dr.Read())
                    {
                        response.Status = Convert.ToString(dr["Status"]);
                        response.Message = Convert.ToString(dr["Message"]);
                        response.ExitClearanceId = Convert.ToInt32(dr["exit_clearance_id"]);
                    }
                }
            }
            catch (Exception ex)
            {
                CommonBL errorlog = new CommonBL();
                errorlog.fnStoreErrorLog("ExitClearanceBL", "UpdateExitClearance", "Exception Message: " + ex.Message + " StackTrace: " + ex.StackTrace, UserId);
            }

            return response;
        }

        public ExitClearanceResponseDO InsertExitClearance(int employeeResignationId, string employeeCode, Dictionary<string, object> clearanceData, int insertedBy)
        {
            ExitClearanceResponseDO response = new ExitClearanceResponseDO
            {
                Status = "Failed",
                Message = "Exit Clearance save failed.",
                ExitClearanceId = 0
            };

            try
            {
                List<MySqlParameter> parameters = new List<MySqlParameter>
                {
                    DataClass.GetParameter("@p_employee_resignation_id", employeeResignationId),
                    DataClass.GetParameter("@p_emp_code", employeeCode),

                    // IT Department
                    DataClass.GetParameter("@p_laptop_returned", GetIntValue(clearanceData, "laptop_returned")),
                    DataClass.GetParameter("@p_desktop_returned", GetIntValue(clearanceData, "desktop_returned")),
                    DataClass.GetParameter("@p_mobile_phone_returned", GetIntValue(clearanceData, "mobile_phone_returned")),
                    DataClass.GetParameter("@p_email_disabled", GetIntValue(clearanceData, "email_disabled")),
                    DataClass.GetParameter("@p_vpn_disabled", GetIntValue(clearanceData, "vpn_disabled")),
                    DataClass.GetParameter("@p_hrms_access_removed", GetIntValue(clearanceData, "hrms_access_removed")),
                    DataClass.GetParameter("@p_it_remarks", GetStringValue(clearanceData, "it_remarks")),

                    // Administration Department
                    DataClass.GetParameter("@p_id_card_returned", GetIntValue(clearanceData, "id_card_returned")),
                    DataClass.GetParameter("@p_access_card_returned", GetIntValue(clearanceData, "access_card_returned")),
                    DataClass.GetParameter("@p_office_keys_returned", GetIntValue(clearanceData, "office_keys_returned")),
                    DataClass.GetParameter("@p_parking_pass_returned", GetIntValue(clearanceData, "parking_pass_returned")),
                    DataClass.GetParameter("@p_administration_remarks", GetStringValue(clearanceData, "administration_remarks")),

                    // Finance Department
                    DataClass.GetParameter("@p_loan_recovery_completed", GetIntValue(clearanceData, "loan_recovery_completed")),
                    DataClass.GetParameter("@p_salary_advance_recovered", GetIntValue(clearanceData, "salary_advance_recovered")),
                    DataClass.GetParameter("@p_expense_claims_processed", GetIntValue(clearanceData, "expense_claims_processed")),
                    DataClass.GetParameter("@p_finance_remarks", GetStringValue(clearanceData, "finance_remarks")),

                    // Security Department
                    DataClass.GetParameter("@p_biometric_disabled", GetIntValue(clearanceData, "biometric_disabled")),
                    DataClass.GetParameter("@p_building_access_revoked", GetIntValue(clearanceData, "building_access_revoked")),
                    DataClass.GetParameter("@p_security_remarks", GetStringValue(clearanceData, "security_remarks")),

                    DataClass.GetParameter("@p_inserted_by", insertedBy),
                    DataClass.GetParameter("@p_is_active", 1)
                };

                using (var dr = DataClass.GetDataReaderFromSpWithParam(parameters, DBName, "SP_InsertExitClearance"))
                {
                    if (dr.Read())
                    {
                        response.Status = Convert.ToString(dr["Status"]);
                        response.Message = Convert.ToString(dr["Message"]);
                        response.ExitClearanceId = Convert.ToInt32(dr["exit_clearance_id"]);
                    }
                }
            }
            catch (Exception ex)
            {
                CommonBL errorlog = new CommonBL();
                errorlog.fnStoreErrorLog("ExitClearanceBL", "InsertExitClearance", "Exception Message: " + ex.Message + " StackTrace: " + ex.StackTrace, UserId);
                response.Message = "An error occurred while saving exit clearance: " + ex.Message;
            }

            return response;
        }

        private int GetIntValue(Dictionary<string, object> data, string key)
        {
            if (data.ContainsKey(key) && data[key] != null)
            {
                return Convert.ToInt32(data[key]);
            }
            return 0;
        }

        private string GetStringValue(Dictionary<string, object> data, string key)
        {
            if (data.ContainsKey(key) && data[key] != null)
            {
                return Convert.ToString(data[key]);
            }
            return string.Empty;
        }
    }
}
