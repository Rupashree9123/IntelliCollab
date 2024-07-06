using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace IntelliCollabUI.Models
{
    public class UserProfile
    {
        public string UserName { get; set; }
        public string  FatherName { get; set; }
        public string UserId { get; set; }
        public string RegistrationNo { get; set; }
        public string StrDateOfBirth { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public DateTime? CreateDate { get; set; }
        public string Branch { get; set; }
        public string Session { get; set; }
        public string Gender { get; set; }
        public long? MobileNo { get; set; }
        public HttpPostedFileBase UploadedImage { get; set; }
        public byte?[] Image { get; set; }
        public string Base64Image { get; set; }
        public bool HasImage { get; set; }
        public string Address { get; set; }
        public string Email { get; set; }
        public string UserType { get; set; }
        public string Post { get; set; }
    }

    public class FormDataModel
    {
        public string SaveObject { get; set; } // This will contain the JSON string representation of your data
        public HttpPostedFileBase UploadedImage { get; set; } // This will contain the uploaded image file
    }
}