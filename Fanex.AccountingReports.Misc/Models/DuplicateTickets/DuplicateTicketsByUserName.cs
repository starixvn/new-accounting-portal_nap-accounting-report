namespace Fanex.AccountingReports.Misc.Models.DuplicateTickets
{
    using System;
    using System.Collections.Generic;

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

        public int BetTypeId { get; set; }

        public string WinLostDate { get; set; }

        public decimal BalanceBeforeBet { get; set; }

        public decimal Stake { get; set; }

        public decimal Odds { get; set; }

        public List<TicketInfo> TicketInfo { get; set; }
    }
}
