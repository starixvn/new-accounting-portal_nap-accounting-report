$(document).ready(function () {
    reportManager.InitPage();
});

ReportManager = function () {
    var self = this;

    self.Tables = {
        datepickerplugin: null,
        dateOptionSelector: null,
        firstDatePickerId1: null,
        secondDatePickerId1: null,
        last3MonthsDatePicker: null,
        showHideFilter: null,
        filterBar: null,
        txtUserName: null,
        autocomplete: null,
        excludeTestAccounts: null,
        autoCompleteManager: null,
        dropdownCompany: null,
        dropdownCurrency: null,

        btnGenerate: null,
        btnToday: null,
        btnYesterday: null,
        btnExport: null,

        mainBingoStatisticReportContainer: null,
        mainBingoStatisticReport: null,
        dropdownCompanyHeaderCheckBox: null
    };

    self.Enums = {
        DateOption: {
            Last3Months: 8,
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
            Company: 5,
            Currency: 6,
            Agent: 7
        },

        SortDirection: {
            Asc: "ASC",
            Desc: "DESC"
        },

        SortDataType: {
            Number: "number",
            String: "string",
            Date: "Date"
        }
    };

    self.Params = {
        reportParams: {
            FromDate: null,
            ToDate: null,
            UserName: null,
            ExcludeTestAccounts: null,
            LicenseeList: null,
            Currency: null,
            ReportType: null,
            IsChangedValue: false
        }
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
        maxGridHeight: 10000
    };
};

ReportManager.prototype.InitPage = function () {
    this.RenderDatePicker();
    this.CachedDOMElements();
    this.MakeUserNameSuggestion();
    this.LoadFilterValues();
    this.BindingEvents();
    this.Tables.btnToday.val(today);
    this.Tables.btnYesterday.val(yesterday);
};

ReportManager.prototype.RenderDatePicker = function () {
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
            LabelCss: "lbl-field col_3 prefix_1",
            OptionCss: "edit-field col_7"
        },
        firstDatePicker: {
            Id: "firstDatePickerId1",
            Label: viewResources.From_Date,
            InitDate: today,
            LabelCss: "lbl-field col_3 align-left",
            TextboxCss: "edit-field col_6"
        },
        secondDatePicker: {
            Id: "secondDatePickerId1",
            Label: viewResources.To_Date,
            InitDate: today,
            LabelCss: "lbl-field col_2 align-left",
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
        AccReport.utility.StringFormat(viewResources.Options_LastMonthRange, [this.CachedData.reportMonthRange]) + '</option>';

    this.Tables.dateOptionSelector.append(dateOption);
    this.Tables.last3MonthsDatePicker = $("#last3MonthsDatePicker");
};

ReportManager.prototype.MakeUserNameSuggestion = function () {
    var self = this;
    this.CachedData.autoCompleteManager = this.Tables.txtUserName.autocomplete({
        serviceUrl: window.rootWebPath.replace(/\/$/g, '') + '/ReportHelper/UserNameSuggestion?' +
            $.param(self.GetUserNameSuggestionParams()),
        minChars: 2
    });

    this.Tables.autocomplete = $('.autocomplete');
};

ReportManager.prototype.ExtendDateOptionEvent = function (dateOptionId) {
    var dateOptionId = parseInt(dateOptionId);
    switch (dateOptionId) {
        case this.Enums.DateOption.Today:
            this.Tables.dateOptionSelector.val(this.Enums.DateOption.Today);
            this.Tables.firstDatePickerId1.datepicker("setDate", today);
            this.Tables.secondDatePickerId1.datepicker("setDate", today);
            break;
        case this.Enums.DateOption.Yesterday:
            this.Tables.dateOptionSelector.val(this.Enums.DateOption.Yesterday);
            this.Tables.firstDatePickerId1.datepicker("setDate", yesterday);
            this.Tables.secondDatePickerId1.datepicker("setDate", yesterday);
            break;
        case this.Enums.DateOption.Last3Months:
            this.Tables.firstDatePickerId1.datepicker("setDate", beginLast3Months);
            this.Tables.secondDatePickerId1.datepicker("setDate", endLast3Months);
            break;
    }
};

