/* File Created: December 20, 2013 */
$(document).ready(function () {
    var reportManager = new Fanex.AccountingReports.Racing.RacingUKTote.ReportManager();
    reportManager.initPage();
});

Namespace.Register("Fanex.AccountingReports.Racing.RacingUKTote.ReportManager");
Fanex.AccountingReports.Racing.RacingUKTote.ReportManager = function () {
    var self = this;
    var _racingUKToteMain = new Fanex.AccountingReports.Racing.RacingUKTote.RacingUKToteMain();
    var _racingUKToteDetail = new Fanex.AccountingReports.Racing.RacingUKTote.RacingUKToteDetail();
    var _racingUKToteBetlist = new Fanex.AccountingReports.Racing.RacingUKTote.RacingUKToteBetlist();
    self.initPage = function () {
        _racingUKToteMain.initPage();
        _racingUKToteDetail.initPage();
        _racingUKToteBetlist.initPage();
    };

    viewRacingUKToteDetail = function (poolCurrency, partnerId, partnerName) {
        _racingUKToteMain.hideReport();
        _racingUKToteDetail.getReportRacingDetail({
            winlostDate: _racingUKToteMain.Params.reportParams.winlostDate,
            partnerId: partnerId,
            poolCurrency: poolCurrency,
            partnerName: partnerName
        });
    };

    BackRacingDailyWinLoss = function () {
        _racingUKToteMain.showReport();
        _racingUKToteDetail.hideReport();
    };

    viewRacingToteBetlist = function (leagueId, currency, raceNo) {
        _racingUKToteBetlist.getContent({
            leagueId: leagueId,
            raceNo: raceNo,
            currency: currency,
            winlostDate: _racingUKToteMain.Params.reportParams.winlostDate
        });
    };

    ResetLayout = function () {
        _racingUKToteDetail.hideReport();
        _racingUKToteMain.hideReport();
    };
};
//var reportManager = new Fanex.AccountingReports.Racing.RacingUKTote.ReportManager();