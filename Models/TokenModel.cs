using System.ComponentModel.DataAnnotations;

namespace IdentityRefreshToken.Models
{
    public class TokenModel
    {
        [Required]
        public string AccessToken { get; set; }

        [Required]
        public string RefreshToken { get; set; } 
        
        [Required]
        public string ClientType { get; set; }
    }

}
