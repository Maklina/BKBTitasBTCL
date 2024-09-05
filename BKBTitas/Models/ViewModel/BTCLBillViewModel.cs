using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;
using System.Web.Mvc;
namespace BKBTitas.Models.ViewModel
{
    public class BTCLBillRequestModel
    {
        public int id { get; set; }

        public string ExchangeCode { get; set; }
        public string PhoneNumber { get; set; }
        public string AccountNumber { get; set; }
        public string BillNumber { get; set; }
        public string Paystatus { get; set; }
        public string LastPayDate { get; set; }
        public string SearchType { get; set; }
        public int TxnNumber { get; set; }
        public string ResponseCode { get; set; }
        public string BankCode { get; set; }
        public string BranchCode { get; set; }
        public string CancelReason { get; set; }
        public string ResponseMessage { get; set; }
        public decimal ConfirmAmount { get; set; }
        public List<BTCLBillDetailsModel> Details { get; set; }
    }

    public class BTCLBillDetailsModel
    {
        [Key]
        public int id { get; set; }

        public bool isChecked { get; set; }
        public string RowNumber { get; set; }
        public string ExchangeCode { get; set; }
        public string TxnNumber { get; set; }
        public string PhoneNumber { get; set; }
        public string AccountNumber { get; set; }
        public string BillNo { get; set; }
        public string BillMonth { get; set; }
        public string BillYear { get; set; }
        public string BillMonthYear { get { return BillMonth + "-" + BillYear; } }
        public decimal BTCLAmount { get; set; }
        public decimal OverdueAmount { get; set; }
        public decimal VAT { get; set; }
        public decimal TotalAmount { get { return BTCLAmount + OverdueAmount + VAT; } }
        public decimal PaidAmount { get; set; }
        public string Name { get; set; }
        public string BillPayStatus { get; set; }
        public string ServiceNumber { get; set; }
        public DateTime? CollectionDate { get; set; }
        public string Fromdate { get; set; }
        public string ToDate { get; set; }
        public string BranchCode { get; set; }
        public string BtclBranchCode { get; set; }
        public string CancelReason { get; set; }
        public decimal ConfirmAmount { get; set; }
       
    }
}