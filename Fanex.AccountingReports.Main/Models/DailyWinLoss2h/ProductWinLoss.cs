// -----------------------------------------------------------------------
// <copyright file="ProductWinLoss.cs" company="Nexcel Solutions Vietnam">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace Fanex.AccountingReports.Main.Models.DailyWinLoss2h
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Product win loss.
    /// </summary>
    [Serializable]
    public class ProductWinLoss
    {
        public ProductWinLoss()
        {
            ReportRows = new List<ProductWinLoss>();
        }

        public int ProductId { get; set; }

        public int Bettype { get; set; }

        public int Site { get; set; }

        public decimal WinLost { get; set; }

        public decimal Position { get; set; }

        public decimal Commission { get; set; }

        public decimal LineTotal { get; set; }

        public decimal Turnover { get; set; }

        public decimal ExtraPositionTaking { get; set; }

        public decimal AfterExtraPositionTaking { get; set; }

        public List<ProductWinLoss> ReportRows { get; set; }
    } 
}