
Namespace.Register("Fanex.AccountingReports.Gaming.NumberGameWinLoss.WinLossByMatch");
Fanex.AccountingReports.Gaming.NumberGameWinLossByBetTypeMatch.WinLossByMatch.MainReport = function () {
    var self = this;

    self.Tables = {
        reportContainer: null,
        report: null
    };

    self.Params = {
        reportParams: {
            CompanyList: null,
            CurrencyList: null,
            BetTypeList: null,
            ReportDate: null,
            TrialData: false
        }
    };

    self.InitPage = function () {
        self.CachedDOMElements();
    };

    self.CachedDOMElements = function () {
        self.Tables.reportContainer = $("#winloss-bymatch-container");
        self.Tables.report = $("#winloss-bymatch-grid");
    };

    self.checkReportParams = function (params) {
        AccReport.popUp.removeHighLight();
        if (params.CompanyList == '') {
            AccReport.popUp.show("Please select  at least one company!", "dropdown-company");
            return false;
        }

        if (params.CurrencyList == '') {
            AccReport.popUp.show("Please select  at least one currency!", "dropdown-currency");
            return false;
        }

        if (params.BetTypeList == '') {
            AccReport.popUp.show("Please select  at least one bet type!", "dropdown-bettype");
            return false;
        }

        return true;
    };

    self.GetReport = function (params) {
        if (!self.checkReportParams(params)) {
            AccReport.enableInput();
            return;
        }

        self.Params.reportParams = params;
        AccReport.utility.CallAjax(this,
            window.rootUrl + "Gaming/NumberGameWinLossByBetTypeMatch/GetWinLossByMatchMainReport",
            self.Params.reportParams,
            self.RenderReport,
            self.RenderReport,
            AccReport.enableInput,
            false);
    };

    self.RenderReport = function (report) {
        self.Tables.reportContainer.removeClass("display-none");
        self.Tables.report.addClass("display-none");
        var bodyData = [];
        var footerData = [];
        if (report && report.DetailInfo && report.DetailInfo.length > 0) {
            var matchGroupsLength = report.DetailInfo.length;
            var info = null;
            for (var i = 0; i < matchGroupsLength; i++) {
                var matchGroup = report.DetailInfo[i];
                info = matchGroup.GrandTotal;
                var matchGroupRow = {
                    Columns: [{ Name: "ColName", Value: matchGroup.Name },
                        { Name: "ColCustomerWinLoss", Value: info.CustomerWinLoss },
                        { Name: "ColAgencyWinLoss", Value: info.AgencyWinLoss },
                        { Name: "ColCommission", Value: info.Commission },
                        { Name: "ColTurnover", Value: info.Turnover },
                        { Name: "ColRealLineTotal", Value: info.LineTotal },
                        { Name: "ColVirtualLineTotal", Value: info.VirtualLineTotal },
                        { Name: "ColVariance", Value: info.Variance },
                        reportManager.GetColumnMargin(info.Margin, info.Turnover, "ColMargin")],
                    SubRows: []
                };

                var liveIndicatorGroupsLength = matchGroup.DetailInfo.length;
                for (var j = 0; j < liveIndicatorGroupsLength; j++) {
                    var liveIndicatorGroup = matchGroup.DetailInfo[j];
                    info = liveIndicatorGroup.GrandTotal;
                    var liveIndicatorGroupRow = {
                        Columns: [{ Name: "ColName", Value: liveIndicatorGroup.Name },
                            { Name: "ColCustomerWinLoss", Value: info.CustomerWinLoss },
                            { Name: "ColAgencyWinLoss", Value: info.AgencyWinLoss },
                            { Name: "ColCommission", Value: info.Commission },
                            { Name: "ColTurnover", Value: info.Turnover },
                            { Name: "ColRealLineTotal", Value: info.LineTotal },
                            { Name: "ColVirtualLineTotal", Value: info.VirtualLineTotal },
                            { Name: "ColVariance", Value: info.Variance },
                            reportManager.GetColumnMargin(info.Margin, info.Turnover, "ColMargin")],
                        SubRows: []
                    };

                    var betTypesLength = liveIndicatorGroup.DetailInfo.length;
                    for (var k = 0; k < betTypesLength; k++) {
                        info = liveIndicatorGroup.DetailInfo[k];
                        var drillDownLink = "<span class='link' onclick=\"reportManager.WinLossByMatch.GetDetailReport('" +
                            info.Id + "','" +
                            info.Name + "','" +
                            liveIndicatorGroup.Id + "','" +
                            matchGroup.Id + "','" +
                            matchGroup.Name + "'); return;\">" +
                            info.Name + '</span>';

                        var betTypeRow = {
                            Columns: [{ Name: "ColName", Value: drillDownLink },
                                { Name: "ColCustomerWinLoss", Value: info.CustomerWinLoss },
                                { Name: "ColAgencyWinLoss", Value: info.AgencyWinLoss },
                                { Name: "ColCommission", Value: info.Commission },
                                { Name: "ColTurnover", Value: info.Turnover },
                                { Name: "ColRealLineTotal", Value: info.LineTotal },
                                { Name: "ColVirtualLineTotal", Value: info.VirtualLineTotal },
                                { Name: "ColVariance", Value: info.Variance },
                                reportManager.GetColumnMargin(info.Margin, info.Turnover, "ColMargin")]
                        };

                        liveIndicatorGroupRow.SubRows.push(betTypeRow);
                    }

                    matchGroupRow.SubRows.push(liveIndicatorGroupRow);
                }

                bodyData.push(matchGroupRow);
            }

            info = report.GrandTotal;
            footerData.push({
                Columns: [{ Name: "ColName", Value: info.Name, CssClass: "align-center" },
                    { Name: "ColCustomerWinLoss", Value: info.CustomerWinLoss },
                    { Name: "ColAgencyWinLoss", Value: info.AgencyWinLoss },
                    { Name: "ColCommission", Value: info.Commission },
                    { Name: "ColTurnover", Value: info.Turnover },
                    { Name: "ColRealLineTotal", Value: info.LineTotal },
                    { Name: "ColVirtualLineTotal", Value: info.VirtualLineTotal },
                    { Name: "ColVariance", Value: info.Variance },
                    reportManager.GetColumnMargin(info.Margin, info.Turnover, "ColMargin")]
            });
        }

        var gridOptions = {
            customizeHeader: true,
            groupHeaderRows: [{
                Columns: [
                    { HeaderText: viewResources.Match, Name: "ColName", MergeRowNumber: 2 },
                    { HeaderText: 'Member Win Loss', Name: "ColCustomerWinLoss", MergeRowNumber: 2 },
                    { HeaderText: 'SMA Win Loss', Name: "ColAgencyWinLoss", MergeRowNumber: 2 },
                    { HeaderText: viewResources.Commission, Name: "ColCommission", MergeRowNumber: 2 },
                    { HeaderText: 'House', Name: "ColTurnover", MergeColNumber: 5, HeaderAlign: "center" }
                ]
            }],
            columns: [{ HeaderText: viewResources.Match, Width: 140, Name: "ColName", HeaderAlign: "center", CellAlign: "left", ShowCollapseExpand: true },
                { HeaderText: 'Member Win Loss', Width: 120, Name: "ColCustomerWinLoss", HeaderAlign: "center", CellAlign: "right", FormatNumber: FormatConstant.TWO_DECIMAL_NUMBER, FormatPositive: '$' },
                { HeaderText: 'SMA Win Loss', Width: 120, Name: "ColAgencyWinLoss", HeaderAlign: "center", CellAlign: "right", FormatNumber: FormatConstant.TWO_DECIMAL_NUMBER },
                { HeaderText: viewResources.Commission, Width: 80, Name: "ColCommission", HeaderAlign: "center", CellAlign: "right", FormatNumber: FormatConstant.TWO_DECIMAL_NUMBER },
                { HeaderText: viewResources.Turnover, Width: 120, Name: "ColTurnover", HeaderAlign: "center", CellAlign: "right", FormatNumber: FormatConstant.TWO_DECIMAL_NUMBER },
                { HeaderText: 'Real Income', Width: 120, Name: "ColRealLineTotal", HeaderAlign: "center", CellAlign: "right", FormatNumber: FormatConstant.TWO_DECIMAL_NUMBER, FormatPositive: '$' },
                { HeaderText: 'Virtual Win Loss', Width: 120, Name: "ColVirtualLineTotal", HeaderAlign: "center", CellAlign: "right", FormatNumber: FormatConstant.TWO_DECIMAL_NUMBER, FormatPositive: '$' },
                { HeaderText: viewResources.Variance, Width: 80, Name: "ColVariance", HeaderAlign: "center", CellAlign: "right", FormatNumber: FormatConstant.TWO_DECIMAL_NUMBER, FormatPositive: '$' },
                { HeaderText: viewResources.Margin, Width: 80, Name: "ColMargin", HeaderAlign: "center", CellAlign: "right", FormatNumber: FormatConstant.TWO_DECIMAL_PERCENT_NUMBER, FormatPositive: '$'}],
            showTitle: true,
            showFooter: true,
            titleInfo: {
                text: AccReport.utility.StringFormat(
                    viewResources.NumberGameWinLossByBetTypeMatch_ByMatchMainReportTitle,
                    [self.Params.reportParams.ReportDate]),
                showExcelButton: true,
                exportExcelFunction: function () {
                    self.ExportReport();
                },
                showCollapseExpandButton: true
            },
            bodyRows: bodyData,
            footerRows: footerData,
            numberOfFrozenColumn: 0,
            gridWidth: 1400,
            gridExpandHeight: 172,
            fullWidth: true,
            fullHeight: true,
            showDistinctRow: false,
            showHighlightRow: true,
            headerColumnHeight: 17,
            showPager: false,
            showTree: true
        };

        self.Tables.report.removeClass("display-none").fanexGrid(gridOptions);
    };

    self.ExportReport = function () {
        AccReport.downloadFile(window.rootUrl + "Gaming/NumberGameWinLossByBetTypeMatch/ExportWinLossByMatchMainReport", self.Params.reportParams);
    };

    self.HideReport = function () {
        self.Tables.reportContainer.addClass("display-none");
    };
};