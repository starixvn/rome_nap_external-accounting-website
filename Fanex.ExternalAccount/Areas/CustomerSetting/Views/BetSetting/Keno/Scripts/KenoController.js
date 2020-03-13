if (typeof Fanex == 'undefined' || typeof Fanex.CustomerSetting == 'undefined' || typeof Fanex.CustomerSetting.Core == 'undefined') {
    alert('Require Fanex.CustomerSetting.Core.js');
};

Fanex.CustomerSetting.Betting = {};

Fanex.CustomerSetting.Betting.Keno = function ($scope, $element) {
    $scope.model = model;

    if (!model)
        return;

    $scope.bettingLimits = $scope.model;

    $scope.Init = function () {
        $scope.bettingLimits[0].title = 'Copy all below bet settings from Australia';
    };

    $scope.Copy = function (scope) {
        if (scope.betting.betTypeId == 1501) {
            if (scope.betting.className == true && scope.betting.bgClass == '') {
                scope.betting.className = 'copied';

                var timeout = setTimeout(function () {
                    clearTimeout(timeout);
                    $('#_' + scope.betting.betTypeId).addClass('pointer');
                }, 50);

                // Change background color and set new value
                var minBet = Fanex.CustomerSetting.Core.ReplaceAll(scope.betting.minBetFormated, ',', '');
                var maxBet = Fanex.CustomerSetting.Core.ReplaceAll(scope.betting.maxBetFormated, ',', '');
                var maxPerMatch = Fanex.CustomerSetting.Core.ReplaceAll(scope.betting.maxPerMatchFormated, ',', '');

                for (var i = 0; i < scope.bettingLimits.length; i++) {
                    var betTypes = scope.bettingLimits[i];

                    if (i !== 0) {
                        betTypes.bgClass = 'inactive';
                    }
                    else if (i == 0) {
                        betTypes.bgClass = '';
                    }

                    betTypes.isBetSettingUpdated = true;
                    betTypes.minBetFormated = parseFloat(minBet == '' ? 0 : minBet).toString().formatNumber();
                    betTypes.maxBetFormated = parseFloat(maxBet == '' ? 0 : maxBet).toString().formatNumber();
                    betTypes.maxPerMatchFormated = parseFloat(maxPerMatch == '' ? 0 : maxPerMatch).toString().formatNumber()

                    betTypes.minBet = parseFloat(minBet == '' ? 0 : minBet).toString().formatNumber();
                    betTypes.maxBet = parseFloat(maxBet == '' ? 0 : maxBet).toString().formatNumber();
                    betTypes.maxPerMatch = parseFloat(maxPerMatch == '' ? 0 : maxPerMatch).toString().formatNumber();
                }
            }
            else if (scope.betting.className == 'copied') {
                scope.betting.bgClass = '';
                scope.betting.className = true;

                for (var i = 0; i < scope.bettingLimits.length; i++) {
                    var betTypes = scope.bettingLimits[i];

                    betTypes.bgClass = '';

                    if (i == 0) {
                        continue;
                    }

                    var minBet = betTypes.preValMinBet.replaceAll(',', '').formatNumber();
                    var maxBet = betTypes.preValMaxBet.replaceAll(',', '').formatNumber();
                    var maxPerMatch = betTypes.preValMaxPerMatch.replaceAll(',', '').formatNumber();

                    betTypes.isBetSettingUpdated = true;
                    betTypes.minBetFormated = minBet;
                    betTypes.maxBetFormated = maxBet;
                    betTypes.maxPerMatchFormated = maxPerMatch;

                    betTypes.minBet = minBet;
                    betTypes.maxBet = maxBet;
                    betTypes.maxPerMatch = maxPerMatch;
                }
            }
        }
    };

    $scope.$watch('bettingLimits[0].minBetFormated', function (newValue, oldValue, _model) {
        var item = _model.bettingLimits[0];
        if ($('#_' + item.betTypeId).hasClass('copied')) {
            var minBet = Fanex.CustomerSetting.Core.ReplaceAll(item.minBetFormated, ',', '');

            for (var i = 0; i < _model.bettingLimits.length; i++) {
                var betTypes = _model.bettingLimits[i];
                if (i == 0) {
                    continue;
                }
                else {
                    betTypes.bgClass = 'inactive';

                    betTypes.isBetSettingUpdated = true;
                    betTypes.minBetFormated = parseFloat(minBet == '' ? 0 : minBet).formatNumber();
                    betTypes.minBet = parseFloat(minBet == '' ? 0 : minBet).formatNumber();
                }
            }
        }
    });

    $scope.$watch('bettingLimits[0].maxBetFormated', function (newValue, oldValue, _model) {
        var item = _model.bettingLimits[0];
        if ($('#_' + item.betTypeId).hasClass('copied')) {
            var maxBet = Fanex.CustomerSetting.Core.ReplaceAll(item.maxBetFormated, ',', '');

            for (var i = 0; i < _model.bettingLimits.length; i++) {
                var betTypes = _model.bettingLimits[i];

                if (i == 0) {
                    continue;
                }
                else {
                    betTypes.bgClass = 'inactive';

                    betTypes.isBetSettingUpdated = true;
                    betTypes.maxBetFormated = parseFloat(maxBet == '' ? 0 : maxBet).formatNumber();
                    betTypes.maxBet = parseFloat(maxBet == '' ? 0 : maxBet).formatNumber();
                }
            }
        }
    });

    $scope.$watch('bettingLimits[0].maxPerMatchFormated', function (newValue, oldValue, _model) {
        var item = _model.bettingLimits[0];
        if ($('#_' + item.betTypeId).hasClass('copied')) {
            var maxPerMatch = Fanex.CustomerSetting.Core.ReplaceAll(item.maxPerMatchFormated, ',', '');

            for (var i = 0; i < _model.bettingLimits.length; i++) {
                var betTypes = _model.bettingLimits[i];

                if (i == 0) {
                    continue;
                }
                else {
                    betTypes.bgClass = 'inactive';

                    betTypes.isBetSettingUpdated = true;
                    betTypes.maxPerMatchFormated = parseFloat(maxPerMatch == '' ? 0 : maxPerMatch).formatNumber();
                    betTypes.maxPerMatch = parseFloat(maxPerMatch == '' ? 0 : maxPerMatch).formatNumber();
                }
            }
        }
    });
};

