/// <reference path="_references.js" />

if (typeof Fanex == 'undefined') {
    Fanex = {};
}

String.prototype.replaceAll = function (args, replace) {
    var src = this;
    while (src.indexOf(args) != -1) {
        src = src.replace(args, replace);
    }

    return src;
};

String.prototype.FormatNumber = function (number) {
    number += '';

    x = number.split('.');
    x1 = x[0];
    x2 = x.length > 1 ? '.' + x[1] : '';
    var rgx = /(\d+)(\d{3})/;
    while (rgx.test(x1)) {
        x1 = x1.replace(rgx, '$1' + ',' + '$2');
    }
    return x1 + x2;
};

String.prototype.formatNumber = function (number) {
    number += '';

    x = number.split('.');
    x1 = x[0];
    x2 = x.length > 1 ? '.' + x[1] : '';
    var rgx = /(\d+)(\d{3})/;
    while (rgx.test(x1)) {
        x1 = x1.replace(rgx, '$1' + ',' + '$2');
    }
    return x1 + x2;
};

String.prototype.decodeHtml = function () {
    var str = this;
    return $("<div/>").html(str).text();
};

String.prototype.encodeHtml = function () {
    var str = this;
    return $('<div/>').text(str).html();
};

Fanex.decodeHtml = function (str) {
    return $("<div/>").html(str).text();
};

Fanex.encodeHtml = function (str) {
    return $('<div/>').text(str).html();
};

String.prototype.formatErrorMessage = function () {
    var msg = this;
    var indexOfStartParenthesis = msg.indexOf('(');
    var indexOfEndParenthesis = msg.indexOf(')');
    var orgMsg = msg.substring(indexOfStartParenthesis, indexOfEndParenthesis + 1);
    var preMsg = msg.substring(0, indexOfStartParenthesis);
    var subMsg = msg.substring(indexOfEndParenthesis + 1, msg.length);

    var tmp = msg.substring(indexOfStartParenthesis, indexOfStartParenthesis + 4);
    var hasUsername = preMsg.indexOf(':') != -1 && preMsg.indexOf('GMT+8') != -1;
    var username = '';

    if ($.isNumeric(orgMsg.substring(1, orgMsg.length - 1))) {
        orgMsg = '(' + orgMsg.substring(1, orgMsg.length - 1) + ')';
    }

    if (hasUsername) {
        username = preMsg.substring(0, preMsg.indexOf(':'));
    }

    var formatMsg = '<div id="msg">' + preMsg + orgMsg + subMsg + '</div>';

    var msg = '';
    if (!hasUsername) {
        msg = orgMsg + subMsg.indexOf(':') > 0 ? ('<div class="username">' + subMsg.substring(0, subMsg.indexOf(':')) + ':</div><div id="msg">' + subMsg.substring(subMsg.indexOf(':') + 1) + '</div>') : ('<div id="msg">' + subMsg + '</div>');
    }
    else {
        msg = '<div class="username">' + username + ':</div><div id="msg">' + preMsg.substring(preMsg.indexOf(':') + 1, preMsg.length) + orgMsg + subMsg + '</div>';
    }

    $('#errMsg').addClass('errMsg');
    $('#errMsg').removeClass('successMsg');

    var ret = preMsg.length > 0 && !hasUsername ? formatMsg : msg;

    return ret;
};

String.prototype.formatSuccessMessage = function () {
    $('#errMsg').removeClass('errMsg');
    $('#errMsg').addClass('successMsg');

    return this;
};

String.prototype.splitAndContains = function (strSearched) {
    var sourceStr = this;
    var sourceSplited = sourceStr.split(",").map(function (item) {
        return item.trim().toUpperCase();
    });

    return sourceSplited.indexOf(strSearched.toUpperCase()) !== -1;
}

