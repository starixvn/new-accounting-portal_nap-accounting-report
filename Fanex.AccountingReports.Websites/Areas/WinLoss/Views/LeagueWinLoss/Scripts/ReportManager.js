$(document).ready(function () {
    var reportManager = new Fanex.AccountingReports.WinLoss.LeagueWinLoss.ReportManager();
    reportManager.initPage();
});

Namespace.Register("Fanex.AccountingReports.WinLoss.LeagueWinLoss.ReportManager");
Fanex.AccountingReports.WinLoss.LeagueWinLoss.ReportManager = function () {
    var self = this;
    var _leagueWinLoss = new Fanex.AccountingReports.WinLoss.LeagueWinLoss.LeagueWinLoss();
    var _leagueWinLossViewBetTypes = new Fanex.AccountingReports.WinLoss.LeagueWinLoss.LeagueWinLossViewBetTypes();
    var _leagueWinLossViewMatches = new Fanex.AccountingReports.WinLoss.LeagueWinLoss.LeagueWinLossViewMatches();
    var _leagueWinLossViewSites = new Fanex.AccountingReports.WinLoss.LeagueWinLoss.LeagueWinLossViewSites();
    var _leagueWinLossViewSupers = new Fanex.AccountingReports.WinLoss.LeagueWinLoss.LeagueWinLossViewSupers();
    var _leagueWinLossViewVirtualRate = new Fanex.AccountingReports.WinLoss.LeagueWinLoss.LeagueWinLossViewVirtualRate();

    self.initPage = function () {
        _leagueWinLoss.initPage();
        _leagueWinLossViewBetTypes.initPage();
        _leagueWinLossViewMatches.initPage();
        _leagueWinLossViewSites.initPage();
        _leagueWinLossViewSupers.initPage();
        _leagueWinLossViewVirtualRate.initPage();
    };
    hideLeagueWinLoss = function () {
        _leagueWinLoss.hideReport();
    };
    hideBettypes = function () {
        _leagueWinLossViewBetTypes.hideReport();
    };
    hideMatches = function () {
        _leagueWinLossViewMatches.hideReport();
    };
    hideSites = function () {
        _leagueWinLossViewSites.hideReport();
    };
    hideSupers = function () {
        _leagueWinLossViewSupers.hideReport();
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
            IsNoDraw: _leagueWinLoss.Params.reportParams.IsNoDraw,
            SiteName: _leagueWinLoss.Params.reportParams.SiteName
        });
    };
    viewBetTypes = function (matchId, isLive, matchEvent, isHedge) {
        _leagueWinLossViewMatches.hideReport();
        _leagueWinLossViewBetTypes.getReport({
            StartDate: _leagueWinLossViewMatches.Params.reportParams.StartDate,
            EndDate: _leagueWinLossViewMatches.Params.reportParams.EndDate,
            LeagueName: _leagueWinLossViewMatches.Params.reportParams.LeagueName,
            SportName: _leagueWinLossViewMatches.Params.reportParams.SportName,
            SportBettype: _leagueWinLossViewMatches.Params.reportParams.SportBettype,
            MatchID: matchId,
            MatchEvent: matchEvent,
            LiveIndicator: isLive,
            IsNormalMode: _leagueWinLossViewMatches.Params.reportParams.IsNormalMode,
            IsNoDraw: _leagueWinLossViewMatches.Params.reportParams.IsNoDraw,
            SiteName: _leagueWinLossViewMatches.Params.reportParams.SiteName,
            IsHedge: isHedge
        });
    };
    viewSite = function (bettype, bettypeName, siteType) {
        _leagueWinLossViewBetTypes.hideReport();
        _leagueWinLossViewSites.getReport({
            StartDate: _leagueWinLossViewBetTypes.Params.reportParams.StartDate,
            EndDate: _leagueWinLossViewBetTypes.Params.reportParams.EndDate,
            LeagueName: _leagueWinLossViewBetTypes.Params.reportParams.LeagueName,
            SportName: _leagueWinLossViewBetTypes.Params.reportParams.SportName,
            MatchID: _leagueWinLossViewBetTypes.Params.reportParams.MatchID,
            MatchEvent: _leagueWinLossViewBetTypes.Params.reportParams.MatchEvent,
            LiveIndicator: _leagueWinLossViewBetTypes.Params.reportParams.LiveIndicator,
            SiteType: siteType,
            BetType: bettype,
            BetTypeName: bettypeName,
            IsNormalMode: _leagueWinLossViewBetTypes.Params.reportParams.IsNormalMode,
            IsNoDraw: _leagueWinLossViewBetTypes.Params.reportParams.IsNoDraw,
            SiteName: _leagueWinLossViewBetTypes.Params.reportParams.SiteName
        });
    };

    viewSuper = function (siteId, siteName) {
        _leagueWinLossViewSites.hideReport();
        _leagueWinLossViewSupers.getReport({
            StartDate: _leagueWinLossViewSites.Params.reportParams.StartDate,
            EndDate: _leagueWinLossViewSites.Params.reportParams.EndDate,
            LeagueName: _leagueWinLossViewSites.Params.reportParams.LeagueName,
            SportName: _leagueWinLossViewSites.Params.reportParams.SportName,
            MatchID: _leagueWinLossViewSites.Params.reportParams.MatchID,
            MatchEvent: _leagueWinLossViewSites.Params.reportParams.MatchEvent,
            LiveIndicator: _leagueWinLossViewSites.Params.reportParams.LiveIndicator,
            SiteID: siteId,
            SiteType: _leagueWinLossViewSites.Params.reportParams.SiteType,
            SiteName: siteName,
            BetType: _leagueWinLossViewSites.Params.reportParams.BetType,
            BetTypeName: _leagueWinLossViewSites.Params.reportParams.BetTypeName,
            IsNormalMode: _leagueWinLossViewSites.Params.reportParams.IsNormalMode,
            IsNoDraw: _leagueWinLossViewSites.Params.reportParams.IsNoDraw
        });
    };
    viewVirtualRate = function (superId, superName) {
        _leagueWinLossViewVirtualRate.getReport({
            CustID: superId,
            SuperName: superName
        });
    };

    backToLeagueWinLoss = function () {
        hideMatches();
        _leagueWinLoss.showReport();
    };

    backToMatches = function () {
        hideBettypes();
        _leagueWinLossViewMatches.showReport();
    };

    backToBetTypes = function () {
        hideSites();
        _leagueWinLossViewBetTypes.showReport();
    };

    backToSites = function () {
        hideSupers();
        _leagueWinLossViewSites.showReport();
    };

    isShowReport = function (report) {
        return report &&
               report.ChildsInfo.length > 0 &&
              (report.EightRM.LineTotal != 0 ||
                    report.BARM.LineTotal != 0 ||
                    report.GrandTotal.LineTotal != 0);
    };
}