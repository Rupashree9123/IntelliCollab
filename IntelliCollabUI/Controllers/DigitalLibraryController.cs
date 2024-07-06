using IntelliCollabUI.Models;
using IntelliCollabUI.Utility;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace IntelliCollabUI.Controllers
{
    [Authentication]
    [RoleFilter]
    public class DigitalLibraryController : Controller
    {
        // GET: DigitalLibrary
        //public ActionResult DigitalLibraryBoard()
        //{
        //    return View();
        //}
        public ActionResult DigitalLibraryDashBoard()
        {
            return View();
        }
        public ActionResult LibraryView()
        {
            return View();
        }
        public ActionResult AssingmentView()
        {
            return View();
        }
        public ActionResult AssingmentUpload()
        {
            return View();
        }

        #region Library
        public ActionResult LibraryUpload()
        {
            return View();
        }
        public ActionResult UploadBook(LibraryBookUpload req)
        {
            string msg = "";
            try
            {
                string fileName = string.Empty;
                string fileContentType = string.Empty; 
                string fullPath = string.Empty;

                string imgName = string.Empty;
                string imgContentType = string.Empty;
                string imgPath = string.Empty;
                string UserId = Convert.ToString(Session["UserID"]);

                LibraryBook libBook = new LibraryBook();
                string path = Server.MapPath(@"~/App_Data/LibraryStorage");
                
                //uploading File
                var uploadFile = req.UploadedFile;
                if (uploadFile != null && uploadFile.ContentLength > 0)
                {
                    fileName = Path.GetFileName(uploadFile.FileName);
                    fullPath = Path.Combine(path, fileName);
                    var a = uploadFile.ContentType;
                    fileContentType = uploadFile.ContentType;
                    uploadFile.SaveAs(fullPath);
                }
                //uploading Image
                var uploadImage = req.UploadedImage;
                if (uploadImage != null && uploadImage.ContentLength > 0)
                {
                    imgName = Path.GetFileName(uploadImage.FileName);
                    imgPath = Path.Combine(path, imgName);
                    imgContentType = uploadImage.ContentType;
                    uploadImage.SaveAs(imgPath);
                }
                
                var saveObjectJson = req.SaveObject;
                libBook = JsonConvert.DeserializeObject<LibraryBook>(saveObjectJson);
                //db work
                
                using (var conns = new SqlConnection(DbConnect.ConnectionString))
                {
                    conns.Open();
                    using (var cmd = new SqlCommand())
                    {
                        cmd.Connection = conns;
                        cmd.CommandType = CommandType.StoredProcedure;

                        cmd.CommandText = "SP_Library_BookUpload_Insert_Update";
                        // Add parameters
                        cmd.Parameters.AddWithValue("@BRANCH", libBook.Branch);
                        cmd.Parameters.AddWithValue("@SUB_ID", libBook.Course);
                        cmd.Parameters.AddWithValue("@UPLOADED_BY", UserId);
                        cmd.Parameters.AddWithValue("@FILE_NAME", fileName);
                        cmd.Parameters.AddWithValue("@FILE_TYPE", fileContentType);
                        cmd.Parameters.AddWithValue("@IMAGE_NAME", imgName);
                        cmd.Parameters.AddWithValue("@IMAGE_TYPE", imgContentType);
                        cmd.Parameters.AddWithValue("@TITLE", libBook.Title);
                        cmd.Parameters.AddWithValue("@DESCP", libBook.Desc);
                        cmd.Parameters.AddWithValue("@LIB_ID", libBook.LibId);
                        

                        cmd.Parameters.Add(new SqlParameter()
                        {
                            ParameterName = "@RESULT",
                            Direction = ParameterDirection.Output,
                            Size = 50
                        });

                        cmd.ExecuteNonQuery();

                        //Retrieve the value of the output parameter
                        msg = Convert.ToString(cmd.Parameters["@RESULT"].Value);



                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return Json(msg, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public ActionResult ViewBook(LibraryBook libBook)
        {
            List<LibraryBook> req = new List<LibraryBook>();
            try
            {
                string path = Server.MapPath(@"~/App_Data/LibraryStorage");
                DataSet getInfo = new DataSet();
                using (var conns = new SqlConnection(DbConnect.ConnectionString))
                {
                    conns.Open();
                    using (var cmd = new SqlCommand())
                    {
                        cmd.Connection = conns;
                        cmd.CommandType = CommandType.StoredProcedure;

                        cmd.CommandText = "SP_Library_Book_View";
                        
                        // Add parameters
                        
                        cmd.Parameters.AddWithValue("@SUB_ID", libBook.Course);
                        cmd.Parameters.AddWithValue("@Branch", libBook.Branch);
                        

                        using (var ad = new SqlDataAdapter(cmd))
                        {
                            ad.Fill(getInfo);
                        }
                    }
                }

                if(getInfo!=null && getInfo.Tables.Count>0 && getInfo.Tables[0].Rows.Count>0)
                {
                    req = getInfo.Tables[0].AsEnumerable().Select(x => new LibraryBook
                    {
                        Branch = x.Field<string>("BRANCH"),
                        Course = x.Field<string>("SUBJECT"),
                        CoverImageName = x.Field<string>("COVER_IMAGE_NAME"),
                        CoverImageExtension = x.Field<string>("COVER_IMAGE_TYPE"),
                        CoverImage = ConvertBase64(Path.Combine(path, x.Field<string>("COVER_IMAGE_NAME"))),
                        FileName = x.Field<string>("FILE_NAMES"),
                        FileExtension = x.Field<string>("FILE_TYPE"),
                        Desc = x.Field<string>("DESCRIPTION"),
                        Title = x.Field<string>("TITLE"),
                        LibId = x.Field<string>("LIB_ID")
                    }).ToList() ;
                }
                
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return Json(req, JsonRequestBehavior.AllowGet);
        }

        [HttpDelete]
        public ActionResult DeleteBook(AsstDelete req)
        {
            string msg = string.Empty;
            using (var conns = new SqlConnection(DbConnect.ConnectionString))
            {
                conns.Open();
                using (var cmd = new SqlCommand())
                {
                    cmd.Connection = conns;
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.CommandText = "SP_LibBook_Delete";

                    // Add parameters

                    cmd.Parameters.AddWithValue("@LIB_ID", req.ASST_ID);
                    cmd.Parameters.Add(new SqlParameter()
                    {
                        ParameterName = "@RESULT",
                        Direction = ParameterDirection.Output,
                        Size = 50
                    });
                    cmd.ExecuteNonQuery();
                    //Retrieve the value of the output parameter
                    msg = Convert.ToString(cmd.Parameters["@RESULT"].Value);
                }
            }
            string path = Server.MapPath(@"~/App_Data/LibraryStorage");
            string FileName = Path.GetFileName(req.FileName);
            string fullPath = Path.Combine(path, FileName);

            if (!string.IsNullOrEmpty(msg) && msg.Equals("OK") && System.IO.File.Exists(fullPath))
            {
                System.IO.File.Delete(fullPath);
                // Optionally, you can handle success or error after deletion
                msg = "File deleted successfully.";
            }
            else
            {
                msg = "File does not exist.";
            }
            return Json(msg, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult Download(string Filename,string contentType)
        {
            string base64String = string.Empty;
            try
            {
                string path = Server.MapPath(@"~/App_Data/LibraryStorage");
                string FileName = Path.GetFileName(Filename);
                string fullPath = Path.Combine(path, FileName);

                return File(fullPath, contentType, FileName);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region Assingment
        [HttpPost]
        public ActionResult UploadAssingment(LibraryBookUpload req)
        {
            string msg = "";
            try
            {
                
                string fileName = string.Empty;
                string fileContentType = string.Empty;
                string fullPath = string.Empty;

                string UserId = Convert.ToString(Session["UserID"]);

                AssingmentUpload Asst = new AssingmentUpload();
                string path = Server.MapPath(@"~/App_Data/AssingmentStorage");
                var saveObjectJson = req.SaveObject;
                Asst = JsonConvert.DeserializeObject<AssingmentUpload>(saveObjectJson);
                if (Asst.StartDate!=null && Asst.EndDate !=null && Asst.Branch != null && Asst.Course !=null)
                {
                    //uploading File
                    var uploadFile = req.UploadedFile;
                    if (uploadFile != null && uploadFile.ContentLength > 0)
                    {
                        fileName = Path.GetFileName(uploadFile.FileName);
                        fullPath = Path.Combine(path, fileName);
                        var a = uploadFile.ContentType;
                        fileContentType = uploadFile.ContentType;
                        uploadFile.SaveAs(fullPath);
                    }



                    //db work
                    using (var conns = new SqlConnection(DbConnect.ConnectionString))
                    {
                        conns.Open();
                        using (var cmd = new SqlCommand())
                        {
                            cmd.Connection = conns;
                            cmd.CommandType = CommandType.StoredProcedure;

                            cmd.CommandText = "SP_Assingment_Insert_Update";
                            // Add parameters
                            cmd.Parameters.AddWithValue("@BRANCH", Asst.Branch);
                            cmd.Parameters.AddWithValue("@SUB_ID", Asst.Course);
                            cmd.Parameters.AddWithValue("@UPLOADED_BY", UserId);
                            cmd.Parameters.AddWithValue("@FILE_NAME", fileName);
                            cmd.Parameters.AddWithValue("@FILE_TYPE", fileContentType);
                            cmd.Parameters.AddWithValue("@START_DATE", Asst.StartDate);
                            cmd.Parameters.AddWithValue("@END_DATE", Asst.EndDate);
                            cmd.Parameters.AddWithValue("@TITLE", Asst.Title);
                            cmd.Parameters.AddWithValue("@DESCP", Asst.Desc);
                            cmd.Parameters.AddWithValue("@ASST_ID", Asst.AsstId);
                            cmd.Parameters.AddWithValue("@SESSION", Asst.Session);
                            cmd.Parameters.AddWithValue("@SEM", Asst.Sem);

                            cmd.Parameters.Add(new SqlParameter()
                            {
                                ParameterName = "@RESULT",
                                Direction = ParameterDirection.Output,
                                Size = 50
                            });

                            cmd.ExecuteNonQuery();

                            //Retrieve the value of the output parameter
                            msg = Convert.ToString(cmd.Parameters["@RESULT"].Value);
                        }
                    }
                }
                else
                {
                    msg = "fill proper parameter";
                }
                
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return Json(msg, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult ViewAssingment(AsstRequest asstReq)
        {
            List<AssingmentUpload> req = new List<AssingmentUpload>();
            try
            {
                string path = Server.MapPath(@"~/App_Data/AssingmentStorage");
                DataSet getInfo = new DataSet();
                using (var conns = new SqlConnection(DbConnect.ConnectionString))
                {
                    conns.Open();
                    using (var cmd = new SqlCommand())
                    {
                        cmd.Connection = conns;
                        cmd.CommandType = CommandType.StoredProcedure;

                        cmd.CommandText = "SP_Assingment_Disp";

                        // Add parameters

                        cmd.Parameters.AddWithValue("@Sem", asstReq.Sem);
                        cmd.Parameters.AddWithValue("@Session", asstReq.Session);
                        cmd.Parameters.AddWithValue("@Branch", asstReq.Branch);
                        cmd.Parameters.AddWithValue("@Cource", asstReq.Course);


                        using (var ad = new SqlDataAdapter(cmd))
                        {
                            ad.Fill(getInfo);
                        }
                    }
                }

                if (getInfo != null && getInfo.Tables.Count > 0 && getInfo.Tables[0].Rows.Count > 0)
                {
                    req = getInfo.Tables[0].AsEnumerable().Select(x => new AssingmentUpload
                    {
                        Branch = x.Field<string>("BRANCH_ID"),
                        Course = x.Field<string>("SUB_ID"),
                        AsstId = x.Field<string>("ASST_ID"),
                        StartDate = x.Field<DateTime>("START_DATE"),
                        EndDate = x.Field<DateTime>("END_DATE"),
                        FileName = x.Field<string>("FILE_NAMES"),
                        FileExtension = x.Field<string>("FILE_TYPE"),
                        UploadedBy = x.Field<string>("UPLOAD_BY"),
                        Sem = x.Field<int>("SEM"),
                        Session = x.Field<string>("SESSION_DATE"),
                        Desc = x.Field<string>("DESCRIPTION"),
                        Title = x.Field<string>("TITLE"),
                        UploadedDate = x.Field<DateTime>("UPLOAD_DATE")
                    }).ToList();
                }

            }
            catch (Exception ex)
            {
                throw ex;
            }
            return Json(req, JsonRequestBehavior.AllowGet);
        }
        [HttpGet]
        public ActionResult DownloadAsst(string Filename, string contentType)
        {
            string base64String = string.Empty;
            try
            {
                string path = Server.MapPath(@"~/App_Data/AssingmentStorage");
                string FileName = Path.GetFileName(Filename);
                string fullPath = Path.Combine(path, FileName);

                return File(fullPath, contentType, FileName);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [HttpDelete]
        public ActionResult DeleteAssingment(AsstDelete req)
        {
            string msg = string.Empty;
            using (var conns = new SqlConnection(DbConnect.ConnectionString))
            {
                conns.Open();
                using (var cmd = new SqlCommand())
                {
                    cmd.Connection = conns;
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.CommandText = "SP_Assingment_Delete";

                    // Add parameters

                    cmd.Parameters.AddWithValue("@ASST_ID", req.ASST_ID);
                    cmd.Parameters.Add(new SqlParameter()
                    {
                        ParameterName = "@RESULT",
                        Direction = ParameterDirection.Output,
                        Size = 50
                    });
                    cmd.ExecuteNonQuery();
                    //Retrieve the value of the output parameter
                    msg = Convert.ToString(cmd.Parameters["@RESULT"].Value);
                }
            }
            string path = Server.MapPath(@"~/App_Data/AssingmentStorage");
            string FileName = Path.GetFileName(req.FileName);
            string fullPath = Path.Combine(path, FileName);

            if (!string.IsNullOrEmpty(msg) && msg.Equals("OK") && System.IO.File.Exists(fullPath))
            {
                System.IO.File.Delete(fullPath);
                // Optionally, you can handle success or error after deletion
                msg = "File deleted successfully.";
            }
            else
            {
                msg = "File does not exist." ;
            }
            return Json(msg,JsonRequestBehavior.AllowGet);
        }
        #endregion
        public string ConvertBase64(string fullPath)
        {
            string base64String = string.Empty;
            if (System.IO.File.Exists(fullPath))
            {
                byte[] fileBytes = System.IO.File.ReadAllBytes(fullPath);
                // Convert the byte array to a Base64 string
                base64String = "data:image/*;base64," + Convert.ToBase64String(fileBytes);
            }
            return base64String;
            //File(Path.Combine(path, x.Field<string>("COVER_IMAGE_NAME")), x.Field<string>("COVER_IMAGE_TYPE"), x.Field<string>("COVER_IMAGE_NAME"))
        }
    }
}