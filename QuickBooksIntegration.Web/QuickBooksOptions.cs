namespace QuickBooksIntegration.Web
{
    public class QuickBooksOptions
    {
        public const string SectionName = "QuickBooks";

        public string ClientId { get; set; }
        public string ClientSecret { get; set; }
        public string RedirectUri { get; set; }
        public string Environment { get; set; }
    }
}
