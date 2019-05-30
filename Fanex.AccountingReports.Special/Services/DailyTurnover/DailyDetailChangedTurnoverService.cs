namespace Fanex.AccountingReports.Special.Services.Implement.DailyTurnover
{
    using System;
    using System.Collections.Generic;
    using Fanex.AccountingReports.Core.Extensions;
    using Fanex.AccountingReports.Core.Services.Impl;
    using Fanex.AccountingReports.Core.Web;
    using Fanex.AccountingReports.Special.Criteria.DailyTurnover;
    using Fanex.AccountingReports.Special.Models.DailyTurnover;
    using Fanex.Common;

    /// <summary>
    /// The implement service of detail report.
    /// </summary>
    public class DailyDetailChangedTurnoverService : ServiceBase
    {
        private readonly string _dailyTurnoverPrefixSessionKey =
                                        "Fanex.AccountingReports.Special.DailyTurnover.";

        private readonly string _dailyTurnoverDetailPrefixSessionKey =
                                        "Fanex.AccountingReports.Special.DailyTurnover.Detail.";

        private readonly DailyTurnoverService _reportService;

        private string _dailyTurnoverDetailSessionKey = string.Empty;

        private string _dailyTurnoverSessionKey = string.Empty;

        public DailyDetailChangedTurnoverService()
        {
            if (AppSession.CurrentUser != null)
            {
                this._dailyTurnoverDetailSessionKey = this._dailyTurnoverDetailPrefixSessionKey
                                                            + AppSession.CurrentUser.Username;
                this._dailyTurnoverSessionKey = this._dailyTurnoverPrefixSessionKey
                                                            + AppSession.CurrentUser.Username;
            }
            else
            {
                this._dailyTurnoverDetailSessionKey = this._dailyTurnoverDetailPrefixSessionKey;
                this._dailyTurnoverSessionKey = this._dailyTurnoverPrefixSessionKey;
            }

            this._reportService = new DailyTurnoverService();
        }

        /// <summary>
        /// Get detail report.
        /// </summary>
        /// <param name="criteria">The criteria.</param>
        /// <returns>The list DailyDetailChangedTurnover.</returns>
        public List<DailyDetailChangedTurnover> GetDetailReport(
            GetDailyTurnoverChangedDetailCriteria criteria)
        {
            var report = new List<DailyDetailChangedTurnover>();

            if (!criteria.IsValid())
            {
                return report;
            }

            report = this.ConvertAndPutDataIntoReport(criteria);

            SessionManager.Set<List<DailyDetailChangedTurnover>>(
                                            this._dailyTurnoverDetailSessionKey,
                                            report);

            return report;
        }

        /// <summary>
        /// Convert and put data into report.
        /// </summary>
        /// <param name="criteria">The criteria.</param>
        /// <returns>The list DailyDetailChangedTurnover.</returns>
        private List<DailyDetailChangedTurnover> ConvertAndPutDataIntoReport(
            GetDailyTurnoverChangedDetailCriteria criteria)
        {
            var report = new List<DailyDetailChangedTurnover>();

            var rowsData = GetReportData<dynamic>(criteria);

            var virtualDailyTurnover = new TurnoverInformation();
            var actualDailyturnover = new TurnoverInformation();

            this.GetDailyTurnoverByCurrency(ref virtualDailyTurnover, ref actualDailyturnover, criteria);

            LoopAndPushDataIntoReport(
                ref report,
                rowsData,
                virtualDailyTurnover,
                actualDailyturnover);

            return report;
        }

        /// <summary>
        /// Loop, convert and push data into report.
        /// </summary>
        /// <param name="report">The report.</param>
        /// <param name="rowsData">The rows data.</param>
        /// <param name="virtualDailyTurnover">The virtual daily turnover.</param>
        /// <param name="actualDailyturnover">The actual daily turnover.</param>
        private void LoopAndPushDataIntoReport(
            ref List<DailyDetailChangedTurnover> report,
            IEnumerable<dynamic> rowsData,
            TurnoverInformation virtualDailyTurnover,
            TurnoverInformation actualDailyturnover)
        {
            int index = 0;
            foreach (var row in rowsData)
            {
                DateTime dateTime = Converter.ToDateTime(row.DateTime);
                var turnoverDifferent = this.GetTurnoverDifferent(row);

                var dailyDetailTurnover = new DailyDetailChangedTurnover
                {
                    Date = dateTime.ToLongFormatedDate(),
                    ActionName = Converter.ToString2(row.Action),
                    Username = Converter.ToString2(row.Username),
                    Match = Converter.ToString2(row.Detail),
                    TurnoverDifference = turnoverDifferent
                };

                if (index == 0)
                {
                    dailyDetailTurnover.TurnoverAmount.Add(virtualDailyTurnover);
                    dailyDetailTurnover.TurnoverAmount.Add(actualDailyturnover);
                }
                else
                {
                    var beforeDetailTurnover = report[report.Count - 1];
                    var virtualTurnoverAmount = this.CalculateTurnoverAmount(
                                                beforeDetailTurnover.TurnoverAmount[0],
                                                beforeDetailTurnover.TurnoverDifference[0]);

                    var actualTurnoverAmount = this.CalculateTurnoverAmount(
                                                beforeDetailTurnover.TurnoverAmount[1],
                                                beforeDetailTurnover.TurnoverDifference[1]);

                    dailyDetailTurnover.TurnoverAmount.Add(virtualTurnoverAmount);
                    dailyDetailTurnover.TurnoverAmount.Add(actualTurnoverAmount);
                }

                report.Add(dailyDetailTurnover);
                index++;
            }
        }

        /// <summary>
        /// Convert and get turnover difference.
        /// </summary>
        /// <param name="row">The data row from database.</param>
        /// <returns>The list TurnoverInformation.</returns>
        private List<TurnoverInformation> GetTurnoverDifferent(dynamic row)
        {
            var turnoverDifferent = new List<TurnoverInformation>();
            var actualTurnoverDifferent = new TurnoverInformation
            {
                TotalTurnover = Converter.ToDecimal(row.Total_Actual),
                SuperTurnover = Converter.ToDecimal(row.Super_Actual),
                MasterTurnover = Converter.ToDecimal(row.Master_Actual),
                AgentTurnover = Converter.ToDecimal(row.Agent_Actual),
                IBCTurnover = Converter.ToDecimal(row.IBC_Actual),
                IsActual = true
            };
            var virtualTurnoverDifferent = new TurnoverInformation
            {
                TotalTurnover = Converter.ToDecimal(row.Total_Virtual),
                SuperTurnover = Converter.ToDecimal(row.Super_Virtual),
                MasterTurnover = Converter.ToDecimal(row.Master_Virtual),
                AgentTurnover = Converter.ToDecimal(row.Agent_Virtual),
                IBCTurnover = Converter.ToDecimal(row.IBC_Virtual),
                IsActual = false
            };

            turnoverDifferent.Add(virtualTurnoverDifferent);
            turnoverDifferent.Add(actualTurnoverDifferent);

            return turnoverDifferent;
        }

        /// <summary>
        /// Get daily turnover by currency.
        /// </summary>
        /// <param name="virtualDailyTurnover">The virtual currency.</param>
        /// <param name="actualDailyTurnover">The actual currency.</param>
        /// <param name="detailCriteria">The detail criteria.</param>
        private void GetDailyTurnoverByCurrency(
            ref TurnoverInformation virtualDailyTurnover,
            ref TurnoverInformation actualDailyTurnover,
            GetDailyTurnoverChangedDetailCriteria detailCriteria)
        {
            var dailyTurnover = SessionManager.Get<List<DailyTurnover>>(
                                                this._dailyTurnoverSessionKey);

            if (dailyTurnover == null)
            {
                GetDailyTurnoverCriteria criteria = new GetDailyTurnoverCriteria();
                criteria.FromDate = detailCriteria.FromDate;
                this._reportService.GetReport(criteria);

                SessionManager.Set<List<DailyTurnover>>(
                                                this._dailyTurnoverSessionKey,
                                                dailyTurnover);
            }

            if (dailyTurnover != null && dailyTurnover.Count >= 2)
            {
                virtualDailyTurnover = dailyTurnover[0].Turnover;
                actualDailyTurnover = dailyTurnover[1].Turnover;
            }
        }

        /// <summary>
        /// Calculate turnover amount of each event.
        /// </summary>
        /// <param name="beforeTurnover">The before event.</param>
        /// <param name="differenceTurnover">The different turnover.</param>
        /// <returns>The TurnoverInformation.</returns>
        private TurnoverInformation CalculateTurnoverAmount(
            TurnoverInformation beforeTurnover,
            TurnoverInformation differenceTurnover)
        {
            var turnoverByCurrency = new TurnoverInformation();

            turnoverByCurrency.TotalTurnover = beforeTurnover.TotalTurnover
                                                - differenceTurnover.TotalTurnover;
            turnoverByCurrency.SuperTurnover = beforeTurnover.SuperTurnover
                                                - differenceTurnover.SuperTurnover;
            turnoverByCurrency.MasterTurnover = beforeTurnover.MasterTurnover
                                                - differenceTurnover.MasterTurnover;
            turnoverByCurrency.AgentTurnover = beforeTurnover.AgentTurnover
                                                - differenceTurnover.AgentTurnover;
            turnoverByCurrency.IBCTurnover = beforeTurnover.IBCTurnover
                                                - differenceTurnover.IBCTurnover;
            turnoverByCurrency.IsActual = beforeTurnover.IsActual;

            return turnoverByCurrency;
        }
    }
}