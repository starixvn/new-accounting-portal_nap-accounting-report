namespace Fanex.AccountingReports.Misc.Models.DailyCreatedCustomers
{
    using System;
    using System.Collections.Generic;

    [Serializable]
    public class SummaryInformation
    {    
        public string Name { get; set; }

        public string CreatedDate { get; set; }

        public int SuperAmount { get; set; }
        
        public int MasterAmount { get; set; }

        public int AgentAmount { get; set; }

        public int MemberAmount { get; set; }       

        public int DirectMasterAmount { get; set; }

        public int DirectAgentAmount { get; set; }

        public List<SummaryInformation> CreatingDetails { get; set; }        

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
