Namespace.Register("Fanex.AccountingReports.Misc.RNGCasinoReport");
Fanex.AccountingReports.Misc.RNGCasinoReport = function () {
    var self = this;

    self.enums = {
        Mode: {
            Recent: "Recent",
            Historycal: "Historical"
        },

        RecentReportTimeSpan: 1,
        numberHoursInDay: 24,
        isMuteVolume: false
    };

    self.Tables = {
        txtAmount: null,
        timeRangeContainer: null,
        ddlMode: null,
        btnGenerate: null,
        txtFromDate: null,
        txtToDate: null,
        recentReportContainer: null,
        recentReportContent: null,
        historyReportContainer: null,
        historyReportContent: null,
        showHideFilter: null,
        iconVolumeControll: null,
        note: null,
        intervalId: null
    };

    self.params = {
        Amount: null,
        FromDate: null,
        ToDate: null,
        IsAutoRefresh: null
    };

    self.cacheDomElements = function () {
        self.Tables.txtAmount = $("#txtWinLoss");
        self.Tables.timeRangeContainer = $("#timeRangeContainer");
        self.Tables.ddlMode = $("#ddlMode");
        self.Tables.btnGenerate = $("#btnGenerate");
        self.Tables.txtFromDate = $("#fromTransDate");
        self.Tables.txtToDate = $("#toTransDate");
        self.Tables.recentReportContainer = $("#recentReportContainer");
        self.Tables.recentReportContent = $("#recentReportContent");
        self.Tables.historyReportContainer = $("#historyReportContainer");
        self.Tables.historyReportContent = $("#historyReportContent");
        self.Tables.showHideFilter = $("#show-hide-filter");
        self.Tables.iconVolumeControll = $(".volume-controll-icon");
        self.Tables.note = $('#spNote');
    };

    self.InitPage = function () {

        var to = moment().format("YYYY/MM/DD HH:mm:ss");
        var from = moment(to).subtract('hours', 1).format("YYYY/MM/DD HH:mm:ss");

        self.cacheDomElements();
        self.setupCalendar(fromTime, 'fromTransDate');
        self.setupCalendar(toTime, 'toTransDate');
        self.bindingEvent();
        self.getRecentReport();
        self.getRecentReportInterval();        
    };

    self.getRecentReportInterval = function () {
        self.Tables.intervalId = setInterval(function () {
            var now = moment();
            var currentTime = now.format("YYYY/MM/DD HH:mm:ss");
            var startOfHour = now.format("YYYY/MM/DD HH:00:00");
            var timeDiff = moment(startOfHour).diff(currentTime, "seconds", true);
            self.params.IsAutoRefresh = true;
            if (self.Tables.ddlMode.val() == self.enums.Mode.Recent && timeDiff == 0) {
                self.getRecentReport();
            }
        }, 1000);
    };

    self.setupCalendar = function (dateTime, txtdate) {
        $('#' + txtdate).datetimepicker({
            showSecond: true,
            dateFormat: 'M dd, yy',
            timeFormat: 'HH:mm:ss',
            hourGrid: 3,
            minuteGrid: 5,
            secondGrid: 5,
            separator: ' ',
            defaultTimezone: '-4',
            minDateTime: new Date(minDate),
            onSelect: function () {                
            }
        });

        if (dateTime != null && dateTime.toString() !== "") {
            $('#' + txtdate).val(dateTime).datetimepicker('setDate', (new Date(dateTime)));
        }
    };

    self.bindingEvent = function () {
        self.Tables.txtAmount.ForceDecimalOnly().ForceCurrencyOnly();

        self.Tables.ddlMode.change(function () {
            self.changeMode();
        });

        self.Tables.btnGenerate.click(function () {
            if (self.Tables.ddlMode.val() === self.enums.Mode.Recent) {
                self.params.IsAutoRefresh = false;
                self.getRecentReport();
            } else if (self.Tables.ddlMode.val() === self.enums.Mode.Historycal) {
                self.getHistoryReport();
            }
        });

        self.Tables.showHideFilter.click(function () {
            if ($(this).hasClass("hide-filter-icon")) {
                $(this).removeClass("hide-filter-icon");
                $(this).addClass("show-filter-icon");
            }
            else {
                $(this).removeClass("show-filter-icon");
                $(this).addClass("hide-filter-icon");
            }

            $(".filter-form").slideToggle();
        });

        self.Tables.iconVolumeControll.live("click", function () {
            if ($(this).hasClass("icon-volume")) {
                $(this).removeClass("icon-volume");
                $(this).addClass("icon-mute-volume");
                self.enums.isMuteVolume = true;
            } else {
                $(this).removeClass("icon-mute-volume");
                $(this).addClass("icon-volume");
                self.enums.isMuteVolume = false;
            }
        });
    };

    self.changeMode = function () {
        self.enums.isMuteVolume = false;
        if (self.Tables.ddlMode.val() === self.enums.Mode.Recent) {
            self.Tables.timeRangeContainer.addClass("display-none");
            self.getRecentReport();
            self.getRecentReportInterval();
        } else if (self.Tables.ddlMode.val() === self.enums.Mode.Historycal) {
            self.Tables.timeRangeContainer.removeClass("display-none");
            clearInterval(self.Tables.intervalId);
            self.Tables.note.html(" Report is not automatically refreshed");
        }
    };

    self.getRecentReportParams = function () {
        self.params.Amount = self.Tables.txtAmount.val().replace(/,/g, '').replace('e', '').replace('E', '');
        var lastHour = moment().subtract('hours', self.enums.RecentReportTimeSpan);
        self.params.FromDate = moment(lastHour).format("YYYY-MM-DD HH:00:00");
        self.params.ToDate = moment(lastHour).format("YYYY-MM-DD HH:59:59.998");
    };

    self.getHistoryReportParams = function () {
        self.params.Amount = self.Tables.txtAmount.val().replace(/,/g, '').replace('e', '').replace('E', '');
        self.params.FromDate = self.Tables.txtFromDate.val();
        self.params.ToDate = self.Tables.txtToDate.val();
    };

    self.validateParams = function () {
        AccReport.popUp.hide();
        var notifyIds = [];
        var msg = '';
        var fromDate = moment(self.params.FromDate).format("YYYY/MM/DD HH:mm:ss");
        var toDate = moment(self.params.ToDate).format("YYYY/MM/DD HH:mm:ss");

        var timeDiff = moment(toDate).diff(fromDate, 'hours', true);
        var timeDiff2 = moment(fromDate).diff(toDate, 'seconds', true);

        if (isNaN(self.params.Amount)) {
            msg = "Invalid Win Loss(RM)";
        }
        else if (self.params.Amount == '' || parseFloat(self.params.Amount.replace(/,/g, '')) < 3000) {
            msg = "Win Loss (RM) must be greater or equal 3,000";
        }
        else if (timeDiff > self.enums.numberHoursInDay) {
            msg = "Report period must be within 24 hours.";
        }
        else if (timeDiff2 > 0) {
            msg = '"From" time input must be less than "To" time input.';
        }

        if (msg.length > 0) {
            AccReport.popUp.show(msg, notifyIds.join(','));
            AccReport.enableInput();
            return false;
        }

        return true;
    };

    self.getRecentReport = function () {
        AccReport.popUp.hide();
        var numberReportRows = 0;
        self.getRecentReportParams();

        var notifyIds = [];
        var msg = '';
        if (isNaN(self.params.Amount)) {
            msg = "Invalid Win Loss(RM)";
        }
        else if (self.params.Amount == '' || parseFloat(self.params.Amount.replace(/,/g, '')) < 3000) {
            msg = "Win Loss (RM) must be greater or equal 3,000";
        }

        if (msg.length > 0) {
            AccReport.popUp.show(msg, notifyIds.join(','));
            AccReport.enableInput();
            return;
        }

        AccReport.disableInput();
        $.ajax({
            type: "POST",
            url: window.rootUrl + "Misc/MemberWinLossOnRNGCasino/GetRecentReport",
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            data: JSON.stringify(self.params),
            beforeSend: function () {
                AccReport.disableInput();
            },
            success: function (rs) {
                self.renderRecentReport(rs);
                self.getNextTimeRefreshed();
                numberReportRows = parseInt($('#numOfMember').val());
            },
            error: function (jqXHR, event, ajaxSettings) {
                if (jqXHR.status &&
                        jqXHR.status == AccReportData.Constants.HtmlErrorCode.SystemError) {
                    var r = jQuery.parseJSON(jqXHR.responseText);
                    AccReport.popUp.show(r.message);
                }
            },
            complete: function () {
                if (numberReportRows > 0 && !self.enums.isMuteVolume) {
                    self.playNotificationAudio();
                }

                AccReport.enableInput();
            }
        });
    };

    self.renderRecentReport = function (gridOptions) {
        var title = '';
        title = gridOptions.TitleInfoText;

        if (self.enums.isMuteVolume) {
            title += '<div class="volume"><span class="volume-controll-icon icon-mute-volume"></span></div>';
        } else {
            title += '<div class="volume"><span class="volume-controll-icon icon-volume"></span></div>';
        }

        gridOptions.columns[0].ShowCollapseExpand = true;

        if (!self.Tables.historyReportContainer.hasClass("display-none")) {
            self.Tables.historyReportContainer.addClass("display-none");
        }

        self.Tables.recentReportContainer.removeClass("display-none");
        var titleInfo = {
            text: title,
            showExcelButton: true,
            exportExcelFunction: function () {
                self.exportRecentReport();
            }
        };

        gridOptions.showTitle = true;
        gridOptions.titleInfo = titleInfo;
        gridOptions.showTree = true;
        gridOptions.gridExpandHeight = 220;
        self.Tables.recentReportContent.removeClass("display-none").fanexGrid(gridOptions);
    };

    self.getHistoryReport = function () {
        self.getHistoryReportParams();

        if (self.validateParams() == true) {
            AccReport.disableInput();
            AccReport.utility.CallAjax(self,
                window.rootUrl + "Misc/MemberWinLossOnRNGCasino/GetHistoryReport",
                self.params,
                self.renderHistoryReport,
                self.renderHistoryReport,
                AccReport.enableInput);
        }
    };

    self.renderHistoryReport = function (gridOptions) {
        if (!self.Tables.recentReportContainer.hasClass("display-none")) {
            self.Tables.recentReportContainer.addClass("display-none");
        }

        self.Tables.historyReportContainer.removeClass("display-none");
        var titleInfo = {
            text: gridOptions.TitleInfoText,
            showExcelButton: true,
            exportExcelFunction: function () {
                self.exportHistoryReport();
            }
        };

        gridOptions.showTitle = true;
        gridOptions.titleInfo = titleInfo;
        gridOptions.showTree = true;
        gridOptions.gridExpandHeight = 220;
        self.Tables.historyReportContent.removeClass("display-none").fanexGrid(gridOptions);
    };

    self.exportRecentReport = function () {
        AccReport.downloadFile(window.rootUrl + "Misc/MemberWinLossOnRNGCasino/ExportRecentReport", self.params);
    };

    self.exportHistoryReport = function () {
        AccReport.downloadFile(window.rootUrl + "Misc/MemberWinLossOnRNGCasino/ExportHistoryReport", self.params);
    };

    self.getNextTimeRefreshed = function () {
        $.ajax({
            type: "POST",
            url: window.rootUrl + "Misc/MemberWinLossOnRNGCasino/GetNextTimeRefreshed",
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            data: null,
            async: false,
            beforeSend: function () {
                AccReport.disableInput();
            },
            success: function (data) {
                $('#nextRefreshTime').val(data.NextTimeRefreshed);
                $('#numOfMember').val(data.NumOfMember);
                self.Tables.note.html("Report will be automatically refreshed at: " + $('#nextRefreshTime').val());
            },
            error: function (event, jqXHR, ajaxSettings) {
            },
            complete: function () {
                AccReport.enableInput();
            }
        });
    };

    self.initFlashMedia = function () {
        var flashObjectUrl = window.rootUrl + "Areas/Misc/Views/MemberWinLossOnRNGCasino/Media/player.swf";
        AudioPlayer.setup(flashObjectUrl, {
            width: 290
        });
    };

    self.playMedia = function () {
        var mediaUrl = window.rootUrl + "Areas/Misc/Views/MemberWinLossOnRNGCasino/Media/koekoeksklok.mp3";
        AudioPlayer.embed("audioplayer", {
            soundFile: mediaUrl,
            titles: "Title",
            artists: "Artist name",
            autostart: "yes"
        });
    };

    self.playNotificationAudio = function () {
        self.initFlashMedia();
        self.playMedia();
    };
};

var rngCasinoReport;

$(document).ready(function () {
    rngCasinoReport = new Fanex.AccountingReports.Misc.RNGCasinoReport();
    rngCasinoReport.InitPage();
});