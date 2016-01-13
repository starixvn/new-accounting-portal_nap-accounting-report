Namespace.Register("Fanex.AccountingReports.Gaming.LiveCasinoGameStatistic");
Fanex.AccountingReports.Gaming.LiveCasinoGameStatistic.ReportByGrandTotal = function () {
    var self = this;

    var _tables = {
        reportContainer: null,
        report: null
    };

    self.initPage = function () {
        cachedDOMElements();
        self.renderReport();
    };

    var cachedDOMElements = function () {
        _tables.reportParentContainer = $("#parentContainer");
        _tables.reportContainer = $('#grandTotalContainer');
        _tables.report = $('#grandTotal');

    };

    self.renderReport = function () {
        AccReport.disableInput();
        var selectParam = reportManager.Params.globalVariable.CacheParam.Params;
        selectParam.IsExportGrandTotal = false;
        selectParam.ViewReport = reportManager.Params.globalVariable.viewReport.GrandTotal;
        selectParam.IsGrandRM = true;

        reportManager.Params.globalVariable.CacheParam.GrandTotal = selectParam;

        AccReport.utility.CallAjax(this,
            window.rootUrl + "Gaming/CasinoGameStatistic/GetReportByGrandTotal",
            selectParam,
            self.RenderGridOptions,
            self.RenderGridOptions,
            AccReport.enableInput);

    };

    self.closeDialog = function () {
        try {
            _tables.reportContainer.dialog("close");
        }
        catch (e) {
        }
    };

    self.exportGrandTotal = function () {
        AccReport.disableInput();
        var exportParams = reportManager.Params.globalVariable.CacheParam.GrandTotal;
        exportParams.IsExportGrandTotal = true;

        AccReport.downloadFile(window.rootUrl + "Gaming/CasinoGameStatistic/ExportGrandTotal", exportParams);
        AccReport.enableInput();
    };

    self.refineData = function (report) {
        var bodyData = [];
        var footerData = [];
        var columns = [];

        var reportLength = report && report.ChildsInfo ? report.ChildsInfo.length : 0;
        var classPayout = "";

        var timeRangeLength = report && report.ChildsInfo ? report.ChildsInfo.length : 0;
        if (timeRangeLength > 0) {
            var info;
            var indexRange = reportManager.GetPagingIndexRange(timeRangeLength);
            for (var i = indexRange.startIndex; i < indexRange.endIndex; i++) {
                var timeRangeWinLoss = report.ChildsInfo[i];
                var reportLength = timeRangeWinLoss.ChildsInfo.length;
                for (var j = 0; j < reportLength; j++) {
                    columns = [];
                    info = timeRangeWinLoss.ChildsInfo[j];

                    columns.push({ Name: "ColName", Value: "Total " + info.CurrencyName });
                    columns.push({ Name: "ColNumberOfPlayers", Value: info.NumberOfPlayers });
                    columns.push({ Name: "ColNumberOfGames", Value: info.NumberOfGames });
                    columns.push({ Name: "ColCurrency", Value: info.CurrencyName });
                    columns.push({ Name: "ColTotalBets", Value: info.TotalBets });
                    columns.push({ Name: "ColTotalWins", Value: info.TotalWins });
                    columns.push({ Name: "ColGameIncome", Value: info.GameIncome });

                    if (info.GamePayout > 1) {
                        classPayout = " red";
                    }
                    else {
                        classPayout = "";
                    }

                    columns.push({ Name: "ColGamePayout", Value: info.GamePayout, TdClass: classPayout });
                    if (reportManager.Params.reportParams.ShowRefundColumn) {
                        columns.push({ Name: "ColTotalRefund", Value: info.TotalRefund });
                    }

                    columns.push({ Name: "ColCompanyTurnover", Value: info.CompanyTurnover });
                    columns.push({ Name: "CompanyCommission", Value: info.CompanyCommission });
                    columns.push({ Name: "ColCompanyNetIncome", Value: info.CompanyNetIncome });
                    bodyData.push({
                        Columns: columns
                    });
                }
            }

            var reportLength = report.GrandTotal.ChildsInfo.length;

            var footerColumns = [];
            info = report.GrandTotal.ChildsInfo[reportLength - 1];
            footerColumns.push({ Name: "ColName", Value: info.Name });
            footerColumns.push({ Name: "ColNumberOfPlayers", Value: info.NumberOfPlayers });
            footerColumns.push({ Name: "ColNumberOfGames", Value: info.NumberOfGames });
            footerColumns.push({ Name: "ColCurrency", Value: info.CurrencyName });
            footerColumns.push({ Name: "ColTotalBets", Value: info.TotalBets });
            footerColumns.push({ Name: "ColTotalWins", Value: info.TotalWins });
            footerColumns.push({ Name: "ColGameIncome", Value: info.GameIncome });

            if (info.GamePayout > 1) {
                classPayout = " red";
            }
            else {
                classPayout = "";
            }

            footerColumns.push({ Name: "ColGamePayout", Value: info.GamePayout, TdClass: classPayout });
            if (reportManager.Params.reportParams.ShowRefundColumn) {
                footerColumns.push({ Name: "ColTotalRefund", Value: info.TotalRefund });
            }

            footerColumns.push({ Name: "ColCompanyTurnover", Value: info.CompanyTurnover });
            footerColumns.push({ Name: "CompanyCommission", Value: info.CompanyCommission });
            footerColumns.push({ Name: "ColCompanyNetIncome", Value: info.CompanyNetIncome });

            footerData.push({ Columns: footerColumns });
        }

        var data = {
            bodyData: bodyData,
            footerData: footerData,
            columns: columns
        };

        return data;
    };

    self.refineDataByUserName = function (report) {
        var bodyData = [];
        var footerData = [];
        var columns = [];

        var reportLength = report && report.ChildsInfo ? report.ChildsInfo.length : 0;
        var classPayout = "";

        var timeRangeLength = report && report.ChildsInfo ? report.ChildsInfo.length : 0;
        if (timeRangeLength > 0) {
            var info;
            var indexRange = reportManager.GetPagingIndexRange(timeRangeLength);
            for (var i = indexRange.startIndex; i < indexRange.endIndex; i++) {
                info = report.ChildsInfo[i];
                columns = [];
                columns.push({ Name: "ColName", Value: "Total " + info.CurrencyName });
                columns.push({ Name: "ColNumberOfPlayers", Value: info.NumberOfPlayers });
                columns.push({ Name: "ColNumberOfGames", Value: info.NumberOfGames });
                columns.push({ Name: "ColCurrency", Value: info.CurrencyName });
                columns.push({ Name: "ColTotalBets", Value: info.TotalBets });
                columns.push({ Name: "ColTotalWins", Value: info.TotalWins });
                columns.push({ Name: "ColGameIncome", Value: info.GameIncome });

                if (info.GamePayout >= 1) {
                    classPayout = " red";
                }
                else {
                    classPayout = "";
                }

                columns.push({ Name: "ColGamePayout", Value: info.GamePayout, TdClass: classPayout });
                if (reportManager.Params.reportParams.ShowRefundColumn) {
                    columns.push({ Name: "ColTotalRefund", Value: info.TotalRefund });
                }

                columns.push({ Name: "ColCompanyTurnover", Value: info.CompanyTurnover });
                columns.push({ Name: "CompanyCommission", Value: info.CompanyCommission });
                columns.push({ Name: "ColCompanyNetIncome", Value: info.CompanyNetIncome });
                bodyData.push({
                    Columns: columns
                });
            }

            var reportLength = report.GrandTotal.ChildsInfo.length;

            var footerColumns = [];
            info = report.GrandTotal.ChildsInfo[reportLength - 1];
            footerColumns.push({ Name: "ColName", Value: info.Name });
            footerColumns.push({ Name: "ColNumberOfPlayers", Value: info.NumberOfPlayers });
            footerColumns.push({ Name: "ColNumberOfGames", Value: info.NumberOfGames });
            footerColumns.push({ Name: "ColCurrency", Value: info.CurrencyName });
            footerColumns.push({ Name: "ColTotalBets", Value: info.TotalBets });
            footerColumns.push({ Name: "ColTotalWins", Value: info.TotalWins });
            footerColumns.push({ Name: "ColGameIncome", Value: info.GameIncome });

            if (info.GamePayout >= 1) {
                classPayout = " red";
            }
            else {
                classPayout = "";
            }

            footerColumns.push({ Name: "ColGamePayout", Value: info.GamePayout, TdClass: classPayout });
            if (reportManager.Params.reportParams.ShowRefundColumn) {
                footerColumns.push({ Name: "ColTotalRefund", Value: info.TotalRefund });
            }

            footerColumns.push({ Name: "ColCompanyTurnover", Value: info.CompanyTurnover });
            footerColumns.push({ Name: "CompanyCommission", Value: info.CompanyCommission });
            footerColumns.push({ Name: "ColCompanyNetIncome", Value: info.CompanyNetIncome });

            footerData.push({ Columns: footerColumns });
        }

        var data = {
            bodyData: bodyData,
            footerData: footerData,
            columns: columns
        };

        return data;
    };

    self.RenderGridOptions = function (report) {
        var bodyData = [];
        var footerData = [];
        var columns = [];
        var data = null;
        var selectParam = reportManager.Params.globalVariable.CacheParam.Params;
        var isReportByUserName = selectParam.ReportType == reportManager.Enums.ReportType.UserName;
        if (isReportByUserName) {
            data = self.refineDataByUserName(report);
        }
        else {
            data = self.refineData(report);
        }

        bodyData = data.bodyData;
        footerData = data.footerData;
        columns = data.columns;

        var columnsHeader = [];
        columnsHeader.push({
            HeaderText: "Grand Total", Width: 100, Name: "ColName", HeaderAlign: "center", CellAlign: "center"
        });
        columnsHeader.push({
            HeaderText: viewResources.No_Players, Width: 80, Name: "ColNumberOfPlayers", HeaderAlign: "center", CellAlign: "right",
            FormatNumber: FormatConstant.INT_NUMBER
        });
        columnsHeader.push({
            HeaderText: viewResources.Bet_Count, Width: 80, Name: "ColNumberOfGames", HeaderAlign: "center", CellAlign: "right",
            FormatNumber: FormatConstant.INT_NUMBER
        });
        columnsHeader.push({
            HeaderText: viewResources.Currency, Width: 80, Name: "ColCurrency", HeaderAlign: "center", CellAlign: "center"
        });
        columnsHeader.push({
            HeaderText: viewResources.Total_bets, Width: 120, Name: "ColTotalBets", HeaderAlign: "center", CellAlign: "right",
            FormatNumber: FormatConstant.TWO_DECIMAL_NUMBER
        });
        columnsHeader.push({
            HeaderText: viewResources.Total_wins, Width: 120, Name: "ColTotalWins", HeaderAlign: "center", CellAlign: "right",
            FormatNumber: FormatConstant.TWO_DECIMAL_NUMBER
        });
        columnsHeader.push({
            HeaderText: viewResources.Game_income, Width: 100, Name: "ColGameIncome", FormatNumber: FormatConstant.TWO_DECIMAL_NUMBER,
            HeaderAlign: "center", CellAlign: "right", FormatNegative: '$'
        });
        columnsHeader.push({
            HeaderText: viewResources.Game_payout, Width: 100, Name: "ColGamePayout", FormatNumber: FormatConstant.TWO_DECIMAL_PERCENT_NUMBER,
            HeaderAlign: "center", CellAlign: "right", FormatNegative: '$'
        });
        if (reportManager.Params.reportParams.ShowRefundColumn) {
            columnsHeader.push({
                HeaderText: viewResources.Total_refund, Width: 100, Name: "ColTotalRefund", FormatNumber: FormatConstant.TWO_DECIMAL_NUMBER,
                HeaderAlign: "center", CellAlign: "right", FormatNegative: '$'
            });
        }

        columnsHeader.push({
            HeaderText: viewResources.Company_Turnover, Width: 100, Name: "ColCompanyTurnover", FormatNumber: FormatConstant.TWO_DECIMAL_NUMBER,
            HeaderAlign: "center", CellAlign: "right"
        });
        columnsHeader.push({
            HeaderText: viewResources.Company_comm, Width: 100, Name: "CompanyCommission", FormatNumber: FormatConstant.TWO_DECIMAL_NUMBER,
            HeaderAlign: "center", CellAlign: "right", FormatNegative: '$'
        });
        columnsHeader.push({
            HeaderText: viewResources.Company_net_income, Width: 100, Name: "ColCompanyNetIncome", FormatNumber: FormatConstant.TWO_DECIMAL_NUMBER,
            HeaderAlign: "center", CellAlign: "right", FormatNegative: '$'
        });

        var title = "GRAND TOTAL BY CURRENCY";

        var gridOptions = {
            columns: columnsHeader,
            showTitle: true,
            showFooter: true,
            titleInfo: {
                text: title,
                showExcelButton: true,
                exportExcelFunction: function () {
                    self.exportGrandTotal();
                }
            },
            bodyRows: bodyData,
            footerRows: footerData,
            numberOfFrozenColumn: 1,
            gridWidth: 1000,
            gridExpandHeight: 355,
            fullWidth: true,
            showDistinctRow: true,
            showHighlightRow: false,
            headerColumnHeight: 29,
            showPager: false,
            customPager: false
        };

        _tables.reportContainer.removeClass("display-none");
        _tables.report.removeClass("display-none").fanexGrid(gridOptions);
        _tables.report.fanexGrid("resizeGrid");

        var selectParam = reportManager.Params.globalVariable.CacheParam.Params;
        var width = 1170;
        if (selectParam.ShowRefundColumn) {
            width = 1290;
        }

        _tables.reportContainer.dialog({
            title: "",
            autoOpen: false,
            width: width,
            modal: false,
            resizable: false,
            close: function (event, ui) {
                _tables.reportContainer.dialog("destroy").remove();
                _tables.reportParentContainer.append('<div id="grandTotalContainer" class="display-none"> \
                                                        <div id="grandTotal" class="freegrid display-none"> \
                                                        </div> \
                                                      </div>');
                _tables.reportContainer = $('#grandTotalContainer');
                _tables.report = $('#grandTotal');
            }
        });

        _tables.reportContainer.dialog("open");
    };
};