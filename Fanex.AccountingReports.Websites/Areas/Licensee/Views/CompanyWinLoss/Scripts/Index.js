/* global Namespace,noteMessage,FormatConstant,labelFrom,today,labelTo,today,minDate,AccReport,reportTitle,moment */
//"use strict";
Namespace.Register("Fanex.AccountingReports.Licensee.CompanyWinLoss");
var companyWinLoss;
Fanex.AccountingReports.Licensee.CompanyWinLoss = function () {
    var self = this;

    var _tables = {
        btnGenerate: null,
        dropdownLicensee: null,
        dropdownCurrency: null,
        dropdownProduct: null,
        datepickerplugin: null,
        reportContainer: null,
        reportContent: null,
        cachedTotalItems: 0
    };

    var enums = {
        keenOcean: 96
    };

    self.params = {
        LicenseeId: null,
        CurrencyId: null,
        ProductPrefix: null,
        FromDate: null,
        ToDate: null,
        IsPagingCallBack: false,
        PageIndex: 0,
        PageSize: 50
    };

    self.resetParams = function () {
        self.params.LicenseeId = 0;
        self.params.CurrencyId = 0;
        self.params.ProductPrefix = '';
        self.params.FromDate = _tables.datepickerplugin.monPickr("getMonth", "MM/dd/yyyy");
        self.params.ToDate = _tables.datepickerplugin.monPickr("getMonth", "MM/dd/yyyy");
        self.params.IsPagingCallBack = false;
        self.params.PageIndex = 0;
        self.params.PageSize = 50;
    };

    self.getParams = function () {
        self.params.LicenseeId = _tables.dropdownLicensee.val();
        self.params.CurrencyId = _tables.dropdownCurrency.val();
        self.params.ProductPrefix = _tables.dropdownProduct.val();
        self.params.FromDate = _tables.datepickerplugin.monPickr("getMonth", "MM/dd/yyyy");
        self.params.ToDate = _tables.datepickerplugin.monPickr("getMonth", "MM/dd/yyyy");
        if (self.params.PageIndex == null) {
            self.params.PageIndex = 0;
        }

        if (self.params.PageSize == null) {
            self.params.PageSize = 50;
        }
    };

    self.initPage = function () {
        self.cachedDOMElements();
        self.renderDatePicker();
        self.getAllLicensees();
        self.getProducts();
        self.bindingEvent();
        self.renderDefaultDropdownCurrency();
        self.renderDefaultDropdownProduct();
    };

    self.cachedDOMElements = function () {
        _tables.btnGenerate = $("#btnGenerate");
        _tables.dropdownLicensee = $("#ddlLicensee");
        _tables.dropdownCurrency = $("#ddlCurrency");
        _tables.dropdownProduct = $("#ddlProduct");
        _tables.datepickerplugin = $("#dvMonth");
        _tables.reportContainer = $("#reportContainer");
        _tables.reportContent = $("#reportContent");
    };

    self.bindingEvent = function () {
        _tables.dropdownLicensee.change(function () {
            var licensee = parseInt(_tables.dropdownLicensee.val());
            if (licensee == enums.keenOcean) {
                _tables.dropdownCurrency.empty();
                self.renderDefaultDropdownCurrency();
            } else {
                self.getCurrencies();
            }

            self.getProducts();
            AccReport.popUp.removeHighLight();
        });

        _tables.btnGenerate.click(function () {
            self.btnGenerateClick();
        });
    };

    self.renderDatePicker = function () {
        $('#dvMonth').monPickr({
            plugInId: '#dvMonth',
            today: today,
            maxDate: today,
            minDate: minDate,
            showDateRange: false,
            log: function (msg) {
                console.warn(msg);
            }
        });

        _tables.datepickerplugin = $('#dvMonth');
    };

    self.getAllLicensees = function () {
        $.ajax({
            type: "POST",
            url: window.rootUrl + "Licensee/Licensee/GetAllLicensees",
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            data: null,
            beforeSend: function () {
                AccReport.disableInput();
            },
            success: function (rs) {
                self.renderLicensee(rs);
            },
            error: function (event, jqXHR, ajaxSettings) {
            },
            complete: function () {
                AccReport.enableInput();
            }
        });
    };

    self.renderLicensee = function (licenseeList) {
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

    self.getCurrencies = function () {
        AccReport.disableInput();
        var enableBA = _tables.dropdownCurrency.find("option:selected").attr("enableBA");
        var data = {
            LicenseeId: _tables.dropdownLicensee.val(),
            ExcludeTest: 1
        };

        if (data.LicenseeId == 0) {
            AccReport.enableInput();
            return;
        }

        $.ajax({
            type: "POST",
            url: window.rootUrl + "Licensee/CompanyWinLoss/GetCurrenciesByLicensee",
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            data: JSON.stringify(data),
            success: function (rs) {
                self.renderDropdownCurrency(rs);
            },
            error: function (event, jqXHR, ajaxSettings) {
                self.renderDropdownCurrency(null);
            },
            complete: function () {
                AccReport.enableInput();
            }
        });
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

    self.getProducts = function () {
        AccReport.disableInput();
        var enableBA = _tables.dropdownCurrency.find("option:selected").attr("enableBA");
        var data = {
            LicenseeId: _tables.dropdownLicensee.val()
        };

        if (data.LicenseeId == 0) {
            AccReport.enableInput();
            return;
        }

        $.ajax({
            type: "POST",
            url: window.rootUrl + "Licensee/CompanyWinLoss/GetProductListByLicense",
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            data: JSON.stringify(data),
            success: function (rs) {
                self.renderDropdownProduct(rs);
            },
            error: function (event, jqXHR, ajaxSettings) {
                self.renderDropdownProduct(null);
            },
            complete: function () {
                AccReport.enableInput();
            }
        });
    };

    self.renderDropdownProduct = function (products) {
        self.renderDefaultDropdownProduct();
        if (products && products.length) {
            var options = '';
            var productCount = products.length;
            for (var i = 0; i < productCount; i++) {
                var product = products[i];
                options += '<option value="' + product.ProductPrefix +
                                 '" text="' + product.ProductName + '">' + product.ProductName + '</option>';
            }

            _tables.dropdownProduct.append(options);
        }
    };

    self.renderDefaultDropdownProduct = function () {
        var defaultProductList = [{ ProductPrefix: '', ProductName: 'All Products'}];
        var options = '';
        var productCount = defaultProductList.length;
        for (var i = 0; i < productCount; i++) {
            var product = defaultProductList[i];
            options += '<option value="' + product.ProductPrefix + '">' + product.ProductName + '</option>';
        }

        _tables.dropdownProduct.empty().append(options);
    };

    self.btnGenerateClick = function () {
        var isvalid = self.validateParams();
        if (!isvalid) {
            return;
        }

        self.resetParams();
        self.getReport();
    };

    self.getReport = function () {
        self.getParams();

        if (self.params.PageIndex == null) {
            self.params.PageIndex = 0;
        }

        if (self.params.PageSize == null) {
            self.params.PageSize = 50;
        }

        AccReport.disableInput();

        $.ajax({
            type: "POST",
            url: window.rootUrl + "Licensee/CompanyWinLoss/GetCompanyWinLoss",
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            data: JSON.stringify(self.params),
            success: function (report) {
                AccReport.popUp.hide();
                self.renderReport(report);
            },
            error: function (event) {
                if (event && event.responseText) {
                    var r = JSON.parse(event.responseText);
                    AccReport.popUp.show(r.message);
                    AccReport.enableInput();
                }
            },
            complete: function () {
                AccReport.enableInput();
            }
        });
    };

    self.validateParams = function () {
        var isValid = true;
        var message = '';
        var notifyIds = [];

        var data = {
            LicenseeId: _tables.dropdownLicensee.val()
        };
        if (data.LicenseeId === "0") {
            isValid = false;
            if (message === '') {
                message = "Please select at least one Licensee";
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

    var _gridOption = {
        itemsPerPage: 50,
        currentPage: 0
    };

    self.renderReport = function (gridOptions) {
        _tables.cachedTotalItems = gridOptions.totalItems;
        _tables.reportContainer.removeClass("display-none");
        var titleInfo = {
            text: gridOptions.TitleInfoText,
            showExcelButton: true,
            exportExcelFunction: function () {
                self.exportReport();
            }
        };
        var reportLength = gridOptions.bodyRows.length;
        var pagerOption = {
            itemsPerPage: _gridOption.itemsPerPage,
            currentPage: _gridOption.currentPage,
            itemsPerPageArray: [50, 100, 200, 500],
            totalItem: _tables.cachedTotalItems,
            pagerSelectCallBack: pageSelectCallback
        };

        gridOptions.showTitle = true;
        gridOptions.titleInfo = titleInfo;
        gridOptions.customPager = true,
        gridOptions.showPager = true;
        gridOptions.pagerOption = pagerOption;
        _tables.reportContent.removeClass("display-none").fanexGrid(gridOptions);
    };

    var pageSelectCallback = function (page_index, itemsPerPage, jq) {
        itemsPerPage = parseInt(itemsPerPage);
        var oldPagingParams = {
            PageIndex: parseInt(self.params.PageIndex),
            TotalItems: parseInt(_tables.cachedTotalItems),
            PageSize: parseInt(self.params.PageSize)
        };

        if (oldPagingParams.PageSize !== itemsPerPage) {
            _gridOption.currentPage = 0;
            self.params.PageIndex = 0;
            self.params.PageSize = itemsPerPage;
        }
        else if (page_index * itemsPerPage <= oldPagingParams.TotalItems) {
            _gridOption.currentPage = page_index;
            self.params.PageIndex = page_index;
        }

        _gridOption.itemsPerPage = itemsPerPage;
        self.params.PageSize = itemsPerPage;

        if (!(oldPagingParams.TotalItems <= oldPagingParams.PageSize &&
        oldPagingParams.PageIndex === 0 && oldPagingParams.PageSize <= itemsPerPage)) {
            self.params.IsPagingCallBack = true;
            self.getReport();
        }

        return false;
    };

    self.exportReport = function () {
        AccReport.downloadFile(window.rootUrl + "Licensee/CompanyWinLoss/ExportReport", self.params);
    };
};

$(document).ready(function () {
    companyWinLoss = new Fanex.AccountingReports.Licensee.CompanyWinLoss();
    companyWinLoss.initPage();
});