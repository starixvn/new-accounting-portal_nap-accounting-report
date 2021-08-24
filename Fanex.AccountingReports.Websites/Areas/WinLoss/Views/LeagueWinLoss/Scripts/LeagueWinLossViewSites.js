Namespace.Register("Fanex.AccountingReports.WinLoss.LeagueWinLoss.LeagueWinLossViewSites");
Fanex.AccountingReports.WinLoss.LeagueWinLoss.LeagueWinLossViewSites = function () {
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
            SportName: null,
            MatchID: null,
            MatchEvent: '',
            LiveIndicator: false,
            SiteType: null,
            BetType: null,
            BetTypeName: null,
            IsNormalMode: true,
            IsNoDraw: false,
            SiteName: '',
            SortColumnName: '',
            SortDirection: '',
            SportType: 0
        }
    };
    self.initPage = function () {
        cachedDOMElements();
    };
    var cachedDOMElements = function () {
        _tables.reportContainer = $("#leagueWinLossSiteContainer");
        _tables.report = $("#leagueWinLossSite");
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
            url: window.rootUrl + "WinLoss/LeagueWinLoss/GetSites",
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

    var renderReport = function (report) {
        var liveStatus = self.Params.reportParams.LiveIndicator ? "Live" : "Non-Live";
        _tables.reportContainer.removeClass("display-none");
        _tables.report.addClass("display-none");
        var bodyData = [];
        var footerData = [];

        var len = report.Sites.length;

        if (len > 0) {
            var currentRow = null;
            var info = null;
            for (var i = 0; i < len; i++) {
                info = report.Sites[i];
                currentRow =
                {
                    Columns: [
                             {
                                 Name: "ColSite", Value: info, CssClass: "colBetType",
                                 CustomCellRender: function (currentCell) {
                                     var clickEvent = ' onclick = "viewSuper(' + currentCell.Value.SiteId
                                                 + ',\'' + currentCell.Value.SiteName
                                                 + '\'' + ");\"";
                                     var html = '<div class="link" ' + clickEvent + '>' + currentCell.Value.SiteName + '</div>';
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
                    SubRows: []
                };

                bodyData.push(currentRow);
            }

            info = report.Total;
            currentRow =
            {
                Columns: [{ Name: "ColSite", Value: info.SiteName, CssClass: "align-right" },
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
                    Columns: [{ Name: "ColSite", Value: info.SiteName, CssClass: "align-right", MergeColNumber: 6 },
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
                    Columns: [{ Name: "ColSite", Value: info.SiteName, CssClass: "align-right", MergeColNumber: 6 },
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
                     Columns: [{ Name: "ColSite", Value: info.SiteName, CssClass: "align-right", MergeColNumber: 6 },
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
                Columns: [{ HeaderText: "Site", Name: "ColSite", MergeRowNumber: 2 },
                          { HeaderText: "Member Win Loss", Name: "ColMemberWinLoss", MergeRowNumber: 2 },
                          { HeaderText: "SMA Win Loss", Name: "ColSMAWinLoss", MergeRowNumber: 2 },
                          { HeaderText: "Extra PT", Name: "ColExtraPT", MergeRowNumber: 2 },
                          { HeaderText: "Commission", Name: "ColCommision", MergeRowNumber: 2 },
                          { HeaderText: "House", Name: "ColVariance", MergeColNumber: 3 },
                          { HeaderText: "House Turnover", Name: "ColVirtualTurnOver", MergeColNumber: 2 },
                          { HeaderText: "Margin", Name: 'ColVirtualMargin', MergeColNumber: 2 }]
            }],
            columns: [{ HeaderText: "Site", Width: 200, Name: "ColSite", HeaderAlign: "center", CellAlign: "left" },
                    { HeaderText: "Member Win Loss", Width: 120, Name: "ColMemberWinLoss", HeaderAlign: "center", CellAlign: "right", FormatNumber: FormatConstant.TWO_DECIMAL_NUMBER, FormatPositive: '$' },
                    { HeaderText: "SMA Win Loss", Width: 120, Name: "ColSMAWinLoss", HeaderAlign: "center", CellAlign: "right", FormatNumber: FormatConstant.TWO_DECIMAL_NUMBER },
                    { HeaderText: "Extra PT", Width: 100, Name: "ColExtraPT", HeaderAlign: "center", CellAlign: "right", FormatNumber: FormatConstant.TWO_DECIMAL_NUMBER },
                    { HeaderText: "Commission", Width: 120, Name: "ColCommision", HeaderAlign: "center", CellAlign: "right", FormatNumber: FormatConstant.TWO_DECIMAL_NUMBER },
                    { HeaderText: "Variance", Width: 120, Name: "ColVariance", HeaderAlign: "center", CellAlign: "right", FormatNumber: FormatConstant.TWO_DECIMAL_NUMBER, FormatPositive: '$', Sort: true, SortDirection: getSortDirection("VarianceIncome") },
                    { HeaderText: "Virtual Win Loss", Width: 120, Name: "ColVirtualWinLoss", HeaderAlign: "center", CellAlign: "right", FormatNumber: FormatConstant.TWO_DECIMAL_NUMBER, FormatPositive: '$', Sort: true, SortDirection: getSortDirection("VirtualWinLoss") },
                    { HeaderText: "Real Income", Width: 120, Name: "ColRealIncome", HeaderAlign: "center", CellAlign: "right", FormatNumber: FormatConstant.TWO_DECIMAL_NUMBER, FormatPositive: '$', Sort: true, SortDirection: getSortDirection("RealIncome") },
                    { HeaderText: "Virtual", Width: 120, Name: "ColVirtualTurnOver", HeaderAlign: "center", CellAlign: "right", FormatNumber: FormatConstant.TWO_DECIMAL_NUMBER, Sort: true, SortDirection: getSortDirection("VirtualTurnover") },
                    { HeaderText: "Real", Width: 120, Name: "ColRealTurnOver", HeaderAlign: "center", CellAlign: "right", FormatNumber: FormatConstant.TWO_DECIMAL_NUMBER, Sort: true, SortDirection: getSortDirection("RealTurnover") },
                    { HeaderText: "Virtual", Width: 120, Name: "ColVirtualMargin", HeaderAlign: "center", CellAlign: "right", FormatNumber: FormatConstant.TWO_DECIMAL_PERCENT_NUMBER, FormatPositive: '$', Sort: true, SortDirection: getSortDirection("VirtualMargin") },
                    { HeaderText: "Real", Width: 120, Name: "ColRealMargin", HeaderAlign: "center", CellAlign: "right", FormatNumber: FormatConstant.TWO_DECIMAL_PERCENT_NUMBER, FormatPositive: '$', Sort: true, SortDirection: getSortDirection("RealMargin") }],
            showTitle: true,
            showFooter: true,
            titleInfo:
            {
                text: self.Params.reportParams.SportName + " - " +
                       self.Params.reportParams.LeagueName + " - " +
                       self.Params.reportParams.MatchEvent + " - " +
                       liveStatus + " - " +
                       self.Params.reportParams.BetTypeName + " - " +
                       self.Params.reportParams.SiteType,
                showExcelButton: true,
                exportExcelFunction: function () {
                    self.exportReport();
                },
                showBackButton: true,
                backFunction: function () {
                    backToBetTypes();
                },

                showCollapseExpandButton: false
            },
            showTree: false,
            bodyRows: bodyData,
            showDistinctRow: false,
            footerRows: footerData,
            gridExpandHeight: 300,
            sortOption:
           {
               serverSortFunction: function (sortColumnName, sortDirection) {
                   var sortColumnName = getSortColumnName(sortColumnName);
                   self.Params.reportParams.SortDirection = sortDirection.toLowerCase();
                   self.Params.reportParams.SortColumnName = sortColumnName;
                   self.sortReport();
               }
           },
        };

        _tables.report.removeClass("display-none").fanexGrid(gridOptions);
    };

    self.exportReport = function () {
        AccReport.downloadFile(window.rootUrl + "WinLoss/LeagueWinLoss/ExportLeagueWinLossViewSites", self.Params.reportParams);
    };

    self.isShowTotalRow = function (total) {
        return total.VirtualWinLoss != 0 || total.RealIncome != 0 || total.VirtualTurnover != 0 || total.RealTurnover != 0;
    };

    self.isShowGrandTotalRow = function (report) {
        return self.isShowTotalRow(report.RM)
        || self.isShowTotalRow(report.BARM);
    };

    var getSortColumnName = function (columnName) {
        var sortColumnName = null;
        switch (columnName) {
            case "ColVariance":
                sortColumnName = "VarianceIncome";
                break;
            case "ColVirtualWinLoss":
                sortColumnName = "VirtualWinLoss";
                break
            case "ColRealIncome":
                sortColumnName = "RealIncome";
                break;
            case "ColVirtualTurnOver":
                sortColumnName = "VirtualTurnover";
                break;
            case "ColRealTurnOver":
                sortColumnName = "RealTurnover";
                break;
            case "ColVirtualMargin":
                sortColumnName = "VirtualMargin";
                break;
            case "ColRealMargin":
                sortColumnName = "RealMargin";
                break;
        }

        return sortColumnName;
    };

    var getSortDirection = function (columnName) {
        return columnName == self.Params.reportParams.SortColumnName ? self.Params.reportParams.SortDirection : '';
    };

    self.sortReport = function () {
        AccReport.disableInput();
        AccReport.utility.CallAjax(this,
                                    window.rootUrl + "WinLoss/LeagueWinLoss/GetSites",
                                    self.Params.reportParams,
                                    renderReport,
                                    renderReport,
                                    AccReport.enableInput);
    };
}