//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace BKBTitas.EntityModel
{
    using System;
    
    public partial class BranchWiseMeterDetails_Result
    {
        public string zone { get; set; }
        public string branch_name { get; set; }
        public string RoutingNo { get; set; }
        public Nullable<System.DateTime> transactionDateTime { get; set; }
        public int paymentId { get; set; }
        public string refNo { get; set; }
        public string customerCode { get; set; }
        public string CustomerName { get; set; }
        public string invoiceNo { get; set; }
        public decimal invoiceAmount { get; set; }
        public decimal sourceTaxAmount { get; set; }
        public decimal paidAmount { get; set; }
        public decimal revenueStamp { get; set; }
    }
}
