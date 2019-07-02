using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Intuit.Ipp.OAuth2PlatformClient;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Primitives;
using OAuth2_CoreMVC_Sample.Helper;
using OAuth2_CoreMVC_Sample.Models;


// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

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
            if (state.Count>0 && !string.IsNullOrEmpty(code))
            {
                await StoreClaims(state);
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
                        return Challenge(new AuthenticationProperties() { RedirectUri = authorizeUrl });
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
        private async Task StoreClaims(StringValues state)
        {
            oAuth2Client = new OAuth2Client(OAuth2Keys.ClientId, OAuth2Keys.ClientSecret, OAuth2Keys.RedirectUrl, OAuth2Keys.Environment);

            if (state.ToString().Equals(oAuth2Client.CSRFToken, StringComparison.Ordinal))
            {
                ViewBag.State = state + " (valid)";
            }
            else
            {
                ViewBag.State = state + " (invalid)";
            }

            string code = Request.Query["code"].ToString() ?? "none";
            string realmId = Request.Query["realmId"].ToString() ?? "none";
            await GetAuthTokensAsync(code, realmId);

            ViewBag.Error = Request.Query["error"].ToString() ?? "none";
        }

        private async Task GetAuthTokensAsync(string code, string realmId)
            {
                if (realmId != null)
                {
                    TempData["realmId"] = realmId;
                }

                var tokenResponse = await oAuth2Client.GetBearerTokenAsync(code);


                OAuth2Keys.RealmId = realmId;
                var claims = new List<Claim>();

                if (TempData["realmId"] != null)
                {
                    claims.Add(new Claim("realmId", TempData["realmId"].ToString()));
                }

                if (!string.IsNullOrWhiteSpace(tokenResponse.AccessToken))
                {
                    claims.Add(new Claim("access_token", tokenResponse.AccessToken));
                    claims.Add(new Claim("access_token_expires_at", (DateTime.Now.AddSeconds(tokenResponse.AccessTokenExpiresIn)).ToString()));
                }

                if (!string.IsNullOrWhiteSpace(tokenResponse.RefreshToken))
                {
                    claims.Add(new Claim("refresh_token", tokenResponse.RefreshToken));
                    claims.Add(new Claim("refresh_token_expires_at", (DateTime.Now.AddSeconds(tokenResponse.RefreshTokenExpiresIn)).ToString()));
                }
                var id = new ClaimsIdentity(claims, "Cookies");
              Token token = _tokens.Token.FirstOrDefault(t => t.RealmId == realmId);
                if (token == null)
                {
                    _tokens.Add(new Token { RealmId = realmId, AccessToken = tokenResponse.AccessToken, RefreshToken = tokenResponse.RefreshToken });
                   await  _tokens.SaveChangesAsync();
                }
            }
     
    }
}
