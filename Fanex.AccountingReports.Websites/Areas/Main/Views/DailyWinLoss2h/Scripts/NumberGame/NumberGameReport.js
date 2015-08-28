NumberGameReport = function () {
    var self = this;

    self.NumberGameReport_Summary = new NumberGameReport_Summary();

    self.NumberGameReport_ByBetType = new NumberGameReport_ByBetType();

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

NumberGameReport.prototype.InitPage = function () {
    this.CachedDOMElements();
    this.BindingEvent_Summary();
    this.NumberGameReport_Summary.InitPage(this);
    this.NumberGameReport_ByBetType.InitPage(this);
};

NumberGameReport.prototype.CachedDOMElements = function () {
    this.Tables.btnGenerate = $("#btnGenerate");
    this.Tables.btnToday = $("#btnToday");
    this.Tables.btnYesterday = $("#btnYesterday");
    this.Tables.btnExport = $("#btnExport");
    this.Tables.datepickerplugin = $("#datepickerplugin");
    this.Tables.datepicker = $(".ui_nexdatepicker_fromdate");

    this.Tables.reportView = $("#numberGameReport");
    this.Tables.gridSplitter = this.Tables.reportView.find(".gridSplitter");
};

NumberGameReport.prototype.BindingEvent_Summary = function () {
    var self = this;

    self.Tables.btnGenerate.unbind("click");
    self.Tables.btnGenerate.click(function () {
        self.Tables.btnExport.attr("disabled", null);
        self.GetReport({
            Winlostdate: self.Tables.datepickerplugin.nexDatePicker("getSingleDate")
        });
    });

    self.Tables.btnToday.unbind("click");
    self.Tables.btnToday.click(function () {
        self.Tables.btnExport.attr("disabled", null);
        self.Tables.datepicker.datepicker("setDate", new Date(self.Tables.btnToday.val()));
        self.GetReport({
            Winlostdate: self.Tables.btnToday.val()
        });
    });

    self.Tables.btnYesterday.unbind("click");
    self.Tables.btnYesterday.click(function () {
        self.Tables.btnExport.attr("disabled", null);
        self.Tables.datepicker.datepicker("setDate", new Date(self.Tables.btnYesterday.val()));
        self.GetReport({
            Winlostdate: self.Tables.btnYesterday.val(),
            SportId: self.CacheData.SportId
        });
    });

    self.Tables.btnExport.unbind("click");
    self.Tables.btnExport.click(function () {
        self.NumberGameReport_Summary.ExportReport();
    });
};

NumberGameReport.prototype.BindingEvent_ByBetTypeReport = function () {
    var self = this;

    self.Tables.btnGenerate.unbind("click");
    self.Tables.btnGenerate.click(function () {
        self.Tables.btnExport.attr("disabled", null);
        self.NumberGameReport_ByBetType.GetReport(self.Tables.datepickerplugin.nexDatePicker("getSingleDate"),
            self.NumberGameReport_ByBetType.Params.reportParams.BetType,
            self.NumberGameReport_ByBetType.Params.reportParams.BetTypeName,
            self.NumberGameReport_ByBetType.Params.reportParams.Liveindicator,
            self.NumberGameReport_ByBetType.Params.reportParams.IsHedge
        );
    });

    self.Tables.btnToday.unbind("click");
    self.Tables.btnToday.click(function () {
        self.Tables.btnExport.attr("disabled", null);
        self.Tables.datepicker.datepicker("setDate", new Date(self.Tables.btnToday.val()));
        self.NumberGameReport_ByBetType.GetReport(self.Tables.btnToday.val(),
            self.NumberGameReport_ByBetType.Params.reportParams.BetType,
            self.NumberGameReport_ByBetType.Params.reportParams.BetTypeName,
            self.NumberGameReport_ByBetType.Params.reportParams.Liveindicator,
            self.NumberGameReport_ByBetType.Params.reportParams.IsHedge
        );
    });

    self.Tables.btnYesterday.unbind("click");
    self.Tables.btnYesterday.click(function () {
        self.Tables.btnExport.attr("disabled", null);
        self.Tables.datepicker.datepicker("setDate", new Date(self.Tables.btnYesterday.val()));
        self.NumberGameReport_ByBetType.GetReport(self.Tables.btnYesterday.val(),
            self.NumberGameReport_ByBetType.Params.reportParams.BetType,
            self.NumberGameReport_ByBetType.Params.reportParams.BetTypeName,
            self.NumberGameReport_ByBetType.Params.reportParams.Liveindicator,
            self.NumberGameReport_ByBetType.Params.reportParams.IsHedge
        );
    });

    self.Tables.btnExport.unbind("click");
    self.Tables.btnExport.click(function () {
        self.NumberGameReport_ByBetType.ExportReport();
    });
};

NumberGameReport.prototype.GetReport = function (params) {
    this.NumberGameReport_Summary.GetReport(params);
};

NumberGameReport.prototype.ShowReportView = function () {
    this.Tables.reportView.removeClass("display-none");
};

NumberGameReport.prototype.HideReportView = function () {
    AccReport.popUp.hide();
    this.Tables.reportView.addClass("display-none");
    this.NumberGameReport_ByBetType.HideReport();
};

NumberGameReport.prototype.ShowSummaryReport = function () {
    this.NumberGameReport_ByBetType.HideReport();
    this.BindingEvent_Summary();
    this.Tables.datepicker.datepicker("setDate", this.NumberGameReport_Summary.Params.reportParams.Winlostdate);
    this.Tables.reportView.removeClass("display-none");
    this.NumberGameReport_Summary.ShowReport();
    this.SetActiveReportManager();
    this.NumberGameReport_Summary.ShowHideSiteTypeInfo();
};

NumberGameReport.prototype.GetReport_ByBetType = function (betTypeId,
    betTypeAliasName,
    betTypeName,
    isLive,
    isHedge) {
    var self = this;
    self.BindingEvent_ByBetTypeReport();
    reportManager.Tables.btnShowHideSiteTypeInfo.addClass("display-none");
    self.NumberGameReport_ByBetType.GetReport(self.NumberGameReport_Summary.Params.reportParams.Winlostdate,
        betTypeId,
        betTypeName,
        isLive,
        isHedge);
};

NumberGameReport.prototype.GetFullBreadCrumbs = function (drillDownLevel, selectParams) {
    var self = this;
    switch (drillDownLevel) {
        case reportManager.Enum.DrillDownLevel.ByBetType:
            return "<a class='breadcrumbs-link' onclick='showHideReportManager.BackToMainReport();'>" + viewResources.Main + "</a> >" +
                   " <a class='breadcrumbs-link' onclick='reportManager.NumberGameReport.ShowSummaryReport();'>" + self.CacheData.SportName + "</a> >" +
                   " " + selectParams.BetTypeName +
                        " \"" + (selectParams.IsLive ?
                                 viewResources.Live :
                                 viewResources.Symbol_Sparse_Non_Live) + "\"";
            break;
        default:
            return "";
    }
};

NumberGameReport.prototype.GetPlainPreviousBreadCrumbs = function (drillDownLevel, selectParams) {
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

NumberGameReport.prototype.SetActiveReportManager = function () {
    if (showHideReportManager.CacheData.currentReport != this) {
        showHideReportManager.CacheData.currentReport.HideReportView();
        showHideReportManager.CacheData.currentReport = this;
    }
};

NumberGameReport.prototype.ShowHideSiteTypeInfo = function () {
    var self = this;
    self.NumberGameReport_Summary.ShowHideSiteTypeInfo();
};
