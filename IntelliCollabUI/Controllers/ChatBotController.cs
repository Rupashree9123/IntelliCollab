
using IntelliCollabUI.Models;
using IntelliCollabUI.Utility;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace IntelliCollabUI.Controllers
{
    public class ChatBotController : Controller
    {
        private string baseUrl = string.Empty;

        public ChatBotController()
        {
            baseUrl = System.Configuration.ConfigurationManager.AppSettings["BaseUrl"].ToString();
        }
        // GET: ChatBot
        public ActionResult Bot()
        {
            return View();
        }


        public async Task<ActionResult> TalkWithAiV2(string ques)
        {
            //string baseUrl = "http://127.0.0.1:5000/AiCall";
            string queryParam = ques;// "Are there any provisions for fee waivers for students who cannot afford the application fee?";

            string encodedQueryParam = Uri.EscapeUriString(queryParam);


            try
            {

                string usrName = Convert.ToString(Session["UserName"]);
                string ans = string.Empty;
                string intentKey = matchingIntent(ques, out ans);

                if (intentKey.Equals("POSITIVE"))
                {
                    var getMessage = new
                    {
                        CHAT_CODE = "THNAKS MSG",
                        CHAT_ID = 0,
                        CHAT_DESCRIPTION = DefaultAnswer.ThanksMsg()
                    };
                    return Json(getMessage, JsonRequestBehavior.AllowGet);
                }
                else if (intentKey.Equals("NEGATIVE"))
                {
                    var getMessage = new
                    {
                        CHAT_CODE = "NEGATIVE MSG",
                        CHAT_ID = 0,
                        CHAT_DESCRIPTION = DefaultAnswer.NegativeMsg()
                    };
                    return Json(getMessage, JsonRequestBehavior.AllowGet);
                }
                else if (intentKey.Equals("Hey"))
                {
                    var getMessage = new
                    {
                        CHAT_CODE = "Hey MSG",
                        CHAT_ID = 0,
                        CHAT_DESCRIPTION = DefaultAnswer.HeyMsg().Replace("{name}", usrName)
                    };
                    return Json(getMessage, JsonRequestBehavior.AllowGet);
                }
                else if (intentKey.Equals("Nev"))
                {
                    string url = $"{baseUrl}AiCallNev?Qa={encodedQueryParam}";

                    string jsonResponse = await GetAsync(url);
                    // Process jsonResponse as needed
                    var AIReply = JsonConvert.DeserializeObject<List<BotRespFromApi>>(jsonResponse);

                    //var getMessage = getChatDesc(AIReply.FirstOrDefault().label);
                    var bertModelReply = AIReply.FirstOrDefault();
                    var getMessage = (bertModelReply.score * 100) > 20 ? getChatDesc(bertModelReply.label) : getChatDesc("NOT_MATCH:");
                    return Json(getMessage, JsonRequestBehavior.AllowGet);
                }
                else if (intentKey.Equals("Fees"))
                {
                    string url = $"{baseUrl}AiCallFees?Qa={encodedQueryParam}";

                    string jsonResponse = await GetAsync(url);
                    // Process jsonResponse as needed
                    var AIReply = JsonConvert.DeserializeObject<List<BotRespFromApi>>(jsonResponse);

                    //var getMessage = getChatDesc(AIReply.FirstOrDefault().label);
                    var bertModelReply = AIReply.FirstOrDefault();
                    var getMessage = (bertModelReply.score * 100) > 20 ? getChatDesc(bertModelReply.label) : getChatDesc("NOT_MATCH:");
                    return Json(getMessage, JsonRequestBehavior.AllowGet);
                }
                else if (intentKey.Equals("LOC_ABOUT"))
                {
                    string url = $"{baseUrl}AiCallLocAbout?Qa={encodedQueryParam}";

                    string jsonResponse = await GetAsync(url);
                    // Process jsonResponse as needed
                    var AIReply = JsonConvert.DeserializeObject<List<BotRespFromApi>>(jsonResponse);
                    var bertModelReply = AIReply.FirstOrDefault();
                    var getMessage = (bertModelReply.score * 100) > 20 ? getChatDesc(bertModelReply.label) : getChatDesc("NOT_MATCH:");
                    return Json(getMessage, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    string url = $"{baseUrl}AiCall?Qa={encodedQueryParam}";

                    string jsonResponse = await GetAsync(url);
                    // Process jsonResponse as needed
                    var AIReply = JsonConvert.DeserializeObject<List<BotRespFromApi>>(jsonResponse);

                    //var getMessage = getChatDesc(AIReply.FirstOrDefault().label);
                    var bertModelReply = AIReply.FirstOrDefault();
                    var getMessage = (bertModelReply.score * 100) > 4 ? getChatDesc(bertModelReply.label) : getChatDesc("NOT_MATCH:");
                    return Json(getMessage, JsonRequestBehavior.AllowGet);
                }

            }
            catch (Exception ex)
            {
                // Handle exception
                return Json(getChatDesc(DefaultAnswer.ErrorAnswer()), JsonRequestBehavior.AllowGet);
            }
        }

        public async Task<string> GetAsync(string reqUrl)
        {
            using (var client = new HttpClient())
            {
                HttpResponseMessage response = await client.GetAsync(reqUrl);
                response.EnsureSuccessStatusCode(); // Throw exception if not successful

                return await response.Content.ReadAsStringAsync();
            }
        }

        private dynamic getChatDesc(string chatCode)
        {
            dynamic result = new { };

            DataSet getInfo = new DataSet();
            try
            {
                if (chatCode.Contains("ANS:"))
                {
                    result = new
                    {
                        CHAT_CODE = "ANS",
                        CHAT_ID = 0,
                        CHAT_DESCRIPTION = chatCode.Substring(4) + DefaultAnswer.EndingMsg()
                    };
                }
                else if (chatCode.Contains("NOT_MATCH:"))
                {
                    result = new
                    {
                        CHAT_CODE = "NOT MATCH",
                        CHAT_ID = 0,
                        CHAT_DESCRIPTION = DefaultAnswer.NotMatchingAnswer()
                    };
                }
                else
                {
                    using (var conns = new SqlConnection(DbConnect.ConnectionString))
                    {
                        conns.Open();
                        using (var cmd = new SqlCommand())
                        {
                            cmd.Connection = conns;
                            cmd.CommandType = CommandType.Text;
                            cmd.CommandText = @"SELECT CHAT_ID
                                            ,CHAT_CODE
                                            ,CHAT_DESCRIPTION
                                            FROM Tbl_ChatCode
                                            where CHAT_CODE = @chatCode";

                            // Add parameters
                            cmd.Parameters.AddWithValue("@chatCode", chatCode);

                            using (var ad = new SqlDataAdapter(cmd))
                            {
                                ad.Fill(getInfo);
                            }
                        }
                    }
                    if (getInfo != null && getInfo.Tables[0].Rows.Count > 0)
                    {
                        result = getInfo.Tables[0].AsEnumerable().Select(x => new
                        {
                            CHAT_CODE = x.Field<string>("CHAT_CODE"),
                            CHAT_ID = x.Field<decimal>("CHAT_ID"),
                            CHAT_DESCRIPTION = x.Field<string>("CHAT_DESCRIPTION") + "<br/>" + DefaultAnswer.EndingMsg()
                        }).FirstOrDefault();
                    }
                    else//not train part
                    {

                        result = new
                        {
                            CHAT_CODE = "NOT TRAIN",
                            CHAT_ID = 0,
                            CHAT_DESCRIPTION = DefaultAnswer.NotTrainAnswer()
                        };
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return result;
        }

        public ActionResult WelcomeMsg()
        {

            try
            {
                var getMessage = new
                {
                    CHAT_CODE = "WELCOME MSG",
                    CHAT_ID = 0,
                    CHAT_DESCRIPTION = DefaultAnswer.WellcomingAnswer()
                };
                return Json(getMessage, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                // Handle exception
                return Json(ex.Message, JsonRequestBehavior.AllowGet);
            }
        }

        private string matchingIntent(string input, out string ans)
        {
            ans = null;
            string reply = string.Empty;
            try
            {
                Regex regex = null;
                MatchCollection matches = null;

                #region thanks Intent
                string ThanksPattern = @"\b(ok(?:ay)?|okey|thanks?|thank(?:you)?|thx|ty)\b";
                string NegativePattern = @"\b(no|nope|nah|not a chance|sorry|incorrect|wrong|false|negative|mistaken|erroneous|inaccurate|unacceptable|not correct|not right|not accurate)\b";
                string HeyPattern = @"\b(hi|hello|hey|howdy|salutations|hiya|yo|wassup|aloha|bonjour|ciao)\b";
                string Nevigate = @"(Take me to\s+|Navigate to\s+|Show me\s+|Go to\s+|Open\s+|Show upcoming\s+|Navigate to\s+|Take\s+|Show\s+)";
                string Fees = @"((fee structure|fee (detail|details)|cost of .* program|.* fees?|program cost|tuition fees?|expense for .*|fee breakdown|program expenses?))";
                string LocAbout = @"\b(about MITM|information on MITM|give me details about MITM|insights about MITM|story behind MITM|complete name of MITM|full title of MITM|college is situated|location of your college|location of MITM|location of MITM college|full address|college location|college address)\b";


                regex = new Regex(ThanksPattern, RegexOptions.IgnoreCase);
                matches = regex.Matches(input);

                if (matches.Count > 0)
                {
                    reply = "POSITIVE";

                }
                else
                {
                    regex = null; matches = null;

                    regex = new Regex(NegativePattern, RegexOptions.IgnoreCase);
                    matches = regex.Matches(input);
                    if (matches.Count > 0)
                    {
                        reply = "NEGATIVE";

                    }
                    else
                    {

                        regex = null; matches = null;

                        regex = new Regex(HeyPattern, RegexOptions.IgnoreCase);
                        matches = regex.Matches(input);
                        if (matches.Count > 0)
                        {
                            reply = "Hey";

                        }
                        else
                        {
                            regex = null; matches = null;

                            regex = new Regex(Nevigate, RegexOptions.IgnoreCase);
                            matches = regex.Matches(input);
                            if (matches.Count > 0)
                            {
                                reply = "Nev";
                                ans = input;//input.Remove(0, matches[0].Value.Length - 1).Trim();
                            }
                            else
                            {
                                regex = null; matches = null;

                                regex = new Regex(Fees, RegexOptions.IgnoreCase);
                                matches = regex.Matches(input);
                                if (matches.Count > 0)
                                {
                                    reply = "Fees";
                                    ans = input;//"ME fee structure";//input.Remove(0, matches[0].Value.Length - 1).Trim();
                                }
                                else
                                {
                                    regex = null; matches = null;

                                    regex = new Regex(LocAbout, RegexOptions.IgnoreCase);
                                    matches = regex.Matches(input);
                                    if (matches.Count > 0)
                                    {
                                        reply = "LOC_ABOUT";
                                        ans = input;//input.Remove(0, matches[0].Value.Length - 1).Trim();
                                    }
                                    else
                                    {
                                        reply = "OTHER";
                                    }
                                }
                            }
                        }

                    }
                }
                #endregion


            }
            catch (Exception ex)
            {

            }

            return reply;
        }
    }
}