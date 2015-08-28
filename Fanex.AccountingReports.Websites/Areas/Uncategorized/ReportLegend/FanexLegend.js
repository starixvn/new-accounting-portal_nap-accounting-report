

Namespace.Register("Fanex.Legend.CookieManager");
Fanex.Legend.CookieManager = function () {
    var self = this;
    var _options = {
        saveCookieOptions: {
            expires: 365,
            path: '/'
        }
    };
    var _cache = {
        ReportNoteCookie: "ReportNoteCookie",
        ReportNoteCountCookie: "ReportNoteCountCookie"
    };
    self.resetCookies = function () {
        $.cookie(_cache.ReportNoteCookie, null, _options.saveCookieOptions);
        $.cookie(_cache.ReportNoteCountCookie, null, _options.saveCookieOptions);
    };
    self.getReportNoteCookie = function () {
        var reportNoteCookieValue = $.cookie(_cache.ReportNoteCookie);
        var notes = [];
        if (reportNoteCookieValue) {
            notes = $.parseJSON(reportNoteCookieValue);
        }

        return notes;
    };
    self.getReportNoteCountCookie = function () {
        var reportNoteCountCookieValue = $.cookie(_cache.ReportNoteCountCookie);
        var noteCounts = [];
        if (reportNoteCountCookieValue) {
            noteCounts = $.parseJSON(reportNoteCountCookieValue);
        }

        return noteCounts;
    };
    self.resetAllNoteCount = function (hash) {
        var noteCounts = self.getReportNoteCountCookie();
        var notesCountsLength = noteCounts.length;
        var isExisted = false;

        for (var i = 0; i < notesCountsLength; i++) {
            var noteCount = noteCounts[i];
            if (noteCount &&
                noteCount.UserId == currentUserId &&
                noteCount.HashContent == hash) {
                noteCounts[i].Count = 1;
                isExisted = true;
            }
        }

        if (!isExisted) {
            noteCounts.push({
                Count: 1,
                HashContent: hash,
                UserId: currentUserId
            });
        }

        $.cookie(_cache.ReportNoteCountCookie,
                  JSON.stringify(noteCounts),
                  _options.saveCookieOptions);
    };
    self.updateNoteCount = function (params, hash) {
        var noteCounts = self.getReportNoteCountCookie();
        var notesCountsLength = noteCounts.length;
        var isExisted = false;

        for (var i = 0; i < notesCountsLength; i++) {
            var noteCount = noteCounts[i];
            if (noteCount &&
                noteCount.UserId == currentUserId &&
                noteCount.HashContent == hash) {
                if (noteCount.Count <= noteViewCountToCollapse) {
                    noteCounts[i].Count++;
                }

                isExisted = true;
            }
        }

        if (!isExisted) {
            noteCounts.push({
                Count: 1,
                HashContent: hash,
                UserId: currentUserId
            });
        }

        $.cookie(_cache.ReportNoteCountCookie,
                  JSON.stringify(noteCounts),
                 _options.saveCookieOptions);
    };
    self.saveNote = function (params, noteContent) {
        var hash = $.sha1(params.MenuID + "_" + params.System + "_" + noteContent);
        var notes = self.getReportNoteCookie();
        var notesLength = notes.length;
        var isExisted = false;

        for (var i = 0; i < notesLength; i++) {
            var note = notes[i];
            if (note &&
                note.MenuId == params.MenuID &&
                note.System == params.System) {
                if (note.HashContent != hash) {
                    notes[i].HashContent = hash;
                    self.resetAllNoteCount(hash);
                }
                else {
                    self.updateNoteCount(params, hash);
                }

                isExisted = true;
            }
        }

        if (!isExisted) {
            notes.push({
                MenuId: params.MenuID,
                System: params.System,
                HashContent: hash
            });

            self.updateNoteCount(params, hash);
        }

        $.cookie(_cache.ReportNoteCookie,
                  JSON.stringify(notes),
                 _options.saveCookieOptions);
    };
    self.isNotifyNote = function (params) {
        var hash = '';
        var notes = self.getReportNoteCookie();
        var notesLength = notes.length;
        var isExisted = false;

        for (var i = 0; i < notesLength; i++) {
            var note = notes[i];
            if (note &&
                note.MenuId == params.MenuID &&
                note.System == params.System) {
                hash = note.HashContent;
                isExisted = true;
            }
        }

        if (isExisted) {
            var noteCounts = self.getReportNoteCountCookie();
            var notesCountsLength = noteCounts.length;
            var isExistedNoteCount = false;

            for (var j = 0; j < notesCountsLength; j++) {
                var noteCount = noteCounts[j];
                if (noteCount &&
                    noteCount.UserId == currentUserId &&
                    noteCount.HashContent == hash) {
                    return noteCount.Count <= noteViewCountToCollapse;
                }
            }

            return true;
        }
    };
};

