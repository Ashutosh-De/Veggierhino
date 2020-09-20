using P_GHAR_API.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace P_GHAR_API.Classes
{
    public class SignUp
    {
        public string FIRST_NAME { get; set; }
        public string LAST_NAME { get; set; }
        public string EMAIL_ID { get; set; }
        public string MOBILE_NO { get; set; }
        public string PASSWORD { get; set; }
    }
    public class ForgotPsd
    {
        public string EMAIL_ID { get; set; }
    }

    
    public class ItemsList
    {
        public string CATEGORY_NAME { get; set; }
        public string ITEM_ID { get; set; }
        public string ITEM_NAME { get; set; }
        public decimal? ITEM_PRICE { get; set; }
        public decimal? ITEM_QUANTITY { get; set; }
        public string ITEM_UNIT { get; set; }
        public string ITEM_DESCRIPTION { get; set; }
        public byte[] ITEM_PHOTO { get; set; }
        public string C_ON { get; set; }
    }

    public class CategoryWiseItems
    {
        public int CategoryId {get;set;}
        public string Name{ get;set;}
        public List<ItemsList> subCategories { get; set; }
    }
    public class mydata
    {
        public string CODEID { get; set; }
        public string Name { get; set; }
        public string ITEM_ID { get; set; }
        public string ITEM_NAME { get; set; }
        public int CATEGORY_ID { get; set; }
        public string CATEGORY_NAME { get; set; }
        public string ITEM_DESCRIPTION { get; set; }
        public decimal? ITEM_PRICE { get; set; }
        public byte[] ITEM_PHOTO { get; set; }
        public string ITEM_UNIT { get; set; }
        public decimal? ITEM_QUANTITY { get; set; }
        public string C_ON { get; set; }
    }
    
   
    public class items
    {
        public string CATEGORY_ID { get; set; }
    }
    public class Addtocarttemp
    {
        public string ITEM_ID { get; set; }
        public string ITEM_QUANTITY { get; set; }
    }
    public class Addtotemp
    {
        public string DEVICEID { get; set; }
        public string TOKEN { get; set; }
        public string USER_ID { get; set; }
        public Addtocarttemp[] ITEMDETAILS { get; set; }
    }

    public class Removetemp
    {
        public string DEVICEID { get; set; }
        public string TOKEN { get; set; }
        public string USER_ID { get; set; }
    }
    
    public class Category
    {
        public string CATEGORY_ID { get; set; }
        public string CATEGORY_NAME { get; set; }
        public string CATEGORY_PHOTO { get; set; }
    }
   
    public class AvlItem
    {
        public int ITEM_ID { get; set; }
        public decimal QUANTITY { get; set; }
    }

    public class profileupdate
    {
        public string DEVICE_ID { get; set; }
        public string TOKEN { get; set; }
        public string USER_ID { get; set; }
        public string MOBILE_NO { get; set; }
        public string ADDRESS { get; set; }
    }
}