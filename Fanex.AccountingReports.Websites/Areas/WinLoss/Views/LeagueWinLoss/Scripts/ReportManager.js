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
    var _leagueWinLossViewBetTypesByStatus = new Fanex.AccountingReports.WinLoss.LeagueWinLoss.LeagueWinLossViewBetTypesByStatus();
    var _leagueWinLossViewMatchesFromBetTypeStatus = new Fanex.AccountingReports.WinLoss.LeagueWinLoss.LeagueWinLossViewMatchesFromBetTypeStatus();

    self.initPage = function () {
        _leagueWinLoss.initPage();
        _leagueWinLossViewBetTypes.initPage();
        _leagueWinLossViewMatches.initPage();
        _leagueWinLossViewSites.initPage();
        _leagueWinLossViewSupers.initPage();
        _leagueWinLossViewVirtualRate.initPage();
        _leagueWinLossViewBetTypesByStatus.initPage();
        _leagueWinLossViewMatchesFromBetTypeStatus.initPage();
        scoreInfoManager.InitPage();
    };
    hideLeagueWinLoss = function () {
        _leagueWinLoss.hideReport();
    };
    hideBettypesByStatus = function () {
        _leagueWinLossViewBetTypesByStatus.hideReport();
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
    hideMatchesFromBetTypeStatus = function () {
        _leagueWinLossViewMatchesFromBetTypeStatus.hideReport();
    };
    viewMatches = function (sportName, leagueName, sportType, leagueId) {
        _leagueWinLoss.hideReport();
        _leagueWinLossViewMatches.getReport({
            StartDate: _leagueWinLoss.Params.reportParams.StartDate,
            EndDate: _leagueWinLoss.Params.reportParams.EndDate,
            SportBettype: _leagueWinLoss.Params.reportParams.SportBettype,
            LeagueName: leagueName,
            SportName: sportName,
            IsNormalMode: _leagueWinLoss.Params.reportParams.IsNormalMode,
            IsNoDraw: _leagueWinLoss.Params.reportParams.IsNoDraw,
            SiteName: _leagueWinLoss.Params.reportParams.SiteName,
            ShowHedgeMatchOnly: false,
            BetType: -1,
            GlobalShowTime: _leagueWinLoss.Params.reportParams.GlobalShowTime,
            LiveIndicator: _leagueWinLoss.Params.reportParams.LiveIndicator,
            SportType: sportType,
            LeagueId: leagueId
        });
    };
    viewBetTypesByStatus = function (sportName, leagueName, sportType, leagueId) {
        _leagueWinLoss.hideReport();
        _leagueWinLossViewBetTypesByStatus.getReport({
            StartDate: _leagueWinLoss.Params.reportParams.StartDate,
            EndDate: _leagueWinLoss.Params.reportParams.EndDate,
            LeagueName: leagueName,
            SportName: sportName,
            SportBettype: _leagueWinLoss.Params.reportParams.SportBettype,
            MatchID: -1,
            MatchEvent: '',
            LiveIndicator: _leagueWinLoss.Params.reportParams.LiveIndicator,
            IsNormalMode: _leagueWinLoss.Params.reportParams.IsNormalMode,
            IsNoDraw: _leagueWinLoss.Params.reportParams.IsNoDraw,
            SiteName: _leagueWinLoss.Params.reportParams.SiteName,
            IsHedge: false,
            IsIncludeSiteType: false,
            IsIncludeHedge: true,
            GlobalShowTime: _leagueWinLoss.Params.reportParams.GlobalShowTime,
            SportType: sportType,
            LeagueId: leagueId
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
            IsHedge: isHedge,
            IsIncludeSiteType: true,
            IsIncludeHedge: false,
            GlobalShowTime: _leagueWinLossViewMatches.Params.reportParams.GlobalShowTime,
            SportType: _leagueWinLossViewMatches.Params.reportParams.SportType,
            LeagueId: _leagueWinLossViewMatches.Params.reportParams.LeagueId
        });
    };
    viewSite = function (bettype, bettypeName, siteType) {
        _leagueWinLossViewBetTypes.hideReport();
        _leagueWinLossViewBetTypesByStatus.hideReport();
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
            SiteName: _leagueWinLossViewBetTypes.Params.reportParams.SiteName,
            SportType: _leagueWinLossViewBetTypes.Params.reportParams.SportType
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
            IsNoDraw: _leagueWinLossViewSites.Params.reportParams.IsNoDraw,
            SportType: _leagueWinLossViewSites.Params.reportParams.SportType
        });
    };
    viewVirtualRate = function (superId, superName) {
        _leagueWinLossViewVirtualRate.getReport({
            CustID: superId,
            SuperName: superName
        });
    };
    viewMatchesFromBetTypeStatus = function (betType, betTypeName, liveStatus, isHedge) {
        _leagueWinLossViewBetTypesByStatus.hideReport();
        _leagueWinLossViewMatchesFromBetTypeStatus.getReport({
            StartDate: _leagueWinLossViewBetTypesByStatus.Params.reportParams.StartDate,
            EndDate: _leagueWinLossViewBetTypesByStatus.Params.reportParams.EndDate,
            SportBettype: _leagueWinLossViewBetTypesByStatus.Params.reportParams.SportBettype,
            LeagueName: _leagueWinLossViewBetTypesByStatus.Params.reportParams.LeagueName,
            SportName: _leagueWinLossViewBetTypesByStatus.Params.reportParams.SportName,
            IsNormalMode: _leagueWinLossViewBetTypesByStatus.Params.reportParams.IsNormalMode,
            IsNoDraw: _leagueWinLossViewBetTypesByStatus.Params.reportParams.IsNoDraw,
            SiteName: _leagueWinLossViewBetTypesByStatus.Params.reportParams.SiteName,
            LiveIndicator: liveStatus,
            ShowHedgeMatchOnly: isHedge,
            BetType: betType,
            BetTypeName: betTypeName,
            GlobalShowTime: _leagueWinLossViewBetTypesByStatus.Params.reportParams.GlobalShowTime,
            SportType: _leagueWinLossViewBetTypesByStatus.Params.reportParams.SportType,
            LeagueId: _leagueWinLossViewBetTypesByStatus.Params.reportParams.LeagueId
        });
    };

    backToLeagueWinLoss = function () {
        hideMatches();
        hideBettypesByStatus();
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

    backToBetTypeByLiveStatus = function () {
        hideMatchesFromBetTypeStatus();
        _leagueWinLossViewBetTypesByStatus.showReport();
    };

    isShowReport = function (report) {
        return report &&
            report.ChildsInfo.length > 0 &&
            (report.EightRM.LineTotal != 0 ||
                report.BARM.LineTotal != 0 ||
                report.GrandTotal.LineTotal != 0);
    };
}