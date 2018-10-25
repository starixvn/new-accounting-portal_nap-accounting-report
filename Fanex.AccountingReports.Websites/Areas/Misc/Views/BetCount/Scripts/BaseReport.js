
Namespace.Register("Fanex.AccountingReports.Misc.BetCount.BaseReport");
Fanex.AccountingReports.Misc.BetCount.BaseReport = function () {
    var self = this;

    self.Enums = {
        sortBy: {
            TotalTurnover: 'TotalTurnover',
            MemberWinLoss: 'MemberWinLoss',
            MarginPercent: 'MarginPercent',
            AgentWinLoss: 'AgentWinLoss',
            MasterWinLoss: 'MasterWinLoss',
            SuperWinLoss: 'SuperWinLoss',
            CompanyWinLoss: 'CompanyWinLoss',
            TotalWinLoss: 'TotalWinLoss',
            Commission: 'Commission',
            ExtraPT: 'ExtraPT',
            None: ''
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

    self.formatCellColor = function (cellValue, isPositive, isNegative) {
        var cssClass = '';
        if (isPositive && cellValue > 0) {
            cssClass = ' color-red';
        }

        return cssClass;
    };
    self.renderReportRow = function (row, isSubTotal) {
        var frozenHtml = '';
        var cssClass = isSubTotal ? 'totalRow' : '';
        frozenHtml = '<td><div class="col80' + (isSubTotal ? '' : ' align-right') + '">' + row.SiteName + '</div></td>';
        self.TempHtml.frozenHtml += '<tr class="' + cssClass + '">' + frozenHtml + ' </tr>';
        self.TempHtml.contentHtml += '<tr class="' + cssClass + '">' +
                                                frozenHtml +
                                                '<td><div class="col120 align-right">' + numeral(row.BetCount).format(FormatConstant.INT_NUMBER) + '</div></td>\
                                                <td><div class="col140 align-right">' + numeral(row.Turnover).format(FormatConstant.TWO_DECIMAL_NUMBER) + '</div></td>\
                                                <td><div class="col150 align-right">' + numeral(row.MemberWinLoss).format(FormatConstant.TWO_DECIMAL_NUMBER) + '</div></td>\
                                                <td><div class="col80 align-right">' + numeral(row.MarginPercent).format(FormatConstant.TWO_DECIMAL_NUMBER) + '</div></td>\
                                                <td><div class="col140 align-right">' + numeral(row.AgentWinLoss).format(FormatConstant.TWO_DECIMAL_NUMBER) + '</div></td>\
                                                <td><div class="col140 align-right">' + numeral(row.MasterWinLoss).format(FormatConstant.TWO_DECIMAL_NUMBER) + '</div></td>\
                                                <td><div class="col140 align-right">' + numeral(row.SuperWinLoss).format(FormatConstant.TWO_DECIMAL_NUMBER) + '</div></td>\
                                                <td><div class="col140 align-right">' + numeral(row.TotalWinLoss).format(FormatConstant.TWO_DECIMAL_NUMBER) + '</div></td>\
                                                <td><div class="col140 align-right">' + numeral(row.Commission).format(FormatConstant.TWO_DECIMAL_NUMBER) + '</div></td>\
                                                <td><div class="col140 align-right">' + numeral(row.ExtraPT).format(FormatConstant.TWO_DECIMAL_NUMBER) + '</div></td>\
                                                <td><div class="col140 align-right' +
                                                self.formatCellColor(row.CompanyWinLoss, true, false) + '">' +
                                                numeral(row.CompanyWinLoss).format(FormatConstant.TWO_DECIMAL_NUMBER) + '</div></td>\
                                            </tr>';
    };
    self.renderReportTotalRow = function (row) {
        var frozenHtml = '';
        frozenHtml = '<th><div class="col80 align-center">' + row.SiteName + '</div></th>';
        self.TempHtml.footerFrozenHtml += '<tr>' + frozenHtml + ' </tr>';
        self.TempHtml.footerContentHtml += '<tr>' +
                                            frozenHtml +
                                            '<th><div class="col120 align-right">' + numeral(row.BetCount).format(FormatConstant.INT_NUMBER) + '</div></th>\
                                            <th><div class="col140 align-right">' + numeral(row.Turnover).format(FormatConstant.TWO_DECIMAL_NUMBER) + '</div></th>\
                                            <th><div class="col150 align-right">' + numeral(row.MemberWinLoss).format(FormatConstant.TWO_DECIMAL_NUMBER) + '</div></th>\
                                            <th><div class="col80 align-right">' + numeral(row.MarginPercent).format(FormatConstant.TWO_DECIMAL_NUMBER) + '</div></th>\
                                            <th><div class="col140 align-right">' + numeral(row.AgentWinLoss).format(FormatConstant.TWO_DECIMAL_NUMBER) + '</div></th>\
                                            <th><div class="col140 align-right">' + numeral(row.MasterWinLoss).format(FormatConstant.TWO_DECIMAL_NUMBER) + '</div></th>\
                                            <th><div class="col140 align-right">' + numeral(row.SuperWinLoss).format(FormatConstant.TWO_DECIMAL_NUMBER) + '</div></th>\
                                            <th><div class="col140 align-right">' + numeral(row.TotalWinLoss).format(FormatConstant.TWO_DECIMAL_NUMBER) + '</div></th>\
                                            <th><div class="col140 align-right">' + numeral(row.Commission).format(FormatConstant.TWO_DECIMAL_NUMBER) + '</div></th>\
                                            <th><div class="col140 align-right">' + numeral(row.ExtraPT).format(FormatConstant.TWO_DECIMAL_NUMBER) + '</div></th>\
                                            <th><div class="col140 align-right' +
                                                self.formatCellColor(row.CompanyWinLoss, true, false) + '">' +
                                                numeral(row.CompanyWinLoss).format(FormatConstant.TWO_DECIMAL_NUMBER) + '</div></th>\
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
}