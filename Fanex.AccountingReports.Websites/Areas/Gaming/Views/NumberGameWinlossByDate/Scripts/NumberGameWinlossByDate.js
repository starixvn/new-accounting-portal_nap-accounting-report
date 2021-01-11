Namespace.Register("Fanex.AccountingReports.Gaming.NumberGameWinlossByDate");
Fanex.AccountingReports.Gaming.NumberGameWinlossByDate = function () {
    var self = this;

    var _params = {
        paramReport: {
            Companies: null,
            Bettypes: null,
            Currencies: null,
            FromDate: null,
            ToDate: null,
            TrialData: false
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
            self.getNumberGameWinlossByDate();
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
                dateFormat: FormatConstant.M_DD_YY_STRING
            },
            firstDatePicker: {
                Id: "firstDatePickerId1",
                Label: labelFrom,
                InitDate: today,
                LabelCss: "lbl-field col_2",
                TextboxCss: "edit-field col_5"
            },
            secondDatePicker: {
                Id: "secondDatePickerId1",
                Label: labelTo,
                InitDate: today,
                LabelCss: "lbl-field col_1",
                TextboxCss: "edit-field col_5"
            },
            selectedItem: 5,
            onSelectDatePicker: function () {
                self.datePickerChange();
            },
            minDate: new Date(minDate),
            maxDate: new Date(yesterday)
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
            url: window.rootUrl + "Gaming/NumberGameWinlossByDate/GetCurrencies",
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
                var sports = [];
                for (var i = 0; i < rs.length; i++) {
                    var sport = rs[i];
                    var bettypes = [];
                    for (j = 0; j < sport.Bettypes.length; j++) {
                        var bettype = sport.Bettypes[j];
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

                    sports.push({
                        Id: i,
                        Value: sport.SportName,
                        Text: sport.SportName,
                        Checked: true,
                        Enable: true,
                        CssClass: "",
                        Items: bettypes
                    });
                }

                self.renderBettypesDropdown(sports);
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
		    minWidth: 230,
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

    self.RenderGrid = function (title, bodyData, footerData) {
        var headerGroup = [
            {
                Columns: [
                    { HeaderText: "Date", Name: "Bettype", MergeRowNumber: 2 },
					{ HeaderText: "Member Win Loss", Name: "CustomerWL", MergeRowNumber: 2 },
					{ HeaderText: "SMA Win Loss", Name: "AgencyWL", MergeRowNumber: 2 },
                    { HeaderText: "Comm", Name: "Comm", MergeRowNumber: 2 },
					{ HeaderText: "House", Name: "Turnover", MergeColNumber: 5, HeaderAlign: "center" }
                ]
            }
        ]

        var columns = [
					{ HeaderText: "Date", Width: 237, Name: "Bettype", CellAlign: "left", ShowCollapseExpand: true, HeaderAlign: "center" },
					{ HeaderText: "Member Win Loss", Width: 120, Name: "CustomerWL", CellAlign: "right", FormatNumber: FormatConstant.TWO_DECIMAL_NUMBER, FormatPositive: '$', HeaderAlign: "center" },
					{ HeaderText: "SMA Win Loss", Width: 120, Name: "AgencyWL", CellAlign: "right", FormatNumber: FormatConstant.TWO_DECIMAL_NUMBER, HeaderAlign: "center" },
                    { HeaderText: "Comm", Width: 120, Name: "Comm", CellAlign: "right", FormatNumber: FormatConstant.TWO_DECIMAL_NUMBER, HeaderAlign: "center" },
					{ HeaderText: "Turnover", Width: 120, Name: "Turnover", CellAlign: "right", FormatNumber: FormatConstant.TWO_DECIMAL_NUMBER, HeaderAlign: "center" },
					{ HeaderText: "Real Income", Width: 120, Name: "LineTotal", CellAlign: "right", HeaderAlign: "center", FormatNumber: FormatConstant.TWO_DECIMAL_NUMBER, FormatPositive: '$' },
                    { HeaderText: "Virtual Win Loss", Width: 120, Name: "VirtualTotal", CellAlign: "right", HeaderAlign: "center", FormatNumber: FormatConstant.TWO_DECIMAL_NUMBER, FormatPositive: '$' },
                    { HeaderText: "Variance", Width: 120, Name: "Variance", CellAlign: "right", HeaderAlign: "center", FormatNumber: FormatConstant.TWO_DECIMAL_NUMBER, FormatPositive: '$' },
					{ HeaderText: "Margin", Width: 120, Name: "Margin", CellAlign: "right", HeaderAlign: "center", FormatNumber: FormatConstant.TWO_DECIMAL_PERCENT_NUMBER, FormatPositive: '$' }
        ];

        var gridOption = {
            columns: columns,
            showFooter: true,
            showTitle: true,
            titleInfo:
				{
				    text: title,
				    showExcelButton: true,
				    exportExcelFunction: function () {
				        self.exportReport();
				    },
				    showCollapseExpandButton: true
				},
            showTree: true,
            bodyRows: bodyData,
            footerRows: footerData,
            gridExpandHeight: 200,
            customizeHeader: true,
            groupHeaderRows: headerGroup,
            numberOfFrozenColumn: 1,
            showDistinctRow: false,
            marginRight: 20
        };

        var currentGrid = $("#numberGameWLByDateMainGrid").fanexGrid(gridOption);
    };

    self.getNumberGameWinlossByDate = function () {
        AccReport.disableInput();
        _params.paramReport.Currencies = $('#ddlCurrency').multiSelect('getCheckedItems').toString();
        _params.paramReport.Bettypes = $('#ddlBettype').multiSelect('getCheckedItems').toString();
        _params.paramReport.Companies = $('#ddlSite').multiSelect('getCheckedItems').toString();
        var dateRange = $('#datepickerplugin').nexDatePicker("getDateRange");
        _params.paramReport.FromDate = dateRange.FromDate;
        _params.paramReport.ToDate = dateRange.ToDate;
        _params.paramReport.TrialData = $(".checkbox-trialdata").is(":checked");

        var params = {
            Currency: _params.paramReport.Currencies,
            SiteID: _params.paramReport.Companies,
            Bettype: _params.paramReport.Bettypes,
            FromDate: _params.paramReport.FromDate,
            ToDate: _params.paramReport.ToDate,
            TrialData: _params.paramReport.TrialData
        };

        if (!self.checkReportParams(_params)) {
            AccReport.enableInput();
            return;
        }

        $.ajax({
            type: "POST",
            url: window.rootUrl + "Gaming/NumberGameWinlossByDate/GetNumberGameWinlossByDate",
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            data: JSON.stringify(params),
            beforeSend: function () {
            },
            success: function (rs) {
                AccReport.popUp.hide();
                self.renderReport(rs);
            },
            error: function (jqXHR, event, ajaxSettings) {
                if (jqXHR.status &&
                    jqXHR.status == AccReportData.Constants.HtmlErrorCode.SystemError) {
                    var r = jQuery.parseJSON(jqXHR.responseText);
                    AccReport.popUp.show(r.message);
                    self.renderReport(null);
                }
            },
            complete: function () {
                AccReport.enableInput();
            }
        });
    };

    self.renderReport = function (rs) {
        var bodyData = [];
        var title = "Number Game Win Loss By Date - From " + _params.paramReport.FromDate + " To " + _params.paramReport.ToDate;
        var footerData = [];
        if (rs && rs.ChildInfo.length > 0) {
            var wlDateLength = rs.ChildInfo.length;
            for (var i = 0; i < wlDateLength; i++) {
                var wlDate = rs.ChildInfo[i];
                var currentRow = {
                    Columns: [
                               { Name: "Bettype", Value: wlDate.WinLossDate },
                               { Name: "CustomerWL", Value: wlDate.GrandTotal.CustomerWinloss },
                               { Name: "AgencyWL", Value: wlDate.GrandTotal.AgencyWinloss },
                               { Name: "Comm", Value: wlDate.GrandTotal.Comm },
                               { Name: "Turnover", Value: wlDate.GrandTotal.Turnover },
                               { Name: "LineTotal", Value: wlDate.GrandTotal.LineTotal },
                               { Name: "VirtualTotal", Value: wlDate.GrandTotal.VirtualTotal },
                               { Name: "Variance", Value: wlDate.GrandTotal.Variance },
                               { Name: "Margin", Value: wlDate.GrandTotal.Margin }
                    ],
                    SubRows: []
                };
                if (wlDate.ChildInfo.length > 0) {
                    var wlStatusLength = wlDate.ChildInfo.length;
                    for (var j = wlStatusLength - 1; j >= 0; j--) {
                        var wlStatus = wlDate.ChildInfo[j];
                        var currStatusRow = {
                            Columns: [
                                        { Name: "Bettype", Value: wlStatus.Status, CssClass: "colBetTypeStatus" },
                                        { Name: "CustomerWL", Value: wlStatus.GrandTotal.CustomerWinloss },
                                        { Name: "AgencyWL", Value: wlStatus.GrandTotal.AgencyWinloss },
                                        { Name: "Comm", Value: wlStatus.GrandTotal.Comm },
                                        { Name: "Turnover", Value: wlStatus.GrandTotal.Turnover },
                                        { Name: "LineTotal", Value: wlStatus.GrandTotal.LineTotal },
                                        { Name: "VirtualTotal", Value: wlStatus.GrandTotal.VirtualTotal },
                                        { Name: "Variance", Value: wlStatus.GrandTotal.Variance },
                                        { Name: "Margin", Value: wlStatus.GrandTotal.Margin }
                            ],
                            SubRows: []
                        };

                        if (wlStatus.ChildInfo.length > 0) {
                            var detailLength = wlStatus.ChildInfo.length;
                            for (var k = 0; k < detailLength; k++) {
                                var wlDetail = wlStatus.ChildInfo[k];
                                var currDetailRow = {
                                    Columns: [
                                            { Name: "Bettype", Value: wlDetail.Bettype, CssClass: "colBetType" },
                                            { Name: "CustomerWL", Value: wlDetail.CustomerWinloss },
                                            { Name: "AgencyWL", Value: wlDetail.AgencyWinloss },
                                            { Name: "Comm", Value: wlDetail.Comm },
                                            { Name: "Turnover", Value: wlDetail.Turnover },
                                            { Name: "LineTotal", Value: wlDetail.LineTotal },
                                            { Name: "VirtualTotal", Value: wlDetail.VirtualTotal },
                                            { Name: "Variance", Value: wlDetail.Variance },
                                            { Name: "Margin", Value: wlDetail.Margin }
                                    ]
                                };
                                currStatusRow.SubRows.push(currDetailRow);
                            }
                        }
                        currentRow.SubRows.push(currStatusRow);
                    }
                }
                bodyData.push(currentRow);
            }

            if (rs.ChildInfo.length > 0) {
                footerData = [
						{
						    Columns: [
										{ Name: "Bettype", Value: "Grand Total" },
										{ Name: "CustomerWL", Value: rs.GrandTotal.CustomerWinloss },
                                        { Name: "AgencyWL", Value: rs.GrandTotal.AgencyWinloss },
                                        { Name: "Comm", Value: rs.GrandTotal.Comm },
                                        { Name: "Turnover", Value: rs.GrandTotal.Turnover },
                                        { Name: "LineTotal", Value: rs.GrandTotal.LineTotal },
                                        { Name: "VirtualTotal", Value: rs.GrandTotal.VirtualTotal },
                                        { Name: "Variance", Value: rs.GrandTotal.Variance },
                                        { Name: "Margin", Value: rs.GrandTotal.Margin }
						    ]
						}]
            }
        }

        self.RenderGrid(title, bodyData, footerData);
    };

    self.checkReportParams = function (params) {
        AccReport.popUp.removeHighLight();
        if (params.paramReport.Companies == '') {
            AccReport.popUp.show("Please select  at least one company!", "ddlSite");
            return false;
        }

        if (params.paramReport.Currencies == '') {
            AccReport.popUp.show("Please select  at least one currency!", "ddlCurrency");
            return false;
        }

        if (params.paramReport.Bettypes == '') {
            AccReport.popUp.show("Please select  at least one bet type!", "ddlBettype");
            return false;
        }

        return true;
    };

    self.exportReport = function () {
        var params = {
            Currency: _params.paramReport.Currencies,
            SiteID: _params.paramReport.Companies,
            Bettype: _params.paramReport.Bettypes,
            FromDate: _params.paramReport.FromDate,
            ToDate: _params.paramReport.ToDate,
            TrialData: _params.paramReport.TrialData
        };
        AccReport.downloadFile(window.rootUrl + "Gaming/NumberGameWinlossByDate/ExportReport", params);
    };

    self.checkboxTrialDataChange = function () {
        self.getLicensee();
    };
};

$(document).ready(function () {
    var numberGameWLByDate = new Fanex.AccountingReports.Gaming.NumberGameWinlossByDate();
    numberGameWLByDate.InitPage();
});