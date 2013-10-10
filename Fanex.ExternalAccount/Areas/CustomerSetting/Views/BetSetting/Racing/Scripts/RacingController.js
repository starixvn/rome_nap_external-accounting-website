if (typeof Fanex == 'undefined' || typeof Fanex.CustomerSetting == 'undefined' || typeof Fanex.CustomerSetting.Core == 'undefined') {
    alert('Require Fanex.CustomerSetting.Core.js');
};

Fanex.CustomerSetting.Betting = {};

Fanex.CustomerSetting.Betting.Racing = function ($scope, $element) {
    $scope.model = model;

    $scope.bettingLimits = $scope.model.settings.sportTypes;

    $scope.Init = function () {
        $scope.bettingLimits[0].betTypeGroups[0].title = 'Copy all below bet settings from Horse Racing UK/Irish Tote';
    };

    $scope.Copy = function (scope) {
        if (scope.betting.id == 151 && scope.bet.id == 41) {
            if (scope.bet.className == true && scope.bet.bgClass == '') {
                scope.bet.className = 'copied';

                var timeout = setTimeout(function () {
                    clearTimeout(timeout);
                    $('#_' + scope.betting.id + '_' + scope.bet.id).addClass('pointer');
                }, 50);

                // Change background color and set new value
                var minBet = Fanex.CustomerSetting.Core.ReplaceAll(scope.bet.minBetFormated, ',', '');
                var maxBet = Fanex.CustomerSetting.Core.ReplaceAll(scope.bet.maxBetFormated, ',', '');
                var maxPerMatch = Fanex.CustomerSetting.Core.ReplaceAll(scope.bet.maxPerMatchFormated, ',', '');

                for (var i = 0; i < scope.bettingLimits.length; i++) {
                    var betTypes = scope.bettingLimits[i].betTypeGroups;

                    for (var j = 0; j < betTypes.length; j++) {

                        if (j != 0 || i !== 0) {
                            betTypes[j].bgClass = 'inactive';
                        }
                        else if (i == 0 && j == 0) {
                            betTypes[j].bgClass = '';
                        }

                        betTypes[j].minBetFormated = parseFloat(minBet == '' ? 0 : minBet).toString().formatNumber();
                        betTypes[j].maxBetFormated = parseFloat(maxBet == '' ? 0 : maxBet).toString().formatNumber();
                        betTypes[j].maxPerMatchFormated = parseFloat(maxPerMatch == '' ? 0 : maxPerMatch).toString().formatNumber()

                        betTypes[j].bettingCommSetting.minBet = parseFloat(minBet == '' ? 0 : minBet).toString().formatNumber();
                        betTypes[j].bettingCommSetting.maxBet = parseFloat(maxBet == '' ? 0 : maxBet).toString().formatNumber();
                        betTypes[j].bettingCommSetting.maxPerMatch = parseFloat(maxPerMatch == '' ? 0 : maxPerMatch).toString().formatNumber();
                    }
                }
            }
            else if (scope.bet.className == 'copied') {
                scope.bet.bgClass = '';
                scope.bet.className = true;

                for (var i = 0; i < scope.bettingLimits.length; i++) {
                    var betTypes = scope.bettingLimits[i].betTypeGroups;

                    for (var j = 0; j < betTypes.length; j++) {
                        betTypes[j].bgClass = '';

                        if (i == 0 && j == 0) {
                            continue;
                        }

                        var minBet = betTypes[j].preValMinBet.replaceAll(',', '').formatNumber();
                        var maxBet = betTypes[j].preValMaxBet.replaceAll(',', '').formatNumber();
                        var maxPerMatch = betTypes[j].preValMaxPerMatch.replaceAll(',', '').formatNumber();

                        betTypes[j].minBetFormated = minBet;
                        betTypes[j].maxBetFormated = maxBet;
                        betTypes[j].maxPerMatchFormated = maxPerMatch;

                        betTypes[j].bettingCommSetting.minBet = minBet;
                        betTypes[j].bettingCommSetting.maxBet = maxBet;
                        betTypes[j].bettingCommSetting.maxPerMatch = maxPerMatch;
                    }
                }
            }
        }
    };

    $scope.$watch('bettingLimits[0].betTypeGroups[0].minBetFormated', function (newValue, oldValue, _model) {
        var item = _model.bettingLimits[0];
        if ($('#_' + item.id + '_' + item.betTypeGroups[0].id).hasClass('copied')) {
            var minBet = Fanex.CustomerSetting.Core.ReplaceAll(item.betTypeGroups[0].minBetFormated, ',', '');

            for (var i = 0; i < _model.bettingLimits.length; i++) {
                var betTypes = _model.bettingLimits[i].betTypeGroups;

                for (var j = 0; j < betTypes.length; j++) {

                    if (i == 0 && j == 0) {
                        continue;
                    }
                    else {
                        betTypes[j].bgClass = 'inactive';

                        betTypes[j].minBetFormated = parseFloat(minBet == '' ? 0 : minBet).formatNumber();

                        betTypes[j].bettingCommSetting.minBet = parseFloat(minBet == '' ? 0 : minBet).formatNumber();
                    }
                }
            }
        }
    });

    $scope.$watch('bettingLimits[0].betTypeGroups[0].maxBetFormated', function (newValue, oldValue, _model) {
        var item = _model.bettingLimits[0];
        if ($('#_' + item.id + '_' + item.betTypeGroups[0].id).hasClass('copied')) {
            var maxBet = Fanex.CustomerSetting.Core.ReplaceAll(item.betTypeGroups[0].maxBetFormated, ',', '');

            for (var i = 0; i < _model.bettingLimits.length; i++) {
                var betTypes = _model.bettingLimits[i].betTypeGroups;

                for (var j = 0; j < betTypes.length; j++) {

                    if (i == 0 && j == 0) {
                        continue;
                    }
                    else {
                        betTypes[j].bgClass = 'inactive';

                        betTypes[j].maxBetFormated = parseFloat(maxBet == '' ? 0 : maxBet).formatNumber();

                        betTypes[j].bettingCommSetting.maxBet = parseFloat(maxBet == '' ? 0 : maxBet).formatNumber();
                    }
                }
            }
        }
    });

    $scope.$watch('bettingLimits[0].betTypeGroups[0].maxPerMatchFormated', function (newValue, oldValue, _model) {
        var item = _model.bettingLimits[0];
        if ($('#_' + item.id + '_' + item.betTypeGroups[0].id).hasClass('copied')) {
            var maxPerMatch = Fanex.CustomerSetting.Core.ReplaceAll(item.betTypeGroups[0].maxPerMatchFormated, ',', '');

            for (var i = 0; i < _model.bettingLimits.length; i++) {
                var betTypes = _model.bettingLimits[i].betTypeGroups;

                for (var j = 0; j < betTypes.length; j++) {

                    if (i == 0 && j == 0) {
                        continue;
                    }
                    else {
                        betTypes[j].bgClass = 'inactive';

                        betTypes[j].maxPerMatchFormated = parseFloat(maxPerMatch == '' ? 0 : maxPerMatch).formatNumber();

                        betTypes[j].bettingCommSetting.maxPerMatch = parseFloat(maxPerMatch == '' ? 0 : maxPerMatch).formatNumber();
                    }
                }
            }
        }
    });
};

