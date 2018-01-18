namespace Fanex.AccountingReports.Main.Services.Implement.DailyWinLoss2hSoccerMinorBettype
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Diagnostics.CodeAnalysis;
    using Fanex.AccountingReports.Main.Enums.DailyWinloss2hSoccerMinorBettype;
    using Fanex.AccountingReports.Main.Resources;
    using Fanex.Common;
    using Fanex.Excel.Enums;
    using Fanex.Excel.Models;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class BaseReportService
    {
        protected IDataReader Reader { get; set; }

        protected GridOptions GridOptions { get; set; }

        /// <summary>
        /// Groups the header rows.
        /// </summary>
        /// <returns>Groups header rows.</returns>
        protected virtual List<GroupHeaderRows> GroupHeaderRows()
        {
            var groupHeaderColumns = new List<GroupHeaderRows>();
            var groupHeaderRow = new GroupHeaderRows();
            groupHeaderRow.Columns.Add(new HeaderColumns(true) { HeaderText = Labels.Sport_Type, Name = Labels.SportType, MergeRowNumber = 2 });
            groupHeaderRow.Columns.Add(new HeaderColumns(true) { HeaderText = Labels.Member_Win_Loss, Name = Labels.WinLoss, MergeRowNumber = 2 });
            groupHeaderRow.Columns.Add(new HeaderColumns(true) { HeaderText = Labels.SMA_Win_Loss, Name = Labels.Position, MergeRowNumber = 2 });
            groupHeaderRow.Columns.Add(new HeaderColumns(true) { HeaderText = Labels.Commission, Name = Labels.Commission, MergeRowNumber = 2 });
            groupHeaderRow.Columns.Add(new HeaderColumns(true) { HeaderText = Labels.Company_Income, Name = Labels.LineTotal + Labels.Real, MergeColNumber = 2 });
            groupHeaderRow.Columns.Add(new HeaderColumns(true) { HeaderText = Labels.Variance, Name = Labels.Variance, MergeRowNumber = 2 });
            groupHeaderRow.Columns.Add(new HeaderColumns(true) { HeaderText = Labels.Company_Turnover, Name = Labels.Turnover + Labels.Real, MergeColNumber = 2 });
            groupHeaderRow.Columns.Add(new HeaderColumns(true) { HeaderText = Labels.Margin, Name = Labels.Margin + Labels.Real, MergeColNumber = 2 });
            groupHeaderColumns.Add(groupHeaderRow);
            return groupHeaderColumns;
        }

        /// <summary>
        /// Headers the row.
        /// </summary>
        /// <returns>Header row.</returns>
        [SuppressMessage("StyleCop.CSharp.StylecopPlus.Rules",
           "SP2100:CodeLineMustNotBeLongerThan",
           Justification = "Reviewed. Suppression is OK here.")]
        protected virtual List<HeaderColumns> HeaderRow()
        {
            var headerRow = new List<HeaderColumns>();
            headerRow.Add(new HeaderColumns() { HeaderText = Labels.Sport_Type, Name = Labels.SportType, IsBold = true, Width = 180 });
            headerRow.Add(new HeaderColumns() { HeaderText = Labels.Member_Win_Loss, Name = Labels.WinLoss, CustomCellType = ExcelCellType.DoubleRedPositive, CellAlignment = HAlignment.Right, Width = 100 });
            headerRow.Add(new HeaderColumns() { HeaderText = Labels.SMA_Win_Loss, Name = Labels.Position, CustomCellType = ExcelCellType.Double, CellAlignment = HAlignment.Right, Width = 100 });
            headerRow.Add(new HeaderColumns() { HeaderText = Labels.Commission, Name = Labels.Commission, CustomCellType = ExcelCellType.Double, CellAlignment = HAlignment.Right, Width = 100 });
            headerRow.Add(new HeaderColumns() { HeaderText = Labels.Real, Name = Labels.LineTotal + Labels.Real, CustomCellType = ExcelCellType.DoubleRedPositive, CellAlignment = HAlignment.Right, Width = 100 });
            headerRow.Add(new HeaderColumns() { HeaderText = Labels.Virtual, Name = Labels.LineTotal + Labels.Virtual, CustomCellType = ExcelCellType.DoubleRedPositive, CellAlignment = HAlignment.Right, Width = 100 });
            headerRow.Add(new HeaderColumns() { HeaderText = Labels.Variance, Name = Labels.Variance, CustomCellType = ExcelCellType.DoubleRedPositive, CellAlignment = HAlignment.Right, Width = 100 });
            headerRow.Add(new HeaderColumns() { HeaderText = Labels.Real, Name = Labels.Turnover + Labels.Real, CustomCellType = ExcelCellType.Double, CellAlignment = HAlignment.Right, Width = 100 });
            headerRow.Add(new HeaderColumns() { HeaderText = Labels.Virtual, Name = Labels.Turnover + Labels.Virtual, CustomCellType = ExcelCellType.Double, CellAlignment = HAlignment.Right, Width = 100 });
            headerRow.Add(new HeaderColumns() { HeaderText = Labels.Real, Name = Labels.Margin + Labels.Real, CustomCellType = ExcelCellType.PercentageRedPositive, CellAlignment = HAlignment.Right, Width = 100 });
            headerRow.Add(new HeaderColumns() { HeaderText = Labels.Virtual, Name = Labels.Margin + Labels.Virtual, CustomCellType = ExcelCellType.PercentageRedPositive, CellAlignment = HAlignment.Right, Width = 100 });
            return headerRow;
        }

        /// <summary>
        /// Recalculates the variance and margin value.
        /// </summary>
        /// <param name="row">The data row.</param>
        protected virtual void RecalculateVarianceAndMarginValue(GridDataRow row)
        {
            double realLineTotal = Math.Round(Convert.ToDouble(row.Columns[(int)ColumnMappingEnums.LineTotalReal].Value), 2);
            double virtualLineTotal = Math.Round(Convert.ToDouble(row.Columns[(int)ColumnMappingEnums.LineTotalVirtual].Value), 2);
            double realTurnOver = Math.Round(Convert.ToDouble(row.Columns[(int)ColumnMappingEnums.TurnoverReal].Value), 2);
            double virtualTurnOver = Math.Round(Convert.ToDouble(row.Columns[(int)ColumnMappingEnums.TurnoverVirtual].Value), 2);
            if (Math.Abs(realTurnOver) < 0)
            {
                row.Columns[(int)ColumnMappingEnums.MarginReal].Value = Labels.NotAvailable;
            }
            else
            {
                row.Columns[(int)ColumnMappingEnums.MarginReal].Value = Converter.ToDecimal(Math.Abs(realTurnOver) < 0 ? 0 : realLineTotal / realTurnOver);
            }

            if (Math.Abs(virtualTurnOver) < 0)
            {
                row.Columns[(int)ColumnMappingEnums.MarginVirtual].Value = Labels.NotAvailable;
            }
            else
            {
                row.Columns[(int)ColumnMappingEnums.MarginVirtual].Value = Converter.ToDecimal(Math.Abs(virtualTurnOver) < 0 ? 0 : virtualLineTotal / virtualTurnOver);
            }

            row.Columns[(int)ColumnMappingEnums.Variance].Value = Converter.ToDecimal(realLineTotal - virtualLineTotal);
        }
    }
}