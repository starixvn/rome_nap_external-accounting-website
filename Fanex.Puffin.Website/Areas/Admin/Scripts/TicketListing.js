/* File Created: February 5, 2013 */
function TicketListing() {

    this.DOMElement =
    {
        $ddlEventType: null,
        $product: null,
        $bettypeWrp: null,
        $chkTransDateRange: null,
        $txtCalendar: null,
        $optFilters: null,
        $eventTypeWrp: null,
        $body: null,
        $msg: null,
        $matchInfoContainer: null,
        $ticketsContainer: null,
        $betlistContainer: null
    };

    // store old params and cache match info
    this.TicketListingDataReq = null;
    this.BetListDataRequest = null;
    this.ExportDataRequest = null;
    this.MatchInfo = null;
};

TicketListing.prototype.Mode = {
    Sportsbook: 0,
    NumberGame: 1,
    VirtualSports: 5,
    LiveCasino: 6
};

TicketListing.prototype.BetTypes = {
    Sportsbook: null,
    NumberGame: null,
    VirtualSports: null,
    LiveCasino: null
};

TicketListing.prototype.CacheDOMElement = function () {
    var self = this;

    self.DOMElement.$ddlEventType = $('.ddlEventType');
    self.DOMElement.$product = $('.product');
    self.DOMElement.$bettypeWrp = $('.bettype-wrp');
    self.DOMElement.$chkTransDateRange = $('.chkTransDateRange');
    self.DOMElement.$txtCalendar = $('.txtCalendar');
    self.DOMElement.$optFilters = $('.opt-filters');
    self.DOMElement.$eventTypeWrp = $('.event-type-wrp');
    self.DOMElement.$body = $('#body');
    self.DOMElement.$msg = $('#msg');
    self.DOMElement.$matchInfoContainer = $('#match-info-container');
    self.DOMElement.$ticketsContainer = $('#tickets-container');
    self.DOMElement.$betlistContainer = $('#betlist-container');
}

