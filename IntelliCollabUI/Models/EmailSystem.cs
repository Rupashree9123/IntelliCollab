using IntelliCollabUI.Utility;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Web;

namespace IntelliCollabUI.Models
{
    public class EmailSystem
    {
        public static long OTP = 0;
        public static string receiverMail = "";

        // SMTP server configuration
        private string smtpServer = "smtp.gmail.com";
        private int smtpPort = 587; // Common port for Gmail SMTP
        private string smtpUser = "kushagrakumarmaity05@gmail.com";
        private string smtpPass = "**********************";  // enter your own password

        // Email message configuration
        private string fromEmail;//= "your-email@gmail.com";
        private string toEmail;//= "recipient-email@example.com";
        private string subject;//= "Test Email";
        private string body;//= "This is a test email sent from a C# application.";

        public EmailSystem(string toEmail,string subject,string body)
        {
            this.toEmail = toEmail;
            this.fromEmail = smtpUser;
            this.subject = subject;
            this.body = body;
        }

        public bool SendMail()
        {
            bool flg = false;
            // Create a new MailMessage object
            MailMessage mailMessage = new MailMessage(fromEmail, toEmail, subject, body);
            mailMessage.IsBodyHtml = true;
            // Enable SSL and configure the SMTP client
            SmtpClient smtpClient = new SmtpClient(smtpServer, smtpPort)
            {
                Credentials = new NetworkCredential(smtpUser, smtpPass),
                EnableSsl = true,
                
            };

            try
            {
                // Send the email
                smtpClient.Send(mailMessage);
                flg = true;
            }
            catch (Exception ex)
            {
                flg = false;
            }
            return flg;
        }
        
        public static string EventMsgDesign(string eventName,string eventDesc,DateTime startTime,DateTime endTime)
        {
            return $@"<!DOCTYPE html>
                    <html lang='en'>
                    <head>
                        <meta charset='UTF-8'>
                        <meta name='viewport' content='width=device-width, initial-scale=1.0'>
                        <title>Event Notification</title>
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
                            .header {{
                                font-size: 1.5em;
                                margin-bottom: 10px;
                            }}
                            .description {{
                                margin-top: 10px;
                            }}
                            .time {{
                                margin-top: 10px;
                                font-size: 1.1em;
                                color: #333;
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
                            <div class='header'>Event: {eventName}</div>
                            <div class='description'>{eventDesc}</div>
                            <div class='time'>
                                <p><strong>Start Time:</strong> {startTime.ToString("f")}</p>
                                <p><strong>End Time:</strong> {endTime.ToString("f")}</p>
                            </div>
                            <div class='footer'>
                                <p>Sincerely,</p>
                                <p>The Event Team</p>
                            </div>
                        </div>
                    </body>
                    </html>";
        }

        public static string GetEmailIds(string userType="ALL")
        {
            string email = string.Empty;
            DataSet ds = new DataSet();
            try
            {
                if (userType.Equals("ALL"))
                {
                    using (var conn = new SqlConnection(DbConnect.ConnectionString))
                    {
                        string sql = @"select a.S_GMAIL from 
                                (select S_GMAIL from Tbl_Student
                                union
                                select F_GMAIL from Tbl_Faculty)a";

                        conn.Open();
                        using (var cmd = new SqlCommand(sql, conn))
                        {

                            using (var da = new SqlDataAdapter(cmd))
                            {
                                da.Fill(ds);
                            }
                        }

                    }

                    if(ds!=null && ds.Tables[0].Rows.Count>0 )
                    {
                        email = string.Join(",", ds.Tables[0].AsEnumerable().Select(x => x.Field<string>("S_GMAIL")).ToList());
                    }
                   
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            
            return email;
        }
    }
}