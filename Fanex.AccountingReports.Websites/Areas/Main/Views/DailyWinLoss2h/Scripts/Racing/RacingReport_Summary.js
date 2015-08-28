RacingReport_Summary = function () {
    var self = this;
    self.Params = {
        reportParams: {
            Winlostdate: null,
            SportId: null
        }
    };

    self.Tables = {
        summaryRacingtReportContainer: null,
        summaryRacingReport: null
    };

    self.CacheData = {
        parentController: null
    };
};

RacingReport_Summary.prototype.InitPage = function (parentController) {
    this.CachedDOMElements();
    this.CacheData.parentController = parentController;
};

RacingReport_Summary.prototype.CachedDOMElements = function () {
    this.Tables.summaryRacingReportContainer = $(".summaryRacingReportContainer");
    this.Tables.summaryRacingReport = $(".summaryRacingReport");
};

RacingReport_Summary.prototype.GetReport = function (params) {
    var self = this;
    AccReport.disableInput();
    self.Params.reportParams = params;
    AccReport.utility.CallAjax(this,
        window.rootUrl + "Main/DailyWinLoss2h/GetSummaryRacingReport",
        self.Params.reportParams,
        self.RenderReport,
        self.RenderReport,
        AccReport.enableInput);
};

RacingReport_Summary.prototype.RenderReport = function (report) {
    var self = this;
    var bodyData = [];
    var footerData = [];
    var maxColIndex = 7;
    if (report && report.ChildsInfo && report.ChildsInfo.length > 0) {
        var reportLength = report.ChildsInfo.length;
        var info = null;
        var groupdId = '';
        var tableId = "summaryRacingReport";

        for (var i = 0; i < reportLength; i++) {
            var sportType = report.ChildsInfo[i];
            groupdId = tableId + "_SportType" + sportType.Id +
                                 "_Live0";

            var info = sportType.GrandTotal;
            bodyData.push({
                Columns: [
                    {
                        Name: "ColSportType",
                        Value: showHideSiteTypeManager.GetShowHideSiteTypeIcon(groupdId) +
                            reportManager.RacingReport.GetReportLink(info, false, reportManager.Enum.DrillDownLevel.BySportType),
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

            var info = sportType.CreditTotal;
            bodyData.push({
                Columns: [
                    {
                        Name: "ColSportType",
                        Value: info.Name,
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

            var info = sportType.DepositTotal;
            bodyData.push({
                Columns: [
                    {
                        Name: "ColSportType",
                        Value: info.Name,
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
        groupdId = tableId + "_SportType0" +
                             "_Live0";
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
    }

    var gridOptions = {
        columns: [
            {
                HeaderText: viewResources.Sport_Type,
                Width: 150, Name: "ColSportType",
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
                  " " + self.CacheData.parentController.CacheData.SportName +
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

    this.Tables.summaryRacingReportContainer.removeClass("display-none");
    self.CacheData.parentController.ShowReportView();
    this.Tables.summaryRacingReport.removeClass("display-none").fanexGrid(gridOptions);
    this.Tables.summaryRacingReport.find(".showHideSiteType.showHideIcon").click(function () {
        var btnIcon = $(this);
        showHideSiteTypeManager.ShowHideSiteTypeInfo_ByGroup(btnIcon, self.Tables.summaryRacingReport);
    });
};

RacingReport_Summary.prototype.ShowReport = function () {
    this.Tables.summaryRacingReportContainer.removeClass("display-none");
};

RacingReport_Summary.prototype.HideReport = function () {
    this.Tables.summaryRacingReportContainer.addClass("display-none");
};

RacingReport_Summary.prototype.ExportReport = function () {
    AccReport.downloadFile(window.rootUrl + "Main/DailyWinLoss2h/ExportSummaryRacingReport", this.Params.reportParams);
};

RacingReport_Summary.prototype.ShowHideSiteTypeInfo = function () {
    if (!this.Tables.summaryRacingReportContainer.hasClass("display-none")) {
        this.Tables.summaryRacingReport.fanexGrid("resizeGrid");
    }
};
