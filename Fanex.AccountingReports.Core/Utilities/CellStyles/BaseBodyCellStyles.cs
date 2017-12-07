namespace Fanex.AccountingReports.Core.Utilities.CellStyles
{
    using Fanex.AccountingReports.Core.Extensions;
    using NPOI.HSSF.UserModel;
    using NPOI.SS.UserModel;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class BaseBodyCellStyles
    {
        public BaseBodyCellStyles(HSSFWorkbook workbook, CustomColor customColor)
        {
            this.CustomColors = customColor;
            this.OddText = NpoiExcelExtensions.TextStyle(workbook, this.CustomColors.BodyOdd, false, NpoiExcelExtensions.Alignment.Center);
            this.EvenText = NpoiExcelExtensions.TextStyle(workbook, this.CustomColors.BodyEven, false, NpoiExcelExtensions.Alignment.Center);
            this.OddDecimal = NpoiExcelExtensions.DecimalStyle(workbook, this.CustomColors.BodyOdd, false);
            this.EvenDecimal = NpoiExcelExtensions.DecimalStyle(workbook, this.CustomColors.BodyEven, false);
            this.OddInteger = NpoiExcelExtensions.IntStyle(workbook, this.CustomColors.BodyOdd, false);
            this.EvenInteger = NpoiExcelExtensions.IntStyle(workbook, this.CustomColors.BodyEven, false);
        }

        public ICellStyle OddText { get; set; }

        public ICellStyle EvenText { get; set; }

        public ICellStyle OddDecimal { get; set; }

        public ICellStyle EvenDecimal { get; set; }

        public ICellStyle OddInteger { get; set; }

        public ICellStyle EvenInteger { get; set; }

        public CustomColor CustomColors { get; set; }
    }
}
