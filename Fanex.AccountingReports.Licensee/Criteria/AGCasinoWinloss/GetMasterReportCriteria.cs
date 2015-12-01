// -----------------------------------------------------------------------
// <copyright file="GetMasterReportCriteria.cs" company="Nexcel Solutions Vietnam">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace Fanex.AccountingReports.Licensee.Criteria.AGCasinoWinloss
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Fanex.Data.Repository;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class GetMasterReportCriteria : GetMainReportCriteria
    {
        public int CustID { get; set; }

        [SpParam(Ignored = true)]
        public string CustomerName { get; set; }

        public override string GetSettingKey()
        {
            return "Licensee_AGCasinoWinloss_GetMasterReport";
        }
    }
}