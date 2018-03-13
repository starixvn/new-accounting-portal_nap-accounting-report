
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
            var info = null;
            for (var i = 0; i < reportLength; i++) {
                info = report.DetailInfo[i];

                var drillDownLink = "<span class='link' onclick=\"reportManager.WinLossByBetType.GetDetailReport('" +
                    info.Id + "','" +
                    info.Name + "'); return;\">" +
                    info.Name + '</span>';

                bodyData.push({
                    Columns: [{ Name: "ColName", Value: drillDownLink },
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

            info = report.GrandTotal;
            footerData.push({
                Columns: [{ Name: "ColName", Value: info.Name },
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
                    { HeaderText: viewResources.Bet_Type, Name: "ColName", MergeRowNumber: 2 },
                    { HeaderText: viewResources.Customer_Win_Loss, Name: "ColCustomerWinLoss", MergeRowNumber: 2 },
                    { HeaderText: viewResources.Agency_Win_Loss, Name: "ColAgencyWinLoss", MergeRowNumber: 2 },
                    { HeaderText: viewResources.Commission, Name: "ColCommission", MergeRowNumber: 2 },
                    { HeaderText: viewResources.Company, Name: "ColTurnover", MergeColNumber: 5, HeaderAlign: "center" }
                ]
            }],
            columns: [{ HeaderText: viewResources.Bet_Type, Width: 140, Name: "ColName", HeaderAlign: "center", CellAlign: "center" },
                { HeaderText: viewResources.Customer_Win_Loss, Width: 120, Name: "ColCustomerWinLoss", HeaderAlign: "center", CellAlign: "right", FormatNumber: FormatConstant.TWO_DECIMAL_NUMBER, FormatPositive: '$' },
                { HeaderText: viewResources.Agency_Win_Loss, Width: 120, Name: "ColAgencyWinLoss", HeaderAlign: "center", CellAlign: "right", FormatNumber: FormatConstant.TWO_DECIMAL_NUMBER },
                { HeaderText: viewResources.Commission, Width: 80, Name: "ColCommission", HeaderAlign: "center", CellAlign: "right", FormatNumber: FormatConstant.TWO_DECIMAL_NUMBER },
                { HeaderText: viewResources.Turnover, Width: 120, Name: "ColTurnover", HeaderAlign: "center", CellAlign: "right", FormatNumber: FormatConstant.TWO_DECIMAL_NUMBER },
                { HeaderText: viewResources.Line_Total, Width: 120, Name: "ColRealLineTotal", HeaderAlign: "center", CellAlign: "right", FormatNumber: FormatConstant.TWO_DECIMAL_NUMBER, FormatPositive: '$' },
                { HeaderText: viewResources.Virtual_Line_Total, Width: 120, Name: "ColVirtualLineTotal", HeaderAlign: "center", CellAlign: "right", FormatNumber: FormatConstant.TWO_DECIMAL_NUMBER, FormatPositive: '$' },
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
                }
            },
            bodyRows: bodyData,
            footerRows: footerData,
            numberOfFrozenColumn: 0,
            gridWidth: 1400,
            gridExpandHeight: 172,
            fullWidth: true,
            fullHeight: true,
            showDistinctRow: true,
            showHighlightRow: true,
            headerColumnHeight: 17,
            showPager: false
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