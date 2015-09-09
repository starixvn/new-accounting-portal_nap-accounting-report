// -----------------------------------------------------------------------
// <copyright file="GetMatchsCriteria.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------
// <history>
//   <change who="Franklin.Nguyen" date="2014.04.28">Created</change>
// </history>

namespace Fanex.AccountingReports.Special.Criteria.SaveBetReport
{
    using System;
    using Fanex.Data.Repository;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class GetMatchsCriteria : CriteriaBase
    {
        public DateTime Date { get; set; }

        public int LeagueId { get; set; }

        public int SportType { get; set; }

        public override bool IsValid()
        {
            return true;
        }

        public override string GetSettingKey()
        {
            return "Special_SaveBet_GetListMatch";
        }
    }
}