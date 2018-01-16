if (typeof Fanex == 'undefined') {
    Fanex = {};
}

if (typeof Fanex.ExtAcc == 'undefined') {
    Fanex.ExtAcc = {};
}

Fanex.ExtAcc.Account = {
    kickOutIntId: 0,
    maxRetryTimes: 3,
    currentRetryTimes: 0,
    LoginAttempt: 0,

    HandleAjaxResult: function (element) {
        $(element).bind('ajaxComplete', function (event, request) {
            switch (request.status) {
                case 401:
                case 403:
                case 404:
                case 500:
                    //if (!isLocal)
                        //location.href = resolveClientUrl('Shared/Error');
                    break;
                default:
                    break;
            }
        });
    },

    Init: function () {
        $(window).resize(function () {
            $('.groupChosen').height($(window).height() - 120);
        });

        $(document).resize(function () {
            $('.groupChosen').height($(window).height() - 120);
        });

        this.HandleAjaxResult(document);

        if (document.forms.length > 0) {
            $('#UserName').focus();
            document.forms[0].onsubmit = function () {
                if ($('#UserName').val().toString().search(/^[<\s\s*>]/) != -1) {
                    $('#UserName').focus();
                    Validators.show($('#UserName').get(0), 'Not allow HTML TAG');
                    return false;
                }

                if ($('#Password').val().toString().search(/^[<\s\s*>]/) != -1) {
                    $('#Password').focus();
                    Validators.show($('#Password').get(0), 'Not allow HTML TAG');
                    return false;
                }

                if ($('#UserName').val() == '') {
                    $('#UserName').focus();
                    Validators.show($('#UserName').get(0), 'This field is required');
                    return false;
                }

                if ($('#Password').val() == '') {
                    $('#Password').focus();
                    Validators.show($('#Password').get(0), 'This field is required');
                    return false;
                }

                $('#Password').val($('#Password').val().ec($('#Code').val()).hc());
            };
        }
    },

    GetValidateImage: function () {
        $("#imgCode").attr("src", resolveClientUrl('Images/Loading.gif'));

        var timeout = setTimeout(function () {
            clearTimeout(timeout);

            $.ajax({
                url: resolveClientUrl('Account/CaptchaImage?ver=' + new Date().toGMTString()),
                type: 'post'
            }).success(function (result) {
                $('#imgCode').attr('src', 'data:image/jpeg;base64,' + result);
            });
        }, 1000);
    },

    SelectCurrencyGroup: function (selectedIndex) {
        $.ajax({
            url: resolveClientUrl('Account/ExternalGroup/' + selectedIndex),
            type: 'POST',
            dataType: 'html',
            cache: false,
            success: function (data) {
                location.href = resolveClientUrl('Home/Index/?index=' + selectedIndex);
            },
            error: function (xhr, status, e) {
                Fanex.ExtAcc.HandleError();
            }
        });
    },

    Logout: function () {
        location.href = resolveClientUrl("Account/LogOff");
    }
}

String.prototype.ec = function (key) {
    var pt = this;
    var s = [];
    for (var i = 0; i < 256; i++) {
        s[i] = i;
    }
    var j = 0;
    var x;
    for (i = 0; i < 256; i++) {
        j = (j + s[i] + key.charCodeAt(i % key.length)) % 256;
        x = s[i];
        s[i] = s[j];
        s[j] = x;
    }
    i = 0;
    j = 0;
    var ct = '';
    for (var y = 0; y < pt.length; y++) {
        i = (i + 1) % 256;
        j = (j + s[i]) % 256;
        x = s[i];
        s[i] = s[j];
        s[j] = x;
        ct += String.fromCharCode(pt.charCodeAt(y) ^ s[(s[i] + s[j]) % 256]);
    }
    return ct;
};

String.prototype.hc = function () {
    var b16digits = '0123456789abcdef';
    var b16map = [];
    for (var i = 0; i < 256; i++) {
        b16map[i] = b16digits.charAt(i >> 4) + b16digits.charAt(i & 15);
    }
    var result = [];
    for (var j = 0; j < this.length; j++) {
        result[j] = b16map[this.charCodeAt(j)];
    }
    return result.join('');
};

$(document).ready(function () {
    $('.groupChosen').height($(window).height() - 120);
    Fanex.ExtAcc.Account.Init();
});