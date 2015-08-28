// -----------------------------------------------------------------------
// <copyright file="DailyTurnover.cs" company="Nexcel Solutions Vietnam">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

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
