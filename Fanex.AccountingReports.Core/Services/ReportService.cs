namespace Fanex.AccountingReports.Core.Services.Impl
{
    using System;
    using System.Collections.Generic;
    using Fanex.AccountingReports.Core.Constants;
    using Fanex.AccountingReports.Core.Criteria;
    using Fanex.AccountingReports.Core.Models;
    using Fanex.AccountingReports.Core.Repository.Impl;
    using Fanex.AccountingReports.Core.Resources;
    using Fanex.Common;
    using Fanex.Data.Repository;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public static class ReportService
    {
        #region StaticFields

        public const string CacheExchangeCurrencies = "Fanex.AccountingReports.Core.ReportService.ExchangeCurrencies";
        public const string BetTrans14SPSuffix = "_14";
        public const string WarehouseSPSuffix = "_WH";
        private static readonly IDynamicRepository BalanceUpRepository;

        private static readonly ReportHelperRepository ReportHelperRepository;

        #endregion StaticFields

        static ReportService()
        {
            BalanceUpRepository = new DynamicRepository();
            ReportHelperRepository = new ReportHelperRepository();
        }

        #region StaticProperties

        public static BalanceUp BalanceUp
        {
            get
            {
                var criteria = new BalanceUpCriteria();
                var balanceUp = BalanceUpRepository.Get<BalanceUp>(criteria);
                return balanceUp;
            }
        }

        public static Dictionary<int, string> ExchangeCurrencies
        {
            get
            {
                var exchangeCurrencies = GetExchangeCurrencies(new GetExchangeCurrenciesCriteria
                {
                    ExcludeTest = false
                });
                return exchangeCurrencies;
            }
        }

        public static DWHDateRange DWHDateRange
        {
            get
            {
                var criteria = new GetDWHDateRangeCriteria();
                var result = ReportHelperRepository.GetDWHDateRange(criteria);

                return result;
            }
        }

        #endregion StaticProperties

        #region StaticMethods

        /// <summary>
        /// Gets the store mapping suffix.
        /// </summary>
        /// <param name="reportDate">The report date.</param>
        /// <returns>The store mapping suffix.</returns>
        public static string GetStoreMappingSuffix(DateTime reportDate)
        {
            if (reportDate > BalanceUp.MoveBetsDay)
            {
                return string.Empty;
            }
            else if (reportDate >= BalanceUp.BalanceUpDay)
            {
                return BetTrans14SPSuffix;
            }
            else
            {
                return WarehouseSPSuffix;
            }
        }

        public static bool IsRequestDataWH(DateTime reportDate)
        {
            if (reportDate >= BalanceUp.BalanceUpDay)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        /// <summary>
        /// Updates the latest exchange currencies.
        /// </summary>
        public static void UpdateLatestExchangeCurrencies()
        {
            var exchangeCurrencies = GetExchangeCurrencies(new GetExchangeCurrenciesCriteria());
        }

        /// <summary>
        /// Calculates the margin.
        /// </summary>
        /// <param name="lineTotal">The line total.</param>
        /// <param name="turnover">The turnover.</param>
        /// <returns>The margin.</returns>
        public static decimal CalculateMargin(decimal lineTotal, decimal turnover)
        {
            decimal result = 0;
            if (turnover == 0)
            {
                return 0;
            }

            result = lineTotal / turnover;
            return result;
        }

        /// <summary>
        /// Gets the name of the period type.
        /// </summary>
        /// <param name="periodId">The period identifier.</param>
        /// <returns>The name of the period type.</returns>
        public static string GetPeriodTypeName(int periodId)
        {
            switch (periodId)
            {
                case PeriodTypeConstants.Daily:
                    return Labels.Daily;

                case PeriodTypeConstants.Weekly:
                    return Labels.Weekly;

                case PeriodTypeConstants.Monthly:
                    return Labels.Monthly;

                case PeriodTypeConstants.Annualy:
                    return Labels.Annualy;

                case PeriodTypeConstants.MonthRange:
                    return Labels.Month_Range;
            }

            return string.Empty;
        }

        /// <summary>
        /// Gets the users name suggestion.
        /// </summary>
        /// <param name="criteria">The criteria.</param>
        /// <returns>The users name suggestion.</returns>
        public static UserNameSuggestion UserNameSuggestion(UserNameSuggestionCriteria criteria)
        {
            var suggestionResult = new UserNameSuggestion
            {
                QueryValue = criteria.Username,
                Suggestions = new List<string>(),
                DataInfo = new string[] { string.Empty }
            };

            var rawData = ReportHelperRepository.UserNameSuggestion(criteria);
            foreach (var data in rawData)
            {
                var info = Converter.ToString2(data.UserName);
                suggestionResult.Suggestions.Add(info);
            }

            return suggestionResult;
        }

        /// <summary>
        /// Gets the currency by licensee.
        /// </summary>
        /// <param name="criteria">The criteria.</param>
        /// <returns>The currency by licensee.</returns>
        public static List<EntityBase<int, string>> GetCurrencyByLicensee(GetCurrencyByLicenseeCriteria criteria)
        {
            var currencyList = new List<EntityBase<int, string>>();
            var rawData = ReportHelperRepository.GetCurrencyByLicensee(criteria);
            EntityBase<int, string> info = new EntityBase<int, string>
            {
                Id = CommonConstants.CurrencyIdBaseRM,
                Name = Labels.BaseRM
            };

            currencyList.Add(info);

            foreach (var data in rawData)
            {
                if (IsExcludeCurrency(data.Currency))
                {
                    continue;
                }

                info = new EntityBase<int, string>
                {
                    Id = Converter.ToInt(data.ExchangeID),
                    Name = Converter.ToString2(data.Currency)
                };

                currencyList.Add(info);
            }

            return currencyList;
        }

        /// <summary>
        /// Gets the licensee support by product.
        /// </summary>
        /// <param name="criteria">The criteria.</param>
        /// <returns>The licensee_ support by product.</returns>
        public static List<EntityBase<int, string>> GetLicensee_SupportByProduct(GetLicensee_SupportByProductCriteria criteria)
        {
            var licenseeList = new List<EntityBase<int, string>>();
            var rawData = ReportHelperRepository.GetLicensee_SupportByProduct(criteria);
            EntityBase<int, string> info;
            foreach (var data in rawData)
            {
                info = new EntityBase<int, string>
                {
                    Id = Converter.ToInt(data.LicenseeID),
                    Name = Converter.ToString2(data.Licenseename)
                };

                licenseeList.Add(info);
            }

            return licenseeList;
        }

        /// <summary>
        /// Gets the name of the site type.
        /// </summary>
        /// <param name="siteType">Type of the site.</param>
        /// <returns>The name of the site type.</returns>
        public static string GetSiteTypeName(int siteType)
        {
            switch (siteType)
            {
                case SiteType.AllSites:
                    return Labels.All_Sites;

                case SiteType.Credit:
                    return Labels.Credit_Site;

                case SiteType.Deposit:
                    return Labels.Deposit_Site;

                default:
                    return string.Empty;
            }
        }

        /// <summary>
        /// Gets the name of the role.
        /// </summary>
        /// <param name="roleId">The role identifier.</param>
        /// <returns>The name of the role.</returns>
        public static string GetRoleName(int roleId)
        {
            switch (roleId)
            {
                case RoleConstants.Super:
                    return Labels.Super;

                case RoleConstants.Master:
                    return Labels.Master;

                case RoleConstants.Agent:
                    return Labels.Agent;

                case RoleConstants.Member:
                    return Labels.Member;

                default:
                    return string.Empty;
            }
        }

        /// <summary>
        /// Gets the live type name.
        /// </summary>
        /// <param name="liveType">Type of the live.</param>
        /// <returns>The live type name.</returns>
        public static string GetLiveTypeName(int liveType)
        {
            string name = string.Empty;
            switch (liveType)
            {
                case LiveTypeConstants.NonLive:
                    name = Labels.Non_Live;
                    break;

                case LiveTypeConstants.Live:
                    name = Labels.Live;
                    break;

                case LiveTypeConstants.MixLiveTypes:
                    name = Labels.Mix_Live_and_Non_Live;
                    break;
            }

            return name;
        }

        /// <summary>
        /// Gets the exchange currencies.
        /// </summary>
        /// <param name="criteria">The criteria.</param>
        /// <returns>The exchange currencies.</returns>
        public static Dictionary<int, string> GetExchangeCurrencies(GetExchangeCurrenciesCriteria criteria)
        {
            var exchangeCurrencies = new Dictionary<int, string>();
            var rawData = (new ExchangeCurrencyRepository()).GetExchangeCurrencies(criteria);
            if (rawData != null)
            {
                rawData.ForEach(currency =>
                {
                    if (!exchangeCurrencies.ContainsKey(currency.CurrencyId))
                    {
                        exchangeCurrencies.Add(currency.CurrencyId, currency.CurrencyName);
                    }
                });
            }

            return exchangeCurrencies;
        }

        public static bool IsExcludeCurrency(string currencyName)
        {
            return string.Equals(currencyName, "C") ||
                string.Equals(currencyName, "UUS") ||
                string.Equals(currencyName, "H");
        }

        public static DateTime ConvertTimeZone(DateTime dateTime)
        {
            TimeZoneInfo timeZone = TimeZoneInfo.FindSystemTimeZoneById("SA Western Standard Time");
            DateTime newDateTime = TimeZoneInfo.ConvertTime(dateTime, timeZone);

            return newDateTime;
        }

        #endregion StaticMethods
    }
}