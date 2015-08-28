// -----------------------------------------------------------------------
// <copyright file="BaseCellStyles.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------
// <history>
//   <change who="Phillip.Truong" date="2014.05.14">Created</change>
// </history>

namespace Fanex.AccountingReports.Core.Utilities.CellStyles
{
    using Fanex.AccountingReports.Core.Extensions;
    using NPOI.HSSF.UserModel;
    using NPOI.HSSF.Util;
    using NPOI.SS.UserModel;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class BaseCellStyles
    {
        public BaseCellStyles(HSSFWorkbook workbook, CustomColor customColor)
        {
            this.CustomColors = customColor;
            this.ReportTitle = NpoiExcelExtensions.TextStyle(workbook, CustomColors.ReportTitle, true, NpoiExcelExtensions.Alignment.Left);
            this.ReportTitle.BorderBottom = BorderStyle.None;
            this.ReportTitle.BorderTop = BorderStyle.None;
            this.ReportTitle.BorderLeft = BorderStyle.None;
            this.ReportTitle.BorderRight = BorderStyle.None;
            this.ReportBodyHeader = NpoiExcelExtensions.TextStyle(workbook, CustomColors.Header, true, NpoiExcelExtensions.Alignment.Center, HSSFColor.White.Index);
            this.ReportBodyData = new BaseBodyCellStyles(workbook, this.CustomColors);
            this.ReportBodyFooter = new BaseFooterCellStyles(workbook, this.CustomColors);
            this.ReportBodySubTotal = new BaseSubTotalCellStyles(workbook, this.CustomColors);
        }

        public ICellStyle ReportTitle { get; set; }

        public ICellStyle ReportBodyHeader { get; set; }

        public BaseBodyCellStyles ReportBodyData { get; set; }

        public BaseSubTotalCellStyles ReportBodySubTotal { get; set; }

        public BaseFooterCellStyles ReportBodyFooter { get; set; }

        public CustomColor CustomColors { get; set; }
    }
}
