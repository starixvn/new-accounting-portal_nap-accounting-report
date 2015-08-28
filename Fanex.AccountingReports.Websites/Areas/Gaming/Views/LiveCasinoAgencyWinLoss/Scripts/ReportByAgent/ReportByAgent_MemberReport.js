Namespace.Register("Fanex.AccountingReports.Gaming.LiveCasinoAgencyWinLoss.ReportByAgent");
Fanex.AccountingReports.Gaming.LiveCasinoAgencyWinLoss.ReportByAgent.MemberReport = function () {
};

Inheritance_Manager.extend(Fanex.AccountingReports.Gaming.LiveCasinoAgencyWinLoss.ReportByAgent.MemberReport,
    Fanex.AccountingReports.Gaming.LiveCasinoAgencyWinLoss.ReportByAgent.BaseReport);

Fanex.AccountingReports.Gaming.LiveCasinoAgencyWinLoss.ReportByAgent.MemberReport.prototype.CachedDOMElements = function () {
    var self = this;
    self.Tables.reportContainer = $("#reportByAgent_MemberReportContainer");
    self.Tables.report = $("#reportByAgent_MemberReport");
};

Fanex.AccountingReports.Gaming.LiveCasinoAgencyWinLoss.ReportByAgent.MemberReport.prototype.GetBreadcrumb = function () {
    var self = this;
    var superParams = reportManager.ReportByAgent.SuperReport.Params.reportParams;
    var masterParams = reportManager.ReportByAgent.MasterReport.Params.reportParams;
    var agentParams = reportManager.ReportByAgent.AgentReport.Params.reportParams;
    if (!reportManager.ReportByAgent.SuperReport.CachedData.hasParams &&
        !reportManager.ReportByAgent.MasterReport.CachedData.hasParams &&
        !reportManager.ReportByAgent.AgentReport.CachedData.hasParams) {
        var backToAgentReportEvent = "reportManager.ReportByAgent.MemberReport.BackToAgentReport();";
        return '<span class="link bold" onclick="' + backToAgentReportEvent + '">' + AccReport.utility.StringFormat(viewResources.LiveCasinoAgencyWinLoss_ReportTitle,
                    [reportManager.Params.reportParams.FromDate, reportManager.Params.reportParams.ToDate]) +
               '</span> > ' + self.Params.reportParams.UserName;
    }

    if (!reportManager.ReportByAgent.SuperReport.CachedData.hasParams &&
        !reportManager.ReportByAgent.MasterReport.CachedData.hasParams) {
        var backToAgentReportEvent = "reportManager.ReportByAgent.MemberReport.BackToAgentReport();";
        return '<span class="link bold" onclick="' + backToAgentReportEvent + '">' + AccReport.utility.StringFormat(viewResources.LiveCasinoAgencyWinLoss_ReportTitle,
                    [reportManager.Params.reportParams.FromDate, reportManager.Params.reportParams.ToDate]) +
                    ' > ' + agentParams.UserName +
               '</span> > ' + self.Params.reportParams.UserName;
    }

    if (!reportManager.ReportByAgent.SuperReport.CachedData.hasParams) {
        var backToAgentReportEvent = "reportManager.ReportByAgent.MemberReport.BackToAgentReport();";
        var backToMasterReportEvent = "reportManager.ReportByAgent.MemberReport.BackToMasterReport();";
        return '<span class="link bold" onclick="' + backToMasterReportEvent + '">' + AccReport.utility.StringFormat(viewResources.LiveCasinoAgencyWinLoss_ReportTitle,
                    [reportManager.Params.reportParams.FromDate, reportManager.Params.reportParams.ToDate]) +
                    ' > ' + masterParams.UserName +
                '</span>' +
                '<span class="link bold" onclick="' + backToAgentReportEvent + '">' +
                    ' > ' + agentParams.UserName +
                '</span> > ' + self.Params.reportParams.UserName;
    }

    var backToAgentReportEvent = "reportManager.ReportByAgent.MemberReport.BackToAgentReport();";
    var backToMasterReportEvent = "reportManager.ReportByAgent.MemberReport.BackToMasterReport();";
    var backToSuperReportEvent = "reportManager.ReportByAgent.MemberReport.BackToSuperReport();";
    return '<span class="link bold" onclick="' + backToSuperReportEvent + '">' + AccReport.utility.StringFormat(viewResources.LiveCasinoAgencyWinLoss_ReportTitle,
                [reportManager.Params.reportParams.FromDate, reportManager.Params.reportParams.ToDate]) +
            '</span>' +
            '<span class="link bold" onclick="' + backToMasterReportEvent + '">' +
                ' > ' + masterParams.UserName +
            '</span> ' +
            '<span class="link bold" onclick="' + backToAgentReportEvent + '">' +
                ' > ' + agentParams.UserName +
            '</span> > ' + self.Params.reportParams.UserName;
};

Fanex.AccountingReports.Gaming.LiveCasinoAgencyWinLoss.ReportByAgent.MemberReport.prototype.GetDrilldownLink = function (currentCol, currentValue) {
    return currentValue.Name;
};

Fanex.AccountingReports.Gaming.LiveCasinoAgencyWinLoss.ReportByAgent.MemberReport.prototype.GetReport = function (username) {
    var self = this;
    reportManager.CachedData.sortColName = null;
    reportManager.CachedData.sortDirection = null;
    reportManager.CachedData.sortDataType = null;
    AccReport.disableInput();
    var uplineParams = reportManager.ReportByAgent.AgentReport.Params.reportParams;
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
    reportManager.ReportByAgent.AgentReport.HideReport();
    reportManager.ReportByAgent.CachedData.currentReportManager = self;

    AccReport.utility.CallAjax(this,
            window.rootUrl + "Gaming/LiveCasinoAgencyWinLoss/GetReport",
            self.Params.reportParams,
            self.RenderReport,
            self.RenderReport,
            AccReport.enableInput);
};

