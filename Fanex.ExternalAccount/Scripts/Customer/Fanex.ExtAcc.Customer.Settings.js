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
        $('#tabs').tabs();

        $('#tab4').css('display', Fanex.ExtAcc.Customer.SyncCasino == 'True' ? 'block' : 'none');
        $('#tab5').css('display', Fanex.ExtAcc.Customer.SyncBingo == 'True' ? 'block' : 'none');
        $('#tab6').css('display', Fanex.ExtAcc.Customer.DisableP2P == 'True' ? 'none' : 'block');
        $('#tab7').css('display', Fanex.ExtAcc.Customer.SyncLiveCasino == 'True' ? 'block' : 'none');


        $('#tabs-4').css('display', Fanex.ExtAcc.Customer.SyncCasino == 'True' ? 'block' : 'none');
        $('#tabs-5').css('display', Fanex.ExtAcc.Customer.SyncBingo == 'True' ? 'block' : 'none');
        $('#tabs-6').css('display', Fanex.ExtAcc.Customer.DisableP2P == 'True' ? 'none' : 'block');
        $('#tabs-7').css('display', Fanex.ExtAcc.Customer.SyncLiveCasino == 'True' ? 'block' : 'none');

        if (Fanex.ExtAcc.Customer.CurrentLevel != Fanex.ExtAcc.Customer.Member) {
            // Loads first tab (Sportsbook's Min PT)
            Fanex.ExtAcc.Customer.Settings.GetSportsbookMinPT();
        }
        else {
            $('#tab1').hide();
            $('#tabs-1').hide();
            $('#tabs').tabs({ selected: 1 });

            $('#tab3').hide();
            $('#tabs-3').hide();
        }

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
    },

    GetSportsbookMinPT: function () {
        Fanex.ExtAcc.BlockUI();

        $.ajax({
            url: resolveClientUrl('Sportsbook/EditMinPT')
        }).success(function (result) {
            $('#tabs-1').css('display', 'block');
            $('#tabs-1').html(result);
        }).error(function (data) {
            if (typeof console != 'undefined') {
                console.log(data.responseText);
            }
        });
    },

    GetRacingPT: function () {
    },

    GetRacingMinPT: function () {
    },

    GetCasinoSetting: function () {
        Fanex.ExtAcc.BlockUI();

        $.ajax({
            url: resolveClientUrl('Casino/EditCasino'),
            data: { custId: Fanex.ExtAcc.Customer.UserId, uplineId: Fanex.ExtAcc.Customer.superId }
        }).success(function (result) {
            $('#tabs-4').css('display', 'block');
            $('#tabs-4').html(result);
        }).error(function (data) {
            if (typeof console != 'undefined') {
                console.log(data.responseText);
            }
        });
    },

    GetBingoSetting: function () {
    },

    GetPokerSetting: function () {
    },

    GetLiveCasinoSetting: function () {
        Fanex.ExtAcc.BlockUI();

        var action = '';

        if (Fanex.ExtAcc.Customer.Settings.CurrentLevel == Fanex.ExtAcc.Customer.Master) {
            action = 'EditMaster';
        }
        else if (Fanex.ExtAcc.Customer.Settings.CurrentLevel == Fanex.ExtAcc.Customer.Agent) {
            action = 'EditAgent';
        }
        else {
            action = 'EditMember';
        }

        $.ajax({
            url: resolveClientUrl('LiveCasino/' + action)
        }).success(function (result) {
            $('#tabs-7').css('display', 'block');
            $('#tabs-7').html(result);
        }).error(function (data) {
            if (typeof console != 'undefined') {
                console.log(data.responseText);
            }
        });
    },

    GetVirtualSportSetting: function () {
    },

    EmptyAllTabs: function () {
        $('#tabs-1').html('');
        $('#tabs-2').html('');
        $('#tabs-3').html('');
        $('#tabs-4').html('');
        $('#tabs-5').html('');
        $('#tabs-6').html('');
        $('#tabs-7').html('');
        $('#tabs-8').html('');

        $('#tabs-1').css('display', 'none');
        $('#tabs-2').css('display', 'none');
        $('#tabs-3').css('display', 'none');
        $('#tabs-4').css('display', 'none');
        $('#tabs-5').css('display', 'none');
        $('#tabs-6').css('display', 'none');
        $('#tabs-7').css('display', 'none');
        $('#tabs-8').css('display', 'none');
    }
};

$(document).ready(function () {
    Fanex.ExtAcc.Customer.Settings.Init();
});