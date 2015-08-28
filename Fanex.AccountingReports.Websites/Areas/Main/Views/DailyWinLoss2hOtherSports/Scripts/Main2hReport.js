Main2hReport = function () {
    var self = this;

    self.Tables = {
        reportContainer: null,
        reportTable: null,
        reportTableBody: null
    };

    self.Params = {
        reportParams: {
            Winlostdate: null
        }
    };
};

Main2hReport.prototype.InitPage = function () {
    this.CachedDOMElements();
};

Main2hReport.prototype.GetReport = function (reportDate) {
    var self = this;

    AccReport.disableInput();
    reportManager.Tables.btnShowHideSiteTypeInfo.attr("disabled", null);
    self.GetParams(reportDate);
    AccReport.utility.CallAjax(this,
        window.rootUrl + "Main/DailyWinLoss2hOtherSports/GetMain2hReport",
        self.Params.reportParams,
        self.RenderReport,
        self.RenderReport,
        AccReport.enableInput);
};

Main2hReport.prototype.CachedDOMElements = function () {
    this.Tables.reportContainer = $('#dailyWinloss2hOthersSportContainer');
    this.Tables.report = $('#dailyWinloss2hOthersSport');
};

Main2hReport.prototype.GetParams = function (reportDate) {
    this.Params.reportParams.Winlostdate = reportDate;
};

