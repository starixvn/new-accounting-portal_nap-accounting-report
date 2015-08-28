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
    hideLeagueWinLossBySport = function () {
        _leagueWinLossBySport.hideReport();
    };
    hideLeagueWinLossByBetType = function () {
        _leagueWinLossByBetType.hideReport();
    };
    viewLeagueWinLossBySport = function (sportId, leagueId, sportName, leagueName) {
        _leagueWinLoss.hideReport();
        _leagueWinLossBySport.getReport({
            Winlostdate: _leagueWinLoss.Params.reportParams.Winlostdate,
            Leagueid: leagueId,
            Sporttype: sportId,
            LeagueName: leagueName,
            SportName: sportName
        });
    };
    viewLeagueWinLossByBetType = function (betTypeId, betTypeName, isLive, isHedge) {
        _leagueWinLossByBetType.getReport({
            Winlostdate: _leagueWinLossBySport.Params.reportParams.Winlostdate,
            Leagueid: _leagueWinLossBySport.Params.reportParams.Leagueid,
            Sporttype: _leagueWinLossBySport.Params.reportParams.Sporttype,
            Bettype: betTypeId,
            BetTypeName: betTypeName,
            Liveindicator: isLive,
            IsHedge: isHedge,
            LeagueName: _leagueWinLossBySport.Params.reportParams.LeagueName,
            SportName: _leagueWinLossBySport.Params.reportParams.SportName
        });
    };
    backToLeagueWinLoss = function () {
        hideLeagueWinLossBySport();
        hideLeagueWinLossByBetType();
        _leagueWinLoss.showReport();
    };
    isShowReport = function (report) {
        return report &&
               report.ChildsInfo.length > 0 &&
              (report.EightRM.LineTotal != 0 ||
                    report.BARM.LineTotal != 0 ||
                    report.GrandTotal.LineTotal != 0);
    };
}