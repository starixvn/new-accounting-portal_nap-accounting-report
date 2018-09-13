
$(document).ready(function () {
    var dailyTurnover = new Fanex.AccountingReports.Special.DailyTurnover();
    dailyTurnover.initPage();
});

Namespace.Register("Fanex.AccountingReports.Special.DailyTurnover");

Fanex.AccountingReports.Special.DailyTurnover = function () {
    var self = this;

    var detailReport = null;

    var _table = {
        report: null,
        reportContent: null,
        detailReport: null,
        detailReportContent: null,
        datePickerPlugin: null,
        btnGenerate: null,
        viewDetail: null,
        viewDetailLink: null,
        detailMessage: null,
        detailDifference: null,
        detailDifferenceContent: null
    };

    var _params = {
        FromDate: null
    };

    self.initPage = function () {
        catchedDOMElement();
        renderDatePicker();
        bindingEvent();
    };

    var catchedDOMElement = function () {
        _table.report = $('#DailyTurnoverContainer');
        _table.reportContent = $('#DailyTurnoverContent');
        _table.detailReport = $('#DailyTurnoverDetailContainer');
        _table.detailReportContent = $('#DailyTurnoverDetailContent');
        _table.datePickerPlugin = $('#datepickerplugin');
        _table.btnGenerate = $('#btnGenerate');
        _table.viewDetail = $('#ViewDetail');
        _table.viewDetailLink = $('#ViewDetailTurnoverChanged');
        _table.detailMessage = $('#DetailMessage');
        _table.detailDifference = $('#DetailDifferentTurnoverContainer');
        _table.detailDifferenceContent = $('#DetailDifferentTurnoverContent');
    };

    var renderDatePicker = function () {
        var maxDate = new Date(today);
        // min date is balance up date
        var minDate = new Date(balanceUpDate);

        _table.datePickerPlugin.nexDatePicker({
            nexDatePickerType: "single",
            datePickerFormat: {
                changeMonth: true,
                changeYear: true,
                maxDate: maxDate,
                minDate: minDate,
                dateFormat: "M dd, yy"
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
            self.getReport();
        });

        _table.viewDetailLink.click(function () {
            self.getDetailReport();
        });
    };

    // BEGIN get report and detail report
    self.getReport = function () {
        AccReport.disableInput();
        hideDetailTable();
        getParams();
        if (validateParams() == true) {
            $.ajax({
                type: "POST",
                url: window.rootUrl + "Special/DailyTurnover/GetReport",
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

    self.getDetailReport = function () {
        _table.detailMessage.removeClass("display-none");
        if (_table.detailDifference.hasClass('ui-dialog-content')) {
            _table.detailDifference.dialog("close");
        }
        _table.detailDifference.addClass("display-none");
        AccReport.disableInput();
        getParams();
        if (validateParams() == true) {
            $.ajax({
                type: "POST",
                url: window.rootUrl + "Special/DailyTurnover/GetDetailReport",
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                data: JSON.stringify(_params),
                beforeSend: function () {
                    AccReport.disableInput();
                },
                success: function (report) {
                    AccReport.popUp.hide();
                    detailReport = report;
                    self.renderDetailReport(report);
                },
                error: function (jqXHR, event, ajaxSettings) {
                    if (jqXHR.status &&
                            jqXHR.status == AccReportData.Constants.HtmlErrorCode.SystemError) {
                        var r = jQuery.parseJSON(jqXHR.responseText);
                        AccReport.popUp.show(r.message);
                    }
                    self.renderDetailReport(null);
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
        _params.FromDate = _table.datePickerPlugin.nexDatePicker("getSingleDate");
    };

    var validateParams = function () {
        return true;
    };

    var hideDetailTable = function () {
        _table.detailReport.addClass("display-none");
        _table.detailMessage.addClass("display-none");

        if (_table.detailDifference.hasClass('ui-dialog-content')) {
            _table.detailDifference.dialog("close");
        }

        _table.detailDifference.addClass("display-none");
    };

    // BEGIN render report and detail report

    // render main report
    self.renderReport = function (report) {
        var reportTitle = "Daily Turnover - Date: " +
                moment(_params.FromDate).format(FormatConstant.MMM_DD_YYYY_STRING_MOMENT);

        if (report != null && report.length >= 2) {
            var bodyData = [];
            var virtualCurrency = report[0];
            var actualCurrency = report[1];

            bodyData = pushDataIntoColumn(virtualCurrency, actualCurrency);

            renderGrid(reportTitle, bodyData);
        }
        else {
            renderGrid(reportTitle, []);
        }
    };

    var renderGrid = function (reportTitle, bodyData) {
        var gridOptions = {
            columns: [
                { HeaderText: "Date", Width: 120, Name: "Date", HeaderAlign: "center", CellAlign: "center", FormatDateTime: FormatConstant.MMM_DD_YYYY_STRING_MOMENT },
                { HeaderText: "Total Turnover", Width: 120, Name: "TotalTurnover", HeaderAlign: "center",
                    CellAlign: "right", FormatNumber: FormatConstant.INT_NUMBER
                },
                { HeaderText: "Super Turnover", Width: 120, Name: "SuperTurnover", HeaderAlign: "center",
                    CellAlign: "right", FormatNumber: FormatConstant.INT_NUMBER
                },
                { HeaderText: "Master Turnover", Width: 120, Name: "MasterTurnover", HeaderAlign: "center",
                    CellAlign: "right", FormatNumber: FormatConstant.INT_NUMBER
                },
                { HeaderText: "Agent Turnover", Width: 120, Name: "AgentTurnover", HeaderAlign: "center",
                    CellAlign: "right", FormatNumber: FormatConstant.INT_NUMBER
                },
                { HeaderText: "House Turnover", Width: 120, Name: "IBCTurnover", HeaderAlign: "center",
                    CellAlign: "right", FormatNumber: FormatConstant.INT_NUMBER
                },
                { HeaderText: "CNT", Width: 360, Name: "CNT", HeaderAlign: "center", CellAlign: "center" }
            ],
            showTitle: true,
            titleInfo: {
                text: reportTitle,
                showExcelButton: true,
                exportExcelFunction: function () {
                    self.exportReport();
                }
            },
            bodyRows: bodyData,
            fullHeight: false,
            gridHeight: 220
        };

        _table.reportContent.empty().removeClass("display-none").fanexGrid(gridOptions);
        _table.reportContent.find('tr').removeClass("freegrid-even highlight-row");
        _table.viewDetail.removeClass("display-none");
    };

    var pushDataIntoColumn = function (virtualCurrency, actualCurrency) {
        var bodyData = [];
        var currentRow = {
            Columns: []
        };

        bodyData.push({
            Columns: [
                { Name: "Date", Value: "Virtual Currency", MergeColNumber: 7, CellAlign: "left",
                    TdClass: "currency"
                }
            ]
        });

        currentRow = getCurrentRow(virtualCurrency);
        bodyData.push(currentRow);

        bodyData.push({
            Columns: [
                { Name: "Date", Value: "Actual Currency", MergeColNumber: 7, CellAlign: "left",
                    TdClass: "currency"
                }
            ]
        });

        currentRow = getCurrentRow(actualCurrency);
        bodyData.push(currentRow);

        return bodyData;
    };

    var getCurrentRow = function (currency) {
        var currentRow = {
            Columns: []
        }

        currentRow.Columns = [
            { Name: "Date", Value: currency.Date },
            { Name: "TotalTurnover", Value: currency.Turnover.TotalTurnover },
            { Name: "SuperTurnover", Value: currency.Turnover.SuperTurnover },
            { Name: "MasterTurnover", Value: currency.Turnover.MasterTurnover },
            { Name: "AgentTurnover", Value: currency.Turnover.AgentTurnover },
            { Name: "IBCTurnover", Value: currency.Turnover.IBCTurnover },
            { Name: "CNT", Value: currency.TicketNormalRejectCount }
        ];

        return currentRow;
    };

    // render detail report
    self.renderDetailReport = function (report) {
        if (report != null && report.length > 0) {
            var numberEvents = report.length;
            var bodyData = [];

            for (var i = 0; i < numberEvents; i++) {
                var event = report[i];
                var cssClass = i % 2 != 0 ? "detail-odd-row" : "";
                bodyData = pushEventIntoColumn(event, bodyData, cssClass, i);
            }

            renderDetailGrid(bodyData);
        }
        else {
            renderDetailGrid([]);
        }
    };

    var renderDetailGrid = function (bodyData) {
        var gridOptions = {
            columns: [
                { HeaderText: "Date Time", Width: 150, Name: "DateTime", HeaderAlign: "center",
                    CellAlign: "center", FormatDateTime: FormatConstant.MMM_DD_YYYY_H_M_S_A_STRING
                },
                { HeaderText: "Action", Width: 130, Name: "Action", HeaderAlign: "center",
                    CellAlign: "center",
                    CustomCellRender: function (currentValue) {
                        return self.getPopupLink(currentValue);
                    }
                },
                { HeaderText: "Username", Width: 150, Name: "Username", HeaderAlign: "center",
                    CellAlign: "center"
                },
                { HeaderText: "Total Turnover", Width: 130, Name: "TotalTurnover", HeaderAlign: "center",
                    CellAlign: "right", FormatNumber: FormatConstant.INT_NUMBER
                },
                { HeaderText: "Super Turnover", Width: 130, Name: "SuperTurnover", HeaderAlign: "center",
                    CellAlign: "right", FormatNumber: FormatConstant.INT_NUMBER
                },
                { HeaderText: "Master Turnover", Width: 130, Name: "MasterTurnover", HeaderAlign: "center",
                    CellAlign: "right", FormatNumber: FormatConstant.INT_NUMBER
                },
                { HeaderText: "Agent Turnover", Width: 130, Name: "AgentTurnover", HeaderAlign: "center",
                    CellAlign: "right", FormatNumber: FormatConstant.INT_NUMBER
                },
                { HeaderText: "House Turnover", Width: 130, Name: "IBCTurnover", HeaderAlign: "center",
                    CellAlign: "right", FormatNumber: FormatConstant.INT_NUMBER
                }
            ],
            showTitle: true,
            titleInfo: {
                showExcelButton: true,
                exportExcelFunction: function () {
                    self.exportDetailReport();
                }
            },
            bodyRows: bodyData,
            gridExpandHeight: 370
        };

        _table.detailReportContent.empty().removeClass("display-none").fanexGrid(gridOptions);
        _table.detailReportContent.find('tr').removeClass("freegrid-even highlight-row");
    };

    var pushEventIntoColumn = function (event, bodyData, cssClass, index) {
        var currentRow = {
            Columns: []
        };

        if (event != null && event.TurnoverAmount != null
                            && event.TurnoverAmount.length >= 2)
            currentRow = {
                Columns: [
                { Name: "DateTime", Value: event.Date, MergeRowNumber: 4, TdClass: cssClass },
                { Name: "Action", Value: index, MergeRowNumber: 4, TdClass: cssClass },
                { Name: "Username", Value: event.Username, MergeRowNumber: 4, TdClass: cssClass },
                { Name: "TotalTurnover", Value: "Virtual Currency", MergeColNumber: 5,
                    CellAlign: "left", TdClass: "detail-currency " + cssClass
                }
            ]
            };
        bodyData.push(currentRow);

        bodyData = pushTurnover(event.TurnoverAmount[0], event.TurnoverAmount[1], bodyData, cssClass);

        return bodyData;
    };

    var pushTurnover = function (virtualTurnover, actualTurnover, bodyData, cssClass) {
        var currentRow = {
            Columns: []
        };

        currentRow = {
            Columns: [
                { Name: "TotalTurnover", Value: virtualTurnover.TotalTurnover,
                    TdClass: cssClass
                },
                { Name: "SuperTurnover", Value: virtualTurnover.SuperTurnover,
                    TdClass: cssClass
                },
                { Name: "MasterTurnover", Value: virtualTurnover.MasterTurnover,
                    TdClass: cssClass
                },
                { Name: "AgentTurnover", Value: virtualTurnover.AgentTurnover,
                    TdClass: cssClass
                },
                { Name: "IBCTurnover", Value: virtualTurnover.IBCTurnover,
                    TdClass: cssClass
                }
            ]
        };
        bodyData.push(currentRow);

        currentRow = {
            Columns: [
                { Name: "TotalTurnover", Value: "Actual Currency", MergeColNumber: 5,
                    CellAlign: "left", TdClass: "detail-currency " + cssClass
                }
            ]
        };
        bodyData.push(currentRow);

        currentRow = {
            Columns: [
                { Name: "TotalTurnover", Value: actualTurnover.TotalTurnover,
                    TdClass: cssClass
                },
                { Name: "SuperTurnover", Value: actualTurnover.SuperTurnover,
                    TdClass: cssClass
                },
                { Name: "MasterTurnover", Value: actualTurnover.MasterTurnover,
                    TdClass: cssClass
                },
                { Name: "AgentTurnover", Value: actualTurnover.AgentTurnover,
                    TdClass: cssClass
                },
                { Name: "IBCTurnover", Value: actualTurnover.IBCTurnover,
                    TdClass: cssClass
                }
            ]
        };
        bodyData.push(currentRow);

        return bodyData;
    };

    self.getPopupLink = function (currentCell) {
        var index = currentCell.Value;
        var params = detailReport[index];

        return "<div class='link bold' onclick='viewDetailDifferent(" + index + ", " + JSON.stringify(params) + ");'>"
                                        + params.ActionName + "</div>";
    };

    // BEGIN open pop-up
    viewDetailDifferent = function (index, event) {
        var popUpTitle = event.ActionName + " - " + event.Date;

        if (event != null && event.TurnoverDifference != null
                            && event.TurnoverDifference.length >= 2) {
            var bodyData = [];
            var currentRow = {
                Columns: []
            };
            var cssClass = "";

            currentRow = {
                Columns: [
                { Name: "Match", Value: event, MergeRowNumber: 4, TdClass: cssClass },
                { Name: "TotalTurnover", Value: "Virtual Currency", MergeColNumber: 5,
                    CellAlign: "left", TdClass: "detail-currency " + cssClass
                }
            ]
            };
            bodyData.push(currentRow);
            bodyData = pushTurnover(event.TurnoverDifference[0], event.TurnoverDifference[1],
                                    bodyData, cssClass);

            renderDetailDifferentTurnoverGrid(popUpTitle, bodyData, index.toString());
        }
        else {
            renderDetailDifferentTurnoverGrid(popUpTitle, [], index.toString());
        }
    };

    var renderDetailDifferentTurnoverGrid = function (title, bodyData, index) {
        var gridOptions = {
            columns: [
                { HeaderText: "Match", Width: 250, Name: "Match", HeaderAlign: "center",
                    CellAlign: "center", CustomCellRender: function (currentCell) {
                        var isVoidAction = currentCell.Value.ActionName.indexOf("Void") == 0 ? true : false;
                        var matchAndCode = currentCell.Value.Match.split("$");
                        return isVoidAction ?
                        '<div>' + currentCell.Value.Match + '</div>' :
                        '<div>' + matchAndCode[0] + '<br/>' + matchAndCode[1] + '</div>';
                    }
                },
                { HeaderText: "Total Turnover", Width: 130, Name: "TotalTurnover", HeaderAlign: "center",
                    CellAlign: "right", FormatNumber: FormatConstant.INT_NUMBER
                },
                { HeaderText: "Super Turnover", Width: 130, Name: "SuperTurnover", HeaderAlign: "center",
                    CellAlign: "right", FormatNumber: FormatConstant.INT_NUMBER
                },
                { HeaderText: "Master Turnover", Width: 130, Name: "MasterTurnover", HeaderAlign: "center",
                    CellAlign: "right", FormatNumber: FormatConstant.INT_NUMBER
                },
                { HeaderText: "Agent Turnover", Width: 130, Name: "AgentTurnover", HeaderAlign: "center",
                    CellAlign: "right", FormatNumber: FormatConstant.INT_NUMBER
                },
                { HeaderText: "IBC Turnover", Width: 130, Name: "IBCTurnover", HeaderAlign: "center",
                    CellAlign: "right", FormatNumber: FormatConstant.INT_NUMBER
                }
            ],
            showTitle: true,
            titleInfo: {
                showExcelButton: true,
                exportExcelFunction: function () {
                    self.exportDetailDifferentReport(index);
                }
            },
            bodyRows: bodyData
        };

        _table.detailDifference.dialog({
            title: title,
            autoOpen: false,
            height: 200,
            width: 1000,
            modal: false,
            resizable: false,
            close: function (event, ui) {
            }
        }).dialog("open");

        _table.detailDifferenceContent.empty().removeClass("display-none").fanexGrid(gridOptions);
    };

    // Export report
    self.exportReport = function () {
        AccReport.downloadFile(window.rootUrl + "Special/DailyTurnover/ExportReport", _params);
    };

    self.exportDetailReport = function () {
        AccReport.downloadFile(window.rootUrl + "Special/DailyTurnover/ExportDetailReport", _params);
    };

    self.exportDetailDifferentReport = function (index) {
        var parameters = "index=" + index + "&fromDate=" + _params.FromDate;
        AccReport.downloadFile(window.rootUrl + "Special/DailyTurnover/ExportDetailDifferenceReport",
                                                                parameters);
    };
};
