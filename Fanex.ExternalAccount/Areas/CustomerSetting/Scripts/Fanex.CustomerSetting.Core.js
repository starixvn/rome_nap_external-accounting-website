/// <reference path="_references.js" />

if (typeof $ == 'undefined' || typeof jQuery == 'undefined') {
    alert("Require jQuery.js");
}

if (typeof $.blockUI == 'undefined') {
    alert("Require jquery.blockUI.js");
}

if (typeof JSON == 'undefined') {
    JSON = {};

    (function () {
        'use strict';

        function f(n) {
            return n < 10 ? '0' + n : n;
        }

        if (typeof Date.prototype.toJSON !== 'function') {

            Date.prototype.toJSON = function () {

                return isFinite(this.valueOf())
                ? this.getUTCFullYear() + '-' +
                    f(this.getUTCMonth() + 1) + '-' +
                    f(this.getUTCDate()) + 'T' +
                    f(this.getUTCHours()) + ':' +
                    f(this.getUTCMinutes()) + ':' +
                    f(this.getUTCSeconds()) + 'Z'
                : null;
            };

            String.prototype.toJSON =
            Number.prototype.toJSON =
            Boolean.prototype.toJSON = function () {
                return this.valueOf();
            };
        }

        var cx = /[\u0000\u00ad\u0600-\u0604\u070f\u17b4\u17b5\u200c-\u200f\u2028-\u202f\u2060-\u206f\ufeff\ufff0-\uffff]/g,
        escapable = /[\\\"\x00-\x1f\x7f-\x9f\u00ad\u0600-\u0604\u070f\u17b4\u17b5\u200c-\u200f\u2028-\u202f\u2060-\u206f\ufeff\ufff0-\uffff]/g,
        gap,
        indent,
        meta = {    // table of character substitutions
            '\b': '\\b',
            '\t': '\\t',
            '\n': '\\n',
            '\f': '\\f',
            '\r': '\\r',
            '"': '\\"',
            '\\': '\\\\'
        },
        rep;


        function quote(string) {
            escapable.lastIndex = 0;
            return escapable.test(string) ? '"' + string.replace(escapable, function (a) {
                var c = meta[a];
                return typeof c === 'string'
                ? c
                : '\\u' + ('0000' + a.charCodeAt(0).toString(16)).slice(-4);
            }) + '"' : '"' + string + '"';
        }

        function str(key, holder) {
            var i,          // The loop counter.
            k,          // The member key.
            v,          // The member value.
            length,
            mind = gap,
            partial,
            value = holder[key];

            if (value && typeof value === 'object' &&
                typeof value.toJSON === 'function') {
                value = value.toJSON(key);
            }

            if (typeof rep === 'function') {
                value = rep.call(holder, key, value);
            }

            switch (typeof value) {
                case 'string':
                    return quote(value);
                case 'number':
                    return isFinite(value) ? String(value) : 'null';
                case 'boolean':
                case 'null':
                    return String(value);
                case 'object':
                    if (!value) {
                        return 'null';
                    }

                    gap += indent;
                    partial = [];

                    if (Object.prototype.toString.apply(value) === '[object Array]') {
                        length = value.length;
                        for (i = 0; i < length; i += 1) {
                            partial[i] = str(i, value) || 'null';
                        }
                        v = partial.length === 0
                    ? '[]'
                    : gap
                    ? '[\n' + gap + partial.join(',\n' + gap) + '\n' + mind + ']'
                    : '[' + partial.join(',') + ']';
                        gap = mind;
                        return v;
                    }

                    if (rep && typeof rep === 'object') {
                        length = rep.length;
                        for (i = 0; i < length; i += 1) {
                            if (typeof rep[i] === 'string') {
                                k = rep[i];
                                v = str(k, value);
                                if (v) {
                                    partial.push(quote(k) + (gap ? ': ' : ':') + v);
                                }
                            }
                        }
                    } else {
                        for (k in value) {
                            if (Object.prototype.hasOwnProperty.call(value, k)) {
                                v = str(k, value);
                                if (v) {
                                    partial.push(quote(k) + (gap ? ': ' : ':') + v);
                                }
                            }
                        }
                    }
                    v = partial.length === 0
                ? '{}'
                : gap
                ? '{\n' + gap + partial.join(',\n' + gap) + '\n' + mind + '}'
                : '{' + partial.join(',') + '}';
                    gap = mind;
                    return v;
            }
        }

        if (typeof JSON.stringify !== 'function') {
            JSON.stringify = function (value, replacer, space) {
                var i;
                gap = '';
                indent = '';

                if (typeof space === 'number') {
                    for (i = 0; i < space; i += 1) {
                        indent += ' ';
                    }
                } else if (typeof space === 'string') {
                    indent = space;
                }

                rep = replacer;
                if (replacer && typeof replacer !== 'function' &&
                    (typeof replacer !== 'object' ||
                    typeof replacer.length !== 'number')) {
                    throw new Error('JSON.stringify');
                }

                return str('', { '': value });
            };
        }

        if (typeof JSON.parse !== 'function') {
            JSON.parse = function (text, reviver) {
                var j;

                function walk(holder, key) {
                    var k, v, value = holder[key];
                    if (value && typeof value === 'object') {
                        for (k in value) {
                            if (Object.prototype.hasOwnProperty.call(value, k)) {
                                v = walk(value, k);
                                if (v !== undefined) {
                                    value[k] = v;
                                } else {
                                    delete value[k];
                                }
                            }
                        }
                    }
                    return reviver.call(holder, key, value);
                }

                text = String(text);
                cx.lastIndex = 0;
                if (cx.test(text)) {
                    text = text.replace(cx, function (a) {
                        return '\\u' +
                        ('0000' + a.charCodeAt(0).toString(16)).slice(-4);
                    });
                }

                if (/^[\],:{}\s]*$/
                    .test(text.replace(/\\(?:["\\\/bfnrt]|u[0-9a-fA-F]{4})/g, '@')
                        .replace(/"[^"\\\n\r]*"|true|false|null|-?\d+(?:\.\d*)?(?:[eE][+\-]?\d+)?/g, ']')
                        .replace(/(?:^|:|,)(?:\s*\[)+/g, ''))) {

                    j = eval('(' + text + ')');

                    return typeof reviver === 'function'
                    ? walk({ '': j }, '')
                    : j;
                }

                throw new SyntaxError('JSON.parse');
            };
        }
    } ());
}

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
                message: '<img src="/Areas/CustomerSetting/Images/Loading.gif" />',
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
        jQuery.unblockUI(document);
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

    CheckNumber: function (evt, ele) {
        if (!evt) return false;

        ele.value = Fanex.CustomerSetting.Core.FormatNumber(Fanex.CustomerSetting.Core.ReplaceAll(ele.value, ',', ''));

        var charCode = (evt.which) ? evt.which : evt.keyCode;
        var ret = true;

        if (charCode > 31 && (charCode < 48 || charCode > 57)) {
            ret = false;
        }

        if (ele && ele.value.length > 1 && ele.value.indexOf('0') == 0) {
            while (ele.value.indexOf('0') == 0 && ele.value.length > 1) {
                ele.value = ele.value.substring(1);
                ele.value = Fanex.CustomerSetting.Core.FormatNumber(Fanex.CustomerSetting.Core.ReplaceAll(ele.value, ',', ''));
            }
        }
        else if (ele && ele.value.length == 0) {
            ele.value = '0';
        }

        return ret;
    },

    ProcessDisplayMultiple: function (length, usernames) {
        $('#arrowUpDown').bind('click', function () {
            Fanex.CustomerSetting.Core.ToggleUsernames(length);
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
                Fanex.CustomerSetting.Core.ToggleUsernames(length);
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
}

String.prototype.formatNumber = function () {
    var number = this;
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
    var number = this;
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

    var tmp = msg.substring(indexOfStartParenthesis, indexOfStartParenthesis + 4);

    var formatMsg = '<font color="#ff5411">' + preMsg + '</font>' + orgMsg +
    '<font color="#ff5411">' + subMsg + '</font>';


    return formatMsg;
};

$(document).ready(function () {
    Fanex.CustomerSetting.Core.Init();
});