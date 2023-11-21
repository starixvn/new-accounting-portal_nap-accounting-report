namespace Fanex.AccountingReports.Misc.Criteria.MAMonitorTool
{
    using System;
    using System.Collections;
    using Data;
    using Fanex.Data.Repository;

    /// <summary>
    /// Get Report Criteria.
    /// </summary>
    public class GetReportCriteria : CriteriaBase
    {
        [SpParam(SqlParamName = "reportby")]
        public int ReportBy { get; set; }

        [SpParam(SqlParamName = "fromdate")]
        public DateTime FromDate { get; set; }

        [SpParam(SqlParamName = "todate")]
        public DateTime ToDate { get; set; }

        public string Products { get; set; }

        [SpParam(SqlParamName = "sporttype")]
        public int SportType { get; set; }

        [SpParam(SqlParamName = "bettype")]
        public int BetTypeId { get; set; }

        [SpParam(SqlParamName = "currency")]
        public int CurrencyId { get; set; }

        [SpParam(SqlParamName = "live")]
        public int LiveStatus { get; set; }

        [SpParam(SqlParamName = "leagues")]
        public string Leagues { get; set; }

        [SpParam(SqlParamName = "usernames")]
        public string Usernames { get; set; }

        [SpParam(Ignored = true)]
        public string MinTurnover { get; set; }

        [SpParam(Ignored = true)]
        public string MaxTurnover { get; set; }

        [SpParam(Ignored = true)]
        public string MinWinLoss { get; set; }

        [SpParam(Ignored = true)]
        public string MaxWinLoss { get; set; }

        [SpParam(Ignored = true)]
        public bool IsPagingCallBack { get; set; }

        [SpParam(Ignored = true)]
        public int StartIndex { get; set; }

        [SpParam(Ignored = true)]
        public int UsersPerPage { get; set; }

        [SpParam(Ignored = true)]
        public bool IsExportExcel { get; set; }

        [SpParam(Ignored = true)]
        public bool IsHistoryReport { get; set; }

        [SpParam(Ignored = true)]
        public bool IsShowSportBook { get; set; }

        /// <summary>
        /// Gets the SP parameters.
        /// </summary>
        /// <returns>The SP parameters.</returns>
        public Hashtable GetSpParams()
        {
            Hashtable storeProcedureParams = new Hashtable();
            storeProcedureParams.Add("reportby", ReportBy);
            storeProcedureParams.Add("fromdate", FromDate);
            storeProcedureParams.Add("todate", ToDate);
            storeProcedureParams.Add("products", Products);

            if (IsShowSportBook)
            {
                storeProcedureParams.Add("sporttype", SportType);
            }

            storeProcedureParams.Add("currency", CurrencyId);
            storeProcedureParams.Add("bettype", BetTypeId);
            if (LiveStatus != -1)
            {
                storeProcedureParams.Add("live", LiveStatus);
            }

            if (!string.IsNullOrEmpty(Leagues))
            {
                storeProcedureParams.Add("leagues", Leagues);
            }

            if (!string.IsNullOrEmpty(Usernames))
            {
                storeProcedureParams.Add("usernames", Usernames);
            }

            return storeProcedureParams;
        }

        /// <summary>
        /// Get store procedure key.
        /// </summary>
        /// <returns>Return store procedure key.</returns>
        public override string GetSettingKey()
        {
            string _spname = string.Empty;
            if (IsHistoryReport)
            {
                _spname = "Misc_MAMonitorTool_GetReport_Archive";
            }
            else
            {
                _spname = "Misc_MAMonitorTool_GetReport";
            }

            return _spname;
        }

        public override bool IsValid()
        {
            return (string.IsNullOrEmpty(Usernames) || Usernames.Length <= 4000) &&
                ((string.IsNullOrEmpty(MinTurnover) && string.IsNullOrEmpty(MaxTurnover)) ||
                (!string.IsNullOrEmpty(MinTurnover) && !string.IsNullOrEmpty(MaxTurnover))) &&
                ((string.IsNullOrEmpty(MinWinLoss) && string.IsNullOrEmpty(MaxWinLoss)) ||
                (!string.IsNullOrEmpty(MinWinLoss) && !string.IsNullOrEmpty(MaxWinLoss)));
        }
    }
}