Namespace.Register("Fanex.AccountingReports.Special.SMATurnover");

Fanex.AccountingReports.Special.SMATurnover = function () {
    var self = this;

    var _table = {
        ddlProduct: null,
        ddlAccountStatus: null,
        btnGenerate: null,
        datepickerplugin: null,
        reportContainer: null,
        report: null,
        productList: null,
        accountStatus: null
    };

    var cachedDomElememts = function () {
        _table.ddlProduct = $("#ddlProduct");
        _table.ddlAccountStatus = $("#ddlAccountStatus");
        _table.btnGenerate = $("#btnGenerate");
        _table.reportContainer = $("#reportContainer");
        _table.report = $("#reportContent");
    };

    var cachedData = {
        currentPage: 0,
        itemsPerPage: 50,
        itemsPerPageArray: [50, 100, 200, 300, 500],
        currentReport: null
    };

    self.reportParams = {
        WinlostDate: null,
        MoveBetsDay: null,
        ShowSB: null,
        ShowRB: null,
        ShowNG: null,
        ShowKN: null,
        ShowLC: null,
        ShowVS: null,
        ShowCS: null,
        ShowOpened: null,
        ShowClosed: null,
        ShowSuspended: null,
        SortColumnName: null,
        SortDirection: null
    };

    self.Enums = {
        sortDirection: {
            Asc: "ASC",
            Desc: "DESC"
        }
    };

    self.initPage = function () {
        cachedDomElememts();
        self.renderDatePicker();
        self.getProducts();
        self.getAccountStatus();
        self.bindingEvents();
    };

    self.bindingEvents = function () {
        _table.btnGenerate.click(function () {
            self.resetParams();
            self.getSMAsTurnOver();
        });
    };

    self.resetParams = function () {
        cachedData.currentPage = 0;
        cachedData.itemsPerPage = 50;
        self.reportParams.WinlostDate = null;
        self.reportParams.MoveBetsDay = null;
        self.reportParams.ShowSB = null;
        self.reportParams.ShowRB = null;
        self.reportParams.ShowNG = null;
        self.reportParams.ShowKN = null;
        self.reportParams.ShowLC = null;
        self.reportParams.ShowVS = null;
        self.reportParams.ShowCS = null;
        self.reportParams.ShowOpened = null;
        self.reportParams.ShowClosed = null;
        self.reportParams.ShowSuspended = null;
        self.reportParams.SortColumnName = null;
        self.reportParams.SortDirection = null;
    };

    self.renderDatePicker = function () {
        var toDay = new Date(today);
        $('#datepickerplugin').nexDatePicker({
            nexDatePickerType: "single",
            firstDatePicker: {
                Id: "datePickerInput",
                Label: labelDate,
                InitDate: toDay,
                LabelCss: "lbl-field col_2",
                TextboxCss: "edit-field col_5"
            },

            datePickerFormat: {
                dateFormat: FormatConstant.M_DD_YY_STRING,
                changeMonth: true,
                changeYear: true,
                minDate: new Date(minDate),
                maxDate: toDay,
                showOtherMonths: true,
                selectOtherMonths: true
            }
        });

        _table.datepickerplugin = $("#datepickerplugin");
    };

    self.getProducts = function () {
        AccReport.disableInput();
        AccReport.utility.CallAjax(this,
                                    window.rootUrl + "Special/SMATurnover/GetProducts",
                                    null,
                                    self.renderProducts,
                                    self.renderProducts,
                                    AccReport.enableInput);
    };

    self.renderProducts = function (rs) {
        var noItemText = 'No product found.';
        var ddwnProducts = [];
        var prefixId = 'ddlProduct_';
        var numberProducts = rs.length;

        for (var i = 0; i < numberProducts; i++) {
            var currentProduct = rs[i];
            ddwnProducts.push({
                Id: prefixId + currentProduct.Id,
                Value: currentProduct.Id,
                Text: currentProduct.Name,
                Checked: true,
                Enable: true,
                CssClass: "",
                Items: []
            });
        }

        $("#ddlProduct").multiSelect(
        {
            minWidth: 140,
            maxHeight: 450,
            shadow: true,
            showHeader: true,
            isCheckAll: true,
            showFooter: false,
            showTooltip: true,
            checkAllText: 'All Products',
            isUsedCustomCheckAllText: true,
            customCheckAllText: 'All Products',
            noItemText: noItemText,
            selectedList: 3,
            listData: ddwnProducts
        });

        _table.productList = {
            sportsbook: $('#ddlProduct_0'),
            racing: $('#ddlProduct_1'),
            rngcasino: $('#ddlProduct_2'),
            numbergame: $('#ddlProduct_3'),
            liveCasino: $('#ddlProduct_4'),
            virtualSports: $('#ddlProduct_5'),
            keno: $('#ddlProduct_6')
        };
    };

    self.getAccountStatus = function () {
        AccReport.disableInput();
        AccReport.utility.CallAjax(this,
                                    window.rootUrl + "Special/SMATurnover/GetAccountStatus",
                                    null,
                                    self.renderAccountStatus,
                                    self.renderAccountStatus,
                                    AccReport.enableInput);
    };

    self.renderAccountStatus = function (rs) {
        var noItemText = 'No status found.';
        var ddwnAccountStatus = [];
        var prefixId = 'ddlAccountStatus_';
        var numberAccountStatus = rs.length;

        for (var i = 0; i < numberAccountStatus; i++) {
            var currentStatus = rs[i];
            ddwnAccountStatus.push({
                Id: prefixId + currentStatus.Id,
                Value: currentStatus.Id,
                Text: currentStatus.Name,
                Checked: true,
                Enable: true,
                CssClass: "",
                Items: []
            });
        }

        $("#ddlAccountStatus").multiSelect(
        {
            minWidth: 140,
            maxHeight: 450,
            shadow: true,
            showHeader: true,
            isCheckAll: true,
            showFooter: false,
            showTooltip: true,
            checkAllText: 'All',
            isUsedCustomCheckAllText: true,
            customCheckAllText: 'All',
            noItemText: noItemText,
            selectedList: 3,
            listData: ddwnAccountStatus
        });

        _table.accountStatus = {
            opened: $('#ddlAccountStatus_0'),
            suspended: $('#ddlAccountStatus_1'),
            closed: $('#ddlAccountStatus_2')
        };
    };

    self.getCheckboxStatus = function (checkboxId) {
        return checkboxId.attr("checked") ? true : false;
    };

    self.getParams = function () {
        var moveBet = moveBetDay;

        self.reportParams.WinlostDate = _table.datepickerplugin.nexDatePicker("getSingleDate");
        self.reportParams.MoveBetsDay = moveBet;
        self.reportParams.ShowSB = self.getCheckboxStatus(_table.productList.sportsbook);
        self.reportParams.ShowRB = self.getCheckboxStatus(_table.productList.racing);
        self.reportParams.ShowNG = self.getCheckboxStatus(_table.productList.numbergame);
        self.reportParams.ShowKN = self.getCheckboxStatus(_table.productList.keno);
        self.reportParams.ShowLC = self.getCheckboxStatus(_table.productList.liveCasino);
        self.reportParams.ShowVS = self.getCheckboxStatus(_table.productList.virtualSports);
        self.reportParams.ShowCS = self.getCheckboxStatus(_table.productList.rngcasino);
        self.reportParams.ShowOpened = self.getCheckboxStatus(_table.accountStatus.opened);
        self.reportParams.ShowClosed = self.getCheckboxStatus(_table.accountStatus.closed);
        self.reportParams.ShowSuspended = self.getCheckboxStatus(_table.accountStatus.suspended);

        return self.reportParams;
    };

    self.checkReportParams = function (params) {
        AccReport.popUp.removeHighLight();

        if (params.ShowSB == false &&
            params.ShowRB == false &&
            params.ShowNG == false &&
            params.ShowKN == false &&
            params.ShowLC == false &&
            params.ShowVS == false &&
            params.ShowCS == false) {
            AccReport.popUp.show("Please select  at least one product!", "ddlProduct");
            return false;
        }

        if (params.ShowOpened == false && params.ShowClosed == false && params.ShowSuspended == false) {
            AccReport.popUp.show("Please select  at least one account status!", "ddlAccountStatus");
            return false;
        }

        return true;
    };

    self.getSMAsTurnOver = function () {
        var params = self.getParams();

        if (self.checkReportParams(params)) {
            AccReport.disableInput();
            AccReport.utility.CallAjax(this,
                                    window.rootUrl + "Special/SMATurnover/GetSMAsTurnover",
                                    params,
                                    self.renderGrid,
                                    self.renderGrid,
                                    AccReport.enableInput);
        }
    };

    self.sortReport = function () {
        var params = self.getParams();

        AccReport.disableInput();
        AccReport.utility.CallAjax(this,
                                    window.rootUrl + "Special/SMATurnover/SortReport",
                                    params,
                                    self.renderGrid,
                                    self.renderGrid,
                                    AccReport.enableInput);
    };

    self.smasTurnOverGridOptions = function (rs) {
        var bodyRows = [];
        var footerData = [];
        var rowNumber = 0;
        var startItemPerPage = cachedData.currentPage * cachedData.itemsPerPage;
        var title = "SMA Turnover Report on" + " " + self.reportParams.WinlostDate;
        var groupHeader = [
            {
                Columns: [
                    { HeaderText: "#", Name: "ColNo", MergeRowNumber: 2 },
                    { HeaderText: "Group Headline", Name: "ColGroupHeadline", MergeRowNumber: 2 },
                    { HeaderText: "Username", Name: "ColUsername", MergeRowNumber: 2 },
                    { HeaderText: "Currency", Name: "ColCurrency", MergeRowNumber: 2 },
                    { HeaderText: "Position", Name: "ColPosition", MergeRowNumber: 2 },
                    { HeaderText: "Turnover", Name: "ColTurnover", MergeRowNumber: 2 },
                    { HeaderText: "Turnover(RM)", Name: "ColTurnoverRM", MergeRowNumber: 2 },
                    { HeaderText: "Member Total", Name: "ColMemberTurnover", MergeColNumber: 2 }
                ]
            }
        ];

        var columns = [
            { HeaderText: "#", Width: 60, Name: "ColNo", HeaderAlign: "center", CellAlign: "center" },
            { HeaderText: "Group Headline", Width: 100, Name: "ColGroupHeadline", HeaderAlign: "center", CellAlign: "center" },
            { HeaderText: "Username", Width: 100, Name: "ColUsername", HeaderAlign: "center", CellAlign: "center" },
            { HeaderText: "Currency", Width: 80, Name: "ColCurrency", HeaderAlign: "center", CellAlign: "center" },
            { HeaderText: "Position", Width: 80, Name: "ColPosition", HeaderAlign: "center", CellAlign: "center" },
            { HeaderText: "Turnover", Width: 120, Name: "ColTurnover", HeaderAlign: "center", CellAlign: "right", FormatNumber: FormatConstant.TWO_DECIMAL_NUMBER },
            { HeaderText: "Turnover(RM)", Width: 120, Name: "ColTurnoverRM", Sort: true, SortDirection: self.getSortDirection("RMTurnOver"), HeaderAlign: "center", CellAlign: "right", FormatNumber: FormatConstant.TWO_DECIMAL_NUMBER },
            { HeaderText: "Turnover", Width: 120, Name: "ColMemberTurnover", HeaderAlign: "center", CellAlign: "right", FormatNumber: FormatConstant.TWO_DECIMAL_NUMBER },
            { HeaderText: "Turnover(RM)", Width: 120, Name: "ColMemberTurnoverRM", Sort: true, SortDirection: self.getSortDirection("MemberRMTurnOver"), HeaderAlign: "center", CellAlign: "right", FormatNumber: FormatConstant.TWO_DECIMAL_NUMBER }
        ];

        if (rs && rs.ReportRows.length > 0) {
            var endGroupHeadlineInfo = self.getEndGroupHeadlineByPage(cachedData.currentPage, cachedData.itemsPerPage, rs);
            var startGroupHeadlineInfo = self.getStartGroupHeadlineByPage(endGroupHeadlineInfo.endGroupHeadlineIndex, endGroupHeadlineInfo.endRowOfGroupHeadlineIndex, rs);

            var startGroupHeadlineIndex = startGroupHeadlineInfo.startGroupHeadlineIndex;
            var startRowOfGroupHeadlineIndex = startGroupHeadlineInfo.startRowOfGroupHeadlineIndex;

            for (var groupHeadlineIndex = startGroupHeadlineIndex; groupHeadlineIndex <= endGroupHeadlineInfo.endGroupHeadlineIndex; groupHeadlineIndex++) {
                var currentGroup = rs.ReportRows[groupHeadlineIndex];
                var numberRowsOfGroup = currentGroup.ReportRows.length;
                for (var rowIndex = startRowOfGroupHeadlineIndex; rowIndex < numberRowsOfGroup; rowIndex++) {
                    var currentRow = currentGroup.ReportRows[rowIndex];
                    var currentRowData = [];

                    currentRowData.push({ Name: "ColNo", Value: startItemPerPage + 1 });

                    if (rowIndex == startRowOfGroupHeadlineIndex) {
                        currentRowData.push(
                            { Name: "ColGroupHeadline", Value: currentGroup.GroupHeadline, MergeRowNumber: numberRowsOfGroup - startRowOfGroupHeadlineIndex,
                                TdClass: "groupHeadlineTootip"
                            });
                    }

                    currentRowData.push(
                        { Name: "ColUsername", Value: currentRow.UserName },
                        { Name: "ColCurrency", Value: currentRow.Currency },
                        { Name: "ColPosition", Value: currentRow.Position },
                        { Name: "ColTurnover", Value: currentRow.TurnOver },
                        { Name: "ColTurnoverRM", Value: currentRow.RMTurnOver },
                        { Name: "ColMemberTurnover", Value: currentRow.MemberTurnOver },
                        { Name: "ColMemberTurnoverRM", Value: currentRow.MemberRMTurnOver });

                    if (rowNumber < cachedData.itemsPerPage) {
                        bodyRows.push({
                            Columns: currentRowData
                        });

                        rowNumber++;
                        startItemPerPage++;
                    }
                }

                var subTotal = [
                        { Name: "ColNo", Value: currentGroup.GroupHeadline + " " + "Total", MergeColNumber: 5, TdClass: "bold subTotal" },
                        { Name: "ColTurnover", Value: "", TdClass: "bold subTotal" },
                        { Name: "ColTurnoverRM", Value: currentGroup.Total.RMTurnOver, TdClass: "bold subTotal" },
                        { Name: "ColMemberTurnover", Value: "", TdClass: "bold subTotal" },
                        { Name: "ColMemberTurnoverRM", Value: currentGroup.Total.MemberRMTurnOver, TdClass: "bold subTotal" }
                    ];

                bodyRows.push({ Columns: subTotal });

                startRowOfGroupHeadlineIndex = 0;
            }

            footerData.push({
                Columns: [
                    { Name: "ColNo", Value: "Grand Total", MergeColNumber: 5 },
                    { Name: "ColTurnover", Value: "" },
                    { Name: "ColTurnoverRM", Value: rs.Total.RMTurnOver },
                    { Name: "ColMemberTurnover", Value: "" },
                    { Name: "ColMemberTurnoverRM", Value: "" }
                ]
            });
        }

        var gridOptions = {
            showTitle: true,
            titleInfo: {
                text: title,
                showExcelButton: true,
                exportExcelFunction: function () {
                    self.exportData();
                }
            },
            showFooter: true,
            footerRows: footerData,
            customizeHeader: true,
            groupHeaderRows: groupHeader,
            columns: columns,
            bodyRows: bodyRows,
            gridExpandHeight: 160,
            showDistinctRow: false,
            showHighlightRow: false,
            customPager: true,
            showPager: true,
            sortOption:
            {
                serverSortFunction: function (sortColumnName, sortDirection) {
                    var sortColumnName = self.getSortColumnName(sortColumnName);
                    self.reportParams.SortDirection = sortDirection.toLowerCase();
                    self.reportParams.SortColumnName = sortColumnName;
                    self.sortReport();
                }
            },
            pagerOption: {
                itemsPerPage: cachedData.itemsPerPage,
                currentPage: cachedData.currentPage,
                totalItem: rs.ReportLength,
                itemsPerPageArray: cachedData.itemsPerPageArray,
                pagerSelectCallBack: function (page_index, itemsPerPage, jq) {
                    cachedData.itemsPerPage = itemsPerPage;
                    cachedData.currentPage = page_index;
                    self.renderGrid(rs);
                }
            }
        };

        return gridOptions;
    };

    self.renderGrid = function (rs) {
        _table.reportContainer.removeClass("display-none");
        var gridOptions = self.smasTurnOverGridOptions(rs);
        _table.report.removeClass("display-none").fanexGrid(gridOptions);

        $("#tblContent_reportContent tbody tr").each(function () {
            var groupHeadline = $(this).find("td.groupHeadlineTootip");

            var title = $(groupHeadline).children().text();
            var groupHeadlineTitle = "Group headline: " + title;
            $(groupHeadline).attr("title", groupHeadlineTitle);
            $(groupHeadline).tipsy({
                html: true,
                opacity: 0.9,
                gravity: 'sw'
            });

            $(groupHeadline).hover(function (e) {
                if (parseInt($('div.tipsy-sw').css('top')) < 0)
                    $('div.tipsy-sw').css({ top: 190 });
            });
        });
    };

    self.getEndGroupHeadlineByPage = function (pageIndex, itemsPerPage, reportData) {
        var groupHeadlines = reportData.ReportRows;
        var endItemIndex = (pageIndex + 1) * itemsPerPage;
        var numberRows = 0;

        for (var groupHeadlineIndex = 0; groupHeadlineIndex < groupHeadlines.length; groupHeadlineIndex++) {
            var currentGroupHeadline = groupHeadlines[groupHeadlineIndex];
            var numberRowsOfGroupHeadline = currentGroupHeadline.ReportRows.length;
            for (var rowIndex = 0; rowIndex < numberRowsOfGroupHeadline; rowIndex++) {
                numberRows += 1;

                if (numberRows == endItemIndex) {
                    return {
                        endGroupHeadlineIndex: groupHeadlineIndex,
                        endRowOfGroupHeadlineIndex: rowIndex
                    };
                }

                if (groupHeadlineIndex == groupHeadlines.length - 1 &&
                    rowIndex == groupHeadlines[groupHeadlines.length - 1].ReportRows.length - 1 &&
                    numberRows < endItemIndex) {
                    return {
                        endGroupHeadlineIndex: groupHeadlines.length - 1,
                        endRowOfGroupHeadlineIndex: groupHeadlines[groupHeadlines.length - 1].ReportRows.length - 1
                    };
                }
            }
        }
    };

    self.getStartGroupHeadlineByPage = function (endGroupHeadlineIndex, endGroupHeadlineRowIndex, reportData) {
        var groupHeadlines = reportData.ReportRows;
        var reportLength = reportData.ReportLength;
        var numPages = reportLength / cachedData.itemsPerPage;

        if (Math.ceil(numPages) != cachedData.currentPage + 1) {
            return self.getStartGroupHeadlineByNumberItems(endGroupHeadlineIndex, endGroupHeadlineRowIndex, cachedData.itemsPerPage, groupHeadlines);
        }
        else {
            var startItemPerPage = cachedData.currentPage * cachedData.itemsPerPage;
            var numberItemsOfEndpage = reportLength - startItemPerPage;
            return self.getStartGroupHeadlineByNumberItems(endGroupHeadlineIndex, endGroupHeadlineRowIndex, numberItemsOfEndpage, groupHeadlines);
        }
    };

    self.getStartGroupHeadlineByNumberItems = function (endGroupHeadlineIndex, endGroupHeadlineRowIndex, numberItemsOfPage, groupHeadlines) {
        var numberRows = 0;

        for (var groupHeadlineIndex = endGroupHeadlineIndex; groupHeadlineIndex >= 0; groupHeadlineIndex--) {
            var currentGroupHeadline = groupHeadlines[groupHeadlineIndex];
            var endRowIndex = groupHeadlineIndex == endGroupHeadlineIndex ? endGroupHeadlineRowIndex : currentGroupHeadline.ReportRows.length - 1;
            for (var rowIndex = endRowIndex; rowIndex >= 0; rowIndex--) {
                numberRows += 1;

                if (numberRows == numberItemsOfPage) {
                    return {
                        startGroupHeadlineIndex: groupHeadlineIndex,
                        startRowOfGroupHeadlineIndex: rowIndex
                    };
                }
            }
        }
    };

    self.exportData = function () {
        AccReport.downloadFile(window.rootUrl + "Special/SMATurnover/ExportData", self.reportParams);
    };

    self.getSortColumnName = function (columnName) {
        var sortColumnName = null;
        switch (columnName) {
            case "ColTurnoverRM":
                sortColumnName = "RMTurnOver";
                break;
            case "ColMemberTurnoverRM":
                sortColumnName = "MemberRMTurnOver";
                break;
        }

        return sortColumnName;
    };

    self.getSortDirection = function (columnName) {
        return columnName == self.reportParams.SortColumnName ? self.reportParams.SortDirection : '';
    };
};

$(document).ready(function () {
    var smaTurnoverReport = new Fanex.AccountingReports.Special.SMATurnover();
    smaTurnoverReport.initPage();
});