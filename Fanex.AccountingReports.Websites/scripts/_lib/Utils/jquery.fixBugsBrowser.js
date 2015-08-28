

var fixBugsBrowser = {
    fixIndexOfBug: function () {
        if (!Array.prototype.indexOf) {
            Array.prototype.indexOf = function (searchElement /*, fromIndex */) {
                //"use strict";
                if (this == null) {
                    throw new TypeError();
                }

                var t = Object(this);
                var len = t.length >>> 0;
                if (len === 0) {
                    return -1;
                }

                var n = 0;
                if (arguments.length > 1) {
                    n = Number(arguments[1]);
                    if (n != n) { // shortcut for verifying if it's NaN
                        n = 0;
                    } else if (n != 0 && n != Infinity && n != -Infinity) {
                        n = (n > 0 || -1) * Math.floor(Math.abs(n));
                    }
                }

                if (n >= len) {
                    return -1;
                }

                var k = n >= 0 ? n : Math.max(len - Math.abs(n), 0);
                for (; k < len; k++) {
                    if (k in t && t[k] === searchElement) {
                        return k;
                    }
                }

                return -1;
            }
        }
    },
    removeSpaceIE9: function (html) {
        if ($.browser.msie && parseInt($.browser.version, 10) === 9 && html != undefined) {
            var expr = new RegExp('>[ \t\r\n\v\f]*<', 'g');
            html = html.replace(expr, '><');
        }

        return html;
    },
    tinyMCE: {
        fixAttr: function () {
            var jQueryFn = $.fn.attr;
            // Makes sure that $('#tinymce_id').attr('value') gets the editors current HTML contents
            $.fn.attr = function (name, value) {
                var self = this;

                if ((!name) || (name !== "value") || (!containsTinyMCE(self)))
                    if (value !== undefined) {
                        return jQueryFn.call(self, name, value);
                    } else {
                        return jQueryFn.call(self, name);
                    }

                if (value !== undefined) {
                    loadOrSave.call(self.filter(":tinymce"), value);
                    jQueryFn.call(self.not(":tinymce"), name, value);
                    return self; // return original set for chaining
                } else {
                    var node = self[0], ed = tinyMCEInstance(node);
                    return ed ? ed.getContent() : jQueryFn.call($(node), name, value);
                }
            };
        }
    }
}