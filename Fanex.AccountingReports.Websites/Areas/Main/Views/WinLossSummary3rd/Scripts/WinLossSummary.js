Namespace.Register('Fanex.AccountingReports.Main.WinLossSummary3rd.WinLossSummary');
Fanex.AccountingReports.Main.WinLossSummary3rd.WinLossSummary = function () {
    var self = this;
    self.Enums = {
        ShowHideCss: {
            Collapse: "collapse",
            Expand: "expand"
        },
        RowType: {
            InfoRow: 1,
            PaddingRow: 2
        }
    };
    self.CachedData = {
        maxGridHeight: 10000,
        isShowSiteTypeInfo: false
    };
    self.CacheData = {
        currentReport: winLossSummary
    };

    self.CachedData = {
        showHideSiteTypeIconTitle: viewResources.Collapse + "/" + viewResources.Expand
    };

    var _tables = {
        winlossSumReport: null,
        gameMatchDetailContainer: null,
        isShowDetail: false,
        btnShowHideSiteTypeInfo: null,
        btnShowHideSiteTypeInfoHistory: null
    };
    var _params = {
        reportParams: {
            FromDate: null,
            ToDate: null
        }
    };

    var drillDownByType = {
        bySport: 1,
        byBettype: 2
    };

    self.initPage = function () {
        cachedDOMElements();
        bindingEvent();
    };
    var bindingEvent = function () {
        _tables.btnShowHideSiteTypeInfo.unbind("click");
        _tables.btnShowHideSiteTypeInfo.click(function () {
            showHideSiteTypeInfo();
        });

        _tables.btnShowHideSiteTypeInfoHistory.unbind("click");
        _tables.btnShowHideSiteTypeInfoHistory.click(function () {
            showHideSiteTypeInfo();
        });
    };

    var cachedDOMElements = function () {
        _tables.winlossSumReport = $('#winlossSumReport');
        _tables.gameMatchDetailContainer = $('#gameMatchDetailContainer');
        _tables.btnShowHideSiteTypeInfo = $("#btnShowHideSiteTypeInfo");
        _tables.btnShowHideSiteTypeInfoHistory = $("#btnShowHideSiteTypeInfoHistory");
    };
    self.resetCollapseExpand = function () {
        _tables.btnShowHideSiteTypeInfo.attr("disabled", true);
        _tables.btnShowHideSiteTypeInfoHistory.attr("disabled", true);
    }

    self.getReport = function (params) {
        AccReport.disableInput();
        _tables.btnShowHideSiteTypeInfo.attr("disabled", null);
        _tables.btnShowHideSiteTypeInfoHistory.attr("disabled", null);
        getParams(params);
        $.ajax({
            type: 'POST',
            url: window.rootUrl + 'Main/WinLossSummary3rd/GetReport',
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
        _tables.gameMatchDetailContainer.removeClass('display-none');
        _tables.gameMatchDetailContainer.addClass('display-none');

        var bodyData = [];
        var footerData = [];

        if (report) {
            checkShowingDetail();
            var len;
            var currentRow;
            var sportTypeWinLosses = report.SportTypeWinLosses;
            if (sportTypeWinLosses && (len = sportTypeWinLosses.length) > 0) {
                for (var i = 0; i < len; i++) {
                    bodyData.push(renderSportTypeWinLoss(sportTypeWinLosses[i]));
                }

                // footer: Grand Total
                footerData.push(renderTotalFooter(viewResources.GrandTotal, report.AllTotal));
                footerData.push(renderTotalFooter("(8RM + BARM) Total:", report.IBCTotal));
            }
        }

        var gridOptions = {
            customizeHeader: true,
            groupHeaderRows: [{
                Columns: [{ HeaderText: '', Name: 'ColView', MergeRowNumber: 2 },
                          { HeaderText: viewResources.BetType, Name: 'ColBetType', MergeRowNumber: 2 },
                          { HeaderText: viewResources.WinLoss, Name: 'ColWinLoss', MergeRowNumber: 2 },
                          { HeaderText: viewResources.Position, Name: 'ColPosition', MergeRowNumber: 2 },
						  { HeaderText: viewResources.Comm, Name: 'ColComm', MergeRowNumber: 2 },
						  { HeaderText: viewResources.LineTotal, Name: 'ColRealLineTotal', MergeColNumber: 2 },
						  { HeaderText: viewResources.Variance, Name: 'ColVariance', MergeRowNumber: 2 },
						  { HeaderText: viewResources.Turnover, Name: 'ColRealTurnover', MergeColNumber: 2 },
						  { HeaderText: viewResources.Margin, Name: 'ColRealMargin', MergeColNumber: 2 }]
            }],
            columns: [{ HeaderText: '', Width: 50, Name: 'ColView', HeaderAlign: 'center', CellAlign: 'center', ShowCollapseExpand: false },
                      { HeaderText: viewResources.BetType, Width: 150, Name: 'ColBetType', HeaderAlign: 'center', CellAlign: 'left' },
                      { HeaderText: viewResources.WinLoss, Width: 105, Name: 'ColWinLoss', HeaderAlign: 'center', CellAlign: 'right' },
					  { HeaderText: viewResources.Position, Width: 105, Name: 'ColPosition', HeaderAlign: 'center', CellAlign: 'right' },
					  { HeaderText: viewResources.Comm, Width: 100, Name: 'ColComm', HeaderAlign: 'center', CellAlign: 'right' },

                      { HeaderText: viewResources.Real, Width: 110, Name: 'ColRealLineTotal', HeaderAlign: 'center', CellAlign: 'right' },
					  { HeaderText: viewResources.Virtual, Width: 110, Name: 'ColVirtualLineTotal', HeaderAlign: 'center', CellAlign: 'right' },

					  { HeaderText: viewResources.Variance, Width: 110, Name: 'ColVariance', HeaderAlign: 'center', CellAlign: 'right' },

					  { HeaderText: viewResources.Real, Width: 110, Name: 'ColRealTurnover', HeaderAlign: 'center', CellAlign: 'right' },
					  { HeaderText: viewResources.Virtual, Width: 110, Name: 'ColVirtualTurnover', HeaderAlign: 'center', CellAlign: 'right' },
					  { HeaderText: viewResources.Real, Width: 60, Name: 'ColRealMargin', HeaderAlign: 'center', CellAlign: 'right' },
					  { HeaderText: viewResources.Virtual, Width: 60, Name: 'ColVirtualMargin', HeaderAlign: 'center', CellAlign: 'right' }],
            showTitle: true,
            titleInfo:
            {
                text: 'Summary report from ' + _params.reportParams.FromDate +
                      ' to ' + _params.reportParams.ToDate,
                showExcelButton: true,
                exportExcelFunction: function () {
                    exportReport();
                },
                showCollapseExpandButton: false
            },
            showTree: true,
            bodyRows: bodyData,
            showFooter: true,
            footerRows: footerData,
            gridExpandHeight: 235,
            numberOfFrozenColumn: 0,
            showDistinctRow: false,
            showHighlightRow: false,
            headerColumnHeight: 15
        };
        _tables.winlossSumReport.removeClass('display-none');
        _tables.winlossSumReport.fanexGrid(gridOptions);
        if (!_tables.winlossSumReport.hasClass("display-none")) {
            _tables.winlossSumReport.fanexGrid("resizeGrid");
        }
    };
    var renderSportTypeWinLoss = function (sportWinloss) {
        var id = sportWinloss.Id;
        var sportName = sportWinloss.Name;
        var sportDrillDown = false;
        var subProduct = '';

        if (id == AccReportData.Constants.SportType.MetricGamingSuperLive) {
            id = AccReportData.Constants.SportType.Soccer;
            subProduct = 'MG-SuperLive';
        }
        else {
            subProduct = '';
        }

        var sportDrillDown = _tables.isShowDetail && sportWinloss.CanShowDetailLink && id > 0;

        // SportType (header)
        var sportType = {
            Columns: [{
                Name: 'ColView', CellAlign: 'center', CustomCellRender: function () {
                    if (sportDrillDown) {
                        return '<div class="viewicon" onclick="ViewMatchGameDetails(\'' +
                                                _params.reportParams.FromDate + '\',\'' +
                                                _params.reportParams.ToDate + '\',' +
                                                id + ',0,0' +
                                                ',\'' +
                                                sportName + '\',' +
                                                drillDownByType.bySport + '\,\'' +
                                                subProduct + '\')">&nbsp;</div>';
                    }
                    else {
                        return '<div>&nbsp;</div>';
                    }
                }
            },
					  { Name: 'ColBetType', Value: sportName, MergeColNumber: 11, CellAlign: 'center' }],
            SubRows: []
        }
        // BetType (body)
        var betTypeWinLosses = sportWinloss.BetTypeWinLosses;
        if (betTypeWinLosses) {
            var betTypeDrillDown = _tables.isShowDetail && sportWinloss.CanShowDetailLink;
            var betType = null;
            var betTypeNewCricket = null;
            var isNewCricket = false;
            var len = betTypeWinLosses.length;
            for (var i = 0; i < len; i++) {
                betType = betTypeWinLosses[i];
                // ibcs
                var total = betType.Total;

                //for new *Cricket
                if (id == 50) {
                    if (total.Id == 500) {
                        isNewCricket = true;
                        betTypeNewCricket = total;
                        continue;
                    }
                    sportType.SubRows.push(renderWinLossCricket(total, true, sportWinloss.Id, id > 0 ? sportName : total.Name, betTypeDrillDown, i));
                }
                else {
                    if (betType.Id < 221 && betType.Id > 212) {
                        sportType.SubRows.push(renderTotalNew(total, 'total', total.Name + ' ' + viewResources.Total));
                    }
                    else {
                        sportType.SubRows.push(renderWinLoss(total, true, sportWinloss.Id, id > 0 ? sportName : total.Name, betTypeDrillDown, i));
                    }
                }

                if (betType.EightRM) { // 8RM
                    sportType.SubRows.push(renderWinLoss(betType.EightRM, false, sportWinloss.Id, id > 0 ? sportName : betType.EightRM.Name, false, i));
                }
                if (betType.BARM) { // BARM
                    sportType.SubRows.push(renderWinLoss(betType.BARM, false, sportWinloss.Id, id > 0 ? sportName : betType.BARM.Name, false, i));
                }
                if (betType.Hedge) { // Hedge
                    sportType.SubRows.push(renderWinLoss(betType.Hedge, false, sportWinloss.Id, id > 0 ? sportName : betType.Hedge.Name, false, i));
                }

                var hasOtherMatchType = betType.EightRM || betType.BARM || betType.Hedge;
                var showNormal = betType.Normal && hasOtherMatchType;
                if (showNormal) { // Normal
                    sportType.SubRows.push(renderWinLoss(betType.Normal, false, sportWinloss.Id, id > 0 ? sportName : betType.Normal.Name, false, i));
                }
            }

            // Total for New Cricket
            if (id == 50) {
                var footerTotal = sportWinloss.TotalForNewCricket;
                sportType.SubRows.push(renderTotal(footerTotal, 'subTotal', viewResources.Total));
            }
            if (isNewCricket == true) {
                sportType.SubRows.push(renderTotalWinLossCricket(betTypeNewCricket, 'subTotal', betTypeNewCricket.Name, betTypeDrillDown, id, id > 0 ? sportName : total.Name));
            }

            // Total (footer)
            var footerTotal = sportWinloss.Total;
            sportType.SubRows.push(renderTotal(footerTotal, 'subTotal', sportName + ' ' + viewResources.Total));
            // IBC Total
            var ibcTotal = sportWinloss.IBCTotal;
            if (ibcTotal) {
                sportType.SubRows.push(renderTotal(ibcTotal, 'subTotal', viewResources._8RM_BARM));
            }
            // Hedge Total
            var hedgeTotal = sportWinloss.HedgeTotal;
            if (hedgeTotal) {
                sportType.SubRows.push(renderTotal(hedgeTotal, 'subTotal', viewResources.HedgeTotal));
            }

            var totalHasOtherMatchType = sportWinloss.IBCTotal || sportWinloss.HedgeTotal;
            var showTotalNormal = sportWinloss.NormalTotal && totalHasOtherMatchType;
            var normalTotal = sportWinloss.NormalTotal;
            if (showTotalNormal) {
                sportType.SubRows.push(renderTotal(normalTotal, 'subTotal', viewResources.NormalTotal));
            }
        }
        return sportType;
    };

    var renderWinLossCricket = function (winloss, ibc, sportId, sportName, showDetailLink, bettypeIndex) {
        var cssClass = bettypeIndex % 2 == 0 ? 'evenRow' : 'oddRow';
        var betType = ibc ? ((winloss.LiveIndicator == 1 ? '<span class="live">Live!</span> ' : '') + winloss.Name) : winloss.Name;
        var groupdId = '';
        return {
            Columns: [{
                Name: 'ColView',
                TdClass: cssClass,
                CustomCellRender: function () {
                    if (showDetailLink) {
                        return '<div class="viewicon" onclick="ViewMatchGameDetails(\'' +
                                    _params.reportParams.FromDate + '\',\'' +
                                    _params.reportParams.ToDate + '\',' +
                                    sportId + ',' +
                                    winloss.Id + ',' +
                                    winloss.LiveIndicator + ',\'' +
                                    sportName + '\',' +
                                    drillDownByType.byBettype + ')">&nbsp;</div>';
                    }
                    else {
                        return '<div>&nbsp;</div>';
                    }
                }
            },
						{ Name: 'ColBetType', Value: betType, CellAlign: ibc ? 'left' : 'center', CssClass: ibc ? 'bold' : '', TdClass: cssClass },
						{ Name: 'ColWinLoss', Value: formatNumberCell(winloss.WinLossStr, true, false, false), TdClass: cssClass },
						{ Name: 'ColPosition', Value: winloss.PositionStr, TdClass: cssClass },
						{ Name: 'ColComm', Value: winloss.CommissionStr, TdClass: cssClass },
						{ Name: 'ColRealLineTotal', Value: formatNumberCell(winloss.RealLineTotalStr, true, false, false), TdClass: cssClass },
						{ Name: 'ColVirtualLineTotal', Value: formatNumberCell(winloss.VirtualLineTotalStr, true, false, false), TdClass: cssClass },
						{ Name: 'ColVariance', Value: formatNumberCell(winloss.VarianceStr, true, false, false), TdClass: cssClass },
						{ Name: 'ColRealTurnover', Value: winloss.RealTurnoverStr, TdClass: cssClass },
						{ Name: 'ColVirtualTurnover', Value: winloss.VirtualTurnoverStr, TdClass: cssClass },
						{ Name: 'ColRealMargin', Value: formatNumberCell(winloss.RealMarginStr, true, false, true), TdClass: cssClass },
						{ Name: 'ColVirtualMargin', Value: formatNumberCell(winloss.VirtualMarginStr, true, false, true), TdClass: cssClass }],
            CssClass: GetSiteTypeRowCss(groupdId, winLossSummary.Enums.RowType.InfoRow)
        }
    };

    var renderTotalWinLossCricket = function (total, css, title, showDetailLink, sportId, sportName) {
        return {
            CssClass: css,
            Columns: [{
                Name: 'ColView',
                TdClass: css,
                CustomCellRender: function () {
                    if (showDetailLink) {
                        return '<div class="viewicon" onclick="ViewMatchGameDetails(\'' +
                                    _params.reportParams.FromDate + '\',\'' +
                                    _params.reportParams.ToDate + '\',' +
                                    sportId + ',' +
                                    total.Id + ',' +
                                    total.LiveIndicator + ',\'' +
                                    sportName + '\',' +
                                    drillDownByType.byBettype + ')">&nbsp;</div>';
                    }
                    else {
                        return '<div>&nbsp;</div>';
                    }
                }
            },
                        { Name: 'ColBetType', Value: total.Name, CellAlign: 'right', CssClass: 'bold', TdClass: css },
						{ Name: 'ColWinLoss', Value: formatNumberCell(total.WinLossStr, true, false, false) },
						{ Name: 'ColPosition', Value: total.PositionStr },
						{ Name: 'ColComm', Value: total.CommissionStr },
						{ Name: 'ColRealLineTotal', Value: formatNumberCell(total.RealLineTotalStr, true, false, false) },
						{ Name: 'ColVirtualLineTotal', Value: formatNumberCell(total.VirtualLineTotalStr, true, false, false) },
						{ Name: 'ColVariance', Value: formatNumberCell(total.VarianceStr, true, false, false) },
						{ Name: 'ColRealTurnover', Value: total.RealTurnoverStr },
						{ Name: 'ColVirtualTurnover', Value: total.VirtualTurnoverStr },
						{ Name: 'ColRealMargin', Value: formatNumberCell(total.RealMarginStr, true, false, true) },
						{ Name: 'ColVirtualMargin', Value: formatNumberCell(total.VirtualMarginStr, true, false, true) }]
        }
    }
    var renderWinLoss = function (winloss, ibc, sportId, sportName, showDetailLink, bettypeIndex) {
        var cssClass = bettypeIndex % 2 == 0 ? 'evenRow' : 'oddRow';
        var betType = ibc ? ((winloss.LiveIndicator == 1 ? '<span class="live">Live!</span> ' : '') + winloss.Name) : winloss.Name;

        console.log(winloss);
        var groupdId = '';
        if (sportId == AccReportData.Constants.SportType.MetricGamingSuperLive) {
            sportId = AccReportData.Constants.SportType.Soccer;
        }
        else if (sportId == 161) {
            sportId = winloss.SportId;
            sportName = winloss.Name;
        }

        if (sportId == 168) {
            showDetailLink = false;
        }

        return {
            Columns: [{
                Name: 'ColView',
                TdClass: cssClass,
                CustomCellRender: function () {
                    if (showDetailLink) {
                        return '<div class="viewicon" onclick="ViewMatchGameDetails(\'' +
                                    _params.reportParams.FromDate + '\',\'' +
                                    _params.reportParams.ToDate + '\',' +
                                    sportId + ',' +
                                    winloss.Id + ',' +
                                    winloss.LiveIndicator + ',\'' +
                                    sportName + '\',' +
                                    drillDownByType.byBettype + ')">&nbsp;</div>';
                    }
                    else {
                        return '<div>&nbsp;</div>';
                    }
                }
            },
						{ Name: 'ColBetType', Value: betType, CellAlign: ibc ? 'left' : 'center', CssClass: ibc ? 'bold' : '', TdClass: cssClass },
						{ Name: 'ColWinLoss', Value: formatNumberCell(winloss.WinLossStr, true, false, false), TdClass: cssClass },
						{ Name: 'ColPosition', Value: winloss.PositionStr, TdClass: cssClass },
						{ Name: 'ColComm', Value: winloss.CommissionStr, TdClass: cssClass },
						{ Name: 'ColRealLineTotal', Value: formatNumberCell(winloss.RealLineTotalStr, true, false, false), TdClass: cssClass },
						{ Name: 'ColVirtualLineTotal', Value: formatNumberCell(winloss.VirtualLineTotalStr, true, false, false), TdClass: cssClass },
						{ Name: 'ColVariance', Value: formatNumberCell(winloss.VarianceStr, true, false, false), TdClass: cssClass },
						{ Name: 'ColRealTurnover', Value: winloss.RealTurnoverStr, TdClass: cssClass },
						{ Name: 'ColVirtualTurnover', Value: winloss.VirtualTurnoverStr, TdClass: cssClass },
						{ Name: 'ColRealMargin', Value: formatNumberCell(winloss.RealMarginStr, true, false, true), TdClass: cssClass },
						{ Name: 'ColVirtualMargin', Value: formatNumberCell(winloss.VirtualMarginStr, true, false, true), TdClass: cssClass }],
            CssClass: GetSiteTypeRowCss(groupdId, winLossSummary.Enums.RowType.InfoRow)
        }
    };
    var renderTotal = function (total, css, title) {
        return {
            CssClass: css,
            Columns: [{ Name: 'ColView', Value: title, MergeColNumber: 2, CellAlign: 'right', CssClass: 'bold', TdClass: css },
						{ Name: 'ColWinLoss', Value: formatNumberCell(total.WinLossStr, true, false, false) },
						{ Name: 'ColPosition', Value: total.PositionStr },
						{ Name: 'ColComm', Value: total.CommissionStr },
						{ Name: 'ColRealLineTotal', Value: formatNumberCell(total.RealLineTotalStr, true, false, false) },
						{ Name: 'ColVirtualLineTotal', Value: formatNumberCell(total.VirtualLineTotalStr, true, false, false) },
						{ Name: 'ColVariance', Value: formatNumberCell(total.VarianceStr, true, false, false) },
						{ Name: 'ColRealTurnover', Value: total.RealTurnoverStr },
						{ Name: 'ColVirtualTurnover', Value: total.VirtualTurnoverStr },
						{ Name: 'ColRealMargin', Value: formatNumberCell(total.RealMarginStr, true, false, true) },
						{ Name: 'ColVirtualMargin', Value: formatNumberCell(total.VirtualMarginStr, true, false, true) }]
        }
    };
    var renderTotalNew = function (total, css, title) {
        var groupdId = '';
        return {
            CssClass: css,
            Columns: [{ Name: 'ColView', Value: title, MergeColNumber: 2, CellAlign: 'right', CssClass: 'bold', TdClass: css },
						{ Name: 'ColWinLoss', Value: formatNumberCell(total.WinLossStr, true, false, false), TdClass: css },
						{ Name: 'ColPosition', Value: total.PositionStr, TdClass: css },
						{ Name: 'ColComm', Value: total.CommissionStr, TdClass: css },
						{ Name: 'ColRealLineTotal', Value: formatNumberCell(total.RealLineTotalStr, true, false, false), TdClass: css },
						{ Name: 'ColVirtualLineTotal', Value: formatNumberCell(total.VirtualLineTotalStr, true, false, false), TdClass: css },
						{ Name: 'ColVariance', Value: formatNumberCell(total.VarianceStr, true, false, false), TdClass: css },
						{ Name: 'ColRealTurnover', Value: total.RealTurnoverStr, TdClass: css },
						{ Name: 'ColVirtualTurnover', Value: total.VirtualTurnoverStr, TdClass: css },
						{ Name: 'ColRealMargin', Value: formatNumberCell(total.RealMarginStr, true, false, true), TdClass: css },
						{ Name: 'ColVirtualMargin', Value: formatNumberCell(total.VirtualMarginStr, true, false, true), TdClass: css }],
            CssClass: GetSiteTypeRowCss(groupdId, winLossSummary.Enums.RowType.InfoRow)
        }
    };
    var renderTotalFooter = function (totalStr, total) {
        return {
            Columns: [{ Name: 'ColView', Value: totalStr, MergeColNumber: 2, CellAlign: 'right' },
					{ Name: 'ColWinLoss', Value: formatNumberCell(total.WinLossStr, true, false, false) },
					{ Name: 'ColPosition', Value: total.PositionStr },
					{ Name: 'ColComm', Value: total.CommissionStr },
					{ Name: 'ColRealLineTotal', Value: formatNumberCell(total.RealLineTotalStr, true, false, false) },
					{ Name: 'ColVirtualLineTotal', Value: formatNumberCell(total.VirtualLineTotalStr, true, false, false) },
					{ Name: 'ColVariance', Value: formatNumberCell(total.VarianceStr, true, false, false) },
					{ Name: 'ColRealTurnover', Value: total.RealTurnoverStr },
					{ Name: 'ColVirtualTurnover', Value: total.VirtualTurnoverStr },
					{ Name: 'ColRealMargin', Value: formatNumberCell(total.RealMarginStr, true, false, true) },
					{ Name: 'ColVirtualMargin', Value: formatNumberCell(total.VirtualMarginStr, true, false, true) }]
        }
    }
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
    var checkShowingDetail = function () {
        var balanceUpDate = new Date(balanceUp);
        var fromdate = new Date(_params.reportParams.FromDate);
        _tables.isShowDetail = fromdate >= balanceUpDate;
    };
    var exportReport = function () {
        AccReport.downloadFile(window.rootUrl + "Main/WinLossSummary3rd/ExportWinLossSummary3rd", _params.reportParams);
    };

    var showHideSiteTypeInfo = function () {
        var showHideCssEnum = winLossSummary.Enums.ShowHideCss;
        if (winLossSummary.CachedData.isShowSiteTypeInfo) {
            // Hide all site type row
            $(".siteTypeRow.infoRow").addClass("display-none");
            $(".siteTypeRow.paddingRow").removeClass("display-none");
            $(".showHideSiteType.showHideIcon").removeClass(showHideCssEnum.Expand)
                                               .addClass(showHideCssEnum.Collapse);
            _tables.btnShowHideSiteTypeInfo.val(viewResources.Expand);
            _tables.btnShowHideSiteTypeInfoHistory.val(viewResources.Expand);
        } else {
            $(".siteTypeRow.paddingRow").addClass("display-none");
            $(".siteTypeRow.infoRow").removeClass("display-none");
            $(".showHideSiteType.showHideIcon").removeClass(showHideCssEnum.Collapse)
                                               .addClass(showHideCssEnum.Expand);
            _tables.btnShowHideSiteTypeInfo.val(viewResources.Collapse);
            _tables.btnShowHideSiteTypeInfoHistory.val(viewResources.Collapse);
        }

        _tables.winlossSumReport.removeClass("display-none").fanexGrid("resizeGrid");

        winLossSummary.CachedData.isShowSiteTypeInfo = !winLossSummary.CachedData.isShowSiteTypeInfo;
    };
    var GetSiteTypeRowCss = function (groupdId, rowType) {
        var rowTypeEnum = winLossSummary.Enums.RowType;
        var rowCss = "";
        switch (rowType) {
            case rowTypeEnum.InfoRow:
                rowCss = "siteTypeRow infoRow " + groupdId +
                         (winLossSummary.CachedData.isShowSiteTypeInfo ? "" : " display-none");
                break;
            case rowTypeEnum.PaddingRow:
                rowCss = "siteTypeRow paddingRow " + groupdId +
                         (winLossSummary.CachedData.isShowSiteTypeInfo ? "display-none " : "");
                break;
        }

        return rowCss;
    };
}
var winLossSummary = new Fanex.AccountingReports.Main.WinLossSummary3rd.WinLossSummary();