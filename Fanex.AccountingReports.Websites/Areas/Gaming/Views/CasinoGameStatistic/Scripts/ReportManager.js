$(document).ready(function () {
    reportManager.InitPage();
});

Namespace.Register("Fanex.AccountingReports.Gaming.LiveCasinoGameStatistic");
Fanex.AccountingReports.Gaming.LiveCasinoGameStatistic.ReportManager = function () {
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
            Company: 5,
            Currency: 6,
            UserNameDaily: 7,
            MonthlyToDaily: 8
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
            SLI: 4
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

    self.ReportByDailyRange = new Fanex.AccountingReports.Gaming.LiveCasinoGameStatistic.ReportByDailyRange();
    self.ReportDailyByCurrency = new Fanex.AccountingReports.Gaming.LiveCasinoGameStatistic.ReportDailyByCurrency();
    self.ReportByGrandTotal = new Fanex.AccountingReports.Gaming.LiveCasinoGameStatistic.ReportByGrandTotal();
    self.ReportByHourly = new Fanex.AccountingReports.Gaming.LiveCasinoGameStatistic.ReportByHourly();
    self.ReportByMonthlyRange = new Fanex.AccountingReports.Gaming.LiveCasinoGameStatistic.ReportByMonthlyRange();
    self.ReportMonthlyByCurrency = new Fanex.AccountingReports.Gaming.LiveCasinoGameStatistic.ReportMonthlyByCurrency();
    self.ReportByUserName = new Fanex.AccountingReports.Gaming.LiveCasinoGameStatistic.ReportByUserName();
    self.ReportByCompany = new Fanex.AccountingReports.Gaming.LiveCasinoGameStatistic.ReportByCompany();
    self.ReportCompanyByCurrency = new Fanex.AccountingReports.Gaming.LiveCasinoGameStatistic.ReportCompanyByCurrency();
    self.ReportByCurrency = new Fanex.AccountingReports.Gaming.LiveCasinoGameStatistic.ReportByCurrency();
    self.ReportByGameType = new Fanex.AccountingReports.Gaming.LiveCasinoGameStatistic.ReportByGameType();
    self.ReportGameTypeByCurrency = new Fanex.AccountingReports.Gaming.LiveCasinoGameStatistic.ReportGameTypeByCurrency();
    self.ReportByGameTable = new Fanex.AccountingReports.Gaming.LiveCasinoGameStatistic.ReportByGameTable();
    self.ReportGameTableByCurrency = new Fanex.AccountingReports.Gaming.LiveCasinoGameStatistic.ReportGameTableByCurrency();

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

        txtTotalBetLessThan: null,
        txtTotalBetMoreThan: null,
        txtTotalWinLessThan: null,
        txtTotalWinMoreThan: null,
        formatNumberonly: null,

        showRefundColumn: null,

        mainLiveCasinoGameStatisticReportContainer: null,
        mainLiveCasinoGameStatisticReport: null,
        hourlyReportContainer: null,
        hourlyReport: null,
        grandTotalContainer: null,
        grandTotal: null,
        dropdownProduct: null,
        dropdown3rdParty: null,
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
        isSelectOtherSLIProducts: false
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
            DataType: null,
            ShowRefundColumn: null,
            IsChangedValue: false,
            SortDirection: null,
            SortColumnName: null,
            SortDataType: null,
            ProductId: null,
            IsDrillDown: null,
            Product: null,
            EffectiveDate: null,
            IsExportGrandTotal: null,
            PrevReportType: null
        },

        globalVariable: {
            viewReport: {
                Daily: 0,
                DailyByCurrency: 1,
                DailyByHourly: 2,
                Monthly: 3,
                MonthlyByCurrency: 4,
                UserName: 5,
                DailyByUserName: 6,
                GameType: 7,
                GameTypeByCurrency: 8,
                GameTable: 9,
                GameTableByCurrency: 10,
                Company: 11,
                CompanyByCurrency: 12,
                Currency: 13,
                GrandTotal: 14,
                Hourly: 15
            },
            gridOptionReport: {
                Daily: null,
                DailyByCurrency: null,
                DailyByHourly: null,
                Monthly: null,
                MonthlyByCurrency: null,
                UserName: null,
                DailyByUserName: null,
                GameType: null,
                GameTypeByCurrency: null,
                GameTable: null,
                GameTableByCurrency: null,
                Company: null,
                CompanyByCurrency: null,
                Currency: null,
                GrandTotal: null,
                Hourly: null
            },
            CacheParam: {
                Daily: null,
                DailyByCurrency: null,
                DailyByHourly: null,
                Monthly: null,
                MonthlyByCurrency: null,
                UserName: null,
                DailyByUserName: null,
                GameType: null,
                GameTypeByCurrency: null,
                GameTable: null,
                GameTableByCurrency: null,
                Company: null,
                CompanyByCurrency: null,
                Currency: null,
                GrandTotal: null,
                Hourly: null,
                Params: null
            },
            CacheDataSource: {
                Daily: null,
                Monthly: null,
                UserName: null,
                GameType: null,
                GameTable: null,
                Company: null
            },
            CacheUserName: null,
            CurrentReportType: null,
            PrevReportType: null,
            IsDrillDown: false,
            DrillReportType: null
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
                LabelCss: "lbl-field col_3 prefix_2",
                OptionCss: "edit-field col_6"
            },
            firstDatePicker: {
                Id: "firstDatePickerId1",
                Label: viewResources.From_Date,
                InitDate: today,
                LabelCss: "lbl-field col_5 align-left",
                TextboxCss: "edit-field col_9"
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
                self.GetCompanyList();
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
        self.Tables.txtUserName = $("#txtUserName");
        self.Tables.excludeTestData = $("#exclude-test-data");
        self.Tables.reportBy = $("#reportBy");

        self.Tables.btnGenerate = $("#btnGenerate");
        self.Tables.btnToday = $("#btnToday");
        self.Tables.btnYesterday = $("#btnYesterday");

        self.Tables.txtTotalBetLessThan = $("#txt-totalbet-lessthan");
        self.Tables.txtTotalBetMoreThan = $("#txt-totalbet-morethan");
        self.Tables.txtTotalWinLessThan = $("#txt-totalwin-lessthan");
        self.Tables.txtTotalWinMoreThan = $("#txt-totalwin-morethan");
        self.Tables.formatNumberonly = $(".format-numberonly");
        self.Tables.showRefundColumn = $("#show-refund-column");

        self.Tables.mainLiveCasinoGameStatisticReportContainer = $("#mainLiveCasinoGameStatisticReportContainer");
        self.Tables.mainLiveCasinoGameStatisticReport = $("#mainLiveCasinoGameStatisticReport");
        self.Tables.hourlyReportContainer = $("#hourlyReportContainer");
        self.Tables.hourlyReport = $("#hourlyReport");
        self.Tables.grandTotalContainer = $("#grandTotalContainer");
        self.Tables.grandTotal = $("#grandTotal");
        self.Tables.dropdownProduct = $("#ddl-product");
        self.Tables.dropdown3rdParty = $("#ddl-3rd");
        self.Tables.gameTableContainer = $("#game-table-container");
        self.Tables.rdGameTable = $("#rdReportByGameTable");
        self.Tables.rdDaily = $("#rdReportByDaily");
        self.Tables.rdGameTableContainer = $("#rdGameTableContainer");
    };

    self.MakeUserNameSuggestion = function () {
        var self = this;
        self.CachedData.autoCompleteManager = self.Tables.txtUserName.autocomplete({
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
        self.GetCurrencyList();
    };

    self.GetProductList = function () {
        AccReport.utility.CallAjax(this,
            window.rootUrl + "Gaming/CasinoGameStatistic/GetProductList",
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
            var option = '<option value="' + product + '">' + product + '</option>';
            self.Tables.dropdownProduct.append(option);
        }
        self.Get3rdPartyByProduct();
        self.CachedData.isLoadedProductList = true;
    };

    self.Get3rdPartyByProduct = function () {
        var param = {
            product: self.Tables.dropdownProduct.val()
        };

        AccReport.utility.CallAjax(this,
            window.rootUrl + "Gaming/CasinoGameStatistic/Get3rdPartyByProduct",
            param,
            self.Render3rdPartyByProduct,
            self.Render3rdPartyByProduct,
            self.CompletedLoadFilter,
            false);
    };

    self.Render3rdPartyByProduct = function (result) {
        var productLength = result.length;
        self.Tables.dropdown3rdParty.empty();

        if (result && result.length > 0) {
            var optionLength = result.length;
            var ddwn3rdParty = [];
            var prefixId = 'ddl_3rdParty_';
            for (var i = 0; i < optionLength; i++) {
                var info = result[i];
                ddwn3rdParty.push({
                    Id: prefixId + info.ProductId,
                    Value: info.ProductId,
                    Text: info.ProductName,
                    Checked: true,
                    Enable: true,
                    CssClass: "",
                    Items: []
                });
            }

            this.Tables.dropdown3rdParty.multiSelect(
            {
                minWidth: 200,
                maxHeight: 450,
                shadow: true,
                showHeader: true,
                isCheckAll: true,
                showFooter: false,
                showTooltip: true,
                checkAllText: 'All',
                isUsedCustomCheckAllText: true,
                customCheckAllText: 'All',
                noItemText: 'No 3rd Party found',
                selectedList: 3,
                listData: ddwn3rdParty,
                onCheck: function () {
                    self.GetGameTypeList();
                    self.changeProduct();
                    self.GetCompanyList();
                }
            });

            this.Tables.dropdown3rdParty = $("#ddl-3rd");

            self.changeProduct();
            self.GetGameTypeList();
            self.GetCompanyList();
        }
    };

    self.GetCompanyList = function () {
        var dateRange = self.Tables.datepickerplugin.nexDatePicker("getDateRange");
        var param = {
            ProductId: self.Tables.dropdown3rdParty.multiSelect('getCheckedItems').toString(),
            FromDate: dateRange.FromDate,
            ToDate: dateRange.ToDate,
        };

        AccReport.utility.CallAjax(this,
            window.rootUrl + "Gaming/CasinoGameStatistic/GetLicenseeList",
            param,
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
        option = '<option affectivedate="Jan 01, 1900" value="0">' + viewResources.All + '</option>';
        self.Tables.dropdownCompany.append(option);

        for (var i = 0; i < companyLength; i++) {
            var company = rs.ChildInfo[i];
            if (company.Id == reportManager.Enums.LicenseeId.IBC) {
                option = '<option affectivedate="'+ company.EffectiveDate +'" value="' + company.Id + '">' + company.Name + '</option>';
                self.Tables.dropdownCompany.append(option);
                continue;
            }

            var licenseeLength = company.ChildInfo.length;
            for (var j = 0; j < licenseeLength; j++) {
                var licensee = company.ChildInfo[j];
                var option = '<option affectivedate="'+ licensee.EffectiveDate +'" value="' + licensee.Id + '">' + licensee.Name + '</option>';
                self.Tables.dropdownCompany.append(option);
            }
        }

        self.CachedData.isLoadedCompanyList = true;
    };

    self.CompletedLoadFilter = function () {
        AccReport.enableInput();
    };

    self.GetCurrencyList = function () {
        var isExcludeDataTest = self.Tables.excludeTestData.prop("checked");
        var params = { ExcludeTest: isExcludeDataTest };
        AccReport.utility.CallAjax(this,
            window.rootUrl + "Gaming/CasinoGameStatistic/GetCurrencyList",
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

    self.GetGameTypeList = function () {
        var productId = self.Tables.dropdown3rdParty.multiSelect('getCheckedItems');
        var params = {
            ProductId: productId.toString()
        };

        AccReport.utility.CallAjax(this,
            window.rootUrl + "Gaming/CasinoGameStatistic/GetProductGameType",
            params,
            self.RenderGameTypeList,
            self.RenderGameTypeList,
            self.CompletedLoadFilter,
            false);
    };
    self.RenderGameTypeList = function (products) {
        self.Tables.dropdownGameType = $("#dropdown-gametype");
        self.Tables.dropdownGameType.empty();

        var gameTypeParentList = [];
        var ddId = "dropdown-gametype";
        var productLength = products ? products.length : 0;
        for (var i = 0; i < productLength; i++) {
            var productGroup = products[i];
            var gameTypeLength = productGroup.GameTypes.length;
            var gameTypeList = [];
            for (var j = 0; j < gameTypeLength; j++) {
                var gameType = productGroup.GameTypes[j];

                gameTypeList.push({
                    Id: "#" + ddId + "_Item_" + productGroup.ProductId + "_" + gameType.Id,
                    Value: gameType.Id,
                    Text: gameType.Name,
                    Checked: true,
                    Enable: true
                });
            }

            gameTypeParentList.push({
                Id: "#" + ddId + "_group_" + productGroup.ProductId,
                Value: productGroup.ProductId,
                Text: productGroup.ProductName,
                Checked: true,
                Enable: true,
                Items: gameTypeList
            });
        }

        $("#" + ddId).multiSelect({
            minWidth: 200,
            maxHeight: 500,
            shadow: true,
            showHeader: true,
            isCheckAll: true,
            showFooter: false,
            showTooltip: true,
            checkAllText: 'All',
            isUsedCustomCheckAllText: true,
            customCheckAllText: 'All',
            noneSelectedText: 'Select options',
            noItemText: 'No Game type found.',
            listData: gameTypeParentList,
            onCheck: function () {
                self.changeGameType();
            }
        });

        this.Tables.dropdownGameType = $("#dropdown-gametype");
        self.CachedData.isLoadedGameTypeList = true;
        this.Tables.dropdownGameType.unbind("change");

        self.changeGameType();
    };

    self.GetGameTableList = function () {
        self.CachedData.isLoadedGameTableList = false;

        var gameTypeId = self.Tables.dropdownGameType.multiSelect('getCheckedItems').toString();
        if (gameTypeId == '') {
            return;
        }

        self.Params.getGameTableParams.GameTypeId = self.Tables.dropdownGameType.multiSelect('getCheckedItems').toString();
        self.Params.getGameTableParams.ExcludeTest = self.Tables.excludeTestData.prop("checked");
        AccReport.utility.CallAjax(this,
            window.rootUrl + "Gaming/CasinoGameStatistic/GetGameTableList",
            self.Params.getGameTableParams,
            self.RenderGameTableList,
            self.RenderGameTableList,
            AccReport.enableInput,
            false);
    };
    self.RenderGameTableList = function (result) {
        self.Tables.gameTableContainer.removeClass("display-none");
        var gameTableLength = result.length;
        self.Tables.dropdownGameTable = $("#dropdown-gametable");
        self.Tables.dropdownGameTable.empty();

        if (result && result.length > 0) {
            var optionLength = result.length;
            var ddwnGameTable = [];
            var prefixId = 'ddl_gametable_';
            for (var i = 0; i < optionLength; i++) {
                var info = result[i];
                ddwnGameTable.push({
                    Id: prefixId + info.Id,
                    Value: info.Id,
                    Text: info.Name,
                    Checked: true,
                    Enable: true,
                    CssClass: "",
                    Items: []
                });
            }

            this.Tables.dropdownGameTable.multiSelect(
            {
                minWidth: 127,
                maxHeight: 400,
                shadow: true,
                showHeader: true,
                isCheckAll: true,
                showFooter: false,
                showTooltip: true,
                checkAllText: 'All',
                isUsedCustomCheckAllText: true,
                customCheckAllText: 'All',
                noItemText: 'No Game table found',
                selectedList: 3,
                listData: ddwnGameTable
            });

            this.Tables.dropdownGameTable = $("#dropdown-gametable");
        }

        $('.tipsy-sw').hide();

        self.CachedData.isLoadedGameTableList = true;
    };

    self.BindingEvents = function () {
        this.Tables.showHideFilter.unbind("click");
        this.Tables.showHideFilter.click(function () {
            self.ShowHideFilter($(this).hasClass("expanded"));
        });

        self.BindingEvents_UserNameSuggestion();

        this.Tables.excludeTestData.unbind("click");
        this.Tables.excludeTestData.click(function () {
            self.excludeTestDataClick();
        });
        self.Tables.formatNumberonly.ForceDecimalOnly();

        this.Tables.btnGenerate.unbind("click");
        this.Tables.btnGenerate.click(function () {
            reportManager.Params.globalVariable.IsDrillDownFromUser = false;
            reportManager.Params.globalVariable.CacheReportType = "";
            self.GetReport(false, self.Tables.datepickerplugin.nexDatePicker("getSingleDate"));
        });

        this.Tables.btnToday.unbind("click");
        this.Tables.btnToday.click(function () {
            self.btnTodayClick();
        });

        this.Tables.btnYesterday.unbind("click");
        this.Tables.btnYesterday.click(function () {
            self.btnYesterdayClick();
        });

        this.Tables.dropdownProduct.change(function () {
            self.Get3rdPartyByProduct();
        });

        this.Tables.dateOptionSelector.change(function (){
            setTimeout(function () {
                    self.GetCompanyList();
            }, 200);
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

        var gridOptions = this.Tables.mainLiveCasinoGameStatisticReport.fanexGrid("getCurrentGridOptions");
        if (gridOptions) {
            if (isHideFilter) {
                gridOptions.gridExpandHeight = 150;
            } else {
                gridOptions.gridExpandHeight = 350;
            }

            this.Tables.mainLiveCasinoGameStatisticReport.fanexGrid("resizeGrid");
        }
    };
    self.BindingEvents_UserNameSuggestion = function () {
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
    };
    self.excludeTestDataClick = function () {
        AccReport.disableInput();
        self.CachedData.autoCompleteManager.setProperties({
            serviceUrl: window.rootWebPath.replace(/\/$/g, '') + '/ReportHelper/UserNameSuggestion?' +
                $.param(self.GetUserNameSuggestionParams()),
            minChars: 2
        });

        self.GetCurrencyList();
        if(self.Tables.dropdownProduct.val() == 'Live Casino'){
            self.GetGameTableList();
        }
        
        AccReport.enableInput();
    };

    self.btnTodayClick = function () {
        var self = this;
        self.SetDatePickerValue(this.Enums.DateOption.Today);
        self.GetReport(false, self.Tables.btnToday.val());
    };
    self.btnYesterdayClick = function () {
        var self = this;
        self.SetDatePickerValue(this.Enums.DateOption.Yesterday);
        self.GetReport(false, self.Tables.btnYesterday.val());
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
        var userName = self.Tables.txtUserName.val().trim();
        
        return {
            FromDate: dateRange.FromDate,
            ToDate: dateRange.ToDate,
            UserName: self.CachedData.isDefaultTextboxUserName ?
                  "" : userName,
            ExcludeTestData: this.Tables.excludeTestData.prop("checked") ? true : false,
            LicenseeId: self.Tables.dropdownCompany.val(),
            GameTypeId: self.Tables.dropdownGameType.multiSelect('getCheckedItems').toString(),
            GameTableId: self.Tables.dropdownGameTable.multiSelect('getCheckedItems').toString(),
            Currency: self.Tables.dropdownCurrency.val(),
            ReportType: self.Tables.reportBy.find("input[name='rdReportBy']:checked").val(),
            ViewReport: 0,
            ShowRefundColumn: self.Tables.showRefundColumn.prop("checked") ? true : false,

            TotalBetLess: self.Tables.txtTotalBetLessThan.getNumberValue(),
            TotalBetMore: self.Tables.txtTotalBetMoreThan.getNumberValue(),
            TotalWinLess: self.Tables.txtTotalWinLessThan.getNumberValue(),
            TotalWinMore: self.Tables.txtTotalWinMoreThan.getNumberValue(),
            SortDirection: reportManager.CachedData.sortDirection,
            SortColumnName: reportManager.CachedData.sortColName,
            SortDataType: reportManager.CachedData.sortDataType,
            ProductId: self.Tables.dropdown3rdParty.multiSelect('getCheckedItems').toString(),
            IsDrillDown: self.Params.globalVariable.IsDrillDown,
            IsExportGrandTotal: false,
            IsGrandRM: false,
            EffectiveDate: $("option:selected", self.Tables.dropdownCompany).attr("affectivedate"),
            Product: self.Tables.dropdownProduct.val(),
            PrevReportType: -1,
        };
    };

    self.GetReport = function () {
        var reportType = parseInt(self.Tables.reportBy.find("input[name='rdReportBy']:checked").val());
        self.Params.globalVariable.IsDrillDown = false;
        self.Params.globalVariable.DrillReportType = null;
        self.Params.globalVariable.CacheUserName = null;
        this.Tables.hourlyReportContainer.addClass("display-none");
        switch (reportType) {
            case self.Enums.ReportType.Daily:
                self.GetReportByDaily();
                break;
            case self.Enums.ReportType.Monthly:
                self.GetByMonthly();
                break;
            case self.Enums.ReportType.UserName:
                self.GetByUserName();
                break;
            case self.Enums.ReportType.GameType:
                self.GetReportByGameType();
                break;
            case self.Enums.ReportType.GameTable:
                self.GetReportByGameTable();
                break;
            case self.Enums.ReportType.Company:
                self.GetReportByCompany();
                break;
            case self.Enums.ReportType.Currency:
                self.GetReportByCurrency();
                break;
        }
    };

    self.GetReportByDaily = function (date, reportType) {
        var self = this;
        AccReport.disableInput();
        self.CachedData.sortDirection = null;
        self.CachedData.sortColName = null;
        self.CachedData.sortDataType = null;

        var selectParam = self.GetReportParams();
        selectParam.ViewReport = self.Params.globalVariable.viewReport.Daily;
        selectParam.PrevReportType = self.Params.globalVariable.CurrentReportType;
        self.Params.globalVariable.PrevReportType = self.Params.globalVariable.CurrentReportType;
        self.Params.globalVariable.CurrentReportType = self.Params.globalVariable.viewReport.Daily;

        if (!self.CheckReportParams(selectParam)) {
            AccReport.enableInput();
            return;
        }

        if (date != null) {
            var fromdate = date.split('|')[0];
            var todate = date.split('|')[1];
            selectParam.FromDate = fromdate;
            selectParam.ToDate = todate;
            selectParam.ReportType = reportType;
            self.Params.globalVariable.IsDrillDown = true;

            selectParam.TotalBetLess = "";
            selectParam.TotalBetMore = "";
            selectParam.TotalWinLess = "";
            selectParam.TotalWinMore = "";
        } else {
            self.Params.globalVariable.IsDrillDown = false;
        }

        selectParam.IsDrillDown = false;
        self.Params.globalVariable.CacheParam.Daily = selectParam;
        self.Params.globalVariable.DrillReportType = reportType;

        self.CachedData.currentPage = 0;
        self.Params.reportParams = selectParam;
        self.Params.globalVariable.CacheParam.Params = selectParam;
        AccReport.utility.CallAjax(this,
            window.rootUrl + "Gaming/CasinoGameStatistic/GetReportByDaily",
            self.Params.reportParams,
            self.RenderReport,
            self.RenderReport,
            AccReport.enableInput);
    };

    self.GetReportDailyByCurrency = function (date, currency) {
        var self = this;
        AccReport.disableInput();
        self.CachedData.sortDirection = null;
        self.CachedData.sortColName = null;
        self.CachedData.sortDataType = null;

        self.Params.globalVariable.IsDrillDown = false;
        var selectParam = self.GetReportParams();

        if (currency === "2" && selectParam.Currency === "-1") {
            currency = "-1";
            self.Params.globalVariable.IsDrillDown = true;

            selectParam.TotalBetLess = "";
            selectParam.TotalBetMore = "";
            selectParam.TotalWinLess = "";
            selectParam.TotalWinMore = "";
        }

        if (self.Params.globalVariable.CacheUserName != null) {
            selectParam.UserName = self.Params.globalVariable.CacheUserName;
            self.Params.globalVariable.IsDrillDown = true;

            selectParam.TotalBetLess = "";
            selectParam.TotalBetMore = "";
            selectParam.TotalWinLess = "";
            selectParam.TotalWinMore = "";
        }

        selectParam.FromDate = date;
        selectParam.ToDate = date;
        selectParam.Currency = currency;
        selectParam.IsDrillDown = self.Params.globalVariable.IsDrillDown;

        selectParam.ViewReport = self.Params.globalVariable.viewReport.DailyByCurrency;

        selectParam.PrevReportType = self.Params.globalVariable.CurrentReportType;
        self.Params.globalVariable.PrevReportType = self.Params.globalVariable.CurrentReportType;
        self.Params.globalVariable.CurrentReportType = self.Params.globalVariable.viewReport.DailyByCurrency;
        self.Params.globalVariable.CacheParam.DailyByCurrency = selectParam;

        if (!self.CheckReportParams(selectParam)) {
            AccReport.enableInput();
            return;
        }

        if (self.Params.globalVariable.DrillReportType != null) {
            selectParam.ReportType = self.Params.globalVariable.DrillReportType;
        }

        self.CachedData.currentPage = 0;
        self.Params.reportParams = selectParam;
        self.Params.globalVariable.CacheParam.Params = selectParam;
        AccReport.utility.CallAjax(this,
            window.rootUrl + "Gaming/CasinoGameStatistic/GetReportDailyByCurrency",
            self.Params.reportParams,
            self.RenderReport,
            self.RenderReport,
            AccReport.enableInput);
    };

    self.GetByMonthly = function () {
        var self = this;
        AccReport.disableInput();
        self.CachedData.sortDirection = null;
        self.CachedData.sortColName = null;
        self.CachedData.sortDataType = null;

        var selectParam = self.GetReportParams();

        selectParam.ViewReport = self.Params.globalVariable.viewReport.Monthly;

        self.Params.globalVariable.PrevReportType = null;
        self.Params.globalVariable.CurrentReportType = self.Params.globalVariable.viewReport.Monthly;

        if (!self.CheckReportParams(selectParam)) {
            AccReport.enableInput();
            return;
        }

        self.CachedData.currentPage = 0;
        self.Params.reportParams = selectParam;
        self.Params.globalVariable.CacheParam.Params = selectParam;
        AccReport.utility.CallAjax(this,
            window.rootUrl + "Gaming/CasinoGameStatistic/GetReportByMonthly",
            self.Params.reportParams,
            self.RenderReport,
            self.RenderReport,
            AccReport.enableInput);
    };

    self.GetByMonthlyByCurrency = function (date, currency) {
        var self = this;
        AccReport.disableInput();
        self.CachedData.sortDirection = null;
        self.CachedData.sortColName = null;
        self.CachedData.sortDataType = null;

        self.Params.globalVariable.IsDrillDown = false;
        var selectParam = self.GetReportParams();
        if (currency === "2" && selectParam.Currency === "-1") {
            currency = "-1";
            self.Params.globalVariable.IsDrillDown = true;

            selectParam.TotalBetLess = "";
            selectParam.TotalBetMore = "";
            selectParam.TotalWinLess = "";
            selectParam.TotalWinMore = "";
        }

        var fromdate = date.split('|')[0];
        var todate = date.split('|')[1];

        selectParam.FromDate = fromdate;
        selectParam.ToDate = todate;
        selectParam.Currency = currency;
        selectParam.IsDrillDown = self.Params.globalVariable.IsDrillDown;
        selectParam.ViewReport = self.Params.globalVariable.viewReport.MonthlyByCurrency;

        self.Params.globalVariable.PrevReportType = self.Params.globalVariable.CurrentReportType;
        self.Params.globalVariable.CurrentReportType = self.Params.globalVariable.viewReport.MonthlyByCurrency;
        self.Params.globalVariable.CacheParam.MonthlyByCurrency = selectParam;

        if (!self.CheckReportParams(selectParam)) {
            AccReport.enableInput();
            return;
        }

        self.CachedData.currentPage = 0;
        self.Params.reportParams = selectParam;
        self.Params.globalVariable.CacheParam.Params = selectParam;
        AccReport.utility.CallAjax(this,
            window.rootUrl + "Gaming/CasinoGameStatistic/GetReportMonthlyByCurrency",
            self.Params.reportParams,
            self.RenderReport,
            self.RenderReport,
            AccReport.enableInput);
    };

    self.GetByUserName = function () {
        var self = this;
        AccReport.disableInput();
        self.CachedData.sortDirection = null;
        self.CachedData.sortColName = null;
        self.CachedData.sortDataType = null;

        var selectParam = self.GetReportParams();

        selectParam.ViewReport = self.Params.globalVariable.viewReport.UserName;
        selectParam.IsDrillDown = true;

        self.Params.globalVariable.PrevReportType = self.Params.globalVariable.CurrentReportType;
        self.Params.globalVariable.CurrentReportType = self.Params.globalVariable.viewReport.UserName;
        self.Params.globalVariable.CacheParam.UserName = selectParam;

        if (!self.CheckReportParams(selectParam)) {
            AccReport.enableInput();
            return;
        }

        self.CachedData.currentPage = 0;
        self.Params.reportParams = selectParam;
        self.Params.globalVariable.CacheParam.Params = selectParam;
        AccReport.utility.CallAjax(this,
            window.rootUrl + "Gaming/CasinoGameStatistic/GetReportByUserName",
            self.Params.reportParams,
            self.RenderReport,
            self.RenderReport,
            AccReport.enableInput);
    };

    self.GetReportDailyByUserName = function (username, reportType) {
        var self = this;
        AccReport.disableInput();
        self.CachedData.sortDirection = null;
        self.CachedData.sortColName = null;
        self.CachedData.sortDataType = null;

        var selectParam = self.GetReportParams();
        selectParam.ViewReport = self.Params.globalVariable.viewReport.Daily;
        selectParam.PrevReportType = self.Params.globalVariable.CurrentReportType;
        self.Params.globalVariable.PrevReportType = self.Params.globalVariable.CurrentReportType;
        self.Params.globalVariable.CurrentReportType = self.Params.globalVariable.viewReport.Daily;

        if (!self.CheckReportParams(selectParam)) {
            AccReport.enableInput();
            return;
        }

        if (username != null) {
            selectParam.UserName = username;
            selectParam.ReportType = reportType;
            self.Params.globalVariable.IsDrillDown = true;
            self.Params.globalVariable.CacheUserName = username;

            selectParam.TotalBetLess = "";
            selectParam.TotalBetMore = "";
            selectParam.TotalWinLess = "";
            selectParam.TotalWinMore = "";
            selectParam.IsDrillDown = true;
        } else {
            self.Params.globalVariable.IsDrillDown = false;
            self.Params.globalVariable.CacheUserName = null;
            selectParam.IsDrillDown = false;
        }
        
        self.Params.globalVariable.CacheParam.Daily = selectParam;
        self.Params.globalVariable.DrillReportType = reportType;

        self.CachedData.currentPage = 0;
        self.Params.reportParams = selectParam;
        self.Params.globalVariable.CacheParam.Params = selectParam;
        AccReport.utility.CallAjax(this,
            window.rootUrl + "Gaming/CasinoGameStatistic/GetReportByDaily",
            self.Params.reportParams,
            self.RenderReport,
            self.RenderReport,
            AccReport.enableInput);
    };

    self.GetReportByGameType = function () {
        var self = this;
        AccReport.disableInput();
        self.CachedData.sortDirection = null;
        self.CachedData.sortColName = null;
        self.CachedData.sortDataType = null;

        var selectParam = self.GetReportParams();
        selectParam.ViewReport = self.Params.globalVariable.viewReport.GameType;
        self.Params.globalVariable.PrevReportType = self.Params.globalVariable.CurrentReportType;
        self.Params.globalVariable.CurrentReportType = self.Params.globalVariable.viewReport.GameType;

        if (!self.CheckReportParams(selectParam)) {
            AccReport.enableInput();
            return;
        }

        selectParam.IsDrillDown = self.Params.globalVariable.IsDrillDown;
        self.Params.globalVariable.CacheParam.GameType = selectParam;

        self.CachedData.currentPage = 0;
        self.Params.reportParams = selectParam;

        self.Params.globalVariable.CacheParam.Params = selectParam;
        AccReport.utility.CallAjax(this,
            window.rootUrl + "Gaming/CasinoGameStatistic/GetReportByGameType",
            self.Params.reportParams,
            self.RenderReport,
            self.RenderReport,
            AccReport.enableInput);
    };

    self.GetReportGameTypeCurrency = function (gameTypeId, currency) {
        var self = this;
        AccReport.disableInput();
        self.CachedData.sortDirection = null;
        self.CachedData.sortColName = null;
        self.CachedData.sortDataType = null;

        var selectParam = self.GetReportParams();

        if (currency === "2" && selectParam.Currency === "-1") {
            currency = "-1";
        }

        selectParam.GameTypeId = gameTypeId;
        selectParam.Currency = currency;

        selectParam.TotalBetLess = "";
        selectParam.TotalBetMore = "";
        selectParam.TotalWinLess = "";
        selectParam.TotalWinMore = "";

        selectParam.ViewReport = self.Params.globalVariable.viewReport.GameTypeByCurrency;
        self.Params.globalVariable.PrevReportType = self.Params.globalVariable.CurrentReportType;
        self.Params.globalVariable.CurrentReportType = self.Params.globalVariable.viewReport.GameTypeByCurrency;

        if (!self.CheckReportParams(selectParam)) {
            AccReport.enableInput();
            return;
        }

        selectParam.IsDrillDown = true;
        self.Params.globalVariable.CacheParam.GameTypeByCurrency = selectParam;

        self.CachedData.currentPage = 0;
        self.Params.reportParams = selectParam;
        self.Params.globalVariable.CacheParam.Params = selectParam;
        AccReport.utility.CallAjax(this,
            window.rootUrl + "Gaming/CasinoGameStatistic/GetReportGameTypeCurrency",
            self.Params.reportParams,
            self.RenderReport,
            self.RenderReport,
            AccReport.enableInput);
    };

    self.GetReportByGameTable = function () {
        var self = this;
        AccReport.disableInput();
        self.CachedData.sortDirection = null;
        self.CachedData.sortColName = null;
        self.CachedData.sortDataType = null;

        var selectParam = self.GetReportParams();
        selectParam.ViewReport = self.Params.globalVariable.viewReport.GameTable;
        self.Params.globalVariable.PrevReportType = self.Params.globalVariable.CurrentReportType;
        self.Params.globalVariable.CurrentReportType = self.Params.globalVariable.viewReport.GameTable;

        if (!self.CheckReportParams(selectParam)) {
            AccReport.enableInput();
            return;
        }

        selectParam.IsDrillDown = self.Params.globalVariable.IsDrillDown;
        self.Params.globalVariable.CacheParam.GameTable = selectParam;

        self.CachedData.currentPage = 0;
        self.Params.reportParams = selectParam;
        self.Params.globalVariable.CacheParam.Params = selectParam;
        AccReport.utility.CallAjax(this,
            window.rootUrl + "Gaming/CasinoGameStatistic/GetReportByGameTable",
            self.Params.reportParams,
            self.RenderReport,
            self.RenderReport,
            AccReport.enableInput);
    };

    self.GetReportGameTableByCurrency = function (gameTableId, currency) {
        var self = this;
        AccReport.disableInput();
        self.CachedData.sortDirection = null;
        self.CachedData.sortColName = null;
        self.CachedData.sortDataType = null;

        var selectParam = self.GetReportParams();

        if (currency === "2" && selectParam.Currency === "-1") {
            currency = "-1";
        }

        selectParam.GameTableId = gameTableId;
        selectParam.Currency = currency;
        
        selectParam.TotalBetLess = "";
        selectParam.TotalBetMore = "";
        selectParam.TotalWinLess = "";
        selectParam.TotalWinMore = "";

        selectParam.ViewReport = self.Params.globalVariable.viewReport.GameTableByCurrency;
        self.Params.globalVariable.PrevReportType = self.Params.globalVariable.CurrentReportType;
        self.Params.globalVariable.CurrentReportType = self.Params.globalVariable.viewReport.GameTableByCurrency;

        if (!self.CheckReportParams(selectParam)) {
            AccReport.enableInput();
            return;
        }

        selectParam.IsDrillDown = true;
        self.Params.globalVariable.CacheParam.GameTableByCurrency = selectParam;

        self.CachedData.currentPage = 0;
        self.Params.reportParams = selectParam;
        self.Params.globalVariable.CacheParam.Params = selectParam;
        AccReport.utility.CallAjax(this,
            window.rootUrl + "Gaming/CasinoGameStatistic/GetReportGameTableByCurrency",
            self.Params.reportParams,
            self.RenderReport,
            self.RenderReport,
            AccReport.enableInput);
    };

    self.GetReportByCompany = function () {
        var self = this;
        AccReport.disableInput();
        self.CachedData.sortDirection = null;
        self.CachedData.sortColName = null;
        self.CachedData.sortDataType = null;

        var selectParam = self.GetReportParams();
        selectParam.ViewReport = self.Params.globalVariable.viewReport.Company;
        self.Params.globalVariable.PrevReportType = self.Params.globalVariable.CurrentReportType;
        self.Params.globalVariable.CurrentReportType = self.Params.globalVariable.viewReport.Company;

        if (!self.CheckReportParams(selectParam)) {
            AccReport.enableInput();
            return;
        }

        selectParam.IsDrillDown = self.Params.globalVariable.IsDrillDown;
        self.Params.globalVariable.CacheParam.Company = selectParam;

        self.CachedData.currentPage = 0;
        self.Params.reportParams = selectParam;
        self.Params.globalVariable.CacheParam.Params = selectParam;
        self.Params.globalVariable.CacheParam.Params = selectParam;
        AccReport.utility.CallAjax(this,
            window.rootUrl + "Gaming/CasinoGameStatistic/GetReportByCompany",
            self.Params.reportParams,
            self.RenderReport,
            self.RenderReport,
            AccReport.enableInput);
    };

    self.GetReportCompanyByCurrency = function (siteId, currency) {
        var self = this;
        AccReport.disableInput();
        self.CachedData.sortDirection = null;
        self.CachedData.sortColName = null;
        self.CachedData.sortDataType = null;

        var selectParam = self.GetReportParams();

        if (currency === "2" && selectParam.Currency === "-1") {
            currency = "-1";
        }

        selectParam.LicenseeId = siteId;
        selectParam.Currency = currency;

        selectParam.ViewReport = self.Params.globalVariable.viewReport.CompanyByCurrency;
        self.Params.globalVariable.PrevReportType = self.Params.globalVariable.CurrentReportType;
        self.Params.globalVariable.CurrentReportType = self.Params.globalVariable.viewReport.CompanyByCurrency;

        if (!self.CheckReportParams(selectParam)) {
            AccReport.enableInput();
            return;
        }

        selectParam.IsDrillDown = true;
        self.Params.globalVariable.CacheParam.CompanyByCurrency = selectParam;

        selectParam.TotalBetLess = "";
        selectParam.TotalBetMore = "";
        selectParam.TotalWinLess = "";
        selectParam.TotalWinMore = "";

        self.CachedData.currentPage = 0;
        self.Params.reportParams = selectParam;
        self.Params.globalVariable.CacheParam.Params = selectParam;
        AccReport.utility.CallAjax(this,
            window.rootUrl + "Gaming/CasinoGameStatistic/GetReportCompanyByCurrency",
            self.Params.reportParams,
            self.RenderReport,
            self.RenderReport,
            AccReport.enableInput);
    };

    self.GetReportByCurrency = function () {
        var self = this;
        AccReport.disableInput();
        self.CachedData.sortDirection = null;
        self.CachedData.sortColName = null;
        self.CachedData.sortDataType = null;

        var selectParam = self.GetReportParams();
        selectParam.ViewReport = self.Params.globalVariable.viewReport.Currency;
        self.Params.globalVariable.PrevReportType = self.Params.globalVariable.CurrentReportType;
        self.Params.globalVariable.CurrentReportType = self.Params.globalVariable.viewReport.Currency;

        if (!self.CheckReportParams(selectParam)) {
            AccReport.enableInput();
            return;
        }

        selectParam.IsDrillDown = true;
        self.Params.globalVariable.CacheParam.Currency = selectParam;

        self.CachedData.currentPage = 0;
        self.Params.reportParams = selectParam;
        self.Params.globalVariable.CacheParam.Params = selectParam;
        AccReport.utility.CallAjax(this,
            window.rootUrl + "Gaming/CasinoGameStatistic/GetReportByCurrency",
            self.Params.reportParams,
            self.RenderReport,
            self.RenderReport,
            AccReport.enableInput);
    };

    self.CheckReportParams = function (selectParam) {
        AccReport.popUp.hide();

        if(isNaN(selectParam.TotalBetLess)){
            AccReport.popUp.show("Total bet less than: Invalid Number.", '');
            return false;
        }

        if(isNaN(selectParam.TotalBetMore)){
            AccReport.popUp.show("Total bet more than: Invalid Number.", '');
            return false;
        }

        if(isNaN(selectParam.TotalWinLess)){
            AccReport.popUp.show("Total win less than: Invalid Number.", '');
            return false;
        }

        if(isNaN(selectParam.TotalWinMore)){
            AccReport.popUp.show("Total win more than: Invalid Number.", '');
            return false;
        }

        if (selectParam.TotalBetLess.length > 0 &&
            selectParam.TotalBetMore.length > 0 &&
            parseFloat(selectParam.TotalBetLess) <= parseFloat(selectParam.TotalBetMore)) {
            AccReport.popUp.show(viewResources.Error_LiveCasinoGameStatistic_BetLess_LessThan_BetMore, '');
            return false;
        }

        if (selectParam.TotalWinLess.length > 0 &&
            selectParam.TotalWinMore.length > 0 &&
            parseFloat(selectParam.TotalWinLess) <= parseFloat(selectParam.TotalWinMore)) {
            AccReport.popUp.show(viewResources.Error_LiveCasinoGameStatistic_WinLess_LessThan_WinMore, '');
            return false;
        }

        var productId = self.Tables.dropdown3rdParty.multiSelect('getCheckedItems');
        if (productId == '') {
            AccReport.popUp.show("3rd Party - There is no selected item.", "");
            return false;
        }

        var gameTypeId = self.Tables.dropdownGameType.multiSelect('getCheckedItems');
        if (gameTypeId == '') {
            AccReport.popUp.show("Game type - There is no selected item.", "");
            return false;
        }

        var isSLIGameTypeSelected = self.isSLIGameTypeSelected();
        if (isSLIGameTypeSelected) {
            var gameTableIds = self.Tables.dropdownGameTable.multiSelect('getCheckedItems');
            if (gameTableIds == '') {
                AccReport.popUp.show(" Game table - There is no selected item.", "");
                return false;
            }
        } else {
            selectParam.GameTableId = '';
        }

        return true;
    };

    self.RenderReport = function (report) {
        var self = this;
        self.Tables.mainLiveCasinoGameStatisticReportContainer.removeClass("display-none");
        self.Tables.mainLiveCasinoGameStatisticReport.addClass("display-none");
        var viewReport = self.Params.globalVariable.viewReport;
        var gridOptionReport = self.Params.globalVariable.gridOptionReport;

        var gridOptions = null;
        var reportType = parseInt(self.Params.globalVariable.CurrentReportType);
        self.CachedData.currentReport = report;

        switch (reportType) {
            case viewReport.Daily:
                self.Params.globalVariable.CacheParam.Daily = self.Params.globalVariable.CacheParam.Params;
                gridOptions = self.ReportByDailyRange.GetGridOptions(report);
                gridOptionReport.Daily = gridOptions;
                self.Params.globalVariable.CacheDataSource.Daily = report;
                break;
            case viewReport.DailyByCurrency:
                self.Params.globalVariable.CacheParam.DailyByCurrency = self.Params.globalVariable.CacheParam.Params;
                gridOptions = self.ReportDailyByCurrency.GetGridOptions(report);
                gridOptionReport.DailyByCurrency = gridOptions;
                break;
            case viewReport.Monthly:
                self.Params.globalVariable.CacheParam.Monthly = self.Params.globalVariable.CacheParam.Params;
                gridOptions = self.ReportByMonthlyRange.GetGridOptions(report);
                gridOptionReport.Monthly = gridOptions;
                self.Params.globalVariable.CacheDataSource.Monthly = report;
                break;
            case viewReport.MonthlyByCurrency:
                self.Params.globalVariable.CacheParam.MonthlyByCurrency = self.Params.globalVariable.CacheParam.Params;
                gridOptions = self.ReportMonthlyByCurrency.GetGridOptions(report);
                gridOptionReport.MonthlyByCurrency = gridOptions;
                break;
            case viewReport.UserName:
                self.Params.globalVariable.CacheParam.UserName = self.Params.globalVariable.CacheParam.Params;
                gridOptions = self.ReportByUserName.GetGridOptions(report);
                gridOptionReport.UserName = gridOptions;
                self.Params.globalVariable.CacheDataSource.UserName = report;
                break;
            case viewReport.GameType:
                self.Params.globalVariable.CacheParam.GameType = self.Params.globalVariable.CacheParam.Params;
                gridOptions = self.ReportByGameType.GetGridOptions(report);
                gridOptionReport.GameType = gridOptions;
                self.Params.globalVariable.CacheDataSource.GameType = report;
                break;
            case viewReport.GameTypeByCurrency:
                self.Params.globalVariable.CacheParam.GameTypeByCurrency = self.Params.globalVariable.CacheParam.Params;
                gridOptions = self.ReportGameTypeByCurrency.GetGridOptions(report);
                gridOptionReport.GameTypeByCurrency = gridOptions;
                break;
            case viewReport.GameTable:
                self.Params.globalVariable.CacheParam.GameTable = self.Params.globalVariable.CacheParam.Params;
                gridOptions = self.ReportByGameTable.GetGridOptions(report);
                gridOptionReport.GameTable = gridOptions;
                self.Params.globalVariable.CacheDataSource.GameTable = report;
                break;
            case viewReport.GameTableByCurrency:
                self.Params.globalVariable.CacheParam.GameTableByCurrency = self.Params.globalVariable.CacheParam.Params;
                gridOptions = self.ReportGameTableByCurrency.GetGridOptions(report);
                gridOptionReport.GameTableByCurrency = gridOptions;
                break;
            case viewReport.Company:
                self.Params.globalVariable.CacheParam.Company = self.Params.globalVariable.CacheParam.Params;
                gridOptions = self.ReportByCompany.GetGridOptions(report);
                gridOptionReport.Company = gridOptions;
                break;
            case viewReport.CompanyByCurrency:
                self.Params.globalVariable.CacheParam.CompanyByCurrency = self.Params.globalVariable.CacheParam.Params;
                gridOptions = self.ReportCompanyByCurrency.GetGridOptions(report);
                gridOptionReport.CompanyByCurrency = gridOptions;
                break;
            case viewReport.Currency:
                self.Params.globalVariable.CacheParam.Currency = self.Params.globalVariable.CacheParam.Params;
                gridOptions = self.ReportByCurrency.GetGridOptions(report);
                gridOptionReport.Currency = gridOptions;
                break;
        }

        if (gridOptions != null) {
            this.Tables.mainLiveCasinoGameStatisticReport.removeClass("display-none").fanexGrid(gridOptions);
        }

        AccReport.enableInput();
    };

    self.RenderParentReport = function (reportType) {
        AccReport.disableInput();
        var gridOptions = [];

        var viewReport = self.Params.globalVariable.viewReport;
        var gridOptionReport = self.Params.globalVariable.gridOptionReport;

        switch (reportType) {
            case viewReport.Daily:
                self.Params.globalVariable.CurrentReportType = viewReport.Daily;
                self.Params.globalVariable.CacheParam.Params = self.Params.globalVariable.CacheParam.Daily;
                gridOptions = gridOptionReport.Daily;
                self.CachedData.currentReport = self.Params.globalVariable.CacheDataSource.Daily;
                break;
            case viewReport.Monthly:
                self.Params.globalVariable.CurrentReportType = viewReport.Monthly;
                self.Params.globalVariable.CacheParam.Params = self.Params.globalVariable.CacheParam.Monthly;
                gridOptions = gridOptionReport.Monthly;
                self.CachedData.currentReport = self.Params.globalVariable.CacheDataSource.Monthly;
                break;
            case viewReport.UserName:
                self.Params.globalVariable.CurrentReportType = viewReport.UserName;
                self.Params.globalVariable.CacheParam.Params = self.Params.globalVariable.CacheParam.UserName;
                gridOptions = gridOptionReport.UserName;
                self.CachedData.currentReport = self.Params.globalVariable.CacheDataSource.UserName;
                break;
            case viewReport.GameType:
                self.Params.globalVariable.CurrentReportType = viewReport.GameType;
                self.Params.globalVariable.CacheParam.Params = self.Params.globalVariable.CacheParam.GameType;
                gridOptions = gridOptionReport.GameType;
                self.CachedData.currentReport = self.Params.globalVariable.CacheDataSource.GameType;
                break;
            case viewReport.GameTable:
                self.Params.globalVariable.CurrentReportType = viewReport.GameTable;
                self.Params.globalVariable.CacheParam.Params = self.Params.globalVariable.CacheParam.GameTable;
                gridOptions = gridOptionReport.GameTable;
                self.CachedData.currentReport = self.Params.globalVariable.CacheDataSource.GameTable;
                break;
            case viewReport.Company:
                self.Params.globalVariable.CurrentReportType = viewReport.Company;
                self.Params.globalVariable.CacheParam.Params = self.Params.globalVariable.CacheParam.Company;
                gridOptions = gridOptionReport.Company;
                self.CachedData.currentReport = self.Params.globalVariable.CacheDataSource.Company;
                break;
        }

        self.Params.globalVariable.PrevReportType = self.Params.globalVariable.CacheParam.Params.PrevReportType;
        self.Params.globalVariable.IsDrillDown = self.Params.globalVariable.CacheParam.Params.IsDrillDown;

        this.Tables.mainLiveCasinoGameStatisticReport.removeClass("display-none").fanexGrid(gridOptions);
        this.Tables.grandTotalContainer.addClass("display-none");
        this.Tables.hourlyReportContainer.addClass("display-none");

        AccReport.enableInput();
    };

    self.GetHourlyReport = function (reportDate, currencyId) {
        var self = this;
        AccReport.disableInput();

        self.Params.globalVariable.PrevReportType = self.Params.globalVariable.CurrentReportType;
        self.Params.globalVariable.CurrentReportType = self.Params.globalVariable.viewReport.DailyByHourly;
        var selectParam = self.GetReportParams();

        if (currencyId === "2") {
            currencyId = "0";
        }

        selectParam.Currency = currencyId;
        selectParam.FromDate = reportDate;
        selectParam.ToDate = reportDate;
        selectParam.ViewReport = self.Params.globalVariable.viewReport.Hourly;
        if (self.Params.globalVariable.CacheUserName != null) {
            selectParam.UserName = self.Params.globalVariable.CacheUserName;
        }

        self.Params.reportParams = selectParam;
        self.Params.globalVariable.CacheParam.Hourly = selectParam;
        AccReport.utility.CallAjax(this,
            window.rootUrl + "Gaming/CasinoGameStatistic/GetReportByHourly",
            self.Params.reportParams,
            self.RenderHourlyReport,
            self.RenderHourlyReport,
            AccReport.enableInput);
    };

    self.RenderHourlyReport = function (report) {
        var self = this;
        this.Tables.mainLiveCasinoGameStatisticReportContainer.removeClass("display-none");
        this.Tables.mainLiveCasinoGameStatisticReport.removeClass("display-none");

        var gridOptions = self.ReportByHourly.GetGridOptions(report, true);

        if (gridOptions != null) {
            this.Tables.mainLiveCasinoGameStatisticReport.removeClass("display-none").fanexGrid(gridOptions);

            gridOptions = self.ReportByHourly.GetGridOptions(report, false);
            this.Tables.hourlyReportContainer.removeClass("display-none");
            this.Tables.hourlyReport.removeClass("display-none").fanexGrid(gridOptions);

        }
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

    self.getSortDirection = function (columnName) {
        return columnName == self.CachedData.sortColName ? self.CachedData.sortDirection : '';
    };

    self.SortReportData = function () {
        AccReport.disableInput();
        var reportType = parseInt(self.Params.globalVariable.CurrentReportType);
        var data = self.GetReportParams();
        data.ViewReport = reportType;

        AccReport.utility.CallAjax(this,
            window.rootUrl + "Gaming/CasinoGameStatistic/SortReport",
            data,
            self.RenderReport,
            self.RenderReport,
            AccReport.enableInput,
            false);
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
            case "ColId":
                sortInfo.sortColumnName = "Id";
                sortInfo.sortDataType = self.Enums.SortDataType.String;
                break;
            case "ColNumberOfPlayers":
                sortInfo.sortColumnName = "NumberOfPlayers";
                sortInfo.sortDataType = self.Enums.SortDataType.String;
                break;
            case "ColNumberOfGames":
                sortInfo.sortColumnName = "NumberOfGames";
                sortInfo.sortDataType = self.Enums.SortDataType.String;
                break;
            case "ColCurrency":
                sortInfo.sortColumnName = "CurrencyName";
                sortInfo.sortDataType = self.Enums.SortDataType.String;
                break;
            case "ColTotalBets":
                sortInfo.sortColumnName = "TotalBets";
                sortInfo.sortDataType = self.Enums.SortDataType.Number;
                break;
            case "ColTotalWins":
                sortInfo.sortColumnName = "TotalWins";
                sortInfo.sortDataType = self.Enums.SortDataType.Number;
                break;
            case "ColGameIncome":
                sortInfo.sortColumnName = "GameIncome";
                sortInfo.sortDataType = self.Enums.SortDataType.Number;
                break;
            case "ColGamePayout":
                sortInfo.sortColumnName = "GamePayout";
                sortInfo.sortDataType = self.Enums.SortDataType.Number;
                break;
            case "ColTotalRefund":
                sortInfo.sortColumnName = "TotalRefund";
                sortInfo.sortDataType = self.Enums.SortDataType.String;
                break;
            case "ColCompanyTurnover":
                sortInfo.sortColumnName = "CompanyTurnover";
                sortInfo.sortDataType = self.Enums.SortDataType.Number;
                break;
            case "CompanyCommission":
                sortInfo.sortColumnName = "CompanyCommission";
                sortInfo.sortDataType = self.Enums.SortDataType.Number;
                break;
            case "ColCompanyNetIncome":
                sortInfo.sortColumnName = "CompanyNetIncome";
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
            case "ColId":
                sortInfo.sortColumnName = "Id";
                sortInfo.sortDataType = self.Enums.SortDataType.Date;
                break;
            case "ColNumberOfPlayers":
                sortInfo.sortColumnName = "NumberOfPlayers";
                sortInfo.sortDataType = self.Enums.SortDataType.String;
                break;
            case "ColNumberOfGames":
                sortInfo.sortColumnName = "NumberOfGames";
                sortInfo.sortDataType = self.Enums.SortDataType.String;
                break;
            case "ColCurrency":
                sortInfo.sortColumnName = "CurrencyName";
                sortInfo.sortDataType = self.Enums.SortDataType.String;
                break;
            case "ColTotalBets":
                sortInfo.sortColumnName = "TotalBets";
                sortInfo.sortDataType = self.Enums.SortDataType.Number;
                break;
            case "ColTotalWins":
                sortInfo.sortColumnName = "TotalWins";
                sortInfo.sortDataType = self.Enums.SortDataType.Number;
                break;
            case "ColGameIncome":
                sortInfo.sortColumnName = "GameIncome";
                sortInfo.sortDataType = self.Enums.SortDataType.Number;
                break;
            case "ColGamePayout":
                sortInfo.sortColumnName = "GamePayout";
                sortInfo.sortDataType = self.Enums.SortDataType.Number;
                break;
            case "ColTotalRefund":
                sortInfo.sortColumnName = "TotalRefund";
                sortInfo.sortDataType = self.Enums.SortDataType.String;
                break;
            case "ColCompanyTurnover":
                sortInfo.sortColumnName = "CompanyTurnover";
                sortInfo.sortDataType = self.Enums.SortDataType.Number;
                break;
            case "CompanyCommission":
                sortInfo.sortColumnName = "CompanyCommission";
                sortInfo.sortDataType = self.Enums.SortDataType.Number;
                break;
            case "ColCompanyNetIncome":
                sortInfo.sortColumnName = "CompanyNetIncome";
                sortInfo.sortDataType = self.Enums.SortDataType.Number;
                break;
        }

        return sortInfo;
    };

    self.changeProduct = function () {
        var productId = self.Tables.dropdown3rdParty.multiSelect('getCheckedItems').toString();
        var isSLI = self.isSLI(productId);

        if (isSLI) {
            self.Tables.gameTableContainer.removeClass("display-none");
            self.CachedData.isSelectOtherSLIProducts = false;
            self.Tables.rdGameTableContainer.removeClass("display-none");

        }
        else {
            self.CachedData.isSelectOtherSLIProducts = true;
            self.Tables.gameTableContainer.addClass("display-none");
            self.Tables.rdGameTableContainer.addClass("display-none");
            if (self.Tables.rdGameTable.prop("checked")) {
                $('#rdReportByGameType').attr('checked', true);
            }
        }
    };

    self.changeGameType = function () {

        var isSLIGameTypeSelected = self.isSLIGameTypeSelected();
        if (isSLIGameTypeSelected) {
            self.Tables.gameTableContainer.removeClass("display-none");
            self.CachedData.isSelectOtherSLIProducts = false;
            self.Tables.rdGameTableContainer.removeClass("display-none");

            self.GetGameTableList();

        } else {
            self.CachedData.isSelectOtherSLIProducts = true;
            self.Tables.gameTableContainer.addClass("display-none");
            self.Tables.rdGameTableContainer.addClass("display-none");
            if (self.Tables.rdGameTable.prop("checked")) {
                $('#rdReportByGameType').attr('checked', true);
            }
        }
    };

    self.isSLIGameTypeSelected = function () {
        var gameTypes_SLI_Selected = [];
        $("input[id^='#dropdown-gametype_Item_4_']").each(function () {
            if (this.checked) {
                gameTypes_SLI_Selected.push(this.value);
            }
        });

        return gameTypes_SLI_Selected.length > 0;
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

    self.isSLI = function (productId) {
        var isSLI = productId.indexOf(self.Enums.Product.SLI) > -1;
        return isSLI;
    }

    self.RenderGrandTotal = function () {
        self.ReportByGrandTotal.initPage();
    }

};



var reportManager = new Fanex.AccountingReports.Gaming.LiveCasinoGameStatistic.ReportManager();
