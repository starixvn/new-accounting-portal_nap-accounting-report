//"use strict";
/* jshint camelcase:false,
eqeqeq: false,
unused:vars */
/* global viewResources,
legendManagement */

var inputedLegendManager = {
    _tables: {
        buttonBack: null,
        reportContainer: null,
        report: null
    },
    _params: {
        getInputedLegendReport: {
            System: null
        }
    },
    _cacheData: {
        isFirstLoad: true
    },
    initPage: function () {
        inputedLegendManager.cachedDOMElements();
        inputedLegendManager.bindingEvent();
    },
    disableInput: function () {
        try {
            $.blockUI();
        } catch (e) {
            parent.blockUI();
        }
    },
    enableInput: function () {
        try {
            $.unblockUI();
        } catch (e) {
            parent.unblockUI();
        }
    },

    cachedDOMElements: function () {
        inputedLegendManager._tables.buttonBack = $("#button-back");

        inputedLegendManager._tables.reportContainer = $("#inputedlegend-reportcontainer");
        inputedLegendManager._tables.report = $("#inputedlegend-report");
    },
    bindingEvent: function () {
        inputedLegendManager._tables.buttonBack.unbind("click");
        inputedLegendManager._tables.buttonBack.click(function () {
            legendManagement.showEditLegendView();
        });
    },
    getInputedLegendReport: function () {
        inputedLegendManager.disableInput();
        inputedLegendManager._params.getInputedLegendReport.System = "ACC-NAP";

        $.ajax({
            type: "POST",
            url: window.rootUrl + "LegendManagement/LegendManagement/GetInputedLegendReport",
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            data: JSON.stringify(inputedLegendManager._params.getInputedLegendReport),
            success: function (rs) {
                inputedLegendManager.renderInputedLegendReport(rs);
            },
            error: function (event, jqXHR, ajaxSettings) {
                inputedLegendManager.renderInputedLegendReport([]);
            },
            complete: function () {
                inputedLegendManager.enableInput();
            }
        });
    },
    renderInputedLegendReport: function (report) {
        inputedLegendManager._tables.reportContainer.removeClass("display-none");
        inputedLegendManager._tables.report.addClass("display-none");
        var bodyData = [];
        var footerData = [];
        if (report && report.length > 0) {
            var reportLength = report.length;
            var info;
            for (var i = 0; i < reportLength; i++) {
                info = report[i];
                bodyData.push({
                    Columns: [{ Name: "ColSystem", Value: info.System },
                        { Name: "ColReportId", Value: info.ReportId },
                        { Name: "ColReportName", Value: info.ReportName },
                        { Name: "ColContent", Value: info.Content}]
                });
            }
        }

        var gridOptions = {
            columns: [{ HeaderText: viewResources.System, Width: 200, Name: "ColSystem", HeaderAlign: "center", CellAlign: "left" },
                { HeaderText: viewResources.Report_Id, Width: 140, Name: "ColReportId", HeaderAlign: "center", CellAlign: "left" },
                { HeaderText: viewResources.Report_Name, Width: 300, Name: "ColReportName", HeaderAlign: "center", CellAlign: "left" },
                { HeaderText: viewResources.Content, Width: 313, Name: "ColContent", HeaderAlign: "center", CellAlign: "left"}],
            showTitle: true,
            showFooter: true,
            titleInfo: {
                text: viewResources.Inputed_Legend_Report_Title
            },
            bodyRows: bodyData,
            footerRows: footerData,
            numberOfFrozenColumn: 0,
            gridExpandHeight: 200,
            fullWidth: true,
            fullHeight: true,
            showDistinctRow: true
        };

        inputedLegendManager._tables.report.removeClass("display-none").fanexGrid(gridOptions);
    }
};