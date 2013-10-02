/*Alert message*/
var lblPasswordRequire = 'Password must have at least 6 characters, including at least 2 numbers and at least 2 digits. (e.g:pass22)';
var alertPasswordEmpty = 'Sorry, password can not be empty!!!'; 
var alertUserName = 'Please Enter User Name.';
var alertCompare3 = 'Min Bet cannot be greater than Max Bet'; 
var alertMinbet = 'Incorrect Min Bet';
var alertMaxbet = 'Incorrect Max Bet'; 
var alertMaxpermatch = 'Incorrect Max Per Match';
var alertCompare = 'Max Bet cannot be greater than Max Per Match';
var alertIncorrectCredit = 'Incorrect Max Credit';
var alertMinLessCredit = 'Max credit must be more than or equal to ';
var alertSite = 'Please Select Site'; 
var alertCurrency = 'Please Select Currency';
var alertCreditInvalid = 'Member Max Credit must be between {1} and {2}';
/*Alert message*/
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

    switch (type) {
        case 0:
            document.getElementById(containerid).innerHTML = "<div class='alertboxscc'><div>" + msg + "</div></div>";
            break;
        case 1:
            document.getElementById(containerid).innerHTML = "<div class='alertboxerr'><div>" + msg + "</div></div>";
            break;
        case 2:
            document.getElementById(containerid).innerHTML = "<div class='alertboxwrn'><div>" + msg + "</div></div>"; ;
            break;
        default:
            break
    }
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
    jQuery.each($(".commlist"), function() {
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