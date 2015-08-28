//"use strict";
/* global body */
var GuideManMessage = {
    MessageType: {
        Success: 1,
        Warning: 2,
        Error: 3
    },

    arrayHighLightControl: [],
    initPage: function () {
        GuideManMessage.popupMessage = $("#dialog-popup");
    },
    show: function (message, targets, messageType) {
        GuideManMessage.popupMessage.removeClass("err")
                                    .removeClass("info")
                                    .empty()
                                    .text(message);

        var bodyWith = $(body).width();
        var popUpWidth = GuideManMessage.popupMessage.width();
        var leftPosition = Math.round((bodyWith - popUpWidth) / 2);
        GuideManMessage.popupMessage.css("left", leftPosition);

        if (typeof (messageType) === 'undefined') {
            GuideManMessage.popupMessage.addClass("err")
                                        .append('<a class="close" title="Close Message" href="javascript:GuideManMessage.removeHighLight();"></a>');

            if (typeof (targets) !== 'undefined') {
                GuideManMessage.highLightControl(targets);
            }
        }
        else {
            switch (messageType) {
                case GuideManMessage.MessageType.Success:
                    GuideManMessage.popupMessage.addClass("info");
                    setTimeout(function () {
                        GuideManMessage.popupMessage.hide();
                    }, 4000);

                    break;
                case GuideManMessage.MessageType.Warning:
                    GuideManMessage.popupMessage.addClass("warning");
                    setTimeout(function () {
                        GuideManMessage.popupMessage.hide();
                    }, 4000);
                    break;
                case GuideManMessage.MessageType.Error:
                    break;
            }
        }

        GuideManMessage.popupMessage.show();
    },
    hide: function () {
        GuideManMessage.popupMessage.hide();
    },
    highLightControl: function (targets) {
        var controlIds = targets.split(',');
        for (var i = 0; i < controlIds.length; i++) {
            var controlId = controlIds[i];
            if (controlId.indexOf("#") < 0) {
                controlId = "#" + controlId;
            }

            GuideManMessage.arrayHighLightControl.push(controlId);
            $(controlId).addClass("input-validation-error");
        }
    },
    removeHighLight: function () {
        var controlIds = GuideManMessage.arrayHighLightControl;
        for (var i = 0; i < controlIds.length; i++) {
            $(controlIds[i]).removeClass("input-validation-error");
        }

        GuideManMessage.arrayHighLightControl = [];
        GuideManMessage.hide();
    }
};