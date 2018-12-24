Namespace.Register("Fanex.AccountingReports.Gaming.NumberGameWinLossByBetTypeMatch.WinLossByBetType");
Fanex.AccountingReports.Gaming.NumberGameWinLossByBetTypeMatch.WinLossByBetType.DetailReport = function () {
    var self = this;

    self.Tables = {
        reportContainer: null,
        report: null,
        reportParentContainer: null
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
        self.Tables.reportContainer = $("#winloss-bybettype-detail-container");
        self.Tables.report = $("#winloss-bybettype-detail-grid");
        self.Tables.reportParentContainer = $("#parentContainer");
    };

    self.GetReport = function (params) {
        self.Params.reportParams = params;
        AccReport.utility.CallAjax(this,
            window.rootUrl + "Gaming/NumberGameWinLossByBetTypeMatch/GetWinLossByBetTypeDetailReport",
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
        var maxColumnIndex = 9;

        var reportName = report ? " - " + report.Name : '';
        if (report && report.DetailInfo && report.DetailInfo.length > 0) {
            var leagueLength = report.DetailInfo.length;
            var info = null;
            for (var i = 0; i < leagueLength; i++) {
                var leagueWinLoss = report.DetailInfo[i];
                bodyData.push({
                    Columns: [{
                        Name: "ColName",
                        Value: leagueWinLoss.Name,
                        MergeColNumber: 4,
                        CssClass: 'align-left bold'
                    },
                    {
                        Name: "ColTurnover",
                        MergeColNumber: 5
                    }]
                });

                var matchLength = leagueWinLoss.DetailInfo.length;
                for (var j = 0; j < matchLength; j++) {
                    info = leagueWinLoss.DetailInfo[j];
                    bodyData.push({
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
                    { HeaderText: "", Name: "ColName", MergeRowNumber: 2 },
                    { HeaderText: 'Member Win Loss', Name: "ColCustomerWinLoss", MergeRowNumber: 2 },
                    { HeaderText: 'SMA Win Loss', Name: "ColAgencyWinLoss", MergeRowNumber: 2 },
                    { HeaderText: viewResources.Commission, Name: "ColCommission", MergeRowNumber: 2 },
                    { HeaderText: 'House', Name: "ColTurnover", MergeColNumber: 5, HeaderAlign: "center" }
                ]
            }],
            columns: [{ HeaderText: "", Width: 100, Name: "ColName", HeaderAlign: "center", CellAlign: "center" },
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
                    viewResources.NumberGameWinLossByBetTypeMatch_ByBetTypeDetailReportTitle,
                    [reportName,
                    self.Params.reportParams.ReportDate]),
                showExcelButton: true,
                exportExcelFunction: function () {
                    self.ExportReport();
                }
            },
            bodyRows: bodyData,
            footerRows: footerData,
            numberOfFrozenColumn: 4,
            gridWidth: 1400,
            gridExpandHeight: 0,
            gridHeight: 450,
            fullWidth: true,
            fullHeight: false,
            showDistinctRow: true,
            showHighlightRow: true,
            headerColumnHeight: 17,
            showPager: false
        };

        self.Tables.report.removeClass("display-none").fanexGrid(gridOptions);
        self.Tables.reportContainer.dialog({
            title: "",
            autoOpen: false,
            height: 550,
            width: 1035,
            modal: true,
            resizable: false,
            close: function (event, ui) {
                self.Tables.reportContainer.dialog("destroy").remove();
                self.Tables.reportParentContainer.append(
                    '<div id="winloss-bybettype-detail-container" class="display-none content-grid width-100percent">\
                        <div id="winloss-bybettype-detail-grid" class="freegrid"></div>\
                    </div>');
                self.Tables.reportContainer = $("#winloss-bybettype-detail-container");
                self.Tables.report = $("#winloss-bybettype-detail-grid");
            }
        });

        self.Tables.reportContainer.dialog("open");
    };

    self.ExportReport = function () {
        AccReport.downloadFile(window.rootUrl + "Gaming/NumberGameWinLossByBetTypeMatch/ExportWinLossByBetTypeDetailReport", self.Params.reportParams);
    };

    self.HideReport = function () {
        self.Tables.reportContainer.addClass("display-none");
    };
};