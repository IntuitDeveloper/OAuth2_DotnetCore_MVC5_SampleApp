using Intuit.Ipp.OAuth2PlatformClient;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using OAuth2_CoreMVC_Sample.Helper;
using OAuth2_CoreMVC_Sample.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OAuth2_CoreMVC_Sample.Controllers
{
    public class ConnectController : Controller
    {
        private readonly OAuth2Keys _oAuth2Keys;
        private readonly IServices _services;
        private readonly TokensContext _tokens;
        public OAuth2Client oAuth2Client;

        public ConnectController(IOptions<OAuth2Keys> oAuth2Keys, IServices services, TokensContext tokens)
        {
            _oAuth2Keys = oAuth2Keys.Value;
            _services = services;
            _tokens = tokens;

        }
        public ActionResult Home()
        {
            return View("Connect");
        }

        // GET: /<controller>/
        public async Task<ActionResult> Index()
        {

            var state = Request.Query["state"];
            string code = Request.Query["code"].ToString();
            string realmId = Request.Query["realmId"].ToString();
           

            if (state.Count>0 && !string.IsNullOrEmpty(code))
            {
                await GetAuthTokensAsync(code, realmId);
                return RedirectToAction("Index", "QBO");
            }
            else
                return RedirectToAction("Home","Connect");
        }
        [HttpGet]
        public IActionResult Login(string connect)
        {
            if (!String.IsNullOrEmpty(OAuth2Keys.ClientId) && (!String.IsNullOrEmpty(OAuth2Keys.ClientSecret)))
            {
                oAuth2Client = new OAuth2Client(OAuth2Keys.ClientId, OAuth2Keys.ClientSecret, OAuth2Keys.RedirectUrl, OAuth2Keys.Environment);

                switch (connect)
                {
                    case "Connect to QuickBooks":
                        List<OidcScopes> scopes = new List<OidcScopes>();
                        scopes.Add(OidcScopes.Accounting);
                        string authorizeUrl = oAuth2Client.GetAuthorizationURL(scopes);
                        OAuth2Keys.AuthURL = authorizeUrl;
                        //  return Challenge(new AuthenticationProperties() { RedirectUri = authorizeUrl });
                        return Redirect(authorizeUrl);
                    default:
                        return (View("Connect"));
                }
            }
            else
            {
                ViewData["Configuration"] = "NullValue";
                return (View("Connect"));
            }
        }

        private async Task GetAuthTokensAsync(string code, string realmId)
        {
            oAuth2Client = new OAuth2Client(OAuth2Keys.ClientId, OAuth2Keys.ClientSecret, OAuth2Keys.RedirectUrl, OAuth2Keys.Environment);
            var tokenResponse = await oAuth2Client.GetBearerTokenAsync(code);
            OAuth2Keys.RealmId = realmId;
            Token token = _tokens.Token.FirstOrDefault(t => t.RealmId == realmId);
            if (token == null)
            {
                _tokens.Add(new Token { RealmId = realmId, AccessToken = tokenResponse.AccessToken, RefreshToken = tokenResponse.RefreshToken });
                await _tokens.SaveChangesAsync();
            }
        }
     
    }
}
