var LiveCasinoWinLoss = {
    fromDate: '',
    toDate: '',
    normalLicensse: [],
    operator: [],
    gameType: 0,
    Title: '',
    specificPeriodIndex: 7,
    maxHeightOfLicenseesDropdown: 400,
    minWidthOfLicenseesDropdown: 150,
    selectLicensees: function () {
        $("#InformationMssg").empty();
        //        try { parent.BlockUI(); } catch (e) { }
        $.ajax({
            type: "POST",
            url: window.rootUrl + "Licensee/LiveCasinoWinLoss/SelAllLicensee",
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            data: null,
            beforeSend: function () {
                AccReport.disableInput();
            },
            success: function (rs) {
                if (rs && rs.length > 0) {
                    LiveCasinoWinLoss.renderLicensees(rs);
                }
                else {
                    LiveCasinoWinLoss.clearAllDataWhenLincenseeEmpty();
                }
            },
            complete: function () {
                AccReport.enableInput();
            },
            error: function () {
                LiveCasinoWinLoss.clearAllDataWhenLincenseeEmpty();
            }

        });
    },

    selectGameType: function () {
        $.ajax({
            type: "POST",
            url: "SelGameType",
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            data: null,
            success: function (rs) {
                var idGameType = $('#ddlGameType');
                idGameType.append('<option value="0">All</option>');
                var length = rs.length;
                for (var i = 0; i < length; i++) {
                    idGameType.append('<option value="' + rs[i].Id + '">' + rs[i].Name + '</option>');
                }
            }
        });
    },

    clearAllDataWhenLincenseeEmpty: function () {
        $("#ddlLicenseese").multiSelect({
            minWidth: LiveCasinoWinLoss.minWidthOfLicenseesDropdown,
            maxHeight: LiveCasinoWinLoss.maxHeightOfLicenseesDropdown,
            shadow: true,
            showHeader: true,
            isCheckAll: true,
            showFooter: false,
            listData: [],
            noItemText: 'No licensee found.'
        });
        //$("#InformationMssg").showErrorMessage("No Licensee found.");
    },

    renderLicensees: function (licensees) {
        var licenseesTempData = [];
        var totalLicensees = licensees.length;
        var oldResellerId = 0;
        var oldResellerName = '';
        var distributors = [];
        var licenseeByReseller = [];
        var oldOperatorResellerId = 0;
        for (var i = 0; i < totalLicensees; i++) {
            var licensee = licensees[i];
            var currentResellerId = licensee.ResellerId;
            var currentResellerName = licensee.ResellerName;
            var currentResellerType = licensee.ResellerType;
            var licenseeId = licensee.LicenseeId;
            var licenseeName = licensee.LicenseeName;

            if (currentResellerType == 1) {
                if (oldResellerId != 0 && currentResellerId != oldResellerId) {
                    distributors.push({
                        Id: "#ddlLicenseese" + oldResellerId,
                        Value: oldResellerId,
                        Text: oldResellerName,
                        Checked: true,
                        Enable: true,
                        CssClass: "",
                        Items: licenseeByReseller
                    });
                    licenseeByReseller = [];
                    oldResellerId = 0;
                    oldResellerName = '';
                }
                licenseeByReseller.push({
                    Id: "#ddlLicenseeseDetail" + licenseeId,
                    Value: licenseeId,
                    Text: licenseeName,
                    Checked: true,
                    Enable: true,
                    CssClass: ""
                });
                oldResellerId = currentResellerId;
                oldResellerName = currentResellerName;
            }
            else if (currentResellerType == 2) {
                if (currentResellerId != oldOperatorResellerId) {
                    licenseesTempData.push({
                        Id: "#ddlLicenseese" + currentResellerId,
                        Value: currentResellerId,
                        Text: currentResellerName,
                        Checked: true,
                        Enable: true,
                        CssClass: "",
                        Items: []
                    });
                }
                oldOperatorResellerId = currentResellerId;
            }
        }
        //assign latest licensee group
        if (licenseeByReseller && licenseeByReseller.length > 0) {
            distributors.push({
                Id: "#ddlLicenseese" + oldResellerId,
                Value: oldResellerId,
                Text: oldResellerName,
                Checked: true,
                Enable: true,
                CssClass: "",
                Items: licenseeByReseller
            });
            licenseeByReseller = [];
        }
        //append the distributors to main data
        licenseesTempData = licenseesTempData.concat(distributors);
        $("#ddlLicenseese").multiSelect(
        {
            minWidth: LiveCasinoWinLoss.minWidthOfLicenseesDropdown,
            maxHeight: LiveCasinoWinLoss.maxHeightOfLicenseesDropdown,
            shadow: true,
            showHeader: true,
            isCheckAll: true,
            showFooter: false,
            checkAllText: 'All Licensees',
            isUsedCustomCheckAllText: true,
            customCheckAllText: 'All Licensees',
            listData: licenseesTempData
        });
    },

    getLicenseesSelected: function () {
        var listChecked = $('#ddlLicenseese').next('div.ui-multiselect-options').find('label:not(.ui-multiselect-state-disabled,.ui-multiselect-group-mark)').find('input:not(#' + $('#ddlLicenseese').attr('id') + "HeaderCheckBox)").filter('[checked]');
        var normalLicensees = [];
        var operator = [];
        for (var i = 0; i < listChecked.length; i++) {
            if (listChecked[i].id.indexOf('Detail') != -1) {
                normalLicensees.push(listChecked[i].value);
            }
            else {
                operator.push(listChecked[i].value);
            }
        }
        var result = { 'normal': normalLicensees, 'operator': operator }
        return result;
    },

    selWinLossReport: function () {
        var licenseesSeleted = LiveCasinoWinLoss.getLicenseesSelected();
        LiveCasinoWinLoss.normalLicensse = licenseesSeleted.normal;
        LiveCasinoWinLoss.operator = licenseesSeleted.operator;
        var dateRange = $('#datepickerplugin').nexDatePicker("getDateRange");
        LiveCasinoWinLoss.fromDate = dateRange.FromDate;
        LiveCasinoWinLoss.toDate = dateRange.ToDate;
        LiveCasinoWinLoss.gameType = $('#ddlGameType').val();

        $("#InformationMssg").empty();
        $('#livecasinowinloss').addClass('hidden');

        if ($('#ddlLicenseese').multiSelect('getCheckedItems').length > 0) {
            AccReport.popUp.removeHighLight();
            $.ajax({
                type: "POST",
                url: window.rootUrl + "Licensee/LiveCasinoWinLoss/SelLiveCasinoWinLossReport",
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                data: '{ "normalLicensee":"' + LiveCasinoWinLoss.normalLicensse.join(',') + '", "operator":"' + LiveCasinoWinLoss.operator.join(',') + '", "fromDate":"' + LiveCasinoWinLoss.fromDate + '", "toDate":"' + LiveCasinoWinLoss.toDate + '", "gameType":"' + LiveCasinoWinLoss.gameType + '"}',
                beforeSend: function () {
                    AccReport.disableInput();
                },
                success: function (rs) {
                    AccReport.popUp.hide();
                    var arr = [];
                    var arrLeft = [];

                    $(".grid-title").removeClass("display-none");
                    var gameTypeValue = $('#ddlGameType').find('option:selected').text();
                    gameTypeValue = gameTypeValue == 'All' ? 'All Game Types' : gameTypeValue;
                    LiveCasinoWinLoss.Title = 'Live Casino Win Loss Report from ' + LiveCasinoWinLoss.fromDate + ' to ' + LiveCasinoWinLoss.toDate + ' - ' + gameTypeValue;
                    $('#reporttile').html(LiveCasinoWinLoss.Title);
                    if (rs && rs.ResellersWinLoss && rs.ResellersWinLoss.length > 0) {
                        var totalGroups = rs.ResellersWinLoss.length;
                        for (var i = 0; i < totalGroups; i++) {
                            var groupData = rs.ResellersWinLoss[i];
                            arr.push(LiveCasinoWinLoss.renderRevenueInfo(groupData.RevenueInfo));
                            arrLeft.push(LiveCasinoWinLoss.renderRevenueInfoLeftCols(groupData.RevenueInfo));
                            if (groupData.Resellers && groupData.Resellers.length > 0) {
                                var totalResellers = groupData.Resellers.length;
                                for (var j = 0; j < totalResellers; j++) {
                                    var licensee = groupData.Resellers[j];
                                    arr.push(LiveCasinoWinLoss.renderTotal(licensee.WinLossTotal));
                                    arrLeft.push(LiveCasinoWinLoss.renderTotalLeftCols(licensee.WinLossTotal));

                                    if (licensee.LicenseeWinLossDetail && licensee.LicenseeWinLossDetail.length > 0) {
                                        var totalRows = licensee.LicenseeWinLossDetail.length;
                                        for (var k = 0; k < totalRows; k++) {
                                            var detail = licensee.LicenseeWinLossDetail[k];
                                            arr.push(LiveCasinoWinLoss.renderDetail(detail));
                                            arrLeft.push(LiveCasinoWinLoss.renderDetailLeftCols(detail));
                                        }
                                    }
                                }
                            }

                            if (i < (totalGroups - 1)) {
                                arr.push(LiveCasinoWinLoss.renderEmptyRow());
                                arrLeft.push(LiveCasinoWinLoss.renderEmptyRow_LeftCols());
                            }
                        }

                        $('#livecasinowinloss').find(".col_footer_content thead").empty().append(LiveCasinoWinLoss.renderFooter(rs.GrandTotal));
                        $('#livecasinowinloss').find(".row_footer_content thead").empty().append(LiveCasinoWinLoss.renderFooterLeftCols(rs.GrandTotal));
                        $('#tblContent tbody').empty().append(arr.join(''));
                        $('#tblContentHeader tbody').empty().append(arrLeft.join(''));
                        $(".grid-toolbox").removeClass("display-none");
                    }
                    else {
                        arr.push(LiveCasinoWinLoss.renderNoData());
                        arrLeft.push(LiveCasinoWinLoss.renderNoDataLeft());
                        $('#tblContent tbody').empty().append(arr.join(''));
                        $('#tblContentHeader tbody').empty().append(arrLeft.join(''));
                        $('#livecasinowinloss').find(".col_footer_content thead").empty(); //.html(NumberGameWinLoss.renderNoData());
                        $('#livecasinowinloss').find(".row_footer_content thead").empty(); //.html(NumberGameWinLoss.renderNoData());
                        $(".grid-toolbox").addClass("display-none");
                    }

                    var gridOpts = {
                        tblContentId: 'tblContent',
                        manualCopyHeader: true,
                        fullWidth: true,
                        fullHeight: true,
                        gridHeight: 180,
                        freezeWidth: 435,
                        numberFreezeCol: 4
                    };
                    $('#livecasinowinloss').removeClass("display-none").gridScroller(gridOpts);
                    $("#InformationMssg").hide();
                },
                error: function (jqXHR, event, ajaxSettings) {
                    if (jqXHR.status &&
                        jqXHR.status == AccReportData.Constants.HtmlErrorCode.SystemError) {
                        var r = jQuery.parseJSON(jqXHR.responseText);
                        AccReport.popUp.show(r.message);
                    }
                },
                complete: function () {
                    AccReport.enableInput();
                }
            });
        }
        else {
            AccReport.popUp.show(selectLicenseeMessage, "ddlLicenseese");
        }

    },
    renderRevenueInfo: function (row) {
        var html = '<tr class="revenueInfo">\
            <td>\
                <div class="align-center width100">&nbsp;</div>\
            </td>\
            <td>\
                <div class="align-right width120">&nbsp;</div>\
            </td>\
            <td>\
                <div class="align-right width100">&nbsp;</div>\
            </td>\
            <td>\
                <div class="align-right width100">&nbsp;</div>\
            </td>\
            <td>\
                <div class="align-center b i width100">' + row.DistributorRevenue + '</div>\
            </td>\
            <td>\
                <div class="align-center b i width170">' + row.StarExpressRevenue + '</div>\
            </td>\
            <td>\
                <div class="align-center b i width200">' + row.AmountDueToOneWorks + '</div>\
            </td>\
        </tr>';
        return html;
    },
    renderRevenueInfoLeftCols: function (row) {
        var html = '<tr>\
            <td style="border-right:none 0px;">\
                <div class="align-center width100">&nbsp;</div>\
            </td>\
            <td style="border-right:none 0px;>\
                <div class="align-right width120">&nbsp;</div>\
            </td>\
            <td style="border-right:none 0px;>\
                <div class="align-right width100">&nbsp;</div>\
            </td>\
            <td>\
                <div class="align-right width100">&nbsp;</div>\
            </td>\
        </tr>';
        return html;
    },
    renderTotal: function (row) {
        var html = '<tr>\
            <td>\
                <div class="align-center b width100">' + row.Licensee + '</div>\
            </td>\
            <td>\
                <div class="align-right width120">' + row.Turnover + '</div>\
            </td>\
            <td>\
                <div class="align-right width100">' + row.TotalTransaction + '</div>\
            </td>\
            <td>\
                <div class="align-right width100">' + row.GrossWin + '</div>\
            </td>\
            <td>\
                <div class="' + (row.DistributorRevenue == '-' ? 'align-center' : 'align-right') + ' b width100">' + row.DistributorRevenue + '</div>\
            </td>\
            <td>\
                <div class="align-right b width170">' + row.StarExpressRevenue + '</div>\
            </td>\
            <td>\
                <div class="align-right b width200">' + row.AmountDueToOneWorks + '</div>\
            </td>\
        </tr>';
        return html;
    },
    renderTotalLeftCols: function (row) {
        var html = '<tr>\
            <td>\
                <div class="align-left b width100">' + row.Licensee + '</div>\
            </td>\
            <td>\
                <div class="align-right width120">' + row.Turnover + '</div>\
            </td>\
            <td>\
                <div class="align-right width100">' + row.TotalTransaction + '</div>\
            </td>\
            <td>\
                <div class="align-right width100">' + row.GrossWin + '</div>\
            </td>\
        </tr>';
        return html;
    },
    renderDetail: function (row) {
        var html = '<tr class="winlossdetail totalRow">\
            <td>\
                <div class="align-center width100">' + row.Licensee + '</div>\
            </td>\
            <td>\
                <div class="align-right width120">' + row.Turnover + '</div>\
            </td>\
            <td>\
                <div class="align-right width100">' + row.TotalTransaction + '</div>\
            </td>\
            <td>\
                <div class="align-right width100">' + row.GrossWin + '</div>\
            </td>\
            <td>\
                <div class="align-right width100">' + row.DistributorRevenue + '</div>\
            </td>\
            <td>\
                <div class="align-right width170">' + row.StarExpressRevenue + '</div>\
            </td>\
            <td>\
                <div class="align-right width200">' + row.AmountDueToOneWorks + '</div>\
            </td>\
        </tr>';
        return html;
    },
    renderDetailLeftCols: function (row) {
        var html = '<tr class="winlossdetail totalRow">\
            <td>\
                <div class="align-right width100">' + row.Licensee + '</div>\
            </td>\
            <td>\
                <div class="align-right width120">' + row.Turnover + '</div>\
            </td>\
            <td>\
                <div class="align-right width100">' + row.TotalTransaction + '</div>\
            </td>\
            <td>\
                <div class="align-right width100">' + row.GrossWin + '</div>\
            </td>\
        </tr>';
        return html;
    },
    renderEmptyRow: function () {
        var html = '<tr class="hideborderright">\
            <td>\
                <div class="align-center width100">&nbsp;</div>\
            </td>\
            <td>\
                <div class="align-right width120">&nbsp;</div>\
            </td>\
            <td>\
                <div class="align-right width100">&nbsp;</div>\
            </td>\
            <td>\
                <div class="align-right width100">&nbsp;</div>\
            </td>\
            <td>\
                <div class="align-right width100">&nbsp;</div>\
            </td>\
            <td>\
                <div class="align-right width170">&nbsp;</div>\
            </td>\
            <td>\
                <div class="align-right width200">&nbsp;</div>\
            </td>\
        </tr>';
        return html;
    },
    renderEmptyRow_LeftCols: function () {
        var html = '<tr class="hideborderright">\
            <td>\
                <div class="align-center width100">&nbsp;</div>\
            </td>\
            <td>\
                <div class="align-right width120">&nbsp;</div>\
            </td>\
            <td>\
                <div class="align-right width100">&nbsp;</div>\
            </td>\
            <td>\
                <div class="align-right width100">&nbsp;</div>\
            </td>\
        </tr>';
        return html;
    },
    renderFooter: function (row) {
        var html = '<tr class="totalRow">\
            <th>\
                <div class="align-center width100">&nbsp;</div>\
            </th>\
            <th>\
                <div class="align-right width120">&nbsp;</div>\
            </th>\
            <th>\
                <div class="align-right width100">&nbsp;</div>\
            </th>\
            <th >\
                <div class="align-right width100">&nbsp;</div>\
            </th>\
            <th>\
                <div class="align-right b width100">Total</div>\
            </th>\
            <th>\
                <div class="align-right b width170">' + row.StarExpressRevenue + '</div>\
            </th>\
            <th>\
                <div class="align-right b width200">' + row.AmountDueToOneWorks + '</div>\
            </th>\
        </tr>';
        return html;
    },
    renderFooterLeftCols: function (row) {
        var html = '<tr class="hideborderright totalRow">\
            <th>\
                <div class="align-center width100">&nbsp;</div>\
            </th>\
            <th>\
                <div class="align-right width120">&nbsp;</div>\
            </th>\
            <th>\
                <div class="align-right width100">&nbsp;</div>\
            </th>\
            <th>\
                <div class="align-right width100">&nbsp;</div>\
            </th>\
        </tr>';
        return html;
    },
    renderNoDataLeft: function () {
        var html = '<tr class="hideborderright">\
            <td>\
                <div class="align-center width100">&nbsp;</div>\
            </td>\
            <td>\
                <div class="align-right width120">&nbsp;</div>\
            </td>\
            <td>\
                <div class="align-right width100">&nbsp;</div>\
            </td>\
            <td>\
                <div class="align-right width100">&nbsp;</div>\
            </td>\
        </tr>';
        return html;
    },
    renderNoData: function () {
        var html = '<tr class="hideborderright">\
            <td>\
                <div class="align-center width100">&nbsp;</div>\
            </td>\
            <td>\
                <div class="align-right width120">&nbsp;</div>\
            </td>\
            <td>\
                <div class="align-right width100">&nbsp;</div>\
            </td>\
            <td>\
                <div class="align-right width100">&nbsp;</div>\
            </td>\
            <td>\
                <div class="align-right b width100 no-data-not-frozen">There is no data</div>\
            </td>\
            <td>\
                <div class="align-right width170">&nbsp;</div>\
            </td>\
            <td>\
                <div class="align-right width200">&nbsp;</div>\
            </td>\
        </tr>';
        return html;
    },
    exportExcel: function () {
        var parameters = $.param({
            normallicensee: LiveCasinoWinLoss.normalLicensse.join(','),
            operator: LiveCasinoWinLoss.operator,
            fromDate: LiveCasinoWinLoss.fromDate,
            toDate: LiveCasinoWinLoss.toDate,
            gameType: LiveCasinoWinLoss.gameType,
            Title: LiveCasinoWinLoss.Title
        });
        AccReport.downloadFile(window.rootUrl + "Licensee/LiveCasinoWinLoss/ExportLiveCasinoWinLossReport", parameters);
    }
};

