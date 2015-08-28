// --------------------------------------------------------------------------
// <copyright file="MixParlayDetailCriteria.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="" date="2012.12.07">Created</change>
// </history>

namespace Fanex.BetList.Accounting.Criteria
{
    using Fanex.NAP.Framework.Data;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class MixParlayDetailCriteria
    {
        [SpParam(SqlParamName = "refno")]
        public string RefNo { get; set; }

        public string GetSettingKey()
        {
            return "BetList_MixParlayBetsSel";
        }
    }
}