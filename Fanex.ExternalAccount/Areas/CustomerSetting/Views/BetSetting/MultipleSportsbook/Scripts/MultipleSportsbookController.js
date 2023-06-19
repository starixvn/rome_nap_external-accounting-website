if (typeof Fanex == 'undefined' || typeof Fanex.CustomerSetting == 'undefined' || typeof Fanex.CustomerSetting.Core == 'undefined') {
    alert('Require Fanex.CustomerSetting.Core.js');
};

Validators.show = function (elementInput, message, xPos, pointerXPos, timeout, yPos, height) {
    var hideTimeout = timeout ? timeout : 1000;
    var hint = document.getElementById('fHint');
    var hintPointer = null;
    if (hint == null) {
        hint = document.createElement("SPAN");
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

        if (typeof (height) == 'undefined') {
            height = 13;
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

        if (height > 13) {
            hintPointer.style.top = '55px';
        }
        else {
            hintPointer.style.top = '33px';
        }
        hint.style.height = height + 'px';
        hint.style.backgroundPositionX = pointerXPos + 'px';
        if (height === 13) {
            hint.style.top = pos[1] - 40 + document.body.scrollTop + yPos + 'px';
        }
        else {
            hint.style.top = pos[1] - 62 + document.body.scrollTop + yPos + 'px';
        }

        elementInput.onchange = function () {
            hint.style.display = 'none';
        };
        elementInput.focus();
    }
};

Validators.showDown = function (elementInput, message, xPos, pointerXPos, yPos, pointerYPos, height, timeout) {
    var hint = document.getElementById('fHintDown');
    var hintPointer = null;
    if (hint == null) {
        var hint = document.createElement("SPAN");
        hint.className = 'hint';
        var hintMsg = document.createElement("SPAN");
        hint.appendChild(hintMsg);
        hintPointer = document.createElement("SPAN");
        hintPointer.className = 'hint-pointer-down';
        hint.appendChild(hintPointer);
        hint.id = 'fHintDown';
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
    if (typeof (timeout) == 'undefined') {
        timeout = 1000;
    }
    this.timeOutHideTooltip = setTimeout(function () {
        if (hint == null) {
            return;
        }
        hint.style.display = 'none';
    }, timeout); // Auto hide in 1s
    if (elementInput != null) {
        if (typeof xPos == 'undefined') {
            xPos = 0;
        }
        if (typeof pointerXPos == 'undefined') {
            pointerXPos = 427;
        }
        if (typeof yPos == 'undefined') {
            yPos = -45;
        }
        if (typeof (pointerYPos) == 'undefined') {
            pointerYPos = -10;
        }
        if (typeof (height) == 'undefined') {
            height = 18;
        }
        hint.style.height = height + 'px';
        hintPointer.style.left = '427px';
        hintPointer.style.top = '-10px';
        var pos = Validators.utils.findPosRelativeToViewport(elementInput);
        hint.style.left = '5px';
        hint.style.top = '36px';
        elementInput.onchange = function () {
            hint.style.display = 'none';
        };
        elementInput.focus();
    };
    hint.style.display = 'block';
};

Fanex.CustomerSetting.Betting = {};

Fanex.CustomerSetting.Betting.MultipleSportsbook = function ($scope, $element) {
    $scope.model = model;

    if (!model)
        return;

    $scope.bettingLimits = $scope.model.bettingLimitItems;

    var length = $scope.model.bettingLimitItems.length;

    $scope.$watch("model.bettingLimitItems[0].minBetFormated", function (newValue, oldValue, _model) {
        if (_model.bettingLimits[0].checked) {
            for (var i = 1; i < length; i++) {
                _model.bettingLimits[i].minBetFormated = parseFloat((newValue === '' || isNaN(newValue.replace(/,/g, '')) ? '0' : newValue).replace(/,/g, '')).formatNumber();
                _model.bettingLimits[i].minBet = _model.bettingLimits[i].minBetFormated;
            }
        }
    });

    $scope.$watch("model.bettingLimitItems[0].maxBetFormated", function (newValue, oldValue, _model) {
        if (_model.bettingLimits[0].checked) {
            for (var i = 1; i < length; i++) {
                _model.bettingLimits[i].maxBetFormated = parseFloat((newValue === '' || isNaN(newValue.replace(/,/g, '')) ? '0' : newValue).replace(/,/g, '')).formatNumber();
                _model.bettingLimits[i].maxBet = _model.bettingLimits[i].maxBetFormated;
            }
        }
    });

    $scope.$watch("model.bettingLimitItems[0].maxPerMatchFormated", function (newValue, oldValue, _model) {
        if (_model.bettingLimits[0].checked) {
            for (var i = 1; i < length; i++) {
                _model.bettingLimits[i].maxPerMatchFormated = parseFloat((newValue === '' || isNaN(newValue.replace(/,/g, '')) ? '0' : newValue).replace(/,/g, '')).formatNumber();
                _model.bettingLimits[i].maxPerMatch = _model.bettingLimits[i].maxPerMatchFormated;
            }
        }
    });

    $scope.$watch("model.bettingLimitItems[0].maxPayoutPerMatchFormated", function (newValue, oldValue, _model) {
        if (_model.bettingLimits[0].checked) {
            for (var i = 1; i < length; i++) {
                _model.bettingLimits[i].maxPayoutPerMatchFormated = parseFloat((newValue === '' || isNaN(newValue.replace(/,/g, '')) ? '0' : newValue).replace(/,/g, '')).formatNumber();
                _model.bettingLimits[i].maxPayoutPerMatch = _model.bettingLimits[i].maxPayoutPerMatchFormated;
            }
        }
    });

    $scope.$watch("model.bettingLimitItems[3].minBetFormated", function (newValue, oldValue, _model) {
        if (_model.bettingLimits[3].checked) {
            for (var i = 3; i < length; i++) {
                _model.bettingLimits[i].minBetFormated = parseFloat((newValue === '' || isNaN(newValue.replace(/,/g, '')) ? '0' : newValue).replace(/,/g, '')).formatNumber();
                _model.bettingLimits[i].minBet = _model.bettingLimits[i].minBetFormated;
            }
        }
    });

    $scope.$watch("model.bettingLimitItems[3].maxBetFormated", function (newValue, oldValue, _model) {
        if (_model.bettingLimits[3].checked) {
            for (var i = 3; i < length; i++) {
                _model.bettingLimits[i].maxBetFormated = parseFloat((newValue === '' || isNaN(newValue.replace(/,/g, '')) ? '0' : newValue).replace(/,/g, '')).formatNumber();
                _model.bettingLimits[i].maxBet = _model.bettingLimits[i].maxBetFormated;
            }
        }
    });

    $scope.$watch("model.bettingLimitItems[3].maxPerMatchFormated", function (newValue, oldValue, _model) {
        if (_model.bettingLimits[3].checked) {
            for (var i = 3; i < length; i++) {
                _model.bettingLimits[i].maxPerMatchFormated = parseFloat((newValue === '' || isNaN(newValue.replace(/,/g, '')) ? '0' : newValue).replace(/,/g, '')).formatNumber();
                _model.bettingLimits[i].maxPerMatch = _model.bettingLimits[i].maxPerMatchFormated;
            }
        }
    });

    $scope.$watch("model.bettingLimitItems[3].maxPayoutPerMatchFormated", function (newValue, oldValue, _model) {
        if (_model.bettingLimits[3].checked) {
            for (var i = 3; i < length; i++) {
                _model.bettingLimits[i].maxPayoutPerMatchFormated = parseFloat((newValue === '' || isNaN(newValue.replace(/,/g, '')) ? '0' : newValue).replace(/,/g, '')).formatNumber();
                _model.bettingLimits[i].maxPayoutPerMatch = _model.bettingLimits[i].maxPayoutPerMatchFormated;
            }
        }
    });

    $scope.$watch("model.bettingLimitItems[11].minBetFormated", function (newValue, oldValue, _model) {
        if (_model.bettingLimits[11].checked) {
            for (var i = 11; i < length; i++) {
                _model.bettingLimits[i].minBetFormated = parseFloat((newValue === '' || isNaN(newValue.replace(/,/g, '')) ? '0' : newValue).replace(/,/g, '')).formatNumber();
                _model.bettingLimits[i].minBet = _model.bettingLimits[i].minBetFormated;
            }
        }
    });

    $scope.$watch("model.bettingLimitItems[11].maxBetFormated", function (newValue, oldValue, _model) {
        if (_model.bettingLimits[11].checked) {
            for (var i = 11; i < length; i++) {
                _model.bettingLimits[i].maxBetFormated = parseFloat((newValue === '' || isNaN(newValue.replace(/,/g, '')) ? '0' : newValue).replace(/,/g, '')).formatNumber();
                _model.bettingLimits[i].maxBet = _model.bettingLimits[i].maxBetFormated;
            }
        }
    });

    $scope.$watch("model.bettingLimitItems[11].maxPerMatchFormated", function (newValue, oldValue, _model) {
        if (_model.bettingLimits[11].checked) {
            for (var i = 11; i < length; i++) {
                _model.bettingLimits[i].maxPerMatchFormated = parseFloat((newValue === '' || isNaN(newValue.replace(/,/g, '')) ? '0' : newValue).replace(/,/g, '')).formatNumber();
                _model.bettingLimits[i].maxPerMatch = _model.bettingLimits[i].maxPerMatchFormated;
            }
        }
    });

    $scope.Check = function (id) {
        var checkboxes = $("input[type=checkbox]");
        var bettingItemIndex = 0;
        var isSoccer = false;
        var isBasketball = false;
        var bettingItem = null;

        for (var i in checkboxes) {
            if (!isNaN(i)) {
                if (i > 0 && i <= $scope.model.bettingLimitItems.length && id !== "Basketball" &&
                    $scope.model.bettingLimitItems[i - 1].resourceKey === "Basketball" && $scope.model.bettingLimitItems[i - 1].checked) {
                    break;
                }
                else {
                    checkboxes[i].disabled = false;
                }
            }
        }

        if (id === "All") {
            if ($scope.model.bettingLimitItems[0].checked) {
                for (var item of $scope.model.bettingLimitItems) {
                    if (item.resourceKey !== "Soccer") {
                        item.minBet = item.minBetFormated = item.preValMinBet;
                        item.maxBet = item.maxBetFormated = item.preValMaxBet;
                        item.maxPerMatch = item.maxPerMatchFormated = item.preValMaxPerMatch;
                        item.maxPayoutPerMatch = item.maxPayoutPerMatchFormated = item.preValMaxPayoutPerMatch;
                    }
                }
            }

            if ($scope.model.bettingLimitItems[3].checked) {
                for (var item2 of $scope.model.bettingLimitItems) {
                    if (item2.resourceKey !== "Basketball" && item2.resourceKey !== "Soccer") {
                        item2.minBet = item2.minBetFormated = item2.preValMinBet;
                        item2.maxBet = item2.maxBetFormated = item2.preValMaxBet;
                        item2.maxPerMatch = item2.maxPerMatchFormated = item2.preValMaxPerMatch;
                        item2.maxPayoutPerMatch = item2.maxPayoutPerMatchFormated = item2.preValMaxPayoutPerMatch;
                    }
                }
            }

            $scope.model.bettingLimitItems[0].checked = false;
            $scope.model.bettingLimitItems[3].checked = false;
            $("#_Basketball").removeClass("copied").removeClass("copyActive").addClass("true");
            $("#_Soccer").removeClass("copied").removeClass("copyActive").addClass("true");            

            if ($("#" + id).is(":checked")) {
                $("#btnSubmit").removeClass("-disabled");

                for (var checkbox in checkboxes) {
                    if (!isNaN(checkbox)) {
                        checkboxes[checkbox].checked = true;
                        checkboxes[checkbox].disabled = false;
                    }

                    if (checkboxes[checkbox].id !== "All") {
                        if (bettingItemIndex < $scope.model.bettingLimitItems.length) {
                            $scope.model.bettingLimitItems[bettingItemIndex].bgClass = "";
                            bettingItemIndex++;
                        }
                    }
                }
            } else {
                $("#btnSubmit").addClass("-disabled");

                for (var cb in checkboxes) {
                    if (!isNaN(cb)) {
                        checkboxes[cb].checked = false;
                    }

                    if (checkboxes[cb].id !== "All") {
                        if (bettingItemIndex < $scope.model.bettingLimitItems.length) {
                            if (isSoccer || isBasketball) {
                                bettingItem = $scope.model.bettingLimitItems[bettingItemIndex];

                                bettingItem.minBet = bettingItem.minBetFormated = bettingItem.preValMinBet;
                                bettingItem.maxBet = bettingItem.maxBetFormated = bettingItem.preValMaxBet;
                                bettingItem.maxPerMatch = bettingItem.maxPerMatchFormated = bettingItem.preValMaxPerMatch;
                                bettingItem.maxPayoutPerMatch = bettingItem.maxPayoutPerMatchFormated = bettingItem.preValMaxPayoutPerMatch;
                            }

                            if ($scope.model.bettingLimitItems[bettingItemIndex].resourceKey === 'Soccer' &&
                                $scope.model.bettingLimitItems[bettingItemIndex].checked) {
                                $scope.model.bettingLimitItems[bettingItemIndex].checked = false;
                                isSoccer = true;
                            }

                            if ($scope.model.bettingLimitItems[bettingItemIndex].resourceKey === 'Basketball' &&
                                $scope.model.bettingLimitItems[bettingItemIndex].checked) {
                                $scope.model.bettingLimitItems[bettingItemIndex].checked = false;
                                isBasketball = true;
                            }

                            $scope.model.bettingLimitItems[bettingItemIndex].bgClass = "inactive";
                            bettingItemIndex++;
                        }
                    }
                }
            }
        }
        else {
            if ($("#chk" + id).is(":checked")) {
                $("#btnSubmit").removeClass("-disabled");

                var allCheckboxesChecked = true;
                for (var j in checkboxes) {
                    if (!isNaN(j)) {
                        if (checkboxes[j].id !== "All" && !$(checkboxes[j]).is(":checked")) {
                            allCheckboxesChecked = false;
                            break;
                        }
                    }
                }

                if (allCheckboxesChecked) {
                    $('#All').get(0).checked = true;
                }

                for (var bettingLimitItem of $scope.model.bettingLimitItems) {
                    if (bettingLimitItem.resourceKey === id) {
                        bettingLimitItem.bgClass = "";
                        break;
                    }
                }
            } else {
                $('#All').get(0).checked = false;

                var allCheckboxesUnChecked = true;
                for (var k in checkboxes) {
                    if (!isNaN(k)) {
                        if (checkboxes[k].id !== "All" && $(checkboxes[k]).is(":checked")) {
                            allCheckboxesUnChecked = false;
                            break;
                        }
                    }
                }

                if (allCheckboxesUnChecked) {
                    $("#btnSubmit").addClass("-disabled");
                }

                for (bettingItem of $scope.model.bettingLimitItems) {
                    if (isSoccer || isBasketball) {
                        bettingItem.minBet = bettingItem.minBetFormated = bettingItem.preValMinBet;
                        bettingItem.maxBet = bettingItem.maxBetFormated = bettingItem.preValMaxBet;
                        bettingItem.maxPerMatch = bettingItem.maxPerMatchFormated = bettingItem.preValMaxPerMatch;
                        bettingItem.maxPayoutPerMatch = bettingItem.maxPayoutPerMatchFormated = bettingItem.preValMaxPayoutPerMatch;
                    }

                    if (id === 'Soccer' && bettingItem.resourceKey === id && bettingItem.checked) {
                        bettingItem.checked = false;
                        isSoccer = true;
                    }

                    if (id === 'Basketball' && bettingItem.resourceKey === id && bettingItem.checked) {
                        bettingItem.checked = false;
                        isBasketball = true;
                    }

                    if (bettingItem.resourceKey === id) {
                        bettingItem.bgClass = "inactive";
                    }
                }
            }
        }
    };

    $scope.ShowMixSportsParlay = function () {
        Validators.show($("#txtmixsportsMin").get(0), "<div><b>Mix Sports Parlay: &nbsp;</b>The parlay whose tickets are not just soccer.</div><div>Example: Soccer - Soccer - Basketball, Tennis - Tennis - Tennis, Baseball - Golf - Football</div>", null, null, 5000, null, 35);
    };

    $scope.ShowBetSettingNotes = function () {
        if (isSpecialGroup) {
            Validators.showDown($("#notes").get(0), "Inputting all fields of bet setting = 0 means you do not set limit to the bet setting of this account.<br/>To prevent members from playing the game, set Min Bet = 0 and other fields = 0.1 (for Sportsbook only).<br/>Updating upline's bet setting might decrease all downlines' bet settings to ensure that they are not greater than upline's bet setting.", null, null, null, null, 55, 10000);
        }
        else {
            Validators.showDown($("#notes").get(0), "Inputting all fields of bet setting = 0 means you do not set limit to the bet setting of this account.<br/>To prevent members from playing the game, set Min Bet = 0 and other fields = 0.1.<br/>Updating upline's bet setting might decrease all downlines' bet settings to ensure that they are not greater than upline's bet setting.", null, null, null, null, 55, 10000);
        }
    };

    $scope.Init = function () {
        for (var bettingLimitItem of $scope.model.bettingLimitItems) {
            if (bettingLimitItem.resourceKey === 'Soccer') {
                bettingLimitItem.title = 'Copy all below bet settings from Soccer';
            }
            else if (bettingLimitItem.resourceKey === 'Basketball') {
                bettingLimitItem.title = 'Copy all below bet settings from Basketball';
            }
            else {
                bettingLimitItem.title = '';
            }
        }
    };

    $scope.Copy = function (scope) {
        var minBet = 0;
        var maxBet = 0;
        var maxPerMatch = 0;
        var maxPayoutPerMatch = 0;
        var i = 0;
        var timeout = null;
        var unchecked = false;
        var checkboxes = $("input[type=checkbox]");
        switch (scope.betting.resourceKey) {
            case 'Soccer':
                $('#All').get(0).checked = false;
                if (!$("#chkSoccer").is(":checked")) {
                    break;
                }

                $scope.model.bettingLimitItems[3].checked = false;

                var element = $('#_' + scope.betting.resourceKey);
                unchecked = element.hasClass('true');
                var soccerBetting = $scope.model.bettingLimitItems[0];

                if (unchecked) {
                    for (i in checkboxes) {
                        if (checkboxes[i].id !== "All" && checkboxes[i].id !== "chkSoccer") {
                            checkboxes[i].checked = false;
                            checkboxes[i].disabled = true;
                        }
                    }

                    $(element).removeClass('true');
                    $(element).addClass('copied');
                    $(element).addClass('copyActive');

                    $scope.model.bettingLimitItems[0].checked = true;

                    minBet = parseFloat((soccerBetting.minBetFormated === '' || isNaN(soccerBetting.minBetFormated.replace(/,/g, '')) ? '0' : soccerBetting.minBetFormated).replace(/,/g, ''));
                    maxBet = parseFloat((soccerBetting.maxBetFormated === '' || isNaN(soccerBetting.maxBetFormated.replace(/,/g, '')) ? '0' : soccerBetting.maxBetFormated).replace(/,/g, ''));
                    maxPerMatch = parseFloat((soccerBetting.maxPerMatchFormated === '' || isNaN(soccerBetting.maxPerMatchFormated.replace(/,/g, '')) ? '0' : soccerBetting.maxPerMatchFormated).replace(/,/g, ''));
                    maxPayoutPerMatch = parseFloat((soccerBetting.maxPayoutPerMatchFormated === '' || isNaN(soccerBetting.maxPayoutPerMatchFormated.replace(/,/g, '')) ? '0' : soccerBetting.maxPayoutPerMatchFormated).replace(/,/g, ''));

                    for (i = 1; i < length; i++) {
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
                    for (i in checkboxes) {
                        if (checkboxes[i].id !== "All" && checkboxes[i].id !== "chkSoccer") {
                            checkboxes[i].checked = false;
                            checkboxes[i].disabled = false;
                        }
                    }

                    $(element).removeClass('copied');
                    $(element).removeClass('copyActive');
                    $(element).addClass('true');

                    $scope.model.bettingLimitItems[0].checked = false;

                    for (i = 1; i < length; i++) {
                        $scope.model.bettingLimitItems[i].bgClass = 'inactive';

                        minBet = $scope.model.bettingLimitItems[i].preValMinBet;
                        maxBet = $scope.model.bettingLimitItems[i].preValMaxBet;
                        maxPerMatch = $scope.model.bettingLimitItems[i].preValMaxPerMatch;
                        maxPayoutPerMatch = $scope.model.bettingLimitItems[i].preValMaxPayoutPerMatch;

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
                timeout = setTimeout(function () {
                    clearTimeout(timeout);

                    $(basketBallElement).addClass('pointer');
                }, 50);

                break;
            case 'Basketball':
                $('#All').get(0).checked = false;
                if (!$("#chkBasketball").is(":checked")) {
                    break;
                }

                $scope.model.bettingLimitItems[0].checked = false;

                var soccerElement = $('#_Soccer');
                var soccerChecked = $(soccerElement).hasClass('copied');
                $(soccerElement).removeClass('copied');
                $(soccerElement).removeClass('copyActive');
                $(soccerElement).addClass('true');

                if (soccerChecked) {
                    for (i = 1; i < length; i++) {
                        $scope.model.bettingLimitItems[i].bgClass = '';

                        minBet = $scope.model.bettingLimitItems[i].preValMinBet;
                        maxBet = $scope.model.bettingLimitItems[i].preValMaxBet;
                        maxPerMatch = $scope.model.bettingLimitItems[i].preValMaxPerMatch;
                        maxPayoutPerMatch = $scope.model.bettingLimitItems[i].preValMaxPayoutPerMatch;

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
                unchecked = element2.hasClass('true');
                var basketballBetting = $scope.model.bettingLimitItems[3];

                if (unchecked) {
                    for (i in checkboxes) {
                        if (checkboxes[i].id !== "All" && checkboxes[i].id !== "chkSoccer" &&
                            checkboxes[i].id !== "chksabaSoccer" && checkboxes[i].id !== "chksoccerParlay" &&
                            checkboxes[i].id !== "chkBasketball") {
                            checkboxes[i].checked = false;
                            checkboxes[i].disabled = true;
                        }
                    }

                    $scope.model.bettingLimitItems[3].bgClass = '';
                    $scope.model.bettingLimitItems[3].checked = true;

                    minBet = parseFloat((basketballBetting.minBetFormated === '' || isNaN(basketballBetting.minBetFormated.replace(/,/g, '')) ? '0' : basketballBetting.minBetFormated).replace(/,/g, ''));
                    maxBet = parseFloat((basketballBetting.maxBetFormated === '' || isNaN(basketballBetting.maxBetFormated.replace(/,/g, '')) ? '0' : basketballBetting.maxBetFormated).replace(/,/g, ''));
                    maxPerMatch = parseFloat((basketballBetting.maxPerMatchFormated === '' || isNaN(basketballBetting.maxPerMatchFormated.replace(/,/g, '')) ? '0' : basketballBetting.maxPerMatchFormated).replace(/,/g, ''));
                    maxPayoutPerMatch = parseFloat((basketballBetting.maxPayoutPerMatchFormated === '' || isNaN(basketballBetting.maxPayoutPerMatchFormated.replace(/,/g, '')) ? '0' : basketballBetting.maxPayoutPerMatchFormated).replace(/,/g, ''));

                    for (i = 4; i < length; i++) {
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
                    timeout = setTimeout(function () {
                        clearTimeout(timeout);

                        $(element2).removeClass('true');
                        $(element2).addClass('copied');
                        $(element2).addClass('copyActive');
                    }, 50);
                }
                else {
                    for (i in checkboxes) {
                        if (checkboxes[i].id !== "All" && checkboxes[i].id !== "chkSoccer" &&
                            checkboxes[i].id !== "chksabaSoccer" && checkboxes[i].id !== "chksoccerParlay" &&
                            checkboxes[i].id !== "chkBasketball") {
                            checkboxes[i].checked = false;
                            checkboxes[i].disabled = false;
                        }
                    }

                    $(element2).removeClass('copied');
                    $(element2).removeClass('copyActive');
                    $(element2).addClass('true');

                    $scope.model.bettingLimitItems[3].checked = false;

                    for (i = 4; i < length; i++) {
                        $scope.model.bettingLimitItems[i].bgClass = 'inactive';

                        minBet = $scope.model.bettingLimitItems[i].preValMinBet;
                        maxBet = $scope.model.bettingLimitItems[i].preValMaxBet;
                        maxPerMatch = $scope.model.bettingLimitItems[i].preValMaxPerMatch;
                        maxPayoutPerMatch = $scope.model.bettingLimitItems[i].preValMaxPayoutPerMatch;

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

var superIds = top.superIds.split(',');
var superNames = "";
var hasUpdateError = false;
var successAccountsMsg = false;
var numberOfDestUsers = 0;
var failedMessages = "";

Fanex.CustomerSetting.Betting.MultipleSportsbook.ProcessUpdateSuper = function (isCopy, custIdIndex, usernameIndex, model, para) {
    $.ajax({
        type: 'post',
        url: toUrl('Sportsbook/EditMultipleBetSettingPost'),
        data: {
            custId: superIds[custIdIndex],
            txtCustomerObject: model,
            para: para
        }
    }).success(function (data) {
        if (top.Fanex.ExtAcc.ShowSecurityCodePage(data)) {
            top.Fanex.ExtAcc.UnBlockUI();
            return;
        }

        if (data.errorCode === "0") {
            successAccountsMsg = true;
        }
        else {
            hasUpdateError = true;
            failedMessages += '<div class="bold" style="color: #444">' + $.trim(superNames[usernameIndex]) + '</div><font color="#fe0000">' + data.errorMessage + '</font>';
        }

        if (usernameIndex === numberOfDestUsers - 1) {
            top.Fanex.ExtAcc.UnBlockUI();

            if (data.errorCode === "0" && !hasUpdateError && numberOfDestUsers > 1) {
                top.$('#errMsg').append('All accounts have been successfully edited.')
                    .show();

                setTimeout(function () {
                    top.$('#errMsg').hide(300);
                }, 3000);
            }

            if (data.errorCode === "0" && !hasUpdateError && numberOfDestUsers === 1) {
                top.$('#errMsg').append('<div class="bold" style="color: #444">' + $.trim(superNames[usernameIndex]) + '</div>' + 'This account has been successfully edited.')
                    .show();

                setTimeout(function () {
                    top.$('#errMsg').hide(300);
                }, 3000);
            }

            if (successAccountsMsg && hasUpdateError) {
                top.$('#errMsg').append('All accounts have been successfully edited except the following accounts:')
                    .append(failedMessages).show();
            }

            if (!successAccountsMsg && hasUpdateError) {
                top.$('#errMsg').append(failedMessages).show();
            }
        }
        else {
            Fanex.CustomerSetting.Betting.MultipleSportsbook.ProcessUpdateSuper(isCopy, ++custIdIndex, ++usernameIndex, model, para);
        }

    }).error(function (data) {
        hasUpdateError = true;

        if (top.Fanex.ExtAcc.ShowSecurityCodePage(data)) {
            top.Fanex.ExtAcc.UnBlockUI();
            return;
        }

        top.$('#errMsg').append('<div class="bold" style="color: #444">' + $.trim(superNames[usernameIndex]) + '</div>' + "Sorry, there was an unexpected issue with the system. Please seek help from our customer support.".formatErrorMessage()).show();

        if (usernameIndex === numberOfDestUsers - 1) {
            top.Fanex.ExtAcc.UnBlockUI();
        }
        else {
            Fanex.CustomerSetting.Betting.MultipleSportsbook.ProcessUpdateSuper(isCopy, ++custIdIndex, ++usernameIndex, model, para);
        }
    });
};

Fanex.CustomerSetting.Betting.MultipleSportsbook.Update = function (isCopy) {
    numberOfDestUsers = isCopy ? superIds.length - 1 : superIds.length;
    superNames = isCopy ? top.$("#superList").val().split(',') : top.$("#superList2").val().split(',');
    hasUpdateError = false;
    successAccountsMsg = false;
    failedMessages = "";

    var checkboxes = $("input[type=checkbox]");
    var hasCheckedAnyBettype = false;
    for (var i in checkboxes) {
        if (!isNaN(i)) {
            if (checkboxes[i].checked) {
                hasCheckedAnyBettype = true;
                break;
            }
        }
    }

    if (!hasCheckedAnyBettype) {
        return;
    }

    var bettings = model.bettingLimitItems;
    var length = bettings.length;

    this.IsValid = function (min, max, maxPerMatch, minMin, maxMax, maxPerMatchMax, betting) {
        var maxPayoutPerMatch;
        var maxPayoutPerMatchMax;
        if (!isNaN(betting.maxPayoutPerMatchFormated.replace(/,/g, ''))) {
            maxPayoutPerMatch = parseFloat(betting.maxPayoutPerMatchFormated.replace(/,/g, ''));
            maxPayoutPerMatchMax = betting.maxPayoutPerMatchMax;
        }

        if (isNaN(betting.minBetFormated.replace(/,/g, '')) || isNaN(parseFloat(betting.minBet))) {
            Validators.show($('#txt' + betting.resourceKey + 'Min').get(0), betting.name + ' - Min Bet must be a value', undefined, undefined, 3000);

            return false;
        }

        if (isNaN(betting.maxBetFormated.replace(/,/g, '')) || isNaN(parseFloat(betting.maxBet))) {
            Validators.show($('#txt' + betting.resourceKey + 'Max').get(0), betting.name + ' - Max Bet must be a value', undefined, undefined, 3000);

            return false;
        }

        if (isNaN(betting.maxPerMatchFormated.replace(/,/g, '')) || isNaN(parseFloat(betting.maxPerMatch))) {
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

    var checkboxIndex = 1;
    for (var betting of bettings) {
        var curValMinBet = parseFloat(betting.minBetFormated.replace(/,/g, ''));
        var curValMaxBet = parseFloat(betting.maxBetFormated.replace(/,/g, ''));

        var curValMaxPerMatch = parseFloat(betting.maxPerMatchFormated.replace(/,/g, ''));
        var curValMaxPerMatchMax = betting.maxPerMatchMax;

        var curValMinMin = betting.minBetMin;
        var curValMaxMax = betting.maxBetMin;
        var bettingIsChecked = checkboxes[checkboxIndex].checked;

        ++checkboxIndex;

        if (bettingIsChecked && !this.IsValid(curValMinBet, curValMaxBet, curValMaxPerMatch, curValMinMin, curValMaxMax, curValMaxPerMatchMax, betting)) {
            return;
        }
    }

    top.Fanex.ExtAcc.Form.ConfirmBox("Confirmation", "Are you sure you want to edit Bet Setting to Destination Users?", 470, 130, false,
        function () {
            top.Fanex.ExtAcc.Form.Close();
            top.Fanex.ExtAcc.BlockUI();

            var para = "00";
            var startAtBasketball = false;
            for (var bettingLimitItem of model.bettingLimitItems) {
                if ($("#All").is(":checked") || model.bettingLimitItems[0].checked) {
                    para += "1";
                }
                else {
                    if (bettingLimitItem.resourceKey === "Basketball") {
                        startAtBasketball = true;
                    }

                    if (startAtBasketball) {
                        if (model.bettingLimitItems[3].checked) {
                            para += "1";
                        }
                        else {
                            para += bettingLimitItem.bgClass ? "0" : "1";
                        }
                    }
                    else {
                        para += bettingLimitItem.bgClass ? "0" : "1";
                    }
                }
            }

            top.$('#errMsg').empty().attr('class', 'succMsg').hide();

            Fanex.CustomerSetting.Betting.MultipleSportsbook.ProcessUpdateSuper(isCopy, isCopy ? 1 : 0, 0, JSON.stringify(model), para);
        });
};