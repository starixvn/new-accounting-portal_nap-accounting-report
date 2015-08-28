
Namespace.Register("Fanex.AccountingReports.Misc.MAMonitorTool");
Fanex.AccountingReports.Misc.MAMonitorTool.BaseReport = function () {
    var self = this;
    self.Tables = {
        reportContainer: null,
        report: null
    };

    self.InitPage = function () {
        self.CachedDOMElements();
    };

    self.CachedDOMElements = function () {
        self.Tables.reportContainer = $("#mainMAMonitorToolReportContainer");
        self.Tables.report = $("#mainMAMonitorToolReport");
    };

    self.RenderReport = function (report) {
        self.Tables.reportContainer.removeClass("display-none");
        self.Tables.report.addClass("display-none");
        var bodyData = [];
        var footerData = [];
        if (report && report.ChildsInfo && report.ChildsInfo.length > 0) {
            var reportLength = report.ChildsInfo.length;
            var info;
            var columns = [];
            for (var i = 0; i < reportLength; i++) {
                info = report.ChildsInfo[i];
                bodyData.push(self.GetGridRow(info, false));
            }

            info = report.GrandTotal;
            footerData.push(self.GetGridRow(info, true));
        }

        function ReRenderAfterPaged(page_index, itemsPerPage, jq) {
            reportManager.Params.getReportParams.IsPagingCallBack = true;
            reportManager.Params.getReportParams.StartIndex = page_index >= 0 ? (page_index * itemsPerPage) : 0;
            reportManager.Params.getReportParams.UsersPerPage = itemsPerPage;

            reportManager.PagingOption.itemsPerPage = itemsPerPage;
            reportManager.PagingOption.currentPage = page_index;
            reportManager.PagingOption.startIndex = reportManager.Params.getReportParams.StartIndex;

            reportManager.buttonGenerateClick();
            self.Tables.report.removeClass("display-none").fanexGrid(gridOptions);
            return false;
        }

        var gridOptions = {
            customizeHeader: true,
            groupHeaderRows: [{
                Columns: [
                    { HeaderText: self.GetColNameTitle(), Name: "ColName", MergeRowNumber: 2 },
                    { HeaderText: viewResources.Member, Name: "ColMemberTurnover", MergeColNumber: 3 },
                    { HeaderText: viewResources.Agent, Name: "ColAgentTurnover", MergeColNumber: 2 },
                    { HeaderText: viewResources.Master, Name: "ColMasterTurnover", MergeColNumber: 2 },
                    { HeaderText: viewResources.Super, Name: "ColSuperTurnover", MergeColNumber: 2}]
            }],
            columns: [{ HeaderText: self.GetColNameTitle(), Width: 290, Name: "ColName", HeaderAlign: "center", CellAlign: "left" },
                { HeaderText: viewResources.Turnover, Width: 120, Name: "ColMemberTurnover", HeaderAlign: "center", CellAlign: "right", FormatNumber: FormatConstant.TWO_DECIMAL_NUMBER, FormatNegative: '$' },
                { HeaderText: viewResources.Win_Loss, Width: 120, Name: "ColMemberWinLoss", HeaderAlign: "center", CellAlign: "right", FormatNumber: FormatConstant.TWO_DECIMAL_NUMBER, FormatNegative: '$' },
                { HeaderText: viewResources.Bet_Count, Width: 120, Name: "ColMemberBetCount", HeaderAlign: "center", CellAlign: "right", FormatNumber: FormatConstant.INT_NUMBER },
                { HeaderText: viewResources.Turnover, Width: 120, Name: "ColAgentTurnover", HeaderAlign: "center", CellAlign: "right", FormatNumber: FormatConstant.TWO_DECIMAL_NUMBER, FormatNegative: '$' },
                { HeaderText: viewResources.Win_Loss, Width: 120, Name: "ColAgentWinLoss", HeaderAlign: "center", CellAlign: "right", FormatNumber: FormatConstant.TWO_DECIMAL_NUMBER, FormatNegative: '$' },
                { HeaderText: viewResources.Turnover, Width: 120, Name: "ColMasterTurnover", HeaderAlign: "center", CellAlign: "right", FormatNumber: FormatConstant.TWO_DECIMAL_NUMBER, FormatNegative: '$' },
                { HeaderText: viewResources.Win_Loss, Width: 120, Name: "ColMasterWinLoss", HeaderAlign: "center", CellAlign: "right", FormatNumber: FormatConstant.TWO_DECIMAL_NUMBER, FormatNegative: '$' },
                { HeaderText: viewResources.Turnover, Width: 120, Name: "ColSuperTurnover", HeaderAlign: "center", CellAlign: "right", FormatNumber: FormatConstant.TWO_DECIMAL_NUMBER, FormatNegative: '$' },
                { HeaderText: viewResources.Win_Loss, Width: 120, Name: "ColSuperWinLoss", HeaderAlign: "center", CellAlign: "right", FormatNumber: FormatConstant.TWO_DECIMAL_NUMBER, FormatNegative: '$'}],
            showTitle: true,
            showFooter: true,
            titleInfo: {
                text: AccReport.utility.StringFormat(viewResources.MAMonitorTool_ReportTitle,
                    [reportManager.Params.getReportParams.FromDate, reportManager.Params.getReportParams.ToDate]),
                showExcelButton: true,
                exportExcelFunction: function () {
                    self.ExportReport();
                }
            },
            bodyRows: bodyData,
            footerRows: footerData,
            numberOfFrozenColumn: 0,
            gridWidth: 1400,
            gridExpandHeight: 350,
            gridWidth: 1400,
            gridExpandHeight: 350,
            fullWidth: true,
            fullHeight: true,
            showDistinctRow: true,
            showHighlightRow: false,
            showPager: true,
            customPager: true,
            pagerOption: {
                itemsPerPage: reportManager.PagingOption.itemsPerPage,
                currentPage: reportManager.PagingOption.currentPage,
                itemsPerPageArray: reportManager.CachedData.itemsPerPageArray,
                totalItem: report.NumberUsers,
                pagerSelectCallBack: ReRenderAfterPaged
            }
        }

        self.Tables.report.removeClass("display-none").fanexGrid(gridOptions);
    };
    self.GetGridRow = function (winLossInfo, isGrandTotalRow) {
        var downlinesWinLoss = reportManager.GetDownlinesWinLoss(winLossInfo);
        var colNameValue = isGrandTotalRow ?
            winLossInfo.Name :
            self.GetColNameValue(parseInt(reportManager.Params.getReportParams.ReportBy), winLossInfo);

        return {
            Columns: [
                {
                    Name: "ColName",
                    Value: colNameValue,
                    CssClass: isGrandTotalRow ? 'align-center' : ''
                },
                { Name: "ColMemberTurnover", Value: downlinesWinLoss.memberWinLoss.Turnover },
                { Name: "ColMemberWinLoss", Value: downlinesWinLoss.memberWinLoss.WinLoss },
                { Name: "ColMemberBetCount", Value: downlinesWinLoss.memberWinLoss.BetCount },
                { Name: "ColAgentTurnover", Value: downlinesWinLoss.agentWinLoss.Turnover },
                { Name: "ColAgentWinLoss", Value: downlinesWinLoss.agentWinLoss.WinLoss },
                { Name: "ColMasterTurnover", Value: downlinesWinLoss.masterWinLoss.Turnover },
                { Name: "ColMasterWinLoss", Value: downlinesWinLoss.masterWinLoss.WinLoss },
                { Name: "ColSuperTurnover", Value: downlinesWinLoss.superWinLoss.Turnover },
                { Name: "ColSuperWinLoss", Value: downlinesWinLoss.superWinLoss.WinLoss}]
        };
    };
    self.GetColNameValue = function (reportBy, winLossInfo) {
        var matchId = winLossInfo.MatchId;
        var colNameValue = '';
        if (reportBy != reportManager.Enums.ReportBy.Game ||
            matchId == reportManager.Enums.Sport.HorseRacing ||
            matchId == reportManager.Enums.Sport.NumberGame ||
            matchId == reportManager.Enums.Sport.LiveCasino ||
            matchId == reportManager.Enums.BetType.P2PGames ||
            matchId == reportManager.Enums.BetType.Outright ||
            matchId == reportManager.Enums.BetType.MixParlay ||
            matchId == reportManager.Enums.BetType.SystemParlay ||
            (winLossInfo.HomeName == '' && winLossInfo.AwayName == '')) {
            colNameValue = winLossInfo.Name;
        }
        else {
            colNameValue =
                '<table border="0" cellspacing="0" cellpadding="0" class="table-noneborder">\
                        <tr>\
                            <td class="col80 colmatchcode">' + winLossInfo.MatchCode + '</td>\
                            <td class="col90 align-right hometeam">' + winLossInfo.HomeName + '</td>\
                            <td class="col30 align-center">' + viewResources.Vs + '</td>\
                            <td class="col90 align-left awayteam">' + winLossInfo.AwayName + '</td>\
                        </tr>\
                    </table>';
        }

        return colNameValue;
    };

    self.GetColNameTitle = function () {
        var reportType = parseInt(reportManager.Params.getReportParams.ReportBy);
        var reportTypeEnum = reportManager.Enums.ReportBy;
        var title = '';
        switch (reportType) {
            case reportTypeEnum.Daily:
                title = viewResources.Date;
                break;
            case reportTypeEnum.Monthly:
                title = viewResources.Month;
                break;
            case reportTypeEnum.Username:
                title = viewResources.UserName;
                break;
            case reportTypeEnum.BetType:
                title = viewResources.Bet_Type;
                break;
            case reportTypeEnum.League:
                title = viewResources.League;
                break;
            case reportTypeEnum.Game:
                title = viewResources.Game;
                break;
            case reportTypeEnum.Currency:
                title = viewResources.Currency;
                break;
            case reportTypeEnum.Sport:
                title = viewResources.Sport;
                break;
            default:
                title = viewResources.Bet_Type;
                break;
        }

        return title;
    };
    self.HideReport = function () {
        self.Tables.reportContainer.addClass("display-none");
    };
    self.ExportReport = function () {
        var self = this;        
        var exportParams = reportManager.Params.getReportParams;
        AccReport.downloadFile(window.rootUrl + "Misc/MAMonitorTool/ExportReport", exportParams);
    };
};