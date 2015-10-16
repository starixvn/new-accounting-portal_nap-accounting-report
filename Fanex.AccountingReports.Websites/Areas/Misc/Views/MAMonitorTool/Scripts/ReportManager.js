$(document).ready(function () {
    reportManager.InitPage();
});

Namespace.Register("Fanex.AccountingReports.Misc.MAMonitorTool");
Fanex.AccountingReports.Misc.MAMonitorTool.ReportManager = function () {
    var self = this;

    self.Enums = {
        Product: {
            All: 0,
            Sportsbook: 1,
            Racing: 2,
            Casino: 3,
            NumberGame: 4,
            Bingo: 5,
            Poker: 6,
            LiveCasino: 7,
            VirtualSport: 8,
            Keno: 9,
            GoldDeluxe: 10,
            MetricGamingSuperLive: 11
        },

        LeagueType: {
            All: 0
        },

        Sport: {
            All: 0,
            Soccer: 1,
            HorseRacing: 151,
            NumberGame: 161,
            LiveCasino: 162
        },

        BetType: {
            P2PGames: 1000,
            Outright: 10,
            MixParlay: 9,
            SystemParlay: 29
        },

        ReportBy: {
            Daily: 0,
            Monthly: 1,
            Username: 2,
            BetType: 3,
            League: 4,
            Game: 5,
            Currency: 6,
            Sport: 7
        },

        LiveStatus: {
            All: -1
        }
    };

    self.Tables = {
        datepickerplugin: null,
        firstDatePickerId1: null,
        secondDatePickerId1: null,

        txtUsername: null,
        autocomplete: null,

        dropdownReportBy: null,
        productContainer: null,
        checkboxProduct: null,
        checkboxProductAll: null,
        dropdownSport: null,
        dropdownCurrency: null,
        dropdownBetType: null,
        leagueTypeContainer: null,
        checkboxSportsbook: null,

        liveStatusContainer: null,
        leagueTypeAll: null,

        textboxMinTurnover: null,
        textboxMaxTurnover: null,
        textboxMinWinLoss: null,
        textboxMaxWinLoss: null,
        numbericTextbox: null,
        radioLeagueType: null,
        radioLiveStatus: null,

        buttonViewHistoryReport: null,
        buttonViewCurrentReport: null,
        currentReportFilter: null,
        historyReportFilter: null,
        monthPicker: null,
        noteMessage: null,
        buttonReset: null,
        buttonGenerate: null,
        dropdownLeagueHeaderCheckBox: null,
        ddlProduct: null,
        dropdownProductHeaderCheckBox: null
    };

    self.CachedData = {
        ProductList: null,
        LeagueTypeList: null,
        ReportTypeList: null,
        CurrencyList: null,
        SportTypeList: null,

        autoCompleteManager: null,
        defaultTextboxUserName: viewResources.Search_All,
        currentTextboxUserName: viewResources.Search_All,
        isDefaultTextboxUserName: true,
        isOnAuto: false,

        isLoadedReportTypeList: false,
        isLoadedProductList: false,
        isLoadedSportList: false,
        isLoadedBetTypeList: false,
        isLoadedCurrencyList: false,
        isLoadedLeagueTypeList: false,
        isLoadedLeagueList: false,

        isHistoryReport: false,
        isChangeMonth: false,

        CssClass: {
            checkboxAll: 'checkbox-all',
            optionAll: 'option-all'
        },

        RoleIndex: {
            Member: 0,
            Agent: 1,
            Master: 2,
            Super: 3,
            Company: 4
        },

        currentPage: 0,
        itemsPerPage: 50,
        defaultItemsPerPage: 50,
        itemsPerPageArray: [50, 100, 200, 500]
    };

    self.Params = {
        getBetTypeList: {
            SportType: null,
            OptionInfoList: null
        },

        getLeagueList: {
            SportType: null,
            FromDate: null,
            ToDate: null,
            CurrencyId: null,
            BetTypeId: null,
            LiveStatus: null,
            LeagueTypeList: null,
            IsOnlyMetricGamingSuperLive: null
        },

        getReportParams: {
            ReportBy: null,
            FromDate: null,
            ToDate: null,
            Products: null,
            SportType: null,
            BetTypeId: null,
            CurrencyId: null,
            LiveStatus: null,
            Leagues: null,
            Usernames: null,
            MinTurnover: null,
            MaxTurnover: null,
            MinWinLoss: null,
            MaxWinLoss: null,
            IsPagingCallBack: null,
            StartIndex: null,
            UsersPerPage: null,
            IsExportExcel: null,
            IsShowSportBook: null
        }
    };

    self.PagingOption = {
        itemsPerPage: 50,
        currentPage: 0,
        startIndex: 0
    };

    self.NormalReport = new Fanex.AccountingReports.Misc.MAMonitorTool.NormalReport();
    self.HistoryReport = new Fanex.AccountingReports.Misc.MAMonitorTool.HistoryReport();

    self.InitPage = function () {
        AccReport.disableInput();
        self.RenderDatePicker();
        self.RenderMonthPicker();
        self.CachedDOMElements();
        self.renderFilterNote(noteMessage);
        self.MakeUserNameSuggestion();
        self.LoadFilterValues();
        self.BindingEvents();
        self.NormalReport.InitPage();
        self.HistoryReport.InitPage();
        self.Tables.historyReportFilter.addClass("display-none");
    };

    self.RenderDatePicker = function () {
        $('#datepickerplugin').nexDatePicker({
            nexDatePickerType: 'daterange',
            datePickerFormat: {
                changeMonth: true,
                changeYear: true,
                dateFormat: FormatConstant.M_DD_YY_STRING,
                minDate: new Date(balanceUpDay),
                maxDate: new Date(yesterday)
            },
            firstDatePicker: {
                Id: 'firstDatePickerId1',
                Label: viewResources.From_Date,
                InitDate: new Date(balanceUpDay),
                LabelCss: 'lbl-field col_3 bold currentreport-filter',
                TextboxCss: 'edit-field col_7 currentreport-filter'
            },
            secondDatePicker: {
                Id: 'secondDatePickerId1',
                Label: viewResources.To_Date,
                InitDate: new Date(yesterday),
                LabelCss: 'lbl-field col_4 bold currentreport-filter',
                TextboxCss: 'edit-field col_6 currentreport-filter'
            },
            onSelectDatePicker: function (selectedDate) {
                AccReport.disableInput();
                self.GetLeagueList();
            }
        });

        this.Tables.datepickerplugin = $("#datepickerplugin");
        this.Tables.firstDatePickerId1 = $("#firstDatePickerId1");
        this.Tables.secondDatePickerId1 = $("#secondDatePickerId1");
        this.Tables.firstDatePickerId1.datepicker("setDate", new Date(balanceUpDay));
    };
    self.renderFilterNote = function (noteMessage) {
        self.Tables.noteMessage.attr("title", noteMessage);
        self.Tables.noteMessage.tipsy({
            html: true,
            opacity: 0.9,
            gravity: 'ne'
        });
    };
    self.CachedDOMElements = function () {
        self.Tables.txtUsername = $("#txt-username");

        self.Tables.liveStatusContainer = $("#livestatus-container");
        self.Tables.radioLiveStatus = self.Tables.liveStatusContainer.find(".radio-livestatus");

        self.Tables.textboxMinTurnover = $("#textbox-minturnover");
        self.Tables.textboxMaxTurnover = $("#textbox-maxturnover");
        self.Tables.textboxMinWinLoss = $("#textbox-minwinloss");
        self.Tables.textboxMaxWinLoss = $("#textbox-maxwinloss");
        self.Tables.numbericTextbox = $(".numberic-textbox");

        self.Tables.buttonViewHistoryReport = $("#button-viewhistoryreport");
        self.Tables.buttonViewCurrentReport = $("#button-viewcurrentreport");
        self.Tables.currentReportFilter = $(".currentreport-filter");
        self.Tables.historyReportFilter = $(".historyreport-filter");
        self.Tables.noteMessage = $("#notemessage");

        self.Tables.buttonReset = $("#button-reset");
        self.Tables.buttonGenerate = $("#button-generate");
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
            excludeTestAccounts: false
        };
    };

    self.LoadFilterValues = function () {
        self.GetReportTypeList();
        self.GetProductList();
        self.GetSportList();
        self.GetCurrencyList();
        self.GetLeagueTypeList();
    };

    self.GetReportTypeList = function () {
        AccReport.utility.CallAjax(this,
            window.rootUrl + "Misc/MAMonitorTool/GetReportTypeList",
            null,
            self.RenderReportTypeList,
            self.RenderReportTypeList,
            self.CompletedLoadFilter,
            true);
    };

    self.RenderReportTypeList = function (rs) {
        self.Tables.dropdownReportBy = $("#dropdown-reportby");
        self.Tables.dropdownReportBy.empty();
        self.CachedData.ReportTypeList = rs;
        if (rs && rs.length) {
            var reportTypeLength = rs.length;
            for (var i = 0; i < reportTypeLength; i++) {
                var reportType = rs[i];
                var option = '<option value="' + reportType.Id + '">' + reportType.Name + '</option>';
                self.Tables.dropdownReportBy.append(option);
            }
        }

        self.CachedData.isLoadedReportTypeList = true;
    };

    self.GetProductList = function () {
        AccReport.utility.CallAjax(this,
            window.rootUrl + "Misc/MAMonitorTool/GetProductList",
            null,
            self.RenderProductList,
            self.RenderProductList,
            self.CompletedLoadFilter,
            true);
    };

    self.RenderProductList = function (rs) {
        self.Tables.productContainer = $("#productcontainer");
        self.CachedData.ProductList = rs;
        var noItemText = 'No products found.';
        var ddlProducts = [];
        var prefixId = 'dropdown-product-';
        var numberProducts = rs.length;

        for (var i = 0; i < numberProducts; i++) {
            var currentProduct = rs[i];
            ddlProducts.push({
                Id: prefixId + currentProduct.Id,
                Value: currentProduct.Id,
                Text: currentProduct.Name,
                Checked: true,
                Enable: true,
                CssClass: "",
                Items: []
            });
        }

        $("#dropdown-product").multiSelect(
        {
            minWidth: 266,
            maxHeight: 450,
            shadow: true,
            showHeader: true,
            isCheckAll: true,
            showFooter: false,
            showTooltip: false,
            checkAllText: 'All Products',
            isUsedCustomCheckAllText: true,
            customCheckAllText: 'All Products',
            noItemText: noItemText,
            selectedList: 3,
            listData: ddlProducts,
            onCheck: function () {
                self.checkboxProductClick();
            }
        });

        self.Tables.ddlProduct = $("#dropdown-product");
        self.Tables.checkboxSportsbook = $("#dropdown-product-" + self.Enums.Product.Sportsbook);
        self.Tables.checkboxMetricGamingSuperLive = $("#dropdown-product-" + self.Enums.Product.MetricGamingSuperLive);
        self.Tables.dropdownProductHeaderCheckBox = $("#dropdown-productHeaderCheckBox");
        self.CachedData.isLoadedProductList = true;
    };

    self.CompletedLoadFilter = function () {
        if (self.CachedData.isLoadedProductList &&
            self.CachedData.isLoadedSportList &&
            !self.CachedData.isLoadedBetTypeList) {
            self.GetBetTypeList();
        }

        if (self.CachedData.isLoadedReportTypeList &&
            self.CachedData.isLoadedProductList &&
            self.CachedData.isLoadedSportList &&
            self.CachedData.isLoadedBetTypeList &&
            self.CachedData.isLoadedCurrencyList &&
            self.CachedData.isLoadedLeagueTypeList &&
            !self.CachedData.isLoadedLeagueList) {
            self.GetLeagueList();
        }

        if (self.CachedData.isLoadedReportTypeList &&
            self.CachedData.isLoadedProductList &&
            self.CachedData.isLoadedSportList &&
            self.CachedData.isLoadedBetTypeList &&
            self.CachedData.isLoadedCurrencyList &&
            self.CachedData.isLoadedLeagueTypeList &&
            self.CachedData.isLoadedLeagueList) {
            AccReport.enableInput();
        }
    };

    self.BindingEvents = function () {
        self.BindingEvents_UsernameSuggestion();
        self.Tables.numbericTextbox.ForceDecimalOnly().ForceCurrencyOnly();
        self.Tables.radioLiveStatus.unbind("click");
        self.Tables.radioLiveStatus.click(function () {
            AccReport.disableInput();
            self.GetLeagueList();
        });

        self.Tables.buttonViewHistoryReport.unbind("click");
        self.Tables.buttonViewHistoryReport.click(function () {
            self.Tables.noteMessage.addClass("display-none");
            self.Tables.currentReportFilter.addClass("display-none");
            self.NormalReport.HideReport();
            self.Tables.historyReportFilter.removeClass("display-none");
            self.CachedData.isHistoryReport = true;
        });

        self.Tables.buttonViewCurrentReport.unbind("click");
        self.Tables.buttonViewCurrentReport.click(function () {
            self.Tables.historyReportFilter.addClass("display-none");
            self.HistoryReport.HideReport();
            self.Tables.currentReportFilter.removeClass("display-none");
            self.Tables.noteMessage.removeClass("display-none");
            self.CachedData.isHistoryReport = false;
        });

        self.Tables.buttonReset.unbind("click");
        self.Tables.buttonReset.click(function () {
            self.buttonResetClick();
        });

        self.Tables.buttonGenerate.unbind("click");
        self.Tables.buttonGenerate.click(function () {
            self.reset();
            self.buttonGenerateClick();
        });
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
    self.checkboxProductClick = function () {
        var isSelectedSportsbook = self.Tables.checkboxSportsbook.prop("checked");
        var isSelectedMetricGaming = self.Tables.checkboxMetricGamingSuperLive.prop("checked");
        if (isSelectedSportsbook) {
            self.Tables.dropdownSport.removeAttr("disabled");
            self.GetSportList();
            self.Tables.dropdownLeague.removeAttr("disabled");
            self.Tables.radioLeagueType.removeAttr("disabled");

            self.Tables.dropdownLeagueDisable.addClass("display-none");
            self.Tables.dropdownLeague.removeClass("display-none");
            self.RenderReportTypeList(self.CachedData.ReportTypeList);
        }
        else if (isSelectedMetricGaming && !isSelectedSportsbook) {
            self.Tables.dropdownSport.removeAttr("disabled");
            self.RenderMetricGamingSportList();
            self.Tables.dropdownLeague.removeAttr("disabled");
            self.Tables.radioLeagueType.removeAttr("disabled");

            self.Tables.dropdownLeagueDisable.addClass("display-none");
            self.Tables.dropdownLeague.removeClass("display-none");
            self.RenderReportTypeList(self.CachedData.ReportTypeList);
        }
        else {
            self.Tables.dropdownSport.attr("disabled", "disabled");
            self.Tables.radioLeagueType.attr("disabled", "disabled");

            self.Tables.dropdownLeague.addClass("display-none");
            self.Tables.dropdownLeagueDisable.removeClass("display-none");

            self.ShowHideReportByGameFilter();
            self.Tables.dropdownReportBy.find('option[value="' + self.Enums.ReportBy.League + '"]').remove();
            self.Tables.dropdownReportBy.find('option[value="' + self.Enums.ReportBy.Sport + '"]').remove();
        }

        self.reloadBetTypeList();
    };

    self.GetSportList = function () {
        AccReport.utility.CallAjax(this,
        window.rootUrl + "Misc/MAMonitorTool/GetSportList",
        null,
        self.RenderSportList,
        self.RenderSportList,
        self.CompletedLoadFilter,
        true);
    };

    self.RenderSportList = function (rs) {
        self.Tables.dropdownSport = $("#dropdown-sport");
        self.Tables.dropdownSport.empty();
        self.CachedData.SportTypeList = rs;
        if (rs && rs.length) {
            var sportLength = rs.length;
            self.Tables.dropdownSport.empty();
            for (var i = 0; i < sportLength; i++) {
                var sport = rs[i];
                var option = '<option value="' + sport.Id + '">' + sport.Name + '</option>';
                self.Tables.dropdownSport.append(option);
            }
        }

        self.Tables.dropdownSport.unbind("change");
        self.Tables.dropdownSport.change(function () {
            self.dropdownSportChange();
        });

        self.CachedData.isLoadedSportList = true;
    };

    self.RenderMetricGamingSportList = function () {
        self.Tables.dropdownSport = $("#dropdown-sport");
        self.Tables.dropdownSport.empty();
        var option = '<option value="' + self.Enums.Sport.Soccer + '">Soccer</option>';
        self.Tables.dropdownSport.append(option);
        self.CachedData.isLoadedSportList = true;
    };

    self.GetBetTypeList = function () {
        var products = self.Tables.dropdownProductHeaderCheckBox.prop("checked") ? "0" : self.Tables.ddlProduct.multiSelect("getCheckedItems").join(',');
        var validateParams = self.validateGetBettypeParams(products);
        if (validateParams) {
            AccReport.popUp.removeHighLight();
            var selectParams = {
                SportType: self.Tables.checkboxSportsbook.prop("checked") ?
                self.Tables.dropdownSport.val() :
                self.Enums.Sport.All,
                Products: products
            };

            self.Params.getBetTypeList = selectParams;
            AccReport.utility.CallAjax(this,
            window.rootUrl + "Misc/MAMonitorTool/GetBetTypeList",
            selectParams,
            self.RenderBetTypeList,
            self.RenderBetTypeList,
            self.CompletedLoadFilter,
            true);
        } else {
            AccReport.enableInput();
        }
    };
    self.RenderBetTypeList = function (rs) {
        self.Tables.dropdownBetType = $("#dropdown-bettype");
        self.Tables.dropdownBetType.empty();
        if (rs && rs.length > 0) {
            var productLength = rs.length;
            if (productLength > 1) {
                self.Tables.dropdownBetType.append("<option class='all-option' value='0'>All</option>");
                for (var i = 0; i < productLength; i++) {
                    var optionGroup = null;
                    var product = rs[i];
                    optionGroup = '<optgroup label="' + product.ProductName + '">';
                    var betTypeLength = product.Bettypes.length;
                    for (var j = 0; j < betTypeLength; j++) {
                        var betType = product.Bettypes[j];
                        optionGroup += '<option class="indent-option" value="' + betType.BettypeId + '">' + betType.BettypeName + '</option>';
                    }

                    optionGroup += "</optgroup>"
                    self.Tables.dropdownBetType.append(optionGroup);
                }
            }
            else {
                self.Tables.dropdownBetType.append("<option value='0'>All</option>");
                var betTypeLength = rs[0].Bettypes.length;
                for (var i = 0; i < betTypeLength; i++) {
                    var betType = rs[0].Bettypes[i];
                    var option = '<option value="' + betType.BettypeId + '">' + betType.BettypeName + '</option>';
                    self.Tables.dropdownBetType.append(option);
                }
            }
        }

        self.Tables.dropdownBetType.unbind("change");
        self.Tables.dropdownBetType.change(function () {
            self.dropdownBetTypeChange();
        });

        self.CachedData.isLoadedBetTypeList = true;
    };

    self.validateGetBettypeParams = function (products) {
        var isValid = true;
        if (products.length == 0) {
            isValid = false;
            AccReport.popUp.show("Please select one product!", "dropdown-product");
        }

        return isValid;
    };

    self.GetCurrencyList = function () {
        AccReport.utility.CallAjax(this,
            window.rootUrl + "Misc/MAMonitorTool/GetCurrencyList",
            null,
            self.RenderCurrencyList,
            self.RenderCurrencyList,
            self.CompletedLoadFilter,
            true);
    };
    self.RenderCurrencyList = function (rs) {
        self.Tables.dropdownCurrency = $("#dropdown-currency");
        self.CachedData.CurrencyList = rs;
        if (rs && rs.length) {
            var currencyLength = rs.length;
            self.Tables.dropdownCurrency.empty();
            for (var i = 0; i < currencyLength; i++) {
                var currency = rs[i];
                var option = '<option value="' + currency.Id + '">' + currency.Name + '</option>';
                self.Tables.dropdownCurrency.append(option);
            }
        }

        self.CachedData.isLoadedCurrencyList = true;
    };
    self.GetLeagueTypeList = function () {
        AccReport.utility.CallAjax(this,
            window.rootUrl + "Misc/MAMonitorTool/GetLeagueTypeList",
            null,
            self.RenderLeagueTypeList,
            self.RenderLeagueTypeList,
            self.CompletedLoadFilter,
            true);
    };

    self.RenderLeagueTypeList = function (rs) {
        self.Tables.leagueTypeContainer = $("#leaguetype-container");
        self.CachedData.LeagueTypeList = rs;
        if (rs && rs.length) {
            var leagueTypeLength = rs.length;
            self.Tables.leagueTypeContainer.empty();
            for (var i = 0; i < leagueTypeLength; i++) {
                var leagueType = rs[i];
                var option =
                '<div class="lbl-field width-auto align-left">\
                    <div class="leaguetype-selection' + (leagueType.Id == 0 ? ' leaguetype-selection-all' : '') + '">\
                        <input id="radio-leaguetype' + leagueType.Id + '" type="radio" name="radio-leaguetype" class="radio-leaguetype' +
                        (leagueType.Id == self.Enums.LeagueType.All ? ' ' + self.CachedData.CssClass.optionAll + '" checked="checked"' : '"') +
                        '/>' +
                        '<label for="radio-leaguetype' + leagueType.Id + '">' + leagueType.Name + '</label>\
                     </div>\
                 </div>';
                self.Tables.leagueTypeContainer.append(option);
            }
        }

        self.Tables.radioLeagueType = self.Tables.leagueTypeContainer.find(".radio-leaguetype");
        self.Tables.leagueTypeAll = self.Tables.leagueTypeContainer.find("." + self.CachedData.CssClass.optionAll);
        self.CachedData.isLoadedLeagueTypeList = true;

        self.Tables.radioLeagueType.unbind("click");
        self.Tables.radioLeagueType.click(function () {
            AccReport.disableInput();
            self.GetLeagueList();
        });
    };

    self.GetLeagueList = function () {
        var isOnlyMetricGamingSuperLive = false;
        if (!self.Tables.checkboxSportsbook.prop('checked') && !self.Tables.checkboxMetricGamingSuperLive.prop('checked')) {
            self.CachedData.isLoadedLeagueList = true;
            self.CompletedLoadFilter();
            return;
        }

        if (!self.Tables.checkboxSportsbook.prop('checked') && self.Tables.checkboxMetricGamingSuperLive.prop('checked')) {
            isOnlyMetricGamingSuperLive = true;
        }

        var dateRange;
        if (self.CachedData.isHistoryReport) {
            dateRange = {
                FromDate: self.Tables.monthPicker.monPickr("getFromDate", FormatConstant.MMM_DD_YY_STRING_MONTH_PICKER),
                ToDate: self.Tables.monthPicker.monPickr("getToDate", FormatConstant.MMM_DD_YY_STRING_MONTH_PICKER)
            };
        } else {
            dateRange = self.Tables.datepickerplugin.nexDatePicker("getDateRange");
        }

        var selectParams = {
            SportType: self.Tables.dropdownSport.val(),
            FromDate: dateRange.FromDate,
            ToDate: dateRange.ToDate,
            CurrencyId: self.Tables.dropdownCurrency.val(),
            BetTypeId: self.Tables.dropdownBetType.val(),
            LiveStatus: self.GetSelectedLiveStatus(),
            LeagueTypeList: self.GetSelectedLeagueType(),
            IsOnlyMetricGamingSuperLive: isOnlyMetricGamingSuperLive
        };

        self.Params.getLeagueList = selectParams;
        AccReport.utility.CallAjax(this,
            window.rootUrl + "Misc/MAMonitorTool/GetLeagueList",
            selectParams,
            self.RenderLeagueList,
            self.RenderLeagueList,
            self.CompletedLoadFilter,
            true);
    };

    self.RenderLeagueList = function (result) {
        self.Tables.dropdownLeague = $("#dropdown-league");
        self.Tables.dropdownLeague.empty();
        var ddwnLeagues = [];
        if (result && result.length > 0) {
            var optionLength = result.length;
            var prefixId = 'dropdown-league';
            for (var i = 0; i < optionLength; i++) {
                var info = result[i];
                ddwnLeagues.push({
                    Id: prefixId + info.Id,
                    Value: info.Id,
                    Text: info.Name,
                    Checked: true,
                    Enable: true,
                    CssClass: "",
                    Items: []
                });
            }
        }

        this.Tables.dropdownLeague.multiSelect(
        {
            minWidth: 266,
            maxHeight: 450,
            shadow: true,
            showHeader: true,
            isCheckAll: true,
            showFooter: false,
            showTooltip: false,
            checkAllText: viewResources.All_Leagues,
            isUsedCustomCheckAllText: true,
            customCheckAllText: viewResources.All_Leagues,
            noItemText: viewResources.No_league_found,
            selectedList: 3,
            listData: ddwnLeagues
        });

        self.Tables.dropdownLeague = $("#dropdown-league");
        self.Tables.dropdownLeagueDisable = $("#dropdown-league-disable");
        self.Tables.dropdownLeagueHeaderCheckBox = $("#dropdown-leagueHeaderCheckBox");
        self.CachedData.isLoadedLeagueList = true;
    };

    self.GetSelectedLiveStatus = function () {
        return self.Tables.liveStatusContainer.find(".radio-livestatus:checked").val();
    };
    self.GetSelectedLeagueType = function () {
        var leagueTypeList = self.CachedData.LeagueTypeList;
        var leagueTypeLength = leagueTypeList.length;
        var optionInfoList = [];
        var isAllLeagueType = self.Tables.leagueTypeAll.prop("checked");
        for (var i = 0; i < leagueTypeLength; i++) {
            var leagueType = leagueTypeList[i];
            if (leagueType.AliasName == "") {
                continue;
            }

            var isSelected = isAllLeagueType ||
                ($("#radio-leaguetype" + leagueType.Id).prop("checked") == true ? true : false);

            optionInfoList.push({
                Name: leagueType.AliasName,
                Value: isSelected
            });
        }

        return optionInfoList;
    };
    self.dropdownSportChange = function () {
        self.reloadBetTypeList();
    };
    self.reloadBetTypeList = function () {
        AccReport.disableInput();
        self.CachedData.isLoadedLeagueList = false;
        self.GetBetTypeList();
    };
    self.dropdownBetTypeChange = function () {
        AccReport.disableInput();
        self.GetLeagueList();
    };
    self.RenderMonthPicker = function () {
        $('#dropdown-month').monPickr({
            today: maxHistoryReportDate,
            maxDate: maxHistoryReportDate,
            minDate: minHistoryReportDate,
            showDateRange: true,
            fromDayId: 'ddlFromDay',
            toDayId: 'ddlToDay',
            sortDesc: true,
            plugInId: '#dropdown-month',
            log: function (msg) {
                console.warn(msg);
            },
            afterChangeDay: function () {
                if (self.CachedData.isChangeMonth) {
                    return;
                }

                AccReport.disableInput();
                self.GetLeagueList();
            },
            beforeChangeMonth: function () {
                self.CachedData.isChangeMonth = true;
            },
            afterChangeMonth: function () {
                AccReport.disableInput();
                self.GetLeagueList();
                self.CachedData.isChangeMonth = false;
            }
        });

        self.Tables.monthPicker = $('#dropdown-month');
    };
    self.buttonResetClick = function () {
        self.RenderReportTypeList(self.CachedData.ReportTypeList);
        self.Tables.firstDatePickerId1.datepicker("setDate", new Date(balanceUpDay));
        self.Tables.secondDatePickerId1.datepicker("setDate", new Date(today));

        self.Tables.ddlProduct.removeAttr("disabled");
        self.RenderProductList(self.CachedData.ProductList);
        self.Tables.dropdownSport.removeAttr("disabled");
        self.Tables.dropdownLeague.removeAttr("disabled");
        self.Tables.radioLeagueType.removeAttr("disabled");

        self.Tables.dropdownLeagueDisable.addClass("display-none");
        self.Tables.dropdownLeague.removeClass("display-none");
        self.RenderSportList(self.CachedData.SportTypeList);
        self.RenderCurrencyList(self.CachedData.CurrencyList);

        self.Tables.liveStatusContainer.find(".radio-livestatus[value='" + self.Enums.LiveStatus.All + "']")
                                       .attr("checked", "checked");
        self.Tables.leagueTypeContainer.find(".radio-livestatus[value='" + self.Enums.LeagueType.All + "']")
                                       .attr("checked", "checked");

        self.Tables.textboxMinTurnover.val("");
        self.Tables.textboxMaxTurnover.val("");
        self.Tables.textboxMinWinLoss.val("");
        self.Tables.textboxMaxWinLoss.val("");
        self.Tables.txtUsername.val("");
        self.Tables.monthPicker.monPickr("reset");
        self.reloadBetTypeList();
    };

    self.GetSelectReportParams = function () {
        var dateRange;
        if (self.CachedData.isHistoryReport) {
            dateRange = {
                FromDate: self.Tables.monthPicker.monPickr("getFromDate", FormatConstant.MMM_DD_YY_STRING_MONTH_PICKER),
                ToDate: self.Tables.monthPicker.monPickr("getToDate", FormatConstant.MMM_DD_YY_STRING_MONTH_PICKER)
            };
        } else {
            dateRange = self.Tables.datepickerplugin.nexDatePicker("getDateRange");
        }

        var hasSportsbook = self.Tables.checkboxSportsbook.prop("checked");

        return {
            ReportBy: self.Tables.dropdownReportBy.val(),
            FromDate: dateRange.FromDate,
            ToDate: dateRange.ToDate,
            Products: self.Tables.dropdownProductHeaderCheckBox.prop("checked") ? "0" : self.Tables.ddlProduct.multiSelect("getCheckedItems").join(','),
            SportType: hasSportsbook ?
                self.Tables.dropdownSport.val() :
                self.Enums.Sport.All,
            BetTypeId: self.Tables.dropdownBetType.val(),
            CurrencyId: self.Tables.dropdownCurrency.val(),
            LiveStatus: self.GetSelectedLiveStatus(),
            Leagues: self.Tables.dropdownLeagueHeaderCheckBox.prop("checked") && $('#radio-leaguetype0').prop("checked") ?
                '' :
                self.Tables.dropdownLeague.multiSelect("getCheckedItems").join(','),
            Usernames: self.CachedData.isDefaultTextboxUserName ?
                      "" :
                      self.Tables.txtUsername.val(),
            MinTurnover: self.Tables.textboxMinTurnover.val(),
            MaxTurnover: self.Tables.textboxMaxTurnover.val(),
            MinWinLoss: self.Tables.textboxMinWinLoss.val(),
            MaxWinLoss: self.Tables.textboxMaxWinLoss.val(),
            UsersPerPage: self.Params.getReportParams.UsersPerPage == null ?
                                    50 : self.Params.getReportParams.UsersPerPage,
            IsPagingCallBack: self.Params.getReportParams.IsPagingCallBack,
            StartIndex: self.Params.getReportParams.StartIndex,
            IsExportExcel: self.Params.getReportParams.IsExportExcel,
            IsHistoryReport: self.CachedData.isHistoryReport,
            IsShowSportBook: self.Tables.checkboxSportsbook.prop("checked")
        };
    };

    self.reset = function () {
        self.resetParams();
        self.resetPagingOption();
    };

    self.resetParams = function () {
        self.Params.getReportParams.ReportBy = null;
        self.Params.getReportParams.FromDate = null;
        self.Params.getReportParams.ToDate = null;
        self.Params.getReportParams.Products = null;
        self.Params.getReportParams.SportType = null;
        self.Params.getReportParams.BetTypeId = null;
        self.Params.getReportParams.CurrencyId = null;
        self.Params.getReportParams.LiveStatus = null;
        self.Params.getReportParams.Leagues = null;
        self.Params.getReportParams.Usernames = null;
        self.Params.getReportParams.MinTurnover = null;
        self.Params.getReportParams.MaxTurnover = null;
        self.Params.getReportParams.MinWinLoss = null;
        self.Params.getReportParams.MaxWinLoss = null;
        self.Params.getReportParams.IsPagingCallBack = null;
        self.Params.getReportParams.StartIndex = null;
        self.Params.getReportParams.UsersPerPage = null;
        self.Params.getReportParams.IsExportExcel = null;
        self.Params.getReportParams.IsShowSportBook = null;
    };

    self.resetPagingOption = function () {
        self.PagingOption.currentPage = 0;
        self.PagingOption.itemsPerPage = 50;
        self.PagingOption.startIndex = 0;
    };

    self.buttonGenerateClick = function () {
        AccReport.disableInput();

        var selectParams = self.GetSelectReportParams();

        self.Params.getReportParams = selectParams;

        if (!self.ValidateParams(selectParams)) {
            AccReport.enableInput();
            return;
        }

        AccReport.utility.CallAjax(this,
            window.rootUrl + "Misc/MAMonitorTool/GetReport",
            selectParams,
            self.RenderReport,
            self.RenderReport,
            AccReport.enableInput,
            false);
    };
    self.RenderReport = function (report) {
        if (self.CachedData.isHistoryReport) {
            self.HistoryReport.RenderReport(report);
        } else {
            self.NormalReport.RenderReport(report);
        }
    };

    self.GetDownlinesWinLoss = function (winLossInfo) {
        var roleIndex = reportManager.CachedData.RoleIndex;
        return {
            memberWinLoss: winLossInfo.ChildsInfo[roleIndex["Member"]],
            agentWinLoss: winLossInfo.ChildsInfo[roleIndex["Agent"]],
            masterWinLoss: winLossInfo.ChildsInfo[roleIndex["Master"]],
            superWinLoss: winLossInfo.ChildsInfo[roleIndex["Super"]]
        };
    };
    self.ValidateParams = function (selectParams) {
        AccReport.popUp.removeHighLight();
        if (selectParams.MaxTurnover.length > 0 &&
            selectParams.MinTurnover.length == 0) {
            AccReport.popUp.show(viewResources.Error_MAMonitorTool_InputTurnoverRange,
                "textbox-minturnover");
            return false;
        }

        if (selectParams.MinTurnover.length > 0 &&
            selectParams.MaxTurnover.length == 0) {
            AccReport.popUp.show(viewResources.Error_MAMonitorTool_InputTurnoverRange,
                "textbox-maxturnover");
            return false;
        }

        var find = ',';
        var replace = new RegExp(find, 'g');
        var minTurnover = parseInt(selectParams.MinTurnover.replace(replace, ''));
        var maxTurnover = parseInt(selectParams.MaxTurnover.replace(replace, ''));
        var minWinLoss = parseInt(selectParams.MinWinLoss.replace(replace, ''));
        var maxWinLoss = parseInt(selectParams.MaxWinLoss.replace(replace, ''));

        if (selectParams.MinTurnover.length > 0 &&
            selectParams.MaxTurnover.length > 0 &&
            minTurnover > maxTurnover) {
            AccReport.popUp.show(viewResources.Error_MAMonitorTool_InputMinMaxTurnoverRange,
                "textbox-minturnover");
            return false;
        }

        if (selectParams.MaxWinLoss.length > 0 &&
            selectParams.MinWinLoss.length == 0) {
            AccReport.popUp.show(viewResources.Error_MAMonitorTool_InputWinLossRange,
                "textbox-minwinloss");
            return false;
        }

        if (selectParams.MinWinLoss.length > 0 &&
            selectParams.MaxWinLoss.length == 0) {
            AccReport.popUp.show(viewResources.Error_MAMonitorTool_InputWinLossRange,
                "textbox-maxwinloss");
            return false;
        }

        if (selectParams.MinWinLoss.length > 0 &&
            selectParams.MaxWinLoss.length > 0 &&
            minWinLoss > maxWinLoss) {
            AccReport.popUp.show(viewResources.Error_MAMonitorTool_InputMinMaxWinLossRange,
                "textbox-minwinloss");
            return false;
        }

        if (selectParams.Usernames.length > 4000) {
            AccReport.popUp.show(viewResources.Error_MAMonitorTool_WrongUsername,
                "txt-username");
            return false;
        }

        if (!self.Tables.dropdownLeagueHeaderCheckBox.prop("checked") &&
            selectParams.Leagues.length == 0) {
            AccReport.popUp.show(viewResources.Error_MAMonitorTool_InputLeague,
                "dropdown-league,dropdown-league-disable");
            return false;
        }

        if (!self.Tables.dropdownProductHeaderCheckBox.prop("checked") &&
            selectParams.Products.length == 0) {
            AccReport.popUp.show("Please select one product!", "dropdown-product");
            return false;
        }

        return true;
    };
    self.ShowHideReportByGameFilter = function () {
        var isShow = self.Tables.productContainer.find("#dropdown-product-" + self.Enums.Product.Sportsbook).prop("checked") ||
            self.Tables.productContainer.find("#dropdown-product-" + self.Enums.Product.Racing).prop("checked") ||
            self.Tables.productContainer.find("#dropdown-product-" + self.Enums.Product.NumberGame).prop("checked") ||
            self.Tables.productContainer.find("#dropdown-product-" + self.Enums.Product.Poker).prop("checked") ||
            self.Tables.productContainer.find("#dropdown-product-" + self.Enums.Product.Keno).prop("checked") ||
            self.Tables.productContainer.find("#dropdown-product-" + self.Enums.Product.LiveCasino).prop("checked");
        if (isShow) {
            self.RenderReportTypeList(self.CachedData.ReportTypeList);
        } else {
            self.Tables.dropdownReportBy.find('option[value="' + self.Enums.ReportBy.Game + '"]').remove();
        }
    };

    self.IsShowSportbook = function () {
        self.Tables.productContainer = $("#productcontainer");
        var isShowSportbook = false;
        isShowSportbook = self.Tables.productContainer.find("#dropdown-product-" + self.Enums.Product.Sportsbook).prop("checked");
        return isShowSportbook;
    };
};

var reportManager = new Fanex.AccountingReports.Misc.MAMonitorTool.ReportManager();
