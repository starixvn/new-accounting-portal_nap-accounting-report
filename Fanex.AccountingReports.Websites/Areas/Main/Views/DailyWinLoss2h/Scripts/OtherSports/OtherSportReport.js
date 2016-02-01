OtherSportReport = function () {
    var self = this;

    self.OtherSportReport_Summary = new OtherSportReport_Summary();

    self.OtherSportReport_BySportType = new OtherSportReport_BySportType();

    self.OtherSportReport_ByBetType = new OtherSportReport_ByBetType();

    self.OtherSportsForNewCricket = new OtherSportsForNewCricket();

    self.Tables = {
        btnGenerate: null,
        btnToday: null,
        btnYesterday: null,
        btnExport: null,
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

OtherSportReport.prototype.InitPage = function () {
    this.CachedDOMElements();
    this.BindingEvent();
    this.OtherSportReport_Summary.InitPage(this);
    this.OtherSportReport_BySportType.InitPage(this);
    this.OtherSportReport_ByBetType.InitPage(this);
};

OtherSportReport.prototype.CachedDOMElements = function () {
    this.Tables.btnGenerate = $("#btnGenerate");
    this.Tables.btnToday = $("#btnToday");
    this.Tables.btnYesterday = $("#btnYesterday");
    this.Tables.btnExport = $("#btnExport");
    this.Tables.datepickerplugin = $("#datepickerplugin");
    this.Tables.datepicker = $(".ui_nexdatepicker_fromdate");
    this.Tables.reportView = $("#otherSportReport");
};

OtherSportReport.prototype.GetReport = function (params) {
    this.OtherSportReport_Summary.GetReport(params);
};

OtherSportReport.prototype.BindingEvent = function () {
    var self = this;

    self.Tables.btnGenerate.unbind("click");
    self.Tables.btnGenerate.click(function () {
        self.Tables.btnExport.attr("disabled", null);
        self.OtherSportReport_Summary.GetReport({
            Winlostdate: self.Tables.datepickerplugin.nexDatePicker("getSingleDate")
        });
    });

    self.Tables.btnToday.unbind("click");
    self.Tables.btnToday.click(function () {
        self.Tables.btnExport.attr("disabled", null);
        self.Tables.datepicker.datepicker("setDate", new Date(self.Tables.btnToday.val()));
        self.OtherSportReport_Summary.GetReport({
            Winlostdate: self.Tables.btnToday.val()
        });
    });

    self.Tables.btnYesterday.unbind("click");
    self.Tables.btnYesterday.click(function () {
        self.Tables.btnExport.attr("disabled", null);
        self.Tables.datepicker.datepicker("setDate", new Date(self.Tables.btnYesterday.val()));
        self.OtherSportReport_Summary.GetReport({
            Winlostdate: self.Tables.btnYesterday.val()
        });
    });

    self.Tables.btnExport.unbind("click");
    self.Tables.btnExport.click(function () {
        self.OtherSportReport_Summary.ExportReport();
    });
};

OtherSportReport.prototype.GetReportLink = function (info, isLive, drillDownLevel) {
    var evt = '';
    switch (drillDownLevel) {
        case reportManager.Enum.DrillDownLevel.BySportType:
            evt = "reportManager.OtherSportReport.OtherSportReport_BySportType.GetReport('" +
                this.OtherSportReport_Summary.Params.reportParams.Winlostdate + "','" +
                info.Id + "','" +
                info.Name + "'," +
                isLive + "," +
                (info.Hedge && info.Hedge != '' ? true : false) + "); return;";
            break;
        case reportManager.Enum.DrillDownLevel.ByBetType:
            evt = "reportManager.OtherSportReport.GetReport_ByBetType('" +
                this.OtherSportReport_Summary.Params.reportParams.Winlostdate + "','" +
                info.Id + "','" +
                info.AliasName + "'," +
                isLive + "," +
                (info.Hedge && info.Hedge != '' ? true : false) + "); return;";
            break;
    }

    var reportLink = '<b>\
                          <a class="report2h-link" onclick="' + evt + '">' + info.Name + '</a>\
                      </b>';
    return reportLink;
};

OtherSportReport.prototype.ShowReportView = function () {
    AccReport.popUp.hide();
    this.Tables.reportView.removeClass("display-none");
};

OtherSportReport.prototype.HideReportView = function () {
    AccReport.popUp.hide();
    this.Tables.reportView.addClass("display-none");
    this.OtherSportReport_BySportType.HideReport();
    this.OtherSportReport_ByBetType.HideReport();
};

OtherSportReport.prototype.BindingEventBySportType = function () {
    var self = this;

    self.Tables.btnGenerate.unbind("click");
    self.Tables.btnGenerate.click(function () {
        self.Tables.btnExport.attr("disabled", null);
        self.OtherSportReport_BySportType.GetReport(self.Tables.datepickerplugin.nexDatePicker("getSingleDate"),
            self.OtherSportReport_BySportType.Params.reportParams.SportType,
            self.OtherSportReport_BySportType.Params.reportParams.SportTypeName,
            self.OtherSportReport_BySportType.Params.reportParams.Liveindicator,
            self.OtherSportReport_BySportType.Params.reportParams.IsHedge);
    });

    self.Tables.btnToday.unbind("click");
    self.Tables.btnToday.click(function () {
        self.Tables.btnExport.attr("disabled", null);
        self.Tables.datepicker.datepicker("setDate", new Date(self.Tables.btnToday.val()));
        self.OtherSportReport_BySportType.GetReport(self.Tables.btnToday.val(),
            self.OtherSportReport_BySportType.Params.reportParams.SportType,
            self.OtherSportReport_BySportType.Params.reportParams.SportTypeName,
            self.OtherSportReport_BySportType.Params.reportParams.Liveindicator,
            self.OtherSportReport_BySportType.Params.reportParams.IsHedge);
    });

    self.Tables.btnYesterday.unbind("click");
    self.Tables.btnYesterday.click(function () {
        self.Tables.btnExport.attr("disabled", null);
        self.Tables.datepicker.datepicker("setDate", new Date(self.Tables.btnYesterday.val()));
        self.OtherSportReport_BySportType.GetReport(self.Tables.btnYesterday.val(),
            self.OtherSportReport_BySportType.Params.reportParams.SportType,
            self.OtherSportReport_BySportType.Params.reportParams.SportTypeName,
            self.OtherSportReport_BySportType.Params.reportParams.Liveindicator,
            self.OtherSportReport_BySportType.Params.reportParams.IsHedge);
    });

    self.Tables.btnExport.unbind("click");
    self.Tables.btnExport.click(function () {
        self.OtherSportReport_BySportType.ExportReport();
    });
};

OtherSportReport.prototype.GetReport_ByBetType = function (winlostdate,
    betTypeId,
    betTypeName,
    isLive,
    isHedge) {
    var self = this;
    self.BindingEvent_ByBetTypeReport();
    reportManager.Tables.btnShowHideSiteTypeInfo.addClass("display-none");
    self.OtherSportReport_ByBetType.GetReport(winlostdate,
        betTypeId,
        betTypeName,
        isLive,
        isHedge);
};

OtherSportReport.prototype.BindingEvent_ByBetTypeReport = function () {
    var self = this;

    self.Tables.btnGenerate.unbind("click");
    self.Tables.btnGenerate.click(function () {
        self.Tables.btnExport.attr("disabled", null);
        self.OtherSportReport_ByBetType.GetReport(self.Tables.datepickerplugin.nexDatePicker("getSingleDate"),
            self.OtherSportReport_ByBetType.Params.reportParams.Bettype,
            self.OtherSportReport_ByBetType.Params.reportParams.BetTypeName,
            self.OtherSportReport_ByBetType.Params.reportParams.Liveindicator,
            self.OtherSportReport_ByBetType.Params.reportParams.IsHedge
        );
    });

    self.Tables.btnToday.unbind("click");
    self.Tables.btnToday.click(function () {
        self.Tables.btnExport.attr("disabled", null);
        self.Tables.datepicker.datepicker("setDate", new Date(self.Tables.btnToday.val()));
        self.OtherSportReport_ByBetType.GetReport(self.Tables.btnToday.val(),
            self.OtherSportReport_ByBetType.Params.reportParams.Bettype,
            self.OtherSportReport_ByBetType.Params.reportParams.BetTypeName,
            self.OtherSportReport_ByBetType.Params.reportParams.Liveindicator,
            self.OtherSportReport_ByBetType.Params.reportParams.IsHedge
        );
    });

    self.Tables.btnYesterday.unbind("click");
    self.Tables.btnYesterday.click(function () {
        self.Tables.btnExport.attr("disabled", null);
        self.Tables.datepicker.datepicker("setDate", new Date(self.Tables.btnYesterday.val()));
        self.OtherSportReport_ByBetType.GetReport(self.Tables.btnYesterday.val(),
            self.OtherSportReport_ByBetType.Params.reportParams.Bettype,
            self.OtherSportReport_ByBetType.Params.reportParams.BetTypeName,
            self.OtherSportReport_ByBetType.Params.reportParams.Liveindicator,
            self.OtherSportReport_ByBetType.Params.reportParams.IsHedge
        );
    });

    self.Tables.btnExport.unbind("click");
    self.Tables.btnExport.click(function () {
        self.OtherSportReport_ByBetType.ExportReport();
    });
};

OtherSportReport.prototype.ShowSummaryReport = function () {
    scoreInfoManager.ClosePopup();
    this.OtherSportReport_BySportType.HideReport();
    this.OtherSportReport_ByBetType.HideReport();
    this.BindingEvent();
    this.Tables.datepicker.datepicker("setDate", this.OtherSportReport_Summary.Params.reportParams.Winlostdate);

    reportManager.Tables.btnShowHideSiteTypeInfo.removeClass("display-none");
    this.OtherSportReport_Summary.ShowReport();
    this.Tables.reportView.removeClass("display-none");
    this.BindingEvent();
    this.SetActiveReportManager();
    this.OtherSportReport_Summary.ShowHideSiteTypeInfo();
};

OtherSportReport.prototype.ShowReportBySportType = function () {
    scoreInfoManager.ClosePopup();
    this.OtherSportReport_Summary.HideReport();
    this.OtherSportReport_ByBetType.HideReport();
    reportManager.WinLossBySite_BetType.HideReportView();
    this.Tables.datepicker.datepicker("setDate", this.OtherSportReport_BySportType.Params.reportParams.Winlostdate);

    reportManager.Tables.btnShowHideSiteTypeInfo.removeClass("display-none");
    this.BindingEventBySportType();
    this.OtherSportReport_BySportType.ShowReport();
    this.Tables.reportView.removeClass("display-none");
    this.SetActiveReportManager();
    this.OtherSportReport_BySportType.ShowHideSiteTypeInfo();
};

OtherSportReport.prototype.GetFullBreadCrumbs = function (drillDownLevel, selectParams) {
    var self = this;
    switch (drillDownLevel) {
        case reportManager.Enum.DrillDownLevel.BySportType:
            return "<a class='breadcrumbs-link' onclick='showHideReportManager.BackToMainReport();'>" + viewResources.Main + "</a> |" +
                   " <a class='breadcrumbs-link' onclick='reportManager.OtherSportReport.ShowSummaryReport();'>" + self.CacheData.SportName + "</a>";
            break;
        case reportManager.Enum.DrillDownLevel.ByBetType:
            return "<a class='breadcrumbs-link' onclick='showHideReportManager.BackToMainReport();'>" + viewResources.Main + "</a> >" +
                   " <a class='breadcrumbs-link' onclick='reportManager.OtherSportReport.ShowSummaryReport();'>" + self.CacheData.SportName + "</a> |" +
                   " <a class='breadcrumbs-link' onclick='reportManager.OtherSportReport.ShowReportBySportType();'>" + self.OtherSportReport_BySportType.Params.reportParams.SportTypeName + "</a> >" +
                     " " + selectParams.BetTypeName +
                        " \"" + (selectParams.IsLive ?
                                 viewResources.Live :
                                 viewResources.Symbol_Sparse_Non_Live) + "\"";
            break;
        default:
            return "";
    }
};

OtherSportReport.prototype.GetPlainPreviousBreadCrumbs = function (drillDownLevel, selectParams) {
    var self = this;
    switch (drillDownLevel) {
        case reportManager.Enum.DrillDownLevel.BySportType:
            return viewResources.Main + " >" +
                   " " + self.CacheData.SportName;
            break;
        case reportManager.Enum.DrillDownLevel.ByBetType:
            return viewResources.Main + " >" +
                   " " + self.CacheData.SportName + " |" +
                   " " + self.OtherSportReport_BySportType.Params.reportParams.SportTypeName + " >" +
                   " " + selectParams.BetTypeName +
                        " \"" + (selectParams.IsLive ?
                                 viewResources.Live :
                                 viewResources.Symbol_Sparse_Non_Live) + "\"";
            break;
        default:
            return "";
    }
};

OtherSportReport.prototype.SetActiveReportManager = function () {
    if (showHideReportManager.CacheData.currentReport != this) {
        showHideReportManager.CacheData.currentReport.HideReportView();
        showHideReportManager.CacheData.currentReport = this;
    }
};

OtherSportReport.prototype.ShowHideSiteTypeInfo = function () {
    var self = this;
    self.OtherSportReport_Summary.ShowHideSiteTypeInfo();
    self.OtherSportReport_BySportType.ShowHideSiteTypeInfo();
};