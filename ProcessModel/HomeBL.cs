
using DataObject;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using static DataObject.HomeDO;

namespace ProcessModel
{
    public class HomeBL
    {
        protected string UserId = null;

        private string DBName = ConfigurationManager.AppSettings["DBName"];
        private static string MySqlconnection = ConfigurationManager.ConnectionStrings["MysqlConnection"].ConnectionString;
        private static string Sqlconnection = ConfigurationManager.ConnectionStrings["Sqlconnection"] != null
        ? ConfigurationManager.ConnectionStrings["Sqlconnection"].ConnectionString
        : string.Empty;
        public List<GetUserNameDO> GetUserName(string UserId)
        {
            List<GetUserNameDO> listData = new List<GetUserNameDO>();

            try
            {
                getDrtolist getDrtolistParam = new getDrtolist();

                List<MySqlParameter> mysqlParameters = new List<MySqlParameter>();

                mysqlParameters.Add(DataClass.GetParameter("@p_user_id", UserId));

                listData = getDrtolistParam.getdatafromreder<GetUserNameDO>(
                                DataClass.GetDataReaderFromSpWithParam(
                                    mysqlParameters,
                                    DBName,
                                    "sp_get_user_name_forHomeDashboard"));
            }
            catch (Exception ex)
            {
                CommonBL errorlog = new CommonBL();

                errorlog.fnStoreErrorLog(
                    "HomeBL",
                    "GetUserName",
                    "Exception Message : " + ex.Message +
                    " StackTrace : " + ex.StackTrace,
                    UserId);
            }

            return listData;
        }

