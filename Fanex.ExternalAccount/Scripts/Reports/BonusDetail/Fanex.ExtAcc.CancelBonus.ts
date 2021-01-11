class CancelBonus {
    private $: JQueryStatic;
    private callback: Function;
    private buttonSubmit: JQuery;
    private resources = {
        popupTitle: "Cancel Bonus",
        confirmMessage: '<div id="div-msgCancelPending" class="hide"></div>Are you sure you want to cancel this Bonus (Ref. No.: {0})?'
    };

    constructor($: JQueryStatic, callback: Function) {
        this.$ = $;
        this.callback = callback;
    }

    RegisterCancelPending(elements) {
        const self = this;
        elements
            .unbind("click")
            .bind("click", function () {
                var $this = self.$(this);
                const custId = $this.data("custid");
                const transId = $this.data("transid");
                Fanex.ExtAcc.Form.ConfirmBox(
                    self.resources.popupTitle,
                    self.resources.confirmMessage.replace("{0}", transId),
                    450,
                    "auto",
                    false,
                    () => self.cancelPendingPost(custId, transId),
                    null,
                    "cancel-bonus-modal");
                self.buttonSubmit = self.$("#cancel-bonus-modal").next(".ui-dialog-buttonpane").find(".btn.-submit");
            });
    }

    private cancelPendingPost(custId: number, transId: number) {
        Fanex.ExtAcc.BlockUI();
        this.buttonSubmit.prop("disabled", true);
        this.$.post(
            resolveClientUrl("BonusDetail/CancelPending"),
            { custId: custId, transId: transId, groupName: this.$("#selectedGroupName").html() },
            data => {
                const isSuccess = data.isSuccess || data.IsSuccess;
                if (!isSuccess) {
                    this.showError(data.errorMessage || data.ErrorMessage);
                    this.buttonSubmit.prop("disabled", false);
                    this.callback();
                } else {
                    this.showSuccess(data.ErrorMessage);
                    setTimeout(() => {
                        this.callback();
                        Fanex.ExtAcc.Form.Close(null, null, "cancel-bonus-modal");
                    }, 1000 * 3);
                }
            })
            .always(Fanex.ExtAcc.UnBlockUI);
    }

    private showError(error: string) {
        this.$("#div-msgCancelPending").html(error).removeClass().addClass("errMsg");
    }

    private showSuccess(message: string) {
        this.$("#div-msgCancelPending").html(message).removeClass().addClass("successMsg");
    }

    private hideMessage() {
        this.$("#div-msgCancelPending").removeClass().addClass("hide").html("");
    }
}