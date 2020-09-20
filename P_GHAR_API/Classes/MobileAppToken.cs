
using P_GHAR_API.Classes;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
namespace P_GHAR_API.Models
{
    public class CMNObject
    {
        public int STATUS_CODE { get; set; }
        public string MESSAGE { get; set; }
    }

    public class MobileAppToken 
    {
        [Required]
        public string USER_ID { get; set; }
        [Required]
        public string PASSWORD { get; set; }
        //Token test 
        [Required]
        public string DEVICEID { get; set; }
    }
    public class SuccessDataObject : CMNObject
    {
        public string TOKEN { get; set; }
        public string USER_ID { get; set; }
        public string USER_NAME { get; set; }
        public string MOBILE_NO { get; set; }
    }
    public class CATE_LIST : CMNObject
    {
        public List<Category> CATEGORY_LIST { get; set; }
    }
    public class CATEGOR_LIST : CMNObject
    {
        public List<CategoryWiseItems> CATEGORY_LIST { get; set; }
    }
    public class CATE_ITEM_LIST : CMNObject
    {
        public List<ItemsList> ITEMS { get; set; }
    }
    public class Forgot_Password
    {
        public int CUST_ID { get; set; }
        public string M_NO { get; set; }

        [Required(ErrorMessage = "Password is required")]
        [DataType("Password")]
        public string Password { get; set; }

        [Display(Name = "Confirm password")]
        [Required(ErrorMessage = "Please enter confirm password")]
        [Compare("Password", ErrorMessage = "Confirm password doesn't match, Type again !")]
        [DataType(DataType.Password)]
        public string ConfirmPassword { get; set; }

    }

    public class Data
    {
        public List<ItemsList> current_condition { get; set; }
    }
}