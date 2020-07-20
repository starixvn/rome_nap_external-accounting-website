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
        this.InitClosePageEventHandler();
        this.SetInputFocused();

        this.returnUrl = $('input#returnUrl').val();
        this.currentIFrame = this.GetCurrentIFrame();

        var currentIFrameId = this.currentIFrame ? this.currentIFrame.attr('id') : '';
        if (currentIFrameId.indexOf('mainIFrame') > -1) {
            $('div.securitycode-block-overlay').css('top', '99px');
            if (this.IsExternalSite()) {
                $('div.securitycode-block-overlay').css('left', '33.33%');
            }
            return;
        }
        if ((currentIFrameId.indexOf('virtualSportsFrame') > -1 || currentIFrameId.indexOf('Frame_') > -1) && this.IsExternalSite()) {
            $('div.securitycode-block-overlay').css('left', '35%');
            return;
        }
        if (currentIFrameId.indexOf('product-') > -1) {
            $('div.securitycode-block-overlay').css('top', '5rem');
            if (this.IsExternalSite()) {
                $('div.securitycode-block-overlay').css('left', '22.75%');
            }
            return;
        }
    },
    IsExternalSite: function () {
        return this.returnUrl.indexOf('/updateCustomerSetting/updateSetting') > -1;
    },
    OnValidated: function () {
        if (!this.IsExternalSite()) {
            $('#securityCodePageContainer').html('');
            return;
        }

        var currentIFrameSrc = this.currentIFrame ? this.currentIFrame.attr('src') : '';
        this.currentIFrame.attr('src', currentIFrameSrc);
    },
    OnShowSecurityCode: function (event) {
        if (event.target.checked) {
            $('input#code').prop('type', 'text');
        } else {
            $('input#code').prop('type', 'password');
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
        this.code = $('input#code').val();

        if (!this.code.trim()) {
            this.SetInputFocused();
            this.ShowError('The security code must not be empty.');
            return;
        }

        $.post(this.verifyUrl, { code: this.code })
            .done(function (result) {
                if (result === 'valid') {
                    Fanex.ExtAcc.SecurityCode.OnValidated();
                    return;
                }

                if (Fanex.ExtAcc.SecurityCode.GetFailedCount() < 5) {
                    $('input#code').val('');

                    Fanex.ExtAcc.SecurityCode.IncrementFailedCount();
                    Fanex.ExtAcc.SecurityCode.ShowError('The security code is invalid.');
                    return;
                }

                Fanex.ExtAcc.SecurityCode.KnockOut();
            })
            .fail(function (error) {
                console.log(error);
            })
            .always(function () {
                Fanex.ExtAcc.SecurityCode.SetInputFocused();
            });
    },
    ShowError: function (errorMessage) {
        $('span#codeError').text(errorMessage);
        $('span#codeError').removeClass('hidden');
    },
    SetInputFocused: function () {
        $('input#code').focus();
    },
    KnockOut: function () {
        top.location.href = resolveClientUrl('Account/LogOff');
    },
    InitClosePageEventHandler: function () {
        $(document).bind('keydown', function (event) {
            if (event.which === 27) {
                Fanex.ExtAcc.SecurityCode.OnValidated();
            }
        });
    },
    OnClosePage: function () {
        Fanex.ExtAcc.SecurityCode.OnValidated();
    }
};

Fanex.ExtAcc.SecurityCode.Init();