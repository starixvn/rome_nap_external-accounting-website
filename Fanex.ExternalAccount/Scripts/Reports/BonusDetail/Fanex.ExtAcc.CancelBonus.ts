class CancelBonus {
    private $: JQueryStatic;
    private callback: Function;
    private msgCancelPending: JQuery;
    private resources = {
        popupTitle: "Cancel Bonus",
        confirmMessage: "Are you sure you want to cancel this Bonus?"
    };

    constructor($: JQueryStatic, callback: Function) {
        this.$ = $;
        this.callback = callback;
        this.msgCancelPending = this.$("#div-msgCancelPending");
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
                    self.resources.confirmMessage,
                    350,
                    "auto",
                    false,
                    function () {
                        self.cancelPendingPost(custId, transId);
                    });
            });
    }

    private cancelPendingPost(custId: number, transId: number) {
        Fanex.ExtAcc.BlockUI();
        this.$.post(
            resolveClientUrl("BonusDetail/CancelPending"),
            { custId: custId, transId: transId, groupName: this.$("#selectedGroupName").html() },
            data => {
                Fanex.ExtAcc.Form.Close();
                const isSuccess = data.isSuccess || data.IsSuccess;
                if (!isSuccess) {
                    this.showError(data.errorMessage || data.ErrorMessage);
                } else {
                    this.showSuccess(data.ErrorMessage);
                    setTimeout(() => {
                        this.hideMessage();
                        this.callback();
                    }, 1000 * 3);
                }
            })
            .always(Fanex.ExtAcc.UnBlockUI);
    }

    private showError(error: string) {
        this.msgCancelPending.html(error).removeClass().addClass("errMsg");
    }

    private showSuccess(message: string) {
        this.msgCancelPending.html(message).removeClass().addClass("successMsg");
    }

    private hideMessage() {
        this.msgCancelPending.removeClass().addClass("hide").html("");
    }
}