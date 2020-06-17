/* File Created: November 12, 2012 */
$(document).ready(function () {
    AccReport.initialize();
    //    for site capture tool
    //    setTimeout(function () {
    //        $("#btnGenerate").trigger("click");
    //    }, 1500);
    //    setInterval(AccReport.reCheckIn, 20000);

    AccReport.legendManager.getReportLegend();
});

var FormatConstant = {
    INT_NUMBER: '0,0',
    TWO_DECIMAL_NUMBER: '0,0.00',
    FOUR_DECIMAL_NUMBER: '0,0.0000',
    TWO_DECIMAL_PERCENT_NUMBER: '0,0.00%',
    MMM_DD_YYYY_STRING: 'MMM DD, YYYY',
    M_DD_YY_STRING: 'M dd, yy',
    MM_DD_YYYY_WITH_SLASH_STRING: 'MM/dd/yyyy',
    MMM_YY_STRING: 'MMM YY',
    MMM_YY_STRING_MONTH_PICKER: 'MMM yyyy',
    MMM_DD_YY_STRING_MONTH_PICKER: 'MMM dd, yyyy',
    MM_DD_YYYY_WITH_SLASH_STRING_MOMENT: 'M/D/YYYY',
    MMM_DD_YYYY_STRING_MOMENT: 'MMM D, YYYY',
    MMM_DD_YYYY_H_M_STRING: 'MMM DD, YYYY HH:mm',
    MMM_DD_YYYY_H_M_S_A_STRING: 'MMMM DD, YYYY h:mm:ss A',
    PERCENT_NUMBER: '0%'
};


var AccReportData = {
    Constants: {
        HtmlErrorCode: {
            SystemError: 500,
            AuthorityError: 403
        },

        SiteType: {
            All_Site: 0,
            Credit: 1,
            Deposit: 2
        },

        SportType: {
            Financial: 201,
            Soccer: 1,
            Racing: 151,
            Casino: 251,
            Bingo: 252,
            _2hReport_MixParlay: 99,
            _2hReport_CombinationMixParlay: 98,
            _2hReport_OtherSports: 3,
            NumberGame: 161,
            LiveCasino: 162,
            VirtualSports: 180,
            VirtualHorseRacing: 181,
            VirtualGreyhounds: 182,
            VirtualSpeedway: 183,
            VirtualMotorsports: 184,
            VirtualCycling: 185,
            Keno: 202,
            SoccerMajorBetType: 1,
            SoccerMinorBetType: 2,
            Bettrade: -2,
            ColossusBet: 204,
            MetricGamingSuperLive: -1
        },

        BetType: {
            Outright: 10,
            Casino: 71,
            P2P: 72,
            Bingo: 73,
            SystemParlay: 29
        },
        ReportMarginBottom: 15,

        RoleId: {
            Member: 1,
            Agent: 2,
            Master: 3,
            Super: 4
        }
    }
};

