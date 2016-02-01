
var GrandTotalCalculator = function (report) {
    var reportLength = report.ChildsInfo.length;
    var dataTotal = [0, 0, 0, 0, 0, 0, 0, 0];
    for (var i = 0; i < reportLength; i++) {

        if (report.ChildsInfo[i].Id == 500) {
            continue;
        }
        dataTotal[0] += report.ChildsInfo[i].GrandTotal.WinLoss;
        dataTotal[1] += report.ChildsInfo[i].GrandTotal.Position;
        dataTotal[2] += report.ChildsInfo[i].GrandTotal.Commission;
        dataTotal[3] += report.ChildsInfo[i].GrandTotal.LineTotal;
        dataTotal[4] += report.ChildsInfo[i].GrandTotal.VirtualLineTotal;
        dataTotal[5] += report.ChildsInfo[i].GrandTotal.Variance;
        dataTotal[6] += report.ChildsInfo[i].GrandTotal.Turnover;
        dataTotal[7] += report.ChildsInfo[i].GrandTotal.VirtualTurnover;
    }
    return dataTotal;
}

var CreditTotalCalculator = function (report) {
    var reportLength = report.ChildsInfo.length;
    var dataTotal = [0, 0, 0, 0, 0, 0, 0, 0];
    for (var i = 0; i < reportLength; i++) {

        if (report.ChildsInfo[i].Id == 500) {
            continue;
        }
        dataTotal[0] += report.ChildsInfo[i].CreditTotal.WinLoss;
        dataTotal[1] += report.ChildsInfo[i].CreditTotal.Position;
        dataTotal[2] += report.ChildsInfo[i].CreditTotal.Commission;
        dataTotal[3] += report.ChildsInfo[i].CreditTotal.LineTotal;
        dataTotal[4] += report.ChildsInfo[i].CreditTotal.VirtualLineTotal;
        dataTotal[5] += report.ChildsInfo[i].CreditTotal.Variance;
        dataTotal[6] += report.ChildsInfo[i].CreditTotal.Turnover;
        dataTotal[7] += report.ChildsInfo[i].CreditTotal.VirtualTurnover;
    }
    return dataTotal;
}

var DepositTotalCalculator = function (report) {
    var reportLength = report.ChildsInfo.length;
    var dataTotal = [0, 0, 0, 0, 0, 0, 0, 0];
    for (var i = 0; i < reportLength; i++) {

        if (report.ChildsInfo[i].Id == 500) {
            continue;
        }
        dataTotal[0] += report.ChildsInfo[i].DepositTotal.WinLoss;
        dataTotal[1] += report.ChildsInfo[i].DepositTotal.Position;
        dataTotal[2] += report.ChildsInfo[i].DepositTotal.Commission;
        dataTotal[3] += report.ChildsInfo[i].DepositTotal.LineTotal;
        dataTotal[4] += report.ChildsInfo[i].DepositTotal.VirtualLineTotal;
        dataTotal[5] += report.ChildsInfo[i].DepositTotal.Variance;
        dataTotal[6] += report.ChildsInfo[i].DepositTotal.Turnover;
        dataTotal[7] += report.ChildsInfo[i].DepositTotal.VirtualTurnover;
    }
    return dataTotal;
}

