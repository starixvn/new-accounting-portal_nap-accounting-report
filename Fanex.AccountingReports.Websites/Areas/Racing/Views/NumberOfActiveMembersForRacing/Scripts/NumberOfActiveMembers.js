Namespace.Register("Fanex.AccountingReports.Racing.NumberOfActiveMembers");

Fanex.AccountingReports.Racing.NumberOfActiveMembers = function () {
    var self = this;

    var _tables = {
        datepickerplugin: null,
        btnGenerate: null,
        reportContainer: null,
        report: null
    };

    var _param = {
        reportParams: {
            FromDate: null,
            ToDate: null
        }
    };

    self.initPage = function () {
        cachedDOMElements();
        renderDatePicker();
        bindingEvent();
    };

    var cachedDOMElements = function () {
        _tables.btnGenerate = $("#btnGenerate");
        _tables.reportContainer = $("#numberOfActiveMemberReportGrid");
        _tables.report = $("#numberOfActiveMemberReportMainGrid");
    };

    var bindingEvent = function () {
        _tables.btnGenerate.click(function () {
            self.getNumberOfActiveMember();
        });
    };

    var renderDatePicker = function () {
        $('#datepickerplugin').nexDatePicker({
            nexDatePickerType: "daterange",
            datePickerFormat: {
                changeMonth: true,
                changeYear: true,
                dateFormat: FormatConstant.M_DD_YY_STRING
            },
            firstDatePicker: {
                Id: "firstDatePickerId1",
                Label: labelFrom,
                InitDate: today,
                LabelCss: "lbl-field col_2",
                TextboxCss: "edit-field col_6"
            },
            secondDatePicker: {
                Id: "secondDatePickerId1",
                Label: labelTo,
                InitDate: today,
                LabelCss: "lbl-field col_1",
                TextboxCss: "edit-field col_6"
            },
            onSelectDatePicker: function () {
            },
            maxDate: new Date(today)
        });

        _tables.datepickerplugin = $("#datepickerplugin");
    };

    self.getNumberOfActiveMember = function () {
        var dateRange = _tables.datepickerplugin.nexDatePicker("getDateRange");
        _param.reportParams.FromDate = dateRange.FromDate;
        _param.reportParams.ToDate = dateRange.ToDate;

        AccReport.disableInput();

        var data = {
            FromDate: _param.reportParams.FromDate,
            ToDate: _param.reportParams.ToDate
        };

        AccReport.disableInput();

        AccReport.utility.CallAjax(this,
                                    window.rootUrl + "Racing/NumberOfActiveMembersForRacing/GetNumberOfActiveMembers",
                                    data,
                                    self.renderData,
                                    self.renderData,
                                    AccReport.enableInput);

    };

    self.renderData = function (rs) {
        var title = "Number of Active Members For Racing - From " + _param.reportParams.FromDate + " To " + _param.reportParams.ToDate;
        var columns = [
					{ HeaderText: totalActiveMemberLabel, Width: 400, Name: "TotalActiveMemberLabel", CellAlign: "Center", ShowCollapseExpand: true, HeaderAlign: "center", CssClass: "display-none" },
					{ HeaderText: totalActiveMemberLabel, Width: 200, Name: "TotalActiveMemberValue", CellAlign: "right", HeaderAlign: "center", FormatNumber: FormatConstant.INT_NUMBER, CssClass: "display-none" }
				];

        var bodyData = [];

        if (rs) {
            bodyData.push({
                CssClass: "totalRow",
                Columns: [{ Name: "TotalActiveMemberLabel", Value: totalActiveMemberLabel },
                    { Name: "TotalActiveMemberValue", Value: rs.TotalActiveMembers, CellAlign: "right"}]
            });
        }

        var gridOption = {
            columns: columns,
            showFooter: false,
            showTitle: true,
            titleInfo:
			    {
			        text: title,
			        showExcelButton: true,
			        exportExcelFunction: function () {
			            self.ExportData();
			        }
			    },
            showTree: true,
            bodyRows: bodyData,
            gridExpandHeight: 200,
            showDistinctRow: false,
            marginRight: 20
        };

        _tables.report.fanexGrid(gridOption);
    };

    self.ExportData = function () {
        var exportParams = {
            FromDate: _param.reportParams.FromDate,
            ToDate: _param.reportParams.ToDate
        };

        AccReport.downloadFile(window.rootUrl + "Racing/NumberOfActiveMembersForRacing/ExportReport", exportParams);
    };
};

$(document).ready(function () {
    var numberOfActiveMembers = new Fanex.AccountingReports.Racing.NumberOfActiveMembers();
    numberOfActiveMembers.initPage();
});