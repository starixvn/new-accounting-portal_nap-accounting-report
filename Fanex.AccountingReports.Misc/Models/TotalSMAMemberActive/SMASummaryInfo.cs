namespace Fanex.AccountingReports.Misc.Models.TotalSMAMemberActive
{
    using System;
    using Fanex.AccountingReports.Core.Models;

    [Serializable]
    public class SMASummaryInfo
    {
        public EntityBase<int, string> Key { get; set; }

        public int Super { get; set; }

        public int Master { get; set; }

        public int Agent { get; set; }
    }
}
