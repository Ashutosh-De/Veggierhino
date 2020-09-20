using Newtonsoft.Json;
using P_GHAR_API.Classes;
using P_GHAR_API.DataAccess;
using P_GHAR_API.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Mail;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Http;

namespace P_GHAR_API.Controllers
{
    public class UserController : ApiController
    {
        string json = string.Empty;
        TokenGeneration objTokenGeneration = new TokenGeneration();
        VeggierhinoEntities db = new VeggierhinoEntities();
        GeneralFunction GF = new GeneralFunction();

        #region Log in and Sign Up Update Profile
        //Sign Up
        [HttpPost]
        public void UserSignUp(P_GHAR_API.Classes.SignUp data)
        {
            try
            {
                if (db.PA_MOB_CUST_SIGHN_UP.AsEnumerable().Where(x => x.EMAIL_ID.ToUpper().Trim() == data.EMAIL_ID.ToUpper().Trim()).Count() > 0)
                {
                    var routData = new CMNObject
                    {
                        STATUS_CODE = 201,
                        MESSAGE = "Credential already exists."
                    };
                    json = JsonConvert.SerializeObject(routData, Formatting.Indented);
                }
                else
                {
                    PA_MOB_CUST_SIGHN_UP SU = new PA_MOB_CUST_SIGHN_UP();
                    SU.FIRST_NAME = data.FIRST_NAME;
                    SU.LAST_NAME = data.LAST_NAME;
                    SU.EMAIL_ID = data.EMAIL_ID;
                    SU.MOBILE_NO = data.MOBILE_NO;
                    SU.ISACTIVE = false;
                    SU.PASSWORD = EncryptDecrypt.Encrypt(data.PASSWORD);
                    SU.C_ON = DateTime.Now.ToString("dd/MM/yyyy");
                    db.PA_MOB_CUST_SIGHN_UP.Add(SU);
                    db.SaveChanges();

                    string CUSTNAME = data.FIRST_NAME + " " + data.LAST_NAME;
                    var senderEmail = new MailAddress("veggierhino@gmail.com", "Welcome " + CUSTNAME);
                    var receiverEmail = new MailAddress(data.EMAIL_ID, "Receiver");
                    // var password = "shashisonali";
                    string subject = "VeggieRhino";
                    string htmlBody;
                    htmlBody = GF.PopulateBody(db.PA_MOB_CUST_SIGHN_UP.AsEnumerable().Where(x => x.EMAIL_ID == data.EMAIL_ID).Select(x => x.CUST_ID).FirstOrDefault(), CUSTNAME);

                    MailMessage mail = new MailMessage();
                    mail.To.Add(data.EMAIL_ID);
                    mail.From = new MailAddress("veggierhino@gmail.com", "VeggieRhino Customer Verification");
                    mail.Subject = subject;
                    mail.IsBodyHtml = true;
                    string sbody = htmlBody;

                    mail.Body = sbody;
                    SmtpClient smtp = new SmtpClient();
                    smtp.Host = "smtp.gmail.com";
                    smtp.Port = 25;
                    smtp.Credentials = new System.Net.NetworkCredential("veggierhino@gmail.com", "VeggieRhino@2020");
                    smtp.EnableSsl = true;
                    if (data.EMAIL_ID != "")
                    {
                        smtp.Send(mail);
                    }

                    var routData = new CMNObject
                    {
                        STATUS_CODE = 200,
                        MESSAGE = "Succesfully Sign Up,Link Shared on your email id please Verify Your Account."
                    };
                    json = JsonConvert.SerializeObject(routData, Formatting.Indented);
                   // bool ismailsent =   GF.SendEmail(data.EMAIL_ID, db.PA_MOB_CUST_SIGHN_UP.AsEnumerable().Where(x => x.EMAIL_ID == data.EMAIL_ID).Select(x => x.CUST_ID).FirstOrDefault(), data.FIRST_NAME);
                   // if(ismailsent==true)
                   // {
                   //     var routData = new CMNObject
                   //     {
                   //         STATUS_CODE = 200,
                   //         MESSAGE = "Succesfully Sign Up,Link Shared on your email id please Verify Your Account"
                   //     };
                   //     json = JsonConvert.SerializeObject(routData, Formatting.Indented);
                   // }
                   //else
                   // {
                   //     var routData = new CMNObject
                   //     {
                   //         STATUS_CODE = 200,
                   //         MESSAGE = "Succesfully Sign Up,Some problem Occured during sending verification mail on your email id."
                   //     };
                   //     json = JsonConvert.SerializeObject(routData, Formatting.Indented);
                   // }
                }
            }
            catch (Exception ex)
            {
                var routData = new CMNObject
                {
                    STATUS_CODE = 401,
                    MESSAGE = ex.Message
                };
                json = JsonConvert.SerializeObject(routData, Formatting.Indented);
            }
            HttpContext.Current.Response.Clear();
            HttpContext.Current.Response.Buffer = true;
            HttpContext.Current.Response.Charset = "UTF-8";
            HttpContext.Current.Response.ContentEncoding = System.Text.Encoding.UTF8;
            HttpContext.Current.Response.ContentType = "application/json";
            HttpContext.Current.Response.Write(json);
            HttpContext.Current.Response.End();
        }

