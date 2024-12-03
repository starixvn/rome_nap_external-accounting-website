Fanex.ExtAcc.AddPtInfoPage(custid);

if (typeof Fanex == 'undefined' || typeof Fanex.ExtAcc == 'undefined') {
    alert("Fanex.ExtAcc.js module is required!");
}
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

    // Password must be at least 6 in length and at least 2 characters and 2 digits
    IsPassword: function (str, isMember) {
        return Fanex.ExtAcc.Customer.validatePassword(str, isMember);
    },

    ShowTooltip: function (elementInput, message, xPos, pointerXPos, yPos, pointerYPos, height, timeout) {
        // <span class="hint"><span>This is the message.<span><span class="hint-pointer">&nbsp;</span></span>
        var hint = document.getElementById('fHintDown');
        var hintPointer = null;
        if (hint == null) {
            var hint = document.createElement("SPAN");
            hint.className = 'hint';
            var hintMsg = document.createElement("SPAN");
            hint.appendChild(hintMsg);
            hintPointer = document.createElement("SPAN");
            hintPointer.className = 'hint-pointerDown';
            hint.appendChild(hintPointer);
            hint.id = 'fHintDown';
            hint.style.top = '10px';
            hint.style.left = '10px';
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
            timeout = 10000;
        }
        this.timeOutHideTooltip = setTimeout(function () {
            if (hint == null) {
                return;
            }
            hint.style.display = 'none';
        }, timeout); // Auto hide in 10s
        if (elementInput != null) {
            if (typeof xPos == 'undefined') {
                xPos = 0;
            }
            if (typeof pointerXPos == 'undefined') {
                pointerXPos = 10;
            }
            if (typeof yPos == 'undefined') {
                yPos = -45;
            }
            if (typeof (pointerYPos) == 'undefined') {
                pointerYPos = 38;
            }
            if (typeof (height) == 'undefined') {
                height = 18;
            }
            hint.style.height = height + 'px';
            hintPointer.style.left = (pointerXPos + 'px');
            hintPointer.style.top = (pointerYPos + 'px');
            var pos = Validators.utils.findPos(elementInput);
            hint.style.left = (pos[0] + xPos) + 'px';
            hint.style.top = (pos[1] + yPos) + 'px';
            elementInput.onchange = function () {
                hint.style.display = 'none';
            };
            elementInput.focus();
        };
        hint.style.display = 'block';
    },
    ValidateMessage: function (elementInput, errorMessage, xPos, pointerXPos, isAnimate, height, direction, timeout) {
        // Alert
        if (direction == 'down') {
            this.ShowTooltip(elementInput, errorMessage, xPos, pointerXPos, 30, -10, height, timeout);
        }
        else {
            Validators.show(elementInput, errorMessage, xPos, pointerXPos, undefined, undefined, height, timeout);
        }
        if (!isAnimate) {
            Validators.animate(elementInput);
        }
    },
    // Check pass call in keyup event
    CheckPassword: function (pwdElement, iconElementId, isMember) {
        if (pwdElement == null) return true;
        var isPwd = this.IsPassword(pwdElement.value, isMember);
        var iconElement = document.getElementById(iconElementId);
        if (!iconElement) return false;
        if (isPwd) {
            document.getElementById('fHintDown').style.display = 'none';
            iconElement.className = 'PwdSuccess';
        }
        else {
            if (isMember) {
                var msg = "<span class='prewrap'>Password must be 6-10 characters without white-space and contain at least: <br/><br/> &nbsp;&nbsp;&nbsp;&nbsp;• 1 uppercase letter <br/> &nbsp;&nbsp;&nbsp;&nbsp;• 1 lowercase letter <br/> &nbsp;&nbsp;&nbsp;&nbsp;• 1 numeric digit <br/><br/>For example: 493abcDE, 59D7!4$h<br/>Notes: your password is cAsE sEnSiTiVe</span>";
                this.ValidateMessage($("#password").get(0), msg, -215, 220, true, 140, 'down', 8000);
            }
            else {
                var msg = "<span class='prewrap'>Password must be at least 8 characters without white-space and contain at least 3 of the following:<br/><br/> &nbsp;&nbsp;&nbsp;&nbsp;• Uppercase letter [A-Z] <br/> &nbsp;&nbsp;&nbsp;&nbsp;• Lowercase letter [a-z] <br/> &nbsp;&nbsp;&nbsp;&nbsp;• Numeric [0-9] <br/> &nbsp;&nbsp;&nbsp;&nbsp;• Special character (!,@,#,etc..)<br/><br/>For example: 59D7!4$h, 493abcDE<br/>Notes: your password is cAsE sEnSiTiVe</span>";
                this.ValidateMessage($("#password").get(0), msg, -255, 260, true, 160, 'down', 8000);
            }

            iconElement.className = 'PwdError';
        }
        return isPwd;
    },

    // Check values input: Password and Max Credit
    CheckInfoValue: function (isMember) {
        if (!this.CheckPassword(document.getElementById("password"), "spnNewPwdIcon", isMember)) {
            return false;
        }

        if ($("#credit").val() == "") {
            this.ValidateMessage(document.getElementById("credit"), Language.ALERT_INCORRECT_MASTER_CREDIT);
            return false;
        }

        if (document.getElementById("credit") && validateExpr.test($("#credit").val()) == false) {
            this.ValidateMessage(document.getElementById("credit"), Language.ALERT_CREDIT_INCORRECT);
            return false;
        }

        if ((parseFloat($("#credit").val(), true) < parseFloat($("#minCredit").html(), true))
            || (parseFloat($("#credit").val(), true) > parseFloat($("maxCredit").html(), true))) {
            var valMin = document.getElementById("minCredit") ? $("#minCredit").html() : 0;
            var str = Language.ALERT_CREDIT_INVALID.replace("{1}", valMin);
            str = str.replace("{2}", $("#maxCredit").html());
            this.ValidateMessage(document.getElementById("credit"), str);
            return false;
        }

        if (document.getElementById('memberMaxCredit')) {
            if (validateExpr.test($("#memberMaxCredit").val()) == false) {
                this.ValidateMessage(document.getElementById("memberMaxCredit"), Language.ALERT_INCORRECT_MEMBER_MAX_CREDIT);
                return false;
            }

            if (parseFloat($("#memberMaxCredit").val(), true) > 0
                && (parseFloat($("#memberMaxCredit").val(), true) > parseFloat($("#lblMemberMaxCredit").html(), true))
                || parseFloat($("#memberMaxCredit").val(), true) < parseFloat($("#lblMinCredit").html(), true)) {
                var str = Language.ALERT_MEMBER_CREDIT_INVALID.replace("{1}", document.getElementById("lblMinCredit") ? $("#lblMinCredit").html() : 0);
                str = str.replace("{2}", $("#lblMemberMaxCredit").html());
                this.ValidateMessage(document.getElementById("memberMaxCredit"), str);
                return false;
            }
        }
        // Only show when add new 20 Member within 1 minute
        if (document.getElementById("textBoxCaptcha") && $("#textBoxCaptcha").val() == "") {
            this.ValidateMessage(document.getElementById("textBoxCaptcha"), Language.REQUIRE_VALIDATION_CODE);
            return false;
        }

        if (!this.ValidationTransferCondition())
            return false;

        return true;
    },

    ValidationTransferCondition: function () {
        var transferConditionId = document.getElementsByName("transferCondition");
        if (transferConditionId != 'undefined' && transferConditionId.length > 0) {
            if (document.getElementById("rdWeekly").checked) {
                var elements = document.getElementById("alldays").getElementsByTagName("input");
                var isSelect = false;
                for (var i = 1; i < elements.length; i++) {
                    if (elements[i].checked) {
                        isSelect = true;
                        break;
                    }
                }
                if (!isSelect) {
                    this.ValidateMessage(document.getElementById("chkMon"), Language.ALERT_CHOOSE_ONE_DAY);
                    return false;
                }
            }
        }
        return true;
    },
    ChooseDate: function () {
        var chkCount = 0;
        if (document.getElementById("rdWeekly").checked) {
            var elements = document.getElementById("alldays").getElementsByTagName("input");
            for (var i = 1; i < elements.length; i++) {
                if (elements[i].checked) {
                    isSelect = true;
                    chkCount++;
                    if (chkCount == 7) {
                        this.ValidateMessage(document.getElementById("rdDaily"), Language.ALERT_CHOOSE_DAILY);
                        return false;
                    }
                }
            }
        }
        return true;
    },

    // This function is used to disable day checkboxs to edit transfer time
    SelectTransferTime: function () {
        if (document.getElementById("isTransferOption") == "0" || !document.getElementById("alldays")) return;
        var elements = document.getElementById("alldays").getElementsByTagName("input");
        var isDaily = document.getElementById("rdDaily").checked;
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
        var selectedOne = document.getElementById("selectedOne");
        var selectedTwo = document.getElementById("selectedTwo");
        var selectedThree = document.getElementById("selectedThree");

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
        commissionSettings.groupA = $("#groupA").val();
        commissionSettings.groupB = $("#groupB").val();
        commissionSettings.groupC = $("#groupC").val();
        commissionSettings.groupD = $("#groupD").val();
        commissionSettings.discount1x2 = $("#discount1x2").val();
        commissionSettings.discountCS = $("#discountCS").val();
        return commissionSettings;
    },

    // Hide Title on top page, when open Popup.
    HideTitle: function () {
        if ((parent.ageWnd != null) && (typeof (document.getElementById('header_main')) != 'undefined')) {
            document.getElementById('header_main').style.display = 'none';
        }
    },

    // Check available username
    CheckUser: function (parentRoleId, username, selectId, select, select2, select3, msgId) {
        if (document.getElementById(selectId)) {
            document.getElementById(selectId).onchange = function () {
                document.getElementById(msgId).className = 'loading2';

                var user = username;

                if (select && select2 && select3) {
                    user += document.getElementById(select).value + document.getElementById(select2).value + document.getElementById(select3).value;
                }
                else if (select && select2) {
                    user += document.getElementById(select).value + document.getElementById(select2).value;
                }

                var data = { username: user };
                $.ajax({
                    url: resolveClientUrl('Customer/IsValidUsername'),
                    data: data,
                    dataType: 'json',
                    type: 'post'
                })
                    .success(function (result) {
                        if (result === true) {
                            document.getElementById(msgId).className = 'successWarning';
                        }
                        else {
                            document.getElementById(msgId).className = 'errorWarning';
                        }
                    });
            }
        }
    },

    GenerateMessage: function (ArrCodes, ArrMsgs, ArrUserName, isAdd) {
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
};