TicketListing.prototype.Init = function () {

    jQuery("#TicketListingForm").validationEngine();

    var self = this;

    self.CacheDOMElement();
    //    Helper.GetLicensees();
    NapTicketControl.Helper.GetBettypes(self.CollectSelectedProduct(), 0, true);
    self.SetupCalendar('', '.txtCalendar');
    $('.txtCalendar').attr('disabled', 'disabled');
    $("#LiveHomeScore").keyup(function () { }).ForceIntegerOnly();
    $("#LiveAwayScore").keyup(function () { }).ForceIntegerOnly();

    // handle events from DOM element
     $('.icon-from-transdate-calendar').click(function () {
        $('#FromTransDate').datetimepicker("show");
    });

    $('.icon-to-transdate-calendar').click(function () {
        $('#ToTransDate').datetimepicker("show");
    });

    $('.toggle-filter-bar').click(function(){
        NapTicketControl.Helper.ToggleFilterBar(this);
    });

    self.DOMElement.$ddlEventType.change(function () {
        var eventType = $(this).val(); // live type drop down list
        var product = $('input[name=Product]:checked').val();
        self.ResetOptionFilter();

        if (eventType === '1' && product === "Sportsbook") { // live ball
            self.DOMElement.$optFilters.show();
        } else {  // dead ball
            self.DOMElement.$optFilters.hide();
        }

        // Comment: don't load bettype again, when selecting event type
        //Helper.GetBettypes(product, eventType);
    });

    // enable/disable Trans Date
    self.DOMElement.$chkTransDateRange.click(function () {
        if ($(this).prop('checked')) {
            // fill in current date
            var today = new Date();
            $('.txtCalendar').removeAttr('disabled');
            self.SetupCalendar(today.format('Y/m/d H:i:s'), '.txtCalendar');

        } else {
            // clear text box
            $('.txtCalendar').val('').attr('disabled', 'disabled');
        }
    });

    // choose another product
    self.DOMElement.$product.click(function () {
        var product = $(this).val();
        var eventType = self.DOMElement.$ddlEventType.val();
        self.ResetOptionFilter();

        if (product === 'Sportsbook') {
            // show Event Type drop down
            self.DOMElement.$eventTypeWrp.show();

            if (eventType == 1) {
                self.DOMElement.$optFilters.show();
            } else {
                self.DOMElement.$optFilters.hide();
            }
        } else {
            // hide Live Type drop down                
            self.DOMElement.$eventTypeWrp.hide();
            self.DOMElement.$optFilters.hide();
        }


        NapTicketControl.Helper.GetBettypes(self.CollectSelectedProduct(), 0, true);
    });

    // check all bettypes
    $('body').delegate('#chkallbettype', 'click', function () {
        if ($('#chkallbettype').prop('checked')) {
            $('.chkbettype').attr('checked', 'checked')
        } else {
            $('.chkbettype').removeAttr('checked');
        }
    });

    $('body').delegate('.chkbettype', 'click', function () {
        var selectedItems = $('.tbl-bettype tbody input:checked').length;
        var totalItems = $('.tbl-bettype tbody input:checkbox').length;

        if (selectedItems === totalItems) {
            $('#chkallbettype').attr('checked', 'checked');
        } else {
            $('#chkallbettype').removeAttr('checked');
        }
    });

    // submit data to server
    $('.submit').click(function () {
        $('#msg').hide();
        var matchID = $("#MatchID").val();


        if (matchID.length > 0) {

            var url = $("#TicketListingForm").attr('action');
            self.HideContainers();

            // should save old params
            var type = self.CollectSelectedProduct();
            var bettypes = self.CollectSelectedBettypes();
            var livebettypes = self.CollectSelectedLiveBettypes();

            var dataReq = {
                MatchID: $('#MatchID').val(),
                LicenseeID: $('#LicenseeID').val(),
                LiveType: $('#LiveType').val(),
                SAFilter: $('#SAFilter').val(),
                LiveHomeScore: $('#LiveHomeScore').val(),
                LiveAwayScore: $('#LiveAwayScore').val(),
                FromTransDate: $('#FromTransDate').val(),
                ToTransDate: $('#ToTransDate').val(),
                Type: type,

                BetTypes: bettypes,
                BetTypesLive: livebettypes
            };

            if (!dataReq.BetTypesLive || dataReq.BetTypesLive.length === 0) {
                delete dataReq.BetTypesLive;
            }

            if (!dataReq.BetTypes || dataReq.BetTypes.length === 0) {
                delete dataReq.BetTypes;
            }
            // save old data request, to serve for follow-up requests
            self.TicketListingDataReq = dataReq;

            $.ajax({
                type: "POST",
                url: url,
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                data: JSON.stringify(dataReq),
                beforeSend: function () {
                    NapTicketControl.disableInput();
                },
                success: function (ticketlisting) {
                    // check result to see any error from server
                    if (ticketlisting.Result.ErrorCode !== 0) {
                        $('#msg').html(tooltip.show('MatchID', ticketlisting.Result.ErrorMsg)).show();
                    } else {
                        self.MatchInfo = ticketlisting.MatchInfo;
                        delete self.MatchInfo.MatchId;

                        // render result
                        self.RenderMatchInfo(ticketlisting.MatchInfo);
                        self.RenderTickets(ticketlisting.Items);
                    }
                },
                error: function () {
//                    console.log('has error :(');
                },
                complete: function () {
                    var height = self.DOMElement.$ticketsContainer.height();
                    $(window).scrollTop(height);
                    NapTicketControl.enableInput();
                }
            });
        } else {
            $('#msg').html(tooltip.show('MatchID', "MatchID is required")).show();
        }
        return false;
    });

    // handle request  to get ticket detail
    $('body').delegate('.lnk-totaltrans', 'click', function () {
        var $this = $(this);
        var bettypeId = $(this).attr('bettypeid');
        var bettypeName = $(this).attr('bettypename');
        var liveIndicator = $(this).attr('liveindicator');
        var params = {};
        params.BetTypeName = bettypeName;
        params.PageIndex = "0";
        params.PageSize = "50";

        // Selected Product is 'Number Game'
        if (self.TicketListingDataReq.Type === 'NumberGame') {
            if (liveIndicator === 'true') // is Live Ball
            {
                params.BetTypes = null;
                params.BetTypesLive = bettypeId;
            } else { // is Dead Ball
                params.BetTypes = bettypeId;
                params.BetTypesLive = null;
            }
        } else {
            params.BetTypes = bettypeId;
        }

        // build dataReq to get ticket details
        var dataReq = {};
        $.extend(true, dataReq, self.TicketListingDataReq, params);
        if (!dataReq.BetTypesLive || dataReq.BetTypesLive.length === 0) {
            delete dataReq.BetTypesLive;
        }

        if (!dataReq.BetTypes || dataReq.BetTypes.length === 0) {
            delete dataReq.BetTypes;
        }

        self.BetListDataRequest = dataReq;
        self.RequestBetList(dataReq);
    });

    $('body').delegate('.lnk-excel', 'click', function () {
        var $this = $(this);
        var bettypeId = $(this).attr('bettypeid');
        var bettypeName = $(this).attr('bettypename');
        var liveIndicator = $(this).attr('liveindicator');

        var params = {};
        params.BetTypeName = bettypeName;

        // Selected Product is 'Number Game'
        if (self.TicketListingDataReq.Type === 'NumberGame') {
            if (liveIndicator === 'true') // is Live Ball
            {
                params.BetTypes = null;
                params.BetTypesLive = bettypeId;
            } else { // is Dead Ball
                params.BetTypes = bettypeId;
                params.BetTypesLive = null;
            }
        } else {
            params.BetTypes = bettypeId;
        }
        // merge with match info
        $.extend(true, params, self.MatchInfo);

        // build dataReq to get ticket details

        var dataReq = {};
        $.extend(true, dataReq, self.TicketListingDataReq, params);

        if (!dataReq.BetTypesLive || dataReq.BetTypesLive.length === 0) {
            delete dataReq.BetTypesLive;
        }

        if (!dataReq.BetTypes || dataReq.BetTypes.length === 0) {
            delete dataReq.BetTypes;
        }

        self.ExportDataRequest = dataReq;
        self.ExportReport(dataReq);
    });
};

