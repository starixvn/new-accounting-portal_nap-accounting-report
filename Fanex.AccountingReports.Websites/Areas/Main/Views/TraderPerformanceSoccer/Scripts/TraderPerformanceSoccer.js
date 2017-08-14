Namespace.Register("Fanex.AccountingReports.Main.TraderPerformance");

Fanex.AccountingReports.Main.TraderPerformanceSoccer = function () {
    var self = this;

    var _tables = {
        datepickerplugin: null,
        btnGenerate: null,
        btnDetail: null,
        btnSummary: null,
        btnDetailPH: null,
        btnSummaryPH: null,
        reportContainer: null,
        report: null,
        reportHeader: null,
        reportContent: null,
        reportTitle: null,
        reportExportExcel: null,
        reportRowHeader: null,
        reportRowFooter: null,
        reportColFooter: null,
        reportPagination: null,
        isMainReport: null,
        reportDivSorter: null,
        reportSorter: null,
        reportDetailContainer: null,
        reportDetail: null,
        reportDetailHeader: null,
        reportDetailContent: null,
        reportDetailTitle: null,
        reportDetailExportExcel: null,
        reportDetailRowHeader: null,
        reportDetailRowFooter: null,
        reportDetailColFooter: null,
        reportDetailPagination: null,
        reportDetailDivSorter: null,
        reportDetailSorter: null
    };

    var _options = {
        report: {
            isShowPager: false,
            tblContentId: 'soccerReportContent',
            fullWidth: true,
            fullHeight: true,
            gridHeight: 180,
            gridWidth: 710,
            freezeWidth: 420,
            freezeHeight: 56,
            column: [40, 100, 40, 40, 40, 40, 40, 40,
                100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100,
                100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100,
                100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100,
                100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100,
                100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100
            ],
            numberFreezeCol: 8
        },

        reportDetail: {
            isShowPager: true,
            tblContentId: 'soccerDetailReportContent',
            fullWidth: true,
            fullHeight: true,
            gridHeight: 180,
            gridWidth: 710,
            freezeWidth: 590,
            freezeHeight: 56,
            column: [40, 100, 40, 150, 150, 80, 60, 60,
                100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100,
                100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100,
                100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100,
                100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100,
                100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100
            ],
            numberFreezeCol: 6
        },

        paging: {
            callback: null,
            startPerPage: 30,
            stepPerPage: 30,
            maxTtemsPerPage: 500,
            itemsPerPage: 30,
            currentPage: 0,
            itemsPerPageArray: [1, 2, 3, 4, 5, 10, 20, 30, 50, 100]
        }
    };

    var _tempHtml = {
        frozenHtml: '',
        contentHtml: '',
        footerFrozenHtml: '',
        footerContentHtml: ''
    };

    var _param = {
        reportParams: {
            FromDate: null,
            ToDate: null,
            IsSoccer: null,
            SortDirection: null,
            SortColumnName: null
        },
        reportDetailParams: {
            FromDate: null,
            ToDate: null,
            IsSoccer: null,
            SortDirection: null,
            SortColumnName: null,
            PageSize: null,
            PageIndex: null,
            TotalItems: null,
            IsPagingCallBack: false
        }
    };

    self.initPage = function () {
        cachedDOMElements();
        renderDatePicker();
        bindingEvent();
        _tables.report.initGrid(_options.report);
        _tables.reportDetail.initGrid(_options.reportDetail);
    };

    var cachedDOMElements = function () {
        _tables.btnGenerate = $("#btnGenerate");
        _tables.btnDetail = $("#btnDetailReport");
        _tables.btnSummary = $("#btnSummarylReport");
        _tables.btnDetailPH = $("#btnDetailPH");
        _tables.btnSummaryPH = $("#btnSummaryPH");

        _tables.reportContainer = $("#soccerReportContainer");
        _tables.report = $("#soccerReport");
        _tables.reportHeader = $("#soccerReportHeader");
        _tables.reportContent = $("#soccerReportContent tbody");
        _tables.reportTitle = $("#soccerReportTitle");
        _tables.reportExportExcel = $("#btnExportTraderSoccerReport");
        _tables.reportRowHeader = $("#soccerReportRowHeader tbody");
        _tables.reportRowFooter = $("#soccerReport .row_footer_content thead");
        _tables.reportColFooter = $("#soccerReport .col_footer_content thead");

        _tables.reportDetailContainer = $("#soccerDetailReportContainer");
        _tables.reportDetail = $("#soccerDetailReport");
        _tables.reportDetailHeader = $("#soccerDetailReportHeader");
        _tables.reportDetailContent = $("#soccerDetailReportContent tbody");
        _tables.reportDetailTitle = $("#soccerDetailReportTitle");
        _tables.reportDetailExportExcel = $("#btnExportSoccerDetailReport");
        _tables.reportDetailRowHeader = $("#soccerDetailReportRowHeader tbody");
        _tables.reportDetailRowFooter = $("#soccerDetailReport .row_footer_content thead");
        _tables.reportDetailColFooter = $("#soccerDetailReport .col_footer_content thead");

        _tables.reportDivSorter = $("#soccerReport .divSorter");
        _tables.reportSorter = $("#soccerReport .colSorter")
        _tables.reportDetailDivSorter = $("#soccerDetailReport .divSorter");
        _tables.reportDetailSorter = $("#soccerDetailReport .colSorter");
        _tables.reportPagination = _tables.reportDetail.find("#pagination");
    };

    self.cachedData = {
        sortColumnName: null,
        sortDirection: null,
        detailReport: null,
        lastSorterBy: null,
        lastDetailReportSorterBy: null
    };

    self.SortEnums = {
        sortBy: {
            Trader: 'Trader',
            WinLossOverall: 'WinLossOverall',
            TurnOverOverall: 'TurnOverOverall',
            LiveBetCountOverall: 'LiveBetCountOverall',
            MarginOverall: 'MarginOverall',
            CommOverall: 'CommOverall',
            None: ''
        }
    };

    var bindingEvent = function () {
        _tables.btnGenerate.click(function () {
            self.resetReportDetailParams();
            _param.reportDetailParams.IsPagingCallBack = false;
            self.getTraderPerformanceSoccerReport();
        });
        _tables.btnDetail.click(function () {
            self.getTraderPerformanceSoccerDetailReport();
        });
        _tables.btnSummary.click(function () {
            self.getSummaryReport();
        });
        _tables.btnDetailPH.click(function () {
            self.exportDetailPH();
        });
        _tables.btnSummaryPH.click(function () {
            self.exportSummaryPH();
        });
        _tables.reportExportExcel.click(function () {
            self.exportData();
        });
        _tables.reportDetailExportExcel.click(function () {
            self.exportDetailData();
        });
        _tables.reportSorter.click(function () {
            AccReport.gridSorterManager.sortColumn($(this).find(".divSorter"), self.sortReportData);
        });
        _tables.reportDetailSorter.click(function () {
            AccReport.gridSorterManager.sortColumn($(this).find(".divSorter"), self.sortDetailReportData);
        });
    };

    var renderDatePicker = function () {
        $('#datepickerplugin').nexDatePicker({
            nexDatePickerType: "daterange",
            datePickerFormat: {
                changeMonth: true,
                changeYear: true,
                dateFormat: FormatConstant.M_DD_YY_STRING,
                minDate: new Date(minDate),
                maxDate: new Date(today)
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
            }
        });

        _tables.datepickerplugin = $("#datepickerplugin");
    };

    self.getParams = function () {
        var dateRange = _tables.datepickerplugin.nexDatePicker("getDateRange");
        _param.reportParams.FromDate = dateRange.FromDate;
        _param.reportParams.ToDate = dateRange.ToDate;
        _param.reportParams.IsSoccer = true;

        return _param.reportParams;
    };

    self.getReportDetailParams = function () {
        var dateRange = _tables.datepickerplugin.nexDatePicker("getDateRange");
        _param.reportDetailParams.FromDate = dateRange.FromDate;
        _param.reportDetailParams.ToDate = dateRange.ToDate;
        _param.reportDetailParams.IsSoccer = true;
        _param.reportDetailParams.PageIndex = _options.paging.currentPage + 1;
        _param.reportDetailParams.PageSize = _options.paging.itemsPerPage;

        return _param.reportDetailParams;
    };

    self.resetReportDetailParams = function () {
        _param.reportDetailParams.FromDate = null;
        _param.reportDetailParams.ToDate = null;
        _param.reportDetailParams.IsSoccer = true;
        _param.reportDetailParams.PageIndex = 1;
        _param.reportDetailParams.PageSize = 30;
        _options.paging.currentPage = 0;
        _options.paging.itemsPerPage = 30;
    };

    self.getCurrentCriteria = function () {
        var currentCriteria = {
            FromDate: null,
            ToDate: null,
            IsSoccer: null
        };
        var dateRange = _tables.datepickerplugin.nexDatePicker("getDateRange");
        currentCriteria.FromDate = dateRange.FromDate;
        currentCriteria.ToDate = dateRange.ToDate;
        currentCriteria.IsSoccer = true;

        return currentCriteria;
    };

    self.getTraderPerformanceSoccerReport = function () {

        _tables.isMainReport = true;
        AccReport.disableInput();

        var data = self.getParams();

        AccReport.disableInput();
        AccReport.utility.CallAjax(this,
                                    window.rootUrl + "Main/TraderPerformanceSoccer/GetTraderPerformanceSoccer",
                                    data,
                                    self.renderReport,
                                    self.renderReport,
                                    AccReport.enableInput);
    };

    self.getTraderPerformanceSoccerDetailReport = function () {
        _tables.isMainReport = false;
        var oldCriteria = _param.reportDetailParams;
        var currentCriteria = self.getCurrentCriteria();
        var compareCriteriaValue = self.compareCriteria(oldCriteria, currentCriteria);
        self.cachedData.sortColumnName = null;
        self.cachedData.sortDirection = null;

        if (_param.reportDetailParams.IsPagingCallBack) {
            AccReport.disableInput();
            var data = self.getReportDetailParams();
            AccReport.disableInput();
            AccReport.utility.CallAjax(this,
                                    window.rootUrl + "Main/TraderPerformanceSoccer/GetTraderPerformanceSoccerDetail",
                                    data,
                                    self.renderReport,
                                    self.renderReport,
                                    AccReport.enableInput);
        }

        if (compareCriteriaValue) {
            self.renderDetailReportFromCache();
        }
        else {
            AccReport.disableInput();
            var data = self.getReportDetailParams();
            AccReport.disableInput();
            AccReport.utility.CallAjax(this,
                                    window.rootUrl + "Main/TraderPerformanceSoccer/GetTraderPerformanceSoccerDetail",
                                    data,
                                    self.renderReport,
                                    self.renderReport,
                                    AccReport.enableInput);
        }
    };

    self.getSummaryReport = function () {
        var oldCriteria = _param.reportParams;
        var currentCriteria = self.getCurrentCriteria();
        var compareValue = self.compareCriteria(oldCriteria, currentCriteria);
        self.cachedData.sortColumnName = null;
        self.cachedData.sortDirection = null;
        _tables.isMainReport = true;
        _param.reportDetailParams.IsPagingCallBack = false;
        if (compareValue) {
            self.renderSummaryReportFromCache();
        }
        else {
            self.resetReportDetailParams();
            self.getTraderPerformanceSoccerReport();
        }
    };

    self.renderSummaryReportFromCache = function () {
        _tables.reportDetailContainer.addClass("display-none");
        _tables.reportContainer.removeClass("display-none");
        _tables.btnSummary.addClass("display-none");
        _tables.btnDetail.removeClass("display-none");
        _tables.report.removeClass("display-none").gridScroller(_options.report);
    };

    self.renderDetailReportFromCache = function () {
        _tables.reportDetailContainer.removeClass("display-none");
        _tables.reportContainer.addClass("display-none");
        _tables.btnDetail.addClass("display-none");
        _tables.btnSummary.removeClass("display-none");
        _tables.reportDetail.removeClass("display-none").gridScroller(_options.reportDetail);
    };

    self.getSortColumnName = function (columnName) {
        var sortColumnName = self.SortEnums.sortBy.None;

        switch (columnName) {
            case "Trader":
                sortColumnName = self.SortEnums.sortBy.Trader;
                break;
            case "WinLossOverall":
                sortColumnName = self.SortEnums.sortBy.WinLossOverall;
                break;
            case "TurnOverOverall":
                sortColumnName = self.SortEnums.sortBy.TurnOverOverall;
                break;
            case "LiveBetCountOverall":
                sortColumnName = self.SortEnums.sortBy.LiveBetCountOverall;
                break;
            case "MarginOverall":
                sortColumnName = self.SortEnums.sortBy.MarginOverall;
                break;
            case "CommOverall":
                sortColumnName = self.SortEnums.sortBy.CommOverall;
                break;
        };

        return sortColumnName;
    };

    self.sortReportData = function (column, sortDirection) {
        var columnName = column.attr("data-columnname");

        if (self.cachedData.lastSorterBy != null &&
            self.cachedData.lastSorterBy.attr("data-columnname") != columnName) {
            AccReport.gridSorterManager.resetColumns(self.cachedData.lastSorterBy);
        }

        self.cachedData.lastSorterBy = column;
        var sortColumnName = self.getSortColumnName(columnName);

        _param.reportParams.SortColumnName = sortColumnName;
        _param.reportParams.SortDirection = sortDirection;

        AccReport.disableInput();
        var data = _param.reportParams;
        AccReport.utility.CallAjax(this,
            window.rootUrl + "Main/TraderPerformanceSoccer/SortTraderPerformanceSoccer",
            data,
            self.renderReport,
            self.renderReport,
            AccReport.enableInput,
            false);
    };

    self.sortDetailReportData = function (column, sortDirection) {
        var columnName = column.attr("data-columnname");

        if (self.cachedData.lastDetailReportSorterBy != null &&
            self.cachedData.lastDetailReportSorterBy.attr("data-columnname") != columnName) {
            AccReport.gridSorterManager.resetColumns(self.cachedData.lastDetailReportSorterBy);
        }

        self.cachedData.lastDetailReportSorterBy = column;
        var sortColumnName = self.getSortColumnName(columnName);

        _param.reportDetailParams.SortColumnName = sortColumnName;
        _param.reportDetailParams.SortDirection = sortDirection;
        _options.paging.itemsPerPage = _param.reportDetailParams.PageSize;

        var data = _param.reportDetailParams;

        AccReport.disableInput();
        AccReport.utility.CallAjax(this,
            window.rootUrl + "Main/TraderPerformanceSoccer/SortTraderPerformanceSoccerDetail",
            data,
            self.renderReport,
            self.renderReport,
            AccReport.enableInput,
            false);
    };

    self.renderReport = function (rs) {
        if (_tables.isMainReport) {
            _tables.reportContainer.removeClass("display-none");
            _tables.reportDetailContainer.addClass("display-none");
            self.renderData(rs);
            if (_tables.btnDetail.hasClass("display-none")) {
                _tables.btnDetail.removeClass("display-none");
            }
            if (!_tables.btnSummary.hasClass("display-none")) {
                _tables.btnSummary.addClass("display-none");
            }
        }
        else {
            _tables.reportDetailContainer.removeClass("display-none");
            _tables.reportContainer.addClass("display-none");
            self.renderDetailData(rs);
            _tables.btnDetail.addClass("display-none");
            if (_tables.btnSummary.hasClass("display-none")) {
                _tables.btnSummary.removeClass("display-none");
            }
        }
    };

    self.renderData = function (rs) {
        numeral.zeroFormat('');
        var title = "Trader Performance - From " + _param.reportParams.FromDate + " To " + _param.reportParams.ToDate;

        _tables.reportTitle.html(title);

        if (rs && rs.ReportRows.length > 0) {
            _tables.reportDivSorter.addClass("divSorter");
            _tables.reportExportExcel.removeClass("display-none");

            self.resetTempHtml();

            var frozenHtml = '';
            var reportLength = rs.ReportRows.length;

            for (var index = 0; index < reportLength; index++) {
                var currentRow = rs.ReportRows[index];
                self.renderReportRow(currentRow, index + 1);
            }

            self.renderReportTotalRow(rs.GrandTotal, grandTotal);

            self.removeSpaceIE9TempHtml();
            _tables.reportRowHeader.empty().append(_tempHtml.frozenHtml);
            _tables.reportContent.empty().append(_tempHtml.contentHtml);
            _tables.reportRowFooter.empty().append(_tempHtml.footerFrozenHtml);
            _tables.reportColFooter.empty().append(_tempHtml.footerContentHtml);
        }
        else {
            self.renderReportNoData();
        }

        _tables.report.removeClass("display-none").gridScroller(_options.report);

        _tables.reportHeader.removeClass("display-none");
        _tables.report.removeClass("display-none");
    };

    self.renderDetailData = function (rs) {
        numeral.zeroFormat('');
        var title = "Trader Performance - From " + _param.reportDetailParams.FromDate + " To " + _param.reportDetailParams.ToDate;

        _tables.reportDetailTitle.html(title);

        if (rs && rs.ReportRows.length > 0) {
            _tables.reportDetailDivSorter.addClass("divSorter");
            _tables.reportDetailExportExcel.removeClass("display-none");

            self.resetTempHtml();

            var frozenHtml = '';
            var reportLength = rs.ReportRows.length;
            for (var index = 0; index < reportLength; index++) {
                var currentRow = rs.ReportRows[index];
                self.renderDetailReportRow(currentRow, index + 1);
                self.renderSubTotalDetailReport(currentRow, "subTotal");
            }

            self.renderDetailReportTotalRow(rs.GrandTotal, grandTotal);

            self.removeSpaceIE9TempHtml();
            _tables.reportDetailRowHeader.empty().append(_tempHtml.frozenHtml);
            _tables.reportDetailContent.empty().append(_tempHtml.contentHtml);
            _tables.reportDetailRowFooter.empty().append(_tempHtml.footerFrozenHtml);
            _tables.reportDetailColFooter.empty().append(_tempHtml.footerContentHtml);

            initPager({
                totalItems: rs.ReportLength,
                currentPage: _param.reportDetailParams.PageIndex - 1,
                pager: _tables.reportDetail.find("#pagination")
            }, self);
        }
        else {
            self.renderReportDetailNoData();
            initPager({
                totalItems: 0,
                currentPage: 1,
                pager: _tables.reportDetail.find("#pagination")
            }, self);
        }

        _tables.reportDetail.removeClass("display-none").gridScroller(_options.reportDetail);
        _tables.reportDetailHeader.removeClass("display-none");
        _tables.reportDetail.removeClass("display-none");
    };

    self.exportData = function () {
        var exportParams = {
            FromDate: _param.reportParams.FromDate,
            ToDate: _param.reportParams.ToDate,
            IsSoccer: true
        };

        AccReport.downloadFile(window.rootUrl + "Main/TraderPerformanceSoccer/ExportReport", exportParams);
    };

    self.exportDetailData = function () {
        var currentCriteria = self.getCurrentCriteria();        
        var exportParams = {
            FromDate: currentCriteria.FromDate,
            ToDate: currentCriteria.ToDate,
            IsSoccer: true
        };

        AccReport.downloadFile(window.rootUrl + "Main/TraderPerformanceSoccer/ExportDetailReport", exportParams);
    };

    self.exportSummaryPH = function () {
        var currentCriteria = self.getCurrentCriteria();
        var exportParams = {
            FromDate: currentCriteria.FromDate,
            ToDate: currentCriteria.ToDate,
            IsSoccer: true
        };

        AccReport.downloadFile(window.rootUrl + "Main/TraderPerformanceSoccer/ExportSummaryPH", exportParams);
    };

    self.exportDetailPH = function () {
        var currentCriteria = self.getCurrentCriteria();
        var exportParams = {
            FromDate: currentCriteria.FromDate,
            ToDate: currentCriteria.ToDate,
            IsSoccer: true
        };

        AccReport.downloadFile(window.rootUrl + "Main/TraderPerformanceSoccer/ExportDetailPH", exportParams);
    };

    self.compareCriteria = function (oldCriteria, currentCriteria) {
        if (oldCriteria.FromDate != currentCriteria.FromDate
            || oldCriteria.ToDate != currentCriteria.ToDate
            || oldCriteria.IsSoccer != currentCriteria.IsSoccer) {
            return false;
        }
        else if (oldCriteria.FromDate == currentCriteria.FromDate
            || oldCriteria.ToDate == currentCriteria.ToDate
            || oldCriteria.IsSoccer == currentCriteria.IsSoccer) {
            return true;
        }
    };

    self.renderPercentCustomCell = function (cellValue) {
        var cellStringValue = null;

        if (cellValue == 0) {
            cellStringValue = '';
        }
        else if (cellValue > 0) {
            cellStringValue = numeral(cellValue).format("0,0.00") + '%';
        }
        else {
            cellStringValue = '<span class="format-negative">(' + numeral(cellValue).format("0,0.00") + '%)</span>';
        }

        return cellStringValue;
    };

    self.renderIntNumberCustomCell = function (cellValue) {
        var cellStringValue = null;

        if (cellValue == 0) {
            cellStringValue = '';
        }
        else {
            cellStringValue = numeral(cellValue).format(FormatConstant.INT_NUMBER);
        }

        return cellStringValue;
    };

    self.renderDecimalNumberCustomCell = function (cellValue) {
        var cellStringValue = null;

        if (cellValue == 0) {
            cellStringValue = '';
        }
        else if (cellValue > 0) {
            cellStringValue = numeral(cellValue).format("0,0.00");
        }
        else {
            cellStringValue = '<span class="format-negative">(' + numeral(cellValue).format("0,0.00") + ')</span>';
        }

        return cellStringValue;
    };

    self.renderReportRow = function (row, index) {
        var frozenHtml = '';
        frozenHtml = '<td><div class="col40 align-center">' + index + '</div></td>\
                    <td><div class="col100 align-center">' + row.Name + '</div></td>\
                    <td><div class="col40 align-right">' + numeral(row.MatchNumber).format(FormatConstant.INT_NUMBER) + '</div></td>\
                    <td><div class="col40 align-right">' + numeral(row.Major).format(FormatConstant.INT_NUMBER) + '</div></td>\
                    <td><div class="col40 align-right">' + numeral(row.Minor1).format(FormatConstant.INT_NUMBER) + '</div></td>\
                    <td><div class="col40 align-right">' + numeral(row.Minor2).format(FormatConstant.INT_NUMBER) + '</div></td>\
                    <td><div class="col40 align-right">' + numeral(row.Minor3).format(FormatConstant.INT_NUMBER) + '</div></td>\
                    <td><div class="col40 align-right">' + numeral(row.Others).format(FormatConstant.INT_NUMBER) + '</div></td>';
        _tempHtml.frozenHtml += '<tr>' + frozenHtml + ' </tr>';
        _tempHtml.contentHtml += '<tr>' +
                                        frozenHtml +
                                        '<td class="winLossGroup"><div class="col100 align-right">' + self.renderDecimalNumberCustomCell(row.ChildInfo[0].FirstHalfOneXTwo) + '</div></td>\
                                        <td class="winLossGroup"><div class="col100 align-right">' + self.renderDecimalNumberCustomCell(row.ChildInfo[0].OneXTwo) + '</div></td>\
                                        <td class="winLossGroup"><div class="col100 align-right">' + self.renderDecimalNumberCustomCell(row.ChildInfo[0].FifteenMinOneXTwo) + '</div></td>\
                                        <td class="winLossGroup"><div class="col100 align-right">' + self.renderDecimalNumberCustomCell(row.ChildInfo[0].TenMinOneXTwo) + '</div></td>\
                                        <td class="winLossGroup"><div class="col100 align-right">' + self.renderDecimalNumberCustomCell(row.ChildInfo[0].FirstHalfHdp) + '</div></td>\
                                        <td class="winLossGroup"><div class="col100 align-right">' + self.renderDecimalNumberCustomCell(row.ChildInfo[0].Hdp) + '</div></td>\
                                        <td class="winLossGroup"><div class="col100 align-right">' + self.renderDecimalNumberCustomCell(row.ChildInfo[0].FifteenMinHdp) + '</div></td>\
                                        <td class="winLossGroup"><div class="col100 align-right">' + self.renderDecimalNumberCustomCell(row.ChildInfo[0].TenMinHdp) + '</div></td>\
                                        <td class="winLossGroup"><div class="col100 align-right">' + self.renderDecimalNumberCustomCell(row.ChildInfo[0].FirstHalfOU) + '</div></td>\
                                        <td class="winLossGroup"><div class="col100 align-right">' + self.renderDecimalNumberCustomCell(row.ChildInfo[0].OU) + '</div></td>\
                                        <td class="winLossGroup"><div class="col100 align-right">' + self.renderDecimalNumberCustomCell(row.ChildInfo[0].FifteenMinOU) + '</div></td>\
                                        <td class="winLossGroup"><div class="col100 align-right">' + self.renderDecimalNumberCustomCell(row.ChildInfo[0].TenMinOU) + '</div></td>\
                                        <td class="winLossGroup"><div class="col100 align-right">' + self.renderDecimalNumberCustomCell(row.ChildInfo[0].SingleTeamOU) + '</div></td>\
                                        <td class="winLossGroup"><div class="col100 align-right">' + self.renderDecimalNumberCustomCell(row.ChildInfo[0].FirstHalfNoOfCorners) + '</div></td>\
                                        <td class="winLossGroup"><div class="col100 align-right">' + self.renderDecimalNumberCustomCell(row.ChildInfo[0].NoOfCorners) + '</div></td>\
                                        <td class="winLossGroup"><div class="col100 align-right">' + self.renderDecimalNumberCustomCell(row.ChildInfo[0].FifteenMinNoOfCorners) + '</div></td>\
                                        <td class="winLossGroup"><div class="col100 align-right">' + self.renderDecimalNumberCustomCell(row.ChildInfo[0].NextCorner) + '</div></td>\
                                        <td class="winLossGroup"><div class="col100 align-right">' + self.renderDecimalNumberCustomCell(row.ChildInfo[0].FirstHalfTotalBookings) + '</div></td>\
                                        <td class="winLossGroup"><div class="col100 align-right">' + self.renderDecimalNumberCustomCell(row.ChildInfo[0].TotalBookings) + '</div></td>\
                                        <td class="winLossGroup"><div class="col100 align-right">' + self.renderDecimalNumberCustomCell(row.ChildInfo[0].TotalBookingsFifteenMinOU) + '</div></td>\
                                        <td class="winLossGroup"><div class="col100 align-right">' + self.renderDecimalNumberCustomCell(row.ChildInfo[0].NextBookings) + '</div></td>\
                                        <td class="winLossGroup"><div class="col100 align-right">' + self.renderDecimalNumberCustomCell(row.ChildInfo[0].FirstHalfOE) + '</div></td>\
                                        <td class="winLossGroup"><div class="col100 align-right">' + self.renderDecimalNumberCustomCell(row.ChildInfo[0].OE) + '</div></td>\
                                        <td class="winLossGroup"><div class="col100 align-right">' + self.renderDecimalNumberCustomCell(row.ChildInfo[0].Overall) + '</div></td>\
                                        <td class="turnOverGroup"><div class="col100 align-right">' + self.renderDecimalNumberCustomCell(row.ChildInfo[1].FirstHalfOneXTwo) + '</div></td>\
                                        <td class="turnOverGroup"><div class="col100 align-right">' + self.renderDecimalNumberCustomCell(row.ChildInfo[1].OneXTwo) + '</div></td>\
                                        <td class="turnOverGroup"><div class="col100 align-right">' + self.renderDecimalNumberCustomCell(row.ChildInfo[1].FifteenMinOneXTwo) + '</div></td>\
                                        <td class="turnOverGroup"><div class="col100 align-right">' + self.renderDecimalNumberCustomCell(row.ChildInfo[1].TenMinOneXTwo) + '</div></td>\
                                        <td class="turnOverGroup"><div class="col100 align-right">' + self.renderDecimalNumberCustomCell(row.ChildInfo[1].FirstHalfHdp) + '</div></td>\
                                        <td class="turnOverGroup"><div class="col100 align-right">' + self.renderDecimalNumberCustomCell(row.ChildInfo[1].Hdp) + '</div></td>\
                                        <td class="turnOverGroup"><div class="col100 align-right">' + self.renderDecimalNumberCustomCell(row.ChildInfo[1].FifteenMinHdp) + '</div></td>\
                                        <td class="turnOverGroup"><div class="col100 align-right">' + self.renderDecimalNumberCustomCell(row.ChildInfo[1].TenMinHdp) + '</div></td>\
                                        <td class="turnOverGroup"><div class="col100 align-right">' + self.renderDecimalNumberCustomCell(row.ChildInfo[1].FirstHalfOU) + '</div></td>\
                                        <td class="turnOverGroup"><div class="col100 align-right">' + self.renderDecimalNumberCustomCell(row.ChildInfo[1].OU) + '</div></td>\
                                        <td class="turnOverGroup"><div class="col100 align-right">' + self.renderDecimalNumberCustomCell(row.ChildInfo[1].FifteenMinOU) + '</div></td>\
                                        <td class="turnOverGroup"><div class="col100 align-right">' + self.renderDecimalNumberCustomCell(row.ChildInfo[1].TenMinOU) + '</div></td>\
                                        <td class="turnOverGroup"><div class="col100 align-right">' + self.renderDecimalNumberCustomCell(row.ChildInfo[1].SingleTeamOU) + '</div></td>\
                                        <td class="turnOverGroup"><div class="col100 align-right">' + self.renderDecimalNumberCustomCell(row.ChildInfo[1].FirstHalfNoOfCorners) + '</div></td>\
                                        <td class="turnOverGroup"><div class="col100 align-right">' + self.renderDecimalNumberCustomCell(row.ChildInfo[1].NoOfCorners) + '</div></td>\
                                        <td class="turnOverGroup"><div class="col100 align-right">' + self.renderDecimalNumberCustomCell(row.ChildInfo[1].FifteenMinNoOfCorners) + '</div></td>\
                                        <td class="turnOverGroup"><div class="col100 align-right">' + self.renderDecimalNumberCustomCell(row.ChildInfo[1].NextCorner) + '</div></td>\
                                        <td class="turnOverGroup"><div class="col100 align-right">' + self.renderDecimalNumberCustomCell(row.ChildInfo[1].FirstHalfTotalBookings) + '</div></td>\
                                        <td class="turnOverGroup"><div class="col100 align-right">' + self.renderDecimalNumberCustomCell(row.ChildInfo[1].TotalBookings) + '</div></td>\
                                        <td class="turnOverGroup"><div class="col100 align-right">' + self.renderDecimalNumberCustomCell(row.ChildInfo[1].TotalBookingsFifteenMinOU) + '</div></td>\
                                        <td class="turnOverGroup"><div class="col100 align-right">' + self.renderDecimalNumberCustomCell(row.ChildInfo[1].NextBookings) + '</div></td>\
                                        <td class="turnOverGroup"><div class="col100 align-right">' + self.renderDecimalNumberCustomCell(row.ChildInfo[1].FirstHalfOE) + '</div></td>\
                                        <td class="turnOverGroup"><div class="col100 align-right">' + self.renderDecimalNumberCustomCell(row.ChildInfo[1].OE) + '</div></td>\
                                        <td class="turnOverGroup"><div class="col100 align-right">' + self.renderDecimalNumberCustomCell(row.ChildInfo[1].Overall) + '</div></td>\
                                        <td class="liveBetCountGroup"><div class="col100 align-right">' + numeral(row.ChildInfo[2].FirstHalfOneXTwo).format(FormatConstant.INT_NUMBER) + '</div></td>\
                                        <td class="liveBetCountGroup"><div class="col100 align-right">' + numeral(row.ChildInfo[2].OneXTwo).format(FormatConstant.INT_NUMBER) + '</div></td>\
                                        <td class="liveBetCountGroup"><div class="col100 align-right">' + numeral(row.ChildInfo[2].FifteenMinOneXTwo).format(FormatConstant.INT_NUMBER) + '</div></td>\
                                        <td class="liveBetCountGroup"><div class="col100 align-right">' + numeral(row.ChildInfo[2].TenMinOneXTwo).format(FormatConstant.INT_NUMBER) + '</div></td>\
                                        <td class="liveBetCountGroup"><div class="col100 align-right">' + numeral(row.ChildInfo[2].FirstHalfHdp).format(FormatConstant.INT_NUMBER) + '</div></td>\
                                        <td class="liveBetCountGroup"><div class="col100 align-right">' + numeral(row.ChildInfo[2].Hdp).format(FormatConstant.INT_NUMBER) + '</div></td>\
                                        <td class="liveBetCountGroup"><div class="col100 align-right">' + numeral(row.ChildInfo[2].FifteenMinHdp).format(FormatConstant.INT_NUMBER) + '</div></td>\
                                        <td class="liveBetCountGroup"><div class="col100 align-right">' + numeral(row.ChildInfo[2].TenMinHdp).format(FormatConstant.INT_NUMBER) + '</div></td>\
                                        <td class="liveBetCountGroup"><div class="col100 align-right">' + numeral(row.ChildInfo[2].FirstHalfOU).format(FormatConstant.INT_NUMBER) + '</div></td>\
                                        <td class="liveBetCountGroup"><div class="col100 align-right">' + numeral(row.ChildInfo[2].OU).format(FormatConstant.INT_NUMBER) + '</div></td>\
                                        <td class="liveBetCountGroup"><div class="col100 align-right">' + numeral(row.ChildInfo[2].FifteenMinOU).format(FormatConstant.INT_NUMBER) + '</div></td>\
                                        <td class="liveBetCountGroup"><div class="col100 align-right">' + numeral(row.ChildInfo[2].TenMinOU).format(FormatConstant.INT_NUMBER) + '</div></td>\
                                        <td class="liveBetCountGroup"><div class="col100 align-right">' + numeral(row.ChildInfo[2].SingleTeamOU).format(FormatConstant.INT_NUMBER) + '</div></td>\
                                        <td class="liveBetCountGroup"><div class="col100 align-right">' + numeral(row.ChildInfo[2].FirstHalfNoOfCorners).format(FormatConstant.INT_NUMBER) + '</div></td>\
                                        <td class="liveBetCountGroup"><div class="col100 align-right">' + numeral(row.ChildInfo[2].NoOfCorners).format(FormatConstant.INT_NUMBER) + '</div></td>\
                                        <td class="liveBetCountGroup"><div class="col100 align-right">' + numeral(row.ChildInfo[2].FifteenMinNoOfCorners).format(FormatConstant.INT_NUMBER) + '</div></td>\
                                        <td class="liveBetCountGroup"><div class="col100 align-right">' + numeral(row.ChildInfo[2].NextCorner).format(FormatConstant.INT_NUMBER) + '</div></td>\
                                        <td class="liveBetCountGroup"><div class="col100 align-right">' + numeral(row.ChildInfo[2].FirstHalfTotalBookings).format(FormatConstant.INT_NUMBER) + '</div></td>\
                                        <td class="liveBetCountGroup"><div class="col100 align-right">' + numeral(row.ChildInfo[2].TotalBookings).format(FormatConstant.INT_NUMBER) + '</div></td>\
                                        <td class="liveBetCountGroup"><div class="col100 align-right">' + numeral(row.ChildInfo[2].TotalBookingsFifteenMinOU).format(FormatConstant.INT_NUMBER) + '</div></td>\
                                        <td class="liveBetCountGroup"><div class="col100 align-right">' + numeral(row.ChildInfo[2].NextBookings).format(FormatConstant.INT_NUMBER) + '</div></td>\
                                        <td class="liveBetCountGroup"><div class="col100 align-right">' + numeral(row.ChildInfo[2].FirstHalfOE).format(FormatConstant.INT_NUMBER) + '</div></td>\
                                        <td class="liveBetCountGroup"><div class="col100 align-right">' + numeral(row.ChildInfo[2].OE).format(FormatConstant.INT_NUMBER) + '</div></td>\
                                        <td class="liveBetCountGroup"><div class="col100 align-right">' + numeral(row.ChildInfo[2].Overall).format(FormatConstant.INT_NUMBER) + '</div></td>\
                                        <td class="marginGroup"><div class="col100 align-right">' + self.renderPercentCustomCell(row.ChildInfo[3].FirstHalfOneXTwo) + '</div></td>\
                                        <td class="marginGroup"><div class="col100 align-right">' + self.renderPercentCustomCell(row.ChildInfo[3].OneXTwo) + '</div></td>\
                                        <td class="marginGroup"><div class="col100 align-right">' + self.renderPercentCustomCell(row.ChildInfo[3].FifteenMinOneXTwo) + '</div></td>\
                                        <td class="marginGroup"><div class="col100 align-right">' + self.renderPercentCustomCell(row.ChildInfo[3].TenMinOneXTwo) + '</div></td>\
                                        <td class="marginGroup"><div class="col100 align-right">' + self.renderPercentCustomCell(row.ChildInfo[3].FirstHalfHdp) + '</div></td>\
                                        <td class="marginGroup"><div class="col100 align-right">' + self.renderPercentCustomCell(row.ChildInfo[3].Hdp) + '</div></td>\
                                        <td class="marginGroup"><div class="col100 align-right">' + self.renderPercentCustomCell(row.ChildInfo[3].FifteenMinHdp) + '</div></td>\
                                        <td class="marginGroup"><div class="col100 align-right">' + self.renderPercentCustomCell(row.ChildInfo[3].TenMinHdp) + '</div></td>\
                                        <td class="marginGroup"><div class="col100 align-right">' + self.renderPercentCustomCell(row.ChildInfo[3].FirstHalfOU) + '</div></td>\
                                        <td class="marginGroup"><div class="col100 align-right">' + self.renderPercentCustomCell(row.ChildInfo[3].OU) + '</div></td>\
                                        <td class="marginGroup"><div class="col100 align-right">' + self.renderPercentCustomCell(row.ChildInfo[3].FifteenMinOU) + '</div></td>\
                                        <td class="marginGroup"><div class="col100 align-right">' + self.renderPercentCustomCell(row.ChildInfo[3].TenMinOU) + '</div></td>\
                                        <td class="marginGroup"><div class="col100 align-right">' + self.renderPercentCustomCell(row.ChildInfo[3].SingleTeamOU) + '</div></td>\
                                        <td class="marginGroup"><div class="col100 align-right">' + self.renderPercentCustomCell(row.ChildInfo[3].FirstHalfNoOfCorners) + '</div></td>\
                                        <td class="marginGroup"><div class="col100 align-right">' + self.renderPercentCustomCell(row.ChildInfo[3].NoOfCorners) + '</div></td>\
                                        <td class="marginGroup"><div class="col100 align-right">' + self.renderPercentCustomCell(row.ChildInfo[3].FifteenMinNoOfCorners) + '</div></td>\
                                        <td class="marginGroup"><div class="col100 align-right">' + self.renderPercentCustomCell(row.ChildInfo[3].NextCorner) + '</div></td>\
                                        <td class="marginGroup"><div class="col100 align-right">' + self.renderPercentCustomCell(row.ChildInfo[3].FirstHalfTotalBookings) + '</div></td>\
                                        <td class="marginGroup"><div class="col100 align-right">' + self.renderPercentCustomCell(row.ChildInfo[3].TotalBookings) + '</div></td>\
                                        <td class="marginGroup"><div class="col100 align-right">' + self.renderPercentCustomCell(row.ChildInfo[3].TotalBookingsFifteenMinOU) + '</div></td>\
                                        <td class="marginGroup"><div class="col100 align-right">' + self.renderPercentCustomCell(row.ChildInfo[3].NextBookings) + '</div></td>\
                                        <td class="marginGroup"><div class="col100 align-right">' + self.renderPercentCustomCell(row.ChildInfo[3].FirstHalfOE) + '</div></td>\
                                        <td class="marginGroup"><div class="col100 align-right">' + self.renderPercentCustomCell(row.ChildInfo[3].OE) + '</div></td>\
                                        <td class="marginGroup"><div class="col100 align-right">' + self.renderPercentCustomCell(row.ChildInfo[3].Overall) + '</div></td>\
                                        <td class="commissionGroup"><div class="col100 align-right">' + self.renderDecimalNumberCustomCell(row.ChildInfo[4].FirstHalfOneXTwo) + '</div></td>\
                                        <td class="commissionGroup"><div class="col100 align-right">' + self.renderDecimalNumberCustomCell(row.ChildInfo[4].OneXTwo) + '</div></td>\
                                        <td class="commissionGroup"><div class="col100 align-right">' + self.renderDecimalNumberCustomCell(row.ChildInfo[4].FifteenMinOneXTwo) + '</div></td>\
                                        <td class="commissionGroup"><div class="col100 align-right">' + self.renderDecimalNumberCustomCell(row.ChildInfo[4].TenMinOneXTwo) + '</div></td>\
                                        <td class="commissionGroup"><div class="col100 align-right">' + self.renderDecimalNumberCustomCell(row.ChildInfo[4].FirstHalfHdp) + '</div></td>\
                                        <td class="commissionGroup"><div class="col100 align-right">' + self.renderDecimalNumberCustomCell(row.ChildInfo[4].Hdp) + '</div></td>\
                                        <td class="commissionGroup"><div class="col100 align-right">' + self.renderDecimalNumberCustomCell(row.ChildInfo[4].FifteenMinHdp) + '</div></td>\
                                        <td class="commissionGroup"><div class="col100 align-right">' + self.renderDecimalNumberCustomCell(row.ChildInfo[4].TenMinHdp) + '</div></td>\
                                        <td class="commissionGroup"><div class="col100 align-right">' + self.renderDecimalNumberCustomCell(row.ChildInfo[4].FirstHalfOU) + '</div></td>\
                                        <td class="commissionGroup"><div class="col100 align-right">' + self.renderDecimalNumberCustomCell(row.ChildInfo[4].OU) + '</div></td>\
                                        <td class="commissionGroup"><div class="col100 align-right">' + self.renderDecimalNumberCustomCell(row.ChildInfo[4].FifteenMinOU) + '</div></td>\
                                        <td class="commissionGroup"><div class="col100 align-right">' + self.renderDecimalNumberCustomCell(row.ChildInfo[4].TenMinOU) + '</div></td>\
                                        <td class="commissionGroup"><div class="col100 align-right">' + self.renderDecimalNumberCustomCell(row.ChildInfo[4].SingleTeamOU) + '</div></td>\
                                        <td class="commissionGroup"><div class="col100 align-right">' + self.renderDecimalNumberCustomCell(row.ChildInfo[4].FirstHalfNoOfCorners) + '</div></td>\
                                        <td class="commissionGroup"><div class="col100 align-right">' + self.renderDecimalNumberCustomCell(row.ChildInfo[4].NoOfCorners) + '</div></td>\
                                        <td class="commissionGroup"><div class="col100 align-right">' + self.renderDecimalNumberCustomCell(row.ChildInfo[4].FifteenMinNoOfCorners) + '</div></td>\
                                        <td class="commissionGroup"><div class="col100 align-right">' + self.renderDecimalNumberCustomCell(row.ChildInfo[4].NextCorner) + '</div></td>\
                                        <td class="commissionGroup"><div class="col100 align-right">' + self.renderDecimalNumberCustomCell(row.ChildInfo[4].FirstHalfTotalBookings) + '</div></td>\
                                        <td class="commissionGroup"><div class="col100 align-right">' + self.renderDecimalNumberCustomCell(row.ChildInfo[4].TotalBookings) + '</div></td>\
                                        <td class="commissionGroup"><div class="col100 align-right">' + self.renderDecimalNumberCustomCell(row.ChildInfo[4].TotalBookingsFifteenMinOU) + '</div></td>\
                                        <td class="commissionGroup"><div class="col100 align-right">' + self.renderDecimalNumberCustomCell(row.ChildInfo[4].NextBookings) + '</div></td>\
                                        <td class="commissionGroup"><div class="col100 align-right">' + self.renderDecimalNumberCustomCell(row.ChildInfo[4].FirstHalfOE) + '</div></td>\
                                        <td class="commissionGroup"><div class="col100 align-right">' + self.renderDecimalNumberCustomCell(row.ChildInfo[4].OE) + '</div></td>\
                                        <td class="commissionGroup"><div class="col100 align-right">' + self.renderDecimalNumberCustomCell(row.ChildInfo[4].Overall) + '</div></td>\
                                    </tr>';
    };

    self.renderReportTotalRow = function (row, totalTypeTitle) {
        var frozenHtml = '';
        frozenHtml = '<th colspan="2"><div class="col145 align-right">' + totalTypeTitle + '</div></th>\
                      <th><div class="col40 align-right">' + numeral(row.MatchNumber).format(FormatConstant.INT_NUMBER) + '</div></th>\
                      <th><div class="col40 align-right">' + self.renderIntNumberCustomCell(row.Major) + '</div></th>\
                      <th><div class="col40 align-right">' + self.renderIntNumberCustomCell(row.Minor1) + '</div></th>\
                      <th><div class="col40 align-right">' + self.renderIntNumberCustomCell(row.Minor2) + '</div></th>\
                      <th><div class="col40 align-right">' + self.renderIntNumberCustomCell(row.Minor3) + '</div></th>\
                      <th><div class="col40 align-right">' + self.renderIntNumberCustomCell(row.Others) + '</div></th>';
        _tempHtml.footerFrozenHtml += '<tr>' + frozenHtml + ' </tr>';

        _tempHtml.footerContentHtml += '<tr>' +
                                            frozenHtml +
                                        '<th><div class="col100 align-right">' + self.renderDecimalNumberCustomCell(row.ChildInfo[0].FirstHalfOneXTwo) + '</div></td>\
                                        <th><div class="col100 align-right">' + self.renderDecimalNumberCustomCell(row.ChildInfo[0].OneXTwo) + '</div></td>\
                                        <th><div class="col100 align-right">' + self.renderDecimalNumberCustomCell(row.ChildInfo[0].FifteenMinOneXTwo) + '</div></td>\
                                        <th><div class="col100 align-right">' + self.renderDecimalNumberCustomCell(row.ChildInfo[0].TenMinOneXTwo) + '</div></td>\
                                        <th><div class="col100 align-right">' + self.renderDecimalNumberCustomCell(row.ChildInfo[0].FirstHalfHdp) + '</div></td>\
                                        <th><div class="col100 align-right">' + self.renderDecimalNumberCustomCell(row.ChildInfo[0].Hdp) + '</div></td>\
                                        <th><div class="col100 align-right">' + self.renderDecimalNumberCustomCell(row.ChildInfo[0].FifteenMinHdp) + '</div></td>\
                                        <th><div class="col100 align-right">' + self.renderDecimalNumberCustomCell(row.ChildInfo[0].TenMinHdp) + '</div></td>\
                                        <th><div class="col100 align-right">' + self.renderDecimalNumberCustomCell(row.ChildInfo[0].FirstHalfOU) + '</div></th>\
                                        <th><div class="col100 align-right">' + self.renderDecimalNumberCustomCell(row.ChildInfo[0].OU) + '</div></th>\
                                        <th><div class="col100 align-right">' + self.renderDecimalNumberCustomCell(row.ChildInfo[0].FifteenMinOU) + '</div></th>\
                                        <th><div class="col100 align-right">' + self.renderDecimalNumberCustomCell(row.ChildInfo[0].TenMinOU) + '</div></th>\
                                        <th><div class="col100 align-right">' + self.renderDecimalNumberCustomCell(row.ChildInfo[0].SingleTeamOU) + '</div></th>\
                                        <th><div class="col100 align-right">' + self.renderDecimalNumberCustomCell(row.ChildInfo[0].FirstHalfNoOfCorners) + '</div></th>\
                                        <th><div class="col100 align-right">' + self.renderDecimalNumberCustomCell(row.ChildInfo[0].NoOfCorners) + '</div></th>\
                                        <th><div class="col100 align-right">' + self.renderDecimalNumberCustomCell(row.ChildInfo[0].FifteenMinNoOfCorners) + '</div></th>\
                                        <th><div class="col100 align-right">' + self.renderDecimalNumberCustomCell(row.ChildInfo[0].NextCorner) + '</div></th>\
                                        <th><div class="col100 align-right">' + self.renderDecimalNumberCustomCell(row.ChildInfo[0].FirstHalfTotalBookings) + '</div></th>\
                                        <th><div class="col100 align-right">' + self.renderDecimalNumberCustomCell(row.ChildInfo[0].TotalBookings) + '</div></th>\
                                        <th><div class="col100 align-right">' + self.renderDecimalNumberCustomCell(row.ChildInfo[0].TotalBookingsFifteenMinOU) + '</div></th>\
                                        <th><div class="col100 align-right">' + self.renderDecimalNumberCustomCell(row.ChildInfo[0].NextBookings) + '</div></th>\
                                        <th><div class="col100 align-right">' + self.renderDecimalNumberCustomCell(row.ChildInfo[0].FirstHalfOE) + '</div></th>\
                                        <th><div class="col100 align-right">' + self.renderDecimalNumberCustomCell(row.ChildInfo[0].OE) + '</div></th>\
                                        <th><div class="col100 align-right">' + self.renderDecimalNumberCustomCell(row.ChildInfo[0].Overall) + '</div></th>\
                                        <th><div class="col100 align-right">' + self.renderDecimalNumberCustomCell(row.ChildInfo[1].FirstHalfOneXTwo) + '</div></th>\
                                        <th><div class="col100 align-right">' + self.renderDecimalNumberCustomCell(row.ChildInfo[1].OneXTwo) + '</div></th>\
                                        <th><div class="col100 align-right">' + self.renderDecimalNumberCustomCell(row.ChildInfo[1].FifteenMinOneXTwo) + '</div></th>\
                                        <th><div class="col100 align-right">' + self.renderDecimalNumberCustomCell(row.ChildInfo[1].TenMinOneXTwo) + '</div></th>\
                                        <th><div class="col100 align-right">' + self.renderDecimalNumberCustomCell(row.ChildInfo[1].FirstHalfHdp) + '</div></th>\
                                        <th><div class="col100 align-right">' + self.renderDecimalNumberCustomCell(row.ChildInfo[1].Hdp) + '</div></th>\
                                        <th><div class="col100 align-right">' + self.renderDecimalNumberCustomCell(row.ChildInfo[1].FifteenMinHdp) + '</div></th>\
                                        <th><div class="col100 align-right">' + self.renderDecimalNumberCustomCell(row.ChildInfo[1].TenMinHdp) + '</div></th>\
                                        <th><div class="col100 align-right">' + self.renderDecimalNumberCustomCell(row.ChildInfo[1].FirstHalfOU) + '</div></th>\
                                        <th><div class="col100 align-right">' + self.renderDecimalNumberCustomCell(row.ChildInfo[1].OU) + '</div></th>\
                                        <th><div class="col100 align-right">' + self.renderDecimalNumberCustomCell(row.ChildInfo[1].FifteenMinOU) + '</div></th>\
                                        <th><div class="col100 align-right">' + self.renderDecimalNumberCustomCell(row.ChildInfo[1].TenMinOU) + '</div></th>\
                                        <th><div class="col100 align-right">' + self.renderDecimalNumberCustomCell(row.ChildInfo[1].SingleTeamOU) + '</div></th>\
                                        <th><div class="col100 align-right">' + self.renderDecimalNumberCustomCell(row.ChildInfo[1].FirstHalfNoOfCorners) + '</div></th>\
                                        <th><div class="col100 align-right">' + self.renderDecimalNumberCustomCell(row.ChildInfo[1].NoOfCorners) + '</div></th>\
                                        <th><div class="col100 align-right">' + self.renderDecimalNumberCustomCell(row.ChildInfo[1].FifteenMinNoOfCorners) + '</div></th>\
                                        <th><div class="col100 align-right">' + self.renderDecimalNumberCustomCell(row.ChildInfo[1].NextCorner) + '</div></th>\
                                        <th><div class="col100 align-right">' + self.renderDecimalNumberCustomCell(row.ChildInfo[1].FirstHalfTotalBookings) + '</div></th>\
                                        <th><div class="col100 align-right">' + self.renderDecimalNumberCustomCell(row.ChildInfo[1].TotalBookings) + '</div></th>\
                                        <th><div class="col100 align-right">' + self.renderDecimalNumberCustomCell(row.ChildInfo[1].TotalBookingsFifteenMinOU) + '</div></th>\
                                        <th><div class="col100 align-right">' + self.renderDecimalNumberCustomCell(row.ChildInfo[1].NextBookings) + '</div></th>\
                                        <th><div class="col100 align-right">' + self.renderDecimalNumberCustomCell(row.ChildInfo[1].FirstHalfOE) + '</div></th>\
                                        <th><div class="col100 align-right">' + self.renderDecimalNumberCustomCell(row.ChildInfo[1].OE) + '</div></th>\
                                        <th><div class="col100 align-right">' + self.renderDecimalNumberCustomCell(row.ChildInfo[1].Overall) + '</div></th>\
                                        <th><div class="col100 align-right">' + numeral(row.ChildInfo[2].FirstHalfOneXTwo).format(FormatConstant.INT_NUMBER) + '</div></th>\
                                        <th><div class="col100 align-right">' + numeral(row.ChildInfo[2].OneXTwo).format(FormatConstant.INT_NUMBER) + '</div></th>\
                                        <th><div class="col100 align-right">' + numeral(row.ChildInfo[2].FifteenMinOneXTwo).format(FormatConstant.INT_NUMBER) + '</div></th>\
                                        <th><div class="col100 align-right">' + numeral(row.ChildInfo[2].TenMinOneXTwo).format(FormatConstant.INT_NUMBER) + '</div></th>\
                                        <th><div class="col100 align-right">' + numeral(row.ChildInfo[2].FirstHalfHdp).format(FormatConstant.INT_NUMBER) + '</div></th>\
                                        <th><div class="col100 align-right">' + numeral(row.ChildInfo[2].Hdp).format(FormatConstant.INT_NUMBER) + '</div></th>\
                                        <th><div class="col100 align-right">' + numeral(row.ChildInfo[2].FifteenMinHdp).format(FormatConstant.INT_NUMBER) + '</div></th>\
                                        <th><div class="col100 align-right">' + numeral(row.ChildInfo[2].TenMinHdp).format(FormatConstant.INT_NUMBER) + '</div></th>\
                                        <th class="turnOverGroup"><div class="col100 align-right">' + numeral(row.ChildInfo[2].FirstHalfOU).format(FormatConstant.INT_NUMBER) + '</div></th>\
                                        <th><div class="col100 align-right">' + numeral(row.ChildInfo[2].OU).format(FormatConstant.INT_NUMBER) + '</div></th>\
                                        <th><div class="col100 align-right">' + numeral(row.ChildInfo[2].FifteenMinOU).format(FormatConstant.INT_NUMBER) + '</div></th>\
                                        <th><div class="col100 align-right">' + numeral(row.ChildInfo[2].TenMinOU).format(FormatConstant.INT_NUMBER) + '</div></th>\
                                        <th><div class="col100 align-right">' + numeral(row.ChildInfo[2].SingleTeamOU).format(FormatConstant.INT_NUMBER) + '</div></th>\
                                        <th><div class="col100 align-right">' + numeral(row.ChildInfo[2].FirstHalfNoOfCorners).format(FormatConstant.INT_NUMBER) + '</div></th>\
                                        <th><div class="col100 align-right">' + numeral(row.ChildInfo[2].NoOfCorners).format(FormatConstant.INT_NUMBER) + '</div></th>\
                                        <th><div class="col100 align-right">' + numeral(row.ChildInfo[2].FifteenMinNoOfCorners).format(FormatConstant.INT_NUMBER) + '</div></th>\
                                        <th><div class="col100 align-right">' + numeral(row.ChildInfo[2].NextCorner).format(FormatConstant.INT_NUMBER) + '</div></th>\
                                        <th><div class="col100 align-right">' + numeral(row.ChildInfo[2].FirstHalfTotalBookings).format(FormatConstant.INT_NUMBER) + '</div></th>\
                                        <th><div class="col100 align-right">' + numeral(row.ChildInfo[2].TotalBookings).format(FormatConstant.INT_NUMBER) + '</div></th>\
                                        <th><div class="col100 align-right">' + numeral(row.ChildInfo[2].TotalBookingsFifteenMinOU).format(FormatConstant.INT_NUMBER) + '</div></th>\
                                        <th><div class="col100 align-right">' + numeral(row.ChildInfo[2].NextBookings).format(FormatConstant.INT_NUMBER) + '</div></th>\
                                        <th><div class="col100 align-right">' + numeral(row.ChildInfo[2].FirstHalfOE).format(FormatConstant.INT_NUMBER) + '</div></th>\
                                        <th><div class="col100 align-right">' + numeral(row.ChildInfo[2].OE).format(FormatConstant.INT_NUMBER) + '</div></th>\
                                        <th><div class="col100 align-right">' + numeral(row.ChildInfo[2].Overall).format(FormatConstant.INT_NUMBER) + '</div></th>\
                                        <th><div class="col100 align-right">' + self.renderPercentCustomCell(row.ChildInfo[3].FirstHalfOneXTwo) + '</div></th>\
                                        <th><div class="col100 align-right">' + self.renderPercentCustomCell(row.ChildInfo[3].OneXTwo) + '</div></th>\
                                        <th><div class="col100 align-right">' + self.renderPercentCustomCell(row.ChildInfo[3].FifteenMinOneXTwo) + '</div></th>\
                                        <th><div class="col100 align-right">' + self.renderPercentCustomCell(row.ChildInfo[3].TenMinOneXTwo) + '</div></th>\
                                        <th><div class="col100 align-right">' + self.renderPercentCustomCell(row.ChildInfo[3].FirstHalfHdp) + '</div></th>\
                                        <th><div class="col100 align-right">' + self.renderPercentCustomCell(row.ChildInfo[3].Hdp) + '</div></th>\
                                        <th><div class="col100 align-right">' + self.renderPercentCustomCell(row.ChildInfo[3].FifteenMinHdp) + '</div></th>\
                                        <th><div class="col100 align-right">' + self.renderPercentCustomCell(row.ChildInfo[3].TenMinHdp) + '</div></th>\
                                        <th><div class="col100 align-right">' + self.renderPercentCustomCell(row.ChildInfo[3].FirstHalfOU) + '</div></th>\
                                        <th><div class="col100 align-right">' + self.renderPercentCustomCell(row.ChildInfo[3].OU) + '</div></th>\
                                        <th><div class="col100 align-right">' + self.renderPercentCustomCell(row.ChildInfo[3].FifteenMinOU) + '</div></th>\
                                        <th><div class="col100 align-right">' + self.renderPercentCustomCell(row.ChildInfo[3].TenMinOU) + '</div></th>\
                                        <th><div class="col100 align-right">' + self.renderPercentCustomCell(row.ChildInfo[3].SingleTeamOU) + '</div></th>\
                                        <th><div class="col100 align-right">' + self.renderPercentCustomCell(row.ChildInfo[3].FirstHalfNoOfCorners) + '</div></th>\
                                        <th><div class="col100 align-right">' + self.renderPercentCustomCell(row.ChildInfo[3].NoOfCorners) + '</div></th>\
                                        <th><div class="col100 align-right">' + self.renderPercentCustomCell(row.ChildInfo[3].FifteenMinNoOfCorners) + '</div></th>\
                                        <th><div class="col100 align-right">' + self.renderPercentCustomCell(row.ChildInfo[3].NextCorner) + '</div></th>\
                                        <th><div class="col100 align-right">' + self.renderPercentCustomCell(row.ChildInfo[3].FirstHalfTotalBookings) + '</div></th>\
                                        <th><div class="col100 align-right">' + self.renderPercentCustomCell(row.ChildInfo[3].TotalBookings) + '</div></th>\
                                        <th><div class="col100 align-right">' + self.renderPercentCustomCell(row.ChildInfo[3].TotalBookingsFifteenMinOU) + '</div></th>\
                                        <th><div class="col100 align-right">' + self.renderPercentCustomCell(row.ChildInfo[3].NextBookings) + '</div></th>\
                                        <th><div class="col100 align-right">' + self.renderPercentCustomCell(row.ChildInfo[3].FirstHalfOE) + '</div></th>\
                                        <th><div class="col100 align-right">' + self.renderPercentCustomCell(row.ChildInfo[3].OE) + '</div></th>\
                                        <th><div class="col100 align-right">' + self.renderPercentCustomCell(row.ChildInfo[3].Overall) + '</div></th>\
                                        <th><div class="col100 align-right">' + self.renderDecimalNumberCustomCell(row.ChildInfo[4].FirstHalfOneXTwo) + '</div></th>\
                                        <th><div class="col100 align-right">' + self.renderDecimalNumberCustomCell(row.ChildInfo[4].OneXTwo) + '</div></th>\
                                        <th><div class="col100 align-right">' + self.renderDecimalNumberCustomCell(row.ChildInfo[4].FifteenMinOneXTwo) + '</div></th>\
                                        <th><div class="col100 align-right">' + self.renderDecimalNumberCustomCell(row.ChildInfo[4].TenMinOneXTwo) + '</div></th>\
                                        <th><div class="col100 align-right">' + self.renderDecimalNumberCustomCell(row.ChildInfo[4].FirstHalfHdp) + '</div></th>\
                                        <th><div class="col100 align-right">' + self.renderDecimalNumberCustomCell(row.ChildInfo[4].Hdp) + '</div></th>\
                                        <th><div class="col100 align-right">' + self.renderDecimalNumberCustomCell(row.ChildInfo[4].FifteenMinHdp) + '</div></th>\
                                        <th><div class="col100 align-right">' + self.renderDecimalNumberCustomCell(row.ChildInfo[4].TenMinHdp) + '</div></th>\
                                        <th><div class="col100 align-right">' + self.renderDecimalNumberCustomCell(row.ChildInfo[4].FirstHalfOU) + '</div></th>\
                                        <th><div class="col100 align-right">' + self.renderDecimalNumberCustomCell(row.ChildInfo[4].OU) + '</div></th>\
                                        <th><div class="col100 align-right">' + self.renderDecimalNumberCustomCell(row.ChildInfo[4].FifteenMinOU) + '</div></th>\
                                        <th><div class="col100 align-right">' + self.renderDecimalNumberCustomCell(row.ChildInfo[4].TenMinOU) + '</div></th>\
                                        <th><div class="col100 align-right">' + self.renderDecimalNumberCustomCell(row.ChildInfo[4].SingleTeamOU) + '</div></th>\
                                        <th><div class="col100 align-right">' + self.renderDecimalNumberCustomCell(row.ChildInfo[4].FirstHalfNoOfCorners) + '</div></th>\
                                        <th><div class="col100 align-right">' + self.renderDecimalNumberCustomCell(row.ChildInfo[4].NoOfCorners) + '</div></th>\
                                        <th><div class="col100 align-right">' + self.renderDecimalNumberCustomCell(row.ChildInfo[4].FifteenMinNoOfCorners) + '</div></th>\
                                        <th><div class="col100 align-right">' + self.renderDecimalNumberCustomCell(row.ChildInfo[4].NextCorner) + '</div></th>\
                                        <th><div class="col100 align-right">' + self.renderDecimalNumberCustomCell(row.ChildInfo[4].FirstHalfTotalBookings) + '</div></th>\
                                        <th><div class="col100 align-right">' + self.renderDecimalNumberCustomCell(row.ChildInfo[4].TotalBookings) + '</div></th>\
                                        <th><div class="col100 align-right">' + self.renderDecimalNumberCustomCell(row.ChildInfo[4].TotalBookingsFifteenMinOU) + '</div></th>\
                                        <th><div class="col100 align-right">' + self.renderDecimalNumberCustomCell(row.ChildInfo[4].NextBookings) + '</div></th>\
                                        <th><div class="col100 align-right">' + self.renderDecimalNumberCustomCell(row.ChildInfo[4].FirstHalfOE) + '</div></th>\
                                        <th><div class="col100 align-right">' + self.renderDecimalNumberCustomCell(row.ChildInfo[4].OE) + '</div></th>\
                                        <th><div class="col100 align-right">' + self.renderDecimalNumberCustomCell(row.ChildInfo[4].Overall) + '</div></th>\
                                    </tr>';
    };

    self.renderReportNoData = function () {
        _tables.reportExportExcel.addClass("display-none");
        _tables.reportDivSorter.removeClass("divSorter");
        var frezeeContent = "<td colspan='8' class='col415'><div class='no-data-message'>There is no data.</div></td>";
        _tables.reportRowHeader.empty().append("<tr>" + frezeeContent + "</tr>");
        _tables.reportContent.empty().append(
                "<tr>" +
                    frezeeContent +
                    "<td colspan='100' class='col11525'>&nbsp</td>\
                 </tr>");
        _tables.reportRowFooter.empty();
        _tables.reportColFooter.empty();
    };

    self.renderReportDetailNoData = function () {
        _tables.reportDetailExportExcel.addClass("display-none");
        _tables.reportDetailDivSorter.removeClass("divSorter");
        var frezeeContent = "<td colspan='6' class='col585'><div class='no-data-message'>There is no data.</div></td>";
        _tables.reportDetailRowHeader.empty().append("<tr>" + frezeeContent + "</tr>");
        _tables.reportDetailContent.empty().append(
                "<tr>" +
                    frezeeContent +
                    "<td colspan='102' class='col11525'>&nbsp</td>\
                 </tr>");
        _tables.reportDetailRowFooter.empty();
        _tables.reportDetailColFooter.empty();
    };

    self.resetTempHtml = function () {
        _tempHtml.frozenHtml = '';
        _tempHtml.contentHtml = '';
        _tempHtml.footerFrozenHtml = '';
        _tempHtml.footerContentHtml = '';
    };

    self.removeSpaceIE9TempHtml = function () {
        _tempHtml.frozenHtml = fixBugsBrowser.removeSpaceIE9(_tempHtml.frozenHtml);
        _tempHtml.contentHtml = fixBugsBrowser.removeSpaceIE9(_tempHtml.contentHtml);
        _tempHtml.footerFrozenHtml = fixBugsBrowser.removeSpaceIE9(_tempHtml.footerFrozenHtml);
        _tempHtml.footerContentHtml = fixBugsBrowser.removeSpaceIE9(_tempHtml.footerContentHtml);
    };

    self.renderDetailReportRow = function (currentGroup, currentGroupIndex) {
        var currentGroupLength = currentGroup.ReportRows.length;
        currentGroupIndex = currentGroupIndex + _param.reportDetailParams.PageSize * (_param.reportDetailParams.PageIndex - 1);

        for (var childRowIndex = 0; childRowIndex < currentGroupLength; childRowIndex++) {
            var frozenHtml = '';
            var row = currentGroup.ReportRows[childRowIndex];

            if (childRowIndex == 0) {
                frozenHtml = '<td rowspan="' + currentGroupLength + '"><div class="col40 align-center">' + currentGroupIndex + '</div></td>\
                    <td rowspan="' + currentGroupLength + '"><div class="col100 align-center">' + currentGroup.Name + '</div></td>';
            }

            frozenHtml += '<td><div class="col40 align-left">' + row.EventDate + '</div></td>\
                <td><div class="col150 align-left">' + row.LeagueName + '</div></td>\
                <td><div class="col150 align-left">' + row.HomeName + '-' + row.AwayName + '</div></td>\
                <td><div class="col80 align-right">' + self.renderDecimalNumberCustomCell(row.Maxbet) + '</div></td>';

            _tempHtml.frozenHtml += '<tr>' + frozenHtml + ' </tr>';
            _tempHtml.contentHtml += '<tr>' +
                                        frozenHtml +
                                        '<td><div class="col60 align-center">' + row.HTHomeScore + '--' + row.HTAwayScore + '</div></td>\
                                        <td><div class="col60 align-center">' + row.FTHomeScore + '--' + row.FTAwayScore + '</div></td>\
                                        <td class="winLossGroup"><div class="col100 align-right">' + self.renderDecimalNumberCustomCell(row.ChildInfo[0].FirstHalfOneXTwo) + '</div></td>\
                                        <td class="winLossGroup"><div class="col100 align-right">' + self.renderDecimalNumberCustomCell(row.ChildInfo[0].OneXTwo) + '</div></td>\
                                        <td class="winLossGroup"><div class="col100 align-right">' + self.renderDecimalNumberCustomCell(row.ChildInfo[0].FifteenMinOneXTwo) + '</div></td>\
                                        <td class="winLossGroup"><div class="col100 align-right">' + self.renderDecimalNumberCustomCell(row.ChildInfo[0].TenMinOneXTwo) + '</div></td>\
                                        <td class="winLossGroup"><div class="col100 align-right">' + self.renderDecimalNumberCustomCell(row.ChildInfo[0].FirstHalfHdp) + '</div></td>\
                                        <td class="winLossGroup"><div class="col100 align-right">' + self.renderDecimalNumberCustomCell(row.ChildInfo[0].Hdp) + '</div></td>\
                                        <td class="winLossGroup"><div class="col100 align-right">' + self.renderDecimalNumberCustomCell(row.ChildInfo[0].FifteenMinHdp) + '</div></td>\
                                        <td class="winLossGroup"><div class="col100 align-right">' + self.renderDecimalNumberCustomCell(row.ChildInfo[0].TenMinHdp) + '</div></td>\
                                        <td class="winLossGroup"><div class="col100 align-right">' + self.renderDecimalNumberCustomCell(row.ChildInfo[0].FirstHalfOU) + '</div></td>\
                                        <td class="winLossGroup"><div class="col100 align-right">' + self.renderDecimalNumberCustomCell(row.ChildInfo[0].OU) + '</div></td>\
                                        <td class="winLossGroup"><div class="col100 align-right">' + self.renderDecimalNumberCustomCell(row.ChildInfo[0].FifteenMinOU) + '</div></td>\
                                        <td class="winLossGroup"><div class="col100 align-right">' + self.renderDecimalNumberCustomCell(row.ChildInfo[0].TenMinOU) + '</div></td>\
                                        <td class="winLossGroup"><div class="col100 align-right">' + self.renderDecimalNumberCustomCell(row.ChildInfo[0].SingleTeamOU) + '</div></td>\
                                        <td class="winLossGroup"><div class="col100 align-right">' + self.renderDecimalNumberCustomCell(row.ChildInfo[0].FirstHalfNoOfCorners) + '</div></td>\
                                        <td class="winLossGroup"><div class="col100 align-right">' + self.renderDecimalNumberCustomCell(row.ChildInfo[0].NoOfCorners) + '</div></td>\
                                        <td class="winLossGroup"><div class="col100 align-right">' + self.renderDecimalNumberCustomCell(row.ChildInfo[0].FifteenMinNoOfCorners) + '</div></td>\
                                        <td class="winLossGroup"><div class="col100 align-right">' + self.renderDecimalNumberCustomCell(row.ChildInfo[0].NextCorner) + '</div></td>\
                                        <td class="winLossGroup"><div class="col100 align-right">' + self.renderDecimalNumberCustomCell(row.ChildInfo[0].FirstHalfTotalBookings) + '</div></td>\
                                        <td class="winLossGroup"><div class="col100 align-right">' + self.renderDecimalNumberCustomCell(row.ChildInfo[0].TotalBookings) + '</div></td>\
                                        <td class="winLossGroup"><div class="col100 align-right">' + self.renderDecimalNumberCustomCell(row.ChildInfo[0].TotalBookingsFifteenMinOU) + '</div></td>\
                                        <td class="winLossGroup"><div class="col100 align-right">' + self.renderDecimalNumberCustomCell(row.ChildInfo[0].NextBookings) + '</div></td>\
                                        <td class="winLossGroup"><div class="col100 align-right">' + self.renderDecimalNumberCustomCell(row.ChildInfo[0].FirstHalfOE) + '</div></td>\
                                        <td class="winLossGroup"><div class="col100 align-right">' + self.renderDecimalNumberCustomCell(row.ChildInfo[0].OE) + '</div></td>\
                                        <td class="winLossGroup"><div class="col100 align-right">' + self.renderDecimalNumberCustomCell(row.ChildInfo[0].Overall) + '</div></td>\
                                        <td class="turnOverGroup"><div class="col100 align-right">' + self.renderDecimalNumberCustomCell(row.ChildInfo[1].FirstHalfOneXTwo) + '</div></td>\
                                        <td class="turnOverGroup"><div class="col100 align-right">' + self.renderDecimalNumberCustomCell(row.ChildInfo[1].OneXTwo) + '</div></td>\
                                        <td class="turnOverGroup"><div class="col100 align-right">' + self.renderDecimalNumberCustomCell(row.ChildInfo[1].FifteenMinOneXTwo) + '</div></td>\
                                        <td class="turnOverGroup"><div class="col100 align-right">' + self.renderDecimalNumberCustomCell(row.ChildInfo[1].TenMinOneXTwo) + '</div></td>\
                                        <td class="turnOverGroup"><div class="col100 align-right">' + self.renderDecimalNumberCustomCell(row.ChildInfo[1].FirstHalfHdp) + '</div></td>\
                                        <td class="turnOverGroup"><div class="col100 align-right">' + self.renderDecimalNumberCustomCell(row.ChildInfo[1].Hdp) + '</div></td>\
                                        <td class="turnOverGroup"><div class="col100 align-right">' + self.renderDecimalNumberCustomCell(row.ChildInfo[1].FifteenMinHdp) + '</div></td>\
                                        <td class="turnOverGroup"><div class="col100 align-right">' + self.renderDecimalNumberCustomCell(row.ChildInfo[1].TenMinHdp) + '</div></td>\
                                        <td class="turnOverGroup"><div class="col100 align-right">' + self.renderDecimalNumberCustomCell(row.ChildInfo[1].FirstHalfOU) + '</div></td>\
                                        <td class="turnOverGroup"><div class="col100 align-right">' + self.renderDecimalNumberCustomCell(row.ChildInfo[1].OU) + '</div></td>\
                                        <td class="turnOverGroup"><div class="col100 align-right">' + self.renderDecimalNumberCustomCell(row.ChildInfo[1].FifteenMinOU) + '</div></td>\
                                        <td class="turnOverGroup"><div class="col100 align-right">' + self.renderDecimalNumberCustomCell(row.ChildInfo[1].TenMinOU) + '</div></td>\
                                        <td class="turnOverGroup"><div class="col100 align-right">' + self.renderDecimalNumberCustomCell(row.ChildInfo[1].SingleTeamOU) + '</div></td>\
                                        <td class="turnOverGroup"><div class="col100 align-right">' + self.renderDecimalNumberCustomCell(row.ChildInfo[1].FirstHalfNoOfCorners) + '</div></td>\
                                        <td class="turnOverGroup"><div class="col100 align-right">' + self.renderDecimalNumberCustomCell(row.ChildInfo[1].NoOfCorners) + '</div></td>\
                                        <td class="turnOverGroup"><div class="col100 align-right">' + self.renderDecimalNumberCustomCell(row.ChildInfo[1].FifteenMinNoOfCorners) + '</div></td>\
                                        <td class="turnOverGroup"><div class="col100 align-right">' + self.renderDecimalNumberCustomCell(row.ChildInfo[1].NextCorner) + '</div></td>\
                                        <td class="turnOverGroup"><div class="col100 align-right">' + self.renderDecimalNumberCustomCell(row.ChildInfo[1].FirstHalfTotalBookings) + '</div></td>\
                                        <td class="turnOverGroup"><div class="col100 align-right">' + self.renderDecimalNumberCustomCell(row.ChildInfo[1].TotalBookings) + '</div></td>\
                                        <td class="turnOverGroup"><div class="col100 align-right">' + self.renderDecimalNumberCustomCell(row.ChildInfo[1].TotalBookingsFifteenMinOU) + '</div></td>\
                                        <td class="turnOverGroup"><div class="col100 align-right">' + self.renderDecimalNumberCustomCell(row.ChildInfo[1].NextBookings) + '</div></td>\
                                        <td class="turnOverGroup"><div class="col100 align-right">' + self.renderDecimalNumberCustomCell(row.ChildInfo[1].FirstHalfOE) + '</div></td>\
                                        <td class="turnOverGroup"><div class="col100 align-right">' + self.renderDecimalNumberCustomCell(row.ChildInfo[1].OE) + '</div></td>\
                                        <td class="turnOverGroup"><div class="col100 align-right">' + self.renderDecimalNumberCustomCell(row.ChildInfo[1].Overall) + '</div></td>\
                                        <td class="liveBetCountGroup"><div class="col100 align-right">' + numeral(row.ChildInfo[2].FirstHalfOneXTwo).format(FormatConstant.INT_NUMBER) + '</div></td>\
                                        <td class="liveBetCountGroup"><div class="col100 align-right">' + numeral(row.ChildInfo[2].OneXTwo).format(FormatConstant.INT_NUMBER) + '</div></td>\
                                        <td class="liveBetCountGroup"><div class="col100 align-right">' + numeral(row.ChildInfo[2].FifteenMinOneXTwo).format(FormatConstant.INT_NUMBER) + '</div></td>\
                                        <td class="liveBetCountGroup"><div class="col100 align-right">' + numeral(row.ChildInfo[2].TenMinOneXTwo).format(FormatConstant.INT_NUMBER) + '</div></td>\
                                        <td class="liveBetCountGroup"><div class="col100 align-right">' + numeral(row.ChildInfo[2].FirstHalfHdp).format(FormatConstant.INT_NUMBER) + '</div></td>\
                                        <td class="liveBetCountGroup"><div class="col100 align-right">' + numeral(row.ChildInfo[2].Hdp).format(FormatConstant.INT_NUMBER) + '</div></td>\
                                        <td class="liveBetCountGroup"><div class="col100 align-right">' + numeral(row.ChildInfo[2].FifteenMinHdp).format(FormatConstant.INT_NUMBER) + '</div></td>\
                                        <td class="liveBetCountGroup"><div class="col100 align-right">' + numeral(row.ChildInfo[2].TenMinHdp).format(FormatConstant.INT_NUMBER) + '</div></td>\
                                        <td class="liveBetCountGroup"><div class="col100 align-right">' + numeral(row.ChildInfo[2].FirstHalfOU).format(FormatConstant.INT_NUMBER) + '</div></td>\
                                        <td class="liveBetCountGroup"><div class="col100 align-right">' + numeral(row.ChildInfo[2].OU).format(FormatConstant.INT_NUMBER) + '</div></td>\
                                        <td class="liveBetCountGroup"><div class="col100 align-right">' + numeral(row.ChildInfo[2].FifteenMinOU).format(FormatConstant.INT_NUMBER) + '</div></td>\
                                        <td class="liveBetCountGroup"><div class="col100 align-right">' + numeral(row.ChildInfo[2].TenMinOU).format(FormatConstant.INT_NUMBER) + '</div></td>\
                                        <td class="liveBetCountGroup"><div class="col100 align-right">' + numeral(row.ChildInfo[2].SingleTeamOU).format(FormatConstant.INT_NUMBER) + '</div></td>\
                                        <td class="liveBetCountGroup"><div class="col100 align-right">' + numeral(row.ChildInfo[2].FirstHalfNoOfCorners).format(FormatConstant.INT_NUMBER) + '</div></td>\
                                        <td class="liveBetCountGroup"><div class="col100 align-right">' + numeral(row.ChildInfo[2].NoOfCorners).format(FormatConstant.INT_NUMBER) + '</div></td>\
                                        <td class="liveBetCountGroup"><div class="col100 align-right">' + numeral(row.ChildInfo[2].FifteenMinNoOfCorners).format(FormatConstant.INT_NUMBER) + '</div></td>\
                                        <td class="liveBetCountGroup"><div class="col100 align-right">' + numeral(row.ChildInfo[2].NextCorner).format(FormatConstant.INT_NUMBER) + '</div></td>\
                                        <td class="liveBetCountGroup"><div class="col100 align-right">' + numeral(row.ChildInfo[2].FirstHalfTotalBookings).format(FormatConstant.INT_NUMBER) + '</div></td>\
                                        <td class="liveBetCountGroup"><div class="col100 align-right">' + numeral(row.ChildInfo[2].TotalBookings).format(FormatConstant.INT_NUMBER) + '</div></td>\
                                        <td class="liveBetCountGroup"><div class="col100 align-right">' + numeral(row.ChildInfo[2].TotalBookingsFifteenMinOU).format(FormatConstant.INT_NUMBER) + '</div></td>\
                                        <td class="liveBetCountGroup"><div class="col100 align-right">' + numeral(row.ChildInfo[2].NextBookings).format(FormatConstant.INT_NUMBER) + '</div></td>\
                                        <td class="liveBetCountGroup"><div class="col100 align-right">' + numeral(row.ChildInfo[2].FirstHalfOE).format(FormatConstant.INT_NUMBER) + '</div></td>\
                                        <td class="liveBetCountGroup"><div class="col100 align-right">' + numeral(row.ChildInfo[2].OE).format(FormatConstant.INT_NUMBER) + '</div></td>\
                                        <td class="liveBetCountGroup"><div class="col100 align-right">' + numeral(row.ChildInfo[2].Overall).format(FormatConstant.INT_NUMBER) + '</div></td>\
                                        <td class="marginGroup"><div class="col100 align-right">' + self.renderPercentCustomCell(row.ChildInfo[3].FirstHalfOneXTwo) + '</div></td>\
                                        <td class="marginGroup"><div class="col100 align-right">' + self.renderPercentCustomCell(row.ChildInfo[3].OneXTwo) + '</div></td>\
                                        <td class="marginGroup"><div class="col100 align-right">' + self.renderPercentCustomCell(row.ChildInfo[3].FifteenMinOneXTwo) + '</div></td>\
                                        <td class="marginGroup"><div class="col100 align-right">' + self.renderPercentCustomCell(row.ChildInfo[3].TenMinOneXTwo) + '</div></td>\
                                        <td class="marginGroup"><div class="col100 align-right">' + self.renderPercentCustomCell(row.ChildInfo[3].FirstHalfHdp) + '</div></td>\
                                        <td class="marginGroup"><div class="col100 align-right">' + self.renderPercentCustomCell(row.ChildInfo[3].Hdp) + '</div></td>\
                                        <td class="marginGroup"><div class="col100 align-right">' + self.renderPercentCustomCell(row.ChildInfo[3].FifteenMinHdp) + '</div></td>\
                                        <td class="marginGroup"><div class="col100 align-right">' + self.renderPercentCustomCell(row.ChildInfo[3].TenMinHdp) + '</div></td>\
                                        <td class="marginGroup"><div class="col100 align-right">' + self.renderPercentCustomCell(row.ChildInfo[3].FirstHalfOU) + '</div></td>\
                                        <td class="marginGroup"><div class="col100 align-right">' + self.renderPercentCustomCell(row.ChildInfo[3].OU) + '</div></td>\
                                        <td class="marginGroup"><div class="col100 align-right">' + self.renderPercentCustomCell(row.ChildInfo[3].FifteenMinOU) + '</div></td>\
                                        <td class="marginGroup"><div class="col100 align-right">' + self.renderPercentCustomCell(row.ChildInfo[3].TenMinOU) + '</div></td>\
                                        <td class="marginGroup"><div class="col100 align-right">' + self.renderPercentCustomCell(row.ChildInfo[3].SingleTeamOU) + '</div></td>\
                                        <td class="marginGroup"><div class="col100 align-right">' + self.renderPercentCustomCell(row.ChildInfo[3].FirstHalfNoOfCorners) + '</div></td>\
                                        <td class="marginGroup"><div class="col100 align-right">' + self.renderPercentCustomCell(row.ChildInfo[3].NoOfCorners) + '</div></td>\
                                        <td class="marginGroup"><div class="col100 align-right">' + self.renderPercentCustomCell(row.ChildInfo[3].FifteenMinNoOfCorners) + '</div></td>\
                                        <td class="marginGroup"><div class="col100 align-right">' + self.renderPercentCustomCell(row.ChildInfo[3].NextCorner) + '</div></td>\
                                        <td class="marginGroup"><div class="col100 align-right">' + self.renderPercentCustomCell(row.ChildInfo[3].FirstHalfTotalBookings) + '</div></td>\
                                        <td class="marginGroup"><div class="col100 align-right">' + self.renderPercentCustomCell(row.ChildInfo[3].TotalBookings) + '</div></td>\
                                        <td class="marginGroup"><div class="col100 align-right">' + self.renderPercentCustomCell(row.ChildInfo[3].TotalBookingsFifteenMinOU) + '</div></td>\
                                        <td class="marginGroup"><div class="col100 align-right">' + self.renderPercentCustomCell(row.ChildInfo[3].NextBookings) + '</div></td>\
                                        <td class="marginGroup"><div class="col100 align-right">' + self.renderPercentCustomCell(row.ChildInfo[3].FirstHalfOE) + '</div></td>\
                                        <td class="marginGroup"><div class="col100 align-right">' + self.renderPercentCustomCell(row.ChildInfo[3].OE) + '</div></td>\
                                        <td class="marginGroup"><div class="col100 align-right">' + self.renderPercentCustomCell(row.ChildInfo[3].Overall) + '</div></td>\
                                        <td class="commissionGroup"><div class="col100 align-right">' + self.renderDecimalNumberCustomCell(row.ChildInfo[4].FirstHalfOneXTwo) + '</div></td>\
                                        <td class="commissionGroup"><div class="col100 align-right">' + self.renderDecimalNumberCustomCell(row.ChildInfo[4].OneXTwo) + '</div></td>\
                                        <td class="commissionGroup"><div class="col100 align-right">' + self.renderDecimalNumberCustomCell(row.ChildInfo[4].FifteenMinOneXTwo) + '</div></td>\
                                        <td class="commissionGroup"><div class="col100 align-right">' + self.renderDecimalNumberCustomCell(row.ChildInfo[4].TenMinOneXTwo) + '</div></td>\
                                        <td class="commissionGroup"><div class="col100 align-right">' + self.renderDecimalNumberCustomCell(row.ChildInfo[4].FirstHalfHdp) + '</div></td>\
                                        <td class="commissionGroup"><div class="col100 align-right">' + self.renderDecimalNumberCustomCell(row.ChildInfo[4].Hdp) + '</div></td>\
                                        <td class="commissionGroup"><div class="col100 align-right">' + self.renderDecimalNumberCustomCell(row.ChildInfo[4].FifteenMinHdp) + '</div></td>\
                                        <td class="commissionGroup"><div class="col100 align-right">' + self.renderDecimalNumberCustomCell(row.ChildInfo[4].TenMinHdp) + '</div></td>\
                                        <td class="commissionGroup"><div class="col100 align-right">' + self.renderDecimalNumberCustomCell(row.ChildInfo[4].FirstHalfOU) + '</div></td>\
                                        <td class="commissionGroup"><div class="col100 align-right">' + self.renderDecimalNumberCustomCell(row.ChildInfo[4].OU) + '</div></td>\
                                        <td class="commissionGroup"><div class="col100 align-right">' + self.renderDecimalNumberCustomCell(row.ChildInfo[4].FifteenMinOU) + '</div></td>\
                                        <td class="commissionGroup"><div class="col100 align-right">' + self.renderDecimalNumberCustomCell(row.ChildInfo[4].TenMinOU) + '</div></td>\
                                        <td class="commissionGroup"><div class="col100 align-right">' + self.renderDecimalNumberCustomCell(row.ChildInfo[4].SingleTeamOU) + '</div></td>\
                                        <td class="commissionGroup"><div class="col100 align-right">' + self.renderDecimalNumberCustomCell(row.ChildInfo[4].FirstHalfNoOfCorners) + '</div></td>\
                                        <td class="commissionGroup"><div class="col100 align-right">' + self.renderDecimalNumberCustomCell(row.ChildInfo[4].NoOfCorners) + '</div></td>\
                                        <td class="commissionGroup"><div class="col100 align-right">' + self.renderDecimalNumberCustomCell(row.ChildInfo[4].FifteenMinNoOfCorners) + '</div></td>\
                                        <td class="commissionGroup"><div class="col100 align-right">' + self.renderDecimalNumberCustomCell(row.ChildInfo[4].NextCorner) + '</div></td>\
                                        <td class="commissionGroup"><div class="col100 align-right">' + self.renderDecimalNumberCustomCell(row.ChildInfo[4].FirstHalfTotalBookings) + '</div></td>\
                                        <td class="commissionGroup"><div class="col100 align-right">' + self.renderDecimalNumberCustomCell(row.ChildInfo[4].TotalBookings) + '</div></td>\
                                        <td class="commissionGroup"><div class="col100 align-right">' + self.renderDecimalNumberCustomCell(row.ChildInfo[4].TotalBookingsFifteenMinOU) + '</div></td>\
                                        <td class="commissionGroup"><div class="col100 align-right">' + self.renderDecimalNumberCustomCell(row.ChildInfo[4].NextBookings) + '</div></td>\
                                        <td class="commissionGroup"><div class="col100 align-right">' + self.renderDecimalNumberCustomCell(row.ChildInfo[4].FirstHalfOE) + '</div></td>\
                                        <td class="commissionGroup"><div class="col100 align-right">' + self.renderDecimalNumberCustomCell(row.ChildInfo[4].OE) + '</div></td>\
                                        <td class="commissionGroup"><div class="col100 align-right">' + self.renderDecimalNumberCustomCell(row.ChildInfo[4].Overall) + '</div></td>\
                                    </tr>';
        }
    };

    self.renderSubTotalDetailReport = function (currentChildInfo, cssClass) {
        var totalInfo = currentChildInfo.GrandTotal;
        var frozenHtml = '';
        frozenHtml = '<td class="' + cssClass + '" colspan="6"><div class="col585 align-right">' + currentChildInfo.Name + '</div></td>'
        _tempHtml.frozenHtml += '<tr>' + frozenHtml + ' </tr>';
        _tempHtml.contentHtml += '<tr>' +
                                        frozenHtml +
                                        '<td class="' + cssClass + '"><div class="col60 align-center"></div></td>\
                                        <td class="' + cssClass + '"><div class="col60 align-center"></div></td>\
                                        <td class="' + cssClass + '"><div class="col100 align-right">' + self.renderDecimalNumberCustomCell(totalInfo.ChildInfo[0].FirstHalfOneXTwo) + '</div></td>\
                                        <td class="' + cssClass + '"><div class="col100 align-right">' + self.renderDecimalNumberCustomCell(totalInfo.ChildInfo[0].OneXTwo) + '</div></td>\
                                        <td class="' + cssClass + '"><div class="col100 align-right">' + self.renderDecimalNumberCustomCell(totalInfo.ChildInfo[0].FifteenMinOneXTwo) + '</div></td>\
                                        <td class="' + cssClass + '"><div class="col100 align-right">' + self.renderDecimalNumberCustomCell(totalInfo.ChildInfo[0].TenMinOneXTwo) + '</div></td>\
                                        <td class="' + cssClass + '"><div class="col100 align-right">' + self.renderDecimalNumberCustomCell(totalInfo.ChildInfo[0].FirstHalfHdp) + '</div></td>\
                                        <td class="' + cssClass + '"><div class="col100 align-right">' + self.renderDecimalNumberCustomCell(totalInfo.ChildInfo[0].Hdp) + '</div></td>\
                                        <td class="' + cssClass + '"><div class="col100 align-right">' + self.renderDecimalNumberCustomCell(totalInfo.ChildInfo[0].FifteenMinHdp) + '</div></td>\
                                        <td class="' + cssClass + '"><div class="col100 align-right">' + self.renderDecimalNumberCustomCell(totalInfo.ChildInfo[0].TenMinHdp) + '</div></td>\
                                        <td class="' + cssClass + '"><div class="col100 align-right">' + self.renderDecimalNumberCustomCell(totalInfo.ChildInfo[0].FirstHalfOU) + '</div></td>\
                                        <td class="' + cssClass + '"><div class="col100 align-right">' + self.renderDecimalNumberCustomCell(totalInfo.ChildInfo[0].OU) + '</div></td>\
                                        <td class="' + cssClass + '"><div class="col100 align-right">' + self.renderDecimalNumberCustomCell(totalInfo.ChildInfo[0].FifteenMinOU) + '</div></td>\
                                        <td class="' + cssClass + '"><div class="col100 align-right">' + self.renderDecimalNumberCustomCell(totalInfo.ChildInfo[0].TenMinOU) + '</div></td>\
                                        <td class="' + cssClass + '"><div class="col100 align-right">' + self.renderDecimalNumberCustomCell(totalInfo.ChildInfo[0].SingleTeamOU) + '</div></td>\
                                        <td class="' + cssClass + '"><div class="col100 align-right">' + self.renderDecimalNumberCustomCell(totalInfo.ChildInfo[0].FirstHalfNoOfCorners) + '</div></td>\
                                        <td class="' + cssClass + '"><div class="col100 align-right">' + self.renderDecimalNumberCustomCell(totalInfo.ChildInfo[0].NoOfCorners) + '</div></td>\
                                        <td class="' + cssClass + '"><div class="col100 align-right">' + self.renderDecimalNumberCustomCell(totalInfo.ChildInfo[0].FifteenMinNoOfCorners) + '</div></td>\
                                        <td class="' + cssClass + '"><div class="col100 align-right">' + self.renderDecimalNumberCustomCell(totalInfo.ChildInfo[0].NextCorner) + '</div></td>\
                                        <td class="' + cssClass + '"><div class="col100 align-right">' + self.renderDecimalNumberCustomCell(totalInfo.ChildInfo[0].FirstHalfTotalBookings) + '</div></td>\
                                        <td class="' + cssClass + '"><div class="col100 align-right">' + self.renderDecimalNumberCustomCell(totalInfo.ChildInfo[0].TotalBookings) + '</div></td>\
                                        <td class="' + cssClass + '"><div class="col100 align-right">' + self.renderDecimalNumberCustomCell(totalInfo.ChildInfo[0].TotalBookingsFifteenMinOU) + '</div></td>\
                                        <td class="' + cssClass + '"><div class="col100 align-right">' + self.renderDecimalNumberCustomCell(totalInfo.ChildInfo[0].NextBookings) + '</div></td>\
                                        <td class="' + cssClass + '"><div class="col100 align-right">' + self.renderDecimalNumberCustomCell(totalInfo.ChildInfo[0].FirstHalfOE) + '</div></td>\
                                        <td class="' + cssClass + '"><div class="col100 align-right">' + self.renderDecimalNumberCustomCell(totalInfo.ChildInfo[0].OE) + '</div></td>\
                                        <td class="' + cssClass + '"><div class="col100 align-right">' + self.renderDecimalNumberCustomCell(totalInfo.ChildInfo[0].Overall) + '</div></td>\
                                        <td class="' + cssClass + '"><div class="col100 align-right">' + self.renderDecimalNumberCustomCell(totalInfo.ChildInfo[1].FirstHalfOneXTwo) + '</div></td>\
                                        <td class="' + cssClass + '"><div class="col100 align-right">' + self.renderDecimalNumberCustomCell(totalInfo.ChildInfo[1].OneXTwo) + '</div></td>\
                                        <td class="' + cssClass + '"><div class="col100 align-right">' + self.renderDecimalNumberCustomCell(totalInfo.ChildInfo[1].FifteenMinOneXTwo) + '</div></td>\
                                        <td class="' + cssClass + '"><div class="col100 align-right">' + self.renderDecimalNumberCustomCell(totalInfo.ChildInfo[1].TenMinOneXTwo) + '</div></td>\
                                        <td class="' + cssClass + '"><div class="col100 align-right">' + self.renderDecimalNumberCustomCell(totalInfo.ChildInfo[1].FirstHalfHdp) + '</div></td>\
                                        <td class="' + cssClass + '"><div class="col100 align-right">' + self.renderDecimalNumberCustomCell(totalInfo.ChildInfo[1].Hdp) + '</div></td>\
                                        <td class="' + cssClass + '"><div class="col100 align-right">' + self.renderDecimalNumberCustomCell(totalInfo.ChildInfo[1].FifteenMinHdp) + '</div></td>\
                                        <td class="' + cssClass + '"><div class="col100 align-right">' + self.renderDecimalNumberCustomCell(totalInfo.ChildInfo[1].TenMinHdp) + '</div></td>\
                                        <td class="' + cssClass + '"><div class="col100 align-right">' + self.renderDecimalNumberCustomCell(totalInfo.ChildInfo[1].FirstHalfOU) + '</div></td>\
                                        <td class="' + cssClass + '"><div class="col100 align-right">' + self.renderDecimalNumberCustomCell(totalInfo.ChildInfo[1].OU) + '</div></td>\
                                        <td class="' + cssClass + '"><div class="col100 align-right">' + self.renderDecimalNumberCustomCell(totalInfo.ChildInfo[1].FifteenMinOU) + '</div></td>\
                                        <td class="' + cssClass + '"><div class="col100 align-right">' + self.renderDecimalNumberCustomCell(totalInfo.ChildInfo[1].TenMinOU) + '</div></td>\
                                        <td class="' + cssClass + '"><div class="col100 align-right">' + self.renderDecimalNumberCustomCell(totalInfo.ChildInfo[1].SingleTeamOU) + '</div></td>\
                                        <td class="' + cssClass + '"><div class="col100 align-right">' + self.renderDecimalNumberCustomCell(totalInfo.ChildInfo[1].FirstHalfNoOfCorners) + '</div></td>\
                                        <td class="' + cssClass + '"><div class="col100 align-right">' + self.renderDecimalNumberCustomCell(totalInfo.ChildInfo[1].NoOfCorners) + '</div></td>\
                                        <td class="' + cssClass + '"><div class="col100 align-right">' + self.renderDecimalNumberCustomCell(totalInfo.ChildInfo[1].FifteenMinNoOfCorners) + '</div></td>\
                                        <td class="' + cssClass + '"><div class="col100 align-right">' + self.renderDecimalNumberCustomCell(totalInfo.ChildInfo[1].NextCorner) + '</div></td>\
                                        <td class="' + cssClass + '"><div class="col100 align-right">' + self.renderDecimalNumberCustomCell(totalInfo.ChildInfo[1].FirstHalfTotalBookings) + '</div></td>\
                                        <td class="' + cssClass + '"><div class="col100 align-right">' + self.renderDecimalNumberCustomCell(totalInfo.ChildInfo[1].TotalBookings) + '</div></td>\
                                        <td class="' + cssClass + '"><div class="col100 align-right">' + self.renderDecimalNumberCustomCell(totalInfo.ChildInfo[1].TotalBookingsFifteenMinOU) + '</div></td>\
                                        <td class="' + cssClass + '"><div class="col100 align-right">' + self.renderDecimalNumberCustomCell(totalInfo.ChildInfo[1].NextBookings) + '</div></td>\
                                        <td class="' + cssClass + '"><div class="col100 align-right">' + self.renderDecimalNumberCustomCell(totalInfo.ChildInfo[1].FirstHalfOE) + '</div></td>\
                                        <td class="' + cssClass + '"><div class="col100 align-right">' + self.renderDecimalNumberCustomCell(totalInfo.ChildInfo[1].OE) + '</div></td>\
                                        <td class="' + cssClass + '"><div class="col100 align-right">' + self.renderDecimalNumberCustomCell(totalInfo.ChildInfo[1].Overall) + '</div></td>\
                                        <td class="' + cssClass + '"><div class="col100 align-right">' + numeral(totalInfo.ChildInfo[2].FirstHalfOneXTwo).format(FormatConstant.INT_NUMBER) + '</div></td>\
                                        <td class="' + cssClass + '"><div class="col100 align-right">' + numeral(totalInfo.ChildInfo[2].OneXTwo).format(FormatConstant.INT_NUMBER) + '</div></td>\
                                        <td class="' + cssClass + '"><div class="col100 align-right">' + numeral(totalInfo.ChildInfo[2].FifteenMinOneXTwo).format(FormatConstant.INT_NUMBER) + '</div></td>\
                                        <td class="' + cssClass + '"><div class="col100 align-right">' + numeral(totalInfo.ChildInfo[2].TenMinOneXTwo).format(FormatConstant.INT_NUMBER) + '</div></td>\
                                        <td class="' + cssClass + '"><div class="col100 align-right">' + numeral(totalInfo.ChildInfo[2].FirstHalfHdp).format(FormatConstant.INT_NUMBER) + '</div></td>\
                                        <td class="' + cssClass + '"><div class="col100 align-right">' + numeral(totalInfo.ChildInfo[2].Hdp).format(FormatConstant.INT_NUMBER) + '</div></td>\
                                        <td class="' + cssClass + '"><div class="col100 align-right">' + numeral(totalInfo.ChildInfo[2].FifteenMinHdp).format(FormatConstant.INT_NUMBER) + '</div></td>\
                                        <td class="' + cssClass + '"><div class="col100 align-right">' + numeral(totalInfo.ChildInfo[2].TenMinHdp).format(FormatConstant.INT_NUMBER) + '</div></td>\
                                        <td class="' + cssClass + '"><div class="col100 align-right">' + numeral(totalInfo.ChildInfo[2].FirstHalfOU).format(FormatConstant.INT_NUMBER) + '</div></td>\
                                        <td class="' + cssClass + '"><div class="col100 align-right">' + numeral(totalInfo.ChildInfo[2].OU).format(FormatConstant.INT_NUMBER) + '</div></td>\
                                        <td class="' + cssClass + '"><div class="col100 align-right">' + numeral(totalInfo.ChildInfo[2].FifteenMinOU).format(FormatConstant.INT_NUMBER) + '</div></td>\
                                        <td class="' + cssClass + '"><div class="col100 align-right">' + numeral(totalInfo.ChildInfo[2].TenMinOU).format(FormatConstant.INT_NUMBER) + '</div></td>\
                                        <td class="' + cssClass + '"><div class="col100 align-right">' + numeral(totalInfo.ChildInfo[2].SingleTeamOU).format(FormatConstant.INT_NUMBER) + '</div></td>\
                                        <td class="' + cssClass + '"><div class="col100 align-right">' + numeral(totalInfo.ChildInfo[2].FirstHalfNoOfCorners).format(FormatConstant.INT_NUMBER) + '</div></td>\
                                        <td class="' + cssClass + '"><div class="col100 align-right">' + numeral(totalInfo.ChildInfo[2].NoOfCorners).format(FormatConstant.INT_NUMBER) + '</div></td>\
                                        <td class="' + cssClass + '"><div class="col100 align-right">' + numeral(totalInfo.ChildInfo[2].FifteenMinNoOfCorners).format(FormatConstant.INT_NUMBER) + '</div></td>\
                                        <td class="' + cssClass + '"><div class="col100 align-right">' + numeral(totalInfo.ChildInfo[2].NextCorner).format(FormatConstant.INT_NUMBER) + '</div></td>\
                                        <td class="' + cssClass + '"><div class="col100 align-right">' + numeral(totalInfo.ChildInfo[2].FirstHalfTotalBookings).format(FormatConstant.INT_NUMBER) + '</div></td>\
                                        <td class="' + cssClass + '"><div class="col100 align-right">' + numeral(totalInfo.ChildInfo[2].TotalBookings).format(FormatConstant.INT_NUMBER) + '</div></td>\
                                        <td class="' + cssClass + '"><div class="col100 align-right">' + numeral(totalInfo.ChildInfo[2].TotalBookingsFifteenMinOU).format(FormatConstant.INT_NUMBER) + '</div></td>\
                                        <td class="' + cssClass + '"><div class="col100 align-right">' + numeral(totalInfo.ChildInfo[2].NextBookings).format(FormatConstant.INT_NUMBER) + '</div></td>\
                                        <td class="' + cssClass + '"><div class="col100 align-right">' + numeral(totalInfo.ChildInfo[2].FirstHalfOE).format(FormatConstant.INT_NUMBER) + '</div></td>\
                                        <td class="' + cssClass + '"><div class="col100 align-right">' + numeral(totalInfo.ChildInfo[2].OE).format(FormatConstant.INT_NUMBER) + '</div></td>\
                                        <td class="' + cssClass + '"><div class="col100 align-right">' + numeral(totalInfo.ChildInfo[2].Overall).format(FormatConstant.INT_NUMBER) + '</div></td>\
                                        <td class="' + cssClass + '"><div class="col100 align-right">' + self.renderPercentCustomCell(totalInfo.ChildInfo[3].FirstHalfOneXTwo) + '</div></td>\
                                        <td class="' + cssClass + '"><div class="col100 align-right">' + self.renderPercentCustomCell(totalInfo.ChildInfo[3].OneXTwo) + '</div></td>\
                                        <td class="' + cssClass + '"><div class="col100 align-right">' + self.renderPercentCustomCell(totalInfo.ChildInfo[3].FifteenMinOneXTwo) + '</div></td>\
                                        <td class="' + cssClass + '"><div class="col100 align-right">' + self.renderPercentCustomCell(totalInfo.ChildInfo[3].TenMinOneXTwo) + '</div></td>\
                                        <td class="' + cssClass + '"><div class="col100 align-right">' + self.renderPercentCustomCell(totalInfo.ChildInfo[3].FirstHalfHdp) + '</div></td>\
                                        <td class="' + cssClass + '"><div class="col100 align-right">' + self.renderPercentCustomCell(totalInfo.ChildInfo[3].Hdp) + '</div></td>\
                                        <td class="' + cssClass + '"><div class="col100 align-right">' + self.renderPercentCustomCell(totalInfo.ChildInfo[3].FifteenMinHdp) + '</div></td>\
                                        <td class="' + cssClass + '"><div class="col100 align-right">' + self.renderPercentCustomCell(totalInfo.ChildInfo[3].TenMinHdp) + '</div></td>\
                                        <td class="' + cssClass + '"><div class="col100 align-right">' + self.renderPercentCustomCell(totalInfo.ChildInfo[3].FirstHalfOU) + '</div></td>\
                                        <td class="' + cssClass + '"><div class="col100 align-right">' + self.renderPercentCustomCell(totalInfo.ChildInfo[3].OU) + '</div></td>\
                                        <td class="' + cssClass + '"><div class="col100 align-right">' + self.renderPercentCustomCell(totalInfo.ChildInfo[3].FifteenMinOU) + '</div></td>\
                                        <td class="' + cssClass + '"><div class="col100 align-right">' + self.renderPercentCustomCell(totalInfo.ChildInfo[3].TenMinOU) + '</div></td>\
                                        <td class="' + cssClass + '"><div class="col100 align-right">' + self.renderPercentCustomCell(totalInfo.ChildInfo[3].SingleTeamOU) + '</div></td>\
                                        <td class="' + cssClass + '"><div class="col100 align-right">' + self.renderPercentCustomCell(totalInfo.ChildInfo[3].FirstHalfNoOfCorners) + '</div></td>\
                                        <td class="' + cssClass + '"><div class="col100 align-right">' + self.renderPercentCustomCell(totalInfo.ChildInfo[3].NoOfCorners) + '</div></td>\
                                        <td class="' + cssClass + '"><div class="col100 align-right">' + self.renderPercentCustomCell(totalInfo.ChildInfo[3].FifteenMinNoOfCorners) + '</div></td>\
                                        <td class="' + cssClass + '"><div class="col100 align-right">' + self.renderPercentCustomCell(totalInfo.ChildInfo[3].NextCorner) + '</div></td>\
                                        <td class="' + cssClass + '"><div class="col100 align-right">' + self.renderPercentCustomCell(totalInfo.ChildInfo[3].FirstHalfTotalBookings) + '</div></td>\
                                        <td class="' + cssClass + '"><div class="col100 align-right">' + self.renderPercentCustomCell(totalInfo.ChildInfo[3].TotalBookings) + '</div></td>\
                                        <td class="' + cssClass + '"><div class="col100 align-right">' + self.renderPercentCustomCell(totalInfo.ChildInfo[3].TotalBookingsFifteenMinOU) + '</div></td>\
                                        <td class="' + cssClass + '"><div class="col100 align-right">' + self.renderPercentCustomCell(totalInfo.ChildInfo[3].NextBookings) + '</div></td>\
                                        <td class="' + cssClass + '"><div class="col100 align-right">' + self.renderPercentCustomCell(totalInfo.ChildInfo[3].FirstHalfOE) + '</div></td>\
                                        <td class="' + cssClass + '"><div class="col100 align-right">' + self.renderPercentCustomCell(totalInfo.ChildInfo[3].OE) + '</div></td>\
                                        <td class="' + cssClass + '"><div class="col100 align-right">' + self.renderPercentCustomCell(totalInfo.ChildInfo[3].Overall) + '</div></td>\
                                        <td class="' + cssClass + '"><div class="col100 align-right">' + self.renderDecimalNumberCustomCell(totalInfo.ChildInfo[4].FirstHalfOneXTwo) + '</div></td>\
                                        <td class="' + cssClass + '"><div class="col100 align-right">' + self.renderDecimalNumberCustomCell(totalInfo.ChildInfo[4].OneXTwo) + '</div></td>\
                                        <td class="' + cssClass + '"><div class="col100 align-right">' + self.renderDecimalNumberCustomCell(totalInfo.ChildInfo[4].FifteenMinOneXTwo) + '</div></td>\
                                        <td class="' + cssClass + '"><div class="col100 align-right">' + self.renderDecimalNumberCustomCell(totalInfo.ChildInfo[4].TenMinOneXTwo) + '</div></td>\
                                        <td class="' + cssClass + '"><div class="col100 align-right">' + self.renderDecimalNumberCustomCell(totalInfo.ChildInfo[4].FirstHalfHdp) + '</div></td>\
                                        <td class="' + cssClass + '"><div class="col100 align-right">' + self.renderDecimalNumberCustomCell(totalInfo.ChildInfo[4].Hdp) + '</div></td>\
                                        <td class="' + cssClass + '"><div class="col100 align-right">' + self.renderDecimalNumberCustomCell(totalInfo.ChildInfo[4].FifteenMinHdp) + '</div></td>\
                                        <td class="' + cssClass + '"><div class="col100 align-right">' + self.renderDecimalNumberCustomCell(totalInfo.ChildInfo[4].TenMinHdp) + '</div></td>\
                                        <td class="' + cssClass + '"><div class="col100 align-right">' + self.renderDecimalNumberCustomCell(totalInfo.ChildInfo[4].FirstHalfOU) + '</div></td>\
                                        <td class="' + cssClass + '"><div class="col100 align-right">' + self.renderDecimalNumberCustomCell(totalInfo.ChildInfo[4].OU) + '</div></td>\
                                        <td class="' + cssClass + '"><div class="col100 align-right">' + self.renderDecimalNumberCustomCell(totalInfo.ChildInfo[4].FifteenMinOU) + '</div></td>\
                                        <td class="' + cssClass + '"><div class="col100 align-right">' + self.renderDecimalNumberCustomCell(totalInfo.ChildInfo[4].TenMinOU) + '</div></td>\
                                        <td class="' + cssClass + '"><div class="col100 align-right">' + self.renderDecimalNumberCustomCell(totalInfo.ChildInfo[4].SingleTeamOU) + '</div></td>\
                                        <td class="' + cssClass + '"><div class="col100 align-right">' + self.renderDecimalNumberCustomCell(totalInfo.ChildInfo[4].FirstHalfNoOfCorners) + '</div></td>\
                                        <td class="' + cssClass + '"><div class="col100 align-right">' + self.renderDecimalNumberCustomCell(totalInfo.ChildInfo[4].NoOfCorners) + '</div></td>\
                                        <td class="' + cssClass + '"><div class="col100 align-right">' + self.renderDecimalNumberCustomCell(totalInfo.ChildInfo[4].FifteenMinNoOfCorners) + '</div></td>\
                                        <td class="' + cssClass + '"><div class="col100 align-right">' + self.renderDecimalNumberCustomCell(totalInfo.ChildInfo[4].NextCorner) + '</div></td>\
                                        <td class="' + cssClass + '"><div class="col100 align-right">' + self.renderDecimalNumberCustomCell(totalInfo.ChildInfo[4].FirstHalfTotalBookings) + '</div></td>\
                                        <td class="' + cssClass + '"><div class="col100 align-right">' + self.renderDecimalNumberCustomCell(totalInfo.ChildInfo[4].TotalBookings) + '</div></td>\
                                        <td class="' + cssClass + '"><div class="col100 align-right">' + self.renderDecimalNumberCustomCell(totalInfo.ChildInfo[4].TotalBookingsFifteenMinOU) + '</div></td>\
                                        <td class="' + cssClass + '"><div class="col100 align-right">' + self.renderDecimalNumberCustomCell(totalInfo.ChildInfo[4].NextBookings) + '</div></td>\
                                        <td class="' + cssClass + '"><div class="col100 align-right">' + self.renderDecimalNumberCustomCell(totalInfo.ChildInfo[4].FirstHalfOE) + '</div></td>\
                                        <td class="' + cssClass + '"><div class="col100 align-right">' + self.renderDecimalNumberCustomCell(totalInfo.ChildInfo[4].OE) + '</div></td>\
                                        <td class="' + cssClass + '"><div class="col100 align-right">' + self.renderDecimalNumberCustomCell(totalInfo.ChildInfo[4].Overall) + '</div></td>\
                                    </tr>';
    };

    self.renderDetailReportTotalRow = function (row, totalTypeTitle) {
        var frozenHtml = '';
        frozenHtml = '<th colspan="6"><div class="col585 align-right">' + totalTypeTitle + '</div></th>';

        _tempHtml.footerFrozenHtml += '<tr>' + frozenHtml + ' </tr>';

        _tempHtml.footerContentHtml += '<tr>' +
                                            frozenHtml +
                                        '<th><div class="col60 align-right"></div></th>\
                                        <th><div class="col60 align-right"></div></th>\
                                        <th><div class="col100 align-right">' + self.renderDecimalNumberCustomCell(row.ChildInfo[0].FirstHalfOneXTwo) + '</div></td>\
                                        <th><div class="col100 align-right">' + self.renderDecimalNumberCustomCell(row.ChildInfo[0].OneXTwo) + '</div></td>\
                                        <th><div class="col100 align-right">' + self.renderDecimalNumberCustomCell(row.ChildInfo[0].FifteenMinOneXTwo) + '</div></td>\
                                        <th><div class="col100 align-right">' + self.renderDecimalNumberCustomCell(row.ChildInfo[0].TenMinOneXTwo) + '</div></td>\
                                        <th><div class="col100 align-right">' + self.renderDecimalNumberCustomCell(row.ChildInfo[0].FirstHalfHdp) + '</div></td>\
                                        <th><div class="col100 align-right">' + self.renderDecimalNumberCustomCell(row.ChildInfo[0].Hdp) + '</div></td>\
                                        <th><div class="col100 align-right">' + self.renderDecimalNumberCustomCell(row.ChildInfo[0].FifteenMinHdp) + '</div></td>\
                                        <th><div class="col100 align-right">' + self.renderDecimalNumberCustomCell(row.ChildInfo[0].TenMinHdp) + '</div></td>\
                                        <th><div class="col100 align-right">' + self.renderDecimalNumberCustomCell(row.ChildInfo[0].FirstHalfOU) + '</div></th>\
                                        <th><div class="col100 align-right">' + self.renderDecimalNumberCustomCell(row.ChildInfo[0].OU) + '</div></th>\
                                        <th><div class="col100 align-right">' + self.renderDecimalNumberCustomCell(row.ChildInfo[0].FifteenMinOU) + '</div></th>\
                                        <th><div class="col100 align-right">' + self.renderDecimalNumberCustomCell(row.ChildInfo[0].TenMinOU) + '</div></th>\
                                        <th><div class="col100 align-right">' + self.renderDecimalNumberCustomCell(row.ChildInfo[0].SingleTeamOU) + '</div></th>\
                                        <th><div class="col100 align-right">' + self.renderDecimalNumberCustomCell(row.ChildInfo[0].FirstHalfNoOfCorners) + '</div></th>\
                                        <th><div class="col100 align-right">' + self.renderDecimalNumberCustomCell(row.ChildInfo[0].NoOfCorners) + '</div></th>\
                                        <th><div class="col100 align-right">' + self.renderDecimalNumberCustomCell(row.ChildInfo[0].FifteenMinNoOfCorners) + '</div></th>\
                                        <th><div class="col100 align-right">' + self.renderDecimalNumberCustomCell(row.ChildInfo[0].NextCorner) + '</div></th>\
                                        <th><div class="col100 align-right">' + self.renderDecimalNumberCustomCell(row.ChildInfo[0].FirstHalfTotalBookings) + '</div></th>\
                                        <th><div class="col100 align-right">' + self.renderDecimalNumberCustomCell(row.ChildInfo[0].TotalBookings) + '</div></th>\
                                        <th><div class="col100 align-right">' + self.renderDecimalNumberCustomCell(row.ChildInfo[0].TotalBookingsFifteenMinOU) + '</div></th>\
                                        <th><div class="col100 align-right">' + self.renderDecimalNumberCustomCell(row.ChildInfo[0].NextBookings) + '</div></th>\
                                        <th><div class="col100 align-right">' + self.renderDecimalNumberCustomCell(row.ChildInfo[0].FirstHalfOE) + '</div></th>\
                                        <th><div class="col100 align-right">' + self.renderDecimalNumberCustomCell(row.ChildInfo[0].OE) + '</div></th>\
                                        <th><div class="col100 align-right">' + self.renderDecimalNumberCustomCell(row.ChildInfo[0].Overall) + '</div></th>\
                                        <th><div class="col100 align-right">' + self.renderDecimalNumberCustomCell(row.ChildInfo[1].FirstHalfOneXTwo) + '</div></th>\
                                        <th><div class="col100 align-right">' + self.renderDecimalNumberCustomCell(row.ChildInfo[1].OneXTwo) + '</div></th>\
                                        <th><div class="col100 align-right">' + self.renderDecimalNumberCustomCell(row.ChildInfo[1].FifteenMinOneXTwo) + '</div></th>\
                                        <th><div class="col100 align-right">' + self.renderDecimalNumberCustomCell(row.ChildInfo[1].TenMinOneXTwo) + '</div></th>\
                                        <th><div class="col100 align-right">' + self.renderDecimalNumberCustomCell(row.ChildInfo[1].FirstHalfHdp) + '</div></th>\
                                        <th><div class="col100 align-right">' + self.renderDecimalNumberCustomCell(row.ChildInfo[1].Hdp) + '</div></th>\
                                        <th><div class="col100 align-right">' + self.renderDecimalNumberCustomCell(row.ChildInfo[1].FifteenMinHdp) + '</div></th>\
                                        <th><div class="col100 align-right">' + self.renderDecimalNumberCustomCell(row.ChildInfo[1].TenMinHdp) + '</div></th>\
                                        <th><div class="col100 align-right">' + self.renderDecimalNumberCustomCell(row.ChildInfo[1].FirstHalfOU) + '</div></th>\
                                        <th><div class="col100 align-right">' + self.renderDecimalNumberCustomCell(row.ChildInfo[1].OU) + '</div></th>\
                                        <th><div class="col100 align-right">' + self.renderDecimalNumberCustomCell(row.ChildInfo[1].FifteenMinOU) + '</div></th>\
                                        <th><div class="col100 align-right">' + self.renderDecimalNumberCustomCell(row.ChildInfo[1].TenMinOU) + '</div></th>\
                                        <th><div class="col100 align-right">' + self.renderDecimalNumberCustomCell(row.ChildInfo[1].SingleTeamOU) + '</div></th>\
                                        <th><div class="col100 align-right">' + self.renderDecimalNumberCustomCell(row.ChildInfo[1].FirstHalfNoOfCorners) + '</div></th>\
                                        <th><div class="col100 align-right">' + self.renderDecimalNumberCustomCell(row.ChildInfo[1].NoOfCorners) + '</div></th>\
                                        <th><div class="col100 align-right">' + self.renderDecimalNumberCustomCell(row.ChildInfo[1].FifteenMinNoOfCorners) + '</div></th>\
                                        <th><div class="col100 align-right">' + self.renderDecimalNumberCustomCell(row.ChildInfo[1].NextCorner) + '</div></th>\
                                        <th><div class="col100 align-right">' + self.renderDecimalNumberCustomCell(row.ChildInfo[1].FirstHalfTotalBookings) + '</div></th>\
                                        <th><div class="col100 align-right">' + self.renderDecimalNumberCustomCell(row.ChildInfo[1].TotalBookings) + '</div></th>\
                                        <th><div class="col100 align-right">' + self.renderDecimalNumberCustomCell(row.ChildInfo[1].TotalBookingsFifteenMinOU) + '</div></th>\
                                        <th><div class="col100 align-right">' + self.renderDecimalNumberCustomCell(row.ChildInfo[1].NextBookings) + '</div></th>\
                                        <th><div class="col100 align-right">' + self.renderDecimalNumberCustomCell(row.ChildInfo[1].FirstHalfOE) + '</div></th>\
                                        <th><div class="col100 align-right">' + self.renderDecimalNumberCustomCell(row.ChildInfo[1].OE) + '</div></th>\
                                        <th><div class="col100 align-right">' + self.renderDecimalNumberCustomCell(row.ChildInfo[1].Overall) + '</div></th>\
                                        <th><div class="col100 align-right">' + numeral(row.ChildInfo[2].FirstHalfOneXTwo).format(FormatConstant.INT_NUMBER) + '</div></th>\
                                        <th><div class="col100 align-right">' + numeral(row.ChildInfo[2].OneXTwo).format(FormatConstant.INT_NUMBER) + '</div></th>\
                                        <th><div class="col100 align-right">' + numeral(row.ChildInfo[2].FifteenMinOneXTwo).format(FormatConstant.INT_NUMBER) + '</div></th>\
                                        <th><div class="col100 align-right">' + numeral(row.ChildInfo[2].TenMinOneXTwo).format(FormatConstant.INT_NUMBER) + '</div></th>\
                                        <th><div class="col100 align-right">' + numeral(row.ChildInfo[2].FirstHalfHdp).format(FormatConstant.INT_NUMBER) + '</div></th>\
                                        <th><div class="col100 align-right">' + numeral(row.ChildInfo[2].Hdp).format(FormatConstant.INT_NUMBER) + '</div></th>\
                                        <th><div class="col100 align-right">' + numeral(row.ChildInfo[2].FifteenMinHdp).format(FormatConstant.INT_NUMBER) + '</div></th>\
                                        <th><div class="col100 align-right">' + numeral(row.ChildInfo[2].TenMinHdp).format(FormatConstant.INT_NUMBER) + '</div></th>\
                                        <th class="turnOverGroup"><div class="col100 align-right">' + numeral(row.ChildInfo[2].FirstHalfOU).format(FormatConstant.INT_NUMBER) + '</div></th>\
                                        <th><div class="col100 align-right">' + numeral(row.ChildInfo[2].OU).format(FormatConstant.INT_NUMBER) + '</div></th>\
                                        <th><div class="col100 align-right">' + numeral(row.ChildInfo[2].FifteenMinOU).format(FormatConstant.INT_NUMBER) + '</div></th>\
                                        <th><div class="col100 align-right">' + numeral(row.ChildInfo[2].TenMinOU).format(FormatConstant.INT_NUMBER) + '</div></th>\
                                        <th><div class="col100 align-right">' + numeral(row.ChildInfo[2].SingleTeamOU).format(FormatConstant.INT_NUMBER) + '</div></th>\
                                        <th><div class="col100 align-right">' + numeral(row.ChildInfo[2].FirstHalfNoOfCorners).format(FormatConstant.INT_NUMBER) + '</div></th>\
                                        <th><div class="col100 align-right">' + numeral(row.ChildInfo[2].NoOfCorners).format(FormatConstant.INT_NUMBER) + '</div></th>\
                                        <th><div class="col100 align-right">' + numeral(row.ChildInfo[2].FifteenMinNoOfCorners).format(FormatConstant.INT_NUMBER) + '</div></th>\
                                        <th><div class="col100 align-right">' + numeral(row.ChildInfo[2].NextCorner).format(FormatConstant.INT_NUMBER) + '</div></th>\
                                        <th><div class="col100 align-right">' + numeral(row.ChildInfo[2].FirstHalfTotalBookings).format(FormatConstant.INT_NUMBER) + '</div></th>\
                                        <th><div class="col100 align-right">' + numeral(row.ChildInfo[2].TotalBookings).format(FormatConstant.INT_NUMBER) + '</div></th>\
                                        <th><div class="col100 align-right">' + numeral(row.ChildInfo[2].TotalBookingsFifteenMinOU).format(FormatConstant.INT_NUMBER) + '</div></th>\
                                        <th><div class="col100 align-right">' + numeral(row.ChildInfo[2].NextBookings).format(FormatConstant.INT_NUMBER) + '</div></th>\
                                        <th><div class="col100 align-right">' + numeral(row.ChildInfo[2].FirstHalfOE).format(FormatConstant.INT_NUMBER) + '</div></th>\
                                        <th><div class="col100 align-right">' + numeral(row.ChildInfo[2].OE).format(FormatConstant.INT_NUMBER) + '</div></th>\
                                        <th><div class="col100 align-right">' + numeral(row.ChildInfo[2].Overall).format(FormatConstant.INT_NUMBER) + '</div></th>\
                                        <th><div class="col100 align-right">' + self.renderPercentCustomCell(row.ChildInfo[3].FirstHalfOneXTwo) + '</div></th>\
                                        <th><div class="col100 align-right">' + self.renderPercentCustomCell(row.ChildInfo[3].OneXTwo) + '</div></th>\
                                        <th><div class="col100 align-right">' + self.renderPercentCustomCell(row.ChildInfo[3].FifteenMinOneXTwo) + '</div></th>\
                                        <th><div class="col100 align-right">' + self.renderPercentCustomCell(row.ChildInfo[3].TenMinOneXTwo) + '</div></th>\
                                        <th><div class="col100 align-right">' + self.renderPercentCustomCell(row.ChildInfo[3].FirstHalfHdp) + '</div></th>\
                                        <th><div class="col100 align-right">' + self.renderPercentCustomCell(row.ChildInfo[3].Hdp) + '</div></th>\
                                        <th><div class="col100 align-right">' + self.renderPercentCustomCell(row.ChildInfo[3].FifteenMinHdp) + '</div></th>\
                                        <th><div class="col100 align-right">' + self.renderPercentCustomCell(row.ChildInfo[3].TenMinHdp) + '</div></th>\
                                        <th><div class="col100 align-right">' + self.renderPercentCustomCell(row.ChildInfo[3].FirstHalfOU) + '</div></th>\
                                        <th><div class="col100 align-right">' + self.renderPercentCustomCell(row.ChildInfo[3].OU) + '</div></th>\
                                        <th><div class="col100 align-right">' + self.renderPercentCustomCell(row.ChildInfo[3].FifteenMinOU) + '</div></th>\
                                        <th><div class="col100 align-right">' + self.renderPercentCustomCell(row.ChildInfo[3].TenMinOU) + '</div></th>\
                                        <th><div class="col100 align-right">' + self.renderPercentCustomCell(row.ChildInfo[3].SingleTeamOU) + '</div></th>\
                                        <th><div class="col100 align-right">' + self.renderPercentCustomCell(row.ChildInfo[3].FirstHalfNoOfCorners) + '</div></th>\
                                        <th><div class="col100 align-right">' + self.renderPercentCustomCell(row.ChildInfo[3].NoOfCorners) + '</div></th>\
                                        <th><div class="col100 align-right">' + self.renderPercentCustomCell(row.ChildInfo[3].FifteenMinNoOfCorners) + '</div></th>\
                                        <th><div class="col100 align-right">' + self.renderPercentCustomCell(row.ChildInfo[3].NextCorner) + '</div></th>\
                                        <th><div class="col100 align-right">' + self.renderPercentCustomCell(row.ChildInfo[3].FirstHalfTotalBookings) + '</div></th>\
                                        <th><div class="col100 align-right">' + self.renderPercentCustomCell(row.ChildInfo[3].TotalBookings) + '</div></th>\
                                        <th><div class="col100 align-right">' + self.renderPercentCustomCell(row.ChildInfo[3].TotalBookingsFifteenMinOU) + '</div></th>\
                                        <th><div class="col100 align-right">' + self.renderPercentCustomCell(row.ChildInfo[3].NextBookings) + '</div></th>\
                                        <th><div class="col100 align-right">' + self.renderPercentCustomCell(row.ChildInfo[3].FirstHalfOE) + '</div></th>\
                                        <th><div class="col100 align-right">' + self.renderPercentCustomCell(row.ChildInfo[3].OE) + '</div></th>\
                                        <th><div class="col100 align-right">' + self.renderPercentCustomCell(row.ChildInfo[3].Overall) + '</div></th>\
                                        <th><div class="col100 align-right">' + self.renderDecimalNumberCustomCell(row.ChildInfo[4].FirstHalfOneXTwo) + '</div></th>\
                                        <th><div class="col100 align-right">' + self.renderDecimalNumberCustomCell(row.ChildInfo[4].OneXTwo) + '</div></th>\
                                        <th><div class="col100 align-right">' + self.renderDecimalNumberCustomCell(row.ChildInfo[4].FifteenMinOneXTwo) + '</div></th>\
                                        <th><div class="col100 align-right">' + self.renderDecimalNumberCustomCell(row.ChildInfo[4].TenMinOneXTwo) + '</div></th>\
                                        <th><div class="col100 align-right">' + self.renderDecimalNumberCustomCell(row.ChildInfo[4].FirstHalfHdp) + '</div></th>\
                                        <th><div class="col100 align-right">' + self.renderDecimalNumberCustomCell(row.ChildInfo[4].Hdp) + '</div></th>\
                                        <th><div class="col100 align-right">' + self.renderDecimalNumberCustomCell(row.ChildInfo[4].FifteenMinHdp) + '</div></th>\
                                        <th><div class="col100 align-right">' + self.renderDecimalNumberCustomCell(row.ChildInfo[4].TenMinHdp) + '</div></th>\
                                        <th><div class="col100 align-right">' + self.renderDecimalNumberCustomCell(row.ChildInfo[4].FirstHalfOU) + '</div></th>\
                                        <th><div class="col100 align-right">' + self.renderDecimalNumberCustomCell(row.ChildInfo[4].OU) + '</div></th>\
                                        <th><div class="col100 align-right">' + self.renderDecimalNumberCustomCell(row.ChildInfo[4].FifteenMinOU) + '</div></th>\
                                        <th><div class="col100 align-right">' + self.renderDecimalNumberCustomCell(row.ChildInfo[4].TenMinOU) + '</div></th>\
                                        <th><div class="col100 align-right">' + self.renderDecimalNumberCustomCell(row.ChildInfo[4].SingleTeamOU) + '</div></th>\
                                        <th><div class="col100 align-right">' + self.renderDecimalNumberCustomCell(row.ChildInfo[4].FirstHalfNoOfCorners) + '</div></th>\
                                        <th><div class="col100 align-right">' + self.renderDecimalNumberCustomCell(row.ChildInfo[4].NoOfCorners) + '</div></th>\
                                        <th><div class="col100 align-right">' + self.renderDecimalNumberCustomCell(row.ChildInfo[4].FifteenMinNoOfCorners) + '</div></th>\
                                        <th><div class="col100 align-right">' + self.renderDecimalNumberCustomCell(row.ChildInfo[4].NextCorner) + '</div></th>\
                                        <th><div class="col100 align-right">' + self.renderDecimalNumberCustomCell(row.ChildInfo[4].FirstHalfTotalBookings) + '</div></th>\
                                        <th><div class="col100 align-right">' + self.renderDecimalNumberCustomCell(row.ChildInfo[4].TotalBookings) + '</div></th>\
                                        <th><div class="col100 align-right">' + self.renderDecimalNumberCustomCell(row.ChildInfo[4].TotalBookingsFifteenMinOU) + '</div></th>\
                                        <th><div class="col100 align-right">' + self.renderDecimalNumberCustomCell(row.ChildInfo[4].NextBookings) + '</div></th>\
                                        <th><div class="col100 align-right">' + self.renderDecimalNumberCustomCell(row.ChildInfo[4].FirstHalfOE) + '</div></th>\
                                        <th><div class="col100 align-right">' + self.renderDecimalNumberCustomCell(row.ChildInfo[4].OE) + '</div></th>\
                                        <th><div class="col100 align-right">' + self.renderDecimalNumberCustomCell(row.ChildInfo[4].Overall) + '</div></th>\
                                    </tr>';
    };

    var initPager = function (pagingCriteria, reportHandler) {
        _param.reportDetailParams.TotalItems = pagingCriteria.totalItems;
        _options.paging.currentPage = pagingCriteria.currentPage;
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
            pageIndex: parseInt(_param.reportDetailParams.PageIndex),
            totalItems: parseInt(_param.reportDetailParams.TotalItems),
            pageSize: parseInt(_param.reportDetailParams.PageSize)
        };

        if (oldPagingParams.pageSize != itemsPerPage) {
            _options.paging.currentPage = 0;
            _param.reportDetailParams.PageIndex = 1;
        }
        else if (page_index * itemsPerPage <= oldPagingParams.totalItems) {
            _options.paging.currentPage = page_index;
            _param.reportDetailParams.PageIndex = page_index + 1;
        }

        _options.paging.itemsPerPage = itemsPerPage;
        _param.reportDetailParams.PageSize = itemsPerPage;

        if (!(oldPagingParams.totalItems <= oldPagingParams.pageSize &&
           oldPagingParams.pageIndex == 1 && oldPagingParams.pageSize <= itemsPerPage)) {
            _param.reportDetailParams.IsPagingCallBack = true;
            self.getTraderPerformanceSoccerDetailReport();
        }

        return false;
    };
};

$(document).ready(function () {
    var traderPerformanceSoccerReport = new Fanex.AccountingReports.Main.TraderPerformanceSoccer();
    traderPerformanceSoccerReport.initPage();
});