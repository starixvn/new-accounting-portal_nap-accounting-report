
Namespace.Register("Fanex.AccountingReports.WinLoss.TreeWinLoss.VirtualRateHistory");
Fanex.AccountingReports.WinLoss.TreeWinLoss.VirtualRateHistory = function () {
    var self = this;
    var _tables = {
        reportParentContainer: null,
        reportContainer: null,
        report: null
    };

    self.Params = {
        reportParams: {
            custId: null,
            Username: null
        }
    };

    self.initPage = function () {
        cachedDOMElements();
    };

    var cachedDOMElements = function () {
        _tables.reportParentContainer = $("#parentContainer");
        _tables.reportContainer = $("#virtualRateHistoryContainer");
        _tables.report = $("#virtualRateHistory");
    };

    self.hideReport = function () {
        _tables.reportContainer.addClass("display-none");
        _tables.report.addClass("display-none");
    };

    self.showReport = function () {
        _tables.reportContainer.removeClass("display-none");
        _tables.report.removeClass("display-none");
    };

    self.getVirtualRateHistory = function (params) {
        AccReport.disableInput();
        getParams(params);
        $.ajax({
            type: "POST",
            url: window.rootUrl + "WinLoss/TreeWinLoss/GetVirtualRate",
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            data: JSON.stringify(self.Params.reportParams),
            beforeSend: function () {
                AccReport.disableInput();
            },
            success: function (rs) {
                self.hideReport();
                renderVirtualRateHistory(rs);
            },
            error: function (event, jqXHR, ajaxSettings) {
                self.hideReport();
                if (jqXHR.status &&
                    jqXHR.status == AccReportData.Constants.HtmlErrorCode.SystemError) {
                    var r = jQuery.parseJSON(jqXHR.responseText);
                    AccReport.popUp.show(r.message);
                    renderVirtualRateHistory(null);
                }
            },
            complete: function () {
                AccReport.enableInput();
            }
        });

    };

    var getParams = function (params) {
        self.Params.reportParams = params;
    };

    var renderVirtualRateHistory = function (report) {
        _tables.reportContainer.removeClass("display-none");
        _tables.report.addClass("display-none");
        var bodyData = [];
        var totalItems = report.ReportRows.length;
        if (report && report.ReportRows && totalItems > 0) {
            for (var i = 0; i < totalItems; i++) {
                var currentRow = [];
                var info = report.ReportRows[i];
                var isVirtualRateInfo = typeof info.ChangedBy === 'undefined' || info.ChangedBy === null;
                var numberRows = info.ReportRows.length;
                var numberMergeRows = info.NumberMergeRows;
                var virtualRateBySport = null;
                if (numberMergeRows === 0) {
                    currentRow = [];
                    if (numberRows > 0) {
                        if (isVirtualRateInfo) {
                            currentRow.push(
                                { Name: "ColNo", Value: i + 1 },
                                { Name: "ColDate", Value: info.Date },
                                { Name: "ColSport", Value: info.ReportRows[0].Sport, MergeColNumber: 5, CellAlign: "left" });
                            bodyData.push({ Columns: currentRow });
                        } else {
                            currentRow.push(
                                { Name: "ColNo", Value: i + 1 },
                                { Name: "ColDate", Value: info.Date },
                                { Name: "ColSport", Value: info.ReportRows[0].Sport, MergeColNumber: 4, CellAlign: "left" },
                                { Name: "ColChangedBy", Value: info.ChangedBy });
                            bodyData.push({ Columns: currentRow });
                        }
                    }
                } else if (numberMergeRows === 1) {
                    for (var j = 0; j < numberRows; j++) {
                        virtualRateBySport = info.ReportRows[j];
                        for (var k = 0; k < virtualRateBySport.VirtualRateByBettype.length; k++) {
                            var currentVirtualRate = virtualRateBySport.VirtualRateByBettype[k];
                            currentRow = [];
                            currentRow.push(
                                { Name: "ColNo", Value: i + 1 },
                                { Name: "ColDate", Value: info.Date },
                                { Name: "ColSport", Value: virtualRateBySport.Sport },
                                { Name: "ColBettype", Value: currentVirtualRate.BettypeName },
                                { Name: "ColLiveStatus", Value: virtualRateBySport.LiveStatus },
                                { Name: "ColVirtualRate", Value: currentVirtualRate.VirtualRate },
                                { Name: "ColChangedBy", Value: info.ChangedBy }
                            );

                            bodyData.push({ Columns: currentRow });
                        }
                    }
                } else {
                    for (var j = 0; j < numberRows; j++) {
                        virtualRateBySport = info.ReportRows[j];
                        for (var k = 0; k < virtualRateBySport.VirtualRateByBettype.length; k++) {
                            currentRow = [];
                            var currentVirtualRate = virtualRateBySport.VirtualRateByBettype[k];
                            if (j == 0 && k == 0) {
                                currentRow.push({ Name: "ColNo", Value: i + 1, MergeRowNumber: numberMergeRows });
                                currentRow.push({ Name: "ColDate", Value: info.Date, MergeRowNumber: numberMergeRows });
                            }

                            currentRow.push(
                                { Name: "ColSport", Value: virtualRateBySport.Sport },
                                { Name: "ColBettype", Value: currentVirtualRate.BettypeName },
                                { Name: "ColLiveStatus", Value: virtualRateBySport.LiveStatus },
                                { Name: "ColVirtualRate", Value: currentVirtualRate.VirtualRate }
                            );

                            if (j == 0 && k == 0) {
                                currentRow.push({ Name: "ColChangedBy", Value: info.ChangedBy, MergeRowNumber: numberMergeRows });
                            }

                            bodyData.push({ Columns: currentRow });
                        }
                    }
                }
            }
        }

        var girdOptions = {
            columns: [{ HeaderText: "No", Width: 50, Name: "ColNo", HeaderAlign: "center", CellAlign: "center" },
                { HeaderText: "Date", Width: 150, Name: "ColDate", HeaderAlign: "center", CellAlign: "center" },
                { HeaderText: "Sport", Width: 120, Name: "ColSport", HeaderAlign: "center", CellAlign: "left" },
                { HeaderText: "Bet Type", Width: 150, Name: "ColBettype", HeaderAlign: "center", CellAlign: "center" },
                { HeaderText: "Live/Non-Live", Width: 120, Name: "ColLiveStatus", HeaderAlign: "center", CellAlign: "center" },
                { HeaderText: "Virtual Rate(%)", Width: 120, Name: "ColVirtualRate", HeaderAlign: "center", CellAlign: "right", FormatNumber: FormatConstant.PERCENT_NUMBER },
                { HeaderText: "Changed By", Width: 100, Name: "ColChangedBy", HeaderAlign: "center", CellAlign: "center"}],
            bodyRows: bodyData,
            showDistinctRow: false,
            showHighlightRow: false,
            fullHeight: false,
            gridHeight: 350

        };
        _tables.report.removeClass("display-none").fanexGrid(girdOptions);
        _tables.reportContainer.dialog({
            title: "VIRTUAL RATE UPDATE HISTORY (" + self.Params.reportParams.Username + ")",
            autoOpen: false,
            height: 400,
            width: 900,
            modal: false,
            resizable: false,
            close: function (event, ui) {
                _tables.reportContainer.dialog("destroy").remove();
                _tables.reportParentContainer.append('<div id="virtualRateHistoryContainer" class="display-none"> \
                                                        <div id="virtualRateHistory" class="freegrid display-none"> \
                                                        </div> \
                                                      </div>');
                _tables.reportContainer = $('#virtualRateHistoryContainer');
                _tables.report = $('#virtualRateHistory');
            }
        });

        _tables.reportContainer.dialog("open");
    };
};
