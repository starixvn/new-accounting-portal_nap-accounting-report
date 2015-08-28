AdvanceMixParlayReport = function () {
    var self = this;

    self.Params = {
        reportParams: {
            FromDate: null,
            ToDate: null,
            IsHistoryReport: null
        }
    };

    self.CachedData = {
        reportWidth: 955
    };
};

AdvanceMixParlayReport.prototype.InitPage = function () {
    this.CachedDOMElements();
};

AdvanceMixParlayReport.prototype.CachedDOMElements = function () {
    this.Tables.reportContainer = $('#advanceMixParlayReportContainer');
    this.Tables.report = $('#advanceMixParlayReport');
};

AdvanceMixParlayReport.prototype.GetReport = function (params) {
    var self = this;

    AccReport.disableInput();
    self.GetParams(params);
    $.ajax({
        type: "POST",
        url: window.rootUrl + "WinLoss/DailyMixParlayWinLoss/GetAdvanceMixParlayReport",
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        data: JSON.stringify(self.Params.reportParams),
        beforeSend: function () {
        },
        success: function (rs) {
            self.RenderReport(rs);
        },
        error: function (event, jqXHR, ajaxSettings) {
            self.RenderReport(null);
        },
        complete: function () {
            AccReport.enableInput();
        }
    });
};

AdvanceMixParlayReport.prototype.GetParams = function (params) {
    this.Params.reportParams.FromDate = params.FromDate;
    this.Params.reportParams.ToDate = params.ToDate;
    this.Params.reportParams.IsHistoryReport = false;
};

