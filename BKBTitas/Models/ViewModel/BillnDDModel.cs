using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace BKBTitas.Models.ViewModel
{
    public class BillViewModel
    {
        [Key]
        public int ID { get; set; }
        [Display(Name = "Payment Id")]
        [JsonProperty("paymentId")]
        public int paymentId { get; set; }
        /*Non-Parameter Parameter*/
        

        [JsonProperty("id")]
        public int payId { get; set; }

        [Display(Name = "Customer Mobile")] //non-meter
        [JsonProperty("mobile")]
        public string mobile { get; set; }

        [Display(Name = "Bank Transaction Id")] //non-meter
        [JsonProperty("bankTransactionId")]
        public string bankTransactionId { get; set; }

        [Display(Name = "Batch No")] //non-meter 
        [JsonProperty("batchNo")]
        public string batchNo { get; set; }

        [Display(Name = "Customer Address")]
        [JsonProperty("connectionAddress")]
        public string connectionAddress { get; set; }

        [Display(Name = "Amount")] //non-meter amount connectionAddress
        [JsonProperty("amount")]
        public decimal amount { get; set; }

        [Display(Name = "Surcharge")]
        [JsonProperty("surcharge")]
        public decimal surcharge { get; set; }
        [Display(Name = "Total")]
        [JsonProperty("total")]
        public decimal total { get; set; }

        [Display(Name = "Payment Amount")]
        [JsonProperty("paymentAmount")]
        public decimal paymentAmount { get; set; }

        [JsonProperty("particulars")]
        public string particulars { get; set; }
        [Display(Name = "Voucher Date")]
        [JsonProperty("voucherDate")]
        public string voucherDate { get; set; }

        [Display(Name = "Bank")]
        [JsonProperty("bank")]
        public string bank { get; set; }
        /*Non-Parameter Parameter*/

        [Display(Name = "Transaction Date")]
        [JsonProperty("transactionDate")]
        public string transactionDate { get; set; }
        [Display(Name = "Customer Code")]
        [JsonProperty("customerCode")]
        public string customerCode { get; set; }
        [Display(Name = "Customer ID")]
        [JsonProperty("customerId")]
        public string customerId { get; set; }
        [Display(Name = "Customer Name")]
        [JsonProperty("customerName")]
        public string customerName { get; set; }
        [Display(Name = "Customer Mobile")]
        [JsonProperty("customerMobile")]
        public string customerMobile { get; set; }

        [Display(Name = "Customer Address")]
        [JsonProperty("customerAddress")]
        public string customerAddress { get; set; }
        [Display(Name = "Transaction Status")]
        [JsonProperty("transactionStatus")]
        public string transactionStatus { get; set; }
        [Display(Name = "Bill Period (From)")]
        public System.DateTime dtFrom { get; set; }
        [Display(Name = "To")]
        public System.DateTime dtTo { get; set; }

        [Display(Name = "InvoiceNo")]
        [JsonProperty("invoiceNo")]
        public string invoiceNo { get; set; }

        [Display(Name = "Invoice Date")]
        [JsonProperty("invoiceDate")]
        public System.DateTime invoiceDate { get; set; }

        [Display(Name = "Issue Month")]
        [JsonProperty("issueMonth")]
        public string issueMonth { get; set; }
        [Display(Name = "Bill Amount")]
        [JsonProperty("invoiceAmount")]
        public Decimal invoiceAmount { get; set; }

        [Display(Name = "Issue Date")]
        [JsonProperty("issueDate")]
        public string issueDate { get; set; }

        [Display(Name = "Total Bill Amount")]
        [JsonProperty("paidAmount")]
        public Decimal paidAmount { get; set; }
        [Display(Name = "Surcharge")]
        [JsonProperty("surchargeAmount")]
        public string surchargeAmount { get; set; }

        [Display(Name = "Source Tax Amount")]
        [JsonProperty("sourceTaxAmount")]
        public decimal sourceTaxAmount { get; set; }
        [Display(Name = "Revenue Stamp")]
        [JsonProperty("revenueStamp")]
        public Decimal revenueStamp { get; set; }
        [Display(Name = "Confirm Total Amount")]
        public Decimal totalAmount { get; set; }
        [Display(Name = "Zone")]
        [JsonProperty("zone")]
        public string zone { get; set; }
        [Display(Name = "Creator")]
        [JsonProperty("creator")]
        public string creator { get; set; }
        [Display(Name = "Creation Time")]
        [JsonProperty("creationTime")]
        public string creationTime { get; set; }
        [Display(Name = "Posting Time")]
        [JsonProperty("postingTime")]
        public string postingTime { get; set; }
        [Display(Name = "Bank Code")]
        [JsonProperty("bankCode")]
        public string bankCode { get; set; }
        [Display(Name = "Branch Code")]
        [JsonProperty("branchCode")]
        public string branchCode { get; set; }
        [Display(Name = "Branch RoutingNo")]
        [JsonProperty("branchRoutingNo")]
        public string branchRoutingNo { get; set; }
        [Display(Name = "Operator")]
        [JsonProperty("operator")]
        public string operator_ { get; set; }
        [Display(Name = "Ref No")]
        [JsonProperty("refNo")]
        public string refNo { get; set; }
        [Display(Name = "Reason")]
        [JsonProperty("reason")]
        public string reason { get; set; }
        [Display(Name = "Cancel Operator")]
        [JsonProperty("cancelOperator")]
        public string cancelOperator { get; set; }
        [Display(Name = "Chalan No")]
        [JsonProperty("chalanNo")]
        public string chalanNo { get; set; }
        [Display(Name = "chalan Date")]
        [JsonProperty("chalanDate")]
        public string chalanDate { get; set; }
        [Display(Name = "Chalan Bank")]
        [JsonProperty("chalanBank")]
        public string chalanBank { get; set; }
        [Display(Name = "Chalan Branch")]
        [JsonProperty("chalanBranch")]
        public string chalanBranch { get; set; }

        [Display(Name = "Application Summary")]
        [JsonProperty("applianceSummary")]
        public string applicationSummary { get; set; }

        [Display(Name = "Settale Date")]
        [JsonProperty("settaleDate")]
        public string settaleDate { get; set; }

        [Display(Name = "Due Date")]
        [JsonProperty("dueDate")]
        public string dueDate { get; set; }

        [Display(Name = "Remarks")]
        public string remarks { get; set; }
        [Display(Name = "Status")]
        [JsonProperty("status")]
        public string status { get; set; }
        [Display(Name = "Message")]
        [JsonProperty("message")]
        public string message { get; set; }
       
    }

    public class ErrorViewModel
    {
        [Display(Name = "Status")]
        public string status { get; set; }
        [Display(Name = "Message")]
        public string message { get; set; }
    }

    public class SuccessViewModel
    {
        [Display(Name = "Status")]
        [JsonProperty("status")]
        public string status { get; set; }
        [Display(Name = "Message")]
        [JsonProperty("message")]
        public string message { get; set; }

        public BillViewModel data { get; set; } 
    }

    public class TxnDataViewModel 
    {
        [Display(Name = "Status")]
        [JsonProperty("status")]
        public string status { get; set; }
        [Display(Name = "Message")]
        [JsonProperty("message")]
        public string message { get; set; }

        [JsonProperty("data")]
        public BillViewModel data { get; set; }
    }

    public class RptDataViewModel
    {
        [Display(Name = "Status")]
        [JsonProperty("status")]
        public string status { get; set; }
        [Display(Name = "Message")]
        [JsonProperty("message")]
        public string message { get; set; }

        [JsonProperty("data")]
        public RptTransaction data { get; set; }
    }


    public class RptDataViewModelNonM
    {
        [Display(Name = "Status")]
        [JsonProperty("status")]
        public string status { get; set; }
        [Display(Name = "Message")]
        [JsonProperty("message")]
        public string message { get; set; }

        [JsonProperty("data")]
        public List<BillViewModel> data { get; set; }
    }
    public class RptTransaction
    {
        [JsonProperty("transactions")]
        public List<BillViewModel> transactions { get; set; }
    }
}