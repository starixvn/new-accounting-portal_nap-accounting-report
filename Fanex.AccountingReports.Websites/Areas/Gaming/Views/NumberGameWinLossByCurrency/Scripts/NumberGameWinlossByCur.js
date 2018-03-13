Namespace.Register("Fanex.AccountingReports.Gaming.NumberGameWinlossByCur");
Fanex.AccountingReports.Gaming.NumberGameWinlossByCur = function () {
    var self = this;

    var _params = {
        paramReport: {
            SiteID: null,
            Bettype: null,
            Currency: null,
            FromDate: null,
            ToDate: null,
            IsLive: null,
            TrialData: false
        }
    };

    var _detailParams = {
        Currency: null,
        SiteID: null,
        Username: null,
        Bettype: null,
        IsLive: null,
        FromDate: null,
        ToDate: null,
        RoleID: 0,
        CustID: 0,
        PageSize: 50,
        PageIndex: 1,
        TrialData: false
    };

    var MainOnclick = '';
    var CurrencyOnclick = '';
    var Super = '';
    var SuperOnclick = '';
    var Master = '';
    var MasterOnclick = '';
    var Agent = '';
    var AgentOnclick = '';
    var Member = '';
    var Title = '';

    var SupperName = '';
    var MasterName = '';

    var _isMainPage = true;

    var _mainModelTmp = {
        param: {
            SiteID: null,
            Bettype: null,
            Currency: null,
            FromDate: null,
            ToDate: null,
            IsLive: null
        },
        model: null
    };
    var _superModelTmp = {
        currency: null,
        model: null
    };
    var _masterModelTmp = {
        superId: null,
        model: null
    };
    var _agentModelTmp = {
        masterId: null,
        model: null
    };
    var _memberModelTmp = {
        agentId: null,
        model: null
    };

    self.BackToMainReport = function () {
        $("#numberGameWLByDateMainGrid_CurrencyContainer").addClass("display-none");
        $("#numberGameWLByDateMainGrid_SuperContainer").addClass("display-none");
        $("#numberGameWLByDateMainGrid_MasterContainer").addClass("display-none");
        $("#numberGameWLByDateMainGrid_AgentContainer").addClass("display-none");
        $("#numberGameWLByDateMainGrid_MemberContainer").addClass("display-none");
        $("#numberGameWLByDateMainGridContainer").removeClass("display-none");
        _isMainPage = true;
    };
    self.BackToCurrencyReport = function () {
        $("#numberGameWLByDateMainGridContainer").addClass("display-none");
        $("#numberGameWLByDateMainGrid_SuperContainer").addClass("display-none");
        $("#numberGameWLByDateMainGrid_MasterContainer").addClass("display-none");
        $("#numberGameWLByDateMainGrid_AgentContainer").addClass("display-none");
        $("#numberGameWLByDateMainGrid_MemberContainer").addClass("display-none");
        $("#numberGameWLByDateMainGrid_CurrencyContainer").removeClass("display-none");
        _isMainPage = false;
        _detailParams.RoleID = 0;
        _detailParams.Username = null;
    };
    self.BackToSuperReport = function () {
        $("#numberGameWLByDateMainGridContainer").addClass("display-none");
        $("#numberGameWLByDateMainGrid_CurrencyContainer").addClass("display-none");
        $("#numberGameWLByDateMainGrid_MasterContainer").addClass("display-none");
        $("#numberGameWLByDateMainGrid_AgentContainer").addClass("display-none");
        $("#numberGameWLByDateMainGrid_MemberContainer").addClass("display-none");
        $("#numberGameWLByDateMainGrid_SuperContainer").removeClass("display-none");
        _isMainPage = false;
        _detailParams.RoleID = 3;
        _detailParams.Username = SupperName;

    };
    self.BackToMasterReport = function () {
        $("#numberGameWLByDateMainGridContainer").addClass("display-none");
        $("#numberGameWLByDateMainGrid_CurrencyContainer").addClass("display-none");
        $("#numberGameWLByDateMainGrid_SuperContainer").addClass("display-none");
        $("#numberGameWLByDateMainGrid_AgentContainer").addClass("display-none");
        $("#numberGameWLByDateMainGrid_MemberContainer").addClass("display-none");
        $("#numberGameWLByDateMainGrid_MasterContainer").removeClass("display-none");
        _isMainPage = false;
        _detailParams.RoleID = 2;
        _detailParams.Username = MasterName;
    };
    self.BackToAgentReport = function () {
        $("#numberGameWLByDateMainGridContainer").addClass("display-none");
        $("#numberGameWLByDateMainGrid_CurrencyContainer").addClass("display-none");
        $("#numberGameWLByDateMainGrid_SuperContainer").addClass("display-none");
        $("#numberGameWLByDateMainGrid_MasterContainer").addClass("display-none");
        $("#numberGameWLByDateMainGrid_MemberContainer").addClass("display-none");
        $("#numberGameWLByDateMainGrid_AgentContainer").removeClass("display-none");
        _isMainPage = false;
    };

    getLinkForTitle = function (Currency, CustID, RoleID, DetailName) {
        self.MainOnclick = "onclick=\"numberGameWLByCur.BackToMainReport();\"";
        self.CurrencyOnclick = "onclick=\"numberGameWLByCur.BackToCurrencyReport();\"";

        switch (RoleID) {
            case 4:
                self.Super = DetailName;
                self.SuperOnclick = "onclick=\"numberGameWLByCur.BackToSuperReport();\"";
                self.Master = '';
                self.Agent = '';
                self.Title = "<span class='link' " + self.MainOnclick +
                                " >Main</span>  >  <span class='link' " + self.CurrencyOnclick + ">" + Currency +
                                 "</span>  >  <span>" + self.Super + "</span>";
                break;
            case 3:
                self.Master = DetailName;
                self.MasterOnclick = "onclick=\"numberGameWLByCur.BackToMasterReport();\"";
                self.Agent = '';

                self.Title = "<span class='link' " + self.MainOnclick +
                                " >Main</span> > <span class='link'" + self.CurrencyOnclick + ">" + Currency +
                                 "</span>  >  <span class='link' " + self.SuperOnclick + ">" + self.Super +
                                 "</span>  >  <span >" + self.Master + "</span>";
                break;
            case 2:
                self.Agent = DetailName;
                self.AgentOnclick = "onclick=\"numberGameWLByCur.BackToAgentReport();\"";

                self.Title = "<span class='link' " + self.MainOnclick +
                                " >Main</span>  >  <span class='link'" + self.CurrencyOnclick + ">" + Currency +
                                "</span>  >  <span class='link' " + self.SuperOnclick + ">" + self.Super +
                                "</span>  >  <span class='link' " + self.MasterOnclick + ">" + self.Master +
                                "</span>  >  <span >" + self.Agent + "</span>";
                break;
            case 1:
                self.Member = DetailName;
                self.Title = "<span class='link' " + self.MainOnclick +
                                 " >Main</span>  >  <span class='link'" + self.CurrencyOnclick + ">" + Currency +
                                 "</span>  >  <span class='link'" + self.SuperOnclick + ">" + self.Super +
                                 "</span>  >  <span class='link' " + self.MasterOnclick + ">" + self.Master +
                                 "</span>  >  <span " + self.AgentOnclick + ">" + self.Agent +
                                 "</span>  >  <span >" + self.Member + "</span>";
                break;
            default:
                self.Title = "<span class='link' " + self.MainOnclick +
                                " >Main</span>  >  <span>" + Currency + "</span> ";
                break;
        }
    };

    self.InitPage = function () {
        self.getBettypes();
        self.getCurrencies();
        self.renderDatePicker();
        self.bindingEvent();
        self.getLicensee();
    };

    self.bindingEvent = function () {
        $("#btnGenerate").click(function () {
            self.getNumberGameWinlossByCur();
        });

        $(".checkbox-trialdata").change(function () {
            self.checkboxTrialDataChange();
        });
    };

    self.hideReport = function () {
        $('#numberGameWLByDateGrid').addClass("display-none");
    };

    self.renderDatePicker = function () {
        $('#datepickerplugin').nexDatePicker({
            nexDatePickerType: "daterange",
            datePickerFormat: {
                changeMonth: true,
                changeYear: true,
                dateFormat: FormatConstant.M_DD_YY_STRING,
                minDate: new Date(minDate),
                maxDate: new Date(yesterday)
            },
            firstDatePicker: {
                Id: "firstDatePickerId1",
                Label: labelFrom,
                InitDate: yesterday,
                LabelCss: "lbl-field col_3 align-right",
                TextboxCss: "edit-field col_7"
            },
            secondDatePicker: {
                Id: "secondDatePickerId1",
                Label: labelTo,
                InitDate: yesterday,
                LabelCss: "lbl-field col_3",
                TextboxCss: "edit-field col_5"
            },
            selectedItem: 5,
            onSelectDatePicker: function () {
                self.datePickerChange();
            }
        });
    };

    self.datePickerChange = function () {
        self.getLicensee();
    };

    self.getLicensee = function () {
        var dateRange = $('#datepickerplugin').nexDatePicker("getDateRange");
        var trialData = $(".checkbox-trialdata").is(":checked");
        var params = {
            FromDate: dateRange.FromDate,
            ToDate: dateRange.ToDate,
            GetIsActiveOnly: true,
            ExcludeLicenseeID: 18,
            SystemID: 3,
            ProductPrefix: "NG",
            TrialData: trialData
        };

        $.ajax({
            type: "POST",
            url: window.rootUrl + "Gaming/Gaming/GetLicenseesByProduct",
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            data: JSON.stringify(params),
            beforeSend: function () {
                AccReport.disableInput();
            },
            success: function (rs) {
                var id = "ddlSite";
                var companies = [];
                var companyLength = rs.ChildInfo.length;
                for (var i = 0; i < companyLength; i++) {
                    var company = rs.ChildInfo[i];
                    var licensees = [];
                    var licenseeLength = company.ChildInfo.length;
                    for (var j = 0; j < licenseeLength; j++) {
                        var licensee = company.ChildInfo[j];
                        licensees.push({
                            Id: id + "-group" + company.Id +
                             "-licensee" + licensee.Id.toString(),
                            Value: licensee.Id,
                            Text: licensee.Name,
                            Checked: true,
                            Enable: true,
                            CssClass: "",
                            Items: []
                        });
                    }

                    companies.push({
                        Id: id + "-group" + company.Id,
                        Value: company.Id,
                        Text: company.Name,
                        Checked: true,
                        Enable: true,
                        CssClass: "",
                        Items: licensees
                    });
                }

                self.renderLicenseesDropdown(companies);
            },
            error: function (event, jqXHR, ajaxSettings) {
                renderLicenseesDropdown([]);
            },
            complete: function () {
                AccReport.enableInput();
            }
        });
    };

    self.renderLicenseesDropdown = function (licensees) {
        var noItemText = '';
        if (licensees.length == 0) {
            noItemText = 'No licensee found.';
        }

        $("#ddlSite").multiSelect(
		{
		    minWidth: 160,
		    maxHeight: 450,
		    shadow: true,
		    showHeader: true,
		    isCheckAll: true,
		    showFooter: false,
		    checkAllText: 'All Licensees',
		    isUsedCustomCheckAllText: true,
		    customCheckAllText: 'All Licensees',
		    noItemText: noItemText,
		    listData: licensees
		});
    };

    self.getCurrencies = function () {
        $.ajax({
            type: "POST",
            url: window.rootUrl + "Gaming/NumberGameWinLossByCurrency/GetCurrencies",
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            data: null,
            beforeSend: function () {
                AccReport.disableInput();
            },
            success: function (rs) {
                var id = "ddlCurrency";
                var currencies = [];
                for (var i = 0; i < rs.length; i++) {
                    var currency = rs[i];
                    currencies.push({
                        Id: "" + currency.Id.toString(),
                        Value: currency.Id,
                        Text: currency.Name,
                        Checked: true,
                        Enable: true,
                        CssClass: "",
                        Items: []
                    });
                }
                self.renderCurrenciesDropdown(currencies);
            },
            error: function (event, jqXHR, ajaxSettings) {
                self.renderCurrenciesDropdown([]);
            },
            complete: function () {
                AccReport.enableInput();
            }
        });
    };

    self.renderCurrenciesDropdown = function (currencies) {
        var noItemText = '';
        if (currencies.length == 0) {
            noItemText = 'No licensee found.';
        }

        $("#ddlCurrency").multiSelect(
		{
		    minWidth: 120,
		    maxHeight: 450,
		    shadow: true,
		    showHeader: true,
		    isCheckAll: true,
		    showFooter: false,
		    checkAllText: 'All Currencies',
		    isUsedCustomCheckAllText: true,
		    customCheckAllText: 'All Currencies',
		    noItemText: noItemText,
		    listData: currencies
		});
    };

    self.getBettypes = function () {
        $.ajax({
            type: "POST",
            url: window.rootUrl + "Gaming/NumberGameWinlossByDate/GetBettypes",
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            data: null,
            beforeSend: function () {
                AccReport.disableInput();
            },
            success: function (rs) {
                var id = "ddlBettype";
                var bettypes = [];
                for (var i = 0; i < rs.length; i++) {
                    var bettype = rs[i];
                    bettypes.push({
                        Id: "" + bettype.Id.toString(),
                        Value: bettype.Id,
                        Text: bettype.Name,
                        Checked: true,
                        Enable: true,
                        CssClass: "",
                        Items: []
                    });
                }
                self.renderBettypesDropdown(bettypes);
            },
            error: function (event, jqXHR, ajaxSettings) {
                self.renderBettypesDropdown([]);
            },
            complete: function () {
                AccReport.enableInput();
            }
        });
    };

    self.renderBettypesDropdown = function (bettypes) {
        var noItemText = '';
        if (bettypes.length == 0) {
            noItemText = 'No licensee found.';
        }

        $("#ddlBettype").multiSelect(
		{
		    minWidth: 150,
		    maxHeight: 450,
		    shadow: true,
		    showHeader: true,
		    isCheckAll: true,
		    showFooter: false,
		    checkAllText: 'All Bettypes',
		    isUsedCustomCheckAllText: true,
		    customCheckAllText: 'All Bettypes',
		    noItemText: noItemText,
		    listData: bettypes
		});
    };

    self.showHideComm = function (isShow, gridId, gridOptions) {
        AccReport.disableInput();
        var arrColHide;

        if (gridId == 'numberGameWLByDateMainGrid') {
            arrColHide = isShow ? [] : [6];
        }
        else {
            arrColHide = isShow ? [] : [2, 4, 5, 7, 8, 10, 11, 13, 14];
        }

        if (!isShow) {
            $('#' + gridId).fanexGrid("hideColumns", gridOptions, arrColHide);
        }
        else {
            $('#' + gridId).fanexGrid("hideColumns", gridOptions, []);
        }
        AccReport.enableInput();
    };

    self.RenderGrid = function (title, bodyData, footerData) {
        var headerGroup = [
            {
                Columns: [
                    { HeaderText: "Currency", Name: "Currency", MergeRowNumber: 2 },
					{ HeaderText: "Turnover", Name: "Turnover", MergeRowNumber: 2 },
					{ HeaderText: "Member Win Loss", Name: "MemberWinLoss", MergeRowNumber: 2 },
                    { HeaderText: "Game Payout", Name: "GamePayout", MergeRowNumber: 2 },
					{ HeaderText: "Company", Name: "CompanyTurnover", MergeColNumber: 5, HeaderAlign: "center" }
                ]
            }
        ]

        var columns = [
					{ HeaderText: "Currency", Width: 150, Name: "Currency", CustomCellRender: function (currentCell) {
					    return "<div class='link centerAlign' onclick='numberGameWLByCur.getNumberGameWinlossByCurDetail(\"" + currentCell.Value + "\",0,0)'>" + currentCell.Value + "</div>";
					}
					},
					{ HeaderText: "Turnover", Width: 120, Name: "Turnover", CellAlign: "right", FormatNumber: FormatConstant.TWO_DECIMAL_NUMBER, HeaderAlign: "center" },
                    { HeaderText: "Member Win Loss", Width: 120, Name: "MemberWinLoss", CellAlign: "right", FormatNumber: FormatConstant.TWO_DECIMAL_NUMBER, HeaderAlign: "center", FormatPositive: '$' },
                    { HeaderText: "Game Payout", Width: 120, Name: "GamePayout", CellAlign: "right", FormatNumber: FormatConstant.TWO_DECIMAL_PERCENT_NUMBER, HeaderAlign: "center", FormatPositive: '$' },
					{ HeaderText: "Net Turnover", Width: 120, Name: "CompanyTurnover", CellAlign: "right", FormatNumber: FormatConstant.TWO_DECIMAL_NUMBER, HeaderAlign: "center" },
					{ HeaderText: "PT Win Loss", Width: 120, Name: "CompanyWinLoss", CellAlign: "right", HeaderAlign: "center", FormatNumber: FormatConstant.TWO_DECIMAL_NUMBER, FormatPositive: '$' },
                    { HeaderText: "Comm Expense", Width: 120, Name: "CommExpense", CellAlign: "right", HeaderAlign: "center", FormatNumber: FormatConstant.TWO_DECIMAL_NUMBER },
                    { HeaderText: "Net Income", Width: 120, Name: "NetIncome", CellAlign: "right", HeaderAlign: "center", FormatNumber: FormatConstant.TWO_DECIMAL_NUMBER, FormatPositive: '$' },
					{ HeaderText: "Margin", Width: 120, Name: "Margin", CellAlign: "right", HeaderAlign: "center", FormatNumber: FormatConstant.TWO_DECIMAL_PERCENT_NUMBER, FormatPositive: '$' }
				];

        var gridOptions = {
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
            customizeHeader: true,
            groupHeaderRows: headerGroup,
            numberOfFrozenColumn: 1,
            showDistinctRow: false,
            marginRight: 20
        };

        $("#numberGameWLByDateMainGrid_CurrencyContainer").addClass("display-none");
        $("#numberGameWLByDateMainGrid_SuperContainer").addClass("display-none");
        $("#numberGameWLByDateMainGrid_MasterContainer").addClass("display-none");
        $("#numberGameWLByDateMainGrid_AgentContainer").addClass("display-none");
        $("#numberGameWLByDateMainGrid_MemberContainer").addClass("display-none");
        $("#numberGameWLByDateMainGridContainer").removeClass("display-none");
        $('#numberGameWLByDateGrid').removeClass("display-none");
        var currentGrid = $("#numberGameWLByDateMainGrid").fanexGrid(gridOptions);

        if ($('#checkbox-showcommision').is(':checked')) {
            self.showHideComm(true, 'numberGameWLByDateMainGrid', gridOptions);
        }
        else {
            self.showHideComm(false, 'numberGameWLByDateMainGrid', gridOptions);
        }
        $('.no-data-frozen ').closest('.grid-container').prev().find('th').first().css('border', '0 none');
    };

    self.getNumberGameWinlossByCur = function () {

        AccReport.disableInput();

        _params.paramReport.Currency = $('#ddlCurrency').multiSelect('getCheckedItems').toString();
        _params.paramReport.Bettype = $('#ddlBettype').multiSelect('getCheckedItems').toString();
        _params.paramReport.SiteID = $('#ddlSite').multiSelect('getCheckedItems').toString();
        var dateRange = $('#datepickerplugin').nexDatePicker("getDateRange");
        _params.paramReport.FromDate = dateRange.FromDate;
        _params.paramReport.ToDate = dateRange.ToDate;
        _params.paramReport.IsLive = $('#ddlLiveStatus').val();
        _params.paramReport.TrialData = $(".checkbox-trialdata").is(":checked");

        if (!self.checkReportParams(_params)) {
            AccReport.enableInput();
            return;
        }

        $.ajax({
            type: "POST",
            url: window.rootUrl + "Gaming/NumberGameWinLossByCurrency/GetNumberGameWinlossByCur",
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            data: JSON.stringify(_params.paramReport),
            beforeSend: function () {
            },
            success: function (rs) {
                AccReport.popUp.hide();
                _mainModelTmp.param = _params.paramReport;
                _mainModelTmp.model = rs.clone();
                self.mapDataMain(rs);
            },
            error: function (jqXHR, event, ajaxSettings) {
                if (jqXHR.status &&
                    jqXHR.status == AccReportData.Constants.HtmlErrorCode.SystemError) {
                    var r = jQuery.parseJSON(jqXHR.responseText);
                    AccReport.popUp.show(r.message);
                    self.mapDataMain([]);
                }
            },
            complete: function () {
                AccReport.enableInput();
            }
        });
    };

    self.checkReportParams = function (params) {
        AccReport.popUp.removeHighLight();
        if (params.paramReport.SiteID == '') {
            AccReport.popUp.show("Please select  at least one company!", "ddlSite");
            return false;
        }

        if (params.paramReport.Currency == '') {
            AccReport.popUp.show("Please select  at least one currency!", "ddlCurrency");
            return false;
        }

        if (params.paramReport.Bettype == '') {
            AccReport.popUp.show("Please select  at least one bet type!", "ddlBettype");
            return false;
        }

        return true;
    };

    self.mapDataMain = function (dataMain) {
        _isMainPage = true;
        var bodyData = [];
        var footerData = [];
        if (dataMain && dataMain.length > 0) {
            var wlDateLength = dataMain.length - 1;
            for (var i = 0; i < wlDateLength; i++) {
                var wlDate = dataMain[i];
                var currentRow = {
                    Columns: [
                               { Name: "Currency", Value: wlDate.Currency },
                               { Name: "Turnover", Value: wlDate.Turnover },
                               { Name: "MemberWinLoss", Value: wlDate.MemberWinLoss },
                               { Name: "GamePayout", Value: wlDate.GamePayout },
                               { Name: "CompanyTurnover", Value: wlDate.NetTurnover },
                               { Name: "CompanyWinLoss", Value: wlDate.PTWinLoss },
                               { Name: "CommExpense", Value: wlDate.CommExpense },
                               { Name: "NetIncome", Value: wlDate.NetIncome },
                               { Name: "Margin", Value: wlDate.Margin }
                            ],
                    SubRows: []
                };
                bodyData.push(currentRow);
            }

            var footerdataTemp = dataMain[wlDateLength];
            footerData = [
						{
						    Columns: [
							  { Name: "Currency", CellAlign: 'center', Value: footerdataTemp.Currency },
                               { Name: "Turnover", Value: footerdataTemp.Turnover },
                               { Name: "MemberWinLoss", Value: footerdataTemp.MemberWinLoss },
                               { Name: "GamePayout", Value: footerdataTemp.GamePayout },
                               { Name: "CompanyTurnover", Value: footerdataTemp.NetTurnover },
                               { Name: "CompanyWinLoss", Value: footerdataTemp.PTWinLoss },
                               { Name: "CommExpense", Value: footerdataTemp.CommExpense },
                               { Name: "NetIncome", Value: footerdataTemp.NetIncome },
                               { Name: "Margin", Value: footerdataTemp.Margin }
                               									]
						}]
        }
        self.RenderGrid("Main", bodyData, footerData);

    };

    self.RenderGridDetail = function (title, bodyData, footerData, roleId) {
        var headerGroup = [
            {
                Columns: [
                    { HeaderText: "Username", Name: "Username", MergeRowNumber: 2 },
					{ HeaderText: "Turnover", Name: "CustomerTurnover", MergeRowNumber: 2 },
					{ HeaderText: "GrossComm", Name: "CustomerGrossComm", MergeRowNumber: 2 },
                    { HeaderText: "Member", Name: "MemberWinloss", MergeColNumber: 3, HeaderAlign: "center" },
                    { HeaderText: "Agent", Name: "AgentWinloss", MergeColNumber: 3, HeaderAlign: "center" },
                    { HeaderText: "Master", Name: "MasterWinloss", MergeColNumber: 3, HeaderAlign: "center" },
                    { HeaderText: "Super", Name: "SuperWinloss", MergeColNumber: 3, HeaderAlign: "center" },
					{ HeaderText: "Company", Name: "CompanyTurnover", MergeRowNumber: 2 }
                ]
            }
        ]

        var columns = [
                    { HeaderText: "Username", Width: 150, Name: "Username", CustomCellRender: function (currentCell) {
                        if (currentCell.Value.RoleID == 1) {
                            return "<div class='centerAlign'>" + currentCell.Value.Username + "</div>";
                        }
                        else {
                            return "<div class='link centerAlign' onclick='numberGameWLByCur.getNumberGameWinlossByCurDetail(\"" + currentCell.Value.Currency + "\"," + currentCell.Value.RoleID + "," + currentCell.Value.CustID + ",\"" + currentCell.Value.Username + "\")'>" + currentCell.Value.Username + "</div>";
                        }

                    }
                    },
					{ HeaderText: "Turnover", Width: 120, Name: "CustomerTurnover", CellAlign: "right", FormatNumber: FormatConstant.TWO_DECIMAL_NUMBER, HeaderAlign: "center" },
                    { HeaderText: "GrossComm", Width: 120, Name: "CustomerGrossComm", CellAlign: "right", FormatNumber: FormatConstant.TWO_DECIMAL_NUMBER, HeaderAlign: "center", FormatPositive: '$' },

                    { HeaderText: "Win Loss", Width: 120, Name: "MemberWinloss", CellAlign: "right", HeaderAlign: "center", FormatNumber: FormatConstant.TWO_DECIMAL_NUMBER, FormatPositive: '$' },
                    { HeaderText: "Comm", Width: 120, Name: "MemberComm", CellAlign: "right", HeaderAlign: "center", FormatNumber: FormatConstant.TWO_DECIMAL_NUMBER, FormatPositive: '$' },
					{ HeaderText: "Total", Width: 120, Name: "MemberTotal", CellAlign: "right", HeaderAlign: "center", FormatNumber: FormatConstant.TWO_DECIMAL_NUMBER, FormatPositive: '$' },

                    { HeaderText: "Win Loss", Width: 120, Name: "AgentWinloss", CellAlign: "right", FormatNumber: FormatConstant.TWO_DECIMAL_NUMBER, HeaderAlign: "center", FormatPositive: '$' },
					{ HeaderText: "Comm", Width: 120, Name: "AgentComm", CellAlign: "right", FormatNumber: FormatConstant.TWO_DECIMAL_NUMBER, HeaderAlign: "center", FormatPositive: '$' },
					{ HeaderText: "Total", Width: 120, Name: "AgentTotal", CellAlign: "right", HeaderAlign: "center", FormatNumber: FormatConstant.TWO_DECIMAL_NUMBER, FormatPositive: '$' },

                    { HeaderText: "Win Loss", Width: 120, Name: "MasterWinloss", CellAlign: "right", HeaderAlign: "center", FormatNumber: FormatConstant.TWO_DECIMAL_NUMBER, FormatPositive: '$' },
                    { HeaderText: "Comm", Width: 120, Name: "MasterComm", CellAlign: "right", HeaderAlign: "center", FormatNumber: FormatConstant.TWO_DECIMAL_NUMBER, FormatPositive: '$' },
					{ HeaderText: "Total", Width: 120, Name: "MasterTotal", CellAlign: "right", HeaderAlign: "center", FormatNumber: FormatConstant.TWO_DECIMAL_NUMBER, FormatPositive: '$' },

                    { HeaderText: "Win Loss", Width: 120, Name: "SuperWinloss", CellAlign: "right", HeaderAlign: "center", FormatNumber: FormatConstant.TWO_DECIMAL_NUMBER, FormatPositive: '$' },
                    { HeaderText: "Comm", Width: 120, Name: "SuperComm", CellAlign: "right", HeaderAlign: "center", FormatNumber: FormatConstant.TWO_DECIMAL_NUMBER, FormatPositive: '$' },
					{ HeaderText: "Total", Width: 120, Name: "SuperTotal", CellAlign: "right", HeaderAlign: "center", FormatNumber: FormatConstant.TWO_DECIMAL_NUMBER, FormatPositive: '$' },

                    { HeaderText: "Company", Width: 100, Name: "CompanyTurnover", CellAlign: "right", HeaderAlign: "center", FormatNumber: FormatConstant.TWO_DECIMAL_NUMBER, FormatPositive: '$' }

                ];

        var gridOptions = {
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
            customizeHeader: true,
            groupHeaderRows: headerGroup,
            numberOfFrozenColumn: 3,
            showDistinctRow: false,
            marginRight: 20,
            showPager: true,
            pagerOption: {
                itemsPerPage: 50,
                maxItemsPerPage: 200,
                currentPage: 0,
                totalItem: 100,
                startPerPage: 50,
                pageStep: 50,
                itemsPerPageArray: [50, 100, 200, 500]
            }
        };

        var gridId = "";
        switch (roleId) {
            case 0:
                gridId = "numberGameWLByDateMainGrid_Currency";
                $("#numberGameWLByDateMainGridContainer").addClass("display-none");
                $("#numberGameWLByDateMainGrid_SuperContainer").addClass("display-none");
                $("#numberGameWLByDateMainGrid_MasterContainer").addClass("display-none");
                $("#numberGameWLByDateMainGrid_AgentContainer").addClass("display-none");
                $("#numberGameWLByDateMainGrid_MemberContainer").addClass("display-none");
                $("#" + gridId).fanexGrid(gridOptions);
                break;
            case 4:
                gridId = "numberGameWLByDateMainGrid_Super";
                $("#numberGameWLByDateMainGridContainer").addClass("display-none");
                $("#numberGameWLByDateMainGrid_CurrencyContainer").addClass("display-none");
                $("#numberGameWLByDateMainGrid_MasterContainer").addClass("display-none");
                $("#numberGameWLByDateMainGrid_AgentContainer").addClass("display-none");
                $("#numberGameWLByDateMainGrid_MemberContainer").addClass("display-none");
                $("#numberGameWLByDateMainGrid_SuperContainer").removeClass("display-none");
                $("#" + gridId).fanexGrid(gridOptions);
                break;
            case 3:
                gridId = "numberGameWLByDateMainGrid_Master";
                $("#numberGameWLByDateMainGridContainer").addClass("display-none");
                $("#numberGameWLByDateMainGrid_CurrencyContainer").addClass("display-none");
                $("#numberGameWLByDateMainGrid_SuperContainer").addClass("display-none");
                $("#numberGameWLByDateMainGrid_AgentContainer").addClass("display-none");
                $("#numberGameWLByDateMainGrid_MemberContainer").addClass("display-none");
                $("#numberGameWLByDateMainGrid_MasterContainer").removeClass("display-none");
                $("#" + gridId).fanexGrid(gridOptions);
                break;
            case 2:
                gridId = "numberGameWLByDateMainGrid_Agent";
                $("#numberGameWLByDateMainGridContainer").addClass("display-none");
                $("#numberGameWLByDateMainGrid_CurrencyContainer").addClass("display-none");
                $("#numberGameWLByDateMainGrid_SuperContainer").addClass("display-none");
                $("#numberGameWLByDateMainGrid_MasterContainer").addClass("display-none");
                $("#numberGameWLByDateMainGrid_MemberContainer").addClass("display-none");
                $("#numberGameWLByDateMainGrid_AgentContainer").removeClass("display-none");
                $("#" + gridId).fanexGrid(gridOptions);
                break;
            default:
                gridId = "numberGameWLByDateMainGrid_Member";
                $("#numberGameWLByDateMainGridContainer").addClass("display-none");
                $("#numberGameWLByDateMainGrid_CurrencyContainer").addClass("display-none");
                $("#numberGameWLByDateMainGrid_SuperContainer").addClass("display-none");
                $("#numberGameWLByDateMainGrid_MasterContainer").addClass("display-none");
                $("#numberGameWLByDateMainGrid_AgentContainer").addClass("display-none");
                $("#numberGameWLByDateMainGrid_MemberContainer").removeClass("display-none");
                $("#" + gridId).fanexGrid(gridOptions);
                break;
        }

        if ($('#checkbox-showcommision').is(':checked')) {
            self.showHideComm(true, gridId, gridOptions);
        }
        else {
            self.showHideComm(false, gridId, gridOptions);
        }
        $('.no-data-frozen ').closest('.grid-container').prev().find('th').first().css('border', '0 none');
    };

    self.getNumberGameWinlossByCurDetail = function (currency, roleId, custId, username) {

        AccReport.disableInput();
        _detailParams.Currency = currency.toString();
        _detailParams.Bettype = $('#ddlBettype').multiSelect('getCheckedItems').toString();
        _detailParams.SiteID = $('#ddlSite').multiSelect('getCheckedItems').toString();
        var dateRange = $('#datepickerplugin').nexDatePicker("getDateRange");
        _detailParams.FromDate = dateRange.FromDate;
        _detailParams.ToDate = dateRange.ToDate;
        _detailParams.IsLive = $('#ddlLiveStatus').val();
        _detailParams.Username = username;
        _detailParams.TrialData = $(".checkbox-trialdata").is(":checked");

        getLinkForTitle(currency, custId, roleId, username);

        if (roleId == 0) {
            _detailParams.RoleID = 0;
            _detailParams.CustID = 0;
        }
        else {
            _detailParams.RoleID = roleId - 1;
            _detailParams.CustID = custId;
        }
        
        $.ajax({
                    type: "POST",
                    url: window.rootUrl + "Gaming/NumberGameWinLossByCurrency/GetNumberGameWinlossDetailByCur",
                    contentType: "application/json; charset=utf-8",
                    dataType: "json",
                    data: JSON.stringify(_detailParams),
                    beforeSend: function () {
                    },
                    success: function (rs) {
                        AccReport.popUp.hide();
                        switch (roleId) {
                            case 0:
                                _superModelTmp.model = rs.clone();
                                _superModelTmp.currency = currency;
                                break;
                            case 4:
                                _masterModelTmp.model = rs.clone();
                                _masterModelTmp.superId = custId;
                                break;
                            case 3:
                                _agentModelTmp.model = rs.clone();
                                _agentModelTmp.masterId = custId;
                                break;
                            case 2:
                                _memberModelTmp.model = rs.clone();
                                _memberModelTmp.agentId = custId;
                                break;
                        }

                        self.mapDataDetail(rs, roleId);
                    },
                    error: function (jqXHR, event, ajaxSettings) {
                        if (jqXHR.status &&
                            jqXHR.status == AccReportData.Constants.HtmlErrorCode.SystemError) {
                            var r = jQuery.parseJSON(jqXHR.responseText);
                            AccReport.popUp.show(r.message);
                            self.mapDataDetail([], roleId);
                        }
                    },
                    complete: function () {
                        AccReport.enableInput();
                    }
                });
    };

    self.mapDataDetail = function (dataDetail, roleId) {
        _isMainPage = false;
        var bodyData = [];
        var footerData = [];
        if (dataDetail && dataDetail.length > 0) {
            var wlDateLength = dataDetail.length - 1;
            for (var i = 0; i < wlDateLength; i++) {
                var wlDate = dataDetail[i];
                var currentRow = {
                    Columns: [
                               { Name: "Username", Value: wlDate },
                               { Name: "CustomerTurnover", Value: wlDate.Turnover },
                               { Name: "CustomerGrossComm", Value: wlDate.GrossComm },
                               { Name: "MemberWinloss", Value: wlDate.MemberWinLoss },
                               { Name: "MemberComm", Value: wlDate.MemberComm },
                               { Name: "MemberTotal", Value: wlDate.MemberTotal },

                               { Name: "AgentWinloss", Value: wlDate.AgentWinLoss },
                               { Name: "AgentComm", Value: wlDate.AgentComm },
                               { Name: "AgentTotal", Value: wlDate.AgentTotal },

                               { Name: "MasterWinloss", Value: wlDate.MasterWinLoss },
                               { Name: "MasterComm", Value: wlDate.MasterComm },
                               { Name: "MasterTotal", Value: wlDate.MasterTotal },

                               { Name: "SuperWinloss", Value: wlDate.SuperWinLoss },
                               { Name: "SuperComm", Value: wlDate.SuperComm },
                               { Name: "SuperTotal", Value: wlDate.SuperTotal },

                                { Name: "CompanyTurnover", Value: wlDate.Company }
                            ],
                    SubRows: []
                };
                bodyData.push(currentRow);
            }

            var footerdataTemp = dataDetail[wlDateLength];
            footerData = [
						{
						    Columns: [
							  { Name: "Username", Value: footerdataTemp, CellAlign: 'center', CustomCellRender: function () {
							      return "Total";
							  }
							  },
                               { Name: "CustomerTurnover", Value: footerdataTemp.Turnover },
                               { Name: "CustomerGrossComm", Value: footerdataTemp.GrossComm },
                               { Name: "MemberWinloss", Value: footerdataTemp.MemberWinLoss },
                               { Name: "MemberComm", Value: footerdataTemp.MemberComm },
                               { Name: "MemberTotal", Value: footerdataTemp.MemberTotal },

                               { Name: "AgentWinloss", Value: footerdataTemp.AgentWinLoss },
                               { Name: "AgentComm", Value: footerdataTemp.AgentComm },
                               { Name: "AgentTotal", Value: footerdataTemp.AgentTotal },

                               { Name: "MasterWinloss", Value: footerdataTemp.MasterWinLoss },
                               { Name: "MasterComm", Value: footerdataTemp.MasterComm },
                               { Name: "MasterTotal", Value: footerdataTemp.MasterTotal },

                               { Name: "MasterWinloss", Value: footerdataTemp.SuperWinLoss },
                               { Name: "MasterComm", Value: footerdataTemp.SuperComm },
                               { Name: "MasterTotal", Value: footerdataTemp.SuperTotal },

                                { Name: "CompanyTurnover", Value: footerdataTemp.Company }
									]
						}]
        }
        self.RenderGridDetail(self.Title, bodyData, footerData, roleId);

    };

    self.exportReport = function () {
        if (_isMainPage) {
            AccReport.downloadFile(window.rootUrl + "Gaming/NumberGameWinLossByCurrency/ExportReportMain", _params.paramReport);
        }
        else {
            AccReport.downloadFile(window.rootUrl + "Gaming/NumberGameWinLossByCurrency/ExportReportDetail", _detailParams);
        }
    }

    self.checkboxTrialDataChange = function () {
        self.getLicensee();
    };
};
var numberGameWLByCur = new Fanex.AccountingReports.Gaming.NumberGameWinlossByCur();

$(document).ready(function () {
    numberGameWLByCur.InitPage();
});