using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataObject
{
    public class AccountDashboardDO
    {
        public class TotalDisbursedcountDO
        {
            public decimal TotalSalaryDisbursed { get; set; }
        }

        public class TotalReimburesementdcountDO
        {
            public decimal TotalReimbursement { get; set; }
        }

        public class TotalActiveEmployeecountDO
        {
            public int ActiveEmployeeCount { get; set; }
        }

        public class EmployeeSalaryDetailsDO
        {
            public int salary_slip_details_id { get; set; }
            public string username { get; set; }
            public string employeecode { get; set; }
            public decimal basic_salary { get; set; }
            public decimal special_allowance { get; set; }
            public decimal total_deductions { get; set; }
            public decimal net_pay { get; set; }
            public string status { get; set; }
        }

        public class EmployeeReimbursementDO
        {
            public string employee_name { get; set; }

            public string employee_code { get; set; }

            public string claim_type { get; set; }

            public decimal claim_amount { get; set; }

            public DateTime claim_date { get; set; }

            public string status { get; set; }
        }
    }
}
