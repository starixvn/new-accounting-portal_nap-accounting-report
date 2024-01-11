namespace Fanex.AccountingReports.Core.Criteria
{
    using Fanex.Data.Repository;

    public class GetDWHDateRangeCriteria : CriteriaBase
    {
        public string Cube
        {
            get
            {
                return "DW";
            }
        }

        public override bool IsValid()
        {
            return true;
        }

        public override string GetSettingKey()
        {
            return "Core_GetDWH_DateRange";
        }
    }
}
