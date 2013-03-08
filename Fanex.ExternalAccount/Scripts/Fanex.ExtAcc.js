/// <reference path="_references.js" />

if (typeof Fanex == 'undefined') {
    Fanex = {};
}

Array.prototype.binarySearch = function (key) {
    var value = this;

    function match(value, target) {
        return key ? value[key] === target[key] : value === target;
    }

    function compare(value, target) {
        return key ? value[key] > target[key] : value > target;
    }

    var startIndex = 0,
        stopIndex = this.length - 1,
        middle = Math.floor((stopIndex + startIndex) / 2);

    while (!match(this[middle], value) && startIndex < stopIndex) {
        //adjust search area
        if (compare(value, this[middle])) {
            stopIndex = middle - 1;
        } else if (compare(this[middle], value)) {
            startIndex = middle + 1;
        }

        //recalculate middle
        middle = Math.floor((stopIndex + startIndex) / 2);
    }

    //make sure it's the right value
    return (!match(this[middle], value)) ? -1 : middle;
};

String.prototype.formatErrorMessage = function () {
    var msg = this;

    var indexOfStartParenthesis = msg.indexOf('(');
    var indexOfEndParenthesis = msg.indexOf(')');
    var orgMsg = msg.substring(indexOfStartParenthesis, indexOfEndParenthesis + 1);
    var preMsg = msg.substring(0, indexOfStartParenthesis);
    var subMsg = msg.substring(indexOfEndParenthesis + 1, msg.length);

    var formatMsg = '<font color="#ff5411">' + preMsg + '</font>' + orgMsg +
    '<font color="#ff5411">' + subMsg + '</font>';

    var msg = subMsg.indexOf(':') > 0 ? ('<div style="float: left; color: Blue; font-weight: bold">' + subMsg.substring(0, subMsg.indexOf(':')) + ':</div><br /><font color="#ff5411">' + subMsg.substring(subMsg.indexOf(':') + 1) + '</font>') : ('<font color="#ff5411">' + subMsg + '</font>');

    var ret = preMsg.length > 0 ? formatMsg : (orgMsg + msg);

    return ret;
};

Fanex.ExtAcc = {
    $this: null,
    BlockedUI: false,
    theme: 'classic',
    leftMenuWidth: Cookie.get('layout_1') ? parseInt(Cookie.get('layout_1')) : 300,

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

    InitSuggestion: function () {
        var groupName = $('#selectedGroupName').html();

        $.ajax(resolveClientUrl("Filter/GetSuggestedUserName?username=" + $("#txtUserName").val() + "&groupname=" + groupName))
        .success(function (data) {
            var source = [];

            $.each(data, function (i) {
                source.push(data[i].Username);
            });

            $("#txtUserName").autocomplete({
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

        $.blockUI.defaults.css.border = '0px solid #aaa',
        $.blockUI.defaults.css.backgroundColor = "transparent";
        $.blockUI.defaults.overlayCSS.backgroundColor = "transparent";

        $('body').height($("#footer").position().top);
        $('#container').height($("#footer").position().top);
        $('#splitterBar').height($("#footer").position().top - 30);

        $('#contentSplitter').width($("#footer").position().left - this.leftMenuWidth);

        $('#contentSplitter').css('height', $('#splitterBar').height() - 20 + 'px');

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

        $('#contentSplitter').css('height', $('#splitterBar').height() - 20 + 'px');
    },

    OnResize: function () { },

    Resize: function () {
        $('body').height($("#footer").position().top);
        $('#container').height($("#footer").position().top);
        $('#splitterBar').height($("#footer").position().top - 30);
        $('#contentSplitter').width($("#footer").position().left - this.leftMenuWidth);

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
    }
};

$(document).ready(function () {
    Fanex.ExtAcc.Init();
});