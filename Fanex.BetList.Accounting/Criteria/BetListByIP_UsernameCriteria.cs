// --------------------------------------------------------------------------
// <copyright file="BetListByIP_UsernameCriteria.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Ricky.Nguyen" date="2012.12.10">Created</change>
// </history>

namespace Fanex.BetList.Accounting.Criteria
{
    using System;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class BetListByIP_UsernameCriteria
    {
        public DateTime FromDate { get; set; }

        public string IP { get; set; }

        public string GetSettingKey()
        {
            return "BetList_IP_Username";
        }
    }
}