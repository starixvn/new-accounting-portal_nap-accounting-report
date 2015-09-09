// -----------------------------------------------------------------------
// <copyright file="GetSportsCriteria.cs" company="Nexcel Solutions Vietnam">
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
    /// Get sports criteria.
    /// </summary>
    public class GetSportsCriteria : CriteriaBase
    {
        public DateTime Date { get; set; }

        public override bool IsValid()
        {
            return true;
        }

        public override string GetSettingKey()
        {
            return "Special_SaveBet_GetListSport";
        }
    }
}