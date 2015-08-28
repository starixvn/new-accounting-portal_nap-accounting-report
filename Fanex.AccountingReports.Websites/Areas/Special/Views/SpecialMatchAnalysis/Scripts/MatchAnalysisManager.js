Namespace.Register("Fanex.AccountingReports.Special.SpecialMatchAnalysis.MatchAnalysisManager");
Fanex.AccountingReports.Special.SpecialMatchAnalysis.MatchAnalysisManager = function () {
    var self = this;

    var _matchAnalysisNormal = new Fanex.AccountingReports.Special.SpecialMatchAnalysis.MatchAnalysisNormal();

    Inheritance_Manager.extend(Fanex.AccountingReports.Special.SpecialMatchAnalysis.MatchAnalysisByMaster,
                               Fanex.AccountingReports.Special.SpecialMatchAnalysis.MatchAnalysisBaseReport);

    var _matchAnalysisByMaster = new Fanex.AccountingReports.Special.SpecialMatchAnalysis.MatchAnalysisByMaster();

    Inheritance_Manager.extend(Fanex.AccountingReports.Special.SpecialMatchAnalysis.MatchAnalysisByAgent,
                               Fanex.AccountingReports.Special.SpecialMatchAnalysis.MatchAnalysisBaseReport);

    var _matchAnalysisByAgent = new Fanex.AccountingReports.Special.SpecialMatchAnalysis.MatchAnalysisByAgent();

    Inheritance_Manager.extend(Fanex.AccountingReports.Special.SpecialMatchAnalysis.MatchAnalysisByMember,
                               Fanex.AccountingReports.Special.SpecialMatchAnalysis.MatchAnalysisBaseReport);
    var _matchAnalysisByMember = new Fanex.AccountingReports.Special.SpecialMatchAnalysis.MatchAnalysisByMember();

    self.initPage = function () {
        _matchAnalysisNormal.initPage();
        _matchAnalysisByMaster.initPage();
        _matchAnalysisByAgent.initPage();
        _matchAnalysisByMember.initPage();
    };

    self.ViewMatchAnalysis = function (eventDate, leagueInfo, matchInfo) {
        _matchAnalysisNormal.ViewMatchAnalysis(eventDate, leagueInfo, matchInfo);
    };
    hideDrillDownView = function () {
        _matchAnalysisByMaster.hideReport();
        _matchAnalysisByAgent.hideReport();
        _matchAnalysisByMember.hideReport();
    };
    self.hideView = function () {
        _matchAnalysisNormal.hideView();
        _matchAnalysisByMaster.hideReport();
        _matchAnalysisByAgent.hideReport();
        _matchAnalysisByMember.hideReport();
    };
    BackToMatchAnalysisBySuper = function () {
        _matchAnalysisByMaster.hideReport();
        _matchAnalysisByAgent.hideReport();
        _matchAnalysisByMember.hideReport();
        _matchAnalysisNormal.showReport();
    };
    ViewMatchAnalysisByMaster = function (custId, custName) {
        _matchAnalysisByMaster.getReport({
            EventDate: _matchAnalysisNormal.Params.reportParams.EventDate,
            MatchId: _matchAnalysisNormal.Params.reportParams.MatchId,
            StatusId: _matchAnalysisNormal.Params.reportParams.StatusId,
            StatusName: _matchAnalysisNormal.Params.reportParams.StatusName,
            Liveindicator: _matchAnalysisNormal.Params.reportParams.Liveindicator,
            LiveindicatorName: _matchAnalysisNormal.Params.reportParams.LiveindicatorName,
            Bettypes: _matchAnalysisNormal.Params.reportParams.Bettypes,
            ReportType: Enums.ReportType.ByMaster,
            ReportTypeName: _matchAnalysisNormal.Params.reportParams.ReportTypeName,
            CustId: custId,
            CustName: custName,
            League: _matchAnalysisNormal.Params.reportParams.League,
            Home: _matchAnalysisNormal.Params.reportParams.Home,
            Away: _matchAnalysisNormal.Params.reportParams.Away
        }, _matchAnalysisNormal.hideReport);
    };
    BackToMatchAnalysisByMaster = function () {
        _matchAnalysisByAgent.hideReport();
        _matchAnalysisByMember.hideReport();
        _matchAnalysisByMaster.showReport();
    };
    ViewMatchAnalysisByAgent = function (custId, custName) {
        _matchAnalysisByAgent.getReport({
            EventDate: _matchAnalysisNormal.Params.reportParams.EventDate,
            MatchId: _matchAnalysisNormal.Params.reportParams.MatchId,
            StatusId: _matchAnalysisNormal.Params.reportParams.StatusId,
            StatusName: _matchAnalysisNormal.Params.reportParams.StatusName,
            Liveindicator: _matchAnalysisNormal.Params.reportParams.Liveindicator,
            LiveindicatorName: _matchAnalysisNormal.Params.reportParams.LiveindicatorName,
            Bettypes: _matchAnalysisNormal.Params.reportParams.Bettypes,
            ReportType: Enums.ReportType.ByAgent,
            ReportTypeName: _matchAnalysisNormal.Params.reportParams.ReportTypeName,
            CustId: custId,
            CustName: custName,
            League: _matchAnalysisNormal.Params.reportParams.League,
            Home: _matchAnalysisNormal.Params.reportParams.Home,
            Away: _matchAnalysisNormal.Params.reportParams.Away
        }, _matchAnalysisByMaster.hideReport);
    };
    BackToMatchAnalysisByAgent = function () {
        _matchAnalysisByMember.hideReport();
        _matchAnalysisByAgent.showReport();
    };
    ViewMatchAnalysisByMember = function (custId, custName) {
        _matchAnalysisByMember.getReport({
            EventDate: _matchAnalysisNormal.Params.reportParams.EventDate,
            MatchId: _matchAnalysisNormal.Params.reportParams.MatchId,
            StatusId: _matchAnalysisNormal.Params.reportParams.StatusId,
            StatusName: _matchAnalysisNormal.Params.reportParams.StatusName,
            Liveindicator: _matchAnalysisNormal.Params.reportParams.Liveindicator,
            LiveindicatorName: _matchAnalysisNormal.Params.reportParams.LiveindicatorName,
            Bettypes: _matchAnalysisNormal.Params.reportParams.Bettypes,
            ReportType: Enums.ReportType.ByMember,
            ReportTypeName: _matchAnalysisNormal.Params.reportParams.ReportTypeName,
            CustId: custId,
            CustName: custName,
            League: _matchAnalysisNormal.Params.reportParams.League,
            Home: _matchAnalysisNormal.Params.reportParams.Home,
            Away: _matchAnalysisNormal.Params.reportParams.Away
        }, _matchAnalysisByAgent.hideReport);
    };
    GenerateReportTitleLink = function (reportType) {
        var reportTitle = '';

        switch (reportType) {
            case Enums.ReportType.ByMaster:
                reportTitle = '<span class="link" onclick="BackToMatchAnalysisBySuper();">Main</span> ' +
                                    ' > <span>' + _matchAnalysisByMaster.Params.reportParams.CustName + '</span>';

                break;
            case Enums.ReportType.ByAgent:
                reportTitle = '<span class="link" onclick="BackToMatchAnalysisBySuper();">Main</span> ' +
                                    ' > <span class="link" onclick="BackToMatchAnalysisByMaster();">' + _matchAnalysisByMaster.Params.reportParams.CustName + '</span>' +
                                    ' > <span>' + _matchAnalysisByAgent.Params.reportParams.CustName + '</span>';
                break;
            case Enums.ReportType.ByMember:
                reportTitle = '<span class="link" onclick="BackToMatchAnalysisBySuper();">Main</span> ' +
                                    ' > <span class="link" onclick="BackToMatchAnalysisByMaster();">' + _matchAnalysisByMaster.Params.reportParams.CustName + '</span>' +
                                    ' > <span class="link" onclick="BackToMatchAnalysisByAgent();">' + _matchAnalysisByAgent.Params.reportParams.CustName + '</span>' +
                                    ' > <span>' + _matchAnalysisByMember.Params.reportParams.CustName + '</span>';
                break;
        }

        return reportTitle;
    };
}