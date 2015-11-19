$(document).ready(function () {
    reportManager.InitPage();
});

ReportManager = function () {
    var self = this;

    self.Enum = {
        PeriodType: {
            Daily: 0,
            Weekly: 1,
            Monthly: 2,
            Annualy: 3,
            MonthRange: 4
        },
        SearchByType: {
            User: 0,
            League: 1
        },

        KeyCode: {
            HTab: 9,
            Number0: 48,
            Number9: 57,
            Backspace: 8,
            Return: 13,
            Null: 0,
            Dot: 46
        },

        Sport: {
            KickOff: 18999
        }
    };

    self.TopPunterByUser = new TopPunterByUser();
    self.TopPunterBetCountByUser = new TopPunterBetCountByUser();
    self.TopPunterIPByUser = new TopPunterIPByUser();
    self.TopPunter_ByUser_SportType = new TopPunter_ByUser_SportType();
    self.TopPunterBetCount_ByUser_SportType = new TopPunterBetCount_ByUser_SportType();
    self.TopPunterBetCount_ByUser_BetType = new TopPunterBetCount_ByUser_BetType();
    self.TopPunter_ByUser_BetType = new TopPunter_ByUser_BetType();
    self.TopPunter_ByLeague = new TopPunter_ByLeague();
    self.Tables = {
        btnGenerate: null,
        dailyCalendarContainer: null,
        dailyCalendar: null,
        txtTopRows: null,
        ddlOrderType: null,
        ddlOrderDirection: null,

        ddlPeriodType: null,
        ddlProduct: null,
        ddlSport: null,
        rdSearchBy: null,
        filterByUser: null,
        filterByLeague: null,

        weeklyCalendarContainer: null,
        fromWeek: null,
        toWeek: null,

        monthlyCalendarContainer: null,
        annualyCalendarContainer: null,
        monthRangeCalendarContainer: null,
        ddlLeague_ByLeague: null,
        ddlBetType_ByLeague: null,

        reportContainer: null,
        report: null
    };
    self.Params = {
        getProducts: {
            IsSB: false
        },
        getSports: {
            IsSB: false
        },
        getLeaguesBySport: {
            FromDate: null,
            ToDate: null,
            Sporttype: null
        },
        getBetTypesBySport: {
            Sporttype: null
        },

        reportParams: {
            UserName: null,
            Fromdate: null,
            Todate: null,
            Sportbettype: null,
            TopRow: null,
            OrderBy: null,
            OrderType: null,
            PeriodType: null,
            Reporttype: null,
            OrderByName: '',
            OrderTypeName: ''
        }
    };

    self.MonthNames = ['January', 'February', 'March', 'April',
                       'May', 'June', 'July', 'August',
                       'September', 'October', 'November', 'December'];

    self.CacheData = {
        DefaultTopRows: 10,
        DefaultUsername: 'All',
        SearchBy: 0,
        CurrentDatePickerContainer: null,
        Sportbook_ProductId: "1_0",
        Sports: [],
        SearchByType: self.Enum.SearchByType.User,
        SelectedSportBetTypes: []
    };
};
ReportManager.prototype.InitPage = function () {
    //    this.RenderDatePicker();
    this.GetProducts();
    this.GetSports();
    this.CachedDOMElements();

    this.RenderFilterNote();
    this.RenderDailyCalendar();
    this.RenderWeeklyCalendar();
    this.RenderMonthlyCalendar();
    this.RenderAnnualyCalendar();
    this.RenderMonthRangeCalendar();
    this.BindingEvent();

    this.Tables.txtTopRows.val(this.CacheData.DefaultTopRows);
    this.Tables.txtUserName.val(this.CacheData.DefaultUsername);
    this.CacheData.CurrentDatePickerContainer = this.Tables.dailyCalendarContainer;

    this.TopPunterByUser.InitPage();
    this.TopPunterBetCountByUser.InitPage();
    this.TopPunterIPByUser.InitPage();
    this.TopPunter_ByUser_SportType.InitPage();
    this.TopPunterBetCount_ByUser_SportType.InitPage();
    this.TopPunterBetCount_ByUser_BetType.InitPage();
    this.TopPunter_ByUser_BetType.InitPage();
    this.TopPunter_ByLeague.InitPage();
};
ReportManager.prototype.CachedDOMElements = function () {
    this.Tables.btnGenerate = $("#btnGenerate");
    this.Tables.filterNote = $("#notemessage");

    this.Tables.txtTopRows = $("#txtTopRows");
    this.Tables.ddlOrderType = $("#ddlOrderType");
    this.Tables.ddlOrderDirection = $("#ddlOrderDirection");

    this.Tables.txtUserName = $("#txtUserName");
    this.Tables.ddlPeriodType = $("#ddlPeriodType");
    this.Tables.rdSearchBy = $(".rdSearchBy");

    this.Tables.filterByUser = $("#filterByUser");
    this.Tables.filterByLeague = $("#filterByLeague");

    this.Tables.reportContainer = $('#topPunterReportContainer');
    this.Tables.report = $('#topPunterReport');

    this.Tables.reportByLeagueContainer = $('#topPunterReportByLeagueContainer');
    this.Tables.reportByLeague = $('#topPunterReportByLeague');
};
ReportManager.prototype.RenderFilterNote = function () {
    this.Tables.filterNote.attr("title", noteMessage);
    this.Tables.filterNote.tipsy({
        html: true,
        opacity: 0.9,
        gravity: 'ne'
    });
};
ReportManager.prototype.BindingEvent = function () {
    var self = this;

    self.Tables.txtTopRows.unbind("blur");
    self.Tables.txtTopRows.blur(function () {
        if (self.Tables.txtTopRows.val() == '' || isNaN(self.Tables.txtTopRows.val())) {
            self.Tables.txtTopRows.val(self.CacheData.DefaultTopRows);
        }
    });

    self.Tables.txtTopRows.unbind("keypress");
    self.Tables.txtTopRows.keypress(function (e) {
        var code = "";
        if (e.keyCode) {
            code = e.keyCode;
        }
        else if (e.which) {
            code = e.which;
        }

        if (code == reportManager.Enum.KeyCode.HTab) {
            return code;
        }

        if ((code < reportManager.Enum.KeyCode.Number0 ||
             code > reportManager.Enum.KeyCode.Number9) &&
            code != reportManager.Enum.KeyCode.Backspace &&
            code != reportManager.Enum.KeyCode.Return &&
            code != reportManager.Enum.KeyCode.Null &&
            code != reportManager.Enum.KeyCode.Dot) {
            return false;
        }

        return true;
    });

    self.Tables.txtUserName.unbind("blur");
    self.Tables.txtUserName.blur(function () {
        var username = self.Tables.txtUserName.val();
        if (username.length == 0) {
            self.Tables.txtUserName.val(self.CacheData.DefaultUsername);
        }
    });

    self.Tables.txtUserName.unbind("click");
    self.Tables.txtUserName.click(function () {
        var username = self.Tables.txtUserName.val();
        if (username == self.CacheData.DefaultUsername) {
            self.Tables.txtUserName.val('');
        }
    });

    self.Tables.ddlPeriodType.unbind("change");
    self.Tables.ddlPeriodType.change(function () {
        self.ChangeCalendar();
    });

    self.Tables.rdSearchBy.unbind("change");
    self.Tables.rdSearchBy.change(function () {
        self.ChangeSearchBy($(this).val());
    });

    self.Tables.ddlSport_ByLeague.unbind("change");
    self.Tables.ddlSport_ByLeague.change(function () {
        self.GetLeaguesBySport();
        self.GetBetTypesBySport();
    });

    self.BindingEvent_ByUser();
};
ReportManager.prototype.BindingEvent_ByUser = function () {
    var self = this;
    self.Tables.btnGenerate.unbind("click");
    self.Tables.btnGenerate.click(function () {
        self.GetReport();
    });
};
ReportManager.prototype.BindingEvent_ByLeague = function () {
    var self = this;
    self.Tables.btnGenerate.unbind("click");
    self.Tables.btnGenerate.click(function () {
        self.GetReport_ByLeague();
    });
};
ReportManager.prototype.GetProducts = function () {
    var self = this;

    AccReport.disableInput();
    $.ajax({
        type: "POST",
        url: window.rootUrl + "PunterAnalysis/GetProducts",
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        async: false,
        data: JSON.stringify(self.Params.getProducts),
        success: function (rs) {
            self.RenderDropdownProducts(rs);
        },
        error: function (event, jqXHR, ajaxSettings) {
            self.RenderDropdownProducts(null);
        },
        complete: function () {
            AccReport.enableInput();
        }
    });
};
ReportManager.prototype.RenderDropdownProducts = function (products) {
    var self = this;
    var ddProduct = [];
    var ddId = "ddlProduct";
    var productLength = products ? products.length : 0;
    for (var i = 0; i < productLength; i++) {
        var productGroup = products[i];
        if (productGroup.Name.toLowerCase() == "sportsbook") {
            ddProduct.push({
                Id: "#" + ddId + "_Product1",
                Value: "1_0",
                Text: productGroup.Name,
                Checked: true,
                Enable: true,
                CssClass: "bold"
            });

            continue;
        }

        var sportLength = productGroup.ChildsInfo.length;
        for (var j = 0; j < sportLength; j++) {
            var sportGroup = productGroup.ChildsInfo[j];
            var betTypeLength = sportGroup.ChildsInfo.length;
            var betTypeList = [];

            for (var k = 0; k < betTypeLength; k++) {
                var betType = sportGroup.ChildsInfo[k];
                betTypeList.push({
                    Id: "#" + ddId + "_Product_Sport" + sportGroup.Id + "_BetType" + betType.Id,
                    Value: sportGroup.Id + "_" + betType.Id,
                    Text: betType.Name,
                    Checked: true,
                    Enable: true
                });
            }

            ddProduct.push({
                Id: "#" + ddId + "_Product_Sport" + sportGroup.Id,
                Value: sportGroup.Id,
                Text: productGroup.Name,
                Checked: true,
                Enable: true,
                Items: betTypeList
            });
        }
    }

    $("#" + ddId).multiSelect({
        minWidth: 190,
        maxHeight: 350,
        shadow: true,
        showHeader: true,
        isCheckAll: true,
        showFooter: false,
        checkAllText: 'All',
        isUsedCustomCheckAllText: true,
        customCheckAllText: 'All',
        noneSelectedText: 'Select an option',
        noItemText: 'No product found.',
        listData: ddProduct,
        onCheck: function () {
            //if sportsbook seleted then load sports
            if (this.value == reportManager.CacheData.Sportbook_ProductId ||
                this.id == 'ddlProductHeaderCheckBox') {
                self.GetSports();
            }
        }
    });

    this.Tables.ddlProduct = $("#ddlProduct");
};
ReportManager.prototype.ChangeCalendar = function () {
    var periodType = parseInt(this.Tables.ddlPeriodType.val());
    this.CacheData.CurrentDatePickerContainer.addClass("display-none");

    switch (periodType) {
        case reportManager.Enum.PeriodType.Daily:
            this.CacheData.CurrentDatePickerContainer = this.Tables.dailyCalendarContainer;
            break;
        case reportManager.Enum.PeriodType.Weekly:
            this.CacheData.CurrentDatePickerContainer = this.Tables.weekyCalendarContainer;
            break;
        case reportManager.Enum.PeriodType.Monthly:
            this.CacheData.CurrentDatePickerContainer = this.Tables.monthlyCalendarContainer;
            break;
        case reportManager.Enum.PeriodType.Annualy:
            this.CacheData.CurrentDatePickerContainer = this.Tables.annualyCalendarContainer;
            break;
        case reportManager.Enum.PeriodType.MonthRange:
            this.CacheData.CurrentDatePickerContainer = this.Tables.monthRangeCalendarContainer;
            break;
    }

    this.CacheData.CurrentDatePickerContainer.removeClass("display-none");
};
ReportManager.prototype.RenderDailyCalendar = function () {
    var self = this;

    $('#dailyCalendar').nexDatePicker({
        nexDatePickerType: "daterange",
        datePickerFormat: {
            changeMonth: true,
            changeYear: true,
            dateFormat: FormatConstant.M_DD_YY_STRING,
            minDate: new Date(minDateDaily),
            maxDate: new Date(maxDate)
        },
        firstDatePicker: {
            Id: "dailyCalendar_FromDate",
            Label: labelFrom,
            InitDate: maxDate,
            LabelCss: "lbl-field col_3 alpha",
            TextboxCss: "edit-field col_7 align-left"
        },
        secondDatePicker: {
            Id: "dailyCalendar_ToDate",
            Label: labelTo,
            InitDate: maxDate,
            LabelCss: "lbl-field col_3",
            TextboxCss: "edit-field col_7 omerga align-left"
        },
        onSelectDatePicker: function () {
            self.GetLeaguesBySport();
        }
    });

    self.Tables.dailyCalendarContainer = $("#dailyCalendarContainer");
    self.Tables.dailyCalendar = $("#dailyCalendar");
};
ReportManager.prototype.RenderWeeklyCalendar = function () {
    this.Tables.weekyCalendarContainer = $("#weeklyCalendarContainer");

    this.Tables.fromWeek = $("#fromWeek");
    this.Tables.fromWeek.attr("data-maxdate", maxDate);
    this.Tables.fromWeek.attr("data-mindate", minDateWeekly);
    this.Tables.fromWeek.attr("data-value", maxDate);

    this.Tables.toWeek = $("#toWeek");
    this.Tables.toWeek.attr("data-maxdate", maxDate);
    this.Tables.toWeek.attr("data-mindate", minDateWeekly);
    this.Tables.toWeek.attr("data-value", maxDate);

    weekPickerManager.registerWeekPicker();
    this.Tables.fromWeek.weekpicker('setDate', new Date(maxDate));
    this.Tables.toWeek.weekpicker('setDate', new Date(maxDate));
    this.Tables.fromWeek.weekpicker('onSelect', this.ChangeSelectWeek_WeeklyCalendar);
    this.Tables.toWeek.weekpicker('onSelect', this.ChangeSelectWeek_WeeklyCalendar);
};
ReportManager.prototype.RenderMonthlyCalendar = function () {
    var self = this;
    self.Tables.monthlyCalendarContainer = $("#monthlyCalendarContainer");
    self.Tables.ddlMonth_MonthlyPicker = $("#ddlMonth_MonthlyPicker");
    self.Tables.ddlYear_MonthlyPicker = $("#ddlYear_MonthlyPicker");

    var yearOptions = self.RenderDropdownYear();
    self.Tables.ddlYear_MonthlyPicker.empty()
                                     .append(yearOptions);

    var monthOptions = self.RenderDropdownMonth(self.Tables.ddlYear_MonthlyPicker.val());
    this.Tables.ddlMonth_MonthlyPicker.empty().append(monthOptions);

    self.Tables.ddlMonth_MonthlyPicker.unbind("change");
    self.Tables.ddlMonth_MonthlyPicker.change(function () {
        self.GetLeaguesBySport();
    });

    self.Tables.ddlYear_MonthlyPicker.unbind("change");
    self.Tables.ddlYear_MonthlyPicker.change(function () {
        var monthOptions = self.RenderDropdownMonth(self.Tables.ddlYear_MonthlyPicker.val(),
                                                    self.Tables.ddlMonth_MonthlyPicker.val());
        self.Tables.ddlMonth_MonthlyPicker.empty().append(monthOptions);
        self.GetLeaguesBySport();
    });
};
ReportManager.prototype.RenderAnnualyCalendar = function () {
    var self = this;
    self.Tables.annualyCalendarContainer = $("#annualyCalendarContainer");
    self.Tables.ddlYear_AnnualyPicker = $("#ddlYear_AnnualyPicker");

    var yearOptions = self.RenderDropdownYear();
    self.Tables.ddlYear_AnnualyPicker.empty()
                                     .append(yearOptions);

    self.Tables.ddlYear_AnnualyPicker.unbind("change");
    self.Tables.ddlYear_AnnualyPicker.change(function () {
        self.GetLeaguesBySport();
    });
};
ReportManager.prototype.RenderMonthRangeCalendar = function () {
    var self = this;
    self.Tables.monthRangeCalendarContainer = $("#monthRangeCalendarContainer");
    self.Tables.ddlFromMonth_MonthRangePicker = $("#ddlFromMonth_MonthRangePicker");
    self.Tables.ddlFromYear_MonthRangePicker = $("#ddlFromYear_MonthRangePicker");
    self.Tables.ddlToMonth_MonthRangePicker = $("#ddlToMonth_MonthRangePicker");
    self.Tables.ddlToYear_MonthRangePicker = $("#ddlToYear_MonthRangePicker");

    var yearOptions = self.RenderDropdownYear();
    self.Tables.ddlFromYear_MonthRangePicker.empty()
                                            .append(yearOptions);
    self.Tables.ddlToYear_MonthRangePicker.empty()
                                            .append(yearOptions);

    var monthOptions = self.RenderDropdownMonth(self.Tables.ddlFromYear_MonthRangePicker.val());
    self.Tables.ddlFromMonth_MonthRangePicker.empty().append(monthOptions);
    self.Tables.ddlToMonth_MonthRangePicker.empty().append(monthOptions);

    self.Tables.ddlFromYear_MonthRangePicker.unbind("change");
    self.Tables.ddlFromYear_MonthRangePicker.change(function () {
        var monthOptions = self.RenderDropdownMonth(self.Tables.ddlFromYear_MonthRangePicker.val(),
                                                    self.Tables.ddlFromMonth_MonthRangePicker.val());
        self.Tables.ddlFromMonth_MonthRangePicker.empty().append(monthOptions);
        self.GetLeaguesBySport();
    });

    self.Tables.ddlFromMonth_MonthRangePicker.unbind("change");
    self.Tables.ddlFromMonth_MonthRangePicker.change(function () {
        self.GetLeaguesBySport();
    });

    self.Tables.ddlToYear_MonthRangePicker.unbind("change");
    self.Tables.ddlToYear_MonthRangePicker.change(function () {
        self.CheckFromToConstraint_MonthRangePicker();
        var monthOptions = self.RenderDropdownMonth(self.Tables.ddlToYear_MonthRangePicker.val(),
                                                    self.Tables.ddlToMonth_MonthRangePicker.val());
        self.Tables.ddlToMonth_MonthRangePicker.empty().append(monthOptions);
        self.GetLeaguesBySport();
    });

    self.Tables.ddlToMonth_MonthRangePicker.unbind("change");
    self.Tables.ddlToMonth_MonthRangePicker.change(function () {
        self.CheckFromToConstraint_MonthRangePicker();
        self.GetLeaguesBySport();
    });
};
ReportManager.prototype.CheckFromToConstraint_MonthRangePicker = function () {
    var self = this;

    var fromMonth = self.Tables.ddlFromMonth_MonthRangePicker.val();
    var fromYear = self.Tables.ddlFromYear_MonthRangePicker.val();
    var toMonth = self.Tables.ddlToMonth_MonthRangePicker.val();
    var toYear = self.Tables.ddlToYear_MonthRangePicker.val();

    var yearOptions = '';
    if (toYear < fromYear) {
        yearOptions = self.RenderDropdownYear(null, toYear, toYear);
        self.Tables.ddlFromYear_MonthRangePicker.val(toYear);

        if (toMonth < fromMonth) {
            self.Tables.ddlFromMonth_MonthRangePicker.val(toMonth);
        }

        var monthOptions = self.RenderDropdownMonth(self.Tables.ddlFromYear_MonthRangePicker.val(),
                                                    self.Tables.ddlFromMonth_MonthRangePicker.val(),
                                                    null,
                                                    toMonth);
        self.Tables.ddlFromMonth_MonthRangePicker.empty().append(monthOptions);
    }
    else {
        yearOptions = self.RenderDropdownYear(null, toYear, self.Tables.ddlFromYear_MonthRangePicker.val());
        if (toMonth < fromMonth) {
            self.Tables.ddlFromMonth_MonthRangePicker.val(toMonth);
        }

        var monthOptions = self.RenderDropdownMonth(self.Tables.ddlFromYear_MonthRangePicker.val(),
                                                self.Tables.ddlFromMonth_MonthRangePicker.val());
        self.Tables.ddlFromMonth_MonthRangePicker.empty().append(monthOptions);
    }

    self.Tables.ddlFromYear_MonthRangePicker.empty()
                                            .append(yearOptions);
};
ReportManager.prototype.RenderDropdownYear = function (minYear, maxYear, currentYear) {
    var date = new Date(maxDate);
    if (minYear == null) {
        minYear = new Date(minDate).getFullYear();
    }

    if (maxYear == null) {
        maxYear = date.getFullYear();
    }

    if (currentYear == null) {
        currentYear = maxYear;
    }

    var yearOptions = '';
    for (var i = minYear; i <= maxYear; i++) {
        if (i == currentYear) {
            yearOptions += '<option selected="selected" value=' + i + '>' + i + '</option>';
        }
        else {
            yearOptions += '<option value=' + i + '>' + i + '</option>';
        }
    }

    return yearOptions;
};
ReportManager.prototype.RenderDropdownMonth = function (selectedYear,
    selectedMonth,
    minMonth,
    maxMonth) {
    var date = new Date(maxDate);

    var current_month = date.getMonth() + 1;
    var current_year = date.getFullYear();
    if (selectedYear < current_year && selectedMonth) {
        current_month = selectedMonth;
    }

    var year = parseInt(selectedYear);
    if (minMonth == null) {
        minMonth = 1;
    }

    var maxMonth;
    if (maxMonth == null) {
        maxMonth = current_month;
        if (selectedYear && year != current_year) {
            maxMonth = 12;
        }
    }

    var monthOptions = '';
    for (var i = minMonth - 1; i <= maxMonth - 1; i++) {
        if (i == current_month - 1) {
            monthOptions += '<option selected="selected" value=' + (i + 1) + '>' + reportManager.MonthNames[i] + '</option>';
        }
        else {
            monthOptions += '<option value=' + (i + 1) + '>' + reportManager.MonthNames[i] + '</option>';
        }
    }

    return monthOptions;
};
ReportManager.prototype.ChangeSelectWeek_WeeklyCalendar = function (startDate, endDate, instance) {
    //                    punter.loadLeagueBySport();
    if ($(instance).attr("id") == "fromWeek") {
        if (reportManager.Tables.toWeek.weekpicker("getDates").start < startDate) {
            reportManager.Tables.toWeek.weekpicker('setDate', new Date(startDate));
            return;
        }
    }

    if ($(instance).attr("id") == "toWeek") {
        if (startDate < reportManager.Tables.fromWeek.weekpicker("getDates").start) {
            reportManager.Tables.fromWeek.weekpicker('setDate', new Date(startDate));
            return;
        }
    }

    reportManager.GetLeaguesBySport();
};
ReportManager.prototype.GetSports = function () {
    var self = this;

    AccReport.disableInput();
    self.Params.getSports.IsSB = true;
    $.ajax({
        type: "POST",
        url: window.rootUrl + "PunterAnalysis/GetSports",
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        async: false,
        data: JSON.stringify(self.Params.getSports),
        success: function (rs) {
            self.CacheData.Sports = rs;
            self.RenderDropdownSports(rs);
            self.RenderDropdownSports_ByLeague();
        },
        error: function (event, jqXHR, ajaxSettings) {
            self.RenderDropdownSports(null);
        },
        complete: function () {
            AccReport.enableInput();
        }
    });
};
ReportManager.prototype.RenderDropdownSports = function (sports) {
    var ddSport = [];
    var ddId = "ddlSport";
    var selectedProducts = this.GetSelectedProducts();
    var isExistsSportbook = $.inArray(this.CacheData.Sportbook_ProductId, selectedProducts) != -1;
    if (isExistsSportbook) {
        var sportGroupLength = sports ? sports.length : 0;
        for (var i = 0; i < sportGroupLength; i++) {
            var sportGroup = sports[i];
            var sportLength = sportGroup.ChildsInfo ? sportGroup.ChildsInfo.length : 0;
            var ddSportGroup = [];
            for (var j = 0; j < sportLength; j++) {
                var sport = sportGroup.ChildsInfo[j];
                var betTypeLength = sport.ChildsInfo ? sport.ChildsInfo.length : 0;
                var ddBetType = [];
                for (var k = 0; k < betTypeLength; k++) {
                    var betType = sport.ChildsInfo[k];

                    ddBetType.push({
                        Id: "#" + ddId + "SportGroup" + sportGroup.Id +
                                         "_Sport" + sport.Id +
                                         "_BetType" + betType.Id,
                        Value: sport.Id + "_" + betType.Id,
                        Text: betType.Name,
                        Checked: true,
                        Enable: true,
                        CssClass: ""
                    });
                }

                ddSportGroup.push({
                    Id: "#" + ddId + "SportGroup" + sportGroup.Id + "_Sport" + sport.Id,
                    Value: (sportGroup.Name != "Parlay" ? sportGroup.Id : "0") + "_" + sport.Id,
                    Text: sport.Name,
                    Checked: true,
                    Enable: true,
                    CssClass: "",
                    Items: ddBetType,
                    Show: false
                });
            }

            if (sportGroup.Name != "Parlay" ||
                (sportGroup.ChildsInfo && sportGroup.ChildsInfo.length > 0)) {
                ddSport.push({
                    Id: "#" + ddId + "SportGroup" + sportGroup.Id.toString(),
                    Value: sportGroup.Id,
                    Text: sportGroup.Name,
                    Checked: true,
                    Enable: true,
                    CssClass: "",
                    Items: ddSportGroup
                });
            }
        }
    }

    $("#" + ddId).multiSelect({
        minWidth: 190,
        maxHeight: 350,
        shadow: true,
        showHeader: true,
        isCheckAll: true,
        showFooter: false,
        checkAllText: 'All',
        isUsedCustomCheckAllText: true,
        customCheckAllText: 'All',
        noneSelectedText: 'Select an option',
        noItemText: 'No sport found.',
        disabled: !isExistsSportbook,
        listData: ddSport,
        onCheck: function () {
        }
    });

    this.Tables.ddlSport = $("#ddlSport");
    if (!isExistsSportbook) {
        this.Tables.ddlSport.multiSelect("unCheckAllItems");
        this.Tables.ddlSport.children('input').eq(0).val('Not applicable');
    }
};
ReportManager.prototype.GetSelectedProducts = function () {
    return this.Tables.ddlProduct.multiSelect('getCheckedItems');
};
ReportManager.prototype.ChangeSearchBy = function (searchByType) {
    var self = this;
    searchByType = parseInt(searchByType);
    this.CacheData.SearchByType = searchByType;
    switch (searchByType) {
        case reportManager.Enum.SearchByType.User:
            this.Tables.filterByLeague.addClass("display-none");
            this.Tables.filterByUser.removeClass("display-none");
            this.TopPunter_ByLeague.HideReport();
            this.BindingEvent_ByUser();
            break;
        case reportManager.Enum.SearchByType.League:
            this.Tables.filterByUser.addClass("display-none");
            this.Tables.filterByLeague.removeClass("display-none");
            this.HideReport();
            this.GetLeaguesBySport();
            this.GetBetTypesBySport();
            this.BindingEvent_ByLeague();
            break;
    }
};
ReportManager.prototype.RenderDropdownSports_ByLeague = function () {
    this.Tables.ddlSport_ByLeague = $("#ddlSport_ByLeague");
    var options = '';

    if (this.CacheData.Sports.length >= 2) {
        var soccerGroup = this.CacheData.Sports[0];
        options += '<option value=' + soccerGroup.Id + '>' + soccerGroup.Name + '</option>';

        var otherSportsGroup = this.CacheData.Sports[1];
        var otherSportsLength = otherSportsGroup.ChildsInfo.length;
        for (var i = 0; i < otherSportsLength; i++) {
            var sport = otherSportsGroup.ChildsInfo[i];
            options += '<option value=' + sport.Id + '>' + sport.Name + '</option>';
        }
    }

    this.Tables.ddlSport_ByLeague.html(options);
};
ReportManager.prototype.GetLeaguesBySport = function () {
    var self = this;
    if (self.CacheData.SearchByType == reportManager.Enum.SearchByType.User) {
        return;
    }

    AccReport.disableInput();
    var dateRange = self.GetFromDate_ToDate();
    self.Params.getLeaguesBySport.FromDate = dateRange.FromDate;
    self.Params.getLeaguesBySport.ToDate = dateRange.ToDate;
    self.Params.getLeaguesBySport.Sporttype = self.Tables.ddlSport_ByLeague.val();
    $.ajax({
        type: "POST",
        url: window.rootUrl + "PunterAnalysis/GetLeaguesBySport",
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        async: false,
        data: JSON.stringify(self.Params.getLeaguesBySport),
        success: function (rs) {
            self.RenderLeaguesBySport(rs);
        },
        error: function (event, jqXHR, ajaxSettings) {
            self.RenderLeaguesBySport(null);
        },
        complete: function () {
            AccReport.enableInput();
        }
    });
};
ReportManager.prototype.RenderLeaguesBySport = function (leagues) {
    this.Tables.ddlLeague_ByLeague = $("#ddlLeague_ByLeague");
    var options = '';
    var leagueLength = leagues ? leagues.length : 0;
    if (leagueLength >= 1) {
        for (var i = 0; i < leagueLength; i++) {
            var league = leagues[i];
            options += '<option value=' + league.Id + '>' + league.Name + '</option>';
        }
    }
    else {
        options += '<option value="-100000">There is no information available.</option>';
    }

    this.Tables.ddlLeague_ByLeague.html(options);
};
ReportManager.prototype.GetFromDate_ToDate = function () {
    var dateRange = {
        FromDate: '',
        ToDate: ''
    };

    var periodType = parseInt(this.Tables.ddlPeriodType.val());
    var date = new Date(maxDate);
    var currentMonth = date.getMonth() + 1;
    var currentYear = date.getFullYear();
    var currentDay = date.getDate();

    switch (periodType) {
        case reportManager.Enum.PeriodType.Daily:
            dateRange = this.Tables.dailyCalendar.nexDatePicker("getDateRange");
            break;
        case reportManager.Enum.PeriodType.Weekly:
            dateRange.FromDate = moment(this.Tables.fromWeek.weekpicker("getDates").start).format(FormatConstant.MM_DD_YYYY_WITH_SLASH_STRING_MOMENT);
            dateRange.ToDate = moment(this.Tables.toWeek.weekpicker("getDates").end).format(FormatConstant.MM_DD_YYYY_WITH_SLASH_STRING_MOMENT);
            break;
        case reportManager.Enum.PeriodType.Monthly:
            var selectedMonth = this.Tables.ddlMonth_MonthlyPicker.val();
            var selectedYear = this.Tables.ddlYear_MonthlyPicker.val();
            dateRange.FromDate = selectedMonth + "/01/" + selectedYear;
            if (selectedYear == currentYear &&
                selectedMonth == currentMonth) {
                dateRange.ToDate = selectedMonth + "/" + currentDay + "/" + selectedYear;
            }
            else {
                dateRange.ToDate = selectedMonth + "/" +
                                   new Date(selectedYear, selectedMonth, 0).getDate() + "/" +
                                   selectedYear;
            }

            break;
        case reportManager.Enum.PeriodType.Annualy:
            var selectedYear = this.Tables.ddlYear_AnnualyPicker.val();
            dateRange.FromDate = "01/01/" + selectedYear;
            if (selectedYear == currentYear) {
                dateRange.ToDate = maxDate;
            } else {
                dateRange.ToDate = "12/31/" + selectedYear;
            }

            break;
        case reportManager.Enum.PeriodType.MonthRange:
            dateRange.FromDate = this.Tables.ddlFromMonth_MonthRangePicker.val() + "/01/" + this.Tables.ddlFromYear_MonthRangePicker.val();

            var selectedToMonth = this.Tables.ddlToMonth_MonthRangePicker.val();
            var selectedToYear = this.Tables.ddlToYear_MonthRangePicker.val();
            if (selectedToYear == currentYear &&
               selectedToMonth == currentMonth) {
                dateRange.ToDate = maxDate;
            } else {
                dateRange.ToDate = selectedToMonth + "/" +
                                   new Date(selectedToYear, selectedToMonth, 0).getDate() + "/" +
                                   selectedToYear;
            }

            break;
    }

    return dateRange;
};
ReportManager.prototype.GetBetTypesBySport = function () {
    var self = this;
    if (self.CacheData.SearchByType == reportManager.Enum.SearchByType.User) {
        return;
    }

    AccReport.disableInput();
    self.Params.getBetTypesBySport.Sporttype = self.Tables.ddlSport_ByLeague.val();
    $.ajax({
        type: "POST",
        url: window.rootUrl + "PunterAnalysis/GetBetTypesBySport",
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        async: false,
        data: JSON.stringify(self.Params.getBetTypesBySport),
        success: function (rs) {
            self.RenderBetTypesBySport(rs);
        },
        error: function (event, jqXHR, ajaxSettings) {
            self.RenderBetTypesBySport(null);
        },
        complete: function () {
            AccReport.enableInput();
        }
    });
};
ReportManager.prototype.RenderBetTypesBySport = function (betTypes) {
    var self = this;
    var ddBetType = [];
    var ddId = "ddlBetType_ByLeague";
    var betTypeLength = betTypes ? betTypes.length : 0;
    for (var i = 0; i < betTypeLength; i++) {
        var betType = betTypes[i];
        ddBetType.push({
            Id: "#" + ddId + "_BetType_" + betType.Id,
            Value: betType.Id,
            Text: betType.Name,
            Checked: true,
            Enable: true,
            Items: []
        });
    }

    $("#" + ddId).multiSelect({
        minWidth: 190,
        maxHeight: 350,
        shadow: true,
        showHeader: true,
        isCheckAll: true,
        showFooter: false,
        checkAllText: 'All',
        isUsedCustomCheckAllText: true,
        customCheckAllText: 'All',
        noneSelectedText: 'Select an option',
        noItemText: 'No bet type found.',
        listData: ddBetType,
        onCheck: function () {
        }
    });

    this.Tables.ddlBetType_ByLeague = $("#ddlBetType_ByLeague");
};
ReportManager.prototype.GetReport = function () {
    var self = this;

    AccReport.disableInput();
    self.GetParams();
    if (self.Params.reportParams.UserName != "") {
        self.GetTopPunterByUser(0, self.Params.reportParams.UserName, true);
        return;
    }

    $.ajax({
        type: "POST",
        url: window.rootUrl + "Special/PunterAnalysis/GetTopPunterReport",
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        data: JSON.stringify(self.Params.reportParams),
        beforeSend: function () {
        },
        success: function (rs) {
            self.RenderReport(rs);
        },
        error: function (event, jqXHR, ajaxSettings) {
            self.RenderReport(null);
        },
        complete: function () {
            AccReport.enableInput();
        }
    });
};
ReportManager.prototype.GetParams = function () {
    var dateRange = this.GetFromDate_ToDate();
    this.Params.reportParams.Fromdate = dateRange.FromDate;
    this.Params.reportParams.Todate = dateRange.ToDate;

    var username = $('#txtUserName').val().replace(/ /g, '');
    this.Params.reportParams.UserName = username == 'All' ? '' : username;

    var selectedProducts = this.Tables.ddlProduct.multiSelect('getCheckedItems');
    var selectedBetTypes = this.Tables.ddlSport.multiSelect('getCheckedItems');
    this.Params.reportParams.Sportbettype = selectedBetTypes.join(",") + "," + selectedProducts.join(",");
    this.Params.reportParams.TopRow = parseInt(this.Tables.txtTopRows.val(), 0);
    this.Params.reportParams.OrderBy = parseInt(this.Tables.ddlOrderType.val(), 0);
    this.Params.reportParams.OrderByName = this.Tables.ddlOrderType.find('option:selected').text();

    this.Params.reportParams.OrderType = parseInt(this.Tables.ddlOrderDirection.val(), 0);
    this.Params.reportParams.OrderTypeName = this.Tables.ddlOrderDirection.find('option:selected').text();

    this.Params.reportParams.PeriodType = parseInt(this.Tables.ddlPeriodType.val(), 0);
    this.Params.reportParams.Reporttype = parseInt(this.Params.reportParams.PeriodType);
    this.CacheData.SelectedSportBetTypes = selectedBetTypes;
};
ReportManager.prototype.RenderReport = function (report) {
    var self = this;

    this.Tables.reportContainer.removeClass("display-none");
    this.Tables.report.addClass("display-none");
    var bodyData = [];
    var footerData = [];

    if (report && report.ChildsInfo.length > 0) {
        var len = report.ChildsInfo.length;
        var currentRow = null;
        var info = null;
        for (var i = 0; i < len; i++) {
            info = report.ChildsInfo[i];
            bodyData.push({
                Columns: [
                         {
                             Name: "ColUserName",
                             Value: info,
                             CustomCellRender: function (currentCell) {
                                 var clickEvent = ' onclick = "reportManager.GetTopPunterByUser(\'' + currentCell.Value.Id + '\',\'' +
                                                                                                currentCell.Value.Name + "');\"";
                                 return '<div class="link" ' + clickEvent + '>' + currentCell.Value.Name + '</div>';
                             }
                         },
                          { Name: "ColWinLoss", Value: reportManager.FormatNumberCell(info.WinLoss_Str, false, true, false) },
                          { Name: "ColTurnover", Value: info.Turnover_Str },
                          { Name: "ColMargin", Value: reportManager.FormatNumberCell(info.Margin_Str, false, true, false) },
                          {
                              Name: "ColBetCount",
                              Value: info,
                              CustomCellRender: function (currentCell) {
                                  var clickEvent = ' onclick = "reportManager.GetTopPunterBetCount_ByUser(\'' + currentCell.Value.Id + '\',\'' +
                                                                                         currentCell.Value.Name + "');\"";

                                  return '<div class="link" ' + clickEvent + '>' + currentCell.Value.BetCount + '</div>';
                              }
                          },
                          {
                              Name: "ColIP",
                              Value: {
                                  isShowLink: self.Params.reportParams.PeriodType != 0 &&
                                              self.Params.reportParams.PeriodType != 1,
                                  Id: info.Id,
                                  Name: info.Name
                              },
                              CustomCellRender: function (currentCell) {
                                  var clickEvent = ' onclick = "reportManager.GetTopPunterIpByUser(\'' + currentCell.Value.Id + '\',\'' +
                                                                                   currentCell.Value.Name + "');\"";
                                  if (currentCell.Value.isShowLink == true) {
                                      return '<div class="link" ' + clickEvent + '>view by ip</div>';
                                  }

                                  return '<div>view by ip</div>';
                              }
                          }]
            });
        }

        info = report.GrandTotal;
        footerData.push({
            Columns: [{ Name: "ColUserName", Value: info.Name, CssClass: "align-right" },
                      { Name: "ColWinLoss", Value: reportManager.FormatNumberCell(info.WinLoss_Str, false, true, false) },
                      { Name: "ColTurnover", Value: info.Turnover_Str },
                      { Name: "ColMargin", Value: reportManager.FormatNumberCell(info.Margin_Str, false, true, false) },
                      { Name: "ColBetCount", Value: info.BetCount_Str },
                      { Name: "ColIP"}]
        });
    }

    var gridOptions = {
        customizeHeader: false,
        columns: [{ HeaderText: "User Name", Width: 140, Name: "ColUserName", HeaderAlign: "center", CellAlign: "left" },
                  { HeaderText: "Win Loss", Width: 120, Name: "ColWinLoss", HeaderAlign: "center", CellAlign: "right" },
                  { HeaderText: "Turnover", Width: 120, Name: "ColTurnover", HeaderAlign: "center", CellAlign: "right" },
                  { HeaderText: "Margin", Width: 80, Name: "ColMargin", HeaderAlign: "center", CellAlign: "right" },
                  { HeaderText: "Bet Count", Width: 100, Name: "ColBetCount", HeaderAlign: "center", CellAlign: "right" },
                  { HeaderText: "IP", Width: 100, Name: "ColIP", HeaderAlign: "center", CellAlign: "center"}],
        showTitle: true,
        showFooter: true,
        titleInfo:
            {
                text: 'Main - Punter Analysis',
                showExcelButton: true,
                exportExcelFunction: function () {
                    self.ExportReport();
                }
            },
        showTree: false,
        bodyRows: bodyData,
        footerRows: footerData,
        gridExpandHeight: 250,
        numberOfFrozenColumn: 1,
        gridWidth: 900,
        showDistinctRow: false,
        showHighlightRow: false
    };

    this.Tables.report.removeClass("display-none").fanexGrid(gridOptions);
    this.ShowReport();
};
ReportManager.prototype.HideReport = function () {
    this.Tables.reportContainer.addClass("display-none");
    this.Tables.report.addClass("display-none");

    this.TopPunterByUser.HideReport();
    this.TopPunter_ByUser_SportType.HideReport();
    this.TopPunterBetCount_ByUser_SportType.HideReport();
    this.TopPunterBetCount_ByUser_BetType.HideReport();
    this.TopPunter_ByUser_BetType.HideReport();
    this.TopPunterBetCountByUser.HideReport();
    this.TopPunterIPByUser.HideReport();
};
ReportManager.prototype.ShowReport = function () {
    this.TopPunterByUser.HideReport();
    this.TopPunter_ByUser_SportType.HideReport();
    this.TopPunterBetCount_ByUser_SportType.HideReport();
    this.TopPunterBetCount_ByUser_BetType.HideReport();
    this.TopPunter_ByUser_BetType.HideReport();
    this.TopPunterBetCountByUser.HideReport();
    this.TopPunterIPByUser.HideReport();

    this.Tables.report.removeClass("display-none");
    this.Tables.reportContainer.removeClass("display-none");
};
ReportManager.prototype.GetParams_ByLeague = function () {
    var dateRange = this.GetFromDate_ToDate();
    var selectedBetTypes = this.Tables.ddlBetType_ByLeague.multiSelect('getCheckedItems');
    selectedBetTypes.push(reportManager.Enum.Sport.KickOff);

    return {
        Fromdate: dateRange.FromDate,
        Todate: dateRange.ToDate,
        TopRow: parseInt(this.Tables.txtTopRows.val(), 0),
        OrderBy: parseInt(this.Tables.ddlOrderType.val(), 0),
        OrderType: parseInt(this.Tables.ddlOrderDirection.val(), 0),
        Reporttype: parseInt(this.Tables.ddlPeriodType.val(), 0),
        LeagueID: parseInt(this.Tables.ddlLeague_ByLeague.val(), 0),
        Sporttype: parseInt(this.Tables.ddlSport_ByLeague.val(), 0),
        Bettype: selectedBetTypes.join(","),

        OrderByName: this.Tables.ddlOrderType.find('option:selected').text(),
        OrderTypeName: this.Tables.ddlOrderDirection.find('option:selected').text(),
        SportName: this.Tables.ddlSport_ByLeague.find('option:selected').text(),
        LeagueName: this.Tables.ddlLeague_ByLeague.find('option:selected').text()
    };
};
ReportManager.prototype.GetReport_ByLeague = function () {
    var self = this;
    AccReport.disableInput();
    self.TopPunter_ByLeague.GetReport(self.GetParams_ByLeague());
};
ReportManager.prototype.GetTopPunterByUser = function (userId, userName, isDirectFromFilter) {
    this.TopPunterByUser.GetReport({
        UserName: userName,
        CustID: userId,
        Fromdate: this.Params.reportParams.Fromdate,
        Todate: this.Params.reportParams.Todate,
        Sportbettype: this.Params.reportParams.Sportbettype,
        TopRow: this.Params.reportParams.TopRow,
        OrderBy: this.Params.reportParams.OrderBy,
        OrderType: this.Params.reportParams.OrderType,
        Reporttype: this.Params.reportParams.Reporttype,
        IsDirectFromFilter: isDirectFromFilter,
        OrderByName: this.Params.reportParams.OrderByName,
        OrderTypeName: this.Params.reportParams.OrderTypeName
    });
};
ReportManager.prototype.GetTopPunterBetCount_ByUser = function (userId, userName) {
    this.TopPunterBetCountByUser.GetReport({
        CustID: userId,
        UserName: userName,
        LeagueID: null,
        Sporttype: null,
        Bettype: null,
        Sportbettype: this.Params.reportParams.Sportbettype,
        Fromdate: this.Params.reportParams.Fromdate,
        Todate: this.Params.reportParams.Todate,
        Reporttype: this.Params.reportParams.PeriodType,
        ByLeague: null
    });
};
ReportManager.prototype.GetTopPunter_ByUser_SportType = function (sportId, sportName) {
    var betTypes = [reportManager.Enum.Sport.KickOff];
    var sport_bettype_all = this.Params.reportParams.Sportbettype.split(',')
    for (var i = 0; i < sport_bettype_all.length; i++) {
        var sport_betType = sport_bettype_all[i].split("_");
        var sportType = sport_betType[0];
        var betType = sport_betType[1];

        if (sport_betType.length >= 2 && $.inArray(betType, betTypes) == -1 && sportType == sportId) {
            betTypes.push(betType);
        }
    }

    this.TopPunter_ByUser_SportType.GetReport({
        UserName: this.TopPunterByUser.Params.reportParams.UserName,
        CustID: this.TopPunterByUser.Params.reportParams.CustID,
        Fromdate: this.Params.reportParams.Fromdate,
        Todate: this.Params.reportParams.Todate,
        TopRow: this.Params.reportParams.TopRow,
        OrderBy: this.Params.reportParams.OrderBy,
        OrderType: this.Params.reportParams.OrderType,
        Reporttype: this.Params.reportParams.Reporttype,
        Sporttype: sportId,
        Bettype: betTypes.join(","),
        SportName: sportName
    });
};
ReportManager.prototype.ShowTopPunter_ByUser_SportType = function () {
    this.TopPunterBetCount_ByUser_BetType.HideReport();
    this.TopPunter_ByUser_BetType.HideReport();
    this.TopPunter_ByUser_SportType.ShowReport();
};
ReportManager.prototype.GetTopPunterIpByUser = function (userId, userName) {
    this.TopPunterIPByUser.GetReport({
        CustID: userId,
        UserName: userName,
        Fromdate: this.Params.reportParams.Fromdate,
        Todate: this.Params.reportParams.Todate,
        Reporttype: this.Params.reportParams.Reporttype
    });
};
ReportManager.prototype.ShowReportTopPunterByUser = function () {
    this.HideReport();
    this.TopPunter_ByUser_SportType.HideReport();
    this.TopPunterBetCount_ByUser_SportType.HideReport();
    this.TopPunterBetCount_ByUser_BetType.HideReport();
    this.TopPunter_ByUser_BetType.HideReport();
    this.TopPunterByUser.ShowReport();
};
ReportManager.prototype.GetTopPunterBetCount_ByUser_SportType = function (sportId, sportName) {
    this.TopPunterBetCount_ByUser_SportType.GetReport({
        CustID: this.TopPunterByUser.Params.reportParams.CustID,
        UserName: this.TopPunterByUser.Params.reportParams.UserName,
        LeagueID: null,
        Sporttype: sportId,
        SportName: sportName,
        Bettype: null,
        Sportbettype: this.Params.reportParams.Sportbettype,
        Fromdate: this.Params.reportParams.Fromdate,
        Todate: this.Params.reportParams.Todate,
        Reporttype: this.Params.reportParams.PeriodType,
        ByLeague: null,
        OrderByName: this.Params.reportParams.OrderByName,
        OrderTypeName: this.Params.reportParams.OrderTypeName
    });
};
ReportManager.prototype.GetTopPunterBetCount_ByUser_BetType = function (betTypeId,
    betTypeName,
    liveType,
    liveTypeName) {
    this.TopPunterBetCount_ByUser_BetType.GetReport({
        CustID: this.TopPunterByUser.Params.reportParams.CustID,
        UserName: this.TopPunterByUser.Params.reportParams.UserName,
        LeagueID: null,
        Sporttype: this.TopPunter_ByUser_SportType.Params.reportParams.Sporttype,
        SportName: this.TopPunter_ByUser_SportType.Params.reportParams.SportName,
        Bettype: betTypeId,
        BetTypeName: betTypeName,
        Fromdate: this.Params.reportParams.Fromdate,
        Todate: this.Params.reportParams.Todate,
        Reporttype: this.Params.reportParams.PeriodType,
        Status: liveType,
        LiveTypeName: liveTypeName
    });
};
ReportManager.prototype.GetTopPunterWinLoss_ByUser_BetType = function (betTypeId,
    betTypeName,
    liveType,
    liveTypeName) {
    this.TopPunter_ByUser_BetType.GetReport({
        CustID: this.TopPunterByUser.Params.reportParams.CustID,
        UserName: this.TopPunterByUser.Params.reportParams.UserName,
        LeagueID: null,
        Sporttype: this.TopPunter_ByUser_SportType.Params.reportParams.Sporttype,
        SportName: this.TopPunter_ByUser_SportType.Params.reportParams.SportName,
        Bettype: betTypeId,
        BetTypeName: betTypeName,
        Fromdate: this.Params.reportParams.Fromdate,
        Todate: this.Params.reportParams.Todate,
        OrderBy: this.Params.reportParams.OrderBy,
        OrderType: this.Params.reportParams.OrderType,
        Reporttype: this.Params.reportParams.Reporttype,
        Status: liveType,
        LiveTypeName: liveTypeName,
        OrderByName: this.Params.reportParams.OrderByName,
        OrderTypeName: this.Params.reportParams.OrderTypeName
    });
};
ReportManager.prototype.FormatNumberCell = function (cellValue,
    isPositive,
    isNegative,
    isPercent) {
    var cssClass = '';
    var number = cellValue.replace(/\,/g, '').replace(/%/g, '');
    var isNumber = $.isNumeric(number);

    if (isNumber &&
            ((isPositive && parseFloat(number) > 0) ||
             (isNegative && parseFloat(number) < 0))) {
        return '<div class="color-red">(' + (parseFloat(number) > 0 ? cellValue : cellValue.replace(/\-/g, '')) + (isPercent ? '%' : '') + ')</div>';
    }

    return cellValue + (isNumber && isPercent ? '%' : '');
};
ReportManager.prototype.ExportReport = function () {
    AccReport.downloadFile(window.rootUrl + "Special/PunterAnalysis/ExportTopPunterReport", this.Params.reportParams);

};

var reportManager = new ReportManager();