Fanex.ExtAcc = {
    $this: null,
    BlockedUI: false,
    theme: 'classic',
    leftMenuWidth: Cookie.get('layout_1') ? parseInt(Cookie.get('layout_1')) : 300,
    AutoInsertCompleted: false,
    AutoInsertCount: 0,
    SuggestionXHR: null,
    shouldReloadLeftPanel: false,

    scPageId: 'securityCodePage',
    $scPageContainer: null,

    DecodeHtml: function (str) {
        return $("<div/>").html(str).text();
    },

    EncodeHtml: function (str) {
        return $('<div/>').text(str).html();
    },

    LoadMainIFrame: function (url, resizeIframe) {
        this.BlockUI();
        $('#contentSplitter').html('');
        //$('#contentSplitter').height($('#mainContent').height() - 20);
        var iframe = document.createElement('iframe');
        iframe.setAttribute('id', 'mainIFrame');
        iframe.setAttribute('src', url);
        iframe.setAttribute('frameborder', 0);
        iframe.setAttribute('width', ($('#contentSplitter').width() - 5) + 'px');
        iframe.setAttribute('onload', resizeIframe ? 'resizeIframe(this)' : 'Fanex.ExtAcc.UnBlockUI()');
        $('#contentSplitter').append(iframe);
    },

    LoadMainFrame: function (url, id, isLeftPanelClick, dontNeedGroupName) {
        this.BlockUI();
        $('#contentSplitter').hide();

        var fullUrl = dontNeedGroupName ? url : url + '/' + $('#selectedGroupName').html();
        $.ajax({
            url: fullUrl, type: 'post', success: function (result) {
                if (Fanex.ExtAcc.ShowSecurityCodePage(result, Fanex.ExtAcc.UnBlockUI)) {
                    return;
                }

                $('#contentSplitter').html('');

                if (url.indexOf('Customer/CustomerList') === -1 || $(result).text().indexOf('You are not authorized on this function') !== -1) {
                    Fanex.ExtAcc.UnBlockUI();
                }

                if (result.errorCode) {
                    $('#contentSplitter').html(result.errorMessage.formatErrorMessage());
                    return;
                }

                $('#contentSplitter').html(result);

                if (document.getElementById('btnLogin')) {
                    location.href = resolveClientUrl('Account/Login');
                    return;
                }

                if (!isLeftPanelClick && url.indexOf('Customer/CustomerList') != -1) {
                    var interval = setInterval(function () {
                        if (document.getElementById('btnSearch')) {
                            clearInterval(interval);
                            Fanex.ExtAcc.Customer.superId = 0;
                            Fanex.ExtAcc.Customer.masterId = 0;
                            Fanex.ExtAcc.Customer.agentId = 0;
                            Fanex.ExtAcc.Customer.CurrentLevel = 4;
                            Fanex.ExtAcc.Customer.Init();

                            var timeout = setTimeout(function () {
                                clearTimeout(timeout);
                                if ($.trim($('#jqgrid').html()) == '' && !Fanex.ExtAcc.Customer.IsLoading && $('#msgNoinfo').css('display') == 'none') {
                                    $('#btnSearch').click();
                                }
                            }, 2000);
                        }
                    }, 100);
                }
            }
        })
            .error(function (data) {
                Fanex.ExtAcc.UnBlockUI();
                if (data.errorCode) {
                    $('#contentSplitter').html(data.errorMessage.formatErrorMessage());
                } else {
                    $('#contentSplitter').html(data.responseText).show();
                }
            })
            .done(function (data) {
                try {
                    $('#contentSplitter').ready(function () {
                        //$('#contentSplitter').height($('#mainContent').height() - 20);
                        $('#contentSplitter').show();

                    });
                } finally {
                    Fanex.ExtAcc.UnBlockUI();
                }
            });
    },

    FormatNumber: function (number) { // Format a number to 1,234,567.89
        number += '';

        x = number.split('.');
        x1 = x[0];
        x2 = x.length > 1 ? '.' + x[1] : '';
        var rgx = /(\d+)(\d{3})/;
        while (rgx.test(x1)) {
            x1 = x1.replace(rgx, '$1' + ',' + '$2');
        }
        return x1 + x2;
    },

    EscapeHtml: function (string) {
        var result = '';
        if (string.length < 1) {
            return result;
        }

        for (var i = 0; i < string.length; i++) {
            if (string.charAt(i) == "&" && string.length - i - 1 >= 4 && string.substr(i, 4) != "&amp;") {
                result = result + "&amp;";
            } else if (string.charAt(i) == "<") {
                result = result + "&lt;";
            } else if (string.charAt(i) == ">") {
                result = result + "&gt;";
            } else {
                result = result + string.charAt(i);
            }
        }

        return result;
    },

    BlockUI: function () {
        if (!this.BlockedUI) {
            $(document).scrollTop(0);
            $('#windowPopup').scrollTop(0);
            $('#contentSplitter').scrollTop(0);
            $.blockUI({
                message: '<img src="' + resolveClientUrl('Assets/styles/Images/loading.gif') + '" />',
                onBlock: function () {
                    Fanex.ExtAcc.BlockedUI = true;
                },
                onUnblock: function () {
                    Fanex.ExtAcc.BlockedUI = false;
                }
            });
        }
    },

    UnBlockUI: function () {
        $.unblockUI();
    },

    modules: [
        {
            url: 'DailyWinloss/Index',
            module: 'Fanex.ExtAcc.DailyWinLoss'
        },
        {
            url: 'WinlossDetail/Index',
            module: 'Fanex.ExtAcc.WinLossDetail'
        },
        {
            url: 'WinLossDetailIBCBetLiveCasino/Index',
            module: 'Fanex.ExtAcc.WinLossDetailIBCBetLiveCasino'
        },
        {
            url: 'Statement/Index',
            module: 'Fanex.ExtAcc.Statement'
        },
        {
            url: 'Group/GroupList',
            module: 'Fanex.ExtAcc.Group'
        },
        {
            url: 'SubGroup/SubGroupList',
            module: 'Fanex.ExtAcc.SubGroup'
        },
        {
            url: 'Function/FunctionList',
            module: 'Fanex.ExtAcc.Function'
        },
        {
            url: 'User/UserList',
            module: 'Fanex.ExtAcc.User'
        },
        {
            url: 'Menu/MenuList',
            module: 'Fanex.ExtAcc.Menus'
        },
        {
            url: 'Customer/CustomerList',
            module: 'Fanex.ExtAcc.Customer'
        }
    ],

    LoadModule: function (module) {
        if (typeof module == 'object' && typeof module.Init == 'function') {
            module.Init();
        }
        else {
            if (typeof console != 'undefined') console.log('Could not load module: ');
            if (typeof console != 'undefined') console.log(module);
        }
    },

    GetCurrentModule: function () {
        var q = window.location.href.split('?')[0].toLowerCase();
        var s = window.location.href.split('#')[0].toLowerCase();
        var url = q > s ? s : q;

        for (var i = 0; i < this.modules.length; i++) {
            if (url.split(this.modules[i].url.toLowerCase()).length > 1) {
                return eval(this.modules[i].module);
            }
        }
    },

    InitSuggestion: function (txtUserName) {
        if (this.SuggestionXHR != null)
            this.SuggestionXHR.abort();

        var groupName = $('#selectedGroupName').html();
        var id = typeof txtUserName == 'undefined' ? '#txtUserName' : '#' + txtUserName;
        this.SuggestionXHR = $.ajax(resolveClientUrl("Filter/GetSuggestedUserName?username=" + Fanex.encodeHtml($(id).val()) + "&groupname=" + groupName))
            .success(function (data) {
                var source = [];

                $.each(data, function (i) {
                    source.push(data[i].Username);
                });

                $(id).autocomplete({
                    source: source
                });

                Fanex.ExtAcc.SuggestionXHR = null;
            })
            .error(function (xhr, status, err) {
                Fanex.ExtAcc.SuggestionXHR = null;
            });
    },

    Init: function () {
        $this = this;

        this.$scPageContainer = $('#securityCodePageContainer');

        $.blockUI.defaults.css.border = '0px solid #aaa';
        $.blockUI.defaults.css.backgroundColor = "transparent";
        $.blockUI.defaults.overlayCSS.backgroundColor = "transparent";

        $('#leftFrame').height($('#contentSplitter').height() - 40);
        $('#mainFrame').height($('#contentSplitter').height() - 35);

        var leftSplitter = $("#leftSplitter");
        var iconCollapseExpand = $("#collapseExpandOnDesktop");
        var iconUser = $('#iconUserInfo');
        var content = $("#contentSplitter");
        var splitterBar = $("#splitterBar");
        var iconNav = $("#iconNav");
        var sidebar = $("#sidebar");

        var widthDevice = $(window).width();

        if (widthDevice >= 768 && widthDevice <= 1366) {
            iconUser.on("click", function () {
                if (iconUser.hasClass("active")) {
                    leftSplitter.addClass("collapse-leftpanel").removeClass("expand-leftpanel");
                    content.removeClass("collapse-content").addClass("expand-content");
                    iconUser.removeClass("active");
                } else {
                    leftSplitter.removeClass("collapse-leftpanel").addClass("expand-leftpanel");
                    content.addClass("collapse-content");
                    iconUser.addClass("active");
                }
                $(window).resize();
            });
        } else {
            $(window).bind('orientationchange', function (e) {
                if (widthDevice < 768) {
                    $this.shouldReloadLeftPanel = true;
                }
            });
            iconUser.on("click", function () {
                if (iconUser.hasClass("active")) {
                    iconUser.removeClass("active");
                    leftSplitter.removeClass("expand-leftpanel").addClass("collapse-leftpanel");
                    content.removeClass("collapse-content").addClass("expand-content");
                } else {
                    iconUser.addClass("active");
                    leftSplitter.removeClass("collapse-leftpanel").addClass("expand-leftpanel");
                    content.removeClass("expand-content").addClass("collapse-content");
                    if ($this.shouldReloadLeftPanel) {
                        document.getElementById('leftFrame').contentDocument.location.reload(false);
                    }

                }
                $this.shouldReloadLeftPanel = false;

                iconNav.removeClass('active');
                sidebar.removeClass('expand-sidebar').addClass('collapse-sidebar');
                $(window).resize();
            });
        }


        $(window).resize(function () {
            Fanex.ExtAcc.Resize();
        });
        this.LoadModule(Fanex.ExtAcc.Form);
        var interval = setInterval(function () {
            if (typeof Fanex.ExtAcc.Header != 'undefined') {
                clearInterval(interval);

                Fanex.ExtAcc.LoadModule(Fanex.ExtAcc.Header);

                var module = Fanex.ExtAcc.GetCurrentModule();
                Fanex.ExtAcc.LoadModule(module);
            }
        }, 100);

        $('#txtLeftPanelUserName').keypress(function (e) {
            if (typeof e == 'undefined') e = window.event;
            var key = (typeof e.keyCode == 'undefined') ? e.which : e.keyCode;
            if (key == 13) {
                if (this.SuggestionXHR != null)
                    this.SuggestionXHR.abort();

                Fanex.ExtAcc.BlockUI();
                var username = this.value.formatString();
                this.value = '';

                $('.ui-autocomplete').hide();

                $("#leftFrame").css("visibility", "hidden");
                $('#leftFrame').attr('src', resolveClientUrl('CustomerSetting/LeftPanel/SearchCustomer/Index?custName=' + (username == '' ? "''" : username) + '&groupName=' + $('#selectedGroupName').html() + '&pageIndex=1'));

                var widthDevice = $(window).width();
                if (/Android|webOS|iPhone|iPad|iPod|BlackBerry|IEMobile|Opera Mini/i.test(navigator.userAgent)) {
                    $('#txtLeftPanelUserName').blur();
                }
                else {
                    setTimeout(function () {
                        $('#txtLeftPanelUserName').focus();
                    }, 100);
                }

            }

            return true;
        });

        $('#searchPanel').bind('click', function () {
            if (this.SuggestionXHR != null)
                this.SuggestionXHR.abort();

            Fanex.ExtAcc.BlockUI();
            var username = $('#txtLeftPanelUserName').val().toString().formatString();
            $('#txtLeftPanelUserName').val('');

            $('.ui-autocomplete').hide();
            $("#leftFrame").css("visibility", "hidden");
            $('#leftFrame').attr('src', resolveClientUrl('CustomerSetting/LeftPanel/SearchCustomer/Index?custName=' + (username == '' ? "''" : username) + '&groupName=' + $('#selectedGroupName').html() + '&pageIndex=1'));
        });

        $('#leftFrame')
            .off('load.leftFrame')
            .on('load.leftFrame', function () {
                Fanex.ExtAcc.UnBlockUI();
            });

        $('#txtLeftPanelUserName').keyup(function (e) {
            if ($('#txtLeftPanelUserName').val().length > 1) {
                Fanex.ExtAcc.InitSuggestion('txtLeftPanelUserName');
            }
        });

        //Fanex.ExtAcc.InitSuggestion('txtLeftPanelUserName');

        setTimeout(Fanex.ExtAcc.CalculateUserConnectionSpeed, 8000);
    },

    CalculateUserConnectionSpeed: function () {
        var iframe = document.getElementById('connection-speed');

        if (typeof (iframe) == 'undefined'
            || iframe == null) {
            iframe = document.createElement('iframe');
            iframe.id = 'connection-speed';
            iframe.style.display = 'none';
            iframe.src = resolveClientUrl("NetworkSpeed/Index");
            document.body.appendChild(iframe);
        }
    },
    ViewUser: function (username, pageIndex) {
        this.BlockUI();

        username = username.formatString();
        $("#leftFrame").css("visibility", "hidden");
        $('#leftFrame').attr('src', resolveClientUrl('CustomerSetting/LeftPanel/SearchCustomer/Index?custName=' + (username == '' ? "''" : username) + '&groupName=' + $('#selectedGroupName').html() + '&pageIndex=' + pageIndex));
    },

    ToggleSpliter: function () {
        //if (this.leftMenuWidth === 300) {
        //    this.leftMenuWidth = 5;
        //    $('#leftSplitter').hide();
        //    $('#splitterBar').attr('title', 'Show left panel');
        //    $('#splitterBar div').attr('class', 'collapsed');
        //}
        //else {
        //    this.leftMenuWidth = 300;
        //    $('#leftSplitter').show();
        //    $('#splitterBar').attr('title', 'Hide left panel');
        //    $('#splitterBar div').attr('class', 'expanded');
        //};

        //$('#leftSplitter').height($('#contentSplitter').height());
        $('#leftFrame').height($('#contentSplitter').height() - 35);
        //$('#mainFrame').height($('#contentSplitter').height() - 30);

        if (document.getElementById('mainIFrame')) {
            $('#mainIFrame').width($('#contentSplitter').width() - 5);
            $('#mainIFrame').height($('#contentSplitter').height() - 5);
        }

        if (document.getElementById('tabs-2')) {
            $('#tabs-2').height($('#contentSplitter').height() - 40);
        }

        if (document.getElementById('tabs-4')) {
            $('#tabs-4').height($('#contentSplitter').height() - 40);
        }

        if (document.getElementById('tabs-5')) {
            $('#tabs-5').height($('#contentSplitter').height() - 40);
        }

        this.Resize();
    },

    OnResize: function () { },

    Resize: function () {
        //$('body').height($("#footer").position().top);
        //$('#container').height($("#footer").position().top);
        //$('#splitterBar').height($("#footer").position().top - 30);

        //$('#contentSplitter').width($("#footer").position().left - this.leftMenuWidth);
        //$('#contentSplitter').css('height', $('#splitterBar').height() - 20 + 'px');

        if (document.getElementById('mainIFrame')) {
            $('#mainIFrame').width($('#contentSplitter').width() - 5);
            $('#mainIFrame').height($('#contentSplitter').height() - 5);
        }

        try {
            $("#jqgrid").fanexGrid("resizeGrid");
        }
        catch (err) {
        }

        //$('#leftSplitter').height($('#contentSplitter').height() + 5);
        $('#leftFrame').height($('#contentSplitter').height() - 35);
        //$('#mainFrame').height($('#contentSplitter').height() - 30);

        if (document.getElementById('tabs-2')) {
            $('#tabs-2').height($('#contentSplitter').height() - 40);
        }

        if (document.getElementById('tabs-4')) {
            $('#tabs-4').height($('#contentSplitter').height() - 40);
        }

        if (document.getElementById('tabs-5')) {
            $('#tabs-5').height($('#contentSplitter').height() - 40);
        }

        // Update to cookie
        Cookie.set('layout_1', this.leftMenuWidth, 4);

        this.OnResize();
    },

    ToggleRows: function (fncId) {
        var hasChild = false;
        var items = $('tr');
        var id = document.getElementById('parent-' + fncId).getAttribute('expand');

        for (var i = 0; i < items.length; i++) {
            if (items[i].getAttribute('parentid') == fncId) {
                if (id == 'true') {
                    // Collapse all children
                    items[i].style.display = 'none';
                }
                else {
                    // Expand all children
                    items[i].style.display = 'table-row';
                }

                hasChild = true;
            }
        }

        if (hasChild) {
            if (id == 'true') {
                document.getElementById('parent-' + fncId).setAttribute('expand', false);
                $('#parent-' + fncId).removeClass('permItemParent');
                $('#parent-' + fncId).addClass('itemParentCollapse1');
            }
            else {
                document.getElementById('parent-' + fncId).setAttribute('expand', true);
                $('#parent-' + fncId).removeClass('itemParentCollapse1');
                $('#parent-' + fncId).addClass('permItemParent');
            }
        }
    },

    TogglejqRows: function (parentId, fncIds) {
        for (var i = 0; i < fncIds.length; i++) {
            var item = $(document).find('[highlightatt=highlight-att-main-' + fncIds[i] + '-' + fncIds[i] + '-body]');

            if (item.css('display') == 'none') {
                item.css('display', 'table-row');
                $('#fn-' + parentId).removeClass('itemParentCollapse2');
                $('#fn-' + parentId).addClass('itemParent');
            }
            else {
                item.css('display', 'none');
                $('#fn-' + parentId).removeClass('itemParent');
                $('#fn-' + parentId).addClass('itemParentCollapse2');
            }

            $("#jqgrid").fanexGrid("resizeGrid");
        }
    },

    ReplaceAll: function (src, args, replace) {
        while (src.indexOf(args) != -1) {
            src = src.replace(args, replace);
        }

        return src;
    },

    CheckNumber: function (event, ele, isFloat, agentCredit) {
        var evt = event ? event : window.event; // Fix for IE7

        if (evt.keyCode === 9) return true; // Ignore TAB key

        var obj = evt.currentTarget ? evt.currentTarget : evt.srcElement; // Fix for IE7
        var value = obj.value;

        if (value == '') return true;

        if (agentCredit) {
            this.CheckConstraintAgentCredits(agentCredit);
        }

        var num;
        var dotPosition = 0;
        var afterDot;
        var caretPosition = getCaret(obj);

        num = value.toString().replace(/\$|\,/g, '');
        dotPosition = num.indexOf(".");
        if ((typeof (isFloat) != 'undefined') && isFloat && dotPosition > 0) {
            afterDot = num.substring(dotPosition + 1, num.length).replace(/[^0-9]/g, '');
            num = num.substring(0, dotPosition);
        }
        num = num.formatFloat();
        if ((typeof (isFloat) != 'undefined') && isFloat && dotPosition > 0) {
            eval(obj).value = num + "." + afterDot;
            num = obj.value;
        } else {
            eval(obj).value = num;
        }

        if (num.length > value.length) {
            caretPosition = caretPosition + 1;
        }

        setCaret(obj, caretPosition);

        return true;
    },

    CheckConstraintAgentCredits: function (agentCredit) {
        var value = Fanex.ExtAcc.ReplaceAll(agentCredit.value, ',', '');
        $('#maxCreditMember').text(Fanex.ExtAcc.FormatNumber(value));
    },

    FormatErrorMessage: function (username, obj) {
        var msgs = obj;
        var msg = '';

        if (msgs instanceof Array) {
            for (var i = 0; i < msgs.length; i++) {
                if (msgs[i].errorCode == "" || msgs[i].errorCode == "0" || parseInt(msgs[i].errorCode) <= 0 || (msgs[i].isSuccess && msgs[i].isSuccess == true)) {
                    msg += '<font color="Blue">' + msgs[i].errorMessage.formatSuccessMessage() + '</font><br />';
                }
                else {
                    msg += '<font color="Red">' + msgs[i].errorMessage.formatErrorMessage() + '</font><br />';
                }
            }

            return '<font color="Blue" style="font-weight: bold">' + username + ':</font><br />' + msg + '<br />';
        }
        else if (msgs instanceof Object) {
            if (msgs.errorCode == "" || msgs.errorCode == "0" || parseInt(msgs.errorCode) <= 0 || (msgs.isSuccess && msgs.isSuccess == true)) {
                return '<font color="Blue" style="font-weight: bold">' + username + ':</font><br />' + '<font color="Blue">' + msgs.errorMessage.formatSuccessMessage() + '</font><br /><br />';
            }
            else {
                return '<font color="Blue" style="font-weight: bold">' + username + ':</font><br />' + '<font color="Red">' + msgs.errorMessage.formatErrorMessage() + '</font><br /><br />';
            }
        }

        return '';
    },

    AddPtInfoPage: function (custId) {
        if ($('.icon-alert-circle-outline').length > 0) {
            return;
        }
        var headerPt = $('.headerPositionTaking').last();
        var infoDiv = document.createElement('DIV');
        infoDiv.setAttribute('title', 'Click to view PT info');
        infoDiv.setAttribute('class', 'icon-alert-circle-outline');
        infoDiv.setAttribute('style', 'float: right; margin: 5px; z-index: 999999');
        infoDiv.innerHTML = '&nbsp;';

        infoDiv.setAttribute('onclick', "window.open(resolveClientUrl('SportsbookSetting/ViewPtInfo?custId="+custId+"'), '_blank')");

        headerPt.append(infoDiv);
    },

    ShowSecurityCodePage: function (result, unBlockUI = null) {
        if (typeof result === 'string' && result.indexOf(this.scPageId) > -1) {
            if (typeof unBlockUI === 'function') {
                unBlockUI();
            }

            this.$scPageContainer.html(result);

            return true;
        }

        return false;
    }
};

