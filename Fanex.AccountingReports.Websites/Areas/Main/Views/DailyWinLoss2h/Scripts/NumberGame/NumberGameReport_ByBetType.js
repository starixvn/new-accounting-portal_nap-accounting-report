NumberGameReport_ByBetType = function () {
    var self = this;
    self.Params = {
        reportParams: {
            Winlostdate: null,
            SportType: null,
            BetType: null,
            BetTypeName: null,
            LiveIndicator: null,
            IsHedge: null
        }
    };

    self.Tables = {
        numberGameReportByBetTypeContainer: null,
        numberGameReportByBetType: null,
        numberReportByBetTypeTitle: null
    };

    self.CacheData = {
        parentController: null
    };
};

NumberGameReport_ByBetType.prototype.InitPage = function (parentController) {
    this.CachedDOMElements();
    this.CacheData.parentController = parentController;
};

NumberGameReport_ByBetType.prototype.CachedDOMElements = function () {
    this.Tables.numberGameReportByBetTypeContainer = $(".numberGameReportByBetTypeContainer");
    this.Tables.numberGameReportByBetType = $('.numberGameReportByBetType');
};

NumberGameReport_ByBetType.prototype.GetReport = function (winlostdate,
    betTypeId,
    betTypeName,
    isLive,
    isHedge) {
    AccReport.disableInput();

    var self = this;
    self.Params.reportParams.Winlostdate = winlostdate;
    self.Params.reportParams.SportType = self.CacheData.parentController.CacheData.SportId;
    self.Params.reportParams.BetType = betTypeId;
    self.Params.reportParams.BetTypeName = betTypeName;
    self.Params.reportParams.LiveIndicator = isLive;
    self.Params.reportParams.IsHedge = isHedge;

    AccReport.utility.CallAjax(this,
        window.rootUrl + "Main/DailyWinLoss2h/GetReportByBetType",
        self.Params.reportParams,
        self.RenderReport,
        self.RenderReport,
        AccReport.enableInput);
};

