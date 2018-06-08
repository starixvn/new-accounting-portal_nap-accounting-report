
Namespace.Register("Fanex.AccountingReports.WinLoss.TreeWinLoss.TreeWinLossByComm");
Fanex.AccountingReports.WinLoss.TreeWinLoss.TreeWinLossByComm = function () {
    var self = this;
    var _tables = {
        reportContainer: null,
        report: null
    };

    self.Params = {
        reportParams: {
            FromDate: new Date(),
            ToDate: new Date(),
            RoleId: 0,
            Currency: 0,
            ProductSelection: null,
            Products: null,
            CurrencyName: null,
            IsNormalMode: true
        }
    };

    self.initPage = function () {
        cachedDOMElements();
    };

    var cachedDOMElements = function () {
        _tables.reportContainer = $("#treeWinLossByCommContainer");
        _tables.report = $("#treeWinLossByComm");
    };

    self.hideReport = function () {
        _tables.reportContainer.addClass("display-none");
        _tables.report.addClass("display-none");
    };

    self.showReport = function () {
        _tables.reportContainer.removeClass("display-none");
        _tables.report.removeClass("display-none");
    };

    self.customSortNumber = function (col1Value, col2Value, sortDirection) {
        var isNumber1 = !isNaN(col1Value);
        var isNumber2 = !isNaN(col2Value);

        if (isNumber1 && isNumber2) {
            var parseValCol1 = parseFloat(col1Value);
            var parseValCol2 = parseFloat(col2Value);
            if (sortDirection.toLowerCase() === 'asc') {
                return parseValCol1 > parseValCol2 ? 1 : -1;
            }
            else {
                return parseValCol1 < parseValCol2 ? 1 : -1;
            }
        }

        if (sortDirection.toLowerCase() === 'asc') {
            if (!isNumber1) {
                return 1;
            }
        }
        else {
            if (!isNumber2) {
                return 1;
            }
        }

        return -1;
    };

    self.getReport = function (params) {
        AccReport.disableInput();
        getParams(params);
        $.ajax({
            type: "POST",
            url: window.rootUrl + "WinLoss/TreeWinLoss/GetTreeWinLossByLevel",
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            data: JSON.stringify(self.Params.reportParams),
            beforeSend: function () {
                AccReport.disableInput();
            },
            success: function (rs) {
                self.hideReport();
                renderTreeWinLossByComm(rs);
            },
            error: function (event, jqXHR, ajaxSettings) {
                self.hideReport();
                if (jqXHR.status &&
                    jqXHR.status == AccReportData.Constants.HtmlErrorCode.SystemError) {
                    var r = jQuery.parseJSON(jqXHR.responseText);
                    AccReport.popUp.show(r.message);
                    renderTreeWinLossByComm(null);
                }
            },
            complete: function () {
                AccReport.enableInput();
            }
        });
    };

    var getParams = function (params) {
        self.Params.reportParams = params;
    };

    var renderTreeWinLossByComm = function (report) {
       // report = report.TreeWinLoss;
        _tables.reportContainer.removeClass("display-none");
        _tables.report.addClass("display-none");
        var bodyData = [];
        var footerData = [];
        var isShowNA = report.IsShowNotAvailableTurnover;
        var totalItems = report.ChildsInfo.length;
        if (report && report.ChildsInfo.length > 0) {
            var len = report.ChildsInfo.length;
            var info = null;
            for (var i = 0; i < len; i++) {
                info = report.ChildsInfo[i];
                info.BaseRMTurnover = isShowNA ? "N/A" : info.BaseRMTurnover;
                info.BaseRMVirtualTurnover = isShowNA ? "N/A" : info.BaseRMVirtualTurnover;
                info.BaseRMMargin = isShowNA ? "N/A" : info.BaseRMMargin;
                bodyData.push({
                    Columns: [{ Name: "ColNo", Value: i + 1 },
                    {
                        Name: "ColUserName", Value: {
                            custId: info.Id,
                            Name: info.Name
                        },
                        CustomCellRender: function (currentCell) {
                            var clickEvent = ' onclick = "viewTreeWinLossBySuper(' + currentCell.Value.custId + ',\'' + currentCell.Value.Name + "');\"";
                            var html = '<div class="link " ' + clickEvent + '>' + currentCell.Value.Name + '</div>';
                            return html;
                        }
                    },
                        { Name: "ColTurnover", Value: info.Turnover },
                        { Name: "ColExtraPT", Value: info.ExtraAmount },
                        { Name: "ColWinLoss", Value: info.WinLoss },
                        { Name: "ColCommission", Value: info.Comm },
                        { Name: "ColWinLossTotal", Value: info.WinLossTotal },
                        { Name: "ColBaseRMTurnover", Value: info.BaseRMTurnover },
                        { Name: "ColBaseRMVirtualTurnover", Value: info.BaseRMVirtualTurnover },
                        { Name: "ColBaseRMTOVariancePlus", Value: info.BaseRMTOVariancePlus },
                        { Name: "ColBaseRMTOVarianceMinus", Value: info.BaseRMTOVarianceMinus },
                        { Name: "ColBaseRMTOVarianceAbsoluteTotal", Value: info.BaseRMTOVarianceAbsoluteTotal },
                        { Name: "ColBaseRMWLVariancePlus", Value: info.BaseRMWLVariancePlus },
                        { Name: "ColBaseRMWLVarianceMinus", Value: info.BaseRMWLVarianceMinus },
                        { Name: "ColBaseRMWLVarianceTotal", Value: info.BaseRMWinlostVarianceTotal },
                        { Name: "ColBaseRMVarianceMargin", Value: info.BaseRMVarianceMargin },
                        { Name: "ColBaseRMTotal", Value: info.BaseRMTotal },
                        { Name: "ColBaseRMVirtualTotal", Value: info.BaseRMVirtualTotal },
                        { Name: "ColBaseRMMargin", Value: info.BaseRMMargin },
                        { Name: "ColVirtualBaseRMMargin", Value: info.VirtualBaseRMMargin },
                        {
                            Name: "ColVirtualRate", Value: {
                                custId: info.Id,
                                virtualRate: info.VirtualRate,
                                userName: info.Name
                            },
                            CustomCellRender: function (currentCell) {
                                var clickEvent = ' onclick = "viewVirtualRateHistory(' + currentCell.Value.custId + ',\'' + currentCell.Value.userName + "');\"";
                                var html = '<div class="link " ' + clickEvent + '>View</div>';
                                return html;
                            }
                        }
                    ]
                });
            }

            for (var j = 0; j < report.TotalInfo.length; j++) {
                info = report.TotalInfo[j];
                info.BaseRMTurnover = isShowNA ? "N/A" : info.BaseRMTurnover;
                info.BaseRMVirtualTurnover = isShowNA ? "N/A" : info.BaseRMVirtualTurnover;
                info.BaseRMMargin = isShowNA ? "N/A" : info.BaseRMMargin;
                footerData.push({
                    Columns: [{ Name: "ColNo", Value: info.Name, MergeColNumber: 7, CellAlign: "right" },
                        { Name: "ColBaseRMTurnover", Value: info.BaseRMTurnover },
                        { Name: "ColBaseRMVirtualTurnover", Value: info.BaseRMVirtualTurnover },
                        { Name: "ColBaseRMTOVariancePlus", Value: info.BaseRMTOVariancePlus },
                        { Name: "ColBaseRMTOVarianceMinus", Value: info.BaseRMTOVarianceMinus },
                        { Name: "ColBaseRMTOVarianceAbsoluteTotal", Value: info.BaseRMTOVarianceAbsoluteTotal },
                        { Name: "ColBaseRMWLVariancePlus", Value: info.BaseRMWLVariancePlus },
                        { Name: "ColBaseRMWLVarianceMinus", Value: info.BaseRMWLVarianceMinus },
                        { Name: "ColBaseRMWLVarianceTotal", Value: info.BaseRMWinlostVarianceTotal },
                        { Name: "ColBaseRMVarianceMargin", Value: info.BaseRMVarianceMargin },
                        { Name: "ColBaseRMTotal", Value: info.BaseRMTotal },
                        { Name: "ColBaseRMVirtualTotal", Value: info.BaseRMVirtualTotal },
                        { Name: "ColBaseRMMargin", Value: info.BaseRMMargin },
                        { Name: "ColVirtualBaseRMMargin", Value: info.VirtualBaseRMMargin }
                    ]
                });
            }

        }

        var gridOptions = {
            customizeHeader: true,
            groupHeaderRows: [
            {
                Columns: [{ HeaderText: "No", Name: "ColNo", MergeRowNumber: 3 },
                { HeaderText: "Username", Name: "ColUserName", MergeRowNumber: 3 },
                { HeaderText: "Real Turnover", Name: "ColTurnover", MergeRowNumber: 3 },
                { HeaderText: "ExtraPT", Name: "ColExtraPT", MergeRowNumber: 3 },
                { HeaderText: "Real WinLoss", Name: "ColWinLoss", MergeRowNumber: 3 },
                { HeaderText: "Comm", Name: "ColCommission", MergeRowNumber: 3 },
                { HeaderText: "Real Income", Name: "ColWinLossTotal", MergeRowNumber: 3 },
                { HeaderText: "BaseRM", Name: "ColBaseRMTurnover", MergeColNumber: 13 },
                { HeaderText: "Virtual Rate(%)", Name: "ColVirtualRate", MergeRowNumber: 3 }]
            },
            {
                Columns: [{ HeaderText: "Real Turnover", Name: "ColBaseRMTurnover", MergeRowNumber: 2 },
                { HeaderText: "Virtual Turnover", Name: "ColBaseRMVirtualTurnover", MergeRowNumber: 2 },
                { HeaderText: "Turnover Variance", Name: "ColBaseRMTOVariancePlus", MergeColNumber: 3 },
                { HeaderText: "WinLoss Variance", Name: "ColBaseRMWLVariancePlus", MergeColNumber: 3 },
                { HeaderText: "Variance Margin(%)", Name: "ColBaseRMVarianceMargin", MergeRowNumber: 2 },
                { HeaderText: "Real Income", Name: "ColBaseRMTotal", MergeRowNumber: 2 },
                { HeaderText: "Virtual WinLoss", Name: "ColBaseRMVirtualTotal", MergeRowNumber: 2 },
                { HeaderText: "Real Margin(%)", Name: "ColBaseRMMargin", MergeRowNumber: 2 },
                { HeaderText: "Virtual Margin(%)", Name: "ColVirtualBaseRMMargin", MergeRowNumber: 2 }]
            }],
            columns: [{ HeaderText: "No", Width: 50, Name: "ColNo", HeaderAlign: "center", CellAlign: "center" },
                { HeaderText: "Username", Width: 120, Name: "ColUserName", HeaderAlign: "center", CellAlign: "center" },
                { HeaderText: "Real Turnover", Width: 120, Name: "ColTurnover", HeaderAlign: "center", CellAlign: "right", FormatNumber: FormatConstant.TWO_DECIMAL_NUMBER },
                { HeaderText: "ExtraPT", Width: 120, Name: "ColExtraPT", HeaderAlign: "center", CellAlign: "right", FormatNumber: FormatConstant.TWO_DECIMAL_NUMBER },
                { HeaderText: "Real WinLoss", Width: 120, Name: "ColWinLoss", HeaderAlign: "center", CellAlign: "right", FormatNumber: FormatConstant.TWO_DECIMAL_NUMBER, FormatPositive: '$' },
                { HeaderText: "Comm", Width: 120, Name: "ColCommission", HeaderAlign: "center", CellAlign: "right", FormatNumber: FormatConstant.TWO_DECIMAL_NUMBER, FormatPositive: '$' },
                { HeaderText: "Total", Width: 120, Name: "ColWinLossTotal", HeaderAlign: "center", CellAlign: "right", FormatNumber: FormatConstant.TWO_DECIMAL_NUMBER, FormatPositive: '$' },
                { HeaderText: "Real Turnover", Width: 120, Name: "ColBaseRMTurnover", HeaderAlign: "center", CellAlign: "right", FormatNumber: FormatConstant.TWO_DECIMAL_NUMBER, Sort: true },
                { HeaderText: "Virtual Turnover", Width: 120, Name: "ColBaseRMVirtualTurnover", HeaderAlign: "center", CellAlign: "right", FormatNumber: FormatConstant.TWO_DECIMAL_NUMBER, Sort: true },
                { HeaderText: "+", Width: 120, Name: "ColBaseRMTOVariancePlus", HeaderAlign: "center", CellAlign: "right", FormatNumber: FormatConstant.TWO_DECIMAL_NUMBER, Sort: true },
                { HeaderText: "-", Width: 120, Name: "ColBaseRMTOVarianceMinus", HeaderAlign: "center", CellAlign: "right", FormatNumber: FormatConstant.TWO_DECIMAL_NUMBER, Sort: true },
                { HeaderText: "Total", Width: 120, Name: "ColBaseRMTOVarianceAbsoluteTotal", HeaderAlign: "center", CellAlign: "right", FormatNumber: FormatConstant.TWO_DECIMAL_NUMBER, Sort: true },
                { HeaderText: "+", Width: 120, Name: "ColBaseRMWLVariancePlus", HeaderAlign: "center", CellAlign: "right", Sort: true, FormatNumber: FormatConstant.TWO_DECIMAL_NUMBER, FormatPositive: '$' },
                { HeaderText: "-", Width: 120, Name: "ColBaseRMWLVarianceMinus", HeaderAlign: "center", CellAlign: "right", Sort: true, FormatNumber: FormatConstant.TWO_DECIMAL_NUMBER, FormatPositive: '$' },
                { HeaderText: "Total", Width: 120, Name: "ColBaseRMWLVarianceTotal", HeaderAlign: "center", CellAlign: "right", Sort: true, FormatNumber: FormatConstant.TWO_DECIMAL_NUMBER, FormatPositive: '$' },
                { HeaderText: "Variance Margin(%)", Width: 150, Name: "ColBaseRMVarianceMargin", HeaderAlign: "center", CellAlign: "right", Sort: true, FormatNumber: FormatConstant.TWO_DECIMAL_NUMBER, FormatPositive: '$', CustomSortFunction: self.customSortNumber },
                { HeaderText: "Real Income", Width: 120, Name: "ColBaseRMTotal", HeaderAlign: "center", CellAlign: "right", Sort: true, FormatPositive: '$', FormatNumber: FormatConstant.TWO_DECIMAL_NUMBER },
                { HeaderText: "Virtual WinLoss", Width: 120, Name: "ColBaseRMVirtualTotal", HeaderAlign: "center", CellAlign: "right", Sort: true, FormatPositive: '$', FormatNumber: FormatConstant.TWO_DECIMAL_NUMBER },
                { HeaderText: "Real Margin(%)", Width: 120, Name: "ColBaseRMMargin", HeaderAlign: "center", CellAlign: "right", Sort: true, FormatPositive: '$', FormatNumber: FormatConstant.TWO_DECIMAL_NUMBER, CustomSortFunction: self.customSortNumber },
                { HeaderText: "Virtual Margin(%)", Width: 150, Name: "ColVirtualBaseRMMargin", HeaderAlign: "center", CellAlign: "right", Sort: true, FormatPositive: '$', FormatNumber: FormatConstant.TWO_DECIMAL_NUMBER, CustomSortFunction: self.customSortNumber },
                { HeaderText: "Virtual Rate(%)", Width: 80, Name: "ColVirtualRate", HeaderAlign: "center", CellAlign: "right" }],
            showTitle: true,
            showFooter: true,
            titleInfo:
            {
                text: "<a class='link' onclick='BackToTreeWinLossByProduct();'>Currency</a> > " +
                " Super Win Loss (" + self.Params.reportParams.Name + " currency)",
                showExcelButton: true,
                exportExcelFunction: function () {
                    self.exportReport();
                }
            },
            showTree: false,
            bodyRows: bodyData,
            footerRows: footerData,
            gridExpandHeight: 185,
            numberOfFrozenColumn: 6,
            showDistinctRow: true,
            showPager: true,
            pagerOption: {
                itemsPerPage: reportManager.CachedData.itemsPerPage,
                currentPage: reportManager.CachedData.currentPage,
                itemsPerPageArray: reportManager.CachedData.itemsPerPageArray,
                totalItem: totalItems
            }
        };
        _tables.report.removeClass("display-none").fanexGrid(gridOptions);
    };

    self.exportReport = function () {
        AccReport.downloadFile(window.rootUrl + "WinLoss/TreeWinLoss/ExportTreeWinLossByLevel", self.Params.reportParams);
    };


}