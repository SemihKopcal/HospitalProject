using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApplicationHospital.Models
{
    public class Appointment
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int ScheduleId { get; set; } // Foreign Key

        [Required]
        public string UserId { get; set; } // Assistant veya Kullanıcı Id'si

        // Navigation Properties
        [ForeignKey("ScheduleId")]
        public Schedule Schedule { get; set; }

        [ForeignKey("UserId")]
        public ApplicationUser User { get; set; }
    }
}