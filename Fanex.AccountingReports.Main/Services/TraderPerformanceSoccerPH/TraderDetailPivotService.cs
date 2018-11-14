using System.Collections.Generic;
using System.Data;
using System.Linq;
using Fanex.Common;

namespace Fanex.AccountingReports.Main.Services.TraderPerformanceSoccerPH
{
    public class TraderDetailPivotServiceImp
    {
        private const string BettypeColumnName = "BettypeName";
        private const string TraderColmunName = "Trader";

        public DataTable Execute(DataTable rawData)
        {
            var dataTable = new DataTable();
            var bettypes = GetBettypes(rawData);
            var pivotColumns = new List<string> { BettypeColumnName, "Win Loss", "Turnover", "Live BetCount", "Margin %", "Commission" };

            CreateNewStructureTable(rawData, dataTable, bettypes, pivotColumns);
            InsertDataIntoNewTable(rawData, dataTable, bettypes, pivotColumns);

            return dataTable;
        }

        private static void InsertDataIntoNewTable(DataTable rawData, DataTable dataTable, ICollection<string> bettypes, ICollection<string> pivotColumns)
        {
            rawData
                .AsEnumerable()
                .Select(s => Converter.To<string>(s[TraderColmunName]))
                .Distinct()
                .ToList()
                .ForEach(trader =>
                {
                    var dataByUser = rawData.AsEnumerable().Where(s => Converter.To<string>(s[TraderColmunName]) == trader).ToList();
                    dataByUser
                    .AsEnumerable()
                    .Select(s => Converter.To<string>(s["Match"]))
                    .Distinct()
                    .ToList()
                    .ForEach(match =>
                    {
                        var dataByMatch = dataByUser.AsEnumerable().Where(s => Converter.To<string>(s["Match"]) == match).ToList();
                        var newDataRow = dataTable.NewRow();
                        for (var columnIndex = 0; columnIndex < rawData.Columns.Count; columnIndex++)
                        {
                            var rawColumnName = rawData.Columns[columnIndex].ColumnName;
                            if (pivotColumns.Contains(rawColumnName))
                            {
                                if (rawColumnName != BettypeColumnName)
                                {
                                    foreach (var bettype in bettypes)
                                    {
                                        var columnName = $"{rawColumnName}_{bettype}";
                                        if (columnName.Contains("Overall") && !bettype.Contains(rawColumnName))
                                        {
                                            continue;
                                        }

                                        var row = dataByMatch.FirstOrDefault(s => Converter.To<string>(s[BettypeColumnName]) == bettype);
                                        newDataRow[columnName] = row == null ? 0 : row[columnIndex];
                                    }
                                }
                            }
                            else
                            {
                                newDataRow[columnIndex] = dataByMatch.FirstOrDefault()[columnIndex];
                            }
                        }

                        dataTable.Rows.Add(newDataRow);
                    });
                });
        }

        private static void CreateNewStructureTable(DataTable rawData, DataTable dataTable, ICollection<string> bettypes, ICollection<string> pivotColumns)
        {
            for (var columnIndex = 0; columnIndex < rawData.Columns.Count; columnIndex++)
            {
                var rawColumnName = rawData.Columns[columnIndex].ColumnName;
                if (pivotColumns.Contains(rawColumnName))
                {
                    if (rawColumnName != BettypeColumnName)
                    {
                        foreach (var bettype in bettypes)
                        {
                            var columnName = $"{rawColumnName}_{bettype}";
                            if (columnName.Contains("Overall") && !bettype.Contains(rawColumnName))
                            {
                                continue;
                            }

                            dataTable.Columns.Add(columnName, rawData.Columns[columnIndex].DataType);
                        }
                    }
                }
                else
                {
                    dataTable.Columns.Add(rawData.Columns[columnIndex].ColumnName, rawData.Columns[columnIndex].DataType);
                }
            }
        }

        private static List<string> GetBettypes(DataTable rawData)
        {
            var userNameWithMaxNumberOfBettypes = rawData.AsEnumerable()
                .GroupBy(g => Converter.To<string>(g[TraderColmunName]))
                .Select(s => new { trader = s.Key, count = s.Count() })
                .OrderByDescending(o => o.count)
                .FirstOrDefault();

            var trader = userNameWithMaxNumberOfBettypes.trader;
            return rawData.AsEnumerable()
                .Where(s => Converter.To<string>(s[TraderColmunName]) == trader)
                .Select(s => Converter.To<string>(s[BettypeColumnName]))
                .Distinct()
                .ToList();
        }
    }
}