Main2hReport = function () {
    var self = this;

    self.Tables = {
        reportContainer: null,
        report: null
    };

    self.Params = {
        reportParams: {
            Winlostdate: null
        }
    };

    self.ParlayEnums = {
        Parlay: 99,
        MixParlay: 9,
        SystemParlay: 29
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
        window.rootUrl + "Main/DailyWinLoss2h/GetMain2hReport",
        self.Params.reportParams,
        self.RenderReport,
        self.RenderReport,
        AccReport.enableInput);
};

Main2hReport.prototype.CachedDOMElements = function () {
    this.Tables.reportContainer = $('#main2hReportContainer');
    this.Tables.report = $('#main2hReport');
};

Main2hReport.prototype.GetParams = function (reportDate) {
    this.Params.reportParams.Winlostdate = reportDate;
}

Main2hReport.prototype.IsCasinoSport = function (Id) {
    return Id == AccReportData.Constants.SportType.Casino;
}

Main2hReport.prototype.RenderReport = function (report) {
    var self = this;
    this.Tables.reportContainer.removeClass("display-none");
    this.Tables.report.addClass("display-none");
    var bodyData = [];
    var footerData = [];
    var maxColIndex = 11;
    var isBeforeCasinoSingleWalletLaunchDate = moment(this.Params.reportParams.Winlostdate).diff(casinoSingleWalletLaunchDate, "days") < 0;
    if (report && report.ChildsInfo && report.ChildsInfo.length > 0) {
        var productLength = report.ChildsInfo.length;
        var info = null;
        var tableId = "main2hReport";
        var rmInfoList = [];
        var groupdId = '';

        for (var i = 0; i < productLength; i++) {
            var product = report.ChildsInfo[i];
            var sportLength = product.ChildsInfo.length;
            rmInfoList = [];

            for (var j = 0; j < sportLength; j++) {
                var sportWinLoss = product.ChildsInfo[j];
                var isCasinoSport = self.IsCasinoSport(sportWinLoss.Id);
                var isShowVirtualTurnover = sportWinLoss.EnableVirtual ||
                    sportWinLoss.Id == AccReportData.Constants.SportType.LiveCasino ||
                    sportWinLoss.Id == AccReportData.Constants.SportType.VirtualSports ||
                    sportWinLoss.Id == AccReportData.Constants.SportType.Keno ||
                    isCasinoSport ||
                    sportWinLoss.Id == AccReportData.Constants.SportType.Bettrade ||
                    sportWinLoss.Id == AccReportData.Constants.SportType.ColossusBet ||
                    sportWinLoss.Id == AccReportData.Constants.SportType.CockFighting;
                var len = sportWinLoss.ChildsInfo.length;
                info = sportWinLoss.NormalTotal;
                groupdId = tableId + "_Product" + product.Id + "_Sport" + sportWinLoss.Id;
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
                    { Name: "ColRealTurnover", Value: info.IsHideRealTurnover ? viewResources.NotAvailable : info.Turnover },
                    { Name: "ColVirtualTurnover", Value: isShowVirtualTurnover ? info.VirtualTurnover : viewResources.NotAvailable },
                    isCasinoSport && isBeforeCasinoSingleWalletLaunchDate ? reportManager.GetCasinoMarginColumn("ColRealMargin") : reportManager.GetColumnTurnover(info.Margin, info.Turnover, "ColRealMargin"),
                    isCasinoSport && isBeforeCasinoSingleWalletLaunchDate ? reportManager.GetCasinoMarginColumn("ColVirtualMargin") : reportManager.GetColumnTurnover(info.VirtualMargin, info.VirtualTurnover, "ColVirtualMargin")],
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
                        { Name: "ColRealTurnover", Value: info.IsHideRealTurnover ? viewResources.NotAvailable : info.Turnover },
                        { Name: "ColVirtualTurnover", Value: isShowVirtualTurnover ? info.VirtualTurnover : viewResources.NotAvailable },
                        isCasinoSport && isBeforeCasinoSingleWalletLaunchDate ? reportManager.GetCasinoMarginColumn("ColRealMargin") : reportManager.GetColumnTurnover(info.Margin, info.Turnover, "ColRealMargin"),
                        isCasinoSport && isBeforeCasinoSingleWalletLaunchDate ? reportManager.GetCasinoMarginColumn("ColVirtualMargin") : reportManager.GetColumnTurnover(info.VirtualMargin, info.VirtualTurnover, "ColVirtualMargin")],
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
                        { Name: "ColRealTurnover", Value: info.IsHideRealTurnover ? viewResources.NotAvailable : info.Turnover },
                        { Name: "ColVirtualTurnover", Value: isShowVirtualTurnover ? info.VirtualTurnover : viewResources.NotAvailable },
                        isCasinoSport && isBeforeCasinoSingleWalletLaunchDate ? reportManager.GetCasinoMarginColumn("ColRealMargin") : reportManager.GetColumnTurnover(info.Margin, info.Turnover, "ColRealMargin"),
                        isCasinoSport && isBeforeCasinoSingleWalletLaunchDate ? reportManager.GetCasinoMarginColumn("ColVirtualMargin") : reportManager.GetColumnTurnover(info.VirtualMargin, info.VirtualTurnover, "ColVirtualMargin")],
                    CssClass: showHideSiteTypeManager.GetSiteTypeRowCss(groupdId,
                        showHideSiteTypeManager.Enums.RowType.InfoRow)
                });

                bodyData.push({
                    Columns: [{
                        Name: "ColSportType",
                        MergeColNumber: maxColIndex
                    }],
                    CssClass: showHideSiteTypeManager.GetSiteTypeRowCss(groupdId,
                        showHideSiteTypeManager.Enums.RowType.PaddingRow)
                });

                if (is8RMActive == 'true' &&
                    sportWinLoss.EightRMTotal.LineTotal) {
                    info = sportWinLoss.EightRMTotal;
                    if (sportWinLoss.Id != AccReportData.Constants.SportType.SoccerMajorBetType &&
                        sportWinLoss.Id != AccReportData.Constants.SportType.SoccerMinorBetType &&
                        sportWinLoss.Id != AccReportData.Constants.SportType._2hReport_OtherSports) {
                        bodyData.push({
                            Columns: [{ Name: "ColSportType", Value: info.Name, CellAlign: "right", CssClass: "bold" },
                                { Name: "ColWinloss", Value: info.WinLoss },
                                { Name: "ColPosition", Value: info.Position },
                                { Name: "ColCommission", Value: info.Commission },
                                { Name: "ColRealLineTotal", Value: info.LineTotal },
                                { Name: "ColVirtualLineTotal", Value: info.VirtualLineTotal },
                                { Name: "ColVariance", Value: info.Variance },
                                { Name: "ColRealTurnover", Value: info.IsHideRealTurnover ? viewResources.NotAvailable : info.Turnover },
                                { Name: "ColVirtualTurnover", Value: isShowVirtualTurnover ? info.VirtualTurnover : viewResources.NotAvailable },
                                reportManager.GetColumnTurnover(info.Margin, info.Turnover, "ColRealMargin"),
                                reportManager.GetColumnTurnover(info.VirtualMargin, info.VirtualTurnover, "ColVirtualMargin")]
                        });
                    } else {
                        rmInfoList.push(info);
                    }
                }

                if (isBARMActive == 'true' &&
                    sportWinLoss.BARMTotal.LineTotal) {
                    info = sportWinLoss.BARMTotal;
                    if (sportWinLoss.Id != AccReportData.Constants.SportType.SoccerMajorBetType &&
                        sportWinLoss.Id != AccReportData.Constants.SportType.SoccerMinorBetType &&
                        sportWinLoss.Id != AccReportData.Constants.SportType._2hReport_OtherSports) {
                        bodyData.push({
                            Columns: [{ Name: "ColSportType", Value: info.Name, CellAlign: "right", CssClass: "bold" },
                                { Name: "ColWinloss", Value: info.WinLoss },
                                { Name: "ColPosition", Value: info.Position },
                                { Name: "ColCommission", Value: info.Commission },
                                { Name: "ColRealLineTotal", Value: info.LineTotal },
                                { Name: "ColVirtualLineTotal", Value: info.VirtualLineTotal },
                                { Name: "ColVariance", Value: info.Variance },
                                { Name: "ColRealTurnover", Value: info.IsHideRealTurnover ? viewResources.NotAvailable : info.Turnover },
                                { Name: "ColVirtualTurnover", Value: isShowVirtualTurnover ? info.VirtualTurnover : viewResources.NotAvailable },
                                reportManager.GetColumnTurnover(info.Margin, info.Turnover, "ColRealMargin"),
                                reportManager.GetColumnTurnover(info.VirtualMargin, info.VirtualTurnover, "ColVirtualMargin")]
                        });
                    } else {
                        rmInfoList.push(info);
                    }
                }
            }

            if (i == 0 &&
                product.ChildsInfo &&
                product.ChildsInfo.length > 0) {
                info = report.NormalTotal;
                groupdId = tableId + "_Product" + product.Id + "_Sport0";
                bodyData.push({
                    Columns: [
                        {
                            Name: "ColSportType",
                            Value: showHideSiteTypeManager.GetShowHideSiteTypeIcon(groupdId) +
                                  '<span class="right bold float-right">' + info.Name + '</span>'
                        },
                        { Name: "ColWinloss", Value: info.WinLoss },
                        { Name: "ColPosition", Value: info.Position },
                        { Name: "ColCommission", Value: info.Commission },
                        { Name: "ColRealLineTotal", Value: info.LineTotal, CssClass: "bold" },
                        { Name: "ColVirtualLineTotal", Value: info.VirtualLineTotal, CssClass: "bold" },
                        { Name: "ColVariance", Value: info.Variance, CssClass: "bold" },
                        { Name: "ColRealTurnover", Value: info.Turnover, CssClass: "bold" },
                        { Name: "ColVirtualTurnover", Value: info.VirtualTurnover, CssClass: "bold" },
                        reportManager.GetColumnTurnover(info.Margin, info.Turnover, "ColRealMargin"),
                        reportManager.GetColumnTurnover(info.VirtualMargin, info.VirtualTurnover, "ColVirtualMargin")],
                    CssClass: "subTotalRow"
                });

                info = product.CreditTotal;
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
                    CssClass: "subTotalRow " + showHideSiteTypeManager.GetSiteTypeRowCss(groupdId,
                        showHideSiteTypeManager.Enums.RowType.InfoRow)
                });

                info = product.DepositTotal;
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
                    CssClass: "subTotalRow " + showHideSiteTypeManager.GetSiteTypeRowCss(groupdId,
                        showHideSiteTypeManager.Enums.RowType.InfoRow)
                });

                var rmInfoLength = rmInfoList.length;
                for (var k = 0; k < rmInfoLength; k++) {
                    info = rmInfoList[k];
                    bodyData.push({
                        Columns: [{ Name: "ColSportType", Value: info.Name, CellAlign: "right", CssClass: "bold" },
                            { Name: "ColWinloss", Value: info.WinLoss },
                            { Name: "ColPosition", Value: info.Position },
                            { Name: "ColCommission", Value: info.Commission },
                            { Name: "ColRealLineTotal", Value: info.LineTotal },
                            { Name: "ColVirtualLineTotal", Value: info.VirtualLineTotal },
                            { Name: "ColVariance", Value: info.Variance },
                            { Name: "ColRealTurnover", Value: info.IsHideRealTurnover ? viewResources.NotAvailable : info.Turnover },
                            { Name: "ColVirtualTurnover", Value: isShowVirtualTurnover ? info.VirtualTurnover : viewResources.NotAvailable },
                            reportManager.GetColumnTurnover(info.Margin, info.Turnover, "ColRealMargin"),
                            reportManager.GetColumnTurnover(info.VirtualMargin, info.VirtualTurnover, "ColVirtualMargin")],
                        CssClass: "subTotalRow"
                    });
                }

                bodyData.push({
                    Columns: [{ Name: "ColSportType", MergeColNumber: maxColIndex}]
                });
            }
        }

        if (report && report.ChildsInfo.length >= 2 &&
                (report.ChildsInfo[0].ChildsInfo.length > 0 ||
                 report.ChildsInfo[1].ChildsInfo.length > 0)) {
            info = report.GrandTotal;
            groupdId = tableId + "_Product0_Sport0";
            footerData.push({
                Columns: [
                    {
                        Name: "ColSportType",
                        Value: showHideSiteTypeManager.GetShowHideSiteTypeIcon(groupdId) +
                              '<span class="right bold float-right">' + info.Name + '</span>'
                    },
                    { Name: "ColWinloss", Value: info.WinLoss },
                    { Name: "ColPosition", Value: info.Position },
                    { Name: "ColCommission", Value: info.Commission },
                    { Name: "ColRealLineTotal", Value: info.LineTotal },
                    { Name: "ColVirtualLineTotal", Value: info.VirtualLineTotal },
                    { Name: "ColVariance", Value: info.Variance },
                    { Name: "ColRealTurnover", Value: info.Turnover },
                    { Name: "ColVirtualTurnover", Value: info.VirtualTurnover },
                    { Name: "ColRealMargin" },
                    { Name: "ColVirtualMargin"}]
            });

            info = report.CreditTotal;
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
                      { Name: "ColRealMargin" },
                      { Name: "ColVirtualMargin"}],
                CssClass: "subTotalRow " + showHideSiteTypeManager.GetSiteTypeRowCss(groupdId,
                        showHideSiteTypeManager.Enums.RowType.InfoRow)
            });

            info = report.DepositTotal;
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
                      { Name: "ColRealMargin" },
                      { Name: "ColVirtualMargin"}],
                CssClass: "subTotalRow " + showHideSiteTypeManager.GetSiteTypeRowCss(groupdId,
                        showHideSiteTypeManager.Enums.RowType.InfoRow)
            });

            var reportNote = "* Virtual All Total: Soccer + Metric Gaming - Super Live + Other Sports + Parlay" +
                             "<br/>* Grand Total: Virtual All Total + 8RM + BARM + Number Game + Live Casino + Casino + Virtual Sports + Keno + Racing +  Bingo + P2P + Bettrade + Colossus Bets + Cockfighting";
            footerData.push({
                Columns: [{
                    Name: "ColSportType",
                    Value: reportNote,
                    MergeColNumber: maxColIndex,
                    CellAlign: "left",
                    TdClass: "grand-total-note"
                }]
            });
        }
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
        noDataText: viewResources.EmptyReportMessage,
        titleInfo: {
            text: viewResources.DailyWinLoss2h_Main2hReport_ReportTitle,
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
        //gridHeight: reportManager.CachedData.maxGridHeight,
        showDistinctRow: false,
        showHighlightRow: false,
        headerColumnHeight: 17
    };

    this.Tables.report.removeClass("display-none").fanexGrid(gridOptions);
    this.Tables.report.find(".showHideSiteType.showHideIcon").click(function () {
        var btnIcon = $(this);
        showHideSiteTypeManager.ShowHideSiteTypeInfo_ByGroup(btnIcon, self.Tables.report);
    });
};

