SoccerReport_ByBetType = function () {
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
        soccerReportByBetTypeContainer: null,
        soccerReportByBetType: null,
        soccerReportByBetTypeTitle: null
    };

    self.CacheData = {
        parentController: null
    };
};

SoccerReport_ByBetType.prototype.InitPage = function (parentController) {
    this.CachedDOMElements();
    this.CacheData.parentController = parentController;
};

SoccerReport_ByBetType.prototype.CachedDOMElements = function () {
    this.Tables.soccerReportByBetTypeContainer = $("#soccerReportByBetTypeContainer");
    this.Tables.soccerReportByBetType = $('#soccerReportByBetType');
};

SoccerReport_ByBetType.prototype.GetReport = function (winlostdate,
    betTypeId,
    betTypeName,
    isLive,
    isHedge) {
    AccReport.disableInput();
    scoreInfoManager.ClosePopup();
    var self = this;
    self.Params.reportParams.Winlostdate = winlostdate;
    self.Params.reportParams.SportType = self.CacheData.parentController.CacheData.SportId;
    if (self.Params.reportParams.SportType == AccReportData.Constants.SportType.SoccerMinorBetType) {
        self.Params.reportParams.SportType = AccReportData.Constants.SportType.SoccerMajorBetType;
    }
    self.Params.reportParams.Bettype = betTypeId;
    self.Params.reportParams.BetTypeName = betTypeName;
    self.Params.reportParams.Liveindicator = isLive;
    self.Params.reportParams.IsHedge = isHedge;
    AccReport.utility.CallAjax(this,
        window.rootUrl + "Main/DailyWinLoss2h/GetReportByBetType",
        self.Params.reportParams,
        self.RenderReport,
        self.RenderReport,
        AccReport.enableInput);
};

