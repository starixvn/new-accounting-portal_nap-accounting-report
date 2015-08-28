$(document).ready(function () {
    reportManager.InitPage();
});

ReportManager = function () {
    var self = this;

    self.Main2hReport = new Main2hReport();
    self.OtherSportReport = new OtherSportReport();
    self.WinLossBySite_BetType = new WinLossBySite_BetType();

    self.Enum = {
        DrillDownLevel: {
            BySportType: 1,
            ByBetType: 2
        }
    };

    self.Tables = {
        btnGenerate: null,
        btnToday: null,
        btnYesterday: null,
        btnExport: null,
        datepickerplugin: null,
        datepicker: null,
        header: null,
        datePickerInput: null
    };

    self.Params = {
        reportParams: {
            Winlostdate: null
        }
    };

    self.CachedData = {
        maxGridHeight: 10000,
        btnShowHideSiteTypeInfo: null
    };
};

ReportManager.prototype.InitPage = function () {
    this.RenderDatePicker();
    this.CachedDOMElements();
    this.Main2hReport.InitPage();
    this.Tables.btnToday.val(shortFormattedToday);
    this.Tables.btnYesterday.val(shortFormattedYesterday);
    this.BindingEvent();
    scoreInfoManager.InitPage();
    reportManager.WinLossBySite_BetType.InitPage();
};

ReportManager.prototype.RenderDatePicker = function () {
    $('#datepickerplugin').nexDatePicker({
        nexDatePickerType: "single",
        firstDatePicker: {
            Id: "datePickerInput",
            Label: viewResources.Date,
            InitDate: today,
            LabelCss: "lbl-field col_2",
            TextboxCss: "edit-field col_6 width-auto"
        },

        datePickerFormat: {
            dateFormat: FormatConstant.M_DD_YY_STRING,
            changeMonth: true,
            changeYear: true,
            minDate: new Date(minDate),
            maxDate: new Date(today),
            showOtherMonths: true,
            selectOtherMonths: true
        }
    });

    this.Tables.datepickerplugin = $("#datepickerplugin");
    this.Tables.datepicker = $(".ui_nexdatepicker_fromdate");
    this.Tables.datePickerInput = $("#datePickerInput");
};

ReportManager.prototype.CachedDOMElements = function () {
    this.Tables.btnGenerate = $("#btnGenerate");
    this.Tables.btnToday = $("#btnToday");
    this.Tables.btnYesterday = $("#btnYesterday");
    this.Tables.btnExport = $("#btnExport");
    this.Tables.header = $("header");
    this.Tables.btnShowHideSiteTypeInfo = $("#btnShowHideSiteTypeInfo");
};

ReportManager.prototype.BindingEvent = function () {
    var self = this;

    self.Tables.btnGenerate.unbind("click");
    self.Tables.btnGenerate.click(function () {
        self.Tables.btnExport.attr("disabled", null);
        self.Main2hReport.GetReport(self.Tables.datepickerplugin.nexDatePicker("getSingleDate"));
    });

    self.Tables.btnToday.unbind("click");
    self.Tables.btnToday.click(function () {
        self.Tables.btnExport.attr("disabled", null);
        self.Tables.datepicker.datepicker("setDate", new Date(self.Tables.btnToday.val()));
        self.Main2hReport.GetReport(self.Tables.btnToday.val());
    });

    self.Tables.btnYesterday.unbind("click");
    self.Tables.btnYesterday.click(function () {
        self.Tables.btnExport.attr("disabled", null);
        self.Tables.datepicker.datepicker("setDate", new Date(self.Tables.btnYesterday.val()));
        self.Main2hReport.GetReport(self.Tables.btnYesterday.val());
    });

    self.Tables.btnExport.unbind("click");
    self.Tables.btnExport.click(function () {
        self.Main2hReport.ExportReport();
    });

    self.Tables.btnShowHideSiteTypeInfo.unbind("click");
    self.Tables.btnShowHideSiteTypeInfo.click(function () {
        showHideSiteTypeManager.ShowHideSiteTypeInfo();
    });
};

ReportManager.prototype.RebindingEvent = function () {
    var self = this;
    self.Tables.btnGenerate.unbind("click");
    self.Tables.btnToday.unbind("click");
    self.Tables.btnYesterday.unbind("click");
    self.Tables.btnExport.unbind("click");
    self.BindingEvent();
};

ReportManager.prototype.GetIBCTotalLabel = function () {
    var ibcMsg = '';
    if (is8RMActive == 'true' && isBARMActive == 'true') {
        ibcMsg = viewResources.IBC + "(" + viewResources._8RM + " + " + viewResources.BARM + ") " + viewResources.Total;
    }
    else if (is8RMActive == 'true') {
        ibcMsg = viewResources.IBC + "(" + viewResources._8RM + ") " + viewResources.Total;
    }
    else if (isBARMActive == 'true') {
        ibcMsg = viewResources.IBC + "(" + viewResources.BARM + ") " + viewResources.Total;
    }

    return ibcMsg;
};

