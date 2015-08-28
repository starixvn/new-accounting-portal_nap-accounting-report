// -----------------------------------------------------------------------
// <copyright file="CustomColor.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------
// <history>
//   <change who="Phillip.Truong" date="2014.04.14">Created</change>
// </history>

namespace Fanex.AccountingReports.Core.Utilities.CellStyles
{
    using System.Drawing;
    using Fanex.AccountingReports.Core.Extensions;
    using NPOI.HSSF.UserModel;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class CustomColor
    {
        public readonly short ReportTitle = 57;

        public readonly short Header = 58;

        public readonly short BodyOdd = 59;

        public readonly short BodyEven = 60;

        public readonly short Total = 61;

        public readonly short SubTotal = 62;

        public CustomColor(HSSFWorkbook workbook)
        {
            var palette = workbook.GetCustomPalette();
            SetColorAtIndex(palette, this.ReportTitle, NpoiExcelExtensions.ReportTitleColor);
            SetColorAtIndex(palette, this.Header, NpoiExcelExtensions.HeaderTitleColor);
            SetColorAtIndex(palette, this.BodyOdd, NpoiExcelExtensions.OddRowColor);
            SetColorAtIndex(palette, this.BodyEven, NpoiExcelExtensions.EvenRowColor);
            SetColorAtIndex(palette, this.Total, NpoiExcelExtensions.TotalRowColor);
            SetColorAtIndex(palette, this.SubTotal, NpoiExcelExtensions.SubTotalRowColor);
        }

        private void SetColorAtIndex(HSSFPalette palette, short index, Color color)
        {
            palette.SetColorAtIndex(index, color.R, color.G, color.B);
        }
    }
}
