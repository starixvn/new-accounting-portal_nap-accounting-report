
Namespace.Register("Fanex.AccountingReports.Misc.BetCount.NormalReport");
Fanex.AccountingReports.Misc.BetCount.NormalReport = function () {
    var self = this;

    var _tables = {
        reportFilter: null,
        filterNote: null,
        ddlProduct: null,
        datepickerplugin: null,
        btnGenerate: null,

        report: null,
        reportContent: null,
        reportTitle: null,
        reportExportExcel: null,
        reportRowHeader: null,
        reportRowFooter: null,
        reportColFooter: null,
        reportPagination: null,
        reportSorter: null,
        reportDivSorter: null
    };

    var _options = {
        filterNote: {
            html: true,
            opacity: 0.9,
            gravity: 'ne'
        },

        report: {
            isShowPager: false,
            tblContentId: 'betCountReportContent',
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
        betCountReport: {
            FromDate: null,
            ToDate: null,
            Products: null,
            SortBy: null,
            SortDirection: null
        }
    };

    var _cachedData = {
        betCountReport: null,
        lastSorterBy: null
    };

    self.initPage = function () {
        initGridScrollerTables();
        cachedDOMElements();
        renderDatePicker();
        renderFilterNote(noteMessage);
        bindingEvent();
    };
    var initGridScrollerTables = function () {
        $("#betCountReport").initGrid({
            isShowPager: false
        });
    };
    var cachedDOMElements = function () {
        _tables.filterNote = $("#notemessage");
        _tables.btnGenerate = $("#btnGenerate");
        _tables.reportFilter = $("#betCountReportFilter");

        _tables.report = $("#betCountReport");
        _tables.reportHeader = $("#betCountReportHeader");
        _tables.reportContent = _tables.report.find("#betCountReportContent tbody");
        _tables.reportTitle = _tables.reportHeader.find("#betCountReportTitle");
        _tables.reportExportExcel = _tables.reportHeader.find("#betCountReport_ExportExcel");
        _tables.reportRowHeader = _tables.report.find("#betCountReportRowHeader tbody");
        _tables.reportRowFooter = _tables.report.find(".row_footer_content thead");
        _tables.reportColFooter = _tables.report.find(".col_footer_content thead");
        _tables.reportPagination = _tables.report.find("#pagination");
        _tables.reportSorter = _tables.report.find(".colSorter");
        _tables.reportDivSorter = _tables.report.find(".divSorter");
    };
    var renderFilterNote = function (noteMessage) {
        //// Assign Title for tooltip
        _tables.filterNote.attr("title", noteMessage);
        _tables.filterNote.tipsy(_options.filterNote);
    };
    var renderDatePicker = function () {
        $('#datepickerplugin').nexDatePicker({
            nexDatePickerType: "quickchoose",
            datePickerFormat: {
                changeMonth: true,
                changeYear: true,
                dateFormat: FormatConstant.M_DD_YY_STRING,
                minDate: mindate
            },
            dropDownList:
            {
                Id: "dropDownListId",
                Label: "Date ",
                LabelCss: "lbl-field col_2",
                OptionCss: "edit-field col_6"
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
            selectedItem: 1,
            onSelectDatePicker: function () {
            },
            maxDate: new Date(today)
        });

        _tables.datepickerplugin = $("#datepickerplugin");
    };
    self.renderDropdownProduct = function (products) {
        var noItemText = 'No product found.';
        var len = products.length;
        var ddwnProducts = [];
        var prefixId = 'ddlProduct_';
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

        $("#ddlProduct").multiSelect(
        {
            minWidth: 140,
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

        _tables.ddlProduct = $("#ddlProduct");
    };
    var bindingEvent = function () {
        _tables.btnGenerate.click(function () {
            self.btnGenerateClick();
        });

        _tables.reportExportExcel.click(function () {
            exportReport();
        });

        _tables.reportSorter.click(function () {
            if (_cachedData.betCountReport &&
                _cachedData.betCountReport.TransactionList.length > 0) {
                AccReport.gridSorterManager.sortColumn($(this).find(".divSorter"), self.sortReport);
            }
        });
    };
    self.btnGenerateClick = function () {
        AccReport.gridSorterManager.resetColumns(_tables.reportDivSorter);
        self.getReport();
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
                    Datestart: _params.betCountReport.FromDate,
                    Dateend: _params.betCountReport.ToDate,
                    Product: _params.betCountReport.Products,
                    IsHistoryReport: false,
                    SortBy: _params.betCountReport.SortBy,
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
        var dateRange = _tables.datepickerplugin.nexDatePicker("getDateRange");
        _params.betCountReport.FromDate = dateRange.FromDate;
        _params.betCountReport.ToDate = dateRange.ToDate;
        _params.betCountReport.Products = _tables.ddlProduct.multiSelect("getCheckedItems").join(',');
        _params.betCountReport.SortBy = '';
    };
    var validateParams = function () {
        var isValid = true;
        var message = '';
        var notifyIds = [];
        if (_params.betCountReport.Products.length == 0) {
            if (message == '') {
                message = "Please select at least one product!";
            }
            notifyIds.push("ddlProduct");
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
        _cachedData.betCountReport = report;

        var reportTitle = "Bet count report from " + _params.betCountReport.FromDate +
                                          " to " + _params.betCountReport.ToDate;

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
            case 'colHeaderTotalTurnover':
                sortBy = self.Enums.sortBy.TotalTurnover;
                break;
            case 'colHeaderMemberWinLoss':
                sortBy = self.Enums.sortBy.MemberWinLoss;
                break;
            case 'colHeaderMarginPercent':
                sortBy = self.Enums.sortBy.MarginPercent;
                break;
            case 'colHeaderAgentWinLoss':
                sortBy = self.Enums.sortBy.AgentWinLoss;
                break;
            case 'colHeaderMasterWinLoss':
                sortBy = self.Enums.sortBy.MasterWinLoss;
                break;
            case 'colHeaderSuperWinLoss':
                sortBy = self.Enums.sortBy.SuperWinLoss;
                break;
            case 'colHeaderCompanyWinLoss':
                sortBy = self.Enums.sortBy.CompanyWinLoss;
                break;
        }

        _params.betCountReport.SortBy = sortBy;
        _params.betCountReport.SortDirection = newSortDirection;

        $.ajax({
            type: "POST",
            url: window.rootUrl + "Misc/BetCount/SortReport",
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            data: JSON.stringify({
                Datestart: _params.betCountReport.FromDate,
                Dateend: _params.betCountReport.ToDate,
                Product: _params.betCountReport.Products,
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
            Datestart: _params.betCountReport.FromDate,
            Dateend: _params.betCountReport.ToDate,
            Product: _params.betCountReport.Products,
            IsHistoryReport: false,
            SortBy: _params.betCountReport.SortBy,
            SortDirection: ''
        };

        AccReport.downloadFile(window.rootUrl + "Misc/BetCount/ExportReport", exportParams);
    };
    self.hidePage = function () {
        AccReport.popUp.removeHighLight();
        _tables.reportFilter.addClass("display-none");
        _tables.filterNote.addClass("display-none");
        _tables.reportHeader.addClass("display-none");
        _tables.report.addClass("display-none");
    };
    self.showPage = function () {
        AccReport.popUp.removeHighLight();
        _tables.reportFilter.removeClass("display-none");
        _tables.filterNote.removeClass("display-none");
        if (_cachedData.betCountReport &&
            _cachedData.betCountReport.TransactionList.length > 0) {
            _tables.reportHeader.removeClass("display-none");
            _tables.report.removeClass("display-none");
        }
    };
}