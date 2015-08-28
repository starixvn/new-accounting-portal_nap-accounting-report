// -----------------------------------------------------------------------
// <copyright file="RacingGroupDailyGetReportServices.cs" company="Nexcel Solutions Vietnam">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace Fanex.AccountingReports.Racing.Services.Implement.RacingGroupDaily
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Linq;
    using Fanex.AccountingReports.Core.Services.Impl;
    using Fanex.AccountingReports.Core.Web;
    using Fanex.AccountingReports.Racing.Criteria.RacingGroupDaily;
    using Fanex.AccountingReports.Racing.Models.RacingGroupDaily;
    using Fanex.AccountingReports.Racing.Repositories.Implement;
    using Fanex.AccountingReports.Racing.Repositories.Interface;
    using Fanex.AccountingReports.Racing.Services.Interface.RacingGroupDaily;
    using Fanex.Common;

    /// <summary>
    /// The get report services of Racing Group Daily report.
    /// </summary>
    public class RacingGroupDailyGetReportServices : ServiceBase, IRacingGroupDailyGetReportServices
    {
        private readonly string _racingGroupDailyPreficSessionKey = 
                                "Fanex.AccountingReports.Racing.RacingGroupDaily.";

        private string _racingGroupDailySessionKey = string.Empty;

        private IRacingGroupDailyRepository _racingGroupDailyRepository;

        public RacingGroupDailyGetReportServices()
        {
            if (AppSession.CurrentUser != null)
            {
                this._racingGroupDailySessionKey = this._racingGroupDailyPreficSessionKey +
                                                    AppSession.CurrentUser.Username;
            }
            else
            {
                this._racingGroupDailySessionKey = this._racingGroupDailyPreficSessionKey;
            }

            this._racingGroupDailyRepository = new RacingGroupDailyRepository();
        }

        /// <summary>
        /// Get Racing Group Daily report.
        /// </summary>
        /// <param name="criteria">The criteria.</param>
        /// <returns>The list RacingGroupDaily.</returns>
        public List<RacingGroupDaily> GetReport(GetWinLostRacingGroupCriteria criteria)
        {
            try
            {
                var fullReport = new List<RacingGroupDaily>();
                var pagingReport = new List<RacingGroupDaily>();

                if (!criteria.IsValid())
                {
                    return fullReport;
                }

                if (!criteria.IsPagingCallback)
                {
                    fullReport = this.GetAndConvertData(criteria);
                    SessionManager.Set<List<RacingGroupDaily>>(
                            this._racingGroupDailySessionKey, 
                            fullReport);
                }
                else
                {
                    fullReport = SessionManager.Get<List<RacingGroupDaily>>(
                            this._racingGroupDailySessionKey);
                }

                if (fullReport != null && fullReport.Count > 1)
                {
                    string pagingSessionKey = this._racingGroupDailySessionKey
                            + "." + criteria.FromDate + "." + criteria.ToDate
                            + "." + criteria.StartIndex + "." + criteria.DatesPerPage;
                    pagingReport = this.GetPagingReport(
                                        fullReport, 
                                        pagingReport, 
                                        criteria, 
                                        pagingSessionKey);
                }

                return pagingReport;
            }
            catch (Exception ex)
            {
                this.HandleException(ex);
                throw;
            }
        }

        #region get and convert date from database.

        /// <summary>
        /// Get and convert data from database to Racing Group Daily report.
        /// </summary>
        /// <param name="criteria">The criteria.</param>
        /// <returns>The list RacingGroupDaily.</returns>
        private List<RacingGroupDaily> GetAndConvertData(GetWinLostRacingGroupCriteria criteria)
        {
            var report = new List<RacingGroupDaily>();
            DataTable groupTable = this._racingGroupDailyRepository.GetDataTable(criteria);
            var totalRows = 0;

            report = this.PushDataByDateIntoReport(report, groupTable, ref totalRows);
            report = this.PushPartnerTotalIntoReport(report, groupTable, totalRows);

            return report;
        }

        /// <summary>
        /// Push data by date into report.
        /// </summary>
        /// <param name="report">The report.</param>
        /// <param name="groupTable">The data table.</param>
        /// <param name="totalRows">The total date in report.</param>
        /// <returns>The list RacingGroupDaily.</returns>
        private List<RacingGroupDaily> PushDataByDateIntoReport(
            List<RacingGroupDaily> report, 
            DataTable groupTable,
            ref int totalRows)
        {
            var winLostDates = groupTable.AsEnumerable().Select(row => row["winlostdate"])
                                                        .Distinct().ToList();

            foreach (var date in winLostDates)
            {
                var rowsInDate = groupTable.AsEnumerable().Where(row => row["winlostdate"].Equals(date))
                                                          .ToList();
                DateTime dateShort = Converter.ToDateTime(date);
                var groupByDate = new RacingGroupDaily
                {
                    WinLostDate = dateShort.ToString("MMM dd, yyyy")
                };

                foreach (var row in rowsInDate)
                {
                    var memberInDate = new RacingGroupDaily
                    {
                        Name = Converter.ToString2(row["PartnerName"]),
                        TurnoverAgent = Converter.ToDecimal(row["turnoverAgents"]),
                        TurnoverIBC = Converter.ToDecimal(row["turnoverHouse"]),
                        TotalTurnover = Converter.ToDecimal(row["totalTurnover"]),
                        IncomeIBCPostitionTaking = Converter.ToDecimal(row["incomeHousePT"]),
                        IncomeIBCNetCommission = Converter.ToDecimal(row["incomeHouseNetComm"]),
                        TotalIBCIncome = Converter.ToDecimal(row["totalHouseIncome"]),
                        Income12BETPositionTaking = Converter.ToDecimal(row["income12BETPT"]),
                        Income12BETCommission = Converter.ToDecimal(row["income12BETComm"]),
                        Total12BETIncome = Converter.ToDecimal(row["total12BETIncome"]),
                        GroupTotalIncome = Converter.ToDecimal(row["groupTotalIncome"]),
                        PartnerTurnover = Converter.ToDecimal(row["turnover3rd"]),
                        PartnerDividendWon = Converter.ToDecimal(row["DividendWon3rd"]),
                        PartnerExpenseCommission = Converter.ToDecimal(row["ExpComm3rd"]),
                        PartnerFee = Converter.ToDecimal(row["Fees3rd"]),
                        PartnerAmountDue = Converter.ToDecimal(row["AmountDue23rd"])
                    };

                    groupByDate.Rows.Add(memberInDate);
                    groupByDate.SumTotal(memberInDate);
                    totalRows++;
                }

                report.Add(groupByDate);
            }

            return report;
        }

        /// <summary>
        /// Push the total of partner into report.
        /// </summary>
        /// <param name="report">The report.</param>
        /// <param name="groupTable">The data table.</param>
        /// <param name="totalDate">The total date in report.</param>
        /// <returns>The list RacingGroupDaily.</returns>
        private List<RacingGroupDaily> PushPartnerTotalIntoReport(
            List<RacingGroupDaily> report,
            DataTable groupTable,
            int totalDate)
        {
            var listPartners = groupTable.AsEnumerable().Select(row => row["PartnerName"])
                                                        .Distinct().ToList();
            var grandTotalPartner = new RacingGroupDaily
            {
                WinLostDate = totalDate.ToString(),
                Name = "Total"
            };

            foreach (var partner in listPartners)
            {
                var rowsByPartner = groupTable.AsEnumerable()
                                                .Where(row => row["PartnerName"].Equals(partner))
                                                .ToList();
                var partnerTotal = new RacingGroupDaily
                {
                    Name = Converter.ToString2(partner)
                };

                foreach (var row in rowsByPartner)
                {
                    var rowPartner = new RacingGroupDaily
                    {
                        TurnoverAgent = Converter.ToDecimal(row["turnoverAgents"]),
                        TurnoverIBC = Converter.ToDecimal(row["turnoverHouse"]),
                        TotalTurnover = Converter.ToDecimal(row["totalTurnover"]),
                        IncomeIBCPostitionTaking = Converter.ToDecimal(row["incomeHousePT"]),
                        IncomeIBCNetCommission = Converter.ToDecimal(row["incomeHouseNetComm"]),
                        TotalIBCIncome = Converter.ToDecimal(row["totalHouseIncome"]),
                        Income12BETPositionTaking = Converter.ToDecimal(row["income12BETPT"]),
                        Income12BETCommission = Converter.ToDecimal(row["income12BETComm"]),
                        Total12BETIncome = Converter.ToDecimal(row["total12BETIncome"]),
                        GroupTotalIncome = Converter.ToDecimal(row["groupTotalIncome"]),
                        PartnerTurnover = Converter.ToDecimal(row["turnover3rd"]),
                        PartnerDividendWon = Converter.ToDecimal(row["DividendWon3rd"]),
                        PartnerExpenseCommission = Converter.ToDecimal(row["ExpComm3rd"]),
                        PartnerFee = Converter.ToDecimal(row["Fees3rd"]),
                        PartnerAmountDue = Converter.ToDecimal(row["AmountDue23rd"])
                    };

                    partnerTotal.SumTotal(rowPartner);
                }

                grandTotalPartner.Rows.Add(partnerTotal);
                grandTotalPartner.SumTotal(partnerTotal);
            }

            report.Add(grandTotalPartner);
            return report;
        }

        #endregion get and convert date from database.

        #region get paging report.

        /// <summary>
        /// Get paging report.
        /// </summary>
        /// <param name="fullReport">The full report.</param>
        /// <param name="pagingReport">The paging report.</param>
        /// <param name="criteria">The criteria.</param>
        /// <param name="pagingSessionKey">The paging session key.</param>
        /// <returns>The list RacingGroupDaily.</returns>
        private List<RacingGroupDaily> GetPagingReport(
            List<RacingGroupDaily> fullReport,
            List<RacingGroupDaily> pagingReport,
            GetWinLostRacingGroupCriteria criteria,
            string pagingSessionKey)
        {
            pagingReport = this.GetPagingReportDetail(
                                            fullReport,
                                            criteria);
            return pagingReport;
        }

        /// <summary>
        /// The detail of get paging report.
        /// </summary>
        /// <param name="fullReport">The full report.</param>
        /// <param name="criteria">The criteria.</param>
        /// <returns>The list RacingGroupDaily.</returns>
        private List<RacingGroupDaily> GetPagingReportDetail(
            List<RacingGroupDaily> fullReport,
            GetWinLostRacingGroupCriteria criteria)
        {
            var pagingReport = new List<RacingGroupDaily>();

            pagingReport = this.LoopAndGetPagingReport(fullReport, criteria, pagingReport);
            pagingReport.Add(fullReport[fullReport.Count - 1]);

            return pagingReport;
        }

        /// <summary>
        /// Loop and get paging report.
        /// </summary>
        /// <param name="fullReport">The full report.</param>
        /// <param name="criteria">The criteria.</param>
        /// <param name="pagingReport">The paging report.</param>
        /// <returns>The list RacingGroupDaily.</returns>
        private List<RacingGroupDaily> LoopAndGetPagingReport(
            List<RacingGroupDaily> fullReport,
            GetWinLostRacingGroupCriteria criteria,
            List<RacingGroupDaily> pagingReport)
        {
            var numberOfPartnerInDate = 0;
            var numberOfDates = fullReport.Count - 1;
            var startIndex = criteria.StartIndex;
            var datesPerPage = criteria.DatesPerPage;
            var index = 0;

            while (index < numberOfDates)
            {
                var dateRow = fullReport[index];
                numberOfPartnerInDate = dateRow.Rows.Count;

                if (numberOfPartnerInDate > startIndex)
                {
                    var numberOfPartnerRemain = numberOfPartnerInDate - startIndex;

                    if (numberOfPartnerRemain <= datesPerPage)
                    {
                        this.AddNewDateIntoPagingReport(
                                            startIndex,
                                            ref pagingReport,
                                            dateRow,
                                            numberOfPartnerRemain);

                        if (numberOfPartnerRemain == datesPerPage)
                        {
                            break;
                        }
                        else
                        {
                            startIndex = 0;
                            datesPerPage -= numberOfPartnerRemain;
                        }
                    }
                    else
                    {
                        this.AddNewDateIntoPagingReport(startIndex, ref pagingReport, dateRow, datesPerPage);
                        break;
                    }
                }
                else
                {
                    startIndex -= numberOfPartnerInDate;
                }

                index++;
            }

            return pagingReport;
        }

        /// <summary>
        /// Add new date into paging report.
        /// </summary>
        /// <param name="startIndex">The start index.</param>
        /// <param name="pagingReport">The paging report.</param>
        /// <param name="dateRow">The date row.</param>
        /// <param name="numberOfPartnerRemain">The number partner remain.</param>
        private void AddNewDateIntoPagingReport(
            int startIndex,
            ref List<RacingGroupDaily> pagingReport,
            RacingGroupDaily dateRow,
            int numberOfPartnerRemain)
        {
            var newDate = new RacingGroupDaily
            {
                WinLostDate = dateRow.WinLostDate,
                Name = dateRow.Name,
                TurnoverAgent = dateRow.TurnoverAgent,
                TurnoverIBC = dateRow.TurnoverIBC,
                TotalTurnover = dateRow.TotalTurnover,
                IncomeIBCPostitionTaking = dateRow.IncomeIBCPostitionTaking,
                IncomeIBCNetCommission = dateRow.IncomeIBCNetCommission,
                TotalIBCIncome = dateRow.TotalIBCIncome,
                Income12BETPositionTaking = dateRow.Income12BETPositionTaking,
                Income12BETCommission = dateRow.Income12BETCommission,
                Total12BETIncome = dateRow.Total12BETIncome,
                GroupTotalIncome = dateRow.GroupTotalIncome,
                PartnerTurnover = dateRow.PartnerTurnover,
                PartnerDividendWon = dateRow.PartnerDividendWon,
                PartnerExpenseCommission = dateRow.PartnerExpenseCommission,
                PartnerFee = dateRow.PartnerFee,
                PartnerAmountDue = dateRow.PartnerAmountDue,
                Rows = dateRow.Rows.Skip(startIndex).Take(numberOfPartnerRemain).ToList(),
            };

            pagingReport.Add(newDate);
        }

        #endregion get paging report.
    }
}
