
(function ($) {
    var formatMethods = {
        removeSpaceIE9: function (html) {
            if ($.browser.msie && parseInt($.browser.version, 10) === 9) {
                var expr = new RegExp('>[ \t\r\n\v\f]*<', 'g');
                html = html.replace(expr, '><');
            }
            return html;
        }
    };

    //// General Function, use this Plugin by below way:
    //// $.formatLib("removeSpaceIE9", "String html");
    //// It will call formatMethods.removeSpaceIE9 Function
    $.customFormat = function (functionName, params) {
        if (formatMethods[functionName]) {
            return formatMethods[functionName].apply(this, Array.prototype.slice.call(arguments, 1));
        }
    };
})(jQuery);
