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

        $('#tab8').css('display', Fanex.ExtAcc.Customer.HideVirtualSports ? 'none' : 'block');
        $('#tab9').css('display', Fanex.ExtAcc.Customer.HideKeno ? 'none' : 'block');
        $('#tab10').css('display', Fanex.ExtAcc.Customer.HideGoldDeluxe ? 'none' : 'block');
        $('#tab11').css('display', Fanex.ExtAcc.Customer.HideColossusBets ? 'none' : 'block');
        $('#tab14').css('display', Fanex.ExtAcc.Customer.HideCricket ? 'none' : 'block');        

        $('#tab1').bind('click', function () {
            Fanex.ExtAcc.Customer.Settings.EmptyAllTabs();
            Fanex.ExtAcc.Customer.Settings.GetSportsbookMinPT();
        });
        
        $('#tab4').bind('click', function () {
            Fanex.ExtAcc.Customer.Settings.EmptyAllTabs();
            Fanex.ExtAcc.Customer.Settings.GetCasinoSetting();
        });        

        $('#tab8').bind('click', function () {
            Fanex.ExtAcc.Customer.Settings.EmptyAllTabs();
            Fanex.ExtAcc.Customer.Settings.RenderDCS(5, "#virtualSportsFrame", "#tabs-8");
        });

        $('#tab9').bind('click', function () {
            Fanex.ExtAcc.Customer.Settings.EmptyAllTabs();
            Fanex.ExtAcc.Customer.Settings.GetKenoSetting();
        });

        $('#tab10').bind('click', function () {
            Fanex.ExtAcc.Customer.Settings.EmptyAllTabs();
            Fanex.ExtAcc.Customer.Settings.GetGoldDeluxeSetting();
        });

        $('#tab11').bind('click', function () {
            Fanex.ExtAcc.Customer.Settings.EmptyAllTabs();
            Fanex.ExtAcc.Customer.Settings.GetColossusBetsSetting();
        });
        
       
        $('#tab14').bind('click', function () {
            Fanex.ExtAcc.Customer.Settings.EmptyAllTabs();
            Fanex.ExtAcc.Customer.Settings.RenderDCS(20, "#cricketFrame", "#tabs-14");
        });

        $('.dynamic-setting-tab').bind('click', function () {
            var productId = $(this).attr('data-product-id');
            Fanex.ExtAcc.Customer.Settings.EmptyAllTabs();
            Fanex.ExtAcc.Customer.Settings.RenderDCS(productId, "#Frame_" + productId, "#tabs-" + productId);
        });

        $('#tabs-list>li:first').click();
    },

    GetSportsbookMinPT: function () {
        Fanex.ExtAcc.Customer.IsMultiple = false;

        Fanex.ExtAcc.Customer.UserId = this.UserId;
        $.ajax({
            url: resolveClientUrl('SportsbookSetting/EditMinPT?custId=' + Fanex.ExtAcc.Customer.Settings.UserId + '&groupName=' + $('#selectedGroupName').html())
        }).success(function (result) {
            $('#tabs-1').css('display', 'block');
            $('#tabs-1').html(result);
            Fanex.ExtAcc.UnBlockUI();
        });
    },



    GetCasinoSetting: function () {
        Fanex.ExtAcc.Customer.IsMultiple = false;
        Fanex.ExtAcc.Customer.UserId = this.UserId;
        var action = '';

        if (this.CurrentLevel == Fanex.ExtAcc.Customer.Master) {
            action = 'Master/EditMaster';
        }
        else if (this.CurrentLevel == Fanex.ExtAcc.Customer.Agent) {
            action = 'Agent/EditAgent';
        }
        else {
            action = 'Member/EditMember';
        }

        Fanex.ExtAcc.Customer.Ids = function () {
            return [this.UserId];
        };

        Fanex.ExtAcc.Customer.Names = function () {
            return [''];
        };

        var url = resolveClientUrl('CustomerSetting/Casino/' + action + '?id=' + this.UserId + '&uplineid=' + Fanex.ExtAcc.Customer.superId + '&groupname=' + $('#selectedGroupName').html());

        $('#tabs-4').height($('#leftFrame').height() - 10);
        $('#tabs-4').css('display', 'block');
        Fanex.ExtAcc.Customer.Settings.LoadProductSetting('#casinoFrame', url);
    },

    GetKenoSetting: function () {
        Fanex.ExtAcc.Customer.IsMultiple = false;
        Fanex.ExtAcc.Customer.UserId = this.UserId;
        var action = '';

        if (this.CurrentLevel == Fanex.ExtAcc.Customer.Master) {
            action = 'customersetting/kenosetting/kenosetting/editmaster';
        }
        else if (this.CurrentLevel == Fanex.ExtAcc.Customer.Agent) {
            action = 'customersetting/kenosetting/kenosetting/EditAgent';
        }
        else {
            action = 'customersetting/kenosetting/kenosetting/AgentEditMember';
        }

        Fanex.ExtAcc.Customer.Ids = function () {
            return [this.UserId];
        };

        Fanex.ExtAcc.Customer.Names = function () {
            return [''];
        };

        var url = resolveClientUrl(action + "?custId=" + this.UserId + '&userId=' + userId + '&groupName=' + $('#selectedGroupName').html());

        $('#tabs-9').height($('#leftFrame').height() - 10);
        $('#tabs-9').css('display', 'block');
        Fanex.ExtAcc.Customer.Settings.LoadProductSetting('#kenoFrame', url);
    },   


    GetGoldDeluxeSetting: function () {
        Fanex.ExtAcc.Customer.IsMultiple = false;
        Fanex.ExtAcc.Customer.UserId = this.UserId;
        var action = '';
        var url = '';

        if (this.CurrentLevel == Fanex.ExtAcc.Customer.Master) {
            url = "GoldDeluxeSetting/GoldDeluxe/EditSetting?upLineLevel=4&formType=1&level=3" + "&uplineId=" + Fanex.ExtAcc.Customer.superId;
        }
        else if (this.CurrentLevel == Fanex.ExtAcc.Customer.Agent) {
            url = "GoldDeluxeSetting/GoldDeluxe/EditSetting?upLineLevel=3&formType=1&level=2" + "&uplineId=" + Fanex.ExtAcc.Customer.masterId;
        }
        else {
            url = "GoldDeluxeSetting/GoldDeluxe/EditSetting?upLineLevel=2&formType=1&level=1" + "&uplineId=" + Fanex.ExtAcc.Customer.agentId;
        }


        var url = resolveClientUrl(url + "&actorId=" + parent.userId + "&custId=" + this.UserId + "&groupName=" + $('#selectedGroupName').html());
        $('#tabs-10').height($('#leftFrame').height() - 10);
        $('#tabs-10').css('display', 'block');
        Fanex.ExtAcc.Customer.Settings.LoadProductSetting('#goldDeluxeFrame', url);
    },

    GetColossusBetsSetting: function () {
        Fanex.ExtAcc.Customer.IsMultiple = false;
        Fanex.ExtAcc.Customer.UserId = this.UserId;
        var action = '';
        var url = 'customersetting/ColossusBetSetting/ColossusBetSetting/EditSetting?custid=' + this.UserId + '&isMultipleEdit=0&userId=' + parent.userId;

        if (this.CurrentLevel == Fanex.ExtAcc.Customer.Master) {
            url += "&srecommend=" + Fanex.ExtAcc.Customer.superId + '&roleId=3';
        }
        else if (this.CurrentLevel == Fanex.ExtAcc.Customer.Agent) {
            url += "&mrecommend=" + Fanex.ExtAcc.Customer.masterId + '&roleId=2';
        }
        else {
            url += "&recommend=" + Fanex.ExtAcc.Customer.agentId + '&roleId=1';
        }


        var url = resolveClientUrl(url);
        $('#tabs-11').height($('#leftFrame').height() - 10);
        $('#tabs-11').css('display', 'block');
        Fanex.ExtAcc.Customer.Settings.LoadProductSetting('#colossusBetsFrame', url);
    },

    RenderDCS: function (productId, frameId, tabId) {
        Fanex.ExtAcc.Customer.IsMultiple = false;
        Fanex.ExtAcc.Customer.UserId = this.UserId;
        var action = '';
        var url = "DCS/ViewCustomerSetting?language=en-US&productId=" + productId + "&customerId=" + this.UserId + "&groupName=" + $('#selectedGroupName').html() + "&customerLevel=";

        if (this.CurrentLevel == Fanex.ExtAcc.Customer.Master) {
            url += 3;
        }
        else if (this.CurrentLevel == Fanex.ExtAcc.Customer.Agent) {
            url += 2;
        }
        else {
            url += 1 + "&UplineLevel=2";
        }
        $(tabId).height($('#leftFrame').height() - 10);

        $(tabId).css('display', 'block');
        Fanex.ExtAcc.Customer.Settings.LoadProductSetting(frameId, resolveClientUrl(url));
    },

    EmptyAllTabs: function () {
        $('#tabs-1').html('');
        // $('#tabs-7').html('');

        $('#tabs-1').css('display', 'none');
        // $('#tabs-7').css('display', 'none');
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