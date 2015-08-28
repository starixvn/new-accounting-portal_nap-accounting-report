// CombinationMixParlay-98
SystemParlayReport = function () {
    var self = this;

    self.Tables = {
        btnGenerate: null,
        btnToday: null,
        btnYesterday: null,
        btnExport: null,
        datepickerplugin: null,
        datepicker: null,

        reportView: null,
        reportTable: null
    };

    self.Params = {
        reportParams: {
            Winlostdate: null,
            SportId: null,
            Bettype: AccReportData.Constants.BetType.SystemParlay,
            site: 0
        }
    };

    self.CacheData = {
        SportId: null,
        SportName: null
    };
};

SystemParlayReport.prototype.InitPage = function () {
    this.CachedDOMElements();
    this.BindingEvent();
    reportManager.Tables.siteTypeCell.removeClass("display-none");
    this.Tables.datepicker.datepicker("option", "minDate", balanceUpDay);
};

SystemParlayReport.prototype.CachedDOMElements = function () {
    this.Tables.btnGenerate = $("#btnGenerate");
    this.Tables.btnToday = $("#btnToday");
    this.Tables.btnYesterday = $("#btnYesterday");
    this.Tables.btnExport = $("#btnExport");
    this.Tables.datepickerplugin = $("#datepickerplugin");
    this.Tables.datepicker = $(".ui_nexdatepicker_fromdate");

    this.Tables.reportView = $("#systemParlayReport");
    this.Tables.reportTable = this.Tables.reportView.find("#tblContent_SystemParlay");
};

SystemParlayReport.prototype.BindingEvent = function () {
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

SystemParlayReport.prototype.GetReport = function (params) {
    var self = this;

    AccReport.disableInput();
    self.Params.reportParams = params;
    self.Params.reportParams.Bettype = AccReportData.Constants.BetType.SystemParlay;
    self.Params.reportParams.site = reportManager.Tables.ddlSiteType.val();
    AccReport.utility.CallAjax(this,
        window.rootUrl + "Main/DailyWinLoss2h/GetMixParlayReport",
        self.Params.reportParams,
        self.RenderReport,
        self.RenderReport,
        AccReport.enableInput);
};

SystemParlayReport.prototype.RenderReport = function (report) {
    var self = this;
    this.Tables.reportTable.addClass("display-none");
    var bodyData = [];
    var footerData = [];
    var maxColIndex = 7;
    if (report && report.ChildsInfo && report.ChildsInfo.length > 0) {
        var info;
        var sportGroupsLength = report.ChildsInfo.length;
        for (var i = 0; i < sportGroupsLength; i++) {
            var sportGroup = report.ChildsInfo[i];
            var reportLength = sportGroup.ChildsInfo.length;
            for (var j = 0; j < reportLength; j++) {
                info = sportGroup.ChildsInfo[j];
                bodyData.push({
                    Columns: [
                        {
                            Name: "ColBetType",
                            Value: info.Name + '(' + info.Status + ')',
                            CellAlign: "left",
                            CssClass: "bold"
                        },
                        { Name: "ColWinloss", Value: info.WinLoss },
                        { Name: "ColPosition", Value: info.Position },
                        { Name: "ColCommission", Value: info.Commission },
                        { Name: "ColRealLineTotal", Value: info.LineTotal },
                        { Name: "ColRealTurnover", Value: info.IsHideRealTurnover ? viewResources.NotAvailable : info.Turnover },
                        reportManager.GetColumnTurnover(info.Margin, info.Turnover, "ColRealMargin")]
                });
            }

            info = sportGroup.GrandTotal;
            bodyData.push({
                Columns: [
                    {
                        Name: "ColBetType",
                        Value: info.Name,
                        CellAlign: "left",
                        CssClass: "bold"
                    },
                    { Name: "ColWinloss", Value: info.WinLoss },
                    { Name: "ColPosition", Value: info.Position },
                    { Name: "ColCommission", Value: info.Commission },
                    { Name: "ColRealLineTotal", Value: info.LineTotal },
                    { Name: "ColRealTurnover", Value: info.IsHideRealTurnover ? viewResources.NotAvailable : info.Turnover },
                    reportManager.GetColumnTurnover(info.Margin, info.Turnover, "ColRealMargin")],
                CssClass: "subTotalRow"
            });

            bodyData.push({
                Columns: [{ Name: "ColBetType", MergeColNumber: maxColIndex}]
            });
        }

        info = report.GrandTotal;
        footerData.push({
            Columns: [{ Name: "ColBetType", Value: info.Name, CellAlign: "right", CssClass: "bold" },
                        { Name: "ColWinloss", Value: info.WinLoss },
                        { Name: "ColPosition", Value: info.Position },
                        { Name: "ColCommission", Value: info.Commission },
                        { Name: "ColRealLineTotal", Value: info.LineTotal, TdClass: "colNote" },
                        { Name: "ColRealTurnover", Value: info.IsHideRealTurnover ? viewResources.NotAvailable : info.Turnover },
                        reportManager.GetColumnTurnover(info.Margin, info.Turnover, "ColRealMargin")]
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
                " " +
                "<a class='breadcrumbs-link' onclick='showHideReportManager.BackToParlayReport();'>PARLAY</a> >" +
                " " + self.CacheData.SportName +
                " " + viewResources.DailyWinLoss2h_ReportTitleSuffix +
                " BY " + reportManager.WinLossBySite_BetType.GetSiteTypeName(self.Params.reportParams.site),
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
    this.Tables.reportTable.removeClass("display-none").fanexGrid(gridOptions);
};

SystemParlayReport.prototype.HideReportView = function () {
    AccReport.popUp.hide();
    reportManager.Tables.siteTypeCell.addClass("display-none");
    this.Tables.reportView.addClass("display-none");
    this.Tables.datepicker.datepicker("option", "minDate", minDate);
    reportManager.Tables.btnShowHideSiteTypeInfo.removeClass("display-none");
};

SystemParlayReport.prototype.ExportReport = function () {
    AccReport.downloadFile(window.rootUrl + "Main/DailyWinLoss2h/ExportSystemParlayReport", this.Params.reportParams);
};