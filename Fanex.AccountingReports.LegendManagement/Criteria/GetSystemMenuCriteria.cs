namespace Fanex.AccountingReports.LegendManagement.Criteria
{
    using Data;
    using Fanex.AccountingReports.LegendManagement.Configurations;
    using Fanex.Data.Repository;

    public class GetSystemMenuCriteria : CriteriaBase
    {
        public GetSystemMenuCriteria()
        {
            this.System = WebSettings.System;
        }

        [SpParam(SqlParamName = "system")]
        public string System { get; set; }

        public override string GetSettingKey()
        {
            return "LegendManagement_SelMenuListBySystem";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}
