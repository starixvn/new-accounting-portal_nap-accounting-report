Namespace.Register("Fanex.AccountingReports.Balance.DirectBalanceDownline");
Fanex.AccountingReports.Balance.DirectBalanceDownline = function () {
    var self = this;

    var _tables = {
        reportContainer: null,
        report: null
    };

    self.InitPage = function () {
        cachedDOMElements();
    };
    var cachedDOMElements = function () {
        _tables.reportContainer = $("#directBalanceDownlineGrid");
        _tables.report = $("#directBalanceDownlineMainGrid");
    };
    self._params = {
        masterId: null,
        agentId: null,
        exclTestCurr: false,
    };

    self.RenderGrid = function (customerType, title, bodyData, footerData) {
        var columns = [];
        if (customerType != "Member") {
            columns = [
                    { HeaderText: "No", Width: 60, Name: "Number", CellAlign: "center" },
                    {
                        HeaderText: customerType, Width: 150, Name: "CustomerType", CustomCellRender: function (currentCell) {
                            return '<div class="link" onclick="ViewDownline(\'' + customerType + '\',\'' + currentCell.Value.CustId + '\')">' + currentCell.Value.Username + '</div>';
                        }
                    },
                    { HeaderText: "Currency", Width: 80, Name: "Currency", CellAlign: "center" },
                    { HeaderText: "Balance", Width: 150, Name: "Balance", CellAlign: "right", HeaderAlign: "center" },
                    { HeaderText: "Base (RM)", Width: 150, Name: "BaseRM", CellAlign: "right", HeaderAlign: "center" }
            ];
        }
        else {
            columns = [
                    { HeaderText: "No", Width: 60, Name: "Number", CellAlign: "center" },
                    { HeaderText: customerType, Width: 150, Name: "CustomerType" },
                    { HeaderText: "Currency", Width: 80, Name: "Currency", CellAlign: "center" },
                    { HeaderText: "Balance", Width: 150, Name: "Balance", CellAlign: "right", HeaderAlign: "center" },
                    { HeaderText: "Base (RM)", Width: 150, Name: "BaseRM", CellAlign: "right", HeaderAlign: "center" }
            ];
        }
        var gridOption = {
            columns: columns,
            showFooter: true,
            showTitle: true,
            titleInfo:
                {
                    text: title,
                    showExcelButton: true,
                    exportExcelFunction: function () {
                        if (customerType == "Agent") {
                            self.exportAgentBalance();
                        }
                        else if (customerType == "Member") {
                            self.exportMemberBalance();
                        }
                    },
                    showBackButton: true,
                    backFunction: function () {
                        BackDirectBalance(customerType);
                    }
                },
            bodyRows: bodyData,
            footerRows: footerData,
            gridExpandHeight: 200,
            showPager: false,
            pagerOption: {
                itemsPerPage: 50,
                currentPage: 0,
                itemsPerPageArray: [50, 100, 200, 500],
                totalItem: bodyData.length
            }
        };

        _tables.report.fanexGrid(gridOption);
    };

    self.ViewDirectBalanceMasterDownline = function (masterId, excludeTestData) {
        self._params.masterId = masterId;
        $.ajax({
            type: "POST",
            url: window.rootUrl + "Balance/DirectBalance/GetDirectBalanceMasterDownline",
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            data: JSON.stringify({
                MasterId: masterId
            })
                ,
            beforeSend: function () {
                AccReport.disableInput();
            },
            success: function (rs) {
                if (rs && rs.DirectBalanceList) {
                    //// Buid Grid Data
                    var bodyData = [];
                    var _title = "Agent Balance";
                    var customerLength = rs.DirectBalanceList.length;
                    for (var customerIndex = 0; customerIndex < customerLength; customerIndex++) {
                        var currentCustomer = rs.DirectBalanceList[customerIndex];
                        bodyData.push(
                            {
                                Columns: [
                                        { Name: "Number", Value: customerIndex + 1 },
                                        { Name: "CustomerType", Value: currentCustomer },
                                        { Name: "Currency", Value: currentCustomer.Currency },
                                        { Name: "Balance", Value: currentCustomer.Balance },
                                        { Name: "BaseRM", Value: currentCustomer.BaseBalance }
                                ]
                            });
                    }

                    if (rs.DirectBalanceList.length > 0) {
                        var footerData = [
                        {
                            Columns: [
                                        { Name: "Number", Value: "Total", MergeColNumber: 4 },
                                        { Name: "BaseRM", Value: rs.BaseBalanceTotal }
                            ]
                        }]
                        self.RenderGrid("Agent", _title, bodyData, footerData);
                    }
                    else {
                        self.RenderGrid("Agent", _title, bodyData);
                    }
                }
            },
            complete: function () {
                AccReport.enableInput();
            },
            error: function () {
            }
        });
    };

    self.ViewDirectBalanceAgentDownline = function (agentId) {
        self._params.agentId = agentId;
        $.ajax({
            type: "POST",
            url: window.rootUrl + "Balance/DirectBalance/GetDirectBalanceAgentDownline",
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            data: JSON.stringify({
                AgentId: agentId
            })
                ,
            beforeSend: function () {
                AccReport.disableInput();
            },
            success: function (rs) {
                if (rs && rs.DirectBalanceList) {
                    //// Buid Grid Data
                    var bodyData = [];
                    var _title = "Member Balance";
                    var customerLength = rs.DirectBalanceList.length;
                    for (var customerIndex = 0; customerIndex < customerLength; customerIndex++) {
                        var currentCustomer = rs.DirectBalanceList[customerIndex];
                        bodyData.push(
                            {
                                Columns: [
                                        { Name: "Number", Value: customerIndex + 1 },
                                        { Name: "CustomerType", Value: currentCustomer.Username },
                                        { Name: "Currency", Value: currentCustomer.Currency },
                                        { Name: "Balance", Value: currentCustomer.Balance },
                                        { Name: "BaseRM", Value: currentCustomer.BaseBalance }
                                ]
                            });
                    }
                    if (rs.DirectBalanceList.length > 0) {
                        var footerData = [
                        {
                            Columns: [
                                        { Name: "Number", Value: "Total", MergeColNumber: 4 },
                                        { Name: "BaseRM", Value: rs.BaseBalanceTotal }
                            ]
                        }]
                        self.RenderGrid("Member", _title, bodyData, footerData);
                    }
                    else {
                        self.RenderGrid("Member", _title, bodyData);
                    }
                }
            },
            complete: function () {
                AccReport.enableInput();
            },
            error: function () {
            }
        });
    };

    self.BackBalanceDownline = function (customerType) {
        self.ViewDirectBalanceMasterDownline(self._params.masterId);
        _tables.report.fanexGrid("resizeGrid");
        // _params.masterId = null;
    };

    self.exportAgentBalance = function () {
        var exportParams = {
            MasterId: self._params.masterId,
            customerType: "Agent"
        };

        AccReport.downloadFile(window.rootUrl + "Balance/DirectBalance/ExportAgentBalance", exportParams);
    };

    self.exportMemberBalance = function () {
        var exportParams = {
            AgentId: self._params.agentId,
            customerType: "Member"
        };
        AccReport.downloadFile(window.rootUrl + "Balance/DirectBalance/ExportMemberBalance", exportParams);
    };

    self.hideReport = function () {
        _tables.reportContainer.addClass("display-none");
    };

    self.showReport = function () {
        _tables.reportContainer.removeClass("display-none");
    };
};