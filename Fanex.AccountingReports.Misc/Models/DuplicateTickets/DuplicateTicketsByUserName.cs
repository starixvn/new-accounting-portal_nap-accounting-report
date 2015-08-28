// -----------------------------------------------------------------------
// <copyright file="DuplicateTicketsByUserName.cs" company="Nexcel Solutions Vietnam">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace Fanex.AccountingReports.Misc.Models.DuplicateTickets
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    [Serializable]
    public class DuplicateTicketsByUserName
    {
        public DuplicateTicketsByUserName()
        {
            TicketInfo = new List<TicketInfo>();
        }

        public string UserName { get; set; }

        public int CustId { get; set; }

        public string BetType { get; set; }

        public string WinLostDate { get; set; }

        public decimal BalanceBeforeBet { get; set; }

        public decimal Stake { get; set; }

        public decimal Odds { get; set; }

        public List<TicketInfo> TicketInfo { get; set; }
    }
}