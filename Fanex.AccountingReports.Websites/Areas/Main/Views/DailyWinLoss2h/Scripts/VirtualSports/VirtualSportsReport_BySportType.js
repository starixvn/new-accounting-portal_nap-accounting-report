VirtualSportsReport_BySportType = function () {
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
        virtualSportReportBySportContainer: null,
        nonLiveVirtualSportReportBySportContainer: null,
        nonLiveVirtualSportReportBySport: null
    };

    self.CacheData = {
        parentController: null
    };
};

VirtualSportsReport_BySportType.prototype.InitPage = function (parentController) {
    this.CachedDOMElements();
    this.CacheData.parentController = parentController;
};

VirtualSportsReport_BySportType.prototype.CachedDOMElements = function () {
    this.Tables.virtualSportReportBySportContainer = $(".virtualSportReportBySportContainer");
    this.Tables.nonLiveVirtualSportReportBySportContainer = this.Tables.virtualSportReportBySportContainer.find(".virtualNonLiveSportReportBySportContainer");
    this.Tables.nonLiveVirtualSportReportBySport = this.Tables.virtualSportReportBySportContainer.find(".virtualNonLiveSportReportBySport");
};

VirtualSportsReport_BySportType.prototype.GetReport = function (reportDate,
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
        window.rootUrl + "Main/DailyWinLoss2h/GetVirtualSportReportBySport",
        self.Params.reportParams,
        self.RenderLiveAndNonLiveReport,
        self.RenderLiveAndNonLiveReport,
        AccReport.enableInput);
};

VirtualSportsReport_BySportType.prototype.RenderLiveAndNonLiveReport = function (report) {
    this.CacheData.parentController.ShowReportBySportType();
    if (report) {
        this.RenderNonLiveReport(report.NoneLiveReport);
    } else {
        this.RenderNonLiveReport(null);
    }
};

VirtualSportsReport_BySportType.prototype.RenderNonLiveReport = function (report) {
    var self = this;
    this.Tables.nonLiveVirtualSportReportBySportContainer.addClass("display-none");
    var gridOptions = self.GetGridOptions_BySportTypeReport(report);

    self.CacheData.parentController.ShowReportView();
    this.Tables.virtualSportReportBySportContainer.removeClass("display-none");
    this.Tables.nonLiveVirtualSportReportBySportContainer.removeClass("display-none");
    this.Tables.nonLiveVirtualSportReportBySport.removeClass("display-none").fanexGrid(gridOptions);
    this.Tables.nonLiveVirtualSportReportBySport.find(".showHideSiteType.showHideIcon").click(function () {
        var btnIcon = $(this);
        showHideSiteTypeManager.ShowHideSiteTypeInfo_ByGroup(btnIcon, self.Tables.nonLiveVirtualSportReportBySport);
    });
};

