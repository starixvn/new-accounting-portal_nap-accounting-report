Namespace.Register("Fanex.AccountingReports.Main.MatchWinLoss2a.SummaryReportByBetType");
Fanex.AccountingReports.Main.MatchWinLoss2a.SummaryReportByBetType = function () {
    var self = this;

    var _tables = {
        summaryReportByBetTypeContainer: null,
        summaryReportByBetType: null
    };
    var _params = {
        reportParams: {
            Winlostdate: null,
            IsGetMainReport: null,
            IsGetDetailReport: null
        }
    };

    var _cacheData = {
        DisplayLinkBetType: [9]
    };

    self.initPage = function () {
        cachedDOMElements();
    };

    var cachedDOMElements = function () {
        _tables.summaryReportByBetTypeContainer = $('#summaryReportByBetTypeContainer');
        _tables.summaryReportByBetType = $('#summaryReportByBetType');
    };

    self.getSummaryReportByBetType = function (reportParams) {
        AccReport.disableInput();
        self.setParams(reportParams);

        AccReport.utility.CallAjax(
            this,
            window.rootUrl + "Main/MatchWinLoss2a/GetSummaryReportByBetType",
            _params.reportParams,
            function (rs) {
                self.renderReport(rs, _params.reportParams);
            },
            function (jqXHR, event, ajaxSettings) {
            },
            AccReport.enableInput);
    };

    self.getParams = function () {
        return _params.reportParams;
    };

    self.setParams = function (reportParams) {
        _params.reportParams.Winlostdate = reportParams.reportDate;
        _params.reportParams.IsGetMainReport = reportParams.isGetMainReport;
        _params.reportParams.IsGetDetailReport = reportParams.isGetDetailReport;
    };

    self.renderReport = function (report, params) {
        _tables.summaryReportByBetTypeContainer.removeClass("display-none");
        _tables.summaryReportByBetType.addClass("display-none");
        _params.reportParams.Winlostdate = params.Winlostdate;
        var isShowMainAndDetailReport = IsShowMainAndDetailReport(params.IsGetMainReport, params.IsGetDetailReport);
        var bodyData = [];
        var footerData = [];

        if (report && report.ChildsInfo.length > 0) {
            var len = report.ChildsInfo.length;
            var currentRow = null;
            var info = null;
            for (var i = 0; i < len; i++) {
                info = report.ChildsInfo[i];
                currentRow =
                {
                    Columns: [{ Name: "ColBetType", Value: info,
                        CustomCellRender: function (currentCell) {
                            if ($.inArray(currentCell.Value.Id, _cacheData.DisplayLinkBetType) == -1) {
                                return currentCell.Value.Name + (currentCell.Value.Name_Suffix != '' ?
                                                        '<br />' + currentCell.Value.Name_Suffix : '');
                            }

                            var clickEvent = ' onclick = "ViewMixparlayDetail(\'' + _params.reportParams.Winlostdate
                                                                                + '\',' + currentCell.Value.Id
                                                                                + ',\'' + currentCell.Value.Name
                                                                                + "');\"";
                            return '<div class="link" ' + clickEvent + '>' + currentCell.Value.Name + (currentCell.Value.Name_Suffix != '' ?
                                                        '<br />' + currentCell.Value.Name_Suffix : '') + '</div>';
                        }
                    },
                              { Name: "ColLineTotal_Soccer_Live", Value: info.LineTotal_Soccer_Live_Str },
                              { Name: "ColVirtualLineTotal_Soccer_Live", Value: info.VirtualLineTotal_Soccer_Live_Str },
                              { Name: "ColLineTotal_Soccer_Non_Live", Value: info.LineTotal_Soccer_Non_Live_Str },
                              { Name: "ColVirtualLineTotal_Soccer_Non_Live", Value: info.VirtualLineTotal_Soccer_Non_Live_Str },
                              { Name: "ColLineTotal_Others_Live", Value: info.LineTotal_Others_Live_Str },
                              { Name: "ColVirtualLineTotal_Others_Live", Value: info.VirtualLineTotal_Others_Live_Str },
                              { Name: "ColLineTotal_Others_Non_Live", Value: info.LineTotal_Others_Non_Live_Str },
                              { Name: "ColVirtualLineTotal_Others_Non_Live", Value: info.VirtualLineTotal_Others_Non_Live_Str },
                              { Name: "ColLineTotalReal", Value: info.LineTotalReal_Str },
                              { Name: "ColLineTotalVirtual", Value: info.LineTotalVirtual_Str },
                              { Name: "ColVariance", Value: info.Variance_Str },
                              { Name: "ColTurnover", Value: info.Turnover_Str },
                              { Name: "ColMargin", Value: info.Margin_Str}],
                    SubRows: []
                };

                bodyData.push(currentRow);
            }

            info = report.GrandTotal;
            currentRow =
            {
                Columns: [{ Name: "ColBetType", Value: info.Name },
                          { Name: "ColLineTotal_Soccer_Live", Value: info.LineTotal_Soccer_Live_Str },
                          { Name: "ColVirtualLineTotal_Soccer_Live", Value: info.VirtualLineTotal_Soccer_Live_Str },
                          { Name: "ColLineTotal_Soccer_Non_Live", Value: info.LineTotal_Soccer_Non_Live_Str },
                          { Name: "ColVirtualLineTotal_Soccer_Non_Live", Value: info.VirtualLineTotal_Soccer_Non_Live_Str },
                          { Name: "ColLineTotal_Others_Live", Value: info.LineTotal_Others_Live_Str },
                          { Name: "ColVirtualLineTotal_Others_Live", Value: info.VirtualLineTotal_Others_Live_Str },
                          { Name: "ColLineTotal_Others_Non_Live", Value: info.LineTotal_Others_Non_Live_Str },
                          { Name: "ColVirtualLineTotal_Others_Non_Live", Value: info.VirtualLineTotal_Others_Non_Live_Str },
                          { Name: "ColLineTotalReal", Value: info.LineTotalReal_Str },
                          { Name: "ColLineTotalVirtual", Value: info.LineTotalVirtual_Str },
                          { Name: "ColVariance", Value: info.Variance_Str },
                          { Name: "ColTurnover", Value: info.Turnover_Str },
                          { Name: "ColMargin", Value: info.Margin_Str}]
            };

            footerData.push(currentRow);
        }

        var gridOptions = {
            customizeHeader: true,
            groupHeaderRows: [{
                Columns: [{ HeaderText: "Bet Type", Name: "ColBetType", MergeRowNumber: 2 },
                          { HeaderText: "Soccer Live", Name: "ColLineTotal_Soccer_Live", MergeColNumber: 2 },
                          { HeaderText: "Soccer Non-Live", Name: "ColLineTotal_Soccer_Non_Live", MergeColNumber: 2 },
                          { HeaderText: "Other Live", Name: "ColLineTotal_Others_Live", MergeColNumber: 2 },
                          { HeaderText: "Other Non-Live", Name: "ColLineTotal_Others_Non_Live", MergeColNumber: 2 },
                          { HeaderText: "Line Total", Name: "ColLineTotalReal", MergeColNumber: 2 },
                          { HeaderText: "Variance", Name: "ColVariance", MergeRowNumber: 2 },
                          { HeaderText: "Turnover", Name: "ColTurnover", MergeRowNumber: 2 },
                          { HeaderText: "Margin%", Name: "ColMargin", MergeRowNumber: 2}]
            }],
            columns: [{ HeaderText: "Bet Type", Width: 150, Name: "ColBetType", HeaderAlign: "center", CellAlign: "center" },
                      { HeaderText: "Real", Width: 100, Name: "ColLineTotal_Soccer_Live", HeaderAlign: "center", CellAlign: "right" },
                      { HeaderText: "Virtual", Width: 100, Name: "ColVirtualLineTotal_Soccer_Live", HeaderAlign: "center", CellAlign: "right" },
                      { HeaderText: "Real", Width: 100, Name: "ColLineTotal_Soccer_Non_Live", HeaderAlign: "center", CellAlign: "right" },
                      { HeaderText: "Virtual", Width: 100, Name: "ColVirtualLineTotal_Soccer_Non_Live", HeaderAlign: "center", CellAlign: "right" },
                      { HeaderText: "Real", Width: 100, Name: "ColLineTotal_Others_Live", HeaderAlign: "center", CellAlign: "right" },
                      { HeaderText: "Virtual", Width: 100, Name: "ColVirtualLineTotal_Others_Live", HeaderAlign: "center", CellAlign: "right" },
                      { HeaderText: "Real", Width: 100, Name: "ColLineTotal_Others_Non_Live", HeaderAlign: "center", CellAlign: "right" },
                      { HeaderText: "Virtual", Width: 100, Name: "ColVirtualLineTotal_Others_Non_Live", HeaderAlign: "center", CellAlign: "right" },
                      { HeaderText: "Real", Width: 100, Name: "ColLineTotalReal", HeaderAlign: "center", CellAlign: "right" },
                      { HeaderText: "Virtual", Width: 100, Name: "ColLineTotalVirtual", HeaderAlign: "center", CellAlign: "right" },
                      { HeaderText: "Variance", Width: 100, Name: "ColVariance", HeaderAlign: "center", CellAlign: "right" },
                      { HeaderText: "Turnover", Width: 120, Name: "ColTurnover", HeaderAlign: "center", CellAlign: "right" },
                      { HeaderText: "Margin%", Width: 80, Name: "ColMargin", HeaderAlign: "center", CellAlign: "right"}],
            showTitle: true,
            showFooter: true,
            titleInfo:
            {
                text: '',
                showExcelButton: true,
                exportExcelFunction: function () {
                    exportSummaryReportByBetType();
                }
            },
            showTree: false,
            bodyRows: bodyData,
            footerRows: footerData,
            gridExpandHeight: isShowMainAndDetailReport ? 0.63 * $(window).height() : 200,
            gridWidth: 900,
            numberOfFrozenColumn: 1,
            showDistinctRow: true
        };

        _tables.summaryReportByBetType.removeClass("display-none").fanexGrid(gridOptions);
    };

    exportSummaryReportByBetType = function () {
        AccReport.downloadFile(window.rootUrl + "Main/MatchWinLoss2a/ExportSummaryReportByBetType", _params.reportParams);
    };
}