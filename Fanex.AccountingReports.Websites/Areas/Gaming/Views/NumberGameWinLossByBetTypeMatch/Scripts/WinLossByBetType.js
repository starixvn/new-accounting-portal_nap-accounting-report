
Namespace.Register("Fanex.AccountingReports.Gaming.NumberGameWinLossByBetTypeMatch");
Fanex.AccountingReports.Gaming.NumberGameWinLossByBetTypeMatch.WinLossByBetType = function () {
    var self = this;

    self.Params = {
        MainReportParams: {
            CompanyList: null,
            CurrencyList: null,
            LiveStatus: null,
            ReportDate: null,
            TrialData: false
        },

        DetailReportParams: {
            CompanyList: null,
            CurrencyList: null,
            LiveStatus: null,
            ReportDate: null,
            BetTypeId: null,
            TrialData: false
        }
    };

    self.MainReport = new Fanex.AccountingReports.Gaming.NumberGameWinLossByBetTypeMatch.WinLossByBetType.MainReport();
    self.DetailReport = new Fanex.AccountingReports.Gaming.NumberGameWinLossByBetTypeMatch.WinLossByBetType.DetailReport();

    self.InitPage = function () {
        self.MainReport.InitPage();
        self.DetailReport.InitPage();
    };

    self.GetMainReport = function (params) {
        self.Params.MainReportParams = params;
        self.MainReport.GetReport(params);
    };

    self.GetDetailReport = function (betTypeId, betTypeName) {
        AccReport.disableInput();
        self.Params.DetailReportParams = {
            CompanyList: self.Params.MainReportParams.CompanyList,
            CurrencyList: self.Params.MainReportParams.CurrencyList,
            LiveStatus: self.Params.MainReportParams.LiveStatus,
            ReportDate: self.Params.MainReportParams.ReportDate,
            BetTypeId: betTypeId,
            TrialData: self.Params.MainReportParams.TrialData
        };

        self.DetailReport.GetReport(self.Params.DetailReportParams);
    };

    self.HideReport = function () {
        self.MainReport.HideReport();
        self.DetailReport.HideReport();
    };
};