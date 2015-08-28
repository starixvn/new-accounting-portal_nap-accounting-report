
Namespace.Register("Fanex.AccountingReports.Misc.MAMonitorTool");
Fanex.AccountingReports.Misc.MAMonitorTool.HistoryReport = function () {
};

Inheritance_Manager.extend(Fanex.AccountingReports.Misc.MAMonitorTool.HistoryReport,
    Fanex.AccountingReports.Misc.MAMonitorTool.BaseReport);

Fanex.AccountingReports.Misc.MAMonitorTool.HistoryReport.prototype.CachedDOMElements = function () {
    var self = this;
    self.Tables.reportContainer = $("#historyMAMonitorToolReportContainer");
    self.Tables.report = $("#historyMAMonitorToolReport");
};
