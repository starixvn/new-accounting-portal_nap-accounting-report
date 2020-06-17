$(document).ready(function () {
    var reportManager = new Fanex.AccountingReports.WinLoss.LeagueWinLoss.ReportManager();
    reportManager.initPage();
});

Namespace.Register("Fanex.AccountingReports.WinLoss.LeagueWinLoss.ReportManager");
Fanex.AccountingReports.WinLoss.LeagueWinLoss.ReportManager = function () {
    var self = this;
    var _leagueWinLoss = new Fanex.AccountingReports.WinLoss.LeagueWinLoss.LeagueWinLoss();
    var _leagueWinLossBySport = new Fanex.AccountingReports.WinLoss.LeagueWinLoss.LeagueWinLossBySport();
    var _leagueWinLossByBetType = new Fanex.AccountingReports.WinLoss.LeagueWinLoss.LeagueWinLossByBetType();

    self.initPage = function () {
        _leagueWinLoss.initPage();
        _leagueWinLossBySport.initPage();
        _leagueWinLossByBetType.initPage();
    };
    hideLeagueWinLoss = function () {
        _leagueWinLoss.hideReport();
    };
    hideLeagueWinLossBySport = function () {
        _leagueWinLossBySport.hideReport();
    };
    hideLeagueWinLossByBetType = function () {
        _leagueWinLossByBetType.hideReport();
    };
    viewLeagueWinLossBySport = function (sportId, leagueId, sportName, leagueName) {
        _leagueWinLoss.hideReport();
        _leagueWinLossBySport.getReport({
            StartDate: _leagueWinLoss.Params.reportParams.StartDate,
            EndDate: _leagueWinLoss.Params.reportParams.EndDate,
            LeagueId: leagueId,
            SportBettype: _leagueWinLoss.Params.reportParams.SportBettype,
            LeagueName: leagueName,
            SportName: sportName,
            IsNormalMode: _leagueWinLoss.Params.reportParams.IsNormalMode,
            IsNoDraw: _leagueWinLoss.Params.reportParams.IsNoDraw,
            SportType: sportId
        });
    };
    viewLeagueWinLossByBetType = function (betTypeId, betTypeName, isLive, isHedge) {
        _leagueWinLossBySport.hideReport();
        _leagueWinLossByBetType.getReport({
            StartDate: _leagueWinLossBySport.Params.reportParams.StartDate,
            EndDate: _leagueWinLossBySport.Params.reportParams.EndDate,
            LeagueId: _leagueWinLossBySport.Params.reportParams.LeagueId,
            SportType: _leagueWinLossBySport.Params.reportParams.SportType,
            BetType: betTypeId,
            BetTypeName: betTypeName,
            LiveIndicator: isLive,
            IsHedge: isHedge,
            LeagueName: _leagueWinLossBySport.Params.reportParams.LeagueName,
            SportName: _leagueWinLossBySport.Params.reportParams.SportName,
            IsNormalMode: _leagueWinLossBySport.Params.reportParams.IsNormalMode,
            IsNoDraw: _leagueWinLossBySport.Params.reportParams.IsNoDraw
        });
    };
    backToLeagueWinLoss = function () {
        hideLeagueWinLossBySport();
        hideLeagueWinLossByBetType();
        _leagueWinLoss.showReport();
    };

    backToLeagueWinLossBySport = function () {
        hideLeagueWinLoss();
        hideLeagueWinLossByBetType();
        _leagueWinLossBySport.showReport();
    };

    isShowReport = function (report) {
        return report &&
               report.ChildsInfo.length > 0 &&
              (report.EightRM.LineTotal != 0 ||
                    report.BARM.LineTotal != 0 ||
                    report.GrandTotal.LineTotal != 0);
    };
}