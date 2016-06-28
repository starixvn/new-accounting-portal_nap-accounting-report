// -----------------------------------------------------------------------
// <copyright file="NumberGameFinancialService.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------
// <history>
//   <change who="Franklin.Nguyen" date="2013.12.04">Created</change>
// </history>

namespace Fanex.AccountingReports.Gaming.Services.Implement.NumberGameFinancial
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using Fanex.AccountingReports.Core.Services.Impl;
    using Fanex.AccountingReports.Core.Web;
    using Fanex.AccountingReports.Gaming.Criteria.NumberGameFinancial;
    using Fanex.AccountingReports.Gaming.Models.NumberGameFinancial;
    using Fanex.AccountingReports.Gaming.Repositories.Implement;
    using Fanex.AccountingReports.Gaming.Repositories.Interface;
    using Fanex.AccountingReports.Gaming.Resources;
    using Fanex.AccountingReports.Gaming.Services.Interface.NumberGameFinancial;
    using Fanex.Common;

    /// <summary>
    /// Number Game Financial Service.
    /// </summary>
    public class NumberGameFinancialService : ServiceBase, INumberGameFinancialService
    {
        private readonly INumberGameFinancialRepository _numberGameFinancialRepository;

        private string _financialReportSessionKey = "Fanex.AccountingReports.Gaming.FinancialReport." +
            (AppSession.CurrentUser != null ? AppSession.CurrentUser.Username : string.Empty);

        public NumberGameFinancialService()
            : this(null)
        {
        }

        public NumberGameFinancialService(INumberGameFinancialRepository numberGameFinancialRepository)
        {
            this._numberGameFinancialRepository = numberGameFinancialRepository ?? new NumberGameFinancialRepository();
        }

        /// <summary>
        /// Gets the number game financial report.
        /// </summary>
        /// <param name="getNumberGameFinancialCriteria">The get number game financial criteria.</param>
        /// <returns>Number Game Financial Report Data.</returns>
        [SuppressMessage("StyleCop.CSharp.StylecopPlus.Rules",
            "SP2101:MethodMustNotContainMoreLinesThan",
            Justification = "Reviewed. Suppression is OK here.")]
        public NumberGameFinancial GetNumberGameFinancial(GetNumberGameFinancialCriteria getNumberGameFinancialCriteria)
        {
            DataSet ds = _numberGameFinancialRepository.GetNumberGameFinancial(getNumberGameFinancialCriteria);

            var result = new NumberGameFinancial
            {
                ReportRows = new List<NumberGameFinancial>()
            };

            try
            {
                var rawData = _numberGameFinancialRepository.GetNumberGameFinancialDetail(ds);

                if (rawData == null)
                {
                    return result;
                }

                rawData
                    .GroupBy(r => r.ReportDate)
                    .ToList()
                    .ForEach(group =>
                    {
                        var numberGameFinancialByDate = new NumberGameFinancial
                        {
                            ReportDate = group.Key,
                            ReportRows = new List<NumberGameFinancial>()
                        };

                        group
                            .GroupBy(l => l.IsLive)
                            .ToList()
                            .ForEach(status =>
                            {
                                var NumberGameFinancialByLive = new NumberGameFinancial
                                {
                                    Status = status.Key ? Labels.Live : Labels.NoneLive,
                                    ReportRows = new List<NumberGameFinancial>()
                                };

                                status.ToList().ForEach(BetType =>
                                {
                                    var reportDetail = new NumberGameFinancial
                                    {
                                        Bettype = BetType.Bettype,
                                        NumberPlayer = BetType.NumberPlayer,
                                        NumberGame = BetType.NumberGame,
                                        TotalBet = BetType.TotalBet,
                                        TotalWin = BetType.TotalWin
                                    };

                                    reportDetail = GetGameInComeAndPayout(reportDetail);
                                    NumberGameFinancialByLive = _numberGameFinancialRepository.GetLiveNumberPlayerAndGameReport(
                                                                                                                                    ds,
                                                                                                                                    NumberGameFinancialByLive,
                                                                                                                                    Converter.ToDateTime(group.Key),
                                                                                                                                    false,
                                                                                                                                    status.Key);
                                    NumberGameFinancialByLive.CalculateGrandTotal(reportDetail);
                                    NumberGameFinancialByLive = GetGameInComeAndPayout(NumberGameFinancialByLive);
                                    NumberGameFinancialByLive.ReportRows.Add(reportDetail);
                                });

                                numberGameFinancialByDate = _numberGameFinancialRepository.GetLiveNumberPlayerAndGameReport(
                                                                                                                            ds,
                                                                                                                            numberGameFinancialByDate,
                                                                                                                            Converter.ToDateTime(group.Key),
                                                                                                                            true,
                                                                                                                            status.Key);
                                numberGameFinancialByDate.CalculateGrandTotal(NumberGameFinancialByLive);
                                numberGameFinancialByDate = GetGameInComeAndPayout(numberGameFinancialByDate);
                                numberGameFinancialByDate.ReportRows.Add(NumberGameFinancialByLive);
                            });

                        _numberGameFinancialRepository.GetGrandTotalNumberPlayerAndGameReport(ds, result);
                        result.CalculateGrandTotal(numberGameFinancialByDate);
                        result = GetGameInComeAndPayout(result);
                        result.ReportRows.Add(numberGameFinancialByDate);
                    });

                SessionManager.Set<NumberGameFinancial>(_financialReportSessionKey, result);
                return result;
            }
            catch (Exception ex)
            {
                HandleException(ex);
                throw;
            }
        }

        private NumberGameFinancial GetGameInComeAndPayout(NumberGameFinancial numberGameFinancial)
        {
            var totalBet = Converter.ToDecimal(numberGameFinancial.TotalBet);
            var totalWin = Converter.ToDecimal(numberGameFinancial.TotalWin);

            numberGameFinancial.GameIncome = totalBet - totalWin;
            numberGameFinancial.Payout = totalBet == 0 ? Labels.NotAvailable : (totalWin / totalBet).ToString();

            return numberGameFinancial;
        }
    }
}