var ParlayReport = function () {
    var self = this;

    self.tables = {
        btnGenerate: null,
        btnToday: null,
        btnYesterday: null,
        btnExport: null,
        btnBack: null,
        datepickerplugin: null,
        datepicker: null,
        datePickerInput: null,
        reportView: null,
        reportContainer: null,
        reportContent: null
    };

    self.reportParams = {
        sportId: null,
        sportName: null,
        winlostdate: null
    };

    self.CacheData = {
        SportId: null,
        SportName: null,
        Winlostdate: null
    };
};

ParlayReport.prototype.InitPage = function () {
    this.cachedDomElements();
    this.bindingEvents();
};

ParlayReport.prototype.cachedDomElements = function () {
    this.tables.btnGenerate = $("#btnGenerate");
    this.tables.btnToday = $("#btnToday");
    this.tables.btnYesterday = $("#btnYesterday");
    this.tables.btnExport = $("#btnExport");
    this.tables.datepickerplugin = $("#datepickerplugin");
    this.tables.datepicker = $(".ui_nexdatepicker_fromdate");
    this.tables.datePickerInput = $("#datePickerInput");
    this.tables.reportContainer = $("#palayReportContainer");
    this.tables.reportContent = $("#parlayReportContent");
    this.tables.btnBack = this.tables.reportContent.find(".btnBack");
};

ParlayReport.prototype.bindingEvents = function () {
    var self = this;

    self.tables.btnGenerate.unbind("click");
    self.tables.btnGenerate.click(function () {
        self.tables.btnExport.attr("disabled", null);
        self.GetReport({
            Winlostdate: self.tables.datepickerplugin.nexDatePicker("getSingleDate")
        });
    });

    self.tables.btnToday.unbind("click");
    self.tables.btnToday.click(function () {
        self.tables.btnExport.attr("disabled", null);
        self.tables.datepicker.datepicker("setDate", new Date(self.tables.btnToday.val()));
        self.GetReport({
            Winlostdate: self.tables.btnToday.val()
        });
    });

    self.tables.btnYesterday.unbind("click");
    self.tables.btnYesterday.click(function () {
        self.tables.btnExport.attr("disabled", null);
        self.tables.datepicker.datepicker("setDate", new Date(self.tables.btnYesterday.val()));
        self.GetReport({
            Winlostdate: self.tables.btnYesterday.val()
        });
    });

    self.tables.btnExport.unbind("click");
    self.tables.btnExport.click(function () {
        self.ExportReport();
    });
};

ParlayReport.prototype.RebindingEvent = function () {
    var self = this;
    self.tables.btnGenerate.unbind("click");
    self.tables.btnToday.unbind("click");
    self.tables.btnYesterday.unbind("click");
    self.tables.btnExport.unbind("click");
    self.bindingEvents();
};

ParlayReport.prototype.GetReport = function (params) {
    var self = this;
    AccReport.disableInput();
    this.reportParams = params;
    reportManager.Main2hReport.Params.reportParams.Winlostdate = reportManager.Tables.datepickerplugin.nexDatePicker("getSingleDate");
    AccReport.utility.CallAjax(this,
        window.rootUrl + "Main/DailyWinLoss2h/GetParlayReport",
        self.reportParams,
        self.RenderReport,
        self.RenderReport,
        AccReport.enableInput);
};

ParlayReport.prototype.RenderReport = function (report) {
    var self = this;
    this.tables.reportContainer.addClass("display-none");
    this.tables.reportContent.addClass("display-none");
    var bodyData = [];
    var footerData = [];
    var maxColIndex = 7;
    if (report && report.ChildsInfo && report.ChildsInfo.length > 0) {
        var reportLength = report.ChildsInfo.length;
        var info = null;
        var groupdId = '';
        var tableId = "parlayReport";

        for (var i = 0; i < reportLength; i++) {
            var betType = report.ChildsInfo[i];
            groupdId = tableId + "_BetType" + betType.Id;

            var info = betType.GrandTotal;
            bodyData.push({
                Columns: [
                    {
                        Name: "ColSportType",
                        Value: showHideSiteTypeManager.GetShowHideSiteTypeIcon(groupdId) +
                            reportManager.Main2hReport.GetSportLink(info),
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
                        Name: "ColSportType",
                        Value: info.Name,
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
                        Name: "ColSportType",
                        Value: info.Name,
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
        groupdId = tableId + "_BetType0_Hedge0_Live0";
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
                    { Name: "ColRealLineTotal", Value: info.LineTotal },
                    { Name: "ColVirtualLineTotal", Value: info.VirtualLineTotal },
                    { Name: "ColVariance", Value: info.Variance },
                    { Name: "ColRealTurnover", Value: info.Turnover },
                    { Name: "ColVirtualTurnover", Value: info.VirtualTurnover },
                        reportManager.GetColumnTurnover(info.Margin, info.Turnover, "ColRealMargin"),
                        reportManager.GetColumnTurnover(info.VirtualMargin, info.VirtualTurnover, "ColVirtualMargin")]
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

    var gridOptions = {
        customizeHeader: true,
        groupHeaderRows: [{
            Columns: [{ HeaderText: viewResources.Sport_Type, Name: "ColSportType", MergeRowNumber: 2 },
                          { HeaderText: viewResources.Member_WinLoss, Name: "ColWinloss", MergeRowNumber: 2 },
                          { HeaderText: viewResources.SMA_WinLoss, Name: "ColPosition", MergeRowNumber: 2 },
                          { HeaderText: viewResources.Commission, Name: "ColCommission", MergeRowNumber: 2 },
                          { HeaderText: viewResources.Company_Income, Name: "ColRealLineTotal", MergeColNumber: 2 },
                          { HeaderText: viewResources.Variance, Name: "ColVariance", MergeRowNumber: 2 },
                          { HeaderText: viewResources.Company_Turnover, Name: "ColRealTurnover", MergeColNumber: 2 },
                          { HeaderText: viewResources.Margin, Name: "ColRealMargin", MergeColNumber: 2}]
        }],

        columns: [
            {
                HeaderText: viewResources.Sport_Type,
                Width: 180, Name: "ColSportType",
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
            { HeaderText: viewResources.Virtual, Width: 100, Name: "ColVirtualMargin", HeaderAlign: "center", FormatNumber: FormatConstant.TWO_DECIMAL_PERCENT_NUMBER, CellAlign: "right", FormatPositive: '$'}],
        showTitle: true,
        showFooter: true,
        titleInfo: {
            text: "<a class='breadcrumbs-link' onclick='showHideReportManager.BackToMainReport();'>" + viewResources.Main + "</a> >" +
                " " + self.CacheData.SportName +
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

    this.tables.reportContainer.removeClass("display-none");
    this.tables.reportContent.removeClass("display-none").fanexGrid(gridOptions);
    this.tables.reportContent.find(".showHideSiteType.showHideIcon").click(function () {
        var btnIcon = $(this);
        showHideSiteTypeManager.ShowHideSiteTypeInfo_ByGroup(btnIcon, self.tables.reportContent);
    });
};

ParlayReport.prototype.ExportReport = function () {
    AccReport.downloadFile(window.rootUrl + "Main/DailyWinLoss2h/ExportParlayReport", this.reportParams);
};

ParlayReport.prototype.HideReportView = function () {
    this.tables.reportContainer.addClass("display-none");
};

ParlayReport.prototype.ShowHideSiteTypeInfo = function () {
    if (!this.tables.reportContainer.hasClass("display-none")) {
        this.tables.reportContent.fanexGrid("resizeGrid");
    }
};
