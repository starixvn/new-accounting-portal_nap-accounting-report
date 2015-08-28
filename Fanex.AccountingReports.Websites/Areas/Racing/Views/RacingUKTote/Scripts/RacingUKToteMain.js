/* File Created: December 20, 2013 */
Namespace.Register("Fanex.AccountingReports.Racing.RacingUKTote.RacingUKToteMain");
Fanex.AccountingReports.Racing.RacingUKTote.RacingUKToteMain = function () {
    var self = this;
    var _tables = {
        btnGenerate: null,
        datepickerplugin: null,
        reportContainer: null,
        report: null,
        netIncomeFormula: null,
        amountDueFormula: null
    };
    self.Params = {
        reportParams: {
            winlostDate: null,
            reportType: 1
        }
    };
    var renderDatePicker = function () {
        $('#datepickerplugin').nexDatePicker({
            nexDatePickerType: "single",
            firstDatePicker: {
                Id: "datePickerInput",
                Label: labelDate,
                InitDate: toDay,
                LabelCss: "lbl-field col_2",
                TextboxCss: "edit-field col_5"
            },

            datePickerFormat: {
                dateFormat: FormatConstant.M_DD_YY_STRING,
                changeMonth: true,
                changeYear: true,
                minDate: new Date(minDate),
                maxDate: new Date(maxDate),
                showOtherMonths: true,
                selectOtherMonths: true
            }
        });

        _tables.datepickerplugin = $("#datepickerplugin");
    };

    self.initPage = function () {
        cachedDOMElements();
        renderDatePicker();
        bindingEvent();
    };

    var cachedDOMElements = function () {
        _tables.btnGenerate = $("#btnGenerate");
        _tables.reportContainer = $("#racingDailyWinLossReportContainer");
        _tables.report = $("#racingDailyWinLossReport");
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

    var bindingEvent = function () {
        _tables.btnGenerate.click(function () {
            self.btnGenerateClick();
        });
    };

    self.btnGenerateClick = function () {
        AccReport.disableInput();
        ResetLayout();
        getParams();
        $.ajax({
            type: "POST",
            url: window.rootUrl + "Racing/RacingUKTote/GetRacingDailyWinLoss",
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            data: JSON.stringify(self.Params.reportParams),
            beforeSend: function () {
                AccReport.disableInput();
            },
            success: function (rs) {
                renderRacingWinLoss(rs);
            },
            error: function (event, jqXHR, ajaxSettings) {
                renderRacingWinLoss([]);
            },
            complete: function () {
                _tables.amountDueFormula = $("#amountDueFormula");
                _tables.netIncomeFormula = $("#netIncomeFormula");
                renderFilterNote(_tables.netIncomeFormula, netIncomeFormula);
                renderFilterNote(_tables.amountDueFormula, amountDueFormula);
                AccReport.enableInput();
            }
        });
    };
    var getParams = function () {
        self.Params.reportParams.winlostDate = _tables.datepickerplugin.nexDatePicker("getSingleDate");
    };

    var renderRacingWinLoss = function (report) {
        _tables.reportContainer.removeClass("display-none");
        _tables.report.addClass("display-none");
        var bodyData = [];
        var footerData = [];
        if (report && report.ChildsInfo && report.ChildsInfo.length > 0) {
            var partnerGroupNumber = report.ChildsInfo.length;
            for (var i = 0; i < partnerGroupNumber; i++) {
                var partnerGroup = report.ChildsInfo[i];
                var partnerGroupLength = partnerGroup.ChildsInfo.length;

                for (var j = 0; j < partnerGroupLength; j++) {
                    var reportItem = partnerGroup.ChildsInfo[j];
                    var currentColumns = [];
                    if (j == 0) {
                        currentColumns.push({ Name: "ColPool", Value: partnerGroup.PartnerName, MergeRowNumber: partnerGroupLength });
                    }
                    var currentRow = {
                        Columns: currentColumns.concat([
                            { Name: "ColCurrency", Value: reportItem.CurrencyName },
                            { Name: "ColWinTOAgency", Value: reportItem.WinTOAgency },
                            { Name: "ColPlaceTOAgency", Value: reportItem.PlaceTOAgency },
                            { Name: "ColWinPlaceTOAgency", Value: reportItem.WinPlaceTOAgency },
                            { Name: "ColTotalTOAgency", Value: reportItem.TotalTOAgency },
                            { Name: "ColCommIncomeAgency", Value: reportItem.CommIncomeAgency },
                            { Name: "ColPTWinLossAgency", Value: reportItem.PTWinLossAgency },
                            { Name: "ColWinTOIBC", Value: reportItem.WinTOIBC },
                            { Name: "ColPlaceTOIBC", Value: reportItem.PlaceTOIBC },
                            { Name: "ColWinPlaceTOIBC", Value: reportItem.WinPlaceTOIBC },
                            { Name: "ColTotalTOIBC", Value: reportItem.TotalTOIBC },
                            { Name: "ColCommIncomeIBC", Value: reportItem.CommIncomeIBC },
                            { Name: "ColToteFeeIBC", Value: reportItem.ToteFeeIBC },
                            { Name: "ColCommExpenseIBC", Value: reportItem.CommExpenseIBC },
                            { Name: "ColPTWinLossIBC", Value: reportItem.PTWinLossIBC },
                            { Name: "ColNetIncomeIBC", Value: reportItem.NetIncomeIBC },
                            { Name: "ColWinTOPartner", Value: reportItem.WinTOPartner },
                            { Name: "ColPlaceTOPartner", Value: reportItem.PlaceTOPartner },
                            { Name: "ColWinPlaceTOPartner", Value: reportItem.WinPlaceTOPartner },
                            { Name: "ColTotalTOPartner", Value: reportItem.TotalTOPartner },
                            { Name: "ColDividendWonPartner", Value: reportItem.DividendWonPartner },
                            { Name: "ColCommExpensePartner", Value: reportItem.CommExpensePartner },
                            { Name: "ColToteFeePartner", Value: reportItem.ToteFeePartner },
                            { Name: "ColAmountDuePartner", Value: reportItem.AmountDuePartner }
                        ])
                    };
                    bodyData.push(currentRow);
                }
                var partnerGroupTotalLength = partnerGroup.Total.ChildsInfo.length;
                for (var k = 0; k < partnerGroupTotalLength; k++) {
                    var TotalItem = partnerGroup.Total.ChildsInfo[k];
                    var currentColumns = [];
                    if (k == 0) {
                        currentColumns.push({ Name: "ColPool", Value: {
                            partnerId: partnerGroup.PartnerId,
                            poolCurrency: partnerGroup.PoolCurrency,
                            partnerName: partnerGroup.PartnerName
                        },
                            CustomCellRender: function (currentCell) {
                                var clickEvent = ' onclick = "viewRacingUKToteDetail(' + currentCell.Value.poolCurrency + ',' + currentCell.Value.partnerId + ',\'' + currentCell.Value.partnerName + "');\"";
                                var html = '<span class="link " ' + clickEvent + '>' + currentCell.Value.partnerName + " Total" + '</span>';
                                return html;
                            },
                            MergeRowNumber: partnerGroupTotalLength
                        });
                    }
                    var currentRow = {
                        Columns: currentColumns.concat([
                            { Name: "ColCurrency", Value: TotalItem.CurrencyName },
                            { Name: "ColWinTOAgency", Value: TotalItem.WinTOAgency },
                            { Name: "ColPlaceTOAgency", Value: TotalItem.PlaceTOAgency },
                            { Name: "ColWinPlaceTOAgency", Value: TotalItem.WinPlaceTOAgency },
                            { Name: "ColTotalTOAgency", Value: TotalItem.TotalTOAgency },
                            { Name: "ColCommIncomeAgency", Value: TotalItem.CommIncomeAgency },
                            { Name: "ColPTWinLossAgency", Value: TotalItem.PTWinLossAgency },
                            { Name: "ColWinTOIBC", Value: TotalItem.WinTOIBC },
                            { Name: "ColPlaceTOIBC", Value: TotalItem.PlaceTOIBC },
                            { Name: "ColWinPlaceTOIBC", Value: TotalItem.WinPlaceTOIBC },
                            { Name: "ColTotalTOIBC", Value: TotalItem.TotalTOIBC },
                            { Name: "ColCommIncomeIBC", Value: TotalItem.CommIncomeIBC },
                            { Name: "ColToteFeeIBC", Value: TotalItem.ToteFeeIBC },
                            { Name: "ColCommExpenseIBC", Value: TotalItem.CommExpenseIBC },
                            { Name: "ColPTWinLossIBC", Value: TotalItem.PTWinLossIBC },
                            { Name: "ColNetIncomeIBC", Value: TotalItem.NetIncomeIBC },
                            { Name: "ColWinTOPartner", Value: TotalItem.WinTOPartner },
                            { Name: "ColPlaceTOPartner", Value: TotalItem.PlaceTOPartner },
                            { Name: "ColWinPlaceTOPartner", Value: TotalItem.WinPlaceTOPartner },
                            { Name: "ColTotalTOPartner", Value: TotalItem.TotalTOPartner },
                            { Name: "ColDividendWonPartner", Value: TotalItem.DividendWonPartner },
                            { Name: "ColCommExpensePartner", Value: TotalItem.CommExpensePartner },
                            { Name: "ColToteFeePartner", Value: TotalItem.ToteFeePartner },
                            { Name: "ColAmountDuePartner", Value: TotalItem.AmountDuePartner }
                        ]),
                        CssClass: 'bold totalMain'
                    };
                    bodyData.push(currentRow);
                }
                bodyData.push({
                    Columns: [{ Name: "ColPool", Value: "", MergeColNumber: 2 },
                    { Name: "ColWinTOAgency", Value: "", MergeColNumber: 23}]
                });
            }
            var grandTotalLength = report.Total.ChildsInfo.length;
            for (var h = 0; h < grandTotalLength; h++) {
                var grandTotalItem = report.Total.ChildsInfo[h];
                var currentColumns = [];
                if (h == 0) {
                    currentColumns.push({ Name: "ColPool", Value: report.Total.PartnerName, MergeRowNumber: grandTotalLength });
                }
                var currentRow = {
                    Columns: currentColumns.concat([
                            { Name: "ColCurrency", Value: grandTotalItem.CurrencyName },
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
                            { Name: "ColAmountDuePartner", Value: grandTotalItem.AmountDuePartner }
                        ])
                };
                footerData.push(currentRow);
            }
        }

        var gridOptions = {
            customizeHeader: true,
            groupHeaderRows: [
                {
                    Columns: [{ HeaderText: "Pool - Currency", Name: "ColPool", MergeRowNumber: 2 },
                        { HeaderText: "Currency", Name: "ColCurrency", MergeRowNumber: 2 },
                        { HeaderText: "Agency", Name: "ColWinTOAgency", MergeColNumber: 6 },
                        { HeaderText: "IBC", Name: "ColWinTOIBC", MergeColNumber: 9 },
                        { HeaderText: "Partner", Name: "ColWinTOPartner", MergeColNumber: 8}]
                }
            ],
            columns: [{ HeaderText: "Pool - Currency", Width: 150, Name: "ColPool", HeaderAlign: "center", CellAlign: "center" },
                { HeaderText: "Currency", Width: 60, Name: "ColCurrency", HeaderAlign: "center", CellAlign: "center" },
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
                { HeaderText: "Net Income  <span id='netIncomeFormula' class='formular' title=''></span>", Width: 100, Name: "ColNetIncomeIBC", HeaderAlign: "center", CellAlign: "right", FormatNumber: FormatConstant.TWO_DECIMAL_NUMBER },
                { HeaderText: "Win TO", Width: 80, Name: "ColWinTOPartner", HeaderAlign: "center", CellAlign: "right", FormatNumber: FormatConstant.TWO_DECIMAL_NUMBER },
                { HeaderText: "Place TO", Width: 80, Name: "ColPlaceTOPartner", HeaderAlign: "center", CellAlign: "right", FormatNumber: FormatConstant.TWO_DECIMAL_NUMBER },
                { HeaderText: "W/P TO", Width: 80, Name: "ColWinPlaceTOPartner", HeaderAlign: "center", CellAlign: "right", FormatNumber: FormatConstant.TWO_DECIMAL_NUMBER },
                { HeaderText: "Total TO", Width: 80, Name: "ColTotalTOPartner", HeaderAlign: "center", CellAlign: "right", FormatNumber: FormatConstant.TWO_DECIMAL_NUMBER },
                { HeaderText: "Dividend Won", Width: 100, Name: "ColDividendWonPartner", HeaderAlign: "center", CellAlign: "right", FormatNumber: FormatConstant.TWO_DECIMAL_NUMBER },
                { HeaderText: "Comm Expense", Width: 100, Name: "ColCommExpensePartner", HeaderAlign: "center", CellAlign: "right", FormatNumber: FormatConstant.TWO_DECIMAL_NUMBER },
                { HeaderText: "Tote Fee", Width: 80, Name: "ColToteFeePartner", HeaderAlign: "center", CellAlign: "right", FormatNumber: FormatConstant.TWO_DECIMAL_NUMBER },
                { HeaderText: "Amount Due  <span id='amountDueFormula' class='formular' title=''></span>", Width: 100, Name: "ColAmountDuePartner", HeaderAlign: "center", CellAlign: "right", FormatNumber: FormatConstant.TWO_DECIMAL_NUMBER}],
            showTitle: true,
            showFooter: true,
            titleInfo:
            {
                text: "Horse Racing Daily Report for UK - Date " + self.Params.reportParams.winlostDate,
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
        _tables.report.addClass("display-none");
        _tables.reportContainer.addClass("display-none");
    };

    self.showReport = function () {
        _tables.report.removeClass("display-none");
        _tables.reportContainer.removeClass("display-none");
    };

    self.exportExcel = function () {
        AccReport.downloadFile(window.rootUrl + "Racing/RacingUKTote/ExportExcelRacingWinLossMain", self.Params.reportParams);
    };
};