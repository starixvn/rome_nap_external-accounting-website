"use strict";

KenoModule.service('dataService', function () {
    var roleId = 0;
    var createCustomerInfo = function (id) {
        roleId = id;
        return {
            "custId": 122222,
            "roleId": id,
            "firstName": "Vu",
            "lastName": "Tran",
            "phone": "---",
            "mobilePhone": "---",
            "fax": "---",
            "username": "Super001"
        };
    };

    var createBetSetting = function (betTypeId, betValue, isEnabled, isDownlinesEnabled) {
        return {
            "betTypeId": betTypeId,
            "betTypeName": "Beijing" + betTypeId,
            "minBet": betValue,
            "maxBet": betValue * 10,
            "maxPerMatch": betValue * 100,
            "isEnabled": isEnabled,
            "isBetSettingUpdated": false,
            "isDownlinesEnabled": isDownlinesEnabled,
            "isStatusUpdated": false,
            "isValid": true,
            "upline": {
                "minBet": betValue - 1,
                "maxBet": roleId == 4 ? -1 : (betValue + 1) * 10,
                "maxPerMatch": roleId == 4 ? -1 : (betValue + 1) * 100,
                "isEnabled": isEnabled,
                "isDownlinesEnabled": isDownlinesEnabled
            }
        };
    };

    var createBetSettingList = function () {
        var betSettings = [];

        for (var i = 6; i > 1; i--) {
            var isEnable = false;
            var isDownlineEnable = false;
            switch (i) {
                case 6:
                    isEnable = false;
                    isDownlineEnable = false;
                    break;
                case 5:
                    isEnable = false;
                    isDownlineEnable = false;
                    break;
                case 4:
                    isEnable = true;
                    isDownlineEnable = true;
                    break;
                case 3:
                    isEnable = true;
                    isDownlineEnable = false;
                    break;
                case 2:
                    isEnable = true;
                    isDownlineEnable = false;
                    break;
            }
            var betSettingTmp = createBetSetting(i, 100 * i, isEnable, isDownlineEnable);
            betSettings.push(betSettingTmp);
        }

        return betSettings;
    };


    var createPositionTaking = function (betTypeId, boolValue1, boolValue2, betTypeName, roleId) {
        var positionTaking = {
            "sportTypeId": 202,
            "betTypeId": betTypeId,
            "betTypeName": betTypeName,
            "s4": 0.9,
            "m4": 0.6,
            "a4": 0.4,
            "n4": 0.1,
            "l4": 0.9,
            "sComm": 0.0095,
            "sCommMax": 0.01,
            "s3": 0.6,
            "m3": 0.6,
            "uS3": 0.5,
            "iS3": boolValue1,
            "n3": 0.1,
            "l3": 0.8,
            "mComm": 0.0075,
            "m2": 0.2,
            "a2": 0.4,
            "uM2": 0.45,
            "iM2": boolValue1,
            "n2": 0.1,
            "l2": 0.6,
            "aComm": 0.0045,
            "a1": 0.2,
            "comm": 0.0035,
            "s1": 0.2,
            "uS1": 0.4,
            "iS1": boolValue2,
            "m1": 0.7,
            "uM1": 0.2,
            "iM1": boolValue2
        };

        switch (roleId) {
            case 4:
                positionTaking.isUpdatePT = false;
                positionTaking.isUpdateS4 = false;
                positionTaking.isUpdateN4 = false;
                positionTaking.isUpdateSCo = false;
                break;
            case 3:
                positionTaking.isUpdatePT = false;
                positionTaking.isUpdateS3 = false;
                positionTaking.isUpdateUS3 = false;
                positionTaking.isUpdateN3 = false;
                positionTaking.isUpdateMComm = false;
                positionTaking.isUpdateAllMember = false;
                positionTaking.newMemberPT = 0;
                positionTaking.isConfirmIsAuto = false;
                break;
            case 2:
                positionTaking.isDisabledMPT = false;
                positionTaking.isUpdatePT = false;
                positionTaking.isUpdateM2 = false;
                positionTaking.isUpdateUM2 = false;
                positionTaking.isUpdateN2 = false;
                positionTaking.isUpdateACO = false;
                positionTaking.isUpdateAllMember = false;
                positionTaking.newMemberPT = 0;
                positionTaking.isConfirmIsAuto = false;
                break;
            case 1:
                positionTaking.isUpdatePT = false;
                positionTaking.isUpdateCO = false;
                break;
            case 41:
                positionTaking.isUpdatePT = false;
                positionTaking.isUpdateS1 = false;
                positionTaking.isUpdateUS1 = false;
                positionTaking.s1max = 0.9;
                positionTaking.s1us1min = 0.1;
                break;
            case 31:
                positionTaking.isUpdatePT = false;
                positionTaking.isUpdateM1 = false;
                positionTaking.isUpdateUM1 = false;
                positionTaking.m1max = 0.5;
                positionTaking.m1um1min = 0.1;
                break;
            default:
                break;
        };

        return positionTaking;
    };

    var createPositionTakingList = function (roleId) {
        var positionTakings = [];

        for (var i = 1; i < 6; i++) {
            positionTakings.push(createPositionTaking(i, true, true, "bettypeName" + i, roleId));
        }

        return positionTakings;
    };

    this.getKenoSetting = function (roleId) {
        var kenoSetting = {};
        kenoSetting.customerInformation = createCustomerInfo(roleId);
        kenoSetting.betSettings = createBetSettingList();
        kenoSetting.positionTakings = createPositionTakingList(roleId);
        kenoSetting.betSettingCopyAll = { "betTypeId": -1, "isCopiedAll": false };
        kenoSetting.positionTakingCopyAll = { "betTypeId": -1, "isCopiedAll": false };
        return kenoSetting;
    };
});