
using System;

namespace DataObject
{
    public class SalaryCalculationDO
    {
        public int remuneration_id { get; set; }
        public int user_id { get; set; }
        public string employee_code { get; set; }
        public string user_fullname { get; set; }
        public string user_mail_id { get; set; }
        public decimal monthly_salary { get; set; }
        public decimal per_day_salary { get; set; }
        public int leave_deduction_days { get; set; }
        public decimal deducted_amount { get; set; }
        public decimal other_deduction { get; set; }
        public decimal deducted_monthly_salary { get; set; }
        public int total_working_days { get; set; }
        public int present_days { get; set; }
        public int absent_days { get; set; }
    public string verification_status { get; set; }
}
}
