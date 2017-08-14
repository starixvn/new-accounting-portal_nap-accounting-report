using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Web;
using Fanex.AccountingReports.Core.Services.Impl;
using Fanex.AccountingReports.Main.Criteria.TraderPerformance;
using Fanex.AccountingReports.Main.Repositories.TraderPerformanceSoccerPH;
using Fanex.Common;
using OfficeOpenXml;

namespace Fanex.AccountingReports.Main.Services.TraderPerformanceSoccerPH
{
    public interface ExcelTraderDetailPHService
    {
        void Execute(GetDetailPHCriteria criteria);
    }

    public class ExcelTraderDetailPHServiceImp : ExcelTraderDetailPHService
    {
        private const int BeginContentRowIndex = 6;
        private const int BeginColumnMainGroupIndex = 14;

        private readonly TraderDataService traderDataService;
        private readonly ExcelPackage excelPackage;

        private ExcelWorksheet templateSheet;
        private ExcelWorksheet sheetTrader;

        private int currentContentRowIndex;

        public ExcelTraderDetailPHServiceImp(TraderDataService traderDataService, ExcelPackage excelPackage)
        {
            this.traderDataService = traderDataService;
            this.excelPackage = excelPackage;
        }

        public void Execute(GetDetailPHCriteria criteria)
        {
            var dataTable = traderDataService.Query(criteria);
            var columnHeaders = new Collection<string>();
            int lastColumnIndex = dataTable.Columns.Count;

            sheetTrader = excelPackage.Workbook.Worksheets.Add("Soccer PH Detail");
            templateSheet = excelPackage.Workbook.Worksheets["Content"];

            RenderTitle(lastColumnIndex, criteria);
            RenderHeader(lastColumnIndex, dataTable, columnHeaders);
            MergeHeader(columnHeaders);
            SetHeaderStyle(lastColumnIndex);
            RenderContent(dataTable, lastColumnIndex);
            SetWidth(lastColumnIndex);
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
                .Select(s => Converter.To<string>(s["username"]))
                .Distinct()
                .ToList()
                .ForEach(user =>
                {
                    var rowsByUser = dataTable.AsEnumerable().Where(s => Converter.To<string>(s["username"]) == user).ToList();
                    for (var rowIndex = 0; rowIndex < rowsByUser.Count; rowIndex++)
                    {
                        var row = rowsByUser[rowIndex];
                        RenderContentByUser(lastColumnIndex, userIndex, row);
                        currentContentRowIndex++;
                    }

                    MergeAndSetSyleContentByUser(lastColumnIndex, user, dataTable);
                    CalculateSubTotalByUser(lastColumnIndex, rowsByUser);
                    currentContentRowIndex++;
                    userIndex++;
                });

            CalculateGrandTotal(lastColumnIndex, dataTable);
        }

        private void RenderContentByUser(int lastColumnIndex, int userIndex, DataRow row)
        {
            for (var columnIndex = 1; columnIndex <= lastColumnIndex; columnIndex++)
            {
                if (columnIndex == 1)
                {
                    sheetTrader.Cells[currentContentRowIndex, columnIndex].Value = userIndex;
                }
                else if (columnIndex == 4)
                {
                    sheetTrader.Cells[currentContentRowIndex, columnIndex].Value = CacheRef.GetLeagueName(row[columnIndex - 1]);
                }
                else if (columnIndex == 7 || columnIndex == 8)
                {
                    sheetTrader.Cells[currentContentRowIndex, columnIndex].Value = CacheRef.GetTeamName(row[columnIndex - 1]);
                }
                else
                {
                    sheetTrader.Cells[currentContentRowIndex, columnIndex].Value = row[columnIndex - 1];
                }
            }
        }

