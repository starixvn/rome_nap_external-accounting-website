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

    Init: function () {
        this.kickOutIntId = setInterval(this.GetValidateImage, 120000);

        if (document.forms.length > 0) {
            $('#UserName').focus();
            document.forms[0].onsubmit = function () {
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
        var objDate = new Date();
        $("#imgCode").attr("src", "CaptchaImage/" + objDate.getMilliseconds());
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
                Fanex.ExtAcc.Account.currentRetryTimes = 0;
                if (data == -1) {
                    try {
                        Fanex.ExtAcc.Form.Alert('Session Expired', 'You have logged in from another session or your login session has expired!', 400, 150, null, function () {
                            location.href = resolveClientUrl('Account/Login');
                        });
                    } catch (e) {
                        location.href = resolveClientUrl('Account/Login');
                    }                    
                }
                else if (data == 2) {
                    location.href = resolveClientUrl('Account/Login');
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