namespace Fanex.AccountingReports.Misc.Models.DailyCreatedCustomers
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    [Serializable]
    public class SummaryInformation
    {    
        /// <summary>
        /// Gets or sets Name.
        /// </summary>   
        /// <value>The name of customer.</value>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets CreatedDate.
        /// </summary>
        /// <value>The created date.</value>
        public string CreatedDate { get; set; }

        /// <summary>
        /// Gets or sets SuperAmount.
        /// </summary>
        /// <value>The amount of super customer.</value>
        public int SuperAmount { get; set; }
        
        /// <summary>
        /// Gets or sets MasterAmount.
        /// </summary>
        /// <value>The amount of Master customer.</value>
        public int MasterAmount { get; set; }

        /// <summary>
        /// Gets or sets AgentAmount.
        /// </summary>
        /// <value>The amount of agent customer.</value>
        public int AgentAmount { get; set; }

        /// <summary>
        /// Gets or sets MemberAmount.
        /// </summary>
        /// <value>The amount of member.</value>
        public int MemberAmount { get; set; }       

        /// <summary>
        /// Gets or sets DirectMasterAmount.
        /// </summary>
        /// <value>The amount of DirectMember.</value>
        public int DirectMasterAmount { get; set; }

        /// <summary>
        /// Gets or sets DirectAgentAmount.
        /// </summary>
        /// <value>The amount of DirectAgent.</value>
        public int DirectAgentAmount { get; set; }

        /// <summary>
        /// Gets or sets CreatingDetails.
        /// </summary>
        /// <value>The list of summary information.</value>
        public List<SummaryInformation> CreatingDetails { get; set; }        

        /// <summary>
        /// Compute total value of each property.
        /// </summary>
        /// <param name="info">The report.</param>
        public void SumDataToTotal(SummaryInformation info)
        {
            if (info == null)
            {
                return;
            }

            this.SuperAmount += info.SuperAmount;          
            this.MasterAmount += info.MasterAmount;
            this.AgentAmount += info.AgentAmount;
            this.MemberAmount += info.MemberAmount;           
            this.DirectMasterAmount += info.DirectMasterAmount;
            this.DirectAgentAmount += info.DirectAgentAmount;
        }
    }
}
