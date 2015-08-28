NumberGameReport_Summary = function () {
    var self = this;
    self.Params = {
        reportParams: {
            Winlostdate: null,
            SportType: null
        }
    };

    self.Tables = {
        numberGameReportContainer: null,
        nonLiveNumberGameReportContainer: null,
        nonLiveNumberGameReport: null,
        liveNumberGameReportContainer: null,
        liveNumberGameReport: null
    };

    self.CacheData = {
        parentController: null
    };
};

NumberGameReport_Summary.prototype.InitPage = function (parentController) {
    this.CachedDOMElements();
    this.CacheData.parentController = parentController;
};

NumberGameReport_Summary.prototype.CachedDOMElements = function () {
    this.Tables.numberGameReportContainer = $(".summaryNumberGameReportContainer");
    this.Tables.nonLiveNumberGameReportContainer = $(".summaryNonLiveNumberGameReportContainer");
    this.Tables.nonLiveNumberGameReport = $(".summaryNonLiveNumberGameReport");
    this.Tables.liveNumberGameReportContainer = $(".summaryLiveNumberGameReportContainer");
    this.Tables.liveNumberGameReport = $(".summaryLiveNumberGameReport");
};

NumberGameReport_Summary.prototype.GetReport = function (params) {
    var self = this;
    AccReport.disableInput();
    self.Params.reportParams = params;
    AccReport.utility.CallAjax(this,
        window.rootUrl + "Main/DailyWinLoss2h/GetSummaryNumberGameReport",
        self.Params.reportParams,
        self.RenderLiveAndNonLiveReport,
        self.RenderLiveAndNonLiveReport,
        AccReport.enableInput);
};

NumberGameReport_Summary.prototype.RenderLiveAndNonLiveReport = function (report) {
    if (report) {
        this.RenderNonLiveReport(report.NoneLiveReport);
        this.RenderLiveReport(report.LiveReport);
    } else {
        this.RenderNonLiveReport(null);
        this.RenderLiveReport(null);
    }
};

NumberGameReport_Summary.prototype.RenderNonLiveReport = function (report) {
    var self = this;
    this.Tables.nonLiveNumberGameReportContainer.addClass("display-none");
    var gridOptions = self.GetGridOptions_SummaryReport(report, false);
    this.Tables.nonLiveNumberGameReportContainer.removeClass("display-none");
    this.Tables.numberGameReportContainer.removeClass("display-none");
    self.CacheData.parentController.ShowReportView();
    this.Tables.nonLiveNumberGameReport.removeClass("display-none").fanexGrid(gridOptions);
    this.Tables.nonLiveNumberGameReport.find(".showHideSiteType.showHideIcon").click(function () {
        var btnIcon = $(this);
        showHideSiteTypeManager.ShowHideSiteTypeInfo_ByGroup(btnIcon, self.Tables.nonLiveNumberGameReport);
    });
};

NumberGameReport_Summary.prototype.RenderLiveReport = function (report) {
    var self = this;
    this.Tables.liveNumberGameReportContainer.addClass("display-none");
    var gridOptions = self.GetGridOptions_SummaryReport(report, true);
    this.Tables.liveNumberGameReportContainer.removeClass("display-none");
    this.Tables.numberGameReportContainer.removeClass("display-none");
    self.CacheData.parentController.ShowReportView();
    this.Tables.liveNumberGameReport.removeClass("display-none").fanexGrid(gridOptions);
    this.Tables.liveNumberGameReport.find(".showHideSiteType.showHideIcon").click(function () {
        var btnIcon = $(this);
        showHideSiteTypeManager.ShowHideSiteTypeInfo_ByGroup(btnIcon, self.Tables.liveNumberGameReport);
    });
};

NumberGameReport_Summary.prototype.GetReportLink = function (info, isLive) {
    var evt = "reportManager.NumberGameReport.GetReport_ByBetType('" + info.Id + "','" +
                    info.Name + "','" +
                    info.AliasName + "'," +
                    isLive + "," +
                    (info.Hedge && info.Hedge != '' ? true : false) + "); return;";
    var reportLink = '<b>\
                          <a class="report2h-link" onclick="' + evt + '">' + info.Name + '</a>\
                      </b>';
    return reportLink;
};

