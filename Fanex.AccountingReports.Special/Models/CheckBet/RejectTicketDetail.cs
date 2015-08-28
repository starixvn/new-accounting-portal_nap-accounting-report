// --------------------------------------------------------------------------
// <copyright file="RejectTicketDetail.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Faisal.Tran" date="2012.12.20">Created</change>
// </history>

namespace Fanex.AccountingReports.Special.Models
{
    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class RejectTicketDetail
    {
        public string RefNo { get; set; }

        public string TransDate { get; set; }

        public string Currency { get; set; }

        public string Bettype { get; set; }

        public string Username { get; set; }

        public string Stake { get; set; }

        public string Odds { get; set; }

        public string Betteam { get; set; }

        public string TotalPT { get; set; }

        public string OldStatus { get; set; }

        public string NewStatus { get; set; }

        public string Creator { get; set; }
    }
}
