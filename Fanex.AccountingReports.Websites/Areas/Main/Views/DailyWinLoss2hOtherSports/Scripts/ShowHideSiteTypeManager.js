
Namespace.Register("Fanex.AccountingReports.Main.DailyWinLoss2h");
Fanex.AccountingReports.Main.DailyWinLoss2h.ShowHideSiteTypeManager = function () {
    var self = this;
    self.Enums = {
        ShowHideCss: {
            Collapse: "collapse",
            Expand: "expand"
        },

        RowType: {
            InfoRow: 1,
            PaddingRow: 2
        }
    };

    self.CachedData = {
        showHideSiteTypeIconTitle: viewResources.Collapse + "/" + viewResources.Expand
    };

    self.GetShowHideSiteTypeIcon = function (groupdId) {
        var showHideCssEnum = showHideSiteTypeManager.Enums.ShowHideCss;
        return '<span title="' + showHideSiteTypeManager.CachedData.showHideSiteTypeIconTitle +
        '" class="showHideSiteType showHideIcon ' +
            (reportManager.CachedData.isShowSiteTypeInfo ?
            showHideCssEnum.Expand :
            showHideCssEnum.Collapse) +
        '" data-sport-group="' + groupdId +
        '"></span>';
    };

    self.ShowHideSiteTypeInfo_ByGroup = function (btnIcon,
        reportTable) {
        var showHideCssEnum = showHideSiteTypeManager.Enums.ShowHideCss;

        var isShowSiteTypeInfo = btnIcon.hasClass(showHideCssEnum.Expand);
        var sportGroup = btnIcon.attr("data-sport-group");
        if (isShowSiteTypeInfo) {
            reportTable.find(".siteTypeRow.infoRow." + sportGroup).addClass("display-none");
            reportTable.find(".siteTypeRow.paddingRow." + sportGroup).removeClass("display-none");
            btnIcon.removeClass(showHideCssEnum.Expand).addClass(showHideCssEnum.Collapse);
        } else {
            reportTable.find(".siteTypeRow.paddingRow." + sportGroup).addClass("display-none");
            reportTable.find(".siteTypeRow.infoRow." + sportGroup).removeClass("display-none");
            btnIcon.removeClass(showHideCssEnum.Collapse).addClass(showHideCssEnum.Expand);
        }

        reportTable.removeClass("display-none").fanexGrid("resizeGrid");
    };

    self.ShowHideSiteTypeInfo = function () {
        var showHideCssEnum = showHideSiteTypeManager.Enums.ShowHideCss;
        if (reportManager.CachedData.isShowSiteTypeInfo) {
            // Hide all site type row
            $(".siteTypeRow.infoRow").addClass("display-none");
            $(".siteTypeRow.paddingRow").removeClass("display-none");
            $(".showHideSiteType.showHideIcon").removeClass(showHideCssEnum.Expand)
                                               .addClass(showHideCssEnum.Collapse);
            reportManager.Tables.btnShowHideSiteTypeInfo.val(viewResources.Expand);
        } else {
            $(".siteTypeRow.paddingRow").addClass("display-none");
            $(".siteTypeRow.infoRow").removeClass("display-none");
            $(".showHideSiteType.showHideIcon").removeClass(showHideCssEnum.Collapse)
                                               .addClass(showHideCssEnum.Expand);
            reportManager.Tables.btnShowHideSiteTypeInfo.val(viewResources.Collapse);
        }

        if (showHideReportManager.CacheData.currentReport &&
            typeof (showHideReportManager.CacheData.currentReport.ShowHideSiteTypeInfo) == "function") {
            showHideReportManager.CacheData.currentReport.ShowHideSiteTypeInfo();
        }

        reportManager.CachedData.isShowSiteTypeInfo = !reportManager.CachedData.isShowSiteTypeInfo;
    };

    self.GetSiteTypeRowCss = function (groupdId, rowType) {
        var rowTypeEnum = showHideSiteTypeManager.Enums.RowType;
        var rowCss = "";
        switch (rowType) {
            case rowTypeEnum.InfoRow:
                rowCss = "siteTypeRow infoRow " + groupdId +
                         (reportManager.CachedData.isShowSiteTypeInfo ? "" : " display-none");
                break;
            case rowTypeEnum.PaddingRow:
                rowCss = "siteTypeRow paddingRow " + groupdId +
                         (reportManager.CachedData.isShowSiteTypeInfo ? "display-none " : "");
                break;
        }

        return rowCss;
    };
};

var showHideSiteTypeManager = new Fanex.AccountingReports.Main.DailyWinLoss2h.ShowHideSiteTypeManager();