Namespace.Register("Fanex.AccountingReports.Licensee.KenoWinLossReport");

Fanex.AccountingReports.Licensee.KenoWinLossReport = function () {
    var self = this;

    var _tables = {
        btnGenerate: null,
        dropdownLicensee: null,
        dropdownCurrency: null,
        dropdownCountryMarket: null,
        datepickerplugin: null,
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
            CountryMarket: null,
            FromDate: null,
            ToDate: null
        }
    };

    self.initPage = function () {
        self.cachedDOMElements();
        self.renderDatePicker();
        self.renderFilterNote(noteMessage);
        self.bindingEvents();
        self.getLicensee();
    };

    self.cachedDOMElements = function () {
        _tables.btnGenerate = $("#btnGenerate");
        _tables.dropdownLicensee = $("#ddlLicensee");
        _tables.dropdownCurrency = $("#Currency"),
        _tables.dropdownCountryMarket = $('#CountryMarket');
        _tables.filterNote = $("#notemessage");
        _tables.reportContainer = $("#kenoWinLossContainer");
        _tables.reportContent = $("#kenoWinLossContent");
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
                LabelCss: "lbl-field col_2",
                OptionCss: "edit-field col_7",
                TextboxCss: "edit-field col_7"
            },
            firstDatePicker: {
                Id: "firstDatePickerId1",
                Label: labelFrom,
                InitDate: today,
                LabelCss: "lbl-field col_4",
                TextboxCss: "edit-field col_5"
            },
            secondDatePicker: {
                Id: "secondDatePickerId1",
                Label: labelTo,
                InitDate: today,
                LabelCss: "lbl-field col_1",
                TextboxCss: "edit-field col_5"
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

    self.getLicensee = function () {
        AccReport.disableInput();
        AccReport.utility.CallAjax(this,
                                    window.rootUrl + "Licensee/KenoWinLoss/GetLicensee",
                                    null,
                                    self.renderLicensee,
                                    AccReport.enableInput,
                                    AccReport.enableInput);
    };

    self.renderLicensee = function (licenseeList) {
        var noItemText = '';
        if (licenseeList.length == 0) {
            noItemText = 'No licensee found.';
        }
        else {
            for (var i = 0; i < licenseeList.length; i++) {
                licenseeList[i].Id = "Licensee_" + licenseeList[i].Value;
                licenseeList[i].Enable = true;
            }
        }

        $("#ddlLicensee").multiSelect(
        {
            minWidth: 170,
            maxHeight: 450,
            shadow: true,
            showHeader: true,
            isCheckAll: true,
            showFooter: false,
            checkAllText: 'All Licensees',
            isUsedCustomCheckAllText: true,
            customCheckAllText: 'All Licensees',
            noItemText: noItemText,
            listData: licenseeList,
            onCheck: function () {
                var selectedLicensees = _tables.dropdownLicensee.multiSelect('getCheckedItems');
                if (selectedLicensees.length == 1 && selectedLicensees[0] == "1") {
                    $('#firstDatePickerId1').datepicker('option', 'minDate', new Date(minDate12BET));
                    $('#secondDatePickerId1').datepicker('option', 'minDate', new Date(minDate12BET));
                }
                else {
                    $('#firstDatePickerId1').datepicker('option', 'minDate', new Date(minDate));
                    $('#secondDatePickerId1').datepicker('option', 'minDate', new Date(minDate));
                }
                self.getCurrencyByLicensee();
            }
        });

        _tables.dropdownLicensee = $('#ddlLicensee');
        self.getCurrencyByLicensee();
    };

    self.getCurrencyByLicensee = function () {
        AccReport.disableInput();
        var param = {};
        param.LicenseeID = _tables.dropdownLicensee.multiSelect('getCheckedItems').join(',');
        AccReport.utility.CallAjax(this,
                                    window.rootUrl + "Licensee/KenoWinLoss/GetCurrencyByLicensee",
                                    param,
                                    self.renderCurrency,
                                    self.renderCurrency,
                                    AccReport.enableInput);
    };

    self.renderCurrency = function (currencyList) {
        var options = '';
        for (var i = 0; i < currencyList.length; i++) {
            options += '<option value=' + currencyList[i].Value + '>' + currencyList[i].Text + '</option>';
        }

        _tables.dropdownCurrency.find('option').remove().end().append(options);
    };

    self.bindingEvents = function () {
        _tables.btnGenerate.click(function () {
            self.generateKenoWinLossReport();
        });
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
        self.params.reportParams.LicenseeID = _tables.dropdownLicensee.multiSelect('getCheckedItems').join(',');
        self.params.reportParams.LicenseeFileNameString = _tables.dropdownLicensee.multiSelect('getUncheckItems').length == 0 ? "All Licensees" : _tables.dropdownLicensee.multiSelect('getCheckedItemsByText').join(',');
        self.params.reportParams.Currency = _tables.dropdownCurrency.val();
        self.params.reportParams.CountryMarket = parseInt(_tables.dropdownCountryMarket.val());
        var dateRange = _tables.datepickerplugin.nexDatePicker("getDateRange");
        self.params.reportParams.FromDate = dateRange.FromDate;
        self.params.reportParams.ToDate = dateRange.ToDate;
    };

    self.exportExcelKenoWinLoss = function () {
        AccReport.downloadFile(window.rootUrl + "Licensee/KenoWinLoss/ExportKenoWinLossReport", self.params.reportParams);
    }

    self.generateKenoWinLossReport = function () {
        AccReport.disableInput();
        self.validateParams();
        self.getParams();
        AccReport.utility.CallAjax(this,
                                    window.rootUrl + "Licensee/KenoWinLoss/GetKenoWinLossReport2",
                                    self.params.reportParams,
                                    self.renderGrid2,
                                    AccReport.enableInput,
                                    AccReport.enableInput);
    }

    self.renderGrid = function (data) {
        var title = "Keno Win Loss Report from " + self.params.reportParams.FromDate + " to " + self.params.reportParams.ToDate + " - " + _tables.dropdownCountryMarket.find('option:selected').text() + " Country/Market";
        var reportData = [];
        var footerData = [];
        for (var i = 0; i < data.ReportRows.length; i++) {
            var row = [];
            row.push({ Name: "Licensee", Value: data.ReportRows[i].Licensee });
            row.push({ Name: "Turnover", Value: data.ReportRows[i].TurnOver });
            row.push({ Name: "TotalTransactions", Value: data.ReportRows[i].TotalTransaction });
            row.push({ Name: "GrossWin", Value: data.ReportRows[i].GrossWin });
            reportData.push({ Columns: row });
        }

        var totalRow = [];
        totalRow.push({ Name: "Licensee", Value: data.Total.Licensee });
        totalRow.push({ Name: "Turnover", Value: data.Total.TurnOver });
        totalRow.push({ Name: "TotalTransactions", Value: data.Total.TotalTransaction });
        totalRow.push({ Name: "GrossWin", Value: data.Total.GrossWin });
        footerData.push({ Columns: totalRow });

        var gridOptions =
            {
                columns: [
                    { HeaderText: "Licensee", Width: 300, Name: "Licensee", HeaderAlign: "center", CellAlign: "left", CellCssClass: "bold" },
                    { HeaderText: "Turnover", Width: 300, Name: "Turnover", HeaderAlign: "center", CellAlign: "right", FormatNumber: FormatConstant.TWO_DECIMAL_NUMBER },
                    { HeaderText: "Total Transactions", Width: 300, Name: "TotalTransactions", HeaderAlign: "center", CellAlign: "right", FormatNumber: FormatConstant.INT_NUMBER },
                    { HeaderText: "Gross Win", Width: 300, Name: "GrossWin", HeaderAlign: "center", CellAlign: "right", FormatNumber: FormatConstant.TWO_DECIMAL_NUMBER }
                ],
                bodyRows: reportData,
                footerRows: footerData,
                showNumberColumn: false,
                gridExpandHeight: 330,
                showTitle: true,
                showFooter: data.ReportRows.length != 0,
                titleInfo: {
                    text: title,
                    showExcelButton: true,
                    exportExcelFunction: function () {
                        self.exportExcelKenoWinLoss();
                    }
                },
                noDataText: "No information is available",
                pagerOption: {
                    itemsPerPage: 20,
                    currentPage: 0,
                    itemsPerPageArray: [10, 20, 50, 100]
                }
            };

        _tables.reportContent.removeClass("display-none").fanexGrid(gridOptions);
    }

    self.renderGrid2 = function (gridOptions) {
        gridOptions.showNumberColumn = false;
        gridOptions.gridExpandHeight = 330;
        gridOptions.showTitle = true;
        gridOptions.titleInfo = {
            text: gridOptions.TitleInfoText,
            showExcelButton: true,
            exportExcelFunction: function () {
                self.exportExcelKenoWinLoss();
            }
        };
        gridOptions.pagerOption = {
            itemsPerPage: 20,
            currentPage: 0,
            itemsPerPageArray: [10, 20, 50, 100]
        };
        _tables.reportContent.removeClass("display-none").fanexGrid(gridOptions);
    }
};

$(document).ready(function () {
    var kenoWinLossReport = new Fanex.AccountingReports.Licensee.KenoWinLossReport();
    kenoWinLossReport.initPage();
});