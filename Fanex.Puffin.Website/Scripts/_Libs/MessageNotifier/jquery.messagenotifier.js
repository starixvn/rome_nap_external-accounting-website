
(function ($) {
    var alertType = {
        successType: 0,
        errorType: 1,
        warningType: 2
    };

    function appendMessageTexts(messageTexts) {
        var html = '';
        if ($.isArray(messageTexts)) {
            var messageLen = messageTexts.length;
            for (var i = 0; i < messageLen; i++) {
                html += "<li>" + messageTexts[i] + "</li>";
            }
        }
        else {
            html += "<li>" + messageTexts + "</li>";
        }
        return html;
    };

    $.fn.showMessage = function (messageTexts, altType) {
        var isShow = false;
        var html = '';
        var messages = appendMessageTexts(messageTexts);
        var $this = $(this);
        // Success=0, Error=1, Warning=2
        switch (altType) {
            case alertType.successType:
                isShow = true;
                html = "<span class='message-success'>" + messages + "</span>";
                break;
            case alertType.errorType:
                isShow = true;
                html = "<span class='message-error'><ul>" + messages + "</ul></span>";
                break;
            case alertType.warningType:
                isShow = true;
                html = "<span class='messsage-warning'><ul>" + messages + "</ul></span>";
                break;
            default:
                break
        }
        if (isShow) {
            $this.addClass("alert-message-box");
            $this.html(html);
            $this.removeClass("display-none");
        }
        window.scroll(0, 0);
        parent.scroll(0, 0);
    };

    $.fn.showErrorMessage = function (messageTexts) {
        $(this).showMessage(messageTexts, 1);
    };

    $.fn.showWarningMessage = function (messageTexts) {
        $(this).showMessage(messageTexts, 2);
    };

    $.fn.showSuccessMessage = function (messageTexts) {
        $(this).showMessage(messageTexts, 0);
    }

    $.fn.removeMessage = function () {
        var $this = $(this);
        $this.empty();
        $this.addClass("display-none");
    }
})(jQuery);
