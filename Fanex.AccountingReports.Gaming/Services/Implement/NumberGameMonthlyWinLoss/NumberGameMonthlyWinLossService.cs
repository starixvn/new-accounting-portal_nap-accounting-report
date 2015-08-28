// -----------------------------------------------------------------------
// <copyright file="NumberGameMonthlyWinLossService.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------
// <history>
//   <change who="Franklin.Nguyen" date="2013.10.12">Created</change>
// </history>

namespace Fanex.AccountingReports.Gaming.Services.Implement.NumberGameMonthlyWinLoss
{
    using System;
    using Fanex.AccountingReports.Core.Services.Impl;
    using Fanex.AccountingReports.Core.Web;
    using Fanex.AccountingReports.Gaming.Criteria.NumberGameMonthlyWinLoss;
    using Fanex.AccountingReports.Gaming.Models.NumberGameMonthlyWinLoss;
    using Fanex.AccountingReports.Gaming.Repositories.Implement;
    using Fanex.AccountingReports.Gaming.Repositories.Interface;
    using Fanex.AccountingReports.Gaming.Services.Interface.NumberGameMonthlyWinLoss;

    /// <summary>
    /// Number Game Monthly Win Loss Service.
    /// </summary>
    public class NumberGameMonthlyWinLossService : ServiceBase, INumberGameMonthlyWinLossService
    {
        private string _numberGameMonthlyWinLossReportSessionKey = "Fanex.AccountingReports.Gaming.NumberGameMonthlyWinLoss." +
            (AppSession.CurrentUser != null ? AppSession.CurrentUser.Username : string.Empty);

        private INumberGameMonthlyWinLossRepository _numberGameMonthlyWinLossRepository;

        public NumberGameMonthlyWinLossService()
            : this(null)
        {
        }

        public NumberGameMonthlyWinLossService(INumberGameMonthlyWinLossRepository numberGameMonthlyWinLossRepository)
        {
            this._numberGameMonthlyWinLossRepository = numberGameMonthlyWinLossRepository ?? new NumberGameMonthlyWinLossRepository();
        }

        /// <summary>
        /// Gets the number game monthly win loss.
        /// </summary>
        /// <param name="getNumberGameMonthlyWinLossCriteria">The get number game monthly win loss criteria.</param>
        /// <returns>Number Game Monthly Win Loss Report.</returns>
        public NumberGameMonthlyWinLoss GetNumberGameMonthlyWinLoss(NumberGameMonthlyWinLossCriteria getNumberGameMonthlyWinLossCriteria)
        {
            var result = new NumberGameMonthlyWinLoss();

            try
            {
                var rawData = _numberGameMonthlyWinLossRepository.GetNumberGameMonthlyWinLoss(getNumberGameMonthlyWinLossCriteria);

                if (rawData == null)
                {
                    return result;
                }

                result.ReportRows = rawData;

                foreach (var row in result.ReportRows)
                {
                    result.CalculateTotal(row);
                }

                SessionManager.Set<NumberGameMonthlyWinLoss>(_numberGameMonthlyWinLossReportSessionKey, result);
                return result;
            }
            catch (Exception ex)
            {
                HandleException(ex);
                throw;
            }
        }
    }
}