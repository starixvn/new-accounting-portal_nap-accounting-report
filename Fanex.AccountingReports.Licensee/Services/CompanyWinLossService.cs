namespace Fanex.AccountingReports.Licensee.Services.Implement
{
    using System.Collections.Generic;
    using System.Data;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using Fanex.AccountingReports.Core.Extensions;
    using Fanex.AccountingReports.Core.Models;
    using Fanex.AccountingReports.Core.Services.Impl;
    using Fanex.AccountingReports.Core.Web;
    using Fanex.AccountingReports.Licensee.Criteria.CompanyWinLoss;
    using Fanex.AccountingReports.Licensee.Models;
    using Fanex.AccountingReports.Licensee.Repositories.Implement;
    using Fanex.AccountingReports.Licensee.Resources;
    using Fanex.Common;
    using Fanex.Data.Repository;
    using Fanex.Excel.Enums;
    using Fanex.Excel.Models;
    using AccountingReports.Core.Configurations;

    public class CompanyWinLossService : ServiceBase
    {
        private const int ExtendCachedTimeOut = 2;

        private readonly string _dataCachedKey = "Fanex.AccountingReports.Misc.CompanyWinLoss."
            + (AppSession.CurrentUser != null ? AppSession.CurrentUser.Username : string.Empty);

        private CompanyWinLossRepository _repository;

        public CompanyWinLossService(CompanyWinLossRepository repository)
        {
            _repository = repository;
        }

        public CompanyWinLossService()
        {
            _repository = _repository ?? new CompanyWinLossRepository();
        }

        public List<Currency> GetCurrenciesByLicensee(ICriteria criteria)
        {
            var currencies = new List<Currency>();
            var reader = _repository.GetDataFromDB(criteria);
            while (reader.Read())
            {
                currencies.Add(new Currency
                {
                    CurrencyId = Converter.To<int>(reader["currency"]),
                    CurrencyName = Converter.To<string>(reader["CurrencyName"])
                });
            }

            return currencies;
        }

        public List<Product> GetProductListByLicense(ICriteria criteria)
        {
            List<Product> products = new List<Product>();
            using (IDataReader reader = _repository.GetDataFromDB(criteria))
            {
                while (reader.Read())
                {
                    products.Add(new Product()
                    {
                        ProductPrefix = Converter.ToString2(reader["ProductPrefix"]),
                        ProductName = Converter.ToString2(reader["ProductName"])
                    });
                }
            }

            return products;
        }

        public GridOptions GetCompanyWinLoss(CompanyWinLossCriteria criteria)
        {
            GridOptions gridOptions = null;
            GridOptions pagingGridOptions = InitGridOptions(criteria);

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

        private void GetPagingReport(GridOptions gridOptions, ref GridOptions pagingGridOptions, CompanyWinLossCriteria criteria)
        {
            int pageSize = criteria.PageSize;
            int pageIndex = criteria.PageIndex;
            pagingGridOptions.BodyRows = gridOptions.BodyRows.Skip(pageIndex * pageSize).Take(pageSize).ToList();
        }

        private GridOptions GetReportData(CompanyWinLossCriteria criteria)
        {
            criteria.MoveBetsDay = ReportService.BalanceUp.MoveBetsDay;
            GridOptions gridOptions = InitGridOptions(criteria);

            using (IDataReader reader = _repository.GetDataFromDB(criteria))
            {
                gridOptions.BodyRows = MapReportDataRows(reader);
            }

            CacheExtension.Set(_dataCachedKey, gridOptions);

            return gridOptions;
        }

        public GridOptions GetFullReportFromCache(CompanyWinLossCriteria criteria)
        {
            GridOptions gridOptions = null;
            gridOptions = CacheExtension.Get<GridOptions>(_dataCachedKey);

            if (gridOptions == null)
            {
                criteria.IsPagingCallBack = false;
                gridOptions = GetCompanyWinLoss(criteria);
            }

            return gridOptions;
        }

        private GridOptions InitGridOptions(CompanyWinLossCriteria criteria)
        {
            return new GridOptions
            {
                CustomizeHeader = true,
                Columns = CreateHeaderColumns(),
                ShowFooter = false,
                ShowDistinctRow = false,
                ShowHighlightRow = false,
                ShowTitle = true,
                TitleType = TitleType.TwoLineBlue,
                TitleInfoText = Labels.Company_Win_Loss + " from " + criteria.FromDate.ToShortFormatedDate() + " to " + criteria.ToDate.ToShortFormatedDate()
            };
        }

        private List<GroupHeaderRows> CreateGroupHeaderRows()
        {
            var groupHeader = new List<GroupHeaderRows>();
            var groupHeaderRow = new GroupHeaderRows();
            groupHeaderRow.Columns.AddRange(new List<HeaderColumns>()
            {
                new HeaderColumns() { HeaderText = "#", Name = Labels.RowId, Width = 50 },
                new HeaderColumns() { HeaderText = Labels.Super, Name = Labels.UserName, Width = 250 },
                new HeaderColumns() { HeaderText = Labels.Company_Win_Loss, Name = Labels.CompanyWL, Width = 250 },
            });
            groupHeader.Add(groupHeaderRow);

            return groupHeader;
        }

        [SuppressMessage("StyleCop.CSharp.StylecopPlus.Rules",
           "SP2100:CodeLineMustNotBeLongerThan",
           Justification = "Reviewed. Suppression is OK here.")]
        private List<HeaderColumns> CreateHeaderColumns()
        {
            var header = new List<HeaderColumns>();
            header.AddRange(new List<HeaderColumns>()
            {
                new HeaderColumns() { HeaderText = "#", Name = Labels.RowId, Width = 50, CustomCellType = ExcelCellType.Integer, CellAlignment = HAlignment.Center },
                new HeaderColumns() { HeaderText = Labels.Super, Name = Labels.UserName, Width = 250, CellAlignment = HAlignment.Left },
                new HeaderColumns() { HeaderText = Labels.Company_Win_Loss, Name = Labels.CompanyWL, Width = 250, CustomCellType = ExcelCellType.DoubleRedNegative, CellAlignment = HAlignment.Right },
            });

            return header;
        }

        private List<GridDataRow> MapReportDataRows(IDataReader reader)
        {
            int dataColIndex = 0;
            List<GridDataRow> dataRows = new List<GridDataRow>();
            while (reader.Read())
            {
                GridDataRow dataRow = new GridDataRow();
                dataRow.Columns.AddRange(new List<Columns>()
                {
                    new Columns(dataColIndex++) { Name = Labels.RowId, Value = Converter.To<int>(reader["RowId"]) },
                    new Columns(dataColIndex++) { Name = Labels.UserName, Value = Converter.To<string>(reader["UserName"]) },
                    new Columns(dataColIndex++) { Name = Labels.CompanyWL, Value = Converter.To<double>(reader["CompanyWL"]) },
                });

                dataRows.Add(dataRow);
            }

            return dataRows;
        }

        public string GetReportName(CompanyWinLossCriteria criteria)
        {
            string reportName = string.Format(string.Format(Labels.Company_Win_Loss.Replace(" ", string.Empty))
                + "_From_" + string.Format(criteria.FromDate.ToShortFormatedDateWithSlash().Replace("/", "_"))
                + "_To_" + string.Format(criteria.ToDate.ToShortFormatedDateWithSlash().Replace("/", "_")));

            return reportName;
        }
    }
}
