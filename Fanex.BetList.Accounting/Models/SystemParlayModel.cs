// --------------------------------------------------------------------------
// <copyright file="SystemParlayModel.cs" company="Nexcel Solutions Vietnam">
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
    public class SystemParlayModel
    {
        public SystemParlayModel()
        {
            TicketsDetailList = new List<TicketDetail>();
            SystemParlayChoice = new List<TicketDetail>();
            SystemParlayDetail = new SystemParlayDetail();
        }

        public SystemParlayDetail SystemParlayDetail { get; set; }

        public IList<TicketDetail> TicketsDetailList { get; set; }

        public IList<TicketDetail> SystemParlayChoice { get; set; }
    }
}