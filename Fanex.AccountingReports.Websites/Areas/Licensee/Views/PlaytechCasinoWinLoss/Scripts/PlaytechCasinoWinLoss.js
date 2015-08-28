Namespace.Register("Fanex.AccountingReports.Licensee.PlaytechCasinoWinLossReport");

Fanex.AccountingReports.Licensee.PlaytechCasinoWinLossReport = function () {
    var self = this;

    var _tables = {
        btnGenerate: null,
        dropdownLicensee: null,
        dropdownCurrency: null,
        reportBy: null,
        datepickerplugin: null,
        currencyConvert: null,
        filterNote: null,
        chkExcludeTestData: null,
        reportContainer: null,
        reportContent: null
    };

    var _options = {
        paging: {
            currentPage: 0,
            itemsPerPage: 50
        }
    };

    self.params = {
        reportParams: {
            LicenseeID: null,
            Currency: null,
            FromDate: null,
            ToDate: null,
            ReportType: null,
            ConvertToCurrency: null,
            ExcludeTestData: null,
            ItemsPerPage: null,
            CurrentPage: null
        }
    };

    self.initPage = function () {
        self.cachedDOMElements();
        self.renderDatePicker();
        self.renderFilterNote(noteMessage);
        self.bindingEvents();
        self.renderDefaultDropdownCurrency();
        self.getLicenseeWithEnableBA();
    };

    self.cachedDOMElements = function () {
        _tables.btnGenerate = $("#btnGenerate");
        _tables.dropdownLicensee = $("#ddlLicensee");
        _tables.dropdownCurrency = $("#ddlCurrency");
        _tables.reportBy = $("#reportBy");
        _tables.currencyConvert = $("#currencyConvert");
        _tables.filterNote = $("#notemessage");
        _tables.chkExcludeTestData = $("#exclude-test-data");

        _tables.reportContainer = $("#playtechCasinoWinLossContainer");
        _tables.reportContent = $("#playtechCasinoWinLossContent");
    };

    self.cachedData = {
        paging: {
            TotalItems: null
        }
    };

    self.renderDatePicker = function () {
        $('#datepickerplugin').nexDatePicker({
            nexDatePickerType: "quickchoose",
            datePickerFormat: {
                changeMonth: true,
                changeYear: true,
                dateFormat: FormatConstant.M_DD_YY_STRING
            },
            dropDownList:
            {
                Id: "dropDownListId",
                Label: "Date ",
                LabelCss: "lbl-field col_3",
                OptionCss: "edit-field col_7"
            },
            firstDatePicker: {
                Id: "firstDatePickerId1",
                Label: labelFrom,
                InitDate: today,
                LabelCss: "lbl-field col_5",
                TextboxCss: "edit-field col_7"
            },
            secondDatePicker: {
                Id: "secondDatePickerId1",
                Label: labelTo,
                InitDate: today,
                LabelCss: "lbl-field col_1",
                TextboxCss: "edit-field col_6"
            },
            selectedItem: 5,
            onSelectDatePicker: function () {
            },
            minDate: new Date(minDate),
            maxDate: new Date(today)
        });

        _tables.datepickerplugin = $("#datepickerplugin");
    };

    self.renderFilterNote = function (noteMessage) {
        //// Assign Title for tooltip
        _tables.filterNote.attr("title", noteMessage);
        _tables.filterNote.tipsy({
            html: true,
            opacity: 0.9,
            gravity: 'ne'
        });
    };

    self.getLicenseeWithEnableBA = function () {
        AccReport.utility.CallAjax(this,
                                    window.rootUrl + "Licensee/Licensee/GetPlaytechCasino",
                                    null,
                                    self.renderLicenseeWithEnableBA,
                                    self.renderLicenseeWithEnableBA,
                                    AccReport.enableInput);
    };

    self.bindingEvents = function () {
        _tables.dropdownLicensee.change(function () {
            self.getCurrencies();
        });

        _tables.chkExcludeTestData.change(function () {
            self.chkExcludeTestDataChange();
        });

        _tables.btnGenerate.click(function () {
            _options.paging.currentPage = 0;
            _options.paging.itemsPerPage = 50;
            self.getPlaytechCasinoWinLossReport();
        });
    };

    self.renderLicenseeWithEnableBA = function (licenseeList) {
        if (licenseeList && licenseeList.length > 0) {
            var options = '';
            options += '<option value="0">Please select one!</option>';
            var licenseeCount = licenseeList.length;
            for (var i = 0; i < licenseeCount; i++) {
                var licensee = licenseeList[i];

                if (licensee.LicenseeName != licensee.BrandName) {
                    options += "<option enableBA='" + licensee.IsEnabledBA +
                             "' isMultiAgent='" + licensee.IsMultiAgent +
                             "' value='" + licensee.LicenseeId + "'>" +
                             licensee.LicenseeName + " (" + licensee.BrandName + ")</option>";
                }
                else {
                    options += "<option enableBA='" + licensee.IsEnabledBA +
                             "' isMultiAgent='" + licensee.IsMultiAgent +
                             "' value='" + licensee.LicenseeId + "'>" +
                             licensee.LicenseeName + "</option>";
                }
            }
            _tables.dropdownLicensee.append(options);
        }
        else {
            var noLicenseesFoundOption = '<option value="0">No licensee found!</option>';
            _tables.dropdownLicensee.append(noLicenseesFoundOption);
        }
    };

    self.renderDefaultDropdownCurrency = function () {
        var defaultCurrencyList = [{ Id: '0', Text: 'All Currencies'}];
        var options = '';
        var currencyCount = defaultCurrencyList.length;
        for (var i = 0; i < currencyCount; i++) {
            var currency = defaultCurrencyList[i];
            options += '<option value="' + currency.Id + '">' + currency.Text + '</option>';
        }

        _tables.dropdownCurrency.empty().append(options);
    };

    self.getCurrencies = function () {
        AccReport.disableInput();
        var data = {
            LicenseeId: _tables.dropdownLicensee.val()
        };

        if (data.LicenseeId == 0) {
            AccReport.enableInput();
            return;
        }

        AccReport.utility.CallAjax(this,
                                    window.rootUrl + "Licensee/Licensee/GetCurrenciesByLicensee",
                                    data,
                                    self.renderDropdownCurrency,
                                    self.renderDropdownCurrency,
                                    AccReport.enableInput);
    };

    self.renderDropdownCurrency = function (currencies) {
        self.renderDefaultDropdownCurrency();
        if (currencies && currencies.length) {
            var options = '';
            var currencyCount = currencies.length;
            for (var i = 0; i < currencyCount; i++) {
                var currency = currencies[i];
                options += '<option value="' + currency.CurrencyId +
                                 '" text="' + currency.CurrencyName + '">' + currency.CurrencyName + '</option>';
            }

            _tables.dropdownCurrency.append(options);
        }
    };

    self.validateParams = function () {
        var isValid = true;
        var message = '';
        var notifyIds = [];
        if (self.params.reportParams.LicenseeID == 0) {
            isValid = false;
            if (message == '') {
                message = "Please select one licensee!";
            }
            notifyIds.push("ddlLicensee");
        }

        if (!isValid) {
            AccReport.popUp.removeHighLight();
            AccReport.popUp.show(message, notifyIds.join(","));
        } else {
            AccReport.popUp.removeHighLight();
        }

        return isValid;
    };

    self.getParams = function () {
        self.params.reportParams.LicenseeID = parseInt(_tables.dropdownLicensee.val());
        self.params.reportParams.Currency = parseInt(_tables.dropdownCurrency.val());

        var dateRange = _tables.datepickerplugin.nexDatePicker("getDateRange");
        self.params.reportParams.FromDate = dateRange.FromDate;
        self.params.reportParams.ToDate = dateRange.ToDate;
        self.params.reportParams.ReportType = _tables.reportBy.find("input[name='rdReportBy']:checked").val();
        self.params.reportParams.ConvertToCurrency = _tables.currencyConvert.find("input[name='rdCurrencyConvert']:checked").val();
        self.params.reportParams.ExcludeTestData = _tables.chkExcludeTestData.attr("checked") ? true : false;
        self.params.reportParams.CurrentPage = _options.paging.currentPage + 1;
        self.params.reportParams.ItemsPerPage = _options.paging.itemsPerPage;
    };

    self.getPlaytechCasinoWinLossReport = function () {
        self.getParams();
        var data = {
            LicenseeID: self.params.reportParams.LicenseeID,
            Currency: self.params.reportParams.Currency,
            FromDate: self.params.reportParams.FromDate,
            ToDate: self.params.reportParams.ToDate,
            ReportType: self.params.reportParams.ReportType,
            ConvertToCurrency: self.params.reportParams.ConvertToCurrency,
            ExcludeTestData: self.params.reportParams.ExcludeTestData,
            CurrentPage: self.params.reportParams.CurrentPage,
            ItemsPerPage: self.params.reportParams.ItemsPerPage
        };
        var validateParams = self.validateParams();
        if (validateParams == true) {
            AccReport.disableInput();
            AccReport.utility.CallAjax(this,
                                    window.rootUrl + "Licensee/PlaytechCasinoWinLoss/GetPlaytechCasinoWinLoss",
                                    data,
                                    self.renderPlaytechCasinoWinLossReport,
                                    self.renderPlaytechCasinoWinLossReport,
                                    AccReport.enableInput);
        }
        else {
            AccReport.enableInput();
        }
    };

    self.renderPlaytechCasinoWinLossReport = function (rs) {
        _tables.reportContainer.removeClass("display-none");
        _tables.reportContent.addClass("display-none");
        var title = "Playtech Casino Win Loss Report From " + self.params.reportParams.FromDate + " To " + self.params.reportParams.ToDate;
        var columns = [
            { HeaderText: "Date", Width: 120, Name: "ColDate", HeaderAlign: "center", CellAlign: "center" },
            { HeaderText: "Currency", Width: 120, Name: "ColCurrency", HeaderAlign: "center", CellAlign: "center" },
            { HeaderText: "Turnover", Width: 150, Name: "ColTurnover", HeaderAlign: "center", CellAlign: "right", FormatNumber: FormatConstant.TWO_DECIMAL_NUMBER },
            { HeaderText: "Total Trans", Width: 100, Name: "ColTotalTrans", HeaderAlign: "center", CellAlign: "right", FormatNumber: FormatConstant.INT_NUMBER },
            { HeaderText: "Customer Win Loss", Width: 150, Name: "ColCustomerWinLoss", HeaderAlign: "center", CellAlign: "right", FormatNumber: FormatConstant.TWO_DECIMAL_NUMBER },
            { HeaderText: "Licensee Win Loss", Width: 150, Name: "ColLicenseeWinLoss", HeaderAlign: "center", CellAlign: "right", FormatNumber: FormatConstant.TWO_DECIMAL_NUMBER },
            { HeaderText: "Amount Due To", Width: 150, Name: "ColAmoutDueTo", HeaderAlign: "center", CellAlign: "right", FormatNumber: FormatConstant.TWO_DECIMAL_NUMBER }
        ];

        var bodyData = [];
        var footerData = [];

        self.cachedData.paging.TotalItems = rs.TotalItems;

        var reportLength = rs.ReportRows.length;
        if (rs && reportLength > 0) {
            var reportByDate = null;
            for (var reportByDateRowIndex = 0; reportByDateRowIndex < reportLength; reportByDateRowIndex++) {
                reportByDate = rs.ReportRows[reportByDateRowIndex];
                var reportByCurrencyLength = reportByDate.ReportRows.length;
                var row = [];
                var reportByCurrency = null;

                for (var reportByCurrencyRowIndex = 0; reportByCurrencyRowIndex < reportByCurrencyLength; reportByCurrencyRowIndex++) {
                    row = [];
                    reportByCurrency = reportByDate.ReportRows[reportByCurrencyRowIndex];
                    if (reportByCurrencyRowIndex == 0) {
                        row.push(
                            { Name: "ColDate", Value: reportByDate.Date, MergeRowNumber: reportByCurrencyLength,
                                TdClass: reportByDateRowIndex % 2 == 0 ? "evenRow" : ""
                            }
                        );
                    }

                    row.push(
                        { Name: "ColCurrency", Value: reportByCurrency.CurrencyName, TdClass: reportByDateRowIndex % 2 == 0 ? "evenRow" : "" },
                        { Name: "ColTurnover", Value: reportByCurrency.TurnOver, TdClass: reportByDateRowIndex % 2 == 0 ? "evenRow" : "" },
                        { Name: "ColTotalTrans", Value: reportByCurrency.TotalTransaction, TdClass: reportByDateRowIndex % 2 == 0 ? "evenRow" : "" },
                        { Name: "ColCustomerWinLoss", Value: reportByCurrency.CustomerWinLoss, TdClass: reportByDateRowIndex % 2 == 0 ? "evenRow" : "" },
                        { Name: "ColLicenseeWinLoss", Value: reportByCurrency.LicenseeWinLoss, TdClass: reportByDateRowIndex % 2 == 0 ? "evenRow" : "" },
                        { Name: "ColAmoutDueTo", Value: reportByCurrency.AmountDueTo, TdClass: reportByDateRowIndex % 2 == 0 ? "evenRow" : "" }
                    );
                    bodyData.push({
                        Columns: row
                    });
                }
            }

            self.assignTotalData(rs.Total, "subTotal", bodyData);
            self.assignBaseCurrencyTotalData(rs.BaseCurrencyTotal, footerData);
        }

        var gridOptions = {
            columns: columns,
            bodyRows: bodyData,
            footerRows: footerData,
            gridExpandHeight: 260,
            showDistinctRow: false,
            showHighlightRow: false,
            showTitle: true,
            showFooter: true,
            titleInfo: {
                text: title,
                showExcelButton: true,
                exportExcelFunction: function () {
                    self.ExportReportData();
                }
            },
            showPager: true,
            customPager: true,
            pagerOption: {
                itemsPerPage: _options.paging.itemsPerPage,
                currentPage: _options.paging.currentPage,
                totalItem: rs.TotalItems,
                itemsPerPageArray: [50, 100, 200, 500],
                pagerSelectCallBack: self.pageSelectCallback
            }
        };

        _tables.reportContent.removeClass("display-none").fanexGrid(gridOptions);
    };

    self.assignTotalData = function (total, cssClass, reportData) {
        var totalRowLength = total.length;
        if (totalRowLength > 0) {
            for (var totalRowIndex = 0; totalRowIndex < totalRowLength; totalRowIndex++) {
                var totalRow = [];
                var currentTotalRow = total[totalRowIndex];
                if (totalRowIndex == 0) {
                    totalRow.push(
                        { Name: "ColDate", Value: "Total", MergeRowNumber: totalRowLength,
                            TdClass: cssClass
                        });
                }

                totalRow.push(
                        { Name: "ColCurrency", Value: currentTotalRow.CurrencyName, TdClass: cssClass },
                        { Name: "ColTurnover", Value: currentTotalRow.TurnOver, TdClass: cssClass },
                        { Name: "ColTotalTrans", Value: currentTotalRow.TotalTransaction, TdClass: cssClass },
                        { Name: "ColCustomerWinLoss", Value: currentTotalRow.CustomerWinLoss, TdClass: cssClass },
                        { Name: "ColLicenseeWinLoss", Value: currentTotalRow.LicenseeWinLoss, TdClass: cssClass },
                        { Name: "ColAmoutDueTo", Value: currentTotalRow.AmountDueTo, TdClass: cssClass }
                    );
                reportData.push({
                    Columns: totalRow
                });
            }
        }
    };

    self.assignBaseCurrencyTotalData = function (total, reportData) {
        var totalRowLength = total.ReportRows.length;
        if (totalRowLength > 0) {
            for (var totalRowIndex = 0; totalRowIndex < totalRowLength; totalRowIndex++) {
                var totalRow = [];
                var currentTotalRow = total.ReportRows[totalRowIndex];
                if (totalRowIndex == 0) {
                    totalRow.push(
                        { Name: "ColDate", Value: "Total", MergeRowNumber: totalRowLength },
                        { Name: "ColCurrency", Value: currentTotalRow.CurrencyName },
                        { Name: "ColTurnover", Value: currentTotalRow.TurnOver },
                        { Name: "ColTotalTrans", Value: total.TotalTransaction, MergeRowNumber: totalRowLength },
                        { Name: "ColCustomerWinLoss", Value: currentTotalRow.CustomerWinLoss },
                        { Name: "ColLicenseeWinLoss", Value: currentTotalRow.LicenseeWinLoss },
                        { Name: "ColAmoutDueTo", Value: currentTotalRow.AmountDueTo }
                    );
                }
                else {
                    totalRow.push(
                        { Name: "ColCurrency", Value: currentTotalRow.CurrencyName },
                        { Name: "ColTurnover", Value: currentTotalRow.TurnOver },
                        { Name: "ColCustomerWinLoss", Value: currentTotalRow.CustomerWinLoss },
                        { Name: "ColLicenseeWinLoss", Value: currentTotalRow.LicenseeWinLoss },
                        { Name: "ColAmoutDueTo", Value: currentTotalRow.AmountDueTo }
                    );
                }
                reportData.push({
                    Columns: totalRow
                });
            }
        }
    };

    self.pageSelectCallback = function (page_index, itemsPerPage, jq) {
        var oldPagingParams = {
            pageIndex: parseInt(self.params.reportParams.CurrentPage),
            totalItems: parseInt(self.cachedData.paging.TotalItems),
            pageSize: parseInt(self.params.reportParams.ItemsPerPage)
        };

        if (oldPagingParams.pageSize != itemsPerPage) {
            _options.paging.currentPage = 0;
            self.params.reportParams.CurrentPage = 1;
        }
        else if (page_index * itemsPerPage <= oldPagingParams.totalItems) {
            _options.paging.currentPage = page_index;
            self.params.reportParams.CurrentPage = page_index + 1;
        }

        _options.paging.itemsPerPage = itemsPerPage;
        self.params.reportParams.ItemsPerPage = itemsPerPage;

        if (!(oldPagingParams.totalItems <= oldPagingParams.pageSize &&
           oldPagingParams.pageIndex == 1 && oldPagingParams.pageSize <= itemsPerPage)) {
            self.getPlaytechCasinoWinLossReport();
        }

        return false;
    };

    self.ExportReportData = function () {
        self.getParams();
        var exportParams = {
            LicenseeID: self.params.reportParams.LicenseeID,
            Currency: self.params.reportParams.Currency,
            FromDate: self.params.reportParams.FromDate,
            ToDate: self.params.reportParams.ToDate,
            ReportType: self.params.reportParams.ReportType,
            ConvertToCurrency: self.params.reportParams.ConvertToCurrency,
            CurrentPage: 0,
            ItemsPerPage: self.cachedData.paging.TotalItems
        };

        AccReport.downloadFile(window.rootUrl + "Licensee/PlaytechCasinoWinLoss/ExportReport", exportParams);
    };

    self.chkExcludeTestDataChange = function () {
        if (_tables.chkExcludeTestData.attr("checked")) {
            _tables.chkExcludeTestData.attr("checked", "checked");
        }
        else {
            _tables.chkExcludeTestData.removeAttr("checked");
        }
    };
};

$(document).ready(function () {
    var playtechCasinoWinLossReport = new Fanex.AccountingReports.Licensee.PlaytechCasinoWinLossReport();
    playtechCasinoWinLossReport.initPage();
});