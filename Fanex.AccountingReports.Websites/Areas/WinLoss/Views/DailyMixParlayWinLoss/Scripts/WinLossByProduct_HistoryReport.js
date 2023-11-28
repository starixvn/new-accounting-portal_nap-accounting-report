WinLossByProduct_HistoryReport = function () {
    var self = this;

    self.Tables = {
        btnGenerate: null,
        monthPicker: null,
        ddlReport: null,
        ddlSiteType_HistoryReport: null
    };

    self.Params = {
        reportParams: {
            FromDate: null,
            ToDate: null,
            Site: 0
        }
    };
};

WinLossByProduct_HistoryReport.prototype.InitPage = function () {
    this.CachedDOMElements();
};

WinLossByProduct_HistoryReport.prototype.CachedDOMElements = function () {
    this.Tables.ddlReport_HistoryReport = $("#ddlReport_HistoryReport");
    this.Tables.btnGenerate_HistoryReport = $("#btnGenerate_HistoryReport");
    this.Tables.monthPicker = $('#ddlMonth');
    this.Tables.ddlSiteType_HistoryReport = $('#ddlSiteType_HistoryReport');

    this.Tables.reportContainer = $('#advanceMixParlayReportContainer_HistoryReport');
    this.Tables.report = $('#advanceMixParlayReport_HistoryReport');
};

WinLossByProduct_HistoryReport.prototype.BindingEvent = function () {
    var self = this;

    self.Tables.btnGenerate_HistoryReport.unbind("click");
    self.Tables.btnGenerate_HistoryReport.click(function () {
        self.GetReport();
    });
};

WinLossByProduct_HistoryReport.prototype.GetReport = function (dateType) {
    var self = this;

    AccReport.disableInput();
    self.GetParams(dateType);
    $.ajax({
        type: "POST",
        url: window.rootUrl + "WinLoss/DailyMixParlayWinLoss/GetAdvanceMixParlayReport",
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        data: JSON.stringify(self.Params.reportParams),
        beforeSend: function () {
        },
        success: function (rs) {
            AccReport.popUp.hide();
            self.RenderReport(rs);
        },
        error: function (jqXHR, event, ajaxSettings) {
            if (jqXHR.status && jqXHR.status == AccReportData.Constants.HtmlErrorCode.SystemError) {
                var r = jQuery.parseJSON(jqXHR.responseText);
                AccReport.popUp.show(r.message);
                self.RenderReport(null);
            }
        },
        complete: function () {
            AccReport.enableInput();
        }
    });
};

WinLossByProduct_HistoryReport.prototype.GetParams = function (dateType) {
    this.Params.reportParams.FromDate = this.Tables.monthPicker.monPickr("getFromDate", FormatConstant.MMM_DD_YY_STRING_MONTH_PICKER);
    this.Params.reportParams.ToDate = this.Tables.monthPicker.monPickr("getToDate", FormatConstant.MMM_DD_YY_STRING_MONTH_PICKER);
    this.Params.reportParams.Site = this.Tables.ddlSiteType_HistoryReport.val();
};

