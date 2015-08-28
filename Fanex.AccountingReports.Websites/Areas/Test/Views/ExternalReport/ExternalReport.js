/* File Created: December 13, 2012 */
$(document).ready(function () {
    var myGrid = new FreezeGrid();
    myGrid.initPage();
});
function FreezeGrid() {
    var self = this;
    self.initPage = function () {
        initGridScrollerTables();
        getData();
    };
    initGridScrollerTables = function () {
        $("#mygrid").initGrid({
            isShowPager: true
        });

    };
    var options = {
        isShowPager: true,
        tblContentId: 'grid_content',
        fullHeight: true,
        fullWidth: true,
        gridWidth: 1815,
        gridHeight: 150,
        freezeWidth: 290,
        numberFreezeCol: 3
    };
    getData = function () {
        $.ajax({
            type: "POST",
            url: window.rootUrl + "Test/ExternalReport/GetDailyWinlost",
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            data: null,
            beforeSend: function () {
                AccReport.disableInput();
            },
            success: function (rs) {
                renderGrid(rs);
            },
            error: function (event, jqXHR, ajaxSettings) {
                renderGrid(null);
            },
            complete: function () {
                AccReport.enableInput();
            }
        });
    };
    renderGrid = function (report) {
        if (report) {
            var rowHeader = '';
            var content = '';

            var rowFooter = '';
            var contentFooter = '';
            var reportLength = report.WinlossDetails.length;
            for (var i = 0; i < reportLength; i++) {
                var winLossData = report.WinlossDetails[i];
                rowHeader += renderRowHeader(winLossData, i+1);
                content += renderContent(winLossData, i);
            }

            rowFooter += renderRowFooter(report);
            contentFooter += renderContentFooter(report);

            $("#row_header tbody").html(rowHeader);
            $("#grid_content tbody").html(content);

            $("#mygrid .row_footer_content thead").html(rowFooter);
            $("#mygrid .col_footer_content thead").html(contentFooter);
        } else {
            $("#row_header tbody").empty();
            $("#grid_content tbody").empty();
            $("#mygrid .row_footer_content thead").empty();
            $("#mygrid .col_footer_content thead").empty();
        }

        $("#mygrid").gridScroller(options);
    };
    renderRowHeader = function (row, index) {
        var html;
        html = '<tr>';
        html += '<td class=""><div class="col50 align-right">' + index + '</div></td>';
        html += '<td><div class="col110 link">' + row.Username + '</div></td>';
        html += '<td><div class="col120 align-right">' + row.TotalTo + '</div></td>';
        html += '</tr>';
        return html;
    };
    renderContent = function (row, index) {
        var html;
        html = '<tr>';
        html += '<td class=""><div class="col50 align-right">&nbsp;</div></td>';
        html += '<td><div class="col110 link">&nbsp;</div></td>';
        html += '<td><div class="col120 align-right">&nbsp;</div></td>';
        html += '<td><div class="col120 align-right">' + row.CompanyTo + '</div></td>';
        html += '<td><div class="col60 align-right">' + row.CompanyPt + '</div></td>';
        html += '<td><div class="col120 align-right">' + row.SuperTo + '</div></td>';
        html += '<td><div class="col60 align-right">' + row.SuperPt + '</div></td>';
        html += '<td><div class="col120 align-right">' + row.MasterTo + '</div></td>';
        html += '<td><div class="col60 align-right">' + row.MasterPt + '</div></td>';
        html += '<td><div class="col120 align-right">' + row.AgentTo + '</div></td>';
        html += '<td><div class="col60 align-right">' + row.AgentPt + '</div></td>';
        html += '<td><div class="col120 align-right">' + row.CompanyWinlost + '</div></td>';
        html += '<td><div class="col60 align-right">' + row.CompanyMargin + '</div></td>';
        html += '<td><div class="col120 align-right">' + row.SuperWinlost + '</div></td>';
        html += '<td><div class="col60 align-right">' + row.SuperMargin + '</div></td>';
        html += '<td><div class="col120 align-right">' + row.MasterWinlost + '</div></td>';
        html += '<td><div class="col60 align-right">' + row.MasterMargin + '</div></td>';
        html += '<td><div class="col120 align-right">' + row.AgentWinlost + '</div></td>';
        html += '<td><div class="col60 align-right">' + row.AgentMargin + '</div></td>';
        html += '</tr>';
        return html;
    };
    renderRowFooter = function (row) {
        var html;
        html = '<tr>';
        html += '<th colspan="2"><div class="col165 link">Grand Total</div></th>';
        html += '<th><div class="col120 align-right">' + row.TotalTo + '</div></th>';
        html += '</tr>';
        return html;
    };
    renderContentFooter = function (row, index) {
        var html;
        html = '<tr>';
        html += '<th class=""><div class="col50 align-right"></div></th>';
        html += '<th><div class="col110 link">Grand Total</div></th>';
        html += '<th><div class="col120 align-right">' + row.TotalTo + '</div></th>';
        html += '<th><div class="col120 align-right">' + row.CompanyTo + '</div></th>';
        html += '<th><div class="col60 align-right">' + row.CompanyPt + '</div></th>';
        html += '<th><div class="col120 align-right">' + row.SuperTo + '</div></th>';
        html += '<th><div class="col60 align-right">' + row.SuperPt + '</div></th>';
        html += '<th><div class="col120 align-right">' + row.MasterTo + '</div></th>';
        html += '<th><div class="col60 align-right">' + row.MasterPt + '</div></th>';
        html += '<th><div class="col120 align-right">' + row.AgentTo + '</div></th>';
        html += '<th><div class="col60 align-right">' + row.AgentPt + '</div></th>';
        html += '<th><div class="col120 align-right">' + row.CompanyWinlost + '</div></th>';
        html += '<th><div class="col60 align-right">' + row.CompanyMargin + '</div></th>';
        html += '<th><div class="col120 align-right">' + row.SuperWinlost + '</div></th>';
        html += '<th><div class="col60 align-right">' + row.SuperMargin + '</div></th>';
        html += '<th><div class="col120 align-right">' + row.MasterWinlost + '</div></th>';
        html += '<th><div class="col60 align-right">' + row.MasterMargin + '</div></th>';
        html += '<th><div class="col120 align-right">' + row.AgentWinlost + '</div></th>';
        html += '<th><div class="col60 align-right">' + row.AgentMargin + '</div></th>';
        html += '</tr>';
        return html;
    };
}