        // GET: User
        [HttpPost]
        public void UserAuthentication(MobileAppToken userToken)
        {
            try
            {
                if (true)
                {
                    string TOKEN = GenerateToken();
                    #region login
                    if (objTokenGeneration.UpdateToken(userToken, TOKEN) == true)
                    {
                        var data = db.PA_MOB_CUST_SIGHN_UP.AsEnumerable().Where(x => x.EMAIL_ID.ToUpper().Trim() == userToken.USER_ID.ToUpper().Trim() && x.PASSWORD == EncryptDecrypt.Encrypt(userToken.PASSWORD) && x.ISACTIVE == true).ToList();
                        try
                        {
                            var routData = new SuccessDataObject
                            {
                                STATUS_CODE = 200,
                                MESSAGE = "Succesfully Login",
                                TOKEN = TOKEN,
                                USER_ID = userToken.USER_ID,
                                USER_NAME = data.AsEnumerable().Select(x=>x.FIRST_NAME).FirstOrDefault()+" "+data.AsEnumerable().Select(x=>x.LAST_NAME).FirstOrDefault(),
                                MOBILE_NO = data.AsEnumerable().Select(x=>x.MOBILE_NO).FirstOrDefault()
                            };
                            json = JsonConvert.SerializeObject(routData, Formatting.Indented);
                        }
                        catch (Exception ex)
                        {
                            var routData = new CMNObject
                            {
                                STATUS_CODE = 500,
                                MESSAGE = "Login Fail",
                            };
                            json = JsonConvert.SerializeObject(routData, Formatting.Indented);
                        }
                    }
                    #endregion
                    else
                    {
                        var routData = new CMNObject
                        {
                            STATUS_CODE = 404,
                            MESSAGE = "User Not Authenticated"
                        };
                        json = JsonConvert.SerializeObject(routData, Formatting.Indented);
                    }
                }
                else
                {
                    var routData = new CMNObject
                    {
                        STATUS_CODE = 201,
                        MESSAGE = "Wrong User Cridentials !!"
                    };
                    json = JsonConvert.SerializeObject(routData, Formatting.Indented);
                }
            }
            catch (Exception ex)
            {
                var routData = new CMNObject
                {
                    STATUS_CODE = 401,
                    MESSAGE = ex.Message
                };
                json = JsonConvert.SerializeObject(routData, Formatting.Indented);
            }
            HttpContext.Current.Response.Clear();
            HttpContext.Current.Response.Buffer = true;
            HttpContext.Current.Response.Charset = "UTF-8";
            HttpContext.Current.Response.ContentEncoding = System.Text.Encoding.UTF8;
            HttpContext.Current.Response.ContentType = "application/json";
            HttpContext.Current.Response.Write(json);
            HttpContext.Current.Response.End();
        }

