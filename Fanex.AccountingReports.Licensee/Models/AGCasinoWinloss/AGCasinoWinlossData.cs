// -----------------------------------------------------------------------
// <copyright file="AGCasinoWinlossData.cs" company="Nexcel Solutions Vietnam">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace Fanex.AccountingReports.Licensee.Models.AGCasinoWinloss
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    [Serializable]
    public class AGCasinoWinlossData
    {
        public AGCasinoWinlossData()
        {
            this.Data = new List<AGCasinoWinloss>();
            this.Total = new List<AGCasinoWinloss>();
            this.GrandTotal = new List<AGCasinoWinloss>();
        }

        public List<AGCasinoWinloss> Data { get; set; }

        public List<AGCasinoWinloss> Total { get; set; }

        public List<AGCasinoWinloss> GrandTotal { get; set; }

        public int TotalItem { get; set; }
    }
}
