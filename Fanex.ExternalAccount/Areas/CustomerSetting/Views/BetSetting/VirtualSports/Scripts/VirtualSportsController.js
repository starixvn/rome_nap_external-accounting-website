if (typeof Fanex == 'undefined' || typeof Fanex.CustomerSetting == 'undefined' || typeof Fanex.CustomerSetting.Core == 'undefined') {
    alert('Require Fanex.CustomerSetting.Core.js');
};

Fanex.CustomerSetting.Betting = {};

Fanex.CustomerSetting.Betting.VirtualSports = function ($scope, $element) {
    $scope.model = model;

    if (!model)
        return;

    $scope.bettingLimits = $scope.model.bettingLimit.bettingLimitItems;
};

Fanex.CustomerSetting.Betting.VirtualSports.Update = function (ondone) {
    var bettings = model.bettingLimit.bettingLimitItems;
    var length = bettings.length;

    this.IsValid = function (min, max, maxPerMatch, minMin, maxMax, maxPerMatchMax, betting) {
        if (isNaN(min)) {
            Validators.show($('#txt' + betting.id + 'Min').get(0), betting.name + ' - Min Bet should be a value', undefined, undefined, 3000);

            return false;
        }

        if (isNaN(max)) {
            Validators.show($('#txt' + betting.id + 'Max').get(0), betting.name + ' - Max Bet should be a value', undefined, undefined, 3000);

            return false;
        }

        if (isNaN(maxPerMatch)) {
            Validators.show($('#txt' + betting.id + 'MaxPerMatch').get(0), betting.name + ' - Max Per Match should be a value', undefined, undefined, 3000);

            return false;
        }

        if (minMin != 0 && min < minMin) {
            Validators.show($('#txt' + betting.id + 'Min').get(0), betting.name + ' - Min Bet should be greater than or equals to ' + minMin.formatNumber(), undefined, undefined, 3000);

            return false;
        }

        if (min > max) {
            Validators.show($('#txt' + betting.id + 'Min').get(0), betting.name + ' - Min Bet should be less than or equals to Max Bet', undefined, undefined, 3000);

            return false;
        }

        if (min > maxPerMatch) {
            Validators.show($('#txt' + betting.id + 'Min').get(0), betting.name + ' - Min Bet should be less than or equals to Max Per Match', undefined, undefined, 3000);

            return false;
        }

        if (maxMax != 0 && max > maxMax) {
            Validators.show($('#txt' + betting.id + 'Max').get(0), betting.name + ' - Max Bet should be less than or equals to ' + maxMax.formatNumber(), undefined, undefined, 3000);

            return false;
        }

        if (max > maxPerMatch) {
            Validators.show($('#txt' + betting.id + 'Max').get(0), betting.name + ' - Max Bet should be less than or equals to Max Per Match', undefined, undefined, 3000);

            return false;
        }

        if (maxPerMatchMax != 0 && maxPerMatch > maxPerMatchMax) {
            Validators.show($('#txt' + betting.id + 'MaxPerMatch').get(0), betting.name + ' - Max Per Match should be less than or equals to ' + maxPerMatchMax.formatNumber(), undefined, undefined, 3000);

            return false;
        }

        return true;
    };

    for (var i = 0; i < length; i++) {
        var bet = bettings[i];

        var min = parseFloat(Fanex.CustomerSetting.Core.ReplaceAll(bet.minBetFormated, ',', ''));
        var max = parseFloat(Fanex.CustomerSetting.Core.ReplaceAll(bet.maxBetFormated, ',', ''));
        var maxPerMatch = parseFloat(Fanex.CustomerSetting.Core.ReplaceAll(bet.maxPerMatchFormated, ',', ''));
        var minMin = bet.minBetMin;
        var maxMax = bet.maxBetMax;
        var maxPerMatchMax = bet.maxPerMatchMax;

        if (!this.IsValid(min, max, maxPerMatch, minMin, maxMax, maxPerMatchMax, bet)) {
            return;
        }
    }

    Fanex.CustomerSetting.Core.BlockUI();

    $.ajax({
        dataType: 'json',
        type: 'post',
        url: toUrl('VirtualSportsBetting/EditBetSettingPost'),
        data: {
            id: custId,
            uplineId: uplineId,
            userId: userId,
            txtCustomerObject: JSON.stringify(model),
            siteName: siteName
        }
    })
    .success(function (data) {
        var errorCodes = data.resultBetSetting;
        var errorMessage = '';
        var success = true;

        $(errorCodes).each(function (i, obj) {
            if (obj.errorCode != '' && parseInt(obj.errorCode) >= 0) {
                success = false;
                errorMessage += model.bettingLimit.bettingLimitItems[i].name + ' - ' + obj.errorMessage + '<br />';
            }
        });

        if (success) {
            $('#errMsg').attr('class', 'successMsg');
            $('#errMsg').html('<font color="Blue">Updated successfully</font>');
            $('#errMsg').show();

            Fanex.CustomerSetting.Core.UnBlockUI();

            if (typeof ondone == 'function') {
                ondone({ errorCode: "0" });
            }
        }
        else {
            $('#errMsg').attr('class', 'errMsg');
            $('#errMsg').html(errorMessage.formatErrorMessage());
            $('#errMsg').show();

            Fanex.CustomerSetting.Core.UnBlockUI();

            if (typeof ondone == 'function') {
                ondone({ errorCode: "1" });
            }
        }
    })
    .error(function (data) {
        if (typeof console != 'undefined')
            console.log(data);

        if (JSON.parse(data.responseText).ErrorCode) {
            $('#errMsg').attr('class', 'errMsg');
            $('#errMsg').html(JSON.parse(data.responseText).ErrorMessage.formatErrorMessage());
            $('#errMsg').show();
        }

        Fanex.CustomerSetting.Core.UnBlockUI();

        if (typeof ondone == 'function') {
            ondone({ errorCode: "1", errorMessage: "Error on updating..." });
        }
    });
};