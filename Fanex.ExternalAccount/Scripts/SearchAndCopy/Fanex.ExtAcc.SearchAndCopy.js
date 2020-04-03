/// <reference path="../_references.js" />
if (typeof Fanex == "undefined" || typeof Fanex.ExtAcc == "undefined") {
    alert("Fanex.ExtAcc.js module is required!")
}

(function ($) {
    var $step1Panel,
        $step2Panel,
        $step1Tab,
        $step2Tab,
        $btnPrevious,
        $btnNext,
        $btnSubmit,
        $username,
        $usernameMessage,
        $message,
        $txtUsername,
        $txtPassword,
        $txtFirstName,
        $txtLastName,
        $txtPhone,
        $txtMobilePhone,
        $txtFax,
        $txtMaxCredit,
        passwordTemplate,
        step2TabIsMouseOver = false;

    function Init() {
        $.ajaxSetup({
            beforeSend: function (xhr) {
                var antiXSRFToken = document.getElementsByName("__RequestVerificationToken");
                if (antiXSRFToken.length > 0) {
                    xhr.setRequestHeader("__RequestVerificationToken", antiXSRFToken[0].value);
                }
            }
        });

        $step1Panel = $('#step1Panel');
        $step2Panel = $('#step2Panel');
        $step1Tab = $('#step1');
        $step2Tab = $('#step2');
        $btnPrevious = $('#btnPrevious');
        $btnNext = $('#btnNext');
        $btnSubmit = $('#btnSubmit');
        $username = $('#username');
        $usernameMessage = $('#usernameMessage');
        $message = $('#msg_content');
        $txtUsername = $('#txtUsername');
        $txtPassword = $('#txtPassword');
        $txtFirstName = $('#txtFirstName');
        $txtLastName = $('#txtLastName');
        $txtPhone = $('#txtPhone');
        $txtMobilePhone = $('#txtMobilePhone');
        $txtFax = $('#txtFax');
        $txtMaxCredit = $('#txtMaxCredit');
        passwordTemplate = $('#passwordTemplate').html();
        $message.width($step1Panel.width());

        $step2Panel.hide();
        $btnPrevious.hide();
        $btnSubmit.hide();
        HideUsernameMessage();
        HideMessage();
        $username.focus(UsernameFocus);
        $username.blur(UsernameBlur);
        $step1Tab.click(function () {
            if ($step1Tab.hasClass('step1-active')) {
                return;
            }

            GotoStep1();
        });
        $step2Tab.mouseover(function () {
            step2TabIsMouseOver = true;
        });
        $step2Tab.mouseout(function () {
            step2TabIsMouseOver = false;
        });
        $step2Tab.click(function () {
            if ($step1Tab.hasClass('step2-active')) {
                return;
            }

            GotoStep2();
        });
        $btnPrevious.click(GotoStep1);
        $btnNext.click(GotoStep2);
        $btnSubmit.click(SubmitClick);
    }

    function SubmitClick() {
        HideMessage();
        HideUsernameMessage();
        $step2Panel.find('input').removeClass('invalid');
        var username = $.trim($txtUsername.val());
        if (!username) {
            ShowErrorMessage('The username is required !');
            $txtUsername.addClass('invalid');
            return;
        }

        var password = $.trim($txtPassword.val());
        if (!password) {
            ShowErrorMessage('The password is required !');
            $txtPassword.addClass('invalid');
            return;
        }

        if (!Fanex.ExtAcc.Customer.validatePwdAgent(password)) {
            ShowErrorMessage(passwordTemplate);
            $txtPassword.addClass('invalid');
            return;
        }

        var maxCredit = parseFloat(Fanex.ExtAcc.ReplaceAll($txtMaxCredit.val(), ',', ''));
        if (isNaN(maxCredit)) {
            ShowErrorMessage('The max credit is required !');
            $txtMaxCredit.addClass('invalid');
            return;
        }

        var requestParams = {
            username: username,
            password: password,
            firstName: $txtFirstName.val(),
            lastName: $txtLastName.val(),
            phone: $txtPhone.val(),
            mobilePhone: $txtMobilePhone.val(),
            fax: $txtFax.val(),
            maxCredit: maxCredit
        };
        console.log(requestParams);
    }

    function GotoStep1() {
        HideMessage();
        $message.width($step1Panel.width());
        $step1Tab.removeClass('step1-inactive');
        $step1Tab.addClass('step1-active');
        $step2Tab.removeClass('step2-active');
        $step2Tab.addClass('step2-inactive');
        $step1Panel.show();
        $step2Panel.hide();
        $btnPrevious.hide();
        $btnNext.show();
        $btnSubmit.hide();
    }

    function GotoStep2() {
        HideMessage();
        $message.width($step2Panel.width());
        SearchCustomer(function () {
            $step1Tab.removeClass('step1-active');
            $step1Tab.addClass('step1-inactive');
            $step2Tab.removeClass('step2-inactive');
            $step2Tab.addClass('step2-active');
            $step1Panel.hide();
            $step2Panel.show();
            $step2Panel.find('input').removeClass('invalid').val('');
            $btnPrevious.show();
            $btnNext.hide();
            $btnSubmit.show();
        });
    }

    function HideUsernameMessage() {
        $usernameMessage.hide();
        $usernameMessage.removeClass();
    }

    function UsernameFocus() {
        $username.removeClass('invalid');
        HideUsernameMessage();
        HideMessage();
    }

    function UsernameBlur() {
        if (step2TabIsMouseOver) {
            return;
        }

        SearchCustomer();
    }

    function SearchCustomer(callback) {
        $username.removeClass('invalid');
        var username = $.trim($username.val());
        if (!username) {
            if (callback) {
                $username.addClass('invalid');
                $usernameMessage.show();
                $usernameMessage.addClass('errorWarning');
                ShowErrorMessage('Please Enter Username');
            }

            return;
        }

        Fanex.ExtAcc.BlockUI();
        var searchParams = {
            username: username
        };
        $.post(resolveClientUrl('SearchAndCopy/SearchCustomer'), searchParams, function (result) {
            $usernameMessage.show();
            if (!result || !result.CustId) {
                $username.addClass('invalid');
                $usernameMessage.addClass('errorWarning');
                ShowErrorMessage('The super doesn’t exist !');
                return;
            }

            $usernameMessage.addClass('successWarning');
            if (callback) {
                callback();
            }
        }).always(function () {
            Fanex.ExtAcc.UnBlockUI();
        });
    }

    function HideMessage() {
        $message.hide();
        $message.removeClass();
    }

    function ShowErrorMessage(message) {
        $message.show();
        $message.addClass('error');
        $message.html(message);
    }

    function ShowSuccessMessage(message) {
        $message.show();
        $message.addClass('success');
        $message.html(message);
    }

    Fanex.ExtAcc.SearchAndCopy = function () {
        Init();
    }
})(jQuery);

$(document).ready(function () {
    Fanex.ExtAcc.SearchAndCopy();
});