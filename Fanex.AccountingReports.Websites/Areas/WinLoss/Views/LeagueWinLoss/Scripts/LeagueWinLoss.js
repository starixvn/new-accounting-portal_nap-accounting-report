Namespace.Register("Fanex.AccountingReports.WinLoss.LeagueWinLoss.LeagueWinLoss");
Fanex.AccountingReports.WinLoss.LeagueWinLoss.LeagueWinLoss = function () {
    var self = this;
    var _tables = {
        btnGenerate: null,
        datepickerplugin: null,
        reportContainer: null,
        report: null,
        ddlSport: null,
        btnToday: null,
        btnYesterday: null,
        monthpickerplugin: null,
        btnReportMode: null,
        bettransMode: null,
        warehouseMode: null,
        bettransModeBtn: null,
        txtLeague: null,
        autocomplete: null,
        ddlDivision: null
    };

    self.Params = {
        reportParams: {
            StartDate: null,
            EndDate: null,
            SportBettype: null,
            IsNormalMode: true,
            IsNoDraw: false,
            LeagueName: '',
            SortColumnName: '',
            SortDirection: '',
            SiteName: ''
        },

        isNormalMode: true,
        cachedNumberSports: null,
        autoCompleteManager: null,
        defaultTextboxLeague: "League Name",
        currentTextboxLeague: "League Name",
        isDefaultTextboxLeague: true,
        isOnAuto: false,
        isFirstLoad: true,
        LeagueName: '',
        SoccerID: 1,
        BasketballID: 2,
        ParlayID: 10000,
        SBAdjustmentId: 10001
    };

    self.initPage = function () {
        cachedDOMElements();
        setDayValueForButtons();
        renderMonthPicker();
        renderDatePicker();
        getSport();
        getDivision();
        hideWarehouseMode();
        makeLeagueSuggestion();
        bindingEvent();
    };
    var cachedDOMElements = function () {
        _tables.btnGenerate = $("#btnGenerate");
        _tables.reportContainer = $("#leagueWinLossReportContainer");
        _tables.report = $("#leagueWinLossReport");
        _tables.ddlSport = $("#dropdown-sport");
        _tables.btnToday = $("#btnToday");
        _tables.btnYesterday = $("#btnYesterday");
        _tables.monthpickerplugin = $("#dvMonth");
        _tables.btnReportMode = $("#btnReportMode");
        _tables.bettransMode = $("#bettrans-mode");
        _tables.warehouseMode = $("#warehouse-mode");
        _tables.bettransModeBtn = $("#bettrans-mode-btn");
        _tables.txtLeague = $("#txt-league");
        _tables.ddlDivision = $("#dropdown-division");
    };

    var setDayValueForButtons = function () {
        _tables.btnToday.attr('value', todayStr);
        _tables.btnYesterday.attr('value', yesterdayStr);
    };

    var hideWarehouseMode = function () {
        _tables.warehouseMode.addClass("display-none");
    };

    var renderDatePicker = function () {
        $('#datepickerplugin').nexDatePicker({
            nexDatePickerType: "single",
            firstDatePicker: {
                Id: "CheckDate",
                Label: labelDate,
                InitDate: toDay,
                LabelCss: "lbl-field col_3 bold",
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

    var getSport = function () {
        AccReport.utility.CallAjax(this,
            window.rootUrl + "WinLoss/LeagueWinLoss/GetSport",
            null,
            self.RenderSport,
            self.RenderSport,
            AccReport.enableInput,
            false);
    };

    var getDivision = function () {
        AccReport.utility.CallAjax(this,
            window.rootUrl + "WinLoss/LeagueWinLoss/GetSiteFilter",
            null,
            self.RenderDivision,
            self.RenderDivision,
            AccReport.enableInput,
            false);
    };

    var renderMonthPicker = function () {
        $('#dvMonth').monPickr({
            plugInId: '#dvMonth',
            today: toDay,
            maxDate: toDay,
            minDate: minWareHouseDate,
            showDateRange: true,
            log: function (msg) {
                console.warn(msg);
            }
        });

        _tables.monthpickerplugin = $('#dvMonth');
    };

    var bindingEvent = function () {
        _tables.btnGenerate.click(function () {
            self.btnGenerateClick();
        });

        _tables.btnToday.click(function () {
            self.btnTodayClick();
        });

        _tables.btnYesterday.click(function () {
            self.btnYesterdayClick();
        });

        _tables.btnReportMode.click(function () {
            if (self.Params.isNormalMode) {
                self.Params.isNormalMode = false;
                _tables.bettransMode.addClass("display-none");
                _tables.bettransModeBtn.addClass("display-none");
                _tables.warehouseMode.removeClass("display-none");
                _tables.btnReportMode.attr('value', "BACK");
            } else {
                self.Params.isNormalMode = true;
                _tables.bettransMode.removeClass("display-none");
                _tables.bettransModeBtn.removeClass("display-none");
                _tables.warehouseMode.addClass("display-none");
                _tables.btnReportMode.attr('value', "HISTORY REPORT");
            }

            $("#leagueWinLossReportContainer").addClass("display-none");
            $("#leagueWinLossReportMatchesContainer").addClass("display-none");
            $("#leagueWinLossBetTypeContainer").addClass("display-none");
            $("#leagueWinLossSiteContainer").addClass("display-none");
            $("#leagueWinLossSuperContainer").addClass("display-none");
            $("#leagueWinLossVirtualRateContainer").addClass("display-none");
            $("#leagueWinLossBetTypeByStatusContainer").addClass("display-none");
        });
    };

    var makeLeagueSuggestion = function () {
        self.Params.autoCompleteManager = _tables.txtLeague.autocomplete({
            serviceUrl: window.rootWebPath.replace(/\/$/g, '') + '/ReportHelper/LeagueSuggestion?' +
            $.param(getLeagueSuggestionParams()),
            minChars: 2
        });

        _tables.autocomplete = $('.autocomplete');
    };

    var getLeagueSuggestionParams = function () {
        var sportTypes = '';
        if (!self.Params.isFirstLoad) {
            sportTypes = $("#dropdown-sport").fanexMultiSelect('getItems', true).toString();
        }

        if (sportTypes == undefined || sportTypes == null) {
            sportTypes = '';
        }

        self.Params.isFirstLoad = false;
        return {
            sportBettype: sportTypes
        };
    };

    self.btnGenerateClick = function () {
        getParams();
        if (!checkReportParams(self.Params.reportParams)) {
            AccReport.enableInput();
            return;
        }

        AccReport.disableInput();
        hideBettypes();
        hideMatches();
        hideSites();
        hideSupers();

        AccReport.utility.CallAjax(this,
            window.rootUrl + "WinLoss/LeagueWinLoss/GetLeagueWinLoss",
            self.Params.reportParams,
            renderLeagueWinLoss,
            renderLeagueWinLoss,
            AccReport.enableInput,
            false);
    };

    self.btnTodayClick = function () {
        getParamByDate(todayStr);
        if (!checkReportParams(self.Params.reportParams)) {
            AccReport.enableInput();
            return;
        }

        AccReport.disableInput();
        hideBettypes();
        hideMatches();
        hideSites();
        hideSupers();

        AccReport.utility.CallAjax(this,
            window.rootUrl + "WinLoss/LeagueWinLoss/GetLeagueWinLoss",
            self.Params.reportParams,
            renderLeagueWinLoss,
            renderLeagueWinLoss,
            AccReport.enableInput,
            false);
    };

    self.btnYesterdayClick = function () {
        getParamByDate(yesterdayStr);
        if (!checkReportParams(self.Params.reportParams)) {
            AccReport.enableInput();
            return;
        }

        AccReport.disableInput();
        hideBettypes();
        hideMatches();
        hideSites();
        hideSupers();

        AccReport.utility.CallAjax(this,
            window.rootUrl + "WinLoss/LeagueWinLoss/GetLeagueWinLoss",
            self.Params.reportParams,
            renderLeagueWinLoss,
            renderLeagueWinLoss,
            AccReport.enableInput,
            false);
    };

    var getSortDirection = function (columnName) {
        return columnName == self.Params.reportParams.SortColumnName ? self.Params.reportParams.SortDirection : '';
    };

    var getParamByDate = function (date) {
        self.Params.reportParams.StartDate = date;
        self.Params.reportParams.EndDate = date;
        self.Params.reportParams.SportBettype = _tables.ddlSport.fanexMultiSelect('getItems', true).toString();
        self.Params.reportParams.IsNormalMode = self.Params.isNormalMode;
        self.Params.reportParams.IsNoDraw = $('input[name=radio-turnover]:checked').val() == 1;
        self.Params.reportParams.LeagueName = self.Params.isDefaultTextboxUserName ?
                      "" :
                      _tables.txtLeague.val();
        self.Params.reportParams.SiteName = _tables.ddlDivision.fanexMultiSelect('getItems', true).toString();
    };

    var getParams = function () {
        if (self.Params.isNormalMode) {
            var date = _tables.datepickerplugin.nexDatePicker("getSingleDate");
            self.Params.reportParams.StartDate = date;
            self.Params.reportParams.EndDate = date;
        } else {
            self.Params.reportParams.StartDate = _tables.monthpickerplugin.monPickr("getFromDate", FormatConstant.MMM_DD_YY_STRING_MONTH_PICKER);
            self.Params.reportParams.EndDate = _tables.monthpickerplugin.monPickr("getToDate", FormatConstant.MMM_DD_YY_STRING_MONTH_PICKER);
        }

        self.Params.reportParams.SportBettype = _tables.ddlSport.fanexMultiSelect('getItems', true).toString();
        self.Params.reportParams.IsNormalMode = self.Params.isNormalMode;
        self.Params.reportParams.IsNoDraw = $('input[name=radio-turnover]:checked').val() == 1;
        self.Params.reportParams.LeagueName = self.Params.isDefaultTextboxUserName ?
                      "" :
                      _tables.txtLeague.val();
        self.Params.reportParams.SiteName = _tables.ddlDivision.fanexMultiSelect('getItems', true).toString();
    };

    var checkReportParams = function (params) {
        AccReport.popUp.removeHighLight();
        if (self.Params.reportParams.SportBettype == '') {
            AccReport.popUp.show("Please select  at least one sport!", "dropdown-sport");
            return false;
        }

        if (self.Params.reportParams.SiteName == '') {
            AccReport.popUp.show("Please select  at least one division!", "dropdown-division");
            return false;
        }

        return true;
    };

    function mappingDataRow(info, cssClass) {
        return {
            Columns: [{ Name: "ColLeague", Value: info.LeagueName, CssClass: "align-right" },
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
                    { Name: "ColRealMargin", Value: info.RealMargin }],
            CssClass: cssClass
        };
    };

    var renderLeagueWinLoss = function (report) {
        var title = self.Params.isNormalMode ? "League Win Loss Report on " + self.Params.reportParams.StartDate
            : "League Win Loss Report From " + self.Params.reportParams.StartDate + " To " + self.Params.reportParams.EndDate;
        self.showReport();
        _tables.reportContainer.removeClass("display-none");
        _tables.report.addClass("display-none");
        var bodyData = [];
        var footerData = [];

        if (report.SportWinLoss.length > 0) {
            var len = report.SportWinLoss.length;
            var currentSubRow = null;
            var info = null;
            for (var i = 0; i < len; i++) {
                var sport = report.SportWinLoss[i];
                var currentRow =
                {
                    Columns: [
                        { Name: "ColLeague", Value: sport.SportName },
                        { Name: "ColMemberWinLoss", Value: sport.Total.MemberWinLoss },
                        { Name: "ColSMAWinLoss", Value: sport.Total.SMAWinLoss },
                        { Name: "ColExtraPT", Value: sport.Total.ExtraPT },
                        { Name: "ColCommision", Value: sport.Total.Commission },
                        { Name: "ColVariance", Value: sport.Total.VarianceIncome },
                        { Name: "ColVirtualWinLoss", Value: sport.Total.VirtualWinLoss },
                        { Name: "ColRealIncome", Value: sport.Total.RealIncome },
                        { Name: "ColVirtualTurnOver", Value: sport.Total.VirtualTurnover },
                        { Name: "ColRealTurnOver", Value: sport.Total.RealTurnover },
                        { Name: "ColVirtualMargin", Value: sport.Total.VirtualMargin },
                        { Name: "ColRealMargin", Value: sport.Total.RealMargin }],
                    SubRows: []
                };

                var childsLength = sport.Leagues.length;
                for (var j = 0; j < childsLength; j++) {
                    info = sport.Leagues[j];
                    currentSubRow =
                    {
                        Columns: [{
                            Name: "ColLeague", Value: {
                                sportName: info.Sport,
                                leagueName: info.LeagueName
                            }, CssClass: "colLeague",
                            CustomCellRender: function (currentCell) {
                                var leagueText = currentCell.Value.leagueName.length > 50 ? currentCell.Value.leagueName.substring(0, 51) + '...' : currentCell.Value.leagueName;
                                var clickEvent = ' onclick = "viewMatches(\'' + currentCell.Value.sportName + '\',\'' + currentCell.Value.leagueName + "');\"";
                                var html = '<div class="link " ' + clickEvent + ' title="' + currentCell.Value.leagueName + '">' + leagueText + '</div>';
                                return html;
                            }
                        },
                                  {
                                      Name: "ColMemberWinLoss",
                                      Value: {
                                          winLoss: info.MemberWinLoss,
                                          sportName: info.Sport,
                                          leagueName: info.LeagueName
                                      },
                                      CustomCellRender: function (currentCell) {
                                          var cssClass = currentCell.Value.winLoss > 0 ? 'format-positive' : '';
                                          var winLossValue = currentCell.Value.winLoss.toFixed(2);
                                          var clickEvent = ' onclick = "viewBetTypesByStatus(\'' + currentCell.Value.sportName + '\',\'' + currentCell.Value.leagueName + "');\"";
                                          var html = '<div class="link ' + cssClass + '" ' + clickEvent + ' title="' + currentCell.Value.leagueName + '">' + parseFloat(winLossValue).toLocaleString('en') + '</div>';
                                          return html;
                                      }
                                  },
                                  { Name: "ColSMAWinLoss", Value: info.SMAWinLoss },
                                  { Name: "ColExtraPT", Value: info.ExtraPT },
                                  { Name: "ColCommision", Value: info.Commission },
                                  { Name: "ColVariance", Value: info.VarianceIncome },
                                  { Name: "ColVirtualWinLoss", Value: info.VirtualWinLoss },
                                  { Name: "ColRealIncome", Value: info.RealIncome },
                                  { Name: "ColVirtualTurnOver", Value: info.VirtualTurnover },
                                  { Name: "ColRealTurnOver", Value: info.RealTurnover },
                                  { Name: "ColVirtualMargin", Value: info.VirtualMargin },
                                  { Name: "ColRealMargin", Value: info.RealMargin }],
                        CssClass: j % 2 == 0 ? "even" : ""
                    };

                    currentRow.SubRows.push(currentSubRow);
                }

                bodyData.push(currentRow);
            }

            currentRow = mappingDataRow(report.Total, "");
            footerData.push(currentRow);

            if (self.isShowTotalRow(report.SoccerRM)) {
                info = report.SoccerRM;
                currentRow =
                {
                    Columns: [{ Name: "ColLeague", Value: info.LeagueName, CssClass: "align-right", MergeColNumber: 6 },
                            { Name: "ColVirtualWinLoss", Value: info.VirtualWinLoss },
                            { Name: "ColRealIncome", Value: info.RealIncome },
                            { Name: "ColVirtualTurnOver", Value: info.VirtualTurnover },
                            { Name: "ColRealTurnOver", Value: info.RealTurnover },
                            { Name: "ColVirtualMargin", Value: "", MergeColNumber: 2 }]
                };

                footerData.push(currentRow);
            }

            if (self.isShowTotalRow(report.OtherSportRM)) {
                info = report.OtherSportRM;
                currentRow =
                {
                    Columns: [{ Name: "ColLeague", Value: info.LeagueName, CssClass: "align-right", MergeColNumber: 6 },
                            { Name: "ColVirtualWinLoss", Value: info.VirtualWinLoss },
                            { Name: "ColRealIncome", Value: info.RealIncome },
                            { Name: "ColVirtualTurnOver", Value: info.VirtualTurnover },
                            { Name: "ColRealTurnOver", Value: info.RealTurnover },
                            { Name: "ColVirtualMargin", Value: "", MergeColNumber: 2 }]
                };

                footerData.push(currentRow);
            }

            if (self.isShowTotalRow(report.SoccerBARM)) {
                info = report.SoccerBARM;
                currentRow =
                {
                    Columns: [{ Name: "ColLeague", Value: info.LeagueName, CssClass: "align-right", MergeColNumber: 6 },
                            { Name: "ColVirtualWinLoss", Value: info.VirtualWinLoss },
                            { Name: "ColRealIncome", Value: info.RealIncome },
                            { Name: "ColVirtualTurnOver", Value: info.VirtualTurnover },
                            { Name: "ColRealTurnOver", Value: info.RealTurnover },
                            { Name: "ColVirtualMargin", Value: "", MergeColNumber: 2 }]
                };

                footerData.push(currentRow);
            }

            if (self.isShowTotalRow(report.OtherSportBARM)) {
                info = report.OtherSportBARM;
                currentRow =
                {
                    Columns: [{ Name: "ColLeague", Value: info.LeagueName, CssClass: "align-right", MergeColNumber: 6 },
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
                    Columns: [{ Name: "ColLeague", Value: info.LeagueName, CssClass: "align-right", MergeColNumber: 6 },
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
                Columns: [{ HeaderText: "League", Name: "ColLeague", MergeRowNumber: 2 },
                          { HeaderText: "Member Win Loss", Name: "ColMemberWinLoss", MergeRowNumber: 2 },
                          { HeaderText: "SMA Win Loss", Name: "ColSMAWinLoss", MergeRowNumber: 2 },
                          { HeaderText: "Extra PT", Name: "ColExtraPT", MergeRowNumber: 2 },
						  { HeaderText: "Commission", Name: "ColCommision", MergeRowNumber: 2 },
						  { HeaderText: "House", Name: "ColVariance", MergeColNumber: 3 },
						  { HeaderText: "House Turnover", Name: "ColVirtualTurnOver", MergeColNumber: 2 },
						  { HeaderText: "Margin", Name: 'ColVirtualMargin', MergeColNumber: 2 }]
            }],
            columns: [{ HeaderText: "League", Width: 380, Name: "ColLeague", HeaderAlign: "center", CellAlign: "left", ShowCollapseExpand: true },
                        { HeaderText: "Member Win Loss", Width: 120, Name: "ColMemberWinLoss", HeaderAlign: "center", CellAlign: "right", FormatNumber: FormatConstant.TWO_DECIMAL_NUMBER, FormatPositive: '$' },
                        { HeaderText: "SMA Win Loss", Width: 120, Name: "ColSMAWinLoss", HeaderAlign: "center", CellAlign: "right", FormatNumber: FormatConstant.TWO_DECIMAL_NUMBER },
                        { HeaderText: "Extra PT", Width: 100, Name: "ColExtraPT", HeaderAlign: "center", CellAlign: "right", FormatNumber: FormatConstant.TWO_DECIMAL_NUMBER },
                        { HeaderText: "Commission", Width: 120, Name: "ColCommision", HeaderAlign: "center", CellAlign: "right", FormatNumber: FormatConstant.TWO_DECIMAL_NUMBER },
                        { HeaderText: "Variance", Width: 120, Name: "ColVariance", HeaderAlign: "center", CellAlign: "right", FormatNumber: FormatConstant.TWO_DECIMAL_NUMBER, FormatPositive: '$', Sort: true, SortDirection: getSortDirection("VarianceIncome") },
                        { HeaderText: "Virtual Win Loss", Width: 120, Name: "ColVirtualWinLoss", HeaderAlign: "center", CellAlign: "right", FormatNumber: FormatConstant.TWO_DECIMAL_NUMBER, FormatPositive: '$', Sort: true, SortDirection: getSortDirection("VirtualWinLoss") },
                        { HeaderText: "Real Income", Width: 120, Name: "ColRealIncome", HeaderAlign: "center", CellAlign: "right", FormatNumber: FormatConstant.TWO_DECIMAL_NUMBER, FormatPositive: '$', Sort: true, SortDirection: getSortDirection("RealIncome") },
                        { HeaderText: "Virtual", Width: 120, Name: "ColVirtualTurnOver", HeaderAlign: "center", CellAlign: "right", FormatNumber: FormatConstant.TWO_DECIMAL_NUMBER, Sort: true, SortDirection: getSortDirection("VirtualTurnover") },
                        { HeaderText: "Real", Width: 120, Name: "ColRealTurnOver", HeaderAlign: "center", CellAlign: "right", FormatNumber: FormatConstant.TWO_DECIMAL_NUMBER, Sort: true, SortDirection: getSortDirection("RealTurnover") },
                        { HeaderText: "Virtual", Width: 120, Name: "ColVirtualMargin", HeaderAlign: "center", CellAlign: "right", FormatNumber: FormatConstant.TWO_DECIMAL_PERCENT_NUMBER, FormatPositive: '$', Sort: true, SortDirection: getSortDirection("VirtualMargin") },
                        { HeaderText: "Real", Width: 120, Name: "ColRealMargin", HeaderAlign: "center", CellAlign: "right", FormatNumber: FormatConstant.TWO_DECIMAL_PERCENT_NUMBER, FormatPositive: '$', Sort: true, SortDirection: getSortDirection("RealMargin") }],
            showTitle: true,
            showFooter: true,
            titleInfo:
            {
                text: title,
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
            gridExpandHeight: 300,
            sortOption:
            {
                serverSortFunction: function (sortColumnName, sortDirection) {
                    var sortColumnName = getSortColumnName(sortColumnName);
                    self.Params.reportParams.SortDirection = sortDirection.toLowerCase();
                    self.Params.reportParams.SortColumnName = sortColumnName;
                    self.sortReport();
                }
            },
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

    self.RenderSport = function (rs) {
        self.Params.cachedNumberSports = 0;
        var isSetDefaultCollapse = true;
        var id = "ddl-sport";
        var sportGroups = [];
        var sportGroupLength = rs.length;
        for (var i = 0; i < sportGroupLength; i++) {
            var sportGroup = rs[i];
            var bettypeGroups = [];
            var bettypes = [];
            var sportLength = sportGroup.ChildItems.length;
            self.Params.cachedNumberSports += sportLength === 0 ? 1 : sportLength;
            for (var j = 0; j < sportLength; j++) {
                var sport = sportGroup.ChildItems[j];
                var bettypeGroupLength = sport.BettypeGroups.length;
                for (var l = 0; l < bettypeGroupLength; l++) {
                    bettypes = [];
                    var bettypeGroup = sport.BettypeGroups[l];
                    var bettypeLength = bettypeGroup.BetTypes.length;
                    for (m = 0; m < bettypeLength; m++) {
                        var bettype = bettypeGroup.BetTypes[m];
                        bettypes.push({
                            id: id + "-" + bettype.Bettype + "-" + bettype.BettypeName,
                            value: sport.SportType + "_" + bettype.Bettype,
                            text: bettype.BettypeName,
                            checked: true,
                            enable: true,
                            cssClass: ""
                        });
                    }

                    if (sport.SportType != self.Params.ParlayID && sport.SportType != self.Params.SBAdjustmentId) {
                        bettypeGroups.push({
                            id: id + "-" + sport.SportName + "-" + bettypeGroup.BetTypeGroupName,
                            value: sport.SportType,
                            text: bettypeGroup.BetTypeGroupName,
                            checked: true,
                            enable: true,
                            cssClass: "",
                            items: bettypes
                        });
                    }
                }
            }

            if (bettypeGroups.length > 0 || bettypes.length > 0) {
                var isRenderBetTypeGroup = sportGroup.GroupId != self.Params.ParlayID && sport.SportType != self.Params.SBAdjustmentId;
                sportGroups.push({
                    id: id + "-group" + sportGroup.GroupId,
                    value: sportGroup.GroupId,
                    text: sportGroup.GroupName,
                    checked: true,
                    enable: true,
                    cssClass: "",
                    items: isRenderBetTypeGroup ? bettypeGroups : bettypes
                });
            }
        }

        _tables.ddlSport.fanexMultiSelect({
            minWidth: 300,
            maxHeight: 450,
            showFilter: true,
            shadow: true,
            showHeader: true,
            isCheckAll: true,
            showFooter: false,
            checkAllText: 'All',
            isUsedCustomCheckAllText: true,
            useCustomCheckAllText: 'All',
            noneSelectedText: 'Select an option',
            noItemText: 'No sport found.',
            disabled: false,
            listData: sportGroups,
            onCheck: function () {
                makeLeagueSuggestion();
            },
            onOpen: function () {
                if (isSetDefaultCollapse) {
                    $('#ddlSportCollapseButton').trigger('click')
                    isSetDefaultCollapse = false;
                }
            }
        });

        _tables.ddlSport = $("#dropdown-sport");
    };

    self.isShowTotalRow = function (total) {
        return total.VirtualWinLoss != 0 || total.RealIncome != 0 || total.VirtualTurnover != 0 || total.RealTurnover != 0;
    };

    self.isShowGrandTotalRow = function (report) {
        return self.isShowTotalRow(report.SoccerRM)
           || self.isShowTotalRow(report.OtherSportRM)
           || self.isShowTotalRow(report.SoccerBARM)
           || self.isShowTotalRow(report.OtherSportBARM);
    };

    var bindingEvents_LeagueSuggestion = function () {
        _tables.txtLeague.unbind("blur");
        _tables.txtLeague.blur(function () {
            var textbox = $(this);
            if (!self.Params.isonAuto) {
                $('#Autocomplete_1').css('display', 'none');
            }
            else {
                textbox.focus();
            }

            var inputValue = '';
            var newName = textbox.val();
            var oldName = self.Params.currentTextboxLeague;
            self.Params.isDefaultTextboxLeague = false;
            textbox.removeClass("default-suggestionbox");
            if (oldName != '' &&
            newName != '' &&
            newName != oldName) {
                inputValue = newName;
            }
            else if (oldName != '' && newName == '') {
                inputValue = oldName;
                self.Params.isDefaultTextboxLeague = true;
                textbox.addClass("default-suggestionbox");
            }
            else if (newName != '') {
                inputValue = newName;
            }
            else {
                self.Params.isDefaultTextboxLeague = true;
                textbox.addClass("default-suggestionbox");
                inputValue = self.Params.defaultTextboxLeague;
            }

            textbox.val(inputValue);
        });

        _tables.txtLeague.unbind("focus");
        _tables.txtLeague.focus(function () {
            var textbox = $(this);
            textbox.removeClass("default-suggestionbox");
            if (self.Params.isDefaultTextboxLeague) {
                textbox.val('');
            }
        });

        _tables.autocomplete.mouseover(function () {
            self.Params.isonAuto = true;
        }).mouseout(function () {
            _tables.txtLeague.removeClass("default-suggestionbox");
            self.Params.isDefaultTextboxUserName = false;
            self.Params.isonAuto = false;
        });
    };

    self.sortReport = function () {
        AccReport.disableInput();
        AccReport.utility.CallAjax(this,
                                    window.rootUrl + "WinLoss/LeagueWinLoss/GetLeagueWinLoss",
                                    self.Params.reportParams,
                                    renderLeagueWinLoss,
                                    renderLeagueWinLoss,
                                    AccReport.enableInput);
    };

    var getSortColumnName = function (columnName) {
        var sortColumnName = null;
        switch (columnName) {
            case "ColVariance":
                sortColumnName = "VarianceIncome";
                break;
            case "ColVirtualWinLoss":
                sortColumnName = "VirtualWinLoss";
                break
            case "ColRealIncome":
                sortColumnName = "RealIncome";
                break;
            case "ColVirtualTurnOver":
                sortColumnName = "VirtualTurnover";
                break;
            case "ColRealTurnOver":
                sortColumnName = "RealTurnover";
                break;
            case "ColVirtualMargin":
                sortColumnName = "VirtualMargin";
                break;
            case "ColRealMargin":
                sortColumnName = "RealMargin";
                break;
        }

        return sortColumnName;
    };

    self.RenderDivision = function (rs) {
        var isSetDefaultCollapse = true;
        var id = "ddl-division";
        var siteGroups = [];
        var siteGroupLength = rs.length;
        for (var i = 0; i < siteGroupLength; i++) {
            var siteGroup = rs[i];
            var sites = [];
            var siteLength = siteGroup.Sites.length;
            for (var j = 0; j < siteLength; j++) {
                var site = siteGroup.Sites[j];
                sites.push({
                    id: id + "-" + site.SiteGroup + "-" + site.SiteName,
                    value: site.SiteName,
                    text: site.SiteName,
                    checked: true,
                    enable: true,
                    cssClass: ""
                });
            }

            if (sites.length > 0) {
                siteGroups.push({
                    id: id + "-group" + siteGroup.GroupName,
                    value: siteGroup.GroupName,
                    text: siteGroup.GroupName,
                    checked: true,
                    enable: true,
                    cssClass: "",
                    items: sites
                });
            }
        }

        _tables.ddlDivision.fanexMultiSelect({
            minWidth: 250,
            maxHeight: 450,
            showFilter: true,
            shadow: true,
            showHeader: true,
            isCheckAll: true,
            showFooter: false,
            checkAllText: 'All',
            isUsedCustomCheckAllText: true,
            useCustomCheckAllText: 'All',
            noneSelectedText: 'Select an option',
            noItemText: 'No division found.',
            disabled: false,
            listData: siteGroups,
            onCheck: function () {
            },
            onOpen: function () {
                if (isSetDefaultCollapse) {
                    $('#ddlDivisionCollapseButton').trigger('click')
                    isSetDefaultCollapse = false;
                }
            }
        });

        _tables.ddlDivision = $("#dropdown-division");
    };
}