// --------------------------------------------------------------------------
// <copyright file="WinLossInfo.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Louis.Nguyen" date="2013.02.18">Created</change>
// </history>

namespace Fanex.AccountingReports.Main.Models.DailyWinLoss2h
{
    using System;
    using System.Collections.Generic;
    using Constants;
    using Fanex.AccountingReports.Core.Services.Impl;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    [Serializable]
    public class WinLossInfo
    {
        public string Name { get; set; }

        public string AliasName { get; set; }

        public int Id { get; set; }

        public int BettypeId { get; set; }

        public decimal WinLoss { get; set; }

        public decimal Position { get; set; }

        public decimal Commission { get; set; }

        public decimal LineTotal { get; set; }

        public decimal VirtualLineTotal { get; set; }

        public decimal Turnover { get; set; }

        public decimal VirtualTurnover { get; set; }

        public decimal Margin
        {
            get
            {
                return Math.Round(ReportService.CalculateMargin(this.LineTotal, this.Turnover), 4);
            }
        }

        public decimal VirtualMargin
        {
            get
            {
                return Math.Round(ReportService.CalculateMargin(this.VirtualLineTotal, this.VirtualTurnover), 4);
            }
        }

        public decimal Variance
        {
            get
            {
                return this.LineTotal - this.VirtualLineTotal;
            }
        }

        public decimal Variance_ApplyVirtualRate { get; set; }

        public string MatchCode { get; set; }

        public string Status { get; set; }

        public string Hedge { get; set; }

        public bool IsLive { get; set; }

        public bool IsNeutral { get; set; }

        public bool EnableVirtual { get; set; }

        public int IBC { get; set; }

        public int SiteType { get; set; }

        public int HomeTeamId { get; set; }

        public string HomeTeamName { get; set; }

        public int AwayTeamId { get; set; }

        public string AwayTeamName { get; set; }

        public bool IsHideRealTurnover { get; set; }

        public int DisplayOrder { get; set; }

        public decimal ExtraPositionTaking { get; set; }

        public decimal AfterExtraPositionTaking
        {
            get
            {
                return Math.Round(this.LineTotal - this.ExtraPositionTaking, 2);
            }
        }

        public bool IsShowAvailableExtraPositionTaking
        {
            get
            {
                return this.Id == SportContants._2hSoccer ||
                    this.Id == SportContants._2hSoccerMajorBetType ||
                    this.Id == SportContants._2hSoccerMinorBetType ||
                    this.Id == SportContants._2hMetricGamingSuperLive ||
                    this.Id == SportContants._2hOtherSports ||
                    this.Id == SportContants._2hParlay ||
                    this.Id == SportContants._2hNumberGame ||
                    this.Id == SportContants._2hLiveCasino ||
                    this.Id == SportContants._2hCasino ||
                    this.Id == SportContants._2hVirtualSports ||
                    this.Id == SportContants._2hKeno ||
                    this.Id == SportContants._2hBingo ||
                    this.Id == SportContants._2hNoneSportGroup ||
                    this.Id == SportContants._2hLiveCasinoSLI ||
                    this.Id == SportContants._2hLiveCasinoGoldDeluxe ||
                    this.Id == SportContants._2hCasinoMBCRNG;
            }
        }

        public List<WinLossInfo> ChildsInfo { get; set; }

        public WinLossInfo EightRMTotal { get; set; }

        public WinLossInfo BARMTotal { get; set; }

        public WinLossInfo CreditTotal { get; set; }

        public WinLossInfo DepositTotal { get; set; }

        public WinLossInfo NormalTotal { get; set; }

        public WinLossInfo GrandTotal { get; set; }

        public WinLossInfo Total { get; set; }

        public WinLossInfo CreditTotalNewCricket { get; set; }

        public WinLossInfo DepositTotalNewCricket { get; set; }

        /// <summary>
        /// Formats the specified win loss.
        /// </summary>
        /// <param name="winLoss">The win loss.</param>
        /// <returns>The formatted win loss.</returns>
        public static WinLossInfo Format(WinLossInfo winLoss)
        {
            if (winLoss == null)
            {
                return winLoss;
            }

            winLoss.WinLoss = Math.Round(winLoss.WinLoss, 2);
            winLoss.Position = Math.Round(winLoss.Position, 2);
            winLoss.Commission = Math.Round(winLoss.Commission, 2);
            winLoss.LineTotal = Math.Round(winLoss.LineTotal, 2);
            winLoss.VirtualLineTotal = Math.Round(winLoss.VirtualLineTotal, 2);
            winLoss.Turnover = Math.Round(winLoss.Turnover, 2);
            winLoss.VirtualTurnover = Math.Round(winLoss.VirtualTurnover, 2);
            winLoss.ExtraPositionTaking = Math.Round(winLoss.ExtraPositionTaking, 2);
            return winLoss;
        }

        /// <summary>
        /// Formats the total.
        /// </summary>
        /// <param name="winLoss">The win loss.</param>
        /// <returns>The formatted win loss.</returns>
        public static WinLossInfo Format_Total(WinLossInfo winLoss)
        {
            if (winLoss == null)
            {
                return winLoss;
            }

            winLoss.EightRMTotal = WinLossInfo.Format(winLoss.EightRMTotal);
            winLoss.BARMTotal = WinLossInfo.Format(winLoss.BARMTotal);
            winLoss.CreditTotal = WinLossInfo.Format(winLoss.CreditTotal);
            winLoss.DepositTotal = WinLossInfo.Format(winLoss.DepositTotal);
            winLoss.NormalTotal = WinLossInfo.Format(winLoss.NormalTotal);
            winLoss.GrandTotal = WinLossInfo.Format(winLoss.GrandTotal);
            winLoss.Total = WinLossInfo.Format(winLoss.Total);
            winLoss.CreditTotalNewCricket = WinLossInfo.Format(winLoss.CreditTotalNewCricket);
            winLoss.DepositTotalNewCricket = WinLossInfo.Format(winLoss.DepositTotalNewCricket);
            return winLoss;
        }

        public void Add(WinLossInfo winLoss)
        {
            this.WinLoss += winLoss.WinLoss;
            this.Position += winLoss.Position;
            this.Commission += winLoss.Commission;
            this.LineTotal += winLoss.LineTotal;
            this.VirtualLineTotal += winLoss.VirtualLineTotal;
            this.Turnover += winLoss.Turnover;
            this.VirtualTurnover += winLoss.VirtualTurnover;
            this.ExtraPositionTaking += winLoss.ExtraPositionTaking;
        }
    }
}