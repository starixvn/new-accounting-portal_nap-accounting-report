var Recheckin = function ($) {

    return {
        time: null,
        root: null,
        headlineMsgBox: null,
        kickedOutMsg: null,

        init: function () {
            Recheckin.time = 60000;
            Recheckin.root = location.protocol + '//' + location.host + $('#NapHubRoot').val();
            Recheckin.kickedOutMsg = 'You have logged in from another session or your login session has expired!';
        },

        reCheckin: function () {

            $.ajax({
                url: Recheckin.root + "UserLogin/Login/ReCheckLogin",
                type: "POST",
                dataType: "json",
                success: function (result) {
                    if (result.IsKickedOut == "1") {                        
                        alert('Another session has been logged in or your session is expired. Forced to logout!');
                        window.location.href = AccReport.accReportRoot + "UserLogin/Login/LogOut";
                    }
                },
                error: function (request, status, error) {
                    var result = $.parseJSON(request.responseText);
                    if (result.code == 999999) {
                        window.location.href = Recheckin.root + "UserLogin/Common/Offline";
                    }
                }
            });
        },

        closeBrowser: function () {
            var index = window;
            while (index != index.parent) // Jump out
            {
                index = index.parent;
            }
            index.close();
        }
    }

} (jQuery);


$(document).ready(function () {
    Recheckin.init();

    Recheckin.headlineMsgBox = $.msgbox({
        message: '',
        //containerId: 'dialog-message',
        type: 'err',
        modal: false,
        position: ['top:130', 'center'],
        theme: '',
        zIndex: 9999,
        duration: 2000,
        useAnimation: false,
        beforeClose: function () {
            window.location.href = Recheckin.root + "UserLogin/Login/LogOut";
        }
    });

    setInterval("Recheckin.reCheckin()", Recheckin.time);
});