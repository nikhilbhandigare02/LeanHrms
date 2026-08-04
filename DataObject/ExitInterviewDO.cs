using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataObject
{
    public class ExitInterviewDO
    {
        public int ExitInterviewId { get; set; }
        public int UserId { get; set; }
        public int InterviewerId { get; set; }
        public DateTime InterviewDate { get; set; }
        public string InterviewTime { get; set; }
        public int InterviewStatus { get; set; }
        public int InterviewMode { get; set; }
        public string Location { get; set; }
        public string Notes { get; set; }
        public int InsertedBy { get; set; }
        public DateTime InsertedDate { get; set; }
        public int? UpdatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public bool IsActive { get; set; }

        // Properties to match SP column names (these will be populated by SP)
        public int Interview_Status_id { get; set; }
        public int Interview_Mode_id { get; set; }

        // Display fields
        public string EmployeeName { get; set; }
        public string InterviewerName { get; set; }
        public string Status { get; set; }
        public string Mode { get; set; }
        public string Remarks { get; set; }
    }
}