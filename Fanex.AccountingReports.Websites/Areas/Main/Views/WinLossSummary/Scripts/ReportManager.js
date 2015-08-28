$(document).ready(function () {
    reportManager.InitPage();
});

ReportManager = function () {
    var self = this;

    self.Enum = {
        LiveType: {
            Live: 1
        }
    };

    self.Tables = {
        btnGenerate: null,
        datepickerplugin: null,

        parentContainer: null,
        reportTable: null,
        reportTable_Title: null,
        reportTable_Body: null,
        leftReport: null,
        gridSplitter: null,
        calculateToday: null,
        calculateYesterday: null
    };
    self.Params = {
        reportParams: {
            Fromdate: null,
            Todate: null,
            _details: null
        }
    };

    self.CacheData = {
    };
};
ReportManager.prototype.InitPage = function () {
    this.RenderDatePicker();
    this.CachedDOMElements();
    this.RenderFilterNote();
    this.BindingEvent();

    this.Tables.calculateToday.html("Today (" + today + ")");
    this.Tables.calculateYesterday.html("Yesterday (" + yesterday + ")");
};
ReportManager.prototype.CachedDOMElements = function () {
    this.Tables.btnGenerate = $("#btnGenerate");
    this.Tables.filterNote = $("#notemessage");

    this.Tables.parentContainer = $("#parentContainer");
    this.Tables.reportTable = this.Tables.parentContainer.find("#summaryReport_Container");
    this.Tables.reportTable_Title = this.Tables.reportTable.find("#summaryReport_Title");
    this.Tables.reportTable_Body = this.Tables.reportTable.find("#summaryReport tbody");
    this.Tables.leftReport = this.Tables.parentContainer.find("#leftReport");

    this.Tables.gridSplitter = this.Tables.parentContainer.find(".gridSplitter");
    this.Tables.calculateToday = this.Tables.parentContainer.find("#calculateToday");
    this.Tables.calculateYesterday = this.Tables.parentContainer.find("#calculateYesterday");
};
ReportManager.prototype.RenderDatePicker = function () {
    $('#datepickerplugin').nexDatePicker({
        nexDatePickerType: "daterange",
        datePickerFormat: {
            changeMonth: true,
            changeYear: true,
            dateFormat: FormatConstant.M_DD_YY_STRING,
            minDate: new Date(minDate),
            maxDate: new Date(today)
        },
        firstDatePicker: {
            Id: "firstDatePickerId1",
            Label: labelFrom,
            InitDate: today,
            LabelCss: "lbl-field col_2",
            TextboxCss: "edit-field col_6"
        },
        secondDatePicker: {
            Id: "secondDatePickerId1",
            Label: labelTo,
            InitDate: today,
            LabelCss: "lbl-field col_1",
            TextboxCss: "edit-field col_6"
        },
        onSelectDatePicker: function () {
        }
    });

    this.Tables.datepickerplugin = $("#datepickerplugin");
};
ReportManager.prototype.RenderFilterNote = function () {
    this.Tables.filterNote.attr("title", noteMessage);
    this.Tables.filterNote.tipsy({
        html: true,
        opacity: 0.9,
        gravity: 'ne'
    });
};
ReportManager.prototype.BindingEvent = function () {
    var self = this;

    self.Tables.btnGenerate.unbind("click");
    self.Tables.btnGenerate.click(function () {
        self.GetReport();
    });
};
ReportManager.prototype.GetReport = function () {
    var self = this;

    AccReport.disableInput();
    self.GetParams();
    $.ajax({
        type: "POST",
        url: window.rootUrl + "Main/WinLossSummary/GetReport",
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        data: JSON.stringify(self.Params.reportParams),
        beforeSend: function () {
        },
        success: function (rs) {
            self.RenderReport(rs);
        },
        error: function (event, jqXHR, ajaxSettings) {
            self.RenderReport(null);
        },
        complete: function () {
            AccReport.enableInput();
        }
    });
};
ReportManager.prototype.GetParams = function () {
    var dateRange = this.Tables.datepickerplugin.nexDatePicker("getDateRange");
    this.Params.reportParams.Fromdate = dateRange.FromDate;
    this.Params.reportParams.Todate = dateRange.ToDate;
};
ReportManager.prototype.RenderReport = function (report) {
    var self = this;

    this.Tables.reportTable.addClass("display-none");
    var arrBody = [];
    var arrFooter = [];
    var title = 'Summary report from ' + self.Params.reportParams.Fromdate +
                ' to ' + self.Params.reportParams.Todate;
    this.Tables.reportTable_Title.html(title);
    if (report && report.ChildsInfo.length > 0) {
        var sportLength = report.ChildsInfo.length;
        for (var i = 0; i < sportLength; i++) {
            var sportInfo = report.ChildsInfo[i];
            self.RenderSportTable(sportInfo);
        }
    }
    else {
        arrBody.push('<tr><td colspan="6" class="no-data-not-frozen">There is no information available.</td></tr>');
    }

    var bodyHtml = arrBody.join('');
    bodyHtml = fixBugsBrowser.removeSpaceIE9(bodyHtml);
    this.Tables.reportTable_Body.empty();
    this.Tables.reportTable_Body.append(bodyHtml);

    var footerHtml = arrFooter.join('');
    footerHtml = fixBugsBrowser.removeSpaceIE9(footerHtml);
    this.Tables.reportTable_Body.append(footerHtml);

    this.Tables.reportTable_Body.removeClass("display-none");
    this.Tables.reportTable.removeClass("display-none");

    this.Tables.gridSplitter.removeClass("display-none");
};

