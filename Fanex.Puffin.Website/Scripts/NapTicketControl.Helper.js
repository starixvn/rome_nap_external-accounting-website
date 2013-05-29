
if(NapTicketControl === undefined)
{
    NapTicketControl = {};
}

NapTicketControl.Helper = (function ($) {

    // define some private variables
    var _productTypes = {
        Sportsbook: 0,
        NumberGame: 1,
        LiveCasino: 6,
        TicketListing: 3
    };

    // used to store local bettypes, 
    // and avoid request to server when they are avaible
    var _betTypes = {
        Sportsbook: null,
        NumberGame: null,
        LiveCasino: null,
        TicketListing: null
    }

    return {
        authorizedMsg: 'You are not authorized on this function.',

        GetLicensees: function () {
            $.ajax({
                type: "POST",
                url: window.rootUrl + "Core/Common/GetLicensees",
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                data: null,
                beforeSend: function () {
                    NapTicketControl.disableInput();
                },
                success: function (licensees) {
                    if (licensees) {
                        NapTicketControl.Helper.RenderLicenseeDropDownList(licensees);
                    }
                },
                error: function () {
                },
                complete: function () {
                    NapTicketControl.enableInput();
                }
            });
        },

        RenderLicenseeDropDownList: function (licensees) {
            var self = this;
            var licenseeLength = licensees.length;
            var $licensee = $('.ddlLicensee');
            var licenseeHtml = '';

            for (var index = 0; index < licenseeLength; index++) {
                var currentLicensee = licensees[index];
                licenseeHtml += '<option value="' + currentLicensee.Id + '">' + currentLicensee.Name + '</option>';
            }

            $licensee.append("<optgroup label='Licensee'></optgroup>");
            $licensee.find('optgroup').append(licenseeHtml);
        },

        GetBettypes: function (product, eventType, isTicketListing) {
            var self = this;
            var mode = product; // determine Mode to transfer to sp on db

            if (isTicketListing === true) {
                mode = "TicketListing";
            }
            // get the list of bet types belong selected product        
            if (_betTypes[mode] !== null) {
                var html = self.RenderBettypes(product, eventType, isTicketListing);
                $('.bettype-wrp').html(html);
            }
            else {

                var param = {
                    mode: mode
                };

                $.ajax({
                    type: "POST",
                    url: window.rootUrl + "Core/Common/GetBettypes",
                    contentType: "application/json; charset=utf-8",
                    dataType: "json",
                    async: false,
                    data: JSON.stringify(param),
                    beforeSend: function () {
                        NapTicketControl.disableInput();
                    },
                    success: function (bettypes) {
                        if (bettypes) {
                            _betTypes[mode] = bettypes;
                            var html = self.RenderBettypes(product, eventType, isTicketListing);
                            $('.bettype-wrp').html(html);
                        }
                    },
                    error: function () {

                    },
                    complete: function () {
                        NapTicketControl.enableInput();
                    }
                });
            }
        },

        // Render Bettypes:
        // product: selected product
        // eventType: only affect when product is Sportsbook (0: dead ball, 1: live ball, 2: all)
        // Note: temperaroly, don't cn
        RenderBettypes: function (product, eventType, isTicketListing) {
            var self = this;

            // NOTE: Warning: If it is tickects listing, will include bettype: Mix Paraly, System Parlay
            if (isTicketListing === true) {
                if (product === "Sportsbook") {
                    var bettypes = $.grep(_betTypes["TicketListing"], function (value) {
                        return value.Type === 0;
                    });
                }

                if (product === "NumberGame") {
                    var bettypes = $.grep(_betTypes["TicketListing"], function (value) {
                        return value.Type === 1;
                    });
                }
            }
            else {
                // get the bettype list from the storage object
                var bettypes = _betTypes[product];
            }

            var deadBallBettypes = $.grep(bettypes, function (value) {
                return value.LiveIndicator === false;
            });
            var liveBallBettypes = $.grep(bettypes, function (value) {
                return value.LiveIndicator === true;
            });

            var html = '<table class="tbl-bettype">';
            html += '<thead><tr><th colspan="5">'
                + '<input type="checkbox" id="chkallbettype" checked="checked" />'
                + '<label for="chkallbettype"><b>All Bet types</b>'
                + '</th></tr></thead>';

            switch (product) {
                //                case 'Sportsbook':                  
                //                    switch (eventType) {                  
                //                        case '0':                  
                //                            // render only dead ball bettypes                  
                //                            html += this.RenderBettypesDetails(deadBallBettypes, 'BetTypes');                  
                //                            break;                  
                //                        case '1':                  
                //                            // render only live ball bettypes                  
                //                            html += this.RenderBettypesDetails(liveBallBettypes, 'BetTypes');                  
                //                            break;                  
                //                        default:                  
                //                            // render all bettypes                  
                //                            html += this.RenderBettypesDetails(bettypes, 'BetTypes');                  
                //                            break;                  
                //                    }                  
                //                    break;                  
                case 'NumberGame':
                    // render dead ball bettypes
                    html += '<tr class="bettype-group-title"> <td colspan="5">&nbsp;' + 'Dead ball' + '</td></tr>';
                    html += this.RenderBettypesDetails(deadBallBettypes, 'BetTypes', 0);

                    // render live ball bettype
                    html += '<tr class="bettype-group-title"> <td colspan="5">&nbsp;' + 'Live ball' + '</td></tr>';
                    html += this.RenderBettypesDetails(liveBallBettypes, 'LiveBetTypes', deadBallBettypes.length);
                    break;
                default:
                    html += this.RenderBettypesDetails(bettypes, 'BetTypes');
                    break;
            }

            html += '</table>';

            return html;
        },

        RenderBettypesDetails: function (bettypes, nameValue, startPosition) {
            var length = bettypes.length;
            var step = 5;
            var repeatTime = length / step;
            var html = '';

            if (startPosition === undefined) {
                startPosition = 0;
            }

            for (var i = 0; i < repeatTime; i++) {
                var offset = i * step;
                html += '<tr>';
                for (var j = 0; j < step; j++) {

                    if (offset + j < length) {
                        var bt = bettypes[offset + j];
                        var id = 'chk-bettype-' + startPosition;
                        var betname = bt.Name;

                        switch (bt.Id) {
                            case '85':
                                betname = "Next Over/Under";
                                break;
                            case '86':
                                if (bt.LiveIndicator === true) {
                                    betname = "Next Odd/Even";
                                } else {
                                    betname = "FT. Odd/Even";
                                }
                                break;
                            case '90':
                                if (bt.LiveIndicator === true) {
                                    betname += " (Live Game)";
                                } else {
                                    betname += " (Pre Game)";
                                }
                                break;
                        }
                        html += '<td>';
                        html += '<input type="checkbox" checked="checked" class="chkbettype"'
                            + ' id=' + id + ' value=' + bt.Id + ' name=' + nameValue + ' />';
                        html += '<label for="' + id + '">' + betname + '</label>';
                        html += '</td>';
                    } else {
                        html += '<td>&nbsp;</td>';
                    }

                    startPosition++;
                }
                html += '</tr>';
            }

            return html;
        },

        showMsgBox: function (msgs) {
            var msgbox = $('#msg-box');

            if (msgbox.length == 0) {
                msgbox = $(
                    '<div class="alert alert-error" id="msg-box">' +
                        '<button type="button" class="close" data-dismiss="alert">&times;</button>' +
                        '<div class="alert-content">' +
                        '</div>' +
                    '</div>').prependTo('body');
            }


            var html = [];

            if ($.isPlainObject(msgs)) {
                var msg = msgs;
                if (msg.ErrorCode > 0) {//Fail
                    html.push('<span class="fail">' + msg.ErrorMsg + '<i class="icon-warning-sign"></i></span><br/>');
                }
                else {
                    html.push('<span>' + msg.ErrorMsg + '&nbsp;<i class="icon-ok"></i></span><br/>');
                }
            }
            else {
                if ($.isArray(msgs)) {
                    for (var i = 0; i < msgs.length; i++) {
                        var msg = msgs[i];
                        if (msg.ErrorCode > 0) {
                            //Fail
                            html.push('<span class="fail">' + msg.ErrorMsg + '&nbsp;<i class="icon-warning-sign"></i></span><br/>');
                        }
                        else {
                            html.push('<span>' + msg.ErrorMsg + '&nbsp;<i class="icon-ok"></i></span><br/>');
                        }
                    }
                } else {
                    html.push('<span class="fail">' + msgs + '<i class="icon-warning-sign"></i></span><br/>');
                }
            }

            msgbox.attr('class', 'alert alert-success').find('.alert-content').html(html.join(''));
            msgbox.fadeIn();
            msgbox.show();
        },

        displayContent: function (isShow) {
            if (isShow) {
                $('#report-content').show();
            }
            else {
                $('#report-content').hide();
            }
        },

        ToggleFilterBar: function () {
            if ($('.filter-bar-inner').css('display') == 'none') {
                $('.filter-bar-inner').show();
            }
            else {
                $('.filter-bar-inner').hide();
            }

        }
    };
} (jQuery));