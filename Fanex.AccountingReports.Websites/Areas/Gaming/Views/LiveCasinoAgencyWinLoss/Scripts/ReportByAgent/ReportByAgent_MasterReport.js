Namespace.Register("Fanex.AccountingReports.Gaming.LiveCasinoAgencyWinLoss.ReportByAgent");
Fanex.AccountingReports.Gaming.LiveCasinoAgencyWinLoss.ReportByAgent.MasterReport = function () {
};

Inheritance_Manager.extend(Fanex.AccountingReports.Gaming.LiveCasinoAgencyWinLoss.ReportByAgent.MasterReport,
    Fanex.AccountingReports.Gaming.LiveCasinoAgencyWinLoss.ReportByAgent.BaseReport);

Fanex.AccountingReports.Gaming.LiveCasinoAgencyWinLoss.ReportByAgent.MasterReport.prototype.CachedDOMElements = function () {
    var self = this;
    self.Tables.reportContainer = $("#reportByAgent_MasterReportContainer");
    self.Tables.report = $("#reportByAgent_MasterReport");
};

Fanex.AccountingReports.Gaming.LiveCasinoAgencyWinLoss.ReportByAgent.MasterReport.prototype.GetBreadcrumb = function () {
    var self = this;
    var uplineParams = reportManager.ReportByAgent.SuperReport.Params.reportParams;
    if (!reportManager.ReportByAgent.SuperReport.CachedData.hasParams) {
        var onclickEvent = "reportManager.ReportByAgent.MasterReport.BackToSuperReport();";
        return '<span class="link bold" onclick="' + onclickEvent + '">' + AccReport.utility.StringFormat(viewResources.LiveCasinoAgencyWinLoss_ReportTitle,
                    [reportManager.Params.reportParams.FromDate, reportManager.Params.reportParams.ToDate]) +
               '</span> > ' + self.Params.reportParams.UserName;
    }

    var onclickEvent = "reportManager.ReportByAgent.MasterReport.BackToSuperReport();";
    return '<span class="link bold" onclick="' + onclickEvent + '">' + AccReport.utility.StringFormat(viewResources.LiveCasinoAgencyWinLoss_ReportTitle,
                [reportManager.Params.reportParams.FromDate, reportManager.Params.reportParams.ToDate]) +
            '</span> > ' + self.Params.reportParams.UserName;
};

Fanex.AccountingReports.Gaming.LiveCasinoAgencyWinLoss.ReportByAgent.MasterReport.prototype.GetDrilldownLink = function (currentCol, currentValue) {
    var onclickEvent = "reportManager.ReportByAgent.AgentReport.GetReport('" + currentValue.Name + "')";
    return '<span class="link bold" onclick="' + onclickEvent + '">' + currentValue.Name + '</span>';
};

Fanex.AccountingReports.Gaming.LiveCasinoAgencyWinLoss.ReportByAgent.MasterReport.prototype.GetReport = function (username) {
    var self = this;
    reportManager.CachedData.sortColName = null;
    reportManager.CachedData.sortDirection = null;
    reportManager.CachedData.sortDataType = null;
    AccReport.disableInput();
    var uplineParams = reportManager.ReportByAgent.SuperReport.Params.reportParams;
    var selectParam = {
        FromDate: uplineParams.FromDate,
        ToDate: uplineParams.ToDate,
        UserName: username,
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
    reportManager.ReportByAgent.SuperReport.HideReport();
    reportManager.ReportByAgent.CachedData.currentReportManager = self;

    AccReport.utility.CallAjax(this,
            window.rootUrl + "Gaming/LiveCasinoAgencyWinLoss/GetReport",
            self.Params.reportParams,
            self.RenderReport,
            self.RenderReport,
            AccReport.enableInput);
};

Fanex.AccountingReports.Gaming.LiveCasinoAgencyWinLoss.ReportByAgent.MasterReport.prototype.BackToSuperReport = function () {
    var self = this;
    self.HideReport();
    self.CachedData.hasParams = false;
    if (reportManager.ReportByAgent.SuperReport.CachedData.hasParams) {
        reportManager.ReportByAgent.CachedData.currentReportManager = reportManager.ReportByAgent.SuperReport;
        reportManager.ReportByAgent.SuperReport.ShowReport();
    } else {
        reportManager.ReportByAgent.CachedData.currentReportManager = reportManager.ReportByAgent.SuperReport;
        reportManager.ReportByAgent.SuperReport.GetReport();
    }
};

Fanex.AccountingReports.Gaming.LiveCasinoAgencyWinLoss.ReportByAgent.MasterReport.prototype.ExportReport = function () {
    var self = this;
    self.Params.reportParams.ReportTitle = self.GetReportTitle();
    AccReport.downloadFile(window.rootUrl + "Gaming/LiveCasinoAgencyWinLoss/ExportReport", self.Params.reportParams);
};

Fanex.AccountingReports.Gaming.LiveCasinoAgencyWinLoss.ReportByAgent.MasterReport.prototype.GetReportTitle = function () {
    var self = this;
    var uplineParams = reportManager.ReportByAgent.SuperReport.Params.reportParams;
    if (!reportManager.ReportByAgent.SuperReport.CachedData.hasParams) {
        return AccReport.utility.StringFormat(viewResources.LiveCasinoAgencyWinLoss_ReportTitle,
                    [reportManager.Params.reportParams.FromDate, reportManager.Params.reportParams.ToDate]) +
               ' > ' + self.Params.reportParams.UserName;
    }

    return AccReport.utility.StringFormat(viewResources.LiveCasinoAgencyWinLoss_ReportTitle,
                [reportManager.Params.reportParams.FromDate, reportManager.Params.reportParams.ToDate]) +
            ' > ' + self.Params.reportParams.UserName;
};