function setCaret(oField, iCaretPos) {
    // IE Support
    if (document.selection) {
        oField.focus();
        // Create empty selection range
        var oSel = document.selection.createRange();
        // Move selection start and end to desired position
        oSel.moveStart('character', iCaretPos);
        oSel.moveEnd('character', iCaretPos - oField.value.length);
        oSel.select();
    }
    // Firefox support
    else if (oField.selectionStart || oField.selectionStart == '0') {
        oField.selectionStart = iCaretPos;
        oField.selectionEnd = iCaretPos;
        oField.focus();
    }
};

function getCaret(oField) {
    var iCaretPos = 0;
    // IE Support
    if (document.selection) {
        oField.focus();
        // To get cursor position, get empty selection range
        var oSel = document.selection.createRange();
        // Move selection start to 0 position
        oSel.moveStart('character', -oField.value.length);
        iCaretPos = oSel.text.length;
    }
    // Firefox support
    else if (oField.selectionStart || oField.selectionStart == '0')
        iCaretPos = oField.selectionStart;
    // Return results
    return (iCaretPos);
};

String.prototype.formatFloat = function () {
    var number = this;

    if (number == '') return 0;
    var sign;
    if (isNaN(number)) {
        number = '';
    }

    sign = (number == (number = Math.abs(number)));
    number = Math.floor(number * 100 + 0.50000000001);
    number = Math.floor(number / 100).toString();

    for (var i = 0; i < Math.floor((number.length - (1 + i)) / 3); i++) {
        number = number.substring(0, number.length - (4 * i + 3)) + ',' + number.substring(number.length - (4 * i + 3));
    }

    return (((sign) ? '' : '-') + number);
};

