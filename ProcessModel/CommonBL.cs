using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Specialized;
using System.Net;
using System.Net.Mail;
using DataObject;
using MySql.Data.MySqlClient;


namespace ProcessModel
{
    public class CommonBL
    {
        protected string UserId = null;
        private string DBName = ConfigurationManager.AppSettings["DBName"];
        private static string Sqlconnection = ConfigurationManager.ConnectionStrings["Sqlconnection"] != null
            ? ConfigurationManager.ConnectionStrings["Sqlconnection"].ConnectionString
            : string.Empty;
        public CommonDO fnStoreErrorLog(string pagename, string functionName, string Error, string UserId)
        {
            CommonDO lstComm = new CommonDO();
            getDrtolist getDrtolistParam = new getDrtolist();
            List<MySqlParameter> mysqlParameters = new List<MySqlParameter>();
            try
            {

                mysqlParameters.Add(DataClass.GetParameter("@p_pagename", pagename));
                mysqlParameters.Add(DataClass.GetParameter("@p_function_name", functionName));
                mysqlParameters.Add(DataClass.GetParameter("@p_ErrorDescription", Error));
                mysqlParameters.Add(DataClass.GetParameter("@p_user_id", UserId));
                mysqlParameters.Add(DataClass.GetParameter("@p_Type", "saveError"));
                lstComm = (from ii in getDrtolistParam.getdatafromreder<CommonDO>(DataClass.GetDataReaderFromSpWithParam(mysqlParameters, "alpha_hrms", "sp_insert_errorlog"))
                           select ii).FirstOrDefault();
            }
            catch (Exception ex)
            {
                CommonBL errorlog = new CommonBL();
                errorlog.fnStoreErrorLog("CommonBL", "fnStoreErrorLog", "Exception Message: " + ex.Message + " StackTrace: " + ex.StackTrace, UserId);
            }
            return lstComm;
        }

        public List<DropDownData> dropdownusername()
        {
            List<DropDownData> dropDownData = new List<DropDownData>();
            try
            {
                getDrtolist getDrtolistParam = new getDrtolist();
                List<MySqlParameter> mysqlParameters = new List<MySqlParameter>();
                dropDownData = getDrtolistParam.getdatafromreder<DropDownData>(DataClass.GetDataReaderFromSpWithParam(mysqlParameters, DBName, "sp_bindusername"));
            }
            catch (Exception ex)
            {
                CommonBL errorlog = new CommonBL();
                errorlog.fnStoreErrorLog("CommonBL", "dropdownusername", "Exception Message" + ex.Message + "Strace=" + ex.StackTrace, UserId);
            }
            return dropDownData;
        }

        public List<DropDownData> dropdownempcode()
        {
            List<DropDownData> dropDownData = new List<DropDownData>();
            try
            {
                getDrtolist getDrtolistParam = new getDrtolist();
                List<MySqlParameter> mysqlParameters = new List<MySqlParameter>();
                dropDownData = getDrtolistParam.getdatafromreder<DropDownData>(DataClass.GetDataReaderFromSpWithParam(mysqlParameters, DBName, "sp_bindempcode"));
            }
            catch (Exception ex)
            {
                CommonBL errorlog = new CommonBL();
                errorlog.fnStoreErrorLog("CommonBL", "dropdownempcode", "Exception Message" + ex.Message + "Strace=" + ex.StackTrace, UserId);
            }
            return dropDownData;
        }
        public List<DropDownData> dropdownComponent_Forremuneration()
        {
            List<DropDownData> dropDownData = new List<DropDownData>();
            try
            {
                getDrtolist getDrtolistParam = new getDrtolist();
                List<MySqlParameter> mysqlParameters = new List<MySqlParameter>();
                dropDownData = getDrtolistParam.getdatafromreder<DropDownData>(DataClass.GetDataReaderFromSpWithParam(mysqlParameters, DBName, "Sp_BindComponent_Forremuneration"));
            }
            catch (Exception ex)
            {
                CommonBL errorlog = new CommonBL();
                errorlog.fnStoreErrorLog("CommonBL", "dropdownComponent_Forremuneration", "Exception Message" + ex.Message + "Strace=" + ex.StackTrace, UserId);
            }
            return dropDownData;
        }
        //public List<DropDown> dropdownsearchbyRole()
        //{
        //    List<DropDown> dropDown = new List<DropDown>();
        //    try
        //    {
        //        getDrtolist getDrtolistParam = new getDrtolist();
        //        List<MySqlParameter> mysqlParameters = new List<MySqlParameter>();
        //        mysqlParameters.Add(DataClass.GetParameter("@type", "getRoleSearch"));
        //        dropDown = getDrtolistParam.getdatafromreder<DropDown>(DataClass.GetDataReaderFromSpWithParam(mysqlParameters, DBName, "Sp_searchby"));
        //    }
        //    catch (Exception ex)
        //    {
        //        CommonBL errorlog = new CommonBL();
        //        errorlog.fnStoreErrorLog("CommonBL", "dropdownsearchbyRole", "Exception Message" + ex.Message + "Strace=" + ex.StackTrace, UserId);
        //    }
        //    return dropDown;
        //}

