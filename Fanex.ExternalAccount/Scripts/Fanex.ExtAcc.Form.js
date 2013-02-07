/// <reference path="_references.js" />
if (typeof Fanex == 'undefined' || typeof Fanex.ExtAcc == 'undefined') {
    alert("Fanex.ExtAcc.js module is required!")
}

Fanex.ExtAcc.Form = {
    Init: function () {
    },

    Show: function (url, title, width, height, reloadOnClose, callBack, marginRight, popupId) {
        var id = popupId ? popupId : 'windowPopup';
        var Form = document.createElement("div");
        var DivContent = document.createElement("div");

        var margin = '30px';

        if (marginRight) {
            margin = marginRight + 'px';
        }

        Form.setAttribute('id', id);
        Form.setAttribute('style', 'display: block; border: none; font-size:12px; z-index:999999; margin-right: ' + margin);

        DivContent.setAttribute('style', 'height: ' + (height - 80) + 'px');

        var divFrame = document.createElement("div");

        divFrame.setAttribute('style', 'height: ' + (height - 80) + 'px; margin-left: 5px; margin-right: ' + margin);
        divFrame.setAttribute('id', 'frame');

        DivContent.appendChild(divFrame);
        Form.appendChild(DivContent);

        var $self = this;

        $.ajax(url).done(function (data) {
            DivContent.innerHTML = data;
            document.body.appendChild(Form);
            $('#' + id).dialog({
                title: title,
                height: height,
                width: width,
                resizable: false,
                draggable: true,
                modal: true,
                autoOpen: true,
                position: { my: "center", at: "center", of: window },
                close: function () {
                    $self.Close(reloadOnClose);
                }
            });

            if (callBack && typeof callBack === 'function') {
                callBack();
            }
        });
    },

    ConfirmBox: function (title, message, width, height, reloadOnClose, okClickEvent, cancelClickEvent, popupId) {
        var Form = document.createElement("div");
        var DivContent = document.createElement("div");
        var id = popupId ? popupId : 'windowPopup';

        Form.setAttribute('id', id);
        Form.setAttribute('style', 'display: none; border: none;font-size:12px');

        DivContent.setAttribute('style', 'height: ' + (height - 125) + 'px');

        var divFrame = document.createElement("div");
        divFrame.setAttribute('style', 'height: ' + (height - 125) + 'px; margin: 2px 5px;line-height: 18px');

        var frame = document.createElement("span");
        frame.innerHTML = message;
        frame.setAttribute('style', 'width: 100%; height: 100%; border: none');

        divFrame.appendChild(frame);
        DivContent.appendChild(divFrame);

        Form.appendChild(DivContent);

        document.body.appendChild(Form);

        var $self = this;

        $('#' + id).css('display', 'block');
        $('#' + id).dialog({ height: height, width: width, minWidth: 300, minHeight: 150,
            title: title,
            resizable: false,
            modal: true,
            buttons: [
            {
                text: "Yes",
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
                text: "No",
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
            }
        });
    },

    Close: function (reload, delay, popupId) {
        var delayTime = delay ? delay : 100;
        var id = popupId ? popupId : 'windowPopup';

        reload && location.reload(true);

        var timeout = setTimeout(function () {
            $('#' + id).remove();
            clearTimeout(timeout);
        }, delayTime);
    }
}