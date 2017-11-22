namespace Fanex.AccountingReports.LegendManagement.Criteria
{
    using Fanex.Data.Repository;

    /// <summary>
    /// Save Menu Legend Criteria.
    /// </summary>
    public class SaveMenuLegendCriteria : CriteriaBase
    {
        public int MenuId { get; set; }

        public string MenuLegend { get; set; }

        public int MenuLegendId { get; set; }

        public string ConfigPath { get; set; }

        /// <summary>
        /// Get the reference SP's Name which is specified in Config file.
        /// </summary>
        /// <returns>Store procedure alias name.</returns>
        public override string GetSettingKey()
        {
            return "SaveMenuLegend";
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
