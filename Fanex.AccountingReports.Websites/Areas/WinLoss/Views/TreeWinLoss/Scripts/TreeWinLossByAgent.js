/* File Created: November 22, 2013 */
Namespace.Register("Fanex.AccountingReports.WinLoss.TreeWinLoss.TreeWinLossByAgent");
Fanex.AccountingReports.WinLoss.TreeWinLoss.TreeWinLossByAgent = function () {
    var self = this;
    var _tables = {
        reportContainer: null
    };

    self.Params = {
        reportParams: {
            FromDate: new Date(),
            ToDate: new Date(),
            ProductSelection: null,
            Products: null,
            RoleId: 0
        }
    };

    self.initPage = function () {
        cachedDOMElements();
    };

    var cachedDOMElements = function () {
        _tables.reportContainer = $("#treeWinLossByAgentContainer");
        _tables.report = $("#treeWinLossByAgent");
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
                renderTreeWinLossByMaster(rs);
            },
            error: function (event, jqXHR, ajaxSettings) {
                self.hideReport();
                if (jqXHR.status &&
                    jqXHR.status == AccReportData.Constants.HtmlErrorCode.SystemError) {
                    var r = jQuery.parseJSON(jqXHR.responseText);
                    AccReport.popUp.show(r.message);
                    renderTreeWinLossByMaster(null);
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

    var renderTreeWinLossByMaster = function (report) {
        report = report.TreeWinLoss;
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
                        { Name: "ColUserName", Value: info.Name },
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
                        { Name: "ColVirtualRate", Value: {
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
                        { Name: "ColBaseRMMargin", Value: info.BaseRMMargin }
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
                { HeaderText: "Turnover", Name: "ColTurnover", MergeRowNumber: 3 },
                { HeaderText: "ExtraPT", Name: "ColExtraPT", MergeRowNumber: 3 },
                { HeaderText: "WinLoss", Name: "ColWinLoss", MergeRowNumber: 3 },
                { HeaderText: "Comm", Name: "ColCommission", MergeRowNumber: 3 },
                { HeaderText: "Total", Name: "ColWinLossTotal", MergeRowNumber: 3 },
                { HeaderText: "BaseRM", Name: "ColBaseRMTurnover", MergeColNumber: 12 },
                { HeaderText: "Virtual Rate(%)", Name: "ColVirtualRate", MergeRowNumber: 3}]
            },
            {
                Columns: [{ HeaderText: "Turnover", Name: "ColBaseRMTurnover", MergeRowNumber: 2 },
                { HeaderText: "Virtual Turnover", Name: "ColBaseRMVirtualTurnover", MergeRowNumber: 2 },
                { HeaderText: "Turnover Variance", Name: "ColBaseRMTOVariancePlus", MergeColNumber: 3 },
                { HeaderText: "WinLoss Variance", Name: "ColBaseRMWLVariancePlus", MergeColNumber: 3 },
                { HeaderText: "Variance Margin(%)", Name: "ColBaseRMVarianceMargin", MergeRowNumber: 2 },
                { HeaderText: "Total", Name: "ColBaseRMTotal", MergeRowNumber: 2 },
                { HeaderText: "Virtual Total", Name: "ColBaseRMVirtualTotal", MergeRowNumber: 2 },
                { HeaderText: "Margin(%)", Name: "ColBaseRMMargin", MergeRowNumber: 2}]
            }],
            columns: [{ HeaderText: "No", Width: 50, Name: "ColNo", HeaderAlign: "center", CellAlign: "center" },
                { HeaderText: "Username", Width: 120, Name: "ColUserName", HeaderAlign: "center", CellAlign: "center" },
                { HeaderText: "Turnover", Width: 120, Name: "ColTurnover", HeaderAlign: "center", CellAlign: "right", FormatNumber: FormatConstant.TWO_DECIMAL_NUMBER },
                { HeaderText: "ExtraPT", Width: 120, Name: "ColExtraPT", HeaderAlign: "center", CellAlign: "right", FormatNumber: FormatConstant.TWO_DECIMAL_NUMBER },
                { HeaderText: "WinLoss", Width: 120, Name: "ColWinLoss", HeaderAlign: "center", CellAlign: "right", FormatPositive: '$', FormatNumber: FormatConstant.TWO_DECIMAL_NUMBER },
                { HeaderText: "Comm", Width: 120, Name: "ColCommission", HeaderAlign: "center", CellAlign: "right", FormatPositive: '$', FormatNumber: FormatConstant.TWO_DECIMAL_NUMBER },
                { HeaderText: "Total", Width: 120, Name: "ColWinLossTotal", HeaderAlign: "center", CellAlign: "right", FormatPositive: '$', FormatNumber: FormatConstant.TWO_DECIMAL_NUMBER },
                { HeaderText: "Turnover", Width: 120, Name: "ColBaseRMTurnover", HeaderAlign: "center", CellAlign: "right", Sort: true, FormatNumber: FormatConstant.TWO_DECIMAL_NUMBER },
                { HeaderText: "Virtual Turnover", Width: 120, Name: "ColBaseRMVirtualTurnover", HeaderAlign: "center", CellAlign: "right", Sort: true, FormatNumber: FormatConstant.TWO_DECIMAL_NUMBER },
                { HeaderText: "+", Width: 120, Name: "ColBaseRMTOVariancePlus", HeaderAlign: "center", CellAlign: "right", Sort: true, FormatNumber: FormatConstant.TWO_DECIMAL_NUMBER },
                { HeaderText: "-", Width: 120, Name: "ColBaseRMTOVarianceMinus", HeaderAlign: "center", CellAlign: "right", Sort: true, FormatNumber: FormatConstant.TWO_DECIMAL_NUMBER },
                { HeaderText: "Total", Width: 120, Name: "ColBaseRMTOVarianceAbsoluteTotal", HeaderAlign: "center", CellAlign: "right", Sort: true, FormatNumber: FormatConstant.TWO_DECIMAL_NUMBER },
                { HeaderText: "+", Width: 120, Name: "ColBaseRMWLVariancePlus", HeaderAlign: "center", CellAlign: "right", Sort: true, FormatPositive: '$', FormatNumber: FormatConstant.TWO_DECIMAL_NUMBER },
                { HeaderText: "-", Width: 120, Name: "ColBaseRMWLVarianceMinus", HeaderAlign: "center", CellAlign: "right", Sort: true, FormatPositive: '$', FormatNumber: FormatConstant.TWO_DECIMAL_NUMBER },
                { HeaderText: "Total", Width: 120, Name: "ColBaseRMWLVarianceTotal", HeaderAlign: "center", CellAlign: "right", Sort: true, FormatPositive: '$', FormatNumber: FormatConstant.TWO_DECIMAL_NUMBER },
                { HeaderText: "Variance Margin(%)", Width: 120, Name: "ColBaseRMVarianceMargin", HeaderAlign: "center", CellAlign: "right", Sort: true, FormatPositive: '$', FormatNumber: FormatConstant.TWO_DECIMAL_NUMBER, CustomSortFunction: self.customSortNumber },
                { HeaderText: "Total", Width: 120, Name: "ColBaseRMTotal", HeaderAlign: "center", CellAlign: "right", Sort: true, FormatPositive: '$', FormatNumber: FormatConstant.TWO_DECIMAL_NUMBER },
                { HeaderText: "Virtual Total", Width: 120, Name: "ColBaseRMVirtualTotal", HeaderAlign: "center", CellAlign: "right", Sort: true, FormatPositive: '$', FormatNumber: FormatConstant.TWO_DECIMAL_NUMBER },
                { HeaderText: "Margin(%)", Width: 120, Name: "ColBaseRMMargin", HeaderAlign: "center", CellAlign: "right", Sort: true, FormatPositive: '$', FormatNumber: FormatConstant.TWO_DECIMAL_NUMBER, CustomSortFunction: self.customSortNumber },
                { HeaderText: "Virtual Rate(%)", Width: 80, Name: "ColVirtualRate", HeaderAlign: "center", CellAlign: "right", FormatNumber: FormatConstant.TWO_DECIMAL_NUMBER}],
            showTitle: true,
            showFooter: true,
            titleInfo:
            {
                text: "<a class='link' onclick='BackToTreeWinLossByProduct();'>Currency</a> > " +
                "<a class='link' onclick='BackToTreeWinLossByComm();'>Super</a> > " +
                "<a class='link' onclick='BackToTreeWinLossBySuper();'>Master</a> > " +
                "<a class='link' onclick='BackToTreeWinLossByMaster();'>Agent</a> > " +
                " Member Win Loss (Agent " + self.Params.reportParams.Name + ")",
                showExcelButton: true,
                exportExcelFunction: function () {
                    self.exportExcel();
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

    self.exportExcel = function () {
        AccReport.downloadFile(window.rootUrl + "WinLoss/TreeWinLoss/ExportTreeWinLossByLevel", self.Params.reportParams);
    };
};