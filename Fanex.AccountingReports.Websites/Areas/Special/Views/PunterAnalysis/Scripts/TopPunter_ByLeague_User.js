TopPunter_ByLeague_User = function () {
    var self = this;

    self.Tables = {
        reportContainer: null,
        report: null
    };

    self.Params = {
        reportParams: {
            CustID: null,
            UserName: null,
            Fromdate: null,
            Todate: null,
            Sporttype: null,
            Bettype: null,
            LeagueID: null,
            Reporttype: null,

            SportName: null,
            LeagueName: null,
            OrderBy: null,
            OrderByName: null,
            OrderType: null,
            OrderTypeName: null
        }
    };
};
TopPunter_ByLeague_User.prototype.InitPage = function () {
    this.CachedDOMElements();
};
TopPunter_ByLeague_User.prototype.CachedDOMElements = function () {
    this.Tables.reportContainer = $('#topPunter_ByLeague_UserContainer');
    this.Tables.report = $('#topPunter_ByLeague_User');
};
TopPunter_ByLeague_User.prototype.GetReport = function (params) {
    var self = this;

    AccReport.disableInput();
    self.Params.reportParams = params;
    $.ajax({
        type: "POST",
        url: window.rootUrl + "Special/PunterAnalysis/GetTopPunter_ByLeague_User",
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
TopPunter_ByLeague_User.prototype.RenderReport = function (report) {
    var self = this;

    this.Tables.reportContainer.removeClass("display-none");
    this.Tables.report.addClass("display-none");
    var bodyData = [];
    var footerData = [];

    if (report && report.ChildsInfo.length > 0) {
        self.Params.reportParams.CustID = report.Id;
        self.Params.reportParams.UserName = report.Name;

        var len = report.ChildsInfo.length;
        var currentRow = null;
        var info = null;
        for (var i = 0; i < len; i++) {
            info = report.ChildsInfo[i];
            bodyData.push({
                Columns: [
                         {
                             Name: "ColSportType",
                             Value: {
                                 isShowLink: info.Id != 0,
                                 Id: info.Id,
                                 Name: info.Name
                             },
                             CustomCellRender: function (currentCell) {
                                 if (currentCell.Value.isShowLink) {
                                     var clickEvent = ' onclick = "reportManager.TopPunter_ByLeague.GetTopPunter_ByUser_SportType(\'' + currentCell.Value.Id + '\',\'' +
                                                                                                currentCell.Value.Name + "');\"";
                                     return '<div class="link" ' + clickEvent + '>' + currentCell.Value.Name + '</div>';
                                 }

                                 return '<div>' + currentCell.Value.Name + '</div>';
                             }
                         },
                          { Name: "ColWinLoss", Value: reportManager.FormatNumberCell(info.WinLoss_Str, false, true, false) },
                          { Name: "ColTurnover", Value: info.Turnover_Str },
                          { Name: "ColMargin", Value: reportManager.FormatNumberCell(info.Margin_Str, false, true, false) },
                          {
                              Name: "ColBetCount",
                              Value: {
                                  Id: info.Id,
                                  Name: info.Name,
                                  isShowLink: info.Id != 0,
                                  BetCount: info.BetCount
                              },
                              CustomCellRender: function (currentCell) {
                                  if (currentCell.Value.isShowLink) {
                                      var clickEvent = ' onclick = "reportManager.TopPunter_ByLeague.GetTopPunterBetCount_ByUser_SportType(\'' + currentCell.Value.Id + '\',\'' +
                                                                                         currentCell.Value.Name + "');\"";

                                      return '<div class="link" ' + clickEvent + '>' + currentCell.Value.BetCount + '</div>';
                                  }

                                  return '<div>' + currentCell.Value.BetCount + '</div>';
                              }
                          }]
            });
        }

        info = report.GrandTotal;
        footerData.push({
            Columns: [{ Name: "ColSportType", Value: info.Name, CssClass: "align-right" },
                      { Name: "ColWinLoss", Value: reportManager.FormatNumberCell(info.WinLoss_Str, false, true, false) },
                      { Name: "ColTurnover", Value: info.Turnover_Str },
                      { Name: "ColMargin", Value: reportManager.FormatNumberCell(info.Margin_Str, false, true, false) },
                      { Name: "ColBetCount", Value: info.BetCount_Str}]
        });
    }

    var title = '';
    if (self.Params.reportParams.IsDirectFromFilter) {
        title = 'Main - Punter Analysis';
    } else {
        var clickEvent = "onclick='reportManager.TopPunter_ByLeague.ShowReport()'";
        title = '<span class="link" ' + clickEvent + '>Main</span> > <span>' + self.Params.reportParams.UserName + '</span>';
    }

    var gridOptions = {
        customizeHeader: false,
        columns: [{ HeaderText: "SportType", Width: 140, Name: "ColSportType", HeaderAlign: "center", CellAlign: "left" },
                  { HeaderText: "Win Loss", Width: 120, Name: "ColWinLoss", HeaderAlign: "center", CellAlign: "right" },
                  { HeaderText: "Turnover", Width: 120, Name: "ColTurnover", HeaderAlign: "center", CellAlign: "right" },
                  { HeaderText: "Margin", Width: 80, Name: "ColMargin", HeaderAlign: "center", CellAlign: "right" },
                  { HeaderText: "Bet Count", Width: 100, Name: "ColBetCount", HeaderAlign: "center", CellAlign: "right"}],
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

    reportManager.TopPunter_ByLeague.ShowReportTopPunterByUser();
    this.Tables.report.removeClass("display-none").fanexGrid(gridOptions);
};
TopPunter_ByLeague_User.prototype.HideReport = function () {
    this.Tables.reportContainer.addClass("display-none");
    this.Tables.report.addClass("display-none");
};
TopPunter_ByLeague_User.prototype.ShowReport = function () {
    this.Tables.report.removeClass("display-none");
    this.Tables.reportContainer.removeClass("display-none");
};
TopPunter_ByLeague_User.prototype.ExportReport = function () {
    AccReport.downloadFile(window.rootUrl + "Special/PunterAnalysis/ExportTopPunter_ByLeague_User", this.Params.reportParams);
};


