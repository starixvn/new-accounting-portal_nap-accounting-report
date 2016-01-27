Namespace.Register('Fanex.AccountingReports.Main.WinLossSummary3rd.CockFighting');
Fanex.AccountingReports.Main.WinLossSummary3rd.CockFighting = function () {
    var self = this;

    var _tables = {
        parentContainer: null,
        winlossSumReportContainer: null,
        gameMatchDetailContainer: null,
        reportContainer: null,
        reportContent: null
    };
    var _params = {
        reportParams: {
            FromDate: null,
            ToDate: null
        }
    };

    self.initPage = function () {
        cachedDOMElements();
    };
    var cachedDOMElements = function () {
        _tables.parentContainer = $("#parentContainer");
        _tables.winlossSumReportContainer = $('#winlossSumReportContainer');
        _tables.gameMatchDetailContainer = $("#gameMatchDetailContainer");
        _tables.reportContainer = $('#cockFightingContainer');
        _tables.reportContent = $('#cockFightingContent');
    };
    self.getReport = function (params) {
        AccReport.disableInput();
        getParams(params);
        $.ajax({
            type: 'POST',
            url: window.rootUrl + 'Main/WinLossSummary3rd/ViewCockFightingReport',
            contentType: 'application/json; charset=utf-8',
            dataType: 'json',
            data: JSON.stringify(_params.reportParams),
            beforeSend: function () { },
            success: function (rs) {
                renderReport(rs);
            },
            error: function (jqXhr, event, ajaxSettings) {
                if (jqXhr.status && jqXhr.status == 500) {
                    var r = jQuery.parseJSON(jqXhr.responseText);
                    AccReport.popUp.show(r.message);
                }
            },
            complete: function () {
                AccReport.enableInput();
            }
        });
    };
    var getParams = function (params) {
        _params.reportParams.FromDate = params.reportParams.FromDate;
        _params.reportParams.ToDate = params.reportParams.ToDate;
    };
    var renderReport = function (report) {
        _tables.winlossSumReportContainer.addClass("display-none");
        _tables.parentContainer.removeClass("display-none");
        _tables.reportContainer.removeClass("display-none");
        _tables.reportContent.addClass("display-none");
        var bodyData = [];
        var footerData = [];
        if (report && report.ReportRows && report.ReportRows.length > 0) {
            var reportLength = report.ReportRows.length;
            var info = null;

            bodyData.push({
                Columns: [{ Name: "ColFightNo", Value: "Cockfighting", MergeColNumber: 14, TdClass: "cockFightingTitle"}]
            });

            for (var i = 0; i < reportLength; i++) {
                var arenaItem = report.ReportRows[i];
                bodyData.push({
                    Columns: [{ Name: "ColFightNo", Value: "", TdClass: "arenaName bold" },
                            { Name: "ColWinLoss", Value: arenaItem.ArenaName, MergeColNumber: 13, TdClass: "arenaName bold", CssClass: "left-align" }
                    ]
                });

                var matchLength = arenaItem.CockFightingMatchs.length;
                for (var j = 0; j < matchLength; j++) {
                    info = arenaItem.CockFightingMatchs[j];
                    bodyData.push({
                        Columns: buildCockFightingMatchInfo(info)
                    });
                }

                info = arenaItem.ArenaTotal;
                bodyData.push({
                    Columns: buildCockFightingArenaTotal(info)
                });
            }

            info = report.GrandTotal;
            footerData.push({
                Columns: [{ Name: "ColFightNo", Value: "Grand Total", MergeColNumber: 4 },
                    { Name: "ColWinLoss", Value: info.RealWinLoss },
                    { Name: "ColPosition", Value: info.RealPosition },
                    { Name: "ColComm", Value: info.RealCommission },
                    { Name: "ColRealLineTotal", Value: info.RealLineTotal },
                    { Name: "ColVirtualLineTotal", Value: info.VirtualLineTotal },
                    { Name: "ColVariance", Value: info.Variance },
                    { Name: "ColRealTurnover", Value: info.RealTurnover },
                    { Name: "ColVirtualTurnover", Value: info.VirtualTurnover },
                    { Name: "ColRealMargin", Value: info.RealMargin },
                    { Name: "ColVirtualMargin", Value: info.VirtualMargin}]
            });
        }

        var gridOptions = {
            customizeHeader: true,
            groupHeaderRows: [
            {
                Columns: [{ HeaderText: "Fight No.", Name: "ColFightNo", MergeRowNumber: 2 },
                          { HeaderText: "Event", Name: "ColHomeTeam", MergeRowNumber: 2, MergeColNumber: 3 },
                          { HeaderText: "Win Loss", Name: "ColWinLoss", MergeRowNumber: 2 },
                          { HeaderText: "Position", Name: "ColPosition", MergeRowNumber: 2 },
                          { HeaderText: "Comm", Name: "ColComm", MergeRowNumber: 2 },
                          { HeaderText: "Line Total", Name: "ColRealLineTotal", MergeColNumber: 2 },
                          { HeaderText: "Variance", Name: "ColVariance", MergeRowNumber: 2 },
                          { HeaderText: "Turnover", Name: "ColRealTurnover", MergeColNumber: 2 },
                          { HeaderText: "Margin", Name: "ColRealMargin", MergeColNumber: 2}]
            }],

            columns: [
            { HeaderText: "Fight No.", Width: 100, Name: "ColFightNo", HeaderAlign: "center", CellAlign: "center" },
            { HeaderText: "Home Team", Width: 100, Name: "ColHomeTeam", HeaderAlign: "center", CellAlign: "right" },
            { HeaderText: "Vs", Width: 50, Name: "ColVs", HeaderAlign: "center", CellAlign: "center" },
            { HeaderText: "Away Team", Width: 100, Name: "ColAwayTeam", HeaderAlign: "center", CellAlign: "left" },
            { HeaderText: "Win Loss", Width: 100, Name: "ColWinLoss", FormatNumber: FormatConstant.TWO_DECIMAL_NUMBER, HeaderAlign: "center", CellAlign: "right", FormatPositive: '$' },
            { HeaderText: "Position", Width: 100, Name: "ColPosition", FormatNumber: FormatConstant.TWO_DECIMAL_NUMBER, HeaderAlign: "center", CellAlign: "right" },
            { HeaderText: "Comm", Width: 100, Name: "ColComm", FormatNumber: FormatConstant.TWO_DECIMAL_NUMBER, HeaderAlign: "center", CellAlign: "right" },
            { HeaderText: "Real", Width: 100, Name: "ColRealLineTotal", FormatNumber: FormatConstant.TWO_DECIMAL_NUMBER, HeaderAlign: "center", CellAlign: "right", FormatPositive: '$' },
            { HeaderText: "Virtual", Width: 100, Name: "ColVirtualLineTotal", FormatNumber: FormatConstant.TWO_DECIMAL_NUMBER, HeaderAlign: "center", CellAlign: "right", FormatPositive: '$' },
            { HeaderText: "Variance", Width: 100, Name: "ColVariance", FormatNumber: FormatConstant.TWO_DECIMAL_NUMBER, HeaderAlign: "center", CellAlign: "right", FormatPositive: '$' },
            { HeaderText: "Real", Width: 100, Name: "ColRealTurnover", FormatNumber: FormatConstant.TWO_DECIMAL_NUMBER, HeaderAlign: "center", CellAlign: "right" },
            { HeaderText: "Virtual", Width: 100, Name: "ColVirtualTurnover", FormatNumber: FormatConstant.TWO_DECIMAL_NUMBER, HeaderAlign: "center", CellAlign: "right" },
            { HeaderText: "Real", Width: 100, Name: "ColRealMargin", FormatNumber: FormatConstant.TWO_DECIMAL_PERCENT_NUMBER, HeaderAlign: "center", CellAlign: "right", FormatPositive: '$' },
            { HeaderText: "Virtual", Width: 100, Name: "ColVirtualMargin", FormatNumber: FormatConstant.TWO_DECIMAL_PERCENT_NUMBER, HeaderAlign: "center", CellAlign: "right", FormatPositive: '$'}],
            showTitle: true,
            showFooter: true,
            titleInfo: {
                text: "Cockfighting | Details report from " + _params.reportParams.FromDate +
                      " to " + _params.reportParams.ToDate,
                showExcelButton: true,
                exportExcelFunction: function () {
                    exportReport();
                },
                showBackButton: true,
                backFunction: function () {
                    back2Main();
                }
            },
            bodyRows: bodyData,
            footerRows: footerData,
            numberOfFrozenColumn: 0,
            gridExpandHeight: 200,
            headerColumnHeight: 17,
            noDataText: viewResources.EmptyReportMessage,
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
    var back2Main = function () {
        scoreInfoManager.ClosePopup();
        // show summary
        _tables.winlossSumReportContainer.removeClass('display-none');
        // hide detail
        _tables.gameMatchDetailContainer.removeClass('display-none');
        _tables.gameMatchDetailContainer.addClass('display-none');
        if (!_tables.gameMatchDetailContainer.hasClass("display-none")) {
            _tables.gameMatchDetailContainer.fanexGrid("resizeGrid");
        }

        _tables.reportContainer.removeClass('display-none');
        _tables.reportContainer.addClass('display-none');
        if (!_tables.reportContainer.hasClass("display-none")) {
            _tables.reportContainer.fanexGrid("resizeGrid");
        }
    };
    var exportReport = function () {
        AccReport.downloadFile(window.rootUrl + "Main/WinLossSummary3rd/ExportCockFightingReport", _params.reportParams);
    };

    var buildCockFightingMatchInfo = function (info) {
        return [{ Name: "ColFightNo", Value: info.FightNo },
        { Name: "ColHomeTeam", Value: info.HomeOwner },
        { Name: "ColVs", Value: "-vs-" },
        { Name: "ColAwayTeam", Value: info.AwayOwner },
        { Name: "ColWinLoss", Value: info.RealWinLoss },
        { Name: "ColPosition", Value: info.RealPosition },
        { Name: "ColComm", Value: info.RealCommission },
        { Name: "ColRealLineTotal", Value: info.RealLineTotal },
        { Name: "ColVirtualLineTotal", Value: info.VirtualLineTotal },
        { Name: "ColVariance", Value: info.Variance },
        { Name: "ColRealTurnover", Value: info.RealTurnover },
        { Name: "ColVirtualTurnover", Value: info.VirtualTurnover },
        { Name: "ColRealMargin", Value: info.RealMargin },
        { Name: "ColVirtualMargin", Value: info.VirtualMargin}];
    }

    var buildCockFightingArenaTotal = function (info) {
        return [{ Name: "ColFightNo", Value: "Total", MergeColNumber: 4, TdClass: "bold totalRow", CssClass: "totalLabel" },
        { Name: "ColWinLoss", Value: info.RealWinLoss, TdClass: "totalRow" },
        { Name: "ColPosition", Value: info.RealPosition, TdClass: "totalRow" },
        { Name: "ColComm", Value: info.RealCommission, TdClass: "totalRow" },
        { Name: "ColRealLineTotal", Value: info.RealLineTotal, TdClass: "totalRow" },
        { Name: "ColVirtualLineTotal", Value: info.VirtualLineTotal, TdClass: "totalRow" },
        { Name: "ColVariance", Value: info.Variance, TdClass: "totalRow" },
        { Name: "ColRealTurnover", Value: info.RealTurnover, TdClass: "totalRow" },
        { Name: "ColVirtualTurnover", Value: info.VirtualTurnover, TdClass: "totalRow" },
        { Name: "ColRealMargin", Value: info.RealMargin, TdClass: "totalRow" },
        { Name: "ColVirtualMargin", Value: info.VirtualMargin, TdClass: "totalRow"}];
    }
}