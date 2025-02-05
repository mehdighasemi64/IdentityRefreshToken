using System.ComponentModel.DataAnnotations;

namespace IdentityRefreshToken.Models
{
    public class AccessTokenRequest
    {
        [Required]
        public string Username { get; set; }

        [Required]
        public string PasswordHash { get; set; } // Store hashed password
                                                 
        [Required]
        public string ClientType { get; set; } // Web, Mobile, ...
    }
}
