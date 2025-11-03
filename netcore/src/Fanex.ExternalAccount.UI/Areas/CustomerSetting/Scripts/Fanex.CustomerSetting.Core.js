/// <reference path="_references.js" />

if (typeof $ == 'undefined' || typeof jQuery == 'undefined') {
    //alert("Require jQuery.js");
}

if (typeof $.blockUI == 'undefined') {
    //alert("Require jquery.blockUI.js");
}

if (!this.JSON) {
    this.JSON = {};
}

if (!Object.keys) Object.keys = function (c) { var b = [], a; for (a in c) c.hasOwnProperty(a) && b.push(a); return b }; if (!Array.prototype.map) Array.prototype.map = function (c, i) { var h, d, a; if (this == null) throw new TypeError(" this is null or not defined"); var b = Object(this), g = b.length >>> 0; if (typeof c !== "function") throw new TypeError(c + " is not a function"); if (arguments.length > 1) h = i; d = new Array(g); a = 0; while (a < g) { var f, e; if (a in b) { f = b[a]; e = c.call(h, f, a, b); d[a] = e } a++ } return d }; if (!Array.prototype.forEach) Array.prototype.forEach = function (c, f) { var e, a; if (this == null) throw new TypeError(" this is null or not defined"); var b = Object(this), g = b.length >>> 0; if (typeof c !== "function") throw new TypeError(c + " is not a function"); if (arguments.length > 1) e = f; a = 0; while (a < g) { var d; if (a in b) { d = b[a]; c.call(e, d, a, b) } a++ } }; (function () { function f(a) { return a < 10 ? "0" + a : a } if (typeof Date.prototype.toJSON !== "function") { Date.prototype.toJSON = function () { return isFinite(this.valueOf()) ? this.getUTCFullYear() + "-" + f(this.getUTCMonth() + 1) + "-" + f(this.getUTCDate()) + "T" + f(this.getUTCHours()) + ":" + f(this.getUTCMinutes()) + ":" + f(this.getUTCSeconds()) + "Z" : null }; String.prototype.toJSON = Number.prototype.toJSON = Boolean.prototype.toJSON = function () { return this.valueOf() } } var cx = /[\u0000\u00ad\u0600-\u0604\u070f\u17b4\u17b5\u200c-\u200f\u2028-\u202f\u2060-\u206f\ufeff\ufff0-\uffff]/g, escapable = /[\\\"\x00-\x1f\x7f-\x9f\u00ad\u0600-\u0604\u070f\u17b4\u17b5\u200c-\u200f\u2028-\u202f\u2060-\u206f\ufeff\ufff0-\uffff]/g, gap, indent, meta = { "\b": "\\b", "\t": "\\t", "\n": "\\n", "\f": "\\f", "\r": "\\r", '"': '\\"', "\\": "\\\\" }, rep; function quote(a) { escapable.lastIndex = 0; return escapable.test(a) ? '"' + a.replace(escapable, function (a) { var b = meta[a]; return typeof b === "string" ? b : "\\u" + ("0000" + a.charCodeAt(0).toString(16)).slice(-4) }) + '"' : '"' + a + '"' } function str(h, i) { var d, e, c, f, g = gap, b, a = i[h]; if (a && typeof a === "object" && typeof a.toJSON === "function") a = a.toJSON(h); if (typeof rep === "function") a = rep.call(i, h, a); switch (typeof a) { case "string": return quote(a); case "number": return isFinite(a) ? String(a) : "null"; case "boolean": case "null": return String(a); case "object": if (!a) return "null"; gap += indent; b = []; if (Object.prototype.toString.apply(a) === "[object Array]") { f = a.length; for (d = 0; d < f; d += 1) b[d] = str(d, a) || "null"; c = b.length === 0 ? "[]" : gap ? "[\n" + gap + b.join(",\n" + gap) + "\n" + g + "]" : "[" + b.join(",") + "]"; gap = g; return c } if (rep && typeof rep === "object") { f = rep.length; for (d = 0; d < f; d += 1) { e = rep[d]; if (typeof e === "string") { c = str(e, a); c && b.push(quote(e) + (gap ? ": " : ":") + c) } } } else for (e in a) if (Object.hasOwnProperty.call(a, e)) { c = str(e, a); c && b.push(quote(e) + (gap ? ": " : ":") + c) } c = b.length === 0 ? "{}" : gap ? "{\n" + gap + b.join(",\n" + gap) + "\n" + g + "}" : "{" + b.join(",") + "}"; gap = g; return c } } if (typeof JSON.stringify !== "function") JSON.stringify = function (d, a, b) { var c; gap = ""; indent = ""; if (typeof b === "number") for (c = 0; c < b; c += 1) indent += " "; else if (typeof b === "string") indent = b; rep = a; if (a && typeof a !== "function" && (typeof a !== "object" || typeof a.length !== "number")) throw new Error("JSON.stringify"); return str("", { "": d }) }; if (typeof JSON.parse !== "function") JSON.parse = function (text, reviver) { var j; function walk(d, e) { var b, c, a = d[e]; if (a && typeof a === "object") for (b in a) if (Object.hasOwnProperty.call(a, b)) { c = walk(a, b); if (c !== undefined) a[b] = c; else delete a[b] } return reviver.call(d, e, a) } text = String(text); cx.lastIndex = 0; if (cx.test(text)) text = text.replace(cx, function (a) { return "\\u" + ("0000" + a.charCodeAt(0).toString(16)).slice(-4) }); if (/^[\],:{}\s]*$/.test(text.replace(/\\(?:["\\\/bfnrt]|u[0-9a-fA-F]{4})/g, "@").replace(/"[^"\\\n\r]*"|true|false|null|-?\d+(?:\.\d*)?(?:[eE][+\-]?\d+)?/g, "]").replace(/(?:^|:|,)(?:\s*\[)+/g, ""))) { j = eval("(" + text + ")"); return typeof reviver === "function" ? walk({ "": j }, "") : j } throw new SyntaxError("JSON.parse"); } })()