var MarginCaculator = function (lineTotal, virtualLineTotal) {
    return lineTotal / virtualLineTotal;
}
OtherSportsForNewCricket = function (report, isLive, CacheData, Params) {
    var self = this;
    self.Params = Params;
    self.CacheData = CacheData;
    var bodyData = [];
    var footerData = [];
    var maxColIndex = 11;
    if (report && report.ChildsInfo && report.ChildsInfo.length > 0) {
        var reportLength = report.ChildsInfo.length;
        var info = null;
        var groupdId = '';
        var betTypeChargeCommission = null;
        var tableId = isLive ? "liveOtherSportReportBySport" : "nonLiveOtherSportReportBySport";
        var grandTotal = null;
        var creditTotal = null;
        var depositTotal = null;

        for (var i = 0; i < reportLength; i++) {

            if (report.ChildsInfo[i].Id == 500) {
                betTypeChargeCommission = report.ChildsInfo[i];
                grandTotal = this.GrandTotalCalculator(report);
                creditTotal = this.CreditTotalCalculator(report);
                depositTotal = this.DepositTotalCalculator(report);
                continue;
            }
            var betType = report.ChildsInfo[i];

            groupdId = tableId + "_BetType" + betType.Id +
                                 "_Hedge" + (betType.Hedge && betType.Hedge != "" ? 0 : 1) +
                                 "_Live" + (isLive ? 1 : 0);
            info = betType.GrandTotal;
            bodyData.push({
                Columns: [
                    {
                        Name: "ColBetType",
                        Value: showHideSiteTypeManager.GetShowHideSiteTypeIcon(groupdId) +
                               reportManager.OtherSportReport.GetReportLink(info, report.IsLive, reportManager.Enum.DrillDownLevel.ByBetType),
                        CellAlign: "left",
                        CssClass: "bold"
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

            info = betType.CreditTotal;
            bodyData.push({
                Columns: [
                    {
                        Name: "ColBetType",
                        Value: reportManager.GetWinlossBySiteLink({
                            SportType: self.Params.reportParams.SportType,
                            SportGroupId: self.CacheData.parentController.CacheData.SportId,
                            WinLossDate: self.Params.reportParams.Winlostdate,
                            BetType: betType.Id,
                            BetTypeName: betType.AliasName,
                            IsLive: report.IsLive,
                            IsHedge: betType.Hedge && betType.Hedge != "" ? true : false,
                            SiteType: AccReportData.Constants.SiteType.Credit,
                            SiteTypeName: info.Name,
                            SportTypeName: self.Params.reportParams.SportTypeName
                        }),
                        CellAlign: "right",
                        CssClass: "bold"
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
                CssClass: showHideSiteTypeManager.GetSiteTypeRowCss(groupdId,
                    showHideSiteTypeManager.Enums.RowType.InfoRow)
            });

            info = betType.DepositTotal;
            bodyData.push({
                Columns: [
                    {
                        Name: "ColBetType",
                        Value: reportManager.GetWinlossBySiteLink({
                            SportType: self.Params.reportParams.SportType,
                            SportGroupId: self.CacheData.parentController.CacheData.SportId,
                            WinLossDate: self.Params.reportParams.Winlostdate,
                            BetType: betType.Id,
                            BetTypeName: betType.AliasName,
                            IsLive: report.IsLive,
                            IsHedge: betType.Hedge && betType.Hedge != "" ? true : false,
                            SiteType: AccReportData.Constants.SiteType.Deposit,
                            SiteTypeName: info.Name,
                            SportTypeName: self.Params.reportParams.SportTypeName
                        }),
                        CellAlign: "right",
                        CssClass: "bold"
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
                CssClass: showHideSiteTypeManager.GetSiteTypeRowCss(groupdId,
                    showHideSiteTypeManager.Enums.RowType.InfoRow)
            });
        }

        // for Total
        info = report.GrandTotal;
        info.Margin = this.MarginCaculator(grandTotal[3],grandTotal[6]);
        info.VirtualMargin = this.MarginCaculator(grandTotal[4], grandTotal[7]);
        groupdId = tableId + "_BetType0_Hedge0" +
                             "_Live" + (isLive ? 1 : 0);
        footerData.push({
            Columns: [
                {
                    Name: "ColBetType",
                    Value: showHideSiteTypeManager.GetShowHideSiteTypeIcon(groupdId) +
                           '<span class="right float-right">' + "Total" + '</span>'
                },
                { Name: "ColWinloss", Value: grandTotal[0] },
                { Name: "ColPosition", Value: grandTotal[1] },
                { Name: "ColCommission", Value: grandTotal[2] },
                { Name: "ColRealLineTotal", Value: grandTotal[3], TdClass: "colNote" },
                { Name: "ColVirtualLineTotal", Value: grandTotal[4], TdClass: "colNote" },
                { Name: "ColVariance", Value: grandTotal[5], TdClass: "colNote" },
                { Name: "ColRealTurnover", Value: grandTotal[6] },
                { Name: "ColVirtualTurnover", Value: grandTotal[7] },
                reportManager.GetColumnTurnover(info.Margin, grandTotal[6], "ColRealMargin"),
                reportManager.GetColumnTurnover(info.VirtualMargin, grandTotal[7], "ColVirtualMargin")]
        });

        info = report.CreditTotal;
        info.Margin = this.MarginCaculator(creditTotal[3], creditTotal[6]);
        info.VirtualMargin = this.MarginCaculator(creditTotal[4], creditTotal[7]);
        footerData.push({
            Columns: [{ Name: "ColBetType", Value: info.Name, CellAlign: "right", CssClass: "bold" },
                        { Name: "ColWinloss", Value: creditTotal[0] },
                        { Name: "ColPosition", Value: creditTotal[1] },
                        { Name: "ColCommission", Value: creditTotal[2] },
                        { Name: "ColRealLineTotal", Value: creditTotal[3] },
                        { Name: "ColVirtualLineTotal", Value: creditTotal[4] },
                        { Name: "ColVariance", Value: creditTotal[5] },
                        { Name: "ColRealTurnover", Value: creditTotal[6] },
                        { Name: "ColVirtualTurnover", Value: creditTotal[7] },
                        reportManager.GetColumnTurnover(info.Margin, info.Turnover, "ColRealMargin"),
                        reportManager.GetColumnTurnover(info.VirtualMargin, info.VirtualTurnover, "ColVirtualMargin")],
            CssClass: showHideSiteTypeManager.GetSiteTypeRowCss(groupdId,
                showHideSiteTypeManager.Enums.RowType.InfoRow)
        });

        info = report.DepositTotal;
        info.Margin = this.MarginCaculator(depositTotal[3], depositTotal[6]);
        info.VirtualMargin = this.MarginCaculator(depositTotal[4], depositTotal[7]);
        footerData.push({
            Columns: [{ Name: "ColBetType", Value: info.Name, CellAlign: "right", CssClass: "bold" },
                        { Name: "ColWinloss", Value: depositTotal[0] },
                        { Name: "ColPosition", Value: depositTotal[1] },
                        { Name: "ColCommission", Value: depositTotal[2] },
                        { Name: "ColRealLineTotal", Value: depositTotal[3] },
                        { Name: "ColVirtualLineTotal", Value: depositTotal[4] },
                        { Name: "ColVariance", Value: depositTotal[5] },
                        { Name: "ColRealTurnover", Value: depositTotal[6] },
                        { Name: "ColVirtualTurnover", Value: depositTotal[7] },
                        reportManager.GetColumnTurnover(info.Margin, depositTotal[0], "ColRealMargin"),
                        reportManager.GetColumnTurnover(info.VirtualMargin, depositTotal[0], "ColVirtualMargin")],
            CssClass: showHideSiteTypeManager.GetSiteTypeRowCss(groupdId,
                showHideSiteTypeManager.Enums.RowType.InfoRow)
        });

        // for Charge Commision
        info = betTypeChargeCommission.GrandTotal;
        groupdId = tableId + "_BetType0_Hedge0" +
                             "_Live" + (isLive ? 1 : 0);
        footerData.push({
            Columns: [
               {
                   Name: "ColBetType",
                   Value: showHideSiteTypeManager.GetShowHideSiteTypeIcon(groupdId) +
                          reportManager.OtherSportReport.GetReportLink(info, report.IsLive, reportManager.Enum.DrillDownLevel.ByBetType),
                   CellAlign: "left",
                   CssClass: "bold"
               },
                { Name: "ColWinloss", Value: info.WinLoss },
                { Name: "ColPosition", Value: info.Position },
                { Name: "ColCommission", Value: info.Commission },
                { Name: "ColRealLineTotal", Value: info.LineTotal, TdClass: "colNote" },
                { Name: "ColVirtualLineTotal", Value: info.VirtualLineTotal, TdClass: "colNote" },
                { Name: "ColVariance", Value: info.Variance, TdClass: "colNote" },
                { Name: "ColRealTurnover", Value: info.Turnover },
                { Name: "ColVirtualTurnover", Value: info.VirtualTurnover },
                reportManager.GetColumnTurnover(info.Margin, info.Turnover, "ColRealMargin"),
                reportManager.GetColumnTurnover(info.VirtualMargin, info.VirtualTurnover, "ColVirtualMargin")]
        });

        info = betTypeChargeCommission.CreditTotal;
        footerData.push({
            Columns: [{
                Name: "ColBetType",
                Value: reportManager.GetWinlossBySiteLink({
                    SportType: self.Params.reportParams.SportType,
                    SportGroupId: self.CacheData.parentController.CacheData.SportId,
                    WinLossDate: self.Params.reportParams.Winlostdate,
                    BetType: betTypeChargeCommission.Id,
                    BetTypeName: betType.AliasName,
                    IsLive: report.IsLive,
                    IsHedge: betType.Hedge && betType.Hedge != "" ? true : false,
                    SiteType: AccReportData.Constants.SiteType.Credit,
                    SiteTypeName: info.Name,
                    SportTypeName: self.Params.reportParams.SportTypeName
                }),
                CellAlign: "right",
                CssClass: "bold"
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
            CssClass: showHideSiteTypeManager.GetSiteTypeRowCss(groupdId,
                showHideSiteTypeManager.Enums.RowType.InfoRow)
        });

        info = betTypeChargeCommission.DepositTotal;
        footerData.push({
            Columns: [{
                Name: "ColBetType",
                Value: reportManager.GetWinlossBySiteLink({
                    SportType: self.Params.reportParams.SportType,
                    SportGroupId: self.CacheData.parentController.CacheData.SportId,
                    WinLossDate: self.Params.reportParams.Winlostdate,
                    BetType: betTypeChargeCommission.Id,
                    BetTypeName: betType.AliasName,
                    IsLive: report.IsLive,
                    IsHedge: betType.Hedge && betType.Hedge != "" ? true : false,
                    SiteType: AccReportData.Constants.SiteType.Deposit,
                    SiteTypeName: info.Name,
                    SportTypeName: self.Params.reportParams.SportTypeName
                }),
                CellAlign: "right",
                CssClass: "bold"
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
            CssClass: showHideSiteTypeManager.GetSiteTypeRowCss(groupdId,
                showHideSiteTypeManager.Enums.RowType.InfoRow)
        });

        // for All Total
        info = betTypeChargeCommission.GrandTotal;
        info.Margin = this.MarginCaculator(info.LineTotal + grandTotal[3], info.Turnover + grandTotal[6]);
        info.VirtualMargin = this.MarginCaculator(info.LineTotal + grandTotal[4], info.Turnover + grandTotal[7]);

        groupdId = tableId + "_BetType0_Hedge0" +
                             "_Live" + (isLive ? 1 : 0);
        footerData.push({
            Columns: [
                {
                    Name: "ColBetType",
                    Value: showHideSiteTypeManager.GetShowHideSiteTypeIcon(groupdId) +
                           '<span class="right float-right">' + "All Total" + '</span>'
                },
                { Name: "ColWinloss", Value: info.WinLoss + grandTotal[0] },
                { Name: "ColPosition", Value: info.Position + grandTotal[1] },
                { Name: "ColCommission", Value: info.Commission + grandTotal[2] },
                { Name: "ColRealLineTotal", Value: info.LineTotal + grandTotal[3], TdClass: "colNote" },
                { Name: "ColVirtualLineTotal", Value: info.VirtualLineTotal + grandTotal[4], TdClass: "colNote" },
                { Name: "ColVariance", Value: info.Variance + grandTotal[5], TdClass: "colNote" },
                { Name: "ColRealTurnover", Value: info.Turnover + grandTotal[6] },
                { Name: "ColVirtualTurnover", Value: info.VirtualTurnover + grandTotal[7] },
                reportManager.GetColumnTurnover(info.Margin, info.Turnover + grandTotal[6], "ColRealMargin"),
                reportManager.GetColumnTurnover(info.VirtualMargin, info.VirtualTurnover + grandTotal[7], "ColVirtualMargin")]
        });

        info = betTypeChargeCommission.CreditTotal;
        info.Margin = this.MarginCaculator(info.LineTotal + creditTotal[3], info.Turnover + creditTotal[6]);
        info.VirtualMargin = this.MarginCaculator(info.LineTotal + creditTotal[4], info.Turnover + creditTotal[7]);

        footerData.push({
            Columns: [{ Name: "ColBetType", Value: info.Name, CellAlign: "right", CssClass: "bold" },
                        { Name: "ColWinloss", Value: info.WinLoss + creditTotal[0] },
                        { Name: "ColPosition", Value: info.Position + creditTotal[1] },
                        { Name: "ColCommission", Value: info.Commission + creditTotal[2] },
                        { Name: "ColRealLineTotal", Value: info.LineTotal + creditTotal[3] },
                        { Name: "ColVirtualLineTotal", Value: info.VirtualLineTotal + creditTotal[4] },
                        { Name: "ColVariance", Value: info.Variance + creditTotal[5] },
                        { Name: "ColRealTurnover", Value: info.Turnover + creditTotal[6] },
                        { Name: "ColVirtualTurnover", Value: info.VirtualTurnover + creditTotal[7] },
                        reportManager.GetColumnTurnover(info.Margin, info.Turnover + creditTotal[6], "ColRealMargin"),
                        reportManager.GetColumnTurnover(info.VirtualMargin, info.VirtualTurnover + creditTotal[7], "ColVirtualMargin")],
            CssClass: showHideSiteTypeManager.GetSiteTypeRowCss(groupdId,
                showHideSiteTypeManager.Enums.RowType.InfoRow)
        });

        info = betTypeChargeCommission.DepositTotal;
        info.Margin = this.MarginCaculator(info.LineTotal + depositTotal[3], info.Turnover + depositTotal[6]);
        info.VirtualMargin = this.MarginCaculator(info.LineTotal + depositTotal[4], info.Turnover + depositTotal[7]);

        footerData.push({
            Columns: [{ Name: "ColBetType", Value: info.Name, CellAlign: "right", CssClass: "bold" },
                        { Name: "ColWinloss", Value: info.WinLoss + depositTotal[0] },
                        { Name: "ColPosition", Value: info.Position + depositTotal[1] },
                        { Name: "ColCommission", Value: info.Commission + depositTotal[2] },
                        { Name: "ColRealLineTotal", Value: info.LineTotal + depositTotal[3] },
                        { Name: "ColVirtualLineTotal", Value: info.VirtualLineTotal + depositTotal[4] },
                        { Name: "ColVariance", Value: info.Variance + depositTotal[5] },
                        { Name: "ColRealTurnover", Value: info.Turnover + depositTotal[6] },
                        { Name: "ColVirtualTurnover", Value: info.VirtualTurnover + depositTotal[7] },
                        reportManager.GetColumnTurnover(info.Margin, info.Turnover + depositTotal[6], "ColRealMargin"),
                        reportManager.GetColumnTurnover(info.VirtualMargin, info.VirtualTurnover + depositTotal[7], "ColVirtualMargin")],
            CssClass: showHideSiteTypeManager.GetSiteTypeRowCss(groupdId,
                showHideSiteTypeManager.Enums.RowType.InfoRow)
        });


        var gridOptions = {
            customizeHeader: true,
            groupHeaderRows: [{
                Columns: [{ HeaderText: viewResources.Bet_Type, Name: "ColBetType", MergeRowNumber: 2 },
                          { HeaderText: viewResources.Member_WinLoss, Name: "ColWinloss", MergeRowNumber: 2 },
                          { HeaderText: viewResources.SMA_WinLoss, Name: "ColPosition", MergeRowNumber: 2 },
                          { HeaderText: viewResources.Commission, Name: "ColCommission", MergeRowNumber: 2 },
                          { HeaderText: viewResources.Company_Income, Name: "ColRealLineTotal", MergeColNumber: 2 },
                          { HeaderText: viewResources.Variance, Name: "ColVariance", MergeRowNumber: 2 },
                          { HeaderText: viewResources.Company_Turnover, Name: "ColRealTurnover", MergeColNumber: 2 },
                          { HeaderText: viewResources.Margin, Name: "ColRealMargin", MergeColNumber: 2 }]
            }],

            columns: [
                {
                    HeaderText: viewResources.Bet_Type,
                    Width: 170, Name: "ColBetType",
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
                { HeaderText: viewResources.Virtual, Width: 100, Name: "ColVirtualMargin", HeaderAlign: "center", FormatNumber: FormatConstant.TWO_DECIMAL_PERCENT_NUMBER, CellAlign: "right", FormatPositive: '$' }],
            showTitle: true,
            showFooter: true,
            titleInfo: {
                text: "<a class='breadcrumbs-link' onclick='showHideReportManager.BackToMainReport();'>" + viewResources.Main + "</a> >" +
                      " <a class='breadcrumbs-link' onclick='reportManager.OtherSportReport.ShowSummaryReport();'>" + self.CacheData.parentController.CacheData.SportName + "</a> |" +
                      " " + self.Params.reportParams.SportTypeName +
                      " \"" + (isLive ? viewResources.Live : viewResources.Symbol_Sparse_Non_Live) + "\"" +
                      " " + viewResources.DailyWinLoss2h_ReportTitleSuffix,
                showExcelButton: false
            },
            bodyRows: bodyData,
            footerRows: footerData,
            numberOfFrozenColumn: 0,
            gridWidth: 1400,
            gridExpandHeight: 200,
            fullWidth: true,
            fullHeight: false,
            gridHeight: reportManager.CachedData.maxGridHeight,
            showDistinctRow: false,
            showHighlightRow: false,
            noDataText: viewResources.EmptyReportMessage,
            headerColumnHeight: 17
        };

        return gridOptions;
    }
};