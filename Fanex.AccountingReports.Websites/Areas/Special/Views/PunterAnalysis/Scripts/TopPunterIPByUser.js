TopPunterIPByUser = function () {
    var self = this;

    self.Tables = {
        reportContainer: null,
        report: null
    };

    self.Params = {
        reportParams: {
            CustID: null,
            UserName: null,
            Fromdate: null,
            Todate: null,
            Reporttype: null
        }
    };
};
TopPunterIPByUser.prototype.InitPage = function () {
    this.CachedDOMElements();
};
TopPunterIPByUser.prototype.CachedDOMElements = function () {
    this.Tables.reportContainer = $('#topPunterIP_ByUser_Container');
    this.Tables.report = $('#topPunterIP_ByUser');
};
TopPunterIPByUser.prototype.GetReport = function (params) {
    var self = this;

    AccReport.disableInput();
    self.Params.reportParams = params;
    $.ajax({
        type: "POST",
        url: window.rootUrl + "Special/PunterAnalysis/GetTopPunterIP_ByUser",
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        data: JSON.stringify(self.Params.reportParams),
        beforeSend: function () {
        },
        success: function (rs) {
            self.RenderReport(rs);
        },
        error: function (event, jqXHR, ajaxSettings) {
            self.RenderReport(null);
        },
        complete: function () {
            AccReport.enableInput();
        }
    });
};

TopPunterIPByUser.prototype.RenderReport = function (report) {
    var self = this;

    this.Tables.reportContainer.removeClass("display-none");
    this.Tables.report.addClass("display-none");
    var bodyData = [];
    if (report && report.ChildsInfo.length > 0) {
        var len = report.ChildsInfo.length;
        var currentRow = null;
        var info = null;
        for (var i = 0; i < len; i++) {
            info = report.ChildsInfo[i];
            bodyData.push({
                Columns: [{ Name: "ColMonth", Value: info.Month },
                          { Name: "ColYear", Value: info.Year },
                          { Name: "ColIP", Value: info.IP },
                          { Name: "ColCountry", Value: info.Country}]
            });
        }
    }

    var clickEvent = 'onclick="reportManager.ShowReport()"';
    var title = '<span class="link"' + clickEvent + '>Main</span>' +
                ' > <span>Punter ' + self.Params.reportParams.UserName +
                ' by IP from ' + self.Params.reportParams.Fromdate +
                ' to ' + self.Params.reportParams.Todate + '</span>';

    var gridOptions = {
        customizeHeader: false,
        columns: [{ HeaderText: "Month", Width: 140, Name: "ColMonth", HeaderAlign: "center", CellAlign: "center" },
                  { HeaderText: "Year", Width: 100, Name: "ColYear", HeaderAlign: "center", CellAlign: "center" },
                  { HeaderText: "IP", Width: 150, Name: "ColIP", HeaderAlign: "center", CellAlign: "center" },
                  { HeaderText: "Country", Width: 100, Name: "ColCountry", HeaderAlign: "center", CellAlign: "center"}],
        showTitle: true,
        showFooter: false,
        titleInfo:
        {
            text: title,
            showExcelButton: true,
            exportExcelFunction: function () {
                self.ExportReport();
            }
        },
        showTree: false,
        bodyRows: bodyData,
        footerRows: [],
        gridExpandHeight: 250,
        numberOfFrozenColumn: 1,
        gridWidth: 900,
        showDistinctRow: false,
        showHighlightRow: false
    };

    reportManager.HideReport();
    this.Tables.report.removeClass("display-none").fanexGrid(gridOptions);
};
TopPunterIPByUser.prototype.HideReport = function () {
    this.Tables.reportContainer.addClass("display-none");
    this.Tables.report.addClass("display-none");
};
TopPunterIPByUser.prototype.ExportReport = function () {
    AccReport.downloadFile(window.rootUrl + "Special/PunterAnalysis/ExportTopPunterIP_ByUser", this.Params.reportParams);
};
