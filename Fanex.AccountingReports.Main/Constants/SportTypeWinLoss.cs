// --------------------------------------------------------------------------
// <copyright file="SportTypeWinLoss.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Kan.Nguyen" date="2013.06.28">Created</change>
// </history>

namespace Fanex.AccountingReports.Main.Common
{
    public enum SportTypeWinLoss : int
    {
        /// <summary>
        /// The unknown.
        /// </summary>
        Unknown = 0,

        /// <summary>
        /// The sport book.
        /// </summary>
        SportBook,

        /// <summary>
        /// The racing.
        /// </summary>
        Racing,

        /// <summary>
        /// The virtual sports.
        /// </summary>
        VirtualSports,

        /// <summary>
        /// The virtual racing.
        /// </summary>
        VirtualRacing,

        /// <summary>
        /// The number game.
        /// </summary>
        NumberGame, // special sport of SportBook

        Bullfighting
    }
}
