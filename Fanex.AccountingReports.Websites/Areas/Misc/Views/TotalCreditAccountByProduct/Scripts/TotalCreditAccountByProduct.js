Namespace.Register("Fanex.AccountingReports.Misc.TotalCreditAccountByProduct");

Fanex.AccountingReports.Misc.TotalCreditAccountByProduct = function () {
    var self = this;

    var _table = {
        ddlProduct: null,
        ddlCurrency: null,
        ddlStatus: null,
        btnGenerate: null,
        reportContainer: null,
        reportDetailContainer: null,
        report: null,
        reportDetail: null,
        currencyList: null
    };

    var cachedDomElememts = function () {
        _table.ddlProduct = $("#ddlProduct");
        _table.ddlStatus = $("#ddlStatus");
        _table.btnGenerate = $("#btnGenerate");
        _table.reportContainer = $("#reportContainer");
        _table.report = $("#reportContent");
        _table.reportDetailContainer = $("#reportDetailContainer");
        _table.reportDetail = $("#reportDetailContent");
    };

    var productEnums = {
        Casino: 251,
        Racing: 151,
        Bingo: 252,
        NumberGame: 161,
        LiveCasiono: 162,
        VirtualSport: 180,
        Keno: 202,
        ColossusBets: 204,
        AllBet: 211,
        MetricGamingSuperLive: -1,
        MacauGames: 212,
        NG2: 213,
        Lottery: 220,
        Lucky3: 160,
        Bullfighting: 207,
        GoldDeluxe: 255,
        PockerGames: 163,
        VirtualGames: 245,
        SAGaming: 244,
        SexyGaming: 243,
        SabaClub: 200,
        TableGames: 222
    };

    var cachedData = {
        totalCreaditAccountByProduct: {
            currentPage: 0,
            itemsPerPage: 50,
            itemsPerPageArray: [50, 100, 200, 300, 500]
        },
        betCountDetail: {
            currentPage: 0,
            itemsPerPage: 50,
            itemsPerPageArray: [50, 100, 200, 300, 500]
        },
        superUserName: null
    };

    self.reportParams = {
        ListCurrency: null,
        ShowEnabled: null,
        ShowDisabled: null,
        SportType: null,
        PageIndex: 0,
        PageSize: 50
    };

    self.reportDetailParams = {
        CustID: null,
        SportType: null,
        SuperUserName: null,
        PageIndex: 0,
        PageSize: 50
    };

    self.initPage = function () {
        cachedDomElememts();
        self.getProducts();
        self.getCurrency();
        self.bindingEvents();
    };

    self.bindingEvents = function () {
        _table.btnGenerate.click(function () {
            cachedData.totalCreaditAccountByProduct.itemsPerPage = 50;
            cachedData.totalCreaditAccountByProduct.currentPage = 0;
            self.hideBetCountDetailReport();
            self.getTotalCreditAccountByProduct();
        });
    };

    self.getProducts = function () {
        AccReport.disableInput();
        AccReport.utility.CallAjax(this,
                                    window.rootUrl + "Misc/TotalCreditAccountByProduct/GetProducts",
                                    null,
                                    self.renderProducts,
                                    self.renderProducts,
                                    AccReport.enableInput);
    };

    self.renderProducts = function (rs) {
        for (var i = 0; i < rs.length; i++) {
            var currency = rs[i];
            _table.ddlProduct.append('<option value="' + currency.Id + '">' + currency.Name + '</option>');
        }
    };

    self.getCurrency = function () {
        AccReport.disableInput();
        AccReport.utility.CallAjax(this,
                                    window.rootUrl + "Misc/TotalCreditAccountByProduct/GetCurrency",
                                    null,
                                    self.renderCurrency,
                                    self.renderCurrency,
                                    AccReport.enableInput);
    };

    self.renderCurrency = function (rs) {
        var noItemText = 'No currency found.';
        var ddwnCurrency = [];
        var currencyLength = rs.length;

        for (var i = 0; i < currencyLength; i++) {
            var currency = rs[i];
            ddwnCurrency.push({
                Id: "" + currency.Id.toString(),
                Value: currency.Id,
                Text: currency.Name,
                Checked: true,
                Enable: true,
                CssClass: "",
                Items: []
            });
        }

        $("#ddlCurrency").multiSelect(
        {
            minWidth: 140,
            maxHeight: 450,
            shadow: true,
            showHeader: true,
            isCheckAll: true,
            showFooter: false,
            showTooltip: false,
            checkAllText: 'All Currencies',
            isUsedCustomCheckAllText: true,
            customCheckAllText: 'All Currencies',
            noItemText: noItemText,
            selectedList: 3,
            listData: ddwnCurrency
        });

        _table.ddlCurrency = $("#ddlCurrency");
    };

    self.hideBetCountDetailReport = function () {
        _table.reportDetailContainer.addClass("display-none");
        _table.reportDetail.addClass("display-none");
    };

    self.getStatus = function () {
        if (_table.ddlStatus.val() === "All") {
            self.reportParams.ShowEnabled = true;
            self.reportParams.ShowDisabled = true;
        }
        else if (_table.ddlStatus.val() === "Enabled") {
            self.reportParams.ShowEnabled = true;
            self.reportParams.ShowDisabled = false;
        }
        else {
            self.reportParams.ShowEnabled = false;
            self.reportParams.ShowDisabled = true;
        }
    };

    self.getParams = function () {
        self.reportParams.ListCurrency = _table.ddlCurrency.multiSelect('getCheckedItems').toString();
        self.reportParams.SportType = parseInt(_table.ddlProduct.val());
        self.getStatus();
        self.reportParams.PageIndex = cachedData.totalCreaditAccountByProduct.currentPage;
        self.reportParams.PageSize = cachedData.totalCreaditAccountByProduct.itemsPerPage;
        return self.reportParams;
    };

    self.getDetailReportParams = function () {
        self.reportDetailParams.SportType = self.reportParams.SportType;
        self.reportDetailParams.PageIndex = cachedData.betCountDetail.currentPage;
        self.reportDetailParams.PageSize = cachedData.betCountDetail.itemsPerPage;

        return self.reportDetailParams;
    };

    self.checkReportParams = function (params) {
        AccReport.popUp.removeHighLight();

        if (params.ListCurrency == '') {
            AccReport.popUp.show("Please select  at least one currency!", "ddlCurrency");
            return false;
        }

        return true;
    };

    self.getTotalCreditAccountByProduct = function () {
        var params = self.getParams();

        if (self.checkReportParams(params)) {
            AccReport.disableInput();
            AccReport.utility.CallAjax(this,
                                    window.rootUrl + "Misc/TotalCreditAccountByProduct/GetTotalCreditAccountByProduct",
                                    params,
                                    self.renderTotalCreaditAccountByProduct,
                                    self.renderTotalCreaditAccountByProduct,
                                    AccReport.enableInput);
        }
    };

    self.getTotalCreditAccountByProductGridOptions = function (rs) {
        var title = "Total Credit Accounts " + self.getTitleByStatus() + " on " + self.getProductNameBySport(self.reportParams.SportType);
        var bodyRows = [];
        var footerData = [];
        var rowNumber = cachedData.totalCreaditAccountByProduct.currentPage * cachedData.totalCreaditAccountByProduct.itemsPerPage;

        var groupHeader = [
            {
                Columns: [
                    { HeaderText: "#", Name: "ColNo", MergeRowNumber: 2 },
                    { HeaderText: "Super", Name: "ColSuper", MergeRowNumber: 2 },
                    { HeaderText: "Status", Name: "ColStatus", MergeRowNumber: 2 },
                    { HeaderText: "Currency", Name: "ColCurrency", MergeRowNumber: 2 },
                    { HeaderText: "Master", Name: "ColMasterEnable", MergeColNumber: 2 },
                    { HeaderText: "Agent", Name: "ColAgentEnable", MergeColNumber: 2 },
                    { HeaderText: "Member", Name: "ColMemberEnable", MergeColNumber: 2 }
                ]
            }
        ];

        var columns = [
            { HeaderText: "#", Width: 60, Name: "ColNo", HeaderAlign: "center", CellAlign: "center", FormatNumber: FormatConstant.INT_NUMBER },
            { HeaderText: "Super", Width: 150, Name: "ColSuper", HeaderAlign: "center", CellAlign: "left" },
            { HeaderText: "Status", Width: 100, Name: "ColStatus", HeaderAlign: "center", CellAlign: "center" },
            { HeaderText: "Currency", Width: 80, Name: "ColCurrency", HeaderAlign: "center", CellAlign: "center" },
            { HeaderText: "Enabled", Width: 80, Name: "ColMasterEnable", HeaderAlign: "center", CellAlign: "right", FormatNumber: FormatConstant.INT_NUMBER },
            { HeaderText: "Disabled", Width: 80, Name: "ColMasterDisable", HeaderAlign: "center", CellAlign: "right", FormatNumber: FormatConstant.INT_NUMBER },
            { HeaderText: "Enabled", Width: 80, Name: "ColAgentEnable", HeaderAlign: "center", CellAlign: "right", FormatNumber: FormatConstant.INT_NUMBER },
            { HeaderText: "Disabled", Width: 80, Name: "ColAgentDisable", HeaderAlign: "center", CellAlign: "right", FormatNumber: FormatConstant.INT_NUMBER },
            { HeaderText: "Enabled", Width: 80, Name: "ColMemberEnable", HeaderAlign: "center", CellAlign: "right", FormatNumber: FormatConstant.INT_NUMBER },
            { HeaderText: "Disabled", Width: 80, Name: "ColMemberDisable", HeaderAlign: "center", CellAlign: "right", FormatNumber: FormatConstant.INT_NUMBER }
        ];

        reportLength = rs.ReportRows.length;
        if (rs && reportLength > 0) {
            var currentRow = null;
            for (var i = 0; i < reportLength; i++) {
                var currentRow = rs.ReportRows[i];
                currentRowData = [
                    { Name: "ColNo", Value: rowNumber + 1 },
                    { Name: "ColSuper", Value: currentRow.UserName },
                    { Name: "ColStatus", Value: currentRow.IsDisable ? "Disabled" : "Enabled" },
                    { Name: "ColCurrency", Value: currentRow.Currency },
                    { Name: "ColMasterEnable", Value: currentRow.MasterEnable },
                    { Name: "ColMasterDisable", Value: currentRow.MasterDisable },
                    { Name: "ColAgentEnable", Value: currentRow.AgentEnable },
                    { Name: "ColAgentDisable", Value: currentRow.AgentDisable },
                    { Name: "ColMemberEnable", Value: currentRow, CustomCellRender: function (currentCell) {
                        return "<a class='link' onclick='getBetCountDetailByCustId(\"" + currentCell.Value.CustId + "\",\"" + currentCell.Value.UserName + "\")'>" +
                            numeral(currentCell.Value.MemberEnable).format(FormatConstant.INT_NUMBER) + "</a>";
                    }
                    },
                    { Name: "ColMemberDisable", Value: currentRow.MemberDisable }
                ];

                bodyRows.push({
                    Columns: currentRowData
                });

                rowNumber++;
            }

            footerData.push({
                Columns: [
                    { Name: "ColNo", Value: "Total", MergeColNumber: 4 },
                    { Name: "ColMasterEnable", Value: rs.Total.MasterEnable },
                    { Name: "ColMasterDisable", Value: rs.Total.MasterDisable },
                    { Name: "ColAgentEnable", Value: rs.Total.AgentEnable },
                    { Name: "ColAgentDisable", Value: rs.Total.AgentDisable },
                    { Name: "ColMemberEnable", Value: rs.Total.MemberEnable },
                    { Name: "ColMemberDisable", Value: rs.Total.MemberDisable }
                ]
            });
        }

        var gridOptions = {
            showTitle: true,
            titleInfo: {
                text: title,
                showExcelButton: true,
                exportExcelFunction: function () {
                    self.exportTotalCreditAccountByProduct();
                }
            },
            showFooter: true,
            footerRows: footerData,
            customizeHeader: true,
            groupHeaderRows: groupHeader,
            columns: columns,
            bodyRows: bodyRows,
            gridExpandHeight: 160,
            showDistinctRow: false,
            showHighlightRow: false,
            customPager: true,
            showPager: true,
            pagerOption: {
                itemsPerPage: cachedData.totalCreaditAccountByProduct.itemsPerPage,
                currentPage: cachedData.totalCreaditAccountByProduct.currentPage,
                totalItem: rs.ReportLength,
                itemsPerPageArray: cachedData.totalCreaditAccountByProduct.itemsPerPageArray,
                pagerSelectCallBack: function (page_index, itemsPerPage, jq) {
                    cachedData.totalCreaditAccountByProduct.itemsPerPage = itemsPerPage;
                    cachedData.totalCreaditAccountByProduct.currentPage = page_index;
                    self.getTotalCreditAccountByProduct();
                }
            }
        };

        return gridOptions;
    };

    self.renderTotalCreaditAccountByProduct = function (rs) {
        _table.reportContainer.removeClass("display-none");
        var gridOptions = self.getTotalCreditAccountByProductGridOptions(rs);
        _table.report.removeClass("display-none").fanexGrid(gridOptions);
    };

    getBetCountDetailByCustId = function (custId, userName) {
        cachedData.superUserName = userName;
        _table.reportContainer.addClass("display-none");
        self.reportDetailParams.CustID = parseInt(custId);
        self.reportDetailParams.SuperUserName = userName;
        var params = self.getDetailReportParams();

        AccReport.disableInput();
        AccReport.utility.CallAjax(this,
                                    window.rootUrl + "Misc/TotalCreditAccountByProduct/GetBetCountDetail",
                                    params,
                                    self.renderBetCountDetailByCustId,
                                    self.renderBetCountDetailByCustId,
                                    AccReport.enableInput);
    };

    backToMainReport = function () {
        _table.reportDetailContainer.addClass("display-none");
        _table.reportContainer.removeClass("display-none");
        _table.report.fanexGrid("resizeGrid");
    };

    self.renderBetCountDetailByCustId = function (rs) {
        var title = "<span class='link' onclick='backToMainReport()'>Main</span> > Bet Count Details for " +
         self.getProductNameBySport(self.reportDetailParams.SportType) + " in current Month - from " + firstDay + " to " + today +
         " (Super: " + cachedData.superUserName + ")";
        var rowNumber = cachedData.betCountDetail.currentPage * cachedData.betCountDetail.itemsPerPage;
        var bodyRows = [];
        var footerData = [];
        var columns = [
            { HeaderText: "#", Width: 60, Name: "ColNo", HeaderAlign: "center", CellAlign: "center", FormatNumber: FormatConstant.INT_NUMBER },
            { HeaderText: "Member", Width: 150, Name: "ColMember", HeaderAlign: "center", CellAlign: "left" },
            { HeaderText: "Status", Width: 150, Name: "ColStatus", HeaderAlign: "center", CellAlign: "center" },
            { HeaderText: "Settled Bet Count", Width: 220, Name: "ColSettledBetCount", HeaderAlign: "center", CellAlign: "right", FormatNumber: FormatConstant.INT_NUMBER },
            { HeaderText: "Running Bet Count", Width: 220, Name: "ColRunningBetCount", HeaderAlign: "center", CellAlign: "right", FormatNumber: FormatConstant.INT_NUMBER }
        ];

        reportLength = rs.ReportRows.length;

        if (rs && reportLength > 0) {
            for (var i = 0; i < reportLength; i++) {
                var currentRow = rs.ReportRows[i];

                var currentRowData = [
                { Name: "ColNo", Value: rowNumber + 1 },
                { Name: "ColMember", Value: currentRow.UserName },
                { Name: "ColStatus", Value: currentRow.IsDisable ? "Disabled" : "Enabled" },
                { Name: "ColSettledBetCount", Value: currentRow.SettledBetCount },
                { Name: "ColRunningBetCount", Value: currentRow.RunningBetCount }
            ];

                bodyRows.push({
                    Columns: currentRowData
                });

                rowNumber++;
            }

            footerData.push({
                Columns: [
                    { Name: "ColNo", Value: "Total", MergeColNumber: 3 },
                    { Name: "ColSettledBetCount", Value: rs.Total.SettledBetCount },
                    { Name: "ColRunningBetCount", Value: rs.Total.RunningBetCount }
                ]
            });
        }

        var gridOptions = {
            showTitle: true,
            titleInfo: {
                text: title,
                showExcelButton: true,
                exportExcelFunction: function () {
                    self.exportBetCountDetail();
                }
            },
            showFooter: true,
            footerRows: footerData,
            columns: columns,
            bodyRows: bodyRows,
            gridExpandHeight: 160,
            showDistinctRow: false,
            showHighlightRow: false,
            customPager: true,
            showPager: true,
            pagerOption: {
                itemsPerPage: cachedData.betCountDetail.itemsPerPage,
                currentPage: cachedData.betCountDetail.currentPage,
                totalItem: rs.ReportLength,
                itemsPerPageArray: cachedData.betCountDetail.itemsPerPageArray,
                pagerSelectCallBack: function (page_index, itemsPerPage, jq) {
                    cachedData.betCountDetail.itemsPerPage = itemsPerPage;
                    cachedData.betCountDetail.currentPage = page_index;
                    getBetCountDetailByCustId(self.reportDetailParams.CustID);
                }
            }
        };

        _table.reportDetailContainer.removeClass("display-none");
        _table.reportDetail.removeClass("display-none").fanexGrid(gridOptions);
    };

    self.exportTotalCreditAccountByProduct = function () {
        AccReport.downloadFile(window.rootUrl + "Misc/TotalCreditAccountByProduct/ExportTotalCreditAccountByProduct", self.reportParams);
    };

    self.exportBetCountDetail = function () {
        AccReport.downloadFile(window.rootUrl + "Misc/TotalCreditAccountByProduct/ExportBetCountDetail", self.reportDetailParams);
    };

    self.getProductNameBySport = function (sportId) {
        var productName = null;

        switch (parseInt(sportId)) {
            case productEnums.Casino:
                productName = "Casino";
                break;
            case productEnums.Racing:
                productName = "Racing";
                break;
            case productEnums.Bingo:
                productName = "Bingo";
                break;
            case productEnums.NumberGame:
                productName = "Number Game";
                break;
            case productEnums.LiveCasiono:
                productName = "Live Casino";
                break;
            case productEnums.VirtualSport:
                productName = "Virtual Sports";
                break;
            case productEnums.Keno:
                productName = "RNG Keno";
                break;
            case productEnums.ColossusBets:
                productName = "Colossus Bets";
                break;
            case productEnums.AllBet:
                productName = "Allbet";
                break;
            case productEnums.MetricGamingSuperLive:
                productName = "Metric Gaming - Superlive";
                break;
            case productEnums.MacauGames:
                productName = "Macau Games";
                break;
            case productEnums.NG2:
                productName = "NG2";
                break;
            case productEnums.Lottery:
                productName = "Lottery";
                break;
            case productEnums.Lucky3:
                productName = "Lucky 3";
                break;
            case productEnums.Bullfighting:
                productName = "Bullfighting";
                break;
            case productEnums.GoldDeluxe:
                productName = "Gold Deluxe";
                break;
            case productEnums.PockerGames:
                productName = "Poker Games";
                break;
            case productEnums.VirtualGames:
                productName = "Virtual Games";
                break;
            case productEnums.SAGaming:
                productName = "SA Gaming";
                break;
            case productEnums.SexyGaming:
                productName = "SexyGaming";
                break;
            case productEnums.SabaClub:
                productName = "Saba.club";
                break;
            case productEnums.TableGames:
                productName = "Table Games";
                break;
            default:
                productName = "Unknown";
                break;
        }

        return productName;
    };

    self.getTitleByStatus = function () {
        var status = '';
        if (_table.ddlStatus.val() === "All") {
            status = "Enabled/Disabled";
        }
        else if (_table.ddlStatus.val() === "Enabled") {
            status = "Enabled";
        }
        else {
            status = "Disabled";
        }

        return status;
    };
};

$(document).ready(function () {
    $("#notemessage").tipsy(
        {
            html: true,
            opacity: 0.9,
            gravity: 'ne'
        }
    );

    var totalCreditAccountByProduct = new Fanex.AccountingReports.Misc.TotalCreditAccountByProduct();
    totalCreditAccountByProduct.initPage();
});