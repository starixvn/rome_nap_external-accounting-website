/* File Created: March 27, 2013 */
$(function () {
    batchMsg.init();
});



var batchMsg = {
    init: function () {
        $('#fsMessage').show();

        $('body').delegate('#btnSearch', 'click', function (e) {
            e.stopPropagation();
            batchMsg.updateMsg();
        });
    },

    updateMsg: function () {
        if (!validateData.checkParamBeforeUdpMsg()) {
            return;
        }

        var param = {
            pmsg: {
                Content_en: $('#msg-en').val(),
                Content_ch: $('#msg-ch').val(),
                Content_cs: $('#msg-cs').val(),
                Content_th: $('#msg-th').val(),
                Content_it: $('#msg-it').val(),
                Content_jp: $('#msg-jp').val(),
                Content_es: $('#msg-es').val(),
                Content_ko: $('#msg-ko').val()
            },
            TransIds: validateData.TransId
        };

        $.ajax({
            type: "POST",
            url: window.rootUrl + "Misc/BatchMessage/UploadMsg",
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            data: JSON.stringify(param),
            beforeSend: function () {
                NapTicketControl.disableInput();
            },
            success: function (rs) {
                NapTicketControl.Helper.showMsgBox(rs);
                var isSuccess = true;
                for (var i = 0; i < rs.length; i++) {
                    if (rs[i].ErrorCode > 0) {
                        isSuccess = false;
                        break;
                    }
                }

                if (isSuccess) {
                    $('.msg-void-ticket').val('');
                    $('#TransId').val('');
                }
            },
            error: function (e) { },
            complete: function (e) {
                if (e.status == 403) {
                    NapTicketControl.Helper.showMsgBox(NapTicketControl.Helper.authorizedMsg);
                }
                NapTicketControl.enableInput();
            }
        });
    }
};