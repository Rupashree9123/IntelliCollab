using IntelliCollabUI.Models;
using IntelliCollabUI.Utility;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
namespace IntelliCollabUI.Controllers
{
    
    public class DashBoardController : Controller
    {
        // GET: DashBoard
        [Authentication]
        public ActionResult Index()
        {
            //if (Session["UserID"] != null)
            //{
            //    return View();
            //}
            //else
            //{
            //    return RedirectToAction("LogIn", "LogIn");
            //} 
            return View();
        }

        [HttpGet]
        public ActionResult GetAllDropdownList(string Id, string Branch)
        {
            dynamic obj;
            var subject = new DataSet();
            var department = new DataSet();
            var student = new DataSet();
            var faculty = new DataSet();
            List<Dropdown> deptList = new List<Dropdown>();
            List<Dropdown> subjectList = new List<Dropdown>();
            List<Dropdown> studentList = new List<Dropdown>();
            List<Dropdown> facultyList = new List<Dropdown>();
            try
            {
                
                using (var conns = new SqlConnection(DbConnect.ConnectionString))
                {
                    conns.Open();
                    using (var cmd = new SqlCommand())
                    {
                        cmd.Connection = conns;
                        cmd.CommandType = CommandType.Text;
                        cmd.CommandText = "SP_Department_Ddl";
                        
                        using (var ad = new SqlDataAdapter(cmd))
                        {
                            ad.Fill(department);
                        }
                    }

                    using (var cmd = new SqlCommand())
                    {
                        cmd.Connection = conns;
                        cmd.CommandType = CommandType.Text;
                        cmd.CommandText = "SP_Subject_Ddl";

                        using (var ad = new SqlDataAdapter(cmd))
                        {
                             ad.Fill(subject); 
                        }
                    }

                    using (var cmd = new SqlCommand())
                    {
                        cmd.Connection = conns;
                        cmd.CommandType = CommandType.Text;
                        cmd.CommandText = "SP_Faculty_Ddl";

                        using (var ad = new SqlDataAdapter(cmd))
                        {
                            ad.Fill(faculty);
                        }
                    }

                    using (var cmd = new SqlCommand())
                    {
                        cmd.Connection = conns;
                        cmd.CommandType = CommandType.Text;
                        cmd.CommandText = "SP_Student_Ddl";

                        using (var ad = new SqlDataAdapter(cmd))
                        {
                            ad.Fill(student);
                        }
                    } 
                }

                if (department != null && department.Tables[0].Rows.Count > 0)
                {
                    deptList = department.Tables[0].AsEnumerable().Select(x => new Dropdown
                    {
                        TEXT = x.Field<string>("Dept_Name"),
                        VALUE = x.Field<string>("Dept_ID")
                    }).ToList();
                }

                if (subject != null && subject.Tables[0].Rows.Count > 0)
                {
                    subjectList = subject.Tables[0].AsEnumerable().Select(x => new Dropdown
                    {
                        TEXT = x.Field<string>("S_Name"),
                        VALUE = x.Field<string>("S_ID")
                    }).ToList();
                }
                
                if (faculty != null && faculty.Tables[0].Rows.Count > 0)
                {
                    facultyList = faculty.Tables[0].AsEnumerable().Select(x => new Dropdown
                    {
                        TEXT = x.Field<string>("F_Name"),
                        VALUE = x.Field<string>("F_ID")
                    }).ToList();
                }
                
                if (student != null && student.Tables[0].Rows.Count > 0)
                {
                    studentList = student.Tables[0].AsEnumerable().Select(x => new Dropdown
                    {
                        TEXT = x.Field<string>("S_Name"),
                        VALUE = x.Field<string>("S_ID")
                    }).ToList();
                }

                obj = new
                {
                    DeptList = deptList,
                    SubjectList = subjectList,
                    FacultyList = facultyList,
                    StudentList = studentList
                };
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return Json(obj, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult GetDashBoardResult()
        {
            DataSet ds = new DataSet();
            dynamic resp = new { };
            try
            {
                string sql = string.Empty;

                using (var conn = new SqlConnection(DbConnect.ConnectionString))
                {
                    sql = @"select * from (SELECT COUNT(*) AS NoOFStd from Tbl_Student) as A,
                                (SELECT COUNT(*) AS NoOFFaculty from Tbl_Faculty) as B,
                                (SELECT COUNT(*) AS NoOFBranch from Tbl_Department) as C";

                    conn.Open();
                    using (var cmd = new SqlCommand(sql,conn))
                    {
                        using (var da = new SqlDataAdapter(cmd))
                        {
                            da.Fill(ds);
                        }
                    }
                }
                if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                {
                    resp = new
                    {
                        NoOfStudent = ds.Tables[0].AsEnumerable().Select(x => x.Field<int>("NoOFStd")).FirstOrDefault(),
                        NoOfFaculty = ds.Tables[0].AsEnumerable().Select(x => x.Field<int>("NoOFFaculty")).FirstOrDefault(),
                        NoOfBranch = ds.Tables[0].AsEnumerable().Select(x => x.Field<int>("NoOFBranch")).FirstOrDefault(),
                    };
                }

                using (var conn = new SqlConnection(DbConnect.ConnectionString))
                {
                    sql = @"select IMAGE,F_ID,F_NAME,dbo.GetBranchNameById(BRANCH_ID) as BRANCH from tbl_Faculty where POST = 'HOD' ";

                    conn.Open();
                    using (var cmd = new SqlCommand(sql, conn))
                    {
                        using (var da = new SqlDataAdapter(cmd))
                        {
                            da.Fill(ds);
                        }
                    }
                }  
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return Json(resp,JsonRequestBehavior.AllowGet);
        }
        [HttpGet]
        public ActionResult GetAllHODList()
        {
            DataSet ds = new DataSet();
            dynamic resp = new { };
            try
            {
                string sql = string.Empty;

                using (var conn = new SqlConnection(DbConnect.ConnectionString))
                {
                    sql = @"select IMAGE,F_ID,F_NAME,dbo.GetBranchNameById(BRANCH_ID) as BRANCH,GENDER from tbl_Faculty where POST = 'HOD' ";

                    conn.Open();
                    using (var cmd = new SqlCommand(sql, conn))
                    {
                        using (var da = new SqlDataAdapter(cmd))
                        {
                            da.Fill(ds);
                        }
                    }
                }
                if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                {

                    var data = ds.Tables[0].AsEnumerable().Select(x => new TblFaculty
                    {
                        F_NAME = x.Field<string>("F_NAME"),
                        F_ID = x.Field<string>("F_ID"),
                        BRANCH_ID = x.Field<string>("BRANCH"),
                        IMAGE = x.Field<object>("IMAGE") != DBNull.Value && x.Field<byte[]>("IMAGE") != null
                                ? x.Field<byte[]>("IMAGE").Select(y => (byte?)y).ToArray() : null,
                        IMAGE_BASE64 = ConvertToBase64(x.Field<object>("IMAGE") != DBNull.Value && x.Field<byte[]>("IMAGE") != null
                                ? x.Field<byte[]>("IMAGE").Select(y => (byte?)y).ToArray() : null),
                        GENDER = x.Field<string>("GENDER")
                    }).ToList();
                    resp = data;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return Json(resp, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult GetTop3EventList()
        {
            DataSet ds = new DataSet();
            dynamic resp = new { };
            try
            {
                string sql = string.Empty;

                using (var conn = new SqlConnection(DbConnect.ConnectionString))
                {
                    sql = @"SELECT TOP (3) EVENT_ID
                        ,EVENT_DESC
                        ,EVENT_TYPE
                        ,START_DATE
                        ,END_DATE
                        ,EVENT_NAME
                        FROM Tbl_Event
                        order by START_DATE DESC";

                    conn.Open();
                    using (var cmd = new SqlCommand(sql, conn))
                    {
                        using (var da = new SqlDataAdapter(cmd))
                        {
                            da.Fill(ds);
                        }
                    }
                }
                if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                {

                    var data = ds.Tables[0].AsEnumerable().Select(x => new Event
                    {
                        EVENT_NAME = x.Field<string>("EVENT_NAME"),
                        EVENT_DESC = x.Field<string>("EVENT_DESC"),
                        STR_START_DATE = x.Field<DateTime>("START_DATE").ToString("dd-MM-yyyy"),
                        STR_END_DATE = x.Field<DateTime>("END_DATE").ToString("dd-MM-yyyy"),
                        START_DATE = x.Field<DateTime>("START_DATE"),
                        END_DATE = x.Field<DateTime>("END_DATE")
                    }).ToList();
                    resp = data;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return Json(resp, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult GetOverallStudentNo(string Sessions)
        {
            DataSet ds = new DataSet();
            DataSet branch = new DataSet();

            var GraphData = new List<TblStudent>();
            var BranchList = new List<string>();
            try
            {
                string sql = string.Empty;

                using (var conn = new SqlConnection(DbConnect.ConnectionString))
                {
                    sql = @"SELECT 
		                        [S_ID]
                              ,[S_NAME]
                              ,[BRANCH_ID]
                              ,[SESSION_DATE]
      
                          FROM [IntelliCollab].[dbo].[Tbl_Student]
                          WHERE SESSION_DATE = @SESSION_DATE";

                    conn.Open();
                    using (var cmd = new SqlCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@SESSION_DATE", Sessions);
                        using (var da = new SqlDataAdapter(cmd))
                        {
                            da.Fill(ds);
                        }
                    }
                    sql = "SELECT Dept_ID from Tbl_Department";
                    using (var cmd = new SqlCommand(sql, conn))
                    {
                        using (var da = new SqlDataAdapter(cmd))
                        {
                            da.Fill(branch);
                        }
                    }
                }
                if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                {

                    GraphData = ds.Tables[0].AsEnumerable().Select(x => new TblStudent
                    {
                        ID = x.Field<string>("S_ID"),
                        NAME = x.Field<string>("S_NAME"),
                        BRANCH = x.Field<string>("BRANCH_ID"),
                        SESSION = x.Field<string>("SESSION_DATE")
                    }).ToList();
                }
                if (branch != null && branch.Tables.Count > 0 && branch.Tables[0].Rows.Count > 0)
                {
                    BranchList = branch.Tables[0].AsEnumerable().Select(x =>  x.Field<string>("Dept_ID")).ToList();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return Json(new { 
                GraphData = GraphData,
                BranchList = BranchList
            }, JsonRequestBehavior.AllowGet);
        }
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
    }


}
