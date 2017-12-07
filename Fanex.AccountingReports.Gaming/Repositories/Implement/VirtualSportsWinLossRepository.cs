namespace Fanex.AccountingReports.Gaming.Repositories.Implement
{
    using System.Collections.Generic;
    using System.Data;
    using System.Linq;
    using Fanex.AccountingReports.Core.Criteria;
    using Fanex.AccountingReports.Core.Services.Impl;
    using Fanex.AccountingReports.Gaming.Criteria.VirtualSportsWinLoss;
    using Fanex.AccountingReports.Gaming.Models.VirtualSportsWinLoss;
    using Fanex.AccountingReports.Gaming.Repositories.Interface;
    using Fanex.AccountingReports.Gaming.Resources;
    using Fanex.Common;
    using Fanex.Data;

    /// <summary>
    /// The repository of Virtual Sports Win Loss report.
    /// </summary>
    public class VirtualSportsWinLossRepository : IVirtualSportsWinLossRepository
    {
        /// <summary>
        /// Get report.
        /// </summary>
        /// <param name="criteria">The criteria.</param>
        /// <returns>The report.</returns>
        public VirtualSportsWinLoss GetVirtualSportsWinLossReport(GetVirtualSportsWinLossCriteria criteria)
        {
            var report = new VirtualSportsWinLoss();
            var settingKey = criteria.GetSettingKey();

            using (ObjectDb db = new ObjectDb(settingKey))
            {
                var multipleResult = db.ExecuteDataSet(criteria.GetSpParams());
                if (multipleResult != null && multipleResult.Tables.Count >= 2)
                {
                    var dateRows = multipleResult.Tables[0].AsEnumerable();
                    var totalRows = multipleResult.Tables[1].Rows;

                    var numberOfRows = 0;
                    report.Rows = this.ConvertDateResult(dateRows, ref numberOfRows);
                    report.Rows.Add(this.ConvertTotalResult(dateRows, ref numberOfRows));
                    report.Currencies = this.ConvertGrandTotalResult(totalRows);
                    report.Date = numberOfRows.ToString();
                }
            }

            return report;
        }

        /// <summary>
        /// Convert date rows into report.
        /// </summary>
        /// <param name="dateRows">The date rows.</param>
        /// <param name="numberOfRows">The number of rows.</param>
        /// <returns>The list dates.</returns>
        private List<VirtualSportsWinLoss> ConvertDateResult(
            EnumerableRowCollection<DataRow> dateRows,
            ref int numberOfRows)
        {
            var listDates = new List<VirtualSportsWinLoss>();
            var groupByDate = dateRows.GroupBy(row => row["rptDate"]).Distinct().ToList();
            var currencies = ReportService.ExchangeCurrencies;

            foreach (var date in groupByDate)
            {
                var dateWinLoss = Converter.ToDateTime(date.Key);
                var currencyWinLossByDate = new VirtualSportsWinLoss(dateWinLoss.ToString("MMM dd, yyyy"));

                var rowsByDate = date.Where(row =>
                        Converter.ToDateTime(row["rptDate"]).Equals(dateWinLoss)).ToList();

                foreach (var row in rowsByDate)
                {
                    var currencyId = Converter.ToInt(row["Currency"]);
                    var currencyName = currencies[currencyId];

                    var currency = new VirtualSportsWinLossDetail
                    {
                        Name = currencyName,
                        BetCount = Converter.ToInt(row["BetCount"]),
                        Turnover = Converter.ToDecimal(row["TurnOver"]),
                        MemberWinLoss = Converter.ToDecimal(row["MemberWL"]),
                        PositionWinLoss = Converter.ToDecimal(row["PositionWL"]),
                        CompanyWinLoss = Converter.ToDecimal(row["CompanyWL"]),
                        AmountDueInspired = Converter.ToDecimal(row["amtDuePartner"])
                    };

                    currencyWinLossByDate.Currencies.Add(currency);
                    numberOfRows++;
                }

                listDates.Add(currencyWinLossByDate);
            }

            return listDates;
        }

        /// <summary>
        /// Convert total result.
        /// </summary>
        /// <param name="dateRows">The date rows.</param>
        /// <param name="numberOfRows">The number of rows.</param>
        /// <returns>The total row.</returns>
        private VirtualSportsWinLoss ConvertTotalResult(
            EnumerableRowCollection<DataRow> dateRows,
            ref int numberOfRows)
        {
            var currencies = ReportService.ExchangeCurrencies;
            var totalRow = new VirtualSportsWinLoss(Labels.Total);
            var listCurrencyId = dateRows.GroupBy(row => row["Currency"]).Distinct().ToList();
            numberOfRows += listCurrencyId.Count;

            foreach (var currencyId in listCurrencyId)
            {
                var currencyName = currencies[Converter.ToInt(currencyId.Key)];
                var currencyInTotal = new VirtualSportsWinLossDetail
                {
                    Name = currencyName
                };

                var rowsByCurrency = dateRows.Where(row => row["Currency"].Equals(currencyId.Key)).ToList();

                foreach (var row in rowsByCurrency)
                {
                    var rowCurrency = new VirtualSportsWinLossDetail
                    {
                        BetCount = Converter.ToInt(row["BetCount"]),
                        Turnover = Converter.ToDecimal(row["TurnOver"]),
                        MemberWinLoss = Converter.ToDecimal(row["MemberWL"]),
                        PositionWinLoss = Converter.ToDecimal(row["PositionWL"]),
                        CompanyWinLoss = Converter.ToDecimal(row["CompanyWL"]),
                        AmountDueInspired = Converter.ToDecimal(row["amtDuePartner"])
                    };

                    currencyInTotal.SumTotal(rowCurrency);
                }

                totalRow.Currencies.Add(currencyInTotal);
            }

            return totalRow;
        }

        /// <summary>
        /// Convert grand total row.
        /// </summary>
        /// <param name="totalRows">The total rows.</param>
        /// <returns>The currencies.</returns>
        private List<VirtualSportsWinLossDetail> ConvertGrandTotalResult(
            DataRowCollection totalRows)
        {
            var currencies = new List<VirtualSportsWinLossDetail>();

            foreach (DataRow row in totalRows)
            {
                var totalRM = new VirtualSportsWinLossDetail
                {
                    Name = Labels.Grand_Total_RM,
                    BetCount = Converter.ToInt(row["BetCount"]),
                    Turnover = Converter.ToDecimal(row["TurnOverRM"]),
                    MemberWinLoss = Converter.ToDecimal(row["MemberWLRM"]),
                    PositionWinLoss = Converter.ToDecimal(row["PositionWLRM"]),
                    CompanyWinLoss = Converter.ToDecimal(row["CompanyWLRM"]),
                    AmountDueInspired = Converter.ToDecimal(row["AmtDuePartnerRM"])
                };

                var totalEUR = new VirtualSportsWinLossDetail
                {
                    Name = Labels.Grand_Total_EUR,
                    BetCount = Converter.ToInt(row["BetCount"]),
                    Turnover = Converter.ToDecimal(row["TurnOverEUR"]),
                    MemberWinLoss = Converter.ToDecimal(row["MemberWLEUR"]),
                    PositionWinLoss = Converter.ToDecimal(row["PositionWLEUR"]),
                    CompanyWinLoss = Converter.ToDecimal(row["CompanyWLEUR"]),
                    AmountDueInspired = Converter.ToDecimal(row["AmtDuePartnerEUR"])
                };

                if (totalRM.BetCount > 0)
                {
                    currencies.Add(totalRM);
                    currencies.Add(totalEUR);
                }
            }

            return currencies;
        }
    }
}
