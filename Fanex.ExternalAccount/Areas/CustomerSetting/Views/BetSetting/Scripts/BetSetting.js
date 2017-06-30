var module = angular.module("bettingApp", []);

angular.element($('#ng-app')).ready(function () {
    $('#ng-app').show();
    try {
        parent.$('#ng-app').show();
    } catch (e) {
        if (typeof console != 'undefined') {
            console.log(e);
        }
    }    
});

function LoadSB() {
    Fanex.CustomerSetting.Core.BlockUI();
    document.location.href = toUrl('Sportsbook/Index/' + custId + '/' + uplineId + '/' + userId + '/' + siteName + '?v=1');
};

function LoadRacing() {
    Fanex.CustomerSetting.Core.BlockUI();
    document.location.href = toUrl('RacingBetting/Index/' + custId + '/' + uplineId + '/' + userId + '/' + siteName + '?v=0001');
};

function LoadVirtualSports() {
    Fanex.CustomerSetting.Core.BlockUI();
    var url = parent.resolveClientUrl("DCS/ViewCustomerSetting?language=en-US&productId=5&customerId=" + custId + "&sssss=v&customerLevel=4&isreadonly=false&groupName=" + parent.$('#selectedGroupName').html() + '&v=0001');
    document.getElementById('virtualsports').src = url;
    setTimeout(function () {
        Fanex.CustomerSetting.Core.UnBlockUI();
    }, 1000);
};

function LoadLiveCasino() {
    Fanex.CustomerSetting.Core.BlockUI();
    document.location.href = toUrl('LiveCasinoBetting/Index/' + custId + '/' + uplineId + '/' + userId + '/' + siteName + '?v=0001');
};

function LoadCasino() {
    Fanex.CustomerSetting.Core.BlockUI();
    document.location.href = toUrl('CasinoBetting/Index/' + custId + '/' + uplineId + '/' + userId + '/' + siteName + '?v=0001');
};

function LoadKeno() {
    Fanex.CustomerSetting.Core.BlockUI();
    document.location.href = toUrl('KenoBetting/Index/' + custId + '/' + uplineId + '/' + userId + '/' + siteName + '?v=0001');
};

function LoadGoldDeluxe() {
    Fanex.CustomerSetting.Core.BlockUI();
    document.location.href = toUrl('GoldDeluxeBetting/Index/' + custId + '/' + uplineId + '/' + userId + '/' + siteName + '?v=0001');
};

function LoadAllBet() {
    Fanex.CustomerSetting.Core.BlockUI();
    var url = parent.resolveClientUrl("DCS/ViewCustomerSetting?language=en-US&productId=21&customerId=" + custId + "&sssss=v&customerLevel=4&isreadonly=false&groupName=" + parent.$('#selectedGroupName').html() + '&v=0001');
    document.getElementById('allbet').src = url;
    setTimeout(function () {
        Fanex.CustomerSetting.Core.UnBlockUI();
    }, 1000);
};

function LoadVoidbridge() {
    Fanex.CustomerSetting.Core.BlockUI();
    var url = parent.resolveClientUrl("DCS/ViewCustomerSetting?language=en-US&productId=22&customerId=" + custId + "&sssss=v&customerLevel=4&isreadonly=false&groupName=" + parent.$('#selectedGroupName').html() + '&v=0001');
    document.getElementById('voidbridge').src = url;
    setTimeout(function () {
        Fanex.CustomerSetting.Core.UnBlockUI();
    }, 1000);
};

function LoadNG2() {
    Fanex.CustomerSetting.Core.BlockUI();
    var url = parent.resolveClientUrl("DCS/ViewCustomerSetting?language=en-US&productId=23&customerId=" + custId + "&sssss=v&customerLevel=4&isreadonly=false&groupName=" + parent.$('#selectedGroupName').html() + '&v=0001');
    document.getElementById('ng2').src = url;
    setTimeout(function () {
        Fanex.CustomerSetting.Core.UnBlockUI();
    }, 1000);
};

function LoadLottery() {
    Fanex.CustomerSetting.Core.BlockUI();
    var url = parent.resolveClientUrl("DCS/ViewCustomerSetting?language=en-US&productId=28&customerId=" + custId + "&sssss=v&customerLevel=4&isreadonly=false&groupName=" + parent.$('#selectedGroupName').html() + '&v=0001');
    document.getElementById('lottery').src = url;
    setTimeout(function () {
        Fanex.CustomerSetting.Core.UnBlockUI();
    }, 1000);
};