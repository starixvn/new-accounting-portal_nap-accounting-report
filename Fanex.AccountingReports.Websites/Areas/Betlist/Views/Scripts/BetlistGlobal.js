function virtualHRPopup(sporttype, matchID, raceNo) {
    try { parent.BlockUI(); } catch (e) { }
    var URL = "";
    var title = "";
    var heightTmp = 0;
    var widthTmp = 0;
    var params = {
        MatchID: matchID,
        RaceNo: raceNo
    };
    switch (sporttype) {
        case 181:
            URL = "BetList/GetVirtualHRPopup";
            title = "Virtual Horse Racing";
            heightTmp = 165;
            widthTmp = 750;
            break;
        case 186:
            URL = "BetList/GetVirtualTennisPopup";
            title = "Tennis - ";
            heightTmp = 140;
            widthTmp = 680;
            break;
    }

    $.ajax({
        type: "POST",
        url: window.rootUrl + URL,
        data: params,
        success: function (result) {
            $("#Popup").html(result);

            $("#Popup").dialog({
                title: title,
                autoOpen: false,
                resizable: false,
                height: heightTmp,
                width: widthTmp,
                modal: true,
                position: ['middle', 250],
                close: function (event, ui) {
                }
            });

            $('a.ui-dialog-titlebar-close').show();
            if (sporttype == 186) {
                title += _leagueName;
                $("span.ui-dialog-title").text(title);
            }
            $("#Popup").dialog("open");

        },
        complete: function () {
            //                                $("#virtualHRPopup").dialog("close");
            try { parent.jQuery.unblockUI(); } catch (e) { }
        }
    });
}
