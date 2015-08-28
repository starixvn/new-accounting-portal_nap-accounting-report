/* File Created: November 16, 2012 */
var peakCocurrentUser = {
    selPeakCocurrentUserCriteria:
    {
        FromDate: "",
        ToDate: "",
        LicenseeName: ""
    },
    gridOpts: {
        tblContentId: 'tblContent',
        manualCopyHeader: true,
        isShowPager: false,
        fullWidth: true,
        fullHeight: true,
        gridWidth: 510,
        gridHeight: 167,
        freezeWidth: 0,
        numberFreezeCol: 0
    },
    getpeakCocurrentUserCriteria: function () {
        var dateRange = $('#datepickerplugin').nexDatePicker("getDateRange");
        peakCocurrentUser.selPeakCocurrentUserCriteria.LicenseeName = $('#dllSite').val();
        peakCocurrentUser.selPeakCocurrentUserCriteria.FromDate = dateRange.FromDate;
        peakCocurrentUser.selPeakCocurrentUserCriteria.ToDate = dateRange.ToDate;
    },
    selPeakCocurrentUserReport: function () {
        peakCocurrentUser.getpeakCocurrentUserCriteria();
        $.ajax({
            type: "POST",
            url: window.rootUrl + "Licensee/PeakConcurrentUsers/Partial",
            contentType: "application/json; charset=utf-8",
            data: JSON.stringify(peakCocurrentUser.selPeakCocurrentUserCriteria),
            beforeSend: function () {
                AccReport.disableInput();
            },
            success: function (rs) {
                peakCocurrentUser.renderGrid(rs);
            },
            error: function () {
            },
            complete: function () {
                AccReport.enableInput();
            }
        });
    },
    renderGrid: function (result) {
        var arr = [];
        var arrFooter = [];
        var rs = result.ListConcurrentUser;
        $(".grid-title").removeClass("display-none");
        if (rs && rs.length > 0) {
            $(".grid-toolbox").removeClass("display-none");
            var parentLength = rs.length;
            for (var i = 0; i < parentLength; i++) {
                var cssRow = "";
                if (i % 2 == 0) {
                    cssRow += " even";
                }
                arr.push(peakCocurrentUser.renderPeakRow(rs[i], cssRow));
            }
            $('#tblContent tbody').empty().append(arr.join(''));
            arrFooter.push(peakCocurrentUser.renderFooter(result));
            $('#tblContentFooter thead').empty().append(arrFooter.join(''));
            $(".grid-toolbox").removeClass("display-none");
        }
        else {
            arr.push(peakCocurrentUser.renderNoData());
            $('#tblContent tbody').empty().append(arr.join(''));
            $('#tblContentFooter thead').empty();
            $(".grid-toolbox").addClass("display-none");
        }
        $('#peakCocurrentUser').removeClass("display-none");
        $('#peakCocurrentUser').gridScroller(peakCocurrentUser.gridOpts);

        $('#reporttile').html('Peak Concurrent Users Report for ' + peakCocurrentUser.selPeakCocurrentUserCriteria.LicenseeName);
    },
    renderFooter: function (result) {
        var html = '<tr><th>\
                <div class="table-cell width250">' + _peakConcurrentUser_Total + '</div>\
            </th>\
              <th>\
                <div class="table-cell width250">' + result.Total + '</div>\
            </th>\
        </tr>\
        <tr><th>\
                <div class="table-cell width250">' + _peakConcurrentUser_Average + '</div>\
            </th>\
              <th>\
                <div class="table-cell width250">' + result.AverageUsers + '</div>\
            </th>\
        </tr>\
        <tr><th>\
                <div class="table-cell width250">' + _peakConcurrentUser_Max + '</div>\
            </th>\
              <th>\
                <div class="table-cell width250">' + result.MaxUsers + '</div>\
            </th>\
        </tr>\
        <tr><th>\
                <div class="table-cell width250">' + _peakConcurrentUser_Min + '</div>\
            </th>\
              <th>\
                <div class="table-cell width250">' + result.MinUsers + '</div>\
            </th>\
        </tr>';
        return html;
    },
    renderPeakRow: function (row, css) {
        var html = '<tr class="' + css + '">\
            <td>\
                <div class="table-cell width250">' + row.Date + '</div>\
            </td>\
            <td>\
                <div class="table-cell width250">' + row.TotalUser + '</div>\
            </td>\
        </tr>';
        return html;
    },
    renderNoData: function () {
        var html = '<tr><td colspan="2"><div class="no-data-message">There is no data.</div> </td></tr>';
        return html;
    },
    exportExcel: function () {
        var parameters = $.param({
            LicenseeName: peakCocurrentUser.selPeakCocurrentUserCriteria.LicenseeName,
            fromDate: peakCocurrentUser.selPeakCocurrentUserCriteria.FromDate,
            toDate: peakCocurrentUser.selPeakCocurrentUserCriteria.ToDate
        });

        var url = window.rootUrl + "Licensee/PeakConcurrentUsers/ExportGamePeakReport?" + parameters;
        $('#frm').attr('src', url);
    }
};


$(document).ready(function () {
    $('#datepickerplugin').nexDatePicker({
        nexDatePickerType: "daterange",
        datePickerFormat: {
            showOtherMonths: true,
            selectOtherMonths: true,
            changeMonth: true,
            changeYear: true,
            dateFormat: "M dd, yy",
            minDate: new Date(_minDate),
            maxDate: new Date(_today)
        },
        firstDatePicker: {
            Id: "firstDatePickerId1",
            Label: "From ",
            InitDate: _today,
            LabelCss: "lbl-field col_2",
            TextboxCss: "edit-field col_5"
        },
        secondDatePicker: {
            Id: "secondDatePickerId1",
            Label: "To ",
            InitDate: _today,
            LabelCss: "lbl-field col_2",
            TextboxCss: "edit-field col_5"
        },
        onSelectDatePicker: function () {
        }
    });
    $("#peakCocurrentUser").initGrid(peakCocurrentUser.gridOpts);

    $('#btnGenerate').click(function () {
        peakCocurrentUser.selPeakCocurrentUserReport();
    });

    $('#btnExport').click(function () {
        peakCocurrentUser.exportExcel();
    });
});