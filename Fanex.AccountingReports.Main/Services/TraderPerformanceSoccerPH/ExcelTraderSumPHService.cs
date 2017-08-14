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
    public interface ExcelTraderSumPHService
    {
        void Execute(GetSummaryPHCriteria criteria);
    }

    public class ExcelTraderSumPHServiceImp : ExcelTraderSumPHService
    {
        private const int BeginContentRowIndex = 6;

        private readonly TraderDataService traderDataService;
        private readonly ExcelPackage excelPackage;

        private readonly ExcelWorksheet templateSheet;
        private readonly ExcelWorksheet sheetTrader;

        public ExcelTraderSumPHServiceImp(TraderDataService traderDataService, ExcelPackage excelPackage)
        {
            this.traderDataService = traderDataService;
            this.excelPackage = excelPackage;

            sheetTrader = excelPackage.Workbook.Worksheets.Add("TraderPerformance_Soccer PH");
            templateSheet = excelPackage.Workbook.Worksheets["Content"];
        }

        public void Execute(GetSummaryPHCriteria criteria)
        {
            var dataTable = traderDataService.Query(criteria);
            var columnHeaders = new Collection<string>();
            int lastColumnIndex = dataTable.Columns.Count;            

            RenderTitle(lastColumnIndex, criteria);
            RenderHeader(dataTable, columnHeaders);
            MergeHeader(columnHeaders);
            SetHeaderStyle(lastColumnIndex);
            RenderContent(dataTable, lastColumnIndex);
            SetContenStyle(dataTable, lastColumnIndex);
            RenderTotal(dataTable, lastColumnIndex);
            ClearSheet();
            Save(criteria);
        }

        private void RenderTitle(int lastColumnIndex, GetSummaryPHCriteria criteria)
        {
            sheetTrader.Cells[1, 1, 2, lastColumnIndex].Value = $"Trader Performance For SOCCER PH - From Date: {criteria.Fromdate:MMM dd, yyyy} To Date: {criteria.Todate:MMM dd, yyyy}";
            sheetTrader.Cells[1, 1, 2, lastColumnIndex].StyleID = templateSheet.Cells["A1"].StyleID;
            sheetTrader.Cells[1, 1, 2, lastColumnIndex].Merge = true;
        }

        private void RenderHeader(DataTable dataTable, ICollection<string> columnHeaders)
        {
            for (var columnIndex = 0; columnIndex < dataTable.Columns.Count; columnIndex++)
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
            sheetTrader.View.FreezePanes(6, 9);
        }

        private void RenderContent(DataTable dataTable, int lastColumnIndex)
        {
            sheetTrader.Cells["A6"].LoadFromDataTable(dataTable, false);
            var lastContentRowIndex = BeginContentRowIndex + dataTable.Rows.Count - 1;

            for (var rowIndex = 0; rowIndex <= dataTable.Rows.Count; rowIndex++)
            {
                var currentRowIndex = BeginContentRowIndex + rowIndex;
                sheetTrader.Cells[$"A{currentRowIndex}"].Value = rowIndex + 1;
            }

            for (var columnIndex = 2; columnIndex <= lastColumnIndex; columnIndex++)
            {
                sheetTrader.Column(columnIndex).Width = 15;
            }

            sheetTrader.Cells[$"A{BeginContentRowIndex}:H{lastContentRowIndex}"].StyleID = templateSheet.Cells["A6"].StyleID;
            sheetTrader.Cells[$"B{BeginContentRowIndex}:H{lastContentRowIndex}"].StyleID = templateSheet.Cells["B6"].StyleID;
            sheetTrader.Cells[$"C{BeginContentRowIndex}:H{lastContentRowIndex}"].StyleID = templateSheet.Cells["H6"].StyleID;
        }

        private void SetContenStyle(DataTable dataTable, int lastColumnIndex)
        {
            var styles = new Dictionary<string, int>
            {
                { "winlost_",  templateSheet.Cells["I6"].StyleID },
                { "stake_",  templateSheet.Cells["J6"].StyleID },
                { "bet_",  templateSheet.Cells["K6"].StyleID },
                { "comm_",  templateSheet.Cells["M6"].StyleID },
                { "margin_",  templateSheet.Cells["L6"].StyleID },
                { "others",  templateSheet.Cells["N6"].StyleID }
            };

            var lastContentRowIndex = BeginContentRowIndex + dataTable.Rows.Count - 1;

            for (var columnIndex = 9; columnIndex <= lastColumnIndex; columnIndex++)
            {
                var columnName = dataTable.Columns[columnIndex - 1].ColumnName;
                var result = styles.FirstOrDefault(s => columnName.Contains(s.Key));
                if (string.IsNullOrEmpty(result.Key))
                {
                    sheetTrader.Cells[BeginContentRowIndex, columnIndex, lastContentRowIndex, columnIndex].StyleID = styles["others"];
                }
                else
                {
                    sheetTrader.Cells[BeginContentRowIndex, columnIndex, lastContentRowIndex, columnIndex].StyleID = result.Value;
                }
            }
        }

        private void RenderTotal(DataTable dataTable, int lastColumnIndex)
        {
            var totalRowIndex = BeginContentRowIndex + dataTable.Rows.Count;
            templateSheet.Cells[$"A7:B7"].Copy(sheetTrader.Cells[$"A{totalRowIndex}:B{totalRowIndex}"]);
            sheetTrader.Cells[$"A{totalRowIndex}:B{totalRowIndex}"].Merge = true;

            for (var columnIndex = 2; columnIndex < lastColumnIndex; columnIndex++)
            {
                sheetTrader.Cells[totalRowIndex, columnIndex + 1].Value = dataTable.AsEnumerable().Sum(s => Converter.To<decimal>(s[columnIndex]));
            }

            sheetTrader.Cells[$"C{totalRowIndex}:H{totalRowIndex}"].StyleID = templateSheet.Cells["H7"].StyleID;
            sheetTrader.Cells[totalRowIndex, 9, totalRowIndex, lastColumnIndex].StyleID = templateSheet.Cells["I7"].StyleID;
        }

        private void ClearSheet()
        {
            excelPackage.Workbook.Worksheets.Delete(templateSheet);
        }

        private void Save(GetSummaryPHCriteria criteria)
        {
            var fileName = $@"TraderPerformance_Soccer_PH_From_Date_{criteria.Fromdate:MM_dd_yyyy}_To_Date_{criteria.Todate:MM_dd_yyyy}";

            var response = HttpContext.Current.Response;
            response.Clear();
            response.ContentType = "application/vnd.ms-excel";
            response.AddHeader("Content-Disposition", string.Format("attachment; filename={0}.xlsx", fileName));
            response.BinaryWrite(excelPackage.GetAsByteArray());
        }
    }
}