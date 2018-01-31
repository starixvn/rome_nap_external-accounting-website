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
                    //if (!isLocal)
                    //location.href = resolveClientUrl('Shared/Error');
                    break;
                default:
                    break;
            }
        });
    },

    Init: function () {
        var interval = setInterval(function () {
            clearInterval(interval);
            Fanex.ExtAcc.Account.ResetLoginAttempt();
        }, 300000);

        this.HandleAjaxResult(document);

        if (document.forms.length > 0) {
            $('#UserName').focus();
            document.forms[0].onsubmit = function () {
                var errorMessage = "";
                var errorField;

                if ($('#UserName').val().toString().search(/^[<\s\s*>]/) != -1) {
                    errorField = $('#UserName');
                    errorMessage = 'Username is not allow HTML TAG';
                } else if ($('#Password').val().toString().search(/^[<\s\s*>]/) != -1) {
                    errorField = $('#Password');
                    errorMessage = 'Password is not allow HTML TAG';
                } else if ($('#UserName').val() == '') {
                    errorField = $('#UserName');
                    errorMessage = 'Username is required';
                } else if ($('#Password').val() == '') {
                    errorField = $('#Password');
                    errorMessage = 'Password is required';
                }

                if (errorMessage !== "") {
                    $("#err-message").html(errorMessage).removeClass("hide");
                    errorField.addClass("invalid-field").focus();
                    return false;
                }

                $("#err-message").addClass("hide");
                $(".invalid-field").removeClass("invalid-field");

                $('#Password').val($('#Password').val().ec($('#Code').val()).hc());
            };
        }

        // Collapse/ expand header navigation
        var iconNav = $("#iconNav");
        var iconUserInfo = $("#iconUserInfo");
        var sidebar = $("#sidebar");
        var leftPanel = $("#leftSplitter");
        var $this = this;

        iconNav.on("click", function () {
            if (iconUserInfo.hasClass('active')) {
                leftPanel.removeClass("expand-leftpanel");
                iconUserInfo.removeClass("active");
                sidebar.addClass("expand-sidebar").show("fast");
            }

            if (!iconNav.hasClass('active')) {
                sidebar.addClass("expand-sidebar").show("fast");
                iconNav.addClass("active");

            } else {
                sidebar.show("fast", function () {
                    sidebar.removeClass("expand-sidebar");
                    iconNav.removeClass("active");
                });
            }
        });

        var hasMenu = isMobile.tablet ?
            $(".main-menu .menu .hasChild") :
            $("#sidebar .menu .hasChild");
        hasMenu.on("click", function () {
            var menu = $(this);
            var submenu = $(this).siblings('.submenu');

            if (menu.hasClass('active')) {

                submenu.removeClass('show-submenu').addClass('hide-submenu');
                menu.removeClass("active");

            } else {
                $(".submenu").removeClass('show-submenu').addClass('hide-submenu');
                hasMenu.removeClass("active");

                menu.addClass("active");
                submenu.removeClass('hide-submenu').addClass('show-submenu');
            }
        });

        $(".submenu .link").on("click", function () {
            if (isMobile.tablet) {
                var submenu = $(this).parent().parent();
                submenu.removeClass('show-submenu').addClass('hide-submenu');
                submenu.siblings(".hasChild").removeClass("active");
            }

            $this.collapseSidebar();
        });

        var widthDevice = $(window).width();
        if (isMobile.phone || (widthDevice >= 768 && widthDevice <= 1600)) {
            iconUserInfo.removeClass('active');
        }

    },
    collapseSidebar: function () {
        var iconNav = $("#iconNav");
        var sidebar = $("#sidebar");

        var widthDevice = top.$(top.window).width();
        if (widthDevice <= 768) {
            top.$("#contentSplitter").removeClass("collapse-content").addClass("expand-content");
            top.$("#sidebar").removeClass("expand-sidebar").addClass("collapse-sidebar");
            top.$("#leftSplitter").removeClass("expand-leftpanel").addClass("collapse-leftpanel");
            iconNav.removeClass("active");
            top.$("#iconUserInfo").removeClass("active");
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
    Fanex.ExtAcc.Account.Init();
});