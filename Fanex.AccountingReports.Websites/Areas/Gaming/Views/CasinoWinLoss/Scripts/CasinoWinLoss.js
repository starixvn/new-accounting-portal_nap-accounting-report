$(document).ready(function () {
    var casinoWinLoss = new Fanex.AccountingReports.Gaming.CasinoWinLoss();
    casinoWinLoss.initPage();
});

Namespace.Register("Fanex.AccountingReports.Gaming.CasinoWinLoss");

Fanex.AccountingReports.Gaming.CasinoWinLoss = function () {
    var self = this;

    var _table = {
        datePickerPlugin: null,
        btnGenerate: null,
        CasinoWinLostContainer: null,
        CasinoWinLostContent: null
    };

    var _params = {
        winLostDate: null
    };

    self.initPage = function () {
        catchedDOMElement();
        renderDatePicker();
        bindingEvent();
    };

    var catchedDOMElement = function () {
        _table.datePickerPlugin = $('#datepickerplugin');
        _table.btnGenerate = $('#btnGenerate');
        _table.CasinoWinLostContainer = $('#CasinoWinLostContainer');
        _table.CasinoWinLostContent = $('#CasinoWinLostContent');
    };

    var renderDatePicker = function () {
        _table.datePickerPlugin.nexDatePicker({
            nexDatePickerType: "single",
            datePickerFormat: {
                changeMonth: true,
                changeYear: true,
                dateFormat: "M dd, yy",
                maxDate: new Date(today),
                minDate: new Date(minDay)
            },
            firstDatePicker: {
                Id: "firstDatePickerId1",
                Label: labelDate,
                InitDate: today,
                LabelCss: "lbl-field col_2",
                TextboxCss: "edit-field col_6"
            },
            onSelectDatePicker: function () {
            }
        });
    };

    var bindingEvent = function () {
        _table.btnGenerate.click(function () {
            getReport();
        });
    };

    var getReport = function () {
        AccReport.disableInput();
        getParams();
        if (validateParams() == true) {
            $.ajax({
                type: "POST",
                url: window.rootUrl + "Gaming/CasinoWinLoss/GetReport",
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                data: JSON.stringify(_params),
                beforeSend: function () {
                    AccReport.disableInput();
                },
                success: function (report) {
                    AccReport.popUp.hide();
                    renderReport(report);
                },
                error: function (jqXHR, event, ajaxSettings) {
                    if (jqXHR.status &&
                            jqXHR.status == AccReportData.Constants.HtmlErrorCode.SystemError) {
                        var r = jQuery.parseJSON(jqXHR.responseText);
                        AccReport.popUp.show(r.message);
                    }
                    renderReport(null);
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
        _params.winLostDate = _table.datePickerPlugin.nexDatePicker("getSingleDate");
    };

    var validateParams = function () {
        return true;
    };

    var renderReport = function (report) {
        var reportTitle = "Casino Daily Report: " + _params.winLostDate;

        if (report != null && report.Rows != null && report.Rows.length > 0) {
            var bodyData = [];
            var footerData = [];

            bodyData = renderBody(report, bodyData);

            if (isDisplayTotalCurrency == 'True') {
                if (report.Rows[0].Rows != null && report.Rows[0].Rows.length > 0) {
                    footerData = renderGrandTotal(report, footerData);
                }
            }
            renderGrid(bodyData, footerData, reportTitle);
        }
        else {
            renderGrid([], [], reportTitle);
        }
    };

    var renderBody = function (report, bodyData) {
        var numberBetType = report.Rows.length - 1;

        for (var i = 0; i < numberBetType; i++) {
            bodyData = renderBetType(report.Rows[i], bodyData);

            if (isDisplayTotalCurrency == 'True') {
                bodyData.push({
                    Columns: [
                { Name: "BetType", Value: "", MergeColNumber: 7 }
                ]
                });
            }
            else {
                if (numberBetType > 1) {
                    bodyData.push({
                        Columns: [
                        { Name: "BetType", Value: "", MergeColNumber: 7 }
                        ]
                    });
                }
            }
        }

        if (isDisplayTotalCurrency == 'True') {
            bodyData = renderBetTypeDetail(report.Rows[numberBetType].Rows, "Total", bodyData, "total");
        }
        else {
            if (numberBetType > 1) {
                bodyData = renderBetTypeDetail(report.Rows[numberBetType].Rows, "Total", bodyData, "total");
            }
        }

        return bodyData;
    };

    var renderBetType = function (betType, bodyData) {
        if (betType != null && betType.Rows != null) {
            var currentColumn = [];

            bodyData = renderBetTypeDetail(betType.Rows, betType.Name, bodyData, "");

            currentColumn = [
            { Name: "Currency", Value: betType.Name + " Total", TdClass: "bold" }
            ];
            currentColumn = renderRow(betType, currentColumn, "bold");

            bodyData.push({
                Columns: currentColumn
            });
        }

        return bodyData;
    };

    var renderBetTypeDetail = function (listCurrencies, name, bodyData, tdPrefixClass) {
        var numberCurrencies = listCurrencies.length;

        for (var i = 0; i < numberCurrencies; i++) {
            var currency = listCurrencies[i];
            var currentColumn = [];
            var tdClass = "";

            if (i % 2 == 0) {
                tdClass = tdPrefixClass == "" ? "" : " total-odd-row";
            }
            else {
                tdClass = tdPrefixClass == "" ? "" : " total-even-row";
            }

            if (i == 0) {
                currentColumn = [
                    { Name: "BetType", Value: name, MergeRowNumber: numberCurrencies + 1,
                        TdClass: "bold" + tdClass
                    }
                ];
            }

            currentColumn = currentColumn.concat(
                [
                    { Name: "Currency", Value: currency.Name, TdClass: tdClass }
                ]
                );
            currentColumn = renderRow(currency, currentColumn, tdClass);

            bodyData.push({
                Columns: currentColumn
            });
        }

        return bodyData;
    };

    var renderGrandTotal = function (report, footerData) {
        var currentColumn = [
            { Name: "BetType", Value: "Grand Total", MergeColNumber: 2 }
        ];

        currentColumn = renderRow(report, currentColumn);
        footerData.push({
            Columns: currentColumn
        });

        return footerData;
    };

    var renderRow = function (currency, currentColumn, tdClass) {
        currentColumn = currentColumn.concat(
            [
                { Name: "Turnover", Value: currency.Turnover, TdClass: tdClass },
                { Name: "WinLoss", Value: currency.WinLost, TdClass: tdClass },
                { Name: "Position", Value: currency.Position, TdClass: tdClass },
                { Name: "Commission", Value: currency.Commission, TdClass: tdClass },
                { Name: "LineTotal", Value: currency.LineTotal, TdClass: tdClass }
            ]
        );

        return currentColumn;
    };

    var renderGrid = function (bodyData, footerData, reportTitle) {
        var columns = [
            { HeaderText: "Bettype", Name: "BetType", Width: 100, HeaderAlign: "center", CellAlign: "center" },
            { HeaderText: "Currency", Name: "Currency", Width: 100, HeaderAlign: "center", CellAlign: "center" },
            { HeaderText: "Turnover", Name: "Turnover", Width: 100, HeaderAlign: "center", CellAlign: "right",
                FormatNumber: FormatConstant.TWO_DECIMAL_NUMBER
            },
            { HeaderText: "Win Loss", Name: "WinLoss", Width: 100, HeaderAlign: "center", CellAlign: "right",
                FormatNumber: FormatConstant.TWO_DECIMAL_NUMBER
            },
            { HeaderText: "Position", Name: "Position", Width: 100, HeaderAlign: "center", CellAlign: "right",
                FormatNumber: FormatConstant.TWO_DECIMAL_NUMBER
            },
            { HeaderText: "Commission", Name: "Commission", Width: 100, HeaderAlign: "center", CellAlign: "right",
                FormatNumber: FormatConstant.TWO_DECIMAL_NUMBER
            },
            { HeaderText: "Line Total", Name: "LineTotal", Width: 100, HeaderAlign: "center", CellAlign: "right",
                FormatNumber: FormatConstant.TWO_DECIMAL_NUMBER
            }
        ];

        var gridOptions = {
            columns: columns,
            showTitle: true,
            titleInfo: {
                text: reportTitle,
                showExcelButton: true,
                exportExcelFunction: function () {
                    exportReport();
                }
            },
            bodyRows: bodyData,
            showFooter: true,
            footerRows: footerData,
            gridExpandHeight: 170,
            showDistinctRow: false,
            showHighlightRow: false
        };

        _table.CasinoWinLostContent.removeClass("display-none").fanexGrid(gridOptions);
    };

    var exportReport = function () {
        AccReport.downloadFile(window.rootUrl + "Gaming/CasinoWinLoss/ExportReport", _params);
    };
};