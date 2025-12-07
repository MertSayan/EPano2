using System.ComponentModel.DataAnnotations;

namespace EPano2.Models
{
    public class AdminUser
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(64)]
        public string Username { get; set; } = string.Empty;

        // BCrypt veya benzeri ile hashlenmiş şifre saklanacak
        [Required]
        [MaxLength(256)]
        public string PasswordHash { get; set; } = string.Empty;

        [MaxLength(32)]
        public string Role { get; set; } = "Admin";

        public bool IsActive { get; set; } = true;
    }
}


