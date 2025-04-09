using System;
using System.ComponentModel.DataAnnotations;

namespace DRF_WPF.Models
{
    public class User
    {
        [Key]
        public int Id { get; set; }
        
        [Required]
        [MaxLength(50)]
        public string Username { get; set; } = string.Empty;
        
        [Required]
        [MaxLength(100)]
        public string Password { get; set; } = string.Empty;
        
        public string? DisplayName { get; set; }
        
        public bool IsAdmin { get; set; }
        
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        
        public DateTime? LastLoginTime { get; set; }
    }
} 