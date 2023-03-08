Namespace.Register("Fanex.AccountingReports.WinLoss.LeagueWinLoss.LeagueWinLossViewBetTypesByStatus");
Fanex.AccountingReports.WinLoss.LeagueWinLoss.LeagueWinLossViewBetTypesByStatus = function () {
    var self = this;
    var _tables = {
        reportContainer: null,
        reportLiveContainer: null,
        reportLive: null,
        reportNonLiveContainer: null,
        reportNonLive: null
    };

    self.Params = {
        reportParams: {
            StartDate: null,
            EndDate: null,
            LeagueName: '',
            SportName: '',
            SportBettype: null,
            MatchID: null,
            MatchEvent: '',
            LiveIndicator: 2,
            IsNormalMode: true,
            IsNoDraw: false,
            SiteName: '',
            IsHedge: 0,
            IsIncludeSiteType: false,
            IsIncludeHedge: true,
            SportType: 0,
            LeagueId: '',
            PermissionProfileID: permissionProfileID
        }
    };
    self.initPage = function () {
        cachedDOMElements();
    };
    var cachedDOMElements = function () {
        _tables.reportContainer = $("#leagueWinLossBetTypeByStatusContainer");
        _tables.reportLiveContainer = $("#liveReport");
        _tables.reportLive = $("#leagueWinLossBetTypeLive");
        _tables.reportNonLiveContainer = $("#nonLiveReport");
        _tables.reportNonLive = $("#leagueWinLossBetTypeNonLive");
    };
    self.hideReport = function () {
        _tables.reportContainer.addClass("display-none");
        _tables.reportLiveContainer.addClass("display-none");
        _tables.reportLive.addClass("display-none");
        _tables.reportNonLiveContainer.addClass("display-none");
        _tables.reportNonLive.addClass("display-none");
    };
    self.showReport = function () {
        _tables.reportLive.removeClass("display-none");
        _tables.reportLiveContainer.removeClass("display-none");
        _tables.reportNonLive.removeClass("display-none");
        _tables.reportNonLiveContainer.removeClass("display-none");
        _tables.reportContainer.removeClass("display-none");
    };
    self.getReport = function (params) {
        self.getReportByStatus(params);
    };
    self.getReportByStatus = function (params) {
        AccReport.disableInput();
        getParams(params);
        $.ajax({
            type: "POST",
            url: window.rootUrl + "WinLoss/LeagueWinLoss/GetBetTypesByStatus",
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            data: JSON.stringify(self.Params.reportParams),
            beforeSend: function () {
                AccReport.disableInput();
            },
            success: function (rs) {
                self.hideReport();
                if (rs) {
                    var gridHeight = $(window).height();

                    gridHeight = gridHeight * 2 / 3 - 20;

                    renderLiveReport(rs.BetTypeWinLossLive, gridHeight);
                    renderNonLiveReport(rs.BetTypeWinLossNonLive, gridHeight);
                }
                else {
                    renderReport(rs);
                }
            },
            error: function (event, jqXHR, ajaxSettings) {
                self.hideReport();
                renderReport([]);
            },
            complete: function () {
                AccReport.enableInput();
            }
        });
    };
    var getParams = function (params) {
        self.Params.reportParams = params;
    };

    var showBetType = function (info, bodyData, liveStatus) {
        var hedgeText = self.Params.reportParams.IsHedge ? " (hedge) " : "";
        var currentRow = {
            Columns: [
                {
                    Name: "ColBetType", Value: info, CssClass: "colBetType",
                    CustomCellRender: function (currentCell) {
                        var clickEvent = ' onclick = "viewMatchesFromBetTypeStatus(' + currentCell.Value.Bettype
                                    + ',\'' + currentCell.Value.BettypeName
                                    + '\',' + liveStatus
                                    + ',' + currentCell.Value.IsHedge + ");\"";
                        var displayBettype = currentCell.Value.IsHedge ? currentCell.Value.BettypeName + ' (hedge)' : currentCell.Value.BettypeName;
                        var html = '<div class="link" ' + clickEvent + '>' + displayBettype + '</div>';
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
        };

        bodyData.push(currentRow);
    };

    var showGroupBetTypeTotal = function (total, bodyData) {
        var currentRow = {
            Columns: [
                { Name: "ColBetType", Value: total.BettypeName, CssClass: "align-right" },
                { Name: "ColMemberWinLoss", Value: total.MemberWinLoss },
                { Name: "ColSMAWinLoss", Value: total.SMAWinLoss },
                { Name: "ColExtraPT", Value: total.ExtraPT },
                { Name: "ColCommision", Value: total.Commission },
                { Name: "ColVariance", Value: total.VarianceIncome },
                { Name: "ColVirtualWinLoss", Value: total.VirtualWinLoss },
                { Name: "ColRealIncome", Value: total.RealIncome },
                { Name: "ColVirtualTurnOver", Value: total.VirtualTurnover },
                { Name: "ColRealTurnOver", Value: total.RealTurnover },
                { Name: "ColVirtualMargin", Value: total.VirtualMargin },
                { Name: "ColRealMargin", Value: total.RealMargin }],
            CssClass: 'subTotal'
        };

        bodyData.push(currentRow);
    };

    var renderNonLiveReport = function (report, gridHeight) {
        var reportTable = _tables.reportNonLive;
        var reportTableContainer = _tables.reportNonLiveContainer
        var liveStatus = "Non-Live";
        var isLive = false;

        _tables.reportContainer.removeClass("display-none");
        reportTableContainer.removeClass("display-none");

        var bodyData = [];
        var footerData = [];
        var info = null;
        var length = report.BettypeWinLoss.length;
        if (length > 0) {
            for (var i = 0; i < length; i++) {
                info = report.BettypeWinLoss[i];
                showBetType(info, bodyData, isLive);
            }
        }

        info = report.Total;
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

        if (self.isShowTotalRow(report.RM)) {
            info = report.RM;
            currentRow =
            {
                Columns: [{ Name: "ColBetType", Value: info.SiteType, CssClass: "align-right", MergeColNumber: 6 },
                        { Name: "ColVirtualWinLoss", Value: info.VirtualWinLoss },
                        { Name: "ColRealIncome", Value: info.RealIncome },
                        { Name: "ColVirtualTurnOver", Value: info.VirtualTurnover },
                        { Name: "ColRealTurnOver", Value: info.RealTurnover },
                        { Name: "ColVirtualMargin", Value: "", MergeColNumber: 2 }]
            };

            footerData.push(currentRow);
        }

        if (self.isShowTotalRow(report.BARM)) {
            info = report.BARM;
            currentRow =
            {
                Columns: [{ Name: "ColBetType", Value: info.SiteType, CssClass: "align-right", MergeColNumber: 6 },
                        { Name: "ColVirtualWinLoss", Value: info.VirtualWinLoss },
                        { Name: "ColRealIncome", Value: info.RealIncome },
                        { Name: "ColVirtualTurnOver", Value: info.VirtualTurnover },
                        { Name: "ColRealTurnOver", Value: info.RealTurnover },
                        { Name: "ColVirtualMargin", Value: "", MergeColNumber: 2 }]
            };

            footerData.push(currentRow);
        }

        if (self.isShowGrandTotalRow(report)) {
            info = report.GrandTotal;
            var totalInfo = report.Total;
            currentRow =
            {
                Columns: [{ Name: "ColBetType", Value: info.BettypeName, CssClass: "align-right", MergeColNumber: 6 },
                           { Name: "ColVirtualWinLoss", Value: totalInfo.VirtualWinLoss },
                           { Name: "ColRealIncome", Value: info.RealIncome },
                           { Name: "ColVirtualTurnOver", Value: totalInfo.VirtualTurnover },
                           { Name: "ColRealTurnOver", Value: totalInfo.RealTurnover },
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
                          { HeaderText: "Extra PT", Name: "ColExtraPT", MergeRowNumber: 2 },
                          { HeaderText: "Commission", Name: "ColCommision", MergeRowNumber: 2 },
                          { HeaderText: "House", Name: "ColVariance", MergeColNumber: 3 },
                          { HeaderText: "House Turnover", Name: "ColVirtualTurnOver", MergeColNumber: 2 },
                          { HeaderText: "Margin", Name: 'ColVirtualMargin', MergeColNumber: 2 }]
            }],
            columns: [{ HeaderText: "Bet Type", Width: 300, Name: "ColBetType", HeaderAlign: "center", CellAlign: "left", ShowCollapseExpand: true },
                    { HeaderText: "Member Win Loss", Width: 120, Name: "ColMemberWinLoss", HeaderAlign: "center", CellAlign: "right", FormatNumber: FormatConstant.TWO_DECIMAL_NUMBER, FormatPositive: '$' },
                    { HeaderText: "SMA Win Loss", Width: 120, Name: "ColSMAWinLoss", HeaderAlign: "center", CellAlign: "right", FormatNumber: FormatConstant.TWO_DECIMAL_NUMBER },
                    { HeaderText: "Extra PT", Width: 100, Name: "ColExtraPT", HeaderAlign: "center", CellAlign: "right", FormatNumber: FormatConstant.TWO_DECIMAL_NUMBER },
                    { HeaderText: "Commission", Width: 120, Name: "ColCommision", HeaderAlign: "center", CellAlign: "right", FormatNumber: FormatConstant.TWO_DECIMAL_NUMBER },
                    { HeaderText: "Variance", Width: 120, Name: "ColVariance", HeaderAlign: "center", CellAlign: "right", FormatNumber: FormatConstant.TWO_DECIMAL_NUMBER, FormatPositive: '$' },
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
                      self.Params.reportParams.LeagueName + " - " +
                      liveStatus,
                showExcelButton: true,
                exportExcelFunction: function () {
                    self.exportReport();
                },

                showBackButton: true,
                backFunction: function () {
                    backToLeagueWinLoss();
                },

                showCollapseExpandButton: true
            },
            showTree: false,
            bodyRows: bodyData,
            showDistinctRow: true,
            footerRows: footerData,
            fullHeight: length === 0,
            gridExpandHeight: length === 0 ? 300 + _tables.reportLive.height() : 0,
            gridHeight: length === 0 ? 0 : ($(window).height() - 280) / 2
        };

        reportTable.removeClass("display-none").fanexGrid(gridOptions);
    }
    self.exportReport = function () {
        AccReport.downloadFile(window.rootUrl + "WinLoss/LeagueWinLoss/ExportLeagueWinLossViewBetTypesByStatus", self.Params.reportParams);
    };

    self.isShowTotalRow = function (total) {
        return total.VirtualWinLoss != 0 || total.RealIncome != 0 || total.VirtualTurnover != 0 || total.RealTurnover != 0;
    };

    self.isShowGrandTotalRow = function (report) {
        return self.isShowTotalRow(report.RM)
        || self.isShowTotalRow(report.BARM);
    };


    var renderLiveReport = function (report, gridHeight) {
        var reportTable = _tables.reportLive;
        var reportTableContainer = _tables.reportLiveContainer;
        var liveStatus = "Live"
        var isLive = true;

        _tables.reportContainer.removeClass("display-none");
        reportTableContainer.removeClass("display-none");

        var bodyData = [];
        var footerData = [];
        var info = null;
        var length = report.BetTypeWinLossLiveGroups.length;
        if (length > 0) {
            for (var i = 0; i < length; i++) {
                info = report.BetTypeWinLossLiveGroups[i];
                var betTypeLength = info.BettypeWinLoss.length;
                if (betTypeLength > 0) {
                    for (var j = 0; j < betTypeLength; j++) {
                        var betTypeInfo = info.BettypeWinLoss[j];
                        showBetType(betTypeInfo, bodyData, isLive);
                    }


                    showGroupBetTypeTotal(info.Total, bodyData);
                }
            }
        }

        info = report.Total;
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

        if (self.isShowTotalRow(report.RM)) {
            info = report.RM;
            currentRow =
            {
                Columns: [{ Name: "ColBetType", Value: info.SiteType, CssClass: "align-right", MergeColNumber: 6 },
                        { Name: "ColVirtualWinLoss", Value: info.VirtualWinLoss },
                        { Name: "ColRealIncome", Value: info.RealIncome },
                        { Name: "ColVirtualTurnOver", Value: info.VirtualTurnover },
                        { Name: "ColRealTurnOver", Value: info.RealTurnover },
                        { Name: "ColVirtualMargin", Value: "", MergeColNumber: 2 }]
            };

            footerData.push(currentRow);
        }

        if (self.isShowTotalRow(report.BARM)) {
            info = report.BARM;
            currentRow =
            {
                Columns: [{ Name: "ColBetType", Value: info.SiteType, CssClass: "align-right", MergeColNumber: 6 },
                        { Name: "ColVirtualWinLoss", Value: info.VirtualWinLoss },
                        { Name: "ColRealIncome", Value: info.RealIncome },
                        { Name: "ColVirtualTurnOver", Value: info.VirtualTurnover },
                        { Name: "ColRealTurnOver", Value: info.RealTurnover },
                        { Name: "ColVirtualMargin", Value: "", MergeColNumber: 2 }]
            };

            footerData.push(currentRow);
        }

        if (self.isShowGrandTotalRow(report)) {
            info = report.GrandTotal;
            var totalInfo = report.Total;
            currentRow =
            {
                Columns: [{ Name: "ColBetType", Value: info.BettypeName, CssClass: "align-right", MergeColNumber: 6 },
                           { Name: "ColVirtualWinLoss", Value: totalInfo.VirtualWinLoss },
                           { Name: "ColRealIncome", Value: info.RealIncome },
                           { Name: "ColVirtualTurnOver", Value: totalInfo.VirtualTurnover },
                           { Name: "ColRealTurnOver", Value: totalInfo.RealTurnover },
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
                          { HeaderText: "Extra PT", Name: "ColExtraPT", MergeRowNumber: 2 },
                          { HeaderText: "Commission", Name: "ColCommision", MergeRowNumber: 2 },
                          { HeaderText: "House", Name: "ColVariance", MergeColNumber: 3 },
                          { HeaderText: "House Turnover", Name: "ColVirtualTurnOver", MergeColNumber: 2 },
                          { HeaderText: "Margin", Name: 'ColVirtualMargin', MergeColNumber: 2 }]
            }],
            columns: [{ HeaderText: "Bet Type", Width: 300, Name: "ColBetType", HeaderAlign: "center", CellAlign: "left", ShowCollapseExpand: true },
                    { HeaderText: "Member Win Loss", Width: 120, Name: "ColMemberWinLoss", HeaderAlign: "center", CellAlign: "right", FormatNumber: FormatConstant.TWO_DECIMAL_NUMBER, FormatPositive: '$' },
                    { HeaderText: "SMA Win Loss", Width: 120, Name: "ColSMAWinLoss", HeaderAlign: "center", CellAlign: "right", FormatNumber: FormatConstant.TWO_DECIMAL_NUMBER },
                    { HeaderText: "Extra PT", Width: 100, Name: "ColExtraPT", HeaderAlign: "center", CellAlign: "right", FormatNumber: FormatConstant.TWO_DECIMAL_NUMBER },
                    { HeaderText: "Commission", Width: 120, Name: "ColCommision", HeaderAlign: "center", CellAlign: "right", FormatNumber: FormatConstant.TWO_DECIMAL_NUMBER },
                    { HeaderText: "Variance", Width: 120, Name: "ColVariance", HeaderAlign: "center", CellAlign: "right", FormatNumber: FormatConstant.TWO_DECIMAL_NUMBER, FormatPositive: '$' },
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
                      self.Params.reportParams.LeagueName + " - " +
                      liveStatus,
                showExcelButton: true,
                exportExcelFunction: function () {
                    self.exportReport();
                },

                showBackButton: true,
                backFunction: function () {
                    backToLeagueWinLoss();
                },

                showCollapseExpandButton: true
            },
            showTree: false,
            bodyRows: bodyData,
            showDistinctRow: true,
            footerRows: footerData,
            fullHeight: length === 0,
            gridExpandHeight: length === 0 ? 300 + _tables.reportLive.height() : 0,
            gridHeight: length === 0 ? 0 : ($(window).height() - 280) / 2
        };

        reportTable.removeClass("display-none").fanexGrid(gridOptions);
    }
}