        public List<DropDownData> dropdownroles()
        {
            List<DropDownData> dropDownData = new List<DropDownData>();
            try
            {
                getDrtolist getDrtolistParam = new getDrtolist();
                List<MySqlParameter> mysqlParameters = new List<MySqlParameter>();

                mysqlParameters.Add(DataClass.GetParameter("@p_type", "Bindrole"));
                dropDownData = getDrtolistParam.getdatafromreder<DropDownData>(DataClass.GetDataReaderFromSpWithParam(mysqlParameters, DBName, "Sp_BindRoleAndBindUser"));
            }
            catch (Exception ex)
            {
                CommonBL errorlog = new CommonBL();
                errorlog.fnStoreErrorLog("CommonBL", "dropdownroles", "Exception Message" + ex.Message + "Strace=" + ex.StackTrace, UserId);
            }
            return dropDownData;
        }
        public List<DropDownData> dropdownusers()
        {
            List<DropDownData> dropDownData = new List<DropDownData>();
            try
            {
                getDrtolist getDrtolistParam = new getDrtolist();
                List<MySqlParameter> mysqlParameters = new List<MySqlParameter>();
                mysqlParameters.Add(DataClass.GetParameter("@p_type", "Binduser"));
                dropDownData = getDrtolistParam.getdatafromreder<DropDownData>(DataClass.GetDataReaderFromSpWithParam(mysqlParameters, DBName, "Sp_BindRoleAndBindUser"));
            }
            catch (Exception ex)
            {
                CommonBL errorlog = new CommonBL();
                errorlog.fnStoreErrorLog("CommonBL", "dropdownusers", "Exception Message" + ex.Message + "Strace=" + ex.StackTrace, UserId);
            }
            return dropDownData;
        }
        public List<DropDownData> dropdownMenu(string type, string menuId)
        {
            List<DropDownData> dropDownData = new List<DropDownData>();
            try
            {
                getDrtolist getDrtolistParam = new getDrtolist();
                List<MySqlParameter> mysqlParameters = new List<MySqlParameter>();
                mysqlParameters.Add(DataClass.GetParameter("@p_type", type));
                mysqlParameters.Add(
                    DataClass.GetParameter("@p_menuid", string.IsNullOrEmpty(menuId) ? DBNull.Value : (object)menuId)
                );


                dropDownData = getDrtolistParam.getdatafromreder<DropDownData>(DataClass.GetDataReaderFromSpWithParam(mysqlParameters, DBName, "SP_BindMenuAndSubmenu"));
            }
            catch (Exception ex)
            {
                CommonBL errorlog = new CommonBL();
                errorlog.fnStoreErrorLog("CommonBL", "dropdownMenu", "Exception Message" + ex.Message + "Strace=" + ex.StackTrace, UserId);
            }
            return dropDownData;
        }
        public List<DropDownData> dropdownSubMenu(string type, string menuId)
        {
            List<DropDownData> dropDownData = new List<DropDownData>();
            try
            {
                getDrtolist getDrtolistParam = new getDrtolist();
                List<MySqlParameter> mysqlParameters = new List<MySqlParameter>();
                mysqlParameters.Add(DataClass.GetParameter("@p_type", type));
                mysqlParameters.Add(DataClass.GetParameter("@p_menuid", menuId));
                dropDownData = getDrtolistParam.getdatafromreder<DropDownData>(DataClass.GetDataReaderFromSpWithParam(mysqlParameters, DBName, "SP_BindMenuAndSubmenu"));
            }
            catch (Exception ex)
            {
                CommonBL errorlog = new CommonBL();
                errorlog.fnStoreErrorLog("CommonBL", "dropdownSubMenu", "Exception Message" + ex.Message + "Strace=" + ex.StackTrace, UserId);
            }
            return dropDownData;
        }

        public List<DropDownData> dropdownDocuments()
        {
            List<DropDownData> dropDownData = new List<DropDownData>();
            try
            {
                getDrtolist getDrtolistParam = new getDrtolist();
                List<MySqlParameter> mysqlParameters = new List<MySqlParameter>();               
                dropDownData = getDrtolistParam.getdatafromreder<DropDownData>(DataClass.GetDataReaderFromSpWithParam(mysqlParameters, DBName, "sp_bindDocuments"));
            }
            catch (Exception ex)
            {
                CommonBL errorlog = new CommonBL();
                errorlog.fnStoreErrorLog("CommanBL", "dropdownDocuments", "Exception Message" + ex.Message + "Strace=" + ex.StackTrace, UserId);
            }
            return dropDownData;
        }

