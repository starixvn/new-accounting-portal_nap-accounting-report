
Namespace.Register("Fanex.AccountingReports.Gaming.NumberGameWinLossByBetTypeMatch.WinLossByBetType");
Fanex.AccountingReports.Gaming.NumberGameWinLossByBetTypeMatch.WinLossByBetType.MainReport = function () {
    var self = this;

    self.Tables = {
        reportContainer: null,
        report: null
    };

    self.Params = {
        reportParams: {
            CompanyList: null,
            CurrencyList: null,
            LiveStatus: null,
            ReportDate: null,
            TrialData: false
        }
    };

    self.InitPage = function () {
        self.CachedDOMElements();
    };

    self.CachedDOMElements = function () {
        self.Tables.reportContainer = $("#winloss-bybettype-container");
        self.Tables.report = $("#winloss-bybettype-grid");
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

        return true;
    };

    self.GetReport = function (params) {        
        if (!self.checkReportParams(params)) {
            AccReport.enableInput();
            return;
        }


        self.Params.reportParams = params;
        AccReport.utility.CallAjax(this,
            window.rootUrl + "Gaming/NumberGameWinLossByBetTypeMatch/GetWinLossByBetTypeMainReport",
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
            var reportLength = report.DetailInfo.length;            
            for (var i = 0; i < reportLength; i++) {
                var sportData = report.DetailInfo[i];
                var bettypeRow = [];                
                for (var j = 0; j < sportData.DetailInfo.length; j++) {
                    var bettypeData = sportData.DetailInfo[j];
                    var drillDownLink = "<span class='link' onclick=\"reportManager.WinLossByBetType.GetDetailReport('" +
                    bettypeData.Id + "','" +
                    bettypeData.Name + "'); return;\">" +
                    bettypeData.Name + '</span>';

                    bettypeRow.push({
                        Columns: [{ Name: "ColName", Value: drillDownLink },
                            { Name: "ColCustomerWinLoss", Value: bettypeData.CustomerWinLoss },
                            { Name: "ColAgencyWinLoss", Value: bettypeData.AgencyWinLoss },
                            { Name: "ColCommission", Value: bettypeData.Commission },
                            { Name: "ColTurnover", Value: bettypeData.Turnover },
                            { Name: "ColRealLineTotal", Value: bettypeData.LineTotal },
                            { Name: "ColVirtualLineTotal", Value: bettypeData.VirtualLineTotal },
                            { Name: "ColVariance", Value: bettypeData.Variance },
                            reportManager.GetColumnMargin(bettypeData.Margin, bettypeData.Turnover, "ColMargin")],                        
                    });
                }
                
                var sportTotal = sportData.GrandTotal;
                bodyData.push({
                    Columns: [{ Name: "ColName", Value: sportTotal.Name },
                        { Name: "ColCustomerWinLoss", Value: sportTotal.CustomerWinLoss },
                        { Name: "ColAgencyWinLoss", Value: sportTotal.AgencyWinLoss },
                        { Name: "ColCommission", Value: sportTotal.Commission },
                        { Name: "ColTurnover", Value: sportTotal.Turnover },
                        { Name: "ColRealLineTotal", Value: sportTotal.LineTotal },
                        { Name: "ColVirtualLineTotal", Value: sportTotal.VirtualLineTotal },
                        { Name: "ColVariance", Value: sportTotal.Variance },
                        reportManager.GetColumnMargin(sportTotal.Margin, sportTotal.Turnover, "ColMargin")],
                    SubRows: bettypeRow
                });
            }

            var grandTotal = report.GrandTotal;
            footerData.push({
                Columns: [{ Name: "ColName", Value: grandTotal.Name },
                    { Name: "ColCustomerWinLoss", Value: grandTotal.CustomerWinLoss },
                    { Name: "ColAgencyWinLoss", Value: grandTotal.AgencyWinLoss },
                    { Name: "ColCommission", Value: grandTotal.Commission },
                    { Name: "ColTurnover", Value: grandTotal.Turnover },
                    { Name: "ColRealLineTotal", Value: grandTotal.LineTotal },
                    { Name: "ColVirtualLineTotal", Value: grandTotal.VirtualLineTotal },
                    { Name: "ColVariance", Value: grandTotal.Variance },
                    reportManager.GetColumnMargin(grandTotal.Margin, grandTotal.Turnover, "ColMargin")]
            });
        }

        var gridOptions = {
            customizeHeader: true,
            groupHeaderRows: [{
                Columns: [
                    { HeaderText: viewResources.Bet_Type, Name: "ColName", MergeRowNumber: 2 },
                    { HeaderText: 'Member Win Loss', Name: "ColCustomerWinLoss", MergeRowNumber: 2 },
                    { HeaderText: 'SMA Win Loss', Name: "ColAgencyWinLoss", MergeRowNumber: 2 },
                    { HeaderText: viewResources.Commission, Name: "ColCommission", MergeRowNumber: 2 },
                    { HeaderText: 'House', Name: "ColTurnover", MergeColNumber: 5, HeaderAlign: "center" }
                ]
            }],
            columns: [{ HeaderText: viewResources.Bet_Type, Width: 140, Name: "ColName", HeaderAlign: "center", CellAlign: "left", ShowCollapseExpand: true },
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
                    viewResources.NumberGameWinLossByBetTypeMatch_ByBetTypeMainReportTitle,
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
        AccReport.downloadFile(window.rootUrl + "Gaming/NumberGameWinLossByBetTypeMatch/ExportWinLossByBetTypeMainReport", self.Params.reportParams);
    };

    self.HideReport = function () {
        self.Tables.reportContainer.addClass("display-none");
    };
};