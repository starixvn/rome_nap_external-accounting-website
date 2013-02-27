var validateExpr = /^(\$|)([0-9]\d{0,2}(\,\d{3})*|([0-9]\d*))(\.?\d*)?$/;
Fanex.Customer = {
    SetNextCustomer: function (selectOne, selectTwo, selectThree, currentValue) {
        if (selectOne == null || selectTwo == null) return;
        selectOne.options.length = 0;
        selectTwo.options.length = 0;
        var arrayAlphabet = new Array("0", "1", "2", "3", "4", "5", "6", "7", "8", "9",
                               "A", "B", "C", "D", "E", "F", "G", "H", "I", "J",
                               "K", "L", "M", "N", "O", "P", "Q", "R", "S", "T",
                               "U", "V", "W", "X", "Y", "Z");

        var noOne = currentValue.substring(0, 1);
        var noTwo = currentValue.substring(1, 2);
        var noThree = '';
        var index = 0;

        // If is member
        if (typeof (selectThree) != 'undefined' && selectThree != null) {
            selectThree.options.length = 0;
            noThree = currentValue.substring(2);
        }

        for (var i = 0; i < arrayAlphabet.length; i++) {
            var selectedOptOne = document.createElement("option");
            selectedOptOne.text = selectedOptOne.value = arrayAlphabet[i];

            var selectedOptTwo = document.createElement("option");
            selectedOptTwo.text = selectedOptTwo.value = arrayAlphabet[i];

            if (noTwo == arrayAlphabet[i]) {
                selectedOptTwo.selected = true;
            }
            if (noOne == arrayAlphabet[i]) {
                selectedOptOne.selected = true;
            }
            selectOne.options.add(selectedOptOne);
            selectTwo.options.add(selectedOptTwo);

            // If is member
            if (typeof (selectThree) != 'undefined' && selectThree != null) {
                var selectedOptThree = document.createElement("option");
                selectedOptThree.text = selectedOptThree.value = arrayAlphabet[i];
                if (noThree == arrayAlphabet[i]) {
                    selectedOptThree.selected = true;
                }
                selectThree.options.add(selectedOptThree);
            }
        }
    },

    IndexOf: function (array, searchString) {
        var found = -1;
        for (var i = 0; i < array.length; i++) {
            if (array[i] == searchString) {
                found = i;
                break;
            }
        }
        return found;
    },

    ValidateMessage: function (elementInput, errorMessage, xPos, pointerXPos, isAnimate) {
        // Alert
        Validators.show(elementInput, errorMessage, xPos, pointerXPos);
        if (!isAnimate) {
            Validators.animate(elementInput);
        }
    },

    // Check pass call in keyup event
    CheckPassword: function (pwdElement, iconElementId) {
        if (pwdElement == null) return true;
        var isPwd = IsPassword(pwdElement.value);
        var iconElement = $(iconElementId);

        if (!iconElement) return false;

        if (isPwd) {
            Validators.hide();
            iconElement.className = 'PwdSuccess';
        }
        else {
            this.ValidateMessage($("password"), Language.ALERT_PASSWORD_REQUIRE, -315, 320, true);
            iconElement.className = 'PwdError';
        }

        return isPwd;
    },

    // Check values input: Password and Max Credit
    CheckInfoValue: function () {
        if (!this.CheckPassword($("password"), "spnNewPwdIcon")) {
            return false;
        }

        if ($("credit").value == "") {
            this.ValidateMessage($("credit"), Language.ALERT_INCORRECT_MASTER_CREDIT);
            return false;
        }

        if ($("credit") && validateExpr.test($("credit").value) == false) {
            this.ValidateMessage($("credit"), Language.ALERT_CREDIT_INCORRECT);
            return false;
        }

        if ((parseFloat($("credit").value, true) < parseFloat($("minCredit").innerHTML, true))
			|| (parseFloat($("credit").value, true) > parseFloat($("maxCredit").innerHTML, true))) {
            var valMin = $("minCredit") ? $("minCredit").innerHTML : 0;
            var str = Language.ALERT_CREDIT_INVALID.replace("{1}", valMin);
            str = str.replace("{2}", $("maxCredit").innerHTML);
            this.ValidateMessage($("credit"), str);
            return false;
        }

        if ($("memberMaxCredit")) {
            if (validateExpr.test($("memberMaxCredit").value) == false) {
                this.ValidateMessage($("memberMaxCredit"), Language.ALERT_INCORRECT_MEMBER_MAX_CREDIT);
                return false;
            }

            if (parseFloat($("memberMaxCredit").value, true) > 0
				&& (parseFloat($("memberMaxCredit").value, true) > parseFloat($("lblMemberMaxCredit").innerHTML, true))
				|| parseFloat($("memberMaxCredit").value, true) < parseFloat($("lblMinCredit").innerHTML, true)) {
                var str = Language.ALERT_MEMBER_CREDIT_INVALID.replace("{1}", $("lblMinCredit") ? $("lblMinCredit").innerHTML : 0);
                str = str.replace("{2}", $("lblMemberMaxCredit").innerHTML);
                this.ValidateMessage($("memberMaxCredit"), str);
                return false;
            }
        }
        // Only show when add new 20 Member within 1 minute
        if ($("textBoxCaptcha") && $("textBoxCaptcha").value == "") {
            this.ValidateMessage($("textBoxCaptcha"), Language.REQUIRE_VALIDATION_CODE);
            return false;
        }

        if (!this.ValidationTransferCondition())
            return false;

        return true;
    },

    ValidationTransferCondition: function () {
        var transferConditionId = document.getElementsByName("transferCondition");
        if (transferConditionId != 'undefined' && transferConditionId.length > 0) {
            if ($("rdWeekly").checked) {
                var elements = $("alldays").getElementsByTagName("input");
                var isSelect = false;
                for (var i = 1; i < elements.length; i++) {
                    if (elements[i].checked) {
                        isSelect = true;
                        break;
                    }
                }
                if (!isSelect) {
                    this.ValidateMessage($("chkMon"), Language.ALERT_CHOOSE_ONE_DAY);
                    return false;
                }
            }
        }
        return true;
    },
    ChooseDate: function () {
        var chkCount = 0;
        if ($("rdWeekly").checked) {
            var elements = $("alldays").getElementsByTagName("input");
            for (var i = 1; i < elements.length; i++) {
                if (elements[i].checked) {
                    isSelect = true;
                    chkCount++;
                    if (chkCount == 7) {
                        this.ValidateMessage($("rdDaily"), Language.ALERT_CHOOSE_DAILY);
                        return false;
                    }
                }
            }
        }
        return true;
    },

    // This function is used to disable day checkboxs to edit transfer time
    SelectTransferTime: function () {
        if ($("isTransferOption") == "0" || !$("alldays")) return;
        var elements = $("alldays").getElementsByTagName("input");
        var isDaily = $("rdDaily").checked;
        for (var i = 1; i < elements.length; i++) {
            elements[i].disabled = isDaily;
        }
    },

    // Set Label value for min Credit Member
    AutoChangeMaxCredit: function (currentId, id) {
        if ($(id)) {
            if ($(currentId).value == '') {
                $(id).innerHTML = 0;
                $(currentId).value = 0;
            }
            else {
                // If credit number have a dot at last. We need remove that dot.
                var dotPosition = $(currentId).value.indexOf(".");
                if (dotPosition > 0 && ($(currentId).value.length - 1 === dotPosition)) {
                    $(id).innerHTML = $(currentId).value = $(currentId).value.substring(0, dotPosition);
                }
                else {
                    $(id).innerHTML = $(currentId).value;
                }
            }
        }
        else {
            // If credit number have a dot at last. We need remove that dot.
            var dotPosition = $(currentId).value.indexOf(".");
            if (dotPosition > 0 && ($(currentId).value.length - 1 === dotPosition)) {
                $(currentId).value = $(currentId).value.substring(0, dotPosition);
            }
        }
    },

    //Check Daily or Weekly
    DisplayTransferCondition: function (transferOption) {
        if (transferOption == "00000000") {
            return true;
        }
        else {
            return false;
        }
    },

    //Set current value for TransferOption
    TransferOption: function (transferOption) {
        var str = transferOption;
        var transferConditionId = document.getElementsByName("transferCondition");

        if (transferConditionId != 'undefined' && transferConditionId.length > 0) {
            if (transferConditionId[1].checked) {
                str = "";
                var arrWeekly = new Array("chkSun", "chkMon", "chkTue", "chkWed", "chkThurs", "chkFri", "chkSat");
                for (var i = 0; i < arrWeekly.length; i++) {
                    if (document.getElementById(arrWeekly[i]).checked) {
                        str += "1";
                    }
                    else {
                        str += "0";
                    }
                }
                str += "1";
            }
        }
        return str;
    },

    //Set next to userName
    SetAutoUserName: function () {
        var selectedOne = $("selectedOne");
        var selectedTwo = $("selectedTwo");
        var selectedThree = $("selectedThree");

        if (typeof (selectedThree) != 'undefined' && selectedThree != null) {
            return selectedOne.value + selectedTwo.value + selectedThree.value;
        }
        else if (typeof (selectedOne) != 'undefined' && typeof (selectedTwo) != 'undefined') {
            return selectedOne.value + selectedTwo.value;
        }
        return "";
    },

    // Set new value commission to Object JSON
    SetValueCommissionSettings: function (commissionSettings) {
        commissionSettings.groupA = $("groupA").value;
        commissionSettings.groupB = $("groupB").value;
        commissionSettings.groupC = $("groupC").value;
        commissionSettings.groupD = $("groupD").value;
        commissionSettings.discount1x2 = $("discount1x2").value;
        commissionSettings.discountCS = $("discountCS").value;
        commissionSettings.discountNumber = $("discountNumber").value;
        return commissionSettings;
    },

    // Hide Title on top page, when open Popup.
    HideTitle: function () {
        if ((parent.ageWnd != null) && (typeof ($('header_main')) != 'undefined')) {
            $('header_main').style.display = 'none';
        }
    },

    // Check available username
    CheckUser: function (username, selectId, select, select2, select3, msgId) {
        if ($(selectId)) {
            $(selectId).onchange = function () {
                $(msgId).className = 'loading2';

                var user = username;

                if (select && select2 && select3) {
                    user += $(select).value + $(select2).value + $(select3).value;
                }
                else if (select && select2) {
                    user += $(select).value + $(select2).value;
                }

                ajax.PostJSON(
                    '../../SearchAndCopy/Handlers/CheckUser.ashx?username=' + user,
                    '',
                    function (result) {
                        if (result.errCode == 101) {
                            $(msgId).className = 'successWarning';
                        }
                        else {
                            $(msgId).className = 'errorWarning';
                        }
                    }
                );
            }
        }
    },

    GenerateMessage: function(ArrCodes, ArrMsgs, ArrUserName, isAdd) {
        var isSucc = ageMsg.CheckMessageIsSucc2(ArrCodes);

        function getClass(errCode) {
            var className = '';

            if (errCode == 9999) {
                return className;
            }

            if (errCode < -1) {
                className = 'EnrichSuccmsg';
            }
            else if (errCode >= 50 && errCode <= 99) {
                className = 'EnrichWarmsg';
            }
            else {
                className = 'EnrichErrmsg';
            }

            return className;
        }

        function InsertRow(tableId, classname, message) {
            var tr = document.createElement('tr');
            var td = document.createElement('td');
            td.className = classname;
            td.innerHTML = message;
            tr.appendChild(td);
            var tbody = document.getElementById(tableId).getElementsByTagName('tbody')[0];
            tbody.appendChild(tr);
        }

        if (isAdd) // Add
        {
            for (var j = 0; j < ArrUserName.length; j++) {
                InsertRow('tblMessage', 'EnrichUserName', ArrUserName[j]);
                for (var i = 0; i < ArrCodes.length; i++) {
                    InsertRow('tblMessage', getClass(ArrCodes[i]), ArrMsgs[i]);
                }
            }
        }
        else // Reset table
        {
            var myTable = '<table id="tblMessage" cellpadding="0" cellspacing="0" border="0" style="width:100%;"><tbody>';
            var myEndTable = '</tbody></table>';

            var tableBodyHTML = '';
            for (var j = 0; j < ArrUserName.length; j++) {
                tableBodyHTML += '<tr><td class="EnrichUserName">';
                tableBodyHTML += ArrUserName[j];
                tableBodyHTML += "</td></tr>";

                for (var i = 0; i < ArrCodes.length; i++) {
                    tableBodyHTML += '<tr><td class="' + getClass(ArrCodes[i]) + '">' + ArrMsgs[i] + "</td></tr>";
                }
            }
            myTable += tableBodyHTML + myEndTable;

            ageMsg.ShowMsgs(myTable, isSucc);
        }
    }
}

