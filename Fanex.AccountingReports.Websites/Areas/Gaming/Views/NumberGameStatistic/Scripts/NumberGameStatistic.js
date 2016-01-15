$(document).ready(function () {
    reportManager.initPage();
});

Namespace.Register("Fanex.AccountingReports.Gaming.NumberGameStatistic");
Fanex.AccountingReports.Gaming.NumberGameStatistic = function () {
    var self = this;

    var reportByDaily = new Fanex.AccountingReports.Gaming.NumberGameStatisticByDaily();
    var reportByUserName = new Fanex.AccountingReports.Gaming.NumberGameStatisticByUserName();
    var reportByLeague = new Fanex.AccountingReports.Gaming.NumberGameStatisticByLeague();
    var reportByBettype = new Fanex.AccountingReports.Gaming.NumberGameStatisticByBettype();

    var Enums = {
        DateOption: {
            Last3Months: 8,
            Last6Months: 9,
            Today: 0,
            Yesterday: 1
        },

        ReportType: {
            League: 0,
            Daily: 1,
            UserName: 2,
            Bettype: 3
        }
    };

    var _tables = {
        btnGenerate: null,
        btnToday: null,
        btnYesterday: null,

        datepickerplugin: null,
        dateOptionSelector: null,
        firstDatePicker: null,
        secondDatePicker: null,
        last3MonthsDatePicker: null,
        last6MonthsDatePicker: null,

        filterNote: null,
        ddlBettype: null,
        txtUserName: null,
        ddlLeague: null,
        reportBy: null
    };

    var params = {
        reportParams: {
            FromDate: new Date(),
            ToDate: new Date(),
            MoveBetsDay: new Date(),
            UserName: '',
            LeagueId: null,
            BetType: '',
            ReportBy: 0
        }
    };

    var _cachedData = {
        defaultTextboxUserName: viewResources.Search_All,
        currentTextboxUserName: viewResources.Search_All,
        isDefaultTextboxUserName: true,
        isOnAuto: false,
        last3Months: 3,
        last6Months: 6,
        reportType: null
    };

    self.initPage = function () {
        cachedDOMElements();
        _tables.btnToday.val(shortFormattedToday);
        _tables.btnYesterday.val(shortFormattedYesterday);
        renderDatePicker();
        renderBettype();
        makeUserNameSuggestion();

        reportByDaily.initPage();
        reportByUserName.initPage();
        reportByLeague.initPage();
        reportByBettype.initPage();

        bindingEvent();
    };

    var cachedDOMElements = function () {
        _tables.btnGenerate = $("#btnGenerate");
        _tables.btnToday = $("#btnToday");
        _tables.btnYesterday = $("#btnYesterday");
        _tables.filterNote = $("#notemessage");
        _tables.txtUserName = $("#txtUserName");
        _tables.ddlLeague = $("#dropdown-league");
        _tables.reportBy = $("#reportBy");
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
                Id: "dateOptionSelector",
                Label: "Time period",
                LabelCss: "lbl-field col_3 prefix_2",
                OptionCss: "edit-field col_6"
            },
            firstDatePicker: {
                Id: "firstDatePickerId1",
                Label: "From Date",
                InitDate: today,
                LabelCss: "lbl-field col_4 align-right",
                TextboxCss: "edit-field col_6"
            },
            secondDatePicker: {
                Id: "secondDatePickerId1",
                Label: "To Date",
                InitDate: today,
                LabelCss: "lbl-field col_3 align-right",
                TextboxCss: "edit-field col_6"
            },
            selectedItem: 0,
            onSelectDatePicker: function () {
            },
            minDate: minDate,
            maxDate: new Date(today)
        });

        _tables.datepickerplugin = $("#datepickerplugin");
        _tables.firstDatePicker = $("#firstDatePickerId1");
        _tables.secondDatePicker = $("#secondDatePickerId1");
        _tables.dateOptionSelector = $("#dateOptionSelector");

        var dateOption = '<option value="' + Enums.DateOption.Last3Months +
                        '" data-fDate="' + beginLast3Months + '" data-tDate="' + endLast3Months + '">' +
        AccReport.utility.StringFormat(viewResources.Options_LastMonthRange, [3]) + '</option>';
        _tables.dateOptionSelector.append(dateOption);

        dateOption = '<option value="' + Enums.DateOption.Last6Months +
                        '" data-fDate="' + beginLast6Months + '" data-tDate="' + endLast6Months + '">' +
        AccReport.utility.StringFormat(viewResources.Options_LastMonthRange, [6]) + '</option>';
        _tables.dateOptionSelector.append(dateOption);

        _tables.last3MonthsDatePicker = $("#last3MonthsDatePicker");
        _tables.last6MonthsDatePicker = $("#last6MonthsDatePicker");
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

    var renderBettype = function () {
        var bettypes = [
            { BettypeId: 81, BettypeName: "1st Ball O/U" },
            { BettypeId: 82, BettypeName: "Last Ball O/U" },
            { BettypeId: 83, BettypeName: "1st Ball O/E" },
            { BettypeId: 84, BettypeName: "Last Ball O/E" },
            { BettypeId: 85, BettypeName: "Over/Under" },
            { BettypeId: 86, BettypeName: "Odd/Even" },
            { BettypeId: 87, BettypeName: "Next High/Low" },
            { BettypeId: 88, BettypeName: "Warrior" },
            { BettypeId: 89, BettypeName: "Next Combo" },
            { BettypeId: 90, BettypeName: "Number Wheel" }
        ];
        var noItemText = 'No bettype found.';
        var ddwnBettypes = [];
        var prefixId = 'ddlBettype_';
        var numberBettype = bettypes.length;

        for (var i = 0; i < numberBettype; i++) {
            var currentBettype = bettypes[i];
            ddwnBettypes.push({
                Id: prefixId + currentBettype.BettypeId,
                Value: currentBettype.BettypeId,
                Text: currentBettype.BettypeName,
                Checked: true,
                Enable: true,
                CssClass: "",
                Items: []
            });
        }

        $("#dropdown-bettype").multiSelect(
        {
            minWidth: 125,
            maxHeight: 450,
            shadow: true,
            showHeader: true,
            isCheckAll: true,
            showFooter: false,
            showTooltip: false,
            checkAllText: 'All',
            isUsedCustomCheckAllText: true,
            customCheckAllText: 'All',
            noItemText: noItemText,
            selectedList: 3,
            listData: ddwnBettypes
        });

        _tables.ddlBettype = $("#dropdown-bettype");
    };

    var makeUserNameSuggestion = function () {
        var self = this;
        _cachedData.autoCompleteManager = _tables.txtUserName.autocomplete({
            serviceUrl: window.rootWebPath.replace(/\/$/g, '') + '/ReportHelper/UserNameSuggestion?' +
            $.param(getUserNameSuggestionParams()),
            minChars: 2
        });

        _tables.autocomplete = $('.autocomplete');
    };

    var getUserNameSuggestionParams = function () {
        return {
            excludeTestAccounts: true
        };
    };


    var bindingEvents_UserNameSuggestion = function () {
        _tables.txtUserName.unbind("blur");
        _tables.txtUserName.blur(function () {
            var textbox = $(this);
            if (!_cachedData.isonAuto) {
                $('#Autocomplete_1').css('display', 'none');
            }
            else {
                textbox.focus();
            }

            var inputValue = '';
            var newName = textbox.val();
            var oldName = _cachedData.currentTextboxUserName;
            _cachedData.isDefaultTextboxUserName = false;
            textbox.removeClass("defaultSuggestionBox");
            if (oldName != '' &&
            newName != '' &&
            newName != oldName) {
                inputValue = newName;
            }
            else if (oldName != '' && newName == '') {
                inputValue = oldName;
                _cachedData.isDefaultTextboxUserName = true;
                textbox.addClass("defaultSuggestionBox");
            }
            else if (newName != '') {
                inputValue = newName;
            }
            else {
                _cachedData.isDefaultTextboxUserName = true;
                textbox.addClass("defaultSuggestionBox");
                inputValue = _cachedData.defaultTextboxUserName;
            }

            textbox.val(inputValue);
        });

        _tables.txtUserName.unbind("focus");
        _tables.txtUserName.focus(function () {
            var textbox = $(this);
            textbox.removeClass("defaultSuggestionBox");
            if (_cachedData.isDefaultTextboxUserName) {
                textbox.val('');
            }
        });

        _tables.autocomplete.mouseover(function () {
            _cachedData.isonAuto = true;
        }).mouseout(function () {
            _tables.txtUserName.removeClass("defaultSuggestionBox");
            _cachedData.isDefaultTextboxUserName = false;
            _cachedData.isonAuto = false;
        });
    };

    var bindingEvent = function () {
        bindingEvents_UserNameSuggestion();

        _tables.btnGenerate.unbind("click");
        _tables.btnGenerate.click(function () {
            getReport();
        });

        _tables.btnToday.click(function () {
            _tables.dateOptionSelector.val(Enums.DateOption.Today);
            _tables.secondDatePicker.datepicker("option", "minDate", new Date(today));
            _tables.firstDatePicker.datepicker("option", "maxDate", new Date(today));
            _tables.firstDatePicker.datepicker("setDate", new Date(today));
            _tables.secondDatePicker.datepicker("setDate", new Date(today));

            getReport();
        });

        _tables.btnYesterday.click(function () {
            _tables.dateOptionSelector.val(Enums.DateOption.Yesterday);
            _tables.secondDatePicker.datepicker("option", "minDate", new Date(yesterday));
            _tables.firstDatePicker.datepicker("option", "maxDate", new Date(yesterday));
            _tables.firstDatePicker.datepicker("setDate", new Date(yesterday));
            _tables.secondDatePicker.datepicker("setDate", new Date(yesterday));

            getReport();
        });
    };

    var getReport = function () {
        getReportParams();
        if (validateParams()) {
            getReportByType(parseInt(params.reportParams.ReportBy));
        }
    };

    var getReportParams = function () {
        var dateRange = _tables.datepickerplugin.nexDatePicker("getDateRange");
        var userName = _tables.txtUserName.val();
        var leagueId = parseInt(_tables.ddlLeague.val());
        params.reportParams.FromDate = dateRange.FromDate;
        params.reportParams.ToDate = dateRange.ToDate;
        params.reportParams.UserName = _cachedData.isDefaultTextboxUserName ?
                  "" : userName,
        params.reportParams.LeagueId = leagueId == 0 ? null : leagueId;
        params.reportParams.BetType = _tables.ddlBettype.multiSelect('getCheckedItems').join(",");
        params.reportParams.ReportBy = _tables.reportBy.find("input[name='rdReportBy']:checked").val();

        return params.reportParams;
    };

    var validateParams = function () {
        var isValid = true;
        var message = '';
        var notifyIds = [];
        if (params.reportParams.BetType.length == 0) {
            isValid = false;
            if (message == '') {
                message = "Please select at least one bet type!";
            }
            notifyIds.push("dropdown-bettype");
        }

        if (!isValid) {
            AccReport.popUp.removeHighLight();
            AccReport.popUp.show(message, notifyIds.join(","));
        } else {
            AccReport.popUp.removeHighLight();
        }

        return isValid;
    };

    var exportReport = function () {
        AccReport.downloadFile(window.rootUrl + "Gaming/NumberGameStatistic/ExportReport", params.reportParams);
    };

    var getReportByType = function (typeId) {
        switch (typeId) {
            case Enums.ReportType.Daily:
                reportByDaily.getReport(params.reportParams);
                break;
            case Enums.ReportType.UserName:
                reportByUserName.getReport(params.reportParams);
                break;
            case Enums.ReportType.League:
                reportByLeague.getReport(params.reportParams);
                break;
            case Enums.ReportType.Bettype:
                reportByBettype.getReport(params.reportParams);
                break;
            default:
                reportByLeague.getReport(params.reportParams);
                break;
        }
    };
}

var reportManager = new Fanex.AccountingReports.Gaming.NumberGameStatistic();