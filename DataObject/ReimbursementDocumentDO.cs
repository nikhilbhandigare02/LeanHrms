using System;

namespace DataObject
{
    public class ReimbursementOwnerDO
    {
        public int ReimbursementId { get; set; }
        public int UserId { get; set; }
        public string ReimbursementNumber { get; set; }
    }

    public class ReimbursementDocumentDO
    {
        public int UserDocDetId { get; set; }
        public int UserId { get; set; }
        public int DocumentMasterId { get; set; }
        public string filepath { get; set; }
        public string FileName { get; set; }
        public string FileExtension { get; set; }
        public int InsertedBy { get; set; }
        public DateTime InsertedDate { get; set; }
        public string DocumentType { get; set; }
    }
}
