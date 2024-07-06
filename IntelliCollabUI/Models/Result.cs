using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace IntelliCollabUI.Models
{
    public class Result
    {
         public string RESULT_ID{get;set;}
         public string BRANCH_ID{get;set;}
         public string FILE_NAMES{get;set;}
         public string FILE_TYPE{get;set;}
         public DateTime UPLOAD_DATE{get;set;}
         public string UPLOAD_BY{get;set;}
         public string SESSION_DATE{get;set;}
         public decimal? SEM {get;set;}
         public string TITLE{get;set;}
         public string DESCRIPTION{get;set;}
         public string EXAM_TYPE{get;set;}
    }
}