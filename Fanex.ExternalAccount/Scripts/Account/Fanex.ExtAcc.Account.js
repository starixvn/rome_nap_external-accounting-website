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
    RecheckinXHR: null,

    HandleAjaxResult: function (element) {
        $(element).bind('ajaxComplete', function (event, request) {
            switch (request.status) {
                case 401:
                case 403:
                case 404:
                case 500:
                    location.href = resolveClientUrl('Shared/Error');
                    break;
                default:
                    break;
            }
        });
    },

    Init: function () {
        this.HandleAjaxResult(document);

        if (this.RecheckinXHR == null && document.URL.toLowerCase().indexOf("account/login") != -1) {
            setInterval(this.ReCheckin, 10000);
        }

        if (this.kickOutIntId == 0 && document.URL.toLowerCase().indexOf("account/login") != -1) {
            var timeout = setTimeout(function () {
                clearTimeout(timeout);

                $.ajax({
                    url: resolveClientUrl('Account/CaptchaImage?ver=' + new Date().toGMTString()),
                    type: 'post'
                }).success(function (result) {
                    $('#imgCode').attr('src', 'data:image/jpeg;base64,' + result);
                });
            }, 1000);

            this.kickOutIntId = setInterval(this.GetValidateImage, 120000);
        }

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

                if ($('#Captcha').val().toString().search(/^[<\s\s*>]/) != -1) {
                    $('#Captcha').focus();
                    Validators.show($('#Captcha').get(0), 'Not allow HTML TAG');
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

                if ($('#Captcha').val() == '' || $('#Captcha').val().length < 4) {
                    $('#Captcha').focus();
                    Validators.show($('#Captcha').get(0), 'This field is required');
                    return false;
                }
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
    },

    ReCheckin: function () {
        if (this.RecheckinXHR != null)
            this.RecheckinXHR.abort();

        this.RecheckinXHR = $.ajax({
            url: resolveClientUrl('Account/ReCheckIn'),
            type: 'GET',
            dataType: 'html',
            cache: false,
            success: function (data) {
                if (data == -1) {
                    try {
                        if (typeof Fanex.ExtAcc.Form != 'undefined') {
                            if (document.getElementById('windowPopup')) {
                                var tag = document.getElementById('windowPopup');

                                if (tag.tagName == "IFRAME") {
                                    Fanex.ExtAcc.Form.Close();
                                } else {
                                    $('#windowPopup').html('You have logged in from another session or your login session has expired!'); // Prevent opening popup if any
                                }
                            }

                            var timeout = setTimeout(function () {
                                clearTimeout(timeout);
                                Fanex.ExtAcc.Form.Alert('Session Expired', 'You have logged in from another session or your login session has expired!', 400, 150, null, function () {
                                    location.href = resolveClientUrl('Account/Login');
                                }, 1000);
                            }, 1000);
                        }
                    } catch (e) {
                        location.href = resolveClientUrl('Account/Login');
                    }
                }
                else if (data == 2) {
                    location.href = resolveClientUrl('Account/Offline');
                }
                else if (data == 3) {
                    location.href = resolveClientUrl('Shared/Error');
                }

                Fanex.ExtAcc.Account.RecheckinXHR = null;
            },
            error: function (xhr, status, e) {
                if (typeof console != 'undefined') console.log(e);

                Fanex.ExtAcc.Account.RecheckinXHR = null;
            }
        });
    }
}

$(document).ready(function () {
    Fanex.ExtAcc.Account.Init();
});