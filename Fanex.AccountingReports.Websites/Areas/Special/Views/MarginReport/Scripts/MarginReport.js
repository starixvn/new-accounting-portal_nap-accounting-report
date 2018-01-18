/* File Created: December 12, 2013 */
$(document).ready(function () {
    var reportManager = new Fanex.AccountingReports.Special.Margin.MarginReport();
    reportManager.initPage();
});
Namespace.Register("Fanex.AccountingReports.Special.Margin.MarginReport");
Fanex.AccountingReports.Special.Margin.MarginReport = function () {
    var self = this;
    self.Params = {
        reportParams: {
            ActualStake: null
        }
    };

    var _tables = {
        btnGenerate: null,
        reportContainer: null,
        report: null
    };

    self.initPage = function () {
        cachedDOMElements();
        bindingEvent();
    };

    var cachedDOMElements = function () {
        _tables.btnGenerate = $("#btnGenerate");
        _tables.reportContainer = $("#marginReportContainer");
        _tables.report = $("#marginReport");

    };

    var bindingEvent = function () {
        _tables.btnGenerate.click(function () {
            self.btnGenerateClick();
        });
    };

    var getParams = function () {
        self.Params.reportParams.ActualStake = $('#txtAmount').val();
    };

    self.btnGenerateClick = function () {
        AccReport.disableInput();
        getParams();
        if (validateParams() == true) {
            $.ajax({
                type: "POST",
                url: window.rootUrl + "Special/MarginReport/GetDataMarginReport",
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                data: JSON.stringify(self.Params.reportParams),
                beforeSend: function () {
                    AccReport.disableInput();
                },
                success: function (rs) {
                    renderMarginReport(rs);
                },
                error: function (event, jqXHR, ajaxSettings) {
                    renderMarginReport([]);
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

    renderMarginReport = function (report) {
        _tables.reportContainer.removeClass("display-none");
        _tables.report.addClass("display-none");
        var bodyData = [];

        var reportLength = report.ChildsInfo.length;
        for (var i = 0; i < reportLength; i++) {
            var info = report.ChildsInfo[i];
            bodyData.push({
                Columns: [{ Name: "ColNo", Value: i + 1 },
                        { Name: "ColUsername", Value: info.UserName },
                        { Name: "ColBettype", Value: info.BettypeName },
                        { Name: "ColWinningAmount", Value: info.WinningAmount },
                        { Name: "ColTurnover", Value: info.Turnover },
                        { Name: "ColRatio", Value: info.Ratio },
                        { Name: "ColTotalBet", Value: info.TotalBet}]
            });
        }
        var gridOptions = {
            columns: [{ HeaderText: "No", Width: 80, Name: "ColNo", HeaderAlign: "center", CellAlign: "center" },
                    { HeaderText: "Username", Width: 220, Name: "ColUsername", HeaderAlign: "center", CellAlign: "left" },
                    { HeaderText: "Type", Width: 100, Name: "ColBettype", HeaderAlign: "center", CellAlign: "center" },
                    { HeaderText: "Winning Amount", Width: 120, Name: "ColWinningAmount", FormatNumber: FormatConstant.TWO_DECIMAL_NUMBER, HeaderAlign: "center", CellAlign: "right" },
                    { HeaderText: "Turnover", Width: 120, Name: "ColTurnover", FormatNumber: FormatConstant.TWO_DECIMAL_NUMBER, HeaderAlign: "center", CellAlign: "right" },
                    { HeaderText: "Ratio", Width: 120, Name: "ColRatio", FormatNumber: FormatConstant.TWO_DECIMAL_NUMBER, HeaderAlign: "center", CellAlign: "right" },
                    { HeaderText: "Bet count", Width: 100, Name: "ColTotalBet", HeaderAlign: "center", CellAlign: "right"}],
            showTitle: true,
            titleInfo:
            {
                text: "Margin Report",
                showExcelButton: true,
                exportExcelFunction: function () {
                    self.exportReport();
                }
            },
            showFooter: false,
            bodyRows: bodyData,
            numberOfFrozenColumn: 0,
            gridWidth: 1400,
            gridExpandHeight: 170,
            fullWidth: false,
            fullHeight: true,
            showPager: true,
            pagerOption: {
                itemsPerPage: 500,
                currentPage: 0,
                itemsPerPageArray: [500, 750, 1000],
                totalItem: bodyData.length
            }

        };
        _tables.report.removeClass("display-none").fanexGrid(gridOptions);
    };

    self.exportReport = function () {
        AccReport.downloadFile(window.rootUrl + "Special/MarginReport/ExportMarginReport", self.Params.reportParams);
    };

    self.checkDecimal = function () {
        var amount = parseFloat($('#txtAmount').val());
        if (isNaN(amount)) {
            return false;
        }
        return true;
    };

    var validateParams = function () {
        var isValid = true;
        var message = '';
        var notifyIds = [];
        var amount = $('#txtAmount').val();
        if ($('#txtAmount').val() != '' && isNaN(amount)) {
            isValid = false;
            message = "Please input decimal value!";
            notifyIds.push("txtAmount");

        }
        if (!isValid) {
            AccReport.popUp.removeHighLight();
            AccReport.popUp.show(message, notifyIds.join(","));
        } else {
            AccReport.popUp.removeHighLight();
        }

        return isValid;
    };

};