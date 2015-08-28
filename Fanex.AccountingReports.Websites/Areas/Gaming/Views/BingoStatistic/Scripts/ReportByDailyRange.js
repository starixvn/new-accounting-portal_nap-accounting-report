ReportManager.prototype.GetGridOptions_ByDailyRange = function (report) {
    var self = this;
    var bodyData = [];
    var footerData = [];
    var timeRangeLength = report && report.ChildsInfo ?
        report.ChildsInfo.length :
        0;
    if (timeRangeLength > 0) {
        var info;
        var columns = [];
        var indexRange = self.GetPagingIndexRange(timeRangeLength);
        for (var i = indexRange.startIndex; i < indexRange.endIndex; i++) {
            var timeRangeWinLoss = report.ChildsInfo[i];
            var reportLength = timeRangeWinLoss.ChildsInfo.length;
            for (var j = 0; j < reportLength; j++) {
                columns = [];
                info = timeRangeWinLoss.ChildsInfo[j];
                if (j == 0) {
                    columns.push({ Name: "ColName", Value: timeRangeWinLoss.Name, MergeRowNumber: reportLength });
                }

                columns.push({ Name: "ColNoPlayers", Value: info.NoPlayers });
                columns.push({ Name: "ColCurrency", Value: info.CurrencyName });
                columns.push({ Name: "ColTotalBets", Value: info.TotalBets });
                columns.push({ Name: "ColTotalWins", Value: info.TotalWins });
                columns.push({ Name: "ColGameIncome", Value: info.GameIncome });
                columns.push({ Name: "ColGamePayout", Value: info.GamePayout });
                columns.push({ Name: "ColCompanyComm", Value: info.CompanyComm });
                columns.push({ Name: "ColCompanyNetIncome", Value: info.CompanyNetIncome });
                bodyData.push({
                    Columns: columns
                });
            }
        }

        var reportLength = report.GrandTotal.ChildsInfo.length;
        for (var i = 0; i < reportLength; i++) {
            info = report.GrandTotal.ChildsInfo[i];
            footerData.push({
                Columns: [{ Name: "ColName", Value: info.Name },
                { Name: "ColNoPlayers", Value: info.NoPlayers },
                { Name: "ColCurrency", Value: info.CurrencyName },
                { Name: "ColTotalBets", Value: info.TotalBets },
                { Name: "ColTotalWins", Value: info.TotalWins },
                { Name: "ColGameIncome", Value: info.GameIncome },
                { Name: "ColGamePayout", Value: info.GamePayout },
                { Name: "ColCompanyComm", Value: info.CompanyComm },
                { Name: "ColCompanyNetIncome", Value: info.CompanyNetIncome}]
            });
        }

        info = report.GrandTotal;
        footerData.push({
            Columns: [{ Name: "ColName", Value: info.Name },
                { Name: "ColNoPlayers", Value: info.NoPlayers },
                { Name: "ColCurrency", Value: info.CurrencyName },
                { Name: "ColTotalBets", Value: info.TotalBets },
                { Name: "ColTotalWins", Value: info.TotalWins },
                { Name: "ColGameIncome", Value: info.GameIncome },
                { Name: "ColGamePayout", Value: info.GamePayout },
                { Name: "ColCompanyComm", Value: info.CompanyComm },
                { Name: "ColCompanyNetIncome", Value: info.CompanyNetIncome}]
        });
    }

    var gridOptions = {
        columns: [{ HeaderText: viewResources.Date, Width: 160, Name: "ColName", HeaderAlign: "center", CellAlign: "center", Sort: true },
            { HeaderText: viewResources.No_Players, Width: 100, Name: "ColNoPlayers", HeaderAlign: "center", CellAlign: "right", FormatNumber: FormatConstant.INT_NUMBER, Sort: true },
            { HeaderText: viewResources.Currency, Width: 100, Name: "ColCurrency", HeaderAlign: "center", CellAlign: "center", Sort: true },
            { HeaderText: viewResources.Total_bets, Width: 120, Name: "ColTotalBets", HeaderAlign: "center", CellAlign: "right", FormatNumber: FormatConstant.TWO_DECIMAL_NUMBER, Sort: true },
            { HeaderText: viewResources.Total_wins, Width: 120, Name: "ColTotalWins", HeaderAlign: "center", CellAlign: "right", FormatNumber: FormatConstant.TWO_DECIMAL_NUMBER, Sort: true },
            { HeaderText: viewResources.Game_income, Width: 120, Name: "ColGameIncome", FormatNumber: FormatConstant.TWO_DECIMAL_NUMBER, HeaderAlign: "center", CellAlign: "right", FormatNegative: '$', Sort: true },
            { HeaderText: viewResources.Game_payout, Width: 120, Name: "ColGamePayout", FormatNumber: FormatConstant.TWO_DECIMAL_PERCENT_NUMBER, HeaderAlign: "center", CellAlign: "right", FormatNegative: '$', Sort: true },
            { HeaderText: viewResources.Company_comm, Width: 120, Name: "ColCompanyComm", FormatNumber: FormatConstant.TWO_DECIMAL_NUMBER, HeaderAlign: "center", CellAlign: "right", FormatNegative: '$', Sort: true },
            { HeaderText: viewResources.Company_net_income, Width: 160, Name: "ColCompanyNetIncome", FormatNumber: FormatConstant.TWO_DECIMAL_NUMBER, HeaderAlign: "center", CellAlign: "right", FormatNegative: '$', Sort: true}],
        showTitle: true,
        showFooter: true,
        titleInfo: {
            text: AccReport.utility.StringFormat(viewResources.BingoStatistic_ReportTitle,
                [self.Params.reportParams.FromDate, self.Params.reportParams.ToDate]),
            showExcelButton: false
        },
        bodyRows: bodyData,
        footerRows: footerData,
        numberOfFrozenColumn: 0,
        gridWidth: 1400,
        gridExpandHeight: 350,
        fullWidth: true,
        fullHeight: false,
        gridHeight: self.CachedData.maxGridHeight,
        showDistinctRow: false,
        showHighlightRow: false,
        headerColumnHeight: 29,
        showPager: true,
        customPager: true,
        sortOption:
        {
            customGridSort: function (currentBodyRows, columnName, currentDirection) {
                var sortInfo = self.GetGridOptions_ByDailyRange_MappingSortInfo(columnName,
                    currentDirection);
                var bodyRows = self.GetGridOptions_ByDailyRange_SortedAndPagingData(sortInfo);
                return bodyRows;
            }
        },
        pagerOption: {
            itemsPerPage: self.CachedData.itemsPerPage,
            currentPage: self.CachedData.currentPage,
            itemsPerPageArray: self.CachedData.itemsPerPageArray,
            totalItem: timeRangeLength,
            pagerSelectCallBack: function (page_index, itemsPerPage, jq) {
                self.CachedData.itemsPerPage = itemsPerPage;
                self.CachedData.currentPage = page_index;
                self.RenderReport(self.CachedData.currentReport);
            }
        }
    };

    return gridOptions;
};

