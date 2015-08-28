
Namespace.Register("Fanex.AccountingReports.Licensee.WinLossByLicensee.CurrencyReport");
Fanex.AccountingReports.Licensee.WinLossByLicensee.CurrencyReport = function () {
    var self = this;

    var _tables = {
        btnGenerate: null,
        dropdownLicensee: null,
        dropdownCurrency: null,
        dropdownDrawFilter: null,
        productsList: null,
        reportBy: null,
        datepickerplugin: null,
        currencyConvert: null,
        filterNote: null,

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
            tblContentId: 'currencyReportContent',
            manualCopyHeader: true,
            fullHeight: true,
            //fullWidth: true,
            //gridWidth: 1635,
            gridHeight: 275,
            //freezeWidth: 395,
            freezeHeight: 65,
            column: [100, 80, 140, 80, 120, 120, 120, 140, 120, 120, 120, 120, 120, 120, 120],
            numberFreezeCol: 4
        }
    };

    self.initPage = function () {
        initGridScrollerTables();
        cachedDOMElements();
        getLicenseeWithEnableBA();
        renderDefaultDropdownCurrency();
        renderFilterNote(noteMessage);
        renderDropdownProduct();
        //        renderDropdownDrawFilter();
        renderDatePicker();
        bindingEvent();
    };
    var initGridScrollerTables = function () {
        $("#currencyReport").initGrid({
            isShowPager: true
        });
    };
    var cachedDOMElements = function () {
        // We must cached dom element cause in this grid tables have many cells
        _tables.btnGenerate = $("#btnGenerate");
        _tables.dropdownLicensee = $("#ddlLicensee");
        _tables.dropdownCurrency = $("#ddlCurrency");
        _tables.dropdownDrawFilter = $("#ddlDrawFilter");
        _tables.reportBy = $("#reportBy");
        _tables.currencyConvert = $("#currencyConvert");
        _tables.filterNote = $("#notemessage");

        _tables.report = $("#currencyReport");
        _tables.reportHeader = $("#currencyReportHeader");
        _tables.reportContent = _tables.report.find("#currencyReportContent tbody");
        _tables.reportTitle = _tables.reportHeader.find("#currencyReportTitle");
        _tables.reportExportExcel = _tables.reportHeader.find("#currencyReport_ExportExcel");
        _tables.reportRowHeader = _tables.report.find("#currencyReportRowHeader tbody");
        _tables.reportRowFooter = _tables.report.find(".row_footer_content thead");
        _tables.reportColFooter = _tables.report.find(".col_footer_content thead");
        _tables.reportPagination = _tables.report.find("#pagination");
        _tables.colLicenseeWinLoss = _tables.report.find(".colLicenseeWinLoss");
        _tables.colIBCAmountDueTo = _tables.report.find(".colIBCAmountDueTo");
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
    var getLicenseeWithEnableBA = function () {
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
                renderLicenseeWithEnableBA(rs);
            },
            error: function (event, jqXHR, ajaxSettings) {
            },
            complete: function () {
                AccReport.enableInput();
            }
        });
    };
    var renderLicenseeWithEnableBA = function (licenseeList) {
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
    var getCurrencies = function () {
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
            url: window.rootUrl + "Licensee/Licensee/GetCurrenciesByLicensee",
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            data: JSON.stringify(data),
            success: function (rs) {
                renderDropdownCurrency(rs);
            },
            error: function (event, jqXHR, ajaxSettings) {
                renderDropdownCurrency(null);
            },
            complete: function () {
                AccReport.enableInput();
            }
        });
    };
    var renderDefaultDropdownCurrency = function () {
        var defaultCurrencyList = [{ Id: '0', Text: 'All Currencies'}];
        var options = '';
        var currencyCount = defaultCurrencyList.length;
        for (var i = 0; i < currencyCount; i++) {
            var currency = defaultCurrencyList[i];
            options += '<option value="' + currency.Id + '">' + currency.Text + '</option>';
        }

        _tables.dropdownCurrency.empty().append(options);
    };
    var renderDropdownCurrency = function (currencies) {
        renderDefaultDropdownCurrency();
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
    var renderDropdownDrawFilter = function () {
        var drawFilters = [{ Id: '1', Text: 'Normal' },
                           { Id: '3', Text: 'No Draw for Turnover only' },
                           { Id: '2', Text: 'No Draw'}];
        var options = '';
        var drawFilterCount = drawFilters.length;
        for (var i = 0; i < drawFilterCount; i++) {
            var drawFilter = drawFilters[i];
            options += '<option value="' + drawFilter.Id + '">' + drawFilter.Text + '</option>';
        }

        _tables.dropdownDrawFilter.append(options);
    };
    var renderDropdownProduct = function () {
        var noItemText = 'No product found.';
        var products = [{ Id: '0', Text: 'Sportsbook' },
                        { Id: '151', Text: 'Racing' },
                        { Id: '161', Text: 'Number Game' },
                        { Id: 'chbBA', Text: 'BA' },
                        { Id: '162', Text: 'Live Casino' },
                        { Id: '180', Text: 'Virtual Sports' },
                        { Id: '251', Text: 'RNG Casino' },
                        { Id: '202', Text: 'Keno' },
                        { Id: '250', Text: 'AG Casino' },
                        { Id: '255', Text: 'Gold Deluxe' },
                        { Id: '203', Text: 'Microgaming'}];

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
            selectedList: 3,
            listData: ddwnProducts
        });

        _tables.productsList = {
            sportsbook: $('#ddlProduct_0'),
            racing: $('#ddlProduct_151'),
            numbergame: $('#ddlProduct_161'),
            BA: $('#ddlProduct_chbBA'),
            liveCasino: $('#ddlProduct_162'),
            virtualSports: $('#ddlProduct_180'),
            rngCasino: $('#ddlProduct_251'),
            keno: $('#ddlProduct_202'),
            agc: $('#ddlProduct_250'),
            gd: $('#ddlProduct_255'),
            mgc: $('#ddlProduct_203')
        };
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
    var bindingEvent = function () {
        _tables.dropdownLicensee.change(function () {
            var licensee = parseInt(_tables.dropdownLicensee.val());
            if (licensee == enums.keenOcean) {
                _tables.dropdownCurrency.empty();
                renderDefaultDropdownCurrency();
            } else {
                getCurrencies();
            }
        });

        _tables.btnGenerate.click(function () {
            self.btnGenerateClick();
        });

        _tables.reportExportExcel.click(function () {
            exportReport();
        });
    };
    self.btnGenerateClick = function () {
        self.Options.paging.currentPage = 0;
        self.Params.reportParams.pageIndex = 1;
        self.getReport();
    };
    self.getReport = function () {
        AccReport.disableInput();
        getParams();
        var reportParams = {
            LicenseeID: self.Params.reportParams.LicenseeID,
            Currency: self.Params.reportParams.Currency,
            FromDate: self.Params.reportParams.FromDate,
            ToDate: self.Params.reportParams.ToDate,
            ShowSB: self.Params.reportParams.ShowSB,
            ShowRB: self.Params.reportParams.ShowRB,
            ShowNG: self.Params.reportParams.ShowNG,
            ShowBA: self.Params.reportParams.ShowBA,
            ShowLC: self.Params.reportParams.ShowLC,
            ShowVS: self.Params.reportParams.ShowVS,
            ShowCS: self.Params.reportParams.ShowCS,
            ShowKN: self.Params.reportParams.ShowKN,
            ShowAGC: self.Params.reportParams.ShowAGC,
            ShowGD: self.Params.reportParams.ShowGD,
            ShowMGC: self.Params.reportParams.ShowMGC,
            ReportType: self.Params.reportParams.ReportType,
            Drawfilter: self.Params.reportParams.Drawfilter,
            ConvertToCurrency: self.Params.reportParams.ConvertToCurrency,
            ItemsPerPage: self.Params.reportParams.pageSize,
            CurrentPage: self.Params.reportParams.pageIndex
        };

        if (validateParams() == true) {
            $.ajax({
                type: "POST",
                url: window.rootUrl + "Licensee/WinLossByLicensee/GetCurrencyReport",
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                data: JSON.stringify(reportParams),
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
        self.Params.reportParams.LicenseeID = parseInt(_tables.dropdownLicensee.val());
        self.Params.reportParams.LicenseeName = _tables.dropdownLicensee.find("option:selected").html();
        self.Params.reportParams.isMultiAgent = _tables.dropdownLicensee.find("option:selected").attr("isMultiAgent");
        self.Params.reportParams.Currency = parseInt(_tables.dropdownCurrency.val());
        self.Params.reportParams.CurrencyName = _tables.dropdownCurrency.find("option:selected").html();
        self.Params.reportParams.Drawfilter = parseInt(_tables.dropdownDrawFilter.val());
        self.Params.reportParams.ShowSB = checkBoxIsChecked(_tables.productsList.sportsbook);
        self.Params.reportParams.ShowRB = checkBoxIsChecked(_tables.productsList.racing);
        self.Params.reportParams.ShowNG = checkBoxIsChecked(_tables.productsList.numbergame);
        self.Params.reportParams.ShowBA = checkBoxIsChecked(_tables.productsList.BA);
        self.Params.reportParams.ShowLC = checkBoxIsChecked(_tables.productsList.liveCasino);
        self.Params.reportParams.ShowVS = checkBoxIsChecked(_tables.productsList.virtualSports);
        self.Params.reportParams.ShowCS = checkBoxIsChecked(_tables.productsList.rngCasino);
        self.Params.reportParams.ShowKN = checkBoxIsChecked(_tables.productsList.keno);
        self.Params.reportParams.ShowAGC = checkBoxIsChecked(_tables.productsList.agc);
        self.Params.reportParams.ShowGD = checkBoxIsChecked(_tables.productsList.gd);
        self.Params.reportParams.ShowMGC = checkBoxIsChecked(_tables.productsList.mgc);

        var dateRange = _tables.datepickerplugin.nexDatePicker("getDateRange");
        self.Params.reportParams.FromDate = dateRange.FromDate;
        self.Params.reportParams.ToDate = dateRange.ToDate;
        self.Params.reportParams.ReportType = _tables.reportBy.find("input[name='rdReportBy']:checked").val();
        self.Params.reportParams.ConvertToCurrency = _tables.currencyConvert.find("input[name='rdCurrencyConvert']:checked").val();
        self.Params.reportParams.ConvertToCurrencyName = _tables.currencyConvert.find("input[name='rdCurrencyConvert']:checked").attr("text");
    };
    var validateParams = function () {
        var isValid = true;
        var message = '';
        var notifyIds = [];
        if (self.Params.reportParams.LicenseeID == 0) {
            isValid = false;
            if (message == '') {
                message = "Please select one licensee!";
            }
            notifyIds.push("ddlLicensee");
        }

        if (self.Params.reportParams.ShowSB == false &&
            self.Params.reportParams.ShowRB == false &&
            self.Params.reportParams.ShowNG == false &&
            self.Params.reportParams.ShowBA == false &&
            self.Params.reportParams.ShowLC == false &&
            self.Params.reportParams.ShowVS == false &&
            self.Params.reportParams.ShowCS == false &&
            self.Params.reportParams.ShowKN == false &&
            self.Params.reportParams.ShowAGC == false &&
            self.Params.reportParams.ShowGD == false &&
            self.Params.reportParams.ShowGD == false &&
            self.Params.reportParams.ShowMGC == false) {
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
        var reportTitle = "Win Loss by Licensee report for " + self.Params.reportParams.LicenseeName + " from " + self.Params.reportParams.FromDate +
            " to " + self.Params.reportParams.ToDate +
            " in " + self.Params.reportParams.CurrencyName +
            (self.Params.reportParams.ConvertToCurrency == 0
            ? '' : " (Base " + self.Params.reportParams.ConvertToCurrencyName + ")");

        _tables.reportTitle.html(reportTitle);
        _tables.colLicenseeWinLoss.html(self.getColLicenseeWinLoss());
        _tables.colIBCAmountDueTo.html(self.getColLicenseeWinLossSuffix());

        self.resetTempHtml();

        if (report &&
            report.TransactionList &&
            report.TransactionList.length) {
            _tables.reportExportExcel.removeClass("display-none");
            var dateGroupNumber = report.TransactionList.length;
            //            var drillDownLevel = self.Params.reportParams.isMultiAgent ?
            //                                            enums.drilldownLevel.Agent :
            //                                            enums.drilldownLevel.Member;

            for (var i = 0; i < dateGroupNumber; i++) {
                var cssClass = i % 2 == 0 ? 'even' : '';
                var dateGroup = report.TransactionList[i];
                var dateGroupLength = dateGroup.TransactionList.length;

                for (var j = 0; j < dateGroupLength; j++) {
                    var frozenHtml = '';
                    var reportItem = dateGroup.TransactionList[j];
                    if (j == 0) {
                        frozenHtml = '<td rowspan="' + dateGroupLength + '" class="' + cssClass + '"><div class="col100 align-center">' + dateGroup.BettingDate + '</div></td>';
                    }

                    frozenHtml += '<td><div class="col80 align-center link" onclick="' +
                                                            getDrilldownLink(reportItem) + '">' +
                                                            reportItem.CurrencyName + '</div></td>\
                                                   <td><div class="col140 align-right">' + numeral(reportItem.Turnover).format(FormatConstant.TWO_DECIMAL_NUMBER) + '</div></td>\
                                                   <td><div class="col80 align-right">' + numeral(reportItem.BetCount).format(FormatConstant.INT_NUMBER) + '</div></td>';

                    self.TempHtml.frozenHtml += '<tr class="' + cssClass + '">' + frozenHtml + ' </tr>';
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
                }
            }

            var totalLength = report.Total.length;
            for (var k = 0; k < totalLength; k++) {
                var totalItem = report.Total[k];
                self.renderTotalRow(totalItem, k == 0, totalLength, enums.drilldownLevel.Currency);
            }

            var baseCurrencyTotalLength = report.BaseCurrencyTotal.length;
            for (var l = 0; l < baseCurrencyTotalLength; l++) {
                var baseCurrencyTotalItem = report.BaseCurrencyTotal[l];
                self.renderBaseCurrencyTotalRow(baseCurrencyTotalItem, l == 0, baseCurrencyTotalLength, enums.drilldownLevel.Currency);
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
        showReport(enums.drilldownLevel.Currency);
    };
    var renderNoDataReport = function () {
        _tables.reportExportExcel.addClass("display-none");
        var frezeeContent = "<td colspan='4'>&nbsp;</td>";
        _tables.reportRowHeader.empty().append("<tr>" + frezeeContent + "</tr>");
        _tables.reportContent.empty().append(
                "<tr>" +
                    frezeeContent +
                    "<td colspan='11' class='c'><div class='no-data-message-currency-report'>There is no data.</div></td>\
                 </tr>");
        _tables.reportRowFooter.empty();
        _tables.reportColFooter.empty();
    };
    var getDrilldownLink = function (row) {
        var link = '';
        link = 'drilldownToSuperReport(' + row.CurrencyId + ',\'' +
                                               row.CurrencyName + '\',\'' +
                                               row.BettingDate + '\',\'' +
                                               row.FromDate + '\',\'' +
                                               row.ToDate + '\')';
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
            self.getReport();
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
    };
    var exportReport = function () {
        AccReport.downloadFile(window.rootUrl +
            "Licensee/WinLossByLicensee/ExportCurrencyReport",
            self.Params.reportParams);
    };
}