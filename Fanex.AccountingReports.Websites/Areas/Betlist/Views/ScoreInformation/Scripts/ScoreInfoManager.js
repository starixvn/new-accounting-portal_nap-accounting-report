ScoreInfoManager = function () {
    var self = this;

    self.Enums = {
        Sports: {
            Soccer: 1,
            Basketball: 2,
            Netball: 32,
            Football: 3,
            Rugby: 26,
            IceHockey: 4,
            FieldHockey: 28,
            Tennis: 5,
            Billard: 7,
            Golf: 10,
            WinterSports: 29,
            Cycling: 33,
            Fencing: 34,
            Judo: 35,
            ModernPentathlon: 36,
            Rowing: 37,
            Sailing: 38,
            Shooting: 39,
            Taekwondo: 40,
            Triathlon: 41,
            Wrestling: 42,
            Baseball: 8,
            Volleyball: 6,
            Badminton: 9,
            Squash: 30,
            HorseRacing: 151,
            NumberGame: 161,
            VirtualSoccer: 180,
            Darts: 25
        },

        NumberOfSet: {
            Basketball: 5,
            HockeySport: 4,
            Tennis: 5,
            Baseball: 9,
            Badminton: 5
        },

        BetType: {
            StartSetX_BetTypeRange: 1311,
            EndSetX_BetTypeRange: 1334
        }
    };

    self.SoccerInfo = new SoccerInfo();
    self.BasketballInfo = new BasketballInfo();
    self.HockeySportInfo = new HockeySportInfo();
    self.TennisInfo = new TennisInfo();
    self.BillardInfo = new BillardInfo();
    self.BaseballInfo = new BaseballInfo();
    self.BadmintonInfo = new BadmintonInfo();
    self.CommonSportInfo = new CommonSportInfo();
    self.NumberGameInfo = new NumberGameInfo();
    self.VirtualSoccerInfo = new VirtualSoccerInfo();

    self.Tables = {
        scoreInformationPopupContainer: null,
        scoreInformationPopup: null
    };

    self.Params = {
        reportParams: {
            MatchId: null,
            Bettype: null,
            ReportDate: null,
            SportType: null,
            SportTypeName: null,
            BetTypeName: null,
            LiveIndicator: null,
            Hedge: null,

            LeagueId: null,
            LeagueName: null,
            MatchCode: null,
            HomeTeam: null,
            HomeTeamName: null,
            AwayTeam: null,
            AwayTeamName: null,
            BetId: null
        }
    };

    self.CacheData = {
        reportViewHtml: null,
        reportWidth: 0,
        reportHeight: 200,
        currentSportReport: null,
        currentDialog: null
    };
};

ScoreInfoManager.prototype.InitPage = function () {
    this.CachedDOMElements();

    this.SoccerInfo.InitPage();
    this.BasketballInfo.InitPage();
    this.HockeySportInfo.InitPage();
    this.TennisInfo.InitPage();
    this.BillardInfo.InitPage();
    this.BaseballInfo.InitPage();
    this.BadmintonInfo.InitPage();
    this.CommonSportInfo.InitPage();
    this.NumberGameInfo.InitPage();
    this.VirtualSoccerInfo.InitPage();

    this.CacheData.reportViewHtml = this.Tables.scoreInformationPopupContainer.html();
};

ScoreInfoManager.prototype.CachedDOMElements = function () {
    var self = this;
    self.Tables.scoreInformationPopupContainer = $("#scoreInformationPopupContainer");
    self.Tables.scoreInformationPopup = $("#scoreInformationPopup");
};

ScoreInfoManager.prototype.GetReport = function (reportDate,
    sportType,
    sportTypeName,
    betType,
    betTypeName,
    liveIndicator,
    hedge,
    leagueId,
    leagueName,
    matchId,
    matchCode,
    homeTeam,
    homeTeamName,
    awayTeam,
    awayTeamName,
    betId,
    customizeFunction) {
    var self = this;

    AccReport.disableInput();
    this.Params.reportParams = {
        MatchId: matchId,
        Bettype: parseInt(betType, 0),
        ReportDate: reportDate,
        SportType: sportType,
        SportTypeName: sportTypeName,
        BetTypeName: betTypeName,
        LiveIndicator: liveIndicator,
        Hedge: hedge,

        LeagueId: leagueId,
        LeagueName: leagueName,
        MatchCode: matchCode,
        HomeTeam: homeTeam,
        HomeTeamName: homeTeamName,
        AwayTeam: awayTeam,
        AwayTeamName: awayTeamName,
        BetId: betId
    };

    //    this.Params.reportParams.SportType = 180;
    //    this.Params.reportParams.MatchId = 4330482;
    //    this.Params.reportParams.LeagueId = 26061;
    //    this.Params.reportParams.Bettype = 1204;

    $.ajax({
        type: "POST",
        url: window.rootUrl + "ScoreInformation/GetScoreInformation",
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        data: JSON.stringify(self.Params.reportParams),
        beforeSend: function () {
        },
        success: function (result) {
            self.RenderReport(result);
        },
        error: function (jqXhr, event, ajaxSettings) {
            if (jqXhr.status && jqXhr.status == 500) { // server errors
                var r = jQuery.parseJSON(jqXhr.responseText);
                AccReport.popUp.show(r.message);
            }
        },
        complete: function () {
            if (customizeFunction) {
                var cFunction = eval(customizeFunction);
                cFunction.call(self);
            }
            AccReport.enableInput();
        }
    });
};

