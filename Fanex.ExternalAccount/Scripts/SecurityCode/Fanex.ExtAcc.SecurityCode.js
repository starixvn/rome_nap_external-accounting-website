/*  Product Name   | Product Id
    Virtual Soports          5
    Sportbook 2             16
    Macau Games             22
    Virtual Games           38
    Live Casino             41
    Saba.games              42
    RNG Lottery             44
*/

String.prototype.contains = function (str) {
    return this.indexOf(str) > -1;
};

Fanex.ExtAcc.SecurityCode = {
    $scPageContainer: null,
    $message: null,
    $messageContent: null,
    $txtSecurityCode: null,
    $txtIsExternalSite: null,
    $cbShowSC: null,
    $btnSubmit: null,
    $scPageOverlay: null,
    $txtMaxAllowAttempt: null,
    $txtSecretKey: null,

    browserSize: {},
    isExternalSite: false,
    isMobile: false,
    isLandscape: false,
    maxAllowAttempt: 0,
    verifyUrl: resolveClientUrl('SecurityCode/Verify'),

    Init: function () {
        this.CacheDOMElements();

        this.isExternalSite = JSON.parse(this.$txtIsExternalSite.val().toLowerCase());
        this.browserSize = this.GetBrowserSize();
        this.isMobile = this.browserSize.width < 768;
        this.isLandscape = this.browserSize.width > this.browserSize.height;
        this.maxAllowAttempt = parseInt(this.$txtMaxAllowAttempt.val(), 10);

        this.AdjustPagePosition();
        this.InitClosePageEventHandler();
        this.RegisterEvents();
    },
    CacheDOMElements: function () {
        this.$scPageContainer = $('#securityCodePageContainer');
        this.$txtSecurityCode = $('#txtSecurityCode');
        this.$txtIsExternalSite = $('#txtIsExternalSite');
        this.$cbShowSC = $('#showSecurityCode');
        this.$btnSubmit = $('#btnSubmitSecurityCode');
        this.$scPageOverlay = $('.securitycode-page-overlay');
        this.$messageContent = $('#status-message-content');
        this.$message = $('#status-message');
        this.$txtMaxAllowAttempt = $('#txtMaxAllowAttempt');
        this.$txtSecretKey = $('#txtSecretKey');
    },
    OnValidated: function () {
        this.isExternalSite ? window.location.reload() : this.$scPageContainer.html('');
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
        if (event.key === 'Enter' || event.keyCode === 13 || event.which === 13) {
            event.preventDefault();

            Fanex.ExtAcc.SecurityCode.Verify();
        }
    },
    OnClick: function () {
        Fanex.ExtAcc.SecurityCode.Verify();
    },
    Verify: function () {
        var self = this,
            txtSecretKey = this.$txtSecretKey.val(),
            txtSecurityCode = this.$txtSecurityCode.val();

        if (!txtSecurityCode.trim()) {
            this.$txtSecurityCode.focus();
            this.ShowError('This field is required.');
            return;
        }

        $.post(this.verifyUrl, {
            txtSecurityCode: txtSecurityCode.encrypt(txtSecretKey),
            txtSecretKey
        })
            .done(function (result) {
                if (result.StatusCode === 1) {
                    self.OnValidated();
                    return;
                }

                if (result.FailedCounter < self.maxAllowAttempt) {
                    self.ShowError('Wrong Security Code. Please try again.');
                    return;
                }

                self.KnockOut();
            })
            .fail(function (error) {
                console.log(error);
            })
            .always(function () {
                self.$txtSecurityCode.focus();
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
        $(window).on('orientationchange', this.OnOrientationChange.bind(this));
    },
    KnockOut: function () {
        top.location.href = resolveClientUrl('Account/LogOff');
    },
    InitClosePageEventHandler: function () {
        $(document).bind('keydown', function (event) {
            if (event.key === 'Escape' || event.keyCode === 27 || event.which === 27) {
                Fanex.ExtAcc.SecurityCode.OnClosePage();
            }
        });
    },
    OnClosePage: function () {
        Fanex.ExtAcc.SecurityCode.OnValidated();
    },
    AdjustPagePosition: function () {
        var scBlockOverlay = $('div.securitycode-block-overlay'),
            calculatedLeft = 'calc(50% - 234px)',
            calculatedTop = `calc(50% - ${scBlockOverlay.innerWidth() / 2}px)`,
            setTopLeft = function (top, left) {
                scBlockOverlay.css({ top, left });
            };

        if (this.isMobile && this.isLandscape) {
            if (this.isExternalSite) {
                $('body').css({
                    'overflow-y': 'scroll',
                    'background-color': '#aaaaaa'
                });
                scBlockOverlay.css('margin', '.5em 0');
            }
            setTopLeft('auto', calculatedLeft);
        }
        if (this.isMobile && !this.isLandscape) {
            if (this.isExternalSite) {
                scBlockOverlay.css('margin', '.25em');
                setTopLeft('auto', 'auto');
            } else {
                scBlockOverlay.css('margin', '0 .25em');
                setTopLeft(calculatedTop, 'auto');
            }
        }
        if (!this.isMobile) {
            setTopLeft(calculatedTop, calculatedLeft);
        }

        scBlockOverlay.css('display', 'block');
    },
    GetBrowserSize: function () {
        var w = window,
            d = document,
            e = d.documentElement,
            g = d.getElementsByTagName('body')[0],
            width = w.innerWidth || e.clientWidth || g.clientWidth,
            height = w.innerHeight || e.clientHeight || g.clientHeight;

        return { width, height };
    },
    OnOrientationChange: function (event) {
        var angle = window.orientation || event.target.screen.orientation.angle;
        this.isLandscape = angle === -90 || angle === 90;

        var $dialog = parent.$('.ui-dialog'),
            $windowPopup = parent.$('#windowPopup'),
            $body = $('body'),
            $securityCodeBlock = $('div.securitycode-block');
        var height = this.isLandscape ? this.browserSize.width : this.browserSize.height;

        if (this.isMobile && this.isExternalSite) {
            var calculatedHeight = `calc(${height + 34}px + .5em`,
                calculatedTop = `calc(50% - ${height / 2 + 17}px - .25em)`;

            if (this.isLandscape) {
                $dialog.css({ top: 'auto', height: calculatedHeight });
                $windowPopup.height(height - 34);
                $securityCodeBlock.width(468);
            } else {
                $dialog.css({ top: calculatedTop, height: calculatedHeight });
                $windowPopup.height(height);
                $body.css('overflow-y', 'inherit');
                $securityCodeBlock.width('100%');
            }
        }
        if (!this.isMobile && this.isExternalSite) {
            this.isLandscape
                ? $body.css({ 'overflow-y': 'scroll', height })
                : $body.css({ 'overflow-y': 'inherit', height });
        }

        this.AdjustPagePosition();
    }
};

Fanex.ExtAcc.SecurityCode.Init();