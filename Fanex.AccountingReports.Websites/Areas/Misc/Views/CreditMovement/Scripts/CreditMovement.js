Namespace.Register("Fanex.AccountingReports.Misc.CreditMovement");

$(document).ready(function () {
    var creditMovement = new Fanex.AccountingReports.Misc.CreditMovement();
    creditMovement.initPage();
});

Fanex.AccountingReports.Misc.CreditMovement = function () {
    var self = this;

    var _tables = {
        datepickerplugin: null,
        reportContainer: null,
        reportContent: null,
        btnGenerate: null,
        ddlSite: null,
        numbericTextbox: null,
        firstDatePickerId: null,
        secondDatePickerId: null
    };

    var _params = {
        fromDate: null,
        toDate: null,
        siteList: null,
        amount: null,
        isPagingCallBack: false,
        pageIndex: 0,
        pageSize: 20
    };

    var _gridOption = {
        itemsPerPage: 20,
        currentPage: 0
    };

    var _cachedTotalItems = 0;
    var _cachedFromDate = today;
    var _cachedToDate = today;

    self.initPage = function () {
        cachedDOMElements();
        renderDatePicker(today, today);
        getSites();
        bindingEvent();
    };

    var renderDatePicker = function (fromDate, toDate) {
        $('#datepickerplugin').nexDatePicker({
            nexDatePickerType: "daterange",
            datePickerFormat: {
                changeMonth: true,
                changeYear: true,
                maxDate: today,
                minDate: minDate,
                dateFormat: FormatConstant.M_DD_YY_STRING
            },
            firstDatePicker: {
                Id: "firstDatePickerId",
                Label: labelFrom,
                InitDate: fromDate,
                LabelCss: "lbl-field col_2",
                TextboxCss: "edit-field col_6"
            },
            secondDatePicker: {
                Id: "secondDatePickerId",
                Label: labelTo,
                InitDate: toDate,
                LabelCss: "lbl-field col_1",
                TextboxCss: "edit-field col_6"
            },
            onSelectDatePicker: function () {
                setDateTimeContrainst();
            }
        });

        _tables.datepickerplugin = $("#datepickerplugin");
        _tables.firstDatePickerId = $("#firstDatePickerId");
        _tables.secondDatePickerId = $("#secondDatePickerId");
    };

    var setDateTimeContrainst = function () {
        var dateRange = $("#datepickerplugin").nexDatePicker("getDateRange");
        var fromDate = dateRange.FromDate;
        var toDate = dateRange.ToDate;
        var diffDays = moment(toDate).diff(moment(fromDate), 'days');

        if (parseInt(diffDays) <= 7 && parseInt(diffDays) >= 0) {
            _cachedFromDate = fromDate;
            _cachedToDate = toDate;
            $(".ui_nexdatepicker_lbl_field").nextAll().remove();
            renderDatePicker(fromDate, toDate);
        }
        else {
            $(".ui_nexdatepicker_lbl_field").nextAll().remove();
            if (_cachedFromDate !== fromDate) {
                _cachedFromDate = fromDate;
                var maxDate = moment(fromDate, "MMM DD YYYY").add("days", 7).format("MMM DD YYYY");
                renderDatePicker(fromDate, maxDate);
                toDate = maxDate;
                _cachedToDate = maxDate;
            }

            if (_cachedToDate !== toDate) {
                _cachedToDate = toDate;
                var minDate = moment(toDate, "MMM DD YYYY").subtract("days", 7).format("MMM DD YYYY");
                renderDatePicker(minDate, toDate);
                fromDate = minDate;
                _cachedFromDate = minDate;
            }
        }
    };

    var getSites = function () {
        AccReport.disableInput();
        AccReport.utility.CallAjax(this,
                                    window.rootUrl + "Misc/CreditMovement/GetSites",
                                    null,
                                    renderSites,
                                    renderSites,
                                    AccReport.enableInput);
    };

    var renderSites = function (rs) {
        var isSetDefaultCollapse = true;
        var id = "ddlSite";
        var siteGroups = [];
        var siteGroupLength = rs.length;
        for (var i = 0; i < siteGroupLength; i++) {
            var siteGroup = rs[i];
            var siteLength = siteGroup.Sites.length;
            if (siteGroup.GroupName == "Credit") {
                var siteLabels = [];
                for (var j = 0; j < siteLength; j++) {
                    var site = siteGroup.Sites[j];
                    var item = {
                        id: id + "-" + site.SiteName,
                        value: site.SiteName,
                        text: site.SiteName,
                        checked: true,
                        enable: true,
                        cssClass: "",
                    };
                    var index = siteLabels.length === 0 ? -1 : siteLabels.findIndex(function (item) { return item.text === site.SiteLabel });
                    if (index === -1) {
                        siteLabels.push({
                            id: id + "-siteLabel-" + site.SiteLabel,
                            value: site.SiteLabel,
                            text: site.SiteLabel,
                            checked: true,
                            enable: true,
                            cssClass: "",
                            items: [item]
                        });
                    } else {
                        siteLabels[index].items.push(item);
                    }
                }

                if (siteLabels.length > 0) {
                    siteGroups.push({
                        id: id + "-group" + siteGroup.GroupName,
                        value: siteGroup.GroupName,
                        text: siteGroup.GroupName,
                        checked: true,
                        enable: true,
                        cssClass: "",
                        items: siteLabels
                    });
                }
            } else {
                var sites = [];
                for (var j = 0; j < siteLength; j++) {
                    var site = siteGroup.Sites[j];
                    sites.push({
                        id: id + "-" + site.SiteGroup + "-" + site.SiteName,
                        value: site.SiteName,
                        text: site.SiteName,
                        checked: true,
                        enable: true,
                        cssClass: "",
                    });
                }

                if (sites.length > 0) {
                    siteGroups.push({
                        id: id + "-group" + siteGroup.GroupName,
                        value: siteGroup.GroupName,
                        text: siteGroup.GroupName,
                        checked: true,
                        enable: true,
                        cssClass: "",
                        items: sites
                    });
                }
            }
        }

        _tables.ddlSite.fanexMultiSelect({
            minWidth: 250,
            maxHeight: 450,
            showFilter: true,
            shadow: true,
            showHeader: true,
            isCheckAll: true,
            showFooter: false,
            checkAllText: 'All',
            isUsedCustomCheckAllText: true,
            useCustomCheckAllText: 'All',
            noneSelectedText: 'Select an option',
            noItemText: 'No site found.',
            disabled: false,
            listData: siteGroups,
            onCheck: function () {
            },
            onOpen: function () {
                if (isSetDefaultCollapse) {
                    $('#ddlDivisionCollapseButton').trigger('click')
                    isSetDefaultCollapse = false;
                }
            }
        });

        _tables.ddlSite = $("#ddlSite");
    };

    var cachedDOMElements = function () {
        _tables.reportContainer = $("#reportContainer");
        _tables.reportContent = $("#reportContent");
        _tables.btnGenerate = $("#btnGenerate");
        _tables.ddlSite = $("#ddlSite");
        _tables.numbericTextbox = $(".numberic-textbox");
    };

    var bindingEvent = function () {
        _tables.numbericTextbox.ForceDecimalOnly().ForceCurrencyOnly();
        _tables.btnGenerate.click(function () {
            resetParams();
            resetGridOptions();
            self.getReport();
        });
    };

    var resetGridOptions = function () {
        _gridOption.itemsPerPage = 20;
        _gridOption.currentPage = 0;
    };

    var resetParams = function () {
        _params.fromDate = null;
        _params.toDate = null;
        _params.siteList = null;
        _params.amount = 300000;
        _params.isPagingCallBack = null;
        _params.pageIndex = 0;
        _params.pageSize = 20;
    };

    self.getReport = function () {
        AccReport.disableInput();
        getParams();

        if (validateParams() == true) {
            $.ajax({
                type: "POST",
                url: window.rootUrl + "Misc/CreditMovement/GetReport",
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                data: JSON.stringify(_params),
                beforeSend: function () {
                    AccReport.disableInput();
                },
                success: function (report) {
                    AccReport.popUp.hide();
                    self.renderReport(report);
                },
                error: function (jqXHR, event, ajaxSettings) {
                    if (jqXHR.status &&
                            jqXHR.status == AccReportData.Constants.HtmlErrorCode.SystemError) {
                        var r = jQuery.parseJSON(jqXHR.responseText);
                        AccReport.popUp.show(r.message);
                    }
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
        var dateRange = _tables.datepickerplugin.nexDatePicker("getDateRange");
        _params.fromDate = dateRange.FromDate + " 00:00:00";
        _params.toDate = dateRange.ToDate + " 23:59:59";
        if ($("#ddlSiteHeaderCheckBox").is(':checked')) {
            _params.siteList = "All";
        }
        else {
            _params.siteList = _tables.ddlSite.fanexMultiSelect("getItems", true).toString();
        }
        var amount = $("#txtAmount").val().replace(/\,/g, "");
        _params.amount = parseInt(amount);

        if (_params.pageIndex == null) {
            _params.pageIndex = 0;
        }

        if (_params.pageSize == null) {
            _params.pageSize = 20;
        }
    };

    var validateParams = function () {
        AccReport.popUp.removeHighLight();
        if (_params.siteList === "") {
            AccReport.popUp.show("Please select  at least one site!", "ddlSite");
            return false;
        }

        if (_params.amount < 100000) {
            AccReport.popUp.show("Minimum amount is 100,000");
            return false;
        }

        return true;
    };

    self.renderReport = function (gridOptions) {
        _cachedTotalItems = gridOptions.totalItems;
        _tables.reportContainer.removeClass("display-none");
        var titleInfo = {
            text: gridOptions.TitleInfoText,
            showExcelButton: true,
            exportExcelFunction: function () {
                self.exportReport();
            }
        };
        var reportLength = gridOptions.bodyRows.length;
        var pagerOption = {
            itemsPerPage: _gridOption.itemsPerPage,
            currentPage: _gridOption.currentPage,
            itemsPerPageArray: [20, 40, 60, 100],
            totalItem: _cachedTotalItems,
            pagerSelectCallBack: self.pageSelectCallback
        };

        gridOptions.showTitle = true;
        gridOptions.titleInfo = titleInfo;
        gridOptions.customPager = true,
        gridOptions.showPager = true;
        gridOptions.pagerOption = pagerOption;
        _tables.reportContent.removeClass("display-none").fanexGrid(gridOptions);
    };

    self.pageSelectCallback = function (page_index, itemsPerPage, jq) {
        itemsPerPage = parseInt(itemsPerPage);
        var oldPagingParams = {
            pageIndex: parseInt(_params.pageIndex),
            totalItems: parseInt(_cachedTotalItems),
            pageSize: parseInt(_params.pageSize)
        };

        if (oldPagingParams.pageSize !== itemsPerPage) {
            _gridOption.currentPage = 0;
            _params.pageIndex = 0;
            _params.pageSize = itemsPerPage;
        }
        else if (page_index * itemsPerPage <= oldPagingParams.totalItems) {
            _gridOption.currentPage = page_index;
            _params.pageIndex = page_index;
        }

        _gridOption.itemsPerPage = itemsPerPage;
        _params.pageSize = itemsPerPage;

        if (!(oldPagingParams.totalItems <= oldPagingParams.pageSize &&
           oldPagingParams.pageIndex === 0 && oldPagingParams.pageSize <= itemsPerPage)) {
            _params.isPagingCallBack = true;
            self.getReport();
        }

        return false;
    };

    self.exportReport = function () {
        AccReport.downloadFile(window.rootUrl + "Misc/CreditMovement/ExportReport", _params);
    };
}