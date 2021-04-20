namespace Fanex.AccountingReports.Main.Models.WinLossSummary3rd
{
    using Fanex.AccountingReports.Core.Constants;
    using Fanex.AccountingReports.Main.Common;
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    [Serializable]
    public class SportTypeWinLossInfo
    {
        private static readonly List<int> SportIdsWithoutDetails = new List<int>
        {
            MainConstants.WinLossSumamryCasinoSportId,
            MainConstants.WinLossSummaryLiveCasinoSportId,
            MainConstants.WinLossSummaryBingoSportId,
            MainConstants.WinLossSummaryP2PSportId,
            MainConstants.WinLossSummaryMixParlaySportId,
            MainConstants.WinLossSummarySystemParlaySportId,
            MainConstants.WinLossSummaryKenoSportId,
            SportContants.GoldDeluxe,
            SportContants.MicrogamingLiveCasino,
            SportContants.MicrogamingRNGCasino,
            SportContants.AGCasino,
            SportContants.Macau,
            SportContants.Bettrade,
            SportContants.ColossusBet,
            SportContants.Allbet,
            SportContants.MacauGames,
            SportContants.NG2,
            SportContants.FishingWorld,
            SportContants.CashOut,
            SportContants.Lottery,
            MainConstants.WinLossSummaryBetradarVirtualSportId,
            SportContants.Lucky3,
            SportContants.Exchange,
            SportContants.Pragmatic,
            SportContants.PockerGame,
            SportContants.CardClub,
            SportContants.NPH,
            SportContants.VirtualGames,
            SportContants.Spedagaming,
            SportContants.SAGaming,
            SportContants.AECasino,
            SportContants.SabaClub,
            SportContants.Togel4D,
            SportContants.TableGames,
            SportContants.BBin,
            SportContants.RNGWar,
            SportContants.ION,
            SportContants.IBCBetLiveCasino,
            SportContants.PGSoft,
            SportContants.SabaClubFreeBet,
            SportContants.WM,
            SportContants.Joker,
            SportContants.SBAjusment,
            SportContants.VirtualSports3,
            SportContants.GDG,
            SportContants.MaxGame,
            SportContants.BG,
            SportContants.Habanero,
            SportContants.SportsLottery,
            SportContants.PPLiveCasino,
            SportContants.CG
        };

        public SportTypeWinLossInfo()
        {
            BetTypeWinLosses = new List<BetTypeWinLossInfo>();
            Total = new WinLossInfo
            {
                RealLineTotal = 0,
                VirtualLineTotal = 0,
                RealTurnover = 0,
                VirtualTurnover = 0,
            };
            TotalForNewCricket = new WinLossInfo
            {
                RealLineTotal = 0,
                VirtualLineTotal = 0,
                RealTurnover = 0,
                VirtualTurnover = 0,
            };
        }

        public int Id { get; set; }

        public string Name { get; set; }

        public bool CanShowDetailLink
        {
            get
            {
                return !SportIdsWithoutDetails.Contains(Id) && Id != 0;
            }
        }

        public List<BetTypeWinLossInfo> BetTypeWinLosses { get; set; }

        /// <summary>
        /// Gets or sets IBC Total = 8RM + BARM.
        /// </summary>
        /// <value>
        /// The IBC total.
        /// </value>
        public WinLossInfo IBCTotal { get; set; }

        public WinLossInfo Total { get; set; }

        public WinLossInfo TotalForNewCricket { get; set; }

        public WinLossInfo HedgeTotal { get; set; }

        public WinLossInfo NormalTotal { get; set; }

        /// <summary>
        /// Calculates the total.
        /// </summary>
        /// <param name="item">The item value.</param>
        public void CalculateTotalNewCricket(WinLossInfo item)
        {
            if (item == null)
            {
                return;
            }

            this.TotalForNewCricket.WinLoss += item.WinLoss;
            this.TotalForNewCricket.Position += item.Position;
            this.TotalForNewCricket.Commission += item.Commission;
            this.TotalForNewCricket.RealLineTotal += item.RealLineTotal;
            this.TotalForNewCricket.VirtualLineTotal += item.VirtualLineTotal;
            this.TotalForNewCricket.RealTurnover += item.RealTurnover;
            this.TotalForNewCricket.VirtualTurnover += item.VirtualTurnover;
        }

        /// <summary>
        /// Calculates the total.
        /// </summary>
        /// <param name="item">The item value.</param>
        public void CalculateTotal(WinLossInfo item)
        {
            if (item == null)
            {
                return;
            }

            var isRM = item.IBC == RowType.BARM || item.IBC == RowType.EightRM;
            this.Total.WinLoss += isRM ? -item.WinLoss : item.WinLoss;
            this.Total.Position += item.Position;
            this.Total.Commission += item.Commission;
            this.Total.RealLineTotal += isRM ? -item.RealLineTotal : item.RealLineTotal;
            this.Total.VirtualLineTotal += isRM ? -item.VirtualLineTotal : item.VirtualLineTotal;
            this.Total.RealTurnover += isRM ? 0 : item.RealTurnover;
            this.Total.VirtualTurnover += isRM ? 0 : item.VirtualTurnover;
        }

        /// <summary>
        /// Calculates the IBC total.
        /// </summary>
        /// <param name="item">The item value.</param>
        public void CalculateIBCTotal(WinLossInfo item)
        {
            if (item == null)
            {
                return;
            }

            this.IBCTotal = this.IBCTotal ?? new WinLossInfo
            {
                RealLineTotal = 0,
                VirtualLineTotal = 0,
                RealTurnover = 0,
                VirtualTurnover = 0
            };

            this.IBCTotal.WinLoss += -item.WinLoss;
            this.IBCTotal.Position += item.Position;
            this.IBCTotal.Commission += item.Commission;

            this.IBCTotal.RealLineTotal += -item.RealLineTotal;
            this.IBCTotal.VirtualLineTotal += -item.VirtualLineTotal;
            this.IBCTotal.RealTurnover += item.RealTurnover;
            this.IBCTotal.VirtualTurnover += item.VirtualTurnover;
        }

        /// <summary>
        /// Calculate the hedge total.
        /// </summary>
        /// <param name="item">The win loss information.</param>
        public void CalculateHedgeTotal(WinLossInfo item)
        {
            if (item == null)
            {
                return;
            }

            this.HedgeTotal.WinLoss += item.WinLoss;
            this.HedgeTotal.Position += item.Position;
            this.HedgeTotal.Commission += item.Commission;
            this.HedgeTotal.RealLineTotal += item.RealLineTotal;
            this.HedgeTotal.VirtualLineTotal += item.VirtualLineTotal;
            this.HedgeTotal.RealTurnover += item.RealTurnover;
            this.HedgeTotal.VirtualTurnover += item.VirtualTurnover;
        }

        public void CalculateNormalTotal(WinLossInfo item)
        {
            if (item == null)
            {
                return;
            }

            this.NormalTotal.WinLoss += item.WinLoss;
            this.NormalTotal.Position += item.Position;
            this.NormalTotal.Commission += item.Commission;
            this.NormalTotal.RealLineTotal += item.RealLineTotal;
            this.NormalTotal.VirtualLineTotal += item.VirtualLineTotal;
            this.NormalTotal.RealTurnover += item.RealTurnover;
            this.NormalTotal.VirtualTurnover += item.VirtualTurnover;
        }
    }
}