using System;

namespace Fanex.BetList.Accounting.Models
{
    public class CashOutTicket
    {
        public int BetType { get; set; }

        public long TransId { get; set; }

        public long RefNo { get; set; }

        public string Status { get; set; }

        public decimal Stake { get; set; }

        public DateTime TransDate { get; set; }

        public int CustId { get; set; }

        public decimal Price { get; set; }

        public decimal WinLost { get; set; }

        public string StatusHtml { get; set; }
    }
}
