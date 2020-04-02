using System;
using System.Threading.Tasks;
using Intuit.Ipp.Core;

namespace OAuth2_CoreMVC_Sample.Helper
{
    public interface IServices
    {
        Task QBOApiCall(Action<ServiceContext> apiCallFunction);
    }
}