WinLossByProduct_HistoryReport.prototype.RenderReport = function (report) {
    var self = this;

    reportManager.HideCombinationWinLossReport_HistoryReport();
    self.Tables.reportContainer.removeClass("display-none");
    self.Tables.report.addClass("display-none");
    var bodyData = [];
    var footerData = [];
    if (report && report.ChildsInfo.length > 0) {
        var len = report.ChildsInfo.length;
        var currentRow = null;
        var info = null;
        for (var i = 0; i < len; i++) {
            var liveTypeGroup = report.ChildsInfo[i];
            var liveTypeGroupLength = liveTypeGroup.ChildsInfo.length;
            for (var j = 0; j < liveTypeGroupLength; j++) {
                var sportGroup = liveTypeGroup.ChildsInfo[j];
                var sportGroupLength = sportGroup.ChildsInfo.length;
                for (var k = 0; k < sportGroupLength; k++) {
                    info = sportGroup.ChildsInfo[k];
                    var columns = [];
                    if (j == 0 && k == 0) {
                        columns.push({
                            Name: "ColLiveType",
                            Value: liveTypeGroup.Name,
                            MergeRowNumber: liveTypeGroup.TotalRowDetail + liveTypeGroupLength + 1,
                            TdClass: "bold col" + liveTypeGroup.Name.replace(/\s+/g, '')
                        });
                    }

                    if (k == 0) {
                        columns.push({
                            Name: "ColSportType",
                            Value: sportGroup.Name,
                            MergeRowNumber: sportGroupLength + 1,
                            CssClass: "bold align-center"
                        });
                    }

                    columns.push({ Name: "ColBetType", Value: info.Name });
                    columns.push({ Name: "ColWinLoss", Value: reportManager.FormatNumberCell(info.MemberWinLoss_Str, true, false, false) });
                    columns.push({ Name: "ColPosition", Value: info.SMAWinLoss_Str });
                    columns.push({ Name: "ColExtraPT", Value: info.ExtraPT_Str });
                    columns.push({ Name: "ColComm", Value: info.Commission_Str });
                    columns.push({ Name: "ColLineTotal", Value: reportManager.FormatNumberCell(info.RealIncome_Str, true, false, false) });
                    columns.push({ Name: "ColTurnover", Value: info.RealTurnover_Str });
                    columns.push({ Name: "ColMargin", Value: reportManager.FormatNumberCell(info.Margin_Str, true, false, true) });
                    columns.push({ Name: "ColMemberBonus", Value: reportManager.FormatNumberCell(info.MemberBonus_Str, true, false, false) });
                    columns.push({ Name: "ColHouseBonus", Value: reportManager.FormatNumberCell(info.HouseBonus_Str, true, false, false) });
                    bodyData.push({
                        Columns: columns
                    });
                }

                info = sportGroup.GrandTotal;
                bodyData.push({
                    CssClass: "betTypeTotal",
                    Columns: [{ Name: "ColBetType", Value: info.Name, CssClass: "align-center" },
                    { Name: "ColWinLoss", Value: reportManager.FormatNumberCell(info.MemberWinLoss_Str, true, false, false) },
                    { Name: "ColPosition", Value: info.SMAWinLoss_Str },
                    { Name: "ColExtraPT", Value: info.ExtraPT_Str },
                    { Name: "ColComm", Value: info.Commission_Str },
                    { Name: "ColLineTotal", Value: reportManager.FormatNumberCell(info.RealIncome_Str, true, false, false) },
                    { Name: "ColTurnover", Value: info.RealTurnover_Str },
                    { Name: "ColMargin", Value: reportManager.FormatNumberCell(info.Margin_Str, true, false, true) },
                    { Name: "ColMemberBonus", Value: reportManager.FormatNumberCell(info.MemberBonus_Str, true, false, false) },
                    { Name: "ColHouseBonus", Value: reportManager.FormatNumberCell(info.HouseBonus_Str, true, false, false) }]
                });
            }

            info = liveTypeGroup.GrandTotal;
            bodyData.push({
                CssClass: "subTotal",
                Columns: [{ Name: "ColSportType", Value: "" },
                    { Name: "ColBetType", Value: info.Name, CssClass: "align-center" },
                    { Name: "ColWinLoss", Value: reportManager.FormatNumberCell(info.MemberWinLoss_Str, true, false, false) },
                    { Name: "ColPosition", Value: info.SMAWinLoss_Str },
                    { Name: "ColExtraPT", Value: info.ExtraPT_Str },
                    { Name: "ColComm", Value: info.Commission_Str },
                    { Name: "ColLineTotal", Value: reportManager.FormatNumberCell(info.RealIncome_Str, true, false, false) },
                    { Name: "ColTurnover", Value: info.RealTurnover_Str },
                    { Name: "ColMargin", Value: reportManager.FormatNumberCell(info.Margin_Str, true, false, true) },
                    { Name: "ColMemberBonus", Value: reportManager.FormatNumberCell(info.MemberBonus_Str, true, false, false) },
                    { Name: "ColHouseBonus", Value: reportManager.FormatNumberCell(info.HouseBonus_Str, true, false, false) }]
            });
        }

        info = report.GrandTotal;
        footerData.push({
            Columns: [{ Name: "ColLiveType", Value: info.Name, MergeColNumber: 3 },
                    { Name: "ColWinLoss", Value: reportManager.FormatNumberCell(info.MemberWinLoss_Str, true, false, false) },
                    { Name: "ColPosition", Value: info.SMAWinLoss_Str },
                    { Name: "ColExtraPT", Value: info.ExtraPT_Str },
                    { Name: "ColComm", Value: info.Commission_Str },
                    { Name: "ColLineTotal", Value: reportManager.FormatNumberCell(info.RealIncome_Str, true, false, false), TdClass: "colLineTotal" },
                    { Name: "ColTurnover", Value: info.RealTurnover_Str },
                    { Name: "ColMargin", Value: reportManager.FormatNumberCell(info.Margin_Str, true, false, true) },
                    { Name: "ColMemberBonus", Value: reportManager.FormatNumberCell(info.MemberBonus_Str, true, false, false) },
                    { Name: "ColHouseBonus", Value: reportManager.FormatNumberCell(info.HouseBonus_Str, true, false, false) }]
        });
    }

    var gridOptions = {
        customizeHeader: false,
        columns: [{ HeaderText: "", Width: 140, Name: "ColLiveType", HeaderAlign: "center", CellAlign: "center" },
                      { HeaderText: "Sport Type", Width: 140, Name: "ColSportType", HeaderAlign: "center", CellAlign: "right" },
                      { HeaderText: "Bet Type", Width: 120, Name: "ColBetType", HeaderAlign: "center", CellAlign: "left" },
                      { HeaderText: "Member Win Loss", Width: 120, Name: "ColWinLoss", HeaderAlign: "center", CellAlign: "right" },
                      { HeaderText: "SMA Win Loss", Width: 100, Name: "ColPosition", HeaderAlign: "center", CellAlign: "right" },
                      { HeaderText: "Extra PT", Width: 120, Name: "ColExtraPT", HeaderAlign: "center", CellAlign: "right" },
                      { HeaderText: "Commission", Width: 100, Name: "ColComm", HeaderAlign: "center", CellAlign: "right" },
                      { HeaderText: "Real Income", Width: 120, Name: "ColLineTotal", HeaderAlign: "center", CellAlign: "right" },
                      { HeaderText: "Real Turnover", Width: 120, Name: "ColTurnover", HeaderAlign: "center", CellAlign: "right" },
                      { HeaderText: "Margin", Width: 100, Name: "ColMargin", HeaderAlign: "center", CellAlign: "right" },
                      { HeaderText: "Member Bonus", Width: 120, Name: "ColMemberBonus", HeaderAlign: "center", CellAlign: "right" },
                      { HeaderText: "House Bonus", Width: 120, Name: "ColHouseBonus", HeaderAlign: "center", CellAlign: "right" }],
        showTitle: true,
        showFooter: true,
        titleInfo:
            {
                text: 'Parlay Win Loss by Product  by ' + reportManager.GetSiteTypeName(this.Params.reportParams.Site) +
                      ' - From ' + this.Params.reportParams.FromDate +
                      ' To ' + this.Params.reportParams.ToDate,
                showExcelButton: true,
                exportExcelFunction: function () {
                    self.ExportReport();
                }
            },
        showTree: false,
        bodyRows: bodyData,
        footerRows: footerData,
        gridExpandHeight: 230,
        gridWidth: 900,
        numberOfFrozenColumn: 0,
        showDistinctRow: false,
        showHighlightRow: false,
        noDataText: viewResources.EmptyReportMessage,
        marginRight: 20
    };

    self.Tables.report.removeClass("display-none").fanexGrid(gridOptions);
};

WinLossByProduct_HistoryReport.prototype.HideReport = function () {
    this.Tables.reportContainer.addClass("display-none");
};

WinLossByProduct_HistoryReport.prototype.ExportReport = function () {
    AccReport.downloadFile(window.rootUrl + "WinLoss/DailyMixParlayWinLoss/ExportAdvanceMixParlayReport",
        this.Params.reportParams);
};