using Intuit.Ipp.Core;
using Intuit.Ipp.Data;
using Intuit.Ipp.DataService;
using Intuit.Ipp.OAuth2PlatformClient;
using Intuit.Ipp.QueryFilter;
using Intuit.Ipp.Security;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Options;

namespace QuickBooksIntegration.Web.Pages
{
    public class CategoryModel : PageModel
    {
        private readonly ILogger<CategoryModel> _logger;
        private readonly QuickBooksOptions _quickBooksOptions;

        public CategoryModel(ILogger<CategoryModel> logger, IOptions<QuickBooksOptions> quickBooksOptions)
        {
            _logger = logger;
            _quickBooksOptions = quickBooksOptions.Value;
        }

        public List<string> Categories { get; set; }

        public async Task<IActionResult> OnGet([FromQuery] string code, [FromQuery] string realmId)
        {
            OAuth2Client oauthClient = new OAuth2Client(
                _quickBooksOptions.ClientId,
                _quickBooksOptions.ClientSecret,
                _quickBooksOptions.RedirectUri,
                _quickBooksOptions.Environment);

            if (string.IsNullOrWhiteSpace(code))
            {
                List<OidcScopes> scopes = new List<OidcScopes>();
                scopes.Add(OidcScopes.Accounting);

                string authorizeUrl = oauthClient.GetAuthorizationURL(scopes);
                return Redirect(authorizeUrl);
            }

            var tokenResponse = await oauthClient.GetBearerTokenAsync(code);

            OAuth2RequestValidator oauthValidator = new OAuth2RequestValidator(tokenResponse.AccessToken);
            ServiceContext serviceContext = new ServiceContext(realmId, IntuitServicesType.QBO, oauthValidator);
            serviceContext.IppConfiguration.BaseUrl.Qbo = "https://sandbox-quickbooks.api.intuit.com/";
            DataService dataService = new DataService(serviceContext);
            QueryService<Account> queryService = new QueryService<Account>(serviceContext);
            List<Account> categories = queryService.ExecuteIdsQuery("SELECT * FROM Account").ToList();

            Categories = categories.Select(p => p.Name).ToList();

            return Page();
        }
    }
}
