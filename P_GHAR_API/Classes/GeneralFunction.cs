using P_GHAR_API.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Web;

namespace P_GHAR_API.Classes
{
    public class GeneralFunction
    {
        VeggierhinoEntities dbCon = new VeggierhinoEntities();

        public static string GetFileUrl(string serverUrl, bool forceHttps)
        {
            if (serverUrl.IndexOf("://") > -1)
                return serverUrl;

            string resultUrl = serverUrl;
            Uri originalUri = System.Web.HttpContext.Current.Request.Url;
            resultUrl = (forceHttps ? "https" : originalUri.Scheme) +
                "://" + originalUri.Authority + resultUrl;
            return resultUrl;
        } 

        public bool IsValidlogin(string USER_ID,string PASSWORD)
        {
            var isvaliduser = dbCon.PA_MOB_CUST_SIGHN_UP.AsEnumerable().Where(x => x.EMAIL_ID.ToUpper().Trim() == USER_ID.ToUpper().Trim() && x.PASSWORD == EncryptDecrypt.Encrypt(PASSWORD) && x.ISACTIVE == true).FirstOrDefault();
            if(isvaliduser!=null)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool sendsuccessmail(string receiveremailId, string CUSTNAME)
        {
            try
            {
                var senderEmail = new MailAddress("mauryashashikant061@gmail.com", "Welcome " + CUSTNAME);
                var receiverEmail = new MailAddress(receiveremailId, "Receiver");
                string subject = "VeggieRhino";
                string htmlBody = "Your Account Verified Successfully";

                MailMessage mail = new MailMessage();
                mail.To.Add(receiveremailId);
                mail.From = new MailAddress("mauryashashikant061@gmail.com", "VeggieRhino Customer Verification");
                mail.Subject = subject;
                mail.IsBodyHtml = true;
                string sbody = htmlBody;

                mail.Body = sbody;
                SmtpClient smtp = new SmtpClient();
                smtp.Host = "smtp.gmail.com";
                smtp.Port = 25;
                smtp.Credentials = new System.Net.NetworkCredential("mauryashashikant061@gmail.com", "shashisonali");
                smtp.EnableSsl = true;
                if (receiveremailId != "")
                {
                    smtp.Send(mail);
                }
                return true;
            }
            catch(Exception ex)
            {
                throw (ex);
            }
        }

        public bool SendEmail(string receiveremailId, int CUSTID, string CUSTNAME)
        {
            try
            {
                var senderEmail = new MailAddress("veggierhino@gmail.com", "Welcome " + CUSTNAME);
                var receiverEmail = new MailAddress(receiveremailId, "Receiver");
               // var password = "shashisonali";
                string subject = "VeggieRhino";
                string htmlBody;
                htmlBody = PopulateBody(CUSTID, CUSTNAME);

                MailMessage mail = new MailMessage();
                mail.To.Add(receiveremailId);
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
                if (receiveremailId != "")
                {
                    smtp.Send(mail);
                }
                return true;
            }
            catch (Exception ex)
            {
                return false;
                throw (ex);
            }
        }

        public string PopulateBody(int CUSTID, string CUSTNAME)
        {
            string customer_id = Convert.ToString(CUSTID);
            //var url = GetFileUrl(VirtualPathUtility.ToAbsolute("/Home/VerifyAccount?CUST_ID=" + CUSTID + "&Date=" + DateTime.Now.ToString("dd/MM/yyy") + "&Tm=" + DateTime.Now.ToString("HH:mm")), false);
            //string url = "http://localhost:59792/Home/VerifyAccount?CUST_ID=" + CUSTID + "&Date=" + DateTime.Now.ToString("dd/MM/yyy") + "&Tm=" + DateTime.Now.ToString("HH:mm");//For Local
            string url = "http://api.veggierhino.com/Home/VerifyAccount?CUST_ID=" + CUSTID + "&Date=" + DateTime.Now.ToString("dd/MM/yyy") + "&Tm=" + DateTime.Now.ToString("HH:mm");//For Live
            //string imgSrc = string.Format("@{0}{1}", HttpContext.Current.Request.Url.Authority, "/Content/Category/Veggi.jpg");
           // var imgSrc = GetFileUrl(VirtualPathUtility.ToAbsolute("~/Content/Category/1.jpg"), false);
            //F:\PaagGhar\P_GHAR_API\P_GHAR_API\Content\Category\Veggi.jpeg
            StringBuilder sb = new StringBuilder();
            //sb.AppendLine(string.Format("<img src='{0}' alt='Veggierhino'/>", imgSrc));
            sb.AppendLine("<html><body><font size='2' color='#333'>");
            sb.AppendLine("<p>Welcome " + CUSTNAME + ", We are very happy to join  you. Please Verify Your Account.</p>");
            sb.AppendLine("<p>Date:- " + DateTime.Now + "</p>");
            sb.AppendLine("<p style='font size=10'>Url is Valid Only for 2 Hours</p>");
            sb.AppendLine("<a href='" + url + "' + Visit W3Schools.com!</a>");
            sb.AppendLine(" <p style='text-align: center;background-color: aquamarine;'><button class='btn btn-primary'>Verify Your Account</button></p>");
            sb.AppendLine("</body></html>");
            string sbody = sb.ToString();
            return sbody;
        }

        public bool VerifyAccount(int Customer_id, string Date, string Tm)
        {
            try
            {
                TimeSpan time = TimeSpan.Parse(Tm);
                DateTime f_time = DateTime.ParseExact(Date, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                DateTime f_time112 = f_time + time;

                DateTime d1 = new DateTime(f_time112.Year, f_time112.Month, f_time112.Day, f_time112.Hour, f_time112.Minute, f_time112.Second);
                DateTime d2 = d1.AddHours(2);
                DateTime d3 = DateTime.Now;
                int res = DateTime.Compare(d2, d3);
                if (res > 0)
                {
                    PA_MOB_CUST_SIGHN_UP cfs = dbCon.PA_MOB_CUST_SIGHN_UP.AsEnumerable().Where(x => x.CUST_ID == Customer_id).SingleOrDefault();
                    cfs.ISACTIVE = true; ;
                    dbCon.SaveChanges();
                    dbCon.Entry(cfs).State = EntityState.Detached;
                    return true;
                    //ViewBag.verifiedAccount = "Your Account verified Sucessfully Now Uou can Log in from App.";
                }
                else
                {
                    return false;
                    //ViewBag.verifiedAccount = "Sorry Link Validity Has Been Expired";
                }
                //return View("VerifyAccount");
            }
            catch (Exception ex)
            {
                throw (ex);
            }
        }

        public bool ValidateToken(string DEVICE_ID, string TOKEN)
        {
            try
            {
                var data = dbCon.PA_MOB_APP_TOKEN_AUTH.AsEnumerable().Where(x => x.DEVICE_ID == DEVICE_ID.ToUpper() && x.TOKEN == TOKEN).FirstOrDefault();
                if (data != null)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                return false;
            }
        }

    }
}