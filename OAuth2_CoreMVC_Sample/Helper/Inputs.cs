using System;
using System.Linq;
using Intuit.Ipp.Data;
using Intuit.Ipp.DataService;
using Intuit.Ipp.QueryFilter;

namespace OAuth2_CoreMVC_Sample.Helper
{
    public class Inputs
    {
        internal static Customer CreateCustomer(DataService dataService)
        {
            var rnd = new Random();
            var newCust = new Customer
            {
                DisplayName = "Testing Company" + rnd.NextDouble(),
                GivenName = "Testing Company" + rnd.NextDouble(),
                FamilyName = "Testing Company" + rnd.NextDouble()
            };
            var response = dataService.Add(newCust);
            return response;
        }

        internal static Invoice CreateInvoice(DataService dataService, QueryService<Account> queryService,
            Customer customer)
        {
            var item = ItemCreate(dataService, queryService);
            var line = new Line
            {
                DetailType = LineDetailTypeEnum.SalesItemLineDetail,
                DetailTypeSpecified = true,
                Description = "Sample for Reimburse Charge with Invoice.",
                Amount = new decimal(40),
                AmountSpecified = true
            };
            var lineDetail = new SalesItemLineDetail
            {
                ItemRef = new ReferenceType {name = item.Name, Value = item.Id}
            };
            line.AnyIntuitObject = lineDetail;

            Line[] lines = {line};

            var invoice = new Invoice
            {
                Line = lines,
                CustomerRef = new ReferenceType {name = customer.DisplayName, Value = customer.Id},
                TxnDate = DateTime.Now.Date
            };

            var response = dataService.Add(invoice);
            return response;
        }


        #region Helper methods

        internal static Item ItemCreate(DataService dataService, QueryService<Account> queryService)
        {
            var random = new Random();
            var expenseAccount = QueryOrAddAccount(dataService, queryService,
                "select * from account where AccountType='Cost of Goods Sold'", AccountTypeEnum.CostofGoodsSold,
                AccountClassificationEnum.Expense, AccountSubTypeEnum.SuppliesMaterialsCogs);
            var incomeAccount = QueryOrAddAccount(dataService, queryService,
                "select * from account where AccountType='Income'", AccountTypeEnum.Income,
                AccountClassificationEnum.Revenue, AccountSubTypeEnum.SalesOfProductIncome);
            var item = new Item
            {
                Name = "Item_" + random.NextDouble(),
                ExpenseAccountRef = new ReferenceType {name = expenseAccount.Name, Value = expenseAccount.Id},
                IncomeAccountRef = new ReferenceType {name = incomeAccount.Name, Value = incomeAccount.Id},
                Type = ItemTypeEnum.NonInventory,
                TypeSpecified = true,
                UnitPrice = new decimal(100.0),
                UnitPriceSpecified = true
            };

            var apiResponse = dataService.Add(item);
            return apiResponse;
        }

        internal static Account QueryOrAddAccount(DataService dataService, QueryService<Account> queryService,
            string query, AccountTypeEnum accountType, AccountClassificationEnum classification,
            AccountSubTypeEnum subType)
        {
            var queryResponse = queryService.ExecuteIdsQuery(query).ToList();

            if (queryResponse.Count == 0)
            {
                var account = AccountCreate(dataService, accountType, classification, subType);
                return account;
            }

            return queryResponse[0];
        }

        internal static Account AccountCreate(DataService dataService, AccountTypeEnum accountType,
            AccountClassificationEnum classification, AccountSubTypeEnum subType)
        {
            var random = new Random();
            var account = new Account
            {
                Name = "Account_" + random.NextDouble(),
                AccountType = accountType,
                AccountTypeSpecified = true,
                Classification = classification,
                ClassificationSpecified = true,
                AccountSubType = subType.ToString(),
                SubAccountSpecified = true
            };
            var apiResponse = dataService.Add(account);
            return apiResponse;
        }

        #endregion
    }
}