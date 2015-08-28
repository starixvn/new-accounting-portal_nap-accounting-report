/* File Created: November 8, 2012 */
var NumberGameWinLoss = {
    fromDate: '',
    toDate: '',
    normalLicensse: [],
    operators: [],
    specificPeriodIndex: 7,
    maxHeightOfLicenseesDropdown: 400,
    minWidthOfLicenseesDropdown: 150,
    selNumberGameReportCriteria:
    {
        NormalLicensee: "",
        Operator: "",
        FromDate: "",
        ToDate: ""
    },
    selectLicensees: function () {
        $("#InformationMssg").empty();
        $.ajax({
            type: "POST",
            url: window.rootUrl + "Licensee/NumberGameWinLoss/SelAllLicensee",
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            data: null,
            beforeSend: function () {
                AccReport.disableInput();
            },
            success: function (rs) {
                if (rs && rs.length > 0) {
                    NumberGameWinLoss.renderLicensees(rs);
                }
                else {
                    NumberGameWinLoss.clearAllDataWhenLincenseeEmpty();
                }
            },
            error: function () {
                NumberGameWinLoss.clearAllDataWhenLincenseeEmpty();
            },
            complete: function () {
                AccReport.enableInput();
            }
        });
    },
    clearAllDataWhenLincenseeEmpty: function () {
        $("#ddlLicenseese").multiSelect({
            minWidth: NumberGameWinLoss.minWidthOfLicenseesDropdown,
            maxHeight: NumberGameWinLoss.maxHeightOfLicenseesDropdown,
            shadow: true,
            showHeader: true,
            isCheckAll: true,
            showFooter: false,
            listData: [],
            noItemText: 'No licensee found.'
        });
        //$("#inforMessage").showErrorMessage("No Licensee found.");
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
            minWidth: NumberGameWinLoss.minWidthOfLicenseesDropdown,
            maxHeight: NumberGameWinLoss.maxHeightOfLicenseesDropdown,
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
        var operators = [];
        for (var i = 0; i < listChecked.length; i++) {
            if (listChecked[i].id.indexOf('Detail') != -1) {
                normalLicensees.push(listChecked[i].value);
            }
            else {
                operators.push(listChecked[i].value);
            }
        }
        var result = { 'normal': normalLicensees, 'operators': operators }
        return result;
    },
    getNumberGameWinLossCriteria: function () {
        var licenseesSeleted = NumberGameWinLoss.getLicenseesSelected();
        NumberGameWinLoss.selNumberGameReportCriteria.NormalLicensee = licenseesSeleted.normal.join(",");
        NumberGameWinLoss.selNumberGameReportCriteria.Operator = licenseesSeleted.operators.join(",");
        var dateRange = $('#datepickerplugin').nexDatePicker("getDateRange");
        NumberGameWinLoss.selNumberGameReportCriteria.FromDate = dateRange.FromDate;
        NumberGameWinLoss.selNumberGameReportCriteria.ToDate = dateRange.ToDate;
    },
    selWinLossReport: function () {
        NumberGameWinLoss.getNumberGameWinLossCriteria();
        $("#InformationMssg").empty();
        $('#numbergamewinloss').addClass('display-none');

        ///AccReport.PopUp.Show
        if ($('#ddlLicenseese').multiSelect('getCheckedItems').length > 0) {
            AccReport.popUp.removeHighLight();
            $.ajax({
                type: "POST",
                url: window.rootUrl + "Licensee/NumberGameWinLoss/SelNumberGameWinLossReport",
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                data: JSON.stringify(NumberGameWinLoss.selNumberGameReportCriteria),
                beforeSend: function () {
                    AccReport.disableInput();
                },
                success: function (rs) {
                    $('#reporttile').html('Number Game Win Loss Report from ' + NumberGameWinLoss.selNumberGameReportCriteria.FromDate + ' to ' + NumberGameWinLoss.selNumberGameReportCriteria.ToDate);
                    var arr = [];
                    var arrLeft = [];
                    $(".grid-title").removeClass("display-none");
                    if (rs && rs.LicenseesWinLoss && rs.LicenseesWinLoss.length > 0) {

                        var totalGroups = rs.LicenseesWinLoss.length;

                        for (var i = 0; i < totalGroups; i++) {
                            var groupData = rs.LicenseesWinLoss[i];
                            arr.push(NumberGameWinLoss.renderRevenueInfo(groupData.RevenueInfo));
                            arrLeft.push(NumberGameWinLoss.renderRevenueInfoLeftCols(groupData.RevenueInfo));
                            if (groupData.Resellers && groupData.Resellers.length > 0) {
                                var totalLicensees = groupData.Resellers.length;
                                for (var j = 0; j < totalLicensees; j++) {
                                    var licensee = groupData.Resellers[j];
                                    arr.push(NumberGameWinLoss.renderTotal(licensee.WinLossTotal));
                                    arrLeft.push(NumberGameWinLoss.renderTotalLeftCols(licensee.WinLossTotal));

                                    if (licensee.LicenseeWinLossDetail && licensee.LicenseeWinLossDetail.length > 0) {
                                        var totalRows = licensee.LicenseeWinLossDetail.length;
                                        for (var k = 0; k < totalRows; k++) {
                                            var detail = licensee.LicenseeWinLossDetail[k];
                                            arr.push(NumberGameWinLoss.renderDetail(detail, "totalRow"));
                                            arrLeft.push(NumberGameWinLoss.renderDetailLeftCols(detail, "totalRow"));
                                        }
                                    }
                                }
                            }

                            if (i < (totalGroups - 1)) {
                                arr.push(NumberGameWinLoss.renderEmptyRow());
                                arrLeft.push(NumberGameWinLoss.renderEmptyRow_LeftCols());
                            }
                        }

                        $('#numbergamewinloss').find(".col_footer_content thead").empty().append(NumberGameWinLoss.renderFooter(rs.GrandTotal));
                        $('#numbergamewinloss').find(".row_footer_content thead").empty().append(NumberGameWinLoss.renderFooterLeftCols(rs.GrandTotal));

                        $('#tblContent tbody').empty().append(arr.join(''));

                        $('#tblContentHeader tbody').empty().append(arrLeft.join(''));
                        $(".grid-toolbox").removeClass("display-none");
                    }
                    else {
                        arr.push(NumberGameWinLoss.renderNoData());
                        arrLeft.push(NumberGameWinLoss.renderNoDataLeft());
                        $('#tblContent tbody').empty().append(arr.join(''));
                        $('#tblContentHeader tbody').empty().append(arrLeft.join(''));
                        $('#numbergamewinloss').find(".col_footer_content thead").empty(); //.html(NumberGameWinLoss.renderNoData());
                        $('#numbergamewinloss').find(".row_footer_content thead").empty(); //.html(NumberGameWinLoss.renderNoData());
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
                    $('#numbergamewinloss').removeClass("display-none").gridScroller(gridOpts);
                    $("#inforMessage").hide();
                },
                error: function () {
                    //$("#inforMessage").showErrorMessage("There is no information available.");
                },
                complete: function () {
                    AccReport.enableInput();
                }
            });
        }
        else {
            AccReport.popUp.show(_selectLicenseeMessage, "ddlLicenseese");
        }
    },
    renderRevenueInfo: function (row) {
        var dataTemplate = doT.template($("#tblContent_Template_Revenue_Info").html());
        return dataTemplate(row);
    },
    renderRevenueInfoLeftCols: function (row) {
        var dataTemplate = doT.template($("#revenue_left_template").html());
        return dataTemplate(row);

    },
    renderTotal: function (row) {
        var dataTemplate = doT.template($("#tblContent_Template_Total_Row").html());
        return dataTemplate(row);
    },
    renderTotalLeftCols: function (row) {
        var dataTemplate = doT.template($("#row_total_left_template").html());
        return dataTemplate(row);
    },
    renderDetail: function (row, cssClass) {
        var dataTemplate = doT.template($("#tblContent_Template").html());
        var result = $("<div></div>").append(dataTemplate(row));
        if (cssClass) {
            result.find("> tr").addClass(cssClass);
        }

        return result.html();
    },
    renderDetailLeftCols: function (row, cssClass) {
        var dataTemplate = doT.template($("#row_header_Template").html());
        var result = $("<div></div>").append(dataTemplate(row));
        if (cssClass) {
            result.find("> tr").addClass(cssClass);
        }

        return result.html();
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
                <div class="align-right width150">&nbsp;</div>\
            </td>\
            <td>\
                <div class="align-right width200">&nbsp;</div>\
            </td>\
        </tr>';
        return html;
    },
    renderEmptyRow_LeftCols: function () {
        var dataTemplate = doT.template($("#row_empty_left_template").html());
        return dataTemplate();
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
                <div class="align-right b width150">' + row.LupineRevenue + '</div>\
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
                <div class="align-right b width100">There is no data</div>\
            </td>\
            <td>\
                <div class="align-right width150">&nbsp;</div>\
            </td>\
            <td>\
                <div class="align-right width200">&nbsp;</div>\
            </td>\
        </tr>';
        return html;
    },
    exportExcel: function () {
        var parameters = $.param(NumberGameWinLoss.selNumberGameReportCriteria);
        AccReport.downloadFile(window.rootUrl + "Licensee/NumberGameWinLoss/ExportGameWinLossReport", parameters);
    }
};



$(document).ready(function () {
    $("#ddlDateSelection").change(function (event) {
        NumberGameWinLoss.setReportDateByCurrentSelection();
    });

    NumberGameWinLoss.selectLicensees();

    var gridOpts = {
        tblContentId: 'tblContent',
        isShowPager: false
    };
    $("#numbergamewinloss").initGrid(gridOpts);

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
            showOtherMonths: true,
            selectOtherMonths: true,
            changeMonth: true,
            changeYear: true,
            dateFormat: "M dd, yy"
        },
        firstDatePicker: {
            Id: "firstDatePickerId1",
            Label: _labelFrom,
            InitDate: _today,
            LabelCss: "lbl-field col_2",
            TextboxCss: "edit-field col_5"
        },
        secondDatePicker: {
            Id: "secondDatePickerId1",
            Label: _labelTo,
            InitDate: _today,
            LabelCss: "lbl-field col_1",
            TextboxCss: "edit-field col_6"
        },
        onSelectDatePicker: function () {
        },
        maxDate: new Date(_today)
    });

    $('#btnGenerate').click(function () {
        NumberGameWinLoss.selWinLossReport();
    });

    $('#btnExport').click(function () {
        NumberGameWinLoss.exportExcel();
    });
});