ShowHideReportManager = function () {
    var self = this;

    self.ReportManager = reportManager;
    self.CacheData = {
        currentReport: reportManager.Main2hReport
    };

    self.ShowOtherSportReport = function (sportId,
        sportName,
        winLossDate) {
        self.CacheData.currentReport = reportManager.OtherSportReport;
        self.ShowDrilldownReport(sportId, sportName, winLossDate);
    };

    self.ShowDrilldownReport = function (sportId, sportName, winLossDate) {
        reportManager.Main2hReport.Tables.reportContainer.addClass("display-none");
        self.CacheData.currentReport.InitPage();
        self.CacheData.currentReport.CacheData.SportId = sportId;
        self.CacheData.currentReport.CacheData.SportName = sportName;
        self.CacheData.currentReport.GetReport({
            Winlostdate: winLossDate
        });
    };

    self.BackToMainReport = function () {
        AccReport.popUp.hide();
        scoreInfoManager.ClosePopup();
        self.CacheData.currentReport.HideReportView();
        reportManager.Tables.datepicker.datepicker("setDate", reportManager.Main2hReport.Params.reportParams.Winlostdate);
        reportManager.RebindingEvent();
        reportManager.Main2hReport.Tables.reportContainer.removeClass("display-none");
        showHideReportManager.CacheData.currentReport = reportManager.Main2hReport;
    };

    self.ShowWinlossBySite = function (winLossDate,
        sportType,
        sportGroupId,
        betType,
        betTypeName,
        isLive,
        isHedge,
        siteType,
        sportTypeName) {
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
            SportTypeName: sportTypeName
        };

        reportManager.WinLossBySite_BetType.CacheData.PreviousBreadScrum = self.CacheData.currentReport.GetFullBreadCrumbs(reportManager.Enum.DrillDownLevel.ByBetType, selectParams);
        reportManager.WinLossBySite_BetType.CacheData.PlainPreviousBreadScrum = self.CacheData.currentReport.GetPlainPreviousBreadCrumbs(reportManager.Enum.DrillDownLevel.ByBetType, selectParams);
        self.CacheData.currentReport = reportManager.WinLossBySite_BetType;
        reportManager.BindingEvent_WinLossBySite_BetType();
        self.CacheData.currentReport.GetReport(selectParams);
    };
};

var showHideReportManager = new ShowHideReportManager();
