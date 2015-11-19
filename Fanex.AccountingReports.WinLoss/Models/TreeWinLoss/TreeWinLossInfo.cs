// -----------------------------------------------------------------------
// <copyright file="TreeWinLossInfo.cs" company="Nexcel Solutions Vietnam">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace Fanex.AccountingReports.WinLoss.Models.TreeWinLoss
{
    using System;
    using System.Collections.Generic;
    using Fanex.AccountingReports.Core.Extensions;
    using Fanex.AccountingReports.Core.Resources;

    /// <summary>
    /// Create Tree Win Loss Info Model. 
    /// </summary>
    [Serializable]
    public class TreeWinLossInfo
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public decimal Turnover { get; set; }

        public decimal WinLoss { get; set; }

        public decimal Comm { get; set; }

        public decimal WinLossTotal { get; set; }

        public decimal BaseRMTurnover { get; set; }

        public decimal BaseRMTotal { get; set; }

        public decimal BaseRMVirtualTotal { get; set; }

        public decimal BaseRMVirtualTurnover { get; set; }

        public decimal BaseRMTOVariancePlus { get; set; }

        public decimal BaseRMTOVarianceMinus { get; set; }

        public decimal BaseRMWLVariancePlus { get; set; }

        public decimal BaseRMWLVarianceMinus { get; set; }

        public decimal BaseRMTOVarianceAbsoluteTotal 
        {
            get
            {
                return Math.Round(Math.Abs(this.BaseRMTOVarianceMinus) + Math.Abs(this.BaseRMTOVariancePlus), 2);
            }
        }

        public decimal BaseRMWinlostVarianceTotal 
        {
            get
            {
                return Math.Round(this.BaseRMWLVariancePlus + this.BaseRMWLVarianceMinus, 2);
            }
        }

        public string BaseRMVarianceMargin 
        {
            get
            {
                if (this.BaseRMTOVarianceAbsoluteTotal != 0)
                {
                    return Math.Round((this.BaseRMWinlostVarianceTotal / this.BaseRMTOVarianceAbsoluteTotal) * 100, 2).ToString();
                }
                else
                {
                    return Labels.NotAvailable;
                }
            }
        }

        public string BaseRMMargin 
        {
            get 
            {
                if (this.BaseRMVirtualTurnover != 0)
                {
                    return Math.Round((this.BaseRMVirtualTotal / this.BaseRMVirtualTurnover) * 100, 2).ToString();
                }
                else
                {
                    return Labels.NotAvailable;
                }
            }
        }

        public decimal VirtualRate { get; set; }
    }
}
