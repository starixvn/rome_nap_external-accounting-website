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
    },
    GetFailedCount: function () {
        return this.failedCount;
    },
    IncrementFailedCount: function () {
        this.failedCount++;
    },
    OnValidated: function () {
        if (this.returnUrl.indexOf('/updateCustomerSetting/updateSetting') === -1) {
            $('#securityCodePageContainer').html('');
            return;
        }

        var currentIFrame = this.GetCurrentIFrame();
        var currentIFrameSrc = currentIFrame.attr('src');
        currentIFrame.attr('src', currentIFrameSrc);
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
        this.returnUrl = $('input#returnUrl').val();

        if (!this.code.trim()) {
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
                    $("label#codeError").removeClass('hidden');

                    Fanex.ExtAcc.SecurityCode.IncrementFailedCount();
                    return;
                }

                top.location.href = resolveClientUrl('Account/LogOff');
            })
            .fail(function (error) {
                console.log(error);
            })
            .always(function () {
                $('input#code').focus();
            });
    }
};