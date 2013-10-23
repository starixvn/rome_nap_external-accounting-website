/*--Global variable*/
var strListPreset="listPrePT";
var strListAutoPreset="listAutoPrePT";
var strListSuper="listSuperPT";
var strListSuperAuto="listSuperAutoPT";
var strListMaster="listMasterPT";
var strListAgent="listAgentPT";

var strListMax="listMaxPT";
var strListMin="listMinPT";
var strListComm="listComm";
var strChkAutoPreset="chkAutoPreset";
var strChkAuto="chkAuto";
var strerrPT="errPT";
var strerrPTMin="errPTMin";

var strChkStake="chkStake";

/*--End global variable declaration--*/
/*Global function*/
function roundNumber(num, dec) {
    var result = Math.round(Math.round(num * Math.pow(10, dec + 1)) / Math.pow(10, 1)) / Math.pow(10, dec);
    return result;
}

function toString(obj) {
    if (obj == null)
        return "";
     else return obj.toString();
}
function obj(id) {
    if (document.getElementById(id))
        return document.getElementById(id);
    else return null
}
function gup(name) {
    name = name.replace(/[\[]/, "\\\[").replace(/[\]]/, "\\\]");
    var regexS = "[\\?&]" + name + "=([^&#]*)";
    var regex = new RegExp(regexS);
    var results = regex.exec(window.location.href);
    if (results == null)
        return "";
    else
        return results[1];
}
// Success=0, Error=1, Warning=2
function showAlertMessage(containerid, msg, type) {
    var errorElement = $('#' + containerid);
    switch (type) {
        case 0:
            msg = '<div class="msgcorrect">' + msg + '</div>';
            break;
        case 1:
        case 2:
            msg = '<div class="msgerror">' + msg + '</div>';
            break;
        default:
            return;
    }
    errorElement.removeClass('display-none');
    errorElement.html(msg);
    window.scroll(0, 0);
    parent.scroll(0, 0);
}
/*Global function*/
//Create option for select control
////obj: jquery object 
function CreateOption(obj, curr, max, min, step, round) {
    obj.children().remove().end();
    var index = 0;
    var i = max;
    if (curr > max) obj.append('<option selected value="' + curr + '">' + curr + '</option>');
    while (i >= min) {
        obj.append('<option value="' + i + '">' + i + '</option>');
        if (i == curr) {
            obj.get(0).selectedIndex = index;
        }
        i = roundNumber(i - step, round);
        index++;
    }
    if (curr < min) obj.append('<option selected value="' + curr + '">' + curr + '</option>');
}

function MenuItemClick(divControl, divpanel) {
    var btn = $("#" + divControl);
    var panel = $("#" + divpanel);
    if (panel.is(":hidden")) {
        btn.removeClass("md3").addClass("md2");
        panel.removeClass("hidden").addClass("show");

    } else {
        btn.removeClass("md2").addClass("md3");
        panel.removeClass("show").addClass("hidden");
    }
}

function AssignSite() {
    var obj = $("#listSite");
    $.ajax({
        type: "POST",
        url: "../../Handlers/PT/GetSiteList.ashx",
        dataType: "text",
        success: function(data) {
            data = $.evalJSON(data);
            var SiteData = $.evalJSON(data.SiteData);
            for (var i in SiteData) {
                obj.append('<option value="' + SiteData[i].Value + '">' + SiteData[i].Value + '</option>');
            }
        }
    });
}

function AssignCurrency() {
    var obj = $("#listCurrency");
    $.ajax({
        type: "POST",
        url: "../../Handlers/PT/GetCurrencyList.ashx",
        dataType: "text",
        success: function(data) {
            data = $.evalJSON(data);
            var CurrencyData = $.evalJSON(data.CurrencyData);
            for (var i in CurrencyData) {
                obj.append('<option value="' + CurrencyData[i].Value + '">' + CurrencyData[i].Text + '</option>');
            }
        }
    });
}

function AssignCommission() {
    jQuery.each($(".select-comm"), function() {
        if ($("#" + $(this).attr("id") + " option").size() <= 1) {
            var curr = $(this).val();
            var max = roundNumber($(this).attr("max"), 4);
            CreateOption($(this), curr, max, 0, 0.0005, 4);
        }
    });
}

function AssignPT() {
    jQuery.each($(".select-pt"), function() {
            if ($("#" + $(this).attr("id") + " option").size() <= 1) {
                var curr = $(this).val();
                var max = $(this).attr("max");
                var min = $(this).attr("min");
                CreateOption($(this), curr, max, min, 0.01, 2);
            }
    });
}

function ResetMaxTransfer(custId) {
    var param = { custId: custId, modifiedUserId: parent.userId, modifiedUsername: parent.username };
    $.ajax({
        type: 'POST',
        url: 'ResetMaxTransfer',
        contentType: 'application/json; charset=utf-8',
        dataType: 'json',
        data: JSON.stringify(param),
        beforeSend: function (jqXHR, settings) {
            PTBlockUI();
        },
        success: function (rs) {
            if (rs) {
                if (rs.ErrorCode == 0) {
                    showAlertMessage('err', 'Reset Max Transfer Successfully', 0);
                    afterUpdate(2000);
                }
                else if (rs.ErrorCode == 500) {
                    showAlertMessage('err', rs.ErrorMessage, 1);
                }
                else {
                    showAlertMessage('err', rs.ErrorMessage, 1);
                }
            }
        },
        error: function (jqXHR, textStatus, errorThrown) {
            if (jqXHR.responseText) {
                var errorObject = JSON.parse(jqXHR.responseText);
                showAlertMessage('err', errorObject.errorMessage, 1);
            }
        },
        complete: function () {
            PTUnBlockUI();
        }
    });
}

function afterUpdate(timeout) {
    t1 = setTimeout(function () {
        //$('#err').html('');
        parent.Fanex.ExtAcc.Form.Close(false, undefined, undefined, function () {
            if ($('#tdIsSyncData')) { // if has 'Enable Bingo' checkbox, need to remove it
                $('#tdIsSyncData').remove();
            }
        });
    }, timeout);
}