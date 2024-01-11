namespace Fanex.AccountingReports.WinLoss.Criteria.TreeWinLoss
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using Data;
    using Fanex.AccountingReports.Core.Services.Impl;
    using Fanex.Data.Repository;

    public class GetTreeWinLossByLevelCriteria : CriteriaBase
    {
        [SpParam(Ignored = true)]
        public bool IsNormalMode { get; set; }

        public DateTime FromDate { get; set; }

        public DateTime ToDate { get; set; }

        public int Currency { get; set; }

        [SpParam(SqlParamName = "srecommend")]
        public int SRecommend { get; set; }

        [SpParam(SqlParamName = "mrecommend")]
        public int MRecommend { get; set; }

        public int Recommend { get; set; }

        public int CustId { get; set; }

        [SpParam(Ignored = true)]
        public string Name { get; set; }

        [SpParam(Ignored = true)]
        public int RoleId { get; set; }

        [SpParam(Ignored = true)]
        public string ProductSelection { get; set; }

        public string Products { get; set; }

        public string SportType { get; set; }

        public override bool IsValid()
        {
            return true;
        }

        [SuppressMessage("StyleCop.CSharp.StylecopPlus.Rules",
        "SP2101:MethodMustNotContainMoreLinesThan",
        Justification = "Reviewed. Suppression is OK here.")]
        public override string GetSettingKey()
        {
            var settingKey = string.Empty;

            switch (this.RoleId)
            {
                case 2:
                    settingKey = "WinLoss_TreeWinLoss_GetDataByAgent";
                    break;

                case 3:
                    settingKey = "WinLoss_TreeWinLoss_GetDataByMaster";
                    break;

                case 4:
                    settingKey = "WinLoss_TreeWinLoss_GetDataBySuper";
                    break;

                case 5:
                    settingKey = "WinLoss_TreeWinLoss_GetDataByCom";
                    break;

                default:
                    settingKey = "WinLoss_TreeWinLoss_GetDataByAgent";
                    break;
            }

            return settingKey;
        }
    }
}
