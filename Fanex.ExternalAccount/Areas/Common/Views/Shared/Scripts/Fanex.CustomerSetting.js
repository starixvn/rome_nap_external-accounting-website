if (typeof Fanex == 'undefined') {
    Fanex = {};
}

Fanex.CustomerSetting = {
    $this: null,
    BlockedUI: false,

    Init: function () {
        $this = this;
    },

    BlockUI: function () {
        if (!$this.BlockedUI) {
            $.blockUI({
                message: '<img src="/Areas/Common/Views/Shared/Images/Loading.gif" />',
                onBlock: function () {
                    $this.BlockedUI = true;
                },
                onUnblock: function () {
                    $this.BlockedUI = false;
                }
            });
        }
    },

    UnBlockUI: function () {
        $.unblockUI(document);
    }
}

$(document).ready(function () {
    Fanex.CustomerSetting.Init();
});