        public List<GetNextHolidayDO> GetNextHoliday(int companyId)
        {
            List<GetNextHolidayDO> listData = new List<GetNextHolidayDO>();

            try
            {
                getDrtolist getDrtolistParam = new getDrtolist();

                List<MySqlParameter> mysqlParameters = new List<MySqlParameter>();

                mysqlParameters.Add(DataClass.GetParameter("@p_company_id", companyId));

                listData = getDrtolistParam.getdatafromreder<GetNextHolidayDO>(
                                DataClass.GetDataReaderFromSpWithParam(
                                    mysqlParameters,
                                    DBName,
                                    "sp_get_next_holiday"));
            }
            catch (Exception ex)
            {
                CommonBL errorlog = new CommonBL();

                errorlog.fnStoreErrorLog(
                    "HomeBL",
                    "GetNextHoliday",
                    "Exception Message : " + ex.Message +
                    " StackTrace : " + ex.StackTrace,
                    UserId);
            }

            return listData;
        }
        public List<AnnouncementCountDO> GetAnnouncementCount(int companyId)
        {
            List<AnnouncementCountDO> listData = new List<AnnouncementCountDO>();

            getDrtolist getDrtolistParam = new getDrtolist();
            List<MySqlParameter> mysqlParameters = new List<MySqlParameter>();

            try
            {
                mysqlParameters.Add(DataClass.GetParameter("@p_company_id", companyId));

                listData = getDrtolistParam.getdatafromreder<AnnouncementCountDO>(
                    DataClass.GetDataReaderFromSpWithParam(
                        mysqlParameters,
                        DBName,
                        "sp_get_announcement_count"));
            }
            catch (Exception ex)
            {
                CommonBL errorlog = new CommonBL();
                errorlog.fnStoreErrorLog(
                    "HomeBL",
                    "GetAnnouncementCount",
                    "Exception Message : " + ex.Message + " StackTrace : " + ex.StackTrace,
                    UserId);
            }

            return listData;
        }
        public List<EventCountDO> GetEventCount(int companyId)
        {
            List<EventCountDO> listData = new List<EventCountDO>();

            getDrtolist getDrtolistParam = new getDrtolist();
            List<MySqlParameter> mysqlParameters = new List<MySqlParameter>();

            try
            {
                mysqlParameters.Add(DataClass.GetParameter("@p_company_id", companyId));

                listData = getDrtolistParam.getdatafromreder<EventCountDO>(
                    DataClass.GetDataReaderFromSpWithParam(
                        mysqlParameters,
                        DBName,
                        "sp_get_upcoming_event_count"));
            }
            catch (Exception ex)
            {
                CommonBL errorlog = new CommonBL();
                errorlog.fnStoreErrorLog(
                    "HomeBL",
                    "GetEventCount",
                    "Exception Message : " + ex.Message + " StackTrace : " + ex.StackTrace,
                    UserId);
            }

            return listData;
        }
        public List<BirthdayCountDO> GetBirthdayCountThisweek()
        {
            List<BirthdayCountDO> items = new List<BirthdayCountDO>();

            if (string.IsNullOrWhiteSpace(Sqlconnection))
            {
                return items;
            }

            try
            {
                using (MySqlConnection con = new MySqlConnection(Sqlconnection))
                using (MySqlCommand cmd = new MySqlCommand("sp_get_birthdays_this_week", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    con.Open();

                    using (MySqlDataReader dr = cmd.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            items.Add(new BirthdayCountDO
                            {
                                birthday_count = Convert.ToInt32(dr["birthday_count"])
                            });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                CommonBL errorlog = new CommonBL();
                errorlog.fnStoreErrorLog(
                    "HomeBL",
                    "GetBirthdayCountThisweek",
                    "Exception Message: " + ex.Message + " StackTrace: " + ex.StackTrace,
                    UserId);
            }

            return items;
        }
        public List<NewsAnnouncementDO> GetCompanyNews(int companyId)
        {
            List<NewsAnnouncementDO> listData = new List<NewsAnnouncementDO>();

            try
            {
                getDrtolist getDrtolistParam = new getDrtolist();

                List<MySqlParameter> mysqlParameters = new List<MySqlParameter>();

                mysqlParameters.Add(DataClass.GetParameter("@p_company_id", companyId));

                listData = getDrtolistParam.getdatafromreder<NewsAnnouncementDO>
                (
                    DataClass.GetDataReaderFromSpWithParam(
                        mysqlParameters,
                        DBName,
                        "sp_get_company_news")
                );

                foreach (var item in listData)
                {
                    int days = (DateTime.Now.Date - item.inserted_date.Date).Days;

                    if (days == 0)
                        item.PostedOn = "Posted Today";
                    else if (days == 1)
                        item.PostedOn = "Posted 1 day ago";
                    else
                        item.PostedOn = "Posted " + days + " days ago";

                    //Create initials from posted_by
                    string[] words = item.posted_by.Split(' ');

                    if (words.Length == 1)
                        item.Initials = words[0].Substring(0, 1).ToUpper();
                    else
                        item.Initials =
                            words[0].Substring(0, 1).ToUpper() +
                            words[1].Substring(0, 1).ToUpper();
                }
            }
            catch (Exception ex)
            {
                CommonBL errorlog = new CommonBL();
                errorlog.fnStoreErrorLog(
                    "HomeBL",
                    "GetCompanyNews",
                    ex.Message + ex.StackTrace,
                    UserId);
            }

            return listData;
        }

        public List<CompanyEventDO> GetCompanyEvents(int companyId)
        {
            List<CompanyEventDO> listData = new List<CompanyEventDO>();

            try
            {
                getDrtolist getDrtolistParam = new getDrtolist();

                List<MySqlParameter> mysqlParameters = new List<MySqlParameter>();

                mysqlParameters.Add(DataClass.GetParameter("@p_company_id", companyId));

                listData = getDrtolistParam.getdatafromreder<CompanyEventDO>
                (
                    DataClass.GetDataReaderFromSpWithParam(
                        mysqlParameters,
                        DBName,
                        "sp_get_company_events")
                );

                foreach (var item in listData)
                {
                    item.EventDay = item.event_date.ToString("dd");

                    if (item.event_time.HasValue)
                    {
                        DateTime dt = item.event_date.Add(item.event_time.Value);

                        item.EventDate = dt.ToString("dd MMM yyyy") +
                                         " · " +
                                         dt.ToString("hh:mm tt");
                    }
                    else
                    {
                        item.EventDate = item.event_date.ToString("dd MMM yyyy");
                    }
                }
            }
            catch (Exception ex)
            {
                CommonBL errorlog = new CommonBL();
                errorlog.fnStoreErrorLog(
                    "HomeBL",
                    "GetCompanyEvents",
                    ex.Message + ex.StackTrace,
                    UserId);
            }

            return listData;
        }
        public List<BirthdayDO> GetUpcomingBirthdays()
        {
            List<BirthdayDO> items = new List<BirthdayDO>();

            if (string.IsNullOrWhiteSpace(Sqlconnection))
            {
                return items;
            }

            try
            {
                using (MySqlConnection con = new MySqlConnection(Sqlconnection))
                using (MySqlCommand cmd = new MySqlCommand("sp_get_upcoming_birthdays", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    con.Open();

                    using (MySqlDataReader dr = cmd.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            BirthdayDO item = new BirthdayDO();

                            item.EmployeeName = dr["EmployeeName"].ToString();
                            item.Department = dr["Department"].ToString();
                            item.DOB = Convert.ToDateTime(dr["DOB"]);

                            // Calculate next birthday
                            DateTime nextBirthday = new DateTime(
                                DateTime.Today.Year,
                                item.DOB.Month,
                                item.DOB.Day);

                            if (nextBirthday < DateTime.Today)
                            {
                                nextBirthday = nextBirthday.AddYears(1);
                            }

                            // Show Today if birthday is today
                            if (nextBirthday.Date == DateTime.Today)
                            {
                                item.DateLabel = "Today";
                                item.BadgeClass = "today";
                            }
                            else
                            {
                                item.DateLabel = nextBirthday.ToString("dd MMM");
                                item.BadgeClass = "normal";
                            }

                            // Initials
                            string[] words = item.EmployeeName.Split(
                                new char[] { ' ' },
                                StringSplitOptions.RemoveEmptyEntries);

                            if (words.Length == 1)
                            {
                                item.Initials = words[0].Substring(0, 1).ToUpper();
                            }
                            else if (words.Length >= 2)
                            {
                                item.Initials = words[0].Substring(0, 1).ToUpper() +
                                                words[1].Substring(0, 1).ToUpper();
                            }
                            else
                            {
                                item.Initials = "";
                            }

                            items.Add(item);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                CommonBL errorlog = new CommonBL();
                errorlog.fnStoreErrorLog(
                    "HomeBL",
                    "GetUpcomingBirthdays",
                    "Exception Message: " + ex.Message +
                    " StackTrace: " + ex.StackTrace,
                    UserId);
            }

            return items;
        }
        public List<NewsAnnouncementDO> SaveCompanyNews(NewsAnnouncementDO news)
        {
            List<NewsAnnouncementDO> listdata = new List<NewsAnnouncementDO>();
            getDrtolist getDrtolistParam = new getDrtolist();
            List<MySqlParameter> mysqlParameters = new List<MySqlParameter>();

            try
            {
                mysqlParameters.Add(DataClass.GetParameter("p_company_id", news.company_id));
                mysqlParameters.Add(DataClass.GetParameter("p_news_title", news.news_title));
                mysqlParameters.Add(DataClass.GetParameter("p_category", news.category));
                mysqlParameters.Add(DataClass.GetParameter("p_posted_by", news.posted_by));
                mysqlParameters.Add(DataClass.GetParameter("p_description", news.description));
                mysqlParameters.Add(DataClass.GetParameter("p_inserted_by", news.inserted_by));

                mysqlParameters.Add(DataClass.GetParameter("p_file_name", news.file_name));
                mysqlParameters.Add(DataClass.GetParameter("p_file_type", news.file_type));
                mysqlParameters.Add(DataClass.GetParameter("p_file_base64", news.file_base64));

                listdata = getDrtolistParam.getdatafromreder<NewsAnnouncementDO>
                (
                    DataClass.GetDataReaderFromSpWithParam(
                        mysqlParameters,
                        DBName,
                        "sp_insert_company_news"
                    )
                );
            }
            catch (Exception ex)
            {
                CommonBL errorlog = new CommonBL();
                errorlog.fnStoreErrorLog(
                    "HomeBL",
                    "SaveCompanyNews",
                    "Exception Message: " + ex.Message + " | StackTrace=" + ex.StackTrace,
                    UserId
                );
            }

            if (listdata == null || listdata.Count == 0)
            {
                listdata = new List<NewsAnnouncementDO>
        {
            new NewsAnnouncementDO
            {
                Success = "Failed",
                Result = "News save did not return any response from database."
            }
        };
            }

            return listdata;
        }
        public List<SaveEventDO> SaveCompanyEvent(SaveEventDO eventDO)
        {
            List<SaveEventDO> listdata = new List<SaveEventDO>();
            getDrtolist getDrtolistParam = new getDrtolist();
            List<MySqlParameter> mysqlParameters = new List<MySqlParameter>();

            try
            {
                mysqlParameters.Add(DataClass.GetParameter("p_company_id", eventDO.company_id));
                mysqlParameters.Add(DataClass.GetParameter("p_event_type", eventDO.event_type));
                mysqlParameters.Add(DataClass.GetParameter("p_event_date", eventDO.event_date));
                mysqlParameters.Add(DataClass.GetParameter("p_event_time", eventDO.event_time));
                mysqlParameters.Add(DataClass.GetParameter("p_event_title", eventDO.event_title));
                mysqlParameters.Add(DataClass.GetParameter("p_event_description", eventDO.event_description));
                mysqlParameters.Add(DataClass.GetParameter("p_inserted_by", eventDO.inserted_by));

                mysqlParameters.Add(DataClass.GetParameter("p_file_name", eventDO.file_name));
                mysqlParameters.Add(DataClass.GetParameter("p_file_type", eventDO.file_type));
                mysqlParameters.Add(DataClass.GetParameter("p_file_base64", eventDO.file_base64));


                listdata = getDrtolistParam.getdatafromreder<SaveEventDO>
                (
                    DataClass.GetDataReaderFromSpWithParam
                    (
                        mysqlParameters,
                        DBName,
                        "sp_insert_company_event"
                    )
                );
            }
            catch (Exception ex)
            {
                CommonBL errorlog = new CommonBL();
                errorlog.fnStoreErrorLog(
                    "HomeBL",
                    "SaveCompanyEvent",
                    "Exception Message: " + ex.Message + " | StackTrace=" + ex.StackTrace,
                    UserId
                );
            }

            if (listdata == null || listdata.Count == 0)
            {
                listdata = new List<SaveEventDO>
        {
            new SaveEventDO
            {
                Success = "Failed",
                Result = "Event save did not return any response from database."
            }
        };
            }

            return listdata;
        }

        //public List<SaveBirthdayDO> SaveBirthday(SaveBirthdayDO birthday)
        //{
        //    List<SaveBirthdayDO> listdata = new List<SaveBirthdayDO>();

        //    if (string.IsNullOrWhiteSpace(Sqlconnection))
        //    {
        //        return listdata;
        //    }

        //    try
        //    {
        //        using (MySqlConnection con = new MySqlConnection(Sqlconnection))
        //        using (MySqlCommand cmd = new MySqlCommand("sp_Save_employee_birthday", con))
        //        {
        //            cmd.CommandType = CommandType.StoredProcedure;
        //            cmd.Parameters.AddWithValue("p_emp_code", birthday.emp_code);
        //            cmd.Parameters.AddWithValue("p_user_fullname", birthday.employee_name);
        //            cmd.Parameters.AddWithValue("p_department", birthday.department);
        //            cmd.Parameters.AddWithValue("p_dob", birthday.DOB);
        //            cmd.Parameters.AddWithValue("p_updated_by", birthday.updated_by);

        //            con.Open();

        //            using (MySqlDataReader dr = cmd.ExecuteReader())
        //            {
        //                while (dr.Read())
        //                {
        //                    SaveBirthdayDO item = new SaveBirthdayDO();

        //                    item.Success = dr["Success"].ToString();
        //                    item.Result = dr["Result"].ToString();

        //                    listdata.Add(item);
        //                }
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        CommonBL errorlog = new CommonBL();
        //        errorlog.fnStoreErrorLog(
        //            "HomeBL",
        //            "SaveBirthday",
        //            "Exception Message: " + ex.Message + " StackTrace: " + ex.StackTrace,
        //            UserId);
        //    }

        //    if (listdata.Count == 0)
        //    {
        //        listdata.Add(new SaveBirthdayDO
        //        {
        //            Success = "Failed",
        //            Result = "Birthday save did not return any response from database."
        //        });
        //    }

        //    return listdata;
        //}

        public List<DashboardBannerDO> GetDashboardBanner()
        {
            List<DashboardBannerDO> bannerList = new List<DashboardBannerDO>();

            //  Next Event + Latest News + every upcoming Holiday, in one
            //  UNION ALL result set (sp_get_dashboard_banner). Each row's
            //  ImageBase64 comes from app_image_library (Event/News/Festival)
            //  when an admin has uploaded one, else the SP's hardcoded ImageUrl.
            try
            {
                getDrtolist getDrtolistParam = new getDrtolist();
                List<MySqlParameter> mysqlParameters = new List<MySqlParameter>();

                List<DashboardBannerDO> dashboardData =
                    getDrtolistParam.getdatafromreder<DashboardBannerDO>(
                        DataClass.GetDataReaderFromSpWithParam(
                            mysqlParameters,
                            DBName,
                            "sp_get_dashboard_banner"));

                if (dashboardData != null)
                {
                    foreach (var item in dashboardData)
                    {
                        if (item.Category == "News")
                        {
                            int days = (DateTime.Today - item.inserted_date.Date).Days;

                            if (days <= 0)
                                item.Meta = "Posted Today";
                            else if (days == 1)
                                item.Meta = "Posted 1 day ago";
                            else
                                item.Meta = $"Posted {days} days ago";
                        }
                    }

                    bannerList.AddRange(dashboardData);
                }
            }
            catch (Exception ex)
            {
                CommonBL errorlog = new CommonBL();
                errorlog.fnStoreErrorLog(
                    "HomeBL",
                    "GetDashboardBanner(EventsNewsHolidays)",
                    "Exception Message : " + ex.Message +
                    " StackTrace : " + ex.StackTrace,
                    UserId);
            }

            //  Today's Birthday
            try
            {
                if (!string.IsNullOrWhiteSpace(Sqlconnection))
                {
                    using (MySqlConnection con = new MySqlConnection(Sqlconnection))
                    using (MySqlCommand cmd = new MySqlCommand("sp_get_today_birthday_banner", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;

                        con.Open();

                        using (MySqlDataReader dr = cmd.ExecuteReader())
                        {
                            while (dr.Read())
                            {
                                bannerList.Add(new DashboardBannerDO
                                {
                                    DisplayOrder = Convert.ToInt32(dr["DisplayOrder"]),
                                    Category = dr["Category"].ToString(),
                                    Title = dr["Title"].ToString(),
                                    Meta = dr["Meta"].ToString(),
                                    Description = dr["Description"].ToString(),
                                    ImageUrl = dr["ImageUrl"].ToString(),
                                    Background = dr["Background"].ToString()
                                });
                            }
                        }
                    }
                }

                bannerList = bannerList
                                .OrderBy(x => x.DisplayOrder)
                                .ToList();
            }
            catch (Exception ex)
            {
                CommonBL errorlog = new CommonBL();
                errorlog.fnStoreErrorLog(
                    "HomeBL",
                    "GetDashboardBanner",
                    "Exception Message : " + ex.Message +
                    " StackTrace : " + ex.StackTrace,
                    UserId);
            }

            return bannerList;
        }

        public List<NewsAnnouncementDO> GetNewsById(int news_announcement_id)
        {
            List<NewsAnnouncementDO> listData = new List<NewsAnnouncementDO>();

            getDrtolist getDrtolistParam = new getDrtolist();
            List<MySqlParameter> mysqlParameters = new List<MySqlParameter>();

            try
            {
                mysqlParameters.Add(DataClass.GetParameter("@p_news_announcement_id", news_announcement_id));

                listData = getDrtolistParam.getdatafromreder<NewsAnnouncementDO>
                (
                    DataClass.GetDataReaderFromSpWithParam
                    (
                        mysqlParameters,
                        DBName,
                        "sp_get_company_news_by_id"
                    )
                );
            }
            catch (Exception ex)
            {
                CommonBL errorlog = new CommonBL();
                errorlog.fnStoreErrorLog(
                    "HomeBL",
                    "GetNewsById",
                    "Exception Message : " + ex.Message +
                    " StackTrace : " + ex.StackTrace,
                    UserId);
            }

            return listData;
        }
        public List<CompanyEventDO> GetEventById(int event_mast_id)
        {
            List<CompanyEventDO> listData = new List<CompanyEventDO>();

            getDrtolist getDrtolistParam = new getDrtolist();
            List<MySqlParameter> mysqlParameters = new List<MySqlParameter>();

            try
            {
                mysqlParameters.Add(DataClass.GetParameter("@p_event_mast_id", event_mast_id));

                listData = getDrtolistParam.getdatafromreder<CompanyEventDO>
                (
                    DataClass.GetDataReaderFromSpWithParam
                    (
                        mysqlParameters,
                        DBName,
                        "sp_get_event_by_id"
                    )
                );
            }
            catch (Exception ex)
            {
                CommonBL errorlog = new CommonBL();
                errorlog.fnStoreErrorLog(
                    "HomeBL",
                    "GetEventById",
                    "Exception : " + ex.Message,
                    UserId);
            }

            return listData;
        }

        public List<HolidayDO> GetHolidayById(int holiday_id)
        {
            List<HolidayDO> listData = new List<HolidayDO>();

            getDrtolist getDrtolistParam = new getDrtolist();
            List<MySqlParameter> mysqlParameters = new List<MySqlParameter>();

            mysqlParameters.Add(DataClass.GetParameter("@p_holiday_id", holiday_id));

            listData = getDrtolistParam.getdatafromreder<HolidayDO>
            (
                DataClass.GetDataReaderFromSpWithParam(
                    mysqlParameters,
                    DBName,
                    "sp_get_holiday_by_id"
                )
            );

            return listData;
        }

        public List<BirthdayMailDO> GetBirthdayMailDetails()
        {
            List<BirthdayMailDO> listData = new List<BirthdayMailDO>();

            if (string.IsNullOrWhiteSpace(Sqlconnection))
            {
                return listData;
            }

            try
            {
                using (MySqlConnection con = new MySqlConnection(Sqlconnection))
                using (MySqlCommand cmd = new MySqlCommand("sp_send_birthday_mail", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    con.Open();

                    using (MySqlDataReader dr = cmd.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            BirthdayMailDO item = new BirthdayMailDO();

                            item.ToMail = dr["ToMail"].ToString();
                            item.CcMail = dr["CcMail"].ToString();
                            item.Subject = dr["Subject"].ToString();
                            item.MailBody = dr["MailBody"].ToString();

                            listData.Add(item);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                CommonBL errorlog = new CommonBL();
                errorlog.fnStoreErrorLog(
                    "HomeBL",
                    "GetBirthdayMailDetails",
                    "Exception Message: " + ex.Message +
                    " StackTrace: " + ex.StackTrace,
                    UserId);
            }

            return listData;
        }
        public void SendBirthdayMail(string toMail, string ccMail, string subject, string body)
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
                    foreach (string email in toMail.Split(';'))
                    {
                        if (!string.IsNullOrWhiteSpace(email))
                            mail.To.Add(email.Trim());
                    }

                    // CC
                    foreach (string email in ccMail.Split(';'))
                    {
                        if (!string.IsNullOrWhiteSpace(email))
                            mail.CC.Add(email.Trim());
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
                //using (MailMessage mail = new MailMessage())
                //{
                //    mail.From = new MailAddress(Email, "HRMS");

                //    foreach (string email in toMail.Split(';'))
                //    {
                //        if (!string.IsNullOrWhiteSpace(email))
                //            mail.To.Add(email.Trim());
                //    }

                //    foreach (string email in ccMail.Split(';'))
                //    {
                //        if (!string.IsNullOrWhiteSpace(email))
                //            mail.CC.Add(email.Trim());
                //    }

                //    mail.Subject = subject;
                //    mail.IsBodyHtml = true;

                //    AlternateView htmlView = AlternateView.CreateAlternateViewFromString(
                //        body,
                //        null,
                //        "text/html"
                //    );

                //    LinkedResource image = new LinkedResource(@"C:\Images\birthday.png");
                //    image.ContentId = "BirthdayImage";
                //    image.TransferEncoding = System.Net.Mime.TransferEncoding.Base64;

                //    htmlView.LinkedResources.Add(image);

                //    mail.AlternateViews.Add(htmlView);

                //    using (SmtpClient smtp = new SmtpClient(Host, Port))
                //    {
                //        smtp.UseDefaultCredentials = false;
                //        smtp.Credentials = new NetworkCredential(Email, Password);
                //        smtp.EnableSsl = true;

                //        smtp.Send(mail);
                //    }
                //}

            }
            catch (Exception ex)
            {
                CommonBL errorlog = new CommonBL();
                errorlog.fnStoreErrorLog(
                    "HomeBL",
                    "SendBirthdayMail",
                    ex.Message + ex.StackTrace,
                    UserId);
            }
        }

    }
}
