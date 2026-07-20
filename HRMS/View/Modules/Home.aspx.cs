using DataObject;
using MySql.Data.MySqlClient;
using ProcessModel;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using static DataObject.HomeDO;
using static ProcessModel.HomeBL;

namespace HRMS.View.Modules
{
    public partial class Home : System.Web.UI.Page
    {
        HomeBL objBL = new HomeBL();
        protected string UserId = null;
        protected void Page_Load(object sender, EventArgs e)
        {
            UserId = Convert.ToString(Session["userId"]);
            if (!IsPostBack)
            {
                eventDate.Attributes["min"] = DateTime.Today.ToString("yyyy-MM-dd");

                if (Session["userId"] == null)
                {
                    Response.Redirect("~/view/authentication/login.aspx", false);
                    return;
                }
                else
                {
                    Session["CurrentPageIndex"] = 0;
                    Session["SearchResults"] = null;

                }


                BindNextHoliday();
                BindAnnouncementCount();
                BindEventCount();
                BindBirthdayCountThisweek();
                BindCompanyNews();
                BindCompanyEvents();
                BindUserName();
                BindBirthdays();
                BindBanner();
            }
        }
        public void BindUserName()
        {
            try
            {
                List<GetUserNameDO> userList = objBL.GetUserName(UserId);

                if (userList != null && userList.Count > 0)
                {
                    litUserName.Text = userList[0].Userfullname;
                }
                else
                {
                    litUserName.Text = "User";
                }
            }
            catch (Exception ex)
            {
                CommonBL errorlog = new CommonBL();
                errorlog.fnStoreErrorLog(
                    "Home",
                    "BindUserName",
                    "Exception Message : " + ex.Message + " StackTrace : " + ex.StackTrace,
                    UserId);

                litUserName.Text = "User";
            }
        }
        public void BindNextHoliday()
        {
            try
            {
                int companyId = Convert.ToInt32(Session["company_id"]);

                List<GetNextHolidayDO> holidayList = objBL.GetNextHoliday(companyId);

                if (holidayList != null && holidayList.Count > 0)
                {
                    litNextHoliday.Text = holidayList[0].holiday_date.ToString("dd MMM");
                }
                else
                {
                    litNextHoliday.Text = "No Holiday";
                }
            }
            catch (Exception ex)
            {
                CommonBL errorlog = new CommonBL();
                errorlog.fnStoreErrorLog("Home", "BindNextHoliday", "Exception Message" + ex.Message + "Strace=" + ex.StackTrace, UserId);
            }
        }

        public void BindAnnouncementCount()
        {
            try
            {

                int companyId = Convert.ToInt32(Session["company_id"]);

                List<AnnouncementCountDO> list = objBL.GetAnnouncementCount(companyId);

                if (list != null && list.Count > 0)
                {
                    litNewsCount.Text = list[0].announcement_count.ToString();
                }
                else
                {
                    litNewsCount.Text = "0";
                }
            }
            catch (Exception ex)
            {
                CommonBL errorlog = new CommonBL();
                errorlog.fnStoreErrorLog("Home", "BindAnnouncementCount", "Exception Message" + ex.Message + "Strace=" + ex.StackTrace, UserId);
            }
        }


        public void BindEventCount()
        {
            try
            {
                int companyId = Convert.ToInt32(Session["company_id"]);

                List<EventCountDO> list = objBL.GetEventCount(companyId);

                if (list != null && list.Count > 0)
                {
                    litUpcomingEvents.Text = list[0].event_count.ToString();
                }
                else
                {
                    litUpcomingEvents.Text = "0";
                }
            }
            catch (Exception ex)
            {
                CommonBL errorlog = new CommonBL();
                errorlog.fnStoreErrorLog("Home", "BindEventCount", "Exception Message" + ex.Message + "Strace=" + ex.StackTrace, UserId);
            }
        }

        public void BindBirthdayCountThisweek()
        {
            try

            {
                int companyId = Convert.ToInt32(Session["company_id"]);

                List<BirthdayCountDO> list = objBL.GetBirthdayCountThisweek();

                if (list != null && list.Count > 0)
                {
                    litUpcomingBirthdays.Text = list[0].birthday_count.ToString();
                }
                else
                {
                    litUpcomingBirthdays.Text = "0";
                }
            }
            catch (Exception ex)
            {
                CommonBL errorlog = new CommonBL();
                errorlog.fnStoreErrorLog("Home", "BindBirthdayCountThisweek", "Exception Message" + ex.Message + "Strace=" + ex.StackTrace, UserId);
            }
        }

