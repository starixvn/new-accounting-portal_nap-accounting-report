CombinationWinLoss = function () {
    var self = this;

    self.Tables = {
        btnGenerate: null,
        btnToday: null,
        btnYesterday: null,
        datepickerplugin: null,
        ddlSiteType: null
    };

    self.Params = {
        reportParams: {
            FromDate: null,
            ToDate: null,
            IsHistoryReport: null,
            Site: 0
        }
    };
};

CombinationWinLoss.prototype.InitPage = function () {
    this.CachedDOMElements();
};

CombinationWinLoss.prototype.CachedDOMElements = function () {
    this.Tables.btnGenerate = $("#btnGenerate");
    this.Tables.btnToday = $("#btnToday");
    this.Tables.btnYesterday = $("#btnYesterday");
    this.Tables.datepickerplugin = $("#datepickerplugin");
    this.Tables.fromDatepicker = $(".ui_nexdatepicker_fromdate");
    this.Tables.toDatepicker = $(".ui_nexdatepicker_todate");
    this.Tables.ddlSiteType = $("#ddlSiteType");

    this.Tables.reportContainer = $('#combinationReportContainer');
    this.Tables.report = $('#combinationReport');
};

CombinationWinLoss.prototype.BindingEvent = function () {
    var self = this;

    self.Tables.btnGenerate.unbind("click");
    self.Tables.btnGenerate.click(function () {
        self.GetReport(reportManager.Enum.DateType.SpecifyDay);
    });

    self.Tables.btnToday.unbind("click");
    self.Tables.btnToday.click(function () {
        self.Tables.fromDatepicker.datepicker("setDate", new Date(self.Tables.btnToday.val()));
        self.Tables.toDatepicker.datepicker("setDate", new Date(self.Tables.btnToday.val()));
        self.GetReport(reportManager.Enum.DateType.ToDay);
    });

    self.Tables.btnYesterday.unbind("click");
    self.Tables.btnYesterday.click(function () {
        self.Tables.fromDatepicker.datepicker("setDate", new Date(self.Tables.btnYesterday.val()));
        self.Tables.toDatepicker.datepicker("setDate", new Date(self.Tables.btnYesterday.val()));
        self.GetReport(reportManager.Enum.DateType.YesterDay);
    });
};

CombinationWinLoss.prototype.GetReport = function (dateType) {
    var self = this;

    AccReport.disableInput();
    self.GetParams(dateType);
    $.ajax({
        type: "POST",
        url: window.rootUrl + "WinLoss/DailyMixParlayWinLoss/GetCombinationWinLossReport",
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        data: JSON.stringify(self.Params.reportParams),
        beforeSend: function () {
        },
        success: function (rs) {
            AccReport.popUp.hide();
            self.RenderReport(rs);
        },
        error: function (jqXHR, event, ajaxSettings) {
            if (jqXHR.status && jqXHR.status == AccReportData.Constants.HtmlErrorCode.SystemError) {
                var r = jQuery.parseJSON(jqXHR.responseText);
                AccReport.popUp.show(r.message);
                self.RenderReport(null);
            }
        },
        complete: function () {
            AccReport.enableInput();
        }
    });
};

CombinationWinLoss.prototype.GetParams = function (dateType) {
    switch (dateType) {
        case reportManager.Enum.DateType.SpecifyDay:
            var dateRange = this.Tables.datepickerplugin.nexDatePicker("getDateRange");
            this.Params.reportParams.FromDate = dateRange.FromDate;
            this.Params.reportParams.ToDate = dateRange.ToDate;
            break;
        case reportManager.Enum.DateType.ToDay:
            this.Params.reportParams.FromDate = this.Tables.btnToday.val();
            this.Params.reportParams.ToDate = this.Tables.btnToday.val();
            break;
        case reportManager.Enum.DateType.YesterDay:
            this.Params.reportParams.FromDate = this.Tables.btnYesterday.val();
            this.Params.reportParams.ToDate = this.Tables.btnYesterday.val();
            break;
    }

    this.Params.reportParams.Site = this.Tables.ddlSiteType.val();
    this.Params.reportParams.IsHistoryReport = false;
};

