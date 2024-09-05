using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace BKBTitas.Models.ViewModel
{
    public class BranchViewModel
    {
        public int id { get; set; }
        public string branch_code { get; set; }
        public string branch_name { get; set; }
        public string region { get; set; }
        public string BBCode { get; set; }
        public string RoutingNo { get; set; }
        public string agentID { get; set; }
        public string agentSequence { get; set; }
        public string token { get; set; }
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string Address3 { get; set; }
        public string City { get; set; }
        public string MainOffice { get; set; }
        public string Status { get; set; }
        public string StoreNum { get; set; }
        public string ISO { get; set; }
        public string ZIP { get; set; }
        public string POSStatus { get; set; }
        public string UnitProfileID { get; set; }
        public string AgentPIN { get; set; }
        public string RAMS { get; set; }
        public string Partyno { get; set; }
        public string HQPartyno { get; set; }
        public string UnitOffice { get; set; }
        public string Legacy { get; set; }
    }

    public class PasswordChangeModel
    {
        [Display(Name = "Old Passwor")]
        [DataType(DataType.Password)]
        public string OldPassword { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "New password")]
        public string NewPassword { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm new password")]
        [System.ComponentModel.DataAnnotations.Compare("NewPassword", ErrorMessage = "The new password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }
    }
    public class PasswordResetModel
    {
        [Required(ErrorMessage = "Required")]
        [Display(Name = "User ID")]
        public string userID { get; set; }
        public IEnumerable<SelectListItem> userList { get; set; }

        [Required]
        [StringLength(100)]
        [DataType(DataType.Password)]
        [Display(Name = "New password")]
        public string NewPassword { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm new password")]
        [System.ComponentModel.DataAnnotations.Compare("NewPassword", ErrorMessage = "The new password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }
    }
    public class RoleModel
    {
        public int id { get; set; }
        public string role_name { get; set; }
    }
    public class UserSignUpModel
    {
        [Required(ErrorMessage = "Required")]
        [Display(Name = "User ID")]
        public string userID { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string userPass { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm Password")]
        [System.ComponentModel.DataAnnotations.Compare("userPass", ErrorMessage = "The new password and confirmation password do not match.")]
        public string confirmPass { get; set; }

        [Required(ErrorMessage = "Required")]
        [Display(Name = "Name")]
        public string userName { get; set; }

        [Required(ErrorMessage = "Required")]
        [Display(Name = "User Role")]
        public int userRole { get; set; }
        public IEnumerable<SelectListItem> roleList { get; set; }

        public string roleName { get; set; }

        [Required(ErrorMessage = "Required")]
        [Display(Name = "Branch/HO")]
        public string branch_code { get; set; }
        public IEnumerable<SelectListItem> BranchList { get; set; }

        public string branchName { get; set; }

        [Required(ErrorMessage = "Required")]
        [Display(Name = "User Status")]
        public string user_status { get; set; }
        public string first_login { get; set; }
        public string UserMobile { get; set; }
    }
}