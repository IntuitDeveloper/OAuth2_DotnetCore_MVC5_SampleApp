using Intuit.Ipp.Core;
using Newtonsoft.Json.Linq;
using System;
using System.Threading.Tasks;

namespace OAuth2_CoreMVC_Sample.Helper
{
    public interface IServices
    {
         Task QBOApiCall(Action<ServiceContext> apiCallFunction,string val=null);
    }
}
