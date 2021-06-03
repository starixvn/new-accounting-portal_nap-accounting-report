namespace Fanex.AccountingReports.WinLoss.Models.TreeWinLoss
{
    using System;
    using Core.Resources;

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

        public decimal BaseRMTOVarianceAbsoluteTotal { get; set; }


        public decimal BaseRMWinlostVarianceTotal { get; set; }

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

        public string VirtualBaseRMMargin
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

        public string BaseRMMargin
        {
            get
            {
                if (this.BaseRMTurnover != 0)
                {
                    return Math.Round((this.BaseRMTotal / this.BaseRMTurnover) * 100, 2).ToString();
                }
                else
                {
                    return Labels.NotAvailable;
                }
            }
        }

        public string VirtualRate { get; set; }

        public decimal ExtraAmount { get; set; }
    }
}