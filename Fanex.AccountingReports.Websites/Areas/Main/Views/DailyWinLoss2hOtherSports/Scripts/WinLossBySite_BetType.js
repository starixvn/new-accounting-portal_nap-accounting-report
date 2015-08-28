WinLossBySite_BetType = function () {
    var self = this;

    self.Tables = {
        reportContainer: null,
        reportTable: null,
        reportTableBody: null
    };

    self.Params = {
        reportParams: {
            winlostdate: null,
            SportType: null,
            bettype: null,
            BetTypeName: null,
            liveindicator: null,
            IsHedge: null,
            site: null,
            PreviousBreadScrum: '',
            SportGroupId: 0
        }
    };

    self.CacheData = {
        PreviousBreadScrum: '',
        PlainPreviousBreadScrum: ''
    };
};

WinLossBySite_BetType.prototype.InitPage = function () {
    this.CachedDOMElements();
};

WinLossBySite_BetType.prototype.CachedDOMElements = function () {
    this.Tables.reportContainer = $('#winLossBySite_BetTypeContainer');
    this.Tables.report = $('#winLossBySite_BetType');
};

WinLossBySite_BetType.prototype.GetParams = function (selectParams) {
    var self = this;
    this.Params.reportParams.winlostdate = selectParams.WinLossDate;
    this.Params.reportParams.SportType = selectParams.SportType;
    if (self.Params.reportParams.SportType == AccReportData.Constants.SportType.SoccerMinorBetType && selectParams.SportTypeName != 'Basketball') {
        self.Params.reportParams.SportType = AccReportData.Constants.SportType.SoccerMajorBetType;
    }
    this.Params.reportParams.bettype = selectParams.BetType;
    this.Params.reportParams.BetTypeName = selectParams.BetTypeName;
    this.Params.reportParams.liveindicator = selectParams.IsLive;
    this.Params.reportParams.IsHedge = selectParams.IsHedge;
    this.Params.reportParams.site = selectParams.SiteType;
    this.Params.reportParams.SportGroupId = selectParams.SportGroupId;
    this.Params.reportParams.PreviousBreadScrum = self.CacheData.PlainPreviousBreadScrum;
};

WinLossBySite_BetType.prototype.GetReport = function (selectParams) {
    var self = this;
    AccReport.disableInput();
    self.GetParams(selectParams);
    AccReport.utility.CallAjax(this,
        window.rootUrl + "Main/DailyWinLoss2h/GetWinLossBySite_BetType",
        self.Params.reportParams,
        self.RenderReport,
        self.RenderReport,
        AccReport.enableInput);
};

