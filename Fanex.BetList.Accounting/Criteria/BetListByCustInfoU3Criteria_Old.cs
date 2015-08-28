// --------------------------------------------------------------------------
// <copyright file="BetListByCustInfoU3Criteria_Old.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="" date="2012.12.17">Created</change>
// </history>

namespace Fanex.BetList.Accounting.Criteria
{
    using System;
    using Fanex.NAP.Framework.Data;
    
    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class BetListByCustInfoU3Criteria_Old : CriteriaBase
    {
        [SpParam(SqlParamName = "CustID")]
        public int CustId { get; set; }

        public DateTime FromDate { get; set; }

        public DateTime ToDate { get; set; }

        [SpParam(SqlParamName = "Bettype")]
        public int BetType { get; set; }

        public override string GetSettingKey()
        {
            return "U3_BetsSelBetList_Old";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}