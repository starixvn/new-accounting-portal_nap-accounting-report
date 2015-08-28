$(document).ready(function () {
    reportManager.InitPage();
});

Namespace.Register("Fanex.AccountingReports.Gaming.LiveCasinoAgencyWinLoss");
Fanex.AccountingReports.Gaming.LiveCasinoAgencyWinLoss.ReportManager = function () {
    var self = this;

    self.Enums = {
        DateOption: {
            Last3Months: 8,
            Last6Months: 9,
            Today: 0,
            Yesterday: 1
        },

        LicenseeId: {
            All: 0,
            IBC: -1
        },

        CurrencyId: {
            All: 0,
            BaseRM: -1
        },

        ReportType: {
            Daily: 0,
            Monthly: 1,
            UserName: 2,
            GameType: 3,
            GameTable: 4,
            Agent: 5
        },

        SortDirection: {
            Asc: "ASC",
            Desc: "DESC"
        },

        SortDataType: {
            Number: "number",
            String: "string",
            Date: "date"
        },

        Product: {
            All: 0,
            LiveCasino: 4,
            GoldDeluxe: 12,
            MicroGaming: 13,
            AGCasino: 11
        },

        GoldDeluxeBettypeRange: {
            MinBettypeValue: 1701,
            MaxBettypeValue: 1703
        },

        SLIBettypeRange: {
            MinBettypeValue: 1101,
            MaxBettypeValue: 1199
        }
    };

    self.ReportByDailyRange = new Fanex.AccountingReports.Gaming.LiveCasinoAgencyWinLoss.ReportByDailyRange();
    self.ReportByMonthlyRange = new Fanex.AccountingReports.Gaming.LiveCasinoAgencyWinLoss.ReportByMonthlyRange();
    self.ReportByUserName = new Fanex.AccountingReports.Gaming.LiveCasinoAgencyWinLoss.ReportByUserName();
    self.ReportByGameType = new Fanex.AccountingReports.Gaming.LiveCasinoAgencyWinLoss.ReportByGameType();
    self.ReportByGameTable = new Fanex.AccountingReports.Gaming.LiveCasinoAgencyWinLoss.ReportByGameTable();
    self.ReportByAgent = new Fanex.AccountingReports.Gaming.LiveCasinoAgencyWinLoss.ReportByAgent();

    self.Tables = {
        datepickerplugin: null,
        dateOptionSelector: null,
        firstDatePickerId1: null,
        secondDatePickerId1: null,
        last3MonthsDatePicker: null,
        last6MonthsDatePicker: null,

        dropdownCurrency: null,
        dropdownCompany: null,
        dropdownGameType: null,
        dropdownGameTable: null,
        excludeTestData: null,

        showHideFilter: null,
        filterBar: null,
        txtUsername: null,

        btnGenerate: null,
        btnToday: null,
        btnYesterday: null,
        btnExport: null,

        mainLiveCasinoAgencyWinLossReportContainer: null,
        mainLiveCasinoAgencyWinLossReport: null,

        autocomplete: null,

        dropdownProduct: null,
        gameTableContainer: null,
        rdGameTable: null,
        rdDaily: null,
        rdGameTableContainer: null
    };

    self.CachedData = {
        reportMonthRange: 3,
        defaultTextboxUserName: viewResources.Search_All,
        currentTextboxUserName: viewResources.Search_All,
        isDefaultTextboxUserName: true,
        isOnAuto: false,
        gridOptions: null,
        currentPage: 0,
        itemsPerPage: 50,
        defaultItemsPerPage: 50,
        itemsPerPageArray: [50, 100, 200, 500],
        currentReport: null,
        maxGridHeight: 10000,

        last3Months: 3,
        last6Months: 6,

        isLoadedProductList: false,
        isLoadedCompanyList: false,
        isLoadedGameTypeList: false,
        isLoadedGameTableList: false,
        isLoadedCurrencyList: false,

        sortColName: null,
        sortDirection: null,
        sortDataType: null,
        reportType: null,

        RoleIndex: {
            Member: 0,
            Agent: 1,
            Master: 2,
            Super: 3,
            Company: 4
        },

        isSelectOtherSLIProducts: false,
        productId: 0
    };

    self.Params = {
        getGameTableParams: {
            GameTypeId: null,
            ExcludeTest: null
        },

        reportParams: {
            FromDate: null,
            ToDate: null,
            UserName: null,
            ExcludeTestData: null,
            LicenseeId: null,
            GameTypeId: null,
            GameTableId: null,
            Currency: null,
            ReportType: null,
            IsChangedValue: false,
            SortDirection: null,
            SortColumnName: null,
            SortDataType: null,
            ProductId: null
        }
    };

    self.InitPage = function () {
        AccReport.disableInput();
        self.RenderDatePicker();
        self.CachedDOMElements();
        self.MakeUserNameSuggestion();
        self.LoadFilterValues();
        self.BindingEvents();
        self.Tables.btnToday.val(today);
        self.Tables.btnYesterday.val(yesterday);

        self.ReportByAgent.InitPage();
    };

    self.RenderDatePicker = function () {
        $('#datepickerplugin').nexDatePicker({
            nexDatePickerType: "quickchoose",
            datePickerFormat: {
                changeMonth: true,
                changeYear: true,
                dateFormat: FormatConstant.M_DD_YY_STRING
            },
            dropDownList:
            {
                Id: "dateOptionSelector",
                Label: viewResources.Time_period,
                LabelCss: "lbl-field col_5 align-left",
                OptionCss: "edit-field col_6"
            },
            firstDatePicker: {
                Id: "firstDatePickerId1",
                Label: viewResources.From_Date,
                InitDate: today,
                LabelCss: "lbl-field col_5 align-left",
                TextboxCss: "edit-field col_6"
            },
            secondDatePicker: {
                Id: "secondDatePickerId1",
                Label: viewResources.To_Date,
                InitDate: today,
                LabelCss: "lbl-field col_3 align-left",
                TextboxCss: "edit-field col_6"
            },
            selectedItem: 0,
            onSelectDatePicker: function () {
            },
            minDate: minDate,
            maxDate: new Date(today)
        });

        this.Tables.datepickerplugin = $("#datepickerplugin");
        this.Tables.firstDatePickerId1 = $("#firstDatePickerId1");
        this.Tables.secondDatePickerId1 = $("#secondDatePickerId1");
        this.Tables.dateOptionSelector = $("#dateOptionSelector");

        var dateOption = '<option value="' + this.Enums.DateOption.Last3Months +
                        '" data-fDate="' + beginLast3Months + '" data-tDate="' + endLast3Months + '">' +
        AccReport.utility.StringFormat(viewResources.Options_LastMonthRange, [reportManager.CachedData.last3Months]) + '</option>';
        this.Tables.dateOptionSelector.append(dateOption);

        dateOption = '<option value="' + this.Enums.DateOption.Last6Months +
                        '" data-fDate="' + beginLast6Months + '" data-tDate="' + endLast6Months + '">' +
        AccReport.utility.StringFormat(viewResources.Options_LastMonthRange, [reportManager.CachedData.last6Months]) + '</option>';
        this.Tables.dateOptionSelector.append(dateOption);

        this.Tables.last3MonthsDatePicker = $("#last3MonthsDatePicker");
        this.Tables.last6MonthsDatePicker = $("#last6MonthsDatePicker");
    };

    self.CachedDOMElements = function () {
        self.Tables.showHideFilter = $(".showHideFilter");
        self.Tables.filterBar = $("#filter-bar");
        self.Tables.txtUsername = $("#txt-username");
        self.Tables.excludeTestData = $("#exclude-test-data");

        self.Tables.btnGenerate = $("#button-generate");
        self.Tables.btnToday = $("#button-today");
        self.Tables.btnYesterday = $("#button-yesterday");
        self.Tables.btnExport = $("#button-export");
        self.Tables.reportBy = $("#reportBy");

        self.Tables.mainLiveCasinoAgencyWinLossReportContainer = $("#mainLiveCasinoAgencyWinLossReportContainer");
        self.Tables.mainLiveCasinoAgencyWinLossReport = $("#mainLiveCasinoAgencyWinLossReport");
        self.Tables.dropdownProduct = $("#ddl-product");
        self.Tables.gameTableContainer = $("#game-table-container");
        self.Tables.rdGameTable = $("#radio-reportbygametable");
        self.Tables.rdDaily = $("#radio-reportbydaily");
        self.Tables.rdGameTableContainer = $("#rdGameTableContainer");
    };

    self.MakeUserNameSuggestion = function () {
        var self = this;
        self.CachedData.autoCompleteManager = self.Tables.txtUsername.autocomplete({
            serviceUrl: window.rootWebPath.replace(/\/$/g, '') + '/ReportHelper/UserNameSuggestion?' +
            $.param(self.GetUserNameSuggestionParams()),
            minChars: 2
        });

        self.Tables.autocomplete = $('.autocomplete');
    };

    self.GetUserNameSuggestionParams = function () {
        return {
            excludeTestAccounts: self.Tables.excludeTestData.prop("checked") ? true : false
        };
    };

    self.LoadFilterValues = function () {
        self.GetProductList();
        self.GetCompanyList();
        self.GetGameTypeList();
        self.GetCurrencyList();
    };

    self.GetProductList = function () {
        AccReport.utility.CallAjax(this,
            window.rootUrl + "Gaming/LiveCasinoAgencyWinLoss/GetProductList",
            null,
            self.RenderProductList,
            self.RenderProductList,
            self.CompletedLoadFilter,
            false);
    };

    self.RenderProductList = function (rs) {
        var productLength = rs.length;
        self.Tables.dropdownProduct.empty();
        for (var i = 0; i < productLength; i++) {
            var product = rs[i];
            var option = '<option value="' + product.Id + '">' + product.Name + '</option>';
            self.Tables.dropdownProduct.append(option);
        }

        self.CachedData.isLoadedProductList = true;
    };

    self.GetCompanyList = function () {
        AccReport.utility.CallAjax(this,
            window.rootUrl + "Gaming/Gaming/GetLiveCasinoLicenseeList",
            null,
            self.RenderCompanyList,
            self.RenderCompanyList,
            self.CompletedLoadFilter,
            false);
    };
    self.RenderCompanyList = function (rs) {
        var currencyLength = rs.length;
        self.Tables.dropdownCompany = $("#dropdown-company");
        self.Tables.dropdownCompany.empty();
        var companyLength = rs.ChildInfo.length;
        var option = '';
        option = '<option value="0">' + viewResources.All + '</option>';
        self.Tables.dropdownCompany.append(option);

        for (var i = 0; i < companyLength; i++) {
            var company = rs.ChildInfo[i];
            if (company.Id == reportManager.Enums.LicenseeId.IBC) {
                option = '<option value="' + company.Id + '">' + company.Name + '</option>';
                self.Tables.dropdownCompany.append(option);
                continue;
            }

            var licenseeLength = company.ChildInfo.length;
            for (var j = 0; j < licenseeLength; j++) {
                var licensee = company.ChildInfo[j];
                var option = '<option value="' + licensee.Id + '">' + licensee.Name + '</option>';
                self.Tables.dropdownCompany.append(option);
            }
        }

        self.CachedData.isLoadedCompanyList = true;
        self.Tables.dropdownCompany.val(reportManager.Enums.LicenseeId.IBC);
    };

    self.CompletedLoadFilter = function () {
        if (self.CachedData.isLoadedProductList &&
            self.CachedData.isLoadedCompanyList &&
            self.CachedData.isLoadedCurrencyList &&
            self.CachedData.isLoadedGameTypeList &&
            !self.CachedData.isSelectOtherSLIProducts) {
            self.GetGameTableList();
        }
    };

    self.GetGameTypeList = function () {
        var productId = self.Tables.dropdownProduct.val();
        var params = { ProductID: productId };
        AccReport.utility.CallAjax(this,
            window.rootUrl + "Gaming/LiveCasinoAgencyWinLoss/GetProductGameType",
            params,
            self.RenderGameTypeList,
            self.RenderGameTypeList,
            self.CompletedLoadFilter,
            false);
    };
    self.RenderGameTypeList = function (rs) {
        var productGameTypeLenght = rs.length;
        self.Tables.dropdownGameType = $("#dropdown-gametype");
        self.Tables.dropdownGameType.empty();
        if (productGameTypeLenght > 1) {
            self.Tables.dropdownGameType.append("<option class='all-option' value='0'>All</option>");
            for (var i = 0; i < productGameTypeLenght; i++) {
                var optionGroup = null;
                var product = rs[i];
                optionGroup = '<optgroup label="' + product.ProductName + '">';
                var gameTypesLength = product.GameTypes.length;
                for (var j = 0; j < gameTypesLength; j++) {
                    var gameType = product.GameTypes[j];
                    optionGroup += '<option class="indent-option" value="' + gameType.Id + '">' + gameType.Name + '</option>';
                }

                optionGroup += "</optgroup>"
                self.Tables.dropdownGameType.append(optionGroup);
            }
        }
        else {
            self.Tables.dropdownGameType.append("<option value='0'>All</option>");
            for (var i = 0; i < rs[0].GameTypes.length; i++) {
                var gameType = rs[0].GameTypes[i];
                var option = '<option value="' + gameType.Id + '">' + gameType.Name + '</option>';
                self.Tables.dropdownGameType.append(option);
            }
        }

        self.CachedData.isLoadedGameTypeList = true;
        this.Tables.dropdownGameType.unbind("change");
        this.Tables.dropdownGameType.change(function () {
            self.dropdownGameTypeChange();
        });
    };
    self.dropdownGameTypeChange = function () {
        var bettype = parseInt(self.Tables.dropdownGameType.val());
        var isSLIProductBettype = self.isSLIProductBettype(bettype);
        if (!isSLIProductBettype) {
            self.Tables.gameTableContainer.addClass("display-none");
            self.Tables.rdGameTableContainer.addClass("display-none");
            self.Tables.rdGameTable.removeAttr("checked");
            self.Tables.rdDaily.attr("checked", "checked");
        }

        var isAllOrSLIProduct = self.isAllOrSLIProduct(parseInt(self.Tables.dropdownProduct.val()));
        if ((isAllOrSLIProduct && isSLIProductBettype) || (isAllOrSLIProduct && bettype == 0)) {
            self.Tables.gameTableContainer.removeClass("display-none");
            AccReport.disableInput();
            self.GetGameTableList();
            self.Tables.rdGameTableContainer.removeClass("display-none");
        }
    };

    self.GetCurrencyList = function () {
        var isExcludeDataTest = self.Tables.excludeTestData.prop("checked");
        var params = { ExcludeTest: isExcludeDataTest };
        AccReport.utility.CallAjax(this,
            window.rootUrl + "Gaming/LiveCasinoAgencyWinLoss/GetCurrencyList",
            params,
            self.RenderCurrencyList,
            self.RenderCurrencyList,
            self.CompletedLoadFilter,
            false);
    };
    self.RenderCurrencyList = function (rs) {
        var currencyLength = rs.length;
        self.Tables.dropdownCurrency = $("#dropdown-currency");
        self.Tables.dropdownCurrency.empty();
        for (var i = 0; i < currencyLength; i++) {
            var currency = rs[i];
            var option = '<option value="' + currency.Id + '">' + currency.Name + '</option>';
            self.Tables.dropdownCurrency.append(option);
        }

        self.CachedData.isLoadedCurrencyList = true;
        self.Tables.dropdownCurrency.val(self.Enums.CurrencyId.BaseRM);
    };

    self.GetGameTableList = function () {
        self.Params.getGameTableParams.GameTypeId = self.Tables.dropdownGameType.val();
        self.Params.getGameTableParams.ExcludeTest = self.Tables.excludeTestData.prop("checked");
        AccReport.utility.CallAjax(this,
            window.rootUrl + "Gaming/LiveCasinoAgencyWinLoss/GetGameTableList",
            self.Params.getGameTableParams,
            self.RenderGameTableList,
            self.RenderGameTableList,
             AccReport.enableInput,
            false);
    };
    self.RenderGameTableList = function (rs) {
        self.Tables.gameTableContainer.removeClass("display-none");
        var gameTableLength = rs.length;
        self.Tables.dropdownGameTable = $("#dropdown-gametable");
        self.Tables.dropdownGameTable.empty();
        for (var i = 0; i < gameTableLength; i++) {
            var gameTable = rs[i];
            var option = '<option value="' + gameTable.Id + '">' + gameTable.Name + '</option>';
            self.Tables.dropdownGameTable.append(option);
        }

        self.CachedData.isLoadedGameTableList = true;
    };

    self.BindingEvents = function () {
        this.Tables.showHideFilter.unbind("click");
        this.Tables.showHideFilter.click(function () {
            self.ShowHideFilter($(this).hasClass("expanded"));
        });

        self.BindingEvents_UsernameSuggestion();

        this.Tables.excludeTestData.unbind("click");
        this.Tables.excludeTestData.click(function () {
            self.excludeTestDataClick();
        });

        this.Tables.btnGenerate.unbind("click");
        this.Tables.btnGenerate.click(function () {
            self.GetReport(self.Tables.datepickerplugin.nexDatePicker("getSingleDate"));
        });

        this.Tables.btnToday.unbind("click");
        this.Tables.btnToday.click(function () {
            self.btnTodayClick();
        });

        this.Tables.btnYesterday.unbind("click");
        this.Tables.btnYesterday.click(function () {
            self.btnYesterdayClick();
        });

        this.Tables.btnExport.unbind("click");
        this.Tables.btnExport.click(function () {
            self.ExportReport();
        });

        this.Tables.dropdownProduct.change(function () {
            self.changeProduct();
        });
    };

    self.ShowHideFilter = function (isHideFilter) {
        var self = this;
        if (isHideFilter) {
            this.Tables.showHideFilter.removeClass("expanded")
                                  .addClass("collapsed");
            this.Tables.showHideFilter.attr("title", viewResources.Show_filter);
            self.Tables.filterBar.addClass("display-none");
        } else {
            this.Tables.showHideFilter.removeClass("collapsed")
                                  .addClass("expanded");
            this.Tables.showHideFilter.attr("title", viewResources.Hide_filter);
            self.Tables.filterBar.removeClass("display-none");
        }

        var activeGrid = self.GetCurrentGrid();
        var gridOptions = activeGrid.fanexGrid("getCurrentGridOptions");
        if (gridOptions) {
            if (isHideFilter) {
                gridOptions.gridExpandHeight = 150;
            } else {
                gridOptions.gridExpandHeight = 350;
            }

            activeGrid.fanexGrid("resizeGrid");
        }
    };
    self.BindingEvents_UsernameSuggestion = function () {
        this.Tables.txtUsername.unbind("blur");
        this.Tables.txtUsername.blur(function () {
            var textbox = $(this);
            if (!self.CachedData.isonAuto) {
                $('#Autocomplete_1').css('display', 'none');
            }
            else {
                textbox.focus();
            }

            var inputValue = '';
            var newName = textbox.val();
            var oldName = self.CachedData.currentTextboxUserName;
            self.CachedData.isDefaultTextboxUserName = false;
            textbox.removeClass("default-suggestionbox");
            if (oldName != '' &&
            newName != '' &&
            newName != oldName) {
                inputValue = newName;
            }
            else if (oldName != '' && newName == '') {
                inputValue = oldName;
                self.CachedData.isDefaultTextboxUserName = true;
                textbox.addClass("default-suggestionbox");
            }
            else if (newName != '') {
                inputValue = newName;
            }
            else {
                self.CachedData.isDefaultTextboxUserName = true;
                textbox.addClass("default-suggestionbox");
                inputValue = self.CachedData.defaultTextboxUserName;
            }

            textbox.val(inputValue);
        });

        this.Tables.txtUsername.unbind("focus");
        this.Tables.txtUsername.focus(function () {
            var textbox = $(this);
            textbox.removeClass("default-suggestionbox");
            if (self.CachedData.isDefaultTextboxUserName) {
                textbox.val('');
            }
        });

        this.Tables.autocomplete.mouseover(function () {
            self.CachedData.isonAuto = true;
        }).mouseout(function () {
            self.Tables.txtUsername.removeClass("default-suggestionbox");
            self.CachedData.isDefaultTextboxUserName = false;
            self.CachedData.isonAuto = false;
        });
    };
    self.excludeTestDataClick = function () {
        AccReport.disableInput();
        self.CachedData.autoCompleteManager.setProperties({
            serviceUrl: window.rootWebPath.replace(/\/$/g, '') + '/ReportHelper/UserNameSuggestion?' +
                $.param(self.GetUserNameSuggestionParams()),
            minChars: 2
        });

        self.GetCurrencyList();
        var bettype = parseInt(self.Tables.dropdownGameType.val());
        var isSLIProductBettype = self.isSLIProductBettype(bettype);
        var isAllOrSLIProduct = self.isAllOrSLIProduct(parseInt(self.Tables.dropdownProduct.val()));
        if (isAllOrSLIProduct && isSLIProductBettype && !self.CachedData.isLoadedGameTableList) {
            self.GetGameTableList();
        }

        AccReport.enableInput();
    };

    self.btnTodayClick = function () {
        var self = this;
        self.SetDatePickerValue(this.Enums.DateOption.Today);
        self.GetReport(self.Tables.btnToday.val());
    };
    self.btnYesterdayClick = function () {
        var self = this;
        self.SetDatePickerValue(this.Enums.DateOption.Yesterday);
        self.GetReport(self.Tables.btnYesterday.val());
    };
    self.SetDatePickerValue = function (dateOptionId) {
        var dateOptionId = parseInt(dateOptionId);
        switch (dateOptionId) {
            case this.Enums.DateOption.Today:
                this.Tables.dateOptionSelector.val(this.Enums.DateOption.Today);
                this.Tables.firstDatePickerId1.datepicker("option", "maxDate", today);
                this.Tables.firstDatePickerId1.datepicker("setDate", today);
                this.Tables.secondDatePickerId1.datepicker("option", "minDate", today);
                this.Tables.secondDatePickerId1.datepicker("setDate", today);
                break;
            case this.Enums.DateOption.Yesterday:
                this.Tables.dateOptionSelector.val(this.Enums.DateOption.Yesterday);
                this.Tables.firstDatePickerId1.datepicker("option", "maxDate", yesterday);
                this.Tables.firstDatePickerId1.datepicker("setDate", yesterday);
                this.Tables.secondDatePickerId1.datepicker("option", "minDate", yesterday);
                this.Tables.secondDatePickerId1.datepicker("setDate", yesterday);
                break;
            default:
                break;
        }
    };
    self.GetReportParams = function () {
        var dateRange = self.Tables.datepickerplugin.nexDatePicker("getDateRange");
        var userName = self.Tables.txtUsername.val();
        var productId = self.Tables.dropdownProduct.val();
        self.CachedData.productId = productId;
        return {
            FromDate: dateRange.FromDate,
            ToDate: dateRange.ToDate,
            UserName: self.CachedData.isDefaultTextboxUserName ?
                  "" : userName,
            ExcludeTestData: this.Tables.excludeTestData.prop("checked") ? true : false,
            LicenseeId: self.Tables.dropdownCompany.val(),
            GameTypeId: self.Tables.dropdownGameType.val(),
            GameTableId: self.Tables.dropdownGameTable.val(),
            Currency: self.Tables.dropdownCurrency.val(),
            ReportType: self.Tables.reportBy.find("input[name='radio-reportby']:checked").val(),
            SortDirection: reportManager.CachedData.sortDirection,
            SortColumnName: reportManager.CachedData.sortColName,
            SortDataType: reportManager.CachedData.sortDataType,
            ProductId: productId
        };
    };

    self.GetReport = function (reportDate) {
        var self = this;

        self.CachedData.sortColName = null;
        self.CachedData.sortDirection = null;
        self.CachedData.sortDataType = null;
        AccReport.disableInput();
        var selectParam = self.GetReportParams();

        self.CachedData.currentPage = 0;
        self.Params.reportParams = selectParam;
        AccReport.utility.CallAjax(this,
            window.rootUrl + "Gaming/LiveCasinoAgencyWinLoss/GetReport",
            self.Params.reportParams,
            self.RenderReport,
            self.RenderReport,
            AccReport.enableInput);
    };

    self.RenderReport = function (report) {
        var self = this;
        self.CachedData.currentReport = report;
        var reportTypeEnum = self.Enums.ReportType;
        var reportType = parseInt(self.Params.reportParams.ReportType);
        if (reportType == reportTypeEnum.Agent) {
            self.ReportByAgent.RenderReport(report);
            return;
        }

        self.ReportByAgent.Reset();
        self.Tables.mainLiveCasinoAgencyWinLossReportContainer.removeClass("display-none");
        self.Tables.mainLiveCasinoAgencyWinLossReport.addClass("display-none");

        var gridOptions = null;
        switch (reportType) {
            case reportTypeEnum.Daily:
                gridOptions = self.ReportByDailyRange.GetGridOptions(report);
                break;
            case reportTypeEnum.Monthly:
                gridOptions = self.ReportByMonthlyRange.GetGridOptions(report);
                break;
            case reportTypeEnum.UserName:
                gridOptions = self.ReportByUserName.GetGridOptions(report);
                break;
            case reportTypeEnum.GameType:
                gridOptions = self.ReportByGameType.GetGridOptions(report);
                break;
            case reportTypeEnum.GameTable:
                gridOptions = self.ReportByGameTable.GetGridOptions(report);
                break;
            default:
                break;
        }

        self.CachedData.gridOptions = gridOptions;
        if (gridOptions != null) {
            this.Tables.mainLiveCasinoAgencyWinLossReport.removeClass("display-none").fanexGrid(gridOptions);
        }
    };

    self.ExportReport = function () {
        var self = this;
        var exportParams = self.GetReportParams();
        exportParams.IsChangedValue = true;
        AccReport.downloadFile(window.rootUrl + "Gaming/LiveCasinoAgencyWinLoss/ExportReport", exportParams);
    };

    self.GetPagingIndexRange = function (maxLength) {
        var self = this;
        var itemsPerPage = parseInt(self.CachedData.itemsPerPage);
        var startIndex = self.CachedData.itemsPerPage * self.CachedData.currentPage;
        var endIndex = startIndex + itemsPerPage;
        endIndex = endIndex <= maxLength ? endIndex : maxLength;
        return {
            startIndex: startIndex,
            endIndex: endIndex
        };
    };

    self.CompareValues = function (sortInfo,
        valueA,
        valueB) {
        var sortDataTypeEnums = this.Enums.SortDataType;
        var isGreaterThan;
        switch (sortInfo.sortDataType) {
            case sortDataTypeEnums.Number:
                isGreaterThan = (sortInfo.sortFactor == 0 || valueB == valueA) ?
                                0 :
                                (valueA - valueB) * sortInfo.sortFactor > 0 ?
                                1 :
                                -1;
                break;
            case sortDataTypeEnums.String:
                isGreaterThan = (sortInfo.sortFactor == 0 || valueB == valueA) ?
                                0 :
                                (valueB < valueA && sortInfo.sortFactor == 1) ||
                                (valueA < valueB && sortInfo.sortFactor == -1) ?
                                1 :
                                -1;
                break;
            case sortDataTypeEnums.Date:
                var dateA = new Date(valueA);
                var dateB = new Date(valueB);
                isGreaterThan = (sortInfo.sortFactor == 0 || dateB == dateA) ?
                                0 :
                                (dateB < dateA && sortInfo.sortFactor == 1) ||
                                (dateA < dateB && sortInfo.sortFactor == -1) ?
                                1 :
                                -1;
                break;
        }

        return isGreaterThan;
    };

    self.IsChangedCriteria = function (oldCriteria,
        newCriteria) {
        return oldCriteria == null ||
           newCriteria.FromDate != oldCriteria.FromDate ||
           newCriteria.ToDate != oldCriteria.ToDate ||
           newCriteria.UserName != oldCriteria.UserName ||
           newCriteria.ExcludeTestData != oldCriteria.ExcludeTestData ||
           newCriteria.LicenseeId != oldCriteria.LicenseeId ||
           newCriteria.GameTypeId != oldCriteria.GameTypeId ||
           newCriteria.GameTableId != oldCriteria.GameTableId ||
           newCriteria.Currency != oldCriteria.Currency ||
           newCriteria.ReportType != oldCriteria.ReportType ||
           newCriteria.ProductId != oldCriteria.ProductId;
    };

    self.GetDownlinesWinLoss = function (winLossInfo) {
        var roleIndex = reportManager.CachedData.RoleIndex;
        return {
            memberWinLoss: winLossInfo.ChildsInfo[roleIndex["Member"]],
            agentWinLoss: winLossInfo.ChildsInfo[roleIndex["Agent"]],
            masterWinLoss: winLossInfo.ChildsInfo[roleIndex["Master"]],
            superWinLoss: winLossInfo.ChildsInfo[roleIndex["Super"]],
            companyWinLoss: winLossInfo.ChildsInfo[roleIndex["Company"]]
        };
    };

    self.GetCurrentGrid = function () {
        var reportType = parseInt(self.Params.reportParams.ReportType);
        var reportTypeEnum = self.Enums.ReportType;
        var result = null;
        switch (reportType) {
            case reportTypeEnum.Daily:
            case reportTypeEnum.Monthly:
            case reportTypeEnum.UserName:
            case reportTypeEnum.GameType:
            case reportTypeEnum.GameTable:
                result = self.Tables.mainLiveCasinoAgencyWinLossReport;
                break;
            case reportTypeEnum.Agent:
                result = self.ReportByAgent.CachedData.currentReportManager.Tables.report;
                break;
            default:
                result = null;
                break;
        }

        return result;
    };

    self.getSortDirection = function (columnName) {
        return columnName == self.CachedData.sortColName ? self.CachedData.sortDirection : '';
    };

    self.AddChildTotalToBodyData = function (columns, info, downlinesWinLoss, bodyData) {
        columns = [];
        columns.push({ Name: "ColName", Value: info.Name, MergeColNumber: 2, TdClass: "subTotal bold" });
        columns.push({ Name: "ColRollingTurnover", Value: info.RollingTurnover, TdClass: "subTotal bold" });
        columns.push({ Name: "ColCompanyTurnover", Value: downlinesWinLoss.companyWinLoss.Turnover, TdClass: "subTotal bold" });
        columns.push({ Name: "ColGrossCommission", Value: info.GrossCommission, TdClass: "subTotal bold" });
        columns.push({ Name: "ColTotalBets", Value: info.TotalBets, TdClass: "subTotal bold" });

        columns.push({ Name: "ColMemberWinLoss", Value: downlinesWinLoss.memberWinLoss.WinLoss, TdClass: "subTotal bold" });
        columns.push({ Name: "ColMemberCommission", Value: downlinesWinLoss.memberWinLoss.Commission, TdClass: "subTotal bold" });
        columns.push({ Name: "ColMemberTotal", Value: downlinesWinLoss.memberWinLoss.RealWinLoss, TdClass: "subTotal bold" });

        columns.push({ Name: "ColAgentWinLoss", Value: downlinesWinLoss.agentWinLoss.WinLoss, TdClass: "subTotal bold" });
        columns.push({ Name: "ColAgentCommission", Value: downlinesWinLoss.agentWinLoss.Commission, TdClass: "subTotal bold" });
        columns.push({ Name: "ColAgentTotal", Value: downlinesWinLoss.agentWinLoss.RealWinLoss, TdClass: "subTotal bold" });

        columns.push({ Name: "ColMasterWinLoss", Value: downlinesWinLoss.masterWinLoss.WinLoss, TdClass: "subTotal bold" });
        columns.push({ Name: "ColMasterCommission", Value: downlinesWinLoss.masterWinLoss.Commission, TdClass: "subTotal bold" });
        columns.push({ Name: "ColMasterTotal", Value: downlinesWinLoss.masterWinLoss.RealWinLoss, TdClass: "subTotal bold" });

        columns.push({ Name: "ColSuperWinLoss", Value: downlinesWinLoss.superWinLoss.WinLoss, TdClass: "subTotal bold" });
        columns.push({ Name: "ColSuperCommission", Value: downlinesWinLoss.superWinLoss.Commission, TdClass: "subTotal bold" });
        columns.push({ Name: "ColSuperTotal", Value: downlinesWinLoss.superWinLoss.RealWinLoss, TdClass: "subTotal bold" });

        columns.push({ Name: "ColCompanyWinLoss", Value: downlinesWinLoss.companyWinLoss.WinLoss, TdClass: "subTotal bold" });
        bodyData.push({
            Columns: columns
        });
    };

    self.SortReportData = function (params) {
        AccReport.disableInput();
        var data = params;
        AccReport.utility.CallAjax(this,
            window.rootUrl + "Gaming/LiveCasinoAgencyWinLoss/SortReport",
            data,
            self.RenderReportAfterServerSort,
            self.RenderReportAfterServerSort,
            AccReport.enableInput,
            false);
    };

    self.RenderReportAfterServerSort = function (report) {
        var self = this;
        self.CachedData.currentReport = report;
        var reportTypeEnum = self.Enums.ReportType;
        var reportType = parseInt(self.Params.reportParams.ReportType);
        if (reportType == reportTypeEnum.Agent) {
            self.ReportByAgent.RenderReportAfterServerSort(report);
            return;
        }

        self.Tables.mainLiveCasinoAgencyWinLossReportContainer.removeClass("display-none");
        self.Tables.mainLiveCasinoAgencyWinLossReport.addClass("display-none");
        var gridOptions = null;
        switch (reportType) {
            case reportTypeEnum.Daily:
                gridOptions = self.ReportByDailyRange.GetGridOptions(report);
                break;
            case reportTypeEnum.Monthly:
                gridOptions = self.ReportByMonthlyRange.GetGridOptions(report);
                break;
            case reportTypeEnum.UserName:
                gridOptions = self.ReportByUserName.GetGridOptions(report);
                break;
            case reportTypeEnum.GameType:
                gridOptions = self.ReportByGameType.GetGridOptions(report);
                break;
            case reportTypeEnum.GameTable:
                gridOptions = self.ReportByGameTable.GetGridOptions(report);
                break;
            default:
                break;
        }

        self.CachedData.gridOptions = gridOptions;
        if (gridOptions != null) {
            this.Tables.mainLiveCasinoAgencyWinLossReport.removeClass("display-none").fanexGrid(gridOptions);
        }
    };

    self.MapSortInfo = function (columnName) {
        var sortInfo = {
            sortColumnName: null,
            sortDataType: null
        };

        switch (columnName) {
            case "ColName":
                sortInfo.sortColumnName = "Name";
                sortInfo.sortDataType = self.Enums.SortDataType.String;
                break;
            case "ColRollingTurnover":
                sortInfo.sortColumnName = "RollingTurnover";
                sortInfo.sortDataType = self.Enums.SortDataType.Number;
                break;
            case "ColMemberTotal":
                sortInfo.sortColumnName = "MemberTotal";
                sortInfo.sortDataType = self.Enums.SortDataType.Number;
                break;
            case "ColAgentTotal":
                sortInfo.sortColumnName = "AgentTotal";
                sortInfo.sortDataType = self.Enums.SortDataType.Number;
                break;
            case "ColMasterTotal":
                sortInfo.sortColumnName = "MasterTotal";
                sortInfo.sortDataType = self.Enums.SortDataType.Number;
                break;
            case "ColSuperTotal":
                sortInfo.sortColumnName = "SuperTotal";
                sortInfo.sortDataType = self.Enums.SortDataType.Number;
                break;
            case "ColCompanyWinLoss":
                sortInfo.sortColumnName = "CompanyWinLoss";
                sortInfo.sortDataType = self.Enums.SortDataType.Number;
                break;
        }

        return sortInfo;
    };

    self.MapSortInfoByTime = function (columnName) {
        var sortInfo = {
            sortColumnName: null,
            sortDataType: null
        };

        switch (columnName) {
            case "ColName":
                sortInfo.sortColumnName = "Name";
                sortInfo.sortDataType = self.Enums.SortDataType.Date;
                break;
            case "ColRollingTurnover":
                sortInfo.sortColumnName = "RollingTurnover";
                sortInfo.sortDataType = self.Enums.SortDataType.Number;
                break;
            case "ColMemberTotal":
                sortInfo.sortColumnName = "MemberTotal";
                sortInfo.sortDataType = self.Enums.SortDataType.Number;
                break;
            case "ColAgentTotal":
                sortInfo.sortColumnName = "AgentTotal";
                sortInfo.sortDataType = self.Enums.SortDataType.Number;
                break;
            case "ColMasterTotal":
                sortInfo.sortColumnName = "MasterTotal";
                sortInfo.sortDataType = self.Enums.SortDataType.Number;
                break;
            case "ColSuperTotal":
                sortInfo.sortColumnName = "SuperTotal";
                sortInfo.sortDataType = self.Enums.SortDataType.Number;
                break;
            case "ColCompanyWinLoss":
                sortInfo.sortColumnName = "CompanyWinLoss";
                sortInfo.sortDataType = self.Enums.SortDataType.Number;
                break;
        }

        return sortInfo;
    };

    self.changeProduct = function () {
        var productId = parseInt(self.Tables.dropdownProduct.val());
        var isAllOrSLIProduct = self.isAllOrSLIProduct(productId);
        if (isAllOrSLIProduct) {
            self.Tables.gameTableContainer.removeClass("display-none");
            self.CachedData.isSelectOtherSLIProducts = false;
            self.GetGameTypeList();
            self.Tables.rdGameTableContainer.removeClass("display-none");
        }
        else {
            self.CachedData.isSelectOtherSLIProducts = true;
            self.GetGameTypeList();
            self.Tables.gameTableContainer.addClass("display-none");
            self.Tables.rdGameTable.removeAttr("checked");
            self.Tables.rdDaily.attr("checked", "checked");
            self.Tables.rdGameTableContainer.addClass("display-none");
        }
    };

    self.isSLIProductBettype = function (bettype) {
        var isSLIProductBettype = false;
        if (bettype < self.Enums.SLIBettypeRange.MinBettypeValue || bettype > self.Enums.SLIBettypeRange.MaxBettypeValue) {
            isSLIProductBettype = false;
        }
        else {
            isSLIProductBettype = true;
        }

        return isSLIProductBettype;
    }

    self.isAllOrSLIProduct = function (productId) {
        var isAllOrSLIProduct = false;
        if (productId == self.Enums.Product.All || productId == self.Enums.Product.LiveCasino) {
            isAllOrSLIProduct = true;
        }

        return isAllOrSLIProduct;
    }
};

var reportManager = new Fanex.AccountingReports.Gaming.LiveCasinoAgencyWinLoss.ReportManager();




