namespace Fanex.AccountingReports.Misc.Services.Implement
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using ExcelUtil.Spreadsheet;
    using Fanex.AccountingReports.Core.Extensions;
    using Fanex.AccountingReports.Core.Models;
    using Fanex.AccountingReports.Core.Services.Impl;
    using Fanex.AccountingReports.Core.Web;
    using Fanex.AccountingReports.Misc.Constants;
    using Fanex.AccountingReports.Misc.Criteria.TotalSMAMemberActive;
    using Fanex.AccountingReports.Misc.Models.TotalSMAMemberActive;
    using Fanex.AccountingReports.Misc.Repositories.Implement;
    using Fanex.AccountingReports.Misc.Resources;
    using Fanex.Common;
    using Fanex.Data.Repository;

    public class TotalSMAMemberActiveService : ServiceBase
    {
        private readonly TotalSMAMemberActiveRepository _repositoryReport;

        public TotalSMAMemberActiveService()
            : this(null)
        {
        }

        public TotalSMAMemberActiveService(TotalSMAMemberActiveRepository repositoryReport)
        {
            _repositoryReport = repositoryReport ?? new TotalSMAMemberActiveRepository();
        }

        #region Implement methods

        [SuppressMessage("StyleCop.CSharp.StylecopPlus.Rules",
            "SP2101:MethodMustNotContainMoreLinesThan",
            Justification = "Reviewed. Suppression is OK here.")]
        public void ExportReport(GetTotalSMAMemberActiveCriteria criteria)
        {
            var report = this.GetReport(criteria);
            if (report == null || report.Count == 0)
            {
                return;
            }

            ExcelFile excelFile = new ExcelFile();
            ExcelWorksheetCollection worksheets = excelFile.Worksheets;
            string sheetName = MiscResources.TotalSMAMemberActiveReport_SheetName;
            ExcelWorksheet workSheet = worksheets.Add(sheetName);

            int startRow = 0;
            int startColumn = 0;
            int currentRow = startRow;
            int currentColumn = startColumn;

            ExcelCell currentCell;
            CellRange currentCellRange;

            var titleTextStyle = ExcelExtensions.GetCellTextStyle(
                ExcelExtensions.HeaderTitleColor,
                true,
                HorizontalAlignmentStyle.Left,
                false,
                ExcelExtensions.BackGroundColor);
            var headerTextStyle = ExcelExtensions.GetCellTitleStyle(ExcelExtensions.HeaderTitleColor);
            var textStyle = ExcelExtensions.GetCellTextStyle(ExcelExtensions.BackGroundColor, false, HorizontalAlignmentStyle.Center);

            int maxColumnIndex = 3;
            for (int i = 0; i < maxColumnIndex; i++)
            {
                workSheet.Columns[startColumn + i].Width = 150 * ExcelExtensions.FontWidth;
            }

            string title = string.Format(
                MiscResources.TotalSMAMemberActiveReport_Title,
                criteria.FromDate.ToShortFormatedDate(),
                criteria.ToDate.ToShortFormatedDate());
            currentColumn = startColumn;
            currentCellRange = workSheet.Cells.GetSubrangeAbsolute(currentRow, currentColumn, currentRow + 1, currentColumn + maxColumnIndex);
            currentCellRange.Merged = true;
            currentCellRange.Value = title;
            currentCellRange.Style = titleTextStyle;
            currentCellRange.Style.HorizontalAlignment = HorizontalAlignmentStyle.Left;
            currentRow += 2;

            // empty row
            workSheet.ExportEmptyRow(startRow, startColumn, ref currentRow, startColumn + maxColumnIndex);

            currentCell = workSheet.Cells[currentRow, currentColumn];
            currentCell.Value = Labels.NewCreated;
            currentCell.Style = headerTextStyle;
            currentColumn++;

            currentCell = workSheet.Cells[currentRow, currentColumn];
            currentCell.Value = Labels.Active;
            currentCell.Style = headerTextStyle;
            currentColumn++;

            currentCell = workSheet.Cells[currentRow, currentColumn];
            currentCell.Value = Labels.Suspended;
            currentCell.Style = headerTextStyle;
            currentColumn++;

            currentCell = workSheet.Cells[currentRow, currentColumn];
            currentCell.Value = Labels.Closed;
            currentCell.Style = headerTextStyle;

            int index = -1;
            var newCreated = report[++index];
            var active = report[++index];
            var suspended = report[++index];
            var closed = report[++index];
            ++index;

            currentRow++;
            currentColumn = startColumn;

            currentCell = workSheet.Cells[currentRow, currentColumn];
            currentCell.Value = this.BuildSMA(newCreated);
            currentCell.Style = textStyle;
            currentColumn++;

            currentCell = workSheet.Cells[currentRow, currentColumn];
            currentCell.Value = this.BuildSMA(active);
            currentCell.Style = textStyle;
            currentColumn++;

            currentCell = workSheet.Cells[currentRow, currentColumn];
            currentCell.Value = this.BuildSMA(suspended);
            currentCell.Style = textStyle;
            currentColumn++;

            currentCell = workSheet.Cells[currentRow, currentColumn];
            currentCell.Value = this.BuildSMA(closed);
            currentCell.Style = textStyle;

            int reportLen = report.Count;
            int maxColumIndexActive = reportLen - index - 1;
            for (int i = 1; i <= maxColumIndexActive; i++)
            {
                workSheet.Columns[startColumn + i].Width = 150 * ExcelExtensions.FontWidth;
            }

            currentRow++;
            workSheet.ExportEmptyRow(startRow, startColumn, ref currentRow, startColumn + maxColumIndexActive);

            title = MiscResources.TotalSMAMemberActiveReport_ActiveTitle;
            currentColumn = startColumn;
            currentCellRange = workSheet.Cells.GetSubrangeAbsolute(currentRow, currentColumn, currentRow + 1, currentColumn + maxColumIndexActive);
            currentCellRange.Merged = true;
            currentCellRange.Value = title;
            currentCellRange.Style = titleTextStyle;
            currentCellRange.Style.HorizontalAlignment = HorizontalAlignmentStyle.Left;
            currentRow += 2;

            // empty row
            workSheet.ExportEmptyRow(startRow, startColumn, ref currentRow, startColumn + maxColumIndexActive);

            currentColumn = startColumn;
            for (int i = index; i < reportLen; i++)
            {
                currentCell = workSheet.Cells[currentRow, currentColumn];
                currentCell.Value = report[i].Key.Name;
                currentCell.Style = headerTextStyle;
                currentColumn++;
            }

            currentRow++;
            currentColumn = startColumn;
            for (int i = index; i < reportLen; i++)
            {
                currentCell = workSheet.Cells[currentRow, currentColumn];
                currentCell.Value = this.BuildSMA(report[i]);
                currentCell.Style = textStyle;
                currentColumn++;
            }

            var reportName = string.Format(
                MiscResources.TotalSMAMemberActiveReport_ReportName,
                criteria.FromDate.ToShortFormatedDateWithSlash(),
                criteria.ToDate.ToShortFormatedDateWithSlash()).Replace("/", "_");
            excelFile.ReponseStream(reportName);
        }

        public void ExportStatusDetails(GetStatusDetailsCriteria criteria)
        {
            Hashtable outputParams = null;
            criteria.PageIndex = criteria.PageSize = 0;
            var rawData = this._repositoryReport.GetStatusDetail(criteria, ref outputParams);

            int totalRow = 0;
            if (outputParams != null)
            {
                totalRow = Converter.ToInt(outputParams["totalrow"]);
            }

            if (totalRow <= 0)
            {
                return;
            }

            ExcelFile excelFile = new ExcelFile();
            ExcelWorksheetCollection worksheets = excelFile.Worksheets;
            Status status = (Status)criteria.Status;
            int pageIndex = 0;
            int maxRowPerSheet = ExcelExtensions.MaxRowPerSheet;
            do
            {
                var sheetData = rawData
                    .Skip(maxRowPerSheet * pageIndex)
                    .Take(maxRowPerSheet)
                    .ToList();
                pageIndex++;

                // export excel here ...
                IList<GroupStatusDetailInfo> report = this.GetStatusDetails(sheetData, status);
                this.ExportStatusDetails(ref worksheets, report, status, pageIndex, criteria.SiteName);
            }
            while (totalRow > maxRowPerSheet * pageIndex);

            var reportName = string.Format(
                MiscResources.TotalSMAMemberActiveReport_ReportName,
                criteria.FromDate.ToShortFormatedDateWithSlash(),
                criteria.ToDate.ToShortFormatedDateWithSlash()).Replace("/", "_");
            excelFile.ReponseStream(reportName);
        }

        [SuppressMessage("StyleCop.CSharp.StylecopPlus.Rules",
            "SP2101:MethodMustNotContainMoreLinesThan",
            Justification = "Reviewed. Suppression is OK here.")]
        public IList<SMASummaryInfo> GetReport(GetTotalSMAMemberActiveCriteria criteria)
        {
            IList<SMASummaryInfo> report = null;
            dynamic totalSMA = null;
            IList<dynamic> activeDetails = null;
            _repositoryReport.GetReport(criteria, ref totalSMA, ref activeDetails);

            if (totalSMA != null && activeDetails != null && activeDetails.Count > 0)
            {
                report = new List<SMASummaryInfo>();

                // New Created
                report.Add(new SMASummaryInfo
                {
                    Key = new EntityBase<int, string>
                    {
                        Name = Labels.NewCreated
                    },
                    Super = Converter.ToInt(totalSMA.NewCreatedSuper),
                    Master = Converter.ToInt(totalSMA.NewCreatedMaster),
                    Agent = Converter.ToInt(totalSMA.NewCreatedAgent)
                });

                // Active
                report.Add(new SMASummaryInfo
                {
                    Key = new EntityBase<int, string>
                    {
                        Name = Labels.Active
                    },
                    Super = Converter.ToInt(totalSMA.ActiveSuper),
                    Master = Converter.ToInt(totalSMA.ActiveMaster),
                    Agent = Converter.ToInt(totalSMA.ActiveAgent)
                });

                // Suspended
                report.Add(new SMASummaryInfo
                {
                    Key = new EntityBase<int, string>
                    {
                        Name = Labels.Suspended
                    },
                    Super = Converter.ToInt(totalSMA.SuspendedSuper),
                    Master = Converter.ToInt(totalSMA.SuspendedMaster),
                    Agent = Converter.ToInt(totalSMA.SuspendedAgent)
                });

                // Closed
                report.Add(new SMASummaryInfo
                {
                    Key = new EntityBase<int, string>
                    {
                        Name = Labels.Closed
                    },
                    Super = Converter.ToInt(totalSMA.ClosedSuper),
                    Master = Converter.ToInt(totalSMA.ClosedMaster),
                    Agent = Converter.ToInt(totalSMA.ClosedAgent)
                });

                // Active Details
                SMASummaryInfo smaSummary = null;
                foreach (var item in activeDetails)
                {
                    smaSummary = this.BuildSMASummaryInfo(item);
                    report.Add(smaSummary);
                }

            }

            return report;
        }

        public IList<EntityBase<int, string>> GetSites(CriteriaBase criteria)
        {
            IList<EntityBase<int, string>> sites = null;
            var rawData = GetReportData<SiteEntity>(criteria);
            sites = new List<EntityBase<int, string>>();
            foreach (var item in rawData)
            {
                sites.Add(new EntityBase<int, string>
                {
                    Id = item.SiteId,
                    Name = item.Site
                });
            }

            return sites;
        }

        public IList<GroupStatusDetailInfo> GetStatusDetails(GetStatusDetailsCriteria criteria)
        {
            Status status = (Status)criteria.Status;
            IList<GroupStatusDetailInfo> report = null;
            Hashtable outputParams = null;
            var rawData = this._repositoryReport.GetStatusDetail(criteria, ref outputParams);

            int totalRow = 0;
            if (outputParams != null)
            {
                totalRow = Converter.ToInt(outputParams["totalrow"]);
            }

            if (totalRow > 0)
            {
                criteria.TotalRow = totalRow;
                report = this.GetStatusDetails(rawData, status);

            }

            return report;
        }

        private IList<GroupStatusDetailInfo> GetStatusDetails(IList<dynamic> rawData, Status status)
        {
            IList<GroupStatusDetailInfo> report = new List<GroupStatusDetailInfo>();

            GroupStatusDetailInfo groupStatusDetail = null;
            StatusDetailInfo statusDetail = null;
            var groupHeadlines = rawData.GroupBy(data => data.groupHeadline);
            foreach (var groupHeadline in groupHeadlines)
            {
                groupStatusDetail = new GroupStatusDetailInfo
                {
                    GroupHeadline = new EntityBase<int, string>
                    {
                        Name = groupHeadline.Key
                    },
                    ChildsInfo = new List<StatusDetailInfo>()
                };

                foreach (var item in groupHeadline)
                {
                    statusDetail = this.BuildStatusDetail(item, status);
                    groupStatusDetail.ChildsInfo.Add(statusDetail);
                }

                report.Add(groupStatusDetail);
            }

            return report;
        }

        [SuppressMessage("StyleCop.CSharp.StylecopPlus.Rules",
            "SP2101:MethodMustNotContainMoreLinesThan",
            Justification = "Reviewed. Suppression is OK here.")]
        private void ExportStatusDetails(
            ref ExcelWorksheetCollection worksheet,
            IList<GroupStatusDetailInfo> report,
            Status status,
            int sheetNo,
            string siteNames)
        {
            string sheetName = MiscResources.TotalSMAMemberActiveReport_SheetName + " " + sheetNo;
            ExcelWorksheet workSheet = worksheet.Add(sheetName);

            int startRow = 0;
            int startColumn = 0;
            int currentRow = startRow;
            int currentColumn = startColumn;

            ExcelCell currentCell;
            CellRange currentCellRange;

            var titleTextStyle = ExcelExtensions.GetCellTextStyle(
      ExcelExtensions.HeaderTitleColor,
      true,
      HorizontalAlignmentStyle.Left,
      false,
      ExcelExtensions.BackGroundColor);
            var headerTextStyle = ExcelExtensions.GetCellTitleStyle(ExcelExtensions.HeaderTitleColor);
            var evenTextStyle = ExcelExtensions.GetCellTextStyle(ExcelExtensions.EvenRowColor, false, HorizontalAlignmentStyle.Center);
            var oddTextStyle = ExcelExtensions.GetCellTextStyle(ExcelExtensions.BackGroundColor, false, HorizontalAlignmentStyle.Center);
            CellStyle textStyle;

            int maxColumnIndex = status == Status.NewCreated ? 8 : 6;
            for (int i = 0; i < maxColumnIndex; i++)
            {
                workSheet.Columns[startColumn + i].Width = 150 * ExcelExtensions.FontWidth;
            }

            workSheet.Columns[maxColumnIndex - 1].Width = 180 * ExcelExtensions.FontWidth;

            string title = Labels.NotAvailable;
            switch (status)
            {
                case Status.NewCreated:
                    title = Labels.NewCreated;
                    break;

                case Status.Suspended:
                    title = Labels.Suspended;
                    break;

                case Status.Closed:
                    title = Labels.Closed;
                    break;

                default:
                    title = Labels.NotAvailable;
                    break;
            }

            title += " " + Labels.Details;
            if (!string.IsNullOrEmpty(siteNames))
            {
                // include sitename into title
                title += " - Site(s): " + siteNames;
            }

            currentColumn = startColumn;
            currentCellRange = workSheet.Cells.GetSubrangeAbsolute(currentRow, currentColumn, currentRow + 1, currentColumn + maxColumnIndex);
            currentCellRange.Merged = true;
            currentCellRange.Value = title;
            currentCellRange.Style = titleTextStyle;
            currentRow += 2;

            // empty row
            workSheet.ExportEmptyRow(startRow, startColumn, ref currentRow, startColumn + maxColumnIndex);

            currentCell = workSheet.Cells[currentRow, currentColumn];
            currentCell.Value = Labels.Date;
            currentCell.Style = headerTextStyle;
            currentColumn++;

            currentCell = workSheet.Cells[currentRow, currentColumn];
            currentCell.Value = Labels.GroupHeadline;
            currentCell.Style = headerTextStyle;
            currentColumn++;

            currentCell = workSheet.Cells[currentRow, currentColumn];
            currentCell.Value = Labels.UserName;
            currentCell.Style = headerTextStyle;
            currentColumn++;

            currentCell = workSheet.Cells[currentRow, currentColumn];
            currentCell.Value = Labels.Status;
            currentCell.Style = headerTextStyle;
            currentColumn++;

            currentCell = workSheet.Cells[currentRow, currentColumn];
            currentCell.Value = Labels.Site;
            currentCell.Style = headerTextStyle;
            currentColumn++;

            currentCell = workSheet.Cells[currentRow, currentColumn];
            currentCell.Value = Labels.Currency;
            currentCell.Style = headerTextStyle;
            currentColumn++;

            currentCell = workSheet.Cells[currentRow, currentColumn];
            currentCell.Value = Labels.Position;
            currentCell.Style = headerTextStyle;
            currentColumn++;

            if (status == Status.NewCreated)
            {
                currentCell = workSheet.Cells[currentRow, currentColumn];
                currentCell.Value = Labels.CreatedTime;
                currentCell.Style = headerTextStyle;
                currentColumn++;

                currentCell = workSheet.Cells[currentRow, currentColumn];
                currentCell.Value = Labels.Creator;
                currentCell.Style = headerTextStyle;
            }

            GroupStatusDetailInfo groupStatusDetail = null;
            StatusDetailInfo statusDetail = null;

            int len = report.Count;
            for (int i = 0; i < len; i++)
            {
                groupStatusDetail = report[i];
                textStyle = i % 2 == 0 ? evenTextStyle : oddTextStyle;

                var childsInfo = groupStatusDetail.ChildsInfo;
                int childLen = childsInfo.Count;
                for (int j = 0; j < childLen; j++)
                {
                    currentRow++;
                    currentColumn = startColumn;

                    statusDetail = childsInfo[j];

                    currentCell = workSheet.Cells[currentRow, currentColumn];
                    currentCell.Value = statusDetail.Date.ToShortFormatedDate();
                    currentCell.Style = textStyle;
                    currentColumn++;

                    if (j == 0)
                    {
                        // GroupHeadline
                        currentCellRange = workSheet.Cells.GetSubrangeAbsolute(currentRow, currentColumn, currentRow + childLen - 1, currentColumn);
                        currentCellRange.Merged = true;
                        currentCellRange.Value = groupStatusDetail.GroupHeadline.Name;
                        currentCellRange.Style = textStyle;
                    }

                    currentColumn++;

                    // Username
                    currentCell = workSheet.Cells[currentRow, currentColumn];
                    currentCell.Value = statusDetail.Username;
                    currentCell.Style = textStyle;
                    currentColumn++;

                    // Status
                    currentCell = workSheet.Cells[currentRow, currentColumn];
                    currentCell.Value = statusDetail.Status;
                    currentCell.Style = textStyle;
                    currentColumn++;

                    // Site
                    currentCell = workSheet.Cells[currentRow, currentColumn];
                    currentCell.Value = statusDetail.Site;
                    currentCell.Style = textStyle;
                    currentColumn++;

                    // Currency
                    currentCell = workSheet.Cells[currentRow, currentColumn];
                    currentCell.Value = statusDetail.Currency;
                    currentCell.Style = textStyle;
                    currentColumn++;

                    // Position
                    currentCell = workSheet.Cells[currentRow, currentColumn];
                    currentCell.Value = statusDetail.Position;
                    currentCell.Style = textStyle;
                    currentColumn++;

                    if (status == Status.NewCreated)
                    {
                        // CreatedTime
                        currentCell = workSheet.Cells[currentRow, currentColumn];
                        currentCell.Value = statusDetail.CreatedTime.ToLongFormatedDate();
                        currentCell.Style = textStyle;
                        currentColumn++;

                        // Creator
                        currentCell = workSheet.Cells[currentRow, currentColumn];
                        currentCell.Value = statusDetail.Creator;
                        currentCell.Style = textStyle;
                    }
                }
            }
        }

        #endregion Implement methods

        private string BuildSMA(SMASummaryInfo smaInfo)
        {
            return string.Format("{0}/{1}/{2}", smaInfo.Super, smaInfo.Master, smaInfo.Agent);
        }

        private SMASummaryInfo BuildSMASummaryInfo(dynamic rawData)
        {
            string[] smaArray = Converter.ToString2(rawData.SMA)
                                         .Split(new char[] { '/' }, StringSplitOptions.RemoveEmptyEntries);
            return new SMASummaryInfo
            {
                Key = new EntityBase<int, string>
                {
                    Id = Converter.ToInt(rawData.Id),
                    Name = Converter.ToString2(rawData.SiteName)
                },
                Super = Converter.ToInt(smaArray[0]),
                Master = Converter.ToInt(smaArray[1]),
                Agent = Converter.ToInt(smaArray[2])
            };
        }

        private StatusDetailInfo BuildStatusDetail(dynamic rawData, Status status)
        {
            StatusDetailInfo statusDetail = new StatusDetailInfo
            {
                Date = Converter.ToDateTime(rawData.date),
                Username = Converter.ToString2(rawData.username),
                Status = Converter.ToString2(rawData.status),
                Site = Converter.ToString2(rawData.site),
                Currency = Converter.ToString2(rawData.currency),
                Position = Converter.ToString2(rawData.position)
            };

            if (status == Status.NewCreated)
            {
                statusDetail.CreatedTime = Converter.ToDateTime(rawData.createddate);
                statusDetail.Creator = Converter.ToString2(rawData.creator);
            }

            return statusDetail;
        }
    }
}
