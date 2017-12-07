namespace Fanex.AccountingReports.Racing.Models.RacingWinLossByLicensee
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// The model of Racing Daily report.
    /// </summary>
    [Serializable]
    public class RacingDailyReport
    {
        public RacingDailyReport()
        {
            Races = new List<RaceWinLost>();
            Races.Add(new RaceWinLost());
            Races.Add(new RaceWinLost());
            Rows = new List<RacingDailyReport>();
            Name = string.Empty;
        }

        public int ID { get; set; }

        public string Name { get; set; }

        public List<RaceWinLost> Races { get; set; }

        public List<RacingDailyReport> Rows { get; set; }
    }
}
