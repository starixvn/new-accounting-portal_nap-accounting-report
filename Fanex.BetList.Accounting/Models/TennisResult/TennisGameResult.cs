// --------------------------------------------------------------------------
// <copyright file="TennisGameResult.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="" date="2014.10.09">Created</change>
// </history>

namespace Fanex.BetList.Accounting.Models
{
    /// <summary>
    /// Tennis model.
    /// </summary>
    public class TennisGameResult
    {
        public TennisResultEntity TennisGame { get; set; }

        public int GameId { get; set; }
    }
}