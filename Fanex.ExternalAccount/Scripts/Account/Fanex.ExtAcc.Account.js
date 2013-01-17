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
        $.ajax({
            url: resolveClientUrl('Account/ReCheckIn'),
            type: 'GET',
            dataType: 'html',
            cache: false,
            success: function (data) {
                this.currentRetryTimes = 0;
                if (data == -1) {
                    navigateToUrl('Account/Login');
                    alert('You have logged in from another session or your login session has expired!');
                }
                else if (data == 2) {
                    navigateToUrl('Account/Login');
                }
            },
            error: function (xhr, status, e) {
                if (typeof console != 'undefined') console.log(e);
                this.currentRetryTimes++;
                if (this.currentRetryTimes >= this.maxRetryTimes) {
                    clearInterval(this.kickOutIntId);
                    window.location.replace(resolveClientUrl('Account/Login'));
                }
            }
        });
    }
}

$(document).ready(function () {
    Fanex.ExtAcc.Account.Init();
});