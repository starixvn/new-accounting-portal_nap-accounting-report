RacingReport_ByBetType = function () {
    var self = this;
    self.Params = {
        reportParams: {
            Winlostdate: null,
            SportType: null,
            Bettype: null,
            BetTypeName: null,
            Liveindicator: null,
            IsHedge: null
        }
    };

    self.Tables = {
        racingReportByBetTypeContainer: null,
        racingReportByBetType: null
    };

    self.CacheData = {
        parentController: null
    };
};

RacingReport_ByBetType.prototype.InitPage = function (parentController) {
    this.CachedDOMElements();
    this.CacheData.parentController = parentController;
}

RacingReport_ByBetType.prototype.CachedDOMElements = function () {
    this.Tables.racingReportByBetTypeContainer = $(".racingReportByBetTypeContainer");
    this.Tables.racingReportByBetType = $(".racingReportByBetType");
};

RacingReport_ByBetType.prototype.GetReport = function (reportDate,
    betTypeId,
    betTypeName,
    isLive,
    isHedge) {
    AccReport.disableInput();

    var self = this;
    self.Params.reportParams.Winlostdate = reportDate;
    self.Params.reportParams.SportType = self.CacheData.parentController.RacingReport_BySportType.Params.reportParams.SportType;
    self.Params.reportParams.SportName = self.CacheData.parentController.RacingReport_BySportType.Params.reportParams.SportName;
    self.Params.reportParams.Liveindicator = isLive;
    self.Params.reportParams.IsHedge = isHedge;
    self.Params.reportParams.Bettype = betTypeId;
    self.Params.reportParams.BetTypeName = betTypeName;
    AccReport.utility.CallAjax(this,
        window.rootUrl + "Main/DailyWinLoss2h/GetRacingReportByBetType",
        self.Params.reportParams,
        self.RenderReport,
        self.RenderReport,
        AccReport.enableInput);
};