ReportManager.prototype.GetWinlossBySiteLink = function (selectParams) {
    var evt = "showHideReportManager.ShowWinlossBySite('" + selectParams.WinLossDate + "','" +
        selectParams.SportType + "','" +
        selectParams.SportGroupId + "','" +
        selectParams.BetType + "','" +
        selectParams.BetTypeName + "'," +
        selectParams.IsLive + "," +
        selectParams.IsHedge + "," +
        selectParams.SiteType + ",'" +
        selectParams.SportTypeName + "'); return;";

    var reportLink = '<b>\
                          <a class="report2h-link" onclick="' + evt + '">' + selectParams.SiteTypeName + '</a>\
                      </b>';
    return reportLink;
};

ReportManager.prototype.DisableGenerateButton = function () {
    var self = this;
    self.Tables.btnGenerate.attr("disabled", "disabled");
    self.Tables.btnToday.attr("disabled", "disabled");
    self.Tables.btnYesterday.attr("disabled", "disabled");
    self.Tables.btnExport.attr("disabled", "disabled");
    self.Tables.datePickerInput.attr("disabled", "disabled");
};

ReportManager.prototype.EnableGenerateButton = function () {
    var self = this;
    self.Tables.btnGenerate.attr("disabled", null);
    self.Tables.btnToday.attr("disabled", null);
    self.Tables.btnYesterday.attr("disabled", null);
    self.Tables.btnExport.attr("disabled", null);
    self.Tables.datePickerInput.attr("disabled", null);
};

ReportManager.prototype.GetColumnTurnover = function (margin,
    turnover,
    columnName) {
    return {
        Name: columnName,
        Value: turnover != 0 ? margin : viewResources.NotAvailable
    };
};

ReportManager.prototype.BindingEvent_WinLossBySite_BetType = function () {
    var self = this;

    self.Tables.btnGenerate.unbind("click");
    self.Tables.btnGenerate.click(function () {
        self.Tables.btnExport.attr("disabled", null);
        self.WinLossBySite_BetType.GetReport({
            WinLossDate: self.Tables.datepickerplugin.nexDatePicker("getSingleDate"),
            SportType: self.WinLossBySite_BetType.Params.reportParams.SportType,
            BetType: self.WinLossBySite_BetType.Params.reportParams.bettype,
            BetTypeName: self.WinLossBySite_BetType.Params.reportParams.BetTypeName,
            IsLive: self.WinLossBySite_BetType.Params.reportParams.liveindicator,
            IsHedge: self.WinLossBySite_BetType.Params.reportParams.IsHedge,
            SiteType: self.WinLossBySite_BetType.Params.reportParams.site,
            SportGroupId: self.WinLossBySite_BetType.Params.reportParams.SportGroupId
        });
    });

    self.Tables.btnToday.unbind("click");
    self.Tables.btnToday.click(function () {
        self.Tables.btnExport.attr("disabled", null);
        self.Tables.datepicker.datepicker("setDate", new Date(self.Tables.btnToday.val()));
        self.WinLossBySite_BetType.GetReport({
            WinLossDate: self.Tables.btnToday.val(),
            SportType: self.WinLossBySite_BetType.Params.reportParams.SportType,
            BetType: self.WinLossBySite_BetType.Params.reportParams.bettype,
            BetTypeName: self.WinLossBySite_BetType.Params.reportParams.BetTypeName,
            IsLive: self.WinLossBySite_BetType.Params.reportParams.liveindicator,
            IsHedge: self.WinLossBySite_BetType.Params.reportParams.IsHedge,
            SiteType: self.WinLossBySite_BetType.Params.reportParams.site,
            SportGroupId: self.WinLossBySite_BetType.Params.reportParams.SportGroupId
        });
    });

    self.Tables.btnYesterday.unbind("click");
    self.Tables.btnYesterday.click(function () {
        self.Tables.btnExport.attr("disabled", null);
        self.Tables.datepicker.datepicker("setDate", new Date(self.Tables.btnYesterday.val()));
        self.WinLossBySite_BetType.GetReport({
            WinLossDate: self.Tables.btnYesterday.val(),
            SportType: self.WinLossBySite_BetType.Params.reportParams.SportType,
            BetType: self.WinLossBySite_BetType.Params.reportParams.bettype,
            BetTypeName: self.WinLossBySite_BetType.Params.reportParams.BetTypeName,
            IsLive: self.WinLossBySite_BetType.Params.reportParams.liveindicator,
            IsHedge: self.WinLossBySite_BetType.Params.reportParams.IsHedge,
            SiteType: self.WinLossBySite_BetType.Params.reportParams.site,
            SportGroupId: self.WinLossBySite_BetType.Params.reportParams.SportGroupId
        });
    });

    self.Tables.btnExport.unbind("click");
    self.Tables.btnExport.click(function () {
        self.WinLossBySite_BetType.ExportReport();
    });
};

var reportManager = new ReportManager();