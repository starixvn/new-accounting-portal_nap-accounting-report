namespace Fanex.AccountingReports.LegendManagement.Criteria
{
    using Fanex.Data.Repository;

    public class GetMenuLegendCriteria : CriteriaBase
    {
        public int MenuId { get; set; }

        public int MenuLegendId { get; set; }

        public string ConfigPath { get; set; }

        public string FunctionName { get; set; }

        public override string GetSettingKey()
        {
            return "SelMenuLegend";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}
