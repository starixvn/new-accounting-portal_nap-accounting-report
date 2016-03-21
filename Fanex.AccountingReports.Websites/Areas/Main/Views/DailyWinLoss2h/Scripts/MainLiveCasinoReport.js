MainLiveCasinoReport = function () {
    var self = this;

    self.Tables = {
        btnGenerate: null,
        btnToday: null,
        btnYesterday: null,
        btnExport: null,
        btnBack: null,
        datepickerplugin: null,
        datepicker: null,

        reportView: null,
        mainLiveCasinoContentReport: null
    };

    self.Params = {
        reportParams: {
            Winlostdate: null,
            SportId: null
        }
    };

    self.CacheData = {
        SportId: null,
        SportName: ''
    };

    self.gridOptions = null;
};

MainLiveCasinoReport.prototype.InitPage = function () {
    this.CachedDOMElements();
    this.BindingEvent();
};

MainLiveCasinoReport.prototype.CachedDOMElements = function () {
    this.Tables.btnGenerate = $("#btnGenerate");
    this.Tables.btnToday = $("#btnToday");
    this.Tables.btnYesterday = $("#btnYesterday");
    this.Tables.btnExport = $("#btnExport");
    this.Tables.datepickerplugin = $("#datepickerplugin");
    this.Tables.datepicker = $(".ui_nexdatepicker_fromdate");
    this.Tables.reportView = $("#mainLiveCasinoReport");
    this.Tables.mainLiveCasinoReport = this.Tables.reportView.find("#mainLiveCasinoReport");
    this.Tables.mainLiveCasinoContentReport = this.Tables.reportView.find("#mainLiveCasinoContentReport");
};

