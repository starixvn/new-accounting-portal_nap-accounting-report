/* File Created: January 17, 2014 *//* File Created: December 20, 2013 */
$(document).ready(function () {
    var reportManager = new Fanex.AccountingReports.Racing.GreyhoundAUSTote.ReportManager();
    reportManager.initPage();
});

Namespace.Register("Fanex.AccountingReports.Racing.GreyhoundAUSTote.ReportManager");
Fanex.AccountingReports.Racing.GreyhoundAUSTote.ReportManager = function () {
    var self = this;
    var _greyhoundAUSToteDetail = new Fanex.AccountingReports.Racing.GreyhoundAUSTote.GreyhoundAUSToteDetail();
    var _greyhoundAUSToteBetlist = new Fanex.AccountingReports.Racing.GreyhoundAUSTote.GreyhoundAUSToteBetlist();
    var _greyhoundAUSToteMain = new Fanex.AccountingReports.Racing.GreyhoundAUSTote.GreyhoundAUSToteMain();
    self.initPage = function () {
        _greyhoundAUSToteMain.initPage();
        _greyhoundAUSToteDetail.initPage();
        _greyhoundAUSToteBetlist.initPage();
    };

    viewGreyhoundAUSToteDetail = function (poolCurrency, partnerId, partnerName) {
        _greyhoundAUSToteMain.hideReport();
        _greyhoundAUSToteDetail.getReportRacingDetail({
            winlostDate: _greyhoundAUSToteMain.Params.reportParams.winlostDate,
            partnerId: partnerId,
            poolCurrency: poolCurrency,
            partnerName: partnerName
        });
    };

    BackRacingDailyWinLoss = function () {
        _greyhoundAUSToteMain.showReport();
        _greyhoundAUSToteDetail.hideReport();
    };

    viewRacingToteBetlist = function (leagueId, currency, raceNo) {
        _greyhoundAUSToteBetlist.getContent({
            leagueId: leagueId,
            raceNo: raceNo,
            currency: currency,
            winlostDate: _greyhoundAUSToteMain.Params.reportParams.winlostDate
        });
    };

    ResetLayout = function () {
        _greyhoundAUSToteDetail.hideReport();
        _greyhoundAUSToteMain.hideReport();
    };

};
//var reportManager = new Fanex.AccountingReports.Racing.RacingSATote.ReportManager();