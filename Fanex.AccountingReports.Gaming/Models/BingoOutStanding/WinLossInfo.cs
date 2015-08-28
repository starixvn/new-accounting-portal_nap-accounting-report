// --------------------------------------------------------------------------
// <copyright file="WinLossInfo.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Louis.Nguyen" date="2012.12.07">Created</change>
// </history>

namespace Fanex.AccountingReports.Gaming.Models.BingoOutStanding
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    [Serializable]
    public class WinLossInfo
    {
        public string Id { get; set; }

        public string Name { get; set; }

        public string CurrencyName { get; set; }

        public string Company { get; set; }

        public decimal OutStanding { get; set; }

        public string Date { get; set; }

        public int ErrorCode { get; set; }

        public string ErrorMessage { get; set; }

        public WinLossInfo GrandTotal { get; set; }

        public List<WinLossInfo> ChildsInfo { get; set; }

        public static WinLossInfo Fortmat(WinLossInfo winLoss)
        {
            if (winLoss == null)
            {
                return winLoss;
            }

            winLoss.OutStanding = Math.Round(winLoss.OutStanding, 2);
            return winLoss;
        }

        public void Add(WinLossInfo winLoss)
        {
            this.OutStanding += winLoss.OutStanding;
        }
    }
}
