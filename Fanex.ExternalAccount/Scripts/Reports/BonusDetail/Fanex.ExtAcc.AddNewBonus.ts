(function ($) {
    $.fn.inputFilter = function (inputFilter) {
        return this.on("input keydown keyup mousedown mouseup select contextmenu drop", function () {
            if (inputFilter(this.value)) {
                this.oldValue = this.value;
                this.oldSelectionStart = this.selectionStart;
                this.oldSelectionEnd = this.selectionEnd;
            } else if (this.hasOwnProperty("oldValue")) {
                this.value = this.oldValue;
                this.setSelectionRange(this.oldSelectionStart, this.oldSelectionEnd);
            } else {
                this.value = "";
            }
        });
    };
}(jQuery));

class AddNewBonus {
    private $: JQueryStatic;
    private callback: Function;
    private msgAddNewBonus: JQuery;
    private formAddNewBonus: JQuery;
    private txtMembers: JQuery;
    private txtAmount: JQuery;
    private groupName: string;
    private buttonSubmit: JQuery;
    private resources = {
        popupTitle: "Add New Bonus",
        usernamesRequiredError: "Member Username(s) is required.",
        usernamesNotAllowBackslashError: "You are not allowed to input backslash '\\' into the Username field.",
        usernamesTooManyError: "The function can only add new up to {0} usernames per request.",
        usernamesDuplicateError: "Duplicated usernames: {0}.",
        amountRequiredError: "Amount is required.",
        amountInvalidError: "Amount must be a positive number."
    };
    private maxUsernamesLength = 100;

    constructor($: JQueryStatic, callback: Function) {
        this.$ = $;
        this.callback = callback;
    }

    public RegisterAddNewBonus(button: JQuery, popupAddNewBonus: JQuery) {
        button.click(() => {
            Fanex.ExtAcc.Form.ConfirmBox(
                this.resources.popupTitle,
                popupAddNewBonus.html(),
                500,
                "auto",
                false,
                () => this.addNewBonusPost(),
                null,
                "addnew-bonus-modal",
                true,
                null,
                true);

            this.cacheElements();
            this.bindEvents();
        });
    }

    private cacheElements() {
        this.msgAddNewBonus = this.$("#div-msgAddNewBonus");
        this.formAddNewBonus = this.$("#form-addNewBonus");
        this.txtMembers = this.formAddNewBonus.find("#txt-MemberUsernames");
        this.txtAmount = this.formAddNewBonus.find("#txt-amount");
        this.groupName = this.$("#selectedGroupName").html();
        this.buttonSubmit = this.$("#addnew-bonus-modal").next(".ui-dialog-buttonpane").find(".btn.-submit");
    }

    private bindEvents() {
        this.formAddNewBonus.unbind("submit").bind("submit", function (e) { e.preventDefault(); });
        this.txtMembers
            .unbind("change")
            .bind("change", () => {
                let membersValue = this.$.trim(this.txtMembers.val());

                if (membersValue.length > 0) {
                    if (membersValue.indexOf("\n") >= 0) {
                        membersValue = membersValue.replace(/\n/g, " ");
                    }

                    if (membersValue.indexOf("\r") >= 0) {
                        membersValue = membersValue.replace(/\r/g, " ");
                    }

                    if (membersValue.indexOf("\t") >= 0) {
                        membersValue = membersValue.replace(/\t/g, " ");
                    }

                    membersValue = membersValue.replace(/\s/g, ",");
                    let usernamesArray = membersValue.split(",").filter(item => item.length > 0);

                    this.txtMembers.val(usernamesArray.join(","));
                }
            });
        this.txtAmount.inputFilter(function (value) {
            return /^-?\d*[.]?\d{0,2}$/.test(value);
        });
    }

    private addNewBonusPost() {
        if (this.isValidMembers() && this.isValidAmount()) {
            Fanex.ExtAcc.BlockUI();
            this.buttonSubmit.prop("disabled", true);
            var formData = this.formAddNewBonus.serializeArray();
            formData.push({ name: "groupName", value: this.$("#selectedGroupName").html() });
            this.$.post(
                resolveClientUrl("BonusDetail/AddNew"),
                formData,
                data => {
                    const isSuccess = data.isSuccess || data.IsSuccess;
                    if (!isSuccess) {
                        this.showError(data.errorMessage || data.ErrorMessage);
                        this.buttonSubmit.prop("disabled", false);
                    } else {
                        this.showSuccess(data.ErrorMessage);
                        setTimeout(() => {
                            this.callback();
                            Fanex.ExtAcc.Form.Close(null, null, "addnew-bonus-modal");
                        }, 1000 * 3);
                    }
                }
            ).always(Fanex.ExtAcc.UnBlockUI);
        }
    }

    private isValidMembers() {
        const membersValue = this.$.trim(this.txtMembers.val());
        if (membersValue.length === 0) {
            this.txtMembers.focus();
            this.showError(this.resources.usernamesRequiredError);
            return false;
        }

        // Check usernames contains '\'.
        if (membersValue.indexOf("\\") >= 0) {
            this.txtMembers.focus();
            this.showError(this.resources.usernamesNotAllowBackslashError);
            return false;
        }

        const membersArray = membersValue.split(",");
        if (membersArray.length > this.maxUsernamesLength) {
            this.txtMembers.focus();
            this.showError(this.resources.usernamesTooManyError.replace("{0}", this.maxUsernamesLength.toString()));
            return false;
        }

        // Check duplicate.
        const duplicatedMembers = membersArray.filter((item, index) => membersArray.indexOf(item) !== index);

        if (duplicatedMembers.length > 0) {
            this.txtMembers.focus();
            this.showError(this.resources.usernamesDuplicateError.replace("{0}", duplicatedMembers.join(", ")));
            return false;
        }

        this.hideMessage();
        return true;
    }

    private isValidAmount() {
        let amountValue = this.$.trim(this.txtAmount.val());

        if (amountValue.length === 0) {
            this.txtAmount.focus();
            this.showError(this.resources.amountRequiredError);
            return false;
        }

        amountValue = amountValue.replace(/\,/g, "");

        if (!this.isNumeric(amountValue)) {
            this.txtAmount.focus();
            this.showError(this.resources.amountInvalidError);
            return false;
        }

        var number = parseFloat(amountValue);

        if (number <= 0) {
            this.txtAmount.focus();
            this.showError(this.resources.amountInvalidError);
            return false;
        }

        this.hideMessage();
        return true;
    }

    private isNumeric(text) {
        return text.match(/^-?(\d+|\d+\.\d*|\d*\.\d+)$/);
    }

    private showError(error: string) {
        this.msgAddNewBonus.html(error).removeClass().addClass("errMsg");
    }

    private showSuccess(message: string) {
        this.msgAddNewBonus.html(message).removeClass().addClass("successMsg");
    }

    private hideMessage() {
        this.msgAddNewBonus.removeClass().addClass("hide").html("");
    }
}