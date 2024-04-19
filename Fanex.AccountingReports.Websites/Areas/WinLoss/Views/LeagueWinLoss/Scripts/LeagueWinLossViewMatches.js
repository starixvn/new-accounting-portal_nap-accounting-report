Namespace.Register("Fanex.AccountingReports.WinLoss.LeagueWinLoss.LeagueWinLossViewMatches ");
Fanex.AccountingReports.WinLoss.LeagueWinLoss.LeagueWinLossViewMatches = function () {
    var self = this;
    var _tables = {
        reportContainer: null,
        report: null
    };

    self.Params = {
        reportParams: {
            StartDate: null,
            EndDate: null,
            SportBettype: null,
            LeagueName: '',
            SportName: '',
            IsNormalMode: true,
            IsNoDraw: false,
            SiteName: '',
            LiveIndicator: 0,
            ShowHedgeOrNormalMatchOnly: 0,
            BetType: 0,
            GlobalShowTime: '',
            SportType: 0,
            LeagueId: '',
            PermissionProfileID: permissionProfileID
        }
    };
    self.initPage = function () {
        cachedDOMElements();
    };
    var cachedDOMElements = function () {
        _tables.reportParentContainer = $("#parentContainer");
        _tables.reportContainer = $('#leagueWinLossReportMatchesContainer');
        _tables.report = $('#leagueWinLossReportMatches');
    };
    self.getReport = function (params) {
        AccReport.disableInput();
        getParams(params);
        const api = self.Params.reportParams.IsNormalMode ? "WinLoss/LeagueWinLoss/GetMatchesWithChoice" : "WinLoss/LeagueWinLoss/GetMatches";
        $.ajax({
            type: "POST",
            url: window.rootUrl + api,
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

    var showMatches = function (info, bodyData) {
        currentRow =
        {
            Columns: [{
                Name: "ColEvent", Value: {
                    startDate: self.Params.reportParams.StartDate,
                    sportType: self.Params.reportParams.SportType,
                    sportName: self.Params.reportParams.SportName,
                    isHedge: info.Total.IsHedge,
                    leagueId: self.Params.reportParams.LeagueId,
                    leagueName: self.Params.reportParams.LeagueName,
                    matchId: info.Total.MatchId,
                    matchCode: info.Total.MatchCode,
                    event: info.Total.MatchEvent,
                    isOutright: info.Total.IsOutright
                },
                CustomCellRender: function (currentCell) {
                    var openScoreInfoEvent = "scoreInfoManager.GetReport('" + currentCell.Value.startDate + "'," + //*
                        currentCell.Value.sportType + ",'" +
                        currentCell.Value.sportName + "','" + //*
                        0 + "','" + //*
                        null + "'," + //*
                        null + ",'" + //*
                        currentCell.Value.isHedge + "','" +
                        currentCell.Value.leagueId + "','" +
                        currentCell.Value.leagueName + "'," + //*
                        currentCell.Value.matchId + ",'" + //*
                        currentCell.Value.matchCode + "','" +
                        null + "','" +
                        null + "','" + //*
                        null + "','" +
                        null + "');"

                    if (currentCell.Value.isOutright) {
                        return '<span>' + currentCell.Value.event + '<span>';
                    } else {
                        return '<span class="link" onclick="event.stopPropagation();' + openScoreInfoEvent + '">' + currentCell.Value.event + '<span>';
                    }
                }
            },
            { Name: "ColMemberWinLoss", Value: info.Total.MemberWinLoss },
            { Name: "ColSMAWinLoss", Value: info.Total.SMAWinLoss },
            { Name: "ColExtraPT", Value: info.Total.ExtraPT },
            { Name: "ColCommision", Value: info.Total.Commission },
            { Name: "ColVariance", Value: info.Total.VarianceIncome },
            { Name: "ColVirtualWinLoss", Value: info.Total.VirtualWinLoss },
            { Name: "ColRealIncome", Value: info.Total.RealIncome },
            { Name: "ColVirtualTurnOver", Value: info.Total.VirtualTurnover },
            { Name: "ColRealTurnOver", Value: info.Total.RealTurnover },
            { Name: "ColVirtualMargin", Value: info.Total.VirtualMargin },
            { Name: "ColRealMargin", Value: info.Total.RealMargin }],
            SubRows: []
        };

        var matchItemLength = info.Matches.length;
        if (matchItemLength > 0) {
            for (var j = 0; j < matchItemLength; j++) {
                var matchInfo = info.Matches[j];
                currentSubRow =
                {
                    Columns: [{
                        Name: "ColEvent", Value: {
                            event: matchInfo.MatchEvent,
                            matchId: matchInfo.MatchId,
                            LiveIndicator: matchInfo.LiveIndicator,
                            isHedge: matchInfo.IsHedge
                        },
                        CustomCellRender: function (currentCell) {
                            var liveStatus = currentCell.Value.LiveIndicator ? "Live" : "Non-Live";
                            var clickEvent = ' onclick = "viewBetTypes(' + currentCell.Value.matchId + ',' + currentCell.Value.LiveIndicator + ',\'' + currentCell.Value.event + '\'' + ',' + currentCell.Value.isHedge + ")\"";
                            var html = '<div class="link " ' + clickEvent + ' title="' + currentCell.Value.event + '">' + liveStatus + '</div>';
                            return html;
                        }
                    },
                    { Name: "ColMemberWinLoss", Value: matchInfo.MemberWinLoss },
                    { Name: "ColSMAWinLoss", Value: matchInfo.SMAWinLoss },
                    { Name: "ColExtraPT", Value: matchInfo.ExtraPT },
                    { Name: "ColCommision", Value: matchInfo.Commission },
                    { Name: "ColVariance", Value: matchInfo.VarianceIncome },
                    { Name: "ColVirtualWinLoss", Value: matchInfo.VirtualWinLoss },
                    { Name: "ColRealIncome", Value: matchInfo.RealIncome },
                    { Name: "ColVirtualTurnOver", Value: matchInfo.VirtualTurnover },
                    { Name: "ColRealTurnOver", Value: matchInfo.RealTurnover },
                    { Name: "ColVirtualMargin", Value: matchInfo.VirtualMargin },
                    { Name: "ColRealMargin", Value: matchInfo.RealMargin }],
                    CssClass: j % 2 == 0 ? "even" : ""
                };

                currentRow.SubRows.push(currentSubRow);
            }
        }

        bodyData.push(currentRow);
    };

    var showMatchesWithChoice = function (info, bodyData) {
        currentRow =
        {
            Columns: [{
                Name: "ColEvent", Value: {
                    startDate: self.Params.reportParams.StartDate,
                    sportType: self.Params.reportParams.SportType,
                    sportName: self.Params.reportParams.SportName,
                    isHedge: info.Total.IsHedge,
                    leagueId: self.Params.reportParams.LeagueId,
                    leagueName: self.Params.reportParams.LeagueName,
                    matchId: info.Total.MatchId,
                    matchCode: info.Total.MatchCode,
                    event: info.Total.MatchEvent,
                    isOutright: info.Total.IsOutright
                },
                CustomCellRender: function (currentCell) {
                    const openScoreInfoEvent = "scoreInfoManager.GetReport('" + currentCell.Value.startDate + "'," + //*
                        currentCell.Value.sportType + ",'" +
                        currentCell.Value.sportName + "','" + //*
                        0 + "','" + //*
                        null + "'," + //*
                        null + ",'" + //*
                        currentCell.Value.isHedge + "','" +
                        currentCell.Value.leagueId + "','" +
                        currentCell.Value.leagueName + "'," + //*
                        currentCell.Value.matchId + ",'" + //*
                        currentCell.Value.matchCode + "','" +
                        null + "','" +
                        null + "','" + //*
                        null + "','" +
                        null + "');"

                    if (currentCell.Value.isOutright) {
                        return '<span>' + currentCell.Value.event + '<span>';
                    } else {
                        return '<span class="link" onclick="event.stopPropagation();' + openScoreInfoEvent + '">' + currentCell.Value.event + '<span>';
                    }
                }
            },
            { Name: "ColMemberWinLoss", Value: info.Total.MemberWinLoss },
            { Name: "ColSMAWinLoss", Value: info.Total.SMAWinLoss },
            { Name: "ColExtraPT", Value: info.Total.ExtraPT },
            { Name: "ColCommision", Value: info.Total.Commission },
            { Name: "ColVariance", Value: info.Total.VarianceIncome },
            { Name: "ColVirtualWinLoss", Value: info.Total.VirtualWinLoss },
            { Name: "ColRealIncome", Value: info.Total.RealIncome },
            { Name: "ColVirtualTurnOver", Value: info.Total.VirtualTurnover },
            { Name: "ColRealTurnOver", Value: info.Total.RealTurnover },
            { Name: "ColVirtualMargin", Value: info.Total.VirtualMargin },
            { Name: "ColRealMargin", Value: info.Total.RealMargin }],
            SubRows: []
        };

        const matchItemLength = info.LiveIndicators.length;
        if (matchItemLength > 0) {
            for (let j = 0; j < matchItemLength; j++) {
                const matchInfo = info.LiveIndicators[j];
                const currentLiveIndicatorRow =
                {
                    Columns: [{
                        Name: "ColEvent", Value: {
                            event: matchInfo.Total.MatchEvent,
                            matchId: matchInfo.Total.MatchId,
                            liveIndicator: matchInfo.Total.LiveIndicator,
                            isHedge: matchInfo.Total.IsHedge
                        },
                        CustomCellRender: function (currentCell) {
                            const clickEvent = ' onclick = "event.stopPropagation();viewBetTypes(' + currentCell.Value.matchId + ',' + currentCell.Value.liveIndicator + ',\'' + info.Total.MatchEvent + '\'' + ',' + currentCell.Value.isHedge + ")\"";
                            const html = '<span class="link " ' + clickEvent + ' title="' + currentCell.Value.event + '">' + currentCell.Value.event + '</span>';
                            return html;
                        }
                    },
                    { Name: "ColMemberWinLoss", Value: matchInfo.Total.MemberWinLoss },
                        { Name: "ColSMAWinLoss", Value: matchInfo.Total.SMAWinLoss },
                        { Name: "ColExtraPT", Value: matchInfo.Total.ExtraPT },
                        { Name: "ColCommision", Value: matchInfo.Total.Commission },
                        { Name: "ColVariance", Value: matchInfo.Total.VarianceIncome },
                        { Name: "ColVirtualWinLoss", Value: matchInfo.Total.VirtualWinLoss },
                        { Name: "ColRealIncome", Value: matchInfo.Total.RealIncome },
                        { Name: "ColVirtualTurnOver", Value: matchInfo.Total.VirtualTurnover },
                        { Name: "ColRealTurnOver", Value: matchInfo.Total.RealTurnover },
                        { Name: "ColVirtualMargin", Value: matchInfo.Total.VirtualMargin },
                        { Name: "ColRealMargin", Value: matchInfo.Total.RealMargin }],
                    SubRows: []
                };
                const betTypes = info.LiveIndicators[j].BetTypes;
                for (let b = 0; b < betTypes.length; b++) {
                    const currentBetTypeRow =
                    {
                        Columns: [{
                            Name: "ColEvent", Value: betTypes[b].Total.MatchEvent
                        },
                            { Name: "ColMemberWinLoss", Value: betTypes[b].Total.MemberWinLoss },
                            { Name: "ColSMAWinLoss", Value: betTypes[b].Total.SMAWinLoss },
                            { Name: "ColExtraPT", Value: betTypes[b].Total.ExtraPT },
                            { Name: "ColCommision", Value: betTypes[b].Total.Commission },
                            { Name: "ColVariance", Value: betTypes[b].Total.VarianceIncome },
                            { Name: "ColVirtualWinLoss", Value: betTypes[b].Total.VirtualWinLoss },
                            { Name: "ColRealIncome", Value: betTypes[b].Total.RealIncome },
                            { Name: "ColVirtualTurnOver", Value: betTypes[b].Total.VirtualTurnover },
                            { Name: "ColRealTurnOver", Value: betTypes[b].Total.RealTurnover },
                            { Name: "ColVirtualMargin", Value: betTypes[b].Total.VirtualMargin },
                            { Name: "ColRealMargin", Value: betTypes[b].Total.RealMargin }],
                        SubRows: []
                    };
                    const choices = betTypes[b].Choices;
                    for (let c = 0; c < choices.length; c++) {
                        const currentChoiceRow =
                        {
                            Columns: [{
                                Name: "ColEvent", Value: choices[c].Choice
                            },
                                { Name: "ColMemberWinLoss", Value: choices[c].MemberWinLoss },
                                { Name: "ColSMAWinLoss", Value: choices[c].SMAWinLoss },
                                { Name: "ColExtraPT", Value: choices[c].ExtraPT },
                                { Name: "ColCommision", Value: choices[c].Commission },
                                { Name: "ColVariance", Value: choices[c].VarianceIncome },
                                { Name: "ColVirtualWinLoss", Value: choices[c].VirtualWinLoss },
                                { Name: "ColRealIncome", Value: choices[c].RealIncome },
                                { Name: "ColVirtualTurnOver", Value: choices[c].VirtualTurnover },
                                { Name: "ColRealTurnOver", Value: choices[c].RealTurnover },
                                { Name: "ColVirtualMargin", Value: choices[c].VirtualMargin },
                                { Name: "ColRealMargin", Value: choices[c].RealMargin }]
                        };
                        currentBetTypeRow.SubRows.push(currentChoiceRow);
                    }
                    currentLiveIndicatorRow.SubRows.push(currentBetTypeRow);
                }
                currentRow.SubRows.push(currentLiveIndicatorRow);
            }
        }

        bodyData.push(currentRow);
    };

    var renderReport = function (report) {
        _tables.reportContainer.removeClass("display-none");
        _tables.report.addClass("display-none");
        var bodyData = [];
        var footerData = [];

        if (report.Match.length > 0) {
            var len = report.Match.length;
            var currentRow = null;
            var info = null;
            for (var i = 0; i < len; i++) {
                info = report.Match[i];
                if (self.Params.reportParams.IsNormalMode) {
                    showMatchesWithChoice(info, bodyData);
                } else {
                    showMatches(info, bodyData);
                }
            }

            info = report.Total;
            currentRow =
            {
                Columns: [{ Name: "ColEvent", Value: info.MatchEvent, CssClass: "align-right" },
                { Name: "ColMemberWinLoss", Value: info.MemberWinLoss },
                { Name: "ColSMAWinLoss", Value: info.SMAWinLoss },
                { Name: "ColExtraPT", Value: info.ExtraPT },
                { Name: "ColCommision", Value: info.Commission },
                { Name: "ColVariance", Value: info.VarianceIncome },
                { Name: "ColVirtualWinLoss", Value: info.VirtualWinLoss },
                { Name: "ColRealIncome", Value: info.RealIncome },
                { Name: "ColVirtualTurnOver", Value: info.VirtualTurnover },
                { Name: "ColRealTurnOver", Value: info.RealTurnover },
                { Name: "ColVirtualMargin", Value: info.VirtualMargin },
                { Name: "ColRealMargin", Value: info.RealMargin }]
            };

            footerData.push(currentRow);

            if (self.isShowTotalRow(report.RM)) {
                info = report.RM;
                currentRow =
                {
                    Columns: [{ Name: "ColEvent", Value: info.MatchEvent, CssClass: "align-right", MergeColNumber: 6 },
                    { Name: "ColVirtualWinLoss", Value: info.VirtualWinLoss },
                    { Name: "ColRealIncome", Value: info.RealIncome },
                    { Name: "ColVirtualTurnOver", Value: info.VirtualTurnover },
                    { Name: "ColRealTurnOver", Value: info.RealTurnover },
                    { Name: "ColVirtualMargin", Value: "", MergeColNumber: 2 }]
                };

                footerData.push(currentRow);
            }

            if (self.isShowTotalRow(report.BARM)) {
                info = report.BARM;
                currentRow =
                {
                    Columns: [{ Name: "ColEvent", Value: info.MatchEvent, CssClass: "align-right", MergeColNumber: 6 },
                    { Name: "ColVirtualWinLoss", Value: info.VirtualWinLoss },
                    { Name: "ColRealIncome", Value: info.RealIncome },
                    { Name: "ColVirtualTurnOver", Value: info.VirtualTurnover },
                    { Name: "ColRealTurnOver", Value: info.RealTurnover },
                    { Name: "ColVirtualMargin", Value: "", MergeColNumber: 2 }]
                };

                footerData.push(currentRow);
            }

            if (self.isShowGrandTotalRow(report)) {
                info = report.GrandTotal;
                var totalInfo = report.Total;
                currentRow =
                {
                    Columns: [{ Name: "ColEvent", Value: info.MatchEvent, CssClass: "align-right", MergeColNumber: 6 },
                    { Name: "ColVirtualWinLoss", Value: totalInfo.VirtualWinLoss },
                    { Name: "ColRealIncome", Value: info.RealIncome },
                    { Name: "ColVirtualTurnOver", Value: totalInfo.VirtualTurnover },
                    { Name: "ColRealTurnOver", Value: totalInfo.RealTurnover },
                    { Name: "ColVirtualMargin", Value: "", MergeColNumber: 2 }]
                };

                footerData.push(currentRow);
            }
        }

        var gridOptions = {
            customizeHeader: true,
            groupHeaderRows: [{
                Columns: [{ HeaderText: "Event", Name: "ColEvent", MergeRowNumber: 2 },
                { HeaderText: "Member Win Loss", Name: "ColMemberWinLoss", MergeRowNumber: 2 },
                { HeaderText: "SMA Win Loss", Name: "ColSMAWinLoss", MergeRowNumber: 2 },
                { HeaderText: "Extra PT", Name: "ColExtraPT", MergeRowNumber: 2 },
                { HeaderText: "Commission", Name: "ColCommision", MergeRowNumber: 2 },
                { HeaderText: "House", Name: "ColVariance", MergeColNumber: 3 },
                { HeaderText: "House Turnover", Name: "ColVirtualTurnOver", MergeColNumber: 2 },
                { HeaderText: "Margin", Name: 'ColVirtualMargin', MergeColNumber: 2 }]
            }],
            columns: [{ HeaderText: "Event", Width: 400, Name: "ColEvent", HeaderAlign: "center", CellAlign: "left", ShowCollapseExpand: true },
            { HeaderText: "Member Win Loss", Width: 120, Name: "ColMemberWinLoss", HeaderAlign: "center", CellAlign: "right", FormatNumber: FormatConstant.TWO_DECIMAL_NUMBER, FormatPositive: '$' },
            { HeaderText: "SMA Win Loss", Width: 120, Name: "ColSMAWinLoss", HeaderAlign: "center", CellAlign: "right", FormatNumber: FormatConstant.TWO_DECIMAL_NUMBER },
            { HeaderText: "Extra PT", Width: 100, Name: "ColExtraPT", HeaderAlign: "center", CellAlign: "right", FormatNumber: FormatConstant.TWO_DECIMAL_NUMBER },
            { HeaderText: "Commission", Width: 120, Name: "ColCommision", HeaderAlign: "center", CellAlign: "right", FormatNumber: FormatConstant.TWO_DECIMAL_NUMBER },
            { HeaderText: "Variance", Width: 120, Name: "ColVariance", HeaderAlign: "center", CellAlign: "right", FormatNumber: FormatConstant.TWO_DECIMAL_NUMBER, FormatPositive: '$' },
            { HeaderText: "Virtual Win Loss", Width: 120, Name: "ColVirtualWinLoss", HeaderAlign: "center", CellAlign: "right", FormatNumber: FormatConstant.TWO_DECIMAL_NUMBER, FormatPositive: '$' },
            { HeaderText: "Real Income", Width: 120, Name: "ColRealIncome", HeaderAlign: "center", CellAlign: "right", FormatNumber: FormatConstant.TWO_DECIMAL_NUMBER, FormatPositive: '$' },
            { HeaderText: "Virtual", Width: 120, Name: "ColVirtualTurnOver", HeaderAlign: "center", CellAlign: "right", FormatNumber: FormatConstant.TWO_DECIMAL_NUMBER },
            { HeaderText: "Real", Width: 120, Name: "ColRealTurnOver", HeaderAlign: "center", CellAlign: "right", FormatNumber: FormatConstant.TWO_DECIMAL_NUMBER },
            { HeaderText: "Virtual", Width: 120, Name: "ColVirtualMargin", HeaderAlign: "center", CellAlign: "right", FormatNumber: FormatConstant.TWO_DECIMAL_PERCENT_NUMBER, FormatPositive: '$' },
            { HeaderText: "Real", Width: 120, Name: "ColRealMargin", HeaderAlign: "center", CellAlign: "right", FormatNumber: FormatConstant.TWO_DECIMAL_PERCENT_NUMBER, FormatPositive: '$' }],
            showTitle: true,
            showFooter: true,
            titleInfo:
            {
                text: self.Params.reportParams.SportName + " - " + self.Params.reportParams.LeagueName,
                showExcelButton: true,
                exportExcelFunction: function () {
                    self.exportReport();
                },
                showBackButton: true,
                backFunction: function () {
                    backToLeagueWinLoss();
                },

                showCollapseExpandButton: true
            },
            showTree: true,
            bodyRows: bodyData,
            showDistinctRow: false,
            footerRows: footerData,
            gridExpandHeight: 300
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
        const url = self.Params.reportParams.IsNormalMode ? "WinLoss/LeagueWinLoss/ExportLeagueWinLossViewMatchesWithChoice" : "WinLoss/LeagueWinLoss/ExportLeagueWinLossViewMatches";
        AccReport.downloadFile(window.rootUrl + url, self.Params.reportParams);
    };

    self.isShowTotalRow = function (total) {
        return total.VirtualWinLoss != 0 || total.RealIncome != 0 || total.VirtualTurnover != 0 || total.RealTurnover != 0;
    };

    self.isShowGrandTotalRow = function (report) {
        return self.isShowTotalRow(report.RM)
            || self.isShowTotalRow(report.BARM);
    }
}