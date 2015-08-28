SoccerReport = function () {
    var self = this;

    self.SoccerReport_Summary = new SoccerReport_Summary();

    self.SoccerReport_ByBetType = new SoccerReport_ByBetType();

    self.Tables = {
        btnGenerate: null,
        btnToday: null,
        btnYesterday: null,
        btnExport: null,
        datepickerplugin: null,
        datepicker: null,
        reportView: null,
        gridSplitter: null,
        soccerReportContainer: null
    };

    self.CacheData = {
        SportId: null,
        SportName: null
    };
};

SoccerReport.prototype.InitPage = function () {
    this.CachedDOMElements();
    this.BindingEvent_Summary();
    this.SoccerReport_Summary.InitPage(this);
    this.SoccerReport_ByBetType.InitPage(this);
};

SoccerReport.prototype.CachedDOMElements = function () {
    this.Tables.btnGenerate = $("#btnGenerate");
    this.Tables.btnToday = $("#btnToday");
    this.Tables.btnYesterday = $("#btnYesterday");
    this.Tables.btnExport = $("#btnExport");
    this.Tables.datepickerplugin = $("#datepickerplugin");
    this.Tables.datepicker = $(".ui_nexdatepicker_fromdate");
    this.Tables.reportView = $("#soccerReport");
    this.Tables.gridSplitter = this.Tables.reportView.find(".gridSplitter");
};

SoccerReport.prototype.BindingEvent_Summary = function () {
    var self = this;

    self.Tables.btnGenerate.unbind("click");
    self.Tables.btnGenerate.click(function () {
        self.Tables.btnExport.attr("disabled", null);
        self.GetReport({
            Winlostdate: self.Tables.datepickerplugin.nexDatePicker("getSingleDate"),
            SoccerType: self.CacheData.SportId
        });
    });

    self.Tables.btnToday.unbind("click");
    self.Tables.btnToday.click(function () {
        self.Tables.btnExport.attr("disabled", null);
        self.Tables.datepicker.datepicker("setDate", new Date(self.Tables.btnToday.val()));
        self.GetReport({
            Winlostdate: self.Tables.btnToday.val(),
            SoccerType: self.CacheData.SportId
        });
    });

    self.Tables.btnYesterday.unbind("click");
    self.Tables.btnYesterday.click(function () {
        self.Tables.btnExport.attr("disabled", null);
        self.Tables.datepicker.datepicker("setDate", new Date(self.Tables.btnYesterday.val()));
        self.GetReport({
            Winlostdate: self.Tables.btnYesterday.val(),
            SoccerType: self.CacheData.SportId
        });
    });

    self.Tables.btnExport.unbind("click");
    self.Tables.btnExport.click(function () {
        self.SoccerReport_Summary.ExportReport();
    });
};

SoccerReport.prototype.BindingEvent_ByBetTypeReport = function () {
    var self = this;

    self.Tables.btnGenerate.unbind("click");
    self.Tables.btnGenerate.click(function () {
        self.Tables.btnExport.attr("disabled", null);
        self.SoccerReport_ByBetType.GetReport(self.Tables.datepickerplugin.nexDatePicker("getSingleDate"),
            self.SoccerReport_ByBetType.Params.reportParams.Bettype,
            self.SoccerReport_ByBetType.Params.reportParams.BetTypeName,
            self.SoccerReport_ByBetType.Params.reportParams.Liveindicator,
            self.SoccerReport_ByBetType.Params.reportParams.IsHedge
        );
    });

    self.Tables.btnToday.unbind("click");
    self.Tables.btnToday.click(function () {
        self.Tables.btnExport.attr("disabled", null);
        self.Tables.datepicker.datepicker("setDate", new Date(self.Tables.btnToday.val()));
        self.SoccerReport_ByBetType.GetReport(self.Tables.btnToday.val(),
            self.SoccerReport_ByBetType.Params.reportParams.Bettype,
            self.SoccerReport_ByBetType.Params.reportParams.BetTypeName,
            self.SoccerReport_ByBetType.Params.reportParams.Liveindicator,
            self.SoccerReport_ByBetType.Params.reportParams.IsHedge
        );
    });

    self.Tables.btnYesterday.unbind("click");
    self.Tables.btnYesterday.click(function () {
        self.Tables.btnExport.attr("disabled", null);
        self.Tables.datepicker.datepicker("setDate", new Date(self.Tables.btnYesterday.val()));
        self.SoccerReport_ByBetType.GetReport(self.Tables.btnYesterday.val(),
            self.SoccerReport_ByBetType.Params.reportParams.Bettype,
            self.SoccerReport_ByBetType.Params.reportParams.BetTypeName,
            self.SoccerReport_ByBetType.Params.reportParams.Liveindicator,
            self.SoccerReport_ByBetType.Params.reportParams.IsHedge
        );
    });

    self.Tables.btnExport.unbind("click");
    self.Tables.btnExport.click(function () {
        self.SoccerReport_ByBetType.ExportReport();
    });
};