ReportManager.prototype.CachedDOMElements = function () {
    this.Tables.showHideFilter = $(".showHideFilter");
    this.Tables.filterBar = $("#filter-bar");
    this.Tables.txtUserName = $("#txtUserName");
    this.Tables.excludeTestAccounts = $("#exclude-test-account");
    this.Tables.dropdownCompany = $("#dropdownCompany");
    this.Tables.dropdownCurrency = $("#dropdownCurrency");
    this.Tables.reportBy = $("#reportBy");

    this.Tables.btnGenerate = $("#btnGenerate");
    this.Tables.btnToday = $("#btnToday");
    this.Tables.btnYesterday = $("#btnYesterday");
    this.Tables.btnExport = $("#btnExport");

    this.Tables.mainBingoStatisticReportContainer = $("#mainBingoStatisticReportContainer");
    this.Tables.mainBingoStatisticReport = $("#mainBingoStatisticReport");
};

ReportManager.prototype.BindingEvents = function () {
    var self = this;

    this.Tables.showHideFilter.unbind("click");
    this.Tables.showHideFilter.click(function () {
        self.ShowHideFilter($(this).hasClass("expanded"));
    });

    this.Tables.txtUserName.unbind("blur");
    this.Tables.txtUserName.blur(function () {
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
        textbox.removeClass("defaultSuggestionBox");
        if (oldName != '' &&
            newName != '' &&
            newName != oldName) {
            inputValue = newName;
        }
        else if (oldName != '' && newName == '') {
            inputValue = oldName;
            self.CachedData.isDefaultTextboxUserName = true;
            textbox.addClass("defaultSuggestionBox");
        }
        else if (newName != '') {
            inputValue = newName;
        }
        else {
            self.CachedData.isDefaultTextboxUserName = true;
            textbox.addClass("defaultSuggestionBox");
            inputValue = self.CachedData.defaultTextboxUserName;
        }

        textbox.val(inputValue);
    });

    this.Tables.txtUserName.unbind("focus");
    this.Tables.txtUserName.focus(function () {
        var textbox = $(this);
        textbox.removeClass("defaultSuggestionBox");
        if (self.CachedData.isDefaultTextboxUserName) {
            textbox.val('');
        }
    });

    this.Tables.autocomplete.mouseover(function () {
        self.CachedData.isonAuto = true;
    }).mouseout(function () {
        self.Tables.txtUserName.removeClass("defaultSuggestionBox");
        self.CachedData.isDefaultTextboxUserName = false;
        self.CachedData.isonAuto = false;
    });

    this.Tables.excludeTestAccounts.unbind("click");
    this.Tables.excludeTestAccounts.click(function () {
        self.CachedData.autoCompleteManager.setProperties({
            serviceUrl: window.rootWebPath.replace(/\/$/g, '') + '/ReportHelper/UserNameSuggestion?' +
                $.param(self.GetUserNameSuggestionParams()),
            minChars: 2
        });
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

    this.Tables.last3MonthsDatePicker.unbind("click");
    this.Tables.last3MonthsDatePicker.click(function () {
        reportManager.ExtendDateOptionEvent($(this).val());
    });
};

ReportManager.prototype.ShowHideFilter = function (isHideFilter) {
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

    var gridOptions = this.Tables.mainBingoStatisticReport.fanexGrid("getCurrentGridOptions");
    if (gridOptions) {
        if (isHideFilter) {
            gridOptions.gridExpandHeight = 150;
        } else {
            gridOptions.gridExpandHeight = 350;
        }

        this.Tables.mainBingoStatisticReport.fanexGrid("resizeGrid");
    }
};

ReportManager.prototype.GetUserNameSuggestionParams = function () {
    return {
        excludeTestAccounts: this.Tables.excludeTestAccounts.attr("checked") ? true : false
    };
};

ReportManager.prototype.LoadFilterValues = function () {
    var self = this;
    AccReport.disableInput();
    AccReport.utility.CallAjax(this,
        window.rootUrl + "Gaming/Gaming/GetLicensee_SupportBingo",
        null,
        self.RenderCompanyFilter,
        self.RenderCompanyFilter,
        self.GetCurrencyFilter);
};

ReportManager.prototype.RenderCompanyFilter = function (result) {
    var self = this;
    this.Tables.dropdownCompany.empty();
    if (result && result.length > 0) {
        var optionLength = result.length;
        var ddwnCompanies = [];
        var prefixId = 'ddlCompany_';
        var isCheckAll = true;
        for (var i = 0; i < optionLength; i++) {
            var info = result[i];
            var isChecked = info.Id == self.Enums.LicenseeId.IBC ? true : false;
            ddwnCompanies.push({
                Id: prefixId + info.Id,
                Value: info.Id,
                Text: info.Name,
                Checked: isChecked,
                Enable: true,
                CssClass: "",
                Items: []
            });

            if (!isChecked) {
                isCheckAll = false;
            }
        }

        this.Tables.dropdownCompany.multiSelect(
        {
            minWidth: 168,
            maxHeight: 450,
            shadow: true,
            showHeader: true,
            isCheckAll: isCheckAll,
            showFooter: false,
            showTooltip: true,
            checkAllText: viewResources.All_Companies,
            isUsedCustomCheckAllText: true,
            customCheckAllText: viewResources.All_Companies,
            noItemText: viewResources.No_company_found,
            selectedList: 3,
            listData: ddwnCompanies
        });

        this.Tables.dropdownCompany = $("#dropdownCompany");
        this.Tables.dropdownCompanyHeaderCheckBox = $("#dropdownCompanyHeaderCheckBox");
    }
};

ReportManager.prototype.GetCurrencyFilter = function () {
    var self = reportManager;
    var selectParam = {
        LicenseeList: self.Tables.dropdownCompany.multiSelect("getCheckedItems").join(','),
        ExcludeTest: false
    };

    AccReport.utility.CallAjax(this,
        window.rootWebPath.replace(/\/$/g, '') + "/ReportHelper/GetCurrencyByLicensee",
        selectParam,
        self.RenderCurrencyFilter,
        self.RenderCurrencyFilter,
        AccReport.enableInput);
};

ReportManager.prototype.RenderCurrencyFilter = function (result) {
    var self = reportManager;
    self.Tables.dropdownCurrency.empty();
    if (result && result.length > 0) {
        var option = '';
        var optionLength = result.length;
        option = '<option value="' + self.Enums.CurrencyId.All + '">' + viewResources.All + '</option>';
        self.Tables.dropdownCurrency.append(option);
        for (var i = 0; i < optionLength; i++) {
            var info = result[i];
            option = '<option value="' + info.Id + '">' + info.Name + '</option>';
            self.Tables.dropdownCurrency.append(option);
        }

        self.Tables.dropdownCurrency.val(self.Enums.CurrencyId.BaseRM);
    }
};

ReportManager.prototype.btnTodayClick = function () {
    var self = this;
    self.ExtendDateOptionEvent(this.Enums.DateOption.Today);
    self.GetReport(self.Tables.btnToday.val());
};

ReportManager.prototype.btnYesterdayClick = function () {
    var self = this;
    self.ExtendDateOptionEvent(this.Enums.DateOption.Yesterday);
    self.GetReport(self.Tables.btnYesterday.val());
};

ReportManager.prototype.GetReport = function (reportDate) {
    var self = this;
    AccReport.disableInput();
    var dateRange = self.Tables.datepickerplugin.nexDatePicker("getDateRange");
    var userName = self.Tables.txtUserName.val();
    var selectParam = {
        FromDate: dateRange.FromDate,
        ToDate: dateRange.ToDate,
        UserName: self.CachedData.isDefaultTextboxUserName ?
                  "" : userName,
        ExcludeTestAccounts: this.Tables.excludeTestAccounts.attr("checked") ? true : false,
        LicenseeList: this.Tables.dropdownCompanyHeaderCheckBox.attr("checked") ?
                      "0" :
                      self.Tables.dropdownCompany.multiSelect("getCheckedItems").join(','),
        Currency: self.Tables.dropdownCurrency.find("option:selected").val(),
        ReportType: self.Tables.reportBy.find("input[name='rdReportBy']:checked").val()
    };

    self.CachedData.currentPage = 0;
    self.Params.reportParams = selectParam;
    AccReport.utility.CallAjax(this,
        window.rootUrl + "Gaming/BingoStatistic/GetReport",
        self.Params.reportParams,
        self.RenderReport,
        self.RenderReport,
        AccReport.enableInput);
};

ReportManager.prototype.RenderReport = function (report) {
    var self = this;
    this.Tables.mainBingoStatisticReportContainer.removeClass("display-none");
    this.Tables.mainBingoStatisticReport.addClass("display-none");
    var reportTypeEnum = self.Enums.ReportType;

    var gridOptions = null;
    var reportType = parseInt(self.Params.reportParams.ReportType);
    self.CachedData.currentReport = report;
    switch (reportType) {
        case reportTypeEnum.Daily:
            gridOptions = self.GetGridOptions_ByDailyRange(report);
            break;
        case reportTypeEnum.Monthly:
            gridOptions = self.GetGridOptions_ByMonthlyRange(report);
            break;
        case reportTypeEnum.UserName:
            gridOptions = self.GetGridOptions_ByUserName(report);
            break;
        case reportTypeEnum.Company:
            gridOptions = self.GetGridOptions_ByCompany(report);
            break;
        case reportTypeEnum.Currency:
            gridOptions = self.GetGridOptions_ByCurrency(report);
            break;
        case reportTypeEnum.Agent:
            gridOptions = self.GetGridOptions_ByAgent(report);
            break;
        default:
            break;
    }

    self.CachedData.gridOptions = gridOptions;
    if (gridOptions != null) {
        this.Tables.mainBingoStatisticReport.removeClass("display-none").fanexGrid(gridOptions);
    }

    //    if (report && report.ChildsInfo && report.ChildsInfo.length > 0) {
    //        this.ShowHideFilter(true);
    //    }
};

ReportManager.prototype.ExportReport = function () {
    var self = this;
    var dateRange = self.Tables.datepickerplugin.nexDatePicker("getDateRange");
    var userName = self.Tables.txtUserName.val();
    var exportParams = {
        FromDate: dateRange.FromDate,
        ToDate: dateRange.ToDate,
        UserName: self.CachedData.isDefaultTextboxUserName ?
                  "" : userName,
        ExcludeTestAccounts: this.Tables.excludeTestAccounts.attr("checked") ? true : false,
        LicenseeList: this.Tables.dropdownCompanyHeaderCheckBox.attr("checked") ?
                      "0" :
                      self.Tables.dropdownCompany.multiSelect("getCheckedItems").join(','),
        Currency: self.Tables.dropdownCurrency.find("option:selected").val(),
        ReportType: self.Tables.reportBy.find("input[name='rdReportBy']:checked").val()
    };

    exportParams.IsChangedValue = self.IsChangedCriteria(self.Params.reportParams, exportParams);
    AccReport.downloadFile(window.rootUrl + "Gaming/BingoStatistic/ExportReport", exportParams);
};

ReportManager.prototype.GetPagingIndexRange = function (maxLength) {
    var self = this;
    var startIndex = self.CachedData.itemsPerPage * self.CachedData.currentPage;
    var endIndex = startIndex + self.CachedData.itemsPerPage;
    endIndex = endIndex <= maxLength ? endIndex : maxLength;
    return {
        startIndex: startIndex,
        endIndex: endIndex
    };
};

ReportManager.prototype.CompareValues = function (sortInfo,
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

ReportManager.prototype.IsChangedCriteria = function (oldCriteria,
    newCriteria) {
    return oldCriteria == null ||
           newCriteria.FromDate != oldCriteria.FromDate ||
           newCriteria.ToDate != oldCriteria.ToDate ||
           newCriteria.UserName != oldCriteria.UserName ||
           newCriteria.ExcludeTestAccounts != oldCriteria.ExcludeTestAccounts ||
           newCriteria.LicenseeList != oldCriteria.LicenseeList ||
           newCriteria.Currency != oldCriteria.Currency ||
           newCriteria.ReportType != oldCriteria.ReportType;
};

var reportManager = new ReportManager();