// --------------------------------------------------------------------------
// <copyright file="INumberGameWinLossByCurrencyService.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="" date="2012.12.07">Created</change>
// </history>

namespace Fanex.AccountingReports.Gaming.Services.NumberGameWinLossByCurrency
{
    using System.Collections.Generic;
    using Fanex.AccountingReports.Core.Models;
    using Fanex.AccountingReports.Gaming.Criteria.NumberGameWinLossByCurrency;
    using Fanex.AccountingReports.Gaming.Models.NumberGameWinLossByCurrency;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public interface INumberGameWinLossByCurrencyService
    {
        List<NumberGameWinLossByCurrencyMain> GetNumberGameWinLossByCurrencyMain(NumberGameWinLossByCurrencyMainCriteria numberGameWLCMainCretia);

        List<NumberGameWinLossByCurrencyDetail> GetNumberGameWinLossByCurrencyDetail(NumberGameWinLossByCurrencyDetailCriteria numberGameWLCDetailCretia);

        List<EntityBase<int, string>> GetCurrencies();

        void GetExportExcelNumberGameWinLossByCurMain(NumberGameWinLossByCurrencyMainCriteria numberGameWLCMainCretia);

        void GetExportExcelNumberGameWinLossByCurDetail(NumberGameWinLossByCurrencyDetailCriteria numberGameWLCDetailCretia);
    }
}