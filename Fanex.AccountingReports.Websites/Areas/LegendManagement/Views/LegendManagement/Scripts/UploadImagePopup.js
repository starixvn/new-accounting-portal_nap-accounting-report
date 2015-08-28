//"use strict";
/* jshint camelcase:false,
unused:vars,
eqeqeq:false */
/* global tinyMCE,
fixBugsBrowser */
var uploadImageManager;
$(document).ready(function () {
    fixBugsBrowser.tinyMCE.fixAttr();
    uploadImageManager.initPage();
});

uploadImageManager = {
    _tables: {
        uploadImageTabs: null,
        uploadImageTabsContent: null,
        tabURL: null,
        tabChoose: null,
        currentUploadTab: null,
        btnCancelUpload: null,
        btnUpload: null,
        txtfile: null,
        txt_url: null,
        upload_message: null,
        urlTab: null,
        locationTab: null,
        txtInputfile: null,
        btnInputfile: null,
        saveImagePath: window.rootWebPath + "/Areas/LegendManagement/Views/LegendManagement/UploadedImage/"
    },
    _cacheData: {
        currentUploadTab: null,
        txtfileVal: '',
        txtInputfileVal: ''
    },
    initPage: function () {
        uploadImageManager._tables.uploadImageDialog = $("#uploadImageDialog");
        uploadImageManager._tables.uploadImageTabs = uploadImageManager._tables.uploadImageDialog.find("#uploadbox-tab li a");
        uploadImageManager._tables.uploadImageTabsContent = uploadImageManager._tables.uploadImageDialog.find(".chooseimg");
        uploadImageManager._tables.tabURL = uploadImageManager._tables.uploadImageDialog.find(".tab-url");
        uploadImageManager._tables.tabChoose = uploadImageManager._tables.uploadImageDialog.find(".tab-choose");
        uploadImageManager._tables.btnCancelUpload = uploadImageManager._tables.uploadImageDialog.find("#btnCancelUpload");
        uploadImageManager._tables.btnUpload = uploadImageManager._tables.uploadImageDialog.find("#btnUpload");
        uploadImageManager._tables.txtfile = uploadImageManager._tables.uploadImageDialog.find("#txtfile");
        uploadImageManager._tables.txt_url = uploadImageManager._tables.uploadImageDialog.find("#txt_url");
        uploadImageManager._tables.upload_message = uploadImageManager._tables.uploadImageDialog.find("#upload_message");
        uploadImageManager._tables.urlTab = uploadImageManager._tables.uploadImageDialog.find("#uploadbox-tab #tab-url-a");
        uploadImageManager._tables.locationTab = uploadImageManager._tables.uploadImageDialog.find("#uploadbox-tab #tab-choose-a");
        uploadImageManager._tables.txtInputfile = uploadImageManager._tables.uploadImageDialog.find("#txtInputfile");
        uploadImageManager._tables.btnInputfile = uploadImageManager._tables.uploadImageDialog.find("#btnInputfile");

        uploadImageManager.bindingEvent();
        uploadImageManager.showView();
    },
    showView: function () {
        uploadImageManager._tables.tabURL.removeClass("display-none");
        uploadImageManager._tables.tabChoose.addClass("display-none");
        uploadImageManager._cacheData.currentUploadTab = uploadImageManager._tables.tabURL;
        uploadImageManager.createBrowseButton();
    },
    bindingEvent: function () {
        uploadImageManager._tables.uploadImageTabs.unbind("click");
        uploadImageManager._tables.urlTab.click(function (e) {
            uploadImageManager.changeUploadTab(uploadImageManager._tables.urlTab);
            uploadImageManager._tables.tabURL.removeClass("display-none");
            uploadImageManager._cacheData.currentUploadTab = uploadImageManager._tables.tabURL;
        });

        uploadImageManager._tables.locationTab.click(function (e) {
            uploadImageManager.changeUploadTab(uploadImageManager._tables.locationTab);
            uploadImageManager._tables.tabChoose.removeClass("display-none");
            uploadImageManager._cacheData.currentUploadTab = uploadImageManager._tables.tabChoose;
        });

        uploadImageManager._tables.btnCancelUpload.unbind("click");
        uploadImageManager._tables.btnCancelUpload.click(function (e) {
            uploadImageManager.cancelUploadImage();
        });

        uploadImageManager._tables.uploadImageDialog.ajaxUpload({
            ajaxFile: window.rootUrl + "LegendManagement/LegendManagement/UploadImage",
            file_types: "jpg,jpeg,gif,png,bmp"
        });

        uploadImageManager._tables.btnUpload.unbind("click");
        uploadImageManager._tables.btnUpload.click(function (e) {
            uploadImageManager.doUploadImage();
        });
    },
    createBrowseButton: function () {
        uploadImageManager._tables.btnInputfile.click(function () {
            uploadImageManager._tables.txtfile.click();
        });

        uploadImageManager._tables.txtfile.unbind("change");
        uploadImageManager._tables.txtfile.bind("change", function () {
            uploadImageManager.txtfileVal = uploadImageManager._tables.txtfile.val();
            uploadImageManager._tables.txtInputfile.val(uploadImageManager.txtfileVal);
        }).change();

        uploadImageManager._tables.txtInputfile.unbind("change");
        uploadImageManager._tables.txtInputfile.bind("change", function () {
            uploadImageManager._tables.upload_message.empty().addClass("display-none");
            uploadImageManager.txtInputfileVal = uploadImageManager._tables.txtInputfile.val();
            if (uploadImageManager.txtInputfileVal != uploadImageManager.txtfileVal) {
                uploadImageManager._tables.txtfile.val(uploadImageManager.txtInputfileVal);
            }
        }).change();
    },
    changeUploadTab: function (uploadTab) {
        uploadImageManager._tables.upload_message.html('');
        uploadImageManager._tables.uploadImageTabs.removeClass("current");
        uploadTab.addClass("current");
        uploadImageManager._tables.uploadImageTabsContent.addClass("display-none");
    },
    cancelUploadImage: function (event) {
        uploadImageManager._tables.txtfile.val('');
        uploadImageManager._tables.txt_url.val('');
        parent.popupManager.closePopup();
    },
    doUploadImage: function () {
        var tabId = uploadImageManager._cacheData.currentUploadTab.attr("id");
        var fileName = '';
        if (tabId == 'tab-choose') {
            fileName = uploadImageManager._tables.txtfile.val();
            if (!uploadImageManager.checkFileType(fileName)) {
                uploadImageManager._tables.txtfile.val('');
                var errorMessage = "<div id='dialog-popup' style='left: 524px; display: block;' class='err'>" +
                                        "Sorry, You have chosen an invalid file extension. Please choose an image file.</div>";

                uploadImageManager._tables.upload_message.html(errorMessage);
                uploadImageManager._tables.upload_message.removeClass("display-none");
                uploadImageManager._tables.txtfile.focus();
            } else {
                var oldElement = uploadImageManager._tables.txtfile;
                // var newElement = $(oldElement).clone();
                $(oldElement).attr('id', 'txtfile');
                $(oldElement).attr('name', 'txtfile');
                $(oldElement).attr('class', 'clone');
                $(oldElement).appendTo('#myUploadForm');
                $('#myUploadForm').submit();
                uploadImageManager.cancelUploadImage();
                setTimeout(function () {
                    fileName = fileName.substring(fileName.lastIndexOf("\\") + 1, fileName.length);
                    var html = '<img src="' + (window.rootWebPath + "/Areas/LegendManagement/Views/LegendManagement/UploadedImage/" + fileName) + '"/>';
                    parent.tinyMCE.execCommand('mceInsertContent', false, html);
                }, 500);
            }
        } else if (tabId === 'tab-url') {
            fileName = uploadImageManager._tables.txt_url.val();
            if (fileName === '') {
                uploadImageManager._tables.txt_url.val('');
                uploadImageManager._tables.upload_message.html("<div class='alertboxerr'><div>Please input image url!</div></div>");
                uploadImageManager._tables.upload_message.removeClass("display-none");
                uploadImageManager._tables.txt_url.focus();
            } else {
                fileName = fileName.substring(fileName.lastIndexOf("\\") + 1, fileName.length);
                var html = '<img src="' + fileName + '"/>';
                tinyMCE.execCommand('mceInsertContent', false, html);
                uploadImageManager.cancelUploadImage();
            }
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
        else {
            return true;
        }
    }
};