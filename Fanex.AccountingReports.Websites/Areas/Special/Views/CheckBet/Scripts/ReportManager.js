$(document).ready(function () {
    var checkBetReportManager = new Fanex.AccountingReports.Special.CheckBet.ReportManager();
    checkBetReportManager.initPage();
});

Namespace.Register("Fanex.AccountingReports.Special.CheckBet.ReportManager");
Fanex.AccountingReports.Special.CheckBet.ReportManager = function () {
    var self = this;

    var _normalTicket = new Fanex.AccountingReports.Special.CheckBet.NormalChangedTickets();
    var _rejectTicket = new Fanex.AccountingReports.Special.CheckBet.RejectChangedTickets();
    var _checkBet = new Fanex.AccountingReports.Special.CheckBet.CheckBet();
    getNormalTickets = function (checkDate) {
        _rejectTicket.hideReport();
        _rejectTicket.resetTempHtml();
        _normalTicket.getNormalTickets(checkDate);
    };

    getRejectTickets = function (checkDate) {
        _normalTicket.hideReport();
        _rejectTicket.getRejectTickets(checkDate);
    };

    self.initPage = function () {
        _checkBet.initPage();
        _normalTicket.initPage();
        _rejectTicket.initPage();
    }
    self.betlistgridOpts = {
        tblContentId: 'tblContent',
        manualCopyHeader: true,
        isShowPager: false,
        fullWidth: true,
        fullHeight: false,
        gridWidth: 1010, //=totalColumnWidth + totalColumn*4 +totalColumn
        gridHeight: 140,
        freezeWidth: 0, //=totalColumnWidth + (totalColumn-1)*4 + (totalColumn-1)
        numberFreezeCol: 0
    };
    var betlistContent = $('#betListContent');
    getBetList = function (refno) {

        $.ajax({
            type: "POST",
            url: window.rootUrl + "BetList/BetList/GetBetListReportByRefNo",
            data: 'refno=' + refno,
            success: function (result) {
                betlistContent.html(result);
                var t = setTimeout(function () {

                    $('#betlistContainer').dialog({
                        title: "Bet list of " + refno,
                        autoOpen: false,
                        height: 200,
                        width: 1000,
                        modal: false,
                        resizable: false,
                        close: function (event, ui) {
                        }
                    }).dialog("open");

                    var grid = betlistContent.find("#BetlistByRefNo");
                    grid.initGrid(self.betlistgridOpts);
                    grid.removeClass("display-none").gridScroller(self.betlistgridOpts);

                    var resultLink = betlistContent.find("a[id^=result_]");

                    var beforeResultLink = $(resultLink[0]).prev();
                    var afterResultLink = $(resultLink[0]).next();
                    if (beforeResultLink &&
                    beforeResultLink.length &&
                    beforeResultLink[0].tagName.toLowerCase() == 'br') {
                        beforeResultLink.remove();
                    }

                    if (afterResultLink &&
                    afterResultLink.length &&
                    afterResultLink[0].tagName.toLowerCase() == 'br') {
                        afterResultLink.remove();
                    }

                    if (resultLink) {
                        resultLink.remove();
                    }

                }, 500);

            },
            error: function () {
                try { parent.jQuery.unblockUI(); } catch (e) { }
            },
            complete: function () {
                try { parent.jQuery.unblockUI(); } catch (e) { }
            }
        });
    };
    hideReport = function () {
        _normalTicket.hideReport();
        _rejectTicket.hideReport();
    };
}
