if (typeof Fanex == 'undefined' || typeof Fanex.CustomerSetting == 'undefined' || typeof Fanex.CustomerSetting.Core == 'undefined') {
    alert('Require Fanex.CustomerSetting.Core.js');
};

Fanex.CustomerSetting.Betting = {};

Fanex.CustomerSetting.Betting.Sportsbook = function ($scope, $element) {
    $scope.model = model;

    if (!model)
        return;

    $scope.bettingLimits = $scope.model.bettingLimitItems;

    var length = $scope.model.bettingLimitItems.length;

    $scope.$watch("model.bettingLimitItems[0].minBetFormated", function (newValue, oldValue, _model) {
        if (_model.bettingLimits[0].checked) {
            for (var i = 1; i < length - 2; i++) {
                _model.bettingLimits[i].minBetFormated = parseFloat(Fanex.CustomerSetting.Core.ReplaceAll((newValue == '' ? '0' : newValue), ',', '')).formatNumber();
                _model.bettingLimits[i].minBet = _model.bettingLimits[i].minBetFormated;
            }
        }
    });

    $scope.$watch("model.bettingLimitItems[0].maxBetFormated", function (newValue, oldValue, _model) {
        if (_model.bettingLimits[0].checked) {
            for (var i = 1; i < length - 2; i++) {
                _model.bettingLimits[i].maxBetFormated = parseFloat(Fanex.CustomerSetting.Core.ReplaceAll((newValue == '' ? '0' : newValue), ',', '')).formatNumber();
                _model.bettingLimits[i].maxBet = _model.bettingLimits[i].maxBetFormated;
            }
        }
    });

    $scope.$watch("model.bettingLimitItems[0].maxPerMatchFormated", function (newValue, oldValue, _model) {
        if (_model.bettingLimits[0].checked) {
            for (var i = 1; i < length - 2; i++) {
                _model.bettingLimits[i].maxPerMatchFormated = parseFloat(Fanex.CustomerSetting.Core.ReplaceAll((newValue == '' ? '0' : newValue), ',', '')).formatNumber();
                _model.bettingLimits[i].maxPerMatch = _model.bettingLimits[i].maxPerMatchFormated;
            }
        }
    });

    $scope.$watch("model.bettingLimitItems[2].minBetFormated", function (newValue, oldValue, _model) {
        if (_model.bettingLimits[2].checked) {
            for (var i = 2; i < length - 2; i++) {
                _model.bettingLimits[i].minBetFormated = parseFloat(Fanex.CustomerSetting.Core.ReplaceAll((newValue == '' ? '0' : newValue), ',', '')).formatNumber();
                _model.bettingLimits[i].minBet = _model.bettingLimits[i].minBetFormated;
            }
        }
    });

    $scope.$watch("model.bettingLimitItems[2].maxBetFormated", function (newValue, oldValue, _model) {
        if (_model.bettingLimits[2].checked) {
            for (var i = 2; i < length - 2; i++) {
                _model.bettingLimits[i].maxBetFormated = parseFloat(Fanex.CustomerSetting.Core.ReplaceAll((newValue == '' ? '0' : newValue), ',', '')).formatNumber();
                _model.bettingLimits[i].maxBet = _model.bettingLimits[i].maxBetFormated;
            }
        }
    });

    $scope.$watch("model.bettingLimitItems[2].maxPerMatchFormated", function (newValue, oldValue, _model) {
        if (_model.bettingLimits[2].checked) {
            for (var i = 2; i < length - 2; i++) {
                _model.bettingLimits[i].maxPerMatchFormated = parseFloat(Fanex.CustomerSetting.Core.ReplaceAll((newValue == '' ? '0' : newValue), ',', '')).formatNumber();
                _model.bettingLimits[i].maxPerMatch = _model.bettingLimits[i].maxPerMatchFormated;
            }
        }
    });

    $scope.$watch("model.bettingLimitItems[10].minBetFormated", function (newValue, oldValue, _model) {
        if (_model.bettingLimits[10].checked) {
            for (var i = 10; i < length ; i++) {
                _model.bettingLimits[i].minBetFormated = parseFloat(Fanex.CustomerSetting.Core.ReplaceAll((newValue == '' ? '0' : newValue), ',', '')).formatNumber();
                _model.bettingLimits[i].minBet = _model.bettingLimits[i].minBetFormated;
            }
        }
    });

    $scope.$watch("model.bettingLimitItems[10].maxBetFormated", function (newValue, oldValue, _model) {
        if (_model.bettingLimits[10].checked) {
            for (var i = 10; i < length; i++) {
                _model.bettingLimits[i].maxBetFormated = parseFloat(Fanex.CustomerSetting.Core.ReplaceAll((newValue == '' ? '0' : newValue), ',', '')).formatNumber();
                _model.bettingLimits[i].maxBet = _model.bettingLimits[i].maxBetFormated;
            }
        }
    });

    $scope.$watch("model.bettingLimitItems[10].maxPerMatchFormated", function (newValue, oldValue, _model) {
        if (_model.bettingLimits[10].checked) {
            for (var i = 10; i < length; i++) {
                _model.bettingLimits[i].maxPerMatchFormated = parseFloat(Fanex.CustomerSetting.Core.ReplaceAll((newValue == '' ? '0' : newValue), ',', '')).formatNumber();
                _model.bettingLimits[i].maxPerMatch = _model.bettingLimits[i].maxPerMatchFormated;
            }
        }
    });

    $scope.Init = function () {
        for (var i = 0; i < length; i++) {
            $scope.model.bettingLimitItems[i].title = $scope.model.bettingLimitItems[i].resourceKey == 'Soccer' ?
            'Copy all below bet settings from Soccer (excluding Number Game)' : $scope.model.bettingLimitItems[i].resourceKey == 'Basketball' ?
            'Copy all below bet settings from Basketball (excluding Number Game)' : $scope.model.bettingLimitItems[i].resourceKey == 'NumberGame' ?
            'Copy all below bet settings from Number Game' : '';
        }
    };

    $scope.Copy = function (scope) {
        switch (scope.betting.resourceKey) {
            case 'Soccer':
                $scope.model.bettingLimitItems[1].checked = false;

                var element = $('#_' + scope.betting.resourceKey);
                var unchecked = element.hasClass('true');
                var soccerBetting = $scope.model.bettingLimitItems[0];

                if (unchecked) {
                    $(element).removeClass('true');
                    $(element).addClass('copied');
                    $(element).addClass('copyActive');

                    $scope.model.bettingLimitItems[0].checked = true;

                    var minBet = parseFloat(Fanex.CustomerSetting.Core.ReplaceAll((soccerBetting.minBetFormated == '' ? '0' : soccerBetting.minBetFormated), ',', ''));
                    var maxBet = parseFloat(Fanex.CustomerSetting.Core.ReplaceAll((soccerBetting.maxBetFormated == '' ? '0' : soccerBetting.maxBetFormated), ',', ''));
                    var maxPerMatch = parseFloat(Fanex.CustomerSetting.Core.ReplaceAll((soccerBetting.maxPerMatchFormated == '' ? '0' : soccerBetting.maxPerMatchFormated), ',', ''));

                    for (var i = 1; i < length - 2; i++) {
                        $scope.model.bettingLimitItems[i].bgClass = 'inactive';

                        $scope.model.bettingLimitItems[i].minBetFormated = minBet.formatNumber();
                        $scope.model.bettingLimitItems[i].maxBetFormated = maxBet.formatNumber();
                        $scope.model.bettingLimitItems[i].maxPerMatchFormated = maxPerMatch.formatNumber();

                        $scope.model.bettingLimitItems[i].minBet = minBet.formatNumber();
                        $scope.model.bettingLimitItems[i].maxBet = maxBet.formatNumber();
                        $scope.model.bettingLimitItems[i].maxPerMatch = maxPerMatch.formatNumber();
                    }
                }
                else {
                    $(element).removeClass('copied');
                    $(element).removeClass('copyActive');
                    $(element).addClass('true');

                    $scope.model.bettingLimitItems[0].checked = false;

                    for (var i = 1; i < length - 2; i++) {
                        $scope.model.bettingLimitItems[i].bgClass = '';

                        var minBet = $scope.model.bettingLimitItems[i].preValMinBet;
                        var maxBet = $scope.model.bettingLimitItems[i].preValMaxBet;
                        var maxPerMatch = $scope.model.bettingLimitItems[i].preValMaxPerMatch;

                        $scope.model.bettingLimitItems[i].minBetFormated = minBet;
                        $scope.model.bettingLimitItems[i].maxBetFormated = maxBet;
                        $scope.model.bettingLimitItems[i].maxPerMatchFormated = maxPerMatch;

                        $scope.model.bettingLimitItems[i].minBet = minBet;
                        $scope.model.bettingLimitItems[i].maxBet = maxBet;
                        $scope.model.bettingLimitItems[i].maxPerMatch = maxPerMatch;
                    }
                }

                var basketBallElement = $('#_Basketball');
                $(basketBallElement).removeClass('copied');
                $(basketBallElement).addClass('true');

                // Prevents model is updating value for class name
                var timeout = setTimeout(function () {
                    clearTimeout(timeout);

                    $(basketBallElement).addClass('pointer');
                }, 50);

                break;
            case 'Basketball':
                $scope.model.bettingLimitItems[0].checked = false;

                var soccerElement = $('#_Soccer');
                var soccerChecked = $(soccerElement).hasClass('copied');
                $(soccerElement).removeClass('copied');
                $(soccerElement).removeClass('copyActive');
                $(soccerElement).addClass('true');

                if (soccerChecked) {
                    for (var i = 1; i < length - 2; i++) {
                        $scope.model.bettingLimitItems[i].bgClass = '';

                        var minBet = $scope.model.bettingLimitItems[i].preValMinBet;
                        var maxBet = $scope.model.bettingLimitItems[i].preValMaxBet;
                        var maxPerMatch = $scope.model.bettingLimitItems[i].preValMaxPerMatch;

                        $scope.model.bettingLimitItems[i].minBetFormated = minBet;
                        $scope.model.bettingLimitItems[i].maxBetFormated = maxBet;
                        $scope.model.bettingLimitItems[i].maxPerMatchFormated = maxPerMatch;

                        $scope.model.bettingLimitItems[i].minBet = minBet;
                        $scope.model.bettingLimitItems[i].maxBet = maxBet;
                        $scope.model.bettingLimitItems[i].maxPerMatch = maxPerMatch;
                    }
                }

                var element2 = $('#_' + scope.betting.resourceKey);
                var unchecked = element2.hasClass('true');
                var basketballBetting = $scope.model.bettingLimitItems[2];

                if (unchecked) {
                    $scope.model.bettingLimitItems[2].bgClass = '';
                    $scope.model.bettingLimitItems[2].checked = true;

                    var minBet = parseFloat(Fanex.CustomerSetting.Core.ReplaceAll((basketballBetting.minBetFormated == '' ? '0' : basketballBetting.minBetFormated), ',', ''));
                    var maxBet = parseFloat(Fanex.CustomerSetting.Core.ReplaceAll((basketballBetting.maxBetFormated == '' ? '0' : basketballBetting.maxBetFormated), ',', ''));
                    var maxPerMatch = parseFloat(Fanex.CustomerSetting.Core.ReplaceAll((basketballBetting.maxPerMatchFormated == '' ? '0' : basketballBetting.maxPerMatchFormated), ',', ''));

                    for (var i = 3; i < length - 2; i++) {
                        $scope.model.bettingLimitItems[i].bgClass = 'inactive';

                        $scope.model.bettingLimitItems[i].minBetFormated = minBet.formatNumber();
                        $scope.model.bettingLimitItems[i].maxBetFormated = maxBet.formatNumber();
                        $scope.model.bettingLimitItems[i].maxPerMatchFormated = maxPerMatch.formatNumber();

                        $scope.model.bettingLimitItems[i].minBet = minBet.formatNumber();
                        $scope.model.bettingLimitItems[i].maxBet = maxBet.formatNumber();
                        $scope.model.bettingLimitItems[i].maxPerMatch = maxPerMatch.formatNumber();
                    }

                    // Prevents model is updating value for class name
                    var timeout = setTimeout(function () {
                        clearTimeout(timeout);

                        $(element2).removeClass('true');
                        $(element2).addClass('copied');
                        $(element2).addClass('copyActive');
                    }, 50);
                }
                else {
                    $(element2).removeClass('copied');
                    $(element2).removeClass('copyActive');
                    $(element2).addClass('true');

                    $scope.model.bettingLimitItems[2].checked = false;

                    for (var i = 3; i < length - 2; i++) {
                        $scope.model.bettingLimitItems[i].bgClass = '';

                        var minBet = $scope.model.bettingLimitItems[i].preValMinBet;
                        var maxBet = $scope.model.bettingLimitItems[i].preValMaxBet;
                        var maxPerMatch = $scope.model.bettingLimitItems[i].preValMaxPerMatch;

                        $scope.model.bettingLimitItems[i].minBetFormated = minBet;
                        $scope.model.bettingLimitItems[i].maxBetFormated = maxBet;
                        $scope.model.bettingLimitItems[i].maxPerMatchFormated = maxPerMatch;

                        $scope.model.bettingLimitItems[i].minBet = minBet;
                        $scope.model.bettingLimitItems[i].maxBet = maxBet;
                        $scope.model.bettingLimitItems[i].maxPerMatch = maxPerMatch;
                    }
                }

                break;
            case 'numbergame':
                $scope.model.bettingLimitItems[1].checked = false;

                var element = $('#_' + scope.betting.resourceKey);
                var unchecked = element.hasClass('true');
                var numberGameBetting = $scope.model.bettingLimitItems[10];

                if (unchecked) {
                    $(element).removeClass('true');
                    $(element).addClass('copied');
                    $(element).addClass('copyActive');

                    $scope.model.bettingLimitItems[10].checked = true;

                    var minBet = parseFloat(Fanex.CustomerSetting.Core.ReplaceAll((numberGameBetting.minBetFormated == '' ? '0' : numberGameBetting.minBetFormated), ',', ''));
                    var maxBet = parseFloat(Fanex.CustomerSetting.Core.ReplaceAll((numberGameBetting.maxBetFormated == '' ? '0' : numberGameBetting.maxBetFormated), ',', ''));
                    var maxPerMatch = parseFloat(Fanex.CustomerSetting.Core.ReplaceAll((numberGameBetting.maxPerMatchFormated == '' ? '0' : numberGameBetting.maxPerMatchFormated), ',', ''));

                    for (var i = 11; i < length; i++) {
                        $scope.model.bettingLimitItems[i].bgClass = 'inactive';

                        $scope.model.bettingLimitItems[i].minBetFormated = minBet.formatNumber();
                        $scope.model.bettingLimitItems[i].maxBetFormated = maxBet.formatNumber();
                        $scope.model.bettingLimitItems[i].maxPerMatchFormated = maxPerMatch.formatNumber();

                        $scope.model.bettingLimitItems[i].minBet = minBet.formatNumber();
                        $scope.model.bettingLimitItems[i].maxBet = maxBet.formatNumber();
                        $scope.model.bettingLimitItems[i].maxPerMatch = maxPerMatch.formatNumber();
                    }
                }
                else {
                    $(element).removeClass('copied');
                    $(element).removeClass('copyActive');
                    $(element).addClass('true');

                    $scope.model.bettingLimitItems[10].checked = false;

                    for (var i = 11; i < length; i++) {
                        $scope.model.bettingLimitItems[i].bgClass = '';

                        var minBet = $scope.model.bettingLimitItems[i].preValMinBet;
                        var maxBet = $scope.model.bettingLimitItems[i].preValMaxBet;
                        var maxPerMatch = $scope.model.bettingLimitItems[i].preValMaxPerMatch;

                        $scope.model.bettingLimitItems[i].minBetFormated = minBet;
                        $scope.model.bettingLimitItems[i].maxBetFormated = maxBet;
                        $scope.model.bettingLimitItems[i].maxPerMatchFormated = maxPerMatch;

                        $scope.model.bettingLimitItems[i].minBet = minBet;
                        $scope.model.bettingLimitItems[i].maxBet = maxBet;
                        $scope.model.bettingLimitItems[i].maxPerMatch = maxPerMatch;
                    }
                }
                return;

            default:
                break;
        }
    };
};

