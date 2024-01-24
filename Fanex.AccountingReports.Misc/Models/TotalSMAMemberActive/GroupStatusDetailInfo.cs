namespace Fanex.AccountingReports.Misc.Models.TotalSMAMemberActive
{
    using System;
    using System.Collections.Generic;
    using Fanex.AccountingReports.Core.Models;

    [Serializable]
    public class GroupStatusDetailInfo
    {
        public EntityBase<int, string> GroupHeadline { get; set; }

        public IList<StatusDetailInfo> ChildsInfo { get; set; }
    }
}
