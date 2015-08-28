//"use strict";
/* jshint camelcase:false,
unused:vars,
eqeqeq:false */
/* global inlineImageExpand,
inlineImageCollapse,
tinyMCE,
defaultReportTemplate,
GuideManMessage,
_cacheData,
defaultTabTemplate,
inputedLegendManager,
moment,
copyLegend,
fixBugsBrowser */
var legendManagement;
var tabManager;
$(document).ready(function () {
    fixBugsBrowser.tinyMCE.fixAttr();
    legendManagement.initPage();
});

legendManagement = {
    _tables: {
        ddlMenu: null,
        chbIsNote: null,
        btnLoad: null,
        btnClear: null,
        btnPreview: null,
        btnSave: null,
        btnCopyLegend: null,

        btnInputedLegendReport: null,
        inputedLegengReportView: null,

        btnAddTab: null,
        btnRemoveTab: null,
        btnAddSubTab: null,
        editLegendView: null,
        copyLegendView: null,
        imageLocation: null,
        btnUploadImage: null,
        htmlEditor: null,
        tinymceContent: null,
        previewReportLegendDialog: null,
        uploadImageDialog: null,
        uploadImageTabs: null,
        uploadImageTabsContent: null,
        tabURL: null,
        tabChoose: null,
        uploadImageDialogTemplate: null,
        btnCancelUpload: null,
        upload_message: null,
        saveImagePath: window.rootWebPath + "/Areas/LegendManagement/Views/LegendManagement/UploadedImage/",
        disableModifyMenu: [-1],
        chbIsReviewed: null
    },
    _params: {
        saveReportLegend: {
            MenuID: null,
            Legend: '',
            IsNote: false,
            IsReviewed: false
        },
        getReportLegend: {
            MenuID: null,
            FunctionName: null,
            IsNote: false
        }
    },
    _cacheData: {
        currentUploadTab: null,
        txtfileVal: '',
        txtInputfileVal: '',
        isIE: false,
        isChrome: false,
        tinyMCECaretPosition: 0,
        menuList: [],
        legendContent: '',
        unsavedLegendContent: '',
        fistTabContent: ''
    },
    initPage: function () {
        legendManagement.disableInput();
        legendManagement.cachedDOMElements();
        legendManagement.getMenuList();
        legendManagement.initHTMLEditor();
        copyLegend.initPage();
        inputedLegendManager.initPage();
        legendManagement.bindingEvent();
    },
    cachedDOMElements: function () {
        legendManagement._tables.ddlMenu = $("#ddlMenu");
        legendManagement._tables.chbIsNote = $("#chbIsNote");
        legendManagement._tables.chbIsReviewed = $("#chbIsReviewed");
        legendManagement._tables.btnLoad = $("#btnLoad");
        legendManagement._tables.btnClear = $("#btnClear");
        legendManagement._tables.btnPreview = $("#btnPreview");
        legendManagement._tables.btnSave = $("#btnSave");
        legendManagement._tables.btnCopyLegend = $("#btnCopyLegend");

        legendManagement._tables.btnInputedLegendReport = $("#button-inputedlegend-report");
        legendManagement._tables.inputedLegengReportView = $("#inputedlegend-reportview");

        legendManagement._tables.editLegendView = $("#editLegendView");
        legendManagement._tables.copyLegendView = $("#copyLegendView");
        legendManagement._tables.btnAddTab = $("#btnAddTab");
        legendManagement._tables.btnRemoveTab = $("#btnRemoveTab");
        legendManagement._tables.btnAddSubTab = $("#btnAddSubTab");

        legendManagement._tables.imageLocation = $("#imageLocation");
        legendManagement._tables.btnUploadImage = $("#upload-image-link");
        legendManagement._tables.htmlEditor = $("#htmlEditor");
        legendManagement._tables.previewReportLegendDialog = $("#previewReportLegendDialog");
        legendManagement._tables.uploadImageDialog = $("#uploadImageDialog");
        legendManagement._tables.uploadImageDialogTemplate = $("#uploadImageDialogTemplate");
        legendManagement._cacheData.isIE = $.browser.msie ? true : false;
        legendManagement._cacheData.isChrome = /chrome/.test(navigator.userAgent.toLowerCase()) ? true : false;
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
    getMenuList: function () {
        legendManagement.disableInput();
        $.ajax({
            type: "POST",
            url: window.rootUrl + "LegendManagement/LegendManagement/GetMenuList",
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            data: null,
            success: function (rs) {
                legendManagement.renderMenuList(rs);
            },
            error: function (event, jqXHR, ajaxSettings) {
                legendManagement.renderMenuList([]);
            },
            complete: function () {
                copyLegend.getAccountingMenuList();
            }
        });
    },
    renderMenuList: function (menuList) {
        legendManagement._cacheData.menuList = menuList;
        var len = menuList.length;
        // var prefixId = 'ddlMenu_';
        var options = '';
        if (len > 0) {
            //            options += '<optgroup label="Template">';
            //            options += '<option value="-1" functionName="[TemplateLegend]">Main Template</option>';

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

        legendManagement._tables.ddlMenu.empty().append(options);
    },
    initHTMLEditor: function () {
        try {
            legendManagement._tables.htmlEditor.tinymce({
                valid_children: "+body[style]",
                // Location of TinyMCE script
                script_url: window.rootUrl + 'Scripts/_Lib/TinyMCE/tiny_mce.js',

                // General options
                theme: "advanced",
                plugins: "table,save,emotions,preview,media,paste,spellchecker",

                // Theme options
                theme_advanced_buttons1: "newdocument,|" +
                    ",bold,italic,underline,strikethrough,|" +
                    ",justifyleft,justifycenter,justifyright,justifyfull,formatselect,fontselect,fontsizeselect,|" +
                    ",forecolor,backcolor,|" +
                    ",code",
                theme_advanced_buttons2: "pastetext,pasteword,|" +
                    ",bullist,numlist,|" +
                    ",outdent,indent,blockquote,|" +
                    ",undo,redo,|" +
                    ",link,unlink,anchor,|" +
                    ",upload,emotions,media,|" +
                    ",tablecontrols,pagebreak",
                theme_advanced_buttons3:
                    legendManagement._cacheData.isIE ||
                    legendManagement._cacheData.isChrome ?
                    "" :
                    "spellchecker",
                theme_advanced_toolbar_location: "top",
                theme_advanced_toolbar_align: "left",

                //theme_advanced_statusbar_location: "bottom",
                theme_advanced_resizing: false,

                // Example content CSS
                content_css: window.rootUrl + "/Areas/LegendManagement/Views/LegendManagement/Styles/LegendManagement.less",
                paste_auto_cleanup_on_paste: true,
                paste_remove_spans: true,
                paste_remove_styles: true,
                tab_focus: ':prev,:next',
                entity_encoding: "raw",
                invalid_elements: "script,noscript,link,title,head,body,html,frame,iframe,input,textarea,select",

                remove_linebreaks: false,
                force_p_newlines: false,
                force_br_newlines: true,
                remove_redundant_brs: false,
                convert_newlines_to_brs: true,
                forced_root_block: false,
                valid_elements: "br",
                verify_html: false,

                spellchecker_languages: "+English=en,Swedish=sv",

                setup: function (ed) {
                    // add upload button
                    //                    ed.addButton('upload', {
                    //                        title: 'Upload image',
                    //                        image: window.rootWebPath + "/Areas/LegendManagement/Views/LegendManagement/Styles/Images/image-up1.png",
                    //                        onclick: function () {
                    //                            legendManagement.uploadImage();
                    //                            return;
                    //                        }
                    //                    });

                    //                    ed.onKeyUp.add(function (ed, e) {
                    //                        legendManagement._cacheData.tinyMCECaretPosition = tinyMCE.activeEditor.selection.getBookmark();
                    //                    });

                    //                    ed.onClick.add(function (ed, e) {
                    //                        legendManagement._cacheData.tinyMCECaretPosition = tinyMCE.activeEditor.selection.getBookmark();
                    //                    });
                },

                onchange_callback: function () {
                    GuideManMessage.removeHighLight();
                }
            });

            setTimeout(function () {
                legendManagement.loadReportLegend();
                setTimeout(function () {
                    legendManagement._tables.tinymceContent = ($(".mceContentBody"), $("#htmlEditor_ifr").contents());
                    legendManagement._tables.tinymceContent.focus(function () {
                        tabManager.initPage();
                    });
                }, 2000);
            }, 2000);
        } catch (e) {
        }
    },
    bindingEvent: function () {
        legendManagement._tables.btnLoad.click(function () {
            legendManagement.loadReportLegend();
        });

        legendManagement._tables.btnClear.click(function () {
            legendManagement.clearReportLegend();
        });

        legendManagement._tables.btnPreview.click(function () {
            legendManagement.previewReportLegend();
        });

        legendManagement._tables.btnSave.click(function () {
            legendManagement.saveReportLegend();
        });

        legendManagement._tables.btnCopyLegend.click(function () {
            legendManagement.showCopyLegendView();
        });

        legendManagement._tables.btnInputedLegendReport.unbind("click");
        legendManagement._tables.btnInputedLegendReport.click(function () {
            legendManagement.showInputedLegendReportView();
        });

        legendManagement._tables.btnUploadImage.click(function () {
            legendManagement.uploadImage();
        });

        legendManagement._tables.btnAddTab.click(function () {
            legendManagement.addTab();
        });

        legendManagement._tables.btnAddSubTab.click(function () {
            legendManagement.addSubTab();
        });

        legendManagement._tables.btnRemoveTab.click(function () {
            legendManagement.removeTab();
        });

        legendManagement._tables.ddlMenu.change(function () {
            var menuId = parseInt(legendManagement._tables.ddlMenu.val());
            var menuIndex = $.inArray(menuId, legendManagement._tables.disableModifyMenu);
            if (menuIndex === -1) {
                legendManagement._tables.btnSave.attr("disabled", null);
            }
            else {
                legendManagement._tables.btnSave.attr("disabled", "disabled");
            }
        });

        legendManagement._tables.chbIsNote.change(function () {
            var isNote = legendManagement._tables.chbIsNote.attr("checked") ? true : false;
            if (isNote) {
                legendManagement._cacheData.unsavedLegendContent = $.trim(tinyMCE.activeEditor.getContent({ format: 'raw' }));
                legendManagement._tables.btnPreview.attr("disabled", "disabled");
                legendManagement._tables.btnAddTab.attr("disabled", "disabled");
                legendManagement._tables.btnRemoveTab.attr("disabled", "disabled");
                legendManagement._tables.btnAddSubTab.attr("disabled", "disabled");
            } else {
                legendManagement._tables.btnPreview.attr("disabled", null);
                legendManagement._tables.btnAddTab.attr("disabled", null);
                legendManagement._tables.btnRemoveTab.attr("disabled", null);
                legendManagement._tables.btnAddSubTab.attr("disabled", null);
                tinyMCE.activeEditor.setContent(legendManagement._cacheData.unsavedLegendContent);
            }
        });
    },
    loadReportLegend: function () {
        legendManagement.disableInput();
        legendManagement.getGetLegendParams();
        if (legendManagement.validGetLegendParams()) {
            $.ajax({
                type: "POST",
                url: window.rootUrl + "LegendManagement/LegendManagement/GetReportLegend",
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                data: JSON.stringify(legendManagement._params.getReportLegend),
                success: function (rs) {
                    if (!legendManagement._params.getReportLegend.IsNote) {
                        legendManagement._cacheData.legendContent = rs.Content;
                    }

                    if (tinyMCE.activeEditor) {
                        legendManagement.completedLoadReportLegend(rs);
                    } else {
                        setTimeout(function () {
                            legendManagement.completedLoadReportLegend(rs);
                        }, 1000);
                    }
                },
                error: function (event, jqXHR, ajaxSettings) {
                    if (!legendManagement._params.getReportLegend.IsNote) {
                        legendManagement._cacheData.legendContent = '';
                    }

                    tinyMCE.activeEditor.setContent('');
                },
                complete: function () {
                    legendManagement._tables.htmlEditor.focus();
                    legendManagement.enableInput();
                }
            });
        }
    },
    completedLoadReportLegend: function (rs) {
        tinyMCE.activeEditor.setContent(rs.Content);
        legendManagement._tables.tinymceContent = ($(".mceContentBody"), $("#htmlEditor_ifr").contents());
        tabManager.initPage();
        if (rs.IsDefault) {
            GuideManMessage.show("Is default template!", "", GuideManMessage.MessageType.Warning);
        }
    },
    getGetLegendParams: function () {
        legendManagement._params.getReportLegend.MenuID = parseInt(legendManagement._tables.ddlMenu.val());
        legendManagement._params.getReportLegend.FunctionName = legendManagement._tables.ddlMenu.find("option:selected")
                                                                                              .attr("functionName");
        legendManagement._params.getReportLegend.IsNote = legendManagement._tables.chbIsNote.attr("checked") ? true : false;
    },
    validGetLegendParams: function () {
        GuideManMessage.removeHighLight();
        return true;
    },
    clearReportLegend: function () {
        GuideManMessage.removeHighLight();
        legendManagement._params.getReportLegend.MenuID = -1;
        legendManagement._params.getReportLegend.FunctionName = '[MainTemplate]';
        legendManagement._params.getReportLegend.IsNote = legendManagement._tables.chbIsNote.attr("checked") ? true : false;
        $.ajax({
            type: "POST",
            url: window.rootUrl + "LegendManagement/LegendManagement/GetReportLegend",
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            data: JSON.stringify(legendManagement._params.getReportLegend),
            success: function (rs) {
                if (!legendManagement._params.getReportLegend.IsNote) {
                    legendManagement._cacheData.legendContent = rs.Content;
                }

                tinyMCE.activeEditor.setContent(rs.Content);
            },
            error: function (event, jqXHR, ajaxSettings) {
                if (!legendManagement._params.getReportLegend.IsNote) {
                    legendManagement._cacheData.legendContent = '';
                }

                tinyMCE.activeEditor.setContent('');
            },
            complete: function () {
                legendManagement._tables.htmlEditor.focus();
                legendManagement.enableInput();
            }
        });

    },
    saveReportLegend: function () {
        legendManagement.disableInput();
        legendManagement.getSaveLegendParams();
        if (legendManagement.validSaveLegendParams()) {
            legendManagement.makeInlineImages();
        }
    },
    doSavingReportLegend: function () {
        legendManagement.disableInput();
        $.ajax({
            type: "POST",
            url: window.rootUrl + "LegendManagement/LegendManagement/SaveReportLegend",
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            data: JSON.stringify(legendManagement._params.saveReportLegend),
            success: function (rs) {
                GuideManMessage.removeHighLight();
                if (rs != null && rs !== '') {
                    GuideManMessage.show(rs, "");
                }
                else {
                    GuideManMessage.show("Updated successfully!", "htmlEditor_ifr", GuideManMessage.MessageType.Success);
                }
            },
            error: function (event, jqXHR, ajaxSettings) {
            },
            complete: function () {
                legendManagement.enableInput();
            }
        });
    },
    getSaveLegendParams: function () {
        legendManagement._params.saveReportLegend.MenuID = parseInt(legendManagement._tables.ddlMenu.val());
        legendManagement._params.saveReportLegend.Legend = $.trim(tinyMCE.activeEditor.getContent({ format: 'raw' }));
        legendManagement._params.saveReportLegend.IsNote = legendManagement._params.getReportLegend.IsNote;
        legendManagement._params.saveReportLegend.IsReviewed = legendManagement._tables.chbIsReviewed.attr("checked") ? true : false;
    },
    validSaveLegendParams: function () {
        GuideManMessage.removeHighLight();
        if (legendManagement._params.saveReportLegend.Legend == null ||
            legendManagement._params.saveReportLegend.Legend === '') {
            legendManagement._tables.htmlEditor.focus();
            GuideManMessage.show("Please input menu lengend content!", "htmlEditor_ifr");
            return false;
        }

        return true;
    },
    previewReportLegend: function () {
        var reportLegendContent = $.trim(tinyMCE.activeEditor.getContent({ format: 'raw' }));
        var menuName = legendManagement._tables.ddlMenu.find("option:selected").html();

        legendManagement._tables.previewReportLegendDialog.empty().html(reportLegendContent);
        legendManagement._tables.previewReportLegendDialog.removeClass("display-none");
        legendManagement._tables.previewReportLegendDialog.dialog({
            width: 650,
            height: 650,
            position: "center",
            modal: true,
            title: "Preview Legend For " + menuName,
            resizable: false
        });

        tabManager.initPreviewDialog();
        legendManagement._tables.previewReportLegendDialog.scrollbars();
        legendManagement.updateScrollbar();
        legendManagement._tables.previewReportLegendDialog.find(".scrollcontent").resize(function () {
            legendManagement.updateScrollbar();
        });
    },
    uploadImage: function () {
        legendManagement._tables.uploadImageDialog.empty().html(
            legendManagement._tables.uploadImageDialogTemplate.html());
        legendManagement._tables.uploadImageDialog.removeClass("display-none");
        legendManagement._tables.uploadImageDialog.dialog({
            width: 400,
            height: 300,
            position: "center",
            modal: true,
            title: "Upload Image",
            resizable: false
        });

        legendManagement._tables.uploadImageTabs = legendManagement._tables.uploadImageDialog.find("#uploadbox-tab li a");
        legendManagement._tables.uploadImageTabsContent = legendManagement._tables.uploadImageDialog.find(".chooseimg");
        legendManagement._tables.tabURL = legendManagement._tables.uploadImageDialog.find(".tab-url");
        legendManagement._tables.tabChoose = legendManagement._tables.uploadImageDialog.find(".tab-choose");
        legendManagement._cacheData.currentUploadTab = legendManagement._tables.tabURL;
        legendManagement._tables.tabURL.removeClass("display-none");
        legendManagement._tables.tabChoose.addClass("display-none");
        legendManagement._tables.btnCancelUpload = legendManagement._tables.uploadImageDialog.find("#btnCancelUpload");
        legendManagement._tables.btnUpload = legendManagement._tables.uploadImageDialog.find("#btnUpload");
        legendManagement._tables.txtfile = legendManagement._tables.uploadImageDialog.find("#txtfile");
        legendManagement._tables.txtfileIE = legendManagement._tables.uploadImageDialog.find("#txtfileIE");
        legendManagement._tables.txt_url = legendManagement._tables.uploadImageDialog.find("#txt_url");
        legendManagement._tables.upload_message = legendManagement._tables.uploadImageDialog.find("#upload_message");
        legendManagement._tables.urlTab = legendManagement._tables.uploadImageDialog.find("#uploadbox-tab #tab-url-a");
        legendManagement._tables.locationTab = legendManagement._tables.uploadImageDialog.find("#uploadbox-tab #tab-choose-a");
        legendManagement._tables.txtInputfile = legendManagement._tables.uploadImageDialog.find("#txtInputfile");
        legendManagement._tables.btnInputfile = legendManagement._tables.uploadImageDialog.find("#btnInputfile");

        legendManagement._tables.uploadImageTabs.unbind("click");
        legendManagement._tables.urlTab.click(function (e) {
            legendManagement.changeUploadTab(legendManagement._tables.urlTab);
            legendManagement._tables.tabURL.removeClass("display-none");
            legendManagement._cacheData.currentUploadTab = legendManagement._tables.tabURL;
        });

        legendManagement._tables.locationTab.click(function (e) {
            legendManagement.changeUploadTab(legendManagement._tables.locationTab);
            legendManagement._tables.tabChoose.removeClass("display-none");
            legendManagement._cacheData.currentUploadTab = legendManagement._tables.tabChoose;
        });

        legendManagement._tables.btnCancelUpload.unbind("click");
        legendManagement._tables.btnCancelUpload.click(function (e) {
            legendManagement.cancelUploadImage();
        });

        legendManagement._tables.btnUpload.unbind("click");
        legendManagement._tables.btnUpload.click(function (e) {
            legendManagement.doUploadImage();
        });

        legendManagement._tables.uploadImageDialog.ajaxUpload({
            ajaxFile: window.rootUrl + "LegendManagement/LegendManagement/UploadImage",
            file_types: "jpg,jpeg,gif,png,bmp"
        });

        if (legendManagement._cacheData.isIE) {
            legendManagement._tables.txtInputfile.addClass("display-none");
            legendManagement._tables.btnInputfile.addClass("display-none");
            legendManagement._tables.txtfileIE.removeClass("display-none");
        }
        else {
            legendManagement._tables.txtfileIE.addClass("display-none");
            legendManagement._tables.txtInputfile.removeClass("display-none");
            legendManagement._tables.btnInputfile.removeClass("display-none");
            legendManagement.createBrowseButton();
        }
    },
    createBrowseButton: function () {
        legendManagement._tables.btnInputfile.click(function () {
            //            legendManagement._tables.txtfile.removeClass("display-none");
            legendManagement._tables.txtfile.click();
        });

        legendManagement._tables.txtfile.unbind("change");
        legendManagement._tables.txtfile.bind("change", function () {
            legendManagement.txtfileVal = legendManagement._tables.txtfile.val();
            legendManagement._tables.txtInputfile.val(legendManagement.txtfileVal);
        }).change();

        legendManagement._tables.txtInputfile.unbind("change");
        legendManagement._tables.txtInputfile.bind("change", function () {
            legendManagement._tables.upload_message.empty().addClass("display-none");
            legendManagement.txtInputfileVal = legendManagement._tables.txtInputfile.val();
            if (legendManagement.txtInputfileVal != legendManagement.txtfileVal) {
                legendManagement._tables.txtfile.val(legendManagement.txtInputfileVal);
            }
        }).change();
    },
    changeUploadTab: function (uploadTab) {
        legendManagement._tables.upload_message.html('');
        legendManagement._tables.uploadImageTabs.removeClass("current");
        uploadTab.addClass("current");
        legendManagement._tables.uploadImageTabsContent.addClass("display-none");
    },
    cancelUploadImage: function (event) {
        legendManagement._tables.txtfile.val('');
        legendManagement._tables.txtfileIE.val('');
        legendManagement._tables.txt_url.val('');
        legendManagement._tables.uploadImageDialog.dialog("close");
    },
    doUploadImageFromUrl: function () {
        var fileName = legendManagement._tables.txt_url.val();
        if (fileName === '') {
            legendManagement._tables.txt_url.val('');
            legendManagement._tables.upload_message.html("<div class='alertboxerr'><div>Please input image url!</div></div>");
            legendManagement._tables.upload_message.removeClass("display-none");
            legendManagement._tables.txt_url.focus();
        } else {
            legendManagement._tables.upload_message.addClass("display-none");
            legendManagement._tables.upload_message.html('');
            fileName = fileName.substring(fileName.lastIndexOf("\\") + 1, fileName.length);
            var html = '<img src="' + fileName + '"/>';
            if (legendManagement._tables.tinymceContent.is(":focus")) {
                tinyMCE.execCommand('mceInsertContent', false, html);
            }

            legendManagement.cancelUploadImage();
        }
    },
    doUploadImageFromClient: function () {
        var fileName = legendManagement._tables.txtfile.val();
        if (!legendManagement.checkFileType(fileName)) {
            legendManagement._tables.txtfile.val('');
            var errorMessage = "<div id='dialog-popup' style='left: 524px; display: block;' class='err'>" +
                                        "Sorry, You have chosen an invalid file extension. Please choose an image file.</div>";

            legendManagement._tables.upload_message.html(errorMessage);
            legendManagement._tables.upload_message.removeClass("display-none");
            legendManagement._tables.txtfile.focus();
        } else {
            legendManagement._tables.upload_message.addClass("display-none");
            legendManagement._tables.upload_message.html('');

            var oldElement = legendManagement._tables.txtfile;
            $(oldElement).attr('id', 'txtfile');
            $(oldElement).attr('name', 'txtfile');
            $(oldElement).attr('class', 'clone');
            $(oldElement).appendTo('#myUploadForm');
            fileName = legendManagement.normalizeFileName(fileName);
            legendManagement._tables.uploadImageDialog.find('#myUploadForm #fileName').val(fileName);
            legendManagement._tables.uploadImageDialog.find('#myUploadForm').submit();
            legendManagement.cancelUploadImage();
            setTimeout(function () {
                var html = '<img src="' + (window.rootWebPath + "/Areas/LegendManagement/Views/LegendManagement/UploadedImage/" + fileName) + '"/>';
                tinyMCE.execCommand('mceInsertContent', false, html);
            }, 500);
        }
    },
    doUploadImage: function () {
        var tabId = legendManagement._cacheData.currentUploadTab.attr("id");
        if (tabId == 'tab-choose') {
            if (legendManagement._cacheData.isIE) {
                legendManagement.ajaxFileUpload();
            }
            else {
                //            legendManagement._tables.txtfile.removeClass("display-none");
                legendManagement.doUploadImageFromClient();
            }
        } else if (tabId == 'tab-url') {
            legendManagement.doUploadImageFromUrl();
        }

        return false;
    },
    checkFileType: function (fileName) {
        if (fileName === '') {
            return false;
        }

        var fileExt = fileName.toLowerCase().substr(fileName.toLowerCase().lastIndexOf('.') + 1);
        var imageExt = "jpg,jpeg,gif,png,bmp";
        if (!imageExt.match(fileExt)) {
            return false;
        }

        return true;
    },
    makeInlineImages: function () {
        var html = $("<div></div>").append($(legendManagement._params.saveReportLegend.Legend));
        // var image = html.find("img");
        // var imageLength = image.length;
        var data = [];
        //        for (var i = 0; i < imageLength; i++) {
        //            $(image[i]).attr("_mce_src", null);
        //            data.push(image[i].src);
        //        }

        $.ajax({
            type: "POST",
            url: window.rootUrl + "LegendManagement/LegendManagement/MakeInlineImages",
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            data: JSON.stringify(data),
            success: function (rs) {
                //                if (rs) {
                //                    var imageLength = rs.length;
                //                    for (var j = 0; j < imageLength; j++) {
                //                        $(image[j]).attr("src", "data:image/png;base64," + rs[j]);
                //                    }
                //                }

                setTimeout(function () {
                    var reportTab = html.find(".report-tab");
                    var reportTabLen = reportTab.length;
                    for (var i = 0; i < reportTabLen; i++) {
                        var reportTabContent = html.find("#" + reportTab.eq(i).attr("rel"));
                        var firstSubTabButton = reportTabContent.find(".report-sub-tab:first");
                        var subTabButtonNotFirst = reportTabContent.find(".report-sub-tab:not(:first)");
                        tabManager.hideSubTab(firstSubTabButton, reportTabContent.find("." + firstSubTabButton.attr("rel")), false);
                        var hideContentLen = subTabButtonNotFirst.length;
                        for (var j = 0; j < hideContentLen; j++) {
                            tabManager.hideSubTab(subTabButtonNotFirst.eq(j), html.find("." + subTabButtonNotFirst.eq(j).attr("rel")), true);
                        }
                    }

                    legendManagement._params.saveReportLegend.Legend = html.html();
                    legendManagement.doSavingReportLegend();
                }, 1000);
            },
            error: function (event, jqXHR, ajaxSettings) {
            },
            complete: function () {
            }
        });
    },
    normalizeFileName: function (fileName) {
        fileName = fileName.replace(/[^a-z0-9._]/gi, '');
        var fileExt = fileName.toLowerCase().substr(fileName.toLowerCase().lastIndexOf('.') + 1);
        fileName = fileName.substring(fileName.lastIndexOf("\\") + 1, fileName.length - fileExt.length - 1);
        fileName = fileName + moment(new Date()).format('_h_mm_ss_MM_DD_YYYY') + "." + fileExt;
        return fileName;
    },
    ajaxFileUpload: function () {
        var fileName = legendManagement._tables.txtfileIE.val();
        if (!legendManagement.checkFileType(fileName)) {
            legendManagement._tables.txtfileIE.val('');
            var errorMessage = "<div id='dialog-popup' style='left: 524px; display: block;' class='err'>" +
                                        "Sorry, You have chosen an invalid file extension. Please choose an image file.</div>";

            legendManagement._tables.upload_message.html(errorMessage);
            legendManagement._tables.upload_message.removeClass("display-none");
            legendManagement._tables.txtfileIE.focus();
        } else {
            fileName = legendManagement.normalizeFileName(fileName);
            $.ajaxFileUpload({
                url: window.rootUrl + "LegendManagement/LegendManagement/UploadImage",
                secureuri: false,
                fileElementId: 'txtfileIE',
                fileElement: legendManagement._tables.txtfileIE,
                dataType: 'json',
                data: {
                    uploadFileName: fileName
                },
                beforeSend: function () {
                },
                complete: function () {
                },
                success: function (data, status) {
                    if (data !== null && data !== undefined && typeof (data.error) !== 'undefined' && data.error !== '') {
                        var errorMessage = "<div id='dialog-popup' style='left: 524px; display: block;' class='err'>" + data.error + "</div>";
                        legendManagement._tables.upload_message.html(errorMessage);
                        legendManagement._tables.upload_message.removeClass("display-none");
                    } else {
                        legendManagement._tables.upload_message.addClass("display-none");
                        legendManagement._tables.upload_message.html('');
                        legendManagement.cancelUploadImage();
                        setTimeout(function () {
                            var html = '<img src="' + (window.rootWebPath + "/Areas/LegendManagement/Views/LegendManagement/UploadedImage/" + fileName) + '"/>';
                            tinyMCE.activeEditor.selection.moveToBookmark(legendManagement._cacheData.tinyMCECaretPosition);
                            tinyMCE.execCommand('mceInsertContent', false, html);
                            legendManagement._cacheData.tinyMCECaretPosition = tinyMCE.activeEditor.selection.getBookmark();
                        }, 200);
                    }
                },
                error: function (data, status, e) {
                    var errorMessage = "<div id='dialog-popup' style='left: 524px; display: block;' class='err'>" + e + "</div>";
                    legendManagement._tables.upload_message.html(errorMessage);
                    legendManagement._tables.upload_message.removeClass("display-none");
                }
            }, fileName);
        }

        return false;
    },
    showCopyLegendView: function () {
        legendManagement._tables.editLegendView.addClass("display-none");
        legendManagement._tables.copyLegendView.removeClass("display-none");
    },
    showInputedLegendReportView: function () {
        legendManagement._tables.editLegendView.addClass("display-none");
        legendManagement._tables.inputedLegengReportView.removeClass("display-none");
        inputedLegendManager.getInputedLegendReport();
    },
    showEditLegendView: function () {
        legendManagement._tables.copyLegendView.addClass("display-none");
        legendManagement._tables.inputedLegengReportView.addClass("display-none");
        legendManagement._tables.editLegendView.removeClass("display-none");
    },
    addTab: function () {
        var currentContent = $.trim(tinyMCE.activeEditor.getContent({ format: 'raw' }));
        var html = $("<div></div>").append(currentContent);
        var htmTabContainers = html.find(".report-tab-container");
        if (htmTabContainers.length === 0) {
            var defaultTab = $("<div></div>").append($(defaultTabTemplate));
            defaultTab.find("#tab-1").html(currentContent);
            tinyMCE.activeEditor.setContent(defaultTab.html());
            tabManager.initPage();
        }
        else {
            var firstTab = htmTabContainers.find(".report-tab:first");
            currentContent = legendManagement._tables.tinymceContent.find("#" + firstTab.attr("rel"));
            legendManagement._cacheData.fistTabContent = currentContent.html();

            var lastTab = htmTabContainers.find(".report-tab:last");
            var nextTabId = parseInt(lastTab.attr("rel").substring(("tab-").length)) + 1;
            var newTab = $('<li><a id="tab-header-' + nextTabId +
                '" href="javascript:;" class="report-tab' + (nextTabId == 1 ? ' current' : '') +
                '" rel="tab-' + nextTabId +
                '">&nbsp;Tab ' + nextTabId + '&nbsp;</a></li>');

            newTab.css(legendManagement.getReportTabContainer_ul_li_Css());
            newTab.find(">a").css(legendManagement.getReportTabContainer_ul_li_a_Css());
            var newContent = $('<div id="tab-' + nextTabId + '" class="content-tab" style="display: none;"></div>');
            newContent.html(legendManagement._cacheData.fistTabContent);
            var htmlTabContentContainers = html.find(".content-tab-container");

            htmTabContainers.append(newTab);
            htmlTabContentContainers.append(newContent);
            tinyMCE.activeEditor.setContent(html.html());
            tabManager.initPage();
            tabManager.changeTab(legendManagement._tables.tinymceContent.find("#" + newTab.find(".report-tab").attr("id")));
        }

        legendManagement._tables.btnAddSubTab.attr("disabled", null);
    },
    removeTab: function () {
        var currentContent = $.trim(tinyMCE.activeEditor.getContent({ format: 'raw' }));
        var html = $("<div></div>").append(currentContent);
        var htmTabContainers = html.find(".report-tab-container");
        var reportTab = htmTabContainers.find(".report-tab");
        var currentButton;
        if (reportTab.length > 1) {
            currentButton = tabManager._cacheData.currentTab;
            var tabParent = currentButton.parent();
            var updatedButton = tabParent.next();
            if (updatedButton.length === 0) {
                updatedButton = tabParent.prev();
            }

            updatedButton = updatedButton.find(".report-tab");
            tabManager.changeTab(updatedButton);
            legendManagement._tables.tinymceContent.find("#" + currentButton.attr("rel")).remove();
            tabParent.remove();
            tabManager.initPage();
        } else if (reportTab.length == 1) {
            currentButton = tabManager._cacheData.currentTab;
            currentContent = legendManagement._tables.tinymceContent.find("#" + currentButton.attr("rel"));

            _cacheData.fistTabContent = '';
            tinyMCE.activeEditor.setContent(currentContent.html());
            legendManagement._tables.btnAddSubTab.attr("disabled", "disabled");
            tabManager.initPage();
        }
    },
    getReportTabContainer_ul_Css: function () {
        return 'display: inline-block; list-style: none outside none; margin: 0 5px 0 0';
    },
    getReportTabContainer_ul_li_Css: function () {
        return {
            'display': 'inline-block',
            'list-style': 'none outside none',
            'margin': '0 5px 0 0'
        };
    },
    getReportTabContainer_ul_li_a_Css: function () {
        return {
            'font-size': '12px',
            'font-weight': 'bold',
            'line-height': '28px',
            'padding': '6px 0.7em',
            'text-decoration': 'none',
            'background': 'none repeat scroll 0 0 #D4D4D4',
            'border-color': '#B1B1B1 #B1B1B1 #B1B1B1',
            'border-style': 'solid',
            'border-width': '1px',
            'cursor': 'pointer',
            'color': '#464646',
            'border-radius': '2px 2px 0 0'
        };
    },
    getReportTabContainer_ul_li_a_current_Css: function () {
        return {
            'background': 'none repeat scroll 0 0 #EBEBEB',
            'border-color': '#B1B1B1 #B1B1B1 #EBEBEB',
            'font-weight': 'bold'
        };
    },
    addSubTab: function () {
        var currentContent = $.trim(tinyMCE.activeEditor.getContent({ format: 'raw' }));
        var html = $("<div></div>").append(currentContent);
        var htmTabContainers = html.find(".report-tab-container");

        var oldHtmlContent = '';
        var newSubTab = '';
        var subTabContainers;
        if (htmTabContainers.length === 0) {
            subTabContainers = html.find(".subTabContainer");
            if (subTabContainers.length === 0) {
                newSubTab = legendManagement.getNewSubTab(html, currentContent);
                html.html('');
                html.append(newSubTab);
            }
            else {
                newSubTab = legendManagement.getNewSubTab(html, defaultReportTemplate);
                subTabContainers.parent().append(newSubTab);
            }

            tinyMCE.activeEditor.setContent(html.html());
        }
        else {
            var currentTabId = parseInt(htmTabContainers.find(".report-tab.current").attr("rel").substring(("tab-").length));
            var currentTabContent = html.find("#tab-" + currentTabId);
            subTabContainers = currentTabContent.find(".subTabContainer");
            if (subTabContainers.length === 0) {
                oldHtmlContent = currentTabContent.html();
                newSubTab = legendManagement.getNewSubTab(currentTabContent, currentTabContent.html());
                currentTabContent.html(newSubTab);
            }
            else {
                newSubTab = legendManagement.getNewSubTab(currentTabContent, defaultReportTemplate);
                currentTabContent.append(newSubTab);
            }

            tinyMCE.activeEditor.setContent(html.html());
        }

        tabManager.initPage();
    },
    getNewSubTab: function (contentParent, content) {
        var lastSubTab = contentParent.find(".subTabContainer:last");
        var nextSubTabId = 1;
        if (lastSubTab.length > 0) {
            nextSubTabId = parseInt(lastSubTab.attr("id").substring(("subTabContainer-").length)) + 1;
        }

        var subTabHeader = $('<ul class="report-sub-tab-container" style="' + legendManagement.getReportSubTabContainer_ul_Css() +
                '"><li><a id="sub-tab-header-' + nextSubTabId +
                '" class="report-sub-tab" isShow="true" rel="sub-tab-' +
                    nextSubTabId + '" href="javascript:;">&nbsp;Section ' +
                    nextSubTabId + '&nbsp;</a></div></li></ul>');
        var subTabContent = $('<div class="subTabContentContainer  sub-tab-' + nextSubTabId +
            '" style="' + legendManagement.getReportSubTabContainer_subTabContentContainer_Css() +
            '">' + content + '</div>');
        var newSubTab = $('<div id="subTabContainer-' + nextSubTabId + '" class="subTabContainer" style="padding-bottom: 10px;"></div>');
        subTabHeader.find(">li").css(legendManagement.getReportSubTabContainer_ul_li_Css());
        subTabHeader.find(">li>a").css(legendManagement.getReportSubTabContainer_ul_li_a_Css());
        newSubTab.append(subTabHeader);
        newSubTab.append(subTabContent);
        return newSubTab;
    },
    getReportSubTabContainer_ul_Css: function () {
        return 'font-family: tahoma,Arial,Helvetica,sans-serif; font-size: 96%; margin: 0;padding: 0;';
    },
    getReportSubTabContainer_ul_li_Css: function () {
        return {
            'display': 'inline-block',
            'list-style': 'none outside none',
            'margin': '0 5px 0 0'
        };
    },
    getReportSubTabContainer_ul_li_a_Css: function () {
        return {
            'background': 'url(' + inlineImageExpand + ') no-repeat scroll 8px 8px #46A3CA',
            'border-color': 'transparent transparent',
            'border-style': 'solid solid none',
            'border-width': '1px 1px medium',
            'color': '#FFFFFF',
            'font-size': '12px',
            'font-weight': 'bold',
            'line-height': '22px',
            'padding': '4px 0.7em 4px 20px',
            'text-decoration': 'none',
            'cursor': 'pointer',
            'border-radius': '2px 2px 0 0'
        };
    },
    getReportSubTabContainer_subTabContentContainer_Css: function () {
        return 'border-color: #46A3CA;border-style: solid;border-width: 1px;border-radius: 0px 4px 4px 4px;padding-right: 10px;word-wrap: break-word;';
    },
    switchFromHTMLEditor: function () {
        tabManager.initPage();
    },
    updateScrollbar: function () {
        if (legendManagement._tables.previewReportLegendDialog.hasClass("no_scroll_h")) {
            legendManagement._tables.previewReportLegendDialog.find(".vscroller .scrollarea").css("bottom", 0);
        } else {
            legendManagement._tables.previewReportLegendDialog.find(".vscroller .scrollarea").css("bottom", 20);
        }

        if (legendManagement._tables.previewReportLegendDialog.hasClass("no_scroll_v")) {
            legendManagement._tables.previewReportLegendDialog.find(".hscroller .scrollarea").css("right", 0);
        } else {
            legendManagement._tables.previewReportLegendDialog.find(".hscroller .scrollarea").css("right", 15);
        }
    }
};

tabManager = {
    _tables: {
        tabs: null,
        tabContents: null,
        subTabs: null,
        subTabContents: null,

        tabsPreviewDialog: null,
        tabContentsPreviewDialog: null,
        subTabsPreviewDialog: null,
        subTabContentsPreviewDialog: null
    },
    _cacheData: {
        currentTab: null,
        currentTabPreviewDialog: null
    },
    initPage: function () {
        tabManager.cacheDOMElement();
        tabManager.bindingEvent();
    },
    cacheDOMElement: function () {
        tabManager._tables.tabs = legendManagement._tables.tinymceContent.find(".report-tab");
        tabManager._tables.tabContents = legendManagement._tables.tinymceContent.find(".content-tab");
        tabManager._cacheData.currentTab = legendManagement._tables.tinymceContent.find(".report-tab.current");

        tabManager._tables.subTabs = legendManagement._tables.tinymceContent.find(".report-sub-tab");
        tabManager._tables.subTabContents = legendManagement._tables.tinymceContent.find(".subTabContentContainer");
    },
    bindingEvent: function () {
        tabManager._tables.tabs.unbind("click");
        tabManager._tables.tabs.click(function (e) {
            var tabButton = $(this);
            tabManager.changeTab(tabButton);
        });

        tabManager._tables.subTabs.unbind("click");
        tabManager._tables.subTabs.click(function (e) {
            var tabButton = $(this);
            tabManager.changeSubTab(tabButton);
        });
    },
    changeTab: function (tabButton) {
        tabManager._cacheData.currentTab.removeClass("current");
        tabManager._cacheData.currentTab.css(legendManagement.getReportTabContainer_ul_li_a_Css());
        tabManager._tables.tabContents.hide();
        tabButton.css(legendManagement.getReportTabContainer_ul_li_a_current_Css());
        tabButton.addClass("current");
        var tabContent = legendManagement._tables.tinymceContent.find("#" + tabButton.attr("rel"));
        tabContent.show();

        tabManager._cacheData.currentTab.attr("data-mce-style", tabManager._cacheData.currentTab.attr("style"));
        tabButton.attr("data-mce-style", tabButton.attr("style"));
        tabContent.attr("data-mce-style", tabContent.attr("style"));
        tabManager._cacheData.currentTab = tabButton;
    },
    changeSubTab: function (tabButton) {
        var reportTab = tabButton.parent().parent().parent();
        tabManager.hideSubTab(tabButton, reportTab.find("." + tabButton.attr("rel")),
                              tabButton.attr("isShow") == "true");
    },
    initPreviewDialog: function () {
        tabManager.cacheDOMElementPreviewDialog();
        tabManager.bindingEventPreviewDialog();
    },
    cacheDOMElementPreviewDialog: function () {
        tabManager._tables.tabsPreviewDialog = legendManagement._tables.previewReportLegendDialog.find(".report-tab");
        tabManager._tables.tabContentsPreviewDialog = legendManagement._tables.previewReportLegendDialog.find(".content-tab");
        tabManager._cacheData.currentTabPreviewDialog = legendManagement._tables.previewReportLegendDialog.find(".report-tab.current");

        tabManager._tables.subTabsPreviewDialog = legendManagement._tables.previewReportLegendDialog.find(".report-sub-tab");
        tabManager._tables.subTabContentsPreviewDialog = legendManagement._tables.previewReportLegendDialog.find(".subTabContentContainer");
    },
    bindingEventPreviewDialog: function () {
        tabManager._tables.tabsPreviewDialog.unbind("click");
        tabManager._tables.tabsPreviewDialog.click(function (e) {
            var tabButton = $(this);
            tabManager.changeTabPreviewDialog(tabButton);
        });

        tabManager._tables.subTabsPreviewDialog.unbind("click");
        tabManager._tables.subTabsPreviewDialog.click(function (e) {
            var tabButton = $(this);
            tabManager.changeSubTabPreviewDialog(tabButton);
        });
    },
    changeTabPreviewDialog: function (tabButton) {
        tabManager._cacheData.currentTabPreviewDialog.removeClass("current");
        tabManager._cacheData.currentTabPreviewDialog.css(legendManagement.getReportTabContainer_ul_li_a_Css());
        tabManager._tables.tabContentsPreviewDialog.hide();
        tabButton.css(legendManagement.getReportTabContainer_ul_li_a_current_Css());
        tabButton.addClass("current");
        var tabContent = legendManagement._tables.previewReportLegendDialog.find("#" + tabButton.attr("rel"));
        tabContent.show();
        tabManager._cacheData.currentTabPreviewDialog = tabButton;
    },
    changeSubTabPreviewDialog: function (tabButton) {
        var reportTab = tabButton.parent().parent().parent();
        tabManager.hideSubTab(tabButton, reportTab.find("." + tabButton.attr("rel")),
                              tabButton.attr("isShow") == "true");
        //        legendManagement._tables.previewReportLegendDialog.scrollbars();
    },
    hideSubTab: function (tabButton, subTabContent, isHide) {
        if (isHide) {
            tabButton.css("background", 'url(' + inlineImageCollapse + ') no-repeat scroll 8px 8px #46A3CA');
            subTabContent.hide();
            tabButton.attr("isShow", false);
        }
        else {
            tabButton.css("background", 'url(' + inlineImageExpand + ') no-repeat scroll 8px 8px #46A3CA');
            subTabContent.show();
            tabButton.attr("isShow", true);
        }

        subTabContent.attr("data-mce-style", subTabContent.attr("style"));
        tabButton.attr("data-mce-style", tabButton.attr("style"));
    }
};