Fanex.AccountingReports.Gaming.LiveCasinoAgencyWinLoss.ReportByAgent.MemberReport.prototype.BackToAgentReport = function () {
    var self = this;
    self.HideReport();
    self.CachedData.hasParams = false;
    if (reportManager.ReportByAgent.AgentReport.CachedData.hasParams) {
        reportManager.ReportByAgent.CachedData.currentReportManager = reportManager.ReportByAgent.AgentReport;
        reportManager.ReportByAgent.AgentReport.ShowReport();
    } else {
        reportManager.ReportByAgent.CachedData.currentReportManager = reportManager.ReportByAgent.SuperReport;
        reportManager.ReportByAgent.SuperReport.GetReport();
    }
};

Fanex.AccountingReports.Gaming.LiveCasinoAgencyWinLoss.ReportByAgent.MemberReport.prototype.BackToMasterReport = function () {
    var self = this;
    self.HideReport();
    self.CachedData.hasParams = false;
    reportManager.ReportByAgent.AgentReport.CachedData.hasParams = false;
    if (reportManager.ReportByAgent.MasterReport.CachedData.hasParams) {
        reportManager.ReportByAgent.CachedData.currentReportManager = reportManager.ReportByAgent.MasterReport;
        reportManager.ReportByAgent.MasterReport.ShowReport();
    } else {
        reportManager.ReportByAgent.CachedData.currentReportManager = reportManager.ReportByAgent.SuperReport;
        reportManager.ReportByAgent.SuperReport.GetReport();
    }
};

Fanex.AccountingReports.Gaming.LiveCasinoAgencyWinLoss.ReportByAgent.MemberReport.prototype.BackToSuperReport = function () {
    var self = this;
    self.HideReport();
    self.CachedData.hasParams = false;
    reportManager.ReportByAgent.AgentReport.CachedData.hasParams = false;
    reportManager.ReportByAgent.MasterReport.CachedData.hasParams = false;
    if (reportManager.ReportByAgent.SuperReport.CachedData.hasParams) {
        reportManager.ReportByAgent.CachedData.currentReportManager = reportManager.ReportByAgent.SuperReport;
        reportManager.ReportByAgent.SuperReport.ShowReport();
    } else {
        reportManager.ReportByAgent.CachedData.currentReportManager = reportManager.ReportByAgent.SuperReport;
        reportManager.ReportByAgent.SuperReport.GetReport();
    }
};

Fanex.AccountingReports.Gaming.LiveCasinoAgencyWinLoss.ReportByAgent.MemberReport.prototype.ExportReport = function () {
    var self = this;
    self.Params.reportParams.ReportTitle = self.GetReportTitle();
    AccReport.downloadFile(window.rootUrl + "Gaming/LiveCasinoAgencyWinLoss/ExportReport", self.Params.reportParams);
};

Fanex.AccountingReports.Gaming.LiveCasinoAgencyWinLoss.ReportByAgent.MemberReport.prototype.GetReportTitle = function () {
    var self = this;
    var superParams = reportManager.ReportByAgent.SuperReport.Params.reportParams;
    var masterParams = reportManager.ReportByAgent.MasterReport.Params.reportParams;
    var agentParams = reportManager.ReportByAgent.AgentReport.Params.reportParams;
    if (!reportManager.ReportByAgent.SuperReport.CachedData.hasParams &&
        !reportManager.ReportByAgent.MasterReport.CachedData.hasParams &&
        !reportManager.ReportByAgent.AgentReport.CachedData.hasParams) {
        return AccReport.utility.StringFormat(viewResources.LiveCasinoAgencyWinLoss_ReportTitle,
                    [reportManager.Params.reportParams.FromDate, reportManager.Params.reportParams.ToDate]) +
               ' > ' + self.Params.reportParams.UserName;
    }

    if (!reportManager.ReportByAgent.SuperReport.CachedData.hasParams &&
        !reportManager.ReportByAgent.MasterReport.CachedData.hasParams) {
        return AccReport.utility.StringFormat(viewResources.LiveCasinoAgencyWinLoss_ReportTitle,
                    [reportManager.Params.reportParams.FromDate, reportManager.Params.reportParams.ToDate]) +
                    ' > ' + agentParams.UserName +
               ' > ' + self.Params.reportParams.UserName;
    }

    if (!reportManager.ReportByAgent.SuperReport.CachedData.hasParams) {
        return AccReport.utility.StringFormat(viewResources.LiveCasinoAgencyWinLoss_ReportTitle,
                    [reportManager.Params.reportParams.FromDate, reportManager.Params.reportParams.ToDate]) +
                ' > ' + masterParams.UserName +
                ' > ' + agentParams.UserName +
                ' > ' + self.Params.reportParams.UserName;
    }

    return AccReport.utility.StringFormat(viewResources.LiveCasinoAgencyWinLoss_ReportTitle,
                [reportManager.Params.reportParams.FromDate, reportManager.Params.reportParams.ToDate]) +
           ' > ' + masterParams.UserName +
           ' > ' + agentParams.UserName +
           ' > ' + self.Params.reportParams.UserName;
};