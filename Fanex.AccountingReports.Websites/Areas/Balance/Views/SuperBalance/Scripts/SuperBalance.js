Namespace.Register("Fanex.AccountingReports.Balance.SuperBalance");
Fanex.AccountingReports.Balance.SuperBalance = function () {
    var self = this;
    var _tables = {
        reportContainer: null,
        report: null,
        reportViewTotalContainer: null,
        reportViewTotal: null,
        BalanceType: null,
        ddlCurrency: null,
        ddlLetter: null,
        txtUsername: null,
        chkViewtotalonly: null
    };

    var _params = {
        superBalanceReport: {
            RoleId: null,
            CustomerType: null,
            Username: null,
            FirstLetter: null,
            Currency: null,
            ViewTotalOny: null,
            BalanceType: null
        }
    };

    var _balanceUsername = {
        superUsername: null,
        masterUsername: null,
        agentUsername: null,
        memberUsername: null
    };

    var _balanceUserId = {
        superId: null,
        masterId: null,
        agentId: null
    };
    var _cachedData = {
        isDefaultTextboxUserName: true,
        currentTextboxUserName: viewResources.Search_All,
        defaultTextboxUserName: viewResources.Search_All
    };

    var cachedDOMElements = function () {
        _tables.reportContainer = $("#superBalanceGrid");
        _tables.report = $("#superBalanceMainGrid");
        _tables.reportViewTotalContainer = $("#superBalanceViewTotalContainerGrid");
        _tables.reportViewTotal = $("#superBalanceViewTotalGrid");
        _tables.BalanceType = $("#BalanceType");
        _tables.ddlCurrency = $("#ddlCurrency");
        _tables.ddlLetter = $("#ddlFirstLetter");
        _tables.txtUsername = $("#txtUsername");
        _tables.chkViewtotalonly = $("#chkViewTotalOnly");
    };

    var getCurrencies = function () {
        AccReport.disableInput();

        AccReport.utility.CallAjax(self,
                window.rootUrl + "Balance/SuperBalance/GetCurrencies",
                null,
                self.renderCurrency,
                self.renderCurrency,
                AccReport.enableInput);
    };

    self.renderCurrency = function (rs) {
        _tables.ddlCurrency.append('<option value="0">All</option>');
        var rsLength = rs.length;
        for (var i = 0; i < rsLength; i++) {
            var currency = rs[i];
            _tables.ddlCurrency.append('<option value="' + currency.Id + '">' + currency.Name + '</option>');
        }
    };

    var getListAlphabet = function () {
        AccReport.disableInput();

        AccReport.utility.CallAjax(self,
            window.rootUrl + "Balance/SuperBalance/GetListAlphabet",
            null,
            self.renderFirstLetterList,
            self.renderFirstLetterList,
            AccReport.enableInput);
    };

    self.renderFirstLetterList = function (rs) {
        for (var i = 0; i < rs.length; i++) {
            var alphabet = rs[i];
            if (alphabet == '0-9') {
                _tables.ddlLetter.append('<option value="[' + alphabet + ']">' + alphabet + '</option>');
            }
            else {
                _tables.ddlLetter.append('<option value="' + alphabet + '">' + alphabet + '</option>');
            }            
        }
    };

    self.renderData = function (rs) {
        if (_params.superBalanceReport.ViewTotalOny) {
            self.renderViewTotalOnlyGrid(rs);
        }
        else {
            self.renderGrid(rs);
        }
    }

    self.renderGrid = function (rs) {
        var columns = [];
        var bodyData = [];
        var footerData = [];
        var customerType = '';
        var title = '';

        _tables.reportContainer.addClass("display-none");
        _tables.reportViewTotalContainer.addClass("display-none");
        switch (_params.superBalanceReport.RoleId) {
            case 1:
                customerType = 'Member';
                title = '<span class="link" onclick="viewToMainReport()">Main</span> > <span class="link" onclick="viewToMasterReport()"> ' + _balanceUsername.superUsername + ' </span> > <span class="link" onclick="viewToAgentReport()"> ' + _balanceUsername.masterUsername + ' </span> > <span> ' + _balanceUsername.agentUsername + '</span>';
                _tables.reportContainer = $("#superBalanceMemberContainerGrid");
                _tables.report = $("#superBalanceMemberGrid");
                break;
            case 2:
                customerType = 'Agent';
                title = '<span class="link" onclick="viewToMainReport()">Main</span> > <span class="link" onclick="viewToMasterReport()"> ' + _balanceUsername.superUsername + ' </span> > <span> ' + _balanceUsername.masterUsername + ' </span>';
                _tables.reportContainer = $("#superBalanceAgentContainerGrid");
                _tables.report = $("#superBalanceAgentGrid");
                break;
            case 3:
                customerType = 'Master';
                title = '<span class="link" onclick="viewToMainReport()">Main</span> > <span> ' + _balanceUsername.superUsername + ' </span>';
                _tables.reportContainer = $("#superBalanceMasterContainerGrid");
                _tables.report = $("#superBalanceMasterGrid");
                break;
            case 4:
                customerType = 'Super';
                title = 'Main';
                _tables.reportContainer = $("#superBalanceGrid");
                _tables.report = $("#superBalanceMainGrid");
                break;
        }

        _tables.reportContainer.removeClass("display-none");

        if (customerType != 'Member') {
            columns = [
                { HeaderText: "No", Width: 60, Name: "Number", HeaderAlign: "center", CellAlign: "center" },
                { HeaderText: customerType, Width: 150, Name: "CustomerType", HeaderAlign: "center", CellAlign: "left", CustomCellRender: function (currentCell)
                { return '<div class="link" onclick="viewSuperBalanceDownline(\'' + customerType + '\',' + currentCell.Value.CustId + ',\'' + currentCell.Value.Username + '\')">' + currentCell.Value.Username + '</div>'; }
                },
                { HeaderText: "Currency", Width: 80, Name: "Currency", HeaderAlign: "center", CellAlign: "center" },
                { HeaderText: "Balance", Width: 150, Name: "Balance", HeaderAlign: "center", CellAlign: "right", FormatNumber: FormatConstant.TWO_DECIMAL_NUMBER },
                { HeaderText: "Base (RM)", Width: 150, Name: "BaseRM", HeaderAlign: "center", CellAlign: "right", FormatNumber: FormatConstant.TWO_DECIMAL_NUMBER }
            ];
        }
        else {
            columns = [
                { HeaderText: "No", Width: 60, Name: "Number", HeaderAlign: "center", CellAlign: "center" },
                { HeaderText: customerType, Width: 150, Name: "CustomerType", HeaderAlign: "center", CellAlign: "left" },
                { HeaderText: "Currency", Width: 80, Name: "Currency", HeaderAlign: "center", CellAlign: "center" },
                { HeaderText: "Balance", Width: 150, Name: "Balance", HeaderAlign: "center", CellAlign: "right", FormatNumber: FormatConstant.TWO_DECIMAL_NUMBER },
                { HeaderText: "Base (RM)", Width: 150, Name: "BaseRM", HeaderAlign: "center", CellAlign: "right", FormatNumber: FormatConstant.TWO_DECIMAL_NUMBER }
            ];
        }

        if (rs && rs.SuperBalanceList) {
            var customerLength = rs.SuperBalanceList.length;

            for (var customerIndex = 0; customerIndex < customerLength; customerIndex++) {
                var currentCustomer = rs.SuperBalanceList[customerIndex];

                if (_params.superBalanceReport.RoleId > 1) {
                    bodyData.push({
                        Columns: [
                                { Name: "Number", Value: customerIndex + 1 },
                                { Name: "CustomerType", Value: currentCustomer },
                                { Name: "Currency", Value: currentCustomer.Currency },
                                { Name: "Balance", Value: currentCustomer.Balance },
                                { Name: "BaseRM", Value: currentCustomer.BaseBalance }
                            ]
                    });
                }
                else {
                    bodyData.push({
                        Columns: [
                                { Name: "Number", Value: customerIndex + 1 },
                                { Name: "CustomerType", Value: currentCustomer.Username },
                                { Name: "Currency", Value: currentCustomer.Currency },
                                { Name: "Balance", Value: currentCustomer.Balance },
                                { Name: "BaseRM", Value: currentCustomer.BaseBalance }
                            ]
                    });
                }
            }
            if (rs.SuperBalanceList.length > 0) {
                footerData = [
                {
                    Columns: [
				                { Name: "Number", Value: "Total", MergeColNumber: 4 },
				                { Name: "BaseRM", Value: rs.BaseBalanceTotal.BaseBalance }
			                ]
                }];
            }
        }

        var gridOptions = {
            columns: columns,
            showFooter: true,
            showTitle: true,
            titleInfo: {
                text: title,
                showExcelButton: true,
                exportExcelFunction: function () {
                    self.Export();
                }
            },
            bodyRows: bodyData,
            footerRows: footerData,
            gridExpandHeight: 220,
            showPager: false,
            pagerOption: {
                itemsPerPage: 50,
                currentPage: 0,
                itemsPerPageArray: [50, 100, 200, 500],
                totalItem: bodyData.length
            }
        };
        _tables.report.fanexGrid(gridOptions);
    };

    self.renderViewTotalOnlyGrid = function (rs) {
        var columns = [];
        var bodyData = [];
        var footerData = [];
        var balanceType = '';
        var title = '';
        _tables.reportContainer.addClass("display-none");
        _tables.reportViewTotalContainer = $("#superBalanceViewTotalContainerGrid");
        _tables.reportViewTotal = $("#superBalanceViewTotalGrid");


        if (_params.superBalanceReport.BalanceType == 1) {
            balanceType = 'Today';
        }
        else {
            balanceType = 'Yesterday';
        }

        title = 'Super ' + balanceType + ' Balance';

        if (rs.HasData) {
            bodyData = [
            {
                CssClass: "totalRow",
                Columns: [{ Name: "Number", Value: "Total", MergeColNumber: 4 },
                    { Name: "BaseRM", Value: rs.BaseBalanceTotal.BaseBalance, CellAlign: "right"}]
            }];
        }

        columns = [{ HeaderText: "No", Width: 60, Name: "Number", HeaderAlign: "center", CellAlign: "center", CssClass: "display-none" },
            { HeaderText: "CustomerType", Width: 150, Name: "CustomerType", HeaderAlign: "center", CellAlign: "center", CssClass: "display-none" },
            { HeaderText: "Currency", Width: 80, Name: "Currency", HeaderAlign: "center", CellAlign: "center", CssClass: "display-none" },
            { HeaderText: "Balance", Width: 150, Name: "Balance", HeaderAlign: "center", CellAlign: "right", FormatNumber: FormatConstant.TWO_DECIMAL_NUMBER, CssClass: "display-none" },
            { HeaderText: "Base (RM)", Width: 150, Name: "BaseRM", HeaderAlign: "center", CellAlign: "right", FormatNumber: FormatConstant.TWO_DECIMAL_NUMBER, CssClass: "display-none"}];
        var gridOptions = {
            columns: columns,
            showFooter: true,
            showTitle: true,
            titleInfo: {
                text: title,
                showExcelButton: true,
                exportExcelFunction: function () {
                    self.Export();
                }
            },
            bodyRows: bodyData,
            footerRows: footerData,
            gridExpandHeight: 220,
            showPager: false,
            pagerOption: {
                itemsPerPage: 50,
                currentPage: 0,
                itemsPerPageArray: [50, 100, 200, 500],
                totalItem: bodyData.length
            }
        };

        _tables.reportViewTotal.fanexGrid(gridOptions);
    }

    self.getSuperBalance = function () {
        AccReport.disableInput();
        _params.superBalanceReport.RoleId = 4;
        var balancetype = _tables.BalanceType.find("input[name='rdBalance']:checked").val();
        var username = _tables.txtUsername.val();

        if (username == _cachedData.defaultTextboxUserName) {
            _params.superBalanceReport.Username = '';
        }
        else {
            _params.superBalanceReport.Username = username;
        }

        var firstletter = _tables.ddlLetter.val();
        if (firstletter == 'All') {
            _params.superBalanceReport.FirstLetter = '';
        } else {
            _params.superBalanceReport.FirstLetter = firstletter;
        }

        _params.superBalanceReport.Currency = _tables.ddlCurrency.val();

        if (_params.superBalanceReport.Currency == 0) {
            _params.superBalanceReport.Currency = '';
        }

        var viewtotalonly = getViewTotalOnlyAttr();

        _params.superBalanceReport.BalanceType = balancetype;
        _params.superBalanceReport.ViewTotalOny = viewtotalonly;

        var data = {
            BalanceType: _params.superBalanceReport.BalanceType,
            Username: _params.superBalanceReport.Username,
            FirstLetter: _params.superBalanceReport.FirstLetter,
            Currency: _params.superBalanceReport.Currency,
            IsViewTotalOnly: _params.superBalanceReport.ViewTotalOny
        };

        AccReport.disableInput();
        AccReport.utility.CallAjax(this,
                                    window.rootUrl + "Balance/SuperBalance/GetSuperBalance",
                                    data,
                                    self.renderData,
                                    self.renderData,
                                    AccReport.enableInput);
    };

    self.getSuperBalanceDownline = function (customerType, custId, username) {
        AccReport.disableInput();
        var data = null;
        var url = null;

        var balanceType = _params.superBalanceReport.BalanceType;
        _params.superBalanceReport.CustomerType = customerType;

        switch (customerType) {
            case 'Super':
                data = {
                    BalanceType: balanceType,
                    CustomerType: customerType,
                    SuperId: custId
                };
                url = window.rootUrl + "Balance/SuperBalance/GetSuperBalanceSuperDownline";
                _params.superBalanceReport.RoleId = 3;
                _balanceUsername.superUsername = username;
                _balanceUserId.superId = custId;
                break;
            case 'Master':
                data = {
                    BalanceType: balanceType,
                    CustomerType: customerType,
                    MasterId: custId
                };
                url = window.rootUrl + "Balance/SuperBalance/GetSuperBalanceMasterDownline";
                _params.superBalanceReport.RoleId = 2;
                _balanceUsername.masterUsername = username;
                _balanceUserId.masterId = custId;
                break;
            case 'Agent':
                data = {
                    BalanceType: balanceType,
                    CustomerType: customerType,
                    AgentId: custId
                };
                url = window.rootUrl + "Balance/SuperBalance/GetSuperBalanceAgentDownline";
                _params.superBalanceReport.RoleId = 1;
                _balanceUsername.agentUsername = username;
                _balanceUserId.agentId = custId;
                break;
        }

        AccReport.utility.CallAjax(this,
            url,
            data,
            self.renderGrid,
            self.renderGrid,
            AccReport.enableInput);
    };

    self.Export = function () {
        switch (_params.superBalanceReport.RoleId) {
            case 1:
                self.ExportMemberBalanceReport();
                break;
            case 2:
                self.ExportAgentBalanceReport();
                break;
            case 3:
                self.ExportMasterBalanceReport();
                break;
            case 4:
                self.ExportMainBalanceReport();
                break;
        }
    };

    self.ExportMainBalanceReport = function () {
        var exportParams = {
            BalanceType: _params.superBalanceReport.BalanceType,
            Username: _params.superBalanceReport.Username,
            FirstLetter: _params.superBalanceReport.FirstLetter,
            Currency: _params.superBalanceReport.Currency,
            IsViewTotalOnly: _params.superBalanceReport.ViewTotalOny
        };

        AccReport.downloadFile(window.rootUrl + "Balance/SuperBalance/ExportReport", exportParams);
    };

    self.ExportMasterBalanceReport = function () {
        var exportParams = {
            BalanceType: _params.superBalanceReport.BalanceType,
            CustomerType: _params.superBalanceReport.CustomerType,
            SuperId: _balanceUserId.superId
        };

        AccReport.downloadFile(window.rootUrl + "Balance/SuperBalance/ExportMasterBalance", exportParams);
    };

    self.ExportAgentBalanceReport = function () {
        var exportParams = {
            BalanceType: _params.superBalanceReport.BalanceType,
            CustomerType: _params.superBalanceReport.CustomerType,
            MasterId: _balanceUserId.masterId
        };

        AccReport.downloadFile(window.rootUrl + "Balance/SuperBalance/ExportAgentBalance", exportParams);
    };

    self.ExportMemberBalanceReport = function () {
        var exportParams = {
            BalanceType: _params.superBalanceReport.BalanceType,
            CustomerType: _params.superBalanceReport.CustomerType,
            AgentId: _balanceUserId.agentId
        };

        AccReport.downloadFile(window.rootUrl + "Balance/SuperBalance/ExportMemberBalance", exportParams);
    };

    viewSuperBalanceDownline = function (customerType, custId, username) {
        self.getSuperBalanceDownline(customerType, custId, username);
    };

    viewToMainReport = function () {
        _tables.reportContainer.addClass("display-none");
        _tables.reportContainer = $("#superBalanceGrid");
        _tables.reportContainer.removeClass("display-none");
        _tables.report = $("#superBalanceMainGrid");
        _params.superBalanceReport.RoleId = 4;
        _tables.report.fanexGrid("resizeGrid");
    };

    viewToMasterReport = function () {
        _tables.reportContainer.addClass("display-none");
        _tables.reportContainer = $("#superBalanceMasterContainerGrid");
        _tables.reportContainer.removeClass("display-none");
        _tables.report = $("#superBalanceMasterGrid");
        _params.superBalanceReport.RoleId = 3;
        _tables.report.fanexGrid("resizeGrid");
    };

    viewToAgentReport = function () {
        _tables.reportContainer.addClass("display-none");
        _tables.reportContainer = $("#superBalanceAgentContainerGrid");
        _tables.reportContainer.removeClass("display-none");
        _tables.report = $("#superBalanceAgentGrid");
        _params.superBalanceReport.RoleId = 2;
        _tables.report.fanexGrid("resizeGrid");
    };

    self.InitPage = function () {
        cachedDOMElements();
        getCurrencies();
        getListAlphabet();
        bindingEvents();
    };

    function bindingEvents() {
        $("#btnGenerate").click(function () {
            self.getSuperBalance();
        });

        _tables.txtUsername.unbind("blur");
        _tables.txtUsername.blur(function () {
            var textbox = $(this);

            var inputValue = '';
            var newName = textbox.val();
            var oldName = _cachedData.currentTextboxUserName;
            _cachedData.isDefaultTextboxUserName = false;
            textbox.removeClass("defaultSuggestionBox");
            if (oldName != '' &&
                newName != '' &&
                newName != oldName) {
                inputValue = newName;
            }
            else if (oldName != '' && newName == '') {
                inputValue = oldName;
                _cachedData.isDefaultTextboxUserName = true;
                textbox.addClass("defaultSuggestionBox");
            }
            else if (newName != '') {
                inputValue = newName;
            }
            else {
                _cachedData.isDefaultTextboxUserName = true;
                textbox.addClass("defaultSuggestionBox");
                inputValue = _cachedData.defaultTextboxUserName;
            }

            textbox.val(inputValue);
        });

        _tables.txtUsername.unbind("focus");
        _tables.txtUsername.focus(function () {
            var textbox = $(this);
            textbox.removeClass("defaultSuggestionBox");
            if (_cachedData.isDefaultTextboxUserName) {
                textbox.val('');
            }
        });
    };

    function getViewTotalOnlyAttr() {
        return _tables.chkViewtotalonly.is(':checked');
    }
};

