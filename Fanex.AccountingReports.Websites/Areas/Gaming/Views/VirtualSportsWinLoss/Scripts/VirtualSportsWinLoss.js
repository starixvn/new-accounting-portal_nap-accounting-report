
Namespace.Register("Fanex.AccountingReports.Gaming.VirtualSportsWinLoss");

Fanex.AccountingReports.Gaming.VirtualSportsWinLoss = function () {
    var self = this;

    self._table = {
        report: null,
        reportContent: null,
        drawFilter: null,
        renderMonthPicker: null,
        renderSportsType: null,
        excludeTestData: null,
        btnGenerate: null
    };

    var _params = {
        fromDate: null,
        toDate: null,
        excludeTest: null,
        drawFilter: null,
        sportType: null,
        isPagingCallBack: null,
        startIndex: null,
        usersPerPage: null
    };

    var _gridOption = {
        itemsPerPage: 50,
        currentPage: 0,
        startIndex: 0
    };

    function CurrencyAndDate(date, name) {
        this._date = date;
        this._currencyName = name;
    };

    self.initPage = function () {
        catchedDOMElement();
        setDefaultValue();
        renderCalendar();
        getSportsType();
        bindingEvent();
    };

    var catchedDOMElement = function () {
        self._table.report = $('#VirtualSportsWinLossContainer');
        self._table.reportContent = $('#VirtualSportsWinLossContent');
        self._table.drawFilter = $('#DrawFilter');
        self._table.renderMonthPicker = $('#renderMonthPicker');
        self._table.renderSportsType = $('#SportsType');
        self._table.excludeTestData = $('#ExcludeTest');
        self._table.btnGenerate = $('#btnGenerate');
    };

    var setDefaultValue = function () {
        self._table.reportContent.empty();
        self._table.drawFilter.find('option[value="1"]').attr('selected', true);
        self._table.excludeTestData.attr('checked', 'checked');
    };

    var renderCalendar = function () {
        self._table.renderMonthPicker.monPickr({
            plugInId: '#renderMonthPicker',
            today: today,
            maxDate: today,
            minDate: minDate,
            showDateRange: true,
            fromDayId: 'ddlFrom',
            month: previousMonth,
            sortDesc: true,
            width: 169,
            log: function (msg) {
                console.warn(msg);
            }
        });
    };

    var getSportsType = function () {
        $.ajax({
            type: "POST",
            url: window.rootUrl + "Gaming/VirtualSportsWinLoss/GetSportsCollection",
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            beforeSend: function () {
                AccReport.disableInput();
            },
            success: function (sportsType) {
                self.renderSports(sportsType);
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
    };

    var bindingEvent = function () {
        self._table.btnGenerate.click(function () {
            self.getReport();
        });
    };

    self.renderSports = function (sportsType) {
        var sportsData = [];
        if (sportsType != null && sportsType.length > 0) {
            var numberOfSports = sportsType.length;

            for (var i = 0; i < numberOfSports; i++) {
                sportsData.push({
                    Id: self._table.renderSportsType.attr('id') + sportsType[i].Id,
                    Value: sportsType[i].Id,
                    Text: sportsType[i].Name,
                    Checked: sportsType[i].Active,
                    Enable: true,
                    CssClass: "",
                    Items: []
                });
            }
        }
        else {
            sportsData = [];
        }

        self._table.renderSportsType.multiSelect(
         {
             minWidth: 180,
             maxHeight: 450,
             shadow: true,
             showHeader: true,
             isCheckAll: true,
             showFooter: false,
             checkAllText: 'All',
             listData: sportsData,
             isUsedCustomCheckAllText: true,
             customCheckAllText: 'All Selected',
             onCheck: function () {
             }
         });
    };

    self.getReport = function () {
        AccReport.disableInput();
        _params = self.getParams();

        if (validateParams()) {
            $.ajax({
                type: "POST",
                url: window.rootUrl + "Gaming/VirtualSportsWinLoss/GetReport",
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

    var getSelectedSport = function () {
        var selectedList = $('#SportsType').multiSelect('getCheckedItems');
        if (selectedList != null && selectedList.length > 0) {
            var numberOfSelected = selectedList.length;

            for (var i = 0; i < numberOfSelected; i++) {
                if (i == 0) {
                    _params.sportType = selectedList[i];
                }
                else {
                    _params.sportType += ',' + selectedList[i];
                }
            }
        }
    };

    self.getParams = function () {
        _params.fromDate = self._table.renderMonthPicker.monPickr("getFromDate", "MMM dd, yyyy");
        _params.toDate = self._table.renderMonthPicker.monPickr("getToDate", "MMM dd, yyyy");
        _params.drawFilter = $('#DrawFilter').val();
        _params.excludeTest = $('#ExcludeTest').is(':checked');
        getSelectedSport();

        if (_params.usersPerPage == null) {
            _params.usersPerPage = 50;
        }

        return _params;
    };

    var validateParams = function () {
        return true;
    };

    self.renderReport = function (report) {
        var reportTitle = "Virtual Sports Win Loss From " + _params.fromDate + " To " + _params.toDate;

        if (report != null && report.Rows != null
                    && report.Rows.length > 0 && report.Currencies.length > 0) {
            var bodyData = [];
            var footerData = [];
            var numberOfRows = report.Date;

            // push data to body
            bodyData = renderBodyData(report);
            // push data to footer
            footerData = renderFooterData(report);

            self.renderGrid(reportTitle, bodyData, footerData, numberOfRows);
        }
        else {
            self.renderGrid(reportTitle, [], [], 0);
        }
    };

    self.renderGrid = function (reportTitle, bodyData, footerData, numberOfRows) {
        var groupHeader = [
            {
                Columns: [
                    { HeaderText: "Date", Name: "Date", MergeRowNumber: 2 },
                    { HeaderText: "Currency", Name: "Currency", MergeRowNumber: 2 },
                    { HeaderText: "Bet Count", Name: "BetCount", MergeRowNumber: 2 },
                    { HeaderText: "Turnover", Name: "Turnover", MergeRowNumber: 2 },
                    { HeaderText: "Member Win Loss", Name: "MemberWinLoss", MergeRowNumber: 2 },
                    { HeaderText: "Win Loss", Name: "PositionWinLoss", MergeColNumber: 2 },
                    { HeaderText: "Amount due to Inspired", Name: "AmountDue", MergeRowNumber: 2 }
                ]
            }
        ];

        var columns = [
            { HeaderText: "Date", Name: "Date", Width: 80, HeaderAlign: "center", CellAlign: "center" },
            { HeaderText: "Currency", Name: "Currency", Width: 100, HeaderAlign: "center",
                CellAlign: "center", FormatNumber: FormatConstant.INT_NUMBER
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
                text: reportTitle,
                showExcelButton: true,
                exportExcelFunction: function () {
                    self.exportReport();
                }
            },
            showFooter: true,
            footerRows: footerData,
            gridExpandHeight: 190,
            showPager: true,
            customPager: true,
            pagerOption: {
                itemsPerPage: _gridOption.itemsPerPage,
                currentPage: _gridOption.currentPage,
                itemsPerPageArray: [50, 100, 200, 500],
                totalItem: numberOfRows,
                pagerSelectCallBack: ReRenderAfterPaged
            }
        };

        self._table.reportContent.removeClass("display-none").fanexGrid(gridOptions);
        self._table.reportContent.find('tr').removeClass('freegrid-even').removeClass('highlight-row');
        // reset _gridOption and reset _params.
        resetGridOptionAndParamsUseInReRender();

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
        AccReport.downloadFile(window.rootUrl + "Gaming/VirtualSportsWinLoss/ExportReport", _params);
    };

    var renderBodyData = function (report) {
        var bodyData = [];
        var currentRow = {
            Columns: []
        };

        var numberOfDate = report.Rows.length;

        for (var i = 0; i < numberOfDate; i++) {
            var dateWinLoss = report.Rows[i];
            var numberOfCurrencyByDate = dateWinLoss.Currencies.length;

            for (var j = 0; j < numberOfCurrencyByDate; j++) {
                var currency = dateWinLoss.Currencies[j];
                // create object contain the date and the currency name belong to this date.
                // this object will help to create the link to view detail report at date.
                var currencyAndDate = new CurrencyAndDate(dateWinLoss.Date, currency.Name);

                if (j == 0) {
                    // create a link to currency if the row not total row.
                    if (dateWinLoss.Date != "Total") {
                        currentRow = {
                            Columns: [
                                    { Name: "Date", Value: dateWinLoss.Date,
                                        MergeRowNumber: numberOfCurrencyByDate
                                    },
                                    { Name: "Currency", Value: currencyAndDate,
                                        CustomCellRender: function (currentCell) {
                                            return self.getNavigationLink(currentCell.Value);
                                        }
                                    },
                                    { Name: "BetCount", Value: currency.BetCount },
                                    { Name: "Turnover", Value: currency.Turnover },
                                    { Name: "MemberWinLoss", Value: currency.MemberWinLoss },
                                    { Name: "PositionWinLoss", Value: currency.PositionWinLoss },
                                    { Name: "CompanyWinLoss", Value: currency.CompanyWinLoss },
                                    { Name: "AmountDue", Value: currency.AmountDueInspired }
                                ]
                        };
                    }
                    else {
                        currentRow = {
                            Columns: [
                                    { Name: "Date", Value: dateWinLoss.Date,
                                        MergeRowNumber: numberOfCurrencyByDate, TdClass: "total"
                                    },
                                    { Name: "Currency", Value: currency.Name, TdClass: "total" },
                                    { Name: "BetCount", Value: currency.BetCount, TdClass: "total" },
                                    { Name: "Turnover", Value: currency.Turnover, TdClass: "total" },
                                    { Name: "MemberWinLoss", Value: currency.MemberWinLoss,
                                        TdClass: "total"
                                    },
                                    { Name: "PositionWinLoss", Value: currency.PositionWinLoss,
                                        TdClass: "total"
                                    },
                                    { Name: "CompanyWinLoss", Value: currency.CompanyWinLoss,
                                        TdClass: "total"
                                    },
                                    { Name: "AmountDue", Value: currency.AmountDueInspired,
                                        TdClass: "total"
                                    }
                                ]
                        };
                    }
                }
                else if (dateWinLoss.Date == "Total") {
                    currentRow = {
                        Columns: [
                                { Name: "Currency", Value: currency.Name, TdClass: "total" },
                                { Name: "BetCount", Value: currency.BetCount, TdClass: "total" },
                                { Name: "Turnover", Value: currency.Turnover, TdClass: "total" },
                                { Name: "MemberWinLoss", Value: currency.MemberWinLoss, TdClass: "total" },
                                { Name: "PositionWinLoss", Value: currency.PositionWinLoss,
                                    TdClass: "total"
                                },
                                { Name: "CompanyWinLoss", Value: currency.CompanyWinLoss, TdClass: "total" },
                                { Name: "AmountDue", Value: currency.AmountDueInspired, TdClass: "total" }
                            ]
                    }
                }
                else {
                    currentRow = {
                        Columns: [
                                { Name: "Currency", Value: currencyAndDate,
                                    CustomCellRender: function (currentCell) {
                                        return self.getNavigationLink(currentCell.Value);
                                    }
                                },
                                { Name: "BetCount", Value: currency.BetCount },
                                { Name: "Turnover", Value: currency.Turnover },
                                { Name: "MemberWinLoss", Value: currency.MemberWinLoss },
                                { Name: "PositionWinLoss", Value: currency.PositionWinLoss },
                                { Name: "CompanyWinLoss", Value: currency.CompanyWinLoss },
                                { Name: "AmountDue", Value: currency.AmountDueInspired }
                            ]
                    }
                }

                bodyData.push(currentRow);
            }
        }

        return bodyData;
    };

    var renderFooterData = function (report) {
        var footerData = [];
        var currentRow = {
            Columns: []
        };

        // render grand total.
        if (report.Currencies != null && report.Currencies.length > 0) {
            var numberOfTotal = report.Currencies.length;

            for (var k = 0; k < numberOfTotal; k++) {
                var grandTotal = report.Currencies[k];
                currentRow = {
                    Columns: [
                            { Name: "Date", Value: grandTotal.Name, MergeColNumber: 2,
                                TdClass: "grand-total"
                            },
                            { Name: "BetCount", Value: grandTotal.BetCount,
                                TdClass: "grand-total"
                            },
                            { Name: "Turnover", Value: grandTotal.Turnover, TdClass: "grand-total" },
                            { Name: "MemberWinLoss", Value: grandTotal.MemberWinLoss,
                                TdClass: "grand-total"
                            },
                            { Name: "PositionWinLoss", Value: grandTotal.PositionWinLoss,
                                TdClass: "grand-total"
                            },
                            { Name: "CompanyWinLoss", Value: grandTotal.CompanyWinLoss,
                                TdClass: "grand-total"
                            },
                            { Name: "AmountDue", Value: grandTotal.AmountDueInspired,
                                TdClass: "grand-total"
                            }
                        ]
                };

                footerData.push(currentRow);
            }
        }

        return footerData;
    };

    self.getNavigationLink = function (currentCell) {
        return "<div class='link bold' onclick=\"ViewReportDetails('" + currentCell._currencyName + "'"
                                                            + "," + "'" + currentCell._date + "')\">"
                                        + currentCell._currencyName + "</div>";
    };

    self.showReport = function () {
        self._table.report.removeClass("display-none");
    };

    self.hideReport = function () {
        self._table.report.addClass("display-none");
    };

    var resetGridOption = function () {
        _gridOption.itemsPerPage = 50;
        _gridOption.currentPage = 0;
        _gridOption.startIndex = 0;
    };

    var resetParam = function () {
        _params.isPagingCallBack = null;
        _params.startIndex = null;
        _params.usersPerPage = null;
    };

    var resetGridOptionAndParamsUseInReRender = function () {
        resetGridOption();
        resetParam();
    };
};