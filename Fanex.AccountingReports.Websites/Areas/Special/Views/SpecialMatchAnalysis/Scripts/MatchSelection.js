
Namespace.Register("Fanex.AccountingReports.Special.SpecialMatchAnalysis.MatchSelection");
Fanex.AccountingReports.Special.SpecialMatchAnalysis.MatchSelection = function () {
    var self = this;

    self.CachedData = {
        MatchList: null
    };

    var _tables = {
        datepickerplugin: null,
        ddlLeague: null,
        btnGenerate: null,

        report: null,
        reportHeader: null,
        reportContent: null,
        reportTitle: null,
        reportColFooter: null
    };

    var _options = {
        report: {
            isShowPager: false,
            tblContentId: 'matchSelectionContent',
            fullHeight: true,
            gridHeight: 130,
            freezeHeight: 280,
            column: [150, 650],
            numberFreezeCol: 0
        }
    };

    var _params = {
        leagueSelection: {
            EventDate: null
        },

        matchSelection: {
            EventDate: null,
            Leagueids: ''
        }
    };

    self.initPage = function () {
        initGridScrollerTables();
        cachedDOMElements();
        renderDatePicker();
        getLeagues();
        bindingEvent();
    };
    var initGridScrollerTables = function () {
        $("#matchSelection").initGrid({
            isShowPager: false
        });
    };
    var cachedDOMElements = function () {
        _tables.btnGenerate = $("#btnGenerate");

        _tables.report = $("#matchSelection");
        _tables.reportContent = _tables.report.find("#matchSelectionContent tbody");
    };
    var bindingEvent = function () {
        _tables.btnGenerate.click(function () {
            self.btnGenerateClick();
        });
    };
    var renderDatePicker = function () {
        $('#datepickerplugin').nexDatePicker({
            nexDatePickerType: "single",
            firstDatePicker: {
                Id: "CheckDate",
                Label: "Date",
                InitDate: "",
                LabelCss: "lbl-field col_2 width-auto",
                TextboxCss: "edit-field col_6"

            },

            datePickerFormat: {
                changeMonth: true,
                changeYear: true,
                dateFormat: FormatConstant.M_DD_YY_STRING,
                minDate: new Date(minDate),
                maxDate: new Date(today)
            },

            onSelectDatePicker: function (selectedDate) {
                AccReport.popUp.removeHighLight();
                getLeagues();
            }
        });

        _tables.datepickerplugin = $("#datepickerplugin");
    };
    var getLeagues = function () {
        AccReport.disableInput();
        _params.leagueSelection.EventDate = _tables.datepickerplugin.nexDatePicker("getSingleDate");

        $.ajax({
            type: "POST",
            url: window.rootUrl + "Special/SpecialMatchAnalysis/GetSportLeague",
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            data: JSON.stringify(_params.leagueSelection),
            beforeSend: function () {
                AccReport.disableInput();
            },
            success: function (rs) {
                var leagues = [];
                var id = "ddlLeague";
                for (var i = 0; i < rs.length; i++) {
                    var sport = rs[i];
                    var sportLeagues = [];
                    for (var j = 0; j < sport.Leagues.length; j++) {
                        var league = sport.Leagues[j];
                        sportLeagues.push({
                            Id: id + "_Sport" + sport.Id.toString() + "_League" + league.Id.toString(),
                            Value: league.Id,
                            Text: league.Name,
                            Checked: true,
                            Enable: true,
                            CssClass: "",
                            Items: []
                        });
                    }

                    leagues.push({
                        Id: id + "_Sport" + sport.Id.toString(),
                        Value: sport.Id,
                        Text: sport.Name,
                        Checked: true,
                        Enable: true,
                        CssClass: "",
                        Items: sportLeagues
                    });
                }

                renderLeaguesDropdown(leagues);
            },
            error: function (event, jqXHR, ajaxSettings) {
                renderLeaguesDropdown([]);
            },
            complete: function () {
                _tables.ddlLeague = $("#ddlLeague");
                AccReport.enableInput();
            }
        });
    };
    var renderLeaguesDropdown = function (leagues) {
        var noItemText = '';
        if (leagues.length == 0) {
            noItemText = 'No league found.';
        }

        $("#ddlLeague").multiSelect(
        {
            minWidth: 500,
            maxHeight: 450,
            shadow: true,
            showHeader: true,
            isCheckAll: true,
            showFooter: false,
            checkAllText: 'All Leagues',
            isUsedCustomCheckAllText: true,
            customCheckAllText: 'All Leagues Selected',
            noItemText: noItemText,
            selectedList: 3,
            listData: leagues
        });
    };
    self.btnGenerateClick = function () {
        self.getReport();
    };
    self.getReport = function () {
        AccReport.disableInput();
        getParams();
        if (validateParams() == true) {
            $.ajax({
                type: "POST",
                url: window.rootUrl + "Special/SpecialMatchAnalysis/GetLeagueMatch",
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                data: JSON.stringify(_params.matchSelection),
                beforeSend: function () {
                    AccReport.disableInput();
                },
                success: function (rs) {
                    self.renderReport(rs);
                },
                error: function (event, jqXHR, ajaxSettings) {
                    self.renderReport(null);
                },
                complete: function () {
                    AccReport.enableInput();
                }
            });
        }
        else {
            AccReport.enableInput();
        }
    };
    var getParams = function () {
        _params.matchSelection.EventDate = _params.leagueSelection.EventDate;
        _params.matchSelection.Leagueids = _tables.ddlLeague.multiSelect('getCheckedItems').join(",");
    };
    var validateParams = function () {
        var isValid = true;
        var message = '';
        var notifyIds = [];
        if (_params.matchSelection.Leagueids.length == 0) {
            isValid = false;
            if (message == '') {
                message = "Please select at least one league!";
            }
            notifyIds.push("ddlLeague");
        }

        if (!isValid) {
            AccReport.popUp.removeHighLight();
            AccReport.popUp.show(message, notifyIds.join(","));
        } else {
            AccReport.popUp.removeHighLight();
        }

        return isValid;
    };
    self.renderReport = function (report) {
        self.CachedData.MatchList = report;
        if (report &&
            report.length > 0) {
            var len = report.length;
            var html = '';
            for (var i = 0; i < len; i++) {
                var league = report[i];

                var matchLength = league.Matches.length;
                html += '<tr class="subTotal">\
                            <td colspan="2" class="colLeagueHeadLine">\
                                <div class="colLeague" onclick="showHideLeagueInfo(this,\'' + league.Id + '\')">\
                                    <span class="colLeagueIcon expand"></span>' + league.Name +
                                '</div></td></tr>';

                for (var j = 0; j < matchLength; j++) {
                    var leagueDetail = league.Matches[j];
                    html += '<tr class="lic_' + league.Id + '">\
                                <td><div class="col150 link align-center" onclick="ViewMatchAnalysis(\'' +
                                        _params.matchSelection.EventDate + "','" + i + "','" + j + "')\">" +
                                        leagueDetail.MatchCode + '</div></td>\
                                <td><div class="col650">' + getMatchDetail(leagueDetail) +
                                '</div></td>\
                            </tr>';
                }
            }

            html = fixBugsBrowser.removeSpaceIE9(html);
            _tables.reportContent.empty().append(html);
        }
        else {
            self.renderReportNoData();
        }

        _tables.report.removeClass("display-none")
                      .gridScroller(_options.report);
        _tables.report.removeClass("display-none");
    };
    self.hideReport = function () {
        _tables.report.addClass("display-none");
    };
    self.showReport = function () {
        _tables.report.removeClass("display-none");
    };
    self.renderReportNoData = function () {
        var html = '<tr>\
                        <td colspan="2" class="no-data-message align-center">There is no data.</td>\
                    </tr>';
        _tables.reportContent.empty().append(html);
    };
    var getMatchDetail = function (leagueDetail) {
        var matchDetail = "";
        switch (leagueDetail.Sport.Id) {
            case Enums.Sport.NumberGame:
                matchDetail = numberGamePrefix + " " + leagueDetail.Name;
                break;
            case Enums.Sport.LiveCasino:
                matchDetail = liveCasinoPrefix + " " + leagueDetail.ShowTime;
                break;
            default:
                if (leagueDetail.Home.Name != "") {
                    if (leagueDetail.Away.Name != "") {
                        matchDetail = (
                       '<div class="display-inline-block">' + leagueDetail.Home.Name + '</div>' +
                       '<div class="vs-message display-inline-block">&nbsp' + '-vs-' + '&nbsp</div>' +
                       '<div class="display-inline-block">' + leagueDetail.Away.Name + '</div>');
                    }
                    else {
                        matchDetail = leagueDetail.Home.Name;
                    }
                } else {
                    if (leagueDetail.Away.Name != "") {
                        matchDetail = leagueDetail.Away.Name;
                    }
                    else {
                        matchDetail = "";
                    }
                }
                break;
        }

        return matchDetail;
    }
    showHideLeagueInfo = function (btnExpand, leagueId) {
        var icon = $(btnExpand).find('.colLeagueIcon');

        if (icon.hasClass("collapse")) {
            icon.removeClass("collapse");
            _tables.report.find("tbody tr.lic_" + leagueId).removeClass("display-none");
            icon.addClass("expand");
        }
        else {
            icon.removeClass("expand");
            _tables.report.find("tbody tr.lic_" + leagueId).addClass("display-none");
            icon.addClass("collapse");
        }

        _tables.report.gridScroller(_options.report);
    };
}