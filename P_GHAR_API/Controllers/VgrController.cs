using Newtonsoft.Json;
using P_GHAR_API.Classes;
using P_GHAR_API.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;

namespace P_GHAR_API.Controllers
{
    public class VgrController : ApiController
    {
        string json = string.Empty;
        VeggierhinoEntities db = new VeggierhinoEntities();
        GeneralFunction GF = new GeneralFunction();

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

        [HttpGet]
        public void Categories()
        {
            try
            {
                var categories = db.PA_MASTER_GENERAL.AsEnumerable().Where(a => a.CODEACCESS == true && a.CODEYPE == "Item_Category").ToList();
                List<Category> CATEGORYList = new List<Category>();

                var url = GetFileUrl(VirtualPathUtility.ToAbsolute("~/Content/Category/"), false);
                foreach (var mod in categories)
                {
                    CATEGORYList.Add(new Category()
                    {
                        CATEGORY_ID = mod.CODEID,
                        CATEGORY_NAME = mod.CODEDESC,
                        CATEGORY_PHOTO = url + "" + mod.CODEID + "" + ".jpg"
                    });
                }
                var routData = new CATE_LIST
                {
                    STATUS_CODE = 200,
                    MESSAGE = "Success",
                    CATEGORY_LIST = CATEGORYList
                };
                json = JsonConvert.SerializeObject(routData, Formatting.Indented);
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
        public void ItmbasedonCatg(items itm)
        {
            try
            {
                if(db.PA_MASTER_GENERAL.AsEnumerable().Where(x=>x.CODEACCESS==true&&x.CODEYPE== "Item_Category"&&x.CODEID==itm.CATEGORY_ID).FirstOrDefault()!=null)
                {
                    var Products = db.PA_MASTER_ITEM.AsEnumerable().Where(x => x.ITEM_CATEGORY == itm.CATEGORY_ID && x.ISACTIVE == true)
                              .Join(db.PA_MASTER_GENERAL.AsEnumerable().Where(a => a.CODEACCESS == true && a.CODEYPE == "Item_Category"), a => a.ITEM_CATEGORY, b => b.CODEID, (a, b) => new { a, b })
                              .Join(db.PA_MASTER_GENERAL.AsEnumerable().Where(a => a.CODEACCESS == true && a.CODEYPE == "Item_Unit"), c => c.a.ITEM_CATEGORY, d => d.CODEID, (c, d) => new { c, d }).ToList();
                    List<ItemsList> lstdata = new List<ItemsList>();
                    if (Products.Count > 0)
                    {
                        foreach (var mod in Products)
                        {
                            lstdata.Add(new ItemsList()
                            {
                                CATEGORY_NAME = mod.c.b.CODEDESC,
                                ITEM_ID = Convert.ToString(mod.c.a.ITEM_ID),
                                ITEM_NAME = mod.c.a.ITEM_NAME,
                                ITEM_DESCRIPTION = mod.c.a.ITEM_DESCRIPTION,
                                ITEM_PRICE = mod.c.a.ITEM_PRICE,
                                ITEM_PHOTO = mod.c.a.ITEM_PHOTO,
                                ITEM_UNIT = mod.d.CODEDESC,
                                ITEM_QUANTITY = mod.c.a.ITEM_QUANTITY,
                                C_ON = mod.c.a.ENTRYON.Value.Date.ToString("dd/MM/yyyy")
                            });
                        }
                        var routData = new CATE_ITEM_LIST
                        {
                            STATUS_CODE = 200,
                            MESSAGE = "Success",
                            ITEMS = lstdata
                        };
                        json = JsonConvert.SerializeObject(routData, Formatting.Indented);
                    }
                    else
                    {
                        var routData = new CMNObject
                        {
                            STATUS_CODE = 401,
                            MESSAGE = "Products Not Available"
                        };
                        json = JsonConvert.SerializeObject(routData, Formatting.Indented);
                    }
                }
                else
                {
                    var routData = new CMNObject
                    {
                        STATUS_CODE = 401,
                        MESSAGE = "Category Not Available"
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
        public void Addtocart(Addtotemp addobj)
        {
            try
            {
                bool tknVld = GF.ValidateToken(addobj.DEVICEID, addobj.TOKEN);
                if (tknVld == true)
                {
                    var ordDetails = addobj.ITEMDETAILS;
                    foreach(var item in ordDetails)
                    {
                        TEMP_PA_CART_DATA tp = new TEMP_PA_CART_DATA();
                        tp.CUSTOMER_ID = addobj.USER_ID;
                        tp.ITEM_ID = item.ITEM_ID;
                        tp.QUANTITY = item.ITEM_QUANTITY;
                        tp.CREATED_ON = DateTime.Now;
                        tp.ISACTIVE = true;
                        db.TEMP_PA_CART_DATA.Add(tp);
                        db.SaveChanges();
                    }

                    var routedata = new CMNObject
                    {
                        STATUS_CODE = 200,
                        MESSAGE = "Product Saved on Db"
                    };
                    json = JsonConvert.SerializeObject(routedata, Formatting.Indented);
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


        #region Not in Use Available item
        [NonAction]
        [HttpPost]
        public void AvailableItem(AvlItem avlitem)
        {
            try
            {
                var isavlitem = db.PA_MASTER_ITEM.AsEnumerable().Where(x => x.ITEM_ID == avlitem.ITEM_ID).FirstOrDefault();
                if (isavlitem.ITEM_QUANTITY >= avlitem.QUANTITY)
                {
                    var routData = new CMNObject
                    {
                        STATUS_CODE = 200,
                        MESSAGE = "Item Available"
                    };
                    json = JsonConvert.SerializeObject(routData, Formatting.Indented);
                }
                else
                {
                    var routData = new CMNObject
                    {
                        STATUS_CODE = 201,
                        MESSAGE = "Out of Stock"
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
        #endregion

        #region Category Wise Items
         [NonAction]
        [HttpGet]
        public void CategoryWiseItems(items itm)
        {
            try
            {

                var categories = db.PA_MASTER_ITEM.AsEnumerable().Select(c => c.ITEM_CATEGORY);
                List<CategoryWiseItems> subCategories = new List<CategoryWiseItems>();
                var data=(from f in db.PA_MASTER_ITEM.AsEnumerable().ToList()
                          join d in db.PA_MASTER_GENERAL.AsEnumerable().Where(a => a.CODEACCESS == true && a.CODEYPE == "Item_Category").ToList() on f.ITEM_CATEGORY equals d.CODEID
                                      select new mydata{
                                          CODEID = d.CODEID,
                                          Name =f.ITEM_NAME,
                                          ITEM_ID =Convert.ToString(f.ITEM_ID),
                                          ITEM_NAME =f.ITEM_NAME,
                                          CATEGORY_ID=Convert.ToInt32(f.ITEM_CATEGORY),
                                          CATEGORY_NAME = d.CODEDESC,
                                          ITEM_DESCRIPTION=f.ITEM_DESCRIPTION, 
                                          ITEM_PRICE =f.ITEM_PRICE,
                                          ITEM_PHOTO =f.ITEM_PHOTO,
                                          ITEM_UNIT =f.ITEM_UNIT,
                                          ITEM_QUANTITY =f.ITEM_QUANTITY,
                                         // C_ON =f.ENTRYON.ToString("dd/MM/yyyy")
                                      }).ToList();

                    foreach (var item in db.PA_MASTER_GENERAL.Where(a => a.CODEACCESS == true && a.CODEYPE == "Item_Category").ToList())
                    {
                        var data1 = data.Where(x => x.CODEID == item.CODEID.ToString()).ToList();
                        if (data1.Count() > 0)
                        {
                            CategoryWiseItems obj = new CategoryWiseItems();
                            List<ItemsList> obj1 = new List<ItemsList>();
                            obj.CategoryId = data1.Select(x => x.CATEGORY_ID).FirstOrDefault();
                            obj.Name = data1.Select(x => x.CATEGORY_NAME).FirstOrDefault();
                            foreach (var itemm in data1)
                            {
                                ItemsList obj2 = new ItemsList();
                                obj2.ITEM_ID = itemm.ITEM_ID;
                                obj2.ITEM_DESCRIPTION = itemm.ITEM_DESCRIPTION;
                                obj2.ITEM_PRICE = itemm.ITEM_PRICE;
                                obj2.ITEM_PHOTO = itemm.ITEM_PHOTO;
                                obj2.ITEM_UNIT = itemm.ITEM_UNIT;
                                obj2.ITEM_QUANTITY = itemm.ITEM_QUANTITY;
                                obj1.Add(obj2);
                            }
                            obj.subCategories = obj1;
                            subCategories.Add(obj);
                        }
                        
                    }
                   
                    var routData = new CATEGOR_LIST
                    {
                        STATUS_CODE = 200,
                        MESSAGE = "Success",
                        CATEGORY_LIST = subCategories
                    };
                    json = JsonConvert.SerializeObject(routData, Formatting.Indented);
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

    }
}
