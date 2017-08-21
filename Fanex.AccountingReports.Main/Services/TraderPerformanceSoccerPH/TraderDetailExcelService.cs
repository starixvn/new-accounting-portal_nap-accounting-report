using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Web;
using Fanex.AccountingReports.Main.Criteria.TraderPerformance;
using Fanex.AccountingReports.Main.Repositories.TraderPerformanceSoccerPH;
using Fanex.Common;
using OfficeOpenXml;

namespace Fanex.AccountingReports.Main.Services.TraderPerformanceSoccerPH
{
    public interface TraderDetailExcelService
    {
        void Execute(GetDetailPHCriteria criteria);
    }

    public class TraderDetailExcelServiceImp : TraderDetailExcelService
    {
        private const int BeginContentRowIndex = 6;
        private const int BeginColumnMainGroupIndex = 8;

        private readonly TraderDataService traderDataService;
        private readonly TraderDetailPivotService traderDetailPivotService;
        private readonly ExcelPackage excelPackage;
        private ExcelWorksheet templateSheet;
        private ExcelWorksheet sheetTrader;

        private int currentContentRowIndex;

        public TraderDetailExcelServiceImp(TraderDataService traderDataService, TraderDetailPivotService traderDetailPivotService, ExcelPackage excelPackage)
        {
            this.traderDataService = traderDataService;
            this.traderDetailPivotService = traderDetailPivotService;
            this.excelPackage = excelPackage;
        }

        public void Execute(GetDetailPHCriteria criteria)
        {
            var rawData = traderDataService.Query(criteria);
            var dataTable = traderDetailPivotService.Execute(rawData);
            var columnHeaders = new Collection<string>();
            int lastColumnIndex = dataTable.Columns.Count;

            sheetTrader = excelPackage.Workbook.Worksheets.Add("TraderPerformanceSoccerPHDetail");
            templateSheet = excelPackage.Workbook.Worksheets["Content"];

            RenderTitle(lastColumnIndex, criteria);
            RenderHeader(lastColumnIndex, dataTable, columnHeaders);
            MergeHeader(columnHeaders);
            SetHeaderStyle(lastColumnIndex);
            RenderContent(dataTable, lastColumnIndex);
            ClearSheet();
            Save(criteria);
        }

        private void RenderTitle(int lastColumnIndex, GetDetailPHCriteria criteria)
        {
            sheetTrader.Cells[1, 1, 2, lastColumnIndex].Value = $"Trader Performance For SOCCER PH Detail - From Date: {criteria.Fromdate:MMM dd, yyyy} To Date: {criteria.Todate:MMM dd, yyyy}";
            sheetTrader.Cells[1, 1, 2, lastColumnIndex].StyleID = templateSheet.Cells["A1"].StyleID;
            sheetTrader.Cells[1, 1, 2, lastColumnIndex].Merge = true;
        }

        private void RenderHeader(int lastColumnIndex, DataTable dataTable, ICollection<string> columnHeaders)
        {
            for (var columnIndex = 0; columnIndex < lastColumnIndex; columnIndex++)
            {
                var arrayBettypes = dataTable.Columns[columnIndex].ColumnName.Split('_');
                var firstHeader = arrayBettypes[0];
                var secondHeader = arrayBettypes.Length == 1
                    ? arrayBettypes[0]
                    : arrayBettypes[1];

                sheetTrader.Cells[4, columnIndex + 1].Value = firstHeader;
                sheetTrader.Cells[5, columnIndex + 1].Value = secondHeader;
                columnHeaders.Add(firstHeader);
            }

            sheetTrader.Row(4).Height = 17;
            sheetTrader.Row(5).Height = 17;
        }

        private void MergeHeader(IEnumerable<string> columnHeaders)
        {
            var startColumn = 1;
            var startRow = 4;
            columnHeaders
                .Distinct()
                .ToList()
                .ForEach(firstHeader =>
                {
                    var numberOfBettypes = columnHeaders.Count(columnName => columnName == firstHeader);
                    if (numberOfBettypes == 1)
                    {
                        sheetTrader.Cells[startRow, startColumn, startRow + 1, startColumn].Merge = true;
                    }
                    else
                    {
                        sheetTrader.Cells[startRow, startColumn, startRow, startColumn + numberOfBettypes - 1].Merge = true;
                    }

                    startColumn += numberOfBettypes;
                });
        }

        private void SetHeaderStyle(int lastColumnIndex)
        {
            sheetTrader.Cells[4, 1, 5, lastColumnIndex].StyleID = templateSheet.Cells["A4"].StyleID;
            sheetTrader.Cells["A4"].Value = "No";
            sheetTrader.View.FreezePanes(6, BeginColumnMainGroupIndex);
        }

