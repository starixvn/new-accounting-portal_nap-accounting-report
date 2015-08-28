$(document).ready(function () {
    var superBalanceReportManager = new Fanex.AccountingReports.Balance.SuperBalanceReportManager();
    superBalanceReportManager.initPage();
});

Namespace.Register("Fanex.AccountingReports.Balance.SuperBalanceReportManager");
Fanex.AccountingReports.Balance.SuperBalanceReportManager = function () {
    var self = this;
    var _superBalance = new Fanex.AccountingReports.Balance.SuperBalance();

    self.initPage = function () {
        _superBalance.InitPage();
    };
};