
Namespace.Register("Fanex.AccountingReports.Racing.RacingDailyReport.RacingDailyReportApply50Percent");

Fanex.AccountingReports.Racing.RacingDailyReport.RacingDailyReportApply50Percent = function () {
    var self = this;

    var _table = {
        RacingDailyReportContainer: null,
        RacingDailyReportContent: null
    };

    var _params = {
        RacingDaily: {
            WinLostDate: null,
            Site: null,
            IsPagingReport: null,
            NumberItemsPerPage: 50,
            StartIndex: 0
        }
    };

    var _gridOption = {
        itemsPerPage: 50,
        currentPage: 0,
        startIndex: 0
    };

    var _detailBetListOption = {
        tblContentId: 'tblContent',
        fullWidth: true,
        gridHeight: 235
    };

    self.initPage = function () {
        catchedDOMElement();
    };

    var catchedDOMElement = function () {
        _table.RacingDailyReportContainer = $('#RacingDailyReportContainer');
        _table.RacingDailyReportContent = $('#RacingDailyReportContent'); 
    };

    self.getDailyReport = function () {
        AccReport.disableInput();

        if (validateParams() == true) {
            $.ajax({
                type: "POST",
                url: window.rootUrl + "Racing/RacingWinLossByLicensee/GetRacingWinLossByLicensee",
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                data: JSON.stringify(_params.RacingDaily),
                beforeSend: function () {
                    AccReport.disableInput();
                },
                success: function (report) {
                    AccReport.popUp.hide();
                    renderDailyReport(report);
                },
                error: function (jqXHR, event, ajaxSettings) {
                    if (jqXHR.status &&
                            jqXHR.status == AccReportData.Constants.HtmlErrorCode.SystemError) {
                        var r = jQuery.parseJSON(jqXHR.responseText);
                        AccReport.popUp.show(r.message);
                    }
                    renderDailyReport(null);
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

    self.getRacingParams = function (params) {
        _params.RacingDaily.WinLostDate = params.WinLostDate;
        _params.RacingDaily.Site = params.Site;
    };

    self.reset = function () {
        resetGridOptions();
        resetParams();
    };

    var resetGridOptions = function () {
        _gridOption.itemsPerPage = 50;
        _gridOption.startIndex = 0;
        _gridOption.currentPage = 0;
    };

    var resetParams = function () {
        _params.RacingDaily.IsPagingReport = false;
        _params.RacingDaily.StartIndex = _gridOption.startIndex;
        _params.RacingDaily.NumberItemsPerPage = _gridOption.itemsPerPage;
    };

    var validateParams = function () {
        return true;
    };

    var renderDailyReport = function (report) {
        var reportTitle = "Racing Daily Report (" + _params.RacingDaily.Site + ") - Date: " + _params.RacingDaily.WinLostDate;
        var bodyData = [];
        var footerData = [];

        if (report != null && report.Rows != null && report.Rows.length > 0) {
            var numberRaces = report.ID;
            bodyData = renderCurrency(report.Rows, bodyData);

            if (report.Races != null && report.Races.length >= 2) {
                footerData = renderGrandTotal(report.Races[1], footerData);
            }

            renderGrid(reportTitle, bodyData, footerData, numberRaces);
        }
        else {
            renderGrid(reportTitle, [], [], 0);
        }
    };

    var renderCurrency = function (currencies, bodyData) {
        var numberCurrencies = currencies.length;

        for (var i = 0; i < numberCurrencies; i++) {
            var currency = currencies[i];

            if (currency.Rows != null && currency.Rows.length > 0) {
                bodyData = renderLeague(currency.ID, currency.Name, currency.Rows, bodyData);

                if (currency.Races != null && currency.Races.length >= 2)
                    bodyData = renderCurrencyTotal(currency.Name, currency.Races, bodyData);
            }
        }

        return bodyData;
    };

    var renderLeague = function (currencyID, currencyName, leagues, bodyData) {
        var numberLeagues = leagues.length;

        for (var i = 0; i < numberLeagues; i++) {
            var league = leagues[i];
            var leagueName = league.Name;

            if (league.Rows != null && league.Rows.length > 0) {
                var numberRaces = league.Rows.length;
                bodyData = renderRace(currencyID, currencyName, league.ID, leagueName, league.Rows, bodyData);

                if (league.Races != null && league.Races.length >= 2) {
                    bodyData = renderLeagueTotal(currencyName, leagueName, league.Races[0], bodyData);
                }

                bodyData.push(
                {
                    Columns: [
                        { Name: "MeetingCurrency", Value: "", MergeColNumber: 9, TdClass: "blank-row" }
                    ]
                });
            } 

        }

        return bodyData;
    };

    var renderRace = function (currencyID, currencyName, leagueID, leagueName, races, bodyData) {
        var numberRaces = races.length;
        var currentRow = [];

        for (var i = 0; i < numberRaces; i++) {
            var race = races[i];
            var winLost = race.Races[0];
            var parameterToBetList = {
                leagueId: leagueID,
                currencyId: currencyID,
                raceId: race.ID,
                name: leagueName + " - " + currencyName
            };

            if (i == 0) {
                currentRow =
                    [
                        { Name: "MeetingCurrency", Value: leagueName + " - " + currencyName,
                            MergeRowNumber: numberRaces, TdClass: "bold"
                        },
                        { Name: "RaceID", Value: parameterToBetList,
                            CustomCellRender: function (currentCell) {
                                return createLinkDetailBetList(currentCell);
                            }
                        }
                    ];

                currentRow = renderWinLost(winLost, currentRow, "");
            }
            else {
                currentRow =
                    [
                        { Name: "RaceID", Value: parameterToBetList,
                            CustomCellRender: function (currentCell) {
                                return createLinkDetailBetList(currentCell);
                            }
                        }
                    ];
                currentRow = renderWinLost(winLost, currentRow, "");
            }

            bodyData.push(
                {
                    Columns: currentRow
                }
            );
        }

        return bodyData;
    };

    var createLinkDetailBetList = function (currentCell) {
        if (currentCell != null) {
            var leagueId = currentCell.Value.leagueId;
            var currencyId = currentCell.Value.currencyId;
            var raceId = currentCell.Value.raceId;
            var name = currentCell.Value.name;
            var link = '<span class="link bold" onclick="OpenDetailBetList('
                                            + leagueId + ',' + raceId + ','
                                            + currencyId + ',\'' + name + '\');">' + raceId + '</span>';
            return link;
        }
    };

    var renderLeagueTotal = function (currencyName, leagueName, winLost, bodyData) {
        var tdClass = "league-total";
        var currentRow = [
            { Name: "MeetingCurrency", Value: leagueName + " - " + currencyName + " Total",
                MergeColNumber: 2, TdClass: tdClass
            }
        ];
        currentRow = renderWinLost(winLost, currentRow, tdClass);
        bodyData.push({
            Columns: currentRow
        });

        return bodyData;
    };

    var renderCurrencyTotal = function (currencyName, total, bodyData) {
        var tdClass = "currency-total";
        var currentRow = [];
        currentRow = [
            { Name: "MeetingCurrency", Value: currencyName + " Total", MergeRowNumber: 2, TdClass: tdClass },
            { Name: "RaceID", Value: currencyName, TdClass: tdClass }
        ];
        currentRow = renderWinLost(total[0], currentRow, tdClass);
        bodyData.push({
            Columns: currentRow
        });
        currentRow = [
            { Name: "RaceID", Value: "Base (RM)", TdClass: tdClass }
        ];
        currentRow = renderWinLost(total[1], currentRow, tdClass);
        bodyData.push(
        {
            Columns: currentRow
        }
        );
        return bodyData;
    };

    var renderGrandTotal = function (grandTotal, footerData) {
        var currentRow = [
            { Name: "MeetingCurrency", Value: "Grand Total", MergeColNumber: 2 }
        ];
        currentRow = renderWinLost(grandTotal, currentRow, "");
        footerData.push(
        {
            Columns: currentRow
        }
        );

        return footerData;
    };

    var renderWinLost = function (winLost, currentRow, tdClass) {
        currentRow = currentRow.concat(
                [
                    { Name: "WinTO", Value: winLost.MemberWinTurnover, TdClass: tdClass },
                    { Name: "PlaceTO", Value: winLost.MemberPlaceTurnover, TdClass: tdClass },
                    { Name: "WinPlaceTO", Value: winLost.MemberWinPlaceTurnover, TdClass: tdClass },
                    { Name: "TotalTO", Value: winLost.MemberTotalTurnover, TdClass: tdClass },
                    { Name: "DividendWon", Value: winLost.MemberDividendWon, TdClass: tdClass },
                    { Name: "LicenseeCommIncome", Value: winLost.Site12BetCommissionIncome, TdClass: tdClass },
                    { Name: "IBCAmountDue", Value: winLost.AmountDueToIBC, TdClass: tdClass }
                ]
                );

        return currentRow;
    };

    var renderGrid = function (reportTitle, bodyData, footerData, numberRaces) {
        var columns = [
            { HeaderText: "Meeting - Currency", Name: "MeetingCurrency", Width: 200, HeaderAlign: "center",
                CellAlign: "center"
            },
            { HeaderText: "Race", Name: "RaceID", Width: 100, HeaderAlign: "center",
                CellAlign: "center"
            },
            { HeaderText: "Win TO", Name: "WinTO", Width: 110, HeaderAlign: "center",
                CellAlign: "right", FormatNumber: FormatConstant.TWO_DECIMAL_NUMBER
            },
            { HeaderText: "Place TO", Name: "PlaceTO", Width: 110, HeaderAlign: "center",
                CellAlign: "right", FormatNumber: FormatConstant.TWO_DECIMAL_NUMBER
            },
            { HeaderText: "W/P TO", Name: "WinPlaceTO", Width: 110, HeaderAlign: "center",
                CellAlign: "right", FormatNumber: FormatConstant.TWO_DECIMAL_NUMBER
            },
            { HeaderText: "Total TO", Name: "TotalTO", Width: 110, HeaderAlign: "center",
                CellAlign: "right", FormatNumber: FormatConstant.TWO_DECIMAL_NUMBER
            },
            { HeaderText: "Dividend Won", Name: "DividendWon", Width: 110, HeaderAlign: "center",
                CellAlign: "right", FormatNumber: FormatConstant.TWO_DECIMAL_NUMBER
            },
            { HeaderText: "Comm Income", Name: "LicenseeCommIncome", Width: 110, HeaderAlign: "center",
                CellAlign: "right", FormatNumber: FormatConstant.TWO_DECIMAL_NUMBER
            },
            { HeaderText: "Amount due to IBC", Name: "IBCAmountDue", Width: 130, HeaderAlign: "center",
                CellAlign: "right", FormatNumber: FormatConstant.TWO_DECIMAL_NUMBER
            },
        ];

        var gridOptions = { 
            columns: columns,
            bodyRows: bodyData,
            showTitle: true,
            titleInfo: {
                text: reportTitle,
                showExcelButton: true,
                exportExcelFunction: function () {
                    self.exportReport();
                }
            },
            showHighlightRow: false,
            showDistinctRow: false,
            showFooter: true,
            footerRows: footerData,
            gridExpandHeight: 198,
            showPager: true,
            customPager: true,
            pagerOption: {
                itemsPerPage: _gridOption.itemsPerPage,
                currentPage: _gridOption.currentPage,
                itemsPerPageArray: [50, 100, 200, 500],
                totalItem: numberRaces,
                pagerSelectCallBack: ReRenderAfterPaged
            }
        };

        _table.RacingDailyReportContainer.removeClass("display-none");
        _table.RacingDailyReportContent.fanexGrid(gridOptions);

        function ReRenderAfterPaged(pageIndex, itemsPerPage, jq) {
            _params.RacingDaily.IsPagingReport = true;
            _params.RacingDaily.StartIndex = pageIndex >= 0 ? (pageIndex * itemsPerPage) : 0;
            _params.RacingDaily.NumberItemsPerPage = itemsPerPage;

            _gridOption.itemsPerPage = itemsPerPage;
            _gridOption.currentPage = pageIndex;
            _gridOption.startIndex = _params.RacingDaily.StartIndex;

            self.getDailyReport();
            return false;
        }
    };

    self.exportReport = function () {
        AccReport.downloadFile(window.rootUrl + "Racing/RacingWinLossByLicensee/Export50PercentReport",
                                _params.RacingDaily);
    };
};