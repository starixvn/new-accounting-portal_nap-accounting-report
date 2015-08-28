
Namespace.Register("Fanex.AccountingReports.Misc.BetCount.HistoryReport");
Fanex.AccountingReports.Misc.BetCount.HistoryReport = function () {
    var self = this;

    var _tables = {
        reportFilter: null,
        ddlProduct: null,
        btnGenerate: null,
        btnBack: null,
        monthPicker: null,

        report: null,
        reportContent: null,
        reportTitle: null,
        reportExportExcel: null,
        reportRowHeader: null,
        reportRowFooter: null,
        reportColFooter: null,
        reportPagination: null,
        reportSorter: null
    };

    var _options = {
        report: {
            isShowPager: false,
            tblContentId: 'historyBetCountReportContent',
            manualCopyHeader: true,
            fullHeight: true,
            fullWidth: true,
            gridWidth: 1230,
            gridHeight: 195,
            freezeWidth: 80,
            freezeHeight: 280,
            numberFreezeCol: 1
        }
    };

    var _params = {
        historyBetCountReport: {
            FromDate: null,
            ToDate: null,
            Products: null,
            SortBy: null,
            SortDirection: null
        }
    };

    var _cachedData = {
        products: null,
        historyBetCountReport: null,
        isFirstLoad: true,
        lastSorterBy: null
    };

    self.initPage = function () {
        initGridScrollerTables();
        cachedDOMElements();
        bindingEvent();
    };
    var initGridScrollerTables = function () {
        $("#historyBetCountReport").initGrid({
            isShowPager: false
        });
    };
    var cachedDOMElements = function () {
        _tables.reportFilter = $("#historyBetCountReportFilter");
        _tables.btnGenerate = $("#btnGenerate_HistoryReport");

        _tables.report = $("#historyBetCountReport");
        _tables.reportHeader = $("#historyBetCountReportHeader");
        _tables.reportContent = _tables.report.find("#historyBetCountReportContent tbody");
        _tables.reportTitle = _tables.reportHeader.find("#historyBetCountReportTitle");
        _tables.reportExportExcel = _tables.reportHeader.find("#historyBetCountReport_ExportExcel");
        _tables.reportRowHeader = _tables.report.find("#historyBetCountReportRowHeader tbody");
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
            if (_cachedData.historyBetCountReport &&
                _cachedData.historyBetCountReport.TransactionList.length > 0) {
                AccReport.gridSorterManager.sortColumn($(this).find(".divSorter"), self.sortReport);
            }
        });
    };
    self.renderDropdownProduct = function (products) {
        _cachedData.products = products;
        var noItemText = 'No product found.';
        var len = products.length;
        var ddwnProducts = [];
        var prefixId = 'ddlProduct_HistoryReport_';
        for (var i = 0; i < len; i++) {
            var product = products[i];
            var checked = product.Id == 1 ? true : false;
            ddwnProducts.push({
                Id: prefixId + product.Id,
                Value: product.Id,
                Text: product.Name,
                Checked: checked,
                Enable: true,
                CssClass: "",
                Items: []
            });
        }

        $("#ddlProduct_HistoryReport").multiSelect(
        {
            minWidth: 152,
            maxHeight: 450,
            shadow: true,
            showHeader: true,
            isCheckAll: false,
            showFooter: false,
            checkAllText: 'All Products',
            isUsedCustomCheckAllText: true,
            customCheckAllText: 'All Products',
            noItemText: noItemText,
            selectedList: 3,
            listData: ddwnProducts
        });

        _tables.ddlProduct = $("#ddlProduct_HistoryReport");
    };
    self.btnGenerateClick = function () {
        AccReport.gridSorterManager.resetColumns(_tables.reportDivSorter);
        self.getReport();
    };
    var renderMonthPicker = function () {
        if (_cachedData.isFirstLoad) {
            $('#ddlMonth').monPickr({
                today: today,
                maxDate: today,
                minDate: minHistoryReportDate,
                showDateRange: true,
                fromDayId: 'ddlFromDay',
                toDayId: 'ddlToDay',
                sortDesc: true,
                log: function (msg) {
                    console.warn(msg);
                }
            });

            _tables.monthPicker = $('#ddlMonth');
            _cachedData.isFirstLoad = false;
        }
        else {
            _tables.monthPicker.monPickr("reset");
        }
    };

    self.getReport = function () {
        AccReport.disableInput();
        getParams();
        if (validateParams() == true) {
            $.ajax({
                type: "POST",
                url: window.rootUrl + "Misc/BetCount/GetReport",
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                data: JSON.stringify({
                    Datestart: _params.historyBetCountReport.FromDate,
                    Dateend: _params.historyBetCountReport.ToDate,
                    Product: _params.historyBetCountReport.Products,
                    IsHistoryReport: true,
                    SortBy: _params.historyBetCountReport.SortBy,
                    SortDirection: ''
                }),
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
        _params.historyBetCountReport.FromDate = _tables.monthPicker.monPickr("getFromDate", FormatConstant.MMM_DD_YY_STRING_MONTH_PICKER);
        _params.historyBetCountReport.ToDate = _tables.monthPicker.monPickr("getToDate", FormatConstant.MMM_DD_YY_STRING_MONTH_PICKER);
        _params.historyBetCountReport.Products = _tables.ddlProduct.multiSelect("getCheckedItems").join(',');
        _params.historyBetCountReport.SortBy = '';
    };
    var validateParams = function () {
        var isValid = true;
        var message = '';
        var notifyIds = [];
        var diffTime = moment(_params.historyBetCountReport.FromDate).diff(moment(_params.historyBetCountReport.ToDate));
        if (diffTime > 0) {
            if (message == '') {
                message = "From date must less than to date!";
            }
            isValid = false;
        }

        if (_params.historyBetCountReport.Products.length == 0) {
            if (message == '') {
                message = "Please select at least one product!";
            }
            notifyIds.push("ddlProduct_HistoryReport");
            isValid = false;
        }

        if (!isValid) {
            AccReport.popUp.removeHighLight();
            AccReport.popUp.show(message, notifyIds.join(","));
        } else {
            AccReport.popUp.removeHighLight();
        }

        return isValid;
    };
    self.renderReport = function (report) {
        _cachedData.historyBetCountReport = report;

        var reportTitle = "Bet count report from " + _params.historyBetCountReport.FromDate +
                                          " to " + _params.historyBetCountReport.ToDate;

        _tables.reportTitle.html(reportTitle);

        if (report &&
            report.TransactionList &&
            report.TransactionList.length > 0) {
            _tables.reportExportExcel.removeClass("display-none");
            _tables.reportDivSorter.addClass("divSorter");
            self.resetTempHtml();
            var frozenHtml = '';

            var siteGroupNumber = report.TransactionList.length;
            for (var i = 0; i < siteGroupNumber; i++) {
                var siteGroup = report.TransactionList[i];
                var transactionLength = siteGroup.TransactionList.length;
                var siteGroupTotal = siteGroup.Total;
                self.renderReportRow(siteGroupTotal, true);

                for (var j = 0; j < transactionLength; j++) {
                    var reportItem = siteGroup.TransactionList[j];
                    self.renderReportRow(reportItem);
                }
            }

            self.renderReportTotalRow(report.Total);
            self.removeSpaceIE9TempHtml();
            _tables.reportRowHeader.empty().append(self.TempHtml.frozenHtml);
            _tables.reportContent.empty().append(self.TempHtml.contentHtml);
            _tables.reportRowFooter.empty().append(self.TempHtml.footerFrozenHtml);
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
    self.renderReportNoData = function () {
        _tables.reportExportExcel.addClass("display-none");
        _tables.reportDivSorter.removeClass("divSorter");
        var frezeeContent = "<td><div>&nbsp</div></td>";
        _tables.reportRowHeader.empty().append("<tr>" + frezeeContent + "</tr>");
        _tables.reportContent.empty().append(
                "<tr>" +
                    frezeeContent +
                    "<td colspan='9'><div class='no-data-message'>There is no data.</div></td>\
                 </tr>");
        _tables.reportRowFooter.empty();
        _tables.reportColFooter.empty();
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
            case 'colHeaderTotalTurnover_HistoryReport':
                sortBy = self.Enums.sortBy.TotalTurnover;
                break;
            case 'colHeaderMemberWinLoss_HistoryReport':
                sortBy = self.Enums.sortBy.MemberWinLoss;
                break;
            case 'colHeaderMarginPercent_HistoryReport':
                sortBy = self.Enums.sortBy.MarginPercent;
                break;
            case 'colHeaderAgentWinLoss_HistoryReport':
                sortBy = self.Enums.sortBy.AgentWinLoss;
                break;
            case 'colHeaderMasterWinLoss_HistoryReport':
                sortBy = self.Enums.sortBy.MasterWinLoss;
                break;
            case 'colHeaderSuperWinLoss_HistoryReport':
                sortBy = self.Enums.sortBy.SuperWinLoss;
                break;
            case 'colHeaderCompanyWinLoss_HistoryReport':
                sortBy = self.Enums.sortBy.CompanyWinLoss;
                break;
        }

        _params.historyBetCountReport.SortBy = sortBy;
        _params.historyBetCountReport.SortDirection = newSortDirection;

        $.ajax({
            type: "POST",
            url: window.rootUrl + "Misc/BetCount/SortReport",
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            data: JSON.stringify({
                Datestart: _params.historyBetCountReport.FromDate,
                Dateend: _params.historyBetCountReport.ToDate,
                Product: _params.historyBetCountReport.Products,
                IsHistoryReport: true,
                SortBy: sortBy,
                SortDirection: newSortDirection
            }),
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
            Datestart: _params.historyBetCountReport.FromDate,
            Dateend: _params.historyBetCountReport.ToDate,
            Product: _params.historyBetCountReport.Products,
            IsHistoryReport: true,
            SortBy: _params.historyBetCountReport.SortBy,
            SortDirection: ''
        };

        AccReport.downloadFile(window.rootUrl + "Misc/BetCount/ExportReport", exportParams);
    };
    self.showPage = function () {
        AccReport.popUp.removeHighLight();
        _tables.reportFilter.removeClass("display-none");
        renderMonthPicker();
        self.renderDropdownProduct(_cachedData.products);
        AccReport.gridSorterManager.resetColumns(_tables.reportSorter);
        if (_cachedData.historyBetCountReport &&
            _cachedData.historyBetCountReport.TransactionList.length > 0) {
            _tables.reportHeader.removeClass("display-none");
            _tables.report.removeClass("display-none");
        }
    };
    self.hidePage = function () {
        AccReport.popUp.removeHighLight();
        _tables.reportFilter.addClass("display-none");
        _tables.reportHeader.addClass("display-none");
        _tables.report.addClass("display-none");
    };
}
