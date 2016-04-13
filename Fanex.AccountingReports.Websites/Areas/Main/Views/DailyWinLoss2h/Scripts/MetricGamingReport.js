MetricGamingReport = function () {
    var self = this;

    self.MetricGamingByBetTypeReport = new MetricGamingByBetType();

    self.Tables = {
        btnGenerate: null,
        btnToday: null,
        btnYesterday: null,
        btnExport: null,
        btnBack: null,
        datepickerplugin: null,
        datepicker: null,

        reportView: null,

        metricGamingLiveContainer: null,
        metricGamingNonLiveContainer: null,
        metricGamingLiveContent: null,
        metricGamingNoneLiveContent: null
    };

    self.Params = {
        reportParams: {
            Winlostdate: null,
            SubProduct: 'MG-SuperLive'
        }
    };

    self.CacheData = {
        SportId: null,
        SportName: ''
    };
};

MetricGamingReport.prototype.InitPage = function () {
    this.CachedDOMElements();
    this.BindingEvent();
    this.MetricGamingByBetTypeReport.InitPage(this);
};

MetricGamingReport.prototype.CachedDOMElements = function () {
    this.Tables.btnGenerate = $("#btnGenerate");
    this.Tables.btnToday = $("#btnToday");
    this.Tables.btnYesterday = $("#btnYesterday");
    this.Tables.btnExport = $("#btnExport");
    this.Tables.datepickerplugin = $("#datepickerplugin");
    this.Tables.datepicker = $(".ui_nexdatepicker_fromdate");
    this.Tables.reportView = $("#metricGamingReport");
    this.Tables.metricGamingLiveContainer = this.Tables.reportView.find("#liveMetricGamingReportContainer");
    this.Tables.metricGamingLiveContent = this.Tables.reportView.find("#liveMetricGamingReport");

    this.Tables.metricGamingNonLiveContainer = this.Tables.reportView.find("#nonLiveMetricGamingReportContainer");
    this.Tables.metricGamingNoneLiveContent = this.Tables.reportView.find("#nonLiveMetricGamingReport");
};

MetricGamingReport.prototype.BindingEvent = function () {
    var self = this;

    self.Tables.btnGenerate.unbind("click");
    self.Tables.btnGenerate.click(function () {
        self.Tables.btnExport.attr("disabled", null);
        self.GetReport({
            Winlostdate: self.Tables.datepickerplugin.nexDatePicker("getSingleDate")
        });
    });

    self.Tables.btnToday.unbind("click");
    self.Tables.btnToday.click(function () {
        self.Tables.btnExport.attr("disabled", null);
        self.Tables.datepicker.datepicker("setDate", new Date(self.Tables.btnToday.val()));
        self.GetReport({
            Winlostdate: self.Tables.btnToday.val()
        });
    });

    self.Tables.btnYesterday.unbind("click");
    self.Tables.btnYesterday.click(function () {
        self.Tables.btnExport.attr("disabled", null);
        self.Tables.datepicker.datepicker("setDate", new Date(self.Tables.btnYesterday.val()));
        self.GetReport({
            Winlostdate: self.Tables.btnYesterday.val()
        });
    });

    self.Tables.btnExport.unbind("click");
    self.Tables.btnExport.click(function () {
        self.ExportReport();
    });
};

MetricGamingReport.prototype.GetReport = function (params) {
    var self = this;
    AccReport.disableInput();
    self.Params.reportParams.Winlostdate = params.Winlostdate;
    AccReport.utility.CallAjax(this,
        window.rootUrl + "Main/DailyWinLoss2h/GetMetricGamingBettypeReport",
        self.Params.reportParams,
        self.RenderReport,
        self.RenderReport,
        AccReport.enableInput);
};

MetricGamingReport.prototype.RenderReport = function (report) {
    var self = this;
    self.ShowSummaryReport();
    if (report) {
        this.RenderNonLiveReport(report.NoneLiveReport);
        this.RenderLiveReport(report.LiveReport);
    } else {
        this.RenderNonLiveReport(null);
        this.RenderLiveReport(null);
    }
};

MetricGamingReport.prototype.HideReportView = function () {
    AccReport.popUp.hide();
    this.Tables.reportView.addClass("display-none");
    this.Tables.metricGamingLiveContainer.addClass("display-none");
    this.Tables.metricGamingNonLiveContainer.addClass("display-none");
};

MetricGamingReport.prototype.ShowSummaryReport = function () {
    AccReport.popUp.hide();
    this.BindingEvent();
    this.Tables.datepicker.datepicker("setDate", this.Params.reportParams.Winlostdate);
    this.MetricGamingByBetTypeReport.HideReport();
    this.Tables.reportView.removeClass("display-none");
    this.Tables.metricGamingLiveContainer.removeClass("display-none");
    this.Tables.metricGamingNonLiveContainer.removeClass("display-none");
    this.SetActiveReportManager();
    this.ShowHideSiteTypeInfo();
};

