namespace Fanex.AccountingReports.Core.Utilities.CellStyles
{
    using Fanex.AccountingReports.Core.Extensions;
    using NPOI.HSSF.UserModel;
    using NPOI.SS.UserModel;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class BaseSubTotalCellStyles
    {
        public BaseSubTotalCellStyles(HSSFWorkbook workbook, CustomColor customColor)
        {
            this.CustomColors = customColor;
            this.Text = NpoiExcelExtensions.TextStyle(workbook, this.CustomColors.SubTotal, true, NpoiExcelExtensions.Alignment.Center);
            this.Decimal = NpoiExcelExtensions.DecimalStyle(workbook, this.CustomColors.SubTotal, true);
            this.Integer = NpoiExcelExtensions.IntStyle(workbook, this.CustomColors.SubTotal, true);
        }

        public ICellStyle Text { get; set; }

        public ICellStyle Integer { get; set; }

        public ICellStyle Decimal { get; set; }

        public CustomColor CustomColors { get; set; }
    }
}