        public List<DropDownData> dropdownDesigntion()
        {
            List<DropDownData> dropDownData = new List<DropDownData>();
            try
            {
                getDrtolist getDrtolistParam = new getDrtolist();
                List<MySqlParameter> mysqlParameters = new List<MySqlParameter>();                //sqlParameters.Add(DataClass.GetParameter("@type", "BindGender"));
                //sqlParameters.Add(DataClass.GetParameter("@type", "BindGender"));
                dropDownData = getDrtolistParam.getdatafromreder<DropDownData>(DataClass.GetDataReaderFromSpWithParam(mysqlParameters, DBName, "sp_bindDesignation"));
            }
            catch (Exception ex)
            {
                CommonBL errorlog = new CommonBL();
                errorlog.fnStoreErrorLog("CommanBL", "dropdownDesigntion", "Exception Message" + ex.Message + "Strace=" + ex.StackTrace, UserId);
            }
            return dropDownData;
        }

        // Renders the onboarding letter (Offer/Appointment/Confirmation) for the given
        // employee + category as HTML, entirely from sp_get_onboarding_document_html -
        // SendDocuments.aspx converts this to a PDF and attaches it, no manual file
        // upload or C#-side letter wording involved.
        public string GetOnboardingDocumentHtml(int userId, string documentCategory, string designation, DateTime? effectiveDate, string additionalDetails, string candidateName = null)
        {
            string letterHtml = null;
            try
            {
                List<MySqlParameter> mysqlParameters = new List<MySqlParameter>();
                mysqlParameters.Add(DataClass.GetParameter("@p_user_id", userId));
                mysqlParameters.Add(DataClass.GetParameter("@p_document_category", documentCategory ?? string.Empty));
                mysqlParameters.Add(DataClass.GetParameter("@p_designation", designation ?? string.Empty));
                mysqlParameters.Add(DataClass.GetParameter("@p_effective_date", effectiveDate.HasValue ? (object)effectiveDate.Value : DBNull.Value));
                mysqlParameters.Add(DataClass.GetParameter("@p_additional_details", additionalDetails ?? string.Empty));
                mysqlParameters.Add(DataClass.GetParameter("@p_candidate_name", candidateName ?? string.Empty));

                getDrtolist getDrtolistParam = new getDrtolist();
                List<OnboardingDocumentHtmlDO> result = getDrtolistParam.getdatafromreder<OnboardingDocumentHtmlDO>(
                    DataClass.GetDataReaderFromSpWithParam(mysqlParameters, DBName, "sp_get_onboarding_document_html")
                );

                if (result != null && result.Count > 0)
                {
                    letterHtml = result[0].LetterHtml;
                }
            }
            catch (Exception ex)
            {
                CommonBL errorlog = new CommonBL();
                errorlog.fnStoreErrorLog("CommonBL", "GetOnboardingDocumentHtml", "Exception Message" + ex.Message + "Strace=" + ex.StackTrace, UserId);
            }
            return letterHtml;
        }

        public void insertlog(string v1, string message, string v2, string v3, string v4)
        {
            try
            {
                // OPTIONAL: write to file (recommended)
                string path = @"C:\Logs\AppLog.txt";

                if (!Directory.Exists(@"C:\Logs"))
                    Directory.CreateDirectory(@"C:\Logs");

                string logText =
                    DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + " | " +
                    v1 + " | " +
                    message + " | " +
                    v2 + " | " +
                    v3 + " | " +
                    v4 + Environment.NewLine;

                File.AppendAllText(path, logText);
            }
            catch(Exception ex)
            {
                CommonBL errorlog = new CommonBL();
                errorlog.fnStoreErrorLog("CommanBL", "insertlog", "Exception Message" + ex.Message + "Strace=" + ex.StackTrace, UserId);
            }
        }

        public List<CompanyLogoDO> GetCompanyLogoByUser(int userId)
        {
            List<CompanyLogoDO> listdata = new List<CompanyLogoDO>();
            try
            {
                getDrtolist getDrtolistParam = new getDrtolist();
                List<MySqlParameter> mysqlParameters = new List<MySqlParameter>();
                mysqlParameters.Add(DataClass.GetParameter("@p_UserId", userId));

                listdata = getDrtolistParam.getdatafromreder<CompanyLogoDO>(
                    DataClass.GetDataReaderFromSpWithParam(mysqlParameters, DBName, "sp_GetCompanyLogoNew_360")
                ).ToList();
            }
            catch (Exception ex)
            {
                CommonBL errorlog = new CommonBL();
                errorlog.fnStoreErrorLog("CommanBL", "GetCompanyLogoByUser", "Exception Message" + ex.Message + "Strace=" + ex.StackTrace, UserId);
            }
            return listdata;
        }

