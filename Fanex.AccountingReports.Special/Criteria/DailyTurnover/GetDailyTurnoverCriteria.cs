// -----------------------------------------------------------------------
// <copyright file="GetDailyTurnoverCriteria.cs" company="Nexcel Solutions Vietnam">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace Fanex.AccountingReports.Special.Criteria.DailyTurnover
{
    using System;
    using Fanex.NAP.Framework.Data;

    /// <summary>
    /// The criteria of daily turnover.
    /// </summary>
    public class GetDailyTurnoverCriteria : CriteriaBase
    {
        public DateTime FromDate { get; set; }

        public bool IsActual { get; set; }

        public bool ErrorCode { get; set; }

        public string ErrorMessage { get; set; }

        public override string GetSettingKey()
        {
            return "Special_DailyTurnover_GetReport";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}
