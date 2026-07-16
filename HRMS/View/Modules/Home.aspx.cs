using DataObject;
using MySql.Data.MySqlClient;
using ProcessModel;
using System;
using System.Collections.Generic;
using System.Linq;
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

                List<NewsAnnouncementDO> result = objBL.SaveCompanyNews(news);

                string status = result[0].Success;
                string remarks = result[0].Result;

                if (status.Equals("Success", StringComparison.OrdinalIgnoreCase))
                {
                    BindCompanyNews();

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
    }
}