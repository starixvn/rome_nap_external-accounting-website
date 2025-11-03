/// <reference path="_references.js" />
if (typeof Fanex == 'undefined' || typeof Fanex.ExtAcc == 'undefined') {
    alert("Fanex.ExtAcc.js module is required!")
}

Fanex.ExtAcc.Form = {
    Init: function () {
    },

    Show: function (url, title, width, height, reloadOnClose, callBack, marginRight, popupId, params) {
        if (typeof Fanex.ExtAcc.BlockUI == 'function')
            Fanex.ExtAcc.BlockUI();

        var id = popupId ? popupId : 'windowPopup';
        var Form = document.getElementById(id) == null
            ? document.createElement("div")
            : document.getElementById(id);
        Form.innerHTML = '';
        var DivContent = document.createElement("div");

        var margin = '0';

        if (marginRight) {
            margin = marginRight + 'px';
        }

        Form.setAttribute('id', id);
        Form.setAttribute('style', 'display: block; border: none; font-size: 12px; z-index: 999999; margin-right: ' + margin);
        DivContent.setAttribute('style', 'height: ' + (height - 50) + 'px');

        var divFrame = document.createElement("div");

        divFrame.setAttribute('style', 'height: ' + (height - 50) + 'px; margin-left: 5px; margin-right: ' + margin);
        divFrame.setAttribute('id', 'frame');

        DivContent.appendChild(divFrame);
        Form.appendChild(DivContent);

        var $self = this;
        var data = null;

        if (params) {
            data = params;
        }

        $.ajax({ url: url, data: data, type: 'post' }).done(function (data) {
            if (typeof Fanex.ExtAcc.UnBlockUI == 'function')
                Fanex.ExtAcc.UnBlockUI();

            if (data.errorCode && data.errorCode == "1") {
                Fanex.ExtAcc.Form.Alert('Error', data.errorMessage.formatErrorMessage(), 300, 150, 'alert');
                return;
            }

            if (Fanex.ExtAcc.ShowSecurityCodePage(data)) {
                return;
            }

            document.body.appendChild(Form);
            $(DivContent).html(data);

            $('#' + id).dialog({
                title: title,
                width: width,
                height: height,
                resizable: false,
                draggable: true,
                modal: true,
                autoOpen: true,
                position: { my: "center", at: "center", of: window },
                close: function (event) {
                    $self.Close(reloadOnClose, null, id);
                    if (typeof closeClick == 'function') {
                        closeClick();
                    }
                }
            });

            $('div#' + id).css('height', (height - 34) + 'px');
            if (callBack && typeof callBack === 'function') {
                callBack();
            }
        });
    },

    ShowFrame: function (url, title, width, height, reloadOnClose, callBack, marginRight, popupId, resizable, resizeIframe) {
        Fanex.ExtAcc.BlockUI();

        var id = popupId ? popupId : 'windowPopup';
        var mainDiv = document.createElement("div");
        var Frame = document.createElement("iframe");
        var margin = '30px';
        mainDiv.setAttribute('id', id);

        if (marginRight !== undefined) {
            margin = marginRight + 'px';
        }

        Frame.setAttribute('frameBorder', "0");
        Frame.setAttribute('name', id);

        var $self = this;
        var data = null;

        mainDiv.appendChild(Frame);
        document.body.appendChild(mainDiv);
        $('#' + id).dialog({
            title: title,
            height: height,
            width: width,
            resizable: resizable ? true : false,
            draggable: true,
            modal: true,
            autoOpen: true,
            position: { my: "center", at: "center", of: window },
            open: function () {
                if (Frame.src == '') {
                    Frame.src = url;
                }
            },
            close: function () {
                Frame.src = 'about:blank';
                $self.Close(reloadOnClose, null, id);
            }
        });

        $('#' + id).css('width', width + 'px');
        $('#' + id).css('height', (height - 34) + 'px');
        Frame.setAttribute('style', 'display: block; border: none; font-size: 12px; z-index: 999999; margin-right: ' + margin + '; height: 100%; width: 100%;');
        Frame.setAttribute('onload', resizeIframe ? 'resizeIframe(this)' : 'Fanex.ExtAcc.UnBlockUI()');
        if (callBack && typeof callBack === 'function') {
            callBack();
        }
    },

    ConfirmBox: function (title, message, width, height, reloadOnClose, okClickEvent, cancelClickEvent, popupId, isCancel, closeClick, isSubmit) {
        var Form = document.createElement("div");
        var DivContent = document.createElement("div");
        var id = popupId ? popupId : 'windowPopup';

        Form.setAttribute('id', id);
        Form.setAttribute('style', 'display: none; border: none; font-size: 13px; z-index: 999999');

        DivContent.setAttribute('style', 'height: ' + (height - 102) + 'px');

        var divFrame = document.createElement("div");
        divFrame.setAttribute('style', 'height: ' + (height - 102) + 'px; line-height: 18px;');

        var frame = document.createElement("span");
        frame.innerHTML = message;
        frame.setAttribute('style', 'width: 100%; height: 100%; border: none');

        divFrame.appendChild(frame);
        DivContent.appendChild(divFrame);

        Form.appendChild(DivContent);

        document.body.appendChild(Form);

        var $self = this;

        $('#' + id).css('display', 'block');
        $('#' + id).dialog({
            height: height, width: width, minWidth: 300, minHeight: 150,
            title: title,
            resizable: false,
            draggable: true,
            modal: true,
            buttons: [
                {
                    text: isSubmit ? "Submit" : "Yes",
                    'class': "btn -submit",
                    click: function () {
                        if (okClickEvent && typeof okClickEvent === 'function') {
                            okClickEvent();
                        }
                        else {
                            $self.Close(reloadOnClose, null, id);
                        }
                    }
                },
                {
                    text: isCancel ? "Cancel" : "No",
                    'class': "btn -normal",
                    click: function () {
                        if (cancelClickEvent && typeof cancelClickEvent === 'function') {
                            cancelClickEvent();
                        }
                        else {
                            $self.Close(reloadOnClose, null, id);
                        }
                    }
                }],
            close: function (event) {
                $self.Close(reloadOnClose, null, id);
                if (typeof closeClick == 'function') {
                    closeClick();
                }
            }
        });

        $('div#' + id).css('height', (height - 86) + 'px');
    },

    Alert: function (title, message, width, height, popupId, okClick) {
        var Form = document.createElement("div");
        var DivContent = document.createElement("div");
        var id = popupId ? popupId : 'windowPopup';

        Form.setAttribute('id', id);
        Form.setAttribute('style', 'display: none; border: none; font-size: 12px; z-index: 999999');

        DivContent.setAttribute('style', 'height: ' + (height - 122) + 'px');

        var divFrame = document.createElement("div");
        divFrame.setAttribute('style', 'height: ' + (height - 122) + 'px;line-height: 18px;');

        var frame = document.createElement("span");
        frame.innerHTML = message;
        frame.setAttribute('style', 'width: 100%; height: 100%; border: none');

        divFrame.appendChild(frame);
        DivContent.appendChild(divFrame);

        Form.appendChild(DivContent);

        document.body.appendChild(Form);

        var $self = this;

        $('#' + id).css('display', 'block');
        $('#' + id).dialog({
            height: height, width: width, minWidth: 300, minHeight: 150,
            title: title,
            resizable: false,
            draggable: true,
            modal: true,
            buttons: [
                {
                    text: "OK",
                    'class': "btn -submit",
                    click: function () {
                        $self.Close(null, null, id);
                        if (typeof okClick == 'function') {
                            okClick();
                        }
                    }
                }],
            close: function (event) {
                $self.Close(null, null, id);
                if (typeof okClick == 'function') {
                    okClick();
                }
            }
        });

        $('div#' + id).css('height', (height - 86) + 'px');
    },

    Close: function (reload, delay, popupId, callBack) {
        Validators.hide();

        var delayTime = delay ? delay : 100;
        var id = popupId ? popupId : 'windowPopup';

        reload && location.reload(true);

        var timeout = setTimeout(function () {
            $('#' + id).remove();
            clearTimeout(timeout);
        }, delayTime);

        if (typeof callBack == 'function') {
            callBack();
        }
    }
};