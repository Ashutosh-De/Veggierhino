//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace P_GHAR_API.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class PA_ORDER_DETAILS
    {
        public int SRNO { get; set; }
        public string ORDER_ID { get; set; }
        public string DELIVERY_BOYID { get; set; }
        public string DELIVERY_ADDRESS { get; set; }
        public string CUSTOMER_ID { get; set; }
        public bool IS_ORDER_CONFIRM { get; set; }
        public string REMARKS { get; set; }
        public bool IS_DELIVERED { get; set; }
        public bool IS_REJECT { get; set; }
        public string REJECT_REASON { get; set; }
        public bool IS_CANCEL { get; set; }
        public string CANCEL_REASON { get; set; }
        public string TranscationID { get; set; }
    }
}