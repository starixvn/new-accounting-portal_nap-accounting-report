
Namespace.Register("Fanex.AccountingReports.Misc.BetCount.HistoryReport");
Fanex.AccountingReports.Misc.BetCount.HistoryReport = function () {
    var self = this;

    var _tables = {
        reportFilter: null,
        ddlProduct: null,
        ddlSport: null,
        btnGenerate: null,
        btnBack: null,
        monthPicker: null,
        report: null,
        reportContent: null,
        reportTitle: null,
        reportExportExcel: null,
        reportRowHeader: null,
        reportRowFooter: null,
        reportColFooter: null,
        reportPagination: null,
        reportSorter: null,
        reportDivSorter: null
    };

    var _options = {
        report: {
            isShowPager: false,
            tblContentId: 'historyBetCountReportContent',
            manualCopyHeader: true,
            fullHeight: true,
            fullWidth: true,
            gridWidth: 1230,
            gridHeight: 195,
            freezeWidth: 80,
            freezeHeight: 280,
            numberFreezeCol: 1
        }
    };

    var _params = {
        historyBetCountReport: {
            FromDate: null,
            ToDate: null,
            Products: null,
            SportBettype: null,
            BettypeId: null,
            SortBy: null,
            SortDirection: null
        }
    };

    var _cachedData = {
        products: null,
        sports: null,
        historyBetCountReport: null,
        isFirstLoad: true,
        lastSorterBy: null,
        Sportbook_ProductId: "1_0",
        VirtualSport_GroupId: "5_0",
        BetradarVirtualSport_GroupId: "27_0",
        NG2_ProductId: "23_0",
        NG_ProductId: "3_0",
        SelectedSportBettype: [],
        Sports: [],
        Sportbook_SportID: [1, 999, -9],
    };

    self.initPage = function () {
        initGridScrollerTables();
        cachedDOMElements();
        bindingEvent();
    };
    var initGridScrollerTables = function () {
        $("#historyBetCountReport").initGrid({
            isShowPager: false
        });
    };
    var cachedDOMElements = function () {
        _tables.reportFilter = $("#historyBetCountReportFilter");
        _tables.btnGenerate = $("#btnGenerate_HistoryReport");

        _tables.report = $("#historyBetCountReport");
        _tables.reportHeader = $("#historyBetCountReportHeader");
        _tables.reportContent = _tables.report.find("#historyBetCountReportContent tbody");
        _tables.reportTitle = _tables.reportHeader.find("#historyBetCountReportTitle");
        _tables.reportExportExcel = _tables.reportHeader.find("#historyBetCountReport_ExportExcel");
        _tables.reportRowHeader = _tables.report.find("#historyBetCountReportRowHeader tbody");
        _tables.reportRowFooter = _tables.report.find(".row_footer_content thead");
        _tables.reportColFooter = _tables.report.find(".col_footer_content thead");
        _tables.reportPagination = _tables.report.find("#pagination");
        _tables.reportSorter = _tables.report.find(".colSorter");
        _tables.reportDivSorter = _tables.report.find(".divSorter");
    };
    var bindingEvent = function () {
        _tables.btnGenerate.click(function () {
            self.btnGenerateClick();
        });

        _tables.reportExportExcel.click(function () {
            exportReport();
        });

        _tables.reportSorter.click(function () {
            if (_cachedData.historyBetCountReport &&
                _cachedData.historyBetCountReport.TransactionList.length > 0) {
                AccReport.gridSorterManager.sortColumn($(this).find(".divSorter"), self.sortReport);
            }
        });
    };

    self.getDropdownSport = function () {
        var self = this;
        $.ajax({
            type: "POST",
            url: window.rootUrl + "Misc/BetCount/GetSports",
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            data: null,
            beforeSend: function () {
                AccReport.disableInput();
            },
            success: function (rs) {
                self.renderDropdownSport(rs);
            },
            error: function (event, jqXHR, ajaxSettings) {
                self.renderDropdownSport([]);
            },
            complete: function () {
                AccReport.enableInput();
            }
        })
    }

    self.renderDropdownProduct = function (products) {
        _cachedData.products = products;
        var self = this;
        var ddProduct = [];
        var noItemText = 'No product found.';
        var ddId = "ddlProduct_HistoryReport_";
        var productLength = products ? products.length : 0;
        var isSetDefaultCollapse = true;
        for (var i = 0; i < productLength; i++) {
            var productGroup = products[i];
            if (productGroup.Name.toLowerCase() == "sportsbook") {
                ddProduct.push({
                    id: "#" + ddId + "_Product1",
                    value: "1_0",
                    text: productGroup.Name,
                    checked: true,
                    enable: true,
                    cssClass: "bold"
                });

                continue;
            }
            if (productGroup.Name.toLowerCase() == "ng2") {
                ddProduct.push({
                    id: "#" + ddId + "_Product23",
                    value: "23_0",
                    text: productGroup.Name,
                    checked: false,
                    enable: true,
                    cssClass: "bold"
                });

                continue;
            }
            if (productGroup.Name.toLowerCase() == "number game") {
                ddProduct.push({
                    id: "#" + ddId + "_Product3",
                    value: "3_0",
                    text: productGroup.Name,
                    checked: false,
                    enable: true,
                    cssClass: "bold"
                });

                continue;
            }
            if (productGroup.Name.toLowerCase() == "virtual sports") {
                ddProduct.push({
                    id: "#" + ddId + "_Product5",
                    value: "5_0",
                    text: productGroup.Name,
                    checked: false,
                    enable: true,
                    cssClass: "bold"
                });

                continue;
            }
            if (productGroup.Name.toLowerCase() == "virtual sports 2") {
                ddProduct.push({
                    id: "#" + ddId + "_Product27",
                    value: "27_0",
                    text: productGroup.Name,
                    checked: false,
                    enable: true,
                    cssClass: "bold"
                });

                continue;
            }
            if (productGroup.Name.toLowerCase() == "mbc-rng casino") {
                var values = '';

                for (var currentProduct = 0; currentProduct < productGroup.ChildsInfo.length; currentProduct++) {
                    var mbcGroup = productGroup.ChildsInfo[currentProduct];
                    var mbcGroupId = mbcGroup.Id;
                    for (var currentGameType = 0; currentGameType < mbcGroup.ChildsInfo.length; currentGameType++) {
                        var bettype = mbcGroup.ChildsInfo[currentGameType];
                        var bettypeId = bettype.Id;
                        values += mbcGroupId + '_' + bettypeId + ',';
                    }
                }


                ddProduct.push({
                    id: "#" + ddId + "_Product251",
                    value: values,
                    text: productGroup.Name,
                    checked: false,
                    enable: true,
                    cssClass: "bold"
                });

                continue;
            }

            if (productGroup.Name.toLowerCase() == "poker games") {
                var porkerSportLength = productGroup.ChildsInfo.length;
                var porkerBetTypeList = [];
                for (var j = 0; j < porkerSportLength; j++) {
                    var porkerSportGroup = productGroup.ChildsInfo[j];
                    var porkerBetTypeLength = porkerSportGroup.ChildsInfo.length;
                    for (var k = 0; k < porkerBetTypeLength; k++) {
                        var betType = porkerSportGroup.ChildsInfo[k];
                        porkerBetTypeList.push({
                            id: "#" + ddId + "_Product_Sport" + porkerSportGroup.Id + "_BetType" + betType.Id,
                            value: porkerSportGroup.Id + "_" + betType.Id,
                            text: betType.Name,
                            checked: false,
                            enable: true
                        });
                    }
                }

                ddProduct.push({
                    id: "#" + ddId + "_Product_Sport" + 163,
                    value: 163,
                    text: productGroup.Name,
                    checked: false,
                    enable: true,
                    items: porkerBetTypeList
                });

                continue;
            }

            var sportLength = productGroup.ChildsInfo.length;
            for (var j = 0; j < sportLength; j++) {
                var sportGroup = productGroup.ChildsInfo[j];
                var betTypeLength = sportGroup.ChildsInfo.length;
                var betTypeList = [];

                for (var k = 0; k < betTypeLength; k++) {
                    var betType = sportGroup.ChildsInfo[k];
                    betTypeList.push({
                        id: "#" + ddId + "_Product_Sport" + sportGroup.Id + "_BetType" + betType.Id,
                        value: sportGroup.Id + "_" + betType.Id,
                        text: betType.Name,
                        checked: false,
                        enable: true
                    });
                }

                ddProduct.push({
                    id: "#" + ddId + "_Product_Sport" + sportGroup.Id,
                    value: sportGroup.Id,
                    text: productGroup.Name,
                    checked: false,
                    enable: true,
                    items: betTypeList
                });
            }
        }

        $("#ddlProduct_HistoryReport").fanexMultiSelect(
        {
            minWidth: 176,
            maxHeight: 450,
            showFilter: true,
            shadow: true,
            showHeader: true,
            isCheckAll: false,
            showFooter: false,
            checkAllText: 'All',
            isUsedCustomCheckAllText: true,
            useCustomCheckAllText: 'All',
            noItemText: noItemText,
            listData: ddProduct,
            onCheck: function () {
                if (this.value == _cachedData.Sportbook_ProductId ||
                    this.value == _cachedData.VirtualSport_GroupId ||
                    this.value == _cachedData.BetradarVirtualSport_GroupId ||
                    this.value == _cachedData.NG2_ProductId ||
                    this.value == _cachedData.NG_ProductId ||
                    this.id == 'ddlProduct_HistoryReportHeaderCheckBox') {
                    self.getDropdownSport();
                }
            },
            onOpen: function () {
                if (isSetDefaultCollapse) {
                    $('#ddlProduct_HistoryReportCollapseButton').trigger('click')
                    isSetDefaultCollapse = false;
                }
            }
        });

        _tables.ddlProduct = $("#ddlProduct_HistoryReport");
    };

    self.renderDropdownSport = function (sports) {
        _cachedData.sports = sports;
        var ddSport = [];
        var ddId = "ddlSport_HistoryReport";
        var selectedProducts = _tables.ddlProduct.fanexMultiSelect('getItems', true);
        var isExistsSportbook = $.inArray(_cachedData.Sportbook_ProductId, selectedProducts) != -1;
        var isExistsVirtualSports = $.inArray(_cachedData.VirtualSport_GroupId, selectedProducts) != -1;
        var isExistsBetradarVirtualSports = $.inArray(_cachedData.BetradarVirtualSport_GroupId, selectedProducts) != -1;
        var isExistsNG2 = $.inArray(_cachedData.NG2_ProductId, selectedProducts) != -1;
        var isExistsNG = $.inArray(_cachedData.NG_ProductId, selectedProducts) != -1;
        var isSetDefaultCollapse = true;
        if (isExistsSportbook || isExistsVirtualSports || isExistsNG2 || isExistsNG || isExistsBetradarVirtualSports) {
            var sportGrouplength = sports ? sports.length : 0;
            for (var i = 0; i < sportGrouplength; i++) {
                var sportGroup = sports[i];
                var sportLength = sportGroup.ChildsInfo ? sportGroup.ChildsInfo.length : 0;
                var ddSportGroup = [];

                if ((_cachedData.Sportbook_SportID.contains(sportGroup.Id) && isExistsSportbook == true) ||
                    (sportGroup.Id <= 218 && sportGroup.Id >= 213 && isExistsNG2 == true) ||
                    ((sportGroup.Id == 161 || sportGroup.Id == 164) && isExistsNG == true) ||
                    (sportGroup.Id <= 199 && sportGroup.Id >= 190 && isExistsBetradarVirtualSports == true) ||
                    (sportGroup.Id <= 186 && sportGroup.Id >= 180 && isExistsVirtualSports == true)) {                    
                    self.generateSportbyProduct(sportGroup, sportLength, ddSportGroup, ddSport, ddId);
                }
            }
        }
        $("#ddlSport_HistoryReport").fanexMultiSelect({
            minWidth: 176,
            maxHeight: 350,
            showFilter: true,
            shadow: true,
            showHeader: true,
            isCheckAll: false,
            showFooter: false,
            checkAllText: 'All',
            isUsedCustomCheckAllText: true,
            useCustomCheckAllText: 'All',
            noneSelectedText: 'Select an option',
            noItemText: 'No sport found.',
            disabled: (!isExistsSportbook && !isExistsVirtualSports && !isExistsNG2 && !isExistsNG && !isExistsBetradarVirtualSports),
            listData: ddSport,
            onCheck: function () {
            },
            onOpen: function () {
                if (isSetDefaultCollapse) {
                    $('#ddlSport_HistoryReportCollapseButton').trigger('click')
                    isSetDefaultCollapse = false;
                }
            }
        });

        _tables.ddlSport = $("#ddlSport_HistoryReport");
        if (!isExistsSportbook && !isExistsVirtualSports && !isExistsNG2 && !isExistsNG && !isExistsBetradarVirtualSports) {
            _tables.ddlSport.fanexMultiSelect('setCheckAll', false);
            _tables.ddlSport.children('input').eq(0).val('Not applicable');
        }
    };

    self.generateSportbyProduct = function (sportGroup, sportLength, ddSportGroup, ddSport, ddId) {
        for (var j = 0; j < sportLength; j++) {
            var sport = sportGroup.ChildsInfo[j];
            var betTypeLength = sport.ChildsInfo ? sport.ChildsInfo.length : 0;
            var ddBetType = [];
            for (var k = 0; k < betTypeLength; k++) {
                var betType = sport.ChildsInfo[k];
                ddBetType.push({
                    id: "#" + ddId + "SportGroup" + sportGroup.Id +
                                            "_Sport" + sport.Id +
                                            "_BetType" + betType.Id,
                    value: sport.Id + "_" + betType.Id,
                    text: betType.Name,
                    checked: true,
                    enable: true,
                    cssClass: ""
                });
            }

            ddSportGroup.push({
                id: "#" + ddId + "SportGroup" + sportGroup.Id + "_Sport" + sport.Id,
                value: (sportGroup.Name != "Parlay" ? sportGroup.Id : "0") + "_" + sport.Id,
                text: sport.Name,
                checked: true,
                enable: true,
                cssClass: "",
                items: ddBetType
            });
        }
        if (sportGroup.Name != "Parlay" || (sportGroup.ChildsInfo && sportGroup.ChildsInfo.length > 0)) {
            ddSport.push({
                id: "#" + ddId + "SportGroup" + sportGroup.Id.toString(),
                value: sportGroup.Id.toString(),
                text: sportGroup.Name,
                checked: true,
                enable: true,
                cssClass: "",
                items: ddSportGroup
            });
        }
    };

    self.btnGenerateClick = function () {
        AccReport.gridSorterManager.resetColumns(_tables.reportDivSorter);
        self.getReport();
    };
    var renderMonthPicker = function () {
        if (_cachedData.isFirstLoad) {
            $('#ddlMonth').monPickr({
                today: today,
                maxDate: today,
                minDate: minHistoryReportDate,
                showDateRange: true,
                fromDayId: 'ddlFromDay',
                toDayId: 'ddlToDay',
                sortDesc: true,
                log: function (msg) {
                    console.warn(msg);
                }
            });

            _tables.monthPicker = $('#ddlMonth');
            _cachedData.isFirstLoad = false;
        }
        else {
            _tables.monthPicker.monPickr("reset");
        }
    };

    self.getReport = function () {
        AccReport.disableInput();
        getParams();
        if (validateParams() == true) {
            $.ajax({
                type: "POST",
                url: window.rootUrl + "Misc/BetCount/GetReport",
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                data: JSON.stringify({
                    Datestart: _params.historyBetCountReport.FromDate,
                    Dateend: _params.historyBetCountReport.ToDate,
                    SportBettype: _params.historyBetCountReport.SportBettype,
                    IsHistoryReport: true,
                    SortBy: _params.historyBetCountReport.SortBy,
                    SortDirection: ''
                }),
                beforeSend: function () {
                    AccReport.disableInput();
                },
                success: function (rs) {
                    self.renderReport(rs);
                },
                error: function (event, jqXHR, ajaxSettings) {
                    self.renderReport(null);
                },
                complete: function () {
                    AccReport.enableInput();
                }
            });
        }
        else {
            AccReport.enableInput();
        }
    };
    var getParams = function () {
        _params.historyBetCountReport.FromDate = _tables.monthPicker.monPickr("getFromDate", FormatConstant.MMM_DD_YY_STRING_MONTH_PICKER);
        _params.historyBetCountReport.ToDate = _tables.monthPicker.monPickr("getToDate", FormatConstant.MMM_DD_YY_STRING_MONTH_PICKER);
        var selectedProducts = _tables.ddlProduct.fanexMultiSelect('getItems', true);
        var selectedSports = _tables.ddlSport.fanexMultiSelect('getItems', true);
        selectedProducts = checkParlay(selectedProducts);
        _params.historyBetCountReport.SportBettype = selectedSports.join(',') + "," + selectedProducts.join(',');
        _cachedData.SelectedSportBettype = selectedSports;
        _params.historyBetCountReport.SortBy = '';
    };

    var checkParlay = function (selectedProducts) {
        for (var i = selectedProducts.length ; i--;) {
            if (selectedProducts[i] === "1_0" || selectedProducts[i] === "5_0" || selectedProducts[i] === "23_0" || selectedProducts[i] === "3_0" || selectedProducts[i] === "27_0") {
                selectedProducts.splice(i, 1);
            }
        }
        return selectedProducts;
    }

    var validateParams = function () {
        var isValid = true;
        var message = '';
        var notifyIds = [];
        var diffTime = moment(_params.historyBetCountReport.FromDate).diff(moment(_params.historyBetCountReport.ToDate));
        if (diffTime > 0) {
            if (message == '') {
                message = "From date must less than to date!";
            }
            isValid = false;
        }

        if (_params.historyBetCountReport.SportBettype.length == 0) {
            if (message == '') {
                message = "Please select at least one product!";
            }
            notifyIds.push("ddlProduct_HistoryReport");
            isValid = false;
        }

        if (!isValid) {
            AccReport.popUp.removeHighLight();
            AccReport.popUp.show(message, notifyIds.join(","));
        } else {
            AccReport.popUp.removeHighLight();
        }

        return isValid;
    };
    self.renderReport = function (report) {
        _cachedData.historyBetCountReport = report;

        var reportTitle = "Bet count report from " + _params.historyBetCountReport.FromDate +
                                          " to " + _params.historyBetCountReport.ToDate;

        _tables.reportTitle.html(reportTitle);

        if (report &&
            report.TransactionList &&
            report.TransactionList.length > 0) {
            _tables.reportExportExcel.removeClass("display-none");
            _tables.reportDivSorter.addClass("divSorter");
            self.resetTempHtml();
            var frozenHtml = '';

            var siteGroupNumber = report.TransactionList.length;
            for (var i = 0; i < siteGroupNumber; i++) {
                var siteGroup = report.TransactionList[i];
                var transactionLength = siteGroup.TransactionList.length;
                var siteGroupTotal = siteGroup.Total;
                self.renderReportRow(siteGroupTotal, true);

                for (var j = 0; j < transactionLength; j++) {
                    var reportItem = siteGroup.TransactionList[j];
                    self.renderReportRow(reportItem);
                }
            }

            self.renderReportTotalRow(report.Total);
            self.removeSpaceIE9TempHtml();
            _tables.reportRowHeader.empty().append(self.TempHtml.frozenHtml);
            _tables.reportContent.empty().append(self.TempHtml.contentHtml);
            _tables.reportRowFooter.empty().append(self.TempHtml.footerFrozenHtml);
            _tables.reportColFooter.empty().append(self.TempHtml.footerContentHtml);
        }
        else {
            self.renderReportNoData();
        }

        _tables.report.removeClass("display-none")
                      .gridScroller(_options.report);

        _tables.reportHeader.removeClass("display-none");
        _tables.report.removeClass("display-none");
    };

    self.renderReportNoData = function () {
        _tables.reportExportExcel.addClass("display-none");
        _tables.reportDivSorter.removeClass("divSorter");
        var frezeeContent = "<td><div>&nbsp</div></td>";
        _tables.reportRowHeader.empty().append("<tr>" + frezeeContent + "</tr>");
        _tables.reportContent.empty().append(
                "<tr>" +
                    frezeeContent +
                    "<td colspan='11'><div class='no-data-message'>There is no data.</div></td>\
                 </tr>");
        _tables.reportRowFooter.empty();
        _tables.reportColFooter.empty();
    };
    self.sortReport = function (column, newSortDirection) {
        var columnId = column.attr("id");
        if (_cachedData.lastSorterBy != null &&
            _cachedData.lastSorterBy.attr("id") != columnId) {
            AccReport.gridSorterManager.resetColumns(_cachedData.lastSorterBy);
        }

        _cachedData.lastSorterBy = column;
        var sortBy = self.Enums.sortBy.None;
        switch (columnId) {
            case 'colHeaderTurnover_HistoryReport':
                sortBy = self.Enums.sortBy.TotalTurnover;
                break;
            case 'colHeaderMemberWinLoss_HistoryReport':
                sortBy = self.Enums.sortBy.MemberWinLoss;
                break;
            case 'colHeaderMarginPercent_HistoryReport':
                sortBy = self.Enums.sortBy.MarginPercent;
                break;
            case 'colHeaderAgentWinLoss_HistoryReport':
                sortBy = self.Enums.sortBy.AgentWinLoss;
                break;
            case 'colHeaderMasterWinLoss_HistoryReport':
                sortBy = self.Enums.sortBy.MasterWinLoss;
                break;
            case 'colHeaderSuperWinLoss_HistoryReport':
                sortBy = self.Enums.sortBy.SuperWinLoss;
                break;
            case 'colHeaderTotal_HistoryReport':
                sortBy = self.Enums.sortBy.TotalWinLoss;
                break;
            case 'colHeaderCompanyWinLoss_HistoryReport':
                sortBy = self.Enums.sortBy.CompanyWinLoss;
                break;
            case 'colHeaderCommission_HistoryReport':
                sortBy = self.Enums.sortBy.Commission;
                break;
            case 'colHeaderExtraPT_HistoryReport':
                sortBy = self.Enums.sortBy.ExtraPT;
                break;
        }

        _params.historyBetCountReport.SortBy = sortBy;
        _params.historyBetCountReport.SortDirection = newSortDirection;

        $.ajax({
            type: "POST",
            url: window.rootUrl + "Misc/BetCount/SortReport",
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            data: JSON.stringify({
                Datestart: _params.historyBetCountReport.FromDate,
                Dateend: _params.historyBetCountReport.ToDate,
                Product: _params.historyBetCountReport.Products,
                IsHistoryReport: true,
                SortBy: sortBy,
                SortDirection: newSortDirection
            }),
            beforeSend: function () {
                AccReport.disableInput();
            },
            success: function (rs) {
                self.renderReport(rs);
            },
            error: function (event, jqXHR, ajaxSettings) {
                self.renderReport(null);
            },
            complete: function () {
                AccReport.enableInput();
            }
        });
    };
    var exportReport = function () {
        var exportParams = {
            Datestart: _params.historyBetCountReport.FromDate,
            Dateend: _params.historyBetCountReport.ToDate,
            Product: _params.historyBetCountReport.Products,
            IsHistoryReport: true,
            SortBy: _params.historyBetCountReport.SortBy,
            SortDirection: ''
        };

        AccReport.downloadFile(window.rootUrl + "Misc/BetCount/ExportReport", exportParams);
    };
    self.showPage = function () {
        AccReport.popUp.removeHighLight();
        _tables.reportFilter.removeClass("display-none");
        renderMonthPicker();
        self.renderDropdownProduct(_cachedData.products);
        AccReport.gridSorterManager.resetColumns(_tables.reportSorter);
        if (_cachedData.historyBetCountReport &&
            _cachedData.historyBetCountReport.TransactionList.length > 0) {
            _tables.reportHeader.removeClass("display-none");
            _tables.report.removeClass("display-none");
        }
    };
    self.hidePage = function () {
        AccReport.popUp.removeHighLight();
        _tables.reportFilter.addClass("display-none");
        _tables.reportHeader.addClass("display-none");
        _tables.report.addClass("display-none");
    };
}
