$(document).ready(function () {
    Inheritance_Manager.extend(Fanex.AccountingReports.Licensee.WinLossByLicensee.CurrencyReport,
                               Fanex.AccountingReports.Licensee.WinLossByLicensee.BaseReport);
    Inheritance_Manager.extend(Fanex.AccountingReports.Licensee.WinLossByLicensee.SuperReport,
                               Fanex.AccountingReports.Licensee.WinLossByLicensee.BaseReport);
    Inheritance_Manager.extend(Fanex.AccountingReports.Licensee.WinLossByLicensee.MasterReport,
                               Fanex.AccountingReports.Licensee.WinLossByLicensee.BaseReport);
    Inheritance_Manager.extend(Fanex.AccountingReports.Licensee.WinLossByLicensee.AgentReport,
                               Fanex.AccountingReports.Licensee.WinLossByLicensee.BaseReport);
    Inheritance_Manager.extend(Fanex.AccountingReports.Licensee.WinLossByLicensee.MemberReport,
                               Fanex.AccountingReports.Licensee.WinLossByLicensee.BaseReport);

    var winLossByLicenseeReportManager = new Fanex.AccountingReports.Licensee.WinLossByLicensee.ReportManager();
    winLossByLicenseeReportManager.initPage();
});

Namespace.Register("Fanex.AccountingReports.Licensee.WinLossByLicensee.ReportManager");
Fanex.AccountingReports.Licensee.WinLossByLicensee.ReportManager = function () {
    var self = this;

    var _currencyReport = new Fanex.AccountingReports.Licensee.WinLossByLicensee.CurrencyReport();
    var _superReport = new Fanex.AccountingReports.Licensee.WinLossByLicensee.SuperReport();
    var _masterReport = new Fanex.AccountingReports.Licensee.WinLossByLicensee.MasterReport();
    var _agentReport = new Fanex.AccountingReports.Licensee.WinLossByLicensee.AgentReport();
    var _memberReport = new Fanex.AccountingReports.Licensee.WinLossByLicensee.MemberReport();

    enums = {
        drilldownLevel: {
            Currency: "Currency",
            Super: "Super",
            Master: "Master",
            Agent: "Agent",
            Member: "Member",
            BetList: "BetList"
        },

        reportType: {
            Daily: 1,
            Monthly: 2
        },

        keenOcean: 96
    };

    var _tables = {
        superReport_Back: null,
        masterReport_Back: null,
        agentReport_Back: null,
        memberReport_Back: null
    };

    self.initPage = function () {
        cachedDOMElements();
        //bindingEvent();

        _currencyReport.initPage();
        _superReport.initPage();
        _masterReport.initPage();
        _agentReport.initPage();
        _memberReport.initPage();
    };
    cachedDOMElements = function () {
        _tables.superReport_Back = $("#superReport_Back");
        _tables.masterReport_Back = $("#masterReport_Back");
        _tables.agentReport_Back = $("#agentReport_Back");
        _tables.memberReport_Back = $("#memberReport_Back");
    };
    bindingEvent = function () {
        _tables.agentReport_Back.click(function () {
            showReport(enums.drilldownLevel.Currency);
                });
        //bindReportBackButton()
    };
    checkBoxIsChecked = function (checkBox) {
        return (checkBox && checkBox.attr("checked")) ? true : false;
    };
    showReport = function (level) {
        switch (level) {
            case enums.drilldownLevel.Currency:
                _agentReport.hideReport();
                _superReport.hideReport();
                _masterReport.hideReport();
                _memberReport.hideReport();
                _currencyReport.showReport();
                break;
            case enums.drilldownLevel.Super:
                _agentReport.hideReport();
                _superReport.showReport();
                _masterReport.hideReport();
                _memberReport.hideReport();
                _currencyReport.hideReport();
                break;
            case enums.drilldownLevel.Master:
                _agentReport.hideReport();
                _superReport.hideReport();
                _masterReport.showReport();
                _memberReport.hideReport();
                _currencyReport.hideReport();
                break;
            case enums.drilldownLevel.Agent:
                _agentReport.showReport();
                _superReport.hideReport();
                _masterReport.hideReport();
                _memberReport.hideReport();
                _currencyReport.hideReport();
                break;
            case enums.drilldownLevel.Member:
                _agentReport.hideReport();
                _superReport.hideReport();
                _masterReport.hideReport();
                _memberReport.showReport();
                _currencyReport.hideReport();
                break;
            case enums.drilldownLevel.Member:
                break;
        }
    };

    drilldownToSuperReport = function (currencyId,
        currencyName,
        bettingDate,
        fromDate,
        toDate) {
        bindReportBackButton(enums.drilldownLevel.Currency);
        _superReport.getReport({
            LicenseeID: _currencyReport.Params.reportParams.LicenseeID,
            LicenseeName: _currencyReport.Params.reportParams.LicenseeName,
            Currency: currencyId,
            CurrencyName: currencyName,
            LevelReport: 1,
            ShowSB: _currencyReport.Params.reportParams.ShowSB,
            ShowRB: _currencyReport.Params.reportParams.ShowRB,
            ShowNG: _currencyReport.Params.reportParams.ShowNG,
            ShowBA: _currencyReport.Params.reportParams.ShowBA,
            ShowLC: _currencyReport.Params.reportParams.ShowLC,
            ShowVS: _currencyReport.Params.reportParams.ShowVS,
            ShowCS: _currencyReport.Params.reportParams.ShowCS,
            ShowKN: _currencyReport.Params.reportParams.ShowKN,
            ShowAGC: _currencyReport.Params.reportParams.ShowAGC,
            ShowGD: _currencyReport.Params.reportParams.ShowGD,
            ShowMGC: _currencyReport.Params.reportParams.ShowMGC,
            ReportType: parseInt(_currencyReport.Params.reportParams.ReportType),
            Drawfilter: _currencyReport.Params.reportParams.Drawfilter,
            ConvertToCurrency: _currencyReport.Params.reportParams.ConvertToCurrency,
            ConvertToCurrencyName: _currencyReport.Params.reportParams.ConvertToCurrencyName,
            BettingDate: bettingDate,
            FromDate: fromDate,
            ToDate: toDate
        });
    };

    drilldownToMasterReport = function (userId,
        userName,
        bettingDate,
        fromDate,
        toDate,
        parentLevel) {

        bindReportBackButton(enums.drilldownLevel.Super);

        _masterReport.getReport({
            LicenseeID: _superReport.Params.reportParams.LicenseeID,
            LicenseeName: _superReport.Params.reportParams.LicenseeName,
            LevelReport: 2,
            Currency: _superReport.Params.reportParams.Currency,
            CurrencyName: _superReport.Params.reportParams.CurrencyName,
            CustID: userId,
            UserName: userName,
            BettingDate: bettingDate,
            FromDate: fromDate,
            ToDate: toDate,

            ShowSB: _superReport.Params.reportParams.ShowSB,
            ShowRB: _superReport.Params.reportParams.ShowRB,
            ShowNG: _superReport.Params.reportParams.ShowNG,
            ShowBA: _superReport.Params.reportParams.ShowBA,
            ShowLC: _superReport.Params.reportParams.ShowLC,
            ShowVS: _superReport.Params.reportParams.ShowVS,
            ShowCS: _superReport.Params.reportParams.ShowCS,
            ShowKN: _superReport.Params.reportParams.ShowKN,
            ShowAGC: _superReport.Params.reportParams.ShowAGC,
            ShowGD: _superReport.Params.reportParams.ShowGD,
            ShowMGC: _superReport.Params.reportParams.ShowMGC,
            ReportType: parseInt(_agentReport.Params.reportParams.ReportType),
            Drawfilter: _superReport.Params.reportParams.Drawfilter,
            ConvertToCurrency: _superReport.Params.reportParams.ConvertToCurrency,
            ConvertToCurrencyName: _superReport.Params.reportParams.ConvertToCurrencyName
        });

    };

    drilldownToAgentReport = function (userId,
        userName,
        bettingDate,
        fromDate,
        toDate,
        parentLevel) {
        bindReportBackButton(enums.drilldownLevel.Master);
        _agentReport.getReport({
            LicenseeID: _masterReport.Params.reportParams.LicenseeID,
            LicenseeName: _masterReport.Params.reportParams.LicenseeName,
            LevelReport: 3,
            Currency: _masterReport.Params.reportParams.Currency,
            CurrencyName: _masterReport.Params.reportParams.CurrencyName,
            CustID: userId,
            UserName: userName,
            BettingDate: bettingDate,
            FromDate: fromDate,
            ToDate: toDate,

            ShowSB: _masterReport.Params.reportParams.ShowSB,
            ShowRB: _masterReport.Params.reportParams.ShowRB,
            ShowNG: _masterReport.Params.reportParams.ShowNG,
            ShowBA: _masterReport.Params.reportParams.ShowBA,
            ShowLC: _masterReport.Params.reportParams.ShowLC,
            ShowVS: _masterReport.Params.reportParams.ShowVS,
            ShowCS: _masterReport.Params.reportParams.ShowCS,
            ShowKN: _masterReport.Params.reportParams.ShowKN,
            ShowAGC: _masterReport.Params.reportParams.ShowAGC,
            ShowGD: _masterReport.Params.reportParams.ShowGD,
            ShowMGC: _masterReport.Params.reportParams.ShowMGC,
            ReportType: parseInt(_agentReport.Params.reportParams.ReportType),
            Drawfilter: _masterReport.Params.reportParams.Drawfilter,
            ConvertToCurrency: _masterReport.Params.reportParams.ConvertToCurrency,
            ConvertToCurrencyName: _masterReport.Params.reportParams.ConvertToCurrencyName
        });
    };
    drilldownToMemberReport = function (userId,
        userName,
        bettingDate,
        fromDate,
        toDate,
        parentLevel) {

       bindReportBackButton(enums.drilldownLevel.Agent);

        _memberReport.getReport({
            LicenseeID: _agentReport.Params.reportParams.LicenseeID,
            LicenseeName: _agentReport.Params.reportParams.LicenseeName,
            Currency: _agentReport.Params.reportParams.Currency,
            CurrencyName: _agentReport.Params.reportParams.CurrencyName,
            LevelReport: 4, 
            CustID: userId,
            UserName: userName,
            BettingDate: bettingDate,
            FromDate: fromDate,
            ToDate: toDate,

            ShowSB: _agentReport.Params.reportParams.ShowSB,
            ShowRB: _agentReport.Params.reportParams.ShowRB,
            ShowNG: _agentReport.Params.reportParams.ShowNG,
            ShowBA: _agentReport.Params.reportParams.ShowBA,
            ShowLC: _agentReport.Params.reportParams.ShowLC,
            ShowVS: _agentReport.Params.reportParams.ShowVS,
            ShowCS: _agentReport.Params.reportParams.ShowCS,
            ShowKN: _agentReport.Params.reportParams.ShowKN,
            ShowAGC: _agentReport.Params.reportParams.ShowAGC,
            ShowGD: _agentReport.Params.reportParams.ShowGD,
            ShowMGC: _agentReport.Params.reportParams.ShowMGC,
            ReportType: parseInt(_agentReport.Params.reportParams.ReportType),
            Drawfilter: _agentReport.Params.reportParams.Drawfilter,
            ConvertToCurrency: _agentReport.Params.reportParams.ConvertToCurrency,
            ConvertToCurrencyName: _agentReport.Params.reportParams.ConvertToCurrencyName
        });
    };
    drilldownToMemberReportFromCurrency = function (currencyId,
        currencyName,
        bettingDate,
        fromDate,
        toDate) {

        bindReportBackButton(enums.drilldownLevel.Currency);

        var agentReportParams = {
            LicenseeID: _currencyReport.Params.reportParams.LicenseeID,
            LicenseeName: _currencyReport.Params.reportParams.LicenseeName,
            Currency: currencyId,
            CurrencyName: currencyName,
            ShowSB: _currencyReport.Params.reportParams.ShowSB,
            ShowRB: _currencyReport.Params.reportParams.ShowRB,
            ShowNG: _currencyReport.Params.reportParams.ShowNG,
            ShowBA: _currencyReport.Params.reportParams.ShowBA,
            ShowLC: _currencyReport.Params.reportParams.ShowLC,
            ShowVS: _currencyReport.Params.reportParams.ShowVS,
            ShowCS: _currencyReport.Params.reportParams.ShowCS,
            ShowKN: _currencyReport.Params.reportParams.ShowKN,
            ShowAGC: _currencyReport.Params.reportParams.ShowAGC,
            ShowGD: _currencyReport.Params.reportParams.ShowGD,
            ShowMGC: _currencyReport.Params.reportParams.ShowMGC,
            ReportType: parseInt(_currencyReport.Params.reportParams.ReportType),
            Drawfilter: _currencyReport.Params.reportParams.Drawfilter,
            ConvertToCurrency: _currencyReport.Params.reportParams.ConvertToCurrency,
            ConvertToCurrencyName: _currencyReport.Params.reportParams.ConvertToCurrencyName,
            BettingDate: bettingDate,
            FromDate: fromDate,
            ToDate: toDate
        };

        _agentReport.getParams(agentReportParams);

        _memberReport.getReport({
            LicenseeID: _agentReport.Params.reportParams.LicenseeID,
            LicenseeName: _agentReport.Params.reportParams.LicenseeName,
            Currency: _agentReport.Params.reportParams.Currency,
            CurrencyName: _agentReport.Params.reportParams.CurrencyName,

            CustID: 0,
            UserName: _currencyReport.Params.reportParams.LicenseeName,
            BettingDate: bettingDate,
            FromDate: fromDate,
            ToDate: toDate,

            ShowSB: _agentReport.Params.reportParams.ShowSB,
            ShowRB: _agentReport.Params.reportParams.ShowRB,
            ShowNG: _agentReport.Params.reportParams.ShowNG,
            ShowBA: _agentReport.Params.reportParams.ShowBA,
            ShowLC: _agentReport.Params.reportParams.ShowLC,
            ShowVS: _agentReport.Params.reportParams.ShowVS,
            ShowCS: _agentReport.Params.reportParams.ShowCS,
            ShowKN: _agentReport.Params.reportParams.ShowKN,
            ShowAGC: _agentReport.Params.reportParams.ShowAGC,
            ShowGD: _agentReport.Params.reportParams.ShowGD,
            ShowMGC: _agentReport.Params.reportParams.ShowMGC,
            ReportType: parseInt(_agentReport.Params.reportParams.ReportType),
            Drawfilter: _agentReport.Params.reportParams.Drawfilter,
            ConvertToCurrency: _agentReport.Params.reportParams.ConvertToCurrency,
            ConvertToCurrencyName: _agentReport.Params.reportParams.ConvertToCurrencyName
        });

    };
    drilldownToBestListReport = function (userId,
        userName,
        bettingDate,
        fromDate,
        toDate,
        drilldownLevel,
        currencyId) {

        //window.open(url, '_blank');
        var winLossMemberBetListCriteria =
        {
        	ShowSB: _memberReport.Params.reportParams.ShowSB,
        	ShowRB: _memberReport.Params.reportParams.ShowRB,
        	ShowNG: _memberReport.Params.reportParams.ShowNG,
        	ShowBA: _memberReport.Params.reportParams.ShowBA,
        	ShowLC: _memberReport.Params.reportParams.ShowLC,
        	ShowVS: _memberReport.Params.reportParams.ShowVS,
        	ShowCS: _memberReport.Params.reportParams.ShowCS,
        	ShowKN: _memberReport.Params.reportParams.ShowKN,
        	ShowAGC: _memberReport.Params.reportParams.ShowAGC,
        	ShowGD: _memberReport.Params.reportParams.ShowGD,
        	ShowMGC: _memberReport.Params.reportParams.ShowMGC,
            CustID: userId,
            CustName: userName,
            BaseCurrency: _memberReport.Params.reportParams.ConvertToCurrency,
            Drawfilter: _memberReport.Params.reportParams.Drawfilter,
            FromDate: fromDate,
            ToDate: toDate,
            CurrencyId: currencyId
        };
        var url = AccReport.accReportRoot + "BetList/BetList/WinLossMemberBetList" + "?" + $.param(winLossMemberBetListCriteria);
        var memBetListWindow = window.open(url, 'BetList', ' scrollbars=yes, toolbar=yes, fullscreen=yes, menubar=yes,location=yes,status=yes,resizable=yes');
        if (memBetListWindow) memBetListWindow.focus();
    };
    bindReportBackButton = function (parentLevel) {
        if (parentLevel == "Currency") {
            _tables.superReport_Back.unbind("click");
            _tables.superReport_Back.click(function () {
                showReport(parentLevel);
            });
        }
        else if (parentLevel == "Super") {
            _tables.masterReport_Back.unbind("click");
            _tables.masterReport_Back.click(function () {
                showReport(parentLevel);
            });
        }
        else if (parentLevel == "Master") {
            _tables.agentReport_Back.unbind("click");
            _tables.agentReport_Back.click(function () {
                showReport(parentLevel);
            });
        }
        else if (parentLevel == "Agent") {
            _tables.memberReport_Back.unbind("click");
            _tables.memberReport_Back.click(function () {
                showReport(parentLevel);
            });
        }
       
    };
}