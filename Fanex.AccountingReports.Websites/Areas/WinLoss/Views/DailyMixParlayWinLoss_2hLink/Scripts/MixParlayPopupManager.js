$(document).ready(function () {
    if (mixParlayPopupManager.CachedData.isFirstLoad) {
        mixParlayPopupManager.InitPage(reportDate, betTypeId, betType);
        mixParlayPopupManager.CachedData.isFirstLoad = false;
    }
});

MixParlayPopupManager = function () {
    var self = this;
    self.AdvanceMixParlayReport = new AdvanceMixParlayReport();
    self.NormalMixParlayReport = new NormalMixParlayReport();

    self.Tables = {
    };

    self.Params = {
        reportParams: {
            Winlostdate: null
        }
    };

    self.CachedData = {
        isFirstLoad: true
    };
};

MixParlayPopupManager.prototype.InitPage = function (reportDate, betTypeId, betType) {
    this.CachedDOMElements();
    this.AdvanceMixParlayReport.InitPage();
    this.NormalMixParlayReport.InitPage();

    var dReportDate = new Date(reportDate);
    var dBeginSupportMPDate = new Date(beginSupportMPDate);
    var dReportDate = new Date(2012, 1, 1);
    if (dReportDate >= dBeginSupportMPDate) {
        var params = {
            FromDate: reportDate,
            ToDate: reportDate,
            IsHistoryReport: false
        };

        this.ResizePopupWidth(this.AdvanceMixParlayReport.CachedData.reportWidth);
        this.AdvanceMixParlayReport.GetReport(params);
    }
    else {
        var params = {
            FromDate: reportDate,
            ToDate: reportDate,
            IsHistoryReport: false
        };

        this.ResizePopupWidth(this.NormalMixParlayReport.CachedData.reportWidth);
        this.NormalMixParlayReport.GetReport(params);
    }
};

MixParlayPopupManager.prototype.CachedDOMElements = function () {
    this.Tables.innerFrame = $("#innerFrame", parent.document);
    this.Tables.uiDialog = $(".ui-dialog", parent.document);
};

MixParlayPopupManager.prototype.ResizePopupWidth = function (width) {
    if (window.parent != null &&
        $(window.parent).width() <= 1024 &&
        width >= advanceMixParlayReport.reportWidth) {
        this.Tables.uiDialog.css("left", "10px");
        this.Tables.innerFrame.css("width", width + 25 + "px");
        this.Tables.uiDialog.css("width", width + 40 + "px");
    } else {
        this.Tables.innerFrame.css("width", width + 25 + "px");
        this.Tables.uiDialog.css("width", width + 40 + "px");
    }

    this.Tables.uiDialog.css("height", 600);
};

MixParlayPopupManager.prototype.FormatNumberCell = function (cellValue,
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

MixParlayPopupManager.prototype.HideNormalMixParlayReport = function () {
    this.NormalMixParlayReport.HideReport();
};

MixParlayPopupManager.prototype.HideAdvanceMixParlayReport = function () {
    this.AdvanceMixParlayReport.HideReport();
};

var mixParlayPopupManager = new MixParlayPopupManager();