TicketListing.prototype.HideContainers = function () {
    var self = this;

    self.DOMElement.$msg.html('');
    self.DOMElement.$matchInfoContainer.parent().addClass('display-none');
    self.DOMElement.$ticketsContainer.parent().addClass('display-none');
    self.DOMElement.$betlistContainer.parent().addClass('display-none');
}

TicketListing.prototype.ResetOptionFilter = function () {
    $('#LiveHomeScore').val('');
    $('#LiveAwayScore').val('');
    $('.chkTransDateRange').removeAttr('checked');
    $('#FromTransDate').val('');
    $('#ToTransDate').val('');
}

TicketListing.prototype.CollectSelectedProduct = function(){
    return $('input[name=Product]:checked').val();
}

TicketListing.prototype.CollectSelectedBettypes = function(){
    var bettypes = '';

    $('input[name=BetTypes]:checked').each(function(){
        bettypes += $(this).val() + ',';
    });
//    console.log(bettypes);
    return bettypes.substring(0, bettypes.length - 1);
}

TicketListing.prototype.CollectSelectedLiveBettypes = function(){
    var livebettypes = '';
    
    $('input[name=LiveBetTypes]:checked').each(function(){
        livebettypes += $(this).val() + ',';
    })
//    console.log(livebettypes);
    return livebettypes.substring(0, livebettypes.length - 1);
}

TicketListing.prototype.SetupCalendar = function (strdatetime, txtdate) {
    $(txtdate).datetimepicker({
        showSecond: true,
        dateFormat: 'yy/m/d',
        timeFormat: 'HH:mm:ss',
        hourGrid: 3,
        minuteGrid: 5,
        secondGrid: 5,
        separator: ' '
    });

    if (strdatetime != null && strdatetime != "") {
        $(txtdate).val(strdatetime).datetimepicker('setDate', (new Date(strdatetime)));
    }
}

