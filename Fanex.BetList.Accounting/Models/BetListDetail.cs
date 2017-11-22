namespace Fanex.BetList.Accounting.Models
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Contains List of BetDetail, and Header Title.
    /// </summary>
    [Serializable]
    public class BetListDetail
    {
        public BetListDetail()
        {
            BetDetails = new List<TicketDetail>();
            Title = string.Empty;
        }

        public IList<TicketDetail> BetDetails { get; set; }

        public string Title { get; set; }

        public decimal TotalWinLoss { get; set; }

        public decimal TotalLicenseeWinLoss { get; set; }
    }
}
