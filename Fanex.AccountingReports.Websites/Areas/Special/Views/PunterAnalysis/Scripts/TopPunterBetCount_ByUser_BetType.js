
TopPunterBetCount_ByUser_BetType = function () {
    var self = this;

    self.Tables = {
        reportContainer: null,
        report: null
    };

    self.Params = {
        reportParams: {
            CustID: null,
            UserName: null,
            LeagueID: null,
            Sporttype: null,
            SportName: null,
            Bettype: null,
            Fromdate: null,
            Todate: null,
            Reporttype: null,
            Status: null,
            LiveTypeName: null
        }
    };
};
TopPunterBetCount_ByUser_BetType.prototype.InitPage = function () {
    this.CachedDOMElements();
};
TopPunterBetCount_ByUser_BetType.prototype.CachedDOMElements = function () {
    this.Tables.reportContainer = $('#topPunterBetCount_ByUser_BetType_Container');
    this.Tables.report = $('#topPunterBetCount_ByUser_BetType');
};
TopPunterBetCount_ByUser_BetType.prototype.GetReport = function (params) {
    var self = this;

    AccReport.disableInput();
    self.Params.reportParams = params;
    $.ajax({
        type: "POST",
        url: window.rootUrl + "Special/PunterAnalysis/GetTopPunterBetCount_ByUser_BetType",
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

TopPunterBetCount_ByUser_BetType.prototype.RenderReport = function (report) {
    var self = this;

    this.Tables.reportContainer.removeClass("display-none");
    this.Tables.report.addClass("display-none");
    var bodyData = [];
    var footerData = [];

    if (report && report.ChildsInfo.length > 0) {
        var len = report.ChildsInfo.length;
        var currentRow = null;
        var info = null;
        for (var i = 0; i < len; i++) {
            info = report.ChildsInfo[i];
            bodyData.push({
                Columns: [{ Name: "ColDate", Value: info.Name },
                          { Name: "ColBetCount", Value: info.BetCount_Str },
                          { Name: "ColWinCount", Value: info.WinCount_Str },
                          { Name: "ColDrawCount", Value: info.DrawCount_Str },
                          { Name: "ColWinLoss", Value: reportManager.FormatNumberCell(info.WinLoss_Str, false, true, false)}]
            });
        }

        info = report.GrandTotal;
        footerData.push({
            Columns: [{ Name: "ColDate", Value: info.Name, CssClass: "align-right" },
                      { Name: "ColBetCount", Value: info.BetCount_Str },
                      { Name: "ColWinCount", Value: info.WinCount_Str },
                      { Name: "ColDrawCount", Value: info.DrawCount_Str },
                      { Name: "ColWinLoss", Value: reportManager.FormatNumberCell(info.WinLoss_Str, false, true, false)}]
        });
    }

    var backToMainEvent = "onclick='reportManager.ShowReport()'";
    var backToByUserReportEvent = "onclick='reportManager.ShowReportTopPunterByUser()'";
    var backToBySportTypeReportEvent = "onclick='reportManager.ShowTopPunter_ByUser_SportType()'";
    var title = '';
    if (reportManager.TopPunterByUser.Params.reportParams.IsDirectFromFilter) {
        title = '<span class="link" ' + backToByUserReportEvent + '>Main</span> > ' +
                '<span class="link" ' + backToBySportTypeReportEvent + '>' + self.Params.reportParams.SportName + '</span> > ' +
                '<span>' + self.Params.reportParams.BetTypeName + ' - ' + self.Params.reportParams.LiveTypeName + '</span>';
    }
    else {
        title = '<span class="link" ' + backToMainEvent + '>Main</span> > ' +
                '<span class="link" ' + backToByUserReportEvent + '>' + self.Params.reportParams.UserName + '</span> > ' +
                '<span class="link" ' + backToBySportTypeReportEvent + '>' + self.Params.reportParams.SportName + '</span> > ' +
                '<span>' + self.Params.reportParams.BetTypeName + ' - ' + self.Params.reportParams.LiveTypeName + '</span>';
    }

    var gridOptions = {
        customizeHeader: false,
        columns: [{ HeaderText: "Date", Width: 140, Name: "ColDate", HeaderAlign: "center", CellAlign: "center" },
                  { HeaderText: "Bet Count", Width: 100, Name: "ColBetCount", HeaderAlign: "center", CellAlign: "right" },
                  { HeaderText: "Win Count", Width: 100, Name: "ColWinCount", HeaderAlign: "center", CellAlign: "right" },
                  { HeaderText: "Draw Count", Width: 100, Name: "ColDrawCount", HeaderAlign: "center", CellAlign: "right" },
                  { HeaderText: "Win Loss", Width: 120, Name: "ColWinLoss", HeaderAlign: "center", CellAlign: "right"}],
        showTitle: true,
        showFooter: true,
        titleInfo:
        {
            text: title,
            showExcelButton: true,
            exportExcelFunction: function () {
                self.ExportReport();
            }
        },
        showTree: false,
        bodyRows: bodyData,
        footerRows: footerData,
        gridExpandHeight: 250,
        numberOfFrozenColumn: 1,
        gridWidth: 900,
        showDistinctRow: false,
        showHighlightRow: false
    };

    reportManager.TopPunter_ByUser_SportType.HideReport();
    this.Tables.report.removeClass("display-none").fanexGrid(gridOptions);
};
TopPunterBetCount_ByUser_BetType.prototype.HideReport = function () {
    this.Tables.reportContainer.addClass("display-none");
    this.Tables.report.addClass("display-none");
};
TopPunterBetCount_ByUser_BetType.prototype.ExportReport = function () {
    AccReport.downloadFile(window.rootUrl + "Special/PunterAnalysis/ExportTopPunterBetCount_ByUser_BetType", this.Params.reportParams);
};