String.prototype.toFixed = function (precision, optionals) {
    var value = parseFloat(this);

    var power = Math.pow(10, precision),
        output;

    // Multiply up by precision, round accurately, then divide and use native toFixed():
    output = (Math.round(value * power) / power).toFixed(precision);

    if (optionals) {
        var optionalsRegExp = new RegExp('0{1,' + optionals + '}$');
        output = output.replace(optionalsRegExp, '');
    }

    return output;
};

String.prototype.formatString = function () {
    var src = escape(this);

    return src.search(/^[<\s\s*>]/) == -1 ? $.trim(src.encodeHtml().replaceAll('/', '').replaceAll('.', '').replaceAll('+', '')) : '';
};

$(document).ready(function () {
    Fanex.ExtAcc.Init();

    $.ajaxSetup({
        beforeSend: function (xhr) {
            var antiXSRFToken = document.getElementsByName("__RequestVerificationToken");
            if (antiXSRFToken.length > 0) {
                xhr.setRequestHeader("__RequestVerificationToken", antiXSRFToken[0].value);
            }
        },
        error: function () {
            Fanex.ExtAcc.UnBlockUI();
        },
        statusCode: {
            500: function () {
                Fanex.ExtAcc.Form.Close();
                Fanex.ExtAcc.LoadMainFrame(resolveClientUrl('Shared/Error'));
            },
            401: function (error, callback) {
                if (JSON && error.responseText) {
                    var errorResult = JSON.parse(error.responseText);
                    if (errorResult.code === 999999) {
                        top.location.href = resolveClientUrl('Shared/Offline');
                    }
                    if (errorResult.code == 401) {
                        top.location.href = resolveClientUrl('Account/Login');
                    }
                }
            }
        }
    });
});


