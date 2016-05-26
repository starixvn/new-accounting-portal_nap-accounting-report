// -----------------------------------------------------------------------
// <copyright file="LiveCasinoProductService.cs" company="Nexcel Solutions Vietnam">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace Fanex.AccountingReports.Gaming.Services.Implement.CasinoGameStatistic
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using Fanex.AccountingReports.Core.Criteria;
    using Fanex.AccountingReports.Core.Models;
    using Fanex.AccountingReports.Core.Services.Impl;
    using Fanex.AccountingReports.Core.Web;
    using Fanex.AccountingReports.Gaming.Criteria.CasinoGameStatistic;
    using Fanex.AccountingReports.Gaming.Models.CasinoGameStatistic;
    using Fanex.AccountingReports.Gaming.Repositories.Implement;
    using Fanex.AccountingReports.Gaming.Repositories.Interface;
    using Fanex.AccountingReports.Gaming.Resources;
    using Fanex.AccountingReports.Gaming.Services.Interface.CasinoGameStatistic;
    using CoreConstants = Fanex.AccountingReports.Core.Constants;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class FilterSectionService : IFilterSectionService
    {
        private ICasinoGameStatisticRepository _repository;

        public FilterSectionService()
            : this(null)
        { 
        }

        public FilterSectionService(ICasinoGameStatisticRepository repository)
        {
            _repository = new CasinoGameStatisticRepository();
        }

        public List<string> GetProductList()
        {
            List<string> productGroup = new List<string>();

            var products = GetProductInfo();

            productGroup = products.ProductGroup;

            return productGroup;            
        }

        private ProductList GetProductInfo()
        {
            var products = _repository.GetProductList(new GetProductListCriteria());
            return products;
        }

        public List<ProductItem> Get3rdPartyByProduct(string product)
        {
            List<ProductItem> product3rdParty = new List<ProductItem>();

            var products = GetProductInfo();

            product3rdParty = products.Products.Where(s => s.ProductGroup == product).ToList();

            return product3rdParty;
        }

        [SuppressMessage("StyleCop.CSharp.Nexcel.NexcelCustomRules",
            "SP2101:MethodMustNotContainMoreLinesThan",
            Justification = "Reviewed. Suppression is OK here.")]
        public List<EntityBase<int, string>> GetCurrencyList(GetExchangeCurrenciesCriteria criteria)
        {
            var result = new List<EntityBase<int, string>>();
            try
            {
                Dictionary<int, string> currencies = ReportService.GetExchangeCurrencies(criteria);
                var currencyList = new List<EntityBase<int, string>>();

                foreach (var row in currencies)
                {
                    if (criteria.ExcludeTest && ReportService.IsExcludeCurrency(row.Value))
                    {
                        continue;
                    }

                    currencyList.Add(new EntityBase<int, string>()
                    {
                        Id = row.Key,
                        Name = row.Value
                    });
                }

                currencyList = currencyList.OrderBy(currency => currency.Name).ToList();

                result.Add(new EntityBase<int, string>()
                {
                    Id = (int)CoreConstants.CurrencyContants.All,
                    Name = Labels.All
                });

                result.Add(new EntityBase<int, string>()
                {
                    Id = (int)CoreConstants.CurrencyContants.BaseRM,
                    Name = Labels.Base_RM
                });

                foreach (var currency in currencyList)
                {
                    result.Add(new EntityBase<int, string>()
                    {
                        Id = currency.Id,
                        Name = currency.Name
                    });
                }
            }
            catch (Exception ex)
            {
                LoggingHandler.Log(ex);
            }

            return result;
        }

        public List<ProductGameType> GetProductGameTypes(GetGameTypeListCriteria criteria)
        {
            List<ProductGameType> result = new List<ProductGameType>();
            var products = GetProductInfo();
            var gameTypes = _repository.GetGameType(criteria);

            var productIds = gameTypes.Select(s => s.ProductId).Distinct().ToList();

            foreach (var productId in productIds)
            {
                var productGameType = new ProductGameType()
                {
                    GameTypes = new List<EntityBase<int, string>>()
                };

                productGameType.ProductName = products.Products.Where(s => s.ProductId == productId).Select(s => s.ProductName).FirstOrDefault();
                productGameType.ProductId = productId;
                var gameTypeByProduct = gameTypes.Where(s => s.ProductId == productId).ToList();

                foreach (var gameType in gameTypeByProduct)
                {
                    productGameType.GameTypes.Add(new EntityBase<int, string>()
                    {
                        Id = gameType.TypeId,
                        Name = gameType.Name
                    });
                }

                result.Add(productGameType);
            }

            return result;
        }

        public List<EntityBase<int, string>> GetGameTableList(GetGameTableListCriteria criteria)
        {
            var gameTableList = _repository.GetGameTableList(criteria);
            gameTableList = gameTableList.OrderBy(info => info.Name).ToList();

            return gameTableList;
        }
    }
}