using System;

namespace DataObject
{
    public class EmpAttendanceDO
    {
        public int UserId { get; set; }
        public string EmployeeCode { get; set; }
        public string EmployeeName { get; set; }
        public DateTime LoginDate { get; set; }
        public TimeSpan? LoginTime { get; set; }
        public TimeSpan? LogoutTime { get; set; }

        // Computed in sp_get_attendance_list (9:30 threshold lives in SQL, not C#).
        public string WorkedHoursDisplay { get; set; }
        public bool? IsBelowMinimum { get; set; }
    }
}