        private void RenderContent(DataTable dataTable, int lastColumnIndex)
        {
            currentContentRowIndex = BeginContentRowIndex;
            var userIndex = 1;
            dataTable.AsEnumerable()
                .Select(s => Converter.To<string>(s["Trader"]))
                .Distinct()
                .ToList()
                .ForEach(trader =>
                {
                    var rowsByUser = dataTable.AsEnumerable().Where(s => Converter.To<string>(s["Trader"]) == trader).ToList();
                    for (var rowIndex = 0; rowIndex < rowsByUser.Count; rowIndex++)
                    {
                        var row = rowsByUser[rowIndex];
                        RenderContentByUser(lastColumnIndex, userIndex, row);
                        currentContentRowIndex++;
                    }

                    MergeAndSetSyleContentByUser(lastColumnIndex, trader, dataTable);
                    CalculateSubTotalByUser(lastColumnIndex, rowsByUser, dataTable);
                    currentContentRowIndex++;
                    userIndex++;
                });

            CalculateGrandTotal(lastColumnIndex, dataTable);
            sheetTrader.Cells.AutoFitColumns();
        }

        private void RenderContentByUser(int lastColumnIndex, int userIndex, DataRow row)
        {
            for (var columnIndex = 1; columnIndex <= lastColumnIndex; columnIndex++)
            {
                if (columnIndex == 1)
                {
                    sheetTrader.Cells[currentContentRowIndex, columnIndex].Value = userIndex;
                }
                else if (columnIndex == 3)
                {
                    var value = row[columnIndex - 1].ToString();
                    if (!string.IsNullOrEmpty(value))
                    {
                        sheetTrader.Cells[currentContentRowIndex, columnIndex].Value = Converter.To<DateTime>(row[columnIndex - 1]);
                    }
                }
                else
                {
                    sheetTrader.Cells[currentContentRowIndex, columnIndex].Value = row[columnIndex - 1];
                }
            }
        }

        private void MergeAndSetSyleContentByUser(int lastColumnIndex, string trader, DataTable dataTable)
        {
            var rowsByUser = dataTable.AsEnumerable().Where(s => s["Trader"].ToString() == trader).ToList();
            var beginContentRowIndex = currentContentRowIndex - rowsByUser.Count;
            var endContentRowIndex = currentContentRowIndex - 1;

            sheetTrader.Cells[$"A{beginContentRowIndex}:A{endContentRowIndex}"].Merge = true;
            sheetTrader.Cells[$"B{beginContentRowIndex}:B{endContentRowIndex}"].Merge = true;

            sheetTrader.Cells[$"A{beginContentRowIndex}:A{endContentRowIndex}"].StyleID = templateSheet.Cells["A6"].StyleID;
            sheetTrader.Cells[$"B{beginContentRowIndex}:B{endContentRowIndex}"].StyleID = templateSheet.Cells["B6"].StyleID;
            sheetTrader.Cells[$"C{beginContentRowIndex}:C{endContentRowIndex}"].StyleID = templateSheet.Cells["C6"].StyleID;
            sheetTrader.Cells[$"D{beginContentRowIndex}:E{endContentRowIndex}"].StyleID = templateSheet.Cells["D6"].StyleID;
            sheetTrader.Cells[$"F{beginContentRowIndex}:F{endContentRowIndex}"].StyleID = templateSheet.Cells["F6"].StyleID;
            sheetTrader.Cells[$"G{beginContentRowIndex}:H{endContentRowIndex}"].StyleID = templateSheet.Cells["G6"].StyleID;

            sheetTrader.Cells[currentContentRowIndex, 1, currentContentRowIndex, BeginColumnMainGroupIndex - 1].Value = trader;
            sheetTrader.Cells[currentContentRowIndex, 1, currentContentRowIndex, BeginColumnMainGroupIndex - 1].Merge = true;
            sheetTrader.Cells[currentContentRowIndex, 1, currentContentRowIndex, BeginColumnMainGroupIndex - 1].StyleID = templateSheet.Cells["A10"].StyleID;

            var styles = new Dictionary<string, int>
            {
                { "Win Loss_",  templateSheet.Cells["H6"].StyleID },
                { "Turnover_",  templateSheet.Cells["I6"].StyleID },
                { "Live BetCount_",  templateSheet.Cells["J6"].StyleID },
                { "Commission_",  templateSheet.Cells["L6"].StyleID },
                { "Margin %_",  templateSheet.Cells["K6"].StyleID },
                { "others",  templateSheet.Cells["M6"].StyleID }
            };

            for (var columnIndex = BeginColumnMainGroupIndex; columnIndex <= lastColumnIndex; columnIndex++)
            {
                var columnName = dataTable.Columns[columnIndex - 1].ColumnName;
                var result = styles.FirstOrDefault(s => columnName.Contains(s.Key));
                if (string.IsNullOrEmpty(result.Key))
                {
                    sheetTrader.Cells[beginContentRowIndex, columnIndex, endContentRowIndex, columnIndex].StyleID = styles["others"];
                }
                else
                {
                    sheetTrader.Cells[beginContentRowIndex, columnIndex, endContentRowIndex, columnIndex].StyleID = result.Value;
                }
            }
        }

