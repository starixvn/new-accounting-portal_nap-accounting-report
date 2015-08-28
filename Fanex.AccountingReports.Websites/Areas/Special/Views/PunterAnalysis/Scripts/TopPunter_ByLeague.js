

TopPunter_ByLeague = function () {
    var self = this;

    self.Tables = {
        reportContainer: null,
        report: null
    };

    self.Params = {
        reportParams: {
            LeagueID: null,
            Sporttype: null,
            Bettype: null,
            Fromdate: null,
            Todate: null,
            TopRow: null,
            Reporttype: null,

            OrderBy: null,
            OrderType: null,
            OrderByName: null,
            OrderTypeName: null,
            SportName: null,
            LeagueName: null
        }
    };

    self.TopPunterByUser = new TopPunter_ByLeague_User();
    self.TopPunterBetCountByUser = new TopPunterBetCount_ByLeague_User();
    self.TopPunter_ByUser_SportType = new TopPunter_ByLeague_SportType();
    self.TopPunterBetCount_ByUser_SportType = new TopPunterBetCount_ByLeague_SportType();
    self.TopPunterBetCount_ByUser_BetType = new TopPunterBetCount_ByLeague_BetType();
    self.TopPunter_ByUser_BetType = new TopPunter_ByLeague_BetType();
    self.CacheData = {
    };
};
TopPunter_ByLeague.prototype.InitPage = function () {
    this.CachedDOMElements();
    this.TopPunterByUser.InitPage();
    this.TopPunterBetCountByUser.InitPage();
    this.TopPunter_ByUser_SportType.InitPage();
    this.TopPunterBetCount_ByUser_SportType.InitPage();
    this.TopPunterBetCount_ByUser_BetType.InitPage();
    this.TopPunter_ByUser_BetType.InitPage();
};
TopPunter_ByLeague.prototype.CachedDOMElements = function () {
    this.Tables.reportContainer = $('#topPunterReportByLeagueContainer');
    this.Tables.report = $('#topPunterReportByLeague');

    this.Tables.summaryReportContainer = $('#summaryReport_ByUser_BetType');
    this.Tables.summaryReport = $('#summaryReport_ByUser_BetTypeContent');
};
TopPunter_ByLeague.prototype.RenderReport = function (report) {
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
                Columns: [
                         {
                             Name: "ColUsername",
                             Value: info,
                             CustomCellRender: function (currentCell) {
                                 var clickEvent = ' onclick = "reportManager.TopPunter_ByLeague.GetTopPunterByUser(\'' + currentCell.Value.Id + '\',\'' +
                                                                                                currentCell.Value.Name + "');\"";
                                 return '<div class="link" ' + clickEvent + '>' + currentCell.Value.Name + '</div>';
                             }
                         },
                          { Name: "ColWinLoss", Value: reportManager.FormatNumberCell(info.WinLoss_Str, false, true, false) },
                          { Name: "ColTurnover", Value: info.Turnover_Str },
                          { Name: "ColMargin", Value: reportManager.FormatNumberCell(info.Margin_Str, false, true, false) },
                          {
                              Name: "ColBetCount",
                              Value: info,
                              CustomCellRender: function (currentCell) {
                                  var clickEvent = ' onclick = "reportManager.TopPunter_ByLeague.GetTopPunterBetCount_ByUser(\'' + currentCell.Value.Id + '\',\'' +
                                                                                         currentCell.Value.Name + "');\"";

                                  return '<div class="link" ' + clickEvent + '>' + currentCell.Value.BetCount + '</div>';
                              }
                          }]
            });
        }

        info = report.GrandTotal;
        footerData.push({
            Columns: [{ Name: "ColUsername", Value: info.Name, CssClass: "align-right" },
                      { Name: "ColWinLoss", Value: reportManager.FormatNumberCell(info.WinLoss_Str, false, true, false) },
                      { Name: "ColTurnover", Value: info.Turnover_Str },
                      { Name: "ColMargin", Value: reportManager.FormatNumberCell(info.Margin_Str, false, true, false) },
                      { Name: "ColBetCount", Value: info.BetCount_Str}]
        });
    }

    var gridOptions = {
        customizeHeader: false,
        columns: [{ HeaderText: "Username", Width: 140, Name: "ColUsername", HeaderAlign: "center", CellAlign: "left" },
                  { HeaderText: "Win Loss", Width: 120, Name: "ColWinLoss", HeaderAlign: "center", CellAlign: "right" },
                  { HeaderText: "Turnover", Width: 120, Name: "ColTurnover", HeaderAlign: "center", CellAlign: "right" },
                  { HeaderText: "Margin", Width: 80, Name: "ColMargin", HeaderAlign: "center", CellAlign: "right" },
                  { HeaderText: "Bet Count", Width: 100, Name: "ColBetCount", HeaderAlign: "center", CellAlign: "right"}],
        showTitle: true,
        showFooter: true,
        titleInfo:
            {
                text: 'Main - Punter Analysis',
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

    this.Tables.report.removeClass("display-none").fanexGrid(gridOptions);
    this.ShowReport();
};
TopPunter_ByLeague.prototype.HideReport = function () {
    this.Tables.reportContainer.addClass("display-none");
    this.Tables.report.addClass("display-none");
};
TopPunter_ByLeague.prototype.GetReport = function (params) {
    var self = this;

    AccReport.disableInput();
    self.Params.reportParams = params;
    $.ajax({
        type: "POST",
        url: window.rootUrl + "Special/PunterAnalysis/GetTopPunter_ByLeague",
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
TopPunter_ByLeague.prototype.ShowReport = function () {
    this.TopPunterByUser.HideReport();
    this.TopPunter_ByUser_SportType.HideReport();
    this.TopPunterBetCount_ByUser_SportType.HideReport();
    this.TopPunterBetCount_ByUser_BetType.HideReport();
    this.TopPunter_ByUser_BetType.HideReport();

    this.TopPunterBetCountByUser.Tables.reportContainer.addClass("display-none");
    this.TopPunterBetCountByUser.Tables.report.addClass("display-none");
    this.Tables.report.removeClass("display-none");
    this.Tables.reportContainer.removeClass("display-none");
};
TopPunter_ByLeague.prototype.GetTopPunterByUser = function (userId, userName) {
    this.TopPunterByUser.GetReport({
        UserName: userName,
        CustID: userId,
        Fromdate: this.Params.reportParams.Fromdate,
        Todate: this.Params.reportParams.Todate,

        Sporttype: this.Params.reportParams.Sporttype,
        Bettype: this.Params.reportParams.Bettype,
        LeagueID: this.Params.reportParams.LeagueID,
        Reporttype: this.Params.reportParams.Reporttype,

        OrderBy: this.Params.reportParams.OrderBy,
        OrderType: this.Params.reportParams.OrderType,
        OrderByName: this.Params.reportParams.OrderByName,
        OrderTypeName: this.Params.reportParams.OrderTypeName,
        SportName: this.Params.reportParams.SportName,
        LeagueName: this.Params.reportParams.LeagueName
    });
};
TopPunter_ByLeague.prototype.ShowReportTopPunterByUser = function () {
    this.HideReport();
    this.TopPunter_ByUser_SportType.HideReport();
    this.TopPunterBetCountByUser.HideReport();
    this.TopPunterBetCount_ByUser_SportType.HideReport();
    this.TopPunterBetCount_ByUser_BetType.HideReport();
    this.TopPunter_ByUser_BetType.HideReport();
    this.TopPunterByUser.ShowReport();
};
TopPunter_ByLeague.prototype.HideReport = function () {
    this.Tables.reportContainer.addClass("display-none");
    this.Tables.report.addClass("display-none");

    this.TopPunterByUser.HideReport();
    this.TopPunter_ByUser_SportType.HideReport();
    this.TopPunterBetCount_ByUser_SportType.HideReport();
    this.TopPunterBetCount_ByUser_BetType.HideReport();
    this.TopPunter_ByUser_BetType.HideReport();
    this.TopPunterBetCountByUser.HideReport();
};
TopPunter_ByLeague.prototype.GetTopPunterBetCount_ByUser = function (userId, userName) {
    this.TopPunterBetCountByUser.GetReport({
        CustID: userId,
        UserName: userName,
        LeagueID: this.Params.reportParams.LeagueID,
        Sporttype: this.Params.reportParams.Sporttype,
        Bettype: this.Params.reportParams.Bettype,
        Sportbettype: null,
        Fromdate: this.Params.reportParams.Fromdate,
        Todate: this.Params.reportParams.Todate,
        Reporttype: this.Params.reportParams.Reporttype,
        ByLeague: true,
        SportName: this.Params.reportParams.SportName,
        LeagueName: this.Params.reportParams.LeagueName
    });
};
TopPunter_ByLeague.prototype.GetTopPunter_ByUser_SportType = function (sportId, sportName) {
    this.TopPunter_ByUser_SportType.GetReport({
        UserName: this.TopPunterByUser.Params.reportParams.UserName,
        CustID: this.TopPunterByUser.Params.reportParams.CustID,
        Fromdate: this.Params.reportParams.Fromdate,
        Todate: this.Params.reportParams.Todate,
        TopRow: this.Params.reportParams.TopRow,
        OrderBy: this.Params.reportParams.OrderBy,
        OrderType: this.Params.reportParams.OrderType,
        Reporttype: this.Params.reportParams.Reporttype,
        Sporttype: sportId,
        Bettype: this.Params.reportParams.Bettype,
        OrderByName: this.Params.reportParams.OrderByName,
        OrderTypeName: this.Params.reportParams.OrderTypeName,
        SportName: sportName,
        LeagueID: this.Params.reportParams.LeagueID,
        LeagueName: this.Params.reportParams.LeagueName,
        ByLeague: true
    });
};
TopPunter_ByLeague.prototype.GetTopPunterBetCount_ByUser_SportType = function (sportId, sportName) {
    this.TopPunterBetCount_ByUser_SportType.GetReport({
        CustID: this.TopPunterByUser.Params.reportParams.CustID,
        UserName: this.TopPunterByUser.Params.reportParams.UserName,
        LeagueID: this.Params.reportParams.LeagueID,
        Sporttype: sportId,
        SportName: sportName,
        Bettype: this.Params.reportParams.Bettype,
        Sportbettype: null,
        Fromdate: this.Params.reportParams.Fromdate,
        Todate: this.Params.reportParams.Todate,
        Reporttype: this.Params.reportParams.PeriodType,
        ByLeague: true,
        Reporttype: this.Params.reportParams.Reporttype,
        LeagueName: this.Params.reportParams.LeagueName
    });
};
TopPunter_ByLeague.prototype.GetTopPunterBetCount_ByUser_BetType = function (betTypeId,
    betTypeName,
    liveType,
    liveTypeName) {
    this.TopPunterBetCount_ByUser_BetType.GetReport({
        CustID: this.TopPunterByUser.Params.reportParams.CustID,
        UserName: this.TopPunterByUser.Params.reportParams.UserName,
        LeagueID: this.Params.reportParams.LeagueID,
        Sporttype: this.TopPunter_ByUser_SportType.Params.reportParams.Sporttype,
        SportName: this.TopPunter_ByUser_SportType.Params.reportParams.SportName,
        Bettype: betTypeId,
        BetTypeName: betTypeName,
        Fromdate: this.Params.reportParams.Fromdate,
        Todate: this.Params.reportParams.Todate,
        Reporttype: this.Params.reportParams.Reporttype,
        Status: liveType,
        LiveTypeName: liveTypeName,
        LeagueName: this.Params.reportParams.LeagueName,
        ByLeague: true
    });
};
TopPunter_ByLeague.prototype.ShowTopPunter_ByUser_SportType = function () {
    this.TopPunterBetCount_ByUser_BetType.HideReport();
    this.TopPunter_ByUser_BetType.HideReport();
    this.TopPunter_ByUser_SportType.ShowReport();
};
TopPunter_ByLeague.prototype.GetTopPunterWinLoss_ByUser_BetType = function (betTypeId,
    betTypeName,
    liveType,
    liveTypeName) {
    this.TopPunter_ByUser_BetType.GetReport({
        CustID: this.TopPunterByUser.Params.reportParams.CustID,
        UserName: this.TopPunterByUser.Params.reportParams.UserName,
        LeagueID: this.Params.reportParams.LeagueID,
        Sporttype: this.TopPunter_ByUser_SportType.Params.reportParams.Sporttype,
        SportName: this.TopPunter_ByUser_SportType.Params.reportParams.SportName,
        Bettype: betTypeId,
        BetTypeName: betTypeName,
        Fromdate: this.Params.reportParams.Fromdate,
        Todate: this.Params.reportParams.Todate,
        OrderBy: this.Params.reportParams.OrderBy,
        OrderType: this.Params.reportParams.OrderType,
        Reporttype: this.Params.reportParams.Reporttype,
        Status: liveType,
        LiveTypeName: liveTypeName,
        OrderByName: this.Params.reportParams.OrderByName,
        OrderTypeName: this.Params.reportParams.OrderTypeName,
        LeagueName: this.Params.reportParams.LeagueName,
        ByLeague: true
    });
};
TopPunter_ByLeague.prototype.ExportReport = function () {
    AccReport.downloadFile(window.rootUrl + "Special/PunterAnalysis/ExportTopPunter_ByLeague", this.Params.reportParams);
};