        [HttpPost]
        public void updateprofile(profileupdate prof)
        {
            try
            {
                bool tknVld = GF.ValidateToken(prof.DEVICE_ID, prof.TOKEN);
                if (tknVld == true)
                {
                    PA_MOB_CUST_SIGHN_UP cfs = db.PA_MOB_CUST_SIGHN_UP.AsEnumerable().Where(x => x.EMAIL_ID.ToUpper().Trim() == prof.USER_ID.ToUpper().Trim() && x.ISACTIVE == true).SingleOrDefault();
                    if (cfs!=null)
                    {
                        cfs.MOBILE_NO = prof.MOBILE_NO;
                        cfs.ADDRESS = prof.ADDRESS;
                        db.SaveChanges();
                        db.Entry(cfs).State = EntityState.Detached;
                        var routData = new CMNObject
                        {
                            STATUS_CODE = 200,
                            MESSAGE = "Profile Successfully Updated"
                        };
                        json = JsonConvert.SerializeObject(routData, Formatting.Indented);
                    }
                   else
                    {
                        var routedata = new CMNObject
                        {
                            STATUS_CODE = 401,
                            MESSAGE = "Invalid USER ID"
                        };
                        json = JsonConvert.SerializeObject(routedata, Formatting.Indented);
                    }
                }
                else
                {
                    var routedata = new CMNObject
                    {
                        STATUS_CODE = 401,
                        MESSAGE = "Invalid User"
                    };
                    json = JsonConvert.SerializeObject(routedata, Formatting.Indented);
                }
            }
            catch (Exception ex)
            {
                var routData = new CMNObject
                {
                    STATUS_CODE = 401,
                    MESSAGE = ex.Message
                };
                json = JsonConvert.SerializeObject(routData, Formatting.Indented);
            }
            HttpContext.Current.Response.Clear();
            HttpContext.Current.Response.Buffer = true;
            HttpContext.Current.Response.Charset = "UTF-8";
            HttpContext.Current.Response.ContentEncoding = System.Text.Encoding.UTF8;
            HttpContext.Current.Response.ContentType = "application/json";
            HttpContext.Current.Response.Write(json);
            HttpContext.Current.Response.End();
        }
        #endregion

        # region Change Password
        [HttpPost]
        public void Forgot_Psd(ForgotPsd psd)
        {
            try
            {
                var M_NO = db.PA_MOB_CUST_SIGHN_UP.AsEnumerable().Where(x => x.EMAIL_ID == psd.EMAIL_ID).Select(x => x.MOBILE_NO).FirstOrDefault();
                if(M_NO.ToList().Count>0||M_NO!=null)
                {
                    var last4digit = M_NO.Substring(M_NO.Length - 4, 4);
                    string en_M_NO = EncryptDecrypt.Encrypt(last4digit);
                    string subject = "Reset Your Password for Log in";
                    string receiver = psd.EMAIL_ID;
                    try
                    {
                        if (subject != null)
                        {
                            //var senderEmail = new MailAddress("mauryashashikant061@gmail.com", "Pak Ghar Reset Password");
                            // var receiverEmail = new MailAddress(receiver, "Receiver");
                            // var password = "shashisonali";
                            string htmlBody;
                            htmlBody = PopulateBody(db.PA_MOB_CUST_SIGHN_UP.AsEnumerable().Where(x => x.EMAIL_ID == psd.EMAIL_ID).Select(x => x.FIRST_NAME).FirstOrDefault(), en_M_NO, db.PA_MOB_CUST_SIGHN_UP.AsEnumerable().Where(x => x.EMAIL_ID == psd.EMAIL_ID).Select(x => x.CUST_ID).FirstOrDefault());

                            MailMessage mail = new MailMessage();
                            mail.To.Add(receiver);
                            mail.From = new MailAddress("veggierhino@gmail.com", "Grahak Mart Reset Password");
                            mail.Subject = subject;
                            mail.IsBodyHtml = true;
                            string sbody = htmlBody;

                            mail.Body = sbody;
                            SmtpClient smtp = new SmtpClient();
                            smtp.Host = "smtp.gmail.com";
                            smtp.Port = 25;
                            smtp.Credentials = new System.Net.NetworkCredential("veggierhino@gmail.com", "VeggieRhino@2020");
                            smtp.EnableSsl = true;
                            if (receiver != "")
                            {
                                smtp.Send(mail);
                            }

                            var routData = new CMNObject
                            {
                                STATUS_CODE = 200,
                                MESSAGE = "Link Shared on Your EMail ID , Reset Your  Password."
                            };
                            json = JsonConvert.SerializeObject(routData, Formatting.Indented);
                        }
                    }
                    catch (Exception ex)
                    {
                        var routData = new CMNObject
                        {
                            STATUS_CODE = 401,
                            MESSAGE = ex.Message
                        };
                        json = JsonConvert.SerializeObject(routData, Formatting.Indented);
                    }
                }
                else
                {
                    var routData = new CMNObject
                    {
                        STATUS_CODE = 201,
                        MESSAGE = "Invalid User!!.."
                    };
                    json = JsonConvert.SerializeObject(routData, Formatting.Indented);
                }
            }
            catch (Exception ex)
            {
                var routData = new CMNObject
                {
                    STATUS_CODE = 401,
                    MESSAGE = ex.Message
                };
                json = JsonConvert.SerializeObject(routData, Formatting.Indented);
            }
            HttpContext.Current.Response.Clear();
            HttpContext.Current.Response.Buffer = true;
            HttpContext.Current.Response.Charset = "UTF-8";
            HttpContext.Current.Response.ContentEncoding = System.Text.Encoding.UTF8;
            HttpContext.Current.Response.ContentType = "application/json";
            HttpContext.Current.Response.Write(json);
            HttpContext.Current.Response.End();
        }