WinLossBySite_BetType.prototype.RenderReport = function (report) {
    var self = this;
    this.Tables.reportContainer.removeClass("display-none");
    this.Tables.report.addClass("display-none");
    var bodyData = [];
    var footerData = [];
    var maxColIndex = 11;
    if (report && report.ChildsInfo && report.ChildsInfo.length > 0) {
        var reportLength = report.ChildsInfo.length;
        var info = null;
        var isShowVirtualTurnover = report.EnableVirtual ||
            this.Params.reportParams.SportGroupId == AccReportData.Constants.SportType.LiveCasino ||
            this.Params.reportParams.SportGroupId == AccReportData.Constants.SportType.VirtualSports;

        for (var i = 0; i < reportLength; i++) {
            var info = report.ChildsInfo[i];
            bodyData.push({
                Columns: [{ Name: "ColSite", Value: info.Name },
                            { Name: "ColWinloss", Value: info.WinLoss },
                            { Name: "ColPosition", Value: info.Position },
                            { Name: "ColCommission", Value: info.Commission },
                            { Name: "ColRealLineTotal", Value: info.LineTotal },
                            { Name: "ColVirtualLineTotal", Value: info.VirtualLineTotal },
                            { Name: "ColVariance", Value: info.Variance },
                            { Name: "ColRealTurnover", Value: info.IsHideRealTurnover ? viewResources.NotAvailable : info.Turnover },
                            { Name: "ColVirtualTurnover", Value: isShowVirtualTurnover ? info.VirtualTurnover : viewResources.NotAvailable },
                            reportManager.GetColumnTurnover(info.Margin, info.Turnover, "ColRealMargin"),
                            reportManager.GetColumnTurnover(info.VirtualMargin, info.VirtualTurnover, "ColVirtualMargin")],
                CssClass: i % 2 == 0 ? "freegrid-even" : ""
            });
        }

        info = report.GrandTotal;
        footerData.push({
            Columns: [{ Name: "ColSite", Value: info.Name, CellAlign: "right" },
                        { Name: "ColWinloss", Value: info.WinLoss },
                        { Name: "ColPosition", Value: info.Position },
                        { Name: "ColCommission", Value: info.Commission },
                        { Name: "ColRealLineTotal", Value: info.LineTotal, TdClass: "colNote" },
                        { Name: "ColVirtualLineTotal", Value: info.VirtualLineTotal, TdClass: "colNote" },
                        { Name: "ColVariance", Value: info.Variance, TdClass: "colNote" },
                        { Name: "ColRealTurnover", Value: info.IsHideRealTurnover ? viewResources.NotAvailable : info.Turnover },
                        { Name: "ColVirtualTurnover", Value: isShowVirtualTurnover ? info.VirtualTurnover : viewResources.NotAvailable },
                        reportManager.GetColumnTurnover(info.Margin, info.Turnover, "ColRealMargin"),
                        reportManager.GetColumnTurnover(info.VirtualMargin, info.VirtualTurnover, "ColVirtualMargin")]
        });
    }

    var reportName = report ? report.Name : reportManager.WinLossBySite_BetType.GetSiteTypeName(self.Params.reportParams.site);
    var gridOptions = {
        customizeHeader: true,
        groupHeaderRows: [
        {
            Columns: [{ HeaderText: viewResources.Site, Name: "ColSite" },
                      { HeaderText: viewResources.Member_WinLoss, Name: "ColWinloss", MergeRowNumber: 2 },
                      { HeaderText: viewResources.SMA_WinLoss, Name: "ColPosition", MergeRowNumber: 2 },
                      { HeaderText: viewResources.Commission, Name: "ColCommission", MergeRowNumber: 2 },
                      { HeaderText: viewResources.Company_Income, Name: "ColRealLineTotal", MergeColNumber: 2 },
                      { HeaderText: viewResources.Variance, Name: "ColVariance", MergeRowNumber: 2 },
                      { HeaderText: viewResources.Company_Turnover, Name: "ColRealTurnover", MergeColNumber: 2 },
                      { HeaderText: viewResources.Margin, Name: "ColRealMargin", MergeColNumber: 2}]
        }],

        columns: [
            {
                HeaderText: reportName,
                Width: 185, Name: "ColSite",
                HeaderAlign: "center",
                CellAlign: "left"
            },
            { HeaderText: viewResources.Member_WinLoss, Width: 100, Name: "ColWinloss", FormatNumber: FormatConstant.TWO_DECIMAL_NUMBER, HeaderAlign: "center", CellAlign: "right", FormatPositive: '$' },
            { HeaderText: viewResources.SMA_WinLoss, Width: 100, Name: "ColPosition", FormatNumber: FormatConstant.TWO_DECIMAL_NUMBER, HeaderAlign: "center", CellAlign: "right" },
            { HeaderText: viewResources.Commission, Width: 100, Name: "ColCommission", FormatNumber: FormatConstant.TWO_DECIMAL_NUMBER, HeaderAlign: "center", CellAlign: "right" },
            { HeaderText: viewResources.Real, Width: 100, Name: "ColRealLineTotal", FormatNumber: FormatConstant.TWO_DECIMAL_NUMBER, HeaderAlign: "center", CellAlign: "right", FormatPositive: '$' },
            { HeaderText: viewResources.Virtual, Width: 100, Name: "ColVirtualLineTotal", FormatNumber: FormatConstant.TWO_DECIMAL_NUMBER, HeaderAlign: "center", CellAlign: "right", FormatPositive: '$' },
            { HeaderText: viewResources.Variance, Width: 100, Name: "ColVariance", FormatNumber: FormatConstant.TWO_DECIMAL_NUMBER, HeaderAlign: "center", CellAlign: "right", FormatPositive: '$' },
            { HeaderText: viewResources.Real, Width: 100, Name: "ColRealTurnover", FormatNumber: FormatConstant.TWO_DECIMAL_NUMBER, HeaderAlign: "center", CellAlign: "right" },
            { HeaderText: viewResources.Virtual, Width: 100, Name: "ColVirtualTurnover", FormatNumber: FormatConstant.TWO_DECIMAL_NUMBER, HeaderAlign: "center", CellAlign: "right" },
            { HeaderText: viewResources.Real, Width: 100, Name: "ColRealMargin", FormatNumber: FormatConstant.TWO_DECIMAL_PERCENT_NUMBER, HeaderAlign: "center", CellAlign: "right", FormatPositive: '$' },
            { HeaderText: viewResources.Virtual, Width: 100, Name: "ColVirtualMargin", HeaderAlign: "center", FormatNumber: FormatConstant.TWO_DECIMAL_PERCENT_NUMBER, CellAlign: "right", FormatPositive: '$'}],
        showTitle: true,
        showFooter: true,
        titleInfo: {
            text: self.CacheData.PreviousBreadScrum + " |" +
                  " " + AccReport.utility.StringFormat(viewResources.DailyWinLoss2h_WinLossBySite_BetType_ReportTitle,
                            [reportName]),
            showExcelButton: false,
            exportExcelFunction: function () {
                self.ExportReport();
            }
        },
        bodyRows: bodyData,
        footerRows: footerData,
        numberOfFrozenColumn: 0,
        gridWidth: 1400,
        gridExpandHeight: 200,
        showDistinctRow: false,
        showHighlightRow: false,
        headerColumnHeight: 17,
        noDataText: viewResources.EmptyReportMessage,
        marginRight: 20
    };

    this.Tables.report.removeClass("display-none").fanexGrid(gridOptions);
};

WinLossBySite_BetType.prototype.ExportReport = function () {
    AccReport.downloadFile(window.rootUrl + "Main/DailyWinLoss2h/ExportWinLossBySite_BetType", this.Params.reportParams);
};

WinLossBySite_BetType.prototype.HideReportView = function () {
    this.Tables.reportContainer.addClass("display-none");
    reportManager.Tables.btnShowHideSiteTypeInfo.removeClass("display-none");
};

WinLossBySite_BetType.prototype.GetSiteTypeName = function (siteType) {
    siteType = parseInt(siteType);
    switch (siteType) {
        case AccReportData.Constants.SiteType.All_Site:
            return viewResources.All_Sites;
        case AccReportData.Constants.SiteType.Credit:
            return viewResources.Credit_Site;
        case AccReportData.Constants.SiteType.Deposit:
            return viewResources.Deposit_Site;
        default:
            return "";
    }
};