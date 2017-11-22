namespace Fanex.AccountingReports.Special.Models.DailyTurnover
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// The changed detail of turnover.
    /// </summary>
    [Serializable]
    public class DailyDetailChangedTurnover
    {
        public DailyDetailChangedTurnover()
        {
            TurnoverAmount = new List<TurnoverInformation>();
            TurnoverDifference = new List<TurnoverInformation>();
        }

        public string Date { get; set; }

        public string ActionName { get; set; }

        public string Username { get; set; }

        public string Match { get; set; }

        public List<TurnoverInformation> TurnoverAmount { get; set; }

        public List<TurnoverInformation> TurnoverDifference { get; set; }
    }
}
