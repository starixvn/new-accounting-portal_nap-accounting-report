/* File Created: November 8, 2012 */
var VirtualSportWinLoss = {
    fromDate: '',
    toDate: '',
    normalLicensse: [],
    operators: [],
    specificPeriodIndex: 7,
    maxHeightOfLicenseesDropdown: 400,
    minWidthOfLicenseesDropdown: 170,
    gridOpts: {
        tblContentId: 'tblContent',
        manualCopyHeader: true,
        fullWidth: true,
        fullHeight: true,
        gridHeight: 180,
        column:[100,100,100,120,120,200,200],
        numberFreezeCol: 4
    },
    selectLicensees: function () {
        $("#InformationMssg").empty();
        $.ajax({
            type: "POST",
            url: window.rootUrl + "Licensee/VirtualSportWinLoss/SelAllLicensee",
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            data: null,
            beforeSend: function () {
                AccReport.disableInput();
            },
            success: function (rs) {
                if (rs && rs.length > 0) {
                    VirtualSportWinLoss.renderLicensees(rs);
                } else {
                    VirtualSportWinLoss.clearAllDataWhenLincenseeEmpty();
                }
            },
            error: function () {
                VirtualSportWinLoss.clearAllDataWhenLincenseeEmpty();
            },
            complete: function () {
                AccReport.enableInput();
            },
        });
    },

    selectSporttype: function () {
        $("#InformationMssg").empty();
        $.ajax({
            type: "POST",
            url: window.rootUrl + "Licensee/VirtualSportWinLoss/SelSporttype",
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            data: null,
            beforeSend: function () {
                AccReport.disableInput();
            },
            success: function (rs) {
                if (rs && rs.length > 0) {
                    VirtualSportWinLoss.renderSporttype(rs);
                } else {
                    VirtualSportWinLoss.clearAllDataWhenSporttypeEmpty();
                }
            },
            error: function () {
                VirtualSportWinLoss.clearAllDataWhenSporttypeEmpty();
            },
            complete: function () {
                AccReport.enableInput();
            },
        });
    },
    selVirtualSportReportCriteria: '',
    clearAllDataWhenLincenseeEmpty: function () {
        $("#ddlLicenseese").multiSelect({
            minWidth: VirtualSportWinLoss.minWidthOfLicenseesDropdown,
            maxHeight: VirtualSportWinLoss.maxHeightOfLicenseesDropdown,
            shadow: true,
            showHeader: true,
            isCheckAll: true,
            showFooter: false,
            listData: [],
            noItemText: 'No licensee found.'
        });
        //$("#inforMessage").showErrorMessage("No Licensee found.");
    },
    clearAllDataWhenSporttypeEmpty: function () {
        $("#ddlSporttype").multiSelect({
            minWidth: 170,
            maxHeight: VirtualSportWinLoss.maxHeightOfLicenseesDropdown,
            shadow: true,
            showHeader: true,
            isCheckAll: true,
            showFooter: false,
            showTooltip: true,
            listData: [],
            noItemText: 'No sport type found.'
        });

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
            } else if (currentResellerType == 2) {
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
        $("#ddlLicenseese").multiSelect({
            minWidth: VirtualSportWinLoss.minWidthOfLicenseesDropdown,
            maxHeight: VirtualSportWinLoss.maxHeightOfLicenseesDropdown,
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
            } else {
                operators.push(listChecked[i].value);
            }
        }
        var result = {
            'normal': normalLicensees,
            'operators': operators
        }
        return result;
    },

    renderSporttype: function (sporttypes) {
        //append the distributors to main data
        var sporttypeTempData = [];

        for (var i = 0; i < sporttypes.length; i++) {
            var id = sporttypes[i].Id;
            var name = sporttypes[i].Name;
            sporttypeTempData.push({
                Id: "#ddlSporttype" + id,
                Value: id,
                Text: name,
                Checked: true,
                Enable: true,
                CssClass: "",
                Items: []
            });
        }

        $("#ddlSporttype").multiSelect({
            minWidth: 170,
            maxHeight: VirtualSportWinLoss.maxHeightOfLicenseesDropdown,
            shadow: true,
            showHeader: true,
            isCheckAll: true,
            showFooter: false,
            showTooltip: true,
            checkAllText: 'All Sports type',
            isUsedCustomCheckAllText: true,
            customCheckAllText: 'All Sports type',
            listData: sporttypeTempData
        });
    },

    getSelectedSport: function () {
        var id = 'ddlSporttype';

        var selectedList = $('#' + id).multiSelect('getCheckedItems');
        var result = "";

        for (var i = 0; i < selectedList.length; i++) {
            result = result + selectedList[i] + ",";
        }

        result = result.replace(/,*$/, "");
        return result;
    },

    selWinLossReport: function () {

        AccReport.popUp.removeHighLight();

        var sportIds = VirtualSportWinLoss.getSelectedSport();
        var licenseesSeleted = VirtualSportWinLoss.getLicenseesSelected();
        var normal = licenseesSeleted.normal.join(',');
        var operators = licenseesSeleted.operators.join(',');
        var fromDate = $('#ddlMonth').monPickr("getFromDate", "MMM dd, yyyy");
        var toDate = $('#ddlMonth').monPickr("getToDate", "MMM dd, yyyy");

        var data = {
            NormalLicensee: normal,
            Operator: operators,
            FromDate: fromDate,
            ToDate: toDate,
            SportList: sportIds
        };
        VirtualSportWinLoss.selVirtualSportReportCriteria = data;
        //        var param = {
        //            param: data
        //        };

        var notifyIds = [];
        var msg = '';
        if (normal.length == 0 && operators.length == 0) {
            msg = 'Please choose Licensee!';
            notifyIds.push("ddlLicenseese");
        }
        if (sportIds.length == 0) {
            msg = 'Please choose Sport!';
            notifyIds.push("ddlSporttype");
        }

        if (msg.length > 0) {
            AccReport.popUp.show(msg, notifyIds.join(','));
            return;
        }


        $("#InformationMssg").empty();
        $('#VirtualSportwinloss').addClass('display-none');

        ///AccReport.PopUp.Show
        if ($('#ddlLicenseese').multiSelect('getCheckedItems').length > 0) {
            AccReport.popUp.removeHighLight();
            $.ajax({
                type: "POST",
                url: window.rootUrl + "Licensee/VirtualSportWinLoss/SelVirtualSportWinLoss",
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                data: JSON.stringify(VirtualSportWinLoss.selVirtualSportReportCriteria),
                beforeSend: function () {
                    AccReport.disableInput();
                },
                success: function (rs) {
                    $('#reporttile').html('Virtual Sports Win Loss Report from ' + fromDate + ' to ' + toDate);
                    var arr = [];
                    var arr_Left = [];
                    var length = rs.Rows.length;                   
                    $(".grid-title").removeClass("display-none");
                     if (rs != null && length > 0) {
                        for (var i = 0; i < length; i++) {
                            if (rs.Rows[i].Type == 'Revenue') {
                                if(i > 0){
                                    arr.push(VirtualSportWinLoss.renderEmptyRow(rs.Rows[i]));
                                    arr_Left.push(VirtualSportWinLoss.renderEmptyRow_Left(rs.Rows[i]));
                                }
                                arr.push(VirtualSportWinLoss.renderRevenueRow(rs.Rows[i]));
                                arr_Left.push(VirtualSportWinLoss.renderRevenueRow_Left(rs.Rows[i]));
                            } else {
                                arr.push(VirtualSportWinLoss.renderResellerRow(rs.Rows[i], false));
                                arr_Left.push(VirtualSportWinLoss.renderResellerRow_Left(rs.Rows[i], false));

                                if (rs.Rows[i].Rows != null) {
                                    var length1 = rs.Rows[i].Rows.length;
                                    for (var j = 0; j < length1; j++) {
                                        arr.push(VirtualSportWinLoss.renderResellerRow(rs.Rows[i].Rows[j], true));
                                        arr_Left.push(VirtualSportWinLoss.renderResellerRow_Left(rs.Rows[i].Rows[j], true));
                                    }
                                }

                            }
                        }
                        $('#tblContent tbody').empty().append(arr.join(''));

                        $('#tblContentHeader tbody').empty().append(arr_Left.join(''));

                        $('#VirtualSportwinloss').find(".col_footer_content thead").empty().append(VirtualSportWinLoss.renderTotalRow(rs));
                        $('#VirtualSportwinloss').find(".row_footer_content thead").empty().append(VirtualSportWinLoss.renderTotalRow_Left(rs));

                        $('#VirtualSportwinloss').removeClass("display-none").gridScroller(VirtualSportWinLoss.gridOpts);
                         $(".grid-toolbox").removeClass("display-none");
                    } else {
                        arr.push(VirtualSportWinLoss.renderNoData());
                        arr_Left.push(VirtualSportWinLoss.renderNoData_Left());
                        $('#tblContent tbody').empty().append(arr.join(''));
                        $('#tblContentHeader tbody').empty().append(arr_Left.join(''));
                        $('#VirtualSportwinloss').find(".col_footer_content thead").empty();
                        $('#VirtualSportwinloss').find(".row_footer_content thead").empty();
                        $(".grid-toolbox").addClass("display-none");
                    }
                    $('#VirtualSportwinloss').removeClass("display-none").gridScroller(VirtualSportWinLoss.gridOpts);
                },
                error: function () {
                    AccReport.popUp.show('There is no information available.');
                },
                complete: function () {
                    AccReport.enableInput();
                }
            });
        } else {
            AccReport.popUp.show(selectLicenseeMessage, "ddlLicenseese");
        }
    },
    renderResellerRow: function (row, isChild) {
        var cssLicenseeName = (isChild == true ? "align-right" : "align-left");
        var cssWinlossDetail = (isChild == true ? "winlossdetail totalRow" : "");
        var cssRevenue = (isChild == true ? "" : "b");

        var html = '<tr class="' + cssWinlossDetail + '">\
            <td>\
                <div class="' + cssLicenseeName + ' width100">' + row.Name + '</div>\
            </td>\
            <td>\
                <div class="align-right width100">' + row.BetCount + '</div>\
            </td>\
            <td>\
                <div class="align-right width100">' + row.Turnover + '</div>\
            </td>\
            <td >\
                <div class="align-right width120">' + row.GrossWinloss + '</div>\
            </td>\
             <td >\
                <div class="'+ (row.DistributorRevenue == '' ? 'align-center' : 'align-right')+' width120 ' + cssRevenue + '">' + (row.DistributorRevenue == '' ? '-' : row.DistributorRevenue) + '</div>\
            </td>\
            <td>\
                <div class="align-right width120  ' + cssRevenue + '"">' + row.OneworksIncome + '</div>\
            </td>\
            <td>\
                <div class="align-right width200  ' + cssRevenue + '"">' + row.AmountInspired + '</div>\
            </td>\
            <td>\
                <div class="align-right width200  ' + cssRevenue + '"">' + row.AmountOneWorks + '</div>\
            </td>\
        </tr>';
        return html;
    },
    renderResellerRow_Left: function (row, isChild) {
        var cssLicenseeName = (isChild == true ? "align-right" : "align-left b");
        var cssWinlossDetail = (isChild == true ? "winlossdetail totalRow" : "");
        var html = '<tr class="' + cssWinlossDetail + '">\
          <td>\
                <div class="' + cssLicenseeName + ' width100 b">' + row.Name + '</div>\
            </td>\
            <td>\
                <div class="align-right width100">' + row.BetCount + '</div>\
            </td>\
            <td>\
                <div class="align-right width100">' + row.Turnover + '</div>\
            </td>\
            <td >\
                <div class="align-right width120">' + row.GrossWinloss + '</div>\
            </td>\
        </tr>';
        return html;
    },
      renderEmptyRow: function (row) {
        var html = '<tr class="hideborderright">\
            <td>\
                <div class="width100">&nbsp;</div>\
            </td>\
            <td>\
                <div class="width100">&nbsp;</div>\
            </td>\
            <td>\
                <div class="width100">&nbsp;</div>\
            </td>\
            <td >\
                <div class="width120">&nbsp;</div>\
            </td>\
             <td >\
                <div class="width120">&nbsp;</div>\
            </td>\
            <td>\
                <div class="align-center b i width120">&nbsp;</div>\
            </td>\
            <td>\
                <div class="align-center b i width200">&nbsp;</div>\
            </td>\
            <td>\
                <div class="width200">&nbsp;</div>\
            </td>\
        </tr>';
        return html;
    },
    renderEmptyRow_Left: function (row) {
        var html = '<tr class="hideborderright ">\
            <td>\
                <div class="width100">&nbsp;</div>\
            </td>\
            <td>\
                <div class="width100">&nbsp;</div>\
            </td>\
            <td>\
                <div class="width100">&nbsp;</div>\
            </td>\
            <td>\
                <div class="width120">&nbsp;</div>\
            </td>\
        </tr>';
        return html;
    },
    renderRevenueRow: function (row) {
        var html = '<tr class="revenueInfo">\
            <td>\
                <div class="width100">&nbsp;</div>\
            </td>\
            <td>\
                <div class="width100">&nbsp;</div>\
            </td>\
            <td>\
                <div class="width100">&nbsp;</div>\
            </td>\
            <td>\
                <div class="width120">&nbsp;</div>\
            </td>\
            <td>\
                <div class="align-center b i width120">' + row.DistributorRevenue + '</div>\
            </td>\
            <td>\
                <div class="align-center b i width120">' + row.OneworksIncome + '</div>\
            </td>\
            <td>\
                <div class="align-center b i width200">' + row.AmountInspired + '</div>\
            </td>\
            <td>\
                <div class="width200">&nbsp;</div>\
            </td>\
        </tr>';
        return html;
    },
    renderRevenueRow_Left: function (row) {
        var html = '<tr class="revenueInfo ">\
             <td style="border-right:none 0px;">\
                <div class="width100 " >&nbsp;</div>\
            </td>\
            <td style="border-right:none 0px;">\
                <div class="width100">&nbsp;</div>\
            </td>\
            <td style="border-right:none 0px;">\
                <div class="width100">&nbsp;</div>\
            </td>\
            <td>\
                <div class="width120">&nbsp;</div>\
            </td>\
        </tr>';
        return html;
    },
    renderTotalRow: function (row) {
        var html = '<tr class="totalRow">\
            <th>\
                <div class="align-right b width100">Total</div>\
            </th>\
            <th>\
                <div class="align-right b width100">' + row.BetCount + '</div>\
            </th>\
            <th>\
                <div class="align-right b width100">' + row.Turnover + '</div>\
            </th>\
            <th >\
                <div class="align-right b width120">' + row.GrossWinloss + '</div>\
            </th>\
             <th >\
                <div class="align-right b width120">' + (row.DistributorRevenue == '' ? '0.00' : row.DistributorRevenue) + '</div>\
            </th>\
            <th>\
                <div class="align-right b width120">' + row.OneworksIncome + '</div>\
            </th>\
            <th>\
                <div class="align-right b width200">' + row.AmountInspired + '</div>\
            </th>\
            <th>\
                <div class="align-right b width200">' + row.AmountOneWorks + '</div>\
            </th>\
        </tr>';
        return html;
    },
    renderTotalRow_Left: function (row) {
        var html = '<tr class="totalRow">\
             <th>\
                <div class="align-right b width100">Total</div>\
            </th>\
            <th>\
                <div class="align-right b width100">' + row.BetCount + '</div>\
            </th>\
            <th>\
                <div class="align-right b width100">' + row.Turnover + '</div>\
            </th>\
            <th>\
                <div class="align-right b width120">' + row.GrossWinloss + '</div>\
            </th>\
        </tr>';
        return html;
    },
    renderNoData: function (row) {
        var html = '<tr class="hideborderright">\
            <td>\
                <div class="align-right b width100">&nbsp;</div>\
            </td>\
            <td>\
                <div class="align-right b width100">&nbsp;</div>\
            </td>\
            <td>\
                <div class="align-right b width100">&nbsp;</div>\
            </td>\
            <td >\
                <div class="align-right b width120">&nbsp;</div>\
            </td>\
            <td>\
                <div class="align-right b width120 no-data-not-frozen">There is no data</div>\
            </td>\
             <td >\
                <div class="align-right b width120">&nbsp;</div>\
            </td>\
            <td>\
                <div class="align-right b width200">&nbsp;</div>\
            </td>\
            <td>\
                <div class="align-right b width200">&nbsp;</div>\
            </td>\
        </tr>';
        return html;
    },
    renderNoData_Left: function (row) {
        var html = '<tr class="hideborderright">\
              <td>\
                <div class="align-right b width100">&nbsp;</div>\
            </td>\
            <td>\
                <div class="align-right b width100">&nbsp;</div>\
            </td>\
            <td>\
                <div class="align-right b width100">&nbsp;</div>\
            </td>\
            <td >\
                <div class="align-right b width120">&nbsp;</div>\
            </td>\
        </tr>';
        return html;
    },
    exportExcel: function () {
        var parameters = $.param(VirtualSportWinLoss.selVirtualSportReportCriteria);
        AccReport.downloadFile(window.rootUrl + "Licensee/VirtualSportWinLoss/ExportGameWinLossReport", parameters);
    }
};



$(document).ready(function () {

    VirtualSportWinLoss.selectLicensees();
    VirtualSportWinLoss.selectSporttype();
    var gridOpts = {
        tblContentId: 'tblContent',
        isShowPager: false
    };
    $("#VirtualSportwinloss").initGrid(gridOpts);

    $("#notemessage").tipsy({
        html: true,
        opacity: 0.9,
        gravity: 'ne'
    });

    $('#ddlMonth').monPickr({
        plugInId: '#ddlMonth',
        today: _today,
        maxDate: _today,
        minDate: _minDate ,
        showDateRange: true,
        fromDayId: 'ddlFrom',
        month: _previousMonth,
        sortDesc: true,
        log: function (msg) {
            console.warn(msg);
        }
    });

    $('#btnGenerate').click(function () {
        VirtualSportWinLoss.selWinLossReport();
    });

      $('#btnExport').click(function () {
         VirtualSportWinLoss.exportExcel();
    });
   
});