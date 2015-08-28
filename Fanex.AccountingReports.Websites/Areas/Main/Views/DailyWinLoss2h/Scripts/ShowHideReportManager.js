ShowHideReportManager = function () {
    var self = this;

    self.ReportManager = reportManager;
    self.CacheData = {
        currentReport: reportManager.Main2hReport
    };

    self.ShowCasinoReport = function (sportId, sportName, winLossDate) {
        self.CacheData.currentReport = reportManager.CasinoReport;
        self.ShowDrilldownReport(sportId, sportName, winLossDate);
    };

    self.ShowLiveCasinoReport = function (sportId, sportName, winLossDate) {
        self.CacheData.currentReport = reportManager.LiveCasinoReport;
        self.ShowDrilldownReport(sportId, sportName, winLossDate);
    };

    self.ShowKenoReport = function (sportId, sportName, winLossDate) {
        self.CacheData.currentReport = reportManager.KenoReport;
        self.ShowDrilldownReport(sportId, sportName, winLossDate);
    };

    self.ShowBingoReport = function (sportId, sportName, winLossDate) {
        self.CacheData.currentReport = reportManager.BingoReport;
        self.ShowDrilldownReport(sportId, sportName, winLossDate);
    };

    self.ShowAdvanceMixParlayReport = function (sportId, sportName, winLossDate) {
        var param = {
            reportDate: winLossDate,
            SportId: sportId,
            IsRedirect: true,
            BetType: 9
        };

        mixParlayReportUrl = window.rootUrl + 'WinLoss/DailyMixParlayWinLoss_2hLink/Index?' + $.param(param);
        window.location = mixParlayReportUrl;
    };

    self.ShowMixParlayReport = function (sportId, sportName, winLossDate) {
        self.CacheData.currentReport = reportManager.MixParlayReport;
        self.ShowDrilldownReport(sportId, sportName, winLossDate);
    };

    self.ShowSystemParlayReport = function (sportId, sportName, winLossDate) {
        self.CacheData.currentReport = reportManager.SystemParlayReport;
        self.ShowDrilldownReport(sportId, sportName, winLossDate);
        reportManager.ParlayReport.tables.reportContainer.addClass("display-none");
        reportManager.Tables.btnShowHideSiteTypeInfo.addClass("display-none");
    };

    self.ShowRacingReport = function (sportId, sportName, winLossDate) {
        self.CacheData.currentReport = reportManager.RacingReport;
        self.ShowDrilldownReport(sportId, sportName, winLossDate);
    };

    self.ShowNumberGameReport = function (sportId, sportName, winLossDate) {
        self.CacheData.currentReport = reportManager.NumberGameReport;
        self.ShowDrilldownReport(sportId, sportName, winLossDate);
    };

    self.ShowOtherSportReport = function (sportId, sportName, winLossDate) {
        self.CacheData.currentReport = reportManager.OtherSportReport;
        self.ShowDrilldownReport(sportId, sportName, winLossDate);
    };

    self.ShowSoccerReport = function (sportId, sportName, winLossDate) {
        self.CacheData.currentReport = reportManager.SoccerReport;
        self.ShowDrilldownReport(sportId, sportName, winLossDate);
    };

    self.ShowBingoReport = function (sportId, sportName, winLossDate) {
        self.CacheData.currentReport = reportManager.BingoReport;
        self.ShowDrilldownReport(sportId, sportName, winLossDate);
    };

    self.ShowVirtualSportReport = function (sportId, sportName, winLossDate) {
        self.CacheData.currentReport = reportManager.VirtualSportReport;
        self.ShowDrilldownReport(sportId, sportName, winLossDate);
    };

    self.ShowDrilldownReport = function (sportId, sportName, winLossDate) {
        reportManager.Main2hReport.Tables.reportContainer.addClass("display-none");
        self.CacheData.currentReport.InitPage();
        self.CacheData.currentReport.CacheData.SportId = sportId;
        self.CacheData.currentReport.CacheData.SportName = sportName;

        if (sportId == AccReportData.Constants.SportType.SoccerMajorBetType ||
            sportId == AccReportData.Constants.SportType.SoccerMinorBetType) {
            self.CacheData.currentReport.GetReport({
                Winlostdate: winLossDate,
                SoccerType: sportId
            });
        } else {
            self.CacheData.currentReport.GetReport({
                Winlostdate: winLossDate
            });
        }
    };

    self.BackToMainReport = function () {
        AccReport.popUp.hide();
        scoreInfoManager.ClosePopup();
        self.CacheData.currentReport.HideReportView();
        reportManager.Tables.datepicker.datepicker("setDate", reportManager.Main2hReport.Params.reportParams.Winlostdate);
        reportManager.RebindingEvent();
        reportManager.Main2hReport.Tables.reportContainer.removeClass("display-none");
        showHideReportManager.CacheData.currentReport = reportManager.Main2hReport;
        self.CacheData.currentReport.Tables.report.fanexGrid("resizeGrid");
    };

    self.ShowWinlossBySite = function (winLossDate,
        sportType,
        sportGroupId,
        betType,
        betTypeName,
        isLive,
        isHedge,
        siteType,
        sportTypeName,
        productId) {
        self.CacheData.currentReport.HideReportView();
        var selectParams = {
            WinLossDate: winLossDate,
            SportType: sportType,
            SportGroupId: sportGroupId,
            BetType: betType,
            BetTypeName: betTypeName,
            IsLive: isLive,
            IsHedge: isHedge,
            SiteType: siteType,
            SportTypeName: sportTypeName,
            ProductId: productId
        };

        reportManager.WinLossBySite_BetType.CacheData.PreviousBreadScrum = self.CacheData.currentReport.GetFullBreadCrumbs(reportManager.Enum.DrillDownLevel.ByBetType, selectParams);
        reportManager.WinLossBySite_BetType.CacheData.PlainPreviousBreadScrum = self.CacheData.currentReport.GetPlainPreviousBreadCrumbs(reportManager.Enum.DrillDownLevel.ByBetType, selectParams);
        self.CacheData.currentReport = reportManager.WinLossBySite_BetType;
        reportManager.BindingEvent_WinLossBySite_BetType();
        reportManager.Tables.btnShowHideSiteTypeInfo.addClass("display-none");
        self.CacheData.currentReport.GetReport(selectParams);
    };

    self.ShowParlayReport = function (sportId, sportName, winLossDate) {
        self.CacheData.currentReport = reportManager.ParlayReport;
        self.ShowDrilldownReport(sportId, sportName, winLossDate);
    };

    self.BackToParlayReport = function () {
        AccReport.popUp.hide();
        scoreInfoManager.ClosePopup();
        self.CacheData.currentReport.HideReportView();
        reportManager.ParlayReport.tables.datepicker.datepicker("setDate", reportManager.ParlayReport.reportParams.Winlostdate);
        reportManager.ParlayReport.RebindingEvent();
        reportManager.ParlayReport.tables.reportContainer.removeClass("display-none");
        showHideReportManager.CacheData.currentReport = reportManager.ParlayReport;
        self.CacheData.currentReport.tables.report.fanexGrid("resizeGrid");
    };

    self.ShowMetricGamingReport = function (sportId, sportName, winLossDate) {
        self.CacheData.currentReport = reportManager.MetricGamingReport;
        self.ShowDrilldownReport(sportId, sportName, winLossDate);
    };

    self.ShowMainLiveCasinoReport = function (sportId, sportName, winLossDate) {
        self.CacheData.currentReport = reportManager.MainLiveCasinoReport;
        self.ShowDrilldownReport(sportId, sportName, winLossDate);
    };

    self.ShowLiveCasinoWinLossByProductReport = function (sportId, sportName, winLossDate) {
        self.CacheData.currentReport = reportManager.LiveCasinoWinLossByProductReport;
        self.ShowDrilldownReport(sportId, sportName, winLossDate);
    };
};

var showHideReportManager = new ShowHideReportManager();
