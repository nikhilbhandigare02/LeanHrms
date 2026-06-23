using DataObject;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.Data;

namespace ProcessModel
{
    public class EmployeeOnboardingBL
    {
        protected string UserId = null;
        private static string Sqlconnection = ConfigurationManager.ConnectionStrings["Sqlconnection"] != null
            ? ConfigurationManager.ConnectionStrings["Sqlconnection"].ConnectionString
            : string.Empty;

        public List<DropDownData> BindLookupData(string lookupType)
        {
            List<DropDownData> items = new List<DropDownData>();
            if (string.IsNullOrWhiteSpace(Sqlconnection) || string.IsNullOrWhiteSpace(lookupType))
            {
                return items;
            }

            try
            {
                using (MySqlConnection con = new MySqlConnection(NormalizeMySqlConnectionString(Sqlconnection)))
                using (MySqlCommand cmd = new MySqlCommand("sp_bindLookupData", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@p_lookupType", lookupType);

                    con.Open();
                    using (MySqlDataReader dr = cmd.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            items.Add(new DropDownData
                            {
                                Id = ReadInt(dr, "id"),
                                Value = ReadString(dr, "value"),
                                Text = ReadString(dr, "name")
                            });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                CommonBL errorlog = new CommonBL();
                errorlog.fnStoreErrorLog("EmployeeOnboardingBL", "BindLookupData", "Exception Message: " + ex.Message + " StackTrace: " + ex.StackTrace, UserId);
            }

            return items;
        }

        public List<DropDownData> BindStoredProcedureDropdown(string storedProcedureName, string lookupType = null)
        {
            List<DropDownData> items = new List<DropDownData>();
            if (string.IsNullOrWhiteSpace(Sqlconnection) || string.IsNullOrWhiteSpace(storedProcedureName))
            {
                return items;
            }

            try
            {
                using (MySqlConnection con = new MySqlConnection(NormalizeMySqlConnectionString(Sqlconnection)))
                using (MySqlCommand cmd = new MySqlCommand(storedProcedureName, con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    if (!string.IsNullOrWhiteSpace(lookupType))
                    {
                        cmd.Parameters.AddWithValue("@p_lookupType", lookupType);
                    }

                    con.Open();
                    using (MySqlDataReader dr = cmd.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            string id = ReadFirstAvailable(dr, "id", "Id", "ID", "user_id", "userloginid", "company_id", "designation_id");
                            string name = ReadFirstAvailable(dr, "name", "Name", "Text", "text", "user_fullname", "company_name", "designation_name");
                            if (string.IsNullOrWhiteSpace(name))
                            {
                                name = id;
                            }

                            items.Add(new DropDownData
                            {
                                Id = ParseInt(id),
                                Text = name
                            });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                CommonBL errorlog = new CommonBL();
                errorlog.fnStoreErrorLog("EmployeeOnboardingBL", "BindStoredProcedureDropdown", "SP=" + storedProcedureName + " Exception Message: " + ex.Message + " StackTrace: " + ex.StackTrace, UserId);
            }

            return items;
        }

        public EmployeeOnboardingResponseDO SaveEmployeeOnboarding(EmployeeOnboardingDO employee)
        {
            EmployeeOnboardingResponseDO response = new EmployeeOnboardingResponseDO
            {
                Status = "Failed",
                Message = "Employee onboarding save failed.",
                UserId = 0
            };

            if (employee == null || string.IsNullOrWhiteSpace(Sqlconnection))
            {
                response.Message = "Employee data or secondary database connection is missing.";
                return response;
            }

            try
            {
                using (MySqlConnection con = new MySqlConnection(NormalizeMySqlConnectionString(Sqlconnection)))
                using (MySqlCommand cmd = new MySqlCommand("SP_SaveEmployeeOnboarding", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandTimeout = 0;

                    cmd.Parameters.AddWithValue("@p_user_id", employee.UserId);
                    cmd.Parameters.AddWithValue("@p_emp_code", employee.EmployeeCode);
                    cmd.Parameters.AddWithValue("@p_username", employee.Username);
                    cmd.Parameters.AddWithValue("@p_fullname", employee.FullName);
                    cmd.Parameters.AddWithValue("@p_email", employee.Email);
                    cmd.Parameters.AddWithValue("@p_contact", employee.Contact);
                    cmd.Parameters.AddWithValue("@p_gender", ToDbIntValue(employee.Gender));
                    cmd.Parameters.AddWithValue("@p_dob", ToDbValue(employee.DateOfBirth));
                    cmd.Parameters.AddWithValue("@p_marital_status", ToDbIntValue(employee.MaritalStatus));
                    cmd.Parameters.AddWithValue("@p_blood_group", ToDbIntValue(employee.BloodGroup));
                    cmd.Parameters.AddWithValue("@p_nationality", ToDbIntValue(employee.Nationality));
                    cmd.Parameters.AddWithValue("@p_employment_type", ToDbIntValue(employee.EmploymentType));
                    cmd.Parameters.AddWithValue("@p_employee_category", ToDbIntValue(employee.EmployeeCategory));
                    cmd.Parameters.AddWithValue("@p_joining_date", ToDbValue(employee.JoiningDate));
                    cmd.Parameters.AddWithValue("@p_confirmation_date", ToDbValue(employee.ConfirmationDate));
                    cmd.Parameters.AddWithValue("@p_probation_end_date", ToDbValue(employee.ProbationEndDate));
                    cmd.Parameters.AddWithValue("@p_retirement_date", ToDbValue(employee.RetirementDate));
                    cmd.Parameters.AddWithValue("@p_probation_months", employee.ProbationMonths);
                    cmd.Parameters.AddWithValue("@p_employee_status", ToDbIntValue(employee.EmployeeStatus));
                    cmd.Parameters.AddWithValue("@p_notice_period", employee.NoticePeriod);
                    cmd.Parameters.AddWithValue("@p_exit_date", ToDbValue(employee.ExitDate));
                    cmd.Parameters.AddWithValue("@p_separation_reason", ToDbIntValue(employee.SeparationReason));
                    cmd.Parameters.AddWithValue("@p_company", ToDbIntValue(employee.Company));
                    cmd.Parameters.AddWithValue("@p_department", ToDbIntValue(employee.Department));
                    cmd.Parameters.AddWithValue("@p_branch_office", employee.BranchOffice);
                    cmd.Parameters.AddWithValue("@p_location", employee.Location);
                    cmd.Parameters.AddWithValue("@p_reporting_manager_id", employee.ReportingManagerId);
                    cmd.Parameters.AddWithValue("@p_functional_manager_id", employee.FunctionalManagerId);
                    cmd.Parameters.AddWithValue("@p_hod_id", employee.HodId);
                    cmd.Parameters.AddWithValue("@p_designation", ToDbIntValue(employee.Designation));
                    cmd.Parameters.AddWithValue("@p_employee_level", ToDbIntValue(employee.EmployeeLevel));
                    cmd.Parameters.AddWithValue("@p_attendance_type", ToDbIntValue(employee.AttendanceType));
                    cmd.Parameters.AddWithValue("@p_weekly_off", ToDbIntValue(employee.WeeklyOff));
                    cmd.Parameters.AddWithValue("@p_working_hours", ToDbStringValue(employee.WorkingHours));
                    cmd.Parameters.AddWithValue("@p_attendance_policy", ToDbIntValue(employee.AttendancePolicy));
                    cmd.Parameters.AddWithValue("@p_punching_device_id", employee.PunchingDeviceId);
                    cmd.Parameters.AddWithValue("@p_biometric_id", employee.BiometricId);
                    cmd.Parameters.AddWithValue("@p_overtime_eligible", employee.OvertimeEligible);
                    cmd.Parameters.AddWithValue("@p_overtime_rate", employee.OvertimeRate);
                    cmd.Parameters.AddWithValue("@p_work_location", ToDbIntValue(employee.WorkLocation));
                    cmd.Parameters.AddWithValue("@p_remote_work_allowed", ToDbIntValue(employee.RemoteWorkAllowed));
                    // Payroll is not part of Employee Registration submit.
                    // cmd.Parameters.AddWithValue("@p_salary_structure", employee.SalaryStructure);
                    // cmd.Parameters.AddWithValue("@p_basic_salary", employee.BasicSalary);
                    // cmd.Parameters.AddWithValue("@p_gross_salary", employee.GrossSalary);
                    // cmd.Parameters.AddWithValue("@p_ctc", employee.Ctc);
                    // cmd.Parameters.AddWithValue("@p_salary_effective_date", ToDbValue(employee.SalaryEffectiveDate));
                    // cmd.Parameters.AddWithValue("@p_payment_mode", employee.PaymentMode);
                    // cmd.Parameters.AddWithValue("@p_uan_number", employee.UanNumber);
                    // cmd.Parameters.AddWithValue("@p_pf_number", employee.PfNumber);
                    // cmd.Parameters.AddWithValue("@p_esic_number", employee.EsicNumber);
                    // cmd.Parameters.AddWithValue("@p_professional_tax_number", employee.ProfessionalTaxNumber);
                    // cmd.Parameters.AddWithValue("@p_labour_welfare_fund_number", employee.LabourWelfareFundNumber);
                    // cmd.Parameters.AddWithValue("@p_tax_regime", employee.TaxRegime);
                    // cmd.Parameters.AddWithValue("@p_tds_applicable", employee.TdsApplicable);
                    cmd.Parameters.AddWithValue("@p_inserted_by", employee.InsertedBy);

                    con.Open();
                    using (MySqlDataReader dr = cmd.ExecuteReader())
                    {
                        if (dr.Read())
                        {
                            response.Status = ReadString(dr, "Status");
                            response.Message = ReadString(dr, "Message");
                            response.UserId = ReadInt(dr, "UserId");
                            response.Username = ReadString(dr, "Username");
                            response.AutoGeneratedPassword = ReadString(dr, "AutoGeneratedPassword");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                CommonBL errorlog = new CommonBL();
                errorlog.fnStoreErrorLog("EmployeeOnboardingBL", "SaveEmployeeOnboarding", "Exception Message: " + ex.Message + " StackTrace: " + ex.StackTrace, UserId);
                response.Message = "Employee onboarding save failed due to an exception: " + ex.Message;
            }

            return response;
        }

        public EmployeeOnboardingResponseDO SaveEmployeeAsset(int userId, EmployeeAssetDO asset, int insertedBy)
        {
            EmployeeOnboardingResponseDO response = new EmployeeOnboardingResponseDO
            {
                Status = "Failed",
                Message = "Employee asset save failed.",
                UserId = userId
            };

            if (asset == null || string.IsNullOrWhiteSpace(Sqlconnection))
            {
                response.Message = "Employee asset data or secondary database connection is missing.";
                return response;
            }

            try
            {
                using (MySqlConnection con = new MySqlConnection(NormalizeMySqlConnectionString(Sqlconnection)))
                using (MySqlCommand cmd = new MySqlCommand("SP_SaveEmployeeAsset", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandTimeout = 0;

                    cmd.Parameters.AddWithValue("@p_asset_assignment_id", 0);
                    cmd.Parameters.AddWithValue("@p_user_id", userId);
                    cmd.Parameters.AddWithValue("@p_asset_type", asset.AssetType);
                    cmd.Parameters.AddWithValue("@p_asset_number", asset.AssetNumber);
                    cmd.Parameters.AddWithValue("@p_asset_name", asset.AssetName);
                    cmd.Parameters.AddWithValue("@p_assigned_date", ToDbValue(asset.AssignedDate));
                    cmd.Parameters.AddWithValue("@p_return_date", ToDbValue(asset.ReturnDate));
                    cmd.Parameters.AddWithValue("@p_asset_condition", ToDbIntValue(asset.AssetCondition));
                    cmd.Parameters.AddWithValue("@p_asset_status", ToDbIntValue(asset.AssetStatus));
                    cmd.Parameters.AddWithValue("@p_inserted_by", insertedBy);

                    con.Open();
                    using (MySqlDataReader dr = cmd.ExecuteReader())
                    {
                        if (dr.Read())
                        {
                            response.Status = ReadString(dr, "Status");
                            response.Message = ReadString(dr, "Message");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                CommonBL errorlog = new CommonBL();
                errorlog.fnStoreErrorLog("EmployeeOnboardingBL", "SaveEmployeeAsset", "Exception Message: " + ex.Message + " StackTrace: " + ex.StackTrace, UserId);
                response.Message = "Employee asset save failed due to an exception: " + ex.Message;
            }

            return response;
        }

        public EmployeeOnboardingResponseDO UpdateEmployeeAsset(int assetId, int userId, EmployeeAssetDO asset, int updatedBy)
        {
            EmployeeOnboardingResponseDO response = new EmployeeOnboardingResponseDO
            {
                Status = "Failed",
                Message = "Employee asset update failed.",
                UserId = userId
            };

            if (assetId <= 0 || asset == null || string.IsNullOrWhiteSpace(Sqlconnection))
            {
                response.Message = "Employee asset id, data, or secondary database connection is missing.";
                return response;
            }

            try
            {
                using (MySqlConnection con = new MySqlConnection(NormalizeMySqlConnectionString(Sqlconnection)))
                using (MySqlCommand cmd = new MySqlCommand("SP_SaveEmployeeAsset", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandTimeout = 0;

                    cmd.Parameters.AddWithValue("@p_asset_assignment_id", assetId);
                    cmd.Parameters.AddWithValue("@p_user_id", userId);
                    cmd.Parameters.AddWithValue("@p_asset_type", asset.AssetType);
                    cmd.Parameters.AddWithValue("@p_asset_number", asset.AssetNumber);
                    cmd.Parameters.AddWithValue("@p_asset_name", asset.AssetName);
                    cmd.Parameters.AddWithValue("@p_assigned_date", ToDbValue(asset.AssignedDate));
                    cmd.Parameters.AddWithValue("@p_return_date", ToDbValue(asset.ReturnDate));
                    cmd.Parameters.AddWithValue("@p_asset_condition", ToDbIntValue(asset.AssetCondition));
                    cmd.Parameters.AddWithValue("@p_asset_status", ToDbIntValue(asset.AssetStatus));
                    cmd.Parameters.AddWithValue("@p_inserted_by", updatedBy);

                    con.Open();
                    using (MySqlDataReader dr = cmd.ExecuteReader())
                    {
                        if (dr.Read())
                        {
                            response.Status = ReadString(dr, "Status");
                            response.Message = ReadString(dr, "Message");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                CommonBL errorlog = new CommonBL();
                errorlog.fnStoreErrorLog("EmployeeOnboardingBL", "UpdateEmployeeAsset", "Exception Message: " + ex.Message + " StackTrace: " + ex.StackTrace, UserId);
                response.Message = "Employee asset update failed due to an exception: " + ex.Message;
            }

            return response;
        }

        public EmployeeOnboardingResponseDO SaveEmployeeEducation(int educationId, int userId, EmployeeEducationDetailsDO education, int insertedBy)
        {
            EmployeeOnboardingResponseDO response = new EmployeeOnboardingResponseDO
            {
                Status = "Failed",
                Message = "Employee education save failed.",
                UserId = userId
            };

            if (education == null || string.IsNullOrWhiteSpace(Sqlconnection))
            {
                response.Message = "Employee education data or secondary database connection is missing.";
                return response;
            }

            try
            {
                using (MySqlConnection con = new MySqlConnection(NormalizeMySqlConnectionString(Sqlconnection)))
                using (MySqlCommand cmd = new MySqlCommand("SP_SaveEmployeeEducation", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandTimeout = 0;

                    cmd.Parameters.AddWithValue("@p_education_id", educationId);
                    cmd.Parameters.AddWithValue("@p_user_id", userId);
                    cmd.Parameters.AddWithValue("@p_qualification_level", education.QualificationLevel);
                    cmd.Parameters.AddWithValue("@p_degree_name", education.DegreeName);
                    cmd.Parameters.AddWithValue("@p_specialization", education.Specialization);
                    cmd.Parameters.AddWithValue("@p_university", education.University);
                    cmd.Parameters.AddWithValue("@p_institute_name", education.InstituteName);
                    cmd.Parameters.AddWithValue("@p_year_of_passing", education.YearOfPassing);
                    cmd.Parameters.AddWithValue("@p_percentage_cgpa", education.PercentageCgpa);
                    cmd.Parameters.AddWithValue("@p_certificate_file", education.CertificateFile);
                    cmd.Parameters.AddWithValue("@p_inserted_by", insertedBy);

                    con.Open();
                    using (MySqlDataReader dr = cmd.ExecuteReader())
                    {
                        if (dr.Read())
                        {
                            response.Status = ReadString(dr, "Status");
                            response.Message = ReadString(dr, "Message");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                CommonBL errorlog = new CommonBL();
                errorlog.fnStoreErrorLog("EmployeeOnboardingBL", "SaveEmployeeEducation", "Exception Message: " + ex.Message + " StackTrace: " + ex.StackTrace, UserId);
                response.Message = "Employee education save failed due to an exception: " + ex.Message;
            }

            return response;
        }

        public EmployeeOnboardingResponseDO SaveEmployeeWorkExperience(int experienceId, int userId, EmployeeWorkExperienceDetailsDO experience, int insertedBy)
        {
            EmployeeOnboardingResponseDO response = new EmployeeOnboardingResponseDO
            {
                Status = "Failed",
                Message = "Employee work experience save failed.",
                UserId = userId
            };

            if (experience == null || string.IsNullOrWhiteSpace(Sqlconnection))
            {
                response.Message = "Employee work experience data or secondary database connection is missing.";
                return response;
            }

            try
            {
                using (MySqlConnection con = new MySqlConnection(NormalizeMySqlConnectionString(Sqlconnection)))
                using (MySqlCommand cmd = new MySqlCommand("SP_SaveEmployeeWorkExperience", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandTimeout = 0;

                    cmd.Parameters.AddWithValue("@p_experience_id", experienceId);
                    cmd.Parameters.AddWithValue("@p_user_id", userId);
                    cmd.Parameters.AddWithValue("@p_organization_name", experience.OrganizationName);
                    cmd.Parameters.AddWithValue("@p_industry", experience.Industry);
                    cmd.Parameters.AddWithValue("@p_designation", experience.Designation);
                    cmd.Parameters.AddWithValue("@p_start_date", ToDbValue(experience.StartDate));
                    cmd.Parameters.AddWithValue("@p_end_date", ToDbValue(experience.EndDate));
                    cmd.Parameters.AddWithValue("@p_total_experience", experience.TotalExperience);
                    cmd.Parameters.AddWithValue("@p_last_drawn_salary", ToDbDecimalValue(experience.LastDrawnSalary));
                    cmd.Parameters.AddWithValue("@p_employment_period", experience.EmploymentPeriod);
                    cmd.Parameters.AddWithValue("@p_inserted_by", insertedBy);

                    con.Open();
                    using (MySqlDataReader dr = cmd.ExecuteReader())
                    {
                        if (dr.Read())
                        {
                            response.Status = ReadString(dr, "Status");
                            response.Message = ReadString(dr, "Message");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                CommonBL errorlog = new CommonBL();
                errorlog.fnStoreErrorLog("EmployeeOnboardingBL", "SaveEmployeeWorkExperience", "Exception Message: " + ex.Message + " StackTrace: " + ex.StackTrace, UserId);
                response.Message = "Employee work experience save failed due to an exception: " + ex.Message;
            }

            return response;
        }

        public EmployeeOnboardingResponseDO DeleteEmployeeAsset(int assetId, int userId, int updatedBy)
        {
            EmployeeOnboardingResponseDO response = new EmployeeOnboardingResponseDO
            {
                Status = "Failed",
                Message = "Employee asset delete failed.",
                UserId = userId
            };

            if (assetId <= 0 || string.IsNullOrWhiteSpace(Sqlconnection))
            {
                response.Message = "Employee asset id or secondary database connection is missing.";
                return response;
            }

            try
            {
                using (MySqlConnection con = new MySqlConnection(NormalizeMySqlConnectionString(Sqlconnection)))
                using (MySqlCommand cmd = new MySqlCommand("SP_DeleteEmployeeAsset", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandTimeout = 0;

                    cmd.Parameters.AddWithValue("@p_asset_assignment_id", assetId);
                    cmd.Parameters.AddWithValue("@p_user_id", userId);
                    cmd.Parameters.AddWithValue("@p_updated_by", updatedBy);

                    con.Open();
                    using (MySqlDataReader dr = cmd.ExecuteReader())
                    {
                        if (dr.Read())
                        {
                            response.Status = ReadString(dr, "Status");
                            response.Message = ReadString(dr, "Message");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                CommonBL errorlog = new CommonBL();
                errorlog.fnStoreErrorLog("EmployeeOnboardingBL", "DeleteEmployeeAsset", "Exception Message: " + ex.Message + " StackTrace: " + ex.StackTrace, UserId);
                response.Message = "Employee asset delete failed due to an exception: " + ex.Message;
            }

            return response;
        }

        public string CheckEmployeeOnboardingDuplicate(string fieldName, string fieldValue, int userId)
        {
            if (string.IsNullOrWhiteSpace(Sqlconnection) || string.IsNullOrWhiteSpace(fieldName) || string.IsNullOrWhiteSpace(fieldValue))
            {
                return string.Empty;
            }

            string columnName = string.Empty;
            string message = string.Empty;

            switch (fieldName.Trim())
            {
                case "EmployeeCode":
                    columnName = "emp_code";
                    message = "Employee Code already exists.";
                    break;
                case "Username":
                    columnName = "username";
                    message = "Username already exists.";
                    break;
                case "Email":
                    columnName = "user_mail_id";
                    message = "Email ID already exists.";
                    break;
                case "Mobile":
                    columnName = "contact_detail";
                    message = "Mobile Number already exists.";
                    break;
                default:
                    return string.Empty;
            }

            try
            {
                using (MySqlConnection con = new MySqlConnection(NormalizeMySqlConnectionString(Sqlconnection)))
                using (MySqlCommand cmd = new MySqlCommand(
                    "SELECT COUNT(1) FROM lean_mngt.userm WHERE " + columnName + " = @fieldValue AND (@userId = 0 OR user_id <> @userId)",
                    con))
                {
                    cmd.CommandType = CommandType.Text;
                    cmd.Parameters.AddWithValue("@fieldValue", fieldValue.Trim());
                    cmd.Parameters.AddWithValue("@userId", userId);

                    con.Open();
                    int count = Convert.ToInt32(cmd.ExecuteScalar());
                    return count > 0 ? message : string.Empty;
                }
            }
            catch (Exception ex)
            {
                CommonBL errorlog = new CommonBL();
                errorlog.fnStoreErrorLog("EmployeeOnboardingBL", "CheckEmployeeOnboardingDuplicate", "Exception Message: " + ex.Message + " StackTrace: " + ex.StackTrace, UserId);
                return string.Empty;
            }
        }

        private object ToDbValue(DateTime? value)
        {
            return value.HasValue ? (object)value.Value : DBNull.Value;
        }

        private object ToDbIntValue(string value)
        {
            int parsed;
            return int.TryParse(Convert.ToString(value), out parsed) && parsed > 0 ? (object)parsed : DBNull.Value;
        }

        private object ToDbStringValue(string value)
        {
            string text = Convert.ToString(value ?? string.Empty).Trim();
            return string.IsNullOrWhiteSpace(text) ? (object)DBNull.Value : text;
        }

        private object ToDbDecimalValue(string value)
        {
            decimal parsed;
            return decimal.TryParse(Convert.ToString(value), out parsed) ? (object)parsed : DBNull.Value;
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

        private int ReadInt(IDataRecord record, string columnName)
        {
            int value = 0;
            int.TryParse(ReadString(record, columnName), out value);
            return value;
        }

        private string ReadFirstAvailable(IDataRecord record, params string[] columnNames)
        {
            foreach (string columnName in columnNames)
            {
                string value = ReadString(record, columnName);
                if (!string.IsNullOrWhiteSpace(value))
                {
                    return value;
                }
            }

            return string.Empty;
        }

        private int ParseInt(string value)
        {
            int parsed = 0;
            int.TryParse(Convert.ToString(value), out parsed);
            return parsed;
        }

        private string NormalizeMySqlConnectionString(string connectionString)
        {
            if (string.IsNullOrWhiteSpace(connectionString))
            {
                return connectionString;
            }

            try
            {
                var builder = new MySqlConnectionStringBuilder(connectionString);
                if (builder.Port == 0)
                {
                    builder.Port = 3306;
                }
                builder.Database = "lean_mngt";
                return builder.ConnectionString;
            }
            catch
            {
                NameValueCollection parts = ParseConnectionString(connectionString);
                var builder = new MySqlConnectionStringBuilder();

                string server = GetConnectionValue(parts, "Server", "Data Source", "Datasource", "Host");
                string user = GetConnectionValue(parts, "User Id", "UserID", "uid", "User");
                string password = GetConnectionValue(parts, "Password", "Pwd");
                string portText = GetConnectionValue(parts, "Port");

                builder.Server = string.IsNullOrWhiteSpace(server) ? "localhost" : server;
                builder.Database = "lean_mngt";
                builder.UserID = user ?? string.Empty;
                builder.Password = password ?? string.Empty;
                builder.Port = uint.TryParse(portText, out uint parsedPort) ? parsedPort : 3306;
                builder.PersistSecurityInfo = true;
                builder.ConvertZeroDateTime = true;

                return builder.ConnectionString;
            }
        }

        private NameValueCollection ParseConnectionString(string connectionString)
        {
            NameValueCollection values = new NameValueCollection(StringComparer.OrdinalIgnoreCase);
            string[] segments = connectionString.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
            foreach (string segment in segments)
            {
                int idx = segment.IndexOf('=');
                if (idx <= 0) continue;
                string key = segment.Substring(0, idx).Trim();
                string value = segment.Substring(idx + 1).Trim();
                values[key] = value;
            }
            return values;
        }

        private string GetConnectionValue(NameValueCollection values, params string[] keys)
        {
            foreach (string key in keys)
            {
                string value = values[key];
                if (!string.IsNullOrWhiteSpace(value))
                {
                    return value;
                }
            }
            return string.Empty;
        }
    }
}
