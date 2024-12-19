using System;
using System.ComponentModel.DataAnnotations;

namespace WebApplicationHospital.Models
{
    public class EmergencyNews
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(200)]
        public string Title { get; set; }

        [Required]
        public string Content { get; set; }

        [Required]
        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}