Namespace.Register("Fanex.AccountingReports.Main.MatchWinLoss2a.SummaryReportBySportType");
Fanex.AccountingReports.Main.MatchWinLoss2a.SummaryReportBySportType = function () {
    var self = this;
    var _tables = {
        summaryReportBySportTypeContainer: null,
        summaryReportBySportType: null
    };

    var _params = {
        reportParams: {
            Winlostdate: null,
            IsGetMainReport: null,
            IsGetDetailReport: null
        }
    };

    var _cacheData = {
        DisplayLinkBetType: ["mbc - rng casino", "metric gaming - super live", "cockfighting"]
    };

    self.initPage = function () {
        cachedDOMElements();
    };

    var cachedDOMElements = function () {
        _tables.summaryReportBySportTypeContainer = $('#summaryReportBySportTypeContainer');
        _tables.summaryReportBySportType = $('#summaryReportBySportType');
    };

    self.getSummaryReportBySportType = function (reportParams) {
        AccReport.disableInput();
        self.getParams(reportParams);

        AccReport.utility.CallAjax(
            this,
            window.rootUrl + "Main/MatchWinLoss2a/GetSummaryReportBySportType",
            _params.reportParams,
            function (rs) {
                self.renderReport(rs, _params.reportParams);
            },
            function (jqXHR, event, ajaxSettings) {
            },
            AccReport.enableInput);
    };

    self.getParams = function (reportParams) {
        _params.reportParams.Winlostdate = reportParams.reportDate;
        _params.reportParams.IsGetMainReport = reportParams.isGetMainReport;
        _params.reportParams.IsGetDetailReport = reportParams.isGetDetailReport;
    };

    self.renderReport = function (report, params) {
        _tables.summaryReportBySportTypeContainer.removeClass("display-none");
        _tables.summaryReportBySportType.addClass("display-none");
        _params.reportParams.Winlostdate = params.Winlostdate;
        var isShowMainAndDetailReport = IsShowMainAndDetailReport(params.IsGetMainReport, params.IsGetDetailReport);
        var bodyData = [];
        var footerData = [];

        if (report &&
            (report.NonThirdPartyInfo.ChildsInfo.length > 0 ||
            report.ThirdPartyInfo.ChildsInfo.length > 0)) {
            var nonThirdPartyWinLoss = report.NonThirdPartyInfo;
            var nonThirdPartyInfoLength = nonThirdPartyWinLoss.ChildsInfo.length;
            var currentRow = null;
            var info = null;
            for (var i = 0; i < nonThirdPartyInfoLength; i++) {
                var sportDetail = nonThirdPartyWinLoss.ChildsInfo[i];
                var sportDetailLength = sportDetail.ChildsInfo.length;
                for (var j = 0; j < sportDetailLength; j++) {
                    info = sportDetail.ChildsInfo[j];
                    currentRow =
                    {
                        Columns: [{ Name: "ColBetType",
                            Value: {
                                Id: info.Id,
                                Name: info.Name,
                                LiveIndicatorId: info.LiveIndicatorId,
                                SportId: sportDetail.Id,
                                SportName: sportDetail.Name
                            },
                            CustomCellRender: function (currentCell) {
                                var betTypeName = getBetTypeName(currentCell.Value.LiveIndicatorId, currentCell.Value.Name);
                                var clickEvent = ' onclick = "ViewDetailReportByBetType(\'' + _params.reportParams.Winlostdate
                                                                                                                    + '\',' + currentCell.Value.Id
                                                                                                                    + ',\'' + currentCell.Value.Name
                                                                                                                    + '\',' + currentCell.Value.SportId
                                                                                                                    + ',\'' + currentCell.Value.SportName
                                                                                                                    + '\',' + currentCell.Value.LiveIndicatorId
                                                                                                                    + ");\"";
                                return '<div class="link colBetType" ' + clickEvent + '>' + betTypeName + '</div>';
                            }
                        },
                                    { Name: "ColComm", Value: info.Commission_Str },
                                    { Name: "ColVirtualComm", Value: info.VirtualComm_Str },
                                    { Name: "ColLineTotalReal", Value: info.LineTotal_Str, CssClass: "bold" },
                                    { Name: "ColVirtualLineTotal", Value: info.VirtualLineTotal_Str, CssClass: "bold" },
                                    { Name: "ColVariance", Value: info.Variance_Str },
                                    { Name: "ColTurnover", Value: info.Turnover_Str },
                                    { Name: "ColMargin", Value: info.Margin_Str}]
                    };

                    bodyData.push(currentRow);
                }

                var info = sportDetail.GrandTotal;
                info.Name = sportDetail.Name;
                currentRow =
                {
                    Columns: [{ Name: "ColBetType", Value: info,
                        CustomCellRender: function (currentCell) {
                            var betTypeName = getBetTypeName(currentCell.Value.LiveIndicatorId, currentCell.Value.Name);
                            var clickEvent = ' onclick = "ViewDetailReportBySportType(\'' + _params.reportParams.Winlostdate
                                        + '\',' + currentCell.Value.Id
                                        + ',\'' + currentCell.Value.Name
                                        + "');\"";
                            return '<div class="link align-right" ' + clickEvent + '>' + betTypeName + '</div>';
                        },
                        CssClass: "bold"
                    },
                             { Name: "ColComm", Value: info.Commission_Str },
                             { Name: "ColVirtualComm", Value: info.VirtualComm_Str },
                             { Name: "ColLineTotalReal", Value: info.LineTotal_Str, CssClass: "bold" },
                             { Name: "ColVirtualLineTotal", Value: info.VirtualLineTotal_Str, CssClass: "bold" },
                             { Name: "ColVariance", Value: info.Variance_Str },
                             { Name: "ColTurnover", Value: info.Turnover_Str },
                             { Name: "ColMargin", Value: info.Margin_Str}],
                    CssClass: "subTotal"
                };

                bodyData.push(currentRow);
            }

            info = nonThirdPartyWinLoss.GrandTotal;
            currentRow =
            {
                Columns: [{ Name: "ColBetType", Value: info.Name, CssClass: "align-right" },
                          { Name: "ColComm", Value: info.Commission_Str },
                          { Name: "ColVirtualComm", Value: info.VirtualComm_Str },
                          { Name: "ColLineTotalReal", Value: info.LineTotal_Str, CssClass: "bold" },
                          { Name: "ColVirtualLineTotal", Value: info.VirtualLineTotal_Str, CssClass: "bold" },
                          { Name: "ColVariance", Value: info.Variance_Str },
                          { Name: "ColTurnover", Value: info.Turnover_Str },
                          { Name: "ColMargin", Value: info.Margin_Str}],
                CssClass: "grandTotal"
            };

            bodyData.push(currentRow);

            info = report.EightRMTotal;
            currentRow =
            {
                Columns: [{ Name: "ColBetType", Value: info.Name, CssClass: "align-right", MergeColNumber: 3 },
                { Name: "ColLineTotalReal", Value: info.LineTotal_Str, CssClass: "bold" },
                { Name: "ColVirtualLineTotal", Value: info.VirtualLineTotal_Str, CssClass: "bold" },
                { Name: "ColVariance", Value: info.Variance_Str },
                { Name: "ColTurnover", Value: info.Turnover_Str },
                { Name: "ColMargin", Value: info.Margin_Str}],
                CssClass: "subTotal"
            };

            bodyData.push(currentRow);

            info = report.BARMTotal;
            currentRow =
            {
                Columns: [{ Name: "ColBetType", Value: info.Name, CssClass: "align-right", MergeColNumber: 3 },
                { Name: "ColLineTotalReal", Value: info.LineTotal_Str, CssClass: "bold" },
                { Name: "ColVirtualLineTotal", Value: info.VirtualLineTotal_Str, CssClass: "bold" },
                { Name: "ColVariance", Value: info.Variance_Str },
                { Name: "ColTurnover", Value: info.Turnover_Str },
                { Name: "ColMargin", Value: info.Margin_Str}],
                CssClass: "subTotal"
            };

            bodyData.push(currentRow);

            var _3rdPartyWinLoss = report.ThirdPartyInfo;
            var _3rdPartyInfoLength = _3rdPartyWinLoss.ChildsInfo.length;
            for (var i = 0; i < _3rdPartyInfoLength; i++) {
                info = _3rdPartyWinLoss.ChildsInfo[i];
                currentRow =
                {
                    Columns: [{ Name: "ColBetType", Value: info,
                        CustomCellRender: function (currentCell) {
                            if ($.inArray(currentCell.Value.Name.toLowerCase(), _cacheData.DisplayLinkBetType) == -1) {
                                return currentCell.Value.Name;
                            }

                            if ($.inArray(currentCell.Value.Name.toLowerCase(), _cacheData.DisplayLinkBetType) == 0) {
                                var clickEvent = ' onclick = "ViewThirdPartyDetailReport(\'' + _params.reportParams.Winlostdate
                                                                                                     + '\',\'' + currentCell.Value.Name
                                                                                                     + "');\"";
                                return '<div class="link" ' + clickEvent + '>' + currentCell.Value.Name + '</div>';
                            }

                            if ($.inArray(currentCell.Value.Name.toLowerCase(), _cacheData.DisplayLinkBetType) == 1) {
                                var clickEvent = ' onclick = "ViewMetricGamingReport(\'' + _params.reportParams.Winlostdate + "');\"";
                                return '<div class="link" ' + clickEvent + '>' + currentCell.Value.Name + '</div>';
                            }

                            if ($.inArray(currentCell.Value.Name.toLowerCase(), _cacheData.DisplayLinkBetType) == 2) {
                                var clickEvent = ' onclick = "ViewCockFightingReport(\'' + _params.reportParams.Winlostdate + "');\"";
                                return '<div class="link" ' + clickEvent + '>' + currentCell.Value.Name + '</div>';
                            }
                        },
                        CssClass: "align-center"
                    },
                              { Name: "ColComm", Value: info.Commission_Str },
                              { Name: "ColVirtualComm", Value: info.VirtualComm_Str },
                              { Name: "ColLineTotalReal", Value: info.LineTotal_Str, CssClass: "bold" },
                              { Name: "ColVirtualLineTotal", Value: info.VirtualLineTotal_Str, CssClass: "bold" },
                              { Name: "ColVariance", Value: info.Variance_Str },
                              { Name: "ColTurnover", Value: info.Turnover_Str },
                              { Name: "ColMargin", Value: info.Margin_Str}]
                };

                bodyData.push(currentRow);
            }

            info = report.GrandTotal;
            currentRow =
            {
                Columns: [{ Name: "ColBetType", Value: info.Name, CssClass: "align-right" },
                          { Name: "ColComm", Value: info.Commission_Str },
                          { Name: "ColVirtualComm", Value: info.VirtualComm_Str },
                          { Name: "ColLineTotalReal", Value: info.LineTotal_Str, CssClass: "bold" },
                          { Name: "ColVirtualLineTotal", Value: info.VirtualLineTotal_Str, CssClass: "bold" },
                          { Name: "ColVariance", Value: info.Variance_Str },
                          { Name: "ColTurnover", Value: info.Turnover_Str },
                          { Name: "ColMargin", Value: info.Margin_Str}]
            };

            footerData.push(currentRow);
        }

        var gridOptions = {
            columns: [{ HeaderText: "Bet Type", Width: 170, Name: "ColBetType", HeaderAlign: "center", CellAlign: "left" },
                      { HeaderText: "Comm", Width: 100, Name: "ColComm", HeaderAlign: "center", CellAlign: "right" },
                      { HeaderText: "Virtual Comm", Width: 100, Name: "ColVirtualComm", HeaderAlign: "center", CellAlign: "right" },
                      { HeaderText: "Line Total", Width: 120, Name: "ColLineTotalReal", HeaderAlign: "center", CellAlign: "right" },
                      { HeaderText: "Virtual Line Total", Width: 120, Name: "ColVirtualLineTotal", HeaderAlign: "center", CellAlign: "right" },
                      { HeaderText: "Variance", Width: 120, Name: "ColVariance", HeaderAlign: "center", CellAlign: "right" },
                      { HeaderText: "Turnover", Width: 120, Name: "ColTurnover", HeaderAlign: "center", CellAlign: "right" },
                      { HeaderText: "Margin%", Width: 120, Name: "ColMargin", HeaderAlign: "center", CellAlign: "right"}],
            showTitle: true,
            showFooter: true,
            titleInfo:
            {
                text: '',
                showExcelButton: true,
                exportExcelFunction: function () {
                    exportSummaryReportBySportType();
                }
            },
            showTree: false,
            bodyRows: bodyData,
            showDistinctRow: false,
            footerRows: footerData,
            gridExpandHeight: isShowMainAndDetailReport ? 0.68 * $(window).height() : 200,
            gridWidth: 800,
            numberOfFrozenColumn: 0
        };

        _tables.summaryReportBySportType.removeClass("display-none").fanexGrid(gridOptions);
    };

    var getBetTypeName = function (liveType, bettypeName) {
        var displayBetTypeName = '';
        if (liveType == true) {
            displayBetTypeName = '<span class="bold">!Live</span> - ';
        }

        displayBetTypeName += bettypeName;
        return displayBetTypeName;
    };

    exportSummaryReportBySportType = function () {
        AccReport.downloadFile(window.rootUrl + "Main/MatchWinLoss2a/ExportSummaryReportBySportType", _params.reportParams);
    };
};