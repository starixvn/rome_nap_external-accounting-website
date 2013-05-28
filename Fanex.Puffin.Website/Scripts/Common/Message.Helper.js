var Message = (function () {
    return {};
} (jQuery));

Message.Helper = (function ($) {

    return {
        displayMoreMsg: function () {
            var symbol = $('#spMoreMsg').html();

            if (symbol == '[+]') {
                $('.other-languages').show();
            }
            else {
                $('.other-languages').hide();
            }

            symbol = symbol == '[-]' ? '[+]' : '[-]';
            $('#spMoreMsg').html(symbol);
        },

        showMoreMessage: function () {
            var symbol = $('#spMoreMsg').html();

            if (symbol === '[+]') {
                $('.other-languages').show();
                $('#spMoreMsg').html('[-]');
            }
        },

        hideMoreMessage: function () {
            var symbol = $('#spMoreMsg').html();

            if (symbol === '[-]') {
                $('.other-languages').hide();
                $('#spMoreMsg').html('[+]');
            }
        },

        displayFormMsg: function (isShow) {
            if (isShow) {
                $('.form').show();
            }
            else {
                $('.form').hide();
            }
        }
    };
} (jQuery));