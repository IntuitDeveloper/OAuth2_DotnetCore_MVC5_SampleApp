using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Intuit.Ipp.Core;
using Intuit.Ipp.Data;
using Intuit.Ipp.DataService;
using Intuit.Ipp.Exception;
using Intuit.Ipp.QueryFilter;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using OAuth2_CoreMVC_Sample.Helper;
using OAuth2_CoreMVC_Sample.Models;
using OAuth2_CoreMVC_Sample.Models.DB;
using Invoice = Intuit.Ipp.Data.Invoice;


// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace OAuth2_CoreMVC_Sample.Controllers
{
    public class QBOController : Controller
    {
       private readonly IServices _services;
        private readonly tokensContext _tokens;
        private string id;
        public QBOController(IServices services, tokensContext tokens)
        {
            _services = services;
            _tokens = tokens;
        }
        // GET: /<controller>/
        public IActionResult Index()
        {
            return View("QBO");
        }

     public async Task<IActionResult> CreateCustomer()
        {
            Token token = await _tokens.Token.FirstOrDefaultAsync(t => t.RealmId == OAuth2Keys.RealmId);

            Action<ServiceContext> apiCallFucntion = new Action<ServiceContext>(CreateNewCustomer);
            await _services.QBOApiCall(apiCallFucntion);
            return View("QBO");
            
        }

        public async Task<IActionResult> CreateInvoice(string customerId)
        {
            id = customerId;
            Action<ServiceContext> apiCallFucntion = new Action<ServiceContext>(CreateNewInvoice);
            await _services.QBOApiCall(apiCallFucntion);

            return View("QBO");
        }
        

        #region HelperMethods

        private void CreateNewCustomer(ServiceContext context)
        {
            DataService dataService = new DataService(context);
            QueryService<Customer> queryService = new QueryService<Customer>(context);
            Customer customer = Inputs.CreateCustomer(dataService);
            ViewData["CustomerInfo"]="Customer with ID:"+customer.Id+" created successfully";
            ViewData["CustomerId"] = customer.Id;
        }

        private void CreateNewInvoice(ServiceContext context)
        {
            DataService dataService = new DataService(context);
            QueryService<Account> queryService = new QueryService<Account>(context);
            QueryService<Customer> customerService = new QueryService<Customer>(context);
            string query = "Select * from Customer where Id='" + id+"'";
            List<Customer> queryResponse = customerService.ExecuteIdsQuery(query).ToList<Customer>();
            Invoice invoice = null;
            if (queryResponse != null)
            {
                 invoice = Inputs.CreateInvoice(dataService, queryService, queryResponse[0]);
                ViewData["InvoiceInfo"] = "Invoice with ID:" + invoice.Id + " created successfully";

            }
            else
            {
                ViewData["InvoiceInfo"] ="Invalid Customer information";

            }
            ViewData["CustomerId"] = id;

        }
        #endregion
    }
}