        // Returns the stored logo path (e.g. "assets/images/NEW_IMSET_LOGO.png") for a
        // specific user's company, or null when the user has no configured logo.
        public string GetCompanyLogoPathByUser(int userId)
        {
            if (userId <= 0)
            {
                return null;
            }

            List<CompanyLogoDO> listdata = GetCompanyLogoByUser(userId);
            if (listdata != null && listdata.Count > 0 && !string.IsNullOrWhiteSpace(listdata[0].LogoPath))
            {
                return listdata[0].LogoPath;
            }
            return null;
        }

        // Logo shown where there is no logged-in user (login / OTP pages). Driven by the
        // "DefaultLogoUserId" AppSetting so no DB schema change is required; returns null
        // when the setting is missing/blank so callers can fall back to the bundled image.
        public string GetDefaultCompanyLogoPath()
        {
            try
            {
                int defaultUserId = 0;
                int.TryParse(ConfigurationManager.AppSettings["DefaultLogoUserId"], out defaultUserId);
                if (defaultUserId > 0)
                {
                    return GetCompanyLogoPathByUser(defaultUserId);
                }
            }
            catch (Exception ex)
            {
                CommonBL errorlog = new CommonBL();
                errorlog.fnStoreErrorLog("CommanBL", "GetDefaultCompanyLogoPath", "Exception Message" + ex.Message + "Strace=" + ex.StackTrace, UserId);
            }
            return null;
        }

        // Best available logo path: the user's company logo, else the configured default,
        // else null (caller falls back to the bundled static image).
        public string ResolveCompanyLogoPath(int userId)
        {
            string path = GetCompanyLogoPathByUser(userId);
            if (string.IsNullOrWhiteSpace(path))
            {
                path = GetDefaultCompanyLogoPath();
            }
            return path;
        }

        // Looks up the user's company logo row, falling back to the "DefaultLogoUserId"
        // company when the user has none (or isn't logged in yet, e.g. userId &lt;= 0 on the
        // login/OTP pages). Returns null when neither has a base64/html logo.
        private CompanyLogoDO GetEffectiveCompanyLogo(int userId)
        {
            CompanyLogoDO logo = null;
            if (userId > 0)
            {
                List<CompanyLogoDO> listdata = GetCompanyLogoByUser(userId);
                logo = listdata != null && listdata.Count > 0 ? listdata[0] : null;
            }

            bool hasImageData = logo != null && (!string.IsNullOrWhiteSpace(logo.LogoHtml) || !string.IsNullOrWhiteSpace(logo.LogoBase64));
            if (!hasImageData)
            {
                int defaultUserId = 0;
                int.TryParse(ConfigurationManager.AppSettings["DefaultLogoUserId"], out defaultUserId);
                //if (defaultUserId > 0 && defaultUserId != userId)
                //{
                    List<CompanyLogoDO> defaultData = GetCompanyLogoByUser(defaultUserId);
                    logo = defaultData != null && defaultData.Count > 0 ? defaultData[0] : null;
                //}
            }

            return logo;
        }

        // Shared "data:<contentType>;base64,<data>" builder for any SP-supplied image
        // (company logo, dashboard banner slides, etc). Returns null when there's no base64.
        public static string BuildImageDataUri(string base64, string contentType)
        {
            if (string.IsNullOrWhiteSpace(base64))
            {
                return null;
            }

            string ct = string.IsNullOrWhiteSpace(contentType) ? "image/png" : contentType;
            return "data:" + ct + ";base64," + base64;
        }

        // Returns a ready-to-use "data:<contentType>;base64,<data>" URI for the user's
        // company logo when sp_GetCompanyLogoByUserId returns base64 image data, else null
        // so callers can fall back to ResolveCompanyLogoPath/the bundled static image.
        public string ResolveCompanyLogoImageUrl(int userId)
        {
            CompanyLogoDO logo = GetEffectiveCompanyLogo(userId);
            return logo == null ? null : BuildImageDataUri(logo.LogoBase64, logo.ContentType);
        }

        // Returns the ready-made <img> markup when sp_GetCompanyLogoByUserId builds it itself
        // (e.g. CONCAT('<img src="data:', v_logo_ct, ';base64,', v_logo_base64, '" style="max-height:60px;"/>')),
        // else builds the same markup from the base64/content-type columns, else null so the
        // caller can fall back to ResolveCompanyLogoPath/the bundled static image.
        public string ResolveCompanyLogoHtml(int userId)
        {
            CompanyLogoDO logo = GetEffectiveCompanyLogo(userId);
            if (logo == null)
            {
                return null;
            }

            if (!string.IsNullOrWhiteSpace(logo.LogoHtml))
            {
                return logo.LogoHtml;
            }

            string dataUri = BuildImageDataUri(logo.LogoBase64, logo.ContentType);
            return dataUri == null ? null : "<img src=\"" + dataUri + "\" style=\"max-height:60px;\"/>";
        }

