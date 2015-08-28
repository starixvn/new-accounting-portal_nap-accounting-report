Namespace.Register("Fanex.AccountingReports.Special.SpecialMatchAnalysis.MatchAnalysisByAgent");
Fanex.AccountingReports.Special.SpecialMatchAnalysis.MatchAnalysisByAgent = function () {
    var self = this;

    self.Options = {
        report: {
            isShowPager: true,
            tblContentId: 'matchAnalysisByAgentReportContent',
            fullHeight: true,
            gridHeight: 232,
            freezeHeight: 280,
            column: [160, 120, 120, 100, 120, 120, 120, 120, 80],
            numberFreezeCol: 1
        }
    };

    self.initGridScrollerTables = function () {
        $("#matchAnalysisByAgentReport").initGrid({
            isShowPager: false
        });
    };
    self.cachedDOMElements = function () {
        self.Tables.report = $("#matchAnalysisByAgentReport");
        self.Tables.reportHeader = $("#matchAnalysisByAgentReportHeader");
        self.Tables.reportContent = self.Tables.report.find("#matchAnalysisByAgentReportContent tbody");
        self.Tables.reportTitle = self.Tables.reportHeader.find("#matchAnalysisByAgentReportTitle");
        self.Tables.reportExportExcel = self.Tables.reportHeader.find("#matchAnalysisByAgentReport_ExportExcel");
        self.Tables.reportRowHeader = self.Tables.report.find("#matchAnalysisByAgentReportRowHeader tbody");
        self.Tables.reportRowFooter = self.Tables.report.find(".row_footer_content thead");
        self.Tables.reportColFooter = self.Tables.report.find(".col_footer_content thead");
    };
    self.bindingEvent = function () {
        self.Tables.reportExportExcel.click(function () {
            self.exportReport();
        });
    };
    self.renderReportRow = function (row, cssClass) {
        var frozenHtml = '';
        var downlineLink = '<div class="link" onclick="ViewMatchAnalysisByMember(' + row.Id + ',\'' + row.Name + '\');">' + row.Name + '</a>';

        frozenHtml = '<td><div class="col160 align-center">' + downlineLink + '</div></td>'
        self.TempHtml.frozenHtml += '<tr class="' + cssClass + '">' + frozenHtml + ' </tr>';
        self.TempHtml.contentHtml += '<tr class="' + cssClass + '">' +
                                        frozenHtml +
                                        '<td><div class="col120 align-right">' + row.MemberTO + '</div></td>\
                                        <td><div class="col120 align-right">' + row.MemberWL + '</div></td>\
                                        <td><div class="col100 align-right">' + row.BetCount + '</div></td>\
                                        <td><div class="col120 align-right">' + row.HouseTO + '</div></td>\
                                        <td><div class="col120 align-right' +
                                                self.formatCellColor(row.HouseWL, false, true) + '">' +
                                                row.HouseWL + '</div></td>\
                                        <td><div class="col120 align-right">' + row.SMATO + '</div></td>\
                                        <td><div class="col120 align-right">' + row.SMAWL + '</div></td>\
                                        <td><div class="col80 align-center">' + row.Currency + '</div></td>\
                                    </tr>';
    };
}