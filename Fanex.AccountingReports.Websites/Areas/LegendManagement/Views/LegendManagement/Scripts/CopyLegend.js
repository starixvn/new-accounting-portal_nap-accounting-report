//"use strict";
/* jshint eqeqeq:false,
unused:vars */
/* global legendManagement, GuideManMessage */
var copyLegend = {
    _tables: {
        chbIsNote: null,
        btnBack: null,
        btnCopy: null,
        ddlMenuAccounting: null,
        ddlTargetSystem: null,
        ddlTargetMenu: null
    },
    _params: {
        getSystemMenu: {
            system: ''
        },

        saveReportLegendMapping: {
            AccMenuID: null,
            NewMenuID: null,
            NewSystem: '',
            IsNote: false
        }
    },
    _cacheData: {
        isFirstLoad: true
    },
    initPage: function () {
        if (copyLegend._cacheData.isFirstLoad) {
            copyLegend.disableInput();
            copyLegend.cachedDOMElements();
            copyLegend.getSystemList();
            copyLegend.bindingEvent();
            copyLegend._cacheData.isFirstLoad = false;
        }
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
        copyLegend._tables.chbIsNote = $("#chbIsNoteCopyLegend");
        copyLegend._tables.btnBack = $("#btnBack");
        copyLegend._tables.btnCopy = $("#btnCopy");
        copyLegend._tables.ddlMenuAccounting = $("#ddlMenuAccounting");
        copyLegend._tables.ddlTargetSystem = $("#ddlTargetSystem");
        copyLegend._tables.ddlTargetMenu = $("#ddlTargetMenu");
    },
    getAccountingMenuList: function () {
        var menuList = legendManagement._cacheData.menuList;
        var len = menuList.length;
        var options = '';
        if (len > 0) {
            for (var i = 0; i < len; i++) {
                var menu = menuList[i];
                options += '<optgroup label="' + menu.Name + '">';
                var subMenuLen = menu.SubMenu ? menu.SubMenu.length : 0;
                for (var j = 0; j < subMenuLen; j++) {
                    var subMenu = menu.SubMenu[j];
                    options += '<option value="' + subMenu.Id +
                        '" functionName="' + subMenu.FunctionName +
                        '">' + subMenu.Name + (subMenu.IsHide ? " (hide)" : "") +
                        '</option>';
                }

                options += '</optgroup>';
            }
        }
        else {
            options += '<option value="">No report menu found!</option>';
        }

        copyLegend._tables.ddlMenuAccounting.empty().append(options);
    },
    getSystemList: function () {
        var systemList = [{ Id: 'ACC-REPORT', Name: 'Accounting reports' },
                          { Id: 'ACC-NAP', Name: 'New accounting portal'}];
        var len = systemList.length;
        var options = '';
        if (len > 0) {
            for (var i = 0; i < len; i++) {
                var system = systemList[i];
                options += '<option value="' + system.Id + '">' + system.Name + '</option>';
            }
        }
        else {
            options += '<option value="">No system found!</option>';
        }

        copyLegend._tables.ddlTargetSystem.empty().append(options);
        copyLegend.getTargetMenuList();
    },
    getTargetMenuList: function () {
        copyLegend.disableInput();
        copyLegend._params.getSystemMenu.system =
            copyLegend._tables.ddlTargetSystem.val().toString();

        $.ajax({
            type: "POST",
            url: window.rootUrl + "LegendManagement/LegendManagement/GetMenuList",
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            data: JSON.stringify(copyLegend._params.getSystemMenu),
            success: function (rs) {
                copyLegend.renderTargetMenuList(rs);
            },
            error: function (event, jqXHR, ajaxSettings) {
                copyLegend.renderTargetMenuList([]);
            },
            complete: function () {
                copyLegend.enableInput();
            }
        });
    },
    renderTargetMenuList: function (menuList) {
        var len = menuList.length;
        var options = '';
        if (len > 0) {
            for (var i = 0; i < len; i++) {
                var menu = menuList[i];
                options += '<optgroup label="' + menu.Name + '">';
                if (menu.SubMenu) {
                    var subMenuLen = menu.SubMenu.length;
                    for (var j = 0; j < subMenuLen; j++) {
                        var subMenu = menu.SubMenu[j];
                        options += '<option value="' + subMenu.Id + '">' + subMenu.Name + (subMenu.IsHide ? " (hide)" : "") + '</option>';
                    }
                }

                options += '</optgroup>';
            }
        }
        else {
            options += '<option value="">No report menu found!</option>';
        }

        copyLegend._tables.ddlTargetMenu.empty().append(options);
    },
    bindingEvent: function () {
        copyLegend._tables.btnBack.click(function () {
            legendManagement.showEditLegendView();
        });

        copyLegend._tables.ddlTargetSystem.change(function () {
            copyLegend.getTargetMenuList();
        });

        copyLegend._tables.btnCopy.click(function () {
            copyLegend.copyLegend();
        });
    },
    getSaveReportLegendMapping: function () {
        copyLegend._params.saveReportLegendMapping.AccMenuID =
            parseInt(copyLegend._tables.ddlMenuAccounting.val());
        copyLegend._params.saveReportLegendMapping.NewMenuID =
            parseInt(copyLegend._tables.ddlTargetMenu.val());
        copyLegend._params.saveReportLegendMapping.NewSystem =
            copyLegend._params.getSystemMenu.system;
        copyLegend._params.saveReportLegendMapping.IsNote =
            copyLegend._tables.chbIsNote.attr("checked") ? true : false;
    },
    validSaveReportLegendMapping: function () {
        GuideManMessage.removeHighLight();
        return true;
    },
    copyLegend: function () {
        copyLegend.disableInput();
        GuideManMessage.removeHighLight();
        copyLegend.getSaveReportLegendMapping();
        if (copyLegend.validSaveReportLegendMapping()) {
            $.ajax({
                type: "POST",
                url: window.rootUrl + "LegendManagement/LegendManagement/SaveReportLegendMapping",
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                data: JSON.stringify(copyLegend._params.saveReportLegendMapping),
                success: function (rs) {
                    if (rs && rs !== '') {
                        GuideManMessage.show(rs, '');
                    } else {
                        GuideManMessage.show("Saved successfully!", '', GuideManMessage.MessageType.Success);
                    }
                },
                error: function (event, jqXHR, ajaxSettings) {
                    GuideManMessage.show("Error! Please try again!", '');
                },
                complete: function () {
                    copyLegend.enableInput();
                }
            });
        }
    }
};