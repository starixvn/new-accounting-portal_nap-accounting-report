/* File Created: January 17, 2014 *//* File Created: December 24, 2013 */
/* File Created: December 20, 2013 */
Namespace.Register("Fanex.AccountingReports.Racing.GreyhoundAUSTote.GreyhoundAUSToteDetail");
Fanex.AccountingReports.Racing.GreyhoundAUSTote.GreyhoundAUSToteDetail = function () {
    var self = this;
    var _tables = {
        btnGenerate: null,
        reportContainer: null,
        report: null,
        netIncomeFormula: null,
        amountDueFormula: null
    };
    self.Params = {
        reportParams: {
            winlostDate: null,
            partnerId: null,
            poolCurrency: null,
            partnerName: null
        }
    };

    self.initPage = function () {
        cachedDOMElements();
    };

    var cachedDOMElements = function () {
        _tables.reportContainer = $("#greyhoundAusDetailReportContainer");
        _tables.report = $("#greyhoundAusDetailReport");

    };

    var renderFilterNote = function (id, noteMessage) {
        //// Assign Title for tooltip
        id.attr("title", noteMessage);
        id.tipsy({
            html: true,
            opacity: 0.9,
            gravity: 'se'
        });
    };

    self.getReportRacingDetail = function (params) {
        AccReport.disableInput();
        getParams(params);
        $.ajax({
            type: "POST",
            url: window.rootUrl + "Racing/GreyhoundAUSTote/GetRacingDailyWinLossDetail",
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            data: JSON.stringify(self.Params.reportParams),
            beforeSend: function () {
                AccReport.disableInput();
            },
            success: function (rs) {
                //self.hideReport();
                renderRacingWinLossDetail(rs);
            },
            error: function (event, jqXHR, ajaxSettings) {
                //self.hideReport();
                if (jqXHR.status &&
                    jqXHR.status == AccReportData.Constants.HtmlErrorCode.SystemError) {
                    var r = jQuery.parseJSON(jqXHR.responseText);
                    AccReport.popUp.show(r.message);
                    renderRacingWinLossDetail(null);
                }
            },
            complete: function () {
                _tables.amountDueFormula = $("#amountDueFormulaDetail");
                _tables.netIncomeFormula = $("#netIncomeFormulaDetail");
                renderFilterNote(_tables.netIncomeFormula, netIncomeFormula);
                renderFilterNote(_tables.amountDueFormula, amountDueFormula);
                AccReport.enableInput();
            }
        });
    };

    var getParams = function (params) {
        self.Params.reportParams = params;
    };
    var renderRacingWinLossDetail = function (report) {
        _tables.reportContainer.removeClass("display-none");
        _tables.report.addClass("display-none");
        var bodyData = [];
        var footerData = [];
        if (report && report.ChildsInfo && report.ChildsInfo.length > 0) {
            var currGroupNumber = report.ChildsInfo.length;
            for (var i = 0; i < currGroupNumber; i++) {
                var currGroup = report.ChildsInfo[i];
                var currGroupLength = currGroup.ChildsInfo.length;

                for (var j = 0; j < currGroupLength; j++) {
                    var leagueItem = currGroup.ChildsInfo[j];
                    var raceItemsLength = leagueItem.ChildsInfo.length;
                    for (var k = 0; k < raceItemsLength; k++) {
                        var raceItem = leagueItem.ChildsInfo[k];
                        var currentColumns = [];
                        if (k == 0) {
                            currentColumns.push({ Name: "ColMeeting", Value: leagueItem.LeagueName, MergeRowNumber: raceItemsLength });
                        }
                        var currentRow = {
                            Columns: currentColumns.concat([
                            { Name: "ColRace", Value: {
                                leagueId: leagueItem.MeetingId,
                                raceNo: raceItem.RaceNo,
                                currency: currGroup.CurrencyId
                            },
                                CustomCellRender: function (currentCell) {
                                    var clickEvent = ' onclick = "viewRacingToteBetlist(' + currentCell.Value.leagueId + ',' + currentCell.Value.currency + ',\'' + currentCell.Value.raceNo + "');\"";
                                    var html = '<span class="link " ' + clickEvent + '>' + currentCell.Value.raceNo + '</span>';
                                    return html;
                                }
                            },
                            { Name: "ColWinTOAgency", Value: raceItem.WinTOAgency },
                            { Name: "ColPlaceTOAgency", Value: raceItem.PlaceTOAgency },
                            { Name: "ColWinPlaceTOAgency", Value: raceItem.WinPlaceTOAgency },
                            { Name: "ColTotalTOAgency", Value: raceItem.TotalTOAgency },
                            { Name: "ColCommIncomeAgency", Value: raceItem.CommIncomeAgency },
                            { Name: "ColPTWinLossAgency", Value: raceItem.PTWinLossAgency },
                            { Name: "ColWinTOIBC", Value: raceItem.WinTOIBC },
                            { Name: "ColPlaceTOIBC", Value: raceItem.PlaceTOIBC },
                            { Name: "ColWinPlaceTOIBC", Value: raceItem.WinPlaceTOIBC },
                            { Name: "ColTotalTOIBC", Value: raceItem.TotalTOIBC },
                            { Name: "ColCommIncomeIBC", Value: raceItem.CommIncomeIBC },
                            { Name: "ColToteFeeIBC", Value: raceItem.ToteFeeIBC },
                            { Name: "ColCommExpenseIBC", Value: raceItem.CommExpenseIBC },
                            { Name: "ColPTWinLossIBC", Value: raceItem.PTWinLossIBC },
                            { Name: "ColNetIncomeIBC", Value: raceItem.NetIncomeIBC },
                            { Name: "ColWinTOPartner", Value: raceItem.WinTOPartner },
                            { Name: "ColPlaceTOPartner", Value: raceItem.PlaceTOPartner },
                            { Name: "ColWinPlaceTOPartner", Value: raceItem.WinPlaceTOPartner },
                            { Name: "ColTotalTOPartner", Value: raceItem.TotalTOPartner },
                            { Name: "ColDividendWonPartner", Value: raceItem.DividendWonPartner },
                            { Name: "ColCommExpensePartner", Value: raceItem.CommExpensePartner },
                            { Name: "ColToteFeePartner", Value: raceItem.ToteFeePartner },
                            { Name: "ColAmountDuePartner", Value: raceItem.AmountDuePartner}])
                        };
                        bodyData.push(currentRow);
                    }
                    //Show Total for League
                    var leagueTotal = leagueItem.Total;
                    bodyData.push({
                        Columns: [{ Name: "ColMeeting", Value: leagueTotal.LeagueName, MergeColNumber: 2 },
                            { Name: "ColWinTOAgency", Value: leagueTotal.WinTOAgency },
                            { Name: "ColPlaceTOAgency", Value: leagueTotal.PlaceTOAgency },
                            { Name: "ColWinPlaceTOAgency", Value: leagueTotal.WinPlaceTOAgency },
                            { Name: "ColTotalTOAgency", Value: leagueTotal.TotalTOAgency },
                            { Name: "ColCommIncomeAgency", Value: leagueTotal.CommIncomeAgency },
                            { Name: "ColPTWinLossAgency", Value: leagueTotal.PTWinLossAgency },
                            { Name: "ColWinTOIBC", Value: leagueTotal.WinTOIBC },
                            { Name: "ColPlaceTOIBC", Value: leagueTotal.PlaceTOIBC },
                            { Name: "ColWinPlaceTOIBC", Value: leagueTotal.WinPlaceTOIBC },
                            { Name: "ColTotalTOIBC", Value: leagueTotal.TotalTOIBC },
                            { Name: "ColCommIncomeIBC", Value: leagueTotal.CommIncomeIBC },
                            { Name: "ColToteFeeIBC", Value: leagueTotal.ToteFeeIBC },
                            { Name: "ColCommExpenseIBC", Value: leagueTotal.CommExpenseIBC },
                            { Name: "ColPTWinLossIBC", Value: leagueTotal.PTWinLossIBC },
                            { Name: "ColNetIncomeIBC", Value: leagueTotal.NetIncomeIBC },
                            { Name: "ColWinTOPartner", Value: leagueTotal.WinTOPartner },
                            { Name: "ColPlaceTOPartner", Value: leagueTotal.PlaceTOPartner },
                            { Name: "ColWinPlaceTOPartner", Value: leagueTotal.WinPlaceTOPartner },
                            { Name: "ColTotalTOPartner", Value: leagueTotal.TotalTOPartner },
                            { Name: "ColDividendWonPartner", Value: leagueTotal.DividendWonPartner },
                            { Name: "ColCommExpensePartner", Value: leagueTotal.CommExpensePartner },
                            { Name: "ColToteFeePartner", Value: leagueTotal.ToteFeePartner },
                            { Name: "ColAmountDuePartner", Value: leagueTotal.AmountDuePartner}],
                        CssClass: "bold totalDetailByCurr"
                    });
                }
                //Show Currency Group 
                var currTotal = currGroup.Total;
                var currTotalLength = currTotal.ChildsInfo.length;
                for (var m = 0; m < currTotalLength; m++) {
                    var currTotalItem = currTotal.ChildsInfo[m];
                    var currentColumns = [];
                    if (m == 0) {
                        currentColumns.push({ Name: "ColMeeting", Value: currTotal.LeagueName, MergeRowNumber: currTotalLength });
                    }
                    var currentRow = {
                        Columns: currentColumns.concat([
                            { Name: "ColRace", Value: currTotalItem.CurrencyName },
                            { Name: "ColWinTOAgency", Value: currTotalItem.WinTOAgency },
                            { Name: "ColPlaceTOAgency", Value: currTotalItem.PlaceTOAgency },
                            { Name: "ColWinPlaceTOAgency", Value: currTotalItem.WinPlaceTOAgency },
                            { Name: "ColTotalTOAgency", Value: currTotalItem.TotalTOAgency },
                            { Name: "ColCommIncomeAgency", Value: currTotalItem.CommIncomeAgency },
                            { Name: "ColPTWinLossAgency", Value: currTotalItem.PTWinLossAgency },
                            { Name: "ColWinTOIBC", Value: currTotalItem.WinTOIBC },
                            { Name: "ColPlaceTOIBC", Value: currTotalItem.PlaceTOIBC },
                            { Name: "ColWinPlaceTOIBC", Value: currTotalItem.WinPlaceTOIBC },
                            { Name: "ColTotalTOIBC", Value: currTotalItem.TotalTOIBC },
                            { Name: "ColCommIncomeIBC", Value: currTotalItem.CommIncomeIBC },
                            { Name: "ColToteFeeIBC", Value: currTotalItem.ToteFeeIBC },
                            { Name: "ColCommExpenseIBC", Value: currTotalItem.CommExpenseIBC },
                            { Name: "ColPTWinLossIBC", Value: currTotalItem.PTWinLossIBC },
                            { Name: "ColNetIncomeIBC", Value: currTotalItem.NetIncomeIBC },
                            { Name: "ColWinTOPartner", Value: currTotalItem.WinTOPartner },
                            { Name: "ColPlaceTOPartner", Value: currTotalItem.PlaceTOPartner },
                            { Name: "ColWinPlaceTOPartner", Value: currTotalItem.WinPlaceTOPartner },
                            { Name: "ColTotalTOPartner", Value: currTotalItem.TotalTOPartner },
                            { Name: "ColDividendWonPartner", Value: currTotalItem.DividendWonPartner },
                            { Name: "ColCommExpensePartner", Value: currTotalItem.CommExpensePartner },
                            { Name: "ColToteFeePartner", Value: currTotalItem.ToteFeePartner },
                            { Name: "ColAmountDuePartner", Value: currTotalItem.AmountDuePartner}]),
                        CssClass: "bold grandTotalDetailByCurr"
                    };
                    bodyData.push(currentRow);
                }
                bodyData.push({ Columns: [
                    { Name: "ColMeeting", Value: "", MergeColNumber: 2 },
                    { Name: "ColWinTOAgency", Value: "", MergeColNumber: 23}]
                });
            }
            //Show Footer
            var racingTotal = report.Total;
            var racingTotalLength = racingTotal.ChildsInfo.length;
            for (var n = 0; n < racingTotalLength; n++) {
                var totalItem = racingTotal.ChildsInfo[n];
                if (n < racingTotalLength - 1) {
                    var currentRow = {
                        Columns: [
                            { Name: "ColMeeting", Value: totalItem.LeagueName },
                            { Name: "ColRace", Value: totalItem.CurrencyName },
                            { Name: "ColWinTOAgency", Value: totalItem.WinTOAgency },
                            { Name: "ColPlaceTOAgency", Value: totalItem.PlaceTOAgency },
                            { Name: "ColWinPlaceTOAgency", Value: totalItem.WinPlaceTOAgency },
                            { Name: "ColTotalTOAgency", Value: totalItem.TotalTOAgency },
                            { Name: "ColCommIncomeAgency", Value: totalItem.CommIncomeAgency },
                            { Name: "ColPTWinLossAgency", Value: totalItem.PTWinLossAgency },
                            { Name: "ColWinTOIBC", Value: totalItem.WinTOIBC },
                            { Name: "ColPlaceTOIBC", Value: totalItem.PlaceTOIBC },
                            { Name: "ColWinPlaceTOIBC", Value: totalItem.WinPlaceTOIBC },
                            { Name: "ColTotalTOIBC", Value: totalItem.TotalTOIBC },
                            { Name: "ColCommIncomeIBC", Value: totalItem.CommIncomeIBC },
                            { Name: "ColToteFeeIBC", Value: totalItem.ToteFeeIBC },
                            { Name: "ColCommExpenseIBC", Value: totalItem.CommExpenseIBC },
                            { Name: "ColPTWinLossIBC", Value: totalItem.PTWinLossIBC },
                            { Name: "ColNetIncomeIBC", Value: totalItem.NetIncomeIBC },
                            { Name: "ColWinTOPartner", Value: totalItem.WinTOPartner },
                            { Name: "ColPlaceTOPartner", Value: totalItem.PlaceTOPartner },
                            { Name: "ColWinPlaceTOPartner", Value: totalItem.WinPlaceTOPartner },
                            { Name: "ColTotalTOPartner", Value: totalItem.TotalTOPartner },
                            { Name: "ColDividendWonPartner", Value: totalItem.DividendWonPartner },
                            { Name: "ColCommExpensePartner", Value: totalItem.CommExpensePartner },
                            { Name: "ColToteFeePartner", Value: totalItem.ToteFeePartner },
                            { Name: "ColAmountDuePartner", Value: totalItem.AmountDuePartner}],
                        CssClass: "bold total"
                    };
                    bodyData.push(currentRow);
                }
                else {
                    var grandTotalLength = totalItem.ChildsInfo.length;
                    for (var p = 0; p < grandTotalLength; p++) {
                        var grandTotalItem = totalItem.ChildsInfo[p];
                        var currentColumns = [];
                        if (p == 0) {
                            currentColumns.push({ Name: "ColMeeting", Value: totalItem.LeagueName, MergeRowNumber: grandTotalLength });
                        }
                        var currentRow = {
                            Columns: currentColumns.concat([
                            { Name: "ColRace", Value: grandTotalItem.CurrencyName },
                            { Name: "ColWinTOAgency", Value: grandTotalItem.WinTOAgency },
                            { Name: "ColPlaceTOAgency", Value: grandTotalItem.PlaceTOAgency },
                            { Name: "ColWinPlaceTOAgency", Value: grandTotalItem.WinPlaceTOAgency },
                            { Name: "ColTotalTOAgency", Value: grandTotalItem.TotalTOAgency },
                            { Name: "ColCommIncomeAgency", Value: grandTotalItem.CommIncomeAgency },
                            { Name: "ColPTWinLossAgency", Value: grandTotalItem.PTWinLossAgency },
                            { Name: "ColWinTOIBC", Value: grandTotalItem.WinTOIBC },
                            { Name: "ColPlaceTOIBC", Value: grandTotalItem.PlaceTOIBC },
                            { Name: "ColWinPlaceTOIBC", Value: grandTotalItem.WinPlaceTOIBC },
                            { Name: "ColTotalTOIBC", Value: grandTotalItem.TotalTOIBC },
                            { Name: "ColCommIncomeIBC", Value: grandTotalItem.CommIncomeIBC },
                            { Name: "ColToteFeeIBC", Value: grandTotalItem.ToteFeeIBC },
                            { Name: "ColCommExpenseIBC", Value: grandTotalItem.CommExpenseIBC },
                            { Name: "ColPTWinLossIBC", Value: grandTotalItem.PTWinLossIBC },
                            { Name: "ColNetIncomeIBC", Value: grandTotalItem.NetIncomeIBC },
                            { Name: "ColWinTOPartner", Value: grandTotalItem.WinTOPartner },
                            { Name: "ColPlaceTOPartner", Value: grandTotalItem.PlaceTOPartner },
                            { Name: "ColWinPlaceTOPartner", Value: grandTotalItem.WinPlaceTOPartner },
                            { Name: "ColTotalTOPartner", Value: grandTotalItem.TotalTOPartner },
                            { Name: "ColDividendWonPartner", Value: grandTotalItem.DividendWonPartner },
                            { Name: "ColCommExpensePartner", Value: grandTotalItem.CommExpensePartner },
                            { Name: "ColToteFeePartner", Value: grandTotalItem.ToteFeePartner },
                            { Name: "ColAmountDuePartner", Value: grandTotalItem.AmountDuePartner}])
                        };
                        footerData.push(currentRow);
                    }
                }
            }
        }

        var gridOptions = {
            customizeHeader: true,
            groupHeaderRows: [
                {
                    Columns: [{ HeaderText: "Meeting - Currency", Name: "ColMeeting", MergeRowNumber: 2 },
                        { HeaderText: "Race", Name: "ColRace", MergeRowNumber: 2 },
                        { HeaderText: "Agency", Name: "ColWinTOAgency", MergeColNumber: 6 },
                        { HeaderText: "IBC", Name: "ColWinTOIBC", MergeColNumber: 9 },
                        { HeaderText: "Partner", Name: "ColWinTOPartner", MergeColNumber: 8}]
                }
            ],
            columns: [{ HeaderText: "Meeting - Currency", Width: 150, Name: "ColMeeting", HeaderAlign: "center", CellAlign: "left" },
                { HeaderText: "Race", Width: 80, Name: "ColRace", HeaderAlign: "center", CellAlign: "center" },
                { HeaderText: "Win TO", Width: 80, Name: "ColWinTOAgency", HeaderAlign: "center", CellAlign: "right", FormatNumber: FormatConstant.TWO_DECIMAL_NUMBER },
                { HeaderText: "Place TO", Width: 80, Name: "ColPlaceTOAgency", HeaderAlign: "center", CellAlign: "right", FormatNumber: FormatConstant.TWO_DECIMAL_NUMBER },
                { HeaderText: "W/P TO", Width: 80, Name: "ColWinPlaceTOAgency", HeaderAlign: "center", CellAlign: "right", FormatNumber: FormatConstant.TWO_DECIMAL_NUMBER },
                { HeaderText: "Total TO", Width: 80, Name: "ColTotalTOAgency", HeaderAlign: "center", CellAlign: "right", FormatNumber: FormatConstant.TWO_DECIMAL_NUMBER },
                { HeaderText: "Comm Income", Width: 100, Name: "ColCommIncomeAgency", HeaderAlign: "center", CellAlign: "right", FormatNumber: FormatConstant.TWO_DECIMAL_NUMBER },
                { HeaderText: "PT Win/Loss", Width: 80, Name: "ColPTWinLossAgency", HeaderAlign: "center", CellAlign: "right", FormatNumber: FormatConstant.TWO_DECIMAL_NUMBER },
                { HeaderText: "Win TO", Width: 80, Name: "ColWinTOIBC", HeaderAlign: "center", CellAlign: "right", FormatNumber: FormatConstant.TWO_DECIMAL_NUMBER },
                { HeaderText: "Place TO", Width: 80, Name: "ColPlaceTOIBC", HeaderAlign: "center", CellAlign: "right", FormatNumber: FormatConstant.TWO_DECIMAL_NUMBER },
                { HeaderText: "W/P TO", Width: 80, Name: "ColWinPlaceTOIBC", HeaderAlign: "center", CellAlign: "right", FormatNumber: FormatConstant.TWO_DECIMAL_NUMBER },
                { HeaderText: "Total TO", Width: 80, Name: "ColTotalTOIBC", HeaderAlign: "center", CellAlign: "right", FormatNumber: FormatConstant.TWO_DECIMAL_NUMBER },
                { HeaderText: "Comm Income", Width: 100, Name: "ColCommIncomeIBC", HeaderAlign: "center", CellAlign: "right", FormatNumber: FormatConstant.TWO_DECIMAL_NUMBER },
                { HeaderText: "Tote Fee", Width: 80, Name: "ColToteFeeIBC", HeaderAlign: "center", CellAlign: "right", FormatNumber: FormatConstant.TWO_DECIMAL_NUMBER },
                { HeaderText: "Comm Expense", Width: 100, Name: "ColCommExpenseIBC", HeaderAlign: "center", CellAlign: "right", FormatNumber: FormatConstant.TWO_DECIMAL_NUMBER },
                { HeaderText: "PT Win/Loss", Width: 80, Name: "ColPTWinLossIBC", HeaderAlign: "center", CellAlign: "right", FormatNumber: FormatConstant.TWO_DECIMAL_NUMBER },
                { HeaderText: "Net Income <span id='netIncomeFormulaDetail' class='formular' title=''></span>", Width: 100, Name: "ColNetIncomeIBC", HeaderAlign: "center", CellAlign: "right", FormatNumber: FormatConstant.TWO_DECIMAL_NUMBER },
                { HeaderText: "Win TO", Width: 80, Name: "ColWinTOPartner", HeaderAlign: "center", CellAlign: "right", FormatNumber: FormatConstant.TWO_DECIMAL_NUMBER },
                { HeaderText: "Place TO", Width: 80, Name: "ColPlaceTOPartner", HeaderAlign: "center", CellAlign: "right", FormatNumber: FormatConstant.TWO_DECIMAL_NUMBER },
                { HeaderText: "W/P TO", Width: 80, Name: "ColWinPlaceTOPartner", HeaderAlign: "center", CellAlign: "right", FormatNumber: FormatConstant.TWO_DECIMAL_NUMBER },
                { HeaderText: "Total TO", Width: 80, Name: "ColTotalTOPartner", HeaderAlign: "center", CellAlign: "right", FormatNumber: FormatConstant.TWO_DECIMAL_NUMBER },
                { HeaderText: "Dividend Won", Width: 100, Name: "ColDividendWonPartner", HeaderAlign: "center", CellAlign: "right", FormatNumber: FormatConstant.TWO_DECIMAL_NUMBER },
                { HeaderText: "Comm Expense", Width: 100, Name: "ColCommExpensePartner", HeaderAlign: "center", CellAlign: "right", FormatNumber: FormatConstant.TWO_DECIMAL_NUMBER },
                { HeaderText: "Tote Fee", Width: 80, Name: "ColToteFeePartner", HeaderAlign: "center", CellAlign: "right", FormatNumber: FormatConstant.TWO_DECIMAL_NUMBER },
                { HeaderText: "Amount Due <span id='amountDueFormulaDetail' class='formular' title=''></span>", Width: 100, Name: "ColAmountDuePartner", HeaderAlign: "center", CellAlign: "right", FormatNumber: FormatConstant.TWO_DECIMAL_NUMBER}],
            showTitle: true,
            showFooter: true,
            titleInfo:
            {
                text: "<a class='link' onclick='BackRacingDailyWinLoss();'> Greyhound </a> > " +
                 "Greyhound Daily for Aust Tote - " + self.Params.reportParams.partnerName + " - " + self.Params.reportParams.winlostDate,
                showExcelButton: true,
                exportExcelFunction: function () {
                    self.exportExcel();
                }
            },
            bodyRows: bodyData,
            footerRows: footerData,
            gridExpandHeight: 0.205 * $(window).height(),
            showDistinctRow: false,
            showHighlightRow: false,
            numberOfFrozenColumn: 2,
            headerColumnHeight: 17
        };
        _tables.report.removeClass("display-none").fanexGrid(gridOptions);
    };

    self.hideReport = function () {
        _tables.reportContainer.addClass("display-none");
        _tables.report.addClass("display-none");
    }

    self.showReport = function () {
        _tables.reportContainer.removeClass("display-none");
        _tables.report.removeClass("display-none");
    }

    self.exportExcel = function () {
        AccReport.downloadFile(window.rootUrl + "Racing/GreyhoundAUSTote/ExportExcelRacingWinLossDetail", self.Params.reportParams);
    };
};