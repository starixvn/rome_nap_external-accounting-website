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
    document.location.href = toUrl('Sportsbook/Index/' + custId + '/' + uplineId + '/' + userId + '/' + siteName);
};

function LoadRacing() {
    Fanex.CustomerSetting.Core.BlockUI();
    document.location.href = toUrl('RacingBetting/Index/' + custId + '/' + uplineId + '/' + userId + '/' + siteName);
};

function LoadVirtualSports() {
    Fanex.CustomerSetting.Core.BlockUI();
    document.location.href = toUrl('VirtualSportsBetting/Index/' + custId + '/' + uplineId + '/' + userId + '/' + siteName);
};

function LoadLiveCasino() {
    Fanex.CustomerSetting.Core.BlockUI();
    document.location.href = toUrl('LiveCasinoBetting/Index/' + custId + '/' + uplineId + '/' + userId + '/' + siteName);
};

function LoadBingo() {
    Fanex.CustomerSetting.Core.BlockUI();
    document.location.href = toUrl('BingoBetting/Index/' + custId + '/' + uplineId + '/' + userId + '/' + siteName);
};

function LoadCasino() {
    Fanex.CustomerSetting.Core.BlockUI();
    document.location.href = toUrl('CasinoBetting/Index/' + custId + '/' + uplineId + '/' + userId + '/' + siteName);
};

function LoadKeno() {
    Fanex.CustomerSetting.Core.BlockUI();
    document.location.href = toUrl('KenoBetting/Index/' + custId + '/' + uplineId + '/' + userId + '/' + siteName);
};

function LoadGoldDeluxe() {
    Fanex.CustomerSetting.Core.BlockUI();
    document.location.href = toUrl('GoldDeluxeBetting/Index/' + custId + '/' + uplineId + '/' + userId + '/' + siteName);
};

function LoadAllBet() {
    Fanex.CustomerSetting.Core.BlockUI();
    var url = parent.resolveClientUrl("DCS/ViewCustomerSetting?language=en-US&productId=21&customerId=" + custId + "&sssss=v&customerLevel=4&isreadonly=false&groupName=" + parent.$('#selectedGroupName').html());
    document.getElementById('allbet').src = url;
    setTimeout(function () {
        Fanex.CustomerSetting.Core.UnBlockUI();
    }, 1000);
};

function LoadVoidbridge() {
    Fanex.CustomerSetting.Core.BlockUI();
    var url = parent.resolveClientUrl("DCS/ViewCustomerSetting?language=en-US&productId=22&customerId=" + custId + "&sssss=v&customerLevel=4&isreadonly=false&groupName=" + parent.$('#selectedGroupName').html());
    document.getElementById('voidbridge').src = url;
    setTimeout(function () {
        Fanex.CustomerSetting.Core.UnBlockUI();
    }, 1000);
};