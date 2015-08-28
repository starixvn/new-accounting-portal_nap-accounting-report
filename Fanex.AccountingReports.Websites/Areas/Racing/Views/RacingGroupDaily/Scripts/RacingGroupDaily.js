$(document).ready(function () {
    var racingGroupDaily = new Fanex.AccountingReports.Racing.RacingGroupDaily();
    racingGroupDaily.initPage();
});

Namespace.Register("Fanex.AccountingReports.Racing.RacingGroupDaily");

Fanex.AccountingReports.Racing.RacingGroupDaily = function () {
    var self = this;

    var _table = {
        racingGroupDaily: null,
        racingGroupDailyContent: null,
        btnGenerate: null,
        datePickerPlugin: null
    };

    var _params = {
        fromDate: null,
        toDate: null,
        isPagingCallBack: null,
        startIndex: null,
        datesPerPage: null
    };

    var _gridOption = {
        datesPerPage: 50,
        currentPage: 0,
        startIndex: 0
    };

    self.initPage = function () {
        catchedDOMElement();
        renderDatePicker();
        bindingEvent();
    };

    var catchedDOMElement = function () {
        _table.racingGroupDaily = $('#RacingGroupDailyContainer');
        _table.racingGroupDailyContent = $('#RacingGroupDailyContent');
        _table.btnGenerate = $('#btnGenerate');
        _table.datePickerPlugin = $('#datepickerplugin');
    };

    var renderDatePicker = function () {
        var maxDate = new Date();
        _table.datePickerPlugin.nexDatePicker({
            nexDatePickerType: "daterange",
            datePickerFormat: {
                changeMonth: true,
                changeYear: true,
                dateFormat: "M dd, yy",
                maxDate: maxDate
            },
            firstDatePicker: {
                Id: "firstDatePickerId1",
                Label: labelFrom,
                InitDate: today,
                LabelCss: "lbl-field col_2",
                TextboxCss: "edit-field col_6"
            },
            secondDatePicker: {
                Id: "secondDatePickerId1",
                Label: labelTo,
                InitDate: today,
                LabelCss: "lbl-field col_1",
                TextboxCss: "edit-field col_6"
            },
            onSelectDatePicker: function () {
            }
        });
    };

    var bindingEvent = function () {
        _table.btnGenerate.click(function () {
            generateReport();
        });
    };

    var generateReport = function () {
        resetParams();
        resetPagerOptions();
        getReport();
    };

    var getReport = function () {
        AccReport.disableInput();
        getParams();
        if (validateParams() == true) {
            $.ajax({
                type: "POST",
                url: window.rootUrl + "Racing/RacingGroupDaily/GetReport",
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                data: JSON.stringify(_params),
                beforeSend: function () {
                    AccReport.disableInput();
                },
                success: function (report) {
                    AccReport.popUp.hide();
                    self.renderReport(report);
                },
                error: function (jqXHR, event, ajaxSettings) {
                    if (jqXHR.status &&
                            jqXHR.status == AccReportData.Constants.HtmlErrorCode.SystemError) {
                        var r = jQuery.parseJSON(jqXHR.responseText);
                        AccReport.popUp.show(r.message);
                    }
                    self.renderReport(null);
                },
                complete: function () {
                    AccReport.enableInput();
                }
            });
        }
        else {
            AccReport.enableInput();
        }
    };

    var getParams = function () {
        var dateRange = _table.datePickerPlugin.nexDatePicker("getDateRange");
        _params.fromDate = dateRange.FromDate;
        _params.toDate = dateRange.ToDate;

        if (_params.datesPerPage == null) {
            _params.datesPerPage = 50;
        }
    };

    var validateParams = function () {
        return true;
    };

    var resetParams = function () {
        _params.isPagingCallBack = false;
        _params.startIndex = null;
        _params.datesPerPage = null;
    };

    var resetPagerOptions = function () {
        _gridOption.startIndex = 0;
        _gridOption.currentPage = 0;
        _gridOption.datesPerPage = 50;
    };

    self.renderReport = function (report) {
        var reportTitle = "Racing Group Daily - From " + _params.fromDate + " To " + _params.toDate;
        var totalDate = 0;

        if (report != null && report.length >= 2) {
            var bodyData = [];
            var footerData = [];

            // save total win lost date in win lost date of element partner total of report.
            totalDate = report[report.length - 1].WinLostDate;

            bodyData = renderDataInDate(report, bodyData);
            footerData = renderFooter(report, footerData);

            renderGrid(reportTitle, bodyData, footerData, totalDate);
        }
        else {
            renderGrid(reportTitle, [], [], totalDate);
        }
    };

    var renderDataInDate = function (report, bodyData) {
        var numberDates = report.length - 1;
        var partnerTotal = report[numberDates];

        for (var i = 0; i < numberDates; i++) {
            var date = report[i];
            bodyData = renderDate(date, bodyData);
        }

        bodyData = renderPartnerTotal(partnerTotal, bodyData);

        return bodyData;
    };

    var renderDate = function (date, bodyData) {
        if (date != null && date.Rows != null && date.Rows.length > 0) {
            var numberRowsInDate = date.Rows.length;
            var columns = [];

            for (var i = 0; i < numberRowsInDate; i++) {
                var row = date.Rows[i];
                columns = renderRow(row, row.Name, 1, "");

                if (i == 0) {
                    columns.splice(
                           0,
                           0,
                           { Name: "Date", Value: date.WinLostDate, MergeRowNumber: numberRowsInDate });
                }

                bodyData.push({
                    Columns: columns
                });
            }

            columns = renderRow(date, "Total", 2, "date-total");
            bodyData.push({
                Columns: columns
            });
        }

        return bodyData;
    };

    var renderPartnerTotal = function (partnerTotal, bodyData) {
        if (partnerTotal != null && partnerTotal.Rows != null && partnerTotal.Rows.length > 0) {
            var numberPartnerTotal = partnerTotal.Rows.length;

            for (var i = 0; i < numberPartnerTotal; i++) {
                var partner = partnerTotal.Rows[i];
                var nameOfPartner = partner.Name + " Total";

                var columns = renderRow(partner, nameOfPartner, 2, "partner-total");
                bodyData.push({
                    Columns: columns
                });
            }
        }

        return bodyData;
    };

    var renderFooter = function (report, footerData) {
        var grandTotal = report[report.length - 1];
        var columns = renderRow(grandTotal, "Grand Total", 2, "");

        footerData.push({
            Columns: columns
        });

        return footerData;
    };

    var renderRow = function (row, name, numberColMerger, tdClass) {
        var columns = [
            { Name: "Name", Value: name, MergeColNumber: numberColMerger, TdClass: tdClass },
            { Name: "TurnoverAgent", Value: row.TurnoverAgent, TdClass: tdClass },
            { Name: "TurnoverIBC", Value: row.TurnoverIBC, TdClass: tdClass },
            { Name: "PartnerTurnover", Value: row.PartnerTurnover, TdClass: tdClass },
            { Name: "TotalTurnover", Value: row.TotalTurnover, TdClass: tdClass },
            { Name: "IncomeIBCPostitionTaking", Value: row.IncomeIBCPostitionTaking, TdClass: tdClass },
            { Name: "IncomeIBCNetCommission", Value: row.IncomeIBCNetCommission, TdClass: tdClass },
            { Name: "TotalIBCIncome", Value: row.TotalIBCIncome, TdClass: tdClass },
            { Name: "Income12BETPositionTaking", Value: row.Income12BETPositionTaking, TdClass: tdClass },
            { Name: "Income12BETCommission", Value: row.Income12BETCommission, TdClass: tdClass },
            { Name: "Total12BETIncome", Value: row.Total12BETIncome, TdClass: tdClass },
            { Name: "GroupTotalIncome", Value: row.GroupTotalIncome, TdClass: tdClass },
            { Name: "Turnover3rd", Value: row.PartnerTurnover, TdClass: tdClass },
            { Name: "PartnerDividendWon", Value: row.PartnerDividendWon, TdClass: tdClass },
            { Name: "PartnerExpenseCommission", Value: row.PartnerExpenseCommission, TdClass: tdClass },
            { Name: "PartnerFee", Value: row.PartnerFee, TdClass: tdClass },
            { Name: "PartnerAmountDue", Value: row.PartnerAmountDue, TdClass: tdClass }
        ];

        return columns;
    };

    var renderGrid = function (reportTitle, bodyData, footerData, totalDate) {
        var groupHeader = [
                {
                    Columns: [
                        { HeaderText: "", Name: "Date", MergeColNumber: 2 },
                        { HeaderText: "Turnover (RM)", Name: "TurnoverAgent", MergeColNumber: 4 },
                        { HeaderText: "IBC (RM)", Name: "IncomeIBCPostitionTaking",
                            MergeColNumber: 3
                        },
                        { HeaderText: "12Bet (RM)", Name: "Income12BETPositionTaking",
                            MergeColNumber: 3
                        },
                        { HeaderText: "Group Total Income", Name: "GroupTotalIncome",
                            MergeRowNumber: 2
                        },
                        { HeaderText: "Partner (RM)", Name: "Turnover3rd",
                            MergeColNumber: 5
                        }
                    ]
                }
            ];
        var columns = getHeaderColumns();

        var gridOptions = {
            customizeHeader: true,
            groupHeaderRows: groupHeader,
            columns: columns,
            showTitle: true,
            titleInfo: {
                text: reportTitle,
                showExcelButton: true,
                exportExcelFunction: function () {
                    self.exportReport();
                }
            },
            bodyRows: bodyData,
            showFooter: true,
            footerRows: footerData,
            gridExpandHeight: 190,
            numberOfFrozenColumn: 2,
            showPager: true,
            customPager: true,
            pagerOption: {
                startPerPage: 50,
                totalItem: totalDate,
                itemsPerPageArray: [50, 100, 200, 500],
                itemsPerPage: _gridOption.datesPerPage,
                currentPage: _gridOption.currentPage,
                pagerSelectCallBack: ReRenderAfterPaged
            }
        };

        _table.racingGroupDailyContent.removeClass("display-none").fanexGrid(gridOptions);

        removeNoNeedClassAndAttr();

        function ReRenderAfterPaged(pageIndex, itemsPerPage, jq) {
            _params.isPagingCallBack = true;
            _params.startIndex = pageIndex >= 0 ? (pageIndex * itemsPerPage) : 0;
            _params.datesPerPage = itemsPerPage;

            _gridOption.datesPerPage = itemsPerPage;
            _gridOption.currentPage = pageIndex;
            _gridOption.startIndex = _params.startIndex;

            getReport();
            return false;
        }
    };

    var removeNoNeedClassAndAttr = function () {
        _table.racingGroupDailyContent.find('tr').removeClass("highlight-row");
        _table.racingGroupDailyContent.find('tr').removeAttr("highlightatt");
        _table.racingGroupDailyContent.find('tr').removeClass("freegrid-even");
    };

    var getHeaderColumns = function () {
        var columns = [
            { HeaderText: "Date", Width: 120, Name: "Date", HeaderAlign: "center",
                CellAlign: "center"
            },
            { HeaderText: "PartnerType", Width: 150, Name: "Name", HeaderAlign: "center",
                CellAlign: "center"
            },
            { HeaderText: "Agency", Width: 120, Name: "TurnoverAgent", HeaderAlign: "center",
                CellAlign: "right", FormatNumber: FormatConstant.TWO_DECIMAL_NUMBER
            },
            { HeaderText: "IBC", Width: 120, Name: "TurnoverIBC", HeaderAlign: "center",
                CellAlign: "right", FormatNumber: FormatConstant.TWO_DECIMAL_NUMBER
            },
            { HeaderText: "Partner", Width: 120, Name: "PartnerTurnover", HeaderAlign: "center",
                CellAlign: "right", FormatNumber: FormatConstant.TWO_DECIMAL_NUMBER
            },
            { HeaderText: "Total TO", Width: 120, Name: "TotalTurnover", HeaderAlign: "center",
                CellAlign: "right", FormatNumber: FormatConstant.TWO_DECIMAL_NUMBER
            },
            { HeaderText: "PT", Width: 120, Name: "IncomeIBCPostitionTaking", HeaderAlign: "center",
                CellAlign: "right", FormatNumber: FormatConstant.TWO_DECIMAL_NUMBER
            },
            { HeaderText: "Net Comm", Width: 120, Name: "IncomeIBCNetCommission", HeaderAlign: "center",
                CellAlign: "right", FormatNumber: FormatConstant.TWO_DECIMAL_NUMBER
            },
            { HeaderText: "Total Income", Width: 120, Name: "TotalIBCIncome", HeaderAlign: "center",
                CellAlign: "right", FormatNumber: FormatConstant.TWO_DECIMAL_NUMBER
            },
            { HeaderText: "PT", Width: 120, Name: "Income12BETPositionTaking", HeaderAlign: "center",
                CellAlign: "right", FormatNumber: FormatConstant.TWO_DECIMAL_NUMBER
            },
            { HeaderText: "Comm", Width: 120, Name: "Income12BETCommission", HeaderAlign: "center",
                CellAlign: "right", FormatNumber: FormatConstant.TWO_DECIMAL_NUMBER
            },
            { HeaderText: "Total Income", Width: 120, Name: "Total12BETIncome", HeaderAlign: "center",
                CellAlign: "right", FormatNumber: FormatConstant.TWO_DECIMAL_NUMBER
            },
            { HeaderText: "Group Total Income", Width: 120, Name: "GroupTotalIncome", HeaderAlign: "center",
                CellAlign: "right", FormatNumber: FormatConstant.TWO_DECIMAL_NUMBER
            },
            { HeaderText: "Turnover", Width: 120, Name: "Turnover3rd", HeaderAlign: "center",
                CellAlign: "right", FormatNumber: FormatConstant.TWO_DECIMAL_NUMBER
            },
            { HeaderText: "Dividend Won", Width: 120, Name: "PartnerDividendWon", HeaderAlign: "center",
                CellAlign: "right", FormatNumber: FormatConstant.TWO_DECIMAL_NUMBER
            },
            { HeaderText: "Comm Expense", Width: 120, Name: "PartnerExpenseCommission",
                HeaderAlign: "center", CellAlign: "right",
                FormatNumber: FormatConstant.TWO_DECIMAL_NUMBER
            },
            { HeaderText: "Fee", Width: 120, Name: "PartnerFee", HeaderAlign: "center",
                CellAlign: "right", FormatNumber: FormatConstant.TWO_DECIMAL_NUMBER
            },
            { HeaderText: "Amount Due", Width: 120, Name: "PartnerAmountDue", HeaderAlign: "center",
                CellAlign: "right", FormatNumber: FormatConstant.TWO_DECIMAL_NUMBER
            }
            ];

        return columns;
    };

    self.exportReport = function () {
        AccReport.downloadFile(window.rootUrl + "Racing/RacingGroupDaily/ExportReport", _params);
    };
};