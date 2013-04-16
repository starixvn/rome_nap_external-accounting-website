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
            $('#tabs').tabs({ selected: 1 });
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
            Fanex.ExtAcc.Customer.Settings.GetLiveCasinoSetting();
        });

        $('#tab7').bind('click', function () {
            Fanex.ExtAcc.Customer.Settings.EmptyAllTabs();
            Fanex.ExtAcc.Customer.Settings.GetVirtualSportSetting();
        });
    },

    GetSportsbookMinPT: function () {
        $.ajax({
            url: resolveClientUrl('Sportsbook/EditMinPT')
        }).success(function (result) {
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
    },

    GetBingoSetting: function () {
    },

    GetLiveCasinoSetting: function () {
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
            $('#tabs-6').html(result);
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
    }
};

$(document).ready(function () {
    Fanex.ExtAcc.Customer.Settings.Init();
});