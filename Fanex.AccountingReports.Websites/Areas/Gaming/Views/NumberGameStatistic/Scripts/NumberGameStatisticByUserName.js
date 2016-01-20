Namespace.Register("Fanex.AccountingReports.Gaming.NumberGameStatisticByUserName");
Fanex.AccountingReports.Gaming.NumberGameStatisticByUserName = function () {
    var self = this;

    var _tables = {
        reportContainer: null,
        reportContent: null
    };

    var cachedReportParams = null;

    var cachedPagingInfo = {
        currentPage: 0,
        itemsPerPage: 50,
        itemsPerPageArray: [50, 100, 200, 300, 500]
    };

    self.initPage = function () {
        cachedDOMElements();
    };

    var cachedDOMElements = function () {
        _tables.reportContainer = $("#reportContainer");
        _tables.reportContent = $("#reportContent");
    };

    self.getReport = function (reportParams) {
        AccReport.disableInput();
        cachedReportParams = reportParams;
        AccReport.utility.CallAjax(this,
                                    window.rootUrl + "Gaming/NumberGameStatistic/GetReportByUserName",
                                    reportParams,
                                    renderReport,
                                    renderReport,
                                    AccReport.enableInput);
    };

    var renderReport = function (result) {
        _tables.reportContainer.removeClass("display-none");
        _tables.reportContent.addClass("display-none");
        var title = "Number Game Statistic Report From " + cachedReportParams.FromDate + " To " + cachedReportParams.ToDate;
        var columns = [
            { HeaderText: "Username", Width: 200, Name: "Username", HeaderAlign: "center", CellAlign: "center" },
            { HeaderText: "Player ID", Width: 100, Name: "PlayerId", HeaderAlign: "center", CellAlign: "right" },
            { HeaderText: "Bet Count", Width: 100, Name: "BetCounts", HeaderAlign: "center", CellAlign: "right", FormatNumber: FormatConstant.INT_NUMBER },
            { HeaderText: "Total Bets", Width: 120, Name: "TotalBets", HeaderAlign: "center", CellAlign: "right", FormatNumber: FormatConstant.TWO_DECIMAL_NUMBER },
            { HeaderText: "Total Wins", Width: 120, Name: "TotalWins", HeaderAlign: "center", CellAlign: "right", FormatNumber: FormatConstant.TWO_DECIMAL_NUMBER, FormatNegative: '$' },
            { HeaderText: "Game Income", Width: 120, Name: "GameIncome", HeaderAlign: "center", CellAlign: "right", FormatNumber: FormatConstant.TWO_DECIMAL_NUMBER, FormatNegative: '$' },
            { HeaderText: "Game Payout", Width: 120, Name: "GamePayout", HeaderAlign: "center", CellAlign: "right", FormatNumber: FormatConstant.TWO_DECIMAL_PERCENT_NUMBER, FormatNegative: '$' },
            { HeaderText: "Company Turnover", Width: 120, Name: "CompanyTurnover", HeaderAlign: "center", CellAlign: "right", FormatNumber: FormatConstant.TWO_DECIMAL_NUMBER },
            { HeaderText: "Company Net Income", Width: 150, Name: "CompanyNetIncome", HeaderAlign: "center", CellAlign: "right", FormatNumber: FormatConstant.TWO_DECIMAL_NUMBER, FormatNegative: '$' }
        ];

        var bodyData = [];
        var footerData = [];

        var reportLength = result.ReportRows.length;
        if (result && reportLength > 0) {
            for (var index = 0; index < reportLength; index++) {
                var currentItem = result.ReportRows[index];
                var currentDataRow = [];
                var gamePayout = currentItem.NumberGameStatisticDetail.GamePayout;
                currentDataRow.push(
                    { Name: "Username", Value: currentItem.UserName },
                    { Name: "PlayerId", Value: currentItem.CustId },
                    { Name: "BetCounts", Value: currentItem.NumberGameStatisticDetail.BetCount },
                    { Name: "TotalBets", Value: currentItem.NumberGameStatisticDetail.TotalBets },
                    { Name: "TotalWins", Value: currentItem.NumberGameStatisticDetail.TotalWins },
                    { Name: "GameIncome", Value: currentItem.NumberGameStatisticDetail.GameIncome },
                    { Name: "GamePayout", Value: gamePayout, CssClass: gamePayout > 1 ? 'format-positive' : '' },
                    { Name: "CompanyTurnover", Value: currentItem.NumberGameStatisticDetail.CompanyTurnover },
                    { Name: "CompanyNetIncome", Value: currentItem.NumberGameStatisticDetail.NetIncome }
                );

                bodyData.push({
                    Columns: currentDataRow
                });
            }

            var grandTotalGamePayout = result.GrandTotal.NumberGameStatisticDetail.GamePayout;
            footerData.push({
                Columns: [
                    { Name: "Username", Value: "Grand Total" },
                    { Name: "PlayerId", Value: "" },
                    { Name: "BetCounts", Value: result.GrandTotal.NumberGameStatisticDetail.BetCount },
                    { Name: "TotalBets", Value: result.GrandTotal.NumberGameStatisticDetail.TotalBets },
                    { Name: "TotalWins", Value: result.GrandTotal.NumberGameStatisticDetail.TotalWins },
                    { Name: "GameIncome", Value: result.GrandTotal.NumberGameStatisticDetail.GameIncome, TdClass: "highlight-column" },
                    { Name: "GamePayout", Value: grandTotalGamePayout, CssClass: grandTotalGamePayout > 1 ? 'format-positive' : '' },
                    { Name: "CompanyTurnover", Value: result.GrandTotal.NumberGameStatisticDetail.CompanyTurnover },
                    { Name: "CompanyNetIncome", Value: result.GrandTotal.NumberGameStatisticDetail.NetIncome, TdClass: "highlight-column" }
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

            showPager: true,
            customPager: true,
            pagerOption: {
                itemsPerPage: cachedReportParams.PageSize,
                currentPage: cachedReportParams.PageIndex,
                itemsPerPageArray: cachedPagingInfo.itemsPerPageArray,
                totalItem: result.TotalItems,
                pagerSelectCallBack: function (page_index, itemsPerPage, jq) {
                    cachedPagingInfo.itemsPerPage = itemsPerPage;
                    cachedPagingInfo.currentPage = page_index;
                    cachedReportParams.PageSize = itemsPerPage;
                    cachedReportParams.PageIndex = page_index;
                    cachedReportParams.IsPagingCallBack = true;
                    self.getReport(cachedReportParams);
                }
            }

        };

        _tables.reportContent.removeClass("display-none").fanexGrid(gridOptions);
    };

    var exportReport = function () {
        AccReport.downloadFile(window.rootUrl + "Gaming/NumberGameStatistic/ExportReportByUserName", cachedReportParams);
    };
}