ReportManager.prototype.GetGridOptions_ByDailyRange_MappingSortInfo = function (columnName, currentDirection) {
    var self = this;
    var sortDirectionEnums = self.Enums.SortDirection;
    var sortDataTypeEnums = self.Enums.SortDataType;
    var sortInfo = {
        sortFactor: 0,
        sortField: '',
        sortDataType: sortDataTypeEnums.String
    };

    switch (currentDirection) {
        case sortDirectionEnums.Asc:
            sortInfo.sortFactor = 1;
            break;
        case sortDirectionEnums.Desc:
            sortInfo.sortFactor = -1;
            break;
        default:
            sortInfo.sortFactor = 0;
            break;
    };

    switch (columnName) {
        case "ColName":
            sortInfo.sortField = "Id";
            sortInfo.sortDataType = sortDataTypeEnums.Date;
            break;
        case "ColNoPlayers":
            sortInfo.sortField = "NoPlayers";
            sortInfo.sortDataType = sortDataTypeEnums.Number;
            break;
        case "ColCurrency":
            sortInfo.sortField = "CurrencyName";
            sortInfo.sortDataType = sortDataTypeEnums.String;
            break;
        case "ColTotalBets":
            sortInfo.sortField = "TotalBets";
            sortInfo.sortDataType = sortDataTypeEnums.Number;
            break;
        case "ColTotalWins":
            sortInfo.sortField = "TotalWins";
            sortInfo.sortDataType = sortDataTypeEnums.Number;
            break;
        case "ColGameIncome":
            sortInfo.sortField = "GameIncome";
            sortInfo.sortDataType = sortDataTypeEnums.Number;
            break;
        case "ColGamePayout":
            sortInfo.sortField = "GamePayout";
            sortInfo.sortDataType = sortDataTypeEnums.Number;
            break;
        case "ColCompanyComm":
            sortInfo.sortField = "CompanyComm";
            sortInfo.sortDataType = sortDataTypeEnums.Number;
            break;
        case "ColCompanyNetIncome":
            sortInfo.sortField = "CompanyNetIncome";
            sortInfo.sortDataType = sortDataTypeEnums.Number;
            break;
    }

    return sortInfo;
};

