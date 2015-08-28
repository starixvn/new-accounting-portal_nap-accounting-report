Namespace.Register("Fanex.AccountingReports.Licensee.TotalTransactions.TotalTransactions");
Fanex.AccountingReports.Licensee.TotalTransactions.TotalTransactions = function () {
    var self = this;

    var _tables = {
        btnGenerate: null,
        report: null,
        ddlSite: null,
        ddlMonth: null,
        productsList: {
            sportsbook: null,
            racing: null,
            numbergame: null,
            liveCasino: null,
            virtualSports: null
        }
    };

    var _options = {
        filterNote: {
            html: true,
            opacity: 0.9,
            gravity: 'ne'
        },

        report: {
            isShowPager: false,
            tblContentId: 'tblContent',
            manualCopyHeader: true,
            fullHeight: true,
            freezeWidth: 0,
            numberFreezeCol: 0,
            gridWidth: 495,
            gridHeight: 180
        }
    };

    var _params = {
        btnGenerate: null,
        NormalLicensee: "",
        BALicensee: "",
        NormalLicenseeName: "",
        BALicenseeName: "",
        FromDate: new Date(),
        ToDate: new Date(),
        ShowSB: true,
        ShowRB: true,
        ShowNG: true,
        ShowLC: true,
        ShowVS: true,
        ShowCS: true,
        ShowKN: true,
        ShowGD: true,
        ShowAGCS: true,
        ShowVG: true,
        SportID: 0,
        Bettype: 0,
        ReportBy: 0,
        GroupBy: 2
    };

    self.initPage = function () {
        initGridScrollerTables();
        cachedDOMElements();

        getLicenseeByReseller();
        renderMonthPicker();
        renderProductsDropdown();
        renderFilterNote(noteMessage);
        bindingEvent();
    };

    var checkBoxIsChecked = function (checkBox) {
        return checkBox.attr("checked") ? true : false;
    };
    var initGridScrollerTables = function () {
        $("#totalTransGrid").initGrid({
            isShowPager: false
        });
    };
    var cachedDOMElements = function () {
        _tables.btnGenerate = $('#btnGenerate');
        _tables.filterNote = $("#notemessage");
        _tables.report = $("#totalTransReport");
    };
    var getLicenseeByReseller = function () {
        $.ajax({
            type: "POST",
            url: window.rootUrl + "Licensee/TotalTransactions/GetAllResellers",
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            data: null,
            beforeSend: function () {
                AccReport.disableInput();
            },
            success: function (rs) {
                var resellers = [];
                var id = "ddlSite";
                for (var i = 0; i < rs.length; i++) {
                    var reseller = rs[i];
                    var licensees = [];
                    for (var j = 0; j < reseller.Licensees.length; j++) {
                        var licensee = reseller.Licensees[j];
                        licensees.push({
                            Id: id + "_Reseller" + reseller.Id.toString() + "_Licensee" + licensee.Id.toString(),
                            Value: licensee.Id,
                            Text: licensee.Name,
                            Checked: true,
                            Enable: true,
                            CssClass: "",
                            Items: []
                        });
                    }

                    resellers.push({
                        Id: id + "_Reseller" + reseller.Id.toString(),
                        Value: reseller.Id,
                        Text: reseller.Name,
                        Checked: true,
                        Enable: true,
                        CssClass: "",
                        Items: licensees
                    });
                }

                renderLicenseesDropdown(resellers);
            },
            error: function (event, jqXHR, ajaxSettings) {
                renderLicenseesDropdown([]);
            },
            complete: function () {
                _tables.ddlSite = $("#ddlSite");
                AccReport.enableInput();
            }
        });
    };
    var renderLicenseesDropdown = function (licensees) {
        var noItemText = '';
        if (licensees.length == 0) {
            noItemText = 'No licensee found.';
        }

        $("#ddlSite").multiSelect(
        {
            minWidth: 170,
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
    }
    var renderMonthPicker = function () {
        $('#ddlMonth').monPickr({
            today: toDay,
            maxDate: toDay,
            minDate: minDate,
            showDateRange: false,
            fromDayId: '',
            toDayId: '',
            sortDesc: true,
            log: function (msg) {
                console.warn(msg);
            }
        });

        _tables.ddlMonth = $('#ddlMonth');
    };
    var renderProductsDropdown = function () {
        var noItemText = 'No product found.';
        var products = [{ Id: 'chbSport', Text: 'Sportsbook' },
                        { Id: 'chbRacing', Text: 'Racing' },
                        { Id: 'chbNG', Text: 'Number Game' },
                        { Id: 'chbLiveCasino', Text: 'Live Casino' },
                        { Id: 'chbGoldDeluxe', Text: 'Gold Deluxe' },
                        { Id: 'chbAGCasino', Text: 'AG Casino' },
                        { Id: 'chbVirtualSports', Text: 'Virtual Sports' },
                        { Id: 'chbRNGCasino', Text: 'RNG Casino' },
                        { Id: 'chbMacau', Text: 'Macau' },
                        { Id: 'chbKN', Text: 'Keno'}];

        var len = products.length;
        var ddwnProducts = [];
        var prefixId = 'ddlProduct_';
        for (var i = 0; i < len; i++) {
            var product = products[i];
            ddwnProducts.push({
                Id: prefixId + product.Id,
                Value: product.Id,
                Text: product.Text,
                Checked: true,
                Enable: true,
                CssClass: "",
                Items: []
            });
        }

        $("#ddlProduct").multiSelect(
        {
            minWidth: 140,
            maxHeight: 450,
            shadow: true,
            showHeader: true,
            isCheckAll: true,
            showFooter: false,
            showTooltip: true,
            checkAllText: 'All Products',
            isUsedCustomCheckAllText: true,
            customCheckAllText: 'All Products',
            noItemText: noItemText,
            listData: ddwnProducts
        });

        _tables.productsList = {
            sportsbook: $('#ddlProduct_chbSport'),
            racing: $('#ddlProduct_chbRacing'),
            numbergame: $('#ddlProduct_chbNG'),
            liveCasino: $('#ddlProduct_chbLiveCasino'),
            virtualSports: $('#ddlProduct_chbVirtualSports'),
            rngCasino: $('#ddlProduct_chbRNGCasino'),
            keno: $('#ddlProduct_chbKN'),
            goldDeluxe: $('#ddlProduct_chbGoldDeluxe'),
            agCasino: $('#ddlProduct_chbAGCasino'),
            macau: $('#ddlProduct_chbMacau')
        };
    };
    var renderFilterNote = function (noteMessage) {
        //// Assign Title for tooltip
        _tables.filterNote.attr("title", noteMessage);
        _tables.filterNote.tipsy(_options.filterNote);
    };
    var bindingEvent = function () {
        _tables.btnGenerate.click(function () {
            getReport();
        });
    };
    var getReport = function () {
        AccReport.disableInput();
        getParams();
        if (validateParams() == true) {
            $.ajax({
                type: "POST",
                url: window.rootUrl + "Licensee/TotalTransactions/Index",
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                data: JSON.stringify(_params),
                beforeSend: function () {
                    AccReport.disableInput();
                },
                success: function (rs) {
                    renderReport(rs);
                },
                error: function (event, jqXHR, ajaxSettings) {
                    renderReport(null);
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
        _params.NormalLicensee = getSelectedNormalLicensee();
        _params.BALicensee = getSelectedBALicensee();
        _params.NormalLicenseeName = getSelectedNormalLicenseeText();
        _params.BALicenseeName = getSelectedBALicenseeText();
        _params.FromDate = _tables.ddlMonth.monPickr("getMonth", FormatConstant.MM_DD_YYYY_WITH_SLASH_STRING);
        _params.ShowSB = checkBoxIsChecked(_tables.productsList.sportsbook);
        _params.ShowRB = checkBoxIsChecked(_tables.productsList.racing);
        _params.ShowNG = checkBoxIsChecked(_tables.productsList.numbergame);
        _params.ShowLC = checkBoxIsChecked(_tables.productsList.liveCasino);
        _params.ShowVS = checkBoxIsChecked(_tables.productsList.virtualSports);
        _params.ShowCS = checkBoxIsChecked(_tables.productsList.rngCasino);
        _params.ShowKN = checkBoxIsChecked(_tables.productsList.keno);
        _params.ShowGD = checkBoxIsChecked(_tables.productsList.goldDeluxe);
        _params.ShowAGCS = checkBoxIsChecked(_tables.productsList.agCasino);
        _params.ShowVG = checkBoxIsChecked(_tables.productsList.macau);
    };
    var validateParams = function () {
        var isValid = true;
        var message = '';
        var notifyIds = [];
        if (_params.NormalLicensee.length == 0 &&
            _params.BALicensee.length == 0) {
            isValid = false;
            if (message == '') {
                message = "Please select at least one licensee!";
            }
            notifyIds.push("ddlSite");
        }

        if (_params.ShowSB == false &&
            _params.ShowRB == false &&
            _params.ShowNG == false &&
            _params.ShowLC == false &&
            _params.ShowVS == false &&
            _params.ShowCS == false &&
            _params.ShowKN == false &&
            _params.ShowAGCS == false &&
            _params.ShowVG == false &&
            _params.ShowGD == false) {
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
    var renderReport = function (report) {
        _tables.report.addClass("display-none");
        var bodyData = [];
        var footerData = [];
        if (report && report.Childs.length > 0) {
            var len = report.Childs.length;
            for (var i = 0; i < len; i++) {
                var reseller = report.Childs[i];
                var currentRow =
                {
                    Columns: [{ Name: "ColLicensee", Value: reseller.Name },
                              { Name: "ColBetCount", Value: reseller.BetCount },
                              { Name: "ColEURCost", Value: reseller.EURCostString}],
                    SubRows: []
                };

                var childsLength = reseller.Childs.length;
                for (var j = 0; j < childsLength; j++) {
                    var licensee = reseller.Childs[j];
                    var currentSubRow =
                    {
                        Columns: [{ Name: "ColLicensee", Value: licensee.Name, CssClass: "colLicensee " },
                        { Name: "ColBetCount", Value: licensee.BetCount },
                        { Name: "ColEURCost", Value: licensee.EURCostString}],
                        CssClass: "licenseeRow "
                    };

                    currentRow.SubRows.push(currentSubRow);
                }

                bodyData.push(currentRow);
            }

            footerData.push({
                Columns: [{ Name: "ColLicensee", Value: report.Name, CustomCellRender: function (currentCell) {
                    return '<div class="align-center">' + currentCell.Value + '</div>';
                }
                },
                { Name: "ColBetCount", Value: report.BetCount },
                { Name: "ColEURCost", Value: report.EURCostString}],
                CssClass: "totalRow"
            });
        }

        //        var selectedLicensee = _tables.ddlSite.multiSelect('getCheckedItemsByText');
        var gridOptions = {
            columns: [{ HeaderText: "Licensee", Width: 160, Name: "ColLicensee", HeaderAlign: "center", CellAlign: "left", ShowCollapseExpand: true },
                      { HeaderText: "Total Transactions", Width: 160, Name: "ColBetCount", HeaderAlign: "center", CellAlign: "right", FormatNumber: FormatConstant.INT_NUMBER },
                      { HeaderText: "Transaction Cost (EUR)", Width: 160, Name: "ColEURCost", HeaderAlign: "center", CellAlign: "right", FormatNumber: FormatConstant.TWO_DECIMAL_NUMBER}],
            showTitle: true,
            showFooter: true,
            titleInfo:
            {
                text: "Total Transactions on " + _tables.ddlMonth.monPickr("getMonth", FormatConstant.MMM_YY_STRING_MONTH_PICKER),
                showExcelButton: true,
                exportExcelFunction: function () {
                    exportReport();
                },
                showCollapseExpandButton: true
            },
            showTree: true,
            bodyRows: bodyData,
            showDistinctRow: false,
            footerRows: footerData
        };

        _tables.report.removeClass("display-none").fanexGrid(gridOptions);
    };
    var exportReport = function () {
        var parameters = {
            NormalLicensee: _params.NormalLicensee,
            BALicensee: _params.BALicensee,
            NormalLicenseeName: _params.NormalLicenseeName,
            BALicenseeName: _params.BALicenseeName,
            FromDate: _params.FromDate,
            ShowSB: _params.ShowSB,
            ShowRB: _params.ShowRB,
            ShowNG: _params.ShowNG,
            ShowLC: _params.ShowLC,
            ShowVS: _params.ShowVS,
            ShowCS: _params.ShowCS,
            ShowKN: _params.ShowKN,
            ShowAGCS: _params.ShowAGCS,
            ShowGD: _params.ShowGD,
            ShowVG: _params.ShowVG
        };

        AccReport.downloadFile(window.rootUrl + "Licensee/TotalTransactions/Export", parameters);
    };
    var getSelectedNormalLicensee = function () {
        var listChecked = _tables.ddlSite.next('div.ui-multiselect-options')
                                        .find('label:not(.ui-multiselect-state-disabled,.ui-multiselect-group-mark)')
                                        .find('input:not(#' + _tables.ddlSite.attr('id') + "HeaderCheckBox)")
                                        .filter(":not([id^='ddlSite_Reseller3'])[checked]");
        var tempArray = [];
        for (var i = 0; i < listChecked.length; i++) {
            tempArray.push(listChecked[i].value);
        }
        return tempArray.join(",");
    };
    var getSelectedNormalLicenseeText = function () {
        var listChecked = _tables.ddlSite.next('div.ui-multiselect-options')
                                        .find('label:not(.ui-multiselect-state-disabled,.ui-multiselect-group-mark)')
                                        .find('input:not(#' + _tables.ddlSite.attr('id') + "HeaderCheckBox)")
                                        .filter(":not([id^='ddlSite_Reseller3'])[checked]");
        var tempArray = [];
        for (var i = 0; i < listChecked.length; i++) {
            tempArray.push(listChecked[i].title);
        }
        return tempArray.join(",");
    };
    var getSelectedBALicensee = function () {
        var listChecked = _tables.ddlSite.next('div.ui-multiselect-options')
                                        .find('label:not(.ui-multiselect-state-disabled,.ui-multiselect-group-mark)')
                                        .find('input:not(#' + _tables.ddlSite.attr('id') + "HeaderCheckBox)")
                                        .filter("[id^='ddlSite_Reseller3'][checked]");
        var tempArray = [];
        for (var i = 0; i < listChecked.length; i++) {
            tempArray.push(listChecked[i].value);
        }
        return tempArray.join(",");
    };
    var getSelectedBALicenseeText = function () {
        var listChecked = _tables.ddlSite.next('div.ui-multiselect-options')
                                        .find('label:not(.ui-multiselect-state-disabled,.ui-multiselect-group-mark)')
                                        .find('input:not(#' + _tables.ddlSite.attr('id') + "HeaderCheckBox)")
                                        .filter("[id^='ddlSite_Reseller3'][checked]");
        var tempArray = [];
        for (var i = 0; i < listChecked.length; i++) {
            tempArray.push(listChecked[i].title);
        }
        return tempArray.join(",");
    };
}