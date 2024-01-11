namespace Fanex.AccountingReports.Licensee.Services.Implement
{
    using AccountingReports.Core.Configurations;
    using ExcelUtil.Spreadsheet;
    using Fanex.AccountingReports.Core.Extensions;
    using Fanex.AccountingReports.Core.Services.Impl;
    using Fanex.AccountingReports.Core.Web;
    using Fanex.AccountingReports.Licensee.Criteria.PeakConcurrentUser;
    using Fanex.AccountingReports.Licensee.Models;
    using Fanex.AccountingReports.Licensee.Models.PeakConcurrentUser;
    using Fanex.AccountingReports.Licensee.Resources;
    using Fanex.Common;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class PeakConcurrentUserService : ServiceBase
    {
        /// <summary>
        /// Gets the concurrent user list.
        /// </summary>
        /// <param name="peakConcurrentUserCriteria">The peak concurrent user criteria.</param>
        /// <returns>The concurrent user list.</returns>
        public PeakConcurrentUser GetConcurrentUserList(PeakConcurrentUserCriteria peakConcurrentUserCriteria)
        {
            var peakCocurrentUser = new PeakConcurrentUser();
            peakConcurrentUserCriteria.ToDate = peakConcurrentUserCriteria.ToDate.AddDays(1).AddSeconds(-1);
            var rawData = GetReportData<dynamic>(peakConcurrentUserCriteria);
            if (rawData != null && (rawData.Count() > 0))
            {
                var concurrentUserList = new List<ConcurrentUser>();
                var listTmp = new List<double>();
                double totalUser = 0;
                foreach (var row in rawData)
                {
                    var cocurrentUser = new ConcurrentUser()
                    {
                        Date = string.Format("{0:MM/dd/yyyy}", row.logdate),
                        TotalUser = ((double)row.totalUser).Format(0)
                    };
                    listTmp.Add(row.totalUser);
                    concurrentUserList.Add(cocurrentUser);
                    totalUser += (int)row.totalUser;
                }

                peakCocurrentUser.ListConcurrentUser = concurrentUserList;
                peakCocurrentUser.AverageUsers = (totalUser / concurrentUserList.Count).Format(2);
                peakCocurrentUser.Total = totalUser.Format(0);
                peakCocurrentUser.MinUsers = listTmp.Min().Format(0);
                peakCocurrentUser.MaxUsers = listTmp.Max().Format(0);
            }

            
            return peakCocurrentUser;
        }

        /// <summary>
        /// Peaks the export2 excel.
        /// </summary>
        /// <param name="peakConcurrentUserCriteria">The peak concurrent user criteria.</param>
        public void PeakExport2Excel(PeakConcurrentUserCriteria peakConcurrentUserCriteria)
        {
            int startCol = 0;
            int startRow = 2;
            var reportName = LicenseeResources.PeakConcurrentUser_ReportFileName;
            var excelFile = new ExcelFile();
            var rawData =  this.GetConcurrentUserList(peakConcurrentUserCriteria);
            var excelReport = excelFile.Worksheets.Add(string.Format("PeakConcurrentUser"));

            var excelFileName = string.Format(
                "Peak Concurrent Users Report for {0} ",
                peakConcurrentUserCriteria.LicenseeName);
            this.ExportExcelPeakConcurrentReportAssignHeader(
                startRow,
                startCol,
                ref excelReport,
                excelFileName);

            int nextRow = 1;
            if (rawData != null && rawData.ListConcurrentUser != null && rawData.ListConcurrentUser.Count > 0)
            {
                foreach (var cocurrent in rawData.ListConcurrentUser)
                {
                    this.ExportExcelPeakConcurrentReportAssignBody(startRow, startCol, ref nextRow, ref excelReport, cocurrent);
                }
            }

            this.ExportExcelPeakConcurrentReportAssignFooter(startRow, startCol, ref nextRow, ref excelReport, ref rawData);

            excelFile.ReponseStream(string.Format(
                "{0}_For_{1}",
                reportName,
                peakConcurrentUserCriteria.LicenseeName));
        }

        public List<Licensee> GetLicensee()
        {
            var licenseeSiteCriteria = new LicenseeCriteria();
            var licenseeSiteList = new List<Licensee>();
            var rawData = GetReportData<dynamic>(licenseeSiteCriteria);
            if (rawData != null)
            {
                foreach (var row in rawData)
                {
                    var licenseeSite = new Licensee()
                    {
                        LicenseeName = row.LicenseeName,
                        BrandName = row.Brandname,
                        LicenseeId = row.LicenseeID,
                        IsEnabledBA = row.EnableBA,
                        IsMultiAgent = AppSettings.MultiAgentLicensses.Contains(row.LicenseeName)
                    };

                    licenseeSiteList.Add(licenseeSite);
                }
            }

            return licenseeSiteList;
        }

        /// <summary>
        /// Exports the excel peak concurrent report assign header.
        /// </summary>
        /// <param name="startRow">The start row.</param>
        /// <param name="startCol">The start col.</param>
        /// <param name="excelReport">The excel report.</param>
        /// <param name="title">The title.</param>
        private void ExportExcelPeakConcurrentReportAssignHeader(int startRow, int startCol, ref ExcelWorksheet excelReport, string title)
        {
            var backgroundHeaderFormat = ExcelExtensions.GetCellTitleStyle(ExcelExtensions.HeaderTitleColor);
            backgroundHeaderFormat.Font.Color = ExcelExtensions.OddRowColor;
            var nextCol = 0;
            excelReport.Rows[startRow].Height = 255 * 2;
            excelReport.Cells[startRow, startCol].Style = backgroundHeaderFormat;
            excelReport.Cells[startRow, startCol].Value = Labels.Date;
            excelReport.Columns[startCol].Width = 255 * 15;

            excelReport.Cells[startRow, startCol + (nextCol += 1)].Style = backgroundHeaderFormat;
            excelReport.Cells[startRow, startCol + nextCol].Value = LicenseeResources.PeakConcurrentUser;
            excelReport.Columns[startCol + nextCol].Width = 255 * 20;

            excelReport.Rows[startRow - 2].Height = 255 * 2;
            excelReport.Cells.GetSubrangeAbsolute(startRow - 2, startCol, startRow - 2, startCol + nextCol).Merged = true;
            excelReport.Cells.GetSubrangeAbsolute(startRow - 2, startCol, startRow - 2, startCol + nextCol).Style = backgroundHeaderFormat;
            excelReport.Cells.GetSubrangeAbsolute(startRow - 2, startCol, startRow - 2, startCol + nextCol).Value = title;
        }

        /// <summary>
        /// Exports the excel peak concurrent report assign body.
        /// </summary>
        /// <param name="startRow">The start row.</param>
        /// <param name="startCol">The start col.</param>
        /// <param name="nextRow">The next row.</param>
        /// <param name="excelReport">The excel report.</param>
        /// <param name="concurrentUser">The concurrent user.</param>
        private void ExportExcelPeakConcurrentReportAssignBody(
            int startRow,
            int startCol,
            ref int nextRow,
            ref ExcelWorksheet excelReport,
            ConcurrentUser concurrentUser)
        {
            var cellStyle = ExcelExtensions.GetCellTextStyle(ExcelExtensions.OddRowColor, false);
            var cellStyleEven = ExcelExtensions.GetCellTextStyle(ExcelExtensions.EvenRowColor, false);
            cellStyle.HorizontalAlignment = HorizontalAlignmentStyle.Center;
            cellStyleEven.HorizontalAlignment = HorizontalAlignmentStyle.Center;
            var style = nextRow % 2 == 1 ? cellStyle : cellStyleEven;
            var nextCol = 0;
            excelReport.Cells[startRow + nextRow, startCol].Style = style;
            excelReport.Cells[startRow + nextRow, startCol].Value = concurrentUser.Date;
            excelReport.Cells[startRow + nextRow, startCol + (nextCol += 1)].Style = style;
            excelReport.Cells[startRow + nextRow, startCol + nextCol].Value = Converter.ToDecimal(concurrentUser.TotalUser);
            nextRow++;
        }

        /// <summary>
        /// Exports the excel peak concurrent report assign footer.
        /// </summary>
        /// <param name="startRow">The start row.</param>
        /// <param name="startCol">The start col.</param>
        /// <param name="nextRow">The next row.</param>
        /// <param name="excelReport">The excel report.</param>
        /// <param name="peakConcurrentUser">The peak concurrent user.</param>
        private void ExportExcelPeakConcurrentReportAssignFooter(
            int startRow,
            int startCol,
            ref int nextRow,
            ref ExcelWorksheet excelReport,
            ref PeakConcurrentUser peakConcurrentUser)
        {
            var cellStyle = ExcelExtensions.GetCellTextStyle(ExcelExtensions.GrandTotalRowColor, false);
            cellStyle.Font.Weight = ExcelFont.BoldWeight;
            cellStyle.HorizontalAlignment = HorizontalAlignmentStyle.Center;
            var nextCol = 0;

            excelReport.Cells[startRow + nextRow, startCol].Style = cellStyle;
            excelReport.Cells[startRow + nextRow, startCol].Value = LicenseeResources.PeakConcurrentUser_Total;
            excelReport.Columns[nextCol].Width = 255 * 30;

            excelReport.Cells[startRow + nextRow, startCol + (nextCol += 1)].Style = cellStyle;
            excelReport.Cells[startRow + nextRow, startCol + nextCol].Value = Converter.ToDecimal(peakConcurrentUser.Total);
            excelReport.Columns[nextCol].Width = 255 * 30;

            nextCol = 0;
            nextRow++;
            excelReport.Cells[startRow + nextRow, startCol].Style = cellStyle;
            excelReport.Cells[startRow + nextRow, startCol].Value = LicenseeResources.PeakConcurrentUser_Average;
            excelReport.Cells[startRow + nextRow, startCol + (nextCol += 1)].Style = cellStyle;
            excelReport.Cells[startRow + nextRow, startCol + nextCol].Value = Converter.ToDecimal(peakConcurrentUser.AverageUsers);

            nextCol = 0;
            nextRow++;
            excelReport.Cells[startRow + nextRow, startCol].Style = cellStyle;
            excelReport.Cells[startRow + nextRow, startCol].Value = LicenseeResources.PeakConcurrentUser_Max;
            excelReport.Cells[startRow + nextRow, startCol + (nextCol += 1)].Style = cellStyle;
            excelReport.Cells[startRow + nextRow, startCol + nextCol].Value = Converter.ToDecimal(peakConcurrentUser.MaxUsers);

            nextCol = 0;
            nextRow++;
            excelReport.Cells[startRow + nextRow, startCol].Style = cellStyle;
            excelReport.Cells[startRow + nextRow, startCol].Value = LicenseeResources.PeakConcurrentUser_Min;
            excelReport.Cells[startRow + nextRow, startCol + (nextCol += 1)].Style = cellStyle;
            excelReport.Cells[startRow + nextRow, startCol + nextCol].Value = Converter.ToDecimal(peakConcurrentUser.MinUsers);
        }
    }
}