Namespace.Register("Fanex.AccountingReports.WinLoss.LeagueWinLoss.LeagueWinLossViewVirtualRate");
Fanex.AccountingReports.WinLoss.LeagueWinLoss.LeagueWinLossViewVirtualRate = function () {
    var self = this;
    var _tables = {
        parentContainer: null,
        reportContainer: null,
        report: null
    };

    self.Params = {
        reportParams: {
            SuperName: null,
            CustID: null
        }
    };
    self.initPage = function () {
        cachedDOMElements();
    };
    var cachedDOMElements = function () {
        _tables.parentContainer = $("#parentContainer");
        _tables.reportContainer = $("#leagueWinLossVirtualRateContainer");
        _tables.report = $("#leagueWinLossVirtualRate");
    };
    self.hideReport = function () {
        _tables.reportContainer.addClass("display-none");
        _tables.report.addClass("display-none");
    };
    self.showReport = function () {
        _tables.report.removeClass("display-none");
        _tables.reportContainer.removeClass("display-none");
    };
    self.getReport = function (params) {
        AccReport.disableInput();
        getParams(params);
        $.ajax({
            type: "POST",
            url: window.rootUrl + "WinLoss/LeagueWinLoss/GetVirtualRate",
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            data: JSON.stringify(self.Params.reportParams),
            beforeSend: function () {
                AccReport.disableInput();
            },
            success: function (rs) {
                self.hideReport();
                renderReport(rs);
            },
            error: function (event, jqXHR, ajaxSettings) {
                self.hideReport();
                renderReport([]);
            },
            complete: function () {
                AccReport.enableInput();
            }
        });
    };
    var getParams = function (params) {
        self.Params.reportParams = params;
    };

    var renderReport = function (report) {
        _tables.reportContainer.removeClass("display-none");
        _tables.report.addClass("display-none");
        var bodyData = [];
        var footerData = [];

        var len = report.length;

        if (len > 0) {
            var currentRow = null;
            var info = null;
            for (var i = 0; i < len; i++) {
                info = report[i];
                currentRow =
                {
                    Columns: [
                            { Name: "ColSport", Value: info.Sport },
                            { Name: "ColEvent", Value: info.LiveStatus },
                            { Name: "ColBetType", Value: info.BetType },
                            { Name: "ColVirtualRate", Value: info.VirtualRate }],
                    SubRows: []
                };

                bodyData.push(currentRow);
            }
        }

        var gridOptions = {
            columns: [{ HeaderText: "Sport", Width: 200, Name: "ColSport", HeaderAlign: "center", CellAlign: "left" },
                    { HeaderText: "Event", Width: 120, Name: "ColEvent", HeaderAlign: "center", CellAlign: "left" },
                    { HeaderText: "Bet Type", Width: 120, Name: "ColBetType", HeaderAlign: "center", CellAlign: "left" },
                    { HeaderText: "VirtualRate", Width: 100, Name: "ColVirtualRate", HeaderAlign: "center", CellAlign: "right", FormatNumber: FormatConstant.TWO_DECIMAL_PERCENT_NUMBER }],
            showTitle: false,
            showFooter: true,
            showTree: false,
            bodyRows: bodyData,
            showDistinctRow: false,
            footerRows: footerData,
            gridExpandHeight: 300
        };

        _tables.report.removeClass("display-none").fanexGrid(gridOptions);
        _tables.reportContainer.dialog({
            title: "VIRTUAL RATE DETAILS (" + self.Params.reportParams.SuperName + ")",
            autoOpen: false,
            height: 550,
            width: 600,
            modal: true,
            resizable: false,
            close: function (event, ui) {
                _tables.reportContainer.dialog("destroy").remove();
                _tables.parentContainer.append(
                    '<div id="leagueWinLossVirtualRateContainer" class="display-none content-grid">\
                        <div id="leagueWinLossVirtualRate" class="freegrid"></div>\
                    </div>');
                _tables.reportContainer = $("#leagueWinLossVirtualRateContainer");
                _tables.report = $("#leagueWinLossVirtualRate");
            }
        });

        _tables.reportContainer.dialog("open");
    };

    self.exportReport = function () {
        AccReport.downloadFile(window.rootUrl + "WinLoss/LeagueWinLoss/ExportLeagueWinLossBySport", self.Params.reportParams);
    };
}