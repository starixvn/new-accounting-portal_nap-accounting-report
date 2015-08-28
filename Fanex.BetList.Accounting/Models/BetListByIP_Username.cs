// --------------------------------------------------------------------------
// <copyright file="BetListByIP_Username.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="" date="2012.12.07">Created</change>
// </history>

namespace Fanex.BetList.Accounting.Models
{
    using System;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    [Serializable]
    public class BetListByIP_Username
    {
        public string Username { get; set; }

        public int CustID { get; set; }

        public int SoccerBets { get; set; }

        public int OtherBets { get; set; }

        public int TotalBets { get; set; }
    }
}