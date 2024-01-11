namespace Fanex.AccountingReports.Licensee.Services.Implement
{
    using AccountingReports.Core.Configurations;
    using ExcelUtil.Spreadsheet;
    using Fanex.AccountingReports.Core.Extensions;
    using Fanex.AccountingReports.Core.Services.Impl;
    using Fanex.AccountingReports.Core.Web;
    using Fanex.AccountingReports.Licensee.Criteria.NegativeBalance;
    using Fanex.AccountingReports.Licensee.Models;
    using Fanex.AccountingReports.Licensee.Models.NegativeBalance;
    using Fanex.AccountingReports.Licensee.Repositories.Implement;
    using Fanex.AccountingReports.Licensee.Resources;
    using Fanex.Common.Extensions;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Drawing;
    using System.Linq;
    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class NegativeBalanceService : ServiceBase
    {
        private readonly NegativeBalanceRepository _negativeBalanceRepository;

        public NegativeBalanceService(NegativeBalanceRepository negativeBalanceRepository = null)
        {
            this._negativeBalanceRepository = negativeBalanceRepository ?? new NegativeBalanceRepository();
        }

        /// <summary>
        /// Gets the negative balance.
        /// </summary>
        /// <param name="negativeBalanceCriteria">The negative balance criteria.</param>
        /// <returns>The negative balance.</returns>
        [SuppressMessage("StyleCop.CSharp.StylecopPlus.Rules",
            "SP2101:MethodMustNotContainMoreLinesThan",
            Justification = "Reviewed. Suppression is OK here.")]
        public NegativeBalanceModel GetNegativeBalance(NegativeBalanceCriteria negativeBalanceCriteria)
        {
            var model = new NegativeBalanceModel();
            var rawData =  this._negativeBalanceRepository.GetNegativeBalanceUser(negativeBalanceCriteria).ToList();
            
            List<NegativeBalanceUser> _listUserTemp = new List<NegativeBalanceUser>();
            if (rawData != null && (rawData.Count() > 0))
            {
                int from = negativeBalanceCriteria.PageSize * (negativeBalanceCriteria.PageIndex - 1);
                int to = negativeBalanceCriteria.PageSize * negativeBalanceCriteria.PageIndex;
                for (int i = from; i < rawData.Count && i < to; i++)
                {
                    _listUserTemp.Add(rawData[i]);
                }

                NegativeBalance currentNegative = null;
                _listUserTemp.OrderBy(negativeBalanceUser => negativeBalanceUser.Site).ForEach(data =>
                    {
                        if (currentNegative == null || currentNegative.LicenseeSite != data.Site)
                        {
                            currentNegative = new NegativeBalance
                            {
                                LicenseeSite = data.Site,
                                NegativeBalanceUserList = new List<NegativeBalanceUser>()
                            };
                            model.NegativeBalanceList.Add(currentNegative);
                        }

                        currentNegative.NegativeBalanceUserList.Add(data);
                    });
                model.TotalNegativeBalanceUser = rawData.Count();
            }

            return model;
        }

        /// <summary>
        /// Gets the excel negative balance.
        /// </summary>
        /// <param name="negativeBalanceCriteria">The negative balance criteria.</param>
        /// <param name="site">The site value.</param>
        /// <returns>The excel negative balance.</returns>
        [SuppressMessage("StyleCop.CSharp.StylecopPlus.Rules",
            "SP2101:MethodMustNotContainMoreLinesThan",
            Justification = "Reviewed. Suppression is OK here.")]
        public ExcelFile GetExcelNegativeBalance(NegativeBalanceCriteria negativeBalanceCriteria, string site)
        {
            //// local varialbles

            int startCol = 0;
            int startRow = 2;

            var excelFile = new ExcelFile();

            var rawData =  this._negativeBalanceRepository.GetNegativeBalanceUser(negativeBalanceCriteria).ToList();
            var model = new NegativeBalanceModel();
            NegativeBalance currentNegative = null;

            rawData.OrderBy(negativeBalanceUser => negativeBalanceUser.Site)
                .Select(s => s).ForEach(data =>
                {
                    if (currentNegative == null || currentNegative.LicenseeSite.ToLower() != data.Site.ToLower())
                    {
                        currentNegative = new NegativeBalance
                        {
                            LicenseeSite = data.Site,
                            NegativeBalanceUserList = new List<NegativeBalanceUser>()
                        };
                        model.NegativeBalanceList.Add(currentNegative);
                    }

                    currentNegative.NegativeBalanceUserList.Add(data);
                });


            var excelReport = excelFile.Worksheets.Add(string.Format("NegativeBalance"));

            //// local varialbles

            //// header

            var siteName = (negativeBalanceCriteria.Site == "All" ?
                        "All Licensees" :
                        negativeBalanceCriteria.Site);
            var reportName = string.Format(
                "Accounting system - Negative Balance Report for {0} ",
                siteName);

            this.ExportExcelNegativeBalanceReportAssignHeader(
                startRow,
                startCol,
                ref excelReport,
                reportName);

            //// header

            //// content
            int nextRow = 1;
            if (model != null && model.NegativeBalanceList != null && model.NegativeBalanceList.Count > 0)
            {
                this.ExportExcelNegativeBalanceReportAssignBody(startRow, startCol, ref nextRow, ref excelReport, model);
            }
            //// content
            return excelFile;
        }

        /// <summary>
        /// Exports the excel negative balance.
        /// </summary>
        /// <param name="negativeBalanceCriteria">The negative balance criteria.</param>
        public void ExportExcelNegativeBalance(NegativeBalanceCriteria negativeBalanceCriteria)
        {
            var reportName = LicenseeResources.NegativeBalance_ReportFileName +
                (negativeBalanceCriteria.Site == "All" ?
                "_All_Licensees" :
                "_" + negativeBalanceCriteria.Site);

            ExcelFile excelFile = this.GetExcelNegativeBalance(negativeBalanceCriteria, string.Empty);
            excelFile.ReponseStream(
                string.Format("{0}", reportName));
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
        /// Exports the excel negative balance report assign header.
        /// </summary>
        /// <param name="startRow">The start row.</param>
        /// <param name="startCol">The start col.</param>
        /// <param name="excelReport">The excel report.</param>
        /// <param name="title">The title.</param>
        private void ExportExcelNegativeBalanceReportAssignHeader(int startRow, int startCol, ref ExcelWorksheet excelReport, string title)
        {
            var backgroundHeaderFormat = ExcelExtensions.GetCellTitleStyle(Color.FromArgb(60, 94, 165));
            backgroundHeaderFormat.Font.Color = Color.FromArgb(255, 255, 255);
            var nextCol = 0;

            excelReport.Rows[startRow].Height = 255 * 2;
            excelReport.Cells[startRow, startCol].Style = backgroundHeaderFormat;
            excelReport.Cells[startRow, startCol].Value = Labels.UserName;
            excelReport.Columns[startCol].Width = 255 * 20;

            excelReport.Cells[startRow, startCol + (nextCol += 1)].Style = backgroundHeaderFormat;
            excelReport.Cells[startRow, startCol + nextCol].Value = Labels.Balance;
            excelReport.Columns[startCol + nextCol].Width = 255 * 20;

            excelReport.Cells[startRow, startCol + (nextCol += 1)].Style = backgroundHeaderFormat;
            excelReport.Cells[startRow, startCol + nextCol].Value = Labels.Currency;
            excelReport.Columns[startCol + nextCol].Width = 255 * 20;

            excelReport.Rows[startRow - 2].Height = 255 * 2;
            excelReport.Cells.GetSubrangeAbsolute(startRow - 2, startCol, startRow - 2, startCol + nextCol).Merged = true;
            excelReport.Cells.GetSubrangeAbsolute(startRow - 2, startCol, startRow - 2, startCol + nextCol).Style = backgroundHeaderFormat;
            excelReport.Cells.GetSubrangeAbsolute(startRow - 2, startCol, startRow - 2, startCol + nextCol).Value = title;
        }

        /// <summary>
        /// Exports the excel negative balance report assign body.
        /// </summary>
        /// <param name="startRow">The start row.</param>
        /// <param name="startCol">The start col.</param>
        /// <param name="nextRow">The next row.</param>
        /// <param name="excelReport">The excel report.</param>
        /// <param name="model">The model.</param>
        private void ExportExcelNegativeBalanceReportAssignBody(int startRow, int startCol, ref int nextRow, ref ExcelWorksheet excelReport, NegativeBalanceModel model)
        {
            var cellStyle = ExcelExtensions.GetCellTextStyle(ExcelExtensions.OddRowColor, false);
            var cellStyleEven = ExcelExtensions.GetCellTextStyle(ExcelExtensions.EvenRowColor, false);
            var cellStyleSite = ExcelExtensions.GetCellTextStyle(ExcelExtensions.SubTotalRowColor, false);
            cellStyle.HorizontalAlignment = cellStyleEven.HorizontalAlignment = HorizontalAlignmentStyle.Center;
            int count = 1;

            for (int i = 0; i < model.NegativeBalanceList.Count; i++)
            {
                var nextCol = 0;
                var siteTemp = model.NegativeBalanceList[i];
                if (model.NegativeBalanceList.Count > 1)
                {
                    excelReport.Cells[startRow + nextRow, startCol].Style = cellStyleSite;
                    excelReport.Cells[startRow + nextRow, startCol].Value = siteTemp.LicenseeSite;

                    excelReport.Cells[startRow + nextRow, startCol + (nextCol += 1)].Style = cellStyleSite;
                    excelReport.Cells[startRow + nextRow, startCol + nextCol].Value = string.Empty;

                    excelReport.Cells[startRow + nextRow, startCol + (nextCol += 1)].Style = cellStyleSite;
                    excelReport.Cells[startRow + nextRow, startCol + nextCol].Value = string.Empty;
                    nextRow++;
                }

                for (int j = 0; j < siteTemp.NegativeBalanceUserList.Count; j++)
                {
                    var style = count % 2 == 1 ? cellStyle : cellStyleEven;
                    nextCol = 0;
                    var negativeUserTmp = siteTemp.NegativeBalanceUserList[j];
                    excelReport.Cells[startRow + nextRow, startCol].Style = style;
                    excelReport.Cells[startRow + nextRow, startCol].Value = negativeUserTmp.UserName;

                    excelReport.Cells[startRow + nextRow, startCol + (nextCol += 1)].Style = style;
                    excelReport.Cells[startRow + nextRow, startCol + nextCol].Style.HorizontalAlignment = HorizontalAlignmentStyle.Right;
                    excelReport.Cells[startRow + nextRow, startCol + nextCol].Value = negativeUserTmp.Balance.Format(2);

                    excelReport.Cells[startRow + nextRow, startCol + (nextCol += 1)].Style = style;
                    excelReport.Cells[startRow + nextRow, startCol + nextCol].Value = negativeUserTmp.Currency;
                    nextRow++;
                    count++;
                }

                count++;
            }
        }
    }
}