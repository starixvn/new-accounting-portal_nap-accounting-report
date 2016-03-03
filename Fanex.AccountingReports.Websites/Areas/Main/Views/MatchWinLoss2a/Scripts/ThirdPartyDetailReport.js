Namespace.Register("Fanex.AccountingReports.Main.MatchWinLoss2a.ThirdPartyDetailReport");
Fanex.AccountingReports.Main.MatchWinLoss2a.ThirdPartyDetailReport = function () {
    var self = this;

    var _tables = {
        reportContainer: null,
        report: null
    };
    var _params = {
        reportParams: {
            Winlostdate: null,
            SportName: null
        },

        metricGamingParams: {
            Winlostdate: null,
            SubProduct: 'MG-SuperLive'
        }
    };

    self.initPage = function () {
        cachedDOMElements();
    };
    var cachedDOMElements = function () {
        _tables.reportParentContainer = $("#parentContainer");
        _tables.reportContainer = $('#thirdPartyReportContainer');
        _tables.report = $('#thirdPartyReport');
    };
    self.getReport = function (params) {
        AccReport.disableInput();
        _params.reportParams = params;

        AccReport.utility.CallAjax(
            this,
            window.rootUrl + "Main/MatchWinLoss2a/GetThirdPartyCasinoReport",
            _params.reportParams,
            function (rs) {
                renderReport(rs);
            },
            function (jqXHR, event, ajaxSettings) {
            },
            AccReport.enableInput);
    };
    var renderReport = function (report) {
        _tables.reportContainer.removeClass("display-none");
        _tables.report.addClass("display-none");
        var bodyData = [];
        var footerData = [];

        if (report && report.ChildsInfo.length > 0) {
            var reportLen = report.ChildsInfo.length;
            for (var i = 0; i < reportLen; i++) {
                var winLossReport = report.ChildsInfo[i];
                var len = winLossReport.ChildsInfo.length;
                var currentRow = null;
                var info = null;
                for (var j = 0; j < len; j++) {
                    info = winLossReport.ChildsInfo[j];

                    if (j == 0) {
                        currentRow =
                        {
                            Columns: [{ Name: "ColBetType", Value: info.Name, MergeRowNumber: len + 1, CssClass: "bold" },
                                      { Name: "ColCurrency", Value: info.Currency },
                                      { Name: "ColTurnover", Value: info.Turnover_Str },
                                      { Name: "ColWinLoss", Value: info.WinLoss_Str },
                                      { Name: "ColPosition", Value: info.Position_Str },
                                      { Name: "ColCommision", Value: info.Commission_Str },
                                      { Name: "ColLineTotal", Value: info.LineTotal_Str },
                                      { Name: "ColMargin", Value: info.Margin_Str}]
                        };
                    }
                    else {
                        currentRow =
                        {
                            Columns: [{ Name: "ColCurrency", Value: info.Currency },
                                      { Name: "ColTurnover", Value: info.Turnover_Str },
                                      { Name: "ColWinLoss", Value: info.WinLoss_Str },
                                      { Name: "ColPosition", Value: info.Position_Str },
                                      { Name: "ColCommision", Value: info.Commission_Str },
                                      { Name: "ColLineTotal", Value: info.LineTotal_Str },
                                      { Name: "ColMargin", Value: info.Margin_Str}]
                        };
                    }

                    bodyData.push(currentRow);
                }

                info = winLossReport.GrandTotal;
                currentRow =
                {
                    CssClass: "bold",
                    Columns: [{ Name: "ColCurrency", Value: info.Name },
                              { Name: "ColTurnover", Value: info.Turnover_Str },
                              { Name: "ColWinLoss", Value: info.WinLoss_Str },
                              { Name: "ColPosition", Value: info.Position_Str },
                              { Name: "ColCommision", Value: info.Commission_Str },
                              { Name: "ColLineTotal", Value: info.LineTotal_Str },
                              { Name: "ColMargin", Value: info.Margin_Str}]
                };

                bodyData.push(currentRow);
            }

            currentRow =
            {
                Columns: [{ Name: "ColCurrency", MergeColNumber: 2 },
                          { Name: "ColWinLoss", MergeColNumber: 6}]
            };

            bodyData.push(currentRow);
            var len = report.Total.ChildsInfo.length;
            for (var j = 0; j < len; j++) {
                info = report.Total.ChildsInfo[j];

                if (j == 0) {
                    currentRow =
                    {
                        CssClass: "subTotal",
                        Columns: [{ Name: "ColBetType", Value: report.Total.Name, MergeRowNumber: len },
                                  { Name: "ColCurrency", Value: info.Currency },
                                  { Name: "ColTurnover", Value: info.Turnover_Str },
                                  { Name: "ColWinLoss", Value: info.WinLoss_Str },
                                  { Name: "ColPosition", Value: info.Position_Str },
                                  { Name: "ColCommision", Value: info.Commission_Str },
                                  { Name: "ColLineTotal", Value: info.LineTotal_Str },
                                  { Name: "ColMargin", Value: info.Margin_Str}]
                    };
                }
                else {
                    currentRow =
                    {
                        CssClass: "subTotal",
                        Columns: [{ Name: "ColCurrency", Value: info.Currency },
                                  { Name: "ColTurnover", Value: info.Turnover_Str },
                                  { Name: "ColWinLoss", Value: info.WinLoss_Str },
                                  { Name: "ColPosition", Value: info.Position_Str },
                                  { Name: "ColCommision", Value: info.Commission_Str },
                                  { Name: "ColLineTotal", Value: info.LineTotal_Str },
                                  { Name: "ColMargin", Value: info.Margin_Str}]
                    };
                }

                bodyData.push(currentRow);
            }

            info = report.GrandTotal;
            currentRow =
            {
                Columns: [{ Name: "ColBetType", Value: info.Name, MergeColNumber: 2 },
                          { Name: "ColTurnover", Value: info.Turnover_Str },
                          { Name: "ColWinLoss", Value: info.WinLoss_Str },
                          { Name: "ColPosition", Value: info.Position_Str },
                          { Name: "ColCommision", Value: info.Commission_Str },
                          { Name: "ColLineTotal", Value: info.LineTotal_Str },
                          { Name: "ColMargin", Value: info.Margin_Str}]
            };

            footerData.push(currentRow);
        }

        var gridOptions = {
            columns: [{ HeaderText: "Bet Type", Width: 120, Name: "ColBetType", HeaderAlign: "center", CellAlign: "center" },
                      { HeaderText: "Currency", Width: 100, Name: "ColCurrency", HeaderAlign: "center", CellAlign: "center" },
                      { HeaderText: "Turnover", Width: 100, Name: "ColTurnover", HeaderAlign: "center", CellAlign: "right" },
                      { HeaderText: "Win Loss", Width: 100, Name: "ColWinLoss", HeaderAlign: "center", CellAlign: "right" },
                      { HeaderText: "Position", Width: 90, Name: "ColPosition", HeaderAlign: "center", CellAlign: "right" },
                      { HeaderText: "Comm", Width: 80, Name: "ColCommision", HeaderAlign: "center", CellAlign: "right" },
                      { HeaderText: "Line Total", Width: 100, Name: "ColLineTotal", HeaderAlign: "center", CellAlign: "right" },
                      { HeaderText: "Margin%", Width: 60, Name: "ColMargin", HeaderAlign: "center", CellAlign: "right"}],
            showTitle: true,
            showFooter: true,
            titleInfo:
            {
                text: 'Casino Daily Report: ' + moment(new Date(_params.reportParams.Winlostdate)).format(FormatConstant.MMM_DD_YYYY_STRING_MOMENT),
                showExcelButton: false,
                showCollapseExpandButton: false
            },
            showTree: false,
            bodyRows: bodyData,
            showDistinctRow: false,
            footerRows: footerData,
            gridExpandHeight: 450,
            gridWidth: 772,
            numberOfFrozenColumn: 0,
            showHighlightRow: false
        };

        _tables.report.removeClass("display-none").fanexGrid(gridOptions);
        _tables.reportContainer.dialog({
            title: "",
            autoOpen: false,
            width: 852,
            modal: false,
            resizable: false,
            close: function (event, ui) {
                _tables.reportContainer.dialog("destroy").remove();
                _tables.reportParentContainer.append('<div id="thirdPartyReportContainer" class="display-none"> \
                                                        <div id="thirdPartyReport" class="freegrid display-none"> \
                                                        </div> \
                                                      </div>');
                _tables.reportContainer = $('#thirdPartyReportContainer');
                _tables.report = $('#thirdPartyReport');
            }
        });

        _tables.reportContainer.dialog("open");
    };
    self.closeDialog = function () {
        try {
            _tables.reportContainer.dialog("close");
        }
        catch (e) {
        }
    };

    self.getMetricGamingReport = function (params) {
        AccReport.disableInput();
        _params.metricGamingParams.Winlostdate = params.Winlostdate;

        AccReport.utility.CallAjax(
            this,
            window.rootUrl + "Main/MatchWinLoss2a/GetMetricGamingByBettypeReport",
            _params.metricGamingParams,
            function (gridOptions) {
                renderMetricGamingReport(gridOptions);
            },
            function (jqXHR, event, ajaxSettings) {
            },
            AccReport.enableInput);
    };

    var renderMetricGamingReport = function (gridOptions) {
        var titleInfo = {
            text: gridOptions.TitleInfoText,
            showExcelButton: true,
            exportExcelFunction: function () {
                exportMetricGamingByBettypeReport();
            }
        };

        gridOptions.titleInfo = titleInfo;
        gridOptions.numberOfFrozenColumn = 0;
        _tables.report.removeClass("display-none").fanexGrid(gridOptions);
        _tables.reportContainer.dialog({
            title: "",
            autoOpen: false,
            width: "auto",
            modal: false,
            resizable: false,
            close: function (event, ui) {
                _tables.reportContainer.dialog("destroy").remove();
                _tables.reportParentContainer.append('<div id="thirdPartyReportContainer" class="display-none"> \
                                                    <div id="thirdPartyReport" class="freegrid display-none"> \
                                                    </div> \
                                                    </div>');
                _tables.reportContainer = $('#thirdPartyReportContainer');
                _tables.report = $('#thirdPartyReport');
            }
        });

        _tables.reportContainer.dialog("open");
    };

    var exportMetricGamingByBettypeReport = function () {
        AccReport.downloadFile(window.rootUrl + "Main/MatchWinLoss2a/ExportMetricGamingByBettypeReport", _params.metricGamingParams);
    };
}