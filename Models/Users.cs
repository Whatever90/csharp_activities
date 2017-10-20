using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace connectingToDBTESTING.Models
{
    public class User : BaseEntity
    {   
        public User()
        {
            Activities = new List<Activity>();
        }

        [Key]
        public int UserId { get; set; }
        
        [Required(ErrorMessage = "Please enter the user's first name.")]
        [StringLength(16, ErrorMessage = "First name must be between 3 and 16 characters", MinimumLength = 3)]
        [Display(Name = "First Name:")]
        [RegularExpression(@"^([a-zA-Z \.\&\'\-]+)$", ErrorMessage = "Invalid First Name")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Please enter the user's last name.")]
        [StringLength(16, ErrorMessage = "Last Name Must be between 3 and 16 characters", MinimumLength = 3)]
        [Display(Name = "Last Name:")]
        [RegularExpression(@"^([a-zA-Z \.\&\'\-]+)$", ErrorMessage = "Invalid Last Name")]
        public string LastName { get; set; }
        
        [EmailAddress(ErrorMessage = "The Email Address is not valid")]
        [Required(ErrorMessage = "Please enter an email address.")]
        [Display(Name = "Email Address:")]
        [RegularExpression(@"^([\w-\.]+)@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([\w-]+\.)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$", ErrorMessage = "Wrong email format")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Please enter a password.")]
        [DataType(DataType.Password)]
        [RegularExpression(@"^(?=.*[a-zA-Z])(?=.*\d)(?=.*[!@#$%^&*()_+])[A-Za-z\d][A-Za-z\d!@#$%^&*()_+]{7,19}$", ErrorMessage = "Wrong password format. Must contain at least one letter and one number. Should not start with a special character.")]
        [MinLength(8, ErrorMessage = "The Password must be at least 8 characters.")]
        public string Password { get; set; }

        [Required(ErrorMessage = "Please enter a confimation password.")]
        [DataType(DataType.Password)]
        [MinLength(8, ErrorMessage = "The Confirm password must be at least 8 characters.")]
        [Compare("Password", ErrorMessage = "Password andx Confirmation don't match")]
        public string ConPassword { get; set; }
        
        public List<Activity> Activities { get; set; }

        
    }
}