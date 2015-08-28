// -----------------------------------------------------------------------
// <copyright file="VirtualSportsWinLoss.cs" company="Nexcel Solutions Vietnam">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace Fanex.AccountingReports.Gaming.Models.VirtualSportsWinLoss
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// The model of Virtual Sports Win Loss report.
    /// </summary>
    [Serializable]
    public class VirtualSportsWinLoss
    {
        public VirtualSportsWinLoss()
        {
            Date = string.Empty;
            Currencies = new List<VirtualSportsWinLossDetail>();
            Rows = new List<VirtualSportsWinLoss>();
        }

        public VirtualSportsWinLoss(string date)
        {
            Date = date;
            Currencies = new List<VirtualSportsWinLossDetail>();
            Rows = new List<VirtualSportsWinLoss>();
        }

        public string Date { get; set; }

        public List<VirtualSportsWinLossDetail> Currencies { get; set; }

        public List<VirtualSportsWinLoss> Rows { get; set; }
    }
}
