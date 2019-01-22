namespace Fanex.AccountingReports.Misc.Criteria.SMATurnover
{
    using System;
    using Data;
    using Fanex.AccountingReports.Misc.Constants;
    using Fanex.Data.Repository;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class GetSMATurnoverReportCriteria : CriteriaBase
    {
        [SpParam(Ignored = true)]
        public int ReportType { get; set; }

        public DateTime FromDate { get; set; }

        public string ProductID { get; set; }

        public bool ShowOpened { get; set; }

        public bool ShowClosed { get; set; }

        public bool ShowSuspended { get; set; }

        [SpParam(Ignored = true)]
        public string SortColumnName { get; set; }

        [SpParam(Ignored = true)]
        public string SortDirection { get; set; }

        public override bool IsValid()
        {
            return true;
        }

        /// <summary>
        /// Gets the setting key.
        /// </summary>
        /// <returns>The setting key.</returns>
        public override string GetSettingKey()
        {
            string settingKey = string.Empty;

            switch (ReportType)
            {
                case SMATurnOverConstants.ReportByDate:
                    settingKey = "SMA_TurnOver_ReportByDate";
                    break;

                case SMATurnOverConstants.ReportByMonth:
                    settingKey = "SMA_TurnOver_ReportByMonth";
                    break;

                case SMATurnOverConstants.ReportByYear:
                    settingKey = "SMA_TurnOver_ReportByYear";
                    break;

                default:
                    break;
            }

            return settingKey;
        }
    }
}
