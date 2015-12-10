// -----------------------------------------------------------------------
// <copyright file="GetLicenseeListCriteria.cs" company="Microsoft">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace Fanex.AccountingReports.Gaming.Criteria.CasinoGameStatistic
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Fanex.Data.Repository;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class GetLicenseeListCriteria : CriteriaBase, ICriteria
    {
        public string ProductId { get; set; }

        [SpParam(Ignored = true)]
        public string FromDate { get; set; }

        [SpParam(Ignored = true)]
        public string ToDate { get; set; }

        public override string GetSettingKey()
        {
            return "Acc_Rpt_CasinoGameStatistics_LicenseeSel";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}