Fanex.CustomerSetting.Betting.Keno.Update = function (ondone) {
    var bettings = model;
    var length = bettings.length;

    this.IsValid = function (min, max, maxPerMatch, minMin, maxMax, maxPerMatchMax, betting) {
        if (isNaN(min)) {
            Validators.show($('#txt' + betting.betTypeId + 'Min').get(0), betting.betTypeName + ' - Min Bet should be a value', undefined, undefined, 3000);

            return false;
        }

        if (isNaN(max)) {
            Validators.show($('#txt' + betting.betTypeId + 'Max').get(0), betting.betTypeName + ' - Max Bet should be a value', undefined, undefined, 3000);

            return false;
        }

        if (isNaN(maxPerMatch)) {
            Validators.show($('#txt' + betting.betTypeId + 'MaxPerMatch').get(0), betting.betTypeName + ' - Max Per Match should be a value', undefined, undefined, 3000);

            return false;
        }

        if (minMin != 0 && min < minMin) {
            Validators.show($('#txt' + betting.betTypeId + 'Min').get(0), betting.betTypeName + ' - Min Bet should be greater than or equals to ' + minMin.formatNumber(), undefined, undefined, 3000);

            return false;
        }

        if (min > max) {
            Validators.show($('#txt' + betting.betTypeId + 'Min').get(0), betting.betTypeName + ' - Min Bet should be less than or equals to Max Bet', undefined, undefined, 3000);

            return false;
        }

        if (min > maxPerMatch) {
            Validators.show($('#txt' + betting.betTypeId + 'Min').get(0), betting.betTypeName + ' - Min Bet should be less than or equals to Max Per Match', undefined, undefined, 3000);

            return false;
        }

        if (maxMax != 0 && max > maxMax) {
            Validators.show($('#txt' + betting.betTypeId + 'Max').get(0), betting.betTypeName + ' - Max Bet should be less than or equals to ' + maxMax.formatNumber(), undefined, undefined, 3000);

            return false;
        }

        if (max > maxPerMatch) {
            Validators.show($('#txt' + betting.betTypeId + 'Max').get(0), betting.betTypeName + ' - Max Bet should be less than or equals to Max Per Match', undefined, undefined, 3000);

            return false;
        }

        if (maxPerMatchMax != 0 && maxPerMatch > maxPerMatchMax) {
            Validators.show($('#txt' + betting.betTypeId + 'MaxPerMatch').get(0), betting.betTypeName + ' - Max Per Match should be less than or equals to ' + maxPerMatchMax.formatNumber(), undefined, undefined, 3000);

            return false;
        }

        return true;
    };

    for (var i = 0; i < length; i++) {
        var bet = bettings[i];

        var min = parseFloat(Fanex.CustomerSetting.Core.ReplaceAll(bet.minBetFormated, ',', ''));
        var max = parseFloat(Fanex.CustomerSetting.Core.ReplaceAll(bet.maxBetFormated, ',', ''));
        var maxPerMatch = parseFloat(Fanex.CustomerSetting.Core.ReplaceAll(bet.maxPerMatchFormated, ',', ''));
        var minMin = bet.upline.minBet || defaultMinBetValue;
        var maxMax = bet.upline.maxBet;
        var maxPerMatchMax = bet.upline.maxPerMatch;

        if (!this.IsValid(min, max, maxPerMatch, minMin, maxMax, maxPerMatchMax, bet)) {
            return;
        }
    }

    Fanex.CustomerSetting.Core.BlockUI();

    $.ajax({
        dataType: 'json',
        type: 'post',
        url: toUrl('KenoBetting/EditBetSettingPost'),
        data: {
            custId: custId,
            uplineId: uplineId,
            userId: userId,
            txtCustomerObject: JSON.stringify(model)            
        }
    })
    .success(function (data) {
        var errorCodes = data;
        var errorMessage = '';
        var success = true;

        $(errorCodes).each(function (i, obj) {
            if (obj.errorCode) {
                success = obj.isSuccess;
                errorMessage += obj.errorMessage + '<br />';
            }
            else if (obj[0].errorCode != '' && parseInt(obj[0].errorCode) >= 0) {
                success = false;
                errorMessage += obj[0].errorMessage + '<br />';
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

        if (JSON.parse(data.responseText).errorCode) {
            $('#errMsg').attr('class', 'errMsg');
            $('#errMsg').html(JSON.parse(data.responseText).errorMessage.formatErrorMessage());
            $('#errMsg').show();
        }

        Fanex.CustomerSetting.Core.UnBlockUI();

        if (typeof ondone == 'function') {
            ondone({ errorCode: "1", errorMessage: "Error on updating..." });
        }
    });
};