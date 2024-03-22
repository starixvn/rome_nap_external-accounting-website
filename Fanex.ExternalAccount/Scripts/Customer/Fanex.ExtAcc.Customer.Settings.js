/// <reference path="../_references.js" />

if (typeof Fanex == 'undefined' || typeof Fanex.ExtAcc == 'undefined') {
    alert("Fanex.ExtAcc.js module is required!");
};

if (typeof Fanex.ExtAcc.Customer == 'undefined') {
    Fanex.ExtAcc.Customer = {};
};

Fanex.ExtAcc.Customer.Settings = {
    UserId: Fanex.ExtAcc.Customer.UserId,
    Master: 3,
    Agent: 2,
    Member: 1,
    CurrentLevel: parseInt(Fanex.ExtAcc.Customer.CurrentLevel),

    Init: function () {
        Fanex.ExtAcc.Customer.UserId = this.UserId;
        Fanex.ExtAcc.Customer.IsMultiple = false;
        $('#tabs').tabs();

        $('#tab4').css('display', Fanex.ExtAcc.Customer.HideCasino ? 'none' : 'block');
        $('#tab8').css('display', Fanex.ExtAcc.Customer.HideVirtualSports ? 'none' : 'block');

        $('#tab4').bind('click', function () {
            Fanex.ExtAcc.Customer.Settings.RenderDCS(6, "#casinoFrame", "#tabs-4");
        });

        $('#tab8').bind('click', function () {
            Fanex.ExtAcc.Customer.Settings.RenderDCS(5, "#virtualSportsFrame", "#tabs-8");
        });

        $('.dynamic-setting-tab').bind('click', function () {
            var productId = $(this).attr('data-product-id');
            Fanex.ExtAcc.Customer.Settings.RenderDCS(productId, "#Frame_" + productId, "#tabs-" + productId);
        });

        $('#tabs-list>li:first').click();
    },

    RenderDCS: function (productId, frameId, tabId) {
        Fanex.ExtAcc.Customer.IsMultiple = false;
        Fanex.ExtAcc.Customer.UserId = this.UserId;
        var action = '';
        var url = "DCS/ViewCustomerSetting?language=en-US&productId=" + productId + "&customerId=" + this.UserId + "&groupName=" + parent.$('#selectedGroupName').html() + "&customerLevel=";

        if (this.CurrentLevel == Fanex.ExtAcc.Customer.Master) {
            url += 3;
        }
        else if (this.CurrentLevel == Fanex.ExtAcc.Customer.Agent) {
            url += 2;
        }
        else {
            url += 1 + "&UplineLevel=2";
        }
        $(tabId).height(parent.$('#leftFrame').height() - 10);

        $(tabId).css('display', 'block');
        Fanex.ExtAcc.Customer.Settings.LoadProductSetting(frameId, resolveClientUrl(url));
    },

    LoadProductSetting: function (frameId, url) {
        Fanex.ExtAcc.BlockUI();
        var iframe = $(frameId);
        iframe.attr('onload', 'Fanex.ExtAcc.UnBlockUI()');
        iframe.attr('src', url);
    }
};

$(document).ready(function () {
    Fanex.ExtAcc.Customer.Settings.Init();
});