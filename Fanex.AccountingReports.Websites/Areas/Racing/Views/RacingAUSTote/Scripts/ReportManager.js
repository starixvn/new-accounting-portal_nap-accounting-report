/* File Created: December 20, 2013 */
$(document).ready(function () {
    var reportManager = new Fanex.AccountingReports.Racing.RacingAUSTote.ReportManager();
    reportManager.initPage();
});

Namespace.Register("Fanex.AccountingReports.Racing.RacingAUSTote.ReportManager");
Fanex.AccountingReports.Racing.RacingAUSTote.ReportManager = function () {
    var self = this;

    var _racingAUSToteDetail = new Fanex.AccountingReports.Racing.RacingAUSTote.RacingAUSToteDetail();
    var _racingAUSToteBetlist = new Fanex.AccountingReports.Racing.RacingAUSTote.RacingAUSToteBetlist();
    var _racingAUSToteMain = new Fanex.AccountingReports.Racing.RacingAUSTote.RacingAUSToteMain();
    self.initPage = function () {
        _racingAUSToteMain.initPage();
        _racingAUSToteDetail.initPage();
        _racingAUSToteBetlist.initPage();
    };

    viewRacingAUSToteDetail = function (poolCurrency, partnerId, partnerName) {
        _racingAUSToteMain.hideReport();
        _racingAUSToteDetail.getReportRacingDetail({
            winlostDate: _racingAUSToteMain.Params.reportParams.winlostDate,
            partnerId: partnerId,
            poolCurrency: poolCurrency,
            partnerName: partnerName
        });
    };

    BackRacingDailyWinLoss = function () {
        _racingAUSToteMain.showReport();
        _racingAUSToteDetail.hideReport();
    };

    viewRacingToteBetlist = function (leagueId, currency, raceNo) {
        _racingAUSToteBetlist.getContent({
            leagueId: leagueId,
            raceNo: raceNo,
            currency: currency,
            winlostDate: _racingAUSToteMain.Params.reportParams.winlostDate
        });
    };

    ResetLayout = function () {
        _racingAUSToteDetail.hideReport();
        _racingAUSToteMain.hideReport();
    };

};
//var reportManager = new Fanex.AccountingReports.Racing.RacingAUSTote.ReportManager();