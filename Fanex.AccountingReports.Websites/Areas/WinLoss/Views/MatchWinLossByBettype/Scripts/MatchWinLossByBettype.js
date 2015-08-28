Namespace.Register("Fanex.AccountingReports.WinLoss.MatchWinLossByBettype");

Fanex.AccountingReports.WinLoss.MatchWinLossByBettype = function () {
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
        _tables.reportContainer = $("#matchWinLossByBettypeReportGrid");
        _tables.report = $("#matchWinLossByBettypeReportMainGrid");
    };

    var bindingEvent = function () {
        _tables.btnGenerate.click(function () {
            self.getMatchWinLossByBettype();
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
            },
            minDate: new Date(minDate),
            maxDate: new Date(today)
        });

        _tables.datepickerplugin = $("#datepickerplugin");
    };

    self.getMatchWinLossByBettype = function () {
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
                                    window.rootUrl + "WinLoss/MatchWinLossByBettype/GetMatchWinLossByBettype",
                                    data,
                                    self.renderData,
                                    self.renderData,
                                    AccReport.enableInput);

    };

    self.renderData = function (rs) {
        var title = "Match Win Loss By Bet Type  Report - From " + _param.reportParams.FromDate + " To " + _param.reportParams.ToDate;

        var columns = [
					{ HeaderText: noTitle, Width: 50, Name: "No", CellAlign: "center", HeaderAlign: "center" },
					{ HeaderText: bettype, Width: 150, Name: "Bettype", CellAlign: "center", HeaderAlign: "center", CustomCellRender: function (currentCell) {
					    if (currentCell.Value.IsLive) {
					        return '<span class="format-positive bold">' + liveIndicator + breakText + hyphen + '</span>' + breakText + currentCell.Value.Bettype;
					    }
					    else {
					        return currentCell.Value.Bettype;
					    }
					}
					},
					{ HeaderText: match, Width: 150, Name: "HomeTeam", CellAlign: "right", HeaderAlign: "center", MergeHeaderColNumber: 3 },
                    { HeaderText: "", Width: 35, Name: "ColVs", CellAlign: "center", HeaderAlign: "center" },
                    { HeaderText: "", Width: 150, Name: "AwayTeam", CellAlign: "left", HeaderAlign: "center" },
                    { HeaderText: baseCurrency, Width: 150, Name: "BaseCurrency", CellAlign: "right", FormatNumber: FormatConstant.TWO_DECIMAL_NUMBER, HeaderAlign: "center" }
				];

        var bodyData = [];
        var footerData = [];

        if (rs && rs.ReportRows && rs.ReportRows.length > 0) {
            var reportLength = rs.ReportRows.length;

            for (var rowIndex = 0; rowIndex < reportLength; rowIndex++) {
                var currentRow = rs.ReportRows[rowIndex];

                bodyData.push({
                    Columns: [
                        { Name: "No", Value: rowIndex + 1 },
                        { Name: "Bettype", Value: currentRow },
                        { Name: "HomeTeam", Value: currentRow.HomeTeam },
                        { Name: "ColVs", Value: "-vs-" },
                        { Name: "AwayTeam", Value: currentRow.AwayTeam },
                        { Name: "BaseCurrency", Value: currentRow.WinLoss, CssClass: "bold" }
                    ]
                });
            }

            footerData.push({
                Columns: [
                    { Name: "No", Value: grandTotal, MergeColNumber: 5 },
                    { Name: "BaseCurrency", Value: rs.TotalWinLoss }
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
            gridExpandHeight: 145 + AccReportData.Constants.ReportMarginBottom,
            showPager: true,
            pagerOption: {
                itemsPerPage: 50,
                currentPage: 0,
                itemsPerPageArray: [50, 100, 200, 500],
                totalItem: bodyData.length
            }
        };

        _tables.report.fanexGrid(gridOptions);
    };

    self.ExportData = function () {
        var exportParams = {
            FromDate: _param.reportParams.FromDate,
            ToDate: _param.reportParams.ToDate
        };

        AccReport.downloadFile(window.rootUrl + "WinLoss/MatchWinLossByBettype/ExportReport", exportParams);
    }
};

$(document).ready(function () {
    var matchWinLossByBettypeReport = new Fanex.AccountingReports.WinLoss.MatchWinLossByBettype();
    matchWinLossByBettypeReport.initPage();
});