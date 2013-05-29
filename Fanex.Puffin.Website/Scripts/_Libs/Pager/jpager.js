/**
* This jQuery plugin displays pagination links inside the selected elements.
* https://github.com/gbirke/jquery_pagination
* @author Gabriel Birke (birke *at* d-scribe *dot* de)
* @version 1.2
* @param {int} maxentries Number of entries to paginate
* @param {Object} opts Several options (see README for documentation)
* @return {Object} jQuery Object
*/
jQuery.fn.pagination = function (maxentries, opts) {
    opts = jQuery.extend({
        items_per_page: 20,
        start_per_page: 10,
        step_per_page: 10,
        item_per_page_arr: [],
        num_display_entries: 5,
        current_page: 0,
        num_edge_entries:3,
        link_to: "#",
        prev_text: "&laquo; ",
        next_text: "&raquo;",
        ellipse_text: "...",
        prev_show_always: true,
        next_show_always: true,
        max_items_per_page: 100,
        is_show_detail: true,

        callback: function () { return false; }
    }, opts || {});

    return this.each(function () {
        /**
        * Calculate the maximum number of pages
        */
        function numPages() {
            return Math.ceil(maxentries / opts.items_per_page);
        }

        /**
        * Calculate start and end point of pagination links depending on 
        * current_page and num_display_entries.
        * @return {Array}
        */
        function getInterval() {
            var ne_half = Math.floor(opts.num_display_entries / 2);
            var np = numPages();
            var upper_limit = np - opts.num_display_entries;
            var start = current_page > ne_half ? Math.max(Math.min(current_page - ne_half, upper_limit), 0) : 0;
            var end = current_page > ne_half ? Math.min(current_page + (ne_half +1), np) : Math.min(opts.num_display_entries, np);

            return [start, end];
        }

        /**
        * This is the event handling function for the pagination links. 
        * @param {int} page_id The new page number
        */
        function pageSelected(page_id, evt) {
            current_page = page_id;
            //drawLinks();
            var continuePropagation = opts.callback(page_id, opts.items_per_page, panel);
            if (!continuePropagation) {
                if (evt.stopPropagation) {
                    evt.stopPropagation();
                }
                else {
                    evt.cancelBubble = true;
                }
            }
            return continuePropagation;
        }

        /**
        * This function inserts the pagination links into the container element
        */
        function drawLinks() {
            panel.empty();
            var interval = getInterval();
            var np = numPages();
            // This helper function returns a handler function that calls pageSelected with the right page_id
            var getClickHandler = function (page_id) {
                return function (evt) { return pageSelected(page_id, evt); }
            }

            //Generate Items Per Page
            $('<div class="itemperpage">Page size </div>').appendTo(panel);
            var optPageSize = $('<select id="itemperpage"></select>').css("width", "50px");
            var totalItemPerPage = opts.item_per_page_arr.length;

            if (totalItemPerPage > 0) {
                for (var j = 0; j < totalItemPerPage; j++) {
                    var itemperpage = opts.item_per_page_arr[j];

                    if (itemperpage == opts.items_per_page) {
                        $('<option value="' + itemperpage + '" selected="true">' + itemperpage + '</option>').appendTo(optPageSize);
                    }
                    else {
                        $('<option value="' + itemperpage + '">' + itemperpage + '</option>').appendTo(optPageSize);
                    }
                }
            }
            else {
                for (var i = opts.start_per_page; i <= opts.max_items_per_page; i += opts.step_per_page) {
                    if (i == opts.items_per_page) {
                        $('<option value="' + i + '" selected="true">' + i + '</option>').appendTo(optPageSize);
                    }
                    else {
                        $('<option value="' + i + '">' + i + '</option>').appendTo(optPageSize);
                    }
                }
            }

            optPageSize.bind("change", function () {
                itemPerPage(this);
            });
            optPageSize.appendTo(panel);
            //            $('<span class="btnseparator"></span>').appendTo(panel);
            // Helper function for generating a single link (or a span tag if it's the current page)
            var numberList = $('<ul ></ul>');

            var appendItem = function (page_id, lastcss, appendopts) {
                page_id = page_id < 0 ? 0 : (page_id < np ? page_id : np - 1); // Normalize page id to sane value
                current_page = current_page < np ? current_page : np - 1;
                appendopts = jQuery.extend({ text: page_id + 1, classes: "" }, appendopts || {});
                if (page_id == current_page) {
                    var lnk = jQuery("<span class='current'>" + (appendopts.text) + "</span>");
                }
                else {
                    var lnk = jQuery("<a>" + (appendopts.text) + "</a>")
						.bind("click", getClickHandler(page_id))
						.attr('href', opts.link_to.replace(/__id__/, page_id));
                }
                if (appendopts.classes) { lnk.addClass(appendopts.classes); }
                $(numberList).append($('<li class="' + (appendopts.classes == "" ? 'sep ' : ' ') + lastcss + '"></li>').append(lnk));
            }
            // Generate "Previous"-Link
            if (opts.prev_text && (current_page > 0 || opts.prev_show_always)) {
                appendItem(current_page - 1, '', { text: opts.prev_text, classes: "prev" });
            }
            // Generate starting points
            if (interval[0] > 0 && opts.num_edge_entries > 0) {
                var end = Math.min(opts.num_edge_entries, interval[0]);
                for (var i = 0; i < end; i++) {
                    appendItem(i, ((i == (end - 1)) ? ' last' : ''));
                }
                if (opts.num_edge_entries < interval[0] && opts.ellipse_text) {
                    jQuery('<li><span class="lnk" title="Go To Page" onclick="openGotoBox();">' + opts.ellipse_text + '</span></li>').appendTo(numberList);
                }
            }
            // Generate interval links
            for (var i = interval[0]; i < interval[1]; i++) {
                appendItem(i, (i == (interval[1]-1) ? ' last' : ''));
            }
            // Generate ending points
            if (interval[1] < np && opts.num_edge_entries > 0) {
                if (np - opts.num_edge_entries > interval[1] && opts.ellipse_text) {
                    jQuery('<li><span class="lnk" title="Go To Page" onclick="openGotoBox();">' + opts.ellipse_text + '</span></li>').appendTo(numberList);
                }
                var begin = Math.max(np - opts.num_edge_entries, interval[1]);
                for (var i = begin; i < np; i++) {
                    appendItem(i, (i == (np - 1) ? ' last' : ''));
                }

            }
            // Generate "Next"-Link
            if (opts.next_text && (current_page < np - 1 || opts.next_show_always)) {
                appendItem(current_page + 1, '', { text: opts.next_text, classes: "next" });
            }
            var list = $('<div class="numberlist"></div>');
            $(list).append($(numberList)).appendTo(panel);

            // Generate Status
            if (opts.is_show_detail) {
                $('<div class="detailtext">Displaying <b>' + (opts.items_per_page * opts.current_page + 1) + '</b> to <b>' + Math.min(opts.items_per_page * (opts.current_page + 1), maxentries) + '</b> of <b>' + maxentries + '</b> item(s)</div>').appendTo(panel);
            }

            //GotoPage Pane
            $(drawGoToPage()).appendTo(panel);
            $("#pagenav_ibtn").bind("click", function () {
                if (isNumberNotNull("txtpage")) {
                    var curr_page = Math.min(parseInt($("#txtpage").val()) - 1, np - 1);
                    if (curr_page < 0) {
                        $("#txtpage").val(0);
                        curr_page = 0;
                    }

                    pageSelected(curr_page, this);
                }
            });
        }

        //check Numeric input field by id
        function isNumberNotNull(id) {
            var result = false;
            try {

                if (!jQuery.trim($("#" + id).val()) == "") {
                    result = !isNaN($("#" + id).val());
                }
            } catch (error) {
            }
            return result;
        }

        function drawGoToPage() {
            var goto_page = '<table class="c gotopage_box" id="page_menu" cellpadding="0px" cellspacing="0px"><tr><td>Go to Page</td><td><input type="text" name="txtpage" id="txtpage" class="txt" onkeypress="return PagerKeyPress(event);" size="4"></td><td><input type="button" id="pagenav_ibtn" class="pagenav_ibtn" ></input></td></tr></table>';
            return goto_page;
        }

        function itemPerPage(obj) {
            opts.items_per_page = $(obj).val();
            drawLinks();
            //alert(opts.current_page);
            opts.callback(opts.current_page, opts.items_per_page, this);
        }
        // Extract current_page from options
        var current_page = opts.current_page;

        // Create a sane value for maxentries and items_per_page
        maxentries = (!maxentries || maxentries < 0) ? 1 : maxentries;
        opts.items_per_page = (!opts.items_per_page || opts.items_per_page < 0) ? 1 : opts.items_per_page;
        // Store DOM element for easy access from all inner functions
        var panel = jQuery(this);
        // Attach control functions to the DOM element 
        this.selectPage = function (page_id) { pageSelected(page_id); }
        this.prevPage = function () {
            if (current_page > 0) {
                pageSelected(current_page - 1);
                return true;
            }
            else {
                return false;
            }
        }
        this.nextPage = function () {
            if (current_page < numPages() - 1) {
                pageSelected(current_page + 1);
                return true;
            }
            else {
                return false;
            }
        }
        // When all initialisation is done, draw the links
        drawLinks();
        // call callback function
        //opts.callback(current_page, opts.items_per_page, this);
    });
}

function openGotoBox() {
    var pagelist = $('#pagination .numberlist');
    var offset = pagelist.offset();
    var goto = $('.gotopage_box');
    goto.toggle();
    var w = pagelist.width();
    var gotow=goto.width();
    goto.css("left", (offset.left + (w - gotow) / 2 - 10) + "px");
    goto.css("top", "-25px");

}

function PagerKeyPress(e) {

    if (!e) e = window.event;
    var code = (e.keyCode) ? e.keyCode : e.which;

    if (code == 69 || code == 101 || code == 45 || code == 37 || code == 39 || code == 36 || code == 35) //e or E or "-"
        return true;
    if ((code < 48 || code > 57) && code != 8 && code != 13 && code != 0 && code != 46 && code != 9) {
        return false;
    }

    if (code == 13) {
        try {
            $("#pagenav_ibtn").click();
            return true;
        } catch (error)
        { return false; }
    }
}
