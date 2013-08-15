/// <reference path="_references.js" />

if (typeof Fanex == 'undefined') {
    Fanex = {};
}

String.prototype.decodeHtml = function () {
    var str = this;
    return $("<div/>").html(str).text();
};

String.prototype.encodeHtml = function(){
    var str = this;
    return $('<div/>').text(str).html();    
};

Fanex.decodeHtml = function (str) {
    return $("<div/>").html(str).text();
};

Fanex.encodeHtml = function(str){
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
    var hasUsername = preMsg.indexOf(':') != -1;
    var username = '';

    if (hasUsername) {
        username = preMsg.substring(0, preMsg.indexOf(':'));
    }

    var formatMsg = '<font color="#ff5411">' + preMsg + '</font>' + orgMsg +
    '<font color="#ff5411">' + subMsg + '</font>';

    var msg = '';

    if (!hasUsername) {
        msg = orgMsg + subMsg.indexOf(':') > 0 ? ('<div style="float: left; color: Blue; font-weight: bold">' + subMsg.substring(0, subMsg.indexOf(':')) + ':</div><br /><font color="#ff5411">' + subMsg.substring(subMsg.indexOf(':') + 1) + '</font>') : ('<font color="#ff5411">' + subMsg + '</font>');
    }
    else {
        msg = '<div style="float: left; color: Blue; font-weight: bold">' + username + ':</div><br /><font color="#ff5411">' + preMsg.substring(preMsg.indexOf(':') + 1, preMsg.length) + orgMsg + subMsg + '</font>';
    }

    var ret = preMsg.length > 0 && !hasUsername ? formatMsg : msg;

    return ret;
};

