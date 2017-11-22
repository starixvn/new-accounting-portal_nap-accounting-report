namespace Fanex.AccountingReports.Misc.Models.SMATurnover
{
    using System;
    using System.Collections.Generic;
    using Fanex.AccountingReports.Core.Constants;
    using Fanex.AccountingReports.Core.Resources;

    /// <summary>
    /// SMA turn over.
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
                    case IBCRoleConstants.Super:
                        userPosition = Labels.Super;
                        break;

                    case IBCRoleConstants.Master:
                        userPosition = Labels.Master;
                        break;

                    case IBCRoleConstants.Agent:
                        userPosition = Labels.Agent;
                        break;

                    case IBCRoleConstants.Member:
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
