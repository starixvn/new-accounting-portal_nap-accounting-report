namespace Fanex.AccountingReports.Gaming.Services.Implement
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using Fanex.AccountingReports.Core.Extensions;
    using Fanex.AccountingReports.Core.Services.Impl;
    using Fanex.AccountingReports.Core.Web;
    using Fanex.AccountingReports.Gaming.Criteria.NumberOfActiveMember;
    using Fanex.AccountingReports.Gaming.Models.NumberOfActiveMember;
    using Fanex.AccountingReports.Gaming.Resources;
    using Fanex.Common;
    using Fanex.Excel.Enums;
    using Fanex.Excel.Models;

    public class NumberOfActiveMemberService : ServiceBase
    {
        private readonly string _numberOfActiveMemberSessionKey = "Fanex.AccountingReports.Gaming.NumberOfActiveMember." +
            (AppSession.CurrentUser != null ? AppSession.CurrentUser.Username : string.Empty);

        private readonly string _numberOfActiveMemberRawDataSessionKey = "Fanex.AccountingReports.Gaming.NumberOfActiveMemberRawData." +
            (AppSession.CurrentUser != null ? AppSession.CurrentUser.Username : string.Empty);

        private readonly string _numberOfActiveMemberProductsSessionKey = "Fanex.AccountingReports.Gaming.NumberOfActiveMemberProducts." +
          (AppSession.CurrentUser != null ? AppSession.CurrentUser.Username : string.Empty);

        public List<Product> GetProduct()
        {
            List<Product> products = new List<Product>();
            var criteria = new GetProductCriteria();
            products = this.GetReportData<Product>(criteria).ToList();
            SessionManager.Set<List<Product>>(this._numberOfActiveMemberProductsSessionKey, products);

            return products;
        }

        public List<NumberActiveMemeberByCombination> GetReportCombinationData(GetNumberOfActiveMemberCriteria criteria)
        {
            var numberActiveMemberByCombinations = InitNumberActiveMemberByCombinations(criteria);
            List<NumberOfActiveMember> report = GetReportData(criteria);

            if (report.Count > 0)
            {
                var combinationByUsers = report.GroupBy(x => x.CustId).ToList();

                foreach (var group in combinationByUsers)
                {
                    var groupData = group.ToList();
                    int numberItemInGroup = groupData.Count;
                    if (numberItemInGroup == 1)
                    {
                        var product = groupData.FirstOrDefault();
                        numberActiveMemberByCombinations[numberItemInGroup - 1]
                            .NumberActiveMemberByProduct
                            .Where(x => x.ProductId == Converter.ToString2(product.ProductId))
                            .ToList()
                            .FirstOrDefault()
                            .MemberCount += 1;
                    }
                    else
                    {
                        int numberActive = 0;
                        List<string> combinations = new List<string>();
                        groupData = groupData.OrderBy(x => Converter.ToInt(x.ProductId)).ToList();
                        foreach (var data in groupData)
                        {
                            combinations.Insert(0, Converter.ToString2(data.ProductId));
                            numberActive = data.MemberCount;
                        }

                        string combinationName = string.Join(";", combinations);
                        numberActiveMemberByCombinations[numberItemInGroup - 1]
                            .NumberActiveMemberByProduct
                            .Where(x => x.ProductId == combinationName)
                            .ToList()
                            .FirstOrDefault()
                            .MemberCount += numberActive;
                    }
                }
            }

            SessionManager.Set<List<NumberActiveMemeberByCombination>>(this._numberOfActiveMemberRawDataSessionKey, numberActiveMemberByCombinations);
            return numberActiveMemberByCombinations;
        }

        public GridOptions GetReport(GetNumberOfActiveMemberCriteria criteria)
        {
            List<NumberActiveMemeberByCombination> report = GetReportCombinationData(criteria);
            GridOptions gridOptions = GetNumberOfActiveMemberGridOptions(report, criteria);
            SessionManager.Set<GridOptions>(this._numberOfActiveMemberSessionKey, gridOptions);
            return gridOptions;
        }

        public GridOptions GetReportFromCache(GetNumberOfActiveMemberCriteria criteria)
        {
            var gridOptions = SessionManager.Get<GridOptions>(this._numberOfActiveMemberSessionKey);

            if (gridOptions == null)
            {
                gridOptions = GetReport(criteria);
            }

            return gridOptions;
        }

        public GridOptions SearchNumberOfActiveMemberByProduct(SearchNumberActiveMemberCriteria criteria)
        {
            List<NumberActiveMemeberByCombination> numberActiveMemeberByCombination = new List<NumberActiveMemeberByCombination>();
            var reportByComBinationGroups = SessionManager.Get<List<NumberActiveMemeberByCombination>>(this._numberOfActiveMemberRawDataSessionKey);
            var getNumberOfActiveMemberCriteria = new GetNumberOfActiveMemberCriteria()
            {
                FromDate = criteria.FromDate,
                ToDate = criteria.ToDate,
                ProductList = criteria.ProductList
            };

            if (reportByComBinationGroups == null)
            {
                reportByComBinationGroups = GetReportCombinationData(getNumberOfActiveMemberCriteria);
            }

            if (string.IsNullOrEmpty(criteria.ProductName))
            {
                numberActiveMemeberByCombination = reportByComBinationGroups;
            }
            else
            {
                foreach (var combination in reportByComBinationGroups)
                {
                    var combinationByProduct = combination.NumberActiveMemberByProduct.Where(s => s.GameName.ToLower().Contains(criteria.ProductName.ToLower())).ToList();
                    numberActiveMemeberByCombination.Add(new NumberActiveMemeberByCombination()
                    {
                        CombinationName = combination.CombinationName,
                        NumberActiveMemberByProduct = combinationByProduct
                    });
                }
            }

            GridOptions gridOptions = GetNumberOfActiveMemberGridOptions(numberActiveMemeberByCombination, getNumberOfActiveMemberCriteria);
            SessionManager.Set<GridOptions>(this._numberOfActiveMemberSessionKey, gridOptions);
            return gridOptions;
        }

        private List<NumberOfActiveMember> GetReportData(GetNumberOfActiveMemberCriteria criteria)
        {
            List<NumberOfActiveMember> report = new List<NumberOfActiveMember>();
            criteria.BalanceUpDay = ReportService.BalanceUp.BalanceUpDay;
            report = (List<NumberOfActiveMember>)GetReportData<NumberOfActiveMember>(criteria);
            return report;
        }

        private GridOptions InitNumberOfActiveMemberGridOptions(GetNumberOfActiveMemberCriteria criteria)
        {
            return new GridOptions
            {
                Columns = CreateReportHeader(),
                ShowTitle = true,
                ShowHighlightRow = false,
                TitleType = TitleType.TwoLineBlue,
                TitleInfoText = string.Format(
                "Number of Active Members from {0} to {1}",
                criteria.FromDate.ToShortFormatedDate(),
                criteria.ToDate.ToShortFormatedDate()),
                ShowFooter = true
            };
        }

        
        private List<HeaderColumns> CreateReportHeader()
        {
            var header = new List<HeaderColumns>();
            header.Add(new HeaderColumns() { HeaderText = Labels.Combinations, Name = Labels.Combinations, Width = 450, ShowCollapseExpand = true, CellAlignment = HAlignment.Left });
            header.Add(new HeaderColumns() { HeaderText = Labels.Number_Of_Active_Members, Name = Labels.Number_Of_Active_Members, Width = 200, CustomCellType = ExcelCellType.Integer });
            return header;
        }

        private GridOptions GetNumberOfActiveMemberGridOptions(List<NumberActiveMemeberByCombination> report, GetNumberOfActiveMemberCriteria criteria)
        {
            var gridOptions = InitNumberOfActiveMemberGridOptions(criteria);
            if (report == null || report.Count == 0)
            {
                return gridOptions;
            }

            var numberCombinations = report.Count;
            int totalActiveMember = 0;
            for (int i = 0; i < numberCombinations; i++)
            {
                var combinationGroup = report[i];
                bool zeroMemberCountInCombinationGroup = CheckZeroMemberCountInCombinationGroup(combinationGroup);
                if (!zeroMemberCountInCombinationGroup)
                {
                    var combinationGroupDataRow = CreateCombinationParentRow(combinationGroup);
                    foreach (var combinationItem in combinationGroup.NumberActiveMemberByProduct)
                    {
                        if (combinationItem.MemberCount > 0)
                        {
                            var combinationDataRow = BindingCombinationDataRow(combinationItem);
                            combinationGroupDataRow.SubRows.Add(combinationDataRow);
                            totalActiveMember += combinationItem.MemberCount;
                        }
                    }

                    gridOptions.BodyRows.Add(combinationGroupDataRow);
                }
            }

            var totalActiveMemberDataRow = CreateTotalMemberActiveDataRow(totalActiveMember);
            gridOptions.FooterRows.Add(totalActiveMemberDataRow);
            return gridOptions;
        }

        private GridDataRow CreateCombinationParentRow(NumberActiveMemeberByCombination combinationGroup)
        {
            GridDataRow dataRow = new GridDataRow();
            var totalActiveMember = GetTotalMemberActiveInGroup(combinationGroup);
            dataRow.CreateColumns(new Columns(0, Labels.Combinations, combinationGroup.CombinationName, HAlignment.Left));
            dataRow.CreateColumns(new Columns(1, Labels.Number_Of_Active_Members, totalActiveMember, HAlignment.Right));
            return dataRow;
        }

        private GridDataRow BindingCombinationDataRow(NumberOfActiveMember activeMember)
        {
            GridDataRow dataRow = new GridDataRow();
            int columnIndex = 0;
            dataRow.CreateColumns(new Columns(columnIndex++, Labels.Combinations, activeMember.GameName, HAlignment.Left));
            dataRow.CreateColumns(new Columns(columnIndex++, Labels.Number_Of_Active_Members, activeMember.MemberCount));
            return dataRow;
        }

        private GridDataRow CreateTotalMemberActiveDataRow(int totalActiveMember)
        {
            GridDataRow gridDataRow = new GridDataRow();
            gridDataRow.CreateColumns(new Columns(0) { Name = Labels.Combinations, Value = Labels.Total_Active_Members, CellAlignment = HAlignment.Left });
            gridDataRow.CreateColumns(new Columns(1) { Name = Labels.Number_Of_Active_Members, Value = totalActiveMember, CellAlignment = HAlignment.Right });
            return gridDataRow;
        }

        private List<NumberActiveMemeberByCombination> InitNumberActiveMemberByCombinations(GetNumberOfActiveMemberCriteria criteria)
        {
            List<string> products = criteria.ProductList.Split(',').ToList();
            var combinationGroups = InitCombinationGroups(products);
            var combinations = GetCombinations(products);
            foreach (var combination in combinations)
            {
                if (!combination.ProductId.Contains(";"))
                {
                    combinationGroups[0].NumberActiveMemberByProduct.Add(new NumberOfActiveMember()
                    {
                        GameName = combination.ProductName,
                        ProductId = combination.ProductId,
                        MemberCount = 0
                    });
                }
                else
                {
                    int numberCombination = combination.ProductId.Split(';').Length;
                    combinationGroups[numberCombination - 1].NumberActiveMemberByProduct.Add(new NumberOfActiveMember()
                    {
                        GameName = combination.ProductName,
                        ProductId = combination.ProductId,
                        MemberCount = 0
                    });
                }
            }

            return combinationGroups;
        }

        private List<NumberActiveMemeberByCombination> InitCombinationGroups(List<string> products)
        {
            List<NumberActiveMemeberByCombination> numberActiveMemberByCombinations = new List<NumberActiveMemeberByCombination>();
            for (int i = 0; i < products.Count; i++)
            {
                var combinationGroup = new NumberActiveMemeberByCombination()
                {
                    CombinationName = string.Format("{0} Combinations", i + 1)
                };

                numberActiveMemberByCombinations.Add(combinationGroup);
            }

            return numberActiveMemberByCombinations;
        }

        private List<ProductCombination> GetCombinations(List<string> products)
        {
            List<Product> cachedProductData = SessionManager.Get<List<Product>>(_numberOfActiveMemberProductsSessionKey);
            List<ProductCombination> results = new List<ProductCombination>();
            for (int i = 0; i < products.Count; i++)
            {
                int resultsLength = results.Count;
                for (int j = 0; j < resultsLength; j++)
                {
                    string productId = string.Format("{0};{1}", products[i], results[j].ProductId);
                    string productName = string.Format(
                        "{0} - {1}",
                        cachedProductData.Where(x => x.ProductId == Converter.ToInt(products[i])).FirstOrDefault().ProductName,
                        results[j].ProductName);
                    results.Add(new ProductCombination()
                    {
                        ProductId = productId,
                        ProductName = productName
                    });
                }

                results.Add(new ProductCombination()
                {
                    ProductId = products[i],
                    ProductName = cachedProductData.Where(x => x.ProductId == Converter.ToInt(products[i])).FirstOrDefault().ProductName
                });
            }

            return results;
        }

        private bool CheckZeroMemberCountInCombinationGroup(NumberActiveMemeberByCombination combination)
        {
            bool isZeroMemberCount = true;
            foreach (var item in combination.NumberActiveMemberByProduct)
            {
                if (item.MemberCount > 0)
                {
                    isZeroMemberCount = false;
                }
            }

            return isZeroMemberCount;
        }

        private int GetTotalMemberActiveInGroup(NumberActiveMemeberByCombination combination)
        {
            int totalMemberActive = 0;
            foreach (var item in combination.NumberActiveMemberByProduct)
            {
                totalMemberActive += item.MemberCount;
            }

            return totalMemberActive;
        }
    }
}
