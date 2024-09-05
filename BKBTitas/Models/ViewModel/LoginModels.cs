using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace BKBTitas.Models.ViewModel
{
    public class LoginModels
    {
        [Required(ErrorMessage = "Please enter the User Name")]
        [Display(Name = "User ID")]
        public string UserId { get; set; }

        [Display(Name = "User Name")]
        public string userName { get; set; }

        [Required(ErrorMessage = "Please enter the Password")]
        [Display(Name = "Password")]
        [DataType(DataType.Password)]
        public string Password { get; set; }
        [Display(Name = "Branch Name")]
        public string BranchCode { get; set; }

        [Display(Name = "User Status")]
        public string UserStatus { get; set; }
        [Display(Name = "User Mobile")]
        public string UserMobile { get; set; }

        public string role_priority { get; set; }

        public int? UserRoleId { get; set; }
        public string RoleName { get; set; }
        public string userIPAddress { get; set; }

        public string agentID { get; set; }
        public string agentSequence { get; set; }

        public string token { get; set; }
    }
    public class NoticeBoardViewModel
    {
        public string id { get; set; }

        [Required(ErrorMessage = "Please enter the Title")]
        [Display(Name = "Title")]
        public string title { get; set; }

        [Required(ErrorMessage = "Please enter the Description")]
        [Display(Name = "Description")]
        public string details { get; set; }

        [Required(ErrorMessage = "Please enter the Effective Date")]
        [Display(Name = "Effective Date")]
        public DateTime effective_date { get; set; }

        [Required(ErrorMessage = "Please enter the End Date")]
        [Display(Name = "End Date")]
        public string end_date { get; set; }
    }
}