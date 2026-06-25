using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataObject
{
    public class AppraisalDetailsDO
    {
        public int appraisal_id { get; set; }
        public int user_id { get; set; }
        public string employee_name { get; set; }  
        public DateTime appraisal_effective_date { get; set; }
        public DateTime salary_revision_date { get; set; }
        public decimal appraisal_ctc { get; set; }
        public decimal gross_salary {  get; set; }
        public decimal net_salary { get; set; }
        public decimal increament_amount { get; set; }
        public decimal increament_percentage { get; set; }
        public int created_by { get; set; }
        public DateTime created_date { get; set; }
        public bool is_active { get; set; }
        public decimal oldCTC { get; set; }

    }
}
