// -----------------------------------------------------------------------
// <copyright file="GetWinLostRacingGroupDailyCriteria.cs" company="Nexcel Solutions Vietnam">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace Fanex.AccountingReports.Racing.Criteria.RacingGroupDaily
{
    using System;
    using Fanex.NAP.Framework.Data;

    /// <summary>
    /// The criteria of Racing Group Daily.
    /// </summary>
    public class GetWinLostRacingGroupDailyCriteria : CriteriaBase
    {
        public DateTime WinLostDate { get; set; }

        public override string GetSettingKey()
        {
            return "Racing_WinLostGroupDaily";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}
