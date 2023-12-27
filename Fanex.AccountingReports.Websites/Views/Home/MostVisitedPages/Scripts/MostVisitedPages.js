
$(document).ready(function () {
    if (isDisableMostvisitedPage == false) {
        maxPages = parseInt(maxPages);
        fixBugsBrowser.fixIndexOfBug();
        var mostVisitedPages = new Fanex.AccountingReports.Home.MostVisitedPages();
        mostVisitedPages.initPage();
    }
});

Namespace.Register("Fanex.AccountingReports.Home.MostVisitedPages");
Fanex.AccountingReports.Home.MostVisitedPages = function () {
    var self = this;

    var _tables = {
        mostVisitedPagesContent: null,
        mostVisitedPagesTiles: null,
        mostVisitedPagesSites: null,
        mostVisitedPagesThumnails: null,
        mostVisitedPagesSortableContainer: null,
        topMenu: null,
        tileWidth: 0,
        tileHeight: 0,
        mostVisitedPagesSortableOptions: null
    };

    var _options = {
        maxPages: window.maxPages,
        sourceFolder: "Views/Home/MostVisitedPages"
    };

    var _cacheData = {
        mostVisitedPagesResult: null,
        favouriteReportsResult: null,
        mixSitesResult: null
    };

    self.initPage = function () {
        _tables.topMenu = $(document).find("body>header>nav");
        _tables.mostVisitedPagesSortableContainer = $(".tile-page-content");
        $(window).resize(function () {
            setTimeout(function () {
                resizeTile();
            }, 300);
        });

        _tables.mostVisitedPagesContent = $("#page-list");
        _tables.mostVisitedPagesSortableOptions = {
            cursor: 'move',
            items: ".tile",
            fixed: ".pinnedtile",
            cancel: ".pinnedtile",
            start: function (event, ui) {
            },
            stop: function (event, ui) {
                ui.item.find(".most-visited").removeClass("ondrag-site");
            }
        };
        getMostVisitedPages();
    };
    isInRange = function (position, maxPages) {
        return 1 <= position && position <= maxPages;
    };
    bindingEvent = function () {
        $(".pin-button").unbind("click");
        $(".remove-button").unbind("click");
        $(".pin-button").click(function (e) {
            pinMostvisitedPage(e, $(this));
        });
        $(".remove-button").click(function (e) {
            deleteMostVisitedPage(e, $(this));
        });
    };
    getMostVisitedPageFromButton = function (button) {
        return button.parent().parent().parent();
    };
    resizeTile = function () {
        try {
            var width = $(window).width();
            var height = $(window).height();

            var topMenuHeight = _tables.topMenu.height();

            var itemPerRow = 2;
            var rowNumber = (_options.maxPages / 2).toFixed(0);
            _tables.tileWidth = ((width - 300) / itemPerRow).toFixed(0);

            if (rowNumber != 0) {
                _tables.tileHeight = ((height - topMenuHeight - 198) / rowNumber).toFixed(0);
            }
            else {
                _tables.tileHeight = height - topMenuHeight - 198;
            }

            _tables.mostVisitedPagesThumnails.css("height", _tables.tileHeight + "px");
            _tables.mostVisitedPagesSites.css("height", _tables.tileHeight + "px");

            if (window.chrome) {
                // is chrome 
                var disableTiles = _tables.mostVisitedPagesContent.find(".disable-drag-drop .most-visited");
                disableTiles.removeClass("invisible");
                disableTiles.find(".pin-button").addClass("invisible");
                disableTiles.find(".remove-button").addClass("invisible");
                _tables.mostVisitedPagesContent.find(".most-visited-site .most-visited").css("width", _tables.tileWidth);
                _tables.mostVisitedPagesSites.css("background-color", "transparent");
                _tables.mostVisitedPagesSites.css("margin-bottom", "0");
                disableTiles = _tables.mostVisitedPagesContent.find(".disable-drag-drop")
                                                                  .css("margin-top", "0px")
                                                                  .css("margin-bottom", "-17px");
            }
        } catch (e) {
        }
    };
    resetLayout = function () {
        // This function use for resetting layout after drag/drop, pin, delete site
        var html = '';
        html += '<div class="tile-page-content">';
        html += '<div class="tileContainer">';
        for (var index = 1; index <= _options.maxPages; index++) {
            html += '<div class="tile col_6 most-visited-' + index + '">\
                        <div class="btnGroup">\
                            <div class="btnGroupContainer">\
                                <span class="pin-button" title="Pin this site at its current position"></span>\
                                <span class="remove-button" title="Remove this site"></span>\
                            </div>\
                        </div>\
                        <div class="reportInfo">\
                            <div class="reportInfoContainer">\
                                <a class="reportLink"></a>\
                            </div>\
                        </div>\
                    </div>';
        }

        html += '</div>';
        html += '</div>';
        _tables.mostVisitedPagesContent.empty().append(html);
        _tables.mostVisitedPagesSortableContainer = $(".tile-page-content");
        _tables.mostVisitedPagesTiles = _tables.mostVisitedPagesContent.find(".tile");
        _tables.mostVisitedPagesSites = _tables.mostVisitedPagesContent.find(".most-visited-site");
        _tables.mostVisitedPagesThumnails = _tables.mostVisitedPagesContent.find(".thumbnail-wrapper");
        bindingEvent();
    };
    getMostVisitedPages = function () {
        AccReport.disableInput();
        $.ajax({
            type: "POST",
            url: window.rootUrl + "Home/GetMostvisitedPages",
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            data: null,
            timeout: 120000,
            success: function (rs) {
                renderDashboard(rs);
                bindingEvent();
            },
            error: function (event, jqXHR, ajaxSettings) {
                renderDashboard(null);
            },
            complete: function () {
                AccReport.enableInput();
            }
        });
    };
    renderDashboard = function (mostVisitedPages) {
        _tables.mostVisitedPagesContent.addClass("display-none");
        renderMostVisitedPages(mostVisitedPages);
        _tables.mostVisitedPagesTiles = _tables.mostVisitedPagesContent.find(".tile");
        _tables.mostVisitedPagesSites = _tables.mostVisitedPagesContent.find(".most-visited-site");
        _tables.mostVisitedPagesThumnails = _tables.mostVisitedPagesContent.find(".thumbnail-wrapper");
        _tables.mostVisitedPagesContent.removeClass("display-none");
        //        resizeTile();
    };
    renderMostVisitedPages = function (rs) {
        var i = 0;
        _cacheData.mostVisitedPagesResult = rs;
        if (rs && rs.length > 0) {
            var len = rs.length;
            var mostvisitedpageDOM;

            for (; i < len; i++) {
                var mostvisitedpage = rs[i];
                mostvisitedpageDOM = _tables.mostVisitedPagesContent.find(".most-visited-" + (i + 1));
                mostvisitedpageDOM.attr("menuId", mostvisitedpage.MenuId);

                var pinButton = mostvisitedpageDOM.find(".pin-button");
                if (mostvisitedpage.Position && mostvisitedpage.Position != 0 &&
                    isInRange(mostvisitedpage.Position, maxPages)) {
                    mostvisitedpageDOM.addClass("pinnedtile");
                    pinButton.attr("title", "Unpin this site");
                    pinButton.attr("pinned", true);
                } else {
                    mostvisitedpageDOM.removeClass("pinnedtile");
                    pinButton.attr("title", "Pin this site at its current position");
                    mostvisitedpageDOM.find(".pin-button").removeAttr("pinned");
                }

                if (mostvisitedpage.MenuName) {
                    var mostvisitedLink = mostvisitedpageDOM.find(".reportLink");
                    mostvisitedLink.attr("title", mostvisitedpage.MenuName + ": " + mostvisitedpage.MenuURL);

                    var href = window.rootWebPath.replace(/\/$/g, '') + "/" +
                               mostvisitedpage.MenuURL.replace(/^\//g, '')
                    mostvisitedLink.attr("href", href);
                    var title = mostvisitedpage.MenuName;
                    mostvisitedLink.html(title);
                }
            }
        }

        var isEvenTile = i % 2 == 1;

        for (; i <= _options.maxPages; i++) {
            mostvisitedpageDOM = _tables.mostVisitedPagesContent.find(".most-visited-" + (i + 1));
            mostvisitedpageDOM.addClass("disable-drag-drop").addClass("pinnedtile");
            mostvisitedpageDOM.addClass("dashed-border")
                              .find("reportInfoContainer").addClass("invisible");

            if (!isEvenTile) {
                mostvisitedpageDOM.addClass("margin_10");
            }
            else {
                isEvenTile = false;
            }
        }

        _tables.mostVisitedPagesSortableContainer.fixedsortable(_tables.mostVisitedPagesSortableOptions)
                                                 .disableSelection();
    };
    deleteMostVisitedPage = function (e, removeButton) {
        e.preventDefault();
        var mostVisitedPage = getMostVisitedPageFromButton(removeButton);
        var menuId = parseInt(mostVisitedPage.attr("menuId"));

        var data = {
            MenuId: menuId
        };

        $.ajax({
            type: "Post",
            url: window.rootUrl + "Home/DeleteMostVisitedPage",
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            data: JSON.stringify(data),
            timeout: 120000,
            success: function (rs) {
                _cacheData.mostVisitedPagesResult = rs;
                resetLayout();
                renderDashboard(_cacheData.mostVisitedPagesResult);
            },
            error: function (event, jqXHR, ajaxSettings) {
                if (_cacheData.mostVisitedPagesResult) {
                    _cacheData.mostVisitedPagesResult = $.grep(_cacheData.mostVisitedPagesResult, function (value) {
                        return value.MenuId != menuId;
                    });
                }

                resetLayout();
                renderDashboard(mostVisitedPagesResult);
            },
            complete: function () {
            }
        });
    };
    pinMostvisitedPage = function (e, pinButton) {
        e.preventDefault();
        var mostVisitedPage = getMostVisitedPageFromButton(pinButton);
        var menuId = parseInt(mostVisitedPage.attr("menuId"));
        var position = 0;
        var isPinned = false;
        if (pinButton.attr("pinned") == "true" || pinButton.attr("pinned") == true) {
            pinButton.removeAttr("pinned");
            pinButton.attr("title", "Pin this site at its current position");
            mostVisitedPage.removeClass("pinnedtile");
        }
        else {
            pinButton.attr("pinned", true);
            pinButton.attr("title", "Unpin this site");
            position = mostVisitedPage.index();
            isPinned = true;
            mostVisitedPage.addClass("pinnedtile");
        }

        mostVisitedPage = {
            MenuId: menuId,
            IsPinned: isPinned,
            Position: isPinned ? position + 1 : 0
        };

        var data = {
            MenuId: menuId,
            Position: mostVisitedPage.Position
        }

        $.ajax({
            type: "Post",
            url: window.rootUrl + "Home/UpdateMostVisitedPagePosition",
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            data: JSON.stringify(data),
            timeout: 120000,
            success: function (rs) {
                updatePinnedPositionOfMostVisitedPage(mostVisitedPage);
                //We must reload grid because fixedsortable plugin not allow we rebind options (fixed item)
                resetLayout();
                renderMostVisitedPages(_cacheData.mostVisitedPagesResult);
                resizeTile();
                _tables.mostVisitedPagesSortableContainer.fixedsortable(_tables.mostVisitedPagesSortableOptions)
                                                        .disableSelection();
            },
            error: function (event, jqXHR, ajaxSettings) {
            },
            complete: function () {
            }
        });
    };
    updatePinnedPositionOfMostVisitedPage = function (mostVisitedPage) {
        var mostVisitedPageLength = _cacheData.mostVisitedPagesResult.length;
        var oldMostVisitedPage;
        for (var i = 0; i < mostVisitedPageLength; i++) {
            oldMostVisitedPage = _cacheData.mostVisitedPagesResult[i];
            if (oldMostVisitedPage && oldMostVisitedPage.MenuId == mostVisitedPage.MenuId) {
                _cacheData.mostVisitedPagesResult[i].IsPinned = mostVisitedPage.IsPinned;
                _cacheData.mostVisitedPagesResult[i].Position = mostVisitedPage.Position;
                break;
            }
        }

        if (mostVisitedPage.Position > 0) {
            if (_cacheData.mostVisitedPagesResult) {
                _cacheData.mostVisitedPagesResult = $.grep(_cacheData.mostVisitedPagesResult, function (value) {
                    return value.MenuId != mostVisitedPage.MenuId;
                });
            }

            _cacheData.mostVisitedPagesResult.splice(mostVisitedPage.Position - 1, 0, oldMostVisitedPage);
        }
    };
}