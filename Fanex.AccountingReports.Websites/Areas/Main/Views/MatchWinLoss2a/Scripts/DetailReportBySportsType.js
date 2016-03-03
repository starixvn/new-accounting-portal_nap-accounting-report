Namespace.Register("Fanex.AccountingReports.Main.MatchWinLoss2a.DetailReportBySportsType");
Fanex.AccountingReports.Main.MatchWinLoss2a.DetailReportBySportsType = function () {
    var self = this;

    var _tables = {
        reportContainer: null,
        report: null
    };

    var _params = {
        reportDetailParams: {
            Winlostdate: null,
            Sporttype: null,
            Bettype: null,
            BetTypeName: '',
            LiveIndicator: null
        }
    };

    var _cacheData = {
    };

    self.initPage = function () {
        cachedDOMElements();
    };

    var cachedDOMElements = function () {
        _tables.reportParentContainer = $("#parentContainer");
        _tables.reportContainer = $('#detailReportBySportsTypeContainer');
        _tables.report = $('#detaiReportBySportsType');
    };

    self.getReport = function (params) {
        AccReport.disableInput();
        _params.reportDetailParams = params;

        AccReport.utility.CallAjax(
            this,
            window.rootUrl + "Main/MatchWinLoss2a/GetDetailReportBySportsType",
            _params.reportDetailParams,
            function (rs) {
                self.renderReport(rs);
            },
            function (jqXHR, event, ajaxSettings) {
            },
            AccReport.enableInput);
    };

    self.getReportBeforeBalanceUpDay = function (params) {
        AccReport.disableInput();
        _params.reportDetailParams = params;

        AccReport.utility.CallAjax(
            this,
            window.rootUrl + "Main/MatchWinLoss2a/GetDetailReportBySportsTypeBeforeBalanceUpDay",
            _params.reportDetailParams,
            function (rs) {
                self.renderReport(rs);
            },
            function (jqXHR, event, ajaxSettings) {
            },
            AccReport.enableInput);
    };

    self.renderReport = function (report) {
        _tables.reportContainer.removeClass("display-none");
        _tables.report.addClass("display-none");
        var bodyData = [];
        var footerData = [];

        if (report && report.ChildsInfo.length > 0) {
            var betTypeGroupLength = report.ChildsInfo.length;
            var currentRow = null;
            var info = null;
            for (var betTypeGroupIndex = 0; betTypeGroupIndex < betTypeGroupLength; betTypeGroupIndex++) {
                betType = report.ChildsInfo[betTypeGroupIndex];
                betTypeLength = betType.ChildsInfo.length;
                for (var betTypeIndex = 0; betTypeIndex < betTypeLength; betTypeIndex++) {
                    var info = betType.ChildsInfo[betTypeIndex];
                    var homeName = info.HomeTeamName + (info.IsNeutral ? ' (N)' : '');
                    currentRow =
                        {
                            Columns: [{ Name: "ColBetType", Value: getBetTypeNameWithMatchCode(info.LiveIndicatorId, info.Name, info.MatchCode) },
                                      { Name: "ColHomeTeamName", Value: homeName },
                                      { Name: "ColVs", Value: ' -vs- ' },
                                      { Name: "ColAwayTeamName", Value: info.AwayTeamName },
                                      { Name: "ColWinLoss", Value: info.WinLoss_Str },
                                      { Name: "ColPosition", Value: info.Position_Str },
                                      { Name: "ColCommision", Value: info.Commission_Str },
                                      { Name: "ColVirtualComm", Value: info.VirtualComm_Str },
                                      { Name: "ColLineTotal", Value: info.LineTotal_Str, CssClass: "bold" },
                                      { Name: "ColVirtualLineTotal", Value: info.VirtualLineTotal_Str, CssClass: "bold" },
                                      { Name: "ColVariance", Value: info.Variance_Str },
                                      { Name: "ColTurnover", Value: info.Turnover_Str },
                                      { Name: "ColMargin", Value: info.Margin_Str}]
                        };

                    bodyData.push(currentRow);
                }
                info = betType.GrandTotal;
                currentRow =
                    {
                        Columns: [{ Name: "ColBetType", Value: getTotalBetTypeTitle(betType.LiveIndicatorId, betType.Name), MergeColNumber: 6, CellAlign: "right", TdClass: "subTotal bold" },
                                    { Name: "ColCommision", Value: info.Commission_Str, TdClass: "subTotal bold" },
                                    { Name: "ColVirtualComm", Value: info.VirtualComm_Str, TdClass: "subTotal bold" },
                                    { Name: "ColLineTotal", Value: info.LineTotal_Str, TdClass: "subTotal bold" },
                                    { Name: "ColVirtualLineTotal", Value: info.VirtualLineTotal_Str, TdClass: "subTotal bold" },
                                    { Name: "ColVariance", Value: info.Variance_Str, TdClass: "subTotal bold" },
                                    { Name: "ColTurnover", Value: info.Turnover_Str, TdClass: "subTotal bold" },
                                    { Name: "ColMargin", Value: info.Margin_Str, TdClass: "subTotal bold"}]
                    };
                bodyData.push(currentRow);
            }

            info = report.GrandTotal;
            currentRow =
            {
                Columns: [{ Name: "ColBetType", Value: info.Name, MergeColNumber: 6, CellAlign: "right" },
                          { Name: "ColCommision", Value: info.Commission_Str },
                          { Name: "ColVirtualComm", Value: info.VirtualComm_Str },
                          { Name: "ColLineTotal", Value: info.LineTotal_Str },
                          { Name: "ColVirtualLineTotal", Value: info.VirtualLineTotal_Str },
                          { Name: "ColVariance", Value: info.Variance_Str },
                          { Name: "ColTurnover", Value: info.Turnover_Str },
                          { Name: "ColMargin", Value: info.Margin_Str}]
            };

            footerData.push(currentRow);
        }

        var gridOptions = {
            columns: [{ HeaderText: "Bet Type", Width: 120, Name: "ColBetType", HeaderAlign: "center", CellAlign: "center" },
                      { HeaderText: "Match", Width: 100, Name: "ColHomeTeamName", HeaderAlign: "center", CellAlign: "right", MergeHeaderColNumber: 3 },
                      { HeaderText: "", Width: 20, Name: "ColVs", HeaderAlign: "center", CellAlign: "center" },
                      { HeaderText: "", Width: 100, Name: "ColAwayTeamName", HeaderAlign: "center", CellAlign: "left" },
                      { HeaderText: "Win Loss", Width: 80, Name: "ColWinLoss", HeaderAlign: "center", CellAlign: "right" },
                      { HeaderText: "Position", Width: 80, Name: "ColPosition", HeaderAlign: "center", CellAlign: "right" },
                      { HeaderText: "Comm", Width: 100, Name: "ColCommision", HeaderAlign: "center", CellAlign: "right" },
                      { HeaderText: "Virtual Comm", Width: 80, Name: "ColVirtualComm", HeaderAlign: "center", CellAlign: "right" },
                      { HeaderText: "Line Total", Width: 80, Name: "ColLineTotal", HeaderAlign: "center", CellAlign: "right" },
                      { HeaderText: "Virtual Line Total", Width: 80, Name: "ColVirtualLineTotal", HeaderAlign: "center", CellAlign: "right" },
                      { HeaderText: "Variance", Width: 80, Name: "ColVariance", HeaderAlign: "center", CellAlign: "right" },
                      { HeaderText: "Turnover", Width: 100, Name: "ColTurnover", HeaderAlign: "center", CellAlign: "right" },
                      { HeaderText: "Margin%", Width: 60, Name: "ColMargin", HeaderAlign: "center", CellAlign: "right"}],
            headerColumnHeight: 30,
            showTitle: true,
            showFooter: true,
            titleInfo:
            {
                text: (_params.reportDetailParams.Sporttype + ' PRODUCT ' +
                      getBetTypeName(_params.reportDetailParams.LiveIndicator, _params.reportDetailParams.BetTypeName) +
                      ' BETTYPE WIN/LOSS REPORT FOR ' + moment(new Date(_params.reportDetailParams.Winlostdate)).format(FormatConstant.MMM_DD_YYYY_STRING_MOMENT)).toUpperCase(),
                showExcelButton: true,
                exportExcelFunction: function () {
                    exportDetailReportBySportsType();
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
            gridExpandHeight: 450,
            fullWidth: false,
            gridWidth: 1000,
            numberOfFrozenColumn: 0
        };

        _tables.report.removeClass("display-none").fanexGrid(gridOptions);
        _tables.reportContainer.dialog({
            title: "",
            autoOpen: false,
            width: "auto",
            modal: false,
            resizable: false,
            close: function (event, ui) {
                _tables.reportContainer.dialog("destroy").remove();
                _tables.reportParentContainer.append('<div id="detailReportBySportsTypeContainer" class="display-none"> \
                                                        <div id="detaiReportBySportsType" class="freegrid display-none"> \
                                                        </div> \
                                                      </div>');
                _tables.reportContainer = $('#detailReportBySportsTypeContainer');
                _tables.report = $('#detaiReportBySportsType');
            }
        });

        _tables.reportContainer.dialog("open");

    };

    var getBetTypeName = function (liveType, bettypeName, matchCode) {
        var displayBetTypeName = '';
        if (liveType == true) {
            displayBetTypeName = 'LIVE-';
        }

        displayBetTypeName += bettypeName;
        return displayBetTypeName;
    };

    var getTotalBetTypeTitle = function (liveType, bettypeName) {
        var displayBetTypeName = '';
        if (liveType == true) {
            displayBetTypeName = '!LIVE-';
        }

        displayBetTypeName += bettypeName + " " + totalTitle;
        return displayBetTypeName;
    };

    var getBetTypeNameWithMatchCode = function (liveType, bettypeName, matchCode) {
        var displayBetTypeName = '';
        if (liveType == true) {
            displayBetTypeName = '<span class="bold">!LIVE - </span>';
        }

        displayBetTypeName += bettypeName;

        if (matchCode != '') {
            displayBetTypeName += " " + matchCode;
        }

        return displayBetTypeName;
    };

    self.closeDialog = function () {
        try {
            _tables.reportContainer.dialog("close");
        }
        catch (e) {
        }
    };

    exportDetailReportBySportsType = function () {
        AccReport.downloadFile(window.rootUrl + "Main/MatchWinLoss2a/ExportDetailReportBySportsType", _params.reportDetailParams);
    };
}