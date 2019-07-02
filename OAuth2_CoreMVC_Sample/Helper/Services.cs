using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Intuit.Ipp.Core;
using Intuit.Ipp.Data;
using Intuit.Ipp.Exception;
using Intuit.Ipp.OAuth2PlatformClient;
using Intuit.Ipp.Security;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using OAuth2_CoreMVC_Sample.Controllers;
using OAuth2_CoreMVC_Sample.Models;

namespace OAuth2_CoreMVC_Sample.Helper
{
    public class Services : IServices
    {
        private readonly TokensContext _tokens;
        public Services(TokensContext tokens)
        {
            _tokens = tokens;
        }


     public async Task<Token> UpdateTokens(string realmId, string newAccessToken, string newRefreshToken)
        {
            Token token = await _tokens.Token.FirstOrDefaultAsync(t => t.RealmId == realmId);
            if (token != null)
            {
                token.AccessToken = newAccessToken;
                token.RefreshToken = newRefreshToken;
            }
            _tokens.SaveChanges();
           return token;
        }

        /// <summary>
        /// Test QBO api call
        /// </summary>
        /// <param name="access_token"></param>
        /// <param name="refresh_token"></param>
        /// <param name="realmId"></param>
        /// <param name="apiCallFunction"></param>
        public async System.Threading.Tasks.Task QBOApiCall(Action<ServiceContext> apiCallFunction,string value=null)
        {
            OAuth2Client oauthClient = new OAuth2Client(OAuth2Keys.ClientId, OAuth2Keys.ClientSecret, OAuth2Keys.RedirectUrl, OAuth2Keys.Environment);

            Token token = await _tokens.Token.FirstOrDefaultAsync(t => t.RealmId == OAuth2Keys.RealmId);
            try
            {
                if (OAuth2Keys.RealmId != "")
                {
                    if (token.AccessToken != null && token.RealmId != null)
                    {
                        OAuth2RequestValidator reqValidator = new OAuth2RequestValidator(token.AccessToken);
                        ServiceContext context = new ServiceContext(token.RealmId, IntuitServicesType.QBO, reqValidator);
                        context.IppConfiguration.MinorVersion.Qbo = "38";
                        apiCallFunction(context);
                    }
                  
                }
            }
            catch (IdsException ex)
            {
                if (ex.Message == "Unauthorized-401")
                {
                    var tokens = await oauthClient.RefreshTokenAsync(token.RefreshToken);
                    if (tokens.AccessToken != null && tokens.RefreshToken != null)
                    {
                      await  UpdateTokens(token.RealmId, tokens.AccessToken, tokens.RefreshToken);
                        await QBOApiCall(apiCallFunction);
                    }
                   
                }
               
            }
        
        }



    }
}
