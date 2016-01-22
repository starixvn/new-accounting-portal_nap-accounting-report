Namespace.Register("Fanex.AccountingReports.Gaming.NumberGameStatisticByDaily");
Fanex.AccountingReports.Gaming.NumberGameStatisticByDaily = function () {
    var self = this;

    var _tables = {
        reportContainer: null,
        reportContent: null
    };

    var cachedReportParams = null;

    self.initPage = function () {
        cachedDOMElements();
    };

    var cachedDOMElements = function () {
        _tables.reportContainer = $("#reportContainer");
        _tables.reportContent = $("#reportContent");
    };

    self.getReport = function (reportParams) {
        reportManager.sortSetting.sortColName = null;
        reportManager.sortSetting.sortDirection = null;

        AccReport.disableInput();
        cachedReportParams = reportParams;
        AccReport.utility.CallAjax(this,
                                    window.rootUrl + "Gaming/NumberGameStatistic/GetReportByDaily",
                                    reportParams,
                                    renderReport,
                                    renderReport,
                                    AccReport.enableInput);
    };

    self.sortReport = function () {
        AccReport.disableInput();
        AccReport.utility.CallAjax(this,
                                    window.rootUrl + "Gaming/NumberGameStatistic/GetReportSortByDaily",
                                    cachedReportParams,
                                    renderReport,
                                    renderReport,
                                    AccReport.enableInput);
    };

    var renderReport = function (result) {
        _tables.reportContainer.removeClass("display-none");
        _tables.reportContent.addClass("display-none");
        var title = "Number Game Statistic Report From " + cachedReportParams.FromDate + " To " + cachedReportParams.ToDate;
        var columns = [
            { HeaderText: "Date", Width: 200, Name: "Date", HeaderAlign: "center", CellAlign: "center", Sort: true, SortDirection: reportManager.getSortDirection("Date") },
            { HeaderText: "No.Players", Width: 100, Name: "NumberPlayers", HeaderAlign: "center", CellAlign: "right", FormatNumber: FormatConstant.INT_NUMBER, Sort: true, SortDirection: reportManager.getSortDirection("Players") },
            { HeaderText: "Bet Count", Width: 100, Name: "BetCounts", HeaderAlign: "center", CellAlign: "right", FormatNumber: FormatConstant.INT_NUMBER, Sort: true, SortDirection: reportManager.getSortDirection("BetCount") },
            { HeaderText: "Total Bets", Width: 120, Name: "TotalBets", HeaderAlign: "center", CellAlign: "right", FormatNumber: FormatConstant.TWO_DECIMAL_NUMBER, Sort: true, SortDirection: reportManager.getSortDirection("TotalBets") },
            { HeaderText: "Total Wins", Width: 120, Name: "TotalWins", HeaderAlign: "center", CellAlign: "right", FormatNumber: FormatConstant.TWO_DECIMAL_NUMBER, FormatNegative: '$', Sort: true, SortDirection: reportManager.getSortDirection("TotalWins") },
            { HeaderText: "Game Income", Width: 120, Name: "GameIncome", HeaderAlign: "center", CellAlign: "right", FormatNumber: FormatConstant.TWO_DECIMAL_NUMBER, FormatNegative: '$', Sort: true, SortDirection: reportManager.getSortDirection("GameIncome") },
            { HeaderText: "Game Payout", Width: 120, Name: "GamePayout", HeaderAlign: "center", CellAlign: "right", FormatNumber: FormatConstant.TWO_DECIMAL_PERCENT_NUMBER, FormatNegative: '$', Sort: true, SortDirection: reportManager.getSortDirection("GamePayout") },
            { HeaderText: "Company Turnover", Width: 140, Name: "CompanyTurnover", HeaderAlign: "center", CellAlign: "right", FormatNumber: FormatConstant.TWO_DECIMAL_NUMBER, Sort: true, SortDirection: reportManager.getSortDirection("CompanyTurnover") },
            { HeaderText: "Company Net Income", Width: 160, Name: "CompanyNetIncome", HeaderAlign: "center", CellAlign: "right", FormatNumber: FormatConstant.TWO_DECIMAL_NUMBER, FormatNegative: '$', Sort: true, SortDirection: reportManager.getSortDirection("NetIncome") }
        ];

        var bodyData = [];
        var footerData = [];

        var reportLength = result.ReportRows.length;
        if (result && reportLength > 0) {
            for (var index = 0; index < reportLength; index++) {
                var currentItem = result.ReportRows[index];
                var currentDataRow = [];
                var gamePayout = currentItem.GamePayout;
                currentDataRow.push(
                    { Name: "Date", Value: currentItem.Date },
                    { Name: "NumberPlayers", Value: currentItem.Players },
                    { Name: "BetCounts", Value: currentItem.BetCount },
                    { Name: "TotalBets", Value: currentItem.TotalBets },
                    { Name: "TotalWins", Value: currentItem.TotalWins },
                    { Name: "GameIncome", Value: currentItem.GameIncome },
                    { Name: "GamePayout", Value: gamePayout, CssClass: gamePayout > 1 ? 'format-positive' : '' },
                    { Name: "CompanyTurnover", Value: currentItem.CompanyTurnover },
                    { Name: "CompanyNetIncome", Value: currentItem.NetIncome }
                );

                bodyData.push({
                    Columns: currentDataRow
                });
            }

            var grandTotalGamePayout = result.GrandTotal.GamePayout;
            footerData.push({
                Columns: [
                    { Name: "Date", Value: "Grand Total" },
                    { Name: "NumberPlayers", Value: result.GrandTotal.Players },
                    { Name: "BetCounts", Value: result.GrandTotal.BetCount },
                    { Name: "TotalBets", Value: result.GrandTotal.TotalBets },
                    { Name: "TotalWins", Value: result.GrandTotal.TotalWins },
                    { Name: "GameIncome", Value: result.GrandTotal.GameIncome, TdClass: "highlight-column" },
                    { Name: "GamePayout", Value: grandTotalGamePayout, CssClass: grandTotalGamePayout > 1 ? 'format-positive' : '' },
                    { Name: "CompanyTurnover", Value: result.GrandTotal.CompanyTurnover },
                    { Name: "CompanyNetIncome", Value: result.GrandTotal.NetIncome, TdClass: "highlight-column" }
                ]
            });
        }

        var gridOptions = {
            columns: columns,
            bodyRows: bodyData,
            footerRows: footerData,
            gridExpandHeight: 260,
            showDistinctRow: true,
            showHighlightRow: false,
            showTitle: true,
            showFooter: true,
            titleInfo: {
                text: title,
                showExcelButton: true,
                exportExcelFunction: function () {
                    exportReport();
                }
            },
            sortOption: {
                serverSortFunction: function (sortColumnName, sortDirection) {
                    var sortDataType = "number";

                    switch (sortColumnName) {
                        case "Date":
                            sortDataType = "date";
                            break;
                        case "NumberPlayers":
                            sortColumnName = "Players";
                            break;
                        case "BetCounts":
                            sortColumnName = "BetCount";
                            break;
                        case "CompanyNetIncome":
                            sortColumnName = "NetIncome";
                            break;
                    }

                    cachedReportParams.IsPagingCallBack = true;
                    cachedReportParams.SortColumnName = sortColumnName;
                    cachedReportParams.SortDirection = sortDirection;
                    cachedReportParams.SortDataType = sortDataType;

                    reportManager.sortSetting.sortColName = sortColumnName;
                    reportManager.sortSetting.sortDirection = sortDirection;

                    self.sortReport();
                }
            }
        };

        _tables.reportContent.removeClass("display-none").fanexGrid(gridOptions);
    };    

    var exportReport = function () {
        AccReport.downloadFile(window.rootUrl + "Gaming/NumberGameStatistic/ExportReportByDaily", cachedReportParams);
    };
}