ReportManager.prototype.RenderSportTable = function (sportInfo) {
    var self = this;
    var arrBody = [];
    var arrFooter = [];

    var reportContainer = self.CreateSportTable(sportInfo);
    var reportContent = reportContainer.find(".sportReport tbody");

    var betTypeLength = sportInfo.ChildsInfo.length;
    for (var j = 0; j < betTypeLength; j++) {
        var betTypeInfo = sportInfo.ChildsInfo[j];
        arrBody.push(self.RenderBetTypeRow(betTypeInfo));
    }

    var bodyHtml = arrBody.join('');
    bodyHtml = fixBugsBrowser.removeSpaceIE9(bodyHtml);
    reportContent.empty().append(bodyHtml);
    this.Tables.leftReport.append(reportContainer);
};
ReportManager.prototype.CreateSportTable = function (sportInfo) {
    var title = '<div class="grid-title">\
                    <div class="grid-title-label">' + sportInfo.Name + '</div>\
                </div>';

    var content = '<table id="sportReport_' + sportInfo.Id + '" class="reportTable sportReport">\
                        <thead>\
                            <tr>\
                                <th>\
                                    <div class="col20"></div>\
                                </th>\
                                <th>\
                                    <div class="col140">Bet Type</div>\
                                </th>\
                                <th>\
                                    <div class="col120">Win Loss</div>\
                                </th>\
                                <th>\
                                    <div class="col100">Position</div>\
                                </th>\
                                <th>\
                                    <div class="col100">Comm</div>\
                                </th>\
                                <th>\
                                    <div class="col120">Line Total</div>\
                                </th>\
                            </tr>\
                        </thead>\
                        <tbody></tbody>\
                  </table>';

    var container = $('<div id="sportReport_Container' + sportInfo.Id + '" class="sportReport_Container"></div>');
    container.append($(title));
    container.append($(content));

    return container;
};
ReportManager.prototype.RenderBetTypeRow = function (betTypeInfo) {
    var html = '<tr>\
                    <td>\
                        <div class="col20"></div>\
                    </td>\
                    <td>\
                        <div class="col140 bold">' +
                            (betTypeInfo.LiveType == reportManager.Enum.LiveType.Live ?
                               "<font color='red'>Live!</font> " : "") +
                                betTypeInfo.Name + '</div>\
                    </td>\
                    <td>\
                        <div class="col120 align-right">' + betTypeInfo.WinLoss_Str + '</div>\
                    </td>\
                    <td>\
                        <div class="col100 align-right">' + betTypeInfo.Position_Str + '</div>\
                    </td>\
                    <td>\
                        <div class="col100 align-right">' + betTypeInfo.Commission_Str + '</div>\
                    </td>\
                    <td>\
                        <div class="col120 align-right">' + betTypeInfo.LineTotal_Str + '</div>\
                    </td>\
                </tr>';
    return html;

};
var reportManager = new ReportManager();