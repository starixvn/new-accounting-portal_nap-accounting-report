namespace Fanex.AccountingReports.Misc.Services.Implement.CreditMovement
{
    using System.Collections.Generic;
    using System.Data;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using Fanex.AccountingReports.Core.Extensions;
    using Fanex.AccountingReports.Core.Web;
    using Fanex.AccountingReports.Misc.Criteria.CreditMovement;
    using Fanex.AccountingReports.Misc.Models.CreditMovement;
    using Fanex.AccountingReports.Misc.Resources;
    using Fanex.Common;
    using Fanex.Data;
    using Fanex.Excel.Enums;
    using Fanex.Excel.Models;

    public class CreditMovementService
    {
        private const int ExtendCachedTimeOut = 2;

        private readonly string _creditMovementDataCachedKey = "Fanex.AccountingReports.Misc.CreditMovement" +
            (AppSession.CurrentUser != null ? AppSession.CurrentUser.Username : string.Empty);

        public GridOptions GetReport(GetCreditMovementCriteria criteria)
        {
            GridOptions gridOptions = null;
            GridOptions pagingGridOptions = InitCreditMovementGridOptions(criteria);

            if (criteria.IsPagingCallBack)
            {
                gridOptions = GetFullReportFromCache(criteria);
                pagingGridOptions.TotalItems = gridOptions.BodyRows.Count;
                GetPagingReport(gridOptions, ref pagingGridOptions, criteria);
                return pagingGridOptions;
            }

            gridOptions = GetReportData(criteria);
            pagingGridOptions.TotalItems = gridOptions.BodyRows.Count;
            GetPagingReport(gridOptions, ref pagingGridOptions, criteria);
            return pagingGridOptions;
        }

        public GridOptions GetFullReportFromCache(GetCreditMovementCriteria criteria)
        {
            GridOptions gridOptions = null;
            gridOptions = CacheExtension.Get<GridOptions>(_creditMovementDataCachedKey);

            if (gridOptions == null)
            {
                criteria.IsPagingCallBack = false;
                gridOptions = GetReportData(criteria);
            }

            return gridOptions;
        }

        public string GetReportName(GetCreditMovementCriteria criteria)
        {
            return string.Format(
                MiscResources.CreditMovement_ReportName,
                criteria.FromDate.ToShortFormatedDateWithSlash().Replace("/", "_"),
                criteria.ToDate.ToShortFormatedDateWithSlash().Replace("/", "_"));
        }

        public IEnumerable<SiteGroup> GetSites()
        {
            List<Site> sites = new List<Site>();
            using (var objectDb = ObjectDbFactory.CreateInstance("Misc_CreditMovement_GetSites"))
            {
                IDataReader reader = objectDb.ExecuteReader();
                while (reader.Read())
                {
                    sites.Add(new Site { SiteId = Converter.ToInt(reader["SiteId"]), SiteName = Converter.ToString2(reader["SiteName"]),
                        SiteGroup = Converter.ToString2(reader["SiteGroup"]), SiteLabel = Converter.ToString2(reader["SiteLabel"])
                    });
                }

                reader.Close();
            }
            var siteGroups = GetSiteGroup(sites);
            return siteGroups;
        }

        public IEnumerable<SiteGroup> GetSiteGroup(List<Site> list)
        {
            var result = new List<SiteGroup>();
            var groups = list.GroupBy(x => x.SiteGroup).Select(x => x.Key);
            foreach (var group in groups)
            {
                var siteGroup = new SiteGroup
                {
                    GroupName = group,
                    Sites = new List<Site>()
                };
                var sites = list.Where(x => x.SiteGroup == group);

                foreach (var site in sites)
                {
                    siteGroup.Sites.Add(new Site
                    {
                        SiteGroup = site.SiteGroup,
                        SiteName = site.SiteName,
                        SiteLabel = site.SiteLabel,
                        SiteId = site.SiteId
                    });
                }

                result.Add(siteGroup);
            }

            return result;
        }

        private GridOptions GetReportData(GetCreditMovementCriteria criteria)
        {
            GridOptions gridOptions = InitCreditMovementGridOptions(criteria);
            using (var objectDb = ObjectDbFactory.CreateInstance(criteria.GetSettingKey()))
            {
                IDataReader reader = objectDb.ExecuteReader(criteria);
                gridOptions.BodyRows = MapReportDataRows(reader);
                reader.Close();
            }

            CacheExtension.Set(_creditMovementDataCachedKey, gridOptions);
            return gridOptions;
        }

        private GridOptions InitCreditMovementGridOptions(GetCreditMovementCriteria criteria)
        {
            return new GridOptions
            {
                GroupHeaderRows = GroupHeaderRows(),
                CustomizeHeader = true,
                Columns = HeaderColumns(),
                ShowFooter = false,
                ShowDistinctRow = false,
                ShowHighlightRow = false,
                ShowTitle = true,
                TitleType = TitleType.TwoLineBlue,
                TitleInfoText = string.Format(
                MiscResources.CreditMovement_ReportTitle,
                criteria.Amount.ToString("0,0"))
            };
        }

        
        private List<GroupHeaderRows> GroupHeaderRows()
        {
            var groupHeader = new List<GroupHeaderRows>();
            var groupHeaderRow = new GroupHeaderRows();
            groupHeaderRow.Columns.AddRange(new List<HeaderColumns>()
            {
                new HeaderColumns() { HeaderText = "#", Name = Labels.No, MergeRowNumber = 2, Width = 50 },
                new HeaderColumns() { HeaderText = Labels.Date, Name = Labels.Date, MergeRowNumber = 2, Width = 100 },
                new HeaderColumns() { HeaderText = Labels.Currency, Name = Labels.Currency, MergeRowNumber = 2, Width = 100 },
                new HeaderColumns() { HeaderText = Labels.Site, Name = Labels.Site, MergeRowNumber = 2, Width = 100 },
                new HeaderColumns() { HeaderText = Labels.Super_Master_Agent_Member, Name = Labels.Super_Master_Agent_Member, MergeRowNumber = 2, Width = 300 },
                new HeaderColumns() { HeaderText = Labels.Credit_Movement, Name = Labels.Original_Currency, MergeColNumber = 2 }
            });
            groupHeader.Add(groupHeaderRow);
            return groupHeader;
        }

        
        private List<HeaderColumns> HeaderColumns()
        {
            var header = new List<HeaderColumns>();
            header.AddRange(new List<HeaderColumns>()
            {
                new HeaderColumns() { HeaderText = "#", Name = Labels.No, Width = 50, CustomCellType = ExcelCellType.Integer, CellAlignment = HAlignment.Center },
                new HeaderColumns() { HeaderText = Labels.Date, Name = Labels.Date, Width = 100, CellAlignment = HAlignment.Center },
                new HeaderColumns() { HeaderText = Labels.Currency, Name = Labels.Currency, Width = 100, CellAlignment = HAlignment.Center },
                new HeaderColumns() { HeaderText = Labels.Site, Name = Labels.Site, Width = 100, CellAlignment = HAlignment.Center },
                new HeaderColumns() { HeaderText = Labels.Super_Master_Agent_Member, Name = Labels.Super_Master_Agent_Member, Width = 300, CellAlignment = HAlignment.Center },
                new HeaderColumns() { HeaderText = Labels.Original_Currency, Name = Labels.Original_Currency, Width = 150, CustomCellType = ExcelCellType.Double, CellAlignment = HAlignment.Right },
                new HeaderColumns() { HeaderText = Labels.Base_Currency, Name = Labels.Base_Currency, Width = 150, CustomCellType = ExcelCellType.Double, CellAlignment = HAlignment.Right },
            });
            return header;
        }

        private List<GridDataRow> MapReportDataRows(IDataReader reader)
        {
            int dataRowIndex = 0;
            int dataColIndex = 0;
            List<GridDataRow> dataRows = new List<GridDataRow>();
            while (reader.Read())
            {
                string downLineDetail = string.Empty;
                string superName = Converter.ToString2(reader["SuperName"]);
                downLineDetail = string.IsNullOrEmpty(superName) ? downLineDetail : superName;
                string masterName = Converter.ToString2(reader["MasterName"]);
                downLineDetail = string.IsNullOrEmpty(masterName) ? downLineDetail : string.Format("{0} / {1}", downLineDetail, masterName);
                string agentName = Converter.ToString2(reader["AgentName"]);
                downLineDetail = string.IsNullOrEmpty(agentName) ? downLineDetail : string.Format("{0} / {1}", downLineDetail, agentName);
                string memberName = Converter.ToString2(reader["CustomerName"]);
                downLineDetail = string.IsNullOrEmpty(memberName) ? downLineDetail : string.Format("{0} / {1}", downLineDetail, memberName);
                GridDataRow dataRow = new GridDataRow();
                dataRow.Columns.AddRange(new List<Columns>()
                {
                    new Columns(dataColIndex++) { Name = Labels.No, Value = dataRowIndex + 1 },
                    new Columns(dataColIndex++) { Name = Labels.Date, Value = Converter.ToDateTime(reader["TransDate"]).ToShortFormatedDate() },
                    new Columns(dataColIndex++) { Name = Labels.Currency, Value = Converter.ToString2(reader["Currency"]) },
                    new Columns(dataColIndex++) { Name = Labels.Site, Value = Converter.ToString2(reader["Site"]) },
                    new Columns(dataColIndex++) { Name = Labels.Super_Master_Agent_Member, Value = downLineDetail },
                    new Columns(dataColIndex++) { Name = Labels.Original_Currency, Value = Converter.ToDecimal(reader["Amount"]) },
                    new Columns(dataColIndex++) { Name = Labels.Base_Currency, Value = Converter.ToDecimal(reader["BaseAmount"]) }
                });

                dataRows.Add(dataRow);
                dataRowIndex++;
            }

            return dataRows;
        }

        private void GetPagingReport(GridOptions gridOptions, ref GridOptions pagingGridOptions, GetCreditMovementCriteria criteria)
        {
            int pageSize = criteria.PageSize;
            int pageIndex = criteria.PageIndex;
            pagingGridOptions.BodyRows = gridOptions.BodyRows.Skip(pageIndex * pageSize).Take(pageSize).ToList();
        }
    }
}
