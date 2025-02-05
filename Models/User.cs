using System.ComponentModel.DataAnnotations;

namespace IdentityRefreshToken.Models
{
    public class User
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string Username { get; set; }

        [Required]
        public string PasswordHash { get; set; } // Store hashed password

        public string RefreshToken { get; set; } // Store refresh token

        public DateTime? RefreshTokenExpiryTime { get; set; } // Expiry date of refresh token
    }
}




