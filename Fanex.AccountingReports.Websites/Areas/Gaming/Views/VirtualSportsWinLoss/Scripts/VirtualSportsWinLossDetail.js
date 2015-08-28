Namespace.Register("Fanex.AccountingReports.Gaming.VirtualSportsWinLossDetail");

Fanex.AccountingReports.Gaming.VirtualSportsWinLossDetail = function () {
    var self = this;

    var _table = {
        report: null,
        reportContent: null
    };

    var _params = {
        date: null,
        excludeTest: null,
        drawFilter: null,
        sportType: null,
        currency: null
    };

    self.initPage = function () {
        catchedDOMElement();
    };

    var catchedDOMElement = function () {
        _table.report = $('#VirtualSportsWinLossDetailContainer');
        _table.reportContent = $('#VirtualSportsWinLossDetailContent');
    };

    self.getReport = function (_detailParams) {
        _params = _detailParams;
        AccReport.disableInput();

        if (validateParams()) {
            $.ajax({
                type: "POST",
                url: window.rootUrl + "Gaming/VirtualSportsWinLoss/GetDetailReport",
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                data: JSON.stringify(_params),
                beforeSend: function () {
                    AccReport.disableInput();
                },
                success: function (report) {
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

    var validateParams = function () {
        return true;
    };

    self.renderReport = function (report) {
        var reportTitle = "Virtual Sports Win Loss Details - " + _params.date + " - " + _params.currency;

        if (report != null && report.length > 0) {
            var bodyData = [];
            var footerData = [];

            bodyData = pushDataToBody(report);
            footerData = pushDataToFooter(report);

            self.renderGrid(reportTitle, bodyData, footerData);
        }
        else {
            self.renderGrid(reportTitle, [], []);
        }
    };

    self.renderGrid = function (reportTitle, bodyData, footerData) {
        var groupHeader = [
            {
                Columns: [
                    { HeaderText: "Username", Name: "Username", MergeRowNumber: 2 },
                    { HeaderText: "Bet Count", Name: "BetCount", MergeRowNumber: 2 },
                    { HeaderText: "Turnover", Name: "Turnover", MergeRowNumber: 2 },
                    { HeaderText: "Member Win Loss", Name: "MemberWinLoss", MergeRowNumber: 2 },
                    { HeaderText: "Win Loss", Name: "PositionWinLoss", MergeColNumber: 2 },
                    { HeaderText: "Amount due to Inspired", Name: "AmountDue", MergeRowNumber: 2 }
                ]
            }
        ];
        var columns = [
            { HeaderText: "Username", Name: "Username", Width: 170, HeaderAlign: "center",
                CellAlign: "center"
            },
            { HeaderText: "Bet Count", Name: "BetCount", Width: 120, HeaderAlign: "center",
                CellAlign: "right", FormatNumber: FormatConstant.INT_NUMBER
            },
            { HeaderText: "Turnover", Name: "Turnover", Width: 120, HeaderAlign: "center",
                CellAlign: "right", FormatNumber: FormatConstant.TWO_DECIMAL_NUMBER
            },
            { HeaderText: "Member Win Loss", Name: "MemberWinLoss", Width: 120, HeaderAlign: "center",
                CellAlign: "right", FormatNumber: FormatConstant.TWO_DECIMAL_NUMBER
            },
            { HeaderText: "Position", Name: "PositionWinLoss", Width: 120, HeaderAlign: "center",
                CellAlign: "right", FormatNumber: FormatConstant.TWO_DECIMAL_NUMBER
            },
            { HeaderText: "Company", Name: "CompanyWinLoss", Width: 120, HeaderAlign: "center",
                CellAlign: "right", FormatNumber: FormatConstant.TWO_DECIMAL_NUMBER
            },
            { HeaderText: "Amount due to Inspired", Name: "AmountDue", Width: 120, HeaderAlign: "center",
                CellAlign: "right", FormatNumber: FormatConstant.TWO_DECIMAL_NUMBER
            }
        ];

        var gridOptions = {
            customizeHeader: true,
            groupHeaderRows: groupHeader,
            columns: columns,
            bodyRows: bodyData,
            showTitle: true,
            titleInfo: {
                text: "<a class='breadcrumbs-link' onclick='BackToMainReport();'>Main</a> > " + reportTitle,
                showExcelButton: true,
                exportExcelFunction: function () {
                    self.exportReport();
                }
            },
            showFooter: true,
            footerRows: footerData,
            gridExpandHeight: 190,
            showPager: true,
            pagerOption: {
                itemsPerPage: 50,
                currentPage: 0,
                itemsPerPageArray: [50, 100, 200, 500],
                totalItem: bodyData.length
            }
        };

        _table.reportContent.removeClass("display-none").fanexGrid(gridOptions);
    };

    self.exportReport = function () {
        AccReport.downloadFile(window.rootUrl + "Gaming/VirtualSportsWinLoss/ExportReportDetail", _params);
    };

    var pushDataToBody = function (report) {
        var numberOfUsers = report.length;
        var bodyData = [];

        for (var i = 0; i < numberOfUsers - 1; i++) {
            var user = report[i];

            bodyData.push({
                Columns: [
                    { Name: "Username", Value: user.Name },
                    { Name: "BetCount", Value: user.BetCount },
                    { Name: "Turnover", Value: user.Turnover },
                    { Name: "MemberWinLoss", Value: user.MemberWinLoss },
                    { Name: "PositionWinLoss", Value: user.PositionWinLoss },
                    { Name: "CompanyWinLoss", Value: user.CompanyWinLoss },
                    { Name: "AmountDue", Value: user.AmountDueInspired }
                ]
            });
        }

        return bodyData;
    };

    var pushDataToFooter = function (report) {
        var footerData = [];        
        var user = report[report.length - 1];

        footerData.push({
            Columns: [
                    { Name: "Username", Value: user.Name },
                    { Name: "BetCount", Value: user.BetCount },
                    { Name: "Turnover", Value: user.Turnover },
                    { Name: "MemberWinLoss", Value: user.MemberWinLoss },
                    { Name: "PositionWinLoss", Value: user.PositionWinLoss },
                    { Name: "CompanyWinLoss", Value: user.CompanyWinLoss },
                    { Name: "AmountDue", Value: user.AmountDueInspired }
            ]
        });

        return footerData;
    };

    self.removeAndHideDetailReport = function () {
        _table.reportContent.empty();
        _table.report.addClass("display-none");
    };
};