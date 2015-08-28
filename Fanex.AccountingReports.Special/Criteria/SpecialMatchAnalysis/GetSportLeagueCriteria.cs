// -----------------------------------------------------------------------
// <copyright file="GetSportLeagueCriteria.cs" company="Microsoft">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------
// <history>
//   <change who="Robert.Tran" date="2012.12.26">Created</change>
// </history>

namespace Fanex.AccountingReports.Special.Criteria.SpecialMatchAnalysis
{
    using System;
    using Fanex.AccountingReports.Core.Services.Impl;
    using Fanex.NAP.Framework.Data;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class GetSportLeagueCriteria : CriteriaBase
    {
        public DateTime EventDate { get; set; }

        public DateTime MoveBetsDay
        {
            get
            {
                return ReportService.BalanceUp.MoveBetsDay;
            }
        }

        public DateTime BalanceUpDay
        {
            get
            {
                return ReportService.BalanceUp.BalanceUpDay;
            }
        }

        public override bool IsValid()
        {
            return true;
        }

        public override string GetSettingKey()
        {
            string storeProcedureName = "Special_SpecialMatchAnalysis_GetLeagues";
            return storeProcedureName;
        }
    }
}
