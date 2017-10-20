using System.ComponentModel.DataAnnotations;
namespace connectingToDBTESTING.Models
{
    public class Guest : BaseEntity
    {
        [Key]
        public int GuestId { get; set; }
        public int ActivityId { get; set; }
        public Activity Activity {get; set; }
        public int UserId { get; set; }
        public User User{ get; set; }

    }
}