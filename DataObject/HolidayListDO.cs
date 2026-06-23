using System;

namespace DataObject
{
    public class HolidayListDO
    {
        public int holiday_id { get; set; }
        public int sr_no { get; set; }
        public DateTime holiday_date { get; set; }
        public string holiday_day { get; set; }
        public string holiday_name { get; set; }
        public int company_id { get; set; }
        public int is_active { get; set; }
        public int inserted_by { get; set; }
        public DateTime inserted_date { get; set; }
        public int updated_by { get; set; }
        public DateTime updated_date { get; set; }
    }
}
