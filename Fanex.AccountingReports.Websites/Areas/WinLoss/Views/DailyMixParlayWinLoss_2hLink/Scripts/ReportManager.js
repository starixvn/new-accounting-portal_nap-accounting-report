$(document).ready(function () {
    reportManager.InitPage();
    if (isredirect == "True") {
        reportManager.Tables.ddlReport.val('1');
        reportManager.HistoryReportManager.Tables.ddlReport_HistoryReport.val('1');
        reportManager.HideCombinationWinLossReport();
        reportManager.WinLossByProduct.BindingEvent();
        reportManager.HistoryReportManager.HideCombinationWinLossReport_HistoryReport();
        reportManager.HistoryReportManager.WinLossByProduct_HistoryReport.BindingEvent();  
        if (new Date(reportDate) <= new Date(movebetday)) {
            var date = new Date(reportDate);
            reportManager.ChangeToHistoryReport();         
            reportManager.HistoryReportManager.Tables.monthPicker.monPickr("setMonth", date);
            reportManager.HistoryReportManager.Tables.monthPicker.monPickr("setFromDay", date.getDate());
            reportManager.HistoryReportManager.Tables.monthPicker.monPickr("setToDay", date.getDate());     
            reportManager.HistoryReportManager.WinLossByProduct_HistoryReport.GetReport();
        }
        else {
            reportManager.Tables.fromDatepicker.datepicker("setDate", new Date(reportDate));
            reportManager.Tables.toDatepicker.datepicker("setDate", new Date(reportDate));
            reportManager.WinLossByProduct.GetReport(reportManager.Enum.DateType.SpecifyDay);
        }
    }
});

ReportManager = function () {
    var self = this;

    self.Enum = {
        ReportType: {
            CombinationWinLoss: 0,
            WinLossByProduct: 1
        },

        DateType: {
            ToDay: 0,
            YesterDay: 1,
            SpecifyDay: 2
        }
    };

    self.CombinationWinLoss = new CombinationWinLoss();
    self.WinLossByProduct = new WinLossByProduct();
    self.HistoryReportManager = new HistoryReportManager();

    self.Tables = {
        btnGenerate: null,
        btnToday: null,
        btnYesterday: null,
        datepickerplugin: null,
        fromDatepicker: null,
        toDatepicker: null,
        filterNote: null,
        ddlReport: null
    };

    self.Params = {
        reportParams: {
            Winlostdate: null
        }
    };
};

ReportManager.prototype.InitPage = function () {
    this.RenderDatePicker();
    this.CachedDOMElements();

    this.Tables.btnToday.val(today);
    this.Tables.btnYesterday.val(yesterday);
    this.RenderFilterNote();
    this.BindingEvent();

    this.CombinationWinLoss.InitPage();
    this.WinLossByProduct.InitPage();
    this.CombinationWinLoss.BindingEvent();
    this.HistoryReportManager.InitPage();
    this.Tables.mixParlayReportFilter_HistoryReport.addClass("display-none");
};

ReportManager.prototype.RenderDatePicker = function () {
    $('#datepickerplugin').nexDatePicker({
        nexDatePickerType: "daterange",
        datePickerFormat: {
            showOtherMonths: true,
            changeMonth: true,
            changeYear: true,
            dateFormat: "M dd, yy",
            minDate: new Date(minDate),
            maxDate: new Date(today)
        },

        firstDatePicker: {
            Id: "firstDatePickerId1",
            Label: labelFrom,
            InitDate: today,
            LabelCss: "lbl-field col_3",
            TextboxCss: "edit-field col_8"
        },

        secondDatePicker: {
            Id: "secondDatePickerId1",
            Label: labelTo,
            InitDate: today,
            LabelCss: "lbl-field col_2",
            TextboxCss: "edit-field col_6"
        },

        onSelectDatePicker: function () {
        }
    });

    this.Tables.datepickerplugin = $("#datepickerplugin");
    this.Tables.fromDatepicker = $(".ui_nexdatepicker_fromdate");
    this.Tables.toDatepicker = $(".ui_nexdatepicker_todate");
};

