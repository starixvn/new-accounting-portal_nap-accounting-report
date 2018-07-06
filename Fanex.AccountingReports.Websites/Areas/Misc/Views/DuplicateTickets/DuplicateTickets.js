Namespace.Register("Fanex.AccountingReports.Misc.DuplicateTickets");

$(document).ready(function () {
    var duplicateTickets = new Fanex.AccountingReports.Misc.DuplicateTickets();
    duplicateTickets.initPage();
});

Fanex.AccountingReports.Misc.DuplicateTickets = function () {
    var self = this;

    var _table = {
        datePickerPlugin: null,
        btnGenerate: null,
        report: null,
        reportContent: null
    };

    var _params = {
        Date: null,
        CheckTime: 1
    };

    var _report = null;

    self.initPage = function () {
        catchedDOMElement();
        renderDatePicker();
        bindingEvent();
        self.initCountDown();
    };

    self.initFlashMedia = function () {
        var flashObjectUrl = window.rootUrl + "Areas/Misc/Views/DuplicateTickets/player.swf";
        AudioPlayer.setup(flashObjectUrl, {
            width: 290
        });
    };

    self.playSound = function () {
        var audio = new Audio(window.rootUrl + "Areas/Misc/Views/DuplicateTickets/Media/koekoeksklok.mp3");
        audio.play();
    };

    self.repeatPlaySoundEvent = function () {
        cachedData.countPlaySoundInAPatch++;
        if (cachedData.countPlaySoundInAPatch >= 4) {
            window.clearInterval(cachedData.countPlaySoundInAPatchEvent);
            cachedData.countPlaySoundInAPatch = 0;
        }

        self.playSound();
    };

    self.repeatPlaySound = function () {                
        cachedData.countPlaySoundInAPatchEvent = setInterval(self.repeatPlaySoundEvent, 3000);
        self.playSound();
    };

    var catchedDOMElement = function () {
        _table.datePickerPlugin = $('#datepickerplugin');
        _table.btnGenerate = $('#btnGenerate');
        _table.report = $('#DuplicateTicketsContainer');
        _table.reportContent = $('#DuplicateTicketsContent');
    };

    var cachedData = {
        countdownTime: 60,
        refreshIntervalCounter: (60 * 1000),
        refreshIntervalEvent: null,
        updateCountdownEvent: null,
        duplicateTicketData: [],
        showDuplicateTicketPopUpNotification: false,
        oneMinute: 60 * 1000,
        oneSecond: 1000,
        MD5Hash_Pre: null,
        MD5Hash_Current: null,
        CountAlertSound: 0,
        isGenerateReport: false,
        countPlaySoundInAPatch: 0,
        countPlaySoundInAPatchEvent: null
    };

    var renderDatePicker = function () {
        // set max date is today.
        var maxDate = new Date();
        // set min date is the moveBetsDate add 1 day. 
        var minDate = new Date(moveBetsDate);
        minDate.setDate(minDate.getDate() + 1);

        $('#datepickerplugin').nexDatePicker({
            nexDatePickerType: "single",
            datePickerFormat: {
                changeMonth: true,
                changeYear: true,
                maxDate: maxDate,
                minDate: minDate,
                dateFormat: "M dd, yy"
            },
            firstDatePicker: {
                Id: "firstDatePickerId1",
                Label: date,
                InitDate: today,
                LabelCss: "lbl-field col_2",
                TextboxCss: "edit-field col_6"
            },
            onSelectDatePicker: function () {
            }
        });
    };

    var bindingEvent = function () {
        _table.btnGenerate.click(function () {
            cachedData.CountAlertSound = 0;
            cachedData.MD5Hash_Pre = null;
            self.reloadReport();
            cachedData.isGenerateReport = true;
        });

        $("#showCountDownTime").click(function () {
            self.showCountDownTimeOptions();
        });

        $(".countDownOption").live("click", function () {
            self.changeCountDownOption(this);
        });

        $("#refreshIcon").click(function () {
            cachedData.CountAlertSound = 0;
            cachedData.MD5Hash_Pre = null;
            self.reloadReport();
            cachedData.isGenerateReport = true;
        });
    };

    self.reloadReport = function () {
        cachedData.showDuplicateTicketPopUpNotification = false;
        self.getDuplicateTicketReport();
        self.initCountDown();
    };

    self.initCountDown = function () {
        self.clearCountDownEvent();
        cachedData.countdownTime = cachedData.refreshIntervalCounter / cachedData.oneSecond;
        var time = self.getTime(cachedData.countdownTime);
        $("#refreshIcon").html(time);

        cachedData.updateCountdownEvent = setInterval(function () {
            self.timerCountDown();
        }, cachedData.oneSecond);

        cachedData.refreshIntervalEvent = setInterval(function () {
            cachedData.isGenerateReport = false;
            self.getDuplicateTicketReport();

            setTimeout(function () {
                AccReport.accReportPopUp.hide();
                cachedData.showDuplicateTicketPopUpNotification = false;
            }, 2 * cachedData.oneMinute);
        }, cachedData.refreshIntervalCounter);
    };

    self.showCountDownTimeOptions = function () {
        $("#countDownOptionContainer").slideToggle("slow");
    };

    self.changeCountDownOption = function (countDownOption) {
        countDownOption = $(countDownOption);
        $(".countDownOption.countDownOptionCheck").removeClass("countDownOptionCheck");
        countDownOption.addClass("countDownOptionCheck");
        var countDownTime = countDownOption.attr("val");
        var time = self.getTime(countDownTime);
        $("#refreshIcon").html(time);
        $("#countDownOptionContainer").slideToggle();

        self.clearCountDownEvent();

        cachedData.countdownTime = parseInt(countDownTime);
        cachedData.refreshIntervalCounter = parseInt(countDownTime * cachedData.oneSecond);

        cachedData.updateCountdownEvent = setInterval(function () {
            self.timerCountDown();
        }, cachedData.oneSecond);

        cachedData.refreshIntervalEvent = setInterval(function () {
            cachedData.isGenerateReport = false;
            self.getDuplicateTicketReport();

            setTimeout(function () {
                AccReport.accReportPopUp.hide();
                cachedData.showDuplicateTicketPopUpNotification = false;
            }, 2 * cachedData.oneMinute);
        }, cachedData.refreshIntervalCounter);
    };

    self.notifyDuplicateTickets = function () {
        if (!cachedData.isGenerateReport) {
            if (cachedData.MD5Hash_Pre != cachedData.MD5Hash_Current) {
                cachedData.MD5Hash_Pre = cachedData.MD5Hash_Current;
                cachedData.CountAlertSound = 1;
            }
            else {
                cachedData.CountAlertSound = cachedData.CountAlertSound + 1;
            }

            if (cachedData.CountAlertSound <= 2 && cachedData.duplicateTicketData.length > 0) {
                self.playNotificationAudio();
            }

            AccReport.popUp.show("There are duplicate tickets. Please check again.", '', 4);
            cachedData.showDuplicateTicketPopUpNotification = true;
        }

        if (cachedData.duplicateTicketData.length == 0) {
            AccReport.accReportPopUp.hide();
        }
    };

    self.timerCountDown = function () {
        if (cachedData.countdownTime > 1) {
            cachedData.countdownTime--;
        } else {
            cachedData.countdownTime = cachedData.refreshIntervalCounter / cachedData.oneSecond;
        }

        var time = self.getTime(cachedData.countdownTime);
        $("#refreshIcon").html(time);
    };

    self.getTime = function (totalSecond) {
        var minute = parseInt(totalSecond / 60);
        var second = parseInt(totalSecond % 60);
        var time = minute + ":" + (second == 0 ? '00' : second);

        return time;
    };

    self.clearCountDownEvent = function () {
        window.clearInterval(cachedData.updateCountdownEvent);
        window.clearInterval(cachedData.refreshIntervalEvent);
    };

    self.playNotificationAudio = function () {
        self.repeatPlaySound();
    };

    self.getDuplicateTicketReport = function () {
        AccReport.disableInput();
        getParams();
        if (validateParams() == true) {
            $.ajax({
                type: "POST",
                url: window.rootUrl + "Misc/DuplicateTickets/GetDuplicateTicketReport",
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                data: JSON.stringify(_params),
                beforeSend: function () {
                    AccReport.disableInput();
                },
                success: function (rs) {
                    if (!cachedData.showDuplicateTicketPopUpNotification) {
                        AccReport.popUp.hide();
                    }
                    cachedData.MD5Hash_Current = rs.MD5Hash;
                    cachedData.duplicateTicketData = rs.Rows;
                    self.notifyDuplicateTickets();
                    self.renderDuplicateTicketData(rs.Rows);
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
                    cachedData.isGenerateReport = false;
                }
            });
        }
        else {
            AccReport.enableInput();
        }
    };

    var getParams = function () {
        _params.Date = _table.datePickerPlugin.nexDatePicker("getSingleDate");
        _params.CheckTime = 1;
    };

    var validateParams = function () {
        return true;
    };

    self.renderDuplicateTicketData = function (report) {
        self.RenderReport(report, 0, 50);
    };

    self.renderDuplicateTicketGrid = function (bodyData, reportTitle, rowIndex, pageIndex, pageSize) {
        var columns = [
            { HeaderText: "#", Width: 60, Name: "ColNo", HeaderAlign: "center",
                CellAlign: "center", FormatNumber: FormatConstant.INT_NUMBER
            },
            { HeaderText: "Username", Width: 130, Name: "ColUserName", HeaderAlign: "center",
                CellAlign: "center"
            },
            { HeaderText: "CustId", Width: 80, Name: "ColCustId", HeaderAlign: "center",
                CellAlign: "right"
            },
            { HeaderText: "Bet Type", Width: 100, Name: "ColBetType", HeaderAlign: "center",
                CellAlign: "center"
            },
            { HeaderText: "Winlostdate", Width: 100, Name: "ColWinLostDate", HeaderAlign: "center",
                CellAlign: "center"
            },
            { HeaderText: "Balance Before Bet", Width: 100, Name: "ColBalanceBeforeBet", HeaderAlign: "center",
                CellAlign: "right", FormatNumber: FormatConstant.TWO_DECIMAL_NUMBER
            },
            { HeaderText: "Stake", Width: 80, Name: "ColStake", HeaderAlign: "center", CellAlign: "right", FormatNumber: FormatConstant.TWO_DECIMAL_NUMBER },
            { HeaderText: "Odds", Width: 50, Name: "ColOdds", HeaderAlign: "center", CellAlign: "right", FormatNumber: FormatConstant.TWO_DECIMAL_NUMBER },
            { HeaderText: "Status", Width: 60, Name: "ColStatus", HeaderAlign: "center", CellAlign: "center" },
            { HeaderText: "Refno", Width: 100, Name: "ColRefNo",
                HeaderAlign: "center", CellAlign: "right"
            },
            { HeaderText: "BetTime", Width: 90, Name: "ColBetTime", HeaderAlign: "center",
                CellAlign: "center"
            },
            { HeaderText: "Time Diff (milisecond)", Width: 70, Name: "ColTimeDiff", HeaderAlign: "center",
                CellAlign: "right", FormatNumber: FormatConstant.INT_NUMBER
            },
            {
                HeaderText: "TransDesc", Width: 700, Name: "ColTransDesc", HeaderAlign: "center",
                CellAlign: "left" 
            }
            ];


        var gridOptions = {
            columns: columns,
            showTitle: true,
            showDistinctRow: false,
            showHighlightRow: false,
            titleInfo: {
                text: reportTitle,
                showExcelButton: true,
                exportExcelFunction: function () {
                    self.exportDuplicateTicketReport();
                }
            },
            bodyRows: bodyData,
            gridExpandHeight: 170,
            showPager: true,

            customPager: true,
            pagerOption: {
                itemsPerPage: pageSize,
                currentPage: pageIndex,
                itemsPerPageArray: [50, 100, 200, 500],
                totalItem: rowIndex,
                pagerSelectCallBack: function (page_index, itemsPerPage, jq) {
                    self.RenderReport(_report, page_index, itemsPerPage);
                }
            }
        };

        _table.reportContent.fanexGrid(gridOptions);
    };

    self.RenderReport = function (report, pageIndex, itemsPerPage) {
        _report = report;

        var reportTitle = "Duplicate Tickets on " + _params.Date;
        var numberUsers = report.length;
        var bodyData = [];

        if (report != null && numberUsers > 0) {
            var rowIndex = 0;
            var startIndex = pageIndex * itemsPerPage;
            var endIndex = startIndex + itemsPerPage;

            for (var i = 0; i < numberUsers; i++) {
                var tdClass = i % 2 == 0 ? "evenRow" : '';
                var currentUser = report[i];
                var numberDuplicateTicketsByUser = currentUser.TicketInfo.length;
                for (var j = 0; j < numberDuplicateTicketsByUser; j++) {
                    if (rowIndex < startIndex || endIndex <= rowIndex) {
                        rowIndex++;
                        continue;
                    }

                    var currentTicketByUser = [];
                    var currentTicketInfo = currentUser.TicketInfo[j];
                    currentTicketByUser.push(
                        { Name: "ColNo", Value: rowIndex + 1, TdClass: tdClass }
                    );

                    if (j == 0 || bodyData.length === 0) {
                        var mergeRowNumber = numberDuplicateTicketsByUser - j;
                        currentTicketByUser.push(
                            { Name: "ColUserName", Value: currentUser.UserName, MergeRowNumber: mergeRowNumber, TdClass: tdClass },
                            { Name: "ColCustId", Value: currentUser.CustId, MergeRowNumber: mergeRowNumber, TdClass: tdClass },
                            { Name: "ColBetType", Value: currentUser.BetType, MergeRowNumber: mergeRowNumber, TdClass: tdClass },
                            { Name: "ColWinLostDate", Value: currentUser.WinLostDate, MergeRowNumber: mergeRowNumber, TdClass: tdClass },
                            { Name: "ColBalanceBeforeBet", Value: currentUser.BalanceBeforeBet, MergeRowNumber: mergeRowNumber, TdClass: tdClass },
                            { Name: "ColStake", Value: currentUser.Stake, MergeRowNumber: mergeRowNumber, TdClass: tdClass },
                            { Name: "ColOdds", Value: currentUser.Odds, MergeRowNumber: mergeRowNumber, TdClass: tdClass }
                        );
                    }
                    var timeDiff = (currentTicketInfo.TimeDifference == 0 && j == 0) ? "-" : currentTicketInfo.TimeDifference
                    currentTicketByUser.push(
                        { Name: "ColStatus", Value: currentTicketInfo.Status, TdClass: tdClass },
                        { Name: "ColRefNo", Value: currentTicketInfo.ReferenceNumber, TdClass: tdClass },
                        { Name: "ColBetTime", Value: currentTicketInfo.BetTime, TdClass: tdClass },
                        { Name: "ColTimeDiff", Value: timeDiff, TdClass: tdClass },
                        { Name: "ColTransDesc", Value: currentTicketInfo.TransDesc, TdClass: tdClass }
                    );

                    rowIndex++;
                    bodyData.push(
                        { Columns: currentTicketByUser }
                    );
                }
            }

            self.renderDuplicateTicketGrid(bodyData, reportTitle, rowIndex, pageIndex, itemsPerPage);
            _table.report.removeClass("display-none");
        }
        else {
            self.renderDuplicateTicketGrid([], reportTitle, 0, pageIndex, itemsPerPage);
        }
    };

    self.exportDuplicateTicketReport = function () {
        AccReport.downloadFile(window.rootUrl + "Misc/DuplicateTickets/ExportDuplicateTicketReport", _params);
    };
}