Namespace.Register("Fanex.Legend.TabManager");
Fanex.Legend.TabManager = function () {
    var self = this;

    var _tables = {
        legendWindow: null,
        tabs: null,
        tabContents: null,
        subTabs: null,
        subTabContents: null
    };
    var _cacheData = {
        currentTab: null,
        currentTabPreviewDialog: null
    };
    self.initPage = function (legendWindow) {
        cacheDOMElement(legendWindow);
        bindingEvent();
    };
    var cacheDOMElement = function (legendWindow) {
        _tables.legendWindow = legendWindow;
        _tables.tabs = legendWindow.find(".report-tab");
        _tables.tabContents = legendWindow.find(".content-tab");
        _cacheData.currentTab = legendWindow.find(".report-tab.current");

        _tables.subTabs = legendWindow.find(".report-sub-tab");
        _tables.subTabContents = legendWindow.find(".subTabContentContainer");
    };
    var bindingEvent = function () {
        _tables.tabs.unbind("click");
        _tables.tabs.click(function (e) {
            var tabButton = $(this);
            self.changeTab(tabButton);
        });

        _tables.subTabs.unbind("click");
        _tables.subTabs.click(function (e) {
            var tabButton = $(this);
            self.changeSubTab(tabButton);
        });
    };
    self.changeTab = function (tabButton) {
        _cacheData.currentTab.removeClass("current");
        _cacheData.currentTab.css(getReportTabContainer_ul_li_a_Css());
        _tables.tabContents.hide();
        tabButton.css(getReportTabContainer_ul_li_a_current_Css());
        tabButton.addClass("current");
        var tabContent = _tables.legendWindow.find("#" + tabButton.attr("rel"));
        tabContent.show();
        _cacheData.currentTab = tabButton;
    };
    self.changeSubTab = function (tabButton) {
        var reportTab = tabButton.parent().parent().parent();
        hideSubTab(tabButton, reportTab.find("." + tabButton.attr("rel")),
                   tabButton.attr("isShow") == "true");
    };
    var getReportTabContainer_ul_Css = function () {
        return 'display: inline-block; list-style: none outside none; margin: 0 5px 0 0';
    };
    var getReportTabContainer_ul_li_Css = function () {
        return {
            'display': 'inline-block',
            'list-style': 'none outside none',
            'margin': '0 5px 0 0'
        };
    };
    var getReportTabContainer_ul_li_a_Css = function () {
        return {
            'font-size': '12px',
            'font-weight': 'bold',
            'line-height': '28px',
            'padding': '6px 0.7em',
            'text-decoration': 'none',
            'background': 'none repeat scroll 0 0 #D4D4D4',
            'border-color': '#B1B1B1 #B1B1B1 #B1B1B1',
            'border-style': 'solid',
            'border-width': '1px',
            'cursor': 'pointer',
            'color': '#464646',
            'border-radius': '2px 2px 0 0'
        };
    };
    var getReportTabContainer_ul_li_a_current_Css = function () {
        return {
            'background': 'none repeat scroll 0 0 #EBEBEB',
            'border-color': '#B1B1B1 #B1B1B1 #EBEBEB',
            'font-weight': 'bold'
        };
    };
    var hideSubTab = function (tabButton, subTabContent, isHide) {
        if (isHide) {
            tabButton.css("background", 'url(' + inlineImageCollapse + ') no-repeat scroll 8px 8px #46A3CA');
            subTabContent.hide();
            tabButton.attr("isShow", false);
        }
        else {
            tabButton.css("background", 'url(' + inlineImageExpand + ') no-repeat scroll 8px 8px #46A3CA');
            subTabContent.show();
            tabButton.attr("isShow", true);
        }
    };
};

