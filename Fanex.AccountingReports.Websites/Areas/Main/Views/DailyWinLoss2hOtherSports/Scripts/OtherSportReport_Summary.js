OtherSportReport_Summary = function () {
    var self = this;
    self.Params = {
        reportParams: {
            Winlostdate: null,
            SportId: null
        }
    };

    self.Tables = {
        summaryOtherSportReportContainer: null,
        summaryNonLiveOtherSportReportContainer: null,
        summaryNonLiveOtherSportReport: null
    };

    self.CacheData = {
        parentController: null
    };
};

OtherSportReport_Summary.prototype.InitPage = function (parentController) {
    this.CachedDOMElements();
    this.CacheData.parentController = parentController;
};

OtherSportReport_Summary.prototype.CachedDOMElements = function () {
    this.Tables.summaryOtherSportReportContainer = $("#summaryOtherSportReportContainer");
    this.Tables.summaryNonLiveOtherSportReportContainer = $("#summaryNonLiveOtherSportReportContainer");
    this.Tables.summaryNonLiveOtherSportReport = $("#summaryNonLiveOtherSportReport");
    this.Tables.summaryLiveOtherSportReportContainer = $(".summaryLiveOtherSportReportContainer");
    this.Tables.summaryLiveOtherSportReportBody = $("#summaryLiveOtherSportReport");
};

OtherSportReport_Summary.prototype.GetReport = function (params) {
    var self = this;

    AccReport.disableInput();
    self.Params.reportParams = params;
    AccReport.utility.CallAjax(this,
        window.rootUrl + "Main/DailyWinLoss2h/GetSummaryOtherSportReport",
        self.Params.reportParams,
        self.RenderLiveAndNonLiveReport,
        self.RenderLiveAndNonLiveReport,
        AccReport.enableInput);
};

OtherSportReport_Summary.prototype.RenderLiveAndNonLiveReport = function (report) {
    this.CacheData.parentController.ShowSummaryReport();
    if (report) {
        this.RenderNonLiveReport(report.NoneLiveReport);
        this.RenderLiveReport(report.LiveReport);
    } else {
        this.RenderNonLiveReport(null);
        this.RenderLiveReport(null);
    }
};

OtherSportReport_Summary.prototype.RenderNonLiveReport = function (report) {
    var self = this;
    this.Tables.summaryNonLiveOtherSportReport.addClass("display-none");
    var gridOptions = self.GetGridOptions_SummaryReport(report, false);
    this.Tables.summaryNonLiveOtherSportReportContainer.removeClass("display-none");
    this.Tables.summaryOtherSportReportContainer.removeClass("display-none");
    self.CacheData.parentController.ShowReportView();
    this.Tables.summaryNonLiveOtherSportReport.removeClass("display-none").fanexGrid(gridOptions);
    this.Tables.summaryNonLiveOtherSportReport.find(".showHideSiteType.showHideIcon").click(function () {
        var btnIcon = $(this);
        showHideSiteTypeManager.ShowHideSiteTypeInfo_ByGroup(btnIcon, self.Tables.summaryNonLiveOtherSportReport);
    });
};

OtherSportReport_Summary.prototype.RenderLiveReport = function (report) {
    var self = this;
    this.Tables.summaryLiveOtherSportReportContainer.addClass("display-none");
    var gridOptions = self.GetGridOptions_SummaryReport(report, true);
    this.Tables.summaryLiveOtherSportReportContainer.removeClass("display-none");
    this.Tables.summaryOtherSportReportContainer.removeClass("display-none");
    self.CacheData.parentController.ShowReportView();
    this.Tables.summaryLiveOtherSportReportBody.removeClass("display-none").fanexGrid(gridOptions);
    this.Tables.summaryLiveOtherSportReportBody.find(".showHideSiteType.showHideIcon").click(function () {
        var btnIcon = $(this);
        showHideSiteTypeManager.ShowHideSiteTypeInfo_ByGroup(btnIcon, self.Tables.summaryLiveOtherSportReportBody);
    });
};

