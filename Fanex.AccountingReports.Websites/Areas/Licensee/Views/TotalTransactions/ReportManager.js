$(document).ready(function () {
    var totalTransactions = new Fanex.AccountingReports.Licensee.TotalTransactions.ReportManager();
    totalTransactions.initPage();
});

Namespace.Register("Fanex.AccountingReports.Licensee.TotalTransactions.ReportManager");
Fanex.AccountingReports.Licensee.TotalTransactions.ReportManager = function () {
    var self = this;

    var _totalTransactions = new Fanex.AccountingReports.Licensee.TotalTransactions.TotalTransactions();

    self.initPage = function () {
        _totalTransactions.initPage();
    };

    hideResellerGroup = function (btnExpand, reseller) {
        _totalTransactions.hideResellerGroup(btnExpand, reseller);
    };
}