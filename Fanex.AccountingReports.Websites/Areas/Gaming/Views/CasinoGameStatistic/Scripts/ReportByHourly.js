Namespace.Register("Fanex.AccountingReports.Gaming.LiveCasinoGameStatistic");
Fanex.AccountingReports.Gaming.LiveCasinoGameStatistic.ReportByHourly = function () {
    var self = this;

    self.GetGridOptions = function (report, isAccumulated) {
        var bodyData = [];
        var footerData = [];
        var columns = [];
        var info;
        var param = reportManager.Params.globalVariable.CacheParam.Hourly;
        var date = param.FromDate;
        var product = param.Product;
        var reportLength = report && report.DataAccumulatedList ? report.DataAccumulatedList.length : 0;
        var classTotalBet = "";
        var classTotalWin = "";
        var classGamePayout = "";
        var classGameInCome = "";
        var classGameComm = "";
        var classCompanyNetIncome = "";        

        if (reportLength > 0) {

            for (var i = 0; i < reportLength; i++) {
                if (isAccumulated) {
                    info = report.DataAccumulatedList[i];
                } else {
                    info = report.DataOnEachHourList[i];
                }

                if (parseFloat(info.TotalBets) < 0) {
                    classTotalBet = " red";
                } else {
                    classTotalBet = "";
                }

                if (parseFloat(info.TotalWins) < 0) {
                    classTotalWin = " red";
                } else {
                    classTotalWin = "";
                }

                if (parseFloat(info.GamePayout) >= 1.05) {
                    classGamePayout = " hourly-column-gamne red";
                }
                else if (parseFloat(info.GamePayout) > 1) {
                    classGamePayout = " red";
                }
                else {
                    classGamePayout = "";
                }

                if (parseFloat(info.GameIncome) <= -100000) {
                    classGameInCome = " hourly-column-gamne";
                } else {
                    classGameInCome = "";
                }

                if (parseFloat(info.CompanyComm) < 0) {
                    classGameComm = " red";
                } else {
                    classGameComm = "";
                }

                if (parseFloat(info.CompanyNetInCome) < 0) {
                    classCompanyNetIncome = " red";
                } else {
                    classCompanyNetIncome = "";
                }

                columns = [];

                columns.push({ Name: "ColName", Value: info.Name, TdClass: "hourly-column-time" });
                columns.push({ Name: "ColOnlinePlayers", Value: info.OnlinePlayers });
                columns.push({ Name: "ColNumberOfGames", Value: info.NumberOfGames });
                columns.push({ Name: "ColCurrency", Value: info.Currency });
                columns.push({ Name: "ColTotalBets", Value: info.TotalBets, TdClass: classTotalBet });
                columns.push({ Name: "ColTotalWins", Value: info.TotalWins, TdClass: classTotalWin });
                columns.push({ Name: "ColGameIncome", Value: info.GameIncome, TdClass: classGameInCome });
                columns.push({ Name: "ColGamePayout", Value: info.GamePayout, TdClass: classGamePayout });
                if (reportManager.Params.reportParams.ShowRefundColumn) {
                    columns.push({ Name: "ColTotalRefund", Value: info.TotalRefund });
                }

                columns.push({ Name: "ColCompanyTO", Value: info.CompanyTO });
                columns.push({ Name: "ColCompanyComm", Value: info.CompanyComm, TdClass: classGameComm });
                columns.push({ Name: "ColCompanyNetIncome", Value: info.CompanyNetInCome, TdClass: classCompanyNetIncome });

                bodyData.push({
                    Columns: columns
                });
            }

            if (isAccumulated) {
                var footerColumns = [];
                info = report;
                footerColumns.push({ Name: "ColName", Value: "Average", TdClass: "colNote" });
                footerColumns.push({ Name: "ColOnlinePlayers", Value: info.OnlinePlayers, TdClass: "colNote" });
                footerColumns.push({ Name: "ColNumberOfGames", Value: "", TdClass: "colNote", MergeColNumber: reportManager.Params.reportParams.ShowRefundColumn ? 10 : 9 });
                footerData.push({ Columns: footerColumns });
            }
        }

        var columnsHeader = [];
        columnsHeader.push({
            HeaderText: "Time", Width: 60, Name: "ColName", HeaderAlign: "center", CellAlign: "right"
        });
        columnsHeader.push({
            HeaderText: "Online Players", Width: 100, Name: "ColOnlinePlayers", HeaderAlign: "center", CellAlign: "right"
        });
        columnsHeader.push({
            HeaderText: viewResources.Bet_Count, Width: 80, Name: "ColNumberOfGames", HeaderAlign: "center", CellAlign: "right",
            FormatNumber: FormatConstant.INT_NUMBER
        });
        columnsHeader.push({
            HeaderText: viewResources.Currency, Width: 80, Name: "ColCurrency", HeaderAlign: "center", CellAlign: "center"
        });
        columnsHeader.push({
            HeaderText: viewResources.Total_bets, Width: 100, Name: "ColTotalBets", HeaderAlign: "center", CellAlign: "right",
            FormatNumber: FormatConstant.TWO_DECIMAL_NUMBER, FormatNegative: '$'
        });
        columnsHeader.push({
            HeaderText: viewResources.Total_wins, Width: 100, Name: "ColTotalWins", HeaderAlign: "center", CellAlign: "right",
            FormatNumber: FormatConstant.TWO_DECIMAL_NUMBER, FormatNegative: '$'
        });
        columnsHeader.push({
            HeaderText: viewResources.Game_income, Width: 100, Name: "ColGameIncome", FormatNumber: FormatConstant.TWO_DECIMAL_NUMBER,
            HeaderAlign: "center", CellAlign: "right", FormatNegative: '$'
        });
        columnsHeader.push({
            HeaderText: viewResources.Game_payout, Width: 100, Name: "ColGamePayout", FormatNumber: FormatConstant.TWO_DECIMAL_PERCENT_NUMBER,
            HeaderAlign: "center", CellAlign: "right", FormatNegative: '$'
        });
        if (reportManager.Params.reportParams.ShowRefundColumn) {
            columnsHeader.push({
                HeaderText: viewResources.Total_refund, Width: 100, Name: "ColTotalRefund", FormatNumber: FormatConstant.TWO_DECIMAL_NUMBER,
                HeaderAlign: "center", CellAlign: "right", FormatNegative: '$'
            });
        }

        columnsHeader.push({
            HeaderText: viewResources.Company_Turnover, Width: 120, Name: "ColCompanyTO", FormatNumber: FormatConstant.TWO_DECIMAL_NUMBER,
            HeaderAlign: "center", CellAlign: "right"
        });
        columnsHeader.push({
            HeaderText: viewResources.Company_comm, Width: 100, Name: "ColCompanyComm", FormatNumber: FormatConstant.TWO_DECIMAL_NUMBER,
            HeaderAlign: "center", CellAlign: "right", FormatNegative: '$'
        });
        columnsHeader.push({
            HeaderText: viewResources.Company_net_income, Width: 140, Name: "ColCompanyNetIncome", FormatNumber: FormatConstant.TWO_DECIMAL_NUMBER,
            HeaderAlign: "center", CellAlign: "right", FormatNegative: '$'
        });

        var title = "";
        var strUserName = "For " + param.UserName;
        if (param.UserName == '') {
            strUserName = "";
        }
        title = "Hourly " + product + " Game Statistic Record {0} on " + date + " (GMT+8)";

        var reportType = reportManager.Params.globalVariable.PrevReportType;
        if (isAccumulated) {
            title = "<span class='link' onclick='reportManager.RenderParentReport(" + reportType + ");'>Back</span> < " + AccReport.utility.StringFormat(viewResources.CasinoGameStatistic_Accumulated_Title, [product, strUserName, date]);
        } else {
            title = AccReport.utility.StringFormat(viewResources.CasinoGameStatistic_Hourly_Title, [product, strUserName, date]);
        }

        var gridOptions = {
            columns: columnsHeader,
            showTitle: true,
            showFooter: isAccumulated,
            titleInfo: {
                text: title,
                showExcelButton: isAccumulated,
                exportExcelFunction: function () {
                    self.exportExcel();
                },
                showBackButton: false
            },
            bodyRows: bodyData,
            footerRows: footerData,
            numberOfFrozenColumn: 1,
            gridWidth: 1400,
            gridExpandHeight: 200,
            gridHeight: 270,
            fullWidth: true,
            fullHeight: false,
            showDistinctRow: true,
            showHighlightRow: false,
            headerColumnHeight: 29,
            showPager: false,
            customPager: false
        };

        return gridOptions;
    };

    self.exportExcel = function () {
        AccReport.disableInput();
        var exportParams = reportManager.Params.globalVariable.CacheParam.Hourly;
        exportParams.ViewReport = reportManager.Params.globalVariable.viewReport.Hourly;
        exportParams.IsExportGrandTotal = false;
        AccReport.downloadFile(window.rootUrl + "Gaming/CasinoGameStatistic/ExportHourly", exportParams);
        AccReport.enableInput();
    }
};