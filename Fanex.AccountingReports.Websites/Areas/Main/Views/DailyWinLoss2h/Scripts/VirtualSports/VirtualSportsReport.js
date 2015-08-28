VirtualSportReport = function () {
    var self = this;

    self.VirtualSportsReport_Summary = new VirtualSportsReport_Summary();
    self.VirtualSportsReport_BySportType = new VirtualSportsReport_BySportType();
    self.VirtualSportsReport_ByBetType = new VirtualSportsReport_ByBetType();
    self.VirtualSports_VirtualHorseRacingReport = new VirtualSports_VirtualHorseRacingReport();

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

VirtualSportReport.prototype.InitPage = function () {
    this.CachedDOMElements();
    this.BindingEvent();
    this.VirtualSportsReport_Summary.InitPage(this);
    this.VirtualSportsReport_BySportType.InitPage(this);
    this.VirtualSportsReport_ByBetType.InitPage(this);
    this.VirtualSports_VirtualHorseRacingReport.InitPage(this);
};

VirtualSportReport.prototype.CachedDOMElements = function () {
    this.Tables.btnGenerate = $("#btnGenerate");
    this.Tables.btnToday = $("#btnToday");
    this.Tables.btnYesterday = $("#btnYesterday");
    this.Tables.btnExport = $("#btnExport");
    this.Tables.datepickerplugin = $("#datepickerplugin");
    this.Tables.datepicker = $(".ui_nexdatepicker_fromdate");
    this.Tables.reportView = $("#virtualSportReport");
};

VirtualSportReport.prototype.GetReport = function (params) {
    this.VirtualSportsReport_Summary.GetReport(params);
};

VirtualSportReport.prototype.BindingEvent = function () {
    var self = this;

    self.Tables.btnGenerate.unbind("click");
    self.Tables.btnGenerate.click(function () {
        self.Tables.btnExport.attr("disabled", null);
        self.VirtualSportsReport_Summary.GetReport({
            Winlostdate: self.Tables.datepickerplugin.nexDatePicker("getSingleDate")
        });
    });

    self.Tables.btnToday.unbind("click");
    self.Tables.btnToday.click(function () {
        self.Tables.btnExport.attr("disabled", null);
        self.Tables.datepicker.datepicker("setDate", new Date(self.Tables.btnToday.val()));
        self.VirtualSportsReport_Summary.GetReport({
            Winlostdate: self.Tables.btnToday.val()
        });
    });

    self.Tables.btnYesterday.unbind("click");
    self.Tables.btnYesterday.click(function () {
        self.Tables.btnExport.attr("disabled", null);
        self.Tables.datepicker.datepicker("setDate", new Date(self.Tables.btnYesterday.val()));
        self.VirtualSportsReport_Summary.GetReport({
            Winlostdate: self.Tables.btnYesterday.val()
        });
    });

    self.Tables.btnExport.unbind("click");
    self.Tables.btnExport.click(function () {
        self.VirtualSportsReport_Summary.ExportReport();
    });
};

VirtualSportReport.prototype.GetReportLink = function (info, isLive, drillDownLevel) {
    var evt = '';
    switch (drillDownLevel) {
        case reportManager.Enum.DrillDownLevel.BySportType:
            evt = "reportManager.VirtualSportReport.VirtualSportsReport_BySportType.GetReport('" + this.VirtualSportsReport_Summary.Params.reportParams.Winlostdate + "','" +
                info.Id + "','" +
                info.Name + "'," +
                info.IsLive + "," +
                (info.Hedge && info.Hedge != '' ? true : false) + "); return;";
            break;
        case reportManager.Enum.DrillDownLevel.ByBetType:
            evt = "reportManager.VirtualSportReport.GetReport_ByBetType('" + this.VirtualSportsReport_Summary.Params.reportParams.Winlostdate + "','" +
                info.Id + "','" +
                info.Name + "'," +
                info.IsLive + "," +
                (info.Hedge && info.Hedge != '' ? true : false) + "); return;";
            break;
    }

    var reportLink = '<b>\
                          <a class="report2h-link" onclick="' + evt + '">' + info.Name + '</a>\
                      </b>';
    return reportLink;
};

VirtualSportReport.prototype.ShowReportView = function () {
    AccReport.popUp.hide();
    this.Tables.reportView.removeClass("display-none");
};

VirtualSportReport.prototype.HideReportView = function () {
    AccReport.popUp.hide();
    this.Tables.reportView.addClass("display-none");
    this.VirtualSportsReport_BySportType.HideReport();
    this.VirtualSportsReport_ByBetType.HideReport();
};

VirtualSportReport.prototype.BindingEventBySportType = function () {
    var self = this;

    self.Tables.btnGenerate.unbind("click");
    self.Tables.btnGenerate.click(function () {
        self.Tables.btnExport.attr("disabled", null);
        self.VirtualSportsReport_BySportType.GetReport(self.Tables.datepickerplugin.nexDatePicker("getSingleDate"),
            self.VirtualSportsReport_BySportType.Params.reportParams.SportType,
            self.VirtualSportsReport_BySportType.Params.reportParams.SportTypeName,
            self.VirtualSportsReport_BySportType.Params.reportParams.Liveindicator,
            self.VirtualSportsReport_BySportType.Params.reportParams.IsHedge);
    });

    self.Tables.btnToday.unbind("click");
    self.Tables.btnToday.click(function () {
        self.Tables.btnExport.attr("disabled", null);
        self.Tables.datepicker.datepicker("setDate", new Date(self.Tables.btnToday.val()));
        self.VirtualSportsReport_BySportType.GetReport(self.Tables.btnToday.val(),
            self.VirtualSportsReport_BySportType.Params.reportParams.SportType,
            self.VirtualSportsReport_BySportType.Params.reportParams.SportTypeName,
            self.VirtualSportsReport_BySportType.Params.reportParams.Liveindicator,
            self.VirtualSportsReport_BySportType.Params.reportParams.IsHedge);
    });

    self.Tables.btnYesterday.unbind("click");
    self.Tables.btnYesterday.click(function () {
        self.Tables.btnExport.attr("disabled", null);
        self.Tables.datepicker.datepicker("setDate", new Date(self.Tables.btnYesterday.val()));
        self.VirtualSportsReport_BySportType.GetReport(self.Tables.btnYesterday.val(),
            self.VirtualSportsReport_BySportType.Params.reportParams.SportType,
            self.VirtualSportsReport_BySportType.Params.reportParams.SportTypeName,
            self.VirtualSportsReport_BySportType.Params.reportParams.Liveindicator,
            self.VirtualSportsReport_BySportType.Params.reportParams.IsHedge);
    });

    self.Tables.btnExport.unbind("click");
    self.Tables.btnExport.click(function () {
        self.VirtualSportsReport_BySportType.ExportReport();
    });
};

VirtualSportReport.prototype.GetReport_ByBetType = function (winlostdate,
    betTypeId,
    betTypeName,
    isLive,
    isHedge) {
    var self = this;
    self.BindingEvent_ByBetTypeReport();
    reportManager.Tables.btnShowHideSiteTypeInfo.addClass("display-none");
    self.VirtualSportsReport_ByBetType.GetReport(winlostdate,
        betTypeId,
        betTypeName,
        isLive,
        isHedge);
};

VirtualSportReport.prototype.BindingEvent_ByBetTypeReport = function () {
    var self = this;

    self.Tables.btnGenerate.unbind("click");
    self.Tables.btnGenerate.click(function () {
        self.Tables.btnExport.attr("disabled", null);
        self.VirtualSportsReport_ByBetType.GetReport(self.Tables.datepickerplugin.nexDatePicker("getSingleDate"),
            self.VirtualSportsReport_ByBetType.Params.reportParams.Bettype,
            self.VirtualSportsReport_ByBetType.Params.reportParams.BetTypeName,
            self.VirtualSportsReport_ByBetType.Params.reportParams.Liveindicator,
            self.VirtualSportsReport_ByBetType.Params.reportParams.IsHedge
        );
    });

    self.Tables.btnToday.unbind("click");
    self.Tables.btnToday.click(function () {
        self.Tables.btnExport.attr("disabled", null);
        self.Tables.datepicker.datepicker("setDate", new Date(self.Tables.btnToday.val()));
        self.VirtualSportsReport_ByBetType.GetReport(self.Tables.btnToday.val(),
            self.VirtualSportsReport_ByBetType.Params.reportParams.Bettype,
            self.VirtualSportsReport_ByBetType.Params.reportParams.BetTypeName,
            self.VirtualSportsReport_ByBetType.Params.reportParams.Liveindicator,
            self.VirtualSportsReport_ByBetType.Params.reportParams.IsHedge
        );
    });

    self.Tables.btnYesterday.unbind("click");
    self.Tables.btnYesterday.click(function () {
        self.Tables.btnExport.attr("disabled", null);
        self.Tables.datepicker.datepicker("setDate", new Date(self.Tables.btnYesterday.val()));
        self.VirtualSportsReport_ByBetType.GetReport(self.Tables.btnYesterday.val(),
            self.VirtualSportsReport_ByBetType.Params.reportParams.Bettype,
            self.VirtualSportsReport_ByBetType.Params.reportParams.BetTypeName,
            self.VirtualSportsReport_ByBetType.Params.reportParams.Liveindicator,
            self.VirtualSportsReport_ByBetType.Params.reportParams.IsHedge
        );
    });

    self.Tables.btnExport.unbind("click");
    self.Tables.btnExport.click(function () {
        self.VirtualSportsReport_ByBetType.ExportReport();
    });
};

VirtualSportReport.prototype.ShowSummaryReport = function () {
    scoreInfoManager.ClosePopup();
    this.VirtualSportsReport_BySportType.HideReport();
    this.VirtualSportsReport_ByBetType.HideReport();
    this.VirtualSports_VirtualHorseRacingReport.HideReport();
    this.BindingEvent();
    this.Tables.datepicker.datepicker("setDate", this.VirtualSportsReport_Summary.Params.reportParams.Winlostdate);

    reportManager.Tables.btnShowHideSiteTypeInfo.removeClass("display-none");
    this.BindingEvent();
    this.SetActiveReportManager();

    this.VirtualSportsReport_Summary.ShowReport();
    this.Tables.reportView.removeClass("display-none");
    this.VirtualSportsReport_Summary.ShowHideSiteTypeInfo();
};

VirtualSportReport.prototype.ShowReportBySportType = function () {
    scoreInfoManager.ClosePopup();
    this.VirtualSportsReport_Summary.HideReport();
    this.VirtualSportsReport_ByBetType.HideReport();
    this.VirtualSports_VirtualHorseRacingReport.HideReport();
    reportManager.WinLossBySite_BetType.HideReportView();
    this.Tables.datepicker.datepicker("setDate", this.VirtualSportsReport_BySportType.Params.reportParams.Winlostdate);

    reportManager.Tables.btnShowHideSiteTypeInfo.removeClass("display-none");
    this.BindingEventBySportType();
    this.SetActiveReportManager();

    this.VirtualSportsReport_BySportType.ShowReport();
    this.Tables.reportView.removeClass("display-none");
    this.VirtualSportsReport_BySportType.ShowHideSiteTypeInfo();
};

VirtualSportReport.prototype.BindingEvent_ByBetTypeReport_VirtualHorseRacingReport = function () {
    var self = this;

    self.Tables.btnGenerate.unbind("click");
    self.Tables.btnGenerate.click(function () {
        self.Tables.btnExport.attr("disabled", null);
        self.VirtualSports_VirtualHorseRacingReport.GetReport({
            Winlostdate: self.Tables.datepickerplugin.nexDatePicker("getSingleDate"),
            Bettype: self.VirtualSports_VirtualHorseRacingReport.Params.reportParams.Bettype,
            BetTypeName: self.VirtualSports_VirtualHorseRacingReport.Params.reportParams.BetTypeName,
            Liveindicator: self.VirtualSports_VirtualHorseRacingReport.Params.reportParams.Liveindicator,
            IsHedge: self.VirtualSports_VirtualHorseRacingReport.Params.reportParams.IsHedge
        });
    });

    self.Tables.btnToday.unbind("click");
    self.Tables.btnToday.click(function () {
        self.Tables.btnExport.attr("disabled", null);
        self.Tables.datepicker.datepicker("setDate", new Date(self.Tables.btnToday.val()));
        self.VirtualSports_VirtualHorseRacingReport.GetReport({
            Winlostdate: self.Tables.btnToday.val(),
            Bettype: self.VirtualSports_VirtualHorseRacingReport.Params.reportParams.Bettype,
            BetTypeName: self.VirtualSports_VirtualHorseRacingReport.Params.reportParams.BetTypeName,
            Liveindicator: self.VirtualSports_VirtualHorseRacingReport.Params.reportParams.Liveindicator,
            IsHedge: self.VirtualSports_VirtualHorseRacingReport.Params.reportParams.IsHedge
        });
    });

    self.Tables.btnYesterday.unbind("click");
    self.Tables.btnYesterday.click(function () {
        self.Tables.btnExport.attr("disabled", null);
        self.Tables.datepicker.datepicker("setDate", new Date(self.Tables.btnYesterday.val()));
        self.VirtualSports_VirtualHorseRacingReport.GetReport({
            Winlostdate: self.Tables.btnYesterday.val(),
            Bettype: self.VirtualSports_VirtualHorseRacingReport.Params.reportParams.Bettype,
            BetTypeName: self.VirtualSports_VirtualHorseRacingReport.Params.reportParams.BetTypeName,
            Liveindicator: self.VirtualSports_VirtualHorseRacingReport.Params.reportParams.Liveindicator,
            IsHedge: self.VirtualSports_VirtualHorseRacingReport.Params.reportParams.IsHedge
        });
    });

    self.Tables.btnExport.unbind("click");
    self.Tables.btnExport.click(function () {
        self.VirtualSports_VirtualHorseRacingReport.ExportReport();
    });
};

VirtualSportReport.prototype.GetFullBreadCrumbs = function (drillDownLevel, selectParams) {
    var self = this;
    switch (drillDownLevel) {
        case reportManager.Enum.DrillDownLevel.BySportType:
            return "<a class='breadcrumbs-link' onclick='showHideReportManager.BackToMainReport();'>" + viewResources.Main + "</a> >" +
                   " <a class='breadcrumbs-link' onclick='reportManager.VirtualSportReport.ShowSummaryReport();'>" + self.CacheData.SportName + "</a>";
            break;
        case reportManager.Enum.DrillDownLevel.ByBetType:
            return "<a class='breadcrumbs-link' onclick='showHideReportManager.BackToMainReport();'>" + viewResources.Main + "</a> >" +
                   " <a class='breadcrumbs-link' onclick='reportManager.VirtualSportReport.ShowSummaryReport();'>" + self.CacheData.SportName + "</a> |" +
                   " <a class='breadcrumbs-link' onclick='reportManager.VirtualSportReport.ShowReportBySportType();'>" + self.VirtualSportsReport_BySportType.Params.reportParams.SportTypeName + "</a> >" +
                     " " + selectParams.BetTypeName;
            break;
        default:
            return "";
    }
};

VirtualSportReport.prototype.GetPlainPreviousBreadCrumbs = function (drillDownLevel, selectParams) {
    var self = this;
    switch (drillDownLevel) {
        case reportManager.Enum.DrillDownLevel.BySportType:
            return viewResources.Main + " >" +
                   " " + self.CacheData.SportName;
            break;
        case reportManager.Enum.DrillDownLevel.ByBetType:
            return viewResources.Main + " >" +
                   " " + self.CacheData.SportName + " |" +
                   " " + self.VirtualSportsReport_BySportType.Params.reportParams.SportTypeName + " >" +
                   " " + selectParams.BetTypeName;
            break;
        default:
            return "";
    }
};

VirtualSportReport.prototype.SetActiveReportManager = function () {
    if (showHideReportManager.CacheData.currentReport != this) {
        showHideReportManager.CacheData.currentReport.HideReportView();
        showHideReportManager.CacheData.currentReport = this;
    }
};

VirtualSportReport.prototype.ShowHideSiteTypeInfo = function () {
    var self = this;
    self.VirtualSportsReport_Summary.ShowHideSiteTypeInfo();
    self.VirtualSportsReport_BySportType.ShowHideSiteTypeInfo();
};
