namespace Fanex.AccountingReports.LegendManagement.Criteria
{
    using Fanex.Data.Repository;

    /// <summary>
    /// Get Report Legend Legend Mapping Criteria.
    /// </summary>
    public class GetReportLegendMappingCriteria : CriteriaBase
    {
        public int MenuId { get; set; }

        public string SystemName { get; set; }

        /// <summary>
        /// Get the reference SP's Name which is specified in Config file.
        /// </summary>
        /// <returns>Store procedure alias name.</returns>
        public override string GetSettingKey()
        {
            return "LegendManagement_GetReportLegendMapping";
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