if (typeof Fanex == 'undefined') {
    Fanex = {};
}

if (typeof Fanex.CustomerSetting == 'undefined') {
    Fanex.CustomerSetting = {};
}

Fanex.CustomerSetting.Core = {
    $this: null,
    BlockedUI: false,

    Init: function () {
        $ = jQuery;
        $this = this;
        jQuery.blockUI.defaults.css.zIndex = '9999999';
        jQuery.blockUI.defaults.css.position = 'absolute';
        jQuery.blockUI.defaults.css.border = '0px solid #aaa';
        jQuery.blockUI.defaults.css.backgroundColor = "transparent";
        jQuery.blockUI.defaults.overlayCSS.backgroundColor = "transparent";
    },

    BlockUI: function () {
        if (!$this.BlockedUI) {
            $.blockUI({
                message: '<div class="loading"></div>',
                onBlock: function () {
                    $this.BlockedUI = true;
                },
                onUnblock: function () {
                    $this.BlockedUI = false;
                }
            });
        }
    },

    UnBlockUI: function () {
        jQuery.unblockUI();
    },

    DecodeHtml: function (str) {
        return $("<div/>").html(str).text();
    },

    EncodeHtml: function (str) {
        return $('<div/>').text(str).html();
    },

    FormatNumber: function (number) { // Format a number to 1,234,567.89
        number += '';

        x = number.split('.');
        x1 = x[0];
        x2 = x.length > 1 ? '.' + x[1] : '';
        var rgx = /(\d+)(\d{3})/;
        while (rgx.test(x1)) {
            x1 = x1.replace(rgx, '$1' + ',' + '$2');
        }
        return x1 + x2;
    },

    ReplaceAll: function (src, args, replace) {
        while (src.indexOf(args) != -1) {
            src = src.replace(args, replace);
        }

        return src;
    },

    CheckNumber: function (event, ele, isFloat) {
        var value = ele.value;

        var evt = event ? event : window.event; // Fix for IE7
        if (evt.keyCode === 9) return true; // Ignore TAB key
        var obj = evt.currentTarget ? evt.currentTarget : evt.srcElement; // Fix for IE7
        var value = obj.value;
        value = !$.isNumeric(this.ReplaceAll(value, ',', '')) ? '' : value;
        obj.value = value;

        if (value == '') return true;

        var num;
        var dotPosition = 0;
        var afterDot;
        var caretPosition = getCaret(obj);

        num = value.toString().replace(/\$|\,/g, '');
        dotPosition = num.indexOf(".");

        if ((typeof (isFloat) != 'undefined') && isFloat && dotPosition > 0) {
            afterDot = num.substring(dotPosition + 1, num.length).replace(/[^0-9]/g, '');
            num = num.substring(0, dotPosition);
        }

        num = num.formatFloat();

        if ((typeof (isFloat) != 'undefined') && isFloat && dotPosition > 0) {
            eval(obj).value = num + "." + afterDot;
            num = obj.value;
        } else {
            eval(obj).value = num;
        }
        if (num.length > value.length) {
            caretPosition = caretPosition + 1;
        }

        if (evt.keyCode == 8 && num.substring(caretPosition - 1, caretPosition) == ',') {
            setCaret(obj, caretPosition - 1);
        }
        else {
            setCaret(obj, caretPosition);
        }

        return true;
    },

    ProcessDisplayMultiple: function (length, usernames) {
        $('#arrowUpDown').bind('click', function () {
            Fanex.CustomerSetting.Core.ToggleUsernames(length, usernames);
        });

        $('#names').css('display', 'block');

        if (usernames.length < (length ? length : 98)) {
            $('#names').css('cursor', 'default');
            $('#names').html(usernames.substring(0, usernames.length - 2));
            $('#arrowUpDown').css('display', 'none');
        }
        else {
            $('#names').css('cursor', 'pointer');
            $('#names').bind('click', function () {
                Fanex.CustomerSetting.Core.ToggleUsernames(length, usernames);
            });
            $('#names').html(usernames.substring(0, (length ? length : 98)) + '...');
            $('#names').attr('title', usernames.substring(0, usernames.length - 2));
            $('#arrowUpDown').css('display', 'block');
        }
    },

    ToggleUsernames: function (length, usernames) {
        if ($('#arrowUpDown').hasClass('collapse')) {
            $('#arrowUpDown').attr('class', 'expand');
            $('#names').html(usernames.substring(0, usernames.length - 2));
            $('#names').attr('title', '');
        }
        else {
            $('#arrowUpDown').attr('class', 'collapse');
            $('#names').html(usernames.substring(0, (length ? length : 98)) + '...');
            $('#names').attr('title', usernames.substring(0, usernames.length - 2));
        }
    }
};

