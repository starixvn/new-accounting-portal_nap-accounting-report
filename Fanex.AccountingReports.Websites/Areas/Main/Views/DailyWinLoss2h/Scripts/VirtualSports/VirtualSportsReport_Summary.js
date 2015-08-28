VirtualSportsReport_Summary = function () {
    var self = this;
    self.Params = {
        reportParams: {
            Winlostdate: null,
            SportId: null
        }
    };

    self.Tables = {
        summaryVirtualSportReportContainer: null,
        summaryNonLiveVirtualSportReportContainer: null,
        summaryNonLiveVirtualSportReport: null
    };

    self.CacheData = {
        parentController: null
    };
};

VirtualSportsReport_Summary.prototype.InitPage = function (parentController) {
    this.CachedDOMElements();
    this.CacheData.parentController = parentController;
};

VirtualSportsReport_Summary.prototype.CachedDOMElements = function () {
    this.Tables.summaryVirtualSportReportContainer = $(".summaryVirtualSportReportContainer");
    this.Tables.nonLiveSummaryVirtualSportReportContainer = this.Tables.summaryVirtualSportReportContainer.find(".summaryNonLiveVirtualSportReportContainer");
    this.Tables.summaryNonLiveVirtualSportReport = this.Tables.summaryVirtualSportReportContainer.find(".summaryNonLiveVirtualSportReport");
};

VirtualSportsReport_Summary.prototype.GetReport = function (params) {
    var self = this;

    AccReport.disableInput();
    self.Params.reportParams = params;
    AccReport.utility.CallAjax(this,
        window.rootUrl + "Main/DailyWinLoss2h/GetVirtualSportReport",
        self.Params.reportParams,
        self.RenderLiveAndNonLiveReport,
        self.RenderLiveAndNonLiveReport,
        AccReport.enableInput);
};

VirtualSportsReport_Summary.prototype.RenderLiveAndNonLiveReport = function (report) {
    if (report) {
        this.RenderNonLiveReport(report.NoneLiveReport);
    } else {
        this.RenderNonLiveReport(null);
    }
};

VirtualSportsReport_Summary.prototype.RenderNonLiveReport = function (report) {
    var self = this;
    this.Tables.nonLiveSummaryVirtualSportReportContainer.addClass("display-none");
    var gridOptions = this.GetGridOptions_SummaryReport(report, false);
    this.CacheData.parentController.ShowReportView();
    this.Tables.summaryVirtualSportReportContainer.removeClass("display-none");
    this.Tables.nonLiveSummaryVirtualSportReportContainer.removeClass("display-none");
    this.Tables.summaryNonLiveVirtualSportReport.removeClass("display-none").fanexGrid(gridOptions);
    this.Tables.summaryNonLiveVirtualSportReport.find(".showHideSiteType.showHideIcon").click(function () {
        var btnIcon = $(this);
        showHideSiteTypeManager.ShowHideSiteTypeInfo_ByGroup(btnIcon, self.Tables.summaryNonLiveVirtualSportReport);
    });
};

