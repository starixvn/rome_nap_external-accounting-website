"use strict";

ColossusBetModule.service('commonService', function () {
    var getRangeValue = function (max, min, stepValue, roundValue) {
        var rangeValueArray = new Array();
        if (max >= min) {
            while (max >= min) {
                rangeValueArray.push(max);
                max = Math.round((max - stepValue) * roundValue) / roundValue;
            }
        } else {
            rangeValueArray = [0];
        }
        return rangeValueArray;
    }

    this.roundPTValue = function (value) {
        return Math.round(value * 100) / 100;
    }

    this.getRangePTValue = function (max, min) {
        var ptValue = {
            stepValue: 0.01,
            roundValue: 100
        };

        return getRangeValue(max, min, ptValue.stepValue, ptValue.roundValue);
    };

    this.getRangeCommValue = function (max, min) {
        var commValues = {
            stepValue: 0.0005,
            roundValue: 10000
        };

        return getRangeValue(max, min, commValues.stepValue, commValues.roundValue);
    };

    this.createTooltip = function (element, titleTootip) {
        element.attr('title', titleTootip);
        element.attr('original-title', titleTootip);
        element.tipsy({
            delayIn: 0,
            delayOut: 0,
            gravity: "s",
            opacity: 1,
            autoHide: {
                isAuto: true,
                timeOut: 7
            }
        });
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
            try {
                parent.BlockUI();
            } catch (e) { }
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