String.prototype.replaceAll = function (args, replace) {
    var src = this;

    while (src.indexOf(args) != -1) {
        src = src.replace(args, replace);
    }

    return src;
};

Number.prototype.replaceAll = function (args, replace) {
    return this;
};

function setCaret(oField, iCaretPos) {
    // IE Support
    if (document.selection) {
        oField.focus();
        // Create empty selection range
        var oSel = document.selection.createRange();
        // Move selection start and end to desired position
        oSel.moveStart('character', iCaretPos);
        oSel.moveEnd('character', iCaretPos - oField.value.length);
        oSel.select();
    }
        // Firefox support
    else if (oField.selectionStart || oField.selectionStart == '0') {
        oField.selectionStart = iCaretPos;
        oField.selectionEnd = iCaretPos;
        oField.focus();
    }
};

function getCaret(oField) {
    var iCaretPos = 0;
    // IE Support
    if (document.selection) {
        oField.focus();
        // To get cursor position, get empty selection range
        var oSel = document.selection.createRange();
        // Move selection start to 0 position
        oSel.moveStart('character', -oField.value.length);
        iCaretPos = oSel.text.length;
    }
        // Firefox support
    else if (oField.selectionStart || oField.selectionStart == '0')
        iCaretPos = oField.selectionStart;
    // Return results
    return (iCaretPos);
};

String.prototype.formatNumber = function () {
    var number = this.replaceAll(',', '');
    number += '';

    x = number.split('.');
    x1 = x[0];
    x2 = x.length > 1 ? '.' + x[1] : '';
    var rgx = /(\d+)(\d{3})/;
    while (rgx.test(x1)) {
        x1 = x1.replace(rgx, '$1' + ',' + '$2');
    }
    return x1 + x2;
}