MainLiveCasinoReport.prototype.BindingEvent = function () {
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

MainLiveCasinoReport.prototype.GetReport = function (params) {
    var self = this;

    AccReport.disableInput();
    self.Params.reportParams = params;
    AccReport.utility.CallAjax(this,
        window.rootUrl + "Main/DailyWinLoss2h/GetMainLiveCasinoReport",
        self.Params.reportParams,
        self.RenderReport,
        self.RenderReport,
        AccReport.enableInput);
};

MainLiveCasinoReport.prototype.RenderReport = function (report) {
    var self = this;
    this.Tables.reportView.addClass("display-none");
    var bodyData = [];
    var footerData = [];
    var maxColIndex = 9;

        if (report && report.ChildsInfo && report.ChildsInfo.length > 0) {
            var reportLength = report.ChildsInfo.length;
            var info = null;
            var tableId = "mainLiveCasinoContentReport";

            for (var i = 0; i < reportLength; i++) {
                var sportType = report.ChildsInfo[i];
                groupdId = tableId + "_SportType" + sportType.Id;
                info = sportType.GrandTotal;
                bodyData.push({
                    Columns: [
                        {
                            Name: "ColSportType",
                            Value: showHideSiteTypeManager.GetShowHideSiteTypeIcon(groupdId) +
                            this.GetSportLink(info),
                            CellAlign: "left",
                            CssClass: "bold"
                        },
                        { Name: "ColWinloss", Value: info.WinLoss },
                        { Name: "ColPosition", Value: info.Position },
                        { Name: "ColCommission", Value: info.Commission },
                        { Name: "ColRealLineTotal", Value: info.LineTotal },
                        { Name: "ColExtraPT", Value: info.IsShowAvailableExtraPositionTaking ? info.ExtraPositionTaking : viewResources.NotAvailable },
                        { Name: "ColAfterExtraPT", Value: info.AfterExtraPositionTaking },
                        { Name: "ColRealTurnover", Value: info.Turnover },
                        reportManager.GetColumnTurnover(info.Margin, info.Turnover, "ColRealMargin")],
                    CssClass: "freegrid-even"
                });

                info = sportType.CreditTotal;
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
                        { Name: "ColExtraPT", Value: info.IsShowAvailableExtraPositionTaking ? info.ExtraPositionTaking : viewResources.NotAvailable },
                        { Name: "ColAfterExtraPT", Value: info.AfterExtraPositionTaking },
                        { Name: "ColRealTurnover", Value: info.IsHideRealTurnover ? viewResources.NotAvailable : info.Turnover },
                        reportManager.GetColumnTurnover(info.Margin, info.Turnover, "ColRealMargin")],
                        CssClass: showHideSiteTypeManager.GetSiteTypeRowCss(groupdId,
                    showHideSiteTypeManager.Enums.RowType.InfoRow)
                });

                info = sportType.DepositTotal;
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
                        { Name: "ColExtraPT", Value: info.IsShowAvailableExtraPositionTaking ? info.ExtraPositionTaking : viewResources.NotAvailable },
                        { Name: "ColAfterExtraPT", Value: info.AfterExtraPositionTaking },
                        { Name: "ColRealTurnover", Value: info.IsHideRealTurnover ? viewResources.NotAvailable : info.Turnover },
                        reportManager.GetColumnTurnover(info.Margin, info.Turnover, "ColRealMargin")],
                        CssClass: showHideSiteTypeManager.GetSiteTypeRowCss(groupdId,
                    showHideSiteTypeManager.Enums.RowType.InfoRow)
                });
            }

            info = report.GrandTotal;
            groupdId = tableId + "_AllTotal";
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
                    { Name: "ColExtraPT", Value: info.ExtraPositionTaking },
                    { Name: "ColAfterExtraPT", Value: info.AfterExtraPositionTaking },
                    { Name: "ColRealTurnover", Value: info.Turnover },
                    reportManager.GetColumnTurnover(info.Margin, info.Turnover, "ColRealMargin")]
            });

            info = report.CreditTotal;
            footerData.push({
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
                    { Name: "ColExtraPT", Value: info.ExtraPositionTaking },
                    { Name: "ColAfterExtraPT", Value: info.AfterExtraPositionTaking },
                    { Name: "ColRealTurnover", Value: info.Turnover },
                    reportManager.GetColumnTurnover(info.Margin, info.Turnover, "ColRealMargin")],
                    CssClass: showHideSiteTypeManager.GetSiteTypeRowCss(groupdId,
                    showHideSiteTypeManager.Enums.RowType.InfoRow)
            });

            info = report.DepositTotal;
            footerData.push({
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
                    { Name: "ColExtraPT", Value: info.ExtraPositionTaking },
                    { Name: "ColAfterExtraPT", Value: info.AfterExtraPositionTaking },
                    { Name: "ColRealTurnover", Value: info.Turnover },
                    reportManager.GetColumnTurnover(info.Margin, info.Turnover, "ColRealMargin")],
                    CssClass: showHideSiteTypeManager.GetSiteTypeRowCss(groupdId,
                    showHideSiteTypeManager.Enums.RowType.InfoRow)
            });
        }

        var gridOptions = {
            customizeHeader: true,
            groupHeaderRows: [{
                Columns: [{ HeaderText: viewResources.Sport_Type, Name: "ColSportType", MergeRowNumber: 2 },
                        { HeaderText: viewResources.Member_WinLoss, Name: "ColWinloss", MergeRowNumber: 2 },
                        { HeaderText: viewResources.SMA_WinLoss, Name: "ColPosition", MergeRowNumber: 2 },
                        { HeaderText: viewResources.Commission, Name: "ColCommission", MergeRowNumber: 2 },
                        { HeaderText: viewResources.Company_Income, Name: "ColRealLineTotal", MergeColNumber: 3, MergeRowNumber: 1 },
                        { HeaderText: viewResources.Company_Turnover, Name: "ColRealTurnover", MergeRowNumber: 2 },
                        { HeaderText: viewResources.Margin, Name: "ColRealMargin", MergeRowNumber: 2 }]
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
                { HeaderText: viewResources.ExtraPT, Width: 100, Name: "ColExtraPT", FormatNumber: FormatConstant.TWO_DECIMAL_NUMBER, HeaderAlign: "center", CellAlign: "right", FormatPositive: '$' },
                { HeaderText: viewResources.AfterExtraPT, Width: 100, Name: "ColAfterExtraPT", FormatNumber: FormatConstant.TWO_DECIMAL_NUMBER, HeaderAlign: "center", CellAlign: "right", FormatPositive: '$' },
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
        
        self.gridOptions = gridOptions;
        this.Tables.reportView.removeClass("display-none");
        this.Tables.mainLiveCasinoReport.removeClass("display-none");
        this.Tables.mainLiveCasinoContentReport.removeClass("display-none").fanexGrid(gridOptions);
        this.Tables.mainLiveCasinoContentReport.find(".showHideSiteType.showHideIcon").click(function () {
            var btnIcon = $(this);
            showHideSiteTypeManager.ShowHideSiteTypeInfo_ByGroup(btnIcon, self.Tables.mainLiveCasinoContentReport);
        });
};

MainLiveCasinoReport.prototype.HideReportView = function () {
    AccReport.popUp.hide();
    this.Tables.reportView.addClass("display-none");
    this.Tables.mainLiveCasinoReport.addClass("display-none");
};

MainLiveCasinoReport.prototype.ShowSummaryReport = function () {
    AccReport.popUp.hide();
    this.BindingEvent();
    this.Tables.datepicker.datepicker("setDate", this.Params.reportParams.Winlostdate);
    this.Tables.reportView.removeClass("display-none");
    this.Tables.mainLiveCasinoReport.removeClass("display-none");
    this.SetActiveReportManager();
};

MainLiveCasinoReport.prototype.ExportReport = function () {
    AccReport.downloadFile(window.rootUrl + "Main/DailyWinLoss2h/ExportMainLiveCasinoReport", this.Params.reportParams);
};

MainLiveCasinoReport.prototype.GetFullBreadCrumbs = function (drillDownLevel, selectParams) {
    var self = this;
    switch (drillDownLevel) {
        case reportManager.Enum.DrillDownLevel.ByBetType:
            return "<a class='breadcrumbs-link' onclick='showHideReportManager.BackToMainReport();'>" + viewResources.Main + "</a> >" +
                   " <a class='breadcrumbs-link' onclick='reportManager.MainLiveCasinoReport.ShowSummaryReport();'>" + self.CacheData.SportName + "</a> >" +
                   " " + selectParams.BetTypeName;
            break;
        default:
            return "";
    }
};

MainLiveCasinoReport.prototype.GetPlainPreviousBreadCrumbs = function (drillDownLevel, selectParams) {
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

MainLiveCasinoReport.prototype.SetActiveReportManager = function () {
    if (showHideReportManager.CacheData.currentReport != this) {
        showHideReportManager.CacheData.currentReport.HideReportView();
        if (!this.Tables.mainLiveCasinoReport.hasClass("display-none")) {
            this.Tables.mainLiveCasinoContentReport.fanexGrid("resizeGrid");
        }
        showHideReportManager.CacheData.currentReport = this;
    }
};

MainLiveCasinoReport.prototype.ShowHideSiteTypeInfo = function () {
    var self = this;
    if (!this.Tables.mainLiveCasinoReport.hasClass("display-none")) {
        this.Tables.mainLiveCasinoContentReport.fanexGrid("resizeGrid");
    }
};

MainLiveCasinoReport.prototype.GetSportLink = function (winLossInfo) {
    sportLink = "<span class='report2h-link bold' onclick=\"showHideReportManager.ShowLiveCasinoWinLossByProductReport('" +
                    winLossInfo.Id + "','" +
                    winLossInfo.Name + "','" +
                    reportManager.Main2hReport.Params.reportParams.Winlostdate + "'); return;\">" +
                    winLossInfo.Name + '</span>';

    return sportLink;
};