        public void BindCompanyNews()
        {
            try
            {
                int companyId = Convert.ToInt32(Session["company_id"]);


                List<NewsAnnouncementDO> news = objBL.GetCompanyNews(companyId);

                // First 3 records
                rptNews.DataSource = news.Take(3).ToList();
                rptNews.DataBind();

                // Remaining records
                rptMoreNews.DataSource = news.Skip(3).ToList();
                rptMoreNews.DataBind();

                // View All count
                litMoreCount.Text = news.Count > 3
                    ? (news.Count - 3).ToString()
                    : "0";
            }
            catch (Exception ex)
            {
                CommonBL errorlog = new CommonBL();
                errorlog.fnStoreErrorLog("Home", "BindCompanyNews", "Exception Message" + ex.Message + "Strace=" + ex.StackTrace, UserId);
            }
        }

        public void BindCompanyEvents()
        {
            try
            {
                int companyId = Convert.ToInt32(Session["company_id"]);


                List<CompanyEventDO> events = objBL.GetCompanyEvents(companyId);

                //First 3
                rptEvents.DataSource = events.Take(3).ToList();
                rptEvents.DataBind();

                //Remaining
                rptMoreEvents.DataSource = events.Skip(3).ToList();
                rptMoreEvents.DataBind();

                //Count
                litMoreEventCount.Text = events.Count > 3
                    ? (events.Count - 3).ToString()
                    : "0";
            }
            catch (Exception ex)
            {
                CommonBL errorlog = new CommonBL();
                errorlog.fnStoreErrorLog(
                    "Home",
                    "BindCompanyEvents",
                    ex.Message + ex.StackTrace,
                    UserId);
            }
        }

        public void BindBirthdays()
        {
            try
            {
                List<BirthdayDO> birthdays = objBL.GetUpcomingBirthdays();

                // First 3 Birthdays
                rptBirthdays.DataSource = birthdays.Take(3).ToList();
                rptBirthdays.DataBind();

                // Remaining Birthdays
                rptMoreBirthdays.DataSource = birthdays.Skip(3).ToList();
                rptMoreBirthdays.DataBind();

                // View All Count
                litMoreBirthdayCount.Text = birthdays.Count > 3
                    ? (birthdays.Count - 3).ToString()
                    : "0";
            }
            catch (Exception ex)
            {
                CommonBL errorlog = new CommonBL();
                errorlog.fnStoreErrorLog(
                    "Home",
                    "BindBirthdays",
                    "Exception Message : " + ex.Message +
                    " StackTrace : " + ex.StackTrace,
                    UserId);
            }
        }

        protected void saveNews(object sender, EventArgs e)
        {
            try
            {
                NewsAnnouncementDO news = new NewsAnnouncementDO();

                news.company_id = Convert.ToInt32(Session["company_id"]);
                news.news_title = newsTitle.Text.Trim();
                news.category = newsTag.SelectedValue;
                news.posted_by = newsPostedBy.Text.Trim();
                news.description = newsDesc.Text.Trim();
                news.inserted_by = UserId;

                // File Upload Base64
                if (fuNewsAttachment.HasFile)
                {
                    news.file_name = fuNewsAttachment.FileName;
                    news.file_type = Path.GetExtension(fuNewsAttachment.FileName);

                    byte[] fileBytes = fuNewsAttachment.FileBytes;

                    news.file_base64 = Convert.ToBase64String(fileBytes);
                }

                List<NewsAnnouncementDO> result = objBL.SaveCompanyNews(news);

                string status = result[0].Success;
                string remarks = result[0].Result;

                if (status.Equals("Success", StringComparison.OrdinalIgnoreCase))
                {
                    BindCompanyNews();
                    BindAnnouncementCount();
                    ClientScript.RegisterStartupScript(
                        this.GetType(),
                        "NewsSaved",
                        "showNewsSavedMessage('" + status + "','" + remarks + "');" +
                        "clearNewsFields();" +
                        "closeModal('modalNews');",
                        true);
                }
                else
                {
                    ClientScript.RegisterStartupScript(
                        this.GetType(),
                        "NewsSaved",
                        "showNewsSavedMessage('" + status + "','" + remarks + "');",
                        true);
                }
            }
            catch (Exception ex)
            {
                CommonBL errorlog = new CommonBL();
                errorlog.fnStoreErrorLog(
                    "Home",
                    "saveNews",
                    ex.Message + ex.StackTrace,
                    UserId);
            }

        }

