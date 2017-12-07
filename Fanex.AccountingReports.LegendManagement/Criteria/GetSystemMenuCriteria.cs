namespace Fanex.AccountingReports.LegendManagement.Criteria
{
    using Fanex.AccountingReports.LegendManagement.Configurations;
    using Fanex.Data.Repository;

    /// <summary>
    /// Get System Menu Criteria.
    /// </summary>
    public class GetSystemMenuCriteria : CriteriaBase
    {
        public GetSystemMenuCriteria()
        {
            this.System = WebSettings.System;
        }

        [SpParam(SqlParamName = "system")]
        public string System { get; set; }

        /// <summary>
        /// Get the reference SP's Name which is specified in Config file.
        /// </summary>
        /// <returns>Store procedure alias name.</returns>
        public override string GetSettingKey()
        {
            return "LegendManagement_SelMenuListBySystem";
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
