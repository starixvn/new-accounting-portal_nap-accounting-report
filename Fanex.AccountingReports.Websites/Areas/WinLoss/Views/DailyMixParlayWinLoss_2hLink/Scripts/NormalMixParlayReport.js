NormalMixParlayReport = function () {
    var self = this;
    
    self.Params = {
        reportParams: {
            Winlostdate: null,
            Bettype: null,
            IsHistoryReport: null
        }
    };

    self.CachedData = {
        reportWidth: 830
    };
};

NormalMixParlayReport.prototype.InitPage = function () {
    this.CachedDOMElements();
};

NormalMixParlayReport.prototype.CachedDOMElements = function () {
    this.Tables.reportContainer = $('#normalMixParlayContainer');
    this.Tables.report = $('#normalMixParlayReport');
};

NormalMixParlayReport.prototype.GetReport = function (params) {
    var self = this;

    AccReport.disableInput();
    self.GetParams(params);
    $.ajax({
        type: "POST",
        url: window.rootUrl + "WinLoss/DailyMixParlayWinLoss_2hLink/GetMixParlayReport",
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

NormalMixParlayReport.prototype.GetParams = function (params) {
    this.Params.reportParams.Winlostdate = params.FromDate;
    this.Params.reportParams.Bettype = 9;
    this.Params.reportParams.IsHistoryReport = false;
};

NormalMixParlayReport.prototype.RenderReport = function (report) {
    var self = this;

    mixParlayPopupManager.HideAdvanceMixParlayReport();
    self.Tables.reportContainer.removeClass("display-none");
    self.Tables.report.addClass("display-none");
    var bodyData = [];
    var footerData = [];
    if (report && report.ChildsInfo.length > 0) {
        var len = report.ChildsInfo.length;
        var currentRow = null;
        var info = null;
        for (var i = 0; i < len; i++) {
            var sportGroup = report.ChildsInfo[i];
            var sportGroupLength = sportGroup.ChildsInfo.length;
            for (var j = 0; j < sportGroupLength; j++) {
                info = sportGroup.ChildsInfo[j];
                bodyData.push({
                    Columns: [{ Name: "ColBetType", Value: info.Name + " (" + info.Status + ")", CssClass: "bold" },
                    { Name: "ColWinLoss", Value: mixParlayPopupManager.FormatNumberCell(info.WinLoss_Str, true, false, false) },
                    { Name: "ColPosition", Value: info.Position_Str },
                    { Name: "ColComm", Value: info.Commission_Str },
                    { Name: "ColLineTotal", Value: mixParlayPopupManager.FormatNumberCell(info.LineTotal_Str, true, false, false) },
                    { Name: "ColTurnover", Value: info.Turnover_Str },
                    { Name: "ColMargin", Value: mixParlayPopupManager.FormatNumberCell(info.Margin_Str, true, false, true)}]
                });
            }

            info = sportGroup.GrandTotal;
            bodyData.push({
                CssClass: "subTotal",
                Columns: [{ Name: "ColBetType", Value: info.Name, CssClass: "" },
                          { Name: "ColWinLoss", Value: mixParlayPopupManager.FormatNumberCell(info.WinLoss_Str, true, false, false) },
                          { Name: "ColPosition", Value: info.Position_Str },
                          { Name: "ColComm", Value: info.Commission_Str },
                          { Name: "ColLineTotal", Value: mixParlayPopupManager.FormatNumberCell(info.LineTotal_Str, true, false, false) },
                          { Name: "ColTurnover", Value: info.Turnover_Str },
                          { Name: "ColMargin", Value: mixParlayPopupManager.FormatNumberCell(info.Margin_Str, true, false, true)}]
            });

            bodyData.push({
                Columns: [{ Name: "ColBetType", MergeColNumber: 7}]
            });
        }

        info = report.GrandTotal;
        footerData.push({
            Columns: [{ Name: "ColBetType", Value: info.Name, CssClass: "align-right" },
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
        columns: [{ HeaderText: "Bet Type", Width: 120, Name: "ColBetType", HeaderAlign: "center", CellAlign: "left" },
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
                text: "MIX PARLAY PRODUCT WIN/LOSS REPORT",
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

NormalMixParlayReport.prototype.HideReport = function () {
    this.Tables.reportContainer.addClass("display-none");
};

NormalMixParlayReport.prototype.ExportReport = function () {
    AccReport.downloadFile(window.rootUrl + "WinLoss/DailyMixParlayWinLoss_2hLink/ExportMixParlayReport",
        this.Params.reportParams);
};