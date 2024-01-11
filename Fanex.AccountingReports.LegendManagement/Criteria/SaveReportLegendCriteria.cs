namespace Fanex.AccountingReports.LegendManagement.Criteria
{
    using Data;
    using Fanex.AccountingReports.LegendManagement.Configurations;
    using Fanex.Data.Repository;

    public class SaveReportLegendCriteria : CriteriaBase
    {
        public SaveReportLegendCriteria()
        {
            this.System = WebSettings.System;
        }

        public int MenuId { get; set; }

        public string Legend { get; set; }

        public int LegendId { get; set; }

        public string System { get; set; }

        [SpParam(Ignored = true)]
        public bool IsNote { get; set; }

        [SpParam(Ignored = true)]
        public bool IsReviewed { get; set; }

        [SpParam(Ignored = true)]
        public string ConfigPath { get; set; }

        public override string GetSettingKey()
        {
            return "LegendManagement_SaveReportLegend";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}
