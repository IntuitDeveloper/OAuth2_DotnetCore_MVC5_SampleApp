using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Intuit.Ipp.OAuth2PlatformClient;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using OAuth2_CoreMVC_Sample.Helper;
using OAuth2_CoreMVC_Sample.Models;

namespace OAuth2_CoreMVC_Sample.Controllers
{
    public class ConnectController : Controller
    {
        private readonly TokensContext _tokens;
        private readonly OAuth2Keys _auth2Keys;
        public OAuth2Client oAuth2Client;
        public ConnectController(TokensContext tokens, IOptions<OAuth2Keys> auth2Keys)
        {
            _tokens = tokens;
            _auth2Keys = auth2Keys.Value;
        }

        public ActionResult Home()
        {
            return View("Connect");
        }

        // GET: /<controller>/
        public async Task<ActionResult> Index()
        {
            var state = Request.Query["state"];
            var code = Request.Query["code"].ToString();
            var realmId = Request.Query["realmId"].ToString();


            if (state.Count > 0 && !string.IsNullOrEmpty(code))
            {
                await GetAuthTokensAsync(code, realmId);
                return RedirectToAction("Index", "QBO");
            }

            return RedirectToAction("Home", "Connect");
        }

        [HttpGet]
        public IActionResult Login(string connect)
        {
            if (!string.IsNullOrEmpty(_auth2Keys.ClientId) && !string.IsNullOrEmpty(_auth2Keys.ClientSecret))
            {
                oAuth2Client = new OAuth2Client(_auth2Keys.ClientId, _auth2Keys.ClientSecret, _auth2Keys.RedirectUrl,
                    _auth2Keys.Environment);

                switch (connect)
                {
                    case "Connect to QuickBooks":
                        var scopes = new List<OidcScopes>();
                        scopes.Add(OidcScopes.Accounting);
                        var authorizeUrl = oAuth2Client.GetAuthorizationURL(scopes);
                        OAuth2Keys.AuthURL = authorizeUrl;
                        return Redirect(authorizeUrl);
                    default:
                        return View("Connect");
                }
            }

            ViewData["Configuration"] = "NullValue";
            return View("Connect");
        }

        private async Task GetAuthTokensAsync(string code, string realmId)
        {
            oAuth2Client = new OAuth2Client(_auth2Keys.ClientId, _auth2Keys.ClientSecret, _auth2Keys.RedirectUrl,
                _auth2Keys.Environment);
            var tokenResponse = await oAuth2Client.GetBearerTokenAsync(code);
            _auth2Keys.RealmId = realmId;
            var token = _tokens.Token.FirstOrDefault(t => t.RealmId == realmId);
            if (token == null)
            {
                _tokens.Add(new Token
                {
                    RealmId = realmId, AccessToken = tokenResponse.AccessToken,
                    RefreshToken = tokenResponse.RefreshToken
                });
                await _tokens.SaveChangesAsync();
            }
        }
    }
}