CombinationWinLoss.prototype.RenderReport = function (report) {
    var self = this;

    reportManager.HideAdvanceMixParlayReport();
    self.Tables.reportContainer.removeClass("display-none");
    self.Tables.report.addClass("display-none");
    var bodyData = [];
    var footerData = [];
    if (report && report.ChildsInfo.length > 0) {
        var len = report.ChildsInfo.length;
        var currentRow = null;
        var info = null;
        for (var i = 0; i < len; i++) {
            var combinationGroup = report.ChildsInfo[i];
            var combinationGroupLength = combinationGroup.ChildsInfo.length;
            for (var j = 0; j < combinationGroupLength; j++) {
                var liveTypeGroup = combinationGroup.ChildsInfo[j];
                var liveTypeGroupLength = liveTypeGroup.ChildsInfo.length;
                for (var k = 0; k < liveTypeGroupLength; k++) {
                    info = liveTypeGroup.ChildsInfo[k];
                    var columns = [];
                    if (j == 0 && k == 0) {
                        columns.push({
                            Name: "ColCombination",
                            Value: combinationGroup.Name,
                            MergeRowNumber: combinationGroup.TotalRowDetail + combinationGroupLength + 1,
                            TdClass: "bold col" + combinationGroup.Name.replace(/\s+/g, '').replace(/&/g, 'And')
                        });
                    }

                    if (k == 0) {
                        columns.push({
                            Name: "ColType",
                            Value: liveTypeGroup.Name,
                            MergeRowNumber: liveTypeGroupLength + 1,
                            CssClass: "bold align-center"
                        });
                    }

                    columns.push({ Name: "ColBetType", Value: info.Name });
                    columns.push({ Name: "ColWinLoss", Value: reportManager.FormatNumberCell(info.WinLoss_Str, true, false, false) });
                    columns.push({ Name: "ColMemberTurnover", Value: info.MemberTurnover_Str });
                    columns.push({ Name: "ColBetCount", Value: info.BetCount_Str });
                    columns.push({ Name: "ColPosition", Value: info.Position_Str });
                    columns.push({ Name: "ColExtraPT", Value: info.ExtraPT_Str });
                    columns.push({ Name: "ColComm", Value: info.Commission_Str });
                    columns.push({ Name: "ColLineTotal", Value: reportManager.FormatNumberCell(info.LineTotal_Str, true, false, false) });
                    columns.push({ Name: "ColTurnover", Value: info.Turnover_Str });
                    columns.push({ Name: "ColMargin", Value: reportManager.FormatNumberCell(info.Margin_Str, true, false, true) });
                    bodyData.push({
                        Columns: columns
                    });
                }

                info = liveTypeGroup.GrandTotal;
                bodyData.push({
                    CssClass: "betTypeTotal",
                    Columns: [{ Name: "ColBetType", Value: info.Name, CssClass: "align-center" },
                    { Name: "ColWinLoss", Value: reportManager.FormatNumberCell(info.WinLoss_Str, true, false, false) },
                    { Name: "ColMemberTurnover", Value: info.MemberTurnover_Str },
                    { Name: "ColBetCount", Value: info.BetCount_Str },
                    { Name: "ColPosition", Value: info.Position_Str },
                    { Name: "ColExtraPT", Value: info.ExtraPT_Str },
                    { Name: "ColComm", Value: info.Commission_Str },
                    { Name: "ColLineTotal", Value: reportManager.FormatNumberCell(info.LineTotal_Str, true, false, false) },
                    { Name: "ColTurnover", Value: info.Turnover_Str },
                    { Name: "ColMargin", Value: reportManager.FormatNumberCell(info.Margin_Str, true, false, true)}]
                });
            }

            info = combinationGroup.GrandTotal;
            bodyData.push({
                CssClass: "subTotal",
                Columns: [{ Name: "ColType", Value: "" },
                    { Name: "ColBetType", Value: info.Name, CssClass: "align-center" },
                    { Name: "ColWinLoss", Value: reportManager.FormatNumberCell(info.WinLoss_Str, true, false, false) },
                    { Name: "ColMemberTurnover", Value: info.MemberTurnover_Str },
                    { Name: "ColBetCount", Value: info.BetCount_Str },
                    { Name: "ColPosition", Value: info.Position_Str },
                    { Name: "ColExtraPT", Value: info.ExtraPT_Str },
                    { Name: "ColComm", Value: info.Commission_Str },
                    { Name: "ColLineTotal", Value: reportManager.FormatNumberCell(info.LineTotal_Str, true, false, false) },
                    { Name: "ColTurnover", Value: info.Turnover_Str },
                    { Name: "ColMargin", Value: reportManager.FormatNumberCell(info.Margin_Str, true, false, true)}]
            });
        }

        info = report.GrandTotal;
        footerData.push({
            Columns: [{ Name: "ColCombination", Value: info.Name, MergeColNumber: 3 },
                    { Name: "ColWinLoss", Value: reportManager.FormatNumberCell(info.WinLoss_Str, true, false, false) },
                    { Name: "ColMemberTurnover", Value: info.MemberTurnover_Str },
                    { Name: "ColBetCount", Value: info.BetCount_Str },
                    { Name: "ColPosition", Value: info.Position_Str },
                    { Name: "ColExtraPT", Value: info.ExtraPT_Str },
                    { Name: "ColComm", Value: info.Commission_Str },
                    { Name: "ColLineTotal", Value: reportManager.FormatNumberCell(info.LineTotal_Str, true, false, false), TdClass: "colLineTotal" },
                    { Name: "ColTurnover", Value: info.Turnover_Str },
                    { Name: "ColMargin", Value: reportManager.FormatNumberCell(info.Margin_Str, true, false, true)}]
        });
    }

    var gridOptions = {
        customizeHeader: true,
        groupHeaderRows: [{
            Columns: [{ HeaderText: "Combinations", Name: "ColCombination", MergeRowNumber: 2 },
                { HeaderText: "Type", Name: "ColType", MergeRowNumber: 2 },
                { HeaderText: "Bet Type", Name: "ColBetType", MergeRowNumber: 2 },
                { HeaderText: viewResources.Member, Name: "ColWinLoss", MergeColNumber: 3 },
                { HeaderText: viewResources.SMA_Win_Loss, Name: "ColPosition", MergeRowNumber: 2 },
                { HeaderText: "ExtraPT", Name: "ColExtraPT", MergeRowNumber: 2 },
                { HeaderText: "Commission", Name: "ColComm", MergeRowNumber: 2 },
                { HeaderText: viewResources.House, Name: "ColLineTotal", MergeColNumber: 3}]
        }],
        columns: [{ HeaderText: "Combinations", Width: 140, Name: "ColCombination", HeaderAlign: "center", CellAlign: "center" },
                      { HeaderText: "Type", Width: 140, Name: "ColType", HeaderAlign: "center", CellAlign: "right" },
                      { HeaderText: "Bet Type", Width: 120, Name: "ColBetType", HeaderAlign: "center", CellAlign: "left" },
                      { HeaderText: "Win Loss", Width: 120, Name: "ColWinLoss", HeaderAlign: "center", CellAlign: "right" },
                      { HeaderText: "Turnover", Width: 120, Name: "ColMemberTurnover", HeaderAlign: "center", CellAlign: "right" },
                      { HeaderText: "Bet Count", Width: 120, Name: "ColBetCount", HeaderAlign: "center", CellAlign: "right" },
                      { HeaderText: "SMA Win Loss", Width: 100, Name: "ColPosition", HeaderAlign: "center", CellAlign: "right" },
                      { HeaderText: "Extra PT", Width: 120, Name: "ColExtraPT", HeaderAlign: "center", CellAlign: "right" },
                      { HeaderText: "Commission", Width: 100, Name: "ColComm", HeaderAlign: "center", CellAlign: "right" },
                      { HeaderText: "Real Income", Width: 120, Name: "ColLineTotal", HeaderAlign: "center", CellAlign: "right" },
                      { HeaderText: "Real Turnover", Width: 120, Name: "ColTurnover", HeaderAlign: "center", CellAlign: "right" },
                      { HeaderText: "Margin", Width: 100, Name: "ColMargin", HeaderAlign: "center", CellAlign: "right"}],
        showTitle: true,
        showFooter: true,
        titleInfo:
        {
            text: 'Parlay Combination Win Loss by ' + reportManager.GetSiteTypeName(this.Params.reportParams.Site) +
                    ' - From ' + this.Params.reportParams.FromDate +
                    ' To ' + this.Params.reportParams.ToDate,
            showExcelButton: true,
            exportExcelFunction: function () {
                self.ExportReport();
            }
        },
        showTree: false,
        bodyRows: bodyData,
        footerRows: footerData,
        gridExpandHeight: 230,
        fullWidth: true,
        numberOfFrozenColumn: 3,
        showDistinctRow: false,
        showHighlightRow: false,
        noDataText: viewResources.EmptyReportMessage,
        marginRight: 20
    };

    self.Tables.report.removeClass("display-none").fanexGrid(gridOptions);
};

CombinationWinLoss.prototype.HideReport = function () {
    this.Tables.reportContainer.addClass("display-none");
};

CombinationWinLoss.prototype.ExportReport = function () {
    AccReport.downloadFile(window.rootUrl + "WinLoss/DailyMixParlayWinLoss/ExportCombinationWinLossReport",
        this.Params.reportParams);
};