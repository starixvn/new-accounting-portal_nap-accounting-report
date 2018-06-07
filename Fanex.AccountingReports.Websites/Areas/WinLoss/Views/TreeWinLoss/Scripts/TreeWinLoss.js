
Namespace.Register("Fanex.AccountingReports.WinLoss.TreeWinLoss.TreeWinLoss");
Fanex.AccountingReports.WinLoss.TreeWinLoss.TreeWinLoss = function () {
    var self = this;
    var _tables = {
        btnGenerate: null,
        btnMode: null,
        normalModeDatepickerPlugin: null,
        historyModeDatepickerPlugin: null,
        normalMode: null,
        historyMode: null,
        reportContainer: null,
        report: null,
        filterNote: null,
        productsList: {
            sportsbook: null,
            racing: null,
            numbergame: null,
            liveCasino: null,
            virtualSports: null,
            rngCasino: null,
            bingo: null,
            poker: null
        },
        ddlProduct: null
    };

    self.Params = {
        reportParams: {
            IsNormalMode: true,
            FromDate: new Date(),
            ToDate: new Date(),
            ProductSelection: null,
            Products: null
        }
    };

    self.initPage = function () {
        cachedDOMElements();
        renderSingleDatePicker();
        renderDateRangeDatePicker();
        getProduct();
        renderFilterNote(noteMessage);
        bindingEvent();
    };

    var cachedDOMElements = function () {
        _tables.btnGenerate = $("#btnGenerate");
        _tables.reportContainer = $("#treeWinLossByProductContainer");
        _tables.report = $("#treeWinLossByProduct");
        _tables.filterNote = $("#notemessage");
        _tables.ddlProduct = $("#ddlProduct");
        _tables.btnMode = $("#btnMode");
        _tables.normalMode = $("#normalMode");
        _tables.historyMode = $("#historyMode");
    };


    var renderSingleDatePicker = function () {
        $('#normalModeDatepickerPlugin').nexDatePicker({
            nexDatePickerType: "single",
            firstDatePicker: {
                Id: "datePickerInput",
                Label: labelDate,
                InitDate: today,
                LabelCss: "lbl-field col_2",
                TextboxCss: "edit-field col_6"
            },

            datePickerFormat: {
                dateFormat: FormatConstant.M_DD_YY_STRING,
                changeMonth: true,
                changeYear: true,
                minDate: new Date(dateAfterMoveBet),
                maxDate: new Date(today),
                showOtherMonths: true,
                selectOtherMonths: true
            }
        });

        _tables.normalModeDatepickerPlugin = $("#normalModeDatepickerPlugin");
    };

    var renderDateRangeDatePicker = function () {
        $('#historyModeDatepickerPlugin').nexDatePicker({
            nexDatePickerType: "daterange",
            datePickerFormat: {
                changeMonth: true,
                changeYear: true,
                dateFormat: FormatConstant.M_DD_YY_STRING
            },
            firstDatePicker: {
                Id: "firstDatePickerId1",
                Label: labelFrom,
                InitDate: yesterday,
                LabelCss: "lbl-field col_2",
                TextboxCss: "edit-field col_5"
            },
            secondDatePicker: {
                Id: "secondDatePickerId1",
                Label: labelTo,
                InitDate: yesterday,
                LabelCss: "lbl-field col_2",
                TextboxCss: "edit-field col_6"
            },
            selectedItem: 0,
            onSelectDatePicker: function () {
            },
            minDate: new Date(minDate),
            maxDate: new Date(yesterday)
        });

        _tables.historyModeDatepickerPlugin = $("#historyModeDatepickerPlugin");
    };

    var renderFilterNote = function (noteMessage) {
        //// Assign Title for tooltip
        _tables.filterNote.attr("title", noteMessage);
        _tables.filterNote.tipsy({
            html: true,
            opacity: 0.9,
            gravity: 'ne'
        });
    };

    var getProduct = function () {
        AccReport.disableInput();
        AccReport.utility.CallAjax(this,
        window.rootUrl + "WinLoss/TreeWinLoss/GetProduct",
        null,
        renderProductsDropdown,
        renderProductsDropdown,
        AccReport.enableInput);
    }

    var renderProductsDropdown = function (products) {
        var noItemText = 'No product found.';
        var len = products.length;
        var ddwnProducts = [];
        var prefixId = 'ddlProduct_';
        for (var i = 0; i < len; i++) {
            var product = products[i];
            ddwnProducts.push({
                Id: prefixId + product.ProductId,
                Value: product.ProductId,
                Text: product.ProductName,
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
            listData: ddwnProducts
        });

        _tables.ddlProduct = $("#ddlProduct");
    };

    var checkBoxIsChecked = function (checkBox) {
        return checkBox.attr("checked") ? true : false;
    };

    var bindingEvent = function () {
        _tables.btnGenerate.click(function () {
            self.btnGenerateClick();
        });

        _tables.btnMode.click(function () {
            btnModeClick();
        });
    };

    self.btnGenerateClick = function () {
        AccReport.disableInput();
        ResetLayout();
        getParams();
        if (validateParams() == true) {
            $.ajax({
                type: "POST",
                url: window.rootUrl + "WinLoss/TreeWinLoss/GetTreeWinLossByProduct",
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                data: JSON.stringify(self.Params.reportParams),
                beforeSend: function () {
                    AccReport.disableInput();
                },
                success: function (rs) {
                    AccReport.popUp.hide();
                    renderTreeWinLossByProduct(rs);
                },
                error: function (jqXHR, event, ajaxSettings) {
                    if (jqXHR.status &&
                    jqXHR.status == AccReportData.Constants.HtmlErrorCode.SystemError) {
                        var r = jQuery.parseJSON(jqXHR.responseText);
                        AccReport.popUp.show(r.message);
                        renderTreeWinLossByProduct(null);
                    }
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
        if (self.Params.reportParams.IsNormalMode) {
            var date = _tables.normalModeDatepickerPlugin.nexDatePicker("getSingleDate");
            self.Params.reportParams.FromDate = date;
            self.Params.reportParams.ToDate = date;

        } else {
            var dateRange = _tables.historyModeDatepickerPlugin.nexDatePicker("getDateRange");
            self.Params.reportParams.FromDate = dateRange.FromDate;
            self.Params.reportParams.ToDate = dateRange.ToDate;
        }

        self.Params.reportParams.ProductSelection = _tables.ddlProduct.multiSelect("getCheckedItems").join(',');
    };

    var validateParams = function () {
        var isValid = true;
        var message = '';
        var notifyIds = [];

        if (self.Params.reportParams.ProductSelection.length == 0) {
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

    self.hideReport = function () {
        _tables.reportContainer.addClass("display-none");
        _tables.report.addClass("display-none");
    };

    self.showReport = function () {
        _tables.reportContainer.removeClass("display-none");
        _tables.report.removeClass("display-none");
    };

    self.customSortNumber = function (col1Value, col2Value, sortDirection) {
        var isNumber1 = !isNaN(col1Value);
        var isNumber2 = !isNaN(col2Value);

        if (isNumber1 && isNumber2) {
            var parseValCol1 = parseFloat(col1Value);
            var parseValCol2 = parseFloat(col2Value);
            if (sortDirection.toLowerCase() === 'asc') {
                return parseValCol1 > parseValCol2 ? 1 : -1;
            }
            else {
                return parseValCol1 < parseValCol2 ? 1 : -1;
            }
        }

        if (sortDirection.toLowerCase() === 'asc') {
            if (!isNumber1) {
                return 1;
            }
        }
        else {
            if (!isNumber2) {
                return 1;
            }
        }

        return -1;
    };

    var renderTreeWinLossByProduct = function (report) {
        _tables.reportContainer.removeClass("display-none");
        _tables.report.addClass("display-none");
        var bodyData = [];
        var footerData = [];
        var isShowNA = report.IsShowNotAvailableTurnover;
        if (report && report.ChildsInfo.length > 0) {
            var len = report.ChildsInfo.length;
            var info = null;
            for (var i = 0; i < len; i++) {
                info = report.ChildsInfo[i];
                info.BaseRMTurnover = isShowNA ? "N/A" : info.BaseRMTurnover;
                info.BaseRMVirtualTurnover = isShowNA ? "N/A" : info.BaseRMVirtualTurnover;
                info.BaseRMMargin = isShowNA ? "N/A" : info.BaseRMMargin;
                bodyData.push({
                    Columns: [{
                        Name: "ColCurrencyName", Value: {
                            currencyId: info.Id,
                            currencyName: info.Name
                        },
                        CustomCellRender: function (currentCell) {
                            var clickEvent = ' onclick = "viewTreeWinLossByComm(' + currentCell.Value.currencyId + ',\'' + currentCell.Value.currencyName + "');\"";
                            var html = '<div class="link " ' + clickEvent + '>' + currentCell.Value.currencyName + '</div>';
                            return html;
                        }
                    },
                        { Name: "ColTurnover", Value: info.Turnover },
                        { Name: "ColExtraPT", Value: info.ExtraAmount },
                        { Name: "ColWinLoss", Value: info.WinLoss },
                        { Name: "ColCommission", Value: info.Comm },
                        { Name: "ColWinLossTotal", Value: info.WinLossTotal },
                        { Name: "ColBaseRMTurnover", Value: info.BaseRMTurnover },
                        { Name: "ColBaseRMVirtualTurnover", Value: info.BaseRMVirtualTurnover },
                        { Name: "ColBaseRMTOVariancePlus", Value: info.BaseRMTOVariancePlus },
                        { Name: "ColBaseRMTOVarianceMinus", Value: info.BaseRMTOVarianceMinus },
                        { Name: "ColBaseRMTOVarianceAbsoluteTotal", Value: info.BaseRMTOVarianceAbsoluteTotal },
                        { Name: "ColBaseRMWLVariancePlus", Value: info.BaseRMWLVariancePlus },
                        { Name: "ColBaseRMWLVarianceMinus", Value: info.BaseRMWLVarianceMinus },
                        { Name: "ColBaseRMWLVarianceTotal", Value: info.BaseRMWinlostVarianceTotal },
                        { Name: "ColBaseRMVarianceMargin", Value: info.BaseRMVarianceMargin },
                        { Name: "ColBaseRMTotal", Value: info.BaseRMTotal },
                        { Name: "ColBaseRMVirtualTotal", Value: info.BaseRMVirtualTotal },
                        { Name: "ColBaseRMMargin", Value: info.BaseRMMargin },
                        { Name: "ColVirtualBaseRMMargin", Value: info.VirtualBaseRMMargin }
                    ]
                });
            }

            for (var j = 0; j < report.TotalInfo.length; j++) {
                info = report.TotalInfo[j];
                info.BaseRMTurnover = isShowNA ? "N/A" : info.BaseRMTurnover;
                info.BaseRMVirtualTurnover = isShowNA ? "N/A" : info.BaseRMVirtualTurnover;
                info.BaseRMMargin = isShowNA ? "N/A" : info.BaseRMMargin;
                footerData.push({
                    Columns: [{ Name: "ColCurrencyName", Value: info.Name, MergeColNumber: 6, CellAlign: "right" },
                        { Name: "ColBaseRMTurnover", Value: info.BaseRMTurnover },
                        { Name: "ColBaseRMVirtualTurnover", Value: info.BaseRMVirtualTurnover },
                        { Name: "ColBaseRMTOVariancePlus", Value: info.BaseRMTOVariancePlus },
                        { Name: "ColBaseRMTOVarianceMinus", Value: info.BaseRMTOVarianceMinus },
                        { Name: "ColBaseRMTOVarianceAbsoluteTotal", Value: info.BaseRMTOVarianceAbsoluteTotal },
                        { Name: "ColBaseRMWLVariancePlus", Value: info.BaseRMWLVariancePlus },
                        { Name: "ColBaseRMWLVarianceMinus", Value: info.BaseRMWLVarianceMinus },
                        { Name: "ColBaseRMWLVarianceTotal", Value: info.BaseRMWinlostVarianceTotal },
                        { Name: "ColBaseRMVarianceMargin", Value: info.BaseRMVarianceMargin },
                        { Name: "ColBaseRMTotal", Value: info.BaseRMTotal },
                        { Name: "ColBaseRMVirtualTotal", Value: info.BaseRMVirtualTotal },
                        { Name: "ColBaseRMMargin", Value: info.BaseRMMargin },
                        { Name: "ColVirtualBaseRMMargin", Value: info.VirtualBaseRMMargin }
                    ]
                });
            }
        }

        var gridOptions = {
            customizeHeader: true,
            groupHeaderRows: [
            {
                Columns: [{ HeaderText: "Currency", Name: "ColCurrencyName", MergeRowNumber: 3 },
                { HeaderText: "Real Turnover", Name: "ColTurnover", MergeRowNumber: 3 },
                { HeaderText: "ExtraPT", Name: "ColExtraPT", MergeRowNumber: 3 },
                { HeaderText: "Real WinLoss", Name: "ColWinLoss", MergeRowNumber: 3 },
                { HeaderText: "Comm", Name: "ColCommission", MergeRowNumber: 3 },
                { HeaderText: "Real Income", Name: "ColWinLossTotal", MergeRowNumber: 3 },
                { HeaderText: "BaseRM", Name: "ColBaseRMTurnover", MergeColNumber: 13 }]
            },
            {
                Columns: [{ HeaderText: "Real Turnover", Name: "ColBaseRMTurnover", MergeRowNumber: 2 },
                { HeaderText: "Virtual Turnover", Name: "ColBaseRMVirtualTurnover", MergeRowNumber: 2 },
                { HeaderText: "Turnover Variance", Name: "ColBaseRMTOVariancePlus", MergeColNumber: 3 },
                { HeaderText: "WinLoss Variance", Name: "ColBaseRMWLVariancePlus", MergeColNumber: 3 },
                { HeaderText: "Variance Margin(%)", Name: "ColBaseRMVarianceMargin", MergeRowNumber: 2 },
                { HeaderText: "Real Income", Name: "ColBaseRMTotal", MergeRowNumber: 2 },
                { HeaderText: "Virtual WinLoss", Name: "ColBaseRMVirtualTotal", MergeRowNumber: 2 },
                { HeaderText: "Real Margin(%)", Name: "ColBaseRMMargin", MergeRowNumber: 2 },
                { HeaderText: "Virtual Margin(%)", Name: "ColVirtualBaseRMMargin", MergeRowNumber: 2 }]
            }],
            columns: [{ HeaderText: "Currency", Width: 80, Name: "ColCurrencyName", HeaderAlign: "center", CellAlign: "center" },
                { HeaderText: "Real Turnover", Width: 120, Name: "ColTurnover", HeaderAlign: "center", CellAlign: "right", FormatNumber: FormatConstant.TWO_DECIMAL_NUMBER },
                { HeaderText: "ExtraPT", Width: 120, Name: "ColExtraPT", HeaderAlign: "center", CellAlign: "right", FormatNumber: FormatConstant.TWO_DECIMAL_NUMBER },
                { HeaderText: "Real WinLoss", Width: 120, Name: "ColWinLoss", HeaderAlign: "center", CellAlign: "right", FormatNumber: FormatConstant.TWO_DECIMAL_NUMBER, FormatPositive: '$' },
                { HeaderText: "Comm", Width: 120, Name: "ColCommission", HeaderAlign: "center", CellAlign: "right", FormatNumber: FormatConstant.TWO_DECIMAL_NUMBER, FormatPositive: '$' },
                { HeaderText: "Total", Width: 120, Name: "ColWinLossTotal", HeaderAlign: "center", CellAlign: "right", FormatNumber: FormatConstant.TWO_DECIMAL_NUMBER, FormatPositive: '$' },
                { HeaderText: "Real Turnover", Width: 120, Name: "ColBaseRMTurnover", HeaderAlign: "center", CellAlign: "right", FormatNumber: FormatConstant.TWO_DECIMAL_NUMBER, Sort: true },
                { HeaderText: "Virtual Turnover", Width: 120, Name: "ColBaseRMVirtualTurnover", HeaderAlign: "center", CellAlign: "right", FormatNumber: FormatConstant.TWO_DECIMAL_NUMBER, Sort: true },
                { HeaderText: "+", Width: 120, Name: "ColBaseRMTOVariancePlus", HeaderAlign: "center", CellAlign: "right", FormatNumber: FormatConstant.TWO_DECIMAL_NUMBER, Sort: true },
                { HeaderText: "-", Width: 120, Name: "ColBaseRMTOVarianceMinus", HeaderAlign: "center", CellAlign: "right", FormatNumber: FormatConstant.TWO_DECIMAL_NUMBER, Sort: true },
                { HeaderText: "Total", Width: 120, Name: "ColBaseRMTOVarianceAbsoluteTotal", HeaderAlign: "center", CellAlign: "right", FormatNumber: FormatConstant.TWO_DECIMAL_NUMBER, Sort: true },
                { HeaderText: "+", Width: 120, Name: "ColBaseRMWLVariancePlus", HeaderAlign: "center", CellAlign: "right", Sort: true, FormatNumber: FormatConstant.TWO_DECIMAL_NUMBER, FormatPositive: '$' },
                { HeaderText: "-", Width: 120, Name: "ColBaseRMWLVarianceMinus", HeaderAlign: "center", CellAlign: "right", Sort: true, FormatNumber: FormatConstant.TWO_DECIMAL_NUMBER, FormatPositive: '$' },
                { HeaderText: "Total", Width: 120, Name: "ColBaseRMWLVarianceTotal", HeaderAlign: "center", CellAlign: "right", Sort: true, FormatNumber: FormatConstant.TWO_DECIMAL_NUMBER, FormatPositive: '$' },
                { HeaderText: "Variance Margin(%)", Width: 120, Name: "ColBaseRMVarianceMargin", HeaderAlign: "center", CellAlign: "right", Sort: true, FormatPositive: '$', CustomSortFunction: self.customSortNumber },
                { HeaderText: "Real Income", Width: 120, Name: "ColBaseRMTotal", HeaderAlign: "center", CellAlign: "right", Sort: true, FormatNumber: FormatConstant.TWO_DECIMAL_NUMBER, FormatPositive: '$' },
                { HeaderText: "Virtual WinLoss", Width: 120, Name: "ColBaseRMVirtualTotal", HeaderAlign: "center", CellAlign: "right", Sort: true, FormatNumber: FormatConstant.TWO_DECIMAL_NUMBER, FormatPositive: '$' },
                { HeaderText: "Real Margin(%)", Width: 120, Name: "ColBaseRMMargin", HeaderAlign: "center", CellAlign: "right", Sort: true, FormatPositive: '$', CustomSortFunction: self.customSortNumber },
                { HeaderText: "Virtual Margin(%)", Width: 120, Name: "ColVirtualBaseRMMargin", HeaderAlign: "center", CellAlign: "right", Sort: true, FormatPositive: '$', CustomSortFunction: self.customSortNumber }],
            showTitle: true,
            showFooter: true,
            titleInfo:
            {
                text: "Win Loss By Currency",
                showExcelButton: true,
                exportExcelFunction: function () {
                    self.exportReport();
                }
            },
            showTree: false,
            bodyRows: bodyData,
            footerRows: footerData,
            gridExpandHeight: 185,
            numberOfFrozenColumn: 5,
            showDistinctRow: true
        };
        _tables.report.removeClass("display-none").fanexGrid(gridOptions);
    };

    self.exportReport = function () {
        AccReport.downloadFile(window.rootUrl + "WinLoss/TreeWinLoss/ExportTreeWinLossByProduct", self.Params.reportParams);
    };

    var btnModeClick = function () {
        if (self.Params.reportParams.IsNormalMode) {
            self.Params.reportParams.IsNormalMode = false;
            _tables.btnMode.val("BACK");
            _tables.normalMode.addClass("display-none");
            _tables.historyMode.removeClass("display-none");
        } else {
            self.Params.reportParams.IsNormalMode = true;
            _tables.btnMode.val("HISTORY REPORT");
            _tables.historyMode.addClass("display-none");
            _tables.normalMode.removeClass("display-none");
        }
    }
}