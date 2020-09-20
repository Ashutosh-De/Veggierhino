using P_GHAR_API.Classes;
using P_GHAR_API.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace P_GHAR_API.Controllers
{
    public class HomeController : Controller
    {
        VeggierhinoEntities db = new VeggierhinoEntities();
        GeneralFunction GF = new GeneralFunction();
        // GET: Home
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Change_Password(string M_NO, string CUST_ID, string Date, string Tm)
        {
            int customer_id = Convert.ToInt32(CUST_ID);

            TimeSpan time = TimeSpan.Parse(Tm);
            DateTime f_time  = DateTime.ParseExact(Date, "dd/MM/yyyy", CultureInfo.InvariantCulture);
            DateTime f_time112 = f_time + time;

            DateTime d1 = new DateTime(f_time112.Year, f_time112.Month, f_time112.Day, f_time112.Hour, f_time112.Minute, f_time112.Second);
            DateTime d2 = d1.AddHours(2);
            DateTime d3 = DateTime.Now;
            int res = DateTime.Compare(d2, d3);
            if(res>0)
            {
                Forgot_Password fp = new Forgot_Password();
                fp.M_NO = M_NO;
                fp.CUST_ID = customer_id;
                return View(fp);
            }
            else
            {
                return RedirectToAction("Expired", "Home", new { err = "Sorry Link Validity Has Been Expired" });
            }
        }

        public ActionResult Expired()
        {
            return View();
        }

        [HttpPost]
        public ActionResult ChangePSD(Forgot_Password fp)
        {
            try
            {
                if(true)
                {
                    if(fp.Password==fp.ConfirmPassword)
                    {
                        if (db.PA_MOB_CUST_SIGHN_UP.AsEnumerable().Where(x => x.CUST_ID == fp.CUST_ID).Count() > 0)
                        {
                            PA_MOB_CUST_SIGHN_UP cfs = db.PA_MOB_CUST_SIGHN_UP.AsEnumerable().Where(x => x.CUST_ID == fp.CUST_ID).SingleOrDefault();
                            cfs.PASSWORD = EncryptDecrypt.Encrypt(fp.Password);
                            db.Entry(cfs).State = EntityState.Modified;
                            db.SaveChanges();
                        }
                        ViewBag.msg = "Password Succefully Changed .";
                    }
                    return View("Change_Password");
                }
            }
            catch(Exception ex)
            {
                throw (ex);
            }
        }

        public ActionResult VerifyAccount(int CUST_ID, string Date, string Tm)
        {
            bool isverify = GF.VerifyAccount(CUST_ID, Date, Tm);
            if (isverify==true)
            {
               // GF.sendsuccessmail(db.PA_MOB_CUST_SIGHN_UP.AsEnumerable().Where(x => x.CUST_ID == CUST_ID).Select(x => x.EMAIL_ID).FirstOrDefault(), db.PA_MOB_CUST_SIGHN_UP.AsEnumerable().Where(x => x.CUST_ID == CUST_ID).Select(x => x.FIRST_NAME).FirstOrDefault());
                ViewBag.verifiedAccount = "Your Account verified Sucessfully Now You can Log in from App.";
            }
            else
            {
                ViewBag.verifiedAccount = "Sorry Link Validity Has Been Expired";
            }
            return View();
        }
        
    }
}