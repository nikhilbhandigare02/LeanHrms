using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataObject
{
    public class HomeDO
    {
        public class GetUserNameDO
        {
            public string Userfullname { get; set; }
        }
        public class GetNextHolidayDO
        {
            public DateTime holiday_date { get; set; }
        }
        public class AnnouncementCountDO
        {
            public int announcement_count { get; set; }
        }
        public class EventCountDO
        {
            public int event_count { get; set; }
        }
        public class BirthdayCountDO
        {
            public int birthday_count { get; set; }
        }
        public class NewsAnnouncementDO
        {
            public int news_announcement_id { get; set; }

            public int company_id { get; set; }
            public string news_title { get; set; }
            public string category { get; set; }
            public string posted_by { get; set; }
            public string description { get; set; }
            public DateTime inserted_date { get; set; }
            public string inserted_by { get; set; }

            public string Success { get; set; }
            public string Result { get; set; }
            //Calculated fields
            public string PostedOn { get; set; }
            public string Initials { get; set; }
        }
        public class CompanyEventDO
        {
            public int event_mast_id { get; set; }

            public string event_type { get; set; }
            public DateTime event_date { get; set; }
            public TimeSpan? event_time { get; set; }

            public string event_title { get; set; }
            public string event_description { get; set; }

            public DateTime inserted_date { get; set; }

            //Display
            public string EventDate { get; set; }
            public string EventDay { get; set; }
        }

        public class BirthdayDO
        {
            public int user_id { get; set; }

            public string EmployeeName { get; set; }

            public string Department { get; set; }

            public DateTime DOB { get; set; }

            // Used for UI
            public string Initials { get; set; }

            // Today / 17 Jul / 25 Aug
            public string DateLabel { get; set; }
            public string BadgeClass { get; set; }
        }

        public class SaveEventDO
        {
            public int company_id { get; set; }

            public string event_type { get; set; }

            public DateTime event_date { get; set; }

            public TimeSpan? event_time { get; set; }

            public string event_title { get; set; }

            public string event_description { get; set; }

            public string inserted_by { get; set; }

            public string Success { get; set; }

            public string Result { get; set; }
        }
        //public class SaveBirthdayDO
        //{
        //    public int user_id { get; set; }

        //    public string emp_code { get; set; }

        //    public string employee_name { get; set; }
        //    public string department { get; set; }
        //    public DateTime DOB { get; set; }
        //    public string updated_by { get; set; }

        //    public string Success { get; set; }
        //    public string Result { get; set; }
        //}
        public class DashboardBannerDO
        {
            public int DisplayOrder { get; set; }
            public string Category { get; set; }
            public string Title { get; set; }
            public string Meta { get; set; }
            public string Description { get; set; }
            public string ImageUrl { get; set; }
            public string Background { get; set; }
            public DateTime inserted_date { get; set; }
            // Used only for UI
            public string IconHtml { get; set; }
        }
    }
}
