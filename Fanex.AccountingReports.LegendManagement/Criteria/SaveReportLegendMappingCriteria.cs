namespace Fanex.AccountingReports.LegendManagement.Criteria
{
    using Fanex.Data.Repository;

    /// <summary>
    /// Save Report Legend Mapping Criteria.
    /// </summary>
    public class SaveReportLegendMappingCriteria : CriteriaBase
    {
        public int AccMenuId { get; set; }

        public string NewSystem { get; set; }

        public int NewMenuId { get; set; }

        public int LegendId { get; set; }

        public bool IsNote { get; set; }

        /// <summary>
        /// Get the reference SP's Name which is specified in Config file.
        /// </summary>
        /// <returns>Store procedure alias name.</returns>
        public override string GetSettingKey()
        {
            return "LegendManagement_SaveReportLegendMapping";
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