// -----------------------------------------------------------------------
// <copyright file="GetCasinoWinLossCriteria.cs" company="Nexcel Solutions Viet Nam">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace Fanex.AccountingReports.Gaming.Criteria.CasinoWinLoss
{
    using System;
    using Fanex.AccountingReports.Core.Services.Impl;
    using Fanex.Data.Repository;

    /// <summary>
    /// The criteria.
    /// </summary>
    public class GetCasinoWinLossCriteria : CriteriaBase
    {
        public DateTime WinLostDate { get; set; }

        public override string GetSettingKey()
        {
            return "Gaming_WinLoss_CS_Detail" + ReportService.GetStoreMappingSuffix(WinLostDate);
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}
