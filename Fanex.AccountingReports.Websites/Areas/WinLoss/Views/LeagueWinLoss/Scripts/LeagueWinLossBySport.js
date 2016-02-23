

Namespace.Register("Fanex.AccountingReports.WinLoss.LeagueWinLoss.LeagueWinLossBySport");
Fanex.AccountingReports.WinLoss.LeagueWinLoss.LeagueWinLossBySport = function () {
    var self = this;
    var _tables = {
        reportContainer: null,
        reportLive: null,
        reportNoneLive: null
    };

    self.Params = {
        reportParams: {
            Winlostdate: null,
            Leagueid: null,
            Sporttype: null,
            LeagueName: '',
            SportName: ''
        }
    };
    self.initPage = function () {
        cachedDOMElements();
    };
    var cachedDOMElements = function () {
        _tables.reportContainer = $("#leagueWinLossReportBySportContainer");
        _tables.reportLiveContainer = $("#liveReport");
        _tables.reportNoneLiveContainer = $("#noneLiveReport");

        _tables.reportLive = $("#leagueWinLossLiveReportBySport");
        _tables.reportNoneLive = $("#leagueWinLossNoneLiveReportBySport");
    };
    self.hideReport = function () {
        _tables.reportContainer.addClass("display-none");
        _tables.reportLiveContainer.addClass("display-none");
        _tables.reportNoneLiveContainer.addClass("display-none");
        _tables.reportLive.addClass("display-none");
        _tables.reportNoneLive.addClass("display-none");
    };
    self.showReport = function () {
        _tables.reportLive.removeClass("display-none");
        _tables.reportNoneLive.removeClass("display-none");
        _tables.reportLiveContainer.removeClass("display-none");
        _tables.reportNoneLiveContainer.removeClass("display-none");
        _tables.reportContainer.removeClass("display-none");
    };
    self.getReport = function (params) {
        AccReport.disableInput();
        getParams(params);
        $.ajax({
            type: "POST",
            url: window.rootUrl + "WinLoss/LeagueWinLoss/GetLeagueWinLossBySport",
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            data: JSON.stringify(self.Params.reportParams),
            beforeSend: function () {
                AccReport.disableInput();
            },
            success: function (rs) {
                self.hideReport();
                if (rs && rs.ChildsInfo.length > 0) {
                    var gridHeight = $(window).height();

                    gridHeight = gridHeight * 2 / 3 - 20;

                    for (var i = 0; i < rs.ChildsInfo.length; i++) {
                        renderReport(rs.ChildsInfo[i], gridHeight);
                    }
                }
                else {
                    renderReport(rs);
                }
            },
            error: function (event, jqXHR, ajaxSettings) {
                self.hideReport();
                var gridHeight = $(window).height();
                renderReport([], gridHeight);
            },
            complete: function () {
                AccReport.enableInput();
            }
        });
    };
    var getParams = function (params) {
        self.Params.reportParams = params;
    };

    var showNormalBetType = function (i, info, bodyData) {
        var currentRow = {
            Columns: [{
                Name: "ColBetType", Value: info, CssClass: "colBetType",
                CustomCellRender: function (currentCell) {
                    var clickEvent = ' onclick = "viewLeagueWinLossByBetType(' + currentCell.Value.Id
                                + ',\'' + currentCell.Value.Name
                                + '\',' + currentCell.Value.IsLive
                                + ',\'' + currentCell.Value.Hedge
                                 + "');\"";
                    var html = '<div class="link" ' + clickEvent + '>' + currentCell.Value.Name + currentCell.Value.Hedge + '</div>';
                    return html;
                }
            },
            { Name: "ColWinLoss", Value: info.WinLoss },
            { Name: "ColPosition", Value: info.Position },
            { Name: "ColCommision", Value: info.Commission },
            { Name: "ColLineTotal", Value: info.LineTotal },
            { Name: "ColTurnOver", Value: info.TurnOver },
            { Name: "ColMargin", Value: info.Margin }],
            CssClass: i % 2 == 0 ? "even" : "",
            SubRows: []
        };

        bodyData.push(currentRow);
    };

    var showChargedCommission = function (info, footerData) {
        var currentRow = {
            Columns: [{
                Name: "ColBetType", Value: info, CssClass: "align-right",
                CustomCellRender: function (currentCell) {
                    var clickEvent = ' onclick = "viewLeagueWinLossByBetType(' + currentCell.Value.Id
                                + ',\'' + "Charged Commission"
                                + '\',' + currentCell.Value.IsLive
                                + ',\'' + currentCell.Value.Hedge
                                 + "');\"";
                    var html = '<div class="link" ' + clickEvent + '>' + "Charged Commission" + currentCell.Value.Hedge + '</div>';
                    return html;
                }
            },
            { Name: "ColWinLoss", Value: info.WinLoss },
            { Name: "ColPosition", Value: info.Position },
            { Name: "ColCommision", Value: info.Commission },
            { Name: "ColLineTotal", Value: info.LineTotal },
            { Name: "ColTurnOver", Value: info.TurnOver },
            { Name: "ColMargin", Value: info.Margin }]
        };

        footerData.push(currentRow);
    };

    var renderReport = function (report, gridHeight) {
        var reportTable = _tables.reportLive;
        var reportTableContainer = _tables.reportLiveContainer;
        if (report) {
            if (report.Name != null && report.Name.toLowerCase() != "live") {
                reportTable = _tables.reportNoneLive;
                reportTableContainer = _tables.reportNoneLiveContainer;
            }
        }

        _tables.reportContainer.removeClass("display-none");
        reportTableContainer.removeClass("display-none");
        reportTable.addClass("display-none");
        var bodyData = [];
        var footerData = [];

        if (report.ChildsInfo.length > 0) {
            var len = report.ChildsInfo.length;
            var currentRow = null;
            var info = null;
            for (var i = 0; i < len; i++) {
                info = report.ChildsInfo[i];
                if (info.Id != 500) {
                    showNormalBetType(i, info, bodyData);
                }
                else {
                    showChargedCommission(info, footerData);
                }
            }

            info = report.Total;
            currentRow =
            {
                Columns: [{ Name: "ColBetType", Value: info.Name, CssClass: "align-right" },
                          { Name: "ColWinLoss", Value: info.WinLoss },
                          { Name: "ColPosition", Value: info.Position },
                          { Name: "ColCommision", Value: info.Commission },
                          { Name: "ColLineTotal", Value: info.LineTotal },
                          { Name: "ColTurnOver", Value: info.TurnOver },
                          { Name: "ColMargin", Value: info.Margin }]
            };

            footerData.splice(0, 0, currentRow);

            if (is8RMActive.toLowerCase() == 'true') {
                info = report.EightRM;
                currentRow =
                {
                    Columns: [{ Name: "ColBetType", Value: info.Name, CssClass: "align-right", MergeColNumber: 4 },
                              { Name: "ColLineTotal", Value: info.LineTotal },
                              { Name: "ColTurnOver", Value: "", MergeColNumber: 2 }]
                };

                footerData.push(currentRow);
            }


            if (isBARMActive.toLowerCase() == 'true') {
                info = report.BARM;
                currentRow =
                {
                    Columns: [{ Name: "ColBetType", Value: info.Name, CssClass: "align-right", MergeColNumber: 4 },
                              { Name: "ColLineTotal", Value: info.LineTotal },
                              { Name: "ColTurnOver", Value: "&nbsp;", MergeColNumber: 2 }]
                };

                footerData.push(currentRow);
            }

            info = report.GrandTotal;
            currentRow =
            {
                Columns: [{ Name: "ColBetType", Value: info.Name, CssClass: "align-right", MergeColNumber: 4 },
                          { Name: "ColLineTotal", Value: info.LineTotal },
                          { Name: "ColTurnOver", Value: "", MergeColNumber: 2 }]
            };

            footerData.push(currentRow);
        }

        var gridOptions = {
            columns: [{ HeaderText: "Bet Type", Width: 300, Name: "ColBetType", HeaderAlign: "center", CellAlign: "left" },
                      { HeaderText: "Win Loss", Width: 100, Name: "ColWinLoss", HeaderAlign: "center", CellAlign: "right", FormatNumber: FormatConstant.TWO_DECIMAL_NUMBER, FormatPositive: '$' },
                      { HeaderText: "Position", Width: 100, Name: "ColPosition", HeaderAlign: "center", CellAlign: "right", FormatNumber: FormatConstant.TWO_DECIMAL_NUMBER },
                      { HeaderText: "Comm", Width: 100, Name: "ColCommision", HeaderAlign: "center", CellAlign: "right", FormatNumber: FormatConstant.TWO_DECIMAL_NUMBER },
                      { HeaderText: "Line Total", Width: 100, Name: "ColLineTotal", HeaderAlign: "center", CellAlign: "right", FormatNumber: FormatConstant.TWO_DECIMAL_NUMBER, FormatPositive: '$' },
                      { HeaderText: "Turnover", Width: 100, Name: "ColTurnOver", HeaderAlign: "center", CellAlign: "right", FormatNumber: FormatConstant.TWO_DECIMAL_NUMBER },
                      { HeaderText: "Margin", Width: 100, Name: "ColMargin", HeaderAlign: "center", CellAlign: "right", FormatNumber: FormatConstant.TWO_DECIMAL_PERCENT_NUMBER, FormatPositive: '$' }],
            showTitle: true,
            showFooter: true,
            titleInfo:
            {
                text: self.Params.reportParams.SportName + " - " +
                      self.Params.reportParams.LeagueName + ((report != null && report.Name != null) ? " - " + report.Name : ""),
                showExcelButton: true,
                exportExcelFunction: function () {
                    self.exportReport();
                },

                showBackButton: true,
                backFunction: function () {
                    backToLeagueWinLoss();
                },

                showCollapseExpandButton: false
            },
            showTree: false,
            bodyRows: bodyData,
            showDistinctRow: false,
            footerRows: footerData,
            fullHeight: report.Id == 0,
            gridExpandHeight: report.Id == 0 ? 220 + _tables.reportLive.height() : 0,
            gridHeight: report.Id == 0 ? 0 : ($(window).height() - 200) / 2
        };

        reportTable.removeClass("display-none").fanexGrid(gridOptions);
    }
    self.exportReport = function () {
        AccReport.downloadFile(window.rootUrl + "WinLoss/LeagueWinLoss/ExportLeagueWinLossBySport", self.Params.reportParams);
    };
}