function SubMenuClick(menuUrl) {
    $('#dvcontent').empty().append('<div id="fullsize" class="fullsize"/><iframe frameborder="0" allowtransparency="true" style="width: 100%; height: 0px; margin-top:2px" hspace="0" vspace="0" marginheight="0" marginwidth="0" scrolling="auto" name="centerFr" id="centerFr"></iframe>');
    $('.title').empty().removeClass('title');

    var iframe = document.getElementById("centerFr");
    var d = new Date().getMilliseconds();
    var parameters = menuUrl.indexOf('?') >= 0 ? '&d=' + d : '?d=' + d;
    iframe.src = menuUrl + parameters;
    var fullheight = $(".fullsize").height();
    var contentheight = fullheight - 44;
    $('#centerFr').height(contentheight + "px");
}