ReportManager.prototype.GetGridOptions_ByDailyRange_SortedAndPagingData = function (sortInfo) {
    var self = this;
    self.CachedData.currentReport.ChildsInfo = self.SortReport_ByDailyRange(sortInfo, self.CachedData.currentReport);
    var report = self.CachedData.currentReport;

    var bodyData = [];
    var timeRangeLength = report && report.ChildsInfo ?
            report.ChildsInfo.length :
            0;
    if (timeRangeLength > 0) {
        var info;
        var columns = [];
        var indexRange = self.GetPagingIndexRange(timeRangeLength);
        for (var i = indexRange.startIndex; i < indexRange.endIndex; i++) {
            var timeRangeWinLoss = report.ChildsInfo[i];
            var reportLength = timeRangeWinLoss.ChildsInfo.length;
            for (var j = 0; j < reportLength; j++) {
                columns = [];
                info = timeRangeWinLoss.ChildsInfo[j];
                if (j == 0) {
                    columns.push({ Name: "ColName", Value: timeRangeWinLoss.Name, MergeRowNumber: reportLength });
                }

                columns.push({ Name: "ColNoPlayers", Value: info.NoPlayers });
                columns.push({ Name: "ColCurrency", Value: info.CurrencyName });
                columns.push({ Name: "ColTotalBets", Value: info.TotalBets });
                columns.push({ Name: "ColTotalWins", Value: info.TotalWins });
                columns.push({ Name: "ColGameIncome", Value: info.GameIncome });
                columns.push({ Name: "ColGamePayout", Value: info.GamePayout });
                columns.push({ Name: "ColCompanyComm", Value: info.CompanyComm });
                columns.push({ Name: "ColCompanyNetIncome", Value: info.CompanyNetIncome });
                bodyData.push({
                    Columns: columns
                });
            }
        }
    }

    return bodyData;
};

ReportManager.prototype.SortReport_ByDailyRange = function (sortInfo, report) {
    var self = this;
    var sortDataTypeEnums = self.Enums.SortDataType;
    if (sortInfo.sortField == "Id") {
        return report.ChildsInfo.sort(function (elementA, elementB) {
            var valueA = elementA[sortInfo.sortField];
            var valueB = elementB[sortInfo.sortField];
            var isGreaterThan = self.CompareValues(sortInfo,
                new Date(valueA),
                new Date(valueB));
            return isGreaterThan;
        });
    }

    var timeRangeLength = report && report.ChildsInfo ?
            report.ChildsInfo.length :
            0;
    if (timeRangeLength > 0) {
        for (var i = 0; i < timeRangeLength; i++) {
            var timeRangeWinLoss = report.ChildsInfo[i];
            report.ChildsInfo[i].ChildsInfo = timeRangeWinLoss.ChildsInfo.sort(function (elementA, elementB) {
                var valueA = elementA[sortInfo.sortField];
                var valueB = elementB[sortInfo.sortField];
                var isGreaterThan = self.CompareValues(sortInfo, valueA, valueB);
                return isGreaterThan;
            });
        }

        report.ChildsInfo = report.ChildsInfo.sort(function (elementA, elementB) {
            var valueA = elementA.ChildsInfo[0][sortInfo.sortField];
            var valueB = elementB.ChildsInfo[0][sortInfo.sortField];
            var isGreaterThan = self.CompareValues(sortInfo, valueA, valueB);
            return isGreaterThan;
        });
    }

    return report.ChildsInfo;
};
