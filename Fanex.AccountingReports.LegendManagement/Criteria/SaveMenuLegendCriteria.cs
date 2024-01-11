namespace Fanex.AccountingReports.LegendManagement.Criteria
{
    using Fanex.Data.Repository;

    public class SaveMenuLegendCriteria : CriteriaBase
    {
        public int MenuId { get; set; }

        public string MenuLegend { get; set; }

        public int MenuLegendId { get; set; }

        public string ConfigPath { get; set; }

        public override string GetSettingKey()
        {
            return "SaveMenuLegend";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}
