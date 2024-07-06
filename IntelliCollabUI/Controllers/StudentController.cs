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
    [RoleFilter]
    public class StudentController : Controller
    {
        // GET: Student
        public ActionResult StudentInfo()
        {
            return View();
        }

        [HttpGet]
        public ActionResult GetStudent(string Id, string Branch, string Session)
        {
            var getInfo = new DataSet();
            List<TblStudent> result = new List<TblStudent>();
            try
            {
                using (var conns = new SqlConnection(DbConnect.ConnectionString))
                {
                    conns.Open();
                    using (var cmd = new SqlCommand())
                    {
                        cmd.Connection = conns;
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.CommandText = "SP_Student_Disp";

  

                        // Add parameters
                        if (string.IsNullOrEmpty(Id))
                        {
                            cmd.Parameters.AddWithValue("@ID", DBNull.Value);
                            cmd.Parameters.AddWithValue("@Branch", Branch);
                            cmd.Parameters.AddWithValue("@Session", Session);
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

                    // S.S_ID AS ID,
                    //S.S_NAME AS NAME, 
                    //S.S_GMAIL AS GMAIL,
                    //S.SESSION_DATE AS SESSION,
                    //S.S_REG_NO,
                    //S.ROLL_NO,
                    //S.GENDER,
                    //S.MOBILE_NO,
                    //S.BRANCH_ID as BRANCH,
                    //S.ACTIVE ,
                    //S.ADDRESS,
                    //S.IMAGE,
                    //S.PASSWORD,
                    //S.FATHER_NAME,
                    //S.ADDS_DATE,
                    //S.DOB

                    result = getInfo.Tables[0].AsEnumerable().Select(x => new TblStudent
                    {
                        ID = x.Field<string>("ID"),
                        NAME = x.Field<string>("NAME"),
                        FATHER_NAME = x.Field<string>("FATHER_NAME"),
                        GMAIL = x.Field<string>("GMAIL"),
                        //POST = x.Field<string>("POST"),
                        GENDER = x.Field<string>("GENDER"),
                        MOBILE_NO = (long?)x.Field<decimal?>("MOBILE_NO"),
                        BRANCH = x.Field<string>("BRANCH"),
                        ACTIVE = x.Field<string>("ACTIVE"),
                        ADDRESS = x.Field<string>("ADDRESS"),
                        DOB = x.Field<DateTime?>("DOB"),
                        PASSWORD = x.Field<string>("PASSWORD"),
                        ADDS_DATE = x.Field<DateTime?>("ADDS_DATE"),
                        ROLL_NO = x.Field<string>("ROLL_NO"),
                        S_REG_NO = x.Field<string>("S_REG_NO"),
                        SESSION = x.Field<string>("SESSION"),
                        //IMAGE = x.Field<byte[]>("IMAGE")
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
        public ActionResult SaveStudent(List<TblStudent> req)
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
                if (req.Where(x => x.SESSION != null && x.GMAIL != null && x.NAME != null && x.PASSWORD != null && x.ACTIVE != null && x.BRANCH != null).Count() > 0)
                {
                    using (var conns = new SqlConnection(DbConnect.ConnectionString))
                    {
                        conns.Open();
                        using (var cmd = new SqlCommand())
                        {
                            cmd.Connection = conns;
                            cmd.CommandType = CommandType.StoredProcedure;
                            cmd.CommandText = "SP_Student_Insert_Update";
                            foreach (var item in req)
                            {
                                // Add parameters

                                /*
                                 
                                 [dbo].[SP_Student_Insert_Update]

                                @S_ID NVARCHAR(20) = NULL,
	@REG_NO NVARCHAR(20) = NULL,
	@ROLL_NO NVARCHAR(20) = NULL,
	@S_NAME NVARCHAR(20),
	@FATHER_NAME NVARCHAR(20) = NULL,
	@ADDS_DATE DATE = NULL,
	@DOB DATE = NULL,
	@SESSION NVARCHAR(20) ,
	@BRANCH NVARCHAR(20),
	@MOBILE_NO numeric(18,0) = NULL ,
	@GENDER NVARCHAR(1) = NULL,
	@PASSWORD NVARCHAR(20),
	@ACTIVE NVARCHAR(1),
	@GMAIL NVARCHAR(50),
	@ADDRESS NVARCHAR(100) = NULL,
	@RESULT NVARCHAR(50) OUTPUT
                                 */



                                cmd.Parameters.AddWithValue("@S_ID", item.ID);
                                cmd.Parameters.AddWithValue("@REG_NO", item.S_REG_NO);
                                cmd.Parameters.AddWithValue("@ROLL_NO", item.ROLL_NO);
                                cmd.Parameters.AddWithValue("@S_NAME", item.NAME);
                                cmd.Parameters.AddWithValue("@FATHER_NAME", item.FATHER_NAME);
                                cmd.Parameters.AddWithValue("@ADDS_DATE", item.ADDS_DATE);
                                cmd.Parameters.AddWithValue("@DOB", item.DOB);
                                cmd.Parameters.AddWithValue("@SESSION", item.SESSION);
                                cmd.Parameters.AddWithValue("@BRANCH", item.BRANCH);
                                cmd.Parameters.AddWithValue("@MOBILE_NO", item.MOBILE_NO);
                                cmd.Parameters.AddWithValue("@GENDER", item.GENDER);
                                cmd.Parameters.AddWithValue("@PASSWORD", item.PASSWORD);
                                cmd.Parameters.AddWithValue("@ACTIVE", item.ACTIVE);
                                cmd.Parameters.AddWithValue("@GMAIL", item.GMAIL);
                                cmd.Parameters.AddWithValue("@ADDRESS", item.ADDRESS);
                                
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
                    msg = "Name, Gmail,Password,branch,active empty field is allow";
                }

            }
            catch (Exception ex)
            {
                throw ex;
            }

            return Json(msg, JsonRequestBehavior.AllowGet);
        }

        //[HttpDelete]
        //public ActionResult DeleteStudent()
        //{

        //}
    }
}