NumberGameReport_Summary.prototype.GetGridOptions_SummaryReport = function (report, isLive) {
    var self = this;
    var bodyData = [];
    var footerData = [];
    var maxColIndex = 11;

    if (report && report.ChildsInfo && report.ChildsInfo.length > 0) {
        var reportLength = report.ChildsInfo.length;
        var info = null;
        var groupdId = '';
        var tableId = isLive ? "summaryLiveNumberGameReport" : "summaryNonLiveNumberGameReport";
        for (var i = 0; i < reportLength; i++) {
            var betType = report.ChildsInfo[i];
            groupdId = tableId + "_BetType" + betType.Id +
                                 "_Hedge" + (betType.Hedge && betType.Hedge != "" ? 0 : 1) +
                                 "_Live" + (isLive ? 1 : 0);
            info = betType.GrandTotal;
            bodyData.push({
                Columns: [
                    {
                        Name: "ColBetType",
                        Value: showHideSiteTypeManager.GetShowHideSiteTypeIcon(groupdId) +
                            this.GetReportLink(info, report.IsLive),
                        CellAlign: "left",
                        CssClass: "bold"
                    },
                    { Name: "ColWinloss", Value: info.WinLoss },
                    { Name: "ColPosition", Value: info.Position },
                    { Name: "ColCommission", Value: info.Commission },
                    { Name: "ColRealLineTotal", Value: info.LineTotal },
                    { Name: "ColVirtualLineTotal", Value: info.VirtualLineTotal },
                    { Name: "ColVariance", Value: info.Variance },
                    { Name: "ColRealTurnover", Value: info.Turnover },
                    { Name: "ColVirtualTurnover", Value: info.VirtualTurnover },
                    reportManager.GetColumnTurnover(info.Margin, info.Turnover, "ColRealMargin"),
                    reportManager.GetColumnTurnover(info.VirtualMargin, info.VirtualTurnover, "ColVirtualMargin")],
                CssClass: "freegrid-even"
            });

            info = betType.CreditTotal;
            bodyData.push({
                Columns: [
                    {
                        Name: "ColBetType",
                        Value: reportManager.GetWinlossBySiteLink({
                            SportType: self.CacheData.parentController.CacheData.SportId,
                            SportGroupId: self.CacheData.parentController.CacheData.SportId,
                            WinLossDate: self.Params.reportParams.Winlostdate,
                            BetType: betType.Id,
                            BetTypeName: betType.AliasName,
                            IsLive: report.IsLive,
                            IsHedge: betType.Hedge && betType.Hedge != "" ? true : false,
                            SiteType: AccReportData.Constants.SiteType.Credit,
                            SiteTypeName: info.Name
                        }),
                        CellAlign: "right",
                        CssClass: "bold"
                    },
                    { Name: "ColWinloss", Value: info.WinLoss },
                    { Name: "ColPosition", Value: info.Position },
                    { Name: "ColCommission", Value: info.Commission },
                    { Name: "ColRealLineTotal", Value: info.LineTotal },
                    { Name: "ColVirtualLineTotal", Value: info.VirtualLineTotal },
                    { Name: "ColVariance", Value: info.Variance },
                    { Name: "ColRealTurnover", Value: info.Turnover },
                    { Name: "ColVirtualTurnover", Value: info.VirtualTurnover },
                    reportManager.GetColumnTurnover(info.Margin, info.Turnover, "ColRealMargin"),
                    reportManager.GetColumnTurnover(info.VirtualMargin, info.VirtualTurnover, "ColVirtualMargin")],
                CssClass: showHideSiteTypeManager.GetSiteTypeRowCss(groupdId,
                    showHideSiteTypeManager.Enums.RowType.InfoRow)
            });

            info = betType.DepositTotal;
            bodyData.push({
                Columns: [
                    {
                        Name: "ColBetType",
                        Value: reportManager.GetWinlossBySiteLink({
                            SportType: self.CacheData.parentController.CacheData.SportId,
                            SportGroupId: self.CacheData.parentController.CacheData.SportId,
                            WinLossDate: self.Params.reportParams.Winlostdate,
                            BetType: betType.Id,
                            BetTypeName: betType.AliasName,
                            IsLive: report.IsLive,
                            IsHedge: betType.Hedge && betType.Hedge != "" ? true : false,
                            SiteType: AccReportData.Constants.SiteType.Deposit,
                            SiteTypeName: info.Name
                        }),
                        CellAlign: "right",
                        CssClass: "bold"
                    },
                    { Name: "ColWinloss", Value: info.WinLoss },
                    { Name: "ColPosition", Value: info.Position },
                    { Name: "ColCommission", Value: info.Commission },
                    { Name: "ColRealLineTotal", Value: info.LineTotal },
                    { Name: "ColVirtualLineTotal", Value: info.VirtualLineTotal },
                    { Name: "ColVariance", Value: info.Variance },
                    { Name: "ColRealTurnover", Value: info.Turnover },
                    { Name: "ColVirtualTurnover", Value: info.VirtualTurnover },
                    reportManager.GetColumnTurnover(info.Margin, info.Turnover, "ColRealMargin"),
                    reportManager.GetColumnTurnover(info.VirtualMargin, info.VirtualTurnover, "ColVirtualMargin")],
                CssClass: showHideSiteTypeManager.GetSiteTypeRowCss(groupdId,
                    showHideSiteTypeManager.Enums.RowType.InfoRow)
            });
        }

        info = report.GrandTotal;
        groupdId = tableId + "_BetType0_Hedge0" +
                             "_Live" + (isLive ? 1 : 0);
        footerData.push({
            Columns: [
                {
                    Name: "ColBetType",
                    Value: showHideSiteTypeManager.GetShowHideSiteTypeIcon(groupdId) +
                           '<span class="right float-right">' + info.Name + '</span>'
                },
                { Name: "ColWinloss", Value: info.WinLoss },
                { Name: "ColPosition", Value: info.Position },
                { Name: "ColCommission", Value: info.Commission },
                { Name: "ColRealLineTotal", Value: info.LineTotal },
                { Name: "ColVirtualLineTotal", Value: info.VirtualLineTotal },
                { Name: "ColVariance", Value: info.Variance },
                { Name: "ColRealTurnover", Value: info.Turnover },
                { Name: "ColVirtualTurnover", Value: info.VirtualTurnover },
                reportManager.GetColumnTurnover(info.Margin, info.Turnover, "ColRealMargin"),
                reportManager.GetColumnTurnover(info.VirtualMargin, info.VirtualTurnover, "ColVirtualMargin")]
        });

        info = report.CreditTotal;
        footerData.push({
            Columns: [{ Name: "ColBetType", Value: info.Name, CellAlign: "right", CssClass: "bold" },
                          { Name: "ColWinloss", Value: info.WinLoss },
                          { Name: "ColPosition", Value: info.Position },
                          { Name: "ColCommission", Value: info.Commission },
                          { Name: "ColRealLineTotal", Value: info.LineTotal },
                          { Name: "ColVirtualLineTotal", Value: info.VirtualLineTotal },
                          { Name: "ColVariance", Value: info.Variance },
                          { Name: "ColRealTurnover", Value: info.Turnover },
                          { Name: "ColVirtualTurnover", Value: info.VirtualTurnover },
                          reportManager.GetColumnTurnover(info.Margin, info.Turnover, "ColRealMargin"),
                          reportManager.GetColumnTurnover(info.VirtualMargin, info.VirtualTurnover, "ColVirtualMargin")],
            CssClass: showHideSiteTypeManager.GetSiteTypeRowCss(groupdId,
                    showHideSiteTypeManager.Enums.RowType.InfoRow)
        });

        info = report.DepositTotal;
        footerData.push({
            Columns: [{ Name: "ColBetType", Value: info.Name, CellAlign: "right", CssClass: "bold" },
                          { Name: "ColWinloss", Value: info.WinLoss },
                          { Name: "ColPosition", Value: info.Position },
                          { Name: "ColCommission", Value: info.Commission },
                          { Name: "ColRealLineTotal", Value: info.LineTotal },
                          { Name: "ColVirtualLineTotal", Value: info.VirtualLineTotal },
                          { Name: "ColVariance", Value: info.Variance },
                          { Name: "ColRealTurnover", Value: info.Turnover },
                          { Name: "ColVirtualTurnover", Value: info.VirtualTurnover },
                          reportManager.GetColumnTurnover(info.Margin, info.Turnover, "ColRealMargin"),
                          reportManager.GetColumnTurnover(info.VirtualMargin, info.VirtualTurnover, "ColVirtualMargin")],
            CssClass: showHideSiteTypeManager.GetSiteTypeRowCss(groupdId,
                    showHideSiteTypeManager.Enums.RowType.InfoRow)
        });
    }

    var gridOptions = {
        customizeHeader: true,
        groupHeaderRows: [{
            Columns: [{ HeaderText: viewResources.Bet_Type, Name: "ColBetType", MergeRowNumber: 2 },
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
                HeaderText: viewResources.Bet_Type,
                Width: 170, Name: "ColBetType",
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
            text: "<a class='breadcrumbs-link' onclick='showHideReportManager.BackToMainReport();'>" + viewResources.Main + "</a> > " +
                        self.CacheData.parentController.CacheData.SportName +
                         " \"" + (isLive ? viewResources.Live : viewResources.Symbol_Sparse_Non_Live) + "\"" +
                         " " + viewResources.DailyWinLoss2h_ReportTitleSuffix,
            showExcelButton: false
        },
        bodyRows: bodyData,
        footerRows: footerData,
        numberOfFrozenColumn: 0,
        gridWidth: 1400,
        gridExpandHeight: 180,
        fullWidth: true,
        fullHeight: false,
        gridHeight: reportManager.CachedData.maxGridHeight,
        showDistinctRow: false,
        showHighlightRow: false,
        noDataText: viewResources.EmptyReportMessage,
        headerColumnHeight: 17
    };
    return gridOptions;
};

NumberGameReport_Summary.prototype.ShowReport = function () {
    AccReport.popUp.hide();
    this.Tables.numberGameReportContainer.removeClass("display-none");
};

NumberGameReport_Summary.prototype.HideReport = function () {
    AccReport.popUp.hide();
    this.Tables.numberGameReportContainer.addClass("display-none");
};

NumberGameReport_Summary.prototype.ExportReport = function () {
    AccReport.downloadFile(window.rootUrl + "Main/DailyWinLoss2h/ExportNumberGameReport", this.Params.reportParams);
};

NumberGameReport_Summary.prototype.ShowHideSiteTypeInfo = function () {
    if (!this.Tables.nonLiveNumberGameReportContainer.hasClass("display-none")) {
        this.Tables.nonLiveNumberGameReport.fanexGrid("resizeGrid");
        this.Tables.liveNumberGameReport.fanexGrid("resizeGrid");
    }
};