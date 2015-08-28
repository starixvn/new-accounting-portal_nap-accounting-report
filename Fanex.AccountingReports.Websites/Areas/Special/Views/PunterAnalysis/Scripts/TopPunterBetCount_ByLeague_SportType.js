TopPunterBetCount_ByLeague_SportType = function () {
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
            Sportbettype: null,
            Fromdate: null,
            Todate: null,
            Reporttype: null,
            ByLeague: null,
            SportName: null,
            LeagueName: null,
            ByLeague: true
        }
    };
};
TopPunterBetCount_ByLeague_SportType.prototype.InitPage = function () {
    this.CachedDOMElements();
};
TopPunterBetCount_ByLeague_SportType.prototype.CachedDOMElements = function () {
    this.Tables.reportContainer = $('#topPunterBetCount_ByLeague_SportType_Container');
    this.Tables.report = $('#topPunterBetCount_ByLeague_SportType');
};
TopPunterBetCount_ByLeague_SportType.prototype.GetReport = function (params) {
    var self = this;

    AccReport.disableInput();
    self.Params.reportParams = params;
    $.ajax({
        type: "POST",
        url: window.rootUrl + "Special/PunterAnalysis/GetTopPunterBetCount_ByUser",
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
TopPunterBetCount_ByLeague_SportType.prototype.RenderReport = function (report) {
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

    var backToMainEvent = "onclick='reportManager.TopPunter_ByLeague.ShowReport()'";
    var backToByUserReportEvent = "onclick='reportManager.TopPunter_ByLeague.ShowReportTopPunterByUser()'";
    var title = '<span class="link" ' + backToMainEvent + '>Main</span> > ' +
                '<span class="link" ' + backToByUserReportEvent + '>' + self.Params.reportParams.UserName + '</span> > ' +
                '<span>' + self.Params.reportParams.SportName + '</span>';

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
        gridExpandHeight: 200,
        numberOfFrozenColumn: 1,
        gridWidth: 900,
        showDistinctRow: false,
        showHighlightRow: false
    };

    reportManager.TopPunter_ByLeague.TopPunterByUser.HideReport();
    this.Tables.report.removeClass("display-none").fanexGrid(gridOptions);
};

TopPunterBetCount_ByLeague_SportType.prototype.HideReport = function () {
    this.Tables.reportContainer.addClass("display-none");
    this.Tables.report.addClass("display-none");
};
TopPunterBetCount_ByLeague_SportType.prototype.ExportReport = function () {
    AccReport.downloadFile(window.rootUrl + "Special/PunterAnalysis/ExportTopPunterBetCount_ByUser", this.Params.reportParams);
};
