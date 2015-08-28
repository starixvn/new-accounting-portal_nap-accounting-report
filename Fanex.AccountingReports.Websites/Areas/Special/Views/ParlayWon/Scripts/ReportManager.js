$(document).ready(function () {
    var reportManager = new Fanex.AccountingReports.Special.ParlayWon.ReportManager();
    reportManager.initPage();
});

Namespace.Register("Fanex.AccountingReports.Special.ParlayWon.ReportManager");
Fanex.AccountingReports.Special.ParlayWon.ReportManager = function () {
    var self = this;

    self.Enums = {
        sortBy: {
            BaseWinLoss: 'BaseWinLoss',
            BaseCompanyWinLoss: 'BaseCompanyWinLoss',
            None: ''
        }
    };

    var _tables = {
        datepickerplugin: null,
        betTypeList: null,
        sportList: null,

        report: null,
        reportContent: null,
        reportTitle: null,
        reportExportExcel: null,
        reportRowHeader: null,
        reportRowFooter: null,
        reportColFooter: null,
        reportPagination: null,
        reportSorter: null
    }

    var _options = {
        report: {
            isShowPager: true,
            tblContentId: 'parlayWonReportContent',
            fullHeight: true,
            gridHeight: 193,
            freezeHeight: 280,
            column: [90, 130, 70, 70, 100, 100, 100, 100, 130, 160, 60, 70],
            numberFreezeCol: 4
        },

        paging: {
            callback: null,
            startPerPage: 50,
            stepPerPage: 50,
            maxTtemsPerPage: 500,
            itemsPerPage: 50,
            currentPage: 0,
            itemsPerPageArray: [1, 2, 50, 100, 200, 500]
        }
    };

    var _params = {
        reportParams: {
            WinlostDate: null,
            Bettype: null,
            BettypeName: null,
            IsSoccer: false,
            SportName: null,
            SortBy: null,
            SortDirection: null,
            IsGetAll: null,
            ItemsPerPage: 50,
            TotalItem: 0,
            CurrentPage: 1,
            IsPagingCallBack: false
        }
    };

    // _tempHtml: use for save frozen, scroll html of report 
    // when render by multi function
    _tempHtml = {
        frozenHtml: '',
        contentHtml: '',
        footerFrozenHtml: '',
        footerContentHtml: ''
    };

    var _cachedData = {
        parlayWonReport: null,
        lastSorterBy: null
    };

    self.initPage = function () {
        initGridScrollerTables();
        renderDatePicker();
        cachedDOMElements();
        bindingEvent();
    };
    var initGridScrollerTables = function () {
        $("#parlayWonReport").initGrid({
            isShowPager: true
        });
    };
    var renderDatePicker = function () {
        $('#datepickerplugin').nexDatePicker({
            nexDatePickerType: "single",
            firstDatePicker: {
                Id: "CheckDate",
                Label: labelDate,
                InitDate: today,
                LabelCss: "lbl-field col_2",
                TextboxCss: "edit-field col_6"

            },

            datePickerFormat: {
                dateFormat: FormatConstant.M_DD_YY_STRING,
                changeMonth: true,
                changeYear: true,
                minDate: minDate,
                maxDate: new Date(today)
            }
        });

        _tables.datepickerplugin = $("#datepickerplugin");
    };
    var cachedDOMElements = function () {
        _tables.btnGenerate = $("#btnGenerate");
        _tables.betTypeList = $("#betTypeList");
        _tables.sportList = $("#sportList");

        _tables.report = $("#parlayWonReport");
        _tables.reportHeader = $("#parlayWonReportHeader");
        _tables.reportContent = _tables.report.find("#parlayWonReportContent tbody");
        _tables.reportTitle = _tables.reportHeader.find("#parlayWonReportTitle");
        _tables.reportExportExcel = _tables.reportHeader.find("#parlayWonReport_ExportExcel");
        _tables.reportRowHeader = _tables.report.find("#parlayWonReportRowHeader tbody");
        _tables.reportRowFooter = _tables.report.find(".row_footer_content thead");
        _tables.reportColFooter = _tables.report.find(".col_footer_content thead");
        _tables.reportPagination = _tables.report.find("#pagination");
        _tables.reportSorter = _tables.report.find(".colSorter");
        _tables.reportDivSorter = _tables.report.find(".divSorter");
    };
    var bindingEvent = function () {
        _tables.btnGenerate.click(function () {
            self.btnGenerateClick();
        });

        _tables.reportExportExcel.click(function () {
            exportReport();
        });

        _tables.reportSorter.click(function () {
            if (_cachedData.parlayWonReport &&
                _cachedData.parlayWonReport.TransactionList.length > 0) {
                AccReport.gridSorterManager.sortColumn($(this).find(".divSorter"), self.sortReport);
            }
        });
    };
    self.btnGenerateClick = function () {
        _params.reportParams.CurrentPage = 1;
        _params.reportParams.IsPagingCallBack = false;
        _options.paging.currentPage = 0;
        AccReport.gridSorterManager.resetColumns(_tables.reportSorter.find(".divSorter"));
        self.getReport();
    };
    self.getReport = function () {
        AccReport.disableInput();
        if (!_params.reportParams.IsPagingCallBack) {
            getParams();
        }

        if (validateParams() == true) {
            $.ajax({
                type: "POST",
                url: window.rootUrl + "Special/ParlayWon/GetReport",
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
        _params.reportParams.WinlostDate = _tables.datepickerplugin.nexDatePicker("getSingleDate");

        var selectedBetType = _tables.betTypeList.find("input[name='rdBetType']:checked");
        _params.reportParams.Bettype = selectedBetType.val();
        _params.reportParams.BettypeName = _params.reportParams.Bettype == "9"
                                            ? "Mix Parlay"
                                            : "System Parlay";

        var selectedSport = _tables.sportList.find("input[name='rdSport']:checked");
        _params.reportParams.IsSoccer = selectedSport.attr("id") == "rdSportSoccer";
        _params.reportParams.SportName = _params.reportParams.IsSoccer ? "Soccer" : "Other Sports";

        _params.reportParams.SortBy = '';
        _params.reportParams.SortDirection = '';
        _params.reportParams.ItemsPerPage = _options.paging.itemsPerPage;
        _params.reportParams.CurrentPage = _options.paging.currentPage + 1;
        _params.reportParams.IsGetAll = false;
    };
    var validateParams = function () {
        return true;
    };
    self.renderReport = function (report) {
        _cachedData.parlayWonReport = report;
        var reportTitle = _params.reportParams.BettypeName + " - " +
                          _params.reportParams.SportName + " Won on " +
                          _params.reportParams.WinlostDate;

        _tables.reportTitle.html(reportTitle);

        if (report &&
            report.TransactionList &&
            report.TransactionList.length > 0) {
            _tables.reportDivSorter.addClass("divSorter");
            _tables.reportExportExcel.removeClass("display-none");
            resetTempHtml();

            var frozenHtml = '';
            var transactionLength = report.TransactionList.length;
            for (var i = 0; i < transactionLength; i++) {
                var reportItem = report.TransactionList[i];
                renderReportRow(reportItem, i % 2 ? 'even' : '');
            }

            renderReportTotalRow(report.Total);
            removeSpaceIE9TempHtml();
            _tables.reportRowHeader.empty().append(_tempHtml.frozenHtml);
            _tables.reportContent.empty().append(_tempHtml.contentHtml);
            _tables.reportRowFooter.empty().append(_tempHtml.footerFrozenHtml);
            _tables.reportColFooter.empty().append(_tempHtml.footerContentHtml);

            initPager({
                totalItems: report.ReportItemLength,
                currentPage: _params.reportParams.CurrentPage,
                pager: _tables.reportPagination
            }, self);
        }
        else {
            renderReportNoData();
            initPager({
                totalItems: 0,
                currentPage: 1,
                pager: _tables.reportPagination
            }, self);
        }

        _tables.report.removeClass("display-none")
                      .gridScroller(_options.report);

        _tables.reportHeader.removeClass("display-none");
        _tables.report.removeClass("display-none");
    };
    var renderReportRow = function (row, cssClass) {
        var frozenHtml = '';
        frozenHtml = '<td><div class="col90 align-center">' + row.RefNo + '</div></td>\
                      <td><div class="col130 ">' + row.UserName + '</div></td>\
                      <td><div class="col70 align-center">' + row.Currency + '</div></td>\
                      <td><div class="col70 align-right">' + numeral(row.Exchange).format(FormatConstant.FOUR_DECIMAL_NUMBER) + '</div></td>';
        _tempHtml.frozenHtml += '<tr class="' + cssClass + '">' + frozenHtml + ' </tr>';
        _tempHtml.contentHtml += '<tr class="' + cssClass + '">' +
                                            frozenHtml +
                                            '<td><div class="col100 align-right">' + numeral(row.OriginalStake).format(FormatConstant.TWO_DECIMAL_NUMBER) + '</div></td>\
                                            <td><div class="col100 align-right">' + numeral(row.OriginalWinLoss).format(FormatConstant.TWO_DECIMAL_NUMBER) + '</div></td>\
                                            <td><div class="col100 align-right">' + numeral(row.BaseStake).format(FormatConstant.TWO_DECIMAL_NUMBER) + '</div></td>\
                                            <td><div class="col100 align-right">' + numeral(row.BaseWinLoss).format(FormatConstant.TWO_DECIMAL_NUMBER) + '</div></td>\
                                            <td><div class="col130 align-right">' + numeral(row.CompanyStake).format(FormatConstant.TWO_DECIMAL_NUMBER) + '</div></td>\
                                            <td><div class="col160 align-right">' + numeral(row.CompanyWinLoss).format(FormatConstant.TWO_DECIMAL_NUMBER) + '</div></td>\
                                            <td><div class="col60 align-right">' + numeral(row.Selection).format(FormatConstant.INT_NUMBER) + '</div></td>\
                                            <td><div class="col90 align-right">' + numeral(row.WinOdds).format(FormatConstant.FOUR_DECIMAL_NUMBER) + '</div></td>\
                                        </tr>';
    };
    var renderReportTotalRow = function (row) {
        var frozenHtml = '';
        frozenHtml = '<th colspan="4"><div class="freeze4Col align-right">' + row.UserName + '</div></th>';
        _tempHtml.footerFrozenHtml += '<tr>' + frozenHtml + ' </tr>';
        _tempHtml.footerContentHtml += '<tr>' +
                                            frozenHtml +
                                            '<th><div class="col100 align-right"></div></th>\
                                            <th><div class="col100 align-right"></div></th>\
                                            <th><div class="col100 align-right">' + numeral(row.BaseStake).format(FormatConstant.TWO_DECIMAL_NUMBER) + '</div></th>\
                                            <th><div class="col100 align-right">' + numeral(row.BaseWinLoss).format(FormatConstant.TWO_DECIMAL_NUMBER) + '</div></th>\
                                            <th><div class="col130 align-right">' + numeral(row.CompanyStake).format(FormatConstant.TWO_DECIMAL_NUMBER) + '</div></th>\
                                            <th><div class="col160 align-right">' + numeral(row.CompanyWinLoss).format(FormatConstant.TWO_DECIMAL_NUMBER) + '</div></th>\
                                            <th><div class="col60 align-right"></div></th>\
                                            <th><div class="col90 align-right"></div></th>\
                                        </tr>';
    };
    var renderReportNoData = function () {
        _tables.reportExportExcel.addClass("display-none");
        _tables.reportDivSorter.removeClass("divSorter");
        var frezeeContent = "<td colspan='4'><div class='no-data-message'>There is no data.</div></td>";
        _tables.reportRowHeader.empty().append("<tr>" + frezeeContent + "</tr>");
        _tables.reportContent.empty().append(
                "<tr>" +
                    frezeeContent +
                    "<td colspan='8'>&nbsp</td>\
                 </tr>");
        _tables.reportRowFooter.empty();
        _tables.reportColFooter.empty();
    };
    var resetTempHtml = function () {
        _tempHtml.frozenHtml = '';
        _tempHtml.contentHtml = '';
        _tempHtml.footerFrozenHtml = '';
        _tempHtml.footerContentHtml = '';
    };
    var removeSpaceIE9TempHtml = function () {
        _tempHtml.frozenHtml = fixBugsBrowser.removeSpaceIE9(_tempHtml.frozenHtml);
        _tempHtml.contentHtml = fixBugsBrowser.removeSpaceIE9(_tempHtml.contentHtml);
        _tempHtml.footerFrozenHtml = fixBugsBrowser.removeSpaceIE9(_tempHtml.footerFrozenHtml);
        _tempHtml.footerContentHtml = fixBugsBrowser.removeSpaceIE9(_tempHtml.footerContentHtml);
    };
    var initPager = function (pagingCriteria, reportHandler) {
        _params.reportParams.TotalItem = pagingCriteria.totalItems;
        _options.paging.currentPage = pagingCriteria.currentPage - 1;
        _options.paging.callback = reportHandler.pageSelectCallback;
        if (pagingCriteria.totalItems > 0) {
            pagingCriteria.pager.removeClass("display-none")
                                .fanexPagination(pagingCriteria.totalItems, _options.paging);
        }
        else {
            pagingCriteria.pager.addClass("display-none");
        }
    };
    self.pageSelectCallback = function (page_index, itemsPerPage, jq) {
        var oldPagingParams = {
            pageIndex: parseInt(_params.reportParams.CurrentPage),
            totalItem: parseInt(_params.reportParams.TotalItem),
            pageSize: parseInt(_params.reportParams.ItemsPerPage)
        };

        if (oldPagingParams.pageSize != itemsPerPage) {
            _options.paging.currentPage = 0;
            _params.reportParams.CurrentPage = 1;
        }
        else if (page_index * itemsPerPage <= oldPagingParams.totalItem) {
            _options.paging.currentPage = page_index;
            _params.reportParams.CurrentPage = page_index + 1;
        }

        _options.paging.itemsPerPage = itemsPerPage;
        _params.reportParams.ItemsPerPage = itemsPerPage;

        if (!(oldPagingParams.totalItem <= oldPagingParams.pageSize &&
           oldPagingParams.pageIndex == 1 && oldPagingParams.pageSize <= itemsPerPage)) {
            _params.reportParams.IsPagingCallBack = true;
            self.getReport();
        }

        return false;
    };
    self.sortReport = function (column, newSortDirection) {
        var columnId = column.attr("id");
        if (_cachedData.lastSorterBy != null &&
            _cachedData.lastSorterBy.attr("id") != columnId) {
            AccReport.gridSorterManager.resetColumns(_cachedData.lastSorterBy);
        }

        _cachedData.lastSorterBy = column;
        var sortBy = self.Enums.sortBy.None;
        switch (columnId) {
            case 'colBaseWinLoss':
                sortBy = self.Enums.sortBy.BaseWinLoss;
                break;
            case 'colBaseCompanyWinLoss':
                sortBy = self.Enums.sortBy.BaseCompanyWinLoss;
                break;
        }

        _params.reportParams.SortBy = sortBy;
        _params.reportParams.SortDirection = newSortDirection;

        $.ajax({
            type: "POST",
            url: window.rootUrl + "Special/ParlayWon/SortReport",
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
    };
    var exportReport = function () {
        var exportParams = {
            WinlostDate: _params.reportParams.WinlostDate,
            Bettype: _params.reportParams.Bettype,
            BettypeName: _params.reportParams.BettypeName,
            IsSoccer: _params.reportParams.IsSoccer,
            SportName: _params.reportParams.SportName,
            SortBy: _params.reportParams.SortBy,
            SortDirection: _params.reportParams.SortDirection,
            IsGetAll: true,
            ItemsPerPage: _params.reportParams.ItemsPerPage,
            TotalItem: _params.reportParams.TotalItem,
            CurrentPage: _params.reportParams.CurrentPage,
            IsPagingCallBack: false
        };

        AccReport.downloadFile(window.rootUrl + "Special/ParlayWon/ExportReport", exportParams);
    };
}