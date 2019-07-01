using System;
using Microsoft.AspNetCore.Identity;

namespace OAuth2_CoreMVC_Sample.Models
{
    public class ApplicationUser : IdentityUser
    {

        public string CompanyId { get; set; }
    }
}
