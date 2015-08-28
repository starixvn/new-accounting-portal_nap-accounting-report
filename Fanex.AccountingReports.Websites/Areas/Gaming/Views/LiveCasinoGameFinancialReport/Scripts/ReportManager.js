
$(document).ready(function () {
    reportManager.initPage();
    $('.monpickr-super-wrapper .monpickr-label').eq(0).addClass('col_3 align-right lbl-field');
    $('.monpickr-super-wrapper .daypickr-wrap-parent').eq(0).addClass('col_3'); 
});

ReportManager = function () {
    var self = this;
    var ddwLicensee_Ready = false;
    var ddwCurrency_Ready = false;
    var ddwGameType_Ready = false;
    var ddwGameTable_Ready = false;

    FormatConstant = {
        INT_NUMBER: '0,0',
        TWO_DECIMAL_NUMBER: '0,0.00'
    };
    var _tables = {
        btnGenerate: null,
        datepickerplugin: null,
        btnFromDate: null,
        btnToDate: null,
        ddwLicensee: null,
        ddwCurrency: null,
        ddwGameType: null,
        ddwGameTable: null,
        errorMessage: null,
        filterNote: null,

        grdContainer: null,
        reportContent: null,
        reportContent_Container: null,
        reportTitle: null,
        reportExportExcel: null,
        reportRowHeader: null,
        reportRowFooter: null,
        reportColFooter: null,
        reportTopLeft: null,
        colGameLevy: null,
        exportFrame: null
    };
    var _params = {
        getGameTables: {
            GameType: null
        },

        reportParams: {
            Licensee: '',
            LicenseeName: '',
            DateStart: '',
            DateEnd: '',
            Currency: '',
            CurrencyName: '',
            GameType: '',
            GameTypeName: '',
            GameTable: '',
            GameTableName: ''
        }
    };

    var _options = {
        specificPeriodIndex: 7,
        report: {
            isShowPager: true,
            tblContentId: 'financialReport_Content',
            manualCopyHeader: true,
            fullHeight: true,
            fullWidth: true,
            gridWidth: 1150,
            //            columns: [120, 80, 80, 100, 100, 100, 100, 100, 100, 100, 120],
            gridHeight: 280,
            freezeWidth: 290,
            freezeHeight: 150,
            numberFreezeCol: 3
        }
    };

    self.TempHtml = {
        frozenHtml: '',
        contentHtml: '',
        footerFrozenHtml: '',
        footerContentHtml: ''
    };
    var _cachedData = {
        fromDate: null,
        toDate: null
    };

    self.initPage = function () {
        AccReport.disableInput();
        renderDatePicker();
        getLicensees();
        getCurrencies();
        getGameTypes();
        cachedDOMElements();
        bindingEvent();
        RenderFilterNote();
    };
    var renderDatePicker = function () {
        var lastdate = '';
        lastdate = lastMonth + '/' + lastDay + '/' + lastYear;
        $('#spMonth').monPickr({
            plugInId: '#spMonth',
            today: lastdate,
            maxDate: _today,
            minDate: _minDate,
            showDateRange: true,
            fromDayId: 'spFrom',
            toDayId: 'spTo',
            fromDayLabel: 'From',
            toDayLabel: 'To',
            log: function (msg) {
                console.warn(msg);
            }
        });

        _tables.datepickerplugin = $('#spMonth');
        _tables.btnFromDate = $('#spFrom');
        _tables.btnToDate = $('#spTo');
    };
    var cachedDOMElements = function () {
        _tables.btnGenerate = $("#btnReport");
        _tables.errorMessage = $("#inforMessage");

        _tables.report = $("#financialReport");
        _tables.reportTopLeft = _tables.report.find(".free-top-left");
        _tables.reportTopLeftHeader = _tables.report.find("#financialReport_RowHeader");
        _tables.reportContent_Container = _tables.report.find("#financialReport_Content");
        _tables.reportContent = _tables.report.find("#financialReport_Content tbody");
        _tables.reportTitle = _tables.report.find("#financialReport_Title");
        _tables.reportExportExcel = _tables.report.find("#financialReport_ExportExcel");
        _tables.reportRowHeader = _tables.reportTopLeftHeader.find("tbody");
        _tables.reportRowFooter = _tables.report.find(".row_footer_content thead");
        _tables.reportColFooter = _tables.report.find(".col_footer_content thead");
        _tables.colGameLevy = _tables.report.find(".colGameLevy");
        _tables.exportFrame = $('#frm');
        _tables.grdContainer = $('#GridContainer');
        _tables.filterNote = $('#notemessage');
    };
    var bindingEvent = function () {
        _tables.btnGenerate.click(function () {
            self.getReport();
        });

        _tables.reportExportExcel.click(function () {
            self.exportReport();
        });
    };
    var getLicensees = function () {
        $.ajax({
            type: "POST",
            url: window.rootUrl + "LiveCasinoGameFinancialReport/GetLicensees",
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            data: null,
            success: function (rs) {
                renderDdwLicensee(rs);
            },
            error: function () {
                renderDdwLicensee([]);
            },
            complete: function () {
            }
        });
    };
    var renderDdwLicensee = function (result) {
        var id = "ddwLicensee";
        var length = result.length;
        var licensees = [];
        for (var i = 0; i < length; i++) {
            var info = result[i];
            licensees.push({
                Id: id + info.Id.toString(),
                Value: info.Id,
                Text: info.AliasName,
                Checked: true,
                Enable: true,
                CssClass: "",
                Items: []
            });
        }

        var noItemText = '';
        if (licensees.length == 0) {
            noItemText = 'No licensee found.';
        }

        $("#ddwLicensee").multiSelect(
        {
            minWidth: 170,
            maxHeight: 450,
            shadow: true,
            showHeader: true,
            isCheckAll: true,
            showFooter: false,
            checkAllText: 'All Licensees',
            isUsedCustomCheckAllText: true,
            customCheckAllText: 'All Licensees',
            noItemText: noItemText,
            listData: licensees
        });

        _tables.ddwLicensee = $("#ddwLicensee");
        ddwLicensee_Ready = true;
        if (ddwCurrency_Ready && ddwGameTable_Ready && ddwGameType_Ready && ddwLicensee_Ready) {
            AccReport.enableInput();
        }
    };
    var getCurrencies = function () {
        $.ajax({
            type: "POST",
            url: window.rootUrl + "LiveCasinoGameFinancialReport/GetCurrencies",
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            data: null,
            success: function (rs) {
                renderDdwCurrency(rs);
            },
            error: function () {
                renderDdwCurrency([]);
            },
            complete: function () {
            }
        });
    };
    var renderDdwCurrency = function (result) {
        var id = "ddwCurrency";
        var length = result.length;
        var currencies = [];
        for (var i = 0; i < length; i++) {
            var info = result[i];
            currencies.push({
                Id: id + info.Id.toString(),
                Value: info.Id,
                Text: info.Name,
                Checked: true,
                Enable: true,
                CssClass: "",
                Items: []
            });
        }

        var noItemText = '';
        if (currencies.length == 0) {
            noItemText = 'No currency found.';
        }

        $("#ddwCurrency").multiSelect(
        {
            minWidth: 170,
            maxHeight: 450,
            shadow: true,
            showHeader: true,
            isCheckAll: true,
            showFooter: false,
            checkAllText: 'All Currencies',
            isUsedCustomCheckAllText: true,
            customCheckAllText: 'All Currencies',
            noItemText: noItemText,
            listData: currencies
        });

        _tables.ddwCurrency = $("#ddwCurrency");
        ddwCurrency_Ready = true;
        if (ddwCurrency_Ready && ddwGameTable_Ready && ddwGameType_Ready && ddwLicensee_Ready) {
            AccReport.enableInput();
        }
    };
    var getGameTypes = function () {
        $.ajax({
            type: "POST",
            url: window.rootUrl + "LiveCasinoGameFinancialReport/GetGameTypes",
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            data: null,
            success: function (rs) {
                renderDdwGameType(rs);
            },
            error: function () {
                renderDdwGameType([]);
            },
            complete: function () {
                getGameTables();
            }
        });
    };
    var renderDdwGameType = function (result) {
        var id = "ddwGameType";
        var length = result.length;
        var gameTypes = [];
        for (var i = 0; i < length; i++) {
            var info = result[i];
            gameTypes.push({
                Id: id + info.Id.toString(),
                Value: info.Id,
                Text: info.Name,
                Checked: true,
                Enable: true,
                CssClass: "",
                Items: []
            });
        }

        var noItemText = '';
        if (gameTypes.length == 0) {
            noItemText = 'No game type found.';
        }

        $("#ddwGameType").multiSelect(
        {
            minWidth: 150,
            maxHeight: 450,
            shadow: true,
            showHeader: true,
            isCheckAll: true,
            showFooter: false,
            checkAllText: 'All Game Types',
            isUsedCustomCheckAllText: true,
            customCheckAllText: 'All Game Types',
            noItemText: noItemText,
            listData: gameTypes,
            onCheck: function () {
                AccReport.disableInput();
                return getGameTables();
            }
        });

        _tables.ddwGameType = $("#ddwGameType");
        ddwGameType_Ready = true;
        if (ddwCurrency_Ready && ddwGameTable_Ready && ddwGameType_Ready && ddwLicensee_Ready) {
            AccReport.enableInput();
        }
    };
    var getGameTables = function () {
        _params.getGameTables.GameType = _tables.ddwGameType != null ?
                                    _tables.ddwGameType.multiSelect('getCheckedItems').join(",") :
                                    "";

        $.ajax({
            type: "POST",
            url: window.rootUrl + "LiveCasinoGameFinancialReport/GetGameTables",
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            data: JSON.stringify(_params.getGameTables),
            success: function (rs) {
                renderDdwGameTable(rs);
            },
            error: function () {
                renderDdwGameTable([]);
            },
            complete: function () {
            }
        });
    };
    var renderDdwGameTable = function (result) {
        var id = "ddwGameTable";
        var length = result == undefined ? 0 : result.length;
        var gameTables = [];
        for (var i = 0; i < length; i++) {
            var info = result[i];
            gameTables.push({
                Id: id + info.Id.toString(),
                Value: info.Id,
                Text: info.Name,
                Checked: true,
                Enable: true,
                CssClass: "",
                Items: []
            });
        }

        var noItemText = '';
        if (gameTables.length == 0) {
            noItemText = 'No game table found.';
        }

        $("#ddwGameTable").multiSelect(
        {
            minWidth: 170,
            maxHeight: 450,
            shadow: true,
            showHeader: true,
            isCheckAll: true,
            showFooter: false,
            checkAllText: 'All Game Tables',
            isUsedCustomCheckAllText: true,
            customCheckAllText: 'All Game Tables',
            noItemText: noItemText,
            listData: gameTables
        });

        _tables.ddwGameTable = $("#ddwGameTable");
        ddwGameTable_Ready = true;
        if (ddwCurrency_Ready && ddwGameTable_Ready && ddwGameType_Ready && ddwLicensee_Ready) {
            AccReport.enableInput();
        }
    };
    self.getReport = function () {
        AccReport.popUp.removeHighLight();
        AccReport.disableInput();
        getParams();
        if (validateParams() == true) {
            _tables.errorMessage.empty().addClass("hidden");
            $.ajax({
                type: "POST",
                url: window.rootUrl + "LiveCasinoGameFinancialReport/GetFinancialReport",
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                data: JSON.stringify(_params.reportParams),
                beforeSend: function () {
                },
                success: function (rs) {
                    self.renderReport(rs);
                },
                error: function (event, jqXHR, ajaxSettings) {
                    self.renderReport(null);
                },
                complete: function () {
                    AccReport.enableInput();
                }
            });
        }
        else {
            AccReport.enableInput();
        }
    };
    var getParams = function () {
        _params.reportParams.Licensee = _tables.ddwLicensee.multiSelect('getCheckedItems').join(",");
        _params.reportParams.LicenseeName = _tables.ddwLicensee.multiSelect('getCheckedItemsByText').join(",");
        _params.reportParams.Currency = _tables.ddwCurrency.multiSelect('getCheckedItems').join(",");
        _params.reportParams.CurrencyName = _tables.ddwCurrency.multiSelect('getCheckedItemsByText').join(",");
        _params.reportParams.GameType = _tables.ddwGameType.multiSelect('getCheckedItems').join(",");
        _params.reportParams.GameTypeName = _tables.ddwGameType.multiSelect('getCheckedItemsByText').join(",");
        _params.reportParams.GameTable = _tables.ddwGameTable.multiSelect('getCheckedItems').join(",");
        _params.reportParams.GameTableName = _tables.ddwGameTable.multiSelect('getCheckedItemsByText').join(",");

        _params.reportParams.DateStart = _tables.datepickerplugin.monPickr("getFromDate", "MM/dd/yyyy");
        _params.reportParams.DateEnd = _tables.datepickerplugin.monPickr("getToDate", "MM/dd/yyyy");
    };
    var validateParams = function () {
        var notifyIds = [];
        var msg = '';
        if (_params.reportParams.Licensee.length == 0) {
            //showAlertMessage("ErrorMessage", "Please select at least one licensee!", 1);
            msg = 'Please select at least one licensee!';
            notifyIds.push("ddwLicensee");
        }

        if (_params.reportParams.Currency.length == 0) {
            //showAlertMessage("ErrorMessage", "Please select at least one currency!", 1);
            msg = 'Please select at least one currency!';
            notifyIds.push("ddwCurrency");
        }


        if (_params.reportParams.GameType.length == 0) {
            //showAlertMessage("ErrorMessage", "Please select at least one game type!", 1);
            msg = 'Please select at least one game type!';
            notifyIds.push("ddwGameType");
        }


        if (_params.reportParams.GameTable.length == 0) {
            //showAlertMessage("ErrorMessage", "Please select at least one game table!", 1);
            msg = 'Please select at least one game table!';
            notifyIds.push("ddwGameTable");
        }

        if (msg.length > 0) {
            AccReport.popUp.show(msg, notifyIds.join(','));
            return false;
        }

        return true;
    };
    self.renderReport = function (data) {
        var bodyData = [];
        if (data && data.ChildInfo && data.ChildInfo.length > 0) {
            var dataLength = data.ChildInfo.length;
            for (var i = 0; i < dataLength; i++) {
                var item = data.ChildInfo[i];
                var currentRow = {
                    Columns: [
                                { Name: "Name", Value: item.Name },
                                { Name: "PlayerCount", Value: item.PlayerCount },
                                { Name: "GameCount", Value: item.GameCount },
                                { Name: "TotalBets", Value: item.TotalBets },
                                { Name: "TotalWins", Value: item.TotalWins },
                                { Name: "GrossIncome", Value: item.GrossIncome },
                                { Name: "Commission", Value: item.Commission },
                                { Name: "NetIncome", Value: item.NetIncome },
                                { Name: "GamePayout", Value: item.GamePayout * 100 },
                                { Name: "GameLevy", Value: item.GameLevy },
                                { Name: "GameLevyToPay", Value: item.GameLevyToPay }
                            ]
                };

                bodyData.push(currentRow);
            }
        }

        var footerRows = [
              {
                  Columns: [
                    { Value: data.GrandTotal.Name, Name: "Name", CellAlign: 'center' },
                    { Value: data.GrandTotal.PlayerCount, Name: "PlayerCount", CellAlign: 'right', FormatNumber: "0,0.", FormatNegative: '($)' },
                    { Value: data.GrandTotal.GameCount, Name: "GameCount", CellAlign: 'right', FormatNumber: "0,0.", FormatNegative: '($)' },
                    { Value: data.GrandTotal.TotalBets, Name: "TotalBets", CellAlign: 'right', FormatNumber: "0,0.00", FormatNegative: '($)' },
                    { Value: data.GrandTotal.TotalWins, Name: "TotalWins", CellAlign: 'right', FormatNumber: "0,0.00", FormatNegative: '($)' },
                    { Value: data.GrandTotal.GrossIncome, Name: "GrossIncome", CellAlign: 'right', FormatNumber: "0,0.00", FormatNegative: '($)' },
                    { Value: data.GrandTotal.Commission, Name: "Commission", CellAlign: 'right', FormatNumber: "0,0.00", FormatNegative: '($)' },
                    { Value: data.GrandTotal.NetIncome, Name: "NetIncome", CellAlign: 'right', FormatNumber: "0,0.00", FormatNegative: '($)' },
                    { Value: data.GrandTotal.GamePayout * 100, Name: "GamePayout", CellAlign: 'right', FormatNumber: "0,0.00", FormatNegative: '($)' },
                    { Value: data.GrandTotal.GameLevy, Name: "GameLevy", CellAlign: 'right', FormatNumber: "0,0.00", FormatNegative: '($)' },
                    { Value: data.GrandTotal.GameLevyToPay, Name: "GameLevyToPay", CellAlign: 'right', FormatNumber: "0,0.00", FormatNegative: '($)' }
                ]
              }
        ];


        var cols = [
                    { HeaderText: resources.Table, Width: 150, Name: "Name", HeaderAlign: "center", CellAlign: "center" },
                    { HeaderText: resources.LiveCasinoGameFinancialReport_Players, Width: 70, Name: "PlayerCount", HeaderAlign: "center", CellAlign: "right", FormatNumber: "0,0.", FormatNegative: '$' },
                    { HeaderText: resources.LiveCasinoGameFinancialReport_Games, Width: 70, Name: "GameCount", HeaderAlign: "center", CellAlign: "right", FormatNumber: "0,0.", FormatNegative: '$' },
                    { HeaderText: resources.Total_Bets, Width: 100, Name: "TotalBets", HeaderAlign: "center", CellAlign: "right", FormatNumber: "0,0.00", FormatNegative: '$' },
                    { HeaderText: resources.Total_Wins, Width: 100, Name: "TotalWins", HeaderAlign: "center", CellAlign: "right", FormatNumber: "0,0.00", FormatNegative: '$' },
                    { HeaderText: resources.Gross_Income, Width: 100, Name: "GrossIncome", HeaderAlign: "center", CellAlign: "right", FormatNumber: "0,0.00", FormatNegative: '$' },
                    { HeaderText: resources.Comm, Width: 100, Name: "Commission", HeaderAlign: "center", CellAlign: "right", FormatNumber: "0,0.00", FormatNegative: '$' },
                    { HeaderText: resources.Net_Income, Width: 100, Name: "NetIncome", HeaderAlign: "center", CellAlign: "right", FormatNumber: "0,0.00", FormatNegative: '$' },
                    { HeaderText: resources.LiveCasinoGameFinancialReport_GamePayout, Width: 100, Name: "GamePayout", HeaderAlign: "center", CellAlign: "right", FormatNumber: "0,0.00", FormatNegative: '$' },
                    { HeaderText: resources.LiveCasinoGameFinancialReport_GameLevy.replace('{0}', data.GameLevy * 100), Width: 100, Name: "GameLevy", HeaderAlign: "center", CellAlign: "right", FormatNumber: "0,0.00", FormatNegative: '$' },
                    { HeaderText: resources.Game_Levy_to_pay, Width: 100, Name: "GameLevyToPay", HeaderAlign: "center", CellAlign: "right", FormatNumber: "0,0.00", FormatNegative: '$'}];

        var gridOptions = {
            customizeHeader: true,
            groupHeaderRows: [
                {
                    Columns: [
                                { HeaderText: resources.Table, Name: "Name", MergeRowNumber: 2 },
                                { HeaderText: resources.LiveCasinoGameFinancialReport_Players, Name: "PlayerCount", MergeRowNumber: 2 },
                                { HeaderText: resources.LiveCasinoGameFinancialReport_Games, Name: "GameCount", MergeRowNumber: 2 },
                                { HeaderText: resources.Company, Name: "TotalBets", MergeColNumber: 5 },
                                { HeaderText: resources.LiveCasinoGameFinancialReport_GamePayout, Name: "GamePayout", MergeRowNumber: 2 },
                                { HeaderText: resources.LiveCasinoGameFinancialReport_GameLevy.replace('{0}', data.GameLevy * 100), Name: "GameLevy", MergeRowNumber: 2 },
                                { HeaderText: resources.Game_Levy_to_pay, Name: "GameLevyToPay", MergeRowNumber: 2 }
                            ]
                }
            ],
            showTitle: true,
            titleInfo:
            {
                text: resources.GridTitle + " from " + _tables.datepickerplugin.monPickr("getFromDate", "MMM dd,yyyy") + " to " + _tables.datepickerplugin.monPickr("getToDate", "MMM dd,yyyy"),
                showExcelButton: true,
                exportExcelFunction: function () {
                    self.exportReport();
                },
                showBackButton: false,
                showSendMailButton: false
            },
            columns: cols,
            bodyRows: bodyData,
            numberOfFrozenColumn: 2,
            gridExpandHeight: 200,
            showFooter: true,
            footerRows: footerRows,
            noDataText: resources.GridNoDataText
        };

        _tables.grdContainer.fanexGrid(gridOptions);
    };

    self.exportReport = function () {
        var url = window.rootUrl + "LiveCasinoGameFinancialReport/ExportFinancialReport?" + $.param(_params.reportParams);
        _tables.exportFrame.attr('src', url);
    }

    var RenderFilterNote = function () {
        _tables.filterNote.attr("title", resources.NoteMessage);
        _tables.filterNote.tipsy({
            html: true,
            opacity: 0.9,
            gravity: 'ne'
        });
    };
};

var reportManager = new ReportManager();
