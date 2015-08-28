Namespace.Register("Fanex.AccountingReports.WinLoss.MatchWinLossSingleBet");

$(document).ready( function (){
    var matchSingleWinLossSingleBet = new Fanex.AccountingReports.WinLoss.MatchWinLossSingleBet();
    matchSingleWinLossSingleBet.initPage();
});

Fanex.AccountingReports.WinLoss.MatchWinLossSingleBet = function () {
    var self = this;

    var _table = {
        report: null,
        reportContent: null,
        datePickerPlugin: null,
        btnGenerate: null
    };

    var _params = {
        fromDate: null,
        toDate: null
    };

    self.initPage = function () {
        catchedDOMElement();
        renderDatePicker();
        bindingEvent();
    };

    var catchedDOMElement = function () {
        _table.report = $("#MatchWinLossSingleBetContainer");
        _table.reportContent = $("#MatchWinLossSingleBetContent");
        _table.btnGenerate = $("#btnGenerate");
        _table.datePickerPlugin = $("#datepickerplugin");
    };

    var renderDatePicker = function () {
        // set max date is today.
        var maxDate = new Date();
        // set min date is the day before today 3 months.
        var minDate = new Date();
        var month = maxDate.getMonth();
        if (month >= 3) {
            minDate.setMonth(maxDate.getMonth() - 3);
            minDate.setDate(maxDate.getDate());
        }
        // if the day belong to Jan, Feb, Mar.
        else {
            minDate.setYear(maxDate.getFullYear() - 1);
            var minMonth = 12 - (3 - month);
            minDate.setMonth(minMonth);
            minDate.setDate(maxDate.getDate());
        }

        $('#datepickerplugin').nexDatePicker({
            nexDatePickerType: "daterange",
            datePickerFormat: {
                changeMonth: true,
                changeYear: true,
                dateFormat: "M dd, yy",
                maxDate: maxDate,
                minDate: minDate
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
            self.getReport();
        });
    };

    self.getReport = function () {
        AccReport.disableInput();
        getParams();

        if (validateParams() == true) {
            $.ajax({
                type: "POST",
                url: window.rootUrl + "WinLoss/MatchWinLossSingleBet/GetReport",
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
                        var responseText = jQuery.parseJSON(jqXHR.responseText);
                        AccReport.popUp.show(responseText.message);
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
        _params.fromDate = dateRange.FromDate + " 00:00:00";
        _params.toDate = dateRange.ToDate + " 23:59:59";
    };

    var validateParams = function () {
        return true;
    }

    self.renderReport = function (report) {
        var reportTitle = "Match Win Loss Single Bet from " + _params.fromDate.replace("00:00:00","") 
                                                 + " to " + _params.toDate.replace("23:59:59","");

        if (report != null && report.Rows != null && report.Rows.length > 0) {
            var numberOfMatchs = report.Rows.length;
            var bodyData = [];
            var footerData = [];

            for (var i = 0; i < numberOfMatchs; i++) {
                var match = report.Rows[i];
                bodyData.push({
                    Columns: [
                        { Name: "Number", Value: i + 1 },
                        { Name: "HomeTeam", Value: match.HomeName },
                        { Name: "Versus", Value: "-vs-" },
                        { Name: "AwayTeam", Value: match.AwayName },
                        { Name: "BaseCurrency",
                            Value: numeral(match.MatchWinLost).format(FormatConstant.TWO_DECIMAL_NUMBER)
                        }
                    ]
                });
            }

            footerData.push({
                Columns: [
                    { Name: "Number", Value: "GRAND TOTAL", MergeColNumber: 4 },
                    { Name: "BaseCurrency",
                        Value: numeral(report.MatchWinLost).format(FormatConstant.TWO_DECIMAL_NUMBER) 
                    }
                ]
            });

            self.renderGrid(bodyData, footerData, reportTitle);
        }
        else {
            self.renderGrid([], [], reportTitle);
        }

        _table.report.removeClass("display-none");
    };

    self.renderGrid = function (bodyData, footerData, reportTitle) {
        var columns = [
            { HeaderText: "No", Name: "Number", Width: 60, HeaderAlign: "center", CellAlign: "center" },
            { HeaderText: "HomeName", Name: "HomeTeam", Width: 250, HeaderAlign: "center",
                CellAlign: "center"
            },
            { HeaderText: "Versus", Name: "Versus", Width: 20, HeaderAlign: "center", CellAlign: "center" },
            { HeaderText: "AwayName", Name: "AwayTeam", Width: 250, HeaderAlign: "center",
                CellAlign: "center"
            },
            { HeaderText: "Base Currency", Name: "BaseCurrency", Width: 120, HeaderAlign: "center",
                CellAlign: "right"
            }
        ];

        var groupHeader = [
            {
                Columns: [
                    { HeaderText: "Match", Name: "Number", MergeColNumber: 4, MergeRowNumber: 2 },
                    { HeaderText: "Base Currency", Name: "BaseCurrency", MergeRowNumber: 2 }
                ]
            }
        ];

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
            gridExpandHeight: 160,
            numberOfFrozenColumn: 0,
            showPager: true,
            pagerOption: {
                itemsPerPage: 50,
                currentPage: 0,
                itemsPerPageArray: [50, 100, 200, 500],
                totalItem: bodyData.length
            }
        };

        _table.reportContent.fanexGrid(gridOptions);
    };

    self.exportReport = function () {
        AccReport.downloadFile(window.rootUrl + "WinLoss/MatchWinLossSingleBet/ExportReport", _params);
    };
}
