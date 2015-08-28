Namespace.Register("Fanex.AccountingReports.Licensee.WinLossByLicensee.AgentReport");
Fanex.AccountingReports.Licensee.WinLossByLicensee.AgentReport = function () {
    var self = this;

    var _tables = {
        report: null,
        reportContent: null,
        reportTitle: null,
        reportExportExcel: null,
        reportRowHeader: null,
        reportRowFooter: null,
        reportColFooter: null,
        reportPagination: null,
        reportSorter: null,
        colLicenseeWinLoss: null,
        colIBCAmountDueTo: null
    };

    var _options = {
        report: {
            isShowPager: true,
            tblContentId: 'agentReportContent',
            manualCopyHeader: true,
            fullHeight: true,
            //fullWidth: true,
           // gridWidth: 1670,
            gridHeight: 275,
            //freezeWidth: 415,
            freezeHeight: 65,
            column: [140, 80, 140, 80, 120, 80, 120, 140, 120, 120, 120, 120, 120, 120, 120],
            numberFreezeCol: 4
        }
    };

    self.initPage = function () {
        initGridScrollerTables();
        cachedDOMElements();
        bindingEvent();
    };
    var initGridScrollerTables = function () {
        $("#agentReport").initGrid({
            isShowPager: true
        });
    };
    var cachedDOMElements = function () {
        _tables.report = $("#agentReport");
        _tables.reportHeader = $("#agentReportHeader");
        _tables.reportContent = _tables.report.find("#agentReportContent tbody");
        _tables.reportTitle = _tables.reportHeader.find("#agentReportTitle");
        _tables.reportExportExcel = _tables.reportHeader.find("#agentReport_ExportExcel");
        _tables.reportRowHeader = _tables.report.find("#agentReportRowHeader tbody");
        _tables.reportRowFooter = _tables.report.find(".row_footer_content thead");
        _tables.reportColFooter = _tables.report.find(".col_footer_content thead");
        _tables.reportPagination = _tables.report.find("#pagination");
        _tables.colLicenseeWinLoss = _tables.report.find(".colLicenseeWinLoss");
        _tables.colIBCAmountDueTo = _tables.report.find(".colIBCAmountDueTo");
    };
    var bindingEvent = function () {
        _tables.reportExportExcel.click(function () {
            exportReport();
        });
    };
    self.getReport = function (params) {
        AccReport.disableInput();
        if (!params.isPagingCallback) {
            self.getParams(params);
        }
        else {
            self.Params.reportParams.ItemsPerPage = self.Params.reportParams.pageSize;
            self.Params.reportParams.CurrentPage = self.Params.reportParams.pageIndex;
        }

        $.ajax({
            type: "POST",
            url: window.rootUrl + "Licensee/WinLossByLicensee/GetLevelReport",
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            data: JSON.stringify(self.Params.reportParams),
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
    };
    self.getParams = function (params) {
        var reportParam = {
            LicenseeID: params.LicenseeID,
            LicenseeName: params.LicenseeName,
            CustID: params.CustID,
            UserName: params.UserName,
            Currency: params.Currency,
            CurrencyName: params.CurrencyName,
            LevelReport: params.LevelReport,
            FromDate: params.FromDate,
            ToDate: params.ToDate,
            ShowSB: params.ShowSB,
            ShowRB: params.ShowRB,
            ShowNG: params.ShowNG,
            ShowBA: params.ShowBA,
            ShowLC: params.ShowLC,
            ShowVS: params.ShowVS,
            ShowCS: params.ShowCS,
            ShowKN: params.ShowKN,
            ShowAGC: params.ShowAGC,
            ShowGD: params.ShowGD,
            ShowMGC: params.ShowMGC,
            ReportType: parseInt(params.ReportType),
            Drawfilter: params.Drawfilter,
            ConvertToCurrency: params.ConvertToCurrency,
            ConvertToCurrencyName: params.ConvertToCurrencyName,
            ItemsPerPage: self.Params.reportParams.pageSize,
            CurrentPage: self.Params.reportParams.pageIndex,

            pageIndex: self.Params.reportParams.pageIndex,
            pageSize: self.Params.reportParams.pageSize,
            totalItem: self.Params.reportParams.totalItem
        };

        switch (reportParam.ReportType) {
            case enums.reportType.Daily:
                reportParam.FromDate = params.BettingDate;
                reportParam.ToDate = params.BettingDate;
                break;
            case enums.reportType.Monthly:
                reportParam.FromDate = params.FromDate;
                reportParam.ToDate = params.ToDate;
                break;
        }

        self.Params.reportParams = reportParam;
        return reportParam;
    };
    self.renderReport = function (report) {
        var reportTitle = "Win Loss by Licensee report for " + self.Params.reportParams.UserName +
            " from " + moment(self.Params.reportParams.FromDate).format(FormatConstant.MMM_DD_YYYY_STRING) +
            " to " + moment(self.Params.reportParams.ToDate).format(FormatConstant.MMM_DD_YYYY_STRING) +
            " in " + self.Params.reportParams.CurrencyName +
            (self.Params.reportParams.ConvertToCurrency == 0 ?
            '' : " (Base " + self.Params.reportParams.ConvertToCurrencyName + ")");
        _tables.reportTitle.html(reportTitle);
        _tables.colLicenseeWinLoss.html(self.getColLicenseeWinLoss());
        _tables.colIBCAmountDueTo.html(self.getColLicenseeWinLossSuffix());

        self.resetTempHtml();

        if (report &&
            report.TransactionList &&
            report.TransactionList.length) {
            _tables.reportExportExcel.removeClass("display-none");
            var reportLength = report.TransactionList.length;
            var drillDownLevel = enums.drilldownLevel.Member;
            for (var i = 0; i < reportLength; i++) {
                var reportItem = report.TransactionList[i];
                //                var cssClass = i % 2 == 0 ? 'even' : '';
                var cssClass = '';

                var frozenHtml = '<td><div class="col140 link" onclick="' +
                                getDrilldownLink(reportItem) + '">' +
                                reportItem.UserName + '</div></td>\
                                <td><div class="col80 align-center">' + reportItem.CurrencyName + '</div></td>\
                                <td><div class="col140 align-right">' + numeral(reportItem.Turnover).format(FormatConstant.TWO_DECIMAL_NUMBER) + '</div></td>\
                                <td><div class="col80 align-right">' + numeral(reportItem.BetCount).format(FormatConstant.INT_NUMBER) + '</div></td>';

                self.TempHtml.contentHtml += '<tr class="' + cssClass + '">' +
                                frozenHtml +
                                '<td><div class="col120 align-right">' + numeral(reportItem.CustomerWinLoss).format(FormatConstant.TWO_DECIMAL_NUMBER) + '</div></td>\
                                <td><div class="col120 align-right">' + numeral(reportItem.CustomerComm).format(FormatConstant.TWO_DECIMAL_NUMBER) + '</div></td>\
                                <td><div class="col120 align-right">' + numeral(reportItem.CustomerWinLossTotal).format(FormatConstant.TWO_DECIMAL_NUMBER) + '</div></td>\
                                <td><div class="col155 align-right">' + numeral(reportItem.LicenseeWinLoss).format(FormatConstant.TWO_DECIMAL_NUMBER) + '</div></td>\
                                <td><div class="col120 align-right">' + numeral(reportItem.BAComm).format(FormatConstant.TWO_DECIMAL_NUMBER) + '</div></td>\
                                <td><div class="col120 align-right">' + numeral(reportItem.RacingWinLoss).format(FormatConstant.TWO_DECIMAL_NUMBER) + '</div></td>\
                                <td><div class="col120 align-right">' + numeral(reportItem.RacingComm).format(FormatConstant.TWO_DECIMAL_NUMBER) + '</div></td>\
                                <td><div class="col120 align-right">' + numeral(reportItem.RacingWinLossTotal).format(FormatConstant.TWO_DECIMAL_NUMBER) + '</div></td>\
                                <td><div class="col120 align-right">' + numeral(reportItem.AmtDuetoBA).format(FormatConstant.TWO_DECIMAL_NUMBER) + '</div></td>\
                                <td><div class="col120 align-right">' + numeral(reportItem.AmtDuetoRacing).format(FormatConstant.TWO_DECIMAL_NUMBER) + '</div></td>\
                                <td><div class="col155 align-right">' + numeral(reportItem.AmtDuetoCompany).format(FormatConstant.TWO_DECIMAL_NUMBER) + '</div></td>\
                            </tr>';
                self.TempHtml.frozenHtml += '<tr class="' + cssClass + '">' + frozenHtml + '</tr>';
            }

            var totalLength = report.Total.length;
            for (var j = 0; j < totalLength; j++) {
                var totalItem = report.Total[j];
                self.renderTotalRow(totalItem, j == 0, totalLength, enums.drilldownLevel.Agent);
            }

            var baseCurrencyTotalLength = report.BaseCurrencyTotal.length;
            for (var k = 0; k < baseCurrencyTotalLength; k++) {
                var baseCurrencyTotalItem = report.BaseCurrencyTotal[k];
                self.renderBaseCurrencyTotalRow(baseCurrencyTotalItem, k == 0, baseCurrencyTotalLength, enums.drilldownLevel.Agent);
            }

            self.removeSpaceIE9TempHtml();
            _tables.reportRowHeader.empty().append(self.TempHtml.frozenHtml);
            _tables.reportContent.empty().append(self.TempHtml.contentHtml);
            _tables.reportRowFooter.empty().append(self.TempHtml.footerFrozenHtml);
            _tables.reportColFooter.empty().append(self.TempHtml.footerContentHtml);

            self.initPager({
                totalItems: report.ReportItemLength,
                currentPage: self.Params.reportParams.pageIndex - 1,
                pager: _tables.reportPagination
            }, self);
        } else {
            renderNoDataReport();
            self.initPager({
                totalItems: 0,
                currentPage: 1,
                pager: _tables.reportPagination
            }, self);
        }

        _tables.report.removeClass("display-none")
                      .gridScroller(_options.report);
        showReport(enums.drilldownLevel.Agent);
    };
    var renderNoDataReport = function () {
        _tables.reportExportExcel.addClass("display-none");
        var frezeeContent = "<td colspan='4'>&nbsp</td>";
        _tables.reportRowHeader.empty().append("<tr>" + frezeeContent + "</tr>");
        _tables.reportContent.empty().append(
                "<tr>" +
                    frezeeContent +
                    "<td colspan='11' class='c'><div class='no-data-message-agent-report'>There is no data.</div></td>\
                 </tr>");
        _tables.reportRowFooter.empty();
        _tables.reportColFooter.empty();
    };
    var getDrilldownLink = function (row) {
        var link = 'drilldownToMemberReport(' + row.UserId + ',\'' +
                                                row.UserName + '\',\'' +
                                                self.Params.reportParams.FromDate + '\',\'' +
                                                self.Params.reportParams.FromDate + '\',\'' +
                                                self.Params.reportParams.ToDate + '\',\'' +
                                                enums.drilldownLevel.Agent + '\')';
        return link;
    };
    self.pageSelectCallback = function (page_index, itemsPerPage, jq) {
        var oldPagingParams = {
            pageIndex: parseInt(self.Params.reportParams.pageIndex),
            totalItem: parseInt(self.Params.reportParams.totalItem),
            pageSize: parseInt(self.Params.reportParams.pageSize)
        };

        if (oldPagingParams.pageSize != itemsPerPage) {
            self.Options.paging.currentPage = 0;
            self.Params.reportParams.pageIndex = 1;
        }
        else if (page_index * itemsPerPage < oldPagingParams.totalItem) {
            self.Params.reportParams.pageIndex = page_index + 1;
            self.Options.paging.currentPage = page_index;
        }

        self.Options.paging.itemsPerPage = itemsPerPage;
        self.Params.reportParams.pageSize = itemsPerPage;

        if (!(oldPagingParams.totalItem <= oldPagingParams.pageSize &&
           oldPagingParams.pageIndex == 1 && oldPagingParams.pageSize <= itemsPerPage)) {
            self.getReport({
                isPagingCallback: true
            });
        }

        return false;
    };
    self.showReport = function () {
        _tables.reportHeader.removeClass("display-none");
        _tables.report.removeClass("display-none");
    };
    self.hideReport = function () {
        _tables.reportHeader.addClass("display-none");
        _tables.report.addClass("display-none");
        self.Options.paging.currentPage = 0;
        self.Options.paging.itemsPerPage = 50;
        self.Params.reportParams.pageIndex = 1;
        self.Params.reportParams.pageSize = 50;
    };
    var exportReport = function () {
        AccReport.downloadFile(window.rootUrl + "Licensee/WinLossByLicensee/ExportLevelReport",
                               self.Params.reportParams);
    };
}