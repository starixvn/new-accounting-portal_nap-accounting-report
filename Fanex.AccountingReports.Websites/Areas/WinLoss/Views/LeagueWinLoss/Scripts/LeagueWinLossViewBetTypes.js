Namespace.Register("Fanex.AccountingReports.WinLoss.LeagueWinLoss.LeagueWinLossViewBetTypes");
Fanex.AccountingReports.WinLoss.LeagueWinLoss.LeagueWinLossViewBetTypes = function () {
    var self = this;
    var _tables = {
        reportContainer: null,
        report: null
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
            LiveIndicator: false,
            IsNormalMode: true,
            IsNoDraw: false,
            SiteName: ''
        }
    };
    self.initPage = function () {
        cachedDOMElements();
    };
    var cachedDOMElements = function () {
        _tables.reportContainer = $("#leagueWinLossBetTypeContainer");
        _tables.report = $("#leagueWinLossBetType");
    };
    self.hideReport = function () {
        _tables.reportContainer.addClass("display-none");
        _tables.report.addClass("display-none");
    };
    self.showReport = function () {
        _tables.report.removeClass("display-none");
        _tables.reportContainer.removeClass("display-none");
    };
    self.getReport = function (params) {
        AccReport.disableInput();
        getParams(params);
        $.ajax({
            type: "POST",
            url: window.rootUrl + "WinLoss/LeagueWinLoss/GetBetTypes",
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            data: JSON.stringify(self.Params.reportParams),
            beforeSend: function () {
                AccReport.disableInput();
            },
            success: function (rs) {
                self.hideReport();
                renderReport(rs);
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

    var showBetType = function (info, bodyData) {
        var bettypeTotal = info.Total;
        var currentRow = {
            Columns: [
                { Name: "ColBetType", Value: bettypeTotal.BettypeName },
                { Name: "ColMemberWinLoss", Value: bettypeTotal.MemberWinLoss },
                { Name: "ColSMAWinLoss", Value: bettypeTotal.SMAWinLoss },
                { Name: "ColExtraPT", Value: bettypeTotal.ExtraPT },
                { Name: "ColCommision", Value: bettypeTotal.Commission },
                { Name: "ColVariance", Value: bettypeTotal.VarianceIncome },
                { Name: "ColVirtualWinLoss", Value: bettypeTotal.VirtualWinLoss },
                { Name: "ColRealIncome", Value: bettypeTotal.RealIncome },
                { Name: "ColVirtualTurnOver", Value: bettypeTotal.VirtualTurnover },
                { Name: "ColRealTurnOver", Value: bettypeTotal.RealTurnover },
                { Name: "ColVirtualMargin", Value: bettypeTotal.VirtualMargin },
                { Name: "ColRealMargin", Value: bettypeTotal.RealMargin }],
            SubRows: []
        };

        var bettypesLength = info.BetTypes.length;

        if (bettypesLength > 0) {
            for (var j = 0; j < bettypesLength; j++) {
                var bettypeInfo = info.BetTypes[j];

                var currentSubRow = {
                    Columns: [
                        {
                            Name: "ColBetType", Value: bettypeInfo, CssClass: "colBetType",
                            CustomCellRender: function (currentCell) {
                                var clickEvent = ' onclick = "viewSite(' + currentCell.Value.Bettype
                                            + ',\'' + currentCell.Value.BettypeName
                                            + '\',\'' + currentCell.Value.SiteType
                                            + '\'' + ");\"";
                                var html = '<div class="link" ' + clickEvent + '>' + currentCell.Value.SiteType + '</div>';
                                return html;
                            }
                        },
                        { Name: "ColMemberWinLoss", Value: bettypeInfo.MemberWinLoss },
                        { Name: "ColSMAWinLoss", Value: bettypeInfo.SMAWinLoss },
                        { Name: "ColExtraPT", Value: bettypeInfo.ExtraPT },
                        { Name: "ColCommision", Value: bettypeInfo.Commission },
                        { Name: "ColVariance", Value: bettypeInfo.VarianceIncome },
                        { Name: "ColVirtualWinLoss", Value: bettypeInfo.VirtualWinLoss },
                        { Name: "ColRealIncome", Value: bettypeInfo.RealIncome },
                        { Name: "ColVirtualTurnOver", Value: bettypeInfo.VirtualTurnover },
                        { Name: "ColRealTurnOver", Value: bettypeInfo.RealTurnover },
                        { Name: "ColVirtualMargin", Value: bettypeInfo.VirtualMargin },
                        { Name: "ColRealMargin", Value: bettypeInfo.RealMargin }],
                };

                currentRow.SubRows.push(currentSubRow);
            }
        }

        bodyData.push(currentRow);
    };

    var renderReport = function (report) {
        var liveStatus = self.Params.reportParams.LiveIndicator ? "Live" : "Non-Live";
        _tables.reportContainer.removeClass("display-none");
        _tables.report.addClass("display-none");
        var bodyData = [];
        var footerData = [];
        var info = null;
        if (report.BettypeWinLoss.length > 0) {
            var len = report.BettypeWinLoss.length;
            for (var i = 0; i < len; i++) {
                info = report.BettypeWinLoss[i];
                showBetType(info, bodyData);
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
                           { Name: "ColRealIncome", Value: totalInfo.RealIncome },
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
                      self.Params.reportParams.MatchEvent + " - " +
                      liveStatus,
                showExcelButton: true,
                exportExcelFunction: function () {
                    self.exportReport();
                },

                showBackButton: true,
                backFunction: function () {
                    backToMatches();
                },

                showCollapseExpandButton: true
            },
            showTree: true,
            bodyRows: bodyData,
            showDistinctRow: false,
            footerRows: footerData,
            gridExpandHeight: 300
        };

        _tables.report.removeClass("display-none").fanexGrid(gridOptions);
    }
    self.exportReport = function () {
        AccReport.downloadFile(window.rootUrl + "WinLoss/LeagueWinLoss/ExportLeagueWinLossViewBetTypes", self.Params.reportParams);
    };

    self.isShowTotalRow = function (total) {
        return total.VirtualWinLoss != 0 || total.RealIncome != 0 || total.VirtualTurnover != 0 || total.RealTurnover != 0;
    };

    self.isShowGrandTotalRow = function (report) {
        return self.isShowTotalRow(report.RM)
        || self.isShowTotalRow(report.BARM);
    };
}