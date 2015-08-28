// -----------------------------------------------------------------------
// <copyright file="KenoWinLossSubTotalCellStyles.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------
// <history>
//   <change who="Phillip.Truong" date="2014.04.14">Created</change>
// </history>

namespace Fanex.AccountingReports.Licensee.Utilities.KenoWinLoss.CellStyles
{
    using Fanex.AccountingReports.Core.Extensions;
    using Fanex.AccountingReports.Licensee.Utilities.KenoWinLoss.Color;
    using NPOI.HSSF.UserModel;
    using NPOI.SS.UserModel;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class KenoWinLossSubTotalCellStyles
    {
        public KenoWinLossSubTotalCellStyles(HSSFWorkbook workbook, CustomColor customColor)
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