MetricGamingReport.prototype.HideSummaryReport = function () {
    AccReport.popUp.hide();
    this.BindingEvent();
    this.Tables.datepicker.datepicker("setDate", this.Params.reportParams.Winlostdate);
    this.Tables.reportView.removeClass("display-none");
    this.Tables.metricGamingLiveContainer.addClass("display-none");
    this.Tables.metricGamingNonLiveContainer.addClass("display-none");
    this.SetActiveReportManager();
};

MetricGamingReport.prototype.ExportReport = function () {
    AccReport.downloadFile(window.rootUrl + "Main/DailyWinLoss2h/ExportMetricGamingBettypeReport", this.Params.reportParams);
};

MetricGamingReport.prototype.GetFullBreadCrumbs = function (drillDownLevel, selectParams) {
    var self = this;
    switch (drillDownLevel) {
        case reportManager.Enum.DrillDownLevel.ByBetType:
            return "<a class='breadcrumbs-link' onclick='showHideReportManager.BackToMainReport();'>" + viewResources.Main + "</a> >" +
                   " <a class='breadcrumbs-link' onclick='reportManager.MetricGamingReport.ShowSummaryReport();'>" + self.CacheData.SportName + "</a> >" +
                   " " + selectParams.BetTypeName;
            break;
        default:
            return "";
    }
};

MetricGamingReport.prototype.GetPlainPreviousBreadCrumbs = function (drillDownLevel, selectParams) {
    var self = this;
    switch (drillDownLevel) {
        case reportManager.Enum.DrillDownLevel.ByBetType:
            return viewResources.Main + " |" +
                   " " + self.CacheData.SportName + " >" +
                   " " + selectParams.BetTypeName;
            break;
        default:
            return "";
    }
};

MetricGamingReport.prototype.SetActiveReportManager = function () {
    if (showHideReportManager.CacheData.currentReport != this) {
        showHideReportManager.CacheData.currentReport.HideReportView();
        showHideReportManager.CacheData.currentReport = this;
    }
};

MetricGamingReport.prototype.ShowHideSiteTypeInfo = function () {
    if (!this.Tables.metricGamingNoneLiveContent.hasClass("display-none")) {
        this.Tables.metricGamingNoneLiveContent.fanexGrid("resizeGrid");
    }

    if (!this.Tables.metricGamingLiveContent.hasClass("display-none")) {
        this.Tables.metricGamingLiveContent.fanexGrid("resizeGrid");
    }
};

MetricGamingReport.prototype.RenderLiveReport = function (report) {
    var self = this;
    this.Tables.metricGamingLiveContent.addClass("display-none");
    var gridOptions = self.GetMetricGamingBetTypesByLiveReport(report, true);
    this.Tables.metricGamingLiveContainer.removeClass("display-none");
    self.ShowReportView();
    this.Tables.metricGamingLiveContent.removeClass("display-none").fanexGrid(gridOptions);
    this.Tables.metricGamingLiveContent.find(".showHideSiteType.showHideIcon").click(function () {
        var btnIcon = $(this);
        showHideSiteTypeManager.ShowHideSiteTypeInfo_ByGroup(btnIcon, self.Tables.metricGamingLiveContent);
    });
};

MetricGamingReport.prototype.RenderNonLiveReport = function (report) {
    var self = this;
    this.Tables.metricGamingNoneLiveContent.addClass("display-none");
    var gridOptions = self.GetMetricGamingBetTypesByLiveReport(report, false);
    this.Tables.metricGamingNonLiveContainer.removeClass("display-none");
    self.ShowReportView();
    this.Tables.metricGamingNoneLiveContent.removeClass("display-none").fanexGrid(gridOptions);
    this.Tables.metricGamingNoneLiveContent.find(".showHideSiteType.showHideIcon").click(function () {
        var btnIcon = $(this);
        showHideSiteTypeManager.ShowHideSiteTypeInfo_ByGroup(btnIcon, self.Tables.metricGamingNoneLiveContent);
    });
};

