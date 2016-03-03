OtherSportReport_BySportType = function () {
    var self = this;
    self.Params = {
        reportParams: {
            Winlostdate: null,
            SportType: null,
            SportTypeName: '',
            Liveindicator: null,
            IsHedge: null
        }
    };

    self.Tables = {
        otherSportReportBySportContainer: null,
        nonLiveOtherSportReportBySportContainer: null,
        nonLiveOtherSportReportBySport: null,
        liveOtherSportReportBySportContainer: null,
        liveOtherSportReportBySport: null
    };

    self.CacheData = {
        parentController: null
    };
};

OtherSportReport_BySportType.prototype.InitPage = function (parentController) {
    this.CachedDOMElements();
    this.CacheData.parentController = parentController;
};

OtherSportReport_BySportType.prototype.CachedDOMElements = function () {
    this.Tables.otherSportReportBySportContainer = $("#otherSportReportBySportContainer");
    this.Tables.nonLiveOtherSportReportBySportContainer = this.Tables.otherSportReportBySportContainer.find(".nonLiveOtherSportReportBySportContainer");
    this.Tables.nonLiveOtherSportReportBySport = $("#nonLiveOtherSportReportBySport");

    this.Tables.liveOtherSportReportBySportContainer = this.Tables.otherSportReportBySportContainer.find("#liveOtherSportReportBySportContainer");
    this.Tables.liveOtherSportReportBySport = $("#liveOtherSportReportBySport");
};

OtherSportReport_BySportType.prototype.GetReport = function (reportDate,
    sportTypeId,
    sportTypeName,
    isLive,
    isHedge) {
    var self = this;

    AccReport.disableInput();
    self.Params.reportParams.Winlostdate = reportDate;
    self.Params.reportParams.SportType = sportTypeId;
    self.Params.reportParams.SportTypeName = sportTypeName;
    self.Params.reportParams.Liveindicator = isLive;
    self.Params.reportParams.IsHedge = isHedge;
    AccReport.utility.CallAjax(this,
        window.rootUrl + "Main/DailyWinLoss2h/GetOtherSportReportBySport",
        self.Params.reportParams,
        self.RenderLiveAndNonLiveReport,
        self.RenderLiveAndNonLiveReport,
        AccReport.enableInput);
};

OtherSportReport_BySportType.prototype.RenderLiveAndNonLiveReport = function (report) {
    this.CacheData.parentController.ShowReportBySportType();
    if (report) {
        this.RenderNonLiveReport(report.NoneLiveReport);
        this.RenderLiveReport(report.LiveReport);
    } else {
        this.RenderNonLiveReport(null);
        this.RenderLiveReport(null);
    }
};

OtherSportReport_BySportType.prototype.RenderNonLiveReport = function (report) {
    var self = this;
    this.Tables.nonLiveOtherSportReportBySport.addClass("display-none");
    var gridOptions = self.GetGridOptions_BySportTypeReport(report, false);
    this.Tables.nonLiveOtherSportReportBySportContainer.removeClass("display-none");
    this.Tables.otherSportReportBySportContainer.removeClass("display-none");
    self.CacheData.parentController.ShowReportView();
    this.Tables.nonLiveOtherSportReportBySport.removeClass("display-none").fanexGrid(gridOptions);
    this.Tables.nonLiveOtherSportReportBySport.find(".showHideSiteType.showHideIcon").click(function () {
        var btnIcon = $(this);
        showHideSiteTypeManager.ShowHideSiteTypeInfo_ByGroup(btnIcon, self.Tables.nonLiveOtherSportReportBySport);
    });
};

OtherSportReport_BySportType.prototype.RenderLiveReport = function (report) {
    var self = this;
    this.Tables.liveOtherSportReportBySport.addClass("display-none");
    var gridOptions = self.GetGridOptions_BySportTypeReport(report, true);

    this.Tables.liveOtherSportReportBySportContainer.removeClass("display-none");
    this.Tables.otherSportReportBySportContainer.removeClass("display-none");
    self.CacheData.parentController.ShowReportView();
    this.Tables.liveOtherSportReportBySport.removeClass("display-none").fanexGrid(gridOptions);
    this.Tables.liveOtherSportReportBySport.find(".showHideSiteType.showHideIcon").click(function () {
        var btnIcon = $(this);
        showHideSiteTypeManager.ShowHideSiteTypeInfo_ByGroup(btnIcon, self.Tables.liveOtherSportReportBySport);
    });
};

