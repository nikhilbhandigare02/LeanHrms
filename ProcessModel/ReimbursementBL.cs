using DataObject;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Web;

namespace ProcessModel
{
    [Serializable]
    public class StatusOptionItem
    {
        public int Id { get; set; }
        public string Value { get; set; }
        public string Text { get; set; }
    }

    [Serializable]
    public class MonthOptionItem
    {
        public int Value { get; set; }
        public string Text { get; set; }
    }

    public class ReimbursementBL
    {
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
                errorlog.fnStoreErrorLog("ReimbursementBL", "BindLookupData", "Exception Message: " + ex.Message + " StackTrace: " + ex.StackTrace, null);
            }

            return items;
        }

        public List<StatusOptionItem> GetStatusOptions()
        {
            var options = new List<StatusOptionItem>();
            //var lookupData = BindLookupData("LeaveStatus");
            var lookupData = BindLookupData("Reimburesementstatus");

            if (lookupData != null && lookupData.Count > 0)
            {
                foreach (var item in lookupData)
                {
                    options.Add(new StatusOptionItem
                    {
                        Id = item.Id,
                        Value = item.Value,
                        Text = item.Text
                    });
                }
            }
            else
            {
                // Fallback to hardcoded values if lookup data not found
                options = new List<StatusOptionItem>
                {
                    new StatusOptionItem { Id = 1, Value = "Pending", Text = "Pending" },
                    new StatusOptionItem { Id = 2, Value = "Accepted", Text = "Approved" },
                    new StatusOptionItem { Id = 3, Value = "Rejected", Text = "Rejected" }
                };
            }
            return options;
        }

        public List<MonthOptionItem> GetMonthOptions()
        {
            var options = new List<MonthOptionItem>();
            var months = System.Globalization.CultureInfo.CurrentCulture.DateTimeFormat.MonthNames;
            int staticYear = 2026;

            for (int i = 0; i < months.Length; i++)
            {
                if (!string.IsNullOrEmpty(months[i]))
                {
                    options.Add(new MonthOptionItem
                    {
                        Value = i + 1,
                        Text = $"{months[i]} {staticYear}"
                    });
                }
            }
            return options;
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

        private string NormalizeMySqlConnectionString(string connectionString)
        {
            if (string.IsNullOrWhiteSpace(connectionString))
            {
                return connectionString;
            }

            try
            {
                var builder = new MySql.Data.MySqlClient.MySqlConnectionStringBuilder(connectionString);
                return builder.ToString();
            }
            catch
            {
                return connectionString;
            }
        }

        public List<ReimbursementDetailsDO> GetReimbursementDetailsList()
        {
            List<ReimbursementDetailsDO> list = new List<ReimbursementDetailsDO>();

            try
            {
                using (MySqlDataReader dr =
                    DataClass.GetDataReaderFromSp(
                        "",
                        "sp_reimbursement_details"))
                {
                    if (dr != null)
                    {
                        while (dr.Read())
                        {
                            ReimbursementDetailsDO item = new ReimbursementDetailsDO();

                            // Map all properties manually
                            for (int i = 0; i < dr.FieldCount; i++)
                            {
                                string colName = dr.GetName(i).ToLower();
                                object val = dr[i];

                                if (val == DBNull.Value)
                                    continue;

                                switch (colName)
                                {
                                    case "reimbursementnumber":
                                        item.reimbursementNumber = Convert.ToString(val);
                                        break;
                                    case "claimtype":
                                        item.claimType = Convert.ToString(val);
                                        break;
                                    case "claimdate":
                                        item.claimDate = Convert.ToDateTime(val);
                                        break;
                                    case "payment_month":
                                        item.paymentMonth = Convert.ToString(val);
                                        break;
                                    case "claimamount":
                                        item.claimAmount = Convert.ToDecimal(val);
                                        break;
                                    case "document":
                                        item.document = Convert.ToString(val);
                                        break;
                                    case "remarks":
                                        item.remarks = Convert.ToString(val);
                                        break;
                                    case "status":
                                        item.status = Convert.ToString(val);
                                        break;
                                    case "userfullname":
                                        item.userFullName = Convert.ToString(val);
                                        break;
                                }
                            }

                            list.Add(item);
                        }
                    }
                }
            }
            catch (Exception)
            {
                // Log the error if needed, but return empty list
            }

            return list;
        }

        // Reimbursement documents for a user, via sp_getEmpReimbursementDocuments
        // (Database\sp_getEmpReimbursementDocuments.sql).
        public List<ReimbursementDocumentDO> GetReimbursementDocuments(int userId,string reimbursement_number)
        {
            List<ReimbursementDocumentDO> list = new List<ReimbursementDocumentDO>();

            try
            {
                List<MySqlParameter> parameters = new List<MySqlParameter>
                {
                    DataClass.GetParameter("p_user_id", userId),
                     DataClass.GetParameter("p_reimbursement_number", reimbursement_number)
                };

                using (MySqlDataReader dr =
                    DataClass.GetDataReaderFromSpWithParam(
                        parameters,
                        "",
                        "sp_getEmpReimbursementDocuments"))
                {
                    if (dr != null)
                    {
                        while (dr.Read())
                        {
                            ReimbursementDocumentDO item = new ReimbursementDocumentDO();

                            for (int i = 0; i < dr.FieldCount; i++)
                            {
                                string colName = dr.GetName(i).ToLower();
                                object val = dr[i];

                                if (val == DBNull.Value)
                                    continue;

                                switch (colName)
                                {
                                    case "userdocdetid":
                                        item.UserDocDetId = Convert.ToInt32(val);
                                        break;
                                    case "userid":
                                        item.UserId = Convert.ToInt32(val);
                                        break;
                                    case "documentmasterid":
                                        item.DocumentMasterId = Convert.ToInt32(val);
                                        break;
                                    case "filepath":
                                        item.filepath = Convert.ToString(val);
                                        break;
                                    case "filename":
                                        item.FileName = Convert.ToString(val);
                                        break;
                                    case "fileextension":
                                        item.FileExtension = Convert.ToString(val);
                                        break;
                                    case "insertedby":
                                        item.InsertedBy = Convert.ToInt32(val);
                                        break;
                                    case "inserteddate":
                                        item.InsertedDate = Convert.ToDateTime(val);
                                        break;
                                    case "documenttype":
                                        item.DocumentType = Convert.ToString(val);
                                        break;
                                }
                            }

                            list.Add(item);
                        }
                    }
                }
            }
            catch (Exception)
            {
                // Log the error if needed, but return empty list
            }

            return list;
        }

        // Resolves the owning user and reimbursement number for a reimbursement_id,
        // via sp_get_reimbursement_owner_by_id (Database\sp_get_reimbursement_owner_by_id.sql).
        public ReimbursementOwnerDO GetReimbursementOwnerById(int reimbursementId)
        {
            ReimbursementOwnerDO result = null;

            try
            {
                List<MySqlParameter> parameters = new List<MySqlParameter>
                {
                    DataClass.GetParameter("p_reimbursement_id", reimbursementId)
                };

                using (MySqlDataReader dr =
                    DataClass.GetDataReaderFromSpWithParam(
                        parameters,
                        "",
                        "sp_get_reimbursement_owner_by_id"))
                {
                    if (dr != null && dr.Read())
                    {
                        result = new ReimbursementOwnerDO
                        {
                            ReimbursementId = Convert.ToInt32(dr["ReimbursementId"]),
                            UserId = Convert.ToInt32(dr["UserId"]),
                            ReimbursementNumber = Convert.ToString(dr["ReimbursementNumber"])
                        };
                    }
                }
            }
            catch (Exception)
            {
                // Log the error if needed
            }

            return result;
        }

        public ResponseDO DeleteReimbursementDetails(string reimbursementNumber)
        {
            ResponseDO response = new ResponseDO();
            try
            {
                int userId = Convert.ToInt32(HttpContext.Current.Session["UserID"] ?? HttpContext.Current.Session["userId"] ?? 0);
                
                List<MySqlParameter> parameters = new List<MySqlParameter>
                {
                    DataClass.GetParameter("p_reimbursement_number", reimbursementNumber),
                    DataClass.GetParameter("p_user_id", userId)
                };

                MySqlDataReader dr =
                    DataClass.GetDataReaderFromSpWithParam(
                        parameters,
                        "",
                        "sp_delete_reimbursement_details");

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

        public ReimbursementDetailsDO GetReimbursementDetailsById(string reimbursementNumber)
        {
            List<MySqlParameter> param = new List<MySqlParameter>();
            ReimbursementDetailsDO result = null;

            param.Add(DataClass.GetParameter(
                "p_reimbursement_number",
                reimbursementNumber));

            try
            {
                using (MySqlDataReader dr =
                    DataClass.GetDataReaderFromSpWithParam(
                        param,
                        "",
                        "sp_reimbursement_detailsbyID"))
                {
                    if (dr != null && dr.HasRows && dr.Read())
                    {
                        result = new ReimbursementDetailsDO();

                        // Map all properties manually
                        for (int i = 0; i < dr.FieldCount; i++)
                        {
                            string colName = dr.GetName(i).ToLower();
                            object val = dr[i];

                            if (val == DBNull.Value)
                                continue;

                            switch (colName)
                            {
                                case "reimbursementnumber":
                                    result.reimbursementNumber = Convert.ToString(val);
                                    break;
                                case "claimtype":
                                    result.claimType = Convert.ToString(val);
                                    break;
                                case "claimdate":
                                    result.claimDate = Convert.ToDateTime(val);
                                    break;
                                case "payment_month":
                                    result.paymentMonth = Convert.ToString(val);
                                    break;
                                case "claimamount":
                                    result.claimAmount = Convert.ToDecimal(val);
                                    break;
                                case "document":
                                    result.document = Convert.ToString(val);
                                    break;
                                case "remarks":
                                    result.remarks = Convert.ToString(val);
                                    break;
                                case "status":
                                    result.status = Convert.ToString(val);
                                    break;
                            }
                        }
                    }
                }
            }
            catch (Exception)
            {
                // Log if needed
            }

            return result;
        }

        public ResponseDO SaveReimbursementDetails(ReimbursementDetailsDO reimbursement)
        {
            ResponseDO response = new ResponseDO();

            try
            {
                int loggedInUserId = Convert.ToInt32(HttpContext.Current.Session["UserID"] ?? HttpContext.Current.Session["userId"] ?? 0);
                
                List<MySqlParameter> parameters = new List<MySqlParameter>
                {
                    DataClass.GetParameter("p_reimbursement_number", reimbursement.reimbursementNumber),
                    DataClass.GetParameter("p_claim_type", reimbursement.claimType),
                    DataClass.GetParameter("p_claim_date", reimbursement.claimDate),
                    DataClass.GetParameter("p_payment_month", reimbursement.paymentMonth),
                    DataClass.GetParameter("p_claim_amount", reimbursement.claimAmount),
                    DataClass.GetParameter("p_document", reimbursement.document),
                    DataClass.GetParameter("p_remarks", reimbursement.remarks),
                    DataClass.GetParameter("p_status", reimbursement.status),
                    DataClass.GetParameter("p_created_by", loggedInUserId)
                };

                MySqlDataReader dr =
                    DataClass.GetDataReaderFromSpWithParam(
                        parameters,
                        "",
                        "sp_save_reimbursement_details");

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

        public ResponseDO UpdateReimbursementDetails(ReimbursementDetailsDO reimbursement)
        {
            ResponseDO response = new ResponseDO();

            try
            {
                int loggedInUserId = Convert.ToInt32(HttpContext.Current.Session["UserID"] ?? HttpContext.Current.Session["userId"] ?? 0);
                
                List<MySqlParameter> parameters = new List<MySqlParameter>
                {
                    DataClass.GetParameter("p_reimbursement_number", reimbursement.reimbursementNumber),
                 
                    DataClass.GetParameter("p_payment_month", reimbursement.paymentMonth),
                   
                    DataClass.GetParameter("p_remarks", reimbursement.remarks),
                    DataClass.GetParameter("p_status", reimbursement.status),
                    DataClass.GetParameter("p_updated_by", loggedInUserId)
                };

                MySqlDataReader dr =
                    DataClass.GetDataReaderFromSpWithParam(
                        parameters,
                        "",
                        "sp_update_reimbursement_details");

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


public ResponseDO UpdateReimbursementStatus(string reimbursementNumber, int status, string remarks, string paymentMonth)
    {
        ResponseDO response = new ResponseDO();

        try
        {
            int userId = Convert.ToInt32(HttpContext.Current.Session["UserID"] ?? HttpContext.Current.Session["userId"] ?? 0);

            string connStr = ConfigurationManager.ConnectionStrings["Sqlconnection"].ConnectionString;

            using (MySqlConnection con = new MySqlConnection(connStr))
            {
                using (MySqlCommand cmd = new MySqlCommand("sp_update_reimbursement_status", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValue("p_reimbursement_number", reimbursementNumber);
                    cmd.Parameters.AddWithValue("p_status", status);
                    cmd.Parameters.AddWithValue("p_remarks", string.IsNullOrEmpty(remarks) ? (object)DBNull.Value : remarks);
                    cmd.Parameters.AddWithValue("p_payment_month", string.IsNullOrEmpty(paymentMonth) ? (object)DBNull.Value : paymentMonth);
                    cmd.Parameters.AddWithValue("p_updated_by", userId);

                    con.Open();

                    using (MySqlDataReader dr = cmd.ExecuteReader())
                    {
                        if (dr.Read())
                        {
                            string statusValue = dr["Status"].ToString();
                            response.Status = statusValue.Equals("Success", StringComparison.OrdinalIgnoreCase) ? 1 : 0;
                            response.message = dr["Remarks"].ToString();
                        }
                    }
                }
            }
        }
        catch (Exception ex)
        {
            response.Status = -1;
            response.message = ex.Message;
        }

        return response;
    }
}
}
