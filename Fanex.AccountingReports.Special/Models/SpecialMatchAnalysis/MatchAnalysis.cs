// -----------------------------------------------------------------------
// <copyright file="MatchAnalysis.cs" company="Microsoft">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------
// <history>
//   <change who="Robert.Tran" date="2012.12.26">Created</change>
// </history>

namespace Fanex.AccountingReports.Special.Models.SpecialMatchAnalysis
{
    using System;
    using System.Collections.Generic;

    public enum ReportType
    {
        /// <summary>
        /// The currency.
        /// </summary>
        Currency = 0,

        /// <summary>
        /// The bet type.
        /// </summary>
        Bettype = 1,

        /// <summary>
        /// The super.
        /// </summary>
        Super = 2,

        /// <summary>
        /// The member.
        /// </summary>
        Member = 3,

        /// <summary>
        /// The super view on master.
        /// </summary>
        SuperViewOnMaster = 4,

        /// <summary>
        /// The master view on agent.
        /// </summary>
        MasterViewOnAgent = 5,

        /// <summary>
        /// The agent view on member.
        /// </summary>
        AgentViewOnMember = 6
    }

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    [Serializable]
    public class MatchAnalysis : BaseModel
    {
        public MatchAnalysis()
        {
            SMAWL = string.Empty;
            SMATO = string.Empty;

            HouseWL = string.Empty;
            HouseTO = string.Empty;

            MemberWL = string.Empty;
            MemberTO = string.Empty;

            BetCount = string.Empty;
            Currency = string.Empty;
        }

        public string SMAWL { get; set; }

        public string SMATO { get; set; }

        public string HouseWL { get; set; }

        public string HouseTO { get; set; }

        public string MemberWL { get; set; }

        public string MemberTO { get; set; }

        public string BetCount { get; set; }

        public string Currency { get; set; }
    }
}
