// -----------------------------------------------------------------------
// <copyright file="MetricGaming.cs" company="Nexcel Solutions Vietnam">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace Fanex.AccountingReports.Main.Models.MatchWinLoss2a
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Fanex.AccountingReports.Main.Resources;
    using Fanex.Common;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class MetricGaming
    {
        public MetricGaming()
        {
            ReportRows = new List<MetricGaming>();
        }

        public string Bettype { get; set; }

        public decimal Comm { get; set; }

        public string VirtualComm
        {
            get
            {
                return Labels.NotAvailable;
            }
        }

        public decimal LineTotal { get; set; }

        public string VirtualLineTotal
        {
            get
            {
                return Labels.NotAvailable;
            }
        }

        public string Variance
        {
            get
            {
                return Labels.NotAvailable;
            }
        }

        public decimal Turnover { get; set; }

        public string Margin
        {
            get
            {
                return this.Turnover == 0 ? Labels.NotAvailable : Converter.ToString2(this.LineTotal / this.Turnover);
            }
        }

        public MetricGaming Total { get; set; }

        public List<MetricGaming> ReportRows { get; set; }

        public void CalculateTotal(MetricGaming reportRow)
        {
            this.Total.Comm += reportRow.Comm;
            this.Total.LineTotal += reportRow.LineTotal;
            this.Total.Turnover += reportRow.Turnover;
        }
    }
}
