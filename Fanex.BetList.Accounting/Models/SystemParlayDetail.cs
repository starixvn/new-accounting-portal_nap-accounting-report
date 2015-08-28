// --------------------------------------------------------------------------
// <copyright file="SystemParlayDetail.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="" date="2012.12.07">Created</change>
// </history>

namespace Fanex.BetList.Accounting.Models
{
    using System.Collections.Generic;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class SystemParlayDetail
    {
        public SystemParlayDetail()
        {
            EventList = new List<Event>();
        }

        public List<Event> EventList { get; set; }

        public decimal SubTotal { get; set; }
    }
}