/*jshint strict: false*/
/*global viewResources,today,minDate,scoreInfoManager,FormatConstant,AccReport,yesterday*/

var dailyWinLoss2hSoccerMinorBettypes;
var DOMElements = {
    datepickerplugin: null,
    datepicker: null,
    datePickerInput: null,
    btnGenerate: null,
    btnToday: null,
    btnYesterday: null,
    btnExport: null,
    btnShowHideSiteTypeInfo: null,
    gridMainContainer: null,
    gridMain: null,
    soccerReport: null,
    gridSoccerLiveContainer: null,
    gridSoccerLive: null,
    gridSoccerNonLiveContainer: null,
    gridSoccerNonLive: null,
    gridBettypeContainer: null,
    gridBettype: null,
    gridSiteContainer: null,
    gridSite: null
};

var reportLevelEnums = {
    Main: 0,
    Soccer: 1,
    Bettype: 2,
    Site: 3
};

var reportLevel = reportLevelEnums.Main;
var groupIndex = 0;
var DailyWinLoss2hSoccerMinorBettypes = function () {
    var self = this;

    self.Cached = {
        SportName: null
    };

    var isCached = false;

    var isExpand = false;

    self.Params = {
        mainReportParams: {
            Winlostdate: null
        },
        sporttypeReportParams: {
            Winlostdate: null
        },
        bettypeReportParams: {
            SportTypeName: null,
            Winlostdate: null,
            Bettype: null,
            Liveindicator: null,
            IsHedge: null,
            BettypeName: null
        },
        siteReportParams: {
            Winlostdate: null,
            Bettype: null,
            Liveindicator: null,
            IsHedge: null,
            Site: null
        }
    };

    var renderDatePicker = function () {
        $('#datepickerplugin').nexDatePicker({
            nexDatePickerType: "single",
            firstDatePicker: {
                Id: "datePickerInput",
                Label: viewResources.Date,
                InitDate: today,
                LabelCss: "lbl-field col_2",
                TextboxCss: "edit-field col_6 width-auto"
            },

            datePickerFormat: {
                dateFormat: FormatConstant.M_DD_YY_STRING,
                changeMonth: true,
                changeYear: true,
                minDate: new Date(minDate),
                maxDate: new Date(today),
                showOtherMonths: true,
                selectOtherMonths: true
            }
        });

        DOMElements.datepickerplugin = $("#datepickerplugin");
        DOMElements.datepicker = $(".ui_nexdatepicker_fromdate");
        DOMElements.datePickerInput = $("#datePickerInput");
    };

    self.Init = function () {
        self.CachedDOMElements();
        DOMElements.btnToday.val(today);
        DOMElements.btnYesterday.val(yesterday);
        renderDatePicker();
        self.BindingEvent();
        scoreInfoManager.InitPage();
    };

    self.GetParams = function () {
        switch (reportLevel) {
            case reportLevelEnums.Main:
                self.Params.mainReportParams.Winlostdate = DOMElements.datepickerplugin.nexDatePicker("getSingleDate");
                break;
            case reportLevelEnums.Soccer:
                self.Params.sporttypeReportParams.Winlostdate = self.Params.mainReportParams.Winlostdate;
                break;
            case reportLevelEnums.Bettype:
                self.Params.bettypeReportParams.Winlostdate = self.Params.mainReportParams.Winlostdate;
                self.Params.bettypeReportParams.SportTypeName = self.Cached.SportName;
                self.Params.bettypeReportParams.Bettype = arguments[0];
                self.Params.bettypeReportParams.Liveindicator = arguments[1] === 1 ? true : false;
                self.Params.bettypeReportParams.IsHedge = arguments[2];
                self.Params.bettypeReportParams.BettypeName = arguments[3];
                break;
            case reportLevelEnums.Site:
                self.Params.siteReportParams.Winlostdate = self.Params.mainReportParams.Winlostdate;
                self.Params.siteReportParams.Bettype = arguments[0];
                self.Params.siteReportParams.Liveindicator = arguments[1] === 1 ? true : false;
                self.Params.siteReportParams.IsHedge = arguments[2];
                self.Params.siteReportParams.Site = arguments[3];
                self.Params.siteReportParams.BettypeName = arguments[4];
                break;
            default:
                break;
        }
    };

    self.CachedDOMElements = function () {
        DOMElements = {
            datepickerplugin: $("#datepickerplugin"),
            datepicker: $(".ui_nexdatepicker_fromdate"),
            datePickerInput: $("#datePickerInput"),
            btnGenerate: $("#btnGenerate"),
            btnToday: $("#btnToday"),
            btnYesterday: $("#btnYesterday"),
            btnExport: $("#btnExport"),
            btnShowHideSiteTypeInfo: $("#btnShowHideSiteTypeInfo"),
            gridMainContainer: $("#gridMainContainer"),
            gridMain: $("#gridMain"),
            soccerReport: $('#soccerReport'),
            gridSoccerLiveContainer: $("#gridSoccerLiveContainer"),
            gridSoccerLive: $("#gridSoccerLive"),
            gridSoccerNonLiveContainer: $("#gridSoccerNonLiveContainer"),
            gridSoccerNonLive: $("#gridSoccerNonLive"),
            gridBettypeContainer: $("#gridBettypeContainer"),
            gridBettype: $("#gridBettype"),
            gridSiteContainer: $("#gridSiteContainer"),
            gridSite: $("#gridSite")
        };
    };

    self.BindingEvent = function () {
        DOMElements.btnGenerate.click(function () {
            switch (reportLevel) {
                case reportLevelEnums.Main:
                    self.GenerateMainReport();
                    break;
                case reportLevelEnums.Soccer:
                    self.Params.sporttypeReportParams.Winlostdate = DOMElements.datepickerplugin.nexDatePicker("getSingleDate");
                    isCached = true;
                    self.GenerateSoccerReport();
                    isCached = false;
                    break;
                case reportLevelEnums.Bettype:
                    self.Params.bettypeReportParams.Winlostdate = DOMElements.datepickerplugin.nexDatePicker("getSingleDate");
                    isCached = true;
                    self.GenerateBettypeReport();
                    isCached = false;
                    break;
                case reportLevelEnums.Site:
                    self.Params.siteReportParams.Winlostdate = DOMElements.datepickerplugin.nexDatePicker("getSingleDate");
                    isCached = true;
                    self.GenerateSiteReport();
                    isCached = false;
                    break;
                default:
                    break;
            }
        });

        DOMElements.btnToday.unbind("click");
        DOMElements.btnToday.click(function () {
            DOMElements.btnExport.attr("disabled", null);
            DOMElements.datepicker.datepicker("setDate", new Date(DOMElements.btnToday.val()));
            DOMElements.btnGenerate.click();
            //self.GenerateMainReport(moment().format(FormatConstant.MMM_DD_YYYY_STRING_MOMENT));
        });

        DOMElements.btnYesterday.unbind("click");
        DOMElements.btnYesterday.click(function () {
            DOMElements.btnExport.attr("disabled", null);
            DOMElements.datepicker.datepicker("setDate", new Date(DOMElements.btnYesterday.val()));
            DOMElements.btnGenerate.click();
            //self.GenerateMainReport(moment(yesterday).format(FormatConstant.MMM_DD_YYYY_STRING_MOMENT));
        });
    };

    self.GenerateMainReport = function (winlostDate) {
        AccReport.disableInput();
        reportLevel = reportLevelEnums.Main;
        if (!isCached) {
            self.GetParams();
        }

        if (winlostDate !== undefined) {
            self.Params.mainReportParams.Winlostdate = winlostDate;
        }

        AccReport.utility.CallAjax(this,
                                    window.rootUrl + "Main/DailyWinLoss2hSoccerMinorBettype/GenerateMainReport",
                                    self.Params.mainReportParams,
                                    self.RenderMainReport,
                                    AccReport.enableInput,
                                    AccReport.enableInput);

    };

    self.RenderMainReport = function (gridOptions) {
        groupIndex = 0;
        gridOptions.numberOfFrozenColumn = 0;
        gridOptions.gridWidth = 1200;
        gridOptions.gridExpandHeight = 200;
        gridOptions.noDataText = viewResources.EmptyReportMessage;
        gridOptions.headerColumnHeight = 17;
        gridOptions.titleInfo = {
            /*jshint camelcase: false */
            text: viewResources.DailyWinLoss2h_SoccerReport_MainReport_Title,
            showExcelButton: false
        };

        if (gridOptions.bodyRows !== undefined && gridOptions.bodyRows.length > 0) {
            self.Cached.SportName = gridOptions.bodyRows[0].Columns[0].Value;
            DOMElements.btnExport.attr('disabled', null);
            self.ExportExcel();
            DOMElements.btnShowHideSiteTypeInfo.show();
            DOMElements.btnShowHideSiteTypeInfo.attr('disabled', null);
            DOMElements.btnShowHideSiteTypeInfo.unbind('click');
            DOMElements.btnShowHideSiteTypeInfo.click(function () {
                var $this = $(this);
                if ($this.val() === "Expand") {
                    $this.val("Collapse");
                    isExpand = true;
                }
                else {
                    $this.val("Expand");
                    isExpand = false;
                }

                self.ExpandCollapseAll(isExpand);
            });
        }

        generateShowHideCell(gridOptions, 9999);
        hideAllGrid();
        DOMElements.gridMainContainer.removeClass('display-none');
        DOMElements.gridMain.removeClass('display-none');
        DOMElements.gridMain.fanexGrid(gridOptions);
        DOMElements.gridMain.find('[class^="data-group-"]').addClass('display-none');
        DOMElements.gridMain.fanexGrid('resizeGrid');
        self.ExpandCollapseAll(isExpand);
    };

    self.ShowHideDataGroup = function (element, dataGroupIndex) {
        if (element !== undefined) {
            var $element = $(element);
            if ($element.hasClass('collapse')) {
                $element.removeClass('collapse');
                $element.addClass('expand');
            }
            else {
                $element.removeClass('expand');
                $element.addClass('collapse');
            }
        }

        DOMElements.gridMain.find(".data-group-" + dataGroupIndex).each(function () {
            var $this = $(this);
            if ($this.hasClass('display-none')) {
                $this.removeClass('display-none');
            } else {
                $this.addClass('display-none');
            }
        });

        DOMElements.gridMain.fanexGrid('resizeGrid');
    };

    self.GenerateSoccerReport = function () {
        AccReport.disableInput();
        reportLevel = reportLevelEnums.Soccer;
        if (!isCached) {
            self.GetParams();
        }

        AccReport.utility.CallAjax(this,
                                    window.rootUrl + "Main/DailyWinLoss2hSoccerMinorBettype/GenerateSoccerReport",
                                    self.Params.sporttypeReportParams,
                                    self.RenderSoccerReport,
                                    AccReport.enableInput,
                                    AccReport.enableInput);

    };

    self.RenderSoccerReport = function (gridOptions) {
        groupIndex = 0;
        DOMElements.soccerReport.removeClass('display-none');
        hideAllGrid();
        self.RenderSoccerNonLiveReport(gridOptions[0]);
        self.RenderSoccerLiveReport(gridOptions[1]);
        if (gridOptions[0].bodyRows !== undefined && gridOptions[0].bodyRows.length > 0) {
            DOMElements.btnShowHideSiteTypeInfo.show();
            DOMElements.btnExport.attr('disabled', null);
            self.ExportExcel();
        }
    };

    self.RenderSoccerLiveReport = function (gridOptions) {
        gridOptions.numberOfFrozenColumn = 0;
        gridOptions.gridWidth = 1200;
        gridOptions.gridExpandHeight = 200;
        gridOptions.noDataText = viewResources.EmptyReportMessage;
        gridOptions.headerColumnHeight = 17;
        gridOptions.titleInfo = {
            /*jshint camelcase: false */
            text: "<a class='breadcrumbs-link' onclick='dailyWinLoss2hSoccerMinorBettypes.BackToReport("
                        + reportLevelEnums.Main
                        + ");'>"
                        + viewResources.Main.toUpperCase() + "</a> > " +
                        self.Cached.SportName.toUpperCase() +
                         " \"" + viewResources.Live.toUpperCase() + "\"" +
                         " " + viewResources.DailyWinLoss2h_ReportTitleSuffix.toUpperCase(),
            showExcelButton: false
        };

        generateShowHideCell(gridOptions, 9999);
        DOMElements.gridSoccerLiveContainer.removeClass('display-none');
        DOMElements.gridSoccerLive.removeClass('display-none');
        DOMElements.gridSoccerLive.fanexGrid(gridOptions);
        DOMElements.gridSoccerLive.find('[class^="data-group-"]').addClass('display-none');
        DOMElements.gridSoccerLive.fanexGrid('resizeGrid');
        self.ExpandCollapseAll(isExpand);
    };

    self.RenderSoccerNonLiveReport = function (gridOptions) {
        gridOptions.numberOfFrozenColumn = 0;
        gridOptions.gridWidth = 1200;
        gridOptions.gridExpandHeight = 200;
        gridOptions.noDataText = viewResources.EmptyReportMessage;
        gridOptions.headerColumnHeight = 17;
        gridOptions.titleInfo = {
            /*jshint camelcase: false */
            text: "<a class='breadcrumbs-link' onclick='dailyWinLoss2hSoccerMinorBettypes.BackToReport("
                        + reportLevelEnums.Main + ");'>"
                        + viewResources.Main.toUpperCase() + "</a> > " +
                        self.Cached.SportName.toUpperCase() +
                         " \"" + viewResources.Symbol_Sparse_Non_Live.toUpperCase() + "\"" +
                         " " + viewResources.DailyWinLoss2h_ReportTitleSuffix.toUpperCase(),
            showExcelButton: false
        };

        generateShowHideCell(gridOptions, 9998);
        DOMElements.gridSoccerNonLiveContainer.removeClass('display-none');
        DOMElements.gridSoccerNonLive.removeClass('display-none');
        DOMElements.gridSoccerNonLive.fanexGrid(gridOptions);
        DOMElements.gridSoccerNonLive.find('[class^="data-group-"]').addClass('display-none');
        DOMElements.gridSoccerNonLive.fanexGrid('resizeGrid');
        self.ExpandCollapseAll(isExpand);
    };

    self.GenerateBettypeReport = function (bettype, liveindicator, ishedge, bettypename) {
        AccReport.disableInput();
        reportLevel = reportLevelEnums.Bettype;
        if (!isCached) {
            self.GetParams(bettype, liveindicator, ishedge, bettypename);
        }

        AccReport.utility.CallAjax(this,
                                    window.rootUrl + "Main/DailyWinLoss2hSoccerMinorBettype/GenerateBettypeReport",
                                    self.Params.bettypeReportParams,
                                    self.RenderBettypeReport,
                                    AccReport.enableInput,
                                    AccReport.enableInput);
    };

    self.RenderBettypeReport = function (gridOptions) {
        gridOptions.numberOfFrozenColumn = 0;
        gridOptions.gridWidth = 1200;
        gridOptions.gridExpandHeight = 200;
        gridOptions.noDataText = viewResources.EmptyReportMessage;
        gridOptions.headerColumnHeight = 17;
        gridOptions.titleInfo = {
            /*jshint camelcase: false */
            text: "<a class='link' onclick='dailyWinLoss2hSoccerMinorBettypes.BackToReport(" + reportLevelEnums.Main + ");'>"
                    + viewResources.Main + "</a>" +
                    " > <a class='link' onclick='dailyWinLoss2hSoccerMinorBettypes.BackToReport(" + reportLevelEnums.Soccer + ");'>"
                    + self.Cached.SportName.toUpperCase() + "</a>" +
                    " > " + this.Params.bettypeReportParams.BettypeName.toUpperCase() +
                    " \"" + (this.Params.bettypeReportParams.Liveindicator ?
                        viewResources.Live.toUpperCase() :
                        viewResources.Symbol_Sparse_Non_Live.toUpperCase()) + "\"",
            showExcelButton: false
        };

        hideAllGrid();
        DOMElements.gridBettypeContainer.removeClass('display-none');
        DOMElements.gridBettype.removeClass('display-none');
        DOMElements.gridBettype.fanexGrid(gridOptions);
        DOMElements.gridBettype.find('[class^="data-group-"]').addClass('display-none');
        DOMElements.gridBettype.fanexGrid('resizeGrid');
        DOMElements.btnShowHideSiteTypeInfo.hide();
        if (gridOptions.bodyRows !== undefined && gridOptions.bodyRows.length > 0) {
            DOMElements.btnExport.attr('disabled', null);
            self.ExportExcel();
        }

        self.ExpandCollapseAll(isExpand);
    };

    self.GenerateSiteReport = function (bettype, liveindicator, ishedge, site, bettypename) {
        AccReport.disableInput();
        reportLevel = reportLevelEnums.Site;
        if (!isCached) {
            self.GetParams(bettype, liveindicator, ishedge, site, bettypename);
        }

        AccReport.utility.CallAjax(this,
                                    window.rootUrl + "Main/DailyWinLoss2hSoccerMinorBettype/GenerateSiteReport",
                                    self.Params.siteReportParams,
                                    self.RenderSiteReport,
                                    AccReport.enableInput,
                                    AccReport.enableInput);
    };

    self.RenderSiteReport = function (gridOptions) {
        groupIndex = 0;
        gridOptions.numberOfFrozenColumn = 0;
        gridOptions.gridWidth = 1200;
        gridOptions.gridExpandHeight = 200;
        gridOptions.noDataText = viewResources.EmptyReportMessage;
        gridOptions.headerColumnHeight = 17;
        gridOptions.titleInfo = {
            /*jshint camelcase: false */
            text: "<a class='link' onclick='dailyWinLoss2hSoccerMinorBettypes.BackToReport(" + reportLevelEnums.Main + ");'>"
                    + viewResources.Main.toUpperCase() + "</a>" +
                    " > <a class='link' onclick='dailyWinLoss2hSoccerMinorBettypes.BackToReport(" + reportLevelEnums.Soccer + ");'>"
                    + self.Cached.SportName.toUpperCase() + "</a>" +
                    " > " + this.Params.siteReportParams.BettypeName.toUpperCase() +
                    " \"" + (this.Params.siteReportParams.Liveindicator ?
                        viewResources.Live.toUpperCase() :
                        viewResources.Symbol_Sparse_Non_Live.toUpperCase()) + "\""
                        + " | "
                        + AccReport.utility.StringFormat(
                            viewResources.DailyWinLoss2h_WinLossBySite_BetType_ReportTitle.toUpperCase(),
                            self.Params.siteReportParams.Site === 1 ? ["CREDIT SITE"] : ["DEPOSIT SITE"]),
            showExcelButton: false
        };

        if (gridOptions.bodyRows !== undefined && gridOptions.bodyRows.length > 0) {
            DOMElements.btnShowHideSiteTypeInfo.hide();
            DOMElements.btnExport.attr('disabled', null);
            self.ExportExcel();
        }

        hideAllGrid();
        DOMElements.gridSiteContainer.removeClass('display-none');
        DOMElements.gridSite.removeClass('display-none');
        DOMElements.gridSite.fanexGrid(gridOptions);
        DOMElements.gridSite.fanexGrid('resizeGrid');
        self.ExpandCollapseAll(isExpand);
    };

    self.ExportExcel = function () {
        DOMElements.btnExport.unbind('click');
        DOMElements.btnExport.click(function () {
            switch (reportLevel) {
                case reportLevelEnums.Main:
                    AccReport.downloadFile(window.rootUrl + "Main/DailyWinLoss2hSoccerMinorBettype/ExportExcelMainReport", self.Params.mainReportParams);
                    break;
                case reportLevelEnums.Soccer:
                    AccReport.downloadFile(window.rootUrl + "Main/DailyWinLoss2hSoccerMinorBettype/ExportExcelSoccerReport", self.Params.sporttypeReportParams);
                    break;
                case reportLevelEnums.Bettype:
                    AccReport.downloadFile(window.rootUrl + "Main/DailyWinLoss2hSoccerMinorBettype/ExportExcelBettypeReport", self.Params.bettypeReportParams);
                    break;
                case reportLevelEnums.Site:
                    AccReport.downloadFile(window.rootUrl + "Main/DailyWinLoss2hSoccerMinorBettype/ExportExcelSiteReport", self.Params.siteReportParams);
                    break;
                default:
                    break;
            }

        });
    };

    self.BackToReport = function (currentReportLevel) {
        reportLevel = currentReportLevel;
        switch (reportLevel) {
            case reportLevelEnums.Main:
                hideAllGrid();
                DOMElements.gridMainContainer.removeClass('display-none');
                DOMElements.gridMain.removeClass('display-none');
                DOMElements.gridMain.fanexGrid('resizeGrid');
                DOMElements.btnShowHideSiteTypeInfo.show();
                break;
            case reportLevelEnums.Soccer:
                hideAllGrid();
                DOMElements.gridSoccerLiveContainer.removeClass('display-none');
                DOMElements.gridSoccerLive.removeClass('display-none');
                DOMElements.gridSoccerLive.fanexGrid('resizeGrid');
                DOMElements.gridSoccerNonLiveContainer.removeClass('display-none');
                DOMElements.gridSoccerNonLive.removeClass('display-none');
                DOMElements.gridSoccerNonLive.fanexGrid('resizeGrid');
                DOMElements.btnShowHideSiteTypeInfo.show();
                break;
            case reportLevelEnums.Bettype:
                hideAllGrid();
                DOMElements.gridBettypeContainer.removeClass('display-none');
                DOMElements.gridBettype.removeClass('display-none');
                DOMElements.gridBettype.fanexGrid('resizeGrid');
                DOMElements.btnShowHideSiteTypeInfo.hide();
                break;
            case reportLevelEnums.Site:
                hideAllGrid();
                DOMElements.gridSiteContainer.removeClass('display-none');
                DOMElements.gridSite.removeClass('display-none');
                DOMElements.gridSite.fanexGrid('resizeGrid');
                DOMElements.btnShowHideSiteTypeInfo.hide();
                break;
            default:
                break;
        }
    };

    self.ExpandCollapseAll = function (isExpand) {
        switch (reportLevel) {
            case reportLevelEnums.Main:
                DOMElements.gridMain.find('span[title="Collapse/Expand"]').each(function () {
                    var $this = $(this);
                    if ((isExpand && !$this.hasClass('expand')) || (!isExpand && $this.hasClass('expand'))) {
                        $this.click();
                    }
                });
                break;
            case reportLevelEnums.Soccer:
                DOMElements.gridSoccerLive.find('span[title="Collapse/Expand"]').each(function () {
                    var $this = $(this);
                    if ((isExpand && !$this.hasClass('expand')) || (!isExpand && $this.hasClass('expand'))) {
                        $this.click();
                    }
                });

                DOMElements.gridSoccerNonLive.find('span[title="Collapse/Expand"]').each(function () {
                    var $this = $(this);
                    if ((isExpand && !$this.hasClass('expand')) || (!isExpand && $this.hasClass('expand'))) {
                        $this.click();
                    }
                });
                break;
            case reportLevelEnums.Bettype:
                break;
            case reportLevelEnums.Site:
                break;
            default:
                break;
        }
    };

    self.ShowHideDataGroup = function (element, dataGroupIndex) {
        if (element !== undefined) {
            var $element = $(element);
            $element.toggleClass('collapse');
            $element.toggleClass('expand');
        }

        var gridElement;
        switch (reportLevel) {
            case reportLevelEnums.Main:
                gridElement = DOMElements.gridMain;
                break;
            case reportLevelEnums.Soccer:
                gridElement = DOMElements.gridSoccerLive;
                gridElement.find(".data-group-" + dataGroupIndex).each(function () {
                    var $this = $(this);
                    $this.toggleClass('display-none');
                });

                gridElement.fanexGrid('resizeGrid');
                gridElement = DOMElements.gridSoccerNonLive;
                break;
            case reportLevelEnums.Bettype:
                break;
            case reportLevelEnums.Site:
                break;
            default:
                break;
        }

        gridElement.find(".data-group-" + dataGroupIndex).each(function () {
            var $this = $(this);
            $this.toggleClass('display-none');
        });

        gridElement.fanexGrid('resizeGrid');
    };

    var generateShowHideCell = function (gridOptions, footerGroupIndex) {
        var html = "";
        var i = 0;
        var value = "";
        if (gridOptions.bodyRows !== undefined && gridOptions.bodyRows.length > 0) {
            var bodyRowsCount = gridOptions.bodyRows.length;
            for (i = 0; i < bodyRowsCount; i++) {
                var bodyRow = gridOptions.bodyRows[i];
                if (bodyRow.CssClass.indexOf('freegrid-even') !== -1) {
                    value = bodyRow.Columns[0].Value;
                    html = viewResources.SportTypeColumnPattern.format(
                    "0",
                    "dailyWinLoss2hSoccerMinorBettypes.ShowHideDataGroup(this, " + groupIndex++ + ")",
                    "dailyWinLoss2hSoccerMinorBettypes." + getDrillDownFunction(bodyRow),
                    value);
                    bodyRow.Columns[0].Value = html;
                }
                else if (reportLevel === reportLevelEnums.Soccer) {
                    value = bodyRow.Columns[0].Value;
                    var bettype = getCustomAttrValue(bodyRow.Columns[0].CustomAttr, 'bettype');
                    var liveIndicator = getCustomAttrValue(bodyRow.Columns[0].CustomAttr, 'liveindicator');
                    var isHedge = getCustomAttrValue(bodyRow.Columns[0].CustomAttr, 'ishedge') === '' ? 0 : 1;
                    var bettypeName = getCustomAttrValue(bodyRow.Columns[0].CustomAttr, 'bettypename');
                    var site = value === "Credit" ? 1 : 2;
                    html = "<span class='link bold' onclick='dailyWinLoss2hSoccerMinorBettypes.GenerateSiteReport("
                        + bettype + ","
                        + liveIndicator + ","
                        + isHedge + ","
                        + site + ",\""
                        + bettypeName + "\")'>" + value + "</span>";
                    bodyRow.Columns[0].Value = html;
                }
            }
        }

        if (gridOptions.footerRows !== undefined && gridOptions.footerRows.length > 0) {
            var footerRowsCount = gridOptions.footerRows.length;
            for (i = 0; i < footerRowsCount; i++) {
                var footerRow = gridOptions.footerRows[i];
                if (footerRow.CssClass.indexOf('freegrid-even') !== -1) {
                    value = footerRow.Columns[0].Value;
                    html = viewResources.SportTypeColumnPattern.format(
                    "0",
                    "dailyWinLoss2hSoccerMinorBettypes.ShowHideDataGroup(this, " + footerGroupIndex + ")",
                    "",
                    value);
                    html = html.replace('class="link bold"', 'class="right bold"');
                    footerRow.Columns[0].Value = html;
                }
            }
        }
    };

    var getDrillDownFunction = function (dataRow) {
        switch (reportLevel) {
            case reportLevelEnums.Main:
                return "GenerateSoccerReport()";
            case reportLevelEnums.Soccer:
                var bettype = getCustomAttrValue(dataRow.Columns[0].CustomAttr, 'bettype');
                var liveIndicator = getCustomAttrValue(dataRow.Columns[0].CustomAttr, 'liveindicator');
                var isHedge = getCustomAttrValue(dataRow.Columns[0].CustomAttr, 'ishedge');
                var bettypeName = getCustomAttrValue(dataRow.Columns[0].CustomAttr, 'bettypename');
                return "GenerateBettypeReport(" + bettype + "," + liveIndicator + ",'" + isHedge + "','" + bettypeName + "')";
            case reportLevelEnums.Bettype:
                break;
            case reportLevelEnums.Site:
                break;
            default:
                break;
        }
    };

    var hideAllGrid = function () {
        DOMElements.gridMainContainer.addClass('display-none');
        DOMElements.gridMain.addClass('display-none');
        DOMElements.gridSoccerLiveContainer.addClass('display-none');
        DOMElements.gridSoccerLive.addClass('display-none');
        DOMElements.gridSoccerNonLiveContainer.addClass('display-none');
        DOMElements.gridSoccerNonLive.addClass('display-none');
        DOMElements.gridBettypeContainer.addClass('display-none');
        DOMElements.gridBettype.addClass('display-none');
        DOMElements.gridSiteContainer.addClass('display-none');
        DOMElements.gridSite.addClass('display-none');
    };

    var getCustomAttrValue = function (attrString, key) {
        return $('<p ' + attrString + '></p>').attr(key);
    };
};

$(document).ready(function () {
    dailyWinLoss2hSoccerMinorBettypes = new DailyWinLoss2hSoccerMinorBettypes();
    dailyWinLoss2hSoccerMinorBettypes.Init();
});