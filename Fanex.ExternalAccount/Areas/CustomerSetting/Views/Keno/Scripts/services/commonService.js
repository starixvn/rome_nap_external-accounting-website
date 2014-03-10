"use strict";

KenoModule.service('commonService', function () {
    var formType = {
        editSuper: 0,
        editMaster: 1,
        editAgent: 2,
        superEditMember: 3,
        masterEditMember: 4,
        agentEditMember: 5
    };

    var role = {
        super: 0,
        master: 1,
        agent: 2,
        member: 3
    };

    this.getRangeValue = function (max, min, stepValue, roundValue) {
        var rangeValueArray = new Array();
        while (max >= min) {
            rangeValueArray.push(max);
            max = Math.round((max - stepValue) * roundValue) / roundValue;
        }
        return rangeValueArray;
    };

    this.regIsDigit = function (input) {
        var reg = new RegExp("^[0-9,]+$");
        return (reg.test(input));
    };

    this.formatInputNumber = function (input) {
        return parseInt(input.toString().replace(/,/gi, ''));
    };

    this.reformatInputNumberForControl = function (input) {
        input = input.toString().replace(/,/gi, '');
        var result = "";

        if (input.length > 3) {
            for (var i = input.length - 3; i > 0; i = i - 3) {
                var end = i + 3;
                result = ',' + input.substring(i, end) + result;
            }

            result = input.substring(0, i + 3) + result;
        }
        else {
            result = input;
        }

        return result;
    };

    this.composeChangedPositiongTakingSections = function (positionTakings, commissionSettings) {
        var changedData = [];
        for (var i = 0; i < positionTakings.length; i++) {
            changedData.push({
                PositionTaking: positionTakings[i],
                CommissionSetting: commissionSettings[i]
            });
        }

        return changedData;
    };

    this.validateNumber = function (event) {
        // Prevent shift key since its not needed
        if (event.shiftKey == true) {
            event.preventDefault();
        }
        // Allow Only: keyboard 0-9, numpad 0-9, backspace, tab, left arrow, right arrow, delete
        if ((event.keyCode >= 48 && event.keyCode <= 57) || (event.keyCode >= 96 && event.keyCode <= 105) || event.keyCode == 8 || event.keyCode == 188 || event.keyCode == 190 || event.keyCode == 9 || event.keyCode == 37 || event.keyCode == 39 || event.keyCode == 46) {
            // Allow normal operation
        } else {
            // Prevent the rest
            event.preventDefault();
        }
    };

    this.blockUI = function () {

        if (parent.BlockUI) {
            try { parent.BlockUI(); } catch (e) { }
        }
        //Called inside a frame set (#centerFr)
        else {
            if (parent.parent.BlockUI) {
                parent.parent.BlockUI();
            }
        }
        //Called from popup
        if (opener) {
            if (opener.BlockUI) {
                opener.BlockUI();
            }
            opener.parent.BlockUI();
        }
    };

    this.unblockUI = function () {

        if (parent.jQuery && parent.jQuery.unblockUI) {
            parent.jQuery.unblockUI();
        }
        //Called inside a frame set (#centerFr)
        if (parent.parent.jQuery && parent.parent.jQuery.unblockUI) {

            parent.parent.jQuery.unblockUI();
        }
        //Called from popup

        if (opener) {
            if (opener.parent.jQuery.unblockUI) {
                opener.parent.jQuery.unblockUI();
            }
            //parent.jQuery.unblockUI();
        }
    };
});