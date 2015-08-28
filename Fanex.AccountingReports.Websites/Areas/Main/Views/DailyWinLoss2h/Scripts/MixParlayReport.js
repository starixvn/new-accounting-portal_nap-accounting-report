MixParlayReport = function () {
    var self = this;

    self.Tables = {
        btnGenerate: null,
        btnToday: null,
        btnYesterday: null,
        btnExport: null,
        btnBack: null,
        datepickerplugin: null,
        datepicker: null,

        reportView: null,
        reportTitle: null,
        reportContentBody: null
    };
    self.Params = {
        reportParams: {
            Winlostdate: null,
            SportId: null,
            Bettype: 9
        }
    };

    self.CacheData = {
    };
};
MixParlayReport.prototype.InitPage = function () {
    this.CachedDOMElements();
    this.BindingEvent();
};
MixParlayReport.prototype.CachedDOMElements = function () {
    this.Tables.btnGenerate = $("#btnGenerate");
    this.Tables.btnToday = $("#btnToday");
    this.Tables.btnYesterday = $("#btnYesterday");
    this.Tables.btnExport = $("#btnExport");
    this.Tables.datepickerplugin = $("#datepickerplugin");
    this.Tables.datepicker = $(".ui_nexdatepicker_fromdate");

    this.Tables.reportView = $("#mixParlayReport");
    this.Tables.btnBack = this.Tables.reportView.find(".btnBack");
    this.Tables.reportTable = this.Tables.reportView.find("#mixParlayReportContent");
    this.Tables.reportTableBody = this.Tables.reportTable.find("tbody");
};
MixParlayReport.prototype.BindingEvent = function () {
    var self = this;

    self.Tables.btnGenerate.unbind("click");
    self.Tables.btnGenerate.click(function () {
        self.Tables.btnExport.attr("disabled", null);
        self.GetReport({
            Winlostdate: self.Tables.datepickerplugin.nexDatePicker("getSingleDate"),
            SportId: self.CacheData.SportId
        });
    });

    self.Tables.btnToday.unbind("click");
    self.Tables.btnToday.click(function () {
        self.Tables.btnExport.attr("disabled", null);
        self.Tables.datepicker.datepicker("setDate", new Date(self.Tables.btnToday.val()));
        self.GetReport({
            Winlostdate: self.Tables.btnToday.val(),
            SportId: self.CacheData.SportId
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

    self.Tables.btnBack.unbind("click");
    self.Tables.btnBack.click(function () {
        showHideReportManager.BackToMainReport();
    });

    self.Tables.btnExport.unbind("click");
    self.Tables.btnExport.click(function () {
        self.ExportReport();
    });
};
MixParlayReport.prototype.GetReport = function (params) {
    var self = this;

    AccReport.disableInput();
    self.Params.reportParams = params;
    self.Params.reportParams.Bettype = 9;

    var reportDate = new Date(self.Params.reportParams.Winlostdate);
    var changeDate = new Date(beginSupportAdvanceMixParlayDate);
    if (reportDate >= changeDate) {
        showHideReportManager.ShowAdvanceMixParlayReport(self.Params.reportParams.SportId,
                                                             self.Params.reportParams.Winlostdate);
        return;
    }

    AccReport.utility.CallAjax(this,
        window.rootUrl + "Main/DailyWinLoss2h/GetMixParlayReport",
        self.Params.reportParams,
        self.RenderReport,
        self.RenderReport,
        AccReport.enableInput);
};

MixParlayReport.prototype.RenderReport = function (report) {
    if (report && report.ChildsInfo && report.ChildsInfo.length > 0) {
        var arrBody = [];
        var arrFooter = [];
        var sportGroups = report.ChildsInfo;
        for (var i = 0; i < sportGroups.length; i++) {
            var sportGroup = sportGroups[i];
            var items = sportGroup.ChildsInfo;
            for (var j = 0; j < items.length; j++) {
                arrBody.push(this.RenderMainRow(items[j]));
            }

            arrBody.push(this.RenderSportTotalRow(sportGroup.GrandTotal));
            arrBody.push(this.RenderEmptyRow());
        }

        arrBody.push(this.RenderTotalRow(report.GrandTotal));

        var bodyHtml = arrBody.join('');
        bodyHtml = fixBugsBrowser.removeSpaceIE9(bodyHtml);
        this.Tables.reportTableBody.empty();
        this.Tables.reportTable.append(bodyHtml);

        var footerHtml = arrFooter.join('');
        footerHtml = fixBugsBrowser.removeSpaceIE9(footerHtml);
        this.Tables.reportTable.append(footerHtml);
    }
    else {
        this.RenderNoDataReport();
    }

    this.Tables.reportView.removeClass("display-none");
    reportManager.ResizeWindow();
};

MixParlayReport.prototype.RenderMainRow = function (info, cssClass) {
    var html = '<tr>\
                    <td>\
                        <div class="col160 bold">' + info.Name + '(' + info.Status + ')</div>\
                    </td>\
                    <td>\
                        <div class="col100 align-right">' + reportManager.FormatNumberCell(info.WinLoss_Str, true, false) + '</div>\
                    </td>\
                    <td>\
                        <div class="col90 align-right">' + info.Position_Str + '</div>\
                    </td>\
                        <td>\
                        <div class="col80 align-right">' + info.Commission_Str + '</div>\
                    </td>\
                        <td>\
                        <div class="col100 align-right">' + reportManager.FormatNumberCell(info.LineTotal_Str, true, false) + '</div>\
                    </td>\
                        <td>\
                        <div class="col100 align-right">' + info.Turnover_Str + '</div>\
                    </td>\
                        <td>\
                        <div class="col80 align-right">' + reportManager.FormatNumberCell(info.Margin_Str, true, false, true) + '</div>\
                    </td>\
              </tr>';

    return html;
};

MixParlayReport.prototype.RenderEmptyRow = function () {
    var html = '<tr>\
                    <td colspan ="7" class ="align-center"></td>\
                </tr>';
    return html;
};

MixParlayReport.prototype.RenderSportTotalRow = function (info) {
    var html = '<tr class="subTotal" >\
                    <td>\
                        <div class="col160 bold">' + info.Name + '</div>\
                    </td>\
                    <td>\
                        <div class="col100 align-right">' + reportManager.FormatNumberCell(info.WinLoss_Str, true, false) + '</div>\
                    </td>\
                    <td>\
                        <div class="col90 align-right">' + info.Position_Str + '</div>\
                    </td>\
                        <td>\
                        <div class="col80 align-right">' + info.Commission_Str + '</div>\
                    </td>\
                        <td>\
                        <div class="col100 align-right">' + reportManager.FormatNumberCell(info.LineTotal_Str, true, false) + '</div>\
                    </td>\
                        <td>\
                        <div class="col100 align-right">' + info.Turnover_Str + '</div>\
                    </td>\
                        <td>\
                        <div class="col80 align-right">' + reportManager.FormatNumberCell(info.Margin_Str, true, false, true) + '</div>\
                    </td>\
              </tr>';

    return html;
};

MixParlayReport.prototype.RenderTotalRow = function (info, cssClass) {
    var html = '<tr class="" >\
                    <td>\
                        <div class="col160  align-right bold">' + info.Name + '</div>\
                    </td>\
                    <td>\
                        <div class="col100 align-right">' + reportManager.FormatNumberCell(info.WinLoss_Str, true, false) + '</div>\
                    </td>\
                    <td>\
                        <div class="col90 align-right">' + info.Position_Str + '</div>\
                    </td>\
                        <td>\
                        <div class="col80 align-right">' + info.Commission_Str + '</div>\
                    </td>\
                    <td class="colLineTotal_ReportTotal">\
                        <div class="col100 align-right bold">' + reportManager.FormatNumberCell(info.LineTotal_Str, true, false) + '</div>\
                    </td>\
                        <td>\
                        <div class="col100 align-right">' + info.Turnover_Str + '</div>\
                    </td>\
                        <td>\
                        <div class="col80 align-right">' + reportManager.FormatNumberCell(info.Margin_Str, true, false, true) + '</div>\
                    </td>\
              </tr>';

    return html;
};

MixParlayReport.prototype.RenderNoDataReport = function () {
    this.Tables.reportTableBody.empty();
    this.Tables.reportTable.append('<tr><td colspan="7" class="no-data-not-frozen">There is no information available.</td></tr>');
};

MixParlayReport.prototype.HideReportView = function () {
    this.Tables.reportView.addClass("display-none");
};

MixParlayReport.prototype.ExportReport = function () {
    AccReport.downloadFile(window.rootUrl + "Main/DailyWinLoss2h/ExportMixParlayReport", this.Params.reportParams);
};

