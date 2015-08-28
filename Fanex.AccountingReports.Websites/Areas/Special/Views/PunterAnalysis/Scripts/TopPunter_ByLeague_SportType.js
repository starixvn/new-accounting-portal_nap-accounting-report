TopPunter_ByLeague_SportType = function () {
    var self = this;

    self.Tables = {
        reportContainer: null,
        report: null
    };

    self.Params = {
        reportParams: {
            CustID: null,
            UserName: null,
            Sporttype: null,
            Bettype: null,
            LeagueID: null,
            Fromdate: null,
            Todate: null,
            OrderBy: null,
            OrderType: null,
            Reporttype: null,
            SportName: '',
            OrderByName: null,
            OrderTypeName: null,
            SportName: null,
            LeagueName: null,
            ByLeague: true
        }
    };
};
TopPunter_ByLeague_SportType.prototype.InitPage = function () {
    this.CachedDOMElements();
};
TopPunter_ByLeague_SportType.prototype.CachedDOMElements = function () {
    this.Tables.reportContainer = $('#topPunter_ByLeague_SportType_Container');
    this.Tables.report = $('#topPunter_ByLeague_SportType');
};
TopPunter_ByLeague_SportType.prototype.GetReport = function (params) {
    var self = this;

    AccReport.disableInput();
    self.Params.reportParams = params;
    $.ajax({
        type: "POST",
        url: window.rootUrl + "Special/PunterAnalysis/GetTopPunter_ByUser_SportType",
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
TopPunter_ByLeague_SportType.prototype.RenderBetCountLink = function (currentCell) {
    if (currentCell.Value.BetCount == "") {
        return '<div>' + currentCell.Value.BetCount + '</div>';
    }

    var clickEvent = ' onclick = "reportManager.TopPunter_ByLeague.GetTopPunterBetCount_ByUser_BetType(\'' +
                            currentCell.Value.BetTypeId + '\',\'' +
                            currentCell.Value.BetTypeName + '\',' +
                            currentCell.Value.LiveType + ',\'' +
                            currentCell.Value.LiveTypeName + "');\"";

    return '<div class="link" ' + clickEvent + '>' + currentCell.Value.BetCount + '</div>';

};
TopPunter_ByLeague_SportType.prototype.RenderWinLossLink = function (currentCell) {
    if (currentCell.Value.BetCount == "") {
        return '<div>' + reportManager.FormatNumberCell(currentCell.Value.WinLoss, false, true, false) + '</div>';
    }

    var clickEvent = ' onclick = "reportManager.TopPunter_ByLeague.GetTopPunterWinLoss_ByUser_BetType(\'' +
                            currentCell.Value.BetTypeId + '\',\'' +
                            currentCell.Value.BetTypeName + '\',' +
                            currentCell.Value.LiveType + ',\'' +
                            currentCell.Value.LiveTypeName + "');\"";

    return '<div class="link" ' + clickEvent + '>' + reportManager.FormatNumberCell(currentCell.Value.WinLoss, false, true, false) + '</div>';

};
TopPunter_ByLeague_SportType.prototype.RenderReport = function (report) {
    var self = this;

    this.Tables.reportContainer.removeClass("display-none");
    this.Tables.report.addClass("display-none");
    var bodyData = [];
    var footerData = [];

    if (report && report.ChildsInfo.length > 0) {
        var len = report.ChildsInfo.length;
        var currentRow = null;
        var info = null;
        var nonLiveInfo = null;
        var liveInfo = null;
        var mixLiveTypeInfo = null;

        for (var i = 0; i < len; i++) {
            info = report.ChildsInfo[i];
            nonLiveInfo = info.ChildsInfo[0];
            liveInfo = info.ChildsInfo[1];
            mixLiveTypeInfo = info.ChildsInfo[2];
            bodyData.push({
                Columns: [{ Name: "ColBetType", Value: info.Name },
                          {
                              Name: "ColWinLossNonLive",
                              Value: {
                                  BetTypeId: info.Id,
                                  BetTypeName: info.Name,
                                  WinLoss: nonLiveInfo.HasData ? nonLiveInfo.WinLoss_Str : "",
                                  LiveType: nonLiveInfo.LiveType,
                                  LiveTypeName: nonLiveInfo.Name
                              },
                              CustomCellRender: self.RenderWinLossLink
                          },
                          { Name: "ColTurnoverNonLive", Value: nonLiveInfo.HasData ? nonLiveInfo.Turnover_Str : "" },
                          { Name: "ColMarginNonLive", Value: nonLiveInfo.HasData ? reportManager.FormatNumberCell(nonLiveInfo.Margin_Str, false, true, false) : "" },
                          {
                              Name: "ColBetCountMixLiveType",
                              Value: {
                                  BetTypeId: info.Id,
                                  BetTypeName: info.Name,
                                  BetCount: nonLiveInfo.HasData ? nonLiveInfo.BetCount_Str : "",
                                  LiveType: nonLiveInfo.LiveType,
                                  LiveTypeName: nonLiveInfo.Name
                              },
                              CustomCellRender: self.RenderBetCountLink
                          },

                          {
                              Name: "ColWinLossLive",
                              Value: {
                                  BetTypeId: info.Id,
                                  BetTypeName: info.Name,
                                  WinLoss: liveInfo.HasData ? liveInfo.WinLoss_Str : "",
                                  LiveType: liveInfo.LiveType,
                                  LiveTypeName: liveInfo.Name
                              },
                              CustomCellRender: self.RenderWinLossLink
                          },
                          { Name: "ColTurnoverLive", Value: liveInfo.HasData ? liveInfo.Turnover_Str : "" },
                          { Name: "ColMarginLive", Value: liveInfo.HasData ? reportManager.FormatNumberCell(liveInfo.Margin_Str, false, true, false) : "" },
                          {
                              Name: "ColBetCountMixLiveType",
                              Value: {
                                  BetTypeId: info.Id,
                                  BetTypeName: info.Name,
                                  BetCount: liveInfo.HasData ? liveInfo.BetCount_Str : "",
                                  LiveType: liveInfo.LiveType,
                                  LiveTypeName: liveInfo.Name
                              },
                              CustomCellRender: self.RenderBetCountLink
                          },

                          {
                              Name: "ColWinLossMixLiveType",
                              Value: {
                                  BetTypeId: info.Id,
                                  BetTypeName: info.Name,
                                  WinLoss: mixLiveTypeInfo.WinLoss_Str,
                                  LiveType: mixLiveTypeInfo.LiveType,
                                  LiveTypeName: mixLiveTypeInfo.Name
                              },
                              CustomCellRender: self.RenderWinLossLink
                          },
                          { Name: "ColTurnoverMixLiveType", Value: mixLiveTypeInfo.Turnover_Str },
                          { Name: "ColMarginMixLiveType", Value: reportManager.FormatNumberCell(mixLiveTypeInfo.Margin_Str, false, true, false) },
                          {
                              Name: "ColBetCountMixLiveType",
                              Value: {
                                  BetTypeId: info.Id,
                                  BetTypeName: info.Name,
                                  BetCount: mixLiveTypeInfo.BetCount_Str,
                                  LiveType: mixLiveTypeInfo.LiveType,
                                  LiveTypeName: mixLiveTypeInfo.Name
                              },
                              CustomCellRender: self.RenderBetCountLink
                          }]
            });
        }

        info = report.GrandTotal;
        nonLiveInfo = info.ChildsInfo[0];
        liveInfo = info.ChildsInfo[1];
        mixLiveTypeInfo = info.ChildsInfo[2];
        footerData.push({
            Columns: [{ Name: "ColBetType", Value: info.Name, CssClass: "align-right" },
                      { Name: "ColWinLossNonLive", Value: reportManager.FormatNumberCell(nonLiveInfo.WinLoss_Str, false, true, false) },
                      { Name: "ColTurnoverNonLive", Value: nonLiveInfo.Turnover_Str },
                      { Name: "ColMarginNonLive", Value: reportManager.FormatNumberCell(nonLiveInfo.Margin_Str, false, true, false) },
                      { Name: "ColBetCountNonLive", Value: nonLiveInfo.BetCount_Str },

                      { Name: "ColWinLossLive", Value: reportManager.FormatNumberCell(liveInfo.WinLoss_Str, false, true, false) },
                      { Name: "ColTurnoverLive", Value: liveInfo.Turnover_Str },
                      { Name: "ColMarginLive", Value: reportManager.FormatNumberCell(liveInfo.Margin_Str, false, true, false) },
                      { Name: "ColBetCountLive", Value: liveInfo.BetCount_Str },

                      { Name: "ColWinLossMixLiveType", Value: reportManager.FormatNumberCell(mixLiveTypeInfo.WinLoss_Str, false, true, false) },
                      { Name: "ColTurnoverMixLiveType", Value: mixLiveTypeInfo.Turnover_Str },
                      { Name: "ColMarginMixLiveType", Value: reportManager.FormatNumberCell(mixLiveTypeInfo.Margin_Str, false, true, false) },
                      { Name: "ColBetCountMixLiveType", Value: mixLiveTypeInfo.BetCount_Str}]
        });
    }

    var backToMainEvent = "onclick='reportManager.TopPunter_ByLeague.ShowReport()'";
    var backToByUserReportEvent = "onclick='reportManager.TopPunter_ByLeague.ShowReportTopPunterByUser()'";
    var title = '<span class="link" ' + backToMainEvent + '>Main</span> > ' +
                '<span class="link" ' + backToByUserReportEvent + '>' + self.Params.reportParams.UserName + '</span> > ' +
                '<span>' + self.Params.reportParams.SportName + '</span>';

    var gridOptions = {
        customizeHeader: true,
        groupHeaderRows: [{
            Columns: [{ HeaderText: "Bet Type", Name: "ColBetType", MergeRowNumber: 2 },
                      { HeaderText: "Non-Live", Name: "ColWinLossNonLive", MergeColNumber: 4 },
                      { HeaderText: "Live", Name: "ColWinLossLive", MergeColNumber: 4 },
                      { HeaderText: "Non-Live & Live", Name: "ColWinLossMixLiveType", MergeColNumber: 4}]
        }],
        columns: [{ HeaderText: "Bet Type", Width: 120, Name: "ColBetType", HeaderAlign: "center", CellAlign: "left" },
                  { HeaderText: "Win Loss", Width: 120, Name: "ColWinLossNonLive", HeaderAlign: "center", CellAlign: "right" },
                  { HeaderText: "Turnover", Width: 120, Name: "ColTurnoverNonLive", HeaderAlign: "center", CellAlign: "right" },
                  { HeaderText: "Margin", Width: 120, Name: "ColMarginNonLive", HeaderAlign: "center", CellAlign: "right" },
                  { HeaderText: "BetCount", Width: 120, Name: "ColBetCountNonLive", HeaderAlign: "center", CellAlign: "right" },

                  { HeaderText: "Win Loss", Width: 120, Name: "ColWinLossLive", HeaderAlign: "center", CellAlign: "right" },
                  { HeaderText: "Turnover", Width: 120, Name: "ColTurnoverLive", HeaderAlign: "center", CellAlign: "right" },
                  { HeaderText: "Margin", Width: 120, Name: "ColMarginLive", HeaderAlign: "center", CellAlign: "right" },
                  { HeaderText: "BetCount", Width: 120, Name: "ColBetCountLive", HeaderAlign: "center", CellAlign: "right" },

                  { HeaderText: "Win Loss", Width: 120, Name: "ColWinLossMixLiveType", HeaderAlign: "center", CellAlign: "right" },
                  { HeaderText: "Turnover", Width: 120, Name: "ColTurnoverMixLiveType", HeaderAlign: "center", CellAlign: "right" },
                  { HeaderText: "Margin", Width: 120, Name: "ColMarginMixLiveType", HeaderAlign: "center", CellAlign: "right" },
                  { HeaderText: "BetCount", Width: 120, Name: "ColBetCountMixLiveType", HeaderAlign: "center", CellAlign: "right"}],
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
TopPunter_ByLeague_SportType.prototype.ShowReport = function () {
    this.Tables.report.removeClass("display-none");
    this.Tables.reportContainer.removeClass("display-none");
};
TopPunter_ByLeague_SportType.prototype.HideReport = function () {
    this.Tables.reportContainer.addClass("display-none");
    this.Tables.report.addClass("display-none");
};
TopPunter_ByLeague_SportType.prototype.ExportReport = function () {
    AccReport.downloadFile(window.rootUrl + "Special/PunterAnalysis/ExportTopPunter_ByUser_SportType", this.Params.reportParams);
};