        protected void saveEvent(object sender, EventArgs e)
        {
            try
            {
                HomeBL objBL = new HomeBL();

                SaveEventDO eventDO = new SaveEventDO();

                eventDO.company_id = Convert.ToInt32(Session["company_id"]);
                eventDO.event_type = eventType.SelectedValue;
                eventDO.event_date = Convert.ToDateTime(eventDate.Text);

                if (!string.IsNullOrWhiteSpace(eventTime.Text))
                    eventDO.event_time = TimeSpan.Parse(eventTime.Text);
                else
                    eventDO.event_time = null;

                eventDO.event_title = eventTitle.Text.Trim();
                eventDO.event_description = eventDesc.Text.Trim();
                eventDO.inserted_by = UserId;


                // File Upload Base64
                if (fueventAttachment.HasFile)
                {
                    eventDO.file_name = fueventAttachment.FileName;
                    eventDO.file_type = Path.GetExtension(fueventAttachment.FileName);

                    byte[] fileBytes = fueventAttachment.FileBytes;

                    eventDO.file_base64 = Convert.ToBase64String(fileBytes);
                }

                List<SaveEventDO> result = objBL.SaveCompanyEvent(eventDO);

                if (result != null && result.Count > 0)
                {
                    string status = result[0].Success;
                    string remarks = result[0].Result;

                    if (status.Equals("Success", StringComparison.OrdinalIgnoreCase))
                    {
                        BindCompanyEvents();
                        BindEventCount();
                        ClientScript.RegisterStartupScript(
                            this.GetType(),
                            "EventSaved",
                            "showNewsSavedMessage('" + status + "','" + remarks + "');closeModal('modalEvent');",
                            true);
                    }
                    else
                    {
                        ClientScript.RegisterStartupScript(
                            this.GetType(),
                            "EventSaved",
                            "showNewsSavedMessage('" + status + "','" + remarks + "');",
                            true);
                    }
                }
                else
                {
                    ClientScript.RegisterStartupScript(
                        this.GetType(),
                        "EventSaved",
                        "showNewsSavedMessage('Failed','Event not added please fill mandatory feilds.');",
                        true);
                }
            }
            catch (Exception ex)
            {
                CommonBL errorlog = new CommonBL();
                errorlog.fnStoreErrorLog(
                    "Home",
                    "saveEvent",
                    "Exception Message: " + ex.Message + " StackTrace: " + ex.StackTrace,
                    UserId);

                ClientScript.RegisterStartupScript(
                    this.GetType(),
                    "EventSaved",
                    "showNewsSavedMessage('Failed','Event not added please fill mandatory feilds.');",
                    true);
            }
        }

        //protected void saveBirthday(object sender, EventArgs e)
        //{
        //    try
        //    {

        //        SaveBirthdayDO birthday = new SaveBirthdayDO();

        //        birthday.emp_code = bdayempCode.Text.Trim();
        //        birthday.employee_name = bdayName.Text.Trim();
        //        birthday.department = bdayDept.SelectedValue;
        //        birthday.DOB = Convert.ToDateTime(bdayDate.Text);
        //        birthday.updated_by = UserId;

        //        List<SaveBirthdayDO> result = objBL.SaveBirthday(birthday);

        //        string status = result[0].Success;
        //        string remarks = result[0].Result;

        //        if (status.Equals("Success", StringComparison.OrdinalIgnoreCase))
        //        {
        //            BindBirthdayCountThisweek();
        //            BindBirthdays();

        //            ClientScript.RegisterStartupScript(
        //                this.GetType(),
        //                "BirthdaySaved",
        //                "showNewsSavedMessage('" + status + "','" + remarks + "');" +
        //                "clearBirthdayFields();" +
        //                "closeModal('modalBirthday');",
        //                true);
        //        }
        //        else
        //        {
        //            ClientScript.RegisterStartupScript(
        //                this.GetType(),
        //                "BirthdaySaved",
        //                "showNewsSavedMessage('" + status + "','" + remarks + "');",
        //                true);
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        CommonBL errorlog = new CommonBL();
        //        errorlog.fnStoreErrorLog(
        //            "Home",
        //            "saveBirthday",
        //            ex.Message + ex.StackTrace,
        //            UserId);
        //    }
        //}

