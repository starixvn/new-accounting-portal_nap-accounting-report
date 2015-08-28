$(document).ready(function () {
    reportManager.InitPage();
});

Namespace.Register("Fanex.AccountingReports.Gaming.BingoOutStanding");
Fanex.AccountingReports.Gaming.BingoOutStanding.ReportManager = function () {
    var self = this;

    self.Tables = {
        showHideFilter: null,
        filterBar: null,
        txtUserName: null,
        autocomplete: null,
        excludeTestAccounts: null,
        autoCompleteManager: null,
        dropdownCompany: null,
        dropdownCurrency: null,

        btnGenerate: null,
        btnExport: null,

        mainBingoOutStandingReportContainer: null,
        mainBingoOutStandingReport: null,
        dropdownCompanyHeaderCheckBox: null
    };

    self.Enums = {
        LicenseeId: {
            All: 0,
            IBC: -1
        },

        CurrencyId: {
            All: 0,
            BaseRM: -1
        },

        SortDirection: {
            Asc: "ASC",
            Desc: "DESC"
        },

        SortDataType: {
            Number: "number",
            String: "string",
            Date: "Date"
        }
    };

    self.Params = {
        reportParams: {
            UserName: null,
            ExcludeTestAccounts: null,
            LicenseeList: null,
            Currency: null,
            IsChangedValue: false
        }
    };

    self.CachedData = {
        reportMonthRange: 3,
        defaultTextboxUserName: viewResources.Search_All,
        currentTextboxUserName: viewResources.Search_All,
        isDefaultTextboxUserName: true,
        isOnAuto: false,
        gridOptions: null,
        currentPage: 0,
        itemsPerPage: 50,
        defaultItemsPerPage: 50,
        itemsPerPageArray: [50, 100, 200, 500],
        currentReport: null,
        maxGridHeight: 10000
    };

    self.InitPage = function () {
        this.CachedDOMElements();
        this.MakeUserNameSuggestion();
        this.LoadFilterValues();
        this.BindingEvents();
    };

    self.CachedDOMElements = function () {
        this.Tables.showHideFilter = $(".showHideFilter");
        this.Tables.filterBar = $("#filter-bar");
        this.Tables.txtUserName = $("#txtUserName");
        this.Tables.excludeTestAccounts = $("#exclude-test-account");
        this.Tables.dropdownCompany = $("#dropdownCompany");
        this.Tables.dropdownCurrency = $("#dropdownCurrency");
        this.Tables.reportBy = $("#reportBy");

        this.Tables.btnGenerate = $("#btnGenerate");
        this.Tables.btnExport = $("#btnExport");

        this.Tables.mainBingoOutStandingReportContainer = $("#mainBingoOutStandingReportContainer");
        this.Tables.mainBingoOutStandingReport = $("#mainBingoOutStandingReport");
    };

    self.MakeUserNameSuggestion = function () {
        self.CachedData.autoCompleteManager = self.Tables.txtUserName.autocomplete({
            serviceUrl: window.rootWebPath.replace(/\/$/g, '') + '/ReportHelper/UserNameSuggestion?' +
            $.param(self.GetUserNameSuggestionParams()),
            minChars: 2
        });

        self.Tables.autocomplete = $('.autocomplete');
    };

    self.LoadFilterValues = function () {
        AccReport.disableInput();
        AccReport.utility.CallAjax(this,
            window.rootUrl + "Gaming/Gaming/GetLicensee_SupportBingo",
            null,
            self.RenderCompanyFilter,
            self.RenderCompanyFilter,
            self.GetCurrencyFilter);
    };

    self.RenderCompanyFilter = function (result) {
        this.Tables.dropdownCompany.empty();
        if (result && result.length > 0) {
            var optionLength = result.length;
            var ddwnCompanies = [];
            var prefixId = 'ddlCompany_';
            var isCheckAll = true;
            for (var i = 0; i < optionLength; i++) {
                var info = result[i];
                var isChecked = info.Id == self.Enums.LicenseeId.IBC ? true : false;
                ddwnCompanies.push({
                    Id: prefixId + info.Id,
                    Value: info.Id,
                    Text: info.Name,
                    Checked: isChecked,
                    Enable: true,
                    CssClass: "",
                    Items: []
                });

                if (!isChecked) {
                    isCheckAll = false;
                }
            }

            this.Tables.dropdownCompany.multiSelect(
            {
                minWidth: 177,
                maxHeight: 450,
                shadow: true,
                showHeader: true,
                isCheckAll: isCheckAll,
                showFooter: false,
                showTooltip: true,
                checkAllText: viewResources.All_Companies,
                isUsedCustomCheckAllText: true,
                customCheckAllText: viewResources.All_Companies,
                noItemText: viewResources.No_company_found,
                selectedList: 3,
                listData: ddwnCompanies
            });

            this.Tables.dropdownCompany = $("#dropdownCompany");
            this.Tables.dropdownCompanyHeaderCheckBox = $("#dropdownCompanyHeaderCheckBox");
        }
    };

    self.GetCurrencyFilter = function () {
        var self = reportManager;
        var selectParam = {
            LicenseeList: self.Tables.dropdownCompany.multiSelect("getCheckedItems").join(','),
            ExcludeTest: false
        };

        AccReport.utility.CallAjax(this,
            window.rootWebPath.replace(/\/$/g, '') + "/ReportHelper/GetCurrencyByLicensee",
            selectParam,
            self.RenderCurrencyFilter,
            self.RenderCurrencyFilter,
            AccReport.enableInput);
    };

    self.RenderCurrencyFilter = function (result) {
        var self = reportManager;
        self.Tables.dropdownCurrency.empty();
        if (result && result.length > 0) {
            var option = '';
            var optionLength = result.length;
            option = '<option value="' + self.Enums.CurrencyId.All + '">' + viewResources.All + '</option>';
            self.Tables.dropdownCurrency.append(option);
            for (var i = 0; i < optionLength; i++) {
                var info = result[i];
                option = '<option value="' + info.Id + '">' + info.Name + '</option>';
                self.Tables.dropdownCurrency.append(option);
            }

            self.Tables.dropdownCurrency.val(self.Enums.CurrencyId.BaseRM);
        }
    };

    self.BindingEvents = function () {
        this.Tables.showHideFilter.unbind("click");
        this.Tables.showHideFilter.click(function () {
            self.ShowHideFilter($(this).hasClass("expanded"));
        });

        this.Tables.txtUserName.unbind("blur");
        this.Tables.txtUserName.blur(function () {
            var textbox = $(this);
            if (!self.CachedData.isonAuto) {
                $('#Autocomplete_1').css('display', 'none');
            }
            else {
                textbox.focus();
            }

            var inputValue = '';
            var newName = textbox.val();
            var oldName = self.CachedData.currentTextboxUserName;
            self.CachedData.isDefaultTextboxUserName = false;
            textbox.removeClass("defaultSuggestionBox");
            if (oldName != '' &&
                newName != '' &&
                newName != oldName) {
                inputValue = newName;
            }
            else if (oldName != '' && newName == '') {
                inputValue = oldName;
                self.CachedData.isDefaultTextboxUserName = true;
                textbox.addClass("defaultSuggestionBox");
            }
            else if (newName != '') {
                inputValue = newName;
            }
            else {
                inputValue = self.CachedData.defaultTextboxUserName;
                self.CachedData.isDefaultTextboxUserName = true;
                textbox.addClass("defaultSuggestionBox");
            }

            textbox.val(inputValue);
        });

        this.Tables.txtUserName.unbind("focus");
        this.Tables.txtUserName.focus(function () {
            var textbox = $(this);
            textbox.removeClass("defaultSuggestionBox");
            if (self.CachedData.isDefaultTextboxUserName) {
                textbox.val('');
            }
        });

        this.Tables.autocomplete.mouseover(function () {
            self.CachedData.isonAuto = true;
        }).mouseout(function () {
            self.Tables.txtUserName.removeClass("defaultSuggestionBox");
            self.CachedData.isDefaultTextboxUserName = false;
            self.CachedData.isonAuto = false;
        });

        this.Tables.excludeTestAccounts.unbind("click");
        this.Tables.excludeTestAccounts.click(function () {
            self.CachedData.autoCompleteManager.setProperties({
                serviceUrl: window.rootWebPath.replace(/\/$/g, '') + '/ReportHelper/UserNameSuggestion?' +
                $.param(self.GetUserNameSuggestionParams()),
                minChars: 2
            });
        });

        this.Tables.btnGenerate.unbind("click");
        this.Tables.btnGenerate.click(function () {
            self.GetReport();
        });

        this.Tables.btnExport.unbind("click");
        this.Tables.btnExport.click(function () {
            self.ExportReport();
        });
    };

    self.ShowHideFilter = function (isHideFilter) {
        var self = this;
        if (isHideFilter) {
            this.Tables.showHideFilter.removeClass("expanded")
                                  .addClass("collapsed");
            this.Tables.showHideFilter.attr("title", viewResources.Show_filter);
            self.Tables.filterBar.addClass("display-none");
        } else {
            this.Tables.showHideFilter.removeClass("collapsed")
                                  .addClass("expanded");
            this.Tables.showHideFilter.attr("title", viewResources.Hide_filter);
            self.Tables.filterBar.removeClass("display-none");
        }

        var gridOptions = this.Tables.mainBingoOutStandingReport.fanexGrid("getCurrentGridOptions");
        if (gridOptions) {
            if (isHideFilter) {
                gridOptions.gridHeight = 150;
            } else {
                gridOptions.gridHeight = 350;
            }

            this.Tables.mainBingoOutStandingReport.fanexGrid("resizeGrid");
        }
    };

    self.GetUserNameSuggestionParams = function () {
        return {
            excludeTestAccounts: this.Tables.excludeTestAccounts.attr("checked") ? true : false
        };
    };

    self.GetReport = function () {
        AccReport.disableInput();
        var userName = self.Tables.txtUserName.val();
        var selectParam = {
            UserName: self.CachedData.isDefaultTextboxUserName ?
                      "" : userName,
            ExcludeTestAccounts: this.Tables.excludeTestAccounts.attr("checked") ? true : false,
            LicenseeList: self.Tables.dropdownCompany.multiSelect("getCheckedItems").join(','),
            Currency: self.Tables.dropdownCurrency.find("option:selected").val()
        };

        self.CachedData.currentPage = 0;
        self.Params.reportParams = selectParam;
        AccReport.utility.CallAjax(this,
            window.rootUrl + "Gaming/BingoOutStanding/GetReport",
            self.Params.reportParams,
            self.RenderReport,
            self.RenderReport,
            AccReport.enableInput);
    };

    self.RenderReport = function (report) {
        this.Tables.mainBingoOutStandingReportContainer.removeClass("display-none");
        this.Tables.mainBingoOutStandingReport.addClass("display-none");
        var reportTypeEnum = self.Enums.ReportType;

        var gridOptions = null;
        self.CachedData.currentReport = report;
        gridOptions = self.GetGridOptions(report);
        self.CachedData.gridOptions = gridOptions;
        if (gridOptions != null) {
            this.Tables.mainBingoOutStandingReport.removeClass("display-none").fanexGrid(gridOptions);
        }

        //    if (report && report.ChildsInfo && report.ChildsInfo.length > 0) {
        //        this.ShowHideFilter(true);
        //    }
    };

    self.GetGridOptions = function (report) {
        var bodyData = [];
        var footerData = [];
        var companyLength = report && report.ChildsInfo ?
            report.ChildsInfo.length :
            0;
        if (companyLength > 0) {
            AccReport.popUp.hide();
            var info;
            var columns = [];
            var indexRange = self.GetPagingIndexRange(companyLength);
            for (var i = indexRange.startIndex; i < indexRange.endIndex; i++) {
                var companyWinLoss = report.ChildsInfo[i];
                var reportLength = companyWinLoss.ChildsInfo.length;
                for (var j = 0; j < reportLength; j++) {
                    columns = [];
                    info = companyWinLoss.ChildsInfo[j];
                    if (j == 0) {
                        columns.push({ Name: "ColName", Value: companyWinLoss.Company, MergeRowNumber: reportLength });
                    }

                    columns.push({ Name: "ColCurrency", Value: info.CurrencyName });
                    columns.push({ Name: "ColUserName", Value: info.Name });
                    columns.push({ Name: "ColDate", Value: info.Date });
                    columns.push({ Name: "ColOutStanding", Value: info.OutStanding });
                    bodyData.push({
                        Columns: columns
                    });
                }
            }

            var reportLength = report.GrandTotal.ChildsInfo.length;
            for (var i = 0; i < reportLength; i++) {
                info = report.GrandTotal.ChildsInfo[i];
                footerData.push({
                    Columns: [{ Name: "ColName", Value: info.Name },
                                { Name: "ColCurrency", Value: info.CurrencyName },
                                { Name: "ColUserName", MergeColNumber: 2 },
                                { Name: "ColOutStanding", Value: info.OutStanding}]
                });
            }

            info = report.GrandTotal;
            footerData.push({
                Columns: [{ Name: "ColName", Value: info.Name },
                            { Name: "ColCurrency", Value: info.CurrencyName },
                            { Name: "ColUserName", MergeColNumber: 2 },
                            { Name: "ColOutStanding", Value: info.OutStanding}]
            });
        } else if (report && report.ErrorCode > 0) {
            AccReport.popUp.show(report.ErrorMessage);
        }

        var gridOptions = {
            columns: [{ HeaderText: viewResources.Company, Width: 160, Name: "ColName", HeaderAlign: "center", CellAlign: "center", Sort: true },
                { HeaderText: viewResources.Currency, Width: 100, Name: "ColCurrency", HeaderAlign: "center", CellAlign: "center", Sort: true },
                { HeaderText: viewResources.UserName, Width: 140, Name: "ColUserName", HeaderAlign: "center", CellAlign: "center", Sort: true },
                { HeaderText: viewResources.Date, Width: 120, Name: "ColDate", HeaderAlign: "center", CellAlign: "center", Sort: true },
                { HeaderText: viewResources.Outstanding, Width: 120, Name: "ColOutStanding", HeaderAlign: "center", CellAlign: "right", FormatNumber: FormatConstant.TWO_DECIMAL_NUMBER, FormatNegative: '$', Sort: true}],
            showTitle: true,
            showFooter: true,
            titleInfo: {
                text: viewResources.BingoOutStanding_ReportTitle,
                showExcelButton: false
            },
            bodyRows: bodyData,
            footerRows: footerData,
            numberOfFrozenColumn: 0,
            gridWidth: 1400,
            gridExpandHeight: 350,
            fullWidth: true,
            fullHeight: true,
            gridHeight: self.CachedData.maxGridHeight,
            showDistinctRow: false,
            showHighlightRow: false,
            headerColumnHeight: 29,
            showPager: true,
            customPager: true,
            sortOption:
            {
                customGridSort: function (currentBodyRows, columnName, currentDirection) {
                    var sortInfo = self.GetGridOptions_MappingSortInfo(columnName,
                        currentDirection);
                    var bodyRows = self.GetGridOptions_SortedAndPagingData(sortInfo);
                    return bodyRows;
                }
            },
            pagerOption: {
                itemsPerPage: self.CachedData.itemsPerPage,
                currentPage: self.CachedData.currentPage,
                itemsPerPageArray: self.CachedData.itemsPerPageArray,
                totalItem: companyLength,
                pagerSelectCallBack: function (page_index, itemsPerPage, jq) {
                    self.CachedData.itemsPerPage = itemsPerPage;
                    self.CachedData.currentPage = page_index;
                    self.RenderReport(self.CachedData.currentReport);
                }
            }
        };

        return gridOptions;
    };

    self.GetGridOptions_MappingSortInfo = function (columnName, currentDirection) {
        var sortDirectionEnums = self.Enums.SortDirection;
        var sortDataTypeEnums = self.Enums.SortDataType;
        var sortInfo = {
            sortFactor: 0,
            sortField: '',
            sortDataType: sortDataTypeEnums.String
        };

        switch (currentDirection) {
            case sortDirectionEnums.Asc:
                sortInfo.sortFactor = 1;
                break;
            case sortDirectionEnums.Desc:
                sortInfo.sortFactor = -1;
                break;
            default:
                sortInfo.sortFactor = 0;
                break;
        };

        switch (columnName) {
            case "ColName":
                sortInfo.sortField = "Company";
                sortInfo.sortDataType = sortDataTypeEnums.String;
                break;
            case "ColCurrency":
                sortInfo.sortField = "CurrencyName";
                sortInfo.sortDataType = sortDataTypeEnums.String;
                break;
            case "ColUserName":
                sortInfo.sortField = "Name";
                sortInfo.sortDataType = sortDataTypeEnums.String;
                break;
            case "ColDate":
                sortInfo.sortField = "Date";
                sortInfo.sortDataType = sortDataTypeEnums.Date;
                break;
            case "ColOutStanding":
                sortInfo.sortField = "OutStanding";
                sortInfo.sortDataType = sortDataTypeEnums.Number;
                break;
        }

        return sortInfo;
    };

    self.GetGridOptions_SortedAndPagingData = function (sortInfo) {
        self.CachedData.currentReport.ChildsInfo = self.SortReport(sortInfo, self.CachedData.currentReport);
        var report = self.CachedData.currentReport;

        var bodyData = [];
        var companyLength = report && report.ChildsInfo ?
            report.ChildsInfo.length :
            0;
        if (companyLength > 0) {
            var info;
            var columns = [];
            var indexRange = self.GetPagingIndexRange(companyLength);
            for (var i = indexRange.startIndex; i < indexRange.endIndex; i++) {
                var companyWinLoss = report.ChildsInfo[i];
                var reportLength = companyWinLoss.ChildsInfo.length;
                for (var j = 0; j < reportLength; j++) {
                    columns = [];
                    info = companyWinLoss.ChildsInfo[j];
                    if (j == 0) {
                        columns.push({ Name: "ColName", Value: companyWinLoss.Company, MergeRowNumber: reportLength });
                    }

                    columns.push({ Name: "ColCurrency", Value: info.CurrencyName });
                    columns.push({ Name: "ColUserName", Value: info.Name });
                    columns.push({ Name: "ColDate", Value: info.Date });
                    columns.push({ Name: "ColOutStanding", Value: info.OutStanding });
                    bodyData.push({
                        Columns: columns
                    });
                }
            }
        }

        return bodyData;
    };

    self.SortReport = function (sortInfo, report) {
        if (sortInfo.sortField == "Company") {
            return report.ChildsInfo.sort(function (elementA, elementB) {
                var valueA = elementA[sortInfo.sortField];
                var valueB = elementB[sortInfo.sortField];
                var isGreaterThan = self.CompareValues(sortInfo, valueA, valueB);
                return isGreaterThan;
            });
        }

        var companyLength = report && report.ChildsInfo ?
            report.ChildsInfo.length :
            0;
        if (companyLength > 0) {
            for (var i = 0; i < companyLength; i++) {
                var companyWinLoss = report.ChildsInfo[i];
                report.ChildsInfo[i].ChildsInfo = companyWinLoss.ChildsInfo.sort(function (elementA, elementB) {
                    var valueA = elementA[sortInfo.sortField];
                    var valueB = elementB[sortInfo.sortField];
                    var isGreaterThan = self.CompareValues(sortInfo, valueA, valueB);
                    return isGreaterThan;
                });
            }

            report.ChildsInfo = report.ChildsInfo.sort(function (elementA, elementB) {
                var valueA = elementA.ChildsInfo[0][sortInfo.sortField];
                var valueB = elementB.ChildsInfo[0][sortInfo.sortField];
                var isGreaterThan = self.CompareValues(sortInfo, valueA, valueB);
                return isGreaterThan;
            });
        }

        return report.ChildsInfo;
    };

    self.ExportReport = function () {
        var self = this;
        var userName = self.Tables.txtUserName.val();
        var exportParams = {
            UserName: self.CachedData.isDefaultTextboxUserName ?
                      "" : userName,
            ExcludeTestAccounts: this.Tables.excludeTestAccounts.attr("checked") ? true : false,
            LicenseeList: self.Tables.dropdownCompany.multiSelect("getCheckedItems").join(','),
            Currency: self.Tables.dropdownCurrency.find("option:selected").val()
        };

        exportParams.IsChangedValue = self.IsChangedCriteria(self.Params.reportParams, exportParams);
        AccReport.downloadFile(window.rootUrl + "Gaming/BingoOutStanding/ExportReport", exportParams);
    };

    self.CompareValues = function (sortInfo,
        valueA,
        valueB) {
        var sortDataTypeEnums = this.Enums.SortDataType;
        var isGreaterThan;
        switch (sortInfo.sortDataType) {
            case sortDataTypeEnums.Number:
                isGreaterThan = (sortInfo.sortFactor == 0 || valueB == valueA) ?
                                0 :
                                (valueA - valueB) * sortInfo.sortFactor > 0 ?
                                1 :
                                -1;
                break;
            case sortDataTypeEnums.String:
                isGreaterThan = (sortInfo.sortFactor == 0 || valueB == valueA) ?
                                0 :
                                (valueB < valueA && sortInfo.sortFactor == 1) ||
                                (valueA < valueB && sortInfo.sortFactor == -1) ?
                                1 :
                                -1;
                break;
            case sortDataTypeEnums.Date:
                var dateA = new Date(valueA);
                var dateB = new Date(valueB);
                isGreaterThan = (sortInfo.sortFactor == 0 || dateB == dateA) ?
                                0 :
                                (dateB < dateA && sortInfo.sortFactor == 1) ||
                                (dateA < dateB && sortInfo.sortFactor == -1) ?
                                1 :
                                -1;
                break;
        }

        return isGreaterThan;
    };

    self.IsChangedCriteria = function (oldCriteria,
        newCriteria) {
        return oldCriteria == null ||
           newCriteria.UserName != oldCriteria.UserName ||
           newCriteria.ExcludeTestAccounts != oldCriteria.ExcludeTestAccounts ||
           newCriteria.LicenseeList != oldCriteria.LicenseeList ||
           newCriteria.Currency != oldCriteria.Currency;
    };

    self.GetPagingIndexRange = function (maxLength) {
        var startIndex = self.CachedData.itemsPerPage * self.CachedData.currentPage;
        var endIndex = startIndex + self.CachedData.itemsPerPage;
        endIndex = endIndex <= maxLength ? endIndex : maxLength;
        return {
            startIndex: startIndex,
            endIndex: endIndex
        };
    };
};

var reportManager = new Fanex.AccountingReports.Gaming.BingoOutStanding.ReportManager();