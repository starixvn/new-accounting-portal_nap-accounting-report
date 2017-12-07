namespace Fanex.AccountingReports.Licensee.Models.PeakConcurrentUser
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    [Serializable]
    public class PeakConcurrentUser
    {
        public PeakConcurrentUser()
        {
            ListConcurrentUser = new List<ConcurrentUser>();
        }

        public List<ConcurrentUser> ListConcurrentUser { get; set; }

        public string AverageUsers { get; set; }

        public string Total { get; set; }

        public string MaxUsers { get; set; }

        public string MinUsers { get; set; }
    }
}