Main2hReport.prototype.ExportReport = function () {
    AccReport.downloadFile(window.rootUrl + "Main/DailyWinLoss2h/ExportMain2hReport", this.Params.reportParams);
};

Main2hReport.prototype.GetSportLink = function (winLossInfo) {
    var sportLink = '';
    switch (winLossInfo.Id) {
        case AccReportData.Constants.SportType.SoccerMajorBetType: //1
        case AccReportData.Constants.SportType.SoccerMinorBetType: //2
            sportLink = "<span class='report2h-link bold' onclick=\"showHideReportManager.ShowSoccerReport('" +
                            winLossInfo.Id + "','" +
                            winLossInfo.Name + "','" +
                            reportManager.Main2hReport.Params.reportParams.Winlostdate + "'); return;\">" +
                            winLossInfo.Name + '</span>';
            break;
        case AccReportData.Constants.SportType.Racing: //151
            sportLink = "<span class='report2h-link bold' onclick=\"showHideReportManager.ShowRacingReport('" +
                            winLossInfo.Id + "','" +
                            winLossInfo.Name + "','" +
                            reportManager.Main2hReport.Params.reportParams.Winlostdate + "'); return;\">" +
                            winLossInfo.Name + '</span>';
            break;
        case AccReportData.Constants.SportType.Casino: //251
            sportLink = "<span class='report2h-link bold' onclick=\"showHideReportManager.ShowCasinoReport('" +
                            winLossInfo.Id + "','" +
                            winLossInfo.Name + "','" +
                            reportManager.Main2hReport.Params.reportParams.Winlostdate + "'); return;\">" +
                            winLossInfo.Name + '</span>';
            break;
        case AccReportData.Constants.SportType.Bingo: //252
            sportLink = "<span class='report2h-link bold' onclick=\"showHideReportManager.ShowBingoReport('" +
                            winLossInfo.Id + "','" +
                            winLossInfo.Name + "','" +
                            reportManager.Main2hReport.Params.reportParams.Winlostdate + "'); return;\">" +
                            winLossInfo.Name + '</span>';
            break;
        case this.ParlayEnums.Parlay: //99
            var reportDate = new Date(reportManager.Main2hReport.Params.reportParams.Winlostdate);
            sportLink = "<span class='report2h-link bold' onclick=\"showHideReportManager.ShowParlayReport('" +
                            winLossInfo.Id + "','" +
                            winLossInfo.Name + "','" +
                            reportManager.Main2hReport.Params.reportParams.Winlostdate + "'); return;\">" +
                            winLossInfo.Name + '</span>';
            break;
        case this.ParlayEnums.MixParlay: //9
            var reportDate = new Date(reportManager.Main2hReport.Params.reportParams.Winlostdate);
            var changeDate = new Date(beginSupportAdvanceMixParlayDate);
            if (reportDate < changeDate) {
                sportLink = "<span class='report2h-link bold' onclick=\"showHideReportManager.ShowMixParlayReport('" +
                            winLossInfo.Id + "','" +
                            winLossInfo.Name + "','" +
                            reportManager.Main2hReport.Params.reportParams.Winlostdate + "'); return;\">" +
                            winLossInfo.Name + '</span>';
            } else {
                sportLink = "<span class='report2h-link bold' onclick=\"showHideReportManager.ShowAdvanceMixParlayReport('" +
                            winLossInfo.Id + "','" +
                            winLossInfo.Name + "','" +
                            reportManager.Main2hReport.Params.reportParams.Winlostdate + "'); return;\">" +
                            winLossInfo.Name + '</span>';
            }

            break;
        case this.ParlayEnums.SystemParlay: //29
            if (new Date(balanceUpDay) <= new Date(reportManager.Main2hReport.Params.reportParams.Winlostdate)) {
                sportLink = "<span class='report2h-link bold' onclick=\"showHideReportManager.ShowSystemParlayReport('" +
                            winLossInfo.Id + "','" +
                            winLossInfo.Name + "','" +
                            reportManager.Main2hReport.Params.reportParams.Winlostdate + "'); return;\">" +
                            winLossInfo.Name + '</span>';
            } else {
                sportLink = '<span class="bold">' + winLossInfo.Name + '</span>';
            }

            break;
        case AccReportData.Constants.SportType._2hReport_OtherSports: //3
            sportLink = "<span class='report2h-link bold' onclick=\"showHideReportManager.ShowOtherSportReport('" +
                            winLossInfo.Id + "','" +
                            winLossInfo.Name + "','" +
                            reportManager.Main2hReport.Params.reportParams.Winlostdate + "'); return;\">" +
                            winLossInfo.Name + '</span>';
            break;
        case AccReportData.Constants.SportType.NumberGame: //161
            sportLink = "<span class='report2h-link bold' onclick=\"showHideReportManager.ShowNumberGameReport('" +
                            winLossInfo.Id + "','" +
                            winLossInfo.Name + "','" +
                            reportManager.Main2hReport.Params.reportParams.Winlostdate + "'); return;\">" +
                            winLossInfo.Name + '</span>';
            break;
        case AccReportData.Constants.SportType.LiveCasino: //162
            sportLink = "<span class='report2h-link bold' onclick=\"showHideReportManager.ShowMainLiveCasinoReport('" +
                            winLossInfo.Id + "','" +
                            winLossInfo.Name + "','" +
                            reportManager.Main2hReport.Params.reportParams.Winlostdate + "'); return;\">" +
                            winLossInfo.Name + '</span>';
            break;
        case AccReportData.Constants.SportType.Keno: //202
            sportLink = "<span class='report2h-link bold' onclick=\"showHideReportManager.ShowKenoReport('" +
                            winLossInfo.Id + "','" +
                            winLossInfo.Name + "','" +
                            reportManager.Main2hReport.Params.reportParams.Winlostdate + "'); return;\">" +
                            winLossInfo.Name + '</span>';
            break;
        case AccReportData.Constants.SportType.VirtualSports: //180
            sportLink = "<span class='report2h-link bold' onclick=\"showHideReportManager.ShowVirtualSportReport('" +
                            winLossInfo.Id + "','" +
                            winLossInfo.Name + "','" +
                            reportManager.Main2hReport.Params.reportParams.Winlostdate + "'); return;\">" +
                            winLossInfo.Name + '</span>';
            break;

        case reportManager._2hSportConstant.metricGaming:
            sportLink = "<span class='report2h-link bold' onclick=\"showHideReportManager.ShowMetricGamingReport('" +
                            winLossInfo.Id + "','" +
                            winLossInfo.Name + "','" +
                            reportManager.Main2hReport.Params.reportParams.Winlostdate + "'); return;\">" +
                            winLossInfo.Name + '</span>';
            break;
        case AccReportData.Constants.SportType.Bettrade: //-2
            sportLink = "<span class='report2h-link bold' onclick=\"showHideReportManager.ShowBettrade('" +
                            winLossInfo.Id + "','" +
                            winLossInfo.Name + "','" +
                            reportManager.Main2hReport.Params.reportParams.Winlostdate + "'); return;\">" +
                            winLossInfo.Name + '</span>';
            break;
        case AccReportData.Constants.SportType.ColossusBet: //204
            sportLink = "<span class='report2h-link bold' onclick=\"showHideReportManager.ShowColossusBet('" +
                            winLossInfo.Id + "','" +
                            winLossInfo.Name + "','" +
                            reportManager.Main2hReport.Params.reportParams.Winlostdate + "'); return;\">" +
                            winLossInfo.Name + '</span>';
            break;
        case AccReportData.Constants.SportType.CockFighting: //204
            sportLink = "<span class='report2h-link bold' onclick=\"showHideReportManager.ShowCockFighting('" +
                            winLossInfo.Id + "','" +
                            winLossInfo.Name + "','" +
                            reportManager.Main2hReport.Params.reportParams.Winlostdate + "'); return;\">" +
                            winLossInfo.Name + '</span>';
            break;
        default:
            sportLink = "<span class='bold'>" + winLossInfo.Name + '</span>';
    }

    return sportLink;
};

Main2hReport.prototype.ShowHideSiteTypeInfo = function () {
    var self = this;
    self.Tables.report.removeClass("display-none").fanexGrid("resizeGrid");
}; 