        private string PopulateBody(string userName, string en_M_NO, int CUST_ID)
        {
            string customer_id = Convert.ToString(CUST_ID);
           // string url = "http://localhost:59792/Home/Change_Password?M_NO=" + en_M_NO + "&CUST_ID=" + customer_id + "&Date=" + DateTime.Now.ToString("dd/MM/yyy") + "&Tm=" + DateTime.Now.ToString("HH:mm");//For Local
            string url = "http://api.veggierhino.com/Home/Change_Password?M_NO=" + en_M_NO + "&CUST_ID=" + customer_id + "&Date=" + DateTime.Now.ToString("dd/MM/yyy") + "&Tm=" + DateTime.Now.ToString("HH:mm");//For Live
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("<html><body><font size='2' color='#333'>");
            sb.AppendLine("<p>Dear " + userName + " ,</p>");
            sb.AppendLine("<p>Date:- " + DateTime.Now + "</p>");
            sb.AppendLine("<a href='" + url + "' + Visit W3Schools.com!</a>");
            sb.AppendLine(" <p style='text-align: center;background-color: aquamarine;'><button class='btn btn-primary'>Reset Your Password</button></p>");
            sb.AppendLine("</body></html>");
            string sbody = sb.ToString();
            return sbody;
        }

        #endregion

        #region Generate Token
         [NonAction]
        public string GenerateToken()
        {
            long j = 1;
            foreach (byte b in Guid.NewGuid().ToByteArray())
            {
                j *= ((int)b + 1);
            }
            string Value = string.Format("{0:x}", j - DateTime.Now.Ticks);

            MD5 md5 = new MD5CryptoServiceProvider();

            //compute hash from the bytes of text  
            md5.ComputeHash(ASCIIEncoding.ASCII.GetBytes(Value));

            //get hash result after compute it  
            byte[] result = md5.Hash;

            StringBuilder strBuilder = new StringBuilder();
            for (int i = 0; i < result.Length; i++)
            {
                //change it into 2 hexadecimal digits  
                //for each byte  
                strBuilder.Append(result[i].ToString("x2"));
            }
            return strBuilder.ToString();
        }

        [NonAction]
        public string StoreToken(string DEVICE_ID, string C_BY)
        {
            string Token = GenerateToken();
            try
            {
                var data = db.PA_MOB_APP_TOKEN_AUTH.AsEnumerable().Where(x => x.DEVICE_ID == DEVICE_ID.ToUpper()).FirstOrDefault();
                if (data != null)
                {
                    data.TOKEN = Token;
                    data.U_ON = DateTime.Now.Date;
                    db.SaveChanges();
                }
                else
                {
                    PA_MOB_APP_TOKEN_AUTH obj = new PA_MOB_APP_TOKEN_AUTH();
                    obj.DEVICE_ID = DEVICE_ID.ToUpper();
                    obj.TOKEN = Token;
                    obj.C_ON = DateTime.Now.Date;
                    db.PA_MOB_APP_TOKEN_AUTH.Add(obj);
                    db.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                Token = null;
            }
            return Token;
        }
        #endregion
    }
}
