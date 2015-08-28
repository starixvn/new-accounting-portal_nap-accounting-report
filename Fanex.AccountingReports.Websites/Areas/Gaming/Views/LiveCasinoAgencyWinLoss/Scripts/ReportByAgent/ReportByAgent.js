Namespace.Register("Fanex.AccountingReports.Gaming.LiveCasinoAgencyWinLoss");
Fanex.AccountingReports.Gaming.LiveCasinoAgencyWinLoss.ReportByAgent = function () {
    var self = this;
    self.SuperReport = new Fanex.AccountingReports.Gaming.LiveCasinoAgencyWinLoss.ReportByAgent.SuperReport();
    self.MasterReport = new Fanex.AccountingReports.Gaming.LiveCasinoAgencyWinLoss.ReportByAgent.MasterReport();
    self.AgentReport = new Fanex.AccountingReports.Gaming.LiveCasinoAgencyWinLoss.ReportByAgent.AgentReport();
    self.MemberReport = new Fanex.AccountingReports.Gaming.LiveCasinoAgencyWinLoss.ReportByAgent.MemberReport();

    self.CachedData = {
        currentReportManager: null
    };

    self.InitPage = function () {
        self.SuperReport.InitPage();
        self.MasterReport.InitPage();
        self.AgentReport.InitPage();
        self.MemberReport.InitPage();
    };

    self.RenderReport = function (report) {
        var roleId = parseInt(report.Id);
        var roleIdConstants = AccReportData.Constants.RoleId;
        var drilldownManager = null;
        if (self.CachedData.currentReportManager) {
            self.CachedData.currentReportManager = null;
            self.Reset();
        }

        switch (roleId) {
            case roleIdConstants.Super:
                drilldownManager = self.SuperReport;
                break;
            case roleIdConstants.Master:
                drilldownManager = self.MasterReport;
                break;
            case roleIdConstants.Agent:
                drilldownManager = self.AgentReport;
                break;
            case roleIdConstants.Member:
                drilldownManager = self.MemberReport;
                break;
            default:
                drilldownManager = self.SuperReport;
                break;
        }

        self.CachedData.currentReportManager = drilldownManager;
        self.CachedData.currentReportManager.Params.reportParams = reportManager.Params.reportParams;
        self.CachedData.currentReportManager.CachedData.hasParams = true;
        drilldownManager.RenderReport(report);
    };

    self.RenderReportAfterServerSort = function (report) {
        self.CachedData.currentReportManager.RenderReport(report);
    };


    self.Reset = function (report) {
        self.SuperReport.HideReport();
        self.MasterReport.HideReport();
        self.AgentReport.HideReport();
        self.MemberReport.HideReport();

        self.SuperReport.CachedData.hasParams = false;
        self.MasterReport.CachedData.hasParams = false;
        self.AgentReport.CachedData.hasParams = false;
        self.MemberReport.CachedData.hasParams = false;
    };
};






