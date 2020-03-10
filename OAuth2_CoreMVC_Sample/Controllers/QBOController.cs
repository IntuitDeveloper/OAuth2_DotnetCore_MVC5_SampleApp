using System;
using System.Linq;
using System.Threading.Tasks;
using Intuit.Ipp.Core;
using Intuit.Ipp.Data;
using Intuit.Ipp.DataService;
using Intuit.Ipp.QueryFilter;
using Microsoft.AspNetCore.Mvc;
using OAuth2_CoreMVC_Sample.Helper;
using OAuth2_CoreMVC_Sample.Models;


// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace OAuth2_CoreMVC_Sample.Controllers
{
    public class QBOController : Controller
    {
        private readonly IServices _services;
        private readonly TokensContext _tokens;
        private string id;

        public QBOController(IServices services, TokensContext tokens)
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
            var apiCallFucntion = new Action<ServiceContext>(CreateNewCustomer);
            await _services.QBOApiCall(apiCallFucntion);
            return View("QBO");
        }

        public async Task<IActionResult> CreateInvoice(string customerId)
        {
            id = customerId;
            var apiCallFucntion = new Action<ServiceContext>(CreateNewInvoice);
            await _services.QBOApiCall(apiCallFucntion);

            return View("QBO");
        }


        #region HelperMethods

        private void CreateNewCustomer(ServiceContext context)
        {
            var dataService = new DataService(context);
            var queryService = new QueryService<Customer>(context);
            var customer = Inputs.CreateCustomer(dataService);
            ViewData["CustomerInfo"] = "Customer with ID:" + customer.Id + " created successfully";
            ViewData["CustomerId"] = customer.Id;
        }

        private void CreateNewInvoice(ServiceContext context)
        {
            var dataService = new DataService(context);
            var queryService = new QueryService<Account>(context);
            var customerService = new QueryService<Customer>(context);
            var query = "Select * from Customer where Id='" + id + "'";
            var queryResponse = customerService.ExecuteIdsQuery(query).ToList();
            Invoice invoice = null;
            if (queryResponse != null)
            {
                invoice = Inputs.CreateInvoice(dataService, queryService, queryResponse[0]);
                ViewData["InvoiceInfo"] = "Invoice with ID:" + invoice.Id + " created successfully";
            }
            else
            {
                ViewData["InvoiceInfo"] = "Invalid Customer information";
            }

            ViewData["CustomerId"] = id;
        }

        #endregion
    }
}