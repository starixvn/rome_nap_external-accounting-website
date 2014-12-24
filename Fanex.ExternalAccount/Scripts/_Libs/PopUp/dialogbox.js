
var currentExecFrame = null;
function OpenPopup(url, title, width, height, modal, resizable, currWindow, duration, fnRecall, fnOnClose) {
    jQuery("#dialog-modal").dialog("destroy").remove();

    var dialoghtml = jQuery('<div id="dialog-modal" title="' + title + '" style=" overflow:hidden"></div>');
    if (url != "") {
        var iframe = jQuery('<iframe id="innerFrame" />').attr({ "frameborder": "0", "scrolling": "auto" }).appendTo(dialoghtml);
    }

    dialoghtml.appendTo(jQuery("body"));

    jQuery("#dialog-modal").dialog({
        resizable: resizable,
        modal: modal,
        width: width,
        height: height,
        autoOpen: false,
        open: function (event, ui) {
            jQuery("#innerFrame").attr({ "src": url }).css({ "height": jQuery(this).css("height"), "width": jQuery(this).css("width") });
        },
        dragStart: function (event, ui) {
            jQuery("#innerFrame").hide();
        },
        dragStop: function (event, ui) {
            jQuery("#innerFrame").css({ "height": jQuery(this).css("height"), "width": jQuery(this).css("width") }).show();
        },
        resizeStart: function (event, ui) {
            jQuery("#innerFrame").hide();
        },
        resizeStop: function (event, ui) {
            jQuery("#innerFrame").css({ "height": jQuery(this).css("height"), "width": jQuery(this).css("width") }).show();
        },
        close: function (event, ui) {
            if (currentExecFrame != null && currentExecFrame.onclose) {
                currentExecFrame.onclose();
                setTimeout('currentExecFrame=null;', 500);
            }
        }
    });

    jQuery("#dialog-modal").dialog("open");
    if (currWindow != null) {
        currentExecFrame = currWindow;
        currentExecFrame.duration = duration;
        if (fnRecall) currentExecFrame.callback = fnRecall;
        if (fnOnClose) currentExecFrame.onclose = fnOnClose;
    }
}

function ClosePopup() {
    //jQuery("#dialog-modal").dialog("destroy").remove();
    jQuery("#dialog-modal").dialog('close');
}

function afterUpdate(autoClose) {
    if (currentExecFrame != null) {
        var duration = currentExecFrame.duration;
        if (duration == null) duration = 0;
        if (currentExecFrame.callback) {
            setTimeout('currentExecFrame.callback();', duration);
        }
        setTimeout('currentExecFrame=null;', duration + 1000);
    }
    if (autoClose)
        setTimeout('ClosePopup()', 2000);
}

