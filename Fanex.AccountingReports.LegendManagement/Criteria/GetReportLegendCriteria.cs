namespace Fanex.AccountingReports.LegendManagement.Criteria
{
    using Data;
    using Fanex.AccountingReports.LegendManagement.Configurations;
    using Fanex.Data.Repository;

    public class GetReportLegendCriteria : CriteriaBase
    {
        public GetReportLegendCriteria()
        {
            this.MenuId = WebSettings.DefaultReportLegendTemplate;
            this.LegendId = WebSettings.DefaultReportLegendTemplate;
            this.System = WebSettings.System;
            this.IsNote = false;
        }

        public int MenuId { get; set; }

        public string System { get; set; }

        [SpParam(Ignored = true)]
        public bool IsNote { get; set; }

        [SpParam(Ignored = true)]
        public int LegendId { get; set; }

        [SpParam(Ignored = true)]
        public string ConfigPath { get; set; }

        [SpParam(Ignored = true)]
        public string FunctionName { get; set; }

        [SpParam(Ignored = true)]
        public string CallBackFunction { get; set; }

        public override string GetSettingKey()
        {
            return "LegendManagement_SelReportLegend";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}
