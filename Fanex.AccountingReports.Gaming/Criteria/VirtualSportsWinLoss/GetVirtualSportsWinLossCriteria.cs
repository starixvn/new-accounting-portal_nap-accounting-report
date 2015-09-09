// -----------------------------------------------------------------------
// <copyright file="GetVirtualSportsWinLossCriteria.cs" company="Nexcel Solutions Vietnam">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace Fanex.AccountingReports.Gaming.Criteria.VirtualSportsWinLoss
{
    using System;
    using Fanex.Data.Repository;

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

        [SpParam(Ignored = true)]
        public bool IsPagingCallBack { get; set; }

        [SpParam(Ignored = true)]
        public int StartIndex { get; set; }

        [SpParam(Ignored = true)]
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