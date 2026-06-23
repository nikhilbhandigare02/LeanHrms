using DataObject;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using System.Net.Mail;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Org.BouncyCastle.Pqc.Crypto.Hqc;


namespace ProcessModel
{
    public class UserDetailsBL
    {
        protected string UserId = null;
        private string DBName = ConfigurationManager.AppSettings["DBName"];
        private static string MySqlconnection = ConfigurationManager.ConnectionStrings["MysqlConnection"].ConnectionString;
        private static string Sqlconnection = ConfigurationManager.ConnectionStrings["Sqlconnection"] != null
            ? ConfigurationManager.ConnectionStrings["Sqlconnection"].ConnectionString
            : string.Empty;
        public int Getpage(int userId, string queryString)
        {
            try
            {
                using (MySqlConnection connection = new MySqlConnection(MySqlconnection))
                {
                    connection.Open();

                    using (MySqlCommand command = new MySqlCommand("sp_getuserIdwisepagaccess", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;

                        // Input parameters
                        command.Parameters.Add(new MySqlParameter("p_userid", MySqlDbType.Int32) { Value = userId });
                        command.Parameters.Add(new MySqlParameter("p_pagename", MySqlDbType.VarChar, 100) { Value = queryString });

                        // Output parameter
                        MySqlParameter parameterStatus = new MySqlParameter("p_status", MySqlDbType.Int32)
                        {
                            Direction = ParameterDirection.Output
                        };
                        command.Parameters.Add(parameterStatus);

                        // Execute
                        command.ExecuteNonQuery();

                        int status = Convert.ToInt32(parameterStatus.Value);

                        return status;
                    }
                }
            }
            catch (Exception ex)
            {
                CommonBL errorlog = new CommonBL();
                errorlog.fnStoreErrorLog("userDetailsBL", "Getpage", "Exception Message: " + ex.Message + " Strace=" + ex.StackTrace, UserId);

                return -1;
            }
        }

        public List<UserDetailsDO> SaveUserDetails(UserDetailsDO user)
        {
            List<UserDetailsDO> listdata = new List<UserDetailsDO>();
            List<MySqlParameter> mysqlParameters = new List<MySqlParameter>();

            try
            {
                // Keep the parameter sequence aligned with Sp_insertuser definition.
                mysqlParameters.Add(DataClass.GetParameter("p_type", "InsertUser"));
                mysqlParameters.Add(DataClass.GetParameter("p_Username", user.Username));
                mysqlParameters.Add(DataClass.GetParameter("p_user_fullname", user.user_fullname));
                mysqlParameters.Add(DataClass.GetParameter("p_user_mail_id", user.user_mail_id));
                mysqlParameters.Add(DataClass.GetParameter("p_password", user.password));
                mysqlParameters.Add(DataClass.GetParameter("p_employee_code", user.EmployeeCode));
                mysqlParameters.Add(DataClass.GetParameter("p_contact_detail", user.contact_detail));
                mysqlParameters.Add(DataClass.GetParameter("p_insertedby", user.Insertedby));
                mysqlParameters.Add(DataClass.GetParameter("p_user_type", user.user_type));
                mysqlParameters.Add(DataClass.GetParameter("p_designation_id", user.designation_id));
                mysqlParameters.Add(DataClass.GetParameter("p_company_id", user.company_id));
                mysqlParameters.Add(DataClass.GetParameter("p_ESIC_no", user.ESIC_no));
                mysqlParameters.Add(DataClass.GetParameter("p_PF_no", user.PF_no));
                mysqlParameters.Add(DataClass.GetParameter("p_department", user.department));
                mysqlParameters.Add(DataClass.GetParameter("p_branch", user.branch));
                mysqlParameters.Add(DataClass.GetParameter("p_division", user.division));
                mysqlParameters.Add(DataClass.GetParameter("p_date_of_joining", user.date_of_joining));
                mysqlParameters.Add(DataClass.GetParameter("p_probation_period_months", user.probation_period_months));
                mysqlParameters.Add(DataClass.GetParameter("p_reporting_manager", user.reporting_manager));
                mysqlParameters.Add(DataClass.GetParameter("p_employee_type", user.employee_type));

                // Registration insert is directed to secondary DB SP/table for current employee-only flow.
                listdata = SaveUserDetailsUsingSqlConnection(mysqlParameters);
            }
            catch (Exception ex)
            {
                CommonBL errorlog = new CommonBL();
                errorlog.fnStoreErrorLog(
                    "UserDetailsBL",
                    "SaveUserDetails",
                    "Exception Message: " + ex.Message + " | StackTrace=" + ex.StackTrace,
                    UserId
                );
            }

            return listdata;
        }

        public List<UserDetailsDO> SaveUserDetailsMainDb(UserDetailsDO user)
        {
            List<UserDetailsDO> listdata = new List<UserDetailsDO>();
            getDrtolist getDrtolistParam = new getDrtolist();
            List<MySqlParameter> mysqlParameters = new List<MySqlParameter>();

            try
            {
                mysqlParameters.Add(DataClass.GetParameter("p_type", "InsertUser"));
                mysqlParameters.Add(DataClass.GetParameter("p_Username", user.Username));
                mysqlParameters.Add(DataClass.GetParameter("p_user_fullname", user.user_fullname));
                mysqlParameters.Add(DataClass.GetParameter("p_user_mail_id", user.user_mail_id));
                mysqlParameters.Add(DataClass.GetParameter("p_password", user.password));
                mysqlParameters.Add(DataClass.GetParameter("p_employee_code", user.EmployeeCode));
                mysqlParameters.Add(DataClass.GetParameter("p_contact_detail", user.contact_detail));
                mysqlParameters.Add(DataClass.GetParameter("p_insertedby", user.Insertedby));
                mysqlParameters.Add(DataClass.GetParameter("p_user_type", user.user_type));
                mysqlParameters.Add(DataClass.GetParameter("p_designation_id", user.designation_id));
                mysqlParameters.Add(DataClass.GetParameter("p_company_id", user.company_id));
                mysqlParameters.Add(DataClass.GetParameter("p_ESIC_no", user.ESIC_no));
                mysqlParameters.Add(DataClass.GetParameter("p_PF_no", user.PF_no));
                mysqlParameters.Add(DataClass.GetParameter("p_department", user.department));
                mysqlParameters.Add(DataClass.GetParameter("p_branch", user.branch));
                mysqlParameters.Add(DataClass.GetParameter("p_division", user.division));
                mysqlParameters.Add(DataClass.GetParameter("p_date_of_joining", user.date_of_joining));
                mysqlParameters.Add(DataClass.GetParameter("p_probation_period_months", user.probation_period_months));
                mysqlParameters.Add(DataClass.GetParameter("p_reporting_manager", user.reporting_manager));
                mysqlParameters.Add(DataClass.GetParameter("p_employee_type", user.employee_type));

                listdata = getDrtolistParam.getdatafromreder<UserDetailsDO>(
                    DataClass.GetDataReaderFromSpWithParam(mysqlParameters, DBName, "Sp_insertuser")
                );
            }
            catch (Exception ex)
            {
                CommonBL errorlog = new CommonBL();
                errorlog.fnStoreErrorLog(
                    "UserDetailsBL",
                    "SaveUserDetailsMainDb",
                    "Exception Message: " + ex.Message + " | StackTrace=" + ex.StackTrace,
                    UserId
                );
            }

            if (listdata == null || listdata.Count == 0)
            {
                listdata = new List<UserDetailsDO>
                {
                    new UserDetailsDO
                    {
                        Status = "Failed",
                        Remarks = "User save did not return any response from database."
                    }
                };
            }

            return listdata;
        }

        private bool IsSuccessResponse(List<UserDetailsDO> response)
        {
            if (response == null || response.Count == 0)
            {
                return false;
            }

            string status = Convert.ToString(response[0].Status ?? string.Empty).Trim();
            return string.Equals(status, "Success", StringComparison.OrdinalIgnoreCase);
        }

        private List<UserDetailsDO> SaveUserDetailsUsingSqlConnection(List<MySqlParameter> mysqlParameters)
        {
            List<UserDetailsDO> listdata = new List<UserDetailsDO>();
            if (string.IsNullOrWhiteSpace(Sqlconnection))
            {
                return listdata;
            }

            try
            {
                string normalizedConnection = NormalizeMySqlConnectionString(Sqlconnection);
                using (MySqlConnection connection = new MySqlConnection(normalizedConnection))
                using (MySqlCommand command = new MySqlCommand("Sp_insertuser", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.CommandTimeout = 0;

                    foreach (var parameter in mysqlParameters)
                    {
                        command.Parameters.AddWithValue(parameter.ParameterName, parameter.Value ?? DBNull.Value);
                    }

                    connection.Open();
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            listdata.Add(new UserDetailsDO
                            {
                                Status = reader["Status"] != DBNull.Value ? reader["Status"].ToString() : "Failed",
                                Remarks = reader["Remarks"] != DBNull.Value ? reader["Remarks"].ToString() : string.Empty
                            });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                CommonBL errorlog = new CommonBL();
                errorlog.fnStoreErrorLog(
                    "UserDetailsBL",
                    "SaveUserDetailsUsingSqlConnection",
                    "Exception Message: " + ex.Message + " | StackTrace=" + ex.StackTrace,
                    UserId
                );
            }

            if (listdata == null || listdata.Count == 0)
            {
                listdata = new List<UserDetailsDO>
                {
                    new UserDetailsDO
                    {
                        Status = "Failed",
                        Remarks = "User save did not return any response from database."
                    }
                };
            }

            return listdata;
        }

        public List<UserDetailsDO> UpdateUserDetails(UserDetailsDO user)
        {
            List<UserDetailsDO> listdata = new List<UserDetailsDO>();
            getDrtolist getDrtolistParam = new getDrtolist();
            List<MySqlParameter> mysqlParameters = new List<MySqlParameter>();

            try
            {
                mysqlParameters.Add(DataClass.GetParameter("p_user_id", user.UserId));
                mysqlParameters.Add(DataClass.GetParameter("p_Username", user.Username));
                mysqlParameters.Add(DataClass.GetParameter("p_user_fullname", user.user_fullname));
                mysqlParameters.Add(DataClass.GetParameter("p_user_mail_id", user.user_mail_id));
                mysqlParameters.Add(DataClass.GetParameter("p_contact_detail", user.contact_detail));
                mysqlParameters.Add(DataClass.GetParameter("p_updatedby", user.UserId));
                mysqlParameters.Add(DataClass.GetParameter("p_designation_id", user.designation_id));
                mysqlParameters.Add(DataClass.GetParameter("p_employee_code", user.EmployeeCode));
                 mysqlParameters.Add(DataClass.GetParameter("@p_company_id", user.company_id));
                mysqlParameters.Add(DataClass.GetParameter("@p_ESIC_no", user.ESIC_no));
                mysqlParameters.Add(DataClass.GetParameter("@p_PF_no", user.PF_no));
                mysqlParameters.Add(DataClass.GetParameter("@p_department", user.department));
                mysqlParameters.Add(DataClass.GetParameter("@p_branch", user.branch));
                mysqlParameters.Add(DataClass.GetParameter("@p_division", user.division));
                mysqlParameters.Add(DataClass.GetParameter("@p_date_of_joining", user.date_of_joining));
                mysqlParameters.Add(DataClass.GetParameter("@p_probation_period_months", user.probation_period_months));
                mysqlParameters.Add(DataClass.GetParameter("@p_reporting_manager", user.reporting_manager));
                mysqlParameters.Add(DataClass.GetParameter("@p_employee_type", user.employee_type));
                mysqlParameters.Add(DataClass.GetParameter("p_type", "UpdateUser"));

                var primaryResult = getDrtolistParam.getdatafromreder<UserDetailsDO>(
                    DataClass.GetDataReaderFromSpWithParam(mysqlParameters, DBName, "Sp_updateuser")
                );
                if (primaryResult != null && primaryResult.Count > 0)
                {
                    listdata = primaryResult;
                }

                List<UserDetailsDO> secondaryResult = new List<UserDetailsDO>();
                try
                {
                    secondaryResult = UpdateUserDetailsUsingSqlConnection(mysqlParameters);
                }
                catch
                {
                    secondaryResult = new List<UserDetailsDO>();
                }
                bool primarySuccess = IsSuccessResponse(primaryResult);
                bool secondarySuccess = IsSuccessResponse(secondaryResult);

                if (primarySuccess && secondarySuccess)
                {
                    listdata = new List<UserDetailsDO>
                    {
                        new UserDetailsDO
                        {
                            Status = "Success",
                            Remarks = "User updated successfully in both databases."
                        }
                    };
                }
                else if (primarySuccess && !secondarySuccess)
                {
                    listdata = new List<UserDetailsDO>
                    {
                        new UserDetailsDO
                        {
                            Status = "Success",
                            Remarks = "User updated in primary database. Secondary database response not received."
                        }
                    };
                }
                else
                {
                    listdata = primaryResult;
                }
            }
            catch (Exception ex)
            {
                CommonBL errorlog = new CommonBL();
                errorlog.fnStoreErrorLog(
                    "UserDetailsBL",
                    "UpdateUserDetails",
                    "Exception Message: " + ex.Message + " | StackTrace=" + ex.StackTrace,
                    UserId
                );
                if (listdata == null || listdata.Count == 0)
                {
                    listdata = new List<UserDetailsDO>
                    {
                        new UserDetailsDO
                        {
                            Status = "Failed",
                            Remarks = "User update failed due to exception."
                        }
                    };
                }
            }

            if (listdata == null || listdata.Count == 0)
            {
                listdata = new List<UserDetailsDO>
                {
                    new UserDetailsDO
                    {
                        Status = "Failed",
                        Remarks = "Update response not received from database."
                    }
                };
            }

            return listdata;
        }

        public List<UserDetailsDO> UpdateUserDetailsMainDb(UserDetailsDO user)
        {
            List<UserDetailsDO> listdata = new List<UserDetailsDO>();
            getDrtolist getDrtolistParam = new getDrtolist();
            List<MySqlParameter> mysqlParameters = new List<MySqlParameter>();

            try
            {
                mysqlParameters.Add(DataClass.GetParameter("p_user_id", user.UserId));
                mysqlParameters.Add(DataClass.GetParameter("p_Username", user.Username));
                mysqlParameters.Add(DataClass.GetParameter("p_user_fullname", user.user_fullname));
                mysqlParameters.Add(DataClass.GetParameter("p_user_mail_id", user.user_mail_id));
                mysqlParameters.Add(DataClass.GetParameter("p_contact_detail", user.contact_detail));
                mysqlParameters.Add(DataClass.GetParameter("p_updatedby", user.UserId));
                mysqlParameters.Add(DataClass.GetParameter("p_designation_id", user.designation_id));
                mysqlParameters.Add(DataClass.GetParameter("p_employee_code", user.EmployeeCode));
                mysqlParameters.Add(DataClass.GetParameter("@p_company_id", user.company_id));
                mysqlParameters.Add(DataClass.GetParameter("@p_ESIC_no", user.ESIC_no));
                mysqlParameters.Add(DataClass.GetParameter("@p_PF_no", user.PF_no));
                mysqlParameters.Add(DataClass.GetParameter("@p_department", user.department));
                mysqlParameters.Add(DataClass.GetParameter("@p_branch", user.branch));
                mysqlParameters.Add(DataClass.GetParameter("@p_division", user.division));
                mysqlParameters.Add(DataClass.GetParameter("@p_date_of_joining", user.date_of_joining));
                mysqlParameters.Add(DataClass.GetParameter("@p_probation_period_months", user.probation_period_months));
                mysqlParameters.Add(DataClass.GetParameter("@p_reporting_manager", user.reporting_manager));
                mysqlParameters.Add(DataClass.GetParameter("@p_employee_type", user.employee_type));
                mysqlParameters.Add(DataClass.GetParameter("p_type", "UpdateUser"));

                listdata = getDrtolistParam.getdatafromreder<UserDetailsDO>(
                    DataClass.GetDataReaderFromSpWithParam(mysqlParameters, DBName, "Sp_updateuser")
                );
            }
            catch (Exception ex)
            {
                CommonBL errorlog = new CommonBL();
                errorlog.fnStoreErrorLog(
                    "UserDetailsBL",
                    "UpdateUserDetailsMainDb",
                    "Exception Message: " + ex.Message + " | StackTrace=" + ex.StackTrace,
                    UserId
                );
            }

            if (listdata == null || listdata.Count == 0)
            {
                listdata = new List<UserDetailsDO>
                {
                    new UserDetailsDO
                    {
                        Status = "Failed",
                        Remarks = "Update response not received from primary database."
                    }
                };
            }

            return listdata;
        }

        public List<UserDetailsDO> UpdateUserDetailsSecondary(UserDetailsDO user)
        {
            List<MySqlParameter> mysqlParameters = new List<MySqlParameter>();

            mysqlParameters.Add(DataClass.GetParameter("p_user_id", user.UserId));
            mysqlParameters.Add(DataClass.GetParameter("p_Username", user.Username));
            mysqlParameters.Add(DataClass.GetParameter("p_user_fullname", user.user_fullname));
            mysqlParameters.Add(DataClass.GetParameter("p_user_mail_id", user.user_mail_id));
            mysqlParameters.Add(DataClass.GetParameter("p_contact_detail", user.contact_detail));
            mysqlParameters.Add(DataClass.GetParameter("p_updatedby", user.UserId));
            mysqlParameters.Add(DataClass.GetParameter("p_designation_id", user.designation_id));
            mysqlParameters.Add(DataClass.GetParameter("p_employee_code", user.EmployeeCode));
            mysqlParameters.Add(DataClass.GetParameter("@p_company_id", user.company_id));
            mysqlParameters.Add(DataClass.GetParameter("@p_ESIC_no", user.ESIC_no));
            mysqlParameters.Add(DataClass.GetParameter("@p_PF_no", user.PF_no));
            mysqlParameters.Add(DataClass.GetParameter("@p_department", user.department));
            mysqlParameters.Add(DataClass.GetParameter("@p_branch", user.branch));
            mysqlParameters.Add(DataClass.GetParameter("@p_division", user.division));
            mysqlParameters.Add(DataClass.GetParameter("@p_date_of_joining", user.date_of_joining));
            mysqlParameters.Add(DataClass.GetParameter("@p_probation_period_months", user.probation_period_months));
            mysqlParameters.Add(DataClass.GetParameter("@p_reporting_manager", user.reporting_manager));
            mysqlParameters.Add(DataClass.GetParameter("@p_employee_type", user.employee_type));
            mysqlParameters.Add(DataClass.GetParameter("p_type", "UpdateUser"));

            List<UserDetailsDO> secondaryResult = UpdateUserDetailsUsingSqlConnection(mysqlParameters);
            if (secondaryResult == null || secondaryResult.Count == 0)
            {
                secondaryResult = new List<UserDetailsDO>
                {
                    new UserDetailsDO
                    {
                        Status = "Failed",
                        Remarks = "Update response not received from secondary database."
                    }
                };
            }

            return secondaryResult;
        }

        public List<UserDetailsDO> UpdateEmployeeProfileCore(UserDetailsDO user)
        {
            List<UserDetailsDO> listdata = new List<UserDetailsDO>();

            if (user == null)
            {
                return new List<UserDetailsDO>
                {
                    new UserDetailsDO
                    {
                        Status = "Failed",
                        Remarks = "Employee update data is missing."
                    }
                };
            }

            if (string.IsNullOrWhiteSpace(Sqlconnection))
            {
                return new List<UserDetailsDO>
                {
                    new UserDetailsDO
                    {
                        Status = "Failed",
                        Remarks = "Secondary database connection is missing."
                    }
                };
            }

            try
            {
                string normalizedConnection = NormalizeMySqlConnectionString(Sqlconnection);
                using (MySqlConnection connection = new MySqlConnection(normalizedConnection))
                using (MySqlCommand command = new MySqlCommand("hrms_employee_profile_update_core", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.CommandTimeout = 0;

                    command.Parameters.AddWithValue("@p_user_id", user.UserId);
                    command.Parameters.AddWithValue("@p_emp_code", ToDbTextValue(user.EmployeeCode));
                    command.Parameters.AddWithValue("@p_username", ToDbTextValue(user.Username));
                    command.Parameters.AddWithValue("@p_fullname", ToDbTextValue(user.user_fullname));
                    command.Parameters.AddWithValue("@p_email", ToDbTextValue(user.user_mail_id));
                    command.Parameters.AddWithValue("@p_contact", ToDbTextValue(user.contact_detail));
                    command.Parameters.AddWithValue("@p_display_name", ToDbTextValue(user.DisplayName));
                    command.Parameters.AddWithValue("@p_gender", ToNullableIntValue(user.Gender));
                    command.Parameters.AddWithValue("@p_dob", ToNullableDateValue(user.DateOfBirth));
                    command.Parameters.AddWithValue("@p_marital_status", ToNullableIntValue(user.MaritalStatus));
                    command.Parameters.AddWithValue("@p_blood_group", ToNullableIntValue(user.BloodGroup));
                    command.Parameters.AddWithValue("@p_nationality", ToNullableIntValue(user.Nationality));
                    command.Parameters.AddWithValue("@p_aadhaar_number", ToDbTextValue(user.AadhaarNumber));
                    command.Parameters.AddWithValue("@p_pan_number", ToDbTextValue(user.PanNumber));
                    command.Parameters.AddWithValue("@p_passport_number", ToDbTextValue(user.PassportNumber));
                    command.Parameters.AddWithValue("@p_passport_expiry_date", ToNullableDateValue(user.PassportExpiryDate));
                    command.Parameters.AddWithValue("@p_employee_photo", ToDbTextValue(user.EmployeePhoto));

                    command.Parameters.AddWithValue("@p_employment_type", ToNullableIntValue(user.employee_type));
                    command.Parameters.AddWithValue("@p_employee_category", ResolveEmployeeCategoryValue(user));
                    command.Parameters.AddWithValue("@p_joining_date", user.date_of_joining == DateTime.MinValue ? (object)DBNull.Value : user.date_of_joining.Date);
                    command.Parameters.AddWithValue("@p_retirement_date", DBNull.Value);
                    command.Parameters.AddWithValue("@p_probation_months", user.probation_period_months);
                    command.Parameters.AddWithValue("@p_employee_status", ToNullableIntValue(user.EmployeeStatus));
                    command.Parameters.AddWithValue("@p_notice_period", user.NoticePeriod);
                    command.Parameters.AddWithValue("@p_exit_date", ToNullableDateValue(user.ExitDate));
                    command.Parameters.AddWithValue("@p_separation_reason", ToNullableIntValue(user.SeparationReason));

                    command.Parameters.AddWithValue("@p_company", user.company_id);
                    command.Parameters.AddWithValue("@p_department", ResolveDepartmentValue(user));
                    command.Parameters.AddWithValue("@p_branch_office", ToDbTextValue(user.branch));
                    command.Parameters.AddWithValue("@p_location", ToDbTextValue(user.Location));
                    command.Parameters.AddWithValue("@p_reporting_manager_id", ToNullableIntValue(user.reporting_manager));
                    command.Parameters.AddWithValue("@p_functional_manager_id", ResolveUserIdByFullName(user.FunctionalManagerName));
                    command.Parameters.AddWithValue("@p_hod_id", ResolveUserIdByFullName(user.HodName));
                    command.Parameters.AddWithValue("@p_designation", user.designation_id);
                    command.Parameters.AddWithValue("@p_employee_level", ToNullableIntValue(user.EmployeeLevel));

                    command.Parameters.AddWithValue("@p_attendance_type", ToNullableIntValue(user.AttendanceType));
                    command.Parameters.AddWithValue("@p_weekly_off", ToNullableIntValue(user.WeeklyOff));
                    command.Parameters.AddWithValue("@p_working_hours", ToNullableTimeValue(user.WorkingHours));
                    command.Parameters.AddWithValue("@p_attendance_policy", ToNullableIntValue(user.AttendancePolicy));
                    command.Parameters.AddWithValue("@p_punching_device_id", ToDbTextValue(user.PunchingDeviceId));
                    command.Parameters.AddWithValue("@p_biometric_id", ToDbTextValue(user.BiometricId));
                    command.Parameters.AddWithValue("@p_overtime_eligible", ResolveYesNoLookupValue(user.OvertimeEligible));
                    command.Parameters.AddWithValue("@p_overtime_rate", ToNullableDecimalValue(user.OvertimeRate));
                    command.Parameters.AddWithValue("@p_work_location", ToNullableIntValue(user.WorkLocation));
                    command.Parameters.AddWithValue("@p_remote_work_allowed", 0);

                    command.Parameters.AddWithValue("@p_alternate_mobile_number", ToDbTextValue(user.AlternateMobileNumber));
                    command.Parameters.AddWithValue("@p_personal_email", ToDbTextValue(user.PersonalEmail));
                    command.Parameters.AddWithValue("@p_permanent_house_number", ToDbTextValue(user.PermanentHouseNumber));
                    command.Parameters.AddWithValue("@p_permanent_building_name", ToDbTextValue(user.PermanentBuildingName));
                    command.Parameters.AddWithValue("@p_permanent_street", ToDbTextValue(user.PermanentStreet));
                    command.Parameters.AddWithValue("@p_permanent_area", ToDbTextValue(user.PermanentArea));
                    command.Parameters.AddWithValue("@p_permanent_landmark", ToDbTextValue(user.PermanentLandmark));
                    command.Parameters.AddWithValue("@p_permanent_city", ResolveExistingContactIntValue(user.UserId, "permanent_city", user.PermanentCity));
                    command.Parameters.AddWithValue("@p_permanent_district", ResolveExistingContactIntValue(user.UserId, "permanent_district", user.PermanentDistrict));
                    command.Parameters.AddWithValue("@p_permanent_state", ResolveExistingContactIntValue(user.UserId, "permanent_state", user.PermanentState));
                    command.Parameters.AddWithValue("@p_permanent_country", ResolveExistingContactIntValue(user.UserId, "permanent_country", user.PermanentCountry));
                    command.Parameters.AddWithValue("@p_permanent_pin_code", ToDbTextValue(user.PermanentPinCode));
                    command.Parameters.AddWithValue("@p_same_as_permanent", ToDbTinyIntValue(user.SameAsPermanent));
                    command.Parameters.AddWithValue("@p_current_house_number", ToDbTextValue(user.CurrentHouseNumber));
                    command.Parameters.AddWithValue("@p_current_building_name", ToDbTextValue(user.CurrentBuildingName));
                    command.Parameters.AddWithValue("@p_current_street", ToDbTextValue(user.CurrentStreet));
                    command.Parameters.AddWithValue("@p_current_area", ToDbTextValue(user.CurrentArea));
                    command.Parameters.AddWithValue("@p_current_landmark", ToDbTextValue(user.CurrentLandmark));
                    command.Parameters.AddWithValue("@p_current_city", ResolveExistingContactIntValue(user.UserId, "current_City", user.CurrentCity));
                    command.Parameters.AddWithValue("@p_current_district", ResolveExistingContactIntValue(user.UserId, "current_district", user.CurrentDistrict));
                    command.Parameters.AddWithValue("@p_current_state", ResolveExistingContactIntValue(user.UserId, "current_state", user.CurrentState));
                    command.Parameters.AddWithValue("@p_current_country", ResolveExistingContactIntValue(user.UserId, "current_CoUNTRY", user.CurrentCountry));
                    command.Parameters.AddWithValue("@p_current_pin_code", ToDbTextValue(user.CurrentPinCode));
                    command.Parameters.AddWithValue("@p_emergency_contact_name", ToDbTextValue(user.EmergencyContactName));
                    command.Parameters.AddWithValue("@p_emergency_contact_number", ToDbTextValue(user.EmergencyContactNumber));
                    command.Parameters.AddWithValue("@p_emergency_contact_relationship", ToDbTextValue(user.EmergencyContactRelationship));

                    command.Parameters.AddWithValue("@p_bank_name", ToDbTextValue(user.BankName));
                    command.Parameters.AddWithValue("@p_branch_name", ToDbTextValue(user.BankBranchName));
                    command.Parameters.AddWithValue("@p_account_holder_name", ToDbTextValue(user.AccountHolderName));
                    command.Parameters.AddWithValue("@p_account_number", ToDbTextValue(user.AccountNumber));
                    command.Parameters.AddWithValue("@p_confirm_account_number", ToDbTextValue(user.ConfirmAccountNumber));
                    command.Parameters.AddWithValue("@p_ifsc_code", ToDbTextValue(user.IfscCode));
                    command.Parameters.AddWithValue("@p_micr_code", GetExistingBankTextValue(user.UserId, "micr_code"));
                    command.Parameters.AddWithValue("@p_account_type", ResolveAccountTypeValue(user));
                    command.Parameters.AddWithValue("@p_salary_account_flag", ResolveSalaryAccountFlagValue(user));
                    command.Parameters.AddWithValue("@p_updated_by", user.Updatedby.HasValue && user.Updatedby.Value > 0 ? user.Updatedby.Value : user.UserId);

                    connection.Open();
                    using (MySqlDataReader reader = command.ExecuteReader())
                    {
                        do
                        {
                            while (reader.Read())
                            {
                                listdata.Add(new UserDetailsDO
                                {
                                    Status = ReadStringSafe(reader, "status", "Status"),
                                    Remarks = ReadStringSafe(reader, "message", "Message", "Remarks")
                                });
                            }
                        } while (reader.NextResult());
                    }
                }
            }
            catch (Exception ex)
            {
                CommonBL errorlog = new CommonBL();
                errorlog.fnStoreErrorLog("UserDetailsBL", "UpdateEmployeeProfileCore", "Exception Message: " + ex.Message + " | StackTrace=" + ex.StackTrace, UserId);
            }

            if (listdata == null || listdata.Count == 0)
            {
                listdata = new List<UserDetailsDO>
                {
                    new UserDetailsDO
                    {
                        Status = "Failed",
                        Remarks = "Update response not received from hrms_employee_profile_update_core."
                    }
                };
            }

            return listdata;
        }

        public List<UserDetailsDO> UpdateEmployeeOfficialContact(int userId, string officialEmail, string officialMobile, int updatedBy)
        {
            List<UserDetailsDO> listdata = new List<UserDetailsDO>();

            if (string.IsNullOrWhiteSpace(Sqlconnection))
            {
                return new List<UserDetailsDO>
                {
                    new UserDetailsDO
                    {
                        Status = "Failed",
                        Remarks = "Secondary database connection is missing."
                    }
                };
            }

            try
            {
                string normalizedConnection = NormalizeMySqlConnectionString(Sqlconnection);
                using (MySqlConnection connection = new MySqlConnection(normalizedConnection))
                using (MySqlCommand command = new MySqlCommand("SP_UpdateEmployeeOfficialContact", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.CommandTimeout = 0;

                    command.Parameters.AddWithValue("@p_user_id", userId);
                    command.Parameters.AddWithValue("@p_email", ToDbTextValue(officialEmail));
                    command.Parameters.AddWithValue("@p_contact", ToDbTextValue(officialMobile));
                    command.Parameters.AddWithValue("@p_updated_by", updatedBy > 0 ? (object)updatedBy : userId);

                    connection.Open();
                    using (MySqlDataReader reader = command.ExecuteReader())
                    {
                        do
                        {
                            while (reader.Read())
                            {
                                listdata.Add(new UserDetailsDO
                                {
                                    Status = ReadStringSafe(reader, "status", "Status"),
                                    Remarks = ReadStringSafe(reader, "message", "Message", "Remarks"),
                                    EmailChanged = ReadIntSafe(reader, "EmailChanged", "email_changed"),
                                    MobileChanged = ReadIntSafe(reader, "MobileChanged", "mobile_changed"),
                                    SendCredentialsMail = ReadIntSafe(reader, "SendCredentialsMail", "send_credentials_mail")
                                });
                            }
                        } while (reader.NextResult());
                    }
                }
            }
            catch (Exception ex)
            {
                CommonBL errorlog = new CommonBL();
                errorlog.fnStoreErrorLog("UserDetailsBL", "UpdateEmployeeOfficialContact", "Exception Message: " + ex.Message + " | StackTrace=" + ex.StackTrace, UserId);
            }

            if (listdata == null || listdata.Count == 0)
            {
                listdata = new List<UserDetailsDO>
                {
                    new UserDetailsDO
                    {
                        Status = "Failed",
                        Remarks = "Update response not received from SP_UpdateEmployeeOfficialContact."
                    }
                };
            }

            return listdata;
        }

        private List<UserDetailsDO> UpdateUserDetailsUsingSqlConnection(List<MySqlParameter> mysqlParameters)
        {
            if (string.IsNullOrWhiteSpace(Sqlconnection))
            {
                return new List<UserDetailsDO>();
            }

            // Secondary DB can have a different user_id for the same employee.
            // Resolve secondary user_id from employee code and use it for update.
            try
            {
                string employeeCode = Convert.ToString(
                    mysqlParameters.FirstOrDefault(p =>
                        string.Equals(p.ParameterName, "p_employee_code", StringComparison.OrdinalIgnoreCase) ||
                        string.Equals(p.ParameterName, "@p_employee_code", StringComparison.OrdinalIgnoreCase)
                    )?.Value
                );
                if (!string.IsNullOrWhiteSpace(employeeCode))
                {
                    int secondaryUserId = GetSecondaryUserIdByEmployeeCode(employeeCode);
                    if (secondaryUserId > 0)
                    {
                        var userIdParam = mysqlParameters.FirstOrDefault(p =>
                            string.Equals(p.ParameterName, "p_user_id", StringComparison.OrdinalIgnoreCase) ||
                            string.Equals(p.ParameterName, "@p_user_id", StringComparison.OrdinalIgnoreCase)
                        );
                        if (userIdParam != null)
                        {
                            userIdParam.Value = secondaryUserId;
                        }
                    }
                }
            }
            catch
            {
                // Keep fallback execution; update may still succeed with incoming user_id.
            }

            try
            {
                // Try MySQL-compatible execution first.
                return UpdateUserDetailsUsingSecondaryMySql(mysqlParameters);
            }
            catch
            {
                // Fallback to SQL Server execution for Sqlconnection strings.
                return UpdateUserDetailsUsingSecondarySql(mysqlParameters);
            }
        }

        private int GetSecondaryUserIdByEmployeeCode(string employeeCode)
        {
            try
            {
                List<UserDetailsDO> secondaryUsers = GetAllUsersFromConnection(Sqlconnection, true);
                UserDetailsDO match = secondaryUsers
                    .FirstOrDefault(u => string.Equals(
                        (u.EmployeeCode ?? string.Empty).Trim(),
                        (employeeCode ?? string.Empty).Trim(),
                        StringComparison.OrdinalIgnoreCase));
                if (match != null && match.UserId > 0)
                {
                    return match.UserId;
                }
            }
            catch
            {
            }
            return 0;
        }

        private object ToDbTextValue(string value)
        {
            return string.IsNullOrWhiteSpace(value) ? (object)DBNull.Value : value.Trim();
        }

        private object ToNullableIntValue(string value)
        {
            int parsedValue;
            return int.TryParse(Convert.ToString(value ?? string.Empty).Trim(), out parsedValue)
                ? (object)parsedValue
                : DBNull.Value;
        }

        private object ToNullableDateValue(DateTime? value)
        {
            return value.HasValue ? (object)value.Value : DBNull.Value;
        }

        private object ToNullableDecimalValue(string value)
        {
            decimal parsedValue;
            return decimal.TryParse(Convert.ToString(value ?? string.Empty).Trim(), out parsedValue)
                ? (object)parsedValue
                : DBNull.Value;
        }

        private object ToNullableTimeValue(string value)
        {
            TimeSpan parsedValue;
            string normalized = Convert.ToString(value ?? string.Empty).Trim();
            if (string.IsNullOrWhiteSpace(normalized))
            {
                return DBNull.Value;
            }

            if (TimeSpan.TryParse(normalized, out parsedValue))
            {
                return parsedValue;
            }

            DateTime parsedDateTime;
            if (DateTime.TryParse(normalized, out parsedDateTime))
            {
                return parsedDateTime.TimeOfDay;
            }

            return DBNull.Value;
        }

        private int ToDbTinyIntValue(string value)
        {
            string normalized = Convert.ToString(value ?? string.Empty).Trim().ToLowerInvariant();
            return normalized == "1" || normalized == "true" || normalized == "yes" || normalized == "y" ? 1 : 0;
        }

        private string ReadStringSafe(IDataRecord reader, params string[] columnNames)
        {
            foreach (string columnName in columnNames)
            {
                try
                {
                    int ordinal = reader.GetOrdinal(columnName);
                    if (!reader.IsDBNull(ordinal))
                    {
                        return Convert.ToString(reader.GetValue(ordinal));
                    }
                }
                catch
                {
                }
            }

            return string.Empty;
        }

        private int ReadIntSafe(IDataRecord reader, params string[] columnNames)
        {
            foreach (string columnName in columnNames)
            {
                try
                {
                    int ordinal = reader.GetOrdinal(columnName);
                    if (!reader.IsDBNull(ordinal))
                    {
                        int parsedValue;
                        return int.TryParse(Convert.ToString(reader.GetValue(ordinal)), out parsedValue) ? parsedValue : 0;
                    }
                }
                catch
                {
                }
            }

            return 0;
        }

        private object ResolveDepartmentValue(UserDetailsDO user)
        {
            object parsedValue = ToNullableIntValue(user.department);
            if (parsedValue != DBNull.Value)
            {
                return parsedValue;
            }

            int lookupId = ResolveLookupIdByText("Department", user.department);
            if (lookupId > 0)
            {
                return lookupId;
            }

            int existingValue = GetExistingIntValue("userm", "department", "user_id", user.UserId);
            return existingValue > 0 ? (object)existingValue : DBNull.Value;
        }

        private object ResolveEmployeeCategoryValue(UserDetailsDO user)
        {
            int existingValue = GetExistingIntValue("userm", "employee_type", "user_id", user.UserId);
            return existingValue > 0 ? (object)existingValue : DBNull.Value;
        }

        private object ResolveAccountTypeValue(UserDetailsDO user)
        {
            object parsedValue = ToNullableIntValue(user.AccountType);
            if (parsedValue != DBNull.Value)
            {
                return parsedValue;
            }

            int lookupId = ResolveLookupIdByText("Account Type", user.AccountType);
            if (lookupId > 0)
            {
                return lookupId;
            }

            int existingValue = GetExistingIntValue("employee_bank_details", "account_type", "user_id", user.UserId);
            return existingValue > 0 ? (object)existingValue : DBNull.Value;
        }

        private object ResolveSalaryAccountFlagValue(UserDetailsDO user)
        {
            object parsedValue = ToNullableIntValue(user.SalaryAccountFlagText);
            if (parsedValue != DBNull.Value)
            {
                return parsedValue;
            }

            int lookupId = ResolveLookupIdByText("YesNo", user.SalaryAccountFlagText);
            if (lookupId > 0)
            {
                return lookupId;
            }

            int existingValue = GetExistingIntValue("employee_bank_details", "salary_account_flag", "user_id", user.UserId);
            return existingValue > 0 ? (object)existingValue : DBNull.Value;
        }

        private object ResolveYesNoLookupValue(string value)
        {
            string normalized = Convert.ToString(value ?? string.Empty).Trim().ToLowerInvariant();
            if (string.IsNullOrWhiteSpace(normalized))
            {
                return DBNull.Value;
            }

            if (normalized == "60" || normalized == "61")
            {
                return Convert.ToInt32(normalized);
            }

            if (normalized == "1" || normalized == "true" || normalized == "yes" || normalized == "y")
            {
                return 60;
            }

            if (normalized == "0" || normalized == "false" || normalized == "no" || normalized == "n")
            {
                return 61;
            }

            int lookupId = ResolveLookupIdByText("YesNo", value);
            return lookupId > 0 ? (object)lookupId : DBNull.Value;
        }

        private object ResolveExistingContactIntValue(int userId, string columnName, string inputValue)
        {
            object parsedValue = ToNullableIntValue(inputValue);
            if (parsedValue != DBNull.Value)
            {
                return parsedValue;
            }

            int existingValue = GetExistingIntValue("employee_contact_information", columnName, "user_id", userId);
            return existingValue > 0 ? (object)existingValue : DBNull.Value;
        }

        private int ResolveLookupIdByText(string lookupType, string text)
        {
            if (string.IsNullOrWhiteSpace(Sqlconnection) || string.IsNullOrWhiteSpace(lookupType) || string.IsNullOrWhiteSpace(text))
            {
                return 0;
            }

            try
            {
                string normalizedConnection = NormalizeMySqlConnectionString(Sqlconnection);
                using (MySqlConnection con = new MySqlConnection(normalizedConnection))
                using (MySqlCommand cmd = new MySqlCommand(@"SELECT lookupid 
                                                             FROM lean_mngt.lookupm 
                                                             WHERE isactive = 1
                                                               AND lookupType = @lookupType
                                                               AND (TRIM(lookupText) = TRIM(@text) OR TRIM(lookupvalue) = TRIM(@text))
                                                             LIMIT 1;", con))
                {
                    cmd.Parameters.AddWithValue("@lookupType", lookupType);
                    cmd.Parameters.AddWithValue("@text", text);
                    con.Open();
                    object value = cmd.ExecuteScalar();
                    int parsedValue;
                    return value != null && value != DBNull.Value && int.TryParse(Convert.ToString(value), out parsedValue)
                        ? parsedValue
                        : 0;
                }
            }
            catch
            {
                return 0;
            }
        }

        private int ResolveUserIdByFullName(string fullName)
        {
            if (string.IsNullOrWhiteSpace(Sqlconnection) || string.IsNullOrWhiteSpace(fullName))
            {
                return 0;
            }

            try
            {
                string normalizedConnection = NormalizeMySqlConnectionString(Sqlconnection);
                using (MySqlConnection con = new MySqlConnection(normalizedConnection))
                using (MySqlCommand cmd = new MySqlCommand(@"SELECT user_id 
                                                             FROM lean_mngt.userm
                                                             WHERE is_active = 1
                                                               AND TRIM(user_fullname) = TRIM(@fullName)
                                                             LIMIT 1;", con))
                {
                    cmd.Parameters.AddWithValue("@fullName", fullName);
                    con.Open();
                    object value = cmd.ExecuteScalar();
                    int parsedValue;
                    return value != null && value != DBNull.Value && int.TryParse(Convert.ToString(value), out parsedValue)
                        ? parsedValue
                        : 0;
                }
            }
            catch
            {
                return 0;
            }
        }

        private int GetExistingIntValue(string tableName, string columnName, string keyColumnName, int keyValue)
        {
            if (string.IsNullOrWhiteSpace(Sqlconnection) || keyValue <= 0)
            {
                return 0;
            }

            try
            {
                string normalizedConnection = NormalizeMySqlConnectionString(Sqlconnection);
                using (MySqlConnection con = new MySqlConnection(normalizedConnection))
                using (MySqlCommand cmd = new MySqlCommand(
                    "SELECT " + columnName + " FROM lean_mngt." + tableName + " WHERE " + keyColumnName + " = @keyValue LIMIT 1;", con))
                {
                    cmd.Parameters.AddWithValue("@keyValue", keyValue);
                    con.Open();
                    object value = cmd.ExecuteScalar();
                    int parsedValue;
                    return value != null && value != DBNull.Value && int.TryParse(Convert.ToString(value), out parsedValue)
                        ? parsedValue
                        : 0;
                }
            }
            catch
            {
                return 0;
            }
        }

        private string GetExistingBankTextValue(int userId, string columnName)
        {
            if (string.IsNullOrWhiteSpace(Sqlconnection) || userId <= 0)
            {
                return string.Empty;
            }

            try
            {
                string normalizedConnection = NormalizeMySqlConnectionString(Sqlconnection);
                using (MySqlConnection con = new MySqlConnection(normalizedConnection))
                using (MySqlCommand cmd = new MySqlCommand(
                    "SELECT " + columnName + " FROM lean_mngt.employee_bank_details WHERE user_id = @keyValue AND is_active = 1 LIMIT 1;", con))
                {
                    cmd.Parameters.AddWithValue("@keyValue", userId);
                    con.Open();
                    object value = cmd.ExecuteScalar();
                    return value == null || value == DBNull.Value ? string.Empty : Convert.ToString(value);
                }
            }
            catch
            {
                return string.Empty;
            }
        }

        private List<UserDetailsDO> UpdateUserDetailsUsingSecondaryMySql(List<MySqlParameter> mysqlParameters)
        {
            List<UserDetailsDO> listdata = new List<UserDetailsDO>();
            string normalizedConnection = NormalizeMySqlConnectionString(Sqlconnection);

            using (MySqlConnection connection = new MySqlConnection(normalizedConnection))
            using (MySqlCommand command = new MySqlCommand("Sp_updateuser", connection))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.CommandTimeout = 0;

                foreach (var parameter in mysqlParameters)
                {
                    string pName = parameter.ParameterName;
                    if (string.Equals(pName, "p_employee_code", StringComparison.OrdinalIgnoreCase) ||
                        string.Equals(pName, "@p_employee_code", StringComparison.OrdinalIgnoreCase))
                    {
                        pName = "p_empcode";
                    }
                    command.Parameters.AddWithValue(pName, parameter.Value ?? DBNull.Value);
                }

                connection.Open();
                using (var reader = command.ExecuteReader())
                {
                    bool hasAnyRow = false;
                    while (reader.Read())
                    {
                        hasAnyRow = true;
                        listdata.Add(new UserDetailsDO
                        {
                            Status = reader["Status"] != DBNull.Value ? reader["Status"].ToString() : "Failed",
                            Remarks = reader["Remarks"] != DBNull.Value ? reader["Remarks"].ToString() : string.Empty
                        });
                    }

                    if (!hasAnyRow)
                    {
                        // Some SP variants don't return Status/Remarks; treat successful execution as success.
                        listdata.Add(new UserDetailsDO
                        {
                            Status = "Success",
                            Remarks = "Updated in secondary database."
                        });
                    }
                }
            }

            if (listdata == null || listdata.Count == 0)
            {
                listdata = new List<UserDetailsDO>
                {
                    new UserDetailsDO
                    {
                        Status = "Failed",
                        Remarks = "User update did not return any response from secondary database."
                    }
                };
            }

            return listdata;
        }

        private List<UserDetailsDO> UpdateUserDetailsUsingSecondarySql(List<MySqlParameter> mysqlParameters)
        {
            List<UserDetailsDO> listdata = new List<UserDetailsDO>();

            using (SqlConnection connection = new SqlConnection(Sqlconnection))
            using (SqlCommand command = new SqlCommand("Sp_updateuser", connection))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.CommandTimeout = 0;

                foreach (var parameter in mysqlParameters)
                {
                    string paramName = parameter.ParameterName;
                    if (!paramName.StartsWith("@"))
                    {
                        paramName = "@" + paramName.TrimStart('@');
                    }
                    if (string.Equals(paramName, "@p_employee_code", StringComparison.OrdinalIgnoreCase))
                    {
                        paramName = "@p_empcode";
                    }
                    command.Parameters.AddWithValue(paramName, parameter.Value ?? DBNull.Value);
                }

                connection.Open();
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    bool hasAnyRow = false;
                    while (reader.Read())
                    {
                        hasAnyRow = true;
                        string status = "Failed";
                        string remarks = string.Empty;
                        if (HasColumn(reader, "Status") && reader["Status"] != DBNull.Value)
                        {
                            status = Convert.ToString(reader["Status"]);
                        }
                        if (HasColumn(reader, "Remarks") && reader["Remarks"] != DBNull.Value)
                        {
                            remarks = Convert.ToString(reader["Remarks"]);
                        }
                        listdata.Add(new UserDetailsDO { Status = status, Remarks = remarks });
                    }

                    if (!hasAnyRow)
                    {
                        // Some SP variants don't return Status/Remarks; treat successful execution as success.
                        listdata.Add(new UserDetailsDO
                        {
                            Status = "Success",
                            Remarks = "Updated in secondary database."
                        });
                    }
                }
            }

            if (listdata == null || listdata.Count == 0)
            {
                listdata = new List<UserDetailsDO>
                {
                    new UserDetailsDO
                    {
                        Status = "Failed",
                        Remarks = "User update did not return any response from secondary database."
                    }
                };
            }

            return listdata;
        }

        private bool HasColumn(IDataRecord record, string columnName)
        {
            for (int i = 0; i < record.FieldCount; i++)
            {
                if (string.Equals(record.GetName(i), columnName, StringComparison.OrdinalIgnoreCase))
                {
                    return true;
                }
            }
            return false;
        }

        private bool IsActiveValue(object value)
        {
            if (value == null || value == DBNull.Value)
            {
                return false;
            }

            string raw = Convert.ToString(value).Trim();
            return raw == "1" ||
                   string.Equals(raw, "true", StringComparison.OrdinalIgnoreCase) ||
                   string.Equals(raw, "active", StringComparison.OrdinalIgnoreCase);
        }

        public List<UserDetailsDO> ViewAllUsers()
        {
            List<UserDetailsDO> users = new List<UserDetailsDO>();
            try
            {
                var secondaryUsers = GetAllUsersFromConnection(Sqlconnection, true);
                users = (secondaryUsers ?? new List<UserDetailsDO>())
                    .GroupBy(u =>
                        (
                            (u.EmployeeCode ?? string.Empty).Trim().ToUpper() + "|" +
                            (u.Username ?? string.Empty).Trim().ToUpper() + "|" +
                            (u.user_mail_id ?? string.Empty).Trim().ToUpper()
                        ))
                    .Select(g => g.OrderByDescending(x => x.UserId).First())
                    .OrderByDescending(u => u.UserId)
                    .ToList();

                EnrichDesignationName(users);

                if (users == null || users.Count == 0)
                {
                    users = GetAllUsersFromConnection(MySqlconnection, false);
                    EnrichDesignationName(users);
                }
            }
            catch (Exception ex)
            {
                CommonBL errorlog = new CommonBL();
                errorlog.fnStoreErrorLog("UserDetailsBL", "ViewAllUsers", "Exception Message" + ex.Message + "Strace=" + ex.StackTrace, UserId);
                users = GetAllUsersFallbackFromPrimary();
            }
            return users;
        }

        public List<UserDetailsDO> ViewAllUsersMainDb()
        {
            List<UserDetailsDO> users = GetAllUsersFallbackFromPrimary();
            EnrichDesignationName(users);
            return users;
        }

        private void EnrichDesignationName(List<UserDetailsDO> users)
        {
            if (users == null || users.Count == 0)
            {
                return;
            }

            try
            {
                var designationMap = GetDesignationMapFromConnections();
                if (designationMap == null || designationMap.Count == 0)
                {
                    return;
                }

                foreach (var user in users)
                {
                    if (!string.IsNullOrWhiteSpace(user.designation_name))
                    {
                        continue;
                    }

                    if (user.designation_id <= 0)
                    {
                        continue;
                    }

                    string designationText;
                    if (designationMap.TryGetValue(user.designation_id.ToString(), out designationText))
                    {
                        user.designation_name = designationText;
                    }
                }
            }
            catch (Exception ex)
            {
                CommonBL errorlog = new CommonBL();
                errorlog.fnStoreErrorLog("UserDetailsBL", "EnrichDesignationName", "Exception Message" + ex.Message + "Strace=" + ex.StackTrace, UserId);
            }
        }

        private Dictionary<string, string> GetDesignationMapFromConnections()
        {
            var map = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

            Action<string> loadFromMySql = (connectionString) =>
            {
                if (string.IsNullOrWhiteSpace(connectionString)) return;
                string normalizedConnection = NormalizeMySqlConnectionString(connectionString);
                using (MySqlConnection con = new MySqlConnection(normalizedConnection))
                using (MySqlCommand cmd = new MySqlCommand("sp_bindDesignation", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    con.Open();
                    using (var dr = cmd.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            string id = dr["Id"] != DBNull.Value ? Convert.ToString(dr["Id"]).Trim() : string.Empty;
                            string text = dr["Text"] != DBNull.Value ? Convert.ToString(dr["Text"]).Trim() : string.Empty;
                            if (!string.IsNullOrWhiteSpace(id) && !string.IsNullOrWhiteSpace(text) && !map.ContainsKey(id))
                            {
                                map[id] = text;
                            }
                        }
                    }
                }
            };

            Action<string> loadFromSql = (connectionString) =>
            {
                if (string.IsNullOrWhiteSpace(connectionString)) return;
                using (SqlConnection con = new SqlConnection(connectionString))
                using (SqlCommand cmd = new SqlCommand("sp_bindDesignation", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    con.Open();
                    using (var dr = cmd.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            string id = dr["Id"] != DBNull.Value ? Convert.ToString(dr["Id"]).Trim() : string.Empty;
                            string text = dr["Text"] != DBNull.Value ? Convert.ToString(dr["Text"]).Trim() : string.Empty;
                            if (!string.IsNullOrWhiteSpace(id) && !string.IsNullOrWhiteSpace(text) && !map.ContainsKey(id))
                            {
                                map[id] = text;
                            }
                        }
                    }
                }
            };

            try { loadFromMySql(MySqlconnection); } catch { }
            try { loadFromMySql(Sqlconnection); } catch { try { loadFromSql(Sqlconnection); } catch { } }

            return map;
        }

        private List<UserDetailsDO> GetAllUsersFallbackFromPrimary()
        {
            List<UserDetailsDO> users = new List<UserDetailsDO>();
            try
            {
                getDrtolist getDrtolistParam = new getDrtolist();
                List<MySqlParameter> sqlParameters = new List<MySqlParameter>();
                sqlParameters.Add(DataClass.GetParameter("@p_type", "GetAllUser"));
                users = getDrtolistParam.getdatafromreder<UserDetailsDO>(
                    DataClass.GetDataReaderFromSpWithParam(sqlParameters, DBName, "Sp_getalluser"));
            }
            catch (Exception ex)
            {
                CommonBL errorlog = new CommonBL();
                errorlog.fnStoreErrorLog("UserDetailsBL", "GetAllUsersFallbackFromPrimary", "Exception Message" + ex.Message + "Strace=" + ex.StackTrace, UserId);
            }

            return users;
        }

        private List<UserDetailsDO> GetAllUsersFromConnection(string connectionString, bool isSecondarySource)
        {
            List<UserDetailsDO> users = new List<UserDetailsDO>();
            if (string.IsNullOrWhiteSpace(connectionString))
            {
                return users;
            }

            try
            {
                string spName = isSecondarySource ? "Sp_getalluser_hrms" : "Sp_getalluser";
                string normalizedConnection = NormalizeMySqlConnectionString(connectionString);
                using (MySqlConnection con = new MySqlConnection(normalizedConnection))
                using (MySqlCommand cmd = new MySqlCommand(spName, con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@p_type", "GetAllUser");
                    con.Open();

                    using (var dr = cmd.ExecuteReader())
                    {
                        Func<IDataRecord, string, bool> hasCol = (rec, col) =>
                        {
                            for (int i = 0; i < rec.FieldCount; i++)
                            {
                                if (string.Equals(rec.GetName(i), col, StringComparison.OrdinalIgnoreCase))
                                    return true;
                            }
                            return false;
                        };

                        Func<IDataRecord, string[], object> getVal = (rec, cols) =>
                        {
                            foreach (var c in cols)
                            {
                                if (hasCol(rec, c))
                                {
                                    var v = rec[c];
                                    return v == DBNull.Value ? null : v;
                                }
                            }
                            return null;
                        };

                        while (dr.Read())
                        {
                            object userIdVal = getVal(dr, new[] { "UserId", "user_id", "id" });
                            object empCodeVal = getVal(dr, new[] { "EmployeeCode", "employee_code", "emp_code" });
                            object usernameVal = getVal(dr, new[] { "Username", "username", "user_name" });
                            object fullNameVal = getVal(dr, new[] { "user_fullname", "User_fullname", "full_name", "employee_name" });
                            object emailVal = getVal(dr, new[] { "user_mail_id", "User_Email", "email", "employee_email" });
                            object contactVal = getVal(dr, new[] { "contact_detail", "Contact_No", "mobile", "phone" });
                            object companyVal = getVal(dr, new[] { "company_id", "CompanyId", "companyid" });
                            object designationNameVal = getVal(dr, new[] { "designation_name", "DesignationName", "designation" });
                            object designationIdVal = getVal(dr, new[] { "designation_id", "DesignationId", "designationid" });
                            object isActiveVal = getVal(dr, new[] { "Isactive", "is_active", "isactive" });
                            object dojVal = getVal(dr, new[] { "date_of_joining", "DateOfJoining", "joining_date", "doj" });
                            object probationVal = getVal(dr, new[] { "probation_period_months", "ProbationPeriodMonths", "probation_months" });
                            object employeeTypeVal = getVal(dr, new[] { "employee_type", "EmployeeType", "employment_type" });
                            string designationName = designationNameVal != null ? Convert.ToString(designationNameVal) : string.Empty;
                            string designationIdRaw = designationIdVal != null ? Convert.ToString(designationIdVal) : string.Empty;
                            int designationIdParsed = 0;
                            int.TryParse(designationIdRaw, out designationIdParsed);

                            if (string.IsNullOrWhiteSpace(designationName) && !string.IsNullOrWhiteSpace(designationIdRaw) && designationIdParsed == 0)
                            {
                                designationName = designationIdRaw;
                            }
                            if (string.IsNullOrWhiteSpace(designationName))
                            {
                                designationName = ExtractDesignationText(dr);
                            }

                            users.Add(new UserDetailsDO
                            {
                                UserId = userIdVal != null ? Convert.ToInt32(userIdVal) : 0,
                                EmployeeCode = empCodeVal != null ? Convert.ToString(empCodeVal) : string.Empty,
                                Username = usernameVal != null ? Convert.ToString(usernameVal) : string.Empty,
                                user_fullname = fullNameVal != null ? Convert.ToString(fullNameVal) : string.Empty,
                                user_mail_id = emailVal != null ? Convert.ToString(emailVal) : string.Empty,
                                contact_detail = contactVal != null ? Convert.ToString(contactVal) : string.Empty,
                                company_id = companyVal != null ? Convert.ToInt32(companyVal) : 0,
                                CompanyId = companyVal != null ? Convert.ToInt32(companyVal) : 0,
                                designation_name = designationName,
                                designation_id = designationIdParsed,
                                Isactive = isActiveVal != null && IsActiveValue(isActiveVal),
                                date_of_joining = dojVal != null ? Convert.ToDateTime(dojVal) : DateTime.MinValue,
                                probation_period_months = probationVal != null && int.TryParse(Convert.ToString(probationVal), out var probationParsed) ? probationParsed : 0,
                                employee_type = employeeTypeVal != null ? Convert.ToString(employeeTypeVal) : string.Empty
                            });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                CommonBL errorlog = new CommonBL();
                errorlog.fnStoreErrorLog("UserDetailsBL", "GetAllUsersFromConnection", "Exception Message" + ex.Message + "Strace=" + ex.StackTrace, UserId);

                // Fallback: if this connection is actually SQL Server, try SqlClient.
                try
                {
                    users = GetAllUsersFromSqlServerConnection(connectionString, isSecondarySource);
                    if ((users == null || users.Count == 0) && isSecondarySource)
                    {
                        users = GetAllUsersFromSqlServerConnection(connectionString, false);
                    }
                }
                catch (Exception ex2)
                {
                    errorlog.fnStoreErrorLog("UserDetailsBL", "GetAllUsersFromSqlServerConnection", "Exception Message" + ex2.Message + "Strace=" + ex2.StackTrace, UserId);
                }
            }

            return users;
        }

        private List<UserDetailsDO> GetAllUsersFromSqlServerConnection(string connectionString, bool isSecondarySource)
        {
            List<UserDetailsDO> users = new List<UserDetailsDO>();
            string spName = isSecondarySource ? "Sp_getalluser_hrms" : "Sp_getalluser";
            using (SqlConnection con = new SqlConnection(connectionString))
            using (SqlCommand cmd = new SqlCommand(spName, con))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@p_type", "GetAllUser");
                con.Open();
                using (var dr = cmd.ExecuteReader())
                {
                    Func<IDataRecord, string, bool> hasCol = (rec, col) =>
                    {
                        for (int i = 0; i < rec.FieldCount; i++)
                        {
                            if (string.Equals(rec.GetName(i), col, StringComparison.OrdinalIgnoreCase))
                                return true;
                        }
                        return false;
                    };

                    Func<IDataRecord, string[], object> getVal = (rec, cols) =>
                    {
                        foreach (var c in cols)
                        {
                            if (hasCol(rec, c))
                            {
                                var v = rec[c];
                                return v == DBNull.Value ? null : v;
                            }
                        }
                        return null;
                    };

                    while (dr.Read())
                    {
                        object userIdVal = getVal(dr, new[] { "UserId", "user_id", "id" });
                        object empCodeVal = getVal(dr, new[] { "EmployeeCode", "employee_code", "emp_code" });
                        object usernameVal = getVal(dr, new[] { "Username", "username", "user_name" });
                        object fullNameVal = getVal(dr, new[] { "user_fullname", "User_fullname", "full_name", "employee_name" });
                        object emailVal = getVal(dr, new[] { "user_mail_id", "User_Email", "email", "employee_email" });
                        object contactVal = getVal(dr, new[] { "contact_detail", "Contact_No", "mobile", "phone" });
                        object companyVal = getVal(dr, new[] { "company_id", "CompanyId", "companyid" });
                        object designationNameVal = getVal(dr, new[] { "designation_name", "DesignationName", "designation" });
                        object designationIdVal = getVal(dr, new[] { "designation_id", "DesignationId", "designationid" });
                        string designationName = designationNameVal != null ? Convert.ToString(designationNameVal) : string.Empty;
                        string designationIdRaw = designationIdVal != null ? Convert.ToString(designationIdVal) : string.Empty;
                        int designationIdParsed = 0;
                        int.TryParse(designationIdRaw, out designationIdParsed);

                        if (string.IsNullOrWhiteSpace(designationName) && !string.IsNullOrWhiteSpace(designationIdRaw) && designationIdParsed == 0)
                        {
                            designationName = designationIdRaw;
                        }
                        if (string.IsNullOrWhiteSpace(designationName))
                        {
                            designationName = ExtractDesignationText(dr);
                        }

                        users.Add(new UserDetailsDO
                        {
                            UserId = userIdVal != null ? Convert.ToInt32(userIdVal) : 0,
                            EmployeeCode = empCodeVal != null ? Convert.ToString(empCodeVal) : string.Empty,
                            Username = usernameVal != null ? Convert.ToString(usernameVal) : string.Empty,
                            user_fullname = fullNameVal != null ? Convert.ToString(fullNameVal) : string.Empty,
                            user_mail_id = emailVal != null ? Convert.ToString(emailVal) : string.Empty,
                            contact_detail = contactVal != null ? Convert.ToString(contactVal) : string.Empty,
                            company_id = companyVal != null ? Convert.ToInt32(companyVal) : 0,
                            CompanyId = companyVal != null ? Convert.ToInt32(companyVal) : 0,
                            designation_name = designationName,
                            designation_id = designationIdParsed
                        });
                    }
                }
            }
            return users;
        }

        private string ExtractDesignationText(IDataRecord record)
        {
            try
            {
                for (int i = 0; i < record.FieldCount; i++)
                {
                    string col = record.GetName(i);
                    if (string.IsNullOrWhiteSpace(col))
                    {
                        continue;
                    }

                    if (!col.ToLowerInvariant().Contains("designation"))
                    {
                        continue;
                    }

                    object raw = record[i];
                    if (raw == null || raw == DBNull.Value)
                    {
                        continue;
                    }

                    string val = Convert.ToString(raw).Trim();
                    if (string.IsNullOrWhiteSpace(val))
                    {
                        continue;
                    }

                    int temp;
                    if (int.TryParse(val, out temp))
                    {
                        continue;
                    }

                    return val;
                }
            }
            catch
            {
                // keep silent; caller already has normal fallbacks
            }

            return string.Empty;
        }

        private string NormalizeMySqlConnectionString(string rawConnectionString)
        {
            if (string.IsNullOrWhiteSpace(rawConnectionString))
            {
                return rawConnectionString;
            }

            try
            {
                // Convert mixed/legacy keys (Data Source, Initial Catalog, uid) into MySQL-compatible keys.
                var parts = rawConnectionString.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
                var map = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

                foreach (var part in parts)
                {
                    int idx = part.IndexOf('=');
                    if (idx <= 0) continue;
                    string key = part.Substring(0, idx).Trim();
                    string value = part.Substring(idx + 1).Trim();
                    map[key] = value;
                }

                var builder = new MySqlConnectionStringBuilder();
                if (map.ContainsKey("Server")) builder.Server = map["Server"];
                else if (map.ContainsKey("Data Source")) builder.Server = map["Data Source"];
                else if (map.ContainsKey("Datasource")) builder.Server = map["Datasource"];

                if (map.ContainsKey("Database")) builder.Database = map["Database"];
                else if (map.ContainsKey("Initial Catalog")) builder.Database = map["Initial Catalog"];

                if (map.ContainsKey("Port") && uint.TryParse(map["Port"], out uint p)) builder.Port = p;

                if (map.ContainsKey("User Id")) builder.UserID = map["User Id"];
                else if (map.ContainsKey("UserID")) builder.UserID = map["UserID"];
                else if (map.ContainsKey("Uid")) builder.UserID = map["Uid"];
                else if (map.ContainsKey("User")) builder.UserID = map["User"];
                else if (map.ContainsKey("Username")) builder.UserID = map["Username"];

                if (map.ContainsKey("Password")) builder.Password = map["Password"];
                else if (map.ContainsKey("Pwd")) builder.Password = map["Pwd"];

                if (map.ContainsKey("Persist Security Info"))
                {
                    if (bool.TryParse(map["Persist Security Info"], out bool persist))
                    {
                        builder.PersistSecurityInfo = persist;
                    }
                }

                return builder.ConnectionString;
            }
            catch
            {
                return rawConnectionString;
            }
        }
        public List<UserDetailsDO> AdvanceSearch(UserDetailsDO user)
        {
            List<UserDetailsDO> listdata = new List<UserDetailsDO>();
            getDrtolist getDrtolistParam = new getDrtolist();
            List<MySqlParameter> sqlParameters = new List<MySqlParameter>();

            try
            {
                sqlParameters.Add(new MySqlParameter("@p_usernameId", user.usernameId ?? (object)DBNull.Value));
                sqlParameters.Add(new MySqlParameter("@p_contact_detail", user.contact_detail ?? (object)DBNull.Value));
                sqlParameters.Add(new MySqlParameter("@p_empcodeId", user.empcodeId ?? (object)DBNull.Value));

                using (var reader = DataClass.GetDataReaderFromSpWithParam(sqlParameters, DBName, "sp_SearchAdvUser"))
                {
                    listdata = getDrtolistParam.getdatafromreder<UserDetailsDO>(reader);
                }
            }
            catch (Exception ex)
            {
                CommonBL errorlog = new CommonBL();
                errorlog.fnStoreErrorLog(
                    "UserDetailsBL",
                    "AdvanceSearch",
                    "Exception Message=" + ex.Message + " Strace=" + ex.StackTrace,
                    UserId
                );
            }

            return listdata;
        }
        public List<UserDetailsDO> GetUserDetails(int userId)
        {
            List<UserDetailsDO> listdata = new List<UserDetailsDO>();
            getDrtolist getDrtolistParam = new getDrtolist();
            List<MySqlParameter> sqlParameters = new List<MySqlParameter>();
            try
            {
                string type = "";
                if (userId != 0)
                {
                    type = "GetUser";

                }
                else
                {
                    type = "GetNewEmployeeId";
                }

                sqlParameters.Add(DataClass.GetParameter("@p_type", type));
                sqlParameters.Add(DataClass.GetParameter("@p_user_id", userId));
                listdata = getDrtolistParam.getdatafromreder<UserDetailsDO>(DataClass.GetDataReaderFromSpWithParam(sqlParameters, DBName, "Sp_getuser1"));

            }
            catch (Exception ex)
            {
                CommonBL errorlog = new CommonBL();
                errorlog.fnStoreErrorLog("UserDetailsBL", "GetUserDetails", "Exception Message" + ex.Message + "Strace=" + ex.StackTrace, UserId);
                throw ex;
            }
            return listdata;
        }

        public List<UserDetailsDO> GetUserDetailsFromSecondary(int userId, string employeeCode)
        {
            List<UserDetailsDO> listdata = new List<UserDetailsDO>();

            try
            {
                // Try MySQL-compatible execution first with multiple parameter variants.
                try
                {
                    listdata = TryGetSecondaryDetailsMySql(userId, employeeCode);
                }
                catch
                {
                    // Fallback to SQL Server execution if secondary DB is SQL.
                    listdata = TryGetSecondaryDetailsSql(userId, employeeCode);
                }
            }
            catch (Exception ex)
            {
                CommonBL errorlog = new CommonBL();
                errorlog.fnStoreErrorLog("UserDetailsBL", "GetUserDetailsFromSecondary", "Exception Message" + ex.Message + "Strace=" + ex.StackTrace, UserId);
            }

            return listdata;
        }

        private List<UserDetailsDO> TryGetSecondaryDetailsMySql(int userId, string employeeCode)
        {
            string normalizedConnection = NormalizeMySqlConnectionString(Sqlconnection);
            List<UserDetailsDO> onboardingDetails = TryGetEmployeeOnboardingDetailsMySql(normalizedConnection, userId);
            if (onboardingDetails != null && onboardingDetails.Count > 0)
            {
                return onboardingDetails;
            }

            var attempts = new List<Action<MySqlCommand>>
            {
                c => { c.Parameters.AddWithValue("@p_type", "GetUser"); c.Parameters.AddWithValue("@p_user_id", userId); },
                c => { c.Parameters.AddWithValue("@p_user_id", userId); },
                c => { c.Parameters.AddWithValue("@p_userid", userId); },
                c => { if (!string.IsNullOrWhiteSpace(employeeCode)) c.Parameters.AddWithValue("@p_employee_code", employeeCode); },
                c => { if (!string.IsNullOrWhiteSpace(employeeCode)) c.Parameters.AddWithValue("@p_empcode", employeeCode); },
                c => { c.Parameters.AddWithValue("@p_type", "GetUser"); if (!string.IsNullOrWhiteSpace(employeeCode)) c.Parameters.AddWithValue("@p_employee_code", employeeCode); }
            };

            foreach (var setup in attempts)
            {
                using (MySqlConnection con = new MySqlConnection(normalizedConnection))
                using (MySqlCommand cmd = new MySqlCommand("Sp_getuserdetails", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    setup(cmd);
                    con.Open();
                    using (var dr = cmd.ExecuteReader())
                    {
                        var list = MapUserDetailsFromReader(dr);
                        if (list != null && list.Count > 0)
                        {
                            return list;
                        }
                    }
                }
            }

            return new List<UserDetailsDO>();
        }

        private List<UserDetailsDO> TryGetEmployeeOnboardingDetailsMySql(string normalizedConnection, int userId)
        {
            if (userId <= 0)
            {
                return new List<UserDetailsDO>();
            }

            try
            {
                using (MySqlConnection con = new MySqlConnection(normalizedConnection))
                using (MySqlCommand cmd = new MySqlCommand("SP_GetEmployeeOnboardingDetails", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@p_user_id", userId);
                    con.Open();
                    using (var dr = cmd.ExecuteReader())
                    {
                        return MapUserDetailsFromReader(dr);
                    }
                }
            }
            catch (Exception ex)
            {
                CommonBL errorlog = new CommonBL();
                errorlog.fnStoreErrorLog("UserDetailsBL", "TryGetEmployeeOnboardingDetailsMySql", "Exception Message" + ex.Message + "Strace=" + ex.StackTrace, UserId);
                return new List<UserDetailsDO>();
            }
        }

        public List<EmployeeAssetDetailsDO> GetEmployeeAssets(int userId)
        {
            List<EmployeeAssetDetailsDO> assets = new List<EmployeeAssetDetailsDO>();
            if (userId <= 0 || string.IsNullOrWhiteSpace(Sqlconnection))
            {
                return assets;
            }

            try
            {
                string normalizedConnection = NormalizeMySqlConnectionString(Sqlconnection);
                using (MySqlConnection con = new MySqlConnection(normalizedConnection))
                using (MySqlCommand cmd = new MySqlCommand("SP_GetEmployeeAssets", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@p_user_id", userId);

                    con.Open();
                    using (MySqlDataReader dr = cmd.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            assets.Add(new EmployeeAssetDetailsDO
                            {
                                AssetAssignmentId = ReadIntValue(dr, "AssetId", "asset_assignment_id"),
                                UserId = ReadIntValue(dr, "UserId", "user_id"),
                                AssetType = ReadStringValue(dr, "AssetType", "asset_type"),
                                AssetNumber = ReadStringValue(dr, "AssetNumber", "asset_number"),
                                AssetName = ReadStringValue(dr, "AssetName", "asset_name"),
                                AssignedDate = ReadDateValue(dr, "AssignedDate", "assigned_date"),
                                ReturnDate = ReadDateValue(dr, "ReturnDate", "return_date"),
                                AssetConditionId = ReadIntValue(dr, "AssetConditionId", "asset_condition_id"),
                                AssetCondition = ReadStringValue(dr, "AssetCondition", "asset_condition"),
                                AssetStatusId = ReadIntValue(dr, "AssetStatusId", "asset_status_id"),
                                AssetStatus = ReadStringValue(dr, "AssetStatus", "asset_status"),
                                InsertedBy = ReadIntValue(dr, "InsertedBy", "inserted_by"),
                                InsertedDate = ReadDateValue(dr, "InsertedDate", "inserted_date"),
                                UpdatedBy = ReadIntValue(dr, "UpdatedBy", "updated_by"),
                                UpdatedDate = ReadDateValue(dr, "UpdatedDate", "updated_date"),
                                IsActive = IsActiveValue(ReadStringValue(dr, "IsActive", "is_active"))
                            });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                CommonBL errorlog = new CommonBL();
                errorlog.fnStoreErrorLog("UserDetailsBL", "GetEmployeeAssets", "Exception Message" + ex.Message + "Strace=" + ex.StackTrace, UserId);
            }

            return assets;
        }

        public List<EmployeeEducationDetailsDO> GetEmployeeEducation(int userId)
        {
            List<EmployeeEducationDetailsDO> educationList = new List<EmployeeEducationDetailsDO>();
            if (userId <= 0 || string.IsNullOrWhiteSpace(Sqlconnection))
            {
                return educationList;
            }

            try
            {
                string normalizedConnection = NormalizeMySqlConnectionString(Sqlconnection);
                using (MySqlConnection con = new MySqlConnection(normalizedConnection))
                using (MySqlCommand cmd = new MySqlCommand("SP_GetEmployeeEducation", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@p_user_id", userId);

                    con.Open();
                    using (MySqlDataReader dr = cmd.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            educationList.Add(new EmployeeEducationDetailsDO
                            {
                                EducationId = ReadIntValue(dr, "EducationId", "education_id"),
                                UserId = ReadIntValue(dr, "UserId", "user_id"),
                                QualificationLevel = ReadStringValue(dr, "QualificationLevel", "qualification_level"),
                                DegreeName = ReadStringValue(dr, "DegreeName", "degree_name"),
                                Specialization = ReadStringValue(dr, "Specialization", "specialization"),
                                University = ReadStringValue(dr, "University", "university"),
                                InstituteName = ReadStringValue(dr, "InstituteName", "institute_name"),
                                YearOfPassing = ReadStringValue(dr, "YearOfPassing", "year_of_passing"),
                                PercentageCgpa = ReadStringValue(dr, "PercentageCgpa", "percentage_cgpa"),
                                CertificateFile = ReadStringValue(dr, "CertificateFile", "certificate_file")
                            });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                CommonBL errorlog = new CommonBL();
                errorlog.fnStoreErrorLog("UserDetailsBL", "GetEmployeeEducation", "Exception Message" + ex.Message + "Strace=" + ex.StackTrace, UserId);
            }

            return educationList;
        }

        public List<EmployeeWorkExperienceDetailsDO> GetEmployeeWorkExperience(int userId)
        {
            List<EmployeeWorkExperienceDetailsDO> experienceList = new List<EmployeeWorkExperienceDetailsDO>();
            if (userId <= 0 || string.IsNullOrWhiteSpace(Sqlconnection))
            {
                return experienceList;
            }

            try
            {
                string normalizedConnection = NormalizeMySqlConnectionString(Sqlconnection);
                using (MySqlConnection con = new MySqlConnection(normalizedConnection))
                using (MySqlCommand cmd = new MySqlCommand("SP_GetEmployeeWorkExperience", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@p_user_id", userId);

                    con.Open();
                    using (MySqlDataReader dr = cmd.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            experienceList.Add(new EmployeeWorkExperienceDetailsDO
                            {
                                ExperienceId = ReadIntValue(dr, "ExperienceId", "experience_id"),
                                UserId = ReadIntValue(dr, "UserId", "user_id"),
                                OrganizationName = ReadStringValue(dr, "OrganizationName", "organization_name"),
                                Industry = ReadStringValue(dr, "Industry", "industry"),
                                Designation = ReadStringValue(dr, "Designation", "designation"),
                                StartDate = ReadDateValue(dr, "StartDate", "start_date"),
                                EndDate = ReadDateValue(dr, "EndDate", "end_date"),
                                TotalExperience = ReadStringValue(dr, "TotalExperience", "total_experience"),
                                LastDrawnSalary = ReadStringValue(dr, "LastDrawnSalary", "last_drawn_salary"),
                                EmploymentPeriod = ReadStringValue(dr, "EmploymentPeriod", "employment_period")
                            });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                CommonBL errorlog = new CommonBL();
                errorlog.fnStoreErrorLog("UserDetailsBL", "GetEmployeeWorkExperience", "Exception Message" + ex.Message + "Strace=" + ex.StackTrace, UserId);
            }

            return experienceList;
        }

        private string ReadStringValue(IDataRecord record, params string[] columnNames)
        {
            string columnName = GetAvailableColumnName(record, columnNames);
            if (string.IsNullOrEmpty(columnName))
            {
                return string.Empty;
            }

            object value = record[columnName];
            return value == null || value == DBNull.Value ? string.Empty : Convert.ToString(value);
        }

        private int ReadIntValue(IDataRecord record, params string[] columnNames)
        {
            int parsed;
            return int.TryParse(ReadStringValue(record, columnNames), out parsed) ? parsed : 0;
        }

        private DateTime? ReadDateValue(IDataRecord record, params string[] columnNames)
        {
            string value = ReadStringValue(record, columnNames);
            if (string.IsNullOrWhiteSpace(value) || value.Trim() == "-")
            {
                return null;
            }

            DateTime parsed;
            string[] formats = { "dd-MM-yyyy", "yyyy-MM-dd", "dd/MM/yyyy", "yyyy/MM/dd", "MM/dd/yyyy", "M/d/yyyy" };
            if (DateTime.TryParseExact(value.Trim(), formats, System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out parsed))
            {
                return parsed;
            }

            return DateTime.TryParse(value, out parsed) ? parsed : (DateTime?)null;
        }

        private string GetAvailableColumnName(IDataRecord record, params string[] columnNames)
        {
            if (columnNames == null)
            {
                return string.Empty;
            }

            foreach (string columnName in columnNames)
            {
                if (HasColumn(record, columnName))
                {
                    return columnName;
                }
            }

            return string.Empty;
        }

        private List<UserDetailsDO> TryGetSecondaryDetailsSql(int userId, string employeeCode)
        {
            var attempts = new List<Action<SqlCommand>>
            {
                c => { c.Parameters.AddWithValue("@p_type", "GetUser"); c.Parameters.AddWithValue("@p_user_id", userId); },
                c => { c.Parameters.AddWithValue("@p_user_id", userId); },
                c => { c.Parameters.AddWithValue("@p_userid", userId); },
                c => { if (!string.IsNullOrWhiteSpace(employeeCode)) c.Parameters.AddWithValue("@p_employee_code", employeeCode); },
                c => { if (!string.IsNullOrWhiteSpace(employeeCode)) c.Parameters.AddWithValue("@p_empcode", employeeCode); },
                c => { c.Parameters.AddWithValue("@p_type", "GetUser"); if (!string.IsNullOrWhiteSpace(employeeCode)) c.Parameters.AddWithValue("@p_employee_code", employeeCode); }
            };

            foreach (var setup in attempts)
            {
                using (SqlConnection con = new SqlConnection(Sqlconnection))
                using (SqlCommand cmd = new SqlCommand("Sp_getuserdetails", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    setup(cmd);
                    con.Open();
                    using (var dr = cmd.ExecuteReader())
                    {
                        var list = MapUserDetailsFromReader(dr);
                        if (list != null && list.Count > 0)
                        {
                            return list;
                        }
                    }
                }
            }

            return new List<UserDetailsDO>();
        }

        private List<UserDetailsDO> MapUserDetailsFromReader(IDataReader dr)
        {
            List<UserDetailsDO> users = new List<UserDetailsDO>();
            while (dr.Read())
            {
                Func<IDataRecord, string, bool> hasCol = (rec, col) =>
                {
                    for (int i = 0; i < rec.FieldCount; i++)
                    {
                        if (string.Equals(rec.GetName(i), col, StringComparison.OrdinalIgnoreCase))
                            return true;
                    }
                    return false;
                };

                Func<IDataRecord, string[], object> getVal = (rec, cols) =>
                {
                    foreach (var c in cols)
                    {
                        if (hasCol(rec, c))
                        {
                            var v = rec[c];
                            return v == DBNull.Value ? null : v;
                        }
                    }
                    return null;
                };

                object userIdVal = getVal((IDataRecord)dr, new[] { "UserId", "user_id", "id" });
                object empCodeVal = getVal((IDataRecord)dr, new[] { "EmployeeCode", "employee_code", "emp_code" });
                object usernameVal = getVal((IDataRecord)dr, new[] { "Username", "username", "user_name" });
                object fullNameVal = getVal((IDataRecord)dr, new[] { "user_fullname", "User_fullname", "full_name", "employee_name" });
                object emailVal = getVal((IDataRecord)dr, new[] { "user_mail_id", "User_Email", "email", "employee_email" });
                object contactVal = getVal((IDataRecord)dr, new[] { "contact_detail", "Contact_No", "mobile", "phone" });
                object companyVal = getVal((IDataRecord)dr, new[] { "company_id", "CompanyId", "companyid", "comp_id", "company" });
                object companyNameVal = getVal((IDataRecord)dr, new[] { "company_name", "CompanyName", "comp_name", "company" });
                object esicVal = getVal((IDataRecord)dr, new[] { "ESIC_no", "esic_no", "ESICNo", "esicno" });
                object pfVal = getVal((IDataRecord)dr, new[] { "PF_no", "pf_no", "PFNo", "pfno" });
                object deptVal = getVal((IDataRecord)dr, new[] { "department", "Department", "dept", "dept_name" });
                object branchVal = getVal((IDataRecord)dr, new[] { "branch_office", "branch", "Branch", "branch_name" });
                object divVal = getVal((IDataRecord)dr, new[] { "division", "Division", "div", "division_name" });
                object dojVal = getVal((IDataRecord)dr, new[] { "date_of_joining", "DateOfJoining", "joining_date", "doj" });
                object probVal = getVal((IDataRecord)dr, new[] { "probation_period_months", "ProbationPeriodMonths", "probation_period", "probation_months" });
                object mgrVal = getVal((IDataRecord)dr, new[] { "reporting_manager_id", "reporting_manager", "ReportingManager", "reportingmanager", "manager_id", "manager" });
                object typeVal = getVal((IDataRecord)dr, new[] { "employee_type", "EmployeeType", "employment_type", "emp_type" });
                object employmentTypeIdVal = getVal((IDataRecord)dr, new[] { "EmploymentTypeId", "employment_type_id" });
                object designationNameVal = getVal((IDataRecord)dr, new[] { "designation_name", "DesignationName", "designation" });
                object designationIdVal = getVal((IDataRecord)dr, new[] { "designation_id", "DesignationId", "designationid", "designation" });
                object firstNameVal = getVal((IDataRecord)dr, new[] { "first_name" });
                object middleNameVal = getVal((IDataRecord)dr, new[] { "middle_name" });
                object lastNameVal = getVal((IDataRecord)dr, new[] { "last_name" });
                object displayNameVal = getVal((IDataRecord)dr, new[] { "display_name" });
                object genderVal = getVal((IDataRecord)dr, new[] { "gender" });
                object dobVal = getVal((IDataRecord)dr, new[] { "DOB", "date_of_birth" });
                object ageVal = getVal((IDataRecord)dr, new[] { "age" });
                object maritalStatusVal = getVal((IDataRecord)dr, new[] { "marital_status" });
                object bloodGroupVal = getVal((IDataRecord)dr, new[] { "blood_group" });
                object nationalityVal = getVal((IDataRecord)dr, new[] { "nationality" });
                object aadhaarVal = getVal((IDataRecord)dr, new[] { "aadhaar_number" });
                object panVal = getVal((IDataRecord)dr, new[] { "pan_number" });
                object passportVal = getVal((IDataRecord)dr, new[] { "passport_number" });
                object passportExpiryVal = getVal((IDataRecord)dr, new[] { "passport_expiry_date" });
                object employeePhotoVal = getVal((IDataRecord)dr, new[] { "employee_photo", "EmployeePhoto" });
                object employeeStatusVal = getVal((IDataRecord)dr, new[] { "employee_status" });
                object noticePeriodVal = getVal((IDataRecord)dr, new[] { "notice_period" });
                object exitDateVal = getVal((IDataRecord)dr, new[] { "exit_date" });
                object separationReasonVal = getVal((IDataRecord)dr, new[] { "separation_reason" });
                object confirmationDateVal = getVal((IDataRecord)dr, new[] { "confirmation_date" });
                object probationEndDateVal = getVal((IDataRecord)dr, new[] { "probation_end_date" });
                object extendedProbationDaysVal = getVal((IDataRecord)dr, new[] { "extended_probation_days" });
                object extendedProbationMonthVal = getVal((IDataRecord)dr, new[] { "extended_probation_month" });
                object probationStatusVal = getVal((IDataRecord)dr, new[] { "probation_status" });
                object probationRemarksVal = getVal((IDataRecord)dr, new[] { "probation_remarks" });
                object approvedByVal = getVal((IDataRecord)dr, new[] { "approved_by" });
                object branchOfficeVal = getVal((IDataRecord)dr, new[] { "branch_office" });
                object locationVal = getVal((IDataRecord)dr, new[] { "location" });
                object employeeLevelVal = getVal((IDataRecord)dr, new[] { "employee_level" });
                object functionalManagerVal = getVal((IDataRecord)dr, new[] { "functional_manager_name" });
                object hodVal = getVal((IDataRecord)dr, new[] { "hod_name" });
                object attendanceTypeVal = getVal((IDataRecord)dr, new[] { "attendance_type" });
                object weeklyOffVal = getVal((IDataRecord)dr, new[] { "weekly_off" });
                object attendancePolicyVal = getVal((IDataRecord)dr, new[] { "attendance_policy" });
                object workingHoursVal = getVal((IDataRecord)dr, new[] { "working_hours" });
                object punchingDeviceVal = getVal((IDataRecord)dr, new[] { "punching_device_id" });
                object biometricVal = getVal((IDataRecord)dr, new[] { "biometric_id" });
                object overtimeEligibleVal = getVal((IDataRecord)dr, new[] { "overtime_eligible" });
                object overtimeRateVal = getVal((IDataRecord)dr, new[] { "overtime_rate" });
                object workLocationVal = getVal((IDataRecord)dr, new[] { "work_location" });
                object remoteWorkVal = getVal((IDataRecord)dr, new[] { "remote_work_allowed" });
                object assetTypeVal = getVal((IDataRecord)dr, new[] { "asset_type" });
                object assetNumberVal = getVal((IDataRecord)dr, new[] { "asset_number" });
                object assetNameVal = getVal((IDataRecord)dr, new[] { "asset_name" });
                object assignedDateVal = getVal((IDataRecord)dr, new[] { "assigned_date" });
                object returnDateVal = getVal((IDataRecord)dr, new[] { "return_date" });
                object assetConditionVal = getVal((IDataRecord)dr, new[] { "asset_condition" });
                object assetStatusVal = getVal((IDataRecord)dr, new[] { "asset_status" });
                object bankNameVal = getVal((IDataRecord)dr, new[] { "bank_name" });
                object bankBranchVal = getVal((IDataRecord)dr, new[] { "branch_name" });
                object accountHolderVal = getVal((IDataRecord)dr, new[] { "account_holder_name" });
                object accountNumberVal = getVal((IDataRecord)dr, new[] { "account_number" });
                object confirmAccountVal = getVal((IDataRecord)dr, new[] { "confirm_account_number" });
                object ifscVal = getVal((IDataRecord)dr, new[] { "ifsc_code" });
                object accountTypeVal = getVal((IDataRecord)dr, new[] { "account_type" });
                object salaryFlagVal = getVal((IDataRecord)dr, new[] { "salary_account_flag" });
                object qualificationVal = getVal((IDataRecord)dr, new[] { "qualification_level" });
                object degreeVal = getVal((IDataRecord)dr, new[] { "degree_name" });
                object specializationVal = getVal((IDataRecord)dr, new[] { "specialization" });
                object universityVal = getVal((IDataRecord)dr, new[] { "university" });
                object instituteVal = getVal((IDataRecord)dr, new[] { "institute_name" });
                object passingYearVal = getVal((IDataRecord)dr, new[] { "year_of_passing" });
                object percentageVal = getVal((IDataRecord)dr, new[] { "percentage_cgpa" });
                object educationCertificateFileVal = getVal((IDataRecord)dr, new[] { "education_certificate_file", "qualification_certificate_file", "certificate_file" });
                object permanentHouseVal = getVal((IDataRecord)dr, new[] { "permanent_house_number" });
                object permanentBuildingVal = getVal((IDataRecord)dr, new[] { "permanent_building_name" });
                object permanentStreetVal = getVal((IDataRecord)dr, new[] { "permanent_street" });
                object permanentAreaVal = getVal((IDataRecord)dr, new[] { "permanent_area" });
                object permanentLandmarkVal = getVal((IDataRecord)dr, new[] { "permanent_landmark" });
                object permanentCityVal = getVal((IDataRecord)dr, new[] { "permanent_city" });
                object permanentDistrictVal = getVal((IDataRecord)dr, new[] { "permanent_district" });
                object permanentStateVal = getVal((IDataRecord)dr, new[] { "permanent_state" });
                object permanentCountryVal = getVal((IDataRecord)dr, new[] { "permanent_country" });
                object permanentPinVal = getVal((IDataRecord)dr, new[] { "permanent_pin_code" });
                object sameAsPermanentVal = getVal((IDataRecord)dr, new[] { "same_as_permanent" });
                object currentHouseVal = getVal((IDataRecord)dr, new[] { "current_house_number" });
                object currentBuildingVal = getVal((IDataRecord)dr, new[] { "current_building_name" });
                object currentStreetVal = getVal((IDataRecord)dr, new[] { "current_street" });
                object currentAreaVal = getVal((IDataRecord)dr, new[] { "current_area" });
                object currentLandmarkVal = getVal((IDataRecord)dr, new[] { "current_landmark" });
                object currentCityVal = getVal((IDataRecord)dr, new[] { "current_city" });
                object currentDistrictVal = getVal((IDataRecord)dr, new[] { "current_district" });
                object currentStateVal = getVal((IDataRecord)dr, new[] { "current_state" });
                object currentCountryVal = getVal((IDataRecord)dr, new[] { "current_country" });
                object currentPinVal = getVal((IDataRecord)dr, new[] { "current_pin_code" });
                object alternateMobileVal = getVal((IDataRecord)dr, new[] { "alternate_mobile_number" });
                object personalEmailVal = getVal((IDataRecord)dr, new[] { "personal_email" });
                object emergencyNameVal = getVal((IDataRecord)dr, new[] { "emergency_contact_name" });
                object emergencyNumberVal = getVal((IDataRecord)dr, new[] { "emergency_contact_number" });
                object emergencyRelationVal = getVal((IDataRecord)dr, new[] { "emergency_contact_relationship" });
                object certificationNameVal = getVal((IDataRecord)dr, new[] { "certification_name" });
                object certificationAuthorityVal = getVal((IDataRecord)dr, new[] { "certification_authority" });
                object certificateNumberVal = getVal((IDataRecord)dr, new[] { "certificate_number" });
                object issueDateVal = getVal((IDataRecord)dr, new[] { "issue_date" });
                object expiryDateVal = getVal((IDataRecord)dr, new[] { "expiry_date" });
                object certificationFileVal = getVal((IDataRecord)dr, new[] { "certification_file", "professional_certificate_file" });
                object renewalRequiredVal = getVal((IDataRecord)dr, new[] { "renewal_required" });

                string designationName = designationNameVal != null ? Convert.ToString(designationNameVal) : string.Empty;
                string designationIdRaw = designationIdVal != null ? Convert.ToString(designationIdVal) : string.Empty;
                int designationIdParsed = 0;
                int.TryParse(designationIdRaw, out designationIdParsed);
                if (string.IsNullOrWhiteSpace(designationName) && !string.IsNullOrWhiteSpace(designationIdRaw) && designationIdParsed == 0)
                {
                    designationName = designationIdRaw;
                }

                users.Add(new UserDetailsDO
                {
                    UserId = userIdVal != null ? Convert.ToInt32(userIdVal) : 0,
                    EmployeeCode = empCodeVal != null ? Convert.ToString(empCodeVal) : string.Empty,
                    Username = usernameVal != null ? Convert.ToString(usernameVal) : string.Empty,
                    user_fullname = fullNameVal != null ? Convert.ToString(fullNameVal) : string.Empty,
                    user_mail_id = emailVal != null ? Convert.ToString(emailVal) : string.Empty,
                    contact_detail = contactVal != null ? Convert.ToString(contactVal) : string.Empty,
                    company_id = companyVal != null && int.TryParse(Convert.ToString(companyVal), out var companyIdParsed) ? companyIdParsed : 0,
                    CompanyId = companyVal != null && int.TryParse(Convert.ToString(companyVal), out var companyIdParsed2) ? companyIdParsed2 : 0,
                    company_name = companyNameVal != null ? Convert.ToString(companyNameVal) : string.Empty,
                    ESIC_no = esicVal != null ? Convert.ToInt32(esicVal) : 0,
                    PF_no = pfVal != null ? Convert.ToInt32(pfVal) : 0,
                    department = deptVal != null ? Convert.ToString(deptVal) : string.Empty,
                    branch = branchVal != null ? Convert.ToString(branchVal) : string.Empty,
                    division = divVal != null ? Convert.ToString(divVal) : string.Empty,
                    date_of_joining = dojVal != null ? Convert.ToDateTime(dojVal) : DateTime.MinValue,
                    probation_period_months = probVal != null ? Convert.ToInt32(probVal) : 0,
                    reporting_manager = mgrVal != null ? Convert.ToString(mgrVal) : string.Empty,
                    employee_type = typeVal != null ? Convert.ToString(typeVal) : string.Empty,
                    EmploymentTypeId = employmentTypeIdVal != null ? Convert.ToString(employmentTypeIdVal) : string.Empty,
                    designation_name = designationName,
                    designation_id = designationIdParsed,
                    FirstName = firstNameVal != null ? Convert.ToString(firstNameVal) : string.Empty,
                    MiddleName = middleNameVal != null ? Convert.ToString(middleNameVal) : string.Empty,
                    LastName = lastNameVal != null ? Convert.ToString(lastNameVal) : string.Empty,
                    DisplayName = displayNameVal != null ? Convert.ToString(displayNameVal) : string.Empty,
                    Gender = genderVal != null ? Convert.ToString(genderVal) : string.Empty,
                    DateOfBirth = dobVal != null ? Convert.ToDateTime(dobVal) : (DateTime?)null,
                    Age = ageVal != null && int.TryParse(Convert.ToString(ageVal), out var ageParsed) ? ageParsed : 0,
                    MaritalStatus = maritalStatusVal != null ? Convert.ToString(maritalStatusVal) : string.Empty,
                    BloodGroup = bloodGroupVal != null ? Convert.ToString(bloodGroupVal) : string.Empty,
                    Nationality = nationalityVal != null ? Convert.ToString(nationalityVal) : string.Empty,
                    AadhaarNumber = aadhaarVal != null ? Convert.ToString(aadhaarVal) : string.Empty,
                    PanNumber = panVal != null ? Convert.ToString(panVal) : string.Empty,
                    PassportNumber = passportVal != null ? Convert.ToString(passportVal) : string.Empty,
                    PassportExpiryDate = passportExpiryVal != null ? Convert.ToDateTime(passportExpiryVal) : (DateTime?)null,
                    EmployeePhoto = employeePhotoVal != null ? Convert.ToString(employeePhotoVal) : string.Empty,
                    EmployeeStatus = employeeStatusVal != null ? Convert.ToString(employeeStatusVal) : string.Empty,
                    NoticePeriod = noticePeriodVal != null && int.TryParse(Convert.ToString(noticePeriodVal), out var noticeParsed) ? noticeParsed : 0,
                    ExitDate = exitDateVal != null ? Convert.ToDateTime(exitDateVal) : (DateTime?)null,
                    SeparationReason = separationReasonVal != null ? Convert.ToString(separationReasonVal) : string.Empty,
                    ConfirmationDate = confirmationDateVal != null ? Convert.ToDateTime(confirmationDateVal) : (DateTime?)null,
                    ProbationEndDate = probationEndDateVal != null ? Convert.ToDateTime(probationEndDateVal) : (DateTime?)null,
                    ExtendedProbationDays = extendedProbationDaysVal != null && int.TryParse(Convert.ToString(extendedProbationDaysVal), out var extDaysParsed) ? extDaysParsed : 0,
                    ExtendedProbationMonth = extendedProbationMonthVal != null && int.TryParse(Convert.ToString(extendedProbationMonthVal), out var extMonthParsed) ? extMonthParsed : 0,
                    ProbationStatus = probationStatusVal != null ? Convert.ToString(probationStatusVal) : string.Empty,
                    ProbationRemarks = probationRemarksVal != null ? Convert.ToString(probationRemarksVal) : string.Empty,
                    ApprovedBy = approvedByVal != null ? Convert.ToString(approvedByVal) : string.Empty,
                    BranchOffice = branchOfficeVal != null ? Convert.ToString(branchOfficeVal) : string.Empty,
                    Location = locationVal != null ? Convert.ToString(locationVal) : string.Empty,
                    EmployeeLevel = employeeLevelVal != null ? Convert.ToString(employeeLevelVal) : string.Empty,
                    FunctionalManagerName = functionalManagerVal != null ? Convert.ToString(functionalManagerVal) : string.Empty,
                    HodName = hodVal != null ? Convert.ToString(hodVal) : string.Empty,
                    AttendanceType = attendanceTypeVal != null ? Convert.ToString(attendanceTypeVal) : string.Empty,
                    WeeklyOff = weeklyOffVal != null ? Convert.ToString(weeklyOffVal) : string.Empty,
                    AttendancePolicy = attendancePolicyVal != null ? Convert.ToString(attendancePolicyVal) : string.Empty,
                    WorkingHours = workingHoursVal != null ? Convert.ToString(workingHoursVal) : string.Empty,
                    PunchingDeviceId = punchingDeviceVal != null ? Convert.ToString(punchingDeviceVal) : string.Empty,
                    BiometricId = biometricVal != null ? Convert.ToString(biometricVal) : string.Empty,
                    OvertimeEligible = overtimeEligibleVal != null ? Convert.ToString(overtimeEligibleVal) : string.Empty,
                    OvertimeRate = overtimeRateVal != null ? Convert.ToString(overtimeRateVal) : string.Empty,
                    WorkLocation = workLocationVal != null ? Convert.ToString(workLocationVal) : string.Empty,
                    RemoteWorkAllowed = remoteWorkVal != null ? Convert.ToString(remoteWorkVal) : string.Empty,
                    AssetType = assetTypeVal != null ? Convert.ToString(assetTypeVal) : string.Empty,
                    AssetNumber = assetNumberVal != null ? Convert.ToString(assetNumberVal) : string.Empty,
                    AssetName = assetNameVal != null ? Convert.ToString(assetNameVal) : string.Empty,
                    AssignedDate = assignedDateVal != null ? Convert.ToDateTime(assignedDateVal) : (DateTime?)null,
                    ReturnDate = returnDateVal != null ? Convert.ToDateTime(returnDateVal) : (DateTime?)null,
                    AssetCondition = assetConditionVal != null ? Convert.ToString(assetConditionVal) : string.Empty,
                    AssetStatus = assetStatusVal != null ? Convert.ToString(assetStatusVal) : string.Empty,
                    BankName = bankNameVal != null ? Convert.ToString(bankNameVal) : string.Empty,
                    BankBranchName = bankBranchVal != null ? Convert.ToString(bankBranchVal) : string.Empty,
                    AccountHolderName = accountHolderVal != null ? Convert.ToString(accountHolderVal) : string.Empty,
                    AccountNumber = accountNumberVal != null ? Convert.ToString(accountNumberVal) : string.Empty,
                    ConfirmAccountNumber = confirmAccountVal != null ? Convert.ToString(confirmAccountVal) : string.Empty,
                    IfscCode = ifscVal != null ? Convert.ToString(ifscVal) : string.Empty,
                    AccountType = accountTypeVal != null ? Convert.ToString(accountTypeVal) : string.Empty,
                    SalaryAccountFlagText = salaryFlagVal != null ? Convert.ToString(salaryFlagVal) : string.Empty,
                    QualificationLevel = qualificationVal != null ? Convert.ToString(qualificationVal) : string.Empty,
                    DegreeName = degreeVal != null ? Convert.ToString(degreeVal) : string.Empty,
                    Specialization = specializationVal != null ? Convert.ToString(specializationVal) : string.Empty,
                    University = universityVal != null ? Convert.ToString(universityVal) : string.Empty,
                    InstituteName = instituteVal != null ? Convert.ToString(instituteVal) : string.Empty,
                    YearOfPassing = passingYearVal != null ? Convert.ToString(passingYearVal) : string.Empty,
                    PercentageCgpa = percentageVal != null ? Convert.ToString(percentageVal) : string.Empty,
                    EducationCertificateFile = educationCertificateFileVal != null ? Convert.ToString(educationCertificateFileVal) : string.Empty,
                    PermanentHouseNumber = permanentHouseVal != null ? Convert.ToString(permanentHouseVal) : string.Empty,
                    PermanentBuildingName = permanentBuildingVal != null ? Convert.ToString(permanentBuildingVal) : string.Empty,
                    PermanentStreet = permanentStreetVal != null ? Convert.ToString(permanentStreetVal) : string.Empty,
                    PermanentArea = permanentAreaVal != null ? Convert.ToString(permanentAreaVal) : string.Empty,
                    PermanentLandmark = permanentLandmarkVal != null ? Convert.ToString(permanentLandmarkVal) : string.Empty,
                    PermanentCity = permanentCityVal != null ? Convert.ToString(permanentCityVal) : string.Empty,
                    PermanentDistrict = permanentDistrictVal != null ? Convert.ToString(permanentDistrictVal) : string.Empty,
                    PermanentState = permanentStateVal != null ? Convert.ToString(permanentStateVal) : string.Empty,
                    PermanentCountry = permanentCountryVal != null ? Convert.ToString(permanentCountryVal) : string.Empty,
                    PermanentPinCode = permanentPinVal != null ? Convert.ToString(permanentPinVal) : string.Empty,
                    SameAsPermanent = sameAsPermanentVal != null ? Convert.ToString(sameAsPermanentVal) : string.Empty,
                    CurrentHouseNumber = currentHouseVal != null ? Convert.ToString(currentHouseVal) : string.Empty,
                    CurrentBuildingName = currentBuildingVal != null ? Convert.ToString(currentBuildingVal) : string.Empty,
                    CurrentStreet = currentStreetVal != null ? Convert.ToString(currentStreetVal) : string.Empty,
                    CurrentArea = currentAreaVal != null ? Convert.ToString(currentAreaVal) : string.Empty,
                    CurrentLandmark = currentLandmarkVal != null ? Convert.ToString(currentLandmarkVal) : string.Empty,
                    CurrentCity = currentCityVal != null ? Convert.ToString(currentCityVal) : string.Empty,
                    CurrentDistrict = currentDistrictVal != null ? Convert.ToString(currentDistrictVal) : string.Empty,
                    CurrentState = currentStateVal != null ? Convert.ToString(currentStateVal) : string.Empty,
                    CurrentCountry = currentCountryVal != null ? Convert.ToString(currentCountryVal) : string.Empty,
                    CurrentPinCode = currentPinVal != null ? Convert.ToString(currentPinVal) : string.Empty,
                    AlternateMobileNumber = alternateMobileVal != null ? Convert.ToString(alternateMobileVal) : string.Empty,
                    PersonalEmail = personalEmailVal != null ? Convert.ToString(personalEmailVal) : string.Empty,
                    EmergencyContactName = emergencyNameVal != null ? Convert.ToString(emergencyNameVal) : string.Empty,
                    EmergencyContactNumber = emergencyNumberVal != null ? Convert.ToString(emergencyNumberVal) : string.Empty,
                    EmergencyContactRelationship = emergencyRelationVal != null ? Convert.ToString(emergencyRelationVal) : string.Empty,
                    CertificationName = certificationNameVal != null ? Convert.ToString(certificationNameVal) : string.Empty,
                    CertificationAuthority = certificationAuthorityVal != null ? Convert.ToString(certificationAuthorityVal) : string.Empty,
                    CertificateNumber = certificateNumberVal != null ? Convert.ToString(certificateNumberVal) : string.Empty,
                    IssueDate = issueDateVal != null ? Convert.ToDateTime(issueDateVal) : (DateTime?)null,
                    ExpiryDate = expiryDateVal != null ? Convert.ToDateTime(expiryDateVal) : (DateTime?)null,
                    CertificationFile = certificationFileVal != null ? Convert.ToString(certificationFileVal) : string.Empty,
                    RenewalRequired = renewalRequiredVal != null ? Convert.ToString(renewalRequiredVal) : string.Empty
                });
            }

            return users;
        }
    
        public UserDetailsDO DeactivateUser(int userId, string employeeCode = "")
        {
            int secondaryUserId = ResolveSecondaryUserIdForDelete(userId, employeeCode);
            UserDetailsDO secondaryResult = DeactivateUserByConnection(Sqlconnection, secondaryUserId, "DeactivateUserSecondary");
            if (secondaryResult == null)
            {
                return new UserDetailsDO { Status = "Failed", Remarks = "Soft delete failed in secondary database." };
            }
            return secondaryResult;
        }

        private UserDetailsDO DeactivateUserByConnection(string connectionString, int userId, string methodName)
        {
            UserDetailsDO result = new UserDetailsDO { Status = "Failed", Remarks = "No response from database." };
            if (string.IsNullOrWhiteSpace(connectionString))
            {
                result.Remarks = "Connection string not configured.";
                return result;
            }

            try
            {
                result = DeactivateUserByConnectionMySql(connectionString, userId);
            }
            catch
            {
                try
                {
                    result = DeactivateUserByConnectionSql(connectionString, userId);
                }
                catch (Exception ex)
                {
                    CommonBL errorlog = new CommonBL();
                    errorlog.fnStoreErrorLog(
                        "UserDetailsBL",
                        methodName,
                        "Exception Message: " + ex.Message + " | StackTrace=" + ex.StackTrace,
                        UserId
                    );
                    result.Status = "Failed";
                    result.Remarks = "Soft delete execution failed.";
                }
            }

            return result;
        }

        private UserDetailsDO DeactivateUserByConnectionMySql(string connectionString, int userId)
        {
            UserDetailsDO result = new UserDetailsDO { Status = "Failed", Remarks = "No response from database." };
            string normalizedConnection = NormalizeMySqlConnectionString(connectionString);
            var attempts = new List<Action<MySqlCommand>>
            {
                c => { c.Parameters.AddWithValue("@p_type", "DeleteUser"); c.Parameters.AddWithValue("@p_user_id", userId); },
                c => { c.Parameters.AddWithValue("@p_type", "DeleteUser"); c.Parameters.AddWithValue("@p_userid", userId); },
                c => { c.Parameters.AddWithValue("@p_user_id", userId); },
                c => { c.Parameters.AddWithValue("@p_userid", userId); }
            };

            foreach (var setup in attempts)
            {
                using (MySqlConnection con = new MySqlConnection(normalizedConnection))
                using (MySqlCommand cmd = new MySqlCommand("Sp_deleteuser", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    setup(cmd);

                    con.Open();
                    using (MySqlDataReader dr = cmd.ExecuteReader())
                    {
                        if (dr.Read())
                        {
                            result.Status = dr["Status"] != DBNull.Value ? dr["Status"].ToString() : "Success";
                            result.Remarks = dr["Remarks"] != DBNull.Value ? dr["Remarks"].ToString() : "User deleted successfully.";
                        }
                        else
                        {
                            result.Status = "Success";
                            result.Remarks = "User deleted successfully.";
                        }
                    }
                    if (string.Equals(result.Status, "Success", StringComparison.OrdinalIgnoreCase))
                    {
                        return result;
                    }
                }
            }
            return result;
        }

        private UserDetailsDO DeactivateUserByConnectionSql(string connectionString, int userId)
        {
            UserDetailsDO result = new UserDetailsDO { Status = "Failed", Remarks = "No response from database." };
            var attempts = new List<Action<SqlCommand>>
            {
                c => { c.Parameters.AddWithValue("@p_type", "DeleteUser"); c.Parameters.AddWithValue("@p_user_id", userId); },
                c => { c.Parameters.AddWithValue("@p_type", "DeleteUser"); c.Parameters.AddWithValue("@p_userid", userId); },
                c => { c.Parameters.AddWithValue("@p_user_id", userId); },
                c => { c.Parameters.AddWithValue("@p_userid", userId); }
            };

            foreach (var setup in attempts)
            {
                using (SqlConnection con = new SqlConnection(connectionString))
                using (SqlCommand cmd = new SqlCommand("Sp_deleteuser", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    setup(cmd);

                    con.Open();
                    using (SqlDataReader dr = cmd.ExecuteReader())
                    {
                        if (dr.Read())
                        {
                            string status = "Success";
                            string remarks = "User deleted successfully.";
                            if (HasColumn(dr, "Status") && dr["Status"] != DBNull.Value)
                            {
                                status = Convert.ToString(dr["Status"]);
                            }
                            if (HasColumn(dr, "Remarks") && dr["Remarks"] != DBNull.Value)
                            {
                                remarks = Convert.ToString(dr["Remarks"]);
                            }
                            result.Status = status;
                            result.Remarks = remarks;
                        }
                        else
                        {
                            result.Status = "Success";
                            result.Remarks = "User deleted successfully.";
                        }
                    }
                    if (string.Equals(result.Status, "Success", StringComparison.OrdinalIgnoreCase))
                    {
                        return result;
                    }
                }
            }
            return result;
        }

        private int ResolveSecondaryUserIdForDelete(int incomingUserId, string employeeCode)
        {
            try
            {
                List<UserDetailsDO> secondaryUsers = GetAllUsersFromConnection(Sqlconnection, true);

                if (string.IsNullOrWhiteSpace(employeeCode))
                {
                    List<UserDetailsDO> primaryDetails = GetUserDetails(incomingUserId);
                    if (primaryDetails != null && primaryDetails.Count > 0)
                    {
                        employeeCode = Convert.ToString(primaryDetails[0].EmployeeCode ?? string.Empty).Trim();
                    }
                }

                if (!string.IsNullOrWhiteSpace(employeeCode))
                {
                    UserDetailsDO byCode = secondaryUsers.FirstOrDefault(u =>
                        string.Equals(
                            Convert.ToString(u.EmployeeCode ?? string.Empty).Trim(),
                            employeeCode,
                            StringComparison.OrdinalIgnoreCase));
                    if (byCode != null && byCode.UserId > 0)
                    {
                        return byCode.UserId;
                    }
                }

                if (incomingUserId > 0)
                {
                    UserDetailsDO direct = secondaryUsers.FirstOrDefault(u => u.UserId == incomingUserId);
                    if (direct != null)
                    {
                        return incomingUserId;
                    }
                }
            }
            catch
            {
            }

            return incomingUserId;
        }

        public void SendUserCredentialsMail(string emailId, string password)
        {
            SendUserCredentialsMail(emailId, string.Empty, password);
        }

        public string GetActiveOfficialEmail(int userId, string fallbackEmail = "")
        {
            try
            {
                if (userId <= 0 || string.IsNullOrWhiteSpace(Sqlconnection))
                {
                    return fallbackEmail;
                }

                string normalizedConnection = NormalizeMySqlConnectionString(Sqlconnection);
                using (MySqlConnection connection = new MySqlConnection(normalizedConnection))
                using (MySqlCommand command = new MySqlCommand(@"
                    SELECT official_email
                    FROM lean_mngt.employee_official_contact_history
                    WHERE user_id = @p_user_id
                      AND is_active = 1
                    ORDER BY contact_history_id DESC
                    LIMIT 1;", connection))
                {
                    command.CommandType = CommandType.Text;
                    command.Parameters.AddWithValue("@p_user_id", userId);
                    connection.Open();

                    object result = command.ExecuteScalar();
                    string activeEmail = Convert.ToString(result ?? string.Empty).Trim();
                    return string.IsNullOrWhiteSpace(activeEmail) ? fallbackEmail : activeEmail;
                }
            }
            catch (Exception ex)
            {
                CommonBL errorlog = new CommonBL();
                errorlog.fnStoreErrorLog("userDetailsBl", "GetActiveOfficialEmail", "Exception Message" + ex.Message + "Strace=" + ex.StackTrace, UserId);
            }

            return fallbackEmail;
        }

        public void SendUserCredentialsMail(string emailId, string username, string password)
        {
            try
            {
                string Email = ConfigurationManager.AppSettings["SenderEmail"];
                string Password = ConfigurationManager.AppSettings["SenderPassword"];
                int Port = Convert.ToInt32(ConfigurationManager.AppSettings["SenderPort"]);
                string Host = ConfigurationManager.AppSettings["SenderHost"];
                string subject = "HRMS Credentials";
                string body = string.IsNullOrWhiteSpace(username)
                    ? $"Your Login Password for HRMS is Password: {password}"
                    : $"Your HRMS login credentials are:\n\nUsername: {username}\nPassword: {password}";

                using (MailMessage mail = new MailMessage())
                {
                    mail.From = new MailAddress(Email, "HRMS");
                    mail.To.Add(emailId);
                    mail.Subject = subject;
                    mail.Body = body;
                    mail.IsBodyHtml = false;

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
                errorlog.fnStoreErrorLog("userDetailsBl", "SendUserCredentialsMail", "Exception Message" + ex.Message + "Strace=" + ex.StackTrace, UserId);
            }
        }

    }
}
