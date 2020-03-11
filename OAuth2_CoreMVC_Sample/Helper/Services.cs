using System;
using System.IO;
using System.Threading.Tasks;
using Intuit.Ipp.Core;
using Intuit.Ipp.Core.Configuration;
using Intuit.Ipp.Exception;
using Intuit.Ipp.OAuth2PlatformClient;
using Intuit.Ipp.Security;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using OAuth2_CoreMVC_Sample.Models;

namespace OAuth2_CoreMVC_Sample.Helper
{
    public class Services : IServices
    {
        private readonly TokensContext _tokens;
        private readonly OAuth2Keys _auth2Keys;
        public Services(TokensContext tokens, IOptions<OAuth2Keys> auth2Keys)
        {
            _tokens = tokens;
            _auth2Keys = auth2Keys.Value;
        }

        /// <summary>
        ///     Test QBO api call
        /// </summary>
        /// <param name="apiCallFunction"></param>
        public async Task QBOApiCall(Action<ServiceContext> apiCallFunction)
        {
            var oauthClient = new OAuth2Client(
                _auth2Keys.ClientId, 
                _auth2Keys.ClientSecret, 
                _auth2Keys.RedirectUrl,
                _auth2Keys.Environment);

            var token = await _tokens.Token.FirstOrDefaultAsync(t => t.RealmId == _auth2Keys.RealmId);
            
            try
            {
                if (_auth2Keys.RealmId != "")
                    if (token.AccessToken != null && token.RealmId != null)
                    {
                        var reqValidator = new OAuth2RequestValidator(token.AccessToken);
                        var configurationProvider =
                            new JsonFileConfigurationProvider(Directory.GetCurrentDirectory() + "\\appsettings.json");
                        var context = new ServiceContext(token.RealmId, IntuitServicesType.QBO, reqValidator,
                            configurationProvider);
                        context.IppConfiguration.BaseUrl.Qbo = _auth2Keys.QBOBaseUrl;
                        apiCallFunction(context);
                    }
            }
            catch (IdsException ex)
            {
                if (ex.Message == "Unauthorized-401")
                {
                    var tokens = await oauthClient.RefreshTokenAsync(token.RefreshToken);
                    if (tokens.AccessToken != null && tokens.RefreshToken != null)
                    {
                        await UpdateTokens(token.RealmId, tokens.AccessToken, tokens.RefreshToken);
                        await QBOApiCall(apiCallFunction);
                    }
                }
            }
        }


        public async Task<Token> UpdateTokens(string realmId, string newAccessToken, string newRefreshToken)
        {
            var token = await _tokens.Token.FirstOrDefaultAsync(t => t.RealmId == realmId);
            if (token != null)
            {
                token.AccessToken = newAccessToken;
                token.RefreshToken = newRefreshToken;
            }

            _tokens.SaveChanges();
            return token;
        }
    }
}