using System;
using System.Collections.Generic;

namespace OAuth2_CoreMVC_Sample.Models.DB
{
    public partial class Token
    {
        public string RealmId { get; set; }
        public string AccessToken { get; set; }
        public string RefreshToken { get; set; }
    }
}
