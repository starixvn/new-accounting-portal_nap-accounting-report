// -----------------------------------------------------------------------
// <copyright file="ExportDetailReportBySportsTypeService.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------
// <history>
//   <change who="Franklin.Nguyen" date="2014.03.05">Created</change>
// </history>

namespace Fanex.AccountingReports.Main.Services.Implement.MatchWinLoss2a
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using Fanex.AccountingReports.Core.Constants;
    using Fanex.AccountingReports.Core.Extensions;
    using Fanex.AccountingReports.Core.Services.Impl;
    using Fanex.AccountingReports.Core.Web;
    using Fanex.AccountingReports.Main.Criteria.MatchWinLoss2a;
    using Fanex.AccountingReports.Main.Models.MatchWinLoss2a;
    using Fanex.AccountingReports.Main.Resources;
    using Fanex.AccountingReports.Main.Services.Interface;
    using NPOI.HSSF.UserModel;
    using NPOI.SS.UserModel;

    /// <summary>
    /// Export detail report by sports type.
    /// </summary>
    public partial class MatchWinLoss2aService : IMatchWinLoss2aService
    {
        /// <summary>
        /// Exports the type of the detail report by sports.
        /// </summary>
        /// <param name="criteria">The criteria.</param>
        [SuppressMessage("StyleCop.CSharp.StylecopPlus.Rules",
            "SP2101:MethodMustNotContainMoreLinesThan",
            Justification = "Reviewed. Suppression is OK here.")]
        public void ExportDetailReportBySportsType(GetDetailReportByBetTypeCriteria criteria)
        {
            try
            {
                var report = SessionManager.Get<WinLossInfo>(_matchWinLoss2aDetailReportBySportsTypeSessionKey);
                if (report == null)
                {
                    bool isBeforeBalanceUpDay = criteria.Winlostdate < ReportService.BalanceUp.BalanceUpDay ?
                    true :
                    false;

                    if (isBeforeBalanceUpDay)
                    {
                        report = this.GetBetTypeDetailReportBySportsBeforeBalanceUpDay(criteria);
                    }
                    else
                    {
                        report = this.GetBetTypeDetailReportBySports(criteria);
                    }
                }

                HSSFWorkbook excelFile = new HSSFWorkbook();
                string sheetName = MainResources.MatchWinLoss2a_SheetName;
                ISheet worksheet = excelFile.CreateSheet(sheetName);
                var currentRow = 0;
                var betTypeGroupLength = report.ChildsInfo.Count;

                this.ExportDetailReportByBetTypeHeader(
                    excelFile,
                    ref worksheet,
                    ref currentRow,
                    criteria);

                currentRow++;

                if (report != null && betTypeGroupLength > 0)
                {
                    this.SetColorIndex(excelFile);
                    IDataFormat dataFormat = excelFile.CreateDataFormat();
                    ReportCellStyle reportCellStyle = new ReportCellStyle(ref excelFile);
                    ICellStyle subTotalTextStyle = NpoiExcelExtensions.TextStyle(excelFile, SubTotalColorIndex, true, NpoiExcelExtensions.Alignment.Right);
                    ICellStyle subTotalDecimalNumberStyle = NpoiExcelExtensions.DecimalStyle(excelFile, SubTotalColorIndex, true);
                    subTotalDecimalNumberStyle.DataFormat = dataFormat.GetFormat(NumberFormatConstants.NegativeNumberFormat);
                    ICellStyle grandTotalTextStyle = NpoiExcelExtensions.TextStyle(excelFile, GrandTotalColorIndex, true, NpoiExcelExtensions.Alignment.Right);
                    grandTotalTextStyle.BorderTop = BorderStyle.Medium;
                    grandTotalTextStyle.TopBorderColor = GrandTotalBorderColorIndex;
                    ICellStyle grandTotalDecimalNumberStyle = NpoiExcelExtensions.DecimalStyle(excelFile, GrandTotalColorIndex, true);
                    grandTotalDecimalNumberStyle.BorderTop = BorderStyle.Medium;
                    grandTotalDecimalNumberStyle.TopBorderColor = GrandTotalBorderColorIndex;
                    grandTotalDecimalNumberStyle.DataFormat = dataFormat.GetFormat(NumberFormatConstants.NegativeNumberFormat);

                    for (var betTypeGroupIndex = 0; betTypeGroupIndex < betTypeGroupLength; betTypeGroupIndex++)
                    {
                        var betType = report.ChildsInfo[betTypeGroupIndex];
                        var betTypeLength = betType.ChildsInfo.Count;

                        for (var betTypeIndex = 0; betTypeIndex < betTypeLength; betTypeIndex++)
                        {
                            var winLossInfo = betType.ChildsInfo[betTypeIndex];
                            this.ExportDetailReportRow_ByBetType(
                                ref worksheet,
                                ref currentRow,
                                winLossInfo,
                                reportCellStyle);

                            currentRow++;
                        }

                        var subTotalTile = string.Format("{0} {1} {2}", betType.LiveIndicatorId ? "!" + Labels.Live + " -" : string.Empty, betType.Name, Labels.Total).ToUpper();
                        this.ExportDetailReportTotal_ByBetType(
                        ref worksheet,
                        ref currentRow,
                        subTotalTile,
                        betType.GrandTotal,
                        subTotalTextStyle,
                        subTotalDecimalNumberStyle);

                        currentRow++;
                    }

                    this.ExportDetailReportTotal_ByBetType(
                        ref worksheet,
                        ref currentRow,
                        report.GrandTotal.Name,
                        report.GrandTotal,
                        grandTotalTextStyle,
                        grandTotalDecimalNumberStyle);
                }

                var reportName = string.Format(
                    MainResources.MatchWinLoss2aDetailBySportType_ReportName,
                    criteria.Winlostdate.ToShortFormatedDateWithSlash().Replace('/', '_'));
                NpoiExcelExtensions.ResponseStreamExcel(excelFile, reportName);
            }
            catch (Exception ex)
            {
                HandleException(ex);
            }
        }
    }
}