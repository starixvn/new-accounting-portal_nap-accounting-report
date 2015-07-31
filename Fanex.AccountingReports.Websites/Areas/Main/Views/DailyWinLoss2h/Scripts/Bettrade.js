Bettrade = function () {
    var self = this;

    self.Tables = {
        btnGenerate: null,
        btnToday: null,
        btnYesterday: null,
        btnExport: null,
        btnBack: null,
        datepickerplugin: null,
        datepicker: null,

        reportView: null
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

Bettrade.prototype.InitPage = function () {
    this.CachedDOMElements();
    this.BindingEvent();
};

Bettrade.prototype.CachedDOMElements = function () {
    this.Tables.btnGenerate = $("#btnGenerate");
    this.Tables.btnToday = $("#btnToday");
    this.Tables.btnYesterday = $("#btnYesterday");
    this.Tables.btnExport = $("#btnExport");
    this.Tables.datepickerplugin = $("#datepickerplugin");
    this.Tables.datepicker = $(".ui_nexdatepicker_fromdate");
    this.Tables.reportView = $("#bettradeReport");
    this.Tables.bettradeReport = this.Tables.reportView.find("#bettradeReport");
    this.Tables.bettradeReportContent = this.Tables.reportView.find("#bettradeContentReport");
};

Bettrade.prototype.BindingEvent = function () {
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

Bettrade.prototype.GetReport = function (params) {
    var self = this;
    AccReport.disableInput();
    self.Params.reportParams.Winlostdate = params.Winlostdate;
    AccReport.utility.CallAjax(this,
        window.rootUrl + "Main/DailyWinLoss2h/GetBettradeReport",
        self.Params.reportParams,
        self.RenderReport,
        self.RenderReport,
        AccReport.enableInput);
};

Bettrade.prototype.RenderReport = function (report) {
    var self = this;
    this.Tables.reportView.addClass("display-none");
    var bodyData = [];
    var footerData = [];
    var maxColIndex = 7;

    if (report && report.ChildsInfo && report.ChildsInfo.length > 0) {
        var reportLength = report.ChildsInfo.length;
        var info = null;
        var groupdId = '';
        var tableId = "bettradeContentReport";

        for (var i = 0; i < reportLength; i++) {
            var betType = report.ChildsInfo[i];
            groupdId = tableId + "_BetType" + betType.Id +
                                 "_Hedge0_Live0";

            info = betType.GrandTotal;
            bodyData.push({
                Columns: [
                    {
                        Name: "ColBetType",
                        Value: showHideSiteTypeManager.GetShowHideSiteTypeIcon(groupdId) +
                            info.Name,
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
                            SportType: self.CacheData.SportId,
                            SportGroupId: self.CacheData.SportId,
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
                    { Name: "ColRealTurnover", Value: info.IsHideRealTurnover ? viewResources.NotAvailable : info.Turnover },
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
                            SportType: self.CacheData.SportId,
                            SportGroupId: self.CacheData.SportId,
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
                    { Name: "ColRealTurnover", Value: info.IsHideRealTurnover ? viewResources.NotAvailable : info.Turnover },
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
            Columns: [
                {
                    Name: "ColBetType",
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

        info = report.DepositTotal;
        footerData.push({
            Columns: [
                {
                    Name: "ColBetType",
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
                " " + self.CacheData.SportName.toUpperCase() +
                " " + viewResources.DailyWinLoss2h_ReportTitleSuffix,
            showExcelButton: false
        },
        bodyRows: bodyData,
        footerRows: footerData,
        numberOfFrozenColumn: 0,
        gridWidth: 1400,
        gridExpandHeight: 200,
        fullWidth: false,
        fullHeight: true,
        gridHeight: 400,
        showDistinctRow: false,
        showHighlightRow: false,
        noDataText: viewResources.EmptyReportMessage,
        headerColumnHeight: 29
    };

    this.Tables.reportView.removeClass("display-none");
    this.Tables.bettradeReport.removeClass("display-none");
    this.Tables.bettradeReportContent.removeClass("display-none").fanexGrid(gridOptions);
    this.Tables.bettradeReportContent.find(".showHideSiteType.showHideIcon").click(function () {
        var btnIcon = $(this);
        showHideSiteTypeManager.ShowHideSiteTypeInfo_ByGroup(btnIcon, self.Tables.bettradeReportContent);
    });
};

Bettrade.prototype.HideReportView = function () {
    AccReport.popUp.hide();
    this.Tables.reportView.addClass("display-none");
    this.Tables.bettradeReport.addClass("display-none");
};

Bettrade.prototype.ShowSummaryReport = function () {
    AccReport.popUp.hide();
    this.BindingEvent();
    this.Tables.datepicker.datepicker("setDate", this.Params.reportParams.Winlostdate);
    this.Tables.reportView.removeClass("display-none");
    this.Tables.bettradeReport.removeClass("display-none");
    this.SetActiveReportManager();
};

Bettrade.prototype.ExportReport = function () {
    AccReport.downloadFile(window.rootUrl + "Main/DailyWinLoss2h/ExportBettradeReport", this.Params.reportParams);
};

Bettrade.prototype.GetFullBreadCrumbs = function (drillDownLevel, selectParams) {
    var self = this;
    switch (drillDownLevel) {
        case reportManager.Enum.DrillDownLevel.ByBetType:
            return "<a class='breadcrumbs-link' onclick='showHideReportManager.BackToMainReport();'>" + viewResources.Main + "</a> >" +
                   " <a class='breadcrumbs-link' onclick='reportManager.Bettrade.ShowSummaryReport();'>" + self.CacheData.SportName + "</a> >" +
                   " " + selectParams.BetTypeName;
            break;
        default:
            return "";
    }
};

Bettrade.prototype.GetPlainPreviousBreadCrumbs = function (drillDownLevel, selectParams) {
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

Bettrade.prototype.SetActiveReportManager = function () {
    if (showHideReportManager.CacheData.currentReport != this) {
        showHideReportManager.CacheData.currentReport.HideReportView();
        showHideReportManager.CacheData.currentReport = this;
    }
};

Bettrade.prototype.ShowHideSiteTypeInfo = function () {
    if (!this.Tables.bettradeReport.hasClass("display-none")) {
        this.Tables.bettradeReportContent.fanexGrid("resizeGrid");
    }
};