using System;

namespace DataObject
{
    public class NoticePeriodDO
    {
        public DateTime? NoticeStartDate { get; set; }
        public DateTime? NoticeEndDate { get; set; }
        public int? RemainingDays { get; set; }
        public DateTime? LastWorkingDate { get; set; }
        public string AttendanceStatus { get; set; }
    }
}
