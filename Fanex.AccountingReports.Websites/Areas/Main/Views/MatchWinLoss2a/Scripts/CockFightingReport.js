Namespace.Register("Fanex.AccountingReports.Main.MatchWinLoss2a.CockFightingReport");
Fanex.AccountingReports.Main.MatchWinLoss2a.CockFightingReport = function () {
    var self = this;
    var cockFightingGridOptions = null;
    var _tables = {
        reportContainer: null,
        report: null,
        reportByArenaContainer: null,
        reportByArenaContent: null
    };
    var _params = {
        getCockFightingParams: {
            WinLostDate: null
        },

        getCockFightingMatchByArenaParams: {
            WinLostDate: null,
            ArenaId: null,
            ArenaName: null
        }
    };

    self.initPage = function () {
        cachedDOMElements();
    };

    var cachedDOMElements = function () {
        _tables.reportParentContainer = $("#parentContainer");
        _tables.reportContainer = $('#cockFightingReportContainer');
        _tables.report = $('#cockFightingReportContent');
        _tables.reportByArenaContainer = $("#cockFightingMatchByArenaReportContainer");
        _tables.reportByArenaContent = $("#cockFightingMatchByArenaReportContent");
    };

    self.closeDialog = function () {
        try {
            _tables.reportContainer.dialog("close");
        }
        catch (e) {
        }
    };

    self.getCockFightingReport = function (params) {
        AccReport.disableInput();
        _params.getCockFightingParams.WinLostDate = params.Winlostdate;

        AccReport.utility.CallAjax(
            this,
            window.rootUrl + "Main/MatchWinLoss2a/GetCockFightingReport",
            _params.getCockFightingParams,
            function (rs) {
                renderCockFightingReport(rs);
            },
            function (jqXHR, event, ajaxSettings) {
            },
            AccReport.enableInput);
    };

    var renderCockFightingReport = function (rs) {
        _tables.reportContainer.removeClass("display-none");
        _tables.report.addClass("display-none");
        var bodyData = [];
        var footerData = [];

        if (rs && rs.ReportRows.length > 0) {
            var reportLength = rs.ReportRows.length;
            for (var i = 0; i < reportLength; i++) {
                var item = rs.ReportRows[i];
                currentRow =
                {
                    Columns: [{ Name: "ColArena", Value: item, CustomCellRender: function (currentCell) {
                        return "<div class='link centerAlign' onclick='ViewCockFightingDetailReport(\"" + _params.getCockFightingParams.WinLostDate + "\",\"" + currentCell.Value.ArenaId + "\",\"" + currentCell.Value.ArenaName + "\")'>" + currentCell.Value.ArenaName + "</div>";
                    }
                    },
                                { Name: "ColWinLoss", Value: item.WinLoss },
                                { Name: "ColPosition", Value: item.Position },
                                { Name: "ColCommission", Value: item.Commission },
                                { Name: "ColVirtualCommission", Value: item.VirtualCommission },
                                { Name: "ColLineTotal", Value: item.RealLineTotal },
                                { Name: "ColVirtualLineTotal", Value: item.VirtualLineTotal },
                                { Name: "ColVariance", Value: item.Variance },
                                { Name: "ColTurnover", Value: item.RealTurnover },
                                { Name: "ColMargin", Value: item.Margin}]
                };

                bodyData.push(currentRow);
            }

            var grandTotal = rs.GrandTotal;
            currentRow =
                {
                    Columns: [{ Name: "ColArena", Value: "Cockfighting Total" },
                                { Name: "ColWinLoss", Value: grandTotal.WinLoss },
                                { Name: "ColPosition", Value: grandTotal.Position },
                                { Name: "ColCommission", Value: grandTotal.Commission },
                                { Name: "ColVirtualCommission", Value: grandTotal.VirtualCommission },
                                { Name: "ColLineTotal", Value: grandTotal.RealLineTotal },
                                { Name: "ColVirtualLineTotal", Value: grandTotal.VirtualLineTotal },
                                { Name: "ColVariance", Value: grandTotal.Variance },
                                { Name: "ColTurnover", Value: grandTotal.RealTurnover },
                                { Name: "ColMargin", Value: grandTotal.Margin}]
                };

            footerData.push(currentRow);
        }

        var gridOptions = {
            columns: [{ HeaderText: "Arena", Width: 100, Name: "ColArena", HeaderAlign: "center", CellAlign: "center" },
                      { HeaderText: "Win Loss", Width: 80, Name: "ColWinLoss", HeaderAlign: "center", CellAlign: "center", FormatNumber: FormatConstant.TWO_DECIMAL_NUMBER },
                      { HeaderText: "Position", Width: 80, Name: "ColPosition", HeaderAlign: "center", CellAlign: "right", FormatNumber: FormatConstant.TWO_DECIMAL_NUMBER },
                      { HeaderText: "Comm", Width: 80, Name: "ColCommission", HeaderAlign: "center", CellAlign: "right", FormatNumber: FormatConstant.TWO_DECIMAL_NUMBER },
                      { HeaderText: "Virtual Comm", Width: 80, Name: "ColVirtualCommission", HeaderAlign: "center", CellAlign: "right", FormatNumber: FormatConstant.TWO_DECIMAL_NUMBER },
                      { HeaderText: "Line Total", Width: 80, Name: "ColLineTotal", HeaderAlign: "center", CellAlign: "right", FormatNumber: FormatConstant.TWO_DECIMAL_NUMBER },
                      { HeaderText: "Virtual Line Total", Width: 80, Name: "ColVirtualLineTotal", HeaderAlign: "center", CellAlign: "right", FormatNumber: FormatConstant.TWO_DECIMAL_NUMBER },
                      { HeaderText: "Variance", Width: 80, Name: "ColVariance", HeaderAlign: "center", CellAlign: "right", FormatNumber: FormatConstant.TWO_DECIMAL_NUMBER },
                      { HeaderText: "Turnover", Width: 80, Name: "ColTurnover", HeaderAlign: "center", CellAlign: "right", FormatNumber: FormatConstant.TWO_DECIMAL_NUMBER },
                      { HeaderText: "Margin%", Width: 80, Name: "ColMargin", HeaderAlign: "center", CellAlign: "right", FormatNumber: FormatConstant.TWO_DECIMAL_PERCENT_NUMBER}],
            showTitle: true,
            showFooter: true,
            titleInfo:
            {
                text: 'Cockfighting Product Win/Loss Report For ' + moment(new Date(_params.getCockFightingParams.WinLostDate)).format(FormatConstant.MMM_DD_YYYY_STRING_MOMENT),
                showExcelButton: true,
                exportExcelFunction: function () {
                    exportCockFightReport();
                }
            },
            showTree: false,
            bodyRows: bodyData,
            showDistinctRow: false,
            footerRows: footerData,
            gridExpandHeight: 450,
            numberOfFrozenColumn: 0,
            showHighlightRow: false
        };

        cockFightingGridOptions = gridOptions;

        _tables.report.removeClass("display-none").fanexGrid(gridOptions);
        _tables.reportContainer.dialog({
            title: "",
            autoOpen: false,
            width: 920,
            modal: false,
            resizable: false,
            close: function (event, ui) {
                _tables.reportContainer.dialog("destroy").remove();
                _tables.reportParentContainer.append('<div id="cockFightingReportContainer" class="display-none"> \
                                                        <div id="cockFightingReportContent" class="freegrid display-none"> \
                                                        </div> \
                                                      </div>');
                _tables.reportContainer = $('#cockFightingReportContainer');
                _tables.report = $('#cockFightingReportContent');
            }
        });

        _tables.reportContainer.dialog("open");
    }

    var exportCockFightReport = function () {
        AccReport.downloadFile(window.rootUrl + "Main/MatchWinLoss2a/ExportCockFightingReport", _params.getCockFightingParams);
    };

    self.getCockFightingMatchByArenaReport = function (params) {
        AccReport.disableInput();
        _params.getCockFightingMatchByArenaParams.WinLostDate = params.Winlostdate;
        _params.getCockFightingMatchByArenaParams.ArenaId = params.ArenaId;
        _params.getCockFightingMatchByArenaParams.ArenaName = params.ArenaName;

        AccReport.utility.CallAjax(
            this,
            window.rootUrl + "Main/MatchWinLoss2a/GetCockFightingMatchByArenaReport",
            _params.getCockFightingMatchByArenaParams,
            function (rs) {
                renderCockFightingMatchByArenaReport(rs);
            },
            function (jqXHR, event, ajaxSettings) {
            },
            AccReport.enableInput);
    };

    var renderCockFightingMatchByArenaReport = function (rs) {
        _tables.reportContainer.addClass("display-none");
        _tables.report.addClass("display-none");
        _tables.reportByArenaContainer.removeClass("display-none");
        _tables.reportByArenaContent.addClass("display-none");
        var bodyData = [];
        var footerData = [];

        if (rs && rs.ReportRows.length > 0) {
            var reportLength = rs.ReportRows.length;
            for (var i = 0; i < reportLength; i++) {
                var item = rs.ReportRows[i];
                currentRow =
                {
                    Columns: [{ Name: "ColFightNo", Value: item.FightNo },
                                { Name: "ColHomeTeam", Value: item.HomeOwner },
                                { Name: "ColVs", Value: "-vs-" },
                                { Name: "ColAwayTeam", Value: item.AwayOwner },
                                { Name: "ColWinLoss", Value: item.WinLoss },
                                { Name: "ColPosition", Value: item.Position },
                                { Name: "ColCommission", Value: item.Commission },
                                { Name: "ColVirtualCommission", Value: item.VirtualCommission },
                                { Name: "ColLineTotal", Value: item.RealLineTotal },
                                { Name: "ColVirtualLineTotal", Value: item.VirtualLineTotal },
                                { Name: "ColVariance", Value: item.Variance },
                                { Name: "ColTurnover", Value: item.RealTurnover },
                                { Name: "ColMargin", Value: item.Margin}]
                };

                bodyData.push(currentRow);
            }

            var grandTotal = rs.GrandTotal;
            currentRow =
                {
                    Columns: [{ Name: "ColFightNo", Value: _params.getCockFightingMatchByArenaParams.ArenaName + " Total", MergeColNumber: 4, CellAlign: "right" },
                                { Name: "ColWinLoss", Value: grandTotal.WinLoss },
                                { Name: "ColPosition", Value: grandTotal.Position },
                                { Name: "ColCommission", Value: grandTotal.Commission },
                                { Name: "ColVirtualCommission", Value: grandTotal.VirtualCommission },
                                { Name: "ColLineTotal", Value: grandTotal.RealLineTotal },
                                { Name: "ColVirtualLineTotal", Value: grandTotal.VirtualLineTotal },
                                { Name: "ColVariance", Value: grandTotal.Variance },
                                { Name: "ColTurnover", Value: grandTotal.RealTurnover },
                                { Name: "ColMargin", Value: grandTotal.Margin}]
                };

            footerData.push(currentRow);
        }

        var gridOptions = {
            customizeHeader: true,
            groupHeaderRows: [
             {
                 Columns: [
                        { HeaderText: "Fight No.", Name: "ColFightNo", MergeRowNumber: 2 },
                        { HeaderText: "Match", Name: "ColHomeTeam", MergeColNumber: 3, MergeRowNumber: 2 },
                        { HeaderText: "Win Loss", Name: "ColWinLoss", MergeRowNumber: 2 },
                        { HeaderText: "Position", Name: "ColPosition", MergeRowNumber: 2 },
                        { HeaderText: "Comm", Name: "ColCommission", MergeRowNumber: 2 },
                        { HeaderText: "Virtual Comm", Name: "ColVirtualCommission", MergeRowNumber: 2 },
                        { HeaderText: "Line Total", Name: "ColLineTotal", MergeRowNumber: 2 },
                        { HeaderText: "Virtual Line Total", Name: "ColVirtualLineTotal", MergeRowNumber: 2 },
                        { HeaderText: "Variance", Name: "ColVariance", MergeRowNumber: 2 },
                        { HeaderText: "Turnover", Name: "ColTurnover", MergeRowNumber: 2 },
                        { HeaderText: "Margin%", Name: "ColMargin", MergeRowNumber: 2 }
                    ]
             }
            ],
            columns: [{ HeaderText: "Fight No.", Width: 80, Name: "ColFightNo", HeaderAlign: "center", CellAlign: "center" },
                      { HeaderText: "Home Team", Width: 80, Name: "ColHomeTeam", HeaderAlign: "center", CellAlign: "right", FormatNumber: FormatConstant.TWO_DECIMAL_NUMBER },
                      { HeaderText: "Vs", Width: 30, Name: "ColVs", HeaderAlign: "center", CellAlign: "center", FormatNumber: FormatConstant.TWO_DECIMAL_NUMBER },
                      { HeaderText: "Away Team", Width: 80, Name: "ColAwayTeam", HeaderAlign: "center", CellAlign: "left", FormatNumber: FormatConstant.TWO_DECIMAL_NUMBER },
                      { HeaderText: "Win Loss", Width: 80, Name: "ColWinLoss", HeaderAlign: "center", CellAlign: "right", FormatNumber: FormatConstant.TWO_DECIMAL_NUMBER },
                      { HeaderText: "Position", Width: 80, Name: "ColPosition", HeaderAlign: "center", CellAlign: "right", FormatNumber: FormatConstant.TWO_DECIMAL_NUMBER },
                      { HeaderText: "Comm", Width: 80, Name: "ColCommission", HeaderAlign: "center", CellAlign: "right", FormatNumber: FormatConstant.TWO_DECIMAL_NUMBER },
                      { HeaderText: "Virtual Comm", Width: 80, Name: "ColVirtualCommission", HeaderAlign: "center", CellAlign: "right", FormatNumber: FormatConstant.TWO_DECIMAL_NUMBER },
                      { HeaderText: "Line Total", Width: 80, Name: "ColLineTotal", HeaderAlign: "center", CellAlign: "right", FormatNumber: FormatConstant.TWO_DECIMAL_NUMBER },
                      { HeaderText: "Virtual Line Total", Width: 80, Name: "ColVirtualLineTotal", HeaderAlign: "center", CellAlign: "right", FormatNumber: FormatConstant.TWO_DECIMAL_NUMBER },
                      { HeaderText: "Variance", Width: 80, Name: "ColVariance", HeaderAlign: "center", CellAlign: "right", FormatNumber: FormatConstant.TWO_DECIMAL_NUMBER },
                      { HeaderText: "Turnover", Width: 80, Name: "ColTurnover", HeaderAlign: "center", CellAlign: "right", FormatNumber: FormatConstant.TWO_DECIMAL_NUMBER },
                      { HeaderText: "Margin%", Width: 80, Name: "ColMargin", HeaderAlign: "center", CellAlign: "right", FormatNumber: FormatConstant.TWO_DECIMAL_PERCENT_NUMBER}],
            showTitle: true,
            showFooter: true,
            titleInfo:
            {
                text: 'Cockfighting Product Arena ' + _params.getCockFightingMatchByArenaParams.ArenaName + ' Win/Loss Report For ' + moment(new Date(_params.getCockFightingMatchByArenaParams.WinLostDate)).format(FormatConstant.MMM_DD_YYYY_STRING_MOMENT),
                showExcelButton: true,
                exportExcelFunction: function () {
                    exportCockFightMatchByArenaReport();
                },
                showBackButton: true,
                backFunction: function () {
                    console.log("back");
                    backToArenaReport(cockFightingGridOptions);
                }
            },
            showTree: false,
            bodyRows: bodyData,
            showDistinctRow: false,
            footerRows: footerData,
            gridExpandHeight: 450,
            numberOfFrozenColumn: 0,
            showHighlightRow: false
        };

        _tables.reportByArenaContent.removeClass("display-none").fanexGrid(gridOptions);
        _tables.reportByArenaContainer.dialog({
            title: "",
            autoOpen: false,
            width: "auto",
            modal: false,
            resizable: false,
            close: function (event, ui) {
                _tables.reportContainer.dialog("destroy").remove();
                _tables.reportParentContainer.append('<div id="cockFightingMatchByArenaReportContainer" class="display-none"> \
                                                        <div id="cockFightingMatchByArenaReportContent" class="freegrid display-none"> \
                                                        </div> \
                                                      </div>');
                _tables.reportByArenaContainer = $('#cockFightingMatchByArenaReportContainer');
                _tables.reportByArenaContent = $('#cockFightingMatchByArenaReportContent');
            }
        });

        _tables.reportByArenaContainer.dialog("open");
        _tables.reportByArenaContent.fanexGrid("resizeGrid");
    };

    var exportCockFightMatchByArenaReport = function () {
        AccReport.downloadFile(window.rootUrl + "Main/MatchWinLoss2a/ExportCockFightingMatchByArenaReport", _params.getCockFightingMatchByArenaParams);
    };

    self.closeCockFightingByArenaDialog = function () {
        try {
            _tables.reportByArenaContainer.dialog("close");
        }
        catch (e) {
        }
    };

    var backToArenaReport = function (gridOptions) {
        self.closeCockFightingByArenaDialog();
        _tables.report.removeClass("display-none").fanexGrid(gridOptions);
        _tables.reportContainer.dialog({
            title: "",
            autoOpen: false,
            width: 920,
            modal: false,
            resizable: false,
            close: function (event, ui) {
                _tables.reportContainer.dialog("destroy").remove();
                _tables.reportParentContainer.append('<div id="cockFightingReportContainer" class="display-none"> \
                                                        <div id="cockFightingReportContent" class="freegrid display-none"> \
                                                        </div> \
                                                      </div>');
                _tables.reportContainer = $('#cockFightingReportContainer');
                _tables.report = $('#cockFightingReportContent');
            }
        });

        _tables.reportContainer.dialog("open");
        _tables.report.fanexGrid("resizeGrid");
    };
}