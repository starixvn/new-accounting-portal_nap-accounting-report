$(document).ready(function () {
    Inheritance_Manager.extend(Fanex.AccountingReports.Misc.BetCount.NormalReport,
                               Fanex.AccountingReports.Misc.BetCount.BaseReport);
    Inheritance_Manager.extend(Fanex.AccountingReports.Misc.BetCount.HistoryReport,
                               Fanex.AccountingReports.Misc.BetCount.BaseReport);

    var betCountReportManager = new Fanex.AccountingReports.Misc.BetCount.ReportManager();
    betCountReportManager.initPage();
});

Namespace.Register("Fanex.AccountingReports.Misc.BetCount.ReportManager");
Fanex.AccountingReports.Misc.BetCount.ReportManager = function () {
    var self = this;

    var _normalBetCountReport = new Fanex.AccountingReports.Misc.BetCount.NormalReport();
    var _historyBetCountReport = new Fanex.AccountingReports.Misc.BetCount.HistoryReport();

    var _tables = {
        btnHistoryReport: null,
        betCountReportFilter: null,
        historyBetCountReportFilter: null,
        btnBackHistoryReport: null
    };
    self.initPage = function () {
        cachedDOMElements();
        getDropdownProduct();
        bindingEvent();

        _normalBetCountReport.initPage();
        _historyBetCountReport.initPage();
    };
    cachedDOMElements = function () {
        _tables.btnHistoryReport = $("#btnViewHistoryReport");
        _tables.btnBackHistoryReport = $("#btnBack_HistoryReport");
    };
    getDropdownProduct = function () {
        $.ajax({
            type: "POST",
            url: window.rootUrl + "Misc/BetCount/GetAllProducts",
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            data: null,
            beforeSend: function () {
                AccReport.disableInput();
            },
            success: function (rs) {
                _normalBetCountReport.renderDropdownProduct(rs);
                _historyBetCountReport.renderDropdownProduct(rs);
            },
            error: function (event, jqXHR, ajaxSettings) {
                _normalBetCountReport.renderDropdownProduct([]);
                _historyBetCountReport.renderDropdownProduct([]);
            },
            complete: function () {
                AccReport.enableInput();
            }
        });
    };
    bindingEvent = function () {
        _tables.btnHistoryReport.click(function () {
            btnHistoryReportClick();
        });

        _tables.btnBackHistoryReport.click(function () {
            btnBackHistoryReportClick();
        });
    };
    btnBackHistoryReportClick = function () {
        _historyBetCountReport.hidePage();
        _normalBetCountReport.showPage();
    };
    btnHistoryReportClick = function () {
        _normalBetCountReport.hidePage();
        _historyBetCountReport.showPage();
    };
}