SoccerReport_ByBetType.prototype.RenderReport = function (report) {
    var self = this;
    this.Tables.soccerReportByBetType.addClass("display-none");
    var bodyData = [];
    var footerData = [];
    var maxColIndex = 14;
    if (report && report.ChildsInfo != null && report.ChildsInfo.length > 0) {
        var liveIndicator = self.Params.reportParams.Liveindicator == false ? 0 : 1;
        var strHedge = self.Params.reportParams.IsHedge == false ? "" : "Hedge";
        var leagueGroups = report.ChildsInfo;
        var hasAwayTeamColumn = this.Params.reportParams.Bettype != AccReportData.Constants.BetType.Outright &&
            this.Params.reportParams.SportType != AccReportData.Constants.SportType.Financial;
        for (var i = 0; i < leagueGroups.length; i++) {
            var leagueGroup = leagueGroups[i];
            bodyData.push({
                Columns: [{ Name: "ColCode" },
                          { Name: "ColHomeTeamName", Value: leagueGroup.Name, MergeColNumber: 13, CellAlign: "left", CssClass: "bold"}],
                CssClass: 'freegrid-even'
            });

            var matchLength = leagueGroup.ChildsInfo.length;
            for (var j = 0; j < matchLength; j++) {
                var info = leagueGroup.ChildsInfo[j];
                var columns = [];
                var evenrow = (j % 2 == 0) ? '' : '';
                var matchCode = info.MatchCode;

                var isOutRightBettype = parseInt(this.Params.reportParams.Bettype) === AccReportData.Constants.BetType.Outright && this.Params.reportParams.BetTypeName === "Outright";
                var openScoreInfoEvent = "scoreInfoManager.GetReport('" + self.CacheData.parentController.SoccerReport_Summary.Params.reportParams.Winlostdate + "'," +
                    AccReportData.Constants.SportType.SoccerMajorBetType + ",'" +
                    self.CacheData.parentController.CacheData.SportName + "','" +
                    self.Params.reportParams.Bettype + "','" +
                    self.Params.reportParams.BetTypeName + "'," +
                    liveIndicator + ",'" +
                    strHedge + "','" +
                    leagueGroup.Id + "','" +
                    leagueGroup.Name + "'," +
                    info.Id + ",'" +
                    info.MatchCode + "','" +
                    info.HomeTeamId + "','" +
                    info.HomeTeamName + "','" +
                    info.AwayTeamId + "','" +
                    info.AwayTeamName + "'); return;"
                var scoreInfoLink = '<a class="report2h-link bold" title="' + viewResources.Show_Score_Information + '" onclick="' + openScoreInfoEvent + '">' + info.MatchCode + '</a>';
                var outRightMatchCode = '<b>' + info.MatchCode + '</b>';
                columns.push({ Name: "ColCode", Value: isOutRightBettype ? outRightMatchCode : scoreInfoLink });                

                if (!hasAwayTeamColumn) {
                    columns.push({ Name: "ColHomeTeamName", Value: info.HomeTeamName, MergeColNumber: 3, CellAlign: "center" });
                }
                else {
                    columns.push({ Name: "ColHomeTeamName", Value: info.HomeTeamName });
                    columns.push({ Name: "ColVs", Value: "-vs-" });
                    columns.push({ Name: "ColAwayTeamName", Value: info.AwayTeamName });
                }

                columns.push({ Name: "ColWinloss", Value: info.WinLoss });
                columns.push({ Name: "ColPosition", Value: info.Position });
                columns.push({ Name: "ColCommission", Value: info.Commission });
                columns.push({ Name: "ColRealLineTotal", Value: info.LineTotal });
                columns.push({ Name: "ColVirtualLineTotal", Value: info.VirtualLineTotal });
                columns.push({ Name: "ColVariance", Value: info.Variance });
                columns.push({ Name: "ColRealTurnover", Value: info.Turnover });
                columns.push({ Name: "ColVirtualTurnover", Value: info.VirtualTurnover });
                columns.push(reportManager.GetColumnTurnover(info.Margin, info.Turnover, "ColRealMargin"));
                columns.push(reportManager.GetColumnTurnover(info.VirtualMargin, info.VirtualTurnover, "ColVirtualMargin"));
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
                                { Name: "ColVirtualLineTotal", Value: info.VirtualLineTotal },
                                { Name: "ColVariance", Value: info.Variance },
                                { Name: "ColRealTurnover", Value: info.Turnover },
                                { Name: "ColVirtualTurnover", Value: info.VirtualTurnover },
                                reportManager.GetColumnTurnover(info.Margin, info.Turnover, "ColRealMargin"),
                                reportManager.GetColumnTurnover(info.VirtualMargin, info.VirtualTurnover, "ColVirtualMargin")]
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
            { Name: "ColVirtualLineTotal", Value: info.VirtualLineTotal },
            { Name: "ColVariance", Value: info.Variance },
            { Name: "ColRealTurnover", Value: info.Turnover },
            { Name: "ColVirtualTurnover", Value: info.VirtualTurnover },
            reportManager.GetColumnTurnover(info.Margin, info.Turnover, "ColRealMargin"),
            reportManager.GetColumnTurnover(info.VirtualMargin, info.VirtualTurnover, "ColVirtualMargin")]
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
        customizeHeader: true,
        groupHeaderRows: [{
            Columns: [{ HeaderText: viewResources.Code, Name: "ColCode", MergeRowNumber: 2 },
                        { HeaderText: viewResources.Event, Name: "ColHomeTeamName", MergeRowNumber: 2, MergeColNumber: 3 },
                        { HeaderText: viewResources.Member_WinLoss, Name: "ColWinloss", MergeRowNumber: 2 },
                        { HeaderText: viewResources.SMA_WinLoss, Name: "ColPosition", MergeRowNumber: 2 },
                        { HeaderText: viewResources.Commission, Name: "ColCommission", MergeRowNumber: 2 },
                        { HeaderText: viewResources.Company_Income, Name: "ColRealLineTotal", MergeColNumber: 2 },
                        { HeaderText: viewResources.Variance, Name: "ColVariance", MergeRowNumber: 2 },
                        { HeaderText: viewResources.Company_Turnover, Name: "ColRealTurnover", MergeColNumber: 2 },
                        { HeaderText: viewResources.Margin, Name: "ColRealMargin", MergeColNumber: 2}]
        }],

        columns: [{ HeaderText: viewResources.Code, Width: 120, Name: "ColCode", HeaderAlign: "center", CellAlign: "center" },
                    {
                        HeaderText: viewResources.Event,
                        Width: 200, Name: "ColHomeTeamName",
                        HeaderAlign: "center",
                        CellAlign: "right",
                        MergeHeaderColNumber: 2
                    },
                    { HeaderText: "", Width: 20, Name: "ColVs", HeaderAlign: "center", CellAlign: "center" },
                    { HeaderText: "", Width: 200, Name: "ColAwayTeamName", HeaderAlign: "center", CellAlign: "left" },
                    { HeaderText: viewResources.Member_WinLoss, Width: 100, Name: "ColWinloss", FormatNumber: FormatConstant.TWO_DECIMAL_NUMBER, HeaderAlign: "center", CellAlign: "right", FormatPositive: '$' },
                    { HeaderText: viewResources.SMA_WinLoss, Width: 80, Name: "ColPosition", FormatNumber: FormatConstant.TWO_DECIMAL_NUMBER, HeaderAlign: "center", CellAlign: "right" },
                    { HeaderText: viewResources.Commission, Width: 80, Name: "ColCommission", FormatNumber: FormatConstant.TWO_DECIMAL_NUMBER, HeaderAlign: "center", CellAlign: "right" },
                    { HeaderText: viewResources.Real, Width: 100, Name: "ColRealLineTotal", FormatNumber: FormatConstant.TWO_DECIMAL_NUMBER, HeaderAlign: "center", CellAlign: "right", FormatPositive: '$' },
                    { HeaderText: viewResources.Virtual, Width: 100, Name: "ColVirtualLineTotal", FormatNumber: FormatConstant.TWO_DECIMAL_NUMBER, HeaderAlign: "center", CellAlign: "right", FormatPositive: '$' },
                    { HeaderText: viewResources.Variance, Width: 80, Name: "ColVariance", FormatNumber: FormatConstant.TWO_DECIMAL_NUMBER, HeaderAlign: "center", CellAlign: "right", FormatPositive: '$' },
                    { HeaderText: viewResources.Real, Width: 100, Name: "ColRealTurnover", FormatNumber: FormatConstant.TWO_DECIMAL_NUMBER, HeaderAlign: "center", CellAlign: "right" },
                    { HeaderText: viewResources.Virtual, Width: 100, Name: "ColVirtualTurnover", FormatNumber: FormatConstant.TWO_DECIMAL_NUMBER, HeaderAlign: "center", CellAlign: "right" },
                    { HeaderText: viewResources.Real, Width: 100, Name: "ColRealMargin", FormatNumber: FormatConstant.TWO_DECIMAL_PERCENT_NUMBER, HeaderAlign: "center", CellAlign: "right", FormatPositive: '$' },
                    { HeaderText: viewResources.Virtual, Width: 100, Name: "ColVirtualMargin", HeaderAlign: "center", FormatNumber: FormatConstant.TWO_DECIMAL_PERCENT_NUMBER, CellAlign: "right", FormatPositive: '$'}],
        showTitle: true,
        showFooter: true,
        titleInfo: {
            text: "<a class='link' onclick='showHideReportManager.BackToMainReport();'>" + viewResources.Main + "</a>" +
                   " > <a class='link' onclick='reportManager.SoccerReport.ShowSummaryReport();'>" + self.CacheData.parentController.CacheData.SportName + "</a>" +
                   " > " + this.Params.reportParams.BetTypeName +
                           " \"" + (this.Params.reportParams.Liveindicator ?
                                       viewResources.Live :
                                       viewResources.Symbol_Sparse_Non_Live) + "\"",
            showExcelButton: false,
            exportExcelFunction: function () {
                self.ExportReport();
            }
        },
        bodyRows: bodyData,
        footerRows: footerData,
        numberOfFrozenColumn: 0,
        gridWidth: 1400,
        gridExpandHeight: 180,
        fullWidth: true,
        showDistinctRow: false,
        showHighlightRow: false,
        headerColumnHeight: 17,
        marginRight: 20,
        noDataText: viewResources.EmptyReportMessage,
        showHighlightRow: true
    };

    this.Tables.soccerReportByBetTypeContainer.removeClass("display-none");
    self.CacheData.parentController.SoccerReport_Summary.HideReport();
    self.CacheData.parentController.ShowReportView();
    this.Tables.soccerReportByBetType.removeClass("display-none").fanexGrid(gridOptions);
};

SoccerReport_ByBetType.prototype.ShowReport = function () {
    AccReport.popUp.hide();
    this.Tables.soccerReportByBetTypeContainer.removeClass("display-none");
};

SoccerReport_ByBetType.prototype.HideReport = function () {
    AccReport.popUp.hide();
    this.Tables.soccerReportByBetTypeContainer.addClass("display-none");
    reportManager.Tables.btnShowHideSiteTypeInfo.removeClass("display-none");
};

SoccerReport_ByBetType.prototype.ExportReport = function () {
    AccReport.downloadFile(window.rootUrl + "Main/DailyWinLoss2h/ExportSoccerReportByBetType", this.Params.reportParams);
};
