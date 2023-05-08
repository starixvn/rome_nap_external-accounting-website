if (typeof Fanex == 'undefined' || typeof Fanex.CustomerSetting == 'undefined' || typeof Fanex.CustomerSetting.Core == 'undefined') {
    alert('Require Fanex.CustomerSetting.Core.js');
};

Validators.show = function (elementInput, message, xPos, pointerXPos, timeout, yPos) {
    // <span class="hint"><span>This is the message.<span><span class="hint-pointer">&nbsp;</span></span>
    var hideTimeout = timeout ? timeout : 1000;
    var hint = document.getElementById('fHint');
    var hintPointer = null;
    if (hint == null) {
        var hint = document.createElement("SPAN");
        hint.className = 'hint';
        var hintMsg = document.createElement("SPAN");
        hint.appendChild(hintMsg);
        hintPointer = document.createElement("SPAN");
        hintPointer.className = 'hint-pointer';
        hint.appendChild(hintPointer);
        hint.id = 'fHint';
        hint.style.top = '10px';
        hint.style.left = '10px';
        hint.style.zIndex = 999999;
        hint.onclick = function () { // Hide onclick
            hint.style.display = 'none';
        };
        document.body.appendChild(hint);
    };
    hintPointer = hint.lastChild;
    hint.firstChild.innerHTML = message;
    if (this.timeOutHideTooltip) {
        clearTimeout(this.timeOutHideTooltip);
        this.timeOutHideTooltip = null;
    };
    this.timeOutHideTooltip = setTimeout(function () {
        hint.style.display = 'none';
    }, hideTimeout); // Auto hide in 1s

    hint.style.display = 'block';
    if (elementInput != null) {
        if (typeof xPos == 'undefined') {
            xPos = 0;
        }

        if (typeof yPos == 'undefined') {
            yPos = 0;
        }

        if (typeof pointerXPos == 'undefined') {
            pointerXPos = 10;
        }

        var pos = Validators.utils.findPosRelativeToViewport(elementInput);
        if (pos[0] + hint.offsetWidth > document.body.offsetWidth) {
            hintPointer.style.left = 'auto';
            hintPointer.style.right = '10px';
            hint.style.left = 'auto';
            hint.style.right = document.body.offsetLeft + document.body.offsetWidth - pos[0] - elementInput.offsetWidth + 'px';
        } else {
            hintPointer.style.left = '10px';
            hintPointer.style.right = 'auto';
            hint.style.left = pos[0] + 'px';
            hint.style.right = 'auto';
        }

        hint.style.backgroundPositionX = pointerXPos + 'px';
        hint.style.top = pos[1] - 40 + document.body.scrollTop + yPos + 'px';

        elementInput.onchange = function () {
            hint.style.display = 'none';
        };
        elementInput.focus();
    }
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
            for (var i = 1; i < length; i++) {
                _model.bettingLimits[i].minBetFormated = parseFloat(Fanex.CustomerSetting.Core.ReplaceAll((newValue == '' ? '0' : newValue), ',', '')).formatNumber();
                _model.bettingLimits[i].minBet = _model.bettingLimits[i].minBetFormated;
            }
        }
    });

    $scope.$watch("model.bettingLimitItems[0].maxBetFormated", function (newValue, oldValue, _model) {
        if (_model.bettingLimits[0].checked) {
            for (var i = 1; i < length; i++) {
                _model.bettingLimits[i].maxBetFormated = parseFloat(Fanex.CustomerSetting.Core.ReplaceAll((newValue == '' ? '0' : newValue), ',', '')).formatNumber();
                _model.bettingLimits[i].maxBet = _model.bettingLimits[i].maxBetFormated;
            }
        }
    });

    $scope.$watch("model.bettingLimitItems[0].maxPerMatchFormated", function (newValue, oldValue, _model) {
        if (_model.bettingLimits[0].checked) {
            for (var i = 1; i < length; i++) {
                _model.bettingLimits[i].maxPerMatchFormated = parseFloat(Fanex.CustomerSetting.Core.ReplaceAll((newValue == '' ? '0' : newValue), ',', '')).formatNumber();
                _model.bettingLimits[i].maxPerMatch = _model.bettingLimits[i].maxPerMatchFormated;
            }
        }
    });

    $scope.$watch("model.bettingLimitItems[0].maxPayoutPerMatchFormated", function (newValue, oldValue, _model) {
        if (_model.bettingLimits[0].checked) {
            for (var i = 1; i < length; i++) {
                _model.bettingLimits[i].maxPayoutPerMatchFormated = parseFloat(Fanex.CustomerSetting.Core.ReplaceAll((newValue == '' ? '0' : newValue), ',', '')).formatNumber();
                _model.bettingLimits[i].maxPayoutPerMatch = _model.bettingLimits[i].maxPayoutPerMatchFormated;
            }
        }
    });

    $scope.$watch("model.bettingLimitItems[3].minBetFormated", function (newValue, oldValue, _model) {
        if (_model.bettingLimits[3].checked) {
            for (var i = 3; i < length; i++) {
                _model.bettingLimits[i].minBetFormated = parseFloat(Fanex.CustomerSetting.Core.ReplaceAll((newValue == '' ? '0' : newValue), ',', '')).formatNumber();
                _model.bettingLimits[i].minBet = _model.bettingLimits[i].minBetFormated;
            }
        }
    });

    $scope.$watch("model.bettingLimitItems[3].maxBetFormated", function (newValue, oldValue, _model) {
        if (_model.bettingLimits[3].checked) {
            for (var i = 3; i < length; i++) {
                _model.bettingLimits[i].maxBetFormated = parseFloat(Fanex.CustomerSetting.Core.ReplaceAll((newValue == '' ? '0' : newValue), ',', '')).formatNumber();
                _model.bettingLimits[i].maxBet = _model.bettingLimits[i].maxBetFormated;
            }
        }
    });

    $scope.$watch("model.bettingLimitItems[3].maxPerMatchFormated", function (newValue, oldValue, _model) {
        if (_model.bettingLimits[3].checked) {
            for (var i = 3; i < length; i++) {
                _model.bettingLimits[i].maxPerMatchFormated = parseFloat(Fanex.CustomerSetting.Core.ReplaceAll((newValue == '' ? '0' : newValue), ',', '')).formatNumber();
                _model.bettingLimits[i].maxPerMatch = _model.bettingLimits[i].maxPerMatchFormated;
            }
        }
    });

    $scope.$watch("model.bettingLimitItems[3].maxPayoutPerMatchFormated", function (newValue, oldValue, _model) {
        if (_model.bettingLimits[3].checked) {
            for (var i = 3; i < length; i++) {
                _model.bettingLimits[i].maxPayoutPerMatchFormated = parseFloat(Fanex.CustomerSetting.Core.ReplaceAll((newValue == '' ? '0' : newValue), ',', '')).formatNumber();
                _model.bettingLimits[i].maxPayoutPerMatch = _model.bettingLimits[i].maxPayoutPerMatchFormated;
            }
        }
    });

    $scope.$watch("model.bettingLimitItems[11].minBetFormated", function (newValue, oldValue, _model) {
        if (_model.bettingLimits[11].checked) {
            for (var i = 11; i < length; i++) {
                _model.bettingLimits[i].minBetFormated = parseFloat(Fanex.CustomerSetting.Core.ReplaceAll((newValue == '' ? '0' : newValue), ',', '')).formatNumber();
                _model.bettingLimits[i].minBet = _model.bettingLimits[i].minBetFormated;
            }
        }
    });

    $scope.$watch("model.bettingLimitItems[11].maxBetFormated", function (newValue, oldValue, _model) {
        if (_model.bettingLimits[11].checked) {
            for (var i = 11; i < length; i++) {
                _model.bettingLimits[i].maxBetFormated = parseFloat(Fanex.CustomerSetting.Core.ReplaceAll((newValue == '' ? '0' : newValue), ',', '')).formatNumber();
                _model.bettingLimits[i].maxBet = _model.bettingLimits[i].maxBetFormated;
            }
        }
    });

    $scope.$watch("model.bettingLimitItems[11].maxPerMatchFormated", function (newValue, oldValue, _model) {
        if (_model.bettingLimits[11].checked) {
            for (var i = 11; i < length; i++) {
                _model.bettingLimits[i].maxPerMatchFormated = parseFloat(Fanex.CustomerSetting.Core.ReplaceAll((newValue == '' ? '0' : newValue), ',', '')).formatNumber();
                _model.bettingLimits[i].maxPerMatch = _model.bettingLimits[i].maxPerMatchFormated;
            }
        }
    });

    $scope.Init = function () {
        for (var i = 0; i < length; i++) {
            $scope.model.bettingLimitItems[i].title = $scope.model.bettingLimitItems[i].resourceKey == 'Soccer' ?
                'Copy all below bet settings from Soccer' : $scope.model.bettingLimitItems[i].resourceKey == 'Basketball' ?
                    'Copy all below bet settings from Basketball ' : '';
        }
    };

    $scope.Copy = function (scope) {
        switch (scope.betting.resourceKey) {
            case 'Soccer':
                $scope.model.bettingLimitItems[3].checked = false;

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
                    var maxPayoutPerMatch = parseFloat(Fanex.CustomerSetting.Core.ReplaceAll((soccerBetting.maxPayoutPerMatchFormated == '' ? '0' : soccerBetting.maxPayoutPerMatchFormated), ',', ''));

                    for (var i = 1; i < length; i++) {
                        $scope.model.bettingLimitItems[i].bgClass = 'inactive';

                        $scope.model.bettingLimitItems[i].minBetFormated = minBet.formatNumber();
                        $scope.model.bettingLimitItems[i].maxBetFormated = maxBet.formatNumber();
                        $scope.model.bettingLimitItems[i].maxPerMatchFormated = maxPerMatch.formatNumber();
                        $scope.model.bettingLimitItems[i].maxPayoutPerMatchFormated = maxPayoutPerMatch.formatNumber();

                        $scope.model.bettingLimitItems[i].minBet = minBet.formatNumber();
                        $scope.model.bettingLimitItems[i].maxBet = maxBet.formatNumber();
                        $scope.model.bettingLimitItems[i].maxPerMatch = maxPerMatch.formatNumber();
                        $scope.model.bettingLimitItems[i].maxPayoutPerMatch = maxPayoutPerMatch.formatNumber();
                    }
                }
                else {
                    $(element).removeClass('copied');
                    $(element).removeClass('copyActive');
                    $(element).addClass('true');

                    $scope.model.bettingLimitItems[0].checked = false;

                    for (var i = 1; i < length; i++) {
                        $scope.model.bettingLimitItems[i].bgClass = '';

                        var minBet = $scope.model.bettingLimitItems[i].preValMinBet;
                        var maxBet = $scope.model.bettingLimitItems[i].preValMaxBet;
                        var maxPerMatch = $scope.model.bettingLimitItems[i].preValMaxPerMatch;
                        var maxPayoutPerMatch = $scope.model.bettingLimitItems[i].preValMaxPayoutPerMatch;

                        $scope.model.bettingLimitItems[i].minBetFormated = minBet;
                        $scope.model.bettingLimitItems[i].maxBetFormated = maxBet;
                        $scope.model.bettingLimitItems[i].maxPerMatchFormated = maxPerMatch;
                        $scope.model.bettingLimitItems[i].maxPayoutPerMatchFormated = maxPayoutPerMatch;

                        $scope.model.bettingLimitItems[i].minBet = minBet;
                        $scope.model.bettingLimitItems[i].maxBet = maxBet;
                        $scope.model.bettingLimitItems[i].maxPerMatch = maxPerMatch;
                        $scope.model.bettingLimitItems[i].maxPayoutPerMatch = maxPayoutPerMatch;
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
                    for (var i = 1; i < length; i++) {
                        $scope.model.bettingLimitItems[i].bgClass = '';

                        var minBet = $scope.model.bettingLimitItems[i].preValMinBet;
                        var maxBet = $scope.model.bettingLimitItems[i].preValMaxBet;
                        var maxPerMatch = $scope.model.bettingLimitItems[i].preValMaxPerMatch;
                        var maxPayoutPerMatch = $scope.model.bettingLimitItems[i].preValMaxPayoutPerMatch;

                        $scope.model.bettingLimitItems[i].minBetFormated = minBet;
                        $scope.model.bettingLimitItems[i].maxBetFormated = maxBet;
                        $scope.model.bettingLimitItems[i].maxPerMatchFormated = maxPerMatch;
                        $scope.model.bettingLimitItems[i].maxPayoutPerMatchFormated = maxPayoutPerMatch;

                        $scope.model.bettingLimitItems[i].minBet = minBet;
                        $scope.model.bettingLimitItems[i].maxBet = maxBet;
                        $scope.model.bettingLimitItems[i].maxPerMatch = maxPerMatch;
                        $scope.model.bettingLimitItems[i].maxPayoutPerMatch = maxPayoutPerMatch;
                    }
                }

                var element2 = $('#_' + scope.betting.resourceKey);
                var unchecked = element2.hasClass('true');
                var basketballBetting = $scope.model.bettingLimitItems[3];

                if (unchecked) {
                    $scope.model.bettingLimitItems[3].bgClass = '';
                    $scope.model.bettingLimitItems[3].checked = true;

                    var minBet = parseFloat(Fanex.CustomerSetting.Core.ReplaceAll((basketballBetting.minBetFormated == '' ? '0' : basketballBetting.minBetFormated), ',', ''));
                    var maxBet = parseFloat(Fanex.CustomerSetting.Core.ReplaceAll((basketballBetting.maxBetFormated == '' ? '0' : basketballBetting.maxBetFormated), ',', ''));
                    var maxPerMatch = parseFloat(Fanex.CustomerSetting.Core.ReplaceAll((basketballBetting.maxPerMatchFormated == '' ? '0' : basketballBetting.maxPerMatchFormated), ',', ''));
                    var maxPayoutPerMatch = parseFloat(Fanex.CustomerSetting.Core.ReplaceAll((basketballBetting.maxPayoutPerMatchFormated == '' ? '0' : basketballBetting.maxPayoutPerMatchFormated), ',', ''));

                    for (var i = 4; i < length; i++) {
                        $scope.model.bettingLimitItems[i].bgClass = 'inactive';

                        $scope.model.bettingLimitItems[i].minBetFormated = minBet.formatNumber();
                        $scope.model.bettingLimitItems[i].maxBetFormated = maxBet.formatNumber();
                        $scope.model.bettingLimitItems[i].maxPerMatchFormated = maxPerMatch.formatNumber();
                        $scope.model.bettingLimitItems[i].maxPayoutPerMatchFormated = maxPayoutPerMatch.formatNumber();

                        $scope.model.bettingLimitItems[i].minBet = minBet.formatNumber();
                        $scope.model.bettingLimitItems[i].maxBet = maxBet.formatNumber();
                        $scope.model.bettingLimitItems[i].maxPerMatch = maxPerMatch.formatNumber();
                        $scope.model.bettingLimitItems[i].maxPayoutPerMatch = maxPayoutPerMatch.formatNumber();
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

                    $scope.model.bettingLimitItems[3].checked = false;

                    for (var i = 4; i < length; i++) {
                        $scope.model.bettingLimitItems[i].bgClass = '';

                        var minBet = $scope.model.bettingLimitItems[i].preValMinBet;
                        var maxBet = $scope.model.bettingLimitItems[i].preValMaxBet;
                        var maxPerMatch = $scope.model.bettingLimitItems[i].preValMaxPerMatch;
                        var maxPayoutPerMatch = $scope.model.bettingLimitItems[i].preValMaxPayoutPerMatch;

                        $scope.model.bettingLimitItems[i].minBetFormated = minBet;
                        $scope.model.bettingLimitItems[i].maxBetFormated = maxBet;
                        $scope.model.bettingLimitItems[i].maxPerMatchFormated = maxPerMatch;
                        $scope.model.bettingLimitItems[i].maxPayoutPerMatchFormated = maxPayoutPerMatch;

                        $scope.model.bettingLimitItems[i].minBet = minBet;
                        $scope.model.bettingLimitItems[i].maxBet = maxBet;
                        $scope.model.bettingLimitItems[i].maxPerMatch = maxPerMatch;
                        $scope.model.bettingLimitItems[i].maxPayoutPerMatch = maxPayoutPerMatch;
                    }
                }
                break;  
            default:
                break;
        }
    };
};

