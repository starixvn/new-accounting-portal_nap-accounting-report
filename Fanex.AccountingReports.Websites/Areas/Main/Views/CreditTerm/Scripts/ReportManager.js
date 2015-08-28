
$(document).ready(function () {
    var reportManager = new Fanex.AccountingReports.Main.CreditTerm.ReportManager();
    reportManager.initPage();
});

$(window).resize(function () {
    setWidthTabReport();
});

setWidthTabReport = function () {
    var filterBarWidth = $('#filter-bar').width();
    var divReportWidth = "width: " + filterBarWidth + "px";
    $('#dvReport').attr("style", divReportWidth);
};

Namespace.Register("Fanex.AccountingReports.Main.CreditTerm.ReportManager");

Fanex.AccountingReports.Main.CreditTerm.ReportManager = function () {
    var self = this;

    var preAgreedReport;
    var installmentAmountReport;

    var _table = {
        preAgreedLink: null,
        installmentAmountLink: null,
        preAgreedHeader: null,
        installAmountHeader: null,
        preAgreedReport: null,
        installmentAmountReport: null
    };

    self.initPage = function () {
        setWidthTabReport();
        initInstance();
        catchedDOMElement();
        bindingEvent();
    };

    var initInstance = function () {
        preAgreedReport = new Fanex.AccountingReports.Main.CreditTerm.CreditTerm();
        preAgreedReport.initPage();

        installmentAmountReport = new Fanex.AccountingReports.Main.CreditTerm.InstallAmount();
        installmentAmountReport.initPage();
    };

    var catchedDOMElement = function () {
        _table.preAgreedLink = $('#PreAgreedLink');
        _table.installmentAmountLink = $('#InstallmentAmountLink');
        _table.preAgreedHeader = $('#PreAgreedAmountHeader');
        _table.installAmountHeader = $('#InstallAmountHeader');
        _table.preAgreedReport = $('#PreAgreedReport');
        _table.installmentAmountReport = $('#InstallmentAmountContainer');
    };

    var bindingEvent = function () {
        _table.preAgreedLink.click(function () {
            _table.preAgreedLink.addClass("selected-tab");
            _table.installmentAmountLink.removeClass("selected-tab");
            showPreAgreed();
            hideInstallmentAmount();
        });

        _table.installmentAmountLink.click(function () {
            _table.preAgreedLink.removeClass("selected-tab");
            _table.installmentAmountLink.addClass("selected-tab");
            showInstallmentAmount();
            hidePreAgreed();
        });
    };

    var hidePreAgreed = function () {
        _table.preAgreedHeader.addClass("display-none");
        _table.preAgreedReport.addClass("display-none");
    };

    var showPreAgreed = function () {
        _table.preAgreedHeader.removeClass("display-none");
        _table.preAgreedReport.removeClass("display-none");
    };

    var hideInstallmentAmount = function () {
        _table.installAmountHeader.addClass("display-none");
        _table.installmentAmountReport.addClass("display-none");
    };

    var showInstallmentAmount = function () {
        _table.installAmountHeader.removeClass("display-none");

        // if it has exist table, show this table.
        if (_table.installmentAmountReport.children().length > 1) {
            _table.installmentAmountReport.removeClass("display-none");
        }
    };
};