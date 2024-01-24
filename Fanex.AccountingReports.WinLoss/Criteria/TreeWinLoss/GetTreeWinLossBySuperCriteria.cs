namespace Fanex.AccountingReports.WinLoss.Criteria.TreeWinLoss
{
    using System;
    using Data;
    using Fanex.AccountingReports.Core.Services.Impl;
    using Fanex.Data.Repository;

    public class GetTreeWinLossBySuperCriteria : CriteriaBase
    {
        [SpParam(Ignored = true)]
        public bool IsNormalMode { get; set; }

        public DateTime FromDate { get; set; }

        public DateTime ToDate { get; set; }

        [SpParam(Ignored = true)]
        public string ProductSelection { get; set; }

        public string Products { get; set; }

        public int SRecommend { get; set; }

        public string SportType { get; set; }

        public override bool IsValid()
        {
            return true;
        }

        public override string GetSettingKey()
        {
            return "WinLoss_TreeWinLoss_GetDataBySuper";
        }
    }
}
