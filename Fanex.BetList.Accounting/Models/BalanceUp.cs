// --------------------------------------------------------------------------
// <copyright file="BalanceUp.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="" date="2012.12.07">Created</change>
// </history>

namespace Fanex.BetList.Accounting.Models
{
    using System;
    using System.Diagnostics.CodeAnalysis;

    /// <summary>
    /// BalanceUp: getting balance up day and move bet day.
    /// </summary>
    public class BalanceUp
    {
        public DateTime BalanceUpDay { get; set; }

        public DateTime MoveBetsDay { get; set; }
    }
}