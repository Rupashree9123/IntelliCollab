using IntelliCollabUI.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using IntelliCollabUI.Utility;
using System.Data;

namespace IntelliCollabUI.Controllers
{
    public class LogInController : Controller
    {
       
        public ActionResult LogIn()
        {
            if(Session["UserID"]!=null && Session["UserRole"] != null)
            {
                return RedirectToAction("Index", "DashBoard");
            }
            else
            {
                ViewBag.IsLogInOrRegPg = true;
                return View();
            }
        }

        [HttpPost]
        public ActionResult LogIn(string email, string password)
        {

            if (!string.IsNullOrEmpty(email) && !string.IsNullOrEmpty(password))
            {
                var getInfo = new DataSet();
                using (var conns = new SqlConnection(DbConnect.ConnectionString))
                {
                    conns.Open();
                    using (var cmd = new SqlCommand())
                    {
                        cmd.Connection = conns;
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.CommandText = "SP_USER_INFO";
                        // Add parameters
                        
                        cmd.Parameters.AddWithValue("@Gmail", email);
                        cmd.Parameters.AddWithValue("@Password", password);

                        using (var ad = new SqlDataAdapter(cmd))
                        {
                            ad.Fill(getInfo);
                        }
                    }
                }

                
                if (getInfo != null && getInfo.Tables.Count > 0 && getInfo.Tables[0].Rows.Count > 0)
                {
                    Session["UserName"] = getInfo.Tables[0].AsEnumerable().Select(x => x.Field<string>("Name")).FirstOrDefault();
                    Session["UserID"] = getInfo.Tables[0].AsEnumerable().Select(x => x.Field<string>("ID")).FirstOrDefault();
                    Session["UserRole"] = getInfo.Tables[0].AsEnumerable().Select(x => x.Field<string>("Role")).FirstOrDefault();
                    Session["GENDER"] = getInfo.Tables[0].AsEnumerable().Select(x => x.Field<string>("GENDER")).FirstOrDefault();
                    getRollAssessBaseOnUser(getInfo.Tables[0].AsEnumerable().Select(x => x.Field<string>("Role")).FirstOrDefault());

                    return RedirectToAction("Index", "DashBoard");
                }
                else
                {
                    ViewBag.ErrorMessage = "You are not authorized";
                    return View();
                }
            }
            else
            {
                ViewBag.ErrorMessage = "Invalid username or password.";
                return View();
            }
        }

        public ActionResult Registration()
        {
            ViewBag.IsLogInOrRegPg = true;
            return View();
        }

        public ActionResult SignUp()
        {
            ViewBag.IsLogInOrRegPg = true;
            return View();
        }

        [HttpPost]
        public ActionResult SignUp(string name,string email, string password, string password2, string branch,string userType,string gender,string ddlSession)
        {

            string newUserId = string.Empty;
            //password matching

            if (password == password2)
            {
                if (!string.IsNullOrEmpty(name) && email.Contains("@gmail.com") && !string.IsNullOrEmpty(password) && !string.IsNullOrEmpty(branch) && !string.IsNullOrEmpty(userType))
                {
                    using (var conns = new SqlConnection(DbConnect.ConnectionString))
                    {
                        conns.Open();
                        using (var cmd = new SqlCommand())
                        {
                            cmd.Connection = conns;
                            cmd.CommandType = CommandType.StoredProcedure;
                            cmd.CommandText = "SP_SING_UP";
                            // Add parameters

                            cmd.Parameters.AddWithValue("@Gmail", email);
                            cmd.Parameters.AddWithValue("@Password", password);
                            cmd.Parameters.AddWithValue("@BranchId", branch);
                            cmd.Parameters.AddWithValue("@UserType", userType);
                            //not required in signUp case
                            cmd.Parameters.AddWithValue("@Name", name);
                            cmd.Parameters.AddWithValue("@S_REG_NO", DBNull.Value);
                            cmd.Parameters.AddWithValue("@SESSION_DATE", ddlSession);
                            cmd.Parameters.AddWithValue("@DATE_OF_JOIN", DBNull.Value);
                            cmd.Parameters.AddWithValue("@DOB", DBNull.Value);
                            cmd.Parameters.AddWithValue("@MOBILE_NO", DBNull.Value);
                            cmd.Parameters.AddWithValue("@GENDER", gender);
                            cmd.Parameters.AddWithValue("@ROLL_NO", DBNull.Value);
                            cmd.Parameters.AddWithValue("@POST", DBNull.Value);

                            cmd.Parameters.Add(new SqlParameter()
                            {
                                ParameterName = "@NewUserId",
                                Direction = ParameterDirection.Output,
                                Size = 10
                            });
                            cmd.ExecuteNonQuery();

                            // Retrieve the value of the output parameter
                            newUserId = Convert.ToString(cmd.Parameters["@NewUserId"].Value);

                        }
                    }

                    if (newUserId is null || newUserId.Equals("NA"))
                    {
                        ViewBag.Message = null;
                        ViewBag.ErrorMessage = "Failed to create account!.";
                    }
                    else
                    {
                        ViewBag.Message = "Create successfully";
                        ViewBag.ErrorMessage = null;
                    }

                }
                else
                {
                    ViewBag.Message = null;
                    ViewBag.ErrorMessage = "Fake email";
                }
            }
            else
            {
                ViewBag.Message = null;
                ViewBag.ErrorMessage = "password not matching";
            }
            
            
            return View();
        }

