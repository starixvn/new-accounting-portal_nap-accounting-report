$(document).ready(function () {
    var creditSitesBetCounts = new Fanex.AccountingReports.Misc.CreditSitesBetCounts.ReportManager();
    creditSitesBetCounts.initPage();
});

Namespace.Register("Fanex.AccountingReports.Misc.CreditSitesBetCounts.ReportManager");
Fanex.AccountingReports.Misc.CreditSitesBetCounts.ReportManager = function () {
    var self = this;

    var _tables = {
        datepickerplugin: null,
        btnGenerate: null,

        report: null,
        reportContent: null,
        reportTitle: null,
        reportExportExcel: null,
        reportColFooter: null
    };

    var _options = {
        report: {
            isShowPager: false,
            tblContentId: 'numberOfBetsReportContent',
            manualCopyHeader: true,
            fullHeight: true,
            fullWidth: true,
            gridHeight: 300,
            freezeHeight: 280,
            column: [220, 220],
            numberFreezeCol: 0
        }
    };

    var _params = {
        reportParams: {
            Datestart: null,
            Dateend: null
        }
    };

    // TempHtml: use for save frozen, scroll html of report 
    // when render by multi function
    self.TempHtml = {
        frozenHtml: '',
        contentHtml: '',
        footerFrozenHtml: '',
        footerContentHtml: ''
    };

    var _cachedData = {
        betCountReport: null,
        lastSorterBy: null
    };

    self.initPage = function () {
        initGridScrollerTables();
        cachedDOMElements();
        renderDatePicker();
        bindingEvent();
    };
    var initGridScrollerTables = function () {
        $("#numberOfBetsReport").initGrid({
            isShowPager: false
        });
    };
    var cachedDOMElements = function () {
        _tables.btnGenerate = $("#btnGenerate");
        _tables.report = $("#numberOfBetsReport");
        _tables.reportHeader = $("#numberOfBetsReportHeader");
        _tables.reportContent = _tables.report.find("#numberOfBetsReportContent tbody");
        _tables.reportTitle = _tables.reportHeader.find("#numberOfBetsReportTitle");
        _tables.reportExportExcel = _tables.reportHeader.find("#numberOfBetsReport_ExportExcel");
        _tables.reportColFooter = _tables.report.find(".col_footer_content thead");
    };
    var bindingEvent = function () {
        _tables.btnGenerate.click(function () {
            self.btnGenerateClick();
        });

        _tables.reportExportExcel.click(function () {
            exportReport();
        });
    };
    var renderDatePicker = function () {
        $('#datepickerplugin').nexDatePicker({
            nexDatePickerType: "daterange",
            datePickerFormat: {
                changeMonth: true,
                changeYear: true,
                dateFormat: "M dd, yy"
            },
            firstDatePicker: {
                Id: "firstDatePickerId1",
                Label: labelFrom,
                InitDate: today,
                LabelCss: "lbl-field col_2",
                TextboxCss: "edit-field col_6"
            },
            secondDatePicker: {
                Id: "secondDatePickerId1",
                Label: labelTo,
                InitDate: today,
                LabelCss: "lbl-field col_1",
                TextboxCss: "edit-field col_6"
            },
            onSelectDatePicker: function () {
            },
            maxDate: new Date(today)
        });

        _tables.datepickerplugin = $("#datepickerplugin");
    };
    self.btnGenerateClick = function () {
        self.getReport();
    };
    self.getReport = function () {
        AccReport.disableInput();
        getParams();
        if (validateParams() == true) {
            $.ajax({
                type: "POST",
                url: window.rootUrl + "Misc/CreditSitesBetCounts/GetReport",
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                data: JSON.stringify(_params.reportParams),
                beforeSend: function () {
                    AccReport.disableInput();
                },
                success: function (rs) {
                    self.renderReport(rs);
                },
                error: function (event, jqXHR, ajaxSettings) {
                    self.renderReport(null);
                },
                complete: function () {
                    AccReport.enableInput();
                }
            });
        }
        else {
            AccReport.enableInput();
        }
    };
    var getParams = function () {
        var dateRange = _tables.datepickerplugin.nexDatePicker("getDateRange");
        _params.reportParams.Datestart = dateRange.FromDate;
        _params.reportParams.Dateend = dateRange.ToDate;
    };
    var validateParams = function () {
        return true;
    };
    self.renderReport = function (report) {
        var reportTitle = "Number of bets for Credit sites from " + _params.reportParams.Datestart +
                                          " to " + _params.reportParams.Dateend;

        _tables.reportTitle.html(reportTitle);
        if (report &&
            report.TransactionList &&
            report.TransactionList.length > 0) {
            _tables.reportExportExcel.removeClass("display-none");
            self.resetTempHtml();
            var frozenHtml = '';
            var siteNumber = report.TransactionList.length;
            for (var i = 0; i < siteNumber; i++) {
                var reportItem = report.TransactionList[i];
                self.renderReportRow(reportItem);
            }

            self.renderReportTotalRow(report.Total);
            self.removeSpaceIE9TempHtml();
            _tables.reportContent.empty().append(self.TempHtml.contentHtml);
            _tables.reportColFooter.empty().append(self.TempHtml.footerContentHtml);
        }
        else {
            self.renderReportNoData();
        }

        _tables.report.removeClass("display-none")
                      .gridScroller(_options.report);

        _tables.reportHeader.removeClass("display-none");
        _tables.report.removeClass("display-none");
    };
    self.resetTempHtml = function () {
        self.TempHtml.frozenHtml = '';
        self.TempHtml.contentHtml = '';
        self.TempHtml.footerFrozenHtml = '';
        self.TempHtml.footerContentHtml = '';
    };
    self.removeSpaceIE9TempHtml = function () {
        self.TempHtml.frozenHtml = fixBugsBrowser.removeSpaceIE9(self.TempHtml.frozenHtml);
        self.TempHtml.contentHtml = fixBugsBrowser.removeSpaceIE9(self.TempHtml.contentHtml);
        self.TempHtml.footerFrozenHtml = fixBugsBrowser.removeSpaceIE9(self.TempHtml.footerFrozenHtml);
        self.TempHtml.footerContentHtml = fixBugsBrowser.removeSpaceIE9(self.TempHtml.footerContentHtml);
    };
    self.renderReportRow = function (row) {
        self.TempHtml.contentHtml += '<tr><td><div class="col220">' + row.SiteName + '</div></td>\
                                          <td><div class="col220 align-right">' + numeral(row.BetCount).format(FormatConstant.INT_NUMBER) + '</div></td>';
    };
    self.renderReportTotalRow = function (row) {
        self.TempHtml.footerContentHtml += '<tr><th><div class="col220">' + row.SiteName + '</div></th>\
                                                <th><div class="col220 align-right">' + numeral(row.BetCount).format(FormatConstant.INT_NUMBER) + '</div></th>';
    };
    self.renderReportNoData = function () {
        _tables.reportExportExcel.addClass("display-none");
        _tables.reportContent.empty().append(
                "<tr><td colspan='2'><div class='no-data-message'>There is no data.</div></td></tr>");
        _tables.reportColFooter.empty();
    };
    var exportReport = function () {
        AccReport.downloadFile(window.rootUrl + "Misc/CreditSitesBetCounts/ExportReport", _params.reportParams);
    };
}