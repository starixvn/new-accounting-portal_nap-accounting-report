Namespace.Register("Fanex.AccountingReports.Misc.BetCount.NormalReport");
Fanex.AccountingReports.Misc.BetCount.NormalReport = function () {
    var self = this;
    //var _historyBetCountReport = new Fanex.AccountingReports.Misc.BetCount.HistoryReport();

    var _tables = {
        reportFilter: null,
        filterNote: null,
        ddlProduct: null,
        ddlSport: null,
        datepickerplugin: null,
        btnGenerate: null,
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
        filterNote: {
            html: true,
            opacity: 0.9,
            gravity: 'ne'
        },

        report: {
            isShowPager: false,
            tblContentId: 'betCountReportContent',
            manualCopyHeader: true,
            fullHeight: true,
            fullWidth: true,
            gridWidth: 1230,
            gridHeight: 195
        }
    };

    var _params = {
        betCountReport: {
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
        betCountReport: null,
        lastSorterBy: null,
        products: null,
        sports: null,
        isFirstLoad: true,
        Sportbook_ProductId: "1",
        VirtualSport_GroupId: "5",
        BetradarVirtualSport_GroupId: "27",
        NG2_ProductId: "23",
        NG_ProductId: "3",
        Sports: [],
        Sportbook_SportID: [1, 999, -9, 2, 43, -999],
        SelectedSportBettype: [],
        BetBuilder_ProductId: "-38",
        productSelectedIds: ["1"],
        BetBuilder_SportID: [1, 2],
        isSelectedAllProduct: false,
        SabaClub_ProductId: "42",
        SabaClub_SportID: [150, 200],
    };

    var productsHasSport = ["1", "5", "23", "27", "3", "-38", "42"];

    self.initPage = function () {
        initGridScrollerTables();
        cachedDOMElements();
        renderDatePicker();
        renderFilterNote(noteMessage);
        bindingEvent();
    };
    var initGridScrollerTables = function () {
        $("#betCountReport").initGrid({
            isShowPager: false
        });
    };
    var cachedDOMElements = function () {
        _tables.filterNote = $("#notemessage");
        _tables.btnGenerate = $("#btnGenerate");
        _tables.reportFilter = $("#betCountReportFilter");

        _tables.report = $("#betCountReport");
        _tables.reportHeader = $("#betCountReportHeader");
        _tables.reportContent = _tables.report.find("#betCountReportContent tbody");
        _tables.reportTitle = _tables.reportHeader.find("#betCountReportTitle");
        _tables.reportExportExcel = _tables.reportHeader.find("#betCountReport_ExportExcel");
        _tables.reportRowHeader = _tables.report.find("#betCountReportRowHeader tbody");
        _tables.reportRowFooter = _tables.report.find(".row_footer_content thead");
        _tables.reportColFooter = _tables.report.find(".col_footer_content thead");
        _tables.reportPagination = _tables.report.find("#pagination");
        _tables.reportSorter = _tables.report.find(".colSorter");
        _tables.reportDivSorter = _tables.report.find(".divSorter");
    };
    var renderFilterNote = function (noteMessage) {
        _tables.filterNote.attr("title", noteMessage);
        _tables.filterNote.tipsy(_options.filterNote);
    };
    var renderDatePicker = function () {
        $('#datepickerplugin').nexDatePicker({
            nexDatePickerType: "quickchoose",
            datePickerFormat: {
                changeMonth: true,
                changeYear: true,
                dateFormat: FormatConstant.M_DD_YY_STRING,
                minDate: mindate
            },
            dropDownList:
            {
                Id: "dropDownListId",
                Label: "Date ",
                LabelCss: "lbl-field col_2",
                OptionCss: "edit-field col_6"
            },
            firstDatePicker: {
                Id: "firstDatePickerId1",
                Label: labelFrom,
                InitDate: today,
                LabelCss: "lbl-field col_4",
                TextboxCss: "edit-field col_6"
            },
            secondDatePicker: {
                Id: "secondDatePickerId1",
                Label: labelTo,
                InitDate: today,
                LabelCss: "lbl-field col_3",
                TextboxCss: "edit-field col_6"
            },
            selectedItem: 1,
            onSelectDatePicker: function () {
            },
            maxDate: new Date(today)
        });

        _tables.datepickerplugin = $("#datepickerplugin");
    };

    self.getDropdownSport = function (productIds) {
        var self = this;
        var data = { ProductID: productIds };
        $.ajax({
            type: "POST",
            url: window.rootUrl + "Misc/BetCount/GetSports",
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            data: JSON.stringify(data),
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
        var ddId = "ddlProduct";
        var noItemText = 'No product found.';
        var productLength = products ? products.length : 0;
        var isSetDefaultCollapse = true;
        for (var i = 0; i < productLength; i++) {
            var productGroup = products[i];
            if (productGroup.Name.toLowerCase() == "sportsbook") {
                ddProduct.push({
                    id: "#" + ddId + "_Product1",
                    value: "1",
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
                    value: "23",
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
                    value: "3",
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
                    value: "5",
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
                    value: "27",
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
                    id: "#" + ddId + "_Product_Sport_Porker",
                    value: "Poker",
                    text: productGroup.Name,
                    checked: false,
                    enable: true,
                    items: porkerBetTypeList
                });

                continue;
            }

            if (productGroup.Name.toLowerCase() == "bet builder") {
                ddProduct.push({
                    id: "#" + ddId + "_Product-38",
                    value: "-38",
                    text: productGroup.Name,
                    checked: false,
                    enable: true,
                    cssClass: "bold"
                });

                continue;
            }

            if (productGroup.Name.toLowerCase() == "saba.club") {
                ddProduct.push({
                    id: "#" + ddId + "_Product42",
                    value: "42",
                    text: productGroup.Name,
                    checked: false,
                    enable: true,
                    cssClass: "bold"
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

                var isMetricGaming = productGroup.Name == "Metric Gaming - Super Live";

                ddProduct.push({
                    id: "#" + ddId + "_Product_Sport" + sportGroup.Id,
                    value: isMetricGaming ? -1 : sportGroup.Id,
                    text: productGroup.Name,
                    checked: false,
                    enable: true,
                    items: betTypeList
                });
            }
        }

        $("#ddlProduct").fanexMultiSelect(
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
                var productId = this.value;

                if (this.id == 'ddlProductHeaderCheckBox') {
                    if (_cachedData.isSelectedAllProduct) {
                        _cachedData.productSelectedIds = [];
                        _cachedData.isSelectedAllProduct = false;
                    } else {
                        _cachedData.productSelectedIds = ["1", "5", "23", "27", "3", "-38", "42"];
                        _cachedData.isSelectedAllProduct = true;
                    }
                } else {
                    if ($.inArray(productId, productsHasSport) != -1) {
                        if ($.inArray(productId, _cachedData.productSelectedIds) != -1) {
                            _cachedData.productSelectedIds = $.grep(_cachedData.productSelectedIds, function (value) {
                                return value != productId;
                            });
                        } else {
                            _cachedData.productSelectedIds.push(productId);
                        }
                    }

                    if (_cachedData.isSelectedAllProduct) {
                        _cachedData.isSelectedAllProduct = false;
                    }
                }

                var productIds = _cachedData.productSelectedIds.join(",");
                self.getDropdownSport(productIds);
            },
            onOpen: function () {
                if (isSetDefaultCollapse) {
                    $('#ddlProductCollapseButton').trigger('click')
                    isSetDefaultCollapse = false;
                }
            }
        });

        _tables.ddlProduct = $("#ddlProduct");
    };

    self.renderDropdownSport = function (sports) {
        _cachedData.sports = sports;
        var ddSport = [];
        var ddId = "ddlSport";
        var selectedProducts = _tables.ddlProduct.fanexMultiSelect('getItems', true);
        var isExistsSportbook = $.inArray(_cachedData.Sportbook_ProductId, selectedProducts) != -1;
        var isExistsVirtualSports = $.inArray(_cachedData.VirtualSport_GroupId, selectedProducts) != -1;
        var isExistsBetradarVirtualSports = $.inArray(_cachedData.BetradarVirtualSport_GroupId, selectedProducts) != -1;
        var isExistsNG2 = $.inArray(_cachedData.NG2_ProductId, selectedProducts) != -1;
        var isExistsNG = $.inArray(_cachedData.NG_ProductId, selectedProducts) != -1;
        var isExistsBetBuilder = $.inArray(_cachedData.BetBuilder_ProductId, selectedProducts) != -1;
        var isExistsSabaClub = $.inArray(_cachedData.SabaClub_ProductId, selectedProducts) != -1;
        var isSetDefaultCollapse = true;
        if (isExistsSportbook || isExistsVirtualSports || isExistsNG2 || isExistsNG || isExistsBetradarVirtualSports || isExistsBetBuilder || isExistsSabaClub) {
            var sportGrouplength = sports ? sports.length : 0;
            for (var i = 0; i < sportGrouplength; i++) {
                var sportGroup = sports[i];
                var sportLength = sportGroup.ChildsInfo ? sportGroup.ChildsInfo.length : 0;
                var ddSportGroup = [];

                if ((_cachedData.Sportbook_SportID.contains(sportGroup.Id) && isExistsSportbook == true) ||
                    (sportGroup.Id <= 218 && sportGroup.Id >= 213 && isExistsNG2 == true) ||
                    ((sportGroup.Id == 161 || sportGroup.Id == 164) && isExistsNG == true) ||
                    (sportGroup.Id <= 199 && sportGroup.Id >= 190 && isExistsBetradarVirtualSports == true) ||
                    (sportGroup.Id <= 186 && sportGroup.Id >= 180 && isExistsVirtualSports == true) ||
                    (_cachedData.BetBuilder_SportID.contains(sportGroup.Id) && isExistsBetBuilder == true) ||
                    (_cachedData.SabaClub_SportID.contains(sportGroup.Id) && isExistsSabaClub == true)) {
                    self.generateSportbyProduct(sportGroup, sportLength, ddSportGroup, ddSport, ddId);
                }
            }
        }

        $("#ddlSport").fanexMultiSelect({
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
            disabled: (!isExistsSportbook && !isExistsVirtualSports && !isExistsNG2 && !isExistsNG && !isExistsBetradarVirtualSports && !isExistsBetBuilder && !isExistsSabaClub),
            listData: ddSport,
            onCheck: function () {
            },
            onOpen: function () {
                if (isSetDefaultCollapse) {
                    $('#ddlSportCollapseButton').trigger('click')
                    isSetDefaultCollapse = false;
                }
            }
        });

        _tables.ddlSport = $("#ddlSport");
        if (!isExistsSportbook && !isExistsVirtualSports && !isExistsNG2 && !isExistsNG && !isExistsBetradarVirtualSports && !isExistsBetBuilder && !isExistsSabaClub) {
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

    var bindingEvent = function () {
        _tables.btnGenerate.click(function () {
            self.btnGenerateClick();
        });

        _tables.reportExportExcel.click(function () {
            exportReport();
        });

        _tables.reportSorter.click(function () {
            if (_cachedData.betCountReport &&
                _cachedData.betCountReport.TransactionList.length > 0) {
                AccReport.gridSorterManager.sortColumn($(this).find(".divSorter"), self.sortReport);
            }
        });
    };
    self.btnGenerateClick = function () {
        AccReport.gridSorterManager.resetColumns(_tables.reportDivSorter);
        self.getReport();
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
                    Datestart: _params.betCountReport.FromDate,
                    Dateend: _params.betCountReport.ToDate,
                    SportBettype: _params.betCountReport.SportBettype,
                    IsHistoryReport: false,
                    SortBy: _params.betCountReport.SortBy,
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
        var dateRange = _tables.datepickerplugin.nexDatePicker("getDateRange");
        _params.betCountReport.FromDate = dateRange.FromDate;
        _params.betCountReport.ToDate = dateRange.ToDate;
        var selectedProducts = _tables.ddlProduct.fanexMultiSelect('getItems', true);
        var selectedSports = _tables.ddlSport.fanexMultiSelect('getItems', true);
        selectedProducts = checkParlay(selectedProducts);
        _params.betCountReport.SportBettype = selectedSports.join(',') + "," + selectedProducts.join(',');
        _cachedData.SelectedSportBettype = selectedSports;
        _params.betCountReport.SortBy = '';
    };

    var checkParlay = function (selectedProducts) {
        for (var i = selectedProducts.length ; i--;) {
            if (selectedProducts[i] === "1"
                || selectedProducts[i] === "5"
                || selectedProducts[i] === "23"
                || selectedProducts[i] === "3"
                || selectedProducts[i] === "27"
                || selectedProducts[i] === "-38"
                || selectedProducts[i] === "42") {
                selectedProducts.splice(i, 1);
            }
        }
        return selectedProducts;
    }

    var validateParams = function () {
        var isValid = true;
        var message = '';
        var notifyIds = [];
        if (_params.betCountReport.SportBettype.length == 0) {
            if (message == '') {
                message = "Please select at least one product!";
            }
            notifyIds.push("ddlProduct");
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
        _cachedData.betCountReport = report;

        var reportTitle = "Bet count report from " + _params.betCountReport.FromDate +
                                          " to " + _params.betCountReport.ToDate;

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
            case 'colHeaderTurnover':
                sortBy = self.Enums.sortBy.TotalTurnover;
                break;
            case 'colHeaderMemberWinLoss':
                sortBy = self.Enums.sortBy.MemberWinLoss;
                break;
            case 'colHeaderMarginPercent':
                sortBy = self.Enums.sortBy.MarginPercent;
                break;
            case 'colHeaderAgentWinLoss':
                sortBy = self.Enums.sortBy.AgentWinLoss;
                break;
            case 'colHeaderMasterWinLoss':
                sortBy = self.Enums.sortBy.MasterWinLoss;
                break;
            case 'colHeaderSuperWinLoss':
                sortBy = self.Enums.sortBy.SuperWinLoss;
                break;
            case 'colHeaderTotalWinloss':
                sortBy = self.Enums.sortBy.TotalWinLoss;
                break;
            case 'colHeaderCompanyWinLoss':
                sortBy = self.Enums.sortBy.CompanyWinLoss;
                break;
            case 'colHeaderCommission':
                sortBy = self.Enums.sortBy.Commission;
                break;
            case 'colHeaderExtraPT':
                sortBy = self.Enums.sortBy.ExtraPT;
                break;
        }

        _params.betCountReport.SortBy = sortBy;
        _params.betCountReport.SortDirection = newSortDirection;

        $.ajax({
            type: "POST",
            url: window.rootUrl + "Misc/BetCount/SortReport",
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            data: JSON.stringify({
                Datestart: _params.betCountReport.FromDate,
                Dateend: _params.betCountReport.ToDate,
                Product: _params.betCountReport.Products,
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
            Datestart: _params.betCountReport.FromDate,
            Dateend: _params.betCountReport.ToDate,
            Product: _params.betCountReport.Products,
            IsHistoryReport: false,
            SortBy: _params.betCountReport.SortBy,
            SortDirection: ''
        };

        AccReport.downloadFile(window.rootUrl + "Misc/BetCount/ExportReport", exportParams);
    };
    self.hidePage = function () {
        AccReport.popUp.removeHighLight();
        _tables.reportFilter.addClass("display-none");
        _tables.filterNote.addClass("display-none");
        _tables.reportHeader.addClass("display-none");
        _tables.report.addClass("display-none");
    };
    self.showPage = function () {
        AccReport.popUp.removeHighLight();
        _tables.reportFilter.removeClass("display-none");
        _tables.filterNote.removeClass("display-none");
        if (_cachedData.betCountReport &&
            _cachedData.betCountReport.TransactionList.length > 0) {
            _tables.reportHeader.removeClass("display-none");
            _tables.report.removeClass("display-none");
        }
    };
}