Number.prototype.formatNumber = function () {
    var number = this.replaceAll(',', '');
    number += '';

    x = number.split('.');
    x1 = x[0];
    x2 = x.length > 1 ? '.' + x[1] : '';
    var rgx = /(\d+)(\d{3})/;
    while (rgx.test(x1)) {
        x1 = x1.replace(rgx, '$1' + ',' + '$2');
    }
    return x1 + x2;
}

String.prototype.formatFloat = function () {
    var number = this;

    if (number == '') return 0;
    var sign;
    if (isNaN(number)) {
        number = '';
    }

    sign = (number == (number = Math.abs(number)));
    number = Math.floor(number * 100 + 0.50000000001);
    number = Math.floor(number / 100).toString();

    for (var i = 0; i < Math.floor((number.length - (1 + i)) / 3) ; i++) {
        number = number.substring(0, number.length - (4 * i + 3)) + ',' + number.substring(number.length - (4 * i + 3));
    }

    return (((sign) ? '' : '-') + number);
};

String.prototype.toFixed = function (precision, optionals) {
    var value = parseFloat(this);

    var power = Math.pow(10, precision),
            output;

    // Multiply up by precision, round accurately, then divide and use native toFixed():
    output = (Math.round(value * power) / power).toFixed(precision);

    if (optionals) {
        var optionalsRegExp = new RegExp('0{1,' + optionals + '}$');
        output = output.replace(optionalsRegExp, '');
    }

    return output;
}

String.prototype.formatErrorMessage = function () {
    var msg = this;

    var indexOfStartParenthesis = msg.indexOf('(');
    var indexOfEndParenthesis = msg.indexOf(')');
    var orgMsg = msg.substring(indexOfStartParenthesis, indexOfEndParenthesis + 1);
    var preMsg = msg.substring(0, indexOfStartParenthesis);
    var subMsg = msg.substring(indexOfEndParenthesis + 1, msg.length);

    if (msg.indexOf('GMT') !== -1) {
        preMsg = msg;
        orgMsg = subMsg = '';
    }

    var formatMsg = '<font color="#fe0000">' + preMsg + '' + orgMsg + '</font>' +
    '<font color="#fe0000">' + subMsg + '</font>';

    return formatMsg;
};

String.prototype.formatErrMsg = function () {
    $('#errMsg').addClass('errMsg');
    $('#errMsg').removeClass('successMsg');

    $('#err').addClass('errMsg');
    $('#err').removeClass('successMsg');

    return this;
};

String.prototype.formatSuccessMessage = function () {
    $('#errMsg').removeClass('errMsg');
    $('#errMsg').addClass('successMsg');

    $('#err').removeClass('errMsg');
    $('#err').addClass('successMsg');

    return this;
}

String.prototype.decodeHtml = function () {
    var str = this;
    return $("<div/>").html(str).text();
};

String.prototype.encodeHtml = function () {
    var str = this;
    return $('<div/>').text(str).html();
};

function disablePulldownAndReleaseRefresh() {
    var maybePreventPullToRefresh = false;
    var lastTouchY = 0;
    var touchstartHandler = function (e) {
        if (e.touches.length != 1) return;
        lastTouchY = e.touches[0].clientY;
        // Pull-to-refresh will only trigger if the scroll begins when the
        // document's Y offset is zero.
        maybePreventPullToRefresh =
            window.pageYOffset == 0;
    }

    var touchmoveHandler = function (e) {
        var touchY = e.touches[0].clientY;
        var touchYDelta = touchY - lastTouchY;
        lastTouchY = touchY;

        if (maybePreventPullToRefresh) {
            // To suppress pull-to-refresh it is sufficient to preventDefault the
            // first overscrolling touchmove.
            maybePreventPullToRefresh = false;
            if (touchYDelta > 0) {
                e.stopPropagation();
                return;
            }
        }
    }

    document.addEventListener('touchstart', touchstartHandler, { passive: false });
    document.addEventListener('touchmove', touchmoveHandler, { passive: false });
}

$(document).ready(function () {
    Fanex.CustomerSetting.Core.Init();
    disablePulldownAndReleaseRefresh();

    $.ajaxSetup({
        beforeSend: function (xhr) {
            var antiXSRFToken = document.getElementsByName("__RequestVerificationToken");
            if (antiXSRFToken.length > 0) {
                xhr.setRequestHeader("__RequestVerificationToken", antiXSRFToken[0].value);
            }
        }
    });
});