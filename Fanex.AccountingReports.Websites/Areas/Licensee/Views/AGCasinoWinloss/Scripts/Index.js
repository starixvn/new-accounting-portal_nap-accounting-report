/* global Namespace,noteMessage,FormatConstant,labelFrom,today,labelTo,today,minDate,AccReport,reportTitle,moment */
//"use strict";
Namespace.Register("Fanex.AccountingReports.Licensee.AGCasinoWinloss");
var aGCasinoWinloss;
Fanex.AccountingReports.Licensee.AGCasinoWinloss = function () {
    var self = this;

    var _tables = {
        btnGenerate: null,
        dropdownLicensee: null,
        dropdownCurrency: null,
        checkTableTypeNormal: null,
        checkTableTypeVIP: null,
        checkExcludeTest: null,
        datepickerplugin: null,
        reportMainContainer: null,
        reportMainGrid: null,
        reportSuperContainer: null,
        reportSuperGrid: null,
        reportMasterContainer: null,
        reportMasterGrid: null,
        reportAgentContainer: null,
        reportAgentGrid: null,
        reportMemberContainer: null,
        reportMemberGrid: null,
        hidPageSize: null,
        hidPageIndex: null,
        hidIsRefresh: null,
        hidReportTypeEnum: null,
        hidCustID: null,
        hidCustomerName: null,
        hidCurrency: null,
        hidFromDate: null,
        hidToDate: null
    };

    self.reportTypeEnum = {
        MainLevel: 0,
        SuperLevel: 1,
        MasterLevel: 2,
        AgentLevel: 3,
        MemberLevel: 4
    };

    self.currentLevel = self.reportTypeEnum.MainLevel;

    self.params = {
        reportMainParams: {
            LicenseeID: null,
            LicenseeName: null,
            Currency: null,
            CurrencyName: null,
            ReportType: null,
            ConvertFiguresTo: null,
            ExcludeTest: null,
            FromDate: null,
            ToDate: null
        },
        reportSuperParams: {
            LicenseeID: null,
            LicenseeName: null,
            Currency: null,
            CurrencyName: null,
            ReportType: null,
            ConvertFiguresTo: null,
            ExcludeTest: null,
            FromDate: null,
            ToDate: null
        },
        reportMasterParams: {
            LicenseeID: null,
            LicenseeName: null,
            Currency: null,
            CurrencyName: null,
            CustID: null,
            CustomerName: null,
            ReportType: null,
            ConvertFiguresTo: null,
            ExcludeTest: null,
            FromDate: null,
            ToDate: null
        },
        reportAgentParams: {
            LicenseeID: null,
            LicenseeName: null,
            Currency: null,
            CurrencyName: null,
            CustID: null,
            CustomerName: null,
            ReportType: null,
            ConvertFiguresTo: null,
            ExcludeTest: null,
            FromDate: null,
            ToDate: null
        },
        reportMemberParams: {
            LicenseeID: null,
            LicenseeName: null,
            Currency: null,
            CurrencyName: null,
            CustID: null,
            CustomerName: null,
            ReportType: null,
            ConvertFiguresTo: null,
            ExcludeTest: null,
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
        _tables.dropdownLicensee = $("#LicenseeID");
        _tables.dropdownCurrency = $("#Currency");
        _tables.checkExcludeTest = $('#ExcludeTest');
        _tables.datepickerplugin = $('#datepickerplugin');
        _tables.filterNote = $("#notemessage");
        _tables.reportMainContainer = $("#AGCasinoWinLossMainContainer");
        _tables.reportMainGrid = $("#AGCasinoWinLossMainGrid");
        _tables.reportSuperContainer = $("#AGCasinoWinLossSuperContainer");
        _tables.reportSuperGrid = $("#AGCasinoWinLossSuperGrid");
        _tables.reportMasterContainer = $("#AGCasinoWinLossMasterContainer");
        _tables.reportMasterGrid = $("#AGCasinoWinLossMasterGrid");
        _tables.reportAgentContainer = $("#AGCasinoWinLossAgentContainer");
        _tables.reportAgentGrid = $("#AGCasinoWinLossAgentGrid");
        _tables.reportMemberContainer = $("#AGCasinoWinLossMemberContainer");
        _tables.reportMemberGrid = $("#AGCasinoWinLossMemberGrid");
        _tables.hidPageSize = $("#PageSize");
        _tables.hidPageIndex = $("#PageIndex");
        _tables.hidIsRefresh = $("#IsRefresh");
        _tables.hidReportTypeEnum = $("#ReportTypeEnum");
        _tables.hidCustID = $("#CustID");
        _tables.hidCustomerName = $("#CustomerName");
        _tables.hidCurrency = $('#CurrencyID');
        _tables.hidFromDate = $('#FromDate_SMAM');
        _tables.hidToDate = $('#ToDate_SMAM');
    };

    self.renderDatePicker = function () {
        _tables.datepickerplugin.html("");
        _tables.datepickerplugin.nexDatePicker({
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
                OptionCss: "edit-field col_7",
                TextboxCss: "edit-field col_7"
            },
            firstDatePicker: {
                Id: "FromDate",
                Label: labelFrom,
                InitDate: today,
                LabelCss: "lbl-field col_5",
                TextboxCss: "edit-field col_5"
            },
            secondDatePicker: {
                Id: "ToDate",
                Label: labelTo,
                InitDate: today,
                LabelCss: "lbl-field col_2",
                TextboxCss: "edit-field col_7"
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
                                    window.rootUrl + "Licensee/AGCasinoWinLoss/Licensee",
                                    null,
                                    self.renderLicensee,
                                    AccReport.enableInput,
                                    AccReport.enableInput);
    };

    self.renderLicensee = function (licenseeList) {
        var options = '<option value=0>Please select one!</option>';
        for (var i = 0; i < licenseeList.length; i++) {
            options += '<option value=' + licenseeList[i].LicenseeID + ' launch-date="' + licenseeList[i].LaunchDate + '">' + licenseeList[i].LicenseeName + '</option>';
        }

        _tables.dropdownLicensee.find('option').remove().end().append(options);
        _tables.dropdownLicensee.change(function () {
            minDate = $(this).find("option:selected").attr("launch-date");
            _tables.datepickerplugin.nextAll().remove();
            self.renderDatePicker();
            self.getCurrencyByLicensee();
        });
    };

    self.getCurrencyByLicensee = function () {
        AccReport.disableInput();
        var param = {};
        param.LicenseeID = _tables.dropdownLicensee.val();
        param.ExcludeTest = _tables.checkExcludeTest.prop('checked');
        AccReport.utility.CallAjax(this,
                                    window.rootUrl + "Licensee/AGCasinoWinLoss/CurrencyByLicensee",
                                    param,
                                    self.renderCurrency,
                                    self.renderCurrency,
                                    AccReport.enableInput);
    };

    self.renderCurrency = function (currencyList) {
        var options = '<option value=0>All Currencies</option>';
        for (var i = 0; i < currencyList.length; i++) {
            options += '<option value=' + currencyList[i].Value + '>' + currencyList[i].Text + '</option>';
        }

        _tables.dropdownCurrency.find('option').remove().end().append(options);
    };

    self.bindingEvents = function () {
        _tables.btnGenerate.click(function () {
            _tables.hidIsRefresh.val(1);
            _tables.hidPageSize.val(50);
            _tables.hidPageIndex.val(0);
            _tables.hidReportTypeEnum.val(self.reportTypeEnum.MainLevel);
            self.getMainReportParams();
            self.generateMainReport();
        });
    };

    self.validateParams = function () {
        var isValid = true;
        var message = '';
        var notifyIds = [];
        if (self.params.reportMainParams.LicenseeID === "0") {
            isValid = false;
            if (message === '') {
                message = "Please select one licensee!";
            }
            notifyIds.push("LicenseeID");
        }

        if (!isValid) {
            AccReport.popUp.removeHighLight();
            AccReport.popUp.show(message, notifyIds.join(","));
        } else {
            AccReport.popUp.removeHighLight();
        }

        return isValid;
    };

    self.getMainReportParams = function () {
        self.params.reportMainParams.LicenseeID = _tables.dropdownLicensee.val();
        self.params.reportMainParams.LicenseeName = _tables.dropdownLicensee.find('option:selected').text();
        self.params.reportMainParams.Currency = _tables.dropdownCurrency.val();
        self.params.reportMainParams.CurrencyName = _tables.dropdownCurrency.find('option:selected').text();
        self.params.reportMainParams.ReportType = $('input[name=ReportType]:checked').val();
        self.params.reportMainParams.ConvertFiguresTo = $('input[name=ConvertFiguresTo]:checked').val();
        self.params.reportMainParams.ExcludeTest = _tables.checkExcludeTest.prop('checked');
        var dateRange = _tables.datepickerplugin.nexDatePicker("getDateRange");
        self.params.reportMainParams.FromDate = dateRange.FromDate;
        self.params.reportMainParams.ToDate = dateRange.ToDate;
        self.params.reportMainParams.PageSize = _tables.hidPageSize.val();
        self.params.reportMainParams.PageIndex = _tables.hidPageIndex.val();
        self.params.reportMainParams.IsRefresh = _tables.hidIsRefresh.val();
    };

    self.getSuperReportParams = function () {
        self.params.reportSuperParams.LicenseeID = self.params.reportMainParams.LicenseeID;
        self.params.reportSuperParams.LicenseeName = self.params.reportMainParams.LicenseeName;
        self.params.reportSuperParams.Currency = _tables.hidCurrency.val();
        self.params.reportSuperParams.CurrencyName = _tables.dropdownCurrency.find("option[value=" + _tables.hidCurrency.val() + "]").text();
        self.params.reportSuperParams.CustID = _tables.hidCustID.val();
        self.params.reportSuperParams.ReportType = self.params.reportMainParams.ReportType;
        self.params.reportSuperParams.ConvertFiguresTo = self.params.reportMainParams.ConvertFiguresTo;
        self.params.reportSuperParams.CustID = _tables.hidCustID.val();
        self.params.reportSuperParams.ExcludeTest = self.params.reportMainParams.ExcludeTest;
        self.params.reportSuperParams.FromDate = moment(_tables.hidFromDate.val()).format('MMM DD, YYYY');
        self.params.reportSuperParams.ToDate = moment(_tables.hidToDate.val()).format('MMM DD, YYYY');
        self.params.reportSuperParams.PageSize = _tables.hidPageSize.val();
        self.params.reportSuperParams.PageIndex = _tables.hidPageIndex.val();
        self.params.reportSuperParams.IsRefresh = _tables.hidIsRefresh.val();
    };

    self.getMasterReportParams = function () {
        self.params.reportMasterParams.LicenseeID = self.params.reportMainParams.LicenseeID;
        self.params.reportMasterParams.LicenseeName = self.params.reportMainParams.LicenseeName;
        self.params.reportMasterParams.Currency = self.params.reportSuperParams.Currency;
        self.params.reportMasterParams.CurrencyName = self.params.reportSuperParams.CurrencyName;
        self.params.reportMasterParams.CustID = _tables.hidCustID.val();
        self.params.reportMasterParams.CustomerName = _tables.hidCustomerName.val();
        self.params.reportMasterParams.ReportType = self.params.reportMainParams.ReportType;
        self.params.reportMasterParams.ConvertFiguresTo = self.params.reportMainParams.ConvertFiguresTo;
        self.params.reportMasterParams.CustID = _tables.hidCustID.val();
        self.params.reportMasterParams.CustomerName = _tables.hidCustomerName.val();
        self.params.reportMasterParams.ExcludeTest = self.params.reportMainParams.ExcludeTest;
        self.params.reportMasterParams.FromDate = self.params.reportSuperParams.FromDate;
        self.params.reportMasterParams.ToDate = self.params.reportSuperParams.ToDate;
        self.params.reportMasterParams.PageSize = _tables.hidPageSize.val();
        self.params.reportMasterParams.PageIndex = _tables.hidPageIndex.val();
        self.params.reportMasterParams.IsRefresh = _tables.hidIsRefresh.val();
    };

    self.getAgentReportParams = function () {
        self.params.reportAgentParams.LicenseeID = self.params.reportMainParams.LicenseeID;
        self.params.reportAgentParams.LicenseeName = self.params.reportMainParams.LicenseeName;
        self.params.reportAgentParams.Currency = self.params.reportSuperParams.Currency;
        self.params.reportAgentParams.CurrencyName = self.params.reportSuperParams.CurrencyName;
        self.params.reportAgentParams.CustID = _tables.hidCustID.val();
        self.params.reportAgentParams.ReportType = self.params.reportMainParams.ReportType;
        self.params.reportAgentParams.ConvertFiguresTo = self.params.reportMainParams.ConvertFiguresTo;
        self.params.reportAgentParams.CustID = _tables.hidCustID.val();
        self.params.reportAgentParams.CustomerName = _tables.hidCustomerName.val();
        self.params.reportAgentParams.ExcludeTest = self.params.reportMainParams.ExcludeTest;
        self.params.reportAgentParams.FromDate = self.params.reportSuperParams.FromDate;
        self.params.reportAgentParams.ToDate = self.params.reportSuperParams.ToDate;
        self.params.reportAgentParams.PageSize = _tables.hidPageSize.val();
        self.params.reportAgentParams.PageIndex = _tables.hidPageIndex.val();
        self.params.reportAgentParams.IsRefresh = _tables.hidIsRefresh.val();
    };

    self.getMemberReportParams = function () {
        self.params.reportMemberParams.LicenseeID = self.params.reportMainParams.LicenseeID;
        self.params.reportMemberParams.LicenseeName = self.params.reportMainParams.LicenseeName;
        self.params.reportMemberParams.Currency = self.params.reportSuperParams.Currency;
        self.params.reportMemberParams.CurrencyName = self.params.reportSuperParams.CurrencyName;
        self.params.reportMemberParams.CustID = _tables.hidCustID.val();
        self.params.reportMemberParams.ReportType = self.params.reportMainParams.ReportType;
        self.params.reportMemberParams.ConvertFiguresTo = self.params.reportMainParams.ConvertFiguresTo;
        self.params.reportMemberParams.CustID = _tables.hidCustID.val();
        self.params.reportMemberParams.CustomerName = _tables.hidCustomerName.val();
        self.params.reportMemberParams.ExcludeTest = self.params.reportMainParams.ExcludeTest;
        self.params.reportMemberParams.FromDate = self.params.reportSuperParams.FromDate;
        self.params.reportMemberParams.ToDate = self.params.reportSuperParams.ToDate;
        self.params.reportMemberParams.PageSize = _tables.hidPageSize.val();
        self.params.reportMemberParams.PageIndex = _tables.hidPageIndex.val();
        self.params.reportMemberParams.IsRefresh = _tables.hidIsRefresh.val();
    };

    self.exportExcel = function () {
        switch (self.currentLevel) {
            case self.reportTypeEnum.SuperLevel:
                AccReport.downloadFile(window.rootUrl + "Licensee/AGCasinoWinLoss/ExportExcelSuperReport", self.params.reportSuperParams);
                break;
            case self.reportTypeEnum.MasterLevel:
                AccReport.downloadFile(window.rootUrl + "Licensee/AGCasinoWinLoss/ExportExcelMasterReport", self.params.reportMasterParams);
                break;
            case self.reportTypeEnum.AgentLevel:
                AccReport.downloadFile(window.rootUrl + "Licensee/AGCasinoWinLoss/ExportExcelAgentReport", self.params.reportAgentParams);
                break;
            case self.reportTypeEnum.MemberLevel:
                AccReport.downloadFile(window.rootUrl + "Licensee/AGCasinoWinLoss/ExportExcelMemberReport", self.params.reportMemberParams);
                break;
            default:
                AccReport.downloadFile(window.rootUrl + "Licensee/AGCasinoWinLoss/ExportExcelMainReport", self.params.reportMainParams);
                break;
        }
    };

    self.generateMainReport = function () {
        self.currentLevel = self.reportTypeEnum.MainLevel;
        AccReport.disableInput();
        if (self.validateParams()) {
            AccReport.utility.CallAjax(this,
                window.rootUrl + "Licensee/AGCasinoWinLoss/GenerateMainReport",
                self.params.reportMainParams,
                self.renderGrid,
                AccReport.enableInput,
                AccReport.enableInput);
        }
        else {
            AccReport.enableInput();
        }
    };

    self.onSuperClick = function (date, custID, currencyID) {
        _tables.hidPageSize.val(50);
        _tables.hidPageIndex.val(0);
        _tables.hidIsRefresh.val(1);
        self.currentLevel = self.reportTypeEnum.SuperLevel;
        _tables.hidCustID.val(custID);
        _tables.hidCurrency.val(currencyID);
        var datePart = date.split('/');
        var fromDate, toDate;
        if (self.params.reportMainParams.ReportType === "1") {
            fromDate = new Date(datePart[2], datePart[0] - 1, datePart[1]);
            toDate = new Date(datePart[2], datePart[0] - 1, datePart[1]);
        }
        else {
            fromDate = new Date(datePart[1], datePart[0] - 1, 1);
            toDate = new Date(datePart[1], datePart[0], 0);
        }

        _tables.hidFromDate.val(fromDate);
        _tables.hidToDate.val(toDate);
        self.getSuperReportParams();
        self.generateSuperReport();
    };

    self.generateSuperReport = function () {
        AccReport.disableInput();
        AccReport.utility.CallAjax(this,
            window.rootUrl + "Licensee/AGCasinoWinLoss/GenerateSuperReport",
            self.params.reportSuperParams,
            self.renderGrid,
            AccReport.enableInput,
            AccReport.enableInput);
    };

    self.onMasterClick = function (custID, customerName) {
        self.currentLevel = self.reportTypeEnum.MasterLevel;
        _tables.hidPageSize.val(50);
        _tables.hidPageIndex.val(0);
        _tables.hidIsRefresh.val(1);
        _tables.hidCustID.val(custID);
        _tables.hidCustomerName.val(customerName);
        self.getMasterReportParams();
        self.generateMasterReport();
    };

    self.generateMasterReport = function () {
        AccReport.disableInput();
        AccReport.utility.CallAjax(this,
            window.rootUrl + "Licensee/AGCasinoWinLoss/GenerateMasterReport",
            self.params.reportMasterParams,
            self.renderGrid,
            AccReport.enableInput,
            AccReport.enableInput);
    };

    self.onAgentClick = function (custID, customerName) {
        self.currentLevel = self.reportTypeEnum.AgentLevel;
        _tables.hidPageSize.val(50);
        _tables.hidPageIndex.val(0);
        _tables.hidIsRefresh.val(1);
        _tables.hidCustID.val(custID);
        _tables.hidCustomerName.val(customerName);
        self.getAgentReportParams();
        self.generateAgentReport();
    };

    self.generateAgentReport = function () {
        AccReport.disableInput();
        AccReport.utility.CallAjax(this,
            window.rootUrl + "Licensee/AGCasinoWinLoss/GenerateAgentReport",
            self.params.reportAgentParams,
            self.renderGrid,
            AccReport.enableInput,
            AccReport.enableInput);
    };

    self.onMemberClick = function (custID, customerName) {
        self.currentLevel = self.reportTypeEnum.MemberLevel;
        _tables.hidPageSize.val(50);
        _tables.hidPageIndex.val(0);
        _tables.hidIsRefresh.val(1);
        _tables.hidCustID.val(custID);
        _tables.hidCustomerName.val(customerName);
        self.getMemberReportParams();
        self.generateMemberReport();
    };

    self.generateMemberReport = function () {
        AccReport.disableInput();
        AccReport.utility.CallAjax(this,
            window.rootUrl + "Licensee/AGCasinoWinLoss/GenerateMemberReport",
            self.params.reportMemberParams,
            self.renderGrid,
            AccReport.enableInput,
            AccReport.enableInput);
    };

    self.renderGrid = function (data) {
        var isMainLevel = self.currentLevel === self.reportTypeEnum.MainLevel;
        var title = self.getReportTitle();
        var reportData = [];
        var footerData = [];
        if (data.Data.length > 0) {
            footerData = self.refineFooterData(data);
            reportData = self.refineBodyData(data);
        }

        var gridOptions =
            {
                columns: [
                    {
                        HeaderText: isMainLevel ? "Date" : "UserName", Width: 150, Name: isMainLevel ? "Date" : "UserName",
                        HeaderAlign: "center", CellAlign: "center"
                    },
                    { HeaderText: "Currency", Width: 150, Name: "Currency", HeaderAlign: "center", CellAlign: "center" },
                    {
                        HeaderText: "Turnover", Width: 150, Name: "Turnover",
                        HeaderAlign: "center", CellAlign: "right", FormatNumber: FormatConstant.TWO_DECIMAL_NUMBER
                    },
                    {
                        HeaderText: "Total Transactions",
                        Width: 150,
                        Name: "TotalTransactions",
                        HeaderAlign: "center",
                        CellAlign: "right",
                        FormatNumber: FormatConstant.INT_NUMBER
                    },
                    {
                        HeaderText: "Customer Win Loss",
                        Width: 150,
                        Name: "CustomerWinLoss",
                        HeaderAlign: "center",
                        CellAlign: "right",
                        FormatNumber: FormatConstant.TWO_DECIMAL_NUMBER
                    },
                    {
                        HeaderText: "Licensee Win Loss",
                        Width: 150,
                        Name: "LicenseeWinLoss",
                        HeaderAlign: "center",
                        CellAlign: "right",
                        FormatNumber: FormatConstant.TWO_DECIMAL_NUMBER
                    },
                    {
                        HeaderText: "Amount Due to AGC",
                        Width: 150,
                        Name: "AmountDueToAsiaGamings",
                        HeaderAlign: "center",
                        CellAlign: "right",
                        FormatNumber: FormatConstant.TWO_DECIMAL_NUMBER
                    }
                ],
                bodyRows: reportData,
                footerRows: footerData,
                showNumberColumn: false,
                gridExpandHeight: 330,
                showTitle: true,
                showFooter: data.length !== 0,
                showDistinctRow: false,
                titleInfo: {
                    text: title,
                    showExcelButton: true,
                    exportExcelFunction: function () {
                        self.exportExcel();
                    },
                    showBackButton: self.currentLevel !== self.reportTypeEnum.MainLevel,
                    backFunction: function () {
                        self.currentLevel--;
                        self.showHideGrid();
                    }
                },
                noDataText: "No information is available",
                showPager: true,
                customPager: true,
                pagerOption: {
                    itemsPerPage: parseInt(_tables.hidPageSize.val()),
                    currentPage: parseInt(_tables.hidPageIndex.val()),
                    totalItem: parseInt(data.TotalItem),
                    itemsPerPageArray: [50, 100, 200, 500],
                    pagerSelectCallBack: self.reRenderAfterPaged
                }
            };

        switch (self.currentLevel) {
            case self.reportTypeEnum.MainLevel:
                self.hideAllGrid();
                _tables.reportMainGrid.removeClass("display-none").fanexGrid(gridOptions);
                break;
            case self.reportTypeEnum.SuperLevel:
                self.hideAllGrid();
                _tables.reportSuperGrid.removeClass("display-none").fanexGrid(gridOptions);
                break;
            case self.reportTypeEnum.MasterLevel:
                self.hideAllGrid();
                _tables.reportMasterGrid.removeClass("display-none").fanexGrid(gridOptions);
                break;
            case self.reportTypeEnum.AgentLevel:
                self.hideAllGrid();
                _tables.reportAgentGrid.removeClass("display-none").fanexGrid(gridOptions);
                break;
            case self.reportTypeEnum.MemberLevel:
                self.hideAllGrid();
                _tables.reportMemberGrid.removeClass("display-none").fanexGrid(gridOptions);
                break;
            default:
                _tables.reportMainGrid.removeClass("display-none").fanexGrid(gridOptions);
                break;
        }
    };

    self.refineFooterData = function (data) {
        var isMainLevel = self.currentLevel === self.reportTypeEnum.MainLevel;
        var footerData = [];
        var i = 0;
        for (i = 0; i < data.GrandTotal.length; i++) {
            var grandTotalRow = [];
            if (i === 0) {
                grandTotalRow.push({
                    Name: isMainLevel ? "Date" : "UserName",
                    Value: isMainLevel ? data.GrandTotal[i].Date : data.GrandTotal[i].UserName,
                    MergeRowNumber: 4
                });
            }

            grandTotalRow.push({ Name: "Currency", Value: data.GrandTotal[i].CurrencyName });
            grandTotalRow.push({ Name: "Turnover", Value: data.GrandTotal[i].Turnover });
            if (i === 0) {
                grandTotalRow.push({ Name: "TotalTransactions", Value: data.GrandTotal[i].TotalTrans, MergeRowNumber: 4 });
            }

            grandTotalRow.push({ Name: "CustomerWinLoss", Value: data.GrandTotal[i].CustomerWinLoss });
            grandTotalRow.push({ Name: "LicenseeWinLoss", Value: data.GrandTotal[i].LicenseeWinLoss });
            grandTotalRow.push({ Name: "AmountDueToAsiaGamings", Value: data.GrandTotal[i].AmountDueToAsiaGamings });
            footerData.push({ Columns: grandTotalRow });
        }

        return footerData;
    };

    self.refineBodyData = function (data) {
        var isMainLevel = self.currentLevel === self.reportTypeEnum.MainLevel;
        var reportData = [];
        var previousRow = data.Data[0];
        var mergeRowNumber = 0;
        var firstRow = 0;
        var oddEven = false;
        var i = 0;
        var row, value;
        for (i = 0; i < data.Data.length; i++) {
            row = [];
            value = data.Data[i].UserName;
            if (self.currentLevel !== self.reportTypeEnum.MemberLevel) {
                value = '<div class="link" onclick="javascript:aGCasinoWinloss.'
                         + self.getCurrentLevelRenderMethod()
                         + '(' + data.Data[i].CustID.toString()
                         + ', \'' + data.Data[i].UserName
                         + '\')" >' + data.Data[i].UserName + '</div>';
            }

            if (i === 0) {
                row.push({
                    Name: isMainLevel ? "Date" : "UserName",
                    Value: isMainLevel ? data.Data[i].Date : value
                });
            }

            if (isMainLevel) {
                value = '<div class="link" onclick="javascript:aGCasinoWinloss.onSuperClick(\''
                        + data.Data[i].Date
                        + '\' ,'
                        + data.Data[i].CustID.toString()
                        + ', ' + data.Data[i].CurrencyID.toString()
                        + ')" >' + data.Data[i].CurrencyName + '</div>';
                row.push({ Name: "Currency", Value: value });
            }
            else {
                row.push({ Name: "Currency", Value: data.Data[i].CurrencyName });
            }

            row.push({ Name: "Turnover", Value: data.Data[i].Turnover });
            row.push({ Name: "TotalTransactions", Value: data.Data[i].TotalTrans });
            row.push({ Name: "CustomerWinLoss", Value: data.Data[i].CustomerWinLoss });
            row.push({ Name: "LicenseeWinLoss", Value: data.Data[i].LicenseeWinLoss });
            row.push({ Name: "AmountDueToAsiaGamings", Value: data.Data[i].AmountDueToAsiaGamings });

            if ((isMainLevel && previousRow.Date !== data.Data[i].Date) ||
                    (!isMainLevel && previousRow.UserName !== data.Data[i].UserName)) {
                if (mergeRowNumber > 1) {
                    reportData[firstRow].Columns[0].MergeRowNumber = mergeRowNumber;
                }

                mergeRowNumber = 1;
                firstRow = i;
                row.unshift({
                    Name: isMainLevel ? "Date" : "UserName",
                    Value: isMainLevel ? data.Data[i].Date : value
                });
                oddEven = !oddEven;
            }
            else {
                mergeRowNumber++;
            }

            previousRow = data.Data[i];
            reportData.push({ Columns: row, CssClass: oddEven ? 'freegrid-even' : '' });
        }

        if (mergeRowNumber > 1) {
            reportData[firstRow].Columns[0].MergeRowNumber = mergeRowNumber;
        }

        for (i = 0; i < data.Total.length; i++) {
            row = [];
            if (i === 0) {
                row.push({
                    Name: isMainLevel ? "Date" : "UserName",
                    Value: isMainLevel ? data.Total[i].Date : data.Total[i].UserName,
                    MergeRowNumber: data.Total.length
                });
            }

            row.push({ Name: "Currency", Value: data.Total[i].CurrencyName });
            row.push({ Name: "Turnover", Value: data.Total[i].Turnover });
            row.push({ Name: "TotalTransactions", Value: data.Total[i].TotalTrans });
            row.push({ Name: "CustomerWinLoss", Value: data.Total[i].CustomerWinLoss });
            row.push({ Name: "LicenseeWinLoss", Value: data.Total[i].LicenseeWinLoss });
            row.push({ Name: "AmountDueToAsiaGamings", Value: data.Total[i].AmountDueToAsiaGamings });
            reportData.push({ Columns: row, CssClass: 'totalRow' });
        }

        return reportData;
    };

    self.reRenderAfterPaged = function (pageIndex, itemsPerPage) {
        _tables.hidIsRefresh.val(0);
        _tables.hidPageSize.val(itemsPerPage);
        _tables.hidPageIndex.val(pageIndex);
        switch (self.currentLevel) {
            case self.reportTypeEnum.SuperLevel:
                self.params.reportSuperParams.PageSize = _tables.hidPageSize.val();
                self.params.reportSuperParams.PageIndex = _tables.hidPageIndex.val();
                self.params.reportSuperParams.IsRefresh = _tables.hidIsRefresh.val();
                self.generateSuperReport();
                break;
            case self.reportTypeEnum.MasterLevel:
                self.params.reportMasterParams.PageSize = _tables.hidPageSize.val();
                self.params.reportMasterParams.PageIndex = _tables.hidPageIndex.val();
                self.params.reportMasterParams.IsRefresh = _tables.hidIsRefresh.val();
                self.generateMasterReport();
                break;
            case self.reportTypeEnum.AgentLevel:
                self.params.reportAgentParams.PageSize = _tables.hidPageSize.val();
                self.params.reportAgentParams.PageIndex = _tables.hidPageIndex.val();
                self.params.reportAgentParams.IsRefresh = _tables.hidIsRefresh.val();
                self.generateAgentReport();
                break;
            case self.reportTypeEnum.MemberLevel:
                self.params.reportMemberParams.PageSize = _tables.hidPageSize.val();
                self.params.reportMemberParams.PageIndex = _tables.hidPageIndex.val();
                self.params.reportMemberParams.IsRefresh = _tables.hidIsRefresh.val();
                self.generateMemberReport();
                break;
            default:
                self.params.reportMainParams.PageSize = _tables.hidPageSize.val();
                self.params.reportMainParams.PageIndex = _tables.hidPageIndex.val();
                self.params.reportMainParams.IsRefresh = _tables.hidIsRefresh.val();
                self.generateMainReport();
                break;
        }

        return false;
    };

    self.getReportTitle = function () {
        var result = reportTitle;
        switch (self.currentLevel) {
            case self.reportTypeEnum.SuperLevel:
                result = result.replace('{0}', self.params.reportSuperParams.LicenseeName);
                result = result.replace('{1}', self.params.reportSuperParams.FromDate);
                result = result.replace('{2}', self.params.reportSuperParams.ToDate);
                result = result.replace('{3}', self.params.reportSuperParams.CurrencyName);
                break;
            case self.reportTypeEnum.MasterLevel:
                result = result.replace('{0}', self.params.reportMasterParams.CustomerName);
                result = result.replace('{1}', self.params.reportMasterParams.FromDate);
                result = result.replace('{2}', self.params.reportMasterParams.ToDate);
                result = result.replace('{3}', self.params.reportMasterParams.CurrencyName);
                break;
            case self.reportTypeEnum.AgentLevel:
                result = result.replace('{0}', self.params.reportAgentParams.CustomerName);
                result = result.replace('{1}', self.params.reportAgentParams.FromDate);
                result = result.replace('{2}', self.params.reportAgentParams.ToDate);
                result = result.replace('{3}', self.params.reportAgentParams.CurrencyName);
                break;
            case self.reportTypeEnum.MemberLevel:
                result = result.replace('{0}', self.params.reportMemberParams.CustomerName);
                result = result.replace('{1}', self.params.reportMemberParams.FromDate);
                result = result.replace('{2}', self.params.reportMemberParams.ToDate);
                result = result.replace('{3}', self.params.reportMemberParams.CurrencyName);
                break;
            default:
                result = result.replace('{0}', self.params.reportMainParams.LicenseeName);
                result = result.replace('{1}', self.params.reportMainParams.FromDate);
                result = result.replace('{2}', self.params.reportMainParams.ToDate);
                result = result.replace('{3}', self.params.reportMainParams.CurrencyName);
                break;
        }

        return result;
    };

    self.showHideGrid = function () {
        switch (self.currentLevel) {
            case self.reportTypeEnum.MainLevel:
                self.hideAllGrid();
                _tables.reportMainContainer.removeClass("display-none");
                _tables.reportMainGrid.fanexGrid("resizeGrid");
                break;
            case self.reportTypeEnum.SuperLevel:
                self.hideAllGrid();
                _tables.reportSuperContainer.removeClass("display-none");
                _tables.reportSuperGrid.fanexGrid("resizeGrid");
                break;
            case self.reportTypeEnum.MasterLevel:
                self.hideAllGrid();
                _tables.reportMasterContainer.removeClass("display-none");
                _tables.reportMasterGrid.fanexGrid("resizeGrid");
                break;
            case self.reportTypeEnum.AgentLevel:
                self.hideAllGrid();
                _tables.reportAgentContainer.removeClass("display-none");
                _tables.reportAgentGrid.fanexGrid("resizeGrid");
                break;
            case self.reportTypeEnum.MemberLevel:
                self.hideAllGrid();
                _tables.reportMemberContainer.removeClass("display-none");
                _tables.reportMemberGrid.fanexGrid("resizeGrid");
                break;
            default:
                _tables.reportMainContainer.removeClass("display-none");
                _tables.reportMainGrid.fanexGrid("resizeGrid");
                break;
        }
    };

    self.hideAllGrid = function () {
        _tables.reportMainContainer.addClass('display-none');
        _tables.reportSuperContainer.addClass('display-none');
        _tables.reportMasterContainer.addClass('display-none');
        _tables.reportAgentContainer.addClass('display-none');
        _tables.reportMemberContainer.addClass('display-none');
    };

    self.getCurrentLevelRenderMethod = function () {
        switch (self.currentLevel + 1) {
            case self.reportTypeEnum.SuperLevel:
                return "onSuperClick";
            case self.reportTypeEnum.MasterLevel:
                return "onMasterClick";
            case self.reportTypeEnum.AgentLevel:
                return "onAgentClick";
            case self.reportTypeEnum.MemberLevel:
                return "onMemberClick";
            default:
                return "generateMainReport";
        }
    };
};

$(document).ready(function () {
    aGCasinoWinloss = new Fanex.AccountingReports.Licensee.AGCasinoWinloss();
    aGCasinoWinloss.initPage();
});