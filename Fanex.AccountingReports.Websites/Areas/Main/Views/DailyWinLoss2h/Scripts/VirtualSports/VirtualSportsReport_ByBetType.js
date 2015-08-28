VirtualSportsReport_ByBetType = function () {
    var self = this;
    self.Params = {
        reportParams: {
            Winlostdate: null,
            SportType: null,
            Bettype: null,
            BetTypeName: null,
            Liveindicator: null,
            IsHedge: null
        }
    };

    self.Tables = {
        virtualSportReportByBetTypeContainer: null,
        virtualSportReportByBetType: null
    };

    self.CacheData = {
        parentController: null
    };
};

VirtualSportsReport_ByBetType.prototype.InitPage = function (parentController) {
    this.CachedDOMElements();
    this.CacheData.parentController = parentController;
};

VirtualSportsReport_ByBetType.prototype.CachedDOMElements = function () {
    this.Tables.virtualSportReportByBetTypeContainer = $("#virtualSportReportByBetTypeContainer");
    this.Tables.virtualSportReportByBetType = this.Tables.virtualSportReportByBetTypeContainer.find("#virtualSportReportByBetType");
};

VirtualSportsReport_ByBetType.prototype.GetReport = function (reportDate,
    betTypeId,
    betTypeName,
    isLive,
    isHedge) {
    var self = this;

    AccReport.disableInput();
    self.Params.reportParams.Winlostdate = reportDate;
    self.Params.reportParams.SportType = self.CacheData.parentController.VirtualSportsReport_BySportType.Params.reportParams.SportType;
    self.Params.reportParams.SportTypeName = self.CacheData.parentController.VirtualSportsReport_BySportType.Params.reportParams.SportTypeName;
    self.Params.reportParams.Liveindicator = isLive;
    self.Params.reportParams.IsHedge = isHedge;
    self.Params.reportParams.Bettype = betTypeId;
    self.Params.reportParams.BetTypeName = betTypeName;

    if (self.Params.reportParams.SportType == AccReportData.Constants.SportType.VirtualHorseRacing ||
        self.Params.reportParams.SportType == AccReportData.Constants.SportType.VirtualGreyhounds ||
        self.Params.reportParams.SportType == AccReportData.Constants.SportType.VirtualSpeedway ||
        self.Params.reportParams.SportType == AccReportData.Constants.SportType.VirtualMotorsports ||
        self.Params.reportParams.SportType == AccReportData.Constants.SportType.VirtualCycling ) {
        self.CacheData.parentController.BindingEvent_ByBetTypeReport_VirtualHorseRacingReport();
        self.CacheData.parentController.VirtualSports_VirtualHorseRacingReport.GetReport(self.Params.reportParams);
        return;
    }

    AccReport.utility.CallAjax(this,
        window.rootUrl + "Main/DailyWinLoss2h/GetReportByBetType",
        self.Params.reportParams,
        self.RenderReport,
        self.RenderReport,
        AccReport.enableInput);
};

