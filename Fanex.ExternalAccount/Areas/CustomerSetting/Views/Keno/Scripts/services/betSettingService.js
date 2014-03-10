"use strict";

KenoModule.service('betSettingService', function (commonService) {
    this.getChangedData = function (betSettings) {
        var changedData = [];

        for (var i = 0; i < betSettings.length; i++) {
            if (betSettings[i].actions.isDataChanged()) {
                changedData.push({
                    MinBet: betSettings[i].minBet,
                    MaxBet: betSettings[i].maxBet,
                    MaxPerMatch: betSettings[i].maxPerMatch,
                    IsEnabled: betSettings[i].isEnabled,
                    IsDownlinesEnabled: betSettings[i].isDownlinesEnabled
                });
            }
        }

        return changedData;
    };

    this.isValidData = function (betSettings) {
        for (var i = 0; i < betSettings.length; i++) {
            var betSetting = betSettings[i];
            if (!betSetting.actions.isValid()) {
                return false;
            }
        }

        return true;
    };

    this.saveOriginalData = function (betSetting) {
        betSetting.originalMinBet = betSetting.minBet;
        betSetting.originalMaxBet = betSetting.maxBet;
        betSetting.originalMaxPerMatch = betSetting.maxPerMatch;
        betSetting.originalIsEnabled = betSetting.isEnabled;
        betSetting.originalIsDownlinesEnabled = betSetting.isDownlinesEnabled;
    };

    this.setAction = function (betSetting, formType) {
        betSetting.actions = this.actions(betSetting);
    };

    this.actions = function (betSetting) {
        return {
            isValid: function () {
                return this.validMinBet() && this.validMaxBet() && this.validMaxPerMatch();
            },

            validMinBet: function () {
                var message = "";

                if (!commonService.regIsDigit(betSetting.minBet)) {
                    message = "bettingLimit_Message_OnlyNumber";
                }
                else if (commonService.formatInputNumber(betSetting.minBet) > commonService.formatInputNumber(betSetting.maxBet)) {
                    message = "bettingLimit_Message_MinBetLessThanOrEqualMaxBet";
                }
                else if (betSetting.upline.minBet > 0 && commonService.formatInputNumber(betSetting.minBet) < commonService.formatInputNumber(betSetting.upline.minBet)) {
                    message = "bettingLimit_Message_MinBetMoreThanOrEqualTo" + commonService.reformatInputNumberForControl(betSetting.upline.minBet);
                }

                if (message != "") {
                    alert(message);
                    //fanex.customerSetting.betLimitService.showInvalidMessage(inputControl[0], message);
                    return false;
                }
                else {
                    //fanex.hideHint();
                    return true;
                }
            },

            validMaxBet: function () {
                var message = "";

                if (!commonService.regIsDigit(betSetting.maxBet)) {
                    message = "bettingLimit_Message_OnlyNumber";
                }
                else if (commonService.formatInputNumber(betSetting.minBet) > commonService.formatInputNumber(betSetting.maxBet)) {
                    message = "bettingLimit_Message_MaxBetMoreThanOrEqualMinBet";
                }
                else if (commonService.formatInputNumber(betSetting.maxBet) > commonService.formatInputNumber(betSetting.maxPerMatch)) {
                    message = "bettingLimit_Message_MaxBetLessThanOrEqualMaxPerMatch";
                }
                else if (betSetting.upline.maxBet > 0 && commonService.formatInputNumber(betSetting.maxBet) > commonService.formatInputNumber(betSetting.upline.maxBet)) {
                    message = "bettingLimit_Message_MaxBetLessThanOrEqualTo" + commonService.reformatInputNumberForControl(betSetting.upline.maxBet);
                }

                if (message != "") {
                    alert(message);
                    //fanex.customerSetting.betLimitService.showInvalidMessage(inputControl[1], message);
                    return false;
                }
                else {
                    //fanex.hideHint();
                    return true;
                }
            },

            validMaxPerMatch: function () {
                var message = "";

                if (!commonService.regIsDigit(betSetting.maxPerMatch)) {
                    message = "bettingLimit_Message_OnlyNumber";
                }
                else if (commonService.formatInputNumber(betSetting.maxBet) > commonService.formatInputNumber(betSetting.maxPerMatch)) {
                    message = "bettingLimit_Message_MaxPerMatchMoreThanOrEqualMaxBet";
                }
                else if (betSetting.upline.maxPerMatch > 0 && commonService.formatInputNumber(betSetting.maxPerMatch) > commonService.formatInputNumber(betSetting.upline.maxPerMatch)) {
                    message = "bettingLimit_Message_MaxPerMatchLessThanOrEqualTo" + commonService.reformatInputNumberForControl(betSetting.upline.maxPerMatch);
                }

                if (message != "") {
                    alert(message);
                    //fanex.customerSetting.betLimitService.showInvalidMessage(inputControl[2], message);
                    return false;
                }
                else {
                    //fanex.hideHint();
                    return true;
                }
            },

            changeMinBet: function () {
                this.validMinBet();
            },

            changeMaxBet: function () {
                this.validMaxBet();
            },

            changeMaxPerMatch: function () {
                this.validMaxPerMatch();
            },

            isDataChanged: function () {
                return betSetting.minBet != betSetting.originalMinBet
                        || betSetting.maxBet != betSetting.originalMaxBet
                        || betSetting.maxPerMatch != betSetting.originalMaxPerMatch
                        || betSetting.isEnabled != betSetting.originalIsEnabled
                        || betSetting.isDownlinesEnabled != betSetting.originalIsDownlinesEnabled;
            },

            copyToAll: function (betSettings) {
                if (!this.isValid()) {
                    return;
                }

                for (var i = 0; i < betSettings.length; i++) {
                    betSettings[i].minBet = betSetting.minBet;
                    betSettings[i].maxBet = betSetting.maxBet;
                    betSettings[i].maxPerMatch = betSetting.maxPerMatch;
                    betSettings[i].isEnabled = betSetting.isEnabled;
                    betSettings[i].isDownlinesEnabled = betSetting.isDownlinesEnabled;
                }
            }
        };
    };

});