OtherSportReport_BySportType.prototype.GetGridOptions_BySportTypeReport = function (report, isLive) {
    var self = this;
    var bodyData = [];
    var footerData = [];
    var maxColIndex = 11;
    var isSportTypeCricket = false;
    var reportLengthNew = report.ChildsInfo.length;
    for (var index = 0; index < reportLengthNew; index++) {
        if (report.ChildsInfo[index].Id == 500) {
            isSportTypeCricket = true;
        }
    }
    if ((report && report.ChildsInfo && report.ChildsInfo.length > 0) && (isSportTypeCricket == true)) {
        return OtherSportsForNewCricket(report, isLive, self.CacheData, self.Params);
    }
    if ((report && report.ChildsInfo && report.ChildsInfo.length > 0) && (isSportTypeCricket == false)) {
        var reportLength = report.ChildsInfo.length;
        var info = null;
        var groupdId = '';
        var tableId = isLive ? "liveOtherSportReportBySport" : "nonLiveOtherSportReportBySport";

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
                               reportManager.OtherSportReport.GetReportLink(info, report.IsLive, reportManager.Enum.DrillDownLevel.ByBetType),
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
                            SportType: self.Params.reportParams.SportType,
                            SportGroupId: self.CacheData.parentController.CacheData.SportId,
                            WinLossDate: self.Params.reportParams.Winlostdate,
                            BetType: betType.Id,
                            BetTypeName: betType.AliasName,
                            IsLive: report.IsLive,
                            IsHedge: betType.Hedge && betType.Hedge != "" ? true : false,
                            SiteType: AccReportData.Constants.SiteType.Credit,
                            SiteTypeName: info.Name,
                            SportTypeName: self.Params.reportParams.SportTypeName
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
                            SportType: self.Params.reportParams.SportType,
                            SportGroupId: self.CacheData.parentController.CacheData.SportId,
                            WinLossDate: self.Params.reportParams.Winlostdate,
                            BetType: betType.Id,
                            BetTypeName: betType.AliasName,
                            IsLive: report.IsLive,
                            IsHedge: betType.Hedge && betType.Hedge != "" ? true : false,
                            SiteType: AccReportData.Constants.SiteType.Deposit,
                            SiteTypeName: info.Name,
                            SportTypeName: self.Params.reportParams.SportTypeName
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

        info = report.EightRMTotal;
        if (info.WinLoss != 0) {
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
                            reportManager.GetColumnTurnover(info.VirtualMargin, info.VirtualTurnover, "ColVirtualMargin")]
            });
        }

        info = report.BARMTotal;
        if (info.WinLoss != 0) {
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
                        reportManager.GetColumnTurnover(info.VirtualMargin, info.VirtualTurnover, "ColVirtualMargin")]
            });
        }
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
                      { HeaderText: viewResources.Margin, Name: "ColRealMargin", MergeColNumber: 2 }]
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
            { HeaderText: viewResources.Virtual, Width: 100, Name: "ColVirtualMargin", HeaderAlign: "center", FormatNumber: FormatConstant.TWO_DECIMAL_PERCENT_NUMBER, CellAlign: "right", FormatPositive: '$' }],
        showTitle: true,
        showFooter: true,
        titleInfo: {
            text: "<a class='breadcrumbs-link' onclick='showHideReportManager.BackToMainReport();'>" + viewResources.Main + "</a> >" +
                  " <a class='breadcrumbs-link' onclick='reportManager.OtherSportReport.ShowSummaryReport();'>" + self.CacheData.parentController.CacheData.SportName + "</a> |" +
                  " " + self.Params.reportParams.SportTypeName +
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

OtherSportReport_BySportType.prototype.ExportReport = function () {
    AccReport.downloadFile(window.rootUrl + "Main/DailyWinLoss2h/ExportOtherSportReportBySport", this.Params.reportParams);
};

OtherSportReport_BySportType.prototype.ShowReport = function () {
    AccReport.popUp.hide();
    this.Tables.otherSportReportBySportContainer.removeClass("display-none");
};

OtherSportReport_BySportType.prototype.HideReport = function () {
    AccReport.popUp.hide();
    this.Tables.otherSportReportBySportContainer.addClass("display-none");
};

OtherSportReport_BySportType.prototype.ShowHideSiteTypeInfo = function () {
    if (!this.Tables.nonLiveOtherSportReportBySport.hasClass("display-none")) {
        this.Tables.nonLiveOtherSportReportBySport.fanexGrid("resizeGrid");
        this.Tables.liveOtherSportReportBySport.fanexGrid("resizeGrid");
    }
};