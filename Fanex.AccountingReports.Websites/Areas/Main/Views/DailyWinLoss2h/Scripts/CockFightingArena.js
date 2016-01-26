CockFightingArena = function () {
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
        reportContainer: null,
        reportContent: null
    };

    self.Params = {
        reportParams: {
            Winlostdate: null
        }
    };

    self.CacheData = {
        PreviousBreadScrum: '',
        PlainPreviousBreadScrum: ''
    };
};

CockFightingArena.prototype.InitPage = function () {
    this.CachedDOMElements();
    this.BindingEvent();
};

CockFightingArena.prototype.CachedDOMElements = function () {
    this.Tables.btnGenerate = $("#btnGenerate");
    this.Tables.btnToday = $("#btnToday");
    this.Tables.btnYesterday = $("#btnYesterday");
    this.Tables.btnExport = $("#btnExport");
    this.Tables.datepickerplugin = $("#datepickerplugin");
    this.Tables.datepicker = $(".ui_nexdatepicker_fromdate");
    this.Tables.reportView = $("#cockFightingArenaReport");
    this.Tables.reportContainer = this.Tables.reportView.find('#cockFightingArenaReport');
    this.Tables.reportContent = this.Tables.reportView.find('#cockFightingArenaContentReport');
};

CockFightingArena.prototype.BindingEvent = function () {
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

CockFightingArena.prototype.GetReport = function (params) {
    var self = this;

    AccReport.disableInput();
    self.Params.reportParams.Winlostdate = params.Winlostdate;
    AccReport.utility.CallAjax(this,
        window.rootUrl + "Main/DailyWinLoss2h/GetCockFightingArenaReport",
        self.Params.reportParams,
        self.RenderReport,
        self.RenderReport,
        AccReport.enableInput);
};

CockFightingArena.prototype.RenderReport = function (report) {
    var self = this;
    this.Tables.reportContainer.removeClass("display-none");
    this.Tables.reportContent.addClass("display-none");
    var bodyData = [];
    var footerData = [];
    var maxColIndex = 11;
    if (report && report.ReportRows && report.ReportRows.length > 0) {
        var reportLength = report.ReportRows.length;
        var info = null;

        for (var i = 0; i < reportLength; i++) {
            var arenaItem = report.ReportRows[i];
            bodyData.push({
                Columns: [{ Name: "ColFightNo", Value: arenaItem.ArenaName, MergeColNumber: 10, TdClass: "arenaName"}]
            });

            var matchLength = arenaItem.CockFightingMatchs.length;
            if (matchLength > 2) {
                for (var j = 0; j < matchLength - 1; j++) {
                    info = arenaItem.CockFightingMatchs[j];
                    bodyData.push({
                        Columns: self.BuildCockFightingMatchInfo(info)
                    });
                }

                info = arenaItem.CockFightingMatchs[matchLength - 1];
                bodyData.push({
                    Columns: self.BuildCockFightingArenaTotal(info)
                });
            } else {
                for (var j = 0; j < matchLength; j++) {
                    info = arenaItem.CockFightingMatchs[j];
                    bodyData.push({
                        Columns: self.BuildCockFightingMatchInfo(info)
                    });
                }
            }
        }

        info = report.GrandTotal;
        footerData.push({
            Columns: [{ Name: "ColFightNo", Value: "Cockfighting Total", MergeColNumber: 4, CellAlign: "right" },
                    { Name: "ColMemberWinLoss", Value: info.WinLoss },
                    { Name: "ColSMAWinLoss", Value: info.Position },
                    { Name: "ColCommission", Value: info.Commission },
                    { Name: "ColCompanyIncome", Value: info.RealLineTotal },
                    { Name: "ColCompanyTurnover", Value: info.RealTurnover },
                    { Name: "ColMargin", Value: info.Margin}]
        });
    }

    var gridOptions = {
        customizeHeader: true,
        groupHeaderRows: [
        {
            Columns: [{ HeaderText: "Fight No.", Name: "ColFightNo", MergeRowNumber: 2 },
                      { HeaderText: "Event", Name: "ColHomeTeam", MergeRowNumber: 2, MergeColNumber: 3 },
                      { HeaderText: "Member Win Loss", Name: "ColMemberWinLoss", MergeRowNumber: 2 },
                      { HeaderText: "SMA Win Loss", Name: "ColSMAWinLoss", MergeRowNumber: 2 },
                      { HeaderText: "Commission", Name: "ColCommission", MergeRowNumber: 2 },
                      { HeaderText: "Company Income", Name: "ColCompanyIncome", MergeRowNumber: 2 },
                      { HeaderText: "Company Turnover", Name: "ColCompanyTurnover", MergeRowNumber: 2 },
                      { HeaderText: "Margin", Name: "ColMargin", MergeRowNumber: 2}]
        }],

        columns: [
            { HeaderText: "Fight No.", Width: 100, Name: "ColFightNo", HeaderAlign: "center", CellAlign: "center" },
            { HeaderText: "Home Team", Width: 100, Name: "ColHomeTeam", HeaderAlign: "center", CellAlign: "right" },
            { HeaderText: "Vs", Width: 30, Name: "ColVs", HeaderAlign: "center", CellAlign: "center" },
            { HeaderText: "Away Team", Width: 100, Name: "ColAwayTeam", HeaderAlign: "center", CellAlign: "left" },
            { HeaderText: "Member Win Loss", Width: 100, Name: "ColMemberWinLoss", FormatNumber: FormatConstant.TWO_DECIMAL_NUMBER, HeaderAlign: "center", CellAlign: "right", FormatPositive: '$' },
            { HeaderText: "SMA Win Loss", Width: 100, Name: "ColSMAWinLoss", FormatNumber: FormatConstant.TWO_DECIMAL_NUMBER, HeaderAlign: "center", CellAlign: "right" },
            { HeaderText: "Commission", Width: 100, Name: "ColCommission", FormatNumber: FormatConstant.TWO_DECIMAL_NUMBER, HeaderAlign: "center", CellAlign: "right" },
            { HeaderText: "Company Income", Width: 100, Name: "ColCompanyIncome", FormatNumber: FormatConstant.TWO_DECIMAL_NUMBER, HeaderAlign: "center", CellAlign: "right", FormatPositive: '$' },
            { HeaderText: "Company Turnover", Width: 100, Name: "ColCompanyTurnover", FormatNumber: FormatConstant.TWO_DECIMAL_NUMBER, HeaderAlign: "center", CellAlign: "right", FormatPositive: '$' },
            { HeaderText: "Margin", Width: 100, Name: "ColMargin", HeaderAlign: "center", FormatNumber: FormatConstant.TWO_DECIMAL_PERCENT_NUMBER, CellAlign: "right", FormatPositive: '$'}],
        showTitle: true,
        showFooter: true,
        titleInfo: {
            text: "<a class='breadcrumbs-link' onclick='showHideReportManager.BackToMainReport();'>" + viewResources.Main + "</a> >" +
            "<a class='breadcrumbs-link' onclick='reportManager.CockFighting.ShowSummaryReport();'>Cockfighting</a> > Cockfighting",
            showExcelButton: false,
            exportExcelFunction: function () {
                ExportReport();
            }
        },
        bodyRows: bodyData,
        footerRows: footerData,
        numberOfFrozenColumn: 0,
        gridExpandHeight: 200,
        showDistinctRow: false,
        showHighlightRow: false,
        headerColumnHeight: 17,
        noDataText: viewResources.EmptyReportMessage
    };

    this.Tables.reportContent.removeClass("display-none").fanexGrid(gridOptions);
};

CockFightingArena.prototype.ExportReport = function () {
    AccReport.downloadFile(window.rootUrl + "Main/DailyWinLoss2h/ExportCockFightingArenaReport", this.Params.reportParams);
};

CockFightingArena.prototype.HideReportView = function () {
    this.Tables.reportView.addClass("display-none");
    this.Tables.reportContent.addClass("display-none");
};

CockFightingArena.prototype.BuildCockFightingMatchInfo = function (info) {
    return [{ Name: "ColFightNo", Value: info.FightNo },
        { Name: "ColHomeTeam", Value: info.HomeOwner },
        { Name: "ColVs", Value: "-vs-" },
        { Name: "ColAwayTeam", Value: info.AwayOwner },
        { Name: "ColMemberWinLoss", Value: info.WinLoss },
        { Name: "ColSMAWinLoss", Value: info.Position },
        { Name: "ColCommission", Value: info.Commission },
        { Name: "ColCompanyIncome", Value: info.RealLineTotal },
        { Name: "ColCompanyTurnover", Value: info.RealTurnover },
        { Name: "ColMargin", Value: info.Margin}];
}

CockFightingArena.prototype.BuildCockFightingArenaTotal = function (info) {
    return [{ Name: "ColFightNo", Value: "", MergeColNumber: 4 },
        { Name: "ColMemberWinLoss", Value: info.WinLoss },
        { Name: "ColSMAWinLoss", Value: info.Position },
        { Name: "ColCommission", Value: info.Commission },
        { Name: "ColCompanyIncome", Value: info.RealLineTotal },
        { Name: "ColCompanyTurnover", Value: info.RealTurnover },
        { Name: "ColMargin", Value: info.Margin}];
}
