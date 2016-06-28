// -----------------------------------------------------------------------
// <copyright file="RacingDetailBetListService.cs" company="Nexcel Solutions Viet Nam">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace Fanex.AccountingReports.Racing.Services.Implement.RacingWinLossByLicensee
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using Fanex.AccountingReports.Core.Extensions;
    using Fanex.AccountingReports.Core.Services.Impl;
    using Fanex.AccountingReports.Core.Web;
    using Fanex.AccountingReports.Racing.Criteria.RacingWinLossByLicensee;
    using Fanex.AccountingReports.Racing.Models.RacingWinLossByLicensee;
    using Fanex.AccountingReports.Racing.Resources;
    using Fanex.AccountingReports.Racing.Services.Interface.RacingWinLossByLicensee;
    using Fanex.Common;

    /// <summary>
    /// The implement of get detail bet list service.
    /// </summary>
    public class RacingDetailBetListService : ServiceBase, IRacingDetailBetListService
    {
        public static readonly string WIN = "WON";
        public static readonly string LOSE = "LOSE";
        public static readonly string REFUND = "Refund";

        private const string RacingDetailBetListPrefixSessionKey =
                            "Fanex.AccountingReports.Racing.RacingWinLossByLicensee.DetailBetList.";

        private string _racingDetailBetListSessionKey = string.Empty;

        public RacingDetailBetListService()
        {
            if (AppSession.CurrentUser != null)
            {
                _racingDetailBetListSessionKey = RacingDetailBetListPrefixSessionKey +
                                                        AppSession.CurrentUser.Username;
            }
            else
            {
                _racingDetailBetListSessionKey = RacingDetailBetListPrefixSessionKey;
            }
        }

        /// <summary>
        /// Get detail bet list.
        /// </summary>
        /// <param name="criteria">The criteria.</param>
        /// <returns>The list DetailBetList.</returns>
        public List<DetailBetList> GetDetailBetList(GetDetailBetListCriteria criteria)
        {
            try
            {
                var report = new List<DetailBetList>();

                report = GetAndConvertData(report, criteria);
                SessionManager.Set<List<DetailBetList>>(_racingDetailBetListSessionKey, report);

                return report;
            }
            catch (Exception ex)
            {
                this.HandleException(ex);
                throw;
            }
        }

        /// <summary>
        /// Get and convert data from database.
        /// </summary>
        /// <param name="report">The report.</param>
        /// <param name="criteria">The criteria.</param>
        /// <returns>The list DetailBetList.</returns>
         [SuppressMessage("StyleCop.CSharp.StylecopPlus.Rules",
            "SP2101:MethodMustNotContainMoreLinesThan",
            Justification = "Reviewed. Suppression is OK here.")]
        private List<DetailBetList> GetAndConvertData(
            List<DetailBetList> report, 
            GetDetailBetListCriteria criteria)
        {
            var rowsData = GetReportData<dynamic>(criteria);
            var totalBet = new DetailBetList();

            foreach (var row in rowsData)
            {
                DateTime date = Converter.ToDateTime(row.transdate);
                int teamid = Converter.ToInt(row.teamid);
                string horseTeamName = CacheRef.GetHorseTeamName(teamid);
                int betType = Converter.ToInt(row.bettype);
                DetailBetList bet = new DetailBetList
                {
                    Member = Converter.ToString2(row.username),
                    Username = Converter.ToString2(row.username2),
                    TransactionID = Converter.ToString2(row.transid),
                    TransactionDate = date.ToLongFormatedDateWithSlash(),
                    BetType = CacheRef.GetBettype(betType),
                    HorseBetType = Converter.ToString2(row.matchcode) + "-" + horseTeamName,
                    IsShowExchangeRate = Converter.ToBool(row.isShowOTExRate),
                    Stake = Converter.ToDecimal(row.stake),
                    MemberWinLoss = Converter.ToDecimal(row.winlost),
                    MemberCommissionIncome = Converter.ToDecimal(row.playercomm),
                    LicenseeWinLossPositionTaking = Converter.ToDecimal(row.WLPTLicensee),
                    LicenseeCommissionIncome = Converter.ToDecimal(row.commIncomeLicensee),
                    IBCWinLossPositionTaking = Converter.ToDecimal(row.WLPTIBC),
                    IBCCommissionIncome = Converter.ToDecimal(row.CommIncomeIBC),
                    Dividend = Converter.ToDecimal(row.dividend)
                };

                if (bet.IsShowExchangeRate)
                {
                    bet.ActualRate = Converter.ToDecimal(row.virtualrate);
                    bet.ExchangeRate = Labels.ExchangeRate;
                }

                switch (betType)
                {
                    case 31:
                    case 32:
                        bet.Status = Converter.ToString2(row.status);
                        bet.IsCombineBetType = false;

                        if (bet.Status.Equals(WIN) || bet.Status.Equals(REFUND))
                        {
                            bet.Odds = Converter.ToDecimal(row.odds);
                        }

                        bet.Odds_Str = bet.Odds == 0 ? "-" : bet.Odds.ToString("0.#####");

                        break;
                    default:
                        bet.WinStatus = Converter.ToString2(row.winStatus);
                        bet.PlaceStatus = Converter.ToString2(row.placeStatus);
                        bet.WinOdds = bet.WinStatus.Equals(WIN) ? Converter.ToDecimal(row.winOdds) : 0;
                        bet.PlaceOdds = bet.PlaceStatus.Equals(LOSE) || bet.PlaceStatus.Equals(REFUND)
                                            ? 0 : Converter.ToDecimal(row.placeOdds);
                        bet.IsCombineBetType = true;
                        bet.Odds_Str = bet.WinOdds.ToString("0.#####");
                        break;
                }

                report.Add(bet);
                totalBet.SumTotal(bet);
            }

            report.Add(totalBet);

            return report;
        }
    }
}
