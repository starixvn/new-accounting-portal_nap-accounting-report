// -----------------------------------------------------------------------
// <copyright file="League.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------
// <history>
//   <change who="Franklin.Nguyen" date="2014.04.28">Created</change>
// </history>

namespace Fanex.AccountingReports.Special.Models.SaveBetReport
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// League Model.
    /// </summary>
    [Serializable]
    public class League
    {
        public League()
        {
            Leagues = new List<League>();
        }

        public int LeagueId { get; set; }

        public string LeagueName { get; set; }

        public List<League> Leagues { get; set; }
    }
}