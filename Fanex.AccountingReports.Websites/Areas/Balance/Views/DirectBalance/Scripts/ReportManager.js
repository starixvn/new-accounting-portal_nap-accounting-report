$(document).ready(function () {
    var directBalanceReportManager = new Fanex.AccountingReports.Balance.ReportManager();
    directBalanceReportManager.initPage();
});

Namespace.Register("Fanex.AccountingReports.Balance.ReportManager");
Fanex.AccountingReports.Balance.ReportManager = function () {
    var self = this;
    var _directBalance = new Fanex.AccountingReports.Balance.DirectBalance();
    var _directBalanceDownline = new Fanex.AccountingReports.Balance.DirectBalanceDownline();
    self.initPage = function () {
        _directBalance.InitPage();
        _directBalanceDownline.InitPage();
    }

    ViewDownline = function (customerType, custId) {
        _directBalance.hideReport();
        if (customerType == "Master") {
            _directBalanceDownline.ViewDirectBalanceMasterDownline(custId);
        }
        else if (customerType == "Agent") {
            _directBalanceDownline.ViewDirectBalanceAgentDownline(custId);
        }
    };

    BackDirectBalance = function (customerType) {
        if (customerType == "Agent") {
            _directBalanceDownline.hideReport();
            _directBalance.BackDirectBalanceDownline(customerType);
            _directBalanceDownline._params.masterId = null;
        }
        else if (customerType == "Member" && _directBalanceDownline._params.masterId == null) {
            _directBalance.BackDirectBalanceDownline(customerType);
        }
        else if (customerType == "Member" && _directBalanceDownline._params.masterId != null) {
            _directBalanceDownline.BackBalanceDownline(customerType);
        }
    };

    ResetMasterId = function () {
        _directBalanceDownline._params.masterId = null;
    };
}