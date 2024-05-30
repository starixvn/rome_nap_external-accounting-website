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
    document.location.href = toUrl('Sportsbook/Index?theme=nap&custId=' + custId + '&uplineId=' + uplineId + "&roleId=4" + '&userId=' + userId + '&groupName=' + siteName);
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

function LoadProductGroup(productGroupId) {
    Fanex.CustomerSetting.Core.BlockUI();
    var url = parent.resolveClientUrl("Customer/EditSuperBetSetting?productGroupId=" + productGroupId + "&custId=" + custId + "&roleId=4&uplineRoleId=5&groupName=" + parent.$('#selectedGroupName').html());
    document.getElementById("product-group-" + productGroupId).src = url;
    setTimeout(Fanex.CustomerSetting.Core.UnBlockUI, 1000);
};

function CopySB() {
    $("#errMsg").empty().hide();
    $("#sourceSuper").focus();
};

function EditSB() {
    $("#errMsg").empty().hide();
    $("#superList2").focus();
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
            if (isCopy) {
                $("#selectButton").addClass("-disabled").attr("disabled", true);
                $("#sourceSuper").addClass("-disabled").attr("disabled", true);
                $("#superList").addClass("-disabled").attr("disabled", true);
                $("#resetButton").removeClass("-disabled").attr("disabled", false);
            }
            else {
                $("#selectButton2").addClass("-disabled").attr("disabled", true);
                $("#superList2").addClass("-disabled").attr("disabled", true);
                $("#resetButton2").removeClass("-disabled").attr("disabled", false);
            }

            if (isCopy) {
                var superIds = result.superIds.split(',');
                var sourceId = superIds[0];
                superIds.shift();
                $("#product-1-0").attr("src", resolveClientUrl("Customer/EditMultipleSuperBetSetting?isCopy=true&custId=" + sourceId + "&custIds=" + superIds.toString() + "&usernames=" + destUsers  + "&uplineRoleId=5&roleId=4&productGroupId=1&groupName=" + groupName));
            }
            else {
                $("#product-1-1").attr("src", resolveClientUrl("Customer/EditMultipleSuperBetSetting?custIds=" + result.superIds + '&usernames=' + destUsers + '&uplineRoleId=5&roleId=4&productGroupId=1&groupName=' + groupName));
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