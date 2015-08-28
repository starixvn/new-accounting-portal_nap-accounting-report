$(document).ready(function () {
    var reportManager = new Fanex.AccountingReports.Racing.HarnessAUSTote.ReportManager();
    reportManager.initPage();
});

Namespace.Register("Fanex.AccountingReports.Racing.HarnessAUSTote.ReportManager");
Fanex.AccountingReports.Racing.HarnessAUSTote.ReportManager = function () {
    var self = this;
    var _harnessAUSToteDetail = new Fanex.AccountingReports.Racing.HarnessAUSTote.HarnessAUSToteDetail();
    var _harnessAUSToteBetlist = new Fanex.AccountingReports.Racing.HarnessAUSTote.HarnessAUSToteBetlist();
    var _harnessAUSToteMain = new Fanex.AccountingReports.Racing.HarnessAUSTote.HarnessAUSToteMain();
    self.initPage = function () {
        _harnessAUSToteMain.initPage();
        _harnessAUSToteDetail.initPage();
        _harnessAUSToteBetlist.initPage();
    };

    cachedParams = {
        partnerName: null,
        numberRace: null
    };

    viewGreyhoundAUSToteDetail = function (poolCurrency, partnerId, partnerName) {
        _harnessAUSToteMain.hideReport();
        cachedParams.partnerName = partnerName;
        _harnessAUSToteDetail.getReportRacingDetail({
            winlostDate: _harnessAUSToteMain.Params.reportParams.winlostDate,
            partnerId: partnerId,
            poolCurrency: poolCurrency,
            partnerName: partnerName
        });
    };

    BackRacingDailyWinLoss = function () {
        _harnessAUSToteMain.showReport();
        _harnessAUSToteDetail.hideReport();
    };

    viewRacingToteBetlist = function (leagueId, currency, raceNo) {
        cachedParams.numberRace = raceNo;
        _harnessAUSToteBetlist.getContent({
            leagueId: leagueId,
            raceNo: raceNo,
            currency: currency,
            winlostDate: _harnessAUSToteMain.Params.reportParams.winlostDate
        });
    };

    ResetLayout = function () {
        _harnessAUSToteDetail.hideReport();
        _harnessAUSToteMain.hideReport();
    };
};