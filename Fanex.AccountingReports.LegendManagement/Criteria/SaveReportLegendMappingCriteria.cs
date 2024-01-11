namespace Fanex.AccountingReports.LegendManagement.Criteria
{
    using Fanex.Data.Repository;

    public class SaveReportLegendMappingCriteria : CriteriaBase
    {
        public int AccMenuId { get; set; }

        public string NewSystem { get; set; }

        public int NewMenuId { get; set; }

        public int LegendId { get; set; }

        public bool IsNote { get; set; }

        public override string GetSettingKey()
        {
            return "LegendManagement_SaveReportLegendMapping";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}
