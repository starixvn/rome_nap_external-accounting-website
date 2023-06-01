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

function CopySB() {
    top.Fanex.ExtAcc.BlockUI();
    var url = toUrl('Sportsbook/EditMultipleBetSetting?copy=1&custId=' + custId + '&groupName=' + siteName);
    $.get(url, function (data) {
        $('#contentSplitter').empty().append(data);

        setTimeout(function () {
            top.Fanex.ExtAcc.UnBlockUI();
            $('#tab2').removeClass('ui-tabs-active').removeClass('ui-state-active');
            $('#tabs').tabs({ selected: 0 });
        }, 200);
    });
};

function EditSB() {
    top.Fanex.ExtAcc.BlockUI();
    var url = toUrl('Sportsbook/EditMultipleBetSetting?edit=1&custId=0&groupName=' + siteName);
    $.get(url, function (data) {
        $('#contentSplitter').empty().append(data);

        setTimeout(function () {
            top.Fanex.ExtAcc.UnBlockUI();
            $('#tab1').removeClass('ui-tabs-active').removeClass('ui-state-active');
            $('#tabs').tabs({ selected: 1 });
        }, 200);
    });
};

function LoadBetSetting(sourceUser, destUsers, isCopy) {
    $("#errMsg").empty().hide();
    if (isCopy && $.trim(sourceUser) === "") {
        $("#errMsg").empty().html("Please input source super").show();
        return;
    }

    if (isCopy && sourceUser.indexOf(",") !== -1) {
        $("#errMsg").empty().html("Please input a single source super").show();
        return;
    }

    if ($.trim(destUsers) === "") {
        $("#errMsg").empty().html("Please input destination super").show();
        return;
    }

    var groupName = Fanex.ExtAcc.Customer.GroupName().groupName;
    var url = toUrl("Sportsbook/CheckMultipleSupersBetSetting?sourceUser=" + sourceUser + "&destUsers=" + destUsers + "&isCopy=" + isCopy + "&groupName=" + groupName);

    top.Fanex.ExtAcc.BlockUI();
    $.post(url, function (result) {
        if (result.errorCode !== '0') {
            $("#errMsg").empty().html(result.errorMessage).show();
        }
        else {
            top.superIds = result.superIds;
            $("#selectButton").addClass("-disabled").attr("disabled", true);
            $("#sourceSuper").addClass("-disabled").attr("disabled", true);
            $("#superList").addClass("-disabled").attr("disabled", true);

            if (isCopy) {
                $("#product-1-0").attr("src", toUrl("Sportsbook/EditMultipleBetSettingContent?copy=1&custId=" + result.superIds.split(',')[0] + "&groupName=" + groupName));
            }
            else {
                $("#product-1-1").attr("src", toUrl("Sportsbook/EditMultipleBetSettingContent?edit=1&custId=0&groupName=" + groupName));
            }
        }

        top.Fanex.ExtAcc.UnBlockUI();
    });
}

function ReplaceChars(textarea) {
    textarea.value = textarea.value
        .replace(/\s\s/g, ' ')
        .replace(/\s/g, ',')
        .replace(/\t/g, ',')
        .replace(/;/g, ',')
        .replace(/\\n|\\r\\n|\\r/g, ',')
        .replaceAll(',,', ',');

    if (textarea.value.startsWith(',')) {
        textarea.value = textarea.value.substr(1);
    }

    if (textarea.value.endsWith(',')) {
        textarea.value = textarea.value.substr(0, textarea.value.length - 1);
    }
}