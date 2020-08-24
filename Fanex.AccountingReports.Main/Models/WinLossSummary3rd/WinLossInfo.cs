namespace Fanex.AccountingReports.Main.Models.WinLossSummary3rd
{
    using System;
    using System.Web.Script.Serialization;
    using Fanex.AccountingReports.Core.Extensions;
    using Fanex.AccountingReports.Main.Resources;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    [Serializable]
    public class WinLossInfo
    {
        private decimal? _variance;
        private decimal? _realMargin;
        private decimal? _virtualMargin;

        public int Id { get; set; }

        public int SportId { get; set; }

        public string Name { get; set; }

        public int LiveIndicator { get; set; }

        [ScriptIgnore]
        public int IBC { get; set; }

        public bool IsHedge { get; set; }

        [ScriptIgnore]
        public string Status { get; set; }

        [ScriptIgnore]
        public decimal WinLoss { get; set; }

        [ScriptIgnore]
        public decimal Position { get; set; }

        [ScriptIgnore]
        public decimal Commission { get; set; }

        [ScriptIgnore]
        public decimal? RealLineTotal { get; set; }

        [ScriptIgnore]
        public decimal? VirtualLineTotal { get; set; }

        [ScriptIgnore]
        public decimal? Variance
        {
            get
            {
                if (!_variance.HasValue || _variance.Value == 0)
                {
                    _variance = this.RealLineTotal - this.VirtualLineTotal;
                }

                return _variance;
            }

            set
            {
                _variance = value;
            }
        }

        [ScriptIgnore]
        public decimal? RealTurnover { get; set; }

        [ScriptIgnore]
        public decimal? VirtualTurnover { get; set; }

        [ScriptIgnore]
        public decimal? RealMargin
        {
            get
            {
                if (!this.RealTurnover.HasValue || this.RealTurnover == 0)
                {
                    return null;
                }
                else if (!_realMargin.HasValue || _realMargin.Value == 0)
                {
                    _realMargin = this.RealLineTotal / this.RealTurnover;
                }

                return _realMargin;
            }

            set
            {
                this._realMargin = value;
            }
        }

        [ScriptIgnore]
        public decimal? VirtualMargin
        {
            get
            {
                if (!this.VirtualTurnover.HasValue || this.VirtualTurnover == 0)
                {
                    return null;
                }
                else if (!_virtualMargin.HasValue || _virtualMargin.Value == 0)
                {
                    _virtualMargin = this.VirtualLineTotal / this.VirtualTurnover;
                }

                return _virtualMargin;
            }

            set
            {
                this._virtualMargin = value;
            }
        }

        //// Fields to display

        public string WinLossStr
        {
            get
            {
                return WinLoss.Format(2);
            }
        }

        public string PositionStr
        {
            get
            {
                return Position.Format(2);
            }
        }

        public string CommissionStr
        {
            get
            {
                return Commission.Format(2);
            }
        }

        public string RealLineTotalStr
        {
            get
            {
                if (RealLineTotal.HasValue)
                {
                    return RealLineTotal.Value.Format(2);
                }

                return Labels.NotAvailable;
            }
        }

        public string VirtualLineTotalStr
        {
            get
            {
                if (VirtualLineTotal.HasValue)
                {
                    return VirtualLineTotal.Value.Format(2);
                }

                return Labels.NotAvailable;
            }
        }

        public string VarianceStr
        {
            get
            {
                if (Variance.HasValue)
                {
                    return Variance.Value.Format(2);
                }

                return Labels.NotAvailable;
            }
        }

        public string RealTurnoverStr
        {
            get
            {
                if (RealTurnover.HasValue)
                {
                    return RealTurnover.Value.Format(2);
                }

                return Labels.NotAvailable;
            }
        }

        public string VirtualTurnoverStr
        {
            get
            {
                if (VirtualTurnover.HasValue)
                {
                    return VirtualTurnover.Value.Format(2);
                }

                return Labels.NotAvailable;
            }
        }

        public string RealMarginStr
        {
            get
            {
                if (RealMargin.HasValue)
                {
                    return (RealMargin.Value * 100).Format(2);
                }

                return Labels.NotAvailable;
            }
        }

        public string VirtualMarginStr
        {
            get
            {
                if (VirtualMargin.HasValue)
                {
                    return (VirtualMargin.Value * 100).Format(2);
                }

                return Labels.NotAvailable;
            }
        }
    }
}