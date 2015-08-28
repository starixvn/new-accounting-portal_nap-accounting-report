// --------------------------------------------------------------------------
// <copyright file="GroupStatusDetailInfo.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Kan.Nguyen" date="2013.07.12">Created</change>
// </history>

namespace Fanex.AccountingReports.Misc.Models.TotalSMAMemberActive
{
    using System;
    using System.Collections.Generic;
    using Fanex.AccountingReports.Core.Models;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    [Serializable]
    public class GroupStatusDetailInfo
    {
        public EntityBase<int, string> GroupHeadline { get; set; }

        public IList<StatusDetailInfo> ChildsInfo { get; set; }
    }
}