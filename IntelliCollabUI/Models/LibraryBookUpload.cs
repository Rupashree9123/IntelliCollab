using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace IntelliCollabUI.Models
{
    public class LibraryBookUpload
    {
        public string SaveObject { get; set; } // This will contain the JSON string representation of your data
        public HttpPostedFileBase UploadedImage { get; set; } // This will contain the uploaded image file
        public HttpPostedFileBase UploadedFile { get; set; } // This will contain the uploaded image file
    }

    public class LibraryBook
    {
        public DateTime Date { get; set; }
        public string LibId { get; set; }
        public string Course { get; set; }
        public string Branch { get; set; }
        public string Title { get; set; }
        public string Desc { get; set; }
        public string CoverImage { get; set; }
        public string CoverImageName { get; set; }
        public string CoverImageExtension { get; set; }
        public string FileName { get; set; }
        public string FileExtension { get; set; }
    }
}