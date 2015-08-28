// --------------------------------------------------------------------------
// <copyright file="BasketballInfo.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Louis.Nguyen" date="2012.10.18">Created</change>
// </history>

namespace Fanex.BetList.Accounting.Models
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    [Serializable]
    public class BasketballInfo : TwoTimesMatchInfo
    {
        public List<string> HomeScores { get; set; }

        public List<string> AwayScores { get; set; }
    }
}