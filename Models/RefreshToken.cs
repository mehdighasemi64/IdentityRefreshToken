namespace IdentityRefreshToken.Models
{
    public class RefreshToken
    {
        public int Id { get; set; }
        public string UserId { get; set; }  // Assuming it's a string (modify as needed)
        public string Token { get; set; }
        public DateTime ExpiryDate { get; set; }
    }

}

