// --------------------------------------------------------------------------
// <copyright file="TennisResultEntity.cs" company="Nexcel Solutions Vietnam">
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
    public class TennisResultEntity
    {
        public string MatchId { get; set; }
 
        public string ResourceId { get; set; }

        public string HomeValue { get; set; }

        public string AwayValue { get; set; }

        public int ResultStatusId { get; set; }

        public int SettlementStatusId { get; set; }
    }
}