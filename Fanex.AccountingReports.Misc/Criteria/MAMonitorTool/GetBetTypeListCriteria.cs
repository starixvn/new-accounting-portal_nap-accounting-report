namespace Fanex.AccountingReports.Misc.Criteria.MAMonitorTool
{
    using System.Collections;
    using System.Collections.Generic;
    using Fanex.AccountingReports.Misc.Models.MAMonitorTool;
    using Fanex.Data.Repository;

    /// <summary>
    /// Get Bet Type List Criteria.
    /// </summary>
    public class GetBetTypeListCriteria : CriteriaBase
    {
        public int SportType { get; set; }

        public string Products { get; set; }

        public override string GetSettingKey()
        {
            return "Misc_MAMonitorTool_GetBetTypeList";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}
