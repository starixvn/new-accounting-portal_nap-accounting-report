Namespace.Register("Fanex.AccountingReports.Gaming.LiveCasinoAgencyWinLoss");
Fanex.AccountingReports.Gaming.LiveCasinoAgencyWinLoss.ReportByUserName = function () {
    var self = this;
    self.GetGridOptions = function (report) {
        var bodyData = [];
        var footerData = [];
        if (report && report.ChildsInfo && report.ChildsInfo.length > 0) {
            var reportLength = report.ChildsInfo.length;
            var info;
            var columns = [];
            var indexRange = reportManager.GetPagingIndexRange(reportLength);
            for (var i = indexRange.startIndex; i < indexRange.endIndex; i++) {
                info = report.ChildsInfo[i];
                var downlinesWinLoss = reportManager.GetDownlinesWinLoss(info);
                columns = [];
                columns.push({ Name: "ColName", Value: info.Name });
                columns.push({ Name: "ColCurrency", Value: info.CurrencyName });
                columns.push({ Name: "ColRollingTurnover", Value: info.RollingTurnover });
                columns.push({ Name: "ColCompanyTurnover", Value: downlinesWinLoss.companyWinLoss.Turnover });
                columns.push({ Name: "ColGrossCommission", Value: info.GrossCommission });
                columns.push({ Name: "ColTotalBets", Value: info.TotalBets });

                columns.push({ Name: "ColMemberWinLoss", Value: downlinesWinLoss.memberWinLoss.WinLoss });
                columns.push({ Name: "ColMemberCommission", Value: downlinesWinLoss.memberWinLoss.Commission });
                columns.push({ Name: "ColMemberTotal", Value: downlinesWinLoss.memberWinLoss.RealWinLoss });

                columns.push({ Name: "ColAgentWinLoss", Value: downlinesWinLoss.agentWinLoss.WinLoss });
                columns.push({ Name: "ColAgentCommission", Value: downlinesWinLoss.agentWinLoss.Commission });
                columns.push({ Name: "ColAgentTotal", Value: downlinesWinLoss.agentWinLoss.RealWinLoss });

                columns.push({ Name: "ColMasterWinLoss", Value: downlinesWinLoss.masterWinLoss.WinLoss });
                columns.push({ Name: "ColMasterCommission", Value: downlinesWinLoss.masterWinLoss.Commission });
                columns.push({ Name: "ColMasterTotal", Value: downlinesWinLoss.masterWinLoss.RealWinLoss });

                columns.push({ Name: "ColSuperWinLoss", Value: downlinesWinLoss.superWinLoss.WinLoss });
                columns.push({ Name: "ColSuperCommission", Value: downlinesWinLoss.superWinLoss.Commission });
                columns.push({ Name: "ColSuperTotal", Value: downlinesWinLoss.superWinLoss.RealWinLoss });

                columns.push({ Name: "ColCompanyWinLoss", Value: downlinesWinLoss.companyWinLoss.WinLoss });
                bodyData.push({
                    Columns: columns
                });
            }

            var reportChildTotalLength = report.GrandTotal.ChildsInfo.length;
            for (var i = 0; i < reportChildTotalLength; i++) {
                info = report.GrandTotal.ChildsInfo[i];
                var downlinesWinLoss = reportManager.GetDownlinesWinLoss(info);
                reportManager.AddChildTotalToBodyData(columns, info, downlinesWinLoss, bodyData);
            }

            columns = [];
            info = report.GrandTotal.GrandTotal;
            var downlinesWinLoss = reportManager.GetDownlinesWinLoss(info);
            columns.push({ Name: "ColName", Value: info.Name, MergeColNumber: 2 });
            columns.push({ Name: "ColRollingTurnover", Value: info.RollingTurnover });
            columns.push({ Name: "ColCompanyTurnover", Value: downlinesWinLoss.companyWinLoss.Turnover });
            columns.push({ Name: "ColGrossCommission", Value: info.GrossCommission });
            columns.push({ Name: "ColTotalBets", Value: info.TotalBets });

            columns.push({ Name: "ColMemberWinLoss", Value: downlinesWinLoss.memberWinLoss.WinLoss });
            columns.push({ Name: "ColMemberCommission", Value: downlinesWinLoss.memberWinLoss.Commission });
            columns.push({ Name: "ColMemberTotal", Value: downlinesWinLoss.memberWinLoss.RealWinLoss });

            columns.push({ Name: "ColAgentWinLoss", Value: downlinesWinLoss.agentWinLoss.WinLoss });
            columns.push({ Name: "ColAgentCommission", Value: downlinesWinLoss.agentWinLoss.Commission });
            columns.push({ Name: "ColAgentTotal", Value: downlinesWinLoss.agentWinLoss.RealWinLoss });

            columns.push({ Name: "ColMasterWinLoss", Value: downlinesWinLoss.masterWinLoss.WinLoss });
            columns.push({ Name: "ColMasterCommission", Value: downlinesWinLoss.masterWinLoss.Commission });
            columns.push({ Name: "ColMasterTotal", Value: downlinesWinLoss.masterWinLoss.RealWinLoss });

            columns.push({ Name: "ColSuperWinLoss", Value: downlinesWinLoss.superWinLoss.WinLoss });
            columns.push({ Name: "ColSuperCommission", Value: downlinesWinLoss.superWinLoss.Commission });
            columns.push({ Name: "ColSuperTotal", Value: downlinesWinLoss.superWinLoss.RealWinLoss });

            columns.push({ Name: "ColCompanyWinLoss", Value: downlinesWinLoss.companyWinLoss.WinLoss });
            footerData.push({
                Columns: columns
            });
        }

        var gridOptions = {
            customizeHeader: true,
            groupHeaderRows: [{
                Columns: [
                    { HeaderText: viewResources.UserName, Name: "ColName", MergeRowNumber: 2 },
                    { HeaderText: viewResources.Currency, Name: "ColCurrency", MergeRowNumber: 2 },
                    { HeaderText: viewResources.Total_bets, Name: "ColRollingTurnover", MergeRowNumber: 2 },
                    { HeaderText: viewResources.Company_Turnover, Name: "ColCompanyTurnover", MergeRowNumber: 2 },
                    { HeaderText: viewResources.Gross_Commission, Name: "ColGrossCommission", MergeRowNumber: 2 },
                    { HeaderText: viewResources.Symbol_TotalBets, Name: "ColTotalBets", MergeRowNumber: 2 },
                    { HeaderText: viewResources.Member, Name: "ColMemberWinLoss", MergeColNumber: 3, HeaderAlign: "center" },
                    { HeaderText: viewResources.Agent, Name: "ColAgentWinLoss", MergeColNumber: 3, HeaderAlign: "center" },
                    { HeaderText: viewResources.Master, Name: "ColMasterWinLoss", MergeColNumber: 3, HeaderAlign: "center" },
                    { HeaderText: viewResources.Super, Name: "ColSuperWinLoss", MergeColNumber: 3, HeaderAlign: "center" },
                    { HeaderText: viewResources.Company, Name: "ColCompanyWinLoss", MergeRowNumber: 2}]
            }],
            columns: [
                { HeaderText: viewResources.UserName, Width: 120, Name: "ColName", HeaderAlign: "center", CellAlign: "center",
                    Sort: true, SortDirection: reportManager.getSortDirection("Name")
                },
                { HeaderText: viewResources.Currency, Width: 80, Name: "ColCurrency", HeaderAlign: "center", CellAlign: "center" },
                { HeaderText: viewResources.Total_bets, Width: 120, Name: "ColRollingTurnover", HeaderAlign: "center", CellAlign: "right",
                    FormatNumber: FormatConstant.TWO_DECIMAL_NUMBER, Sort: true, SortDirection: reportManager.getSortDirection("RollingTurnover")
                },
                { HeaderText: viewResources.Company_Turnover, Width: 120, Name: "ColCompanyTurnover", HeaderAlign: "center", CellAlign: "right", FormatNumber: FormatConstant.TWO_DECIMAL_NUMBER },
                { HeaderText: viewResources.Gross_Commission, Width: 120, Name: "ColGrossCommission", HeaderAlign: "center", CellAlign: "right", FormatNumber: FormatConstant.TWO_DECIMAL_NUMBER },
                { HeaderText: viewResources.Symbol_TotalBets, Width: 80, Name: "ColTotalBets", HeaderAlign: "center", CellAlign: "right", FormatNumber: FormatConstant.INT_NUMBER },

                { HeaderText: viewResources.Win_Loss, Width: 120, Name: "ColMemberWinLoss", HeaderAlign: "center", CellAlign: "right", FormatNumber: FormatConstant.TWO_DECIMAL_NUMBER },
                { HeaderText: viewResources.Commission, Width: 100, Name: "ColMemberCommission", HeaderAlign: "center", CellAlign: "right", FormatNumber: FormatConstant.TWO_DECIMAL_NUMBER },
                { HeaderText: viewResources.Total, Width: 120, Name: "ColMemberTotal", HeaderAlign: "center", CellAlign: "right",
                    FormatNumber: FormatConstant.TWO_DECIMAL_NUMBER, Sort: true, SortDirection: reportManager.getSortDirection("MemberTotal")
                },

                { HeaderText: viewResources.Win_Loss, Width: 120, Name: "ColAgentWinLoss", HeaderAlign: "center", CellAlign: "right", FormatNumber: FormatConstant.TWO_DECIMAL_NUMBER },
                { HeaderText: viewResources.Commission, Width: 100, Name: "ColAgentCommission", HeaderAlign: "center", CellAlign: "right", FormatNumber: FormatConstant.TWO_DECIMAL_NUMBER },
                { HeaderText: viewResources.Total, Width: 120, Name: "ColAgentTotal", HeaderAlign: "center", CellAlign: "right",
                    FormatNumber: FormatConstant.TWO_DECIMAL_NUMBER, Sort: true, SortDirection: reportManager.getSortDirection("AgentTotal")
                },

                { HeaderText: viewResources.Win_Loss, Width: 120, Name: "ColMasterWinLoss", HeaderAlign: "center", CellAlign: "right", FormatNumber: FormatConstant.TWO_DECIMAL_NUMBER },
                { HeaderText: viewResources.Commission, Width: 100, Name: "ColMasterCommission", HeaderAlign: "center", CellAlign: "right", FormatNumber: FormatConstant.TWO_DECIMAL_NUMBER },
                { HeaderText: viewResources.Total, Width: 120, Name: "ColMasterTotal", HeaderAlign: "center", CellAlign: "right",
                    FormatNumber: FormatConstant.TWO_DECIMAL_NUMBER, Sort: true, SortDirection: reportManager.getSortDirection("MasterTotal")
                },

                { HeaderText: viewResources.Win_Loss, Width: 120, Name: "ColSuperWinLoss", HeaderAlign: "center", CellAlign: "right", FormatNumber: FormatConstant.TWO_DECIMAL_NUMBER },
                { HeaderText: viewResources.Commission, Width: 100, Name: "ColSuperCommission", HeaderAlign: "center", CellAlign: "right", FormatNumber: FormatConstant.TWO_DECIMAL_NUMBER },
                { HeaderText: viewResources.Total, Width: 120, Name: "ColSuperTotal", HeaderAlign: "center", CellAlign: "right",
                    FormatNumber: FormatConstant.TWO_DECIMAL_NUMBER, Sort: true, SortDirection: reportManager.getSortDirection("SuperTotal")
                },

                { HeaderText: viewResources.Company, Width: 120, Name: "ColCompanyWinLoss", HeaderAlign: "center", CellAlign: "right",
                    FormatNumber: FormatConstant.TWO_DECIMAL_NUMBER, Sort: true, SortDirection: reportManager.getSortDirection("CompanyWinLoss"), FormatNegative: '$'
                }],
            showTitle: true,
            showFooter: true,
            titleInfo: {
                text: AccReport.utility.StringFormat(viewResources.LiveCasinoAgencyWinLoss_ReportTitle,
                    [reportManager.Params.reportParams.FromDate, reportManager.Params.reportParams.ToDate]),
                showExcelButton: true,
                exportExcelFunction: function () {
                    self.ExportReport();
                }
            },
            bodyRows: bodyData,
            footerRows: footerData,
            numberOfFrozenColumn: 6,
            gridWidth: 1420,
            gridExpandHeight: 300,
            fullWidth: true,
            showDistinctRow: false,
            showHighlightRow: false,
            showPager: true,
            customPager: true,
            sortOption:
            {
                serverSortFunction: function (sortColumnName, sortDirection) {
                    var sortInfo = reportManager.MapSortInfo(sortColumnName);
                    reportManager.CachedData.sortColName = sortInfo.sortColumnName;
                    reportManager.CachedData.sortDirection = sortDirection;
                    reportManager.CachedData.sortDataType = sortInfo.sortDataType;

                    reportManager.Params.reportParams.SortDirection = sortDirection;
                    reportManager.Params.reportParams.SortColumnName = sortInfo.sortColumnName;
                    reportManager.Params.reportParams.SortDataType = sortInfo.sortDataType;
                    reportManager.SortReportData(reportManager.Params.reportParams);
                }
            },
            pagerOption: {
                itemsPerPage: reportManager.CachedData.itemsPerPage,
                currentPage: reportManager.CachedData.currentPage,
                itemsPerPageArray: reportManager.CachedData.itemsPerPageArray,
                totalItem: reportLength,
                pagerSelectCallBack: function (page_index, itemsPerPage, jq) {
                    reportManager.CachedData.itemsPerPage = itemsPerPage;
                    reportManager.CachedData.currentPage = page_index;
                    reportManager.RenderReport(reportManager.CachedData.currentReport);
                }
            }
        };

        return gridOptions;
    };

    self.ExportReport = function () {
        AccReport.downloadFile(window.rootUrl + "Gaming/LiveCasinoAgencyWinLoss/ExportReport", reportManager.Params.reportParams);
    };
};