        private void MergeAndSetSyleContentByUser(int lastColumnIndex, string username, DataTable dataTable)
        {
            var rowsByUser = dataTable.AsEnumerable().Where(s => s["username"].ToString() == username).ToList();
            var beginContentRowIndex = currentContentRowIndex - rowsByUser.Count;
            var endContentRowIndex = currentContentRowIndex - 1;

            sheetTrader.Cells[$"A{beginContentRowIndex}:A{endContentRowIndex}"].Merge = true;
            sheetTrader.Cells[$"B{beginContentRowIndex}:B{endContentRowIndex}"].Merge = true;

            sheetTrader.Cells[$"A{beginContentRowIndex}:A{endContentRowIndex}"].StyleID = templateSheet.Cells["A6"].StyleID;
            sheetTrader.Cells[$"B{beginContentRowIndex}:B{endContentRowIndex}"].StyleID = templateSheet.Cells["B6"].StyleID;
            sheetTrader.Cells[$"C{beginContentRowIndex}:C{endContentRowIndex}"].StyleID = templateSheet.Cells["C6"].StyleID;
            sheetTrader.Cells[$"D{beginContentRowIndex}:M{endContentRowIndex}"].StyleID = templateSheet.Cells["D6"].StyleID;
            sheetTrader.Cells[$"F{beginContentRowIndex}:F{endContentRowIndex}"].StyleID = templateSheet.Cells["F6"].StyleID;

            sheetTrader.Cells[currentContentRowIndex, 1, currentContentRowIndex, BeginColumnMainGroupIndex - 1].Value = username;
            sheetTrader.Cells[currentContentRowIndex, 1, currentContentRowIndex, BeginColumnMainGroupIndex - 1].Merge = true;
            sheetTrader.Cells[currentContentRowIndex, 1, currentContentRowIndex, BeginColumnMainGroupIndex - 1].StyleID = templateSheet.Cells["A10"].StyleID;
            sheetTrader.Cells[currentContentRowIndex, BeginColumnMainGroupIndex, currentContentRowIndex, lastColumnIndex].StyleID = templateSheet.Cells["I10"].StyleID;

            var styles = new Dictionary<string, int>
            {
                { "winlost_",  templateSheet.Cells["I6"].StyleID },
                { "stake_",  templateSheet.Cells["J6"].StyleID },
                { "bet_",  templateSheet.Cells["K6"].StyleID },
                { "comm_",  templateSheet.Cells["M6"].StyleID },
                { "margin_",  templateSheet.Cells["L6"].StyleID },
                { "others",  templateSheet.Cells["N6"].StyleID }
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

        private void CalculateSubTotalByUser(int lastColumnIndex, IEnumerable<DataRow> rowsByUser)
        {
            for (var columnIndex = BeginColumnMainGroupIndex; columnIndex <= lastColumnIndex; columnIndex++)
            {
                sheetTrader.Cells[currentContentRowIndex, columnIndex].Value = rowsByUser.Sum(s => Converter.To<decimal>(s[columnIndex - 1]));
            }
        }

        private void CalculateGrandTotal(int lastColumnIndex, DataTable dataTable)
        {
            for (var columnIndex = BeginColumnMainGroupIndex; columnIndex <= lastColumnIndex; columnIndex++)
            {
                sheetTrader.Cells[currentContentRowIndex, columnIndex].Value = dataTable.AsEnumerable().Sum(s => Converter.To<decimal>(s[columnIndex - 1]));
            }

            sheetTrader.Cells[currentContentRowIndex, 1, currentContentRowIndex, BeginColumnMainGroupIndex - 1].Value = "Grand Total";
            sheetTrader.Cells[currentContentRowIndex, 1, currentContentRowIndex, BeginColumnMainGroupIndex - 1].Merge = true;
            sheetTrader.Cells[currentContentRowIndex, 1, currentContentRowIndex, BeginColumnMainGroupIndex - 1].StyleID = templateSheet.Cells["A11"].StyleID;
            sheetTrader.Cells[currentContentRowIndex, BeginColumnMainGroupIndex, currentContentRowIndex, lastColumnIndex].StyleID = templateSheet.Cells["I11"].StyleID;
        }

        private void SetWidth(int lastColumnIndex)
        {
            for (var columnIndex = BeginColumnMainGroupIndex; columnIndex <= lastColumnIndex; columnIndex++)
            {
                sheetTrader.Column(columnIndex).Width = 15;
            }

            var columns = new Dictionary<int, int>
            {
                { 1, 5 },
                { 2, 15 },
                { 3, 10 },
                { 4, 30 },
                { 7, 20 },
                { 8, 20 }
            };

            foreach (var column in columns)
            {
                sheetTrader.Column(column.Key).Width = column.Value;
            }
        }

        private void ClearSheet()
        {
            excelPackage.Workbook.Worksheets.Delete(templateSheet);
        }

        private void Save(GetDetailPHCriteria criteria)
        {
            var fileName = $@"TraderPerformance_Soccer_Detail_PH_From_Date_{criteria.Fromdate:MM_dd_yyyy}_To_Date_{criteria.Todate:MM_dd_yyyy}";

            var response = HttpContext.Current.Response;
            response.Clear();
            response.ContentType = "application/vnd.ms-excel";
            response.AddHeader("Content-Disposition", string.Format("attachment; filename={0}.xlsx", fileName));
            response.BinaryWrite(excelPackage.GetAsByteArray());
        }
    }
}