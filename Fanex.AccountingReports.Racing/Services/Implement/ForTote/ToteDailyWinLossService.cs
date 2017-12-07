namespace Fanex.AccountingReports.Racing.Services.Implement
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using Fanex.AccountingReports.Core.Resources;
    using Fanex.AccountingReports.Core.Services.Impl;
    using Fanex.AccountingReports.Core.Web;
    using Fanex.AccountingReports.Racing.Criteria.ForTote;
    using Fanex.AccountingReports.Racing.Models.RacingTote;
    using Fanex.Common;

    /// <summary>
    /// Create racing daily win loss service.
    /// </summary>
    public class ToteDailyWinLossService : ToteDailyWinLossAbstractService
    {
        /// <summary>
        /// Render daily win loss main.
        /// </summary>
        /// <param name="criteria">The racing for tote main criteria.</param>
        /// <returns>The daily win loss.</returns>
        [SuppressMessage("StyleCop.CSharp.StylecopPlus.Rules",
        "SP2101:MethodMustNotContainMoreLinesThan",
        Justification = "Reviewed. Suppression is OK here.")]
        public override IDailyWinLoss GetRacingDailyWinLossMain(GetMainForToteCriteria criteria)
        {
            var racingWinLoss = new WinLossTote
            {
                Total = new WinLossTote
                {
                    PartnerName = Labels.Grand_Total,
                    ChildsInfo = new List<WinLossTote>()
                }
            };
            try
            {
                var currencies = ReportService.ExchangeCurrencies;

                var rawData = GetReportData<dynamic>(criteria);
                if (rawData != null && rawData.Count() > 0)
                {
                    var racingWinLossTotalPool = new WinLossTote();
                    var racingWinLossTotalBase = new WinLossTote
                    {
                        CurrencyName = Labels.BaseRM
                    };
                    var grandTotalDictionary = new Dictionary<int, int>();
                    var partnerIds = rawData.GroupBy(p => new { p.partnerID, p.PartnerName, p.PoolCurrency });

                    foreach (var partner in partnerIds)
                    {
                        var key = partner.Key.partnerID;
                        var partnerName = partner.Key.PartnerName;
                        var poolCurr = Converter.ToInt(partner.Key.PoolCurrency);
                        var curr = from c in rawData
                                   where c.partnerID == key
                                   select c;
                        var racingWinLossByPartner = new WinLossTote
                        {
                            PartnerId = key,
                            PartnerName = partnerName,
                            PoolCurrency = poolCurr,
                            ChildsInfo = new List<WinLossTote>(),
                            Total = new WinLossTote
                            {
                                PartnerName = partnerName + " " + Labels.Total,
                                ChildsInfo = new List<WinLossTote>()
                            }
                        };
                        var winLossPartnerTotalPool = new WinLossTote
                        {
                            CurrencyName = currencies[poolCurr]
                        };
                        var winLossPartnerTotalBase = new WinLossTote
                        {
                            CurrencyName = Labels.BaseRM
                        };
                        foreach (var row in curr)
                        {
                            var racingWinLossPartnerDetail = new WinLossTote()
                            {
                                CurrencyName = Converter.ToString2(row.currencyName),
                                WinTOAgency = Converter.ToDecimal(row.winTO_Agency_Origin),
                                PlaceTOAgency = Converter.ToDecimal(row.placeTO_Agency_Origin),
                                WinPlaceTOAgency = Converter.ToDecimal(row.winplaceTO_Agency_Origin),
                                TotalTOAgency = Converter.ToDecimal(row.totalTO_Agency_Origin),
                                CommIncomeAgency = Converter.ToDecimal(row.commIncome_Agency_Origin),
                                PTWinLossAgency = Converter.ToDecimal(row.PTWL_Agency_Origin),
                                WinTOIBC = Converter.ToDecimal(row.winTO_IBC_Origin),
                                PlaceTOIBC = Converter.ToDecimal(row.placeTO_IBC_Origin),
                                WinPlaceTOIBC = Converter.ToDecimal(row.winplaceTO_IBC_Origin),
                                TotalTOIBC = Converter.ToDecimal(row.totalTO_IBC_Origin),
                                CommIncomeIBC = Converter.ToDecimal(row.commIncome_IBC_origin),
                                ToteFeeIBC = Converter.ToDecimal(row.FeeDue2_origin),
                                CommExpenseIBC = Converter.ToDecimal(row.commIncome_Agency_Origin),
                                PTWinLossIBC = Converter.ToDecimal(row.PTWL_IBC_origin),
                                NetIncomeIBC = Converter.ToDecimal(row.netIncome_IBC_Origin),
                                WinTOPartner = Converter.ToDecimal(row.winTO_Partner_origin),
                                PlaceTOPartner = Converter.ToDecimal(row.placeTO_Partner_origin),
                                WinPlaceTOPartner = Converter.ToDecimal(row.winplaceTO_Partner_origin),
                                TotalTOPartner = Converter.ToDecimal(row.totalTO_Partner_Origin),
                                DividendWonPartner = Converter.ToDecimal(row.dividendWon_origin),
                                CommExpensePartner = Converter.ToDecimal(row.commIncome_IBC_origin),
                                ToteFeePartner = Converter.ToDecimal(row.FeeDue2_origin),
                                AmountDuePartner = Converter.ToDecimal(row.AmountDue_Origin)
                            };
                            racingWinLossByPartner.ChildsInfo.Add(racingWinLossPartnerDetail);

                            var winLossPool = new WinLossTote()
                            {
                                WinTOAgency = Converter.ToDecimal(row.winTO_Agency_PoolCurr),
                                PlaceTOAgency = Converter.ToDecimal(row.placeTO_Agency_PoolCurr),
                                WinPlaceTOAgency = Converter.ToDecimal(row.winplaceTO_Agency_PoolCurr),
                                TotalTOAgency = Converter.ToDecimal(row.totalTO_Agency_PoolCurr),
                                CommIncomeAgency = Converter.ToDecimal(row.commIncome_Agency_PoolCurr),
                                PTWinLossAgency = Converter.ToDecimal(row.PTWL_Agency_PoolCurr),
                                WinTOIBC = Converter.ToDecimal(row.winTO_IBC_PoolCurr),
                                PlaceTOIBC = Converter.ToDecimal(row.placeTO_IBC_PoolCurr),
                                WinPlaceTOIBC = Converter.ToDecimal(row.winplaceTO_IBC_PoolCurr),
                                TotalTOIBC = Converter.ToDecimal(row.totalTO_IBC_PoolCurr),
                                CommIncomeIBC = Converter.ToDecimal(row.commIncome_IBC_PoolCurr),
                                ToteFeeIBC = Converter.ToDecimal(row.FeeDue2_PoolCurr),
                                CommExpenseIBC = Converter.ToDecimal(row.commIncome_Agency_PoolCurr),
                                PTWinLossIBC = Converter.ToDecimal(row.PTWL_IBC_PoolCurr),
                                NetIncomeIBC = Converter.ToDecimal(row.netIncome_IBC_PoolCurr),
                                WinTOPartner = Converter.ToDecimal(row.winTO_Partner_PoolCurr),
                                PlaceTOPartner = Converter.ToDecimal(row.placeTO_Partner_PoolCurr),
                                WinPlaceTOPartner = Converter.ToDecimal(row.winplaceTO_Partner_PoolCurr),
                                TotalTOPartner = Converter.ToDecimal(row.totalTO_Partner_PoolCurr),
                                DividendWonPartner = Converter.ToDecimal(row.dividendWon_PoolCurr),
                                CommExpensePartner = Converter.ToDecimal(row.commIncome_IBC_PoolCurr),
                                ToteFeePartner = Converter.ToDecimal(row.FeeDue2_PoolCurr),
                                AmountDuePartner = Converter.ToDecimal(row.AmountDue_PoolCurr)
                            };
                            winLossPartnerTotalPool.SumRacingWinLoss(winLossPool);
                            var winLossBase = new WinLossTote()
                            {
                                WinTOAgency = Converter.ToDecimal(row.winTO_Agency),
                                PlaceTOAgency = Converter.ToDecimal(row.placeTO_Agency),
                                WinPlaceTOAgency = Converter.ToDecimal(row.winplaceTO_Agency),
                                TotalTOAgency = Converter.ToDecimal(row.totalTO_Agency),
                                CommIncomeAgency = Converter.ToDecimal(row.commIncome_Agency),
                                PTWinLossAgency = Converter.ToDecimal(row.PTWL_Agency),
                                WinTOIBC = Converter.ToDecimal(row.winTO_IBC),
                                PlaceTOIBC = Converter.ToDecimal(row.placeTO_IBC),
                                WinPlaceTOIBC = Converter.ToDecimal(row.winplaceTO_IBC),
                                TotalTOIBC = Converter.ToDecimal(row.totalTO_IBC),
                                CommIncomeIBC = Converter.ToDecimal(row.commIncome_IBC),
                                ToteFeeIBC = Converter.ToDecimal(row.FeeDue2),
                                CommExpenseIBC = Converter.ToDecimal(row.commIncome_Agency),
                                PTWinLossIBC = Converter.ToDecimal(row.PTWL_IBC),
                                NetIncomeIBC = Converter.ToDecimal(row.netIncome_IBC),
                                WinTOPartner = Converter.ToDecimal(row.winTO_Partner),
                                PlaceTOPartner = Converter.ToDecimal(row.placeTO_Partner),
                                WinPlaceTOPartner = Converter.ToDecimal(row.winplaceTO_Partner),
                                TotalTOPartner = Converter.ToDecimal(row.totalTO_Partner),
                                DividendWonPartner = Converter.ToDecimal(row.dividendWon),
                                CommExpensePartner = Converter.ToDecimal(row.commIncome_IBC),
                                ToteFeePartner = Converter.ToDecimal(row.FeeDue2),
                                AmountDuePartner = Converter.ToDecimal(row.AmountDue)
                            };
                            winLossPartnerTotalBase.SumRacingWinLoss(winLossBase);
                            int grandTotalIndex;
                            if (grandTotalDictionary.ContainsKey(poolCurr))
                            {
                                grandTotalIndex = grandTotalDictionary[poolCurr];
                            }
                            else
                            {
                                grandTotalIndex = racingWinLoss.Total.ChildsInfo.Count;
                                grandTotalDictionary.Add(poolCurr, grandTotalIndex);
                                racingWinLoss.Total.ChildsInfo.Add(new WinLossTote
                                {
                                    CurrencyName = currencies[poolCurr]
                                });
                            }

                            racingWinLoss.Total.ChildsInfo[grandTotalIndex].SumRacingWinLoss(winLossPool);
                        }

                        racingWinLossByPartner.Total.ChildsInfo.Add(winLossPartnerTotalPool);
                        racingWinLossByPartner.Total.ChildsInfo.Add(winLossPartnerTotalBase);
                        racingWinLossTotalPool.SumRacingWinLoss(winLossPartnerTotalPool);
                        racingWinLossTotalBase.SumRacingWinLoss(winLossPartnerTotalBase);
                        racingWinLossTotalPool.CurrencyName = winLossPartnerTotalPool.CurrencyName;
                        racingWinLoss.ChildsInfo.Add(racingWinLossByPartner);
                    }

                    racingWinLoss.Total.ChildsInfo.Add(racingWinLossTotalBase);
                }
            }
            catch (Exception ex)
            {
                HandleException(ex);
                throw;
            }

            SessionManager.Set<IDailyWinLoss>(RacingDailyWinLossUtil.RacingWinLossMain, racingWinLoss);
            return racingWinLoss;
        }

        /// <summary>
        /// Render daily win loss detail report.
        /// </summary>
        /// <param name="criteria">The racing for tote detail criteria.</param>
        /// <returns>The daily win loss detail.</returns>
        [SuppressMessage("StyleCop.CSharp.StylecopPlus.Rules",
        "SP2101:MethodMustNotContainMoreLinesThan",
        Justification = "Reviewed. Suppression is OK here.")]
        public override IDailyWinLossDetail GetRacingDailyWinLossDetail(GetDetailForToteCriteria criteria)
        {
            var currencies = ReportService.ExchangeCurrencies;
            var currencyNamebyPool = currencies[criteria.PoolCurrency];

            var racingUKWinLossDetail = new WinLossToteDetail
            {
                ChildsInfo = new List<WinLossToteDetail>(),
                Total = new WinLossToteDetail
                {
                    ChildsInfo = new List<WinLossToteDetail>()
                }
            };
            var racingWinLossPoolGrandTotal = new WinLossToteDetail
            {
                CurrencyName = currencyNamebyPool
            };
            var racingWinLossBaseGrandTotal = new WinLossToteDetail
            {
                CurrencyName = Labels.BaseRM
            };
            var totalDictionary = new Dictionary<int, int>();
            try
            {
                var racingWinLossDetailBaseGrandTotal = new WinLossToteDetail
                {
                    CurrencyName = Labels.BaseRM
                };
                var racingWinLossDetailPoolGrandTotal = new WinLossToteDetail
                {
                    CurrencyName = currencyNamebyPool
                };
                var rawData = this.GetReportData<dynamic>(criteria);
                if (rawData.Count() > 0)
                {
                    var currGroup = rawData.GroupBy(s => new { s.currency, s.currencyName });

                    foreach (var curr in currGroup)
                    {
                        var currencyId = curr.Key.currency;
                        var currencyName = curr.Key.currencyName;
                        var racingWinLossDetailByCurr = new WinLossToteDetail
                        {
                            ChildsInfo = new List<WinLossToteDetail>(),
                            CurrencyId = currencyId,
                            Total = new WinLossToteDetail
                            {
                                LeagueName = string.Format("{0} Total", currencyName),
                                ChildsInfo = new List<WinLossToteDetail>()
                            },
                        };
                        var racingWinLossDetailByCurrGrandTotal = new WinLossToteDetail
                        {
                            CurrencyName = currencyName
                        };
                        var racingWinLossDetailByCurrPoolGrandTotal = new WinLossToteDetail
                        {
                            CurrencyName = currencyNamebyPool
                        };
                        var racingWinLossDetailByCurrBaseGrandTotal = new WinLossToteDetail
                        {
                            CurrencyName = Labels.BaseRM
                        };
                        var meetingGroup = rawData.Where(r => r.currency == currencyId).GroupBy(r => r.MeetingID);
                        foreach (var meeting in meetingGroup)
                        {
                            var leagueName = CacheRef.GetLeagueName(meeting.Key);
                            var racingWinLossByLeague = new WinLossToteDetail
                            {
                                ChildsInfo = new List<WinLossToteDetail>(),
                                LeagueName = leagueName,
                                MeetingId = meeting.Key,
                                Total = new WinLossToteDetail
                                {
                                    ChildsInfo = new List<WinLossToteDetail>()
                                }
                            };
                            var racingWinLossByLeagueTotal = new WinLossToteDetail
                            {
                                LeagueName = string.Format("{0} - {1} {2}", leagueName, currencyName, Labels.Total)
                            };
                            var detail = rawData.Where(s => s.currency == currencyId && s.MeetingID == meeting.Key);
                            foreach (var race in detail)
                            {
                                var racingWinLoss = new WinLossToteDetail()
                                {
                                    RaceNo = Converter.ToInt(race.RaceNo),
                                    WinTOAgency = Converter.ToDecimal(race.winTO_Agency_Origin),
                                    PlaceTOAgency = Converter.ToDecimal(race.placeTO_Agency_Origin),
                                    WinPlaceTOAgency = Converter.ToDecimal(race.winplaceTO_Agency_Origin),
                                    TotalTOAgency = Converter.ToDecimal(race.totalTO_Agency_Origin),
                                    CommIncomeAgency = Converter.ToDecimal(race.commIncome_Agency_Origin),
                                    PTWinLossAgency = Converter.ToDecimal(race.PTWL_Agency_Origin),
                                    WinTOIBC = Converter.ToDecimal(race.winTO_IBC_Origin),
                                    PlaceTOIBC = Converter.ToDecimal(race.placeTO_IBC_Origin),
                                    WinPlaceTOIBC = Converter.ToDecimal(race.winplaceTO_IBC_Origin),
                                    TotalTOIBC = Converter.ToDecimal(race.totalTO_IBC_Origin),
                                    CommIncomeIBC = Converter.ToDecimal(race.commIncome_IBC_origin),
                                    ToteFeeIBC = Converter.ToDecimal(race.FeeDue2_origin),
                                    CommExpenseIBC = Converter.ToDecimal(race.commIncome_Agency_Origin),
                                    PTWinLossIBC = Converter.ToDecimal(race.PTWL_IBC_origin),
                                    NetIncomeIBC = Converter.ToDecimal(race.netIncome_IBC_Origin),
                                    WinTOPartner = Converter.ToDecimal(race.winTO_Partner_origin),
                                    PlaceTOPartner = Converter.ToDecimal(race.placeTO_Partner_origin),
                                    WinPlaceTOPartner = Converter.ToDecimal(race.winplaceTO_Partner_origin),
                                    TotalTOPartner = Converter.ToDecimal(race.totalTO_Partner_Origin),
                                    DividendWonPartner = Converter.ToDecimal(race.dividendWon_origin),
                                    CommExpensePartner = Converter.ToDecimal(race.commIncome_IBC_origin),
                                    ToteFeePartner = Converter.ToDecimal(race.FeeDue2_origin),
                                    AmountDuePartner = Converter.ToDecimal(race.AmountDue_Origin)
                                };
                                racingWinLossByLeague.ChildsInfo.Add(racingWinLoss);
                                racingWinLossByLeagueTotal.SumRacingWinLossDetail(racingWinLoss);
                                racingWinLossDetailByCurrGrandTotal.SumRacingWinLossDetail(racingWinLoss);
                                var racingWinLossDetailPool = new WinLossToteDetail()
                                {
                                    WinTOAgency = Converter.ToDecimal(race.winTO_Agency_PoolCurr),
                                    PlaceTOAgency = Converter.ToDecimal(race.placeTO_Agency_PoolCurr),
                                    WinPlaceTOAgency = Converter.ToDecimal(race.winplaceTO_Agency_PoolCurr),
                                    TotalTOAgency = Converter.ToDecimal(race.totalTO_Agency_PoolCurr),
                                    CommIncomeAgency = Converter.ToDecimal(race.commIncome_Agency_PoolCurr),
                                    PTWinLossAgency = Converter.ToDecimal(race.PTWL_Agency_PoolCurr),
                                    WinTOIBC = Converter.ToDecimal(race.winTO_IBC_PoolCurr),
                                    PlaceTOIBC = Converter.ToDecimal(race.placeTO_IBC_PoolCurr),
                                    WinPlaceTOIBC = Converter.ToDecimal(race.winplaceTO_IBC_PoolCurr),
                                    TotalTOIBC = Converter.ToDecimal(race.totalTO_IBC_PoolCurr),
                                    CommIncomeIBC = Converter.ToDecimal(race.commIncome_IBC_PoolCurr),
                                    ToteFeeIBC = Converter.ToDecimal(race.FeeDue2_PoolCurr),
                                    CommExpenseIBC = Converter.ToDecimal(race.commIncome_Agency_PoolCurr),
                                    PTWinLossIBC = Converter.ToDecimal(race.PTWL_IBC_PoolCurr),
                                    NetIncomeIBC = Converter.ToDecimal(race.netIncome_IBC_PoolCurr),
                                    WinTOPartner = Converter.ToDecimal(race.winTO_Partner_PoolCurr),
                                    PlaceTOPartner = Converter.ToDecimal(race.placeTO_Partner_PoolCurr),
                                    WinPlaceTOPartner = Converter.ToDecimal(race.winplaceTO_Partner_PoolCurr),
                                    TotalTOPartner = Converter.ToDecimal(race.totalTO_Partner_PoolCurr),
                                    DividendWonPartner = Converter.ToDecimal(race.dividendWon_PoolCurr),
                                    CommExpensePartner = Converter.ToDecimal(race.commIncome_IBC_PoolCurr),
                                    ToteFeePartner = Converter.ToDecimal(race.FeeDue2_PoolCurr),
                                    AmountDuePartner = Converter.ToDecimal(race.AmountDue_PoolCurr)
                                };
                                racingWinLossDetailByCurrPoolGrandTotal.SumRacingWinLossDetail(racingWinLossDetailPool);
                                racingWinLossPoolGrandTotal.SumRacingWinLossDetail(racingWinLossDetailPool);

                                var racingWinLossDetailBase = new WinLossToteDetail()
                                {
                                    WinTOAgency = Converter.ToDecimal(race.winTO_Agency),
                                    PlaceTOAgency = Converter.ToDecimal(race.placeTO_Agency),
                                    WinPlaceTOAgency = Converter.ToDecimal(race.winplaceTO_Agency),
                                    TotalTOAgency = Converter.ToDecimal(race.totalTO_Agency),
                                    CommIncomeAgency = Converter.ToDecimal(race.commIncome_Agency),
                                    PTWinLossAgency = Converter.ToDecimal(race.PTWL_Agency),
                                    WinTOIBC = Converter.ToDecimal(race.winTO_IBC),
                                    PlaceTOIBC = Converter.ToDecimal(race.placeTO_IBC),
                                    WinPlaceTOIBC = Converter.ToDecimal(race.winplaceTO_IBC),
                                    TotalTOIBC = Converter.ToDecimal(race.totalTO_IBC),
                                    CommIncomeIBC = Converter.ToDecimal(race.commIncome_IBC),
                                    ToteFeeIBC = Converter.ToDecimal(race.FeeDue2),
                                    CommExpenseIBC = Converter.ToDecimal(race.commIncome_Agency),
                                    PTWinLossIBC = Converter.ToDecimal(race.PTWL_IBC),
                                    NetIncomeIBC = Converter.ToDecimal(race.netIncome_IBC),
                                    WinTOPartner = Converter.ToDecimal(race.winTO_Partner),
                                    PlaceTOPartner = Converter.ToDecimal(race.placeTO_Partner),
                                    WinPlaceTOPartner = Converter.ToDecimal(race.winplaceTO_Partner),
                                    TotalTOPartner = Converter.ToDecimal(race.totalTO_Partner),
                                    DividendWonPartner = Converter.ToDecimal(race.dividendWon),
                                    CommExpensePartner = Converter.ToDecimal(race.commIncome_IBC),
                                    ToteFeePartner = Converter.ToDecimal(race.FeeDue2),
                                    AmountDuePartner = Converter.ToDecimal(race.AmountDue)
                                };
                                racingWinLossDetailByCurrBaseGrandTotal.SumRacingWinLossDetail(racingWinLossDetailBase);
                                racingWinLossBaseGrandTotal.SumRacingWinLossDetail(racingWinLossDetailBase);
                                int totalIndex;
                                if (totalDictionary.ContainsKey(meeting.Key))
                                {
                                    totalIndex = totalDictionary[meeting.Key];
                                }
                                else
                                {
                                    totalIndex = racingUKWinLossDetail.Total.ChildsInfo.Count;
                                    totalDictionary.Add(meeting.Key, totalIndex);
                                    racingUKWinLossDetail.Total.ChildsInfo.Add(new WinLossToteDetail
                                    {
                                        LeagueName = leagueName,
                                        CurrencyName = currencyNamebyPool
                                    });
                                }

                                racingUKWinLossDetail.Total.ChildsInfo[totalIndex].SumRacingWinLossDetail(racingWinLossDetailPool);
                            }

                            racingWinLossByLeague.Total = racingWinLossByLeagueTotal;
                            racingWinLossDetailByCurr.ChildsInfo.Add(racingWinLossByLeague);
                        }

                        if (currencyId != 2)
                        {
                            racingWinLossDetailByCurr.Total.ChildsInfo.Add(racingWinLossDetailByCurrGrandTotal);
                        }

                        racingWinLossDetailByCurr.Total.ChildsInfo.Add(racingWinLossDetailByCurrBaseGrandTotal);
                        racingUKWinLossDetail.ChildsInfo.Add(racingWinLossDetailByCurr);
                    }

                    var racingGrandTotalBaseRM = new WinLossToteDetail
                    {
                        LeagueName = Labels.Grand_Total,
                        ChildsInfo = new List<WinLossToteDetail>()
                    };

                    racingGrandTotalBaseRM.ChildsInfo.Add(racingWinLossPoolGrandTotal);
                    racingGrandTotalBaseRM.ChildsInfo.Add(racingWinLossBaseGrandTotal);
                    racingUKWinLossDetail.Total.ChildsInfo.Add(racingGrandTotalBaseRM);
                }
            }
            catch (Exception ex)
            {
                HandleException(ex);
                throw;
            }

            SessionManager.Set<WinLossToteDetail>(RacingDailyWinLossUtil.RacingWinLossDetail, racingUKWinLossDetail);
            return racingUKWinLossDetail;
        }
    }
}
