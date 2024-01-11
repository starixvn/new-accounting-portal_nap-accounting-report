namespace Fanex.AccountingReports.LegendManagement.Criteria
{
    using Fanex.Data.Repository;

    public class GetReportLegendMappingCriteria : CriteriaBase
    {
        public int MenuId { get; set; }

        public string SystemName { get; set; }

        public override string GetSettingKey()
        {
            return "LegendManagement_GetReportLegendMapping";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}
