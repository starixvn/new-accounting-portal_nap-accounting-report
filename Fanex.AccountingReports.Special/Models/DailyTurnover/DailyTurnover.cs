namespace Fanex.AccountingReports.Special.Models.DailyTurnover
{
    using System;

    /// <summary>
    /// The model of daily turnover.
    /// </summary>
    [Serializable] 
    public class DailyTurnover
    {
        public DailyTurnover()
        {
            Turnover = new TurnoverInformation();
        }

        public string Date { get; set; }

        public TurnoverInformation Turnover { get; set; }

        public string TicketNormalRejectCount { get; set; }
    }
}