        public void BindBanner()
        {
            try
            {

                List<DashboardBannerDO> list = objBL.GetDashboardBanner();

                foreach (var item in list)
                {
                    switch (item.Category)
                    {
                        case "Event":
                            item.IconHtml = @"<svg width='16' height='16' viewBox='0 0 24 24' fill='none' stroke='currentColor' stroke-width='2'>
                                        <path d='M8 2v4M16 2v4M3 10h18M5 4h14a2 2 0 0 1 2 2v14a2 2 0 0 1-2 2H5a2 2 0 0 1-2-2V6a2 2 0 0 1 2-2z'/>
                                      </svg>";
                            break;

                        case "News":
                            item.IconHtml = @"<svg width='16' height='16' viewBox='0 0 24 24' fill='none' stroke='currentColor' stroke-width='2'>
                                        <path d='M14 2H6a2 2 0 0 0-2 2v16a2 2 0 0 0 2 2h12a2 2 0 0 0 2-2V8z'/>
                                        <polyline points='14,2 14,8 20,8'/>
                                        <line x1='16' y1='13' x2='8' y2='13'/>
                                        <line x1='16' y1='17' x2='8' y2='17'/>
                                      </svg>";
                            break;

                        case "Birthday":
                            item.IconHtml = @"<svg width='16' height='16' viewBox='0 0 24 24' fill='none' stroke='currentColor' stroke-width='2'>
                                        <path d='M12 6c1 0 1.5-1 1-2s-1-1-1-2c-.5 1-1 1-1 2s0 2 1 2z'/>
                                        <path d='M4 22v-8a2 2 0 0 1 2-2h12a2 2 0 0 1 2 2v8'/>
                                        <path d='M2 22h20'/>
                                      </svg>";
                            break;

                        case "Holiday":
                            item.IconHtml = @"<svg width='16' height='16' viewBox='0 0 24 24' fill='none' stroke='currentColor' stroke-width='2'>
                                        <rect x='3' y='4' width='18' height='18' rx='2'/>
                                        <line x1='16' y1='2' x2='16' y2='6'/>
                                        <line x1='8' y1='2' x2='8' y2='6'/>
                                        <line x1='3' y1='10' x2='21' y2='10'/>
                                      </svg>";
                            break;

                        default:
                            item.IconHtml = "";
                            break;
                    }
                }

                rptBanner.DataSource = list;
                rptBanner.DataBind();

                rptDots.DataSource = list;
                rptDots.DataBind();
            }
            catch (Exception ex)
            {
                CommonBL errorlog = new CommonBL();
                errorlog.fnStoreErrorLog(
                    "Home",
                    "BindBanner",
                    "Exception Message : " + ex.Message +
                    " StackTrace : " + ex.StackTrace,
                    UserId);
            }
        }

        protected void rptNews_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            if (e.CommandName == "ViewNews")
            {
                litNewsModalTitle.Text = "View News / Announcement";

                int news_announcement_id = Convert.ToInt32(e.CommandArgument);

                List<NewsAnnouncementDO> newsList = objBL.GetNewsById(news_announcement_id);

                if (newsList != null && newsList.Count > 0)
                {
                    NewsAnnouncementDO news = newsList[0];

                    newsTitle.Text = news.news_title;
                    newsTag.SelectedValue = news.category;
                    newsPostedBy.Text = news.posted_by;
                    newsDesc.Text = news.description;

                    newsTitle.ReadOnly = true;
                    newsPostedBy.ReadOnly = true;
                    newsDesc.ReadOnly = true;
                    newsTag.Enabled = false;

                    btnSaveNews.Visible = false;
                    fuNewsAttachment.Visible = false;
                    if (!string.IsNullOrEmpty(news.file_base64))
                    {
                        btnDownloadAttachment.Visible = true;
                        btnDownloadAttachment.CommandArgument = news.news_announcement_id.ToString();
                    }
                    else
                    {
                        btnDownloadAttachment.Visible = false;
                    }
                    ScriptManager.RegisterStartupScript(
                        this,
                        GetType(),
                        "OpenModal",
                        "openModal('modalNews');",
                        true);
                }
            }
        }

