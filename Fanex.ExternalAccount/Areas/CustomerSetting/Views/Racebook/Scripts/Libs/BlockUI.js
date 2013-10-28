function BlockUI(msg, duration, func) {
    if (msg == null || msg == "") {
        jQuery.blockUI({
            message: '<table class="blockUI"><tr><td class="tdloading"></td></tr></table>',
            fadeIn: 0,
            fadeOut: 0,
            css: {
                top: ($(window).height() - 100) / 2 + 'px',
                left: ($(window).width() - 100) / 2 + 'px',
                border: 'none'
            }
        });
    } else {
        if (duration == null) {
            jQuery.blockUI({
                message: '<table class="blockUI"><tr><td class="tdloading"></td></tr><tr><td><b id="blockmsg">' + msg + '</b></td></tr></table>',
                fadeIn: 0,
                fadeOut: 0,
                css: {
                    top: ($(window).height() - 136) / 2 + 'px',
                    left: ($(window).width() - 136) / 2 + 'px',
                    border: 'none'
                }
            });
        }
        else
        {
            jQuery.blockUI({
                message: '<table class="blockUI"><tr><td class="tdloading"></td></tr><tr><td><b id="blockmsg" class="msgwaiting">' + msg + '</b></td></tr></table>',
                fadeIn: 0,
                fadeOut: 0,
                css: {
                    top: ($(window).height() - 236) / 2 + 'px',
                    left: ($(window).width() - 536) / 2 + 'px',
                    border: 'none'
                },
                timeout:   duration, 
                onBlock: func
            });
        }
    }
}

