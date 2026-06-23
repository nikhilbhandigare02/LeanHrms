using DataObject;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Configuration;

namespace ProcessModel
{
    public class HolidayListBL
    {
        private string DBName = ConfigurationManager.AppSettings["DBName"];
        private static string MySqlconnection = ConfigurationManager.ConnectionStrings["MysqlConnection"].ConnectionString;
        protected string UserId = null;

        public List<HolidayListDO> GetHolidayList(int companyId)
        {
            List<HolidayListDO> listdata = new List<HolidayListDO>();

            try
            {
                string dbName = GetDbName();
                using (MySqlConnection connection = new MySqlConnection(string.Format(MySqlconnection, dbName)))
                using (MySqlCommand command = new MySqlCommand(
                    @"SELECT holiday_id, company_id, holiday_date, holiday_day, holiday_name, is_active
                      FROM holiday_list_master
                      WHERE company_id = @company_id AND is_active = 1
                      ORDER BY holiday_date", connection))
                {
                    command.Parameters.AddWithValue("@company_id", companyId);
                    connection.Open();
                    using (MySqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            listdata.Add(new HolidayListDO
                            {
                                holiday_id = Convert.ToInt32(reader["holiday_id"]),
                                company_id = Convert.ToInt32(reader["company_id"]),
                                holiday_date = Convert.ToDateTime(reader["holiday_date"]),
                                holiday_day = Convert.ToString(reader["holiday_day"]),
                                holiday_name = Convert.ToString(reader["holiday_name"]),
                                is_active = Convert.ToInt32(reader["is_active"])
                            });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                StoreError("GetHolidayList", ex);
            }

            return listdata;
        }

        public void DeactivateCompanyHolidays(int companyId, int updatedBy)
        {
            try
            {
                ExecuteNonQuery(
                    @"UPDATE holiday_list_master
                      SET is_active = 0, updated_by = @updated_by, updated_date = NOW()
                      WHERE company_id = @company_id AND is_active = 1",
                    new Dictionary<string, object>
                    {
                        { "@company_id", companyId },
                        { "@updated_by", updatedBy }
                    });
            }
            catch (Exception ex)
            {
                StoreError("DeactivateCompanyHolidays", ex);
                throw;
            }
        }

        public void InsertHoliday(HolidayListDO holiday)
        {
            try
            {
                ExecuteNonQuery(
                    @"INSERT INTO holiday_list_master
                      (company_id, holiday_date, holiday_day, holiday_name, is_active, inserted_by, inserted_date)
                      VALUES
                      (@company_id, @holiday_date, @holiday_day, @holiday_name, 1, @inserted_by, NOW())",
                    new Dictionary<string, object>
                    {
                        { "@company_id", holiday.company_id },
                        { "@holiday_date", holiday.holiday_date },
                        { "@holiday_day", holiday.holiday_day },
                        { "@holiday_name", holiday.holiday_name },
                        { "@inserted_by", holiday.inserted_by }
                    });
            }
            catch (Exception ex)
            {
                StoreError("InsertHoliday", ex);
                throw;
            }
        }

        public void UpdateHoliday(HolidayListDO holiday)
        {
            try
            {
                ExecuteNonQuery(
                    @"UPDATE holiday_list_master
                      SET holiday_date = @holiday_date,
                          holiday_day = @holiday_day,
                          holiday_name = @holiday_name,
                          updated_by = @updated_by,
                          updated_date = NOW()
                      WHERE holiday_id = @holiday_id AND company_id = @company_id AND is_active = 1",
                    new Dictionary<string, object>
                    {
                        { "@holiday_id", holiday.holiday_id },
                        { "@company_id", holiday.company_id },
                        { "@holiday_date", holiday.holiday_date },
                        { "@holiday_day", holiday.holiday_day },
                        { "@holiday_name", holiday.holiday_name },
                        { "@updated_by", holiday.updated_by }
                    });
            }
            catch (Exception ex)
            {
                StoreError("UpdateHoliday", ex);
                throw;
            }
        }

        public void DeleteHoliday(int holidayId, int companyId, int updatedBy)
        {
            try
            {
                ExecuteNonQuery(
                    @"UPDATE holiday_list_master
                      SET is_active = 0, updated_by = @updated_by, updated_date = NOW()
                      WHERE holiday_id = @holiday_id AND company_id = @company_id",
                    new Dictionary<string, object>
                    {
                        { "@holiday_id", holidayId },
                        { "@company_id", companyId },
                        { "@updated_by", updatedBy }
                    });
            }
            catch (Exception ex)
            {
                StoreError("DeleteHoliday", ex);
                throw;
            }
        }

        private void ExecuteNonQuery(string query, Dictionary<string, object> parameters)
        {
            string dbName = GetDbName();
            using (MySqlConnection connection = new MySqlConnection(string.Format(MySqlconnection, dbName)))
            using (MySqlCommand command = new MySqlCommand(query, connection))
            {
                command.CommandTimeout = 0;
                foreach (KeyValuePair<string, object> parameter in parameters)
                {
                    command.Parameters.AddWithValue(parameter.Key, parameter.Value ?? DBNull.Value);
                }

                connection.Open();
                command.ExecuteNonQuery();
            }
        }

        private string GetDbName()
        {
            return string.IsNullOrEmpty(DBName) ? ConfigurationManager.AppSettings["DBName"] : DBName;
        }

        private void StoreError(string methodName, Exception ex)
        {
            CommonBL errorlog = new CommonBL();
            errorlog.fnStoreErrorLog(
                "HolidayListBL",
                methodName,
                "Exception Message: " + ex.Message + " | StackTrace=" + ex.StackTrace,
                UserId
            );
        }
    }
}