NumberGameReport_ByBetType.prototype.RenderReport = function (report) {
    var self = this;
    this.Tables.numberGameReportByBetTypeContainer.addClass("display-none");
    var bodyData = [];
    var footerData = [];
    var maxColIndex = 11;
    if (report && report.ChildsInfo != null && report.ChildsInfo.length > 0) {
        var leagueGroups = report.ChildsInfo;
        for (var i = 0; i < leagueGroups.length; i++) {
            var leagueGroup = leagueGroups[i];
            bodyData.push({
                Columns: [{ Name: "ColCode" },
                          { Name: "ColWinloss", Value: leagueGroup.Name, MergeColNumber: 10, CellAlign: "left", CssClass: "bold"}],
                CssClass: 'freegrid-even'
            });

            var matchLength = leagueGroup.ChildsInfo.length;
            for (var j = 0; j < matchLength; j++) {
                var info = leagueGroup.ChildsInfo[j];
                var columns = [];
                columns.push({ Name: "ColCode", Value: info.MatchCode });
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
                bodyData.push({ Columns: columns });
            }

            if (matchLength >= 2) {
                info = leagueGroup.GrandTotal;
                bodyData.push({
                    Columns: [{ Name: "ColCode", CellAlign: "right", CssClass: "bold" },
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
                    Value: (this.Params.reportParams.LiveIndicator ? '<font color="red">!' + viewResources.Live + '</font>' : "") +
                        " " + info.Name,
                    CellAlign: "right"
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
    }

    var gridOptions = {
        customizeHeader: true,
        groupHeaderRows: [{
            Columns: [{ HeaderText: viewResources.Code, Name: "ColCode", MergeRowNumber: 2 },
                        { HeaderText: viewResources.Member_WinLoss, Name: "ColWinloss", MergeRowNumber: 2 },
                        { HeaderText: viewResources.SMA_WinLoss, Name: "ColPosition", MergeRowNumber: 2 },
                        { HeaderText: viewResources.Commission, Name: "ColCommission", MergeRowNumber: 2 },
                        { HeaderText: viewResources.Company_Income, Name: "ColRealLineTotal", MergeColNumber: 2 },
                        { HeaderText: viewResources.Variance, Name: "ColVariance", MergeRowNumber: 2 },
                        { HeaderText: viewResources.Company_Turnover, Name: "ColRealTurnover", MergeColNumber: 2 },
                        { HeaderText: viewResources.Margin, Name: "ColRealMargin", MergeColNumber: 2}]
        }],

        columns: [{ HeaderText: viewResources.Code, Width: 160, Name: "ColCode", HeaderAlign: "center", CellAlign: "center" },
                    { HeaderText: viewResources.Member_WinLoss, Width: 100, Name: "ColWinloss", FormatNumber: FormatConstant.TWO_DECIMAL_NUMBER, HeaderAlign: "center", CellAlign: "right", FormatPositive: '$' },
                    { HeaderText: viewResources.SMA_WinLoss, Width: 100, Name: "ColPosition", FormatNumber: FormatConstant.TWO_DECIMAL_NUMBER, HeaderAlign: "center", CellAlign: "right" },
                    { HeaderText: viewResources.Commission, Width: 100, Name: "ColCommission", FormatNumber: FormatConstant.TWO_DECIMAL_NUMBER, HeaderAlign: "center", CellAlign: "right" },
                    { HeaderText: viewResources.Real, Width: 100, Name: "ColRealLineTotal", FormatNumber: FormatConstant.TWO_DECIMAL_NUMBER, HeaderAlign: "center", CellAlign: "right", FormatPositive: '$' },
                    { HeaderText: viewResources.Virtual, Width: 100, Name: "ColVirtualLineTotal", FormatNumber: FormatConstant.TWO_DECIMAL_NUMBER, HeaderAlign: "center", CellAlign: "right", FormatPositive: '$' },
                    { HeaderText: viewResources.Variance, Width: 100, Name: "ColVariance", FormatNumber: FormatConstant.TWO_DECIMAL_NUMBER, HeaderAlign: "center", CellAlign: "right", FormatPositive: '$' },
                    { HeaderText: viewResources.Real, Width: 100, Name: "ColRealTurnover", FormatNumber: FormatConstant.TWO_DECIMAL_NUMBER, HeaderAlign: "center", CellAlign: "right" },
                    { HeaderText: viewResources.Virtual, Width: 100, Name: "ColVirtualTurnover", FormatNumber: FormatConstant.TWO_DECIMAL_NUMBER, HeaderAlign: "center", CellAlign: "right" },
                    { HeaderText: viewResources.Real, Width: 100, Name: "ColRealMargin", FormatNumber: FormatConstant.TWO_DECIMAL_PERCENT_NUMBER, HeaderAlign: "center", CellAlign: "right", FormatPositive: '$' },
                    { HeaderText: viewResources.Virtual, Width: 100, Name: "ColVirtualMargin", HeaderAlign: "center", FormatNumber: FormatConstant.TWO_DECIMAL_PERCENT_NUMBER, CellAlign: "right", FormatPositive: '$'}],
        showTitle: true,
        showFooter: true,
        titleInfo: {
            text: "<a class='link' onclick='showHideReportManager.BackToMainReport();'>" + viewResources.Main + "</a>" +
                   " > <a class='link' onclick='reportManager.NumberGameReport.ShowSummaryReport();'>" + self.CacheData.parentController.CacheData.SportName + "</a>" +
                   " > " + this.Params.reportParams.BetTypeName +
                           " \"" + (this.Params.reportParams.LiveIndicator ?
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
        gridExpandHeight: 168,
        fullWidth: true,
        showDistinctRow: false,
        showHighlightRow: false,
        headerColumnHeight: 17,
        noDataText: viewResources.EmptyReportMessage,
        showHighlightRow: true
    };

    this.Tables.numberGameReportByBetTypeContainer.removeClass("display-none");
    self.CacheData.parentController.NumberGameReport_Summary.HideReport();
    self.CacheData.parentController.ShowReportView();
    this.Tables.numberGameReportByBetType.removeClass("display-none").fanexGrid(gridOptions);
};

NumberGameReport_ByBetType.prototype.ShowReport = function () {
    this.Tables.numberGameReportByBetTypeContainer.removeClass("display-none");
};

NumberGameReport_ByBetType.prototype.HideReport = function () {
    this.Tables.numberGameReportByBetTypeContainer.addClass("display-none");
    reportManager.Tables.btnShowHideSiteTypeInfo.removeClass("display-none");
}

NumberGameReport_ByBetType.prototype.ExportReport = function () {
    AccReport.downloadFile(window.rootUrl + "Main/DailyWinLoss2h/ExportNumberGameReportByBetType", this.Params.reportParams);
};