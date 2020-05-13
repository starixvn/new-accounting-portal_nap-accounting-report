

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
        bettransModeBtn: null
    };

    self.Params = {
        reportParams: {
            StartDate: null,
            EndDate: null,
            SportTypeList: null,
            IsNormalMode: true
        },

        isNormalMode: true
    };

    self.initPage = function () {
        cachedDOMElements();
        setDayValueForButtons();
        renderMonthPicker();
        renderDatePicker();
        getSport();
        hideWarehouseMode();
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

    var getSport = function () {
        AccReport.utility.CallAjax(this,
            window.rootUrl + "WinLoss/LeagueWinLoss/GetSport",
            null,
            self.RenderSport,
            self.RenderSport,
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

        });
    };

    self.btnGenerateClick = function () {
        getParams();
        if (!checkReportParams(self.Params.reportParams)) {
            AccReport.enableInput();
            return;
        }

        AccReport.disableInput();
        hideLeagueWinLossBySport();
        hideLeagueWinLossByBetType();

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
        hideLeagueWinLossBySport();
        hideLeagueWinLossByBetType();

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
        hideLeagueWinLossBySport();
        hideLeagueWinLossByBetType();

        AccReport.utility.CallAjax(this,
            window.rootUrl + "WinLoss/LeagueWinLoss/GetLeagueWinLoss",
            self.Params.reportParams,
            renderLeagueWinLoss,
            renderLeagueWinLoss,
            AccReport.enableInput,
            false);
    };

    var getParamByDate = function (date) {
        self.Params.reportParams.StartDate = date;
        self.Params.reportParams.EndDate = date;
        self.Params.reportParams.SportTypeList = _tables.ddlSport.multiSelect('getCheckedItems').toString();
        self.Params.reportParams.IsNormalMode = self.Params.isNormalMode;
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

        self.Params.reportParams.SportTypeList = _tables.ddlSport.multiSelect('getCheckedItems').toString();
        self.Params.reportParams.IsNormalMode = self.Params.isNormalMode;
    };

    var checkReportParams = function (params) {
        AccReport.popUp.removeHighLight();
        if (self.Params.reportParams.SportTypeList == '') {
            AccReport.popUp.show("Please select  at least one sport!", "dropdown-sport");
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
                    Columns: [{ Name: "ColLeague", Value: sport.SportName, MergeColNumber: 12 }],
                    SubRows: []
                };

                var childsLength = sport.Leagues.length;
                for (var j = 0; j < childsLength; j++) {
                    info = sport.Leagues[j];
                    currentSubRow =
                    {
                        Columns: [{
                            Name: "ColLeague", Value: {
                                sportId: info.SportType,
                                sportName: info.Sport,
                                leagueId: info.LeagueId,
                                leagueName: info.LeagueName
                            }, CssClass: "colLeague",
                            CustomCellRender: function (currentCell) {
                                var clickEvent = ' onclick = "viewLeagueWinLossBySport(' + currentCell.Value.sportId + ', ' + currentCell.Value.leagueId + ',\'' + currentCell.Value.sportName + '\',\'' + currentCell.Value.leagueName + "');\"";
                                var html = '<div class="link " ' + clickEvent + '>' + currentCell.Value.leagueName + '</div>';
                                return html;
                            }
                        },
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
                        CssClass: j % 2 == 0 ? "even" : ""
                    };

                    currentRow.SubRows.push(currentSubRow);
                }

                var sportTotalRow = mappingDataRow(sport.Total, "subTotal");
                currentRow.SubRows.push(sportTotalRow);
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
            columns: [{ HeaderText: "League", Width: 300, Name: "ColLeague", HeaderAlign: "center", CellAlign: "left", ShowCollapseExpand: true },
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
        AccReport.downloadFile(window.rootUrl + "WinLoss/LeagueWinLoss/ExportLeagueWinLoss", self.Params.reportParams);
    };

    self.RenderSport = function (rs) {
        var id = "ddl-sport";
        var sportGroups = [];
        var sportGroupLength = rs.length;
        for (var i = 0; i < sportGroupLength; i++) {
            var sportGroup = rs[i];
            var sports = [];
            var sportLength = sportGroup.ChildItems.length;
            for (var j = 0; j < sportLength; j++) {
                var sport = sportGroup.ChildItems[j];
                sports.push({
                    Id: id + "-" + sport.SportType + "-" + sport.SportName,
                    Value: sport.SportType,
                    Text: sport.SportName,
                    Checked: true,
                    Enable: true,
                    CssClass: "",
                    Items: []
                });
            }

            sportGroups.push({
                Id: id + "-group" + sportGroup.GroupId,
                Value: sportGroup.GroupId,
                Text: sportGroup.GroupName,
                Checked: true,
                Enable: true,
                CssClass: "",
                Items: sports
            });
        }

        _tables.ddlSport.multiSelect(
         {
             minWidth: 160,
             maxHeight: 450,
             shadow: true,
             showHeader: true,
             isCheckAll: true,
             showFooter: false,
             checkAllText: "All",
             isUsedCustomCheckAllText: true,
             customCheckAllText: "All",
             noItemText: "No sport found.",
             listData: sportGroups
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
}