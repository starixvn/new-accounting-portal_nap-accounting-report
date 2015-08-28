Namespace.Register("Fanex.AccountingReports.Licensee.WinLossByLicensee.BaseReport");
Fanex.AccountingReports.Licensee.WinLossByLicensee.BaseReport = function () {
    var self = this;

    self.Params = {
        reportParams: {
            LicenseeID: null,
            LicenseeName: null,
            Currency: null,
            CurrencyName: '',
            LevelReport: null,
            FromDate: null,
            ToDate: null,
            ShowSB: true,
            ShowRB: true,
            ShowNG: true,
            ShowBA: true,
            ShowLC: true,
            ShowVS: false,
            ShowAGC: true,
            ShowGD: true,
            ShowMGC: true,
            ReportType: null,
            Drawfilter: 1,
            ConvertToCurrency: null,
            ConvertToCurrencyName: '',
            isMultiAgent: false,
            ItemsPerPage: null,
            CurrentPage: null,
            CustID: null,

            pageIndex: 1,
            pageSize: 50,
            totalItem: 0
        }
    };

    self.Options = {
        paging: {
            callback: null,
            startPerPage: 50,
            stepPerPage: 50,
            maxTtemsPerPage: 500,
            itemsPerPage: 50,
            currentPage: 0,
            itemsPerPageArray: [50, 100, 200, 500]
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
    self.getColLicenseeWinLoss = function () {
        var suffix = self.getColLicenseeWinLossSuffix();
        if (suffix != '') {
            return "Licensee Win Loss" + ' (' + suffix + ')';
        }
        return "Licensee Win Loss";
    };
    self.getColLicenseeWinLossSuffix = function () {
        var suffix = '';
        var products = [];

        if (self.Params.reportParams.ShowSB == true) {
            products.push("SB");
        }

        if (self.Params.reportParams.ShowNG == true) {
            products.push("NG");
        }

        if (self.Params.reportParams.ShowLC == true) {
            products.push("LC");
        }

        if (self.Params.reportParams.ShowVS == true) {
            products.push("VS");
        }

        if (self.Params.reportParams.ShowCS == true) {
            products.push("CS");
        }

        if (self.Params.reportParams.ShowKN == true) {
            products.push("KN");
        }

        if (self.Params.reportParams.ShowAGC == true) {
            products.push("AGC");
        }

        if (self.Params.reportParams.ShowGD == true) {
            products.push("GD");
        }

        if (self.Params.reportParams.ShowMGC == true) {
            products.push("MGC");
        }

        if (products.length > 0) {
            suffix = products.join(' + ');
        }
        else {
            suffix = 'SB + NG + LC + VS + CS + KN';
        }

        return suffix;
    };
    self.renderTotalRow = function (row, isFirstRow, totalLength, level) {
        var html = '';
        var frozenHtml = '';

        if (isFirstRow) {
            frozenHtml = '<td rowspan="' + totalLength + '" class="totalRow"><div class="' +
                            (level == enums.drilldownLevel.Currency ? "col100" : "col140") +
                            ' align-center">Total</div></td>';
        }

        frozenHtml += '<td><div class="col80 align-center">' + row.CurrencyName + '</div></td>\
                       <td><div class="col140 align-right">' + numeral(row.Turnover).format(FormatConstant.TWO_DECIMAL_NUMBER) + '</div></td>\
                       <td><div class="col80 align-right">' + numeral(row.BetCount).format(FormatConstant.INT_NUMBER) + '</div></td>';

        html = '<tr class="totalRow">' +
                frozenHtml +
                '<td><div class="col120 align-right">' + numeral(row.CustomerWinLoss).format(FormatConstant.TWO_DECIMAL_NUMBER) + '</div></td>\
                <td><div class="col120 align-right">' + numeral(row.CustomerComm).format(FormatConstant.TWO_DECIMAL_NUMBER) + '</div></td>\
                <td><div class="col120 align-right">' + numeral(row.CustomerWinLossTotal).format(FormatConstant.TWO_DECIMAL_NUMBER) + '</div></td>\
                <td><div class="col155 align-right">' + numeral(row.LicenseeWinLoss).format(FormatConstant.TWO_DECIMAL_NUMBER) + '</div></td>\
                <td><div class="col120 align-right">' + numeral(row.BAComm).format(FormatConstant.TWO_DECIMAL_NUMBER) + '</div></td>\
                <td><div class="col120 align-right">' + numeral(row.RacingWinLoss).format(FormatConstant.TWO_DECIMAL_NUMBER) + '</div></td>\
                <td><div class="col120 align-right">' + numeral(row.RacingComm).format(FormatConstant.TWO_DECIMAL_NUMBER) + '</div></td>\
                <td><div class="col120 align-right">' + numeral(row.RacingWinLossTotal).format(FormatConstant.TWO_DECIMAL_NUMBER) + '</div></td>\
                <td><div class="col120 align-right">' + numeral(row.AmtDuetoBA).format(FormatConstant.TWO_DECIMAL_NUMBER) + '</div></td>\
                <td><div class="col120 align-right">' + numeral(row.AmtDuetoRacing).format(FormatConstant.TWO_DECIMAL_NUMBER) + '</div></td>\
                <td><div class="col155 align-right">' + numeral(row.AmtDuetoCompany).format(FormatConstant.TWO_DECIMAL_NUMBER) + '</div></td>\
            </tr>';

        self.TempHtml.frozenHtml += '<tr class="totalRow">' + frozenHtml + '</tr>';
        self.TempHtml.contentHtml += html;
    };
    self.renderBaseCurrencyTotalRow = function (row, isFirstRow, totalLength, level) {
        var html = '';
        var frozenHtml = '';

        if (isFirstRow) {
            frozenHtml = '<th rowspan="' + totalLength + '"><div class="' +
             (level == enums.drilldownLevel.Currency ? "col100" : "col140") +
             ' align-center">Total</div></th>';
        }

        frozenHtml += '<th><div class="col80 align-center">' + row.CurrencyName + '</div></th>\
                       <th><div class="col140 align-right">' + numeral(row.Turnover).format(FormatConstant.TWO_DECIMAL_NUMBER) + '</div></th>';
        if (isFirstRow) {
            frozenHtml += ' <th rowspan="' + totalLength + '"><div class="col80 align-right">' + numeral(row.BetCount).format(FormatConstant.INT_NUMBER) + '</div></th>';
        }

        html = '<tr class="">' +
                frozenHtml +
                '<th><div class="col120 align-right">' + numeral(row.CustomerWinLoss).format(FormatConstant.TWO_DECIMAL_NUMBER) + '</div></th>\
                <th><div class="col120 align-right">' + numeral(row.CustomerComm).format(FormatConstant.TWO_DECIMAL_NUMBER) + '</div></th>\
                <th><div class="col120 align-right">' + numeral(row.CustomerWinLossTotal).format(FormatConstant.TWO_DECIMAL_NUMBER) + '</div></th>\
                <th><div class="col155 align-right">' + numeral(row.LicenseeWinLoss).format(FormatConstant.TWO_DECIMAL_NUMBER) + '</div></th>\
                <th><div class="col120 align-right">' + numeral(row.BAComm).format(FormatConstant.TWO_DECIMAL_NUMBER) + '</div></th>\
                <th><div class="col120 align-right">' + numeral(row.RacingWinLoss).format(FormatConstant.TWO_DECIMAL_NUMBER) + '</div></th>\
                <th><div class="col120 align-right">' + numeral(row.RacingComm).format(FormatConstant.TWO_DECIMAL_NUMBER) + '</div></th>\
                <th><div class="col120 align-right">' + numeral(row.RacingWinLossTotal).format(FormatConstant.TWO_DECIMAL_NUMBER) + '</div></th>\
                <th><div class="col120 align-right">' + numeral(row.AmtDuetoBA).format(FormatConstant.TWO_DECIMAL_NUMBER) + '</div></th>\
                <th><div class="col120 align-right">' + numeral(row.AmtDuetoRacing).format(FormatConstant.TWO_DECIMAL_NUMBER) + '</div></th>\
                <th><div class="col155 align-right">' + numeral(row.AmtDuetoCompany).format(FormatConstant.TWO_DECIMAL_NUMBER) + '</div></th>\
            </tr>';

        self.TempHtml.footerFrozenHtml += '<tr class="">' + frozenHtml + '</tr>';
        self.TempHtml.footerContentHtml += html;
    };
    self.initPager = function (pagingCriteria, reportHandler) {
        self.Params.reportParams.totalItem = pagingCriteria.totalItems;
        self.Options.paging.currentPage = pagingCriteria.currentPage;
        self.Options.paging.callback = reportHandler.pageSelectCallback;
        if (pagingCriteria.totalItems > 0) {
            pagingCriteria.pager.removeClass("display-none")
                                    .fanexPagination(pagingCriteria.totalItems, self.Options.paging);
        }
        else {
            pagingCriteria.pager.addClass("display-none");
        }
    };
}
