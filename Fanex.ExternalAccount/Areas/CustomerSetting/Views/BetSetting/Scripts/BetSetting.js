var module = angular.module("bettingApp", []);

angular.element(document.getElementById('ng-app')).ready(function () {
    $('#tabs').show();
});

function LoadSB() {
    window.selectedTab = 0;
    $('#tab1').addClass('ui-tabs-active ui-state-active');
    Fanex.CustomerSetting.Core.BlockUI();
    document.location.href = toUrl('Sportsbook/Index/' + custId + '/' + uplineId + '/' + userId + '/' + siteName);
};

function LoadRacing() {
    window.selectedTab = 1;
    Fanex.CustomerSetting.Core.BlockUI();
    document.location.href = toUrl('RacingBetting/Index/' + custId + '/' + uplineId + '/' + userId + '/' + siteName);
};

function LoadVirtualSports() {
    window.selectedTab = 2;
    Fanex.CustomerSetting.Core.BlockUI();
    document.location.href = toUrl('VirtualSportsBetting/Index/' + custId + '/' + uplineId + '/' + userId + '/' + siteName);
};

function LoadLiveCasino() {
    window.selectedTab = 3;
    Fanex.CustomerSetting.Core.BlockUI();
    document.location.href = toUrl('LiveCasinoBetting/Index/' + custId + '/' + uplineId + '/' + userId + '/' + siteName);
};

function LoadBingo() {
    window.selectedTab = 4;
    Fanex.CustomerSetting.Core.BlockUI();
    document.location.href = toUrl('BingoBetting/Index/' + custId + '/' + uplineId + '/' + userId + '/' + siteName);
};

function LoadCasino() {
    window.selectedTab = 5;
    Fanex.CustomerSetting.Core.BlockUI();
    document.location.href = toUrl('CasinoBetting/Index/' + custId + '/' + uplineId + '/' + userId + '/' + siteName);
};