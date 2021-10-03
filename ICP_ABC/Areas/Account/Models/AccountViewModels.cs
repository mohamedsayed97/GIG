using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ICP_ABC.Areas.Account.Models
{
    public class ExternalLoginConfirmationViewModel
    {

        //[Required]
        //[Display(Name = "Name")]
        //public string Email { get; set; }

        [Required]
        [StringLength(50)]
        [Display(Name = "Name")]
        public string UserName { get; set; }
    }


    public class ExternalLoginListViewModel
    {
        public string ReturnUrl { get; set; }
    }

    public class SendCodeViewModel
    {
        public string SelectedProvider { get; set; }
        public ICollection<System.Web.Mvc.SelectListItem> Providers { get; set; }
        public string ReturnUrl { get; set; }
        public bool RememberMe { get; set; }
    }

    public class VerifyCodeViewModel
    {
        [Required]
        public string Provider { get; set; }

        [Required]
        [Display(Name = "Code")]
        public string Code { get; set; }
        public string ReturnUrl { get; set; }

        [Display(Name = "Remember this browser?")]
        public bool RememberBrowser { get; set; }

        public bool RememberMe { get; set; }
    }

    public class ForgotViewModel
    {
        [Required]
        [Display(Name = "Email")]
        public string Email { get; set; }
    }

    public class LoginViewModel
    {
        //[Required]
        //[Display(Name = "Email")]
        //[EmailAddress]
        //public string Email { get; set; }

        [Required]
        [StringLength(50)]
        [Display(Name = "User Name")]
        public string UserName { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [Display(Name = "Remember me?")]
        public bool RememberMe { get; set; }
    }

    public class EditViewModel
    {
        [Required]
        [Display(Name = "Code")]
        public string Code { get; set; }

        public bool UnBlockRight { get; set; }

        [Required]
        [StringLength(50)]
        [Display(Name = "User Name")]
        public string UserName { get; set; }

        [Required]
        [StringLength(50)]
        [EmailAddress]
        [Display(Name = "Email Address")]
        public string Email { get; set; }

        [Required]
        [MaxLength(100)]
        public string FullName { get; set; }

        [Required]
        [StringLength(50)]
        [Display(Name = "OldName")]
        public string OldUserName { get; set; }

        public bool BranchRight { get; set; }

        [Required]
        public int BranchID { get; set; }

        //public List<Branch> Branches { get; set; }
        public string ExpireDate { get; set; }

        [Required]
        public int GroupID { get; set; }

   

        public string CloseDueDate { get; set; }

        public int PositionID { get; set; }


        

        [Required]
        [StringLength(10, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 8)]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }
    }

    public class RegisterViewModel
    {
        [Required]
        [Display(Name = "Code")]
        public string Code { get; set; }

        [Required]
        [StringLength(50)]
        [Display(Name = "Name")]
        public string UserName { get; set; }

        [Required]
        [MaxLength(100)]
        public string FullName { get; set; }

        [Required]
        [StringLength(50)]
        [EmailAddress]
        [Display(Name = "Email Address")]
        public string Email { get; set; }
        [Required]
        public bool BranchRight { get; set; }
        [Required]
        public bool UnBlockRight { get; set; }

        [Required]
        public int BranchID { get; set; }



        //public List<Branch> Branches { get; set; }

        [Required]
        public int GroupID { get; set; }

        //public List<UserGroup> userGroups { get; set; }
        [Required]
        public int PositionID { get; set; }

        [Required]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime ExpireDate { get; set; }

        //[Required]
        [Required]
        [RegularExpression("^(?=.*[a-z])(?=.*[A-Z])(?=.*\\d)(?=.*[-+_!@#$%^&*.,?]).{6,10}$", ErrorMessage = "Password must be between 6 and 10 characters and contain one uppercase letter, one lowercase letter, one digit and one special character.")]

        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }
        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }

        [Required]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime CloseDueDate { get; set; }
    }

    public class ChangeExpiredPasswordViewModel
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(50)]
        [Display(Name = "User Name")]
        public string UserName { get; set; }

        [Required]
        [RegularExpression("^(?=.*[a-z])(?=.*[A-Z])(?=.*\\d)(?=.*[-+_!@#$%^&*.,?]).{6,10}$", ErrorMessage = "Password must be between 6 and 10 characters and contain one uppercase letter, one lowercase letter, one digit and one special character.")]
        //[StringLength(10, ErrorMessage = "Max Length must be {10} and min {6} .", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "OldPassword")]
        public string OldPassword { get; set; }

        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime CloseDueDate { get; set; }

        [Required]
        //[StringLength(10, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 8)]
        [RegularExpression("^(?=.*[a-z])(?=.*[A-Z])(?=.*\\d)(?=.*[-+_!@#$%^&*.,?]).{6,10}$", ErrorMessage = "Password must be between 6 and 10 characters and contain one uppercase letter, one lowercase letter, one digit and one special character.")]
      
        [DataType(DataType.Password)]
        [Display(Name = "New Password")]
        public string NewPassword { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm New password")]
        [Compare("NewPassword", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmNewPassword { get; set; }
    }

    public class DetailsViewModel
    {
        [Required]
        [Display(Name = "Code")]
        public string Code { get; set; }

        public bool UnBlockRight { get; set; }

        [Required]
        [StringLength(50)]
        [Display(Name = "User Name")]
        public string UserName { get; set; }

        [Required]
        [MaxLength(100)]
        public string FullName { get; set; }

        [Required]
        [StringLength(50)]
        [EmailAddress]
        [Display(Name = "Email Address")]
        public string Email { get; set; }

        //[Required]
        //[StringLength(50)]
        //[EmailAddress]
        //[Display(Name = "Name")]
        //public string Email { get; set; }

        public bool BranchRight { get; set; }

        [Required]
        public int BranchID { get; set; }

        //public List<Branch> Branches { get; set; }
        public bool Auth { get; set; }
        public bool Check { get; set; }
        [Required]
        public int GroupID { get; set; }

        //public List<UserGroup> userGroups { get; set; }

        public int PositionID { get; set; }

        public DateTime SysDate { get; set; }

        public string Maker { get; set; }

        public string LastEditor { get; set; }

        [Required]
        public DateTime ExpireDate { get; set; }



        [Required]
        [StringLength(10, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 8)]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [StringLength(10, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 8)]
        public string ConfirmPassword { get; set; }

        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime CloseDueDate { get; set; }
        //-----------------------------------------------

        public bool AuthForEditAndDelete { get; set; }
    }

    public class ResetPasswordViewModel
    {
        [Required]
        [StringLength(50)]
        [Display(Name = "User Name")]
        public string UserName { get; set; }

        [Required]
        [StringLength(10, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 8)]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }

        public string Code { get; set; }
    }

    public class ForgotPasswordViewModel
    {
        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; }
    }
}
