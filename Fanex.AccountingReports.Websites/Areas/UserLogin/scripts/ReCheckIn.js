function ReCheckin() {
    var KickedOut = 3;
    var strOnUserId = $("#hdfOnUserId").val();
    try{
    $.ajax({
        url: location.protocol + '//' + location.host + "/UserLogin/Login/ReCheckLogin",
        type: "POST",       
        dataType: "json",
        success: function (result) {
            console.log(result);
            if (result) {
                window.location.href = location.protocol + '//' + location.host + "/UserLogin/Login/LogOut";
            }
        },
        error: function (request, status, error) {
            console.log(error);
        }
    });
    }
    catch(ex){}
}

function closeBrowser() {
    var index = window;
    while (index != index.parent) // Jump out
    {
        index = index.parent;
    }

    index.close();
}

$(document).ready(function () {
    setInterval("ReCheckin()", 10000);
});