VirtualSportsReport_Summary.prototype.GetGridOptions_SummaryReport = function (report, isLive) {
    var self = this;
    var bodyData = [];
    var footerData = [];
    var maxColIndex = 7;
    if (report && report.ChildsInfo && report.ChildsInfo.length > 0) {
        var reportLength = report.ChildsInfo.length;
        var info = null;
        var groupdId = '';
        var tableId = isLive ? "summaryNonLiveVirtualSportReport" : "summaryLiveVirtualSportReport";
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
                            reportManager.VirtualSportReport.GetReportLink(info, report.IsLive, reportManager.Enum.DrillDownLevel.BySportType),
                        CellAlign: "left",
                        CssClass: "bold"
                    },
                    { Name: "ColWinloss", Value: info.WinLoss },
                    { Name: "ColPosition", Value: info.Position },
                    { Name: "ColCommission", Value: info.Commission },
                    { Name: "ColRealLineTotal", Value: info.LineTotal },
                    { Name: "ColRealTurnover", Value: info.Turnover },
                    reportManager.GetColumnTurnover(info.Margin, info.Turnover, "ColRealMargin")],
                CssClass: "freegrid-even"
            });

            info = sportType.CreditTotal;
            bodyData.push({
                Columns: [{ Name: "ColSportType", Value: info.Name, CellAlign: "right", CssClass: "bold" },
                            { Name: "ColWinloss", Value: info.WinLoss },
                            { Name: "ColPosition", Value: info.Position },
                            { Name: "ColCommission", Value: info.Commission },
                            { Name: "ColRealLineTotal", Value: info.LineTotal },
                            { Name: "ColRealTurnover", Value: info.Turnover },
                            reportManager.GetColumnTurnover(info.Margin, info.Turnover, "ColRealMargin")],
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
                            { Name: "ColRealTurnover", Value: info.Turnover },
                            reportManager.GetColumnTurnover(info.Margin, info.Turnover, "ColRealMargin")],
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
                { Name: "ColRealTurnover", Value: info.Turnover },
                reportManager.GetColumnTurnover(info.Margin, info.Turnover, "ColRealMargin")]
        });

        info = report.CreditTotal;
        footerData.push({
            Columns: [{ Name: "ColSportType", Value: info.Name, CellAlign: "right", CssClass: "bold" },
                        { Name: "ColWinloss", Value: info.WinLoss },
                        { Name: "ColPosition", Value: info.Position },
                        { Name: "ColCommission", Value: info.Commission },
                        { Name: "ColRealLineTotal", Value: info.LineTotal },
                        { Name: "ColRealTurnover", Value: info.Turnover },
                        reportManager.GetColumnTurnover(info.Margin, info.Turnover, "ColRealMargin")],
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
                        { Name: "ColRealTurnover", Value: info.Turnover },
                        reportManager.GetColumnTurnover(info.Margin, info.Turnover, "ColRealMargin")],
            CssClass: showHideSiteTypeManager.GetSiteTypeRowCss(groupdId,
                showHideSiteTypeManager.Enums.RowType.InfoRow)
        });

        info = report.EightRMTotal;
        if (info.WinLoss != 0) {
            footerData.push({
                Columns: [{ Name: "ColSportType", Value: info.Name, CellAlign: "right", CssClass: "bold" },
                            { Name: "ColWinloss", Value: info.WinLoss },
                            { Name: "ColPosition", MergeColNumber: 9}]
            });
        }

        info = report.BARMTotal;
        if (info.WinLoss != 0) {
            footerData.push({
                Columns: [{ Name: "ColSportType", Value: info.Name, CellAlign: "right", CssClass: "bold" },
                        { Name: "ColWinloss", Value: info.WinLoss },
                        { Name: "ColPosition", MergeColNumber: 9}]
            });
        }
    }

    var gridOptions = {
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
            { HeaderText: viewResources.Company_Income, Width: 100, Name: "ColRealLineTotal", FormatNumber: FormatConstant.TWO_DECIMAL_NUMBER, HeaderAlign: "center", CellAlign: "right", FormatPositive: '$' },
            { HeaderText: viewResources.Company_Turnover, Width: 100, Name: "ColRealTurnover", FormatNumber: FormatConstant.TWO_DECIMAL_NUMBER, HeaderAlign: "center", CellAlign: "right" },
            { HeaderText: viewResources.Margin, Width: 100, Name: "ColRealMargin", FormatNumber: FormatConstant.TWO_DECIMAL_PERCENT_NUMBER, HeaderAlign: "center", CellAlign: "right", FormatPositive: '$'}],
        showTitle: true,
        showFooter: true,
        titleInfo: {
            text: "<a class='breadcrumbs-link' onclick='showHideReportManager.BackToMainReport();'>" + viewResources.Main + "</a> > " +
                          self.CacheData.parentController.CacheData.SportName +
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
        headerColumnHeight: 29
    };

    return gridOptions;
};

VirtualSportsReport_Summary.prototype.ShowReport = function () {
    AccReport.popUp.hide();
    this.Tables.summaryVirtualSportReportContainer.removeClass("display-none");
};

VirtualSportsReport_Summary.prototype.HideReport = function () {
    AccReport.popUp.hide();
    this.Tables.summaryVirtualSportReportContainer.addClass("display-none");
};

VirtualSportsReport_Summary.prototype.ExportReport = function () {
    AccReport.downloadFile(window.rootUrl + "Main/DailyWinLoss2h/ExportVirtualSportReport", this.Params.reportParams);
};

VirtualSportsReport_Summary.prototype.ShowHideSiteTypeInfo = function () {
    if (!this.Tables.summaryVirtualSportReportContainer.hasClass("display-none")) {
        this.Tables.summaryNonLiveVirtualSportReport.fanexGrid("resizeGrid");
    }
};
