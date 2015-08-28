NumberGameInfo = function () {
    var self = this;
    self.Tables = {
        report: null,
        report_Header: null,
        report_Body: null
    };

    self.CacheData = {
        reportWidth: 765,
        reportHeight: 200
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
NumberGameInfo.prototype.InitPage = function () {
    this.CachedDOMElements();
};

NumberGameInfo.prototype.CachedDOMElements = function () {
    var self = this;
    self.Tables.report = $(".numberGameReport_ScoreInfo");
    self.Tables.report_Header = $("#headerInfo_numberGame_ScoreInfo");
    self.Tables.report_Body = $("#contentInfo_numberGame_ScoreInfo");
};

NumberGameInfo.prototype.RenderReport = function (report, params) {
    var self = this;
    self.Params.reportParams = params;

    var headerTitle = scoreInfoManager.RenderScoreTitle(report);
    self.Tables.report_Header.html(headerTitle);
    self.Tables.report_Body.empty().append(report.MatchInformation);
    self.Tables.report.removeClass("display-none");
};