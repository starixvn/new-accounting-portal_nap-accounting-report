var TopPunter_ByLeague_BetType = function () {
    var self = this;

    self.Tables = {
        reportContainer: null,
        report: null,

        summaryReportContainer: null,
        summaryReport: null,
        summaryReportBody: null,
        summaryReportTitle: null,

        ddwColumnSettingContainer: null,
        ddwColumnSetting: null,
        columnSetting: null
    };

    self.Params = {
        reportParams: {
            CustID: null,
            UserName: null,
            LeagueID: null,
            Sporttype: null,
            SportName: null,
            Bettype: null,
            Fromdate: null,
            Todate: null,
            Reporttype: null,
            Status: null,
            LiveTypeName: null,
            OrderByName: null,
            OrderTypeName: null,
            LeagueName: null,
            ByLeague: true
        }
    };

    self.CacheData = {
        summaryReportHeight: 0,
        report: null,
        selectedClumnGroups: [],
        GrandTotalColumnId: "0_0_0",
        isRefreshReportView: false
    };
};

TopPunter_ByLeague_BetType.prototype.InitPage = function () {
    this.CachedDOMElements();
};
TopPunter_ByLeague_BetType.prototype.CachedDOMElements = function () {
    this.Tables.reportContainer = $('#topPunter_ByLeague_BetType_Container');
    this.Tables.report = $('#topPunter_ByLeague_BetType');

    this.Tables.summaryReportContainer = $('#summaryReport_ByLeague_BetType');
    this.Tables.summaryReport = this.Tables.summaryReportContainer.find('#summaryReport_ByLeague_BetTypeContent');
    this.Tables.summaryReportBody = this.Tables.summaryReport.find("tbody");
    this.Tables.summaryReportTitle = this.Tables.summaryReportContainer.find("#summaryReport_ByLeague_BetType_Title");
};
TopPunter_ByLeague_BetType.prototype.SettingReportColumn = function () {
    if (this.Tables.ddwColumnSettingContainer.hasClass('display-none')) {
        this.Tables.ddwColumnSettingContainer.removeClass("display-none");
        this.Tables.ddwColumnSetting.multiSelect('openMe');
    } else {
        this.Tables.ddwColumnSettingContainer.addClass("display-none");
        this.Tables.ddwColumnSetting.multiSelect('hideMe');
    }
};
TopPunter_ByLeague_BetType.prototype.GetReport = function (params) {
    var self = this;

    AccReport.disableInput();
    self.Params.reportParams = params;
    $.ajax({
        type: "POST",
        url: window.rootUrl + "Special/PunterAnalysis/GetTopPunter_ByUser_BetType",
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        data: JSON.stringify(self.Params.reportParams),
        beforeSend: function () {
        },
        success: function (rs) {
            self.RenderReport(rs);
        },
        error: function (event, jqXHR, ajaxSettings) {
            self.RenderReport(null);
        },
        complete: function () {
            AccReport.enableInput();
        }
    });
};
TopPunter_ByLeague_BetType.prototype.RenderReport = function (report) {
    var self = this;
    self.Tables.summaryReportContainer.addClass("display-none");
    self.Tables.summaryReport.addClass("display-none");
    self.CacheData.report = report;
    switch (this.Params.reportParams.Reporttype) {
        case reportManager.Enum.PeriodType.Daily:
            self.RenderReportDaily(report);
            break;
        case reportManager.Enum.PeriodType.MonthRange:
            self.RenderSummaryReport(report);
            self.RenderReportNonDaily(report);
            break;
        default:
            self.RenderReportNonDaily(report);
            break;
    }
};
TopPunter_ByLeague_BetType.prototype.RenderReportDaily = function (report) {
    var self = this;

    this.Tables.reportContainer.removeClass("display-none");
    this.Tables.report.addClass("display-none");
    var bodyData = [];
    var rowColumns = [];
    var footerData = [];
    var customizeHeaderColumns = [];
    var headerColumns = [];
    var columnIdSuffix;
    if (report) {
        var leagueLength = report.ChildsInfo.length;
        for (var i = 0; i < leagueLength; i++) {
            var league = report.ChildsInfo[i];
            rowColumns = [];
            rowColumns.push({ Name: "ColLeague", Value: league.Name });
            var yearLength = league.ChildsInfo.length;
            for (var j = 0; j < yearLength; j++) {
                var year = league.ChildsInfo[j];
                columnIdSuffix = "_Year" + report.GrandTotal.ChildsInfo[j].WinLossDate_Str;
                if (year) {
                    var yearGrandTotal = year.GrandTotal;
                    var monthLength = year.ChildsInfo.length;
                    if (self.IsIncludeGroup(report.GrandTotal.ChildsInfo[j].WinLossDate_Str)) {
                        rowColumns.push({ Name: "ColWinLoss" + columnIdSuffix, Value: reportManager.FormatNumberCell(yearGrandTotal.WinLoss_Str, false, true, false) });
                        rowColumns.push({ Name: "ColTurnover" + columnIdSuffix, Value: yearGrandTotal.Turnover_Str });
                        rowColumns.push({ Name: "ColMargin" + columnIdSuffix, Value: reportManager.FormatNumberCell(yearGrandTotal.Margin_Str, false, true, false) });
                        rowColumns.push({ Name: "ColBetCount" + columnIdSuffix, Value: yearGrandTotal.BetCount_Str });
                    }

                    for (var k = 0; k < monthLength; k++) {
                        var month = year.ChildsInfo[k];
                        columnIdSuffix = "_Year" + report.GrandTotal.ChildsInfo[j].WinLossDate_Str +
                                     "_Month" + report.GrandTotal.ChildsInfo[j].ChildsInfo[k].WinLossDate_Str;
                        if (self.IsIncludeGroup(report.GrandTotal.ChildsInfo[j].ChildsInfo[k].WinLossDate_Str)) {
                            if (month) {
                                rowColumns.push({ Name: "ColWinLoss" + columnIdSuffix, Value: reportManager.FormatNumberCell(month.WinLoss_Str, false, true, false) });
                                rowColumns.push({ Name: "ColTurnover" + columnIdSuffix, Value: month.Turnover_Str });
                                rowColumns.push({ Name: "ColMargin" + columnIdSuffix, Value: reportManager.FormatNumberCell(month.Margin_Str, false, true, false) });
                                rowColumns.push({ Name: "ColBetCount" + columnIdSuffix, Value: month.BetCount_Str });
                            }
                            else {
                                rowColumns.push({ Name: "ColWinLoss" + columnIdSuffix });
                                rowColumns.push({ Name: "ColTurnover" + columnIdSuffix });
                                rowColumns.push({ Name: "ColMargin" + columnIdSuffix });
                                rowColumns.push({ Name: "ColBetCount" + columnIdSuffix });
                            }
                        }
                    }
                } else {
                    if (self.IsIncludeGroup(report.GrandTotal.ChildsInfo[j].WinLossDate_Str)) {
                        rowColumns.push({ Name: "ColWinLoss" + columnIdSuffix });
                        rowColumns.push({ Name: "ColTurnover" + columnIdSuffix });
                        rowColumns.push({ Name: "ColMargin" + columnIdSuffix });
                        rowColumns.push({ Name: "ColBetCount" + columnIdSuffix });
                    }

                    var monthLength = report.GrandTotal.ChildsInfo[j].ChildsInfo.length;
                    for (var k = 0; k < monthLength; k++) {
                        columnIdSuffix = "_Year" + report.GrandTotal.ChildsInfo[j].WinLossDate_Str +
                                     "_Month" + report.GrandTotal.ChildsInfo[j].ChildsInfo[k].WinLossDate_Str;
                        if (self.IsIncludeGroup(report.GrandTotal.ChildsInfo[j].ChildsInfo[k].WinLossDate_Str)) {
                            rowColumns.push({ Name: "ColWinLoss" + columnIdSuffix });
                            rowColumns.push({ Name: "ColTurnover" + columnIdSuffix });
                            rowColumns.push({ Name: "ColMargin" + columnIdSuffix });
                            rowColumns.push({ Name: "ColBetCount" + columnIdSuffix });
                        }
                    }
                }
            }

            var leagueGrandTotal = league.GrandTotal;
            if (self.IsIncludeGroup(self.CacheData.GrandTotalColumnId)) {
                rowColumns.push({ Name: "ColWinLoss_GrandTotal", Value: reportManager.FormatNumberCell(leagueGrandTotal.WinLoss_Str, false, true, false) });
                rowColumns.push({ Name: "ColTurnover_GrandTotal", Value: leagueGrandTotal.Turnover_Str });
                rowColumns.push({ Name: "ColMargin_GrandTotal", Value: reportManager.FormatNumberCell(leagueGrandTotal.Margin_Str, false, true, false) });
                rowColumns.push({ Name: "ColBetCount_GrandTotal", Value: leagueGrandTotal.BetCount_Str });
            }

            bodyData.push({ Columns: rowColumns });
        }

        var grandTotal = report.GrandTotal;
        var yearLength = grandTotal.ChildsInfo.length;
        customizeHeaderColumns.push({ HeaderText: "League", Name: "ColLeague", MergeRowNumber: 2 });
        headerColumns.push({ HeaderText: "League", Width: 160, Name: "ColLeague", HeaderAlign: "center", CellAlign: "left" });

        rowColumns = [];
        rowColumns.push({ Name: "ColLeague", Value: grandTotal.Name, CssClass: "align-right" });

        for (var i = 0; i < yearLength; i++) {
            var year = grandTotal.ChildsInfo[i];
            var yearGrandTotal = year;
            var monthLength = year.ChildsInfo.length;
            columnIdSuffix = "_Year" + year.WinLossDate_Str;
            if (self.IsIncludeGroup(year.WinLossDate_Str)) {
                customizeHeaderColumns.push({
                    HeaderText: year.Name,
                    Name: "ColWinLoss" + columnIdSuffix,
                    MergeColNumber: 4
                });

                headerColumns.push({ HeaderText: "Win Loss", Width: 140, Name: "ColWinLoss" + columnIdSuffix, HeaderAlign: "center", CellAlign: "right" });
                headerColumns.push({ HeaderText: "Turnover", Width: 140, Name: "ColTurnover" + columnIdSuffix, HeaderAlign: "center", CellAlign: "right" });
                headerColumns.push({ HeaderText: "Margin", Width: 100, Name: "ColMargin" + columnIdSuffix, HeaderAlign: "center", CellAlign: "right" });
                headerColumns.push({ HeaderText: "Bet Count", Width: 100, Name: "ColBetCount" + columnIdSuffix, HeaderAlign: "center", CellAlign: "right" });

                rowColumns.push({ Name: "ColWinLoss" + columnIdSuffix, Value: reportManager.FormatNumberCell(yearGrandTotal.WinLoss_Str, false, true, false) });
                rowColumns.push({ Name: "ColTurnover" + columnIdSuffix, Value: yearGrandTotal.Turnover_Str });
                rowColumns.push({ Name: "ColMargin" + columnIdSuffix, Value: reportManager.FormatNumberCell(yearGrandTotal.Margin_Str, false, true, false) });
                rowColumns.push({ Name: "ColBetCount" + columnIdSuffix, Value: yearGrandTotal.BetCount_Str });
            }

            for (var j = 0; j < monthLength; j++) {
                var month = year.ChildsInfo[j];
                columnIdSuffix = "_Year" + year.WinLossDate_Str + "_Month" + month.WinLossDate_Str;
                if (self.IsIncludeGroup(month.WinLossDate_Str)) {
                    customizeHeaderColumns.push({
                        HeaderText: month.Name,
                        Name: "ColWinLoss" + columnIdSuffix,
                        MergeColNumber: 4
                    });

                    headerColumns.push({ HeaderText: "Win Loss", Width: 140, Name: "ColWinLoss" + columnIdSuffix, HeaderAlign: "center", CellAlign: "right" });
                    headerColumns.push({ HeaderText: "Turnover", Width: 140, Name: "ColTurnover" + columnIdSuffix, HeaderAlign: "center", CellAlign: "right" });
                    headerColumns.push({ HeaderText: "Margin", Width: 100, Name: "ColMargin" + columnIdSuffix, HeaderAlign: "center", CellAlign: "right" });
                    headerColumns.push({ HeaderText: "Bet Count", Width: 100, Name: "ColBetCount" + columnIdSuffix, HeaderAlign: "center", CellAlign: "right" });

                    rowColumns.push({ Name: "ColWinLoss" + columnIdSuffix, Value: reportManager.FormatNumberCell(month.WinLoss_Str, false, true, false) });
                    rowColumns.push({ Name: "ColTurnover" + columnIdSuffix, Value: month.Turnover_Str });
                    rowColumns.push({ Name: "ColMargin" + columnIdSuffix, Value: reportManager.FormatNumberCell(month.Margin_Str, false, true, false) });
                    rowColumns.push({ Name: "ColBetCount" + columnIdSuffix, Value: month.BetCount_Str });
                }
            }
        }

        var allTotal = grandTotal.GrandTotal;
        if (self.IsIncludeGroup(self.CacheData.GrandTotalColumnId)) {
            rowColumns.push({ Name: "ColWinLoss_GrandTotal", Value: reportManager.FormatNumberCell(allTotal.WinLoss_Str, false, true, false) });
            rowColumns.push({ Name: "ColTurnover_GrandTotal", Value: allTotal.Turnover_Str });
            rowColumns.push({ Name: "ColMargin_GrandTotal", Value: reportManager.FormatNumberCell(allTotal.Margin_Str, false, true, false) });
            rowColumns.push({ Name: "ColBetCount_GrandTotal", Value: allTotal.BetCount_Str });

            customizeHeaderColumns.push({
                HeaderText: grandTotal.Name,
                Name: "ColWinLoss_GrandTotal",
                MergeColNumber: 4
            });

            headerColumns.push({ HeaderText: "Win Loss", Width: 140, Name: "ColWinLoss_GrandTotal", HeaderAlign: "center", CellAlign: "right" });
            headerColumns.push({ HeaderText: "Turnover", Width: 140, Name: "ColTurnover_GrandTotal", HeaderAlign: "center", CellAlign: "right" });
            headerColumns.push({ HeaderText: "Margin", Width: 100, Name: "ColMargin_GrandTotal", HeaderAlign: "center", CellAlign: "right" });
            headerColumns.push({ HeaderText: "Bet Count", Width: 100, Name: "ColBetCount_GrandTotal", HeaderAlign: "center", CellAlign: "right" });
        }

        footerData.push({ Columns: rowColumns });
    }
    else {
        customizeHeaderColumns.push({ HeaderText: "League", Name: "ColLeague", MergeRowNumber: 2 });
        headerColumns.push({ HeaderText: "League", Width: 160, Name: "ColLeague", HeaderAlign: "center", CellAlign: "left" });
        columnIdSuffix = "_GrandTotal";
        customizeHeaderColumns.push({
            HeaderText: "Grand Total",
            Name: "ColWinLoss" + columnIdSuffix,
            MergeColNumber: 4
        });

        headerColumns.push({ HeaderText: "Win Loss", Width: 140, Name: "ColWinLoss" + columnIdSuffix, HeaderAlign: "center", CellAlign: "right" });
        headerColumns.push({ HeaderText: "Turnover", Width: 140, Name: "ColTurnover" + columnIdSuffix, HeaderAlign: "center", CellAlign: "right" });
        headerColumns.push({ HeaderText: "Margin", Width: 100, Name: "ColMargin" + columnIdSuffix, HeaderAlign: "center", CellAlign: "right" });
        headerColumns.push({ HeaderText: "Bet Count", Width: 100, Name: "ColBetCount" + columnIdSuffix, HeaderAlign: "center", CellAlign: "right" });
    }

    var backToMainEvent = "onclick='reportManager.TopPunter_ByLeague.ShowReport()'";
    var backToByUserReportEvent = "onclick='reportManager.TopPunter_ByLeague.ShowReportTopPunterByUser()'";
    var backToByUser_SportTypeReportEvent = "onclick='reportManager.TopPunter_ByLeague.ShowTopPunter_ByUser_SportType()'";
    var title = '';
    if (reportManager.TopPunterByUser.Params.reportParams.IsDirectFromFilter) {
        title = '<span class="link" ' + backToByUserReportEvent + '>Main</span> > ' +
                '<span class="link" ' + backToByUser_SportTypeReportEvent + '>' + self.Params.reportParams.SportName + '</span> > ' +
                '<span>' + self.Params.reportParams.BetTypeName + ' - ' + self.Params.reportParams.LiveTypeName + '</span>';
    }
    else {
        title = '<span class="link" ' + backToMainEvent + '>Main</span> > ' +
                '<span class="link" ' + backToByUserReportEvent + '>' + self.Params.reportParams.UserName + '</span> > ' +
                '<span class="link" ' + backToByUser_SportTypeReportEvent + '>' + self.Params.reportParams.SportName + '</span> > ' +
                '<span>' + self.Params.reportParams.BetTypeName + ' - ' + self.Params.reportParams.LiveTypeName + '</span>';
    }

    var gridOptions = {
        customizeHeader: true,
        groupHeaderRows: [{
            Columns: customizeHeaderColumns
        }],
        columns: headerColumns,
        showTitle: true,
        showFooter: true,
        titleInfo:
            {
                text: title,
                showExcelButton: true,
                exportExcelFunction: function () {
                    self.ExportReport();
                }
            },
        showTree: false,
        bodyRows: bodyData,
        footerRows: footerData,
        gridExpandHeight: 200,
        numberOfFrozenColumn: 1,
        gridWidth: 2000,
        showDistinctRow: false,
        showHighlightRow: false
    };

    reportManager.TopPunter_ByLeague.TopPunter_ByUser_SportType.HideReport();
    this.Tables.report.removeClass("display-none").fanexGrid(gridOptions);
    if (report && report.ChildsInfo.length > 0) {
        this.Tables.ddwColumnSettingContainer = $('<span id="spGroupColumns_ByLeague" class="display-none exeption-multiselect-click"></span>');
        this.Tables.ddwColumnSetting = $('<span id="ddwColumnSetting_ByLeague"></span>');
        this.Tables.columnSetting = $('<input type="button" class="columnSetting exeption-multiselect-click" title="Show/Hide columns" />');
        self.Tables.columnSetting.unbind("click");
        self.Tables.columnSetting.click(function (event) {
            self.SettingReportColumn();
            event.preventDefault();
            event.stopPropagation();
        });

        var toolBox = this.Tables.reportContainer.find(".grid-toolbox");
        toolBox.prepend(this.Tables.columnSetting);

        this.Tables.ddwColumnSettingContainer.empty().append(this.Tables.ddwColumnSetting);
        toolBox.prepend(this.Tables.ddwColumnSettingContainer);
        this.RenderColumnSettingDaily(report);
    }

    if (self.CacheData.IsRefreshReportView) {
        self.SettingReportColumn();
        self.CacheData.IsRefreshReportView = false;
    }
};
TopPunter_ByLeague_BetType.prototype.RenderReportNonDaily = function (report) {
    var self = this;

    this.Tables.reportContainer.removeClass("display-none");
    this.Tables.report.addClass("display-none");
    var bodyData = [];
    var rowColumns = [];
    var footerData = [];
    var customizeHeaderColumns = [];
    var headerColumns = [];
    var columnIdSuffix;
    if (report) {
        var leagueLength = report.ChildsInfo.length;
        for (var i = 0; i < leagueLength; i++) {
            var league = report.ChildsInfo[i];
            var leagueGrandTotal = league.GrandTotal;
            rowColumns = [];
            rowColumns.push({ Name: "ColLeague", Value: league.Name });
            var dateLength = league.ChildsInfo.length;
            if (self.IsIncludeGroup(self.CacheData.GrandTotalColumnId)) {
                switch (this.Params.reportParams.Reporttype) {
                    case reportManager.Enum.PeriodType.Annualy:
                        rowColumns.push({ Name: "ColWinLoss_GrandTotal", Value: reportManager.FormatNumberCell(leagueGrandTotal.WinLoss_Str, false, true, false) });
                        rowColumns.push({ Name: "ColTurnover_GrandTotal", Value: leagueGrandTotal.Turnover_Str });
                        rowColumns.push({ Name: "ColMargin_GrandTotal", Value: reportManager.FormatNumberCell(leagueGrandTotal.Margin_Str, false, true, false) });
                        rowColumns.push({ Name: "ColBetCount_GrandTotal", Value: leagueGrandTotal.BetCount_Str });
                        //bodyData.push({ Columns: rowColumns });
                        break;
                    default:
                        break;
                }
            }

            for (var j = 0; j < dateLength; j++) {
                var date = league.ChildsInfo[j];
                if (self.IsIncludeGroup(report.GrandTotal.ChildsInfo[j].WinLossDate_Str)) {
                    columnIdSuffix = "_Date" + report.GrandTotal.ChildsInfo[j].WinLossDate_Str;
                    if (date) {
                        rowColumns.push({ Name: "ColWinLoss" + columnIdSuffix, Value: reportManager.FormatNumberCell(date.WinLoss_Str, false, true, false) });
                        rowColumns.push({ Name: "ColTurnover" + columnIdSuffix, Value: date.Turnover_Str });
                        rowColumns.push({ Name: "ColMargin" + columnIdSuffix, Value: reportManager.FormatNumberCell(date.Margin_Str, false, true, false) });
                        rowColumns.push({ Name: "ColBetCount" + columnIdSuffix, Value: date.BetCount_Str });
                    } else {
                        rowColumns.push({ Name: "ColWinLoss" + columnIdSuffix });
                        rowColumns.push({ Name: "ColTurnover" + columnIdSuffix });
                        rowColumns.push({ Name: "ColMargin" + columnIdSuffix });
                        rowColumns.push({ Name: "ColBetCount" + columnIdSuffix });
                    }
                }
            }

            if (self.IsIncludeGroup(self.CacheData.GrandTotalColumnId)) {
                switch (this.Params.reportParams.Reporttype) {
                    case reportManager.Enum.PeriodType.Daily:
                    case reportManager.Enum.PeriodType.Weekly:
                    case reportManager.Enum.PeriodType.MonthRange:
                        rowColumns.push({ Name: "ColWinLoss_GrandTotal", Value: reportManager.FormatNumberCell(leagueGrandTotal.WinLoss_Str, false, true, false) });
                        rowColumns.push({ Name: "ColTurnover_GrandTotal", Value: leagueGrandTotal.Turnover_Str });
                        rowColumns.push({ Name: "ColMargin_GrandTotal", Value: reportManager.FormatNumberCell(leagueGrandTotal.Margin_Str, false, true, false) });
                        rowColumns.push({ Name: "ColBetCount_GrandTotal", Value: leagueGrandTotal.BetCount_Str });
                        break;
                    default:
                        break;
                }
            }

            bodyData.push({ Columns: rowColumns });
        }

        var grandTotal = report.GrandTotal;
        var dateLength = grandTotal.ChildsInfo.length;
        rowColumns = [];
        customizeHeaderColumns.push({ HeaderText: "League", Name: "ColLeague", MergeRowNumber: 2 });
        headerColumns.push({ HeaderText: "League", Width: 160, Name: "ColLeague", HeaderAlign: "center", CellAlign: "left" });
        rowColumns.push({ Name: "ColLeague", Value: grandTotal.Name, CssClass: "align-right" });
        if (self.IsIncludeGroup(self.CacheData.GrandTotalColumnId)) {
            columnIdSuffix = "_GrandTotal";
            switch (this.Params.reportParams.Reporttype) {
                case reportManager.Enum.PeriodType.Annualy:
                    customizeHeaderColumns.push({
                        HeaderText: grandTotal.Name,
                        Name: "ColWinLoss" + columnIdSuffix,
                        MergeColNumber: 4
                    });

                    headerColumns.push({ HeaderText: "Win Loss", Width: 140, Name: "ColWinLoss" + columnIdSuffix, HeaderAlign: "center", CellAlign: "right" });
                    headerColumns.push({ HeaderText: "Turnover", Width: 140, Name: "ColTurnover" + columnIdSuffix, HeaderAlign: "center", CellAlign: "right" });
                    headerColumns.push({ HeaderText: "Margin", Width: 100, Name: "ColMargin" + columnIdSuffix, HeaderAlign: "center", CellAlign: "right" });
                    headerColumns.push({ HeaderText: "Bet Count", Width: 100, Name: "ColBetCount" + columnIdSuffix, HeaderAlign: "center", CellAlign: "right" });

                    rowColumns.push({ Name: "ColWinLoss" + columnIdSuffix, Value: reportManager.FormatNumberCell(grandTotal.GrandTotal.WinLoss_Str, false, true, false) });
                    rowColumns.push({ Name: "ColTurnover" + columnIdSuffix, Value: grandTotal.GrandTotal.Turnover_Str });
                    rowColumns.push({ Name: "ColMargin" + columnIdSuffix, Value: reportManager.FormatNumberCell(grandTotal.GrandTotal.Margin_Str, false, true, false) });
                    rowColumns.push({ Name: "ColBetCount" + columnIdSuffix, Value: grandTotal.GrandTotal.BetCount_Str });
                    break;
                default:
                    break;
            }
        }

        for (var i = 0; i < dateLength; i++) {
            var date = grandTotal.ChildsInfo[i];
            if (self.IsIncludeGroup(date.WinLossDate_Str)) {
                columnIdSuffix = "_Date" + date.WinLossDate_Str;
                var headerText = '';
                if (this.Params.reportParams.Reporttype == reportManager.Enum.PeriodType.MonthRange) {
                    var clickEvent = 'reportManager.TopPunter_ByLeague.TopPunter_ByUser_BetType.GetReportFromMonthRange(' + date.Id + ')';
                    headerText = '<div class="link" onclick="' + clickEvent + '">' + date.Name + '</div>';
                } else {
                    headerText = date.Name;
                }

                customizeHeaderColumns.push({
                    HeaderText: headerText,
                    Name: "ColWinLoss" + columnIdSuffix,
                    MergeColNumber: 4
                });

                headerColumns.push({ HeaderText: "Win Loss", Width: 140, Name: "ColWinLoss" + columnIdSuffix, HeaderAlign: "center", CellAlign: "right" });
                headerColumns.push({ HeaderText: "Turnover", Width: 140, Name: "ColTurnover" + columnIdSuffix, HeaderAlign: "center", CellAlign: "right" });
                headerColumns.push({ HeaderText: "Margin", Width: 100, Name: "ColMargin" + columnIdSuffix, HeaderAlign: "center", CellAlign: "right" });
                headerColumns.push({ HeaderText: "Bet Count", Width: 100, Name: "ColBetCount" + columnIdSuffix, HeaderAlign: "center", CellAlign: "right" });

                rowColumns.push({ Name: "ColWinLoss" + columnIdSuffix, Value: reportManager.FormatNumberCell(date.WinLoss_Str, false, true, false) });
                rowColumns.push({ Name: "ColTurnover" + columnIdSuffix, Value: date.Turnover_Str });
                rowColumns.push({ Name: "ColMargin" + columnIdSuffix, Value: reportManager.FormatNumberCell(date.Margin_Str, false, true, false) });
                rowColumns.push({ Name: "ColBetCount" + columnIdSuffix, Value: date.BetCount_Str });
            }
        }

        if (self.IsIncludeGroup(self.CacheData.GrandTotalColumnId)) {
            columnIdSuffix = "_GrandTotal";
            switch (this.Params.reportParams.Reporttype) {
                case reportManager.Enum.PeriodType.Daily:
                case reportManager.Enum.PeriodType.Weekly:
                case reportManager.Enum.PeriodType.MonthRange:
                    customizeHeaderColumns.push({
                        HeaderText: grandTotal.Name,
                        Name: "ColWinLoss" + columnIdSuffix,
                        MergeColNumber: 4
                    });

                    headerColumns.push({ HeaderText: "Win Loss", Width: 140, Name: "ColWinLoss" + columnIdSuffix, HeaderAlign: "center", CellAlign: "right" });
                    headerColumns.push({ HeaderText: "Turnover", Width: 140, Name: "ColTurnover" + columnIdSuffix, HeaderAlign: "center", CellAlign: "right" });
                    headerColumns.push({ HeaderText: "Margin", Width: 100, Name: "ColMargin" + columnIdSuffix, HeaderAlign: "center", CellAlign: "right" });
                    headerColumns.push({ HeaderText: "Bet Count", Width: 100, Name: "ColBetCount" + columnIdSuffix, HeaderAlign: "center", CellAlign: "right" });

                    rowColumns.push({ Name: "ColWinLoss" + columnIdSuffix, Value: reportManager.FormatNumberCell(grandTotal.GrandTotal.WinLoss_Str, false, true, false) });
                    rowColumns.push({ Name: "ColTurnover" + columnIdSuffix, Value: grandTotal.GrandTotal.Turnover_Str });
                    rowColumns.push({ Name: "ColMargin" + columnIdSuffix, Value: reportManager.FormatNumberCell(grandTotal.GrandTotal.Margin_Str, false, true, false) });
                    rowColumns.push({ Name: "ColBetCount" + columnIdSuffix, Value: grandTotal.GrandTotal.BetCount_Str });
                    break;
                default:
                    break;
            }
        }

        footerData.push({ Columns: rowColumns });
    }
    else {
        customizeHeaderColumns.push({ HeaderText: "League", Name: "ColLeague", MergeRowNumber: 2 });
        headerColumns.push({ HeaderText: "League", Width: 160, Name: "ColLeague", HeaderAlign: "center", CellAlign: "left" });
        columnIdSuffix = "_GrandTotal";
        customizeHeaderColumns.push({
            HeaderText: "Grand Total",
            Name: "ColWinLoss" + columnIdSuffix,
            MergeColNumber: 4
        });

        headerColumns.push({ HeaderText: "Win Loss", Width: 140, Name: "ColWinLoss" + columnIdSuffix, HeaderAlign: "center", CellAlign: "right" });
        headerColumns.push({ HeaderText: "Turnover", Width: 140, Name: "ColTurnover" + columnIdSuffix, HeaderAlign: "center", CellAlign: "right" });
        headerColumns.push({ HeaderText: "Margin", Width: 100, Name: "ColMargin" + columnIdSuffix, HeaderAlign: "center", CellAlign: "right" });
        headerColumns.push({ HeaderText: "Bet Count", Width: 100, Name: "ColBetCount" + columnIdSuffix, HeaderAlign: "center", CellAlign: "right" });
    }

    var backToMainEvent = "onclick='reportManager.TopPunter_ByLeague.ShowReport()'";
    var backToByUserReportEvent = "onclick='reportManager.TopPunter_ByLeague.ShowReportTopPunterByUser()'";
    var backToByUser_SportTypeReportEvent = "onclick='reportManager.TopPunter_ByLeague.ShowTopPunter_ByUser_SportType()'";
    var title = '';
    if (reportManager.TopPunterByUser.Params.reportParams.IsDirectFromFilter) {
        title = '<span class="link" ' + backToByUserReportEvent + '>Main</span> > ' +
                '<span class="link" ' + backToByUser_SportTypeReportEvent + '>' + self.Params.reportParams.SportName + '</span> > ' +
                '<span>' + self.Params.reportParams.BetTypeName + ' - ' + self.Params.reportParams.LiveTypeName + '</span>';
    }
    else {
        title = '<span class="link" ' + backToMainEvent + '>Main</span> > ' +
                '<span class="link" ' + backToByUserReportEvent + '>' + self.Params.reportParams.UserName + '</span> > ' +
                '<span class="link" ' + backToByUser_SportTypeReportEvent + '>' + self.Params.reportParams.SportName + '</span> > ' +
                '<span>' + self.Params.reportParams.BetTypeName + ' - ' + self.Params.reportParams.LiveTypeName + '</span>';
    }

    var gridOptions = {
        customizeHeader: true,
        groupHeaderRows: [{
            Columns: customizeHeaderColumns
        }],
        columns: headerColumns,
        showTitle: true,
        showFooter: true,
        titleInfo:
            {
                text: title,
                showExcelButton: true,
                exportExcelFunction: function () {
                    self.ExportReport();
                }
            },
        showTree: false,
        bodyRows: bodyData,
        footerRows: footerData,
        gridExpandHeight: 300 + self.CacheData.summaryReportHeight,
        numberOfFrozenColumn: 1,
        gridWidth: 2000,
        showDistinctRow: false,
        showHighlightRow: false
    };

    reportManager.TopPunter_ByLeague.TopPunter_ByUser_SportType.HideReport();
    this.Tables.report.removeClass("display-none").fanexGrid(gridOptions);
    if (report) {
        this.Tables.ddwColumnSettingContainer = $('<span id="spGroupColumns_ByLeague" class="display-none exeption-multiselect-click"></span>');
        this.Tables.ddwColumnSetting = $('<span id="ddwColumnSetting_ByLeague"></span>');
        this.Tables.columnSetting = $('<input type="button" class="columnSetting exeption-multiselect-click" title="Show/Hide columns" />');
        self.Tables.columnSetting.unbind("click");
        self.Tables.columnSetting.click(function (event) {
            self.SettingReportColumn();
            event.preventDefault();
            event.stopPropagation();
        });

        var toolBox = this.Tables.reportContainer.find(".grid-toolbox");
        toolBox.prepend(this.Tables.columnSetting);

        this.Tables.ddwColumnSettingContainer.empty().append(this.Tables.ddwColumnSetting);
        toolBox.prepend(this.Tables.ddwColumnSettingContainer);
        this.RenderColumnSettingNonDaily(report);
    }

    if (self.CacheData.IsRefreshReportView) {
        self.SettingReportColumn();
        self.CacheData.IsRefreshReportView = false;
    }
};
TopPunter_ByLeague_BetType.prototype.RenderSummaryReport = function (report) {
    var self = this;
    this.Tables.summaryReportTitle.html(self.Params.reportParams.BetTypeName + ' - ' +
     self.Params.reportParams.LiveTypeName);
    this.Tables.summaryReport.addClass("display-none");
    var arrBody = [];
    var html = '';
    if (report && report.GrandTotal.ChildsInfo.length > 0) {
        var yearLength = report.GrandTotal.ChildsInfo.length;
        for (var i = 0; i < yearLength; i++) {
            var year = report.GrandTotal.ChildsInfo[i];
            if (year) {
                html = '<tr>\
                             <td>\
                                <div class="col100 align-center">' + year.Id + '</div>\
                            </td>\
                             <td>\
                                <div class="col100 align-right">' + reportManager.FormatNumberCell(year.WinLoss_Str, false, true, false) + '</div>\
                            </td>\
                             <td>\
                                <div class="col100 align-right">' + year.Turnover_Str + '</div>\
                            </td>\
                             <td>\
                                <div class="col100 align-right">' + reportManager.FormatNumberCell(year.Margin_Str, false, true, false) + '</div>\
                            </td>\
                             <td>\
                                <div class="col100 align-right">' + year.BetCount_Str + '</div>\
                            </td>\
                        </tr>';
                arrBody.push(html);
            }
        }
    } else {
        arrBody.push('<tr><td colspan="5" class="no-data-not-frozen c">There is no information available.</td></tr>');
    }

    var bodyHtml = arrBody.join('');
    bodyHtml = fixBugsBrowser.removeSpaceIE9(bodyHtml);
    this.Tables.summaryReportBody.empty();
    this.Tables.summaryReportBody.append(bodyHtml);

    reportManager.TopPunter_ByLeague.TopPunter_ByUser_SportType.HideReport();
    this.Tables.summaryReport.removeClass("display-none");
    this.Tables.summaryReportContainer.removeClass("display-none");
    this.CacheData.summaryReportHeight = this.Tables.summaryReportContainer.height();
};
TopPunter_ByLeague_BetType.prototype.ShowReport = function () {
    this.Tables.report.removeClass("display-none");
    this.Tables.reportContainer.removeClass("display-none");
};
TopPunter_ByLeague_BetType.prototype.HideReport = function () {
    this.Tables.reportContainer.addClass("display-none");
    this.Tables.report.addClass("display-none");
    this.Tables.summaryReportContainer.addClass("display-none");
    this.Tables.summaryReport.addClass("display-none");
};
TopPunter_ByLeague_BetType.prototype.GetReportFromMonthRange = function (year) {
    var fromDate = '1/1/' + year;
    if (new Date(fromDate) < new Date(this.Params.reportParams.Fromdate)) {
        fromDate = this.Params.reportParams.Fromdate;
    }

    var toDate = '12/31/' + year;
    if (new Date(this.Params.reportParams.Todate) < new Date(toDate)) {
        toDate = this.Params.reportParams.Todate;
    }

    this.GetReport({
        CustID: this.Params.reportParams.CustID,
        UserName: this.Params.reportParams.UserName,
        LeagueID: null,
        Sporttype: this.Params.reportParams.Sporttype,
        SportName: this.Params.reportParams.SportName,
        Bettype: this.Params.reportParams.Bettype,
        BetTypeName: this.Params.reportParams.BetTypeName,
        Fromdate: fromDate,
        Todate: toDate,
        OrderBy: this.Params.reportParams.OrderBy,
        OrderType: this.Params.reportParams.OrderType,
        Reporttype: reportManager.Enum.PeriodType.Annualy,
        Status: this.Params.reportParams.Status,
        LiveTypeName: this.Params.reportParams.LiveTypeName
    });
};
TopPunter_ByLeague_BetType.prototype.ExportReport = function () {
    AccReport.downloadFile(window.rootUrl + "Special/PunterAnalysis/ExportTopPunter_ByUser_BetType", this.Params.reportParams);
};
TopPunter_ByLeague_BetType.prototype.RenderColumnSettingDaily = function (report) {
    var self = this;

    var columnGroups = [];
    var grandTotal = report.GrandTotal;
    var yearLength = grandTotal.ChildsInfo.length;
    for (var i = 0; i < yearLength; i++) {
        var year = grandTotal.ChildsInfo[i];
        var yearGrandTotal = year;
        var monthLength = year.ChildsInfo.length;
        columnGroups.push({
            Id: year.WinLossDate_Str,
            Name: year.Name
        });

        for (var j = 0; j < monthLength; j++) {
            var month = year.ChildsInfo[j];
            columnGroups.push({
                Id: month.WinLossDate_Str,
                Name: month.Name
            });
        }
    }

    columnGroups.push({
        Id: self.CacheData.GrandTotalColumnId,
        Name: grandTotal.Name
    });

    var ddwColumnSetting = [];
    var ddwId = "ddwColumnSetting_ByLeague";
    var columnGroupsLength = columnGroups ? columnGroups.length : 0;
    for (var i = 0; i < columnGroupsLength; i++) {
        var columnGroup = columnGroups[i];
        ddwColumnSetting.push({
            Id: "#" + ddwId + "_Group_" + columnGroup.Id,
            Value: columnGroup.Id,
            Text: columnGroup.Name,
            Checked: self.IsIncludeGroup(columnGroup.Id),
            Enable: true,
            Items: []
        });
    }

    $("#" + ddwId).multiSelect({
        minWidth: 190,
        maxHeight: 350,
        shadow: true,
        showHeader: true,
        isCheckAll: self.CacheData.SelectedColumnGroups == null ||
                    self.CacheData.SelectedColumnGroups.length == columnGroupsLength,
        showFooter: false,
        checkAllText: 'All',
        isUsedCustomCheckAllText: true,
        customCheckAllText: 'All',
        noneSelectedText: 'Select an option',
        noItemText: 'No group found.',
        listData: ddwColumnSetting,
        onCheck: function () {
            self.UpdateColumnSetting();
        }
    });

    this.Tables.ddwColumnSetting = $("#ddwColumnSetting_ByLeague");
};
TopPunter_ByLeague_BetType.prototype.RenderColumnSettingNonDaily = function (report) {
    var self = this;

    var columnGroups = [];
    var grandTotal = report.GrandTotal;
    var dateLength = grandTotal.ChildsInfo.length;
    switch (this.Params.reportParams.Reporttype) {
        case reportManager.Enum.PeriodType.Annualy:
            columnGroups.push({
                Id: self.CacheData.GrandTotalColumnId,
                Name: grandTotal.WinLossDate_Str
            });

            break;
        default:
            break;
    }

    for (var i = 0; i < dateLength; i++) {
        var date = grandTotal.ChildsInfo[i];
        columnGroups.push({
            Id: date.WinLossDate_Str,
            Name: date.Name
        });
    }

    switch (this.Params.reportParams.Reporttype) {
        case reportManager.Enum.PeriodType.Daily:
        case reportManager.Enum.PeriodType.Weekly:
        case reportManager.Enum.PeriodType.MonthRange:
            columnGroups.push({
                Id: self.CacheData.GrandTotalColumnId,
                Name: grandTotal.WinLossDate_Str
            });
            break;
        default:
            break;
    }

    var ddwColumnSetting = [];
    var ddwId = "ddwColumnSetting_ByLeague";
    var columnGroupsLength = columnGroups ? columnGroups.length : 0;
    for (var i = 0; i < columnGroupsLength; i++) {
        var columnGroup = columnGroups[i];
        ddwColumnSetting.push({
            Id: "#" + ddwId + "_Group_" + columnGroup.Id,
            Value: columnGroup.Id,
            Text: columnGroup.Name,
            Checked: self.IsIncludeGroup(columnGroup.Id),
            Enable: true,
            Items: []
        });
    }

    $("#" + ddwId).multiSelect({
        minWidth: 190,
        maxHeight: 350,
        shadow: true,
        showHeader: true,
        isCheckAll: self.CacheData.SelectedColumnGroups == null ||
                    self.CacheData.SelectedColumnGroups.length == columnGroupsLength,
        showFooter: false,
        checkAllText: 'All',
        isUsedCustomCheckAllText: true,
        customCheckAllText: 'All',
        noneSelectedText: 'Select an option',
        noItemText: 'No group found.',
        listData: ddwColumnSetting,
        onCheck: function () {
            self.UpdateColumnSetting();
        }
    });

    this.Tables.ddwColumnSetting = $("#ddwColumnSetting_ByLeague");
};
TopPunter_ByLeague_BetType.prototype.UpdateColumnSetting = function () {
    var selectedGroups = this.Tables.ddwColumnSetting.multiSelect('getCheckedItems');
    if (selectedGroups.length == 0) {
        AccReport.popUp.show("Please choose at least one column!", "errMsg");
        return;
    }

    AccReport.popUp.removeHighLight();
    this.CacheData.SelectedColumnGroups = selectedGroups;
    this.CacheData.IsRefreshReportView = true;
    this.RenderReport(this.CacheData.report);
};
TopPunter_ByLeague_BetType.prototype.IsIncludeGroup = function (groupId) {
    return this.CacheData.SelectedColumnGroups == null ||
           $.inArray(groupId, this.CacheData.SelectedColumnGroups) != -1;
};