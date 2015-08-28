Namespace.Register("Fanex.AccountingReports.Special.SpecialMatchAnalysis.MatchAnalysisByMaster");
Fanex.AccountingReports.Special.SpecialMatchAnalysis.MatchAnalysisByMaster = function () {
    var self = this;

    self.Options = {
        report: {
            isShowPager: true,
            tblContentId: 'matchAnalysisByMasterReportContent',
            fullHeight: true,
            gridHeight: 232,
            freezeHeight: 280,
            column: [160, 120, 120, 100, 120, 120, 120, 120, 80],
            numberFreezeCol: 1
        }
    };

    self.initGridScrollerTables = function () {
        $("#matchAnalysisByMasterReport").initGrid({
            isShowPager: false
        });
    };
    self.cachedDOMElements = function () {
        self.Tables.report = $("#matchAnalysisByMasterReport");
        self.Tables.reportHeader = $("#matchAnalysisByMasterReportHeader");
        self.Tables.reportContent = self.Tables.report.find("#matchAnalysisByMasterReportContent tbody");
        self.Tables.reportTitle = self.Tables.reportHeader.find("#matchAnalysisByMasterReportTitle");
        self.Tables.reportExportExcel = self.Tables.reportHeader.find("#matchAnalysisByMasterReport_ExportExcel");
        self.Tables.reportRowHeader = self.Tables.report.find("#matchAnalysisByMasterReportRowHeader tbody");
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
        var downlineLink = '<div class="link" onclick="ViewMatchAnalysisByAgent(' + row.Id + ',\'' + row.Name + '\');">' + row.Name + '</a>';

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