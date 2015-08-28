Namespace.Register("Fanex.AccountingReports.Gaming.LiveCasinoAgencyWinLoss.ReportByAgent");
Fanex.AccountingReports.Gaming.LiveCasinoAgencyWinLoss.ReportByAgent.SuperReport = function () {
};

Inheritance_Manager.extend(Fanex.AccountingReports.Gaming.LiveCasinoAgencyWinLoss.ReportByAgent.SuperReport,
    Fanex.AccountingReports.Gaming.LiveCasinoAgencyWinLoss.ReportByAgent.BaseReport);

Fanex.AccountingReports.Gaming.LiveCasinoAgencyWinLoss.ReportByAgent.SuperReport.prototype.CachedDOMElements = function () {
    var self = this;
    self.Tables.reportContainer = $("#mainLiveCasinoAgencyWinLossReportContainer");
    self.Tables.report = $("#mainLiveCasinoAgencyWinLossReport");
};

Fanex.AccountingReports.Gaming.LiveCasinoAgencyWinLoss.ReportByAgent.SuperReport.prototype.GetBreadcrumb = function () {
    return AccReport.utility.StringFormat(viewResources.LiveCasinoAgencyWinLoss_ReportTitle,
        [reportManager.Params.reportParams.FromDate, reportManager.Params.reportParams.ToDate]);
};

Fanex.AccountingReports.Gaming.LiveCasinoAgencyWinLoss.ReportByAgent.SuperReport.prototype.GetDrilldownLink = function (currentCol, currentValue) {
    var onclickEvent = "reportManager.ReportByAgent.MasterReport.GetReport('" + currentValue.Name + "');";
    return '<span class="link bold" onclick="' + onclickEvent + '">' + currentValue.Name + '</span>';
};

Fanex.AccountingReports.Gaming.LiveCasinoAgencyWinLoss.ReportByAgent.SuperReport.prototype.GetReport = function () {
    var self = this;
    reportManager.CachedData.sortColName = null;
    reportManager.CachedData.sortDirection = null;
    reportManager.CachedData.sortDataType = null;
    AccReport.disableInput();
    var uplineParams = reportManager.Params.reportParams;
    var selectParam = {
        FromDate: uplineParams.FromDate,
        ToDate: uplineParams.ToDate,
        UserName: '',
        ExcludeTestData: uplineParams.ExcludeTestData,
        LicenseeId: uplineParams.LicenseeId,
        GameTypeId: uplineParams.GameTypeId,
        GameTableId: uplineParams.GameTableId,
        Currency: uplineParams.Currency,
        ReportType: uplineParams.ReportType,
        ProductId: reportManager.CachedData.productId
    };

    self.CachedData.currentPage = 0;
    self.Params.reportParams = selectParam;
    self.CachedData.hasParams = true;
    reportManager.ReportByAgent.CachedData.currentReportManager = self;
    AccReport.utility.CallAjax(this,
            window.rootUrl + "Gaming/LiveCasinoAgencyWinLoss/GetReport",
            self.Params.reportParams,
            self.RenderReport,
            self.RenderReport,
            AccReport.enableInput);
};

Fanex.AccountingReports.Gaming.LiveCasinoAgencyWinLoss.ReportByAgent.SuperReport.prototype.ExportReport = function () {
    var self = this;
    self.Params.reportParams.ReportTitle = self.GetReportTitle();
    AccReport.downloadFile(window.rootUrl + "Gaming/LiveCasinoAgencyWinLoss/ExportReport", reportManager.Params.reportParams);
};

Fanex.AccountingReports.Gaming.LiveCasinoAgencyWinLoss.ReportByAgent.SuperReport.prototype.GetReportTitle = function () {
    return AccReport.utility.StringFormat(viewResources.LiveCasinoAgencyWinLoss_ReportTitle,
        [reportManager.Params.reportParams.FromDate, reportManager.Params.reportParams.ToDate]);
};