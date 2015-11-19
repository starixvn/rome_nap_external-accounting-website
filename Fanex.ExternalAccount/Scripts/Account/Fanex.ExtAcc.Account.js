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

    ResetLoginAttempt: function () {
        $.ajax(resolveClientUrl("Account/ResetLoginAttempt"))
        .done(function (data) {
            $('#captcha').hide();
        })
        .fail(function (data) {
            if (typeof console != 'undefined')
                console.log(data);
        })
    },

    HandleAjaxResult: function (element) {
        $(element).bind('ajaxComplete', function (event, request) {
            switch (request.status) {
                case 401:
                case 403:
                case 404:
                case 500:
                    if (!isLocal)
                        location.href = resolveClientUrl('Shared/Error');
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

        var interval = setInterval(function () {
            clearInterval(interval);
            Fanex.ExtAcc.Account.ResetLoginAttempt();
        }, 300000);

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
                /*
                if ($('#Captcha').val().toString().search(/^[<\s\s*>]/) != -1) {
                $('#Captcha').focus();
                Validators.show($('#Captcha').get(0), 'Not allow HTML TAG');
                return false;
                }*/

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
                /*
                if ($('#Captcha').val() == '' || $('#Captcha').val().length < 4) {
                $('#Captcha').focus();
                Validators.show($('#Captcha').get(0), 'This field is required');
                return false;
                }*/
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
                if (typeof console != 'undefined') console.log(e);
            }
        });
    },

    Logout: function () {
        $.ajax(resolveClientUrl("Account/LogOff"))
        .done(function (data) {
            location.href = resolveClientUrl("Account/Login");
        })
        .fail(function (data) {
            if (typeof console != 'undefined') console.log(data);
        })
    }
}

$(document).ready(function () {
    $('.groupChosen').height($(window).height() - 120);
    Fanex.ExtAcc.Account.Init();
});