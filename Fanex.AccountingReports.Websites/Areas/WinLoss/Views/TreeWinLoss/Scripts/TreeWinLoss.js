
Namespace.Register("Fanex.AccountingReports.WinLoss.TreeWinLoss.TreeWinLoss");
Fanex.AccountingReports.WinLoss.TreeWinLoss.TreeWinLoss = function () {
    var self = this;
    var _tables = {
        btnGenerate: null,
        datepickerplugin: null,
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
        }
    };

    self.Params = {
        reportParams: {
            FromDate: new Date(),
            ToDate: new Date(),
            ShowSB: true,
            ShowNG: true,
            ShowRB: true,
            ShowBI: true,
            ShowCasino: true,
            ShowLiveCasino: true,
            ShowP2P: true,
            ShowVS: true,
            ShowKeno: true,
            ShowGD: true,
            ShowMGSL: true,
            ShowAGCS: true,
            ShowVG: true,
            ShowMGCLC: true,
            ShowMGCRNG: true,
            ShowCB: true
        }
    };

    self.initPage = function () {
        cachedDOMElements();
        renderDatePicker();
        renderProductsDropdown();
        renderFilterNote(noteMessage);
        bindingEvent();
    };

    var cachedDOMElements = function () {
        _tables.btnGenerate = $("#btnGenerate");
        _tables.reportContainer = $("#treeWinLossByProductContainer");
        _tables.report = $("#treeWinLossByProduct");
        _tables.filterNote = $("#notemessage");
    };

    var renderDatePicker = function () {
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
                LabelCss: "lbl-field col_2",
                OptionCss: "edit-field col_5"
            },
            firstDatePicker: {
                Id: "firstDatePickerId1",
                Label: labelFrom,
                InitDate: today,
                LabelCss: "lbl-field col_2",
                TextboxCss: "edit-field col_5"
            },
            secondDatePicker: {
                Id: "secondDatePickerId1",
                Label: labelTo,
                InitDate: today,
                LabelCss: "lbl-field col_2",
                TextboxCss: "edit-field col_6"
            },
            selectedItem: 0,
            onSelectDatePicker: function () {
            },
            minDate: new Date(minDate),
            maxDate: new Date(today)
        });

        _tables.datepickerplugin = $("#datepickerplugin");
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

    var renderProductsDropdown = function () {
        var noItemText = 'No product found.';
        var products = [{ Id: 'chbSport', Text: 'Sportsbook' },
                        { Id: 'chbNG', Text: 'Number Game' },
                        { Id: 'chbRacing', Text: 'Racing' },
                        { Id: 'chbBingo', Text: 'Bingo' },
                        { Id: 'chbRNGCasino', Text: 'Casino' },
                        { Id: 'chbMacau', Text: 'Macau' },
                        { Id: 'chbLiveCasino', Text: 'Live Casino' },
                        { Id: 'chbGoldDeluxe', Text: 'Gold Deluxe' },
                        { Id: 'chbAGCasino', Text: 'AG Casino' },
                        { Id: 'chbPoker', Text: 'Poker' },
                        { Id: 'chbVirtualSports', Text: 'Virtual Sports' },
                        { Id: 'chbKeno', Text: 'Keno' },
                        { Id: 'chbMetricGamingSuperLive', Text: 'Metric Gaming - Super Live' },
                        { Id: 'chbMicroGamingLiveCasino', Text: 'Microgaming - Live Casino' },
                        { Id: 'chbMicroGamingRNGCasino', Text: 'Microgaming - RNG Casino' },
                        { Id: 'chbColossusBets', Text: 'Colossus Bets'}];

        var len = products.length;
        var ddwnProducts = [];
        var prefixId = 'ddlProduct_';
        for (var i = 0; i < len; i++) {
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
            listData: ddwnProducts
        });

        _tables.productsList = {
            sportsbook: $('#ddlProduct_chbSport'),
            racing: $('#ddlProduct_chbRacing'),
            numbergame: $('#ddlProduct_chbNG'),
            liveCasino: $('#ddlProduct_chbLiveCasino'),
            goldDeluxe: $('#ddlProduct_chbGoldDeluxe'),
            virtualSports: $('#ddlProduct_chbVirtualSports'),
            rngCasino: $('#ddlProduct_chbRNGCasino'),
            bingo: $('#ddlProduct_chbBingo'),
            poker: $('#ddlProduct_chbPoker'),
            keno: $('#ddlProduct_chbKeno'),
            metricGamingSuperLive: $('#ddlProduct_chbMetricGamingSuperLive'),
            microgamingLiveCasino: $('#ddlProduct_chbMicroGamingLiveCasino'),
            microgamingRNGCasino: $('#ddlProduct_chbMicroGamingRNGCasino'),
            agCasino: $('#ddlProduct_chbAGCasino'),
            macau: $('#ddlProduct_chbMacau'),
            colossusBets: $('#ddlProduct_chbColossusBets')
        };
    };

    var checkBoxIsChecked = function (checkBox) {
        return checkBox.attr("checked") ? true : false;
    };

    var bindingEvent = function () {
        _tables.btnGenerate.click(function () {
            self.btnGenerateClick();
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
        var dateRange = _tables.datepickerplugin.nexDatePicker("getDateRange");
        self.Params.reportParams.FromDate = dateRange.FromDate;
        self.Params.reportParams.ToDate = dateRange.ToDate;
        self.Params.reportParams.ShowSB = checkBoxIsChecked(_tables.productsList.sportsbook);
        self.Params.reportParams.ShowNG = checkBoxIsChecked(_tables.productsList.numbergame);
        self.Params.reportParams.ShowRB = checkBoxIsChecked(_tables.productsList.racing);
        self.Params.reportParams.ShowBI = checkBoxIsChecked(_tables.productsList.bingo);
        self.Params.reportParams.ShowCasino = checkBoxIsChecked(_tables.productsList.rngCasino);
        self.Params.reportParams.ShowLiveCasino = checkBoxIsChecked(_tables.productsList.liveCasino);
        self.Params.reportParams.ShowP2P = checkBoxIsChecked(_tables.productsList.poker);
        self.Params.reportParams.ShowVS = checkBoxIsChecked(_tables.productsList.virtualSports);
        self.Params.reportParams.ShowKeno = checkBoxIsChecked(_tables.productsList.keno);
        self.Params.reportParams.ShowGD = checkBoxIsChecked(_tables.productsList.goldDeluxe);
        self.Params.reportParams.ShowMGSL = checkBoxIsChecked(_tables.productsList.metricGamingSuperLive);
        self.Params.reportParams.ShowMGCLC = checkBoxIsChecked(_tables.productsList.microgamingLiveCasino);
        self.Params.reportParams.ShowMGCRNG = checkBoxIsChecked(_tables.productsList.microgamingRNGCasino);
        self.Params.reportParams.ShowAGCS = checkBoxIsChecked(_tables.productsList.agCasino);
        self.Params.reportParams.ShowVG = checkBoxIsChecked(_tables.productsList.macau);
        self.Params.reportParams.ShowCB = checkBoxIsChecked(_tables.productsList.colossusBets);
    };

    var validateParams = function () {
        var isValid = true;
        var message = '';
        var notifyIds = [];

        if (self.Params.reportParams.ShowSB == false &&
            self.Params.reportParams.ShowRB == false &&
            self.Params.reportParams.ShowNG == false &&
            self.Params.reportParams.ShowBI == false &&
            self.Params.reportParams.ShowLiveCasino == false &&
            self.Params.reportParams.ShowCasino == false &&
            self.Params.reportParams.ShowVS == false &&
            self.Params.reportParams.ShowKeno == false &&
            self.Params.reportParams.ShowP2P == false &&
            self.Params.reportParams.ShowGD == false &&
            self.Params.reportParams.ShowMGSL == false &&
            self.Params.reportParams.ShowMGCLC == false &&
            self.Params.reportParams.ShowMGCRNG == false &&
            self.Params.reportParams.ShowVG == false &&
            self.Params.reportParams.ShowAGCS == false &&
            self.Params.reportParams.ShowCB == false) {
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
        var isShowNA = false;
        if ((!self.Params.reportParams.ShowSB &&
                !self.Params.reportParams.ShowNG &&
                !self.Params.reportParams.ShowKeno &&
                !self.Params.reportParams.ShowRB &&
                !self.Params.reportParams.ShowLiveCasino &&
                !self.Params.reportParams.ShowVS &&
                !self.Params.reportParams.ShowGD &&
                !self.Params.reportParams.ShowMGSL &&
                !self.Params.reportParams.ShowVG &&
                !self.Params.reportParams.ShowAGCS &&
                !self.Params.reportParams.ShowMGCLC &&
                !self.Params.reportParams.ShowMGCRNG &&
                !self.Params.reportParams.ShowCB) && (self.Params.reportParams.ShowCasino || self.Params.reportParams.ShowBI || self.Params.reportParams.ShowP2P)) {
            isShowNA = true;
        }
        if (report && report.ChildsInfo.length > 0) {
            var len = report.ChildsInfo.length;
            var info = null;
            for (var i = 0; i < len; i++) {
                info = report.ChildsInfo[i];
                info.BaseRMTurnover = isShowNA ? "N/A" : info.BaseRMTurnover;
                info.BaseRMVirtualTurnover = isShowNA ? "N/A" : info.BaseRMVirtualTurnover;
                bodyData.push({
                    Columns: [{ Name: "ColCurrencyName", Value: {
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
                        { Name: "ColBaseRMMargin", Value: info.BaseRMMargin }
                    ]
                });
            }

            for (var j = 0; j < report.TotalInfo.length; j++) {
                info = report.TotalInfo[j];
                info.BaseRMTurnover = isShowNA ? "N/A" : info.BaseRMTurnover;
                info.BaseRMVirtualTurnover = isShowNA ? "N/A" : info.BaseRMVirtualTurnover;
                footerData.push({
                    Columns: [{ Name: "ColCurrencyName", Value: info.Name, MergeColNumber: 5, CellAlign: "right" },
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
                        { Name: "ColBaseRMMargin", Value: info.BaseRMMargin }
                    ]
                });
            }
        }

        var gridOptions = {
            customizeHeader: true,
            groupHeaderRows: [
            {
                Columns: [{ HeaderText: "Currency", Name: "ColCurrencyName", MergeRowNumber: 3 },
                { HeaderText: "Turnover", Name: "ColTurnover", MergeRowNumber: 3 },
                { HeaderText: "WinLoss", Name: "ColWinLoss", MergeRowNumber: 3 },
                { HeaderText: "Comm", Name: "ColCommission", MergeRowNumber: 3 },
                { HeaderText: "Total", Name: "ColWinLossTotal", MergeRowNumber: 3 },
                { HeaderText: "BaseRM", Name: "ColBaseRMTurnover", MergeColNumber: 12}]
            },
            {
                Columns: [{ HeaderText: "Turnover", Name: "ColBaseRMTurnover", MergeRowNumber: 2 },
                { HeaderText: "Virtual Turnover", Name: "ColBaseRMVirtualTurnover", MergeRowNumber: 2 },
                { HeaderText: "Turnover Variance", Name: "ColBaseRMTOVariancePlus", MergeColNumber: 3 },
                { HeaderText: "Winlost Variance", Name: "ColBaseRMWLVariancePlus", MergeColNumber: 3 },
                { HeaderText: "Variance Margin(%)", Name: "ColBaseRMVarianceMargin", MergeRowNumber: 2 },
                { HeaderText: "Total", Name: "ColBaseRMTotal", MergeRowNumber: 2 },
                { HeaderText: "Virtual Total", Name: "ColBaseRMVirtualTotal", MergeRowNumber: 2 },
                { HeaderText: "Margin(%)", Name: "ColBaseRMMargin", MergeRowNumber: 2}]
            }],
            columns: [{ HeaderText: "Currency", Width: 80, Name: "ColCurrencyName", HeaderAlign: "center", CellAlign: "center" },
                { HeaderText: "Turnover", Width: 120, Name: "ColTurnover", HeaderAlign: "center", CellAlign: "right", FormatNumber: FormatConstant.TWO_DECIMAL_NUMBER },
                { HeaderText: "WinLoss", Width: 120, Name: "ColWinLoss", HeaderAlign: "center", CellAlign: "right", FormatNumber: FormatConstant.TWO_DECIMAL_NUMBER, FormatPositive: '$' },
                { HeaderText: "Comm", Width: 120, Name: "ColCommission", HeaderAlign: "center", CellAlign: "right", FormatNumber: FormatConstant.TWO_DECIMAL_NUMBER, FormatPositive: '$' },
                { HeaderText: "Total", Width: 120, Name: "ColWinLossTotal", HeaderAlign: "center", CellAlign: "right", FormatNumber: FormatConstant.TWO_DECIMAL_NUMBER, FormatPositive: '$' },
                { HeaderText: "Turnover", Width: 120, Name: "ColBaseRMTurnover", HeaderAlign: "center", CellAlign: "right", FormatNumber: FormatConstant.TWO_DECIMAL_NUMBER, Sort: true },
                { HeaderText: "Virtual Turnover", Width: 120, Name: "ColBaseRMVirtualTurnover", HeaderAlign: "center", CellAlign: "right", FormatNumber: FormatConstant.TWO_DECIMAL_NUMBER, Sort: true },
                { HeaderText: "+", Width: 120, Name: "ColBaseRMTOVariancePlus", HeaderAlign: "center", CellAlign: "right", FormatNumber: FormatConstant.TWO_DECIMAL_NUMBER, Sort: true },
                { HeaderText: "-", Width: 120, Name: "ColBaseRMTOVarianceMinus", HeaderAlign: "center", CellAlign: "right", FormatNumber: FormatConstant.TWO_DECIMAL_NUMBER, Sort: true },
                { HeaderText: "Absolute Total", Width: 120, Name: "ColBaseRMTOVarianceAbsoluteTotal", HeaderAlign: "center", CellAlign: "right", FormatNumber: FormatConstant.TWO_DECIMAL_NUMBER, Sort: true },
                { HeaderText: "+", Width: 120, Name: "ColBaseRMWLVariancePlus", HeaderAlign: "center", CellAlign: "right", Sort: true, FormatNumber: FormatConstant.TWO_DECIMAL_NUMBER, FormatPositive: '$' },
                { HeaderText: "-", Width: 120, Name: "ColBaseRMWLVarianceMinus", HeaderAlign: "center", CellAlign: "right", Sort: true, FormatNumber: FormatConstant.TWO_DECIMAL_NUMBER, FormatPositive: '$' },
                { HeaderText: "Total", Width: 120, Name: "ColBaseRMWLVarianceTotal", HeaderAlign: "center", CellAlign: "right", Sort: true, FormatNumber: FormatConstant.TWO_DECIMAL_NUMBER, FormatPositive: '$' },
                { HeaderText: "Variance Margin(%)", Width: 120, Name: "ColBaseRMVarianceMargin", HeaderAlign: "center", CellAlign: "right", Sort: true, FormatPositive: '$', CustomSortFunction: self.customSortNumber },
                { HeaderText: "Total", Width: 120, Name: "ColBaseRMTotal", HeaderAlign: "center", CellAlign: "right", Sort: true, FormatNumber: FormatConstant.TWO_DECIMAL_NUMBER, FormatPositive: '$' },
                { HeaderText: "Virtual Total", Width: 120, Name: "ColBaseRMVirtualTotal", HeaderAlign: "center", CellAlign: "right", Sort: true, FormatNumber: FormatConstant.TWO_DECIMAL_NUMBER, FormatPositive: '$' },
                { HeaderText: "Margin(%)", Width: 120, Name: "ColBaseRMMargin", HeaderAlign: "center", CellAlign: "right", Sort: true, FormatPositive: '$', CustomSortFunction: self.customSortNumber}],
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

}