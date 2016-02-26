Namespace.Register("Fanex.AccountingReports.Special.SpecialMatchAnalysis.MatchAnalysisBaseReport");
Fanex.AccountingReports.Special.SpecialMatchAnalysis.MatchAnalysisBaseReport = function () {
    var self = this;

    self.Params = {
        reportParams: {
            EventDate: null,
            MatchId: null,
            StatusId: null,
            StatusName: '',
            Liveindicator: null,
            LiveindicatorName: '',
            ReportType: null,
            ReportTypeName: '',
            CustId: null,
            CustName: '',
            League: '1',
            Home: '2',
            Away: '3'
        }
    };

    self.Tables = {
        report: null,
        reportHeader: null,
        reportContent: null,
        reportTitle: null,
        reportExportExcel: null,
        reportRowHeader: null,
        reportRowFooter: null,
        reportColFooter: null
    };

    // TempHtml: use for save frozen, scroll html of report 
    // when render by multi function
    self.TempHtml = {
        frozenHtml: '',
        contentHtml: '',
        footerFrozenHtml: '',
        footerContentHtml: ''
    };

    self.initPage = function () {
        self = this;
        self.initGridScrollerTables();
        self.cachedDOMElements();
        self.bindingEvent();
    };

    self.getReport = function (params, callBackFunction) {
        AccReport.disableInput();
        self.Params.reportParams = params;

        $.ajax({
            type: "POST",
            url: window.rootUrl + "Special/SpecialMatchAnalysis/GetMatchAnalysis",
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            data: JSON.stringify(self.Params.reportParams),
            beforeSend: function () {
                AccReport.disableInput();
            },
            success: function (rs) {
                callBackFunction();
                self.renderReport(rs);
            },
            error: function (event, jqXHR, ajaxSettings) {
                self.renderReport(null);
            },
            complete: function () {
                AccReport.enableInput();
            }
        });
    };
    self.renderReport = function (report) {
        var reportTitle = GenerateReportTitleLink(self.Params.reportParams.ReportType);
        self.Tables.reportTitle.html(reportTitle);

        if (report &&
            report.Rows &&
            report.Rows.length > 0) {
            var len = report.Rows.length;
            self.resetTempHtml();
            self.Tables.reportExportExcel.removeClass("display-none");

            for (var i = 0; i < len; i++) {
                var reportItem = report.Rows[i];
                self.renderReportRow(reportItem, i % 2 ? 'even' : '');
            }

            self.renderReportTotalRow(report.Total);

            self.removeSpaceIE9TempHtml();
            self.Tables.reportRowHeader.empty().append(self.TempHtml.frozenHtml);
            self.Tables.reportContent.empty().append(self.TempHtml.contentHtml);
            self.Tables.reportRowFooter.empty().append(self.TempHtml.footerFrozenHtml);
            self.Tables.reportColFooter.empty().append(self.TempHtml.footerContentHtml);
        }
        else {
            self.renderReportNoData();
        }

        self.Tables.report.removeClass("display-none")
                      .gridScroller(self.Options.report);
        self.Tables.reportHeader.removeClass("display-none");
        self.Tables.report.removeClass("display-none");
    };
    self.formatCellColor = function (cellValue, isPositive, isNegative) {
        var cssClass = '';
        cellValue = cellValue.replace(/\,/g, '');

        if (isNegative && parseFloat(cellValue) < 0) {
            cssClass = ' color-red';
        }

        return cssClass;
    };
    self.renderReportTotalRow = function (rawRow) {
        var row = rawRow[0];
        var frozenHtml = '';
        frozenHtml = '<th><div class="col160 align-center">' + row.Name + '</div></th>'
        self.TempHtml.footerFrozenHtml += '<tr>' + frozenHtml + ' </tr>';
        self.TempHtml.footerContentHtml += '<tr>' +
                                            frozenHtml +
                                            '<th><div class="col120 align-right">' + row.MemberTO + '</div></th>\
                                            <th><div class="col120 align-right">' + row.MemberWL + '</div></th>\
                                            <th><div class="col100 align-right">' + row.BetCount + '</div></th>\
                                            <th><div class="col120 align-right">' + row.HouseTO + '</div></th>\
                                            <th><div class="col120 align-right' +
                                                self.formatCellColor(row.HouseWL, false, true) + '">' +
                                                row.HouseWL + '</div></th>\
                                            <th><div class="col120 align-right">' + row.SMATO + '</div></th>\
                                            <th><div class="col120 align-right">' + row.SMAWL + '</div></th>\
                                            <th><div class="col80 align-center">' + row.Currency + '</div></th>\
                                        </tr>';
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
    self.renderReportNoData = function () {
        self.Tables.reportExportExcel.addClass("display-none");
        var html = '<tr>\
                    <td colspan="9" class="no-data-message align-center">There is no data.</td>\
                </tr>';
        self.Tables.reportContent.empty().append(html);
        self.Tables.reportRowFooter.empty();
        self.Tables.reportColFooter.empty();
    };
    self.hideReport = function () {
        self.Tables.reportHeader.addClass("display-none");
        self.Tables.report.addClass("display-none");
    };
    self.showReport = function () {
        self.Tables.reportHeader.removeClass("display-none");
        self.Tables.report.removeClass("display-none");
    };
    self.exportReport = function () {
        AccReport.downloadFile(window.rootUrl + "Special/SpecialMatchAnalysis/ExportReport", self.Params.reportParams);
    };
}