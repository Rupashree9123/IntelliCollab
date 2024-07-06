using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace IntelliCollabUI.Models
{
    public class TblFaculty
    {
        public decimal ROW_ID { get; set; }
        public string F_ID { get; set; }
        public string F_NAME { get; set; }
        public string DEPT { get; set; }
        public string BRANCH_ID { get; set; }
        public DateTime? DATE_OF_JOIN { get; set; }
        public string POST { get; set; }
        public long? MOBILE { get; set; }
        public string F_GMAIL { get; set; }
        public string GENDER { get; set; }
        public int? SEM { get; set; }
        public string SUB_ID { get; set; }
        public string SESSION { get; set; }
        public string PASSWORD { get; set; }
        public string FATHER_NAME { get; set; }
        public string ACTIVE { get; set; }
        public string ADDRESS { get; set; }
        public byte?[] IMAGE { get; set; }
        public string IMAGE_BASE64 { get; set; }
    }
}