$(document).ready(function () {
    var companyBalanceDifference = new Fanex.AccountingReports.Balance.CompanyBalanceDifference();
    companyBalanceDifference.initPage();
});

Namespace.Register("Fanex.AccountingReports.Balance.CompanyBalanceDifference");
Fanex.AccountingReports.Balance.CompanyBalanceDifference = function () {
    var self = this;

    var _tables = {
        report: null
    };

    self.initPage = function () {
        cachedDOMElements();
        getCompanyBalanceDifference();
    };
    var cachedDOMElements = function () {
        _tables.report = $("#companyDifferenceReport");
    };
    var getCompanyBalanceDifference = function () {
        $.ajax({
            type: "POST",
            url: window.rootUrl + "Balance/CompanyBalanceDifference/GetCompanyBalanceDifference",
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            data: null,
            beforeSend: function () {
                AccReport.disableInput();
            },
            success: function (rs) {
                renderCompanyBalanceDifference(rs);
            },
            error: function (event, jqXHR, ajaxSettings) {
                renderCompanyBalanceDifference([]);
            },
            complete: function () {
                AccReport.enableInput();
            }
        });
    };
    var renderCompanyBalanceDifference = function (report) {
        _tables.report.addClass("display-none");

//        var testNumber = 199998.6099;
//        report = [{ BalanceUpDate: '20/01/2013',
//            BalanceBeforeBalanceUpDate: testNumber,
//            BalanceAfterBalanceUpDate: testNumber,
//            Difference: testNumber
//        },
//        { BalanceUpDate: '20/01/2013',
//            BalanceBeforeBalanceUpDate: testNumber,
//            BalanceAfterBalanceUpDate: testNumber,
//            Difference: testNumber
//        },
//        { BalanceUpDate: '20/01/2013',
//            BalanceBeforeBalanceUpDate: testNumber,
//            BalanceAfterBalanceUpDate: testNumber,
//            Difference: testNumber
//        }];

        var bodyData = [];
        if (report && report.length > 0) {
            var len = report.length;
            for (var i = 0; i < len; i++) {
                var info = report[i];
                var currentRow =
                {
                    Columns: [{ Name: "ColNo", Value: (i + 1) },
                              { Name: "ColBalanceUpDate", Value: info.BalanceUpDate },
                              { Name: "ColBalanceBeforeBalanceUpDate", Value: info.BalanceBeforeBalanceUpDate },
                              { Name: "ColBalanceAfterBalanceUpDate", Value: info.BalanceAfterBalanceUpDate },
                              { Name: "ColDifference", Value: info.Difference}],
                    SubRows: []
                };

                bodyData.push(currentRow);
            }
        }

        var gridOptions = {
            columns: [{ HeaderText: "No", Width: 40, Name: "ColNo", CellAlign: "center", FormatNumber: FormatConstant.INT_NUMBER },
                      { HeaderText: "Balance-Up Day", Width: 100, Name: "ColBalanceUpDate", CellAlign: "center" },
                      { HeaderText: "Company Balance Before BU", Width: 200, Name: "ColBalanceBeforeBalanceUpDate", HeaderAlign: "center", CellAlign: "right", FormatNumber: FormatConstant.TWO_DECIMAL_NUMBER },
                      { HeaderText: "Company Balance After  BU", Width: 200, Name: "ColBalanceAfterBalanceUpDate", HeaderAlign: "center", CellAlign: "right", FormatNumber: FormatConstant.TWO_DECIMAL_NUMBER },
                      { HeaderText: "Difference", Width: 200, Name: "ColDifference", HeaderAlign: "center", CellAlign: "right", FormatNumber: FormatConstant.TWO_DECIMAL_NUMBER}],
            showFooter: false,
            showTitle: false,
            bodyRows: bodyData
        };

        _tables.report.removeClass("display-none").fanexGrid(gridOptions);
    };
}