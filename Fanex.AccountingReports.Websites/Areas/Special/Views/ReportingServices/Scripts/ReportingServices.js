$(document).ready(function () {
    var reportingServices = new Fanex.AccountingReports.Special.ReportingServices.ReportingServices();
    reportingServices.initPage();
});

Namespace.Register("Fanex.AccountingReports.Special.ReportingServices.ReportingServices");
Fanex.AccountingReports.Special.ReportingServices.ReportingServices = function () {
    var self = this;

    var reportParam = {
        Path: null
    }

    var tables = {
        reportContainer: null,
        reportContent: null
    }

    var cachedParams = {
        isFirstLoad: true
    };

    self.initPage = function () {
        cachedDomElements();
        if (cachedParams.isFirstLoad) {
            self.renderDefaultPage();
        }
    };

    var cachedDomElements = function () {
        tables.reportContainer = $("#reportingServicesContainer");
        tables.reportContent = $("#reportingServicesContent");
    };

    self.renderDefaultPage = function () {
        getReport(defaultPath);
    };

    getReport = function (path) {
        AccReport.disableInput();

        if (path) {
            path = path.replace(/\^/g, "\\");
        }

        reportParam.Path = path;

        if (path == defaultPath) {
            cachedParams.isFirstLoad = true;
        }

        AccReport.utility.CallAjax(this,
            window.rootUrl + "Special/ReportingServices/GetReport",
            reportParam,
            self.renderReport,
            self.renderReport,
            AccReport.enableInput);
    };

    downloadFile = function (path) {
        path = path.replace(/\^/g, "\\");
        reportParam.Path = path;
        AccReport.downloadFile(window.rootUrl + "Special/ReportingServices/Download", reportParam);
    };

    self.renderReport = function (rs) {
        var parentDirectory = rs.ParentFolder.replace(/\\/g, "^");
        tables.reportContent.removeClass("display-none");
        var title = "Click on the link as below to download report excel file";
        var columns = [
            { HeaderText: "No", Width: 40, Name: "Number", HeaderAlign: "center", CellAlign: "center", FormatNumber: FormatConstant.INT_NUMBER },
            { HeaderText: "Directory/Files", Width: 450, Name: "Directory", HeaderAlign: "center", CellAlign: "left" }
        ];

        var bodyData = [];
        var rowIndex = 0;

        if (rs) {
            if (rs.Folders.length > 0) {
                rowIndex = self.renderFolders(rs.Folders, bodyData, rowIndex);
            }

            if (rs.Files.length > 0) {
                rowIndex = self.renderFiles(rs.Files, bodyData, rowIndex);
            }
        }

        var gridOptions = {
            columns: columns,
            showFooter: false,
            showTitle: true,
            titleInfo: {
                text: title,
                showBackButton: cachedParams.isFirstLoad ? false : true,
                backFunction: function () {
                    getReport(parentDirectory);
                }
            },
            bodyRows: bodyData,
            gridExpandHeight: 200,
            showPager: false
        };

        tables.reportContent.fanexGrid(gridOptions);
        cachedParams.isFirstLoad = false;
    };

    self.renderFolders = function (folders, bodyData, rowIndex) {
        for (var folderIndex = 0; folderIndex < folders.length; folderIndex++) {
            rowIndex++;
            var currentFolder = folders[folderIndex];
            bodyData.push({
                Columns: [
                    { Name: "Number", Value: rowIndex },
                    { Name: "Directory", Value: currentFolder, CustomCellRender: function (currentCell) {
                        var currentPath = currentCell.Value.Path.replace(/\\/g, "^");
                        { return '<div class="folderBtn" onclick="getReport(\'' + currentPath + '\')">' + currentCell.Value.FolderName + '</div>'; }
                    }
                    }
                ]
            });
        }

        return rowIndex;
    };

    self.renderFiles = function (files, bodyData, rowIndex) {
        for (var fileIndex = 0; fileIndex < files.length; fileIndex++) {
            rowIndex++;
            var currentFile = files[fileIndex];
            bodyData.push({
                Columns: [
                    { Name: "Number", Value: rowIndex },
                    { Name: "Directory", Value: currentFile, CustomCellRender: function (currentCell) {
                        var currentPath = currentCell.Value.Path.replace(/\\/g, "^");
                        { return '<div class="excelBtn" onclick="downloadFile(\'' + currentPath + '\')">' + currentCell.Value.FileName + '</div>'; }
                    }
                    }
                ]
            });
        }

        return rowIndex;
    };
};