        private void CalculateSubTotalByUser(int lastColumnIndex, IEnumerable<DataRow> rowsByUser, DataTable dataTable)
        {
            var styles = new Dictionary<string, int>
            {
                { "Win Loss_",  templateSheet.Cells["H10"].StyleID },
                { "Turnover_",  templateSheet.Cells["I10"].StyleID },
                { "Live BetCount_",  templateSheet.Cells["J10"].StyleID },
                { "Commission_",  templateSheet.Cells["L10"].StyleID },
                { "Margin %_",  templateSheet.Cells["K10"].StyleID },
                { "others",  templateSheet.Cells["M10"].StyleID }
            };

            for (var columnIndex = BeginColumnMainGroupIndex; columnIndex <= lastColumnIndex; columnIndex++)
            {
                var columnName = dataTable.Columns[columnIndex - 1].ColumnName;
                var result = styles.FirstOrDefault(s => columnName.Contains(s.Key));
                if (string.IsNullOrEmpty(result.Key))
                {
                    sheetTrader.Cells[currentContentRowIndex, columnIndex].StyleID = styles["others"];
                }
                else
                {
                    sheetTrader.Cells[currentContentRowIndex, columnIndex].StyleID = result.Value;
                }

                if (columnName.Contains("Margin %_"))
                {
                    decimal margin = CalculateMargin(rowsByUser, columnName);
                    sheetTrader.Cells[currentContentRowIndex, columnIndex].Value = margin;
                }
                else
                {
                    sheetTrader.Cells[currentContentRowIndex, columnIndex].Value = rowsByUser.Sum(s => Converter.To<decimal>(s[columnIndex - 1]));
                }
            }
        }

        private static decimal CalculateMargin(IEnumerable<DataRow> rows, string columnName)
        {
            var bettype = columnName.Split('_')[1];
            var suffixWinLoss = columnName.Contains("Overall") ? "_Win Loss" : string.Empty;
            var suffixTurover = columnName.Contains("Overall") ? "_Turnover" : string.Empty;
            var sumWL = rows.Sum(s => Converter.To<decimal>(s[$"Win Loss_{bettype}{suffixWinLoss}"]));
            var sumTO = rows.Sum(s => Converter.To<decimal>(s[$"Turnover_{bettype}{suffixTurover}"]));
            var margin = sumTO == 0 ? 0.0m : sumWL / sumTO;
            return margin;
        }

        private void CalculateGrandTotal(int lastColumnIndex, DataTable dataTable)
        {
            var styles = new Dictionary<string, int>
            {
                { "Win Loss_",  templateSheet.Cells["H11"].StyleID },
                { "Turnover_",  templateSheet.Cells["I11"].StyleID },
                { "Live BetCount_",  templateSheet.Cells["J11"].StyleID },
                { "Commission_",  templateSheet.Cells["L11"].StyleID },
                { "Margin %_",  templateSheet.Cells["K11"].StyleID },
                { "others",  templateSheet.Cells["M11"].StyleID }
            };

            for (var columnIndex = BeginColumnMainGroupIndex; columnIndex <= lastColumnIndex; columnIndex++)
            {
                var columnName = dataTable.Columns[columnIndex - 1].ColumnName;
                var result = styles.FirstOrDefault(s => columnName.Contains(s.Key));
                if (string.IsNullOrEmpty(result.Key))
                {
                    sheetTrader.Cells[currentContentRowIndex, columnIndex].StyleID = styles["others"];
                }
                else
                {
                    sheetTrader.Cells[currentContentRowIndex, columnIndex].StyleID = result.Value;
                }

                if (columnName.Contains("Margin %_"))
                {
                    decimal margin = CalculateMargin(dataTable.AsEnumerable(), columnName);
                    sheetTrader.Cells[currentContentRowIndex, columnIndex].Value = margin;
                }
                else
                {
                    sheetTrader.Cells[currentContentRowIndex, columnIndex].Value = dataTable.AsEnumerable().Sum(s => Converter.To<decimal>(s[columnIndex - 1]));
                }
            }

            sheetTrader.Cells[currentContentRowIndex, 1, currentContentRowIndex, BeginColumnMainGroupIndex - 1].Value = "Grand Total";
            sheetTrader.Cells[currentContentRowIndex, 1, currentContentRowIndex, BeginColumnMainGroupIndex - 1].Merge = true;
            sheetTrader.Cells[currentContentRowIndex, 1, currentContentRowIndex, BeginColumnMainGroupIndex - 1].StyleID = templateSheet.Cells["A11"].StyleID;
        }

        private void ClearSheet()
        {
            excelPackage.Workbook.Worksheets.Delete(templateSheet);
        }

        private void Save(GetDetailPHCriteria criteria)
        {
            var fileName = $@"TraderPerformance_SoccerPH_Detail_From_Date_{criteria.Fromdate:MM_dd_yyyy}_To_Date_{criteria.Todate:MM_dd_yyyy}";

            var response = HttpContext.Current.Response;
            response.Clear();
            response.ContentType = "application/vnd.ms-excel";
            response.AddHeader("Content-Disposition", string.Format("attachment; filename={0}.xlsx", fileName));
            response.BinaryWrite(excelPackage.GetAsByteArray());
        }
    }
}