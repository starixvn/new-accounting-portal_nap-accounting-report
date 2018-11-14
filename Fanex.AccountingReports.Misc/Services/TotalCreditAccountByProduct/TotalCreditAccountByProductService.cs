namespace Fanex.AccountingReports.Misc.Services.Implement.TotalCreditAccountByProduct
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Linq;
    using Fanex.AccountingReports.Core.Configurations;
    using Fanex.AccountingReports.Core.Criteria;
    using Fanex.AccountingReports.Core.Models;
    using Fanex.AccountingReports.Core.Services.Impl;
    using Fanex.AccountingReports.Core.Web;
    using Fanex.AccountingReports.Misc.Criteria.TotalCreditAccountByProduct;
    using Fanex.AccountingReports.Misc.Models.TotalCreditAccountByProduct;
    using Fanex.Common;
    using Fanex.Data;

    /// <summary>
    /// Total credit account by product service.
    /// </summary>
    public partial class TotalCreditAccountByProductService : ServiceBase
    {
        private readonly string _totalCreditAccountByProductSessionKey = "Fanex.AccountingReports.Misc.TotalCreditAccountByProduct." +
            (AppSession.CurrentUser != null ? AppSession.CurrentUser.Username : string.Empty);

        private readonly string _betCountDetailSessionKey = "Fanex.AccountingReports.Misc.BetCountDetail." +
            (AppSession.CurrentUser != null ? AppSession.CurrentUser.Username : string.Empty);

        /// <summary>
        /// Gets the products.
        /// </summary>
        /// <returns>List sport products.</returns>
        public List<BaseModel> GetProducts()
        {
            try
            {
                List<BaseModel> result = new List<BaseModel>();
                TotalCreditAccountByProductCollection collection = TotalCreditAccountByProductSection.Instance.Products;
                foreach (TotalCreditAccountByProductElement element in collection)
                {
                    if (!element.IsActive)
                    {
                        continue;
                    }

                    result.Add(new BaseModel()
                    {
                        Id = Converter.ToInt(element.Id),
                        Name = Converter.ToString2(element.Name),
                        AliasName = Converter.ToString2(element.AliasName)
                    });
                }

                return result.OrderBy(x => x.Name).ToList();
            }
            catch (Exception ex)
            {
                HandleException(ex);
                throw;
            }
        }

        /// <summary>
        /// Gets the currencies.
        /// </summary>
        /// <returns>The currencies.</returns>
        public List<EntityBase<int, string>> GetCurrencies()
        {
            var result = new List<EntityBase<int, string>>();
            try
            {
                Dictionary<int, string> currencies = ReportService.GetExchangeCurrencies(
                    new GetExchangeCurrenciesCriteria
                    {
                        ExcludeTest = true
                    });

                foreach (var currency in currencies)
                {
                    if (ReportService.IsExcludeCurrency(currency.Value))
                    {
                        continue;
                    }

                    result.Add(new EntityBase<int, string>()
                    {
                        Id = currency.Key,
                        Name = currency.Value
                    });
                }
            }
            catch (Exception ex)
            {
                HandleException(ex);
            }

            return result.OrderBy(r => r.Name).ToList();
        }

        /// <summary>
        /// Gets the total credit account by product.
        /// </summary>
        /// <param name="criteria">The criteria.</param>
        /// <returns>Total credit account by product.</returns>
        public TotalCreditAccountByProduct GetTotalCreditAccountByProduct(GetTotalCreditAccountByProductCriteria criteria)
        {
            try
            {
                var result = this.GetTotalCreditAccountByProductData(criteria);
                var startItemIndex = criteria.PageIndex * criteria.PageSize;

                var pagingResult = new TotalCreditAccountByProduct
                {
                    ReportRows = result.ReportRows.Skip(startItemIndex).Take(criteria.PageSize).ToList(),
                    Total = result.Total,
                    ReportLength = result.ReportRows.Count
                };

                SessionManager.Set<TotalCreditAccountByProduct>(_totalCreditAccountByProductSessionKey, result);
                return pagingResult;
            }
            catch (Exception ex)
            {
                HandleException(ex);
                throw;
            }
        }

        /// <summary>
        /// Gets the bet count detail.
        /// </summary>
        /// <param name="criteria">The criteria.</param>
        /// <returns>Get bet count detail.</returns>
        public BetCountDetail GetBetCountDetail(GetBetCountDetailCriteria criteria)
        {
            BalanceUp balanceUp = ReportService.BalanceUp;
            criteria.BalanceUpDay = balanceUp.BalanceUpDay;
            criteria.MoveBetsDay = balanceUp.MoveBetsDay;

            try
            {
                var result = this.GetBetCountDetailByCustId(criteria);
                var startItemIndex = criteria.PageIndex * criteria.PageSize;

                var pagingResult = new BetCountDetail
                {
                    ReportRows = result.ReportRows.Skip(startItemIndex).Take(criteria.PageSize).ToList(),
                    Total = result.Total,
                    ReportLength = result.ReportRows.Count
                };

                SessionManager.Set<BetCountDetail>(_betCountDetailSessionKey, result);
                return pagingResult;
            }
            catch (Exception ex)
            {
                HandleException(ex);
                throw;
            }
        }

        /// <summary>
        /// Gets the total credit account by product data.
        /// </summary>
        /// <param name="criteria">The criteria.</param>
        /// <returns>Get total credit account by product data.</returns>
        private TotalCreditAccountByProduct GetTotalCreditAccountByProductData(GetTotalCreditAccountByProductCriteria criteria)
        {
            var result = new TotalCreditAccountByProduct
            {
                Total = new TotalCreditAccountByProduct()
            };

            using (var objectDb = new ObjectDb(criteria.GetSettingKey()))
            {
                var _params = criteria.GetSpParams();

                IDataReader reader = objectDb.ExecuteReader(_params);

                while (reader.Read())
                {
                    TotalCreditAccountByProduct totalCreditAccountByProduct = new TotalCreditAccountByProduct
                    {
                        CustId = Converter.ToInt(reader["custid"]),
                        UserName = Converter.ToString2(reader["username"]),
                        IsDisable = Converter.ToBool(reader["IsDisable"]),
                        Currency = Converter.ToString2(reader["currency"]),
                        MasterEnable = Converter.ToInt(reader["MasterEnabled"]),
                        MasterDisable = Converter.ToInt(reader["MasterDisabled"]),
                        AgentEnable = Converter.ToInt(reader["AgentEnabled"]),
                        AgentDisable = Converter.ToInt(reader["AgentDisabled"]),
                        MemberEnable = Converter.ToInt(reader["MemberEnabled"]),
                        MemberDisable = Converter.ToInt(reader["MemberDisabled"])
                    };

                    result.Total.CalculateTotal(totalCreditAccountByProduct);
                    result.ReportRows.Add(totalCreditAccountByProduct);
                }
            }

            return result;
        }

        /// <summary>
        /// Gets the bet count detail by customer identifier.
        /// </summary>
        /// <param name="criteria">The criteria.</param>
        /// <returns>Get bet count detail data.</returns>
        private BetCountDetail GetBetCountDetailByCustId(GetBetCountDetailCriteria criteria)
        {
            var result = new BetCountDetail
            {
                Total = new BetCountDetail()
            };

            using (var objectDb = new ObjectDb(criteria.GetSettingKey()))
            {
                var _params = criteria.GetSpParams();

                IDataReader reader = objectDb.ExecuteReader(_params);

                while (reader.Read())
                {
                    BetCountDetail betCountDetail = new BetCountDetail
                    {
                        UserName = Converter.ToString2(reader["username"]),
                        IsDisable = Converter.ToBool(reader["IsDisable"]),
                        SettledBetCount = Converter.ToInt(reader["SettledBets"]),
                        RunningBetCount = Converter.ToInt(reader["RunningBets"])
                    };

                    result.Total.CalculateTotal(betCountDetail);
                    result.ReportRows.Add(betCountDetail);
                }
            }

            return result;
        }
    }
}