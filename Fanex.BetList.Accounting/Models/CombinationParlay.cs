// --------------------------------------------------------------------------
// <copyright file="CombinationParlay.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="" date="2012.12.07">Created</change>
// </history>

namespace Fanex.BetList.Accounting.Models
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class CombinationParlay
    {
        public IList<GroupParlayEvent> GroupParlayEvents { get; set; }

        public ParlayEvent Total { get; set; }

        public decimal OutStanding { get; set; }
    }
}