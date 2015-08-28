$(document).ready(function () {
    reportManager.InitPage();
});

ReportManager = function () {
    var self = this;
    self.Enum = {
    };

    self.Tables = {
        btnGenerate: null,
        btnExport: null,
        datepickerplugin: null,

        parentContainer: null,
        reportTable: null,
        reportTable_Title: null,
        reportTable_Body: null
    };
    self.Params = {
        reportParams: {
            FromDate: null,
            ToDate: null,
            IsDirectFromFilter: false
        }
    };

    self.CacheData = {
    };

    self.InitPage = function () {
        self.RenderDatePicker();
        self.CachedDOMElements();
        self.BindingEvent();
    };
    self.CachedDOMElements = function () {
        self.Tables.btnGenerate = $("#btnGenerate");
        self.Tables.btnExport = $("#btnExport");

        self.Tables.parentContainer = $("#parentContainer");
        self.Tables.reportContainer = self.Tables.parentContainer.find("#bingoReportContainer");
        self.Tables.report = self.Tables.reportContainer.find("#bingoReport");
    };
    self.RenderDatePicker = function () {
        $('#datepickerplugin').nexDatePicker({
            nexDatePickerType: "daterange",
            datePickerFormat: {
                changeMonth: true,
                changeYear: true,
                dateFormat: FormatConstant.M_DD_YY_STRING,
                minDate: new Date(minDate),
                maxDate: new Date(today)
            },
            firstDatePicker: {
                Id: "firstDatePickerId1",
                Label: viewResources.From,
                InitDate: today,
                LabelCss: "lbl-field col_2",
                TextboxCss: "edit-field col_6"
            },
            secondDatePicker: {
                Id: "secondDatePickerId1",
                Label: viewResources.To,
                InitDate: today,
                LabelCss: "lbl-field col_1",
                TextboxCss: "edit-field col_6"
            },
            onSelectDatePicker: function () {
            }
        });

        this.Tables.datepickerplugin = $("#datepickerplugin");
    };
    self.BindingEvent = function () {
        self.Tables.btnGenerate.unbind("click");
        self.Tables.btnGenerate.click(function () {
            self.GetReport();
        });

        self.Tables.btnExport.unbind("click");
        self.Tables.btnExport.click(function () {
            self.ExportReport_DirectFromFilter();
        });
    };
    self.GetParams = function () {
        var dateRange = this.Tables.datepickerplugin.nexDatePicker("getDateRange");
        return {
            FromDate: dateRange.FromDate,
            ToDate: dateRange.ToDate
        };
    };
    self.GetReport = function () {
        AccReport.disableInput();
        self.Params.reportParams = self.GetParams();
        $.ajax({
            type: "POST",
            url: window.rootUrl + "Gaming/BingoWinLoss/GetReport",
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            data: JSON.stringify(self.Params.reportParams),
            beforeSend: function () {
            },
            success: function (rs) {
                AccReport.popUp.hide();
                self.RenderReport(rs);
            },
            error: function (jqXhr, event, ajaxSettings) {
                if (jqXhr.status && jqXhr.status == 500) {
                    var r = jQuery.parseJSON(jqXhr.responseText);
                    AccReport.popUp.show(r.message);
                    self.RenderReport(null);
                }
            },
            complete: function () {
                AccReport.enableInput();
            }
        });
    };
    self.RenderReport = function (report) {
        self.Tables.reportContainer.removeClass("display-none");
        self.Tables.report.addClass("display-none");
        var bodyData = [];
        var footerData = [];
        if (report && report.ChildsInfo.length > 0) {
            var currentRow = null;
            var info = null;
            var betTypeGroupLength = report.ChildsInfo.length;
            for (var i = 0; i < betTypeGroupLength; i++) {
                var betTypeWinLoss = report.ChildsInfo[i];
                var infoLength = betTypeWinLoss.ChildsInfo.length;
                for (var j = 0; j < infoLength; j++) {
                    info = betTypeWinLoss.ChildsInfo[j];
                    var columns = [];
                    if (j == 0) {
                        columns.push({ Name: "ColBetType", Value: betTypeWinLoss.Name, MergeRowNumber: infoLength });
                    }

                    columns.push({ Name: "ColCurrencyName", Value: info.CurrencyName });
                    columns.push({ Name: "ColTurnover", Value: info.Turnover_Str });
                    columns.push({ Name: "ColWinLoss", Value: info.WinLoss_Str });
                    columns.push({ Name: "ColPosition", Value: info.Position_Str });
                    columns.push({ Name: "ColCommission", Value: info.Commission_Str });
                    columns.push({ Name: "ColLineTotal", Value: info.LineTotal_Str });
                    bodyData.push({
                        Columns: columns
                    });
                }
            }

            info = report.Total;
            footerData.push({
                Columns: [{ Name: "ColBetType", Value: info.Name, MergeColNumber: 2 },
                          { Name: "ColTurnover", Value: info.Turnover_Str },
                          { Name: "ColWinLoss", Value: info.WinLoss_Str },
                          { Name: "ColPosition", Value: info.Position_Str },
                          { Name: "ColCommission", Value: info.Commission_Str },
                          { Name: "ColLineTotal", Value: info.LineTotal_Str}]
            });
        }

        var gridOptions = {
            columns: [{ HeaderText: viewResources.Bet_Type, Width: 100, Name: "ColBetType", HeaderAlign: "center", CellAlign: "center" },
                      { HeaderText: viewResources.Currency, Width: 100, Name: "ColCurrencyName", HeaderAlign: "center", CellAlign: "center" },
                      { HeaderText: viewResources.Turnover, Width: 120, Name: "ColTurnover", HeaderAlign: "center", CellAlign: "right" },
                      { HeaderText: viewResources.Win_Loss, Width: 120, Name: "ColWinLoss", HeaderAlign: "center", CellAlign: "right" },
                      { HeaderText: viewResources.Position, Width: 120, Name: "ColPosition", HeaderAlign: "center", CellAlign: "right" },
                      { HeaderText: viewResources.Commission, Width: 120, Name: "ColCommission", HeaderAlign: "center", CellAlign: "right" },
                      { HeaderText: viewResources.Line_Total, Width: 120, Name: "ColLineTotal", HeaderAlign: "center", CellAlign: "right"}],
            showTitle: true,
            showFooter: true,
            titleInfo:
            {
                text: AccReport.utility.StringFormat(viewResources.BingoWinLoss_ReportTitle,
                        [moment(self.Params.reportParams.FromDate).format(FormatConstant.MMM_DD_YYYY_STRING),
                        moment(self.Params.reportParams.ToDate).format(FormatConstant.MMM_DD_YYYY_STRING)]),

                showExcelButton: true,
                exportExcelFunction: function () {
                    self.ExportReport();
                },
                showCollapseExpandButton: false
            },
            showDistinctRow: true,
            showTree: false,
            bodyRows: bodyData,
            footerRows: footerData,
            gridExpandHeight: 200,
            fullWidth: true,
            numberOfFrozenColumn: 2
        };

        self.Tables.report.removeClass("display-none").fanexGrid(gridOptions);
    };
    self.ExportReport_DirectFromFilter = function () {
        var reportParams = self.GetParams();
        reportParams.IsDirectFromFilter = true;
        AccReport.downloadFile(window.rootUrl + "Gaming/BingoWinLoss/ExportReport", reportParams);
    };
    self.ExportReport = function () {
        AccReport.downloadFile(window.rootUrl + "Gaming/BingoWinLoss/ExportReport", self.Params.reportParams);
    };
};

var reportManager = new ReportManager();