        protected void btnDownloadAttachment_Click(object sender, EventArgs e)
        {
            Button btn = (Button)sender;

            int newsId = Convert.ToInt32(btn.CommandArgument);

            List<NewsAnnouncementDO> newsList = objBL.GetNewsById(newsId);

            if (newsList != null && newsList.Count > 0)
            {
                NewsAnnouncementDO news = newsList[0];

                byte[] bytes = Convert.FromBase64String(news.file_base64);

                Response.Clear();
                Response.ContentType = news.file_type;
                Response.AddHeader("Content-Disposition",
                    "attachment; filename=" + news.file_name);
                Response.BinaryWrite(bytes);
                Response.End();
            }
        }

        private void ResetNewsModal()
        {
            litNewsModalTitle.Text = "Add News / Announcement";

            newsTitle.Text = "";
            newsTag.SelectedIndex = 0;
            newsPostedBy.Text = "";
            newsDesc.Text = "";

            newsTitle.ReadOnly = false;
            newsPostedBy.ReadOnly = false;
            newsDesc.ReadOnly = false;
            newsTag.Enabled = true;

            btnSaveNews.Visible = true;
            fuNewsAttachment.Visible = true;
            btnDownloadAttachment.Visible = false;
        }
        protected void btnAddNews_Click(object sender, EventArgs e)
        {
            ResetNewsModal();

            ScriptManager.RegisterStartupScript(
                this,
                GetType(),
                "OpenNews",
                "openModal('modalNews');",
                true);
        }

        //protected void rptEvents_ItemCommand(object source, RepeaterCommandEventArgs e)
        //{
        //    if (e.CommandName == "ViewEvents")
        //    {
        //        int event_mast_id = Convert.ToInt32(e.CommandArgument);

        //        List<CompanyEventDO> eventList = objBL.GetEventById(event_mast_id);

        //        if (eventList != null && eventList.Count > 0)
        //        {
        //            CompanyEventDO ev = eventList[0];

        //            eventTitle.Text = ev.event_title;
        //            eventType.SelectedValue = ev.event_type;
        //            eventDate.Text = Convert.ToDateTime(ev.event_date).ToString("yyyy-MM-dd");
        //            eventTime.Text = ev.eventtime;
        //            eventDesc.Text = ev.event_description;

        //            eventTitle.ReadOnly = true;
        //            eventType.Enabled = false;
        //            eventDate.ReadOnly = true;
        //            eventTime.ReadOnly = true;
        //            eventDesc.ReadOnly = true;

        //            btnSaveEvent.Visible = false;
        //            fueventAttachment.Visible = false;

        //            if (!string.IsNullOrEmpty(ev.file_base64))
        //            {
        //                btnDownloadEvent.Visible = true;
        //                btnDownloadEvent.CommandArgument = ev.event_mast_id.ToString();
        //            }
        //            else
        //            {
        //                btnDownloadEvent.Visible = false;
        //            }

