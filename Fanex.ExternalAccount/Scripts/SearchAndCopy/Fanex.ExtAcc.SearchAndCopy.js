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
        $validationMessage,
        $txtUsername,
        $txtPassword,
        $txtFirstName,
        $txtLastName,
        $txtPhone,
        $txtMobilePhone,
        $txtFax,
        $txtMaxCredit,
        $notAllowedSuperPrefixes,
        passwordTemplate,
        existingUsername,
        sccode;

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
        $validationMessage = $('#validation_message');
        $txtUsername = $('#txtUsername');
        $txtPassword = $('#txtPassword');
        $txtFirstName = $('#txtFirstName');
        $txtLastName = $('#txtLastName');
        $txtPhone = $('#txtPhone');
        $txtMobilePhone = $('#txtMobilePhone');
        $txtFax = $('#txtFax');
        $txtMaxCredit = $('#txtMaxCredit');
        $notAllowedSuperPrefixes = $('#notAllowedSuperPrefixes');
        passwordTemplate = $('#passwordTemplate').html();
        sccode = $('#SCCode').val();

        GetNotAllowedSuperPrefixes();

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
            ShowErrorMessage('Username is required !');
            $txtUsername.addClass('invalid');
            return;
        }

        var password = $.trim($txtPassword.val());
        if (!password) {
            ShowErrorMessage('Password is required !');
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
            ShowErrorMessage('Max Credit is required !');
            $txtMaxCredit.addClass('invalid');
            return;
        }

        Fanex.ExtAcc.BlockUI();
        var requestParams = {
            username: username,
            password: password.encrypt(sccode),
            firstName: $.trim($txtFirstName.val()),
            lastName: $.trim($txtLastName.val()),
            phone: $.trim($txtPhone.val()),
            mobilePhone: $.trim($txtMobilePhone.val()),
            fax: $.trim($txtFax.val()),
            credit: maxCredit,
            existingUsername: existingUsername,
            groupName: $('#selectedGroupName').html() === '' ? 'ACCDDD' : $('#selectedGroupName').html()
        };
        $.post(resolveClientUrl('SearchAndCopy/SuperPost'), requestParams, function (result) {
            if (Fanex.ExtAcc.ShowSecurityCodePage(result)) {
                return;
            }

            if ((result.IsSuccess !== undefined && !result.IsSuccess) || (result.isSuccess !== undefined && !result.isSuccess)) {
                ShowErrorMessage(result.ErrorMessage || result.errorMessage);
                return;
            }

            ShowSuccessMessage("Created Super Successfully !");
        }).always(function () {
            Fanex.ExtAcc.UnBlockUI();
        });
    }

    function GotoStep1() {
        HideMessage();
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

        var searchParams = {
            username: username
        };
        $usernameMessage.removeClass();
        $usernameMessage.addClass('loadingWarning');
        $usernameMessage.show();
        $.post(resolveClientUrl('SearchAndCopy/SearchCustomer'), searchParams, function (result) {
            $usernameMessage.removeClass();
            if (!result.IsSuccess) {
                $username.addClass('invalid');
                $usernameMessage.addClass('errorWarning');
                ShowErrorMessage(result.ErrorMessage);
                return;
            }

            existingUsername = username;
            $usernameMessage.addClass('successWarning');
            if (callback) {
                callback();
            }
        });
    }

    function HideMessage() {
        $validationMessage.hide();
        $validationMessage.removeClass();
    }

    function ShowErrorMessage(message) {
        $validationMessage.show();
        $validationMessage.addClass('error');
        $validationMessage.html(message);
    }

    function ShowSuccessMessage(message) {
        $validationMessage.show();
        $validationMessage.addClass('success');
        $validationMessage.html(message);
    }

    function GetNotAllowedSuperPrefixes() {
        var params = {
            prefix: null,
            allowNewSuper: false
        };

        $.post(resolveClientUrl('SearchAndCopy/GetSuperPrefixes'), params, function (data) {
            if (data && data.length > 0) {
                $notAllowedSuperPrefixes.addClass('row-info');
                $notAllowedSuperPrefixes.html(`* The following username prefixes are not allowed for new supers anymore: ${data[0].Prefixes}`);
            } else {
                $notAllowedSuperPrefixes.removeClass('row-info');
                $notAllowedSuperPrefixes.html('');
            }
        });
    }

    Fanex.ExtAcc.SearchAndCopy = function () {
        Init();
    };
})(jQuery);

$(document).ready(function () {
    Fanex.ExtAcc.SearchAndCopy();
});