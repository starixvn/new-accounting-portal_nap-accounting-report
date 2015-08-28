Namespace.Register("Fanex.AccountingReports.Gaming.NumberOfActiveMember");

$(document).ready(function () {
    var numberOfActiveMember = new Fanex.AccountingReports.Gaming.NumberOfActiveMember();
    numberOfActiveMember.initPage();
});


Fanex.AccountingReports.Gaming.NumberOfActiveMember = function () {
    var self = this;

    var _tables = {
        datepickerplugin: null,
        btnGenerate: null,
        report: null,
        reportContent: null,
        reportHeader: null,
        reportTitle: null,
        reportExportExcel: null,
        ddlProducts: null
    };

    var _param = {
        reportParams: {
            FromDate: null,
            ToDate: null,
            ProductList: null
        },

        searchReportParams: {
            FromDate: null,
            ToDate: null,
            ProductList: null,
            ProductName: null
        }
    };

    self.cachedProductName = '';

    self.TempHtml = {
        contentHtml: ''
    };

    self.initPage = function () {
        cachedDOMElements();
        renderDatePicker();
        self.getProduct();
        bindingEvent();
    };

    var cachedDOMElements = function () {
        _tables.btnGenerate = $("#btnGenerate");
        _tables.ddlProducts = $("#ddlProduct");
        _tables.reportContainer = $("#numberActiveMemberContainer");
        _tables.report = $("#numberActiveMemberContent");
    };

    var bindingEvent = function () {
        _tables.btnGenerate.click(function () {
            self.cachedProductName = '';
            self.btnGenerateClick();
        });

        $("#btnSearch").live("click", function () {
            self.searchByProduct();
        });

        $("#txtProductName").live("keypress", function (e) {
            var key = e.which;
            if (key == 13)  // the enter key code
            {
                self.searchByProduct();
            }
        });
    };

    var renderDatePicker = function () {
        $('#datepickerplugin').nexDatePicker({
            nexDatePickerType: "daterange",
            datePickerFormat: {
                changeMonth: true,
                changeYear: true,
                dateFormat: "M dd, yy",
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

    self.btnGenerateClick = function () {
        self.getReport();
    };

    self.getProduct = function () {
        AccReport.disableInput();
        AccReport.utility.CallAjax(this,
        window.rootUrl + "Gaming/NumberOfActiveMember/GetProduct",
        null,
        self.RenderProduct,
        self.RenderProduct,
        AccReport.enableInput);
    };

    self.RenderProduct = function (rs) {
        var products = [];
        var noItemText = 'No product found.';
        for (var i = 0; i < rs.length; i++) {
            var product = rs[i];
            products.push({ Id: product.ProductId, Text: product.ProductName });
        }

        var numberProducts = products.length;
        var ddwnProducts = [];
        var prefixId = 'ddlProduct_';
        for (var i = 0; i < numberProducts; i++) {
            var product = products[i];
            ddwnProducts.push({
                Id: prefixId + product.Id,
                Value: product.Id,
                Text: product.Text,
                Checked: true,
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
            isCheckAll: true,
            showFooter: false,
            showTooltip: true,
            checkAllText: 'All Products',
            isUsedCustomCheckAllText: true,
            customCheckAllText: 'All Products',
            noItemText: noItemText,
            selectedList: 3,
            listData: ddwnProducts
        });

        _tables.ddlProducts = $("#ddlProduct");
    };

    self.getReport = function () {
        AccReport.disableInput();
        getParams()

        if (validateParams() == true) {
            AccReport.utility.CallAjax(this,
            window.rootUrl + "Gaming/NumberOfActiveMember/GetReport",
            _param.reportParams,
            self.renderReport,
            self.renderReport,
            AccReport.enableInput);
        }        
    };

    var getParams = function () {
        var dateRange = _tables.datepickerplugin.nexDatePicker("getDateRange");
        _param.reportParams.FromDate = dateRange.FromDate;
        _param.reportParams.ToDate = dateRange.ToDate;
        _param.reportParams.ProductList = _tables.ddlProducts.multiSelect("getCheckedItems").join(',');
    };

    var getSearchParams = function () {
        var dateRange = _tables.datepickerplugin.nexDatePicker("getDateRange");
        _param.searchReportParams.FromDate = dateRange.FromDate;
        _param.searchReportParams.ToDate = dateRange.ToDate;
        _param.searchReportParams.ProductList = _tables.ddlProducts.multiSelect("getCheckedItems").join(',');
        _param.searchReportParams.ProductName = $("#txtProductName").val();
    };

    var validateParams = function () {
        var notifyIds = [];
        var msg = '';
        if (_param.reportParams.ProductList.length == 0) {
            msg = 'Please select at least one product!';
            notifyIds.push("ddlProduct");
        }

        if (msg.length > 0) {
            AccReport.popUp.show(msg, notifyIds.join(','));
            AccReport.enableInput();
            return false;
        }

        return true;
    };

    self.renderReport = function (gridOptions) {
        _tables.reportContainer.removeClass("display-none");
        _tables.report.addClass("display-none");
        gridOptions.showTree = true;
        var titleInfo = {
            text: gridOptions.TitleInfoText + "<div class='search'><div class='productBox'><input id='txtProductName' type='text' value='" + self.cachedProductName + "'/><input id='btnSearch' class='icon-search' /></div></div>",
            showExcelButton: true,
            exportExcelFunction: function () {
                self.exportReport();
            },
            showCollapseExpandButton: true
        };
        gridOptions.titleInfo = titleInfo;
        gridOptions.gridExpandHeight = 200;
        _tables.report.removeClass("display-none").fanexGrid(gridOptions);
    };

    self.searchByProduct = function () {
        AccReport.disableInput();
        getSearchParams();
        self.cachedProductName = _param.searchReportParams.ProductName;
        if (validateParams() == true) {
            $.ajax({
                type: "POST",
                url: window.rootUrl + "Gaming/NumberOfActiveMember/SearchNumberOfActiveMemberByProduct",
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                data: JSON.stringify(_param.searchReportParams),
                beforeSend: function () {
                    AccReport.disableInput();
                },
                success: function (report) {
                    self.renderReport(report);
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

    self.exportReport = function () {
        AccReport.downloadFile(window.rootUrl + "Gaming/NumberOfActiveMember/ExportReport", _param.reportParams);
    };
};