MetricGamingReport.prototype.GetMetricGamingBetTypesByLiveReport = function (report, isLive) {
    var self = this;
    var bodyData = [];
    var footerData = [];
    var maxColIndex = 11;
    if (report && report.ChildsInfo && report.ChildsInfo.length > 0) {
        var reportLength = report.ChildsInfo.length;
        var info = null;
        var groupdId = '';
        var tableId = isLive ? "liveMetricGamingReport" : "nonLiveMetricGamingReport";
        for (var i = 0; i < reportLength; i++) {
            var betType = report.ChildsInfo[i];
            groupdId = tableId + "_BetType" + betType.Id +
                                 "_Live" + (isLive ? 1 : 0);
            info = betType.GrandTotal;
            bodyData.push({
                Columns: [
                    {
                        Name: "ColBetType",
                        Value: showHideSiteTypeManager.GetShowHideSiteTypeIcon(groupdId) +
                            this.GetReportLink(info, isLive),
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
                            SportType: self.CacheData.SportId,
                            SportGroupId: self.CacheData.SportId,
                            WinLossDate: self.Params.reportParams.Winlostdate,
                            BetType: betType.Id,
                            BetTypeName: betType.Name,
                            IsLive: isLive,
                            IsHedge: false,
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
                            SportType: self.CacheData.SportId,
                            SportGroupId: self.CacheData.SportId,
                            WinLossDate: self.Params.reportParams.Winlostdate,
                            BetType: betType.Id,
                            BetTypeName: betType.Name,
                            IsLive: isLive,
                            IsHedge: false,
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
        groupdId = tableId + "_BetType0_" +
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
    }

    var liveText = isLive ? 'LIVE' : 'NON-LIVE';
    var titleText = "<a class='breadcrumbs-link' onclick='showHideReportManager.BackToMainReport();'>" + viewResources.Main + "</a> >" +
                " " + self.CacheData.SportName.toUpperCase() +
                " \"" +
                liveText +
                "\" " + viewResources.DailyWinLoss2h_ReportTitleSuffix;

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
            text: titleText,
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

MetricGamingReport.prototype.GetReportLink = function (info, isLive) {
    var self = this;
    var evt = "reportManager.MetricGamingReport.GetReport_ByBetType('" + self.Params.reportParams.Winlostdate + "','" +
                    info.Id + "','" +
                    info.Name + "'," +
                    isLive + "," +
                    (info.Hedge && info.Hedge != '' ? true : false) + "); return;";
    var reportLink = '<b>\
                          <a class="report2h-link" onclick="' + evt + '">' + info.Name + '</a>\
                      </b>';
    return reportLink;
};

MetricGamingReport.prototype.GetReport_ByBetType = function (winlostdate,
    betTypeId,
    betTypeName,
    isLive,
    isHedge) {
    var self = this;
    self.BindingEvent_ByBetTypeReport();
    reportManager.Tables.btnShowHideSiteTypeInfo.addClass("display-none");
    self.MetricGamingByBetTypeReport.GetReport(winlostdate,
        betTypeId,
        betTypeName,
        isLive,
        isHedge);
};

MetricGamingReport.prototype.ShowReportView = function () {
    this.Tables.reportView.removeClass("display-none");
};

MetricGamingReport.prototype.BindingEvent_ByBetTypeReport = function () {
    var self = this;

    self.Tables.btnGenerate.unbind("click");
    self.Tables.btnGenerate.click(function () {
        self.Tables.btnExport.attr("disabled", null);
        self.MetricGamingByBetTypeReport.GetReport(self.Tables.datepickerplugin.nexDatePicker("getSingleDate"),
            self.MetricGamingByBetTypeReport.Params.reportParams.Bettype,
            self.MetricGamingByBetTypeReport.Params.reportParams.BetTypeName,
            self.MetricGamingByBetTypeReport.Params.reportParams.Liveindicator,
            self.MetricGamingByBetTypeReport.Params.reportParams.IsHedge
        );
    });

    self.Tables.btnToday.unbind("click");
    self.Tables.btnToday.click(function () {
        self.Tables.btnExport.attr("disabled", null);
        self.Tables.datepicker.datepicker("setDate", new Date(self.Tables.btnToday.val()));
        self.MetricGamingByBetTypeReport.GetReport(self.Tables.btnToday.val(),
            self.MetricGamingByBetTypeReport.Params.reportParams.Bettype,
            self.MetricGamingByBetTypeReport.Params.reportParams.BetTypeName,
            self.MetricGamingByBetTypeReport.Params.reportParams.Liveindicator,
            self.MetricGamingByBetTypeReport.Params.reportParams.IsHedge
        );
    });

    self.Tables.btnYesterday.unbind("click");
    self.Tables.btnYesterday.click(function () {
        self.Tables.btnExport.attr("disabled", null);
        self.Tables.datepicker.datepicker("setDate", new Date(self.Tables.btnYesterday.val()));
        self.MetricGamingByBetTypeReport.GetReport(self.Tables.btnYesterday.val(),
            self.MetricGamingByBetTypeReport.Params.reportParams.Bettype,
            self.MetricGamingByBetTypeReport.Params.reportParams.BetTypeName,
            self.MetricGamingByBetTypeReport.Params.reportParams.Liveindicator,
            self.MetricGamingByBetTypeReport.Params.reportParams.IsHedge
        );
    });

    self.Tables.btnExport.unbind("click");
    self.Tables.btnExport.click(function () {
        self.MetricGamingByBetTypeReport.ExportReport();
    });
};