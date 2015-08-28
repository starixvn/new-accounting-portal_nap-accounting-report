RacingReport = function () {
    var self = this;

    self.RacingReport_Summary = new RacingReport_Summary();

    self.RacingReport_BySportType = new RacingReport_BySportType();

    self.RacingReport_ByBetType = new RacingReport_ByBetType();

    self.Tables = {
        btnGenerate: null,
        btnToday: null,
        btnYesterday: null,
        btnExport: null,
        btnBack: null,
        datepickerplugin: null,
        datepicker: null,

        reportView: null,
        gridSplitter: null
    };

    self.CacheData = {
        SportId: null,
        SportName: null
    };
};

RacingReport.prototype.InitPage = function () {
    this.CachedDOMElements();
    this.BindingEvent();
    this.RacingReport_Summary.InitPage(this);
    this.RacingReport_BySportType.InitPage(this);
    this.RacingReport_ByBetType.InitPage(this);
};

RacingReport.prototype.CachedDOMElements = function () {
    this.Tables.btnGenerate = $("#btnGenerate");
    this.Tables.btnToday = $("#btnToday");
    this.Tables.btnYesterday = $("#btnYesterday");
    this.Tables.btnExport = $("#btnExport");
    this.Tables.datepickerplugin = $("#datepickerplugin");
    this.Tables.datepicker = $(".ui_nexdatepicker_fromdate");

    this.Tables.reportView = $("#racingReport");
    this.Tables.gridSplitter = this.Tables.reportView.find(".gridSplitter");
};

RacingReport.prototype.BindingEvent = function () {
    var self = this;

    self.Tables.btnGenerate.unbind("click");
    self.Tables.btnGenerate.click(function () {
        self.Tables.btnExport.attr("disabled", null);
        self.RacingReport_Summary.GetReport({
            Winlostdate: self.Tables.datepickerplugin.nexDatePicker("getSingleDate")
        });
    });

    self.Tables.btnToday.unbind("click");
    self.Tables.btnToday.click(function () {
        self.Tables.btnExport.attr("disabled", null);
        self.Tables.datepicker.datepicker("setDate", new Date(self.Tables.btnToday.val()));
        self.RacingReport_Summary.GetReport({
            Winlostdate: self.Tables.btnToday.val()
        });
    });

    self.Tables.btnYesterday.unbind("click");
    self.Tables.btnYesterday.click(function () {
        self.Tables.btnExport.attr("disabled", null);
        self.Tables.datepicker.datepicker("setDate", new Date(self.Tables.btnYesterday.val()));
        self.RacingReport_Summary.GetReport({
            Winlostdate: self.Tables.btnYesterday.val()
        });
    });

    self.Tables.btnExport.unbind("click");
    self.Tables.btnExport.click(function () {
        self.RacingReport_Summary.ExportReport();
    });
};

RacingReport.prototype.GetReport = function (params) {
    this.RacingReport_Summary.GetReport(params);
};

RacingReport.prototype.GetReportLink = function (info, isLive, drillDownLevel) {
    var evt = '';
    switch (drillDownLevel) {
        case reportManager.Enum.DrillDownLevel.BySportType:
            evt = "reportManager.RacingReport.RacingReport_BySportType.GetReport('" +
                this.RacingReport_Summary.Params.reportParams.Winlostdate + "','" +
                info.Id + "','" +
                info.Name + "'," +
                isLive + "," +
                (info.Hedge && info.Hedge != '' ? true : false) + "); return;";
            break;
        case reportManager.Enum.DrillDownLevel.ByBetType:
            evt = "reportManager.RacingReport.GetReport_ByBetType('" +
                this.RacingReport_Summary.Params.reportParams.Winlostdate + "','" +
                info.Id + "','" +
                info.Name + "'," +
                isLive + "," +
                (info.Hedge && info.Hedge != '' ? true : false) + "); return;";
            break;
    }

    var reportLink = '<b>\
                          <a class="report2h-link" onclick="' + evt + '">' + info.Name + '</a>\
                      </b>';
    return reportLink;
};

RacingReport.prototype.ShowReportView = function () {
    this.Tables.reportView.removeClass("display-none");
};

