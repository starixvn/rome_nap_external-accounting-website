var ValidateMessage = (function ($) {

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
        },

        validateMessageConstraints: function () {
            if ($("#msg-en").val() == "") {
                $('#msg').html(tooltip.show('msg-en', 'You must enter English message')).show();
                return false;
            }
            if ($("#msg-en").val().indexOf("@@") < 0) {
                $('#msg').html(tooltip.show('msg-en', 'You must include @@ in English message for replacing ticket number')).show();
                return false;
            }
            if ($("#msg-ch").val() == "") {
                $('#msg').html(tooltip.show('msg-ch', 'You must enter Chinese (Mandarin) message')).show();
                return false;
            }
            if ($("#msg-ch").val().indexOf("@@") < 0) {
                $('#msg').html(tooltip.show('msg-ch', 'You must include @@ in Chinese (Mandarin) message for replacing ticket number')).show();
                return false;
            }

            if ($("#msg-cs").val() != '' && $("#msg-cs").val().indexOf("@@") < 0) {
                $('#msg').html(tooltip.show('msg-cs', 'You must include @@ in Chinese (Cantonese) message for replacing ticket number')).show();
                return false;
            }

            if ($("#msg-th").val() != '' && $("#msg-th").val().indexOf("@@") < 0) {
                Message.Helper.showMoreMessage();
                $('#msg').html(tooltip.show('msg-th', 'You must include @@ in Thai message for replacing ticket number')).show();
                return false;
            }

            if ($("#msg-it").val() != '' && $("#msg-it").val().indexOf("@@") < 0) {
                Message.Helper.showMoreMessage();
                $('#msg').html(tooltip.show('msg-it', 'You must include @@ in Italian message for replacing ticket number')).show();
                return false;
            }

            if ($("#msg-jp").val() != '' && $("#msg-jp").val().indexOf("@@") < 0) {
                Message.Helper.showMoreMessage();
                $('#msg').html(tooltip.show('msg-jp', 'You must include @@ in Japanese message for replacing ticket number')).show();
                return false;
            }

            if ($("#msg-es").val() != '' && $("#msg-es").val().indexOf("@@") < 0) {
                Message.Helper.showMoreMessage();
                $('#msg').html(tooltip.show('msg-es', 'You must include @@ in Spanish message for replacing ticket number')).show();
                return false;
            }

            if ($("#msg-ko").val() != '' && $("#msg-ko").val().indexOf("@@") < 0) {
                Message.Helper.showMoreMessage();
                $('#msg').html(tooltip.show('msg-ko', 'You must include @@ in Korean message for replacing ticket number')).show();
                return false;
            }

            if ($('#password').val() == '') {
                $('#msg').html(tooltip.show('password', 'Please input password.')).show();
                return false;
            }

            return true;
        }
    };
} (jQuery));