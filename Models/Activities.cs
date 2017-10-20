using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace connectingToDBTESTING.Models
{
    public class Activity: BaseEntity
    {   
        public Activity()
        {
            Guests = new List<Guest>();
        }
        public class MyDateAttribute : ValidationAttribute
        {
            public override bool IsValid(object value)
            {
                DateTime d = Convert.ToDateTime(value);
                return d >= DateTime.Now;

            }
        }
        public List<Guest> Guests { get; set; }
        [Key]
        public int ActivityId { get; set; }
        
        [Required(ErrorMessage = "Please enter the activity name.")]
        [StringLength(225, ErrorMessage = "Activity name must be between 2 and 225 characters", MinimumLength = 2)]
        [Display(Name = "Activity: ")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Please enter date.")]
        [DataType(DataType.Date)]
        [MyDate(ErrorMessage ="Can't set date in past")]
        public DateTime? Date { get; set; }
        public int UserId { get; set; }
        public User User {get; set;}
        public int GuestsAmount { get; set; }

        [Required(ErrorMessage = "Please enter duration.")]
        [Display(Name = "Duration:")]
        public string Duration { get; set; }
        
        [Required(ErrorMessage = "Please enter description.")]
        [StringLength(225, ErrorMessage = "Description must be between 10 and 225 characters", MinimumLength = 10)]
        [Display(Name = "Description:")]
        public string Description { get; set; } 
        public DateTime? End { get; set; }
        
    }
    
}
