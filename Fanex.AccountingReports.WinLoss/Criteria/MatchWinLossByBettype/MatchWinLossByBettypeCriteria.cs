// -----------------------------------------------------------------------
// <copyright file="MatchWinLossByBettypeCriteria.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------
// <history>
//   <change who="Franklin.Nguyen" date="2013.12.13">Created</change>
// </history>

namespace Fanex.AccountingReports.WinLoss.Criteria.MatchWinLossByBettype
{
    using System;
    using Fanex.AccountingReports.Core.Extensions;
    using Fanex.NAP.Framework.Data;

    /// <summary>
    /// Match Win Loss By Bet Type Criteria.
    /// </summary>
    public class MatchWinLossByBettypeCriteria : CriteriaBase
    {
        private DateTime _todate = DateTime.Today;

        public DateTime FromDate { get; set; }

        public DateTime ToDate
        {
            get 
            {
                return _todate;
            }

            set 
            {
                _todate = value.GetEndOfDate();
            }
        }

        public override string GetSettingKey()
        {
            return "MatchWinLossByBetType";
        }

        public override bool IsValid()
        {
            return this.FromDate <= this.ToDate;
        }
    }
}