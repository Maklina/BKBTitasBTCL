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
    
    public partial class BranchZoneNonMeterSummary_Result
    {
        public Nullable<long> SL { get; set; }
        public string branch_name { get; set; }
        public string zone { get; set; }
        public string RoutingNo { get; set; }
        public Nullable<int> NoOfTransaction { get; set; }
        public Nullable<decimal> BillAmount { get; set; }
        public Nullable<decimal> SurCharge { get; set; }
        public Nullable<decimal> TotalCollection { get; set; }
        public Nullable<decimal> TotalRevenueStamp { get; set; }
    }
}
