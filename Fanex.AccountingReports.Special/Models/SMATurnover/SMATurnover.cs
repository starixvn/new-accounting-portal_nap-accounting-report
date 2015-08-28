// -----------------------------------------------------------------------
// <copyright file="SMATurnover.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------
// <history>
//   <change who="Franklin.Nguyen" date="2014.06.05">Created</change>
// </history>

namespace Fanex.AccountingReports.Special.Models.SMATurnover
{
    using System;
    using System.Collections.Generic;
    using Fanex.AccountingReports.Special.Constants;
    using Fanex.AccountingReports.Special.Resources;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    [Serializable]
    public class SMATurnover
    {
        public SMATurnover()
        {
            ReportRows = new List<SMATurnover>();
        }

        public int GroupId { get; set; }

        public string GroupHeadline { get; set; }

        public string UserName { get; set; }

        public string Currency { get; set; }

        public int RoleId { get; set; }

        public string Position
        {
            get
            {
                string userPosition = string.Empty;

                switch (this.RoleId)
                {
                    case RoleConstants.Super:
                        userPosition = Labels.Super;
                        break;

                    case RoleConstants.Master:
                        userPosition = Labels.Master;
                        break;

                    case RoleConstants.Agent:
                        userPosition = Labels.Agent;
                        break;

                    case RoleConstants.Member:
                        userPosition = Labels.Member;
                        break;

                    default:
                        break;
                }

                return userPosition;
            }
        }

        public decimal TurnOver { get; set; }

        public decimal RMTurnOver { get; set; }

        public decimal MemberTurnOver { get; set; }

        public decimal MemberRMTurnOver { get; set; }

        public SMATurnover Total { get; set; }

        public List<SMATurnover> ReportRows { get; set; }

        public int ReportLength { get; set; }

        public void CalculateTotal(SMATurnover info)
        {
            this.Total.TurnOver += info.TurnOver;
            this.Total.RMTurnOver += info.RMTurnOver;
            this.Total.MemberTurnOver += info.MemberTurnOver;
            this.Total.MemberRMTurnOver += info.MemberRMTurnOver;
        }
    }
}