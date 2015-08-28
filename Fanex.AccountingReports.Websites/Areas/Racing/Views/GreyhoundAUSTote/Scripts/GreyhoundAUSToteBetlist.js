/* File Created: January 17, 2014 *//* File Created: December 27, 2013 */
Namespace.Register("Fanex.AccountingReports.Racing.GreyhoundAUSTote.GreyhoundAUSToteBetlist");
Fanex.AccountingReports.Racing.GreyhoundAUSTote.GreyhoundAUSToteBetlist = function () {
    var self = this;
    var _tables = {
        report: null,
        reportContainer: null
    };

    self.Params = {
        reportParams: {
            winlostDate: null,
            leagueId: null,
            currency: null,
            raceNo: null
        }
    };

    self.initPage = function () {
        cachedDOMElements();
    };

    var cachedDOMElements = function () {
        _tables.reportParentContainer = $("#parentContainer");
        _tables.report = $("#betListContent");
        _tables.reportContainer = $("#betListContainer");
    };

    var getParams = function (params) {
        self.Params.reportParams = params;
    };

    self.betlistgridOpts = {
        tblContentId: 'tblContent',
        manualCopyHeader: true,
        isShowPager: false,
        fullWidth: true,
        fullHeight: false,
        gridWidth: 1150, //=totalColumnWidth + totalColumn*4 +totalColumn
        gridHeight: 300,
        freezeWidth: 0, //=totalColumnWidth + (totalColumn-1)*4 + (totalColumn-1)
        numberFreezeCol: 0
    };

    self.getContent = function (params) {
        AccReport.disableInput();
        getParams(params);
        $.ajax({
            type: "POST",
            url: window.rootUrl + "BetList/BetList/GetBetlistRacingToteWinLoss",
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            data: JSON.stringify(self.Params.reportParams),
            success: function (rs) {
                self.RenderBetList(rs);
            },
            complete: AccReport.enableInput
        });
    };

    self.RenderBetList = function (report) {
        _tables.reportContainer.removeClass("display-none");
        _tables.report.addClass("display-none");
        var bodyData = [];
        var footerData = [];
        if (report && report.BetListDetail && report.BetListDetail.length > 0) {
            var numberOfTicket = report.BetListDetail.length;
            for (var i = 0; i < numberOfTicket; i++) {
                var ticket = report.BetListDetail[i];
                var currentRow = {
                    Columns: [
                            { Name: "ColNo", Value: ticket.No },
                            { Name: "ColUsername", Value: ticket.Username },
                            { Name: "ColTransTime", Value: "<span>Ref No:" + ticket.TransId + "</span></br>" + ticket.TransDate },
                            { Name: "ColBetType", Value: "<span class='choice'>" + ticket.Choice + "</span><br/><span class='bettype'>" + ticket.BettypeName },
                            { Name: "ColOdds", Value: ticket.Odds },
                            { Name: "ColStake", Value: ticket.Stake },
                            { Name: "ColMemberWL", Value: ticket.MemberWinLoss + "<br/><span class='extra'>" + ticket.PlayerComm + "</span>" },
                            { Name: "ColAgencyWL", Value: ticket.AgencyPTWinLoss + "<br/><span class='extra'>" + ticket.AgencyCommIncome + "</span>" },
                            { Name: "ColIBCWL", Value: ticket.IBCPTWinLoss + "<br/><span class='extra'>" + ticket.IBCCommIncome + "</span>" },
                            { Name: "ColStatus", Value: ticket.Status },
                            { Name: "ColDividend", Value: ticket.Dividend }
                        ]
                };
                bodyData.push(currentRow);
            }
            var total = report.Total;
            var currentRow = {
                Columns: [
                            { Name: "ColNo", Value: "Total", MergeColNumber: 5, CellAlign: "right" },
                            { Name: "ColStake", Value: total.Stake },
                            { Name: "ColMemberWL", Value: total.MemberWinLoss + "<br/><span class='extra'>" + total.PlayerComm + "</span>" },
                            { Name: "ColAgencyWL", Value: total.AgencyPTWinLoss + "<br/><span class='extra'>" + total.AgencyCommIncome + "</span>" },
                            { Name: "ColIBCWL", Value: total.IBCPTWinLoss + "<br/><span class='extra'>" + total.IBCCommIncome + "</span>" },
                            { Name: "ColStatus", Value: "" },
                            { Name: "ColDividend", Value: total.Dividend }
                            ]
            };
            footerData.push(currentRow);
        }

        var gridOptions = {
            columns: [{ HeaderText: "No", Width: 30, Name: "ColNo", HeaderAlign: "center", CellAlign: "center" },
                { HeaderText: "Member", Width: 100, Name: "ColUsername", HeaderAlign: "center", CellAlign: "center" },
                { HeaderText: "Trans.Time", Width: 120, Name: "ColTransTime", HeaderAlign: "center", CellAlign: "center" },
                { HeaderText: "Horse/Bet type", Width: 220, Name: "ColBetType", HeaderAlign: "center", CellAlign: "right" },
                { HeaderText: "Odds", Width: 80, Name: "ColOdds", HeaderAlign: "center", CellAlign: "right" },
                { HeaderText: "Stake", Width: 80, Name: "ColStake", HeaderAlign: "center", CellAlign: "right" },
                { HeaderText: "Member WinLoss/Comm", Width: 100, Name: "ColMemberWL", HeaderAlign: "center", CellAlign: "right" },
                { HeaderText: "Agency WinLoss/Comm", Width: 100, Name: "ColAgencyWL", HeaderAlign: "center", CellAlign: "right" },
                { HeaderText: "IBC WinLoss/Comm", Width: 100, Name: "ColIBCWL", HeaderAlign: "center", CellAlign: "right" },
                { HeaderText: "Status", Width: 80, Name: "ColStatus", HeaderAlign: "center", CellAlign: "center" },
                { HeaderText: "Dividend", Width: 80, Name: "ColDividend", HeaderAlign: "center", CellAlign: "right"}],
            showTitle: true,
            showFooter: true,
            titleInfo:
            {
                text: "Greyhound Daily Report for Aust - Date " + self.Params.reportParams.winlostDate,
                showExcelButton: false
            },
            bodyRows: bodyData,
            footerRows: footerData,
            gridExpandHeight: 0.3 * $(window).height(),
            showDistinctRow: false,
            fullWidth: false,
            gridWidth: 800,
            showHighlightRow: false,
            numberOfFrozenColumn: 0,
            bodyRowHeight: 34,
            headerColumnHeight: 34
        };

        var heightPopup = report.BetListDetail.length > 10 ? 560 : report.BetListDetail.length * 34;

        _tables.report.removeClass("display-none").fanexGrid(gridOptions);
        _tables.reportContainer.dialog({
            title: "",
            autoOpen: false,
            height: heightPopup + 220,
            width: 1200,
            modal: false,
            resizable: false,
            close: function (event, ui) {
                _tables.reportContainer.dialog("destroy").remove();
                _tables.reportParentContainer.append('<div id="betListContainer" class="display-none"> \
                                                        <div id="betListContent" class="freegrid display-none"> \
                                                        </div> \
                                                      </div>');
                _tables.reportContainer = $('#betListContainer');
                _tables.report = $('#betListContent');
            }
        });

        _tables.reportContainer.dialog("open");
    };
};