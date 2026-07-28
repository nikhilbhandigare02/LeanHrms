using DataObject;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;
using System.Xml.Linq;
using System.Configuration;
using System.Web;


namespace ProcessModel
{
    public  class userDocumentBL
    {
        string UserId = Convert.ToString(HttpContext.Current.Session["userId"]);
        private string DBName = ConfigurationManager.AppSettings["DBName"];
        private static string MySqlconnection = ConfigurationManager.ConnectionStrings["MysqlConnection"].ConnectionString;
        public List<userDocumentsDO> SaveUserDocument(int userId, FileAttachment file, string fileExt, string basePath, string webPath)
        {
            List<userDocumentsDO> listdata = new List<userDocumentsDO>();
            getDrtolist getDrtolistParam = new getDrtolist();
            List<MySqlParameter> mysqlParameters = new List<MySqlParameter>();

            try
            {
                mysqlParameters.Add(DataClass.GetParameter("@p_user_id", userId));
                mysqlParameters.Add(DataClass.GetParameter("@p_document_master_id", file.DocumentMasterId));
                mysqlParameters.Add(DataClass.GetParameter("@p_base_path", basePath)); // folder only
                mysqlParameters.Add(DataClass.GetParameter("@p_file_name", file.FileName)); // filename without extension
                mysqlParameters.Add(DataClass.GetParameter("@p_file_extension", fileExt));
                mysqlParameters.Add(DataClass.GetParameter("@p_reference_number", file.ReferenceNumber));
                mysqlParameters.Add(DataClass.GetParameter("@p_email_id", file.EmailId));
                mysqlParameters.Add(DataClass.GetParameter("@p_inserted_by", UserId));
                mysqlParameters.Add(DataClass.GetParameter("@p_webPath", webPath));
                mysqlParameters.Add(DataClass.GetParameter("@p_DocType", "EmpDoc"));

                listdata = getDrtolistParam.getdatafromreder<userDocumentsDO>(
                    DataClass.GetDataReaderFromSpWithParam(mysqlParameters, DBName, "sp_saveUserDocument")
                );
            }
            catch (Exception ex)
            {
                CommonBL errorlog = new CommonBL();
                errorlog.fnStoreErrorLog("userDocumentBL", "SaveUserDocument",
                    "Exception Message: " + ex.Message + " StackTrace=" + ex.StackTrace, UserId);
            }

            return listdata;
        }

        public List<userDocumentsDO> GetUserDocuments(int userId)
        {
            List<userDocumentsDO> listdata = new List<userDocumentsDO>();
            getDrtolist getDrtolistParam = new getDrtolist();
            List<MySqlParameter> mysqlParameters = new List<MySqlParameter>();

            try
            {
                mysqlParameters.Add(DataClass.GetParameter("@p_user_id", userId));

                listdata = getDrtolistParam.getdatafromreder<userDocumentsDO>(
                    DataClass.GetDataReaderFromSpWithParam(mysqlParameters, DBName, "sp_getUserDocuments")
                );
            }
            catch (Exception ex)
            {
                CommonBL errorlog = new CommonBL();
                errorlog.fnStoreErrorLog("userDocumentBL", "GetUserDocuments",
                    "Exception Message: " + ex.Message + " StackTrace=" + ex.StackTrace, UserId);
            }

            return listdata;
        }
        public userDocumentsDO GetUserDocumentById(int docId)
        {
            userDocumentsDO doc = null;
            try
            {
                List<MySqlParameter> mysqlParameters = new List<MySqlParameter>
        {
            DataClass.GetParameter("@p_user_doc_det_id", docId)
        };

                var list = new getDrtolist().getdatafromreder<userDocumentsDO>(
                    DataClass.GetDataReaderFromSpWithParam(mysqlParameters, DBName, "sp_getUserDocumentById")
                );

                // Since SP returns only one record, assign first item if exists
                if (list.Count > 0)
                    doc = list[0];
            }
            catch (Exception ex)
            {
                new CommonBL().fnStoreErrorLog("userDocumentBL", "GetUserDocumentById", ex.Message + ex.StackTrace, UserId);
            }

            return doc;
        }

        // Relative location (file_path + file_name + file_extention, as stored) of the
        // most recent active document for a given user + document type, read directly
        // from alpha_hrms.user_document_details. Callers combine this with
        // EmployeeDocumentServerPath to get the physical file on disk. Used for the
        // Employee Photograph preview on AddEmployee.aspx.
        public string GetLatestDocumentRelativePath(int userId, int documentMasterId)
        {
            string relativePath = null;

            try
            {
                using (MySqlConnection con = new MySqlConnection(MySqlconnection))
                using (MySqlCommand cmd = new MySqlCommand(
                    @"SELECT file_path, file_name, file_extention
                      FROM alpha_hrms.user_document_details
                      WHERE user_id = @p_user_id
                        AND document_master_id = @p_document_master_id
                        AND is_active = 1
                      ORDER BY inserted_date DESC
                      LIMIT 1", con))
                {
                    cmd.Parameters.AddWithValue("@p_user_id", userId);
                    cmd.Parameters.AddWithValue("@p_document_master_id", documentMasterId);
                    con.Open();

                    using (MySqlDataReader dr = cmd.ExecuteReader())
                    {
                        if (dr.Read())
                        {
                            string filePath = dr["file_path"] == DBNull.Value ? string.Empty : Convert.ToString(dr["file_path"]);
                            string fileName = dr["file_name"] == DBNull.Value ? string.Empty : Convert.ToString(dr["file_name"]);
                            string fileExt = dr["file_extention"] == DBNull.Value ? string.Empty : Convert.ToString(dr["file_extention"]);

                            if (!string.IsNullOrWhiteSpace(fileName))
                            {
                                relativePath = Path.Combine(filePath, fileName + fileExt);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                CommonBL errorlog = new CommonBL();
                errorlog.fnStoreErrorLog("userDocumentBL", "GetLatestDocumentRelativePath",
                    "Exception Message: " + ex.Message + " StackTrace=" + ex.StackTrace, UserId);
            }

            return relativePath;
        }

        public userDocumentsDO DeactivateDocument(int UserDocDetId)
        {
            userDocumentsDO result = new userDocumentsDO();

            using (MySqlConnection con = new MySqlConnection(MySqlconnection))
            {
                MySqlCommand cmd = new MySqlCommand("Sp_deleteDocument", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@p_type", "DeleteDocument");
                cmd.Parameters.AddWithValue("@p_UserDocDetId", UserDocDetId);

                con.Open();
                using (MySqlDataReader dr = cmd.ExecuteReader())
                {
                    if (dr.Read())
                    {
                        result.Status = dr["Status"].ToString();
                        result.Remarks = dr["Remarks"].ToString();
                    }
                }
            }
            return result;
        }



    }
}