var AccReport = function ($) {
    var MessageType = {
        Success: 1,
        Warning: 2,
        Error: 3,
        Notification: 4
    };

    var SortDirection = {
        Desc: 'DESC',
        Asc: 'ASC',
        None: ''
    };

    return {
        disableInput: function () {
            try { $.blockUI(); } catch (e) { }
        },
        enableInput: function () {
            try { $.unblockUI(); } catch (e) { }
        },
        accReportRoot: null,
        accReportPopUp: null, //
        initialize: function () {
            AccReport.accReportRoot = window.rootUrl;
            AccReport.accReportPopUp = $("#dialog-popup");
            AccReport.pageTracker.doTracking();
        },
        popUp:
        {
            arrayHighLightControl: [],
            show: function (message, targets, messageType) {
                AccReport.accReportPopUp
                .removeClass("err")
                .removeClass("info")
                .empty()
                .text(message);

                var bodyWith = $(window).width();
                var popUpWidth = AccReport.accReportPopUp.width();
                var leftPosition = Math.round((bodyWith - popUpWidth) / 2);
                AccReport.accReportPopUp.css("left", leftPosition);
                //// 1: Infor, 2: Warning, 3: undefined, Error
                if (typeof (messageType) == 'undefined') {
                    AccReport.accReportPopUp
                    .addClass("err")
                    .append('<a class="close" title="Close Message" href="javascript:AccReport.popUp.hide();"></a>');

                    if (typeof (targets) != 'undefined') {
                        AccReport.popUp.highLightControl(targets);
                    }
                }
                else {
                    switch (messageType) {
                        case MessageType.Success: ////Success
                            AccReport.accReportPopUp.addClass("info");
                            setTimeout(function () {
                                AccReport.accReportPopUp.hide();
                            }, 4000);
                            break;
                        case MessageType.Warning:
                            break;
                        case MessageType.Error:
                            break;
                        case MessageType.Notification:
                            AccReport.accReportPopUp.addClass("info")
                            .append('<a class="close" title="Close Message" href="javascript:AccReport.popUp.hide();"></a>');
                            break;
                    }
                }

                AccReport.accReportPopUp.show();
            },
            hide: function () {
                AccReport.accReportPopUp.hide();
            },
            highLightControl: function (targets) {
                var controlIds = targets.split(',');
                for (var i = 0; i < controlIds.length; i++) {
                    var controlId = controlIds[i];
                    if (controlId.indexOf("#") < 0) {
                        controlId = "#" + controlId;
                    }
                    AccReport.popUp.arrayHighLightControl.push(controlId);
                    var control = $(controlId);
                    control.addClass("input-validation-error");
                    if (control.hasClass("ui-multiselect")) {
                        control.next().addClass("input-validation-error");
                    }
                }

            },
            removeHighLight: function () {
                var controlIds = AccReport.popUp.arrayHighLightControl;
                for (var i = 0; i < controlIds.length; i++) {
                    var control = $(controlIds[i]);
                    control.removeClass("input-validation-error");
                    if (control.hasClass("ui-multiselect")) {
                        control.next().removeClass("input-validation-error");
                    }
                }
                AccReport.popUp.arrayHighLightControl = [];
                AccReport.popUp.hide();
            }
        },
        pageTracker: {
            doTracking: function () {
                if (typeof (isDisableMostvisitedPage) != 'undefined' && isDisableMostvisitedPage == false && currentMenuId != 0) {
                    $.ajax({
                        type: "POST",
                        url: window.rootUrl + "Home/UpdateMostVisitedPageCount",
                        contentType: "application/json; charset=utf-8",
                        dataType: "json",
                        data: JSON.stringify({
                            MenuId: currentMenuId
                        }),
                        timeout: 120000,
                        success: function () {
                        },
                        error: function (event, jqXHR, ajaxSettings) {
                        },
                        complete: function () {
                        }
                    });
                }
            }
        },
        downloadFile: function (url, data, method) {
            //url and data options required
            if (url && data) {
                //data can be string of parameters or array/object
                data = typeof data == 'string' ? data : $.param(data);
                if (url.indexOf('MAMonitorTool') > 0) {
                    $.fileDownload(url, { httpMethod: "POST", data: data });
                }
                else {
                    var downloadUrl = url + "?" + data;
                    var iframe = $('body iframe');
                    if (iframe.length == 0) {
                        iframe = $('<iframe width="0" height="0" class="display-none"></iframe>');
                        iframe.appendTo('body');
                    }

                    iframe.attr('src', downloadUrl);
                }
            }
        },
        reCheckIn: function () {
            //$.ajax({
            //    url: AccReport.accReportRoot + "UserLogin/Login/ReCheckLogin",
            //    type: "POST",
            //    dataType: "json",
            //    success: function (result) {
            //        if (typeof (result) != 'undefined' && result != null) {
            //            if (result.IsKickedOut == "1" || result.IsKickedOut == "2") {
            //                alert('Another session has been logged in or your session is expired. Forced to logout!');
            //                window.location.href = AccReport.accReportRoot + "UserLogin/Login/LogOut";
            //            }
            //            else if (result.IsKickedOut == "3") {
            //                window.location.href = AccReport.accReportRoot + "UserLogin/Common/Offline";
            //            }
            //        }
            //    },
            //    error: function (request, status, error) {
            //    }
            //});
        },
        gridSorterManager: {
            sortColumn: function (column, sortFunction) {
                var sortDirection = column.attr("sortDirection");
                if (sortDirection == null) {
                    sortDirection = SortDirection.None;
                }
                var updatedSortDirection = SortDirection.None;

                switch (sortDirection) {
                    case SortDirection.None:
                        column.removeClass("asc").addClass("desc");
                        updatedSortDirection = SortDirection.Desc;
                        break;
                    case SortDirection.Desc:
                        column.removeClass("desc").addClass("asc");
                        updatedSortDirection = SortDirection.Asc;
                        break;
                    case SortDirection.Asc:
                        column.removeClass("asc").addClass("desc");
                        updatedSortDirection = SortDirection.Desc;
                        break;
                }

                sortFunction(column, updatedSortDirection);
                column.attr("sortDirection", updatedSortDirection);
            },
            resetColumns: function (columns) {
                columns.removeClass("asc").removeClass("desc");
                columns.attr("sortDirection", SortDirection.None);
            }
        },
        utility: {
            StringFormat: function (strValue, paramArr) {
                var len = paramArr.length;
                for (var i = 0; i < len; i++) {
                    var paramInfo = paramArr[i];
                    var formatIndex = '{' + i + '}';
                    while (strValue.indexOf(formatIndex) != -1) {
                        strValue = strValue.replace(formatIndex, paramInfo);
                    }
                }

                return strValue;
            },

            CallAjax: function (reportHandler,
                url,
                requestParams,
                successFunction,
                errorFunction,
                completeFunction,
                isKeepingMessage) {
                $.ajax({
                    type: "POST",
                    url: url,
                    contentType: "application/json; charset=utf-8",
                    dataType: "json",
                    data: requestParams ? JSON.stringify(requestParams) : null,
                    beforeSend: function () {
                    },
                    success: function (rs) {
                        if (!isKeepingMessage) {
                            AccReport.popUp.hide();
                        }

                        if (successFunction && typeof (successFunction) == 'function') {
                            successFunction.call(reportHandler, rs);
                        }
                    },
                    error: function (jqXHR, event, ajaxSettings) {
                        if (jqXHR.status &&
                            jqXHR.status == AccReportData.Constants.HtmlErrorCode.SystemError) {
                            var r = jQuery.parseJSON(jqXHR.responseText);
                            AccReport.popUp.show(r.message);
                        }

                        if (errorFunction && typeof (errorFunction) == 'function') {
                            errorFunction.call(reportHandler);
                        }
                    },
                    complete: function () {
                        if (completeFunction && typeof (completeFunction) == 'function') {
                            completeFunction();
                        }
                    }
                });
            }
        },
        legendManager: {
            init: function () {
                var systemName = "ACC-NAP";
                fanexLegendManager._params.getReportLegend.System = systemName;
                fanexLegendManager._params.getReportNote.System = systemName;
            },
            getReportLegend: function () {
                try {
                    fanexLegendManager.initPage(menuId, functionName, menuName, "#previewReportLegendDialog");
                    fanexLegendManager.getReportLegend($("#filter-bar"), false);

                    if (isLegendEnable.toLowerCase() == "true") {
                        $(".note-info-message").addClass("display-none");
                    }
                }
                catch (ex) {
                }
            }
        }
    };
} (jQuery);

String.prototype.format = function () {
    var str = this;
    for (var i = 0; i < arguments.length; i++) {
        var reg = new RegExp("\\{" + i + "\\}", "gm");
        str = str.replace(reg, arguments[i]);
    }

    return str;
}