TicketListing.prototype.RenderMatchInfo = function (matchInfo) {
    var self = this;

    var gridOptions = {
        columns: [
            { HeaderText: "CODE", Name: "code", Width: 80, HeaderAlign: "center", CellAlign: "center" },
            { HeaderText: "MATCH EVENT", Name: "matchEvent", Width: 300, HeaderAlign: "center", CellAlign: "center" },
            { HeaderText: "EVENT DATE", Name: "eventDate", Width: 80, HeaderAlign: "center", CellAlign: "center" },
        ],
        bodyRows: self.GenerateMatchInfoData(matchInfo)
    };

    self.DOMElement.$matchInfoContainer.fanexGrid(gridOptions);
};

TicketListing.prototype.GenerateMatchInfoData = function(matchInfo){
    var rows = [];
    rows.push({
        Columns: [{ Name: "code", Value: matchInfo.MatchCode, CssClass: 'bold' },
            { Name: "matchEvent", Value: (matchInfo.HomeName + ' -vs- ' + matchInfo.AwayName), CssClass: 'bold' },
            { Name: "eventDate", Value: matchInfo.EventDate, CssClass: 'bold' }
    ]});

    return rows;
};

TicketListing.prototype.RenderTickets = function (tickets) {
    var self = this;

    var gridOptions = {
        columns: [
            { HeaderText: "BET TYPES", Name: "bettypes", Width: 200, HeaderAlign: "center", CellAlign: "left", ShowCollapseExpand: true },
            { HeaderText: "TOTAL BETS", Name: "totalBets", Width: 80, HeaderAlign: "center", CellAlign: "right" },
            { HeaderText: "EXPORT", Name: "export", Width: 80, HeaderAlign: "center", CellAlign: "center" },
        ],
        bodyRows: self.GenerateTicketsData(tickets),
        showTree: true
    };
    self.DOMElement.$ticketsContainer.fanexGrid(gridOptions);
};

TicketListing.prototype.GenerateTicketsData = function(tickets){
    var self = this;
    var rows = [];

    $.each(tickets, function(idx, ticket) {
        var currentTicket = {
            Columns: [
                    { Name: "bettypes", Value: ticket.BetTypeName, CssClass: 'bold' },
                    { Name: "totalBets", Value: ticket, CssClass: 'bold' },
                    { Name: "export", Value: ticket }
                ]
        };

        currentTicket.Columns[1].CustomCellRender = function(cell){
            return TicketListing.prototype.RenderTotalTransCell(cell);
        };

        currentTicket.Columns[2].CustomCellRender = function(cell){
            return TicketListing.prototype.RenderExportCell(cell);
        }

        // add additional attributes
        currentTicket.Columns[1].CustomAttr = 'bettypeID="' + ticket.BetTypeID + '"';
        rows.push(currentTicket);
    });

    return rows;
};

TicketListing.prototype.RenderTotalTransCell = function(cell){
    return '<a href="javascript:void(0);" class="lnk-totaltrans" ' 
    + 'bettypeID="' + cell.Value.BetTypeID + '" bettypeName="' + cell.Value.BetTypeName
    + '" liveIndicator="' + cell.Value.LiveIndicator + '" >'
    + cell.Value.TotalTrans + '</a>';
}

TicketListing.prototype.RenderExportCell = function(cell){
    return '<a href="javascript:;" class="lnk-excel" '
    + 'bettypeID="' + cell.Value.BetTypeID + '" bettypeName="' + cell.Value.BetTypeName
    + '" liveIndicator="' + cell.Value.LiveIndicator + '" >'
    + '<i class="icon-excel"></i></a>';
}

// Detail Betlist for TicketListing
TicketListing.prototype.RequestBetList = function (dataReq) {
    var self = this;

    $.ajax({
        type: "POST",
        url: window.rootUrl + "Misc/TicketListing/GetTicketListingBetList",
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        data: JSON.stringify(dataReq),
        beforeSend: function () {
            NapTicketControl.disableInput();
        },
        success: function (betlist) {
            // check result to see any error from server
            self.RenderTicketListingBetlist(betlist, dataReq.MatchID, dataReq.BetTypeName, dataReq.PageIndex, dataReq.PageSize);
        },
        error: function () {
//            console.log('has error :(');
        },
        complete: function () {
            $(window).scrollTop($(document).height());
            NapTicketControl.enableInput();
        }
    });
}

