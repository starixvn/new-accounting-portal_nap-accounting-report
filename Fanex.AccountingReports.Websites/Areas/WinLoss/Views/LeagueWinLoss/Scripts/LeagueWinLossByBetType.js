Namespace.Register("Fanex.AccountingReports.WinLoss.LeagueWinLoss.LeagueWinLossByBetType");
Fanex.AccountingReports.WinLoss.LeagueWinLoss.LeagueWinLossByBetType = function () {
    var self = this;
    var _tables = {
        reportContainer: null,
        report: null
    };

    self.Params = {
        reportParams: {
            Winlostdate: null,
            Leagueid: null,
            Sporttype: null,
            Bettype: null,
            BetTypeName: null,
            Liveindicator: null,
            IsHedge: null,
            LeagueName: '',
            SportName: ''
        }
    };
    self.initPage = function () {
        cachedDOMElements();
    };
    var cachedDOMElements = function () {
        _tables.reportParentContainer = $("#parentContainer");
        _tables.reportContainer = $('#leagueWinLossReportByBetTypeContainer');
        _tables.report = $('#leagueWinLossReportByBetType');
    };
    self.getReport = function (params) {
        AccReport.disableInput();
        getParams(params);
        $.ajax({
            type: "POST",
            url: window.rootUrl + "WinLoss/LeagueWinLoss/GetLeagueWinLossByBetType",
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            data: JSON.stringify(self.Params.reportParams),
            beforeSend: function () {
                AccReport.disableInput();
            },
            success: function (rs) {
                renderReport(rs);
            },
            error: function (event, jqXHR, ajaxSettings) {
                renderReport(null);
            },
            complete: function () {
                AccReport.enableInput();
            }
        });
    };
    var getParams = function (params) {
        self.Params.reportParams = params;
    };
    var renderReport = function (report) {
        _tables.reportContainer.removeClass("display-none");
        _tables.report.addClass("display-none");
        var bodyData = [];
        var footerData = [];

        if (report.ChildsInfo.length > 0) {
            var len = report.ChildsInfo.length;
            var currentRow = null;
            var info = null;
            for (var i = 0; i < len; i++) {
                info = report.ChildsInfo[i];
                currentRow =
                {
                    Columns: [{ Name: "ColCode", Value: info.MatchCode },
                              { Name: "ColEvent", Value: info.Event },
                              { Name: "ColWinLoss", Value: info.WinLoss },
                              { Name: "ColPosition", Value: info.Position },
                              { Name: "ColCommision", Value: info.Commission },
                              { Name: "ColLineTotal", Value: info.LineTotal },
                              { Name: "ColTurnOver", Value: info.TurnOver },
                              { Name: "ColMargin", Value: info.Margin}],
                    CssClass: i % 2 == 0 ? "even" : "",
                    SubRows: []
                };

                bodyData.push(currentRow);
            }

            info = report.Total;
            currentRow =
            {
                Columns: [{ Name: "ColCode", Value: info.Name, CssClass: "align-right", MergeColNumber: 2 },
                          { Name: "ColWinLoss", Value: info.WinLoss },
                          { Name: "ColPosition", Value: info.Position },
                          { Name: "ColCommision", Value: info.Commission },
                          { Name: "ColLineTotal", Value: info.LineTotal },
                          { Name: "ColTurnOver", Value: info.TurnOver },
                          { Name: "ColMargin", Value: info.Margin}]
            };

            footerData.push(currentRow);

            if (is8RMActive.toLowerCase() == 'true') {
                info = report.EightRM;
                currentRow =
                {
                    Columns: [{ Name: "ColCode", Value: info.Name, CssClass: "align-right", MergeColNumber: 5 },
                              { Name: "ColLineTotal", Value: info.LineTotal },
                              { Name: "ColTurnOver", Value: "", MergeColNumber: 2}]
                };

                footerData.push(currentRow);
            }

            if (isBARMActive.toLowerCase() == 'true') {
                info = report.BARM;
                currentRow =
                {
                    Columns: [{ Name: "ColCode", Value: info.Name, CssClass: "align-right", MergeColNumber: 5 },
                              { Name: "ColLineTotal", Value: info.LineTotal },
                              { Name: "ColTurnOver", Value: "&nbsp;", MergeColNumber: 2}]
                };

                footerData.push(currentRow);
            }

            info = report.GrandTotal;
            currentRow =
            {
                Columns: [{ Name: "ColCode", Value: info.Name, CssClass: "align-right", MergeColNumber: 5 },
                          { Name: "ColLineTotal", Value: info.LineTotal },
                          { Name: "ColTurnOver", Value: "", MergeColNumber: 2}]
            };

            footerData.push(currentRow);
        }

        var gridOptions = {
            columns: [{ HeaderText: "Code", Width: 100, Name: "ColCode", HeaderAlign: "center", CellAlign: "center" },
                      { HeaderText: "Event", Width: 200, Name: "ColEvent", HeaderAlign: "center", CellAlign: "center" },
                      { HeaderText: "Win Loss", Width: 100, Name: "ColWinLoss", HeaderAlign: "center", CellAlign: "right", FormatNumber: FormatConstant.TWO_DECIMAL_NUMBER, FormatPositive: '$' },
                      { HeaderText: "Position", Width: 100, Name: "ColPosition", HeaderAlign: "center", CellAlign: "right", FormatNumber: FormatConstant.TWO_DECIMAL_NUMBER },
                      { HeaderText: "Comm", Width: 100, Name: "ColCommision", HeaderAlign: "center", CellAlign: "right", FormatNumber: FormatConstant.TWO_DECIMAL_NUMBER },
                      { HeaderText: "Line Total", Width: 100, Name: "ColLineTotal", HeaderAlign: "center", CellAlign: "right", FormatNumber: FormatConstant.TWO_DECIMAL_NUMBER, FormatPositive: '$' },
                      { HeaderText: "Turnover", Width: 100, Name: "ColTurnOver", HeaderAlign: "center", CellAlign: "right", FormatNumber: FormatConstant.TWO_DECIMAL_NUMBER },
                      { HeaderText: "Margin", Width: 100, Name: "ColMargin", HeaderAlign: "center", CellAlign: "right", FormatNumber: FormatConstant.TWO_DECIMAL_PERCENT_NUMBER, FormatPositive: '$'}],
            showTitle: true,
            showFooter: true,
            titleInfo:
            {
                text: self.Params.reportParams.SportName + " - " +
                      self.Params.reportParams.LeagueName + " - " + self.Params.reportParams.BetTypeName,
                showExcelButton: true,
                exportExcelFunction: function () {
                    self.exportReport();
                },

                backFunction: function () {
                    backToLeagueWinLoss();
                },

                showCollapseExpandButton: false
            },
            showTree: false,
            bodyRows: bodyData,
            showDistinctRow: false,
            footerRows: footerData,
            gridWidth: 900
        };

        _tables.report.removeClass("display-none").fanexGrid(gridOptions);
        _tables.reportContainer.dialog({
            title: "League Winloss Report",
            autoOpen: false,
            height: 550,
            width: 980,
            modal: false,
            resizable: false,
            close: function (event, ui) {
                _tables.reportContainer.dialog("destroy").remove();
                _tables.reportParentContainer.append('<div id="leagueWinLossReportByBetTypeContainer" class="display-none"> \
                                                        <div id="leagueWinLossReportByBetType" class="freegrid display-none"> \
                                                        </div> \
                                                      </div>');
                _tables.reportContainer = $('#leagueWinLossReportByBetTypeContainer');
                _tables.report = $('#leagueWinLossReportByBetType');
            }
        });

        _tables.reportContainer.dialog("open");
    };
    self.hideReport = function (isDialogCall) {
        try {
            _tables.reportContainer.dialog("close");
        }
        catch (e) {
        }

        _tables.reportContainer.addClass("display-none");
    };
    self.exportReport = function () {
        AccReport.downloadFile(window.rootUrl + "WinLoss/LeagueWinLoss/ExportLeagueWinLossByBetType", self.Params.reportParams);
    };
}