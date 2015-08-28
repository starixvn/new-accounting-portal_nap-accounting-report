// -----------------------------------------------------------------------
// <copyright file="ToteDailyWinLossAbstractService.cs" company="Nexcel Solutions Vietnam">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace Fanex.AccountingReports.Racing.Services.Implement
{
    using Fanex.AccountingReports.Core.Services.Impl;
    using Fanex.AccountingReports.Racing.Criteria.ForTote;
    using Fanex.AccountingReports.Racing.Models.RacingTote;

    /// <summary>
    /// Create daily win loss abstract class.
    /// </summary>
    public abstract class ToteDailyWinLossAbstractService : ServiceBase
    {
        public abstract IDailyWinLoss GetRacingDailyWinLossMain(GetMainForToteCriteria criteria);

        public abstract IDailyWinLossDetail GetRacingDailyWinLossDetail(GetDetailForToteCriteria criteria);
    }
}