Fanex.CustomerSetting.Betting.Racing.Update = function (ondone) {
    var bettings = model.settings.sportTypes;
    var length = bettings.length;

    this.IsValid = function (min, max, maxPerMatch, minMin, bet, betting) {
        if (isNaN(min)) {
            Validators.show($('#txt' + bet.id + betting.id + 'Min').get(0), bet.name + ' ' + betting.name + ' - Min Bet should be a value', undefined, undefined, 3000);

            return false;
        }

        if (isNaN(max)) {
            Validators.show($('#txt' + bet.id + betting.id + 'Max').get(0), bet.name + ' ' + betting.name + ' - Max Bet should be a value', undefined, undefined, 3000);

            return false;
        }

        if (isNaN(maxPerMatch)) {
            Validators.show($('#txt' + bet.id + betting.id + 'MaxPerMatch').get(0), bet.name + ' ' + betting.name + ' - Max Per Match should be a value', undefined, undefined, 3000);

            return false;
        }

        if (minMin != 0 && min < minMin) {
            Validators.show($('#txt' + bet.id + betting.id + 'Min').get(0), bet.name + ' ' + betting.name + ' - Min Bet should be greater than or equals to ' + minMin.formatNumber(), undefined, undefined, 3000);

            return false;
        }

        if (min > max) {
            Validators.show($('#txt' + bet.id + betting.id + 'Min').get(0), bet.name + ' ' + betting.name + ' - Min Bet should be less than or equals to Max Bet', undefined, undefined, 3000);

            return false;
        }

        if (min > maxPerMatch) {
            Validators.show($('#txt' + bet.id + betting.id + 'Min').get(0), bet.name + ' ' + betting.name + ' - Min Bet should be less than or equals to Max Per Match', undefined, undefined, 3000);

            return false;
        }

        if (max > maxPerMatch) {
            Validators.show($('#txt' + bet.id + betting.id + 'Max').get(0), bet.name + ' ' + betting.name + ' - Max Bet should be less than or equals to Max Per Match', undefined, undefined, 3000);

            return false;
        }


        return true;
    };

    for (var i = 0; i < length; i++) {
        var bets = bettings[i];

        for (var j = 0; j < bets.betTypeGroups.length; j++) {
            var min = parseFloat(Fanex.CustomerSetting.Core.ReplaceAll(bets.betTypeGroups[j].minBetFormated, ',', ''));
            var max = parseFloat(Fanex.CustomerSetting.Core.ReplaceAll(bets.betTypeGroups[j].maxBetFormated, ',', ''));
            var maxPerMatch = parseFloat(Fanex.CustomerSetting.Core.ReplaceAll(bets.betTypeGroups[j].maxPerMatchFormated, ',', ''));
            var minMin = bets.betTypeGroups[j].bettingCommSetting.minBetMin;


            if (!this.IsValid(min, max, maxPerMatch, minMin, bets, bets.betTypeGroups[j])) {
                return;
            }
        }
    }

    Fanex.CustomerSetting.Core.BlockUI();

    $.ajax({
        dataType: 'json',
        type: 'post',
        url: toUrl('RacingBetting/EditBetSettingPost'),
        data: {
            id: custId,
            uplineId: uplineId,
            userId: userId,
            txtCustomerObject: JSON.stringify(model),
            siteName: siteName
        }
    })
    .success(function (data) {
        var errorCodes = data.errorCode.split('\u0007');
        var success = true;

        $(errorCodes).each(function (i, error) {
            if (error != '' && parseInt(error) >= 0) {
                success = false;
            }
        });

        if (data.errorCode == "0" || data.errorCode == "" || success) {
            $('#errMsg').attr('class', 'successMsg');
            $('#errMsg').html('<font color="Blue">Updated successfully</font>');
            $('#errMsg').show();
        }
        else {
            $('#errMsg').attr('class', 'errMsg');
            $('#errMsg').html(data.errorMessage.formatErrorMessage());
            $('#errMsg').show();
        }

        Fanex.CustomerSetting.Core.UnBlockUI();

        if (typeof ondone == 'function') {
            ondone({
                errorCode: data.errorCode == "0" || data.errorCode == "" || success ? "0" : "1",
                errorMessage: data.errorMessage
            });
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