        //            ScriptManager.RegisterStartupScript(
        //                this,
        //                GetType(),
        //                "OpenEvent",
        //                "openModal('modalEvent');",
        //                true);
        //        }
        //    }
        //}
        protected void rptEvents_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            if (e.CommandName == "ViewEvents")
            {
                litEventModalTitle.Text = "View Event";

                string[] args = e.CommandArgument.ToString().Split('|');

                string recordType = args[0];
                int id = Convert.ToInt32(args[1]);

                if (recordType == "Event")
                {
                    List<CompanyEventDO> eventList = objBL.GetEventById(id);

                    if (eventList != null && eventList.Count > 0)
                    {
                        CompanyEventDO ev = eventList[0];

                        eventTitle.Text = ev.event_title;
                        eventType.SelectedValue = ev.event_type;
                        eventDate.Text = Convert.ToDateTime(ev.event_date).ToString("yyyy-MM-dd");
                        eventTime.Text = ev.eventtime;
                        eventDesc.Text = ev.event_description;

                        eventTitle.ReadOnly = true;
                        eventType.Enabled = false;
                        eventDate.ReadOnly = true;
                        eventTime.ReadOnly = true;
                        eventDesc.ReadOnly = true;

                        btnSaveEvent.Visible = false;
                        fueventAttachment.Visible = false;

                        if (!string.IsNullOrEmpty(ev.file_base64))
                        {
                            btnDownloadEvent.Visible = true;
                            btnDownloadEvent.CommandArgument = ev.event_mast_id.ToString();
                        }
                        else
                        {
                            btnDownloadEvent.Visible = false;
                        }

                        ScriptManager.RegisterStartupScript(
                            this,
                            GetType(),
                            "OpenEvent",
                            "openModal('modalEvent');",
                            true);
                    }
                }
                else if (recordType == "Holiday")
                {
                    litEventModalTitle.Text = "View Holiday";

                    List<HolidayDO> holidayList = objBL.GetHolidayById(id);

                    if (holidayList != null && holidayList.Count > 0)
                    {
                        HolidayDO h = holidayList[0];

                        eventTitle.Text = h.holiday_name;
                        eventType.SelectedValue = "Holiday";
                        eventDate.Text = Convert.ToDateTime(h.holiday_date).ToString("yyyy-MM-dd");
                        eventTime.Text = "";
                        eventDesc.Text = h.holiday_day;

                        eventTitle.ReadOnly = true;
                        eventType.Enabled = false;
                        eventDate.ReadOnly = true;
                        eventTime.ReadOnly = true;
                        eventDesc.ReadOnly = true;

                        btnSaveEvent.Visible = false;
                        fueventAttachment.Visible = false;
                        btnDownloadEvent.Visible = false;

                        ScriptManager.RegisterStartupScript(
                            this,
                            GetType(),
                            "OpenEvent",
                            "openModal('modalEvent');",
                            true);
                    }
                }
            }
        }
        protected void btnDownloadEvent_Click(object sender, EventArgs e)
        {
            Button btn = (Button)sender;

            int EventId = Convert.ToInt32(btn.CommandArgument);

            List<CompanyEventDO> EventList = objBL.GetEventById(EventId);

            if (EventList != null && EventList.Count > 0)
            {
                CompanyEventDO news = EventList[0];

                byte[] bytes = Convert.FromBase64String(news.file_base64);

                Response.Clear();
                Response.ContentType = news.file_type;
                Response.AddHeader("Content-Disposition",
                    "attachment; filename=" + news.file_name);
                Response.BinaryWrite(bytes);
                Response.End();
            }
        }

        private void ResetEventModal()
        {

            litEventModalTitle.Text = "Add Event / Holiday";

            eventTitle.Text = "";
            eventDate.Text = "";
            eventTime.Text = "";
            eventDesc.Text = "";
            eventType.SelectedIndex = 0;

            eventTitle.ReadOnly = false;
            eventDate.ReadOnly = false;
            eventTime.ReadOnly = false;
            eventDesc.ReadOnly = false;
            eventType.Enabled = true;

            fueventAttachment.Visible = true;
            btnSaveEvent.Visible = true;
            btnDownloadEvent.Visible = false;
        }
        protected void btnAddEvent_Click(object sender, EventArgs e)
        {
            ResetEventModal();

            ScriptManager.RegisterStartupScript(
                this,
                GetType(),
                "OpenEvent",
                "openModal('modalEvent');",
                true);
        }

        protected void btnsendmail_Click(object sender, EventArgs e)
        {
            try
            {
                List<BirthdayMailDO> birthdayList = objBL.GetBirthdayMailDetails();

                if (birthdayList != null && birthdayList.Count > 0)
                {
                    foreach (BirthdayMailDO item in birthdayList)
                    {
                        objBL.SendBirthdayMail(
                            item.ToMail,
                            item.CcMail,
                            item.Subject,
                            item.MailBody);
                    }

                    ClientScript.RegisterStartupScript(
                        this.GetType(),
                        "BirthdayMail",
                        "showNewsSavedMessage('Success','Birthday mail sent successfully.');",
                        true);
                }
                else
                {
                    ClientScript.RegisterStartupScript(
                        this.GetType(),
                        "BirthdayMail",
                        "showNewsSavedMessage('error','No birthdays today.');",
                        true);
                }
            }
            catch (Exception ex)
            {
                CommonBL errorlog = new CommonBL();
                errorlog.fnStoreErrorLog(
                    "Home",
                    "btnsendmail_Click",
                    ex.Message + ex.StackTrace,
                    UserId);

                ClientScript.RegisterStartupScript(
                    this.GetType(),
                    "BirthdayMailError",
                    "showNewsSavedMessage('error','Failed to send birthday mail.');",
                    true);
            }
        }


    }
}