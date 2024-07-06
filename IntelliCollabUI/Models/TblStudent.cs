using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace IntelliCollabUI.Models
{
    public class TblStudent
    {
        public string ID { get; set; }
        public string NAME { get; set; }
        public string GMAIL { get; set; }
        public string SESSION { get; set; }
        public string S_REG_NO { get; set; }
        public string ROLL_NO { get; set; }
        public string GENDER { get; set; }
        public decimal? MOBILE_NO { get; set; }
        public string BRANCH { get; set; }
        public string ACTIVE { get; set; }
        public string ADDRESS { get; set; }
        public byte[] IMAGE { get; set; }
        public string PASSWORD { get; set; }
        public string FATHER_NAME { get; set; }
        public DateTime? ADDS_DATE { get; set; }
        public DateTime? DOB { get; set; }
    }
}