ReportManager.prototype.CachedDOMElements = function () {
    this.Tables.btnGenerate = $("#btnGenerate");
    this.Tables.btnToday = $("#btnToday");
    this.Tables.btnYesterday = $("#btnYesterday");
    this.Tables.filterNote = $("#notemessage");
    this.Tables.ddlReport = $("#ddlReport");

    this.Tables.btnHistoryReport = $("#btnHistoryReport");
    this.Tables.normalMixParlayReport = $("#normalMixParlayReport");
    this.Tables.historyMixParlayReport = $("#historyMixParlayReport");

    this.Tables.mixParlayReportFilter = $("#mixParlayReportFilter");
    this.Tables.mixParlayReportFilter_HistoryReport = $("#mixParlayReportFilter_HistoryReport");
};

ReportManager.prototype.BindingEvent = function () {
    var self = this;

    self.Tables.ddlReport.change(function () {
        var reportType = parseInt(self.Tables.ddlReport.val());
        switch (reportType) {
            case self.Enum.ReportType.CombinationWinLoss:
                self.HideAdvanceMixParlayReport();
                self.CombinationWinLoss.BindingEvent();
                break;
            case self.Enum.ReportType.WinLossByProduct:
                self.HideCombinationWinLossReport();
                self.WinLossByProduct.BindingEvent();
                break;
        };
    });

    self.Tables.btnHistoryReport.click(function () {
        self.ChangeToHistoryReport();
    });
};

ReportManager.prototype.RenderFilterNote = function () {
    this.Tables.filterNote.attr("title", noteMessage);
    this.Tables.filterNote.tipsy({
        html: true,
        opacity: 0.9,
        gravity: 'ne'
    });
};

ReportManager.prototype.FormatNumberCell = function (cellValue,
    isPositive,
    isNegative,
    isPercent) {
    var cssClass = '';
    var number = cellValue.replace(/\,/g, '');
    var isNumber = $.isNumeric(number);

    if (isNumber &&
            ((isPositive && parseFloat(number) > 0) ||
             (isNegative && parseFloat(number) < 0))) {
        return '<div class="color-red">' + cellValue + (isPercent ? '%' : '') + '</div>';
    }

    return cellValue + (isNumber && isPercent ? '%' : '');
};

ReportManager.prototype.HideCombinationWinLossReport = function () {
    this.CombinationWinLoss.HideReport();
};

ReportManager.prototype.HideAdvanceMixParlayReport = function () {
    this.WinLossByProduct.HideReport();
};

ReportManager.prototype.HideCombinationWinLossReport_HistoryReport = function () {
    this.HistoryReportManager.CombinationWinLoss_HistoryReport.HideReport();
};

ReportManager.prototype.HideAdvanceMixParlayReport_HistoryReport = function () {
    this.HistoryReportManager.WinLossByProduct_HistoryReport.HideReport();
};

ReportManager.prototype.ChangeToHistoryReport = function () {
    this.CombinationWinLoss.HideReport();
    this.WinLossByProduct.HideReport();
    this.Tables.normalMixParlayReport.addClass("display-none");
    this.Tables.mixParlayReportFilter.addClass("display-none");
    this.Tables.mixParlayReportFilter_HistoryReport.removeClass("display-none");
    this.HistoryReportManager.ShowView();
    this.Tables.historyMixParlayReport.removeClass("display-none");
};

ReportManager.prototype.ShowView = function () {
    this.Tables.historyMixParlayReport.addClass("display-none");
    this.Tables.mixParlayReportFilter_HistoryReport.addClass("display-none");
    this.Tables.normalMixParlayReport.removeClass("display-none");
    this.Tables.mixParlayReportFilter.removeClass("display-none");
};

ReportManager.prototype.GetSiteTypeName = function (siteType) {
    siteType = parseInt(siteType);
    switch (siteType) {
        case AccReportData.Constants.SiteType.All_Site:
            return viewResources.All_Sites;
        case AccReportData.Constants.SiteType.Credit:
            return viewResources.Credit_Site;
        case AccReportData.Constants.SiteType.Deposit:
            return viewResources.Deposit_Site;
        default:
            return "";
    }
};

var reportManager = new ReportManager();
