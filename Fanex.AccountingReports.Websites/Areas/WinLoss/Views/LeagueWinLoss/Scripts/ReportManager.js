$(document).ready(function () {
    var reportManager = new Fanex.AccountingReports.WinLoss.LeagueWinLoss.ReportManager();
    reportManager.initPage();
});

Namespace.Register("Fanex.AccountingReports.WinLoss.LeagueWinLoss.ReportManager");
Fanex.AccountingReports.WinLoss.LeagueWinLoss.ReportManager = function () {
    var self = this;
    var _leagueWinLoss = new Fanex.AccountingReports.WinLoss.LeagueWinLoss.LeagueWinLoss();
    var _leagueWinLossViewBetTypes = new Fanex.AccountingReports.WinLoss.LeagueWinLoss.LeagueWinLossViewBetTypes();
    //var _leagueWinLossByBetType = new Fanex.AccountingReports.WinLoss.LeagueWinLoss.LeagueWinLossByBetType();
    var _leagueWinLossViewMatches = new Fanex.AccountingReports.WinLoss.LeagueWinLoss.LeagueWinLossViewMatches();

    self.initPage = function () {
        _leagueWinLoss.initPage();
        _leagueWinLossViewBetTypes.initPage();
        _leagueWinLossViewMatches.initPage();
    };
    hideLeagueWinLoss = function () {
        _leagueWinLoss.hideReport();
    };
    hideLeagueWinLossBySport = function () {
        _leagueWinLossViewBetTypes.hideReport();
    };
    //hideLeagueWinLossByBetType = function () {
    //    _leagueWinLossByBetType.hideReport();
    //};
    hideMatches = function () {
        _leagueWinLossViewMatches.hideReport();
    };
    viewMatches = function (sportName, leagueName) {
        _leagueWinLoss.hideReport();
        _leagueWinLossViewMatches.getReport({
            StartDate: _leagueWinLoss.Params.reportParams.StartDate,
            EndDate: _leagueWinLoss.Params.reportParams.EndDate,
            SportBettype: _leagueWinLoss.Params.reportParams.SportBettype,
            LeagueName: leagueName,
            SportName: sportName,
            IsNormalMode: _leagueWinLoss.Params.reportParams.IsNormalMode,
            IsNoDraw: _leagueWinLoss.Params.reportParams.IsNoDraw
        });
    };
    viewBetTypes = function (matchId, isLive) {
        _leagueWinLossViewMatches.hideReport();
        _leagueWinLossViewBetTypes.getReport({
            StartDate: _leagueWinLossViewMatches.Params.reportParams.StartDate,
            EndDate: _leagueWinLossViewMatches.Params.reportParams.EndDate,
            MatchID: matchId,
            LiveIndicator: isLive,
            SportBettype: _leagueWinLossViewMatches.Params.reportParams.SportBettype,
            IsNormalMode: _leagueWinLossViewMatches.Params.reportParams.IsNormalMode,
            IsNoDraw: _leagueWinLossViewMatches.Params.reportParams.IsNoDraw
        });
    };
    backToLeagueWinLoss = function () {
        hideLeagueWinLossBySport();
        // hideLeagueWinLossByBetType();
        hideMatches();
        _leagueWinLoss.showReport();
    };

    backToLeagueWinLossBySport = function () {
        hideLeagueWinLoss();
        // hideLeagueWinLossByBetType();
        _leagueWinLossViewBetTypes.showReport();
    };

    isShowReport = function (report) {
        return report &&
               report.ChildsInfo.length > 0 &&
              (report.EightRM.LineTotal != 0 ||
                    report.BARM.LineTotal != 0 ||
                    report.GrandTotal.LineTotal != 0);
    };
}