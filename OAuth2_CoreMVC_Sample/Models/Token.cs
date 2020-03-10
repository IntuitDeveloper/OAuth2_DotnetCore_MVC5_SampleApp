namespace OAuth2_CoreMVC_Sample.Models
{
    public class Token
    {
        public string RealmId { get; set; }
        public string AccessToken { get; set; }
        public string RefreshToken { get; set; }
    }
}