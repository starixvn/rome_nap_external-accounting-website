/* File Created: February 6, 2013 */

$(function () {
    var ticketListingManager = new TicketListingManager();
    ticketListingManager.Init();
});

function TicketListingManager() {
    this.ticketListing = new TicketListing();
}

TicketListingManager.prototype.Init = function () {
    var self = this;
    self.ticketListing.Init();
}