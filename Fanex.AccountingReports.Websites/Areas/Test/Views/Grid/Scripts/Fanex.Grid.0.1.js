// --------------------------------------------------------------------------
// <copyright file="Fanex.Grid.js" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------

(function () {

    //// Method expose for user use
    var methods = {
        createNexGrid: function (opts) {
            opts = $.extend({}, $.fn.nexGrid.defaults, opts);
            return this.each(function () {
                return new NexGrid(this, opts);
            });
        },
        reRender: function (opts) {
            $(this).html("");
            opts = $.extend({}, $.fn.nexGrid.defaults, opts);
            return this.each(function () {
                return new NexGrid(this, opts);
            });
        }
    };


    $.fn.nexGrid = function (options) {
        if (methods[options]) {
            return methods[options].apply(this, Array.prototype.slice.call(arguments, 1));
        }
        else if (typeof options === 'object' || !options) {
            return methods.createNexGrid.apply(this, arguments);
        }
        else {
            //throw error
        }
    };

    $.fn.nexGrid.defaults = {
        //// nexGridType can be single, daterange, quickchoose
        nexGridType: "single",
        customizeHeader: false,
        headerRows: [],
        columns: [],
        bodyRows: [],
        numberOfFrozenColumn: 0,
        showFooter: false,
        footerRows: [],
        showTitle: false,
        headerColumnHeight: 25,
        bodyRowHeight: 25,
        footerRowHeight: 25,
        titleInfo:
        {
            text: "",
            showExcelButton: false,
            exportExcelFunction: null,
            showBackButton: false,
            backFunction: null,
            showSendMailButton: false,
            sendMailFunction: null,
            showCollapseExpandButton: false
        },
        showPager: false,
        pagerOption: null,
        pagerOption: {
            itemsPerPage: 100,
            maxItemsPerPage: 500,
            currentPage: 1,
            totalItem: 0,
            startPerPage: 100,
            pageStep: 50
        },
        showTree: false
    };

    var NexGrid = function (grid, options) {
        this.Options = options;
        this.$grid = $(grid);
        this.gridId = this.$grid.attr("id");
        this.tableContentId = "tblContent_" + this.gridId;
        this.frozenContentId = "frozenContent_" + this.gridId;

        this.columnsHastable = nexGridHelper.GetColumnsHastable(options.columns);
        this.frozenColsName = [];
        this.columnsWidth = nexGridHelper.GetArrayWidth(options.columns);
        if (options.numberOfFrozenColumn > 0) {
            for (var colIndex = 0; colIndex < options.numberOfFrozenColumn; colIndex++) {
                this.frozenColsName.push(options.columns[colIndex].Name);
            }
        }

        ////
        this.Options.pagerOption.totalItem = this.Options.bodyRows.length;

        //// Render Table Main Frame
        var html = this.RenderTitle() + this.RenderBody(this.RenderHeader()) + (options.showFooter ? this.RenderFooter() : "")
        this.$grid.html(html);

        this.gridOptions = null;

        this.Initialize();

        return this.$grid;
    };

    NexGrid.prototype = {
        Initialize: function () {
            var me = this;
            var options = this.Options;

            //// Render Grid
            me.gridOptions = {
                tblContentId: me.tableContentId,
                fullHeight: true,
                column: me.columnsWidth,
                gridHeight: 50,
                numberFreezeCol: options.numberOfFrozenColumn,

            };
            $(this.$grid).initGrid({
                isShowPager: this.Options.showPager
            }).gridScroller(me.gridOptions);

            //// Render Pager
            if (this.Options.showPager == true) {
                this.RenderPager();
            }

            this.RegisterEvents();

        },
        RenderTitle: function () {
            var me = this;
            var options = me.Options;

            if (options.showTitle == false) {
                return '';
            }
            else {
                var titleHtml = [];
                titleHtml.push('<div class="grid-title display-none"><div class="grid-title-label" id="reporttile">');
                titleHtml.push(options.titleInfo.text);
                titleHtml.push('</div>');
                titleHtml.push(' <div class="grid-toolbox">');
                if (options.titleInfo.showBackButton == true) {
                    titleHtml.push('<input id="btnBack' + me.gridId + '" class="btnBack" type="button" title="Back" value="" name="btnBack" />');
                }
                if (options.titleInfo.showCollapseExpandButton == true) {
                    titleHtml.push('<input class="btExpand expanded" id="btnCollapseExpand' + me.gridId + '" type="button" value="" title="Collapse/Expand">');
                }
                if (options.titleInfo.showExcelButton == true) {
                    titleHtml.push('<input type="button" id="btnExport' + me.gridId + '" class="btnExcel" title="Export to Excel"/>');

                }
                if (options.titleInfo.showSendMailButton == true) {
                    titleHtml.push('<input id="btnSendMail' + me.gridId + '" class="btnSendMail" type="button" title="Send Mail">')
                }
                titleHtml.push('</div></div>');
                return titleHtml.join("");
            }
        },
        RenderCommonHeader: function (type) {
            var me = this;
            var options = this.Options;
            var divHeaderClass = "";
            var divContainerClass = "";
            var tableClass = "";
            var hasId = false;
            var tableContainId = me.tableContentId;
            var columns = options.columns.slice(0);
            var columnLength = columns.length;
            var replaceMark = "_";

            switch (type) {
                case "freeheader":
                    divContainerClass = "free-cell free-top-left";
                    tableClass = "grid-fixed-table free-cell-top-left";
                    columnLength = options.numberOfFrozenColumn;
                    break;
                case "frozenheader":
                    divHeaderClass = "row_header_wrapper";
                    divContainerClass = "row_header row-headers-background";
                    tableClass = "grid-fixed-table row_header_content";
                    columnLength = options.numberOfFrozenColumn;
                    hasId = true;
                    tableContainId = me.frozenContentId;
                    replaceMark = "frozenBody";
                    break;
                case "header":
                    divHeaderClass = "col_header row-headers-background scroll-y";
                    divContainerClass = "header-container";
                    tableClass = "grid-fixed-table col_header_content";
                    break;
                case "body":
                    divHeaderClass = "grid-container container";
                    divContainerClass = "content-container";
                    tableClass = "grid-fixed-table col_body_content";
                    hasId = true;
                    break;
            }

            var headerHtml = [];
            var trsHeader = [];
            //// Build Header
            if (divHeaderClass != "") {
                headerHtml.push("<div class='" + divHeaderClass + "'>");
            }
            headerHtml.push("<div class='" + divContainerClass + "'>");
            headerHtml.push("<table class='" + tableClass + "' "
            + (hasId ? ("id='" + tableContainId + "'") : "")
            + " ><thead>");

            if (options.customizeHeader) {
                var currentGroupHeader = options.cloneGroupHeaderRows;
                if (type == "freeheader" || type == "frozenheader") {
                    currentGroupHeader = nexGridHelper.CreateRowsForFrozenColumn(options.cloneGroupHeaderRows.slice(0), options.numberOfFrozenColumn, me.frozenColsName);
                }
                for (var rowIndex = 0; rowIndex < currentGroupHeader.length; rowIndex++) {
                    var currentRow = currentGroupHeader[rowIndex];
                    if (currentRow.Columns.length > 0) {
                        headerHtml.push("<tr>");
                        for (var colIndex = 0; colIndex < currentRow.Columns.length && colIndex < columnLength; colIndex++) {
                            var currentCol = currentRow.Columns[colIndex];
                            if (isValid(currentCol)) {
                                var customSpan = "";
                                var currentHeight = options.headerColumnHeight;
                                if (isValid(currentCol) && isValid(currentCol.MergeRowNumber)) {
                                    nexGridHelper.RemoveItemForRowSpan(currentGroupHeader, rowIndex, currentCol.MergeRowNumber, currentCol.Name);
                                    var currentMergeRowNumber = " rowspan='" + currentCol.MergeRowNumber + "'";
                                    if (type == "freeheader" || type == "frozenheader") {
                                        currentHeight = ((options.headerColumnHeight * currentCol.MergeRowNumber) + (currentCol.MergeRowNumber - 1) * 5);
                                        currentMergeRowNumber = "";
                                    }
                                    customSpan += currentMergeRowNumber;
                                }

                                if (isValid(currentCol) && isValid(currentCol.MergeColNumber)) {
                                    customSpan += " colspan='" + currentCol.MergeColNumber + "'";
                                    currentCol.Width = nexGridHelper.GetColMergeWidth(me.columnsWidth, me.columnsHastable[currentCol.Name].ColumnIndex, currentCol.MergeColNumber);
                                }
                                var cellInfo =
                                {
                                    CellTag: "th",
                                    CustomSpan: customSpan,
                                    CssClass: currentCol.CssClass,
                                    SortClass: (currentRow.Columns[colIndex].Sort == true ? " colSorter" : ""),
                                    CellWidth: (currentCol.Width || options.columns[colIndex].Width),
                                    CellHeight: currentHeight,
                                    CellId: (me.columnsHastable[currentCol.Name].Sort == true ? (me.gridId + "_header_sort_" + currentCol.Name) : ""),
                                    Sort: currentRow.Columns[colIndex].Sort,
                                    CellColumnName: currentCol.Name,
                                    Text: currentCol.Text,
                                    CustomRender: currentCol.CustomRender || options.columns[colIndex].CustomRender,
                                    SortDirection: currentRow.Columns[colIndex].SortDirection || ""
                                };
                                trsHeader.push(me.RenderHeaderCell(cellInfo, currentCol));
                            }

                        }
                        headerHtml.push(trsHeader.join(""));
                        headerHtml.push("</tr>");
                        trsHeader = [];
                    }
                }
            }
            else {
                headerHtml.push("<tr>");
                for (var colIndex = 0; colIndex < columnLength; colIndex++) {
                    var currentCol = options.columns[colIndex];

                    var cellInfo =
                                {
                                    CellTag: "th",
                                    CustomSpan: "",
                                    CssClass: currentCol.CssClass,
                                    SortClass: (currentCol.Sort == true ? " colSorter" : ""),
                                    CellWidth: (currentCol.Width || options.columns[colIndex].Width),
                                    CellHeight: options.headerColumnHeight,
                                    CellId: (me.columnsHastable[currentCol.Name].Sort == true ? (me.gridId + "_header_sort_" + currentCol.Name) : ""),
                                    Sort: currentCol.Sort,
                                    CellColumnName: currentCol.Name,
                                    Text: currentCol.Text,
                                    CustomRender: currentCol.CustomRender || options.columns[colIndex].CustomRender,
                                    SortDirection: currentCol.SortDirection || ""
                                };
                    trsHeader.push(me.RenderHeaderCell(cellInfo, currentCol));
                }
                headerHtml.push(trsHeader.join(""));
                headerHtml.push("</tr>");
            }

            headerHtml.push("</thead><tbody>{$}" + replaceMark + "</tbody></table>");
            headerHtml.push("</div>");
            if (divHeaderClass != "") {
                headerHtml.push("</div>");
            }
            return headerHtml.join("");
        },
        RenderFrozenRow: function (currentRow, colHtmlTag) {
            var me = this;
            var options = me.Options;
            var frozenRowHtml = [];
            var colsLength = currentRow.Columns.length;
            frozenRowHtml.push('<tr class="' + (currentRow.CssClass || "") + '">');
            for (var columnIndex = 0; columnIndex < colsLength; columnIndex++) {
                var currentCol = currentRow.Columns[columnIndex];

                var customSpan = "";
                var currentHeight = "height:" + options.bodyRowHeight + "px;";
                var customClass = "table-cell";
                if (isValid(currentCol.MergeRowNumber)) {
                    customSpan += " rowspan='" + currentCol.MergeRowNumber + "'";
                    if (nexGridHelper.CheckHideColSpan(currentRow.Columns)) {
                        currentHeight = "height:" + ((options.bodyRowHeight * currentCol.MergeRowNumber) + (currentCol.MergeRowNumber - 1) * 5) + "px;";
                        customSpan = "";
                    }
                }

                if (isValid(currentCol.MergeColNumber)) {
                    customSpan += " colspan='" + currentCol.MergeColNumber + "'";

                }

                var cellContent = typeof currentCol.CustomRender == 'function' ? currentCol.CustomRender(currentCol) : currentCol.Value;
                if (me.columnsHastable[currentCol.Name].FormatNumber) {
                    cellContent = numeral(cellContent).format(me.columnsHastable[currentCol.Name].FormatNumber);
                }

                frozenRowHtml.push('<' + colHtmlTag + '' + customSpan + ' >');
                frozenRowHtml.push('<div class="' + (currentCol.CssClass || "") + " " + customClass + '" style="width:' + me.columnsHastable[currentCol.Name].Width + 'px;' + currentHeight + 'text-align:' + (me.columnsHastable[currentCol.Name].CellAlign || 'left') + '">');
                frozenRowHtml.push(cellContent);
                frozenRowHtml.push('</div></' + colHtmlTag + '>');
            }
            frozenRowHtml.push('</tr>');
            return frozenRowHtml.join("");
        },
        RenderNormalRow: function (currentRow, colHtmlTag, rowIndex, treeCollapseExpandClass) {
            var me = this;
            var options = me.Options;
            var rowHtml = [];
            var colsLength = currentRow.Columns.length;
            var currentCollapseExpandId = "";
            var parentRowClass = "";
            if (colsLength > 0) {
                if (options.showTree == true && currentRow.SubRows) {
                    parentRowClass = "parentRow";
                }
                rowHtml.push('<tr class="' + (currentRow.CssClass || "") + (treeCollapseExpandClass || "") + ' ' + parentRowClass + '">');
                for (var colIndex = 0; colIndex < colsLength; colIndex++) {
                    var currentCol = currentRow.Columns[colIndex];
                    var currentWidth = me.columnsHastable[currentCol.Name].Width;
                    var customSpan = "";
                    var currentHeight = "height:" + options.bodyRowHeight + "px;";
                    if (isValid(currentCol.MergeRowNumber)) {
                        customSpan += " rowspan='" + currentCol.MergeRowNumber + "'";

                    }

                    if (isValid(currentCol.MergeColNumber)) {
                        customSpan += " colspan='" + currentCol.MergeColNumber + "'";
                        currentWidth = nexGridHelper.GetColMergeWidth(me.columnsWidth, me.columnsHastable[currentCol.Name].ColumnIndex, currentCol.MergeColNumber);
                    }

                    var cellContent = typeof currentCol.CustomRender == 'function' ? currentCol.CustomRender(currentCol) : currentCol.Value;

                    if (me.columnsHastable[currentCol.Name].FormatNumber) {
                        cellContent = numeral(cellContent).format(me.columnsHastable[currentCol.Name].FormatNumber);
                    }

                    var trClass = "";
                    if (options.showTree == true && currentRow.SubRows && me.columnsHastable[currentCol.Name].ShowCollapseExpand == true) {
                        trClass = "trgridcollapseexpand";
                    }

                    rowHtml.push('<' + colHtmlTag + ' ' + customSpan + ' class="' + trClass + '" >');
                    rowHtml.push('<div class="table-cell ' + (currentCol.CssClass || "") + '" style="width:' + currentWidth + 'px;' + currentHeight + 'text-align:' + (me.columnsHastable[currentCol.Name].CellAlign || 'left') + ';">');
                    if (options.showTree == true && currentRow.SubRows && me.columnsHastable[currentCol.Name].ShowCollapseExpand == true) {
                        currentCollapseExpandId = me.gridId + '_tree_' + rowIndex;
                        rowHtml.push('<span class="gridcollapseexpand gridexpand" id="' + currentCollapseExpandId + '"></span>');
                        currentCollapseExpandId += " childRow";
                    }
                    rowHtml.push(cellContent);
                    rowHtml.push('</div></' + colHtmlTag + '>');


                }
                rowHtml.push('</tr>');
                if (options.showTree == true && currentRow.SubRows) {
                    var SubRowsLength = currentRow.SubRows.length;
                    for (var subRowIndex = 0; subRowIndex < SubRowsLength; subRowIndex++) {
                        var currentSubRow = currentRow.SubRows[subRowIndex];
                        rowHtml.push(me.RenderNormalRow(currentSubRow, "td", 0, currentCollapseExpandId));
                    }
                }
            }

            return rowHtml.join("");
        },
        RenderHeaderCell: function (cellInfo, currentColumn) {
            var cellHtml = [];
            var cellTag = cellInfo.CellTag || "td";
            var customSpan = cellInfo.CustomSpan || " ";
            var cssClass = cellInfo.CssClass || " ";
            var sortClass = cellInfo.SortClass || " ";
            var cellWidth = "width:" + (cellInfo.CellWidth || "200") + "px;";
            var cellHeight = isValid(cellInfo.CellHeight) ? ("height:" + (cellInfo.CellHeight || "25") + "px;") : " ";
            var cellId = isValid(cellInfo.CellId) ? ' id="' + cellInfo.CellId + '"' : " ";
            var sort = cellInfo.Sort || false;
            var cellColumnName = cellInfo.CellColumnName || "";


            cellHtml.push("<" + cellTag + " " + customSpan + " >");
            cellHtml.push("<div class='table-cell "
                                                + cssClass
                                                + sortClass
                                                + "' style='"
                                                + cellWidth
                                                + cellHeight
                                                + "' "
                                                + cellId
                                                + ">");
            if (sort == true) {
                cellHtml.push('<div class="display-inline-block">');
            }

            if (typeof cellInfo.CustomRender == 'function') {
                cellHtml.push(cellInfo.CustomRender(currentColumn));
            }
            else {
                cellHtml.push(cellInfo.Text);
            }

            if (sort == true) {
                var sortDirection = cellInfo.SortDirection || "";
                cellHtml.push('</div><div class="divSorter '
                + sortDirection
                + '" data-columnname="'
                + cellColumnName
                + '"> </div>');
            }

            cellHtml.push("</div>")
            cellHtml.push("</" + cellTag + ">");
            return cellHtml.join("");
        },
        RenderHeader: function () {
            var me = this;
            var options = this.Options;
            var headerHtml = [];

            //// Add last column of Group Header
            if (options.customizeHeader) {
                this.Options.cloneGroupHeaderRows = options.groupHeaderRows.clone();
                this.Options.cloneGroupHeaderRows.push({
                    Columns: options.columns.clone()
                });
            }

            //// If there are any item of header column
            var columnLength = options.columns.length;
            if (columnLength > 0) {
                if (options.numberOfFrozenColumn > 0) {
                    headerHtml.push(me.RenderCommonHeader("freeheader").replace("{$}_", ""));
                    headerHtml.push(me.RenderCommonHeader("frozenheader"));
                }
                headerHtml.push(me.RenderCommonHeader("header").replace("{$}_", ""));
            }

            return headerHtml.join("");
        },
        RenderBody: function (headerHtml, reRenderBody) {
            var me = this;
            var options = this.Options;


            var bodyHtml = (reRenderBody == true ? "" : (headerHtml + me.RenderCommonHeader("body")));
            var bodyRowsHtml = [];
            var bodyFrozenHtml = [];


            var currentBodyRows = [];
            if (options.showPager == true
            && options.customPager != true) {
                var startIndex = (options.pagerOption.currentPage * options.pagerOption.itemsPerPage);
                var endIndex = ((options.pagerOption.currentPage + 1) * options.pagerOption.itemsPerPage);
                currentBodyRows = options.bodyRows.slice(startIndex, endIndex);
            } else {
                currentBodyRows = options.bodyRows;
            }

            var bodyLength = currentBodyRows.length;
            var currentFrozenRows = (options.numberOfFrozenColumn > 0) ? nexGridHelper.CreateRowsForFrozenColumn(currentBodyRows, options.numberOfFrozenColumn, me.frozenColsName) : [];
            var frozenBodyLength = currentFrozenRows.length;

            for (var rowIndex = 0; rowIndex < frozenBodyLength; rowIndex++) {
                var currentRow = currentBodyRows[rowIndex];
                var currentFrozenRow = currentFrozenRows[rowIndex];
                bodyRowsHtml.push(me.RenderNormalRow(currentRow, "td", rowIndex));
                bodyFrozenHtml.push(me.RenderFrozenRow(currentFrozenRow, "td", rowIndex));
            }

            for (var rowIndex = frozenBodyLength; rowIndex < bodyLength; rowIndex++) {
                var currentRow = currentBodyRows[rowIndex];
                bodyRowsHtml.push(me.RenderNormalRow(currentRow, "td", rowIndex));
            }

            if (reRenderBody == true) {
                me.$grid.find("#" + me.frozenContentId + "> tbody").empty().html(bodyFrozenHtml.join(""));
                me.$grid.find("#" + me.tableContentId + "> tbody").empty().html(bodyRowsHtml.join(""));
            }
            else {
                bodyHtml = bodyHtml.replace("{$}frozenBody", bodyFrozenHtml.join(""));
                bodyHtml = bodyHtml.replace("{$}_", bodyRowsHtml.join(""));
            }
            return bodyHtml;
        },
        RenderFooter: function () {
            var me = this;
            var options = me.Options;
            var footerHtml = [];
            var frozenFooterHtml = [];

            footerHtml.push('<div class="col_footer row-footer-background scroll-y">'
                                + '<div class="footer-container">'
                                    + '<table class="col_footer_content grid-fixed-table">'
                                       + '<thead>');

            frozenFooterHtml.push(' <div class="row_footer row-footer-background z999">'
                                       + ' <table class="grid-fixed-table row_footer_content">'
                                        + '    <thead>');

            var footerRowsLength = options.footerRows.length;
            var currentFrozenRows = (options.numberOfFrozenColumn > 0) ? nexGridHelper.CreateRowsForFrozenColumn(options.footerRows, options.numberOfFrozenColumn, me.frozenColsName) : [];
            var frozenFooterLength = currentFrozenRows.length;

            //// Render Frozen fotter and main Footer
            for (var rowIndex = 0; rowIndex < frozenFooterLength; rowIndex++) {
                var currentRow = options.footerRows[rowIndex];
                var currentFrozenRow = currentFrozenRows[rowIndex];
                footerHtml.push(me.RenderNormalRow(currentRow, "th"));
                frozenFooterHtml.push(me.RenderFrozenRow(currentFrozenRow, "th"));
            }

            //// Render only main Footer
            for (var rowIndex = frozenFooterLength; rowIndex < footerRowsLength; rowIndex++) {
                var currentRow = options.footerRows[rowIndex];
                footerHtml.push(me.RenderNormalRow(currentRow, "th"));
            }

            frozenFooterHtml.push('   </thead>'
                                + '</table>'
                            + '</div>');

            footerHtml.push(' </thead>'
                        + '</table>'
                    + '</div>'
                + '</div>');

            return frozenFooterHtml.join("") + footerHtml.join("");
        },
        RenderPager: function () {
            var me = this;
            var options = this.Options;
            var pagerOption = {};

            if (options.customPager && options.customPager == true) {
                pagerOption = options.customPagerOption;
            }
            else {
                pagerOption = {
                    callback: function (page_index, itemsPerPage, jq) {
                        options.pagerOption.currentPage = page_index;
                        options.pagerOption.itemsPerPage = itemsPerPage;
                        //methods.reRender(options);
                        //$(me.$grid).gridScroller(options);
                        //$(me.$grid).nexGrid("reRender", options);
                        //$("#" + me.gridId).nexGrid("reRender", options);
                        me.RenderBody("", true);
                        $(me.$grid).gridScroller(me.gridOptions);
                        me.RenderPager();
                    },
                    startPerPage: options.pagerOption.startPerPage,
                    stepPerPage: options.pagerOption.pageStep,
                    maxTtemsPerPage: options.pagerOption.maxItemsPerPage,
                    itemsPerPage: options.pagerOption.itemsPerPage,
                    currentPage: options.pagerOption.currentPage,
                    total_items: options.pagerOption.totalItem
                };
            }

            this.$grid.find("#pagination").fanexPagination(pagerOption.total_items, pagerOption);
        },
        RegisterEvents: function () {
            var me = this;
            var options = me.Options;

            //// For Onclick Title Button
            if (options.showTitle == true) {
                if (typeof options.titleInfo.exportExcelFunction == 'function') {
                    me.$grid
                    .off("click", '#btnExport' + me.gridId, null)
                    .on("click", '#btnExport' + me.gridId, options.titleInfo.exportExcelFunction);
                }
                if (typeof options.titleInfo.backFunction == 'function') {
                    me.$grid
                    .off("click", '#btnBack' + me.gridId, null)
                    .on("click", '#btnBack' + me.gridId, options.titleInfo.backFunction);
                }
                if (typeof options.titleInfo.sendMailFunction == 'function') {
                    me.$grid
                    .off("click", '#btnSendMail' + me.gridId, null)
                    .on("click", '#btnSendMail' + me.gridId, options.titleInfo.sendMailFunction);
                }
                if (options.titleInfo.showCollapseExpandButton == true) {
                    me.$grid
                    .off("click", '#btnCollapseExpand' + me.gridId, null)
                    .on("click", '#btnCollapseExpand' + me.gridId, function () {
                        var $this = $(this);
                        if ($this.hasClass("expanded")) {
                            me.$grid.find(".childRow").hide();
                            me.$grid.find(".gridcollapseexpand").removeClass("gridexpand").addClass("gridcollapse");
                            $this.removeClass("expanded").addClass("collapsed");
                        }
                        else {
                            $this.removeClass("collapsed").addClass("expanded");
                            me.$grid.find(".gridcollapseexpand").removeClass("gridcollapse").addClass("gridexpand");
                            me.$grid.find(".childRow").show();
                        }
                        $(me.$grid).gridScroller(me.gridOptions);
                    });
                }
            }

            //// For Sorting
            var colsLength = options.columns.length;
            for (var colIndex = 0; colIndex < colsLength; colIndex++) {
                var currentCol = options.columns[colIndex];
                if (currentCol.Sort == true) {
                    me.$grid
                    .off("click", '#' + me.gridId + "_header_sort_" + currentCol.Name, null)
                    .on("click", '#' + me.gridId + "_header_sort_" + currentCol.Name, function () {
                        var $this = $(this).find(".divSorter");
                        var currentDirection = "";
                        if ($this.hasClass("desc")) {
                            me.$grid.find(".divSorter").removeClass("asc").removeClass("desc");
                            $this.addClass("asc");
                            currentDirection = "ASC";
                        } else if ($this.hasClass("asc")) {
                            me.$grid.find(".divSorter").removeClass("asc").removeClass("desc");
                            $this.addClass("desc");
                            currentDirection = "DESC";
                        }
                        else {
                            me.$grid.find(".divSorter").removeClass("asc").removeClass("desc");
                            $this.addClass("desc");
                            currentDirection = "DESC";
                        }
                        if (typeof (options.customSortFunction) == 'function') {
                            options.customSortFunction($this.data("columnname"), currentDirection);
                        }
                        else {
                            for (var colIndex = 0; colIndex < options.columns.length; colIndex++) {
                                if (options.columns[colIndex].Name == $this.data("columnname")) {
                                    options.columns[colIndex].SortDirection = currentDirection.toLowerCase();
                                }
                            }
                            options.bodyRows = nexGridHelper.GetSortedArray(options.bodyRows, $this.data("columnname"), currentDirection);
                            me.RenderBody("", true);
                            $(me.$grid).gridScroller(me.gridOptions);
                        }
                    });
                }
            }

            //// Bind collapse expand event
            if (options.showTree == true) {
                me.$grid
                .off("click", ".parentRow > td.trgridcollapseexpand", null)
                .on("click", ".parentRow > td.trgridcollapseexpand", function () {
                    var $this = $(this);
                    var collapseexpandspan = $this.find(".gridcollapseexpand");
                    if (collapseexpandspan.hasClass("gridexpand")) {
                        collapseexpandspan.removeClass("gridexpand").addClass("gridcollapse");
                    }
                    else {
                        collapseexpandspan.removeClass("gridcollapse").addClass("gridexpand");
                    }
                    $("." + collapseexpandspan.attr("id")).toggle();
                    $(me.$grid).gridScroller(me.gridOptions);
                });
            }
        }
    };

    //// Grid Helper 
    var nexGridHelper =
    {
        RemoveItemForRowSpan: function (rows, currentRowIndex, rowSpanSize, columnName) {
            var currentLength = currentRowIndex + rowSpanSize;
            var rowsLength = currentLength < rows.length ? currentLength : rows.length;
            for (var rowIndex = currentRowIndex + 1; rowIndex < rowsLength; rowIndex++) {
                for (var colIndex = 0; colIndex < rows[rowIndex].Columns.length; colIndex++) {
                    var currentCol = rows[rowIndex].Columns[colIndex];
                    if (currentCol.Name == columnName) {
                        if (isValid(currentCol.Sort) && currentCol.Sort == true) {
                            rows[currentRowIndex].Columns[colIndex].Sort = true;
                            rows[currentRowIndex].Columns[colIndex].SortDirection = currentCol.SortDirection;
                        }
                        rows[rowIndex].Columns.remove(colIndex);
                    }
                }
            }
        },
        CreateRowsForFrozenColumn: function (mainRows, frozenColNumber, frozenColsName) {
            var frozenRows = [];
            var rowLength = mainRows.length;
            for (var rowIndex = 0; rowIndex < rowLength; rowIndex++) {

                var currentRow = mainRows[rowIndex];

                var newRow = clone(currentRow);
                newRow.Columns = [];
                var isset = false;
                var colsLength = currentRow.Columns.length < frozenColNumber ? currentRow.Columns.length : frozenColNumber;
                for (var colIndex = 0; colIndex < colsLength; colIndex++) {
                    var currentCol = currentRow.Columns[colIndex];
                    if (nexGridHelper.CheckExistName(frozenColsName, currentCol.Name)) {
                        newRow.Columns.push(clone(currentRow.Columns[colIndex]));
                        isset = true;
                    }
                    else if (isValid(prevCol.MergeCol)
                    && prevCol.MergeCol == true
                    && prevCol.MergeColNumber + colIndex <= colsLength) {

                    }
                    var prevCol = currentCol;
                }
                if (isset) {
                    frozenRows.push(newRow);
                }
            }

            return frozenRows;
        },
        CheckExistName: function (frozenColsName, name) {
            for (var colIndex = 0; colIndex < frozenColsName.length; colIndex++) {
                if (frozenColsName[colIndex] == name) {
                    return true;
                }
            }
            return false;
        },
        CheckExistColumnNameOnNextRows: function (frozenRows, numberMergeRow, startIndex, colName) {
            var frozenRowsLength = frozenRows.length < (startIndex + numberMergeRow) ? frozenRows.length : (startIndex + numberMergeRow);
            for (var startIndex = 0; rowIndex < frozenRowsLength; rowIndex++) {
                if (frozenRows[startIndex].Name == colName) {
                    return true;
                }
            }
            return false;
        },
        CheckHideColSpan: function (currentFrozenColums) {
            var colsLength = currentFrozenColums.length;
            for (var colIndex = 0; colIndex < colsLength; colIndex++) {
                if (!isValid(currentFrozenColums[colIndex].MergeRowNumber)) {
                    return false;
                }
            }
            return true;
        },
        GetArrayWidth: function (columns) {
            var arrayWidth = [];
            var columnLength = columns.length;
            for (var columnIndex = 0; columnIndex < columnLength; columnIndex++) {
                var currentColumn = columns[columnIndex];
                arrayWidth.push(currentColumn.Width);
            }
            return arrayWidth;
        },
        GetColMergeWidth: function (arrayWidth, startIndex, colMergeNumber) {
            var colWidth = 0;
            var colLength = arrayWidth.length < (startIndex + colMergeNumber) ? arrayWidth.length : (startIndex + colMergeNumber);
            for (var colIndex = startIndex; colIndex < colLength; colIndex++) {
                colWidth += arrayWidth[colIndex];
            }
            colWidth += ((colMergeNumber - 1) * 5);
            return colWidth;
        },
        GetColumnWidth: function (columns, name) {
            var colLength = columns.length;
            for (var colIndex = 0; colIndex < colLength; colIndex++) {
                if (columns[colIndex].Name == name) {
                    return columns[colIndex].Width;
                }
            }
            return 0;
        },
        GetColumnsHastable: function (columns) {
            var hastable = {};
            var colLength = columns.length;
            for (var colIndex = 0; colIndex < colLength; colIndex++) {
                var currentCol = columns[colIndex];
                hastable[currentCol.Name] = currentCol;
                hastable[currentCol.Name].ColumnIndex = colIndex;
            }
            return hastable;
        },
        GetSortedArray: function (rows, sortField, sortDirection) {
            if (sortDirection == 'ASC') {
                return rows.sort(function (row1, row2) {
                    return nexGridHelper.CompareRow(row1, row2, sortField, function (col1Value, col2Value) {
                        return col1Value > col2Value ? 1 : -1;
                    });
                });
            }
            else {
                return rows.sort(function (row1, row2) {
                    return nexGridHelper.CompareRow(row1, row2, sortField, function (col1Value, col2Value) {
                        return col1Value < col2Value ? 1 : -1;
                    });
                });
            }
        },
        CompareRow: function (row1, row2, sortField, compareFunction) {
            var col1Length = row1.Columns.length;
            var col2Length = row2.Columns.length;
            var col1Value = "";
            var col2Value = "";
            for (var col1Index = 0; col1Index < col1Length; col1Index++) {
                if (row1.Columns[col1Index].Name == sortField) {
                    col1Value = row1.Columns[col1Index].Value;
                    break;
                }
            }
            for (var col2Index = 0; col2Index < col2Length; col2Index++) {
                if (row2.Columns[col2Index].Name == sortField) {
                    col2Value = row2.Columns[col2Index].Value;
                    break;
                }
            }
            return compareFunction(col1Value, col2Value);
        }
    };
} (jQuery));


// Array Remove - By John Resig (MIT Licensed)
Array.prototype.remove = function (from, to) {
    var rest = this.slice((to || from) + 1 || this.length);
    this.length = from < 0 ? this.length + from : from;
    return this.push.apply(this, rest);
};

function isValid(obj) {
    if (typeof obj != "undefined" && obj != null) {
        return true;
    }
    return false;
}

function clone(obj) {
    var target = {};
    for (var i in obj) {
        if (obj.hasOwnProperty(i)) {
            target[i] = obj[i];
        }
    }
    return target;
}

Array.prototype.clone = function () {
    return this.slice(0);
};