Namespace.Register("Fanex.AccountingReports.Misc.DailyCreatedCustomers.DailyCreatedCustomersDetails");
Fanex.AccountingReports.Misc.DailyCreatedCustomers.DailyCreatedCustomersDetails = function () {
    var self = this;

    var _tables = {
        report: null,
        reportContent: null
    };

    var _params = {
        reportParams: {
            Date: null,
            RoleID: null,            
            IsDirect: null,
            RoleName: null
        }
    };

    self.TempHtml = {
        contentHtml: ''
    };

    self.initPage = function () {
        cachedDOMElements();
    };
    var initGridScrollerTables = function () {
        $("#dailyCreatedCustomersDetailsReport").initGrid({
            isShowPager: false
        });
    };
    var cachedDOMElements = function () {
        _tables.report = $("#dailyCreatedCustomersContainer");
        _tables.reportContent = $("#dailyCreatedCustomersContent");
    };

    self.getReport = function (params) {
        AccReport.disableInput();
        getParams(params);
        if (validateParams() == true) {
            $.ajax({
                type: "POST",
                url: window.rootUrl + "Misc/DailyCreatedCustomers/GetReportDetails",
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                data: JSON.stringify(_params.reportParams),
                beforeSend: function () {
                    AccReport.disableInput();
                },
                success: function (rs) {
                    AccReport.popUp.hide();
                    self.renderReport(rs);
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
    var getParams = function (params) {
        _params.reportParams = params;
    };
    var validateParams = function () {
        return true;
    };
    self.renderReport = function (report) {
        _params.reportParams.RoleName = GetRoleName(_params.reportParams.RoleID, _params.reportParams.IsDirect);
        var reportTitle = _params.reportParams.RoleName + " List (" + _params.reportParams.Date + ")";
        var bodyData = [];

        if (report &&
            report.length > 0) {
            var infoLength = report.length;
            for (var i = 0; i < infoLength; i++) {
                var info = report[i];
                bodyData.push(
                    {
                        Columns:
                        [
                            { Name: "Number", Value: i + 1 },
                            { Name: "Username", Value: info.UserName },
                            { Name: "CreatedDate", Value: info.CreatedDate },
                            { Name: "Creator", Value: info.Creator }
                        ]
                    }
                );
            }
            self.RenderGrid(bodyData, reportTitle);

        } else {
            self.RenderGrid([], reportTitle);
        }

        self.showReport();
    };

    self.RenderGrid = function (bodyData, title) {
        var columns =
        [
            { HeaderText: "#", Name: "Number", Width: 50, HeaderAlign: "center", CellAlign: "center" },
            { HeaderText: "Username", Name: "Username", Width: 200, HeaderAlign: "center", CellAlign: "center" },
            { HeaderText: "Created Date", Name: "CreatedDate", Width: 180, HeaderAlign: "center", CellAlign: "center" },
            { HeaderText: "Created By", Name: "Creator", Width: 120, HeaderAlign: "center", CellAlign: "center" }
        ];

        var gridOptions = {
            columns: columns,
            showTitle: true,
            titleInfo: {
                text: title,
                showExcelButton: true,
                exportExcelFunction: function () {
                    self.exportReport();
                }
            },
            bodyRows: bodyData,
            fullHeight: false,
            gridHeight: $(window).height() - 160,
            numberFreezeCol: 0,
            showPager: true,
            pagerOption: {
                itemsPerPage: 50,
                currentPage: 0,
                itemsPerPageArray: [50, 100, 200, 500],
                totalItem: bodyData.length
            }
        };

        _tables.reportContent.removeClass("display-none").fanexGrid(gridOptions);
    };

    self.hideReport = function () {
        _tables.report.addClass("display-none");
    };

    self.showReport = function () {
        _tables.report.removeClass("display-none");
    };

    self.exportReport = function () {
        AccReport.downloadFile(window.rootUrl + "Misc/DailyCreatedCustomers/ExportReportDetails", _params.reportParams);
    };
}