ScoreInfoManager.prototype.RenderReport = function (report) {
    var self = this;
    var sportType = parseInt(self.Params.reportParams.SportType);
    self.ClosePopup();
    switch (sportType) {
        case self.Enums.Sports.Soccer:
            self.CacheData.currentSportReport = self.SoccerInfo;
            break;
        case self.Enums.Sports.Basketball:
        case self.Enums.Sports.Netball:
        case self.Enums.Sports.Football:
        case self.Enums.Sports.Rugby:
            self.CacheData.currentSportReport = self.BasketballInfo;
            break;
        case self.Enums.Sports.IceHockey:
        case self.Enums.Sports.FieldHockey:
            self.CacheData.currentSportReport = self.HockeySportInfo;
            break;
        case self.Enums.Sports.Tennis:
            self.CacheData.currentSportReport = self.TennisInfo;
            break;
        case self.Enums.Sports.Billard:
        case self.Enums.Sports.Golf:
        case self.Enums.Sports.Motorsports:
        case self.Enums.Sports.WinterSports:
        case self.Enums.Sports.Cycling:
        case self.Enums.Sports.Fencing:
        case self.Enums.Sports.Judo:
        case self.Enums.Sports.ModernPentathlon:
        case self.Enums.Sports.Rowing:
        case self.Enums.Sports.Sailing:
        case self.Enums.Sports.Shooting:
        case self.Enums.Sports.Taekwondo:
        case self.Enums.Sports.Triathlon:
        case self.Enums.Sports.Wrestling:
            self.CacheData.currentSportReport = self.BillardInfo;
            break;
        case self.Enums.Sports.Baseball:
            self.CacheData.currentSportReport = self.BaseballInfo;
            break;
        case self.Enums.Sports.Badminton:
        case self.Enums.Sports.Volleyball:
        case self.Enums.Sports.Squash:
            self.CacheData.currentSportReport = self.BadmintonInfo;
            break;
        case self.Enums.Sports.NumberGame:
            self.CacheData.currentSportReport = self.NumberGameInfo;
            break;
        case self.Enums.Sports.VirtualSoccer:
            self.CacheData.currentSportReport = self.VirtualSoccerInfo;
            break;
        default:
            self.CacheData.currentSportReport = self.CommonSportInfo;
    }

    if (self.CacheData.currentSportReport) {
        self.CacheData.currentSportReport.RenderReport(report, self.Params.reportParams);
        self.Tables.scoreInformationPopup.removeClass("display-none");
        self.Tables.scoreInformationPopup.dialog({
            title: "Score Information",
            autoOpen: false,
            height: self.CacheData.currentSportReport.CacheData.reportHeight,
            width: self.CacheData.currentSportReport.CacheData.reportWidth,
            modal: false,
            resizable: true,
            close: function (event, ui) {
                self.Tables.scoreInformationPopup.dialog("destroy").remove();
                self.Tables.scoreInformationPopupContainer.append(self.CacheData.reportViewHtml);
                self.InitPage();
                self.CacheData.currentDialog = null;
            }
        });

        self.CacheData.currentDialog = self.Tables.scoreInformationPopup.dialog("open");
    }
};

ScoreInfoManager.prototype.RenderScoreTitle = function (report) {
    var self = this;
    var headerTitle = "<span class='sportTypeName_Header'>" + self.Params.reportParams.SportTypeName + "</span>" + " - " + self.Params.reportParams.LeagueName;
    headerTitle += "<br />";
    headerTitle += "Bettype: " + (self.Params.reportParams.LiveIndicator ? "<font color='red'>Live!</font> " : "NON-LIVE ") + self.Params.reportParams.BetTypeName;
    headerTitle += "<br />";
    headerTitle += "Trader: " + (report && report.TraderName && report.TraderName != '' ? report.TraderName : "Unassigned");
    return headerTitle;
};

ScoreInfoManager.prototype.RenderScoreTitle_SportLeague = function (report) {
    var self = this;
    var headerTitle = '';
    if (self.CacheData.currentSportReport) {
        headerTitle = "<span class='sportTypeName_Header'>" + self.Params.reportParams.SportTypeName + "</span>" + " - " + self.Params.reportParams.LeagueName;
        self.CacheData.currentSportReport.Tables.report_Header.html(headerTitle);
    }

    return headerTitle;
};

ScoreInfoManager.prototype.ClosePopup = function () {
    var self = this;
    if (self.CacheData.currentDialog) {
        self.Tables.scoreInformationPopup.dialog("close");
        self.CacheData.currentDialog = null;
    }
};

var scoreInfoManager = new ScoreInfoManager();