VirtualSportsReport_BySportType.prototype.GetGridOptions_BySportTypeReport = function (report) {
    var self = this;
    var bodyData = [];
    var footerData = [];
    var maxColIndex = 7;
    if (report && report.ChildsInfo && report.ChildsInfo.length > 0) {
        var reportLength = report.ChildsInfo.length;
        var info = null;
        var groupdId = '';
        var tableId = "virtualNonLiveSportReportBySport";

        for (var i = 0; i < reportLength; i++) {
            var betType = report.ChildsInfo[i];
            groupdId = tableId + "_BetType" + betType.Id +
                                 "_Hedge0_Live0";

            var info = betType.GrandTotal;
            bodyData.push({
                Columns: [
                    {
                        Name: "ColBetType",
                        Value: showHideSiteTypeManager.GetShowHideSiteTypeIcon(groupdId) +
                            reportManager.VirtualSportReport.GetReportLink(info, report.IsLive, reportManager.Enum.DrillDownLevel.ByBetType),
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
                            BetTypeName: betType.Name,
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
                    { Name: "ColRealTurnover", Value: info.Turnover },
                    reportManager.GetColumnTurnover(info.Margin, info.Turnover, "ColRealMargin")],
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
                            BetTypeName: betType.Name,
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
                    { Name: "ColRealTurnover", Value: info.Turnover },
                    reportManager.GetColumnTurnover(info.Margin, info.Turnover, "ColRealMargin")],
                CssClass: showHideSiteTypeManager.GetSiteTypeRowCss(groupdId,
                    showHideSiteTypeManager.Enums.RowType.InfoRow)
            });
        }

        info = report.GrandTotal;
        groupdId = tableId + "_BetType0_Hedge0_Live0";
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
                { Name: "ColRealTurnover", Value: info.Turnover },
                reportManager.GetColumnTurnover(info.Margin, info.Turnover, "ColRealMargin")]
        });

        info = report.CreditTotal;
        footerData.push({
            Columns: [{ Name: "ColBetType", Value: info.Name, CellAlign: "right", CssClass: "bold" },
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
            Columns: [{ Name: "ColBetType", Value: info.Name, CellAlign: "right", CssClass: "bold" },
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
                Columns: [{ Name: "ColBetType", Value: info.Name, CellAlign: "right", CssClass: "bold" },
                            { Name: "ColWinloss", Value: info.WinLoss },
                            { Name: "ColPosition", MergeColNumber: 5}]
            });
        }

        info = report.BARMTotal;
        if (info.WinLoss != 0) {
            footerData.push({
                Columns: [{ Name: "ColBetType", Value: info.Name, CellAlign: "right", CssClass: "bold" },
                            { Name: "ColWinloss", Value: info.WinLoss },
                            { Name: "ColPosition", MergeColNumber: 5}]
            });
        }
    }

    var gridOptions = {
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
            { HeaderText: viewResources.Company_Income, Width: 100, Name: "ColRealLineTotal", FormatNumber: FormatConstant.TWO_DECIMAL_NUMBER, HeaderAlign: "center", CellAlign: "right", FormatPositive: '$' },
            { HeaderText: viewResources.Company_Turnover, Width: 100, Name: "ColRealTurnover", FormatNumber: FormatConstant.TWO_DECIMAL_NUMBER, HeaderAlign: "center", CellAlign: "right" },
            { HeaderText: viewResources.Margin, Width: 100, Name: "ColRealMargin", FormatNumber: FormatConstant.TWO_DECIMAL_PERCENT_NUMBER, HeaderAlign: "center", CellAlign: "right", FormatPositive: '$'}],
        showTitle: true,
        showFooter: true,
        titleInfo: {
            text: "<a class='breadcrumbs-link' onclick='showHideReportManager.BackToMainReport();'>" + viewResources.Main + "</a> >" +
                  " <a class='breadcrumbs-link' onclick='reportManager.VirtualSportReport.ShowSummaryReport();'>" + self.CacheData.parentController.CacheData.SportName + "</a> |" +
                  " " + self.Params.reportParams.SportTypeName +
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

VirtualSportsReport_BySportType.prototype.ExportReport = function () {
    AccReport.downloadFile(window.rootUrl + "Main/DailyWinLoss2h/ExportVirtualSportReportBySport", this.Params.reportParams);
};

VirtualSportsReport_BySportType.prototype.ShowReport = function () {
    AccReport.popUp.hide();
    this.Tables.virtualSportReportBySportContainer.removeClass("display-none");
};

VirtualSportsReport_BySportType.prototype.HideReport = function () {
    AccReport.popUp.hide();
    this.Tables.virtualSportReportBySportContainer.addClass("display-none");
};

VirtualSportsReport_BySportType.prototype.ShowHideSiteTypeInfo = function () {
    if (!this.Tables.nonLiveVirtualSportReportBySportContainer.hasClass("display-none")) {
        this.Tables.nonLiveVirtualSportReportBySport.fanexGrid("resizeGrid");
    }
};
