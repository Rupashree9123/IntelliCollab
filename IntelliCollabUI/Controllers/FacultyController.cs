using IntelliCollabUI.Models;
using IntelliCollabUI.Utility;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

using System.Web.Mvc;

namespace IntelliCollabUI.Controllers
{
    [Authentication]
    [RoleFilter]
    public class FacultyController : Controller
    {
        // GET: Faculty
        public ActionResult FacultyInfo()
        {
            return View();
        }

        public ActionResult Faculty()
        {
            return View();
        }

        [HttpGet]
        public ActionResult GetFacultyInfo(string Id, string Branch, string Sess)
        {
            var getInfo = new DataSet();
            List<TblFaculty> result = new List<TblFaculty>();
            try
            {
                using (var conns = new SqlConnection(DbConnect.ConnectionString))
                {
                    conns.Open();
                    using (var cmd = new SqlCommand())
                    {
                        cmd.Connection = conns;
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.CommandText = "SP_Faculty_Info";
                        // Add parameters
                        if (string.IsNullOrEmpty(Id))
                        {
                            cmd.Parameters.AddWithValue("@ID", DBNull.Value);
                            cmd.Parameters.AddWithValue("@Branch", Branch);
                            cmd.Parameters.AddWithValue("@Session", Sess);
                        }
                        else
                        {
                            cmd.Parameters.AddWithValue("@ID", Id);
                            cmd.Parameters.AddWithValue("@Branch", DBNull.Value);
                            cmd.Parameters.AddWithValue("@Session", DBNull.Value);
                        }

                        using (var ad = new SqlDataAdapter(cmd))
                        {
                            ad.Fill(getInfo);
                        }
                    }
                }

                if (getInfo != null && getInfo.Tables[0].Rows.Count > 0)
                {
                    result = getInfo.Tables[0].AsEnumerable().Select(x => new TblFaculty
                    {
                        BRANCH_ID = x.Field<string>("BRANCH_ID"),
                        F_NAME = x.Field<string>("NAME"),
                        F_GMAIL = x.Field<string>("GMAIL"),
                        F_ID = x.Field<string>("ID"),
                        GENDER = x.Field<string>("GENDER"),
                        MOBILE = (long)x.Field<decimal?>("MOBILE_NO"),
                        POST = x.Field<string>("POST"),
                        SEM = x.Field<int?>("SEM"),
                        SUB_ID = x.Field<string>("SUB_ID"),
                        DEPT = x.Field<string>("DEPT"),
                        ROW_ID = x.Field<decimal>("ROW_ID"),
                        SESSION = x.Field<string>("SESSION")
                    }).ToList();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpDelete]
        public ActionResult DeleteFacultySubjectMappingInfo(List<long> FIdList)
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
                        cmd.CommandText = $@"Delete FROM Tbl_Semester_Wise_Subject_Faculty_Maping
                                            where F_ID IN ({string.Join(",", FIdList)})";

                        var result = true;//cmd.ExecuteNonQuery()>0;
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
        [HttpPut]
        public ActionResult SaveFacultySubjectMapping(List<FacultySubjectMapping> req)
        {
            /*
                @SEM numeric(18,0),
	            @BRANCH NVARCHAR(20),
	            @SUB_ID NVARCHAR(20),
	            @F_ID NVARCHAR(20),
	            @SESSION NVARCHAR(20) ,
	            @ROW_ID numeric(18,0),
	            @RESULT NVARCHAR(50) OUTPUT
             */
            string msg = "";
            try
            {
                if (req.Where(x => x.SESSION != null && x.SUB_ID != null && x.SEM != null && x.ROW_ID != null && x.F_ID != null && x.BRANCH_ID != null).Count() > 0)
                {
                    using (var conns = new SqlConnection(DbConnect.ConnectionString))
                    {
                        conns.Open();
                        using (var cmd = new SqlCommand())
                        {
                            cmd.Connection = conns;
                            cmd.CommandType = CommandType.StoredProcedure;
                            cmd.CommandText = "SP_Faculty_Info_Save";
                            foreach (var item in req)
                            {
                                // Add parameters

                                cmd.Parameters.AddWithValue("@SEM", item.SEM);
                                cmd.Parameters.AddWithValue("@BRANCH", item.BRANCH_ID);
                                cmd.Parameters.AddWithValue("@SUB_ID", item.SUB_ID);
                                cmd.Parameters.AddWithValue("@F_ID", item.F_ID);
                                cmd.Parameters.AddWithValue("@SESSION", item.SESSION);
                                cmd.Parameters.AddWithValue("@ROW_ID", item.ROW_ID);
                                cmd.Parameters.Add(new SqlParameter()
                                {
                                    ParameterName = "@RESULT",
                                    Direction = ParameterDirection.Output,
                                    Size = 50
                                });

                                cmd.ExecuteNonQuery();

                                // Retrieve the value of the output parameter
                                msg = Convert.ToString(cmd.Parameters["@RESULT"].Value);

                            }
                        }
                    }
                }
                else
                {
                    msg = "No empty field is allow";
                }

            }
            catch (Exception ex)
            {
                throw ex;
            }

            return Json(msg, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult GetFaculty(string Id, string Branch)
        {
            var getInfo = new DataSet();
            List<TblFaculty> result = new List<TblFaculty>();
            try
            {
                using (var conns = new SqlConnection(DbConnect.ConnectionString))
                {
                    conns.Open();
                    using (var cmd = new SqlCommand())
                    {
                        cmd.Connection = conns;
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.CommandText = "SP_Faculty_DISP";
                        // Add parameters
                        if (string.IsNullOrEmpty(Id))
                        {
                            cmd.Parameters.AddWithValue("@ID", DBNull.Value);
                            cmd.Parameters.AddWithValue("@Branch", Branch);
                        }
                        else
                        {
                            cmd.Parameters.AddWithValue("@ID", Id);
                            cmd.Parameters.AddWithValue("@Branch", DBNull.Value);
                        }

                        using (var ad = new SqlDataAdapter(cmd))
                        {
                            ad.Fill(getInfo);
                        }
                    }
                }

                if (getInfo != null && getInfo.Tables[0].Rows.Count > 0)
                {
                    result = getInfo.Tables[0].AsEnumerable().Select(x => new TblFaculty
                    {
                        F_ID = x.Field<string>("ID"),
                        F_NAME = x.Field<string>("NAME"),
                        FATHER_NAME = x.Field<string>("FATHER_NAME"),
                        F_GMAIL = x.Field<string>("GMAIL"),
                        POST = x.Field<string>("POST"),
                        GENDER = x.Field<string>("GENDER"),
                        MOBILE = (long?)x.Field<decimal?>("MOBILE_NO"),
                        DEPT = x.Field<string>("DEPT"),
                        ACTIVE = x.Field<string>("ACTIVE"),
                        ADDRESS = x.Field<string>("ADDRESS"),
                        DATE_OF_JOIN = x.Field<DateTime?>("DATE_OF_JOIN"),
                        PASSWORD = x.Field<string>("PASSWORD")
                    }).ToList();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpPut]
        public ActionResult SaveFaculty(List<TblFaculty> req)
        {

            string msg = "";
            try
            {
                if (req.Where(x => x.F_NAME != null && !x.F_NAME.Equals("null") && x.F_GMAIL != null && !x.F_GMAIL.Equals("null") && x.DEPT != null && !x.DEPT.Equals("-1")).Count() > 0)
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

                                cmd.CommandText = "SP_Faculty_Insert_Update";
                                // Add parameters
                                cmd.Parameters.AddWithValue("@F_ID", item.F_ID);
                                cmd.Parameters.AddWithValue("@F_NAME", item.F_NAME);
                                cmd.Parameters.AddWithValue("@FATHER_NAME", item.FATHER_NAME);
                                cmd.Parameters.AddWithValue("@POST", item.POST);
                                cmd.Parameters.AddWithValue("@DEPT", item.DEPT);
                                cmd.Parameters.AddWithValue("@MOBILE_NO", item.MOBILE);
                                cmd.Parameters.AddWithValue("@GENDER", item.GENDER);
                                cmd.Parameters.AddWithValue("@PASSWORD", item.PASSWORD);
                                cmd.Parameters.AddWithValue("@ACTIVE", item.ACTIVE);
                                cmd.Parameters.AddWithValue("@GMAIL", item.F_GMAIL);
                                cmd.Parameters.AddWithValue("@ADDRESS", item.ADDRESS);

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
                }
                else
                {
                    msg = "Faculty Name,Department and Gmail is mendatory";
                }

            }
            catch (Exception ex)
            {
                throw ex;
            }

            return Json(msg, JsonRequestBehavior.AllowGet);
        }

        [HttpDelete]
        public ActionResult DeleteFaculty(List<string> FIdList)
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
                        cmd.CommandText = $@"Delete FROM Tbl_Faculty
                                            where F_ID IN ({string.Join(",", FIdList)})";

                        var result = true;//cmd.ExecuteNonQuery()>0;
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
    }
}