AdvanceMixParlayReport.prototype.RenderReport = function (report) {
    var self = this;

    mixParlayPopupManager.HideNormalMixParlayReport();
    self.Tables.reportContainer.removeClass("display-none");
    self.Tables.report.addClass("display-none");
    var bodyData = [];
    var footerData = [];
    if (report && report.ChildsInfo.length > 0) {
        var len = report.ChildsInfo.length;
        var currentRow = null;
        var info = null;
        for (var i = 0; i < len; i++) {
            var liveTypeGroup = report.ChildsInfo[i];
            var liveTypeGroupLength = liveTypeGroup.ChildsInfo.length;
            for (var j = 0; j < liveTypeGroupLength; j++) {
                var sportGroup = liveTypeGroup.ChildsInfo[j];
                var sportGroupLength = sportGroup.ChildsInfo.length;
                for (var k = 0; k < sportGroupLength; k++) {
                    info = sportGroup.ChildsInfo[k];
                    var columns = [];
                    if (j == 0 && k == 0) {
                        columns.push({
                            Name: "ColLiveType",
                            Value: liveTypeGroup.Name,
                            MergeRowNumber: liveTypeGroup.TotalRowDetail + liveTypeGroupLength + 1,
                            TdClass: "bold col" + liveTypeGroup.Name.replace(/\s+/g, '')
                        });
                    }

                    if (k == 0) {
                        columns.push({
                            Name: "ColSportType",
                            Value: sportGroup.Name,
                            MergeRowNumber: sportGroupLength + 1,
                            CssClass: "bold align-center"
                        });
                    }

                    columns.push({ Name: "ColBetType", Value: info.Name });
                    columns.push({ Name: "ColWinLoss", Value: mixParlayPopupManager.FormatNumberCell(info.WinLoss_Str, true, false, false) });
                    columns.push({ Name: "ColPosition", Value: info.Position_Str });
                    columns.push({ Name: "ColComm", Value: info.Commission_Str });
                    columns.push({ Name: "ColLineTotal", Value: mixParlayPopupManager.FormatNumberCell(info.LineTotal_Str, true, false, false) });
                    columns.push({ Name: "ColTurnover", Value: info.Turnover_Str });
                    columns.push({ Name: "ColMargin", Value: mixParlayPopupManager.FormatNumberCell(info.Margin_Str, true, false, true) });
                    bodyData.push({
                        Columns: columns
                    });
                }

                info = sportGroup.GrandTotal;
                bodyData.push({
                    CssClass: "betTypeTotal",
                    Columns: [{ Name: "ColBetType", Value: info.Name, CssClass: "align-center" },
                    { Name: "ColWinLoss", Value: mixParlayPopupManager.FormatNumberCell(info.WinLoss_Str, true, false, false) },
                    { Name: "ColPosition", Value: info.Position_Str },
                    { Name: "ColComm", Value: info.Commission_Str },
                    { Name: "ColLineTotal", Value: mixParlayPopupManager.FormatNumberCell(info.LineTotal_Str, true, false, false) },
                    { Name: "ColTurnover", Value: info.Turnover_Str },
                    { Name: "ColMargin", Value: mixParlayPopupManager.FormatNumberCell(info.Margin_Str, true, false, true)}]
                });
            }

            info = liveTypeGroup.GrandTotal;
            bodyData.push({
                CssClass: "subTotal",
                Columns: [{ Name: "ColSportType", Value: "" },
                    { Name: "ColBetType", Value: info.Name, CssClass: "align-center" },
                    { Name: "ColWinLoss", Value: mixParlayPopupManager.FormatNumberCell(info.WinLoss_Str, true, false, false) },
                    { Name: "ColPosition", Value: info.Position_Str },
                    { Name: "ColComm", Value: info.Commission_Str },
                    { Name: "ColLineTotal", Value: mixParlayPopupManager.FormatNumberCell(info.LineTotal_Str, true, false, false) },
                    { Name: "ColTurnover", Value: info.Turnover_Str },
                    { Name: "ColMargin", Value: mixParlayPopupManager.FormatNumberCell(info.Margin_Str, true, false, true)}]
            });
        }

        info = report.GrandTotal;
        footerData.push({
            Columns: [{ Name: "ColLiveType", Value: info.Name, MergeColNumber: 3 },
                    { Name: "ColWinLoss", Value: mixParlayPopupManager.FormatNumberCell(info.WinLoss_Str, true, false, false) },
                    { Name: "ColPosition", Value: info.Position_Str },
                    { Name: "ColComm", Value: info.Commission_Str },
                    { Name: "ColLineTotal", Value: mixParlayPopupManager.FormatNumberCell(info.LineTotal_Str, true, false, false), TdClass: "colLineTotal" },
                    { Name: "ColTurnover", Value: info.Turnover_Str },
                    { Name: "ColMargin", Value: mixParlayPopupManager.FormatNumberCell(info.Margin_Str, true, false, true)}]
        });
    }

    var gridOptions = {
        customizeHeader: false,
        columns: [{ HeaderText: "", Width: 140, Name: "ColLiveType", HeaderAlign: "center", CellAlign: "center" },
                      { HeaderText: "Sport Type", Width: 140, Name: "ColSportType", HeaderAlign: "center", CellAlign: "right" },
                      { HeaderText: "Bet Type", Width: 120, Name: "ColBetType", HeaderAlign: "center", CellAlign: "left" },
                      { HeaderText: "Win Loss", Width: 120, Name: "ColWinLoss", HeaderAlign: "center", CellAlign: "right" },
                      { HeaderText: "Position", Width: 100, Name: "ColPosition", HeaderAlign: "center", CellAlign: "right" },
                      { HeaderText: "Comm", Width: 100, Name: "ColComm", HeaderAlign: "center", CellAlign: "right" },
                      { HeaderText: "Line Total", Width: 120, Name: "ColLineTotal", HeaderAlign: "center", CellAlign: "right" },
                      { HeaderText: "Turnover", Width: 120, Name: "ColTurnover", HeaderAlign: "center", CellAlign: "right" },
                      { HeaderText: "Margin", Width: 100, Name: "ColMargin", HeaderAlign: "center", CellAlign: "right"}],
        showTitle: true,
        showFooter: true,
        titleInfo:
            {
                text: 'Parlay Win Loss by Product - From ' + moment(this.Params.reportParams.FromDate).format(FormatConstant.MMM_DD_YYYY_STRING) +
                      ' To ' + moment(this.Params.reportParams.ToDate).format(FormatConstant.MMM_DD_YYYY_STRING),
                showExcelButton: true,
                exportExcelFunction: function () {
                    self.ExportReport();
                }
            },
        showTree: false,
        bodyRows: bodyData,
        footerRows: footerData,
        fullHeight: false,
        gridExpandHeight: 500,
        gridWidth: 900,
        numberOfFrozenColumn: 0,
        showDistinctRow: false,
        noDataText: viewResources.EmptyReportMessage,
        showHighlightRow: false
    };

    self.Tables.report.removeClass("display-none").fanexGrid(gridOptions);
    self.Tables.report.find(".free-cell-top-right").css("height", "30px");
};

AdvanceMixParlayReport.prototype.HideReport = function () {
    this.Tables.reportContainer.addClass("display-none");
};

AdvanceMixParlayReport.prototype.ExportReport = function () {
    AccReport.downloadFile(window.rootUrl + "WinLoss/DailyMixParlayWinLoss/ExportAdvanceMixParlayReport",
        this.Params.reportParams);
};