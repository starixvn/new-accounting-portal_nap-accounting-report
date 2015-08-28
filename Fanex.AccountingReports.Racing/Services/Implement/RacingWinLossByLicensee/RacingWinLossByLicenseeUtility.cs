// -----------------------------------------------------------------------
// <copyright file="RacingWinLossByLicenseeUtility.cs" company="Nexcel Solutions Viet Nam">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace Fanex.AccountingReports.Racing.Services.Implement.RacingWinLossByLicensee
{
    using Fanex.AccountingReports.Core.Extensions;
    using NPOI.HSSF.UserModel;
    using NPOI.HSSF.Util;
    using NPOI.SS.UserModel;

    /// <summary>
    /// The utilities method use to export excel of Racing Win Loss By Licensee.
    /// </summary>
    public static class RacingWinLossByLicenseeUtility
    {
        public const string HeaderRowClass = "header";

        public const string NormalRowClass = "normal-row";

        public const string LeagueTotalRowClass = "league-total-row";

        public const string CurrencyTotalRowClass = "currency-total-row";

        public const string GrandTotalRowClass = "grand-total-row";

        public const string BlankRowClass = "blank-row";

        public const string RacingWinLossByLicenseePrefixSessionKey =
                "Fanex.AccountingReports.Racing.RacingWinLossByLicensee.";

        /// <summary>
        /// Initial the style of cell.
        /// </summary>
        /// <param name="excelFile">The excel file.</param>
        /// <param name="styleCell">The style cell.</param>
        /// <param name="rowClass">The class of row.</param>
        public static void InitCellStyle(
            HSSFWorkbook excelFile,
            ref CustomStyleCell styleCell,
            string rowClass)
        {
            HSSFPalette palette = excelFile.GetCustomPalette();
            short cellColor;

            switch (rowClass)
            {
                case HeaderRowClass:
                    styleCell.TextStyle = NpoiExcelExtensions.HeaderStyle(excelFile);
                    break;

                case BlankRowClass:
                     palette.SetColorAtIndex(HSSFColor.Plum.Index, (byte)223, (byte)234, (byte)255);
                     cellColor = HSSFColor.Plum.Index;
                     styleCell.TextStyle = NpoiExcelExtensions.TextStyle(excelFile, cellColor, true, NpoiExcelExtensions.Alignment.Center);
                     styleCell.DecimalStyle = NpoiExcelExtensions.DecimalStyle(excelFile, cellColor, false);
                     break;

                case NormalRowClass:
                    cellColor = HSSFColor.White.Index;
                    styleCell.TextStyle = NpoiExcelExtensions.TextStyle(excelFile, cellColor, true, NpoiExcelExtensions.Alignment.Center);
                    styleCell.DecimalStyle = NpoiExcelExtensions.DecimalStyle(excelFile, cellColor, false);
                    break;

                case LeagueTotalRowClass:
                    palette.SetColorAtIndex(HSSFColor.Tan.Index, (byte)255, (byte)238, (byte)221);
                    cellColor = HSSFColor.Tan.Index;
                    styleCell.TextStyle = NpoiExcelExtensions.TextStyle(excelFile, cellColor, true, NpoiExcelExtensions.Alignment.Center);
                    styleCell.DecimalStyle = NpoiExcelExtensions.DecimalStyle(excelFile, cellColor, true);
                    break;

                case CurrencyTotalRowClass:
                    palette.SetColorAtIndex(HSSFColor.Turquoise.Index, (byte)211, (byte)225, (byte)200);
                    cellColor = HSSFColor.Turquoise.Index;
                    styleCell.TextStyle = NpoiExcelExtensions.TextStyle(excelFile, cellColor, true, NpoiExcelExtensions.Alignment.Center);
                    styleCell.DecimalStyle = NpoiExcelExtensions.DecimalStyle(excelFile, cellColor, true);
                    break;

                case GrandTotalRowClass:
                    palette.SetColorAtIndex(HSSFColor.Teal.Index, (byte)219, (byte)225, (byte)187);
                    cellColor = HSSFColor.Teal.Index;
                    styleCell.TextStyle = NpoiExcelExtensions.TextStyle(excelFile, cellColor, true, NpoiExcelExtensions.Alignment.Center);
                    styleCell.DecimalStyle = NpoiExcelExtensions.DecimalStyle(excelFile, cellColor, true);
                    palette.SetColorAtIndex(HSSFColor.Lime.Index, (byte)255, (byte)102, (byte)0);
                    styleCell.TextStyle.BorderTop = BorderStyle.Medium;
                    styleCell.TextStyle.TopBorderColor = HSSFColor.Lime.Index;
                    styleCell.DecimalStyle.BorderTop = BorderStyle.Medium;
                    styleCell.DecimalStyle.TopBorderColor = HSSFColor.Lime.Index;
                    break;
            }
        }

        public struct CustomStyleCell
        {
            public ICellStyle TextStyle;

            public ICellStyle DecimalStyle;

            public CustomStyleCell(HSSFWorkbook excelFile)
            {
                TextStyle = excelFile.CreateCellStyle();
                DecimalStyle = excelFile.CreateCellStyle();
            }
        }
    }
}
