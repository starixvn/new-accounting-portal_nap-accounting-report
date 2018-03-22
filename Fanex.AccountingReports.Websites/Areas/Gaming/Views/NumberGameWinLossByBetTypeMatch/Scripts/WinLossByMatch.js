
Namespace.Register("Fanex.AccountingReports.Gaming.NumberGameWinLossByBetTypeMatch");
Fanex.AccountingReports.Gaming.NumberGameWinLossByBetTypeMatch.WinLossByMatch = function () {
    var self = this;

    self.Params = {
        MainReportParams: {
            CompanyList: null,
            CurrencyList: null,
            BetTypeList: null,
            ReportDate: null,
            TrialData: false
        },

        DetailReportParams: {
            CompanyList: null,
            CurrencyList: null,
            ReportDate: null,
            BetTypeId: null,
            LiveStatus: null,
            MatchId: null,
            MatchCode: null,
            TrialData: false
        }
    };

    self.MainReport = new Fanex.AccountingReports.Gaming.NumberGameWinLossByBetTypeMatch.WinLossByMatch.MainReport();
    self.DetailReport = new Fanex.AccountingReports.Gaming.NumberGameWinLossByBetTypeMatch.WinLossByMatch.DetailReport();

    self.InitPage = function () {
        self.MainReport.InitPage();
        self.DetailReport.InitPage();
    };

    self.GetMainReport = function (params) {
        self.Params.MainReportParams = params;
        self.MainReport.GetReport(params);
    };

    self.GetDetailReport = function (betTypeId,
        betTypeName,
        liveStatus,
        matchId,
        matchCode) {
        AccReport.disableInput();
        self.Params.DetailReportParams = {
            CompanyList: self.Params.MainReportParams.CompanyList,
            CurrencyList: self.Params.MainReportParams.CurrencyList,
            ReportDate: self.Params.MainReportParams.ReportDate,
            BetTypeId: betTypeId,
            LiveStatus: liveStatus,
            MatchId: matchId,
            MatchCode: matchCode,
            TrialData: self.Params.MainReportParams.TrialData
        };

        self.DetailReport.GetReport(self.Params.DetailReportParams);
    };

    self.HideReport = function () {
        self.MainReport.HideReport();
        self.DetailReport.HideReport();
    };
};