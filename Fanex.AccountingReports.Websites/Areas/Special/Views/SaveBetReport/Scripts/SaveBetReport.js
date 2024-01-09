Namespace.Register("Fanex.AccountingReports.Special.SaveBetReport.SaveBetReport");
Fanex.AccountingReports.Special.SaveBetReport.SaveBetReport = function () {
    var self = this;

    var _tables = {
        datepickerplugin: null,
        btnGenerate: null,
        reportContainer: null,
        report: null,
        ddlSports: null,
        ddlLeagues: null,
        sportContainer: null,
        leagueContainer: null,
        generateBtnContainer: null,
        noneSportAndLeagueMsg: null
    };

    var _params = {
        sportParams: {
            Date: null
        },

        leagueParams: {
            Date: null,
            SportType: null
        },

        saveBetReportParams: {
            Date: null,
            LeagueId: null,
            SportType: null
        },

        downloadReportParams: {
            ReportName: null,
            EventDate: null
        },

        downloadReportParamsForDocker: {
            FileName: null,
            SubFolder: null,
            ContentType: "application/x-zip-compressed",
            FunctionFolder: "save-bet",
            Folders: null,
        }
    };

    var _cachedData = {
        firstSportType: null,
        getAllMatchByLeague: -1
    };

    self.cachedDomElements = function () {
        _tables.btnGenerate = $("#btnGenerate");
        _tables.ddlSports = $("#ddlSport");
        _tables.ddlLeagues = $("#ddlLeague");
        _tables.reportContainer = $("#saveBetReportContainer");
        _tables.report = $("#saveBetReport");
        _tables.sportContainer = $("#sportContainer");
        _tables.leagueContainer = $("#leagueContainer");
        _tables.generateBtnContainer = $("#generateBtnContainer");
        _tables.noneSportAndLeagueMsg = $("#noneSportAndLeagueMsg");
    };

    self.initPage = function () {
        self.cachedDomElements();
        self.renderDatePicker();
        self.datePickerChange();
        self.bindingEvents();
    };

    self.bindingEvents = function () {
        _tables.ddlSports.change(function () {
            self.getLeagues();
        });

        _tables.btnGenerate.click(function () {
            self.getSaveBetReport();
        });
    };

    self.renderDatePicker = function () {
        $('#datepickerplugin').nexDatePicker({
            nexDatePickerType: "single",
            firstDatePicker: {
                Id: "CheckDate",
                Label: labelDate,
                InitDate: today,
                LabelCss: "lbl-field col_2",
                TextboxCss: "edit-field col_6"
            },

            datePickerFormat: {
                dateFormat: FormatConstant.M_DD_YY_STRING,
                changeMonth: true,
                changeYear: true,
                minDate: new Date(minDate),
                maxDate: new Date(today)
            },

            onSelectDatePicker: function () {
                self.datePickerChange();
            }
        });

        _tables.datepickerplugin = $("#datepickerplugin");
    };

    self.renderSports = function (rs) {
        _tables.ddlSports.empty();
        _tables.ddlLeagues.empty();
        var rsLength = rs.Sports.length;
        for (var i = 0; i < rsLength; i++) {
            var sport = rs.Sports[i];
            _tables.ddlSports.append('<option value="' + sport.SportType + '">' + sport.SportName + '</option>');
        }
    };

    self.getLeagueParams = function () {
        _params.leagueParams.Date = _tables.datepickerplugin.nexDatePicker("getSingleDate");
        _params.leagueParams.SportType = _tables.ddlSports.val();

        return _params.leagueParams;
    };

    self.getLeagues = function () {
        AccReport.disableInput();

        var data = self.getLeagueParams();

        AccReport.disableInput();
        AccReport.utility.CallAjax(this,
            window.rootUrl + "Special/SaveBetReport/GetLeaguesBySport",
            data,
            self.renderLeagues,
            self.renderLeagues,
            AccReport.enableInput);
    };

    self.renderLeagues = function (rs) {
        _tables.ddlLeagues.empty();
        var rsLength = rs.Leagues.length;
        _tables.ddlLeagues.append('<option value="All">All</option>');
        for (var i = 0; i < rsLength; i++) {
            var league = rs.Leagues[i];
            _tables.ddlLeagues.append('<option value="' + league.LeagueId + '">' + league.LeagueName + '</option>');
        }
    };

    downloadSaveBetReport = function (reportName, eventDate) {
        var reportDate = eventDate.split('_');
        _params.downloadReportParamsForDocker.FileName = reportName + ".zip";
        _params.downloadReportParamsForDocker.SubFolder = reportDate[2] + '' + reportDate[0] + '' + reportDate[1];
        _params.downloadReportParamsForDocker.Folders = dockerFolders;
        $.ajax({
            type: "POST",
            url: dockerFileApi + "/download",
            contentType: "application/json; charset=utf-8",
            data: JSON.stringify(_params.downloadReportParamsForDocker),
            dataType: 'json',
            xhrFields: {
                withCredentials: true
            },
            success: function (response) {
                if (response.success == false) {
                    AccReport.popUp.show("Sorry, File not found in docker. Access Denied !!!");
                }
                else {
                    var bytes = self.base64ToBytes(response.data.fileContents);
                    var blob = new Blob([bytes], { type: response.data.contentType });
                    var isIE = false || !!document.documentMode;
                    if (isIE) {
                        window.navigator.msSaveBlob(blob, response.data.fileDownloadName);
                    } else {
                        var url = window.URL || window.webkitURL;
                        link = url.createObjectURL(blob);
                        var a = $("<a />");
                        a.attr("download", response.data.fileDownloadName);
                        a.attr("href", link);
                        a.attr("id", 'file_' + response.data.fileDownloadName);
                        $("body").append(a);
                        a[0].click();
                        $(a).remove();
                    }
                }

            },
            error: function () {
                AccReport.popUp.show("Sorry, File not found. Access Denied !!!");
            }
        });
    };

    self.base64ToBytes = function (base64) {
        var s = window.atob(base64);
        var bytes = new Uint8Array(s.length);
        for (var i = 0; i < s.length; i++) {
            bytes[i] = s.charCodeAt(i);
        }
        return bytes;
    };

    self.getSaveBetReportParams = function () {
        _params.saveBetReportParams.Date = _tables.datepickerplugin.nexDatePicker("getSingleDate");
        _params.saveBetReportParams.SportType = _tables.ddlSports.val();
        _params.saveBetReportParams.LeagueId = _tables.ddlLeagues.val();

        if (_params.saveBetReportParams.LeagueId == "All") {
            _params.saveBetReportParams.LeagueId = _cachedData.getAllMatchByLeague;
        }

        return _params.saveBetReportParams;
    };

    self.getSaveBetReport = function () {
        AccReport.disableInput();

        var data = self.getSaveBetReportParams();

        AccReport.disableInput();
        AccReport.utility.CallAjax(this,
            window.rootUrl + "Special/SaveBetReport/GetSaveBetReport",
            data,
            self.renderSaveBetReport,
            self.renderSaveBetReport,
            AccReport.enableInput);
    };

    self.renderSaveBetReport = function (rs) {
        _tables.reportContainer.removeClass("display-none");

        var groupHeaderRows = [
            {
                Columns: [
                    { HeaderText: "MatchID", Name: "ColMatchId", MergeRowNumber: 2 },
                    { HeaderText: "Match Code", Name: "ColMatchCode", MergeRowNumber: 2 },
                    { HeaderText: "Event", Name: "ColEvent", MergeColNumber: 3, MergeRowNumber: 2 },
                    { HeaderText: "Download", Name: "ColDownload", MergeRowNumber: 2 }
                ]
            }
        ];

        var columns = [
            { HeaderText: "MatchID", Width: 120, Name: "ColMatchId", HeaderAlign: "center", CellAlign: "center" },
            { HeaderText: "Match Code", Width: 120, Name: "ColMatchCode", HeaderAlign: "center", CellAlign: "center" },
            { HeaderText: "HomeTeam", Width: 250, Name: "ColEvent", HeaderAlign: "center", CellAlign: "right", MergeHeaderColNumber: 3 },
            { HeaderText: "Vs", Width: 50, Name: "ColVs", HeaderAlign: "center", CellAlign: "center" },
            { HeaderText: "AwayTeam", Width: 250, Name: "ColAwayTeam", HeaderAlign: "center", CellAlign: "left" },
            {
                HeaderText: "Download", Width: 120, Name: "ColDownload", HeaderAlign: "center", CellAlign: "center", CustomCellRender: function (cellValue) {
                    var eventDate = cellValue.Value.EventDate;
                    var reportName = cellValue.Value.MatchId + "_" + cellValue.Value.EventDate + "_" + cellValue.Value.SportType;
                    return '<a class="downloadBtn" onclick="downloadSaveBetReport(\'' + reportName + '\', \'' + eventDate + '\')"></a>';
                }
            }
        ];

        var bodyData = [];

        var reportLength = rs.Matchs.length;
        for (var i = 0; i < reportLength; i++) {
            var match = rs.Matchs[i];
            bodyData.push({
                Columns: [{ Name: "ColMatchId", Value: match.MatchId },
                { Name: "ColMatchCode", Value: match.MatchCode },
                { Name: "ColEvent", Value: match.HomeTeam },
                { Name: "ColVs", Value: "Vs" },
                { Name: "ColAwayTeam", Value: match.AwayTeam },
                { Name: "ColDownload", Value: match }
                ]
            });
        }

        var gridOptions = {
            customizeHeader: true,
            groupHeaderRows: groupHeaderRows,
            columns: columns,
            showTitle: true,
            titleInfo:
            {
                text: "Save Bet Report",
                showExcelButton: false
            },
            showFooter: false,
            bodyRows: bodyData,
            gridExpandHeight: 170,
            showPager: true,
            pagerOption: {
                itemsPerPage: 50,
                currentPage: 0,
                itemsPerPageArray: [50, 100, 200, 500],
                totalItem: reportLength
            }
        };


        _tables.report.removeClass("display-none").fanexGrid(gridOptions);
    };

    self.datePickerChange = function () {
        var date = _tables.datepickerplugin.nexDatePicker("getSingleDate");
        var noneDataMsg = noneSportAndLeagueMsg + " " + date + ".";

        $.ajax({
            type: "POST",
            url: window.rootUrl + "Special/SaveBetReport/GetSportsByDate",
            data: { Date: date },
            async: false,
            dataType: "json",
            success: function (rs) {
                if (rs.Sports.length > 0) {
                    _tables.sportContainer.removeClass("display-none");
                    _tables.noneSportAndLeagueMsg.addClass("display-none");
                    self.renderSports(rs);
                    _cachedData.firstSportType = rs.Sports[0].SportType;
                } else {
                    _tables.sportContainer.addClass("display-none");
                    _tables.reportContainer.addClass("display-none");
                    _tables.report.addClass("display-none");
                    _tables.noneSportAndLeagueMsg.removeClass("display-none").html(noneDataMsg);
                }
            }
        });

        $.ajax({
            type: "POST",
            url: window.rootUrl + "Special/SaveBetReport/GetLeaguesBySport",
            data: { Date: date, SportType: _cachedData.firstSportType },
            async: false,
            dataType: "json",
            success: function (rs) {
                if (rs.Leagues.length > 0) {
                    _tables.leagueContainer.removeClass("display-none");
                    _tables.generateBtnContainer.removeClass("display-none");
                    self.renderLeagues(rs);
                } else {
                    _tables.leagueContainer.addClass("display-none");
                    _tables.generateBtnContainer.addClass("display-none");
                }
            }
        });
    };
};

$(document).ready(function () {
    var saveBetReport = new Fanex.AccountingReports.Special.SaveBetReport.SaveBetReport();
    saveBetReport.initPage();
});