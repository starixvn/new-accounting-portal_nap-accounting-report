$(document).ready(function () {
    var totalTransactions = new TotalTransactions();
    totalTransactions.initPage();
});


function TotalTransactions() {
    var self = this;
    var tables = {
        totalTransactionsReport: null,
        totalTransactionsReportBody: null,
        totalTransactionsReporFooter: null,
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
    var totalTransactionsReportOpts = {
        tblContentId: 'tblContent',
        manualCopyHeader: true,
        fullHeight: true,
        freezeWidth: 455,
        numberFreezeCol: 0,
        gridWidth: 1355,
        gridHeight: 180
    };
    var totalTransactionsReportParams = {
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
        ShowVS: false,
        SportID: 0,
        Bettype: 0,
        ReportBy: 0,
        GroupBy: 2
    };

    self.initPage = function () {
        getLicenseeByReseller();
        $('#dllMonth').monPickr({
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
        $('#btnGenerate').click(function () {
            getTotalTransactionsByLicensee();
        });
        $('#totalTransactionsReport_ExportExcel').click(function () {
            exportTotalTransactionsByLicensee();
        });
        renderProductsDropdown();
        tables.totalTransactionsReport = $("#totalTransGrid");
        tables.totalTransactionsReportBody = tables.totalTransactionsReport.find("#tblContent tbody");
        tables.totalTransactionsReporFooter = tables.totalTransactionsReport.find("table.col_footer_content thead")
        tables.ddlMonth = $('#dllMonth');
        tables.productsList = {
            sportsbook: $('#chbSport'),
            racing: $('#chbRacing'),
            numbergame: $('#chbNG'),
            liveCasino: $('#chbLiveCasino'),
            virtualSports: $('#chbVirtualSports')
        };

        $("#totalTransGrid").initGrid({
            isShowPager: false
        });

        $(".btExpand").click(function () {
            hideAllResellerGroup($(this));
        });

        //// Assign Title for tooltip
        $("#notemessage").attr("title", noteMessage);
        $("#notemessage").tipsy({
            html: true,
            opacity: 0.9,
            gravity: 'ne'
        });
    };
    getLicenseeByReseller = function () {
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
                tables.ddlSite = $("#ddlSite");
                AccReport.enableInput();
            }
        });
    };
    renderLicenseesDropdown = function (licensees) {
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
            selectedList: 3,
            listData: licensees
        });
    }
    renderProductsDropdown = function () {
        var noItemText = 'No product found.';
        var products = [{ Id: 'chbSport', Text: 'Sportsbook' },
                        { Id: 'chbRacing', Text: 'Racing' },
                        { Id: 'chbNG', Text: 'Number Game' },
                        { Id: 'chbLiveCasino', Text: 'Live Casino' },
                        { Id: 'chbVirtualSports', Text: 'Virtual Sports'}];

        var len = products.length;
        var ddwnProducts = [];
        for (var i = 0; i < len; i++) {
            var product = products[i];
            ddwnProducts.push({
                Id: product.Id,
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
            checkAllText: 'All Products',
            isUsedCustomCheckAllText: true,
            customCheckAllText: 'All Products',
            noItemText: noItemText,
            selectedList: 3,
            listData: ddwnProducts
        });
    };
    checkBoxIsChecked = function (checkBox) {
        return checkBox.attr("checked") ? true : false;
    };
    getTotalTransactionsReportParams = function () {
        totalTransactionsReportParams.NormalLicensee = getSelectedNormalLicensee();
        totalTransactionsReportParams.BALicensee = getSelectedBALicensee();
        totalTransactionsReportParams.NormalLicenseeName = getSelectedNormalLicenseeText();
        totalTransactionsReportParams.BALicenseeName = getSelectedBALicenseeText();
        totalTransactionsReportParams.FromDate = tables.ddlMonth.monPickr("getMonth", "MM/dd/yyyy");
        totalTransactionsReportParams.ShowSB = checkBoxIsChecked(tables.productsList.sportsbook);
        totalTransactionsReportParams.ShowRB = checkBoxIsChecked(tables.productsList.racing);
        totalTransactionsReportParams.ShowNG = checkBoxIsChecked(tables.productsList.numbergame);
        totalTransactionsReportParams.ShowLC = checkBoxIsChecked(tables.productsList.liveCasino);
        totalTransactionsReportParams.ShowVS = checkBoxIsChecked(tables.productsList.virtualSports);
    };
    validateInput = function () {
        var isValid = true;
        var message = '';
        var notifyIds = [];
        if (totalTransactionsReportParams.NormalLicensee.length == 0 &&
            totalTransactionsReportParams.BALicensee.length == 0) {
            isValid = false;
            if (message == '') {
                message = "Please select at least one licensee!";
            }
            notifyIds.push("ddlSite");
        }

        if (totalTransactionsReportParams.ShowSB == false &&
            totalTransactionsReportParams.ShowRB == false &&
            totalTransactionsReportParams.ShowNG == false &&
            totalTransactionsReportParams.ShowLC == false &&
            totalTransactionsReportParams.ShowVS == false) {
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
    getTotalTransactionsByLicensee = function () {
        AccReport.disableInput();
        getTotalTransactionsReportParams();
        if (validateInput() == true) {
            $.ajax({
                type: "POST",
                url: window.rootUrl + "Licensee/TotalTransactions/Index",
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                data: JSON.stringify(totalTransactionsReportParams),
                beforeSend: function () {
                    AccReport.disableInput();
                },
                success: function (rs) {
                    renderTotalTransactionsReport(rs);
                },
                error: function (event, jqXHR, ajaxSettings) {
                    renderTotalTransactionsReport_NoData();
                },
                complete: function () {
                    var selectedLicensee = tables.ddlSite.multiSelect('getCheckedItemsByText');
                    $("#reporttile").html("Total Transactions on " + tables.ddlMonth.monPickr("getMonth", FormatConstant.MMM_YY_String_MONTH_PICKER));
                    var t = setTimeout(function () {
                        tables.totalTransactionsReport.removeClass("display-none")
                                                      .gridScroller(totalTransactionsReportOpts);
                    }, 100);

                    AccReport.enableInput();
                }
            });
        }
        else {
            AccReport.enableInput();
        }
    };
    renderTotalTransactionsReport = function (totalTransactionsReport) {
        if (totalTransactionsReport && totalTransactionsReport.Childs.length > 0) {
            var html = '';
            var len = totalTransactionsReport.Childs.length;
            for (var i = 0; i < len; i++) {
                var reseller = totalTransactionsReport.Childs[i];
                html += renderTotalTransactionsReport_RenderRow(reseller, "ResellerRow", reseller.Name);
                var childsLength = reseller.Childs.length;
                for (var j = 0; j < childsLength; j++) {
                    var licensee = reseller.Childs[j];
                    html += renderTotalTransactionsReport_RenderRow(licensee, "LicenseeRow", reseller.Name);
                }
            }
            tables.totalTransactionsReporFooter.empty().append(renderTotalTransactionsReport_RenderRow(totalTransactionsReport, "TotalRow", ""))
            tables.totalTransactionsReportBody.empty().append(html);
        }
        else {
            renderTotalTransactionsReport_NoData();
        }
    };
    hideAllResellerGroup = function (btnExpand) {
        if (btnExpand.hasClass("collapsed")) {
            btnExpand.removeClass("collapsed");
            $("#tblContent [class*='reseller_']").removeClass("display-none");
            $("#tblContent .resellerRow .collapse").removeClass("collapse").addClass("expand");
            btnExpand.addClass("expanded");
        }
        else {
            btnExpand.removeClass("expanded");
            $("#tblContent [class*='reseller_']").addClass("display-none");
            $("#tblContent .expand").removeClass("expand").addClass("collapse");
            btnExpand.addClass("collapsed");
        }
        tables.totalTransactionsReport.gridScroller(totalTransactionsReportOpts);
    };
    hideResellerGroup = function (btnExpand, reseller) {
        if (btnExpand.hasClass("collapse")) {
            btnExpand.removeClass("collapse");
            $("#tblContent .reseller_" + reseller).removeClass("display-none");
            btnExpand.addClass("expand");
        }
        else {
            btnExpand.removeClass("expand");
            $("#tblContent .reseller_" + reseller).addClass("display-none");
            btnExpand.addClass("collapse");
        }
        tables.totalTransactionsReport.gridScroller(totalTransactionsReportOpts);
    };
    renderTotalTransactionsReport_RenderRow = function (transaction, rowType, resellerName) {
        var cssClass = "";
        var html = '';
        var betCount = numeral(transaction.BetCount).format(FormatConstant.IntNumber);
        switch (rowType) {
            case "ResellerRow":
                cssClass = "resellerRow";
                html = '<tr class="' + cssClass + '">\
                            <td><div class="col100"><span class="expand" onclick="hideResellerGroup($(this),\'' + (transaction.Name || '') + '\');"></span>' + transaction.Name + '</div></td>\
                            <td><div class="align-right colRemark">' + betCount + '</div></td>\
                            <td><div class="align-right col100">' + transaction.EURCostString + '</div></td>\
                            <td><div class="align-right col100">' + transaction.EURCostString + '</div></td>\
                            <td><div class="align-right col100">' + transaction.EURCostString + '</div></td>\
                            <td><div class="align-right col100">' + transaction.EURCostString + '</div></td>\
                            <td><div class="align-right col80">' + transaction.EURCostString + '</div></td>\
                            <td><div class="align-right col80">' + transaction.EURCostString + '</div></td>\
                            <td><div class="align-right col100">' + transaction.EURCostString + '</div></td>\
                            <td><div class="align-right col55">' + transaction.EURCostString + '</div></td>\
                            <td><div class="align-right col55">' + transaction.EURCostString + '</div></td>\
                        </tr>';
                break;
            case "LicenseeRow":
                cssClass = "licenseeRow reseller_" + resellerName;
                html = '<tr class="' + cssClass + '">\
                            <td><div class="col100"><span>' + transaction.Name + '</span></div></td>\
                             <td><div class="align-right colRemark">' + transaction.EURCostString + '</div></td>\
                            <td><div class="align-right col100">' + transaction.EURCostString + '</div></td>\
                            <td><div class="align-right col100">' + transaction.EURCostString + '</div></td>\
                            <td><div class="align-right col100">' + transaction.EURCostString + '</div></td>\
                            <td><div class="align-right col100">' + transaction.EURCostString + '</div></td>\
                            <td><div class="align-right col80">' + transaction.EURCostString + '</div></td>\
                            <td><div class="align-right col80">' + transaction.EURCostString + '</div></td>\
                            <td><div class="align-right col100">' + transaction.EURCostString + '</div></td>\
                            <td><div class="align-right col55">' + transaction.EURCostString + '</div></td>\
                            <td><div class="align-right col55">' + transaction.EURCostString + '</div></td>\
                        </tr>';
                break;
            case "TotalRow":
                cssClass = "totalRow";
                html = '<tr class="' + cssClass + '">\
                            <th><div class="col100">' + transaction.Name + '</th>\
                            <th><div class="align-right colRemark">' + betCount + '</div></th>\
                            <th><div class="align-right col100">' + transaction.EURCostString + '</div></th>\
                            <td><div class="align-right col100">' + transaction.EURCostString + '</div></td>\
                            <td><div class="align-right col100">' + transaction.EURCostString + '</div></td>\
                            <td><div class="align-right col100">' + transaction.EURCostString + '</div></td>\
                            <td><div class="align-right col80">' + transaction.EURCostString + '</div></td>\
                            <td><div class="align-right col80">' + transaction.EURCostString + '</div></td>\
                            <td><div class="align-right col100">' + transaction.EURCostString + '</div></td>\
                            <td><div class="align-right col55">' + transaction.EURCostString + '</div></td>\
                            <td><div class="align-right col55">' + transaction.EURCostString + '</div></td>\
                        </tr>';
                break;
        }
        return html;
    };
    renderTotalTransactionsReport_NoData = function () {
        var html = '<tr>\
                    <td colspan="11" class="bold-weight align-center">There is no data.</td>\
                </tr>';
        tables.totalTransactionsReportBody.empty().append(html);
        tables.totalTransactionsReporFooter.empty();
    };
    exportTotalTransactionsByLicensee = function () {
        var parameters = {
            NormalLicensee: totalTransactionsReportParams.NormalLicensee,
            BALicensee: totalTransactionsReportParams.BALicensee,
            NormalLicenseeName: totalTransactionsReportParams.NormalLicenseeName,
            BALicenseeName: totalTransactionsReportParams.BALicenseeName,
            FromDate: totalTransactionsReportParams.FromDate,
            ShowSB: totalTransactionsReportParams.ShowSB,
            ShowRB: totalTransactionsReportParams.ShowRB,
            ShowNG: totalTransactionsReportParams.ShowNG,
            ShowLC: totalTransactionsReportParams.ShowLC,
            ShowVS: totalTransactionsReportParams.ShowVS
        };
        AccReport.downloadFile(window.rootUrl + "Licensee/TotalTransactions/Export", parameters);
    };
    getSelectedNormalLicensee = function () {
        var listChecked = tables.ddlSite.next('div.ui-multiselect-options')
                                        .find('label:not(.ui-multiselect-state-disabled,.ui-multiselect-group-mark)')
                                        .find('input:not(#' + tables.ddlSite.attr('id') + "HeaderCheckBox)")
                                        .filter(":not([id^='ddlSite_Reseller3'])[checked]");
        var tempArray = [];
        for (var i = 0; i < listChecked.length; i++) {
            tempArray.push(listChecked[i].value);
        }
        return tempArray.join(",");
    };
    getSelectedNormalLicenseeText = function () {
        var listChecked = tables.ddlSite.next('div.ui-multiselect-options')
                                        .find('label:not(.ui-multiselect-state-disabled,.ui-multiselect-group-mark)')
                                        .find('input:not(#' + tables.ddlSite.attr('id') + "HeaderCheckBox)")
                                        .filter(":not([id^='ddlSite_Reseller3'])[checked]");
        var tempArray = [];
        for (var i = 0; i < listChecked.length; i++) {
            tempArray.push(listChecked[i].title);
        }
        return tempArray.join(",");
    };
    getSelectedBALicensee = function () {
        var listChecked = tables.ddlSite.next('div.ui-multiselect-options')
                                        .find('label:not(.ui-multiselect-state-disabled,.ui-multiselect-group-mark)')
                                        .find('input:not(#' + tables.ddlSite.attr('id') + "HeaderCheckBox)")
                                        .filter("[id^='ddlSite_Reseller3'][checked]");
        var tempArray = [];
        for (var i = 0; i < listChecked.length; i++) {
            tempArray.push(listChecked[i].value);
        }
        return tempArray.join(",");
    };
    getSelectedBALicenseeText = function () {
        var listChecked = tables.ddlSite.next('div.ui-multiselect-options')
                                        .find('label:not(.ui-multiselect-state-disabled,.ui-multiselect-group-mark)')
                                        .find('input:not(#' + tables.ddlSite.attr('id') + "HeaderCheckBox)")
                                        .filter("[id^='ddlSite_Reseller3'][checked]");
        var tempArray = [];
        for (var i = 0; i < listChecked.length; i++) {
            tempArray.push(listChecked[i].title);
        }
        return tempArray.join(",");
    };
}