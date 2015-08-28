
Namespace.Register("Fanex.AccountingReports.Special.SpecialMatchAnalysis.MatchAnalysisNormal");
Fanex.AccountingReports.Special.SpecialMatchAnalysis.MatchAnalysisNormal = function () {
    var self = this;

    self.Params = {
        betTypeSelection: {
            EventDate: null,
            MatchId: null
        },

        reportParams: {
            EventDate: null,
            MatchId: null,
            StatusId: null,
            StatusName: '',
            Liveindicator: null,
            LiveindicatorName: '',
            ReportType: null,
            ReportTypeName: '',
            Bettypes: '',
            IsMemberReport: false,
            League: '1',
            Home: '2',
            Away: '3'
        }
    };

    var _tables = {
        btnGenerate: null,
        ddlReportBy: null,
        ddlBetStatus: null,
        ddlLiveStatus: null,
        ddlBettype: null,
        matchInfoGrid: null,
        matchInfoGridContent: null,

        report: null,
        reportHeader: null,
        reportContent: null,
        reportTitle: null,
        reportExportExcel: null,
        reportRowHeader: null,
        reportRowFooter: null,
        reportColFooter: null,
        colReportType: null,
        colCurrency: null
    };

    var _options = {
        report: {
            isShowPager: true,
            tblContentId: 'matchAnalysisReportContent',
            fullHeight: true,
            gridHeight: 232,
            freezeHeight: 280,
            column: [160, 120, 120, 100, 120, 120, 120, 120, 80],
            numberFreezeCol: 1
        },

        memberColumn: [160, 120, 120, 100, 120, 120, 120, 120, 80],
        normalColumn: [160, 120, 120, 100, 120, 120, 120, 120]
    };

    // TempHtml: use for save frozen, scroll html of report 
    // when render by multi function
    self.TempHtml = {
        frozenHtml: '',
        contentHtml: '',
        footerFrozenHtml: '',
        footerContentHtml: ''
    };

    self.initPage = function () {
        initGridScrollerTables();
        cachedDOMElements();
        bindingEvent();
    };
    var initGridScrollerTables = function () {
        $("#matchAnalysisReport").initGrid({
            isShowPager: false
        });
    };
    var cachedDOMElements = function () {
        _tables.btnGenerate = $("#btnGenerateMatchAnalysis");
        _tables.btnBack = $("#btnBackMatchAnalysis");

        _tables.ddlReportBy = $("#ddlReportBy");
        _tables.ddlBetStatus = $("#ddlBetStatus");
        _tables.ddlLiveStatus = $("#ddlLiveStatus");

        _tables.report = $("#matchAnalysisReport");
        _tables.reportHeader = $("#matchAnalysisReportHeader");
        _tables.reportContent = _tables.report.find("#matchAnalysisReportContent tbody");
        _tables.reportTitle = _tables.reportHeader.find("#matchAnalysisReportTitle");
        _tables.reportExportExcel = _tables.reportHeader.find("#matchAnalysisReport_ExportExcel");
        _tables.reportRowHeader = _tables.report.find("#matchAnalysisReportRowHeader tbody");
        _tables.reportRowFooter = _tables.report.find(".row_footer_content thead");
        _tables.reportColFooter = _tables.report.find(".col_footer_content thead");
        _tables.colCurrency = _tables.report.find(".colCurrency");
        _tables.colReportType = _tables.report.find(".ttlreportType");
    };
    var bindingEvent = function () {
        _tables.ddlReportBy.change(function () {
            renderDropdownBetStatus(_tables.ddlReportBy.val());
        });

        _tables.btnGenerate.click(function () {
            self.btnGenerateClick();
        });

        _tables.btnBack.click(function () {
            ViewMatchSelection();
        });

        _tables.reportExportExcel.click(function () {
            exportReport();
        });
    };
    self.ViewMatchAnalysis = function (eventDate, leagueInfo, matchInfo) {
        self.Params.reportParams.EventDate = eventDate;
        self.Params.reportParams.MatchId = matchInfo.MatchId;

        self.Params.betTypeSelection.EventDate = eventDate;
        self.Params.betTypeSelection.MatchId = matchInfo.MatchId;

        renderDropdownBetStatus(Enums.ReportType.Currency);
        getBetTypeSelection(eventDate, matchInfo);
        renderMatchInfo(eventDate, leagueInfo, matchInfo);

        _tables.ddlReportBy.val('0');
        _tables.ddlBetStatus.val('0');
        _tables.ddlLiveStatus.val('-1');
        _tables.ddlLiveStatus.val('-1');
    };
    var getBetTypeSelection = function (eventDate, matchInfo) {
        AccReport.disableInput();
        $.ajax({
            type: "POST",
            url: window.rootUrl + "Special/SpecialMatchAnalysis/GetBettype",
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            data: JSON.stringify(self.Params.betTypeSelection),
            beforeSend: function () {
                AccReport.disableInput();
            },
            success: function (rs) {
                renderDropdownBetType(rs);
            },
            error: function (event, jqXHR, ajaxSettings) {
                renderDropdownBetType([]);
            },
            complete: function () {
                AccReport.enableInput();
            }
        });
    };
    var renderDropdownBetStatus = function (reportType) {
        var betStatus = [{ Id: Enums.BetStatus.All, Name: 'All' },
                         { Id: Enums.BetStatus.Won, Name: 'Won' },
                         { Id: Enums.BetStatus.Lose, Name: 'Lose' },
                         { Id: Enums.BetStatus.Draw, Name: 'Draw'}];

        var options = '';
        var betStatusCount = betStatus.length;
        for (var i = 0; i < betStatusCount; i++) {
            var status = betStatus[i];
            if (!(reportType == Enums.ReportType.Member &&
                 (status.Id == Enums.BetStatus.All || status.Id == Enums.BetStatus.Draw))) {
                options += "<option value='" + status.Id + "'>" + status.Name + "</option>";
            }
        }

        _tables.ddlBetStatus.empty().append(options);
    };
    var renderDropdownBetType = function (betTypes) {
        var noItemText = 'No product found.';
        var len = betTypes.length;
        var ddwnBetTypes = [];
        var prefixId = 'ddlBettype_';
        for (var i = 0; i < len; i++) {
            var betType = betTypes[i];
            ddwnBetTypes.push({
                Id: prefixId + betType.Id,
                Value: betType.Id,
                Text: betType.Name,
                Checked: true,
                Enable: true,
                CssClass: "",
                Items: []
            });
        }

        $("#ddlBettype").multiSelect(
        {
            minWidth: 200,
            maxHeight: 450,
            shadow: true,
            showHeader: true,
            isCheckAll: true,
            showFooter: false,
            showTooltip: true,
            checkAllText: 'All Bet types',
            isUsedCustomCheckAllText: true,
            customCheckAllText: 'All Bet types',
            noItemText: noItemText,
            columnNumber: 2,
            listData: ddwnBetTypes
        });

        _tables.ddlBettype = $("#ddlBettype");
    };
    var renderMatchInfo = function (eventDate, leagueInfo, matchInfo) {
        var html = "<tr>\
                        <td class='col-event-date align-center'>" + moment(eventDate).format(FormatConstant.MM_DD_YYYY_WITH_SLASH_STRING_MOMENT) + "</td>\
                        <td class='col-league align-center'>" + leagueInfo.Name + "</td>\
                        <td class='col-event align-center'>" + getMatchDetail(matchInfo) + "</td>\
                    </tr>";

        _tables.matchInfoGrid = $("#matchInfoGrid");
        _tables.matchInfoGridContent = _tables.matchInfoGrid.find("tbody");
        _tables.matchInfoGridContent.empty().append(html);
        _tables.matchInfoGrid.removeClass("display-none");

        self.Params.reportParams.League = leagueInfo.Name;
        self.Params.reportParams.Home = matchInfo.Home.Name;
        self.Params.reportParams.Away = matchInfo.Away.Name;
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
    self.btnGenerateClick = function () {
        self.getReport();
    };
    self.getReport = function () {
        AccReport.disableInput();
        getParams();
        if (validateParams() == true) {
            $.ajax({
                type: "POST",
                url: window.rootUrl + "Special/SpecialMatchAnalysis/GetMatchAnalysis",
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                data: JSON.stringify(self.Params.reportParams),
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
        self.Params.reportParams.ReportType = parseInt(_tables.ddlReportBy.val());
        self.Params.reportParams.ReportTypeName = _tables.ddlReportBy.find("option:selected").text();
        self.Params.reportParams.StatusId = parseInt(_tables.ddlBetStatus.val());
        self.Params.reportParams.StatusName = _tables.ddlBetStatus.find("option:selected").text();
        self.Params.reportParams.Liveindicator = parseInt(_tables.ddlLiveStatus.val());
        if (self.Params.reportParams.Liveindicator == -1) {
            self.Params.reportParams.Liveindicator = null;
        }

        self.Params.reportParams.LiveindicatorName = _tables.ddlLiveStatus.find("option:selected").text();
        self.Params.reportParams.Bettypes = _tables.ddlBettype.multiSelect('getCheckedItems').join(",");
    };
    var validateParams = function () {
        var isValid = true;
        var message = '';
        var notifyIds = [];
        if (self.Params.reportParams.Bettypes.length == 0) {
            isValid = false;
            if (message == '') {
                message = "Please select at least one bet type!";
            }
            notifyIds.push("ddlBettype");
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
        self.Params.reportParams.IsMemberReport =
            self.Params.reportParams.ReportType == Enums.ReportType.Member ||
            self.Params.reportParams.ReportType == Enums.ReportType.Super;
        if (self.Params.reportParams.IsMemberReport) {
            _options.report.column = _options.memberColumn;
            _tables.colCurrency.removeClass('display-none');
        }
        else {
            _options.report.column = _options.normalColumn;
            _tables.colCurrency.addClass('display-none');
        }

        var reportTitle = "Special Match Analysis Details";
        _tables.reportTitle.html(reportTitle);
        setTitle();

        if (report &&
            report.Rows &&
            report.Rows.length > 0) {
            _options.report.numberFreezeCol = 1;
            var len = report.Rows.length;
            self.resetTempHtml();
            _tables.reportExportExcel.removeClass("display-none");

            for (var i = 0; i < len; i++) {
                var reportItem = report.Rows[i];
                renderReportRow(reportItem, i % 2 ? 'even' : '');
            }

            renderReportTotalRow(report.Total);

            self.removeSpaceIE9TempHtml();
            _tables.reportRowHeader.empty().append(self.TempHtml.frozenHtml);
            _tables.reportContent.empty().append(self.TempHtml.contentHtml);
            _tables.reportRowFooter.empty().append(self.TempHtml.footerFrozenHtml);
            _tables.reportColFooter.empty().append(self.TempHtml.footerContentHtml);
        } else {
            renderReportNoData();
        }

        hideDrillDownView();
        _tables.report.removeClass("display-none")
                      .gridScroller(_options.report);
        _tables.reportHeader.removeClass("display-none");
        _tables.report.removeClass("display-none");
    }
    var renderReportRow = function (row, cssClass) {
        var frozenHtml = '';
        var downlineLink = '';
        if (self.Params.reportParams.ReportType == Enums.ReportType.Super) {
            downlineLink = '<div class="link" onclick="ViewMatchAnalysisByMaster(' + row.Id + ',\'' + row.Name + '\');">' + row.Name + '</a>';
        } else {
            downlineLink = row.Name;
        }

        frozenHtml = '<td><div class="col160 align-center">' + downlineLink + '</div></td>'
        self.TempHtml.frozenHtml += '<tr class="' + cssClass + '">' + frozenHtml + ' </tr>';
        self.TempHtml.contentHtml += '<tr class="' + cssClass + '">' +
                                        frozenHtml +
                                        '<td><div class="col120 align-right">' + row.MemberTO + '</div></td>\
                                        <td><div class="col120 align-right">' + row.MemberWL + '</div></td>\
                                        <td><div class="col100 align-right">' + row.BetCount + '</div></td>\
                                        <td><div class="col120 align-right">' + row.HouseTO + '</div></td>\
                                        <td><div class="col120 align-right' +
                                                self.formatCellColor(row.HouseWL, false, true) + '">' +
                                                row.HouseWL + '</div></td>\
                                        <td><div class="col120 align-right">' + row.SMATO + '</div></td>\
                                        <td><div class="col120 align-right">' + row.SMAWL + '</div></td>' +
                                        (self.Params.reportParams.IsMemberReport ? '<td><div class="col80 align-center">' + row.Currency + '</div></td>' : '') +
                                    '</tr>';
    };
    var renderReportTotalRow = function (row) {
        var frozenHtml = '';
        frozenHtml = '<th><div class="col160 align-center">' + row.Name + '</div></th>'
        self.TempHtml.footerFrozenHtml += '<tr>' + frozenHtml + ' </tr>';
        self.TempHtml.footerContentHtml += '<tr>' +
                                            frozenHtml +
                                            '<th><div class="col120 align-right">' + row.MemberTO + '</div></th>\
                                            <th><div class="col120 align-right">' + row.MemberWL + '</div></th>\
                                            <th><div class="col100 align-right">' + row.BetCount + '</div></th>\
                                            <th><div class="col120 align-right">' + row.HouseTO + '</div></th>\
                                            <th><div class="col120 align-right' +
                                                self.formatCellColor(row.HouseWL, false, true) + '">' +
                                                row.HouseWL + '</div></th>\
                                            <th><div class="col120 align-right">' + row.SMATO + '</div></th>\
                                            <th><div class="col120 align-right">' + row.SMAWL + '</div></th>' +
                                           (self.Params.reportParams.IsMemberReport ? '<th><div class="col80 align-center">' + row.Currency + '</div></th>' : '') +
                                        '</tr>';
    };
    var renderReportNoData = function () {
        _options.report.numberFreezeCol = 0;
        _tables.reportExportExcel.addClass("display-none");
        var html = '<tr>\
                    <td colspan="' + _options.report.column.length + '" class="no-data-message align-center">There is no data.</td>\
                </tr>';
        _tables.reportContent.empty().append(html);
        _tables.reportRowFooter.empty();
        _tables.reportColFooter.empty();
    };
    self.formatCellColor = function (cellValue, isPositive, isNegative) {
        var cssClass = '';
        cellValue = cellValue.replace(/\,/g, '');

        if (isNegative && parseFloat(cellValue) < 0) {
            cssClass = ' color-red';
        }

        return cssClass;
    };
    self.resetTempHtml = function () {
        self.TempHtml.frozenHtml = '';
        self.TempHtml.contentHtml = '';
        self.TempHtml.footerFrozenHtml = '';
        self.TempHtml.footerContentHtml = '';
    };
    self.removeSpaceIE9TempHtml = function () {
        self.TempHtml.frozenHtml = fixBugsBrowser.removeSpaceIE9(self.TempHtml.frozenHtml);
        self.TempHtml.contentHtml = fixBugsBrowser.removeSpaceIE9(self.TempHtml.contentHtml);
        self.TempHtml.footerFrozenHtml = fixBugsBrowser.removeSpaceIE9(self.TempHtml.footerFrozenHtml);
        self.TempHtml.footerContentHtml = fixBugsBrowser.removeSpaceIE9(self.TempHtml.footerContentHtml);
    };
    var setTitle = function () {
        var title = '';
        switch (self.Params.reportParams.ReportType) {
            case 0:
                title = 'Currency';
                break;
            case 1:
                title = 'Bet Type';
                break;
            case 2:
                title = 'Super';
                break;
            case 3:
                switch (self.Params.reportParams.StatusId) {
                    case 112:
                        title = 'Top 100 (WON) Member';
                        break;
                    case 113:
                        title = 'Top 100 (LOSE) Member';
                        break;
                    default:
                        title = 'Report';
                        break;
                }
                break;
            default:
                title = 'Report';
                break;
        }

        _tables.colReportType.html(title);
    };
    self.hideView = function () {
        _tables.matchInfoGrid.addClass("display-none");
        self.hideReport();
    };
    self.hideReport = function () {
        _tables.reportHeader.addClass("display-none");
        _tables.report.addClass("display-none");
    };
    self.showReport = function () {
        _tables.reportHeader.removeClass("display-none");
        _tables.report.removeClass("display-none");
    };
    var exportReport = function () {
        AccReport.downloadFile(window.rootUrl + "Special/SpecialMatchAnalysis/ExportReport", self.Params.reportParams);
    };
}