TicketListing.prototype.RenderTicketListingBetlist = function (betlist, matchId, bettypeName, pageIndex, pageSize) {
    var self = this;

    var gridOptions = {

        titleInfo: {
            text: "Tickets Listing for - Match ID: " + matchId + " - Bet type: " + bettypeName,
            showExcelButton: true,
            exportExcelFunction: function () {
                var dataReq = {};
                $.extend(true, dataReq, self.BetListDataRequest, self.MatchInfo);
                self.ExportReport(dataReq);
            },
            showBackButton: true,
            backFunction: function () {
                var parent = self.DOMElement.$betlistContainer.parent();
                $(parent).addClass("display-none");
            }
        },
        showTitle: true,
        columns: [
            { HeaderText: "BetID", Name: "betId", Width: 80, HeaderAlign: "center", CellAlign: "center" },
            { HeaderText: "Trans.Date", Name: "transDate", Width: 100, HeaderAlign: "center", CellAlign: "left" },
            { HeaderText: "Member", Name: "member", Width: 130, HeaderAlign: "center", CellAlign: "left" },
            { HeaderText: "Choice", Name: "choice", Width: 270, HeaderAlign: "center", CellAlign: "right" },
            { HeaderText: "Odds", Name: "odds", Width: 80, HeaderAlign: "center", CellAlign: "right" },
            { HeaderText: "Stake", Name: "stake", Width: 80, HeaderAlign: "center", CellAlign: "right", FormatNumber: "0,0.00" },
            { HeaderText: "Winloss", Name: "winloss", Width: 80, HeaderAlign: "center", CellAlign: "right" },
            { HeaderText: "Currency", Name: "currency", Width: 80, HeaderAlign: "center", CellAlign: "center" },
            { HeaderText: "BetType", Name: "bettype", Width: 80, HeaderAlign: "center", CellAlign: "center" },
            { HeaderText: "Status", Name: "status", Width: 80, HeaderAlign: "center", CellAlign: "center" },
            { HeaderText: "S.A", Name: "sa", Width: 40, HeaderAlign: "center", CellAlign: "center" },
        ],
        bodyRows: self.GenerateBetListData(betlist.Items),
        showPager: true,
        customPager: true,
        pagerOption: {
            itemsPerPage: pageSize,
            currentPage: pageIndex,
            totalItem: betlist.TotalItem,
            itemsPerPageArray: [50, 100],
            pagerSelectCallBack: function (pageIndex, pageSize, jq) {
                self.RenderPagingEvent(pageIndex, pageSize, jq);
            }
        }
    };

    self.DOMElement.$betlistContainer.fanexGrid(gridOptions);
}

TicketListing.prototype.GenerateBetListData = function(betlist){
    var self = this;
    var rows = [];

    $.each(betlist, function(idx, bet) {
        var currentBet = {
            Columns: [
                    { Name: "betId", Value: bet.BetId },
                    { Name: "transDate", Value: bet.TransDate },
                    { Name: "member", Value: bet.Member },
                    { Name: "choice", Value: bet.ChoiceHtml },
                    { Name: "odds", Value: bet.OddHtml },
                    { Name: "stake", Value: bet.Stake },
                    { Name: "winloss", Value: bet.AgentWinloss },
                    { Name: "currency", Value: bet.Currency },
                    { Name: "bettype", Value: bet},
                    { Name: "status", Value: bet.Status },
                    { Name: "sa", Value: bet.SAFlag }
                ]
        };

//        currentBet.Columns[3].CustomCellRender = function(cell){
//            return TicketListing.prototype.RenderChoiceCell(cell);
//        };
        
        currentBet.Columns[6].CustomCellRender = function(cell){
            return TicketListing.prototype.RenderWinlossCell(cell);
        };

        currentBet.Columns[8].CustomCellRender = function (cell) {
            return TicketListing.prototype.RenderBetTypeCell(cell);
        };

        currentBet.Columns[10].CustomCellRender = function (cell) {
            return TicketListing.prototype.RenderSACell(cell);
        };

        rows.push(currentBet);
    });

    return rows;
};