Fanex.CustomerSetting.Betting.Sportsbook.Update = function (ondone) {
    var bettings = model.bettingLimitItems;
    var length = bettings.length;

    this.IsValid = function (min, max, maxPerMatch, minMin, maxMax, maxPerMatchMax, betting, isNumberGame, maxPerBall, maxPerBallMax) {
        var isNumberGameOrHappy5 = betting.id === 161 || betting.id === 164;
        var subFix = isNumberGameOrHappy5 ? '1' : '';

        if (isNaN(min)) {
            Validators.show($('#txt' + betting.resourceKey + 'Min' + subFix).get(0), betting.name + ' - Min Bet should be a value', undefined, undefined, 3000);

            return false;
        }

        if (isNaN(max)) {
            Validators.show($('#txt' + betting.resourceKey + 'Max' + subFix).get(0), betting.name + ' - Max Bet should be a value', undefined, undefined, 3000);

            return false;
        }

        if (isNaN(maxPerMatch)) {
            Validators.show($('#txt' + betting.resourceKey + 'MaxPerMatch' + subFix).get(0), betting.name + ' - Max Per Match should be a value', undefined, undefined, 3000);

            return false;
        }

        if (isNumberGame && isNaN(maxPerBall)) {
            Validators.show($('#txt' + betting.resourceKey + 'MaxPerBall1').get(0), betting.name + ' - Max Per Ball should be a value', undefined, undefined, 3000);

            return false;
        }

        if (minMin != 0 && min  < minMin) {
            Validators.show($('#txt' + betting.resourceKey + 'Min' + subFix).get(0), betting.name + ' - Min Bet should be greater than or equals to ' + minMin.formatNumber(), undefined, undefined, 3000);

            return false;
        }

        if (min > max) {
            Validators.show($('#txt' + betting.resourceKey + 'Min' + subFix).get(0), betting.name + ' - Min Bet should be less than or equals to Max Bet', undefined, undefined, 3000);

            return false;
        }

        if (min > maxPerMatch) {
            Validators.show($('#txt' + betting.resourceKey + 'Min' + subFix).get(0), betting.name + ' - Min Bet should be less than or equals to Max Per Match', undefined, undefined, 3000);

            return false;
        }

        if (maxMax != 0 && max > maxMax) {
            Validators.show($('#txt' + betting.resourceKey + 'Max' + subFix).get(0), betting.name + ' - Max Bet should be less than or equals to ' + maxMax, undefined, undefined, 3000);

            return false;
        }

        if (max > maxPerMatch) {
            Validators.show($('#txt' + betting.resourceKey + 'Max' + subFix).get(0), betting.name + ' - Max Bet should be less than or equals to Max Per Match', undefined, undefined, 3000);

            return false;
        }

        if (maxPerMatchMax != 0 && maxPerMatch > maxPerMatchMax) {
            Validators.show($('#txt' + betting.resourceKey + 'MaxPerMatch' + subFix).get(0), betting.name + ' - Max Per Match should be less than or equals to ' + maxPerMatchMax.formatNumber(), undefined, undefined, 3000);

            return false;
        }

        if (isNumberGame && maxPerBallMax != 0 && maxPerBall > maxPerBallMax) {
            Validators.show($('#txt' + betting.resourceKey + 'MaxPerBall1').get(0), betting.name + ' - Max Per Ball should be less than or equals to ' + maxPerBallMax.formatNumber(), undefined, undefined, 3000);

            return false;
        }

        if (isNumberGameOrHappy5 && defaultMinBetValue && min < defaultMinBetValue) {
            Validators.show($('#txt' + betting.resourceKey + 'Min' + subFix).get(0), betting.name + ' - Min Bet should be greater than or equals to ' + defaultMinBetValue.formatNumber(), undefined, undefined, 3000);

            return false;
        }

        return true;
    };

    for (var i = 0; i < length; i++) {
        var betting = bettings[i];

        var curValMinBet = parseFloat(Fanex.CustomerSetting.Core.ReplaceAll(betting.minBetFormated, ',', ''));
        var curValMaxBet = parseFloat(Fanex.CustomerSetting.Core.ReplaceAll(betting.maxBetFormated, ',', ''));

        var curValMaxPerMatch = parseFloat(Fanex.CustomerSetting.Core.ReplaceAll(betting.maxPerMatchFormated, ',', ''));
        var curValMaxPerMatchMax = betting.maxPerMatchMax;

        var curValMinMin = betting.minBetMin;
        var curValMaxMax = betting.maxBetMin;

        if (typeof betting.maxPerBall != 'undefined') {
            var curValMaxPerBall = parseFloat(Fanex.CustomerSetting.Core.ReplaceAll(betting.maxPerBallFormated, ',', ''));
            var maxPerBallMax = betting.maxPerBallMax;

            if (!this.IsValid(curValMinBet, curValMaxBet, curValMaxPerMatch, curValMinMin, curValMaxMax, curValMaxPerMatchMax, betting, true, curValMaxPerBall, maxPerBallMax)) {
                return;
            }
        }
        else {
            if (!this.IsValid(curValMinBet, curValMaxBet, curValMaxPerMatch, curValMinMin, curValMaxMax, curValMaxPerMatchMax, betting)) {
                return;
            }
        }
    }

    Fanex.CustomerSetting.Core.BlockUI();

    $.ajax({
        dataType: 'json',
        type: 'post',
        url: toUrl('Sportsbook/EditBetSettingPost'),
        data: {
            custId: custId,
            uplineId: uplineId,
            userId: userId,
            txtCustomerObject: JSON.stringify(model)
        }
    })
    .success(function (data) {
        if (data.errorCode == "0") {
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
            ondone(data);
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