RacingReport_ByBetType.prototype.RenderReport = function (report) {
    var self = this;
    this.Tables.racingReportByBetType.addClass("display-none");
    var bodyData = [];
    var footerData = [];
    if (report && report.ChildsInfo && report.ChildsInfo.length > 0) {
        var leagueGroups = report.ChildsInfo;
        for (var i = 0; i < leagueGroups.length; i++) {
            var leagueGroup = leagueGroups[i];
            bodyData.push({
                Columns: [{ Name: "ColCode", Value: leagueGroup.Name, MergeColNumber: 7, CellAlign: "left", CssClass: "bold"}],
                CssClass: "freegrid-even"
            });
            var items = leagueGroup.ChildsInfo;
            for (var j = 0; j < items.length; j++) {
                var evenrow = (j % 2) == 0 ? "" : "";
                var info = items[j];
                bodyData.push({
                    Columns: [{ Name: "ColCode", Value: info.Name, CssClass: "bold", CellAlign: "center" },
                              { Name: "ColWinloss", Value: info.WinLoss },
                              { Name: "ColPosition", Value: info.Position },
                              { Name: "ColCommission", Value: info.Commission },
                              { Name: "ColRealLineTotal", Value: info.LineTotal },
                              { Name: "ColRealTurnover", Value: info.Turnover },
                              reportManager.GetColumnTurnover(info.Margin, info.Turnover, "ColRealMargin")],
                    CssClass: evenrow
                });
            }

            var info = leagueGroup.GrandTotal;
            bodyData.push({
                Columns: [
                    {
                        Name: "ColCode",
                        Value: info.Name,
                        CellAlign: "right",
                        CssClass: "bold"
                    },
                    { Name: "ColWinloss", Value: info.WinLoss },
                    { Name: "ColPosition", Value: info.Position },
                    { Name: "ColCommission", Value: info.Commission },
                    { Name: "ColRealLineTotal", Value: info.LineTotal, TdClass: "colNote", CssClass: "bold" },
                    { Name: "ColRealTurnover", Value: info.Turnover, CssClass: "bold" },
                    reportManager.GetColumnTurnover(info.Margin, info.Turnover, "ColRealMargin")]
            });
        }

        info = report.GrandTotal;
        footerData.push({
            Columns: [{ Name: "ColCode", Value: info.Name, CellAlign: "right", CssClass: "bold" },
                        { Name: "ColWinloss", Value: info.WinLoss },
                        { Name: "ColPosition", Value: info.Position },
                        { Name: "ColCommission", Value: info.Commission },
                        { Name: "ColRealLineTotal", Value: info.LineTotal, TdClass: "colNote" },
                        { Name: "ColRealTurnover", Value: info.Turnover },
                        reportManager.GetColumnTurnover(info.Margin, info.Turnover, "ColRealMargin")]
        });
    }

    var gridOptions = {
        columns: [{
            HeaderText: viewResources.Bet_Type,
            Width: 105, Name: "ColCode",
            HeaderAlign: "center",
            CellAlign: "left"
        },
                      { HeaderText: viewResources.Member_WinLoss, Width: 100, Name: "ColWinloss", FormatNumber: FormatConstant.TWO_DECIMAL_NUMBER, HeaderAlign: "center", CellAlign: "right", FormatPositive: '$' },
                      { HeaderText: viewResources.SMA_WinLoss, Width: 100, Name: "ColPosition", FormatNumber: FormatConstant.TWO_DECIMAL_NUMBER, HeaderAlign: "center", CellAlign: "right", FormatPositive: '$' },
                      { HeaderText: viewResources.Commission, Width: 100, Name: "ColCommission", FormatNumber: FormatConstant.TWO_DECIMAL_NUMBER, HeaderAlign: "center", CellAlign: "right", FormatPositive: '$' },
                      { HeaderText: viewResources.Company_Income, Width: 100, Name: "ColRealLineTotal", FormatNumber: FormatConstant.TWO_DECIMAL_NUMBER, HeaderAlign: "center", CellAlign: "right", FormatPositive: '$' },
                      { HeaderText: viewResources.Company_Turnover, Width: 100, Name: "ColRealTurnover", FormatNumber: FormatConstant.TWO_DECIMAL_NUMBER, HeaderAlign: "center", CellAlign: "right", FormatPositive: '$' },
                      { HeaderText: viewResources.Margin, Width: 100, Name: "ColRealMargin", FormatNumber: FormatConstant.TWO_DECIMAL_PERCENT_NUMBER, HeaderAlign: "center", CellAlign: "right", FormatPositive: '$'}],
        showTitle: true,
        showFooter: true,
        titleInfo: {
            text: "<a class='breadcrumbs-link' onclick='showHideReportManager.BackToMainReport();'>" + viewResources.Main + "</a> >" +
                  " <a class='breadcrumbs-link' onclick='reportManager.RacingReport.ShowSummaryReport();'>" + self.CacheData.parentController.CacheData.SportName + "</a> |" +
                  " <a class='breadcrumbs-link' onclick='reportManager.RacingReport.ShowReportBySportType();'>" + self.CacheData.parentController.RacingReport_BySportType.Params.reportParams.SportName + "</a> >" +
                  " " + self.Params.reportParams.BetTypeName,
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
        headerColumnHeight: 29,
        noDataText: viewResources.EmptyReportMessage,
        showHighlightRow: true
    };
    this.Tables.racingReportByBetTypeContainer.removeClass("display-none");
    this.CacheData.parentController.RacingReport_BySportType.HideReport();
    self.CacheData.parentController.ShowReportView();
    this.Tables.racingReportByBetType.removeClass("display-none").fanexGrid(gridOptions);
};

RacingReport_ByBetType.prototype.ShowReport = function () {
    this.Tables.racingReportByBetTypeContainer.removeClass("display-none");
};

RacingReport_ByBetType.prototype.HideReport = function () {
    this.Tables.racingReportByBetTypeContainer.addClass("display-none");
    reportManager.Tables.btnShowHideSiteTypeInfo.removeClass("display-none");
};

RacingReport_ByBetType.prototype.ExportReport = function () {
    AccReport.downloadFile(window.rootUrl + "Main/DailyWinLoss2h/ExportRacingReportByBetType", this.Params.reportParams);
};