Fanex.ExtAcc = {
    $this: null,
    BlockedUI: false,
    theme: 'classic',
    leftMenuWidth: Cookie.get('layout_1') ? parseInt(Cookie.get('layout_1')) : 300,
    AutoInsertCompleted: false,
    AutoInsertCount: 0,

    LoadMainFrame: function (url, id) {
        this.BlockUI();

        if (typeof id != 'undefined') {
            $('#' + id).hide();
        }

        $.ajax({ url: url + '/' + $('#selectedGroupName').html(), type: 'post', success: function (result) {
            $('#contentSplitter').html(result);

            Fanex.ExtAcc.UnBlockUI();
        }
        })
        .error(function (data) {
            if (typeof console != undefined)
                console.log(data.responseText);

            Fanex.ExtAcc.UnBlockUI();
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
        if (!$this.BlockedUI) {
            $.blockUI({
                message: '<img src="' + resolveClientUrl('Images/Loading.gif') + '" />',
                onBlock: function () {
                    $this.BlockedUI = true;
                },
                onUnblock: function () {
                    $this.BlockedUI = false;
                }
            });
        }
    },

    UnBlockUI: function () {
        $.unblockUI(document);
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
            url: 'Statement/Index',
            module: 'Fanex.ExtAcc.Statement'
        },
        {
            url: 'Group/GroupList',
            module: 'Fanex.ExtAcc.Group'
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
        var groupName = $('#selectedGroupName').html();
        var id = typeof txtUserName == 'undefined' ? '#txtUserName' : '#' + txtUserName;
        console.log($(id));

        $.ajax(resolveClientUrl("Filter/GetSuggestedUserName?username=" + Fanex.encodeHtml($(id).val()) + "&groupname=" + groupName))
        .success(function (data) {
            var source = [];

            $.each(data, function (i) {
                source.push(data[i].Username);
            });

            $(id).autocomplete({
                source: source
            });
        })
        .error(function (err) {
            if (typeof console != 'undefined') {
                consolse.log(err);
            }
        });
    },

    Init: function () {
        $this = this;

        $.blockUI.defaults.css.border = '0px solid #aaa';
        $.blockUI.defaults.css.backgroundColor = "transparent";
        $.blockUI.defaults.overlayCSS.backgroundColor = "transparent";

        $('body').height($("#footer").position().top);
        $('#container').height($("#footer").position().top);
        $('#splitterBar').height($("#footer").position().top - 30);

        $('#contentSplitter').width($("#footer").position().left - this.leftMenuWidth);

        $('#contentSplitter').css('height', $('#splitterBar').height() - 20 + 'px');

        $('#leftSplitter').height($('#contentSplitter').height());
        $('#leftFrame').height($('#contentSplitter').height() - 35);
        $('#mainFrame').height($('#contentSplitter').height() - 35);

        $('#splitterBar').click(function (event) {
            Fanex.ExtAcc.ToggleSpliter();
            Fanex.ExtAcc.Resize();
        });

        $(window).resize(function () {
            Fanex.ExtAcc.Resize();
        });

        if (this.leftMenuWidth === 300) {
            $('#leftSplitter').show();
            $('#splitterBar').attr('title', 'Hide left panel');
            $('#splitterBar div').attr('class', 'expanded');
        }
        else {
            $('#leftSplitter').hide();
            $('#splitterBar').attr('title', 'Show left panel');
            $('#splitterBar div').attr('class', 'collapsed');
        }

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
                var username = $.trim(this.value.encodeHtml());

                $('#leftFrame').attr('src', resolveClientUrl('CustomerSetting/LeftPanel/SearchCustomer/Index/' + (username == '' ? "''" : username) + '/' + $('#selectedGroupName').html()));

                return true;
            } else {
                return true;
            }
        });

        $('#btnSearch').bind('click', function () {
            var username = $.trim($('#txtLeftPanelUserName').val().toString().encodeHtml());

            $('#leftFrame').attr('src', resolveClientUrl('CustomerSetting/LeftPanel/SearchCustomer/Index/' + (username == '' ? "''" : username) + '/' + $('#selectedGroupName').html()));
        });

        $('#txtLeftPanelUserName').keyup(function (e) {
            if ($('#txtLeftPanelUserName').val().length > 1) {
                Fanex.ExtAcc.InitSuggestion('txtLeftPanelUserName');
            }
        });

        Fanex.ExtAcc.InitSuggestion('txtLeftPanelUserName');
    },

    ToggleSpliter: function () {
        if (this.leftMenuWidth === 300) {
            this.leftMenuWidth = 5;
            $('#leftSplitter').hide();
            $('#splitterBar').attr('title', 'Show left panel');
            $('#splitterBar div').attr('class', 'collapsed');
        }
        else {
            this.leftMenuWidth = 300;
            $('#leftSplitter').show();
            $('#splitterBar').attr('title', 'Hide left panel');
            $('#splitterBar div').attr('class', 'expanded');
        };

        $('#leftSplitter').height($('#contentSplitter').height());
        $('#leftFrame').height($('#contentSplitter').height() - 30);
        $('#mainFrame').height($('#contentSplitter').height() - 30);

        if (document.getElementById('tabs-4')) {
            $('#tabs-4').height($('#contentSplitter').height() - 55);
        }

        this.Resize();
    },

    OnResize: function () { },

    Resize: function () {
        $('body').height($("#footer").position().top);
        $('#container').height($("#footer").position().top);
        $('#splitterBar').height($("#footer").position().top - 30);

        $('#contentSplitter').width($("#footer").position().left - this.leftMenuWidth);
        $('#contentSplitter').css('height', $('#splitterBar').height() - 20 + 'px');

        try {
            $("#jqgrid").fanexGrid("resizeGrid");
        }
        catch (err) {
        }

        $('#leftSplitter').height($('#contentSplitter').height() + 5);
        $('#leftFrame').height($('#contentSplitter').height() - 30);
        $('#mainFrame').height($('#contentSplitter').height() - 30);

        if (document.getElementById('tabs-4')) {
            $('#tabs-4').height($('#contentSplitter').height() - 55);
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
            var item = $(document).find('[highlightatt=highlight-att-main-' + fncIds[i] + '-' + fncIds[i] + ']');

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

    AutoInsertCasino: function (custId, uplineId, uplineName) {
        $.ajax({
            url: resolveClientUrl('Customer/AutoInsertCasinoPost'),
            type: 'post',
            data: { custId: custId, uplineId: uplineId, uplineName: uplineName },
            dataType: 'json'
        })
        .success(function (result) {
            if (Fanex.ExtAcc.AutoInsertCount < 3) {
                ++Fanex.ExtAcc.AutoInsertCount;
            }
            if (Fanex.ExtAcc.AutoInsertCount >= 3) {
                Fanex.ExtAcc.AutoInsertCount = 0;
                Fanex.ExtAcc.AutoInsertCompleted = true;
            }
        })
        .error(function (data) {
            if (typeof console != 'undefined') {
                console.log(data.responseText);
            }

            if (Fanex.ExtAcc.AutoInsertCount < 3) {
                ++Fanex.ExtAcc.AutoInsertCount;
            }
            if (Fanex.ExtAcc.AutoInsertCount >= 3) {
                Fanex.ExtAcc.AutoInsertCount = 0;
                Fanex.ExtAcc.AutoInsertCompleted = true;
            }
        });
    },

    AutoInsertBingo: function (custId, uplineId, uplineName) {
        $.ajax({
            url: resolveClientUrl('Customer/AutoInsertBingoPost'),
            type: 'post',
            data: { custId: custId, uplineId: uplineId, uplineName: uplineName },
            dataType: 'json'
        })
        .success(function (result) {
            if (Fanex.ExtAcc.AutoInsertCount < 3) {
                ++Fanex.ExtAcc.AutoInsertCount;
            }
            if (Fanex.ExtAcc.AutoInsertCount >= 3) {
                Fanex.ExtAcc.AutoInsertCount = 0;
                Fanex.ExtAcc.AutoInsertCompleted = true;
            }
        })
        .error(function (data) {
            if (typeof console != 'undefined') {
                console.log(data.responseText);
            }

            if (Fanex.ExtAcc.AutoInsertCount < 3) {
                ++Fanex.ExtAcc.AutoInsertCount;
            }
            if (Fanex.ExtAcc.AutoInsertCount >= 3) {
                Fanex.ExtAcc.AutoInsertCount = 0;
                Fanex.ExtAcc.AutoInsertCompleted = true;
            }
        });
    },

    AutoInsertLiveCasino: function (custId, parentId, uplineRoleId, groupName) {
        $.ajax({
            url: resolveClientUrl('Customer/AutoInsertLiveCasinoPost'),
            type: 'post',
            data: { custId: custId, parentId: parentId, uplineRoleId: uplineRoleId, groupName: groupName },
            dataType: 'json'
        })
        .success(function (result) {
            if (Fanex.ExtAcc.AutoInsertCount < 3) {
                ++Fanex.ExtAcc.AutoInsertCount;
            }
            if (Fanex.ExtAcc.AutoInsertCount >= 3) {
                Fanex.ExtAcc.AutoInsertCount = 0;
                Fanex.ExtAcc.AutoInsertCompleted = true;
            }
        })
        .error(function (data) {
            if (typeof console != 'undefined') {
                console.log(data.responseText);
            }

            if (Fanex.ExtAcc.AutoInsertCount < 3) {
                ++Fanex.ExtAcc.AutoInsertCount;
            }
            if (Fanex.ExtAcc.AutoInsertCount >= 3) {
                Fanex.ExtAcc.AutoInsertCount = 0;
                Fanex.ExtAcc.AutoInsertCompleted = true;
            }
        });
    },

    CheckNumber: function (evt, ele, agentCredit) {
        ele.value = Fanex.ExtAcc.FormatNumber(Fanex.ExtAcc.ReplaceAll(ele.value, ',', ''));

        if (agentCredit) {
            Fanex.ExtAcc.CheckConstraintAgentCredits(agentCredit);
        }

        var charCode = (evt.which) ? evt.which : event.keyCode;
        var ret = true;

        if (charCode > 31 && (charCode < 48 || charCode > 57)) {
            ret = false;
        }

        if (ele && ele.value.length > 1 && ele.value.indexOf('0') == 0) {
            while (ele.value.indexOf('0') == 0 && ele.value.length > 1) {
                ele.value = ele.value.substring(1);
                ele.value = Fanex.ExtAcc.FormatNumber(Fanex.ExtAcc.ReplaceAll(ele.value, ',', ''));
            }
        }
        else if (ele && ele.value.length == 0) {
            ele.value = '0';
        }

        return ret;
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
                    msg += '<font color="Blue">' + msgs[i].errorMessage + '</font><br />';
                }
                else {
                    msg += '<font color="Red">' + msgs[i].errorMessage + '</font><br />';
                }
            }

            return '<font color="Blue" style="font-weight: bold">' + username + ':</font><br />' + msg;
        }
        else if (msgs instanceof Object) {
            if (msgs.errorCode == "" || msgs.errorCode == "0" || parseInt(msgs.errorCode) <= 0 || (msgs.isSuccess && msgs.isSuccess == true)) {
                return '<font color="Blue" style="font-weight: bold">' + username + ':</font><br />' + '<font color="Blue">' + msgs.errorMessage + '</font><br />';
            }
            else {
                return '<font color="Blue" style="font-weight: bold">' + username + ':</font><br />' + '<font color="Red">' + msgs.errorMessage + '</font><br />';
            }
        }

        return '';
    }
};

$(document).ready(function () {
    Fanex.ExtAcc.Init();
});