RacingReport.prototype.HideReportView = function () {
    this.Tables.reportView.addClass("display-none");
    this.RacingReport_BySportType.HideReport();
    this.RacingReport_ByBetType.HideReport();
};

RacingReport.prototype.BindingEventBySportType = function () {
    var self = this;

    self.Tables.btnGenerate.unbind("click");
    self.Tables.btnGenerate.click(function () {
        self.Tables.btnExport.attr("disabled", null);
        self.RacingReport_BySportType.GetReport(self.Tables.datepickerplugin.nexDatePicker("getSingleDate"),
            self.RacingReport_BySportType.Params.reportParams.SportType,
            self.RacingReport_BySportType.Params.reportParams.SportName,
            self.RacingReport_BySportType.Params.reportParams.Liveindicator,
            self.RacingReport_BySportType.Params.reportParams.IsHedge);
    });

    self.Tables.btnToday.unbind("click");
    self.Tables.btnToday.click(function () {
        self.Tables.btnExport.attr("disabled", null);
        self.Tables.datepicker.datepicker("setDate", new Date(self.Tables.btnToday.val()));
        self.RacingReport_BySportType.GetReport(self.Tables.btnToday.val(),
            self.RacingReport_BySportType.Params.reportParams.SportType,
            self.RacingReport_BySportType.Params.reportParams.SportName,
            self.RacingReport_BySportType.Params.reportParams.Liveindicator,
            self.RacingReport_BySportType.Params.reportParams.IsHedge);
    });

    self.Tables.btnYesterday.unbind("click");
    self.Tables.btnYesterday.click(function () {
        self.Tables.btnExport.attr("disabled", null);
        self.Tables.datepicker.datepicker("setDate", new Date(self.Tables.btnYesterday.val()));
        self.RacingReport_BySportType.GetReport(self.Tables.btnYesterday.val(),
            self.RacingReport_BySportType.Params.reportParams.SportType,
            self.RacingReport_BySportType.Params.reportParams.SportName,
            self.RacingReport_BySportType.Params.reportParams.Liveindicator,
            self.RacingReport_BySportType.Params.reportParams.IsHedge);
    });

    self.Tables.btnExport.unbind("click");
    self.Tables.btnExport.click(function () {
        self.RacingReport_BySportType.ExportReport();
    });
};

RacingReport.prototype.GetReport_ByBetType = function (winlostdate,
    betTypeId,
    betTypeName,
    isLive,
    isHedge) {
    var self = this;
    self.BindingEvent_ByBetTypeReport();
    reportManager.Tables.btnShowHideSiteTypeInfo.addClass("display-none");
    self.RacingReport_ByBetType.GetReport(winlostdate,
        betTypeId,
        betTypeName,
        isLive,
        isHedge);
};

RacingReport.prototype.BindingEvent_ByBetTypeReport = function () {
    var self = this;

    self.Tables.btnGenerate.unbind("click");
    self.Tables.btnGenerate.click(function () {
        self.Tables.btnExport.attr("disabled", null);
        self.RacingReport_ByBetType.GetReport(self.Tables.datepickerplugin.nexDatePicker("getSingleDate"),
            self.RacingReport_ByBetType.Params.reportParams.Bettype,
            self.RacingReport_ByBetType.Params.reportParams.BetTypeName,
            self.RacingReport_ByBetType.Params.reportParams.Liveindicator,
            self.RacingReport_ByBetType.Params.reportParams.IsHedge
        );
    });

    self.Tables.btnToday.unbind("click");
    self.Tables.btnToday.click(function () {
        self.Tables.btnExport.attr("disabled", null);
        self.Tables.datepicker.datepicker("setDate", new Date(self.Tables.btnToday.val()));
        self.RacingReport_ByBetType.GetReport(self.Tables.btnToday.val(),
            self.RacingReport_ByBetType.Params.reportParams.Bettype,
            self.RacingReport_ByBetType.Params.reportParams.BetTypeName,
            self.RacingReport_ByBetType.Params.reportParams.Liveindicator,
            self.RacingReport_ByBetType.Params.reportParams.IsHedge
        );
    });

    self.Tables.btnYesterday.unbind("click");
    self.Tables.btnYesterday.click(function () {
        self.Tables.btnExport.attr("disabled", null);
        self.Tables.datepicker.datepicker("setDate", new Date(self.Tables.btnYesterday.val()));
        self.RacingReport_ByBetType.GetReport(self.Tables.btnYesterday.val(),
            self.RacingReport_ByBetType.Params.reportParams.Bettype,
            self.RacingReport_ByBetType.Params.reportParams.BetTypeName,
            self.RacingReport_ByBetType.Params.reportParams.Liveindicator,
            self.RacingReport_ByBetType.Params.reportParams.IsHedge
        );
    });

    self.Tables.btnExport.unbind("click");
    self.Tables.btnExport.click(function () {
        self.RacingReport_ByBetType.ExportReport();
    });
};

