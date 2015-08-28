
Namespace.Register("Fanex.AccountingReports.Misc.MAMonitorTool");
Fanex.AccountingReports.Misc.MAMonitorTool.NormalReport = function () {
};

Inheritance_Manager.extend(Fanex.AccountingReports.Misc.MAMonitorTool.NormalReport,
    Fanex.AccountingReports.Misc.MAMonitorTool.BaseReport);

Fanex.AccountingReports.Misc.MAMonitorTool.NormalReport.prototype.CachedDOMElements = function () {
    var self = this;
    self.Tables.reportContainer = $("#mainMAMonitorToolReportContainer");
    self.Tables.report = $("#mainMAMonitorToolReport");
};

