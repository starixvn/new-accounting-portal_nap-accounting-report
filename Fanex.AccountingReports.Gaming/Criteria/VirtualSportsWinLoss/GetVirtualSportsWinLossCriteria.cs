// -----------------------------------------------------------------------
// <copyright file="GetVirtualSportsWinLossCriteria.cs" company="Nexcel Solutions Vietnam">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace Fanex.AccountingReports.Gaming.Criteria.VirtualSportsWinLoss
{
    using System;
    using Fanex.NAP.Framework.Data;

    /// <summary>
    /// The criteria of Virtual Sports Win Loss report.
    /// </summary>
    public class GetVirtualSportsWinLossCriteria : CriteriaBase
    {
        public DateTime FromDate { get; set; }

        public DateTime ToDate { get; set; }

        public bool ExcludeTest { get; set; }

        public int DrawFilter { get; set; }

        public string SportType { get; set; }

        public bool IsPagingCallBack { get; set; }

        public int StartIndex { get; set; }

        public int UsersPerPage { get; set; }

        public override string GetSettingKey()
        {
            return "Gaming_VirtualSportsWinLoss_GetReport";
        }

        public override bool IsValid()
        {
            return this.FromDate <= this.ToDate;
        }
    }
}
