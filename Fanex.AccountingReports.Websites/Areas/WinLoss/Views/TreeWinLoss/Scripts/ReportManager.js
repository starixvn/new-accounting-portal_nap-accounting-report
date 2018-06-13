$(document).ready(function () {
    reportManager.initPage();
});

Namespace.Register("Fanex.AccountingReports.WinLoss.TreeWinLoss.ReportManager");
Fanex.AccountingReports.WinLoss.TreeWinLoss.ReportManager = function () {
    var self = this;
    var _treeWinLoss = new Fanex.AccountingReports.WinLoss.TreeWinLoss.TreeWinLoss();
    var _treeWinLossByComm = new Fanex.AccountingReports.WinLoss.TreeWinLoss.TreeWinLossByComm();
    var _treeWinLossBySuper = new Fanex.AccountingReports.WinLoss.TreeWinLoss.TreeWinLossBySuper();
    var _treeWinLossByMaster = new Fanex.AccountingReports.WinLoss.TreeWinLoss.TreeWinLossByMaster();
    var _treeWinLossByAgent = new Fanex.AccountingReports.WinLoss.TreeWinLoss.TreeWinLossByAgent();
    var _virtualRateHistory = new Fanex.AccountingReports.WinLoss.TreeWinLoss.VirtualRateHistory();

    self.initPage = function () {
        _treeWinLoss.initPage();
        _treeWinLossByComm.initPage();
        _treeWinLossBySuper.initPage();
        _treeWinLossByMaster.initPage();
        _treeWinLossByAgent.initPage();
        _virtualRateHistory.initPage();
    };

    self.CachedData = {
        currentPage: 0,
        itemsPerPage: 100,
        defaultItemsPerPage: 100,
        itemsPerPageArray: [100, 200, 300, 400, 500],
        currentReport: null,
        maxGridHeight: 10000
    };


    viewTreeWinLossByComm = function (currencyId, currencyName) {
        _treeWinLoss.hideReport();
        _treeWinLossByComm.getReport({
            RoleId: 5,
            FromDate: _treeWinLoss.Params.reportParams.FromDate,
            ToDate: _treeWinLoss.Params.reportParams.ToDate,
            Currency: currencyId,
            Name: currencyName,
            ProductSelection: _treeWinLoss.Params.reportParams.ProductSelection,
            Products: _treeWinLoss.Params.reportParams.Products,
            IsNormalMode: _treeWinLoss.Params.reportParams.IsNormalMode
        });
    };

    viewTreeWinLossBySuper = function (custId, Username) {
        _treeWinLossByComm.hideReport();
        _treeWinLossBySuper.getReport({
            RoleId: 4,
            FromDate: _treeWinLoss.Params.reportParams.FromDate,
            ToDate: _treeWinLoss.Params.reportParams.ToDate,
            SRecommend: custId,
            Name: Username,
            ProductSelection: _treeWinLoss.Params.reportParams.ProductSelection,
            Products: _treeWinLoss.Params.reportParams.Products,
            IsNormalMode: _treeWinLoss.Params.reportParams.IsNormalMode
        });
    };

    viewTreeWinLossByMaster = function (custId, Username) {
        _treeWinLossBySuper.hideReport();
        _treeWinLossByMaster.getReport({
            RoleId: 3,
            FromDate: _treeWinLoss.Params.reportParams.FromDate,
            ToDate: _treeWinLoss.Params.reportParams.ToDate,
            MRecommend: custId,
            Name: Username,
            ProductSelection: _treeWinLoss.Params.reportParams.ProductSelection,
            Products: _treeWinLoss.Params.reportParams.Products,
            IsNormalMode: _treeWinLoss.Params.reportParams.IsNormalMode
        });
    };

    viewTreeWinLossByAgent = function (custId, Username) {
        _treeWinLossByMaster.hideReport();
        _treeWinLossByAgent.getReport({
            RoleId: 2,
            FromDate: _treeWinLoss.Params.reportParams.FromDate,
            ToDate: _treeWinLoss.Params.reportParams.ToDate,
            Recommend: custId,
            Name: Username,
            ProductSelection: _treeWinLoss.Params.reportParams.ProductSelection,
            Products: _treeWinLoss.Params.reportParams.Products,
            IsNormalMode: _treeWinLoss.Params.reportParams.IsNormalMode
        });
    };

    viewVirtualRateHistory = function (custId, Username) {
        _virtualRateHistory.getVirtualRateHistory({
            custId: custId,
            Username: Username
        });
    };

    ResetLayout = function () {
        _treeWinLossBySuper.hideReport();
        _treeWinLossByMaster.hideReport();
        _treeWinLossByComm.hideReport();
        _treeWinLossByAgent.hideReport();
        _treeWinLoss.hideReport();
    }

    BackToTreeWinLossByProduct = function () {
        _treeWinLossBySuper.hideReport();
        _treeWinLossByMaster.hideReport();
        _treeWinLossByComm.hideReport();
        _treeWinLossByAgent.hideReport();
        _treeWinLoss.showReport();
    };

    BackToTreeWinLossByComm = function () {
        _treeWinLossBySuper.hideReport();
        _treeWinLossByMaster.hideReport();
        _treeWinLossByComm.showReport();
        _treeWinLossByAgent.hideReport();
        _treeWinLoss.hideReport();
    };

    BackToTreeWinLossBySuper = function () {
        _treeWinLossBySuper.showReport();
        _treeWinLossByMaster.hideReport();
        _treeWinLossByComm.hideReport();
        _treeWinLossByAgent.hideReport();
        _treeWinLoss.hideReport();
    };

    BackToTreeWinLossByMaster = function () {
        _treeWinLossBySuper.hideReport();
        _treeWinLossByMaster.showReport();
        _treeWinLossByComm.hideReport();
        _treeWinLossByAgent.hideReport();
        _treeWinLoss.hideReport();
    };

    BackToTreeWinLossByAgent = function () {
        _treeWinLossBySuper.hideReport();
        _treeWinLossByMaster.hideReport();
        _treeWinLossByComm.hideReport();
        _treeWinLossByAgent.showReport();
        _treeWinLoss.hideReport();
    };
};

var reportManager = new Fanex.AccountingReports.WinLoss.TreeWinLoss.ReportManager();