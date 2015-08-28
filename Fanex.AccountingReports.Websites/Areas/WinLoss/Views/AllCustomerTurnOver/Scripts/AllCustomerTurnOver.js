Namespace.Register("Fanex.AccountingReports.WinLoss.AllCustomerTurnOver");

$(document).ready(function () {
    var allCustomerTurnOver = new Fanex.AccountingReports.WinLoss.AllCustomerTurnOver();
    allCustomerTurnOver.initPage();
});

var BetFromOnChange = function () {
    var allCustomerTurnOver = new Fanex.AccountingReports.WinLoss.AllCustomerTurnOver();
    allCustomerTurnOver.setFromText();
};

Fanex.AccountingReports.WinLoss.AllCustomerTurnOver = function () {
    var self = this;

    var _tables = {
        datepickerplugin: null,
        report: null,
        reportContent: null,
        reportMessageTitle: null,
        reportHeader: null
    };

    var _params = {
        fromDate: null,
        toDate: null,
        site: null,
        from: null,
        isPagingCallBack: null,
        startIndex: null,
        usersPerPage: null
    };

    var _gridOption = {
        itemsPerPage: 50,
        currentPage: 0,
        startIndex: 0
    };

    self.initPage = function () {
        cachedDOMElements();
        renderDatePicker();
        bindingEvent();
    };

    var renderDatePicker = function () {
        var maxDate = new Date();
        var minDate = new Date();
        var month = maxDate.getMonth();

        if (month >= 3) {
            minDate.setMonth(maxDate.getMonth() - 3);
            minDate.setDate(maxDate.getDate());
        }
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
                maxDate: maxDate,
                minDate: minDate,
                dateFormat: "M dd, yy"
            },
            firstDatePicker: {
                Id: "firstDatePickerId1",
                Label: labelFrom,
                InitDate: today,
                LabelCss: "lbl-field col_2",
                TextboxCss: "edit-field col_6"
            },
            secondDatePicker: {
                Id: "firstDatePickerId2",
                Label: labelTo,
                InitDate: today,
                LabelCss: "lbl-field col_1",
                TextboxCss: "edit-field col_6"
            },
            onSelectDatePicker: function () {
            }
        });

        _tables.datepickerplugin = $("#datepickerplugin");

    };

    var cachedDOMElements = function () {
        _tables.report = $("#AllCustomerTurnOverContainer");
        _tables.reportContent = $("#AllCustomerTurnOverReportContent");
        _tables.reportHeader = $("#AllCustomerTurnOverHeader");
        _tables.reportMessageTitle = $("#AllCustomerTurnOverReportMessageTitle");
    };

    var bindingEvent = function () {
        resetHtml();
        $("#btnIBC").click(function () {
            setSite("BEST-ODDS");
        });
        $("#btn789Y").click(function () {
            setSite("789Y");
        });
        $("#btn961i").click(function () {
            setSite("961i.com");
        });
        $("#btnk998").click(function () {
            setSite("k998");
        });
        $("#btnTROJAN").click(function () {
            setSite("TROJAN");
        });
        $("#btnIBSBET").click(function () {
            setSite("IBSBET");
        });
        $("#btnKING119").click(function () {
            setSite("KING119");
        });
        $("#btnALL").click(function () {
            setSite("All");
        });
    };

    var resetGridOption = function () {
        _gridOption.itemsPerPage = 50;
        _gridOption.currentPage = 0;
        _gridOption.startIndex = 0;
    };

    var resetParam = function () {
        _params.fromDate = null;
        _params.toDate = null;
        _params.site = null;
        _params.from = null;
        _params.isPagingCallBack = null;
        _params.startIndex = null;
        _params.usersPerPage = null;
    };

    var setSite = function (site) {
        $("#Site").val(site);
        $("#SiteType").val(site);
        resetGridOption();
        resetParam();
        self.getReport();
    };

    self.setFromText = function () {
        var from = $('#BetFrom').val();
        var fromText = getFromText(from);
        $('#FromText').val(fromText);
    };

    var getFromText = function (from) {
        var fromText = "";

        if (from == "p") {
            fromText = "Phone";
        }
        else if (from == "i") {
            fromText = "Internet";
        }
        else {
            fromText = "Internet + Phone";
        }
        return fromText;
    };

    self.getReport = function () {
        AccReport.disableInput();
        getParams();

        if (validateParams() == true) {
            $.ajax({
                type: "POST",
                url: window.rootUrl + "WinLoss/AllCustomerTurnOver/GetReport",
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
        var dateRange = _tables.datepickerplugin.nexDatePicker("getDateRange");
        _params.fromDate = dateRange.FromDate + " 00:00:00";
        _params.toDate = dateRange.ToDate + " 23:59:59";
        _params.site = $("#Site").val();
        _params.from = $("#BetFrom").val();

        if (_params.usersPerPage == null) {
            _params.usersPerPage = 50;
        }
    };

    var validateParams = function () {
        return true;
    };

    var resetHtml = function () {
        $("#Site").val("");
    };

    self.renderReport = function (report) {
        var reportMessageTitle = "Current Selected Site : <b>" + $("#SiteType").val()
                                        + "</b> which bet via : <b>" + $("#FromText").val() + "</b>";
        _tables.reportMessageTitle.html(reportMessageTitle);

        var reportTitle = "All Customer Turn Over From " + _params.fromDate.replace("00:00:00", "") + " To " + _params.toDate.replace("23:59:59", "");
        var bodyData = {
            data: [],
            totalUsers: 0
        };
        var footerData = [];
        var info = null;
        var rowIndex = _gridOption.startIndex + 1;

        if (report != null && report.Site
            && report.Site.Rows != null && report.Site.Rows.length > 0) {

            var currentSubRow = [];
            var lengthSite = report.Site.Rows.length;
            for (var i = 0; i < lengthSite; i++) {
                var site = report.Site.Rows[i];
                var currentRow = {
                    Columns: [{ Name: "Site", Value: site.Name, MergeColNumber: 3}],
                    SubRows: []
                };

                for (var j = 0; j < site.Rows.length; j++) {
                    info = site.Rows[j];
                    currentSubRow = {
                        Columns: [{ Name: "Site", Value: rowIndex, CssClass: "remove-padding-left" },
                                  { Name: "UserName", Value: info.Name },
                                  { Name: "TurnOver", Value: info.Turnover, CssClass: "right" }
                                ],
                        CssClass: j % 2 == 0 ? "even" : ""
                    };
                    rowIndex++;
                    currentRow.SubRows.push(currentSubRow);
                }

                var totalSiteName = site.Name + "'s Total";
                currentSubRow = {
                    Columns: [
                            { Name: "Site" },
                            { Name: "UserName", Value: totalSiteName },
                            { Name: "TurnOver", Value: site.Turnover, CssClass: "right" }
                        ],
                    CssClass: "footer-site-total"
                };
                currentRow.SubRows.push(currentSubRow);
                bodyData.data.push(currentRow);
            }
            bodyData.totalUsers = report.TotalUsers;
            currentRow = {
                Columns: [
                        { Name: "Site" },
                        { Name: "UserName", Value: "GRAND TOTAL" },
                        { Name: "TurnOver", Value: report.Site.Turnover, CssClass: "right" }
                    ],
                CssClass: "footer-grand-total"
            };
            footerData.push(currentRow);

            self.renderGrid(bodyData, footerData, reportTitle);
        }
        else {
            self.renderGrid([], [], reportTitle);
        }

        _params.isPagingCallBack = false;
        _tables.reportHeader.removeClass("display-none");
        _tables.report.removeClass("display-none");
    };

    self.renderGrid = function (bodyData, footerData, reportTitle) {
        var gridOptions = {
            columns: [
                    { HeaderText: "No", Width: 60, Name: "Site", HeaderAlign: "center", CellAlign: "center", ShowCollapseExpand: true },
                    { HeaderText: "User", Width: 250, Name: "UserName", HeaderAlign: "center", CellAlign: "center" },
                    { HeaderText: "Turnover", Width: 150, Name: "TurnOver", HeaderAlign: "center",
                        CellAlign: "right", FormatNumber: FormatConstant.TWO_DECIMAL_NUMBER
                    }
                ],
            showTitle: true,
            titleInfo: {
                text: reportTitle,
                showExcelButton: true,
                exportExcelFunction: function () {
                    self.exportReport();
                },
                showCollapseExpandButton: true
            },
            showTree: true,
            bodyRows: bodyData.data,
            showDistinctRow: false,
            showFooter: true,
            footerRows: footerData,
            gridExpandHeight: 210,
            showPager: true,
            customPager: true,
            pagerOption: {
                startPerPage: 50,
                totalItem: bodyData.totalUsers,
                itemsPerPageArray: [50, 100, 200, 500],
                itemsPerPage: _gridOption.itemsPerPage,
                currentPage: _gridOption.currentPage,
                pagerSelectCallBack: ReRenderAfterPaged
            }
        };

        _tables.reportContent.fanexGrid(gridOptions);

        function ReRenderAfterPaged(pageIndex, itemsPerPage, jq) {
            _params.isPagingCallBack = true;
            _params.startIndex = pageIndex >= 0 ? (pageIndex * itemsPerPage) : 0;
            _params.usersPerPage = itemsPerPage;

            _gridOption.itemsPerPage = itemsPerPage;
            _gridOption.currentPage = pageIndex;
            _gridOption.startIndex = _params.startIndex;

            self.getReport();
            return false;
        }
    };

    self.exportReport = function () {
        AccReport.downloadFile(window.rootUrl + "WinLoss/AllCustomerTurnOver/ExportReport", _params);
    };
}