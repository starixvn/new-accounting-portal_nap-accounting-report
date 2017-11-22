namespace Fanex.AccountingReports.Racing.Services.Implement.RacingWinLossByLicensee
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using System.Web;
    using Fanex.AccountingReports.Core.Criteria;
    using Fanex.AccountingReports.Core.Services.Impl;
    using Fanex.AccountingReports.Core.Web;
    using Fanex.AccountingReports.Racing.Criteria.RacingWinLossByLicensee;
    using Fanex.AccountingReports.Racing.Models.RacingWinLossByLicensee;
    using Fanex.AccountingReports.Racing.Services.Interface.RacingWinLossByLicensee;
    using Fanex.Common;

    /// <summary>
    /// The implement of get report service of Racing Win Loss By Licensee report.
    /// </summary>
    public class RacingWinLossByLicenseeService : ServiceBase, IRacingWinLossByLicenseeService
    {
        private const string RacingWinLossByLicenseePrefixSessionKey =
                                "Fanex.AccountingReports.Racing.RacingWinLossByLicensee.";

        private const string RacingWinLossByLicenseePagingPrefixSessonKey =
                                "Fanex.AccountingReports.Racing.RacingWinLossByLicensee.Paging.";

        private string _racingWinLossByLicenseeSessionKey = string.Empty;

        public RacingWinLossByLicenseeService()
        {
            if (AppSession.CurrentUser != null)
            {
                _racingWinLossByLicenseeSessionKey += RacingWinLossByLicenseePrefixSessionKey +
                                                      AppSession.CurrentUser.Username;
            }
            else
            {
                _racingWinLossByLicenseeSessionKey = RacingWinLossByLicenseePrefixSessionKey;
            }
        }

        /// <summary>
        /// Get racing win loss by licensee report.
        /// </summary>
        /// <param name="criteria">The criteria.</param>
        /// <returns>The RacingDailyReport pagingReport.</returns>
        public RacingDailyReport GetReport(GetRacingDailyReportCriteria criteria)
        {
            try
            {
                var fullReport = new RacingDailyReport();
                var pagingReport = new RacingDailyReport();

                if (!criteria.IsPagingReport)
                {
                    fullReport = this.GetAndConvertData(fullReport, criteria);
                    SessionManager.Set<RacingDailyReport>(_racingWinLossByLicenseeSessionKey + criteria.Site, fullReport);
                }
                else
                {
                    fullReport = SessionManager.Get<RacingDailyReport>(_racingWinLossByLicenseeSessionKey + criteria.Site);
                }

                if (fullReport != null && fullReport.Rows != null && fullReport.Rows.Count > 0)
                {
                    string pagingSessionKey = RacingWinLossByLicenseePagingPrefixSessonKey
                                                + AppSession.CurrentUser.Username
                                                + "." + criteria.WinLostDate + "." + criteria.Site 
                                                + "." + criteria.StartIndex + "." + criteria.NumberItemsPerPage;

                    pagingReport = this.GetPagingReport(fullReport, pagingReport, criteria, pagingSessionKey);
                }

                return pagingReport;
            }
            catch (Exception ex)
            {
                this.HandleException(ex);
                throw;
            }
        }

        #region convert data

        /// <summary>
        /// Get and convert data.
        /// </summary>
        /// <param name="fullReport">The full report.</param>
        /// <param name="criteria">The criteria.</param>
        /// <returns>The RacingDailyReport fullReport.</returns>
        public RacingDailyReport GetAndConvertData(
            RacingDailyReport fullReport,
            GetRacingDailyReportCriteria criteria)
        {
            var rowsData = GetReportData<dynamic>(criteria);

            GetCurrencyByLicenseeCriteria licenseeCriteria = new GetCurrencyByLicenseeCriteria();
            var currenciesLicensee = ReportService.GetCurrencyByLicensee(licenseeCriteria);

            fullReport = GetAndConvertCurrencies(rowsData, currenciesLicensee, fullReport);

            return fullReport;
        }

        /// <summary>
        /// Get and convert currencies.
        /// </summary>
        /// <param name="rowsData">The rows data.</param>
        /// <param name="currenciesLicensee">The currencies licensee.</param>
        /// <param name="fullReport">The full report.</param>
        /// <returns>The RacingDailyReport fullReport.</returns>
        private RacingDailyReport GetAndConvertCurrencies(
            IEnumerable<dynamic> rowsData, 
            List<AccountingReports.Core.Models.EntityBase<int, string>> currenciesLicensee,
            RacingDailyReport fullReport)
        {
            var currencies = rowsData.Select(row => row.currency).Distinct().ToList();

            foreach (var currencyItem in currencies)
            {
                var rowsInCurrency = rowsData.Where(row => row.currency == currencyItem).ToList();
                int currencyID = Converter.ToInt(currencyItem);
                var currencyName = currenciesLicensee.Where(s => s.Id == currencyID)
                                                .Select(s => s.Name).FirstOrDefault();

                RacingDailyReport currency = new RacingDailyReport
                {
                    ID = currencyID,
                    Name = currencyName
                };

                currency = GetAndConvertLeagues(rowsInCurrency, currency);
                fullReport.Rows.Add(currency);
                fullReport.Races[1].SumTotal(currency.Races[1]);
            }

            fullReport.ID = rowsData.Count();
            return fullReport;
        }

        /// <summary>
        /// Get and convert the leagues.
        /// </summary>
        /// <param name="rowsInCurrency">The rows in currency.</param>
        /// <param name="currency">The currency input.</param>
        /// <returns>The RacingDailyReport currencies.</returns>
        private RacingDailyReport GetAndConvertLeagues(List<dynamic> rowsInCurrency, RacingDailyReport currency)
        {
            var leagues = rowsInCurrency.Select(row => row.leagueid).Distinct().ToList();

            foreach (var leagueItem in leagues)
            {
                var rowsInLeague = rowsInCurrency.Where(row => row.leagueid == leagueItem).ToList();
                int leagueID = Converter.ToInt(leagueItem);
                string leagueName = CacheRef.GetLeagueName(leagueID);

                RacingDailyReport league = new RacingDailyReport
                {
                    ID = leagueID,
                    Name = leagueName
                };

                league = GetAndConvertRaces(rowsInLeague, league);

                currency.Rows.Add(league);
                currency.Races[0].SumTotal(league.Races[0]);
                currency.Races[1].SumTotal(league.Races[1]);
            }

            return currency;
        }

        /// <summary>
        /// Get and convert races.
        /// </summary>
        /// <param name="rowsInLeague">The rows in league.</param>
        /// <param name="league">The league input.</param>
        /// <returns>The RacingDailyReport league.</returns>
        private RacingDailyReport GetAndConvertRaces(List<dynamic> rowsInLeague, RacingDailyReport league)
        {
            foreach (var raceItem in rowsInLeague)
            {
                RacingDailyReport race = new RacingDailyReport
                {
                    ID = Converter.ToInt(raceItem.groupmatch)
                };
                RaceWinLost raceBaseWinLost = new RaceWinLost
                {
                    MemberWinTurnover = Converter.ToDecimal(raceItem.winTOMem),
                    MemberPlaceTurnover = Converter.ToDecimal(raceItem.placeTOMem),
                    MemberWinPlaceTurnover = Converter.ToDecimal(raceItem.winplaceTOMem),
                    MemberTotalTurnover = Converter.ToDecimal(raceItem.totalTOMem),
                    MemberDividendWon = Converter.ToDecimal(raceItem.DividendWonMem),
                    Site12BetWinTurnover = Converter.ToDecimal(raceItem.winTOLicensee),
                    Site12BetPlaceTurnover = Converter.ToDecimal(raceItem.placeTOLicensee),
                    Site12BetWinPlaceTurnover = Converter.ToDecimal(raceItem.winplaceTOLicensee),
                    Site12BetTotalTurnover = Converter.ToDecimal(raceItem.totalTOLicensee),
                    Site12BetPositionTakingWinLoss = Converter.ToDecimal(raceItem.PTWLLicensee),
                    Site12BetCommissionIncome = Converter.ToDecimal(raceItem.commIncomeLicensee),
                    IBCPositionTakingWinLoss = Converter.ToDecimal(raceItem.PTWLIBC),
                    IBCCommissionIncome = Converter.ToDecimal(raceItem.CommIncomeIBC),
                    AmountDueToIBC = Converter.ToDecimal(raceItem.AmountDue2IBC)
                };
                RaceWinLost raceWinLost = new RaceWinLost
                {
                    MemberWinTurnover = Converter.ToDecimal(raceItem.winTOMem_Origin),
                    MemberPlaceTurnover = Converter.ToDecimal(raceItem.placeTOMem_Origin),
                    MemberWinPlaceTurnover = Converter.ToDecimal(raceItem.winplaceTOMem_Origin),
                    MemberTotalTurnover = Converter.ToDecimal(raceItem.totalTOMem_Origin),
                    MemberDividendWon = Converter.ToDecimal(raceItem.DividendWonMem_Origin),
                    Site12BetWinTurnover = Converter.ToDecimal(raceItem.winTOLicensee_Origin),
                    Site12BetPlaceTurnover = Converter.ToDecimal(raceItem.placeTOLicensee_Origin),
                    Site12BetWinPlaceTurnover = Converter.ToDecimal(raceItem.winplaceTOLicensee_Origin),
                    Site12BetTotalTurnover = Converter.ToDecimal(raceItem.totalTOLicensee_Origin),
                    Site12BetPositionTakingWinLoss = Converter.ToDecimal(raceItem.PTWLLicensee_Origin),
                    Site12BetCommissionIncome = Converter.ToDecimal(raceItem.commIncomeLicensee_Origin),
                    IBCPositionTakingWinLoss = Converter.ToDecimal(raceItem.PTWLIBC_Origin),
                    IBCCommissionIncome = Converter.ToDecimal(raceItem.CommIncomeIBC_Origin),
                    AmountDueToIBC = Converter.ToDecimal(raceItem.AmountDue2IBC_Origin)
                }; 
                race.Races[0] = raceWinLost;
                race.Races[1] = raceBaseWinLost;
                league.Rows.Add(race);
                league.Races[0].SumTotal(raceWinLost);
                league.Races[1].SumTotal(raceBaseWinLost);
            }

            return league;
        }

        #endregion convert data 

        #region clear all paging session key
        /// <summary>
        /// Clear all paging session key when user generate new report.
        /// </summary>
        private void ClearAllPagingSessionKey()
        {
            for (int i = 0; i < HttpContext.Current.Session.Count; i++)
            {
                if (HttpContext.Current.Session.Keys[i].StartsWith(RacingWinLossByLicenseePagingPrefixSessonKey))
                {
                    HttpContext.Current.Session.Remove(HttpContext.Current.Session.Keys[i]);
                    i--;
                }
            }
        }
        #endregion clear all paging session key

        #region get paging report

        /// <summary>
        /// Get paging report.
        /// </summary>
        /// <param name="fullReport">The full report.</param>
        /// <param name="pagingReport">The paging report.</param>
        /// <param name="criteira">The criteria.</param>
        /// <param name="pagingSessionKey">The paging session key.</param>
        /// <returns>The RacingDailyReport paging report.</returns>
        private RacingDailyReport GetPagingReport(
            RacingDailyReport fullReport,
            RacingDailyReport pagingReport,
            GetRacingDailyReportCriteria criteira,
            string pagingSessionKey)
        {
            pagingReport = new RacingDailyReport();
            pagingReport.Races[1] = fullReport.Races[1];
            pagingReport.ID = fullReport.ID;
            pagingReport = GetPagingReportDetail(fullReport, pagingReport, criteira);
            SessionManager.Set<RacingDailyReport>(pagingSessionKey, pagingReport);

            return pagingReport;
        }

        /// <summary>
        /// Get paging report detail.
        /// </summary>
        /// <param name="fullReport">The full report.</param>
        /// <param name="pagingReport">The paging report.</param>
        /// <param name="criteria">The criteria.</param>
        /// <returns>RacingDailyReport paging report.</returns>
        private RacingDailyReport GetPagingReportDetail(
            RacingDailyReport fullReport, 
            RacingDailyReport pagingReport,
            GetRacingDailyReportCriteria criteria)
        {
            int startIndex = criteria.StartIndex;
            int numberRacesPerPage = criteria.NumberItemsPerPage;
            int numberCurrencies = fullReport.Rows.Count;
            pagingReport = LoopAndGetPagingReport(
                fullReport, 
                pagingReport,
                startIndex,
                numberRacesPerPage, 
                numberCurrencies);

            return pagingReport;
        }

        /// <summary>
        /// Loop and get paging report.
        /// </summary>
        /// <param name="fullReport">The full report.</param>
        /// <param name="pagingReport">The paging report.</param>
        /// <param name="startIndex">The start index.</param>
        /// <param name="numberRacesPerPage">The number races per page.</param>
        /// <param name="numberCurrencies">The number currencies.</param>
        /// <returns>RacingDailyReport paging report.</returns>
        [SuppressMessage("StyleCop.CSharp.StylecopPlus.Rules",
            "SP2101:MethodMustNotContainMoreLinesThan",
            Justification = "Reviewed. Suppression is OK here.")]
        private RacingDailyReport LoopAndGetPagingReport(
            RacingDailyReport fullReport, 
            RacingDailyReport pagingReport,
            int startIndex,
            int numberRacesPerPage, 
            int numberCurrencies)
        {
            int index = 0;
            int numberRacesRemainInLeague = 0;

            while (index < numberCurrencies)
            {
                if (numberRacesPerPage == 0)
                {
                    break;
                }

                RacingDailyReport currency = fullReport.Rows[index];
                RacingDailyReport pagingCurrency = new RacingDailyReport();
                pagingCurrency.Races = currency.Races;
                pagingCurrency.ID = currency.ID;
                pagingCurrency.Name = currency.Name;

                if (currency != null && currency.Rows != null && currency.Rows.Count > 0)
                {
                    int numberLeaguesInCurrency = currency.Rows.Count;

                    for (int j = 0; j < numberLeaguesInCurrency; j++)
                    {
                        RacingDailyReport league = currency.Rows[j];

                        if (league != null && league.Rows != null && league.Rows.Count > 0)
                        {
                            int numberRacesInLeague = league.Rows.Count;

                            if (numberRacesInLeague > startIndex)
                            {
                                numberRacesRemainInLeague = numberRacesInLeague - startIndex;

                                if (numberRacesRemainInLeague <= numberRacesPerPage)
                                {
                                    var pagingLeague = CreateNewPagingLeague(startIndex, numberRacesRemainInLeague, league);
                                    pagingCurrency.Rows.Add(pagingLeague);
                                    numberRacesPerPage -= numberRacesRemainInLeague;
                                    if (numberRacesPerPage < startIndex)
                                    {
                                        numberRacesPerPage = startIndex;
                                        break;
                                    }
                                }
                                else
                                {
                                    var pagingLeague = CreateNewPagingLeague(startIndex, numberRacesPerPage, league);
                                    pagingCurrency.Rows.Add(pagingLeague);
                                    numberRacesPerPage = startIndex;
                                    break;
                                }
                            }
                            else
                            {
                                startIndex -= numberRacesInLeague;
                            }
                        }
                    }
                }

                pagingReport.Rows.Add(pagingCurrency);
                index++;
            }

            return pagingReport;
        }

        /// <summary>
        /// Create new paging league.
        /// </summary>
        /// <param name="startIndex">The start index.</param>
        /// <param name="numberRacesTakingInLeague">The number races to take.</param>
        /// <param name="league">The league.</param>
        /// <returns>The new paging league.</returns>
        private RacingDailyReport CreateNewPagingLeague(
            int startIndex, 
            int numberRacesTakingInLeague, 
            RacingDailyReport league)
        {
            var pagingLeague = new RacingDailyReport();
            pagingLeague.Races = league.Races;
            pagingLeague.ID = league.ID;
            pagingLeague.Name = league.Name;
            pagingLeague.Rows = league.Rows.Skip(startIndex)
                                        .Take(numberRacesTakingInLeague)
                                        .ToList<RacingDailyReport>();
            return pagingLeague;
        }

        #endregion get paging report
    }
}
