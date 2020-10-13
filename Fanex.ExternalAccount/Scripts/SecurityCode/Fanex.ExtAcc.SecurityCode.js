/*  Product Name   | Product Id
    Virtual Soports          5
    Sportbook 2             16
    Macau Games             22
    NG2                     23
    Fishing World           24
    Virtual Games           38
    Live Casino             41
    Saba.club               42
    RNG Lottery             44
*/
Fanex.ExtAcc.SecurityCode = {
    $scPageContainer: null,
    $message: null,
    $messageContent: null,
    $txtSecurityCode: null,
    $txtReturnUrl: null,
    $cbShowSC: null,
    $btnSubmit: null,
    $scPageOverlay: null,
    $txtMaxAllowAttempt: null,

    currentIFrame: null,
    returnUrl: '',
    verifyUrl: resolveClientUrl('SecurityCode/Verify'),
    GetCurrentIFrame: function () {
        var mainFrame = parent.$('iframe#mainIFrame');
        if (mainFrame.length === 1) {
            return mainFrame;
        }

        var virtualSportsFrame = parent.$('iframe#virtualSportsFrame');
        var virtualSportsTab = parent.$('div#tabs-8');
        if (virtualSportsFrame.length === 1 &&
            virtualSportsTab.length === 1 &&
            virtualSportsTab.css('display') === 'block') {
            return virtualSportsFrame;
        }

        var productIds = [5, 16, 22, 23, 24, 38, 41, 42, 44];
        var productId;
        for (productId of productIds) {
            var productTab = parent.$('div#tabs-' + productId);
            if (productTab.length === 1 && productTab.css('display') === 'block') {
                var frame = parent.$('iframe#Frame_' + productId);
                if (frame.length === 1) {
                    return frame;
                }

                var product = parent.$('iframe#product-' + productId);
                return product;
            }
        }

        return null;
    },
    Init: function () {
        this.CacheDOMElements();

        this.returnUrl = this.$txtReturnUrl.val();
        this.currentIFrame = this.GetCurrentIFrame();

        this.AdjustPagePosition();
        this.InitClosePageEventHandler();
        this.RegisterEvents();
    },
    CacheDOMElements: function () {
        this.$scPageContainer = $('#securityCodePageContainer');
        this.$txtSecurityCode = $('#txtSecurityCode');
        this.$txtReturnUrl = $('#txtReturnUrl');
        this.$cbShowSC = $('#showSecurityCode');
        this.$btnSubmit = $('#btnSubmit');
        this.$scPageOverlay = $('.securitycode-page-overlay');
        this.$messageContent = $('#status-message-content');
        this.$message = $('#status-message');
        this.$txtMaxAllowAttempt = $('#txtMaxAllowAttempt');
    },
    IsExternalSite: function () {
        return this.returnUrl.indexOf('/updateCustomerSetting/updateSetting') > -1
            || this.returnUrl.indexOf('/updateCustomerSetting/updateMultipleSetting') > -1;
    },
    OnValidated: function () {
        if (!this.IsExternalSite()) {
            this.$scPageContainer.html('');
            return;
        }

        var currentIFrameSrc = this.currentIFrame ? this.currentIFrame.attr('src') : '';
        this.currentIFrame.attr('src', currentIFrameSrc);
    },
    OnShowSecurityCode: function (event) {
        if (event.target.checked) {
            Fanex.ExtAcc.SecurityCode.$txtSecurityCode.prop('type', 'text');
        } else {
            Fanex.ExtAcc.SecurityCode.$txtSecurityCode.prop('type', 'password');
        }
    },
    OnKeyUp: function (event) {
        if (event.target.value.trim().length < 6) {
            $(event.target).removeClass('valid').addClass('error');
        } else {
            $(event.target).removeClass('error').addClass('valid');
        }
    },
    OnKeyPress: function (event) {
        if (event.which === 13) {
            event.preventDefault();

            Fanex.ExtAcc.SecurityCode.Verify();
        }
    },
    OnClick: function () {
        Fanex.ExtAcc.SecurityCode.Verify();
    },
    Verify: function () {
        var maxAllowAttempt = parseInt(this.$txtMaxAllowAttempt.val(), 10);
        var txtSecurityCode = this.$txtSecurityCode.val();

        if (!txtSecurityCode.trim()) {
            this.$txtSecurityCode.focus();
            this.ShowError('This field is required.');
            return;
        }

        $.post(this.verifyUrl, { txtSecurityCode })
            .done(function (result) {
                if (result.StatusCode === 1) {
                    Fanex.ExtAcc.SecurityCode.OnValidated();
                    return;
                }

                if (result.FailedCounter < maxAllowAttempt) {
                    Fanex.ExtAcc.SecurityCode.ShowError('Wrong Security Code. Please try again.');
                    return;
                }

                Fanex.ExtAcc.SecurityCode.KnockOut();
            })
            .fail(function (error) {
                console.log(error);
            })
            .always(function () {
                Fanex.ExtAcc.SecurityCode.$txtSecurityCode.focus();
            });
    },
    ShowError: function (errorMessage) {
        this.$messageContent.text(errorMessage);
        this.$message.removeClass('hidden');
    },
    RegisterEvents: function () {
        this.$txtSecurityCode.val('').focus();
        this.$txtSecurityCode.keypress(this.OnKeyPress);
        this.$txtSecurityCode.keyup(this.OnKeyUp);
        this.$cbShowSC.change(this.OnShowSecurityCode);
        this.$btnSubmit.click(this.OnClick);
        this.$scPageOverlay.click(this.OnClosePage);
    },
    KnockOut: function () {
        top.location.href = resolveClientUrl('Account/LogOff');
    },
    InitClosePageEventHandler: function () {
        $(document).bind('keydown', function (event) {
            if (event.which === 27) {
                Fanex.ExtAcc.SecurityCode.OnClosePage();
            }
        });
    },
    OnClosePage: function () {
        Fanex.ExtAcc.SecurityCode.OnValidated();
    },
    AdjustPagePosition: function () {
        var scBlockOverlay = $('div.securitycode-block-overlay');

        var currentIFrameId = this.currentIFrame ? this.currentIFrame.attr('id') : '';
        if (currentIFrameId.indexOf('mainIFrame') > -1) {
            scBlockOverlay.css('top', '99px');
        } else if (currentIFrameId.indexOf('virtualSportsFrame') > -1 || currentIFrameId.indexOf('Frame_') > -1) {
            scBlockOverlay.css('top', '30%');
        } else if (currentIFrameId.indexOf('product-') > -1) {
            scBlockOverlay.css('top', '5rem');
        } else {
            scBlockOverlay.css('top', '30%');
        }
        
        scBlockOverlay.css('display', 'block');
    }
};

Fanex.ExtAcc.SecurityCode.Init();