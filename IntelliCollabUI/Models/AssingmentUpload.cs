using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace IntelliCollabUI.Models
{
    public class AssingmentUpload
    {
        public string AsstId { get; set; }
        public int Sem { get; set; }
        public string Course { get; set; }
        public string Branch { get; set; }
        public string Title { get; set; }
        public string Desc { get; set; }
        public string Session { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string FileName { get; set; }
        public string FileExtension { get; set; }
        public string UploadedBy { get; set; }
        public DateTime UploadedDate { get; set; }
    }

    public class AsstRequest
    {
        public int Sem { get; set; } 
        public string Session { get; set; }
        public string Branch { get; set; }
        public string Course { get; set; }
    }

    public class AsstDelete
    {
        public string ASST_ID { get; set; }
        public string FileName { get; set; }
    }
}