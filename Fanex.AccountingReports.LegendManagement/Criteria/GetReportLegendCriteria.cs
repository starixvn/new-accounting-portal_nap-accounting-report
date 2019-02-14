namespace Fanex.AccountingReports.LegendManagement.Criteria
{
    using Data;
    using Fanex.AccountingReports.LegendManagement.Configurations;
    using Fanex.Data.Repository;

    /// <summary>
    /// Get Report Legend Criteria.
    /// </summary>
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

        /// <summary>
        /// Get the reference SP's Name which is specified in Config file.
        /// </summary>
        /// <returns>Store procedure alias name.</returns>
        public override string GetSettingKey()
        {
            return "LegendManagement_SelReportLegend";
        }

        /// <summary>
        /// Check data's constrains.
        /// </summary>
        /// <returns>True: valid; False: invalid.</returns>
        public override bool IsValid()
        {
            return true;
        }
    }
}