OtherSportReport_Summary.prototype.GetGridOptions_SummaryReport = function (report, isLive) {
    var self = this;
    var bodyData = [];
    var footerData = [];
    var maxColIndex = 11;
    if (report && report.ChildsInfo && report.ChildsInfo.length > 0) {
        var reportLength = report.ChildsInfo.length;
        var info = null;
        var groupdId = '';
        var tableId = isLive ? "liveSoccerReport" : "nonLiveSoccerReport";
        for (var i = 0; i < reportLength; i++) {
            var sportType = report.ChildsInfo[i];
            groupdId = tableId + "_SportType" + sportType.Id +
                                 "_Hedge0" +
                                 "_Live" + (isLive ? 1 : 0);
            info = sportType.GrandTotal;
            bodyData.push({
                Columns: [
                    {
                        Name: "ColSportType",
                        Value: showHideSiteTypeManager.GetShowHideSiteTypeIcon(groupdId) +
                            reportManager.OtherSportReport.GetReportLink(info, report.IsLive, reportManager.Enum.DrillDownLevel.BySportType),
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

            info = sportType.CreditTotal;
            bodyData.push({
                Columns: [{ Name: "ColSportType", Value: info.Name, CellAlign: "right", CssClass: "bold" },
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

            info = sportType.DepositTotal;
            bodyData.push({
                Columns: [{ Name: "ColSportType", Value: info.Name, CellAlign: "right", CssClass: "bold" },
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
        groupdId = tableId + "_SportType0_Hedge0" +
                             "_Live" + (isLive ? 1 : 0);
        footerData.push({
            Columns: [
                {
                    Name: "ColSportType",
                    Value: showHideSiteTypeManager.GetShowHideSiteTypeIcon(groupdId) +
                           '<span class="right float-right">' + info.Name + '</span>'
                },
                { Name: "ColWinloss", Value: info.WinLoss },
                { Name: "ColPosition", Value: info.Position },
                { Name: "ColCommission", Value: info.Commission },
                { Name: "ColRealLineTotal", Value: info.LineTotal, TdClass: "colNote" },
                { Name: "ColVirtualLineTotal", Value: info.VirtualLineTotal, TdClass: "colNote" },
                { Name: "ColVariance", Value: info.Variance, TdClass: "colNote" },
                { Name: "ColRealTurnover", Value: info.Turnover },
                { Name: "ColVirtualTurnover", Value: info.VirtualTurnover },
                reportManager.GetColumnTurnover(info.Margin, info.Turnover, "ColRealMargin"),
                reportManager.GetColumnTurnover(info.VirtualMargin, info.VirtualTurnover, "ColVirtualMargin")]
        });

        info = report.CreditTotal;
        footerData.push({
            Columns: [{ Name: "ColSportType", Value: info.Name, CellAlign: "right", CssClass: "bold" },
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
            Columns: [{ Name: "ColSportType", Value: info.Name, CellAlign: "right", CssClass: "bold" },
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

        info = report.EightRMTotal;
        if (info.WinLoss != 0) {
            footerData.push({
                Columns: [{ Name: "ColSportType", Value: info.Name, CellAlign: "right", CssClass: "bold" },
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
        }

        info = report.BARMTotal;
        if (info.WinLoss != 0) {
            footerData.push({
                Columns: [{ Name: "ColSportType", Value: info.Name, CellAlign: "right", CssClass: "bold" },
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
        }
    }

    var gridOptions = {
        customizeHeader: true,
        groupHeaderRows: [{
            Columns: [{ HeaderText: viewResources.Sport_Type, Name: "ColSportType", MergeRowNumber: 2 },
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
                HeaderText: viewResources.Sport_Type,
                Width: 170, Name: "ColSportType",
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
            text: "<a class='breadcrumbs-link' onclick='showHideReportManager.BackToMainReport();'>" + viewResources.Main + "</a> >" +
                         " " + self.CacheData.parentController.CacheData.SportName +
                         " \"" + (isLive ? viewResources.Live : viewResources.Symbol_Sparse_Non_Live) + "\"" +
                         " " + viewResources.DailyWinLoss2h_ReportTitleSuffix,
            showExcelButton: false
        },
        bodyRows: bodyData,
        footerRows: footerData,
        numberOfFrozenColumn: 0,
        gridWidth: 1400,
        gridExpandHeight: 200,
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

OtherSportReport_Summary.prototype.ShowReport = function () {
    AccReport.popUp.hide();
    this.Tables.summaryOtherSportReportContainer.removeClass("display-none");
};

OtherSportReport_Summary.prototype.HideReport = function () {
    AccReport.popUp.hide();
    this.Tables.summaryOtherSportReportContainer.addClass("display-none");
};

OtherSportReport_Summary.prototype.ExportReport = function () {
    AccReport.downloadFile(window.rootUrl + "Main/DailyWinLoss2h/ExportSummaryOtherSportReport", this.Params.reportParams);
};

OtherSportReport_Summary.prototype.ShowHideSiteTypeInfo = function () {
    if (!this.Tables.summaryNonLiveOtherSportReport.hasClass("display-none")) {
        this.Tables.summaryNonLiveOtherSportReport.fanexGrid("resizeGrid");
        this.Tables.summaryLiveOtherSportReportBody.fanexGrid("resizeGrid");
    }
};
