namespace Fanex.AccountingReports.Gaming.Criteria.VirtualSportsWinLoss
{
    using System;
    using Fanex.Data.Repository;

    /// <summary>
    /// The criteria of Virtual Sports Win Loss detail report.
    /// </summary>
    public class GetVirtualSportsWinLossDetailCriteria : CriteriaBase
    {
        public DateTime Date { get; set; }

        public bool ExcludeTest { get; set; }

        public int DrawFilter { get; set; }

        public string SportType { get; set; }

        public string Currency { get; set; }

        public int RoleId { get; set; }

        public bool Debug { get; set; }

        public override string GetSettingKey()
        {
            return "Gaming_VirtualSportsWinLoss_GetReportDetail";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}
