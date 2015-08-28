VirtualSports_VirtualHorseRacingReport = function () {
    var self = this;

    self.Tables = {
        virtualSports_VirtualHorseRacingReportContainer: null,
        virtualSports_VirtualHorseRacingReport: null,
        gridSplitter: null
    };

    self.Params = {
        reportParams: {
            Winlostdate: null,
            Sporttype: null,
            Bettype: null,
            BetTypeName: null,
            Liveindicator: null,
            IsHedge: null
        }
    };

    self.CacheData = {
        parentController: null
    };
};

VirtualSports_VirtualHorseRacingReport.prototype.InitPage = function (parentController) {
    this.CachedDOMElements();
    this.CacheData.parentController = parentController;
};

VirtualSports_VirtualHorseRacingReport.prototype.CachedDOMElements = function () {
    this.Tables.reportView = $("#virtualSportReport");
    this.Tables.gridSplitter = this.Tables.reportView.find(".gridSplitter");
    this.Tables.virtualSports_VirtualHorseRacingReportContainer = $("#virtualSports_VirtualHorseRacingReportContainer");
    this.Tables.virtualSports_VirtualHorseRacingReport = this.Tables.virtualSports_VirtualHorseRacingReportContainer.find("#virtualSports_VirtualHorseRacingReport");
};

VirtualSports_VirtualHorseRacingReport.prototype.HideReportView = function () {
    this.Tables.virtualSports_VirtualHorseRacingReportContainer.addClass("display-none");
};

VirtualSports_VirtualHorseRacingReport.prototype.GetReport = function (params) {
    var self = this;

    AccReport.disableInput();
    self.Params.reportParams.Winlostdate = params.Winlostdate;
    self.Params.reportParams.Sporttype = self.CacheData.parentController.VirtualSportsReport_BySportType.Params.reportParams.SportType;
    self.Params.reportParams.SportTypeName = self.CacheData.parentController.VirtualSportsReport_BySportType.Params.reportParams.SportTypeName;
    self.Params.reportParams.Liveindicator = params.Liveindicator;
    self.Params.reportParams.IsHedge = params.IsHedge;
    self.Params.reportParams.Bettype = params.Bettype;
    self.Params.reportParams.BetTypeName = params.BetTypeName;
    AccReport.utility.CallAjax(this,
        window.rootUrl + "Main/DailyWinLoss2h/GetVirtualSports_VirtualHorseRacingReport",
        self.Params.reportParams,
        self.RenderReport,
        self.RenderReport,
        AccReport.enableInput);
};

VirtualSports_VirtualHorseRacingReport.prototype.RenderReport = function (report) {
    var self = this;
    var bodyData = [];
    var footerData = [];
    var maxColIndex = 7;

    if (report && report.ChildsInfo && report.ChildsInfo.length > 0) {
        var leagueGroups = report.ChildsInfo;
        for (var i = 0; i < leagueGroups.length; i++) {
            var leagueGroup = leagueGroups[i];
            bodyData.push({
                Columns: [{ Name: "ColBetType", Value: leagueGroup.Name, MergeColNumber: maxColIndex, CellAlign: "left", CssClass: "bold"}],
                CssClass: "freegrid-even"
            });

            var reportLength = leagueGroup.ChildsInfo.length;
            for (var j = 0; j < reportLength; j++) {
                var evenrow = j % 2 == 0 ? "" : "";
                var info = leagueGroup.ChildsInfo[j];
                bodyData.push({
                    Columns: [{ Name: "ColBetType", Value: info.Name, CssClass: "bold", CellAlign: "center" },
                                { Name: "ColWinloss", Value: info.WinLoss },
                                { Name: "ColPosition", Value: info.Position },
                                { Name: "ColCommission", Value: info.Commission },
                                { Name: "ColRealLineTotal", Value: info.LineTotal },
                                { Name: "ColRealTurnover", Value: info.Turnover },
                                reportManager.GetColumnTurnover(info.Margin, info.Turnover, "ColRealMargin")],
                    CssClass: evenrow
                });
            }

            info = leagueGroup.GrandTotal;
            footerData.push({
                Columns: [{ Name: "ColBetType", Value: info.Name, CellAlign: "right", CssClass: "bold" },
                        { Name: "ColWinloss", Value: info.WinLoss },
                        { Name: "ColPosition", Value: info.Position },
                        { Name: "ColCommission", Value: info.Commission },
                        { Name: "ColRealLineTotal", Value: info.LineTotal, TdClass: "colNote" },
                        { Name: "ColRealTurnover", Value: info.Turnover },
                        reportManager.GetColumnTurnover(info.Margin, info.Turnover, "ColRealMargin")]
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
                  " <a class='breadcrumbs-link' onclick='reportManager.VirtualSportReport.ShowReportBySportType();'>" + self.CacheData.parentController.VirtualSportsReport_BySportType.Params.reportParams.SportTypeName + "</a> >" +
                  " " + self.Params.reportParams.BetTypeName,
            showExcelButton: false
        },
        bodyRows: bodyData,
        footerRows: footerData,
        numberOfFrozenColumn: 0,
        gridWidth: 1400,
        gridExpandHeight: 200,
        fullWidth: true,
        fullHeight: true,
        gridHeight: 400,
        showDistinctRow: false,
        showHighlightRow: false,
        noDataText: viewResources.EmptyReportMessage,
        headerColumnHeight: 29
    };

    this.CacheData.parentController.VirtualSportsReport_BySportType.HideReport();
    this.Tables.reportView.removeClass("display-none");
    this.Tables.virtualSports_VirtualHorseRacingReportContainer.removeClass("display-none");
    this.Tables.virtualSports_VirtualHorseRacingReport.removeClass("display-none").fanexGrid(gridOptions);
};

VirtualSports_VirtualHorseRacingReport.prototype.HideReport = function () {
    this.Tables.virtualSports_VirtualHorseRacingReportContainer.addClass("display-none");
};

VirtualSports_VirtualHorseRacingReport.prototype.ExportReport = function () {
    AccReport.downloadFile(window.rootUrl + "Main/DailyWinLoss2h/ExportVirtualSports_VirtualHorseRacingReport", this.Params.reportParams);
}; 