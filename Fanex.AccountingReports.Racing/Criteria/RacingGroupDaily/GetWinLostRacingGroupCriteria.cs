// -----------------------------------------------------------------------
// <copyright file="GetWinLostRacingGroupCriteria.cs" company="Nexcel Solutions Vietnam">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace Fanex.AccountingReports.Racing.Criteria.RacingGroupDaily
{
    using System;
    using Fanex.NAP.Framework.Data;

    /// <summary>
    /// The criteria of Racing Group.
    /// </summary>
    public class GetWinLostRacingGroupCriteria : CriteriaBase
    {
        public DateTime FromDate { get; set; }

        public DateTime ToDate { get; set; }

        public bool IsPagingCallback { get; set; }

        public int StartIndex { get; set; }

        public int DatesPerPage { get; set; }

        public override string GetSettingKey()
        {
            return "Racing_WinLossGroup";
        }

        public override bool IsValid()
        {
            return FromDate <= ToDate;
        }
    }
}
