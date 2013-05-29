var jOldVerion = parseFloat($.fn.jquery.substring(0, 3)) <= 1.5 ? true : false;

(function ($) {

    $.fn.gridScroller = function (opts) {

        return this.each(function () {
            var o = $(this);
            var xscrollbar = o.find(".x-scrollbar");
            var yscrollbar = o.find(".y-scrollbar");
            var container = o.find(".container");
            var rowheader = o.find(".row_header");
            var options = $.extend({}, $.fn.renderScroller.defaults, opts);

            $(window).resize(function () {
                o.resetGrid(options.tblContentId);
                var opt = $.extend({}, {}, options);
                opt.manualCopyHeader = true;
                o.renderScroller(opt);
                o.find('.grid-fixed-table').append(' ');
            });
            o.renderScroller(options);
            o.find('.grid-fixed-table').append(' ');
            return new scroller(o, container, xscrollbar, yscrollbar);
        });
       
    };

    $.fn.resetGrid = function () {
        return this.each(function (id) {
            var o = $(this);
            var xscrollbar = o.find(".x-scrollbar");
            var container = o.find(".container");
            var tblContent = $("#" + id);
            var colHeaderContainer = o.find(".col_header");
            var colFooterContainer = o.find(".col_footer");
            
            container.css("width", "");
            o.css("width", "");
            o.find(".content-container").css("width","")
            o.find(".footer-container").css("width", "");
            o.find(".scrollbar-x-content").css("width", "");
            o.find(".free-cell-bottom-left").css("width", "");
            xscrollbar.css("width", "");
            colHeaderContainer.css("width", "");
            colFooterContainer.css("width", "");
            colHeaderContainer.find(".header-container").css("width",  "");
            
        });
    };

    $.fn.renderScroller = function (options) {
        return this.each(function () {

            var bar = 17; 
            var o=$(this);
            var title=o.find(".grid-title");
            var titleHeight= title.length>0 ? title.height():0;
            var pager= o.find(".grid-pager");
            var pagerHeight = pager.length>0 ? pager.height():0;
          
            var xscrollbar = o.find(".x-scrollbar");
            var yscrollbar = o.find(".y-scrollbar");
            var container = o.find(".container");
            var tblContent=o.find("#" + options.tblContentId);
         
            var tblWidth=Math.max(tblContent.width(),options.gridWidth);
            var tblHeight=tblContent.height();
        
            var freeCellTop= o.find(".free-top-right");
            var freeCellBottom= o.find(".free-bottom-right"); 

            var tblWidth = Math.max(tblContent.width(), options.gridWidth);
        

            var freeCellTop = o.find(".free-top-right");
            var freeCellBottom = o.find(".free-bottom-right");

            var colHeaderContainer = o.find(".col_header");
            var rowHeaderContainer = o.find(".row_header");
            var rowHeaderWarpper = o.find(".row_header_wrapper");
            var colFooterContainer = o.find(".col_footer");
            var rowFooterContainer = o.find(".row_footer");
         
            var freezeWidth = options.freezeWidth;
            var numOfFreezeCol = options.numberFreezeCol;
            var freezeHeight = tblContent.find("thead").height();
           
            var freezeFooterHeight =colFooterContainer.length > 0 ? colFooterContainer.find("thead").height():0;

            var freezeFooterHeight = colFooterContainer.length > 0 ? colFooterContainer.find("thead").height() : 0;

            var gridWidth = options.fullWidth ? (($(window).width() < options.minWidth ? options.minWidth : $(window).width()) - options.marginRight) : options.gridWidth;
            var isScrollX = gridWidth < tblWidth;
            gridWidth = isScrollX ? gridWidth : (tblWidth + bar);

            var gridHeight = options.fullHeight ? ($(window).height() - options.gridHeight  ) : options.gridHeight;
            
            var isScrollY = gridHeight - titleHeight - pagerHeight - bar - freezeFooterHeight < tblHeight ;
            gridHeight = isScrollY ? gridHeight  : tblHeight + titleHeight + pagerHeight + bar + freezeFooterHeight;
            var containerHeight = gridHeight - titleHeight -freezeFooterHeight - pagerHeight - bar; 
            
            if (!options.manualCopyHeader) {
                var thead = $("<thead></thead>");
                var tbody = $("<tbody></tbody>");

                var theadFreezeCell = $("<thead></thead>");
                var i, j;
                var tr, cols;
                var rows = tblContent.find("thead tr");
                for (i = 0; i < rows.length; i++) {
                    var className = $(rows[i]).attr("class");
                    cols = $(rows[i]).find("th").clone();
                    tr = $(className !== undefined ? "<tr class='" + className + "'></tr>" : "<tr></tr>");
                    for (j = 0; j < numOfFreezeCol; j++) {
                        tr.append(cols[j]);
                    }
                    thead.append(tr.clone());
                    theadFreezeCell.append(tr);
                }

                o.find(".row_header_content").empty().append(thead);
                o.find(".free-cell-top-left").empty().append(theadFreezeCell);

                rows = tblContent.find("tbody tr");
                for (i = 0; i < rows.length; i++) {
                    var className = $(rows[i]).attr("class");
                    cols = $(rows[i]).find("td").clone();
                    tr = $(className !== undefined ? "<tr class='" + className + "'></tr>" : "<tr></tr>");
                    for (j = 0; j < numOfFreezeCol; j++) {
                        tr.append(cols[j]);
                    }
                    tbody.append(tr);
                }
                o.find(".row_header_content").append(tbody);

                rows = tblContent.find("thead tr");
                thead = $("<thead></thead>");
                for (i = 0; i < rows.length; i++) {
                    var className = $(rows[i]).attr("class");
                    tr = $(className !== undefined ? "<tr class='" + className + "'></tr>" : "<tr></tr>");
                    cols = $(rows[i]).find("th").clone();
                    for (j = 0; j < cols.length; j++) {
                        tr.append(cols[j]);
                    }
                    thead.append(tr);
                }
                o.find(".col_header_content").empty().append(thead);
            }
          
            container.css({"width": gridWidth + "px", "height": containerHeight  + "px"}); 
            o.css({"width": gridWidth + "px", "height": (gridHeight - (isScrollX ? -1: bar ) ) + "px"});
            o.find(".content-container").css("width",tblWidth + "px")
            o.find(".footer-container").css("width", tblWidth + "px");
            o.find(".scrollbar-x-content").css("width", (tblWidth - freezeWidth) + "px");
            o.find(".free-cell-bottom-left").css("width", freezeWidth + "px");
            pager.css("width", gridWidth + "px");
            rowHeaderContainer.css({"width":(freezeWidth +5 + 17) + "px"});
            rowHeaderWarpper.css({"width":(freezeWidth + 5) + "px"});
            
            var containerHeight = gridHeight - titleHeight -freezeFooterHeight - pagerHeight - bar;
            o.css("height", (gridHeight - (isScrollX ? -1: bar ) ) + "px");
            rowHeaderContainer.css("height", containerHeight  + "px");
            container.css("height", containerHeight  + "px");
            o.find(".free-cell-top-right").css("height", freezeHeight  - ($.browser.msie ? (options.padding*2 +1):0) + "px");
            o.find(".free-cell-bottom-right").css("height", (freezeFooterHeight + bar + 1) + "px");
            o.find(".scrollbar-y-content").css("height", (tblHeight - freezeHeight)  + "px");
            o.find(".free-bottom-left").css("top", (gridHeight- pagerHeight - bar ) + "px");
            
            freeCellBottom.css({"top":(gridHeight- pagerHeight - freezeFooterHeight - bar ) + "px", "left":(gridWidth - bar -1) + "px"});
            freeCellTop.css("left", (gridWidth - bar - 1) + "px");
            colHeaderContainer.css("width", gridWidth + "px");
            colHeaderContainer.find(".header-container").css("width", tblWidth + "px");
            colFooterContainer.css("top",(gridHeight - pagerHeight - freezeFooterHeight - bar ) + "px").css("width", gridWidth + "px");
            rowHeaderContainer.css("height", containerHeight  + "px");
            rowFooterContainer.css("top",(gridHeight - pagerHeight - freezeFooterHeight -bar ) + "px");
            
            yscrollbar.css({"height":(containerHeight - freezeHeight - ((isScrollY && $.browser.msie) ? 1:0)) + "px", "top": (freezeHeight + titleHeight +  ((isScrollY && $.browser.msie) ? 1:0)) + "px", "left": (gridWidth - bar - 1) + "px"});
            xscrollbar.css({"width":(gridWidth - freezeWidth- bar - options.padding*2) + "px", "top":(gridHeight - pagerHeight - bar) + "px", "left": (freezeWidth + options.padding*2)  + "px"});
            pager.css({"width":gridWidth + "px","top": (gridHeight - pagerHeight - (isScrollX ? -1 : bar )) + "px"});

            if (isScrollY) {
                yscrollbar.css("overflow-y", "scroll");
            }
            else {
                yscrollbar.css("overflow-y", "hidden");
            }

            if (isScrollX) {
                xscrollbar.css("overflow-x", "scroll");
            }
            else {
                xscrollbar.css("overflow-x", "hidden");
            }
        });
    };

     $.fn.renderScroller.defaults = {
        tblContentId:'grid1',
        manualCopyHeader:false,
        fullWidth:true,
        fullHeight:true,
		gridWidth: 0,//=totalColumnWidth + totalColumn*4 +totalColumn
        minWidth:600,
        gridHeight:300,
        freezeWidth:241,//=totalColumnWidth + (totalColumn-1)*4 + (totalColumn-1)
        numberFreezeCol:2,
        padding:2,
        marginRight:6
	    };

    var scroller = function (obj, container, rowheader, xscrollbar, yscrollbar) {
        this.preLeft = xscrollbar.scrollLeft();
        this.preTop = yscrollbar.scrollTop();
        this.colHeaderContainer = $(obj).find(".col_header");
        this.rowHeaderContainer = $(obj).find(".row_header");
        this.colFooterContainer = $(obj).find(".col_footer");
        this.initialize(container,rowheader, xscrollbar, yscrollbar);
    };

    scroller.prototype = {
        initialize: function (container, rowheader, xscrollbar, yscrollbar) {
            var me = this;

            yscrollbar.scroll(function () {
                var top = yscrollbar.scrollTop();
                if (me.preTop != top) {
                    if (jOldVerion) {
                        container.attr("scrollTop", top);
                        me.rowHeaderContainer.attr("scrollTop", top);
                        
                    }
                    else {
                        container.prop("scrollTop", top);
                        me.rowHeaderContainer.prop("scrollTop", top);
                    }
                    me.preTop = top;
                }
            });

            container.scroll(function () {
                var top = container.scrollTop();
                if (me.preTop != top) {
                    if (jOldVerion) {
                        me.rowHeaderContainer.attr("scrollTop", top);
                        yscrollbar.attr("scrollTop", top);
                        
                    }
                    else {
                        me.rowHeaderContainer.prop("scrollTop", top);
                        yscrollbar.prop("scrollTop", top);
                        
                    }
                    me.preTop = top;
                }
            });

            rowheader.scroll(function () {
                var top = rowheader.scrollTop();
                if (me.preTop != top) {
                    if (jOldVerion) {
                        container.attr("scrollTop", top);
                        yscrollbar.attr("scrollTop", top);
                    }
                    else {
                        container.prop("scrollTop", top);
                        yscrollbar.prop("scrollTop", top);
                    }
                    me.preTop = top;
                }
            });

            xscrollbar.scroll(function () {
                var left = xscrollbar.scrollLeft();
                if (me.preLeft != left) {
                    if (jOldVerion) {
                        me.colHeaderContainer.attr("scrollLeft", left);
                        me.colFooterContainer.attr("scrollLeft", left);
                        container.attr("scrollLeft", left);
                    }
                    else {
                        me.colHeaderContainer.prop("scrollLeft", left);
                        me.colFooterContainer.prop("scrollLeft", left);
                        container.prop("scrollLeft", left);
                    }
                    me.preLeft = left;
                }
            });
        }
    };
})(jQuery);
