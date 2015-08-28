// -----------------------------------------------------------------------
// <copyright file="AGCasinoWinloss.cs" company="Nexcel Solutions Vietnam">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace Fanex.AccountingReports.Licensee.Models.AGCasinoWinloss
{
    using System;
    using System.Diagnostics.CodeAnalysis;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    [Serializable]
    public class AGCasinoWinloss
    {
        public AGCasinoWinloss()
        { 
        }

        [SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", Justification = "ID is valid name.")]
        public AGCasinoWinloss(AGCasinoWinloss refRow)
        {
            this.Date = refRow.Date;
            this.CustID = refRow.CustID;
            this.UserName = refRow.UserName;
            this.CurrencyID = refRow.CurrencyID;
            this.CurrencyName = refRow.CurrencyName;
            this.Turnover = refRow.Turnover;
            this.TotalTrans = refRow.TotalTrans;
            this.CustomerWinLoss = refRow.CustomerWinLoss;
            this.LicenseeWinLoss = refRow.LicenseeWinLoss;
            this.AmountDueToAsiaGamings = refRow.AmountDueToAsiaGamings;
        }

        public string Date { get; set; }

        [SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", Justification = "ID is valid name.")]
        public int CustID { get; set; }

        public string UserName { get; set; }

        [SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", Justification = "ID is valid name.")]
        public int CurrencyID { get; set; }

        public string CurrencyName { get; set; }

        public double Turnover { get; set; }

        public int TotalTrans { get; set; }

        public double CustomerWinLoss { get; set; }

        public double LicenseeWinLoss { get; set; }

        public double AmountDueToAsiaGamings { get; set; }
    }
}
