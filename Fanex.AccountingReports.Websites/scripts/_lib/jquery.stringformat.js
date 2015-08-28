var stringformat = {
    removeSpaceIE9: function (html) {
        if ($.browser.msie && parseInt($.browser.version, 10) === 9) {
            var expr = new RegExp('>[ \t\r\n\v\f]*<', 'g');
            html = html.replace(expr, '><');
        }
        return html;
    },
    get_MMDDYYYY_String: function (date) {
        if (date != null) {
            var month = date.getMonth() + 1;
            if (month < 10) {
                month = "0" + month;
            }

            var day = date.getDate();
            if (day < 10) {
                day = "0" + day;
            }

            var year = date.getFullYear();
            return month + "/" + day + "/" + year;
        }
        else {
            return "01/01/1990";
        }
    }
}