function isValid(obj) {
    if (typeof obj != "undefined" && obj != null) {
        return true;
    }
    return false;
}

function clone(obj) {
    var target = {};
    for (var i in obj) {
        if (obj.hasOwnProperty(i)) {
            target[i] = obj[i];
        }
    }
    return target;
}

Array.prototype.clone = function () {
    return this.slice(0);
};

Array.prototype.contains = function (obj) {
    var i = this.length;
    while (i--) {
        if (this[i] === obj) {
            return true;
        }
    }
    return false;
}

function setPagerOptionOnMobile(gridOption) {
    if (isMobile.phone && gridOption.pagerOption != null) {
        gridOption.pagerOption.numDisplayEntries = 2;
        gridOption.pagerOption.numEdgeEntries = 1;
    }
}

function workaroundForGridOnPhone() {
    if (isMobile.phone) {
        // TODO: this is workaround for android browsers, header auto transparent when scrolling

        if (isMobile.android.device) {
            $('#jqgrid .grid-container')
                .unbind('scroll.workaround.grid')
                .bind('scroll.workaround.grid',
                    function () {
                        var firstHeaderColumn = $("#jqgrid .row-headers-background table tr th div.table-cell").first();
                        var firstHeaderColumnTop = $("#jqgrid .free-top-left table tr th div.table-cell").first();
                        if (firstHeaderColumn && firstHeaderColumnTop) {
                            var currentWidth = firstHeaderColumnTop.width();
                            if (currentWidth) {
                                firstHeaderColumn.width(currentWidth + 1).width(currentWidth - 1);
                            }
                        }
                    });
        }

        $(window)
            .unbind('orientationchange.grid')
            .bind('orientationchange.grid',
                function (event) {
                    setTimeout(function () {
                        $('#jqgrid').fanexGrid("resizeHeight", $(window).height() - 70);
                    }, 300);
                });
    }
}

function isNullOrUndefined(obj) {
    return obj == null;
}

function isNullOrUndefinedOrEmpty(obj) {
    obj = obj === '' ? null : obj;
    return isNullOrUndefined(obj);
}

function onPruductStatusSettingSuccess(result, callback) {
    if (typeof result === 'string' && result.indexOf('securityCodePage') > -1) {
        if (typeof callback === 'function') {
            callback();
        }

        $('#securityCodePageContainer').html(result);

        return false;
    }

    return true;
}

function resizeIframe(obj) {
    obj.style.minHeight = obj.contentWindow.document.documentElement.scrollHeight + 'px';
    obj.style.minWidth = obj.contentWindow.document.documentElement.scrollWidth + 'px';
    Fanex.ExtAcc.UnBlockUI();
}