

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
            StartDate: null,
            EndDate: null,
            LeagueId: null,
            SportType: null,
            LeagueName: '',
            SportName: '',
            IsNormalMode: true
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
                if (rs && rs.length > 0) {
                    var gridHeight = $(window).height();

                    gridHeight = gridHeight * 2 / 3 - 20;

                    for (var i = 0; i < rs.length; i++) {
                        renderReport(rs[i], gridHeight);
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

    var showBetType = function (i, info, bodyData) {
        var currentRow = {
            Columns: [{
                Name: "ColBetType", Value: info, CssClass: "colBetType",
                CustomCellRender: function (currentCell) {
                    var clickEvent = ' onclick = "viewLeagueWinLossByBetType(' + currentCell.Value.Bettype
                                + ',\'' + currentCell.Value.BettypeName
                                + '\',' + currentCell.Value.LiveIndicator
                                + '\,' + currentCell.Value.IsHedge
                                + ");\"";
                    var hedgeText = currentCell.Value.IsHedge == true ? " (hedge)" : "";
                    var html = '<div class="link" ' + clickEvent + '>' + currentCell.Value.BettypeName + hedgeText + '</div>';
                    return html;
                }
                },
                { Name: "ColMemberWinLoss", Value: info.MemberWinLoss },
                { Name: "ColSMAWinLoss", Value: info.SMAWinLoss },
                { Name: "ColExtraPT", Value: info.ExtraPT },
                { Name: "ColCommision", Value: info.Commission },
                { Name: "ColVariance", Value: info.VarianceIncome },
                { Name: "ColVirtualWinLoss", Value: info.VirtualWinLoss },
                { Name: "ColRealIncome", Value: info.RealIncome },
                { Name: "ColVirtualTurnOver", Value: info.VirtualTurnover },
                { Name: "ColRealTurnOver", Value: info.RealTurnover },
                { Name: "ColVirtualMargin", Value: info.VirtualMargin },
                { Name: "ColRealMargin", Value: info.RealMargin }],
            CssClass: i % 2 == 0 ? "even" : "",
            SubRows: []
        };

        bodyData.push(currentRow);
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
        var bettypeWinloss = report.BetTypeWinLoss;
        if (bettypeWinloss.BettypeWinLoss.length > 0) {
            var len = bettypeWinloss.BettypeWinLoss.length;
            var currentRow = null;
            var info = null;
            for (var i = 0; i < len; i++) {
                info = bettypeWinloss.BettypeWinLoss[i];
                showBetType(i, info, bodyData);
            }

            info = bettypeWinloss.Total;
            currentRow =
            {
                Columns: [{ Name: "ColBetType", Value: info.BettypeName, CssClass: "align-right" },
                        { Name: "ColMemberWinLoss", Value: info.MemberWinLoss },
                        { Name: "ColSMAWinLoss", Value: info.SMAWinLoss },
                        { Name: "ColExtraPT", Value: info.ExtraPT },
                        { Name: "ColCommision", Value: info.Commission },
                        { Name: "ColVariance", Value: info.VarianceIncome },
                        { Name: "ColVirtualWinLoss", Value: info.VirtualWinLoss },
                        { Name: "ColRealIncome", Value: info.RealIncome },
                        { Name: "ColVirtualTurnOver", Value: info.VirtualTurnover },
                        { Name: "ColRealTurnOver", Value: info.RealTurnover },
                        { Name: "ColVirtualMargin", Value: info.VirtualMargin },
                        { Name: "ColRealMargin", Value: info.RealMargin }]
            };

            footerData.splice(0, 0, currentRow);

            if (self.isShowTotalRow(bettypeWinloss.RM)) {
                info = bettypeWinloss.RM;
                currentRow =
                {
                    Columns: [{ Name: "ColBetType", Value: info.BettypeName, CssClass: "align-right", MergeColNumber: 6 },
                            { Name: "ColVirtualWinLoss", Value: info.VirtualWinLoss },
                            { Name: "ColRealIncome", Value: info.RealIncome },
                            { Name: "ColVirtualTurnOver", Value: info.VirtualTurnover },
                            { Name: "ColRealTurnOver", Value: info.RealTurnover },
                            { Name: "ColVirtualMargin", Value: "", MergeColNumber: 2 }]
                };

                footerData.push(currentRow);
            }

            if (self.isShowTotalRow(bettypeWinloss.BARM)) {
                info = bettypeWinloss.BARM;
                currentRow =
                {
                    Columns: [{ Name: "ColBetType", Value: info.BettypeName, CssClass: "align-right", MergeColNumber: 6 },
                            { Name: "ColVirtualWinLoss", Value: info.VirtualWinLoss },
                            { Name: "ColRealIncome", Value: info.RealIncome },
                            { Name: "ColVirtualTurnOver", Value: info.VirtualTurnover },
                            { Name: "ColRealTurnOver", Value: info.RealTurnover },
                            { Name: "ColVirtualMargin", Value: "", MergeColNumber: 2 }]
                };

                footerData.push(currentRow);
            }

            info = bettypeWinloss.GrandTotal;
            currentRow =
            {
                Columns: [{ Name: "ColBetType", Value: info.BettypeName, CssClass: "align-right", MergeColNumber: 6 },
                           { Name: "ColVirtualWinLoss", Value: info.VirtualWinLoss },
                           { Name: "ColRealIncome", Value: info.RealIncome },
                           { Name: "ColVirtualTurnOver", Value: info.VirtualTurnover },
                           { Name: "ColRealTurnOver", Value: info.RealTurnover },
                           { Name: "ColVirtualMargin", Value: "", MergeColNumber: 2 }]
            };

            footerData.push(currentRow);
        }

        var gridOptions = {
            customizeHeader: true,
            groupHeaderRows: [{
                Columns: [{ HeaderText: "Bet Type", Name: "ColBetType", MergeRowNumber: 2 },
                          { HeaderText: "Member Win Loss", Name: "ColMemberWinLoss", MergeRowNumber: 2 },
                          { HeaderText: "SMA Win Loss", Name: "ColSMAWinLoss", MergeRowNumber: 2 },
                          { HeaderText: "Extra PT", Width: 100, Name: "ColExtraPT", MergeRowNumber: 2 },
						  { HeaderText: "Commission", Name: "ColCommision", MergeRowNumber: 2 },
						  { HeaderText: "House", Name: "ColVariance", MergeColNumber: 3 },
						  { HeaderText: "House Turnover", Name: "ColVirtualTurnOver", MergeColNumber: 2 },
						  { HeaderText: "Margin", Name: 'ColVirtualMargin', MergeColNumber: 2 }]
            }],
            columns: [{ HeaderText: "Bet Type", Width: 300, Name: "ColBetType", HeaderAlign: "center", CellAlign: "left" },
                    { HeaderText: "Member Win Loss", Width: 120, Name: "ColMemberWinLoss", HeaderAlign: "center", CellAlign: "right", FormatNumber: FormatConstant.TWO_DECIMAL_NUMBER, FormatPositive: '$' },
                    { HeaderText: "SMA Win Loss", Width: 120, Name: "ColSMAWinLoss", HeaderAlign: "center", CellAlign: "right", FormatNumber: FormatConstant.TWO_DECIMAL_NUMBER, FormatPositive: '$' },
                    { HeaderText: "Extra PT", Width: 100, Name: "ColExtraPT", HeaderAlign: "center", CellAlign: "right", FormatNumber: FormatConstant.TWO_DECIMAL_NUMBER },
                    { HeaderText: "Commission", Width: 120, Name: "ColCommision", HeaderAlign: "center", CellAlign: "right", FormatNumber: FormatConstant.TWO_DECIMAL_NUMBER },
                    { HeaderText: "Variance", Width: 120, Name: "ColVariance", HeaderAlign: "center", CellAlign: "right", FormatNumber: FormatConstant.TWO_DECIMAL_NUMBER },
                    { HeaderText: "Virtual Win Loss", Width: 120, Name: "ColVirtualWinLoss", HeaderAlign: "center", CellAlign: "right", FormatNumber: FormatConstant.TWO_DECIMAL_NUMBER, FormatPositive: '$' },
                    { HeaderText: "Real Income", Width: 120, Name: "ColRealIncome", HeaderAlign: "center", CellAlign: "right", FormatNumber: FormatConstant.TWO_DECIMAL_NUMBER, FormatPositive: '$' },
                    { HeaderText: "Virtual", Width: 120, Name: "ColVirtualTurnOver", HeaderAlign: "center", CellAlign: "right", FormatNumber: FormatConstant.TWO_DECIMAL_NUMBER },
                    { HeaderText: "Real", Width: 120, Name: "ColRealTurnOver", HeaderAlign: "center", CellAlign: "right", FormatNumber: FormatConstant.TWO_DECIMAL_NUMBER },
                    { HeaderText: "Virtual", Width: 120, Name: "ColVirtualMargin", HeaderAlign: "center", CellAlign: "right", FormatNumber: FormatConstant.TWO_DECIMAL_PERCENT_NUMBER, FormatPositive: '$' },
                    { HeaderText: "Real", Width: 120, Name: "ColRealMargin", HeaderAlign: "center", CellAlign: "right", FormatNumber: FormatConstant.TWO_DECIMAL_PERCENT_NUMBER, FormatPositive: '$' }],
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

    self.isShowTotalRow = function (total) {
        return total.VirtualWinLoss != 0 || total.RealIncome != 0 || total.VirtualTurnover != 0 || total.RealTurnover != 0;
    }
}