        public List<DropDownData> dropdowCompany()
        {
            List<DropDownData> dropDownData = new List<DropDownData>();
            try
            {
                getDrtolist getDrtolistParam = new getDrtolist();
                List<MySqlParameter> mysqlParameters = new List<MySqlParameter>();
                dropDownData = getDrtolistParam.getdatafromreder<DropDownData>(DataClass.GetDataReaderFromSpWithParam(mysqlParameters, DBName, "Sp_DropdownCompanyName"));
            }
            catch (Exception ex)
            {
                CommonBL errorlog = new CommonBL();
                errorlog.fnStoreErrorLog("CommonBL", "dropdowCompany", "Exception Message" + ex.Message + "Strace=" + ex.StackTrace, UserId);
            }
            return dropDownData;
        }
        // Employee list for the Salary Slip dropdown.
        // SP must return columns aliased: Id (user_id), Text (employee name), Value (emp code, optional).
        public List<DropDownData> dropdownEmployee()
        {
            List<DropDownData> dropDownData = new List<DropDownData>();
            try
            {
                getDrtolist getDrtolistParam = new getDrtolist();
                List<MySqlParameter> mysqlParameters = new List<MySqlParameter>();
                dropDownData = getDrtolistParam.getdatafromreder<DropDownData>(DataClass.GetDataReaderFromSpWithParam(mysqlParameters, DBName, "sp_get_employee_dropdown_salarySlip"));
            }
            catch (Exception ex)
            {
                CommonBL errorlog = new CommonBL();
                errorlog.fnStoreErrorLog("CommonBL", "dropdownEmployee", "Exception Message" + ex.Message + "Strace=" + ex.StackTrace, UserId);
            }
            return dropDownData;
        }

        // ---- Image library (logo / signature / etc. stored as base64) ----

        public AppImageDO SaveAppImage(AppImageDO img, int insertedBy)
        {
            AppImageDO result = new AppImageDO { Status = "Failed", Remarks = "Unable to save image." };
            try
            {
                getDrtolist getDrtolistParam = new getDrtolist();
                List<MySqlParameter> mysqlParameters = new List<MySqlParameter>
                {
                    DataClass.GetParameter("@p_image_type", img.ImageType),
                    DataClass.GetParameter("@p_image_name", img.ImageName),
                    DataClass.GetParameter("@p_image_base64", img.ImageBase64, MySqlDbType.LongText),
                    DataClass.GetParameter("@p_content_type", img.ContentType),
                    DataClass.GetParameter("@p_file_extension", img.FileExtension),
                    DataClass.GetParameter("@p_inserted_by", insertedBy)
                };
                result = getDrtolistParam.getdatafromreder<AppImageDO>(
                    DataClass.GetDataReaderFromSpWithParam(mysqlParameters, DBName, "sp_save_app_image")).FirstOrDefault() ?? result;
            }
            catch (Exception ex)
            {
                CommonBL errorlog = new CommonBL();
                errorlog.fnStoreErrorLog("CommonBL", "SaveAppImage", "Exception Message" + ex.Message + "Strace=" + ex.StackTrace, UserId);
            }
            return result;
        }

        public List<AppImageDO> GetAppImages()
        {
            List<AppImageDO> list = new List<AppImageDO>();
            try
            {
                getDrtolist getDrtolistParam = new getDrtolist();
                List<MySqlParameter> mysqlParameters = new List<MySqlParameter>();
                list = getDrtolistParam.getdatafromreder<AppImageDO>(
                    DataClass.GetDataReaderFromSpWithParam(mysqlParameters, DBName, "sp_get_app_images"));
            }
            catch (Exception ex)
            {
                CommonBL errorlog = new CommonBL();
                errorlog.fnStoreErrorLog("CommonBL", "GetAppImages", "Exception Message" + ex.Message + "Strace=" + ex.StackTrace, UserId);
            }
            return list;
        }

        public AppImageDO DeleteAppImage(int imageId, int updatedBy)
        {
            AppImageDO result = new AppImageDO { Status = "Failed", Remarks = "Unable to delete image." };
            try
            {
                getDrtolist getDrtolistParam = new getDrtolist();
                List<MySqlParameter> mysqlParameters = new List<MySqlParameter>
                {
                    DataClass.GetParameter("@p_image_id", imageId),
                    DataClass.GetParameter("@p_updated_by", updatedBy)
                };
                result = getDrtolistParam.getdatafromreder<AppImageDO>(
                    DataClass.GetDataReaderFromSpWithParam(mysqlParameters, DBName, "sp_delete_app_image")).FirstOrDefault() ?? result;
            }
            catch (Exception ex)
            {
                CommonBL errorlog = new CommonBL();
                errorlog.fnStoreErrorLog("CommonBL", "DeleteAppImage", "Exception Message" + ex.Message + "Strace=" + ex.StackTrace, UserId);
            }
            return result;
        }

