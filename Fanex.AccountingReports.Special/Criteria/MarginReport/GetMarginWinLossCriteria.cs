// -----------------------------------------------------------------------
// <copyright file="GetMarginWinLossCriteria.cs" company="Nexcel Slutions Vietnam">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace Fanex.AccountingReports.Special.Criteria.MarginReport
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Fanex.NAP.Framework.Data;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class GetMarginWinLossCriteria : CriteriaBase
    {
        [SpParam(SqlParamName = "actual_stake")]
        public decimal ActualStake { get; set; }

        public override string GetSettingKey()
        {
            return "Special_MarginReport_BetRatioByUser";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}
