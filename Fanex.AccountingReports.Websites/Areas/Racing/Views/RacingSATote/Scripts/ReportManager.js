/* File Created: December 20, 2013 */
$(document).ready(function () {
    var reportManager = new Fanex.AccountingReports.Racing.RacingSATote.ReportManager();
    reportManager.initPage();
});

Namespace.Register("Fanex.AccountingReports.Racing.RacingSATote.ReportManager");
Fanex.AccountingReports.Racing.RacingSATote.ReportManager = function () {
    var self = this;
    var _racingSAToteBetlist = new Fanex.AccountingReports.Racing.RacingSATote.RacingSAToteBetlist();
    var _racingSAToteDetail = new Fanex.AccountingReports.Racing.RacingSATote.RacingSAToteDetail();
    var _racingSAToteBetlist = new Fanex.AccountingReports.Racing.RacingSATote.RacingSAToteBetlist();
    var _racingSAToteMain = new Fanex.AccountingReports.Racing.RacingSATote.RacingSAToteMain();
    self.initPage = function () {
        _racingSAToteMain.initPage();
        _racingSAToteDetail.initPage();
        _racingSAToteBetlist.initPage();
    };

    viewRacingSaToteDetail = function (poolCurrency, partnerId, partnerName) {
        _racingSAToteMain.hideReport();
        _racingSAToteDetail.getReportRacingDetail({
            winlostDate: _racingSAToteMain.Params.reportParams.winlostDate,
            partnerId: partnerId,
            poolCurrency: poolCurrency,
            partnerName: partnerName
        });
    };

    BackRacingDailyWinLoss = function () {
        _racingSAToteMain.showReport();
        _racingSAToteDetail.hideReport();
    };

    viewRacingToteBetlist = function (leagueId, currency, raceNo) {
        _racingSAToteBetlist.getContent({
            leagueId: leagueId,
            raceNo: raceNo,
            currency: currency,
            winlostDate: _racingSAToteMain.Params.reportParams.winlostDate
        });
    };

    ResetLayout = function () {
        _racingSAToteDetail.hideReport();
        _racingSAToteMain.hideReport();
    };
};