TicketListing.prototype.RenderChoiceCell = function(cell){
    var html = '';
    var isMixParlay = cell.Value.IsMixParlay;
    if(cell.Value.SAFlag === 'false')
    {
        html += '<span class="bl-choice b">' + (isMixParlay ? cell.Value.ParlaySportEvent.Choice : cell.Value.SportEvent.Choice) + '</span>'
            + '<span class="b"> ' + (isMixParlay ? cell.Value.ParlaySportEvent.Hdp : cell.Value.SportEvent.Hdp) + '</span>'
            + '<span class="bl-choice b"> '
            + (cell.Value.Ticket.IsLive ? (isMixParlay ? cell.Value.ParlaySportEvent.LiveScore : cell.Value.SportEvent.LiveScore) : '') + '</span>'
            + '<span class="b"> ' + (isMixParlay ? ('@' + cell.Value.ParlayTicket.Odds) : '') + '</span>'
            + (isMixParlay ? ('<br><span class="bl_bettypem">' + cell.Value.ParlaySportEvent.BetType + '</span>') : '');
    }
    else {
        html += '<span class="bl-choice b">' + (isMixParlay ? cell.Value.ParlaySportEvent.Choice : cell.Value.SportEvent.Choice) + '</span>'
            + '<span class="b"> ' + (isMixParlay ? cell.Value.ParlaySportEvent.Hdp : cell.Value.SportEvent.Hdp) + '</span>'
            + '<span class="bl-choice b"> '
            + (cell.Value.Ticket.IsLive ? (isMixParlay ? cell.Value.ParlaySportEvent.LiveScore : cell.Value.SportEvent.LiveScore) : '') + '</span>'
            + '<span class="b"> ' + (isMixParlay ? ('@' + cell.Value.ParlayTicket.Odds) : '') + '</span>'
            + (isMixParlay ? ('<br><span class="bl_bettypem">' + cell.Value.ParlaySportEvent.BetType + '</span>') : '');
    }

    return html;
}

TicketListing.prototype.RenderWinlossCell = function (cell) {

    var html = '';

    if (cell.Value.length > 0) {
        var value = parseFloat(cell.Value);

        if (value < 0) {
            html += '<span style="color: Red;">' + numeral(cell.Value).format('0,0.00'); +'</span>';
        } else {
            html += '<span style="color: Black;">' + numeral(cell.Value).format('0,0.00'); cell.Value + '</span>';
        }
    }

    return html;
}

TicketListing.prototype.RenderBetTypeCell = function (cell) {
    var html = '';
    var bettypeId = cell.Value.BetTypeId;

    if (bettypeId === 29) // ie. System Parlay
    {
        html += "<span style='color:blue' >" + cell.Value.BetType
            + " - <span style='color: gray' >" + cell.Value.Selection + "</span></span><br />";
    }
    else {
        html += cell.Value.BetType;
    }

    return html;
}

TicketListing.prototype.RenderSACell = function (cell) {

    var html = '';
    if(cell.Value === true){
        html += '<span class="icon-tick"></span>'
    }

    return html;
}

TicketListing.prototype.RenderPagingEvent = function (pageIndex, pageSize, jq) {
    var self = this;

    if (self.BetListDataRequest !== null) {
        //  send request to get data
        // modify paging params
        var dataReq = {};
        var pagingParams = {
            PageIndex: pageIndex,
            PageSize: pageSize
        };

        $.extend(true, dataReq, self.BetListDataRequest, pagingParams);
        self.RequestBetList(dataReq);
    }

    return false;
}

// Export
TicketListing.prototype.ExportReport = function (dataReq) {
    NapTicketControl.downloadFile(window.rootUrl + "Misc/TicketListing/ExportTicketListingBetList", dataReq);    
}

TicketListing.prototype.ToggleFilterBar = function (self) {
    // here, self is .toggle-filter-bar DOM element
    var iEle = $(self).find('i');

    if (iEle.hasClass('icon-circle-arrow-up')) {
        iEle.removeClass('icon-circle-arrow-up');
        iEle.addClass('icon-circle-arrow-down');
        $('.filter-bar-inner').hide();
        $(self).attr('title', 'Expand filter');

    } else {
        iEle.removeClass('icon-circle-arrow-down');
        iEle.addClass('icon-circle-arrow-up');
        $('.filter-bar-inner').show();
        $(self).attr('title', 'Collapse filter');
    }
}