Namespace.Register("Fanex.AccountingReports.Gaming.NumberGameFinancial");

Fanex.AccountingReports.Gaming.NumberGameFinancial = function () {
    var self = this;

    var _tables = {
        datepickerplugin: null,
        btnGenerate: null,
        reportContainer: null,
        report: null
    };

    var _param = {
        reportParams: {
            FromDate: null,
            ToDate: null
        }
    };

    self.initPage = function () {
        cachedDOMElements();
        renderDatePicker();
        bindingEvent();
    };

    var cachedDOMElements = function () {
        _tables.btnGenerate = $("#btnGenerate");
        _tables.reportContainer = $("#NumberGameFinancialGrid");
        _tables.report = $("#NumberGameFinancialMainGrid");
    };

    var bindingEvent = function () {
        _tables.btnGenerate.click(function () {
            self.getNumberGameFinancial();
        });
    };

    var renderDatePicker = function () {
        $('#datepickerplugin').nexDatePicker({
            nexDatePickerType: "daterange",
            datePickerFormat: {
                changeMonth: true,
                changeYear: true,
                dateFormat: FormatConstant.M_DD_YY_STRING
            },
            firstDatePicker: {
                Id: "firstDatePickerId1",
                Label: labelFrom,
                InitDate: fromdate,
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
            },
            maxDate: new Date(today)
        });

        _tables.datepickerplugin = $("#datepickerplugin");
    };

    self.getNumberGameFinancial = function () {
        var dateRange = _tables.datepickerplugin.nexDatePicker("getDateRange");
        _param.reportParams.FromDate = dateRange.FromDate;
        _param.reportParams.ToDate = dateRange.ToDate;

        AccReport.disableInput();

        var data = {
            FromDate: _param.reportParams.FromDate,
            ToDate: _param.reportParams.ToDate
        };

        AccReport.disableInput();

        AccReport.utility.CallAjax(this,
            window.rootUrl + "Gaming/NumberGameFinancial/GetNumberGameFinancial",
            data,
            self.renderData,
            self.renderData,
            AccReport.enableInput);

    };

    self.renderData = function (rs) {
        var title = "Number Game Financial Report - From " + _param.reportParams.FromDate + " To " + _param.reportParams.ToDate;
        var columns = [{ HeaderText: date, Width: 237, Name: "Bettype", CellAlign: "left", ShowCollapseExpand: true, HeaderAlign: "center" },
			{ HeaderText: numberPlayer, Width: 120, Name: "NumberPlayer", CellAlign: "right", HeaderAlign: "center", FormatNumber: FormatConstant.INT_NUMBER },
			{ HeaderText: numberGame, Width: 120, Name: "NumberGame", CellAlign: "right", HeaderAlign: "center", FormatNumber: FormatConstant.INT_NUMBER },
            { HeaderText: totalBet, Width: 150, Name: "TotalBets", CellAlign: "right", FormatNumber: FormatConstant.TWO_DECIMAL_NUMBER, HeaderAlign: "center" },
			{ HeaderText: totalWin, Width: 150, Name: "TotalWins", CellAlign: "right", FormatNumber: FormatConstant.TWO_DECIMAL_NUMBER, HeaderAlign: "center" },
			{ HeaderText: gameIncome, Width: 150, Name: "GameIncome", CellAlign: "right", HeaderAlign: "center", FormatNumber: FormatConstant.TWO_DECIMAL_NUMBER, FormatNegative: '$' },
            {
                HeaderText: payout,
                Width: 120,
                Name: "Payout",
                CellAlign: "right",
                HeaderAlign: "center",
                FormatNumber: FormatConstant.TWO_DECIMAL_PERCENT_NUMBER,
                CustomCellRender: function (currentCell) {
                    if (currentCell.Value.Payout > 1) {
                        return '<span class="format-negative">' + numeral(currentCell.Value.Payout).format('0.00%') + '</span>';
                    }
                    else {
                        return numeral(currentCell.Value.Payout).format('0.00%');
                    }
                }
            }];

        var bodyData = [];
        var footerData = [];

        if (rs && rs.ReportRows.length > 0) {
            var reportDateLength = rs.ReportRows.length;

            for (var dateRowIndex = 0; dateRowIndex < reportDateLength; dateRowIndex++) {
                var currentDate = rs.ReportRows[dateRowIndex];
                var currentDateRow = {
                    Columns: [{ Name: "Bettype", Value: currentDate.ReportDate },
                        { Name: "NumberPlayer", Value: currentDate.NumberPlayer },
                        { Name: "NumberGame", Value: currentDate.NumberGame },
                        { Name: "TotalBets", Value: currentDate.TotalBet },
                        { Name: "TotalWins", Value: currentDate.TotalWin },
                        { Name: "GameIncome", Value: currentDate.GameIncome },
                        { Name: "Payout", Value: currentDate}],
                    SubRows: []
                };

                if (currentDate.ReportRows.length > 0) {
                    var statusLength = currentDate.ReportRows.length;

                    for (var statusRowIndex = 0; statusRowIndex < statusLength; statusRowIndex++) {
                        var currentStatus = currentDate.ReportRows[statusRowIndex];
                        var currentStatusRow = {
                            Columns: [{ Name: "Bettype", Value: currentStatus.Status },
                                { Name: "NumberPlayer", Value: currentStatus.NumberPlayer },
                                { Name: "NumberGame", Value: currentStatus.NumberGame },
                                { Name: "TotalBets", Value: currentStatus.TotalBet },
                                { Name: "TotalWins", Value: currentStatus.TotalWin },
                                { Name: "GameIncome", Value: currentStatus.GameIncome },
                                { Name: "Payout", Value: currentStatus}],
                            SubRows: []
                        };

                        if (currentStatus.ReportRows.length > 0) {
                            var bettypeLength = currentStatus.ReportRows.length;

                            for (var bettypeRowIndex = 0; bettypeRowIndex < bettypeLength; bettypeRowIndex++) {
                                var currentBettype = currentStatus.ReportRows[bettypeRowIndex];
                                var currentBettypeRow = {
                                    Columns: [{ Name: "Bettype", Value: currentBettype.Bettype },
                                       { Name: "NumberPlayer", Value: currentBettype.NumberPlayer },
                                       { Name: "NumberGame", Value: currentBettype.NumberGame },
                                       { Name: "TotalBets", Value: currentBettype.TotalBet },
                                       { Name: "TotalWins", Value: currentBettype.TotalWin },
                                       { Name: "GameIncome", Value: currentBettype.GameIncome },
                                       { Name: "Payout", Value: currentBettype}]
                                };

                                currentStatusRow.SubRows.push(currentBettypeRow);
                            }
                        }

                        currentDateRow.SubRows.push(currentStatusRow);
                    }
                }

                bodyData.push(currentDateRow);
                footerData = [{
                    Columns: [{ Name: "Bettype", Value: grandTotal },
					    { Name: "NumberPlayer", Value: rs.NumberPlayer },
                        { Name: "NumberGame", Value: rs.NumberGame },
                        { Name: "TotalBets", Value: rs.TotalBet },
                        { Name: "TotalWins", Value: rs.TotalWin },
                        { Name: "GameIncome", Value: rs.GameIncome },
                        { Name: "Payout", Value: rs.Payout}]
                }];
            }
        }

        var gridOption = {
            columns: columns,
            showFooter: true,
            showTitle: true,
            titleInfo:
				{
				    text: title,
				    showExcelButton: true,
				    exportExcelFunction: function () {
				        self.ExportData();
				    }
				},
            showTree: true,
            bodyRows: bodyData,
            footerRows: footerData,
            gridExpandHeight: 160,
            showDistinctRow: false,
            marginRight: 20
        };

        _tables.report.fanexGrid(gridOption);
    };

    self.ExportData = function () {
        var exportParams = {
            FromDate: _param.reportParams.FromDate,
            ToDate: _param.reportParams.ToDate
        };

        AccReport.downloadFile(window.rootUrl + "Gaming/NumberGameFinancial/ExportReport", exportParams);
    }
};

$(document).ready(function () {
    var NumberGameFinancial = new Fanex.AccountingReports.Gaming.NumberGameFinancial();
    NumberGameFinancial.initPage();
});