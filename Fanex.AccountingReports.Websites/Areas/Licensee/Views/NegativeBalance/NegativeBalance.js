/* File Created: November 20, 2012 */
var negativeBalance = {
    selNegativeBalanceCriteria:
    {
        Site: "",
        IsGetCache: true,
        PageSize: 50,
        PageIndex: 1
    },    
    gridOpts: {
        tblContentId: 'tblContent',
        manualCopyHeader: true,
        isShowPager: true,
        fullWidth: true,
        fullHeight: true,
        gridWidth: 660,
        gridHeight: 165,
        freezeWidth: 0,
        numberFreezeCol: 0
    },
    pagerOpts: {
        callback: null,
        startPerPage: 50,
        maxTtemsPerPage: 500,
        itemsPerPageArray: [50, 100, 200, 500],
        itemsPerPage: 50,
        currentPage: 1
    },
    cache: '',
    count: 0,
    selnegativeBalanceReport: function (isCache) {
        negativeBalance.selNegativeBalanceCriteria.IsGetCache = isCache;
        if (isCache == false) {
            negativeBalance.selNegativeBalanceCriteria.Site = $('#dllSite').val();
            negativeBalance.selNegativeBalanceCriteria.PageIndex = 1;

        }
        $("#tblContainer").find('div').removeClass('asc').removeClass('desc');
        $.ajax({
            type: "POST",
            url: window.rootUrl + "Licensee/NegativeBalance/GetNegativeBalance",
            contentType: "application/json; charset=utf-8",
            data: JSON.stringify(negativeBalance.selNegativeBalanceCriteria),
            beforeSend: function () {
                AccReport.disableInput();
            },
            success: function (result) {
                if (result.NegativeBalanceList.length == 0) {
                    negativeBalance.gridOpts.isShowPager = false;
                }
                negativeBalance.cache = result;
                var tilte = negativeBalance.selNegativeBalanceCriteria.Site == "All" ? "All Licensees" : negativeBalance.selNegativeBalanceCriteria.Site;
                $('#reporttile').html('Negative Balance Report for ' + tilte);
                negativeBalance.renderGrid(result);
                negativeBalance.selNegativeBalanceCriteria.IsGetCache = true;
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
        var rowNo = negativeBalance.selNegativeBalanceCriteria.PageSize * (negativeBalance.selNegativeBalanceCriteria.PageIndex - 1) + 1;
        var rs = result.NegativeBalanceList;
        negativeBalance.count = result.TotalNegativeBalanceUser;
        $(".grid-title").removeClass("display-none");
        if (rs && rs.length > 0) {
            var parentLength = rs.length;
            for (var i = 0; i < parentLength; i++) {
                var cssRow = "";
                if (negativeBalance.selNegativeBalanceCriteria.Site == "All") {
                    arr.push(negativeBalance.renderSiteRow(rs[i].LicenseeSite));
                }
                var childLength = rs[i].NegativeBalanceUserList.length;
                for (var j = 0; j < childLength; j++) {
                    cssRow = "negative_" + rs[i].LicenseeSite;
                    if (j % 2 == 0) {
                        cssRow += " even";
                    }
                    arr.push(negativeBalance.renderNegativeBalanceRow(rs[i].NegativeBalanceUserList[j], cssRow, rowNo));
                    rowNo++;
                }
            }
            $('#tblContent tbody').empty().append(arr.join(''));
            $("#pagination").removeClass("display-none");
            $(".grid-toolbox").removeClass("display-none");
        }
        else {
            arr.push(negativeBalance.renderNoData());
            $('#tblContent tbody').empty().append(arr.join(''));
            $("#pagination").addClass("display-none");
            $(".grid-toolbox").addClass("display-none");
        }
        negativeBalance.initPager(negativeBalance.count, negativeBalance.selNegativeBalanceCriteria.PageSize, negativeBalance.selNegativeBalanceCriteria.PageIndex - 1);
        $('#negativeBalance').removeClass("display-none").gridScroller(negativeBalance.gridOpts);

    },
    sort: function (sortBy, sortDirection) {
        negativeBalance.selNegativeBalanceCriteria.SortBy = sortBy;
        var objSort = $('#' + sortDirection);
        if (objSort.hasClass('desc')) {
            $("#tblContainer").find('div').removeClass('asc').removeClass('desc');
            negativeBalance.selNegativeBalanceCriteria.SortDirection = "ASC";
            objSort.addClass('asc')
            objSort.removeClass('desc');
            for (var i = 0; i < negativeBalance.cache.NegativeBalanceList.length; i++) {
                negativeBalance.sortByField(negativeBalance.cache.NegativeBalanceList[i].NegativeBalanceUserList, sortBy, 'ASC');
            }
        }
        else {
            $("#tblContainer").find('div').removeClass('asc').removeClass('desc');
            negativeBalance.selNegativeBalanceCriteria.SortDirection = "DESC";
            objSort.addClass('desc')
            objSort.removeClass('DESC');
            for (var i = 0; i < negativeBalance.cache.NegativeBalanceList.length; i++) {
                negativeBalance.sortByField(negativeBalance.cache.NegativeBalanceList[i].NegativeBalanceUserList, sortBy, 'DESC');
            }
        }
        negativeBalance.renderGrid(negativeBalance.cache);
    },
    collapseExpandSite: function (colCollapse, site) {
        var btnCollapse = colCollapse.find(".btnCollapse");

        if (btnCollapse.hasClass('expand')) {
            btnCollapse.removeClass("expand");
            $("#tblContent .negative_" + site).addClass("display-none");
            btnCollapse.addClass("collapse");
            btnCollapse.attr('onclick', '').unbind('click');
            btnCollapse.bind('click', function (site) {
                negativeBalance.collapseExpandSite($(this), $(this).attr("name"));
            });
        }
        else {
            btnCollapse.removeClass("collapse");
            $("#tblContent .negative_" + site).removeClass("display-none");
            btnCollapse.addClass("expand");
            btnCollapse.attr('onclick', '').unbind('click');
            btnCollapse.bind('click', function (site) {
                negativeBalance.collapseExpandSite($(this), $(this).attr("name"));
            });
        }
        $('#negativeBalance').gridScroller(negativeBalance.gridOpts);
    },
    renderSiteRow: function (siteName) {
        var onclickCollapse = "negativeBalance.collapseExpandSite($(this),'" + siteName + "');";
        var html = '<tr class="site">\
        <td>\
                <div class="withrowNo">&nbsp</div>\
            </td>\
            <td class="cursor-pointer" name="' + siteName + '"  onclick="' + onclickCollapse + '">\
                <div class="align-left bold width200"><span class="expand btnCollapse"></span>' + siteName + '</div>\
            </td>\
              <td>\
                <div class="width200">&nbsp;</div>\
            </td>\
            <td>\
                <div class="width200">&nbsp;</div>\
            </td>\
        </tr>';
        return html;
    },
    renderNegativeBalanceRow: function (row, css, rowNo) {
        var html = '<tr class="' + css + '">\
         <td>\
                <div class="withrowNo">' + rowNo + '</div>\
            </td>\
            <td>\
                <div class="align-left width200"><span class="margin-left">' + row.UserName + '</span></div>\
            </td>\
            <td>\
                <div class="align-right width200">' + numeral(row.Balance).format(FormatConstant.TWO_DECIMAL_NUMBER) + '</div>\
            </td>\
            <td>\
                <div class="align-center width200">' + row.Currency + '</div>\
            </td>\
        </tr>';
        return html;
    },
    renderNoData: function () {
        var html = '<tr class="hideborderright"><td colspan="4"><div class="no-data-message">There is no data.</div></tr>';
        return html;
    },
    initPager: function (total_items, itemsPerPage, currentPage) {
        negativeBalance.pagerOpts.callback = negativeBalance.pageSelectCallback;
        negativeBalance.pagerOpts.itemsPerPage = itemsPerPage;
        negativeBalance.pagerOpts.currentPage = currentPage;

        $("#pagination").fanexPagination(total_items, negativeBalance.pagerOpts);
    },
    pageSelectCallback: function (page_index, _itemsPerPage, jq) {
        var max_page = negativeBalance.count;
        page_index = page_index < max_page ? page_index : max_page - 1;
        negativeBalance.selNegativeBalanceCriteria.PageIndex = page_index + 1;
        if (_itemsPerPage != negativeBalance.selNegativeBalanceCriteria.PageSize) {
            negativeBalance.selNegativeBalanceCriteria.PageIndex = 1;
        }
        negativeBalance.selNegativeBalanceCriteria.PageSize = _itemsPerPage;
        negativeBalance.selnegativeBalanceReport(true);
        return false;
    },
    sortByField: function (array, sortBy, sortDirection) {
        if (sortDirection == 'ASC') {
            return array.sort(function (a, b) {
                return a.Balance > b.Balance ? 1 : -1;
            });
        }
        else {
            return array.sort(function (a, b) {

                return a.Balance < b.Balance ? 1 : -1;
            });
        }
    },
    exportExcel: function () {
        var parameters = $.param({
            Site: negativeBalance.selNegativeBalanceCriteria.Site
        });
        var url = window.rootUrl + "Licensee/NegativeBalance/ExportNegativeBalanceReport?" + parameters;
        $('#frm').attr('src', url);
    }
};

$(document).ready(function () {
    $("#negativeBalance").initGrid(negativeBalance.gridOpts);

    $('#btnSubmit').click(function () {
        negativeBalance.selnegativeBalanceReport(false);
    });

    $('#btnExport').click(function () {
        negativeBalance.exportExcel();
    });    
});