SoccerReport.prototype.GetReport = function (params) {
    this.SoccerReport_Summary.GetReport(params);
};

SoccerReport.prototype.ShowReportView = function () {
    this.Tables.reportView.removeClass("display-none");
};

SoccerReport.prototype.HideReportView = function () {
    AccReport.popUp.hide();
    this.Tables.reportView.addClass("display-none");
    this.SoccerReport_ByBetType.HideReport();
};

SoccerReport.prototype.ShowSummaryReport = function () {
    this.SoccerReport_ByBetType.HideReport();
    this.BindingEvent_Summary();
    this.Tables.datepicker.datepicker("setDate", this.SoccerReport_Summary.Params.reportParams.Winlostdate);
    this.Tables.reportView.removeClass("display-none");
    this.SoccerReport_Summary.ShowReport();
    this.ShowHideSiteTypeInfo();
    this.SetActiveReportManager();
};

SoccerReport.prototype.GetReport_ByBetType = function (winlostdate,
    betTypeId,
    betTypeName,
    isLive,
    isHedge) {
    var self = this;
    self.BindingEvent_ByBetTypeReport();
    reportManager.Tables.btnShowHideSiteTypeInfo.addClass("display-none");
    self.SoccerReport_ByBetType.GetReport(winlostdate,
        betTypeId,
        betTypeName,
        isLive,
        isHedge);
};

SoccerReport.prototype.GetFullBreadCrumbs = function (drillDownLevel, selectParams) {
    var self = this;
    switch (drillDownLevel) {
        case reportManager.Enum.DrillDownLevel.ByBetType:
            return "<a class='breadcrumbs-link' onclick='showHideReportManager.BackToMainReport();'>" + viewResources.Main + "</a> >" +
                   " <a class='breadcrumbs-link' onclick='reportManager.SoccerReport.ShowSummaryReport();'>" + self.CacheData.SportName + "</a> >" +
                   " " + selectParams.BetTypeName +
                        " \"" + (selectParams.IsLive ?
                                 viewResources.Live :
                                 viewResources.Symbol_Sparse_Non_Live) + "\"";
            break;
        default:
            return "";
    }
};

SoccerReport.prototype.GetPlainPreviousBreadCrumbs = function (drillDownLevel, selectParams) {
    var self = this;
    switch (drillDownLevel) {
        case reportManager.Enum.DrillDownLevel.ByBetType:
            return viewResources.Main + " >" +
                   " " + self.CacheData.SportName + " >" +
                   " " + selectParams.BetTypeName +
                        " \"" + (selectParams.IsLive ?
                                 viewResources.Live :
                                 viewResources.Symbol_Sparse_Non_Live) + "\"";
            break;
        default:
            return "";
    }
};

SoccerReport.prototype.SetActiveReportManager = function () {
    if (showHideReportManager.CacheData.currentReport != this) {
        showHideReportManager.CacheData.currentReport.HideReportView();
        showHideReportManager.CacheData.currentReport = this;
    }
};

SoccerReport.prototype.ShowHideSiteTypeInfo = function () {
    var self = this;
    self.SoccerReport_Summary.ShowHideSiteTypeInfo();
};