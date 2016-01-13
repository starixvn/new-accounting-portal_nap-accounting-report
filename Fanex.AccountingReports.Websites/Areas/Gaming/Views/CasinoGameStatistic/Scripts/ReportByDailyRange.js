Namespace.Register("Fanex.AccountingReports.Gaming.LiveCasinoGameStatistic");
Fanex.AccountingReports.Gaming.LiveCasinoGameStatistic.ReportByDailyRange = function () {
    var self = this;

    self.GetGridOptions = function (report) {
        var isDrillDown = reportManager.Params.globalVariable.IsDrillDown;
        var reportType = reportManager.Params.globalVariable.PrevReportType;
        var isBaseRM = reportManager.GetReportParams().Currency === "-1";
        var classPayout = "";

        var bodyData = [];
        var footerData = [];
        var columns = [];
        var timeRangeLength = report && report.ChildsInfo ?
            report.ChildsInfo.length : 0;
        if (timeRangeLength > 0) {
            var info;
            var indexRange = reportManager.GetPagingIndexRange(timeRangeLength);
            for (var i = indexRange.startIndex; i < indexRange.endIndex; i++) {
                var timeRangeWinLoss = report.ChildsInfo[i];
                var reportLength = timeRangeWinLoss.ChildsInfo.length;
                for (var j = 0; j < reportLength; j++) {
                    columns = [];
                    info = timeRangeWinLoss.ChildsInfo[j];
                    if (j === 0) {
                        columns.push({ Name: "ColName",
                            Value: info, CustomCellRender: function (currentCell) {
                                var clickEvent = ' onclick = "reportManager.GetHourlyReport(\'' + currentCell.Value.Name + '\''
                                                                                + ',\'' + currentCell.Value.CurrencyId
                                                                                + "');\"";

                                var result = '<div>' + currentCell.Value.Name + '</div>';
                                if (isBaseRM) {
                                    result = '<div ' + clickEvent + ' class="link">' + currentCell.Value.Name + '</div>';
                                }

                                return result;
                            },
                            MergeRowNumber: reportLength
                        });
                    }

                    columns.push({ Name: "ColNumberOfPlayers", Value: info.NumberOfPlayers });
                    columns.push({ Name: "ColNumberOfGames", Value: info.NumberOfGames });
                    columns.push({ Name: "ColCurrency", Value: info, CustomCellRender: function (currentCell) {
                        var clickEvent = ' onclick = "reportManager.GetReportDailyByCurrency(\'' + currentCell.Value.Name + '\''
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

                    columns.push({ Name: "ColGamePayout", Value: info.GamePayout, TdClass: classPayout });
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

            var reportLength = report.GrandTotal.ChildsInfo.length;

            var footerColumns = [];
            info = report.GrandTotal.ChildsInfo[reportLength - 1];
            footerColumns.push({ Name: "ColName", Value: info, CustomCellRender: function (currentCell) {
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
                classPayout = "red";
            }
            else {
                classPayout = "";
            }

            footerColumns.push({ Name: "ColGamePayout", Value: info.GamePayout, TdClass: classPayout });
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
            HeaderText: viewResources.Date, Width: 160, Name: "ColName", HeaderAlign: "center", CellAlign: "center",
            Sort: true, SortDirection: reportManager.getSortDirection("Name")
        });
        columnsHeader.push({
            HeaderText: viewResources.No_Players, Width: 100, Name: "ColNumberOfPlayers", HeaderAlign: "center", CellAlign: "right",
            FormatNumber: FormatConstant.INT_NUMBER, Sort: true, SortDirection: reportManager.getSortDirection("NumberOfPlayers")
        });
        columnsHeader.push({
            HeaderText: viewResources.Bet_Count, Width: 100, Name: "ColNumberOfGames", HeaderAlign: "center", CellAlign: "right",
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
        var showBackButton = false;
        if (isDrillDown) {
            title = "<span class='link' onclick='reportManager.RenderParentReport(" + reportType + ");'>Back</span> < " + AccReport.utility.StringFormat(viewResources.CasinoGameStatistic_ReportTitle, [reportManager.Params.reportParams.Product, reportManager.Params.reportParams.FromDate, reportManager.Params.reportParams.ToDate]);

            var username = reportManager.Params.reportParams.UserName;
            if (username != "") {
                title = "<span class='link' onclick='reportManager.RenderParentReport(" + reportType + ");'>Back</span> < " + AccReport.utility.StringFormat(viewResources.CasinoGameStatistic_UserName_ReportTitle, [reportManager.Params.reportParams.Product, username, reportManager.Params.reportParams.FromDate, reportManager.Params.reportParams.ToDate]);
            }
            showBackButton = true;
        }

        if (reportManager.Params.globalVariable.showBackButton) {
            showBackButton = true;
        }

        var gridOptions = {
            columns: columnsHeader,
            showTitle: true,
            showFooter: true,
            titleInfo: {
                text: title,
                showExcelButton: true,
                exportExcelFunction: function () {
                    self.exportExcel();
                },
                showBackButton: false
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
                    var sortInfo = reportManager.MapSortInfoByTime(sortColumnName);
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
                totalItem: timeRangeLength,
                pagerSelectCallBack: function (page_index, itemsPerPage, jq) {
                    reportManager.CachedData.itemsPerPage = itemsPerPage;
                    reportManager.CachedData.currentPage = page_index;
                    reportManager.RenderReport(reportManager.CachedData.currentReport);
                }
            }
        };

        return gridOptions;
    };

    self.exportExcel = function () {
        AccReport.disableInput();
        var exportParams = reportManager.Params.globalVariable.CacheParam.Daily;
        exportParams.ViewReport = reportManager.Params.globalVariable.viewReport.Daily;
        exportParams.IsExportGrandTotal = true;
        AccReport.downloadFile(window.rootUrl + "Gaming/CasinoGameStatistic/ExportReport", exportParams);
        AccReport.enableInput();
    };
};