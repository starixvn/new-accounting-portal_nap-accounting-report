
$(document).ready(function () {
    reportManager.InitPage();
});

Namespace.Register("Fanex.AccountingReports.Gaming.NumberGameWinLossByBetTypeMatch");
Fanex.AccountingReports.Gaming.NumberGameWinLossByBetTypeMatch.ReportManager = function () {
    var self = this;

    self.WinLossByBetType = new Fanex.AccountingReports.Gaming.NumberGameWinLossByBetTypeMatch.WinLossByBetType();
    self.WinLossByMatch = new Fanex.AccountingReports.Gaming.NumberGameWinLossByBetTypeMatch.WinLossByMatch();

    self.Tables = {
        buttonGenerate: null,
        dropdownReportBy: null,
        dropdownCompany: null,
        dropdownCurrency: null,
        dropdownBettype: null,
        datepickerplugin: null,
        dropdownLiveStatus: null,
        dropdownBetType: null,
        chkTrialData: null,

        winLossByBetTypeFilter: null,
        winLossByMatchFilter: null
    };

    self.Enums = {
        ReportBy: {
            ByBetType: 1,
            ByMatch: 2
        }
    };

    self.Params = {
        winLossByBetType_MainReportParams: {
            CompanyList: null,
            CurrencyList: null,
            LiveStatus: null,
            ReportDate: null,
            TrialData: false
        },

        winLossByMatch_MainReportParams: {
            ReportDate: null,
            CompanyList: null,
            CurrencyList: null,
            BetTypeList: null,
            TrialData: false
        },

        licenseesByProductParams: {
            FromDate: null,
            ToDate: null,
            GetIsActiveOnly: true,
            ExcludeLicenseeID: 18,
            SystemID: 3,
            ProductPrefix: "NG",
            TrialData: false
        }
    };

    self.CachedData = {
        isLoadedCompanyList: false,
        isLoadedCurrencyList: false,
        isLoadedBetTypeList: false,
        isLoadedDatePicker: false,

        reportBy: -1
    };

    self.InitPage = function () {
        AccReport.disableInput();
        self.CachedDOMElements();
        self.BindingEvents();
        self.LoadFilter();
        self.WinLossByBetType.InitPage();
        self.WinLossByMatch.InitPage();
        self.CachedData.reportBy = self.Enums.ReportBy.ByBetType;
        self.dropdownReportByChange();
    };

    self.CachedDOMElements = function () {
        self.Tables.buttonGenerate = $("#button-generate");
        self.Tables.dropdownReportBy = $("#dropdown-reportby");
        self.Tables.dropdownLiveStatus = $("#dropdown-livestatus");
        self.Tables.winLossByBetTypeFilter = $(".winloss-bybettype-filter");
        self.Tables.winLossByMatchFilter = $(".winloss-bymatch-filter");
        self.Tables.chkTrialData = $(".checkbox-trialdata");
    };

    self.BindingEvents = function () {
        self.Tables.dropdownReportBy.unbind("change");
        self.Tables.dropdownReportBy.change(function () {
            self.dropdownReportByChange();
        });

        self.Tables.buttonGenerate.unbind("click");
        self.Tables.buttonGenerate.click(function () {
            self.buttonGenerateClick();
        });

        self.Tables.chkTrialData.change(function () {
            self.checkboxTrialDataChange();
        });
    };

    self.LoadFilter = function () {        
        self.GetCurrencyList();
        self.GetBetTypeList();
        self.RenderDatePicker();
        self.GetCompanyList();
    };

    self.GetCompanyList = function () {
        self.Params.licenseesByProductParams.FromDate = self.Tables.datepickerplugin.nexDatePicker("getSingleDate");
        self.Params.licenseesByProductParams.ToDate = self.Tables.datepickerplugin.nexDatePicker("getSingleDate");
        self.Params.licenseesByProductParams.TrialData = self.Tables.chkTrialData.is(":checked");

        AccReport.utility.CallAjax(this,
            window.rootUrl + "Gaming/Gaming/GetLicenseesByProduct",
            self.Params.licenseesByProductParams,
            self.RenderCompanyList,
            self.RenderCompanyList,
            self.CompletedLoadFilter,
            false);
    };
    self.RenderCompanyList = function (rs) {
        var id = "dropdown-company";
        var companies = [];
        var companyLength = rs.ChildInfo.length;
        for (var i = 0; i < companyLength; i++) {
            var company = rs.ChildInfo[i];
            var licensees = [];
            var licenseeLength = company.ChildInfo.length;
            for (var j = 0; j < licenseeLength; j++) {
                var licensee = company.ChildInfo[j];
                licensees.push({
                    Id: id + "-group" + company.Id +
                             "-licensee" + licensee.Id.toString(),
                    Value: licensee.Id,
                    Text: licensee.Name,
                    Checked: true,
                    Enable: true,
                    CssClass: "",
                    Items: []
                });
            }

            companies.push({
                Id: id + "-group" + company.Id,
                Value: company.Id,
                Text: company.Name,
                Checked: true,
                Enable: true,
                CssClass: "",
                Items: licensees
            });
        }

        $("#dropdown-company").multiSelect(
        {
            minWidth: 160,
            maxHeight: 450,
            shadow: true,
            showHeader: true,
            isCheckAll: true,
            showFooter: false,
            checkAllText: viewResources.All_Licensees,
            isUsedCustomCheckAllText: true,
            customCheckAllText: viewResources.All_Licensees,
            noItemText: viewResources.Error_NoLicenseeFound,
            listData: companies
        });

        self.Tables.dropdownCompany = $("#dropdown-company");
        self.CachedData.isLoadedCompanyList = true;
    };

    self.GetCurrencyList = function () {
        AccReport.utility.CallAjax(this,
            window.rootUrl + "Gaming/NumberGameWinLossByBetTypeMatch/GetCurrencyList",
            null,
            self.RenderCurrencyList,
            self.RenderCurrencyList,
            self.CompletedLoadFilter,
            false);
    };
    self.RenderCurrencyList = function (rs) {
        var id = "dropdown-currency";
        var currencies = [];
        var currencyLength = rs.length;
        for (var i = 0; i < currencyLength; i++) {
            var currency = rs[i];
            currencies.push({
                Id: id + currency.Id,
                Value: currency.Id,
                Text: currency.Name,
                Checked: true,
                Enable: true,
                CssClass: "",
                Items: []
            });
        }

        $("#dropdown-currency").multiSelect(
        {
            minWidth: 120,
            maxHeight: 450,
            shadow: true,
            showHeader: true,
            isCheckAll: true,
            showFooter: false,
            checkAllText: viewResources.All_Currencies,
            isUsedCustomCheckAllText: true,
            customCheckAllText: viewResources.All_Currencies,
            noItemText: viewResources.Error_NoCurrencyFound,
            listData: currencies
        });

        self.Tables.dropdownCurrency = $("#dropdown-currency");
        self.CachedData.isLoadedCurrencyList = true;
    };

    self.GetBetTypeList = function () {
        AccReport.utility.CallAjax(this,
            window.rootUrl + "Gaming/Gaming/GetNumberGameBetTypeList",
            null,
            self.RenderBetTypeList,
            self.RenderBetTypeList,
            self.CompletedLoadFilter,
            false);
    };
    self.RenderBetTypeList = function (rs) {
        var id = "dropdown-bettype";
        var betTypes = [];
        var betTypeLength = rs.length;
        for (var i = 0; i < betTypeLength; i++) {
            var betType = rs[i];
            betTypes.push({
                Id: id + betType.Id,
                Value: betType.Id,
                Text: betType.Name,
                Checked: true,
                Enable: true,
                CssClass: "",
                Items: []
            });
        }

        $("#dropdown-bettype").multiSelect(
        {
            minWidth: 150,
            maxHeight: 450,
            shadow: true,
            showHeader: true,
            isCheckAll: true,
            showFooter: false,
            checkAllText: viewResources.All_Bet_Types,
            isUsedCustomCheckAllText: true,
            customCheckAllText: viewResources.All_Bet_Types,
            noItemText: viewResources.Error_NoBetTypeFound,
            listData: betTypes
        });

        self.Tables.dropdownBettype = $("#dropdown-bettype");
        self.CachedData.isLoadedBetTypeList = true;
    };

    self.RenderDatePicker = function () {
        $('#datepickerplugin').nexDatePicker({
            nexDatePickerType: "single",
            firstDatePicker: {
                Id: "datePickerInput",
                Label: viewResources.Date,
                InitDate: today,
                LabelCss: "lbl-field col_2",
                TextboxCss: "edit-field col_6 width-auto"
            },

            datePickerFormat: {
                dateFormat: FormatConstant.M_DD_YY_STRING,
                changeMonth: true,
                changeYear: true,
                showOtherMonths: true,
                selectOtherMonths: true,
                minDate: new Date(minDateWinLossByBetType),
                maxDate: new Date(today)
            },

            onSelectDatePicker: function () {
                self.datePickerChange();
            }
        });

        self.Tables.datepickerplugin = $("#datepickerplugin");
        self.Tables.datepicker = $(".ui_nexdatepicker_fromdate");
        self.Tables.datePickerInput = $("#datePickerInput");
        self.CachedData.isLoadedDatePicker = true;
        self.CompletedLoadFilter();
    };

    self.CompletedLoadFilter = function () {
        if (self.CachedData.isLoadedCompanyList &&
            self.CachedData.isLoadedCurrencyList &&
            self.CachedData.isLoadedBetTypeList &&
            self.CachedData.isLoadedDatePicker) {
            AccReport.enableInput();
        }
    };
    self.dropdownReportByChange = function () {
        var reportBy = parseInt(self.Tables.dropdownReportBy.val());
        switch (reportBy) {
            case self.Enums.ReportBy.ByBetType:
                self.Tables.datePickerInput.datepicker("option", "minDate", new Date(minDateWinLossByBetType));
                self.Tables.datePickerInput.datepicker("option", "maxDate", new Date(today));
                self.Tables.winLossByMatchFilter.addClass("display-none");
                self.Tables.winLossByBetTypeFilter.removeClass("display-none");

                AccReport.popUp.hide();
                self.WinLossByMatch.HideReport();
                break;
            case self.Enums.ReportBy.ByMatch:
                self.Tables.datePickerInput.datepicker("option", "minDate", new Date(minDateWinLossByMatch));
                self.Tables.datePickerInput.datepicker("option", "maxDate", new Date(yesterday));
                self.Tables.winLossByBetTypeFilter.addClass("display-none");
                self.Tables.winLossByMatchFilter.removeClass("display-none");

                AccReport.popUp.hide();
                self.WinLossByBetType.HideReport();
                break;
        }

        self.CachedData.reportBy = reportBy;
    };

    self.GetParams = function () {
        var params = {};
        params.ReportDate = self.Tables.datepickerplugin.nexDatePicker("getSingleDate");
        params.CompanyList = self.Tables.dropdownCompany.multiSelect('getCheckedItems').toString();
        params.CurrencyList = self.Tables.dropdownCurrency.multiSelect('getCheckedItems').toString();
        params.TrialData = self.Tables.chkTrialData.is(":checked");

        switch (self.CachedData.reportBy) {
            case self.Enums.ReportBy.ByBetType:
                params.LiveStatus = self.Tables.dropdownLiveStatus.val();
                self.Params.winLossByBetType_MainReportParams = params;
                break;
            case self.Enums.ReportBy.ByMatch:
                params.BetTypeList = self.Tables.dropdownBettype.multiSelect('getCheckedItems').toString();
                self.Params.winLossByMatch_MainReportParams = params;
                break;
        }

        return params;
    };

    self.buttonGenerateClick = function () {
        AccReport.disableInput();
        var params = self.GetParams();
        switch (self.CachedData.reportBy) {
            case self.Enums.ReportBy.ByBetType:
                self.WinLossByBetType.GetMainReport(params);
                break;
            case self.Enums.ReportBy.ByMatch:
                self.WinLossByMatch.GetMainReport(params);
                break;
        }
    };

    self.GetColumnMargin = function (margin,
        turnover,
        columnName) {
        return {
            Name: columnName,
            Value: turnover != 0 ? margin : viewResources.NotAvailable
        };
    };

    self.datePickerChange = function () {
        self.GetCompanyList();
    };

    self.checkboxTrialDataChange = function () {
        AccReport.disableInput();
        self.GetCompanyList();
    };

};

var reportManager = new Fanex.AccountingReports.Gaming.NumberGameWinLossByBetTypeMatch.ReportManager();

