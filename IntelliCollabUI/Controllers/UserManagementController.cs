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
    public class UserManagementController : Controller
    {
        // GET: UserManagement
        public ActionResult Attendance()
        {
            return View();
        }

        public ActionResult Results()
        {
            return View();
        }

        public ActionResult CalendarView()
        {
            return View();
        }

        public ActionResult PersonalInfo()
        {
            return View();
        }

        #region Result 
        public ActionResult ViewResult(string Session,string Branch,string ExamType,decimal? Sem)
        {
            DataSet getInfo = new DataSet();
            List<Result> result = new List<Result>();
            using (var conns = new SqlConnection(DbConnect.ConnectionString))
            {
                conns.Open();
                using (var cmd = new SqlCommand())
                {
                    cmd.Connection = conns;
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandText = "SP_Result_Disp";

                    cmd.Parameters.AddWithValue("@Sem", Sem);
                    cmd.Parameters.AddWithValue("@Session", Session);
                    cmd.Parameters.AddWithValue("@Branch", Branch);
                    cmd.Parameters.AddWithValue("@ExamType", ExamType);

                    using (var ad = new SqlDataAdapter(cmd))
                    {
                        ad.Fill(getInfo);
                    }
                }
            }

            if (getInfo != null && getInfo.Tables[0].Rows.Count > 0)
            {
                result = getInfo.Tables[0].AsEnumerable().Select(x => new Result
                {
                    RESULT_ID = x.Field<string>("RESULT_ID"),
                    FILE_NAMES = x.Field<string>("FILE_NAMES"),
                    FILE_TYPE = x.Field<string>("FILE_TYPE"),
                    BRANCH_ID = x.Field<string>("BRANCH_ID"),
                    UPLOAD_DATE = x.Field<DateTime>("UPLOAD_DATE"),
                    SEM = x.Field<int>("SEM"),
                    UPLOAD_BY = x.Field<string>("UPLOAD_BY"),
                    TITLE = x.Field<string>("TITLE"),
                    DESCRIPTION = x.Field<string>("DESCRIPTION"),
                    SESSION_DATE = x.Field<string>("SESSION_DATE")
                }).ToList();
            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpDelete]
        public ActionResult DeleteResult(AsstDelete req)
        {
            string msg = string.Empty;
            using (var conns = new SqlConnection(DbConnect.ConnectionString))
            {
                conns.Open();
                using (var cmd = new SqlCommand())
                {
                    cmd.Connection = conns;
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.CommandText = "SP_Result_Delete";

                    // Add parameters

                    cmd.Parameters.AddWithValue("@RESULT_ID", req.ASST_ID);
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
            string path = Server.MapPath(@"~/App_Data/Result");
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

        [HttpPost]
        public ActionResult UploadResult(LibraryBookUpload req)
        {
            string msg = "";
            string fullPath = string.Empty;
            try
            {

                string fileName = string.Empty;
                string fileContentType = string.Empty;
                

                string UserId = Convert.ToString(Session["UserID"]);

                Result resultUpl = new Result();
                string path = Server.MapPath(@"~/App_Data/Result");


                var saveObjectJson = req.SaveObject;
                resultUpl = JsonConvert.DeserializeObject<Result>(saveObjectJson);
                resultUpl.UPLOAD_BY = !string.IsNullOrEmpty(UserId)? UserId: null;

                if (resultUpl.BRANCH_ID != null && resultUpl.SEM != null && resultUpl.SESSION_DATE !=null && resultUpl.UPLOAD_BY !=null)
                {
                    //uploading File
                    var uploadFile = req.UploadedFile;
                    if (uploadFile != null && uploadFile.ContentLength > 0)
                    {
                        fileName = Path.GetFileName(uploadFile.FileName);
                        fullPath = Path.Combine(path, fileName);
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

                            cmd.CommandText = "SP_Result_Insert_Update";
                            // Add parameters
                            cmd.Parameters.AddWithValue("@BRANCH", resultUpl.BRANCH_ID);
                            cmd.Parameters.AddWithValue("@UPLOADED_BY", UserId);
                            cmd.Parameters.AddWithValue("@FILE_NAME", fileName);
                            cmd.Parameters.AddWithValue("@FILE_TYPE", fileContentType);
                            cmd.Parameters.AddWithValue("@TITLE", resultUpl.TITLE);
                            cmd.Parameters.AddWithValue("@DESCP", resultUpl.DESCRIPTION);
                            cmd.Parameters.AddWithValue("@RESULT_ID", resultUpl.RESULT_ID);
                            cmd.Parameters.AddWithValue("@SESSION", resultUpl.SESSION_DATE);
                            cmd.Parameters.AddWithValue("@SEM", resultUpl.SEM);
                            cmd.Parameters.AddWithValue("@EXAM_TYPE", resultUpl.EXAM_TYPE);

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
                if (System.IO.File.Exists(fullPath))
                {
                    System.IO.File.Delete(fullPath);
                }
                throw ex;
            }
            return Json(msg, JsonRequestBehavior.AllowGet);
        }
        
        
        //DownloadResult
        [HttpGet]
        public ActionResult DownloadResult(string Filename, string contentType)
        {
            string base64String = string.Empty;
            try
            {
                string path = Server.MapPath(@"~/App_Data/Result");
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

        #region Event/Notice 
        [HttpGet]
        public ActionResult getEventTypeList()
        {
            DataSet getInfo = new DataSet();
            List<EventType> result = new List<EventType>();
            using (var conns = new SqlConnection(DbConnect.ConnectionString))
            {
                conns.Open();
                using (var cmd = new SqlCommand())
                {
                    cmd.Connection = conns;
                    cmd.CommandType = CommandType.Text;
                    cmd.CommandText = "SELECT * FROM Tbl_EventType";

                    using (var ad = new SqlDataAdapter(cmd))
                    {
                        ad.Fill(getInfo);
                    }
                }
            }

            if (getInfo!=null && getInfo.Tables[0].Rows.Count>0)
            {
                result = getInfo.Tables[0].AsEnumerable().Select(x=> new EventType { 
                    EVENT_ID = x.Field<string>("EVENT_ID"),
                    EVENT_NAME = x.Field<string>("EVENT_NAME"),
                }).ToList();
            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        [HttpGet]
        public ActionResult getEventOrNoticeList(DateTime startDate)
        {
            DataSet getInfo = new DataSet();
            List<Event> result = new List<Event>();

            using (var conns = new SqlConnection(DbConnect.ConnectionString))
            {
                conns.Open();
                using (var cmd = new SqlCommand())
                {
                    cmd.Connection = conns;
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.CommandText = "SP_Event_Disp";

                    // Add parameters

                    cmd.Parameters.AddWithValue("@Date", startDate);
                   
                    using (var ad = new SqlDataAdapter(cmd))
                    {
                        ad.Fill(getInfo);
                    }
                }
            }
            if (getInfo != null && getInfo.Tables[0].Rows.Count > 0)
            {
                result = getInfo.Tables[0].AsEnumerable().Select(x => new Event
                {
                    EVENT_ID = x.Field<string>("EVENT_ID"),
                    EVENT_NAME = x.Field<string>("EVENT_NAME"),
                    EVENT_DESC = x.Field<string>("EVENT_DESC"),
                    EVENT_TYPE = x.Field<string>("EVENT_TYPE_ID"),
                    EVENT_TYPE_NAME = x.Field<string>("EVENT_TYPE_NAME"),
                    START_DATE = x.Field<DateTime>("START_DATE"),
                    END_DATE = x.Field<DateTime>("END_DATE"),
                    STR_START_DATE = (x.Field<DateTime>("START_DATE")).ToString("dd/MM/yyyy"),
                    STR_END_DATE = (x.Field<DateTime>("END_DATE")).ToString("dd/MM/yyyy"),
                    NO_DAYS = NoOdDays(x.Field<DateTime>("END_DATE"), x.Field<DateTime>("START_DATE"))
                }).ToList();
            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpPut]
        public ActionResult saveEvent(List<Event> req)
        {
            string msg = string.Empty;

            using (var conns = new SqlConnection(DbConnect.ConnectionString))
            {
                conns.Open();
                foreach(var item in req)
                {
                    using (var cmd = new SqlCommand())
                    {
                        cmd.Connection = conns;
                        cmd.CommandType = CommandType.StoredProcedure;

                        cmd.CommandText = "SP_Event_Insert_Update";

                        // Add parameters
                        
                        cmd.Parameters.AddWithValue("@StartDate", item.START_DATE);
                        cmd.Parameters.AddWithValue("@EndDate", item.END_DATE);
                        cmd.Parameters.AddWithValue("@EventName", item.EVENT_NAME);
                        cmd.Parameters.AddWithValue("@EventDesc", item.EVENT_DESC);
                        cmd.Parameters.AddWithValue("@EventType", item.EVENT_TYPE);
                        cmd.Parameters.AddWithValue("@EventId", item.EVENT_ID);
                        cmd.Parameters.Add(new SqlParameter()
                        {
                            ParameterName = "@RESULT",
                            Direction = ParameterDirection.Output,
                            Size = 50
                        });

                        cmd.ExecuteNonQuery();

                        // Retrieve the value of the output parameter
                        msg = Convert.ToString(cmd.Parameters["@RESULT"].Value);

                        var bodyMsg = EmailSystem.EventMsgDesign(item.EVENT_NAME, item.EVENT_DESC,item.START_DATE,item.END_DATE);
                        var emailIds = EmailSystem.GetEmailIds();
                        EmailSystem obj = new EmailSystem(emailIds,"IntelliColab Notices" ,bodyMsg);
                        if (obj.SendMail())
                        {
                            msg += " and Notification has been sent to the email addresses.";
                        }
                        else
                        {
                            msg += " and The notification failed to be sent to the email addresses.";
                        }
                    }
                }
               
            }
            
            return Json(msg, JsonRequestBehavior.AllowGet);
        }

        [HttpDelete]
        public ActionResult deleteEvent(List<string> eId)
        {
            string msg = string.Empty;
            try
            {
                using (var conns = new SqlConnection(DbConnect.ConnectionString))
                {
                    conns.Open();
                    using (var cmd = new SqlCommand())
                    {
                        cmd.Connection = conns;
                        cmd.CommandType = CommandType.Text;
                        cmd.CommandText = $@"Delete FROM Tbl_Event
                                            where EVENT_ID IN ({string.Join(",", eId)})";

                        var result = cmd.ExecuteNonQuery()>0;
                        if (result)
                        {
                            msg = "Records deleted successfully";
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return Json(msg, JsonRequestBehavior.AllowGet);
        }
        #endregion


        #region Personal info


        [HttpPost]
        public ActionResult SavePersonalInfo(FormDataModel req)
        {
            string msg = string.Empty;
            UserProfile saveUserProfile = new UserProfile();
            try
            {
                var saveObjectJson = req.SaveObject;
                saveUserProfile = JsonConvert.DeserializeObject<UserProfile>(saveObjectJson);


                var uploadedImage = req.UploadedImage;
                byte[] imageData = null;
                if (uploadedImage != null && uploadedImage.ContentLength > 0)
                {
                    using (var binaryReader = new BinaryReader(uploadedImage.InputStream))
                    {
                        imageData = binaryReader.ReadBytes(uploadedImage.ContentLength);
                    }
                    //converting byte[] to byte?[]
                    saveUserProfile.Image = imageData.Select(x => (byte?)x).ToArray();
                }

               
                /*
                    @ID NVARCHAR(100),
                    @REG_NO NVARCHAR(15),
                    @NAME NVARCHAR(20),
                    @FATHER_NAME NVARCHAR(20) = NULL,
                    @MOBILE_NO NUMERIC(12,0) = NULL,
                    @GENDER NVARCHAR(1) = NULL,
                    @BRANCH_ID NVARCHAR(10) = NULL,
                    @SESSION NVARCHAR(10) = NULL,
                    @DOB DATE = NULL,
                    @EMAIL NVARCHAR(50),
                    @ADDRESS NVARCHAR(100) = NULL,
                    @POST NVARCHAR(10) = NULL,
                    @IMAGE VARBINARY(MAX) = NULL,
                    @RESULT NVARCHAR(50) OUTPUT
                 */
                using (var conns = new SqlConnection(DbConnect.ConnectionString))
                {
                    conns.Open();
                    using (var cmd = new SqlCommand())
                    {
                        cmd.Connection = conns;
                        cmd.CommandType = CommandType.StoredProcedure;

                        cmd.CommandText = "SP_Personal_Info_Save";
                        // Add parameters
                        cmd.Parameters.AddWithValue("@ID", saveUserProfile.UserId);
                        cmd.Parameters.AddWithValue("@REG_NO", saveUserProfile.RegistrationNo);
                        cmd.Parameters.AddWithValue("@NAME", saveUserProfile.UserName);
                        cmd.Parameters.AddWithValue("@FATHER_NAME", saveUserProfile.FatherName);
                        cmd.Parameters.AddWithValue("@MOBILE_NO", saveUserProfile.MobileNo);
                        cmd.Parameters.AddWithValue("@GENDER", saveUserProfile.Gender);
                        cmd.Parameters.AddWithValue("@BRANCH_ID", saveUserProfile.Branch);
                        cmd.Parameters.AddWithValue("@SESSION", saveUserProfile.Session);
                        cmd.Parameters.AddWithValue("@DOB", saveUserProfile.DateOfBirth);
                        cmd.Parameters.AddWithValue("@EMAIL", saveUserProfile.Email);
                        cmd.Parameters.AddWithValue("@ADDRESS", saveUserProfile.Address);
                        cmd.Parameters.AddWithValue("@POST", saveUserProfile.Post);

                        //if(saveUserProfile.Image != null)
                        //{
                        //    // Convert the byte[] image data to SqlParameter of type VarBinary
                        //    SqlParameter imageParam = new SqlParameter("@IMAGE", SqlDbType.VarBinary, saveUserProfile.Image.Length);
                        //    imageParam.Value = saveUserProfile.Image.Select(b => b ?? 0).ToArray();//converting btye?[] to byte[]
                        //    cmd.Parameters.Add(imageParam);
                        //}
                        //else
                        //{
                        //    cmd.Parameters.AddWithValue("@IMAGE", SqlDbType.VarBinary).Value = null;
                        //}

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
                    //image saving 
                    if(saveUserProfile.Image!=null)
                    {
                        using (var cmd = new SqlCommand())
                        {
                            cmd.Connection = conns;
                            cmd.CommandType = CommandType.StoredProcedure;

                            cmd.CommandText = "SP_Personal_Save_Image";
                            // Add parameters
                            cmd.Parameters.AddWithValue("@ID", saveUserProfile.UserId);

                            if (saveUserProfile.Image != null)
                            {
                                // Convert the byte[] image data to SqlParameter of type VarBinary
                                SqlParameter imageParam = new SqlParameter("@IMAGE", SqlDbType.VarBinary, saveUserProfile.Image.Length);
                                imageParam.Value = saveUserProfile.Image.Select(b => b ?? 0).ToArray();//converting btye?[] to byte[]
                                cmd.Parameters.Add(imageParam);
                            }

                            cmd.Parameters.Add(new SqlParameter()
                            {
                                ParameterName = "@RESULT",
                                Direction = ParameterDirection.Output,
                                Size = 50
                            });

                            cmd.ExecuteNonQuery();

                            //Retrieve the value of the output parameter
                            msg += "And "+ Convert.ToString(cmd.Parameters["@RESULT"].Value);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            

            return Json(msg, JsonRequestBehavior.AllowGet);
        }
        [HttpGet]
        public ActionResult GetPersonalInfo()
        {
            UserProfile UserDetail = new UserProfile();
            string userId = Convert.ToString(Session["UserID"]);
            DataSet result = new DataSet();
            using (var conns = new SqlConnection(DbConnect.ConnectionString))
            {
                conns.Open();
                using (var cmd = new SqlCommand())
                {
                    cmd.Connection = conns;
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandText = "SP_Personal_Info_Disp";
                    // Add parameters

                    cmd.Parameters.AddWithValue("@ID", userId);


                    using (var ad = new SqlDataAdapter(cmd))
                    {
                        ad.Fill(result);
                    }
                }
            }
            if (result != null && result.Tables.Count > 0 && result.Tables[0].Rows.Count > 0)
            {

                UserDetail = result.Tables[0].AsEnumerable().Select(x => new UserProfile
                {
                    DateOfBirth = x.Field<DateTime?>("DOB"),
                    UserName = x.Field<string>("NAME"),
                    UserId = x.Field<string>("ID"),
                    RegistrationNo = x.Field<string>("REG_NO"),
                    Branch = x.Field<string>("BRANCH_ID"),
                    FatherName = x.Field<string>("FATHER_NAME"),
                    Session = x.Field<string>("SESSION_DATE"),
                    Gender = x.Field<string>("GENDER"),
                    MobileNo = (long?)x.Field<decimal?>("MOBILE_NO"),
                    Address = x.Field<string>("ADDRESS"),
                    UserType = x.Field<string>("USER_TYPE"),
                    Email = x.Field<string>("EMAIL"),
                    Image = x.Field<object>("IMAGE") != DBNull.Value ? x.Field<byte[]>("IMAGE")?.Select(y=> (byte?)y).ToArray() : null,
                    Base64Image = ConvertToBase64(x.Field<object>("IMAGE") != DBNull.Value ? x.Field<byte[]>("IMAGE")?.Select(y => (byte?)y).ToArray() : null)
                }).FirstOrDefault();

            }

            return Json(UserDetail, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult UploadProfileImage(FormDataModel req)
        {
            string msg = string.Empty;
            UserProfile saveUserProfile = new UserProfile();
            try
            {
                var saveObjectJson = req.SaveObject;
                saveUserProfile = JsonConvert.DeserializeObject<UserProfile>(saveObjectJson);

                var uploadedImage = req.UploadedImage;
                byte[] imageData = null;
                if (uploadedImage != null && uploadedImage.ContentLength > 0)
                {
                    using (var binaryReader = new BinaryReader(uploadedImage.InputStream))
                    {
                        imageData = binaryReader.ReadBytes(uploadedImage.ContentLength);
                    }
                    //converting byte[] to byte?[]
                    saveUserProfile.Image = imageData.Select(x => (byte?)x).ToArray();
                }

                using (var conns = new SqlConnection(DbConnect.ConnectionString))
                {
                    conns.Open();
                    //image saving 
                    if (saveUserProfile.Image != null)
                    {
                        using (var cmd = new SqlCommand())
                        {
                            cmd.Connection = conns;
                            cmd.CommandType = CommandType.StoredProcedure;

                            cmd.CommandText = "SP_Personal_Save_Image";
                            // Add parameters
                            cmd.Parameters.AddWithValue("@ID", saveUserProfile.UserId);

                            if (saveUserProfile.Image != null)
                            {
                                // Convert the byte[] image data to SqlParameter of type VarBinary
                                SqlParameter imageParam = new SqlParameter("@IMAGE", SqlDbType.VarBinary, saveUserProfile.Image.Length);
                                imageParam.Value = saveUserProfile.Image.Select(b => b ?? 0).ToArray();//converting btye?[] to byte[]
                                cmd.Parameters.Add(imageParam);
                            }
                            else
                            {
                                cmd.Parameters.AddWithValue("@IMAGE", SqlDbType.VarBinary).Value = null;
                            }

                            cmd.Parameters.Add(new SqlParameter()
                            {
                                ParameterName = "@RESULT",
                                Direction = ParameterDirection.Output,
                                Size = 50
                            });

                            cmd.ExecuteNonQuery();

                            //Retrieve the value of the output parameter
                            msg =  Convert.ToString(cmd.Parameters["@RESULT"].Value);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }


            return Json(msg, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region Attendance
        [HttpGet]
        public ActionResult GetAttendanceInfo(string Year, string Month, decimal Sem, string Branch, string Sessions)
        {
            List<Tbl_Attendance> result = new List<Tbl_Attendance>();
            List<TblStudent> stdRecords = new List<TblStudent>();
            DataSet getRecords = new DataSet();
            DataSet stdDetails = new DataSet();
            try
            {

                using (var conns = new SqlConnection(DbConnect.ConnectionString))
                {
                    conns.Open();
                    using (var cmd = new SqlCommand())
                    {
                        cmd.Connection = conns;
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.CommandText = "SP_Attendance_Disp";
                        // Add parameters

                        cmd.Parameters.AddWithValue("@Year", Year);
                        cmd.Parameters.AddWithValue("@Month", Month);
                        cmd.Parameters.AddWithValue("@Sem", Sem);
                        cmd.Parameters.AddWithValue("@Branch", Branch);
                        cmd.Parameters.AddWithValue("@Session", Sessions);


                        using (var ad = new SqlDataAdapter(cmd))
                        {
                            ad.Fill(getRecords);
                        }
                    }
                }

                using (var conns = new SqlConnection(DbConnect.ConnectionString))
                {
                    conns.Open();
                    using (var cmd = new SqlCommand())
                    {
                        cmd.Connection = conns;
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.CommandText = "SP_Student_Disp";



                        // Add parameters

                        cmd.Parameters.AddWithValue("@ID", DBNull.Value);
                        cmd.Parameters.AddWithValue("@Branch", Branch);
                        cmd.Parameters.AddWithValue("@Session", Sessions);



                        using (var ad = new SqlDataAdapter(cmd))
                        {
                            ad.Fill(stdDetails);
                        }
                    }
                }

                if (getRecords != null && getRecords.Tables.Count > 0 && getRecords.Tables[0].Rows.Count > 0)
                {
                    result = getRecords.Tables[0].AsEnumerable().Select(x => new Tbl_Attendance
                    {
                        S_ID = x.Field<string>("S_ID"),
                        S_NAME = x.Field<string>("S_NAME"),
                        YEARS = x.Field<string>("YEARS"),
                        SESSION = x.Field<string>("SESSION"),
                        BRANCH = x.Field<string>("BRANCH"),
                        SEM = x.Field<decimal>("SEM"),
                        DATA_MOMENT = x.Field<DateTime>("DATA_MOMENT"),
                        MONTHS = x.Field<string>("MONTHS"),
                        DAY_1 = x.Field<string>("DAY_1"),
                        DAY_2 = x.Field<string>("DAY_2"),
                        DAY_3 = x.Field<string>("DAY_3"),
                        DAY_4 = x.Field<string>("DAY_4"),
                        DAY_5 = x.Field<string>("DAY_5"),
                        DAY_6 = x.Field<string>("DAY_6"),
                        DAY_7 = x.Field<string>("DAY_7"),
                        DAY_8 = x.Field<string>("DAY_8"),
                        DAY_9 = x.Field<string>("DAY_9"),
                        DAY_10 = x.Field<string>("DAY_10"),
                        DAY_11 = x.Field<string>("DAY_11"),
                        DAY_12 = x.Field<string>("DAY_12"),
                        DAY_13 = x.Field<string>("DAY_13"),
                        DAY_14 = x.Field<string>("DAY_14"),
                        DAY_15 = x.Field<string>("DAY_15"),
                        DAY_16 = x.Field<string>("DAY_16"),
                        DAY_17 = x.Field<string>("DAY_17"),
                        DAY_18 = x.Field<string>("DAY_18"),
                        DAY_19 = x.Field<string>("DAY_19"),
                        DAY_20 = x.Field<string>("DAY_20"),
                        DAY_21 = x.Field<string>("DAY_21"),
                        DAY_22 = x.Field<string>("DAY_22"),
                        DAY_23 = x.Field<string>("DAY_23"),
                        DAY_24 = x.Field<string>("DAY_24"),
                        DAY_25 = x.Field<string>("DAY_25"),
                        DAY_26 = x.Field<string>("DAY_26"),
                        DAY_27 = x.Field<string>("DAY_27"),
                        DAY_28 = x.Field<string>("DAY_28"),
                        DAY_29 = x.Field<string>("DAY_29"),
                        DAY_30 = x.Field<string>("DAY_30"),
                        DAY_31 = x.Field<string>("DAY_31")
                    }).ToList();
                }

                if (stdDetails != null && stdDetails.Tables[0].Rows.Count > 0)
                {
                    stdRecords = stdDetails.Tables[0].AsEnumerable().Select(x => new TblStudent
                    {
                        ID = x.Field<string>("ID"),
                        NAME = x.Field<string>("NAME"),
                        BRANCH = x.Field<string>("BRANCH"),
                        SESSION = x.Field<string>("SESSION"),
                    }).ToList();
                }

                //combining both models
                foreach (var item in stdRecords)
                {
                    if (result.Where(x => x.S_ID == item.ID).Count() == 0)
                    {
                        result.Add(new Tbl_Attendance
                        {
                            S_ID = item.ID,
                            S_NAME = item.NAME,
                            BRANCH = item.BRANCH,
                            SESSION = item.SESSION,
                            SEM = Sem,
                            YEARS = Year,
                            MONTHS = Month
                        });
                    }
                }

                return Json(result, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                stdRecords.Clear();
                getRecords.Clear();
                stdDetails.Clear();
            }
        }

        [HttpPut]
        public ActionResult SaveAttendance(List<Tbl_Attendance> req)
        {

            string msg = "";
            try
            {
                if (req.Where(x => x.S_ID != null && x.YEARS != null && x.MONTHS != null && x.BRANCH != null && x.SEM > 0 && x.SESSION != null).Count() > 0)
                {
                    using (var conns = new SqlConnection(DbConnect.ConnectionString))
                    {
                        conns.Open();

                        foreach (var item in req)
                        {
                            using (var cmd = new SqlCommand())
                            {
                                cmd.Connection = conns;
                                cmd.CommandType = CommandType.StoredProcedure;

                                cmd.CommandText = "SP_Attendance_Insert_Update";

                                // Add parameters
                                cmd.Parameters.AddWithValue("@Year", item.YEARS);
                                cmd.Parameters.AddWithValue("@Month", item.MONTHS);
                                cmd.Parameters.AddWithValue("@Sem", item.SEM);
                                cmd.Parameters.AddWithValue("@Branch", item.BRANCH);
                                cmd.Parameters.AddWithValue("@Session", item.SESSION);
                                cmd.Parameters.AddWithValue("@S_ID", item.S_ID);
                                cmd.Parameters.AddWithValue("@DAY_1", item.DAY_1);
                                cmd.Parameters.AddWithValue("@DAY_2", item.DAY_2);
                                cmd.Parameters.AddWithValue("@DAY_3", item.DAY_3);
                                cmd.Parameters.AddWithValue("@DAY_4", item.DAY_4);
                                cmd.Parameters.AddWithValue("@DAY_5", item.DAY_5);
                                cmd.Parameters.AddWithValue("@DAY_6", item.DAY_6);
                                cmd.Parameters.AddWithValue("@DAY_7", item.DAY_7);
                                cmd.Parameters.AddWithValue("@DAY_8", item.DAY_8);
                                cmd.Parameters.AddWithValue("@DAY_9", item.DAY_9);
                                cmd.Parameters.AddWithValue("@DAY_10", item.DAY_10);
                                cmd.Parameters.AddWithValue("@DAY_11", item.DAY_11);
                                cmd.Parameters.AddWithValue("@DAY_12", item.DAY_12);
                                cmd.Parameters.AddWithValue("@DAY_13", item.DAY_13);
                                cmd.Parameters.AddWithValue("@DAY_14", item.DAY_14);
                                cmd.Parameters.AddWithValue("@DAY_15", item.DAY_15);
                                cmd.Parameters.AddWithValue("@DAY_16", item.DAY_16);
                                cmd.Parameters.AddWithValue("@DAY_17", item.DAY_17);
                                cmd.Parameters.AddWithValue("@DAY_18", item.DAY_18);
                                cmd.Parameters.AddWithValue("@DAY_19", item.DAY_19);
                                cmd.Parameters.AddWithValue("@DAY_20", item.DAY_20);
                                cmd.Parameters.AddWithValue("@DAY_21", item.DAY_21);
                                cmd.Parameters.AddWithValue("@DAY_22", item.DAY_22);
                                cmd.Parameters.AddWithValue("@DAY_23", item.DAY_23);
                                cmd.Parameters.AddWithValue("@DAY_24", item.DAY_24);
                                cmd.Parameters.AddWithValue("@DAY_25", item.DAY_25);
                                cmd.Parameters.AddWithValue("@DAY_26", item.DAY_26);
                                cmd.Parameters.AddWithValue("@DAY_27", item.DAY_27);
                                cmd.Parameters.AddWithValue("@DAY_28", item.DAY_28);
                                cmd.Parameters.AddWithValue("@DAY_29", item.DAY_29);
                                cmd.Parameters.AddWithValue("@DAY_30", item.DAY_30);
                                cmd.Parameters.AddWithValue("@DAY_31", item.DAY_31);

                                cmd.Parameters.Add(new SqlParameter()
                                {
                                    ParameterName = "@RESULT",
                                    Direction = ParameterDirection.Output,
                                    Size = 50
                                });

                                cmd.ExecuteNonQuery();

                                //Retrieve the value of the output parameter
                                msg += Convert.ToString(cmd.Parameters["@RESULT"].Value) + " /n ";

                            }
                        }

                    }
                }
                else
                {
                    msg = "Sorry!. Save operation can't be perform.";
                }

            }
            catch (Exception ex)
            {
                throw ex;
            }

            return Json(msg, JsonRequestBehavior.AllowGet);
        }
        #endregion

        public static string ConvertToBase64(byte?[] byteArray)
        {
            if (byteArray == null)
                return null;

            // Convert nullable byte array to byte array
            byte[] bytes = byteArray.Select(b => b ?? 0).ToArray();

            // Convert byte array to base64 string
            string base64String = "data:image/*;base64," + Convert.ToBase64String(bytes);
            return base64String;
        }

        private int NoOdDays(DateTime ed,DateTime st)
        {
            TimeSpan diff = ed - st;
            return (int)diff.TotalDays;
        }
    }
}



                








