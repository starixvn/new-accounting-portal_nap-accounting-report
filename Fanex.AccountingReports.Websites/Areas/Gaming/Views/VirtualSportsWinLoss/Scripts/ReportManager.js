
$(document).ready(function () {
    var reportManager = new Fanex.AccountingReports.Gaming.VirtualSportsWinLossManager();
    reportManager.initPage();
});

Namespace.Register("Fanex.AccountingReports.Gaming.VirtualSportsWinLossManager");
Fanex.AccountingReports.Gaming.VirtualSportsWinLossManager = function () {

    var self = this;

    var virtualSportsWinLoss = new Fanex.AccountingReports.Gaming.VirtualSportsWinLoss();
    var virtualSportsWinLossDetail = new Fanex.AccountingReports.Gaming.VirtualSportsWinLossDetail();

    self.initPage = function () {
        virtualSportsWinLoss.initPage();
        virtualSportsWinLossDetail.initPage();
    };

    ViewReportDetails = function (currencyName, date) {
        var _params = virtualSportsWinLoss.getParams();
        var _detailParams = {
            date: date,
            excludeTest: _params.excludeTest,
            drawFilter: _params.drawFilter,
            sportType: _params.sportType,
            currency: currencyName
        };

        hideMainReport();
        virtualSportsWinLossDetail.getReport(_detailParams);
    };

    BackToMainReport = function () {
        virtualSportsWinLossDetail.removeAndHideDetailReport();
        showMainReport();
    };

    var hideMainReport = function () {
        virtualSportsWinLoss.hideReport();
    };

    var showMainReport = function () {
        virtualSportsWinLoss.showReport();
        virtualSportsWinLoss._table.reportContent.fanexGrid("resizeGrid");
    };
};