Main2hReport.prototype.RenderReport = function (report) {
    var self = this;
    this.Tables.reportContainer.removeClass("display-none");
    this.Tables.report.addClass("display-none");
    var bodyData = [];
    var footerData = [];
    var maxColIndex = 11;
    if (report && report.ChildsInfo && report.ChildsInfo.length > 0) {
        var sportLength = report.ChildsInfo.length;
        var info = null;
        var tableId = "main2hReport";
        var groupdId = '';

        for (var i = 0; i < sportLength; i++) {
            var sportWinLoss = report.ChildsInfo[i];
            info = sportWinLoss.NormalTotal;
            groupdId = tableId + "_Product0" + "_Sport" + sportWinLoss.Id;
            bodyData.push({
                Columns: [
                    {
                        Name: "ColSportType",
                        Value: showHideSiteTypeManager.GetShowHideSiteTypeIcon(groupdId) +
                            this.GetSportLink(sportWinLoss)
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
                    reportManager.GetColumnTurnover(info.VirtualMargin, info.VirtualTurnover, "ColVirtualMargin")],
                CssClass: "freegrid-even"
            });

            info = sportWinLoss.CreditTotal;
            bodyData.push({
                Columns: [{ Name: "ColSportType", Value: info.Name, CellAlign: "right", CssClass: "bold" },
                            { Name: "ColWinloss", Value: info.WinLoss },
                            { Name: "ColPosition", Value: info.Position },
                            { Name: "ColCommission", Value: info.Commission },
                            { Name: "ColRealLineTotal", Value: info.LineTotal },
                            { Name: "ColVirtualLineTotal", Value: info.VirtualLineTotal },
                            { Name: "ColVariance", Value: info.Variance },
                            { Name: "ColRealTurnover", Value: info.Turnover },
                            { Name: "ColVirtualTurnover", Value: info.VirtualTurnover },
                            reportManager.GetColumnTurnover(info.Margin, info.Turnover, "ColRealMargin"),
                            reportManager.GetColumnTurnover(info.VirtualMargin, info.VirtualTurnover, "ColVirtualMargin")],
                CssClass: showHideSiteTypeManager.GetSiteTypeRowCss(groupdId,
                    showHideSiteTypeManager.Enums.RowType.InfoRow)
            });

            info = sportWinLoss.DepositTotal;
            bodyData.push({
                Columns: [{ Name: "ColSportType", Value: info.Name, CellAlign: "right", CssClass: "bold" },
                            { Name: "ColWinloss", Value: info.WinLoss },
                            { Name: "ColPosition", Value: info.Position },
                            { Name: "ColCommission", Value: info.Commission },
                            { Name: "ColRealLineTotal", Value: info.LineTotal },
                            { Name: "ColVirtualLineTotal", Value: info.VirtualLineTotal },
                            { Name: "ColVariance", Value: info.Variance },
                            { Name: "ColRealTurnover", Value: info.Turnover },
                            { Name: "ColVirtualTurnover", Value: info.VirtualTurnover },
                            reportManager.GetColumnTurnover(info.Margin, info.Turnover, "ColRealMargin"),
                            reportManager.GetColumnTurnover(info.VirtualMargin, info.VirtualTurnover, "ColVirtualMargin")],
                CssClass: showHideSiteTypeManager.GetSiteTypeRowCss(groupdId,
                    showHideSiteTypeManager.Enums.RowType.InfoRow)
            });

            if (is8RMActive == 'true' && sportWinLoss.EightRMTotal.LineTotal) {
                info = sportWinLoss.EightRMTotal;
                bodyData.push({
                    Columns: [{ Name: "ColSportType", Value: info.Name, CellAlign: "right", CssClass: "bold" },
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

            if (isBARMActive == 'true' && sportWinLoss.BARMTotal.LineTotal) {
                info = sportWinLoss.BARMTotal;
                bodyData.push({
                    Columns: [{ Name: "ColSportType", Value: info.Name, CellAlign: "right", CssClass: "bold" },
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

        bodyData.push({
            Columns: [{ Name: "ColSportType", MergeColNumber: maxColIndex}]
        });

        info = report.GrandTotal;
        footerData.push({
            Columns: [{ Name: "ColSportType", Value: info.Name, CellAlign: "right" },
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
            Columns: [{ HeaderText: viewResources.Sport_Type, Name: "ColSportType", MergeRowNumber: 2 },
                      { HeaderText: viewResources.Member_WinLoss, Name: "ColWinloss", MergeRowNumber: 2 },
                      { HeaderText: viewResources.SMA_WinLoss, Name: "ColPosition", MergeRowNumber: 2 },
                      { HeaderText: viewResources.Commission, Name: "ColCommission", MergeRowNumber: 2 },
                      { HeaderText: viewResources.Company_Income, Name: "ColRealLineTotal", MergeColNumber: 2 },
                      { HeaderText: viewResources.Variance, Name: "ColVariance", MergeRowNumber: 2 },
                      { HeaderText: viewResources.Company_Turnover, Name: "ColRealTurnover", MergeColNumber: 2 },
                      { HeaderText: viewResources.Margin, Name: "ColRealMargin", MergeColNumber: 2}]
        }],

        columns: [
            {
                HeaderText: viewResources.Sport_Type,
                Width: 180, Name: "ColSportType",
                HeaderAlign: "center",
                CellAlign: "left"
            },
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
            text: viewResources.DailyWinLoss2h_OtherSportReport_MainReport_Title,
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
        showDistinctRow: false,
        showHighlightRow: false,
        noDataText: viewResources.EmptyReportMessage,
        headerColumnHeight: 17
    };

    this.Tables.report.removeClass("display-none").fanexGrid(gridOptions);
    this.Tables.report.find(".showHideSiteType.showHideIcon").click(function () {
        var btnIcon = $(this);
        showHideSiteTypeManager.ShowHideSiteTypeInfo_ByGroup(btnIcon, self.Tables.report);
    });
};

Main2hReport.prototype.ExportReport = function () {
    AccReport.downloadFile(window.rootUrl + "Main/DailyWinLoss2hOtherSports/ExportMain2hReport", this.Params.reportParams);
};

Main2hReport.prototype.GetSportLink = function (winLossInfo) {
    var sportLink = '';
    switch (winLossInfo.Id) {
        case AccReportData.Constants.SportType._2hReport_OtherSports:
            sportLink = "<span class='report2h-link bold' onclick=\"showHideReportManager.ShowOtherSportReport('" +
                            winLossInfo.Id + "','" +
                            winLossInfo.Name + "','" +
                            reportManager.Main2hReport.Params.reportParams.Winlostdate + "'); return;\">" +
                            winLossInfo.Name + '</span>';
            break;
        default:
            sportLink = "<div class='bold'>" + winLossInfo.Name + '</div>';
    }

    return sportLink;
};

Main2hReport.prototype.ShowHideSiteTypeInfo = function () {
    this.Tables.report.removeClass("display-none").fanexGrid("resizeGrid");
};