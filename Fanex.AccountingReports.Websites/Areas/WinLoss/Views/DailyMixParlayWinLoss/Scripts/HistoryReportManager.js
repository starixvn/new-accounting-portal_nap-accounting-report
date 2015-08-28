HistoryReportManager = function () {
    var self = this;

    self.CombinationWinLoss_HistoryReport = new CombinationWinLoss_HistoryReport();
    self.WinLossByProduct_HistoryReport = new WinLossByProduct_HistoryReport();

    self.Tables = {
        btnGenerate_HistoryReport: null,
        btnBack_HistoryReport: null,
        monthPicker: null,
        ddlReport_HistoryReport: null
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

HistoryReportManager.prototype.InitPage = function () {
    this.RenderMonthPicker();
    this.CachedDOMElements();
    this.BindingEvent();

    this.CombinationWinLoss_HistoryReport.InitPage();
    this.WinLossByProduct_HistoryReport.InitPage();
    this.CombinationWinLoss_HistoryReport.BindingEvent();
};

HistoryReportManager.prototype.RenderMonthPicker = function () {
    if (this.CachedData.isFirstLoad) {
        $('#ddlMonth').monPickr({
            today: new Date(maxHistoryReportDate),
            maxDate: maxHistoryReportDate,
            minDate: minHistoryReportDate,
            showDateRange: true,
            fromDayId: 'ddlFromDay',
            toDayId: 'ddlToDay',
            sortDesc: true,
            log: function (msg) {
                console.warn(msg);
            }
        });

        this.Tables.monthPicker = $('#ddlMonth');
        this.CachedData.isFirstLoad = false;
    }
    else {
        this.Tables.monthPicker.monPickr("reset");
    }
};

HistoryReportManager.prototype.CachedDOMElements = function () {
    this.Tables.ddlReport_HistoryReport = $("#ddlReport_HistoryReport");
    this.Tables.btnGenerate_HistoryReport = $("#btnGenerate_HistoryReport");
    this.Tables.btnBack_HistoryReport = $("#btnBack_HistoryReport");
};

HistoryReportManager.prototype.BindingEvent = function () {
    var self = this;

    self.Tables.ddlReport_HistoryReport.change(function () {
        var reportType = parseInt(self.Tables.ddlReport_HistoryReport.val());
        switch (reportType) {
            case reportManager.Enum.ReportType.CombinationWinLoss:
                self.HideAdvanceMixParlayReport_HistoryReport();
                self.CombinationWinLoss_HistoryReport.BindingEvent();
                break;
            case reportManager.Enum.ReportType.WinLossByProduct:
                self.HideCombinationWinLossReport_HistoryReport();
                self.WinLossByProduct_HistoryReport.BindingEvent();
                break;
        };
    });

    self.Tables.btnBack_HistoryReport.click(function () {
        self.ChangeToNormalReport();
    });
};

HistoryReportManager.prototype.HideCombinationWinLossReport_HistoryReport = function () {
    this.CombinationWinLoss_HistoryReport.HideReport();
};

HistoryReportManager.prototype.HideAdvanceMixParlayReport_HistoryReport = function () {
    this.WinLossByProduct_HistoryReport.HideReport();
};

HistoryReportManager.prototype.ShowView = function () {
    this.RenderMonthPicker();
};

HistoryReportManager.prototype.ChangeToNormalReport = function () {
    this.CombinationWinLoss_HistoryReport.HideReport();
    this.WinLossByProduct_HistoryReport.HideReport();
    reportManager.ShowView();
};