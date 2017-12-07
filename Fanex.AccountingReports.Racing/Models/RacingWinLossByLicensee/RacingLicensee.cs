namespace Fanex.AccountingReports.Racing.Models.RacingWinLossByLicensee
{
    using System;

    /// <summary>
    /// The Racing Licensee.
    /// </summary>
    [Serializable]
    public class RacingLicensee
    {
        public int ID { get; set; }

        public string Name { get; set; }

        public bool HasPositoinTaking { get; set; }
    }
}