        public List<DropDownData> dropdowterminationReason()
        {
            List<DropDownData> dropDownData = new List<DropDownData>();
            try
            {
                getDrtolist getDrtolistParam = new getDrtolist();
                List<MySqlParameter> mysqlParameters = new List<MySqlParameter>();
                dropDownData = getDrtolistParam.getdatafromreder<DropDownData>(DataClass.GetDataReaderFromSpWithParam(mysqlParameters, DBName, "sp_get_termination_reasons"));
            }
            catch (Exception ex)
            {
                CommonBL errorlog = new CommonBL();
                errorlog.fnStoreErrorLog("CommonBL", "dropdowterminationReason", "Exception Message" + ex.Message + "Strace=" + ex.StackTrace, UserId);
            }
            return dropDownData;
        }
        public List<DropDownData> dropdownassignby()
        {
            List<DropDownData> dropDownData = new List<DropDownData>();
            try
            {
                getDrtolist getDrtolistParam = new getDrtolist();
                List<MySqlParameter> mysqlParameters = new List<MySqlParameter>();

                dropDownData = getDrtolistParam.getdatafromreder<DropDownData>(
                    DataClass.GetDataReaderFromSpWithParam(mysqlParameters, DBName, "sp_bindassignby"));
            }
            catch (Exception ex)
            {
                CommonBL errorlog = new CommonBL();
                errorlog.fnStoreErrorLog("CommonBL", "dropdownassignby",
                    "Exception Message" + ex.Message + "Strace=" + ex.StackTrace, UserId);
            }
            return dropDownData;
        }

        public List<DropDownData> dropdownEmpexporIntern()
        {
            List<DropDownData> dropDownData = new List<DropDownData>();
            try
            {
                getDrtolist getDrtolistParam = new getDrtolist();
                List<MySqlParameter> MySqlParameter = new List<MySqlParameter>();

                dropDownData = getDrtolistParam.getdatafromreder<DropDownData>(DataClass.GetDataReaderFromSpWithParam(MySqlParameter, DBName, "sp_BindEmployeeExpOrIntern"));
            }
            catch (Exception ex)
            {
                CommonBL errorlog = new CommonBL();
                errorlog.fnStoreErrorLog("CommonBL", "dropdownEmpexporIntern", "Exception Message" + ex.Message + "Strace=" + ex.StackTrace, UserId);
            }
            return dropDownData;
        }

        // Secondary DB dropdowns for Employee Registration flow
        public List<DropDownData> dropdownDesignationSecondary()
        {
            return GetDropDownDataFromSecondary("sp_bindDesignations");
        }

        public List<DropDownData> dropdowCompanySecondary()
        {
            return GetDropDownDataFromSecondary("Sp_BindDropdownCompany");
        }

        public List<DropDownData> dropdownassignbySecondary()
        {
            return GetDropDownDataFromSecondary("sp_bindassignby");
        }

        public List<DropDownData> dropdownEmpexporInternSecondary()
        {
            return GetDropDownDataFromSecondary("sp_BindEmployeeExpOrIntern");
        }

        private List<DropDownData> GetDropDownDataFromSecondary(string spName)
        {
            List<DropDownData> data = new List<DropDownData>();
            if (string.IsNullOrWhiteSpace(Sqlconnection))
            {
                return data;
            }

            Exception mysqlEx = null;
            Exception sqlEx = null;
            try
            {
                data = GetDropDownDataFromSecondaryMySql(spName);
            }
            catch (Exception ex)
            {
                mysqlEx = ex;
            }

            if (data != null && data.Count > 0)
            {
                return data;
            }

            try
            {
                data = GetDropDownDataFromSecondarySql(spName);
            }
            catch (Exception ex)
            {
                sqlEx = ex;
            }

            if ((data == null || data.Count == 0) && (mysqlEx != null || sqlEx != null))
            {
                fnStoreErrorLog(
                    "CommonBL",
                    "GetDropDownDataFromSecondary",
                    "SP=" + spName
                    + " | MySqlError=" + (mysqlEx != null ? mysqlEx.Message : "none")
                    + " | SqlError=" + (sqlEx != null ? sqlEx.Message : "none"),
                    UserId
                );
            }

            return data;
        }

        private List<DropDownData> GetDropDownDataFromSecondaryMySql(string spName)
        {
            List<DropDownData> data = new List<DropDownData>();
            string normalized = NormalizeMySqlConnectionString(Sqlconnection);
            using (MySqlConnection con = new MySqlConnection(normalized))
            using (MySqlCommand cmd = new MySqlCommand(spName, con))
            {
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                con.Open();
                using (MySqlDataReader dr = cmd.ExecuteReader())
                {
                    data = MapDropDownData(dr);
                }
            }
            return data;
        }

        private List<DropDownData> GetDropDownDataFromSecondarySql(string spName)
        {
            List<DropDownData> data = new List<DropDownData>();
            using (SqlConnection con = new SqlConnection(Sqlconnection))
            using (SqlCommand cmd = new SqlCommand(spName, con))
            {
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                con.Open();
                using (SqlDataReader dr = cmd.ExecuteReader())
                {
                    data = MapDropDownData(dr);
                }
            }
            return data;
        }