$(document).ready(function () {
    $("#ddlDateSelection").change(function (event) {
        LiveCasinoWinLoss.setReportDateByCurrentSelection();
    });


    LiveCasinoWinLoss.selectLicensees();
    LiveCasinoWinLoss.selectGameType();

    var gridOpts = {
        tblContentId: 'tblContent',
        isShowPager: false
    };
    $("#livecasinowinloss").initGrid(gridOpts);
    $("#notemessage").tipsy(
        {
            html: true,
            opacity: 0.9,
            gravity: 'ne'
        }
    );
    $('#datepickerplugin').nexDatePicker({
        nexDatePickerType: "quickchoose",
        datePickerFormat: {
            changeMonth: true,
            changeYear: true,
            changeMonth: true,
            dateFormat: "M dd, yy"
        },
        firstDatePicker: {
            Id: "firstDatePickerId1",
            Label: labelFrom,
            InitDate: _today,
            LabelCss: "lbl-field col_2",
            TextboxCss: "edit-field col_5"
        },
        secondDatePicker: {
            Id: "secondDatePickerId1",
            Label: labelTo,
            InitDate: _today,
            LabelCss: "lbl-field col_1",
            TextboxCss: "edit-field col_6"
        },
        onSelectDatePicker: function () {
        },
        maxDate: new Date(_today)
    });

    $('#btnGenerate').click(function () {
        LiveCasinoWinLoss.selWinLossReport();
    });

    $('#btnExport').click(function () {
        LiveCasinoWinLoss.exportExcel();
    });
});