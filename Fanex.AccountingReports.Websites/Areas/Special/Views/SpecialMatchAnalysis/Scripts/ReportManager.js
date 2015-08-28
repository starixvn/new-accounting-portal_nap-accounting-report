$(document).ready(function () {
    var specialMatchAnalysis = new Fanex.AccountingReports.Special.SpecialMatchAnalysis.ReportManager();
    specialMatchAnalysis.initPage();
});


Namespace.Register("Fanex.AccountingReports.Special.SpecialMatchAnalysis.ReportManager");
Fanex.AccountingReports.Special.SpecialMatchAnalysis.ReportManager = function () {
    var self = this;

    Enums = {
        Sport: {
            NumberGame: 161,
            LiveCasino: 162
        },

        ReportType: {
            Currency: 0,
            Bettype: 1,
            Super: 2,
            Member: 3,

            ByMaster: 4,
            ByAgent: 5,
            ByMember: 6
        },

        BetStatus: {
            All: 0,
            Won: 112,
            Lose: 113,
            Draw: 111
        }
    };

    var _matchSelection = new Fanex.AccountingReports.Special.SpecialMatchAnalysis.MatchSelection();
    var _matchAnalysis = new Fanex.AccountingReports.Special.SpecialMatchAnalysis.MatchAnalysisManager();

    var _tables = {
        matchSelectionFilter: null,
        matchAnalysisFilter: null
    };

    self.initPage = function () {
        _matchSelection.initPage();
        _matchAnalysis.initPage();

        cachedDOMElements();
    };
    cachedDOMElements = function () {
        _tables.matchSelectionFilter = $("#matchSelectionFilter");
        _tables.matchAnalysisFilter = $("#matchAnalysisFilter");
    };
    ViewMatchAnalysis = function (eventDate, leagueIndex, matchIndex) {
        _tables.matchSelectionFilter.addClass("display-none");
        _matchSelection.hideReport();
        _tables.matchAnalysisFilter.removeClass("display-none");

        var leagueInfo = _matchSelection.CachedData.MatchList[leagueIndex];
        var matchInfo = leagueInfo.Matches[matchIndex];
        _matchAnalysis.ViewMatchAnalysis(eventDate, leagueInfo, matchInfo);
    };
    ViewMatchSelection = function () {
        _matchAnalysis.hideView();
        _tables.matchAnalysisFilter.addClass("display-none");
        _tables.matchSelectionFilter.removeClass("display-none");
        _matchSelection.showReport();
    }
}