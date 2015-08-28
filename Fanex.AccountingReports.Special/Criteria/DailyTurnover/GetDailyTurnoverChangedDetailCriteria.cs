// -----------------------------------------------------------------------
// <copyright file="GetDailyTurnoverChangedDetailCriteria.cs" company="Nexcel Solutions Vietnam">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace Fanex.AccountingReports.Special.Criteria.DailyTurnover
{
    using System;
    using Fanex.NAP.Framework.Data;

    /// <summary>
    /// The criteria of detail daily turnover.
    /// </summary>
    public class GetDailyTurnoverChangedDetailCriteria : CriteriaBase
    {
        public DateTime FromDate { get; set; }

        public override string GetSettingKey()
        {
            return "Special_DailyTurnoverChangedDetail_GetReport";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}
