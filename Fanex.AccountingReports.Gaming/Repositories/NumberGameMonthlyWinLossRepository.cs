namespace Fanex.AccountingReports.Gaming.Repositories.Implement
{
    using System.Collections.Generic;
    using Fanex.AccountingReports.Gaming.Criteria.NumberGameMonthlyWinLoss;
    using Fanex.AccountingReports.Gaming.Models.NumberGameMonthlyWinLoss;
    using Fanex.AccountingReports.Gaming.Resources;
    using Fanex.Common;
    using Fanex.Data;

    /// <summary>
    /// Number Game Monthly Win Loss Repository.
    /// </summary>
    public class NumberGameMonthlyWinLossRepository
    {
        /// <summary>
        /// Gets the number game monthly win loss.
        /// </summary>
        /// <param name="getNumberGameMonthlyWinLossCriteria">The get number game monthly win loss criteria.</param>
        /// <returns>List report data.</returns>
        public List<NumberGameMonthlyWinLoss> GetNumberGameMonthlyWinLoss(NumberGameMonthlyWinLossCriteria getNumberGameMonthlyWinLossCriteria)
        {
            var report = new NumberGameMonthlyWinLoss();

            using (var db = ObjectDbFactory.CreateInstance(getNumberGameMonthlyWinLossCriteria.GetSettingKey()))
            {
                var param = getNumberGameMonthlyWinLossCriteria;
                var reader = db.ExecuteReader(param);

                if (reader == null)
                {
                    return null;
                }

                while (reader.Read())
                {
                    var lineTotal = Converter.ToDecimal(reader["linetotal"]);
                    var turnOver = Converter.ToDecimal(reader["Turnover"]);
                    var margin = GetMarginValue(lineTotal, turnOver);

                    report.ReportRows.Add(new NumberGameMonthlyWinLoss()
                    {
                        Month = Converter.ToInt(reader["Month"]),
                        Year = Converter.ToInt(reader["Year"]),
                        Winloss = Converter.ToDecimal(reader["winlost"]),
                        Position = Converter.ToDecimal(reader["position"]),
                        Commission = Converter.ToDecimal(reader["commission"]),
                        Linetotal = lineTotal,
                        Turnover = turnOver,
                        Margin = margin
                    });
                }
            }

            return report.ReportRows;
        }

        private string GetMarginValue(decimal lineTotal, decimal turnOver)
        {
            return turnOver == 0 ? Labels.NotAvailable : (lineTotal / turnOver).ToString();
        }
    }
}