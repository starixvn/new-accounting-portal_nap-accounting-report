//"use strict";
/* global GuideManMessage */
/* exported GuideMan */
var GuideMan;
 $(function () {
    GuideMan.initPage();
});

var GuideMan = {
    initPage: function () {
        //Set fix width for filter-bar
        $("#filter-bar").css("width", ($(window).width() - 20) + "px");
        $(window).resize(function () {
            $("#filter-bar").css("width", ($(window).width() - 20) + "px");
        });

        GuideManMessage.initPage();
    }
};
