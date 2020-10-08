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

    code: '',
    currentIFrame: null,
    failedCount: 0,
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
    GetFailedCount: function () {
        return this.failedCount;
    },
    IncrementFailedCount: function () {
        this.failedCount++;
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
    },
    IsExternalSite: function () {
        return this.returnUrl.indexOf('/updateCustomerSetting/updateSetting') > -1;
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
        this.code = this.$txtSecurityCode.val();

        if (!this.code.trim()) {
            this.$txtSecurityCode.focus();
            this.ShowError('This field is required.');
            return;
        }

        $.post(this.verifyUrl, { code: this.code })
            .done(function (result) {
                if (result === 'valid') {
                    Fanex.ExtAcc.SecurityCode.OnValidated();
                    return;
                }

                if (Fanex.ExtAcc.SecurityCode.GetFailedCount() < 5) {
                    Fanex.ExtAcc.SecurityCode.IncrementFailedCount();
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

        var setTopLeft = function (top, left) {
            scBlockOverlay.css('top', top);
            if (Fanex.ExtAcc.SecurityCode.IsExternalSite()) {
                scBlockOverlay.css('left', left);
            } else {
                scBlockOverlay.css('left', '37.77%');
            }
        };

        var currentIFrameId = this.currentIFrame ? this.currentIFrame.attr('id') : '';
        if (currentIFrameId.indexOf('mainIFrame') > -1) {
            setTopLeft('99px', '33.33%');
        } else if (currentIFrameId.indexOf('virtualSportsFrame') > -1 || currentIFrameId.indexOf('Frame_') > -1) {
            setTopLeft('30%', '35%');
        } else if (currentIFrameId.indexOf('product-') > -1) {
            setTopLeft('5rem', '22.75%');
        } else {
            scBlockOverlay.css('top', '30%');
            scBlockOverlay.css('left', '37.77%');
        }

        scBlockOverlay.css('display', 'block');
    }
};

Fanex.ExtAcc.SecurityCode.Init();