RacingReport.prototype.ShowSummaryReport = function () {
    scoreInfoManager.ClosePopup();
    this.RacingReport_BySportType.HideReport();
    this.RacingReport_ByBetType.HideReport();
    this.BindingEvent();
    this.Tables.datepicker.datepicker("setDate", this.RacingReport_Summary.Params.reportParams.Winlostdate);
    this.RacingReport_Summary.ShowReport();
    this.Tables.reportView.removeClass("display-none");
    this.BindingEvent();
    this.SetActiveReportManager();
    this.RacingReport_Summary.ShowHideSiteTypeInfo();
};

RacingReport.prototype.ShowReportBySportType = function () {
    this.RacingReport_Summary.HideReport();
    this.RacingReport_ByBetType.HideReport();
    reportManager.WinLossBySite_BetType.HideReportView();
    this.Tables.datepicker.datepicker("setDate", this.RacingReport_BySportType.Params.reportParams.Winlostdate);
    this.BindingEventBySportType();
    this.RacingReport_BySportType.ShowReport();
    this.Tables.reportView.removeClass("display-none");
    this.SetActiveReportManager();
    this.RacingReport_BySportType.ShowHideSiteTypeInfo();
};

RacingReport.prototype.GetFullBreadCrumbs = function (drillDownLevel, selectParams) {
    var self = this;
    switch (drillDownLevel) {
        case reportManager.Enum.DrillDownLevel.BySportType:
            return "<a class='breadcrumbs-link' onclick='showHideReportManager.BackToMainReport();'>" + viewResources.Main + "</a> >" +
                   " <a class='breadcrumbs-link' onclick='reportManager.RacingReport.ShowSummaryReport();'>" + self.CacheData.SportName + "</a>";
            break;
        case reportManager.Enum.DrillDownLevel.ByBetType:
            return "<a class='breadcrumbs-link' onclick='showHideReportManager.BackToMainReport();'>" + viewResources.Main + "</a> >" +
                   " <a class='breadcrumbs-link' onclick='reportManager.RacingReport.ShowSummaryReport();'>" + self.CacheData.SportName + "</a> |" +
                   " <a class='breadcrumbs-link' onclick='reportManager.RacingReport.ShowReportBySportType();'>" + self.RacingReport_BySportType.Params.reportParams.SportName + "</a> >" +
                    " " + selectParams.BetTypeName;
            break;
        default:
            return "";
    }
};

RacingReport.prototype.GetPlainPreviousBreadCrumbs = function (drillDownLevel, selectParams) {
    var self = this;
    switch (drillDownLevel) {
        case reportManager.Enum.DrillDownLevel.BySportType:
            return viewResources.Main + " >" +
                   " " + self.CacheData.SportName;
            break;
        case reportManager.Enum.DrillDownLevel.ByBetType:
            return viewResources.Main + " >" +
                   " " + self.CacheData.SportName + " |" +
                   " " + self.RacingReport_BySportType.Params.reportParams.SportName + " >" +
                   " " + selectParams.BetTypeName;
            break;
        default:
            return "";
    }
};

RacingReport.prototype.SetActiveReportManager = function () {
    if (showHideReportManager.CacheData.currentReport != this) {
        showHideReportManager.CacheData.currentReport.HideReportView();
        showHideReportManager.CacheData.currentReport = this;
    }
};

RacingReport.prototype.ShowHideSiteTypeInfo = function () {
    var self = this;
    self.RacingReport_Summary.ShowHideSiteTypeInfo();
    self.RacingReport_BySportType.ShowHideSiteTypeInfo();
};
