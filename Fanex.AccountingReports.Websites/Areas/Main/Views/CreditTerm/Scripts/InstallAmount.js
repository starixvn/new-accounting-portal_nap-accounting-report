
Namespace.Register("Fanex.AccountingReports.Main.CreditTerm.InstallAmount");

Fanex.AccountingReports.Main.CreditTerm.InstallAmount = function () {
    var self = this;

    var _table = {
        btnGenerate: null,
        role: null,
        installmentAmountContainer: null,
        installmentAmountContent: null
    };

    var _params = {
        roleID: null
    };

    self.initPage = function () {
        catchedDOMElement();
        bindingEvent();
        // set member is default role.
        _table.role.attr("value", "1").selected;
    };

    var catchedDOMElement = function () {
        _table.installmentAmountContainer = $('#InstallmentAmountContainer');
        _table.installmentAmountContent = $('#InstallmentAmountContent');
        _table.role = $('#Role');
        _table.btnGenerate = $('#btnGenerateInstallAmount');
    };

    var bindingEvent = function () {
        _table.btnGenerate.click(function () {
            self.getReport();
        });
    };

    self.getReport = function () {
        AccReport.disableInput();
        getParams();
        if (validateParams()) {
            $.ajax({
                type: "POST",
                url: window.rootUrl + "Main/CreditTerm/GetInstallmentAmountReport",
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
        _params.roleID = _table.role.find(':selected').val();
    };

    var validateParams = function () {
        if (_params.roleID == 1 || _params.roleID == 2
                    || _params.roleID == 3 || _params.roleID == 4) {
            return true;
        }

        return false;
    };

    self.renderReport = function (report) {
        var reportTitle = "Credit Term - Installment Amount Report";

        if (report != null && report.length > 0) {
            var numberMember = report.length;
            var bodyData = [];
            var cssClass = "";

            for (var i = 0; i < numberMember; i++) {
                var member = report[i];

                bodyData.push({
                    Columns: [
                        { Name: "Member", Value: member.Username },
                        { Name: "Currency", Value: member.CurrencyName },
                        { Name: "Balance", Value: member.Balance,
                            CustomCellRender: function (currentCell) {
                                return setColorToValue(currentCell);
                            }
                        },
                        { Name: "BalanceBase", Value: member.BalanceBase,
                            CustomCellRender: function (currentCell) {
                                return setColorToValue(currentCell);
                            }
                        },
                        { Name: "InstallmentAmount", Value: member.InstallmentAmount,
                            CustomCellRender: function (currentCell) {
                                return setColorToValue(currentCell);
                            }
                        },
                        { Name: "InstallmentDay",
                            Value: numeral(member.NumberDayInstallment).format(FormatConstant.INT_NUMBER)
                        }
                    ]
                });
            }

            renderGrid(bodyData, reportTitle);
        }
        else {
            renderGrid([], reportTitle);
        }
    };

    var renderGrid = function (bodyData, reportTitle) {
        var gridOptions = {
            columns: [
                { HeaderText: "Member", Name: "Member", Width: 120, HeaderAlign: "center",
                    CellAlign: "center"
                },
                { HeaderText: "CURR", Name: "Currency", Width: 80, HeaderAlign: "center",
                    CellAlign: "center"
                },
                { HeaderText: "Balance", Name: "Balance", Width: 140, HeaderAlign: "center",
                    CellAlign: "right"
                },
                { HeaderText: "Base(RM)", Name: "BalanceBase", Width: 140, HeaderAlign: "center",
                    CellAlign: "right"
                },
                { HeaderText: "Installment Amount", Name: "InstallmentAmount", Width: 140,
                    HeaderAlign: "center", CellAlign: "right"
                },
                { HeaderText: "Day of Installment", Name: "InstallmentDay", Width: 140,
                    HeaderAlign: "center", CellAlign: "right"
                }
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
            gridExpandHeight: 205,
            showPager: true,
            pagerOption: {
                itemsPerPage: 50,
                currentPage: 0,
                itemsPerPageArray: [50, 100, 200, 500],
                totalItem: bodyData.length
            }
        };

        _table.installmentAmountContent.removeClass("display-none").fanexGrid(gridOptions);
    };

    self.exportReport = function () {
        AccReport.downloadFile(window.rootUrl + "Main/CreditTerm/ExportInstallmentAmountReport", _params);
    };

    var setColorToValue = function (currentCell) {
        if (currentCell.Value >= 0) {
            return '<div><font color ="blue">'
                        + numeral(currentCell.Value).format(FormatConstant.TWO_DECIMAL_NUMBER) +
                   '</font></div>';
        }
        else {
            return '<div><font color ="red">'
                        + numeral(currentCell.Value).format(FormatConstant.TWO_DECIMAL_NUMBER) +
                   '</font></div>';
        }
    };
};