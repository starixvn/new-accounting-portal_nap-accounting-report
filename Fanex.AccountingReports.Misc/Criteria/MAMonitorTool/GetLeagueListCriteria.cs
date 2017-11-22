namespace Fanex.AccountingReports.Misc.Criteria.MAMonitorTool
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using Fanex.AccountingReports.Misc.Models.MAMonitorTool;
    using Fanex.Data.Repository;

    /// <summary>
    /// Get League List Criteria.
    /// </summary>
    public class GetLeagueListCriteria : CriteriaBase
    {
        [SpParam(SqlParamName = "sporttype")]
        public int SportType { get; set; }

        [SpParam(SqlParamName = "fromdate")]
        public DateTime FromDate { get; set; }

        [SpParam(SqlParamName = "todate")]
        public DateTime ToDate { get; set; }

        [SpParam(SqlParamName = "currency")]
        public int CurrencyId { get; set; }

        [SpParam(SqlParamName = "bettype")]
        public int BetTypeId { get; set; }

        [SpParam(SqlParamName = "live")]
        public int LiveStatus { get; set; }

        [SpParam(Ignored = true)]
        public List<OptionInfo> LeagueTypeList { get; set; }

        [SpParam(SqlParamName = "only4mgsl")]
        public bool IsOnlyMetricGamingSuperLive { get; set; }

        /// <summary>
        /// Gets the SP parameters.
        /// </summary>
        /// <returns>The SP parameters.</returns>
        public override Hashtable GetSpParams()
        {
            Hashtable storeProcedureParams = new Hashtable();
            storeProcedureParams.Add("sporttype", SportType);
            storeProcedureParams.Add("fromdate", FromDate);
            storeProcedureParams.Add("todate", ToDate);
            storeProcedureParams.Add("currency", CurrencyId);
            storeProcedureParams.Add("bettype", BetTypeId);
            if (LiveStatus != -1)
            {
                storeProcedureParams.Add("live", LiveStatus);
            }

            if (LeagueTypeList != null)
            {
                foreach (var optionInfo in LeagueTypeList)
                {
                    if (string.IsNullOrEmpty(optionInfo.Name))
                    {
                        continue;
                    }

                    storeProcedureParams.Add(optionInfo.Name, optionInfo.Value);
                }
            }

            // storeProcedureParams.Add("only4mgsl", IsOnlyMetricGamingSuperLive);
            return storeProcedureParams;
        }

        public override string GetSettingKey()
        {
            return "Misc_MAMonitorTool_GetLeagueList";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}
