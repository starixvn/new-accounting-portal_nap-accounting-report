$(document).ready(function () {

    var reportManager = new Fanex.AccountingReports.Main.MatchWinLoss2a.ReportManager();
    reportManager.initPage();
});

Namespace.Register("Fanex.AccountingReports.Main.MatchWinLoss2a.ReportManager");
Fanex.AccountingReports.Main.MatchWinLoss2a.ReportManager = function () {
    var self = this;

    var _summaryReportByBetType = new Fanex.AccountingReports.Main.MatchWinLoss2a.SummaryReportByBetType();
    var _summaryReportBySportType = new Fanex.AccountingReports.Main.MatchWinLoss2a.SummaryReportBySportType();
    var _detailReportByBetType = new Fanex.AccountingReports.Main.MatchWinLoss2a.DetailReportByBetType();
    var _detailReportBySportsType = new Fanex.AccountingReports.Main.MatchWinLoss2a.DetailReportBySportsType();
    var _thirdPartyDetailReport = new Fanex.AccountingReports.Main.MatchWinLoss2a.ThirdPartyDetailReport();
    var cockFightingReport = new Fanex.AccountingReports.Main.MatchWinLoss2a.CockFightingReport();
    var _tables = {
        btnGenerate: null,
        btnToday: null,
        btnYesterday: null,
        btnExport: null,
        datepickerplugin: null,
        firstDatePickerId1: null,
        chkMainReport: null,
        chkDetailReport: null
    };

    var _params = {
        reportParams: {
            Winlostdate: null,
            IsGetMainReport: null,
            IsGetDetailReport: null
        }
    };

    var dateOption = {
        Today: 0,
        Yesterday: 1
    };

    var _cachedData = {
        CompletingReportCount: 0
    };

    self.initPage = function () {
        renderDatePicker();
        cachedDOMElements();
        _summaryReportByBetType.initPage();
        _summaryReportBySportType.initPage();
        _detailReportByBetType.initPage();
        _detailReportBySportsType.initPage();
        _thirdPartyDetailReport.initPage();
        cockFightingReport.initPage();
        _tables.btnToday.val(moment(shortFormattedToday).format("MMM DD, YYYY"));
        _tables.btnYesterday.val(moment(shortFormattedYesterday).format("MMM DD, YYYY"));
        bindingEvent();
    };

    var renderDatePicker = function () {
        $('#datepickerplugin').nexDatePicker({
            nexDatePickerType: "single",
            firstDatePicker: {
                Id: "CheckDate",
                Label: labelDate,
                InitDate: today,
                LabelCss: "lbl-field col_2",
                TextboxCss: "edit-field col_5"
            },

            datePickerFormat: {
                dateFormat: FormatConstant.M_DD_YY_STRING,
                showOtherMonths: true,
                selectOtherMonths: true,
                changeMonth: true,
                changeYear: true,
                minDate: new Date(minDate),
                maxDate: new Date(today)
            }
        });

        _tables.datepickerplugin = $("#datepickerplugin");
        _tables.firstDatePickerId1 = $("#CheckDate");
    };

    var cachedDOMElements = function () {
        _tables.btnGenerate = $("#btnGenerate");
        _tables.btnToday = $("#btnToday");
        _tables.btnYesterday = $("#btnYesterday");
        _tables.btnExport = $("#btnExport");
        _tables.chkMainReport = $("#chkMainReport");
        _tables.chkDetailReport = $("#chkDetailReport");
    };

    var bindingEvent = function () {
        _tables.btnGenerate.click(function () {
            _tables.btnExport.attr("disabled", null);
            var reportParams = {
                reportDate: _tables.datepickerplugin.nexDatePicker("getSingleDate"),
                isGetMainReport: _tables.chkMainReport.is(":checked"),
                isGetDetailReport: _tables.chkDetailReport.is(":checked")
            };
            self.GetReport(reportParams);
        });

        _tables.btnToday.click(function () {
            _tables.btnExport.attr("disabled", null);
            var reportParams = {
                reportDate: _tables.btnToday.val(),
                isGetMainReport: _tables.chkMainReport.is(":checked"),
                isGetDetailReport: _tables.chkDetailReport.is(":checked")
            };
            self.SetDatePickerValue(dateOption.Today);
            self.GetReport(reportParams);
        });

        _tables.btnYesterday.click(function () {
            _tables.btnExport.attr("disabled", null);
            var reportParams = {
                reportDate: _tables.btnYesterday.val(),
                isGetMainReport: _tables.chkMainReport.is(":checked"),
                isGetDetailReport: _tables.chkDetailReport.is(":checked")
            };
            self.SetDatePickerValue(dateOption.Yesterday);
            self.GetReport(reportParams);
        });

        _tables.btnExport.click(function () {
            exportAllDetailReport();
        });
    };

    self.GetReport = function (reportParams) {
        if (new Date(reportParams.reportDate) < new Date(balanceUpDay)) {
            self.GetReportBeforeBalanceUpDay(reportParams);
        } else {
            self.GetReportAfterBalanceUpDay(reportParams);
        }
    };

    self.GetReportBeforeBalanceUpDay = function (reportParams) {
        if (self.checkParams()) {
            self.getSummaryReportByBetType(reportParams);
            self.getSummaryReportBySportType(reportParams);
        }
    };

    self.getSummaryReportByBetType = function (reportParams) {
        if (_params.reportParams.IsGetMainReport) {
            _summaryReportByBetType.getSummaryReportByBetType(reportParams);
        }
        else {
            $('#summaryReportByBetTypeContainer').addClass("display-none");
            $('#summaryReportByBetType').addClass("display-none");
        }
    };

    self.getSummaryReportBySportType = function (reportParams) {
        if (_params.reportParams.IsGetDetailReport) {
            _summaryReportBySportType.getSummaryReportBySportType(reportParams);
        }
        else {
            $('#summaryReportBySportTypeContainer').addClass("display-none");
            $('#summaryReportBySportType').addClass("display-none");
        }
    };

    self.renderSummaryReportByBetType = function (rs, reportParams) {
        if (_params.reportParams.IsGetMainReport) {
            _summaryReportByBetType.renderReport(rs.SummaryReportByBetType, reportParams);
        }
        else {
            $('#summaryReportByBetTypeContainer').addClass("display-none");
            $('#summaryReportByBetType').addClass("display-none");
        }
    };

    self.renderSummaryReportBySportsType = function (rs, reportParams) {
        if (_params.reportParams.IsGetDetailReport) {
            _summaryReportBySportType.renderReport(rs.SummaryReportBySportType, reportParams);
        }
        else {
            $('#summaryReportBySportTypeContainer').addClass("display-none");
            $('#summaryReportBySportType').addClass("display-none");
        }
    };

    self.GetReportAfterBalanceUpDay = function (reportParams) {
        self.getParams(reportParams);
        if (self.checkParams()) {
            AccReport.disableInput();
            AccReport.utility.CallAjax(this,
                window.rootUrl + "Main/MatchWinLoss2a/GetMainReport",
                _params.reportParams,
                function (rs) {
                    self.renderSummaryReportByBetType(rs, _params.reportParams);
                    self.renderSummaryReportBySportsType(rs, _params.reportParams);
                },
                function (jqXHR, event, ajaxSettings) {
                },
                AccReport.enableInput);
        }
    };

    self.getParams = function (reportParams) {
        _params.reportParams.Winlostdate = reportParams.reportDate;
    };

    self.checkParams = function () {
        if (_tables.chkMainReport.is(":checked") &&
        _tables.chkDetailReport.is(":checked")) {
            _params.reportParams.IsGetMainReport = true;
            _params.reportParams.IsGetDetailReport = true;
            return true;
        }
        else if (_tables.chkMainReport.is(":checked") &&
        !_tables.chkDetailReport.is(":checked")) {
            _params.reportParams.IsGetMainReport = true;
            _params.reportParams.IsGetDetailReport = false;
            return true;
        }
        else if (!_tables.chkMainReport.is(":checked") &&
        _tables.chkDetailReport.is(":checked")) {
            _params.reportParams.IsGetMainReport = false;
            _params.reportParams.IsGetDetailReport = true;
            return true;
        }
        else {
            _params.reportParams.IsGetMainReport = false;
            _params.reportParams.IsGetDetailReport = false;
            AccReport.popUp.show("Please select at least one Report option!", '');
            return false;
        }
    };

    self.SetDatePickerValue = function (dateOptionId) {
        var dateOptionId = parseInt(dateOptionId);

        switch (dateOptionId) {
            case dateOption.Today:
                _tables.firstDatePickerId1.datepicker("option", "maxDate", new Date(shortFormattedToday));
                _tables.firstDatePickerId1.datepicker("setDate", new Date(shortFormattedToday));
                break;
            case dateOption.Yesterday:
                _tables.firstDatePickerId1.datepicker("option", "maxDate", new Date(shortFormattedYesterday));
                _tables.firstDatePickerId1.datepicker("setDate", new Date(shortFormattedYesterday));
                break;
            default:
                break;
        }
    };

    IsShowMainAndDetailReport = function (isShowMainReport, isShowDetailReport) {
        if (isShowMainReport && isShowDetailReport) {
            return true;
        }
        else {
            return false;
        }
    };

    ViewMixparlayDetail = function (reportDate, betTypeId, betType) {
        ClosePopup();
        var tempParams = {
            reportDate: reportDate,
            BetTypeId: betTypeId,
            BetType: betType,
            IsRedirect: false
        };

        var mixParlayReportUrl = '';
        if (tempParams.reportDate < new Date(beginSupportMPDate)) {
            mixParlayReportUrl = '../MixParlay/MixParlayPopUp?' + $.param(tempParams);
            OpenPopup(mixParlayReportUrl, "", 901, 600, false, false, window, 3000, null, null);
        } else {
            tempParams.IsRedirect = true;
            mixParlayReportUrl = window.rootUrl + 'WinLoss/DailyMixParlayWinLoss/Index?' + $.param(tempParams);
            window.location = mixParlayReportUrl;
        }
    };

    DisableInput = function () {
        AccReport.disableInput();
    };
    EnableInput = function () {
        _cachedData.CompletingReportCount++;

        if (_cachedData.CompletingReportCount >= 2) {
            AccReport.enableInput();
            _cachedData.CompletingReportCount = 0;
        }
    };

    ViewDetailReportByBetType = function (reportDate,
        betTypeId,
        betTypeName,
        sportTypeId,
        sportTypeName,
        liveIndicatorId) {
       // _thirdPartyDetailReport.closeDialog();
        _detailReportBySportsType.closeDialog();
        if (new Date(reportDate) >= new Date(balanceUpDay)) {
            _detailReportByBetType.getReport({
                Winlostdate: reportDate,
                Sporttype: sportTypeName,
                Bettype: betTypeId,
                BetTypeName: betTypeName,
                LiveIndicator: liveIndicatorId
            });
        }
        else {
            _detailReportByBetType.getReportBeforeBalanceUpDay({
                Winlostdate: reportDate,
                Sporttype: sportTypeName,
                Bettype: betTypeId,
                BetTypeName: betTypeName,
                LiveIndicator: liveIndicatorId
            });
        }
    };

    ViewDetailReportBySportType = function (reportDate,
        sportTypeId,
        sportTypeName) {
        _thirdPartyDetailReport.closeDialog();
        _detailReportByBetType.closeDialog();
        if (new Date(reportDate) >= new Date(balanceUpDay)) {
            _detailReportBySportsType.getReport({
                Winlostdate: reportDate,
                Sporttype: sportTypeName,
                Bettype: 0,
                BetTypeName: '',
                LiveIndicator: -1
            });
        }
        else {
            _detailReportBySportsType.getReportBeforeBalanceUpDay({
                Winlostdate: reportDate,
                Sporttype: sportTypeName,
                Bettype: 0,
                BetTypeName: '',
                LiveIndicator: -1
            });
        }
    };

    ViewThirdPartyDetailReport = function (reportDate, sportTypeName) {
        _detailReportByBetType.closeDialog();
        _detailReportBySportsType.closeDialog();
        _thirdPartyDetailReport.closeDialog();
        _thirdPartyDetailReport.getReport({
            Winlostdate: reportDate,
            SportName: sportTypeName
        });
    };

    ViewMetricGamingReport = function (reportDate) {
        _detailReportByBetType.closeDialog();
        _detailReportBySportsType.closeDialog();
        _thirdPartyDetailReport.closeDialog();
        cockFightingReport.closeDialog();
        cockFightingReport.closeCockFightingByArenaDialog();
        _thirdPartyDetailReport.getMetricGamingReport({
            Winlostdate: reportDate
        });
    };

    exportAllDetailReport = function () {
        var oldParam = _summaryReportByBetType.getParams();
        _params.reportParams.Winlostdate = oldParam.Winlostdate;
        AccReport.downloadFile(window.rootUrl + "Main/MatchWinLoss2a/ExportAllDetailReport", _params.reportParams);
    };

    ViewCockFightingReport = function (reportDate) {
        _detailReportByBetType.closeDialog();
        _detailReportBySportsType.closeDialog();
        _thirdPartyDetailReport.closeDialog();
        cockFightingReport.closeDialog();
        cockFightingReport.closeCockFightingByArenaDialog();
        cockFightingReport.getCockFightingReport({
            Winlostdate: reportDate
        });
    };

    ViewCockFightingDetailReport = function (reportDate, arenaId, arenaName) {
        _detailReportByBetType.closeDialog();
        _detailReportBySportsType.closeDialog();
        _thirdPartyDetailReport.closeDialog();
        cockFightingReport.closeDialog();
        cockFightingReport.closeCockFightingByArenaDialog();
        cockFightingReport.getCockFightingMatchByArenaReport({
            Winlostdate: reportDate,
            ArenaId: arenaId,
            ArenaName: arenaName
        });
    };
}