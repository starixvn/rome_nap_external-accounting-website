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

        $('#tab4').css('display', Fanex.ExtAcc.Customer.SyncCasino ? 'block' : 'none');
        $('#tab5').css('display', Fanex.ExtAcc.Customer.SyncBingo ? 'block' : 'none');
        $('#tab6').css('display', Fanex.ExtAcc.Customer.DisableP2P ? 'none' : 'block');
        $('#tab7').css('display', Fanex.ExtAcc.Customer.SyncLiveCasino ? 'block' : 'none');
        $('#tab8').css('display', Fanex.ExtAcc.Customer.HideVirtualSports ? 'none' : 'block');
        $('#tab9').css('display', Fanex.ExtAcc.Customer.HideKeno ? 'none' : 'block');
        $('#tab10').css('display', Fanex.ExtAcc.Customer.HideGoldDeluxe ? 'none' : 'block');
        $('#tab10').css('display', Fanex.ExtAcc.Customer.HideGoldDeluxe ? 'none' : 'block');
        $('#tab11').css('display', Fanex.ExtAcc.Customer.HideColossusBets ? 'none' : 'block');
        $('#tab12').css('display', Fanex.ExtAcc.Customer.HideSportsbook2 ? 'none' : 'block');

        if (Fanex.ExtAcc.Customer.CurrentLevel != Fanex.ExtAcc.Customer.Member) {
            // Loads first tab (Sportsbook's Min PT)
            Fanex.ExtAcc.Customer.Settings.GetSportsbookMinPT();
        }
        else {
            // Loads first tab (Racing PT)
            Fanex.ExtAcc.Customer.Settings.GetRacingPT();

            $('#tab1').hide();
            $('#tabs-1').hide();
            $('#tabs').tabs({ selected: 1 });

            $('#tab3').hide();
            $('#tabs-3').hide();
        }

        $('#tab6').hide();
        $('#tabs-6').hide();

        $('#tab1').bind('click', function () {
            Fanex.ExtAcc.Customer.Settings.EmptyAllTabs();
            Fanex.ExtAcc.Customer.Settings.GetSportsbookMinPT();
        });

        $('#tab2').bind('click', function () {
            Fanex.ExtAcc.Customer.Settings.EmptyAllTabs();
            Fanex.ExtAcc.Customer.Settings.GetRacingPT();
        });

        $('#tab3').bind('click', function () {
            Fanex.ExtAcc.Customer.Settings.EmptyAllTabs();
            Fanex.ExtAcc.Customer.Settings.GetRacingMinPT();
        });

        $('#tab4').bind('click', function () {
            Fanex.ExtAcc.Customer.Settings.EmptyAllTabs();
            Fanex.ExtAcc.Customer.Settings.GetCasinoSetting();
        });

        $('#tab5').bind('click', function () {
            Fanex.ExtAcc.Customer.Settings.EmptyAllTabs();
            Fanex.ExtAcc.Customer.Settings.GetBingoSetting();
        });

        $('#tab6').bind('click', function () {
            Fanex.ExtAcc.Customer.Settings.EmptyAllTabs();
            Fanex.ExtAcc.Customer.Settings.GetPokerSetting();
        });

        $('#tab7').bind('click', function () {
            Fanex.ExtAcc.Customer.Settings.EmptyAllTabs();
            Fanex.ExtAcc.Customer.Settings.GetLiveCasinoSetting();
        });

        $('#tab8').bind('click', function () {
            Fanex.ExtAcc.Customer.Settings.EmptyAllTabs();
            Fanex.ExtAcc.Customer.Settings.GetVirtualSportSetting();
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
        $('#tab12').bind('click', function () {
            Fanex.ExtAcc.Customer.Settings.EmptyAllTabs();
            Fanex.ExtAcc.Customer.Settings.GetSportsbook2Setting();
        });
    },

    GetSportsbookMinPT: function () {
        Fanex.ExtAcc.Customer.IsMultiple = false;

        Fanex.ExtAcc.BlockUI();
        Fanex.ExtAcc.Customer.UserId = this.UserId;
        $.ajax({
            url: resolveClientUrl('SportsbookSetting/EditMinPT?custId=' + Fanex.ExtAcc.Customer.Settings.UserId + '&groupName=' + $('#selectedGroupName').html())
        }).success(function (result) {
            $('#tabs-1').css('display', 'block');
            $('#tabs-1').html(result);
            Fanex.ExtAcc.UnBlockUI();
        }).error(function (data) {
            if (typeof console != 'undefined') {
                console.log(data.responseText);
            }
            Fanex.ExtAcc.UnBlockUI();
        });
    },

    GetRacingPT: function () {
        Fanex.ExtAcc.Customer.IsMultiple = false;
        Fanex.ExtAcc.Customer.UserId = this.UserId;
        var action = '';
        var uplineId = 0;
        if (Fanex.ExtAcc.Customer.Settings.CurrentLevel == Fanex.ExtAcc.Customer.Master) {
            uplineId = Fanex.ExtAcc.Customer.superId;
            action = 'Master';
        }
        else if (Fanex.ExtAcc.Customer.Settings.CurrentLevel == Fanex.ExtAcc.Customer.Agent) {
            uplineId = Fanex.ExtAcc.Customer.masterId;
            action = 'Agent';
        }
        else {
            if (Fanex.ExtAcc.Customer.isSuperEditMember) {
                uplineId = Fanex.ExtAcc.Customer.superId;
                action = 'SuperEditMember';
            }
            else if (Fanex.ExtAcc.Customer.isMasterEditMember) {
                uplineId = Fanex.ExtAcc.Customer.masterId;
                action = 'MasterEditMember';
            }
            else {
                uplineId = Fanex.ExtAcc.Customer.agentId;
                action = 'Member';
            }
        }
        var url = resolveClientUrl('CustomerSetting/Racebook/RacingSetting/' + action + '?id=' + this.UserId + '&uplineid=' + uplineId + '&groupName=' + $('#selectedGroupName').html());
        $('#tabs-2').height($('#leftFrame').height() - 10);
        $('#tabs-2').css('display', 'block');
        $('#racingFrame').attr('src', url);
    },

    GetRacingMinPT: function () {
        Fanex.ExtAcc.Customer.IsMultiple = false;
        Fanex.ExtAcc.Customer.UserId = this.UserId;
        var typeUrl = '';
        var custId = this.UserId;
        var roleId = this.CurrentLevel;
        if (roleId === 3) { // master
            typeUrl = 'MasterMinMaxPT';
        }
        else if (roleId === 2) { // agent
            typeUrl = 'AgentMinMaxPT';
        }

        if (typeUrl !== '') {
            var url = resolveClientUrl('CustomerSetting/Racebook/RacingSetting/' + typeUrl + '?id=' + custId + '&roleId=' + roleId + '&groupName=' + $('#selectedGroupName').html());
            $('#tabs-3').height($('#leftFrame').height() - 10);
            $('#tabs-3').css('display', 'block');
            $('#minMaxracingFrame').attr('src', url);
        }
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

        var url = resolveClientUrl('CustomerSetting/Casino/' + action + '/' + this.UserId + '/' + Fanex.ExtAcc.Customer.superId + '/' + $('#selectedGroupName').html());

        $('#tabs-4').height($('#leftFrame').height() - 10);
        $('#tabs-4').css('display', 'block');
        $('#casinoFrame').attr('src', url);
    },

    GetBingoSetting: function () {
        Fanex.ExtAcc.Customer.IsMultiple = false;
        Fanex.ExtAcc.Customer.UserId = this.UserId;
        var action = '';
        var uplineId = 0;
        if (this.CurrentLevel == Fanex.ExtAcc.Customer.Master) {
            action = 'Master';
            uplineId = Fanex.ExtAcc.Customer.superId;
        }
        else if (this.CurrentLevel == Fanex.ExtAcc.Customer.Agent) {
            action = 'Agent';
            uplineId = Fanex.ExtAcc.Customer.masterId;
        }
        else {
            action = 'Member';
            uplineId = Fanex.ExtAcc.Customer.agentId;
        }
        var url = resolveClientUrl('CustomerSetting/Bingo/BingoSetting/' + action + '?id=' + this.UserId + '&uplineId=' + uplineId + '&groupName=' + $('#selectedGroupName').html());
        $('#tabs-5').height($('#leftFrame').height() - 10);
        $('#tabs-5').css('display', 'block');
        $('#bingoFrame').attr('src', url);
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
        $('#kenoFrame').attr('src', url);
    },

    GetLiveCasinoSetting: function () {
        Fanex.ExtAcc.Customer.IsMultiple = false;
        Fanex.ExtAcc.Customer.UserId = this.UserId;
        var action = '';

        if (this.CurrentLevel == Fanex.ExtAcc.Customer.Master) {
            action = "LiveCasinoSetting/Setting/EditSetting?uplineId=" + Fanex.ExtAcc.Customer.superId + "&parentId=" + Fanex.ExtAcc.Customer.superId + "&uplineLevel=4&customerId=" + this.UserId + "&customerLevel=3";
        }
        else if (this.CurrentLevel == Fanex.ExtAcc.Customer.Agent) {
            action = "LiveCasinoSetting/Setting/EditSetting?uplineId=" + Fanex.ExtAcc.Customer.masterId + "&parentId=" + Fanex.ExtAcc.Customer.masterId + "&uplineLevel=3&customerId=" + this.UserId + "&customerLevel=2";
        }
        else {
            action = "LiveCasinoSetting/Setting/EditSetting?uplineId=" + Fanex.ExtAcc.Customer.agentId + "&parentId=" + Fanex.ExtAcc.Customer.agentId + "&uplineLevel=2&customerId=" + this.UserId + "&customerLevel=1";
        }

        var url = resolveClientUrl(action + "&actorId=" + parent.userId + "&actorUsername=" + parent.username + '&groupName=' + $('#selectedGroupName').html());

        $('#tabs-7').height($('#leftFrame').height() - 10);
        $('#tabs-7').css('display', 'block');
        $('#livecasinoFrame').attr('src', url);
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


        var url = resolveClientUrl(url  + "&actorId=" + parent.userId + "&custId=" + this.UserId + "&groupName=" + $('#selectedGroupName').html());
        $('#tabs-10').height($('#leftFrame').height() - 10);
        $('#tabs-10').css('display', 'block');
        $('#goldDeluxeFrame').attr('src', url);
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
        $('#colossusBetsFrame').attr('src', url);
    },

    GetSportsbook2Setting: function () {
        Fanex.ExtAcc.Customer.IsMultiple = false;
        Fanex.ExtAcc.Customer.UserId = this.UserId;
        var action = '';
        var url = "DCS/ViewCustomerSetting?language=en-US&productId=16&customerId=" + this.UserId + "&groupName=" + $('#selectedGroupName').html() + "&customerLevel=";

        if (this.CurrentLevel == Fanex.ExtAcc.Customer.Master) {
            url += 3;
        }
        else if (this.CurrentLevel == Fanex.ExtAcc.Customer.Agent) {
            url += 2;
        }
        else {
            url += 1 + "&UplineLevel=2";
        }

        $('#tabs-12').height($('#leftFrame').height() - 10);
        $('#tabs-12').css('display', 'block');
        $('#sportsbook2Frame').attr('src', resolveClientUrl(url));
    },

    GetVirtualSportSetting: function () {
        Fanex.ExtAcc.Customer.IsMultiple = false;
        Fanex.ExtAcc.BlockUI();
        Fanex.ExtAcc.Customer.UserId = this.UserId;
        var uplineId, uplineRoleId, targetCustId, targetRoleId;
        targetRoleId = this.CurrentLevel;
        if (targetRoleId == Fanex.ExtAcc.Customer.Master) {
            uplineId = Fanex.ExtAcc.Customer.superId;
            uplineRoleId = Fanex.ExtAcc.Customer.Super;
        }
        else if (targetRoleId == Fanex.ExtAcc.Customer.Agent) {
            uplineId = Fanex.ExtAcc.Customer.masterId;
            uplineRoleId = Fanex.ExtAcc.Customer.Master;
        }
        else {
            uplineId = Fanex.ExtAcc.Customer.agentId;
            uplineRoleId = Fanex.ExtAcc.Customer.Agent;
        }

        targetCustId = this.UserId;
        if (uplineId > 0 && uplineRoleId > 0 && targetCustId > 0 && targetRoleId > 0) {
            $.ajax({
                type: 'POST',
                url: resolveClientUrl('VirtualSports/EditSingle?custId=' + uplineId + '&roleId=' + uplineRoleId + '&targetCustId=' + targetCustId + '&targetRoleId=' + targetRoleId + '&groupName=' + $('#selectedGroupName').html())
            }).success(function (result) {
                var container = $('#tabs-8');
                if (result.errorCode && result.errorCode == "1") {
                    container.css('display', 'block');
                    container.html(result.errorMessage.formatErrorMessage());
                    return;
                }

                container.css('display', 'block');
                container.html(result);

            }).error(function (data) {
                if (typeof console != 'undefined') {
                    console.log(data.responseText);
                }
            }).complete(function (jqXHR, textStatus) {
                Fanex.ExtAcc.UnBlockUI();
            });
        }
    },

    EmptyAllTabs: function () {
        $('#tabs-1').html('');
        $('#tabs-6').html('');
//        $('#tabs-7').html('');
        $('#tabs-8').html('');

        $('#tabs-1').css('display', 'none');
        $('#tabs-6').css('display', 'none');
//        $('#tabs-7').css('display', 'none');
        $('#tabs-8').css('display', 'none');
    }
};

$(document).ready(function () {
    Fanex.ExtAcc.Customer.Settings.Init();
});