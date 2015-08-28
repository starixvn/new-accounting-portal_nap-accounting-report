// --------------------------------------------------------------------------
// <copyright file="ReportCellStyle.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Louis.Nguyen" date="2012.12.07">Created</change>
// </history>

namespace Fanex.AccountingReports.Main.Services.Implement.MatchWinLoss2a
{
    using Fanex.AccountingReports.Core.Constants;
    using Fanex.AccountingReports.Core.Extensions;
    using NPOI.HSSF.UserModel;
    using NPOI.HSSF.Util;
    using NPOI.SS.UserModel;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public struct ReportCellStyle
    {
        public ICellStyle TextStyle;
        public ICellStyle RightTextStyle;
        public ICellStyle CenterTextStyle;
        public ICellStyle BoldTextStyle;
        public ICellStyle DecimalNumberStyle;
        public ICellStyle BoldDecimalNumberStyle;

        public ReportCellStyle(ref HSSFWorkbook excelFile)
        {
            IDataFormat dataFormat = excelFile.CreateDataFormat();
            TextStyle = NpoiExcelExtensions.TextStyle(excelFile, HSSFColor.White.Index, false, NpoiExcelExtensions.Alignment.Left);
            RightTextStyle = NpoiExcelExtensions.TextStyle(excelFile, HSSFColor.White.Index, false, NpoiExcelExtensions.Alignment.Right);
            CenterTextStyle = NpoiExcelExtensions.TextStyle(excelFile, HSSFColor.White.Index, false, NpoiExcelExtensions.Alignment.Center);
            BoldTextStyle = NpoiExcelExtensions.TextStyle(excelFile, HSSFColor.White.Index, true, NpoiExcelExtensions.Alignment.Right);
            DecimalNumberStyle = NpoiExcelExtensions.DecimalStyle(excelFile, HSSFColor.White.Index, false);
            DecimalNumberStyle.DataFormat = dataFormat.GetFormat(NumberFormatConstants.NegativeNumberFormat);
            BoldDecimalNumberStyle = NpoiExcelExtensions.DecimalStyle(excelFile, HSSFColor.White.Index, true);
            BoldDecimalNumberStyle.DataFormat = dataFormat.GetFormat(NumberFormatConstants.NegativeNumberFormat);
        }
    }
}
