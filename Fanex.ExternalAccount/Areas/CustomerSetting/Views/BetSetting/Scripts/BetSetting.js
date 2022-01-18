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
    document.location.href = toUrl('Sportsbook/Index?custId=' + custId + '&uplineId=' + uplineId + '&userId=' + userId + '&groupName=' + siteName);
};

function LoadVirtualSports() {
    Fanex.CustomerSetting.Core.BlockUI();
    var url = parent.resolveClientUrl("DCS/ViewCustomerSetting?language=en-US&productId=5&customerId=" + custId + "&sssss=v&customerLevel=4&isreadonly=false&groupName=" + parent.$('#selectedGroupName').html() + '&v=0001');
    document.getElementById('product-5').src = url;
    setTimeout(function () {
        Fanex.CustomerSetting.Core.UnBlockUI();
    }, 1000);
};

function LoadCasino() {
    Fanex.CustomerSetting.Core.BlockUI();
    var url = parent.resolveClientUrl("DCS/ViewCustomerSetting?language=en-US&productId=6&customerId=" + custId + "&sssss=v&customerLevel=4&isreadonly=false&groupName=" + parent.$('#selectedGroupName').html() + '&v=0001');
    document.getElementById('product-6').src = url;
    setTimeout(function () {
        Fanex.CustomerSetting.Core.UnBlockUI();
    }, 1000);
};

function LoadKeno() {
    Fanex.CustomerSetting.Core.BlockUI();
    document.location.href = toUrl('KenoBetting/Index?custId=' + custId + '&uplineId=' + uplineId + '&userId=' + userId + '&groupName=' + siteName);
};

function LoadLiveCasino() {
    Fanex.CustomerSetting.Core.BlockUI();
    var url = parent.resolveClientUrl("DCS/ViewCustomerSetting?language=en-US&productId=41&customerId=" + custId + "&sssss=v&customerLevel=4&isreadonly=false&groupName=" + parent.$('#selectedGroupName').html() + '&v=0001');
    document.getElementById('livecasino').src = url;
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

function LoadRNGLottery() {
    Fanex.CustomerSetting.Core.BlockUI();
    var url = parent.resolveClientUrl("DCS/ViewCustomerSetting?language=en-US&productId=44&customerId=" + custId + "&sssss=v&customerLevel=4&isreadonly=false&groupName=" + parent.$('#selectedGroupName').html() + '&v=0001');
    document.getElementById('rnglottery').src = url;
    setTimeout(function () {
        Fanex.CustomerSetting.Core.UnBlockUI();
    }, 1000);
};

function LoadProduct(productId) {
    Fanex.CustomerSetting.Core.BlockUI();
    var url = parent.resolveClientUrl("DCS/ViewCustomerSetting?language=en-US&productId=" + productId + "&customerId=" + custId + "&sssss=v&customerLevel=4&isreadonly=false&groupName=" + parent.$('#selectedGroupName').html() + '&v=0001');
    document.getElementById("product-" + productId).src = url;
    setTimeout(Fanex.CustomerSetting.Core.UnBlockUI, 1000);
};