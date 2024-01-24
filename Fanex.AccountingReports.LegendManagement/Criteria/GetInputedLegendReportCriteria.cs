namespace Fanex.AccountingReports.LegendManagement.Criteria
{
    using Fanex.Data.Repository;

    public class GetInputedLegendReportCriteria : CriteriaBase
    {
        public GetInputedLegendReportCriteria()
        {
            System = string.Empty;
        }

        public string System { get; set; }

        public override string GetSettingKey()
        {
            return "LegendManagement_GetInputedLegend";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}
