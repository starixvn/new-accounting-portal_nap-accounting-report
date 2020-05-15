Namespace.Register("Fanex.AccountingReports.WinLoss.LeagueWinLoss.LeagueWinLossByBetType");
Fanex.AccountingReports.WinLoss.LeagueWinLoss.LeagueWinLossByBetType = function () {
    var self = this;
    var _tables = {
        reportContainer: null,
        report: null
    };

    self.Params = {
        reportParams: {
            StartDate: null,
            EndDate: null,
            LeagueId: null,
            SportType: null,
            BetType: null,
            BetTypeName: null,
            LiveIndicator: null,
            IsHedge: null,
            LeagueName: '',
            SportName: '',
            IsNormalMode: true
        }
    };
    self.initPage = function () {
        cachedDOMElements();
    };
    var cachedDOMElements = function () {
        _tables.reportParentContainer = $("#parentContainer");
        _tables.reportContainer = $('#leagueWinLossReportByBetTypeContainer');
        _tables.report = $('#leagueWinLossReportByBetType');
    };
    self.getReport = function (params) {
        AccReport.disableInput();
        getParams(params);
        $.ajax({
            type: "POST",
            url: window.rootUrl + "WinLoss/LeagueWinLoss/GetLeagueWinLossByBetType",
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            data: JSON.stringify(self.Params.reportParams),
            beforeSend: function () {
                AccReport.disableInput();
            },
            success: function (rs) {
                renderReport(rs);
            },
            error: function (event, jqXHR, ajaxSettings) {
                renderReport(null);
            },
            complete: function () {
                AccReport.enableInput();
            }
        });
    };
    var getParams = function (params) {
        self.Params.reportParams = params;
    };
    var renderReport = function (report) {
        var liveStr = self.Params.reportParams.LiveIndicator == true ? "Live" : "Non-Live";
        _tables.reportContainer.removeClass("display-none");
        _tables.report.addClass("display-none");
        var bodyData = [];
        var footerData = [];

        if (report.Match.length > 0) {
            var len = report.Match.length;
            var currentRow = null;
            var info = null;
            for (var i = 0; i < len; i++) {
                info = report.Match[i];
                currentRow =
                {
                    Columns: [{ Name: "ColCode", Value: info.MatchCode },
                            { Name: "ColEvent", Value: info.MatchEvent },
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
            }

            info = report.Total;
            currentRow =
            {
                Columns: [{ Name: "ColCode", Value: info.MatchEvent, CssClass: "align-right", MergeColNumber: 2 },
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

            footerData.push(currentRow);


            if (self.isShowTotalRow(report.RM)) {
                info = report.RM;
                currentRow =
                {
                    Columns: [{ Name: "ColCode", Value: info.MatchEvent, CssClass: "align-right", MergeColNumber: 7 },
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
                    Columns: [{ Name: "ColCode", Value: info.MatchEvent, CssClass: "align-right", MergeColNumber: 7 },
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
                     Columns: [{ Name: "ColCode", Value: info.MatchEvent, CssClass: "align-right", MergeColNumber: 7 },
                                { Name: "ColVirtualWinLoss", Value: totalInfo.VirtualWinLoss },
                                { Name: "ColRealIncome", Value: info.RealIncome },
                                { Name: "ColVirtualTurnOver", Value: totalInfo.VirtualTurnover },
                                { Name: "ColRealTurnOver", Value: totalInfo.RealTurnover },
                                { Name: "ColVirtualMargin", Value: "", MergeColNumber: 2 }]
                 };

                footerData.push(currentRow);
            }
        }

        var gridOptions = {
            customizeHeader: true,
            groupHeaderRows: [{
                Columns: [{ HeaderText: "Code", Name: "ColCode", MergeRowNumber: 2 },
                          { HeaderText: "Event", Name: "ColEvent", MergeRowNumber: 2 },
                          { HeaderText: "Member Win Loss", Name: "ColMemberWinLoss", MergeRowNumber: 2 },
                          { HeaderText: "SMA Win Loss", Name: "ColSMAWinLoss", MergeRowNumber: 2 },
                          { HeaderText: "Extra PT", Name: "ColExtraPT", MergeRowNumber: 2 },
						  { HeaderText: "Commission", Name: "ColCommision", MergeRowNumber: 2 },
						  { HeaderText: "House", Name: "ColVariance", MergeColNumber: 3 },
						  { HeaderText: "House Turnover", Name: "ColVirtualTurnOver", MergeColNumber: 2 },
						  { HeaderText: "Margin", Name: 'ColVirtualMargin', MergeColNumber: 2 }]
            }],
            columns: [{ HeaderText: "Code", Width: 100, Name: "ColCode", HeaderAlign: "center", CellAlign: "center" },
                    { HeaderText: "Event", Width: 200, Name: "ColEvent", HeaderAlign: "center", CellAlign: "center" },
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
                      self.Params.reportParams.LeagueName + " - " + self.Params.reportParams.BetTypeName + " - " + liveStr,
                showExcelButton: true,
                exportExcelFunction: function () {
                    self.exportReport();
                },
                showBackButton: true,
                backFunction: function () {
                    backToLeagueWinLossBySport();
                },

                showCollapseExpandButton: false
            },
            showTree: false,
            bodyRows: bodyData,
            showDistinctRow: false,
            footerRows: footerData,
            gridExpandHeight: 300
        };

        _tables.report.removeClass("display-none").fanexGrid(gridOptions);
    };
    self.hideReport = function () {
        _tables.reportContainer.addClass("display-none");
    };
    self.exportReport = function () {
        AccReport.downloadFile(window.rootUrl + "WinLoss/LeagueWinLoss/ExportLeagueWinLossByBetType", self.Params.reportParams);
    };

    self.isShowTotalRow = function (total) {
        return total.VirtualWinLoss != 0 || total.RealIncome != 0 || total.VirtualTurnover != 0 || total.RealTurnover != 0;
    };

    self.isShowGrandTotalRow = function (report) {
        return self.isShowTotalRow(report.RM)
        || self.isShowTotalRow(report.BARM);
    }
}