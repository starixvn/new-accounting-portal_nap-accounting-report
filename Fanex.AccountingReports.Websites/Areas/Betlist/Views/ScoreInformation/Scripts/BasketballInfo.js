BasketballInfo = function () {
    var self = this;
    self.Tables = {
        report: null,
        report_Header: null,
        report_Body: null
    };

    self.CacheData = {
        reportWidth: 780,
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

BasketballInfo.prototype.InitPage = function () {
    this.CachedDOMElements();
};

BasketballInfo.prototype.CachedDOMElements = function () {
    var self = this;
    self.Tables.report = $(".basketballReport_ScoreInfo");
    self.Tables.report_Header = $("#headerInfo_Basketball_ScoreInfo");
    self.Tables.report_Body = $("#contentInfo_Basketball_ScoreInfo");
};

BasketballInfo.prototype.RenderReport = function (report, params) {
    var self = this;
    self.Params.reportParams = params;

    var headerTitle = scoreInfoManager.RenderScoreTitle(report);
    self.Tables.report_Header.html(headerTitle);
    self.Tables.report_Body.html(report.MatchInformation);
    self.Tables.report.removeClass("display-none");
};

BasketballInfo.prototype.RenderScore = function (score) {
    if (score == null) {
        return '';
    }

    return score;
};