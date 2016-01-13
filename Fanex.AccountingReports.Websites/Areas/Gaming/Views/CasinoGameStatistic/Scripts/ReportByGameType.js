Namespace.Register("Fanex.AccountingReports.Gaming.LiveCasinoGameStatistic");
Fanex.AccountingReports.Gaming.LiveCasinoGameStatistic.ReportByGameType = function () {
    var self = this;

    self.GetGridOptions = function (report) {
        var bodyData = [];
        var footerData = [];
        var gameTypeLength = report && report.ChildsInfo ? report.ChildsInfo.length : 0;
        var classPayout = "";
        
        if (gameTypeLength > 0) {
            var info;
            var columns = [];
            var indexRange = reportManager.GetPagingIndexRange(gameTypeLength);
            for (var i = indexRange.startIndex; i < indexRange.endIndex; i++) {
                var gameTypeWinLoss = report.ChildsInfo[i];
                var reportLength = gameTypeWinLoss.ChildsInfo.length;
                for (var j = 0; j < reportLength; j++) {
                    columns = [];
                    info = gameTypeWinLoss.ChildsInfo[j];
                    if (j == 0) {
                        columns.push({ Name: "ColName", Value: gameTypeWinLoss.Name , MergeRowNumber: reportLength });
                    }

                    columns.push({ Name: "ColNumberOfPlayers", Value: info.NumberOfPlayers });
                    columns.push({ Name: "ColNumberOfGames", Value: info.NumberOfGames });
                    columns.push({ Name: "ColCurrency", Value: info, CustomCellRender: function(currentCell) {
                        var clickEvent = ' onclick = "reportManager.GetReportGameTypeCurrency(\'' + currentCell.Value.Id + '\''
                                + ',\'' + currentCell.Value.CurrencyId
                                + "');\"";
                            var result = '<div ' + clickEvent + ' class="link">' + currentCell.Value.CurrencyName + '</div>';

                            return result;
                        }
                    });
                    columns.push({ Name: "ColTotalBets", Value: info.TotalBets });
                    columns.push({ Name: "ColTotalWins", Value: info.TotalWins });
                    columns.push({ Name: "ColGameIncome", Value: info.GameIncome });

                    if (info.GamePayout > 1) {
                        classPayout = " red";
                    }
                    else {
                        classPayout = "";
                    }

                    columns.push({ Name: "ColGamePayout", Value: info.GamePayout, TdClass:classPayout });
                    if (reportManager.Params.reportParams.ShowRefundColumn) {
                        columns.push({ Name: "ColTotalRefund", Value: info.TotalRefund });
                    }

                    columns.push({ Name: "ColCompanyTurnover", Value: info.CompanyTurnover });
                    columns.push({ Name: "CompanyCommission", Value: info.CompanyCommission });
                    columns.push({ Name: "ColCompanyNetIncome", Value: info.CompanyNetIncome });
                    bodyData.push({
                        Columns: columns
                    });
                }
            }

            var totalChildLength = report && report.GrandTotal ?
                report.GrandTotal.ChildsInfo.length : 0;

            for (var totalChildIndex = 0; totalChildIndex < totalChildLength - 1; totalChildIndex++) {
                var currentChildTotal = report.GrandTotal.ChildsInfo[totalChildIndex];
                self.AddChildTotalToBodyData(columns, currentChildTotal, bodyData);
            }

            var reportLength = report.GrandTotal.ChildsInfo.length;

            var footerColumns = [];
            info = report.GrandTotal.ChildsInfo[reportLength - 1];
            footerColumns.push({ Name: "ColName", Value: info, CustomCellRender: function(currentCell) {
                    var clickEvent = ' onclick = "reportManager.RenderGrandTotal();"';
                    var result = '<div ' + clickEvent + ' class="link">' + currentCell.Value.Name + '</div>';

                    return result;
                }
            });
            footerColumns.push({ Name: "ColNumberOfPlayers", Value: info.NumberOfPlayers });
            footerColumns.push({ Name: "ColNumberOfGames", Value: info.NumberOfGames });
            footerColumns.push({ Name: "ColCurrency", Value: info.CurrencyName });
            footerColumns.push({ Name: "ColTotalBets", Value: info.TotalBets });
            footerColumns.push({ Name: "ColTotalWins", Value: info.TotalWins });
            footerColumns.push({ Name: "ColGameIncome", Value: info.GameIncome });

            if (info.GamePayout > 1) {
                classPayout = " red";
            }
            else {
                classPayout = "";
            }

            footerColumns.push({ Name: "ColGamePayout", Value: info.GamePayout, TdClass:classPayout });
            if (reportManager.Params.reportParams.ShowRefundColumn) {
                footerColumns.push({ Name: "ColTotalRefund", Value: info.TotalRefund });
            }

            footerColumns.push({ Name: "ColCompanyTurnover", Value: info.CompanyTurnover });
            footerColumns.push({ Name: "CompanyCommission", Value: info.CompanyCommission });
            footerColumns.push({ Name: "ColCompanyNetIncome", Value: info.CompanyNetIncome });

            footerData.push({ Columns: footerColumns });
        }

        var columnsHeader = [];
        columnsHeader.push({
            HeaderText: "Game Type", Width: 160, Name: "ColName", HeaderAlign: "center", CellAlign: "center",
            Sort: true, SortDirection: reportManager.getSortDirection("Name")
        });
        columnsHeader.push({
            HeaderText: viewResources.No_Players, Width: 100, Name: "ColNumberOfPlayers", HeaderAlign: "center", CellAlign: "right",
            FormatNumber: FormatConstant.INT_NUMBER, Sort: true, SortDirection: reportManager.getSortDirection("NumberOfPlayers")
        });
        columnsHeader.push({
            HeaderText: "Bet Count", Width: 100, Name: "ColNumberOfGames", HeaderAlign: "center", CellAlign: "right",
            FormatNumber: FormatConstant.INT_NUMBER, Sort: true, SortDirection: reportManager.getSortDirection("NumberOfGames")
        });
        columnsHeader.push({
            HeaderText: viewResources.Currency, Width: 100, Name: "ColCurrency", HeaderAlign: "center", CellAlign: "center",
            Sort: true, SortDirection: reportManager.getSortDirection("CurrencyName")
        });
        columnsHeader.push({
            HeaderText: viewResources.Total_bets, Width: 120, Name: "ColTotalBets", HeaderAlign: "center", CellAlign: "right",
            FormatNumber: FormatConstant.TWO_DECIMAL_NUMBER, Sort: true, SortDirection: reportManager.getSortDirection("TotalBets")
        });
        columnsHeader.push({
            HeaderText: viewResources.Total_wins, Width: 120, Name: "ColTotalWins", HeaderAlign: "center", CellAlign: "right",
            FormatNumber: FormatConstant.TWO_DECIMAL_NUMBER, Sort: true, SortDirection: reportManager.getSortDirection("TotalWins")
        });
        columnsHeader.push({
            HeaderText: viewResources.Game_income, Width: 120, Name: "ColGameIncome", FormatNumber: FormatConstant.TWO_DECIMAL_NUMBER,
            HeaderAlign: "center", CellAlign: "right", FormatNegative: '$', Sort: true, SortDirection: reportManager.getSortDirection("GameIncome")
        });
        columnsHeader.push({
            HeaderText: viewResources.Game_payout, Width: 120, Name: "ColGamePayout", FormatNumber: FormatConstant.TWO_DECIMAL_PERCENT_NUMBER,
            HeaderAlign: "center", CellAlign: "right", FormatNegative: '$', Sort: true, SortDirection: reportManager.getSortDirection("GamePayout")
        });
        if (reportManager.Params.reportParams.ShowRefundColumn) {
            columnsHeader.push({
                HeaderText: viewResources.Total_refund, Width: 120, Name: "ColTotalRefund", FormatNumber: FormatConstant.TWO_DECIMAL_NUMBER,
                HeaderAlign: "center", CellAlign: "right", FormatNegative: '$', Sort: true, SortDirection: reportManager.getSortDirection("TotalRefund")
            });
        }

        columnsHeader.push({
            HeaderText: viewResources.Company_Turnover, Width: 140, Name: "ColCompanyTurnover", FormatNumber: FormatConstant.TWO_DECIMAL_NUMBER,
            HeaderAlign: "center", CellAlign: "right", Sort: true, SortDirection: reportManager.getSortDirection("CompanyTurnover")
        });
        columnsHeader.push({
            HeaderText: viewResources.Company_comm, Width: 120, Name: "CompanyCommission", FormatNumber: FormatConstant.TWO_DECIMAL_NUMBER,
            HeaderAlign: "center", CellAlign: "right", FormatNegative: '$', Sort: true, SortDirection: reportManager.getSortDirection("CompanyCommission")
        });
        columnsHeader.push({
            HeaderText: viewResources.Company_net_income, Width: 160, Name: "ColCompanyNetIncome", FormatNumber: FormatConstant.TWO_DECIMAL_NUMBER,
            HeaderAlign: "center", CellAlign: "right", FormatNegative: '$', Sort: true, SortDirection: reportManager.getSortDirection("CompanyNetIncome")
        });

        var title = AccReport.utility.StringFormat(viewResources.CasinoGameStatistic_ReportTitle, [reportManager.Params.reportParams.Product, reportManager.Params.reportParams.FromDate, reportManager.Params.reportParams.ToDate]);
        var gridOptions = {
            columns: columnsHeader,
            showTitle: true,
            showFooter: true,
            titleInfo: {
                text: title,
                showExcelButton: true,
                exportExcelFunction: function () {
                    self.exportExcel();
                }
            },
            bodyRows: bodyData,
            footerRows: footerData,
            numberOfFrozenColumn: 0,
            gridWidth: 1400,
            gridExpandHeight: 355,
            fullWidth: true,
            showDistinctRow: true,
            showHighlightRow: false,
            headerColumnHeight: 29,
            showPager: true,
            customPager: true,
            sortOption:
            {
                serverSortFunction: function (sortColumnName, sortDirection) {
                    var sortInfo = reportManager.MapSortInfo(sortColumnName);
                    reportManager.CachedData.sortColName = sortInfo.sortColumnName;
                    reportManager.CachedData.sortDirection = sortDirection;
                    reportManager.CachedData.sortDataType = sortInfo.sortDataType;
                    reportManager.SortReportData();
                }
            },
            pagerOption: {
                itemsPerPage: reportManager.CachedData.itemsPerPage,
                currentPage: reportManager.CachedData.currentPage,
                itemsPerPageArray: reportManager.CachedData.itemsPerPageArray,
                totalItem: gameTypeLength,
                pagerSelectCallBack: function (page_index, itemsPerPage, jq) {
                    reportManager.CachedData.itemsPerPage = itemsPerPage;
                    reportManager.CachedData.currentPage = page_index;
                    reportManager.RenderReport(reportManager.CachedData.currentReport);
                }
            }
        };

        return gridOptions;
    };

    self.AddChildTotalToBodyData = function (columns, currentChildTotal, bodyData) {
        columns = [];
        columns.push({ Name: "ColName", Value: currentChildTotal.Name, TdClass: "subTotal bold" });
        columns.push({ Name: "ColNumberOfPlayers", Value: currentChildTotal.NumberOfPlayers, TdClass: "subTotal bold" });
        columns.push({ Name: "ColNumberOfGames", Value: currentChildTotal.NumberOfGames, TdClass: "subTotal bold" });
        columns.push({ Name: "ColCurrency", Value: currentChildTotal.CurrencyName, TdClass: "subTotal bold" });
        columns.push({ Name: "ColTotalBets", Value: currentChildTotal.TotalBets, TdClass: "subTotal bold" });
        columns.push({ Name: "ColTotalWins", Value: currentChildTotal.TotalWins, TdClass: "subTotal bold" });
        columns.push({ Name: "ColGameIncome", Value: currentChildTotal.GameIncome, TdClass: "subTotal bold" });
        columns.push({ Name: "ColGamePayout", Value: currentChildTotal.GamePayout, TdClass: "subTotal bold" });
        if (reportManager.Params.reportParams.ShowRefundColumn) {
            columns.push({ Name: "ColTotalRefund", Value: currentChildTotal.TotalRefund, TdClass: "subTotal bold" });
        }

        columns.push({ Name: "ColCompanyTurnover", Value: currentChildTotal.CompanyTurnover, TdClass: "subTotal bold" });
        columns.push({ Name: "CompanyCommission", Value: currentChildTotal.CompanyCommission, TdClass: "subTotal bold" });
        columns.push({ Name: "ColCompanyNetIncome", Value: currentChildTotal.CompanyNetIncome, TdClass: "subTotal bold" });
        bodyData.push({
            Columns: columns
        });
    };

    self.exportExcel = function () {
        AccReport.disableInput();
        var exportParams = reportManager.Params.globalVariable.CacheParam.GameType;
        exportParams.ViewReport = reportManager.Params.globalVariable.viewReport.GameType;
        exportParams.IsExportGrandTotal = true;
        AccReport.downloadFile(window.rootUrl + "Gaming/CasinoGameStatistic/ExportReport", exportParams);
        AccReport.enableInput();
    };
};