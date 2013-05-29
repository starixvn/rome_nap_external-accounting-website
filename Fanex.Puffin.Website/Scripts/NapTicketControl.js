/* File Created: January 28, 2013 */
$(function () {
    NapTicketControl.initialize();
});

var TicketControlConstants = {
    FormatConstant: {
        INT_NUMBER: '0,0',
        TWO_DECIMAL_NUMBER: '0,0.00',
        FOUR_DECIMAL_NUMBER: '0,0.0000',
        TWO_DECIMAL_PERCENT_NUMBER: '0,0.00%',
        MMM_DD_YYYY_STRING: 'MMM DD, YYYY',
        M_DD_YY_STRING: 'M dd, yy',
        MM_DD_YYYY_WITH_SLASH_STRING: 'MM/dd/yyyy',
        MMM_YY_STRING: 'MMM YY',
        MMM_YY_STRING_MONTH_PICKER: 'MMM yyyy',
        MMM_DD_YY_STRING_MONTH_PICKER: 'MMM dd, yyyy',
        MM_DD_YYYY_WITH_SLASH_STRING_MOMENT: 'M/D/YYYY',
        MMM_DD_YYYY_STRING_MOMENT: 'MMM D, YYYY'
    },

    MessageType: {
        Success: 1,
        Warning: 2,
        Error: 3
    }
}

var NapTicketControl = (function ($) {
    var MessageType = {
        Success: 1,
        Warning: 2,
        Error: 3
    };

    return {
        miniMenu: null,

        init: function () {
            NapTicketControl.miniMenu = $('header .mini-menu').change(function () {
                window.location.replace($(this).val());
            });

            $(document).click(function () {
                // hide message tooltip
                $('#msg').hide();
            });

            var Message = {};
        },

        disableInput: function () {
            try { $.blockUI(); } catch (e) { }
        },

        enableInput: function () {
            try { $.unblockUI(); } catch (e) { }
        },
        accReportRoot: null,
        accReportPopUp: null, //
        initialize: function () {
            NapTicketControl.napTicketControlPopUp = $("#dialog-popup");
        },
        popUp: {
            arrayHighLightControl: [],
            show: function (message, targets, messageType) {
                NapTicketControl.napTicketControlPopUp
                .removeClass("err")
                .removeClass("info")
                .empty()
                .text(message);

                var bodyWith = $(window).width();
                var popUpWidth = NapTicketControl.napTicketControlPopUp.width();
                var leftPosition = Math.round((bodyWith - popUpWidth) / 2);
                NapTicketControl.napTicketControlPopUp.css("left", leftPosition);
                //// 1: Infor, 2: Warning, 3: undefined, Error
                if (typeof (messageType) == 'undefined') {
                    NapTicketControl.napTicketControlPopUp
                    .addClass("err")
                    .append('<a class="close" title="Close Message" href="javascript:NapTicketControl.popUp.hide();"></a>');

                    if (typeof (targets) != 'undefined') {
                        NapTicketControl.popUp.highLightControl(targets);
                    }
                }
                else {
                    switch (messageType) {
                        case MessageType.Success: ////Success
                            NapTicketControl.napTicketControlPopUp.addClass("info");
                            setTimeout(function () {
                                NapTicketControl.napTicketControlPopUp.hide();
                            }, 4000);
                            break;
                        case MessageType.Warning:
                            break;
                        case MessageType.Error:
                            break;
                    }
                }

                NapTicketControl.napTicketControlPopUp.show();
            },

            hide: function () {
                NapTicketControl.napTicketControlPopUp.hide();
            },

            highLightControl: function (targets) {
                var controlIds = targets.split(',');
                for (var i = 0; i < controlIds.length; i++) {
                    var controlId = controlIds[i];
                    if (controlId.indexOf("#") < 0) {
                        controlId = "#" + controlId;
                    }
                    NapTicketControl.popUp.arrayHighLightControl.push(controlId);
                    $(controlId).addClass("input-validation-error");
                }

            },

            removeHighLight: function () {
                var controlIds = NapTicketControl.popUp.arrayHighLightControl;
                for (var i = 0; i < controlIds.length; i++) {
                    $(controlIds[i]).removeClass("input-validation-error");
                }
                NapTicketControl.popUp.arrayHighLightControl = [];
                NapTicketControl.popUp.hide();
            },

            reCheckIn: function () {
                $.ajax({
                    url: NapTicketControl.NapTicketControlRoot + "UserLogin/Login/ReCheckLogin",
                    type: "POST",
                    dataType: "json",
                    success: function (result) {
                        if (typeof (result) != 'undefined' && result != null) {
                            if (result.IsKickedOut == "1" || result.IsKickedOut == "2") {
                                alert('Another session has been logged in or your session is expired. Forced to logout!');
                                window.location.href = NapTicketControl.NapTicketControlRoot + "UserLogin/Login/LogOut";
                            }
                            else if (result.IsKickedOut == "3") {
                                window.location.href = NapTicketControl.NapTicketControlRoot + "UserLogin/Common/Offline";
                            }
                        }
                    },
                    error: function (request, status, error) {
                    }
                });
            }
        },

        showMessageBox: function (type, msg) {
            var msgbox = $('#msg-box');

            if (msgbox.length == 0) {
                msgbox = $(
                    '<div class="alert alert-error" id="msg-box">' +
                        '<button type="button" class="close" data-dismiss="alert">&times;</button>' +
                        '<div class="alert-content">' +
                        '</div>' +
                    '</div>').prependTo('body');
            }

            msgbox.attr('class', type).find('.alert-content').html(msg);
            msgbox.fadeIn();
        },

        downloadFile: function (url, data, method) {
            //url and data options required
            if (url && data) {
                //data can be string of parameters or array/object
                data = typeof data == 'string' ? data : $.param(data);
                var downloadUrl = url + "?" + data;
                var iframe = $('body iframe');
                if (iframe.length == 0) {
                    iframe = $('<iframe width="0" height="0" class="display-none"></iframe>');
                    iframe.appendTo('body');
                }

                iframe.attr('src', downloadUrl);
            }
        }
    };
} (jQuery));

$(document).ready(function () {
    NapTicketControl.init();
});