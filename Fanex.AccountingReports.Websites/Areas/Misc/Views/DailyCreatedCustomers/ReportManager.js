$(document).ready(function () {
    var dailyCreatedCustomers = new Fanex.AccountingReports.Misc.DailyCreatedCustomers.ReportManager();
    dailyCreatedCustomers.initPage();
});

Namespace.Register("Fanex.AccountingReports.Misc.DailyCreatedCustomers.ReportManager");
Fanex.AccountingReports.Misc.DailyCreatedCustomers.ReportManager = function () {
    var self = this;
    Enums = {
        RoleId: {
            Super: 4,
            DirectMaster: 3,
            DirectAgent: 2,
            Member: 1
        }
    };

    var _dailyCreatedCustomers = new Fanex.AccountingReports.Misc.DailyCreatedCustomers.DailyCreatedCustomers();
    var _dailyCreatedCustomersDetails = new Fanex.AccountingReports.Misc.DailyCreatedCustomers.DailyCreatedCustomersDetails();

    self.initPage = function () {
        _dailyCreatedCustomers.initPage();
        _dailyCreatedCustomersDetails.initPage();
    };

    ViewReportDetails = function (roleId, date, isDirect) {        
        _dailyCreatedCustomersDetails.getReport({
            Date: date,
            RoleID: roleId,
            IsDirect: isDirect
        });
    };
    GetRoleName = function (roleId, isDirect) {
        roleId = parseInt(roleId);
        switch (isDirect) {
            case true:
                switch (roleId) {
                    case Enums.RoleId.DirectMaster:
                        return "Direct Master";
                    case Enums.RoleId.DirectAgent:
                        return "Direct Agent";
                }
            case false:
                switch (roleId) {
                    case Enums.RoleId.Super:
                        return "Super";
                    case Enums.RoleId.DirectMaster:
                        return "Master";
                    case Enums.RoleId.DirectAgent:
                        return "Agent";
                    case Enums.RoleId.Member:
                        return "Member";
                }
        }
    };
    HideDetailsReport = function () {
        _dailyCreatedCustomersDetails.hideReport();
    };
}