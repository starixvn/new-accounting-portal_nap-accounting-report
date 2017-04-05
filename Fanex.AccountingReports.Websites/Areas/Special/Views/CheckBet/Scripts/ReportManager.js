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
        gridWidth: 1230, //=totalColumnWidth + totalColumn*4 +totalColumn
        gridHeight: 400,
        freezeWidth: 0, //=totalColumnWidth + (totalColumn-1)*4 + (totalColumn-1)
        numberFreezeCol: 0
    };
    self.combineParlayDetailGridOptions = {
        tblContentId: 'tblContent',
        manualCopyHeader: true,
        isShowPager: false,
        fullWidth: true,
        fullHeight: false,
        gridWidth: 850, //=totalColumnWidth + totalColumn*4 +totalColumn
        gridHeight: 400,
        freezeWidth: 0, //=totalColumnWidth + (totalColumn-1)*4 + (totalColumn-1)
        numberFreezeCol: 0
    };

    var betlistContent = $('#betListContent');
    getBetList = function (refno) {
        if ($('#betlistContainer').length > 0) {
            $('.ui-dialog-content').dialog('close');
        }

        if ($('#combineParlayContainer').length > 0) {
            $('.ui-dialog-content').dialog('close');
        }
        $.blockUI();
        $.ajax({
            type: "GET",
            url: window.rootUrl + "BetList/BetList/GetBetListReportByRefNo",
            data: 'refno=' + refno,
            success: function (result) {
                betlistContent.html(result);
                betlistContent.find(".bl_status .ip").remove();
                betlistContent.find(".bl_status .result").not(".cashout-history").remove();

                var t = setTimeout(function () {

                    $('#betlistContainer').dialog({
                        title: "Bet list of " + refno,
                        autoOpen: false,
                        height: 'auto',
                        width: 1200,
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
            },
            complete: function () {
                $.unblockUI();
            }
        });
    };
    hideReport = function () {
        _normalTicket.hideReport();
        _rejectTicket.hideReport();
    };

    var combineParlayContent = $("#combineParlayContent");
    showCombMPDetail = function (refno, date, tranid, betid, custId) {
        $.ajax({
            type: 'GET',
            url: window.rootUrl + 'BetList/BetList/GetCombinationMP?refNo='
            + refno + '&transId=' + tranid + '&betId=' + betid + '&custId=' + custId,
            beforeSend: function () {
                AccReport.disableInput();
            },
            success: function (rs) {
                if (rs) {
                    combineParlayContent.empty().html(rs);
                    var dlgContainer = $('#combineParlayContainer');
                    dlgContainer.dialog({
                        title: "System Parlay",
                        autoOpen: false,
                        height: 'auto',
                        width: 850,
                        modal: false,
                        resizable: false,
                        close: function (event, ui) {

                        }
                    });
                    dlgContainer.dialog("open");

                    var grid = combineParlayContent.find("#combineMixParlayDetail");
                    grid.initGrid(self.combineParlayDetailGridOptions);
                    grid.removeClass("display-none").gridScroller(self.combineParlayDetailGridOptions);
                }
            },
            error: function (event, jqXHR, ajaxSettings) {
            },
            complete: function () {
                AccReport.enableInput();
            }
        });
    };

    showNewCombMP = function (transId, transIndex, refNo, handicap1, handicap2, betcheck, betteam) {
        showSystemParlay(transId, refNo, handicap1, handicap2, betcheck, betteam)
    };

    showMP = function (transid, refno) {
        var grid = $('#BetlistByRefNo');
        var parlayContent = $("div[id^='divEvent']")
        $.ajax({
            type: 'GET',
            url: window.rootUrl + 'BetList/BetList/GetMixParlayDetail?refno=' + refno + '&transId=' + transid,
            beforeSend: function () {
                AccReport.disableInput();
            },
            success: function (rs) {
                if (rs) {
                    parlayContent.css("display", "block").html(rs);
                    self.betlistgridOpts.isResetScroller = false;
                    grid.gridScroller(self.betlistgridOpts);
                }
                AccReport.enableInput();
            },
            error: function (event, jqXHR, ajaxSettings) {
                AccReport.enableInput();
            },
            complete: function () { }
        });
    };

    showCombMP = function(transId, transIndex, refNo) {
        showSystemParlay(transId, refNo, '', '', '', '');
    };

    showSystemParlay = function (transid, refno, handicap1, handicap2, betcheck, betteam) {
        var grid = $('#BetlistByRefNo');
        var parlayContent = $("div[id^='divEvent']")
        if (parlayContent.html() == '') {
            $.ajax({
                type: 'GET',
                url: window.rootUrl + 'BetList/BetList/GetMixParlayDetail?refno=' + refno + '&transId=' + transid + '&handicap1=' + handicap1 + '&handicap2=' + handicap2 + '&betcheck=' + betcheck + '&betteam=' + betteam,
                beforeSend: function () {
                    AccReport.disableInput();
                },
                success: function (rs) {
                    if (rs) {
                        if (handicap1 != '') {
                            $("div[id^='combinationOfferName']").addClass("display-none");
                        }

                        parlayContent.css("display", "block").html(rs);
                        self.betlistgridOpts.isResetScroller = false;
                        grid.gridScroller(self.betlistgridOpts);
                    }
                    AccReport.enableInput();
                },
                error: function (event, jqXHR, ajaxSettings) {
                    AccReport.enableInput();
                },
                complete: function () { }
            });
        }
        else {
            parlayContent.toggle();

            if (parlayContent.css("display") == "block") {
                $("div[id^='combinationOfferName']").addClass("display-none");
            } else {
                $("div[id^='combinationOfferName']").removeClass("display-none");
            }

            grid.gridScroller(self.betlistgridOpts);
        }
    };

}