VirtualSportsReport_ByBetType.prototype.RenderReport = function (report) {
    var self = this;
    var bodyData = [];
    var footerData = [];
    var maxColIndex = 10;

    if (report && report.ChildsInfo != null && report.ChildsInfo.length > 0) {
        var liveIndicator = self.Params.reportParams.Liveindicator == false ? 0 : 1;
        var strHedge = self.Params.reportParams.IsHedge == false ? "" : "Hedge";
        var leagueGroups = report.ChildsInfo;
        var hasAwayTeamColumn = this.Params.reportParams.Bettype == AccReportData.Constants.BetType.Outright ||
            this.Params.reportParams.SportType == AccReportData.Constants.SportType.Financial;

        for (var i = 0; i < leagueGroups.length; i++) {
            var leagueGroup = leagueGroups[i];
            bodyData.push({
                Columns: [{ Name: "ColCode" },
                          { Name: "ColHomeTeamName", Value: leagueGroup.Name, MergeColNumber: 9, CellAlign: "left", CssClass: "bold"}],
                CssClass: "freegrid-even"
            });

            var matchLength = leagueGroup.ChildsInfo.length;
            for (var j = 0; j < matchLength; j++) {
                var info = leagueGroup.ChildsInfo[j];
                var columns = [];
                var evenrow = (j % 2 == 0) ? '' : '';

                columns.push({ Name: "ColCode", Value: info.MatchCode });
                if (hasAwayTeamColumn) {
                    columns.push({ Name: "ColHomeTeamName", Value: info.HomeTeamName + (info.IsNeutral ? " " + viewResources.Symbol_NeutralSuffix : ""), MergeColNumber: 3 });
                }
                else {
                    columns.push({ Name: "ColHomeTeamName", Value: info.HomeTeamName + (info.IsNeutral ? " " + viewResources.Symbol_NeutralSuffix : "") });
                    columns.push({ Name: "ColVs", Value: "-vs-" });
                    columns.push({ Name: "ColAwayTeamName", Value: info.AwayTeamName });
                }

                columns.push({ Name: "ColWinloss", Value: info.WinLoss });
                columns.push({ Name: "ColPosition", Value: info.Position });
                columns.push({ Name: "ColCommission", Value: info.Commission });
                columns.push({ Name: "ColRealLineTotal", Value: info.LineTotal });
                columns.push({ Name: "ColRealTurnover", Value: info.Turnover });
                columns.push(reportManager.GetColumnTurnover(info.Margin, info.Turnover, "ColRealMargin"));
                bodyData.push({ Columns: columns, CssClass: evenrow });
            }

            if (matchLength >= 2) {
                info = leagueGroup.GrandTotal;
                bodyData.push({
                    Columns: [{ Name: "ColCode", CellAlign: "right", CssClass: "bold", MergeColNumber: 4 },
                                { Name: "ColWinloss", Value: info.WinLoss },
                                { Name: "ColPosition", Value: info.Position },
                                { Name: "ColCommission", Value: info.Commission },
                                { Name: "ColRealLineTotal", Value: info.LineTotal },
                                { Name: "ColRealTurnover", Value: info.Turnover },
                                reportManager.GetColumnTurnover(info.Margin, info.Turnover, "ColRealMargin")]
                });
            }
        }

        info = report.GrandTotal;
        footerData.push({
            Columns: [
                {
                    Name: "ColCode",
                    Value: (this.Params.reportParams.Liveindicator ? '<font color="red">!' + viewResources.Live + '</font>' : "") +
                            " " + info.Name,
                    CellAlign: "right",
                    CssClass: "bold",
                    MergeColNumber: 4
                },
                { Name: "ColWinloss", Value: info.WinLoss },
                { Name: "ColPosition", Value: info.Position },
                { Name: "ColCommission", Value: info.Commission },
                { Name: "ColRealLineTotal", Value: info.LineTotal },
                { Name: "ColRealTurnover", Value: info.Turnover },
                reportManager.GetColumnTurnover(info.Margin, info.Turnover, "ColRealMargin")]
        });

        var winlossRM = report.EightRMTotal.WinLoss +
            report.BARMTotal.WinLoss;
        var turnoverRM = report.EightRMTotal.Turnover + report.BARMTotal.Turnover;
        var virtualTurnoverRM = report.EightRMTotal.VirtualTurnover + report.BARMTotal.VirtualTurnover;
        var lineTotalRM = report.EightRMTotal.LineTotal + report.BARMTotal.LineTotal;
        var virtualLineTotalRM = report.EightRMTotal.VirtualLineTotal + report.BARMTotal.VirtualLineTotal;
        var marginRM = turnoverRM == 0 ? viewResources.NotAvailable : lineTotalRM / turnoverRM;
        var virtualMarginRM = virtualTurnoverRM == 0 ? viewResources.NotAvailable : virtualLineTotalRM / virtualTurnoverRM;
        if (winlossRM != 0) {
            footerData.push({
                Columns: [{ Name: "ColCode", Value: reportManager.GetIBCTotalLabel(), CellAlign: "right", CssClass: "bold", MergeColNumber: 4 },
                            { Name: "ColWinloss", Value: winlossRM, CssClass: "bold" },
                            { Name: "ColPosition", Value: report.EightRMTotal.Position + report.BARMTotal.Position },
                            { Name: "ColCommission", Value: report.EightRMTotal.Commission + report.BARMTotal.Commission },
                            { Name: "ColRealLineTotal", Value: lineTotalRM },
                            { Name: "ColVirtualLineTotal", Value: virtualLineTotalRM },
                            { Name: "ColVariance", Value: report.EightRMTotal.Variance + report.BARMTotal.Variance },
                            { Name: "ColRealTurnover", Value: turnoverRM },
                            { Name: "ColVirtualTurnover", Value: virtualTurnoverRM },
                            { Name: "ColRealMargin", Value: marginRM },
                            { Name: "ColVirtualMargin", Value: virtualMarginRM}]
            });
        }
    }

    var gridOptions = {
        columns: [{ HeaderText: viewResources.Code, Width: 120, Name: "ColCode", HeaderAlign: "center", CellAlign: "center" },
                    {
                        HeaderText: viewResources.Event,
                        Width: 200, Name: "ColHomeTeamName",
                        HeaderAlign: "center",
                        CellAlign: "right",
                        MergeHeaderColNumber: 3
                    },
                    { HeaderText: "", Width: 20, Name: "ColVs", HeaderAlign: "center", CellAlign: "center" },
                    { HeaderText: "", Width: 200, Name: "ColAwayTeamName", HeaderAlign: "center", CellAlign: "left" },
                    { HeaderText: viewResources.Member_WinLoss, Width: 100, Name: "ColWinloss", FormatNumber: FormatConstant.TWO_DECIMAL_NUMBER, HeaderAlign: "center", CellAlign: "right", FormatPositive: '$' },
                    { HeaderText: viewResources.SMA_WinLoss, Width: 100, Name: "ColPosition", FormatNumber: FormatConstant.TWO_DECIMAL_NUMBER, HeaderAlign: "center", CellAlign: "right" },
                    { HeaderText: viewResources.Commission, Width: 100, Name: "ColCommission", FormatNumber: FormatConstant.TWO_DECIMAL_NUMBER, HeaderAlign: "center", CellAlign: "right" },
                    { HeaderText: viewResources.Company_Income, Width: 100, Name: "ColRealLineTotal", FormatNumber: FormatConstant.TWO_DECIMAL_NUMBER, HeaderAlign: "center", CellAlign: "right", FormatPositive: '$' },
                    { HeaderText: viewResources.Company_Turnover, Width: 100, Name: "ColRealTurnover", FormatNumber: FormatConstant.TWO_DECIMAL_NUMBER, HeaderAlign: "center", CellAlign: "right" },
                    { HeaderText: viewResources.Margin, Width: 100, Name: "ColRealMargin", FormatNumber: FormatConstant.TWO_DECIMAL_PERCENT_NUMBER, HeaderAlign: "center", CellAlign: "right", FormatPositive: '$'}],
        showTitle: true,
        showFooter: true,
        titleInfo: {
            text: "<a class='breadcrumbs-link' onclick='showHideReportManager.BackToMainReport();'>" + viewResources.Main + "</a> >" +
                  " <a class='breadcrumbs-link' onclick='reportManager.VirtualSportReport.ShowSummaryReport();'>" + self.CacheData.parentController.CacheData.SportName + "</a> |" +
                  " <a class='breadcrumbs-link' onclick='reportManager.VirtualSportReport.ShowReportBySportType();'>" + self.CacheData.parentController.VirtualSportsReport_BySportType.Params.reportParams.SportTypeName + "</a> >" +
                  " " + self.Params.reportParams.BetTypeName,
            showExcelButton: false,
            exportExcelFunction: function () {
                self.ExportReport();
            }
        },
        bodyRows: bodyData,
        footerRows: footerData,
        numberOfFrozenColumn: 0,
        gridWidth: 1400,
        gridExpandHeight: 200,
        fullWidth: true,
        showDistinctRow: false,
        showHighlightRow: false,
        headerColumnHeight: 29,
        noDataText: viewResources.EmptyReportMessage,
        showHighlightRow: true
    };

    this.Tables.virtualSportReportByBetTypeContainer.removeClass("display-none");
    this.CacheData.parentController.VirtualSportsReport_BySportType.HideReport();
    self.CacheData.parentController.ShowReportView();
    this.Tables.virtualSportReportByBetType.removeClass("display-none").fanexGrid(gridOptions);
};

VirtualSportsReport_ByBetType.prototype.ShowReport = function () {
    AccReport.popUp.hide();
    this.Tables.virtualSportReportByBetTypeContainer.removeClass("display-none");
};

VirtualSportsReport_ByBetType.prototype.HideReport = function () {
    AccReport.popUp.hide();
    this.Tables.virtualSportReportByBetTypeContainer.addClass("display-none");
};

VirtualSportsReport_ByBetType.prototype.ExportReport = function () {
    AccReport.downloadFile(window.rootUrl + "Main/DailyWinLoss2h/ExportVirtualSportReportByBetType", this.Params.reportParams);
}; 