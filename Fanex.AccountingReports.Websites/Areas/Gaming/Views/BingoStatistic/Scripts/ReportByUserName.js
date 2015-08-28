ReportManager.prototype.GetGridOptions_ByUserName = function (report) {
    var self = this;
    var bodyData = [];
    var footerData = [];
    if (report && report.ChildsInfo && report.ChildsInfo.length > 0) {
        var reportLength = report.ChildsInfo.length;
        var info;
        for (var i = 0; i < reportLength; i++) {
            info = report.ChildsInfo[i];
            bodyData.push({
                Columns: [{ Name: "ColName", Value: info.Name },
                    { Name: "ColCurrency", Value: info.CurrencyName },
                    { Name: "ColTotalBets", Value: info.TotalBets },
                    { Name: "ColTotalWins", Value: info.TotalWins },
                    { Name: "ColGameIncome", Value: info.GameIncome },
                    { Name: "ColGamePayout", Value: info.GamePayout },
                    { Name: "ColCompanyComm", Value: info.CompanyComm },
                    { Name: "ColCompanyNetIncome", Value: info.CompanyNetIncome}]
            });
        }

        var reportLength = report.GrandTotal.ChildsInfo.length;
        for (var i = 0; i < reportLength; i++) {
            info = report.GrandTotal.ChildsInfo[i];
            footerData.push({
                Columns: [{ Name: "ColName", Value: info.Name },
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
        columns: [{ HeaderText: viewResources.UserName, Width: 160, Name: "ColName", HeaderAlign: "center", CellAlign: "center", Sort: true },
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
        headerColumnHeight: 25,
        showPager: true,
        pagerOption: {
            itemsPerPage: self.CachedData.itemsPerPage,
            currentPage: 0,
            itemsPerPageArray: self.CachedData.itemsPerPageArray,
            totalItem: bodyData.length
        }
    };

    return gridOptions;
};