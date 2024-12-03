Namespace.Register("Fanex.AccountingReports.Balance.DirectBalance");
Fanex.AccountingReports.Balance.DirectBalance = function () {
    var self = this;
    var _tables = {
        reportContainer: null,
        report: null,
        BalanceType: null
    };

    var _params = {
        directBalanceReport: {
            RoleId: null,
            Username: null,
            Currency: null,
            BalanceType: null,
            ExclTestCurr: false
        }
    };

    self.isReDirect = true;

    self.InitPage = function () {
        cachedDOMElements();
        getCurrencies();
        bindingEvent();
    };
    var cachedDOMElements = function () {
        _tables.reportContainer = $("#directBalanceGrid");
        _tables.report = $("#directBalanceMainGrid");
        _tables.BalanceType = $("#BalanceType");
    };

    var getCurrencies = function () {
        $.ajax({
            type: "POST",
            url: window.rootUrl + "Balance/DirectBalance/GetCurrencies",
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            data: null,
            beforeSend: function () {
                AccReport.disableInput();
            },
            success: function (rs) {
                var dlCurrency = $("#ddlCurrency");
                dlCurrency.append('<option value="0">All</option>');
                for (var i = 0; i < rs.length; i++) {
                    var currency = rs[i];
                    dlCurrency.append('<option value="' + currency.Id + '">' + currency.Name + '</option>');
                }
            },
            error: function (event, jqXHR, ajaxSettings) {
                self.renderCurrenciesDropdown([]);
            },
            complete: function () {
                AccReport.enableInput();
            }
        });
    };

    self.BackDirectBalanceDownline = function (customerType) {
        if (customerType == "Agent") {
            _tables.reportContainer.removeClass("display-none");
            //self.getDirectBalance(2, "Master Balance", "Master", $('#btnDirectMaster'));
        }
        else if (customerType == "Member") {
            self.getDirectBalance(3, "Agent Balance", "Agent", $('#btnDirectAgent'));
        }
    };

    self.showReport = function () {
        _tables.reportContainer.removeClass("display-none");
    };
    self.hideReport = function () {
        _tables.reportContainer.addClass("display-none");
    };

    self.RenderGrid = function (customerType, title, bodyData, footerData) {
        var columns = [];
        if (customerType != "Member") {
            columns = [
					{ HeaderText: "No", Width: 60, Name: "Number", CellAlign: "center" },
					{
					    HeaderText: customerType, Width: 150, Name: "CustomerType", CustomCellRender: function (currentCell) {
					        return '<div class="link" onclick="ViewDownline(\'' + customerType + '\',\'' + currentCell.Value.CustId + '\',' + _params.directBalanceReport.BalanceType + ')">' + currentCell.Value.Username + '</div>';
					    }
					},
					{ HeaderText: "Currency", Width: 80, Name: "Currency", CellAlign: "center" },
					{ HeaderText: "Balance", Width: 150, Name: "Balance", CellAlign: "right", HeaderAlign: "center" },
                    { HeaderText: "Base (RM)", Width: 150, Name: "BaseRM", CellAlign: "right", HeaderAlign: "center" },
                    { HeaderText: "Reward Receivable", Width: 150, Name: "Reward", CellAlign: "right", HeaderAlign: "center" },
                    { HeaderText: "Reward Receivable (RM)", Width: 150, Name: "RewardRM", CellAlign: "right", HeaderAlign: "center" }
            ];
        }
        else {
            columns = [
					{ HeaderText: "No", Width: 60, Name: "Number", CellAlign: "center" },
					{ HeaderText: customerType, Width: 150, Name: "CustomerType" },
					{ HeaderText: "Currency", Width: 80, Name: "Currency", CellAlign: "center" },
					{ HeaderText: "Balance", Width: 150, Name: "Balance", CellAlign: "right", HeaderAlign: "center" },
                    { HeaderText: "Base (RM)", Width: 150, Name: "BaseRM", CellAlign: "right", HeaderAlign: "center" },
                    { HeaderText: "Reward Receivable", Width: 150, Name: "Reward", CellAlign: "right", HeaderAlign: "center" },
                    { HeaderText: "Reward Receivable (RM)", Width: 150, Name: "RewardRM", CellAlign: "right", HeaderAlign: "center" }
            ];
        }

        var gridOption = {
            customizeHeader: true,
            groupHeaderRows: [{
                Columns: [{ HeaderText: "No", Name: "Number", MergeRowNumber: 2 },
                { HeaderText: customerType, Name: "CustomerType", MergeRowNumber: 2 },
                { HeaderText: "Currency", Name: "Currency", MergeRowNumber: 2 },
                { HeaderText: "Main Wallet", Name: "Balance", MergeColNumber: 2 },
                { HeaderText: "Reward Wallet", Name: "Reward", MergeColNumber: 2 }]
            }],
            columns: columns,
            showFooter: true,
            showTitle: true,
            titleInfo:
				{
				    text: title,
				    showExcelButton: true,
				    exportExcelFunction: function () {
				        self.exportReport();
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

    self.getDirectBalance = function () {
        AccReport.disableInput();
        var customerType = null;
        var title = null;
        $('#directBalanceDownlineGrid').addClass("display-none");
        var roleId = $('#ddlReport').val();
        if (roleId == 2) {
            customerType = "Master";
            title = "Master Balance";
        }
        else if (roleId == 3) {
            customerType = "Agent";
            title = "Agent Balance";
        }
        else if (roleId == 4) {
            customerType = "Member";
            title = "Member Balance";
        }
        var username = $('#txtUsername').val();
        var Currency = $('#Currency').val();
        if (username == "All") {
            _params.directBalanceReport.Username = '';
        }
        else {
            _params.directBalanceReport.Username = username;
        }
        _params.directBalanceReport.RoleId = roleId;
        _params.directBalanceReport.BalanceType = _tables.BalanceType.find("input[name='rdBalance']:checked").val();
        _params.directBalanceReport.Currency = $('#ddlCurrency').val();
        _params.directBalanceReport.ExclTestCurr = $("#checkbox-excludetest").is(":checked");
        if (_params.directBalanceReport.Currency == 0)
            _params.directBalanceReport.Currency = '';
        $.ajax({
            type: "POST",
            url: window.rootUrl + "Balance/DirectBalance/GetDirectBalance",
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            data: JSON.stringify({
                RoleID: _params.directBalanceReport.RoleId,
                Username: _params.directBalanceReport.Username,
                BalanceType: _params.directBalanceReport.BalanceType,
                Currency: _params.directBalanceReport.Currency,
                ExclTestCurr: _params.directBalanceReport.ExclTestCurr
            })
				,
            beforeSend: function () {
                AccReport.disableInput();
            },
            success: function (rs) {
                if (rs && rs.DirectBalanceList) {
                    //// Buid Grid Data
                    var bodyData = [];
                    var customerLength = rs.DirectBalanceList.length;
                    for (var customerIndex = 0; customerIndex < customerLength; customerIndex++) {
                        var currentCustomer = rs.DirectBalanceList[customerIndex];
                        if (roleId < 4) {
                            bodyData.push(
							{
							    Columns: [
										{ Name: "Number", Value: customerIndex + 1 },
										{ Name: "CustomerType", Value: currentCustomer },
										{ Name: "Currency", Value: currentCustomer.Currency },
										{ Name: "Balance", Value: currentCustomer.Balance },
                                        { Name: "BaseRM", Value: currentCustomer.BaseBalance },
                                        { Name: "Reward", Value: currentCustomer.Reward },
                                        { Name: "RewardRM", Value: currentCustomer.RewardRM }
							    ]
							});
                        }
                        else {
                            bodyData.push(
							{
							    Columns: [
										{ Name: "Number", Value: customerIndex + 1 },
										{ Name: "CustomerType", Value: currentCustomer.Username },
										{ Name: "Currency", Value: currentCustomer.Currency },
										{ Name: "Balance", Value: currentCustomer.Balance },
                                        { Name: "BaseRM", Value: currentCustomer.BaseBalance },
                                        { Name: "Reward", Value: currentCustomer.Reward },
                                        { Name: "RewardRM", Value: currentCustomer.RewardRM }
							    ]
							});
                        }
                    }
                    if (rs.DirectBalanceList.length > 0) {
                        var footerData = [
						{
						    Columns: [
										{ Name: "Number", Value: "Total", MergeColNumber: 4 },
                                        { Name: "BaseRM", Value: rs.BaseBalanceTotal },
                                        { Name: "Reward", Value: "" },
                                        { Name: "RewardRM", Value: rs.BaseRewardTotal }
						    ]
						}]
                    }
                    self.RenderGrid(customerType, title, bodyData, footerData);
                }
                else {
                    self.RenderGrid(customerType, title, [], []);
                }
            },
            complete: function () {
                AccReport.enableInput();
            },
            error: function () {
            }
        });
    };

    self.exportReport = function () {
        var exportParams = {
            RoleID: _params.directBalanceReport.RoleId,
            Username: _params.directBalanceReport.Username,
            BalanceType: _params.directBalanceReport.BalanceType,
            Currency: _params.directBalanceReport.Currency
        };
        AccReport.downloadFile(window.rootUrl + "Balance/DirectBalance/ExportReport", exportParams);
    };

    function bindingEvent() {
        $("#btnGenerate").click(function () {
            self.getDirectBalance();
        });
        $("#txtUsername").focus(function () {
            $("#txtUsername").val('');
        });
        $("#txtUsername").blur(function () {
            var username = $("#txtUsername").val();
            if (username == '') {
                $("#txtUsername").val('All');
            }
        });
    }
};