var fanexLegendManager = {
    _cookieManager: new Fanex.Legend.CookieManager(),
    _tabManager: new Fanex.Legend.TabManager(),
    _tables: {
        previewReportLegendDialog: null
    },
    _params: {
        getReportLegend: {
            MenuID: null,
            MenuName: '',
            FunctionName: null,
            System: "ACC-NAP",
            IsNote: false,
            CallBackFunction: 'fanexLegendManager.getReportLegendCallBack'
        },

        getReportNote: {
            MenuID: null,
            MenuName: '',
            FunctionName: null,
            System: "ACC-NAP",
            IsNote: true,
            CallBackFunction: 'fanexLegendManager.getReportNoteCallBack'
        }
    },

    _cacheData: {
        reportLegend: '',
        reportNote: '',
        reportNoteHeader: null,
        reportNoteContent: null,
        legendContainer: null,
        noteContainer: null
    },
    initPage: function (menuId, functionName, menuName, dialogToDisplayLegend) {
        menuId = parseInt(menuId);
        if (menuId && menuId != -1) {
            fanexLegendManager._params.getReportLegend.MenuID = parseInt(menuId);
            fanexLegendManager._params.getReportLegend.MenuName = menuName;
            fanexLegendManager._params.getReportLegend.FunctionName = functionName;

            fanexLegendManager._params.getReportNote.MenuID = parseInt(menuId);
            fanexLegendManager._params.getReportNote.MenuName = menuName;
            fanexLegendManager._params.getReportNote.FunctionName = functionName;
            fanexLegendManager._tables.previewReportLegendDialog = $(dialogToDisplayLegend);
        }
    },
    getReportLegend: function (targetContainer, isNote) {
        if (!fanexLegendManager._params.getReportLegend.MenuID ||
            fanexLegendManager._params.getReportLegend.MenuID == -1) {
            return;
        }

        if (isNote) {
            fanexLegendManager._cacheData.noteContainer = $(targetContainer);
        }
        else {
            fanexLegendManager._cacheData.legendContainer = $(targetContainer);
        }

        if (isLegendEnable.toLowerCase() == "false") {
            if (fanexLegendManager._cacheData.legendContainer &&
                fanexLegendManager._cacheData.legendContainer.children().length == 0) {
                fanexLegendManager._cacheData.legendContainer.addClass("display-none");
            }

            return;
        }

        var url = window.rootUrl + getReportLegendUrl;

        //        url = "http://localhost:60280/Home/GetJsonpReportLegend";
        var data = fanexLegendManager._params.getReportLegend;
        if (isNote) {
            data = fanexLegendManager._params.getReportNote;
        }

        crossDomainManagement.crossDomainCall(url, data, function (rs) {
        }, function (event, jqXHR, ajaxSettings) {
        });
    },
    getReportLegendCallBack: function (rs) {
        fanexLegendManager._cacheData.reportLegend = rs.Reponse.replace("\\\"", "\"");
        fanexLegendManager.renderReportLegend(false);
    },
    renderLegend: function () {
        if (fanexLegendManager._cacheData.reportLegend &&
            fanexLegendManager._cacheData.reportLegend != '<br data-mce-bogus="1">' &&
            fanexLegendManager._cacheData.reportLegend != '') {
            var legendIcon = '<span id="reportLegend" class="reportLegend" title="" onclick="(fanexLegendManager || parent.fanexLegendManager).showReportLegend();"></span>';
            if (fanexLegendManager._cacheData.legendContainer.length === 0) {
                $("#body .content").prepend('<div id="filter-bar"></div>');
                fanexLegendManager._cacheData.legendContainer = $("#filter-bar");
            }

            var oldLegendIcon = fanexLegendManager._cacheData.legendContainer.find("#reportLegend");
            if (oldLegendIcon.length == 0) {
                if (fanexLegendManager._cacheData.legendContainer.length > 0) {
                    fanexLegendManager._cacheData.legendContainer.prepend($(legendIcon));
                }
            } else {
                oldLegendIcon.click(function () {
                    fanexLegendManager.showReportLegend();
                }).removeClass("display-none");
            }
        } else if (fanexLegendManager._cacheData.legendContainer &&
                   fanexLegendManager._cacheData.legendContainer.children().length == 0) {
            fanexLegendManager._cacheData.legendContainer.addClass("display-none");
        }
    },
    renderNote: function (targetContainer) {
        var isNotify = fanexLegendManager._cookieManager.isNotifyNote(fanexLegendManager._params.getReportNote);
        if (fanexLegendManager._cacheData.reportNote &&
            fanexLegendManager._cacheData.reportNote != '<br data-mce-bogus="1">' &&
            fanexLegendManager._cacheData.reportNote != '') {
            var reportNoteContainer = $('<div id="noteContainer" class="note-info-contaniner" title="">' +
                    '<div class="header"><span class="noteContainerIcon" isExpand="true">Note</span></div></div>');
            fanexLegendManager._cacheData.reportNoteContent = $('<div class="note-info-content"></div>');
            fanexLegendManager._cacheData.reportNoteContent.html(fanexLegendManager._cacheData.reportNote);
            reportNoteContainer.append(fanexLegendManager._cacheData.reportNoteContent);

            var oldLegendIcon = fanexLegendManager._cacheData.noteContainer.find("#noteContainer");
            if (oldLegendIcon.length == 0) {
                if (fanexLegendManager._cacheData.noteContainer.length > 0) {
                    fanexLegendManager._cacheData.noteContainer.append();
                    fanexLegendManager._cacheData.noteContainer.append(reportNoteContainer);
                }

                fanexLegendManager._cacheData.reportNoteHeader = reportNoteContainer.find(".header");
                fanexLegendManager._cacheData.reportNoteHeader.unbind("click");
                fanexLegendManager._cacheData.reportNoteHeader.click(function () {
                    var icon = fanexLegendManager._cacheData.reportNoteHeader.find(".noteContainerIcon");
                    fanexLegendManager.collapseExpandNote(icon.attr("isExpand") == "true");
                });

                fanexLegendManager.collapseExpandNote(!isNotify);
            }
        }
    },
    renderReportLegend: function (isNote) {
        if (isNote) {
            fanexLegendManager.renderNote();
        }
        else {
            fanexLegendManager.renderLegend();
        }
    },
    getReportNoteCallBack: function (rs) {
        //                _cookieManager.legend.resetCookies();
        fanexLegendManager._cacheData.reportNote = rs.Reponse.replace("\\\"", "\"");
        fanexLegendManager._cookieManager.saveNote(fanexLegendManager._params.getReportNote,
                                                   fanexLegendManager._cacheData.reportNote);
        fanexLegendManager.renderReportLegend(true);
    },
    collapseExpandNote: function (isExpand) {
        var icon = fanexLegendManager._cacheData.reportNoteHeader.find(".noteContainerIcon");
        if (isExpand == true) {
            icon.attr("isExpand", false);
            icon.addClass("collapse");
            fanexLegendManager._cacheData.reportNoteContent.addClass("hidden");
        }
        else {
            icon.attr("isExpand", true);
            fanexLegendManager._cacheData.reportNoteHeader.find(".noteContainerIcon").removeClass("collapse");
            fanexLegendManager._cacheData.reportNoteContent.removeClass("hidden");
        }
    },
    showReportLegend: function (iconId) {
        var icon = $(iconId);
        fanexLegendManager._tables.previewReportLegendDialog.empty().html(fanexLegendManager._cacheData.reportLegend);
        fanexLegendManager._tables.previewReportLegendDialog.removeClass("display-none");
        fanexLegendManager._tables.previewReportLegendDialog.dialog({
            width: 675,
            height: 675,
            position: "center",
            modal: true,
            title: fanexLegendManager._params.getReportLegend.MenuName + " Legend",
            resizable: false
        });

        fanexLegendManager.updateScrollbar();
        fanexLegendManager._tables.previewReportLegendDialog.scrollbars();
        fanexLegendManager._tables.previewReportLegendDialog.find(".scrollcontent").resize(function () {
            fanexLegendManager.updateScrollbar();
        });

        fanexLegendManager._tabManager.initPage(fanexLegendManager._tables.previewReportLegendDialog);
        fanexLegendManager.resetStyle();
    },
    updateScrollbar: function () {
        if (fanexLegendManager._tables.previewReportLegendDialog.hasClass("no_scroll_h")) {
            fanexLegendManager._tables.previewReportLegendDialog.find(".vscroller .scrollarea").css("bottom", 0);
        } else {
            fanexLegendManager._tables.previewReportLegendDialog.find(".vscroller .scrollarea").css("bottom", 20);
        }

        if (fanexLegendManager._tables.previewReportLegendDialog.hasClass("no_scroll_v")) {
            fanexLegendManager._tables.previewReportLegendDialog.find(".hscroller .scrollarea").css("right", 0);
        } else {
            fanexLegendManager._tables.previewReportLegendDialog.find(".hscroller .scrollarea").css("right", 15);
        }
    },
    resetStyle: function () {
        fanexLegendManager._tables.previewReportLegendDialog.find(".title-tab").css("font-size", "");
        fanexLegendManager._tables.previewReportLegendDialog.find(".content ul").css("font-size", "");
    }
};