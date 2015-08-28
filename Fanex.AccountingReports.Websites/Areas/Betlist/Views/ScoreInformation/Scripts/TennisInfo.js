TennisInfo = function () {
    var self = this;
    self.Tables = {
        reportContainer: null,
        report: null,
        report_Header: null,
        report_Body: null
    };

    self.CacheData = {
        reportWidth: 820,
        reportHeight: 'auto'
    };

    self.Params = {
        reportParams: {
            MatchID: null,
            Bettype: null,
            ReportDate: null,
            SportType: null,
            SportTypeName: null,
            BetTypeName: null,
            LiveIndicator: null,
            Hedge: null,

            LeagueId: null,
            LeagueName: null,
            MatchCode: null,
            HomeTeam: null,
            HomeTeamName: null,
            AwayTeam: null,
            AwayTeamName: null,
            BetId: null
        }
    };
};
TennisInfo.prototype.InitPage = function () {
    this.CachedDOMElements();
};

TennisInfo.prototype.CachedDOMElements = function () {
    var self = this;
    self.Tables.reportContainer = $("#tennisReportContainer_ScoreInfo");
    self.Tables.report = self.Tables.reportContainer.find(".tennisReport_ScoreInfo");
    self.Tables.report_Header = $("#headerInfo_Tennis_ScoreInfo");
    self.Tables.report_Body = $("#contentInfo_Tennis_ScoreInfo");
};

TennisInfo.prototype.RenderReport = function (report, params) {
    var self = this;
    self.Params.reportParams = params;
    var headerTitle = scoreInfoManager.RenderScoreTitle(report);
    self.Tables.report_Header.html(headerTitle);
    self.Tables.report_Body.empty().append(report.MatchInformation);
    self.Tables.report.removeClass("display-none");
};