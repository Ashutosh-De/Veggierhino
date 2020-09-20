using P_GHAR_API.Classes;
using P_GHAR_API.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace P_GHAR_API.DataAccess
{
    public class TokenGeneration
    {
        GeneralFunction GF = new GeneralFunction();

        public bool UpdateToken(MobileAppToken userToken, string TOKEN)
        {
            bool retval = false;
            try
            {
                using (VeggierhinoEntities objEntity = new VeggierhinoEntities())
                {
                    bool Result = GF.IsValidlogin(userToken.USER_ID, userToken.PASSWORD);
                    if(Result==true)
                    {
                        PA_MOB_APP_TOKEN_AUTH objToken = objEntity.PA_MOB_APP_TOKEN_AUTH.Where(e => e.LOGIN_ID == userToken.USER_ID && e.DEVICE_ID == userToken.DEVICEID).FirstOrDefault();
                        if (objToken != null)
                        {
                            objToken.TOKEN = TOKEN;
                            objToken.U_ON = DateTime.Now;
                        }
                        else
                        {
                            objToken = new PA_MOB_APP_TOKEN_AUTH();
                            objToken.TOKEN = TOKEN;
                            objToken.DEVICE_ID = userToken.DEVICEID.ToUpper();
                            objToken.LOGIN_ID = userToken.USER_ID;
                            objToken.C_ON = DateTime.Now;
                            objToken.U_ON = DateTime.Now;
                            objEntity.PA_MOB_APP_TOKEN_AUTH.Add(objToken);
                        }
                        objEntity.SaveChanges();
                        retval = true;
                    }
                }

            }
            catch (Exception ex)
            {
                throw ex;
            }
            return retval;
        }
    }
}