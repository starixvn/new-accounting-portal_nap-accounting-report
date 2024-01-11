namespace Fanex.AccountingReports.Core.Utilities.CellStyles
{
    using Fanex.AccountingReports.Core.Extensions;
    using NPOI.HSSF.UserModel;
    using NPOI.HSSF.Util;
    using NPOI.SS.UserModel;

    public class BaseFooterCellStyles
    {
        public BaseFooterCellStyles(HSSFWorkbook workbook, CustomColor customColor)
        {
            this.CustomColors = customColor;
            this.TextUpper = NpoiExcelExtensions.TextStyle(workbook, this.CustomColors.Total, true, NpoiExcelExtensions.Alignment.Center);
            this.TextUpper.BorderTop = BorderStyle.Medium;
            this.TextUpper.TopBorderColor = HSSFColor.Orange.Index;
            this.DecimalUpper = NpoiExcelExtensions.DecimalStyle(workbook, this.CustomColors.Total, true);
            this.DecimalUpper.BorderTop = BorderStyle.Medium;
            this.DecimalUpper.TopBorderColor = HSSFColor.Orange.Index;
            this.IntegerUpper = NpoiExcelExtensions.IntStyle(workbook, this.CustomColors.Total, true);
            this.IntegerUpper.BorderTop = BorderStyle.Medium;
            this.IntegerUpper.TopBorderColor = HSSFColor.Orange.Index;

            this.Text = NpoiExcelExtensions.TextStyle(workbook, this.CustomColors.Total, true, NpoiExcelExtensions.Alignment.Center);
            this.Decimal = NpoiExcelExtensions.DecimalStyle(workbook, this.CustomColors.Total, true);
            this.Integer = NpoiExcelExtensions.IntStyle(workbook, this.CustomColors.Total, true);
        }

        public ICellStyle TextUpper { get; set; }

        public ICellStyle DecimalUpper { get; set; }

        public ICellStyle IntegerUpper { get; set; }
        
        public ICellStyle Text { get; set; }

        public ICellStyle Decimal { get; set; }

        public ICellStyle Integer { get; set; }

        public CustomColor CustomColors { get; set; }
    }
}