Fanex.CustomerSetting.Betting.Sportsbook.Update = function (ondone) {
    var bettings = model.bettingLimitItems;
    var length = bettings.length;

    this.IsValid = function (min, max, maxPerMatch, minMin, maxMax, maxPerMatchMax, betting) {

        var maxPayoutPerMatch;
        var maxPayoutPerMatchMax;
        if (typeof betting.maxPayoutPerMatch != 'undefined') {
            maxPayoutPerMatch = parseFloat(Fanex.CustomerSetting.Core.ReplaceAll(betting.maxPayoutPerMatchFormated, ',', ''));
            maxPayoutPerMatchMax = betting.maxPayoutPerMatchMax;
        }

        if (isNaN(min)) {
            Validators.show($('#txt' + betting.resourceKey + 'Min').get(0), betting.name + ' - Min Bet must be a value', undefined, undefined, 3000);

            return false;
        }

        if (isNaN(max)) {
            Validators.show($('#txt' + betting.resourceKey + 'Max').get(0), betting.name + ' - Max Bet must be a value', undefined, undefined, 3000);

            return false;
        }

        if (isNaN(maxPerMatch)) {
            Validators.show($('#txt' + betting.resourceKey + 'MaxPerMatch').get(0), betting.name + ' - Max Per Match must be a value', undefined, undefined, 3000);

            return false;
        }

        if (isNaN(maxPayoutPerMatch)) {
            Validators.show($('#txt' + betting.resourceKey + 'MaxPayoutPerMatch').get(0), betting.name + ' - Max Payout Per Match must be a value', -115, 150, 3000);

            return false;
        }
     

        if (minMin != 0 && min < minMin) {
            Validators.show($('#txt' + betting.resourceKey + 'Min').get(0), betting.name + ' - Min Bet must be greater than or equals to ' + minMin.formatNumber(), undefined, undefined, 3000);

            return false;
        }

        if (min > max) {
            Validators.show($('#txt' + betting.resourceKey + 'Min').get(0), betting.name + ' - Min Bet must be less than or equals to Max Bet', undefined, undefined, 3000);

            return false;
        }

        if (min > maxPerMatch) {
            Validators.show($('#txt' + betting.resourceKey + 'Min').get(0), betting.name + ' - Min Bet must be less than or equals to Max Per Match', undefined, undefined, 3000);

            return false;
        }

        if (maxMax != 0 && max > maxMax) {
            Validators.show($('#txt' + betting.resourceKey + 'Max').get(0), betting.name + ' - Max Bet must be less than or equals to ' + maxMax, undefined, undefined, 3000);

            return false;
        }

        if (max > maxPerMatch) {
            Validators.show($('#txt' + betting.resourceKey + 'Max').get(0), betting.name + ' - Max Bet must be less than or equals to Max Per Match', undefined, undefined, 3000);

            return false;
        }

        if (maxPerMatchMax != 0 && maxPerMatch > maxPerMatchMax) {
            Validators.show($('#txt' + betting.resourceKey + 'MaxPerMatch').get(0), betting.name + ' - Max Per Match must be less than or equals to ' + maxPerMatchMax.formatNumber(), undefined, undefined, 3000);

            return false;
        }

        if (maxPayoutPerMatchMax != 0 && maxPayoutPerMatch > maxPayoutPerMatchMax) {
            Validators.show($('#txt' + betting.resourceKey + 'MaxPayoutPerMatch').get(0), betting.name + ' - Max Payout Per Match must be less than or equals to ' + maxPayoutPerMatchMax.formatNumber(), undefined, undefined, 3000);

            return false;
        }

        if (maxPerMatch > maxPayoutPerMatch) {
            Validators.show($('#txt' + betting.resourceKey + 'MaxPerMatch').get(0), betting.name + ' - Max Per Match must be less than or equal to Max Payout Per Match', undefined, undefined, 3000);

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

            if (!this.IsValid(curValMinBet, curValMaxBet, curValMaxPerMatch, curValMinMin, curValMaxMax, curValMaxPerMatchMax, betting)) {
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

            Fanex.CustomerSetting.Core.UnBlockUI();

            if (top.Fanex.ExtAcc.ShowSecurityCodePage(data)) {
                return;
            }

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

            if (typeof ondone == 'function') {
                ondone(data);
            }
        })
        .error(function (data) {

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