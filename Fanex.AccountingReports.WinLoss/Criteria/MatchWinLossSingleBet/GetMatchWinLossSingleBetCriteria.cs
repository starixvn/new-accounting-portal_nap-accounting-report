// -----------------------------------------------------------------------
// <copyright file="GetMatchWinLossSingleBetCriteria.cs" company="Nexcel Solutions Vietnam">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace Fanex.AccountingReports.WinLoss.Criteria.MatchWinLossSingleBet
{
    using System;
    using Fanex.Data.Repository;

    /// <summary>
    /// The criteria.
    /// </summary>
    public class GetMatchWinLossSingleBetCriteria : CriteriaBase
    {
        public DateTime FromDate { get; set; }

        public DateTime ToDate { get; set; }

        public override string GetSettingKey()
        {
            return "WinLoss_MatchWinLossSingleBet_GetReport";
        }

        public override bool IsValid()
        {
            return FromDate <= ToDate;
        }
    }
}
