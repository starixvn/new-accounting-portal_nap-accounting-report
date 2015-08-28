// --------------------------------------------------------------------------
// <copyright file="TennisSetResult.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="" date="2014.10.09">Created</change>
// </history>

namespace Fanex.BetList.Accounting.Models
{
    using System.Collections.Generic;

    /// <summary>
    /// Tennis model.
    /// </summary>
    public class TennisSetResult
    {
        public TennisSetResult()
        {
            this.GameResults = new List<TennisGameResult>();
        }

        public List<TennisGameResult> GameResults { get; set; }

        public TennisResultEntity SetDetail { get; set; }

        public int MaxGame { get; set; }
    }
}