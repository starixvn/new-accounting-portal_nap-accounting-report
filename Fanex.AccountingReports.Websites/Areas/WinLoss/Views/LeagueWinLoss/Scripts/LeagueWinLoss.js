

Namespace.Register("Fanex.AccountingReports.WinLoss.LeagueWinLoss.LeagueWinLoss");
Fanex.AccountingReports.WinLoss.LeagueWinLoss.LeagueWinLoss = function () {
    var self = this;
    var _tables = {
        btnGenerate: null,
        datepickerplugin: null,
        reportContainer: null,
        report: null
    };

    self.Params = {
        reportParams: {
            Winlostdate: null
        }
    };

    self.initPage = function () {
        cachedDOMElements();
        renderDatePicker();
        bindingEvent();
    };
    var cachedDOMElements = function () {
        _tables.btnGenerate = $("#btnGenerate");
        _tables.reportContainer = $("#leagueWinLossReportContainer");
        _tables.report = $("#leagueWinLossReport");

    };
    var renderDatePicker = function () {
        $('#datepickerplugin').nexDatePicker({
            nexDatePickerType: "single",
            firstDatePicker: {
                Id: "CheckDate",
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
                maxDate: new Date(toDay)
            }
        });

        _tables.datepickerplugin = $("#datepickerplugin");
    };
    var bindingEvent = function () {
        _tables.btnGenerate.click(function () {
            self.btnGenerateClick();
        });
    };
    self.btnGenerateClick = function () {
        AccReport.disableInput();
        hideLeagueWinLossBySport();
        getParams();
        $.ajax({
            type: "POST",
            url: window.rootUrl + "WinLoss/LeagueWinLoss/GetLeagueWinLoss",
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            data: JSON.stringify(self.Params.reportParams),
            beforeSend: function () {
                AccReport.disableInput();
            },
            success: function (rs) {
                renderLeagueWinLoss(rs);
            },
            error: function (event, jqXHR, ajaxSettings) {
                renderLeagueWinLoss([]);
            },
            complete: function () {
                AccReport.enableInput();
            }
        });
    };
    var getParams = function () {
        self.Params.reportParams.Winlostdate = _tables.datepickerplugin.nexDatePicker("getSingleDate");
    };
    function showNormalSportsTotal(grandTotal, currentRow) {

        var totalRow = {
            Columns: [{ Name: "ColLeague", Value: grandTotal.Name, CssClass: "colLeague align-right" },
                      { Name: "ColWinLoss", Value: grandTotal.WinLoss },
                      { Name: "ColPosition", Value: grandTotal.Position },
                      { Name: "ColCommision", Value: grandTotal.Commission },
                      { Name: "ColLineTotal", Value: grandTotal.LineTotal },
                      { Name: "ColTurnOver", Value: grandTotal.TurnOver },
                      { Name: "ColMargin", Value: grandTotal.Margin }],
            CssClass: " subTotal"
        };

        currentRow.SubRows.push(totalRow);
    };

    function showNewCritketTotal(grandTotal, currentRow) {
        var childTotals = grandTotal.ChildsInfo;
        var grandTotalLength = childTotals.length;
        for (var k = 0; k < grandTotalLength; k++) {
            var info = childTotals[k];
            var totalRow = {
                Columns: [{ Name: "ColLeague", Value: info.Name, CssClass: "colLeague align-right" },
                          { Name: "ColWinLoss", Value: info.WinLoss },
                          { Name: "ColPosition", Value: info.Position },
                          { Name: "ColCommision", Value: info.Commission },
                          { Name: "ColLineTotal", Value: info.LineTotal },
                          { Name: "ColTurnOver", Value: info.TurnOver },
                          { Name: "ColMargin", Value: info.Margin }],
                CssClass: " subTotal"
            };
            currentRow.SubRows.push(totalRow);
        }
    };
    var renderLeagueWinLoss = function (report) {
        self.showReport();
        _tables.reportContainer.removeClass("display-none");
        _tables.report.addClass("display-none");
        var bodyData = [];
        var footerData = [];

        if (report.ChildsInfo.length > 0) {
            var len = report.ChildsInfo.length;
            var currentSubRow = null;
            var info = null;
            for (var i = 0; i < len; i++) {
                var sport = report.ChildsInfo[i];
                var currentRow =
                {
                    Columns: [{ Name: "ColLeague", Value: sport.Name, MergeColNumber: 7 }],
                    SubRows: []
                };

                var childsLength = sport.ChildsInfo.length;
                for (var j = 0; j < childsLength; j++) {
                    info = sport.ChildsInfo[j];
                    currentSubRow =
                    {
                        Columns: [{
                            Name: "ColLeague", Value: {
                                sportId: sport.Id,
                                sportName: sport.Name,
                                leagueId: info.Id,
                                leagueName: info.Name
                            }, CssClass: "colLeague",
                            CustomCellRender: function (currentCell) {
                                var clickEvent = ' onclick = "viewLeagueWinLossBySport(' + currentCell.Value.sportId + ', ' + currentCell.Value.leagueId + ',\'' + currentCell.Value.sportName + '\',\'' + currentCell.Value.leagueName + "');\"";
                                var html = '<div class="link " ' + clickEvent + '>' + currentCell.Value.leagueName + '</div>';
                                return html;
                            }
                        },
                                  { Name: "ColWinLoss", Value: info.WinLoss },
                                  { Name: "ColPosition", Value: info.Position },
                                  { Name: "ColCommision", Value: info.Commission },
                                  { Name: "ColLineTotal", Value: info.LineTotal },
                                  { Name: "ColTurnOver", Value: info.TurnOver },
                                  { Name: "ColMargin", Value: info.Margin }],
                        CssClass: j % 2 == 0 ? "even" : ""
                    };

                    currentRow.SubRows.push(currentSubRow);
                }

                if (sport.Id == 50) {
                    showNewCritketTotal(sport.GrandTotal, currentRow);
                }
                else {
                    showNormalSportsTotal(sport.GrandTotal, currentRow);
                }

                bodyData.push(currentRow);
            }

            info = report.Total;
            currentRow =
            {
                Columns: [{ Name: "ColLeague", Value: info.Name, CssClass: "align-right" },
                          { Name: "ColWinLoss", Value: info.WinLoss },
                          { Name: "ColPosition", Value: info.Position },
                          { Name: "ColCommision", Value: info.Commission },
                          { Name: "ColLineTotal", Value: info.LineTotal },
                          { Name: "ColTurnOver", Value: info.TurnOver },
                          { Name: "ColMargin", Value: info.Margin }]
            };

            footerData.push(currentRow);

            if (is8RMActive.toLowerCase() == 'true') {
                info = report.EightRM;
                currentRow =
                {
                    Columns: [{ Name: "ColLeague", Value: info.Name, CssClass: "align-right", MergeColNumber: 4 },
                              { Name: "ColLineTotal", Value: info.LineTotal },
                              { Name: "ColTurnOver", Value: "", MergeColNumber: 2 }]
                };

                footerData.push(currentRow);
            }

            if (isBARMActive.toLowerCase() == 'true') {
                info = report.BARM;
                currentRow =
                {
                    Columns: [{ Name: "ColLeague", Value: info.Name, CssClass: "align-right", MergeColNumber: 4 },
                              { Name: "ColLineTotal", Value: info.LineTotal },
                              { Name: "ColTurnOver", Value: "&nbsp;", MergeColNumber: 2 }]
                };

                footerData.push(currentRow);
            }

            info = report.GrandTotal;
            currentRow =
            {
                Columns: [{ Name: "ColLeague", Value: info.Name, CssClass: "align-right", MergeColNumber: 4 },
                          { Name: "ColLineTotal", Value: info.LineTotal },
                          { Name: "ColTurnOver", Value: "", MergeColNumber: 2 }]
            };

            footerData.push(currentRow);


        }

        var gridOptions = {
            columns: [{ HeaderText: "League", Width: 300, Name: "ColLeague", HeaderAlign: "center", CellAlign: "left", ShowCollapseExpand: true },
                      { HeaderText: "Win Loss", Width: 100, Name: "ColWinLoss", HeaderAlign: "center", CellAlign: "right", FormatNumber: FormatConstant.TWO_DECIMAL_NUMBER, FormatPositive: '$' },
                      { HeaderText: "Position", Width: 100, Name: "ColPosition", HeaderAlign: "center", CellAlign: "right", FormatNumber: FormatConstant.TWO_DECIMAL_NUMBER },
                      { HeaderText: "Comm", Width: 100, Name: "ColCommision", HeaderAlign: "center", CellAlign: "right", FormatNumber: FormatConstant.TWO_DECIMAL_NUMBER },
                      { HeaderText: "Line Total", Width: 100, Name: "ColLineTotal", HeaderAlign: "center", CellAlign: "right", FormatNumber: FormatConstant.TWO_DECIMAL_NUMBER, FormatPositive: '$' },
                      { HeaderText: "Turnover", Width: 100, Name: "ColTurnOver", HeaderAlign: "center", CellAlign: "right", FormatNumber: FormatConstant.TWO_DECIMAL_NUMBER },
                      { HeaderText: "Margin", Width: 100, Name: "ColMargin", HeaderAlign: "center", CellAlign: "right", FormatNumber: FormatConstant.TWO_DECIMAL_PERCENT_NUMBER, FormatPositive: '$' }],
            showTitle: true,
            showFooter: true,
            titleInfo:
            {
                text: "League Win Loss Report on " + self.Params.reportParams.Winlostdate,
                showExcelButton: true,
                exportExcelFunction: function () {
                    self.exportReport();
                },

                showCollapseExpandButton: true
            },
            showTree: true,
            bodyRows: bodyData,
            showDistinctRow: false,
            footerRows: footerData,
            gridExpandHeight: 165
        };

        _tables.report.removeClass("display-none").fanexGrid(gridOptions);
    };
    self.hideReport = function () {
        _tables.reportContainer.addClass("display-none");
    };
    self.showReport = function () {
        _tables.reportContainer.removeClass("display-none");
    };
    self.exportReport = function () {
        AccReport.downloadFile(window.rootUrl + "WinLoss/LeagueWinLoss/ExportLeagueWinLoss", self.Params.reportParams);
    };
}