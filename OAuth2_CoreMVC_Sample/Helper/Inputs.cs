using Intuit.Ipp.Core;
using Intuit.Ipp.Data;
using Intuit.Ipp.DataService;
using Intuit.Ipp.QueryFilter;
using OAuth2_CoreMVC_Sample.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace OAuth2_CoreMVC_Sample.Helper
{
    public class Inputs
    {
        internal static Customer CreateCustomer(DataService dataService)
        {
            Random rnd = new Random();
            Customer newCust = new Customer
            {

                DisplayName = "Testing Company" + rnd.NextDouble(),
                GivenName = "Testing Company" + rnd.NextDouble(),
                FamilyName = "Testing Company" + rnd.NextDouble(),
            };
            Customer response = dataService.Add(newCust);
            return response;
        }

        internal static Invoice CreateInvoice(DataService dataService, QueryService<Account> queryService, Customer customer)
        {
           
           Item item = ItemCreate(dataService, queryService);
            Line line = new Line
            {
                DetailType = LineDetailTypeEnum.SalesItemLineDetail,
                DetailTypeSpecified = true,
                Description = "Sample for Reimburse Charge with Invoice.",
                Amount = new Decimal(40),
                AmountSpecified = true
               
            };
            SalesItemLineDetail lineDetail = new SalesItemLineDetail
            {
                ItemRef = new ReferenceType { name = item.Name, Value = item.Id }
            };
            line.AnyIntuitObject = lineDetail;

            Line[] lines = { line };

            Invoice invoice = new Invoice
            {
                Line = lines,
                CustomerRef = new ReferenceType { name = customer.DisplayName, Value = customer.Id },
                TxnDate = DateTime.Now.Date
            };

            Invoice response = dataService.Add(invoice);
            return response;
        }


        #region Helper methods

        internal static Item ItemCreate(DataService dataService, QueryService<Account> queryService)
        {
            Random random = new Random();
            Account expenseAccount = QueryOrAddAccount(dataService, queryService, "select * from account where AccountType='Cost of Goods Sold'", AccountTypeEnum.CostofGoodsSold, AccountClassificationEnum.Expense, AccountSubTypeEnum.SuppliesMaterialsCogs);
            Account incomeAccount = QueryOrAddAccount(dataService, queryService, "select * from account where AccountType='Income'", AccountTypeEnum.Income, AccountClassificationEnum.Revenue, AccountSubTypeEnum.SalesOfProductIncome);
            Item item = new Item
            {
                Name = "Item_" + random.NextDouble(),
                ExpenseAccountRef = new ReferenceType { name = expenseAccount.Name, Value = expenseAccount.Id },
                IncomeAccountRef = new ReferenceType { name = incomeAccount.Name, Value = incomeAccount.Id },
                Type = ItemTypeEnum.NonInventory,
                TypeSpecified = true,
                UnitPrice = new Decimal(100.0),
                UnitPriceSpecified = true
            };

            Item apiResponse = dataService.Add(item);
            return apiResponse;
        }

        internal static Account QueryOrAddAccount(DataService dataService, QueryService<Account> queryService, String query, AccountTypeEnum accountType, AccountClassificationEnum classification, AccountSubTypeEnum subType)
        {
            List<Account> queryResponse = queryService.ExecuteIdsQuery(query).ToList<Account>();

            if (queryResponse.Count == 0)
            {
                Account account = AccountCreate(dataService, accountType, classification, subType);
                return account;
            }
            return queryResponse[0];
        }

        internal static Account AccountCreate(DataService dataService, AccountTypeEnum accountType, AccountClassificationEnum classification, AccountSubTypeEnum subType)
        {
            Random random = new Random();
            Account account = new Account
            {
                Name = "Account_" + random.NextDouble(),
                AccountType = accountType,
                AccountTypeSpecified = true,
                Classification = classification,
                ClassificationSpecified = true,
                AccountSubType = subType.ToString(),
                SubAccountSpecified = true
            };
            Account apiResponse = dataService.Add(account);
            return apiResponse;
        }

  

        #endregion
    }
}