// Overwrite some functions
if (typeof (Fanex) != 'undefined' && typeof (Fanex.PTEngine) != 'undefined') { // Require Fanex.PTEngine namespace
    Fanex.PTEngine.calculateMinWidth = function (text) {
        if (typeof (text) != 'string') return 60;
        var measure = 6; // Default for en-US
        if (Fanex.PTEngine.CURRENT_CONTEXT.language == 'th-TH') {
            measure = 4.5;
        }
        else if (Fanex.PTEngine.CURRENT_CONTEXT.language == 'zh-TW'
			|| Fanex.PTEngine.CURRENT_CONTEXT.language == 'zh-CN'
			|| Fanex.PTEngine.CURRENT_CONTEXT.language == 'ko-KR') {
            measure = 7;
        }
        return Math.round(text.length * measure);
    };
}

//RegisterStartUp(function () {
//    age.addEvent($('credit'), 'blur', function () {
//        Fanex.Customer.AutoChangeMaxCredit('credit', 'lblMemberMaxCredit');
//    });

//    age.addEvent($('credit'), 'keyup', function () {
//        Fanex.FormatNumber(arguments[0], true);
//    });

//    age.addEvent($('memberMaxCredit'), 'keyup', function () {
//        Fanex.FormatNumber(arguments[0], true);
//    });
//});