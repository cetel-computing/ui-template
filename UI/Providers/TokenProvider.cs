namespace FlazorTemplate.Providers
{
    public class TokenProvider
    {
        public string? AccessToken { get; set; }

        public string? RefreshToken { get; set; }

        public string? InitialCustomer { get; set; }

        public string? CustomerFilter { get; set; }
    }
}
