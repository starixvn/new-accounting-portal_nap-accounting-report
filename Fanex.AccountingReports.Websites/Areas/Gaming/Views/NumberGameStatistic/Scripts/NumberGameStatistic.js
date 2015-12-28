$(document).ready(function () {
    reportManager.initPage();
});

Namespace.Register("Fanex.AccountingReports.Gaming.NumberGameStatistic");
Fanex.AccountingReports.Gaming.NumberGameStatistic = function () {
    var self = this;

    var Enums = {
        DateOption: {
            Last3Months: 8,
            Last6Months: 9,
            Today: 0,
            Yesterday: 1
        }
    };

    var _tables = {
        btnGenerate: null,
        btnToday: null,
        btnYesterday: null,

        datepickerplugin: null,
        dateOptionSelector: null,
        firstDatePickerId1: null,
        secondDatePickerId1: null,
        last3MonthsDatePicker: null,
        last6MonthsDatePicker: null,

        reportContainer: null,
        reportContent: null,
        filterNote: null,
        firstDatePicker: null,
        secondDatePicker: null
    };

    var params = {
        reportParams: {
            FromDate: new Date(),
            ToDate: new Date(),
            MoveBetsDay: new Date(),
            CustId: null,
            LeagueId: null,
            BetType: '',
            ReportBy: 0
        }
    };

    self.initPage = function () {
        cachedDOMElements();
        _tables.btnToday.val(shortFormattedToday);
        _tables.btnYesterday.val(shortFormattedYesterday);
        renderDatePicker();
        bindingEvent();
    };

    var cachedDOMElements = function () {
        _tables.btnGenerate = $("#btnGenerate");
        _tables.btnToday = $("#btnToday");
        _tables.btnYesterday = $("#btnYesterday");
        _tables.reportContainer = $("#reportContainer");
        _tables.reportContent = $("#reportContent");
        _tables.filterNote = $("#notemessage");
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
                LabelCss: "lbl-field col_5 align-left",
                TextboxCss: "edit-field col_6"
            },
            secondDatePicker: {
                Id: "secondDatePickerId1",
                Label: "To Date",
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

        _tables.datepickerplugin = $("#datepickerplugin");
        _tables.firstDatePickerId1 = $("#firstDatePickerId1");
        _tables.secondDatePickerId1 = $("#secondDatePickerId1");
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

    var bindingEvent = function () {
        _tables.btnGenerate.unbind("click");
        _tables.btnGenerate.click(function () {
            getReport();
        });

        _tables.btnToday.click(function () {
            _tables.secondDatePicker.datepicker("option", "minDate", new Date(today));
            _tables.firstDatePicker.datepicker("option", "maxDate", new Date(today));
            _tables.firstDatePicker.datepicker("setDate", new Date(today));
            _tables.secondDatePicker.datepicker("setDate", new Date(today));

            getReport();
        });

        _tables.btnYesterday.click(function () {
            _tables.secondDatePicker.datepicker("option", "minDate", new Date(yesterday));
            _tables.firstDatePicker.datepicker("option", "maxDate", new Date(yesterday));
            _tables.firstDatePicker.datepicker("setDate", new Date(yesterday));
            _tables.secondDatePicker.datepicker("setDate", new Date(yesterday));

            getReport();
        });
    };

    var getReport = function () {
        AccReport.disableInput();
        getReportParams();
        AccReport.utility.CallAjax(this,
                                    window.rootUrl + "Gaming/NumberGameStatistic/GetReport",
                                    params.reportParams,
                                    renderReport,
                                    renderReport,
                                    AccReport.enableInput);
    };

    var renderReport = function (result) {
        _tables.reportContainer.removeClass("display-none");
        _tables.reportContent.addClass("display-none");
        var title = "Number Game Statistic Report From " + params.reportParams.FromDate + " To " + params.reportParams.ToDate;
        var columns = [
            { HeaderText: "League", Width: 200, Name: "League", HeaderAlign: "center", CellAlign: "center" },
            { HeaderText: "No.Players", Width: 100, Name: "NumberPlayers", HeaderAlign: "center", CellAlign: "right", FormatNumber: FormatConstant.INT_NUMBER },
            { HeaderText: "Bet Count", Width: 100, Name: "BetCounts", HeaderAlign: "center", CellAlign: "right", FormatNumber: FormatConstant.INT_NUMBER },
            { HeaderText: "Total Bets", Width: 120, Name: "TotalBets", HeaderAlign: "center", CellAlign: "right", FormatNumber: FormatConstant.TWO_DECIMAL_NUMBER },
            { HeaderText: "Total Wins", Width: 120, Name: "TotalWins", HeaderAlign: "center", CellAlign: "right", FormatNumber: FormatConstant.TWO_DECIMAL_NUMBER },
            { HeaderText: "Game Income", Width: 120, Name: "GameIncome", HeaderAlign: "center", CellAlign: "right", FormatNumber: FormatConstant.TWO_DECIMAL_NUMBER },
            { HeaderText: "Game Payout", Width: 120, Name: "GamePayout", HeaderAlign: "center", CellAlign: "right", FormatNumber: FormatConstant.TWO_DECIMAL_NUMBER },
            { HeaderText: "Company Turnover", Width: 120, Name: "CompanyTurnover", HeaderAlign: "center", CellAlign: "right", FormatNumber: FormatConstant.TWO_DECIMAL_NUMBER },
            { HeaderText: "Comapny Net Income", Width: 150, Name: "CompanyNetIncome", HeaderAlign: "center", CellAlign: "right", FormatNumber: FormatConstant.TWO_DECIMAL_NUMBER }
        ];

        var bodyData = [];
        var footerData = [];

        var reportLength = result.ReportRows.length;
        if (result && reportLength > 0) {
            for (var index = 0; index < reportLength; index++) {
                var currentItem = result.ReportRows[index];
                var currentDataRow = [];
                currentDataRow.push(
                    { Name: "League", Value: currentItem.LeagueName },
                    { Name: "NumberPlayers", Value: currentItem.Players },
                    { Name: "BetCounts", Value: currentItem.BetCount },
                    { Name: "TotalBets", Value: currentItem.TotalBets },
                    { Name: "TotalWins", Value: currentItem.TotalWins },
                    { Name: "GameIncome", Value: currentItem.GameIncome },
                    { Name: "GamePayout", Value: currentItem.GamePayout },
                    { Name: "CompanyTurnover", Value: currentItem.CompanyTurnover },
                    { Name: "CompanyNetIncome", Value: currentItem.NetIncome }
                );

                bodyData.push({
                    Columns: currentDataRow
                });
            }

            footerData.push({
                Columns: [
                    { Name: "League", Value: "Grand Total" },
                    { Name: "NumberPlayers", Value: result.GrandTotal.Players },
                    { Name: "BetCounts", Value: result.GrandTotal.BetCount },
                    { Name: "TotalBets", Value: result.GrandTotal.TotalBets },
                    { Name: "TotalWins", Value: result.GrandTotal.TotalWins },
                    { Name: "GameIncome", Value: result.GrandTotal.GameIncome, TdClass: "highlight-column" },
                    { Name: "GamePayout", Value: result.GrandTotal.GamePayout },
                    { Name: "CompanyTurnover", Value: result.GrandTotal.CompanyTurnover },
                    { Name: "CompanyNetIncome", Value: result.GrandTotal.NetIncome, TdClass: "highlight-column" }
                ]
            });
        }

        var gridOptions = {
            columns: columns,
            bodyRows: bodyData,
            footerRows: footerData,
            gridExpandHeight: 260,
            showDistinctRow: true,
            showHighlightRow: false,
            showTitle: true,
            showFooter: true,
            titleInfo: {
                text: title,
                showExcelButton: true,
                exportExcelFunction: function () {
                    exportReport();
                }
            }
        };

        _tables.reportContent.removeClass("display-none").fanexGrid(gridOptions);
    };

    var getReportParams = function () {
        var dateRange = _tables.datepickerplugin.nexDatePicker("getDateRange");
        params.reportParams.FromDate = dateRange.FromDate;
        params.reportParams.ToDate = dateRange.ToDate;

        return params.reportParams;
    };

    var resetReportParams = function () {
    };

    var exportReport = function () {
        AccReport.downloadFile(window.rootUrl + "Gaming/NumberGameStatistic/ExportReport", params.reportParams);
    };


}

var reportManager = new Fanex.AccountingReports.Gaming.NumberGameStatistic();