        private List<DropDownData> MapDropDownData(System.Data.IDataReader dr)
        {
            List<DropDownData> items = new List<DropDownData>();
            while (dr.Read())
            {
                string id = string.Empty;
                string text = string.Empty;

                id = GetReaderValue(dr, new[] { "Id", "id", "ID", "value", "Value", "designation_id", "company_id", "user_id", "emp_id" });
                text = GetReaderValue(dr, new[] { "Text", "text", "TEXT", "name", "Name", "designation_name", "company_name", "username", "user_fullname", "employee_type" });

                if (string.IsNullOrWhiteSpace(id) && dr.FieldCount > 0)
                {
                    id = Convert.ToString(dr[0]);
                }
                if (string.IsNullOrWhiteSpace(text))
                {
                    if (dr.FieldCount > 1)
                    {
                        text = Convert.ToString(dr[1]);
                    }
                    else
                    {
                        text = id;
                    }
                }

                items.Add(new DropDownData
                {
                    Id = ParseInt(id),
                    Text = text
                });
            }
            return items;
        }

        private string GetReaderValue(System.Data.IDataRecord record, string[] columnNames)
        {
            foreach (string col in columnNames)
            {
                for (int i = 0; i < record.FieldCount; i++)
                {
                    if (string.Equals(record.GetName(i), col, StringComparison.OrdinalIgnoreCase))
                    {
                        object v = record[i];
                        return v == DBNull.Value ? string.Empty : Convert.ToString(v);
                    }
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
                return builder.ConnectionString;
            }
            catch
            {
                NameValueCollection parts = ParseConnectionString(connectionString);
                var builder = new MySqlConnectionStringBuilder();

                string server = GetConnectionValue(parts, "Server", "Data Source", "Datasource", "Host");
                string database = GetConnectionValue(parts, "Database", "Initial Catalog");
                string user = GetConnectionValue(parts, "User Id", "UserID", "uid", "User");
                string password = GetConnectionValue(parts, "Password", "Pwd");
                string portText = GetConnectionValue(parts, "Port");

                builder.Server = string.IsNullOrWhiteSpace(server) ? "localhost" : server;
                builder.Database = database ?? string.Empty;
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

        public List<DropDownData> dropdownEmployeeCode_ForRenumeration()
        {
            List<DropDownData> dropDownData = new List<DropDownData>();
            try
            {
                getDrtolist getDrtolistParam = new getDrtolist();
                List<MySqlParameter> mysqlParameters = new List<MySqlParameter>();
                dropDownData = getDrtolistParam.getdatafromreder<DropDownData>(DataClass.GetDataReaderFromSpWithParam(mysqlParameters, DBName, "SP_BindEmployeeCodeFor_renumeration"));
            }
            catch (Exception ex)
            {
                CommonBL errorlog = new CommonBL();
                errorlog.fnStoreErrorLog("CommonBL", "dropdownEmployeeCode_ForRenumeration", "Exception Message" + ex.Message + "Strace=" + ex.StackTrace, UserId);
            }
            return dropDownData;
        }

        public List<DropDownData> BindLookupData(string lookupType)
        {
            List<DropDownData> items = new List<DropDownData>();
            if (string.IsNullOrWhiteSpace(Sqlconnection) || string.IsNullOrWhiteSpace(lookupType))
            {
                return items;
            }

            try
            {
                string normalized = NormalizeMySqlConnectionString(Sqlconnection);
                using (MySqlConnection con = new MySqlConnection(normalized))
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
                errorlog.fnStoreErrorLog("CommonBL", "BindLookupData", "Exception Message: " + ex.Message + " StackTrace: " + ex.StackTrace, UserId);
            }

            return items;
        }

        private int ReadInt(IDataRecord dr, string columnName)
        {
            for (int i = 0; i < dr.FieldCount; i++)
            {
                if (string.Equals(dr.GetName(i), columnName, StringComparison.OrdinalIgnoreCase))
                {
                    object v = dr[i];
                    if (v == DBNull.Value) return 0;
                    int result = 0;
                    int.TryParse(Convert.ToString(v), out result);
                    return result;
                }
            }
            return 0;
        }

        private string ReadString(IDataRecord dr, string columnName)
        {
            for (int i = 0; i < dr.FieldCount; i++)
            {
                if (string.Equals(dr.GetName(i), columnName, StringComparison.OrdinalIgnoreCase))
                {
                    object v = dr[i];
                    if (v == DBNull.Value) return string.Empty;
                    return Convert.ToString(v);
                }
            }
            return string.Empty;
        }



       public List<DropDownData_account> dropdownusername_accountdashboard()
  {
      List<DropDownData_account> dropDownData = new List<DropDownData_account>();
      try
      {
          getDrtolist getDrtolistParam = new getDrtolist();
          List<MySqlParameter> mysqlParameters = new List<MySqlParameter>();
          dropDownData = getDrtolistParam.getdatafromreder<DropDownData_account>(DataClass.GetDataReaderFromSpWithParam(mysqlParameters, DBName, "sp_bindusername_Accountdashboard"));
      }
      catch (Exception ex)
      {
          CommonBL errorlog = new CommonBL();
          errorlog.fnStoreErrorLog("CommonBL", "dropdownusername", "Exception Message" + ex.Message + "Strace=" + ex.StackTrace, UserId);
      }
      return dropDownData;
  }
  public List<DropDownData_account> dropdownempcode_accountdashboard()
  {
      List<DropDownData_account> dropDownData = new List<DropDownData_account>();
      try
      {
          getDrtolist getDrtolistParam = new getDrtolist();
          List<MySqlParameter> mysqlParameters = new List<MySqlParameter>();
          dropDownData = getDrtolistParam.getdatafromreder<DropDownData_account>(DataClass.GetDataReaderFromSpWithParam(mysqlParameters, DBName, "sp_bindempcode_Accountdashboard"));
      }
      catch (Exception ex)
      {
          CommonBL errorlog = new CommonBL();
          errorlog.fnStoreErrorLog("CommonBL", "dropdownusername", "Exception Message" + ex.Message + "Strace=" + ex.StackTrace, UserId);
      }
      return dropDownData;
  }

  public void SendEmail(string toMail, string ccMail, string subject, string body)
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

              // TO
              foreach (string email in toMail.Split(new[] { ';', ',' }, StringSplitOptions.RemoveEmptyEntries))
              {
                  if (!string.IsNullOrWhiteSpace(email))
                      mail.To.Add(email.Trim());
              }

              // CC
              if (!string.IsNullOrWhiteSpace(ccMail))
              {
                  foreach (string email in ccMail.Split(new[] { ';', ',' }, StringSplitOptions.RemoveEmptyEntries))
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
          fnStoreErrorLog("CommonBL", "SendEmail", "Exception Message: " + ex.Message + " StackTrace: " + ex.StackTrace, UserId);
      }
  }

  public void SendEmail(string toMail, string ccMail, string bccMail, string subject, string body)
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

              // TO
              foreach (string email in toMail.Split(new[] { ';', ',' }, StringSplitOptions.RemoveEmptyEntries))
              {
                  if (!string.IsNullOrWhiteSpace(email))
                      mail.To.Add(email.Trim());
              }

              // CC
              if (!string.IsNullOrWhiteSpace(ccMail))
              {
                  foreach (string email in ccMail.Split(new[] { ';', ',' }, StringSplitOptions.RemoveEmptyEntries))
                  {
                      if (!string.IsNullOrWhiteSpace(email))
                          mail.CC.Add(email.Trim());
                  }
              }

              // BCC
              if (!string.IsNullOrWhiteSpace(bccMail))
              {
                  foreach (string email in bccMail.Split(new[] { ';', ',' }, StringSplitOptions.RemoveEmptyEntries))
                  {
                      if (!string.IsNullOrWhiteSpace(email))
                          mail.Bcc.Add(email.Trim());
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
          fnStoreErrorLog("CommonBL", "SendEmail", "Exception Message: " + ex.Message + " StackTrace: " + ex.StackTrace, UserId);
      }
  }

  public void SendEmail(string toMail, string ccMail, string bccMail, string subject, string body, byte[] attachmentBytes, string attachmentFileName)
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

              // TO
              foreach (string email in toMail.Split(new[] { ';', ',' }, StringSplitOptions.RemoveEmptyEntries))
              {
                  if (!string.IsNullOrWhiteSpace(email))
                      mail.To.Add(email.Trim());
              }

              // CC
              if (!string.IsNullOrWhiteSpace(ccMail))
              {
                  foreach (string email in ccMail.Split(new[] { ';', ',' }, StringSplitOptions.RemoveEmptyEntries))
                  {
                      if (!string.IsNullOrWhiteSpace(email))
                          mail.CC.Add(email.Trim());
                  }
              }

              // BCC
              if (!string.IsNullOrWhiteSpace(bccMail))
              {
                  foreach (string email in bccMail.Split(new[] { ';', ',' }, StringSplitOptions.RemoveEmptyEntries))
                  {
                      if (!string.IsNullOrWhiteSpace(email))
                          mail.Bcc.Add(email.Trim());
                  }
              }

              mail.Subject = subject;
              mail.Body = body;
              mail.IsBodyHtml = true;

              if (attachmentBytes != null && attachmentBytes.Length > 0)
              {
                  string fileName = string.IsNullOrWhiteSpace(attachmentFileName) ? "attachment.pdf" : attachmentFileName;
                  mail.Attachments.Add(new Attachment(new MemoryStream(attachmentBytes), fileName, "application/pdf"));
              }

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
          fnStoreErrorLog("CommonBL", "SendEmail", "Exception Message: " + ex.Message + " StackTrace: " + ex.StackTrace, UserId);
      }
  }
    }
}
