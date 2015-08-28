// -----------------------------------------------------------------------
// <copyright file="KenoWinLossCellStyles.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------
// <history>
//   <change who="Phillip.Truong" date="2014.05.14">Created</change>
// </history>

namespace Fanex.AccountingReports.Licensee.Utilities.KenoWinLoss.CellStyles
{
    using Fanex.AccountingReports.Core.Extensions;
    using Fanex.AccountingReports.Licensee.Utilities.KenoWinLoss.Color;
    using NPOI.HSSF.UserModel;
    using NPOI.HSSF.Util;
    using NPOI.SS.UserModel;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class KenoWinLossCellStyles
    {
        public KenoWinLossCellStyles(HSSFWorkbook workbook, CustomColor customColor)
        {
            this.CustomColors = customColor;
            this.ReportTitle = NpoiExcelExtensions.TextStyle(workbook, CustomColors.ReportTitle, true, NpoiExcelExtensions.Alignment.Left);
            this.ReportTitle.BorderBottom = BorderStyle.None;
            this.ReportTitle.BorderTop = BorderStyle.None;
            this.ReportTitle.BorderLeft = BorderStyle.None;
            this.ReportTitle.BorderRight = BorderStyle.None;
            this.ReportBodyHeader = NpoiExcelExtensions.TextStyle(workbook, CustomColors.Header, true, NpoiExcelExtensions.Alignment.Center, HSSFColor.White.Index);
            this.ReportBodyData = new KenoWinLossBodyCellStyles(workbook, this.CustomColors);
            this.ReportBodyFooter = new KenoWinLossFooterCellStyles(workbook, this.CustomColors);
            this.ReportBodySubTotal = new KenoWinLossSubTotalCellStyles(workbook, this.CustomColors);
        }

        public ICellStyle ReportTitle { get; set; }

        public ICellStyle ReportBodyHeader { get; set; }

        public KenoWinLossBodyCellStyles ReportBodyData { get; set; }

        public KenoWinLossSubTotalCellStyles ReportBodySubTotal { get; set; }

        public KenoWinLossFooterCellStyles ReportBodyFooter { get; set; }

        public CustomColor CustomColors { get; set; }
    }
}