        public ActionResult ForgetPwsd()
        {
            return View();
        }

        [HttpPost]
        public ActionResult SendOTP(string email)
        {
            try
            {
                Random random = new Random();
                int OTP = random.Next(100000,1000000);
                EmailSystem.OTP = OTP;
                EmailSystem.receiverMail = email;

                string body = $@"<!DOCTYPE html>
                                <html lang='en'>
                                <head>
                                    <meta charset='UTF-8'>
                                    <meta name='viewport' content='width=device-width, initial-scale=1.0'>
                                    <title>Verification Code</title>
                                    <style>
                                        body {{
                                            font-family: Arial, sans-serif;
                                            line-height: 1.6;
                                        }}
                                        .container {{
                                            max-width: 600px;
                                            margin: auto;
                                            padding: 20px;
                                            border: 1px solid #ccc;
                                            border-radius: 5px;
                                            background-color: #f9f9f9;
                                        }}
                                        .footer {{
                                            margin-top: 20px;
                                            font-size: 0.9em;
                                            color: #555;
                                        }}
                                    </style>
                                </head>
                                <body>
                                    <div class='container'>
                                        <p>Hi,</p>

                                        <p>We received a request to access your Intellicolab Account <strong>{email}</strong> through your email address. Your IntelliColab verification code is:</p>

                                        <p><strong>{OTP}</strong></p>

                                        <p>If you did not request this code, it is possible that someone else is trying to access the IntelliColab Account <strong>{email}</strong>. Do not forward or give this code to anyone.</p>

                                        <p>Sincerely yours,</p>

                                        <p>The IntelliColab Accounts team</p>
                                    </div>
                                </body>
                                </html>";


                EmailSystem obj = new EmailSystem(email, "IntelliColab Verification Code", body);
                if(obj.SendMail())
                {
                    ViewBag.OTP_FLAG = true;
                    ViewBag.msg = "An OTP has been successfully sent to your email.";
                }
                else
                {
                    ViewBag.OTP_FLAG = false;
                    ViewBag.msg = "Invalid mail";
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
           
            return View("ForgetPwsd");
        }

        [HttpPost]
        public ActionResult ChangePassword(long passcode,string passwords,string confirmPswd )
        {
            string userType = string.Empty;
            bool isUpdate = false;
            if (passwords.Equals(confirmPswd))
            {
                
                if (EmailSystem.OTP == passcode)
                {
                    EmailSystem.OTP = 0;//reset the OTP

                    using (var conn = new SqlConnection(DbConnect.ConnectionString))
                    {
                        string sql = @"select a.UserType from 
                                (select 'S' as UserType,S_GMAIL from Tbl_Student
                                union
                                select 'F' as UserType,F_GMAIL from Tbl_Faculty)a where a.S_GMAIL = @mail ";

                        conn.Open();
                        using (var cmd = new SqlCommand(sql, conn))
                        {
                            cmd.Parameters.AddWithValue("@mail", EmailSystem.receiverMail);
                            using (var reader = cmd.ExecuteReader())
                            {
                                if (reader.Read())
                                {
                                    userType = reader["UserType"].ToString();
                                }
                            }
                        }
                        
                    }

                    if(userType.Equals("S"))
                    {
                        using (var conn = new SqlConnection(DbConnect.ConnectionString))
                        {
                            string sql = @"UPDATE Tbl_Student
                                            SET
                                            PASSWORD = @pswd
                                            where S_GMAIL = @mail";

                            conn.Open();
                            using (var cmd = new SqlCommand(sql, conn))
                            {
                                cmd.Parameters.AddWithValue("@pswd", confirmPswd);
                                cmd.Parameters.AddWithValue("@mail", EmailSystem.receiverMail);
                                isUpdate = cmd.ExecuteNonQuery() > 0;
                            }

                        }
                    }
                    else if (userType.Equals("F"))
                    {
                        using (var conn = new SqlConnection(DbConnect.ConnectionString))
                        {
                            string sql = @"UPDATE Tbl_Faculty
                                            SET
                                            PASSWORD = @pswd
                                            where F_GMAIL = @mail";

                            conn.Open();
                            using (var cmd = new SqlCommand(sql, conn))
                            {
                                cmd.Parameters.AddWithValue("@pswd", confirmPswd);
                                cmd.Parameters.AddWithValue("@mail", EmailSystem.receiverMail);
                                isUpdate = cmd.ExecuteNonQuery() > 0;
                            }
                        }
                    }
                    else
                    {
                        ViewBag.msg = "No account found!.";
                    }
                     if(isUpdate)
                    {
                        ViewBag.msg = "Password change successfully";
                    }
                    else
                    {
                        ViewBag.msg = "Failed to change Password !.";
                    }
                    

                }
                else
                {
                    ViewBag.msg = "OTP is not matching";
                }
            }
            else
            {
                ViewBag.msg = "Both Password is not matching!.";
            }
            return View("ForgetPwsd");
        }
        private void getRollAssessBaseOnUser(String roleId)
        {
            try
            {
                DataSet ds = new DataSet();
                string sql = string.Empty;

                using (var conn = new SqlConnection(DbConnect.ConnectionString))
                {
                    if (roleId.Equals("A"))
                    {
                        sql = $@"SELECT [AccessId]
                              ,[Controller]
                              ,[Action]
                              ,[Active]
                              ,[ScreenInfo]
                              ,[RoleType]
                          FROM [IntelliCollab].[dbo].[AccessControl]";
                    }
                    if(roleId.Equals("F"))
                    {
                        sql = $@"SELECT [AccessId]
                              ,[Controller]
                              ,[Action]
                              ,[Active]
                              ,[ScreenInfo]
                              ,[RoleType]
                          FROM [IntelliCollab].[dbo].[AccessControl]
                          WHERE [RoleType] IN ('{roleId}','S')  or [RoleType] is null AND Active = 'Y'";
                    }
                    else if(roleId.Equals("S"))
                    {
                        sql = $@"SELECT [AccessId]
                              ,[Controller]
                              ,[Action]
                              ,[Active]
                              ,[ScreenInfo]
                              ,[RoleType]
                          FROM [IntelliCollab].[dbo].[AccessControl]
                          WHERE [RoleType] = '{roleId}'  or [RoleType] is null AND Active = 'Y'";
                    }
                    

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
                    Session["AssessList"] = ds.Tables[0].AsEnumerable().Select(x=> new AccessRole
                    { 
                        AccessId =  x.Field<decimal>("AccessId"),
                        Actions =  x.Field<string>("Action"),
                        Controller =  x.Field<string>("Controller"),
                        Active =  x.Field<string>("Active"),
                        RoleType =  x.Field<string>("RoleType"),
                        ScreenInfo =  x.Field<string>("ScreenInfo")
                    }).ToList();
                    
                }


            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [HttpGet]
        public ActionResult logOut()
        {
            Session.Abandon();
            Session.Clear();
            return View("LogIn");
        }
    }
}
