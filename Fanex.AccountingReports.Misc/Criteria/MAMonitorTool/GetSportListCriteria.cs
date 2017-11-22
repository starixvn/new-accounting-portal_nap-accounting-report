namespace Fanex.AccountingReports.Misc.Criteria.MAMonitorTool
{
    using Fanex.Data.Repository;

    /// <summary>
    /// Get Sport List Criteria.
    /// </summary>
    public class GetSportListCriteria : CriteriaBase
    {
        public override string GetSettingKey()
        {
            return "Misc_MAMonitorTool_GetSportList";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}
