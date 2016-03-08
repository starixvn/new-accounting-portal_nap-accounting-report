Namespace.Register('Fanex.AccountingReports.Main.WinLossSummary3rd.GameMatchDetails');
Fanex.AccountingReports.Main.WinLossSummary3rd.GameMatchDetails = function () {
    var _winLossSummary = new Fanex.AccountingReports.Main.WinLossSummary3rd.WinLossSummary();
    var self = this;

    var _tables = {
        winlossSumReportContainer: null,
        gameMatchDetailContainer: null,
        gameMatchDetail: null,
        parentName: null
    };
    var _params = {
        reportParams: {
            FromDate: null,
            ToDate: null,
            SportType: null,
            BetType: null,
            LiveIndicator: null,
            ParentName: null,
            DrillDownByType: null
        }
    };
    var _type = {
        SportBook: 1,
        Racing: 2,
        VirtualSports: 3,
        VirtualRacing: 4,
        NumberGame: 5 // special sport of SportBook
    };
    var betTypeOutright = 10;

    self.initPage = function () {
        cachedDOMElements();
    };
    var cachedDOMElements = function () {
        _tables.winlossSumReportContainer = $('#winlossSumReportContainer');
        _tables.gameMatchDetailContainer = $('#gameMatchDetailContainer');
        _tables.gameMatchDetail = $('#gameMatchDetail');
        _tables.btnShowHideSiteTypeInfoHistory = $("#btnShowHideSiteTypeInfoHistory");
    };
    self.getReport = function (params, parentName) {
        AccReport.disableInput();
        _tables.parentName = parentName;
        getParams(params);
        $.ajax({
            type: 'POST',
            url: window.rootUrl + 'Main/WinLossSummary3rd/ViewGameMatchDetails',
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
        _params.reportParams.SportType = params.reportParams.SportType;
        _params.reportParams.BetType = params.reportParams.BetType;
        _params.reportParams.LiveIndicator = params.reportParams.LiveIndicator;
        _params.reportParams.ParentName = _tables.parentName;
        _params.reportParams.DrillDownByType = params.reportParams.DrillDownByType;
    };
    var renderReport = function (report) {
        _tables.btnShowHideSiteTypeInfoHistory.attr("disabled", true);
        _tables.winlossSumReportContainer.removeClass('display-none');
        _tables.winlossSumReportContainer.addClass('display-none');

        var bodyData = [];
        var footerData = [];
        var type = 0

        if (report) {
            type = report.Type;
            var betSummaries = report.MatchGameBetTypes;
            if (betSummaries != null) { // body
                bodyData = renderGridBody(type, betSummaries);
                // Footer: Grand Total
                footerData = renderGridFooter(type, report.Total);
            }
        }

        var groupHeader = renderGridGroupHeaderRows(type);
        var column = renderGridColumns(type);
        var gridOptions = {
            customizeHeader: true,
            groupHeaderRows: groupHeader,
            columns: column,
            showTitle: true,
            showFooter: true,
            titleInfo:
            {
                text: _tables.parentName + ' | Details report from ' + _params.reportParams.FromDate +
                      ' to ' + _params.reportParams.ToDate,
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
            gridExpandHeight: 250,
            headerColumnHeight: 15,
            showPager: true,
            pagerOption: {
                itemsPerPage: 50,
                currentPage: 0,
                itemsPerPageArray: [50, 100, 200, 500],
                totalItem: bodyData.length
            }
        };
        _tables.gameMatchDetail.removeClass('display-none').fanexGrid(gridOptions);
    };
    var renderGridGroupHeaderRows = function (type) {
        var header = [];
        if (type == _type.SportBook || type == _type.VirtualSports) {
            header.push({
                Columns: [{ HeaderText: viewResources.Code, Name: 'ColCode', MergeRowNumber: 2 },
                          { HeaderText: viewResources.Event, Name: 'ColHome', MergeColNumber: 3, MergeRowNumber: 2 },
                          { HeaderText: viewResources.WinLoss, Name: 'ColWinLoss', MergeRowNumber: 2 },
                          { HeaderText: viewResources.Position, Name: 'ColPosition', MergeRowNumber: 2 },
						  { HeaderText: viewResources.Comm, Name: 'ColComm', MergeRowNumber: 2 },
						  { HeaderText: viewResources.LineTotal, Name: 'ColRealLineTotal', MergeColNumber: 2 },
						  { HeaderText: viewResources.Variance, Name: 'ColVariance', MergeRowNumber: 2 },
						  { HeaderText: viewResources.Turnover, Name: 'ColRealTurnover', MergeColNumber: 2 },
						  { HeaderText: viewResources.Margin, Name: 'ColRealMargin', MergeColNumber: 2}]
            });
        }
        else {
            header.push({
                Columns: [{ HeaderText: viewResources.Code, Name: 'ColCode', MergeRowNumber: 2 },
                          { HeaderText: viewResources.WinLoss, Name: 'ColWinLoss', MergeRowNumber: 2 },
                          { HeaderText: viewResources.Position, Name: 'ColPosition', MergeRowNumber: 2 },
						  { HeaderText: viewResources.Comm, Name: 'ColComm', MergeRowNumber: 2 },
						  { HeaderText: viewResources.LineTotal, Name: 'ColRealLineTotal', MergeColNumber: 2 },
						  { HeaderText: viewResources.Variance, Name: 'ColVariance', MergeRowNumber: 2 },
						  { HeaderText: viewResources.Turnover, Name: 'ColRealTurnover', MergeColNumber: 2 },
						  { HeaderText: viewResources.Margin, Name: 'ColRealMargin', MergeColNumber: 2}]
            });
        }

        return header;
    };
    var renderGridColumns = function (type) {
        var column = [];
        if (type == _type.SportBook || type == _type.VirtualSports) {
            column.push(
					  { HeaderText: viewResources.Code, Width: 80, Name: 'ColCode', HeaderAlign: 'center', CellAlign: 'center' },
                      { HeaderText: '', Width: 100, Name: 'ColHome', HeaderAlign: 'center', CellAlign: 'right' },
					  { HeaderText: '', Width: 30, Name: 'ColVS', HeaderAlign: 'center', CellAlign: 'center' },
					  { HeaderText: '', Width: 100, Name: 'ColAway', HeaderAlign: 'center', CellAlign: 'left' },
					  { HeaderText: viewResources.WinLoss, Width: 100, Name: 'ColWinLoss', HeaderAlign: 'center', CellAlign: 'right' },
					  { HeaderText: viewResources.Position, Width: 100, Name: 'ColPosition', HeaderAlign: 'center', CellAlign: 'right' },
					  { HeaderText: viewResources.Comm, Width: 100, Name: 'ColComm', HeaderAlign: 'center', CellAlign: 'right' },

                      { HeaderText: viewResources.Real, Width: 100, Name: 'ColRealLineTotal', HeaderAlign: 'center', CellAlign: 'right' },
					  { HeaderText: viewResources.Virtual, Width: 100, Name: 'ColVirtualLineTotal', HeaderAlign: 'center', CellAlign: 'right' },

					  { HeaderText: viewResources.Variance, Width: 100, Name: 'ColVariance', HeaderAlign: 'center', CellAlign: 'right' },

					  { HeaderText: viewResources.Real, Width: 100, Name: 'ColRealTurnover', HeaderAlign: 'center', CellAlign: 'right' },
					  { HeaderText: viewResources.Virtual, Width: 100, Name: 'ColVirtualTurnover', HeaderAlign: 'center', CellAlign: 'right' },
					  { HeaderText: viewResources.Real, Width: 60, Name: 'ColRealMargin', HeaderAlign: 'center', CellAlign: 'right' },
					  { HeaderText: viewResources.Virtual, Width: 60, Name: 'ColVirtualMargin', HeaderAlign: 'center', CellAlign: 'right' });
        }
        else {
            column.push(
					  { HeaderText: viewResources.Code, Width: 155, Name: 'ColCode', HeaderAlign: 'center', CellAlign: 'center' },
					  { HeaderText: viewResources.WinLoss, Width: 110, Name: 'ColWinLoss', HeaderAlign: 'center', CellAlign: 'right' },
					  { HeaderText: viewResources.Position, Width: 110, Name: 'ColPosition', HeaderAlign: 'center', CellAlign: 'right' },
					  { HeaderText: viewResources.Comm, Width: 110, Name: 'ColComm', HeaderAlign: 'center', CellAlign: 'right' },

                      { HeaderText: viewResources.Real, Width: 100, Name: 'ColRealLineTotal', HeaderAlign: 'center', CellAlign: 'right' },
					  { HeaderText: viewResources.Virtual, Width: 100, Name: 'ColVirtualLineTotal', HeaderAlign: 'center', CellAlign: 'right' },

					  { HeaderText: viewResources.Variance, Width: 100, Name: 'ColVariance', HeaderAlign: 'center', CellAlign: 'right' },

					  { HeaderText: viewResources.Real, Width: 100, Name: 'ColRealTurnover', HeaderAlign: 'center', CellAlign: 'right' },
					  { HeaderText: viewResources.Virtual, Width: 100, Name: 'ColVirtualTurnover', HeaderAlign: 'center', CellAlign: 'right' },
					  { HeaderText: viewResources.Real, Width: 100, Name: 'ColRealMargin', HeaderAlign: 'center', CellAlign: 'right' },
					  { HeaderText: viewResources.Virtual, Width: 100, Name: 'ColVirtualMargin', HeaderAlign: 'center', CellAlign: 'right' });
        }

        return column;
    };
    var renderGridBody = function (type, details) {
        var body = [];
        if (type == _type.SportBook || type == _type.VirtualSports) {
            body = renderSportBookBody(type, details);
        }
        else {
            body = renderNumberGameBody(details);
        }

        return body;
    };
    var renderSportBookBody = function (type, details) {
        var body = [];
        var data = null;
        var leagues = null;
        var len = details.length;
        for (var i = 0; i < len; i++) {
            data = details[i];
            var betTypeId = data.Key.Id;
            var betTypeName = data.Key.Name;
            if (data.LiveIndicator == 1) {
                betTypeName = '<span class="live">Live!</span>' + ' ' + betTypeName;
            }
            // BetType (header)
            body.push({
                CssClass: 'betDetailHeader',
                Columns: [{ Name: 'ColCode', Value: betTypeName, MergeColNumber: 14}]
            });
            // League
            leagues = data.MatchGameLeagues;
            var league = null;
            var leagueLen = leagues.length;
            for (var j = 0; j < leagueLen; j++) {
                league = leagues[j];
                body.push({
                    CssClass: 'leagueDetailHeader',
                    Columns: [{ Name: 'ColCode' },
							  { Name: 'ColWinLoss', Value: league.Key.Name, MergeColNumber: 13, CellAlign: 'left'}]
                });
                // WinLoss info
                var winlosses = league.MatchGameWinLosses;
                var winloss = null;
                var winlossLen = winlosses.length;
                for (var k = 0; k < winlossLen; k++) {
                    winloss = winlosses[k];
                    var matchCode = winloss.MatchCode;
                    if (matchCode.length > 8) {
                        matchCode = matchCode.substring(0, 8) + '...';
                    }

                    var openScoreInfoEvent = "scoreInfoManager.GetReport('" + _params.reportParams.FromDate + "'," + //*
													_params.reportParams.SportType + ",'" +
                                                    _tables.parentName + "','" + //*
                                                    betTypeId + "','" + //*
                                                    data.Key.Name + "'," + //*
                                                    data.LiveIndicator + ",'" + //*
                                                    false + "','" +
                                                    winloss.LeagueId + "','" +
                                                    league.Key.Name + "'," + //*
                                                    winloss.MatchId + ",'" + //*
                                                    winloss.MatchCode + "','" +
                                                    winloss.HomeTeam.Id + "','" +
                                                    winloss.HomeTeam.Name + "','" + //*
                                                    winloss.AwayTeam.Id + "','" +
                                                    winloss.AwayTeam.Name + "'); return;"
                    openScoreInfoEvent = (type == _type.SportBook && betTypeId != betTypeOutright) // BetType != Outright
						? '<div class="link bold" title="' + winloss.MatchCode + '" onclick="' + openScoreInfoEvent + '">' + matchCode + '</div>'
						: winloss.MatchCode;
                    var winlossColumns = [{ Name: 'ColCode', Value: openScoreInfoEvent}];
                    if (betTypeId == betTypeOutright) { // outright
                        winlossColumns.push({ Name: 'ColHome', Value: winloss.HomeTeam.Name, MergeColNumber: 3, CellAlign: 'left' });
                    }
                    else {
                        winlossColumns.push({ Name: 'ColHome', Value: winloss.HomeTeam.Name });
                        winlossColumns.push({ Name: 'ColVS', Value: viewResources.Vs });
                        winlossColumns.push({ Name: 'ColAway', Value: winloss.AwayTeam.Name });
                    }
                    winlossColumns.push({ Name: 'ColWinLoss', Value: formatNumberCell(winloss.WinLossDetail.WinLossStr, true, false, false) });
                    winlossColumns.push({ Name: 'ColPosition', Value: winloss.WinLossDetail.PositionStr });
                    winlossColumns.push({ Name: 'ColComm', Value: winloss.WinLossDetail.CommissionStr });
                    winlossColumns.push({ Name: 'ColRealLineTotal', Value: formatNumberCell(winloss.WinLossDetail.RealLineTotalStr, true, false, false) });
                    winlossColumns.push({ Name: 'ColVirtualLineTotal', Value: formatNumberCell(winloss.WinLossDetail.VirtualLineTotalStr, true, false, false) });
                    winlossColumns.push({ Name: 'ColVariance', Value: formatNumberCell(winloss.WinLossDetail.VarianceStr, true, false, false) });
                    winlossColumns.push({ Name: 'ColRealTurnover', Value: winloss.WinLossDetail.RealTurnoverStr });
                    winlossColumns.push({ Name: 'ColVirtualTurnover', Value: winloss.WinLossDetail.VirtualTurnoverStr });
                    winlossColumns.push({ Name: 'ColRealMargin', Value: formatNumberCell(winloss.WinLossDetail.RealMarginStr, true, false, true) });
                    winlossColumns.push({ Name: 'ColVirtualMargin', Value: formatNumberCell(winloss.WinLossDetail.VirtualMarginStr, true, false, true) });
                    body.push({ Columns: winlossColumns });
                }
                // Total for each League
                var leagueTotal = league.Total;
                body.push({
                    CssClass: 'leagueTotal',
                    Columns: [{ Name: 'ColCode', Value: viewResources.Total, CellAlign: 'right', MergeColNumber: 4, CssClass: 'bold' },
							{ Name: 'ColWinLoss', Value: formatNumberCell(leagueTotal.WinLossStr, true, false, false) },
							{ Name: 'ColPosition', Value: leagueTotal.PositionStr },
							{ Name: 'ColComm', Value: leagueTotal.CommissionStr },
							{ Name: 'ColRealLineTotal', Value: formatNumberCell(leagueTotal.RealLineTotalStr, true, false, false) },
							{ Name: 'ColVirtualLineTotal', Value: formatNumberCell(leagueTotal.VirtualLineTotalStr, true, false, false) },
							{ Name: 'ColVariance', Value: formatNumberCell(leagueTotal.VarianceStr, true, false, false) },
							{ Name: 'ColRealTurnover', Value: leagueTotal.RealTurnoverStr },
							{ Name: 'ColVirtualTurnover', Value: leagueTotal.VirtualTurnoverStr },
							{ Name: 'ColRealMargin', Value: formatNumberCell(leagueTotal.RealMarginStr, true, false, true) },
							{ Name: 'ColVirtualMargin', Value: formatNumberCell(leagueTotal.VirtualMarginStr, true, false, true)}]
                });
            }
            // Total for each Bet
            var betTotal = data.Total;
            body.push({
                CssClass: 'subTotal',
                Columns: [{ Name: 'ColCode', Value: betTypeName + ' Total:', MergeColNumber: 4, CellAlign: 'right', CssClass: 'bold' },
							{ Name: 'ColWinLoss', Value: formatNumberCell(betTotal.WinLossStr, true, false, false) },
							{ Name: 'ColPosition', Value: betTotal.PositionStr },
							{ Name: 'ColComm', Value: betTotal.CommissionStr },
							{ Name: 'ColRealLineTotal', Value: formatNumberCell(betTotal.RealLineTotalStr, true, false, false) },
							{ Name: 'ColVirtualLineTotal', Value: formatNumberCell(betTotal.VirtualLineTotalStr, true, false, false) },
							{ Name: 'ColVariance', Value: formatNumberCell(betTotal.VarianceStr, true, false, false) },
							{ Name: 'ColRealTurnover', Value: betTotal.RealTurnoverStr },
							{ Name: 'ColVirtualTurnover', Value: betTotal.VirtualTurnoverStr },
							{ Name: 'ColRealMargin', Value: formatNumberCell(betTotal.RealMarginStr, true, false, true) },
							{ Name: 'ColVirtualMargin', Value: formatNumberCell(betTotal.VirtualMarginStr, true, false, true)}]
            });
        }
        return body;
    };
    var renderNumberGameBody = function (details) {
        var body = [];
        var data = null;
        var leagues = null;
        var len = details.length;
        for (var i = 0; i < len; i++) {
            data = details[i];
            var betTypeName = data.Key.Name;
            if (data.LiveIndicator == 1) {
                betTypeName = '<span class="live">Live!</span>' + ' ' + betTypeName;
            }
            // BetType (header)
            body.push({
                CssClass: 'betDetailHeader',
                Columns: [{ Name: 'ColCode', Value: betTypeName, MergeColNumber: 11}]
            });
            // League
            leagues = data.MatchGameLeagues;
            var league = null;
            var leagueLen = leagues.length;
            for (var j = 0; j < leagueLen; j++) {
                league = leagues[j];
                body.push({
                    CssClass: 'leagueDetailHeader',
                    Columns: [{ Name: 'ColCode' },
							  { Name: 'ColWinLoss', Value: league.Key.Name, MergeColNumber: 10, CellAlign: 'left'}]
                });
                // WinLoss info
                var winlosses = league.MatchGameWinLosses;
                var winloss = null;
                var winlossLen = winlosses.length;
                for (var k = 0; k < winlossLen; k++) {
                    winloss = winlosses[k];
                    body.push({
                        Columns: [{ Name: 'ColCode', Value: winloss.MatchCode },
							{ Name: 'ColWinLoss', Value: formatNumberCell(winloss.WinLossDetail.WinLossStr, true, false, false) },
							{ Name: 'ColPosition', Value: winloss.WinLossDetail.PositionStr },
							{ Name: 'ColComm', Value: winloss.WinLossDetail.CommissionStr },
							{ Name: 'ColRealLineTotal', Value: formatNumberCell(winloss.WinLossDetail.RealLineTotalStr, true, false, false) },
							{ Name: 'ColVirtualLineTotal', Value: formatNumberCell(winloss.WinLossDetail.VirtualLineTotalStr, true, false, false) },
							{ Name: 'ColVariance', Value: formatNumberCell(winloss.WinLossDetail.VarianceStr, true, false, false) },
							{ Name: 'ColRealTurnover', Value: winloss.WinLossDetail.RealTurnoverStr },
							{ Name: 'ColVirtualTurnover', Value: winloss.WinLossDetail.VirtualTurnoverStr },
							{ Name: 'ColRealMargin', Value: formatNumberCell(winloss.WinLossDetail.RealMarginStr, true, false, true) },
							{ Name: 'ColVirtualMargin', Value: formatNumberCell(winloss.WinLossDetail.VirtualMarginStr, true, false, true)}]
                    });
                }
                // Total for each League
                var leagueTotal = league.Total;
                body.push({
                    CssClass: 'leagueTotal',
                    Columns: [{ Name: 'ColCode', Value: viewResources.Total, CellAlign: 'right', CssClass: 'bold' },
							{ Name: 'ColWinLoss', Value: formatNumberCell(leagueTotal.WinLossStr, true, false, false) },
							{ Name: 'ColPosition', Value: leagueTotal.PositionStr },
							{ Name: 'ColComm', Value: leagueTotal.CommissionStr },
							{ Name: 'ColRealLineTotal', Value: formatNumberCell(leagueTotal.RealLineTotalStr, true, false, false) },
							{ Name: 'ColVirtualLineTotal', Value: formatNumberCell(leagueTotal.VirtualLineTotalStr, true, false, false) },
							{ Name: 'ColVariance', Value: formatNumberCell(leagueTotal.VarianceStr, true, false, false) },
							{ Name: 'ColRealTurnover', Value: leagueTotal.RealTurnoverStr },
							{ Name: 'ColVirtualTurnover', Value: leagueTotal.VirtualTurnoverStr },
							{ Name: 'ColRealMargin', Value: formatNumberCell(leagueTotal.RealMarginStr, true, false, true) },
							{ Name: 'ColVirtualMargin', Value: formatNumberCell(leagueTotal.VirtualMarginStr, true, false, true)}]
                });
            }
            // Total for each Bet
            var betTotal = data.Total;
            body.push({
                CssClass: 'subTotal',
                Columns: [{ Name: 'ColCode', Value: betTypeName + ' Total:', CellAlign: 'right', CssClass: 'bold' },
							{ Name: 'ColWinLoss', Value: formatNumberCell(betTotal.WinLossStr, true, false, false) },
							{ Name: 'ColPosition', Value: betTotal.PositionStr },
							{ Name: 'ColComm', Value: betTotal.CommissionStr },
							{ Name: 'ColRealLineTotal', Value: formatNumberCell(betTotal.RealLineTotalStr, true, false, false) },
							{ Name: 'ColVirtualLineTotal', Value: formatNumberCell(betTotal.VirtualLineTotalStr, true, false, false) },
							{ Name: 'ColVariance', Value: formatNumberCell(betTotal.VarianceStr, true, false, false) },
							{ Name: 'ColRealTurnover', Value: betTotal.RealTurnoverStr },
							{ Name: 'ColVirtualTurnover', Value: betTotal.VirtualTurnoverStr },
							{ Name: 'ColRealMargin', Value: formatNumberCell(betTotal.RealMarginStr, true, false, true) },
							{ Name: 'ColVirtualMargin', Value: formatNumberCell(betTotal.VirtualMarginStr, true, false, true)}]
            });
        }
        return body;
    };
    var renderGridFooter = function (type, total) {
        var footer = [];
        if (type == _type.SportBook || type == _type.VirtualSports) {
            footer.push({
                Columns: [{ Name: 'ColCode', Value: viewResources.GrandTotal_Lower, MergeColNumber: 4, CellAlign: 'right' },
							{ Name: 'ColWinLoss', Value: formatNumberCell(total.WinLossStr, true, false, false) },
							{ Name: 'ColPosition', Value: total.PositionStr },
							{ Name: 'ColComm', Value: total.CommissionStr },
							{ Name: 'ColRealLineTotal', Value: formatNumberCell(total.RealLineTotalStr, true, false, false) },
							{ Name: 'ColVirtualLineTotal', Value: formatNumberCell(total.VirtualLineTotalStr, true, false, false) },
							{ Name: 'ColVariance', Value: formatNumberCell(total.VarianceStr, true, false, false) },
							{ Name: 'ColRealTurnover', Value: total.RealTurnoverStr },
							{ Name: 'ColVirtualTurnover', Value: total.VirtualTurnoverStr },
							{ Name: 'ColRealMargin', Value: formatNumberCell(total.RealMarginStr, true, false, true) },
							{ Name: 'ColVirtualMargin', Value: formatNumberCell(total.VirtualMarginStr, true, false, true)}]
            });
        }
        else {
            footer.push({
                Columns: [{ Name: 'ColCode', Value: viewResources.GrandTotal_Lower, CellAlign: 'right' },
							{ Name: 'ColWinLoss', Value: formatNumberCell(total.WinLossStr, true, false, false) },
							{ Name: 'ColPosition', Value: total.PositionStr },
							{ Name: 'ColComm', Value: total.CommissionStr },
							{ Name: 'ColRealLineTotal', Value: formatNumberCell(total.RealLineTotalStr, true, false, false) },
							{ Name: 'ColVirtualLineTotal', Value: formatNumberCell(total.VirtualLineTotalStr, true, false, false) },
							{ Name: 'ColVariance', Value: formatNumberCell(total.VarianceStr, true, false, false) },
							{ Name: 'ColRealTurnover', Value: total.RealTurnoverStr },
							{ Name: 'ColVirtualTurnover', Value: total.VirtualTurnoverStr },
							{ Name: 'ColRealMargin', Value: formatNumberCell(total.RealMarginStr, true, false, true) },
							{ Name: 'ColVirtualMargin', Value: formatNumberCell(total.VirtualMarginStr, true, false, true)}]
            });
        }

        return footer;
    };
    var formatNumberCell = function (cellValue, isPositive, isNegative, isPercent) {
        var number = cellValue.replace(/\,/g, '');
        var isNumber = $.isNumeric(number);

        if (isNumber &&
            ((isPositive && parseFloat(number) > 0) ||
             (isNegative && parseFloat(number) < 0))) {
            return '<div class="color-red">' + cellValue + (isPercent ? '%' : '') + '</div>';
        }

        return cellValue + (isNumber && isPercent ? '%' : '');
    };
    var back2Main = function () {
        _tables.btnShowHideSiteTypeInfoHistory.attr("disabled", null);
        _tables.btnShowHideSiteTypeInfo.attr("disabled", null);
        scoreInfoManager.ClosePopup();
        // show summary
        _tables.winlossSumReportContainer.removeClass('display-none');
        // hide detail
        _tables.gameMatchDetailContainer.removeClass('display-none');
        _tables.gameMatchDetailContainer.addClass('display-none');
        if (!_tables.gameMatchDetailContainer.hasClass("display-none")) {
            _tables.gameMatchDetailContainer.fanexGrid("resizeGrid");
        }
    };
    var exportReport = function () {
        AccReport.downloadFile(window.rootUrl + "Main/WinLossSummary3rd/ExportWinLossSummary3rdDetails", _params.reportParams);
    };
}