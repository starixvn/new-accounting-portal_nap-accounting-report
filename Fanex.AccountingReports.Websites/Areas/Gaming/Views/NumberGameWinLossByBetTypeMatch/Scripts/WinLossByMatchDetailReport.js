
Namespace.Register("Fanex.AccountingReports.Gaming.NumberGameWinLossByBetTypeMatch.WinLossByMatch");
Fanex.AccountingReports.Gaming.NumberGameWinLossByBetTypeMatch.WinLossByMatch.DetailReport = function () {
    var self = this;

    self.Tables = {
        reportContainer: null,
        report: null,
        reportParentContainer: null
    };

    self.Params = {
        reportParams: {
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

    self.InitPage = function () {
        self.CachedDOMElements();
    };

    self.CachedDOMElements = function () {
        self.Tables.reportContainer = $("#winloss-bymatch-detail-container");
        self.Tables.report = $("#winloss-bymatch-detail-grid");
        self.Tables.reportParentContainer = $("#parentContainer");
    };

    self.GetReport = function (params) {
        self.Params.reportParams = params;
        AccReport.utility.CallAjax(this,
            window.rootUrl + "Gaming/NumberGameWinLossByBetTypeMatch/GetWinLossByMatchDetailReport",
            self.Params.reportParams,
            self.RenderReport,
            self.RenderReport,
            AccReport.enableInput,
            false);
    };
    self.RenderReport = function (report) {
        self.Tables.reportContainer.removeClass("display-none");
        self.Tables.report.addClass("display-none");
        var bodyData = [];
        var footerData = [];
        var roleLevelIndex = {
            Company: 0,
            Member: 1,
            Agent: 2,
            Master: 3,
            Super: 4
        };

        if (report && report.DetailInfo && report.DetailInfo.length > 0) {
            var info = null;
            var reportDetailLength = report.DetailInfo.length;
            for (var i = 0; i < reportDetailLength; i++) {
                info = report.DetailInfo[i];
                bodyData.push({
                    Columns: [{ Name: "ColNo", Value: (i + 1) },
                        { Name: "ColUserName", Value: info.Name },

                        { Name: "ColCompanyTurnover", Value: info.DetailInfo[roleLevelIndex.Company].Turnover },
                        { Name: "ColGrossComm", Value: info.DetailInfo[roleLevelIndex.Company].Commission },

                        { Name: "ColCustomerWinLoss", Value: info.DetailInfo[roleLevelIndex.Member].WinLoss },
                        { Name: "ColCustomerCommission", Value: info.DetailInfo[roleLevelIndex.Member].Commission },
                        { Name: "ColCustomerTotal", Value: info.DetailInfo[roleLevelIndex.Member].LineTotal },

                        { Name: "ColAgentWinLoss", Value: info.DetailInfo[roleLevelIndex.Agent].WinLoss },
                        { Name: "ColAgentCommission", Value: info.DetailInfo[roleLevelIndex.Agent].Commission },
                        { Name: "ColAgentTotal", Value: info.DetailInfo[roleLevelIndex.Agent].LineTotal },

                        { Name: "ColMasterWinLoss", Value: info.DetailInfo[roleLevelIndex.Master].WinLoss },
                        { Name: "ColMasterCommission", Value: info.DetailInfo[roleLevelIndex.Master].Commission },
                        { Name: "ColMasterTotal", Value: info.DetailInfo[roleLevelIndex.Master].LineTotal },

                        { Name: "ColSuperWinLoss", Value: info.DetailInfo[roleLevelIndex.Super].WinLoss },
                        { Name: "ColSuperCommission", Value: info.DetailInfo[roleLevelIndex.Super].Commission },
                        { Name: "ColSuperTotal", Value: info.DetailInfo[roleLevelIndex.Super].LineTotal}]
                });
            }

            info = report.GrandTotal;
            footerData.push({
                Columns: [{ Name: "ColNo", Value: "" },
                    { Name: "ColUserName", Value: info.Name },

                    { Name: "ColCompanyTurnover", Value: info.DetailInfo[roleLevelIndex.Company].Turnover },
                    { Name: "ColGrossComm", Value: info.DetailInfo[roleLevelIndex.Company].Commission },

                    { Name: "ColCustomerWinLoss", Value: info.DetailInfo[roleLevelIndex.Member].WinLoss },
                    { Name: "ColCustomerCommission", Value: info.DetailInfo[roleLevelIndex.Member].Commission },
                    { Name: "ColCustomerTotal", Value: info.DetailInfo[roleLevelIndex.Member].LineTotal },

                    { Name: "ColAgentWinLoss", Value: info.DetailInfo[roleLevelIndex.Agent].WinLoss },
                    { Name: "ColAgentCommission", Value: info.DetailInfo[roleLevelIndex.Agent].Commission },
                    { Name: "ColAgentTotal", Value: info.DetailInfo[roleLevelIndex.Agent].LineTotal },

                    { Name: "ColMasterWinLoss", Value: info.DetailInfo[roleLevelIndex.Master].WinLoss },
                    { Name: "ColMasterCommission", Value: info.DetailInfo[roleLevelIndex.Master].Commission },
                    { Name: "ColMasterTotal", Value: info.DetailInfo[roleLevelIndex.Master].LineTotal },

                    { Name: "ColSuperWinLoss", Value: info.DetailInfo[roleLevelIndex.Super].WinLoss },
                    { Name: "ColSuperCommission", Value: info.DetailInfo[roleLevelIndex.Super].Commission },
                    { Name: "ColSuperTotal", Value: info.DetailInfo[roleLevelIndex.Super].LineTotal}]
            });
        }

        var gridOptions = {
            customizeHeader: true,
            groupHeaderRows: [{
                Columns: [
                    { HeaderText: viewResources.Symbol_No, Name: "ColNo", MergeRowNumber: 2 },
                    { HeaderText: viewResources.User_Name, Name: "ColUserName", MergeRowNumber: 2 },
                    { HeaderText: viewResources.Company_Turnover, Name: "ColCompanyTurnover", MergeRowNumber: 2 },
                    { HeaderText: viewResources.Gross_Comm, Name: "ColGrossComm", MergeRowNumber: 2 },

                    { HeaderText: viewResources.Customer, Name: "ColCustomerWinLoss", MergeColNumber: 3, HeaderAlign: "center" },
                    { HeaderText: viewResources.Agent, Name: "ColAgentWinLoss", MergeColNumber: 3, HeaderAlign: "center" },
                    { HeaderText: viewResources.Master, Name: "ColMasterWinLoss", MergeColNumber: 3, HeaderAlign: "center" },
                    { HeaderText: viewResources.Super, Name: "ColSuperWinLoss", MergeColNumber: 3, HeaderAlign: "center"}]
            }],
            columns: [{ HeaderText: viewResources.Symbol_No, Width: 60, Name: "ColNo", HeaderAlign: "center", CellAlign: "center" },
                { HeaderText: viewResources.User_Name, Width: 150, Name: "ColUserName", HeaderAlign: "center", CellAlign: "center" },
                {
                    HeaderText: viewResources.Company_Turnover,
                    Width: 120,
                    Name: "ColCompanyTurnover",
                    HeaderAlign: "center",
                    CellAlign: "right",
                    FormatNumber: FormatConstant.TWO_DECIMAL_NUMBER
                },
                {
                    HeaderText: viewResources.Gross_Comm,
                    Width: 120,
                    Name: "ColGrossComm",
                    HeaderAlign: "center",
                    CellAlign: "right",
                    FormatNumber: FormatConstant.TWO_DECIMAL_NUMBER
                },

                {
                    HeaderText: viewResources.Win_Loss,
                    Width: 120,
                    Name: "ColCustomerWinLoss",
                    HeaderAlign: "center",
                    CellAlign: "right",
                    FormatNumber: FormatConstant.TWO_DECIMAL_NUMBER,
                    FormatPositive: '$'
                },
                {
                    HeaderText: viewResources.Commission,
                    Width: 100,
                    Name: "ColCustomerCommission",
                    HeaderAlign: "center",
                    CellAlign: "right",
                    FormatNumber: FormatConstant.TWO_DECIMAL_NUMBER
                },
                {
                    HeaderText: viewResources.Total,
                    Width: 120,
                    Name: "ColCustomerTotal",
                    HeaderAlign: "center",
                    CellAlign: "right",
                    FormatNumber: FormatConstant.TWO_DECIMAL_NUMBER,
                    FormatPositive: '$'
                },

                {
                    HeaderText: viewResources.Win_Loss,
                    Width: 120,
                    Name: "ColAgentWinLoss",
                    HeaderAlign: "center",
                    CellAlign: "right",
                    FormatNumber: FormatConstant.TWO_DECIMAL_NUMBER,
                    FormatPositive: '$'
                },
                {
                    HeaderText: viewResources.Commission,
                    Width: 100,
                    Name: "ColAgentCommission",
                    HeaderAlign: "center",
                    CellAlign: "right",
                    FormatNumber: FormatConstant.TWO_DECIMAL_NUMBER
                },
                {
                    HeaderText: viewResources.Total,
                    Width: 120,
                    Name: "ColAgentTotal",
                    HeaderAlign: "center",
                    CellAlign: "right",
                    FormatNumber: FormatConstant.TWO_DECIMAL_NUMBER,
                    FormatPositive: '$'
                },

                {
                    HeaderText: viewResources.Win_Loss,
                    Width: 120,
                    Name: "ColMasterWinLoss",
                    HeaderAlign: "center",
                    CellAlign: "right",
                    FormatNumber: FormatConstant.TWO_DECIMAL_NUMBER,
                    FormatPositive: '$'
                },
                {
                    HeaderText: viewResources.Commission,
                    Width: 100,
                    Name: "ColMasterCommission",
                    HeaderAlign: "center",
                    CellAlign: "right",
                    FormatNumber: FormatConstant.TWO_DECIMAL_NUMBER
                },
                {
                    HeaderText: viewResources.Total,
                    Width: 120,
                    Name: "ColMasterTotal",
                    HeaderAlign: "center",
                    CellAlign: "right",
                    FormatNumber: FormatConstant.TWO_DECIMAL_NUMBER,
                    FormatPositive: '$'
                },

                {
                    HeaderText: viewResources.Win_Loss,
                    Width: 120,
                    Name: "ColSuperWinLoss",
                    HeaderAlign: "center",
                    CellAlign: "right",
                    FormatNumber: FormatConstant.TWO_DECIMAL_NUMBER,
                    FormatPositive: '$'
                },
                {
                    HeaderText: viewResources.Commission,
                    Width: 100,
                    Name: "ColSuperCommission",
                    HeaderAlign: "center",
                    CellAlign: "right",
                    FormatNumber: FormatConstant.TWO_DECIMAL_NUMBER
                },
                {
                    HeaderText: viewResources.Total,
                    Width: 120,
                    Name: "ColSuperTotal",
                    HeaderAlign: "center",
                    CellAlign: "right",
                    FormatNumber: FormatConstant.TWO_DECIMAL_NUMBER,
                    FormatPositive: '$'
                }],
            showTitle: true,
            showFooter: true,
            titleInfo: {
                text: report ? report.Name : '',
                showExcelButton: true,
                exportExcelFunction: function () {
                    self.ExportReport();
                }
            },
            bodyRows: bodyData,
            footerRows: footerData,
            numberOfFrozenColumn: 4,
            gridWidth: 1035,
            gridExpandHeight: 0,
            gridHeight: 450,
            fullWidth: false,
            fullHeight: false,
            showDistinctRow: true,
            showHighlightRow: true,
            headerColumnHeight: 17,
            showPager: false
        };

        self.Tables.reportContainer.dialog({
            title: "",
            autoOpen: false,
            height: 550,
            width: 1065,
            modal: true,
            resizable: false,
            close: function (event, ui) {
                self.Tables.reportContainer.dialog("destroy").remove();
                self.Tables.reportParentContainer.append(
                    ' <div id="winloss-bymatch-detail-container" class="display-none content-grid width-100percent">\
                        <div id="winloss-bymatch-detail-grid" class="freegrid"></div>\
                    </div>');
                self.Tables.reportContainer = $("#winloss-bymatch-detail-container");
                self.Tables.report = $("#winloss-bymatch-detail-grid");
            }
        });

        self.Tables.reportContainer.dialog("open");
        self.Tables.report.removeClass("display-none").fanexGrid(gridOptions);
    };

    self.ExportReport = function () {
        AccReport.downloadFile(window.rootUrl + "Gaming/NumberGameWinLossByBetTypeMatch/ExportWinLossByMatchDetailReport", self.Params.reportParams);
    };

    self.HideReport = function () {
        self.Tables.reportContainer.addClass("display-none");
    };
};