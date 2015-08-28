Namespace.Register("Fanex.AccountingReports.Gaming.NumberGameMonthlyWinLossReport");

Fanex.AccountingReports.Gaming.NumberGameMonthlyWinLossReport = function () {
    var self = this;

    var _tables = {
        datepickerplugin: null,
        btnGenerate: null,
        reportContainer: null,
        report: null,
        filterNote: null
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
        renderFilterNote(noteMessage);
    };

    var cachedDOMElements = function () {
        _tables.btnGenerate = $("#btnGenerate");
        _tables.reportContainer = $("#numberGameMonthlyWinLossReportGrid");
        _tables.report = $("#numberGameMonthlyWinLossReportMainGrid");
        _tables.filterNote = $("#notemessage");
    };

    var bindingEvent = function () {
        _tables.btnGenerate.click(function () {
            self.getNumberGameMonthlyWinLoss();
        });
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
                InitDate: new Date(initFromDate),
                LabelCss: "lbl-field col_2",
                TextboxCss: "edit-field col_6"
            },
            secondDatePicker: {
                Id: "secondDatePickerId1",
                Label: labelTo,
                InitDate: new Date(initToDate),
                LabelCss: "lbl-field col_1",
                TextboxCss: "edit-field col_6"
            },
            onSelectDatePicker: function () {
            },
            maxDate: new Date(today)
        });

        _tables.datepickerplugin = $("#datepickerplugin");
        $("#firstDatePickerId1").datepicker("setDate", new Date(initFromDate));
        $("#secondDatePickerId1").datepicker("setDate", new Date(initToDate));
    };

    self.getMarginValue = function (lineTotal, turnOver) {
        if (turnOver == 0) {
            return notAvailable;
        }
        else {
            return lineTotal / turnOver;
        }
    }

    self.getNumberGameMonthlyWinLoss = function () {
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
                                    window.rootUrl + "Gaming/NumberGameMonthlyWinLoss/GetNumberGameMonthlyWinLoss",
                                    data,
                                    self.renderData,
                                    self.renderData,
                                    AccReport.enableInput);

    };

    self.renderData = function (rs) {
        var title = "Number Game Monthly Win Loss Report - From " + _param.reportParams.FromDate + " To " + _param.reportParams.ToDate;
        var columns = [
					{ HeaderText: month, Width: 150, Name: "Month", CellAlign: "center", HeaderAlign: "center", CustomCellRender: function (currentCell) {
					    return '<div class="table-cell">' + currentCell.Value.Month + '/' + currentCell.Value.Year + '</div>';
					}
					},
					{ HeaderText: memberWinLoss, Width: 150, Name: "MemberWinLoss", CellAlign: "right", HeaderAlign: "center", FormatNumber: FormatConstant.TWO_DECIMAL_NUMBER, FormatPositive: '$' },
					{ HeaderText: position, Width: 150, Name: "Position", CellAlign: "right", HeaderAlign: "center", FormatNumber: FormatConstant.TWO_DECIMAL_NUMBER },
                    { HeaderText: commission, Width: 120, Name: "Commission", CellAlign: "right", FormatNumber: FormatConstant.TWO_DECIMAL_NUMBER, HeaderAlign: "center" },
					{ HeaderText: lineTotal, Width: 150, Name: "LineTotal", CellAlign: "right", FormatNumber: FormatConstant.TWO_DECIMAL_NUMBER, HeaderAlign: "center", FormatPositive: '$' },
					{ HeaderText: turnOver, Width: 150, Name: "TurnOver", CellAlign: "right", HeaderAlign: "center", FormatNumber: FormatConstant.TWO_DECIMAL_NUMBER },
                    { HeaderText: margin, Width: 150, Name: "Margin", CellAlign: "right", HeaderAlign: "center", FormatNumber: FormatConstant.TWO_DECIMAL_PERCENT_NUMBER, FormatPositive: '$' }
				];

        var bodyData = [];
        var footerData = [];

        if (rs && rs.ReportRows && rs.ReportRows.length > 0) {
            var reportLength = rs.ReportRows.length;

            for (var rowIndex = 0; rowIndex < reportLength; rowIndex++) {
                var currentRow = rs.ReportRows[rowIndex];
                bodyData.push({
                    Columns: [
                        { Name: "Month", Value: currentRow },
                        { Name: "MemberWinLoss", Value: currentRow.Winloss },
                        { Name: "Position", Value: currentRow.Position },
                        { Name: "Commission", Value: currentRow.Commission },
                        { Name: "LineTotal", Value: currentRow.Linetotal },
                        { Name: "TurnOver", Value: currentRow.Turnover },
                        { Name: "Margin", Value: currentRow.Margin }
                    ]
                });
            }

            var marginTotal = self.getMarginValue(rs.Linetotal, rs.Turnover);

            footerData.push({
                Columns: [
                    { Name: "Month", Value: total },
                    { Name: "MemberWinLoss", Value: rs.Winloss },
                    { Name: "Position", Value: rs.Position },
                    { Name: "Commission", Value: rs.Commission },
                    { Name: "LineTotal", Value: rs.Linetotal },
                    { Name: "TurnOver", Value: rs.Turnover },
                    { Name: "Margin", Value: marginTotal }
                ]
            });
        }

        var gridOptions = {
            columns: columns,
            showFooter: true,
            showTitle: true,
            titleInfo: {
                text: title,
                showExcelButton: true,
                exportExcelFunction: function () {
                    self.ExportData();
                }
            },
            bodyRows: bodyData,
            footerRows: footerData,
            gridExpandHeight: 160
        };

        _tables.report.fanexGrid(gridOptions);
    };

    self.ExportData = function () {
        var exportParams = {
            FromDate: _param.reportParams.FromDate,
            ToDate: _param.reportParams.ToDate
        };

        AccReport.downloadFile(window.rootUrl + "Gaming/NumberGameMonthlyWinLoss/ExportReport", exportParams);
    };
};

$(document).ready(function () {
    var numberGameMonthlyWinLossReport = new Fanex.AccountingReports.Gaming.NumberGameMonthlyWinLossReport();
    numberGameMonthlyWinLossReport.initPage();
});